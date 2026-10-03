using SharpISOBMFF;
using SharpMPEG4;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// MPEG-4 Visual (ISO/IEC 14496-2) track: its samples video object planes, each with the units the stream has before it
    /// - the headers of the visual object sequence, visual object and video object layer, a group of VOP header, user data -
    /// and the stuffing after it; in an 'mp4v' sample entry, whose 'esds' is of objectTypeIndication 0x20 (ISO/IEC 14496-1,
    /// Table 5) and has the headers before the first plane as its decoder specific information (ISO/IEC 14496-14, 3.1.2).
    /// </summary>
    public class MPEG4Track : TrackBase
    {
        public override string HandlerName => HandlerNames.Video;
        public override string HandlerType => HandlerTypes.Video;
        public override string Language { get; set; } = "und";
        public override bool ReturnsPreviousSample => true;

        /// <summary>The objectTypeIndication of MPEG-4 Visual (14496-1, Table 5).</summary>
        public const byte VISUAL_ISO_14496_2 = 0x20;
        public const byte VISUAL_STREAM = 0x04;

        private readonly MPEG4Context _context = new MPEG4Context();

        // The units before the first plane, as the stream has them: the decoder specific information
        private readonly List<byte[]> _configuration = new List<byte[]>();
        private bool _configurationComplete;
        private byte[] _decoderSpecificInfo;

        // Of the sample being assembled: whether it has its plane, and whether that is an I-VOP
        private bool _sampleHasPlane;
        private bool _sampleIsRandomAccessPoint;

        private uint _width, _height, _bitRate, _bufferSize;
        private uint _pixelAspectH = 1, _pixelAspectV = 1;

        public MPEG4Track()
        {
            DefaultSampleFlags = new SampleFlags() { SampleDependsOn = 1, SampleIsDifferenceSample = true };
            TimescaleFallback = 30000;
            FrameTickFallback = 1001;
        }

        public MPEG4Track(uint timescale, int sampleDuration) : this()
        {
            Timescale = timescale;
            DefaultSampleDuration = sampleDuration;
        }

        /// <summary>A track of an 'mp4v' entry's 'esds', as a file has it: its decoder specific information read, where it has one.</summary>
        public MPEG4Track(Box config, uint timescale, int sampleDuration) : this(timescale, sampleDuration)
        {
            var decoderConfig = (config as ESDBox)?._ES?.Children?.OfType<DecoderConfigDescriptor>().FirstOrDefault()
                ?? throw new ArgumentException($"Invalid ESDBox: {config?.FourCC}");
            byte[] info = decoderConfig.DecSpecificInfo.OfType<GenericDecoderSpecificInfo>().FirstOrDefault()?.Data;
            if (info != null && info.Length > 0)
            {
                foreach (var (offset, length) in MPEG4Context.Units(info, 0, info.Length))
                    Read(info, offset, length);
                _decoderSpecificInfo = info;
            }
            _configurationComplete = true;
            if (config.GetParent() is VisualSampleEntry entry && _width == 0)
            {
                _width = entry.Width;
                _height = entry.Height;
            }
        }

        /// <summary>Whether a sample entry's objectTypeIndication is MPEG-4 Visual.</summary>
        public static bool IsMPEG4Visual(byte objectTypeIndication) => objectTypeIndication == VISUAL_ISO_14496_2;

        /// <summary>
        /// A unit of the stream, from its start code to the next, as <see cref="MPEG4Context.Units"/> finds them: added to
        /// the sample being assembled - stuffing after its plane too - or, after the plane, ending it and starting the next,
        /// where the sample ended is given out.
        /// </summary>
        public override void ProcessSample(byte[] buffer, int offset, int length, out ArraySegment<byte> output, out bool isRandomAccessPoint)
        {
            output = default;
            isRandomAccessPoint = false;

            // the end of the stream: the last sample
            if (buffer == null || length == 0)
            {
                isRandomAccessPoint = _sampleIsRandomAccessPoint;
                output = TakeMPEG4Sample();
                return;
            }

            if (length < 4 || buffer[offset] != 0 || buffer[offset + 1] != 0 || buffer[offset + 2] != 1)
            {
                if (Logger.IsWarningEnabled) Logger.LogWarning("An MPEG-4 Visual unit does not start with a start code: dropped");
                return;
            }

            int code = buffer[offset + 3];
            if (_sampleHasPlane && !OfThePlane(code))
            {
                isRandomAccessPoint = _sampleIsRandomAccessPoint;
                output = TakeMPEG4Sample();
            }

            switch (Read(buffer, offset, length))
            {
                case VideoObjectPlane plane:
                    _sampleHasPlane = true;
                    _sampleIsRandomAccessPoint = plane.VopCodingType == MPEG4VopCodingTypes.I && plane.VopCoded == 1;
                    _configurationComplete = true;
                    break;
                case UnknownUnit when code == MPEG4StartCodes.VOP:
                    // a plane not read - of a static sprite, of a layer not read: a sample of its own
                    _sampleHasPlane = true;
                    _configurationComplete = true;
                    break;
                default:
                    if (!_configurationComplete && code != MPEG4StartCodes.GROUP_OF_VOP)
                        _configuration.Add(CopyOf(buffer, offset, length));
                    else
                        _configurationComplete = true;
                    break;
            }

            AppendToSample(buffer, offset, length);
        }

        /// <summary>
        /// Whether a unit after a plane is of it: stuffing (6.2.3); of a studio profile, whose planes are not read, its slices,
        /// and the extension and user data before them (6.2.13).
        /// </summary>
        private bool OfThePlane(int code) =>
            code == MPEG4StartCodes.STUFFING ||
            (_context.IsStudio && (code == MPEG4StartCodes.SLICE || code == MPEG4StartCodes.EXTENSION || code == MPEG4StartCodes.USER_DATA));

        private ArraySegment<byte> TakeMPEG4Sample()
        {
            _sampleHasPlane = false;
            _sampleIsRandomAccessPoint = false;
            return TakeSample();
        }

        // A unit read, and what it says of the sample entry kept
        private SharpH26X.IItuSerializable Read(byte[] buffer, int offset, int length)
        {
            SharpH26X.IItuSerializable unit;
            try
            {
                unit = _context.ReadUnit(buffer, offset, length, Logger);
            }
            catch (Exception ex)
            {
                if (Logger.IsWarningEnabled) Logger.LogWarning($"An MPEG-4 Visual unit could not be read: {ex.Message}");
                return null;
            }

            if (unit is VideoObjectLayer layer)
            {
                // of a rectangular layer, its size; the layer of a fine granularity scalable stream has it too
                if (layer.VideoObjectLayerWidth != 0)
                {
                    _width = layer.VideoObjectLayerWidth;
                    _height = layer.VideoObjectLayerHeight;
                }
                SetAspectRatio(layer);
                if (layer.VbvParameters == 1)
                {
                    // 6.3.3: of 400 bit/s, and of 16384 bits
                    _bitRate = ((layer.FirstHalfBitRate << 15) | layer.LatterHalfBitRate) * 400;
                    _bufferSize = ((layer.FirstHalfVbvBufferSize << 3) | layer.LatterHalfVbvBufferSize) * 2048;
                }
            }
            return unit;
        }

        /// <summary>The pixel aspect ratio of aspect_ratio_info (Table 6-14), or of par_width and par_height where it is extended.</summary>
        private void SetAspectRatio(VideoObjectLayer layer)
        {
            (_pixelAspectH, _pixelAspectV) = layer.AspectRatioInfo switch
            {
                2 => (12u, 11u),
                3 => (10u, 11u),
                4 => (16u, 11u),
                5 => (40u, 33u),
                15 when layer.ParWidth != 0 && layer.ParHeight != 0 => ((uint)layer.ParWidth, (uint)layer.ParHeight),
                _ => (1u, 1u),
            };
        }

        public override Box CreateSampleEntryBox()
        {
            var entry = new VisualSampleEntry(IsoStream.FromFourCC("mp4v"));
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

            var esds = new ESDBox();
            esds.SetParent(entry);
            entry.Children.Add(esds);

            var descriptor = new ES_Descriptor { Children = new List<Descriptor>() };
            esds._ES = descriptor;
            descriptor.ESID = (ushort)TrackID;

            var decoderConfig = new DecoderConfigDescriptor { Children = new List<Descriptor>() };
            decoderConfig.SetParent(descriptor);
            descriptor.Children.Add(decoderConfig);
            decoderConfig.ObjectTypeIndication = VISUAL_ISO_14496_2;
            decoderConfig.StreamType = VISUAL_STREAM;
            decoderConfig.Reserved = true;
            decoderConfig.BufferSizeDB = _bufferSize;
            decoderConfig.MaxBitrate = _bitRate;
            decoderConfig.AvgBitrate = 0;

            byte[] info = DecoderSpecificInfo();
            if (info != null && info.Length > 0)
            {
                var specific = new GenericDecoderSpecificInfo { Data = info };
                specific.SetParent(decoderConfig);
                decoderConfig.Children.Add(specific);
            }

            var slConfig = new SLConfigDescriptor { Predefined = 2, Ocr = new byte[0], UseTimeStampsFlag = true };
            slConfig.SetParent(descriptor);
            descriptor.Children.Add(slConfig);

            if (_pixelAspectH != _pixelAspectV)
            {
                var pasp = new PixelAspectRatioBox { HSpacing = _pixelAspectH, VSpacing = _pixelAspectV };
                pasp.SetParent(entry);
                entry.Children.Add(pasp);
            }

            return entry;
        }

        // The units before the first plane, as the stream has them
        private byte[] DecoderSpecificInfo()
        {
            if (_decoderSpecificInfo != null)
                return _decoderSpecificInfo;
            return _configuration.Count == 0 ? null : _configuration.SelectMany(unit => unit).ToArray();
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            // the width at which the samples are shown square
            ulong width = _pixelAspectV == 0 ? _width : (ulong)_width * _pixelAspectH / _pixelAspectV;
            tkhd.Width = (uint)width << 16;
            tkhd.Height = _height << 16;
        }

        /// <summary>The units of a sample, each from its start code to the next.</summary>
        public override IEnumerable<ArraySegment<byte>> ParseSample(byte[] buffer, int offset, int length) =>
            MPEG4Context.Units(buffer, offset, length).Select(u => new ArraySegment<byte>(buffer, u.Offset, u.Length)).ToList();

        /// <summary>The headers a decoder needs before the first sample, as the entry has them.</summary>
        public override IEnumerable<byte[]> GetContainerSamples()
        {
            byte[] info = DecoderSpecificInfo();
            return info == null ? Array.Empty<byte[]>() : new[] { info };
        }

        public override ITrack Clone()
        {
            var clone = new MPEG4Track(Timescale, DefaultSampleDuration)
            {
                _width = _width,
                _height = _height,
                _bitRate = _bitRate,
                _bufferSize = _bufferSize,
                _pixelAspectH = _pixelAspectH,
                _pixelAspectV = _pixelAspectV,
                _decoderSpecificInfo = DecoderSpecificInfo(),
                _configurationComplete = true,
            };
            return CopySettingsTo(clone);
        }
    }
}
