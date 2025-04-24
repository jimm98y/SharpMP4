using System.Collections.Generic;

namespace ItuGenerator
{
    public class CSharpGeneratorH265 : ICustomGenerator
    {
        public string PreprocessDefinitionsFile(string definitions)
        {
            definitions = definitions
                            .Replace("intrinsic_params_equal_flag ? 0 : numViewsMinus1", "(intrinsic_params_equal_flag != 0 ? 0 : numViewsMinus1)")
                            .Replace("i = vps_base_layer_internal_flag ? 2 : 1;", "i = (uint)(vps_base_layer_internal_flag != 0 ? 2 : 1);")
                            .Replace("i = vps_base_layer_internal_flag ? 1 : 0;", "i = (uint)(vps_base_layer_internal_flag != 0 ? 1 : 0);")
                            .Replace("i = vps_base_layer_internal_flag ? 1 : 2;", "i = (uint)(vps_base_layer_internal_flag != 0 ? 1 : 2);")
                            .Replace("j = vps_base_layer_internal_flag ? 0 : 1;", "j = (uint)(vps_base_layer_internal_flag != 0 ? 0 : 1);")
                            .Replace("i = vps_base_layer_internal_flag ? 0 : 1;", "i = (uint)(vps_base_layer_internal_flag != 0 ? 0 : 1);")
                            .Replace("delta_dlt( i )", "delta_dlt()")
                            .Replace("1  <<  cm_octant_depth", "(uint)(1  <<  (int)cm_octant_depth)")
                            .Replace("_flag ?", "_flag != 0 ? ")
                            .Replace(" scalingList", " scalingLst"); // TODO remove this temporary fix
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
            if (ituClass.ClassName == "depth_rep_info_element")
            {
                ituClass.ClassParameter = "()"; // remove all "out" parameters
            }
        }

        public string ReplaceParameter(string parameter)
        {
            switch (parameter)
            {
                case "MaxSubLayersInLayerSetMinus1": // TODO: optimize/cache this
                    return "H265Helpers.GetMaxSubLayersInLayerSetMinus1(context)";
                case "chroma_format_idc":
                    return "((H265Context)context).SeqParameterSetRbsp.ChromaFormatIdc";
                case "sps_palette_predictor_initializer":
                    return "(((H265Context)context).SeqParameterSetRbsp.BitDepthChromaMinus8 + 8)";
                case "transform_skip_enabled_flag":
                    return "((H265Context)context).PicParameterSetRbsp.TransformSkipEnabledFlag";
                case "cabac_init_present_flag":
                    return "((H265Context)context).PicParameterSetRbsp.CabacInitPresentFlag";
                case "dependent_slice_segments_enabled_flag":
                    return "((H265Context)context).PicParameterSetRbsp.DependentSliceSegmentsEnabledFlag";
                case "chroma_qp_offset_list_enabled_flag":
                    return "((H265Context)context).PicParameterSetRbsp.PpsRangeExtension.ChromaQpOffsetListEnabledFlag";
                case "num_extra_slice_header_bits":
                    return "((H265Context)context).PicParameterSetRbsp.NumExtraSliceHeaderBits";
                case "pps_palette_predictor_initializer":
                    return "comp == 0 ? (((H265Context)context).PicParameterSetRbsp.PpsSccExtension.LumaBitDepthEntryMinus8 + 8) : (((H265Context)context).PicParameterSetRbsp.PpsSccExtension.ChromaBitDepthEntryMinus8 + 8)";
                case "MaxLayersMinus1":
                    return "Math.Min( 62, ((H265Context)context).VideoParameterSetRbsp.VpsMaxLayersMinus1)";
                case "NumLayerSets":
                    return "(((H265Context)context).VideoParameterSetRbsp.VpsNumLayerSetsMinus1 + 1 + ((H265Context)context).VideoParameterSetRbsp.VpsExtension.NumAddLayerSets)";
                case "lt_ref_pic_poc_lsb_sps":
                    return "(((H265Context)context).SeqParameterSetRbsp.Log2MaxPicOrderCntLsbMinus4 + 4)";
                case "num_ref_idx_l0_active_minus1":
                    return "((H265Context)context).SliceSegmentLayerRbsp.SliceSegmentHeader.NumRefIdxL0ActiveMinus1";
                case "num_cp":
                    return "((H265Context)context).VideoParameterSetRbsp.Vps3dExtension.NumCp";
                case "slice_type":
                    return "((H265Context)context).SliceSegmentLayerRbsp.SliceSegmentHeader.SliceType";
                case "nuh_layer_id":
                    return "((H265Context)context).NalHeader.NalUnitHeader.NuhLayerId";
                case "nal_unit_type":
                    return "((H265Context)context).NalHeader.NalUnitHeader.NalUnitType";
                case "vps_base_layer_internal_flag":
                    return "((H265Context)context).VideoParameterSetRbsp.VpsBaseLayerInternalFlag";
                case "vps_num_hrd_parameters":
                    return "((H265Context)context).VideoParameterSetRbsp.VpsNumHrdParameters";
                case "layer_id_in_nuh":
                    return "((H265Context)context).VideoParameterSetRbsp.VpsExtension.LayerIdInNuh";
                case "sub_pic_hrd_params_present_flag":
                    return "((H265Context)context).SeqParameterSetRbsp.VuiParameters.HrdParameters.SubPicHrdParamsPresentFlag";
                case "frame_field_info_present_flag":
                    return "((H265Context)context).SeqParameterSetRbsp.VuiParameters.FrameFieldInfoPresentFlag";
                case "num_ref_idx_l1_active_minus1":
                    return "((H265Context)context).SliceSegmentLayerRbsp.SliceSegmentHeader.NumRefIdxL1ActiveMinus1";
                case "num_short_term_ref_pic_sets":
                    return "((H265Context)context).SeqParameterSetRbsp.NumShortTermRefPicSets";
                case "num_long_term_ref_pics_sps":
                    return "((H265Context)context).SeqParameterSetRbsp.NumLongTermRefPicsSps";
                case "sub_pic_cpb_params_in_pic_timing_sei_flag":
                    return "((H265Context)context).SeqParameterSetRbsp.VuiParameters.HrdParameters.SubPicCpbParamsInPicTimingSeiFlag";
                case "list_entry_l0":
                    return " (uint)Math.Ceiling( Math.Log2( H265Helpers.GetNumPicTotalCurr(context) ) ) ";
                case "list_entry_l1":
                    return " (uint)Math.Ceiling( Math.Log2( H265Helpers.GetNumPicTotalCurr(context) ) ) ";
                case "NumPicTotalCurr":
                    return "H265Helpers.GetNumPicTotalCurr(context)";
                case "NumOutputLayersInOutputLayerSet":
                    return "H265Helpers.GetNumOutputLayersInOutputLayerSet(context)";
                case "inter_layer_pred_layer_idc":
                    return " (uint)Math.Ceiling( Math.Log2( H265Helpers.GetNumDirectRefLayers(context)[ ((H265Context)context).NalHeader.NalUnitHeader.NuhLayerId ] ) ) ";
                case "ChromaArrayType":
                    return "(((H265Context)context).SeqParameterSetRbsp.SeparateColourPlaneFlag == 0 ? ((H265Context)context).SeqParameterSetRbsp.ChromaFormatIdc : 0)";
                case "sei_ols_idx":
                    return "((H265Context)context).SeiRbsp.SeiMessage.Last().Value.SeiPayload.BspNesting.SeiOlsIdx";
                case "sei_partitioning_scheme_idx":
                    return "((H265Context)context).SeiRbsp.SeiMessage.Last().Value.SeiPayload.BspNesting.SeiPartitioningSchemeIdx";
                case "cm_octant_depth":
                    return "((H265Context)context).PicParameterSetRbsp.PpsMultilayerExtension.ColourMappingTable.CmOctantDepth";
                case "lists_modification_present_flag":
                    return "((H265Context)context).PicParameterSetRbsp.ListsModificationPresentFlag";
                case "CpbCnt":
                    return "H265Helpers.GetCpbCnt(context)";
                case "nal_initial_arrival_delay":
                    return "(((H265Context)context).VideoParameterSetRbsp.HrdParameters[i].InitialCpbRemovalDelayLengthMinus1 + 1)";
                case "cp_ref_voi":
                    return "((H265Context)context).VideoParameterSetRbsp.Vps3dExtension.CpRefVoi";
                case "cp_in_slice_segment_header_flag":
                    return "((H265Context)context).VideoParameterSetRbsp.Vps3dExtension.CpInSliceSegmentHeaderFlag";
                case "deblocking_filter_override_enabled_flag":
                    return "((H265Context)context).PicParameterSetRbsp.DeblockingFilterOverrideEnabledFlag";
                case "pps_slice_chroma_qp_offsets_present_flag":
                    return "((H265Context)context).PicParameterSetRbsp.PpsSliceChromaQpOffsetsPresentFlag";
                case "default_ref_layers_active_flag":
                    return "((H265Context)context).VideoParameterSetRbsp.VpsExtension.DefaultRefLayersActiveFlag";
                case "max_one_active_ref_layer_flag":
                    return "((H265Context)context).VideoParameterSetRbsp.VpsExtension.MaxOneActiveRefLayerFlag";
                case "poc_lsb_not_present_flag":
                    return "((H265Context)context).VideoParameterSetRbsp.VpsExtension.PocLsbNotPresentFlag";
                case "sample_adaptive_offset_enabled_flag":
                    return "((H265Context)context).SeqParameterSetRbsp.SampleAdaptiveOffsetEnabledFlag";
                case "NumOutputLayerSets":
                    return "((H265Context)context).VideoParameterSetRbsp.VpsExtension.NumOutputLayerSets";
                case "vps_poc_lsb_aligned_flag":
                    return "((H265Context)context).VideoParameterSetRbsp.VpsExtension.VpsPocLsbAlignedFlag";
                case "vps_num_layer_sets_minus1":
                    return "((H265Context)context).VideoParameterSetRbsp.VpsNumLayerSetsMinus1";
                case "vps_max_sub_layers_minus1":
                    return "((H265Context)context).VideoParameterSetRbsp.VpsMaxSubLayersMinus1";
                case "vps_max_layers_minus1":
                    return "((H265Context)context).VideoParameterSetRbsp.VpsMaxLayersMinus1";
                case "slice_segment_header_extension_present_flag":
                    return "((H265Context)context).PicParameterSetRbsp.SliceSegmentHeaderExtensionPresentFlag";
                case "tiles_enabled_flag":
                    return "((H265Context)context).PicParameterSetRbsp.TilesEnabledFlag";
                case "weighted_pred_flag":
                    return "((H265Context)context).PicParameterSetRbsp.WeightedPredFlag";
                case "weighted_bipred_flag":
                    return "((H265Context)context).PicParameterSetRbsp.WeightedBipredFlag";
                case "pps_loop_filter_across_slices_enabled_flag":
                    return "((H265Context)context).PicParameterSetRbsp.PpsLoopFilterAcrossSlicesEnabledFlag";
                case "entropy_coding_sync_enabled_flag":
                    return "((H265Context)context).PicParameterSetRbsp.EntropyCodingSyncEnabledFlag";
                case "poc_reset_info_present_flag":
                    return "((H265Context)context).PicParameterSetRbsp.PpsMultilayerExtension.PocResetInfoPresentFlag";
                case "output_flag_present_flag":
                    return "((H265Context)context).PicParameterSetRbsp.OutputFlagPresentFlag";
                case "sps_temporal_mvp_enabled_flag":
                    return "((H265Context)context).SeqParameterSetRbsp.SpsTemporalMvpEnabledFlag";
                case "sps_max_sub_layers_minus1":
                    return "((H265Context)context).SeqParameterSetRbsp.SpsMaxSubLayersMinus1";
                case "separate_colour_plane_flag":
                    return "((H265Context)context).SeqParameterSetRbsp.SeparateColourPlaneFlag";
                case "long_term_ref_pics_present_flag":
                    return "((H265Context)context).SeqParameterSetRbsp.LongTermRefPicsPresentFlag";
                case "depthMaxValue":
                    return "(( 1  <<  ( (int)((H265Context)context).PicParameterSetRbsp.Pps3dExtension.PpsBitDepthForDepthLayersMinus8 + 8 ) ) - 1)";
                case "NumViews":
                    return "H265Helpers.GetNumViews(context)";
                case "NumRefListLayers":
                    return "H265Helpers.GetNumRefListLayers(context)";
                case "NumDirectRefLayers":
                    return "H265Helpers.GetNumDirectRefLayers(context)";
                case "NumLayersInIdList":
                    return "H265Helpers.GetNumLayersInIdList(context)";
                case "NumIndependentLayers":
                    return "H265Helpers.GetNumIndependentLayers(context)";
                case "NumActiveRefLayerPics":
                    return "H265Helpers.GetNumActiveRefLayerPics(context)";
                case "NumDeltaPocs":
                    return "H265Helpers.GetNumDeltaPocs(context)";
                case "MaxTemporalId":
                    return "H265Helpers.GetMaxTemporalId(context)";
                case "OlsIdxToLsIdx":
                    return "H265Helpers.GetOlsIdxToLsIdx(context)";
                case "PicOrderCnt":
                    return "H265Helpers.GetPicOrderCnt";
                case "pic_layer_id":
                    return "H265Helpers.GetPicLayerId";
                case "RefPicList0":
                    return "H265Helpers.GetRefPicList0(context)";
                case "RefPicList1":
                    return "H265Helpers.GetRefPicList1(context)";
                case "BspSchedCnt":
                    return "H265Helpers.GetBspSchedCnt(context)";
                case "NecessaryLayerFlag":
                    return "H265Helpers.GetNecessaryLayerFlag(context)";
                case "IdDirectRefLayer":
                    return "H265Helpers.GetIdDirectRefLayer(context)";
                case "vclInitialArrivalDelayPresent":
                    return "H265Helpers.GetVclInitialArrivalDelayPresent(context)";
                case "LayerSetLayerIdList":
                    return "H265Helpers.GetLayerSetLayerIdList(context)";
                case "LayerIdxInVps":
                    return "H265Helpers.GetLayerIdxInVps(context)";
                case "OlsHighestOutputLayerId":
                    return "H265Helpers.GetOlsHighestOutputLayerId(context)";
                case "ViewOIdxList":
                    return "H265Helpers.GetViewOIdxList(context)";
                case "inCmpPredAvailFlag":
                    return "H265Helpers.GetInCmpPredAvailFlag(context)";
                case "nalInitialArrivalDelayPresent":
                    return "H265Helpers.GetNalInitialArrivalDelayPresent(context)";
                case "ViewIdx":
                    return "H265Helpers.GetViewOrderIdx(context)[ ((H265Context)context).NalHeader.NalUnitHeader.NuhLayerId ]";
                case "DepthFlag":
                    return "H265Helpers.GetDepthLayerFlag(context)[ ((H265Context)context).NalHeader.NalUnitHeader.NuhLayerId ]";
                case "defaultOutputLayerIdc":
                    return "Math.Min( ((H265Context)context).VideoParameterSetRbsp.VpsExtension.DefaultOutputLayerIdc, 2 )";
                case "PartNumY":
                    return "(1u  <<  (int)((H265Context)context).PicParameterSetRbsp.PpsMultilayerExtension.ColourMappingTable.CmyPartNumLog2)";
                case "numViewsMinus1":
                    return "(((H265Context)context).SeiRbsp.SeiMessage.Last().Value.SeiPayload.MultiviewAcquisitionInfo != null ? ((H265Context)context).SeiRbsp.SeiMessage.Last().Value.SeiPayload.ScalableNesting.NestingNumLayersMinus1 : 0)";
                case "NalHrdBpPresentFlag":
                    return "((H265Context)context).SeqParameterSetRbsp.VuiParameters.HrdParameters.NalHrdParametersPresentFlag";
                case "VclHrdBpPresentFlag":
                    return "((H265Context)context).SeqParameterSetRbsp.VuiParameters.HrdParameters.VclHrdParametersPresentFlag";
                case "CpbDpbDelaysPresentFlag":
                    return "(((H265Context)context).SeqParameterSetRbsp.VuiParameters.HrdParameters.NalHrdParametersPresentFlag == 1 || ((H265Context)context).SeqParameterSetRbsp.VuiParameters.HrdParameters.VclHrdParametersPresentFlag == 1 ? 1 : 0)";
                case "PocMsbValRequiredFlag":
                    return "((((H265Context)context).NalHeader.NalUnitHeader.NalUnitType == H265NALTypes.BLA_W_LP || ((H265Context)context).NalHeader.NalUnitHeader.NalUnitType == H265NALTypes.BLA_N_LP || ((H265Context)context).NalHeader.NalUnitHeader.NalUnitType == H265NALTypes.BLA_W_RADL || ((H265Context)context).NalHeader.NalUnitHeader.NalUnitType == H265NALTypes.CRA_NUT)  &&  ( ((H265Context)context).VideoParameterSetRbsp.VpsExtension.VpsPocLsbAlignedFlag == 0  || ( ((H265Context)context).VideoParameterSetRbsp.VpsExtension.VpsPocLsbAlignedFlag != 0  && H265Helpers.GetNumDirectRefLayers(context)[ ((H265Context)context).NalHeader.NalUnitHeader.NuhLayerId ]  ==  0 ) ) ? 1 : 0)";
                case "CurrPic":
                    return "H265Helpers.GetCurrPic(context)";
                case "RefRpsIdx":
                    return "H265Helpers.GetRefRpsIdx(context)";
                case "time_offset_value":
                    return "time_offset_length[i]";
                case "nal_initial_cpb_removal_delay":
                case "nal_initial_cpb_removal_offset":
                case "nal_initial_alt_cpb_removal_delay":
                case "nal_initial_alt_cpb_removal_offset":
                case "vcl_initial_cpb_removal_delay":
                case "vcl_initial_cpb_removal_offset":
                case "vcl_initial_alt_cpb_removal_delay":
                case "vcl_initial_alt_cpb_removal_offset":
                case "vcl_initial_arrival_delay":
                    return "(((H265Context)context).SeqParameterSetRbsp.VuiParameters.HrdParameters.InitialCpbRemovalDelayLengthMinus1 + 1)";
                case "du_cpb_removal_delay_increment_minus1":
                    return "(((H265Context)context).SeqParameterSetRbsp.VuiParameters.HrdParameters.DuCpbRemovalDelayIncrementLengthMinus1 + 1)";
                case "start_of_coded_interval":
                case "coded_pivot_value":
                    return "( ( coded_data_bit_depth + 7 )  >>  3 )  <<  3";
                case "target_pivot_value":
                    return "( ( target_bit_depth + 7 )  >>  3 )  <<  3";
                case "view_id_val":
                    return "view_id_len";
                case "dimension_id":
                    return "(dimension_id_len_minus1[i] + 1)";
                case "delta_val_diff_minus_min":
                    return "(uint)Math.Ceiling( Math.Log2( max_diff - min_diff_minus1 + 1 ) )";
                case "vps_rep_format_idx":
                    return "(uint)Math.Ceiling( Math.Log2( vps_num_rep_formats_minus1 + 1 ) )";
                case "lt_idx_sps":
                    return "(uint)Math.Ceiling( Math.Log2( ((H265Context)context).SeqParameterSetRbsp.NumLongTermRefPicsSps ) )";
                case "poc_lsb_lt":
                    return "( ((H265Context)context).SeqParameterSetRbsp.Log2MaxPicOrderCntLsbMinus4 + 4)";
                case "man_gvd_z_near":
                    return "(man_len_gvd_z_near_minus1[ i ] + 1)";
                case "man_gvd_z_far":
                    return "(man_len_gvd_z_far_minus1[ i ] + 1)";
                case "man_gvd_focal_length_x":
                    return "(exp_gvd_focal_length_x[ i ] == 0 ? Math.Max( 0, prec_gvd_focal_length - 30 ) : Math.Max( 0, exp_gvd_focal_length_x[ i ] + prec_gvd_focal_length - 31 ))";
                case "man_gvd_focal_length_y":
                    return "(exp_gvd_focal_length_y[ i ] == 0 ? Math.Max( 0, prec_gvd_focal_length - 30 ) : Math.Max( 0, exp_gvd_focal_length_y[ i ] + prec_gvd_focal_length - 31 ))";
                case "man_gvd_principal_point_x":
                    return "(exp_gvd_principal_point_x[ i ] == 0 ? Math.Max( 0, prec_gvd_principal_point - 30 ) : Math.Max( 0, exp_gvd_principal_point_x[ i ] + prec_gvd_principal_point - 31 ))";
                case "man_gvd_principal_point_y":
                    return "(exp_gvd_principal_point_y[ i ] == 0 ? Math.Max( 0, prec_gvd_principal_point - 30 ) : Math.Max( 0, exp_gvd_principal_point_y[ i ] + prec_gvd_principal_point - 31 ))";
                case "mantissa_ref_viewing_distance":
                    return "(exponent_ref_viewing_distance[ i ] == 0 ? Math.Max( 0, prec_ref_viewing_dist - 30 ) : Math.Max( 0, exponent_ref_viewing_distance[ i ] + prec_ref_viewing_dist - 31 ))";
                case "mantissa_ref_display_width":
                    return "(exponent_ref_display_width[ i ] == 0 ? Math.Max( 0, prec_ref_display_width - 30 ) : Math.Max( 0, exponent_ref_display_width[ i ] + prec_ref_display_width - 31 ))";
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
                case "layer_set_idx_for_ols_minus1":
                    return "(uint)Math.Ceiling( Math.Log2( (((H265Context)context).VideoParameterSetRbsp.VpsNumLayerSetsMinus1 + 1 + ((H265Context)context).VideoParameterSetRbsp.VpsExtension.NumAddLayerSets) - 1 ) ) ";
                case "pre_lut_coded_value":
                    return "(( ( colour_remap_input_bit_depth + 7 )  >>  3 )  <<  3)";
                case "pre_lut_target_value":
                    return "(( ( colour_remap_output_bit_depth + 7 )  >>  3 )  <<  3)";
                case "post_lut_coded_value":
                    return "(( ( colour_remap_input_bit_depth + 7 )  >>  3 )  <<  3)";
                case "post_lut_target_value":
                    return "(( ( colour_remap_output_bit_depth + 7 )  >>  3 )  <<  3)";
                case "profile_tier_level_idx":
                    return "(uint)Math.Ceiling( Math.Log2( vps_num_profile_tier_level_minus1 + 1 ) )";
                case "direct_dependency_type":
                    return "(direct_dep_type_len_minus2 + 2)";
                case "overlay_element_label_min":
                case "overlay_element_label_max":
                    return "(overlay_element_label_value_length_minus8 + 8)";
                case "mantissa_r":
                    return "(exponent_r[ i ][ j ][ k ]  == 0 ? Math.Max( 0, prec_rotation_param - 30 ) : Math.Max( 0, exponent_r[ i ][ j ][ k ] + prec_rotation_param - 31 ))";
                case "mantissa_t":
                    return "(exponent_t[ i ][ j ]  == 0 ? Math.Max( 0, prec_translation_param - 30 ) : Math.Max( 0, exponent_t[ i ][ j ] + prec_translation_param - 31 ))";
                case "bsp_hrd_idx":
                    return "(uint)Math.Ceiling( Math.Log2(((H265Context)context).VideoParameterSetRbsp.VpsNumHrdParameters + vps_num_add_hrd_params ) )";
                case "res_coeff_r":
                    return "((uint)Math.Max( 0, ( 10 + (8 + ((H265Context)context).PicParameterSetRbsp.PpsMultilayerExtension.ColourMappingTable.LumaBitDepthCmInputMinus8) - (8 + ((H265Context)context).PicParameterSetRbsp.PpsMultilayerExtension.ColourMappingTable.LumaBitDepthCmOutputMinus8) - ((H265Context)context).PicParameterSetRbsp.PpsMultilayerExtension.ColourMappingTable.CmResQuantBits - ( ((H265Context)context).PicParameterSetRbsp.PpsMultilayerExtension.ColourMappingTable.CmDeltaFlcBitsMinus1 + 1 ) ) ))";
                case "man_gvd_r":
                    return "(exp_gvd_r[ i ][ j ][ k ] == 0 ? Math.Max( 0, prec_gvd_rotation_param - 30 ) : Math.Max( 0, exp_gvd_r[ i ][ j ][ k ] + prec_gvd_rotation_param - 31 ))";
                case "entry_point_offset_minus1":
                    return "(offset_len_minus1 + 1)";
                case "man_gvd_t_x":
                    return "(exp_gvd_t_x[ i ] == 0 ? Math.Max( 0, prec_gvd_translation_param - 30 ) : Math.Max( 0, exp_gvd_t_x[ i ] + prec_gvd_translation_param - 31 ))";
                case "output_slice_segment_address":
                    return "(uint)Math.Ceiling( Math.Log2( H265Helpers.GetPicSizeInCtbsY(context) ) )";
                case "highest_layer_idx_plus1":
                    return "(uint)Math.Ceiling( Math.Log2( H265Helpers.GetNumLayersInTreePartition(context)[ j ] + 1 ) )";
                default:
                    //throw new NotImplementedException(parameter);
                    return parameter;
            }
        }

        public string FixMissingParameters(ItuClass b, string parameter, string classType)
        {
            if (classType == "depth_rep_info_element")
                parameter = "()";

            return parameter;
        }

        public string FixCondition(string condition, MethodType methodType)
        {
            condition = condition.Replace("slice_type == B", "H265FrameTypes.IsB(slice_type)");
            condition = condition.Replace("slice_type != B", "!H265FrameTypes.IsB(slice_type)");
            condition = condition.Replace("slice_type == P", "H265FrameTypes.IsP(slice_type)");
            condition = condition.Replace("slice_type != P", "!H265FrameTypes.IsP(slice_type)");
            condition = condition.Replace("slice_type == I", "H265FrameTypes.IsI(slice_type)");
            condition = condition.Replace("slice_type != I", "!H265FrameTypes.IsI(slice_type)");
            condition = condition.Replace("nal_unit_type != ", "nal_unit_type != H265NALTypes.");
            condition = condition.Replace("nal_unit_type == ", "nal_unit_type == H265NALTypes.");
            condition = condition.Replace("nal_unit_type >= ", "nal_unit_type >= H265NALTypes.");
            condition = condition.Replace("nal_unit_type <= ", "nal_unit_type <= H265NALTypes.");
            condition = condition.Replace("sop_vcl_nut[ i ] != ", "sop_vcl_nut[i] != H265NALTypes.");
            condition = condition.Replace("EXTENDED_ISO", "H265Constants.EXTENDED_ISO");
            condition = condition.Replace("EXTENDED_SAR", "H265Constants.EXTENDED_SAR");
            condition = condition.Replace("Abs(", "(uint)Math.Abs(");
            condition = condition.Replace("Min(", "(uint)Math.Min(");
            condition = condition.Replace("Max(", "(uint)Math.Max(");
            condition = condition.Replace("byte_aligned()", "stream.ByteAligned()");
            condition = condition.Replace("more_data_in_payload()", "stream.MoreDataInPayload()");
            condition = condition.Replace("more_data_in_slice_segment_header_extension()", "stream.MoreDataInSliceSegmentHeaderExtension()");
            condition = condition.Replace("payload_extension_present()", "stream.PayloadExtensionPresent()");
            condition = condition.Replace("more_rbsp_data()", $"stream.{(methodType == MethodType.Read ? "Read" : "Write")}MoreRbspData(this)");
            condition = condition.Replace("next_bits(", $"stream.{(methodType == MethodType.Read ? "Read" : "Write")}NextBits(this, ");
            return condition;
        }

        public string FixStatement(string fieldValue)
        {
            fieldValue = fieldValue.Replace("Abs(", "(uint)Math.Abs(");
            fieldValue = fieldValue.Replace("Min(", "(uint)Math.Min(");
            fieldValue = fieldValue.Replace("Max(", "(uint)Math.Max(");
            fieldValue = fieldValue.Replace("= scaling_list_dc_coef_minus8", "= (uint)scaling_list_dc_coef_minus8");
            return fieldValue;
        }

        public string GetCtorParameterType(string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter))
                return "";

            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                { "scalingLst",                    "u(32)[]" }, // TODO: remove this temporary fix
                { "NumBytesInNalUnit",             "u(32)" },
                { "profilePresentFlag",            "u(32)" },
                { "maxNumSubLayersMinus1",         "u(32)" },
                { "stRpsIdx",                      "u(32)" },
                { "payloadType",                   "u(32)" },
                { "payloadSize",                   "u(32)" },
                { "commonInfPresentFlag",          "u(32)" },
                { "subLayerId",                    "u(32)" },
                { "inpDepth",                      "u(32)" },
                { "idxY",                          "u(32)" },
                { "idxCb",                         "u(32)" },
                { "idxCr",                         "u(32)" },
                { "inpLength",                     "u(32)" },
                { "OutSign",                       "u(32)" },
                { "OutExp",                        "u(32)" },
                { "OutMantissa",                   "u(32)" },
                { "OutManLen",                     "u(32)" },
                { "i",                             "u(32)" },
            };

            return map[parameter];
        }

        public string FixFieldValue(string fieldValue)
        {
            fieldValue = fieldValue.Replace("<<", "<< (int)");
            return fieldValue;
        }

        public void FixMethodAllocation(string name, ref string method, ref string typedef)
        {
            // TODO
        }
    }
}