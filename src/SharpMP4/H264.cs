using SharpH264;
using SharpH26X;
using SharpISOBMFF;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SharpMP4
{
    /// <summary>
    /// H264 track.
    /// </summary>
    /// <remarks>
    /// https://www.itu.int/rec/T-REC-H.264/en
    /// https://yumichan.net/video-processing/video-compression/introduction-to-h264-nal-unit/
    /// https://stackoverflow.com/questions/38094302/how-to-understand-header-of-h264
    /// </remarks>
    public class H264Track : TrackBase
    {
        public const string BRAND = "avc1";
        public override string HandlerName => HandlerNames.Video;
        public override string HandlerType => HandlerTypes.Video;
        public override string Language { get; set; } = "eng";

        private List<byte[]> _nalBuffer = new List<byte[]>();
        private bool _nalBufferContainsVCL = false;

        /// <summary>
        /// SPS (Sequence Parameter Set) NAL units.
        /// </summary>
        public Dictionary<ulong, SeqParameterSetRbsp> Sps { get; set; } = new Dictionary<ulong, SeqParameterSetRbsp>();
        public Dictionary<ulong, byte[]> SpsRaw { get; set; } = new Dictionary<ulong, byte[]>();

        /// <summary>
        /// PPS (Picture Parameter Set) NAL units.
        /// </summary>
        public Dictionary<ulong, PicParameterSetRbsp> Pps { get; set; } = new Dictionary<ulong, PicParameterSetRbsp>();
        public Dictionary<ulong, byte[]> PpsRaw { get; set; } = new Dictionary<ulong, byte[]>();

        /// <summary>
        /// Overrides any auto-detected timescale.
        /// </summary>
        public uint TimescaleOverride { get; set; } = 0;

        /// <summary>
        /// Overrides any auto-detected frame tick.
        /// </summary>
        public uint FrameTickOverride { get; set; } = 0;

        /// <summary>
        /// If it is not possible to retrieve timescale from the SPS, use this value as a fallback.
        /// </summary>
        public uint TimescaleFallback { get; set; } = 600;

        /// <summary>
        /// If it is not possible to retrieve frame tick from the SPS, use this value as a fallback.
        /// </summary>
        public uint FrameTickFallback { get; set; } = 25;

        private H264Context _context = new H264Context();

        /// <summary>
        /// Ctor.
        /// </summary>
        public H264Track()
        {
            this.CompatibleBrand = BRAND; // avc1
            this.DefaultSampleFlags = new SampleFlags() { SampleDependsOn = 1, SampleIsDifferenceSample = true };
        }

        /// <summary>
        /// Process 1 NAL (Network Abstraction Layer) unit.
        /// </summary>
        /// <param name="sample">NAL bytes.</param>
        /// <returns><see cref="Task"/></returns>
        public override async Task ProcessSampleAsync(byte[] sample)
        {
            using (ItuStream stream = new ItuStream(new MemoryStream(sample)))
            {
                ulong ituSize = 0;
                var nu = new SharpH264.NalUnit((uint)sample.Length);
                _context.NalHeader = nu;
                ituSize += nu.Read(_context, stream);

                if (nu.NalUnitType == H264NALTypes.SPS)
                {
                    _context.SeqParameterSetRbsp = new SharpH264.SeqParameterSetRbsp();
                    _context.SeqParameterSetRbsp.Read(_context, stream);
                    if (!Sps.ContainsKey(_context.SeqParameterSetRbsp.SeqParameterSetData.SeqParameterSetId))
                    {
                        Sps.Add(_context.SeqParameterSetRbsp.SeqParameterSetData.SeqParameterSetId, _context.SeqParameterSetRbsp);
                        SpsRaw.Add(_context.SeqParameterSetRbsp.SeqParameterSetData.SeqParameterSetId, sample);
                    }

                    // if SPS contains the timescale, set it
                    if (Timescale == 0 || SampleDuration == 0)
                    {
                        var timescale = CalculateTimescale(_context.SeqParameterSetRbsp);
                        if (timescale.Timescale != 0 && timescale.FrameTick != 0)
                        {
                            Timescale = timescale.Timescale; // MaxFPS = Ceil( time_scale / ( 2 * num_units_in_tick ) )
                            SampleDuration = timescale.FrameTick * 2;
                        }
                        else
                        {
                            Timescale = TimescaleFallback;
                            SampleDuration = FrameTickFallback;
                        }
                    }

                    if (TimescaleOverride != 0)
                    {
                        Timescale = TimescaleOverride;
                    }
                    if (FrameTickOverride != 0)
                    {
                        SampleDuration = FrameTickOverride;
                    }
                }
                else if (nu.NalUnitType == H264NALTypes.PPS)
                {
                    _context.PicParameterSetRbsp = new SharpH264.PicParameterSetRbsp();
                    _context.PicParameterSetRbsp.Read(_context, stream);
                    if (!Pps.ContainsKey(_context.PicParameterSetRbsp.PicParameterSetId))
                    {
                        Pps.Add(_context.PicParameterSetRbsp.PicParameterSetId, _context.PicParameterSetRbsp);
                        PpsRaw.Add(_context.PicParameterSetRbsp.PicParameterSetId, sample);
                    }
                }
                else if (nu.NalUnitType == H264NALTypes.SEI)
                {
                    if (_nalBufferContainsVCL)
                    {
                        await CreateSample();
                    }

                    _nalBuffer.Add(sample);
                }
                else
                {
                    if (nu.NalUnitType >= 1 && nu.NalUnitType <= 5)
                    {
                        if ((sample[1] & 0x80) != 0) // https://stackoverflow.com/questions/69373668/ffmpeg-error-first-slice-in-a-frame-missing-when-decoding-h-265-stream
                        {
                            if (_nalBufferContainsVCL)
                            {
                                await CreateSample();
                            }
                        }

                        _nalBufferContainsVCL = true;
                    }

                    _nalBuffer.Add(sample);
                }
            }
        }

        /// <summary>
        /// Flush all remaining NAL units from the buffer.
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        public override async Task FlushAsync()
        {
            if (_nalBuffer.Count == 0 || !_nalBufferContainsVCL)
                return;

            await CreateSample();
        }

        private async Task CreateSample()
        {
            if (_nalBuffer.Count == 0)
                return;

            IEnumerable<byte> result = new byte[0];

            foreach (var nal in _nalBuffer)
            {
                int len = nal.Length;

                // for each NAL, add 4 byte NAL size
                byte[] size = new byte[]
                {
                    (byte)((len & 0xff000000) >> 24),
                    (byte)((len & 0xff0000) >> 16),
                    (byte)((len & 0xff00) >> 8),
                    (byte)(len & 0xff)
                };
                result = result.Concat(size).Concat(nal);
            }

            if (Log.DebugEnabled) Log.Debug($"AU: {_nalBuffer.Count}");

            await base.ProcessSampleAsync(result.ToArray());
            _nalBuffer.Clear();
            _nalBufferContainsVCL = false;
        }

        public override Box CreateSampleEntryBox()
        {
            var sps = this.Sps.First().Value;
            var dim = CalculateDimensions(sps);

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
           
            visualSampleEntry.Width = (ushort)dim.Width;
            visualSampleEntry.Height = (ushort)dim.Height;
            visualSampleEntry.Compressorname = BinaryUTF8String.GetBytes("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0");

            AVCConfigurationBox avcConfigurationBox = new AVCConfigurationBox();
            avcConfigurationBox.SetParent(visualSampleEntry);

            avcConfigurationBox._AVCConfig = new AVCDecoderConfigurationRecord();
            avcConfigurationBox._AVCConfig.Reserved0 = 0; // TODO: Investigate this value
            avcConfigurationBox._AVCConfig.SequenceParameterSetNALUnit = this.SpsRaw.Values.ToArray();
            avcConfigurationBox._AVCConfig.SequenceParameterSetLength = this.SpsRaw.Values.Select(x => (ushort)x.Length).ToArray();
            avcConfigurationBox._AVCConfig.NumOfSequenceParameterSets = (byte)this.Sps.Count;
            avcConfigurationBox._AVCConfig.PictureParameterSetNALUnit = this.PpsRaw.Values.ToArray();
            avcConfigurationBox._AVCConfig.PictureParameterSetLength = this.PpsRaw.Values.Select(x => (ushort)x.Length).ToArray();
            avcConfigurationBox._AVCConfig.NumOfPictureParameterSets = (byte)this.Pps.Count;
            avcConfigurationBox._AVCConfig._AVCLevelIndication = (byte)sps.SeqParameterSetData.LevelIdc;
            avcConfigurationBox._AVCConfig._AVCProfileIndication = (byte)sps.SeqParameterSetData.ProfileIdc;
            avcConfigurationBox._AVCConfig.BitDepthLumaMinus8 = (byte)sps.SeqParameterSetData.BitDepthLumaMinus8;
            avcConfigurationBox._AVCConfig.BitDepthChromaMinus8 = (byte)sps.SeqParameterSetData.BitDepthChromaMinus8;
            avcConfigurationBox._AVCConfig.ChromaFormat = (byte)sps.SeqParameterSetData.ChromaFormatIdc;
            avcConfigurationBox._AVCConfig.ConfigurationVersion = 1;
            avcConfigurationBox._AVCConfig.LengthSizeMinusOne = 3;
            avcConfigurationBox._AVCConfig.ProfileCompatibility =
                (byte)((sps.SeqParameterSetData.ConstraintSet0Flag != 0 ? 128 : 0) +
                       (sps.SeqParameterSetData.ConstraintSet1Flag != 0 ? 64 : 0) +
                       (sps.SeqParameterSetData.ConstraintSet2Flag != 0 ? 32 : 0) +
                       (sps.SeqParameterSetData.ConstraintSet3Flag != 0 ? 16 : 0) +
                       (sps.SeqParameterSetData.ConstraintSet4Flag != 0 ? 8 : 0) +
                       (sps.SeqParameterSetData.ReservedZero2bits & 0x3));
            visualSampleEntry.Children.Add(avcConfigurationBox);

            return visualSampleEntry;
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            var dim = CalculateDimensions(Sps.FirstOrDefault().Value);
            // convert to fixed point 1616
            tkhd.Width = dim.Width << 16; // TODO: simplify API
            tkhd.Height = dim.Height << 16; // TODO: simplify API
        }

        public (uint Width, uint Height) CalculateDimensions(SeqParameterSetRbsp sps)
        {
            var spsData = sps.SeqParameterSetData;
            ulong width = (spsData.PicWidthInMbsMinus1 + 1) * 16;
            ulong mult = 2;
            if (spsData.FrameMbsOnlyFlag != 0)
            {
                mult = 1;
            }
            ulong height = 16 * (spsData.PicHeightInMapUnitsMinus1 + 1) * mult;
            if (spsData.FrameCroppingFlag != 0)
            {
                ulong chromaFormat = spsData.ChromaFormatIdc;
                ulong chromaArrayType = 0;
                if (spsData.SeparateColourPlaneFlag == 0)
                {
                    chromaArrayType = chromaFormat;
                }

                ulong cropUnitX = 1;
                ulong cropUnitY = mult;
                if (chromaArrayType != 0)
                {
                    uint subWidth = 2;
                    uint subHeight = 1;
                    if(chromaFormat == 3) 
                        subWidth = 1;
                    if(chromaFormat == 1) 
                        subHeight = 2;

                    cropUnitX = subWidth;
                    cropUnitY = subHeight * mult;
                }

                width -= cropUnitX * (spsData.FrameCropLeftOffset + spsData.FrameCropRightOffset);
                height -= cropUnitY * (spsData.FrameCropTopOffset + spsData.FrameCropBottomOffset);
            }
            return ((uint)width, (uint)height);
        }

        private (uint Timescale, uint FrameTick) CalculateTimescale(SeqParameterSetRbsp sps)
        {
            uint timescale = 0;
            uint frametick = 0;
            var vui = sps.SeqParameterSetData.VuiParameters;
            if (vui != null && vui.TimingInfoPresentFlag != 0)
            {
                // MaxFPS = Ceil( time_scale / ( 2 * num_units_in_tick ) )
                timescale = vui.TimeScale;
                frametick = vui.NumUnitsInTick;

                if (timescale == 0 || frametick == 0)
                {
                    if (Log.WarnEnabled) Log.Warn($"Invalid values in vui: timescale: {timescale} and frametick: {frametick}.");
                    timescale = 0;
                    frametick = 0;
                }
            }
            else
            {
                if (Log.WarnEnabled) Log.Warn("Can't determine frame rate because SPS does not contain vuiParams");
            }

            return (timescale, frametick);
        }
    }
}
