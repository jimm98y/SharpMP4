using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SharpMp4
{
    public class H265Track : TrackBase
    {
        public Dictionary<int, H265SpsNalUnit> Sps { get; set; } = new Dictionary<int, H265SpsNalUnit>();
        public Dictionary<int, H265PpsNalUnit> Pps { get; set; } = new Dictionary<int, H265PpsNalUnit>();
        public Dictionary<int, H265VpsNalUnit> Vps { get; set; } = new Dictionary<int, H265VpsNalUnit>();

        public H265Track(uint trackID, uint timescale) : base(trackID)
        {
            base.Handler = "vide";
            this.Timescale = timescale;
        }

        private H265NalSliceHeader _lastSliceHeader;
        private List<byte[]> _nalBuffer = new List<byte[]>();

        public override async Task ProcessSampleAsync(byte[] sample, uint duration)
        {
            var header = H265NalUnitHeader.ParseNALHeader(sample[0]);
            if (header.NalUnitType == H265NalUnitTypes.SPS)
            {
                if (Log.DebugEnabled) Log.Debug($"-Parsed SPS: {ToHexString(sample)}");
                var sps = H265SpsNalUnit.Parse(sample);
                if (!Sps.ContainsKey(sps.SeqParameterSetId))
                {
                    Sps.Add(sps.SeqParameterSetId, sps);
                }
                if (Log.DebugEnabled) Log.Debug($"Rebuilt SPS: {ToHexString(H265SpsNalUnit.Build(sps))}");
            }
            else if (header.NalUnitType == H265NalUnitTypes.PPS)
            {
                if (Log.DebugEnabled) Log.Debug($"-Parsed PPS: {ToHexString(sample)}");
                var pps = H265PpsNalUnit.Parse(sample);
                if (!Pps.ContainsKey(pps.PicParameterSetId))
                {
                    Pps.Add(pps.PicParameterSetId, pps);
                }
                if (Log.DebugEnabled) Log.Debug($"Rebuilt PPS: {ToHexString(H265PpsNalUnit.Build(pps))}");
            }
            else if (header.NalUnitType == H265NalUnitTypes.VPS)
            {
                if (Log.DebugEnabled) Log.Debug($"-Parsed VPS: {ToHexString(sample)}");
                var vps = H265VpsNalUnit.Parse(sample);
                if (!Vps.ContainsKey(vps.VpsParameterSetId))
                {
                    Vps.Add(vps.VpsParameterSetId, vps);
                }
                if (Log.DebugEnabled) Log.Debug($"Rebuilt VPS: {ToHexString(H265VpsNalUnit.Build(vps))}");
            }
            else
            {
                var sliceHeader = ReadSliceHeader(sample);

                _nalBuffer.Add(sample);

                if (IsNewSample(_lastSliceHeader, sliceHeader))
                {
                    int len = sample.Length;
                    IEnumerable<byte> result = new byte[0];

                    foreach (var nal in _nalBuffer)
                    {
                        // for each NAL, add 4 byte NAL size
                        byte[] size = new byte[] {
                            (byte)((len & 0xff000000) >> 24),
                            (byte)((len & 0xff0000) >> 16),
                            (byte)((len & 0xff00) >> 8),
                            (byte)(len & 0xff)
                        };
                        result = result.Concat(size).Concat(sample);
                    }

                    await base.ProcessSampleAsync(result.ToArray(), duration);
                    _nalBuffer.Clear();
                }

                _lastSliceHeader = sliceHeader;
            }
        }

        public static bool IsNewSample(H265NalSliceHeader oldHeader, H265NalSliceHeader newHeader)
        {
            throw new NotImplementedException();
        }

        private H265NalSliceHeader ReadSliceHeader(byte[] sample)
        {
            throw new NotImplementedException();
        }
    }

    public class H265VpsNalUnit
    {
        public H265VpsNalUnit(int vpsParameterSetId, int vpsReservedThree2bits, int vpsMaxLayersMinus1, int vpsMaxSubLayersMinus1, int vpsTemporalIdNestingFlag, int vpsReserved0xffff16bits, int generalProfileSpace, int generalTierFlag, int generalProfileIdc, int generalProfileCompatibilityFlags, long generalProfileConstraintIndicatorFlags, int generalLevelIdc, bool[] subLayerProfilePresentFlag, bool[] subLayerLevelPresentFlag, int[] subLayers, int[] subLayerProfileSpace, int[] subLayerTierFlag, int[] subLayerProfileIdc, bool[,] subLayerProfileCompatibilityFlag, bool[] subLayerProgressiveSourceFlag, bool[] subLayerInterlacedSourceFlag, bool[] subLayerNonPackedConstraintFlag, bool[] subLayerFrameOnlyConstraintFlag, int[] subLayerLevelIdc, long[] reservedBits, bool vpsSubLayerOrderingInfoPresentFlag, int[] vpsMaxDecPicBufferingMinus1, int[] vpsMaxNumReorderPics, int[] vpsMaxLatencyIncreasePlus1, int vpsMaxLayerId, int vpsNumLayerSetsMinus1, bool[,] layerIdIncludedFlag, bool vpsTimingInfoPresentFlag, int vpsNumUnitsInTick, int vpsTimeScale, bool vpsPocProportionalToTimingFlag, int vpsNumTicksPocDiffOneMinus1, int vpsNumHrdParameters, int[] hrdLayerSetIdx, bool[] cprmsPresentFlag, H265HrdParameters[] hrdParameters, bool vpsExtensionFlag, List<bool> vpsExtensionDataFlag)
        {
            VpsParameterSetId = vpsParameterSetId;
            VpsReservedThree2bits = vpsReservedThree2bits;
            VpsMaxLayersMinus1 = vpsMaxLayersMinus1;
            VpsMaxSubLayersMinus1 = vpsMaxSubLayersMinus1;
            VpsTemporalIdNestingFlag = vpsTemporalIdNestingFlag;
            VpsReserved0xffff16bits = vpsReserved0xffff16bits;
            GeneralProfileSpace = generalProfileSpace;
            GeneralTierFlag = generalTierFlag;
            GeneralProfileIdc = generalProfileIdc;
            GeneralProfileCompatibilityFlags = generalProfileCompatibilityFlags;
            GeneralProfileConstraintIndicatorFlags = generalProfileConstraintIndicatorFlags;
            GeneralLevelIdc = generalLevelIdc;
            SubLayerProfilePresentFlag = subLayerProfilePresentFlag;
            SubLayerLevelPresentFlag = subLayerLevelPresentFlag;
            SubLayers = subLayers;
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

        public int VpsParameterSetId { get; set; }
        public int VpsReservedThree2bits { get; set; }
        public int VpsMaxLayersMinus1 { get; set; }
        public int VpsMaxSubLayersMinus1 { get; set; }
        public int VpsTemporalIdNestingFlag { get; set; }
        public int VpsReserved0xffff16bits { get; set; }
        public int GeneralProfileSpace { get; set; }
        public int GeneralTierFlag { get; set; }
        public int GeneralProfileIdc { get; set; }
        public int GeneralProfileCompatibilityFlags { get; set; }
        public long GeneralProfileConstraintIndicatorFlags { get; set; }
        public int GeneralLevelIdc { get; set; }
        public bool[] SubLayerProfilePresentFlag { get; set; }
        public bool[] SubLayerLevelPresentFlag { get; set; }
        public int[] SubLayers { get; set; }
        public int[] SubLayerProfileSpace { get; set; }
        public int[] SubLayerTierFlag { get; set; }
        public int[] SubLayerProfileIdc { get; set; }
        public bool[,] SubLayerProfileCompatibilityFlag { get; set; }
        public bool[] SubLayerProgressiveSourceFlag { get; set; }
        public bool[] SubLayerInterlacedSourceFlag { get; set; }
        public bool[] SubLayerNonPackedConstraintFlag { get; set; }
        public bool[] SubLayerFrameOnlyConstraintFlag { get; set; }
        public int[] SubLayerLevelIdc { get; set; }
        public long[] ReservedBits { get; set; }
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
        public List<bool> VpsExtensionDataFlag { get; set; }

        public static H265VpsNalUnit Parse(byte[] vps)
        {
            using (MemoryStream ms = new MemoryStream(vps))
            {
                return Parse((ushort)vps.Length, ms);
            }
        }

        private static H265VpsNalUnit Parse(ushort size, Stream stream)
        {
            BitStreamReader bitstream = new BitStreamReader(stream);

            int vpsParameterSetId = bitstream.ReadBits(4);
            int vpsReservedThree2bits = bitstream.ReadBits(2);
            int vpsMaxLayersMinus1 = bitstream.ReadBits(6);
            int vpsMaxSubLayersMinus1 = bitstream.ReadBits(3);
            int vpsTemporalIdNestingFlag = bitstream.ReadBit();
            int vpsReserved0xffff16bits = bitstream.ReadBits(16);

            int generalProfileSpace = bitstream.ReadBits(2);
            int generalTierFlag = bitstream.ReadBit();
            int generalProfileIdc = bitstream.ReadBits(5);

            int generalProfileCompatibilityFlags = bitstream.ReadBits(32);
            long generalProfileConstraintIndicatorFlags = bitstream.ReadBitsLong(48); // TODO
            int generalLevelIdc = bitstream.ReadBits(8);

            bool[] subLayerProfilePresentFlag = new bool[vpsMaxSubLayersMinus1];
            bool[] subLayerLevelPresentFlag = new bool[vpsMaxSubLayersMinus1];
            for (int i = 0; i < vpsMaxSubLayersMinus1; i++)
            {
                subLayerProfilePresentFlag[i] = bitstream.ReadBit() != 0;
                subLayerLevelPresentFlag[i] = bitstream.ReadBit() != 0;
            }

            int[] subLayers = null;
            if (vpsMaxSubLayersMinus1 > 0)
            {
                subLayers = new int[8];
                for (int i = vpsMaxSubLayersMinus1; i < 8; i++)
                {
                    subLayers[i] = bitstream.ReadBits(2);
                }
            }

            int[] subLayerProfileSpace = new int[vpsMaxSubLayersMinus1];
            int[] subLayerTierFlag = new int[vpsMaxSubLayersMinus1];
            int[] subLayerProfileIdc = new int[vpsMaxSubLayersMinus1];
            bool[,] subLayerProfileCompatibilityFlag = new bool[vpsMaxSubLayersMinus1, 32];
            bool[] subLayerProgressiveSourceFlag = new bool[vpsMaxSubLayersMinus1];

            bool[] subLayerInterlacedSourceFlag = new bool[vpsMaxSubLayersMinus1];
            bool[] subLayerNonPackedConstraintFlag = new bool[vpsMaxSubLayersMinus1];
            bool[] subLayerFrameOnlyConstraintFlag = new bool[vpsMaxSubLayersMinus1];
            int[] subLayerLevelIdc = new int[vpsMaxSubLayersMinus1];
            long[] reservedBits = new long[vpsMaxSubLayersMinus1];

            for (int i = 0; i < vpsMaxSubLayersMinus1; i++)
            {
                if (subLayerProfilePresentFlag[i])
                {
                    subLayerProfileSpace[i] = bitstream.ReadBits(2);
                    subLayerTierFlag[i] = bitstream.ReadBit();
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

            bool vpsSubLayerOrderingInfoPresentFlag = bitstream.ReadBit() != 0;
            int[] vpsMaxDecPicBufferingMinus1 = new int[vpsSubLayerOrderingInfoPresentFlag ? 1 : vpsMaxSubLayersMinus1 + 1];
            int[] vpsMaxNumReorderPics = new int[vpsSubLayerOrderingInfoPresentFlag ? 1 : vpsMaxSubLayersMinus1 + 1];
            int[] vpsMaxLatencyIncreasePlus1 = new int[vpsSubLayerOrderingInfoPresentFlag ? 1 : vpsMaxSubLayersMinus1 + 1];
            for (int i = (vpsSubLayerOrderingInfoPresentFlag ? 0 : vpsMaxSubLayersMinus1); i <= vpsMaxSubLayersMinus1; i++)
            {
                vpsMaxDecPicBufferingMinus1[i] = bitstream.ReadUE();
                vpsMaxNumReorderPics[i] = bitstream.ReadUE();
                vpsMaxLatencyIncreasePlus1[i] = bitstream.ReadUE();
            }
            int vpsMaxLayerId = bitstream.ReadBits(6);
            int vpsNumLayerSetsMinus1 = bitstream.ReadUE();
            bool[,] layerIdIncludedFlag = new bool[vpsNumLayerSetsMinus1, vpsMaxLayerId];
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
                vpsParameterSetId,
                vpsReservedThree2bits,
                vpsMaxLayersMinus1,
                vpsMaxSubLayersMinus1,
                vpsTemporalIdNestingFlag,
                vpsReserved0xffff16bits,
                generalProfileSpace,
                generalTierFlag,
                generalProfileIdc,
                generalProfileCompatibilityFlags,
                generalProfileConstraintIndicatorFlags,
                generalLevelIdc,
                subLayerProfilePresentFlag,
                subLayerLevelPresentFlag,
                subLayers,
                subLayerProfileSpace,
                subLayerTierFlag,
                subLayerProfileIdc,
                subLayerProfileCompatibilityFlag,
                subLayerProgressiveSourceFlag,
                subLayerInterlacedSourceFlag,
                subLayerNonPackedConstraintFlag,
                subLayerFrameOnlyConstraintFlag,
                subLayerLevelIdc,
                reservedBits,
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

        public static byte[] Build(H265VpsNalUnit vps)
        {
            throw new NotImplementedException();
        }
    }

    public class H265SpsNalUnit
    {
        public int SeqParameterSetId { get; set; }

        public static byte[] Build(H265SpsNalUnit sps)
        {
            throw new NotImplementedException();
        }

        public static H265SpsNalUnit Parse(byte[] sps)
        {
            using (MemoryStream ms = new MemoryStream(sps))
            {
                return Parse((ushort)sps.Length, ms);
            }
        }

        private static H265SpsNalUnit Parse(ushort length, Stream ms)
        {
            throw new NotImplementedException();
        }

        public (ushort Width, ushort Height) CalculateDimensions()
        {
            throw new NotImplementedException();
        }
    }

    public class H265VuiParameters
    {
        public static H265VuiParameters Parse(BitStreamReader bitstream)
        {
            throw new NotImplementedException();
        }

        public static void Build(BitStreamWriter bitstream, H265VuiParameters vui)
        {
            throw new NotImplementedException();
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

        public static H265HrdParameters Parse(BitStreamReader bitstream, bool commonInfPresentFlag, int maxNumSubLayersMinus1)
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

            bool[] fixedPicRateGeneralFlag = new bool[maxNumSubLayersMinus1];
            bool[] fixedPicRateWithinCvsFlag = new bool[maxNumSubLayersMinus1];
            bool[] lowDelayHrdFlag = new bool[maxNumSubLayersMinus1];
            int[] cpbCntMinus1 = new int[maxNumSubLayersMinus1];
            int[] elementalDurationInTcMinus1 = new int[maxNumSubLayersMinus1];
            H265HrdSubLayerParameters[] nalHrdSubLayerParameters = new H265HrdSubLayerParameters[maxNumSubLayersMinus1];
            H265HrdSubLayerParameters[] vclHrdSubLayerParameters = new H265HrdSubLayerParameters[maxNumSubLayersMinus1];
            for (int i = 0; i <= maxNumSubLayersMinus1; i++)
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

        public static void Build(BitStreamWriter bitstream, H265HrdParameters hrd)
        {
            throw new NotImplementedException();
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

        public static H265HrdSubLayerParameters Parse(BitStreamReader bitstream, int cpbCnt, bool subPicHrdParamsPresentFlag)
        {
            int[] bitRateValueMinus1 = new int[cpbCnt];
            int[] cpbSizeValueMinus1 = new int[cpbCnt];
            int[] cpbSizeDuValueMinus1 = new int[cpbCnt];
            int[] bitRateDuValueMinus1 = new int[cpbCnt];
            bool[] cbrFlag = new bool[cpbCnt];

            for (int i = 0; i <= cpbCnt; i++)
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

        public static void Build(BitStreamWriter bitstream, H265HrdSubLayerParameters hrd)
        {
            throw new NotImplementedException();
        }
    }

    public class H265PpsNalUnit
    {
        public int PicParameterSetId { get; set; }

        public static byte[] Build(H265PpsNalUnit pps)
        {
            throw new NotImplementedException();
        }

        public static H265PpsNalUnit Parse(byte[] pps)
        {
            using (MemoryStream ms = new MemoryStream(pps))
            {
                return Parse((ushort)pps.Length, ms);
            }
        }

        private static H265PpsNalUnit Parse(ushort size, Stream stream)
        {
            throw new NotImplementedException();
        }
    }

    public class H265NalUnitHeader
    {
        public int NalUnitType { get; set; }

        public static H265NalUnitHeader ParseNALHeader(byte header)
        {
            throw new NotImplementedException();
        }
    }

    public class H265NalUnitTypes
    {
        public const int VPS = 32;
        public const int SPS = 33;
        public const int PPS = 34;
    }

    public class H265NalSliceHeader
    {
    }

    public class HevcConfigurationBox : Mp4Box
    {
        public const string TYPE = "hvcC";

        public HevcDecoderConfigurationRecord HevcDecoderConfigurationRecord { get; }

        public HevcConfigurationBox(uint size, Mp4Box parent, HevcDecoderConfigurationRecord record) : base(size, TYPE, parent)
        {
            HevcDecoderConfigurationRecord = record;
        }

        public static async Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            HevcConfigurationBox hvcc = new HevcConfigurationBox(
                size,
                parent,
                await HevcDecoderConfigurationRecord.Parse(size - 8, stream));

            return hvcc;
        }

        public static Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            HevcConfigurationBox b = (HevcConfigurationBox)box;
            return HevcDecoderConfigurationRecord.Build(b.HevcDecoderConfigurationRecord, stream);
        }

        public override uint CalculateSize()
        {
            return base.CalculateSize() + HevcDecoderConfigurationRecord.CalculateSize();
        }
    }

    public class HevcDecoderConfigurationRecord
    {
        public byte ConfigurationVersion { get; set; }
        public int GeneralProfileSpace { get; set; }
        public int GeneralTierFlag { get; set; }
        public int GeneralProfileIdc { get; set; }
        public uint GeneralProfileCompatibilityFlags { get; set; }
        public ulong GeneralConstraintIndicatorFlags { get; set; }
        public int FrameOnlyConstraintFlag { get; set; }
        public int NonPackedConstraintFlag { get; set; }
        public int InterlacedSourceFlag { get; set; }
        public int ProgressiveSourceFlag { get; set; }
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

        public HevcDecoderConfigurationRecord(byte configurationVersion, int generalProfileSpace, int generalTierFlag, int generalProfileIdc, uint generalProfileCompatibilityFlags, ulong generalConstraintIndicatorFlags, int frameOnlyConstraintFlag, int nonPackedConstraintFlag, int interlacedSourceFlag, int progressiveSourceFlag, byte generalLevelIdc, int reserved1, int minSpatialSegmentationIdc, int reserved2, int parallelismType, int reserved3, int chromaFormat, int reserved4, int bitDepthLumaMinus8, int reserved5, int bitDepthChromaMinus8, ushort avgFrameRate, int constantFrameRate, int numTemporalLayers, int temporalIdNested, int lengthSizeMinusOne, List<HevcNalArray> nalArrays)
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

        public static async Task<HevcDecoderConfigurationRecord> Parse(uint size, Stream stream)
        {
            byte configurationVersion = IsoReaderWriter.ReadByte(stream);

            int a = IsoReaderWriter.ReadByte(stream);
            int generalProfileSpace = (a & 0xC0) >> 6;
            int generalTierFlag = (a & 0x20) > 0 ? 1 : 0;
            int generalProfileIdc = a & 0x1F;

            uint generalProfileCompatibilityFlags = IsoReaderWriter.ReadUInt32(stream);
            ulong generalConstraintIndicatorFlags = IsoReaderWriter.ReadUInt48(stream);

            int frameOnlyConstraintFlag = (generalConstraintIndicatorFlags >> 44 & 0x08) > 0 ? 1 : 0;
            int nonPackedConstraintFlag = (generalConstraintIndicatorFlags >> 44 & 0x04) > 0 ? 1 : 0;
            int interlacedSourceFlag = (generalConstraintIndicatorFlags >> 44 & 0x02) > 0 ? 1 : 0;
            int progressiveSourceFlag = (generalConstraintIndicatorFlags >> 44 & 0x01) > 0 ? 1 : 0;

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

        public static async Task<uint> Build(HevcDecoderConfigurationRecord b, Stream stream)
        {
            uint size = 0;
            size += IsoReaderWriter.WriteByte(stream, b.ConfigurationVersion);
            size += IsoReaderWriter.WriteByte(stream, (byte)((b.GeneralProfileSpace << 6) + (b.GeneralTierFlag == 1 ? 0x20 : 0) + b.GeneralProfileIdc));
            size += IsoReaderWriter.WriteUInt32(stream, b.GeneralProfileCompatibilityFlags);
            
            ulong generalConstraintIndicatorFlags = b.GeneralConstraintIndicatorFlags;
            if (b.FrameOnlyConstraintFlag != 0)
            {
                generalConstraintIndicatorFlags |= 1L << 47;
            }
            if (b.NonPackedConstraintFlag != 0)
            {
                generalConstraintIndicatorFlags |= 1L << 46;
            }
            if (b.InterlacedSourceFlag != 0)
            {
                generalConstraintIndicatorFlags |= 1L << 45;
            }
            if (b.ProgressiveSourceFlag != 0)
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

    public class H265BoxBuilder
    {
        public static VisualSampleEntryBox CreateVisualSampleEntryBox(Mp4Box parent, H265Track track)
        {
            var sps = track.Sps.First().Value; // TODO: not sure about multiple SPS values...
            var vps = track.Vps.First().Value; // TODO: not sure about multiple VPS values...
            var dim = sps.CalculateDimensions();

            VisualSampleEntryBox visualSampleEntry = new VisualSampleEntryBox(0, parent, "hvc1");
            visualSampleEntry.DataReferenceIndex = 1;
            visualSampleEntry.Depth = 24;
            visualSampleEntry.FrameCount = 1;
            visualSampleEntry.HorizontalResolution = 72;
            visualSampleEntry.VerticalResolution = 72;
            visualSampleEntry.Width = dim.Width;
            visualSampleEntry.Height = dim.Height;
            visualSampleEntry.CompressorName = "hevc\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0";
            visualSampleEntry.CompressorNameDisplayableData = 4;

            HevcConfigurationBox hevcConfigurationBox = new HevcConfigurationBox(0, visualSampleEntry, new HevcDecoderConfigurationRecord());
            hevcConfigurationBox.HevcDecoderConfigurationRecord.ConfigurationVersion = 1;
            //hevcConfigurationBox.HevcDecoderConfigurationRecord.GeneralProfileIdc = sps.GeneralProfileIdc;
            //hevcConfigurationBox.HevcDecoderConfigurationRecord.ChromaFormat = sps.ChromaFormatIdc;
            //hevcConfigurationBox.HevcDecoderConfigurationRecord.GeneralLevelIdc = sps.GeneralLevelIdc;
            //hevcConfigurationBox.HevcDecoderConfigurationRecord.GeneralProfileCompatibilityFlags = sps.GeneralProfileCompatibilityFlags;
            //hevcConfigurationBox.HevcDecoderConfigurationRecord.GeneralConstraintIndicatorFlags = vps.GeneralProfileConstraintIndicatorFlags;
            //hevcConfigurationBox.HevcDecoderConfigurationRecord.BitDepthChromaMinus8 = sps.BitDepthChromaMinus8;
            //hevcConfigurationBox.HevcDecoderConfigurationRecord.BitDepthLumaMinus8 = sps.BitDepthLumaMinus8;
            //hevcConfigurationBox.HevcDecoderConfigurationRecord.TemporalIdNested = sps.SpsTemporalIdNestingFlag;
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
    }
}
