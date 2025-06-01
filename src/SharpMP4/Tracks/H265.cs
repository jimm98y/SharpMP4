using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SharpH265;
using SharpH26X;
using SharpISOBMFF;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// H265 track.
    /// </summary>
    /// <remarks>https://www.itu.int/rec/T-REC-H.265/en</remarks>
    public class H265Track : TrackBase
    {
        public const string BRAND = "hvc1";

        private List<byte[]> _nalBuffer = new List<byte[]>();
        private bool _nalBufferContainsVCL = false;

        /// <summary>
        /// VPS (Video Parameter Set) NAL units.
        /// </summary>
        public Dictionary<ulong, VideoParameterSetRbsp> Vps { get; set; } = new Dictionary<ulong, VideoParameterSetRbsp>();
        public Dictionary<ulong, byte[]> VpsRaw { get; set; } = new Dictionary<ulong, byte[]>();

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

        public override string HandlerName => "VideoHandler\0"; //HandlerNames.Video;
        public override string HandlerType => HandlerTypes.Video;
        public override string Language { get; set; } = "und";

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
        public uint TimescaleFallback { get; set; } = 24000;

        /// <summary>
        /// If it is not possible to retrieve frame tick from the SPS, use this value as a fallback.
        /// </summary>
        public uint FrameTickFallback { get; set; } = 1001;

        private H265Context _context = new H265Context();

        /// <summary>
        /// Ctor.
        /// </summary>
        public H265Track()
        {
            CompatibleBrand = BRAND; // hvc1
            DefaultSampleFlags = new SampleFlags() { SampleDependsOn = 1, SampleIsDifferenceSample = true };
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
                // https://www.bing.com/search?pglt=43&q=hevc+hvc1&cvid=a9d4d6d6a96a4cddb12f3d94ac802bdd&gs_lcrp=EgZjaHJvbWUyBggAEEUYOTIGCAEQABhAMgYIAhAAGEAyBggDEAAYQDIGCAQQABhAMgYIBRAAGEAyBggGEEUYPNIBCDMyMjhqMGoxqAIAsAIA&FORM=ANNTA1&PC=U531
                // for hvc1, SPS, PPS, VPS should not be in MDAT
                // for hev1, SPS, PPS, VPS may be in MDAT
                ulong ituSize = 0;
                var nu = new NalUnit((uint)sample.Length);
                _context.NalHeader = nu;
                ituSize += nu.Read(_context, stream);

                if (nu.NalUnitHeader.NalUnitType == H265NALTypes.SPS_NUT)
                {
                    _context.SeqParameterSetRbsp = new SeqParameterSetRbsp();
                    _context.SeqParameterSetRbsp.Read(_context, stream);
                    if (!Sps.ContainsKey(_context.SeqParameterSetRbsp.SpsSeqParameterSetId))
                    {
                        Sps.Add(_context.SeqParameterSetRbsp.SpsSeqParameterSetId, _context.SeqParameterSetRbsp);
                        SpsRaw.Add(_context.SeqParameterSetRbsp.SpsSeqParameterSetId, sample);
                    }

                    // if SPS contains the timescale, set it
                    if (Timescale == 0 || SampleDuration == 0)
                    {
                        var timescale = CalculateTimescale(_context.SeqParameterSetRbsp);
                        if (timescale.Timescale != 0 && timescale.FrameTick != 0)
                        {
                            Timescale = timescale.Timescale;
                            SampleDuration = timescale.FrameTick;
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
                else if (nu.NalUnitHeader.NalUnitType == H265NALTypes.PPS_NUT)
                {
                    _context.PicParameterSetRbsp = new PicParameterSetRbsp();
                    _context.PicParameterSetRbsp.Read(_context, stream);
                    if (!Pps.ContainsKey(_context.PicParameterSetRbsp.PpsPicParameterSetId))
                    {
                        Pps.Add(_context.PicParameterSetRbsp.PpsPicParameterSetId, _context.PicParameterSetRbsp);
                        PpsRaw.Add(_context.PicParameterSetRbsp.PpsPicParameterSetId, sample);
                    }
                }
                else if (nu.NalUnitHeader.NalUnitType == H265NALTypes.VPS_NUT)
                {
                    _context.VideoParameterSetRbsp = new VideoParameterSetRbsp();
                    _context.VideoParameterSetRbsp.Read(_context, stream);
                    if (!Vps.ContainsKey(_context.VideoParameterSetRbsp.VpsVideoParameterSetId))
                    {
                        Vps.Add(_context.VideoParameterSetRbsp.VpsVideoParameterSetId, _context.VideoParameterSetRbsp);
                        VpsRaw.Add(_context.VideoParameterSetRbsp.VpsVideoParameterSetId, sample);
                    }
                }
                else if (nu.NalUnitHeader.NalUnitType == H265NALTypes.PREFIX_SEI_NUT || nu.NalUnitHeader.NalUnitType == H265NALTypes.SUFFIX_SEI_NUT)
                {
                    if (_nalBufferContainsVCL)
                    {
                        await CreateSample();
                    }

                    _nalBuffer.Add(sample);
                }
                else
                {
                    if (nu.NalUnitHeader.NalUnitType >= 0 && nu.NalUnitHeader.NalUnitType <= 31)
                    {
                        if ((sample[2] & 0x80) != 0) // https://stackoverflow.com/questions/69373668/ffmpeg-error-first-slice-in-a-frame-missing-when-decoding-h-265-stream
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

        private (uint Timescale, uint FrameTick) CalculateTimescale(SeqParameterSetRbsp sps)
        {
            uint timescale = 0;
            uint frametick = 0;
            var vui = sps.VuiParameters;
            if (vui != null && vui.VuiTimingInfoPresentFlag != 0)
            {
                // MaxFPS = Ceil( time_scale / ( 2 * num_units_in_tick ) )
                timescale = vui.VuiTimeScale;
                frametick = vui.VuiNumUnitsInTick;

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

            await base.ProcessSampleAsync(result.ToArray());
            _nalBuffer.Clear();
        }

        public override Box CreateSampleEntryBox()
        {
            var sps = Sps.First().Value;
            var vps = Vps.First().Value;
            var dim = CalculateDimensions(sps);

            VisualSampleEntry visualSampleEntry = new VisualSampleEntry(IsoStream.FromFourCC(BRAND));
            visualSampleEntry.Children = new List<Box>();
            visualSampleEntry.DataReferenceIndex = 1;
            visualSampleEntry.Depth = 24;
            visualSampleEntry.FrameCount = 1;
            visualSampleEntry.Horizresolution = 72 << 16; // TODO simplify API
            visualSampleEntry.Vertresolution = 72 << 16; // TODO simplify API
            visualSampleEntry.Width = (ushort)dim.Width;
            visualSampleEntry.Height = (ushort)dim.Height;
            visualSampleEntry.ReservedSampleEntry = new byte[6];
            visualSampleEntry.PreDefined0 = new uint[3];
            visualSampleEntry.Compressorname = BinaryUTF8String.GetBytes("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0");

            HEVCConfigurationBox hevcConfigurationBox = new HEVCConfigurationBox();
            hevcConfigurationBox.SetParent(visualSampleEntry);
            hevcConfigurationBox._HEVCConfig = new HEVCDecoderConfigurationRecord();
            hevcConfigurationBox._HEVCConfig.ConfigurationVersion = 1;
            hevcConfigurationBox._HEVCConfig.GeneralProfileIdc = (byte)sps.ProfileTierLevel.GeneralProfileIdc;
            hevcConfigurationBox._HEVCConfig.ChromaFormat = (byte)sps.ChromaFormatIdc;
            hevcConfigurationBox._HEVCConfig.GeneralLevelIdc = (byte)sps.ProfileTierLevel.GeneralLevelIdc;
            hevcConfigurationBox._HEVCConfig.GeneralProfileCompatibilityFlags = BuildGeneralProfileCompatibilityFlags(sps);
            hevcConfigurationBox._HEVCConfig.GeneralConstraintIndicatorFlags = BuildGeneralProfileConstraintIndicatorFlags(vps);
            hevcConfigurationBox._HEVCConfig.BitDepthChromaMinus8 = (byte)sps.BitDepthChromaMinus8;
            hevcConfigurationBox._HEVCConfig.BitDepthLumaMinus8 = (byte)sps.BitDepthLumaMinus8;
            hevcConfigurationBox._HEVCConfig.TemporalIdNested = sps.SpsTemporalIdNestingFlag != 0;
            hevcConfigurationBox._HEVCConfig.LengthSizeMinusOne = 3; // 4 bytes size block inserted in between NAL units

            hevcConfigurationBox._HEVCConfig.Reserved4 = new bool[3];
            hevcConfigurationBox._HEVCConfig.NumOfArrays = 3;
            hevcConfigurationBox._HEVCConfig.ArrayCompleteness = new bool[3] { true, true, true };
            hevcConfigurationBox._HEVCConfig.NumNalus = new ushort[3] { (ushort)VpsRaw.Count, (ushort)SpsRaw.Count, (ushort)PpsRaw.Count };
            hevcConfigurationBox._HEVCConfig.NALUnitType = new byte[3] { (byte)H265NALTypes.VPS_NUT, (byte)H265NALTypes.SPS_NUT, (byte)H265NALTypes.PPS_NUT };
            
            // correct order is VPS, SPS, PPS. Other order produced ffmpeg errors such as "VPS 0 does not exist" and "SPS 0 does not exist."
            hevcConfigurationBox._HEVCConfig.NalUnit = new byte[3][][];
            hevcConfigurationBox._HEVCConfig.NalUnitLength = new ushort[3][];
            hevcConfigurationBox._HEVCConfig.NalUnit[0] = new byte[VpsRaw.Count][];
            hevcConfigurationBox._HEVCConfig.NalUnit[1] = new byte[SpsRaw.Count][];
            hevcConfigurationBox._HEVCConfig.NalUnit[2] = new byte[PpsRaw.Count][];
            hevcConfigurationBox._HEVCConfig.NalUnitLength[0] = new ushort[VpsRaw.Count];
            hevcConfigurationBox._HEVCConfig.NalUnitLength[1] = new ushort[SpsRaw.Count];
            hevcConfigurationBox._HEVCConfig.NalUnitLength[2] = new ushort[PpsRaw.Count];

            for (int i = 0; i < VpsRaw.Count; i++)
            {
                hevcConfigurationBox._HEVCConfig.NalUnit[0][i] = VpsRaw.Values.ElementAt(i);
                hevcConfigurationBox._HEVCConfig.NalUnitLength[0][i] = (ushort)VpsRaw.Values.ElementAt(i).Length;
            }

            for (int i = 0; i < SpsRaw.Count; i++)
            {
                hevcConfigurationBox._HEVCConfig.NalUnit[1][i] = SpsRaw.Values.ElementAt(i);
                hevcConfigurationBox._HEVCConfig.NalUnitLength[1][i] = (ushort)SpsRaw.Values.ElementAt(i).Length;
            }

            for (int i = 0; i < PpsRaw.Count; i++)
            {
                hevcConfigurationBox._HEVCConfig.NalUnit[2][i] = PpsRaw.Values.ElementAt(i);
                hevcConfigurationBox._HEVCConfig.NalUnitLength[2][i] = (ushort)PpsRaw.Values.ElementAt(i).Length;
            }
                        
            visualSampleEntry.Children.Add(hevcConfigurationBox);

            return visualSampleEntry;
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            var dim = CalculateDimensions(Sps.FirstOrDefault().Value);
            tkhd.Width = dim.Width << 16; // TODO: simplify API
            tkhd.Height = dim.Height << 16; // TODO: simplify API
        }

        public (uint Width, uint Height) CalculateDimensions(SeqParameterSetRbsp sps)
        {
            ulong width = sps.PicWidthInLumaSamples;
            ulong height = sps.PicHeightInLumaSamples;
            return ((uint)width, (uint)height);
        }

        public uint BuildGeneralProfileCompatibilityFlags(SeqParameterSetRbsp sps)
        {
            using (var ms = new MemoryStream())
            {
                IsoStream isoStream = new IsoStream(ms);
                for (int i = 0; i < 32; i++)
                {
                    isoStream.WriteBit(sps.ProfileTierLevel.GeneralProfileCompatibilityFlag[i]);
                }

                var bytes = ms.ToArray();
                uint ret = 
                    ((uint)bytes[0] << 24) +
                    ((uint)bytes[1] << 16) +
                    ((uint)bytes[2] << 8) +
                    bytes[3];
                return ret;
            }
        }

        public ulong BuildGeneralProfileConstraintIndicatorFlags(VideoParameterSetRbsp vps)
        {
            using (var ms = new MemoryStream())
            {
                IsoStream isoStream = new IsoStream(ms);
                isoStream.WriteBit(vps.ProfileTierLevel.GeneralProgressiveSourceFlag);
                isoStream.WriteBit(vps.ProfileTierLevel.GeneralInterlacedSourceFlag);
                isoStream.WriteBit(vps.ProfileTierLevel.GeneralNonPackedConstraintFlag);
                isoStream.WriteBit(vps.ProfileTierLevel.GeneralFrameOnlyConstraintFlag);

                if (vps.ProfileTierLevel.GeneralProfileIdc == 4 || vps.ProfileTierLevel.GeneralProfileCompatibilityFlag[4] != 0 ||
                    vps.ProfileTierLevel.GeneralProfileIdc == 5 || vps.ProfileTierLevel.GeneralProfileCompatibilityFlag[5] != 0 ||
                    vps.ProfileTierLevel.GeneralProfileIdc == 6 || vps.ProfileTierLevel.GeneralProfileCompatibilityFlag[6] != 0 ||
                    vps.ProfileTierLevel.GeneralProfileIdc == 7 || vps.ProfileTierLevel.GeneralProfileCompatibilityFlag[7] != 0 ||
                    vps.ProfileTierLevel.GeneralProfileIdc == 8 || vps.ProfileTierLevel.GeneralProfileCompatibilityFlag[8] != 0 ||
                    vps.ProfileTierLevel.GeneralProfileIdc == 9 || vps.ProfileTierLevel.GeneralProfileCompatibilityFlag[9] != 0 ||
                    vps.ProfileTierLevel.GeneralProfileIdc == 10 || vps.ProfileTierLevel.GeneralProfileCompatibilityFlag[10] != 0)
                {
                    isoStream.WriteBit(vps.ProfileTierLevel.GeneralMax12bitConstraintFlag);
                    isoStream.WriteBit(vps.ProfileTierLevel.GeneralMax10bitConstraintFlag);
                    isoStream.WriteBit(vps.ProfileTierLevel.GeneralMax8bitConstraintFlag);
                    isoStream.WriteBit(vps.ProfileTierLevel.GeneralMax422chromaConstraintFlag);
                    isoStream.WriteBit(vps.ProfileTierLevel.GeneralMax420chromaConstraintFlag);
                    isoStream.WriteBit(vps.ProfileTierLevel.GeneralMaxMonochromeConstraintFlag);
                    isoStream.WriteBit(vps.ProfileTierLevel.GeneralIntraConstraintFlag);
                    isoStream.WriteBit(vps.ProfileTierLevel.GeneralOnePictureOnlyConstraintFlag);
                    isoStream.WriteBit(vps.ProfileTierLevel.GeneralLowerBitRateConstraintFlag);

                    if (vps.ProfileTierLevel.GeneralProfileIdc == 5 || vps.ProfileTierLevel.GeneralProfileCompatibilityFlag[5] != 0 ||
                        vps.ProfileTierLevel.GeneralProfileIdc == 9 || vps.ProfileTierLevel.GeneralProfileCompatibilityFlag[9] != 0 ||
                        vps.ProfileTierLevel.GeneralProfileIdc == 10 || vps.ProfileTierLevel.GeneralProfileCompatibilityFlag[10] != 0)
                    {
                        isoStream.WriteBit(vps.ProfileTierLevel.GeneralMax14bitConstraintFlag);
                        isoStream.WriteBits(33, vps.ProfileTierLevel.GeneralReservedZero33bits);
                    }
                    else
                    {
                        isoStream.WriteBits(34, vps.ProfileTierLevel.GeneralReservedZero34bits);
                    }
                }
                else if (vps.ProfileTierLevel.GeneralProfileIdc == 2 || vps.ProfileTierLevel.GeneralProfileCompatibilityFlag[2] != 0)
                {
                    isoStream.WriteBits(7, vps.ProfileTierLevel.GeneralReservedZero7bits);
                    isoStream.WriteBit(vps.ProfileTierLevel.GeneralOnePictureOnlyConstraintFlag);
                    isoStream.WriteBits(35, vps.ProfileTierLevel.GeneralReservedZero35bits);
                }
                else
                {
                    isoStream.WriteBits(43, vps.ProfileTierLevel.GeneralReservedZero43bits);
                }

                if (vps.ProfileTierLevel.GeneralProfileIdc >= 1 && vps.ProfileTierLevel.GeneralProfileIdc <= 5 ||
                    vps.ProfileTierLevel.GeneralProfileIdc == 9 ||
                    vps.ProfileTierLevel.GeneralProfileCompatibilityFlag[1] != 0 || vps.ProfileTierLevel.GeneralProfileCompatibilityFlag[2] != 0 ||
                    vps.ProfileTierLevel.GeneralProfileCompatibilityFlag[3] != 0 || vps.ProfileTierLevel.GeneralProfileCompatibilityFlag[4] != 0 ||
                    vps.ProfileTierLevel.GeneralProfileCompatibilityFlag[5] != 0 || vps.ProfileTierLevel.GeneralProfileCompatibilityFlag[9] != 0)
                {
                    isoStream.WriteBit(vps.ProfileTierLevel.GeneralInbldFlag);
                }
                else
                {
                    isoStream.WriteBit(vps.ProfileTierLevel.GeneralReservedZeroBit);
                }

                var bytes = ms.ToArray();
                ulong ret =
                    ((ulong)bytes[0] << 40) +
                    ((ulong)bytes[1] << 32) +
                    ((ulong)bytes[2] << 24) +
                    ((ulong)bytes[3] << 16) +
                    ((ulong)bytes[4] << 8) +
                    ((ulong)bytes[5]);
                return ret;
            }
        }
    }
}
