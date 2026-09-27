using SharpH26X;
using SharpMP4.Common;
using SharpMP4.Tracks;
using System.Text.RegularExpressions;
using H264 = SharpH264;
using H265 = SharpH265;
using H266 = SharpH266;

namespace SharpMP4.Tests.Conformance;

/// <summary>
/// The headers of a bitstream as SharpMP4 reads them: every NAL unit parsed in order, as a
/// decoder would, so the parameter sets in force are the ones the stream set up, and every syntax
/// element read along the way recorded.
/// </summary>
public static partial class SharpTrace
{
    /// <summary>Reads an Annex B H.264, H.265 or H.266 stream, NAL unit by NAL unit.</summary>
    /// <returns>The units, and the exception that stopped the reading, if one did.</returns>
    public static (List<TracedUnit> Units, Exception? Error) ReadH26X(string path, string codec)
    {
        var capture = new FieldCapture();
        var units = new List<TracedUnit>();

        Func<ArraySegment<byte>, ItuStream, string> parse = codec switch
        {
            "h264" => H264Parser(new H264.H264Context()),
            "h265" => H265Parser(new H265.H265Context()),
            // As ffmpeg, reject what a conforming stream cannot contain (VPS_C_2 has an OLS without
            // an output layer).
            "h266" => H266Parser(new H266.H266Context { Strict = true }),
            _ => throw new ArgumentException($"not an H.26x codec: {codec}", nameof(codec)),
        };

        using var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 16);
        var splitter = new H265Track();

        try
        {
            foreach (var nalu in splitter.ParseSample(file))
            {
                if (nalu.Count < 2)
                    continue;

                capture.Fields = [];
                using var stream = new ItuStream(new MemoryStream(nalu.Array!, nalu.Offset, nalu.Count), capture);
                string title = "unreadable";
                Exception? error = null;
                try
                {
                    title = parse(nalu, stream);
                }
                catch (Exception ex)
                {
                    // Read on, as a decoder skips a unit it cannot read: ffmpeg gives up on some
                    // too, and whether that one is among them is for the comparison to say.
                    error = ex;
                }

                // Whatever was read before a failure is kept: it shows where the reading went.
                var unit = new TracedUnit(title) { Error = error };
                unit.Fields.AddRange(capture.Fields);
                units.Add(unit);
            }
        }
        catch (Exception ex)
        {
            return (units, ex);
        }

        return (units, null);
    }

    /// <summary>
    /// Reads an AV1 stream OBU by OBU: an IVF file a temporal unit per frame, anything else as the
    /// OBUs of the low overhead format end to end.
    /// </summary>
    public static (List<TracedUnit> Units, Exception? Error) ReadAv1(string path)
    {
        var capture = new FieldCapture();
        var units = new List<TracedUnit>();
        // ffmpeg reads every layer; a decoder only those of the operating point it chose. And it
        // rejects what a conforming stream cannot contain, which Argon's error sets are full of.
        var context = new SharpAV1.AV1Context { AllLayers = 1, Strict = true };

        // After an OBU it cannot read, ffmpeg drops the rest of its packet - the temporal unit -
        // and reads on with the next; so does this, skipping to the next temporal delimiter.
        bool skipping = false;

        try
        {
            bool annexB = IsAnnexB(path);
            foreach (var temporalUnit in Path.GetExtension(path).Equals(".ivf", StringComparison.OrdinalIgnoreCase)
                ? IvfFrames(path)
                : annexB ? AnnexBObus(path) : [File.ReadAllBytes(path)])
            {
                int offset = 0;

                while (offset < temporalUnit.Length)
                {
                    int left = temporalUnit.Length - offset;
                    if (skipping)
                    {
                        if (((temporalUnit[offset] >> 3) & 0xf) != SharpAV1.AV1ObuTypes.OBU_TEMPORAL_DELIMITER)
                        {
                            offset += ObuLength(temporalUnit, offset);
                            continue;
                        }

                        skipping = false;
                    }

                    // Each OBU is read from where it starts, whatever the one before left.
                    capture.Fields = [];
                    using var reader = new SharpAVX.AomStream(new MemoryStream(temporalUnit, offset, left), capture);
                    Exception? error = null;
                    try
                    {
                        context.Read(reader, left);
                    }
                    catch (Exception ex)
                    {
                        error = ex;
                    }

                    var unit = new TracedUnit($"obu_type {context._ObuType}") { Error = error };
                    unit.Fields.AddRange(capture.Fields);
                    units.Add(unit);

                    if (error != null)
                    {
                        skipping = true;
                        offset += ObuLength(temporalUnit, offset);
                        continue;
                    }

                    int header = 1 + (context._ObuExtensionFlag != 0 ? 1 : 0) + (context.ObuSizeLen >> 3);
                    int size = context._ObuHasSizeField != 0 ? header + context._ObuSize : left;

                    // A frame header is kept for the redundant copies of it that may follow - that
                    // of a frame OBU as well, whose tile group the copy does not read.
                    if (context._ObuType is SharpAV1.AV1ObuTypes.OBU_FRAME_HEADER or SharpAV1.AV1ObuTypes.OBU_FRAME)
                        context.LastObuFrameHeader = temporalUnit.AsSpan(offset + header, context._ObuSize).ToArray();

                    if (size <= 0)
                        throw new InvalidDataException($"an OBU of {size} bytes at {offset}");

                    offset += size;
                }
            }
        }
        catch (Exception ex)
        {
            return (units, ex);
        }

        return (units, null);
    }

    /// <summary>
    /// Whether a stream is in the length delimited format of Annex B, which Argon Streams uses for
    /// all but its not_annexb sets; everything else here is a sequence of OBUs, each with its size.
    /// </summary>
    public static bool IsAnnexB(string path) =>
        path.Contains("argon", StringComparison.OrdinalIgnoreCase) &&
        !path.Contains("not_annexb", StringComparison.OrdinalIgnoreCase) &&
        Path.GetExtension(path).Equals(".obu", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// A stream of OBUs as IVF, in a temporary file: a frame to each temporal unit, the OBUs in it
    /// as they are - except that an Annex B stream's get obu_has_size_field and an obu_size, their
    /// payloads copied as they are.
    /// </summary>
    public static string ObusToIvf(string path)
    {
        var frames = IsAnnexB(path) ? AnnexBTemporalUnits(path) : TemporalUnits(File.ReadAllBytes(path));

        string output = Path.Combine(Path.GetTempPath(), $"sharpmp4-{Guid.NewGuid():N}.ivf");
        using var file = File.Create(output);
        using var writer = new BinaryWriter(file);
        writer.Write("DKIF"u8);
        writer.Write((ushort)0);
        writer.Write((ushort)32);
        writer.Write("AV01"u8);
        writer.Write((ushort)64);
        writer.Write((ushort)64);
        writer.Write(30u);
        writer.Write(1u);
        writer.Write((uint)frames.Count);
        writer.Write(0u);
        for (int i = 0; i < frames.Count; i++)
        {
            writer.Write((uint)frames[i].Length);
            writer.Write((ulong)i);
            writer.Write(frames[i]);
        }
        return output;
    }

    /// <summary>The length of the OBU at an offset, from its header: to the end without obu_size.</summary>
    private static int ObuLength(byte[] bytes, int offset)
    {
        int header = 1 + ((bytes[offset] >> 2) & 1);
        if ((bytes[offset] & 0x02) == 0)
            return bytes.Length - offset;

        int p = offset + header;
        long size = 0;
        for (int i = 0; i < 8 && p < bytes.Length; i++)
        {
            byte b = bytes[p++];
            size |= (long)(b & 0x7f) << (7 * i);
            if ((b & 0x80) == 0)
                break;
        }
        return (int)Math.Max(1, Math.Min(bytes.Length - offset, p - offset + size));
    }

    /// <summary>The temporal units of a stream of OBUs with their sizes: each starts with a temporal delimiter.</summary>
    private static List<byte[]> TemporalUnits(byte[] bytes)
    {
        var units = new List<byte[]>();
        int start = 0, position = 0;
        while (position < bytes.Length)
        {
            int type = (bytes[position] >> 3) & 0xf;
            if (type == SharpAV1.AV1ObuTypes.OBU_TEMPORAL_DELIMITER && position > start)
            {
                units.Add(bytes[start..position]);
                start = position;
            }

            int header = 1 + ((bytes[position] >> 2) & 1);
            if ((bytes[position] & 0x02) == 0)
                break; // no size: the rest of the stream

            int p = position + header;
            long size = 0;
            for (int i = 0; i < 8 && p < bytes.Length; i++)
            {
                byte b = bytes[p++];
                size |= (long)(b & 0x7f) << (7 * i);
                if ((b & 0x80) == 0)
                    break;
            }
            position = (int)Math.Min(bytes.Length, p + size);
        }
        units.Add(bytes[start..]);
        return units;
    }

    /// <summary>The temporal units of an Annex B stream, their OBUs given sizes.</summary>
    private static List<byte[]> AnnexBTemporalUnits(string path)
    {
        var units = new List<byte[]>();
        var unit = new MemoryStream();
        foreach (var obu in AnnexBObus(path))
        {
            if (((obu[0] >> 3) & 0xf) == SharpAV1.AV1ObuTypes.OBU_TEMPORAL_DELIMITER && unit.Length > 0)
            {
                units.Add(unit.ToArray());
                unit = new MemoryStream();
            }

            int header = 1 + ((obu[0] >> 2) & 1);
            if ((obu[0] & 0x02) != 0)
            {
                unit.Write(obu);
                continue;
            }

            unit.WriteByte((byte)(obu[0] | 0x02));
            unit.Write(obu, 1, header - 1);
            int size = obu.Length - header;
            do
            {
                byte b = (byte)(size & 0x7f);
                size >>= 7;
                unit.WriteByte(size > 0 ? (byte)(b | 0x80) : b);
            }
            while (size > 0);
            unit.Write(obu, header, obu.Length - header);
        }
        if (unit.Length > 0)
            units.Add(unit.ToArray());
        return units;
    }

    /// <summary>
    /// The OBUs of an Annex B stream, one at a time: temporal units and frame units behind their
    /// sizes, and each OBU behind its obu_length - which it need not repeat in an obu_size.
    /// </summary>
    private static IEnumerable<byte[]> AnnexBObus(string path)
    {
        var bytes = File.ReadAllBytes(path);
        int position = 0;

        long Leb128()
        {
            long value = 0;
            for (int i = 0; i < 8 && position < bytes.Length; i++)
            {
                byte b = bytes[position++];
                value |= (long)(b & 0x7f) << (7 * i);
                if ((b & 0x80) == 0)
                    break;
            }
            return value;
        }

        while (position < bytes.Length)
        {
            long temporalUnitEnd = Leb128();
            temporalUnitEnd += position;
            while (position < temporalUnitEnd && position < bytes.Length)
            {
                long frameUnitEnd = Leb128();
                frameUnitEnd += position;
                while (position < frameUnitEnd && position < bytes.Length)
                {
                    int length = (int)Leb128();
                    if (length <= 0 || position + length > bytes.Length)
                        throw new InvalidDataException($"an Annex B OBU of {length} bytes at {position}");
                    yield return bytes.AsSpan(position, length).ToArray();
                    position += length;
                }
            }
        }
    }

    /// <summary>The frames of an IVF file: a 32 byte header, then each frame behind its size and time.</summary>
    private static IEnumerable<byte[]> IvfFrames(string path)
    {
        using var file = File.OpenRead(path);
        using var reader = new BinaryReader(file);

        reader.ReadBytes(6);
        int headerLength = reader.ReadUInt16();
        file.Seek(headerLength, SeekOrigin.Begin);

        while (file.Position + 12 <= file.Length)
        {
            int length = reader.ReadInt32();
            reader.ReadInt64();
            yield return reader.ReadBytes(length);
        }
    }

    // H.264: the NAL unit header, then the RBSP its type says.
    private static Func<ArraySegment<byte>, ItuStream, string> H264Parser(H264.H264Context context) => (nalu, stream) =>
    {
        var header = new H264.NalUnit((uint)nalu.Count);
        context.NalHeader = header;
        header.Read(context, stream);

        IItuSerializable? rbsp = header.NalUnitType switch
        {
            H264.H264NALTypes.SLICE or H264.H264NALTypes.IDR_SLICE or H264.H264NALTypes.SLICE_NOPARTITIONING
                => context.SliceLayerWithoutPartitioningRbsp = new H264.SliceLayerWithoutPartitioningRbsp(),
            H264.H264NALTypes.DPA => context.SliceDataPartitionaLayerRbsp = new H264.SliceDataPartitionaLayerRbsp(),
            H264.H264NALTypes.DPB => context.SliceDataPartitionbLayerRbsp = new H264.SliceDataPartitionbLayerRbsp(),
            H264.H264NALTypes.DPC => context.SliceDataPartitioncLayerRbsp = new H264.SliceDataPartitioncLayerRbsp(),
            H264.H264NALTypes.SEI => context.SeiRbsp = new H264.SeiRbsp(),
            H264.H264NALTypes.SPS => context.SeqParameterSetRbsp = new H264.SeqParameterSetRbsp(),
            H264.H264NALTypes.PPS => context.PicParameterSetRbsp = new H264.PicParameterSetRbsp(),
            H264.H264NALTypes.AUD => context.AccessUnitDelimiterRbsp = new H264.AccessUnitDelimiterRbsp(),
            H264.H264NALTypes.END_OF_SEQUENCE => context.EndOfSeqRbsp = new H264.EndOfSeqRbsp(),
            H264.H264NALTypes.END_OF_STREAM => context.EndOfStreamRbsp = new H264.EndOfStreamRbsp(),
            H264.H264NALTypes.FILLER_DATA => context.FillerDataRbsp = new H264.FillerDataRbsp(),
            H264.H264NALTypes.SPS_EXT => context.SeqParameterSetExtensionRbsp = new H264.SeqParameterSetExtensionRbsp(),
            H264.H264NALTypes.PREFIX_NAL => context.PrefixNalUnitRbsp = new H264.PrefixNalUnitRbsp(),
            H264.H264NALTypes.SUBSET_SPS => context.SubsetSeqParameterSetRbsp = new H264.SubsetSeqParameterSetRbsp(),
            H264.H264NALTypes.DPS => context.DepthParameterSetRbsp = new H264.DepthParameterSetRbsp(),
            H264.H264NALTypes.SLICE_EXT or H264.H264NALTypes.SLICE_EXT_VIEW_COMPONENT
                => context.SliceLayerExtensionRbsp = new H264.SliceLayerExtensionRbsp(),
            _ => null,
        };

        rbsp?.Read(context, stream);
        return $"nal_unit_type {header.NalUnitType}";
    };

    // H.265: the same, with its two byte NAL unit header.
    private static Func<ArraySegment<byte>, ItuStream, string> H265Parser(H265.H265Context context) => (nalu, stream) =>
    {
        var header = new H265.NalUnit((uint)nalu.Count);
        context.NalHeader = header;
        header.Read(context, stream);
        uint type = header.NalUnitHeader.NalUnitType;

        // Decoders ignore NAL units with nuh_layer_id 63 (7.4.2.2), as ffmpeg does.
        if (header.NalUnitHeader.NuhLayerId == 63)
            return $"nal_unit_type {type}";

        IItuSerializable? rbsp = type switch
        {
            <= H265.H265NALTypes.CRA_NUT when type <= H265.H265NALTypes.RASL_R || type >= H265.H265NALTypes.BLA_W_LP
                => context.SliceSegmentLayerRbsp = new H265.SliceSegmentLayerRbsp(),
            H265.H265NALTypes.VPS_NUT => context.VideoParameterSetRbsp = new H265.VideoParameterSetRbsp(),
            H265.H265NALTypes.SPS_NUT => context.SeqParameterSetRbsp = new H265.SeqParameterSetRbsp(),
            H265.H265NALTypes.PPS_NUT => context.PicParameterSetRbsp = new H265.PicParameterSetRbsp(),
            H265.H265NALTypes.AUD_NUT => context.AccessUnitDelimiterRbsp = new H265.AccessUnitDelimiterRbsp(),
            H265.H265NALTypes.EOS_NUT => context.EndOfSeqRbsp = new H265.EndOfSeqRbsp(),
            H265.H265NALTypes.EOB_NUT => context.EndOfBitstreamRbsp = new H265.EndOfBitstreamRbsp(),
            H265.H265NALTypes.FD_NUT => context.FillerDataRbsp = new H265.FillerDataRbsp(),
            H265.H265NALTypes.PREFIX_SEI_NUT or H265.H265NALTypes.SUFFIX_SEI_NUT => context.SeiRbsp = new H265.SeiRbsp(),
            _ => null,
        };

        rbsp?.Read(context, stream);
        return $"nal_unit_type {type}";
    };

    // H.266: the same, with the picture header and adaptation parameter sets of its own.
    private static Func<ArraySegment<byte>, ItuStream, string> H266Parser(H266.H266Context context) => (nalu, stream) =>
    {
        var header = new H266.NalUnit((uint)nalu.Count);
        context.NalHeader = header;
        header.Read(context, stream);
        uint type = header.NalUnitHeader.NalUnitType;

        IItuSerializable? rbsp = type switch
        {
            <= H266.H266NALTypes.GDR_NUT => context.SliceLayerRbsp = new H266.SliceLayerRbsp(),
            H266.H266NALTypes.OPI_NUT => context.OperatingPointInformationRbsp = new H266.OperatingPointInformationRbsp(),
            H266.H266NALTypes.DCI_NUT => context.DecodingCapabilityInformationRbsp = new H266.DecodingCapabilityInformationRbsp(),
            H266.H266NALTypes.VPS_NUT => context.VideoParameterSetRbsp = new H266.VideoParameterSetRbsp(),
            H266.H266NALTypes.SPS_NUT => context.SeqParameterSetRbsp = new H266.SeqParameterSetRbsp(),
            H266.H266NALTypes.PPS_NUT => context.PicParameterSetRbsp = new H266.PicParameterSetRbsp(),
            H266.H266NALTypes.PREFIX_APS_NUT or H266.H266NALTypes.SUFFIX_APS_NUT
                => context.AdaptationParameterSetRbsp = new H266.AdaptationParameterSetRbsp(),
            H266.H266NALTypes.PH_NUT => context.PictureHeaderRbsp = new H266.PictureHeaderRbsp(),
            H266.H266NALTypes.AUD_NUT => context.AccessUnitDelimiterRbsp = new H266.AccessUnitDelimiterRbsp(),
            H266.H266NALTypes.EOS_NUT => context.EndOfSeqRbsp = new H266.EndOfSeqRbsp(),
            H266.H266NALTypes.EOB_NUT => context.EndOfBitstreamRbsp = new H266.EndOfBitstreamRbsp(),
            H266.H266NALTypes.PREFIX_SEI_NUT or H266.H266NALTypes.SUFFIX_SEI_NUT => context.SeiRbsp = new H266.SeiRbsp(),
            H266.H266NALTypes.FD_NUT => context.FillerDataRbsp = new H266.FillerDataRbsp(),
            _ => null,
        };

        rbsp?.Read(context, stream);
        return $"nal_unit_type {type}";
    };

    /// <summary>
    /// Takes the syntax elements ItuStream reports as it reads them. It reports each as a line -
    /// its depth in dashes, the name, the bits it took and the value - and the structures around
    /// them the same way, whose values are objects rather than numbers, and so are left out.
    /// </summary>
    private sealed partial class FieldCapture : IMp4Logger
    {
        [GeneratedRegex(@"^-* (\S+) +(\d+)   (-?\d+|byte\[\])$")]
        private static partial Regex FieldLine();

        public List<TracedField> Fields { get; set; } = [];

        public bool IsErrorEnabled { get; set; }
        public bool IsWarningEnabled { get; set; }
        public bool IsInfoEnabled { get; set; } = true;
        public bool IsDebugEnabled { get; set; }
        public bool IsTraceEnabled { get; set; }

        public void LogInfo(string info)
        {
            var match = FieldLine().Match(info);
            if (match.Success)
                Fields.Add(new TracedField(match.Groups[1].Value, int.Parse(match.Groups[2].Value),
                    // A run of bytes read as one, logged without its value.
                    match.Groups[3].Value == "byte[]" ? TracedField.Opaque : long.Parse(match.Groups[3].Value)));
        }

        public void LogError(string error) { }
        public void LogWarning(string warning) { }
        public void LogDebug(string debug) { }
        public void LogTrace(string trace) { }
    }
}
