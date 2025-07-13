using System;
using System.Collections.Generic;

namespace AomGenerator.CSharp
{
    public class CSharpGeneratorAV1 : ICustomGenerator
    {
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
            condition = condition.Replace("Min(", "Math.Min(");
            condition = condition.Replace("Max(", "Math.Max(");

            condition = condition.Replace("INTRA_FRAME", "AV1RefFrames.INTRA_FRAME");
            condition = condition.Replace(" NONE ", " AV1RefFrames.NONE ");
            condition = condition.Replace("LAST_FRAME", "AV1RefFrames.LAST_FRAME");
            condition = condition.Replace("LAST2_FRAME", "AV1RefFrames.LAST2_FRAME");
            condition = condition.Replace("LAST3_FRAME", "AV1RefFrames.LAST3_FRAME");
            condition = condition.Replace("GOLDEN_FRAME", "AV1RefFrames.GOLDEN_FRAME");
            condition = condition.Replace("BWDREF_FRAME", "AV1RefFrames.BWDREF_FRAME");
            condition = condition.Replace("ALTREF2_FRAME", "AV1RefFrames.ALTREF2_FRAME");
            condition = condition.Replace("ALTREF_FRAME", "AV1RefFrames.ALTREF_FRAME");
            condition = condition.Replace("OBU_SEQUENCE_HEADER", "AV1ObuTypes.OBU_SEQUENCE_HEADER");
            condition = condition.Replace("OBU_TEMPORAL_DELIMITER", "AV1ObuTypes.OBU_TEMPORAL_DELIMITER");
            //condition = condition.Replace("OBU_FRAME_HEADER", "AV1ObuTypes.OBU_FRAME_HEADER");
            condition = condition.Replace("OBU_TILE_GROUP", "AV1ObuTypes.OBU_TILE_GROUP");
            condition = condition.Replace("OBU_METADATA", "AV1ObuTypes.OBU_METADATA");
            condition = condition.Replace("OBU_FRAME", "AV1ObuTypes.OBU_FRAME");
            condition = condition.Replace("OBU_REDUNDANT_FRAME_HEADER", "AV1ObuTypes.OBU_REDUNDANT_FRAME_HEADER");
            condition = condition.Replace("OBU_TILE_LIST", "AV1ObuTypes.OBU_TILE_LIST");
            condition = condition.Replace("OBU_PADDING", "AV1ObuTypes.OBU_PADDING");


            return condition;
        }

        public string FixFieldValue(string fieldValue)
        {
            return fieldValue;
        }

        public void FixNestedIndexes(List<string> ret, AomField field)
        {
            // nothing to do
        }

        public string FixStatement(string fieldValue)
        {
            fieldValue = fieldValue.Replace("Min(", "Math.Min(");
            fieldValue = fieldValue.Replace("Max(", "Math.Max(");

            fieldValue = fieldValue.Replace(" NONE ", " AV1RefFrames.NONE ");
            fieldValue = fieldValue.Replace("INTRA_FRAME", "AV1RefFrames.INTRA_FRAME");
            fieldValue = fieldValue.Replace("LAST_FRAME", "AV1RefFrames.LAST_FRAME");
            fieldValue = fieldValue.Replace("LAST2_FRAME", "AV1RefFrames.LAST2_FRAME");
            fieldValue = fieldValue.Replace("LAST3_FRAME", "AV1RefFrames.LAST3_FRAME");
            fieldValue = fieldValue.Replace("GOLDEN_FRAME", "AV1RefFrames.GOLDEN_FRAME");
            fieldValue = fieldValue.Replace("BWDREF_FRAME", "AV1RefFrames.BWDREF_FRAME");
            fieldValue = fieldValue.Replace("ALTREF2_FRAME", "AV1RefFrames.ALTREF2_FRAME");
            fieldValue = fieldValue.Replace("ALTREF_FRAME", "AV1RefFrames.ALTREF_FRAME");
            fieldValue = fieldValue.Replace("OBU_SEQUENCE_HEADER", "AV1ObuTypes.OBU_SEQUENCE_HEADER");
            fieldValue = fieldValue.Replace("OBU_TEMPORAL_DELIMITER", "AV1ObuTypes.OBU_TEMPORAL_DELIMITER");
            //fieldValue = fieldValue.Replace("OBU_FRAME_HEADER", "AV1ObuTypes.OBU_FRAME_HEADER");
            fieldValue = fieldValue.Replace("OBU_TILE_GROUP", "AV1ObuTypes.OBU_TILE_GROUP");
            fieldValue = fieldValue.Replace("OBU_METADATA", "AV1ObuTypes.OBU_METADATA");
            fieldValue = fieldValue.Replace("OBU_FRAME", "AV1ObuTypes.OBU_FRAME");
            fieldValue = fieldValue.Replace("OBU_REDUNDANT_FRAME_HEADER", "AV1ObuTypes.OBU_REDUNDANT_FRAME_HEADER");
            fieldValue = fieldValue.Replace("OBU_TILE_LIST", "AV1ObuTypes.OBU_TILE_LIST");
            fieldValue = fieldValue.Replace("OBU_PADDING", "AV1ObuTypes.OBU_PADDING");
            
            fieldValue = fieldValue.Replace("get_position", "stream.GetPosition");

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

        public string GetFieldDefaultValue(AomField field)
        {
            switch (field.Name)
            {
                default:
                    return "";
            }
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
