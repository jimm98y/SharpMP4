using System.Collections.Generic;

namespace ItuGenerator
{
    public class CSharpGeneratorH265 : ICustomGenerator
    {
        public string PreprocessDefinitionsFile(string definitions)
        {
            definitions = definitions
                            .Replace("intrinsic_params_equal_flag ? 0 : num_views_minus1", "(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1)")
                            .Replace("delta_dlt( i )", "delta_dlt()")
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
        }

        public string ReplaceParameter(string parameter)
        {
            switch (parameter)
            {
                case "chroma_format_idc":
                    return "((H265Context)context).SeqParameterSetRbsp.ChromaFormatIdc";
                case "sps_palette_predictor_initializer":
                    return "(((H265Context)context).SeqParameterSetRbsp.BitDepthChromaMinus8 + 8)";
                case "transform_skip_enabled_flag":
                    return "((H265Context)context).PicParameterSetRbsp.TransformSkipEnabledFlag";
                case "pps_palette_predictor_initializer":
                    return "comp == 0 ? (((H265Context)context).PicParameterSetRbsp.PpsSccExtension.LumaBitDepthEntryMinus8 + 8) : (((H265Context)context).PicParameterSetRbsp.PpsSccExtension.ChromaBitDepthEntryMinus8 + 8)";

                default:
                    //throw new NotImplementedException(parameter);
                    return parameter;
            }
        }

        public string FixMissingParameters(ItuClass b, string parameter, string classType)
        {
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
            condition = condition.Replace("EXTENDED_ISO", "H265Constants.EXTENDED_ISO");
            condition = condition.Replace("EXTENDED_SAR", "H265Constants.EXTENDED_SAR");
            condition = condition.Replace("Abs(", "(uint)Math.Abs(");
            condition = condition.Replace("Min(", "(uint)Math.Min(");
            condition = condition.Replace("Max(", "(uint)Math.Max(");
            condition = condition.Replace("byte_aligned()", "stream.ByteAligned()");
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