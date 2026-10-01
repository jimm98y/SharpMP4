using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ItuGenerator.CSharp
{
    /// <summary>
    /// H.261 (ITU-T Rec. H.261 (03/93)), whose picture layer H261.js transcribes from the Recommendation's prose: the
    /// picture's size, of its source format, is kept in the hand-written part of H261Context.
    /// </summary>
    public class CSharpGeneratorH261 : ICustomGenerator
    {
        public string ContextClass => "H261Context";

        public string PreprocessDefinitionsFile(string definitions) => definitions;

        public string FixCondition(string condition, MethodType methodType)
        {
            bool read = methodType == MethodType.Read;

            // PEI after each PSPARE: written, as many as were read
            condition = Regex.Replace(condition, @"\bmore_pei_set\b",
                read ? "(this.more_pei[whileIndex] == 1 ? 1 : 0)" : "(whileIndex + 1 < this.MorePei.Count ? 1 : 0)");
            return condition;
        }

        public string GetDerivedVariables(string field)
        {
            switch (field)
            {
                case "source_format":
                    return "ituContext.OnSourceFormat(source_format);";
                default:
                    return "";
            }
        }

        public string GetVariableSize(string parameter) => throw new NotImplementedException(parameter);
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
