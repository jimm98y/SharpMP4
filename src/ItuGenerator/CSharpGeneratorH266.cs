using System.Collections.Generic;

namespace ItuGenerator
{
    internal class CSharpGeneratorH266 : ICustomGenerator
    {
        public string PreprocessDefinitionsFile(string definitions)
        {
            definitions = definitions
                .Replace("sli_sublayer_info_present_flag ? 0", "sli_sublayer_info_present_flag != 0 ? 0")
                .Replace("subLayerInfoFlag ?", "subLayerInfoFlag != 0 ?")
                .Replace("[ listIdx ][ rplsIdx ]", "") // ref_pic_list_struct is a class, each instance is stored separately
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
                case "sps_subpic_width_minus1":
                case "sps_subpic_ctu_top_left_x":
                    return "(uint)Math.Ceiling( Math.Log2(  ( sps_pic_width_max_in_luma_samples + ((H266Context)context).CtbSizeY - 1 ) / ((H266Context)context).CtbSizeY ) )";
                case "sps_subpic_height_minus1":
                case "sps_subpic_ctu_top_left_y":
                    return "(uint)Math.Ceiling( Math.Log2(  ( sps_pic_height_max_in_luma_samples + ((H266Context)context).CtbSizeY - 1 ) / ((H266Context)context).CtbSizeY ) )";
                case "sps_subpic_id":
                    return "sps_subpic_id_len_minus1 + 1";
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
                case "pps_subpic_id":
                    return "pps_subpic_id_len_minus1 + 1";
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
                case "sps_num_ref_pic_lists[":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[";
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
                    return "((H266Context)context).VideoParameterSetRbsp.GeneralTimingHrdParameters.GeneralNalHrdParamsPresentFlag";
                case "general_vcl_hrd_params_present_flag":
                    return "((H266Context)context).VideoParameterSetRbsp.GeneralTimingHrdParameters.GeneralVclHrdParamsPresentFlag";
                case "general_du_hrd_params_present_flag":
                    return "((H266Context)context).VideoParameterSetRbsp.GeneralTimingHrdParameters.GeneralDuHrdParamsPresentFlag";
                case "hrd_cpb_cnt_minus1":
                    return "((H266Context)context).VideoParameterSetRbsp.GeneralTimingHrdParameters.HrdCpbCntMinus1";
                case "ltrp_in_header_flag":
                    return "ref_pic_list_struct[i].LtrpInHeaderFlag";
                case "poc_lsb_lt":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsLog2MaxPicOrderCntLsbMinus4 + 4";
                case "rpl_idx":
                    return "(uint)Math.Ceiling( Math.Log2( ((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[ i ] ) )";
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
                case "colour_transf_lut":
                    return "(uint)(2 + ( colour_transform_bit_depth_minus8 + 8 ) - ( colour_transform_log2_number_of_points_per_lut_minus1 + 1 ))";
                case "colourTransformSize":
                    return "( (1  <<  ( (int)colour_transform_log2_number_of_points_per_lut_minus1 + 1 ) ) + 1 )";
                case "nal_unit_type":
                    return "((H266Context)context).NalHeader.NalUnitHeader.NalUnitType";
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
                case "pt_cpb_removal_delay_delta_idx":
                    return "(uint)Math.Ceiling( Math.Log2( ((H266Context)context).SeiPayload.BufferingPeriod.BpNumCpbRemovalDelayDeltasMinus1 + 1 ) )";
                case "pt_cpb_removal_delay_minus1":
                    return "((H266Context)context).SeiPayload.BufferingPeriod.BpNumCpbRemovalDelayDeltasMinus1 + 1";
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
                case "pt_du_common_cpb_removal_delay_increment_minus1":
                    return "((H266Context)context).SeiPayload.BufferingPeriod.BpDuCpbRemovalDelayIncrementLengthMinus1 + 1";
                case "sn_subpic_id":
                    return "sn_subpic_id_len_minus1 + 1";
                case "mantissa_r":
                    return "(exponent_r[ i ][ j ][ k ]  == 0 ? Math.Max( 0, prec_rotation_param - 30 ) : Math.Max( 0, exponent_r[ i ][ j ][ k ] + prec_rotation_param - 31 ))";
                case "mantissa_t":
                    return "(exponent_t[ i ][ j ]  == 0 ? Math.Max( 0, prec_translation_param - 30 ) : Math.Max( 0, exponent_t[ i ][ j ] + prec_translation_param - 31 ))";
                case "rpls_poc_lsb_lt":
                    return "((H266Context)context).SeqParameterSetRbsp.SpsLog2MaxPicOrderCntLsbMinus4 + 4";
                case "num_ref_entries":
                    return "((H266Context)context).PictureHeaderRbsp.PictureHeaderStructure.RefPicLists.RefPicListStruct.First(x => x != null).NumRefEntries";
                case "pt_du_cpb_removal_delay_increment_minus1":
                    return "((H266Context)context).SeiPayload.BufferingPeriod.BpDuCpbRemovalDelayIncrementLengthMinus1 + 1";
                case "dui_du_cpb_removal_delay_increment":
                    return "((H266Context)context).SeiPayload.BufferingPeriod.BpDuCpbRemovalDelayIncrementLengthMinus1 + 1";
                case "TemporalId":
                    return "(((H266Context)context).NalHeader.NalUnitHeader.NuhTemporalIdPlus1 - 1)";

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
            condition = condition.Replace("nal_unit_type  !=  ", "nal_unit_type != H266NALTypes.");
            condition = condition.Replace("nal_unit_type  ==  ", "nal_unit_type == H266NALTypes.");
            condition = condition.Replace("nal_unit_type  >=  ", "nal_unit_type >= H266NALTypes.");
            condition = condition.Replace("nal_unit_type  <=  ", "nal_unit_type <= H266NALTypes.");

            condition = condition.Replace("payload_type_byte  ==  0xFF", "payload_type_byte[whileIndex]  ==  0xFF");
            condition = condition.Replace("payload_size_byte  ==  0xFF", "payload_size_byte[whileIndex]  ==  0xFF");

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
            return condition;
        }

        public string FixStatement(string fieldValue)
        {
            fieldValue = fieldValue.Replace("Abs(", "(uint)Math.Abs(");
            fieldValue = fieldValue.Replace("Min(", "(uint)Math.Min(");
            fieldValue = fieldValue.Replace("Max(", "(uint)Math.Max(");
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
                { "payloadSize",                   "u(32)" },
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
                { "rplsIdx",                       "u(32)" },
            };

            return map[parameter];
        }

        public string FixFieldValue(string fieldValue)
        {
            fieldValue = fieldValue.Replace("scaling_list_dc_coef[ id - 14 ]", "(uint)scaling_list_dc_coef[ id - 14 ][ id ]");
            fieldValue = fieldValue.Replace("scaling_list_delta_coef[ id ][ i ]", "(uint)scaling_list_delta_coef[ id ][ i ]");
            return fieldValue;
        }

        public void FixMethodAllocation(string name, ref string method, ref string typedef)
        {
            // TODO
        }
    }
}