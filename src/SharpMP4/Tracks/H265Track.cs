using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpH265;
using SharpH26X;
using SharpISOBMFF;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// H265 track.
    /// </summary>
    /// <remarks>https://www.itu.int/rec/T-REC-H.265/en</remarks>
    public class H265Track : H26XTrackBase
    {
        public const string BRAND = "hvc1";

        private List<byte[]> _nalBuffer = new List<byte[]>();
        private bool _nalBufferContainsVCL = false;
        private bool _nalBufferContainsIDR = false;

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

        /// <summary>
        /// Prefix SEI (Supplementary Enhancement Information) NAL units.
        /// </summary>
        public List<SeiRbsp> PrefixSei { get; set; } = new List<SeiRbsp>();
        public List<byte[]> PrefixSeiRaw { get; set; } = new List<byte[]>();


        private H265Context _context = new H265Context();

        /// <summary>
        /// Ctor.
        /// </summary>
        public H265Track() : base()
        {
            CompatibleBrand = BRAND; // hvc1
        }

        public H265Track(uint timescale, int sampleDuration) : this()
        {
            Timescale = timescale;
            DefaultSampleDuration = sampleDuration;
        }

        public H265Track(Box config, uint timescale, int sampleDuration) : this(timescale, sampleDuration)
        {
            HEVCConfigurationBox hvcC = config as HEVCConfigurationBox;
            if (hvcC == null)
                throw new ArgumentException($"Invalid HEVCConfigurationBox: {config.FourCC}");

            NalLengthSize = hvcC._HEVCConfig.LengthSizeMinusOne + 1; // usually 4 bytes

            foreach (var nalus in hvcC._HEVCConfig.NalUnit)
            {
                foreach (var nalu in nalus)
                {
                    ProcessSample(nalu, out _, out _);
                }
            }
        }

        /// <summary>
        /// Process 1 NAL (Network Abstraction Layer) unit.
        /// </summary>
        /// <param name="sample">NAL bytes.</param>
        /// <param name="output">Returns a completed sample (1 AU) or null when no sample is currently available.</returns>
        /// <param name="isRandomAccessPoint">true when the sample contains a keyframe.</param>
        public override void ProcessSample(byte[] sample, out byte[] output, out bool isRandomAccessPoint)
        {
            isRandomAccessPoint = _nalBufferContainsIDR;
            output = null;

            if (sample == null)
            {
                // flush the last AU
                if (_nalBuffer.Count > 0 && _nalBufferContainsVCL)
                {
                    output = CreateSample(_nalBuffer);
                    _nalBuffer.Clear();
                    _nalBufferContainsVCL = false;
                    _nalBufferContainsIDR = false;
                }
                return;
            }

            // check for Annex-B
            if (sample.Length >= 3 && sample[0] == 0 && sample[1] == 0 && (sample[2] == 1 || (sample.Length >= 4 && sample[2] == 0 && sample[3] == 1)))
            {
                throw new ArgumentException("NAL unit must not have Annex-B prefix!");
            }

            using (ItuStream stream = new ItuStream(new MemoryStream(sample)))
            {
                // for hvc1, SPS, PPS, VPS should not be in MDAT
                // for hev1, SPS, PPS, VPS may be in MDAT
                ulong ituSize = 0;
                var nu = new NalUnit((uint)sample.Length);
                _context.NalHeader = nu;
                ituSize += nu.Read(_context, stream);

                if (nu.NalUnitHeader.NalUnitType == H265NALTypes.AUD_NUT)
                {
                    // access unit delimiter NAL unit with nuh_layer_id equal to 0(when present)
                    if (_nalBufferContainsVCL && nu.NalUnitHeader.NuhLayerId == 0)
                    {
                        output = CreateSample(_nalBuffer);
                        _nalBuffer.Clear();
                        _nalBufferContainsVCL = false;
                        _nalBufferContainsIDR = false;
                    }
                }
                else if (nu.NalUnitHeader.NalUnitType == H265NALTypes.SPS_NUT)
                {
                    _context.SeqParameterSetRbsp = new SeqParameterSetRbsp();
                    _context.SeqParameterSetRbsp.Read(_context, stream);
                    if (!Sps.ContainsKey(_context.SeqParameterSetRbsp.SpsSeqParameterSetId))
                    {
                        Sps.Add(_context.SeqParameterSetRbsp.SpsSeqParameterSetId, _context.SeqParameterSetRbsp);
                        SpsRaw.Add(_context.SeqParameterSetRbsp.SpsSeqParameterSetId, sample);
                    }

                    // if SPS contains the timescale, set it
                    if (Timescale == 0 || DefaultSampleDuration == 0)
                    {
                        var timescale = _context.SeqParameterSetRbsp.CalculateTimescale();
                        if (timescale.Timescale != 0 && timescale.FrameTick != 0)
                        {
                            Timescale = timescale.Timescale;
                            DefaultSampleDuration = (int)timescale.FrameTick;
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

                    // SPS NAL unit with nuh_layer_id equal to 0 (when present)
                    if (_nalBufferContainsVCL && nu.NalUnitHeader.NuhLayerId == 0)
                    {
                        output = CreateSample(_nalBuffer);
                        _nalBuffer.Clear();
                        _nalBufferContainsVCL = false;
                        _nalBufferContainsIDR = false;
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

                    // PPS NAL unit with nuh_layer_id equal to 0 (when present)
                    if (_nalBufferContainsVCL && nu.NalUnitHeader.NuhLayerId == 0)
                    {
                        output = CreateSample(_nalBuffer);
                        _nalBuffer.Clear();
                        _nalBufferContainsVCL = false;
                        _nalBufferContainsIDR = false;
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

                    // VPS NAL unit with nuh_layer_id equal to 0 (when present)
                    if (_nalBufferContainsVCL && nu.NalUnitHeader.NuhLayerId == 0)
                    {
                        output = CreateSample(_nalBuffer);
                        _nalBuffer.Clear();
                        _nalBufferContainsVCL = false;
                        _nalBufferContainsIDR = false;
                    }
                }
                else if (nu.NalUnitHeader.NalUnitType == H265NALTypes.PREFIX_SEI_NUT || nu.NalUnitHeader.NalUnitType == H265NALTypes.SUFFIX_SEI_NUT)
                {
                    if (nu.NalUnitHeader.NalUnitType == H265NALTypes.PREFIX_SEI_NUT)
                    {
                        _context.SeiRbsp = new SeiRbsp();
                        _context.SeiRbsp.Read(_context, stream);
                        if (!PrefixSei.Contains(_context.SeiRbsp))
                        {
                            PrefixSei.Add(_context.SeiRbsp);
                            PrefixSeiRaw.Add(sample);
                        }
                    }

                    // Prefix SEI NAL unit with nuh_layer_id equal to 0 (when present)
                    if (_nalBufferContainsVCL && nu.NalUnitHeader.NuhLayerId == 0)
                    {
                        output = CreateSample(_nalBuffer);
                        _nalBuffer.Clear();
                        _nalBufferContainsVCL = false;
                        _nalBufferContainsIDR = false;
                    }

                    _nalBuffer.Add(sample);
                }
                else if(nu.NalUnitHeader.NalUnitType >= H265NALTypes.RSV_NVCL41 && nu.NalUnitHeader.NalUnitType <= H265NALTypes.RSV_NVCL44)
                {
                    // NAL units with nal_unit_type in the range of RSV_NVCL41..RSV_NVCL44 with nuh_layer_id equal to 0 (when present)
                    if (_nalBufferContainsVCL && nu.NalUnitHeader.NuhLayerId == 0)
                    {
                        output = CreateSample(_nalBuffer);
                        _nalBuffer.Clear();
                        _nalBufferContainsVCL = false;
                        _nalBufferContainsIDR = false;
                    }
                }
                else if (nu.NalUnitHeader.NalUnitType >= H265NALTypes.UNSPEC48 && nu.NalUnitHeader.NalUnitType <= H265NALTypes.UNSPEC55)
                {
                    // NAL units with nal_unit_type in the range of UNSPEC48..UNSPEC55 with nuh_layer_id equal to 0 (when present)
                    if (_nalBufferContainsVCL && nu.NalUnitHeader.NuhLayerId == 0)
                    {
                        output = CreateSample(_nalBuffer);
                        _nalBuffer.Clear();
                        _nalBufferContainsVCL = false;
                        _nalBufferContainsIDR = false;
                    }
                }
                else
                {
                    if (nu.NalUnitHeader.NalUnitType >= H265NALTypes.TRAIL_N && nu.NalUnitHeader.NalUnitType <= H265NALTypes.RSV_VCL31)
                    {
                        if (nu.NalUnitHeader.NalUnitType >= H265NALTypes.BLA_W_LP && nu.NalUnitHeader.NalUnitType <= H265NALTypes.CRA_NUT)
                        {
                            // keyframe
                            _nalBufferContainsIDR = true;
                        }

                        // first VCL NAL unit of the coded picture shall have first_slice_segment_in_pic_flag equal to 1
                        if ((sample[2] & 0x80) != 0) // https://stackoverflow.com/questions/69373668/ffmpeg-error-first-slice-in-a-frame-missing-when-decoding-h-265-stream
                        {
                            if (_nalBufferContainsVCL)
                            {
                                output = CreateSample(_nalBuffer);
                                _nalBuffer.Clear();
                                _nalBufferContainsVCL = false;
                                _nalBufferContainsIDR = false;
                            }
                        }

                        _nalBufferContainsVCL = true;
                    }

                    _nalBuffer.Add(sample);
                }
            }
        }

        private byte[] CreateSample(List<byte[]> buffer)
        {
            if (buffer.Count == 0)
                return null;

            IEnumerable<byte> result = new byte[0];

            foreach (var nal in _nalBuffer)
            {
                uint nalUnitLength = (uint)nal.Length;

                byte[] size;
                switch (NalLengthSize)
                {
                    case 1:
                        {
                            if (nalUnitLength > byte.MaxValue) throw new ArgumentOutOfRangeException(nameof(nalUnitLength));
                            size = new byte[]
                            {
                                (byte)(nalUnitLength & 0xff)
                            };
                        }
                        break;

                    case 2:
                        {
                            if (nalUnitLength > ushort.MaxValue) throw new ArgumentOutOfRangeException(nameof(nalUnitLength));
                            size = new byte[]
                            {
                                (byte)((nalUnitLength & 0xff00) >> 8),
                                (byte)(nalUnitLength & 0xff)
                            };
                        }
                        break;

                    case 3:
                        {
                            if (nalUnitLength > 16777215) throw new ArgumentOutOfRangeException(nameof(nalUnitLength));
                            size = new byte[]
                            {
                                (byte)((nalUnitLength & 0xff0000) >> 16),
                                (byte)((nalUnitLength & 0xff00) >> 8),
                                (byte)(nalUnitLength & 0xff)
                            };
                        }
                        break;

                    case 4:
                        {
                            if (nalUnitLength > uint.MaxValue) throw new ArgumentOutOfRangeException(nameof(nalUnitLength));
                            size = new byte[]
                            {
                                (byte)((nalUnitLength & 0xff000000) >> 24),
                                (byte)((nalUnitLength & 0xff0000) >> 16),
                                (byte)((nalUnitLength & 0xff00) >> 8),
                                (byte)(nalUnitLength & 0xff)
                            };
                        }
                        break;

                    default:
                        throw new NotSupportedException($"NAL unit length {NalLengthSize} not supported!");
                }

                result = result.Concat(size).Concat(nal);
            }

            return result.ToArray();
        }

        public override Box CreateSampleEntryBox()
        {
            var sps = Sps.First().Value; // we need at least SPS
            var dim = sps.CalculateDimensions();

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
            hevcConfigurationBox._HEVCConfig.GeneralConstraintIndicatorFlags = BuildGeneralProfileConstraintIndicatorFlags(sps);
            hevcConfigurationBox._HEVCConfig.BitDepthChromaMinus8 = (byte)sps.BitDepthChromaMinus8;
            hevcConfigurationBox._HEVCConfig.BitDepthLumaMinus8 = (byte)sps.BitDepthLumaMinus8;
            hevcConfigurationBox._HEVCConfig.TemporalIdNested = sps.SpsTemporalIdNestingFlag != 0;
            hevcConfigurationBox._HEVCConfig.LengthSizeMinusOne = 3; // 4 bytes size block inserted in between NAL units

            int nalArrayCount = 3 + (PrefixSei.Count > 0 ? 1 : 0);

            hevcConfigurationBox._HEVCConfig.Reserved4 = new bool[nalArrayCount];
            hevcConfigurationBox._HEVCConfig.NumOfArrays = (byte)nalArrayCount;
            hevcConfigurationBox._HEVCConfig.ArrayCompleteness = new bool[nalArrayCount];
            for (int i = 0; i < nalArrayCount; i++) 
            { 
                hevcConfigurationBox._HEVCConfig.ArrayCompleteness[i] = true; 
            }
            hevcConfigurationBox._HEVCConfig.NumNalus = new ushort[nalArrayCount];
            hevcConfigurationBox._HEVCConfig.NALUnitType = new byte[nalArrayCount];

            hevcConfigurationBox._HEVCConfig.NumNalus[0] = (ushort)VpsRaw.Count;
            hevcConfigurationBox._HEVCConfig.NumNalus[1] = (ushort)SpsRaw.Count;
            hevcConfigurationBox._HEVCConfig.NumNalus[2] = (ushort)PpsRaw.Count;            
            hevcConfigurationBox._HEVCConfig.NALUnitType[0] = (byte)H265NALTypes.VPS_NUT;
            hevcConfigurationBox._HEVCConfig.NALUnitType[1] = (byte)H265NALTypes.SPS_NUT;
            hevcConfigurationBox._HEVCConfig.NALUnitType[2] = (byte)H265NALTypes.PPS_NUT;        
            if(PrefixSei.Count > 0)
            {
                hevcConfigurationBox._HEVCConfig.NumNalus[3] = (ushort)PrefixSei.Count;            
                hevcConfigurationBox._HEVCConfig.NALUnitType[3] = (byte)H265NALTypes.PREFIX_SEI_NUT;        
            }

            // correct order is VPS, SPS, PPS. Other order produced ffmpeg errors such as "VPS 0 does not exist" and "SPS 0 does not exist."
            hevcConfigurationBox._HEVCConfig.NalUnit = new byte[nalArrayCount][][];
            hevcConfigurationBox._HEVCConfig.NalUnit[0] = new byte[VpsRaw.Count][];
            hevcConfigurationBox._HEVCConfig.NalUnit[1] = new byte[SpsRaw.Count][];
            hevcConfigurationBox._HEVCConfig.NalUnit[2] = new byte[PpsRaw.Count][];
            hevcConfigurationBox._HEVCConfig.NalUnitLength = new ushort[nalArrayCount][];
            hevcConfigurationBox._HEVCConfig.NalUnitLength[0] = new ushort[VpsRaw.Count];
            hevcConfigurationBox._HEVCConfig.NalUnitLength[1] = new ushort[SpsRaw.Count];
            hevcConfigurationBox._HEVCConfig.NalUnitLength[2] = new ushort[PpsRaw.Count];
            
            if(PrefixSei.Count > 0)
            {
                hevcConfigurationBox._HEVCConfig.NalUnit[3] = new byte[PrefixSei.Count][];
                hevcConfigurationBox._HEVCConfig.NalUnitLength[3] = new ushort[PrefixSei.Count];
            }

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

            if (PrefixSei.Count > 0)
            {
                // optional NALU 39
                for (int i = 0; i < PrefixSei.Count; i++)
                {
                    hevcConfigurationBox._HEVCConfig.NalUnit[3][i] = PrefixSeiRaw.ElementAt(i);
                    hevcConfigurationBox._HEVCConfig.NalUnitLength[3][i] = (ushort)PrefixSeiRaw.ElementAt(i).Length;
                }
            }

            visualSampleEntry.Children.Add(hevcConfigurationBox);

            return visualSampleEntry;
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            var dim = Sps.FirstOrDefault().Value.CalculateDimensions();
            tkhd.Width = dim.Width << 16; // TODO: simplify API
            tkhd.Height = dim.Height << 16; // TODO: simplify API
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

        public ulong BuildGeneralProfileConstraintIndicatorFlags(SeqParameterSetRbsp sps)
        {
            using (var ms = new MemoryStream())
            {
                IsoStream isoStream = new IsoStream(ms);
                isoStream.WriteBit(sps.ProfileTierLevel.GeneralProgressiveSourceFlag);
                isoStream.WriteBit(sps.ProfileTierLevel.GeneralInterlacedSourceFlag);
                isoStream.WriteBit(sps.ProfileTierLevel.GeneralNonPackedConstraintFlag);
                isoStream.WriteBit(sps.ProfileTierLevel.GeneralFrameOnlyConstraintFlag);

                if (sps.ProfileTierLevel.GeneralProfileIdc == 4 || sps.ProfileTierLevel.GeneralProfileCompatibilityFlag[4] != 0 ||
                    sps.ProfileTierLevel.GeneralProfileIdc == 5 || sps.ProfileTierLevel.GeneralProfileCompatibilityFlag[5] != 0 ||
                    sps.ProfileTierLevel.GeneralProfileIdc == 6 || sps.ProfileTierLevel.GeneralProfileCompatibilityFlag[6] != 0 ||
                    sps.ProfileTierLevel.GeneralProfileIdc == 7 || sps.ProfileTierLevel.GeneralProfileCompatibilityFlag[7] != 0 ||
                    sps.ProfileTierLevel.GeneralProfileIdc == 8 || sps.ProfileTierLevel.GeneralProfileCompatibilityFlag[8] != 0 ||
                    sps.ProfileTierLevel.GeneralProfileIdc == 9 || sps.ProfileTierLevel.GeneralProfileCompatibilityFlag[9] != 0 ||
                    sps.ProfileTierLevel.GeneralProfileIdc == 10 || sps.ProfileTierLevel.GeneralProfileCompatibilityFlag[10] != 0)
                {
                    isoStream.WriteBit(sps.ProfileTierLevel.GeneralMax12bitConstraintFlag);
                    isoStream.WriteBit(sps.ProfileTierLevel.GeneralMax10bitConstraintFlag);
                    isoStream.WriteBit(sps.ProfileTierLevel.GeneralMax8bitConstraintFlag);
                    isoStream.WriteBit(sps.ProfileTierLevel.GeneralMax422chromaConstraintFlag);
                    isoStream.WriteBit(sps.ProfileTierLevel.GeneralMax420chromaConstraintFlag);
                    isoStream.WriteBit(sps.ProfileTierLevel.GeneralMaxMonochromeConstraintFlag);
                    isoStream.WriteBit(sps.ProfileTierLevel.GeneralIntraConstraintFlag);
                    isoStream.WriteBit(sps.ProfileTierLevel.GeneralOnePictureOnlyConstraintFlag);
                    isoStream.WriteBit(sps.ProfileTierLevel.GeneralLowerBitRateConstraintFlag);

                    if (sps.ProfileTierLevel.GeneralProfileIdc == 5 || sps.ProfileTierLevel.GeneralProfileCompatibilityFlag[5] != 0 ||
                        sps.ProfileTierLevel.GeneralProfileIdc == 9 || sps.ProfileTierLevel.GeneralProfileCompatibilityFlag[9] != 0 ||
                        sps.ProfileTierLevel.GeneralProfileIdc == 10 || sps.ProfileTierLevel.GeneralProfileCompatibilityFlag[10] != 0)
                    {
                        isoStream.WriteBit(sps.ProfileTierLevel.GeneralMax14bitConstraintFlag);
                        isoStream.WriteBits(33, sps.ProfileTierLevel.GeneralReservedZero33bits);
                    }
                    else
                    {
                        isoStream.WriteBits(34, sps.ProfileTierLevel.GeneralReservedZero34bits);
                    }
                }
                else if (sps.ProfileTierLevel.GeneralProfileIdc == 2 || sps.ProfileTierLevel.GeneralProfileCompatibilityFlag[2] != 0)
                {
                    isoStream.WriteBits(7, sps.ProfileTierLevel.GeneralReservedZero7bits);
                    isoStream.WriteBit(sps.ProfileTierLevel.GeneralOnePictureOnlyConstraintFlag);
                    isoStream.WriteBits(35, sps.ProfileTierLevel.GeneralReservedZero35bits);
                }
                else
                {
                    isoStream.WriteBits(43, sps.ProfileTierLevel.GeneralReservedZero43bits);
                }

                if (sps.ProfileTierLevel.GeneralProfileIdc >= 1 && sps.ProfileTierLevel.GeneralProfileIdc <= 5 ||
                    sps.ProfileTierLevel.GeneralProfileIdc == 9 ||
                    sps.ProfileTierLevel.GeneralProfileCompatibilityFlag[1] != 0 || sps.ProfileTierLevel.GeneralProfileCompatibilityFlag[2] != 0 ||
                    sps.ProfileTierLevel.GeneralProfileCompatibilityFlag[3] != 0 || sps.ProfileTierLevel.GeneralProfileCompatibilityFlag[4] != 0 ||
                    sps.ProfileTierLevel.GeneralProfileCompatibilityFlag[5] != 0 || sps.ProfileTierLevel.GeneralProfileCompatibilityFlag[9] != 0)
                {
                    isoStream.WriteBit(sps.ProfileTierLevel.GeneralInbldFlag);
                }
                else
                {
                    isoStream.WriteBit(sps.ProfileTierLevel.GeneralReservedZeroBit);
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

        public override IEnumerable<byte[]> GetContainerSamples()
        {
            return VpsRaw.Values.ToArray().Concat(SpsRaw.Values.ToArray()).Concat(PpsRaw.Values.ToArray()).Concat(PrefixSeiRaw).ToArray();
        }

        public override ITrack Clone()
        {
            return new H265Track(Timescale, DefaultSampleDuration); 
        }
    }
}
