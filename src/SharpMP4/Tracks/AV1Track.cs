using SharpISOBMFF;
using SharpAV1;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// AV1 Track.
    /// </summary>
    /// <remarks>
    /// https://aomedia.org/specifications/av1/
    /// </remarks>
    public class AV1Track : TrackBase
    {
        public const string BRAND = "av01";

        public override string HandlerName => HandlerNames.Video;
        public override string HandlerType => HandlerTypes.Video;
        public override string Language { get; set; } = "eng";

        private bool _sampleContainsKeyframeRandomAccessPoint = false;
        private bool _sampleContainsDelayedRandomAccessPoint = false;
        private bool _sampleContainsKeyFrameDependentRecoveryPoint = false;
        private bool _sampleContainsSequenceHeader = false;

        /// <summary>
        /// Sequence Header Open Bitstream Unit - raw bytes.
        /// </summary>
        public byte[] SequenceHeaderObuRaw { get; set; } = null;
        /// <summary>
        /// Sequence Header Open Bitstream Unit.
        /// </summary>
        public AV1Context SequenceHeaderObu { get; set; } = null;

        private AV1Context _context = new AV1Context();

        public AV1Track()
        {
            CompatibleBrand = BRAND; // av01
            DefaultSampleFlags = new SampleFlags() { SampleDependsOn = 1, SampleIsDifferenceSample = true };
            TimescaleFallback = 24000;
            FrameTickFallback = 1001;
        }

        public AV1Track(uint timescale, int sampleDuration) : this()
        {
            Timescale = timescale;
            DefaultSampleDuration = sampleDuration;
        }

        public AV1Track(Box config, uint timescale, int sampleDuration) : this(timescale, sampleDuration)
        {
            AV1CodecConfigurationBox av01 = config as AV1CodecConfigurationBox;
            if (av01 == null)
                throw new ArgumentException($"Invalid AV1CodecConfigurationBox: {config.FourCC}");

            ProcessSample(av01.Av1Config.ConfigOBUs, out _, out _);
        }

        /// <summary>
        /// Process 1 OBU.
        /// </summary>
        /// <param name="sample">OBU bytes.</param>
        /// <param name="isRandomAccessPoint">true when the sample contains a keyframe.</param>
        public override void ProcessSample(byte[] buffer, int offset, int length, out ArraySegment<byte> output, out bool isRandomAccessPoint)
        {
            isRandomAccessPoint = false; 
            output = default;

            if (buffer == null || length == 0)
            {
                return;
            }

            // without the sequence header we cannot process AV1
            if (SequenceHeaderObuRaw == null)
            {
                int obuHeader = buffer[offset];
                int obuType = (obuHeader & 0x78) >> 3;
                if (obuType != AV1ObuTypes.OBU_SEQUENCE_HEADER)
                {
                    if (this.Logger.IsErrorEnabled) this.Logger.LogError("OBU Sequence Header missing, dropping sample");
                    return;
                }
            }

            var ms = new MemoryStream(buffer, offset, length);
            using (AomStream stream = new AomStream(ms))
            {
                int len = length;
                do
                {
                    if (this.Logger.IsDebugEnabled) this.Logger.LogDebug($"---OBU begin {len}---");

                    _context.Read(stream, len);

                    int obuTotalSize = 0;
                    if (_context._ObuHasSizeField != 0)
                    {
                        obuTotalSize = _context._ObuSize + 1 /* obu header */ + (_context._ObuExtensionFlag != 0 ? 1 : 0) /* obu extension */ + (_context.ObuSizeLen >> 3);
                    }
                    else
                    {
                        obuTotalSize = len;
                    }
                    
                    len -= obuTotalSize;

                    if (_context._ObuType == AV1ObuTypes.OBU_SEQUENCE_HEADER)
                    {
                        if (this.Logger.IsDebugEnabled) this.Logger.LogDebug($"OBU Sequence Header");

                        if (SequenceHeaderObuRaw == null)
                        {
                            // The configOBUs field SHALL contain at most one present, it SHALL be the first OBU.
                            SequenceHeaderObuRaw = CopyOf(buffer, offset, length);
                            using (var aomStream = new AomStream(new MemoryStream(SequenceHeaderObuRaw)))
                            {
                                SequenceHeaderObu = new AV1Context();
                                SequenceHeaderObu.Read(aomStream, length); 
                            }
                        }
                        else
                        {
                            AppendToSample(buffer, offset, length);
                            _sampleContainsSequenceHeader = true;
                        }

                        if (Timescale == 0 || DefaultSampleDuration == 0)
                        {
                            Timescale = TimescaleFallback;
                            DefaultSampleDuration = FrameTickFallback;
                        }

                        if (TimescaleOverride != 0)
                        {
                            Timescale = TimescaleOverride;
                        }
                        if (FrameTickOverride != 0)
                        {
                            DefaultSampleDuration = FrameTickOverride;
                        }
                    }
                    else if (_context._ObuType == AV1ObuTypes.OBU_TEMPORAL_DELIMITER)
                    {
                        if (this.Logger.IsDebugEnabled) this.Logger.LogDebug("OBU Temporal Delimiter");

                        if (HasSample)
                        {
                            output = TakeSample();
                            isRandomAccessPoint = _sampleContainsKeyframeRandomAccessPoint || _sampleContainsDelayedRandomAccessPoint || _sampleContainsKeyFrameDependentRecoveryPoint;
                            _sampleContainsKeyframeRandomAccessPoint = false;
                            _sampleContainsDelayedRandomAccessPoint = false;
                            _sampleContainsKeyFrameDependentRecoveryPoint = false;
                            _sampleContainsSequenceHeader = false;
                        }

                        AppendToSample(buffer, offset, length);
                    }
                    else if (_context._ObuType == AV1ObuTypes.OBU_FRAME_HEADER)
                    {
                        if (this.Logger.IsDebugEnabled) this.Logger.LogDebug("OBU Frame Header");

                        _context.LastObuFrameHeader = CopyOf(buffer,
                            offset + 1 /* obu header */ + (_context._ObuExtensionFlag != 0 ? 1 : 0) /* obu extension */ + (_context.ObuSizeLen >> 3),
                            _context._ObuSize);

                        AppendToSample(buffer, offset, length);

                        if (HasSample && _context._ShowFrame != 0)
                        {
                            output = TakeSample();
                            isRandomAccessPoint = _sampleContainsKeyframeRandomAccessPoint || _sampleContainsDelayedRandomAccessPoint || _sampleContainsKeyFrameDependentRecoveryPoint;
                            _sampleContainsKeyframeRandomAccessPoint = false;
                            _sampleContainsDelayedRandomAccessPoint = false;
                            _sampleContainsKeyFrameDependentRecoveryPoint = false;
                            _sampleContainsSequenceHeader = false;
                        }
                    }
                    else if (_context._ObuType == AV1ObuTypes.OBU_TILE_GROUP)
                    {
                        if (this.Logger.IsDebugEnabled) this.Logger.LogDebug("OBU Tile Group");

                        AppendToSample(buffer, offset, length);
                    }
                    else if (_context._ObuType == AV1ObuTypes.OBU_METADATA)
                    {
                        if (this.Logger.IsDebugEnabled) this.Logger.LogDebug("OBU Metadata");

                        AppendToSample(buffer, offset, length);
                    }
                    else if (_context._ObuType == AV1ObuTypes.OBU_FRAME)
                    {
                        if (this.Logger.IsDebugEnabled) this.Logger.LogDebug("OBU Frame");

                        AppendToSample(buffer, offset, length);

                        if(_context._FrameType == AV1FrameTypes.KEY_FRAME && _context._ShowFrame != 0)
                        {
                            _sampleContainsKeyframeRandomAccessPoint = true; 
                        }
                        else if(_context._FrameType == AV1FrameTypes.KEY_FRAME && _context._ShowFrame == 0 && _sampleContainsSequenceHeader)
                        {
                            _sampleContainsDelayedRandomAccessPoint = true;
                        }
                        else if(_context._ShowExistingFrame == 1 && _context.RefFrameType[_context._FrameToShowMapIdx] == AV1FrameTypes.KEY_FRAME)
                        {
                            _sampleContainsKeyFrameDependentRecoveryPoint = true;
                        }

                        if (HasSample && _context._ShowFrame != 0)
                        {
                            output = TakeSample();
                            isRandomAccessPoint = _sampleContainsKeyframeRandomAccessPoint || _sampleContainsDelayedRandomAccessPoint || _sampleContainsKeyFrameDependentRecoveryPoint;
                            _sampleContainsKeyframeRandomAccessPoint = false;
                            _sampleContainsDelayedRandomAccessPoint = false;
                            _sampleContainsKeyFrameDependentRecoveryPoint = false;
                            _sampleContainsSequenceHeader = false;
                        }
                    }
                    else if (_context._ObuType == AV1ObuTypes.OBU_REDUNDANT_FRAME_HEADER)
                    {
                        if (this.Logger.IsDebugEnabled) this.Logger.LogDebug("OBU Redundant Frame Header");

                        AppendToSample(buffer, offset, length);

                        if (HasSample && _context._ShowFrame != 0)
                        {
                            output = TakeSample();
                            isRandomAccessPoint = _sampleContainsKeyframeRandomAccessPoint || _sampleContainsDelayedRandomAccessPoint || _sampleContainsKeyFrameDependentRecoveryPoint;
                            _sampleContainsKeyframeRandomAccessPoint = false;
                            _sampleContainsDelayedRandomAccessPoint = false;
                            _sampleContainsKeyFrameDependentRecoveryPoint = false;
                            _sampleContainsSequenceHeader = false;
                        }
                    }
                    else if (_context._ObuType == AV1ObuTypes.OBU_TILE_LIST)
                    {
                        if (this.Logger.IsDebugEnabled) this.Logger.LogDebug("OBU Tile List");

                        AppendToSample(buffer, offset, length);
                    }
                    else if (_context._ObuType == AV1ObuTypes.OBU_PADDING)
                    {
                        if (this.Logger.IsDebugEnabled) this.Logger.LogDebug("OBU Padding");

                        AppendToSample(buffer, offset, length);
                    }
                    else if(
                        _context._ObuType == AV1ObuTypes.OBU_RESERVED_0 || 
                        _context._ObuType == AV1ObuTypes.OBU_RESERVED_10 || 
                        _context._ObuType == AV1ObuTypes.OBU_RESERVED_11 || 
                        _context._ObuType == AV1ObuTypes.OBU_RESERVED_12 || 
                        _context._ObuType == AV1ObuTypes.OBU_RESERVED_13 || 
                        _context._ObuType == AV1ObuTypes.OBU_RESERVED_14 
                        )
                    {
                        // reserved
                        if (this.Logger.IsDebugEnabled) this.Logger.LogDebug("OBU Reserved");

                        AppendToSample(buffer, offset, length);
                    }
                    else
                    {
                        // invalid
                        if (this.Logger.IsDebugEnabled) this.Logger.LogDebug("Invalid OBU!!!");
                    }

                    if (this.Logger.IsDebugEnabled) this.Logger.LogDebug($"---OBU end {obuTotalSize}---");
                } while (len > 0);

                if (ms.Position != ms.Length)
                {
                    if (this.Logger.IsDebugEnabled) this.Logger.LogDebug("---OBU error---");
                }
            }
        }

        public override Box CreateSampleEntryBox()
        {
            VisualSampleEntry visualSampleEntry = new VisualSampleEntry(IsoStream.FromFourCC(BRAND));
            visualSampleEntry.Children = new List<Box>();
            visualSampleEntry.ReservedSampleEntry = new byte[6]; // TODO simplify API
            visualSampleEntry.PreDefined0 = new uint[3]; // TODO simplify API

            visualSampleEntry.DataReferenceIndex = 1;
            visualSampleEntry.Depth = 24;
            visualSampleEntry.FrameCount = 1;
            // convert to fixed point 1616
            visualSampleEntry.Horizresolution = 72 << 16; // TODO simplify API
            visualSampleEntry.Vertresolution = 72 << 16; // TODO simplify API

            visualSampleEntry.Width = (ushort)_context._RenderWidth;
            visualSampleEntry.Height = (ushort)_context._RenderHeight;
            visualSampleEntry.Compressorname = BinaryUTF8String.GetBytes("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0");

            AV1CodecConfigurationBox av01ConfigurationBox = new AV1CodecConfigurationBox();
            av01ConfigurationBox.SetParent(visualSampleEntry);

            av01ConfigurationBox.Av1Config = new AV1CodecConfigurationRecord();
            av01ConfigurationBox.Av1Config.Reserved = 0;
            av01ConfigurationBox.Av1Config.Reserved0 = 0;
            av01ConfigurationBox.Av1Config.Version = 1;
            av01ConfigurationBox.Av1Config.InitialPresentationDelayMinusOne = 0;
            av01ConfigurationBox.Av1Config.InitialPresentationDelayPresent = false;
            av01ConfigurationBox.Av1Config.ChromaSamplePosition = (byte)_context._ChromaSamplePosition;
            av01ConfigurationBox.Av1Config.ChromaSubsamplingx = _context._Subsamplingx != 0;
            av01ConfigurationBox.Av1Config.ChromaSubsamplingy = _context._Subsamplingy != 0;
            av01ConfigurationBox.Av1Config.ConfigOBUs = SequenceHeaderObuRaw; 
            av01ConfigurationBox.Av1Config.HighBitdepth = _context._HighBitdepth != 0;
            av01ConfigurationBox.Av1Config.Marker = true; // shall be set to true
            av01ConfigurationBox.Av1Config.Monochrome = _context._MonoChrome != 0;
            av01ConfigurationBox.Av1Config.SeqLevelIdx0 = (byte)_context._SeqLevelIdx[0];
            av01ConfigurationBox.Av1Config.SeqProfile = (byte)_context._SeqProfile;
            av01ConfigurationBox.Av1Config.SeqTier0 = _context._SeqTier[0] != 0;
            av01ConfigurationBox.Av1Config.TwelveBit = _context._TwelveBit != 0;

            visualSampleEntry.Children.Add(av01ConfigurationBox);

            return visualSampleEntry;
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            tkhd.Width = (uint)_context._RenderWidth << 16;
            tkhd.Height = (uint)_context._RenderHeight << 16;
        }

        public override IEnumerable<ArraySegment<byte>> ParseSample(byte[] sample, int sampleOffset, int sampleLength)
        {
            List<ArraySegment<byte>> result = new List<ArraySegment<byte>>();
            var ms = new MemoryStream(sample);
            using (AomStream stream = new AomStream(ms))
            {
                int len = sample.Length;
                int currentPosition = 0;

                do
                {
                    _context.Read(stream, len);

                    int obuTotalSize = 0;
                    if (_context._ObuHasSizeField != 0)
                    {
                        obuTotalSize = _context._ObuSize + 1 /* obu header */ + (_context._ObuExtensionFlag != 0 ? 1 : 0) /* obu extension */ + (_context.ObuSizeLen >> 3);
                    }
                    else
                    {
                        obuTotalSize = len;
                    }

                    if(_context._ObuType == AV1ObuTypes.OBU_SEQUENCE_HEADER)
                    {
                        if (SequenceHeaderObuRaw == null) // we care only about the first one
                        {
                            SequenceHeaderObuRaw = sample.Skip(currentPosition).Take(1 + (_context._ObuExtensionFlag != 0 ? 1 : 0) + (_context.ObuSizeLen >> 3) + _context._ObuSize).ToArray();
                            using (var aomStream = new AomStream(new MemoryStream(SequenceHeaderObuRaw)))
                            {
                                SequenceHeaderObu = new AV1Context();
                                SequenceHeaderObu.Read(aomStream, sample.Length);
                            }
                        }
                    }
                    else if (_context._ObuType == AV1ObuTypes.OBU_FRAME_HEADER)
                    {
                        _context.LastObuFrameHeader = sample.Skip(currentPosition + 1 /* obu header */ + (_context._ObuExtensionFlag != 0 ? 1 : 0) /* obu extension */ + (_context.ObuSizeLen >> 3)).Take(_context._ObuSize).ToArray();
                    }

                    var sampleBytes = new ArraySegment<byte>(sample, currentPosition, obuTotalSize);
                    result.Add(sampleBytes);
                    currentPosition += obuTotalSize;

                    len -= obuTotalSize;
                } while (len > 0);
            }

            return result;
        }

        public override IEnumerable<byte[]> GetContainerSamples()
        {
            if(SequenceHeaderObuRaw == null)
                return null;

            return new byte[][] { SequenceHeaderObuRaw };
        }

        public override ITrack Clone()
        {
            return new AV1Track(Timescale, DefaultSampleDuration);
        }
    }
}
