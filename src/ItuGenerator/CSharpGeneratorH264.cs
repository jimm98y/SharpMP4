using System.Collections.Generic;

namespace ItuGenerator
{
    public class CSharpGeneratorH264 : ICustomGenerator
    {
        public string PreprocessDefinitionsFile(string definitions)
        {
            definitions = definitions
                            .Replace("3dv_acquisition_idc", "three_dv_acquisition_idc")
                            .Replace("3dv_acquisition_element", "three_dv_acquisition_element")
                            .Replace("intrinsic_params_equal_flag ? 0 : num_views_minus1", "(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1)")
                            .Replace(" scalingList", " scalingLst"); // TODO remove this temporary fix
            return definitions;
        }

        public string GetFieldDefaultValue(ItuField field)
        {
            switch (field.Name)
            {
                case "chroma_format_idc":
                    return "1";

                default:
                    return "";
            }
        }

        public void FixClassParameters(ItuClass ituClass)
        {
            if (ituClass.ClassName == "depth_representation_sei_element")
            {
                ituClass.ClassParameter = "()"; // remove all "out" parameters
            }
        }

        public string ReplaceParameter(string parameter)
        {
            switch (parameter)
            {
                case "NumClockTS":
                    return "this.pic_struct switch\r\n{\r\n0u => 1,\r\n1u => 1,\r\n2u => 1,\r\n3u => 2,\r\n4u => 2,\r\n5u => 3,\r\n6u => 3,\r\n7u => 2,\r\n8u => 3,\r\n_ => throw new NotSupportedException()\r\n}";
                case "AllViewsPairedFlag":
                    return "((Func<uint>)(() =>\r\n            {\r\n                uint AllViewsPairedFlag = 1;\r\n                for (int i = 1; i <= ((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetMvcExtension.NumViewsMinus1; i++)\r\n                    AllViewsPairedFlag = (uint)((AllViewsPairedFlag != 0 && ((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetMvcdExtension.DepthViewPresentFlag[i] != 0 && ((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetMvcdExtension.TextureViewPresentFlag[i] != 0) ? 1 : 0);\r\n                return AllViewsPairedFlag;\r\n            })).Invoke()";
                case "cpb_cnt_minus1":
                    return "((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.HrdParameters.CpbCntMinus1";
                case "ChromaArrayType":
                    return "(((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.SeparateColourPlaneFlag == 0 ? ((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.ChromaFormatIdc : 0)";
                case "IdrPicFlag":
                    return "(uint)((((H264Context)context).NalHeader.NalUnitType == 5) ? 1 : 0)";
                case "NalHrdBpPresentFlag":
                    return "((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.NalHrdParametersPresentFlag";
                case "VclHrdBpPresentFlag":
                    return "((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.VclHrdParametersPresentFlag";
                case "profile_idc":
                    return "((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.ProfileIdc";
                case "chroma_format_idc":
                    return "((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.ChromaFormatIdc";
                case "CpbDpbDelaysPresentFlag":
                    return "(uint)(((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.NalHrdParametersPresentFlag == 1 || ((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.VclHrdParametersPresentFlag == 1 ? 1 : 0)";
                case "pic_struct_present_flag":
                    return "((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.PicStructPresentFlag";
                case "NumDepthViews":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetMvcdExtension.NumDepthViews";
                case "PicSizeInMapUnits":
                    return "(((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.PicWidthInMbsMinus1 + 1) * (((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.PicHeightInMapUnitsMinus1 + 1)";
                case "frame_mbs_only_flag":
                    return "((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.FrameMbsOnlyFlag";
                case "num_slice_groups_minus1":
                    return "((H264Context)context).PicParameterSetRbsp.NumSliceGroupsMinus1";
                case "num_views_minus1":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetMvcExtension.NumViewsMinus1";
                case "anchor_pic_flag":
                    return "((H264Context)context).NalHeader.NalUnitHeaderMvcExtension.AnchorPicFlag";
                case "ref_dps_id0":
                    return "((H264Context)context).DepthParameterSetRbsp.RefDpsId0";
                case "predWeight0":
                    return "((H264Context)context).DepthParameterSetRbsp.PredWeight0";
                case "deltaFlag":
                    return "0";
                case "ref_dps_id1":
                    return "((H264Context)context).DepthParameterSetRbsp.RefDpsId1";
                case "num_anchor_refs_l0":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetMvcExtension.NumAnchorRefsL0";
                case "num_anchor_refs_l1":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetMvcExtension.NumAnchorRefsL1";
                case "num_non_anchor_refs_l0":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetMvcExtension.NumNonAnchorRefsL0";
                case "num_non_anchor_refs_l1":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetMvcExtension.NumNonAnchorRefsL1";
                case "num_init_pic_parameter_set_minus1":
                    return "((H264Context)context).SeiPayload.MvcdViewScalabilityInfo.NumPicParameterSetMinus1"; // looks like there is a typo...
                case "additional_shift_present":
                    return "((H264Context)context).SeiPayload.ThreeDimensionalReferenceDisplaysInfo.AdditionalShiftPresentFlag.Select(x => (uint)x).ToArray()"; // TODO: looks like a typo
                case "texture_view_present_flag":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetMvcdExtension.TextureViewPresentFlag.Select(x => (uint)x).ToArray()";
                case "initial_cpb_removal_delay":
                case "initial_cpb_removal_delay_offset":
                    return "((((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters != null && ((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.HrdParameters != null ? ((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.HrdParameters.InitialCpbRemovalDelayLengthMinus1 : 23) + 1)";
                case "alpha_opaque_value":
                case "alpha_transparent_value":
                    return "(this.bit_depth_aux_minus8 + 9)";
                case "slice_group_id":
                    return "(uint)Math.Ceiling(Math.Log2(((H264Context)context).PicParameterSetRbsp.NumSliceGroupsMinus1 + 1))";
                case "cpb_removal_delay":
                    return "((((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters != null && ((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.HrdParameters != null ? ((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.HrdParameters.CpbRemovalDelayLengthMinus1 : 23) + 1)";
                case "dpb_output_delay":
                    return "((((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters != null && ((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.HrdParameters != null ? ((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.HrdParameters.DpbOutputDelayLengthMinus1 : 23) + 1)";
                case "time_offset":
                case "time_offset_length":
                    return "(((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters != null && ((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.HrdParameters != null ? ((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.HrdParameters.TimeOffsetLength : 24)";
                case "start_of_coded_interval":
                case "coded_pivot_value":
                case "target_pivot_value":
                    return "(((((H264Context)context).SeiPayload.ToneMappingInfo.CodedDataBitDepth + 7) >> 3) << 3)";
                case "pre_lut_coded_value":
                case "pre_lut_target_value":
                    return "(((((H264Context)context).SeiPayload.ColourRemappingInfo.ColourRemapInputBitDepth + 7) >> 3) << 3)";
                case "post_lut_coded_value":
                case "post_lut_target_value":
                    return "(((((H264Context)context).SeiPayload.ColourRemappingInfo.ColourRemapOutputBitDepth + 7) >> 3) << 3)";
                case "ar_object_confidence":
                    return "(((H264Context)context).SeiPayload.AnnotatedRegions.ArObjectConfidenceLengthMinus1 + 1)";
                case "da_mantissa":
                    return "(this.da_mantissa_len_minus1 + 1)";
                case "exponent0":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSet3davcExtension.DepthRanges.ThreeDvAcquisitionElement.ExpLen";
                case "mantissa0":
                    return "(((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSet3davcExtension.DepthRanges.ThreeDvAcquisitionElement.MantissaLenMinus1[i] + 1)";
                case "exponent1":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSet3davcExtension.DepthRanges.ThreeDvAcquisitionElement.ExpLen";
                case "mantissa_focal_length_x":
                    return "(exponent_focal_length_x[ i ] == 0) ? (Math.Max( 0, prec_focal_length - 30 )) : (Math.Max( 0, exponent_focal_length_x[ i ] + prec_focal_length - 31))";
                case "mantissa_focal_length_y":
                    return "(exponent_focal_length_y[ i ] == 0) ? (Math.Max( 0, prec_focal_length - 30 )) : (Math.Max( 0, exponent_focal_length_y[ i ] + prec_focal_length - 31))";
                case "mantissa_principal_point_x":
                    return "(exponent_principal_point_x[ i ] == 0) ? (Math.Max( 0, prec_principal_point - 30 )) : (Math.Max( 0, exponent_principal_point_x[ i ] + prec_principal_point - 31))";
                case "mantissa_principal_point_y":
                    return "(exponent_principal_point_y[ i ] == 0) ? (Math.Max( 0, prec_principal_point - 30 )) : (Math.Max( 0, exponent_principal_point_y[ i ] + prec_principal_point - 31))";
                case "mantissa_skew_factor":
                    return "(exponent_skew_factor[ i ] == 0) ? (Math.Max( 0, prec_skew_factor - 30 )) : (Math.Max( 0, exponent_skew_factor[ i ] + prec_skew_factor - 31))";
                case "mantissa_r":
                    return "(exponent_r[ i ][ j ][ k ]  == 0) ? (Math.Max( 0, prec_rotation_param - 30 )) : (Math.Max( 0, exponent_r[ i ][ j ][ k ] + prec_rotation_param - 31))";
                case "mantissa_t":
                    return "(exponent_t[ i ][ j ] == 0) ? (Math.Max( 0, prec_translation_param - 30 )) : (Math.Max( 0, exponent_t[ i ][ j ] + prec_translation_param - 31))";
                case "mantissa_ref_baseline":
                    return "(exponent_ref_baseline[ i ] == 0) ? (Math.Max( 0, prec_ref_baseline - 30 )) : (Math.Max( 0, exponent_ref_baseline[ i ] + prec_ref_baseline - 31))";
                case "mantissa_ref_display_width":
                    return "(exponent_ref_display_width[ i ] == 0) ? (Math.Max( 0, prec_ref_display_width - 30 )) : (Math.Max( 0, exponent_ref_display_width[ i ] + prec_ref_display_width - 31))";
                case "mantissa_ref_viewing_distance":
                    return "(exponent_ref_viewing_distance[ i ] == 0) ? (Math.Max( 0, prec_ref_viewing_dist - 30 )) : (Math.Max( 0, exponent_ref_viewing_distance[ i ] + prec_ref_viewing_dist - 31))";
                case "man_gvd_z_near":
                    return "(man_len_gvd_z_near_minus1[ i ] + 1)";
                case "man_gvd_z_far":
                    return "(man_len_gvd_z_far_minus1[ i ] + 1)";
                case "man_gvd_focal_length_x":
                    return "(exp_gvd_focal_length_x[ i ] == 0) ? (Math.Max( 0, prec_gvd_focal_length - 30 )) : (Math.Max( 0, exp_gvd_focal_length_x[ i ] + prec_gvd_focal_length - 31))";
                case "man_gvd_focal_length_y":
                    return "(exp_gvd_focal_length_y[ i ] == 0) ? (Math.Max( 0, prec_gvd_focal_length - 30 )) : (Math.Max( 0, exp_gvd_focal_length_y[ i ] + prec_gvd_focal_length - 31))";
                case "man_gvd_principal_point_x":
                    return "(exp_gvd_principal_point_x[ i ] == 0) ? (Math.Max( 0, prec_gvd_principal_point - 30 )) : (Math.Max( 0, exp_gvd_principal_point_x[ i ] + prec_gvd_principal_point - 31))";
                case "man_gvd_principal_point_y":
                    return "(exp_gvd_principal_point_y[ i ] == 0) ? (Math.Max( 0, prec_gvd_principal_point - 30 )) : (Math.Max( 0, exp_gvd_principal_point_y[ i ] + prec_gvd_principal_point - 31))";
                case "man_gvd_r":
                    return "(exp_gvd_r[ i ][ j ][ k ] == 0) ? (Math.Max( 0, prec_gvd_rotation_param - 30 )) : (Math.Max( 0,  exp_gvd_r[ i ][ j ][ k ] + prec_gvd_rotation_param - 31))";
                case "man_gvd_t_x":
                    return "(exp_gvd_t_x[ i ] == 0) ? (Math.Max( 0, prec_gvd_translation_param - 30 )) : (Math.Max( 0,  exp_gvd_t_x[ i ] + prec_gvd_translation_param - 31))";
                case "separate_colour_plane_flag":
                    return "((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.SeparateColourPlaneFlag";
                case "redundant_pic_cnt_present_flag":
                    return "((H264Context)context).PicParameterSetRbsp.RedundantPicCntPresentFlag";
                case "entropy_coding_mode_flag":
                    return "((H264Context)context).PicParameterSetRbsp.EntropyCodingModeFlag";
                case "svc_extension_flag":
                    return "((H264Context)context).NalHeader.SvcExtensionFlag";
                case "avc_3d_extension_flag":
                    return "((H264Context)context).NalHeader.Avc3dExtensionFlag";
                case "frame_num":
                    return "(((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.Log2MaxFrameNumMinus4 + 4)";
                case "pic_order_cnt_type":
                    return "((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.PicOrderCntType";
                case "pic_order_cnt_lsb":
                    return "(((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.Log2MaxPicOrderCntLsbMinus4 + 4)";
                case "bottom_field_pic_order_in_frame_present_flag":
                    return "((H264Context)context).PicParameterSetRbsp.BottomFieldPicOrderInFramePresentFlag";
                case "delta_pic_order_always_zero_flag":
                    return "((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.DeltaPicOrderAlwaysZeroFlag";
                case "nal_unit_type":
                    return "((H264Context)context).NalHeader.NalUnitType";
                case "weighted_pred_flag":
                    return "((H264Context)context).PicParameterSetRbsp.WeightedPredFlag";
                case "weighted_bipred_idc":
                    return "((H264Context)context).PicParameterSetRbsp.WeightedBipredIdc";
                case "nal_ref_idc":
                    return "((H264Context)context).NalHeader.NalRefIdc";
                case "deblocking_filter_control_present_flag":
                    return "((H264Context)context).PicParameterSetRbsp.DeblockingFilterControlPresentFlag";
                case "slice_group_map_type":
                    return "((H264Context)context).PicParameterSetRbsp.SliceGroupMapType";
                case "slice_group_change_cycle":
                    return "((H264Context)context).SliceLayerWithoutPartitioningRbsp.SliceHeader.SliceGroupChangeCycle";
                case "slice_type":
                    return "((H264Context)context).SliceLayerWithoutPartitioningRbsp.SliceHeader.SliceType";
                case "num_ref_idx_l0_active_minus1":
                    return "(((H264Context)context).SliceLayerWithoutPartitioningRbsp.SliceHeader.NumRefIdxActiveOverrideFlag != 0 ? ((H264Context)context).SliceLayerWithoutPartitioningRbsp.SliceHeader.NumRefIdxL0ActiveMinus1 : ((H264Context)context).PicParameterSetRbsp.NumRefIdxL0DefaultActiveMinus1)";
                case "num_ref_idx_l1_active_minus1":
                    return "((H264Context)context).SliceLayerWithoutPartitioningRbsp.SliceHeader.NumRefIdxL1ActiveMinus1";
                case "use_ref_base_pic_flag":
                    return "((H264Context)context).NalHeader.NalUnitHeaderSvcExtension.UseRefBasePicFlag";
                case "idr_flag":
                    return "((H264Context)context).NalHeader.NalUnitHeaderSvcExtension.IdrFlag";
                case "quality_id":
                    return "((H264Context)context).NalHeader.NalUnitHeaderSvcExtension.QualityId";
                case "no_inter_layer_pred_flag":
                    return "((H264Context)context).NalHeader.NalUnitHeaderSvcExtension.NoInterLayerPredFlag";
                case "slice_header_restriction_flag":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetSvcExtension.SliceHeaderRestrictionFlag";
                case "inter_layer_deblocking_filter_control_present_flag":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetSvcExtension.InterLayerDeblockingFilterControlPresentFlag";
                case "extended_spatial_scalability_idc":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetSvcExtension.ExtendedSpatialScalabilityIdc";
                case "adaptive_tcoeff_level_prediction_flag":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetSvcExtension.AdaptiveTcoeffLevelPredictionFlag";
                case "slice_header_prediction_flag":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSet3davcExtension.SliceHeaderPredictionFlag";
                case "seq_view_synthesis_flag":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSet3davcExtension.SeqViewSynthesisFlag";
                case "three_dv_acquisition_idc":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSet3davcExtension.ThreeDvAcquisitionIdc";
                case "DepthFlag":
                    return "( ((H264Context)context).NalHeader.NalUnitType  !=  21 ) ? false : ( ((H264Context)context).NalHeader.Avc3dExtensionFlag != 0 ? ((H264Context)context).NalHeader.NalUnitHeader3davcExtension.DepthFlag : 1 )";
                case "depth_disp_delay_offset_fp":
                    return "(this.offset_len_minus1 + 1)";

                default:
                    //throw new NotImplementedException(parameter);
                    return parameter;
            }
        }

        public string FixMissingParameters(ItuClass b, string parameter, string classType)
        {
            if (parameter.Contains("NumDepthViews")) parameter = parameter.Replace("NumDepthViews", ReplaceParameter("NumDepthViews"));
            parameter = parameter.Replace("ScalingList4x4[ i ]", "new uint[6 * 16]");
            parameter = parameter.Replace("UseDefaultScalingMatrix4x4Flag[ i ]", "0");
            parameter = parameter.Replace("ScalingList8x8[ i - 6 ]", "new uint[6 * 64]");
            parameter = parameter.Replace("UseDefaultScalingMatrix8x8Flag[ i - 6 ]", $"0");
            parameter = parameter.Replace("ZNearSign, ZNearExp, ZNearMantissa, ZNearManLen", "new uint[index, numViews], new uint[index, numViews], new uint[index, numViews], new uint[index, numViews]");
            parameter = parameter.Replace("ZFarSign, ZFarExp, ZFarMantissa, ZFarManLen", "new uint[index, numViews], new uint[index, numViews], new uint[index, numViews], new uint[index, numViews]");

            if (classType == "depth_representation_sei_element")
                parameter = "()";

            return parameter;
        }

        public string FixCondition(string condition, MethodType methodType)
        {
            condition = condition.Replace("slice_type == B", "H264FrameTypes.IsB(slice_type)");
            condition = condition.Replace("slice_type == P", "H264FrameTypes.IsP(slice_type)");
            condition = condition.Replace("slice_type == I", "H264FrameTypes.IsI(slice_type)");
            condition = condition.Replace("slice_type != I", "!H264FrameTypes.IsI(slice_type)");
            condition = condition.Replace("slice_type == SP", "H264FrameTypes.IsSP(slice_type)");
            condition = condition.Replace("slice_type == SI", "H264FrameTypes.IsSI(slice_type)");
            condition = condition.Replace("slice_type != SI", "!H264FrameTypes.IsSI(slice_type)");
            condition = condition.Replace("slice_type == EP", "H264FrameTypes.IsP(slice_type)");
            condition = condition.Replace("slice_type == EB", "H264FrameTypes.IsB(slice_type)");
            condition = condition.Replace("slice_type == EI", "H264FrameTypes.IsI(slice_type)");
            condition = condition.Replace("slice_type != EI", "!H264FrameTypes.IsI(slice_type)");
            condition = condition.Replace("Extended_ISO", "H264Constants.Extended_ISO");
            condition = condition.Replace("Extended_SAR", "H264Constants.Extended_SAR");
            condition = condition.Replace("Abs(", "Math.Abs(");
            condition = condition.Replace("Min(", "Math.Min(");
            condition = condition.Replace("Max(", "Math.Max(");
            condition = condition.Replace("byte_aligned()", "stream.ByteAligned()");
            condition = condition.Replace("more_rbsp_data()", $"stream.{(methodType == MethodType.Read ? "Read" : "Write")}MoreRbspData(this)");
            condition = condition.Replace("next_bits(", $"stream.{(methodType == MethodType.Read ? "Read" : "Write")}NextBits(this, ");
            condition = condition.Replace("more_rbsp_trailing_data()", "stream.MoreRbspTrailingData()");
            return condition;
        }

        public string FixStatement(string fieldValue)
        {
            fieldValue = fieldValue.Replace("Abs(", "Math.Abs(");
            fieldValue = fieldValue.Replace("Min(", "Math.Min(");
            fieldValue = fieldValue.Replace("Max(", "Math.Max(");
            return fieldValue;
        }

        public string GetCtorParameterType(string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter))
                return "";

            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                { "NumBytesInNALunit",             "u(32)" },
                { "scalingLst",                    "u(32)[]" }, // TODO: remove this temporary fix
                { "sizeOfScalingList",             "u(32)" },
                { "useDefaultScalingMatrixFlag",   "u(32)" },
                { "payloadType",                   "u(32)" },
                { "payloadSize",                   "u(32)" },
                { "outSign",                       "u(32)[,]" },
                { "outExp",                        "u(32)[,]" },
                { "outMantissa",                   "u(32)[,]" },
                { "outManLen",                     "u(32)[,]" },
                { "numViews",                      "u(32)" },
                { "predDirection",                 "u(32)" },
                { "index",                         "u(32)" },
                { "expLen",                        "u(32)" },
            };

            return map[parameter];
        }

        public string FixFieldValue(string fieldValue)
        {
            fieldValue = fieldValue.Replace("<<", "<< (int)");
            fieldValue = fieldValue.Replace("mantissaPred + mantissa_diff", "(uint)(mantissaPred + mantissa_diff)");
            return fieldValue;
        }

        public void FixMethodAllocation(string name, ref string method, ref string typedef)
        {
            if (name == "depth_timing_offset" && string.IsNullOrWhiteSpace(typedef))
            {
                method = $"this.depth_timing_offset = new DepthTimingOffset[1];\r\n" + method;
                typedef = "[ 0 ]";
            }
            else if (name == "depth_grid_position" && string.IsNullOrWhiteSpace(typedef))
            {
                method = $"this.depth_grid_position = new DepthGridPosition[1];\r\n" + method;
                typedef = "[ 0 ]";
            }
        }

        public string GetDerivedVariables(string field)
        {
            switch(field)
            {
                case "sei_payload":
                    return "((H264Context)context).SetSeiPayload(sei_payload);";
            }
            return "";
        }
    }
}
