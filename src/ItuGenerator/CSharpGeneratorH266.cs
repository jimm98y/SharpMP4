using System.Collections.Generic;

namespace ItuGenerator
{
    internal class CSharpGeneratorH266 : ICustomGenerator
    {
        public string PreprocessDefinitionsFile(string definitions)
        {
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
                default:
                    //throw new NotImplementedException(parameter);
                    return parameter;
            }
        }

        public string GetDerivedVariables(string name)
        {
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