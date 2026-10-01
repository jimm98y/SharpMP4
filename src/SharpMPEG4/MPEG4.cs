using SharpH26X;
using System;
using System.Collections.Generic;

namespace SharpMPEG4
{
    /// <summary>The start code values (ISO/IEC 14496-2 Table 6-3): the byte after the start code prefix 0x000001.</summary>
    public static class MPEG4StartCodes
    {
        public const int VIDEO_OBJECT_MIN = 0x00;
        public const int VIDEO_OBJECT_MAX = 0x1F;
        public const int VIDEO_OBJECT_LAYER_MIN = 0x20;
        public const int VIDEO_OBJECT_LAYER_MAX = 0x2F;
        public const int VISUAL_OBJECT_SEQUENCE = 0xB0;
        public const int VISUAL_OBJECT_SEQUENCE_END = 0xB1;
        public const int USER_DATA = 0xB2;
        public const int GROUP_OF_VOP = 0xB3;
        public const int VISUAL_OBJECT = 0xB5;
        public const int VOP = 0xB6;
        public const int SLICE = 0xB7;
        public const int EXTENSION = 0xB8;
        public const int STUFFING = 0xC3;
    }

    /// <summary>The values of vop_coding_type (Table 6-24).</summary>
    public static class MPEG4VopCodingTypes
    {
        public const int I = 0;
        public const int P = 1;
        public const int B = 2;
        public const int S = 3;
    }

    /// <summary>A unit of the stream the syntax here does not describe, from its start code to the next, kept as it is.</summary>
    public sealed class UnknownUnit : IItuSerializable
    {
        public byte[] Data { get; set; } = Array.Empty<byte>();
        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            var data = new byte[stream.Bitstream.RemainingBytes];
            int read = stream.Bitstream.ReadBytes(data, 0, data.Length);
            Array.Resize(ref data, read);
            Data = data;
            return (ulong)read * 8;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            stream.Bitstream.WriteBytes(Data, 0, Data.Length);
            return (ulong)Data.Length * 8;
        }
    }

    /// <summary>
    /// next_start_code( ) (5.2.4): a bit of 0 and the bits of 1 to the next byte, and the bytes of 0 before the next start
    /// code - of a unit read on its own, all it has left, kept as they are, so that it is written as it was.
    /// </summary>
    public sealed class NextStartCode : IItuSerializable
    {
        /// <summary>The bits read to the next byte, and how many.</summary>
        public int AlignmentBits { get; set; }
        public int AlignmentBitCount { get; set; }

        /// <summary>The bytes after them to the end of the unit.</summary>
        public byte[] Stuffing { get; set; } = Array.Empty<byte>();

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;
            AlignmentBits = 0;
            AlignmentBitCount = 0;
            while (!stream.ByteAligned())
            {
                int bit = stream.Bitstream.ReadBit();
                if (bit < 0)
                    break;
                AlignmentBits = (AlignmentBits << 1) | bit;
                AlignmentBitCount++;
                size++;
            }

            var stuffing = new byte[stream.Bitstream.RemainingBytes];
            int read = stream.Bitstream.ReadBytes(stuffing, 0, stuffing.Length);
            Array.Resize(ref stuffing, read);
            Stuffing = stuffing;
            return size + (ulong)read * 8;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            for (int bit = AlignmentBitCount - 1; bit >= 0; bit--)
                stream.Bitstream.WriteBit((AlignmentBits >> bit) & 1);
            if (Stuffing.Length > 0)
                stream.Bitstream.WriteBytes(Stuffing, 0, Stuffing.Length);
            return (ulong)AlignmentBitCount + (ulong)Stuffing.Length * 8;
        }
    }

    /// <summary>
    /// The data of a plane after its header - its macroblocks, and the video packets of them (6.2.5.3) - or of a plane of a
    /// short video header, its GOBs (6.2.5.2): not read, but taken as they are, the bits to the next byte, then the bytes.
    /// </summary>
    public sealed class VopData : IItuSerializable
    {
        public int LeadingBits { get; set; }
        public int LeadingBitCount { get; set; }
        public byte[] Data { get; set; } = Array.Empty<byte>();

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            var bits = new NextStartCode();
            ulong size = bits.Read(context, stream);
            LeadingBits = bits.AlignmentBits;
            LeadingBitCount = bits.AlignmentBitCount;
            Data = bits.Stuffing;
            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream) =>
            new NextStartCode { AlignmentBits = LeadingBits, AlignmentBitCount = LeadingBitCount, Stuffing = Data }.Write(context, stream);
    }

    /// <summary>
    /// A matrix of a video object layer (6.3.3): values of so many bits in zigzag order, as many as there are up to the
    /// 64th, or up to one of 0, which ends it and is not one of them - the values read, the 0 with them where it was.
    /// </summary>
    public abstract class LoadedMatrix : IItuSerializable
    {
        private readonly int _bits;

        protected LoadedMatrix(int bits) { _bits = bits; }

        /// <summary>The values as the stream has them, a 0 that ended the matrix the last.</summary>
        public List<uint> Values { get; set; } = new List<uint>();

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;
            Values = new List<uint>();
            while (Values.Count < 64)
            {
                size += stream.ReadUnsignedInt(size, (ulong)_bits, out uint value, "value");
                Values.Add(value);
                if (value == 0)
                    break;
            }
            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;
            foreach (uint value in Values)
                size += stream.WriteUnsignedInt((ulong)_bits, value, "value");
            return size;
        }
    }

    /// <summary>intra_quant_mat (6.3.3): of 8-bit values.</summary>
    public sealed class IntraQuantMat : LoadedMatrix { public IntraQuantMat() : base(8) { } }

    /// <summary>nonintra_quant_mat (6.3.3): of 8-bit values.</summary>
    public sealed class NonintraQuantMat : LoadedMatrix { public NonintraQuantMat() : base(8) { } }

    /// <summary>intra_quant_mat_grayscale[ i ] (6.3.3): of an auxiliary component, of 8-bit values.</summary>
    public sealed class IntraQuantMatGrayscale : LoadedMatrix { public IntraQuantMatGrayscale(uint component) : base(8) { } }

    /// <summary>nonintra_quant_mat_grayscale[ i ] (6.3.3): of an auxiliary component, of 8-bit values.</summary>
    public sealed class NonintraQuantMatGrayscale : LoadedMatrix { public NonintraQuantMatGrayscale(uint component) : base(8) { } }

    /// <summary>fgs_frequency_weighting_matrix (6.3.3): of 3-bit values.</summary>
    public sealed class FgsFrequencyWeightingMatrix : LoadedMatrix { public FgsFrequencyWeightingMatrix() : base(3) { } }

    /// <summary>fgst_frequency_weighting_matrix (6.3.3): of 3-bit values.</summary>
    public sealed class FgstFrequencyWeightingMatrix : LoadedMatrix { public FgstFrequencyWeightingMatrix() : base(3) { } }

    /// <summary>
    /// sprite_trajectory( ) (6.2.5.4): warping_mv_code( du[ i ] ) and warping_mv_code( dv[ i ] ) of each warping point -
    /// dmv_length, of the variable length code of Table B.34, dmv_code of as many bits as it says, and a marker bit.
    /// </summary>
    public sealed class SpriteTrajectory : IItuSerializable
    {
        /// <summary>A warping_mv_code( d ): its dmv_length - the SSS of Table B.34 - its dmv_code, and its marker bit.</summary>
        public struct WarpingMvCode
        {
            public int DmvLength;
            public uint DmvCode;
            public int MarkerBit;
        }

        /// <summary>du[ i ] and dv[ i ] of each point, du the first.</summary>
        public List<(WarpingMvCode Du, WarpingMvCode Dv)> Points { get; set; } = new List<(WarpingMvCode, WarpingMvCode)>();

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            var ituContext = (MPEG4Context)context;
            ulong size = 0;
            Points = new List<(WarpingMvCode, WarpingMvCode)>();
            for (uint i = 0; i < ituContext.VideoObjectLayer.NoOfSpriteWarpingPoints; i++)
            {
                var du = ReadCode(stream, ref size);
                var dv = ReadCode(stream, ref size);
                Points.Add((du, dv));
            }
            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;
            foreach (var (du, dv) in Points)
            {
                size += WriteCode(stream, du);
                size += WriteCode(stream, dv);
            }
            return size;
        }

        // Table B.34: 00 for 0, 010 and 011 for 1 and 2, 100, 101 and 110 for 3 to 5, then a 1 more for each above 5, and a 0
        private static WarpingMvCode ReadCode(ItuStream stream, ref ulong size)
        {
            int prefix = (Bit(stream) << 1) | Bit(stream);
            size += 2;
            int length;
            if (prefix == 0)
                length = 0;
            else
            {
                int third = Bit(stream);
                size++;
                if (prefix == 1)
                    length = 1 + third;
                else if (prefix == 2)
                    length = 3 + third;
                else if (third == 0)
                    length = 5;
                else
                {
                    // 1110 for 6, a 1 more before the 0 for each above it
                    length = 6;
                    while (true)
                    {
                        size++;
                        if (Bit(stream) == 0)
                            break;
                        if (++length > 14)
                            throw new InvalidOperationException("No dmv_length of Table B.34 is of more than eleven 1s.");
                    }
                }
            }

            var code = new WarpingMvCode { DmvLength = length };
            if (length != 0)
                size += stream.ReadUnsignedInt(size, (ulong)length, out code.DmvCode, "dmv_code");
            size += stream.ReadUnsignedInt(size, 1, out uint marker, "marker_bit");
            code.MarkerBit = (int)marker;
            return code;
        }

        private static ulong WriteCode(ItuStream stream, WarpingMvCode code)
        {
            ulong size = 0;
            int length = code.DmvLength;
            if (length == 0)
                size += WriteBits(stream, 0, 2);
            else if (length <= 2)
                size += WriteBits(stream, 0b010 | (uint)(length - 1), 3);
            else if (length <= 5)
                size += WriteBits(stream, (uint)(0b100 + length - 3), 3);
            else
            {
                // 6 is 1110, each length above it a 1 more, 14 of eleven 1s and a 0
                int ones = length - 3;
                for (int i = 0; i < ones; i++)
                    size += WriteBits(stream, 1, 1);
                size += WriteBits(stream, 0, 1);
            }
            if (length != 0)
                size += stream.WriteUnsignedInt((ulong)length, code.DmvCode, "dmv_code");
            size += stream.WriteUnsignedInt(1, (uint)code.MarkerBit, "marker_bit");
            return size;
        }

        internal static int Bit(ItuStream stream)
        {
            int bit = stream.Bitstream.ReadBit();
            if (bit < 0)
                throw new InvalidOperationException("The stream ended in a variable length code.");
            return bit;
        }

        internal static ulong WriteBits(ItuStream stream, uint value, int count)
        {
            for (int bit = count - 1; bit >= 0; bit--)
                stream.Bitstream.WriteBit((int)((value >> bit) & 1));
            return (ulong)count;
        }
    }

    /// <summary>
    /// brightness_change_factor( ) (6.2.5.4): brightness_change_factor_size, of the variable length code of Table B.35, and
    /// brightness_change_factor_code of the bits it says - 5, 6, 7, 9 or 10.
    /// </summary>
    public sealed class BrightnessChangeFactor : IItuSerializable
    {
        /// <summary>brightness_change_factor_size: the number of 1s of its code, 0 to 4.</summary>
        public int BrightnessChangeFactorSize { get; set; }
        public uint BrightnessChangeFactorCode { get; set; }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        private static readonly int[] CodeBits = { 5, 6, 7, 9, 10 };

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;
            int ones = 0;
            while (ones < 4)
            {
                size++;
                if (SpriteTrajectory.Bit(stream) == 0)
                    break;
                ones++;
            }
            BrightnessChangeFactorSize = ones;
            size += stream.ReadUnsignedInt(size, (ulong)CodeBits[ones], out uint code, "brightness_change_factor_code");
            BrightnessChangeFactorCode = code;
            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;
            for (int i = 0; i < BrightnessChangeFactorSize; i++)
                size += SpriteTrajectory.WriteBits(stream, 1, 1);
            if (BrightnessChangeFactorSize < 4)
                size += SpriteTrajectory.WriteBits(stream, 0, 1);
            return size + stream.WriteUnsignedInt((ulong)CodeBits[BrightnessChangeFactorSize], BrightnessChangeFactorCode, "brightness_change_factor_code");
        }
    }

    /// <summary>
    /// The plane of a static sprite (an S-VOP of sprite_enable "static"), whose sprite pieces are of macroblocks: not read,
    /// so the plane is kept as its bytes.
    /// </summary>
    public sealed class StaticSpriteVop : IItuSerializable
    {
        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }
        public ulong Read(IItuContext context, ItuStream stream) =>
            throw new NotSupportedException("The pieces of a static sprite (ISO/IEC 14496-2 6.2.5.4) are not read.");
        public ulong Write(IItuContext context, ItuStream stream) =>
            throw new NotSupportedException("The pieces of a static sprite (ISO/IEC 14496-2 6.2.5.4) are not written.");
    }

    /// <summary>backward_shape( ) of the plane of an enhancement layer (6.2.5): of macroblocks, not read, so the plane is kept as its bytes.</summary>
    public sealed class BackwardShape : IItuSerializable
    {
        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }
        public ulong Read(IItuContext context, ItuStream stream) =>
            throw new NotSupportedException("The backward shape of an enhancement layer (ISO/IEC 14496-2 6.2.5) is not read.");
        public ulong Write(IItuContext context, ItuStream stream) =>
            throw new NotSupportedException("The backward shape of an enhancement layer (ISO/IEC 14496-2 6.2.5) is not written.");
    }

    /// <summary>forward_shape( ) of the plane of an enhancement layer (6.2.5): of macroblocks, not read, so the plane is kept as its bytes.</summary>
    public sealed class ForwardShape : IItuSerializable
    {
        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }
        public ulong Read(IItuContext context, ItuStream stream) =>
            throw new NotSupportedException("The forward shape of an enhancement layer (ISO/IEC 14496-2 6.2.5) is not read.");
        public ulong Write(IItuContext context, ItuStream stream) =>
            throw new NotSupportedException("The forward shape of an enhancement layer (ISO/IEC 14496-2 6.2.5) is not written.");
    }

    /// <summary>
    /// The state MPEG-4 Visual's units are read in - the visual object and video object layer the planes after them are of -
    /// and the reading of a stream unit by unit: each from its start code to the next.
    /// </summary>
    public partial class MPEG4Context
    {
        /// <summary>The visual object sequence last read.</summary>
        public VisualObjectSequence VisualObjectSequence { get; set; }

        /// <summary>The visual object last read.</summary>
        public VisualObject VisualObject { get; set; }

        /// <summary>The video object layer being read, or last read: the one of the planes after it.</summary>
        public VideoObjectLayer VideoObjectLayer { get; set; }

        /// <summary>The complexity estimation of the video object layer, which the planes' estimates are of.</summary>
        public DefineVopComplexityEstimationHeader ComplexityEstimation { get; set; }

        /// <summary>visual_object_verid (6.3.2): 1 where the visual object has no identifier.</summary>
        public uint VisualObjectVerid { get; set; } = 1;

        /// <summary>video_object_layer_verid (6.3.3): visual_object_verid where the layer has no identifier.</summary>
        public uint VideoObjectLayerVerid { get; set; } = 1;

        /// <summary>vop_coding_type of the plane being read.</summary>
        public uint VopCodingType { get; set; }

        public void OnVisualObject(VisualObject visualObject)
        {
            VisualObject = visualObject;
            VisualObjectVerid = 1;
        }

        public void OnVideoObjectLayer(VideoObjectLayer layer)
        {
            VideoObjectLayer = layer;
            ComplexityEstimation = null;
        }

        public void OnObjectLayerIdentifier(uint isObjectLayerIdentifier)
        {
            if (isObjectLayerIdentifier == 0)
                VideoObjectLayerVerid = VisualObjectVerid;
        }

        /// <summary>
        /// The bits of vop_time_increment and fixed_vop_time_increment (6.3.3): those vop_time_increment_resolution - 1
        /// takes, at least 1.
        /// </summary>
        public ulong VopTimeIncrementBits
        {
            get
            {
                uint resolution = VideoObjectLayer?.VopTimeIncrementResolution ?? 0;
                ulong bits = 1;
                while (resolution > 1 && (1u << (int)bits) < resolution)
                    bits++;
                return bits;
            }
        }

        /// <summary>The bits of vop_id (6.3.5): those of vop_time_increment and 3, at most 15.</summary>
        public ulong VopIdBits => Math.Min(VopTimeIncrementBits + 3, 15);

        /// <summary>The bits of vop_quant (6.3.5): quant_precision, where not_8_bit, else 5.</summary>
        public ulong QuantBits => VideoObjectLayer != null && VideoObjectLayer.Not8Bit != 0 ? VideoObjectLayer.QuantPrecision : 5;

        // Table 6-17: aux_comp_count of each video_object_layer_shape_extension
        private static readonly int[] AuxCompCounts = { 1, 1, 2, 2, 3, 1, 2, 1, 1, 2, 3, 2, 3 };

        /// <summary>aux_comp_count of the layer (Table 6-17): the auxiliary components of its grayscale shape.</summary>
        public int AuxCompCount
        {
            get
            {
                // of a layer of version 1, which has no extension, 0000: the alpha only
                uint extension = VideoObjectLayer?.VideoObjectLayerShapeExtension ?? 0;
                return extension < AuxCompCounts.Length ? AuxCompCounts[extension] : 0;
            }
        }

        /// <summary>
        /// Whether the visual object sequence is of a studio profile (Table G.1, 11100001 to 11101000), whose visual
        /// objects (6.2.13) are not read here: its units are kept as their bytes.
        /// </summary>
        public bool IsStudio => _studioLayer || VisualObjectSequence != null && VisualObjectSequence.ProfileAndLevelIndication >= 0xE1 && VisualObjectSequence.ProfileAndLevelIndication <= 0xE8;

        // Whether the last video object layer is of a studio object type (Table 6-11: Simple Studio, Core Studio), whose
        // header (6.2.13.2) is not the one read here - of a stream without a visual object sequence, the only sign of it
        private bool _studioLayer;

        /// <summary>
        /// Whether a start code prefix follows - or nothing does: a unit is read on its own, to the next start code. Bytes
        /// fewer than a prefix at its end are not one: what reads to the next start code reads them.
        /// </summary>
        public bool AtStartCode(ItuStream stream)
        {
            var state = stream.Bitstream.BeginPeek();
            try
            {
                long value = 0;
                for (int i = 0; i < 24; i++)
                {
                    int bit = stream.Bitstream.ReadBit();
                    if (bit < 0)
                        return i == 0;
                    value = (value << 1) | (long)bit;
                }
                return value == 1;
            }
            finally
            {
                stream.Bitstream.EndPeek(state);
            }
        }

        /// <summary>A stream to read a unit with: its bits as they are, MPEG-4 Visual having no emulation prevention bytes.</summary>
        public static ItuStream StreamOf(byte[] buffer, int offset, int length, SharpMP4.Common.IMp4Logger logger = null)
        {
            var stream = new ItuStream(buffer, offset, length, logger);
            stream.Bitstream.SkipPreventionBytes = false;
            stream.Bitstream.InsertPreventionBytes = false;
            return stream;
        }

        /// <summary>A stream to write units into: their bits as they are.</summary>
        public static ItuStream StreamOf(System.IO.Stream output, SharpMP4.Common.IMp4Logger logger = null)
        {
            var stream = new ItuStream(output, logger ?? SharpMP4.Common.DefaultMp4Logger.Instance);
            stream.Bitstream.SkipPreventionBytes = false;
            stream.Bitstream.InsertPreventionBytes = false;
            return stream;
        }

        /// <summary>
        /// The units of a chunk of the stream - a sample - each where its start code prefix starts and as long as it is to
        /// the next: bytes before the first start code, which no unit has, are left out.
        /// </summary>
        public static List<(int Offset, int Length)> Units(byte[] data, int offset, int length)
        {
            var starts = new List<int>();
            int end = offset + length;
            for (int i = offset; i + 3 <= end; i++)
            {
                if (data[i] == 0 && data[i + 1] == 0 && data[i + 2] == 1)
                {
                    starts.Add(i);
                    i += 2;
                }
            }

            var units = new List<(int, int)>(starts.Count);
            for (int k = 0; k < starts.Count; k++)
            {
                int next = k + 1 < starts.Count ? starts[k + 1] : end;
                units.Add((starts[k], next - starts[k]));
            }
            return units;
        }

        /// <summary>
        /// Reads a unit, from its start code, by what the start code says it is; and keeps what the units after it depend on.
        /// A unit the syntax here does not describe, or has a part of that is not read, is kept as its bytes.
        /// </summary>
        public IItuSerializable ReadUnit(byte[] buffer, int offset, int length, SharpMP4.Common.IMp4Logger logger = null)
        {
            if (length < 4 || buffer[offset] != 0 || buffer[offset + 1] != 0 || buffer[offset + 2] != 1)
                throw new InvalidOperationException("A unit starts with a start code: 0x000001 and its value.");

            int code = buffer[offset + 3];
            if (code >= MPEG4StartCodes.VIDEO_OBJECT_LAYER_MIN && code <= MPEG4StartCodes.VIDEO_OBJECT_LAYER_MAX && length >= 6)
            {
                // video_object_type_indication, after random_accessible_vol
                int type = ((buffer[offset + 4] << 1) | (buffer[offset + 5] >> 7)) & 0xFF;
                _studioLayer = type == 0x0F || type == 0x10;
                if (_studioLayer)
                    VideoObjectLayer = null;
            }

            IItuSerializable unit = UnitOf(code);
            if (unit is not UnknownUnit)
            {
                try
                {
                    StreamOf(buffer, offset, length, logger).ReadClass(0, this, unit, "");
                    Keep(unit);
                    return unit;
                }
                catch (NotSupportedException)
                {
                    // of what is not read: the unit as it is
                }
            }

            unit = new UnknownUnit();
            StreamOf(buffer, offset, length, logger).ReadClass(0, this, unit, "");
            return unit;
        }

        /// <summary>Writes a unit read with <see cref="ReadUnit"/>, keeping what the units after it depend on as reading does.</summary>
        public void WriteUnit(ItuStream stream, IItuSerializable unit)
        {
            stream.WriteClass(this, unit, "");
            Keep(unit);
        }

        private IItuSerializable UnitOf(int code)
        {
            if (IsStudio && code != MPEG4StartCodes.VISUAL_OBJECT_SEQUENCE)
                return new UnknownUnit();

            switch (code)
            {
                case MPEG4StartCodes.VISUAL_OBJECT_SEQUENCE:
                    return new VisualObjectSequence();
                case MPEG4StartCodes.VISUAL_OBJECT_SEQUENCE_END:
                    return new VisualObjectSequenceEnd();
                case MPEG4StartCodes.USER_DATA:
                    return new UserData();
                case MPEG4StartCodes.GROUP_OF_VOP:
                    return new GroupOfVideoObjectPlane();
                case MPEG4StartCodes.VISUAL_OBJECT:
                    return new VisualObject();
                case MPEG4StartCodes.VOP:
                    // the planes of a layer that is read
                    return VideoObjectLayer != null ? new VideoObjectPlane() : new UnknownUnit();
                case MPEG4StartCodes.STUFFING:
                    return new Stuffing();
                default:
                    if (code <= MPEG4StartCodes.VIDEO_OBJECT_MAX)
                        return new VideoObject();
                    if (code >= MPEG4StartCodes.VIDEO_OBJECT_LAYER_MIN && code <= MPEG4StartCodes.VIDEO_OBJECT_LAYER_MAX)
                        return new VideoObjectLayer();
                    return new UnknownUnit();
            }
        }

        private void Keep(IItuSerializable unit)
        {
            switch (unit)
            {
                case VisualObjectSequence sequence:
                    VisualObjectSequence = sequence;
                    break;
            }
        }

        /// <summary>
        /// The planes of a stream of a short video header (6.2.5.2) - of no start codes - each where its
        /// short_video_start_marker starts, byte aligned: 0x0000 and a byte of 100000xx.
        /// </summary>
        public static List<(int Offset, int Length)> ShortHeaderPlanes(byte[] data, int offset, int length)
        {
            var starts = new List<int>();
            int end = offset + length;
            for (int i = offset; i + 3 <= end; i++)
            {
                if (data[i] == 0 && data[i + 1] == 0 && (data[i + 2] & 0xFC) == 0x80)
                {
                    starts.Add(i);
                    i += 2;
                }
            }

            var planes = new List<(int, int)>(starts.Count);
            for (int k = 0; k < starts.Count; k++)
            {
                int next = k + 1 < starts.Count ? starts[k + 1] : end;
                planes.Add((starts[k], next - starts[k]));
            }
            return planes;
        }

        /// <summary>Reads a plane of a short video header: its header, and its GOBs as they are.</summary>
        public VideoPlaneWithShortHeader ReadShortHeaderPlane(ItuStream stream)
        {
            var plane = new VideoPlaneWithShortHeader();
            stream.ReadClass(0, this, plane, "");
            return plane;
        }
    }
}
