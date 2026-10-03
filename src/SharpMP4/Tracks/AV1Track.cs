using SharpISOBMFF;
using SharpAV1;
using SharpAVX;
using System.Collections.Generic;
using System.IO;
using System;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// AV1 Track.
    /// </summary>
    /// <remarks>
    /// https://aomedia.org/specifications/av1/ and the AV1 Codec ISO Media File Format Binding
    /// (https://aomediacodec.github.io/av1-isobmff/): a sample is a temporal unit, without its temporal delimiter and
    /// without padding OBUs (2.4).
    /// </remarks>
    public class AV1Track : TrackBase
    {
        public const string BRAND = "av01";

        public override string HandlerName => HandlerNames.Video;
        public override string HandlerType => HandlerTypes.Video;
        public override string Language { get; set; } = "und";

        // A temporal unit is handed out when the next one begins - at its temporal delimiter or first OBU - or at the end
        public override bool ReturnsPreviousSample => true;

        // Of the temporal unit being assembled: whether a frame header has come yet, whether a sequence header came before
        // it, and whether it is a random access point - its first frame a key frame shown, a sequence header before it.
        private bool _sampleHasFrame = false;
        private bool _sampleHasSequenceHeaderFirst = false;
        private bool _sampleIsRandomAccessPoint = false;

        // The spatial layer of the last frame of the temporal unit being assembled that is shown, -1 before one is: the
        // OBUs after it are of the next temporal unit, but for the tile groups of its frame and the frames of the layers
        // above it, which a temporal unit of more than one spatial layer shows each of.
        private int _shownSpatialId = -1;

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

            ProcessConfiguration(av01.Av1Config.ConfigOBUs);
        }

        /// <summary>
        /// The OBUs of a configuration - the configOBUs of an 'av1C', or a clone's sequence header - read as the stream's are,
        /// but put in no sample: the samples bring their own.
        /// </summary>
        private void ProcessConfiguration(byte[] obus)
        {
            if (obus == null || obus.Length == 0)
                return;

            ProcessSample(obus, 0, obus.Length, out _, out _);
            if (HasSample)
                TakeAv1Sample(out _);
        }

        /// <summary>
        /// The temporal unit assembled, and whether it is a sync sample - a Random Access Point, as the AV1 binding has it
        /// (2.4): its first frame a Key Frame with show_frame 1, a Sequence Header OBU before it. Delayed Random Access
        /// Points and Key Frame Dependent Recovery Points are not: 'av1f' is for them.
        /// </summary>
        private ArraySegment<byte> TakeAv1Sample(out bool isRandomAccessPoint)
        {
            isRandomAccessPoint = _sampleIsRandomAccessPoint;
            _sampleHasFrame = false;
            _sampleHasSequenceHeaderFirst = false;
            _sampleIsRandomAccessPoint = false;
            _shownSpatialId = -1;
            return TakeSample();
        }

        /// <summary>A frame header of the temporal unit: the first says whether the unit is a random access point.</summary>
        private void OnFrameHeader()
        {
            if (_sampleHasFrame)
                return;
            _sampleHasFrame = true;
            _sampleIsRandomAccessPoint = _sampleHasSequenceHeaderFirst && _context._ShowExistingFrame == 0 &&
                _context._FrameType == AV1FrameTypes.KEY_FRAME && _context._ShowFrame != 0;
        }

        /// <summary>
        /// Takes OBUs, one or more - an OBU at a time as an encoder hands them out, a temporal unit of an IVF frame or
        /// a sample of a file - each with its header and, but for the last, its size. A temporal unit ends at the temporal
        /// delimiter after it, or at the first OBU after its frame shown that is not of that frame; a null buffer ends the
        /// last one. Temporal delimiters and padding OBUs are no sample's (AV1 binding 2.4).
        /// </summary>
        /// <param name="isRandomAccessPoint">true when the sample finished is a sync sample.</param>
        public override void ProcessSample(byte[] buffer, int offset, int length, out ArraySegment<byte> output, out bool isRandomAccessPoint)
        {
            isRandomAccessPoint = false;
            output = default;

            if (buffer == null)
            {
                // the last temporal unit
                if (HasSample)
                    output = TakeAv1Sample(out isRandomAccessPoint);
                return;
            }

            int end = offset + length;
            int position = offset;
            while (position < end)
            {
                int start = position;
                if (!ObuSize(buffer, start, end, out int total, out int headerLength))
                {
                    if (this.Logger.IsErrorEnabled) this.Logger.LogError($"An OBU past the {end - start} bytes left, dropping the rest");
                    return;
                }
                position += total;

                int obuType = (buffer[start] >> 3) & 0xF;
                int spatialId = (buffer[start] & 0x04) != 0 ? (buffer[start + 1] >> 3) & 0x3 : 0;

                if (obuType == AV1ObuTypes.OBU_TEMPORAL_DELIMITER)
                {
                    if (this.Logger.IsDebugEnabled) this.Logger.LogDebug("OBU Temporal Delimiter");

                    // the temporal unit before it ends; the delimiter is in no sample
                    if (HasSample)
                        TakeInto(ref output, ref isRandomAccessPoint);
                    continue;
                }

                if (obuType == AV1ObuTypes.OBU_PADDING)
                {
                    // in no sample: padding is the container's to do
                    if (this.Logger.IsDebugEnabled) this.Logger.LogDebug("OBU Padding");
                    continue;
                }

                // after the frame the temporal unit shows, an OBU not of that frame - nor of a layer above it - is of the next
                bool ofTheShownFrame = obuType == AV1ObuTypes.OBU_TILE_GROUP || obuType == AV1ObuTypes.OBU_REDUNDANT_FRAME_HEADER ||
                    obuType == AV1ObuTypes.OBU_TILE_LIST || spatialId > _shownSpatialId;
                if (_shownSpatialId >= 0 && !ofTheShownFrame && HasSample)
                    TakeInto(ref output, ref isRandomAccessPoint);

                // without the sequence header we cannot process AV1
                if (SequenceHeaderObuRaw == null && obuType != AV1ObuTypes.OBU_SEQUENCE_HEADER)
                {
                    if (this.Logger.IsErrorEnabled) this.Logger.LogError("OBU Sequence Header missing, dropping OBU");
                    continue;
                }

                bool read = true;
                try
                {
                    using (var stream = new AomStream(new MemoryStream(buffer, start, total, writable: false)))
                        _context.Read(stream, total);
                }
                catch (Exception ex) when (ex is not OutOfMemoryException)
                {
                    // kept all the same: the sample is what the stream has
                    read = false;
                    if (this.Logger.IsWarningEnabled) this.Logger.LogWarning($"OBU type {obuType} could not be read: {ex.Message}");
                }

                if (obuType == AV1ObuTypes.OBU_SEQUENCE_HEADER)
                {
                    if (this.Logger.IsDebugEnabled) this.Logger.LogDebug($"OBU Sequence Header");

                    if (SequenceHeaderObuRaw == null && read)
                    {
                        // The configOBUs field SHALL contain at most one present, it SHALL be the first OBU.
                        SequenceHeaderObuRaw = CopyOf(buffer, start, total);
                        using (var aomStream = new AomStream(new MemoryStream(SequenceHeaderObuRaw)))
                        {
                            SequenceHeaderObu = new AV1Context();
                            SequenceHeaderObu.Read(aomStream, total);
                        }

                        SetTiming();
                    }

                    // in the sample too: a random access point has one before its first frame (2.4)
                    if (!_sampleHasFrame)
                        _sampleHasSequenceHeaderFirst = true;
                }
                else if ((obuType == AV1ObuTypes.OBU_FRAME_HEADER || obuType == AV1ObuTypes.OBU_FRAME) && read)
                {
                    if (this.Logger.IsDebugEnabled) this.Logger.LogDebug(obuType == AV1ObuTypes.OBU_FRAME ? "OBU Frame" : "OBU Frame Header");

                    // A redundant frame header may repeat the header of a frame OBU as well as
                    // that of a frame header OBU. The copy reads only the header's syntax, so
                    // the tile group after it does no harm.
                    int payload = start + headerLength;
                    _context.LastObuFrameHeader = CopyOf(buffer, payload, start + total - payload);

                    OnFrameHeader();

                    // the frame the temporal unit shows - of its layer - after which the next temporal unit starts
                    if (_context._ShowExistingFrame != 0 || _context._ShowFrame != 0)
                        _shownSpatialId = Math.Max(_shownSpatialId, spatialId);
                }

                AppendToSample(buffer, start, total);
            }
        }

        /// <summary>
        /// The temporal unit assembled, handed out: one a call. A call given more than one temporal unit whole has the one
        /// before the last lost, which is said.
        /// </summary>
        private void TakeInto(ref ArraySegment<byte> output, ref bool isRandomAccessPoint)
        {
            if (output.Array != null && this.Logger.IsErrorEnabled)
                this.Logger.LogError("More than one temporal unit ended in one call: give the OBUs of a temporal unit at most a call, the one before is lost");
            output = TakeAv1Sample(out isRandomAccessPoint);
        }

        /// <summary>
        /// The size of the OBU at a position - its header, its leb128() obu_size and its payload, or all that is left where
        /// it has no obu_size - and of its header alone with that size. False where it does not fit what is left.
        /// </summary>
        private static bool ObuSize(byte[] buffer, int start, int end, out int total, out int headerLength)
        {
            total = 0;
            headerLength = 1 + ((buffer[start] & 0x04) != 0 ? 1 : 0);
            if (start + headerLength > end)
                return false;

            if ((buffer[start] & 0x02) == 0)
            {
                // obu_has_size_field 0: the OBU is the rest
                total = end - start;
                return true;
            }

            long size = 0;
            int i = 0;
            for (; ; i++)
            {
                if (i == 8 || start + headerLength + i >= end)
                    return false;
                byte b = buffer[start + headerLength + i];
                size |= (long)(b & 0x7f) << (7 * i);
                if ((b & 0x80) == 0)
                    break;
            }
            headerLength += i + 1;

            if (size > end - start - headerLength)
                return false;
            total = headerLength + (int)size;
            return true;
        }

        /// <summary>
        /// The timescale and frame duration, where the track has none: the sequence header's timing_info (AV1 5.5.3,
        /// 6.4.2) - a tick of num_units_in_display_tick of time_scale, a picture num_ticks_per_picture of them where the
        /// interval is equal - or the fallback where it has none. The overrides above both.
        /// </summary>
        private void SetTiming()
        {
            if (Timescale == 0 || DefaultSampleDuration == 0)
            {
                var sequence = SequenceHeaderObu;
                uint timeScale = sequence == null ? 0 : (uint)sequence._TimeScale;
                uint tick = sequence == null ? 0 : (uint)sequence._NumUnitsInDisplayTick;
                long duration = sequence != null && sequence._EqualPictureInterval != 0 ? tick * ((long)sequence._NumTicksPerPictureMinus1 + 1) : tick;
                if (sequence != null && sequence._TimingInfoPresentFlag != 0 && timeScale != 0 && duration > 0 && duration <= int.MaxValue)
                {
                    Timescale = timeScale;
                    DefaultSampleDuration = (int)duration;
                }
                else
                {
                    Timescale = TimescaleFallback;
                    DefaultSampleDuration = FrameTickFallback;
                }
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

        /// <summary>The frame's size as shown, or the sequence's largest before a frame has been read.</summary>
        private int Width => _context._RenderWidth > 0 ? _context._RenderWidth : SequenceHeaderObu == null ? 0 : SequenceHeaderObu._MaxFrameWidthMinus1 + 1;

        private int Height => _context._RenderHeight > 0 ? _context._RenderHeight : SequenceHeaderObu == null ? 0 : SequenceHeaderObu._MaxFrameHeightMinus1 + 1;

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

            visualSampleEntry.Width = (ushort)Width;
            visualSampleEntry.Height = (ushort)Height;
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
            tkhd.Width = (uint)Width << 16;
            tkhd.Height = (uint)Height << 16;
        }

        /// <summary>
        /// The OBUs of a sample, each with its header and size, as a slice of the sample: what given to
        /// <see cref="ProcessSample(byte[], int, int, out ArraySegment{byte}, out bool)"/> one at a time makes the sample again.
        /// </summary>
        public override IEnumerable<ArraySegment<byte>> ParseSample(byte[] sample, int sampleOffset, int sampleLength)
        {
            var result = new List<ArraySegment<byte>>();
            int end = sampleOffset + sampleLength;
            for (int position = sampleOffset; position < end;)
            {
                if (!ObuSize(sample, position, end, out int total, out _))
                {
                    if (this.Logger.IsErrorEnabled) this.Logger.LogError($"An OBU past the {end - position} bytes left of its sample: the rest of it as one");
                    result.Add(new ArraySegment<byte>(sample, position, end - position));
                    break;
                }

                result.Add(new ArraySegment<byte>(sample, position, total));
                position += total;
            }

            return result;
        }

        /// <summary>The sequence header, which a decoder needs before the first sample; none before one has come.</summary>
        public override IEnumerable<byte[]> GetContainerSamples()
        {
            if (SequenceHeaderObuRaw == null)
                return Array.Empty<byte[]>();

            return new byte[][] { SequenceHeaderObuRaw };
        }

        /// <summary>A track of the same configuration: its sequence header read into it, so it writes a sample entry at once.</summary>
        public override ITrack Clone()
        {
            var clone = CopySettingsTo(new AV1Track(Timescale, DefaultSampleDuration));
            clone.ProcessConfiguration(SequenceHeaderObuRaw);
            return clone;
        }
    }
}
