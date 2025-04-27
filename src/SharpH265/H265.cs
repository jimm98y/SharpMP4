using SharpH26X;

namespace SharpH265
{
    public class H265Constants
    {
        public const uint EXTENDED_ISO = 255;
        public const uint EXTENDED_SAR = 255;
    }

    public class H265FrameTypes
    {
        public const uint P = 0;
        public const uint B = 1;
        public const uint I = 2;

        public static bool IsP(uint value) { return value == P; }
        public static bool IsB(uint value) { return value == B; }
        public static bool IsI(uint value) { return value == I; }
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
        public uint MaxLayersMinus1 { get; set; }
        public uint CpbCnt { get; set; }
        
        public uint NumViews { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[] NumLayersInIdList { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }


        // TODO
        public int[] MaxSubLayersInLayerSetMinus1 { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[] OlsIdxToLsIdx { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[][][] BspSchedCnt { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[] NumDeltaPocs { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[] MaxTemporalId { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[][] NecessaryLayerFlag { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint RefRpsIdx { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[] ViewOIdxList { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint VclInitialArrivalDelayPresent { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint NalInitialArrivalDelayPresent { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[] LayerIdxInVps { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[] OlsHighestOutputLayerId { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[] NumOutputLayersInOutputLayerSet { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint PicSizeInCtbsY { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }


        public int[][] IdDirectRefLayer { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[] NumDirectRefLayers { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint NumIndependentLayers { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[] NumLayersInTreePartition { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[][] LayerSetLayerIdList { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint NumLayerSets { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint FirstAddLayerSetIdx { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint LastAddLayerSetIdx { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[][] CpPresentFlag { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[][] DependencyFlag { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[] layerIdInListFlag { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[][] IdRefLayer { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[][] IdPredictedLayer { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[][] TreePartitionLayerIdList { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[] NumRefLayers { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[] NumPredictedLayers { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[][] ScalabilityId { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[] DependencyId { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[] AuxId { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[] DepthLayerFlag { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public int[] ViewOrderIdx { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public SeiPayload SeiPayload { get; set; }

        public void SetSeiPayload(SeiPayload payload)
        {
            if(SeiPayload == null)
            {
                SeiPayload = payload;
            }

            if(payload.ActiveParameterSets != null)
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
            if(num_add_layer_sets > 0)
            {
                // F-8
                FirstAddLayerSetIdx = vps_num_layer_sets_minus1 + 1;
                LastAddLayerSetIdx = FirstAddLayerSetIdx + num_add_layer_sets - 1;
            }
        }

        public void OnHighestLayerIdxPlus1(uint i) // F-9
        {
            var vps_num_layer_sets_minus1 = VideoParameterSetRbsp.VpsNumLayerSetsMinus1;
            var highest_layer_idx_plus1 = VideoParameterSetRbsp.VpsExtension.HighestLayerIdxPlus1;

            int layerNum = 0;
            uint lsIdx = vps_num_layer_sets_minus1 + 1 + i;
            for (int treeIdx = 1; treeIdx < NumIndependentLayers; treeIdx++)
                for (int layerCnt = 0; layerCnt < highest_layer_idx_plus1[i][treeIdx]; layerCnt++)
                    LayerSetLayerIdList[lsIdx][layerNum++] = TreePartitionLayerIdList[treeIdx][layerCnt];
            NumLayersInIdList[lsIdx] = layerNum;
        }

        public void OnDimensionId() // F-3
        {
            var dimension_id = VideoParameterSetRbsp.VpsExtension.DimensionId;
            var layer_id_in_nuh = VideoParameterSetRbsp.VpsExtension.LayerIdInNuh;
            var scalability_mask_flag = VideoParameterSetRbsp.VpsExtension?.ScalabilityMaskFlag;

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

        public void OnDirectDependencyFlag()
        {
            var direct_dependency_flag = VideoParameterSetRbsp.VpsExtension.DirectDependencyFlag;
            var layer_id_in_nuh = VideoParameterSetRbsp.VpsExtension.LayerIdInNuh;

            uint k = 0;
            
            // F-4
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

            for (int n = 1; n < NumViews; n++)
            {
                uint i = ViewOIdxList[n];
                for (int m = 0; m < num_cp[i]; m++)
                    CpPresentFlag[i][cp_ref_voi[i][m]] = 1;
            }
        }
    }
}
