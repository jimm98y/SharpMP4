
using System;
using System.Collections.Generic;

namespace SharpH265
{
    public class H265Constants
    {
        public const uint EXTENDED_ISO = 255;
        public const uint EXTENDED_SAR = 255;
    }

    public class H265FrameTypes
    {
        public const ulong B = 0;
        public const ulong P = 1;
        public const ulong I = 2;

        public static bool IsB(ulong value) { return value == B; }
        public static bool IsP(ulong value) { return value == P; }
        public static bool IsI(ulong value) { return value == I; }
    }

    public class H265NALTypes
    {
        public const uint TRAIL_N = 0;                 // Coded slice segment of a non-TSA, non-STSA trailing picture
        public const uint TRAIL_R = 1;                 // Coded slice segment of a non-TSA, non-STSA trailing picture
        public const uint TSA_N = 2;                   // Coded slice segment of a TSA picture
        public const uint TSA_R = 3;                   // Coded slice segment of a TSA picture
        public const uint STSA_N = 4;                  // Coded slice segment of a STSA picture
        public const uint STSA_R = 5;                  // Coded slice segment of a STSA picture
        public const uint RADL_N = 6;                  // Coded slice segment of a RADL picture
        public const uint RADL_R = 7;                  // Coded slice segment of a RADL picture
        public const uint RASL_N = 8;                  // Coded slice segment of a RASL picture
        public const uint RASL_R = 9;                  // Coded slice segment of a RASL picture

        public const uint RSV_VCL_N10 = 10;            // Reserved non-IRAP SLNR VCL NAL unit types
        public const uint RSV_VCL_R11 = 11;            // Reserved non-IRAP sub-layer reference VCL NAL unit types
        public const uint RSV_VCL_N12 = 12;            // Reserved non-IRAP SLNR VCL NAL unit types
        public const uint RSV_VCL_R13 = 13;            // Reserved non-IRAP sub-layer reference VCL NAL unit types
        public const uint RSV_VCL_N14 = 14;            // Reserved non-IRAP SLNR VCL NAL unit types
        public const uint RSV_VCL_R15 = 15;            // Reserved non-IRAP sub-layer reference VCL NAL unit types

        public const uint BLA_W_LP = 16;               // Coded slice segment of a BLA picture
        public const uint BLA_W_RADL = 17;             // Coded slice segment of a BLA picture
        public const uint BLA_N_LP = 18;               // Coded slice segment of a BLA picture
        public const uint IDR_W_RADL = 19;             // Coded slice segment of an IDR picture
        public const uint IDR_N_LP = 20;               // Coded slice segment of an IDR picture
        public const uint CRA_NUT = 21;                // Coded slice segment of a CRA picture

        public const uint RSV_IRAP_VCL22 = 22;         // Reserved IRAP VCL NAL unit types
        public const uint RSV_IRAP_VCL23 = 23;         // Reserved IRAP VCL NAL unit types
        public const uint RSV_VCL24 = 24;              // Reserved VCL NAL unit types
        public const uint RSV_VCL25 = 25;              // Reserved VCL NAL unit types
        public const uint RSV_VCL26 = 26;              // Reserved VCL NAL unit types
        public const uint RSV_VCL27 = 27;              // Reserved VCL NAL unit types
        public const uint RSV_VCL28 = 28;              // Reserved VCL NAL unit types
        public const uint RSV_VCL29 = 29;              // Reserved VCL NAL unit types
        public const uint RSV_VCL30 = 30;              // Reserved VCL NAL unit types
        public const uint RSV_VCL31 = 31;              // Reserved VCL NAL unit types

        public const uint VPS_NUT = 32;                // Video parameter set
        public const uint SPS_NUT = 33;                // Sequence parameter set
        public const uint PPS_NUT = 34;                // Picture parameter set
        public const uint AUD_NUT = 35;                // Access unit delimiter
        public const uint EOS_NUT = 36;                // End of sequence
        public const uint EOB_NUT = 37;                // End of stream
        public const uint FD_NUT = 38;                 // Filler data
        public const uint PREFIX_SEI_NUT = 39;         // Prefix SEI
        public const uint SUFFIX_SEI_NUT = 40;         // Suffix SEI

        public const uint RSV_NVCL41 = 41;             // Reserved non-VCL NAL unit types
        public const uint RSV_NVCL42 = 42;             // Reserved non-VCL NAL unit types
        public const uint RSV_NVCL43 = 43;             // Reserved non-VCL NAL unit types
        public const uint RSV_NVCL44 = 44;             // Reserved non-VCL NAL unit types
        public const uint RSV_NVCL45 = 45;             // Reserved non-VCL NAL unit types
        public const uint RSV_NVCL46 = 46;             // Reserved non-VCL NAL unit types
        public const uint RSV_NVCL47 = 47;             // Reserved non-VCL NAL unit types
        public const uint UNSPEC48 = 48;               // Unspecified NAL unit types
        public const uint UNSPEC49 = 49;               // Unspecified NAL unit types
        public const uint UNSPEC50 = 50;               // Unspecified NAL unit types
        public const uint UNSPEC51 = 51;               // Unspecified NAL unit types
        public const uint UNSPEC52 = 52;               // Unspecified NAL unit types
        public const uint UNSPEC53 = 53;               // Unspecified NAL unit types
        public const uint UNSPEC54 = 54;               // Unspecified NAL unit types
        public const uint UNSPEC55 = 55;               // Unspecified NAL unit types
        public const uint UNSPEC56 = 56;               // Unspecified NAL unit types
        public const uint UNSPEC57 = 57;               // Unspecified NAL unit types
        public const uint UNSPEC58 = 58;               // Unspecified NAL unit types
        public const uint UNSPEC59 = 59;               // Unspecified NAL unit types
        public const uint UNSPEC60 = 60;               // Unspecified NAL unit types
        public const uint UNSPEC61 = 61;               // Unspecified NAL unit types
        public const uint UNSPEC62 = 62;               // Unspecified NAL unit types
        public const uint UNSPEC63 = 63;               // Unspecified NAL unit types
    }

    public partial class H265Context
    {
        public SeiPayload SeiPayload { get; set; }

        public ulong[][][] BspSchedCnt { get; set; }
        public int[][] IdDirectRefLayer { get; set; }
        public int[][] LayerSetLayerIdList { get; set; }
        public uint[][] CpPresentFlag { get; set; }
        public int[][] DependencyFlag { get; set; }
        public int[][] IdRefLayer { get; set; }
        public int[][] IdPredictedLayer { get; set; }
        public int[][] TreePartitionLayerIdList { get; set; }
        public int[][] ScalabilityId { get; set; }
        public uint[][] OutputLayerFlag { get; set; }
        public uint[][] NecessaryLayerFlag { get; set; }
        public int[][] IdRefListLayer { get; set; }
        public uint[][] ViewCompLayerPresentFlag { get; set; }
        public int[][] ViewCompLayerId { get; set; }
        public uint[][] UsedByCurrPicS0 { get; set; }
        public uint[][] UsedByCurrPicS1 { get; set; }
        public int[][] DeltaPocS0 { get; set; }
        public int[][] DeltaPocS1 { get; set; }
        public int[] NumLayersInIdList { get; set; }
        public int[] MaxSubLayersInLayerSetMinus1 { get; set; }
        public uint[] NumDirectRefLayers { get; set; }
        public uint[] NumLayersInTreePartition { get; set; }
        public int[] layerIdInListFlag { get; set; }
        public uint[] NumRefLayers { get; set; }
        public uint[] NumPredictedLayers { get; set; }
        public int[] DependencyId { get; set; }
        public int[] AuxId { get; set; }
        public int[] DepthLayerFlag { get; set; } = new int[] { 0 };
        public int[] ViewOrderIdx { get; set; }
        public uint[] OlsIdxToLsIdx { get; set; }
        public uint[] NumOutputLayersInOutputLayerSet { get; set; }
        public uint[] OlsHighestOutputLayerId { get; set; }
        public uint[] NumNecessaryLayers { get; set; }
        public uint[] LayerIdxInVps { get; set; }
        public uint[] NumRefListLayers { get; set; } = new uint[] { 0 };
        public uint[] ViewOIdxList { get; set; }
        public ulong[] NumNegativePics { get; set; }
        public ulong[] NumPositivePics { get; set; }
        public ulong[] NumDeltaPocs { get; set; }
        public uint[] MaxTemporalId { get; set; }
        public uint MaxLayersMinus1 { get; set; }
        public ulong CpbCnt { get; set; }
        public uint NumViews { get; set; }
        public uint NumIndependentLayers { get; set; }
        public ulong NumLayerSets { get; set; }
        public ulong FirstAddLayerSetIdx { get; set; }
        public ulong LastAddLayerSetIdx { get; set; }
        public int PicSizeInCtbsY { get; set; }
        public int MinCbLog2SizeY { get; set; }
        public int CtbLog2SizeY { get; set; }
        public int MinCbSizeY { get; set; }
        public int CtbSizeY { get; set; }
        public int PicWidthInMinCbsY { get; set; }
        public int PicWidthInCtbsY { get; set; }
        public int PicHeightInMinCbsY { get; set; }
        public int PicHeightInCtbsY { get; set; }
        public int PicSizeInMinCbsY { get; set; }
        public int PicSizeInSamplesY { get; set; }
        public int PicWidthInSamplesC { get; set; }
        public int PicHeightInSamplesC { get; set; }
        public int SubWidthC { get; set; }
        public int SubHeightC { get; set; }
        public uint VclInitialArrivalDelayPresent { get; set; }
        public uint NalInitialArrivalDelayPresent { get; set; }
        public ulong RefRpsIdx { get; set; }
        public uint NumActiveRefLayerPics { get; private set; }
        public uint[] refLayerPicIdc { get; private set; }
        public uint TemporalId { get; private set; }
        public uint NumPicTotalCurr { get; private set; }
        public uint[] PocLsbLt { get; private set; }
        public uint[] UsedByCurrPicLt { get; private set; }
        public ulong CurrRpsIdx { get; private set; }
        public int inCmpPredAvailFlag { get; internal set; } // TODO

        public void SetSeiPayload(SeiPayload payload)
        {
            if (SeiPayload == null)
            {
                SeiPayload = payload;
            }

            if (payload.ActiveParameterSets != null)
                SeiPayload.ActiveParameterSets = payload.ActiveParameterSets;
            if (payload.AlphaChannelInfo != null)
                SeiPayload.AlphaChannelInfo = payload.AlphaChannelInfo;
            if (payload.AlternativeDepthInfo != null)
                SeiPayload.AlternativeDepthInfo = payload.AlternativeDepthInfo;
            if (payload.AlternativeTransferCharacteristics != null)
                SeiPayload.AlternativeTransferCharacteristics = payload.AlternativeTransferCharacteristics;
            if (payload.AmbientViewingEnvironment != null)
                SeiPayload.AmbientViewingEnvironment = payload.AmbientViewingEnvironment;
            if (payload.BspInitialArrivalTime != null)
                SeiPayload.BspInitialArrivalTime = payload.BspInitialArrivalTime;
            if (payload.BspNesting != null)
                SeiPayload.BspNesting = payload.BspNesting;
            if (payload.BufferingPeriod != null)
                SeiPayload.BufferingPeriod = payload.BufferingPeriod;
            if (payload.ChromaResamplingFilterHint != null)
                SeiPayload.ChromaResamplingFilterHint = payload.ChromaResamplingFilterHint;
            if (payload.CodedRegionCompletion != null)
                SeiPayload.CodedRegionCompletion = payload.CodedRegionCompletion;
            if (payload.ColourRemappingInfo != null)
                SeiPayload.ColourRemappingInfo = payload.ColourRemappingInfo;
            if (payload.ContentColourVolume != null)
                SeiPayload.ContentColourVolume = payload.ContentColourVolume;
            if (payload.ContentLightLevelInfo != null)
                SeiPayload.ContentLightLevelInfo = payload.ContentLightLevelInfo;
            if (payload.CubemapProjection != null)
                SeiPayload.CubemapProjection = payload.CubemapProjection;
            if (payload.DecodedPictureHash != null)
                SeiPayload.DecodedPictureHash = payload.DecodedPictureHash;
            if (payload.DecodingUnitInfo != null)
                SeiPayload.DecodingUnitInfo = payload.DecodingUnitInfo;
            if (payload.DeinterlacedFieldIdentification != null)
                SeiPayload.DeinterlacedFieldIdentification = payload.DeinterlacedFieldIdentification;
            if (payload.DependentRapIndication != null)
                SeiPayload.DependentRapIndication = payload.DependentRapIndication;
            if (payload.DepthRepresentationInfo != null)
                SeiPayload.DepthRepresentationInfo = payload.DepthRepresentationInfo;
            if (payload.DisplayOrientation != null)
                SeiPayload.DisplayOrientation = payload.DisplayOrientation;
            if (payload.EquirectangularProjection != null)
                SeiPayload.EquirectangularProjection = payload.EquirectangularProjection;
            if (payload.FillerPayload != null)
                SeiPayload.FillerPayload = payload.FillerPayload;
            if (payload.FilmGrainCharacteristics != null)
                SeiPayload.FilmGrainCharacteristics = payload.FilmGrainCharacteristics;
            if (payload.FrameFieldInfo != null)
                SeiPayload.FrameFieldInfo = payload.FrameFieldInfo;
            if (payload.FramePackingArrangement != null)
                SeiPayload.FramePackingArrangement = payload.FramePackingArrangement;
            if (payload.GreenMetadata != null)
                SeiPayload.GreenMetadata = payload.GreenMetadata;
            if (payload.InterLayerConstrainedTileSets != null)
                SeiPayload.InterLayerConstrainedTileSets = payload.InterLayerConstrainedTileSets;
            if (payload.KneeFunctionInfo != null)
                SeiPayload.KneeFunctionInfo = payload.KneeFunctionInfo;
            if (payload.LayersNotPresent != null)
                SeiPayload.LayersNotPresent = payload.LayersNotPresent;
            if (payload.MasteringDisplayColourVolume != null)
                SeiPayload.MasteringDisplayColourVolume = payload.MasteringDisplayColourVolume;
            if (payload.MctsExtractionInfoNesting != null)
                SeiPayload.MctsExtractionInfoNesting = payload.MctsExtractionInfoNesting;
            if (payload.MctsExtractionInfoSets != null)
                SeiPayload.MctsExtractionInfoSets = payload.MctsExtractionInfoSets;
            if (payload.MultiviewAcquisitionInfo != null)
                SeiPayload.MultiviewAcquisitionInfo = payload.MultiviewAcquisitionInfo;
            if (payload.MultiviewSceneInfo != null)
                SeiPayload.MultiviewSceneInfo = payload.MultiviewSceneInfo;
            if (payload.MultiviewViewPosition != null)
                SeiPayload.MultiviewViewPosition = payload.MultiviewViewPosition;
            if (payload.NoDisplay != null)
                SeiPayload.NoDisplay = payload.NoDisplay;
            if (payload.OmniViewport != null)
                SeiPayload.OmniViewport = payload.OmniViewport;
            if (payload.OverlayInfo != null)
                SeiPayload.OverlayInfo = payload.OverlayInfo;
            if (payload.PanScanRect != null)
                SeiPayload.PanScanRect = payload.PanScanRect;
            if (payload.PicTiming != null)
                SeiPayload.PicTiming = payload.PicTiming;
            if (payload.PictureSnapshot != null)
                SeiPayload.PictureSnapshot = payload.PictureSnapshot;
            if (payload.PostFilterHint != null)
                SeiPayload.PostFilterHint = payload.PostFilterHint;
            if (payload.ProgressiveRefinementSegmentEnd != null)
                SeiPayload.ProgressiveRefinementSegmentEnd = payload.ProgressiveRefinementSegmentEnd;
            if (payload.ProgressiveRefinementSegmentStart != null)
                SeiPayload.ProgressiveRefinementSegmentStart = payload.ProgressiveRefinementSegmentStart;
            if (payload.RecoveryPoint != null)
                SeiPayload.RecoveryPoint = payload.RecoveryPoint;
            if (payload.RegionalNesting != null)
                SeiPayload.RegionalNesting = payload.RegionalNesting;
            if (payload.RegionRefreshInfo != null)
                SeiPayload.RegionRefreshInfo = payload.RegionRefreshInfo;
            if (payload.RegionwisePacking != null)
                SeiPayload.RegionwisePacking = payload.RegionwisePacking;
            if (payload.ReservedSeiMessage != null)
                SeiPayload.ReservedSeiMessage = payload.ReservedSeiMessage;
            if (payload.ScalableNesting != null)
                SeiPayload.ScalableNesting = payload.ScalableNesting;
            if (payload.SceneInfo != null)
                SeiPayload.SceneInfo = payload.SceneInfo;
            if (payload.SegmentedRectFramePackingArrangement != null)
                SeiPayload.SegmentedRectFramePackingArrangement = payload.SegmentedRectFramePackingArrangement;
            if (payload.SphereRotation != null)
                SeiPayload.SphereRotation = payload.SphereRotation;
            if (payload.StructureOfPicturesInfo != null)
                SeiPayload.StructureOfPicturesInfo = payload.StructureOfPicturesInfo;
            if (payload.SubBitstreamProperty != null)
                SeiPayload.SubBitstreamProperty = payload.SubBitstreamProperty;
            if (payload.TemporalMotionConstrainedTileSets != null)
                SeiPayload.TemporalMotionConstrainedTileSets = payload.TemporalMotionConstrainedTileSets;
            if (payload.TemporalMvPredictionConstraints != null)
                SeiPayload.TemporalMvPredictionConstraints = payload.TemporalMvPredictionConstraints;
            if (payload.TemporalSubLayerZeroIdx != null)
                SeiPayload.TemporalSubLayerZeroIdx = payload.TemporalSubLayerZeroIdx;
            if (payload.ThreeDimensionalReferenceDisplaysInfo != null)
                SeiPayload.ThreeDimensionalReferenceDisplaysInfo = payload.ThreeDimensionalReferenceDisplaysInfo;
            if (payload.TimeCode != null)
                SeiPayload.TimeCode = payload.TimeCode;
            if (payload.ToneMappingInfo != null)
                SeiPayload.ToneMappingInfo = payload.ToneMappingInfo;
            if (payload.UserDataRegisteredItutT35 != null)
                SeiPayload.UserDataRegisteredItutT35 = payload.UserDataRegisteredItutT35;
            if (payload.UserDataUnregistered != null)
                SeiPayload.UserDataUnregistered = payload.UserDataUnregistered;
        }

        public void OnDeltaIdxMinus1(ulong stRpsIdx)
        {
            var delta_idx_minus1 = SeqParameterSetRbsp.StRefPicSet[stRpsIdx].DeltaIdxMinus1;
            RefRpsIdx = stRpsIdx - (delta_idx_minus1 + 1); // 7-59
        }

        public void OnNestingMaxTemporalIdPlus1(uint i)
        {
            var nesting_max_temporal_id_plus1 = SeiPayload.ScalableNesting.NestingMaxTemporalIdPlus1;
            MaxTemporalId[i] = nesting_max_temporal_id_plus1[i] - 1;
        }

        public void OnUsedByCurrPicS0Flag(uint i, ulong stRpsIdx, StRefPicSet st_ref_pic_set)
        {
            var num_negative_pics = st_ref_pic_set.NumNegativePics;
            var used_by_curr_pic_s0_flag = st_ref_pic_set.UsedByCurrPicS0Flag;
            var delta_poc_s0_minus1 = st_ref_pic_set.DeltaPocS0Minus1;

            if (NumNegativePics == null || NumNegativePics.Length <= (int)stRpsIdx)
                NumNegativePics = new ulong[stRpsIdx + 1];
            if (NumDeltaPocs == null || NumDeltaPocs.Length <= (int)stRpsIdx)
                NumDeltaPocs = new ulong[stRpsIdx + 1];

            NumNegativePics[stRpsIdx] = num_negative_pics; // 7-63

            if (UsedByCurrPicS0 == null || UsedByCurrPicS0.Length <= (int)stRpsIdx)
                UsedByCurrPicS0 = new uint[stRpsIdx + 1][];
            if (UsedByCurrPicS0[stRpsIdx] == null || UsedByCurrPicS0[stRpsIdx].Length < (int)num_negative_pics)
                UsedByCurrPicS0[stRpsIdx] = new uint[num_negative_pics];

            if (DeltaPocS0 == null || DeltaPocS0.Length <= (int)stRpsIdx)
                DeltaPocS0 = new int[stRpsIdx + 1][];
            if (DeltaPocS0[stRpsIdx] == null || DeltaPocS0[stRpsIdx].Length < (int)num_negative_pics)
                DeltaPocS0[stRpsIdx] = new int[num_negative_pics];

            UsedByCurrPicS0[stRpsIdx][i] = used_by_curr_pic_s0_flag[i]; // 7-65
            if (i == 0)
            {
                DeltaPocS0[stRpsIdx][i] = -((int)delta_poc_s0_minus1[i] + 1); // 7-67
            }
            else
            {
                DeltaPocS0[stRpsIdx][i] = DeltaPocS0[stRpsIdx][i - 1] - ((int)delta_poc_s0_minus1[i] + 1); // 7-69
            }
        }

        public void OnUsedByCurrPicS1Flag(uint i, ulong stRpsIdx, StRefPicSet st_ref_pic_set)
        {
            var num_positive_pics = st_ref_pic_set.NumPositivePics;
            var used_by_curr_pic_s1_flag = st_ref_pic_set.UsedByCurrPicS1Flag;
            var delta_poc_s1_minus1 = st_ref_pic_set.DeltaPocS1Minus1;

            if (NumPositivePics == null || NumPositivePics.Length <= (int)stRpsIdx)
                NumPositivePics = new ulong[stRpsIdx + 1];
            if (NumDeltaPocs == null || NumDeltaPocs.Length <= (int)stRpsIdx)
                NumDeltaPocs = new ulong[stRpsIdx + 1];

            NumPositivePics[stRpsIdx] = num_positive_pics; // 7-64

            if (UsedByCurrPicS1 == null || UsedByCurrPicS1.Length <= (int)stRpsIdx)
                UsedByCurrPicS1 = new uint[stRpsIdx + 1][];
            if (UsedByCurrPicS1[stRpsIdx] == null || UsedByCurrPicS1[stRpsIdx].Length < (int)num_positive_pics)
                UsedByCurrPicS1[stRpsIdx] = new uint[num_positive_pics];

            if (DeltaPocS1 == null || DeltaPocS1.Length <= (int)stRpsIdx)
                DeltaPocS1 = new int[stRpsIdx + 1][];
            if (DeltaPocS1[stRpsIdx] == null || DeltaPocS1[stRpsIdx].Length < (int)num_positive_pics)
                DeltaPocS1[stRpsIdx] = new int[num_positive_pics];

            UsedByCurrPicS1[stRpsIdx][i] = used_by_curr_pic_s1_flag[i]; // 7-66
            if (i == 0)
            {
                DeltaPocS1[stRpsIdx][i] = (int)delta_poc_s1_minus1[i] + 1; // 7-68
            }
            else
            {
                DeltaPocS1[stRpsIdx][i] = DeltaPocS1[stRpsIdx][i - 1] + ((int)delta_poc_s1_minus1[i] + 1); // 7-70
            }
            NumDeltaPocs[stRpsIdx] = NumNegativePics[stRpsIdx] + NumPositivePics[stRpsIdx]; // 7-71
        }

        public void OnNumBspSchedulesMinus1(uint h, uint i, uint t)
        {
            var num_signalled_partitioning_schemes = VideoParameterSetRbsp.VpsExtension.VpsVui.VpsVuiBspHrdParams.NumSignalledPartitioningSchemes[h];

            if (BspSchedCnt == null ||
                BspSchedCnt.Length < (int)(num_signalled_partitioning_schemes + 1) ||
                BspSchedCnt[h].Length < (int)(num_signalled_partitioning_schemes + 1))
            {
                BspSchedCnt = new ulong[num_signalled_partitioning_schemes + 1][][];
                for (int j = 0; j < (int)num_signalled_partitioning_schemes + 1; j++)
                {
                    BspSchedCnt[j] = new ulong[num_signalled_partitioning_schemes + 1][];
                    for (int k = 0; k < (int)num_signalled_partitioning_schemes + 1; k++)
                    {
                        BspSchedCnt[j][k] = new ulong[MaxSubLayersInLayerSetMinus1[OlsIdxToLsIdx[h]] + 1];
                    }
                }
            }

            var num_bsp_schedules_minus1 = VideoParameterSetRbsp.VpsExtension.VpsVui.VpsVuiBspHrdParams.NumBspSchedulesMinus1;
            BspSchedCnt[h][i][t] = num_bsp_schedules_minus1[h][i][t] + 1;
        }

        public void OnNalHrdParametersPresentFlag(uint value)
        {
            NalInitialArrivalDelayPresent = value;
        }

        public void OnVclHrdParametersPresentFlag(uint value)
        {
            VclInitialArrivalDelayPresent = value;
        }

        public void OnLayerSetIdxForOlsMinus1(uint i, ulong NumOutputLayerSets) // F-11
        {
            var layer_set_idx_for_ols_minus1 = VideoParameterSetRbsp.VpsExtension.LayerSetIdxForOlsMinus1;

            if (OlsIdxToLsIdx == null || OlsIdxToLsIdx.Length < (int)NumOutputLayerSets)
            {
                OlsIdxToLsIdx = new uint[NumOutputLayerSets];
            }

            OlsIdxToLsIdx[i] = (i < NumLayerSets) ? i : (layer_set_idx_for_ols_minus1[i] + 1);
        }

        public void OnLayerIdInNuh(uint i)
        {
            if (LayerIdxInVps == null || LayerIdxInVps.Length < Math.Min(62, VideoParameterSetRbsp.VpsMaxLayersMinus1) + 1)
            {
                LayerIdxInVps = new uint[Math.Min(62, VideoParameterSetRbsp.VpsMaxLayersMinus1) + 1];
            }

            var layer_id_in_nuh = VideoParameterSetRbsp.VpsExtension.LayerIdInNuh;
            LayerIdxInVps[layer_id_in_nuh[i]] = i;
        }

        public void OnOutputLayerFlag(uint ii, uint jj)
        {
            var NumOutputLayerSets = VideoParameterSetRbsp.VpsExtension.NumOutputLayerSets;
            var default_output_layer_idc = VideoParameterSetRbsp.VpsExtension.DefaultOutputLayerIdc;
            var output_layer_flag = VideoParameterSetRbsp.VpsExtension.OutputLayerFlag;
            var vps_num_layer_sets_minus1 = VideoParameterSetRbsp.VpsNumLayerSetsMinus1;

            var defaultOutputLayerIdc = Math.Min(default_output_layer_idc, 2);

            if (OutputLayerFlag == null || OutputLayerFlag.Length < (int)vps_num_layer_sets_minus1 + 1)
            {
                OutputLayerFlag = new uint[vps_num_layer_sets_minus1 + 1][];
                for (int i = 0; i < (int)vps_num_layer_sets_minus1 + 1; i++)
                {
                    OutputLayerFlag[i] = new uint[NumLayersInIdList[OlsIdxToLsIdx[i]]];
                }
            }
            if (NumOutputLayersInOutputLayerSet == null || NumOutputLayersInOutputLayerSet.Length < (int)vps_num_layer_sets_minus1 + 1)
                NumOutputLayersInOutputLayerSet = new uint[vps_num_layer_sets_minus1 + 1];
            if (OlsHighestOutputLayerId == null || OlsHighestOutputLayerId.Length < (int)vps_num_layer_sets_minus1 + 1)
                OlsHighestOutputLayerId = new uint[vps_num_layer_sets_minus1 + 1];
            if (NecessaryLayerFlag == null || NecessaryLayerFlag.Length < (int)NumOutputLayerSets)
            {
                NecessaryLayerFlag = new uint[NumOutputLayerSets][];
                for (int i = 0; i < (int)NumOutputLayerSets; i++)
                {
                    NecessaryLayerFlag[i] = new uint[NumLayersInIdList[OlsIdxToLsIdx[i]]];
                }
            }
            if (NumNecessaryLayers == null || NumNecessaryLayers.Length < (int)NumOutputLayerSets)
                NumNecessaryLayers = new uint[NumOutputLayerSets];

            if (defaultOutputLayerIdc == 0 || defaultOutputLayerIdc == 1)
            {
                for (int i = 0; i <= (int)vps_num_layer_sets_minus1; i++)
                {
                    int nuhLayerIdA = LayerSetLayerIdList[OlsIdxToLsIdx[i]][0]; // highest value in LayerSetLayerIdList[OlsIdxToLsIdx[i]]
                    for (int k = 0; k < LayerSetLayerIdList[OlsIdxToLsIdx[i]].Length; k++)
                    {
                        if (LayerSetLayerIdList[OlsIdxToLsIdx[i]][k] > nuhLayerIdA)
                            nuhLayerIdA = LayerSetLayerIdList[OlsIdxToLsIdx[i]][k];
                    }

                    for (int j = 0; j < NumLayersInIdList[OlsIdxToLsIdx[i]] - 1; j++)
                    {
                        if (defaultOutputLayerIdc == 0 ||
                             LayerSetLayerIdList[OlsIdxToLsIdx[i]][j] == nuhLayerIdA)
                            OutputLayerFlag[i][j] = 1;
                        else
                            OutputLayerFlag[i][j] = 0;
                    }
                }
            }

            for (uint i = ((defaultOutputLayerIdc == 2) ? 0 : ((uint)vps_num_layer_sets_minus1 + 1)); i <= NumOutputLayerSets - 1; i++)
            {
                for (int j = 0; j <= NumLayersInIdList[OlsIdxToLsIdx[i]] - 1; j++)
                {
                    OutputLayerFlag[i][j] = output_layer_flag[i][j];
                }

                NumOutputLayersInOutputLayerSet[i] = 0;
                for (int j = 0; j < NumLayersInIdList[OlsIdxToLsIdx[i]]; j++)
                {
                    NumOutputLayersInOutputLayerSet[i] += OutputLayerFlag[i][j];
                    if (OutputLayerFlag[i][j] != 0)
                        OlsHighestOutputLayerId[i] = (uint)LayerSetLayerIdList[OlsIdxToLsIdx[i]][j];
                }
            }

            for (int olsIdx = 0; olsIdx < (int)NumOutputLayerSets; olsIdx++)
            {
                uint lsIdx = OlsIdxToLsIdx[olsIdx];
                for (int lsLayerIdx = 0; lsLayerIdx < NumLayersInIdList[lsIdx]; lsLayerIdx++)
                    NecessaryLayerFlag[olsIdx][lsLayerIdx] = 0;
                for (int lsLayerIdx = 0; lsLayerIdx < NumLayersInIdList[lsIdx]; lsLayerIdx++)
                    if (OutputLayerFlag[olsIdx][lsLayerIdx] != 0)
                    {
                        NecessaryLayerFlag[olsIdx][lsLayerIdx] = 1;
                        int currLayerId = LayerSetLayerIdList[lsIdx][lsLayerIdx];
                        for (int rLsLayerIdx = 0; rLsLayerIdx < lsLayerIdx; rLsLayerIdx++)
                        {
                            int refLayerId = LayerSetLayerIdList[lsIdx][rLsLayerIdx];
                            if (DependencyFlag[LayerIdxInVps[currLayerId]][LayerIdxInVps[refLayerId]] != 0)
                                NecessaryLayerFlag[olsIdx][rLsLayerIdx] = 1;
                        }
                        NumNecessaryLayers[olsIdx] = 0;
                        for (lsLayerIdx = 0; lsLayerIdx < NumLayersInIdList[lsIdx]; lsLayerIdx++)
                            NumNecessaryLayers[olsIdx] += NecessaryLayerFlag[olsIdx][lsLayerIdx];
                    }
            }
        }

        public void OnLog2DiffMaxMinLumaCodingBlockSize()
        {
            var separate_colour_plane_flag = SeqParameterSetRbsp.SeparateColourPlaneFlag;
            var chroma_format_idc = SeqParameterSetRbsp.ChromaFormatIdc;
            var log2_min_luma_coding_block_size_minus3 = SeqParameterSetRbsp.Log2MinLumaCodingBlockSizeMinus3;
            var log2_diff_max_min_luma_coding_block_size = SeqParameterSetRbsp.Log2DiffMaxMinLumaCodingBlockSize;
            var pic_width_in_luma_samples = SeqParameterSetRbsp.PicWidthInLumaSamples;
            var pic_height_in_luma_samples = SeqParameterSetRbsp.PicHeightInLumaSamples;

            // TODO: this shoud happen at the beginning of the decoding process
            if (separate_colour_plane_flag == 0)
            {
                if (chroma_format_idc == 0)
                {
                    SubWidthC = 1;
                    SubHeightC = 1;
                }
                else if (chroma_format_idc == 1)
                {
                    SubWidthC = 2;
                    SubHeightC = 2;
                }
                else if (chroma_format_idc == 2)
                {
                    SubWidthC = 2;
                    SubHeightC = 1;
                }
                else if (chroma_format_idc == 3)
                {
                    SubWidthC = 1;
                    SubHeightC = 1;
                }
            }
            else
            {
                SubWidthC = 1;
                SubHeightC = 1;
            }

            MinCbLog2SizeY = (int)log2_min_luma_coding_block_size_minus3 + 3; // 7-10
            CtbLog2SizeY = MinCbLog2SizeY + (int)log2_diff_max_min_luma_coding_block_size; // 7-11
            MinCbSizeY = 1 << MinCbLog2SizeY; // 7-12
            CtbSizeY = 1 << CtbLog2SizeY; // 7-13
            PicWidthInMinCbsY = (int)pic_width_in_luma_samples / MinCbSizeY; // 7-14
            PicWidthInCtbsY = (int)Math.Ceiling((double)pic_width_in_luma_samples / CtbSizeY); // 7-15
            PicHeightInMinCbsY = (int)pic_height_in_luma_samples / MinCbSizeY; // 7-16
            PicHeightInCtbsY = (int)Math.Ceiling((double)pic_height_in_luma_samples / CtbSizeY); // 7-17
            PicSizeInMinCbsY = PicWidthInMinCbsY * PicHeightInMinCbsY; // 7-18
            PicSizeInCtbsY = PicWidthInCtbsY * PicHeightInCtbsY; // 7-19
            PicSizeInSamplesY = (int)pic_width_in_luma_samples * (int)pic_height_in_luma_samples; // 7-20
            PicWidthInSamplesC = (int)pic_width_in_luma_samples / SubWidthC; // 7-21
            PicHeightInSamplesC = (int)pic_height_in_luma_samples / SubHeightC; // 7-22
        }

        public void OnSubLayersVpsMaxMinus1()
        {
            var sub_layers_vps_max_minus1 = VideoParameterSetRbsp.VpsExtension.SubLayersVpsMaxMinus1;

            if (MaxSubLayersInLayerSetMinus1 == null || MaxSubLayersInLayerSetMinus1.Length < (int)NumLayerSets)
                MaxSubLayersInLayerSetMinus1 = new int[NumLayerSets];

            for (int i = 0; i < (int)NumLayerSets; i++)
            {
                uint maxSlMinus1 = 0;
                for (int k = 0; k < NumLayersInIdList[i]; k++)
                {
                    int lId = LayerSetLayerIdList[i][k];
                    maxSlMinus1 = Math.Max(maxSlMinus1, sub_layers_vps_max_minus1[LayerIdxInVps[lId]]);
                }
                MaxSubLayersInLayerSetMinus1[i] = (int)maxSlMinus1;
            }
        }

        public void OnVpsMaxLayersMinus1()
        {
            var vps_max_layers_minus1 = VideoParameterSetRbsp.VpsMaxLayersMinus1;
            MaxLayersMinus1 = Math.Min(62, vps_max_layers_minus1);
        }

        public void OnCpbCntMinus1(uint i)
        {
            var cpb_cnt_minus1 = SeqParameterSetRbsp.VuiParameters.HrdParameters.CpbCntMinus1;
            CpbCnt = cpb_cnt_minus1[i] + 1; // E.3.3
        }

        public void OnNumAddLayerSets()
        {
            var vps_num_layer_sets_minus1 = VideoParameterSetRbsp.VpsNumLayerSetsMinus1;
            var num_add_layer_sets = VideoParameterSetRbsp.VpsExtension.NumAddLayerSets;

            NumLayerSets = vps_num_layer_sets_minus1 + 1 + num_add_layer_sets; // F-7
            if (num_add_layer_sets > 0)
            {
                // F-8
                FirstAddLayerSetIdx = vps_num_layer_sets_minus1 + 1;
                LastAddLayerSetIdx = FirstAddLayerSetIdx + num_add_layer_sets - 1;
            }
        }

        public void OnHighestLayerIdxPlus1(uint i) // F-9
        {
            var num_add_layer_sets = VideoParameterSetRbsp.VpsExtension.NumAddLayerSets;
            var vps_num_layer_sets_minus1 = VideoParameterSetRbsp.VpsNumLayerSetsMinus1;
            var highest_layer_idx_plus1 = VideoParameterSetRbsp.VpsExtension.HighestLayerIdxPlus1;

            if (LayerSetLayerIdList == null || LayerSetLayerIdList.Length < (int)vps_num_layer_sets_minus1 + 1 + (int)num_add_layer_sets)
            {
                LayerSetLayerIdList = new int[vps_num_layer_sets_minus1 + 1 + num_add_layer_sets][];
            }

            int layerNum = 0;
            uint lsIdx = (uint)vps_num_layer_sets_minus1 + 1 + i;
            for (int treeIdx = 1; treeIdx < NumIndependentLayers; treeIdx++)
            {
                if (LayerSetLayerIdList[lsIdx] == null || LayerSetLayerIdList[lsIdx].Length < (NumIndependentLayers * (highest_layer_idx_plus1[i][treeIdx])))
                {
                    LayerSetLayerIdList[lsIdx] = new int[layerNum + 1];
                }

                for (int layerCnt = 0; layerCnt < highest_layer_idx_plus1[i][treeIdx]; layerCnt++)
                {
                    LayerSetLayerIdList[lsIdx][layerNum++] = TreePartitionLayerIdList[treeIdx][layerCnt];
                }
            }
            NumLayersInIdList[lsIdx] = layerNum;
        }

        public void OnDimensionId() // F-3
        {
            var dimension_id = VideoParameterSetRbsp.VpsExtension.DimensionId;
            var layer_id_in_nuh = VideoParameterSetRbsp.VpsExtension.LayerIdInNuh;
            var scalability_mask_flag = VideoParameterSetRbsp.VpsExtension.ScalabilityMaskFlag;

            if (ScalabilityId == null || ScalabilityId.Length < MaxLayersMinus1 + 1)
            {
                ScalabilityId = new int[MaxLayersMinus1 + 1][];
                for (int i = 0; i < MaxLayersMinus1 + 1; i++)
                {
                    ScalabilityId[i] = new int[16];
                }
            }
            if (DepthLayerFlag == null || DepthLayerFlag.Length < MaxLayersMinus1 + 1)
                DepthLayerFlag = new int[MaxLayersMinus1 + 1];
            if (ViewOrderIdx == null || ViewOrderIdx.Length < MaxLayersMinus1 + 1)
                ViewOrderIdx = new int[MaxLayersMinus1 + 1];
            if (DependencyId == null || DependencyId.Length < MaxLayersMinus1 + 1)
                DependencyId = new int[MaxLayersMinus1 + 1];
            if (AuxId == null || AuxId.Length < MaxLayersMinus1 + 1)
                AuxId = new int[MaxLayersMinus1 + 1];

            NumViews = 1;
            for (int i = 0; i <= MaxLayersMinus1; i++)
            {
                uint lId = layer_id_in_nuh[i];
                for (int smIdx = 0, j = 0; smIdx < 16; smIdx++)
                {
                    if (scalability_mask_flag[smIdx] != 0)
                        ScalabilityId[i][smIdx] = (int)dimension_id[i][j++];
                    else
                        ScalabilityId[i][smIdx] = 0;
                }
                DepthLayerFlag[lId] = ScalabilityId[i][0];
                ViewOrderIdx[lId] = ScalabilityId[i][1];
                DependencyId[lId] = ScalabilityId[i][2];
                AuxId[lId] = ScalabilityId[i][3];
                if (i > 0)
                {
                    uint newViewFlag = 1;
                    for (int j = 0; j < i; j++)
                        if (ViewOrderIdx[lId] == ViewOrderIdx[layer_id_in_nuh[j]])
                            newViewFlag = 0;
                    NumViews += newViewFlag;
                }
            }
        }

        public void OnDirectDependencyType()
        {
            var layer_id_in_nuh = VideoParameterSetRbsp.VpsExtension.LayerIdInNuh;

            if (ViewOIdxList == null || ViewOIdxList.Length < MaxLayersMinus1 + 1)
                ViewOIdxList = new uint[MaxLayersMinus1 + 1];
            if (NumRefListLayers == null || NumRefListLayers.Length < MaxLayersMinus1 + 1)
                NumRefListLayers = new uint[MaxLayersMinus1 + 1];
            if (IdRefListLayer == null || IdRefListLayer.Length < MaxLayersMinus1 + 1)
            {
                IdRefListLayer = new int[MaxLayersMinus1 + 1][];
                for (int i = 0; i < MaxLayersMinus1 + 1; i++)
                {
                    IdRefListLayer[i] = new int[MaxLayersMinus1 + 1];
                }
            }
            if (ViewCompLayerPresentFlag == null)
            {
                ViewCompLayerPresentFlag = new uint[MaxLayersMinus1 + 1][];
                for (int i = 0; i < MaxLayersMinus1 + 1; i++)
                {
                    ViewCompLayerPresentFlag[i] = new uint[2];
                }
            }
            if (ViewCompLayerId == null)
            {
                ViewCompLayerId = new int[MaxLayersMinus1 + 1][];
                for (int i = 0; i < MaxLayersMinus1 + 1; i++)
                {
                    ViewCompLayerId[i] = new int[2];
                }
            }

            // I-7
            int idx = 0;
            ViewOIdxList[idx++] = 0;
            for (int i = 1; i <= MaxLayersMinus1; i++)
            {
                uint lId = layer_id_in_nuh[i];
                int newViewFlag = 1;
                for (int j = 0; j < i; j++)
                    if (ViewOrderIdx[layer_id_in_nuh[i]] == ViewOrderIdx[layer_id_in_nuh[j]])
                        newViewFlag = 0;
                if (newViewFlag != 0)
                    ViewOIdxList[idx++] = (uint)ViewOrderIdx[lId];
            }

            // I-8
            for (int i = 0; i <= MaxLayersMinus1; i++)
            {
                uint iNuhLId = layer_id_in_nuh[i];
                NumRefListLayers[iNuhLId] = 0;
                for (int j = 0; j < NumDirectRefLayers[iNuhLId]; j++)
                {
                    int jNuhLId = IdDirectRefLayer[iNuhLId][j];
                    if (DepthLayerFlag[iNuhLId] == DepthLayerFlag[jNuhLId])
                        IdRefListLayer[iNuhLId][NumRefListLayers[iNuhLId]++] = jNuhLId;
                }
            }

            // I-9
            for (int depFlag = 0; depFlag <= 1; depFlag++)
            {
                for (int i = 0; i < NumViews; i++)
                {
                    uint iViewOIdx = ViewOIdxList[i];
                    int layerId = -1;
                    for (int j = 0; j <= MaxLayersMinus1; j++)
                    {
                        int jNuhLId = (int)layer_id_in_nuh[j];
                        if (DepthLayerFlag[jNuhLId] == depFlag && ViewOrderIdx[jNuhLId] == iViewOIdx && DependencyId[jNuhLId] == 0 && AuxId[jNuhLId] == 0)
                            layerId = jNuhLId;

                    }
                    ViewCompLayerPresentFlag[iViewOIdx][depFlag] = (layerId != -1) ? 1u : 0u;
                    ViewCompLayerId[iViewOIdx][depFlag] = layerId;
                }
            }
        }

        public void OnDirectDependencyFlag()
        {
            var direct_dependency_flag = VideoParameterSetRbsp.VpsExtension.DirectDependencyFlag;
            var layer_id_in_nuh = VideoParameterSetRbsp.VpsExtension.LayerIdInNuh;


            uint k = 0;

            // F-4
            if (DependencyFlag == null || DependencyFlag.Length < MaxLayersMinus1 + 1)
            {
                DependencyFlag = new int[MaxLayersMinus1 + 1][];
                for (int i = 0; i < MaxLayersMinus1 + 1; i++)
                {
                    DependencyFlag[i] = new int[MaxLayersMinus1 + 1];
                }
            }

            for (int i = 0; i <= MaxLayersMinus1; i++)
            {
                for (int j = 0; j <= MaxLayersMinus1; j++)
                {
                    DependencyFlag[i][j] = direct_dependency_flag[i][j];
                    for (k = 0; k < i; k++)
                        if (direct_dependency_flag[i][k] != 0 && DependencyFlag[k][j] != 0)
                            DependencyFlag[i][j] = 1;
                }
            }

            // F-5
            if (IdDirectRefLayer == null || IdDirectRefLayer.Length < MaxLayersMinus1 + 1)
            {
                IdDirectRefLayer = new int[MaxLayersMinus1 + 1][];
                for (int i = 0; i < MaxLayersMinus1 + 1; i++)
                {
                    IdDirectRefLayer[i] = new int[MaxLayersMinus1 + 1];
                }
            }
            if (IdRefLayer == null || IdRefLayer.Length < MaxLayersMinus1 + 1)
            {
                IdRefLayer = new int[MaxLayersMinus1 + 1][];
                for (int i = 0; i < MaxLayersMinus1 + 1; i++)
                {
                    IdRefLayer[i] = new int[MaxLayersMinus1 + 1];
                }
            }
            if (IdPredictedLayer == null || IdPredictedLayer.Length < MaxLayersMinus1 + 1)
            {
                IdPredictedLayer = new int[MaxLayersMinus1 + 1][];
                for (int i = 0; i < MaxLayersMinus1 + 1; i++)
                {
                    IdPredictedLayer[i] = new int[MaxLayersMinus1 + 1];
                }
            }
            if (NumDirectRefLayers == null || NumDirectRefLayers.Length < MaxLayersMinus1 + 1)
            {
                NumDirectRefLayers = new uint[MaxLayersMinus1 + 1];
            }
            if (NumRefLayers == null || NumRefLayers.Length < MaxLayersMinus1 + 1)
            {
                NumRefLayers = new uint[MaxLayersMinus1 + 1];
            }
            if (NumPredictedLayers == null || NumPredictedLayers.Length < MaxLayersMinus1 + 1)
            {
                NumPredictedLayers = new uint[MaxLayersMinus1 + 1];
            }

            for (int i = 0; i <= MaxLayersMinus1; i++)
            {
                int iNuhLId = (int)layer_id_in_nuh[i];
                int j = 0, d = 0, r = 0, p = 0;
                for (j = 0, d = 0, r = 0, p = 0; j <= MaxLayersMinus1; j++)
                {
                    int jNuhLid = (int)layer_id_in_nuh[j];
                    if (direct_dependency_flag[i][j] != 0)
                        IdDirectRefLayer[iNuhLId][d++] = jNuhLid;
                    if (DependencyFlag[i][j] != 0)
                        IdRefLayer[iNuhLId][r++] = jNuhLid;
                    if (DependencyFlag[j][i] != 0)
                        IdPredictedLayer[iNuhLId][p++] = jNuhLid;
                }
                NumDirectRefLayers[iNuhLId] = (uint)d;
                NumRefLayers[iNuhLId] = (uint)r;
                NumPredictedLayers[iNuhLId] = (uint)p;
            }

            k = 0;

            // F-6
            if (layerIdInListFlag == null || layerIdInListFlag.Length < 64)
                layerIdInListFlag = new int[64];

            if (TreePartitionLayerIdList == null || TreePartitionLayerIdList.Length < MaxLayersMinus1 + 1)
            {
                TreePartitionLayerIdList = new int[MaxLayersMinus1 + 1][];
                for (int i = 0; i < MaxLayersMinus1 + 1; i++)
                {
                    TreePartitionLayerIdList[i] = new int[MaxLayersMinus1 + 1];
                }
            }
            if (NumLayersInTreePartition == null || NumLayersInTreePartition.Length < MaxLayersMinus1 + 1)
            {
                NumLayersInTreePartition = new uint[MaxLayersMinus1 + 1];
            }

            for (int i = 0; i <= 63; i++)
                layerIdInListFlag[i] = 0;
            for (int i = 0; i <= MaxLayersMinus1; i++)
            {
                int iNuhLId = (int)layer_id_in_nuh[i];
                if (NumDirectRefLayers[iNuhLId] == 0)
                {
                    uint h = 0;
                    TreePartitionLayerIdList[k][0] = iNuhLId;
                    for (int j = 0; j < NumPredictedLayers[iNuhLId]; j++)
                    {
                        int predLId = IdPredictedLayer[iNuhLId][j];
                        if (layerIdInListFlag[predLId] == 0)
                        {
                            TreePartitionLayerIdList[k][h++] = predLId;
                            layerIdInListFlag[predLId] = 1;
                        }
                    }
                    NumLayersInTreePartition[k++] = h;
                }
            }
            NumIndependentLayers = k;
        }

        public void OnCpRefVoi() // I-12
        {
            var num_cp = VideoParameterSetRbsp.Vps3dExtension.NumCp;
            var cp_ref_voi = VideoParameterSetRbsp.Vps3dExtension.CpRefVoi;

            if (CpPresentFlag == null || CpPresentFlag.Length < NumViews)
            {
                CpPresentFlag = new uint[NumViews][];
                for (int i = 0; i < NumViews; i++)
                {
                    CpPresentFlag[i] = new uint[MaxLayersMinus1 + 1];
                }
            }

            for (int n = 1; n < NumViews; n++)
            {
                uint i = ViewOIdxList[n];
                for (int m = 0; m < num_cp[i]; m++)
                    CpPresentFlag[i][cp_ref_voi[i][m]] = 1;
            }
        }

        public void OnNumInterLayerRefPicsMinus1()
        {
            var nuh_layer_id = NalHeader.NalUnitHeader.NuhLayerId;
            var sub_layers_vps_max_minus1 = VideoParameterSetRbsp.VpsExtension.SubLayersVpsMaxMinus1;
            var max_tid_il_ref_pics_plus1 = VideoParameterSetRbsp.VpsExtension.MaxTidIlRefPicsPlus1;
            var default_ref_layers_active_flag = VideoParameterSetRbsp.VpsExtension.DefaultRefLayersActiveFlag;
            var inter_layer_pred_enabled_flag = SliceSegmentLayerRbsp.SliceSegmentHeader.InterLayerPredEnabledFlag;
            var max_one_active_ref_layer_flag = VideoParameterSetRbsp.VpsExtension.MaxOneActiveRefLayerFlag;
            var num_inter_layer_ref_pics_minus1 = SliceSegmentLayerRbsp.SliceSegmentHeader.NumInterLayerRefPicsMinus1;

            if (refLayerPicIdc == null || refLayerPicIdc.Length < MaxLayersMinus1 + 1)
            {
                refLayerPicIdc = new uint[MaxLayersMinus1 + 1];
            }

            uint j = 0;
            for (uint i = 0; i < NumDirectRefLayers[nuh_layer_id]; i++)
            {
                uint refLayerIdx = LayerIdxInVps[IdDirectRefLayer[nuh_layer_id][i]];
                if (sub_layers_vps_max_minus1[refLayerIdx] >= TemporalId && (TemporalId == 0 || max_tid_il_ref_pics_plus1[refLayerIdx][LayerIdxInVps[nuh_layer_id]] > TemporalId))
                    refLayerPicIdc[j++] = i;
            }
            uint numRefLayerPics = j;

            if (nuh_layer_id == 0 || numRefLayerPics == 0)
                NumActiveRefLayerPics = 0;
            else if (default_ref_layers_active_flag != 0)
                NumActiveRefLayerPics = numRefLayerPics;
            else if (inter_layer_pred_enabled_flag == 0)
                NumActiveRefLayerPics = 0;
            else if (max_one_active_ref_layer_flag != 0 || NumDirectRefLayers[nuh_layer_id] == 1)
                NumActiveRefLayerPics = 1;
            else
                NumActiveRefLayerPics = num_inter_layer_ref_pics_minus1 + 1;
        }

        public void OnNuhTemporalIdPlus1()
        {
            var nuh_temporal_id_plus1 = NalHeader.NalUnitHeader.NuhTemporalIdPlus1;
            TemporalId = nuh_temporal_id_plus1 - 1;
        }

        public void OnListEntryL0()
        {
            var pps_curr_pic_ref_enabled_flag = PicParameterSetRbsp.PpsSccExtension.PpsCurrPicRefEnabledFlag;
            var num_long_term_sps = SliceSegmentLayerRbsp.SliceSegmentHeader.NumLongTermSps;
            var num_long_term_pics = SliceSegmentLayerRbsp.SliceSegmentHeader.NumLongTermPics;

            NumPicTotalCurr = 0;
            for (int i = 0; i < (int)NumNegativePics[CurrRpsIdx]; i++)
                if (UsedByCurrPicS0[CurrRpsIdx][i] != 0)
                    NumPicTotalCurr++;
            for (int i = 0; i < (int)NumPositivePics[CurrRpsIdx]; i++)
                if (UsedByCurrPicS1[CurrRpsIdx][i] != 0)
                    NumPicTotalCurr++;
            for (int i = 0; i < (int)(num_long_term_sps + num_long_term_pics); i++)
                if (UsedByCurrPicLt[i] != 0)
                    NumPicTotalCurr++;
            if (pps_curr_pic_ref_enabled_flag != 0)
                NumPicTotalCurr++;
        }

        public void OnUsedByCurrPicLtFlag(uint i)
        {
            var num_long_term_sps = SliceSegmentLayerRbsp.SliceSegmentHeader.NumLongTermSps;
            var num_long_term_pics = SliceSegmentLayerRbsp.SliceSegmentHeader.NumLongTermPics;
            var lt_ref_pic_poc_lsb_sps = SeqParameterSetRbsp.LtRefPicPocLsbSps;
            var lt_idx_sps = SliceSegmentLayerRbsp.SliceSegmentHeader.LtIdxSps;
            var used_by_curr_pic_lt_sps_flag = SeqParameterSetRbsp.UsedByCurrPicLtSpsFlag;
            var poc_lsb_lt = SliceSegmentLayerRbsp.SliceSegmentHeader.PocLsbLt;
            var used_by_curr_pic_lt_flag = SliceSegmentLayerRbsp.SliceSegmentHeader.UsedByCurrPicLtFlag;

            if(PocLsbLt == null || PocLsbLt.Length < (int)(num_long_term_sps + num_long_term_pics))
            {
                PocLsbLt = new uint[num_long_term_sps + num_long_term_pics];
            }
            if(UsedByCurrPicLt == null || UsedByCurrPicLt.Length < (int)(num_long_term_sps + num_long_term_pics))
            {
                UsedByCurrPicLt = new uint[num_long_term_sps + num_long_term_pics];
            }

            if (i < num_long_term_sps)
            {
                PocLsbLt[i] = lt_ref_pic_poc_lsb_sps[lt_idx_sps[i]];
                UsedByCurrPicLt[i] = used_by_curr_pic_lt_sps_flag[lt_idx_sps[i]];
            }
            else
            {
                PocLsbLt[i] = poc_lsb_lt[i];
                UsedByCurrPicLt[i] = used_by_curr_pic_lt_flag[i];
            }
        }

        public void OnShortTermRefPicSetIdx()
        {
            var short_term_ref_pic_set_sps_flag = SliceSegmentLayerRbsp.SliceSegmentHeader.ShortTermRefPicSetSpsFlag;
            var short_term_ref_pic_set_idx = SliceSegmentLayerRbsp.SliceSegmentHeader.ShortTermRefPicSetIdx;
            var num_short_term_ref_pic_sets = SeqParameterSetRbsp.NumShortTermRefPicSets;

            if (short_term_ref_pic_set_sps_flag == 1)
            {
                CurrRpsIdx = short_term_ref_pic_set_idx;
            }
            else
            {
                CurrRpsIdx = num_short_term_ref_pic_sets;
            }
        }
    }
}
