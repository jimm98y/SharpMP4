using SharpAVX;
using SharpISOBMFF;
using SharpVP9;
using System;
using System.Collections.Generic;
using System.IO;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// VP9 Track: each sample a frame, or a superframe of frames and its index (Annex B), as a WebM block is.
    /// </summary>
    /// <remarks>
    /// VP9 Bitstream &amp; Decoding Process Specification v0.7; VP Codec ISO Media File Format Binding v1.0
    /// (https://www.webmproject.org/vp9/mp4/).
    /// </remarks>
    public class VP9Track : TrackBase
    {
        public const string BRAND = "vp09";

        public override string HandlerName => HandlerNames.Video;
        public override string HandlerType => HandlerTypes.Video;
        public override string Language { get; set; } = "eng";

        private readonly VP9Context _context = new VP9Context();

        // What the first key frame said - or the 'vpcC' the track was read with - which the sample entry is made of
        private bool _configured;
        private int _profile, _bitDepth = 8, _chromaSubsampling = 1, _fullRange;
        private int _width, _height, _renderWidth, _renderHeight;

        /// <summary>
        /// The level (VP Codec ISO Media File Format Binding, 2.2): 10 for level 1, 11 for 1.1 and so on. 0 works it out
        /// from the frame size and rate, by libvpx's table of levels (vp9_level_defs), as the stream does not say it.
        /// </summary>
        public byte Level { get; set; }

        /// <summary>The colour primaries of the sample entry (ISO/IEC 23091-2): the stream does not say them, 2 unspecified.</summary>
        public byte ColourPrimaries { get; set; } = 2;

        /// <summary>The transfer characteristics of the sample entry (ISO/IEC 23091-2): the stream does not say them, 2 unspecified.</summary>
        public byte TransferCharacteristics { get; set; } = 2;

        /// <summary>The matrix coefficients of the sample entry (ISO/IEC 23091-2); null, what the stream's color_space says.</summary>
        public byte? MatrixCoefficients { get; set; }

        // of the stream's color_space (7.2.2), as ffmpeg's decoder takes it: CS_UNKNOWN, CS_BT_601, CS_BT_709, CS_SMPTE_170,
        // CS_SMPTE_240, CS_BT_2020, CS_RESERVED, CS_RGB
        private static readonly byte[] MatrixOfColorSpace = { 2, 5, 1, 6, 7, 9, 2, 0 };
        private int _colorSpace;

        public VP9Track()
        {
            DefaultSampleFlags = new SampleFlags() { SampleDependsOn = 1, SampleIsDifferenceSample = true };
            TimescaleFallback = 24000;
            FrameTickFallback = 1001;
        }

        public VP9Track(uint timescale, int sampleDuration) : this()
        {
            Timescale = timescale;
            DefaultSampleDuration = sampleDuration;
        }

        /// <summary>A track of a 'vpcC', as a file has it: the sample entry is made again of it until a key frame says otherwise.</summary>
        public VP9Track(Box config, uint timescale, int sampleDuration) : this(timescale, sampleDuration)
        {
            if (config is not VPCodecConfigurationBox vpcC)
                throw new ArgumentException($"Invalid VPCodecConfigurationBox: {config?.FourCC}");

            _profile = vpcC.Profile;
            Level = vpcC.Level;
            _bitDepth = vpcC.BitDepth;
            _chromaSubsampling = vpcC.ChromaSubsampling;
            _fullRange = vpcC.VideoFullRangeFlag ? 1 : 0;
            if (vpcC.Version >= 1)
            {
                ColourPrimaries = vpcC.ColourPrimaries;
                TransferCharacteristics = vpcC.TransferCharacteristics;
                MatrixCoefficients = vpcC.MatrixCoefficients;
            }
            if (config.GetParent() is VisualSampleEntry entry)
            {
                _width = _renderWidth = entry.Width;
                _height = _renderHeight = entry.Height;
            }
            _configured = true;
        }

        /// <summary>
        /// A sample: a frame, or a superframe of frames and its index. Its frames are read, so that the frames after it are
        /// read as a decoder reads them; it is a sync sample where its first frame is a key frame (frame_type KEY_FRAME, not
        /// a frame shown again), whose headers the sample entry is made of.
        /// </summary>
        public override void ProcessSample(byte[] buffer, int offset, int length, out ArraySegment<byte> output, out bool isRandomAccessPoint)
        {
            isRandomAccessPoint = false;
            output = default;
            if (buffer == null || length == 0)
                return;

            output = new ArraySegment<byte>(buffer, offset, length);
            int[] sizes = VP9Context.SuperframeFrameSizes(buffer, offset, length) ?? new[] { length };
            int position = 0;
            for (int i = 0; i < sizes.Length; i++)
            {
                if (sizes[i] <= 0 || sizes[i] > length - position)
                {
                    if (Logger.IsWarningEnabled) Logger.LogWarning($"A VP9 frame of {sizes[i]} bytes does not fit the {length - position} left of its sample");
                    return;
                }

                try
                {
                    using (var stream = new AomStream(new MemoryStream(buffer, offset + position, sizes[i], writable: false)))
                        _context.Read(stream, sizes[i]);
                }
                catch (Exception ex)
                {
                    if (Logger.IsWarningEnabled) Logger.LogWarning($"A VP9 frame could not be read: {ex.Message}");
                    return;
                }

                bool keyFrame = _context._ShowExistingFrame == 0 && _context._FrameType == VP9Constants.KEY_FRAME;
                if (i == 0)
                    isRandomAccessPoint = keyFrame;
                if (keyFrame)
                    Configure();
                position += sizes[i];
            }
        }

        // The sample entry of the key frame just read
        private void Configure()
        {
            _profile = _context._Profile;
            _bitDepth = _context._BitDepth;
            _colorSpace = _context._ColorSpace;
            _fullRange = _context._ColorRange;
            // 4:2:0 as its chroma sits colocated with luma, which the stream does not say, as ffmpeg writes it; 4:4:0,
            // which the binding has no value for, as 4:2:0 too
            int x = _context._Subsamplingx, y = _context._Subsamplingy;
            _chromaSubsampling = x == 1 && y == 0 ? 2 : x == 0 && y == 0 ? 3 : 1;
            _width = _context._FrameWidth;
            _height = _context._FrameHeight;
            _renderWidth = _context._RenderWidth;
            _renderHeight = _context._RenderHeight;
            _configured = true;
        }

        public override Box CreateSampleEntryBox()
        {
            VisualSampleEntry visualSampleEntry = new VisualSampleEntry(IsoStream.FromFourCC(BRAND));
            visualSampleEntry.Children = new List<Box>();
            visualSampleEntry.ReservedSampleEntry = new byte[6];
            visualSampleEntry.PreDefined0 = new uint[3];
            visualSampleEntry.DataReferenceIndex = 1;
            visualSampleEntry.Depth = 24;
            visualSampleEntry.FrameCount = 1;
            visualSampleEntry.Horizresolution = 72 << 16;
            visualSampleEntry.Vertresolution = 72 << 16;
            visualSampleEntry.Width = (ushort)_width;
            visualSampleEntry.Height = (ushort)_height;
            visualSampleEntry.Compressorname = BinaryUTF8String.GetBytes("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0");

            // version 1 (VP Codec ISO Media File Format Binding, 2.2)
            var vpcC = new VPCodecConfigurationBox();
            vpcC.SetParent(visualSampleEntry);
            vpcC.Version = 1;
            vpcC.Flags = 0;
            vpcC.Profile = (byte)_profile;
            vpcC.Level = Level != 0 ? Level : LevelOf(_width, _height);
            vpcC.BitDepth = (byte)_bitDepth;
            vpcC.ChromaSubsampling = (byte)_chromaSubsampling;
            vpcC.VideoFullRangeFlag = _fullRange != 0;
            vpcC.ColourPrimaries = ColourPrimaries;
            vpcC.TransferCharacteristics = TransferCharacteristics;
            vpcC.MatrixCoefficients = MatrixCoefficients ?? MatrixOfColorSpace[_colorSpace & 7];
            vpcC.CodecInitializationDataSize = 0;
            vpcC.CodecInitializationData = Array.Empty<byte>();
            visualSampleEntry.Children.Add(vpcC);

            if (!_configured && Logger.IsWarningEnabled)
                Logger.LogWarning("No VP9 key frame has been read: the sample entry says nothing of the stream");
            return visualSampleEntry;
        }

        /// <summary>
        /// The lowest level of libvpx's table (vp9_level_defs) a frame of this size fits, at the track's frame rate where it
        /// is known: by its luma samples, its longer side and its luma samples a second.
        /// </summary>
        private byte LevelOf(int width, int height)
        {
            long samples = (long)width * height;
            int breadth = Math.Max(width, height);
            double rate = Timescale != 0 && DefaultSampleDuration > 0 ? samples * (double)Timescale / DefaultSampleDuration : 0;
            foreach (var (level, sampleRate, pictureSize, pictureBreadth) in Levels)
            {
                if (samples <= pictureSize && breadth <= pictureBreadth && rate <= sampleRate)
                    return level;
            }
            return Levels[Levels.Length - 1].Level;
        }

        // level, max luma sample rate, max luma picture size, max luma picture breadth (libvpx, vp9/encoder/vp9_encoder.c)
        private static readonly (byte Level, double SampleRate, long PictureSize, int Breadth)[] Levels =
        {
            (10, 829440, 36864, 512),
            (11, 2764800, 73728, 768),
            (20, 4608000, 122880, 960),
            (21, 9216000, 245760, 1344),
            (30, 20736000, 552960, 2048),
            (31, 36864000, 983040, 2752),
            (40, 83558400, 2228224, 4160),
            (41, 160432128, 2228224, 4160),
            (50, 311951360, 8912896, 8384),
            (51, 588251136, 8912896, 8384),
            (52, 1176502272, 8912896, 8384),
            (60, 1176502272, 35651584, 16832),
            (61, 2353004544, 35651584, 16832),
            (62, 4706009088, 35651584, 16832),
        };

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            tkhd.Width = (uint)_renderWidth << 16;
            tkhd.Height = (uint)_renderHeight << 16;
        }

        /// <summary>The frames of a sample, and the index of a superframe after them.</summary>
        public override IEnumerable<ArraySegment<byte>> ParseSample(byte[] sample, int sampleOffset, int sampleLength)
        {
            var units = new List<ArraySegment<byte>>();
            int[] sizes = VP9Context.SuperframeFrameSizes(sample, sampleOffset, sampleLength);
            if (sizes == null)
            {
                units.Add(new ArraySegment<byte>(sample, sampleOffset, sampleLength));
                return units;
            }

            int position = 0;
            foreach (int size in sizes)
            {
                if (size <= 0 || size > sampleLength - position)
                    break;
                units.Add(new ArraySegment<byte>(sample, sampleOffset + position, size));
                position += size;
            }
            units.Add(new ArraySegment<byte>(sample, sampleOffset + position, sampleLength - position));
            return units;
        }

        public override ITrack Clone()
        {
            return new VP9Track(Timescale, DefaultSampleDuration)
            {
                _configured = _configured,
                _profile = _profile,
                _bitDepth = _bitDepth,
                _chromaSubsampling = _chromaSubsampling,
                _fullRange = _fullRange,
                _colorSpace = _colorSpace,
                _width = _width,
                _height = _height,
                _renderWidth = _renderWidth,
                _renderHeight = _renderHeight,
                Level = Level,
                ColourPrimaries = ColourPrimaries,
                TransferCharacteristics = TransferCharacteristics,
                MatrixCoefficients = MatrixCoefficients,
            };
        }
    }
}
