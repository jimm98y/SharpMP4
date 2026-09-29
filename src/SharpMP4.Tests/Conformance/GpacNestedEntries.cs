using System.Reflection;
using SharpISOBMFF;

namespace SharpMP4.Tests.Conformance;

/// <summary>
/// The entries GPAC gives in entries - a senc's samples and their subsamples, an iloc's items and their extents, a
/// decoder configuration's parameter sets - each compared with where SharpMP4 keeps it: in arrays of arrays, or in
/// objects of its own. Compared by the same forms as a box's fields.
/// </summary>
public static partial class BoxTreeComparison
{
    /// <summary>
    /// Compares the nested entries of a box whose entries GPAC nests, and says which of its entries' elements it
    /// did, not to be compared as flat lists; none of a box of another kind.
    /// </summary>
    private static HashSet<string> CompareNested(Box box, DumpedBox gpac, string at, StreamResult result)
    {
        var tree = gpac.EntryTree;
        switch (box)
        {
            case SampleEncryptionBox senc:
                CompareSampleEncryption(senc, tree, at, result);
                return ["FullBoxInfo", "SampleEncryptionEntry", "SubSampleEncryptionEntry"];
            case SubSampleInformationBox subs:
                CompareSubSamples(subs, tree, at, result);
                return ["SampleEntry", "SubSample"];
            case ItemLocationBox iloc:
                CompareItemLocations(iloc, tree, at, result);
                return ["ItemLocationEntry", "ItemExtentEntry"];
            case ItemPropertyAssociationBox ipma:
                CompareItemPropertyAssociations(ipma, tree, at, result);
                return ["AssociationEntry", "Property"];
            case SampleGroupDescriptionBox sgpd:
                CompareSampleGroupDescriptions(sgpd, tree, at, result);
                // its entries, and the entries in them, counted there
                return tree.Select(e => e.Name).Concat(Descendants(tree).Select(e => e.Name)).ToHashSet();
            case TrackHeaderBox tkhd:
                foreach (var matrix in tree.Where(e => e.Name == "Matrix"))
                {
                    string[] names = ["m11", "m12", "m13", "m21", "m22", "m23", "m31", "m32", "m33"];
                    for (int i = 0; i < names.Length; i++)
                        Check(result, tkhd, at, "Matrix", matrix.Attributes, names[i], At(tkhd.Matrix, i));
                }
                return ["Matrix"];
            case EntityToGroupBox group:
                var entities = tree.Where(e => e.Name == "EntityToGroupTypeBoxEntry").ToList();
                for (int i = 0; i < entities.Count && !Missing(result, at, "EntityToGroupTypeBoxEntry", i, group.EntityId?.Length ?? 0); i++)
                    Check(result, group, at, $"EntityToGroupTypeBoxEntry[{i}]", entities[i].Attributes, "EntityID", group.EntityId![i]);
                return ["EntityToGroupTypeBoxEntry"];
            case TrackSelectionBox tsel:
                var criteria = tree.Where(e => e.Name == "TrackSelectionCriteria").ToList();
                for (int i = 0; i < criteria.Count && !Missing(result, at, "TrackSelectionCriteria", i, tsel.AttributeList?.Length ?? 0); i++)
                    Check(result, tsel, at, $"TrackSelectionCriteria[{i}]", criteria[i].Attributes, "value", tsel.AttributeList![i]);
                return ["TrackSelectionCriteria"];
            case ProgressiveDownloadInfoBox pdin:
                var rates = tree.Where(e => e.Name == "DownloadInfo").ToList();
                for (int i = 0; i < rates.Count && !Missing(result, at, "DownloadInfo", i, pdin.Items?.Length ?? 0); i++)
                {
                    Check(result, pdin, at, $"DownloadInfo[{i}]", rates[i].Attributes, "rate", pdin.Items![i].Rate);
                    Check(result, pdin, at, $"DownloadInfo[{i}]", rates[i].Attributes, "estimatedTime", pdin.Items![i].InitialDelay);
                }
                return ["DownloadInfo"];
            case ProtectionSystemSpecificHeaderBox pssh:
                var keys = tree.Where(e => e.Name == "PSSHKey").ToList();
                for (int i = 0; i < keys.Count && !Missing(result, at, "PSSHKey", i, pssh.KeyIDs?.Length ?? 0); i++)
                    CompareObject(pssh, pssh.KeyIDs![i], keys[i], at, $"PSSHKey[{i}]", result);
                foreach (var data in tree.Where(e => e.Name == "PSSHData"))
                {
                    Check(result, pssh, at, "PSSHData", data.Attributes, "size", pssh.DataSize);
                    Check(result, pssh, at, "PSSHData", data.Attributes, "value", pssh.Data);
                }
                return ["PSSHKey", "PSSHData"];
            case SubsegmentIndexBox ssix:
                var subsegments = tree.Where(e => e.Name == "Subsegment").ToList();
                for (int i = 0; i < subsegments.Count && !Missing(result, at, "Subsegment", i, ssix.RangeCount?.Length ?? 0); i++)
                {
                    Check(result, ssix, at, $"Subsegment[{i}]", subsegments[i].Attributes, "range_count", ssix.RangeCount![i]);
                    var ranges = subsegments[i].Named("Range");
                    for (int j = 0; j < ranges.Count && !Missing(result, at, $"Subsegment[{i}]/Range", j, At(ssix.RangeSize, i)?.Length ?? 0); j++)
                    {
                        Check(result, ssix, at, $"Subsegment[{i}]/Range[{j}]", ranges[j].Attributes, "level", At(At(ssix.Level, i), j));
                        Check(result, ssix, at, $"Subsegment[{i}]/Range[{j}]", ranges[j].Attributes, "range_size", ssix.RangeSize![i][j]);
                    }
                }
                return ["Subsegment", "Range"];
            case TextSampleEntrytx3gDup tx3g:
                // its default style: its face style flags in GPAC's words, Bold, Italic and Underlined, or Normal
                foreach (var style in tree.Where(e => e.Name == "DefaultStyle").SelectMany(e => e.Named("StyleRecord")))
                {
                    var record = tx3g.StyleRecord;
                    if (Missing(result, at, "DefaultStyle/StyleRecord", 0, record == null ? 0 : 1))
                        break;
                    string[] faces = new[] { (1, "Bold"), (2, "Italic"), (4, "Underlined") }.Where(f => (record!.FaceStyleFlags & f.Item1) != 0).Select(f => f.Item2).ToArray();
                    Check(result, tx3g, at, "DefaultStyle/StyleRecord", style.Attributes, "startChar", record!.StartChar);
                    Check(result, tx3g, at, "DefaultStyle/StyleRecord", style.Attributes, "endChar", record.EndChar);
                    Check(result, tx3g, at, "DefaultStyle/StyleRecord", style.Attributes, "fontID", record.FontId);
                    Check(result, tx3g, at, "DefaultStyle/StyleRecord", style.Attributes, "styles", faces.Length == 0 ? "Normal" : string.Join(" ", faces));
                    Check(result, tx3g, at, "DefaultStyle/StyleRecord", style.Attributes, "fontSize", record.FontSize);
                    Check(result, tx3g, at, "DefaultStyle/StyleRecord", style.Attributes, "textColor", record.TextColor);
                }
                return ["DefaultStyle", "StyleRecord"];
            case OperatingPointsInformationProperty oinf:
                foreach (var entry in tree.Where(e => e.Name == "OperatingPointsInformation"))
                {
                    if (!Missing(result, at, "OperatingPointsInformation", 0, oinf.OpInfo == null ? 0 : 1))
                        CompareObject(oinf, oinf.OpInfo!, entry, at, "OperatingPointsInformation", result);
                }
                return tree.Select(e => e.Name).Concat(Descendants(tree).Select(e => e.Name)).ToHashSet();
            case FDSessionGroupBox segr:
                // each session group's group IDs and hint track IDs, as GPAC lists them
                var sessions = tree.Where(e => e.Name == "FDSessionGroupBoxEntry").ToList();
                for (int i = 0; i < sessions.Count && !Missing(result, at, "FDSessionGroupBoxEntry", i, segr.GroupID?.Length ?? 0); i++)
                {
                    Check(result, segr, at, $"FDSessionGroupBoxEntry[{i}]", sessions[i].Attributes, "groupIDs", string.Concat((At(segr.GroupID, i) ?? []).Select(g => $"{g} ")));
                    Check(result, segr, at, $"FDSessionGroupBoxEntry[{i}]", sessions[i].Attributes, "channels", string.Concat((At(segr.HintTrackID, i) ?? []).Select(t => $"{t} ")));
                }
                return ["FDSessionGroupBoxEntry"];
            case ESDBox esds:
                CompareEsDescriptor(esds, esds._ES, tree, at, result);
                return tree.Select(e => e.Name).Concat(Descendants(tree).Select(e => e.Name)).ToHashSet();
            case AppleInitialObjectDescriptorBox iods:
                CompareInitialObjectDescriptor(iods, tree, at, result);
                return tree.Select(e => e.Name).Concat(Descendants(tree).Select(e => e.Name)).ToHashSet();
        }

        // a decoder configuration record, of whichever box it is in: an avcC, an hvcC, an lhvC, an mvcC
        foreach (var property in box.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            switch (Value(property, box))
            {
                case AVCDecoderConfigurationRecord avc:
                    CompareAvcRecord(avc, box, tree, at, result);
                    return ["AVCDecoderConfigurationRecord", "SequenceParameterSet", "PictureParameterSet"];
                case MVCDecoderConfigurationRecord mvc:
                    CompareLayeredAvcRecord(box, tree, "MVCDecoderConfigurationRecord", mvc.ConfigurationVersion, mvc._AVCProfileIndication, mvc.ProfileCompatibility, mvc._AVCLevelIndication,
                        mvc.CompleteRepresentation, mvc.LengthSizeMinusOne, mvc.SequenceParameterSetLength, mvc.SequenceParameterSetNALUnit, mvc.PictureParameterSetLength, mvc.PictureParameterSetNALUnit, at, result);
                    return ["MVCDecoderConfigurationRecord", "SequenceParameterSet", "PictureParameterSet"];
                case SVCDecoderConfigurationRecord svc:
                    CompareLayeredAvcRecord(box, tree, "SVCDecoderConfigurationRecord", svc.ConfigurationVersion, svc._AVCProfileIndication, svc.ProfileCompatibility, svc._AVCLevelIndication,
                        svc.CompleteRepresenation, svc.LengthSizeMinusOne, svc.SequenceParameterSetLength, svc.SequenceParameterSetNALUnit, svc.PictureParameterSetLength, svc.PictureParameterSetNALUnit, at, result);
                    return ["SVCDecoderConfigurationRecord", "SequenceParameterSet", "PictureParameterSet"];
                case HEVCDecoderConfigurationRecord hevc:
                    CompareHevcRecord(hevc, hevc.NALUnitType, hevc.ArrayCompleteness, hevc.NalUnitLength, hevc.NalUnit, box, tree, "HEVCDecoderConfigurationRecord", at, result);
                    return ["HEVCDecoderConfigurationRecord", "ParameterSetArray", "ParameterSet"];
                case LHEVCDecoderConfigurationRecord lhevc:
                    CompareHevcRecord(lhevc, lhevc.NALUnitType, lhevc.ArrayCompleteness, lhevc.NalUnitLength, lhevc.NalUnit, box, tree, "L-HEVCDecoderConfigurationRecord", at, result);
                    return ["L-HEVCDecoderConfigurationRecord", "ParameterSetArray", "ParameterSet"];
            }
        }

        return [];
    }

    /// <summary>Compares one of GPAC's values with SharpMP4's, if GPAC gives it; a failure is keyed by where and what.</summary>
    private static void Check(StreamResult result, Box box, string at, string element, Dictionary<string, string> gpac, string name, object? ours)
    {
        if (!gpac.TryGetValue(name, out string? gpacValue))
            return;
        result.FieldsCompared++;
        if (!Forms(ours, box, name).Any(form => Same(gpacValue, form)))
            result.Fail(Outcome.Diverged, $"{at}/{Strip(element)}.{name}: differs", $"{at}/{element}.{name}: GPAC {gpacValue}, SharpMP4 {Show(ours)}");
    }

    /// <summary>Whether GPAC has more entries of a kind than SharpMP4: a failure, and the rest of them not compared.</summary>
    private static bool Missing(StreamResult result, string at, string element, int index, int count)
    {
        if (index < count)
            return false;
        result.Fail(Outcome.Diverged, $"{at}/{Strip(element)}: missing", $"{at}/{element}: GPAC has entry {index}, SharpMP4 {count} of them");
        return true;
    }

    /// <summary>An element's path without its indices: a failure's key is the same of every entry.</summary>
    private static string Strip(string element) => System.Text.RegularExpressions.Regex.Replace(element, @"\[\d+\]", "");

    private static T? At<T>(T[]? array, int index) => array != null && index < array.Length ? array[index] : default;

    private static IEnumerable<GpacEntry> Descendants(IEnumerable<GpacEntry> entries) =>
        entries.SelectMany(e => e.Children.Concat(Descendants(e.Children)));

    /// <summary>GPAC's names of the fields of sample group entries SharpMP4 names otherwise, by the entry's class.</summary>
    private static readonly Dictionary<string, string> SampleGroupEntryNames = new(StringComparer.Ordinal)
    {
        ["CencSampleEncryptionInformationGroupEntry.IsEncrypted"] = "IsProtected",
        ["CencSampleEncryptionInformationGroupEntry.IV_size"] = "PerSampleIVSize",
        ["CencSampleEncryptionInformationGroupEntry.KID"] = "_KID",
        ["CencSampleEncryptionInformationGroupEntry.constant_IV_size"] = "ConstantIVSize",
        ["CencSampleEncryptionInformationGroupEntry.constant_IV"] = "ConstantIV",
        ["RectangularRegionGroupEntry.ID"] = "GroupID",
        ["RectangularRegionGroupEntry.independent"] = "IndependentIdc",
        ["RectangularRegionGroupEntry.full_picture"] = "FullPicture",
        ["RectangularRegionGroupEntry.filter_disabled"] = "FilteringDisabled",
        ["RectangularRegionGroupEntry.x"] = "HorizontalOffset",
        ["RectangularRegionGroupEntry.y"] = "VerticalOffset",
        ["RectangularRegionGroupEntry.w"] = "RegionWidth",
        ["RectangularRegionGroupEntry.h"] = "RegionHeight",
        ["LayerInfoGroupEntry.num_layers"] = "NumLayersInTrack",
        ["OperatingPointsRecord.dependency_layers"] = "MaxLayerCount",
        ["OperatingPointsRecord.maxBitDepth"] = "MaxBitDepthMinus8",
        ["ProtectionSystemSpecificKeyID.KID"] = "Key",
    };

    /// <summary>
    /// Compares an sgpd's entries, each GPAC's element of it with SharpMP4's entry of its place: its fields, by their
    /// names or GPAC's; one SharpMP4 keeps as bytes (an UnknownEntry) by its length. The entries in an entry - an
    /// oinf's operating points - are counted as fields SharpMP4 has none of.
    /// </summary>
    private static void CompareSampleGroupDescriptions(SampleGroupDescriptionBox sgpd, List<GpacEntry> tree, string at, StreamResult result)
    {
        var ours = sgpd._SampleGroupDescriptionEntry ?? [];
        for (int i = 0; i < tree.Count; i++)
        {
            if (Missing(result, at, tree[i].Name, i, ours.Length))
                break;
            var entry = ours[i];
            string element = $"{tree[i].Name}[{i}]";
            if (entry is UnknownEntry)
            {
                Check(result, sgpd, at, element, tree[i].Attributes, "size", At(sgpd.DescriptionLength, i) is uint length and > 0 ? length : sgpd.DefaultLength);
                continue;
            }

            CompareObject(sgpd, Record(entry), tree[i], at, element, result);
        }
    }

    /// <summary>
    /// What an entry's fields are of: the entry, or the one record it holds - an oinf entry's OperatingPointsRecord,
    /// whose fields GPAC gives as the entry's.
    /// </summary>
    private static object Record(object entry)
    {
        var records = entry.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(p => typeof(IMp4Serializable).IsAssignableFrom(p.PropertyType) && !typeof(Box).IsAssignableFrom(p.PropertyType))
            .ToList();
        return records.Count == 1 && Value(records[0], entry) is object record ? record : entry;
    }

    /// <summary>
    /// Compares one of GPAC's entries with an object of SharpMP4's: its attributes with the object's fields, by
    /// their names or GPAC's; and the entries in it - a linf's LayerInfoItem, an oinf's OperatingPoint - each
    /// attribute of the j-th of a kind with the j-th of the object's array of that name.
    /// </summary>
    private static void CompareObject(Box box, object owner, GpacEntry entry, string at, string element, StreamResult result)
    {
        var properties = Properties(owner.GetType());
        string ownerClass = owner.GetType().Name;

        PropertyInfo? Find(string name) =>
            properties.GetValueOrDefault(Normal(SampleGroupEntryNames.GetValueOrDefault($"{ownerClass}.{name}", name)));

        foreach (var (name, _) in entry.Attributes)
        {
            if (Find(name) is not PropertyInfo property)
            {
                Unpaired.AddOrUpdate($"{ownerClass}.{name}", 1, (_, n) => n + 1);
                continue;
            }
            Check(result, box, at, element, entry.Attributes, name, Value(property, owner));
        }

        foreach (var group in entry.Children.GroupBy(c => c.Name))
        {
            var children = group.ToList();
            for (int j = 0; j < children.Count; j++)
            {
                foreach (var (name, _) in children[j].Attributes)
                {
                    object? ours;
                    if (name == "dimension_identifier" && owner is OperatingPointsRecord operatingPoints)
                    {
                        // those of the scalability types its mask has, as GPAC lists them
                        var dimensions = At(operatingPoints.DimensionIdentifier, j) ?? [];
                        ours = string.Concat(Enumerable.Range(0, 16).Where(k => ((operatingPoints.ScalabilityMask >> k) & 1) == 1 && k < dimensions.Length).Select(k => $"{dimensions[k]} "));
                    }
                    else if (Find(name) is PropertyInfo property && Value(property, owner) is Array array)
                    {
                        if (Missing(result, $"{at}/{element}", group.Key, j, array.Length))
                            break;
                        ours = array.GetValue(j);
                    }
                    else
                    {
                        Unpaired.AddOrUpdate($"{ownerClass}.{group.Key}.{name}", 1, (_, n) => n + 1);
                        continue;
                    }
                    Check(result, box, at, $"{element}/{group.Key}[{j}]", children[j].Attributes, name, ours);
                }
            }
        }
    }

    /// <summary>
    /// Compares an esds's ES_Descriptor, its DecoderConfigDescriptor with its decoder specific info as a data URL,
    /// and its SLConfigDescriptor's predefined, with GPAC's: its ES_ID as es and the number, as GPAC names streams.
    /// </summary>
    private static void CompareEsDescriptor(Box box, ES_Descriptor? es, List<GpacEntry> tree, string at, StreamResult result)
    {
        foreach (var element in tree.Where(e => e.Name == "ES_Descriptor"))
        {
            if (Missing(result, at, "ES_Descriptor", 0, es == null ? 0 : 1))
                return;
            Check(result, box, at, "ES_Descriptor", element.Attributes, "ES_ID", $"es{es!.ESID}");
            Check(result, box, at, "ES_Descriptor", element.Attributes, "binaryID", es.ESID);
            Check(result, box, at, "ES_Descriptor", element.Attributes, "streamPriority", es.StreamPriority);

            var config = es.DecConfigDescr;
            foreach (var decoder in element.Named("decConfigDescr").SelectMany(d => d.Named("DecoderConfigDescriptor")))
            {
                if (Missing(result, at, "ES_Descriptor/DecoderConfigDescriptor", 0, config == null ? 0 : 1))
                    break;
                foreach (string name in new[] { "objectTypeIndication", "streamType", "bufferSizeDB", "maxBitrate", "avgBitrate" })
                    Check(result, box, at, "ES_Descriptor/DecoderConfigDescriptor", decoder.Attributes, name, Value(Properties(config!.GetType())[Normal(name)], config));

                // the decoder specific info's bytes: as SharpMP4 keeps them, or as it writes an AudioSpecificConfig
                foreach (var info in decoder.Named("decSpecificInfo").SelectMany(d => d.Named("DecoderSpecificInfo")))
                {
                    byte[]? bytes = config!.Children?.OfType<GenericDecoderSpecificInfo>().FirstOrDefault()?.Data;
                    if (bytes == null && config.Children?.OfType<AudioSpecificConfig>().FirstOrDefault() is AudioSpecificConfig asc)
                    {
                        using var written = new MemoryStream();
                        using (var stream = new IsoStream(written))
                            asc.Write(stream);
                        bytes = written.ToArray();
                    }
                    Check(result, box, at, "ES_Descriptor/DecoderConfigDescriptor/DecoderSpecificInfo", info.Attributes, "src", bytes);
                }
            }

            foreach (var sl in element.Named("slConfigDescr").SelectMany(d => d.Named("SLConfigDescriptor")))
            {
                foreach (var predefined in sl.Named("predefined"))
                    Check(result, box, at, "ES_Descriptor/SLConfigDescriptor/predefined", predefined.Attributes, "value", es.SlConfigDescr?.Predefined);
            }
        }
    }

    /// <summary>Compares an iods's initial object descriptor - its id as od and the number, its profiles, the tracks it includes - with GPAC's.</summary>
    private static void CompareInitialObjectDescriptor(AppleInitialObjectDescriptorBox iods, List<GpacEntry> tree, string at, StreamResult result)
    {
        var descriptor = iods.Descriptor?.OfType<IOD_Descriptor>().FirstOrDefault();
        foreach (var element in tree.Where(e => e.Name == "MP4InitialObjectDescriptor"))
        {
            if (Missing(result, at, "MP4InitialObjectDescriptor", 0, descriptor == null ? 0 : 1))
                return;
            Check(result, iods, at, "MP4InitialObjectDescriptor", element.Attributes, "objectDescriptorID", $"od{descriptor!._ObjectDescriptorID}");
            Check(result, iods, at, "MP4InitialObjectDescriptor", element.Attributes, "binaryID", descriptor._ObjectDescriptorID);
            var properties = Properties(descriptor.GetType());
            foreach (var profile in element.Named("Profile"))
            {
                foreach (var (name, _) in profile.Attributes)
                    Check(result, iods, at, "MP4InitialObjectDescriptor/Profile", profile.Attributes, name, properties.TryGetValue(Normal(name), out var property) ? Value(property, descriptor) : null);
            }

            var included = descriptor.EsIdInc?.ToList() ?? [];
            var tracks = Descendants([element]).Where(e => e.Name == "ES_ID_Inc").ToList();
            for (int i = 0; i < tracks.Count && !Missing(result, at, "MP4InitialObjectDescriptor/ES_ID_Inc", i, included.Count); i++)
                Check(result, iods, at, $"MP4InitialObjectDescriptor/ES_ID_Inc[{i}]", tracks[i].Attributes, "trackID", included[i].TrackID);
        }
    }

    private static void CompareSampleEncryption(SampleEncryptionBox senc, List<GpacEntry> tree, string at, StreamResult result)
    {
        foreach (var info in tree.Where(e => e.Name == "FullBoxInfo"))
        {
            Check(result, senc, at, "FullBoxInfo", info.Attributes, "Version", senc.Version);
            Check(result, senc, at, "FullBoxInfo", info.Attributes, "Flags", senc.Flags);
        }

        // the box keeps its samples as bytes, whose IV size is the track's: split by the size GPAC, which read the track's
        // 'tenc', gives each, they are to hold the IVs and subsamples GPAC read
        var samples = tree.Where(e => e.Name == "SampleEncryptionEntry").ToList();
        int IvSizeOf(int i) => i < samples.Count && samples[i].Attributes.TryGetValue("IV_size", out string? size) && int.TryParse(size, out int n) ? n : -1;
        var entries = SharpMP4.Encryption.SampleEncryptionReader.ReadSampleData(senc.SampleData, (int)senc.SampleCount, senc.Flags, IvSizeOf);
        for (int i = 0; i < samples.Count; i++)
        {
            if (Missing(result, at, "SampleEncryptionEntry", i, entries.Count))
                break;
            var sample = entries[i];
            string element = $"SampleEncryptionEntry[{i}]";
            var gpac = samples[i].Attributes;
            Check(result, senc, at, element, gpac, "sampleNumber", i + 1);
            Check(result, senc, at, element, gpac, "IV", sample.IV ?? []);
            Check(result, senc, at, element, gpac, "SubsampleCount", sample.Subsamples?.Length ?? 0);

            var subsamples = samples[i].Named("SubSampleEncryptionEntry");
            for (int j = 0; j < subsamples.Count; j++)
            {
                if (Missing(result, at, $"{element}/SubSampleEncryptionEntry", j, sample.Subsamples?.Length ?? 0))
                    break;
                var subsample = sample.Subsamples![j];
                Check(result, senc, at, $"{element}/SubSampleEncryptionEntry[{j}]", subsamples[j].Attributes, "NumClearBytes", subsample.ClearBytes);
                Check(result, senc, at, $"{element}/SubSampleEncryptionEntry[{j}]", subsamples[j].Attributes, "NumEncryptedBytes", subsample.ProtectedBytes);
            }
        }
    }

    private static void CompareSubSamples(SubSampleInformationBox subs, List<GpacEntry> tree, string at, StreamResult result)
    {
        var samples = tree.Where(e => e.Name == "SampleEntry").ToList();
        for (int i = 0; i < samples.Count; i++)
        {
            if (Missing(result, at, "SampleEntry", i, subs.SampleDelta?.Length ?? 0))
                break;
            string element = $"SampleEntry[{i}]";
            // the first delta 0 of subs_slice_hvc1.mp4 and subs_tile_hvc1.mp4 is 1 in GPAC's dump, which GPAC today would
            // give as 0 (isomedia/box_dump.c, subs_box_dump): made by a GPAC that read it otherwise, or of the file before
            Check(result, subs, at, element, samples[i].Attributes, "SampleDelta", i == 0 && subs.SampleDelta![i] == 0 ? 1u : subs.SampleDelta![i]);
            Check(result, subs, at, element, samples[i].Attributes, "SubSampleCount", At(subs.SubsampleCount, i));

            var subsamples = samples[i].Named("SubSample");
            for (int j = 0; j < subsamples.Count; j++)
            {
                if (Missing(result, at, $"{element}/SubSample", j, At(subs.SubsampleSize, i)?.Length ?? 0))
                    break;
                var gpac = subsamples[j].Attributes;
                string sub = $"{element}/SubSample[{j}]";
                Check(result, subs, at, sub, gpac, "Size", subs.SubsampleSize![i][j]);
                Check(result, subs, at, sub, gpac, "Priority", At(At(subs.SubsamplePriority, i), j));
                Check(result, subs, at, sub, gpac, "Discardable", At(At(subs.Discardable, i), j));
                Check(result, subs, at, sub, gpac, "Reserved", At(At(subs.CodecSpecificParameters, i), j));
            }
        }
    }

    private static void CompareItemLocations(ItemLocationBox iloc, List<GpacEntry> tree, string at, StreamResult result)
    {
        var items = tree.Where(e => e.Name == "ItemLocationEntry").ToList();
        for (int i = 0; i < items.Count; i++)
        {
            if (Missing(result, at, "ItemLocationEntry", i, iloc.ItemID?.Length ?? 0))
                break;
            string element = $"ItemLocationEntry[{i}]";
            var gpac = items[i].Attributes;
            Check(result, iloc, at, element, gpac, "item_ID", iloc.ItemID![i]);
            Check(result, iloc, at, element, gpac, "data_reference_index", At(iloc.DataReferenceIndex, i));
            Check(result, iloc, at, element, gpac, "base_offset", At(iloc.BaseOffset, i));
            Check(result, iloc, at, element, gpac, "construction_method", At(iloc.ConstructionMethod, i));

            var extents = items[i].Named("ItemExtentEntry");
            for (int j = 0; j < extents.Count; j++)
            {
                if (Missing(result, at, $"{element}/ItemExtentEntry", j, At(iloc.ExtentOffset, i)?.Length ?? 0))
                    break;
                string extent = $"{element}/ItemExtentEntry[{j}]";
                Check(result, iloc, at, extent, extents[j].Attributes, "extent_offset", iloc.ExtentOffset![i][j]);
                Check(result, iloc, at, extent, extents[j].Attributes, "extent_length", At(At(iloc.ExtentLength, i), j));
                // an extent's index, of iloc versions 1 and 2 with index_size: none, 0 to GPAC
                Check(result, iloc, at, extent, extents[j].Attributes, "extent_index", At(At(iloc.ItemReferenceIndex, i), j) ?? []);
            }
        }
    }

    private static void CompareItemPropertyAssociations(ItemPropertyAssociationBox ipma, List<GpacEntry> tree, string at, StreamResult result)
    {
        var entries = tree.Where(e => e.Name == "AssociationEntry").ToList();
        for (int i = 0; i < entries.Count; i++)
        {
            if (Missing(result, at, "AssociationEntry", i, ipma.ItemID?.Length ?? 0))
                break;
            string element = $"AssociationEntry[{i}]";
            Check(result, ipma, at, element, entries[i].Attributes, "item_ID", ipma.ItemID![i]);
            Check(result, ipma, at, element, entries[i].Attributes, "association_count", At(ipma.AssociationCount, i));

            var properties = entries[i].Named("Property");
            for (int j = 0; j < properties.Count; j++)
            {
                if (Missing(result, at, $"{element}/Property", j, At(ipma.PropertyIndex, i)?.Length ?? 0))
                    break;
                Check(result, ipma, at, $"{element}/Property[{j}]", properties[j].Attributes, "index", ipma.PropertyIndex![i][j]);
                Check(result, ipma, at, $"{element}/Property[{j}]", properties[j].Attributes, "essential", At(At(ipma.Essential, i), j));
            }
        }
    }

    private static void CompareAvcRecord(AVCDecoderConfigurationRecord avc, Box box, List<GpacEntry> tree, string at, StreamResult result)
    {
        foreach (var record in tree.Where(e => e.Name == "AVCDecoderConfigurationRecord"))
        {
            var gpac = record.Attributes;
            string element = "AVCDecoderConfigurationRecord";
            CompareAvcFields(box, record, element, avc.ConfigurationVersion, avc._AVCProfileIndication, avc.ProfileCompatibility, avc._AVCLevelIndication, avc.LengthSizeMinusOne,
                avc.SequenceParameterSetLength, avc.SequenceParameterSetNALUnit, avc.PictureParameterSetLength, avc.PictureParameterSetNALUnit, at, result);
            // of the High profiles' extension, which GPAC gives where there is one
            if (avc.HasExtensions)
            {
                Check(result, box, at, element, gpac, "chroma_format", avc.ChromaFormat);
                Check(result, box, at, element, gpac, "luma_bit_depth", avc.BitDepthLumaMinus8 + 8);
                Check(result, box, at, element, gpac, "chroma_bit_depth", avc.BitDepthChromaMinus8 + 8);
            }
        }
    }

    /// <summary>An mvcC's or an svcC's record: an avcC's fields, and whether the track's representation is complete.</summary>
    private static void CompareLayeredAvcRecord(Box box, List<GpacEntry> tree, string element, byte version, byte profile, byte compatibility, byte level, bool complete, byte lengthSizeMinusOne,
        ushort[]? spsLengths, byte[][]? sps, ushort[]? ppsLengths, byte[][]? pps, string at, StreamResult result)
    {
        foreach (var record in tree.Where(e => e.Name == element))
        {
            CompareAvcFields(box, record, element, version, profile, compatibility, level, lengthSizeMinusOne, spsLengths, sps, ppsLengths, pps, at, result);
            Check(result, box, at, element, record.Attributes, "complete_representation", complete);
        }
    }

    private static void CompareAvcFields(Box box, GpacEntry record, string element, byte version, byte profile, byte compatibility, byte level, byte lengthSizeMinusOne,
        ushort[]? spsLengths, byte[][]? sps, ushort[]? ppsLengths, byte[][]? pps, string at, StreamResult result)
    {
        var gpac = record.Attributes;
        Check(result, box, at, element, gpac, "configurationVersion", version);
        Check(result, box, at, element, gpac, "AVCProfileIndication", profile);
        Check(result, box, at, element, gpac, "profile_compatibility", compatibility);
        Check(result, box, at, element, gpac, "AVCLevelIndication", level);
        Check(result, box, at, element, gpac, "nal_unit_size", lengthSizeMinusOne + 1);
        CompareParameterSets(record.Named("SequenceParameterSet"), spsLengths, sps, box, $"{at}/{element}", "SequenceParameterSet", result);
        CompareParameterSets(record.Named("PictureParameterSet"), ppsLengths, pps, box, $"{at}/{element}", "PictureParameterSet", result);
    }

    private static void CompareParameterSets(List<GpacEntry> sets, ushort[]? lengths, byte[][]? units, Box box, string at, string element, StreamResult result)
    {
        for (int i = 0; i < sets.Count; i++)
        {
            if (Missing(result, at, element, i, units?.Length ?? 0))
                break;
            Check(result, box, at, $"{element}[{i}]", sets[i].Attributes, "size", At(lengths, i));
            Check(result, box, at, $"{element}[{i}]", sets[i].Attributes, "content", units![i]);
        }
    }

    /// <summary>GPAC's names of an HEVC or L-HEVC decoder configuration record's fields SharpMP4 names otherwise.</summary>
    private static readonly Dictionary<string, string> HevcRecordNames = new(StringComparer.Ordinal)
    {
        ["profile_space"] = "GeneralProfileSpace",
        ["tier_flag"] = "GeneralTierFlag",
        ["profile_idc"] = "GeneralProfileIdc",
        ["level_idc"] = "GeneralLevelIdc",
        ["constraint_indicator_flags"] = "GeneralConstraintIndicatorFlags",
    };

    /// <summary>
    /// The four flags GPAC gives of general_constraint_indicator_flags apart (H.265 7.3.3): its first 4 bits of 48,
    /// progressive_source_flag first.
    /// </summary>
    private static readonly string[] SourceFlags = ["progressive_source_flag", "interlaced_source_flag", "non_packed_constraint_flag", "frame_only_constraint_flag"];

    private static void CompareHevcRecord(object record, byte[]? types, bool[]? complete, ushort[][]? lengths, byte[][][]? units, Box box, List<GpacEntry> tree, string element, string at, StreamResult result)
    {
        var properties = Properties(record.GetType());
        foreach (var entry in tree.Where(e => e.Name == element))
        {
            foreach (var (name, _) in entry.Attributes)
            {
                object? ours;
                if (name == "nal_unit_size" && properties.TryGetValue("lengthsizeminusone", out var lengthSize))
                    ours = Convert.ToInt32(Value(lengthSize, record)) + 1;
                else if (name is "luma_bit_depth" or "chroma_bit_depth" && properties.TryGetValue(name == "luma_bit_depth" ? "bitdepthlumaminus8" : "bitdepthchromaminus8", out var depth))
                    ours = Convert.ToInt32(Value(depth, record)) + 8;
                else if (Array.IndexOf(SourceFlags, name) is int flag and >= 0 && properties.TryGetValue("generalconstraintindicatorflags", out var constraints))
                    ours = (Convert.ToUInt64(Value(constraints, record)) >> (47 - flag)) & 1;
                // the rest of the 48 bits, the four flags given apart, in GPAC's lower case hex
                else if (name == "constraint_indicator_flags" && properties.TryGetValue("generalconstraintindicatorflags", out var rest))
                    ours = (Convert.ToUInt64(Value(rest, record)) & ((1UL << 44) - 1)).ToString("x");
                else if (properties.TryGetValue(Normal(HevcRecordNames.GetValueOrDefault(name, name)), out var property))
                    ours = Value(property, record);
                else
                {
                    Unpaired.AddOrUpdate($"{record.GetType().Name}.{name}", 1, (_, n) => n + 1);
                    continue;
                }
                Check(result, box, at, element, entry.Attributes, name, ours);
            }

            var arrays = entry.Named("ParameterSetArray");
            for (int j = 0; j < arrays.Count; j++)
            {
                if (Missing(result, $"{at}/{element}", "ParameterSetArray", j, types?.Length ?? 0))
                    break;
                string array = $"{element}/ParameterSetArray[{j}]";
                Check(result, box, at, array, arrays[j].Attributes, "nalu_type", types![j]);
                Check(result, box, at, array, arrays[j].Attributes, "complete_set", At(complete, j));
                CompareParameterSets(arrays[j].Named("ParameterSet"), At(lengths, j), At(units, j), box, $"{at}/{array}", "ParameterSet", result);
            }
        }
    }
}
