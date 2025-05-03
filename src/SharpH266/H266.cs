using System;

namespace SharpH266
{
    public class H266Constants
    {
        public const int ALF_APS = 0; // ALF parameters
        public const int LMCS_APS = 1; // LMCS parameters
        public const int SCALING_APS = 2; // ScalingList parameters
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
        public SeiPayload SeiPayload { get; set; }

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
        public int NumAlfFilters { get; private set; }
        public uint LmcsMaxBinIdx { get; private set; }

        public void SetSeiPayload(SeiPayload payload)
        {
            if (SeiPayload == null)
            {
                SeiPayload = payload;
            }

            throw new NotImplementedException();
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

            if(LayerUsedAsRefLayerFlag == null || LayerUsedAsRefLayerFlag.Length < vps_max_layers_minus1 + 1)
                LayerUsedAsRefLayerFlag = new uint[vps_max_layers_minus1 + 1];
            if(NumDirectRefLayers == null || NumDirectRefLayers.Length < vps_max_layers_minus1 + 1)
                NumDirectRefLayers = new int[vps_max_layers_minus1 + 1];
            if(NumRefLayers == null || NumRefLayers.Length < vps_max_layers_minus1 + 1)
                NumRefLayers = new int[vps_max_layers_minus1 + 1];
            if(GeneralLayerIdx == null || GeneralLayerIdx.Length < vps_max_layers_minus1 + 1)
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

            if(NumOutputLayersInOls == null || NumOutputLayersInOls.Length < TotalNumOlss)
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
            if(LayerUsedAsOutputLayerFlag == null || LayerUsedAsOutputLayerFlag.Length < vps_max_layers_minus1 + 1)
                LayerUsedAsOutputLayerFlag = new uint[vps_max_layers_minus1 + 1];
            if(MultiLayerOlsIdx == null || MultiLayerOlsIdx.Length < TotalNumOlss)
                LayerUsedAsOutputLayerFlag = new uint[TotalNumOlss];
            if(NumLayersInOls == null || NumLayersInOls.Length < TotalNumOlss)
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
    }
}
