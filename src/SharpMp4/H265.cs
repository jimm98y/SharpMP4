using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SharpMp4
{
    /// <summary>
    /// H265 track.
    /// </summary>
    /// <remarks>https://www.itu.int/rec/T-REC-H.265/en</remarks>
    public class H265Track : TrackBase
    {
        private List<byte[]> _nalBuffer = new List<byte[]>();
        private bool _nalBufferContainsVCL = false;

        /// <summary>
        /// VPS (Video Parameter Set) NAL units.
        /// </summary>
        public Dictionary<int, H265VpsNalUnit> Vps { get; set; } = new Dictionary<int, H265VpsNalUnit>();

        /// <summary>
        /// SPS (Sequence Parameter Set) NAL units.
        /// </summary>
        public Dictionary<int, H265SpsNalUnit> Sps { get; set; } = new Dictionary<int, H265SpsNalUnit>();

        /// <summary>
        /// PPS (Picture Parameter Set) NAL units.
        /// </summary>
        public Dictionary<int, H265PpsNalUnit> Pps { get; set; } = new Dictionary<int, H265PpsNalUnit>();

        public override string HdlrName => HdlrNames.Video;
        public override string HdlrType => HdlrTypes.Video;

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

        /// <summary>
        /// Ctor.
        /// </summary>
        public H265Track()
        {
            this.CompatibleBrand = VisualSampleEntryBox.TYPE6; // hvc1
            this.DefaultSampleFlags = new SampleFlags() { SampleDependsOn = 1, SampleIsDifferenceSample = true };
        }
        
        /// <summary>
        /// Process 1 NAL (Network Abstraction Layer) unit.
        /// </summary>
        /// <param name="sample">NAL bytes.</param>
        /// <returns><see cref="Task"/></returns>
        public override async Task ProcessSampleAsync(byte[] sample)
        {
            using (NalBitStreamReader bitstream = new NalBitStreamReader(sample))
            {
                // https://www.bing.com/search?pglt=43&q=hevc+hvc1&cvid=a9d4d6d6a96a4cddb12f3d94ac802bdd&gs_lcrp=EgZjaHJvbWUyBggAEEUYOTIGCAEQABhAMgYIAhAAGEAyBggDEAAYQDIGCAQQABhAMgYIBRAAGEAyBggGEEUYPNIBCDMyMjhqMGoxqAIAsAIA&FORM=ANNTA1&PC=U531
                // for hvc1, SPS, PPS, VPS should not be in MDAT
                // for hev1, SPS, PPS, VPS may be in MDAT
                var header = H265NalUnitHeader.ParseNALHeader(bitstream);
                if (header.NalUnitType == H265NalUnitTypes.SPS)
                {
                    if (Log.DebugEnabled) Log.Debug($"-Parsed SPS: {Utilities.ToHexString(sample)}");
                    var sps = H265SpsNalUnit.Parse(sample);
                    if (!Sps.ContainsKey(sps.SeqParameterSetId))
                    {
                        Sps.Add(sps.SeqParameterSetId, sps);
                    }
                    if (Log.DebugEnabled) Log.Debug($"Rebuilt SPS: {Utilities.ToHexString(H265SpsNalUnit.Build(sps))}");

                    // if SPS contains the timescale, set it
                    if (Timescale == 0 || SampleDuration == 0)
                    {
                        var timescale = sps.CalculateTimescale();
                        if (timescale.Timescale != 0 && timescale.FrameTick != 0)
                        {
                            Timescale = (uint)timescale.Timescale; 
                            SampleDuration = (uint)timescale.FrameTick;
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
                else if (header.NalUnitType == H265NalUnitTypes.PPS)
                {
                    if (Log.DebugEnabled) Log.Debug($"-Parsed PPS: {Utilities.ToHexString(sample)}");
                    var pps = H265PpsNalUnit.Parse(sample);
                    if (!Pps.ContainsKey(pps.PicParameterSetId))
                    {
                        Pps.Add(pps.PicParameterSetId, pps);
                    }
                    if (Log.DebugEnabled) Log.Debug($"Rebuilt PPS: {Utilities.ToHexString(H265PpsNalUnit.Build(pps))}");
                }
                else if (header.NalUnitType == H265NalUnitTypes.VPS)
                {
                    if (Log.DebugEnabled) Log.Debug($"-Parsed VPS: {Utilities.ToHexString(sample)}");
                    var vps = H265VpsNalUnit.Parse(sample);
                    if (!Vps.ContainsKey(vps.VpsParameterSetId))
                    {
                        Vps.Add(vps.VpsParameterSetId, vps);
                    }
                    if (Log.DebugEnabled) Log.Debug($"Rebuilt VPS: {Utilities.ToHexString(H265VpsNalUnit.Build(vps))}");
                }
                else if (header.NalUnitType == H265NalUnitTypes.PREFIX_SEI_NUT || header.NalUnitType == H265NalUnitTypes.SUFFIX_SEI_NUT)
                {
                    if (_nalBufferContainsVCL)
                    {
                        await CreateSample();
                    }

                    _nalBuffer.Add(sample);
                }
                else
                {
                    if (Log.DebugEnabled) Log.Debug($"NAL: {header.NalUnitType}, {sample.Length}");

                    if (header.IsVCL())
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

        /// <summary>
        /// Flush all remaining NAL units from the buffer.
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        public override async Task FlushAsync()
        {
            if (_nalBuffer.Count == 0 || !_nalBufferContainsVCL)
                return;

            if ((_nalBuffer[0][2] & 0x80) != 0) 
            {
                await CreateSample();
            }
            else
            {
                if (Log.WarnEnabled) Log.Warn($"Invalid NAL in the buffer");
            }
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

        public override Mp4Box CreateSampleEntryBox(Mp4Box parent)
        {
            return CreateVisualSampleEntryBox(parent, this);
        }

        private static VisualSampleEntryBox CreateVisualSampleEntryBox(Mp4Box parent, H265Track track)
        {
            var sps = track.Sps.First().Value;
            var vps = track.Vps.First().Value;
            var dim = sps.CalculateDimensions();

            VisualSampleEntryBox visualSampleEntry = new VisualSampleEntryBox(0, 0, parent, VisualSampleEntryBox.TYPE6);
            visualSampleEntry.DataReferenceIndex = 1;
            visualSampleEntry.Depth = 24;
            visualSampleEntry.FrameCount = 1;
            visualSampleEntry.HorizontalResolution = 72;
            visualSampleEntry.VerticalResolution = 72;
            visualSampleEntry.Width = dim.Width;
            visualSampleEntry.Height = dim.Height;
            visualSampleEntry.CompressorName = "hevc\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0";
            visualSampleEntry.CompressorNameDisplayableData = 4;

            HevcConfigurationBox hevcConfigurationBox = new HevcConfigurationBox(0, 0, visualSampleEntry, new HevcDecoderConfigurationRecord());
            hevcConfigurationBox.HevcDecoderConfigurationRecord.ConfigurationVersion = 1;
            hevcConfigurationBox.HevcDecoderConfigurationRecord.GeneralProfileIdc = sps.ProfileTier.GeneralProfileIdc;
            hevcConfigurationBox.HevcDecoderConfigurationRecord.ChromaFormat = sps.ChromaFormatIdc;
            hevcConfigurationBox.HevcDecoderConfigurationRecord.GeneralLevelIdc = (byte)sps.ProfileTier.GeneralLevelIdc;
            hevcConfigurationBox.HevcDecoderConfigurationRecord.GeneralProfileCompatibilityFlags = sps.ProfileTier.GetGeneralProfileCompatibilityFlags();
            hevcConfigurationBox.HevcDecoderConfigurationRecord.GeneralConstraintIndicatorFlags = vps.ProfileTier.GetGeneralProfileConstraintIndicatorFlags();
            hevcConfigurationBox.HevcDecoderConfigurationRecord.BitDepthChromaMinus8 = sps.BitDepthChromaMinus8;
            hevcConfigurationBox.HevcDecoderConfigurationRecord.BitDepthLumaMinus8 = sps.BitDepthLumaMinus8;
            hevcConfigurationBox.HevcDecoderConfigurationRecord.TemporalIdNested = sps.SpsTemporalIdNestingFlag ? 1 : 0;
            hevcConfigurationBox.HevcDecoderConfigurationRecord.LengthSizeMinusOne = 3; // 4 bytes size block inserted in between NAL units

            HevcNalArray spsArray = new HevcNalArray();
            spsArray.ArrayCompleteness = 1;
            spsArray.NalUnitType = H265NalUnitTypes.SPS;
            foreach (var sp in track.Sps.Values)
            {
                spsArray.NalUnits.Add(H265SpsNalUnit.Build(sp));
            }

            HevcNalArray ppsArray = new HevcNalArray();
            ppsArray.ArrayCompleteness = 1;
            ppsArray.NalUnitType = H265NalUnitTypes.PPS;
            foreach (var pp in track.Pps.Values)
            {
                ppsArray.NalUnits.Add(H265PpsNalUnit.Build(pp));
            }

            HevcNalArray vpsArray = new HevcNalArray();
            vpsArray.ArrayCompleteness = 1;
            vpsArray.NalUnitType = H265NalUnitTypes.VPS;
            foreach (var vp in track.Vps.Values)
            {
                vpsArray.NalUnits.Add(H265VpsNalUnit.Build(vp));
            }

            // correct order is VPS, SPS, PPS. Other order produced ffmpeg errors such as "VPS 0 does not exist" and "SPS 0 does not exist."
            hevcConfigurationBox.HevcDecoderConfigurationRecord.NalArrays.Add(vpsArray);
            hevcConfigurationBox.HevcDecoderConfigurationRecord.NalArrays.Add(spsArray);
            hevcConfigurationBox.HevcDecoderConfigurationRecord.NalArrays.Add(ppsArray);

            visualSampleEntry.Children.Add(hevcConfigurationBox);

            return visualSampleEntry;
        }

        public override void FillTkhdBox(TkhdBox tkhd)
        {
            var dim = Sps.FirstOrDefault().Value.CalculateDimensions();
            tkhd.Width = dim.Width;
            tkhd.Height = dim.Height;
        }
    }

    public class H265VpsNalUnit
    {
        public H265NalUnitHeader Header { get; set; }
        public int VpsParameterSetId { get; set; }
        public bool VpsBaseLayerInternalFlag { get; set; }
        public bool VpsBaseLayerAvailableFlag { get; set; }
        public int VpsMaxLayersMinus1 { get; set; }
        public int VpsMaxSubLayersMinus1 { get; set; }
        public bool VpsTemporalIdNestingFlag { get; set; }
        public int VpsReserved0xffff16bits { get; set; }
        public H265ProfileTier ProfileTier { get; set; }
        public bool VpsSubLayerOrderingInfoPresentFlag { get; set; }
        public int[] VpsMaxDecPicBufferingMinus1 { get; set; }
        public int[] VpsMaxNumReorderPics { get; set; }
        public int[] VpsMaxLatencyIncreasePlus1 { get; set; }
        public int VpsMaxLayerId { get; set; }
        public int VpsNumLayerSetsMinus1 { get; set; }
        public bool[,] LayerIdIncludedFlag { get; set; }
        public bool VpsTimingInfoPresentFlag { get; set; }
        public int VpsNumUnitsInTick { get; set; }
        public int VpsTimeScale { get; set; }
        public bool VpsPocProportionalToTimingFlag { get; set; }
        public int VpsNumTicksPocDiffOneMinus1 { get; set; }
        public int VpsNumHrdParameters { get; set; }
        public int[] HrdLayerSetIdx { get; set; }
        public bool[] CprmsPresentFlag { get; set; }
        public H265HrdParameters[] HrdParameters { get; set; }
        public bool VpsExtensionFlag { get; set; }
        public List<bool> VpsExtensionDataFlag { get; set; } = new List<bool>();

        public H265VpsNalUnit(
            H265NalUnitHeader header,
            int vpsParameterSetId,
            bool vpsBaseLayerInternalFlag,
            bool vpsBaseLayerAvailableFlag,
            int vpsMaxLayersMinus1,
            int vpsMaxSubLayersMinus1, 
            bool vpsTemporalIdNestingFlag,
            int vpsReserved0xffff16bits,
            H265ProfileTier profileTier,
            bool vpsSubLayerOrderingInfoPresentFlag, 
            int[] vpsMaxDecPicBufferingMinus1,
            int[] vpsMaxNumReorderPics, 
            int[] vpsMaxLatencyIncreasePlus1, 
            int vpsMaxLayerId, 
            int vpsNumLayerSetsMinus1, 
            bool[,] layerIdIncludedFlag, 
            bool vpsTimingInfoPresentFlag, 
            int vpsNumUnitsInTick,
            int vpsTimeScale, 
            bool vpsPocProportionalToTimingFlag, 
            int vpsNumTicksPocDiffOneMinus1, 
            int vpsNumHrdParameters, 
            int[] hrdLayerSetIdx, 
            bool[] cprmsPresentFlag, 
            H265HrdParameters[] hrdParameters, 
            bool vpsExtensionFlag, 
            List<bool> vpsExtensionDataFlag)
        {
            Header = header;
            VpsParameterSetId = vpsParameterSetId;
            VpsBaseLayerInternalFlag = vpsBaseLayerInternalFlag;
            VpsBaseLayerAvailableFlag = vpsBaseLayerAvailableFlag;
            VpsMaxLayersMinus1 = vpsMaxLayersMinus1;
            VpsMaxSubLayersMinus1 = vpsMaxSubLayersMinus1;
            VpsTemporalIdNestingFlag = vpsTemporalIdNestingFlag;
            VpsReserved0xffff16bits = vpsReserved0xffff16bits;
            ProfileTier = profileTier;
            VpsSubLayerOrderingInfoPresentFlag = vpsSubLayerOrderingInfoPresentFlag;
            VpsMaxDecPicBufferingMinus1 = vpsMaxDecPicBufferingMinus1;
            VpsMaxNumReorderPics = vpsMaxNumReorderPics;
            VpsMaxLatencyIncreasePlus1 = vpsMaxLatencyIncreasePlus1;
            VpsMaxLayerId = vpsMaxLayerId;
            VpsNumLayerSetsMinus1 = vpsNumLayerSetsMinus1;
            LayerIdIncludedFlag = layerIdIncludedFlag;
            VpsTimingInfoPresentFlag = vpsTimingInfoPresentFlag;
            VpsNumUnitsInTick = vpsNumUnitsInTick;
            VpsTimeScale = vpsTimeScale;
            VpsPocProportionalToTimingFlag = vpsPocProportionalToTimingFlag;
            VpsNumTicksPocDiffOneMinus1 = vpsNumTicksPocDiffOneMinus1;
            VpsNumHrdParameters = vpsNumHrdParameters;
            HrdLayerSetIdx = hrdLayerSetIdx;
            CprmsPresentFlag = cprmsPresentFlag;
            HrdParameters = hrdParameters;
            VpsExtensionFlag = vpsExtensionFlag;
            VpsExtensionDataFlag = vpsExtensionDataFlag;
        }

        public static H265VpsNalUnit Parse(byte[] vps)
        {
            using (MemoryStream ms = new MemoryStream(vps))
            {
                return Parse((ushort)vps.Length, ms);
            }
        }

        private static H265VpsNalUnit Parse(ushort size, MemoryStream stream)
        {
            NalBitStreamReader bitstream = new NalBitStreamReader(stream);
            H265NalUnitHeader header = H265NalUnitHeader.ParseNALHeader(bitstream);

            int vpsParameterSetId = bitstream.ReadBits(4);

            bool vpsBaseLayerInternalFlag = bitstream.ReadBit() != 0;
            bool vpsBaseLayerAvailableFlag = bitstream.ReadBit() != 0;

            int vpsMaxLayersMinus1 = bitstream.ReadBits(6);
            int vpsMaxSubLayersMinus1 = bitstream.ReadBits(3);
            bool vpsTemporalIdNestingFlag = bitstream.ReadBit() != 0;
            int vpsReserved0xffff16bits = bitstream.ReadBits(16);

            if (vpsReserved0xffff16bits != 0xffff)
                throw new Exception("Invalid VPS!");

            H265ProfileTier profileTier = H265ProfileTier.Parse(bitstream, true, vpsMaxSubLayersMinus1);

            bool vpsSubLayerOrderingInfoPresentFlag = bitstream.ReadBit() != 0;
            int[] vpsMaxDecPicBufferingMinus1 = new int[vpsSubLayerOrderingInfoPresentFlag ? 1 : vpsMaxSubLayersMinus1 + 1];
            int[] vpsMaxNumReorderPics = new int[vpsSubLayerOrderingInfoPresentFlag ? 1 : vpsMaxSubLayersMinus1 + 1];
            int[] vpsMaxLatencyIncreasePlus1 = new int[vpsSubLayerOrderingInfoPresentFlag ? 1 : vpsMaxSubLayersMinus1 + 1];
            for (int i = vpsSubLayerOrderingInfoPresentFlag ? 0 : vpsMaxSubLayersMinus1; i <= vpsMaxSubLayersMinus1; i++)
            {
                vpsMaxDecPicBufferingMinus1[i] = bitstream.ReadUE();
                vpsMaxNumReorderPics[i] = bitstream.ReadUE();
                vpsMaxLatencyIncreasePlus1[i] = bitstream.ReadUE();
            }
            int vpsMaxLayerId = bitstream.ReadBits(6);
            int vpsNumLayerSetsMinus1 = bitstream.ReadUE();
            bool[,] layerIdIncludedFlag = new bool[vpsNumLayerSetsMinus1 + 1, vpsMaxLayerId + 1];
            for (int i = 1; i <= vpsNumLayerSetsMinus1; i++)
            {
                for (int j = 0; j <= vpsMaxLayerId; j++)
                {
                    layerIdIncludedFlag[i, j] = bitstream.ReadBit() != 0;
                }
            }
            bool vpsTimingInfoPresentFlag = bitstream.ReadBit() != 0;

            int vpsNumUnitsInTick = 0;
            int vpsTimeScale = 0;
            bool vpsPocProportionalToTimingFlag = false;
            int vpsNumTicksPocDiffOneMinus1 = -1;
            int vpsNumHrdParameters = 0;
            int[] hrdLayerSetIdx = null;
            bool[] cprmsPresentFlag = null;
            H265HrdParameters[] hrdParameters = null;
            if (vpsTimingInfoPresentFlag)
            {
                vpsNumUnitsInTick = bitstream.ReadBits(32);
                vpsTimeScale = bitstream.ReadBits(32);
                vpsPocProportionalToTimingFlag = bitstream.ReadBit() != 0;
                if (vpsPocProportionalToTimingFlag)
                {
                    vpsNumTicksPocDiffOneMinus1 = bitstream.ReadUE();
                }
                vpsNumHrdParameters = bitstream.ReadUE();
                hrdLayerSetIdx = new int[vpsNumHrdParameters];
                cprmsPresentFlag = new bool[vpsNumHrdParameters];
                hrdParameters = new H265HrdParameters[vpsNumHrdParameters];
                for (int i = 0; i < vpsNumHrdParameters; i++)
                {
                    hrdLayerSetIdx[i] = bitstream.ReadUE();
                    if (i > 0)
                    {
                        cprmsPresentFlag[i] = bitstream.ReadBit() != 0;
                    }
                    else
                    {
                        cprmsPresentFlag[0] = true;
                    }

                    hrdParameters[i] = H265HrdParameters.Parse(bitstream, cprmsPresentFlag[i], vpsMaxSubLayersMinus1);
                }
            }

            bool vpsExtensionFlag = bitstream.ReadBit() != 0;
            List<bool> vpsExtensionDataFlag = new List<bool>();
            if (vpsExtensionFlag)
            {
                while (bitstream.HasMoreRBSPData(size))
                {
                    vpsExtensionDataFlag.Add(bitstream.ReadBit() != 0);
                }
            }

            bitstream.ReadTrailingBits();

            return new H265VpsNalUnit(
                header,
                vpsParameterSetId,
                vpsBaseLayerInternalFlag,
                vpsBaseLayerAvailableFlag,
                vpsMaxLayersMinus1,
                vpsMaxSubLayersMinus1,
                vpsTemporalIdNestingFlag,
                vpsReserved0xffff16bits,
                profileTier,
                vpsSubLayerOrderingInfoPresentFlag,
                vpsMaxDecPicBufferingMinus1,
                vpsMaxNumReorderPics,
                vpsMaxLatencyIncreasePlus1,
                vpsMaxLayerId,
                vpsNumLayerSetsMinus1,
                layerIdIncludedFlag,
                vpsTimingInfoPresentFlag,
                vpsNumUnitsInTick,
                vpsTimeScale,
                vpsPocProportionalToTimingFlag,
                vpsNumTicksPocDiffOneMinus1,
                vpsNumHrdParameters,
                hrdLayerSetIdx,
                cprmsPresentFlag,
                hrdParameters,
                vpsExtensionFlag,
                vpsExtensionDataFlag
                );
        }

        public static byte[] Build(H265VpsNalUnit b)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Build(ms, b);
                ms.Seek(0, SeekOrigin.Begin);
                return ms.ToArray();
            }
        }

        public static void Build(MemoryStream stream, H265VpsNalUnit b)
        {
            NalBitStreamWriter bitstream = new NalBitStreamWriter(stream);
            H265NalUnitHeader.BuildNALHeader(bitstream, b.Header);

            bitstream.WriteBits(4, b.VpsParameterSetId);

            bitstream.WriteBit(b.VpsBaseLayerInternalFlag);
            bitstream.WriteBit(b.VpsBaseLayerAvailableFlag);

            bitstream.WriteBits(6, b.VpsMaxLayersMinus1);
            bitstream.WriteBits(3, b.VpsMaxSubLayersMinus1);
            bitstream.WriteBit(b.VpsTemporalIdNestingFlag);
            bitstream.WriteBits(16, b.VpsReserved0xffff16bits);

            H265ProfileTier.Build(bitstream, b.ProfileTier, b.VpsMaxSubLayersMinus1);

            bitstream.WriteBit(b.VpsSubLayerOrderingInfoPresentFlag);
            for (int i = b.VpsSubLayerOrderingInfoPresentFlag ? 0 : b.VpsMaxSubLayersMinus1; i <= b.VpsMaxSubLayersMinus1; i++)
            {
                bitstream.WriteUE((uint)b.VpsMaxDecPicBufferingMinus1[i]);
                bitstream.WriteUE((uint)b.VpsMaxNumReorderPics[i]);
                bitstream.WriteUE((uint)b.VpsMaxLatencyIncreasePlus1[i]);
            }
            bitstream.WriteBits(6, b.VpsMaxLayerId);
            bitstream.WriteUE((uint)b.VpsNumLayerSetsMinus1);
            for (int i = 1; i <= b.VpsNumLayerSetsMinus1; i++)
            {
                for (int j = 0; j <= b.VpsMaxLayerId; j++)
                {
                    bitstream.WriteBit(b.LayerIdIncludedFlag[i, j]);
                }
            }
            bitstream.WriteBit(b.VpsTimingInfoPresentFlag);

            if (b.VpsTimingInfoPresentFlag)
            {
                bitstream.WriteBits(32, b.VpsNumUnitsInTick);
                bitstream.WriteBits(32, b.VpsTimeScale);
                bitstream.WriteBit(b.VpsPocProportionalToTimingFlag);
                if (b.VpsPocProportionalToTimingFlag)
                {
                    bitstream.WriteUE((uint)b.VpsNumTicksPocDiffOneMinus1);
                }
                bitstream.WriteUE((uint)b.VpsNumHrdParameters);
                for (int i = 0; i < b.VpsNumHrdParameters; i++)
                {
                    bitstream.WriteUE((uint)b.HrdLayerSetIdx[i]);
                    if (i > 0)
                    {
                        bitstream.WriteBit(b.CprmsPresentFlag[i]);
                    }
                   
                    H265HrdParameters.Build(bitstream, b.HrdParameters[i], true, b.VpsMaxSubLayersMinus1);
                }
            }

            bitstream.WriteBit(b.VpsExtensionFlag);
            if (b.VpsExtensionFlag)
            {
                foreach(var d in b.VpsExtensionDataFlag)
                    bitstream.WriteBit(d);
            }

            bitstream.WriteTrailingBits();
        }
    }

    public class H265ProfileTier
    {
        public int GeneralProfileSpace { get; set; }
        public bool GeneralTierFlag { get; set; }
        public int GeneralProfileIdc { get; set; }
        public bool[] GeneralProfileCompatibilityFlags { get; set; }
        public bool GeneralProgressiveSourceFlag { get; set; }
        public bool GeneralInterlacedSourceFlag { get; set; }
        public bool GeneralNonPackedConstraintFlag { get; set; }
        public bool GeneralFrameOnlyConstraintFlag { get; set; }
        public bool GeneralMax12BitConstraintFlag { get; set; }
        public bool GeneralMax10BitConstraintFlag { get; set; }
        public bool GeneralMax8BitConstraintFlag { get; set; }
        public bool GeneralMax422ChromaConstraintFlag { get; set; }
        public bool GeneralMax420ChromaConstraintFlag { get; set; }
        public bool GeneralMaxMonochromeConstraintFlag { get; set; }
        public bool GeneralIntraConstraintFlag { get; set; }
        public bool GeneralOnePictureOnlyConstraintFlag { get; set; }
        public bool GeneralLowerBitRateConstraintFlag { get; set; }
        public bool GeneralMax14BitConstraintFlag { get; set; }
        public long GeneralReservedZero33Bits { get; set; }
        public long GeneralReservedZero34Bits { get; set; }
        public int GeneralReservedZero7Bits { get; set; }
        public long GeneralReservedZero35Bits { get; set; }
        public long GeneralReservedZero43Bits { get; set; }
        public bool GeneralInbldFlag { get; set; }
        public bool GeneralReservedZeroBit { get; set; }
        public int GeneralLevelIdc { get; }
        public bool[] SubLayerProfilePresentFlag { get; }
        public bool[] SubLayerLevelPresentFlag { get; }
        public int[] ReservedZero2Bits { get; }
        public int[] SubLayerProfileSpace { get; }
        public bool[] SubLayerTierFlag { get; }
        public int[] SubLayerProfileIdc { get; }
        public bool[,] SubLayerProfileCompatibilityFlag { get; }
        public bool[] SubLayerProgressiveSourceFlag { get; }
        public bool[] SubLayerInterlacedSourceFlag { get; }
        public bool[] SubLayerNonPackedConstraintFlag { get; }
        public bool[] SubLayerFrameOnlyConstraintFlag { get; }
        public int[] SubLayerLevelIdc { get; }
        public long[] ReservedBits { get; }

        public H265ProfileTier(
            int generalProfileSpace,
            bool generalTierFlag,
            int generalProfileIdc,
            bool[] generalProfileCompatibilityFlags, 
            bool generalProgressiveSourceFlag, 
            bool generalInterlacedSourceFlag,
            bool generalNonPackedConstraintFlag,
            bool generalFrameOnlyConstraintFlag, 
            bool generalMax12BitConstraintFlag, 
            bool generalMax10BitConstraintFlag, 
            bool generalMax8BitConstraintFlag,
            bool generalMax422ChromaConstraintFlag,
            bool generalMax420ChromaConstraintFlag,
            bool generalMaxMonochromeConstraintFlag,
            bool generalIntraConstraintFlag, 
            bool generalOnePictureOnlyConstraintFlag,
            bool generalLowerBitRateConstraintFlag,
            bool generalMax14BitConstraintFlag, 
            long generalReservedZero33Bits, 
            long generalReservedZero34Bits, 
            int generalReservedZero7Bits, 
            long generalReservedZero35Bits,
            long generalReservedZero43Bits,
            bool generalInbldFlag, 
            bool generalReservedZeroBit,
            int generalLevelIdc, 
            bool[] subLayerProfilePresentFlag, 
            bool[] subLayerLevelPresentFlag, 
            int[] reservedZero2Bits, 
            int[] subLayerProfileSpace, 
            bool[] subLayerTierFlag, 
            int[] subLayerProfileIdc, 
            bool[,] subLayerProfileCompatibilityFlag,
            bool[] subLayerProgressiveSourceFlag, 
            bool[] subLayerInterlacedSourceFlag, 
            bool[] subLayerNonPackedConstraintFlag,
            bool[] subLayerFrameOnlyConstraintFlag, 
            int[] subLayerLevelIdc, 
            long[] reservedBits)
        {
            GeneralProfileSpace = generalProfileSpace;
            GeneralTierFlag = generalTierFlag;
            GeneralProfileIdc = generalProfileIdc;
            GeneralProfileCompatibilityFlags = generalProfileCompatibilityFlags;
            GeneralProgressiveSourceFlag = generalProgressiveSourceFlag;
            GeneralInterlacedSourceFlag = generalInterlacedSourceFlag;
            GeneralNonPackedConstraintFlag = generalNonPackedConstraintFlag;
            GeneralFrameOnlyConstraintFlag = generalFrameOnlyConstraintFlag;
            GeneralMax12BitConstraintFlag = generalMax12BitConstraintFlag;
            GeneralMax10BitConstraintFlag = generalMax10BitConstraintFlag;
            GeneralMax8BitConstraintFlag = generalMax8BitConstraintFlag;
            GeneralMax422ChromaConstraintFlag = generalMax422ChromaConstraintFlag;
            GeneralMax420ChromaConstraintFlag = generalMax420ChromaConstraintFlag;
            GeneralMaxMonochromeConstraintFlag = generalMaxMonochromeConstraintFlag;
            GeneralIntraConstraintFlag = generalIntraConstraintFlag;
            GeneralOnePictureOnlyConstraintFlag = generalOnePictureOnlyConstraintFlag;
            GeneralLowerBitRateConstraintFlag = generalLowerBitRateConstraintFlag;
            GeneralMax14BitConstraintFlag = generalMax14BitConstraintFlag;
            GeneralReservedZero33Bits = generalReservedZero33Bits;
            GeneralReservedZero34Bits = generalReservedZero34Bits;
            GeneralReservedZero7Bits = generalReservedZero7Bits;
            GeneralReservedZero35Bits = generalReservedZero35Bits;
            GeneralReservedZero43Bits = generalReservedZero43Bits;
            GeneralInbldFlag = generalInbldFlag;
            GeneralReservedZeroBit = generalReservedZeroBit;
            GeneralLevelIdc = generalLevelIdc;
            SubLayerProfilePresentFlag = subLayerProfilePresentFlag;
            SubLayerLevelPresentFlag = subLayerLevelPresentFlag;
            ReservedZero2Bits = reservedZero2Bits;
            SubLayerProfileSpace = subLayerProfileSpace;
            SubLayerTierFlag = subLayerTierFlag;
            SubLayerProfileIdc = subLayerProfileIdc;
            SubLayerProfileCompatibilityFlag = subLayerProfileCompatibilityFlag;
            SubLayerProgressiveSourceFlag = subLayerProgressiveSourceFlag;
            SubLayerInterlacedSourceFlag = subLayerInterlacedSourceFlag;
            SubLayerNonPackedConstraintFlag = subLayerNonPackedConstraintFlag;
            SubLayerFrameOnlyConstraintFlag = subLayerFrameOnlyConstraintFlag;
            SubLayerLevelIdc = subLayerLevelIdc;
            ReservedBits = reservedBits;
        }

        public static H265ProfileTier Parse(NalBitStreamReader bitstream, bool profilePresent, int maxNumSubLayersMinus1)
        {
            int generalProfileSpace = 0;
            bool generalTierFlag = false;
            int generalProfileIdc = 0;
            bool[] generalProfileCompatibilityFlags = new bool[32];
            bool generalProgressiveSourceFlag = false;
            bool generalInterlacedSourceFlag = false;
            bool generalNonPackedConstraintFlag = false;
            bool generalFrameOnlyConstraintFlag = false;
            bool generalMax12BitConstraintFlag = false;
            bool generalMax10BitConstraintFlag = false;
            bool generalMax8BitConstraintFlag = false;
            bool generalMax422ChromaConstraintFlag = false;
            bool generalMax420ChromaConstraintFlag = false;
            bool generalMaxMonochromeConstraintFlag = false;
            bool generalIntraConstraintFlag = false;
            bool generalOnePictureOnlyConstraintFlag = false;
            bool generalLowerBitRateConstraintFlag = false;
            bool generalMax14BitConstraintFlag = false;
            long generalReservedZero33Bits = 0;
            long generalReservedZero34Bits = 0;
            int generalReservedZero7Bits = 0;
            long generalReservedZero35Bits = 0;
            long generalReservedZero43Bits = 0;
            bool generalInbldFlag = false;
            bool generalReservedZeroBit = false;

            if (profilePresent)
            {
                generalProfileSpace = bitstream.ReadBits(2);
                generalTierFlag = bitstream.ReadBit() != 0;
                generalProfileIdc = bitstream.ReadBits(5);

                for (int i = 0; i < 32; i++)
                {
                    generalProfileCompatibilityFlags[i] = bitstream.ReadBit() != 0;
                }

                generalProgressiveSourceFlag = bitstream.ReadBit() != 0;
                generalInterlacedSourceFlag = bitstream.ReadBit() != 0;
                generalNonPackedConstraintFlag = bitstream.ReadBit() != 0;
                generalFrameOnlyConstraintFlag = bitstream.ReadBit() != 0;

                if (generalProfileIdc == 4 || generalProfileCompatibilityFlags[4] ||
                    generalProfileIdc == 5 || generalProfileCompatibilityFlags[5] ||
                    generalProfileIdc == 6 || generalProfileCompatibilityFlags[6] ||
                    generalProfileIdc == 7 || generalProfileCompatibilityFlags[7] ||
                    generalProfileIdc == 8 || generalProfileCompatibilityFlags[8] ||
                    generalProfileIdc == 9 || generalProfileCompatibilityFlags[9] ||
                    generalProfileIdc == 10 || generalProfileCompatibilityFlags[10])
                {
                    generalMax12BitConstraintFlag = bitstream.ReadBit() != 0;
                    generalMax10BitConstraintFlag = bitstream.ReadBit() != 0;
                    generalMax8BitConstraintFlag = bitstream.ReadBit() != 0;
                    generalMax422ChromaConstraintFlag = bitstream.ReadBit() != 0;
                    generalMax420ChromaConstraintFlag = bitstream.ReadBit() != 0;
                    generalMaxMonochromeConstraintFlag = bitstream.ReadBit() != 0;
                    generalIntraConstraintFlag = bitstream.ReadBit() != 0;
                    generalOnePictureOnlyConstraintFlag = bitstream.ReadBit() != 0;
                    generalLowerBitRateConstraintFlag = bitstream.ReadBit() != 0;

                    if (generalProfileIdc == 5 || generalProfileCompatibilityFlags[5] ||
                        generalProfileIdc == 9 || generalProfileCompatibilityFlags[9] ||
                        generalProfileIdc == 10 || generalProfileCompatibilityFlags[10])
                    {
                        generalMax14BitConstraintFlag = bitstream.ReadBit() != 0;
                        generalReservedZero33Bits = bitstream.ReadBitsLong(33);

                        if (generalReservedZero33Bits != 0)
                            throw new Exception("Invalid H265ProfileTier!");
                    }
                    else
                    {
                        generalReservedZero34Bits = bitstream.ReadBitsLong(34);

                        if (generalReservedZero34Bits != 0)
                            throw new Exception("Invalid H265ProfileTier!");
                    }
                }
                else if (generalProfileIdc == 2 || generalProfileCompatibilityFlags[2])
                {
                    generalReservedZero7Bits = bitstream.ReadBits(7);
                    if (generalReservedZero7Bits != 0)
                        throw new Exception("Invalid H265ProfileTier!");

                    generalOnePictureOnlyConstraintFlag = bitstream.ReadBit() != 0;
                    generalReservedZero35Bits = bitstream.ReadBitsLong(35);

                    if (generalReservedZero35Bits != 0)
                        throw new Exception("Invalid H265ProfileTier!");

                }
                else
                {
                    generalReservedZero43Bits = bitstream.ReadBitsLong(43);

                    if (generalReservedZero43Bits != 0)
                        throw new Exception("Invalid H265ProfileTier!");
                }

                if ((generalProfileIdc >= 1 && generalProfileIdc <= 5) ||
                    generalProfileIdc == 9 ||
                    generalProfileCompatibilityFlags[1] || generalProfileCompatibilityFlags[2] ||
                    generalProfileCompatibilityFlags[3] || generalProfileCompatibilityFlags[4] ||
                    generalProfileCompatibilityFlags[5] || generalProfileCompatibilityFlags[9])
                {
                    generalInbldFlag = bitstream.ReadBit() != 0;
                }
                else
                {
                    generalReservedZeroBit = bitstream.ReadBit() != 0;

                    if (generalReservedZeroBit)
                        throw new Exception("Invalid H265ProfileTier!");
                }
            }

            int generalLevelIdc = bitstream.ReadBits(8);

            bool[] subLayerProfilePresentFlag = new bool[maxNumSubLayersMinus1];
            bool[] subLayerLevelPresentFlag = new bool[maxNumSubLayersMinus1];
            for (int i = 0; i < maxNumSubLayersMinus1; i++)
            {
                subLayerProfilePresentFlag[i] = bitstream.ReadBit() != 0;
                subLayerLevelPresentFlag[i] = bitstream.ReadBit() != 0;
            }

            int[] reservedZero2Bits = null;
            if (maxNumSubLayersMinus1 > 0)
            {
                reservedZero2Bits = new int[8];
                for (int i = maxNumSubLayersMinus1; i < 8; i++)
                {
                    reservedZero2Bits[i] = bitstream.ReadBits(2);

                    if (reservedZero2Bits[i] != 0)
                        throw new Exception("Invalid H265ProfileTier!");
                }
            }

            int[] subLayerProfileSpace = new int[maxNumSubLayersMinus1];
            bool[] subLayerTierFlag = new bool[maxNumSubLayersMinus1];
            int[] subLayerProfileIdc = new int[maxNumSubLayersMinus1];
            bool[,] subLayerProfileCompatibilityFlag = new bool[maxNumSubLayersMinus1, 32];
            bool[] subLayerProgressiveSourceFlag = new bool[maxNumSubLayersMinus1];

            bool[] subLayerInterlacedSourceFlag = new bool[maxNumSubLayersMinus1];
            bool[] subLayerNonPackedConstraintFlag = new bool[maxNumSubLayersMinus1];
            bool[] subLayerFrameOnlyConstraintFlag = new bool[maxNumSubLayersMinus1];
            int[] subLayerLevelIdc = new int[maxNumSubLayersMinus1];
            long[] reservedBits = new long[maxNumSubLayersMinus1];

            for (int i = 0; i < maxNumSubLayersMinus1; i++)
            {
                if (subLayerProfilePresentFlag[i])
                {
                    subLayerProfileSpace[i] = bitstream.ReadBits(2);
                    subLayerTierFlag[i] = bitstream.ReadBit() != 0;
                    subLayerProfileIdc[i] = bitstream.ReadBits(5);
                    for (int j = 0; j < 32; j++)
                    {
                        subLayerProfileCompatibilityFlag[i, j] = bitstream.ReadBit() != 0;
                    }
                    subLayerProgressiveSourceFlag[i] = bitstream.ReadBit() != 0;
                    subLayerInterlacedSourceFlag[i] = bitstream.ReadBit() != 0;
                    subLayerNonPackedConstraintFlag[i] = bitstream.ReadBit() != 0;
                    subLayerFrameOnlyConstraintFlag[i] = bitstream.ReadBit() != 0;
                    reservedBits[i] = bitstream.ReadBitsLong(44); // reserved
                }
                if (subLayerLevelPresentFlag[i])
                    subLayerLevelIdc[i] = bitstream.ReadBits(8);
            }

            return new H265ProfileTier(
                generalProfileSpace,
                generalTierFlag,
                generalProfileIdc,
                generalProfileCompatibilityFlags,
                generalProgressiveSourceFlag,
                generalInterlacedSourceFlag,
                generalNonPackedConstraintFlag,
                generalFrameOnlyConstraintFlag,
                generalMax12BitConstraintFlag,
                generalMax10BitConstraintFlag,
                generalMax8BitConstraintFlag,
                generalMax422ChromaConstraintFlag,
                generalMax420ChromaConstraintFlag,
                generalMaxMonochromeConstraintFlag,
                generalIntraConstraintFlag,
                generalOnePictureOnlyConstraintFlag,
                generalLowerBitRateConstraintFlag,
                generalMax14BitConstraintFlag,
                generalReservedZero33Bits,
                generalReservedZero34Bits,
                generalReservedZero7Bits,
                generalReservedZero35Bits,
                generalReservedZero43Bits,
                generalInbldFlag,
                generalReservedZeroBit,
                generalLevelIdc,
                subLayerProfilePresentFlag,
                subLayerLevelPresentFlag,
                reservedZero2Bits,
                subLayerProfileSpace,
                subLayerTierFlag,
                subLayerProfileIdc,
                subLayerProfileCompatibilityFlag,
                subLayerProgressiveSourceFlag,
                subLayerInterlacedSourceFlag,
                subLayerNonPackedConstraintFlag,
                subLayerFrameOnlyConstraintFlag,
                subLayerLevelIdc,
                reservedBits
            );
        }

        public static void Build(NalBitStreamWriter bitstream, H265ProfileTier b, int maxNumSubLayersMinus1)
        {
            bitstream.WriteBits(2, b.GeneralProfileSpace);
            bitstream.WriteBit(b.GeneralTierFlag);
            bitstream.WriteBits(5, b.GeneralProfileIdc);
            b.BuildGeneralProfileCompatibilityFlags(bitstream);
            b.BuildGeneralProfileConstraintIndicatorFlags(bitstream);

            bitstream.WriteBits(8, b.GeneralLevelIdc);

            for (int i = 0; i < maxNumSubLayersMinus1; i++)
            {
                bitstream.WriteBit(b.SubLayerProfilePresentFlag[i]);
                bitstream.WriteBit(b.SubLayerLevelPresentFlag[i]);
            }

            if (maxNumSubLayersMinus1 > 0)
            {
                for (int i = maxNumSubLayersMinus1; i < 8; i++)
                {
                    bitstream.WriteBits(2, b.ReservedZero2Bits[i]);
                }
            }

            for (int i = 0; i < maxNumSubLayersMinus1; i++)
            {
                if (b.SubLayerProfilePresentFlag[i])
                {
                    bitstream.WriteBits(2, b.SubLayerProfileSpace[i]);
                    bitstream.WriteBit(b.SubLayerTierFlag[i]);
                    bitstream.WriteBits(5, b.SubLayerProfileIdc[i]);
                    for (int j = 0; j < 32; j++)
                    {
                        bitstream.WriteBit(b.SubLayerProfileCompatibilityFlag[i, j]);
                    }
                    bitstream.WriteBit(b.SubLayerProgressiveSourceFlag[i]);
                    bitstream.WriteBit(b.SubLayerInterlacedSourceFlag[i]);
                    bitstream.WriteBit(b.SubLayerNonPackedConstraintFlag[i]);
                    bitstream.WriteBit(b.SubLayerFrameOnlyConstraintFlag[i]);
                    bitstream.WriteBitsLong(44, b.ReservedBits[i]); // reserved
                }
                if (b.SubLayerLevelPresentFlag[i])
                    bitstream.WriteBits(8, b.SubLayerLevelIdc[i]);
            }
        }

        private void BuildGeneralProfileCompatibilityFlags(RawBitStreamWriter bitstream)
        {
            for (int i = 0; i < 32; i++)
            {
                bitstream.WriteBit(GeneralProfileCompatibilityFlags[i]);
            }
        }

        private void BuildGeneralProfileConstraintIndicatorFlags(RawBitStreamWriter bitstream)
        {
            bitstream.WriteBit(GeneralProgressiveSourceFlag);
            bitstream.WriteBit(GeneralInterlacedSourceFlag);
            bitstream.WriteBit(GeneralNonPackedConstraintFlag);
            bitstream.WriteBit(GeneralFrameOnlyConstraintFlag);

            if (GeneralProfileIdc == 4 || GeneralProfileCompatibilityFlags[4] ||
                GeneralProfileIdc == 5 || GeneralProfileCompatibilityFlags[5] ||
                GeneralProfileIdc == 6 || GeneralProfileCompatibilityFlags[6] ||
                GeneralProfileIdc == 7 || GeneralProfileCompatibilityFlags[7] ||
                GeneralProfileIdc == 8 || GeneralProfileCompatibilityFlags[8] ||
                GeneralProfileIdc == 9 || GeneralProfileCompatibilityFlags[9] ||
                GeneralProfileIdc == 10 || GeneralProfileCompatibilityFlags[10])
            {
                bitstream.WriteBit(GeneralMax12BitConstraintFlag);
                bitstream.WriteBit(GeneralMax10BitConstraintFlag);
                bitstream.WriteBit(GeneralMax8BitConstraintFlag);
                bitstream.WriteBit(GeneralMax422ChromaConstraintFlag);
                bitstream.WriteBit(GeneralMax420ChromaConstraintFlag);
                bitstream.WriteBit(GeneralMaxMonochromeConstraintFlag);
                bitstream.WriteBit(GeneralIntraConstraintFlag);
                bitstream.WriteBit(GeneralOnePictureOnlyConstraintFlag);
                bitstream.WriteBit(GeneralLowerBitRateConstraintFlag);

                if (GeneralProfileIdc == 5 || GeneralProfileCompatibilityFlags[5] ||
                    GeneralProfileIdc == 9 || GeneralProfileCompatibilityFlags[9] ||
                    GeneralProfileIdc == 10 || GeneralProfileCompatibilityFlags[10])
                {
                    bitstream.WriteBit(GeneralMax14BitConstraintFlag);
                    bitstream.WriteBitsLong(33, GeneralReservedZero33Bits);
                }
                else
                {
                    bitstream.WriteBitsLong(34, GeneralReservedZero34Bits);
                }
            }
            else if (GeneralProfileIdc == 2 || GeneralProfileCompatibilityFlags[2])
            {
                bitstream.WriteBits(7, GeneralReservedZero7Bits);
                bitstream.WriteBit(GeneralOnePictureOnlyConstraintFlag);
                bitstream.WriteBitsLong(35, GeneralReservedZero35Bits);
            }
            else
            {
                bitstream.WriteBitsLong(43, GeneralReservedZero43Bits);
            }

            if ((GeneralProfileIdc >= 1 && GeneralProfileIdc <= 5) ||
                GeneralProfileIdc == 9 ||
                GeneralProfileCompatibilityFlags[1] || GeneralProfileCompatibilityFlags[2] ||
                GeneralProfileCompatibilityFlags[3] || GeneralProfileCompatibilityFlags[4] ||
                GeneralProfileCompatibilityFlags[5] || GeneralProfileCompatibilityFlags[9])
            {
                bitstream.WriteBit(GeneralInbldFlag);
            }
            else
            {
                bitstream.WriteBit(GeneralReservedZeroBit);
            }
        }

        public ulong GetGeneralProfileConstraintIndicatorFlags()
        {
            using (var stream = new MemoryStream())
            {
                using (RawBitStreamWriter bitstream = new RawBitStreamWriter(stream))
                {
                    BuildGeneralProfileConstraintIndicatorFlags(bitstream);
                    bitstream.Flush();
                    while(stream.Length < 8)
                    {
                        bitstream.WriteBits(8, 0);
                    }
                    return BitConverter.ToUInt64(stream.ToArray(), 0);
                }
            }
        }

        public uint GetGeneralProfileCompatibilityFlags()
        {
            using (var stream = new MemoryStream())
            {
                using (RawBitStreamWriter bitstream = new RawBitStreamWriter(stream))
                {
                    BuildGeneralProfileCompatibilityFlags(bitstream);
                    bitstream.Flush();
                    while (stream.Length < 4)
                    {
                        bitstream.WriteBits(8, 0);
                    }
                    return BitConverter.ToUInt32(stream.ToArray(), 0);
                }
            }
        }
    }

    public class H265SpsNalUnit
    {
        public H265NalUnitHeader Header { get; set; }
        public int SeqParameterSetId { get; set; }
        public int SpsVideoParameterSetId { get; set; }
        public int SpsMaxSubLayersMinus1 { get; set; }
        public bool SpsTemporalIdNestingFlag { get; set; }
        public H265ProfileTier ProfileTier { get; set; }
        public int SpsSeqParameterSetId { get; set; }
        public int ChromaFormatIdc { get; set; }
        public bool SeparateColorPlaneFlag { get; set; }
        public int PicWidthInLumaSamples { get; set; }
        public int PicHeightInLumaSamples { get; set; }
        public bool ConformanceWindowFlag { get; set; }
        public int ConfWinLeftOffset { get; set; }
        public int ConfWinRightOffset { get; set; }
        public int ConfWinTopOffset { get; set; }
        public int ConfWinBottomOffset { get; set; }
        public int BitDepthLumaMinus8 { get; set; }
        public int BitDepthChromaMinus8 { get; set; }
        public int Log2MaxPicOrderCntLsbMinus4 { get; set; }
        public bool SpsSubLayerOrderingInfoPresentFlag { get; set; }
        public int[] SpsMaxDecPicBufferingMinus1 { get; set; }
        public int[] SpsMaxNumReorderPics { get; set; }
        public int[] SpsMaxLatencyIncreasePlus1 { get; set; }
        public int Log2MinLumaCodingBlockSizeMinus3 { get; set; }
        public int Log2DiffMaxMinLumaCodingBlockSize { get; set; }
        public int Log2MinTransformBlockSizeMinus2 { get; set; }
        public int Log2DiffMaxMinTransformBlockSize { get; set; }
        public int MaxTransformHierarchyDepthInter { get; set; }
        public int MaxTransformHierarchyDepthIntra { get; set; }
        public bool ScalingListEnabledFlag { get; set; }
        public bool SpsScalingListDataPresentFlag { get; set; }
        public List<H265ScalingListElement> ScalingListElements { get; set; }
        public bool AmpEnabledFlag { get; set; }
        public bool SampleAdaptiveOffsetEnabledFlag { get; set; }
        public bool PcmEnabledFlag { get; set; }
        public int PcmSampleBitDepthLumaMinus1 { get; set; }
        public int PcmSampleBitDepthChromaMinus1 { get; set; }
        public int Log2MinPcmLumaCodingBlockSizeMinus3 { get; set; }
        public int Log2DiffMaxMinPcmLumaCodingBlockSize { get; set; }
        public bool PcmLoopFilterDisabledFlag { get; set; }
        public int NumShortTermRefPicSets { get; set; }
        public long[] NumDeltaPocs { get; set; }
        public bool[] DeltaRpsSign { get; set; }
        public int[] AbsDeltaRpsMinus1 { get; set; }
        public bool[] InterRefPicSetPredictionFlag { get; set; }
        public long[] NumNegativePics { get; set; }
        public long[] NumPositivePics { get; set; }
        public List<int>[] Pics { get; set; }
        public bool LongTermRefPicsPresentFlag { get; set; }
        public int NumLongTermRefPicsSps { get; set; }
        public int[] LtRefPicPocLsbSps { get; set; }
        public bool[] UsedByCurrPicLtSpsFlag { get; set; }
        public bool SpsTemporalMvpEnabledFlag { get; set; }
        public bool StrongIntraSmoothingEnabledFlag { get; set; }
        public bool VuiParametersPresentFlag { get; set; }
        public H265VuiParameters VuiParameters { get; set; }
        public bool SpsExtensionPresentFlag { get; set; }

        public H265SpsNalUnit(
            H265NalUnitHeader header,
            int spsVideoParameterSetId,
            int spsMaxSubLayersMinus1,
            bool spsTemporalIdNestingFlag,
            H265ProfileTier profileTier,
            int spsSeqParameterSetId, 
            int chromaFormatIdc, 
            bool separateColorPlaneFlag, 
            int picWidthInLumaSamples, 
            int picHeightInLumaSamples,
            bool conformanceWindowFlag,
            int confWinLeftOffset, 
            int confWinRightOffset, 
            int confWinTopOffset, 
            int confWinBottomOffset, 
            int bitDepthLumaMinus8, 
            int bitDepthChromaMinus8,
            int log2MaxPicOrderCntLsbMinus4,
            bool spsSubLayerOrderingInfoPresentFlag, 
            int[] spsMaxDecPicBufferingMinus1, 
            int[] spsMaxNumReorderPics, 
            int[] spsMaxLatencyIncreasePlus1,
            int log2MinLumaCodingBlockSizeMinus3,
            int log2DiffMaxMinLumaCodingBlockSize, 
            int log2MinTransformBlockSizeMinus2, 
            int log2DiffMaxMinTransformBlockSize, 
            int maxTransformHierarchyDepthInter,
            int maxTransformHierarchyDepthIntra, 
            bool scalingListEnabledFlag, 
            bool spsScalingListDataPresentFlag,
            List<H265ScalingListElement> scalingListElements, 
            bool ampEnabledFlag, 
            bool sampleAdaptiveOffsetEnabledFlag, 
            bool pcmEnabledFlag, 
            int pcmSampleBitDepthLumaMinus1, 
            int pcmSampleBitDepthChromaMinus1,
            int log2MinPcmLumaCodingBlockSizeMinus3, 
            int log2DiffMaxMinPcmLumaCodingBlockSize, 
            bool pcmLoopFilterDisabledFlag,
            int numShortTermRefPicSets, 
            long[] numDeltaPocs,
            bool[] deltaRpsSign, 
            int[] absDeltaRpsMinus1, 
            bool[] interRefPicSetPredictionFlag, 
            long[] numNegativePics,
            long[] numPositivePics,
            List<int>[] pics, 
            bool longTermRefPicsPresentFlag, 
            int numLongTermRefPicsSps, 
            int[] ltRefPicPocLsbSps,
            bool[] usedByCurrPicLtSpsFlag, 
            bool spsTemporalMvpEnabledFlag,
            bool strongIntraSmoothingEnabledFlag, 
            bool vuiParametersPresentFlag, 
            H265VuiParameters vuiParameters,
            bool spsExtensionPresentFlag)
        {
            Header = header;
            SpsVideoParameterSetId = spsVideoParameterSetId;
            SpsMaxSubLayersMinus1 = spsMaxSubLayersMinus1;
            SpsTemporalIdNestingFlag = spsTemporalIdNestingFlag;
            ProfileTier = profileTier;
            SpsSeqParameterSetId = spsSeqParameterSetId;
            ChromaFormatIdc = chromaFormatIdc;
            SeparateColorPlaneFlag = separateColorPlaneFlag;
            PicWidthInLumaSamples = picWidthInLumaSamples;
            PicHeightInLumaSamples = picHeightInLumaSamples;
            ConformanceWindowFlag = conformanceWindowFlag;
            ConfWinLeftOffset = confWinLeftOffset;
            ConfWinRightOffset = confWinRightOffset;
            ConfWinTopOffset = confWinTopOffset;
            ConfWinBottomOffset = confWinBottomOffset;
            BitDepthLumaMinus8 = bitDepthLumaMinus8;
            BitDepthChromaMinus8 = bitDepthChromaMinus8;
            Log2MaxPicOrderCntLsbMinus4 = log2MaxPicOrderCntLsbMinus4;
            SpsSubLayerOrderingInfoPresentFlag = spsSubLayerOrderingInfoPresentFlag;
            SpsMaxDecPicBufferingMinus1 = spsMaxDecPicBufferingMinus1;
            SpsMaxNumReorderPics = spsMaxNumReorderPics;
            SpsMaxLatencyIncreasePlus1 = spsMaxLatencyIncreasePlus1;
            Log2MinLumaCodingBlockSizeMinus3 = log2MinLumaCodingBlockSizeMinus3;
            Log2DiffMaxMinLumaCodingBlockSize = log2DiffMaxMinLumaCodingBlockSize;
            Log2MinTransformBlockSizeMinus2 = log2MinTransformBlockSizeMinus2;
            Log2DiffMaxMinTransformBlockSize = log2DiffMaxMinTransformBlockSize;
            MaxTransformHierarchyDepthInter = maxTransformHierarchyDepthInter;
            MaxTransformHierarchyDepthIntra = maxTransformHierarchyDepthIntra;
            ScalingListEnabledFlag = scalingListEnabledFlag;
            SpsScalingListDataPresentFlag = spsScalingListDataPresentFlag;
            ScalingListElements = scalingListElements;
            AmpEnabledFlag = ampEnabledFlag;
            SampleAdaptiveOffsetEnabledFlag = sampleAdaptiveOffsetEnabledFlag;
            PcmEnabledFlag = pcmEnabledFlag;
            PcmSampleBitDepthLumaMinus1 = pcmSampleBitDepthLumaMinus1;
            PcmSampleBitDepthChromaMinus1 = pcmSampleBitDepthChromaMinus1;
            Log2MinPcmLumaCodingBlockSizeMinus3 = log2MinPcmLumaCodingBlockSizeMinus3;
            Log2DiffMaxMinPcmLumaCodingBlockSize = log2DiffMaxMinPcmLumaCodingBlockSize;
            PcmLoopFilterDisabledFlag = pcmLoopFilterDisabledFlag;
            NumShortTermRefPicSets = numShortTermRefPicSets;
            NumDeltaPocs = numDeltaPocs;
            DeltaRpsSign = deltaRpsSign;
            AbsDeltaRpsMinus1 = absDeltaRpsMinus1;
            InterRefPicSetPredictionFlag = interRefPicSetPredictionFlag;
            NumNegativePics = numNegativePics;
            NumPositivePics = numPositivePics;
            Pics = pics;
            LongTermRefPicsPresentFlag = longTermRefPicsPresentFlag;
            NumLongTermRefPicsSps = numLongTermRefPicsSps;
            LtRefPicPocLsbSps = ltRefPicPocLsbSps;
            UsedByCurrPicLtSpsFlag = usedByCurrPicLtSpsFlag;
            SpsTemporalMvpEnabledFlag = spsTemporalMvpEnabledFlag;
            StrongIntraSmoothingEnabledFlag = strongIntraSmoothingEnabledFlag;
            VuiParametersPresentFlag = vuiParametersPresentFlag;
            VuiParameters = vuiParameters;
            SpsExtensionPresentFlag = spsExtensionPresentFlag;
        }

        public static byte[] Build(H265SpsNalUnit b)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Build(ms, b);
                ms.Seek(0, SeekOrigin.Begin);
                return ms.ToArray();
            }
        }

        public static void Build(MemoryStream stream, H265SpsNalUnit b)
        {
            NalBitStreamWriter bitstream = new NalBitStreamWriter(stream);
            H265NalUnitHeader.BuildNALHeader(bitstream, b.Header);

            bitstream.WriteBits(4, b.SpsVideoParameterSetId);
            bitstream.WriteBits(3, b.SpsMaxSubLayersMinus1);
            bitstream.WriteBit(b.SpsTemporalIdNestingFlag);

            H265ProfileTier.Build(bitstream, b.ProfileTier, b.SpsMaxSubLayersMinus1);

            bitstream.WriteUE((uint)b.SpsSeqParameterSetId);
            bitstream.WriteUE((uint)b.ChromaFormatIdc);
            if (b.ChromaFormatIdc == 3)
            {
                bitstream.WriteBit(b.SeparateColorPlaneFlag);
            }
            bitstream.WriteUE((uint)b.PicWidthInLumaSamples);
            bitstream.WriteUE((uint)b.PicHeightInLumaSamples);
            bitstream.WriteBit(b.ConformanceWindowFlag);

            if (b.ConformanceWindowFlag)
            {
                bitstream.WriteUE((uint)b.ConfWinLeftOffset);
                bitstream.WriteUE((uint)b.ConfWinRightOffset);
                bitstream.WriteUE((uint)b.ConfWinTopOffset);
                bitstream.WriteUE((uint)b.ConfWinBottomOffset);
            }

            bitstream.WriteUE((uint)b.BitDepthLumaMinus8);
            bitstream.WriteUE((uint)b.BitDepthChromaMinus8);
            bitstream.WriteUE((uint)b.Log2MaxPicOrderCntLsbMinus4);
            bitstream.WriteBit(b.SpsSubLayerOrderingInfoPresentFlag);

            for (int i = b.SpsSubLayerOrderingInfoPresentFlag ? 0 : b.SpsMaxSubLayersMinus1; i <= b.SpsMaxSubLayersMinus1; i++)
            {
                bitstream.WriteUE((uint)b.SpsMaxDecPicBufferingMinus1[i]);
                bitstream.WriteUE((uint)b.SpsMaxNumReorderPics[i]);
                bitstream.WriteUE((uint)b.SpsMaxLatencyIncreasePlus1[i]);
            }

            bitstream.WriteUE((uint)b.Log2MinLumaCodingBlockSizeMinus3);
            bitstream.WriteUE((uint)b.Log2DiffMaxMinLumaCodingBlockSize);
            bitstream.WriteUE((uint)b.Log2MinTransformBlockSizeMinus2);
            bitstream.WriteUE((uint)b.Log2DiffMaxMinTransformBlockSize);
            bitstream.WriteUE((uint)b.MaxTransformHierarchyDepthInter);
            bitstream.WriteUE((uint)b.MaxTransformHierarchyDepthIntra);

            bitstream.WriteBit(b.ScalingListEnabledFlag);

            if (b.ScalingListEnabledFlag)
            {
                bitstream.WriteBit(b.SpsScalingListDataPresentFlag);
                if (b.SpsScalingListDataPresentFlag)
                {
                    H265ScalingListElement.Build(bitstream, b.ScalingListElements);
                }
            }

            bitstream.WriteBit(b.AmpEnabledFlag);
            bitstream.WriteBit(b.SampleAdaptiveOffsetEnabledFlag);
            bitstream.WriteBit(b.PcmEnabledFlag);

            if (b.PcmEnabledFlag)
            {
                bitstream.WriteBits(4, b.PcmSampleBitDepthLumaMinus1);
                bitstream.WriteBits(4, b.PcmSampleBitDepthChromaMinus1);
                bitstream.WriteUE((uint)b.Log2MinPcmLumaCodingBlockSizeMinus3);
                bitstream.WriteUE((uint)b.Log2DiffMaxMinPcmLumaCodingBlockSize);
                bitstream.WriteBit(b.PcmLoopFilterDisabledFlag);
            }

            bitstream.WriteUE((uint)b.NumShortTermRefPicSets);

            for (int rpsIdx = 0; rpsIdx < b.NumShortTermRefPicSets; rpsIdx++)
            {
                if (rpsIdx != 0)
                {
                    bitstream.WriteBit(b.InterRefPicSetPredictionFlag[rpsIdx]);
                }
                
                if (rpsIdx != 0 && b.InterRefPicSetPredictionFlag[rpsIdx])
                {
                    bitstream.WriteBit(b.DeltaRpsSign[rpsIdx]);
                    bitstream.WriteUE((uint)b.AbsDeltaRpsMinus1[rpsIdx]);

                    for (int i = 0; i <= b.NumDeltaPocs[rpsIdx - 1]; i++)
                    {
                        foreach(var bbb in b.Pics[rpsIdx])
                            bitstream.WriteBit(bbb);
                    }
                }
                else
                {
                    bitstream.WriteUE((uint)b.NumNegativePics[rpsIdx]);
                    bitstream.WriteUE((uint)b.NumPositivePics[rpsIdx]);
                    long delta_pics = (uint)b.NumNegativePics[rpsIdx] + (uint)b.NumPositivePics[rpsIdx];

                    for (long i = 0; i < delta_pics; ++i)
                    {

                        for (int h = 0; h < b.Pics[rpsIdx].Count; h += 2)
                        {
                            bitstream.WriteUE((uint)b.Pics[rpsIdx][h]);
                            bitstream.WriteBit(b.Pics[rpsIdx][h + 1]);
                        }
                    }
                }
            }

            bitstream.WriteBit(b.LongTermRefPicsPresentFlag);
            if (b.LongTermRefPicsPresentFlag)
            {
                bitstream.WriteUE((uint)b.NumLongTermRefPicsSps);
                for (int i = 0; i < b.NumLongTermRefPicsSps; i++)
                {
                    bitstream.WriteBits((uint)b.Log2MaxPicOrderCntLsbMinus4 + 4, b.LtRefPicPocLsbSps[i]);
                    bitstream.WriteBit(b.UsedByCurrPicLtSpsFlag[i]);
                }
            }
            bitstream.WriteBit(b.SpsTemporalMvpEnabledFlag);
            bitstream.WriteBit(b.StrongIntraSmoothingEnabledFlag);
            bitstream.WriteBit(b.VuiParametersPresentFlag);
            if (b.VuiParametersPresentFlag)
            {
                H265VuiParameters.Build(bitstream, b.VuiParameters, b.SpsMaxSubLayersMinus1);
            }

            bitstream.WriteBit(b.SpsExtensionPresentFlag);

            if(b.SpsExtensionPresentFlag)
            {
                throw new NotSupportedException("SPS Extensions not supported yet!");
            }
            
            bitstream.WriteTrailingBits();
        }

        public static H265SpsNalUnit Parse(byte[] sps)
        {
            using (MemoryStream ms = new MemoryStream(sps))
            {
                return Parse((ushort)sps.Length, ms);
            }
        }

        private static H265SpsNalUnit Parse(ushort size, MemoryStream stream)
        {
            NalBitStreamReader bitstream = new NalBitStreamReader(stream);
            H265NalUnitHeader header = H265NalUnitHeader.ParseNALHeader(bitstream);

            int spsVideoParameterSetId = bitstream.ReadBits(4);
            int spsMaxSubLayersMinus1 = bitstream.ReadBits(3);
            bool spsTemporalIdNestingFlag = bitstream.ReadBit() != 0;

            H265ProfileTier profileTier = H265ProfileTier.Parse(bitstream, true, spsMaxSubLayersMinus1);
                        
            int spsSeqParameterSetId = bitstream.ReadUE();
            int chromaFormatIdc = bitstream.ReadUE();
            bool separateColorPlaneFlag = false;
            if (chromaFormatIdc == 3)
            {
                separateColorPlaneFlag = bitstream.ReadBit() != 0;
            }
            int picWidthInLumaSamples = bitstream.ReadUE();
            int picHeightInLumaSamples = bitstream.ReadUE();
            bool conformanceWindowFlag = bitstream.ReadBit() != 0;

            int confWinLeftOffset = 0;
            int confWinRightOffset = 0;
            int confWinTopOffset = 0;
            int confWinBottomOffset = 0;
            if (conformanceWindowFlag)
            {
                confWinLeftOffset = bitstream.ReadUE();
                confWinRightOffset = bitstream.ReadUE();
                confWinTopOffset = bitstream.ReadUE();
                confWinBottomOffset = bitstream.ReadUE();
            }

            int bitDepthLumaMinus8 = bitstream.ReadUE();
            int bitDepthChromaMinus8 = bitstream.ReadUE();
            int log2MaxPicOrderCntLsbMinus4 = bitstream.ReadUE();
            bool spsSubLayerOrderingInfoPresentFlag = bitstream.ReadBit() != 0;

            int jCount = spsMaxSubLayersMinus1 - (spsSubLayerOrderingInfoPresentFlag ? 0 : spsMaxSubLayersMinus1) + 1;
            int[] spsMaxDecPicBufferingMinus1 = new int[jCount];
            int[] spsMaxNumReorderPics = new int[jCount];
            int[] spsMaxLatencyIncreasePlus1 = new int[jCount];

            for (int i = spsSubLayerOrderingInfoPresentFlag ? 0 : spsMaxSubLayersMinus1; i <= spsMaxSubLayersMinus1; i++)
            {
                spsMaxDecPicBufferingMinus1[i] = bitstream.ReadUE();
                spsMaxNumReorderPics[i] = bitstream.ReadUE();
                spsMaxLatencyIncreasePlus1[i] = bitstream.ReadUE();
            }

            int log2MinLumaCodingBlockSizeMinus3 = bitstream.ReadUE();
            int log2DiffMaxMinLumaCodingBlockSize = bitstream.ReadUE();
            int log2MinTransformBlockSizeMinus2 = bitstream.ReadUE();
            int log2DiffMaxMinTransformBlockSize = bitstream.ReadUE();
            int maxTransformHierarchyDepthInter = bitstream.ReadUE();
            int maxTransformHierarchyDepthIntra = bitstream.ReadUE();

            bool scalingListEnabledFlag = bitstream.ReadBit() != 0;
            bool spsScalingListDataPresentFlag = false;
            List<H265ScalingListElement> scalingListElements = null; 

            if (scalingListEnabledFlag)
            {
                spsScalingListDataPresentFlag = bitstream.ReadBit() != 0;
                if (spsScalingListDataPresentFlag)
                {
                    scalingListElements = H265ScalingListElement.Parse(bitstream);
                }
            }

            bool ampEnabledFlag = bitstream.ReadBit() != 0;
            bool sampleAdaptiveOffsetEnabledFlag = bitstream.ReadBit() != 0;
            bool pcmEnabledFlag = bitstream.ReadBit() != 0;

            int pcmSampleBitDepthLumaMinus1 = 0;
            int pcmSampleBitDepthChromaMinus1 = 0;
            int log2MinPcmLumaCodingBlockSizeMinus3 = 0;
            int log2DiffMaxMinPcmLumaCodingBlockSize = 0;
            bool pcmLoopFilterDisabledFlag = false;
            if (pcmEnabledFlag)
            {
                pcmSampleBitDepthLumaMinus1 = bitstream.ReadBits(4);
                pcmSampleBitDepthChromaMinus1 = bitstream.ReadBits(4);
                log2MinPcmLumaCodingBlockSizeMinus3 = bitstream.ReadUE();
                log2DiffMaxMinPcmLumaCodingBlockSize = bitstream.ReadUE();
                pcmLoopFilterDisabledFlag = bitstream.ReadBit() != 0;
            }

            int numShortTermRefPicSets = bitstream.ReadUE();

            if (numShortTermRefPicSets > 64)
                throw new Exception("Invalid SPS, num_short_term_ref_pic_sets cannot be larger than 64");

            long[] numDeltaPocs = new long[numShortTermRefPicSets];
            long[] deltaIdxMinus1 = new long[numShortTermRefPicSets];
            bool[] deltaRpsSign = new bool[numShortTermRefPicSets];
            int[] absDeltaRpsMinus1 = new int[numShortTermRefPicSets];
            bool[] interRefPicSetPredictionFlag = new bool[numShortTermRefPicSets];
            long[] numNegativePics = new long[numShortTermRefPicSets];
            long[] numPositivePics = new long[numShortTermRefPicSets];
            List<int>[] pics = new List<int>[numShortTermRefPicSets];
            for (int rpsIdx = 0; rpsIdx < numShortTermRefPicSets; rpsIdx++)
            {
                if (rpsIdx != 0)
                {
                    interRefPicSetPredictionFlag[rpsIdx] = bitstream.ReadBit() != 0;
                }

                pics[rpsIdx] = new List<int>();

                if (rpsIdx != 0 && interRefPicSetPredictionFlag[rpsIdx])
                {
                    if (rpsIdx == numShortTermRefPicSets)
                        deltaIdxMinus1[rpsIdx] = bitstream.ReadUE();
                    
                    deltaRpsSign[rpsIdx] = bitstream.ReadBit() != 0;
                    absDeltaRpsMinus1[rpsIdx] = bitstream.ReadUE();

                    int RefRpsIdx = rpsIdx - ((int)deltaIdxMinus1[rpsIdx] + 1);
                    for (int i = 0; i <= numDeltaPocs[RefRpsIdx]; i++)
                    {
                        int usedByCurrPicFlag = bitstream.ReadBit();
                        pics[rpsIdx].Add(usedByCurrPicFlag);

                        int useDeltaFlag = 0;
                        if (usedByCurrPicFlag == 0)
                        {
                            useDeltaFlag = bitstream.ReadBit();
                            pics[rpsIdx].Add(useDeltaFlag);
                        }

                        if (usedByCurrPicFlag != 0 || useDeltaFlag != 0)
                        {
                            numDeltaPocs[rpsIdx]++;
                        }
                    }
                }
                else
                {
                    numNegativePics[rpsIdx] = bitstream.ReadUE();
                    numPositivePics[rpsIdx] = bitstream.ReadUE();
                    long deltaPics = numNegativePics[rpsIdx] + numPositivePics[rpsIdx];

                    if (deltaPics < 0 || deltaPics > short.MaxValue)
                        throw new Exception("Sanity check for delta_pocs has failed!");

                    numDeltaPocs[rpsIdx] = deltaPics;

                    for (long i = 0; i < numNegativePics[rpsIdx]; ++i)
                    {
                        int deltaPocS0Minus1 = bitstream.ReadUE();
                        int usedByCurrPicS0Flag = bitstream.ReadBit();
                        pics[rpsIdx].Add(deltaPocS0Minus1);
                        pics[rpsIdx].Add(usedByCurrPicS0Flag);
                    }

                    for (long i = 0; i < numPositivePics[rpsIdx]; ++i)
                    {
                        int deltaPocS1Minus1 = bitstream.ReadUE();
                        int usedByCurrPicS1Flag = bitstream.ReadBit();
                        pics[rpsIdx].Add(deltaPocS1Minus1);
                        pics[rpsIdx].Add(usedByCurrPicS1Flag);
                    }
                }
            }

            bool longTermRefPicsPresentFlag = bitstream.ReadBit() != 0;
            int numLongTermRefPicsSps = 0;
            int[] ltRefPicPocLsbSps = null;
            bool[] usedByCurrPicLtSpsFlag = null;
            if (longTermRefPicsPresentFlag)
            {
                numLongTermRefPicsSps = bitstream.ReadUE();
                ltRefPicPocLsbSps = new int[numLongTermRefPicsSps];
                usedByCurrPicLtSpsFlag = new bool[numLongTermRefPicsSps];
                for (int i = 0; i < numLongTermRefPicsSps; i++)
                {
                    ltRefPicPocLsbSps[i] = bitstream.ReadBits(log2MaxPicOrderCntLsbMinus4 + 4);
                    usedByCurrPicLtSpsFlag[i] = bitstream.ReadBit() != 0;
                }
            }
            bool spsTemporalMvpEnabledFlag = bitstream.ReadBit() != 0;
            bool strongIntraSmoothingEnabledFlag = bitstream.ReadBit() != 0;
            bool vuiParametersPresentFlag = bitstream.ReadBit() != 0;
            H265VuiParameters vuiParameters = null;
            if (vuiParametersPresentFlag)
            {
                vuiParameters = H265VuiParameters.Parse(bitstream, spsMaxSubLayersMinus1);
            }

            bool spsExtensionPresentFlag = bitstream.ReadBit() != 0;
            if(spsExtensionPresentFlag)
            {
                throw new NotSupportedException("SPS extensions not supported yet!");
            }

            bitstream.ReadTrailingBits();

            return new H265SpsNalUnit(
                header,
                spsVideoParameterSetId,
                spsMaxSubLayersMinus1,
                spsTemporalIdNestingFlag,
                profileTier,
                spsSeqParameterSetId,
                chromaFormatIdc,
                separateColorPlaneFlag,
                picWidthInLumaSamples,
                picHeightInLumaSamples,
                conformanceWindowFlag,
                confWinLeftOffset,
                confWinRightOffset,
                confWinTopOffset,
                confWinBottomOffset,
                bitDepthLumaMinus8,
                bitDepthChromaMinus8,
                log2MaxPicOrderCntLsbMinus4,
                spsSubLayerOrderingInfoPresentFlag,
                spsMaxDecPicBufferingMinus1,
                spsMaxNumReorderPics,
                spsMaxLatencyIncreasePlus1,
                log2MinLumaCodingBlockSizeMinus3,
                log2DiffMaxMinLumaCodingBlockSize,
                log2MinTransformBlockSizeMinus2,
                log2DiffMaxMinTransformBlockSize,
                maxTransformHierarchyDepthInter,
                maxTransformHierarchyDepthIntra,
                scalingListEnabledFlag,
                spsScalingListDataPresentFlag,
                scalingListElements,
                ampEnabledFlag,
                sampleAdaptiveOffsetEnabledFlag,
                pcmEnabledFlag,
                pcmSampleBitDepthLumaMinus1,
                pcmSampleBitDepthChromaMinus1,
                log2MinPcmLumaCodingBlockSizeMinus3,
                log2DiffMaxMinPcmLumaCodingBlockSize,
                pcmLoopFilterDisabledFlag,
                numShortTermRefPicSets,
                numDeltaPocs,
                deltaRpsSign,
                absDeltaRpsMinus1,
                interRefPicSetPredictionFlag,
                numNegativePics,
                numPositivePics,
                pics,
                longTermRefPicsPresentFlag,
                numLongTermRefPicsSps,
                ltRefPicPocLsbSps,
                usedByCurrPicLtSpsFlag,
                spsTemporalMvpEnabledFlag,
                strongIntraSmoothingEnabledFlag,
                vuiParametersPresentFlag,
                vuiParameters,
                spsExtensionPresentFlag
            );
        }

        public (ushort Width, ushort Height) CalculateDimensions()
        {
            int width = this.PicWidthInLumaSamples;
            int height = this.PicHeightInLumaSamples;
            return ((ushort)width, (ushort)height);
        }

        public (int Timescale, int FrameTick) CalculateTimescale()
        {
            int timescale = 0;
            int frametick = 0;
            var vui = this.VuiParameters;
            if (vui != null && vui.VuiTimingInfoPresentFlag)
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
    }

    public class H265ScalingListElement
    {
        public bool ScalingListFlag { get; set; }
        public List<int> Values { get; set; } = new List<int>();

        public static List<H265ScalingListElement> Parse(NalBitStreamReader bitstream)
        {
            List<H265ScalingListElement> scalingListElements = new List<H265ScalingListElement>();
            for (int i = 0; i < 4; i++)
            {
                for (int k = 0; k < (i == 3 ? 2 : 6); k++)
                {
                    var element = new H265ScalingListElement();
                    element.ScalingListFlag = bitstream.ReadBit() != 0;
                    if (!element.ScalingListFlag)
                    {
                        element.Values.Add(bitstream.ReadUE());
                    }
                    else
                    {
                        int coef_num = Math.Min(64, 1 << (4 + (i << 1)));
                        if (i > 1)
                        {
                            element.Values.Add(bitstream.ReadUE());
                        }

                        for (int l = 0; l < coef_num; l++)
                        {
                            element.Values.Add(bitstream.ReadUE());
                        }
                    }
                    scalingListElements.Add(element);
                }
            }

            return scalingListElements;
        }

        public static void Build(NalBitStreamWriter bitstream, List<H265ScalingListElement> scalingListElements)
        {
            int index = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int k = 0; k < (i == 3 ? 2 : 6); k++)
                {
                    var element = scalingListElements[index++];
                    bitstream.WriteBit(element.ScalingListFlag);

                    int elindex = 0;
                    if (!element.ScalingListFlag)
                    {
                        bitstream.WriteUE((uint)element.Values[elindex++]);
                    }
                    else
                    {
                        int coef_num = Math.Min(64, 1 << (4 + (i << 1)));
                        if (i > 1)
                        {
                            bitstream.WriteUE((uint)element.Values[elindex++]);
                        }

                        for (int l = 0; l < coef_num; l++)
                        {
                            bitstream.WriteUE((uint)element.Values[elindex++]);
                        }
                    }
                    scalingListElements.Add(element);
                }
            }
        }
    }

    public class H265VuiParameters
    {
        public bool AspectRatioInfoPresentFlag { get; set; }
        public int AspectRatioIdc { get; set; }
        public int SarWidth { get; set; }
        public int SarHeight { get; set; }
        public bool OverscanInfoPresentFlag { get; set; }
        public bool OverscanAppropriateFlag { get; set; }
        public bool VideoSignalTypePresentFlag { get; set; }
        public int VideoFormat { get; set; }
        public bool VideoFullRangeFlag { get; set; }
        public bool ColorDescriptionPresentFlag { get; set; }
        public byte ColorPrimaries { get; set; }
        public byte TransferCharacteristics { get; set; }
        public byte MatrixCoeffs { get; set; }
        public bool ChromaLocInfoPresentFlag { get; set; }
        public int ChromaSampleLocTypeTopField { get; set; }
        public int ChromaSampleLocTypeBottomField { get; set; }
        public bool NeutralChromaIndicationFlag { get; set; }
        public bool FieldSeqFlag { get; set; }
        public bool FrameFieldInfoPresentFlag { get; set; }
        public bool DefaultDisplayWindowFlag { get; set; }
        public int DefDispWinLeftOffset { get; set; }
        public int DefDispWinRightOffset { get; set; }
        public int DefDispWinTopOffset { get; set; }
        public int DefDispWinBottomOffset { get; set; }
        public bool VuiTimingInfoPresentFlag { get; set; }
        public int VuiNumUnitsInTick { get; set; }
        public int VuiTimeScale { get; set; }
        public bool VuiPocProportionalToTimingFlag { get; set; }
        public int VuiNumTicksPocDiffOneMinus1 { get; set; }
        public bool VuiHrdParametersPresentFlag { get; set; }
        public H265HrdParameters VuiHrdParameters { get; set; }
        public bool BitstreamRestrictionFlag { get; set; }
        public bool TilesFixedStructureFlag { get; set; }
        public bool MotionVectorsOverPicBoundariesFlag { get; set; }
        public bool RestrictedRefPicListsFlag { get; set; }
        public int MinSpatialSegmentationIdc { get; set; }
        public int MaxBytesPerPicDenom { get; set; }
        public int MaxBitsPerMinCuDenom { get; set; }
        public int Log2MaxMvLengthHorizontal { get; set; }
        public int Log2MaxMvLengthVertical { get; set; }

        public H265VuiParameters(
            bool aspectRatioInfoPresentFlag, 
            int aspectRatioIdc, 
            int sarWidth, 
            int sarHeight, 
            bool overscanInfoPresentFlag, 
            bool overscanAppropriateFlag, 
            bool videoSignalTypePresentFlag, 
            int videoFormat, 
            bool videoFullRangeFlag, 
            bool colorDescriptionPresentFlag, 
            byte colorPrimaries, 
            byte transferCharacteristics, 
            byte matrixCoeffs, 
            bool chromaLocInfoPresentFlag, 
            int chromaSampleLocTypeTopField, 
            int chromaSampleLocTypeBottomField, 
            bool neutralChromaIndicationFlag, 
            bool fieldSeqFlag, 
            bool frameFieldInfoPresentFlag, 
            bool defaultDisplayWindowFlag, 
            int defDispWinLeftOffset, 
            int defDispWinRightOffset, 
            int defDispWinTopOffset, 
            int defDispWinBottomOffset, 
            bool vuiTimingInfoPresentFlag, 
            int vuiNumUnitsInTick, 
            int vuiTimeScale, 
            bool vuiPocProportionalToTimingFlag, 
            int vuiNumTicksPocDiffOneMinus1, 
            bool vuiHrdParametersPresentFlag, 
            H265HrdParameters vuiHrdParameters, 
            bool bitstreamRestrictionFlag, 
            bool tilesFixedStructureFlag, 
            bool motionVectorsOverPicBoundariesFlag, 
            bool restrictedRefPicListsFlag, 
            int minSpatialSegmentationIdc, 
            int maxBytesPerPicDenom, 
            int maxBitsPerMinCuDenom, 
            int log2MaxMvLengthHorizontal,
            int log2MaxMvLengthVertical)
        {
            AspectRatioInfoPresentFlag = aspectRatioInfoPresentFlag;
            AspectRatioIdc = aspectRatioIdc;
            SarWidth = sarWidth;
            SarHeight = sarHeight;
            OverscanInfoPresentFlag = overscanInfoPresentFlag;
            OverscanAppropriateFlag = overscanAppropriateFlag;
            VideoSignalTypePresentFlag = videoSignalTypePresentFlag;
            VideoFormat = videoFormat;
            VideoFullRangeFlag = videoFullRangeFlag;
            ColorDescriptionPresentFlag = colorDescriptionPresentFlag;
            ColorPrimaries = colorPrimaries;
            TransferCharacteristics = transferCharacteristics;
            MatrixCoeffs = matrixCoeffs;
            ChromaLocInfoPresentFlag = chromaLocInfoPresentFlag;
            ChromaSampleLocTypeTopField = chromaSampleLocTypeTopField;
            ChromaSampleLocTypeBottomField = chromaSampleLocTypeBottomField;
            NeutralChromaIndicationFlag = neutralChromaIndicationFlag;
            FieldSeqFlag = fieldSeqFlag;
            FrameFieldInfoPresentFlag = frameFieldInfoPresentFlag;
            DefaultDisplayWindowFlag = defaultDisplayWindowFlag;
            DefDispWinLeftOffset = defDispWinLeftOffset;
            DefDispWinRightOffset = defDispWinRightOffset;
            DefDispWinTopOffset = defDispWinTopOffset;
            DefDispWinBottomOffset = defDispWinBottomOffset;
            VuiTimingInfoPresentFlag = vuiTimingInfoPresentFlag;
            VuiNumUnitsInTick = vuiNumUnitsInTick;
            VuiTimeScale = vuiTimeScale;
            VuiPocProportionalToTimingFlag = vuiPocProportionalToTimingFlag;
            VuiNumTicksPocDiffOneMinus1 = vuiNumTicksPocDiffOneMinus1;
            VuiHrdParametersPresentFlag = vuiHrdParametersPresentFlag;
            VuiHrdParameters = vuiHrdParameters;
            BitstreamRestrictionFlag = bitstreamRestrictionFlag;
            TilesFixedStructureFlag = tilesFixedStructureFlag;
            MotionVectorsOverPicBoundariesFlag = motionVectorsOverPicBoundariesFlag;
            RestrictedRefPicListsFlag = restrictedRefPicListsFlag;
            MinSpatialSegmentationIdc = minSpatialSegmentationIdc;
            MaxBytesPerPicDenom = maxBytesPerPicDenom;
            MaxBitsPerMinCuDenom = maxBitsPerMinCuDenom;
            Log2MaxMvLengthHorizontal = log2MaxMvLengthHorizontal;
            Log2MaxMvLengthVertical = log2MaxMvLengthVertical;
        }

        public static H265VuiParameters Parse(NalBitStreamReader bitstream, int spsMaxSubLayersMinus1)
        {
            bool aspectRatioInfoPresentFlag = bitstream.ReadBit() != 0;
            int aspectRatioIdc = 0;
            int sarWidth = 0;
            int sarHeight = 0;
            if (aspectRatioInfoPresentFlag)
            {
                aspectRatioIdc = bitstream.ReadBits(8);
                if (aspectRatioIdc == 255) // EXTENDED_SAR
                {
                    sarWidth = bitstream.ReadBits(16);
                    sarHeight = bitstream.ReadBits(16);
                }
            }
            bool overscanInfoPresentFlag = bitstream.ReadBit() != 0;
            bool overscanAppropriateFlag = false;
            if (overscanInfoPresentFlag)
            {
                overscanAppropriateFlag = bitstream.ReadBit() != 0;
            }
            bool videoSignalTypePresentFlag = bitstream.ReadBit() != 0;

            int videoFormat = 0;
            bool videoFullRangeFlag = false;
            bool colorDescriptionPresentFlag = false;
            byte colorPrimaries = 0;
            byte transferCharacteristics = 0;
            byte matrixCoeffs = 0;
            if (videoSignalTypePresentFlag)
            {
                videoFormat = bitstream.ReadBits(3);
                videoFullRangeFlag = bitstream.ReadBit() != 0;
                colorDescriptionPresentFlag = bitstream.ReadBit() != 0;
                if (colorDescriptionPresentFlag)
                {
                    colorPrimaries = (byte)bitstream.ReadBits(8);
                    transferCharacteristics = (byte)bitstream.ReadBits(8);
                    matrixCoeffs = (byte)bitstream.ReadBits(8);
                }
            }
            bool chromaLocInfoPresentFlag = bitstream.ReadBit() != 0;
            int chromaSampleLocTypeTopField = 0;
            int chromaSampleLocTypeBottomField = 0;
            if (chromaLocInfoPresentFlag)
            {
                chromaSampleLocTypeTopField = bitstream.ReadUE();
                chromaSampleLocTypeBottomField = bitstream.ReadUE();
            }
            bool neutralChromaIndicationFlag = bitstream.ReadBit() != 0;
            bool fieldSeqFlag = bitstream.ReadBit() != 0;
            bool frameFieldInfoPresentFlag = bitstream.ReadBit() != 0;
            bool defaultDisplayWindowFlag = bitstream.ReadBit() != 0;
            int defDispWinLeftOffset = 0;
            int defDispWinRightOffset = 0;
            int defDispWinTopOffset = 0;
            int defDispWinBottomOffset = 0;
            if (defaultDisplayWindowFlag)
            {
                defDispWinLeftOffset = bitstream.ReadUE();
                defDispWinRightOffset = bitstream.ReadUE();
                defDispWinTopOffset = bitstream.ReadUE();
                defDispWinBottomOffset = bitstream.ReadUE();
            }
            bool vuiTimingInfoPresentFlag = bitstream.ReadBit() != 0;

            int vuiNumUnitsInTick = 0;
            int vuiTimeScale = 0;
            bool vuiPocProportionalToTimingFlag = false;
            int vuiNumTicksPocDiffOneMinus1 = 0;
            bool vuiHrdParametersPresentFlag = false;
            H265HrdParameters vuiHrdParameters = null;
            if (vuiTimingInfoPresentFlag)
            {
                vuiNumUnitsInTick = bitstream.ReadBits(32);
                vuiTimeScale = bitstream.ReadBits(32);
                vuiPocProportionalToTimingFlag = bitstream.ReadBit() != 0;
                if (vuiPocProportionalToTimingFlag)
                {
                    vuiNumTicksPocDiffOneMinus1 = bitstream.ReadUE();
                }
                vuiHrdParametersPresentFlag = bitstream.ReadBit() != 0;
                if (vuiHrdParametersPresentFlag)
                {
                    vuiHrdParameters = H265HrdParameters.Parse(bitstream, true, spsMaxSubLayersMinus1);
                }
            }
            bool bitstreamRestrictionFlag = bitstream.ReadBit() != 0;

            bool tilesFixedStructureFlag = false;
            bool motionVectorsOverPicBoundariesFlag = false;
            bool restrictedRefPicListsFlag = false;
            int minSpatialSegmentationIdc = 0;
            int maxBytesPerPicDenom = 0;
            int maxBitsPerMinCuDenom = 0;
            int log2MaxMvLengthHorizontal = 0;
            int log2MaxMvLengthVertical = 0;
            if (bitstreamRestrictionFlag)
            {
                tilesFixedStructureFlag = bitstream.ReadBit() != 0;
                motionVectorsOverPicBoundariesFlag = bitstream.ReadBit() != 0;
                restrictedRefPicListsFlag = bitstream.ReadBit() != 0;
                minSpatialSegmentationIdc = bitstream.ReadUE();
                maxBytesPerPicDenom = bitstream.ReadUE();
                maxBitsPerMinCuDenom = bitstream.ReadUE();
                log2MaxMvLengthHorizontal = bitstream.ReadUE();
                log2MaxMvLengthVertical = bitstream.ReadUE();
            }

            return new H265VuiParameters(
                aspectRatioInfoPresentFlag,
                aspectRatioIdc,
                sarWidth,
                sarHeight,
                overscanInfoPresentFlag,
                overscanAppropriateFlag,
                videoSignalTypePresentFlag,
                videoFormat,
                videoFullRangeFlag,
                colorDescriptionPresentFlag,
                colorPrimaries,
                transferCharacteristics,
                matrixCoeffs,
                chromaLocInfoPresentFlag,
                chromaSampleLocTypeTopField,
                chromaSampleLocTypeBottomField,
                neutralChromaIndicationFlag,
                fieldSeqFlag,
                frameFieldInfoPresentFlag,
                defaultDisplayWindowFlag,
                defDispWinLeftOffset,
                defDispWinRightOffset,
                defDispWinTopOffset,
                defDispWinBottomOffset,
                vuiTimingInfoPresentFlag,
                vuiNumUnitsInTick,
                vuiTimeScale,
                vuiPocProportionalToTimingFlag,
                vuiNumTicksPocDiffOneMinus1,
                vuiHrdParametersPresentFlag,
                vuiHrdParameters,
                bitstreamRestrictionFlag,
                tilesFixedStructureFlag,
                motionVectorsOverPicBoundariesFlag,
                restrictedRefPicListsFlag,
                minSpatialSegmentationIdc,
                maxBytesPerPicDenom,
                maxBitsPerMinCuDenom,
                log2MaxMvLengthHorizontal,
                log2MaxMvLengthVertical
            );
        }

        public static void Build(NalBitStreamWriter bitstream, H265VuiParameters b, int spsMaxSubLayersMinus1)
        {
            bitstream.WriteBit(b.AspectRatioInfoPresentFlag);
            if (b.AspectRatioInfoPresentFlag)
            {
                bitstream.WriteBits(8, b.AspectRatioIdc);
                if (b.AspectRatioIdc == 255) // EXTENDED_SAR
                {
                    bitstream.WriteBits(16, b.SarWidth);
                    bitstream.WriteBits(16, b.SarHeight);
                }
            }
            bitstream.WriteBit(b.OverscanInfoPresentFlag);
            if (b.OverscanInfoPresentFlag)
            {
                bitstream.WriteBit(b.OverscanAppropriateFlag);
            }
            bitstream.WriteBit(b.VideoSignalTypePresentFlag);

            if (b.VideoSignalTypePresentFlag)
            {
                bitstream.WriteBits(3, b.VideoFormat);
                bitstream.WriteBit(b.VideoFullRangeFlag);
                bitstream.WriteBit(b.ColorDescriptionPresentFlag);
                if (b.ColorDescriptionPresentFlag)
                {
                    bitstream.WriteBits(8, b.ColorPrimaries);
                    bitstream.WriteBits(8, b.TransferCharacteristics);
                    bitstream.WriteBits(8, b.MatrixCoeffs);
                }
            }
            bitstream.WriteBit(b.ChromaLocInfoPresentFlag);
            if (b.ChromaLocInfoPresentFlag)
            {
                bitstream.WriteUE((uint)b.ChromaSampleLocTypeTopField);
                bitstream.WriteUE((uint)b.ChromaSampleLocTypeBottomField);
            }
            bitstream.WriteBit(b.NeutralChromaIndicationFlag);
            bitstream.WriteBit(b.FieldSeqFlag);
            bitstream.WriteBit(b.FrameFieldInfoPresentFlag);
            bitstream.WriteBit(b.DefaultDisplayWindowFlag);
            if (b.DefaultDisplayWindowFlag)
            {
                bitstream.WriteUE((uint)b.DefDispWinLeftOffset);
                bitstream.WriteUE((uint)b.DefDispWinRightOffset);
                bitstream.WriteUE((uint)b.DefDispWinTopOffset);
                bitstream.WriteUE((uint)b.DefDispWinBottomOffset);
            }
            bitstream.WriteBit(b.VuiTimingInfoPresentFlag);

            if (b.VuiTimingInfoPresentFlag)
            {
                bitstream.WriteBits(32, b.VuiNumUnitsInTick);
                bitstream.WriteBits(32, b.VuiTimeScale);
                bitstream.WriteBit(b.VuiPocProportionalToTimingFlag);
                if (b.VuiPocProportionalToTimingFlag)
                {
                    bitstream.WriteUE((uint)b.VuiNumTicksPocDiffOneMinus1);
                }
                bitstream.WriteBit(b.VuiHrdParametersPresentFlag);
                if (b.VuiHrdParametersPresentFlag)
                {
                    H265HrdParameters.Build(bitstream, b.VuiHrdParameters, true, spsMaxSubLayersMinus1);
                }
            }
            bitstream.WriteBit(b.BitstreamRestrictionFlag);

            if (b.BitstreamRestrictionFlag)
            {
                bitstream.WriteBit(b.TilesFixedStructureFlag);
                bitstream.WriteBit(b.MotionVectorsOverPicBoundariesFlag);
                bitstream.WriteBit(b.RestrictedRefPicListsFlag);
                bitstream.WriteUE((uint)b.MinSpatialSegmentationIdc);
                bitstream.WriteUE((uint)b.MaxBytesPerPicDenom);
                bitstream.WriteUE((uint)b.MaxBitsPerMinCuDenom);
                bitstream.WriteUE((uint)b.Log2MaxMvLengthHorizontal);
                bitstream.WriteUE((uint)b.Log2MaxMvLengthVertical);
            }
        }
    }

    public class H265HrdParameters
    {
        public bool NalHrdParametersPresentFlag { get; set; }
        public bool VclHrdParametersPresentFlag { get; set; }
        public bool SubPicHrdParamsPresentFlag { get; set; }
        public int TickDivisorMinus2 { get; set; }
        public int DuCpbRemovalDelayIncrementLengthMinus1 { get; set; }
        public bool SubPicCpbParamsInPicTimingSeiFlag { get; set; }
        public int DpbOutputDelayDuLengthMinus1 { get; set; }
        public int BitRateScale { get; set; }
        public int CpbSizeScale { get; set; }
        public int CpbSizeDuScale { get; set; }
        public int InitialCpbRemovalDelayLengthMinus1 { get; set; }
        public int AuCpbRemovalDelayLengthMinus1 { get; set; }
        public int DpbOutputDelayLengthMinus1 { get; set; }
        public bool[] FixedPicRateGeneralFlag { get; set; }
        public bool[] FixedPicRateWithinCvsFlag { get; set; }
        public bool[] LowDelayHrdFlag { get; set; }
        public int[] CpbCntMinus1 { get; set; }
        public int[] ElementalDurationInTcMinus1 { get; set; }
        public H265HrdSubLayerParameters[] NalHrdSubLayerParameters { get; set; }
        public H265HrdSubLayerParameters[] VclHrdSubLayerParameters { get; set; }

        public H265HrdParameters(
            bool nalHrdParametersPresentFlag,
            bool vclHrdParametersPresentFlag,
            bool subPicHrdParamsPresentFlag,
            int tickDivisorMinus2, 
            int duCpbRemovalDelayIncrementLengthMinus1, 
            bool subPicCpbParamsInPicTimingSeiFlag, 
            int dpbOutputDelayDuLengthMinus1, 
            int bitRateScale, 
            int cpbSizeScale, 
            int cpbSizeDuScale,
            int initialCpbRemovalDelayLengthMinus1, 
            int auCpbRemovalDelayLengthMinus1, 
            int dpbOutputDelayLengthMinus1,
            bool[] fixedPicRateGeneralFlag, 
            bool[] fixedPicRateWithinCvsFlag,
            bool[] lowDelayHrdFlag, 
            int[] cpbCntMinus1,
            int[] elementalDurationInTcMinus1,
            H265HrdSubLayerParameters[] nalHrdSubLayerParameters,
            H265HrdSubLayerParameters[] vclHrdSubLayerParameters)
        {
            NalHrdParametersPresentFlag = nalHrdParametersPresentFlag;
            VclHrdParametersPresentFlag = vclHrdParametersPresentFlag;
            SubPicHrdParamsPresentFlag = subPicHrdParamsPresentFlag;
            TickDivisorMinus2 = tickDivisorMinus2;
            DuCpbRemovalDelayIncrementLengthMinus1 = duCpbRemovalDelayIncrementLengthMinus1;
            SubPicCpbParamsInPicTimingSeiFlag = subPicCpbParamsInPicTimingSeiFlag;
            DpbOutputDelayDuLengthMinus1 = dpbOutputDelayDuLengthMinus1;
            BitRateScale = bitRateScale;
            CpbSizeScale = cpbSizeScale;
            CpbSizeDuScale = cpbSizeDuScale;
            InitialCpbRemovalDelayLengthMinus1 = initialCpbRemovalDelayLengthMinus1;
            AuCpbRemovalDelayLengthMinus1 = auCpbRemovalDelayLengthMinus1;
            DpbOutputDelayLengthMinus1 = dpbOutputDelayLengthMinus1;
            FixedPicRateGeneralFlag = fixedPicRateGeneralFlag;
            FixedPicRateWithinCvsFlag = fixedPicRateWithinCvsFlag;
            LowDelayHrdFlag = lowDelayHrdFlag;
            CpbCntMinus1 = cpbCntMinus1;
            ElementalDurationInTcMinus1 = elementalDurationInTcMinus1;
            NalHrdSubLayerParameters = nalHrdSubLayerParameters;
            VclHrdSubLayerParameters = vclHrdSubLayerParameters;
        }

        public static H265HrdParameters Parse(NalBitStreamReader bitstream, bool commonInfPresentFlag, int spsMaxSubLayersMinus1)
        {
            bool nalHrdParametersPresentFlag = false;
            bool vclHrdParametersPresentFlag = false;
            bool subPicHrdParamsPresentFlag = false;

            int tickDivisorMinus2 = -2;
            int duCpbRemovalDelayIncrementLengthMinus1 = -1;
            bool subPicCpbParamsInPicTimingSeiFlag = false;
            int dpbOutputDelayDuLengthMinus1 = -1;
            int bitRateScale = 0;
            int cpbSizeScale = 0;
            int cpbSizeDuScale = 0;
            int initialCpbRemovalDelayLengthMinus1 = -1;
            int auCpbRemovalDelayLengthMinus1 = -1;
            int dpbOutputDelayLengthMinus1 = -1;
            if (commonInfPresentFlag)
            {
                nalHrdParametersPresentFlag = bitstream.ReadBit() != 0;
                vclHrdParametersPresentFlag = bitstream.ReadBit() != 0;
                if (nalHrdParametersPresentFlag || vclHrdParametersPresentFlag)
                {
                    subPicHrdParamsPresentFlag = bitstream.ReadBit() != 0;
                    if (subPicHrdParamsPresentFlag)
                    {
                        tickDivisorMinus2 = bitstream.ReadBits(8);
                        duCpbRemovalDelayIncrementLengthMinus1 = bitstream.ReadBits(5);
                        subPicCpbParamsInPicTimingSeiFlag = bitstream.ReadBit() != 0;
                        dpbOutputDelayDuLengthMinus1 = bitstream.ReadBits(5);
                    }
                    bitRateScale = bitstream.ReadBits(4);
                    cpbSizeScale = bitstream.ReadBits(4);
                    if (subPicHrdParamsPresentFlag)
                    {
                        cpbSizeDuScale = bitstream.ReadBits(4);
                    }
                    initialCpbRemovalDelayLengthMinus1 = bitstream.ReadBits(5);
                    auCpbRemovalDelayLengthMinus1 = bitstream.ReadBits(5);
                    dpbOutputDelayLengthMinus1 = bitstream.ReadBits(5);
                }
            }

            bool[] fixedPicRateGeneralFlag = new bool[spsMaxSubLayersMinus1 + 1];
            bool[] fixedPicRateWithinCvsFlag = new bool[spsMaxSubLayersMinus1 + 1];
            bool[] lowDelayHrdFlag = new bool[spsMaxSubLayersMinus1 + 1];
            int[] cpbCntMinus1 = new int[spsMaxSubLayersMinus1 + 1];
            int[] elementalDurationInTcMinus1 = new int[spsMaxSubLayersMinus1 + 1];
            H265HrdSubLayerParameters[] nalHrdSubLayerParameters = new H265HrdSubLayerParameters[spsMaxSubLayersMinus1 + 1];
            H265HrdSubLayerParameters[] vclHrdSubLayerParameters = new H265HrdSubLayerParameters[spsMaxSubLayersMinus1 + 1];
            for (int i = 0; i <= spsMaxSubLayersMinus1; i++)
            {
                fixedPicRateGeneralFlag[i] = bitstream.ReadBit() != 0;
                if (!fixedPicRateGeneralFlag[i])
                {
                    fixedPicRateWithinCvsFlag[i] = bitstream.ReadBit() != 0;
                }
                if (fixedPicRateWithinCvsFlag[i])
                {
                    elementalDurationInTcMinus1[i] = bitstream.ReadUE();
                }
                else
                {
                    lowDelayHrdFlag[i] = bitstream.ReadBit() != 0;
                }
                if (!lowDelayHrdFlag[i])
                {
                    cpbCntMinus1[i] = bitstream.ReadUE();
                }
                if (nalHrdParametersPresentFlag)
                {
                    nalHrdSubLayerParameters[i] = H265HrdSubLayerParameters.Parse(bitstream, cpbCntMinus1[i], subPicHrdParamsPresentFlag);
                }
                if (vclHrdParametersPresentFlag)
                {
                    vclHrdSubLayerParameters[i] = H265HrdSubLayerParameters.Parse(bitstream, cpbCntMinus1[i], subPicHrdParamsPresentFlag);
                }
            }

            return new H265HrdParameters(
                nalHrdParametersPresentFlag,
                vclHrdParametersPresentFlag,
                subPicHrdParamsPresentFlag,
                tickDivisorMinus2,
                duCpbRemovalDelayIncrementLengthMinus1,
                subPicCpbParamsInPicTimingSeiFlag,
                dpbOutputDelayDuLengthMinus1 ,
                bitRateScale,
                cpbSizeScale,
                cpbSizeDuScale,
                initialCpbRemovalDelayLengthMinus1,
                auCpbRemovalDelayLengthMinus1,
                dpbOutputDelayLengthMinus1,
                fixedPicRateGeneralFlag,
                fixedPicRateWithinCvsFlag,
                lowDelayHrdFlag,
                cpbCntMinus1,
                elementalDurationInTcMinus1,
                nalHrdSubLayerParameters,
                vclHrdSubLayerParameters
                );
        }

        public static void Build(NalBitStreamWriter bitstream, H265HrdParameters b, bool commonInfPresentFlag, int spsMaxSubLayersMinus1)
        {
            if (commonInfPresentFlag)
            {
                bitstream.WriteBit(b.NalHrdParametersPresentFlag);
                bitstream.WriteBit(b.VclHrdParametersPresentFlag);
                if (b.NalHrdParametersPresentFlag || b.VclHrdParametersPresentFlag)
                {
                    bitstream.WriteBit(b.SubPicHrdParamsPresentFlag);
                    if (b.SubPicHrdParamsPresentFlag)
                    {
                        bitstream.WriteBits(8, b.TickDivisorMinus2);
                        bitstream.WriteBits(5, b.DuCpbRemovalDelayIncrementLengthMinus1);
                        bitstream.WriteBit(b.SubPicCpbParamsInPicTimingSeiFlag);
                        bitstream.WriteBits(5, b.DpbOutputDelayDuLengthMinus1);
                    }
                    bitstream.WriteBits(4, b.BitRateScale);
                    bitstream.WriteBits(4, b.CpbSizeScale);
                    if (b.SubPicHrdParamsPresentFlag)
                    {
                        bitstream.WriteBits(4, b.CpbSizeDuScale);
                    }
                    bitstream.WriteBits(5, b.InitialCpbRemovalDelayLengthMinus1);
                    bitstream.WriteBits(5, b.AuCpbRemovalDelayLengthMinus1);
                    bitstream.WriteBits(5, b.DpbOutputDelayLengthMinus1);
                }
            }

            for (int i = 0; i <= spsMaxSubLayersMinus1; i++)
            {
                bitstream.WriteBit(b.FixedPicRateGeneralFlag[i]);
                if (!b.FixedPicRateGeneralFlag[i])
                {
                    bitstream.WriteBit(b.FixedPicRateWithinCvsFlag[i]);
                }
                if (b.FixedPicRateWithinCvsFlag[i])
                {
                    bitstream.WriteUE((uint)b.ElementalDurationInTcMinus1[i]);
                }
                else
                {
                    bitstream.WriteBit(b.LowDelayHrdFlag[i]);
                }
                if (!b.LowDelayHrdFlag[i])
                {
                    bitstream.WriteUE((uint)b.CpbCntMinus1[i]);
                }
                if (b.NalHrdParametersPresentFlag)
                {
                    H265HrdSubLayerParameters.Build(bitstream, b.NalHrdSubLayerParameters[i], b.CpbCntMinus1[i], b.SubPicHrdParamsPresentFlag);
                }
                if (b.VclHrdParametersPresentFlag)
                {
                    H265HrdSubLayerParameters.Build(bitstream, b.VclHrdSubLayerParameters[i], b.CpbCntMinus1[i], b.SubPicHrdParamsPresentFlag);
                }
            }
        }
    }

    public class H265HrdSubLayerParameters
    {
        public H265HrdSubLayerParameters(int[] bitRateValueMinus1, int[] cpbSizeValueMinus1, int[] cpbSizeDuValueMinus1, int[] bitRateDuValueMinus1, bool[] cbrFlag)
        {
            BitRateValueMinus1 = bitRateValueMinus1;
            CpbSizeValueMinus1 = cpbSizeValueMinus1;
            CpbSizeDuValueMinus1 = cpbSizeDuValueMinus1;
            BitRateDuValueMinus1 = bitRateDuValueMinus1;
            CbrFlag = cbrFlag;
        }

        public int[] BitRateValueMinus1 { get; set; }
        public int[] CpbSizeValueMinus1 { get; set; }
        public int[] CpbSizeDuValueMinus1 { get; set; }
        public int[] BitRateDuValueMinus1 { get; set; }
        public bool[] CbrFlag { get; set; }

        public static H265HrdSubLayerParameters Parse(NalBitStreamReader bitstream, int cpbCntMinus1, bool subPicHrdParamsPresentFlag)
        {
            int[] bitRateValueMinus1 = new int[cpbCntMinus1 + 1];
            int[] cpbSizeValueMinus1 = new int[cpbCntMinus1 + 1];
            int[] cpbSizeDuValueMinus1 = new int[cpbCntMinus1 + 1];
            int[] bitRateDuValueMinus1 = new int[cpbCntMinus1 + 1];
            bool[] cbrFlag = new bool[cpbCntMinus1 + 1];

            for (int i = 0; i <= cpbCntMinus1; i++)
            {
                bitRateValueMinus1[i] = bitstream.ReadUE();
                cpbSizeValueMinus1[i] = bitstream.ReadUE();
                if (subPicHrdParamsPresentFlag)
                {
                    cpbSizeDuValueMinus1[i] = bitstream.ReadUE();
                    bitRateDuValueMinus1[i] = bitstream.ReadUE();
                }
                cbrFlag[i] = bitstream.ReadBit() != 0;
            }

            return new H265HrdSubLayerParameters(
                bitRateValueMinus1,
                cpbSizeValueMinus1,
                cpbSizeDuValueMinus1,
                bitRateDuValueMinus1,
                cbrFlag
            );
        }

        public static void Build(NalBitStreamWriter bitstream, H265HrdSubLayerParameters b, int cpbCntMinus1, bool subPicHrdParamsPresentFlag)
        {
            for (int i = 0; i <= cpbCntMinus1; i++)
            {
                bitstream.WriteUE((uint)b.BitRateValueMinus1[i]);
                bitstream.WriteUE((uint)b.CpbSizeValueMinus1[i]);
                if (subPicHrdParamsPresentFlag)
                {
                    bitstream.WriteUE((uint)b.CpbSizeDuValueMinus1[i]);
                    bitstream.WriteUE((uint)b.BitRateDuValueMinus1[i]);
                }
                bitstream.WriteBit(b.CbrFlag[i]);
            }
        }
    }

    public class H265PpsNalUnit
    {
        public H265NalUnitHeader Header { get; set; }
        public int PicParameterSetId { get; set; }
        public int PpsPicParameterSetId { get; set; }
        public int PpsSeqParameterSetId { get; set; }
        public bool DependentSliceSegmentsEnabledFlag { get; set; }
        public bool OutputFlagPresentFlag { get; set; }
        public int NumExtraSliceHeaderBits { get; set; }
        public bool SignDataHidingEnabledFlag { get; set; }
        public bool CabacInitPresentFlag { get; set; }
        public int NumRefIdxL0DefaultActiveMinus1 { get; set; }
        public int NumRefIdxL1DefaultActiveMinus1 { get; set; }
        public int InitQpMinus26 { get; set; }
        public bool ConstrainedIntraPredFlag { get; set; }
        public bool TransformSkipEnabledFlag { get; set; }
        public bool CuQpDeltaEnabledFlag { get; set; }
        public int DiffCuQpDeltaDepth { get; set; }
        public int PpsCbQpOffset { get; set; }
        public int PpsCrQpOffset { get; set; }
        public bool PpsSliceChromaQpOffsetsPresentFlag { get; set; }
        public bool WeightedPredFlag { get; set; }
        public bool WeightedBipredFlag { get; set; }
        public bool TransquantBypassEnabledFlag { get; set; }
        public bool TilesEnabledFlag { get; set; }
        public bool EntropyCodingSyncEnabledFlag { get; set; }
        public List<int> ColumnWidthMinus1 { get; set; }
        public List<int> RowHeightMinus1 { get; set; }
        public int NumTileColumnsMinus1 { get; set; }
        public int NumTileRowsMinus1 { get; set; }
        public bool UniformSpacingFlag { get; set; }
        public bool LoopFilterAcrossTilesEnabledFlag { get; set; }
        public bool PpsLoopFilterAcrossSlicesEnabledFlag { get; set; }
        public bool DeblockingFilterControlPresentFlag { get; set; }
        public bool DeblockingFilterOverrideEnabledFlag { get; set; }
        public bool PpsDeblockingFilterDisabledFlag { get; set; }
        public int PpsBetaOffsetDiv2 { get; set; }
        public int PpsTcOffsetDiv2 { get; set; }
        public bool PpsScalingListDataPresentFlag { get; set; }
        public List<H265ScalingListElement> PpsScalingList { get; set; }
        public bool ListsModificationPresentFlag { get; set; }
        public int Log2ParallelMergeLevelMinus2 { get; set; }
        public bool SliceSegmentHeaderExtensionPresentFlag { get; set; }
        public bool PpsExtensionPresentFlag { get; set; }
        public bool PpsRangeExtensionFlag { get; set; }
        public bool PpsMultilayerExtensionFlag { get; set; }
        public bool Pps3dExtensionFlag { get; set; }
        public bool PpsSccExtensionFlag { get; set; }
        public int PpsExtension4Bits { get; set; }
        public List<bool> PpsExtension4BitsData { get; set; }

        public H265PpsNalUnit(
            H265NalUnitHeader header,
            int ppsPicParameterSetId,
            int ppsSeqParameterSetId,
            bool dependentSliceSegmentsEnabledFlag, 
            bool outputFlagPresentFlag,
            int numExtraSliceHeaderBits, 
            bool signDataHidingEnabledFlag,
            bool cabacInitPresentFlag, 
            int numRefIdxL0DefaultActiveMinus1,
            int numRefIdxL1DefaultActiveMinus1, 
            int initQpMinus26, 
            bool constrainedIntraPredFlag, 
            bool transformSkipEnabledFlag,
            bool cuQpDeltaEnabledFlag, 
            int diffCuQpDeltaDepth, 
            int ppsCbQpOffset, 
            int ppsCrQpOffset, 
            bool ppsSliceChromaQpOffsetsPresentFlag,
            bool weightedPredFlag, 
            bool weightedBipredFlag, 
            bool transquantBypassEnabledFlag, 
            bool tilesEnabledFlag, 
            bool entropyCodingSyncEnabledFlag, 
            List<int> columnWidthMinus1,
            List<int> rowHeightMinus1, 
            int numTileColumnsMinus1, 
            int numTileRowsMinus1, 
            bool uniformSpacingFlag, 
            bool loopFilterAcrossTilesEnabledFlag, 
            bool ppsLoopFilterAcrossSlicesEnabledFlag, 
            bool deblockingFilterControlPresentFlag, 
            bool deblockingFilterOverrideEnabledFlag, 
            bool ppsDeblockingFilterDisabledFlag,
            int ppsBetaOffsetDiv2,
            int ppsTcOffsetDiv2, 
            bool ppsScalingListDataPresentFlag, 
            List<H265ScalingListElement> ppsScalingList,
            bool listsModificationPresentFlag, 
            int log2ParallelMergeLevelMinus2, 
            bool sliceSegmentHeaderExtensionPresentFlag, 
            bool ppsExtensionPresentFlag, 
            bool ppsRangeExtensionFlag, 
            bool ppsMultilayerExtensionFlag, 
            bool pps3dExtensionFlag,
            bool ppsSccExtensionFlag, 
            int ppsExtension4Bits, 
            List<bool> ppsExtension4BitsData)
        {
            Header = header;
            PpsPicParameterSetId = ppsPicParameterSetId;
            PpsSeqParameterSetId = ppsSeqParameterSetId;
            DependentSliceSegmentsEnabledFlag = dependentSliceSegmentsEnabledFlag;
            OutputFlagPresentFlag = outputFlagPresentFlag;
            NumExtraSliceHeaderBits = numExtraSliceHeaderBits;
            SignDataHidingEnabledFlag = signDataHidingEnabledFlag;
            CabacInitPresentFlag = cabacInitPresentFlag;
            NumRefIdxL0DefaultActiveMinus1 = numRefIdxL0DefaultActiveMinus1;
            NumRefIdxL1DefaultActiveMinus1 = numRefIdxL1DefaultActiveMinus1;
            InitQpMinus26 = initQpMinus26;
            ConstrainedIntraPredFlag = constrainedIntraPredFlag;
            TransformSkipEnabledFlag = transformSkipEnabledFlag;
            CuQpDeltaEnabledFlag = cuQpDeltaEnabledFlag;
            DiffCuQpDeltaDepth = diffCuQpDeltaDepth;
            PpsCbQpOffset = ppsCbQpOffset;
            PpsCrQpOffset = ppsCrQpOffset;
            PpsSliceChromaQpOffsetsPresentFlag = ppsSliceChromaQpOffsetsPresentFlag;
            WeightedPredFlag = weightedPredFlag;
            WeightedBipredFlag = weightedBipredFlag;
            TransquantBypassEnabledFlag = transquantBypassEnabledFlag;
            TilesEnabledFlag = tilesEnabledFlag;
            EntropyCodingSyncEnabledFlag = entropyCodingSyncEnabledFlag;
            ColumnWidthMinus1 = columnWidthMinus1;
            RowHeightMinus1 = rowHeightMinus1;
            NumTileColumnsMinus1 = numTileColumnsMinus1;
            NumTileRowsMinus1 = numTileRowsMinus1;
            UniformSpacingFlag = uniformSpacingFlag;
            LoopFilterAcrossTilesEnabledFlag = loopFilterAcrossTilesEnabledFlag;
            PpsLoopFilterAcrossSlicesEnabledFlag = ppsLoopFilterAcrossSlicesEnabledFlag;
            DeblockingFilterControlPresentFlag = deblockingFilterControlPresentFlag;
            DeblockingFilterOverrideEnabledFlag = deblockingFilterOverrideEnabledFlag;
            PpsDeblockingFilterDisabledFlag = ppsDeblockingFilterDisabledFlag;
            PpsBetaOffsetDiv2 = ppsBetaOffsetDiv2;
            PpsTcOffsetDiv2 = ppsTcOffsetDiv2;
            PpsScalingListDataPresentFlag = ppsScalingListDataPresentFlag;
            PpsScalingList = ppsScalingList;
            ListsModificationPresentFlag = listsModificationPresentFlag;
            Log2ParallelMergeLevelMinus2 = log2ParallelMergeLevelMinus2;
            SliceSegmentHeaderExtensionPresentFlag = sliceSegmentHeaderExtensionPresentFlag;
            PpsExtensionPresentFlag = ppsExtensionPresentFlag;
            PpsRangeExtensionFlag = ppsRangeExtensionFlag;
            PpsMultilayerExtensionFlag = ppsMultilayerExtensionFlag;
            Pps3dExtensionFlag = pps3dExtensionFlag;
            PpsSccExtensionFlag = ppsSccExtensionFlag;
            PpsExtension4Bits = ppsExtension4Bits;
            PpsExtension4BitsData = ppsExtension4BitsData;
        }

        public static byte[] Build(H265PpsNalUnit b)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Build(ms, b);
                ms.Seek(0, SeekOrigin.Begin);
                return ms.ToArray();
            }
        }

        public static void Build(MemoryStream stream, H265PpsNalUnit b)
        {
            NalBitStreamWriter bitstream = new NalBitStreamWriter(stream);
            H265NalUnitHeader.BuildNALHeader(bitstream, b.Header);

            bitstream.WriteUE((uint)b.PpsPicParameterSetId);
            bitstream.WriteUE((uint)b.PpsSeqParameterSetId);
            bitstream.WriteBit(b.DependentSliceSegmentsEnabledFlag);
            bitstream.WriteBit(b.OutputFlagPresentFlag);
            bitstream.WriteBits(3, b.NumExtraSliceHeaderBits);
            bitstream.WriteBit(b.SignDataHidingEnabledFlag);
            bitstream.WriteBit(b.CabacInitPresentFlag);
            bitstream.WriteUE((uint)b.NumRefIdxL0DefaultActiveMinus1);
            bitstream.WriteUE((uint)b.NumRefIdxL1DefaultActiveMinus1);
            bitstream.WriteSE(b.InitQpMinus26);
            bitstream.WriteBit(b.ConstrainedIntraPredFlag);
            bitstream.WriteBit(b.TransformSkipEnabledFlag);
            bitstream.WriteBit(b.CuQpDeltaEnabledFlag);
            if (b.CuQpDeltaEnabledFlag)
            {
                bitstream.WriteUE((uint)b.DiffCuQpDeltaDepth);
            }
            bitstream.WriteSE(b.PpsCbQpOffset);
            bitstream.WriteSE(b.PpsCrQpOffset);
            bitstream.WriteBit(b.PpsSliceChromaQpOffsetsPresentFlag);
            bitstream.WriteBit(b.WeightedPredFlag);
            bitstream.WriteBit(b.WeightedBipredFlag);
            bitstream.WriteBit(b.TransquantBypassEnabledFlag);
            bitstream.WriteBit(b.TilesEnabledFlag);
            bitstream.WriteBit(b.EntropyCodingSyncEnabledFlag);
            if (b.TilesEnabledFlag)
            {
                bitstream.WriteUE((uint)b.NumTileColumnsMinus1);
                bitstream.WriteUE((uint)b.NumTileRowsMinus1);
                bitstream.WriteBit(b.UniformSpacingFlag);
                if (!b.UniformSpacingFlag)
                {
                    for (int i = 0; i < b.NumTileColumnsMinus1; i++)
                    {
                        bitstream.WriteUE((uint)b.ColumnWidthMinus1[i]);
                    }

                    for (int i = 0; i < b.NumTileRowsMinus1; i++)
                    {
                        bitstream.WriteUE((uint)b.RowHeightMinus1[i]);
                    }
                }

                bitstream.WriteBit(b.LoopFilterAcrossTilesEnabledFlag);
            }

            bitstream.WriteBit(b.PpsLoopFilterAcrossSlicesEnabledFlag);
            bitstream.WriteBit(b.DeblockingFilterControlPresentFlag);
            if (b.DeblockingFilterControlPresentFlag)
            {
                bitstream.WriteBit(b.DeblockingFilterOverrideEnabledFlag);
                bitstream.WriteBit(b.PpsDeblockingFilterDisabledFlag);
                if (!b.PpsDeblockingFilterDisabledFlag)
                {
                    bitstream.WriteSE(b.PpsBetaOffsetDiv2);
                    bitstream.WriteSE(b.PpsTcOffsetDiv2);
                }
            }

            bitstream.WriteBit(b.PpsScalingListDataPresentFlag);
            if (b.PpsScalingListDataPresentFlag)
            {
                H265ScalingListElement.Build(bitstream, b.PpsScalingList);
            }

            bitstream.WriteBit(b.ListsModificationPresentFlag);
            bitstream.WriteUE((uint)b.Log2ParallelMergeLevelMinus2);
            bitstream.WriteBit(b.SliceSegmentHeaderExtensionPresentFlag);
            bitstream.WriteBit(b.PpsExtensionPresentFlag);
            if (b.PpsExtensionPresentFlag)
            {
                bitstream.WriteBit(b.PpsRangeExtensionFlag);
                bitstream.WriteBit(b.PpsMultilayerExtensionFlag);
                bitstream.WriteBit(b.Pps3dExtensionFlag);
                bitstream.WriteBit(b.PpsSccExtensionFlag);
                bitstream.WriteBits(4, b.PpsExtension4Bits);
            }

            if (b.PpsRangeExtensionFlag)
            {
                throw new NotSupportedException("pps_range_extension not supported yet!");
            }

            if (b.PpsMultilayerExtensionFlag)
            {
                throw new NotSupportedException("pps_multilayer_extension not supported yet!");
            }

            if (b.Pps3dExtensionFlag)
            {
                throw new NotSupportedException("pps_3d_extension not supported yet!");
            }

            if (b.PpsSccExtensionFlag)
            {
                throw new NotSupportedException("pps_scaling_extension not supported yet!");
            }

            List<bool> ppsExtension4BitsData = new List<bool>();
            if (b.PpsExtension4Bits != 0)
            {
                foreach(var bbb in ppsExtension4BitsData)
                    bitstream.WriteBit(bbb);
            }

            bitstream.WriteTrailingBits();
        }

        public static H265PpsNalUnit Parse(byte[] pps)
        {
            using (MemoryStream ms = new MemoryStream(pps))
            {
                return Parse((ushort)pps.Length, ms);
            }
        }

        private static H265PpsNalUnit Parse(ushort size, MemoryStream stream)
        {
            NalBitStreamReader bitstream = new NalBitStreamReader(stream);
            H265NalUnitHeader header = H265NalUnitHeader.ParseNALHeader(bitstream);

            int ppsPicParameterSetId = bitstream.ReadUE();
            int ppsSeqParameterSetId = bitstream.ReadUE();
            bool dependentSliceSegmentsEnabledFlag = bitstream.ReadBit() != 0;
            bool outputFlagPresentFlag = bitstream.ReadBit() != 0;
            int numExtraSliceHeaderBits = bitstream.ReadBits(3);
            bool signDataHidingEnabledFlag = bitstream.ReadBit() != 0;
            bool cabacInitPresentFlag = bitstream.ReadBit() != 0;
            int numRefIdxL0DefaultActiveMinus1 = bitstream.ReadUE();
            int numRefIdxL1DefaultActiveMinus1 = bitstream.ReadUE();
            int initQpMinus26 = bitstream.ReadSE();
            bool constrainedIntraPredFlag = bitstream.ReadBit() != 0;
            bool transformSkipEnabledFlag = bitstream.ReadBit() != 0;
            bool cuQpDeltaEnabledFlag = bitstream.ReadBit() != 0;
            int diffCuQpDeltaDepth = 0;
            if (cuQpDeltaEnabledFlag)
            {
                diffCuQpDeltaDepth = bitstream.ReadUE();
            }
            int ppsCbQpOffset = bitstream.ReadSE();
            int ppsCrQpOffset = bitstream.ReadSE();
            bool ppsSliceChromaQpOffsetsPresentFlag = bitstream.ReadBit() != 0;
            bool weightedPredFlag = bitstream.ReadBit() != 0;
            bool weightedBipredFlag = bitstream.ReadBit() != 0;
            bool transquantBypassEnabledFlag = bitstream.ReadBit() != 0;
            bool tilesEnabledFlag = bitstream.ReadBit() != 0;
            bool entropyCodingSyncEnabledFlag = bitstream.ReadBit() != 0;
            List<int> columnWidthMinus1 = null;
            List<int> rowHeightMinus1 = null;
            int numTileColumnsMinus1 =0;
            int numTileRowsMinus1 = 0;
            bool uniformSpacingFlag = false;
            bool loopFilterAcrossTilesEnabledFlag = false;
            if (tilesEnabledFlag)
            {
                numTileColumnsMinus1 = bitstream.ReadUE();
                numTileRowsMinus1 = bitstream.ReadUE();
                uniformSpacingFlag = bitstream.ReadBit() != 0;
                if(!uniformSpacingFlag)
                {
                    columnWidthMinus1 = new List<int>();
                    rowHeightMinus1 = new List<int>();
                    
                    for (int i = 0; i < numTileColumnsMinus1; i++)
                    {
                        columnWidthMinus1.Add(bitstream.ReadUE());
                    }

                    for (int i = 0; i < numTileRowsMinus1; i++)
                    {
                        rowHeightMinus1.Add(bitstream.ReadUE());
                    }
                }

                loopFilterAcrossTilesEnabledFlag = bitstream.ReadBit() != 0;
            }

            bool ppsLoopFilterAcrossSlicesEnabledFlag = bitstream.ReadBit() != 0;
            bool deblockingFilterControlPresentFlag = bitstream.ReadBit() != 0;
            bool deblockingFilterOverrideEnabledFlag = false;
            bool ppsDeblockingFilterDisabledFlag = false;
            int ppsBetaOffsetDiv2 = 0;
            int ppsTcOffsetDiv2 = 0;
            if (deblockingFilterControlPresentFlag)
            {
                deblockingFilterOverrideEnabledFlag = bitstream.ReadBit() != 0;
                ppsDeblockingFilterDisabledFlag = bitstream.ReadBit() != 0;
                if(!ppsDeblockingFilterDisabledFlag)
                {
                    ppsBetaOffsetDiv2 = bitstream.ReadSE();
                    ppsTcOffsetDiv2 = bitstream.ReadSE();
                }
            }

            bool ppsScalingListDataPresentFlag = bitstream.ReadBit() != 0;
            List<H265ScalingListElement> ppsScalingList = null;
            if (ppsScalingListDataPresentFlag)
            {
                ppsScalingList = H265ScalingListElement.Parse(bitstream);
            }

            bool listsModificationPresentFlag = bitstream.ReadBit() != 0;
            int log2ParallelMergeLevelMinus2 = bitstream.ReadUE();
            bool sliceSegmentHeaderExtensionPresentFlag = bitstream.ReadBit() != 0;
            bool ppsExtensionPresentFlag = bitstream.ReadBit() != 0;
            bool ppsRangeExtensionFlag = false;
            bool ppsMultilayerExtensionFlag = false;
            bool pps3dExtensionFlag = false;
            bool ppsSccExtensionFlag = false;
            int ppsExtension4Bits = 0;
            if (ppsExtensionPresentFlag)
            {
                ppsRangeExtensionFlag = bitstream.ReadBit() != 0;
                ppsMultilayerExtensionFlag = bitstream.ReadBit() != 0;
                pps3dExtensionFlag = bitstream.ReadBit() != 0;
                ppsSccExtensionFlag = bitstream.ReadBit() != 0;
                ppsExtension4Bits = bitstream.ReadBits(4);
            }

            if(ppsRangeExtensionFlag)
            {
                throw new NotSupportedException("pps_range_extension not supported yet!");
            }

            if (ppsMultilayerExtensionFlag)
            {
                throw new NotSupportedException("pps_multilayer_extension not supported yet!");
            }

            if (pps3dExtensionFlag)
            {
                throw new NotSupportedException("pps_3d_extension not supported yet!");
            }

            if(ppsSccExtensionFlag)
            {
                throw new NotSupportedException("pps_scaling_extension not supported yet!");
            }

            List<bool> ppsExtension4BitsData = new List<bool>();
            if (ppsExtension4Bits != 0)
            {
                while (bitstream.HasMoreRBSPData(size))
                {
                    ppsExtension4BitsData.Add(bitstream.ReadBit() != 0);
                }
            }

            bitstream.ReadTrailingBits();

            return new H265PpsNalUnit(
                        header,
                        ppsPicParameterSetId,
                        ppsSeqParameterSetId,
                        dependentSliceSegmentsEnabledFlag,
                        outputFlagPresentFlag,
                        numExtraSliceHeaderBits,
                        signDataHidingEnabledFlag,
                        cabacInitPresentFlag,
                        numRefIdxL0DefaultActiveMinus1,
                        numRefIdxL1DefaultActiveMinus1,
                        initQpMinus26,
                        constrainedIntraPredFlag,
                        transformSkipEnabledFlag,
                        cuQpDeltaEnabledFlag,
                        diffCuQpDeltaDepth,
                        ppsCbQpOffset,
                        ppsCrQpOffset,
                        ppsSliceChromaQpOffsetsPresentFlag,
                        weightedPredFlag,
                        weightedBipredFlag,
                        transquantBypassEnabledFlag,
                        tilesEnabledFlag,
                        entropyCodingSyncEnabledFlag,
                        columnWidthMinus1,
                        rowHeightMinus1,
                        numTileColumnsMinus1,
                        numTileRowsMinus1,
                        uniformSpacingFlag,
                        loopFilterAcrossTilesEnabledFlag,
                        ppsLoopFilterAcrossSlicesEnabledFlag,
                        deblockingFilterControlPresentFlag,
                        deblockingFilterOverrideEnabledFlag,
                        ppsDeblockingFilterDisabledFlag,
                        ppsBetaOffsetDiv2,
                        ppsTcOffsetDiv2,
                        ppsScalingListDataPresentFlag,
                        ppsScalingList,
                        listsModificationPresentFlag,
                        log2ParallelMergeLevelMinus2,
                        sliceSegmentHeaderExtensionPresentFlag,
                        ppsExtensionPresentFlag,
                        ppsRangeExtensionFlag,
                        ppsMultilayerExtensionFlag,
                        pps3dExtensionFlag,
                        ppsSccExtensionFlag,
                        ppsExtension4Bits,
                        ppsExtension4BitsData
                    );
        }
    }

    public class H265NalUnitHeader
    {
        public H265NalUnitHeader(int nalUnitType, int nuhLayerId, int nuhTemporalIdPlus1)
        {
            NalUnitType = nalUnitType;
            NuhLayerId = nuhLayerId;
            NuhTemporalIdPlus1 = nuhTemporalIdPlus1;
        }

        public int NalUnitType { get; set; }
        public int NuhLayerId { get; set; }
        public int NuhTemporalIdPlus1 { get; set; }

        public bool IsVCL()
        {
            return NalUnitType >= 0 && NalUnitType <= 31;
        }
        
        public static H265NalUnitHeader ParseNALHeader(NalBitStreamReader bitstream)
        {
            if(bitstream.ReadBit() != 0) // forbidden zero bit
                throw new Exception("Invalid NAL header!");

            int nalUnitType = bitstream.ReadBits(6);
            int nuhLayerId = bitstream.ReadBits(6);
            int nuhTemporalIdPlus1 = bitstream.ReadBits(3);

            H265NalUnitHeader header = new H265NalUnitHeader(
                nalUnitType,
                nuhLayerId,
                nuhTemporalIdPlus1);
            return header;
        }

        public static void BuildNALHeader(NalBitStreamWriter bitstream, H265NalUnitHeader header)
        {
            bitstream.WriteBit(0); // forbidden 0 bit
            bitstream.WriteBits(6, header.NalUnitType);
            bitstream.WriteBits(6, header.NuhLayerId);
            bitstream.WriteBits(3, header.NuhTemporalIdPlus1);
        }
    }

    public class H265NalUnitTypes
    {
        public const int VPS = 32;
        public const int SPS = 33;
        public const int PPS = 34;
        public const int PREFIX_SEI_NUT = 39;
        public const int SUFFIX_SEI_NUT = 40;
    }

    public class HevcConfigurationBox : Mp4Box
    {
        public const string TYPE = "hvcC";

        public HevcDecoderConfigurationRecord HevcDecoderConfigurationRecord { get; set; }

        public HevcConfigurationBox(uint size, ulong largeSize, Mp4Box parent, HevcDecoderConfigurationRecord record) : base(size, largeSize, TYPE, parent)
        {
            HevcDecoderConfigurationRecord = record;
        }

        public static async Task<Mp4Box> ParseAsync(uint size, ulong largeSize, string type, Mp4Box parent, Stream stream)
        {
            ulong recordSize = (size == 1 ? largeSize : size) - (ulong)GetParsedSize(size);
            HevcConfigurationBox hvcc = new HevcConfigurationBox(
                size,
                largeSize,
                parent,
                await HevcDecoderConfigurationRecord.Parse(recordSize, stream));

            return hvcc;
        }

        public static Task<ulong> BuildAsync(Mp4Box box, Stream stream)
        {
            HevcConfigurationBox b = (HevcConfigurationBox)box;
            return HevcDecoderConfigurationRecord.Build(b.HevcDecoderConfigurationRecord, stream);
        }

        public override ulong CalculateSize()
        {
            return base.CalculateSize() + HevcDecoderConfigurationRecord.CalculateSize();
        }
    }

    public class HevcDecoderConfigurationRecord
    {
        public byte ConfigurationVersion { get; set; }
        public int GeneralProfileSpace { get; set; }
        public bool GeneralTierFlag { get; set; }
        public int GeneralProfileIdc { get; set; }
        public uint GeneralProfileCompatibilityFlags { get; set; }
        public ulong GeneralConstraintIndicatorFlags { get; set; }
        public bool FrameOnlyConstraintFlag { get; set; }
        public bool NonPackedConstraintFlag { get; set; }
        public bool InterlacedSourceFlag { get; set; }
        public bool ProgressiveSourceFlag { get; set; }
        public byte GeneralLevelIdc { get; set; }
        public int Reserved1 { get; set; }
        public int MinSpatialSegmentationIdc { get; set; }
        public int Reserved2 { get; set; }
        public int ParallelismType { get; set; }
        public int Reserved3 { get; set; }
        public int ChromaFormat { get; set; }
        public int Reserved4 { get; set; }
        public int BitDepthLumaMinus8 { get; set; }
        public int Reserved5 { get; set; }
        public int BitDepthChromaMinus8 { get; set; }
        public ushort AvgFrameRate { get; set; }
        public int ConstantFrameRate { get; set; }
        public int NumTemporalLayers { get; set; }
        public int TemporalIdNested { get; set; }
        public int LengthSizeMinusOne { get; set; }
        public List<HevcNalArray> NalArrays { get; set; } = new List<HevcNalArray>();

        public HevcDecoderConfigurationRecord()
        { }

        public HevcDecoderConfigurationRecord(
            byte configurationVersion, 
            int generalProfileSpace, 
            bool generalTierFlag, 
            int generalProfileIdc,
            uint generalProfileCompatibilityFlags,
            ulong generalConstraintIndicatorFlags, 
            bool frameOnlyConstraintFlag, 
            bool nonPackedConstraintFlag, 
            bool interlacedSourceFlag,
            bool progressiveSourceFlag, 
            byte generalLevelIdc, 
            int reserved1, 
            int minSpatialSegmentationIdc,
            int reserved2, 
            int parallelismType, 
            int reserved3, 
            int chromaFormat,
            int reserved4, 
            int bitDepthLumaMinus8,
            int reserved5, 
            int bitDepthChromaMinus8,
            ushort avgFrameRate, 
            int constantFrameRate, 
            int numTemporalLayers, 
            int temporalIdNested, 
            int lengthSizeMinusOne, 
            List<HevcNalArray> nalArrays)
        {
            ConfigurationVersion = configurationVersion;
            GeneralProfileSpace = generalProfileSpace;
            GeneralTierFlag = generalTierFlag;
            GeneralProfileIdc = generalProfileIdc;
            GeneralProfileCompatibilityFlags = generalProfileCompatibilityFlags;
            GeneralConstraintIndicatorFlags = generalConstraintIndicatorFlags;
            FrameOnlyConstraintFlag = frameOnlyConstraintFlag;
            NonPackedConstraintFlag = nonPackedConstraintFlag;
            InterlacedSourceFlag = interlacedSourceFlag;
            ProgressiveSourceFlag = progressiveSourceFlag;
            GeneralLevelIdc = generalLevelIdc;
            Reserved1 = reserved1;
            MinSpatialSegmentationIdc = minSpatialSegmentationIdc;
            Reserved2 = reserved2;
            ParallelismType = parallelismType;
            Reserved3 = reserved3;
            ChromaFormat = chromaFormat;
            Reserved4 = reserved4;
            BitDepthLumaMinus8 = bitDepthLumaMinus8;
            Reserved5 = reserved5;
            BitDepthChromaMinus8 = bitDepthChromaMinus8;
            AvgFrameRate = avgFrameRate;
            ConstantFrameRate = constantFrameRate;
            NumTemporalLayers = numTemporalLayers;
            TemporalIdNested = temporalIdNested;
            LengthSizeMinusOne = lengthSizeMinusOne;
            NalArrays = nalArrays;
        }

        public static async Task<HevcDecoderConfigurationRecord> Parse(ulong size, Stream stream)
        {
            byte configurationVersion = IsoReaderWriter.ReadByte(stream);

            int a = IsoReaderWriter.ReadByte(stream);
            int generalProfileSpace = (a & 0xC0) >> 6;
            bool generalTierFlag = (a & 0x20) > 0;
            int generalProfileIdc = a & 0x1F;

            uint generalProfileCompatibilityFlags = IsoReaderWriter.ReadUInt32(stream);
            ulong generalConstraintIndicatorFlags = IsoReaderWriter.ReadUInt48(stream);

            bool frameOnlyConstraintFlag = (generalConstraintIndicatorFlags >> 44 & 0x08) > 0;
            bool nonPackedConstraintFlag = (generalConstraintIndicatorFlags >> 44 & 0x04) > 0;
            bool interlacedSourceFlag = (generalConstraintIndicatorFlags >> 44 & 0x02) > 0;
            bool progressiveSourceFlag = (generalConstraintIndicatorFlags >> 44 & 0x01) > 0;

            generalConstraintIndicatorFlags &= 0x7fffffffffffL;

            byte generalLevelIdc = IsoReaderWriter.ReadByte(stream);

            a = IsoReaderWriter.ReadUInt16(stream);
            int reserved1 = (a & 0xF000) >> 12;
            int minSpatialSegmentationIdc = a & 0xFFF;

            a = IsoReaderWriter.ReadByte(stream);
            int reserved2 = (a & 0xFC) >> 2;
            int parallelismType = a & 0x3;

            a = IsoReaderWriter.ReadByte(stream);
            int reserved3 = (a & 0xFC) >> 2;
            int chromaFormat = a & 0x3;

            a = IsoReaderWriter.ReadByte(stream);
            int reserved4 = (a & 0xF8) >> 3;
            int bitDepthLumaMinus8 = a & 0x7;

            a = IsoReaderWriter.ReadByte(stream);
            int reserved5 = (a & 0xF8) >> 3;
            int bitDepthChromaMinus8 = a & 0x7;

            ushort avgFrameRate = IsoReaderWriter.ReadUInt16(stream);

            a = IsoReaderWriter.ReadByte(stream);
            int constantFrameRate = (a & 0xC0) >> 6;
            int numTemporalLayers = (a & 0x38) >> 3;
            int temporalIdNested = (a & 0x4) > 0 ? 1 : 0;
            int lengthSizeMinusOne = a & 0x3;

            int numOfArrays = IsoReaderWriter.ReadByte(stream);
            List<HevcNalArray> nalArrays = new List<HevcNalArray>();
            for (int i = 0; i < numOfArrays; i++)
            {
                HevcNalArray array = new HevcNalArray();

                a = IsoReaderWriter.ReadByte(stream);
                array.ArrayCompleteness = (a & 0x80) > 0 ? 1 : 0;
                array.Reserved = (a & 0x40) > 0 ? 1 : 0;
                array.NalUnitType = a & 0x3F;

                int numNalus = IsoReaderWriter.ReadUInt16(stream);
                for (int j = 0; j < numNalus; j++)
                {
                    int nalUnitLength = IsoReaderWriter.ReadUInt16(stream);
                    byte[] nal = new byte[nalUnitLength];
                    await IsoReaderWriter.ReadBytesAsync(stream, nal, 0, nal.Length);
                    array.NalUnits.Add(nal);
                }
                nalArrays.Add(array);
            }

            return new HevcDecoderConfigurationRecord(
                configurationVersion,
                generalProfileSpace,
                generalTierFlag,
                generalProfileIdc,
                generalProfileCompatibilityFlags,
                generalConstraintIndicatorFlags,
                frameOnlyConstraintFlag,
                nonPackedConstraintFlag,
                interlacedSourceFlag,
                progressiveSourceFlag,
                generalLevelIdc,
                reserved1,
                minSpatialSegmentationIdc,
                reserved2,
                parallelismType,
                reserved3,
                chromaFormat,
                reserved4,
                bitDepthLumaMinus8,
                reserved5,
                bitDepthChromaMinus8,
                avgFrameRate,
                constantFrameRate,
                numTemporalLayers,
                temporalIdNested,
                lengthSizeMinusOne,
                nalArrays
            );
        }

        public uint CalculateSize()
        {
            uint size = 23;
            foreach (HevcNalArray array in NalArrays)
            {
                size += 3;
                foreach (byte[] nalUnit in array.NalUnits)
                {
                    size += 2;
                    size += (uint)nalUnit.Length;
                }
            }
            return size;
        }

        public static async Task<ulong> Build(HevcDecoderConfigurationRecord b, Stream stream)
        {
            uint size = 0;
            size += IsoReaderWriter.WriteByte(stream, b.ConfigurationVersion);
            size += IsoReaderWriter.WriteByte(stream, (byte)((b.GeneralProfileSpace << 6) + (b.GeneralTierFlag ? 0x20 : 0) + b.GeneralProfileIdc));
            size += IsoReaderWriter.WriteUInt32(stream, b.GeneralProfileCompatibilityFlags);
            
            ulong generalConstraintIndicatorFlags = b.GeneralConstraintIndicatorFlags;
            if (b.FrameOnlyConstraintFlag)
            {
                generalConstraintIndicatorFlags |= 1L << 47;
            }
            if (b.NonPackedConstraintFlag)
            {
                generalConstraintIndicatorFlags |= 1L << 46;
            }
            if (b.InterlacedSourceFlag)
            {
                generalConstraintIndicatorFlags |= 1L << 45;
            }
            if (b.ProgressiveSourceFlag)
            {
                generalConstraintIndicatorFlags |= 1L << 44;
            }

            size += IsoReaderWriter.WriteUInt48(stream, generalConstraintIndicatorFlags);
            size += IsoReaderWriter.WriteByte(stream, b.GeneralLevelIdc);
            size += IsoReaderWriter.WriteUInt16(stream, (ushort)((b.Reserved1 << 12) + b.MinSpatialSegmentationIdc));
            size += IsoReaderWriter.WriteByte(stream, (byte)((b.Reserved2 << 2) + b.ParallelismType));
            size += IsoReaderWriter.WriteByte(stream, (byte)((b.Reserved3 << 2) + b.ChromaFormat));
            size += IsoReaderWriter.WriteByte(stream, (byte)((b.Reserved4 << 3) + b.BitDepthLumaMinus8));
            size += IsoReaderWriter.WriteByte(stream, (byte)((b.Reserved5 << 3) + b.BitDepthChromaMinus8));
            size += IsoReaderWriter.WriteUInt16(stream, b.AvgFrameRate);
            size += IsoReaderWriter.WriteByte(stream, (byte)((b.ConstantFrameRate << 6) + (b.NumTemporalLayers << 3) + (b.TemporalIdNested != 0 ? 0x4 : 0) + b.LengthSizeMinusOne));
            size += IsoReaderWriter.WriteByte(stream, (byte)b.NalArrays.Count);

            foreach (HevcNalArray nalArray in b.NalArrays)
            {
                size += IsoReaderWriter.WriteByte(stream, (byte)((nalArray.ArrayCompleteness != 0 ? 0x80 : 0) + (nalArray.Reserved != 0 ? 0x40 : 0) + nalArray.NalUnitType));
                size += IsoReaderWriter.WriteUInt16(stream, (ushort)nalArray.NalUnits.Count);
                
                foreach (byte[] nalUnit in nalArray.NalUnits)
                {
                    size += IsoReaderWriter.WriteUInt16(stream, (ushort)nalUnit.Length);
                    size += await IsoReaderWriter.WriteBytesAsync(stream, nalUnit, 0, nalUnit.Length);
                }
            }

            return size;
        }
    }

    public sealed class HevcNalArray
    {
        public int ArrayCompleteness { get; set; }
        public int Reserved { get; set; }
        public int NalUnitType { get; set; }
        public List<byte[]> NalUnits { get; set; } = new List<byte[]>();
    }
}
