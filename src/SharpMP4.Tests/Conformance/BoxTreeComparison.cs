using System.Reflection;
using System.Text;
using SharpISOBMFF;

namespace SharpMP4.Tests.Conformance;

/// <summary>
/// Compares the boxes SharpMP4 reads out of a file with GPAC's dump of it: which boxes there are,
/// where, and how large. Within a parent, boxes are paired by type in the order they come, as the
/// dump keeps only that order.
/// </summary>
public static class BoxTreeComparison
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
                // GPAC's hex with no 0x: tx3g's displayFlags 262144 is 40000
                yield return Convert.ToUInt64(number < 0 ? 0 : number).ToString("X");
                break;
            case byte[] bytes:
                yield return "0x" + Convert.ToHexString(bytes);
                // each byte in hex, as a colour: tx3g's backgroundColor ff 0 0 ff
                yield return string.Join(" ", bytes.Select(b => b.ToString("x")));
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
                    CompareFields(ours[i].Source!, theirs[i].Fields, at, result);
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
