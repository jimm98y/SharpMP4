using SharpISOBMFF;
using SharpProRes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMP4.Tracks
{
    /// <summary>The profiles of Apple ProRes, each of a sample entry of its own: RDD 36 does not name them.</summary>
    public enum ProResProfile
    {
        /// <summary>ProRes 422 Proxy: 'apco'.</summary>
        Proxy,
        /// <summary>ProRes 422 LT: 'apcs'.</summary>
        LT,
        /// <summary>ProRes 422: 'apcn'.</summary>
        Standard,
        /// <summary>ProRes 422 HQ: 'apch'.</summary>
        HQ,
        /// <summary>ProRes 4444: 'ap4h'.</summary>
        P4444,
        /// <summary>ProRes 4444 XQ: 'ap4x'.</summary>
        P4444XQ,
    }

    /// <summary>
    /// Apple ProRes track (SMPTE RDD 36): its samples frames, each a sync sample, ProRes coding every frame on its own. Of a
    /// sample entry only QuickTime has - 'apco' to 'ap4x', which says the profile, as the frames do not - so of a QuickTime
    /// file. Each frame's header is read for the picture's size, its fields, its colour and its pixel aspect ratio, which
    /// the entry's 'fiel', 'colr' and 'pasp' say. A track read from a file writes back the entry it was read with.
    /// </summary>
    public class ProResTrack : TrackBase
    {
        public override string HandlerName => HandlerNames.Video;
        public override string HandlerType => HandlerTypes.Video;
        public override string Language { get; set; } = "und";

        // the sample entry and Apple's name of each profile, in the order of ProResProfile
        private static readonly string[] Entries = { "apco", "apcs", "apcn", "apch", "ap4h", "ap4x" };
        private static readonly string[] Names = { "Apple ProRes 422 Proxy", "Apple ProRes 422 LT", "Apple ProRes 422", "Apple ProRes 422 HQ", "Apple ProRes 4444", "Apple ProRes 4444 XQ" };

        /// <summary>Whether a sample entry's type is ProRes's.</summary>
        public static bool IsProRes(string coding) => Array.IndexOf(Entries, coding) >= 0;

        /// <summary>The profile of the track, which its sample entry says.</summary>
        public ProResProfile Profile { get; }

        private readonly ProResContext _context = new ProResContext();

        // the entry of a track read from a file, written back as it was
        private SampleEntryCopy _entry;

        private uint _width, _height;
        private uint _interlaceMode;
        private uint _colorPrimaries, _transferCharacteristic, _matrixCoefficients;
        private bool _hasAlpha;
        private uint _pixelAspectH = 1, _pixelAspectV = 1;

        public ProResTrack(ProResProfile profile)
        {
            Profile = profile;
            CompatibleBrand = QuickTimeBrand;
            DefaultSampleFlags = new SampleFlags() { SampleDependsOn = 2 };
            TimescaleFallback = 30000;
            FrameTickFallback = 1001;
        }

        public ProResTrack(ProResProfile profile, uint timescale, int sampleDuration) : this(profile)
        {
            Timescale = timescale;
            DefaultSampleDuration = sampleDuration;
        }

        /// <summary>A track of a ProRes sample entry, as a file has it: its profile the entry's, the entry written back as it was.</summary>
        /// <param name="coding">The entry's coding: its type, or of a protected entry what it was before ('frma').</param>
        public ProResTrack(VisualSampleEntry entry, string coding, uint timescale, int sampleDuration) : this(ProfileOf(coding), timescale, sampleDuration)
        {
            _entry = SampleEntryCopy.Of(entry);
            _width = entry.Width;
            _height = entry.Height;
        }

        private static ProResProfile ProfileOf(string coding)
        {
            int index = Array.IndexOf(Entries, coding);
            if (index < 0)
                throw new ArgumentException($"Not a sample entry of ProRes: {coding}", nameof(coding));
            return (ProResProfile)index;
        }

        /// <summary>
        /// A frame: a sample of its own and a sync sample. Its header is read for what the sample entry says of the picture;
        /// a frame whose header does not read is a sample still, and warned of.
        /// </summary>
        public override void ProcessSample(byte[] buffer, int offset, int length, out ArraySegment<byte> output, out bool isRandomAccessPoint)
        {
            output = default;
            isRandomAccessPoint = false;
            if (buffer == null || length == 0)
                return;

            output = new ArraySegment<byte>(buffer, offset, length);
            isRandomAccessPoint = true;
            try
            {
                var frame = _context.ReadFrame(ProResContext.StreamOf(buffer, offset, length, Logger));
                if (frame.FrameSize != length && Logger.IsWarningEnabled)
                    Logger.LogWarning($"A ProRes frame says it is {frame.FrameSize} bytes, its sample is {length}.");

                var header = frame.FrameHeader;
                _width = header.HorizontalSize;
                _height = header.VerticalSize;
                _interlaceMode = header.InterlaceMode;
                _colorPrimaries = header.ColorPrimaries;
                _transferCharacteristic = header.TransferCharacteristic;
                _matrixCoefficients = header.MatrixCoefficients;
                _hasAlpha = header.AlphaChannelType != 0;
                SetPixelAspectRatio(header.AspectRatioInformation);
            }
            catch (Exception ex) when (ex is not OutOfMemoryException)
            {
                if (Logger.IsWarningEnabled) Logger.LogWarning($"A ProRes frame header could not be read: {ex.Message}");
            }
        }

        /// <summary>
        /// The pixel aspect ratio of aspect_ratio_information (RDD 36 Table 3): square, or a picture of 4:3 or 16:9, of
        /// whose size the samples' ratio is.
        /// </summary>
        private void SetPixelAspectRatio(uint aspectRatioInformation)
        {
            (ulong dw, ulong dh) = aspectRatioInformation switch
            {
                2 => (4ul, 3ul),
                3 => (16ul, 9ul),
                _ => (0ul, 0ul),
            };
            if (dw == 0 || _width == 0 || _height == 0)
            {
                (_pixelAspectH, _pixelAspectV) = (1, 1);
                return;
            }

            ulong h = dw * _height, v = dh * _width;
            ulong gcd = Gcd(h, v);
            (_pixelAspectH, _pixelAspectV) = ((uint)(h / gcd), (uint)(v / gcd));
        }

        private static ulong Gcd(ulong a, ulong b)
        {
            while (b != 0)
                (a, b) = (b, a % b);
            return a;
        }

        /// <summary>
        /// The sample entry: of a track read from a file, the one it was read with; else the profile's, as Apple writes it -
        /// its name, a depth of 32 of a picture with alpha, 24 else - with 'colr' of its colour where its frames say one,
        /// 'fiel' and 'pasp'.
        /// </summary>
        public override Box CreateSampleEntryBox()
        {
            if (_entry != null)
                return _entry.Create();

            var entry = new VisualSampleEntry(IsoStream.FromFourCC(Entries[(int)Profile]));
            entry.Children = new List<Box>();
            entry.ReservedSampleEntry = new byte[6];
            entry.PreDefined0 = new uint[3];
            entry.DataReferenceIndex = 1;
            entry.Depth = (ushort)(_hasAlpha ? 32 : 24);
            entry.FrameCount = 1;
            entry.Horizresolution = 72 << 16;
            entry.Vertresolution = 72 << 16;
            entry.Width = (ushort)_width;
            entry.Height = (ushort)_height;
            entry.Compressorname = PascalString(Names[(int)Profile]);

            // of colour 0 and 2 alike unknown (RDD 36 Tables 5 and 6, as H.273's): no 'colr' where all three are
            if (Specified(_colorPrimaries) || Specified(_transferCharacteristic) || Specified(_matrixCoefficients))
            {
                var colr = new ColourInformationBox
                {
                    ColourType = IsoStream.FromFourCC("nclc"),
                    ColourPrimaries = (ushort)_colorPrimaries,
                    TransferCharacteristics = (ushort)_transferCharacteristic,
                    MatrixCoefficients = (ushort)_matrixCoefficients,
                };
                colr.SetParent(entry);
                entry.Children.Add(colr);
            }

            // one field, or two of a picture each: the top first (9) or the bottom (14), as ffmpeg writes them
            var fiel = new FielBox
            {
                Total = (byte)(_interlaceMode == 0 ? 1 : 2),
                Order = (byte)(_interlaceMode == 1 ? 9 : _interlaceMode == 2 ? 14 : 0),
            };
            fiel.SetParent(entry);
            entry.Children.Add(fiel);

            var pasp = new PixelAspectRatioBox { HSpacing = _pixelAspectH, VSpacing = _pixelAspectV };
            pasp.SetParent(entry);
            entry.Children.Add(pasp);
            return entry;
        }

        private static bool Specified(uint code) => code != 0 && code != 2;

        // a Pascal string of 32 bytes, as QuickTime's compressor name is: its length, then its characters
        private static byte[] PascalString(string text)
        {
            var bytes = new byte[32];
            byte[] characters = Encoding.ASCII.GetBytes(text);
            int length = Math.Min(characters.Length, 31);
            bytes[0] = (byte)length;
            Buffer.BlockCopy(characters, 0, bytes, 1, length);
            return bytes;
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            // the width at which the samples are shown square
            ulong width = _pixelAspectV == 0 ? _width : (ulong)_width * _pixelAspectH / _pixelAspectV;
            tkhd.Width = (uint)width << 16;
            tkhd.Height = _height << 16;
        }

        public override ITrack Clone()
        {
            return CopySettingsTo(new ProResTrack(Profile, Timescale, DefaultSampleDuration)
            {
                _entry = _entry?.Clone(),
                _width = _width,
                _height = _height,
                _interlaceMode = _interlaceMode,
                _colorPrimaries = _colorPrimaries,
                _transferCharacteristic = _transferCharacteristic,
                _matrixCoefficients = _matrixCoefficients,
                _hasAlpha = _hasAlpha,
                _pixelAspectH = _pixelAspectH,
                _pixelAspectV = _pixelAspectV,
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _entry?.Dispose();
            base.Dispose(disposing);
        }
    }
}
