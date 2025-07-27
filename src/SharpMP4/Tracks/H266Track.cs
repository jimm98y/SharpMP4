using SharpISOBMFF;
using SharpH26X;
using SharpH266;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// H266 track.
    /// </summary>
    /// <remarks>https://www.itu.int/rec/T-REC-H.266/en</remarks>
    public class H266Track : H26XTrackBase
    {
        public const string BRAND = "vvc1";

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

        private H266Context _context = new H266Context();

        /// <summary>
        /// Ctor.
        /// </summary>
        public H266Track() : base()
        {
            CompatibleBrand = BRAND; // vvc1
        }

        public H266Track(uint timescale, int sampleDuration) : this()
        {
            Timescale = timescale;
            DefaultSampleDuration = sampleDuration;
        }

        public H266Track(Box sampleEntry, uint timescale, int sampleDuration) : this(timescale, sampleDuration)
        {
            VisualSampleEntry visualSampleEntry = (VisualSampleEntry)sampleEntry;
            VvcConfigurationBox vvcC = visualSampleEntry.Children.OfType<VvcConfigurationBox>().Single();

            NalLengthSize = vvcC._VvcConfig._LengthSizeMinusOne + 1; // usually 4 bytes

            foreach (var nalus in vvcC._VvcConfig.NalUnit)
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

            using (ItuStream stream = new ItuStream(new MemoryStream(sample)))
            {
                ulong ituSize = 0;
                var nu = new NalUnit((uint)sample.Length);
                _context.NalHeader = nu;
                ituSize += nu.Read(_context, stream);

                if (nu.NalUnitHeader.NalUnitType == H266NALTypes.AUD_NUT)
                {
                    if (_nalBufferContainsVCL)
                    {
                        output = CreateSample(_nalBuffer);
                        _nalBuffer.Clear();
                        _nalBufferContainsVCL = false;
                        _nalBufferContainsIDR = false;
                    }

                    _nalBuffer.Add(sample);
                }
                else if (nu.NalUnitHeader.NalUnitType == H266NALTypes.OPI_NUT)
                {
                    _context.OperatingPointInformationRbsp = new OperatingPointInformationRbsp();
                    _context.OperatingPointInformationRbsp.Read(_context, stream);
                    if (_nalBufferContainsVCL)
                    {
                        output = CreateSample(_nalBuffer);
                        _nalBuffer.Clear();
                        _nalBufferContainsVCL = false;
                        _nalBufferContainsIDR = false;
                    }

                    _nalBuffer.Add(sample);
                }
                if(nu.NalUnitHeader.NalUnitType == H266NALTypes.DCI_NUT)
                {
                    if (_nalBufferContainsVCL)
                    {
                        output = CreateSample(_nalBuffer);
                        _nalBuffer.Clear();
                        _nalBufferContainsVCL = false;
                        _nalBufferContainsIDR = false;
                    }

                    _nalBuffer.Add(sample);
                }
                else if (nu.NalUnitHeader.NalUnitType == H266NALTypes.VPS_NUT)
                {
                    _context.VideoParameterSetRbsp = new VideoParameterSetRbsp();
                    _context.VideoParameterSetRbsp.Read(_context, stream);
                    if (!Vps.ContainsKey(_context.VideoParameterSetRbsp.VpsVideoParameterSetId))
                    {
                        Vps.Add(_context.VideoParameterSetRbsp.VpsVideoParameterSetId, _context.VideoParameterSetRbsp);
                        VpsRaw.Add(_context.VideoParameterSetRbsp.VpsVideoParameterSetId, sample);
                    }

                    if (_nalBufferContainsVCL)
                    {
                        output = CreateSample(_nalBuffer);
                        _nalBuffer.Clear();
                        _nalBufferContainsVCL = false;
                        _nalBufferContainsIDR = false;
                    }
                }
                else if (nu.NalUnitHeader.NalUnitType == H266NALTypes.SPS_NUT)
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

                    if (_nalBufferContainsVCL)
                    {
                        output = CreateSample(_nalBuffer);
                        _nalBuffer.Clear();
                        _nalBufferContainsVCL = false;
                        _nalBufferContainsIDR = false;
                    }
                }
                else if (nu.NalUnitHeader.NalUnitType == H266NALTypes.PPS_NUT)
                {
                    _context.PicParameterSetRbsp = new PicParameterSetRbsp();
                    _context.PicParameterSetRbsp.Read(_context, stream);
                    if (!Pps.ContainsKey(_context.PicParameterSetRbsp.PpsPicParameterSetId))
                    {
                        Pps.Add(_context.PicParameterSetRbsp.PpsPicParameterSetId, _context.PicParameterSetRbsp);
                        PpsRaw.Add(_context.PicParameterSetRbsp.PpsPicParameterSetId, sample);
                    }

                    if (_nalBufferContainsVCL)
                    {
                        output = CreateSample(_nalBuffer);
                        _nalBuffer.Clear();
                        _nalBufferContainsVCL = false;
                        _nalBufferContainsIDR = false;
                    }
                }
                else if (nu.NalUnitHeader.NalUnitType == H266NALTypes.PREFIX_APS_NUT || nu.NalUnitHeader.NalUnitType == H266NALTypes.PH_NUT)
                {
                    if (_nalBufferContainsVCL)
                    {
                        output = CreateSample(_nalBuffer);
                        _nalBuffer.Clear();
                        _nalBufferContainsVCL = false;
                        _nalBufferContainsIDR = false;
                    }

                    _nalBuffer.Add(sample);
                }
                else if (nu.NalUnitHeader.NalUnitType == H266NALTypes.PREFIX_SEI_NUT)
                {
                    _context.SeiRbsp = new SeiRbsp();
                    _context.SeiRbsp.Read(_context, stream);
                    if (!PrefixSei.Contains(_context.SeiRbsp))
                    {
                        PrefixSei.Add(_context.SeiRbsp);
                        PrefixSeiRaw.Add(sample);
                    }

                    if (_nalBufferContainsVCL)
                    {
                        output = CreateSample(_nalBuffer);
                        _nalBuffer.Clear();
                        _nalBufferContainsVCL = false;
                        _nalBufferContainsIDR = false;
                    }

                    _nalBuffer.Add(sample);
                }
                else if (nu.NalUnitHeader.NalUnitType == H266NALTypes.RSV_NVCL_26 || nu.NalUnitHeader.NalUnitType == H266NALTypes.UNSPEC_28 || nu.NalUnitHeader.NalUnitType == H266NALTypes.UNSPEC_29)
                {
                    if (_nalBufferContainsVCL)
                    {
                        output = CreateSample(_nalBuffer);
                        _nalBuffer.Clear();
                        _nalBufferContainsVCL = false;
                        _nalBufferContainsIDR = false;
                    }

                    _nalBuffer.Add(sample);
                }
                else if (nu.NalUnitHeader.NalUnitType == H266NALTypes.EOS_NUT)
                {
                    if (_nalBufferContainsVCL)
                    {
                        output = CreateSample(_nalBuffer);
                        _nalBuffer.Clear();
                        _nalBufferContainsVCL = false;
                        _nalBufferContainsIDR = false;
                    }
                }
                else
                {
                    if (nu.NalUnitHeader.NalUnitType >= H266NALTypes.TRAIL_NUT && nu.NalUnitHeader.NalUnitType <= H266NALTypes.RSV_IRAP_11)
                    {
                        if (nu.NalUnitHeader.NalUnitType >= H266NALTypes.IDR_W_RADL && nu.NalUnitHeader.NalUnitType <= H266NALTypes.GDR_NUT)
                        {
                            // keyframe
                            _nalBufferContainsIDR = true;
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

            VvcConfigurationBox vvcConfigurationBox = new VvcConfigurationBox();
            vvcConfigurationBox.SetParent(visualSampleEntry);
            vvcConfigurationBox._VvcConfig = new VvcDecoderConfigurationRecord();
            vvcConfigurationBox._VvcConfig.BitDepthMinus8 = (byte)sps.SpsBitdepthMinus8;
            vvcConfigurationBox._VvcConfig.ChromaFormatIdc = (byte)sps.SpsChromaFormatIdc;
            vvcConfigurationBox._VvcConfig.AvgFrameRate = 0;
            vvcConfigurationBox._VvcConfig.ConstantFrameRate = 1;
            vvcConfigurationBox._VvcConfig.MaxPictureWidth = (ushort)dim.Width; // TODO?
            vvcConfigurationBox._VvcConfig.MaxPictureHeight = (ushort)dim.Height; // TODO?

            vvcConfigurationBox._VvcConfig.NumSublayers = (byte)(sps.SpsMaxSublayersMinus1 + 1);
            vvcConfigurationBox._VvcConfig.PtlPresentFlag = sps.SpsPtlDpbHrdParamsPresentFlag != 0;
            if (vvcConfigurationBox._VvcConfig.PtlPresentFlag)
            {
                byte[] generalConstraintsInfo = BuildGeneralConstraintsInfo(sps.ProfileTierLevel.GeneralConstraintsInfo);
                vvcConfigurationBox._VvcConfig.NativePtl = new VvcPTLRecord((byte)sps.SpsMaxSublayersMinus1)
                {
                    GeneralConstraintInfo = generalConstraintsInfo,
                    NumBytesConstraintInfo = (byte)generalConstraintsInfo.Length,
                    GeneralLevelIdc = (byte)sps.ProfileTierLevel.GeneralLevelIdc,
                    GeneralProfileIdc = (byte)sps.ProfileTierLevel.GeneralProfileIdc,
                    GeneralSubProfileIdc = sps.ProfileTierLevel.GeneralSubProfileIdc,
                    GeneralTierFlag = sps.ProfileTierLevel.GeneralTierFlag != 0,
                    PtlFrameOnlyConstraintFlag = sps.ProfileTierLevel.PtlFrameOnlyConstraintFlag != 0,
                    PtlMultiLayerEnabledFlag = sps.ProfileTierLevel.PtlMultilayerEnabledFlag != 0,
                    PtlNumSubProfiles = (byte)sps.ProfileTierLevel.PtlNumSubProfiles,
                    PtlReservedZeroBit = new bool[9],
                    PtlSublayerLevelPresentFlag = sps.ProfileTierLevel.PtlSublayerLevelPresentFlag.Select(x => x != 0).ToArray(),
                    SublayerLevelIdc = sps.ProfileTierLevel.SublayerLevelIdc.Select(x => (byte)x).ToArray(),
                };
            }
            vvcConfigurationBox._VvcConfig.OlsIdx = (ushort)(_context.OperatingPointInformationRbsp == null ? 0 : _context.OperatingPointInformationRbsp.OpiOlsIdx);

            vvcConfigurationBox._VvcConfig._LengthSizeMinusOne = 3; // 4 bytes size block inserted in between NAL units

            int nalArrayCount = 3 + (PrefixSei.Count > 0 ? 1 : 0);

            vvcConfigurationBox._VvcConfig.Reserved1 = new byte[nalArrayCount];
            vvcConfigurationBox._VvcConfig.NumOfArrays = (byte)nalArrayCount;
            vvcConfigurationBox._VvcConfig.ArrayCompleteness = new bool[nalArrayCount];
            for (int i = 0; i < nalArrayCount; i++)
            {
                vvcConfigurationBox._VvcConfig.ArrayCompleteness[i] = true;
            }
            vvcConfigurationBox._VvcConfig.NumNalus = new ushort[nalArrayCount];
            vvcConfigurationBox._VvcConfig.NALUnitType = new byte[nalArrayCount];

            vvcConfigurationBox._VvcConfig.NumNalus[0] = (ushort)VpsRaw.Count;
            vvcConfigurationBox._VvcConfig.NumNalus[1] = (ushort)SpsRaw.Count;
            vvcConfigurationBox._VvcConfig.NumNalus[2] = (ushort)PpsRaw.Count;
            vvcConfigurationBox._VvcConfig.NALUnitType[0] = (byte)H266NALTypes.VPS_NUT;
            vvcConfigurationBox._VvcConfig.NALUnitType[1] = (byte)H266NALTypes.SPS_NUT;
            vvcConfigurationBox._VvcConfig.NALUnitType[2] = (byte)H266NALTypes.PPS_NUT;
            if (PrefixSei.Count > 0)
            {
                vvcConfigurationBox._VvcConfig.NumNalus[3] = (ushort)PrefixSei.Count;
                vvcConfigurationBox._VvcConfig.NALUnitType[3] = (byte)H266NALTypes.PREFIX_SEI_NUT;
            }

            // correct order is VPS, SPS, PPS. Other order produced ffmpeg errors such as "VPS 0 does not exist" and "SPS 0 does not exist."
            vvcConfigurationBox._VvcConfig.NalUnit = new byte[nalArrayCount][][];
            vvcConfigurationBox._VvcConfig.NalUnit[0] = new byte[VpsRaw.Count][];
            vvcConfigurationBox._VvcConfig.NalUnit[1] = new byte[SpsRaw.Count][];
            vvcConfigurationBox._VvcConfig.NalUnit[2] = new byte[PpsRaw.Count][];
            vvcConfigurationBox._VvcConfig.NalUnitLength = new ushort[nalArrayCount][];
            vvcConfigurationBox._VvcConfig.NalUnitLength[0] = new ushort[VpsRaw.Count];
            vvcConfigurationBox._VvcConfig.NalUnitLength[1] = new ushort[SpsRaw.Count];
            vvcConfigurationBox._VvcConfig.NalUnitLength[2] = new ushort[PpsRaw.Count];

            if (PrefixSei.Count > 0)
            {
                vvcConfigurationBox._VvcConfig.NalUnit[3] = new byte[PrefixSei.Count][];
                vvcConfigurationBox._VvcConfig.NalUnitLength[3] = new ushort[PrefixSei.Count];
            }

            for (int i = 0; i < VpsRaw.Count; i++)
            {
                vvcConfigurationBox._VvcConfig.NalUnit[0][i] = VpsRaw.Values.ElementAt(i);
                vvcConfigurationBox._VvcConfig.NalUnitLength[0][i] = (ushort)VpsRaw.Values.ElementAt(i).Length;
            }

            for (int i = 0; i < SpsRaw.Count; i++)
            {
                vvcConfigurationBox._VvcConfig.NalUnit[1][i] = SpsRaw.Values.ElementAt(i);
                vvcConfigurationBox._VvcConfig.NalUnitLength[1][i] = (ushort)SpsRaw.Values.ElementAt(i).Length;
            }

            for (int i = 0; i < PpsRaw.Count; i++)
            {
                vvcConfigurationBox._VvcConfig.NalUnit[2][i] = PpsRaw.Values.ElementAt(i);
                vvcConfigurationBox._VvcConfig.NalUnitLength[2][i] = (ushort)PpsRaw.Values.ElementAt(i).Length;
            }

            if (PrefixSei.Count > 0)
            {
                // optional
                for (int i = 0; i < PrefixSei.Count; i++)
                {
                    vvcConfigurationBox._VvcConfig.NalUnit[3][i] = PrefixSeiRaw.ElementAt(i);
                    vvcConfigurationBox._VvcConfig.NalUnitLength[3][i] = (ushort)PrefixSeiRaw.ElementAt(i).Length;
                }
            }

            visualSampleEntry.Children.Add(vvcConfigurationBox);

            return visualSampleEntry;
        }

        private byte[] BuildGeneralConstraintsInfo(GeneralConstraintsInfo gci)
        {
            using (var ms = new MemoryStream())
            {
                IsoStream isoStream = new IsoStream(ms);

                isoStream.WriteBit(gci.GciPresentFlag);

                if (gci.GciPresentFlag != 0)
                {
                    //  general
                    isoStream.WriteBit(gci.GciIntraOnlyConstraintFlag);
                    isoStream.WriteBit(gci.GciAllLayersIndependentConstraintFlag);
                    isoStream.WriteBit(gci.GciOneAuOnlyConstraintFlag);

                    //  picture format
                    isoStream.WriteBits(4, gci.GciSixteenMinusMaxBitdepthConstraintIdc);
                    isoStream.WriteBits(2, gci.GciThreeMinusMaxChromaFormatConstraintIdc);

                    //  NAL unit type related
                    isoStream.WriteBit(gci.GciNoMixedNaluTypesInPicConstraintFlag);
                    isoStream.WriteBit(gci.GciNoTrailConstraintFlag);
                    isoStream.WriteBit(gci.GciNoStsaConstraintFlag);
                    isoStream.WriteBit(gci.GciNoRaslConstraintFlag);
                    isoStream.WriteBit(gci.GciNoRadlConstraintFlag);
                    isoStream.WriteBit(gci.GciNoIdrConstraintFlag);
                    isoStream.WriteBit(gci.GciNoCraConstraintFlag);
                    isoStream.WriteBit(gci.GciNoGdrConstraintFlag);
                    isoStream.WriteBit(gci.GciNoApsConstraintFlag);
                    isoStream.WriteBit(gci.GciNoIdrRplConstraintFlag);

                    //  tile, slice, subpicture partitioning
                    isoStream.WriteBit(gci.GciOneTilePerPicConstraintFlag);
                    isoStream.WriteBit(gci.GciPicHeaderInSliceHeaderConstraintFlag);
                    isoStream.WriteBit(gci.GciOneSlicePerPicConstraintFlag);
                    isoStream.WriteBit(gci.GciNoRectangularSliceConstraintFlag);
                    isoStream.WriteBit(gci.GciOneSlicePerSubpicConstraintFlag);
                    isoStream.WriteBit(gci.GciNoSubpicInfoConstraintFlag);

                    //  CTU and block partitioning
                    isoStream.WriteBits(2, gci.GciThreeMinusMaxLog2CtuSizeConstraintIdc);
                    isoStream.WriteBit(gci.GciNoPartitionConstraintsOverrideConstraintFlag);
                    isoStream.WriteBit(gci.GciNoMttConstraintFlag);
                    isoStream.WriteBit(gci.GciNoQtbttDualTreeIntraConstraintFlag);

                    //  intra
                    isoStream.WriteBit(gci.GciNoPaletteConstraintFlag);
                    isoStream.WriteBit(gci.GciNoIbcConstraintFlag);
                    isoStream.WriteBit(gci.GciNoIspConstraintFlag);
                    isoStream.WriteBit(gci.GciNoMrlConstraintFlag);
                    isoStream.WriteBit(gci.GciNoMipConstraintFlag);
                    isoStream.WriteBit(gci.GciNoCclmConstraintFlag);

                    //  inter
                    isoStream.WriteBit(gci.GciNoRefPicResamplingConstraintFlag);
                    isoStream.WriteBit(gci.GciNoResChangeInClvsConstraintFlag);
                    isoStream.WriteBit(gci.GciNoWeightedPredictionConstraintFlag);
                    isoStream.WriteBit(gci.GciNoRefWraparoundConstraintFlag);
                    isoStream.WriteBit(gci.GciNoTemporalMvpConstraintFlag);
                    isoStream.WriteBit(gci.GciNoSbtmvpConstraintFlag);
                    isoStream.WriteBit(gci.GciNoAmvrConstraintFlag);
                    isoStream.WriteBit(gci.GciNoBdofConstraintFlag);
                    isoStream.WriteBit(gci.GciNoSmvdConstraintFlag);
                    isoStream.WriteBit(gci.GciNoDmvrConstraintFlag);
                    isoStream.WriteBit(gci.GciNoMmvdConstraintFlag);
                    isoStream.WriteBit(gci.GciNoAffineMotionConstraintFlag);
                    isoStream.WriteBit(gci.GciNoProfConstraintFlag);
                    isoStream.WriteBit(gci.GciNoBcwConstraintFlag);
                    isoStream.WriteBit(gci.GciNoCiipConstraintFlag);
                    isoStream.WriteBit(gci.GciNoGpmConstraintFlag);

                    //  transform, quantization, residual
                    isoStream.WriteBit(gci.GciNoLumaTransformSize64ConstraintFlag);
                    isoStream.WriteBit(gci.GciNoTransformSkipConstraintFlag);
                    isoStream.WriteBit(gci.GciNoBdpcmConstraintFlag);
                    isoStream.WriteBit(gci.GciNoMtsConstraintFlag);
                    isoStream.WriteBit(gci.GciNoLfnstConstraintFlag);
                    isoStream.WriteBit(gci.GciNoJointCbcrConstraintFlag);
                    isoStream.WriteBit(gci.GciNoSbtConstraintFlag);
                    isoStream.WriteBit(gci.GciNoActConstraintFlag);
                    isoStream.WriteBit(gci.GciNoExplicitScalingListConstraintFlag);
                    isoStream.WriteBit(gci.GciNoDepQuantConstraintFlag);
                    isoStream.WriteBit(gci.GciNoSignDataHidingConstraintFlag);
                    isoStream.WriteBit(gci.GciNoCuQpDeltaConstraintFlag);
                    isoStream.WriteBit(gci.GciNoChromaQpOffsetConstraintFlag);

                    //  loop filter
                    isoStream.WriteBit(gci.GciNoSaoConstraintFlag);
                    isoStream.WriteBit(gci.GciNoAlfConstraintFlag);
                    isoStream.WriteBit(gci.GciNoCcalfConstraintFlag);
                    isoStream.WriteBit(gci.GciNoLmcsConstraintFlag);
                    isoStream.WriteBit(gci.GciNoLadfConstraintFlag);
                    isoStream.WriteBit(gci.GciNoVirtualBoundariesConstraintFlag);
                    isoStream.WriteBits(8, gci.GciNumReservedBits);

                    for (int i = 0; i < gci.GciNumReservedBits; i++)
                    {
                        isoStream.WriteBit(gci.GciReservedZeroBit[i]);
                    }
                }

                isoStream.WriteByteAlignment(0);

                return ms.ToArray();
            }
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            var dim = Sps.FirstOrDefault().Value.CalculateDimensions();
            tkhd.Width = dim.Width << 16; // TODO: simplify API
            tkhd.Height = dim.Height << 16; // TODO: simplify API
        }

        public override IEnumerable<byte[]> GetContainerSamples()
        {
            return VpsRaw.Values.ToArray().Concat(SpsRaw.Values.ToArray()).Concat(PpsRaw.Values.ToArray()).Concat(PrefixSeiRaw).ToArray();
        }

        public override ITrack Clone()
        {
            return new H266Track(Timescale, DefaultSampleDuration);
        }
    }
}
