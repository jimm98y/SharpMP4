using SharpH26X;
using System;

namespace SharpProRes
{
    /// <summary>
    /// Bytes of a frame or picture header past its fields: the header's size counts them, and decoders skip them (RDD 36
    /// 6.1.1, 6.2.1) - a later version of the bitstream may have fields there. Kept as they are, so they are written as they
    /// were.
    /// </summary>
    public abstract class HeaderRemainder : IItuSerializable
    {
        public byte[] Data { get; set; } = Array.Empty<byte>();

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        /// <summary>How many bytes the header has past its fields.</summary>
        protected abstract long Count(ProResContext context);

        public ulong Read(IItuContext context, ItuStream stream)
        {
            long count = Count((ProResContext)context);
            if (count < 0)
                throw new InvalidOperationException($"A ProRes header is {-count} bytes shorter than its fields.");
            Data = ProResContext.ReadBytes(stream, count);
            return (ulong)Data.Length * 8;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            stream.Bitstream.WriteBytes(Data, 0, Data.Length);
            return (ulong)Data.Length * 8;
        }
    }

    /// <summary>What the frame header has past its fields (RDD 36 6.1.1: frame_header_size).</summary>
    public sealed class FrameHeaderRemainder : HeaderRemainder
    {
        protected override long Count(ProResContext context) => context.FrameHeaderRemainderSize;
    }

    /// <summary>What a picture header has past its fields (RDD 36 6.2.1: picture_header_size).</summary>
    public sealed class PictureHeaderRemainder : HeaderRemainder
    {
        protected override long Count(ProResContext context) => context.PictureHeaderRemainderSize;
    }

    /// <summary>
    /// The slices of a picture (RDD 36 5.3), which are not read: the rest of the picture after its slice table, taken as it
    /// is - as many bytes as its picture_size leaves.
    /// </summary>
    public sealed class SliceData : IItuSerializable
    {
        public byte[] Data { get; set; } = Array.Empty<byte>();

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            long count = ((ProResContext)context).SliceDataSize;
            if (count < 0)
                throw new InvalidOperationException($"A ProRes picture's slice table runs {-count} bytes past its picture_size.");
            Data = ProResContext.ReadBytes(stream, count);
            return (ulong)Data.Length * 8;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            stream.Bitstream.WriteBytes(Data, 0, Data.Length);
            return (ulong)Data.Length * 8;
        }
    }

    /// <summary>
    /// The state a ProRes frame is read in - what its frame header and the header of the picture being read say - and what
    /// RDD 36's semantics derive of it: the size of a picture, a field's of an interlaced frame (6.2), its macroblocks and
    /// slices, and the stuffing after the pictures (6.1.2).
    /// </summary>
    public partial class ProResContext
    {
        // The size of the frame header's fields, in bytes - frame_header_size to load_chroma_quantization_matrix - and of a
        // picture header's: what is past them is kept as it is
        private const int FrameHeaderFieldsSize = 20;
        private const int PictureHeaderFieldsSize = 8;

        public uint FrameSize { get; private set; }
        public uint FrameHeaderSize { get; set; }
        public uint HorizontalSize { get; set; }
        public uint VerticalSize { get; set; }
        public uint InterlaceMode { get; set; }
        public uint Log2DesiredSliceSizeInMb { get; set; }

        private int _quantizationMatricesSize;
        private int _picture = -1;
        private uint _pictureHeaderSize;
        private readonly uint[] _pictureSizes = new uint[2];

        /// <summary>frame_size read: a frame begins, of no picture yet.</summary>
        public void OnFrame(uint frameSize)
        {
            FrameSize = frameSize;
            _picture = -1;
            _pictureSizes[0] = _pictureSizes[1] = 0;
        }

        /// <summary>The matrices the frame header loads: 64 bytes each.</summary>
        public void OnQuantizationMatrices(uint loadLuma, uint loadChroma) => _quantizationMatricesSize = 64 * ((int)loadLuma + (int)loadChroma);

        /// <summary>picture_header_size read: the next picture of the frame begins.</summary>
        public void OnPictureHeader(uint pictureHeaderSize)
        {
            _picture++;
            _pictureHeaderSize = pictureHeaderSize;
        }

        public void OnPictureSize(uint pictureSize) => _pictureSizes[Math.Min(_picture, 1)] = pictureSize;

        /// <summary>The bytes of the frame header past its fields.</summary>
        public long FrameHeaderRemainderSize => (long)FrameHeaderSize - FrameHeaderFieldsSize - _quantizationMatricesSize;

        /// <summary>The bytes of the picture header past its fields.</summary>
        public long PictureHeaderRemainderSize => (long)_pictureHeaderSize - PictureHeaderFieldsSize;

        /// <summary>
        /// picture_vertical_size (6.2): of a progressive frame its height; of an interlaced one the field's, the top field
        /// the first of interlace_mode 1, the second of 2.
        /// </summary>
        public uint PictureVerticalSize
        {
            get
            {
                if (InterlaceMode == 0)
                    return VerticalSize;
                bool top = (InterlaceMode == 1 && _picture == 0) || (InterlaceMode == 2 && _picture == 1);
                return top ? (VerticalSize + 1) / 2 : VerticalSize / 2;
            }
        }

        public uint WidthInMb => (HorizontalSize + 15) / 16;

        public uint HeightInMb => (PictureVerticalSize + 15) / 16;

        /// <summary>
        /// number_of_slices_per_mb_row (6.2): slices of the desired size, then of halves of it, until the row is covered.
        /// </summary>
        public uint NumberOfSlicesPerMbRow
        {
            get
            {
                uint count = 0;
                uint sliceSize = 1u << (int)Log2DesiredSliceSizeInMb;
                uint remaining = WidthInMb;
                do
                {
                    while (remaining >= sliceSize)
                    {
                        count++;
                        remaining -= sliceSize;
                    }
                    sliceSize /= 2;
                } while (remaining > 0 && sliceSize > 0);
                return count;
            }
        }

        /// <summary>The bytes of the picture being read after its header and slice table: its slices.</summary>
        public long SliceDataSize => (long)_pictureSizes[Math.Min(_picture, 1)] - _pictureHeaderSize - 2L * HeightInMb * NumberOfSlicesPerMbRow;

        /// <summary>stuffing_size (6.1.2): what the frame has after its headers and pictures.</summary>
        public long StuffingSize
        {
            get
            {
                long data = 4 + 4 + FrameHeaderSize + (long)_pictureSizes[0];
                if (InterlaceMode == 1 || InterlaceMode == 2)
                    data += _pictureSizes[1];
                return Math.Max(0, (long)FrameSize - data);
            }
        }

        /// <summary>So many bytes of the stream, after what was read of it: at a byte, ProRes's headers being of whole bytes.</summary>
        internal static byte[] ReadBytes(ItuStream stream, long count)
        {
            if (count > stream.Bitstream.RemainingBytes)
                throw new InvalidOperationException($"A ProRes frame is cut short: {count} bytes asked for, {stream.Bitstream.RemainingBytes} left.");
            var data = new byte[count];
            int read = stream.Bitstream.ReadBytes(data, 0, data.Length);
            if (read != data.Length)
                throw new InvalidOperationException($"A ProRes frame is cut short: {count} bytes asked for, {read} read.");
            return data;
        }

        /// <summary>A stream to read a frame with: its bytes as they are, ProRes having no emulation prevention bytes.</summary>
        public static ItuStream StreamOf(byte[] buffer, int offset, int length, SharpMP4.Common.IMp4Logger logger = null)
        {
            var stream = new ItuStream(buffer, offset, length, logger);
            stream.Bitstream.SkipPreventionBytes = false;
            stream.Bitstream.InsertPreventionBytes = false;
            return stream;
        }

        /// <summary>A stream to write frames into: their bytes as they are.</summary>
        public static ItuStream StreamOf(System.IO.Stream output, SharpMP4.Common.IMp4Logger logger = null)
        {
            var stream = new ItuStream(output, logger ?? SharpMP4.Common.DefaultMp4Logger.Instance);
            stream.Bitstream.SkipPreventionBytes = false;
            stream.Bitstream.InsertPreventionBytes = false;
            return stream;
        }

        /// <summary>Reads a frame - a sample - to its slices, which are kept as they are.</summary>
        public Frame ReadFrame(ItuStream stream)
        {
            var frame = new Frame();
            stream.ReadClass(0, this, frame, "");
            return frame;
        }

        /// <summary>Writes a frame read with <see cref="ReadFrame"/>.</summary>
        public void WriteFrame(ItuStream stream, Frame frame) => stream.WriteClass(this, frame, "");
    }
}
