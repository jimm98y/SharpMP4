using System;

namespace SharpH266
{
    public class H266Constants
    {
        public const int ALF_APS = 0; // ALF parameters
        public const int LMCS_APS = 1; // LMCS parameters
        public const int SCALING_APS = 2; // ScalingList parameters
    }

    public class H266FrameTypes
    {
        public const uint B = 0;
        public const uint P = 1;
        public const uint I = 2;

        public static bool IsB(uint value) { return value == B; }
        public static bool IsP(uint value) { return value == P; }
        public static bool IsI(uint value) { return value == I; }
    }

    public class H266NALTypes
    {          
        public const uint TRAIL_NUT = 0;                     // Coded slice of a trailing picture or subpicture
        public const uint STSA_NUT = 1;                      // Coded slice of an STSA picture or subpicture
        public const uint RADL_NUT = 2;                      // Coded slice of a random access picture or subpicture
        public const uint RASL_NUT = 3;                      // Coded slice of a random access picture or subpicture
        
        public const uint RSV_VCL_4 = 4;                     // Reserved for future use
        public const uint RSV_VCL_5 = 5;                     // Reserved for future use
        public const uint RSV_VCL_6 = 6;                     // Reserved for future use

        public const uint IDR_W_RADL = 7;                    // Coded slice of an IDR picture or subpicture
        public const uint IDR_N_LP = 8;                      // Coded slice of an IDR picture or subpicture
        public const uint CRA_NUT = 9;                       // Coded slice of a CRA picture or subpicture
        public const uint GDR_NUT = 10;                      // Coded slice of a GDR picture or subpicture
                                                             // 
        public const uint RSV_IRAP_11 = 11;                  // Reserved IRAP VCL NAL unit
        public const uint OPI_NUT = 12;                      // Operating Point Information
        public const uint DCI_NUT = 13;                      // Decoding Capability Information
        public const uint VPS_NUT = 14;                      // Video parameter set
        public const uint SPS_NUT = 15;                      // Sequence parameter set
        public const uint PPS_NUT = 16;                      // Picture parameter set
        public const uint PREFIX_APS_NUT = 17;               // Adaptation Parameter Set
        public const uint SUFFIX_APS_NUT = 18;               // Adaptation Parameter Set
        public const uint PH_NUT = 19;                       // Picture Header
        public const uint AUD_NUT = 20;                      // AU Delimiter
        public const uint EOS_NUT = 21;                      // End of Sequence
        public const uint EOB_NUT = 22;                      // End of Bitstream
        public const uint PREFIX_SEI_NUT = 23;               // Supplemental enhancement information
        public const uint SUFFIX_SEI_NUT = 24;               // Supplemental enhancement information
        public const uint FD_NUT = 25;                       // Filler Data

        public const uint RSV_NVCL_26 = 26;                  // Reserved non-VCL NAL unit types
        public const uint RSV_NVCL_27 = 27;                  // Reserved non-VCL NAL unit types

        public const uint UNSPEC_28 = 28;                    // Unspecified
        public const uint UNSPEC_29 = 29;                    // Unspecified
        public const uint UNSPEC_30 = 30;                    // Unspecified
        public const uint UNSPEC_31 = 31;                    // Unspecified
    }

    public partial class H266Context
    {
        internal byte[][] cbr_flag;
        internal uint[][] bit_rate_du_value_minus1;
        internal uint[][] cpb_size_du_value_minus1;
        internal uint[][] bit_rate_value_minus1;
        internal uint[][] cpb_size_value_minus1;
        internal uint[][] num_ref_entries;
        internal byte[][][] inter_layer_ref_pic_flag;
        internal byte[][][] st_ref_pic_flag;
        internal uint[][][] abs_delta_poc_st;
        internal byte[][][] strp_entry_sign_flag;
        internal uint[][][] rpls_poc_lsb_lt;
        internal uint[][][] ilrp_idx;

        public SeiPayload SeiPayload { get; set; }
        public GeneralTimingHrdParameters GeneralTimingHrdParameters { get; set; }

        public uint olsModeIdc { get; set; }
        public uint TotalNumOlss { get; set; }
        public uint VpsNumDpbParams { get; set; }
        public uint[] NumOutputLayersInOls { get; set; }
        public uint[][] OutputLayerIdInOls { get; set; }
        public uint[][] layerIncludedInOlsFlag { get; set; }
        public uint[][] NumSubLayersInLayerInOLS { get; set; }
        public uint[] LayerUsedAsOutputLayerFlag { get; set; }
        public int[][] OutputLayerIdx { get; set; }
        public uint[][] LayerIdInOls { get; set; }
        public int NumMultiLayerOlss { get; set; }
        public int[] MultiLayerOlsIdx { get; set; }
        public uint[] LayerUsedAsRefLayerFlag { get; set; }
        public int[] NumDirectRefLayers { get; set; }
        public int[] NumRefLayers { get; set; }
        public int[] GeneralLayerIdx { get; set; }
        public int[][] DirectRefLayerIdx { get; set; }
        public int[][] ReferenceLayerIdx { get; set; }
        public uint[] NumLayersInOls { get; set; }
        public uint CtbLog2SizeY { get; set; }
        public uint CtbSizeY { get; set; }
        public uint MaxNumMergeCand { get; set; }
        public int NumExtraPhBits { get; set; }
        public int NumAlfFilters { get; set; }
        public uint LmcsMaxBinIdx { get; set; }
        public uint PicWidthInCtbsY { get; set; }
        public uint PicHeightInCtbsY { get; set; }
        public uint PicSizeInCtbsY { get; set; }
        public uint PicWidthInMinCbsY { get; set; }
        public uint PicHeightInMinCbsY { get; set; }
        public uint PicSizeInMinCbsY { get; set; }
        public uint PicSizeInSamplesY { get; set; }
        public uint PicWidthInSamplesC { get; set; }
        public uint PicHeightInSamplesC { get; set; }
        public uint SubWidthC { get; set; }
        public uint SubHeightC { get; set; }
        public uint MinCbLog2SizeY { get; set; }
        public uint MinCbSizeY { get; set; }
        public uint IbcBufWidthY { get; set; }
        public uint IbcBufWidthC { get; set; }
        public uint VSize { get; set; }
        public uint CtbWidthC { get; set; }
        public uint CtbHeightC { get; set; }
        public int NumTileColumns { get; set; }
        public int NumTileRows { get; set; }
        public int NumTilesInPic { get; set; }
        public uint[] ColWidthVal { get; set; }
        public uint[] RowHeightVal { get; set; }
        public uint[] TileColBdVal { get; set; }
        public uint[] TileRowBdVal { get; set; }
        public uint[] CtbToTileColBd { get; set; }
        public uint[] ctbToTileColIdx { get; set; }
        public uint[] CtbToTileRowBd { get; set; }
        public uint[] ctbToTileRowIdx { get; set; }
        public uint[] SubpicWidthInTiles { get; set; }
        public uint[] SubpicHeightInTiles { get; set; }
        public uint[] subpicHeightLessThanOneTileFlag { get; set; }
        public uint[] NumCtusInSlice { get; set; }
        public uint[] SliceTopLeftTileIdx { get; set; }
        public uint[] sliceWidthInTiles { get; set; }
        public uint[] sliceHeightInTiles { get; set; }
        public uint[] NumSlicesInTile { get; set; }
        public uint[] sliceHeightInCtus { get; set; }
        public uint[][] CtbAddrInSlice { get; set; }
        public uint[] NumSlicesInSubpic { get; set; }
        public int[] SubpicIdxForSlice { get; set; }
        public uint[] SubpicLevelSliceIdx { get; set; }
        public uint[][] NumLtrpEntries { get; set; }
        public uint[] RplsIdx { get; set; } = new uint[2];
        public uint NumWeightsL0 { get; set; }
        public uint[] NumRefIdxActive { get; set; }
        public int CurrSubpicIdx { get; set; }
        public uint[] SubpicIdVal { get; set; }
        public int NumExtraShBits { get; set; }
        public int NumEntryPoints { get; set; }
        public uint NumCtusInCurrSlice { get; set; }
        public uint[] CtbAddrInCurrSlice { get; set; }
        public uint NumWeightsL1 { get; set; }
        public uint[][][] AbsDeltaPocSt { get; private set; }

        public void SetGeneralTimingHrdParameters(GeneralTimingHrdParameters generalTimingHrdParameters)
        {
            GeneralTimingHrdParameters = generalTimingHrdParameters;
        }

        public void SetSeiPayload(SeiPayload payload)
        {
            if (SeiPayload == null)
            {
                SeiPayload = payload;
            }

            if (payload.AlternativeTransferCharacteristics != null)
                SeiPayload.AlternativeTransferCharacteristics = payload.AlternativeTransferCharacteristics;
            if (payload.AmbientViewingEnvironment != null)
                SeiPayload.AmbientViewingEnvironment = payload.AmbientViewingEnvironment;
            if (payload.BufferingPeriod != null)
                SeiPayload.BufferingPeriod = payload.BufferingPeriod;
            if (payload.ContentColourVolume != null)
                SeiPayload.ContentColourVolume = payload.ContentColourVolume;
            if (payload.ContentLightLevelInfo != null)
                SeiPayload.ContentLightLevelInfo = payload.ContentLightLevelInfo;
            if (payload.DecodedPictureHash != null)
                SeiPayload.DecodedPictureHash = payload.DecodedPictureHash;
            if (payload.DecodingUnitInfo != null)
                SeiPayload.DecodingUnitInfo = payload.DecodingUnitInfo;
            if (payload.DependentRapIndication != null)
                SeiPayload.DependentRapIndication = payload.DependentRapIndication;
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
            if (payload.GeneralizedCubemapProjection != null)
                SeiPayload.GeneralizedCubemapProjection = payload.GeneralizedCubemapProjection;
            if (payload.MasteringDisplayColourVolume != null)
                SeiPayload.MasteringDisplayColourVolume = payload.MasteringDisplayColourVolume;
            if (payload.OmniViewport != null)
                SeiPayload.OmniViewport = payload.OmniViewport;
            if (payload.ParameterSetsInclusionIndication != null)
                SeiPayload.ParameterSetsInclusionIndication = payload.ParameterSetsInclusionIndication;
            if (payload.PicTiming != null)
                SeiPayload.PicTiming = payload.PicTiming;
            if (payload.RegionwisePacking != null)
                SeiPayload.RegionwisePacking = payload.RegionwisePacking;
            if (payload.ReservedMessage != null)
                SeiPayload.ReservedMessage = payload.ReservedMessage;
            if (payload.SampleAspectRatioInfo != null)
                SeiPayload.SampleAspectRatioInfo = payload.SampleAspectRatioInfo;
            if (payload.ScalableNesting != null)
                SeiPayload.ScalableNesting = payload.ScalableNesting;
            if (payload.SphereRotation != null)
                SeiPayload.SphereRotation = payload.SphereRotation;
            if (payload.SubpicLevelInfo != null)
                SeiPayload.SubpicLevelInfo = payload.SubpicLevelInfo;
            if (payload.UserDataRegisteredItutT35 != null)
                SeiPayload.UserDataRegisteredItutT35 = payload.UserDataRegisteredItutT35;
            if (payload.UserDataUnregistered != null)
                SeiPayload.UserDataUnregistered = payload.UserDataUnregistered;
        }

        public void OnVpsNumOutputLayerSetsMinus2()
        {
            var vps_each_layer_is_an_ols_flag = VideoParameterSetRbsp.VpsEachLayerIsAnOlsFlag;
            var vps_ols_mode_idc = VideoParameterSetRbsp.VpsOlsModeIdc;
            var vps_max_layers_minus1 = VideoParameterSetRbsp.VpsMaxLayersMinus1;
            var vps_num_output_layer_sets_minus2 = VideoParameterSetRbsp.VpsNumOutputLayerSetsMinus2;

            if (vps_each_layer_is_an_ols_flag == 0)
                olsModeIdc = vps_ols_mode_idc;
            else
                olsModeIdc = 4;

            if (olsModeIdc == 4 || olsModeIdc == 0 || olsModeIdc == 1)
                TotalNumOlss = vps_max_layers_minus1 + 1;
            else if (olsModeIdc == 2)
                TotalNumOlss = vps_num_output_layer_sets_minus2 + 2;
        }

        public void OnVpsNumDpbParamsMinus1()
        {
            var vps_each_layer_is_an_ols_flag = VideoParameterSetRbsp.VpsEachLayerIsAnOlsFlag;
            var vps_num_dpb_params_minus1 = VideoParameterSetRbsp.VpsNumDpbParamsMinus1;

            if (vps_each_layer_is_an_ols_flag != 0)
                VpsNumDpbParams = 0;
            else
                VpsNumDpbParams = vps_num_dpb_params_minus1 + 1;
        }

        public void OnVpsDirectRefLayerFlag()
        {
            var vps_max_layers_minus1 = VideoParameterSetRbsp.VpsMaxLayersMinus1;
            var vps_direct_ref_layer_flag = VideoParameterSetRbsp.VpsDirectRefLayerFlag;
            var vps_layer_id = VideoParameterSetRbsp.VpsLayerId;

            uint[][] dependencyFlag = new uint[vps_max_layers_minus1 + 1][];
            for (int i = 0; i < dependencyFlag.Length; i++)
            {
                dependencyFlag[i] = new uint[vps_max_layers_minus1 + 1];
            }
            if (DirectRefLayerIdx == null || DirectRefLayerIdx.Length < vps_max_layers_minus1 + 1)
            {
                DirectRefLayerIdx = new int[vps_max_layers_minus1 + 1][];
                for (int i = 0; i < DirectRefLayerIdx.Length; i++)
                {
                    DirectRefLayerIdx[i] = new int[vps_max_layers_minus1 + 1];
                }
            }
            if (ReferenceLayerIdx == null || ReferenceLayerIdx.Length < vps_max_layers_minus1 + 1)
            {
                ReferenceLayerIdx = new int[vps_max_layers_minus1 + 1][];
                for (int i = 0; i < ReferenceLayerIdx.Length; i++)
                {
                    ReferenceLayerIdx[i] = new int[vps_max_layers_minus1 + 1];
                }
            }

            if (LayerUsedAsRefLayerFlag == null || LayerUsedAsRefLayerFlag.Length < vps_max_layers_minus1 + 1)
                LayerUsedAsRefLayerFlag = new uint[vps_max_layers_minus1 + 1];
            if (NumDirectRefLayers == null || NumDirectRefLayers.Length < vps_max_layers_minus1 + 1)
                NumDirectRefLayers = new int[vps_max_layers_minus1 + 1];
            if (NumRefLayers == null || NumRefLayers.Length < vps_max_layers_minus1 + 1)
                NumRefLayers = new int[vps_max_layers_minus1 + 1];
            if (GeneralLayerIdx == null || GeneralLayerIdx.Length < vps_max_layers_minus1 + 1)
                GeneralLayerIdx = new int[vps_max_layers_minus1 + 1];

            for (int i = 0; i <= vps_max_layers_minus1; i++)
            {
                for (int j = 0; j <= vps_max_layers_minus1; j++)
                {
                    dependencyFlag[i][j] = vps_direct_ref_layer_flag[i][j];
                    for (int k = 0; k < i; k++)
                        if (vps_direct_ref_layer_flag[i][k] != 0 && dependencyFlag[k][j] != 0)
                            dependencyFlag[i][j] = 1;
                }
                LayerUsedAsRefLayerFlag[i] = 0;
            }

            for (int i = 0; i <= vps_max_layers_minus1; i++)
            {
                int d = 0, r = 0;
                for (int j = 0; j <= vps_max_layers_minus1; j++)
                {
                    if (vps_direct_ref_layer_flag[i][j] != 0)
                    {
                        DirectRefLayerIdx[i][d++] = j;
                        LayerUsedAsRefLayerFlag[j] = 1;
                    }
                    if (dependencyFlag[i][j] != 0)
                        ReferenceLayerIdx[i][r++] = j;
                }
                NumDirectRefLayers[i] = d;
                NumRefLayers[i] = r;
            }

            for (int i = 0; i <= vps_max_layers_minus1; i++)
                GeneralLayerIdx[vps_layer_id[i]] = i;
        }

        public void OnVpsOlsOutputLayerFlag(uint jj)
        {
            var vps_layer_id = VideoParameterSetRbsp.VpsLayerId;
            var vps_ptl_max_tid = VideoParameterSetRbsp.VpsPtlMaxTid;
            var vps_ols_ptl_idx = VideoParameterSetRbsp.VpsOlsPtlIdx;
            var vps_max_layers_minus1 = VideoParameterSetRbsp.VpsMaxLayersMinus1;
            var vps_ols_mode_idc = VideoParameterSetRbsp.VpsOlsModeIdc;
            var vps_each_layer_is_an_ols_flag = VideoParameterSetRbsp.VpsEachLayerIsAnOlsFlag;
            var vps_max_tid_il_ref_pics_plus1 = VideoParameterSetRbsp.VpsMaxTidIlRefPicsPlus1;
            var vps_direct_ref_layer_flag = VideoParameterSetRbsp.VpsDirectRefLayerFlag;
            var vps_ols_output_layer_flag = VideoParameterSetRbsp.VpsOlsOutputLayerFlag;

            if (NumOutputLayersInOls == null || NumOutputLayersInOls.Length < TotalNumOlss)
                NumOutputLayersInOls = new uint[TotalNumOlss];
            if (OutputLayerIdInOls == null || OutputLayerIdInOls.Length < TotalNumOlss)
            {
                OutputLayerIdInOls = new uint[TotalNumOlss][];
                OutputLayerIdInOls[0] = new uint[1];
            }
            if (NumSubLayersInLayerInOLS == null || NumSubLayersInLayerInOLS.Length < TotalNumOlss)
            {
                NumSubLayersInLayerInOLS = new uint[TotalNumOlss][];
                for (int i = 0; i < TotalNumOlss; i++)
                {
                    NumSubLayersInLayerInOLS[i] = new uint[TotalNumOlss];
                }
            }
            if (layerIncludedInOlsFlag == null || layerIncludedInOlsFlag.Length < TotalNumOlss)
            {
                layerIncludedInOlsFlag = new uint[TotalNumOlss][];
                for (int i = 0; i < TotalNumOlss; i++)
                {
                    layerIncludedInOlsFlag[i] = new uint[TotalNumOlss];
                }
            }
            if (OutputLayerIdx == null || OutputLayerIdx.Length < TotalNumOlss)
            {
                OutputLayerIdx = new int[TotalNumOlss][];
                for (int i = 0; i < TotalNumOlss; i++)
                {
                    OutputLayerIdx[i] = new int[TotalNumOlss];
                }
            }
            if (LayerIdInOls == null || LayerIdInOls.Length < TotalNumOlss)
            {
                LayerIdInOls = new uint[TotalNumOlss][];
                for (int i = 0; i < TotalNumOlss; i++)
                {
                    LayerIdInOls[i] = new uint[TotalNumOlss];
                }
            }
            if (LayerUsedAsOutputLayerFlag == null || LayerUsedAsOutputLayerFlag.Length < vps_max_layers_minus1 + 1)
                LayerUsedAsOutputLayerFlag = new uint[vps_max_layers_minus1 + 1];
            if (MultiLayerOlsIdx == null || MultiLayerOlsIdx.Length < TotalNumOlss)
                LayerUsedAsOutputLayerFlag = new uint[TotalNumOlss];
            if (NumLayersInOls == null || NumLayersInOls.Length < TotalNumOlss)
                NumLayersInOls = new uint[TotalNumOlss];

            NumOutputLayersInOls[0] = 1;
            OutputLayerIdInOls[0][0] = vps_layer_id[0];
            NumSubLayersInLayerInOLS[0][0] = vps_ptl_max_tid[vps_ols_ptl_idx[0]] + 1;
            LayerUsedAsOutputLayerFlag[0] = 1;
            for (int i = 1; i <= vps_max_layers_minus1; i++)
            {
                if (olsModeIdc == 4 || olsModeIdc < 2)
                    LayerUsedAsOutputLayerFlag[i] = 1;
                else if (vps_ols_mode_idc == 2)
                    LayerUsedAsOutputLayerFlag[i] = 0;
            }
            for (int i = 1; i < TotalNumOlss; i++)
            {
                if (olsModeIdc == 4 || olsModeIdc == 0)
                {
                    NumOutputLayersInOls[i] = 1;
                    OutputLayerIdInOls[i][0] = vps_layer_id[i];
                    if (vps_each_layer_is_an_ols_flag != 0)
                        NumSubLayersInLayerInOLS[i][0] = vps_ptl_max_tid[vps_ols_ptl_idx[i]] + 1;
                    else
                    {
                        NumSubLayersInLayerInOLS[i][i] = vps_ptl_max_tid[vps_ols_ptl_idx[i]] + 1;
                        for (int k = i - 1; k >= 0; k--)
                        {
                            NumSubLayersInLayerInOLS[i][k] = 0;
                            for (int m = k + 1; m <= i; m++)
                            {
                                uint maxSublayerNeeded = Math.Min(NumSubLayersInLayerInOLS[i][m], vps_max_tid_il_ref_pics_plus1[m][k]);
                                if (vps_direct_ref_layer_flag[m][k] != 0 &&
                                  NumSubLayersInLayerInOLS[i][k] < maxSublayerNeeded)
                                    NumSubLayersInLayerInOLS[i][k] = maxSublayerNeeded;
                            }
                        }
                    }
                }
                else if (vps_ols_mode_idc == 1)
                {
                    NumOutputLayersInOls[i] = (uint)(i + 1);
                    for (int j = 0; j < NumOutputLayersInOls[i]; j++)
                    {
                        if (OutputLayerIdInOls[i] == null || OutputLayerIdInOls.Length <= NumOutputLayersInOls[i])
                            OutputLayerIdInOls[i] = new uint[NumOutputLayersInOls[i] + 1];

                        OutputLayerIdInOls[i][j] = vps_layer_id[j];
                        NumSubLayersInLayerInOLS[i][j] = vps_ptl_max_tid[vps_ols_ptl_idx[i]] + 1;
                    }
                }
                else if (vps_ols_mode_idc == 2)
                {
                    for (int j = 0; j <= vps_max_layers_minus1; j++)
                    {
                        layerIncludedInOlsFlag[i][j] = 0;
                        NumSubLayersInLayerInOLS[i][j] = 0;
                    }
                    int highestIncludedLayer = 0;
                    for (int k = 0, j = 0; k <= vps_max_layers_minus1; k++)
                    {
                        if (vps_ols_output_layer_flag[i][k] != 0)
                        {
                            layerIncludedInOlsFlag[i][k] = 1;
                            highestIncludedLayer = k;
                            LayerUsedAsOutputLayerFlag[k] = 1;
                            OutputLayerIdx[i][j] = k;
                            OutputLayerIdInOls[i][j++] = vps_layer_id[k];
                            NumSubLayersInLayerInOLS[i][k] = vps_ptl_max_tid[vps_ols_ptl_idx[i]] + 1;
                        }
                    }
                    NumOutputLayersInOls[i] = jj;
                    for (int j = 0; j < NumOutputLayersInOls[i]; j++)
                    {
                        int idx = OutputLayerIdx[i][j];
                        for (int k = 0; k < NumRefLayers[idx]; k++)
                        {
                            if (layerIncludedInOlsFlag[i][ReferenceLayerIdx[idx][k]] == 0)
                                layerIncludedInOlsFlag[i][ReferenceLayerIdx[idx][k]] = 1;
                        }
                    }
                    for (int k = highestIncludedLayer - 1; k >= 0; k--)
                        if (layerIncludedInOlsFlag[i][k] != 0 && vps_ols_output_layer_flag[i][k] == 0)
                            for (int m = k + 1; m <= highestIncludedLayer; m++)
                            {
                                uint maxSublayerNeeded = Math.Min(NumSubLayersInLayerInOLS[i][m], vps_max_tid_il_ref_pics_plus1[m][k]);
                                if (vps_direct_ref_layer_flag[m][k] != 0 && layerIncludedInOlsFlag[i][m] != 0 &&
                                   NumSubLayersInLayerInOLS[i][k] < maxSublayerNeeded)
                                    NumSubLayersInLayerInOLS[i][k] = maxSublayerNeeded;
                            }
                }
            }

            NumLayersInOls[0] = 1;
            LayerIdInOls[0][0] = vps_layer_id[0];
            NumMultiLayerOlss = 0;
            for (int i = 1; i < TotalNumOlss; i++)
            {
                if (vps_each_layer_is_an_ols_flag != 0)
                {
                    NumLayersInOls[i] = 1;
                    LayerIdInOls[i][0] = vps_layer_id[i];
                }
                else if (vps_ols_mode_idc == 0 || vps_ols_mode_idc == 1)
                {
                    NumLayersInOls[i] = (uint)(i + 1);
                    for (int j = 0; j < NumLayersInOls[i]; j++)
                        LayerIdInOls[i][j] = vps_layer_id[j];
                }
                else if (vps_ols_mode_idc == 2)
                {
                    for (int k = 0, j = 0; k <= vps_max_layers_minus1; k++)
                        if (layerIncludedInOlsFlag[i][k] != 0)
                            LayerIdInOls[i][j++] = vps_layer_id[k];
                    NumLayersInOls[i] = jj;
                }
                if (NumLayersInOls[i] > 1)
                {
                    MultiLayerOlsIdx[i] = NumMultiLayerOlss;
                    NumMultiLayerOlss++;
                }
            }
        }

        public void OnSpsLog2CtuSizeMinus5()
        {
            var sps_log2_ctu_size_minus5 = SeqParameterSetRbsp.SpsLog2CtuSizeMinus5;
            CtbLog2SizeY = sps_log2_ctu_size_minus5 + 5;
            CtbSizeY = (uint)(1 << (int)CtbLog2SizeY);
        }

        public void OnSpsSixMinusMaxNumMergeCand()
        {
            var sps_six_minus_max_num_merge_cand = SeqParameterSetRbsp.SpsSixMinusMaxNumMergeCand;
            MaxNumMergeCand = 6 - sps_six_minus_max_num_merge_cand;
        }

        public void OnSpsExtraPhBitPresentFlag()
        {
            var sps_num_extra_ph_bytes = SeqParameterSetRbsp.SpsNumExtraPhBytes;
            var sps_extra_ph_bit_present_flag = SeqParameterSetRbsp.SpsExtraPhBitPresentFlag;
            NumExtraPhBits = 0;
            for (int i = 0; i < (sps_num_extra_ph_bytes * 8); i++)
                if (sps_extra_ph_bit_present_flag[i] != 0)
                    NumExtraPhBits++;
        }

        public void OnAlfChromaFilterSignalFlag()
        {
            NumAlfFilters = 25;
        }

        public void OnLmcsDeltaMaxBinIdx()
        {
            var lmcs_delta_max_bin_idx = AdaptationParameterSetRbsp.LmcsData.LmcsDeltaMaxBinIdx;
            LmcsMaxBinIdx = 15 - lmcs_delta_max_bin_idx;
        }

        public void OnSpsLog2MinLumaCodingBlockSizeMinus2()
        {
            var sps_log2_min_luma_coding_block_size_minus2 = SeqParameterSetRbsp.SpsLog2MinLumaCodingBlockSizeMinus2;
            var sps_chroma_format_idc = SeqParameterSetRbsp.SpsChromaFormatIdc;

            SubWidthC = sps_chroma_format_idc == 1 || sps_chroma_format_idc == 2 ? 2u : 1u;
            SubHeightC = sps_chroma_format_idc == 1 ? 2u : 1u;

            MinCbLog2SizeY = sps_log2_min_luma_coding_block_size_minus2 + 2; // 43
            MinCbSizeY = (uint)(1 << (int)MinCbLog2SizeY); // 44
            IbcBufWidthY = 256 * 128 / CtbSizeY; // 45
            IbcBufWidthC = IbcBufWidthY / SubWidthC; // 46
            VSize = (uint)Math.Min(64, CtbSizeY); // 47

            if (sps_chroma_format_idc == 0)
            {
                CtbWidthC = 0;
                CtbHeightC = 0;
            }
            else
            {
                CtbWidthC = CtbSizeY / SubWidthC;
                CtbHeightC = CtbSizeY / SubHeightC;
            }
        }

        public void OnPpsPicHeightInLumaSamples()
        {
            var pps_pic_width_in_luma_samples = PicParameterSetRbsp.PpsPicWidthInLumaSamples;
            var pps_pic_height_in_luma_samples = PicParameterSetRbsp.PpsPicHeightInLumaSamples;

            PicWidthInCtbsY = (uint)Math.Ceiling(pps_pic_width_in_luma_samples / (double)CtbSizeY); // 64
            PicHeightInCtbsY = (uint)Math.Ceiling(pps_pic_height_in_luma_samples / (double)CtbSizeY); // 65
            PicSizeInCtbsY = PicWidthInCtbsY * PicHeightInCtbsY; // 66
            PicWidthInMinCbsY = pps_pic_width_in_luma_samples / MinCbSizeY; // 67
            PicHeightInMinCbsY = pps_pic_height_in_luma_samples / MinCbSizeY; // 68
            PicSizeInMinCbsY = PicWidthInMinCbsY * PicHeightInMinCbsY; // 69
            PicSizeInSamplesY = pps_pic_width_in_luma_samples * pps_pic_height_in_luma_samples; // 70
            PicWidthInSamplesC = pps_pic_width_in_luma_samples / SubWidthC; // 71
            PicHeightInSamplesC = pps_pic_height_in_luma_samples / SubHeightC; // 72
        }

        public void OnPpsTileRowHeightMinus1()
        {
            var pps_num_exp_tile_columns_minus1 = PicParameterSetRbsp.PpsNumExpTileColumnsMinus1;
            var pps_num_exp_tile_rows_minus1 = PicParameterSetRbsp.PpsNumExpTileRowsMinus1;
            var pps_tile_column_width_minus1 = PicParameterSetRbsp.PpsTileColumnWidthMinus1;
            var pps_tile_row_height_minus1 = PicParameterSetRbsp.PpsTileRowHeightMinus1;
            var pps_single_slice_per_subpic_flag = PicParameterSetRbsp.PpsSingleSlicePerSubpicFlag;
            var pps_num_slices_in_pic_minus1 = PicParameterSetRbsp.PpsNumSlicesInPicMinus1;
            var pps_slice_width_in_tiles_minus1 = PicParameterSetRbsp.PpsSliceWidthInTilesMinus1;
            var pps_slice_height_in_tiles_minus1 = PicParameterSetRbsp.PpsSliceHeightInTilesMinus1;
            var pps_num_exp_slices_in_tile = PicParameterSetRbsp.PpsNumExpSlicesInTile;
            var pps_exp_slice_height_in_ctus_minus1 = PicParameterSetRbsp.PpsExpSliceHeightInCtusMinus1;
            var pps_tile_idx_delta_present_flag = PicParameterSetRbsp.PpsTileIdxDeltaPresentFlag;
            var pps_tile_idx_delta_val = PicParameterSetRbsp.PpsTileIdxDeltaVal;

            var sps_num_subpics_minus1 = SeqParameterSetRbsp.SpsNumSubpicsMinus1;
            var sps_subpic_ctu_top_left_x = SeqParameterSetRbsp.SpsSubpicCtuTopLeftx;
            var sps_subpic_width_minus1 = SeqParameterSetRbsp.SpsSubpicWidthMinus1;
            var sps_subpic_ctu_top_left_y = SeqParameterSetRbsp.SpsSubpicCtuTopLefty;
            var sps_subpic_height_minus1 = SeqParameterSetRbsp.SpsSubpicHeightMinus1;
            var sps_subpic_info_present_flag = SeqParameterSetRbsp.SpsSubpicInfoPresentFlag;

            if (ColWidthVal == null || ColWidthVal.Length < pps_num_exp_tile_columns_minus1 + 1)
                ColWidthVal = new uint[pps_num_exp_tile_columns_minus1 + 1];
            if (RowHeightVal == null || RowHeightVal.Length < pps_num_exp_tile_rows_minus1 + 1)
                RowHeightVal = new uint[pps_num_exp_tile_rows_minus1 + 1];

            // 14
            int i;
            uint remainingWidthInCtbsY = PicWidthInCtbsY;
            for (i = 0; i <= pps_num_exp_tile_columns_minus1; i++)
            {
                ColWidthVal[i] = pps_tile_column_width_minus1[i] + 1;
                remainingWidthInCtbsY -= ColWidthVal[i];
            }
            uint uniformTileColWidth = pps_tile_column_width_minus1[pps_num_exp_tile_columns_minus1] + 1;
            while (remainingWidthInCtbsY >= uniformTileColWidth)
            {
                ColWidthVal[i++] = uniformTileColWidth;
                remainingWidthInCtbsY -= uniformTileColWidth;
            }
            if (remainingWidthInCtbsY > 0)
                ColWidthVal[i++] = remainingWidthInCtbsY;
            NumTileColumns = i;

            // 15
            int j;
            uint remainingHeightInCtbsY = PicHeightInCtbsY;
            for (j = 0; j <= pps_num_exp_tile_rows_minus1; j++)
            {
                RowHeightVal[j] = pps_tile_row_height_minus1[j] + 1;
                remainingHeightInCtbsY -= RowHeightVal[j];
            }
            uint uniformTileRowHeight = pps_tile_row_height_minus1[pps_num_exp_tile_rows_minus1] + 1;
            while (remainingHeightInCtbsY >= uniformTileRowHeight)
            {
                RowHeightVal[j++] = uniformTileRowHeight;
                remainingHeightInCtbsY -= uniformTileRowHeight;
            }
            if (remainingHeightInCtbsY > 0)
                RowHeightVal[j++] = remainingHeightInCtbsY;
            NumTileRows = j;

            // needed in PPS
            NumTilesInPic = NumTileColumns * NumTileRows;

            if (TileColBdVal == null || TileColBdVal.Length < NumTileColumns + 1)
                TileColBdVal = new uint[NumTileColumns + 1];

            for (TileColBdVal[0] = 0, i = 0; i < NumTileColumns; i++)
                TileColBdVal[i + 1] = TileColBdVal[i] + ColWidthVal[i];

            if (TileRowBdVal == null || TileRowBdVal.Length < NumTileRows + 1)
                TileRowBdVal = new uint[NumTileRows + 1];

            for (TileRowBdVal[0] = 0, j = 0; j < NumTileRows; j++)
                TileRowBdVal[j + 1] = TileRowBdVal[j] + RowHeightVal[j];

            // 18
            if (CtbToTileColBd == null || CtbToTileColBd.Length < PicWidthInCtbsY + 1)
                CtbToTileColBd = new uint[PicWidthInCtbsY + 1];
            if (ctbToTileColIdx == null || ctbToTileColIdx.Length < PicWidthInCtbsY + 1)
                ctbToTileColIdx = new uint[PicWidthInCtbsY + 1];

            uint ctbAddrX = 0;
            uint tileX = 0;
            for (ctbAddrX = 0; ctbAddrX <= PicWidthInCtbsY; ctbAddrX++)
            {
                if (ctbAddrX == TileColBdVal[tileX + 1])
                    tileX++;
                CtbToTileColBd[ctbAddrX] = TileColBdVal[tileX];
                ctbToTileColIdx[ctbAddrX] = tileX;
            }

            // 19
            if (CtbToTileRowBd == null || CtbToTileRowBd.Length < PicHeightInCtbsY + 1)
                CtbToTileRowBd = new uint[PicHeightInCtbsY + 1];
            if (ctbToTileRowIdx == null || ctbToTileRowIdx.Length < PicHeightInCtbsY + 1)
                ctbToTileRowIdx = new uint[PicHeightInCtbsY + 1];

            uint ctbAddrY = 0;
            uint tileY = 0;
            for (ctbAddrY = 0; ctbAddrY <= PicHeightInCtbsY; ctbAddrY++)
            {
                if (ctbAddrY == TileRowBdVal[tileY + 1])
                    tileY++;
                CtbToTileRowBd[ctbAddrY] = TileRowBdVal[tileY];
                ctbToTileRowIdx[ctbAddrY] = tileY;
            }

            // 20
            if (SubpicWidthInTiles == null || SubpicWidthInTiles.Length < sps_num_subpics_minus1 + 1)
                SubpicWidthInTiles = new uint[sps_num_subpics_minus1 + 1];
            if (SubpicHeightInTiles == null || SubpicHeightInTiles.Length < sps_num_subpics_minus1 + 1)
                SubpicHeightInTiles = new uint[sps_num_subpics_minus1 + 1];
            if (subpicHeightLessThanOneTileFlag == null || subpicHeightLessThanOneTileFlag.Length < sps_num_subpics_minus1 + 1)
                subpicHeightLessThanOneTileFlag = new uint[sps_num_subpics_minus1 + 1];

            for (i = 0; i <= sps_num_subpics_minus1; i++)
            {
                uint leftX = sps_subpic_ctu_top_left_x[i];
                uint rightX = leftX + sps_subpic_width_minus1[i];
                SubpicWidthInTiles[i] = ctbToTileColIdx[rightX] + 1 - ctbToTileColIdx[leftX];
                uint topY = sps_subpic_ctu_top_left_y[i];
                uint bottomY = topY + sps_subpic_height_minus1[i];
                SubpicHeightInTiles[i] = ctbToTileRowIdx[bottomY] + 1 - ctbToTileRowIdx[topY];
                if (SubpicHeightInTiles[i] == 1 && sps_subpic_height_minus1[i] + 1 < RowHeightVal[ctbToTileRowIdx[topY]])
                    subpicHeightLessThanOneTileFlag[i] = 1;
                else
                    subpicHeightLessThanOneTileFlag[i] = 0;
            }

            // 21
            if (NumCtusInSlice == null || NumCtusInSlice.Length < sps_num_subpics_minus1 + 1)
                NumCtusInSlice = new uint[sps_num_subpics_minus1 + 1];
            if (CtbAddrInSlice == null || CtbAddrInSlice.Length < sps_num_subpics_minus1 + 1)
                CtbAddrInSlice = new uint[sps_num_subpics_minus1 + 1][];

            if (pps_single_slice_per_subpic_flag != 0)
            {
                if (sps_subpic_info_present_flag == 0) /* There is no subpicture info and only one slice in a picture. */
                {
                    for (j = 0; j < NumTileRows; j++)
                    {
                        for (i = 0; i < NumTileColumns; i++)
                            AddCtbsToSlice(0, TileColBdVal[i], TileColBdVal[i + 1], TileRowBdVal[j], TileRowBdVal[j + 1]);
                    }
                }
                else
                {
                    for (i = 0; i <= sps_num_subpics_minus1; i++)
                    {
                        NumCtusInSlice[i] = 0;
                        if (subpicHeightLessThanOneTileFlag[i] != 0) /* The slice consists of a set of CTU rows in a tile. */
                            AddCtbsToSlice(i, sps_subpic_ctu_top_left_x[i],
                             sps_subpic_ctu_top_left_x[i] + sps_subpic_width_minus1[i] + 1,
                             sps_subpic_ctu_top_left_y[i],
                             sps_subpic_ctu_top_left_y[i] + sps_subpic_height_minus1[i] + 1);
                        else
                        { /* The slice consists of a number of complete tiles covering a rectangular region. */
                            tileX = ctbToTileColIdx[sps_subpic_ctu_top_left_x[i]];
                            tileY = ctbToTileRowIdx[sps_subpic_ctu_top_left_y[i]];
                            for (j = 0; j < SubpicHeightInTiles[i]; j++)
                                for (int k = 0; k < SubpicWidthInTiles[i]; k++)
                                    AddCtbsToSlice(i, TileColBdVal[tileX + k], TileColBdVal[tileX + k + 1],
                                      TileRowBdVal[tileY + j], TileRowBdVal[tileY + j + 1]);
                        }
                    }
                }
            }
            else
            {
                int tileIdx = 0;
                for (i = 0; i <= pps_num_slices_in_pic_minus1; i++)
                    NumCtusInSlice[i] = 0;

                if (SliceTopLeftTileIdx == null || SliceTopLeftTileIdx.Length < pps_num_slices_in_pic_minus1 + 1)
                    SliceTopLeftTileIdx = new uint[pps_num_slices_in_pic_minus1 + 1];
                if (sliceWidthInTiles == null || sliceWidthInTiles.Length < pps_num_slices_in_pic_minus1 + 1)
                    sliceWidthInTiles = new uint[pps_num_slices_in_pic_minus1 + 1];
                if (sliceHeightInTiles == null || sliceHeightInTiles.Length < pps_num_slices_in_pic_minus1 + 1)
                    sliceHeightInTiles = new uint[pps_num_slices_in_pic_minus1 + 1];
                if (NumSlicesInTile == null || NumSlicesInTile.Length < pps_num_slices_in_pic_minus1 + 1)
                    NumSlicesInTile = new uint[pps_num_slices_in_pic_minus1 + 1];
                if (sliceHeightInCtus == null || sliceHeightInCtus.Length < pps_num_slices_in_pic_minus1 + 1)
                    sliceHeightInCtus = new uint[pps_num_slices_in_pic_minus1 + 1];

                for (i = 0; i <= pps_num_slices_in_pic_minus1; i++)
                {
                    SliceTopLeftTileIdx[i] = (uint)tileIdx;
                    tileX = (uint)(tileIdx % NumTileColumns);
                    tileY = (uint)(tileIdx / NumTileColumns);
                    if (i < pps_num_slices_in_pic_minus1)
                    {
                        sliceWidthInTiles[i] = pps_slice_width_in_tiles_minus1[i] + 1;
                        sliceHeightInTiles[i] = pps_slice_height_in_tiles_minus1[i] + 1;
                    }
                    else
                    {
                        sliceWidthInTiles[i] = (uint)(NumTileColumns - tileX);
                        sliceHeightInTiles[i] = (uint)(NumTileRows - tileY);
                        NumSlicesInTile[i] = 1;
                    }
                    if (sliceWidthInTiles[i] == 1 && sliceHeightInTiles[i] == 1)
                    {
                        if (pps_num_exp_slices_in_tile[i] == 0)
                        {
                            NumSlicesInTile[i] = 1;
                            sliceHeightInCtus[i] = RowHeightVal[SliceTopLeftTileIdx[i] / NumTileColumns];
                        }
                        else
                        {
                            remainingHeightInCtbsY = RowHeightVal[SliceTopLeftTileIdx[i] / NumTileColumns];
                            for (j = 0; j < pps_num_exp_slices_in_tile[i]; j++)
                            {
                                sliceHeightInCtus[i + j] = pps_exp_slice_height_in_ctus_minus1[i][j] + 1;
                                remainingHeightInCtbsY -= sliceHeightInCtus[i + j];
                            }
                            uint uniformSliceHeight = sliceHeightInCtus[i + j - 1];
                            while (remainingHeightInCtbsY >= uniformSliceHeight)
                            {
                                sliceHeightInCtus[i + j] = uniformSliceHeight;
                                remainingHeightInCtbsY -= uniformSliceHeight;
                                j++;
                            }
                            if (remainingHeightInCtbsY > 0)
                            {
                                sliceHeightInCtus[i + j] = remainingHeightInCtbsY;
                                j++;
                            }
                            NumSlicesInTile[i] = (uint)j;
                        }
                        uint ctbY = TileRowBdVal[tileY];
                        for (j = 0; j < NumSlicesInTile[i]; j++)
                        {
                            AddCtbsToSlice(i + j, TileColBdVal[tileX], TileColBdVal[tileX + 1], ctbY, ctbY + sliceHeightInCtus[i + j]);
                            ctbY += sliceHeightInCtus[i + j];
                            sliceWidthInTiles[i + j] = 1;
                            sliceHeightInTiles[i + j] = 1;
                        }
                        i += (int)(NumSlicesInTile[i] - 1);
                    }
                    else
                    {
                        for (j = 0; j < sliceHeightInTiles[i]; j++)
                        {
                            for (int k = 0; k < sliceWidthInTiles[i]; k++)
                            {
                                AddCtbsToSlice(i, TileColBdVal[tileX + k], TileColBdVal[tileX + k + 1], TileRowBdVal[tileY + j], TileRowBdVal[tileY + j + 1]);
                            }
                        }
                    }
                    if (i < pps_num_slices_in_pic_minus1)
                    {
                        if (pps_tile_idx_delta_present_flag != 0)
                        {
                            tileIdx += pps_tile_idx_delta_val[i];
                        }
                        else
                        {
                            tileIdx += (int)sliceWidthInTiles[i];
                            if (tileIdx % NumTileColumns == 0)
                                tileIdx += (int)((sliceHeightInTiles[i] - 1) * NumTileColumns);
                        }
                    }
                }
            }

            // 23
            if (NumSlicesInSubpic == null || NumSlicesInSubpic.Length < sps_num_subpics_minus1 + 1)
                NumSlicesInSubpic = new uint[sps_num_subpics_minus1 + 1];
            if (SubpicIdxForSlice == null || SubpicIdxForSlice.Length < pps_num_slices_in_pic_minus1 + 1)
                SubpicIdxForSlice = new int[pps_num_slices_in_pic_minus1 + 1];
            if (SubpicLevelSliceIdx == null || SubpicLevelSliceIdx.Length < pps_num_slices_in_pic_minus1 + 1)
                SubpicLevelSliceIdx = new uint[pps_num_slices_in_pic_minus1 + 1];

            for (i = 0; i <= sps_num_subpics_minus1; i++)
            {
                NumSlicesInSubpic[i] = 0;
                for (j = 0; j <= pps_num_slices_in_pic_minus1; j++)
                {
                    uint posX = CtbAddrInSlice[j][0] % PicWidthInCtbsY;
                    uint posY = CtbAddrInSlice[j][0] / PicWidthInCtbsY;
                    if ((posX >= sps_subpic_ctu_top_left_x[i]) &&
                      (posX < sps_subpic_ctu_top_left_x[i] + sps_subpic_width_minus1[i] + 1) &&
                      (posY >= sps_subpic_ctu_top_left_y[i]) &&
                      (posY < sps_subpic_ctu_top_left_y[i] + sps_subpic_height_minus1[i] + 1))
                    {
                        SubpicIdxForSlice[j] = i;
                        SubpicLevelSliceIdx[j] = NumSlicesInSubpic[i];
                        NumSlicesInSubpic[i]++;
                    }
                }
            }
        }

        public void AddCtbsToSlice(int sliceIdx, uint startX, uint stopX, uint startY, uint stopY)
        {
            for (uint ctbY = startY; ctbY < stopY; ctbY++)
            {
                for (uint ctbX = startX; ctbX < stopX; ctbX++)
                {
                    CtbAddrInSlice[sliceIdx][NumCtusInSlice[sliceIdx]] = ctbY * PicWidthInCtbsY + ctbX;
                    NumCtusInSlice[sliceIdx]++;
                }
            }
        }

        public void OnStRefPicFlag(uint listIdx, uint rplsIdx, RefPicListStruct refPicListStruct)
        {
            var num_ref_entries = refPicListStruct.NumRefEntries;
            var inter_layer_ref_pic_flag = refPicListStruct.InterLayerRefPicFlag;
            var st_ref_pic_flag = refPicListStruct.StRefPicFlag;
            var sps_num_ref_pic_lists = SeqParameterSetRbsp.SpsNumRefPicLists;

            if (NumLtrpEntries == null || NumLtrpEntries.Length < sps_num_ref_pic_lists.Length)
            {
                NumLtrpEntries = new uint[sps_num_ref_pic_lists.Length][];
            }

            if(NumLtrpEntries[listIdx] == null)
                NumLtrpEntries[listIdx] = new uint[sps_num_ref_pic_lists[listIdx] + 1];

            NumLtrpEntries[listIdx][rplsIdx] = 0;

            for (int i = 0; i < num_ref_entries[listIdx][rplsIdx]; i++)
                if (inter_layer_ref_pic_flag[listIdx][rplsIdx][i] == 0 && st_ref_pic_flag[listIdx][rplsIdx][i] == 0)
                    NumLtrpEntries[listIdx][rplsIdx]++;
        }

        public void OnRplIdx(RefPicLists refPicLists, uint i)
        {
            var rpl_sps_flag = refPicLists.RplSpsFlag;
            var rpl_idx = refPicLists.RplIdx;
            var sps_num_ref_pic_lists = SeqParameterSetRbsp.SpsNumRefPicLists;

            RplsIdx[i] = rpl_sps_flag[i] != 0 ? rpl_idx[i] : sps_num_ref_pic_lists[i];
        }

        public void OnNumL0Weights(uint num_l0_weights)
        {
            var pps_wp_info_in_ph_flag = PicParameterSetRbsp.PpsWpInfoInPhFlag;

            if (pps_wp_info_in_ph_flag == 1)
                NumWeightsL0 = num_l0_weights;
            else
                NumWeightsL0 = NumRefIdxActive[0];
        }

        public void OnShNumRefIdxActiveMinus1()
        {
            var sh_num_ref_idx_active_override_flag = SliceLayerRbsp.SliceHeader.ShNumRefIdxActiveOverrideFlag;
            var sh_slice_type = SliceLayerRbsp.SliceHeader.ShSliceType;
            var sh_num_ref_idx_active_minus1 = SliceLayerRbsp.SliceHeader.ShNumRefIdxActiveMinus1;
            var pps_num_ref_idx_default_active_minus1 = PicParameterSetRbsp.PpsNumRefIdxDefaultActiveMinus1;

            if(NumRefIdxActive == null || NumRefIdxActive.Length < 2)
                NumRefIdxActive = new uint[2];

            for (int i = 0; i < 2; i++)
            {
                if (sh_slice_type == H266FrameTypes.B || (sh_slice_type == H266FrameTypes.P && i == 0))
                {
                    if (sh_num_ref_idx_active_override_flag != 0)
                        NumRefIdxActive[i] = sh_num_ref_idx_active_minus1[i] + 1;
                    else
                    {
                        var num_ref_entries = SliceLayerRbsp.SliceHeader.RefPicLists.RefPicListStruct.NumRefEntries;

                        if (num_ref_entries[i][RplsIdx[i]] >= pps_num_ref_idx_default_active_minus1[i] + 1)
                            NumRefIdxActive[i] = pps_num_ref_idx_default_active_minus1[i] + 1;
                        else
                            NumRefIdxActive[i] = num_ref_entries[i][RplsIdx[i]];
                    }
                }
                else /* sh_slice_type  ==  I  | |  ( sh_slice_type  ==  P  &&  i  ==  1 ) */
                    NumRefIdxActive[i] = 0;
            }
        }

        public void OnPpsSubpicId()
        {
            var sps_num_subpics_minus1 = SeqParameterSetRbsp.SpsNumSubpicsMinus1;
            var sps_subpic_id_mapping_explicitly_signalled_flag = SeqParameterSetRbsp.SpsSubpicIdMappingExplicitlySignalledFlag;
            var pps_subpic_id_mapping_present_flag = PicParameterSetRbsp.PpsSubpicIdMappingPresentFlag;
            var pps_subpic_id = PicParameterSetRbsp.PpsSubpicId;
            var sps_subpic_id = SeqParameterSetRbsp.SpsSubpicId;

            if (SubpicIdVal == null || SubpicIdVal.Length < sps_num_subpics_minus1 + 1)
                SubpicIdVal = new uint[sps_num_subpics_minus1 + 1];

            for (uint i = 0; i <= sps_num_subpics_minus1; i++)
                if (sps_subpic_id_mapping_explicitly_signalled_flag != 0)
                    SubpicIdVal[i] = pps_subpic_id_mapping_present_flag != 0 ? pps_subpic_id[i] : sps_subpic_id[i];
                else
                    SubpicIdVal[i] = i;
        }

        public void OnShSubpicId(uint sh_subpic_id)
        {
            CurrSubpicIdx = Array.IndexOf(SubpicIdVal, sh_subpic_id);
        }

        public void OnSpsExtraShBitPresentFlag()
        {
            var sps_num_extra_sh_bytes = SeqParameterSetRbsp.SpsNumExtraShBytes;
            var sps_extra_sh_bit_present_flag = SeqParameterSetRbsp.SpsExtraShBitPresentFlag;

            NumExtraShBits = 0;
            for (int i = 0; i < (sps_num_extra_sh_bytes * 8); i++)
                if (sps_extra_sh_bit_present_flag[i] != 0)
                    NumExtraShBits++;
        }

        public void OnShSliceHeaderExtensionDataByte()
        {
            var sps_entry_point_offsets_present_flag = SeqParameterSetRbsp.SpsEntryPointOffsetsPresentFlag;
            var sps_entropy_coding_sync_enabled_flag = SeqParameterSetRbsp.SpsEntropyCodingSyncEnabledFlag;

            NumEntryPoints = 0;
            if (sps_entry_point_offsets_present_flag != 0)
            {
                for (int i = 1; i < NumCtusInCurrSlice; i++)
                {
                    uint ctbAddrX = CtbAddrInCurrSlice[i] % PicWidthInCtbsY;
                    uint ctbAddrY = CtbAddrInCurrSlice[i] / PicWidthInCtbsY;
                    uint prevCtbAddrX = CtbAddrInCurrSlice[i - 1] % PicWidthInCtbsY;
                    uint prevCtbAddrY = CtbAddrInCurrSlice[i - 1] / PicWidthInCtbsY;
                    if (CtbToTileRowBd[ctbAddrY] != CtbToTileRowBd[prevCtbAddrY] ||
                    CtbToTileColBd[ctbAddrX] != CtbToTileColBd[prevCtbAddrX] ||
                    (ctbAddrY != prevCtbAddrY && sps_entropy_coding_sync_enabled_flag != 0))
                        NumEntryPoints++;
                }
            }
        }

        public void OnShNumTilesInSliceMinus1()
        {
            var pps_rect_slice_flag = PicParameterSetRbsp.PpsRectSliceFlag;
            var sh_slice_address = SliceLayerRbsp.SliceHeader.ShSliceAddress;
            var sh_num_tiles_in_slice_minus1 = SliceLayerRbsp.SliceHeader.ShNumTilesInSliceMinus1;

            if (CtbAddrInCurrSlice == null || CtbAddrInCurrSlice.Length < NumCtusInCurrSlice)
                CtbAddrInCurrSlice = new uint[NumCtusInCurrSlice];

            if (pps_rect_slice_flag != 0)
            {
                uint picLevelSliceIdx = sh_slice_address;
                for (int j = 0; j < CurrSubpicIdx; j++)
                    picLevelSliceIdx += NumSlicesInSubpic[j];
                NumCtusInCurrSlice = NumCtusInSlice[picLevelSliceIdx];
                for (int i = 0; i < NumCtusInCurrSlice; i++)
                    CtbAddrInCurrSlice[i] = CtbAddrInSlice[picLevelSliceIdx][i];
            }
            else
            {
                NumCtusInCurrSlice = 0;
                for (int tileIdx = (int)sh_slice_address; tileIdx <= sh_slice_address + sh_num_tiles_in_slice_minus1; tileIdx++)
                {
                    int tileX = tileIdx % NumTileColumns;
                    int tileY = tileIdx / NumTileColumns;
                    for (int ctbY = (int)TileRowBdVal[tileY]; ctbY < TileRowBdVal[tileY + 1]; ctbY++)
                    {
                        for (int ctbX = (int)TileColBdVal[tileX]; ctbX < TileColBdVal[tileX + 1]; ctbX++)
                        {
                            CtbAddrInCurrSlice[NumCtusInCurrSlice] = (uint)(ctbY * PicWidthInCtbsY + ctbX);
                            NumCtusInCurrSlice++;
                        }
                    }
                }
            }
        }

        public void OnNumL1Weights(uint num_l1_weights)
        {
            var pps_weighted_bipred_flag = PicParameterSetRbsp.PpsWeightedBipredFlag;
            var pps_wp_info_in_ph_flag = PicParameterSetRbsp.PpsWpInfoInPhFlag;
            var num_ref_entries = SliceLayerRbsp.SliceHeader.RefPicLists.RefPicListStruct.NumRefEntries;

            if (pps_weighted_bipred_flag == 0 ||
                (pps_wp_info_in_ph_flag != 0 && num_ref_entries[1][RplsIdx[1]] == 0))
                NumWeightsL1 = 0;
            else if (pps_wp_info_in_ph_flag != 0)
                NumWeightsL1 = num_l1_weights;
            else
                NumWeightsL1 = NumRefIdxActive[1];
        }

        public void OnAbsDeltaPocSt(uint listIdx, uint rplsIdx, uint i, RefPicListStruct refPicListStruct)
        {
            var sps_weighted_pred_flag = SeqParameterSetRbsp.SpsWeightedPredFlag;
            var sps_weighted_bipred_flag = SeqParameterSetRbsp.SpsWeightedBipredFlag;
            var sps_num_ref_pic_lists = SeqParameterSetRbsp.SpsNumRefPicLists;
            var num_ref_entries = refPicListStruct.NumRefEntries;

            if (AbsDeltaPocSt == null || AbsDeltaPocSt.Length < sps_num_ref_pic_lists.Length)
            {
                AbsDeltaPocSt = new uint[sps_num_ref_pic_lists.Length][][];
            }

            if (AbsDeltaPocSt[listIdx] == null)
                AbsDeltaPocSt[listIdx] = new uint[sps_num_ref_pic_lists[listIdx] + 1][];

            if(AbsDeltaPocSt[listIdx][rplsIdx] == null || AbsDeltaPocSt[listIdx][rplsIdx].Length < num_ref_entries[listIdx][rplsIdx])
                AbsDeltaPocSt[listIdx][rplsIdx] = new uint[num_ref_entries[listIdx][rplsIdx]];

            if ((sps_weighted_pred_flag != 0 || sps_weighted_bipred_flag != 0) && i != 0)
                AbsDeltaPocSt[listIdx][rplsIdx][i] = abs_delta_poc_st[listIdx][rplsIdx][i];
            else
                AbsDeltaPocSt[listIdx][rplsIdx][i] = abs_delta_poc_st[listIdx][rplsIdx][i] + 1;
        }
    }
}
