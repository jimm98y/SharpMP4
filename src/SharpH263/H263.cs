using SharpH26X;
using System;
using System.Collections.Generic;

namespace SharpH263
{
    /// <summary>The picture types of H.263: of PTYPE bit 9, and of PLUSPTYPE's picture type code (5.1.4.3).</summary>
    public static class H263PictureTypes
    {
        public const int I = 0;
        public const int P = 1;
        public const int IMPROVED_PB = 2;
        public const int B = 3;
        public const int EI = 4;
        public const int EP = 5;
    }

    /// <summary>
    /// The data of a picture after its header (5.1, Figure 7): its GOBs or slices, ESTUF, EOS and PSTUF to the next picture
    /// start code - not read, but taken as they are: the bits to the next byte, then the bytes.
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
    /// The Back-Channel Message of a picture header (5.1.17, N.4.2), of the videomux submode of the Reference Picture Selection
    /// mode (Annex N): not read - its GN/MBA is of a length the picture's segments say - so a picture that has one is kept
    /// as its bytes.
    /// </summary>
    public sealed class Bcm : IItuSerializable
    {
        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }
        public ulong Read(IItuContext context, ItuStream stream) =>
            throw new NotSupportedException("The back-channel message of the videomux submode (H.263 Annex N) is not read.");
        public ulong Write(IItuContext context, ItuStream stream) =>
            throw new NotSupportedException("The back-channel message of the videomux submode (H.263 Annex N) is not written.");
    }

    /// <summary>
    /// The Reference Picture Resampling parameters of a picture header (5.1.18, P.2), whose warping parameters are coded with
    /// the variable length code of Table D.3: not read, so a picture that has them is kept as its bytes.
    /// </summary>
    public sealed class Rprp : IItuSerializable
    {
        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }
        public ulong Read(IItuContext context, ItuStream stream) =>
            throw new NotSupportedException("The Reference Picture Resampling parameters (H.263 Annex P) are not read.");
        public ulong Write(IItuContext context, ItuStream stream) =>
            throw new NotSupportedException("The Reference Picture Resampling parameters (H.263 Annex P) are not written.");
    }

    /// <summary>
    /// The state H.263's pictures are read in - the picture format, and the modes a picture header's PLUSPTYPE keeps from those
    /// before it (5.1.4.5) - and the reading of a stream picture by picture, each from its picture start code to the next.
    /// </summary>
    public partial class H263Context
    {
        /// <summary>
        /// Whether the Temporal, SNR and Spatial Scalability mode (Annex O) is in use, which is negotiated outside the stream
        /// (ITU-T Rec. H.245): null, taken to be of a picture of a type only it has - B, EI or EP - as ffmpeg takes it.
        /// </summary>
        public bool? Scalability { get; set; }

        /// <summary>The picture type of the picture being read (<see cref="H263PictureTypes"/>).</summary>
        public int PictureType { get; private set; }

        /// <summary>The width and height of the pictures, in samples.</summary>
        public int Width { get; private set; }
        public int Height { get; private set; }

        /// <summary>Whether the picture being read has PLUSPTYPE.</summary>
        public bool HasPlusPtype { get; private set; }

        // The modes of the optional part of PLUSPTYPE, as last sent: on until a picture without PLUSPTYPE (5.1.4.5)
        private bool _customPcf;
        private bool _rpsMode;

        public bool CustomPcfInUse => _customPcf;
        public bool RpsInUse => _rpsMode;
        public bool ScalabilityInUse => Scalability ?? (PictureType == H263PictureTypes.B || PictureType == H263PictureTypes.EI || PictureType == H263PictureTypes.EP);

        // The sizes of the standard source formats (5.1.3): sub-QCIF, QCIF, CIF, 4CIF and 16CIF
        private static readonly (int Width, int Height)[] SourceFormats = { (0, 0), (128, 96), (176, 144), (352, 288), (704, 576), (1408, 1152) };

        /// <summary>PTYPE bits 6-8 read: a standard source format, or "111", PLUSPTYPE following.</summary>
        public void OnSourceFormat(uint sourceFormat)
        {
            HasPlusPtype = sourceFormat == 7;
            if (HasPlusPtype)
                return;

            // without PLUSPTYPE, every mode of it off (5.1.4.5, rule 3)
            _customPcf = false;
            _rpsMode = false;
            SetSourceFormat(sourceFormat);
        }

        /// <summary>The optional part of PLUSPTYPE read: the modes in use from this picture on, and the source format.</summary>
        public void OnOpptype(PictureLayer picture)
        {
            _customPcf = picture.CustomPcf == 1;
            _rpsMode = picture.RpsMode == 1;
            SetSourceFormat(picture.OpptypeSourceFormat);
        }

        public void OnPictureTypeCode(uint pictureTypeCode) => PictureType = (int)pictureTypeCode;

        public void OnPictureCodingType(uint pictureCodingType) => PictureType = pictureCodingType == 0 ? H263PictureTypes.I : H263PictureTypes.P;

        /// <summary>CPFMT read (5.1.5): (PWI + 1) * 4 samples a line, PHI * 4 lines.</summary>
        public void OnCustomPictureFormat(uint pictureWidthIndication, uint pictureHeightIndication)
        {
            Width = ((int)pictureWidthIndication + 1) * 4;
            Height = (int)pictureHeightIndication * 4;
        }

        private void SetSourceFormat(uint sourceFormat)
        {
            if (sourceFormat >= 1 && sourceFormat < SourceFormats.Length)
                (Width, Height) = SourceFormats[sourceFormat];
        }

        /// <summary>A stream to read a picture with: its bits as they are, H.263 having no emulation prevention bytes.</summary>
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
        /// The pictures of a chunk of the stream, each where its picture start code starts - byte aligned (5.1.1): 0x0000 and a
        /// byte of 100000xx, which no GOB, slice or end of sequence start code is - and as long as it is to the next.
        /// </summary>
        public static List<(int Offset, int Length)> Pictures(byte[] data, int offset, int length)
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
