using System.Reflection;
using System.Text;
using SharpISOBMFF;

namespace SharpMP4.Tests.Conformance;

/// <summary>
/// Compares the boxes SharpMP4 reads out of a file with GPAC's dump of it: which boxes there are,
/// where, and how large. Within a parent, boxes are paired by type in the order they come, as the
/// dump keeps only that order.
/// </summary>
public static partial class BoxTreeComparison
{
    /// <summary>Reads a file with SharpMP4 into the same shape as a dump.</summary>
    public static (DumpedBox Root, Exception? Error) ReadWithSharpMp4(string path)
    {
        var root = new DumpedBox("file", null, []);
        var container = new Container();
        Exception? error = null;
        try
        {
            using var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 16);
            container.Read(new IsoStream(new StreamWrapper(file)));
        }
        catch (Exception ex)
        {
            // What was read before the failure is compared all the same.
            error = ex;
        }

        Add(WithinFile(container.Children, (ulong)new FileInfo(path).Length), root);
        return (root, error);
    }

    /// <summary>
    /// The top-level boxes that end within the file. GPAC leaves out one that runs past the end of
    /// a file cut short, as the last 'mdat' of green\video_2500000bps_0.mp4 does.
    /// </summary>
    private static List<Box> WithinFile(List<Box>? boxes, ulong length)
    {
        var within = new List<Box>();
        ulong end = 0;
        foreach (var box in boxes ?? [])
        {
            if (box.Header != null && box.Header.Size != 0)
                end += box.Header.GetBoxSizeInBits() >> 3;
            if (end > length)
                break;
            within.Add(box);
        }

        return within;
    }

    /// <summary>Boxes GPAC reads but leaves out of its dump, by the type of their parent.</summary>
    private static readonly HashSet<(string Parent, string Type)> NotDumped =
    [
        ("hinf", "maxr"),
    ];

    /// <summary>
    /// Boxes GPAC keeps only one of in a parent, dumping that one alone: a sample entry's 'colr'
    /// (nalu\hevc\hev1_clg1_header.mp4 has two) and a user data box's 'strk' (sg-tl-st.mp4 has two).
    /// </summary>
    private static readonly HashSet<string> OneKept = ["colr", "strk"];

    private static void Add(List<Box>? boxes, DumpedBox parent)
    {
        if (boxes == null)
            return;

        foreach (var box in boxes)
        {
            ulong? size = null;
            if (box.Header != null && box.Header.Size != 0)
                size = box.Header.GetBoxSizeInBits() >> 3;

            var node = new DumpedBox(TypeName(box.FourCC), size, []) { Source = box };
            parent.Children.Add(node);
            Add(BoxFields(box), node);
            Add(box.Children, node);
        }
    }

    /// <summary>A box type as GPAC writes it: a byte outside ASCII in hex, '©swr' as A9swr.</summary>
    internal static string TypeName(uint fourCC)
    {
        var name = new System.Text.StringBuilder();
        for (int shift = 24; shift >= 0; shift -= 8)
        {
            byte b = (byte)(fourCC >> shift);
            name.Append(b >= 0x80 || b < 0x20 ? b.ToString("X2") : ((char)b).ToString());
        }
        return name.ToString();
    }

    /// <summary>
    /// The boxes a box keeps in fields of its own - an iref's references, a meta's handler - which
    /// the syntax reads one by one rather than into its children.
    /// </summary>
    internal static List<Box> BoxFields(Box box)
    {
        var found = new List<Box>();
        for (var type = box.GetType(); type != null && type != typeof(Box); type = type.BaseType)
        {
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                object? value = field.GetValue(box);
                if (value is Box single && !ReferenceEquals(single, box))
                    found.Add(single);
                else if (value is IEnumerable<Box> many && field.Name != "children")
                    found.AddRange(many.Where(b => b != null));
            }
        }

        return found;
    }

    public static StreamResult Compare(string path, DumpedBox sharp, Exception? error, DumpedBox gpac)
    {
        var result = new StreamResult { Path = path };
        CompareChildren(sharp, gpac, "", result);

        if (error != null)
        {
            result.Fail(Outcome.SharpFailed, $"{error.GetType().Name}",
                $"threw: {error.GetType().Name}: {error.Message}");
        }

        if (result.Keys.Count == 0)
            result.Outcome = Outcome.Match;
        return result;
    }

    /// <summary>What GPAC dumps of every box, which is not a field of it: its place and its size, compared already.</summary>
    private static readonly HashSet<string> NotFields = new(StringComparer.Ordinal) { "Size", "Type", "Specification", "Container" };

    /// <summary>
    /// GPAC's fields of a box that SharpMP4 has none of the name of, by box type and name: what is compared
    /// no further, and so what to map next.
    /// </summary>
    public static System.Collections.Concurrent.ConcurrentDictionary<string, int> Unpaired { get; } = new();

    /// <summary>
    /// Compares the fields GPAC dumps of a box with SharpMP4's of the same name, the name taken without case and
    /// underscores (@CreationTime is creation_time): the value, as a number (in decimal, or in hex as 0x...), a
    /// 4CC, a truth value or a string, whichever the field is.
    /// </summary>
    private static void CompareFields(Box box, Dictionary<string, string> gpacFields, string at, StreamResult result)
    {
        var properties = Properties(box.GetType());
        foreach (var (name, gpacValue) in gpacFields)
        {
            if (NotFields.Contains(name))
                continue;

            // a tfhd's default sample flags, in their parts
            if (box is TrackFragmentHeaderBox fragmentHeader && SampleFlagsPart(0, name) != null)
            {
                CompareSampleFlags(fragmentHeader.DefaultSampleFlags, new() { [name] = gpacValue }, at, result);
                continue;
            }

            if (!TryGetProperty(box, properties, name, out var property))
            {
                // of a box SharpMP4 does not know, which one
                string owner = box is UnknownBox ? $"{box.GetType().Name} '{TypeName(box.FourCC)}'" : box.GetType().Name;
                Unpaired.AddOrUpdate($"{owner}.{name}", 1, (_, n) => n + 1);
                continue;
            }

            object? value;
            try
            {
                value = property.GetValue(box);
            }
            catch (Exception)
            {
                continue;
            }

            // what the box keeps as a place in the file, which is closed by now, is not compared
            if (value is StreamMarker)
                continue;

            result.FieldsCompared++;
            if (!Forms(value, box, name).Any(form => Same(gpacValue, form)))
                result.Fail(Outcome.Diverged, $"{at}.{name}: differs", $"{at}.{name}: GPAC {gpacValue}, SharpMP4 {Show(value)}");
        }
    }

    /// <summary>
    /// A value of SharpMP4's in the forms GPAC may write it in: as it is; a 16.16 or 8.8 fixed point number as the
    /// number it is (tkhd's width 11534336 is 176.00, its volume 256 is 1.00); an integer as the other signedness
    /// of its width (-1 as 65535); bytes in hex, as the integer they are, as text or as a counted text; a string
    /// as its counted text and in hex; and what GPAC writes of some boxes in words of its own.
    /// </summary>
    private static IEnumerable<object?> Forms(object? value, Box box, string name)
    {
        yield return value;
        switch (value)
        {
            case byte or ushort or uint or ulong or sbyte or short or int or long:
                decimal number = Convert.ToDecimal(value);
                yield return number / 65536m;
                yield return number / 256m;
                yield return value switch
                {
                    byte b => (sbyte)b,
                    sbyte s => (byte)s,
                    ushort u => (short)u,
                    short s => (ushort)s,
                    uint u => (int)u,
                    int s => (uint)s,
                    ulong u => (long)u,
                    long s => (ulong)s,
                    _ => value,
                };
                // GPAC's hex with no 0x: tx3g's displayFlags 262144 is 40000, subs's reserved 00000000
                yield return Convert.ToUInt64(number < 0 ? 0 : number).ToString("X");
                yield return Convert.ToUInt64(number < 0 ? 0 : number).ToString("X8");
                // and in lower case: an oinf's general_constraint_indicator_flags f8800000000
                yield return Convert.ToUInt64(number < 0 ? 0 : number).ToString("x");
                // an oinf's maxBitDepth, of maxBitDepthMinus8
                if (name == "maxBitDepth")
                    yield return number + 8;
                // a matrix's value, 0x00010000: of its bits, a negative one's too
                yield return "0x" + (value is int or short or sbyte or long ? unchecked((uint)Convert.ToInt64(value)) : Convert.ToUInt64(number)).ToString("X8");
                break;
            case byte[] bytes:
                yield return "0x" + Convert.ToHexString(bytes);
                // a parameter set's content, as a data URL; a decoder specific info's, its bytes URL-encoded
                yield return "data:application/octet-string," + Convert.ToHexString(bytes);
                yield return "data:application/octet-string," + string.Concat(bytes.Select(b => "%" + b.ToString("X2")));
                // a field of no width - an iloc's base_offset of base_offset_size 0 - is 0
                if (bytes.Length == 0)
                    yield return 0UL;
                // each byte in hex, as a colour: tx3g's backgroundColor ff 0 0 ff
                yield return string.Join(" ", bytes.Select(b => b.ToString("x")));
                // each byte a number, as GPAC lists them: an oinf layer's dependent_on_layerID 0
                yield return string.Concat(bytes.Select(b => $"{b} "));
                // a UUID, in GPAC's braces: {F78CAA0C-36BE4CE9-87D203C2-56DABEB2}
                if (bytes.Length == 16)
                    yield return "{" + string.Join("-", Enumerable.Range(0, 4).Select(i => Convert.ToHexString(bytes, i * 4, 4))) + "}";
                yield return Convert.ToHexString(bytes);
                if (bytes.Length is > 0 and <= 8)
                    yield return bytes.Aggregate(0UL, (v, b) => (v << 8) | b);
                yield return Encoding.UTF8.GetString(bytes).Split('\0')[0];
                if (bytes.Length > 0 && bytes[0] < bytes.Length)
                    yield return Encoding.UTF8.GetString(bytes, 1, bytes[0]);
                break;
            case BinaryUTF8String text:
                yield return text.Text;
                yield return text.ToString();
                if (text.Bytes != null)
                    yield return "0x" + Convert.ToHexString(text.Bytes);
                // its text in hex, without the zero it ends with: fpar's scheme_specific_info
                yield return "0x" + Convert.ToHexString(Encoding.UTF8.GetBytes(text.Text));
                break;
            case string text:
                yield return "0x" + Convert.ToHexString(Encoding.UTF8.GetBytes(text));
                break;
        }

        // an ISO-639-2/T code, three letters of 5 bits each, 0x60 added (14496-12 8.4.2.3), as GPAC's text
        if (name == "LanguageCode")
        {
            if (value is byte[] { Length: 3 } letters)
                yield return new string(letters.Select(l => (char)(l + 0x60)).ToArray());
            else if (value is ushort or short or uint)
            {
                uint packed = Convert.ToUInt32(value) & 0x7FFF;
                yield return new string([(char)(((packed >> 10) & 0x1F) + 0x60), (char)(((packed >> 5) & 0x1F) + 0x60), (char)((packed & 0x1F) + 0x60)]);
            }
            // GPAC's reading of what is not three letters (isomedia/box_code_base.c, mdhd_box_read): 0 as und; one
            // under 0x400 as a QuickTime language code, of which its u8 keeps the last 5 bits (0x0055 is 21, hin)
            if (value is string { Length: 3 } text && text.All(c => c >= 0x60 && c < 0x80))
            {
                int packed = ((text[0] - 0x60) << 10) | ((text[1] - 0x60) << 5) | (text[2] - 0x60);
                if (packed == 0)
                    yield return "und";
                else if (packed < 0x400)
                    yield return QuickTimeLanguages[packed & 0x1F];
            }
        }

        // a decoder configuration's chroma_format, in GPAC's words (0 to 3 of H.264's and H.265's chroma_format_idc)
        if (name == "chroma_format" && value is byte chroma && chroma < 4)
            yield return new[] { "YUV 4:0:0", "YUV 4:2:0", "YUV 4:2:2", "YUV 4:4:4" }[chroma];
        // an sdtp's two bits of a sample, in GPAC's words
        if (value is byte dependency && name is "isLeading" or "dependsOnOther" or "dependedOn" or "hasRedundancy" && dependency < 3)
            yield return new[] { "unknown", "yes", "no" }[dependency];
        // an sbgp's index in a traf: 0x10000 and up are the traf's own descriptions, which GPAC gives apart
        if (name == "group_description_index" && value is uint index && index > 0xFFFF)
            yield return index & 0xFFFF;

        // GPAC's own words of a few boxes' fields
        if (value is byte angle && name == "angle")
            yield return angle * 90;
        if (value is bool axis && name == "axis")
            yield return axis ? "horizontal" : "vertical";
        // no base_data_offset in the 'tfhd': the data are where its 'moof' is (default-base-is-moof), or where the
        // 'moof' or the previous 'traf's data are
        if (name == "BaseDataOffset" && value is ulong offset && offset == 0)
        {
            yield return "moof";
            yield return "moof-or-previous-traf";
        }
        // GPAC takes an 'iSLT' for a full box, which leaves 4 of its salt's 8 bytes: 0 (f2.mp4's are 1 and 2, in boxes of 16 bytes)
        if (name == "salt" && box is ISMACrypSaltBox)
            yield return 0UL;
        // an 'mdhd' of timescale 0, which GPAC reads as 90000 (isomedia/box_code_base.c, mdhd_box_read)
        if (name == "TimeScale" && box is MediaHeaderBox && value is uint timescale && timescale == 0)
            yield return 90000u;
    }

    /// <summary>
    /// GPAC's names of the fields of entries SharpMP4 names otherwise, by the box's class, the entry's element and
    /// GPAC's name; where more than one, separated by |, the first SharpMP4 has - an 'stsz' of sizes, or of one size.
    /// </summary>
    private static readonly Dictionary<string, string> GpacEntryNames = new(StringComparer.Ordinal)
    {
        ["SampleSizeBox.SampleSizeEntry.Size"] = "EntrySize|SampleSize",
        ["CompactSampleSizeBox.SampleSizeEntry.Size"] = "EntrySize",
        ["ChunkOffsetBox.ChunkEntry.offset"] = "ChunkOffset",
        ["ChunkLargeOffsetBox.ChunkOffsetEntry.offset"] = "ChunkOffset",
        ["CompositionOffsetBox.CompositionOffsetEntry.CompositionOffset"] = "SampleOffset|SampleOffset0",
        ["TrackRunBox.TrackRunEntry.Duration"] = "SampleDuration",
        ["TrackRunBox.TrackRunEntry.Size"] = "SampleSize",
        ["TrackRunBox.TrackRunEntry.CTSOffset"] = "SampleCompositionTimeOffset|SampleCompositionTimeOffset0",
        ["SampleAuxiliaryInformationSizesBox.SAISize.size"] = "SampleInfoSize|DefaultSampleInfoSize",
        ["FileTypeBox.BrandEntry.AlternateBrand"] = "CompatibleBrands",
        ["SegmentTypeBox.BrandEntry.AlternateBrand"] = "CompatibleBrands",
        ["SampleDependencyTypeBox.SampleDependencyEntry.dependsOnOther"] = "SampleDependsOn",
        ["SampleDependencyTypeBox.SampleDependencyEntry.dependedOn"] = "SampleIsDependedOn",
        ["SampleDependencyTypeBox.SampleDependencyEntry.hasRedundancy"] = "SampleHasRedundancy",
        ["EditListBox.EditListEntry.Duration"] = "EditDuration",
        ["EditListBox.EditListEntry.MediaRate"] = "MediaRateInteger",
        ["TrackFragmentRandomAccessBox.RandomAccessEntry.traf"] = "TrafNumber",
        ["TrackFragmentRandomAccessBox.RandomAccessEntry.trun"] = "TrunNumber",
        ["TrackFragmentRandomAccessBox.RandomAccessEntry.sample"] = "SampleDelta",
        ["SegmentIndexBox.Reference.type"] = "ReferenceType",
        ["SegmentIndexBox.Reference.size"] = "ReferencedSize",
        ["SegmentIndexBox.Reference.duration"] = "SubsegmentDuration",
        ["SegmentIndexBox.Reference.SAP_type"] = "SAPType",
        ["TrackReferenceTypeBox.TrackReferenceEntry.TrackID"] = "TrackIDs",
        ["SingleItemTypeReferenceBox.ItemReferenceBoxEntry.ItemID"] = "ToItemID",
        ["ShadowSyncSampleBox.SyncShadowEntry.ShadowedSample"] = "ShadowedSampleNumber",
        ["ShadowSyncSampleBox.SyncShadowEntry.SyncSample"] = "SyncSampleNumber",
        ["FontTableBox.FontRecord.ID"] = "FontId",
        ["FontTableBox.FontRecord.name"] = "FontName",
        ["LevelAssignmentBox.Assignement.assignement_type"] = "AssignmentType",
        ["SubTrackInformationBox.SubTrackInformationAttribute.value"] = "AttributeList",
    };

    /// <summary>The lists of entries GPAC gives that are SharpMP4's arrays, an entry an element: by the box's class and the entry's element.</summary>
    private static readonly HashSet<string> FlatEntries = new(StringComparer.Ordinal)
    {
        "SampleSizeBox.SampleSizeEntry", "CompactSampleSizeBox.SampleSizeEntry", "ChunkOffsetBox.ChunkEntry",
        "ChunkLargeOffsetBox.ChunkOffsetEntry", "CompositionOffsetBox.CompositionOffsetEntry", "PaddingBitsBox.PaddingBitsEntry",
        "TrackRunBox.TrackRunEntry", "TimeToSampleBox.TimeToSampleEntry", "SyncSampleBox.SyncSampleEntry",
        "SampleToGroupBox.SampleGroupBoxEntry", "SampleAuxiliaryInformationSizesBox.SAISize", "SampleToChunkBox.SampleToChunkEntry",
        "TrickPlayBox.TrickPlayBoxEntry", "FileTypeBox.BrandEntry", "SegmentTypeBox.BrandEntry",
        "SampleDependencyTypeBox.SampleDependencyEntry", "EditListBox.EditListEntry", "TrackFragmentRandomAccessBox.RandomAccessEntry",
        "SegmentIndexBox.Reference", "TrackReferenceTypeBox.TrackReferenceEntry", "SingleItemTypeReferenceBox.ItemReferenceBoxEntry",
        "SampleAuxiliaryInformationOffsetsBox.SAIChunkOffset", "FilePartitionBox.FilePartitionBoxEntry",
        "ShadowSyncSampleBox.SyncShadowEntry", "FontTableBox.FontRecord", "FECReservoirBox.FECReservoirBoxEntry",
        "LevelAssignmentBox.Assignement", "SubTrackInformationBox.SubTrackInformationAttribute",
    };

    /// <summary>
    /// Compares the entries GPAC gives of a box - each attribute of its i-th entry of a kind - with the i-th of the
    /// array SharpMP4 has of that name, or with the field of that name of the i-th of its entries where it keeps them
    /// as objects (a trun's TrunEntry). A padb's entries are its samples', two to SharpMP4's each. Attributes of
    /// entries SharpMP4 has none of the name of are counted with GPAC's fields it has none of.
    /// </summary>
    private static void CompareEntries(Box box, Dictionary<string, List<Dictionary<string, string>>> gpacEntries, string at, StreamResult result)
    {
        var properties = Properties(box.GetType());
        string boxClass = box.GetType().Name;

        // entries SharpMP4 keeps as objects: the one array of a class of its own
        var objects = properties.Values
            .Where(p => p.PropertyType.IsArray && p.PropertyType.GetElementType() is { IsClass: true } t && t != typeof(string) && !t.IsArray && !typeof(Box).IsAssignableFrom(t))
            .Select(p => Value(p, box) as Array).Where(a => a != null).ToList();
        Array? entryObjects = objects.Count == 1 ? objects[0] : null;

        foreach (var (element, entries) in gpacEntries)
        {
            // a trun's first sample's flags, a trex's default ones, in their parts
            if (element is "FirstSampleFlags" or "DefaultSampleFlags" && properties.TryGetValue(Normal(element), out var flagsProperty) && Value(flagsProperty, box) is uint boxFlags)
            {
                foreach (var entry in entries)
                    CompareSampleFlags(boxFlags, entry, $"{at}/{element}", result);
                continue;
            }

            // only flat lists, an entry each of SharpMP4's arrays' elements: lists in lists - iloc's extents, ipma's
            // properties, a decoder configuration's parameter sets - not yet
            if (!FlatEntries.Contains($"{boxClass}.{element}"))
            {
                Unpaired.AddOrUpdate($"{boxClass}.{element} (entries not compared)", entries.Count, (_, n) => n + entries.Count);
                continue;
            }

            var attributes = entries.SelectMany(e => e.Keys).Distinct().ToList();
            foreach (string attribute in attributes)
            {
                string key = $"{boxClass}.{element}.{attribute}";
                Func<int, (bool Found, object? Value)>? ours = null;

                // a trun's samples' own flags, in their parts
                if (entryObjects is TrunEntry[] runEntries && SampleFlagsPart(0, attribute) != null)
                {
                    for (int i = 0; i < Math.Min(entries.Count, runEntries.Length); i++)
                        CompareSampleFlags(runEntries[i].SampleFlags, entries[i].Where(e => e.Key == attribute).ToDictionary(), $"{at}/{element}[{i}]", result);
                    continue;
                }

                if (box is PaddingBitsBox padding && attribute == "PaddingBits")
                {
                    // two samples' padding a byte: pad1 of the first, pad2 of the second
                    ours = i => (true, i / 2 < (i % 2 == 0 ? padding.Pad1 : padding.Pad2)?.Length ? (i % 2 == 0 ? padding.Pad1 : padding.Pad2)![i / 2] : null);
                }
                else
                {
                    string[] names = GpacEntryNames.TryGetValue(key, out string? mapped) ? mapped.Split('|') : [attribute];
                    if (entryObjects != null)
                    {
                        var elementProperties = Properties(entryObjects.GetType().GetElementType()!);
                        var candidates = names.Select(n => elementProperties.GetValueOrDefault(Normal(n))).Where(p => p != null).Select(p => p!).ToList();
                        // of more than one - a version 0 trun's unsigned offset, a version 1's signed - the one that is set
                        if (candidates.Count > 0)
                            ours = i => i < entryObjects.Length && entryObjects.GetValue(i) is object entry
                                ? (true, candidates.Select(p => Value(p, entry)).FirstOrDefault(v => v != null && (!v.GetType().IsValueType || !Equals(v, Activator.CreateInstance(v.GetType())))) ?? Value(candidates[0], entry))
                                : (false, null);
                    }
                    if (ours == null)
                    {
                        // the first of the names SharpMP4 has a value of: an array, else one value for all entries
                        var property = names.Select(n => properties.GetValueOrDefault(Normal(n))).FirstOrDefault(p => p != null && Value(p, box) != null);
                        object? value = property != null ? Value(property, box) : null;
                        if (value is Array array && value is not byte[] { Length: 0 })
                            ours = i => i < array.Length ? (true, array.GetValue(i)) : (false, null);
                        else if (value != null)
                            ours = _ => (true, value);
                    }
                }

                if (ours == null)
                {
                    Unpaired.AddOrUpdate(key, entries.Count, (_, n) => n + entries.Count);
                    continue;
                }

                for (int i = 0; i < entries.Count; i++)
                {
                    if (!entries[i].TryGetValue(attribute, out string? gpacValue))
                        continue;
                    result.FieldsCompared++;
                    var (found, value) = ours(i);
                    if (!found)
                    {
                        result.Fail(Outcome.Diverged, $"{at}/{element}.{attribute}: missing", $"{at}/{element}[{i}].{attribute}: GPAC {gpacValue}, SharpMP4 has no entry {i}");
                        break;
                    }
                    if (!Forms(value, box, attribute).Any(form => Same(gpacValue, form)))
                    {
                        result.Fail(Outcome.Diverged, $"{at}/{element}.{attribute}: differs", $"{at}/{element}[{i}].{attribute}: GPAC {gpacValue}, SharpMP4 {Show(value)}");
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// A part of sample flags (14496-12 8.8.3.1), by GPAC's name for it: reserved(4) is_leading(2)
    /// sample_depends_on(2) sample_is_depended_on(2) sample_has_redundancy(2) sample_padding_value(3)
    /// sample_is_non_sync_sample(1) sample_degradation_priority(16); GPAC's Sync is 1 of a sync sample. Null of a
    /// name that is not a part of them.
    /// </summary>
    private static uint? SampleFlagsPart(uint flags, string name) => name.StartsWith("Sample", StringComparison.Ordinal) && name != "SamplePadding" ? SampleFlagsPart(flags, name.Substring(6)) : name switch
    {
        "IsLeading" => (flags >> 26) & 3,
        "DependsOn" => (flags >> 24) & 3,
        "IsDependedOn" => (flags >> 22) & 3,
        "HasRedundancy" => (flags >> 20) & 3,
        "SamplePadding" => (flags >> 17) & 7,
        "Sync" => ((flags >> 16) & 1) == 0 ? 1u : 0u,
        "DegradationPriority" => flags & 0xFFFF,
        _ => null,
    };

    /// <summary>Compares GPAC's parts of sample flags - of an entry, or of a box - with those of SharpMP4's flags.</summary>
    private static void CompareSampleFlags(uint flags, Dictionary<string, string> gpac, string at, StreamResult result)
    {
        foreach (var (name, gpacValue) in gpac)
        {
            if (SampleFlagsPart(flags, name) is not uint part)
                continue;
            result.FieldsCompared++;
            if (!Same(gpacValue, part))
                result.Fail(Outcome.Diverged, $"{at}.{name}: differs", $"{at}.{name}: GPAC {gpacValue}, SharpMP4 {part} (of sample flags 0x{flags:X8})");
        }
    }

    private static object? Value(PropertyInfo property, object owner)
    {
        try
        {
            return property.GetValue(owner);
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>GPAC's ISO 639-2/T codes of QuickTime's language codes 0 to 31 (isomedia/box_code_base.c, qtLanguages).</summary>
    private static readonly string[] QuickTimeLanguages =
    [
        "eng", "fra", "deu", "ita", "nld", "swe", "spa", "dan", "por", "nor", "heb", "jpn", "ara", "fin", "ell", "isl",
        "mlt", "tur", "hrv", "zho", "urd", "hin", "tha", "kor", "lit", "pol", "hun", "est", "lav", "sme", "fao", "fas",
    ];

    /// <summary>How many of GPAC's fields of a box are SharpMP4's, in the forms it writes them in.</summary>
    internal static int MatchingFields(Box box, Dictionary<string, string> gpacFields)
    {
        var properties = Properties(box.GetType());
        int matching = 0;
        foreach (var (name, gpacValue) in gpacFields)
        {
            if (NotFields.Contains(name) || !TryGetProperty(box, properties, name, out var property))
                continue;
            try
            {
                object? value = property.GetValue(box);
                if (value is not StreamMarker && Forms(value, box, name).Any(form => Same(gpacValue, form)))
                    matching++;
            }
            catch (Exception)
            {
            }
        }
        return matching;
    }

    /// <summary>
    /// GPAC's names of fields SharpMP4 names otherwise, by the box's class and GPAC's name: the spec's name is
    /// SharpMP4's, GPAC's is often its own (isomedia/box_dump.c).
    /// </summary>
    private static readonly Dictionary<string, string> GpacNames = new(StringComparer.Ordinal)
    {
        ["HandlerBox.hdlrType"] = "HandlerType",
        ["MediaHeaderBox.LanguageCode"] = "Language",
        ["CopyrightBox.LanguageCode"] = "Language",
        ["CopyrightBox.CopyrightNotice"] = "Notice",
        ["ExtendedLanguageBox.LanguageCode"] = "ExtendedLanguage",
        ["VisualSampleEntry.BitDepth"] = "Depth",
        ["VisualSampleEntry.XDPI"] = "Horizresolution",
        ["VisualSampleEntry.YDPI"] = "Vertresolution",
        ["AudioSampleEntry.BitsPerSample"] = "Samplesize",
        ["AudioSampleEntry.Channels"] = "Channelcount",
        ["AudioSampleEntry.Version"] = "Soundversion",
        ["MovieFragmentHeaderBox.FragmentSequenceNumber"] = "SequenceNumber",
        ["SampleSizeBox.ConstantSampleSize"] = "SampleSize",
        ["CompactSampleSizeBox.SampleSizeBits"] = "FieldSize",
        ["PaddingBitsBox.EntryCount"] = "SampleCount",
        ["TrackExtendsBox.SampleDescriptionIndex"] = "DefaultSampleDescriptionIndex",
        ["TrackExtendsBox.SampleDuration"] = "DefaultSampleDuration",
        ["TrackExtendsBox.SampleSize"] = "DefaultSampleSize",
        ["TrackFragmentHeaderBox.SampleDuration"] = "DefaultSampleDuration",
        ["TrackFragmentHeaderBox.SampleSize"] = "DefaultSampleSize",
        ["TrackHeaderBox.AlternateGroupID"] = "AlternateGroup",
        ["HintMediaHeaderBox.AverageBitRate"] = "Avgbitrate",
        ["HintMediaHeaderBox.AveragePDUSize"] = "AvgPDUsize",
        ["HintMediaHeaderBox.MaximumPDUSize"] = "MaxPDUsize",
        ["TrackFragmentRandomAccessBox.number_of_entries"] = "NumberOfEntry",
        ["MovieFragmentRandomAccessOffsetBox.container_size"] = "ParentSize",
        ["TrackEncryptionBox.IV_size"] = "DefaultPerSampleIVSize",
        ["TrackEncryptionBox.KID"] = "DefaultKID",
        ["TrackEncryptionBox.isEncrypted"] = "DefaultIsProtected",
        ["DataEntryUrlBox.URL"] = "Location",
        ["BaseLocationBox.basePurlLocation"] = "PurchaseLocation",
        ["RtpHintSampleEntry.LastCompatibleVersion"] = "Highestcompatibleversion",
        ["ReceivedRtpHintSampleEntry.LastCompatibleVersion"] = "Highestcompatibleversion",
        ["ReceivedRtcpHintSampleEntry.LastCompatibleVersion"] = "Highestcompatibleversion",
        ["FDHintSampleEntry.LastCompatibleVersion"] = "Highestcompatibleversion",
        ["HintBytesSent.RTPBytesSent"] = "Bytessent",
        ["HintBytesSenttotlDup.RTPBytesSent"] = "Bytessent",
        ["hintrepeatedBytesSent.RepeatedBytes"] = "Bytessent",
        ["HintLargestPacket.MaximumSize"] = "Bytes",
        ["HintLongestPacket.MaximumDuration"] = "Time",
        ["HintMaxRelativeTime.MaximumTransmitTime"] = "Time",
        ["HintMinRelativeTime.MinimumTransmitTime"] = "Time",
        ["HintPayloadID.PayloadString"] = "RtpmapString",
        ["TimeOffset.TimeStampOffset"] = "Offset",
        ["SimpleTextSampleEntry.mime_type"] = "MimeFormat",
        ["XMLSubtitleSampleEntry.namespace"] = "Ns",
        ["XMLMetaDataSampleEntry.namespace"] = "Ns",
        ["ProducerReferenceTimeBox.NTP"] = "NtpTimestamp",
        ["ProducerReferenceTimeBox.timestamp"] = "MediaTime",
        ["TextSampleEntrytx3gDup.backgroundColor"] = "BackgroundColorRgba",
        ["TargetOlsProperty.target_ols_index"] = "TargetOlsIdx",
        ["AmrSpecificBox.Version"] = "DecoderVersion",
        ["AmrSpecificBox.SupportedModes"] = "ModeSet",
        ["AmrSpecificBox.ModeRotating"] = "ModeChangePeriod",
    };

    /// <summary>SharpMP4's property of a field GPAC dumps: by GPAC's name for it where it has one of its own, else by the same name.</summary>
    private static bool TryGetProperty(Box box, Dictionary<string, PropertyInfo> properties, string name, out PropertyInfo property)
    {
        property = null!;
        return GpacNames.TryGetValue($"{box.GetType().Name}.{name}", out string? ours)
            ? properties.TryGetValue(Normal(ours), out property!)
            : properties.TryGetValue(Normal(name), out property!);
    }

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>> PropertiesByType = new();

    /// <summary>A box type's public properties, by their names without case and underscores; the most derived of a name.</summary>
    private static Dictionary<string, PropertyInfo> Properties(Type type) => PropertiesByType.GetOrAdd(type, t =>
    {
        var byName = new Dictionary<string, PropertyInfo>(StringComparer.Ordinal);
        for (var current = t; current != null && current != typeof(object); current = current.BaseType)
        {
            foreach (var property in current.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                if (property.GetIndexParameters().Length == 0)
                    byName.TryAdd(Normal(property.Name), property);
            }
        }
        return byName;
    });

    private static string Normal(string name) => new string(name.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();

    /// <summary>Whether GPAC's text of a field is SharpMP4's value, in any of the forms it could be written in.</summary>
    private static bool Same(string gpac, object? ours)
    {
        // GPAC's (null) of a string it has none of
        if (gpac == "(null)")
            gpac = "";
        // a number with GPAC's words for it: an oinf's scalability_mask 4 (Spatial scalability)
        var described = System.Text.RegularExpressions.Regex.Match(gpac, @"^(\S+) \(.*\)$");
        if (described.Success && Same(described.Groups[1].Value, ours))
            return true;
        switch (ours)
        {
            case null:
                return gpac.Length == 0;
            case string text:
                return text.TrimEnd('\0') == gpac || text.TrimEnd('\0').Trim() == gpac.Trim();
            case bool flag:
                return gpac is "1" or "yes" or "true" ? flag : gpac is "0" or "no" or "false" && !flag;
            case byte or sbyte or short or ushort or int or uint or long or ulong or decimal:
                decimal number = Convert.ToDecimal(ours);
                if (decimal.TryParse(gpac.Trim(), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out decimal decimalValue) && decimalValue == number)
                    return true;
                if (gpac.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
                    && ulong.TryParse(gpac.AsSpan(2), System.Globalization.NumberStyles.HexNumber, null, out ulong hex) && number >= 0 && hex == (ulong)number)
                    return true;
                // a 4CC, as the text of its bytes, a space at its end kept
                return ours is uint fourCC && TypeName(fourCC) == gpac;
            default:
                return ours.ToString()?.TrimEnd('\0') == gpac;
        }
    }

    private static string Show(object? value) => value switch
    {
        null => "null",
        uint fourCC => $"{fourCC} ({TypeName(fourCC)})",
        _ => value.ToString() ?? "",
    };

    private static void CompareChildren(DumpedBox sharp, DumpedBox gpac, string where, StreamResult result)
    {
        var types = gpac.Children.Select(b => b.Type).Concat(sharp.Children.Select(b => b.Type)).Distinct();
        foreach (string type in types)
        {
            var ours = sharp.Children.Where(b => b.Type == type).ToList();
            var theirs = gpac.Children.Where(b => b.Type == type).ToList();
            string at = where.Length == 0 ? type : $"{where}/{type}";

            if (theirs.Count == 0 && NotDumped.Contains((sharp.Type, type)))
                continue;

            // Of several, pair GPAC's one with the one of ours as large whose fields are most GPAC's - the first
            // 'colr' of hev1_clg1_header.mp4, whose transfer_characteristics GPAC gives - or else the last.
            if (theirs.Count == 1 && ours.Count > 1 && OneKept.Contains(type))
            {
                var asLarge = ours.Where(b => b.Size == theirs[0].Size).ToList();
                ours = [asLarge.Count > 0
                    ? asLarge.OrderByDescending(b => b.Source != null ? MatchingFields(b.Source, theirs[0].Fields) : 0).ThenByDescending(b => ours.IndexOf(b)).First()
                    : ours[^1]];
            }

            for (int i = 0; i < Math.Min(ours.Count, theirs.Count); i++)
            {
                result.UnitsCompared++;
                // GPAC gives 0 for a box it does not parse - a sample entry of a type it does not know.
                if (ours[i].Size != null && theirs[i].Size is ulong gpacSize && gpacSize != 0)
                {
                    result.FieldsCompared++;
                    if (ours[i].Size != theirs[i].Size)
                    {
                        result.Fail(Outcome.Diverged, $"{at}: size",
                            $"{at}[{i}]: GPAC reads {theirs[i].Size} bytes, SharpMP4 {ours[i].Size}");
                    }
                }

                if (ours[i].Source != null)
                {
                    CompareFields(ours[i].Source!, theirs[i].Fields, at, result);
                    var nested = CompareNested(ours[i].Source!, theirs[i], at, result);
                    CompareEntries(ours[i].Source!, theirs[i].Entries.Where(e => !nested.Contains(e.Key)).ToDictionary(), at, result);
                }
                CompareChildren(ours[i], theirs[i], at, result);
            }

            if (ours.Count != theirs.Count)
            {
                result.UnitsUnpaired += Math.Abs(ours.Count - theirs.Count);
                string key = theirs.Count > ours.Count ? $"{at}: missing" : $"{at}: extra";
                result.Fail(Outcome.Diverged, key,
                    $"{at}: GPAC reads {theirs.Count}, SharpMP4 {ours.Count}");
            }
        }
    }
}
