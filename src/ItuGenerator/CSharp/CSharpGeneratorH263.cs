using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ItuGenerator.CSharp
{
    /// <summary>
    /// H.263 (ITU-T Rec. H.263 (01/2005)), whose picture layer H263.js transcribes from the Recommendation's prose: what the
    /// picture header reads of the modes in use - the custom picture clock frequency, the Reference Picture Selection and
    /// scalability modes, which a picture keeps from those before it (5.1.4.5) or are negotiated outside the stream - is in
    /// the hand-written part of H263Context.
    /// </summary>
    public class CSharpGeneratorH263 : ICustomGenerator
    {
        public string ContextClass => "H263Context";

        public string PreprocessDefinitionsFile(string definitions) => definitions;

        public string FixCondition(string condition, MethodType methodType)
        {
            bool read = methodType == MethodType.Read;

            // the modes in use, as the pictures before say, or as negotiated
            condition = Regex.Replace(condition, @"\bcustom_pcf_in_use\b", "(ituContext.CustomPcfInUse ? 1 : 0)");
            condition = Regex.Replace(condition, @"\brps_in_use\b", "(ituContext.RpsInUse ? 1 : 0)");
            condition = Regex.Replace(condition, @"\bscalability_in_use\b", "(ituContext.ScalabilityInUse ? 1 : 0)");

            // a PB-frame (Annex G) by PTYPE, or an Improved PB-frame (Annex M) by PLUSPTYPE: TRB and DBQUANT follow
            condition = Regex.Replace(condition, @"\bpb_frame\b",
                "((source_format != 7 && pb_frames_mode == 1) || (source_format == 7 && picture_type_code == 2) ? 1 : 0)");

            // PEI after each PSUPP: written, as many as were read
            condition = Regex.Replace(condition, @"\bmore_pei_set\b",
                read ? "(this.more_pei[whileIndex] == 1 ? 1 : 0)" : "(whileIndex + 1 < this.MorePei.Count ? 1 : 0)");
            return condition;
        }

        public string GetVariableSize(string parameter)
        {
            switch (parameter)
            {
                // 5.1.22: 3 bits, or 5 with a custom picture clock frequency
                case "trb":
                    return "(ituContext.CustomPcfInUse ? 5u : 3u)";
                default:
                    throw new NotImplementedException(parameter);
            }
        }

        public string GetDerivedVariables(string field)
        {
            switch (field)
            {
                case "source_format":
                    return "ituContext.OnSourceFormat(source_format);";
                case "opptype_reserved_bits":
                    return "ituContext.OnOpptype(this);";
                case "picture_type_code":
                    return "ituContext.OnPictureTypeCode(picture_type_code);";
                case "picture_coding_type":
                    return "ituContext.OnPictureCodingType(picture_coding_type);";
                case "picture_height_indication":
                    return "ituContext.OnCustomPictureFormat(picture_width_indication, picture_height_indication);";
                default:
                    return "";
            }
        }

        public string ReplaceParameter(string parameter) => parameter;
        public string GetCtorParameterType(string parameter) => string.IsNullOrWhiteSpace(parameter) ? "" : "u(32)";
        public string GetParameterType(string parameter) => string.IsNullOrWhiteSpace(parameter) ? "" : "u(32)";
        public string AppendMethod(ItuCode field, MethodType methodType, string spacing, string retm) => retm;
        public string GetFieldDefaultValue(ItuField field) => "";
        public void FixClassParameters(ItuClass ituClass) { }
        public string FixMissingParameters(ItuClass b, string parameter, string classType) => parameter;
        public string FixStatement(string fieldValue) => fieldValue;
        public string FixFieldValue(string fieldValue) => fieldValue;
        public void FixMethodAllocation(string name, ref string method, ref string typedef) { }
        public void FixNestedIndexes(List<string> ret, ItuField field) { }
        public string GetLocalArrayInitializer(string name) => null;
        public string FixAllocations(string spacing, string appendType, string variableType, string variableName) => "";
        public string FixVariableType(string variableType) => variableType;
        public string FixAppendType(string appendType, string variableName) => appendType;
    }
}
