using SharpH26X;
using SharpMP4.Common;
using SharpMP4.Tracks;
using H264 = SharpH264;
using H265 = SharpH265;
using H266 = SharpH266;

namespace SharpMP4.Tests.Conformance;

/// <summary>
/// Reads an H.264, H.265 or H.266 stream NAL unit by NAL unit and writes each again with a context of its own - one that
/// has only written - checking the bytes are the stream's, emulation prevention and all.
/// </summary>
/// <remarks>
/// Reading fills in what a unit leaves out and the spec infers; writing must not, or a value that was coded is written
/// over with the one that would have been inferred. Comparing reads alone cannot see that - both sides of a read agree -
/// and a write compared with its own read back cannot either: only the stream's own bytes can.
/// </remarks>
public static class H26xRoundTrip
{
    public static StreamResult Check(string path, string codec)
    {
        var result = new StreamResult { Path = path };
        UnitCodec reader = UnitCodec.Create(codec);
        UnitCodec writer = UnitCodec.Create(codec);

        using var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 16);
        int units = 0;
        string? unreadable = null;
        foreach (var nalu in new H265Track().ParseAnnexB(file))
        {
            if (nalu.Count < 2)
                continue;

            (IItuSerializable Header, IItuSerializable? Rbsp) unit;
            Rest rest;
            try
            {
                using var input = new ItuStream(new MemoryStream(nalu.Array!, nalu.Offset, nalu.Count, writable: false), NullMp4Logger.Instance);
                unit = reader.Read(nalu, input);
                rest = Rest.Read(input, nalu.Count);
            }
            catch (Exception ex)
            {
                // A unit the reading gives up on - whether rightly is for the comparison with ffmpeg to say, not this -
                // cannot be written, and what follows may need it: a parameter set the writing never sees. Compared to
                // here, then.
                unreadable = $"unit {units} could not be read ({ex.GetType().Name}); compared to there";
                break;
            }

            // A unit nothing reads - one of a type no parser has, or a layer decoders ignore - is not written either.
            if (unit.Rbsp == null)
            {
                units++;
                continue;
            }

            string name = unit.Rbsp.GetType().Name;
            var written = new MemoryStream();
            try
            {
                using var output = new ItuStream(written, NullMp4Logger.Instance);
                writer.Write(output, unit.Header, unit.Rbsp);
                rest.Write(output);
                output.EndNalUnit();
            }
            catch (Exception ex)
            {
                result.Fail(Outcome.SharpFailed, $"{name}: write threw {ex.GetType().Name}",
                    $"unit {units}: writing threw: {ex.Message} {ex.StackTrace?.Split('\n').FirstOrDefault()?.Trim()}");
                units++;
                continue;
            }

            byte[] bytes = written.ToArray();
            int differs = FirstDifference(bytes, nalu);
            if (differs >= 0)
            {
                result.Fail(Outcome.Diverged, $"{name}: written differently",
                    $"unit {units} ({name}): written as {bytes.Length} bytes of {nalu.Count}, first differing at byte {differs}");
            }
            units++;
        }

        result.UnitsCompared = units;
        if (result.Keys.Count == 0)
        {
            result.Outcome = unreadable == null ? Outcome.Match : Outcome.NoReference;
            result.Detail = unreadable ?? "";
        }
        return result;
    }

    /// <summary>
    /// What the reading leaves of a unit - a slice's data, which only its header is read of - carried over as it is: the bits
    /// to the next byte boundary, as an H.264 slice header need not end on one, then the bytes.
    /// </summary>
    private sealed class Rest
    {
        private readonly List<byte> _bits = [];
        private byte[] _bytes = [];
        private int _count;

        public static Rest Read(ItuStream input, int storedLength)
        {
            var rest = new Rest();
            while (!input.ByteAligned() && input.Bitstream.BitsPosition < storedLength * 8L)
            {
                input.ReadUnsignedInt(0, 1, out byte bit, "rest");
                rest._bits.Add(bit);
            }

            // The position counts the emulation prevention bytes skipped, so this is what is left as stored - an upper
            // bound on what reads.
            int left = storedLength - (int)(input.Bitstream.BitsPosition / 8);
            if (left > 0)
            {
                rest._bytes = new byte[left];
                rest._count = input.ReadBytes(rest._bytes, 0, left);
            }
            return rest;
        }

        public void Write(ItuStream output)
        {
            foreach (byte bit in _bits)
                output.WriteBit(bit);
            if (_count > 0)
                output.WriteBytes(_bytes, 0, _count);
        }
    }

    /// <summary>The first byte written that is not the stream's, or past what either has; -1 if none.</summary>
    private static int FirstDifference(byte[] written, ArraySegment<byte> nalu)
    {
        int common = Math.Min(written.Length, nalu.Count);
        for (int i = 0; i < common; i++)
        {
            if (written[i] != nalu.Array![nalu.Offset + i])
                return i;
        }
        return written.Length == nalu.Count ? -1 : common;
    }

    /// <summary>
    /// One codec's NAL units, read into a context and written from one: the unit is put where the context keeps the unit in
    /// hand, as reading does, so that writing takes the same branches.
    /// </summary>
    private abstract class UnitCodec
    {
        public static UnitCodec Create(string codec) => codec switch
        {
            "h264" => new H264Units(),
            "h265" => new H265Units(),
            "h266" => new H266Units(),
            _ => throw new ArgumentException($"not an H.26x codec: {codec}", nameof(codec)),
        };

        public abstract (IItuSerializable Header, IItuSerializable? Rbsp) Read(ArraySegment<byte> nalu, ItuStream stream);

        public abstract void Write(ItuStream stream, IItuSerializable header, IItuSerializable rbsp);
    }

    private sealed class H264Units : UnitCodec
    {
        private readonly H264.H264Context _context = new();

        public override (IItuSerializable, IItuSerializable?) Read(ArraySegment<byte> nalu, ItuStream stream)
        {
            var header = new H264.NalUnit((uint)nalu.Count);
            _context.NalHeader = header;
            header.Read(_context, stream);

            IItuSerializable? rbsp = header.NalUnitType switch
            {
                H264.H264NALTypes.SLICE or H264.H264NALTypes.IDR_SLICE or H264.H264NALTypes.SLICE_NOPARTITIONING => new H264.SliceLayerWithoutPartitioningRbsp(),
                H264.H264NALTypes.DPA => new H264.SliceDataPartitionaLayerRbsp(),
                H264.H264NALTypes.DPB => new H264.SliceDataPartitionbLayerRbsp(),
                H264.H264NALTypes.DPC => new H264.SliceDataPartitioncLayerRbsp(),
                H264.H264NALTypes.SEI => new H264.SeiRbsp(),
                H264.H264NALTypes.SPS => new H264.SeqParameterSetRbsp(),
                H264.H264NALTypes.PPS => new H264.PicParameterSetRbsp(),
                H264.H264NALTypes.AUD => new H264.AccessUnitDelimiterRbsp(),
                H264.H264NALTypes.END_OF_SEQUENCE => new H264.EndOfSeqRbsp(),
                H264.H264NALTypes.END_OF_STREAM => new H264.EndOfStreamRbsp(),
                H264.H264NALTypes.FILLER_DATA => new H264.FillerDataRbsp(),
                H264.H264NALTypes.SPS_EXT => new H264.SeqParameterSetExtensionRbsp(),
                H264.H264NALTypes.PREFIX_NAL => new H264.PrefixNalUnitRbsp(),
                H264.H264NALTypes.SUBSET_SPS => new H264.SubsetSeqParameterSetRbsp(),
                H264.H264NALTypes.DPS => new H264.DepthParameterSetRbsp(),
                H264.H264NALTypes.SLICE_EXT or H264.H264NALTypes.SLICE_EXT_VIEW_COMPONENT => new H264.SliceLayerExtensionRbsp(),
                _ => null,
            };

            if (rbsp != null)
            {
                Bind(_context, rbsp);
                rbsp.Read(_context, stream);
            }
            return (header, rbsp);
        }

        public override void Write(ItuStream stream, IItuSerializable header, IItuSerializable rbsp)
        {
            _context.NalHeader = (H264.NalUnit)header;
            header.Write(_context, stream);
            Bind(_context, rbsp);
            rbsp.Write(_context, stream);
        }

        private static void Bind(H264.H264Context context, IItuSerializable rbsp)
        {
            switch (rbsp)
            {
                case H264.SliceLayerWithoutPartitioningRbsp u: context.SliceLayerWithoutPartitioningRbsp = u; break;
                case H264.SliceDataPartitionaLayerRbsp u: context.SliceDataPartitionaLayerRbsp = u; break;
                case H264.SliceDataPartitionbLayerRbsp u: context.SliceDataPartitionbLayerRbsp = u; break;
                case H264.SliceDataPartitioncLayerRbsp u: context.SliceDataPartitioncLayerRbsp = u; break;
                case H264.SeiRbsp u: context.SeiRbsp = u; break;
                case H264.SeqParameterSetRbsp u: context.SeqParameterSetRbsp = u; break;
                case H264.PicParameterSetRbsp u: context.PicParameterSetRbsp = u; break;
                case H264.AccessUnitDelimiterRbsp u: context.AccessUnitDelimiterRbsp = u; break;
                case H264.EndOfSeqRbsp u: context.EndOfSeqRbsp = u; break;
                case H264.EndOfStreamRbsp u: context.EndOfStreamRbsp = u; break;
                case H264.FillerDataRbsp u: context.FillerDataRbsp = u; break;
                case H264.SeqParameterSetExtensionRbsp u: context.SeqParameterSetExtensionRbsp = u; break;
                case H264.PrefixNalUnitRbsp u: context.PrefixNalUnitRbsp = u; break;
                case H264.SubsetSeqParameterSetRbsp u: context.SubsetSeqParameterSetRbsp = u; break;
                case H264.DepthParameterSetRbsp u: context.DepthParameterSetRbsp = u; break;
                case H264.SliceLayerExtensionRbsp u: context.SliceLayerExtensionRbsp = u; break;
            }
        }
    }

    private sealed class H265Units : UnitCodec
    {
        private readonly H265.H265Context _context = new();

        public override (IItuSerializable, IItuSerializable?) Read(ArraySegment<byte> nalu, ItuStream stream)
        {
            var header = new H265.NalUnit((uint)nalu.Count);
            _context.NalHeader = header;
            header.Read(_context, stream);
            uint type = header.NalUnitHeader.NalUnitType;

            // Decoders ignore NAL units with nuh_layer_id 63 (7.4.2.2).
            if (header.NalUnitHeader.NuhLayerId == 63)
                return (header, null);

            IItuSerializable? rbsp = type switch
            {
                <= H265.H265NALTypes.CRA_NUT when type <= H265.H265NALTypes.RASL_R || type >= H265.H265NALTypes.BLA_W_LP => new H265.SliceSegmentLayerRbsp(),
                H265.H265NALTypes.VPS_NUT => new H265.VideoParameterSetRbsp(),
                H265.H265NALTypes.SPS_NUT => new H265.SeqParameterSetRbsp(),
                H265.H265NALTypes.PPS_NUT => new H265.PicParameterSetRbsp(),
                H265.H265NALTypes.AUD_NUT => new H265.AccessUnitDelimiterRbsp(),
                H265.H265NALTypes.EOS_NUT => new H265.EndOfSeqRbsp(),
                H265.H265NALTypes.EOB_NUT => new H265.EndOfBitstreamRbsp(),
                H265.H265NALTypes.FD_NUT => new H265.FillerDataRbsp(),
                H265.H265NALTypes.PREFIX_SEI_NUT or H265.H265NALTypes.SUFFIX_SEI_NUT => new H265.SeiRbsp(),
                _ => null,
            };

            if (rbsp != null)
            {
                Bind(_context, rbsp);
                rbsp.Read(_context, stream);
            }
            return (header, rbsp);
        }

        public override void Write(ItuStream stream, IItuSerializable header, IItuSerializable rbsp)
        {
            _context.NalHeader = (H265.NalUnit)header;
            header.Write(_context, stream);
            Bind(_context, rbsp);
            rbsp.Write(_context, stream);
        }

        private static void Bind(H265.H265Context context, IItuSerializable rbsp)
        {
            switch (rbsp)
            {
                case H265.SliceSegmentLayerRbsp u: context.SliceSegmentLayerRbsp = u; break;
                case H265.VideoParameterSetRbsp u: context.VideoParameterSetRbsp = u; break;
                case H265.SeqParameterSetRbsp u: context.SeqParameterSetRbsp = u; break;
                case H265.PicParameterSetRbsp u: context.PicParameterSetRbsp = u; break;
                case H265.AccessUnitDelimiterRbsp u: context.AccessUnitDelimiterRbsp = u; break;
                case H265.EndOfSeqRbsp u: context.EndOfSeqRbsp = u; break;
                case H265.EndOfBitstreamRbsp u: context.EndOfBitstreamRbsp = u; break;
                case H265.FillerDataRbsp u: context.FillerDataRbsp = u; break;
                case H265.SeiRbsp u: context.SeiRbsp = u; break;
            }
        }
    }

    private sealed class H266Units : UnitCodec
    {
        private readonly H266.H266Context _context = new();

        public override (IItuSerializable, IItuSerializable?) Read(ArraySegment<byte> nalu, ItuStream stream)
        {
            var header = new H266.NalUnit((uint)nalu.Count);
            _context.NalHeader = header;
            header.Read(_context, stream);
            uint type = header.NalUnitHeader.NalUnitType;

            IItuSerializable? rbsp = type switch
            {
                <= H266.H266NALTypes.GDR_NUT => new H266.SliceLayerRbsp(),
                H266.H266NALTypes.OPI_NUT => new H266.OperatingPointInformationRbsp(),
                H266.H266NALTypes.DCI_NUT => new H266.DecodingCapabilityInformationRbsp(),
                H266.H266NALTypes.VPS_NUT => new H266.VideoParameterSetRbsp(),
                H266.H266NALTypes.SPS_NUT => new H266.SeqParameterSetRbsp(),
                H266.H266NALTypes.PPS_NUT => new H266.PicParameterSetRbsp(),
                H266.H266NALTypes.PREFIX_APS_NUT or H266.H266NALTypes.SUFFIX_APS_NUT => new H266.AdaptationParameterSetRbsp(),
                H266.H266NALTypes.PH_NUT => new H266.PictureHeaderRbsp(),
                H266.H266NALTypes.AUD_NUT => new H266.AccessUnitDelimiterRbsp(),
                H266.H266NALTypes.EOS_NUT => new H266.EndOfSeqRbsp(),
                H266.H266NALTypes.EOB_NUT => new H266.EndOfBitstreamRbsp(),
                H266.H266NALTypes.PREFIX_SEI_NUT or H266.H266NALTypes.SUFFIX_SEI_NUT => new H266.SeiRbsp(),
                H266.H266NALTypes.FD_NUT => new H266.FillerDataRbsp(),
                _ => null,
            };

            if (rbsp != null)
            {
                Bind(_context, rbsp);
                rbsp.Read(_context, stream);
            }
            return (header, rbsp);
        }

        public override void Write(ItuStream stream, IItuSerializable header, IItuSerializable rbsp)
        {
            _context.NalHeader = (H266.NalUnit)header;
            header.Write(_context, stream);
            Bind(_context, rbsp);
            rbsp.Write(_context, stream);
        }

        private static void Bind(H266.H266Context context, IItuSerializable rbsp)
        {
            switch (rbsp)
            {
                case H266.SliceLayerRbsp u: context.SliceLayerRbsp = u; break;
                case H266.OperatingPointInformationRbsp u: context.OperatingPointInformationRbsp = u; break;
                case H266.DecodingCapabilityInformationRbsp u: context.DecodingCapabilityInformationRbsp = u; break;
                case H266.VideoParameterSetRbsp u: context.VideoParameterSetRbsp = u; break;
                case H266.SeqParameterSetRbsp u: context.SeqParameterSetRbsp = u; break;
                case H266.PicParameterSetRbsp u: context.PicParameterSetRbsp = u; break;
                case H266.AdaptationParameterSetRbsp u: context.AdaptationParameterSetRbsp = u; break;
                case H266.PictureHeaderRbsp u: context.PictureHeaderRbsp = u; break;
                case H266.AccessUnitDelimiterRbsp u: context.AccessUnitDelimiterRbsp = u; break;
                case H266.EndOfSeqRbsp u: context.EndOfSeqRbsp = u; break;
                case H266.EndOfBitstreamRbsp u: context.EndOfBitstreamRbsp = u; break;
                case H266.SeiRbsp u: context.SeiRbsp = u; break;
                case H266.FillerDataRbsp u: context.FillerDataRbsp = u; break;
            }
        }
    }
}
