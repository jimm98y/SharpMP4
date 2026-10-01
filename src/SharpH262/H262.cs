using SharpH26X;
using System;
using System.Collections.Generic;

namespace SharpH262
{
    /// <summary>The start code values (H.262 Table 6-1): the byte after the start code prefix 0x000001.</summary>
    public static class H262StartCodes
    {
        public const int PICTURE = 0x00;
        public const int SLICE_MIN = 0x01;
        public const int SLICE_MAX = 0xAF;
        public const int USER_DATA = 0xB2;
        public const int SEQUENCE_HEADER = 0xB3;
        public const int SEQUENCE_ERROR = 0xB4;
        public const int EXTENSION = 0xB5;
        public const int SEQUENCE_END = 0xB7;
        public const int GROUP = 0xB8;
    }

    /// <summary>The extension_start_code_identifier values (H.262 Table 6-2).</summary>
    public static class H262ExtensionIds
    {
        public const int SEQUENCE = 1;
        public const int SEQUENCE_DISPLAY = 2;
        public const int QUANT_MATRIX = 3;
        public const int COPYRIGHT = 4;
        public const int SEQUENCE_SCALABLE = 5;
        public const int PICTURE_DISPLAY = 7;
        public const int PICTURE_CODING = 8;
        public const int PICTURE_SPATIAL_SCALABLE = 9;
        public const int PICTURE_TEMPORAL_SCALABLE = 10;
        public const int CAMERA_PARAMETERS = 11;
        public const int ITU_T = 12;
    }

    /// <summary>A unit of the stream the syntax does not describe, from its start code to the next, kept as it is.</summary>
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
    /// sequence_end_code (6.2.2): the unit that ends a video sequence, and what follows it in the unit - nothing, in a
    /// stream as the syntax has it.
    /// </summary>
    public sealed class SequenceEnd : IItuSerializable
    {
        private uint sequence_end_code;
        public uint SequenceEndCode { get { return sequence_end_code; } set { sequence_end_code = value; } }
        public NextStartCode NextStartCode { get; set; }
        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = stream.ReadUnsignedInt(0, 32, out sequence_end_code, "sequence_end_code");
            NextStartCode = new NextStartCode();
            return size + stream.ReadClass(size, context, NextStartCode, "next_start_code");
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = stream.WriteUnsignedInt(32, sequence_end_code, "sequence_end_code");
            return size + stream.WriteClass(context, NextStartCode, "next_start_code");
        }
    }

    /// <summary>
    /// next_start_code( ) (5.3): the bits of 0 to the next byte, and the bytes of 0 before the next start code - of a unit
    /// read on its own, all it has left, kept as they are, so that it is written as it was.
    /// </summary>
    public sealed class NextStartCode : IItuSerializable
    {
        /// <summary>The bits read to the next byte, and how many.</summary>
        public int AlignmentBits { get; set; }
        public int AlignmentBitCount { get; set; }

        /// <summary>The bytes after them to the end of the unit: zero_byte of 0, in a stream as the syntax has it.</summary>
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
    /// The macroblocks of a slice (6.2.4), which are not read: the rest of the unit, taken as it is - the bits to the next
    /// byte, then the bytes.
    /// </summary>
    public sealed class SliceData : IItuSerializable
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
    /// The state H.262's units are read in - what one unit says that the syntax of another depends on - and the reading of a
    /// stream unit by unit: each from its start code to the next, as ffmpeg's reader takes them.
    /// </summary>
    public partial class H262Context
    {
        public SequenceHeader SequenceHeader { get; set; }
        public SequenceExtension SequenceExtension { get; set; }
        public SequenceScalableExtension SequenceScalableExtension { get; set; }
        public PictureHeader PictureHeader { get; set; }
        public PictureCodingExtension PictureCodingExtension { get; set; }

        /// <summary>What extension_data( i ) may be, of an extension unit: 0 after the sequence extension, 2 after the picture coding extension.</summary>
        public uint ExtensionsAfter { get; private set; }

        // Of the content description data of a picture header, what its parts read that it read before them
        public uint CountingType { get; set; }
        public uint DataLength { get; set; }
        public uint DataType { get; set; }

        /// <summary>vertical_size (6.3.3): vertical_size_value, and vertical_size_extension above it.</summary>
        public uint VerticalSize => ((SequenceExtension?.VerticalSizeExtension ?? 0) << 12) | (SequenceHeader?.VerticalSizeValue ?? 0);

        /// <summary>horizontal_size (6.3.3): horizontal_size_value, and horizontal_size_extension above it.</summary>
        public uint HorizontalSize => ((SequenceExtension?.HorizontalSizeExtension ?? 0) << 12) | (SequenceHeader?.HorizontalSizeValue ?? 0);

        /// <summary>number_of_frame_centre_offsets (6.3.12), by the sequence extension and the last picture coding extension.</summary>
        public int NumberOfFrameCentreOffsets
        {
            get
            {
                var picture = PictureCodingExtension;
                if (picture == null)
                    return 1;
                if ((SequenceExtension?.ProgressiveSequence ?? 0) == 1)
                    return picture.RepeatFirstField == 1 ? (picture.TopFieldFirst == 1 ? 3 : 2) : 1;
                // a field picture's picture_structure is 1 or 2, a frame's 3
                if (picture.PictureStructure != 3)
                    return 1;
                return picture.RepeatFirstField == 1 ? 3 : 2;
            }
        }

        /// <summary>nextbits( ): so many bits ahead, looked at and not read; -1 where the stream ends first.</summary>
        public long NextBits(ItuStream stream, int count)
        {
            var state = stream.Bitstream.BeginPeek();
            try
            {
                long value = 0;
                for (int i = 0; i < count; i++)
                {
                    int bit = stream.Bitstream.ReadBit();
                    if (bit < 0)
                        return -1;
                    value = (value << 1) | (long)bit;
                }
                return value;
            }
            finally
            {
                stream.Bitstream.EndPeek(state);
            }
        }

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

        /// <summary>A stream to read a unit with: its bits as they are, H.262 having no emulation prevention bytes.</summary>
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
        /// the next, the bytes of 0 before that one next_start_code( )'s: bytes before the first start code, which no unit
        /// has, are left out.
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
        /// A unit the syntax does not describe is kept as its bytes.
        /// </summary>
        public IItuSerializable ReadUnit(ItuStream stream)
        {
            long code = NextBits(stream, 32);
            if (code < 0 || (code >> 8) != 1)
                throw new InvalidOperationException("A unit starts with a start code: 0x000001 and its value.");

            IItuSerializable unit = UnitOf((int)(code & 0xFF), stream);
            stream.ReadClass(0, this, unit, "");
            Keep(unit);
            return unit;
        }

        /// <summary>Writes a unit read with <see cref="ReadUnit"/>, keeping what the units after it depend on as reading does.</summary>
        public void WriteUnit(ItuStream stream, IItuSerializable unit)
        {
            stream.WriteClass(this, unit, "");
            Keep(unit);
        }

        private IItuSerializable UnitOf(int code, ItuStream stream)
        {
            switch (code)
            {
                case H262StartCodes.SEQUENCE_HEADER:
                    return new SequenceHeader();
                case H262StartCodes.GROUP:
                    return new GroupOfPicturesHeader();
                case H262StartCodes.PICTURE:
                    return new PictureHeader();
                case H262StartCodes.USER_DATA:
                    return new UserData();
                case H262StartCodes.SEQUENCE_END:
                    return new SequenceEnd();
                case H262StartCodes.EXTENSION:
                    // the identifier after the start code: the sequence and picture coding extensions read their own start
                    // code; the others are read by extension_data( i ) of what they follow
                    long id = NextBits(stream, 36) & 0xF;
                    if (id == H262ExtensionIds.SEQUENCE)
                        return new SequenceExtension();
                    if (id == H262ExtensionIds.PICTURE_CODING)
                        return new PictureCodingExtension();
                    return new ExtensionData(ExtensionsAfter);
                default:
                    if (code >= H262StartCodes.SLICE_MIN && code <= H262StartCodes.SLICE_MAX)
                        return new Slice();
                    return new UnknownUnit();
            }
        }

        private void Keep(IItuSerializable unit)
        {
            switch (unit)
            {
                case SequenceHeader header:
                    SequenceHeader = header;
                    // a sequence header of ISO/IEC 11172-2 has no extension after it
                    SequenceExtension = null;
                    SequenceScalableExtension = null;
                    break;
                case SequenceExtension sequence:
                    SequenceExtension = sequence;
                    ExtensionsAfter = 0;
                    break;
                case ExtensionData extension when extension.SequenceScalableExtension != null:
                    SequenceScalableExtension = extension.SequenceScalableExtension;
                    break;
                case GroupOfPicturesHeader:
                    ExtensionsAfter = 1;
                    break;
                case PictureHeader picture:
                    PictureHeader = picture;
                    break;
                case PictureCodingExtension coding:
                    PictureCodingExtension = coding;
                    ExtensionsAfter = 2;
                    break;
            }
        }
    }
}
