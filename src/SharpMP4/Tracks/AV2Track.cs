using SharpAV2;
using SharpAVX;
using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.IO;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// AV2 Track.
    /// </summary>
    /// <remarks>
    /// https://av2.aomedia.org/v1.0.0/ and the AV2 Codec ISO Media File Format Binding (working group draft,
    /// https://github.com/AOMediaCodec/av2-isobmff): a sample is a temporal unit, its OBUs each led by a
    /// leb128() num_bytes_in_obu (the length delimited format of AV2 Annex B); the sequence header, the
    /// layer configuration record and the content interpretation are in the 'av2C' box, not in the samples.
    /// </remarks>
    public class AV2Track : TrackBase
    {
        public const string BRAND = "av02";

        public override string HandlerName => HandlerNames.Video;
        public override string HandlerType => HandlerTypes.Video;
        public override string Language { get; set; } = "eng";

        private readonly AV2Context _context = new AV2Context();

        // The OBUs of the 'av2C' box, each with its length, in the order they came: the first sequence header
        // and the first of each other OBU of the types the box carries.
        private readonly List<byte[]> _configObus = new List<byte[]>();

        // The first coded frame OBU of each extended layer in the sample: the sample is a sync sample if
        // each is a closed loop key frame.
        private readonly Dictionary<int, int> _firstFrames = new Dictionary<int, int>();

        /// <summary>The first sequence header OBU, with its length, as it is in the 'av2C' box.</summary>
        public byte[] SequenceHeaderObuRaw { get; private set; }

        public AV2Track()
        {
            CompatibleBrand = BRAND; // av02
            DefaultSampleFlags = new SampleFlags() { SampleDependsOn = 1, SampleIsDifferenceSample = true };
            TimescaleFallback = 24000;
            FrameTickFallback = 1001;
        }

        public AV2Track(uint timescale, int sampleDuration) : this()
        {
            Timescale = timescale;
            DefaultSampleDuration = sampleDuration;
        }

        public AV2Track(Box config, uint timescale, int sampleDuration) : this(timescale, sampleDuration)
        {
            AV2CodecConfigurationBox av2C = config as AV2CodecConfigurationBox;
            if (av2C == null)
                throw new ArgumentException($"Invalid AV2CodecConfigurationBox: {config.FourCC}");

            foreach (byte[] obu in av2C.ConfigObus)
            {
                byte[] framed = Framed(obu);
                ProcessSample(framed, 0, framed.Length, out _, out _);
            }
        }

        /// <summary>
        /// Takes OBUs each led by its leb128() length, one or more: a temporal delimiter ends the sample
        /// before it, and a null buffer the last one.
        /// </summary>
        /// <param name="isRandomAccessPoint">true when the sample finished is a sync sample.</param>
        public override void ProcessSample(byte[] buffer, int offset, int length, out ArraySegment<byte> output, out bool isRandomAccessPoint)
        {
            output = default;
            isRandomAccessPoint = false;

            if (buffer == null)
            {
                // the last temporal unit
                output = FinishSample(out isRandomAccessPoint);
                return;
            }

            int end = offset + length;
            int position = offset;
            while (position < end)
            {
                int start = position;
                long size = ReadLeb128(buffer, ref position, end);
                if (size <= 0 || position + size > end)
                {
                    if (this.Logger.IsErrorEnabled) this.Logger.LogError($"An OBU of {size} bytes where {end - position} are left, dropping the rest");
                    return;
                }

                int obuType = (buffer[position] >> 2) & 0x1f;

                // Without the sequence header, the OBUs cannot be read
                if (SequenceHeaderObuRaw == null && obuType != AV2Context.OBU_SEQUENCE_HEADER && obuType != AV2Context.OBU_TEMPORAL_DELIMITER
                    && obuType != AV2Context.OBU_LAYER_CONFIGURATION_RECORD && obuType != AV2Context.OBU_CONTENT_INTERPRETATION)
                {
                    if (this.Logger.IsErrorEnabled) this.Logger.LogError("OBU Sequence Header missing, dropping OBU");
                    position += (int)size;
                    continue;
                }

                try
                {
                    using (var stream = new AomStream(new MemoryStream(buffer, position, (int)size)))
                        _context.Read(stream, (int)size);
                }
                catch (Exception ex)
                {
                    // Kept all the same: the sample is what the stream has
                    if (this.Logger.IsWarningEnabled) this.Logger.LogWarning($"OBU type {obuType} could not be read: {ex.Message}");
                }

                int total = position + (int)size - start;
                switch (obuType)
                {
                    case AV2Context.OBU_TEMPORAL_DELIMITER:
                        // The sample boundary is the temporal unit's: the delimiter is not in the sample
                        if (HasSample)
                        {
                            output = FinishSample(out isRandomAccessPoint);
                            // The sample finished lies where the next is put together: with the next
                            // temporal unit's OBUs in the same buffer - an IVF frame holds one whole - it is
                            // copied out before they are
                            if (position + size < end)
                                output = new ArraySegment<byte>(CopyOf(output.Array, output.Offset, output.Count));
                        }
                        break;

                    case AV2Context.OBU_SEQUENCE_HEADER:
                        if (SequenceHeaderObuRaw == null)
                        {
                            SequenceHeaderObuRaw = CopyOf(buffer, start, total);
                            // In the order of the temporal unit: a layer configuration record before it,
                            // as the draft has for a multistream
                            _configObus.Add(SequenceHeaderObuRaw);
                            SetTiming();
                        }
                        else if (!SameAs(SequenceHeaderObuRaw, buffer, start, total))
                        {
                            if (this.Logger.IsWarningEnabled) this.Logger.LogWarning("A sequence header other than the first: the track keeps the first, a sample entry holds one");
                        }
                        break;

                    case AV2Context.OBU_LAYER_CONFIGURATION_RECORD:
                    case AV2Context.OBU_CONTENT_INTERPRETATION:
                        // In the 'av2C' box, not in a sample
                        if (!_configObus.Exists(o => SameAs(o, buffer, start, total)))
                            _configObus.Add(CopyOf(buffer, start, total));
                        break;

                    case AV2Context.OBU_PADDING:
                        // Not in a sample: padding is the container's
                        break;

                    default:
                        if (IsCodedFrame(obuType) && !_firstFrames.ContainsKey(_context._ObuXlayerId))
                            _firstFrames[_context._ObuXlayerId] = obuType;
                        AppendToSample(buffer, start, total);
                        break;
                }

                position += (int)size;
            }
        }

        private ArraySegment<byte> FinishSample(out bool isRandomAccessPoint)
        {
            // A sync sample: the first coded frame of every extended layer in it a closed loop key frame
            isRandomAccessPoint = _firstFrames.Count > 0;
            foreach (int type in _firstFrames.Values)
                isRandomAccessPoint &= type == AV2Context.OBU_CLOSED_LOOP_KEY;
            _firstFrames.Clear();
            return TakeSample();
        }

        private void SetTiming()
        {
            if (Timescale == 0 || DefaultSampleDuration == 0)
            {
                Timescale = TimescaleFallback;
                DefaultSampleDuration = FrameTickFallback;
            }
            if (TimescaleOverride != 0)
                Timescale = TimescaleOverride;
            if (FrameTickOverride != 0)
                DefaultSampleDuration = FrameTickOverride;
        }

        /// <summary>The OBU types of a coded frame, a sync sample's first of which is a closed loop key frame.</summary>
        private static bool IsCodedFrame(int obuType)
        {
            switch (obuType)
            {
                case AV2Context.OBU_CLOSED_LOOP_KEY:
                case AV2Context.OBU_OPEN_LOOP_KEY:
                case AV2Context.OBU_LEADING_TILE_GROUP:
                case AV2Context.OBU_REGULAR_TILE_GROUP:
                case AV2Context.OBU_SWITCH:
                case AV2Context.OBU_LEADING_SEF:
                case AV2Context.OBU_REGULAR_SEF:
                case AV2Context.OBU_LEADING_TIP:
                case AV2Context.OBU_REGULAR_TIP:
                case AV2Context.OBU_BRIDGE_FRAME:
                case AV2Context.OBU_RAS_FRAME:
                    return true;
                default:
                    return false;
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

            visualSampleEntry.Width = (ushort)Width;
            visualSampleEntry.Height = (ushort)Height;
            visualSampleEntry.Compressorname = BinaryUTF8String.GetBytes("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0");

            AV2CodecConfigurationBox av2C = new AV2CodecConfigurationBox();
            av2C.SetParent(visualSampleEntry);
            av2C.Reserved1 = 0;
            av2C.ConfigObusCountMinus1 = (byte)Math.Max(0, _configObus.Count - 1);
            av2C.ConfigObu = _configObus.ToArray();
            visualSampleEntry.Children.Add(av2C);

            // Exactly one 'colr' of 'nclx': the stream's color description, or unspecified without one
            var (primaries, transfer, matrix, fullRange) = ColorDescription();
            ColourInformationBox colr = new ColourInformationBox();
            colr.SetParent(visualSampleEntry);
            colr.ColourType = IsoStream.FromFourCC("nclx");
            colr.ColourPrimaries = (ushort)primaries;
            colr.TransferCharacteristics = (ushort)transfer;
            colr.MatrixCoefficients = (ushort)matrix;
            colr.FullRangeFlag = fullRange;
            visualSampleEntry.Children.Add(colr);

            visualSampleEntry.Children.Add(PixelInformation(visualSampleEntry));

            return visualSampleEntry;
        }

        /// <summary>
        /// 'pixi' with the least significant bit of its flags - px_flags - set, as the draft has it: the bits
        /// of each channel, then what each is (ISO/IEC 23008-12:2024/CDAM 2:2025 6.5.6.3): colour, unsigned
        /// integers, and subsampled as the sequence header says, where the chroma samples are, as libavif
        /// signals it for AV1.
        /// </summary>
        private PixelInformationProperty PixelInformation(Box parent)
        {
            PixelInformationProperty pixi = new PixelInformationProperty(flags: 1);
            pixi.SetParent(parent);
            byte channels = (byte)(_context._NumPlanes > 0 ? _context._NumPlanes : 3);
            pixi.NumChannels = channels;
            pixi.BitsPerChannel = new byte[channels];
            pixi.ChannelIdc = new byte[channels];         // 0: colour or grayscale
            pixi.Reserved = new bool[channels];
            pixi.ComponentFormat = new byte[channels];    // 0: unsigned integer
            pixi.SubsamplingFlag = new bool[channels];
            pixi.ChannelLabelFlag = new bool[channels];
            pixi.SubsamplingType = new byte[channels];
            pixi.SubsamplingLocation = new byte[channels];
            pixi.ChannelLabel = new BinaryUTF8String[channels];

            // The chroma sample position, as ci_chroma_sample_position_top has it: 0 to 4 are those of
            // Chroma420SampleLocType (ISO/IEC 23091-4), which subsampling_location takes - AV1's vertical,
            // left, is 0 and its colocated, top left, 2 in libavif's mapping. Unknown, or bottom (5), which
            // subsampling_location has no value for, no subsampling is signalled, as libavif does.
            int position = _context._CiChromaSamplePositionPresentFlag != 0 ? _context._CiChromaSamplePositionTop : -1;
            bool signalled = position >= 0 && position <= 4;

            for (int i = 0; i < channels; i++)
            {
                pixi.BitsPerChannel[i] = (byte)(_context._BitDepth > 0 ? _context._BitDepth : 8);
                pixi.SubsamplingFlag[i] = signalled;
                if (signalled)
                {
                    byte type = SubsamplingType(i);
                    pixi.SubsamplingType[i] = type;
                    pixi.SubsamplingLocation[i] = (byte)(type == 0 ? 0 : position);
                }
            }
            return pixi;
        }

        /// <summary>
        /// subsampling_type of a channel (ISO/IEC 23008-12:2024/CDAM 2:2025 6.5.6.3): 0 4:4:4, 1 4:2:2, 2 4:2:0,
        /// 4 4:4:0 - the luma channel's is 4:4:4.
        /// </summary>
        private byte SubsamplingType(int channel)
        {
            if (channel == 0)
                return 0;
            if (_context._SubsamplingX == 0)
                return (byte)(_context._SubsamplingY == 0 ? 0 : 4);
            return (byte)(_context._SubsamplingY == 0 ? 1 : 2);
        }

        /// <summary>
        /// The color description of the content interpretation OBU, as Table 6.13 (AV2 6.10.6) gives it for
        /// ci_color_description_idc; unspecified (2) where the stream has none.
        /// </summary>
        private (int Primaries, int Transfer, int Matrix, bool FullRange) ColorDescription()
        {
            if (_context._CiColorDescriptionPresentFlag == 0)
                return (2, 2, 2, false);

            bool fullRange = _context._CiFullRangeFlag != 0;
            switch (_context._CiColorDescriptionIdc)
            {
                case 0: return (_context._CiColorPrimaries, _context._CiTransferCharacteristics, _context._CiMatrixCoefficients, fullRange);
                case 1: return (1, 1, 1, fullRange);   // BT.709 SDR
                case 2: return (9, 16, 9, fullRange);  // BT.2100 PQ
                case 3: return (9, 18, 9, fullRange);  // BT.2100 HLG
                case 4: return (1, 13, 0, fullRange);  // sRGB
                case 5: return (1, 13, 5, fullRange);  // sYCC
                default: return (2, 2, 2, fullRange);  // reserved
            }
        }

        /// <summary>The frame's size as shown: cropped (compute_image_size, AV2 5.18.4.4), or the sequence's largest before a frame.</summary>
        private int Width => _context._CropWidth > 0 ? _context._CropWidth : _context._MaxFrameWidthMinus1 + 1;

        private int Height => _context._CropHeight > 0 ? _context._CropHeight : _context._MaxFrameHeightMinus1 + 1;

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            tkhd.Width = (uint)Width << 16;
            tkhd.Height = (uint)Height << 16;
        }

        /// <summary>The OBUs of a sample, each with the leb128() length it is led by, as a slice of the sample.</summary>
        public override IEnumerable<ArraySegment<byte>> ParseSample(byte[] buffer, int offset, int length)
        {
            var result = new List<ArraySegment<byte>>();
            int end = offset + length;
            int position = offset;
            while (position < end)
            {
                int start = position;
                long size = ReadLeb128(buffer, ref position, end);
                if (size <= 0 || position + size > end)
                    break;

                try
                {
                    using (var stream = new AomStream(new MemoryStream(buffer, position, (int)size)))
                        _context.Read(stream, (int)size);
                }
                catch (Exception ex)
                {
                    if (this.Logger.IsWarningEnabled) this.Logger.LogWarning($"An OBU could not be read: {ex.Message}");
                }

                position += (int)size;
                result.Add(new ArraySegment<byte>(buffer, start, position - start));
            }
            return result;
        }

        /// <summary>The OBUs of the 'av2C' box, each with its length: what goes before the first sample.</summary>
        public override IEnumerable<byte[]> GetContainerSamples()
        {
            if (_configObus.Count == 0)
                return null;
            return _configObus.ToArray();
        }

        public override ITrack Clone()
        {
            return new AV2Track(Timescale, DefaultSampleDuration);
        }

        private static long ReadLeb128(byte[] buffer, ref int position, int end)
        {
            long value = 0;
            for (int i = 0; i < 8 && position < end; i++)
            {
                byte b = buffer[position++];
                value |= (long)(b & 0x7f) << (7 * i);
                if ((b & 0x80) == 0)
                    break;
            }
            return value;
        }

        /// <summary>An OBU led by its length, as the samples and the 'av2C' box hold it.</summary>
        private static byte[] Framed(byte[] obu)
        {
            var framed = new List<byte>();
            long size = obu.Length;
            do
            {
                byte b = (byte)(size & 0x7f);
                size >>= 7;
                framed.Add(size > 0 ? (byte)(b | 0x80) : b);
            } while (size > 0);
            framed.AddRange(obu);
            return framed.ToArray();
        }

        private static bool SameAs(byte[] kept, byte[] buffer, int offset, int length)
        {
            if (kept.Length != length)
                return false;
            for (int i = 0; i < length; i++)
            {
                if (kept[i] != buffer[offset + i])
                    return false;
            }
            return true;
        }
    }
}
