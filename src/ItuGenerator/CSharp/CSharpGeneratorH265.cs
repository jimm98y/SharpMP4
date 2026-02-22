using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ItuGenerator.CSharp
{
    public class CSharpGeneratorH265 : ICustomGenerator
    {        public string AppendMethod(ItuCode field, MethodType methodType, string spacing, string retm)
        {
            string name = (field as ItuField).Name;

            if (name == "sei_payload" || name == "vui_payload")
            {
                retm = $"{spacing}stream.MarkCurrentBitsPosition();\r\n{retm}";
            }
            else if (name == "slice_segment_header_extension_length")
            {
                retm = $"{retm}\r\n{spacing}stream.MarkCurrentBitsPosition();";
            }

            return retm;
        }

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
                case "collocated_from_l0_flag":
                    return "1";

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
                    return "ituContext.MaxSubLayersInLayerSetMinus1";
                case "chroma_format_idc":
                    return "ituContext.SeqParameterSetRbsp.ChromaFormatIdc";
                case "transform_skip_enabled_flag":
                    return "ituContext.PicParameterSetRbsp.TransformSkipEnabledFlag";
                case "cabac_init_present_flag":
                    return "ituContext.PicParameterSetRbsp.CabacInitPresentFlag";
                case "dependent_slice_segments_enabled_flag":
                    return "ituContext.PicParameterSetRbsp.DependentSliceSegmentsEnabledFlag";
                case "chroma_qp_offset_list_enabled_flag":
                    return "ituContext.PicParameterSetRbsp.PpsRangeExtension != null && ituContext.PicParameterSetRbsp.PpsRangeExtension.ChromaQpOffsetListEnabledFlag";
                case "num_extra_slice_header_bits":
                    return "ituContext.PicParameterSetRbsp.NumExtraSliceHeaderBits";
                case "MaxLayersMinus1":
                    return "Math.Min( 62, ituContext.VideoParameterSetRbsp.VpsMaxLayersMinus1)";
                case "NumLayerSets":
                    return "(ituContext.VideoParameterSetRbsp.VpsNumLayerSetsMinus1 + 1 + ituContext.VideoParameterSetRbsp.VpsExtension.NumAddLayerSets)";
                case "num_ref_idx_l0_active_minus1":
                    return "ituContext.SliceSegmentLayerRbsp.SliceSegmentHeader.NumRefIdxL0ActiveMinus1";
                case "num_cp":
                    return "ituContext.VideoParameterSetRbsp.Vps3dExtension.NumCp";
                case "slice_type":
                    return "ituContext.SliceSegmentLayerRbsp.SliceSegmentHeader.SliceType";
                case "nuh_layer_id":
                    return "ituContext.NalHeader.NalUnitHeader.NuhLayerId";
                case "nal_unit_type":
                    return "ituContext.NalHeader.NalUnitHeader.NalUnitType";
                case "vps_base_layer_internal_flag":
                    return "ituContext.VideoParameterSetRbsp.VpsBaseLayerInternalFlag";
                case "vps_num_hrd_parameters":
                    return "(uint)ituContext.VideoParameterSetRbsp.VpsNumHrdParameters";
                case "layer_id_in_nuh":
                    return "ituContext.VideoParameterSetRbsp.VpsExtension.LayerIdInNuh";
                case "sub_pic_hrd_params_present_flag":
                    return "ituContext.SeqParameterSetRbsp.VuiParameters.HrdParameters.SubPicHrdParamsPresentFlag";
                case "frame_field_info_present_flag":
                    return "ituContext.SeqParameterSetRbsp.VuiParameters.FrameFieldInfoPresentFlag";
                case "num_ref_idx_l1_active_minus1":
                    return "ituContext.SliceSegmentLayerRbsp.SliceSegmentHeader.NumRefIdxL1ActiveMinus1";
                case "num_short_term_ref_pic_sets":
                    return "ituContext.SeqParameterSetRbsp.NumShortTermRefPicSets";
                case "num_long_term_ref_pics_sps":
                    return "ituContext.SeqParameterSetRbsp.NumLongTermRefPicsSps";
                case "sub_pic_cpb_params_in_pic_timing_sei_flag":
                    return "ituContext.SeqParameterSetRbsp.VuiParameters.HrdParameters.SubPicCpbParamsInPicTimingSeiFlag";
                case "NumPicTotalCurr":
                    return "ituContext.NumPicTotalCurr";
                case "NumOutputLayersInOutputLayerSet":
                    return "ituContext.NumOutputLayersInOutputLayerSet";
                case "inter_layer_pred_layer_idc":
                    return " (uint)Math.Ceiling( MathEx.Log2( ituContext.NumDirectRefLayers[ ituContext.NalHeader.NalUnitHeader.NuhLayerId ] ) ) ";
                case "ChromaArrayType":
                    return "(ituContext.SeqParameterSetRbsp.SeparateColourPlaneFlag == 0 ? ituContext.SeqParameterSetRbsp.ChromaFormatIdc : 0)";
                case "sei_ols_idx":
                    return "ituContext.SeiPayload.BspNesting.SeiOlsIdx";
                case "sei_partitioning_scheme_idx":
                    return "ituContext.SeiPayload.BspNesting.SeiPartitioningSchemeIdx";
                case "cm_octant_depth":
                    return "ituContext.PicParameterSetRbsp.PpsMultilayerExtension.ColourMappingTable.CmOctantDepth";
                case "lists_modification_present_flag":
                    return "ituContext.PicParameterSetRbsp.ListsModificationPresentFlag";
                case "CpbCnt":
                    return "ituContext.CpbCnt";
                case "cp_ref_voi":
                    return "(uint)ituContext.VideoParameterSetRbsp.Vps3dExtension.CpRefVoi";
                case "cp_in_slice_segment_header_flag":
                    return "ituContext.VideoParameterSetRbsp.Vps3dExtension != null && ituContext.VideoParameterSetRbsp.Vps3dExtension.CpInSliceSegmentHeaderFlag";
                case "deblocking_filter_override_enabled_flag":
                    return "ituContext.PicParameterSetRbsp.DeblockingFilterOverrideEnabledFlag";
                case "pps_slice_chroma_qp_offsets_present_flag":
                    return "ituContext.PicParameterSetRbsp.PpsSliceChromaQpOffsetsPresentFlag";
                case "default_ref_layers_active_flag":
                    return "ituContext.VideoParameterSetRbsp.VpsExtension.DefaultRefLayersActiveFlag";
                case "max_one_active_ref_layer_flag":
                    return "ituContext.VideoParameterSetRbsp.VpsExtension.MaxOneActiveRefLayerFlag";
                case "poc_lsb_not_present_flag":
                    return "ituContext.VideoParameterSetRbsp.VpsExtension.PocLsbNotPresentFlag";
                case "sample_adaptive_offset_enabled_flag":
                    return "ituContext.SeqParameterSetRbsp.SampleAdaptiveOffsetEnabledFlag";
                case "NumOutputLayerSets":
                    return "ituContext.VideoParameterSetRbsp.VpsExtension.NumOutputLayerSets";
                case "vps_poc_lsb_aligned_flag":
                    return "ituContext.VideoParameterSetRbsp.VpsExtension.VpsPocLsbAlignedFlag";
                case "vps_num_layer_sets_minus1":
                    return "ituContext.VideoParameterSetRbsp.VpsNumLayerSetsMinus1";
                case "vps_max_sub_layers_minus1":
                    return "ituContext.VideoParameterSetRbsp.VpsMaxSubLayersMinus1";
                case "vps_max_layers_minus1":
                    return "ituContext.VideoParameterSetRbsp.VpsMaxLayersMinus1";
                case "slice_segment_header_extension_present_flag":
                    return "ituContext.PicParameterSetRbsp.SliceSegmentHeaderExtensionPresentFlag";
                case "tiles_enabled_flag":
                    return "ituContext.PicParameterSetRbsp.TilesEnabledFlag";
                case "weighted_pred_flag":
                    return "ituContext.PicParameterSetRbsp.WeightedPredFlag";
                case "weighted_bipred_flag":
                    return "ituContext.PicParameterSetRbsp.WeightedBipredFlag";
                case "pps_loop_filter_across_slices_enabled_flag":
                    return "ituContext.PicParameterSetRbsp.PpsLoopFilterAcrossSlicesEnabledFlag";
                case "entropy_coding_sync_enabled_flag":
                    return "ituContext.PicParameterSetRbsp.EntropyCodingSyncEnabledFlag";
                case "poc_reset_info_present_flag":
                    return "ituContext.PicParameterSetRbsp.PpsMultilayerExtension.PocResetInfoPresentFlag";
                case "output_flag_present_flag":
                    return "ituContext.PicParameterSetRbsp.OutputFlagPresentFlag";
                case "sps_temporal_mvp_enabled_flag":
                    return "ituContext.SeqParameterSetRbsp.SpsTemporalMvpEnabledFlag";
                case "sps_max_sub_layers_minus1":
                    return "ituContext.SeqParameterSetRbsp.SpsMaxSubLayersMinus1";
                case "separate_colour_plane_flag":
                    return "ituContext.SeqParameterSetRbsp.SeparateColourPlaneFlag";
                case "long_term_ref_pics_present_flag":
                    return "ituContext.SeqParameterSetRbsp.LongTermRefPicsPresentFlag";
                case "depthMaxValue":
                    return "(( 1  <<  ( (int)ituContext.PicParameterSetRbsp.Pps3dExtension.PpsBitDepthForDepthLayersMinus8 + 8 ) ) - 1)";
                case "NumViews":
                    return "ituContext.NumViews";
                case "NumRefListLayers":
                    return "ituContext.NumRefListLayers";
                case "NumDirectRefLayers":
                    return "ituContext.NumDirectRefLayers";
                case "NumLayersInIdList":
                    return "ituContext.NumLayersInIdList";
                case "NumIndependentLayers":
                    return "ituContext.NumIndependentLayers";
                case "NumActiveRefLayerPics":
                    return "ituContext.NumActiveRefLayerPics";
                case "NumDeltaPocs":
                    return "ituContext.NumDeltaPocs";
                case "MaxTemporalId":
                    return "ituContext.MaxTemporalId";
                case "OlsIdxToLsIdx":
                    return "ituContext.OlsIdxToLsIdx";
                case "PicOrderCnt":
                    return "ituContext.PicOrderCnt";
                case "pic_layer_id":
                    return "ituContext.PicLayerId";
                case "RefPicList0":
                    return "ituContext.RefPicList0";
                case "RefPicList1":
                    return "ituContext.RefPicList1";
                case "BspSchedCnt":
                    return "ituContext.BspSchedCnt";
                case "NecessaryLayerFlag":
                    return "ituContext.NecessaryLayerFlag";
                case "IdDirectRefLayer":
                    return "ituContext.IdDirectRefLayer";
                case "vclInitialArrivalDelayPresent":
                    return "ituContext.VclInitialArrivalDelayPresent";
                case "LayerSetLayerIdList":
                    return "ituContext.LayerSetLayerIdList";
                case "LayerIdxInVps":
                    return "ituContext.LayerIdxInVps";
                case "OlsHighestOutputLayerId":
                    return "ituContext.OlsHighestOutputLayerId";
                case "ViewOIdxList":
                    return "ituContext.ViewOIdxList";
                case "inCmpPredAvailFlag":
                    return "ituContext.inCmpPredAvailFlag";
                case "nalInitialArrivalDelayPresent":
                    return "ituContext.NalInitialArrivalDelayPresent";
                case "ViewIdx":
                    return "ituContext.ViewOrderIdx[ ituContext.NalHeader.NalUnitHeader.NuhLayerId ]";
                case "DepthFlag":
                    return "ituContext.DepthLayerFlag[ ituContext.NalHeader.NalUnitHeader.NuhLayerId ]";
                case "defaultOutputLayerIdc":
                    return "Math.Min( ituContext.VideoParameterSetRbsp.VpsExtension.DefaultOutputLayerIdc, 2 )";
                case "PartNumY":
                    return "(1u  <<  (int)ituContext.PicParameterSetRbsp.PpsMultilayerExtension.ColourMappingTable.CmyPartNumLog2)";
                case "numViewsMinus1":
                    return "(ituContext.SeiPayload.MultiviewAcquisitionInfo != null ? ituContext.SeiPayload.ScalableNesting.NestingNumLayersMinus1 : 0)";
                case "NalHrdBpPresentFlag":
                    return "ituContext.SeqParameterSetRbsp.VuiParameters.HrdParameters.NalHrdParametersPresentFlag";
                case "VclHrdBpPresentFlag":
                    return "ituContext.SeqParameterSetRbsp.VuiParameters.HrdParameters.VclHrdParametersPresentFlag";
                case "CpbDpbDelaysPresentFlag":
                    return "(ituContext.SeqParameterSetRbsp.VuiParameters.HrdParameters.NalHrdParametersPresentFlag == 1 || ituContext.SeqParameterSetRbsp.VuiParameters.HrdParameters.VclHrdParametersPresentFlag == 1 ? 1 : 0)";
                case "PocMsbValRequiredFlag":
                    return "((ituContext.NalHeader.NalUnitHeader.NalUnitType == H265NALTypes.BLA_W_LP || ituContext.NalHeader.NalUnitHeader.NalUnitType == H265NALTypes.BLA_N_LP || ituContext.NalHeader.NalUnitHeader.NalUnitType == H265NALTypes.BLA_W_RADL || ituContext.NalHeader.NalUnitHeader.NalUnitType == H265NALTypes.CRA_NUT)  &&  ( ituContext.VideoParameterSetRbsp.VpsExtension.VpsPocLsbAlignedFlag == 0  || ( ituContext.VideoParameterSetRbsp.VpsExtension.VpsPocLsbAlignedFlag != 0  && ituContext.NumDirectRefLayers[ ituContext.NalHeader.NalUnitHeader.NuhLayerId ]  ==  0 ) ) ? 1 : 0)";
                case "CurrPic":
                    return "ituContext.CurrPic";
                case "RefRpsIdx":
                    return "ituContext.RefRpsIdx";
                default:
                    //throw new NotImplementedException(parameter);
                    return parameter;
            }
        }

        public string GetDerivedVariables(string field)
        {
            switch(field)
            {
                case "sei_payload":
                    return "ituContext.SetSeiPayload(sei_payload);";
                case "pps_pic_parameter_set_id":
                    return "ituContext.SetPpsPicParameterSetId(pps_pic_parameter_set_id);";
                case "slice_pic_parameter_set_id":
                    return "ituContext.SetSlicePicParameterSetId(slice_pic_parameter_set_id);";
                case "sps_seq_parameter_set_id":
                    return "ituContext.SetSpsSeqParameterSetId(sps_seq_parameter_set_id);";
                case "pps_seq_parameter_set_id":
                    return "ituContext.SetPpsSeqParameterSetId(pps_seq_parameter_set_id);";

                case "vps_max_layers_minus1":
                    return "ituContext.OnVpsMaxLayersMinus1();";
                case "num_add_layer_sets":
                    return "ituContext.OnNumAddLayerSets();\r\n }\r\n else \r\n {\r\n ituContext.OnNumAddLayerSets(); \r\n";
                case "highest_layer_idx_plus1":
                    return "ituContext.OnHighestLayerIdxPlus1(i);";
                case "direct_dependency_flag":
                    return "ituContext.OnDirectDependencyFlag();";
                case "dimension_id":
                    return "ituContext.OnDimensionId();";
                case "cp_ref_voi":
                    return "ituContext.OnCpRefVoi();";
                case "cpb_cnt_minus1":
                    return "ituContext.OnCpbCntMinus1(i);";
                case "sub_layers_vps_max_minus1":
                    return "ituContext.OnSubLayersVpsMaxMinus1();";
                case "log2_diff_max_min_luma_coding_block_size":
                    return "ituContext.OnLog2DiffMaxMinLumaCodingBlockSize();";
                case "layer_set_idx_for_ols_minus1":
                    return "ituContext.OnLayerSetIdxForOlsMinus1(i, NumOutputLayerSets);\r\n\t\t\t\t}\r\n\t\t\t\telse\r\n\t\t\t\t{\r\n\t\t\t\t\tituContext.OnLayerSetIdxForOlsMinus1(i, NumOutputLayerSets);";
                case "output_layer_flag":
                    return "ituContext.OnOutputLayerFlag(i, j);";
                case "layer_id_in_nuh":
                    return "ituContext.OnLayerIdInNuh(i);";
                case "nal_hrd_parameters_present_flag":
                    return "ituContext.OnNalHrdParametersPresentFlag(nal_hrd_parameters_present_flag);";
                case "vcl_hrd_parameters_present_flag":
                    return "ituContext.OnVclHrdParametersPresentFlag(vcl_hrd_parameters_present_flag);";
                case "direct_dependency_type":
                    return "ituContext.OnDirectDependencyType();";
                case "num_bsp_schedules_minus1":
                    return "ituContext.OnNumBspSchedulesMinus1(h, i, t);";
                case "used_by_curr_pic_s0_flag":
                    return "ituContext.OnUsedByCurrPicS0Flag(i, stRpsIdx, this);";
                case "used_by_curr_pic_s1_flag":
                    return "ituContext.OnUsedByCurrPicS1Flag(i, stRpsIdx, this);";
                case "delta_idx_minus1":
                    return "ituContext.OnDeltaIdxMinus1(stRpsIdx);";
                case "nesting_max_temporal_id_plus1":
                    return "ituContext.OnNestingMaxTemporalIdPlus1(i);";
                case "num_inter_layer_ref_pics_minus1":
                    return "ituContext.OnNumInterLayerRefPicsMinus1();";
                case "nuh_temporal_id_plus1":
                    return "ituContext.OnNuhTemporalIdPlus1();";
                case "list_entry_l0":
                    return "ituContext.OnListEntryL0();";
                case "used_by_curr_pic_lt_flag":
                    return "ituContext.OnUsedByCurrPicLtFlag(i);";
                case "short_term_ref_pic_set_idx":
                    return "ituContext.OnShortTermRefPicSetIdx();";
                case "inter_layer_pred_layer_idc":
                    return "ituContext.OnInterLayerPredLayerIdc();";
                case "slice_type":
                    return "ituContext.OnSliceType();";
                case "layer_id_included_flag":
                    return "ituContext.OnLayerIDIncludedFlag(i, j);";
            }

            return "";
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
            condition = condition.Replace("more_data_in_payload()", "(!(stream.ByteAligned() && 8 * payloadSize == stream.GetBitsPositionSinceLastMark()))");
            condition = condition.Replace("payload_extension_present()", $"stream.{(methodType == MethodType.Read ? "Read" : "Write")}MoreRbspData(this, payloadSize)");
            condition = condition.Replace("more_rbsp_data()", $"stream.{(methodType == MethodType.Read ? "Read" : "Write")}MoreRbspData(this)");
            condition = condition.Replace("more_data_in_slice_segment_header_extension()", $"stream.{(methodType == MethodType.Read ? "Read" : "Write")}MoreRbspData(this, slice_segment_header_extension_length)");
            condition = condition.Replace("next_bits(", $"stream.{(methodType == MethodType.Read ? "Read" : "Write")}NextBits(this, ");

            condition = condition.Replace("i = MaxNumSubLayersMinus1 - 1", "i = (int)MaxNumSubLayersMinus1 - 1");
            condition = condition.Replace("i = maxNumSubLayersMinus1;", "i = (int)maxNumSubLayersMinus1;");
            condition = condition.Replace("sh_slice_type == ", "sh_slice_type == H266FrameTypes.");
            condition = condition.Replace("i < maxNumSubLayersMinus1", "i < (int)maxNumSubLayersMinus1");
            condition = condition.Replace("i = lmcs_min_bin_idx", "i = (uint)lmcs_min_bin_idx");
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
                { "maxNumSubLayersMinus1",         "u(64)" },
                { "stRpsIdx",                      "u(64)" },
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
                { "st_ref_pic_flag",        null },
            };

            if (map.ContainsKey(parameter))
                return map[parameter];
            else
                return "u(32)";
        }

        public string FixFieldValue(string fieldValue)
        {
            fieldValue = fieldValue.Replace("<<", "<< (int)");
            fieldValue = fieldValue.Replace("cp_ref_voi[ i ][ m ]", "(uint)cp_ref_voi[ i ][ m ]");
            return fieldValue;
        }

        public void FixMethodAllocation(string name, ref string method, ref string typedef)
        {
            // TODO
        }

        public string GetVariableSize(string parameter)
        {
            switch (parameter)
            {
                case "sps_palette_predictor_initializer":
                    return "(ituContext.SeqParameterSetRbsp.BitDepthChromaMinus8 + 8)";
                case "direct_dependency_type":
                    return "(direct_dep_type_len_minus2 + 2)";
                case "overlay_element_label_min":
                case "overlay_element_label_max":
                    return "(overlay_element_label_value_length_minus8 + 8)";
                case "mantissa_r":
                    return "(exponent_r[ i ][ j ][ k ]  == 0 ? Math.Max( 0, prec_rotation_param - 30 ) : Math.Max( 0, exponent_r[ i ][ j ][ k ] + prec_rotation_param - 31 ))";
                case "mantissa_t":
                    return "(exponent_t[ i ][ j ]  == 0 ? Math.Max( 0, prec_translation_param - 30 ) : Math.Max( 0, exponent_t[ i ][ j ] + prec_translation_param - 31 ))";
                case "pps_palette_predictor_initializer":
                    return "comp == 0 ? (ituContext.PicParameterSetRbsp.PpsSccExtension.LumaBitDepthEntryMinus8 + 8) : (ituContext.PicParameterSetRbsp.PpsSccExtension.ChromaBitDepthEntryMinus8 + 8)";
                case "nal_initial_arrival_delay":
                    return "(ituContext.VideoParameterSetRbsp.HrdParameters[i].InitialCpbRemovalDelayLengthMinus1 + 1)";
                case "nal_initial_cpb_removal_delay":
                case "nal_initial_cpb_removal_offset":
                case "nal_initial_alt_cpb_removal_delay":
                case "nal_initial_alt_cpb_removal_offset":
                case "vcl_initial_cpb_removal_delay":
                case "vcl_initial_cpb_removal_offset":
                case "vcl_initial_alt_cpb_removal_delay":
                case "vcl_initial_alt_cpb_removal_offset":
                case "vcl_initial_arrival_delay":
                    return "(ituContext.SeqParameterSetRbsp.VuiParameters.HrdParameters.InitialCpbRemovalDelayLengthMinus1 + 1)";
                case "du_cpb_removal_delay_increment_minus1":
                    return "(ituContext.SeqParameterSetRbsp.VuiParameters.HrdParameters.DuCpbRemovalDelayIncrementLengthMinus1 + 1)";
                case "start_of_coded_interval":
                case "coded_pivot_value":
                    return "( ( coded_data_bit_depth + 7 )  >>  3 )  <<  3";
                case "target_pivot_value":
                    return "( ( target_bit_depth + 7 )  >>  3 )  <<  3";
                case "dimension_id":
                    return "(dimension_id_len_minus1[j] + 1)";
                case "reserved_payload_extension_data":
                    return "0 /* TODO */"; // TODO: specification shall ignore this, but when present it's 8 * payloadSize − nEarlierBits − nPayloadZeroBits − 1
                case "cpb_delay_offset":
                    return "ituContext.SeqParameterSetRbsp.VuiParameters.HrdParameters.AuCpbRemovalDelayLengthMinus1 + 1";
                case "dpb_delay_offset":
                    return "ituContext.SeqParameterSetRbsp.VuiParameters.HrdParameters.DpbOutputDelayLengthMinus1 + 1";
                case "au_cpb_removal_delay_delta_minus1":
                    return "ituContext.SeqParameterSetRbsp.VuiParameters.HrdParameters.AuCpbRemovalDelayLengthMinus1 + 1";
                case "au_cpb_removal_delay_minus1":
                    return "(ituContext.SeqParameterSetRbsp.VuiParameters.HrdParameters.AuCpbRemovalDelayLengthMinus1 + 1)";
                case "pic_dpb_output_delay":
                    return "ituContext.SeqParameterSetRbsp.VuiParameters.HrdParameters.DpbOutputDelayLengthMinus1 + 1";
                case "pic_dpb_output_du_delay":
                    return "ituContext.SeqParameterSetRbsp.VuiParameters.HrdParameters.DpbOutputDelayDuLengthMinus1 + 1";
                case "du_common_cpb_removal_delay_increment_minus1":
                case "du_spt_cpb_removal_delay_increment":
                    return "ituContext.SeqParameterSetRbsp.VuiParameters.HrdParameters.DuCpbRemovalDelayIncrementLengthMinus1 + 1";
                case "pic_spt_dpb_output_du_delay":
                    return "ituContext.SeqParameterSetRbsp.VuiParameters.HrdParameters.DpbOutputDelayDuLengthMinus1 + 1";
                case "time_offset_value":
                    return "time_offset_length[i]";
                case "pre_lut_coded_value":
                    return "(( ( colour_remap_input_bit_depth + 7 )  >>  3 )  <<  3)";
                case "pre_lut_target_value":
                    return "(( ( colour_remap_output_bit_depth + 7 )  >>  3 )  <<  3)";
                case "post_lut_coded_value":
                    return "(( ( colour_remap_input_bit_depth + 7 )  >>  3 )  <<  3)";
                case "post_lut_target_value":
                    return "(( ( colour_remap_output_bit_depth + 7 )  >>  3 )  <<  3)";
                case "output_slice_segment_address":
                    return "(uint)Math.Ceiling( MathEx.Log2( ituContext.PicSizeInCtbsY ) )";
                case "view_id_val":
                    return "view_id_len";
                case "highest_layer_idx_plus1":
                    return "(uint)Math.Ceiling( MathEx.Log2( ituContext.NumLayersInTreePartition[ j ] + 1 ) )";
                case "layer_set_idx_for_ols_minus1":
                    return "(uint)Math.Ceiling( MathEx.Log2( (ituContext.VideoParameterSetRbsp.VpsNumLayerSetsMinus1 + 1 + ituContext.VideoParameterSetRbsp.VpsExtension.NumAddLayerSets) - 1 ) ) ";
                case "profile_tier_level_idx":
                    return "(uint)Math.Ceiling( MathEx.Log2( vps_num_profile_tier_level_minus1 + 1 ) )";
                case "vps_rep_format_idx":
                    return "(uint)Math.Ceiling( MathEx.Log2( vps_num_rep_formats_minus1 + 1 ) )";
                case "direct_dependency_all_layers_type":
                    return "direct_dep_type_len_minus2 + 2";
                case "bsp_hrd_idx":
                    return "(uint)Math.Ceiling( MathEx.Log2(ituContext.VideoParameterSetRbsp.VpsNumHrdParameters + vps_num_add_hrd_params ) )";
                case "lt_ref_pic_poc_lsb_sps":
                    return "(ituContext.SeqParameterSetRbsp.Log2MaxPicOrderCntLsbMinus4 + 4)";
                case "res_coeff_r":
                    return "((uint)Math.Max( 0, ( 10 + (8 + ituContext.PicParameterSetRbsp.PpsMultilayerExtension.ColourMappingTable.LumaBitDepthCmInputMinus8) - (8 + ituContext.PicParameterSetRbsp.PpsMultilayerExtension.ColourMappingTable.LumaBitDepthCmOutputMinus8) - ituContext.PicParameterSetRbsp.PpsMultilayerExtension.ColourMappingTable.CmResQuantBits - ( ituContext.PicParameterSetRbsp.PpsMultilayerExtension.ColourMappingTable.CmDeltaFlcBitsMinus1 + 1 ) ) ))";
                case "man_gvd_r":
                    return "(exp_gvd_r[ i ][ j ][ k ] == 0 ? Math.Max( 0, prec_gvd_rotation_param - 30 ) : Math.Max( 0, exp_gvd_r[ i ][ j ][ k ] + prec_gvd_rotation_param - 31 ))";
                case "entry_point_offset_minus1":
                    return "(offset_len_minus1 + 1)";
                case "man_gvd_t_x":
                    return "(exp_gvd_t_x[ i ] == 0 ? Math.Max( 0, prec_gvd_translation_param - 30 ) : Math.Max( 0, exp_gvd_t_x[ i ] + prec_gvd_translation_param - 31 ))";
                case "alpha_transparent_value":
                case "alpha_opaque_value":
                    return "alpha_channel_bit_depth_minus8 + 9";
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
                case "mantissa_ref_viewing_distance":
                    return "(exponent_ref_viewing_distance[ i ] == 0 ? Math.Max( 0, prec_ref_viewing_dist - 30 ) : Math.Max( 0, exponent_ref_viewing_distance[ i ] + prec_ref_viewing_dist - 31 ))";
                case "da_mantissa":
                    return "(this.da_mantissa_len_minus1 + 1)";
                case "num_val_delta_dlt":
                case "max_diff":
                    return "ituContext.PicParameterSetRbsp.Pps3dExtension.PpsBitDepthForDepthLayersMinus8 + 8";
                case "min_diff_minus1":
                    return "(uint)Math.Ceiling( MathEx.Log2( max_diff + 1 ) )";
                case "delta_dlt_val0":
                    return "ituContext.PicParameterSetRbsp.Pps3dExtension.PpsBitDepthForDepthLayersMinus8 + 8";
                case "delta_val_diff_minus_min":
                    return "(uint)Math.Ceiling( MathEx.Log2( max_diff - min_diff_minus1 + 1 ) )";
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
                case "slice_segment_address":
                    return "(uint)Math.Ceiling( MathEx.Log2( ituContext.PicSizeInCtbsY ) )";
                case "slice_pic_order_cnt_lsb":
                    return "ituContext.SeqParameterSetRbsp.Log2MaxPicOrderCntLsbMinus4 + 4";
                case "short_term_ref_pic_set_idx":
                    return "(uint)Math.Ceiling( MathEx.Log2( ituContext.SeqParameterSetRbsp.NumShortTermRefPicSets ) )";
                case "lt_idx_sps":
                    return "(uint)Math.Ceiling( MathEx.Log2( ituContext.SeqParameterSetRbsp.NumLongTermRefPicsSps ) )";
                case "poc_lsb_lt":
                    return "( ituContext.SeqParameterSetRbsp.Log2MaxPicOrderCntLsbMinus4 + 4)";
                case "list_entry_l0":
                    return "(uint)Math.Ceiling( MathEx.Log2( ituContext.NumPicTotalCurr ) ) ";
                case "list_entry_l1":
                    return "(uint)Math.Ceiling( MathEx.Log2( ituContext.NumPicTotalCurr ) ) ";
                case "inter_layer_pred_layer_idc":
                case "num_inter_layer_ref_pics_minus1":
                    return "(uint)Math.Ceiling( MathEx.Log2( ituContext.NumDirectRefLayers[ ituContext.NalHeader.NalUnitHeader.NuhLayerId ] ) )";
                case "poc_lsb_val":
                    return "ituContext.SeqParameterSetRbsp.Log2MaxPicOrderCntLsbMinus4 + 4";
            }

            Debug.WriteLine(parameter);
            throw new NotImplementedException();
        }

        public string FixAllocations(string spacing, string appendType, string variableType, string variableName)
        {
            if(variableName == "layer_id_included_flag[ i ]")
            {
                return $"\r\n{spacing}this.{variableName} = new {variableType.Replace("vps_max_layer_id", "vps_max_layer_id + 1")}{appendType};";
            }
            else if(variableName == "dimension_id")
            {
                return $"\r\n{spacing}this.{variableName} = new {variableType}{appendType};\r\n{spacing}this.{variableName}[0] = new ulong[ NumScalabilityTypes];";
            }
            else if(variableName == "direct_dependency_flag")
            {
                return $"\r\n{spacing}this.{variableName} = new {variableType}{appendType};\r\n{spacing}this.{variableName}[0] = new byte[ituContext.VideoParameterSetRbsp.VpsMaxLayersMinus1 + 1];";
            }
            else if(variableName == "direct_dependency_flag[ i ]")
            {
                return $"\r\n{spacing}this.{variableName} = new {variableType.Replace(" i", "ituContext.VideoParameterSetRbsp.VpsMaxLayersMinus1 + 1")}{appendType};";
            }

            return "";
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
            if (variableName == "cp_ref_voi" ||
                variableName == "vps_cp_scale[ n ]" ||
                variableName == "vps_cp_off[ n ]" ||
                variableName == "vps_cp_inv_scale_plus_scale[ n ]" ||
                variableName == "vps_cp_inv_off_plus_off[ n ]" ||
                variableName == "cp_scale" ||
                variableName == "cp_off" ||
                variableName == "cp_scale" ||
                variableName == "cp_inv_scale_plus_scale" ||
                variableName == "cp_inv_off_plus_off")
            {
                appendType += "[]"; // TODO fix this workaround
            }
            else if (
                variableName == "vps_cp_scale" ||
                variableName == "vps_cp_off" ||
                variableName == "vps_cp_inv_scale_plus_scale" ||
                variableName == "vps_cp_inv_off_plus_off"
                )
            {
                appendType += "[][]"; // TODO fix this workaround
            }

            return appendType;
        }

        public void FixNestedIndexes(List<string> ret, ItuField field)
        {
            if (field != null && (
                            field.Name == "coded_res_flag" ||
                            field.Name == "res_coeff_q" ||
                            field.Name == "res_coeff_r" ||
                            field.Name == "res_coeff_s"
                            ))
            {
                ret.Remove(ret[0]);
                ret.Insert(0, "[ idxCr ]");
                ret.Insert(0, "[ idxCb ]");
                ret.Insert(0, "[ idxShiftY ]");
            }

            if (field != null && (
                field.Name == "num_cp" ||
                field.Name == "cp_in_slice_segment_header_flag" ||
                field.Name == "cp_ref_voi" ||
                field.Name == "vps_cp_scale" ||
                field.Name == "vps_cp_off" ||
                field.Name == "vps_cp_inv_scale_plus_scale" ||
                field.Name == "vps_cp_inv_off_plus_off"
                ))
            {
                ret.Remove(ret[0]);
            }

            if (field != null && 
                field.Name == "slice_reserved_flag"
                )
            {
                ret.Remove(ret[0]);
            }
        }

        public string ContextClass => "H265Context";
    }
}
