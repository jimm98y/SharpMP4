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
            // TODO
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
                case "sps_subpic_width_minus1":
                case "sps_subpic_ctu_top_left_x":
                    return "(uint)Math.Ceiling( Math.Log2(  ( sps_pic_width_max_in_luma_samples + ((H266Context)context).CtbSizeY - 1 ) / ((H266Context)context).CtbSizeY ) )";
                case "sps_subpic_height_minus1":
                case "sps_subpic_ctu_top_left_y":
                    return "(uint)Math.Ceiling( Math.Log2(  ( sps_pic_height_max_in_luma_samples + ((H266Context)context).CtbSizeY - 1 ) / ((H266Context)context).CtbSizeY ) )";
                case "sps_subpic_id":
                    return "sps_subpic_id_len_minus1 + 1";

                default:
                    //throw new NotImplementedException(parameter);
                    return parameter;
            }
        }

        public string GetDerivedVariables(string name)
        {
            switch(name)
            {
                case "vps_num_output_layer_sets_minus2":
                    return "((H266Context)context).OnVpsNumOutputLayerSetsMinus2();";
                case "vps_num_dpb_params_minus1":
                    return "((H266Context)context).OnVpsNumDpbParamsMinus1();";
                case "vps_ols_output_layer_flag":
                    return "((H266Context)context).OnVpsOlsOutputLayerFlag(j);";
                case "vps_direct_ref_layer_flag":
                    return "((H266Context)context).OnVpsDirectRefLayerFlag();";
            }
            return "";
        }

        public string FixMissingParameters(ItuClass b, string parameter, string classType)
        {
            if (parameter == null)
                parameter = "()";
            return parameter;
        }

        public string FixCondition(string condition, MethodType methodType)
        {
            condition = condition.Replace("nal_unit_type  !=  ", "nal_unit_type != H266NALTypes.");
            condition = condition.Replace("nal_unit_type  ==  ", "nal_unit_type == H266NALTypes.");
            condition = condition.Replace("nal_unit_type  >=  ", "nal_unit_type >= H266NALTypes.");
            condition = condition.Replace("nal_unit_type  <=  ", "nal_unit_type <= H266NALTypes.");

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
            return fieldValue;
        }

        public void FixMethodAllocation(string name, ref string method, ref string typedef)
        {
            // TODO
        }
    }
}