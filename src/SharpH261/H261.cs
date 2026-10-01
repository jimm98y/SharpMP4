using SharpH26X;
using System;
using System.Collections.Generic;

namespace SharpH261
{
    /// <summary>
    /// The data of a picture after its header (4.2.1, Figure 5): its GOBs, to the next picture start code - not read, but
    /// taken as they are: the bits to the next byte, then the bytes.
    /// </summary>
    public sealed class PictureData : IItuSerializable
    {
        public int LeadingBits { get; set; }
        public int LeadingBitCount { get; set; }
        public byte[] Data { get; set; } = Array.Empty<byte>();

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;
            LeadingBits = 0;
            LeadingBitCount = 0;
            while (!stream.ByteAligned())
            {
                int bit = stream.Bitstream.ReadBit();
                if (bit < 0)
                    break;
                LeadingBits = (LeadingBits << 1) | bit;
                LeadingBitCount++;
                size++;
            }

            var data = new byte[stream.Bitstream.RemainingBytes];
            int read = stream.Bitstream.ReadBytes(data, 0, data.Length);
            Array.Resize(ref data, read);
            Data = data;
            return size + (ulong)read * 8;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            for (int bit = LeadingBitCount - 1; bit >= 0; bit--)
                stream.Bitstream.WriteBit((LeadingBits >> bit) & 1);
            if (Data.Length > 0)
                stream.Bitstream.WriteBytes(Data, 0, Data.Length);
            return (ulong)LeadingBitCount + (ulong)Data.Length * 8;
        }
    }

    /// <summary>
    /// The state H.261's pictures are read in - the picture format of the last header - and the reading of a stream picture
    /// by picture, each from its picture start code to the next.
    /// </summary>
    public partial class H261Context
    {
        /// <summary>The width and height of the pictures, in samples: QCIF's or CIF's (4.2.1.3, 3.1).</summary>
        public int Width { get; private set; }
        public int Height { get; private set; }

        /// <summary>PTYPE bit 4 read: QCIF where 0, CIF where 1.</summary>
        public void OnSourceFormat(uint sourceFormat) => (Width, Height) = sourceFormat == 1 ? (352, 288) : (176, 144);

        /// <summary>A stream to read a picture with: its bits as they are, H.261 having no emulation prevention bytes.</summary>
        public static ItuStream StreamOf(byte[] buffer, int offset, int length, SharpMP4.Common.IMp4Logger logger = null)
        {
            var stream = new ItuStream(buffer, offset, length, logger);
            stream.Bitstream.SkipPreventionBytes = false;
            stream.Bitstream.InsertPreventionBytes = false;
            return stream;
        }

        /// <summary>A stream to write pictures into: their bits as they are.</summary>
        public static ItuStream StreamOf(System.IO.Stream output, SharpMP4.Common.IMp4Logger logger = null)
        {
            var stream = new ItuStream(output, logger ?? SharpMP4.Common.DefaultMp4Logger.Instance);
            stream.Bitstream.SkipPreventionBytes = false;
            stream.Bitstream.InsertPreventionBytes = false;
            return stream;
        }

        /// <summary>
        /// The pictures of a chunk of the stream, each where its picture start code starts and as long as it is to the next.
        /// The start code (4.2.1.1) need not be byte aligned in the syntax, but a picture is taken where it is - as ffmpeg's
        /// encoder writes it and a sample of a file has it: 0x0001 and a byte whose upper bits are 0000, of the group number
        /// 0, which no group of blocks start code has.
        /// </summary>
        public static List<(int Offset, int Length)> Pictures(byte[] data, int offset, int length)
        {
            var starts = new List<int>();
            int end = offset + length;
            for (int i = offset; i + 3 <= end; i++)
            {
                if (data[i] == 0 && data[i + 1] == 1 && (data[i + 2] & 0xF0) == 0)
                {
                    starts.Add(i);
                    i += 2;
                }
            }

            var pictures = new List<(int, int)>(starts.Count);
            for (int k = 0; k < starts.Count; k++)
            {
                int next = k + 1 < starts.Count ? starts[k + 1] : end;
                pictures.Add((starts[k], next - starts[k]));
            }
            return pictures;
        }

        /// <summary>Reads a picture, from its picture start code: its header, and its data as it is.</summary>
        public PictureLayer ReadPicture(ItuStream stream)
        {
            var picture = new PictureLayer();
            stream.ReadClass(0, this, picture, "");
            return picture;
        }

        /// <summary>Writes a picture read with <see cref="ReadPicture"/>.</summary>
        public void WritePicture(ItuStream stream, PictureLayer picture) => stream.WriteClass(this, picture, "");
    }
}
