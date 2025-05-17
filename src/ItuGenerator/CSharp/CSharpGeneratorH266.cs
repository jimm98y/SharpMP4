using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ItuGenerator.CSharp
{
    internal class CSharpGeneratorH266 : ICustomGenerator
    {
        public string AppendMethod(ItuCode field, MethodType methodType, string spacing, string retm)
        {
            string name = (field as ItuField).Name;

            if (name == "sei_payload" || name == "vui_payload")
            {
                retm = $"{spacing}stream.MarkCurrentBitsPosition();\r\n{retm}";
            }
            else if (name == "pt_cpb_removal_delay_minus1")
            {
                retm = $"if(this.pt_cpb_removal_delay_minus1 == null) this.pt_cpb_removal_delay_minus1 = new ulong[((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1 + 1];\r\n{retm}";
            }
            else if (name == "num_ref_entries")
            {
                if (methodType == MethodType.Read)
                    retm = "    \r\nthis.num_ref_entries = ((H266Context)context).num_ref_entries;\r\n            this.inter_layer_ref_pic_flag = ((H266Context)context).inter_layer_ref_pic_flag;\r\n            this.st_ref_pic_flag = ((H266Context)context).st_ref_pic_flag;\r\n            this.abs_delta_poc_st = ((H266Context)context).abs_delta_poc_st;\r\n            this.strp_entry_sign_flag = ((H266Context)context).strp_entry_sign_flag;\r\n            this.rpls_poc_lsb_lt = ((H266Context)context).rpls_poc_lsb_lt;\r\n            this.ilrp_idx = ((H266Context)context).ilrp_idx;\r\n" + retm + "\r\n ((H266Context)context).inter_layer_ref_pic_flag[listIdx][rplsIdx] = new byte[this.num_ref_entries[listIdx][rplsIdx]];\r\n            ((H266Context)context).st_ref_pic_flag[listIdx][rplsIdx] = new byte[this.num_ref_entries[listIdx][rplsIdx]];\r\n            ((H266Context)context).abs_delta_poc_st[listIdx][rplsIdx] = new ulong[this.num_ref_entries[listIdx][rplsIdx]];\r\n            ((H266Context)context).strp_entry_sign_flag[listIdx][rplsIdx] = new byte[this.num_ref_entries[listIdx][rplsIdx]];\r\n            ((H266Context)context).rpls_poc_lsb_lt[listIdx][rplsIdx] = new ulong[this.num_ref_entries[listIdx][rplsIdx]];\r\n            ((H266Context)context).ilrp_idx[listIdx][rplsIdx] = new ulong[this.num_ref_entries[listIdx][rplsIdx]];";
            }

            if (methodType == MethodType.Write)
            {
                string setVariables = "";
                if ((field as ItuField).ClassType == "ref_pic_list_struct")
                {
                    if ((field as ItuField).Parameter == "( i, j )")
                    {
                        setVariables = "this.ref_pic_list_struct.ListIdx = i;\r\n                    this.ref_pic_list_struct.RplsIdx = j;\r\n";
                    }
                    else if ((field as ItuField).Parameter == "(i, sps_num_ref_pic_lists[i])")
                    {
                        setVariables = "this.ref_pic_list_struct.ListIdx = i;\r\n                    this.ref_pic_list_struct.RplsIdx = ((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[i];\r\n";
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                else if ((field as ItuField).ClassType == "sublayer_hrd_parameters")
                {
                    setVariables = "sublayer_hrd_parameters.SubLayerId = i;\r\n";
                }
                retm = $"{setVariables}{retm}";
            }

            return retm;
        }

        public string PreprocessDefinitionsFile(string definitions)
        {
            definitions = definitions
                .Replace("sli_sublayer_info_present_flag ? 0", "sli_sublayer_info_present_flag != 0 ? 0")
                .Replace("subLayerInfoFlag ?", "subLayerInfoFlag != 0 ?")
                .Replace("dph_sei_single_component_flag ? ", "dph_sei_single_component_flag != 0 ?")
                .Replace("sps_rpl1_same_as_rpl0_flag ?", "sps_rpl1_same_as_rpl0_flag != 0 ?")
                .Replace("sps_sublayer_cpb_params_present_flag ?", "sps_sublayer_cpb_params_present_flag != 0 ?")
                .Replace("sps_same_qp_table_for_chroma_flag ? 1 : ( sps_joint_cbcr_enabled_flag ? 3 : 2 )", "sps_same_qp_table_for_chroma_flag != 0 ? 1 : ( sps_joint_cbcr_enabled_flag != 0 ? 3 : 2 ) ")
                .Replace("intrinsic_params_equal_flag ? 0 : num_views_minus1", "(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1)")
                .Replace("bp_sublayer_initial_cpb_removal_delay_present_flag ?", "bp_sublayer_initial_cpb_removal_delay_present_flag != 0 ?")
                .Replace("vps_sublayer_cpb_params_present_flag ? 0", "vps_sublayer_cpb_params_present_flag != 0 ? 0");
            return definitions;
        }

        public string GetFieldDefaultValue(ItuField field)
        {
            switch (field.Name)
            {
                default:
                    return "";
            }
        }

        public void FixClassParameters(ItuClass ituClass)
        {
            if (ituClass.ClassName.StartsWith("depth_rep_info_element"))
            {
                ituClass.ClassParameter = "()"; // remove all "out" parameters
            }
        }

        public string ReplaceParameter(string parameter)
        {
            switch (parameter)
            {
                case "TotalNumOlss":
                    return "((H266Context)context).TotalNumOlss";
                case "VpsNumDpbParams":
                    return "((H266Context)context).VpsNumDpbParams";
                case "NumOutputLayersInOls":
                    return "((H266Context)context).NumOutputLayersInOls";
                case "OutputLayerIdInOls":
                    return "((H266Context)context).OutputLayerIdInOls";
                case "NumSubLayersInLayerInOLS":
                    return "((H266Context)context).NumSubLayersInLayerInOLS";
                case "layerIncludedInOlsFlag":
                    return "((H266Context)context).layerIncludedInOlsFlag";
                case "LayerUsedAsOutputLayerFlag":
                    return "((H266Context)context).LayerUsedAsOutputLayerFlag";
                case "OutputLayerIdx":
                    return "((H266Context)context).OutputLayerIdx";
                case "NumMultiLayerOlss":
                    return "((H266Context)context).NumMultiLayerOlss";
                case "MultiLayerOlsIdx":
                    return "((H266Context)context).MultiLayerOlsIdx";
                case "LayerUsedAsRefLayerFlag":
                    return "((H266Context)context).LayerUsedAsRefLayerFlag";
                case "NumDirectRefLayers":
                    return "((H266Context)context).NumDirectRefLayers";
                case "NumRefLayers":
                    return "((H266Context)context).NumRefLayers";
                case "GeneralLayerIdx":
                    return "((H266Context)context).GeneralLayerIdx";
                case "DirectRefLayerIdx":
                    return "((H266Context)context).DirectRefLayerIdx";
                case "ReferenceLayerIdx":
                    return "((H266Context)context).ReferenceLayerIdx";
                case "NumLayersInOls":
                    return "((H266Context)context).NumLayersInOls";
                case "LayerIdInOls":
                    return "((H266Context)context).LayerIdInOls";
                case "CtbLog2SizeY":
                    return "((H266Context)context).CtbLog2SizeY";
                case "CtbSizeY":
                    return "((H266Context)context).CtbSizeY";
                case "MaxNumMergeCand":
                    return "((H266Context)context).MaxNumMergeCand";
                case "NumAlfFilters":
                    return "((H266Context)context).NumAlfFilters";
                case "LmcsMaxBinIdx":
                    return "((H266Context)context).LmcsMaxBinIdx";
                case "sps_poc_msb_cycle_flag":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsPocMsbCycleFlag";
                case "sps_alf_enabled_flag":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsAlfEnabledFlag";
                case "sps_chroma_format_idc":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsChromaFormatIdc";
                case "sps_explicit_scaling_list_enabled_flag":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsExplicitScalingListEnabledFlag";
                case "sps_virtual_boundaries_enabled_flag":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsVirtualBoundariesEnabledFlag";
                case "sps_virtual_boundaries_present_flag":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsVirtualBoundariesPresentFlag";
                case "pps_alf_info_in_ph_flag":
                    return "((H266Context)context).PicParameterSetRbsp.PpsAlfInfoInPhFlag";
                case "pps_output_flag_present_flag":
                    return "((H266Context)context).PicParameterSetRbsp.PpsOutputFlagPresentFlag";
                case "pps_rpl_info_in_ph_flag":
                    return "((H266Context)context).PicParameterSetRbsp.PpsRplInfoInPhFlag";
                case "sps_partition_constraints_override_enabled_flag":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsPartitionConstraintsOverrideEnabledFlag";
                case "sps_qtbtt_dual_tree_intra_flag":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsQtbttDualTreeIntraFlag";
                case "pps_cu_qp_delta_enabled_flag":
                    return "((H266Context)context).PicParameterSetRbsp.PpsCuQpDeltaEnabledFlag";
                case "pps_cu_chroma_qp_offset_list_enabled_flag":
                    return "((H266Context)context).PicParameterSetRbsp.PpsCuChromaQpOffsetListEnabledFlag";
                case "sps_temporal_mvp_enabled_flag":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsTemporalMvpEnabledFlag";
                case "NumExtraPhBits":
                    return "((H266Context)context).NumExtraPhBits";
                case "sps_ccalf_enabled_flag":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsCcalfEnabledFlag";
                case "sps_lmcs_enabled_flag":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsLmcsEnabledFlag";
                case "sps_mmvd_fullpel_only_enabled_flag":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsMmvdFullpelOnlyEnabledFlag";
                case "sps_bdof_control_present_in_ph_flag":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsBdofControlPresentInPhFlag";
                case "sps_dmvr_control_present_in_ph_flag":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsDmvrControlPresentInPhFlag";  
                case "sps_prof_control_present_in_ph_flag":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsProfControlPresentInPhFlag";
                case "sps_joint_cbcr_enabled_flag":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsJointCbcrEnabledFlag";
                case "sps_long_term_ref_pics_flag":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsLongTermRefPicsFlag";
                case "sps_num_ref_pic_lists":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists";
                case "sps_num_ref_pic_lists[i]":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[i]";
                case "sps_sao_enabled_flag":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsSaoEnabledFlag";
                case "sps_inter_layer_prediction_enabled_flag":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsInterLayerPredictionEnabledFlag";
                case "pps_picture_header_extension_present_flag":
                    return "((H266Context)context).PicParameterSetRbsp.PpsPictureHeaderExtensionPresentFlag";
                case "pps_chroma_tool_offsets_present_flag":
                    return "((H266Context)context).PicParameterSetRbsp.PpsChromaToolOffsetsPresentFlag";
                case "pps_deblocking_filter_disabled_flag":
                    return "((H266Context)context).PicParameterSetRbsp.PpsDeblockingFilterDisabledFlag";
                case "pps_dbf_info_in_ph_flag":
                    return "((H266Context)context).PicParameterSetRbsp.PpsDbfInfoInPhFlag";
                case "pps_weighted_pred_flag":
                    return "((H266Context)context).PicParameterSetRbsp.PpsWeightedPredFlag";
                case "pps_weighted_bipred_flag":
                    return "((H266Context)context).PicParameterSetRbsp.PpsWeightedBipredFlag";
                case "pps_qp_delta_info_in_ph_flag":
                    return "((H266Context)context).PicParameterSetRbsp.PpsQpDeltaInfoInPhFlag";
                case "pps_sao_info_in_ph_flag":
                    return "((H266Context)context).PicParameterSetRbsp.PpsSaoInfoInPhFlag";
                case "pps_wp_info_in_ph_flag":
                    return "((H266Context)context).PicParameterSetRbsp.PpsWpInfoInPhFlag";
                case "pps_rpl1_idx_present_flag":
                    return "((H266Context)context).PicParameterSetRbsp.PpsRpl1IdxPresentFlag";
                case "aps_chroma_present_flag":
                    return "((H266Context)context).AdaptationParameterSetRbsp.ApsChromaPresentFlag";
                case "bp_max_sublayers_minus1":
                    return "((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1";
                case "bp_cpb_cnt_minus1":
                    return "((H266Context)context).SeiPayload.BufferingPeriod.BpCpbCntMinus1";
                case "bp_du_hrd_params_present_flag":
                    return "((H266Context)context).SeiPayload.BufferingPeriod.BpDuHrdParamsPresentFlag"; 
                case "bp_du_dpb_params_in_pic_timing_sei_flag":
                    return "((H266Context)context).SeiPayload.BufferingPeriod.BpDuDpbParamsInPicTimingSeiFlag";
                case "bp_cpb_removal_delay_deltas_present_flag":
                    return "((H266Context)context).SeiPayload.BufferingPeriod.BpCpbRemovalDelayDeltasPresentFlag";
                case "bp_du_cpb_params_in_pic_timing_sei_flag":
                    return "((H266Context)context).SeiPayload.BufferingPeriod.BpDuCpbParamsInPicTimingSeiFlag";
                case "bp_alt_cpb_params_present_flag":
                    return "((H266Context)context).SeiPayload.BufferingPeriod.BpAltCpbParamsPresentFlag";
                case "bp_nal_hrd_params_present_flag":
                    return "((H266Context)context).SeiPayload.BufferingPeriod.BpNalHrdParamsPresentFlag";
                case "bp_additional_concatenation_info_present_flag":
                    return "((H266Context)context).SeiPayload.BufferingPeriod.BpAdditionalConcatenationInfoPresentFlag"; 
                case "bp_num_cpb_removal_delay_deltas_minus1":
                    return "((H266Context)context).SeiPayload.BufferingPeriod.BpNumCpbRemovalDelayDeltasMinus1"; 
                case "bp_vcl_hrd_params_present_flag":
                    return "((H266Context)context).SeiPayload.BufferingPeriod.BpVclHrdParamsPresentFlag"; 
                case "bp_sublayer_initial_cpb_removal_delay_present_flag":
                    return "((H266Context)context).SeiPayload.BufferingPeriod.BpSublayerInitialCpbRemovalDelayPresentFlag"; 
                case "general_nal_hrd_params_present_flag":
                    return "((H266Context)context).GeneralTimingHrdParameters.GeneralNalHrdParamsPresentFlag";
                case "general_vcl_hrd_params_present_flag":
                    return "((H266Context)context).GeneralTimingHrdParameters.GeneralVclHrdParamsPresentFlag";
                case "general_du_hrd_params_present_flag":
                    return "((H266Context)context).GeneralTimingHrdParameters.GeneralDuHrdParamsPresentFlag";
                case "hrd_cpb_cnt_minus1":
                    return "((H266Context)context).GeneralTimingHrdParameters.HrdCpbCntMinus1";
                case "NumTilesInPic":
                    return "((H266Context)context).NumTilesInPic";
                case "ltrp_in_header_flag":
                    return "ref_pic_list_struct.LtrpInHeaderFlag";
                case "colourTransformSize":
                    return "( (1  <<  ( (int)colour_transform_log2_number_of_points_per_lut_minus1 + 1 ) ) + 1 )";
                case "nal_unit_type":
                    return "((H266Context)context).NalHeader.NalUnitHeader.NalUnitType";
                case "num_ref_entries":
                    return "((H266Context)context).num_ref_entries";
                case "TemporalId":
                    return "(((H266Context)context).NalHeader.NalUnitHeader.NuhTemporalIdPlus1 - 1)";
                case "PicWidthInCtbsY":
                    return "((H266Context)context).PicWidthInCtbsY";
                case "PicHeightInCtbsY":
                    return "((H266Context)context).PicHeightInCtbsY";
                case "PicSizeInCtbsY":
                    return "((H266Context)context).PicSizeInCtbsY";
                case "PicWidthInMinCbsY":
                    return "((H266Context)context).PicWidthInMinCbsY";
                case "PicHeightInMinCbsY":
                    return "((H266Context)context).PicHeightInMinCbsY";
                case "PicSizeInMinCbsY":
                    return "((H266Context)context).PicSizeInMinCbsY";
                case "PicSizeInSamplesY":
                    return "((H266Context)context).PicSizeInSamplesY";
                case "PicWidthInSamplesC":
                    return "((H266Context)context).PicWidthInSamplesC";
                case "PicHeightInSamplesC":
                    return "((H266Context)context).PicHeightInSamplesC";
                case "SubWidthC":
                    return "((H266Context)context).SubWidthC";
                case "SubHeightC":
                    return "((H266Context)context).SubHeightC";
                case "MinCbLog2SizeY":
                    return "((H266Context)context).MinCbLog2SizeY";
                case "MinCbSizeY":
                    return "((H266Context)context).MinCbSizeY";
                case "IbcBufWidthY":
                    return "((H266Context)context).IbcBufWidthY";
                case "IbcBufWidthC":
                    return "((H266Context)context).IbcBufWidthC";
                case "VSize":
                    return "((H266Context)context).VSize";
                case "CtbWidthC":
                    return "((H266Context)context).CtbWidthC";
                case "CtbHeightC":
                    return "((H266Context)context).CtbHeightC";
                case "NumTileColumns":
                    return "((H266Context)context).NumTileColumns";
                case "NumTileRows":
                    return "((H266Context)context).NumTileRows";
                case "ColWidthVal":
                    return "((H266Context)context).ColWidthVal";
                case "RowHeightVal":
                    return "((H266Context)context).RowHeightVal";
                case "TileColBdVal":
                    return "((H266Context)context).TileColBdVal";
                case "TileRowBdVal":
                    return "((H266Context)context).TileRowBdVal";
                case "CtbToTileColBd":
                    return "((H266Context)context).CtbToTileColBd";
                case "ctbToTileColIdx":
                    return "((H266Context)context).ctbToTileColIdx";
                case "CtbToTileRowBd":
                    return "((H266Context)context).CtbToTileRowBd";
                case "ctbToTileRowIdx":
                    return "((H266Context)context).ctbToTileRowIdx";
                case "SubpicWidthInTiles":
                    return "((H266Context)context).SubpicWidthInTiles";
                case "SubpicHeightInTiles":
                    return "((H266Context)context).SubpicHeightInTiles";
                case "subpicHeightLessThanOneTileFlag":
                    return "((H266Context)context).subpicHeightLessThanOneTileFlag";
                case "NumCtusInSlice":
                    return "((H266Context)context).NumCtusInSlice";
                case "SliceTopLeftTileIdx":
                    return "((H266Context)context).SliceTopLeftTileIdx";
                case "sliceWidthInTiles":
                    return "((H266Context)context).sliceWidthInTiles";
                case "sliceHeightInTiles":
                    return "((H266Context)context).sliceHeightInTiles";
                case "NumSlicesInTile":
                    return "((H266Context)context).NumSlicesInTile";
                case "sliceHeightInCtus":
                    return "((H266Context)context).sliceHeightInCtus";
                case "CtbAddrInSlice":
                    return "((H266Context)context).CtbAddrInSlice";
                case "NumSlicesInSubpic":
                    return "((H266Context)context).NumSlicesInSubpic";
                case "SubpicIdxForSlice":
                    return "((H266Context)context).SubpicIdxForSlice";
                case "SubpicLevelSliceIdx":
                    return "((H266Context)context).SubpicLevelSliceIdx";
                case "NumLtrpEntries":
                    return "((H266Context)context).NumLtrpEntries";
                case "RplsIdx":
                    return "((H266Context)context).RplsIdx";
                case "OnNumL0Weights":
                    return "((H266Context)context).OnNumL0Weights";
                case "NumExtraShBits":
                    return "((H266Context)context).NumExtraShBits";
                case "CurrSubpicIdx":
                    return "((H266Context)context).CurrSubpicIdx";
                case "sps_subpic_info_present_flag":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsSubpicInfoPresentFlag";
                case "pps_rect_slice_flag":
                    return "((H266Context)context).PicParameterSetRbsp.PpsRectSliceFlag";
                case "pps_slice_chroma_qp_offsets_present_flag":
                    return "((H266Context)context).PicParameterSetRbsp.PpsSliceChromaQpOffsetsPresentFlag";
                case "pps_cabac_init_present_flag":
                    return "((H266Context)context).PicParameterSetRbsp.PpsCabacInitPresentFlag";
                case "sps_subpic_id_len_minus1":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsSubpicIdLenMinus1";
                case "sps_idr_rpl_present_flag":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsIdrRplPresentFlag";
                case "ph_inter_slice_allowed_flag":
                    return "((H266Context)context).SliceLayerRbsp.SliceHeader.PictureHeaderStructure.PhInterSliceAllowedFlag";   
                case "ph_lmcs_enabled_flag":
                    return "((H266Context)context).SliceLayerRbsp.SliceHeader.PictureHeaderStructure.PhLmcsEnabledFlag";
                case "ph_explicit_scaling_list_enabled_flag":
                    return "((H266Context)context).SliceLayerRbsp.SliceHeader.PictureHeaderStructure.PhExplicitScalingListEnabledFlag";
                case "ph_temporal_mvp_enabled_flag":
                    return "((H266Context)context).SliceLayerRbsp.SliceHeader.PictureHeaderStructure.PhTemporalMvpEnabledFlag";
                case "pps_deblocking_filter_override_enabled_flag":
                    return "((H266Context)context).PicParameterSetRbsp.PpsDeblockingFilterOverrideEnabledFlag";
                case "pps_slice_header_extension_present_flag":
                    return "((H266Context)context).PicParameterSetRbsp.PpsSliceHeaderExtensionPresentFlag";
                case "sps_dep_quant_enabled_flag":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsDepQuantEnabledFlag";
                case "sps_sign_data_hiding_enabled_flag":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsSignDataHidingEnabledFlag";
                case "sps_transform_skip_enabled_flag":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsTransformSkipEnabledFlag";
                case "NumWeightsL0":
                    return "((H266Context)context).NumWeightsL0";
                case "NumRefIdxActive":
                    return "((H266Context)context).NumRefIdxActive";
                case "SubpicIdVal":
                    return "((H266Context)context).SubpicIdVal";
                case "NumEntryPoints":
                    return "((H266Context)context).NumEntryPoints";
                case "NumWeightsL1":
                    return "((H266Context)context).NumWeightsL1";
                case "CtbAddrInCurrSlice":
                    return "((H266Context)context).CtbAddrInCurrSlice";
                case "NumCtusInCurrSlice":
                    return "((H266Context)context).NumCtusInCurrSlice";
                case "AbsDeltaPocSt":
                    return "((H266Context)context).AbsDeltaPocSt";

                default:
                    //throw new NotImplementedException(parameter);
                    return parameter;
            }
        }

        public string GetDerivedVariables(string name)
        {
            switch(name)
            {
                case "sei_payload":
                    return "((H266Context)context).SetSeiPayload(sei_payload);";
                case "general_timing_hrd_parameters":
                    return "((H266Context)context).SetGeneralTimingHrdParameters(general_timing_hrd_parameters);";
                case "vps_num_output_layer_sets_minus2":
                    return "((H266Context)context).OnVpsNumOutputLayerSetsMinus2();";
                case "vps_num_dpb_params_minus1":
                    return "((H266Context)context).OnVpsNumDpbParamsMinus1();";
                case "vps_ols_output_layer_flag":
                    return "((H266Context)context).OnVpsOlsOutputLayerFlag(j);";
                case "vps_direct_ref_layer_flag":
                    return "((H266Context)context).OnVpsDirectRefLayerFlag();";
                case "alf_chroma_filter_signal_flag":
                    return "((H266Context)context).OnAlfChromaFilterSignalFlag();";
                case "lmcs_delta_max_bin_idx":
                    return "((H266Context)context).OnLmcsDeltaMaxBinIdx();";
                case "sps_extra_ph_bit_present_flag":
                    return "((H266Context)context).OnSpsExtraPhBitPresentFlag();";
                case "sps_six_minus_max_num_merge_cand":
                    return "((H266Context)context).OnSpsSixMinusMaxNumMergeCand();";
                case "sps_log2_ctu_size_minus5":
                    return "((H266Context)context).OnSpsLog2CtuSizeMinus5();";
                case "pps_pic_height_in_luma_samples":
                    return "((H266Context)context).OnPpsPicHeightInLumaSamples();";
                case "sps_log2_min_luma_coding_block_size_minus2":
                    return "((H266Context)context).OnSpsLog2MinLumaCodingBlockSizeMinus2();";
                case "pps_tile_row_height_minus1": // TODO: not sure this is the right place
                    return "((H266Context)context).OnPpsTileRowHeightMinus1();";
                case "st_ref_pic_flag": 
                    return "((H266Context)context).OnStRefPicFlag(listIdx, rplsIdx, this);";
                case "rpl_idx": 
                    return "((H266Context)context).OnRplIdx(this, i);";
                case "num_l0_weights": 
                    return "((H266Context)context).OnNumL0Weights(num_l0_weights);";
                case "sh_num_ref_idx_active_minus1": 
                    return "((H266Context)context).OnShNumRefIdxActiveMinus1();";
                case "sh_subpic_id": 
                    return "((H266Context)context).OnShSubpicId(sh_subpic_id);";
                case "pps_subpic_id": 
                    return "((H266Context)context).OnPpsSubpicId();";
                case "sps_extra_sh_bit_present_flag": 
                    return "((H266Context)context).OnSpsExtraShBitPresentFlag();";
                case "sh_slice_header_extension_data_byte": 
                    return "((H266Context)context).OnShSliceHeaderExtensionDataByte();";
                case "sh_num_tiles_in_slice_minus1": 
                    return "((H266Context)context).OnShNumTilesInSliceMinus1();";
                case "num_l1_weights": 
                    return "((H266Context)context).OnNumL1Weights(num_l1_weights);";
                case "abs_delta_poc_st": 
                    return "((H266Context)context).OnAbsDeltaPocSt(listIdx, rplsIdx, i, this);";
            }
            return "";
        }

        public string FixMissingParameters(ItuClass b, string parameter, string classType)
        {
            if (parameter == null)
                parameter = "()";

            if (classType.StartsWith("depth_rep_info_element"))
                parameter = "()";

            return parameter;
        }

        public string FixCondition(string condition, MethodType methodType)
        {
            condition = condition.Replace("nal_unit_type != ", "nal_unit_type != H266NALTypes.");
            condition = condition.Replace("nal_unit_type == ", "nal_unit_type == H266NALTypes.");
            condition = condition.Replace("nal_unit_type >= ", "nal_unit_type >= H266NALTypes.");
            condition = condition.Replace("nal_unit_type <= ", "nal_unit_type <= H266NALTypes.");
            condition = condition.Replace("nal_unit_type  !=  ", "nal_unit_type != H266NALTypes.");
            condition = condition.Replace("nal_unit_type  ==  ", "nal_unit_type == H266NALTypes.");
            condition = condition.Replace("nal_unit_type  >=  ", "nal_unit_type >= H266NALTypes.");
            condition = condition.Replace("nal_unit_type  <=  ", "nal_unit_type <= H266NALTypes.");

            condition = condition.Replace("sh_slice_type != ", "sh_slice_type != H266FrameTypes.");
            condition = condition.Replace("sh_slice_type == ", "sh_slice_type == H266FrameTypes.");

            condition = condition.Replace("payload_type_byte  ==  0xFF", "payload_type_byte[whileIndex]  ==  0xFF");
            condition = condition.Replace("payload_size_byte  ==  0xFF", "payload_size_byte[whileIndex]  ==  0xFF");

            condition = condition.Replace("AbsDeltaPocSt[ i ]", "(((((H266Context)context).SeqParameterSetRbsp.SpsWeightedPredFlag != 0 || ((H266Context)context).SeqParameterSetRbsp.SpsWeightedBipredFlag != 0) && i != 0) ? abs_delta_poc_st[i] : (abs_delta_poc_st[i] + 1))");
            condition = condition.Replace("- sh_slice_address", "- (int)sh_slice_address");

            condition = condition.Replace("ALF_APS", "H266Constants.ALF_APS");
            condition = condition.Replace("LMCS_APS", "H266Constants.LMCS_APS");
            condition = condition.Replace("SCALING_APS", "H266Constants.SCALING_APS");

            condition = condition.Replace("Abs(", "(uint)Math.Abs(");
            condition = condition.Replace("Min(", "(uint)Math.Min(");
            condition = condition.Replace("Max(", "(uint)Math.Max(");
            condition = condition.Replace("byte_aligned()", "stream.ByteAligned()");
            condition = condition.Replace("more_data_in_payload()", "(!(stream.ByteAligned() && 8 * payloadSize == stream.GetBitsPositionSinceLastMark()))");
            condition = condition.Replace("payload_extension_present()", "stream.ReadMoreRbspData(this, payloadSize)");
            condition = condition.Replace("more_rbsp_data()", $"stream.{(methodType == MethodType.Read ? "Read" : "Write")}MoreRbspData(this)");
            condition = condition.Replace("next_bits(", $"stream.{(methodType == MethodType.Read ? "Read" : "Write")}NextBits(this, ");

            condition = condition.Replace("i = MaxNumSubLayersMinus1 - 1", "i = (int)MaxNumSubLayersMinus1 - 1");
            condition = condition.Replace("i = maxNumSubLayersMinus1;", "i = (int)maxNumSubLayersMinus1;");
            condition = condition.Replace("i < maxNumSubLayersMinus1", "i < (int)maxNumSubLayersMinus1");
            condition = condition.Replace("i = lmcs_min_bin_idx", "i = (uint)lmcs_min_bin_idx");
            return condition;
        }

        public string FixStatement(string fieldValue)
        {
            fieldValue = fieldValue.Replace("Abs(", "(uint)Math.Abs(");
            fieldValue = fieldValue.Replace("Min(", "(uint)Math.Min(");
            fieldValue = fieldValue.Replace("Max(", "(uint)Math.Max(");

            fieldValue = fieldValue.Replace("/* LukasV added default */", "/* LukasV added default */;\r\n((H266Context)context).OnStRefPicFlag(listIdx, rplsIdx, this)");
            return fieldValue;
        }

        public string GetCtorParameterType(string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter))
                return "";

            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                { "NumBytesInNalUnit",             "u(32)" },
                { "payloadType",                   "u(32)" },
                { "payloadSize",                   "u(64)" },
                { "subLayerId",                    "u(32)" },
                { "OutSign",                       "u(32)" },
                { "OutExp",                        "u(32)" },
                { "OutMantissa",                   "u(32)" },
                { "OutManLen",                     "u(32)" },
                { "i",                             "u(32)" },
                { "profileTierPresentFlag",        "u(32)" },
                { "MaxNumSubLayersMinus1",         "u(32)" },
                { "MaxSubLayersMinus1",            "u(32)" },
                { "subLayerInfoFlag",              "u(32)" },
                { "firstSubLayer",                 "u(32)" },
                { "MaxSubLayersVal",               "u(32)" },
                { "listIdx",                       "u(32)" },
                { "rplsIdx",                       "u(64)" },
            };

            return map[parameter];
        }

        public string GetParameterType(string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter))
                return "";

            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                { "levelVal",               "i(64)" },
                { "levelCode",              "i(64)" },
                { "coeffNum",               "i(64)" },
                { "coeffLevel",             "i(64)" },
                { "numComps",               "i(64)" },
                { "numViews",               "u(64)" },
                { "mapUnitCnt",             "u(64)" },
                { "DepthViewId",            "u(64)" },
                { "numRefDisplays",         "u(64)" },
                { "numValues",              "u(64)" },
                { "mantissaPred",           "u(64)" },
                { "psIdx",                  "u(64)" },
                { "numSignificantSets",     "u(64)" },
                { "currLsIdx",              "u(64)" },
                { "numQpTables",            "i(64)" },
                { "st_ref_pic_flag",        null },
            };

            if (map.ContainsKey(parameter))
                return map[parameter];
            else
                return "u(32)";
        }

        public string FixFieldValue(string fieldValue)
        {
            fieldValue = fieldValue.Replace("scaling_list_dc_coef[ id - 14 ]", "(uint)scaling_list_dc_coef[ id - 14 ][ id ]");
            fieldValue = fieldValue.Replace("scaling_list_delta_coef[ id ][ i ]", "(uint)scaling_list_delta_coef[ id ][ i ]");
            return fieldValue;
        }

        public void FixMethodAllocation(string name, ref string method, ref string typedef)
        {
            
        }

        public string GetVariableSize(string parameter)
        {
            switch(parameter)
            {
                case "sps_subpic_ctu_top_left_x":
                    return "(uint)Math.Ceiling( Math.Log2(  ( sps_pic_width_max_in_luma_samples + ((H266Context)context).CtbSizeY - 1 ) / ((H266Context)context).CtbSizeY ) )";
                case "sps_subpic_width_minus1":
                case "sps_subpic_height_minus1":
                case "sps_subpic_ctu_top_left_y":
                    return "(uint)Math.Ceiling( Math.Log2(  ( sps_pic_height_max_in_luma_samples + ((H266Context)context).CtbSizeY - 1 ) / ((H266Context)context).CtbSizeY ) )";
                case "sps_subpic_id":
                    return "sps_subpic_id_len_minus1 + 1";
                case "ph_pic_order_cnt_lsb":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsLog2MaxPicOrderCntLsbMinus4 + 4";
                case "ph_poc_msb_cycle_val":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsPocMsbCycleLenMinus1 + 1";
                case "alf_luma_coeff_delta_idx":
                    return "(uint)Math.Ceiling( Math.Log2( alf_luma_num_filters_signalled_minus1 + 1 ) )";
                case "ar_object_confidence":
                    return "ar_object_confidence_length_minus1 + 1";
                case "lmcs_delta_abs_cw":
                    return "lmcs_delta_cw_prec_minus1 + 1";
                case "sdi_view_id_val":
                    return "sdi_view_id_len_minus1 + 1";
                case "bp_cpb_removal_delay_delta_val":
                    return "bp_cpb_removal_delay_length_minus1 + 1";
                case "pt_dpb_output_delay":
                    return "((H266Context)context).SeiPayload.BufferingPeriod.BpDpbOutputDelayLengthMinus1 + 1";
                case "pt_dpb_output_du_delay":
                    return "((H266Context)context).SeiPayload.BufferingPeriod.BpDpbOutputDelayDuLengthMinus1 + 1";
                case "pt_nal_cpb_alt_initial_removal_delay_delta":
                case "pt_nal_cpb_alt_initial_removal_offset_delta":
                case "pt_vcl_cpb_alt_initial_removal_delay_delta":
                case "pt_vcl_cpb_alt_initial_removal_offset_delta":
                    return "((H266Context)context).SeiPayload.BufferingPeriod.BpCpbInitialRemovalDelayLengthMinus1 + 1";
                case "pt_nal_cpb_delay_offset":
                case "pt_vcl_cpb_delay_offset":
                    return "((H266Context)context).SeiPayload.BufferingPeriod.BpCpbRemovalDelayLengthMinus1 + 1";
                case "pt_nal_dpb_delay_offset":
                case "pt_vcl_dpb_delay_offset":
                    return "((H266Context)context).SeiPayload.BufferingPeriod.BpDpbOutputDelayLengthMinus1 + 1";
                case "pps_subpic_id":
                    return "pps_subpic_id_len_minus1 + 1";
                case "vui_reserved_payload_extension_data":
                    return "0 /* TODO */"; // TODO: this should not be present, but if present, then it's 8 * payloadSize - nEarlierBits - nPayloadZeroBits - 1
                case "rpls_poc_lsb_lt":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsLog2MaxPicOrderCntLsbMinus4 + 4";
                case "sei_reserved_payload_extension_data":
                    return "0 /* TODO */"; // TODO: this should not be present, but if present, then it's 8 * payloadSize - nEarlierBits - nPayloadZeroBits - 1
                case "mantissa_focal_length_x":
                    return "(exponent_focal_length_x[ i ] == 0 ? Math.Max( 0, prec_focal_length - 30 ) : Math.Max( 0, exponent_focal_length_x[ i ] + prec_focal_length - 31 ))";
                case "mantissa_focal_length_y":
                    return "(exponent_focal_length_y[ i ] == 0 ? Math.Max( 0, prec_focal_length - 30 ) : Math.Max( 0, exponent_focal_length_y[ i ] + prec_focal_length - 31 ))";
                case "mantissa_principal_point_x":
                    return "(exponent_principal_point_x[ i ] == 0 ? Math.Max( 0, prec_principal_point - 30 ) : Math.Max( 0, exponent_principal_point_x[ i ] + prec_principal_point - 31 ))";
                case "mantissa_principal_point_y":
                    return "(exponent_principal_point_y[ i ] == 0 ? Math.Max( 0, prec_principal_point - 30 ) : Math.Max( 0, exponent_principal_point_y[ i ] + prec_principal_point - 31 ))";
                case "mantissa_skew_factor":
                    return "(exponent_skew_factor[ i ] == 0 ? Math.Max( 0, prec_skew_factor - 30 ) : Math.Max( 0, exponent_skew_factor[ i ] + prec_skew_factor - 31 ))";
                case "mantissa_r":
                    return "(exponent_r[ i ][ j ][ k ]  == 0 ? Math.Max( 0, prec_rotation_param - 30 ) : Math.Max( 0, exponent_r[ i ][ j ][ k ] + prec_rotation_param - 31 ))";
                case "mantissa_t":
                    return "(exponent_t[ i ][ j ]  == 0 ? Math.Max( 0, prec_translation_param - 30 ) : Math.Max( 0, exponent_t[ i ][ j ] + prec_translation_param - 31 ))";
                case "da_mantissa":
                    return "(this.da_mantissa_len_minus1 + 1)";
                case "alpha_transparent_value":
                case "alpha_opaque_value":
                    return "alpha_channel_bit_depth_minus8 + 9";
                case "colour_transf_lut":
                case "colour_transform_chroma_offset":
                    return "(uint)(2 + ( colour_transform_bit_depth_minus8 + 8 ) - ( colour_transform_log2_number_of_points_per_lut_minus1 + 1 ))"; // 2 + bitDepth – log2numLutPoints
                case "bp_max_initial_removal_delay_for_concatenation":
                    return "bp_cpb_initial_removal_delay_length_minus1 + 1";
                case "bp_cpb_removal_delay_delta_minus1":
                    return "bp_cpb_removal_delay_length_minus1 + 1";
                case "bp_nal_initial_cpb_removal_delay":
                case "bp_nal_initial_alt_cpb_removal_delay":
                case "bp_vcl_initial_cpb_removal_delay":
                case "bp_vcl_initial_alt_cpb_removal_delay":
                    return "bp_cpb_initial_removal_delay_length_minus1 + 1";
                case "bp_nal_initial_cpb_removal_offset":
                case "bp_nal_initial_alt_cpb_removal_offset":
                case "bp_vcl_initial_cpb_removal_offset":
                case "bp_vcl_initial_alt_cpb_removal_offset":
                    return "bp_cpb_initial_removal_delay_length_minus1 + 1";
                case "pt_cpb_removal_delay_minus1":
                    return "((H266Context)context).SeiPayload.BufferingPeriod.BpCpbRemovalDelayLengthMinus1 + 1";
                case "pt_du_common_cpb_removal_delay_increment_minus1":
                    return "((H266Context)context).SeiPayload.BufferingPeriod.BpDuCpbRemovalDelayIncrementLengthMinus1 + 1";
                case "sn_subpic_id":
                    return "sn_subpic_id_len_minus1 + 1";
                case "pt_cpb_removal_delay_delta_idx":
                    return "(uint)Math.Ceiling( Math.Log2( ((H266Context)context).SeiPayload.BufferingPeriod.BpNumCpbRemovalDelayDeltasMinus1 + 1 ) )";
                case "pt_du_cpb_removal_delay_increment_minus1":
                    return "((H266Context)context).SeiPayload.BufferingPeriod.BpDuCpbRemovalDelayIncrementLengthMinus1 + 1";
                case "dui_du_cpb_removal_delay_increment":
                    return "((H266Context)context).SeiPayload.BufferingPeriod.BpDuCpbRemovalDelayIncrementLengthMinus1 + 1";
                case "dui_dpb_output_du_delay":
                    return "((H266Context)context).SeiPayload.BufferingPeriod.BpDpbOutputDelayDuLengthMinus1 + 1";
                case "rpl_idx":
                    return "(uint)Math.Ceiling( Math.Log2( ((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[ i ] ) )";
                case "poc_lsb_lt":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsLog2MaxPicOrderCntLsbMinus4 + 4";
                case "sh_subpic_id":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsSubpicIdLenMinus1 + 1";
                case "sh_slice_address":
                    return "(uint)(((H266Context)context).PicParameterSetRbsp.PpsRectSliceFlag == 0 ? Math.Ceiling( Math.Log2 ( ((H266Context)context).NumTilesInPic ) )  :  Math.Ceiling( Math.Log2( ((H266Context)context).NumSlicesInSubpic[ ((H266Context)context).CurrSubpicIdx ] ) ) )";
                case "sh_entry_point_offset_minus1":
                    return "sh_entry_offset_len_minus1 + 1";
            }

            Debug.WriteLine(parameter);
            throw new NotImplementedException();
        }

        public string GetDerivedInstances(string field)
        {
            return "";
        }

        public string FixAllocations(string spacing, string appendType, string variableType, string variableName)
        {
            string ret;

            if (variableName == "ref_pic_list_struct")
            {
                if (variableType == "RefPicListStruct[ 2]")
                    ret = "\r\nif (((H266Context)context).num_ref_entries == null)\r\n                ((H266Context)context).num_ref_entries = new ulong[2][] { new ulong[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[0] + 1], new ulong[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[1] + 1] };\r\n            if (((H266Context)context).inter_layer_ref_pic_flag == null)\r\n                ((H266Context)context).inter_layer_ref_pic_flag = new byte[2][][] { new byte[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[0] + 1][], new byte[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[1] + 1][] };\r\n            if (((H266Context)context).st_ref_pic_flag == null)\r\n                ((H266Context)context).st_ref_pic_flag = new byte[2][][] { new byte[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[0] + 1][], new byte[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[1] + 1][] };\r\n            if (((H266Context)context).abs_delta_poc_st == null)\r\n                ((H266Context)context).abs_delta_poc_st = new ulong[2][][] { new ulong[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[0] + 1][], new ulong[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[1] + 1][] };\r\n            if (((H266Context)context).strp_entry_sign_flag == null)\r\n                ((H266Context)context).strp_entry_sign_flag = new byte[2][][] { new byte[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[0] + 1][], new byte[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[1] + 1][] };\r\n            if (((H266Context)context).rpls_poc_lsb_lt == null)\r\n                ((H266Context)context).rpls_poc_lsb_lt = new ulong[2][][] { new ulong[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[0] + 1][], new ulong[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[1] + 1][] };\r\n            if (((H266Context)context).ilrp_idx == null)\r\n                ((H266Context)context).ilrp_idx = new ulong[2][][] { new ulong[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[0] + 1][], new ulong[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[1] + 1][] };";
                else
                    ret = "\r\nif (((H266Context)context).num_ref_entries == null)\r\n                ((H266Context)context).num_ref_entries = new ulong[2][];\r\n            if (((H266Context)context).inter_layer_ref_pic_flag == null)\r\n                ((H266Context)context).inter_layer_ref_pic_flag = new byte[2][][];\r\n            if (((H266Context)context).st_ref_pic_flag == null)\r\n                ((H266Context)context).st_ref_pic_flag = new byte[2][][];\r\n            if (((H266Context)context).abs_delta_poc_st == null)\r\n                ((H266Context)context).abs_delta_poc_st = new ulong[2][][];\r\n            if (((H266Context)context).strp_entry_sign_flag == null)\r\n                ((H266Context)context).strp_entry_sign_flag = new byte[2][][];\r\n            if (((H266Context)context).rpls_poc_lsb_lt == null)\r\n                ((H266Context)context).rpls_poc_lsb_lt = new ulong[2][][];\r\n            if (((H266Context)context).ilrp_idx == null)\r\n                ((H266Context)context).ilrp_idx = new ulong[2][][];";
            }
            else if (variableName == "ref_pic_list_struct[ i ]")
            {
                ret = "\r\nif (((H266Context)context).num_ref_entries[i] == null)\r\n                    ((H266Context)context).num_ref_entries[i] = new ulong[sps_num_ref_pic_lists[i] + 1];\r\n                if (((H266Context)context).inter_layer_ref_pic_flag[i] == null)\r\n                    ((H266Context)context).inter_layer_ref_pic_flag[i] = new byte[sps_num_ref_pic_lists[i] + 1][];\r\n                if (((H266Context)context).st_ref_pic_flag[i] == null)\r\n                    ((H266Context)context).st_ref_pic_flag[i] = new byte[sps_num_ref_pic_lists[i] + 1][];\r\n                if (((H266Context)context).abs_delta_poc_st[i] == null)\r\n                    ((H266Context)context).abs_delta_poc_st[i] = new ulong[sps_num_ref_pic_lists[i] + 1][];\r\n                if (((H266Context)context).strp_entry_sign_flag[i] == null)\r\n                    ((H266Context)context).strp_entry_sign_flag[i] = new byte[sps_num_ref_pic_lists[i] + 1][];\r\n                if (((H266Context)context).rpls_poc_lsb_lt[i] == null)\r\n                    ((H266Context)context).rpls_poc_lsb_lt[i] = new ulong[sps_num_ref_pic_lists[i] + 1][];\r\n                if (((H266Context)context).ilrp_idx[i] == null)\r\n                    ((H266Context)context).ilrp_idx[i] = new ulong[sps_num_ref_pic_lists[i] + 1][];";
            }
            else if (variableName.StartsWith("sublayer_hrd_parameters"))
            {
                ret = "\r\nif(((H266Context)context).cbr_flag == null)\r\n                ((H266Context)context).cbr_flag = new byte[MaxSubLayersVal + 1][];\r\n            if(((H266Context)context).bit_rate_du_value_minus1 == null)\r\n                ((H266Context)context).bit_rate_du_value_minus1 = new ulong[MaxSubLayersVal + 1][];\r\n            if(((H266Context)context).cpb_size_du_value_minus1 == null)\r\n                ((H266Context)context).cpb_size_du_value_minus1 = new ulong[MaxSubLayersVal + 1][];\r\n            if(((H266Context)context).bit_rate_value_minus1 == null)\r\n                ((H266Context)context).bit_rate_value_minus1 = new ulong[MaxSubLayersVal + 1][];\r\n            if(((H266Context)context).cpb_size_value_minus1 == null)\r\n                ((H266Context)context).cpb_size_value_minus1 = new ulong[MaxSubLayersVal + 1][];\r\n";
            }
            else if (
                variableName.StartsWith("bit_rate_value_minus1") ||
                variableName.StartsWith("cpb_size_value_minus1") ||
                variableName.StartsWith("cpb_size_du_value_minus1") ||
                variableName.StartsWith("bit_rate_du_value_minus1")
                )
            {
                ret = null;
            }
            else if (variableName.StartsWith("cbr_flag"))
            {
                ret = "((H266Context)context).cbr_flag[subLayerId] = new byte[((H266Context)context).GeneralTimingHrdParameters.HrdCpbCntMinus1 + 1];\r\n            ((H266Context)context).bit_rate_du_value_minus1[subLayerId] = new ulong[((H266Context)context).GeneralTimingHrdParameters.HrdCpbCntMinus1 + 1];\r\n            ((H266Context)context).cpb_size_du_value_minus1[subLayerId] = new ulong[((H266Context)context).GeneralTimingHrdParameters.HrdCpbCntMinus1 + 1];\r\n            ((H266Context)context).bit_rate_value_minus1[subLayerId] = new ulong[((H266Context)context).GeneralTimingHrdParameters.HrdCpbCntMinus1 + 1];\r\n            ((H266Context)context).cpb_size_value_minus1[subLayerId] = new ulong[((H266Context)context).GeneralTimingHrdParameters.HrdCpbCntMinus1 + 1];\r\n            this.bit_rate_value_minus1 = ((H266Context)context).bit_rate_value_minus1;\r\n            this.cpb_size_value_minus1 = ((H266Context)context).cpb_size_value_minus1;\r\n            this.cpb_size_du_value_minus1 = ((H266Context)context).cpb_size_du_value_minus1;\r\n            this.bit_rate_du_value_minus1 = ((H266Context)context).bit_rate_du_value_minus1;\r\n            this.cbr_flag = ((H266Context)context).cbr_flag;\r\n";
            }
            else if (
                variableName.StartsWith("rpls_poc_lsb_lt") ||
                variableName.StartsWith("strp_entry_sign_flag") ||
                variableName.StartsWith("abs_delta_poc_st") ||
                variableName.StartsWith("st_ref_pic_flag") ||
                variableName.StartsWith("inter_layer_ref_pic_flag") ||
                variableName.StartsWith("ilrp_idx")
                )
            {
                ret = null;
            }
            else
            {
                ret = "";
            }
            
            return ret;
        }

        public string FixVariableType(string variableType)
        {
            if (variableType.Contains(" 0  &&  i  <=  "))
            {
                variableType = variableType.Replace(" 0  &&  i  <=  ", "");
            }
            if (variableType.Contains("[  0]"))
            {
                variableType = variableType.Replace("[  0]", "[MaxNumSubLayersMinus1]");
            }

            return variableType;
        }

        public string FixAppendType(string appendType, string variableName)
        {
            if (variableName == "ar_label")
            {
                appendType += "[]"; // TODO fix this workaround
            }
            else if (
                variableName == "colour_transf_lut" ||
                variableName == "scaling_list_dc_coef"
                )
            {
                appendType += "[]"; // TODO fix this workaround
            }

            return appendType;
        }

        public void FixNestedIndexes(List<string> ret, ItuField field)
        {         
            if (field != null && (
                field.Name == "ref_pic_list_struct" ||
                field.Name == "sublayer_hrd_parameters"
                ))
            {
                ret.Clear();
            }
        }
    }
}