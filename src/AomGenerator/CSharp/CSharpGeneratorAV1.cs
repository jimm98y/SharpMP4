using System;
using System.Collections.Generic;
using System.Text;

namespace AomGenerator.CSharp
{
    public class CSharpGeneratorAV1 : ICustomGenerator
    {
        public string AppendMethod(AomCode field, MethodType methodType, string spacing, string retm)
        {
            return retm;
        }

        public string FixAllocations(string spacing, string appendType, string variableType, string variableName)
        {
            return "";
        }

        public string FixAppendType(string appendType, string variableName)
        {
            return appendType;
        }

        public void FixClassParameters(AomClass ituClass)
        {
            
        }

        public string FixCondition(string condition, MethodType methodType)
        {
            return condition;
        }

        public string FixFieldValue(string fieldValue)
        {
            return fieldValue;
        }

        public void FixMethodAllocation(string name, ref string method, ref string typedef)
        {
            
        }

        public string FixMissingParameters(AomClass b, string parameter, string classType)
        {
            if (parameter == null) return "";
            return parameter;
        }

        public void FixNestedIndexes(List<string> ret, AomField field)
        {
            // nothing to do
        }

        public string FixStatement(string fieldValue)
        {
            return fieldValue;
        }

        public string FixVariableType(string variableType)
        {
            return variableType;
        }

        public string GetCtorParameterType(string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter))
                return "";

            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                { "sz", "f(32)" },
                { "nbBits", "f(32)" },
                { "op", "f(32)" },
                { "a", "f(32)" },
                { "b", "f(32)" },
                { "idLen", "f(32)" },
                { "blkSize", "f(32)" },
                { "target", "f(32)" },
                { "type", "f(32)" },
                { "refc", "f(32)" },
                { "idx", "f(32)" },
                { "low", "f(32)" },
                { "high", "f(32)" },
                { "r", "f(32)" },
                { "mx", "f(32)" },
                { "numSyms", "f(32)" },
                { "v", "f(32)" },
                { "c", "f(32)" },
                { "sbSize4", "f(32)" },
                { "bSize", "f(32)" },
                { "subSize", "f(32)" },
                { "bw4", "f(32)" },
                { "bh4", "f(32)" },
                { "diff", "f(32)" },
                { "max", "f(32)" },
                { "feature", "f(32)" },
                { "allowSelect", "f(32)" },
                { "row", "f(32)" },
                { "col", "f(32)" },
                { "txSz", "f(32)" },
                { "depth", "f(32)" },
                { "preSkip", "f(32)" },
                { "isCompound", "f(32)" },
                { "refFrame", "f(32)" },
                { "refList", "f(32)" },
                { "comp", "f(32)" },
                { "plane", "f(32)" },
                { "baseX", "f(32)" },
                { "baseY", "f(32)" },
                { "x", "f(32)" },
                { "y", "f(32)" },
                { "startX", "f(32)" },
                { "startY", "f(32)" },
                { "w", "f(32)" },
                { "h", "f(32)" },
                { "subsize", "f(32)" },
                { "blockX", "f(32)" },
                { "blockY", "f(32)" },
                { "txSet", "f(32)" },
                { "txType", "f(32)" },
                { "mode", "f(32)" },
                { "x4", "f(32)" },
                { "y4", "f(32)" },
                { "colorMap", "f(32)" },
                { "n", "f(32)" },
                { "candidateR", "f(32)" },
                { "candidateC", "f(32)" },
                { "mvec", "f(32)" },
                { "border", "f(32)" },
                { "unitRow", "f(32)" },
                { "unitCol", "f(32)" },
                { "k", "f(32)" },
            };

            return map[parameter];
        }

        public string GetDerivedVariables(string field)
        {
            switch (field)
            {
                default:
                    return "";
            }
        }

        public string GetFieldDefaultValue(AomField field)
        {
            switch (field.Name)
            {
                default:
                    return "";
            }
        }

        public string GetParameterType(string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter))
                return "";

            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                { "operating_point_idc", "f(32)[]" },
                { "seq_tier", "f(32)[]" },
                { "decoder_model_present_for_this_op", "f(32)[]" },
                { "initial_display_delay_present_for_this_op", "f(32)[]" },
            };

            if (map.ContainsKey(parameter))
                return map[parameter];
            else
                return "f(32)";
        }

        public string GetVariableSize(string parameter)
        {
            throw new NotImplementedException();
        }

        public string PreprocessDefinitionsFile(string definitions)
        {
            // rename ref -> refc to avoid C# conflicts with built-in ref keyword
            definitions = definitions.Replace(" ref ", " refc ");
            definitions = definitions.Replace("[ref]", "[refc]");
            definitions = definitions.Replace(", ref,", ", refc,");
            definitions = definitions.Replace("ref++", "refc++");
            return definitions;
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
    }
}
