using SharpH262;
using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// H.262 | ISO/IEC 13818-2 (MPEG-2 video) track - and ISO/IEC 11172-2 (MPEG-1 video), whose headers H.262's read: its
    /// samples coded frames, of a picture or of the two field pictures of a frame, each with the sequence header, group of
    /// pictures header and extensions that come before it, as the stream has them; in an 'mp4v' sample entry, whose 'esds'
    /// says the profile by its objectTypeIndication (ISO/IEC 14496-1, Table 5) and has the first sequence header and its
    /// extension as its decoder specific information.
    /// </summary>
    public class H262Track : TrackBase
    {
        public override string HandlerName => HandlerNames.Video;
        public override string HandlerType => HandlerTypes.Video;
        public override string Language { get; set; } = "und";
        public override bool ReturnsPreviousSample => true;

        // The objectTypeIndication values of MPEG-2 video by profile, and of MPEG-1 video (14496-1, Table 5)
        public const byte MPEG2_SIMPLE = 0x60;
        public const byte MPEG2_MAIN = 0x61;
        public const byte MPEG2_SNR = 0x62;
        public const byte MPEG2_SPATIAL = 0x63;
        public const byte MPEG2_HIGH = 0x64;
        public const byte MPEG2_422 = 0x65;
        public const byte MPEG1 = 0x6A;
        public const byte VISUAL_STREAM = 0x04;

        private readonly H262Context _context = new H262Context();

        // The first sequence header and its extension, as the stream has them: the decoder specific information
        private byte[] _sequenceHeader;
        private byte[] _sequenceExtension;
        private byte[] _decoderSpecificInfo;

        // Of the sample being assembled: whether it has a picture, and whether that is an I picture; whether the picture being
        // read is the second field of a frame, and whether one is to come - of the same sample as the first
        private bool _sampleHasPicture;
        private bool _sampleIsRandomAccessPoint;
        private bool _secondField;
        private bool _expectSecondField;

        private byte _objectTypeIndication = MPEG2_MAIN;
        private uint _width, _height, _bitRate, _bufferSize;
        private uint _pixelAspectH = 1, _pixelAspectV = 1;

        public H262Track()
        {
            DefaultSampleFlags = new SampleFlags() { SampleDependsOn = 1, SampleIsDifferenceSample = true };
            TimescaleFallback = 90000;
            FrameTickFallback = 3600;
        }

        public H262Track(uint timescale, int sampleDuration) : this()
        {
            Timescale = timescale;
            DefaultSampleDuration = sampleDuration;
        }

        /// <summary>A track of an 'mp4v' entry's 'esds', as a file has it: its sequence header read, where it has one.</summary>
        public H262Track(Box config, uint timescale, int sampleDuration) : this(timescale, sampleDuration)
        {
            var decoderConfig = (config as ESDBox)?._ES?.Children?.OfType<DecoderConfigDescriptor>().FirstOrDefault()
                ?? throw new ArgumentException($"Invalid ESDBox: {config?.FourCC}");
            _objectTypeIndication = decoderConfig.ObjectTypeIndication;
            byte[] info = decoderConfig.DecSpecificInfo.OfType<GenericDecoderSpecificInfo>().FirstOrDefault()?.Data;
            if (info != null && info.Length > 0)
            {
                foreach (var (offset, length) in H262Context.Units(info, 0, info.Length))
                    Read(info, offset, length);
                _decoderSpecificInfo = info;
            }
            if (config.GetParent() is VisualSampleEntry entry && _width == 0)
            {
                _width = entry.Width;
                _height = entry.Height;
            }
        }

        /// <summary>
        /// A track of one of QuickTime's own MPEG-1 or MPEG-2 entries (<see cref="QuickTimeEntries"/>), which have no
        /// configuration - the sequence header is in the samples: of its size the entry's, written again as the same entry.
        /// </summary>
        /// <param name="coding">The entry's coding: its type, or of a protected entry what it was before ('frma').</param>
        public H262Track(VisualSampleEntry entry, string coding, uint timescale, int sampleDuration) : this(timescale, sampleDuration)
        {
            if (!QuickTimeEntries.TryGetValue(coding, out bool mpeg1))
                throw new ArgumentException($"Not a QuickTime entry of MPEG-1 or MPEG-2 video: {coding}");
            SampleEntryType = coding;
            _objectTypeIndication = mpeg1 ? MPEG1 : MPEG2_MAIN;
            _width = entry.Width;
            _height = entry.Height;
        }

        /// <summary>
        /// The sample entry the track is written in: 'mp4v' with an 'esds' (ISO/IEC 14496-14), unless the track is of one of
        /// QuickTime's own entries, which it is written in again - without a configuration, as QuickTime has it.
        /// </summary>
        public string SampleEntryType
        {
            get => _sampleEntryType;
            set
            {
                _sampleEntryType = value;
                // QuickTime's own entries make the file a QuickTime file
                CompatibleBrand = value == "mp4v" ? null : QuickTimeBrand;
            }
        }

        private string _sampleEntryType = "mp4v";

        /// <summary>
        /// QuickTime's sample entries of MPEG-1 video (true) and MPEG-2 video (false), as FFmpeg knows them
        /// (libavformat/isom_tags.c): Apple's camcorder and CoreMedia types, HDV, IMX, XDCAM, Avid's and Final Cut Pro's.
        /// </summary>
        public static readonly IReadOnlyDictionary<string, bool> QuickTimeEntries = new Dictionary<string, bool>
        {
            { "m1v ", true }, { "m1v1", true }, { "mpeg", true }, { "mp1v", true },
            { "m2v1", false }, { "mp2v", false }, { "AVmp", false },
            { "hdv1", false }, { "hdv2", false }, { "hdv3", false }, { "hdv4", false }, { "hdv5", false }, { "hdv6", false },
            { "hdv7", false }, { "hdv8", false }, { "hdv9", false }, { "hdva", false },
            { "mx3n", false }, { "mx3p", false }, { "mx4n", false }, { "mx4p", false }, { "mx5n", false }, { "mx5p", false },
            { "xd51", false }, { "xd54", false }, { "xd55", false }, { "xd59", false }, { "xd5a", false }, { "xd5b", false },
            { "xd5c", false }, { "xd5d", false }, { "xd5e", false }, { "xd5f", false },
            { "xdv1", false }, { "xdv2", false }, { "xdv3", false }, { "xdv4", false }, { "xdv5", false }, { "xdv6", false },
            { "xdv7", false }, { "xdv8", false }, { "xdv9", false }, { "xdva", false }, { "xdvb", false }, { "xdvc", false },
            { "xdvd", false }, { "xdve", false }, { "xdvf", false }, { "xdhd", false }, { "xdh2", false },
        };

        /// <summary>Whether a sample entry's objectTypeIndication is MPEG-2 video, of any profile, or MPEG-1 video.</summary>
        public static bool IsH262(byte objectTypeIndication) =>
            objectTypeIndication >= MPEG2_SIMPLE && objectTypeIndication <= MPEG2_422 || objectTypeIndication == MPEG1;

        /// <summary>
        /// A unit of the stream, from its start code to the next, as <see cref="H262Context.Units"/> finds them: added to the
        /// sample being assembled, or ending it - a sequence header, group of pictures header or picture header after its
        /// picture, but for the second field of a frame - and starting the next, where the sample ended is given out.
        /// </summary>
        public override void ProcessSample(byte[] buffer, int offset, int length, out ArraySegment<byte> output, out bool isRandomAccessPoint)
        {
            output = default;
            isRandomAccessPoint = false;

            // the end of the stream: the last sample
            if (buffer == null || length == 0)
            {
                isRandomAccessPoint = _sampleIsRandomAccessPoint;
                output = TakeH262Sample();
                return;
            }

            if (length < 4 || buffer[offset] != 0 || buffer[offset + 1] != 0 || buffer[offset + 2] != 1)
            {
                if (Logger.IsWarningEnabled) Logger.LogWarning("An H.262 unit does not start with a start code: dropped");
                return;
            }

            int code = buffer[offset + 3];
            bool startsSample = code == H262StartCodes.SEQUENCE_HEADER || code == H262StartCodes.GROUP ||
                (code == H262StartCodes.PICTURE && !_expectSecondField);
            if (startsSample && _sampleHasPicture)
            {
                isRandomAccessPoint = _sampleIsRandomAccessPoint;
                output = TakeH262Sample();
            }

            switch (Read(buffer, offset, length))
            {
                case PictureHeader picture:
                    _secondField = _expectSecondField;
                    _expectSecondField = false;
                    if (!_sampleHasPicture)
                    {
                        // of a frame of two field pictures, the first says whether it is a sync sample
                        _sampleHasPicture = true;
                        _sampleIsRandomAccessPoint = picture.PictureCodingType == 1;
                    }
                    break;
                case PictureCodingExtension coding:
                    // a field picture, the first of its frame: the other is of the same sample
                    if (coding.PictureStructure != 3 && !_secondField)
                        _expectSecondField = true;
                    break;
            }

            AppendToSample(buffer, offset, length);
        }

        private ArraySegment<byte> TakeH262Sample()
        {
            _sampleHasPicture = false;
            _sampleIsRandomAccessPoint = false;
            return TakeSample();
        }

        // A unit read, and what it says of the sample entry kept
        private SharpH26X.IItuSerializable Read(byte[] buffer, int offset, int length)
        {
            SharpH26X.IItuSerializable unit;
            try
            {
                unit = _context.ReadUnit(H262Context.StreamOf(buffer, offset, length, Logger));
            }
            catch (Exception ex)
            {
                if (Logger.IsWarningEnabled) Logger.LogWarning($"An H.262 unit could not be read: {ex.Message}");
                return null;
            }

            switch (unit)
            {
                case SequenceHeader header:
                    _sequenceHeader ??= CopyOf(buffer, offset, length);
                    _width = header.HorizontalSizeValue;
                    _height = header.VerticalSizeValue;
                    _bitRate = header.BitRateValue;
                    _bufferSize = header.VbvBufferSizeValue;
                    SetAspectRatio(header.AspectRatioInformation);
                    // MPEG-1 video, until a sequence extension says otherwise
                    if (_sequenceExtension == null)
                        _objectTypeIndication = MPEG1;
                    break;
                case SequenceExtension extension:
                    _sequenceExtension ??= CopyOf(buffer, offset, length);
                    _width = _context.HorizontalSize;
                    _height = _context.VerticalSize;
                    _bitRate = (extension.BitRateExtension << 18) | (_context.SequenceHeader?.BitRateValue ?? 0);
                    _bufferSize = (extension.VbvBufferSizeExtension << 10) | (_context.SequenceHeader?.VbvBufferSizeValue ?? 0);
                    _objectTypeIndication = ObjectTypeOf(extension.ProfileAndLevelIndication);
                    SetAspectRatio(_context.SequenceHeader?.AspectRatioInformation ?? 1);
                    break;
            }
            return unit;
        }

        /// <summary>
        /// The objectTypeIndication of a profile_and_level_indication (6.3.5, Table 8-2): the profile, and of the escaped
        /// ones the 4:2:2 profile's; the multi-view profile has none of its own, and is taken as the main profile.
        /// </summary>
        private static byte ObjectTypeOf(uint profileAndLevel)
        {
            if ((profileAndLevel & 0x80) != 0)
                return profileAndLevel == 0x82 || profileAndLevel == 0x85 ? MPEG2_422 : MPEG2_MAIN;
            switch ((profileAndLevel >> 4) & 0x7)
            {
                case 1: return MPEG2_HIGH;
                case 2: return MPEG2_SPATIAL;
                case 3: return MPEG2_SNR;
                case 5: return MPEG2_SIMPLE;
                default: return MPEG2_MAIN;
            }
        }

        /// <summary>
        /// The pixel aspect ratio of aspect_ratio_information (6.3.3, Table 6-3): of MPEG-2 a display aspect ratio - square
        /// samples, 3:4, 9:16 or 1:2.21 - over the frame's; MPEG-1's are of the samples themselves, and taken as square.
        /// </summary>
        private void SetAspectRatio(uint aspectRatioInformation)
        {
            (_pixelAspectH, _pixelAspectV) = (1, 1);
            if (_objectTypeIndication == MPEG1 || _width == 0 || _height == 0)
                return;

            (ulong dw, ulong dh) = aspectRatioInformation switch
            {
                2 => (4ul, 3ul),
                3 => (16ul, 9ul),
                4 => (221ul, 100ul),
                _ => (0ul, 0ul),
            };
            if (dw == 0)
                return;

            // the samples' ratio: the display's, of the frame's size
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

        public override Box CreateSampleEntryBox()
        {
            var entry = new VisualSampleEntry(IsoStream.FromFourCC(SampleEntryType));
            entry.Children = new List<Box>();
            entry.ReservedSampleEntry = new byte[6];
            entry.PreDefined0 = new uint[3];
            entry.DataReferenceIndex = 1;
            entry.Depth = 24;
            entry.FrameCount = 1;
            entry.Horizresolution = 72 << 16;
            entry.Vertresolution = 72 << 16;
            entry.Width = (ushort)_width;
            entry.Height = (ushort)_height;
            entry.Compressorname = BinaryUTF8String.GetBytes("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0");

            // QuickTime's own entries have no configuration: the sequence header is in the samples
            if (SampleEntryType != "mp4v")
            {
                AddPixelAspectRatio(entry);
                return entry;
            }

            var esds = new ESDBox();
            esds.SetParent(entry);
            entry.Children.Add(esds);

            var descriptor = new ES_Descriptor { Children = new List<Descriptor>() };
            esds._ES = descriptor;
            descriptor.ESID = (ushort)TrackID;

            var decoderConfig = new DecoderConfigDescriptor { Children = new List<Descriptor>() };
            decoderConfig.SetParent(descriptor);
            descriptor.Children.Add(decoderConfig);
            decoderConfig.ObjectTypeIndication = _objectTypeIndication;
            decoderConfig.StreamType = VISUAL_STREAM;
            decoderConfig.Reserved = true;
            // vbv_buffer_size in units of 16 kbit (6.3.3), in bytes; bit_rate in units of 400 bit/s, the all ones of
            // MPEG-1's variable rate none
            decoderConfig.BufferSizeDB = _bufferSize * 2048;
            decoderConfig.MaxBitrate = _bitRate == 0x3FFFF && _objectTypeIndication == MPEG1 ? 0 : _bitRate * 400;
            decoderConfig.AvgBitrate = 0;

            byte[] info = _decoderSpecificInfo ?? DecoderSpecificInfo();
            if (info != null && info.Length > 0)
            {
                var specific = new GenericDecoderSpecificInfo { Data = info };
                specific.SetParent(decoderConfig);
                decoderConfig.Children.Add(specific);
            }

            var slConfig = new SLConfigDescriptor { Predefined = 2, Ocr = new byte[0], UseTimeStampsFlag = true };
            slConfig.SetParent(descriptor);
            descriptor.Children.Add(slConfig);

            AddPixelAspectRatio(entry);
            return entry;
        }

        private void AddPixelAspectRatio(VisualSampleEntry entry)
        {
            if (_pixelAspectH != _pixelAspectV)
            {
                var pasp = new PixelAspectRatioBox { HSpacing = _pixelAspectH, VSpacing = _pixelAspectV };
                pasp.SetParent(entry);
                entry.Children.Add(pasp);
            }
        }

        // The first sequence header and its extension, as ffmpeg writes them
        private byte[] DecoderSpecificInfo()
        {
            if (_sequenceHeader == null)
                return null;
            return _sequenceExtension == null ? _sequenceHeader : _sequenceHeader.Concat(_sequenceExtension).ToArray();
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            // the width at which the samples are shown square
            ulong width = (ulong)_width * _pixelAspectH / _pixelAspectV;
            tkhd.Width = (uint)width << 16;
            tkhd.Height = _height << 16;
        }

        /// <summary>The units of a sample, each from its start code to the next.</summary>
        public override IEnumerable<ArraySegment<byte>> ParseSample(byte[] buffer, int offset, int length) =>
            H262Context.Units(buffer, offset, length).Select(u => new ArraySegment<byte>(buffer, u.Offset, u.Length)).ToList();

        /// <summary>The sequence header and its extension, which a decoder needs before the first sample, as the entry has them.</summary>
        public override IEnumerable<byte[]> GetContainerSamples()
        {
            byte[] info = _decoderSpecificInfo ?? DecoderSpecificInfo();
            return info == null ? Array.Empty<byte[]>() : new[] { info };
        }

        public override ITrack Clone()
        {
            var clone = new H262Track(Timescale, DefaultSampleDuration)
            {
                SampleEntryType = SampleEntryType,
                _objectTypeIndication = _objectTypeIndication,
                _width = _width,
                _height = _height,
                _bitRate = _bitRate,
                _bufferSize = _bufferSize,
                _pixelAspectH = _pixelAspectH,
                _pixelAspectV = _pixelAspectV,
                _sequenceHeader = _sequenceHeader,
                _sequenceExtension = _sequenceExtension,
                _decoderSpecificInfo = _decoderSpecificInfo,
            };
            return CopySettingsTo(clone);
        }
    }
}
