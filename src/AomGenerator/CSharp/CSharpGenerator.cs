using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace AomGenerator.CSharp
{
    public enum MethodType
    {
        Read,
        Write
    }

    public interface ICustomGenerator
    {
        string FixVariableType(string variableType);
        string FixAppendType(string appendType, string variableName);
        string FixAllocations(string spacing, string appendType, string variableType, string variableName);
        string PreprocessDefinitionsFile(string definitions);
        string GetFieldDefaultValue(AomField field);
        void FixClassParameters(AomClass ituClass);
        string ReplaceParameter(string parameter);
        string FixCondition(string condition, MethodType methodType);
        string FixStatement(string fieldValue, MethodType methodType);
        string GetCtorParameterType(string parameter);
        string FixFieldValue(string fieldValue);
        void FixNestedIndexes(List<string> ret, AomField field);
    }

    public class CSharpGenerator
    {
        private ICustomGenerator specificGenerator = null;

        private HashSet<string> _fields = new HashSet<string>();

        public CSharpGenerator(ICustomGenerator specificGenerator)
        {
            this.specificGenerator = specificGenerator ?? throw new ArgumentNullException(nameof(specificGenerator));
        }

        public string GenerateParser(string type, IEnumerable<AomClass> aomClasses)
        {
            string resultCode =
            $@"using System;
using System.Collections.Generic;
using System.Numerics;

namespace Sharp{type}
{{
";
            resultCode += @$"
    public partial class {type}Context : IAomContext
    {{
        AomStream stream = null;
        int obu_padding_length = 0;
        private void ReadDropObu() {{ }}
        private void WriteDropObu() {{ }}
        private void ReadSetFrameRefs() {{ }}
        private void WriteSetFrameRefs() {{ }}
        private void ReadResetGrainParams() {{ }}
        private void WriteResetGrainParams() {{ }}
        private void ReadLoadGrainParams(uint p) {{ }}
        private void WriteLoadGrainParams(uint p) {{ }}
        private int ChooseOperatingPoint() {{ throw new NotImplementedException(); }}
";

            foreach (var ituClass in aomClasses)
            {
                resultCode += GenerateMethods(ituClass);
            }

            resultCode += @$"
        }}
    }}
";
            return resultCode;
        }

        private string GenerateMethods(AomClass aomClass)
        {
            specificGenerator.FixClassParameters(aomClass);

            string resultCode = @$"
    /*
{aomClass.Syntax.Replace("*/", "*//*")}
    */
";
            resultCode += GenerateFields(aomClass);


            string[] ctorParameters = GetMethodParameters(aomClass);
            var typeMappings = GetCSharpTypeMapping();
            string[] ctorParameterDefs = ctorParameters.Select(x => $"{(typeMappings.ContainsKey(specificGenerator.GetCtorParameterType(x)) ? typeMappings[specificGenerator.GetCtorParameterType(x)] : "")} {x}").ToArray();
            string ituClassParameters = $"{string.Join(", ", ctorParameterDefs)}";

            resultCode += BuildRequiredVariables(aomClass);

            resultCode += $@"
         public void Read{aomClass.ClassName.ToPropertyCase()}({ituClassParameters})
         {{
";
            foreach (var field in aomClass.Fields)
            {
                resultCode += "\r\n" + BuildMethod(aomClass, null, field, 3, MethodType.Read);
            }
            aomClass.AddedFields.Clear();
            resultCode += $@"
         }}
";
            resultCode += $@"
         public void Write{aomClass.ClassName.ToPropertyCase()}({ituClassParameters})
         {{
";
            foreach (var field in aomClass.Fields)
            {
                resultCode += "\r\n" + BuildMethod(aomClass, null, field, 3, MethodType.Write);
            }
            aomClass.AddedFields.Clear();
            resultCode += $@"
         }}
";

            return resultCode;
        }

        private string[] GetMethodParameters(AomClass aomClass)
        {
            var parameters = aomClass.ClassParameter.Substring(1, aomClass.ClassParameter.Length - 2).Split(',').Select(x => x.Trim()).ToArray();
            if (parameters.Length == 1 && string.IsNullOrEmpty(parameters[0]))
            {
                return new string[] { };
            }
            return parameters;
        }

        private string GenerateFields(AomClass aomClass)
        {
            string resultCode = "";
            aomClass.FlattenedFields = FlattenFields(aomClass, aomClass.Fields);

            foreach (var field in aomClass.FlattenedFields)
            {
                resultCode += BuildField(aomClass, field);
            }

            return resultCode;
        }

        private List<AomField> FlattenFields(AomClass b, IEnumerable<AomCode> fields, AomBlock parent = null)
        {
            Dictionary<string, AomField> ret = new Dictionary<string, AomField>();

            if (parent == null)
            {
                // add also ctor params as fields
                string[] ctorParams = GetMethodParameters(b);
                for (int i = 0; i < ctorParams.Length; i++)
                {
                    var f = new AomField()
                    {
                        Name = ctorParams[i].ToFirstLower(),
                        Type = specificGenerator.GetCtorParameterType(ctorParams[i])
                    };
                    b.AddedFields.Add(f);
                    AddAndResolveDuplicates(b, ret, f);
                }
            }

            foreach (var code in fields)
            {
                if (code is AomField field)
                {
                    field.Parent = parent; // keep track of parent blocks

                    var p = parent;
                    while (p != null)
                    {
                        if (p.Type == "while")
                        {
                            field.MakeList = true;
                            Debug.WriteLine($"Field {field.Name} is a list");
                        }
                        p = p.Parent;
                    }

                    AddAndResolveDuplicates(b, ret, field);
                }
                else if (code is AomBlock block)
                {
                    block.Parent = parent; // keep track of parent blocks

                    // make sure we define for cycle variables
                    if (block.Type == "for")
                    {
                        string[] partsFor = block.Condition.Substring(1, block.Condition.Length - 2).Split(';');
                        string[] parts = partsFor[0].Split(',');
                        var conditionChars = new char[] { '=' };
                        foreach (var part in parts)
                        {
                            int variableIndex = part.IndexOfAny(conditionChars);
                            if (variableIndex == -1)
                                continue;
                            string variable = part.Substring(0, variableIndex).TrimStart(conditionChars).Trim();

                            if (b.RequiresDefinition.FirstOrDefault(x => x.Name == variable) == null && b.AddedFields.FirstOrDefault(x => x.Name == variable) == null)
                            {
                                b.RequiresDefinition.Add(new AomField() { Name = variable, Type = "ns(32)" });
                            }
                        }
                    }

                    var blockFields = FlattenFields(b, block.Content, block);
                    foreach (var blockField in blockFields)
                    {
                        AddAndResolveDuplicates(b, ret, blockField);
                    }
                }
                else if (code is AomBlockIfThenElse blockifThenElse)
                {
                    blockifThenElse.Parent = parent; // keep track of parent blocks
                    ((AomBlock)blockifThenElse.BlockIf).Parent = parent;
                    if ((AomBlock)blockifThenElse.BlockElse != null) ((AomBlock)blockifThenElse.BlockElse).Parent = parent;

                    var blockFields = FlattenFields(b, ((AomBlock)blockifThenElse.BlockIf).Content, (AomBlock)blockifThenElse.BlockIf);
                    foreach (var blockField in blockFields)
                    {
                        AddAndResolveDuplicates(b, ret, blockField);
                    }

                    foreach (var blockelseif in blockifThenElse.BlockElseIf)
                    {
                        ((AomBlock)blockelseif).Parent = parent;
                        var blockElseIfFields = FlattenFields(b, ((AomBlock)blockelseif).Content, (AomBlock)blockelseif);
                        foreach (var blockField in blockElseIfFields)
                        {
                            AddAndResolveDuplicates(b, ret, blockField);
                        }
                    }

                    if (blockifThenElse.BlockElse != null)
                    {
                        var blockElseFields = FlattenFields(b, ((AomBlock)blockifThenElse.BlockElse).Content, (AomBlock)blockifThenElse.BlockElse);
                        foreach (var blockElseField in blockElseFields)
                        {
                            AddAndResolveDuplicates(b, ret, blockElseField);
                        }
                    }
                }
            }

            return ret.Values.ToList();
        }

        private void AddAndResolveDuplicates(AomClass b, Dictionary<string, AomField> ret, AomField field)
        {
            string name = field.Name;
            if (!ret.TryAdd(name, field))
            {
                if (string.IsNullOrEmpty(field.Type))
                {
                    if (field.Parameter != ret[name].Parameter) // we have to duplicate these
                    {
                        if (field.Name == "read_global_param")
                            return;

                        AddNewDuplicatedField(ret, field, name);
                    }
                    else
                    {
                        // just log a warning for now
                        Debug.WriteLine($"-------Field {field.Name} already exists in {b.ClassName} class, possible issue with the value being overwritten! Type: {field.Type}, Value: {field.Value}");
                    }
                }
                else
                {
                    // just log a warning for now
                    Debug.WriteLine($"-------Field {field.Name} already exists in {b.ClassName} class, possible issue with the value being overwritten! Type: {field.Type}, Value: {field.Value}");
                }
            }
        }

        private static void AddNewDuplicatedField(Dictionary<string, AomField> ret, AomField field, string name)
        {
            int index = 0;
            while (!ret.TryAdd($"{name}{index}", field))
            {
                index++;
            }

            field.ClassType = field.Name;
            field.Name = $"{name}{index}";
        }

        private string BuildRequiredVariables(AomClass aomClass)
        {
            string resultCode = "";

            foreach (var v in aomClass.RequiresDefinition)
            {
                if (!_fields.Add(v.Name))
                    continue;

                string type = GetCSharpTypeMapping()[v.Type];
                if (string.IsNullOrEmpty(v.FieldArray))
                {
                    string value = "= 0";
                    if (!string.IsNullOrWhiteSpace(v.Value))
                    {
                        value = v.Value;
                    }

                    resultCode += $"\r\n\t\t\t{type} {v.Name} {value};";
                }
                else
                {
                    int arrayDimensions = 0;
                    int indicesArrayDimensions = 0;
                    int level = 0;
                    for (int i = 0; i < v.FieldArray.Length; i++)
                    {
                        if (v.FieldArray[i] == '[')
                        {
                            if (level == 0)
                                arrayDimensions++;

                            level++;
                        }
                        else if (v.FieldArray[i] == ']')
                        {
                            level--;
                        }
                        else if (v.FieldArray[i] == ',')
                        {
                            indicesArrayDimensions++;
                        }
                    }

                    string array = "";
                    if (arrayDimensions == 1 && indicesArrayDimensions > 0)
                    {
                        array += "[";
                        for (int i = 0; i < indicesArrayDimensions; i++)
                        {
                            array += ",";
                        }
                        array += "]";
                    }
                    else
                    {
                        for (int i = 0; i < arrayDimensions; i++)
                        {
                            array += "[]";
                        }
                    }
                    resultCode += $"\r\n\t\t\t{type}{array} {v.Name} = null;"; // TODO: size
                }
            }

            return resultCode;
        }

        private string BuildField(AomClass ituClass, AomField field)
        {
            string type = GetCSharpType(field);
            if (ituClass.AddedFields != null && ituClass.AddedFields.FirstOrDefault(x => x.Name == field.Name) != null)
            {
                // NumOutputLayerSets - adding a calculated field as a property
                if (string.IsNullOrEmpty(field.Type))
                    type = GetCSharpTypeMapping()[ituClass.AddedFields.FirstOrDefault(x => x.Name == field.Name).Type];
            }

            string defaultInitializer = specificGenerator.GetFieldDefaultValue(field);
            string initializer = string.IsNullOrEmpty(defaultInitializer) ? "" : $"= {defaultInitializer}";

            string propertyName = field.Name.ToPropertyCase();
            if (propertyName == ituClass.ClassName.ToPropertyCase())
            {
                propertyName = $"_{propertyName}";
            }

            if (_fields.Add(field.Name))
            {
                return $"\t\tprivate {type} {field.Name}{initializer};\r\n";
            }
            else
            {
                return "";
            }
        }

        public void AddRequiresAllocation(AomField field)
        {
            AomBlock parent = null;
            parent = field.Parent;
            while (parent != null)
            {
                if (parent.Type == "for")
                {
                    // add the allocation above the first for in the hierarchy
                    parent.RequiresAllocation.Add(field);
                }

                parent = parent.Parent;
            }
        }

        public int GetLoopNestingLevel(AomCode code)
        {
            int ret = 0;
            var field = code as AomField;
            var block = code as AomBlock;
            AomBlock parent = null;

            if (field != null)
                parent = field.Parent;

            if (block != null)
                parent = block.Parent;

            while (parent != null)
            {
                if (parent.Type == "for" || parent.Type == "while")
                    ret++;
                parent = parent.Parent;
            }

            return ret;
        }

        private string GetSpacing(int level)
        {
            string ret = "";
            for (int i = 0; i < level; i++)
            {
                ret += "\t";
            }
            return ret;
        }

        private string BuildMethod(AomClass b, AomBlock parent, AomCode field, int level, MethodType methodType)
        {
            string spacing = GetSpacing(level);
            var block = field as AomBlock;
            if (block != null)
            {
                return BuildBlock(b, parent, block, level, methodType);
            }

            var blockIf = field as AomBlockIfThenElse;
            if (blockIf != null)
            {
                string ret = BuildBlock(b, parent, (AomBlock)blockIf.BlockIf, level, methodType);
                foreach (var elseBlock in blockIf.BlockElseIf)
                {
                    ret += BuildBlock(b, parent, (AomBlock)elseBlock, level, methodType);
                }
                if (blockIf.BlockElse != null)
                {
                    ret += BuildBlock(b, parent, (AomBlock)blockIf.BlockElse, level, methodType);
                }
                return ret;
            }

            var comment = field as AomComment;
            if (comment != null)
            {
                return BuildComment(b, comment, level, methodType);
            }

            var retrn = field as AomReturn;
            if (retrn != null)
            {
                return BuildReturn(b, parent, retrn, level, methodType);
            }

            var brk = field as AomBreak;
            if (brk != null)
            {
                return $"{GetSpacing(level)}break;";
            }

            if ((field as AomField).Type == null && (!string.IsNullOrWhiteSpace((field as AomField).Value) || !string.IsNullOrWhiteSpace((field as AomField).Increment)))
            {
                return BuildStatement(b, parent, field as AomField, level, methodType);
            }

            string name = (field as AomField).Name;
            string m = methodType == MethodType.Read ? GetReadMethod(b, field as AomField) : GetWriteMethod(field as AomField);

            string typedef = (field as AomField).FieldArray;

            string fieldComment = "";
            if (!string.IsNullOrEmpty((field as AomField)?.Comment))
            {
                fieldComment = "//" + (field as AomField).Comment;
            }

            string retm = "";

            if ((field as AomField).Type != null)
            {
                if (methodType == MethodType.Read)
                    retm = $"{spacing}{m} out this.{name}{typedef}, \"{name}\"); {fieldComment}";
                else if (methodType == MethodType.Write)
                    retm = $"{spacing}{m} this.{name}{typedef}, \"{name}\"); {fieldComment}";
                else
                    throw new NotSupportedException();
            }
            else
            {
                retm = $"{spacing}{m}; {fieldComment}";
            }

            return retm;
        }

        private string BuildReturn(AomClass b, AomBlock parent, AomReturn retrn, int level, MethodType methodType)
        {
            string p = "";
            if (!string.IsNullOrEmpty(retrn.Parameter))
                p = specificGenerator.FixStatement(retrn.Parameter, methodType);

            return $"{GetSpacing(level)}return" + p + ";";
        }

        private string BuildStatement(AomClass b, AomBlock parent, AomField field, int level, MethodType methodType)
        {
            string fieldValue = field.Value;
            string fieldArray = field.FieldArray;

            if (!string.IsNullOrEmpty(fieldArray))
                fieldArray = specificGenerator.FixStatement(fieldArray, methodType);

            if (!string.IsNullOrEmpty(fieldValue))
            {
                fieldValue = specificGenerator.FixStatement(fieldValue, methodType);
                fieldValue = specificGenerator.FixFieldValue(fieldValue);

                string trimmed = fieldValue.TrimStart(new char[] { ' ', '=' });
                if (trimmed.StartsWith("!"))
                {
                    fieldValue = $"= {trimmed.Substring(1)} == 0";
                }

                if (fieldValue.Contains("flag") && !fieldValue.Contains(")"))
                    fieldValue = fieldValue.Replace("||", "|").Replace("&&", "&");

                fieldValue = FixMissingFields(b, fieldValue);
            }

            if (b.FlattenedFields.FirstOrDefault(x => x.Name == field.Name) != null || parent != null)
            {
                return $"{GetSpacing(level)}{field.Name}{fieldArray}{field.Increment}{fieldValue}".TrimEnd() + ";";
            }
            else
            {
                if (b.AddedFields.FirstOrDefault(x => x.Name == field.Name) == null && b.RequiresDefinition.FirstOrDefault(x => x.Name == field.Name) == null)
                {
                    b.AddedFields.Add(new AomField() { Name = field.Name, Value = fieldValue });
                    return $"{GetSpacing(level)}uint {field.Name}{fieldArray}{fieldValue}".TrimEnd() + ";";
                }
                else
                {
                    return $"{GetSpacing(level)}{field.Name}{fieldArray}{field.Increment}{fieldValue}".TrimEnd() + ";";
                }
            }
        }

        private string FixMissingFields(AomClass b, string condition)
        {
            Regex r = new Regex("\\b[a-zA-Z_][\\w_]+");
            var matches = r.Matches(condition).OfType<Match>().Select(x => x.Value).Distinct().ToArray();
            foreach (var match in matches)
            {
                if (b.FlattenedFields.FirstOrDefault(x => x.Name == match) == null &&
                    b.AddedFields.FirstOrDefault(x => x.Name == match) == null &&
                    b.RequiresDefinition.FirstOrDefault(x => x.Name == match) == null
                    )
                {
                    condition = condition.Replace(match, specificGenerator.ReplaceParameter(match));
                }
            }

            return condition;
        }

        private string BuildComment(AomClass b, AomComment comment, int level, MethodType methodType)
        {
            return $"/* {comment.Comment} */\r\n";
        }

        private string BuildBlock(AomClass b, AomBlock parent, AomBlock block, int level, MethodType methodType)
        {
            string spacing = GetSpacing(level);
            string ret = "";

            string condition = block.Condition;
            string blockType = block.Type;

            if (!string.IsNullOrEmpty(condition))
            {
                if (blockType == "if" || blockType == "else if" || blockType == "while")
                {
                    condition = FixCondition(b, condition, methodType);
                }
                else if (blockType == "for")
                {
                    condition = specificGenerator.FixCondition(condition, methodType);
                }
            }

            if (!string.IsNullOrEmpty(condition))
            {
                condition = condition.Replace("<<", "<< (int)");

                condition = FixMissingFields(b, condition);
            }

            ret += $"\r\n{spacing}{blockType} {condition}\r\n{spacing}{{";

            foreach (var field in block.Content)
            {
                ret += "\r\n" + BuildMethod(b, block, field, level + 1, methodType);
            }

            ret += $"\r\n{spacing}}}";

            return ret;
        }

        private string FixCondition(AomClass b, string condition, MethodType methodType)
        {
            string[] parts = condition.Substring(1, condition.Length - 2).Split(new string[] { "||", "&&" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = parts[i].Trim();

                if (parts[i].StartsWith("!"))
                {
                    string trimmed = parts[i].Trim(new char[] { ' ', '(', ')' });
                    if (!trimmed.Contains("(") && !parts[i].Contains("()")) 
                    {
                        // we don't have bool anymore, so in this case it's easy fix
                        condition = condition.Replace(parts[i].TrimEnd(')'), parts[i].TrimEnd(')').Substring(1, parts[i].TrimEnd(')').Length - 1) + "== 0");
                    }
                }
                else if (!parts[i].Contains('=') && !parts[i].Contains('>') && !parts[i].Contains('<'))
                {
                    string trimmed = parts[i].Trim(new char[] { ' ', '(', ')' });
                    if (!trimmed.Contains("(") && !parts[i].Contains("()")) 
                    {
                        if (trimmed.StartsWith("!"))
                        {
                            if (condition.Contains($"{trimmed} != 0"))
                                condition = condition.Replace($"{trimmed} != 0", $"{trimmed.Substring(1)} == 0");
                            else
                                condition = condition.Replace(trimmed, trimmed.Substring(1) + " == 0");
                        }
                        else
                            condition = condition.Replace(trimmed, trimmed + " != 0");
                    }
                }
            }

            condition = specificGenerator.FixCondition(condition, methodType);

            return condition;
        }

        private string GetCSharpType(AomField field)
        {
            Dictionary<string, string> map = GetCSharpTypeMapping();

            if (string.IsNullOrWhiteSpace(field.Type))
            {
                if (
                    field.ClassType == "operating_point_idc" ||
                    field.ClassType == "decoder_model_present_for_this_op" ||
                    field.ClassType == "initial_display_delay_present_for_this_op" ||
                    field.ClassType == "decoder_model_present_for_this_op" ||
                    field.ClassType == "seq_tier" ||
                    field.ClassType == "cdef_y_pri_strength" ||
                    field.ClassType == "cdef_y_sec_strength" ||
                    field.ClassType == "cdef_uv_pri_strength" ||
                    field.ClassType == "cdef_uv_sec_strength" ||
                    field.ClassType == "cdef_damping_minus_3" ||
                    field.ClassType == "loop_filter_level" ||
                    field.ClassType == "loop_filter_ref_deltas" ||
                    field.ClassType == "GmType" ||
                    field.ClassType == "tile_size_minus_1" ||
                    field.ClassType == "initial_display_delay_minus_1" ||
                    field.ClassType == "RefValid" ||
                    field.ClassType == "RefOrderHint" ||
                    field.ClassType == "OrderHints" ||
                    field.ClassType == "ref_order_hint" ||
                    field.ClassType == "expectedFrameId" ||
                    field.ClassType == "RefFrameSignBias" ||
                    field.ClassType == "LoopRestorationSize" ||
                    field.ClassType == "FrameRestorationType" ||
                    field.ClassType == "MiColStarts" ||
                    field.ClassType == "MiRowStarts" ||
                    field.ClassType == "loop_filter_mode_deltas" ||
                    field.ClassType == "LosslessArray" ||
                    field.ClassType == "SkipModeFrame"
                    )
                {
                    return map["su(32)[]"];
                }
                else if(
                    field.ClassType == "FeatureEnabled" ||
                    field.ClassType == "FeatureData" ||
                    field.ClassType == "gm_params" ||
                    field.ClassType == "SegQMLevel" 
                    )
                {
                    return map["su(32)[][]"];
                }

                    return map["su(32)"];
            }

            int arrayDimensions = 0;
            if (!string.IsNullOrEmpty(field.FieldArray))
            {
                int level = 0;
                for (int i = 0; i < field.FieldArray.Length; i++)
                {
                    if (field.FieldArray[i] == '[')
                    {
                        if (level == 0)
                            arrayDimensions++;

                        level++;
                    }
                    else if (field.FieldArray[i] == ']')
                    {
                        level--;
                    }
                }
            }


            string arraySuffix = "";
            for (int i = 0; i < arrayDimensions; i++)
            {
                arraySuffix += "[]";
            }

            return map[field.Type] + arraySuffix;
        }

        private static Dictionary<string, string> GetCSharpTypeMapping()
        {
            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                { "f(1)",                       "uint" },
                { "f(2)",                       "uint" },
                { "f(3)",                       "uint" },
                { "f(4)",                       "uint" },
                { "f(5)",                       "uint" },
                { "f(6)",                       "uint" },
                { "f(8)",                       "uint" },
                { "f(9)",                       "uint" },
                { "f(12)",                      "uint" },
                { "f(16)",                      "uint" },
                { "f(32)",                      "uint" },
                { "f(32)[]",                    "uint[]" },
                { "f(b2)",                      "uint" },
                { "f(bitsToRead)",              "uint" },
                { "f(idLen)",                   "uint" },
                { "f(N)",                       "uint" },
                { "f(n)",                       "uint" },
                { "f(OrderHintBits)",           "uint" },
                { "f(SUPERRES_DENOM_BITS)",     "uint" },
                { "f(tileBits)",                "uint" },
                { "f(TileRowsLog2+TileColsLog2)", "uint" },
                { "f(time_offset_length)",      "uint" },
                { "L(1)",                       "uint" },
                { "L(2)",                       "uint" },
                { "L(3)",                       "uint" },
                { "L(b2)",                      "uint" },
                { "L(BitDepth)",                "uint" },
                { "L(cdef_bits)",               "uint" },
                { "L(delta_q_rem_bits)",        "uint" },
                { "L(n)",                       "uint" },
                { "L(paletteBits)",             "uint" },
                { "L(SGRPROJ_PARAMS_BITS)",     "uint" },
                { "le(TileSizeBytes)",          "uint" },
                { "leb128()",                   "uint" },
                { "ns(32)",                     "uint" },
                { "ns(maxWidth)",               "uint" },
                { "ns(maxHeight)",              "uint" },
                { "ns(numSyms - mk)",           "uint" },
                { "NS(numSyms - mk)",           "uint" },
                { "NS(PaletteSizeUV)",          "uint" },
                { "S()",                        "uint" },
                { "su(32)",                     "int" },
                { "su(32)[]",                   "int[]" },
                { "su(32)[][]",                 "int[][]" },
                { "su(64)",                     "long" },
                { "su(1+6)",                    "uint" },
                { "su(1+bitsToRead)",           "uint" },
                { "uvlc()",                     "uint" },
            };
            return map;
        }

        private string GetReadMethod(AomClass b, AomField aomField)
        {
            switch (aomField.Type)
            {
                case "f(1)":
                    return "stream.ReadFixed(1,";
                
                case "f(2)":
                    return "stream.ReadFixed(2,";
                
                case "f(3)":
                    return "stream.ReadFixed(3,";

                case "f(4)":
                    return "stream.ReadFixed(4,";

                case "f(5)":
                    return "stream.ReadFixed(5,";

                case "f(6)":
                    return "stream.ReadFixed(6,";

                case "f(8)":
                    return "stream.ReadFixed(8,";

                case "f(9)":
                    return "stream.ReadFixed(9,";

                case "f(12)":
                    return "stream.ReadFixed(12,";

                case "f(16)":
                    return "stream.ReadFixed(16,";

                case "f(32)":
                    return "stream.ReadFixed(32,";

                case "f(b2)":
                    return "stream.ReadVariable(b2,";

                case "f(bitsToRead)":
                    return "stream.ReadVariable(bitsToRead,";

                case "f(idLen)":
                    return "stream.ReadVariable(idLen,";

                case "f(N)":
                    return "stream.ReadVariable(N,";

                case "f(n)":
                    return "stream.ReadVariable(n,";

                case "f(OrderHintBits)":
                    return "stream.ReadVariable(OrderHintBits,";

                case "f(SUPERRES_DENOM_BITS)":
                    return "stream.ReadVariable(SUPERRES_DENOM_BITS,";

                case "f(tileBits)":
                    return "stream.ReadVariable(tileBits,";

                case "f(TileRowsLog2+TileColsLog2)":
                    return "stream.ReadVariable(TileRowsLog2+TileColsLog2,";

                case "f(time_offset_length)":
                    return "stream.ReadVariable(time_offset_length,";

                case "L(1)":
                    return "stream.ReadL(1,";

                case "L(2)":
                    return "stream.ReadL(2,";

                case "L(3)":
                    return "stream.ReadL(3,";

                case "L(b2)":
                    return "stream.ReadL(b2,";

                case "L(BitDepth)":
                    return "stream.ReadL(BitDepth,";

                case "L(cdef_bits)":
                    return "stream.ReadL(cdef_bits,";

                case "L(delta_q_rem_bits)":
                    return "stream.ReadL(delta_q_rem_bits,";

                case "L(n)":
                    return "stream.ReadL(n,";

                case "L(paletteBits)":
                    return "stream.ReadL(paletteBits,";

                case "L(SGRPROJ_PARAMS_BITS)":
                    return "stream.ReadL(SGRPROJ_PARAMS_BITS,";

                case "le(TileSizeBytes)":
                    return "stream.ReadLeVar(TileSizeBytes,";

                case "leb128()":
                    return "stream.ReadLeb128(";

                case "ns(32)":
                    return "stream.ReadUnsignedInt32(size,";

                case "ns(maxHeight)":
                    return "stream.ReadUnsignedInt(maxHeight,";

                case "ns(maxWidth)":
                    return "stream.ReadUnsignedInt(maxWidth,";

                case "NS(numSyms - mk)":
                case "ns(numSyms - mk)":
                    return "stream.ReadUnsignedInt(numSyms - mk,";

                case "NS(PaletteSizeUV)":
                    return "stream.ReadNS(PaletteSizeUV,";

                case "S()":
                    return "stream.ReadS(size,";

                case "su(32)":
                    return "stream.ReadSignedInt32(size,";

                case "su(32)[]":
                    return "stream.ReadSignedInt32(size,";

                case "su(32)[][]":
                    return "stream.ReadSignedInt32(size,";

                case "su(1+6)":
                    return "stream.ReadSignedIntVar(1+6,";

                case "su(1+bitsToRead)":
                    return "stream.ReadSignedIntVar(1+bitsToRead,";

                case "uvlc()":
                    return "stream.ReadUvlc(";

                default:
                    if (aomField.Type == null)
                    {
                        return $"Read{aomField.ClassType.ToPropertyCase()}{aomField.Parameter}";
                    }
                    throw new NotImplementedException();
            }
        }

        private string GetWriteMethod(AomField aomField)
        {
            switch (aomField.Type)
            {
                case "f(1)":
                    return "stream.WriteFixed(1,";
                
                case "f(2)":
                    return "stream.WriteFixed(2,";
                
                case "f(3)":
                    return "stream.WriteFixed(3,";

                case "f(4)":
                    return "stream.WriteFixed(4,";

                case "f(5)":
                    return "stream.WriteFixed(5,";

                case "f(6)":
                    return "stream.WriteFixed(6,";

                case "f(8)":
                    return "stream.WriteFixed(8,";

                case "f(9)":
                    return "stream.WriteFixed(9,";

                case "f(12)":
                    return "stream.WriteFixed(12,";

                case "f(16)":
                    return "stream.WriteFixed(16,";

                case "f(32)":
                    return "stream.WriteFixed(32,";

                case "f(b2)":
                    return "stream.WriteVariable(b2,";

                case "f(bitsToRead)":
                    return "stream.WriteVariable(bitsToRead,";

                case "f(idLen)":
                    return "stream.WriteVariable(idLen,";

                case "f(N)":
                    return "stream.WriteVariable(N,";

                case "f(n)":
                    return "stream.WriteVariable(n,";

                case "f(paletteBits)":
                    return "stream.WriteVariable(paletteBits,";

                case "f(OrderHintBits)":
                    return "stream.WriteVariable(OrderHintBits,";

                case "f(SUPERRES_DENOM_BITS)":
                    return "stream.WriteVariable(SUPERRES_DENOM_BITS,";

                case "f(tileBits)":
                    return "stream.WriteVariable(tileBits,";

                case "f(TileRowsLog2+TileColsLog2)":
                    return "stream.WriteVariable(TileRowsLog2+TileColsLog2,";

                case "f(time_offset_length)":
                    return "stream.WriteVariable(time_offset_length,";

                case "L(1)":
                    return "stream.WriteL(1, ";

                case "L(2)":
                    return "stream.WriteL(2, ";

                case "L(3)":
                    return "stream.WriteL(3, ";

                case "L(b2)":
                    return "stream.WriteL(b2, ";

                case "L(BitDepth)":
                    return "stream.WriteL(BitDepth, ";

                case "L(cdef_bits)":
                    return "stream.WriteL(cdef_bits, ";

                case "L(delta_q_rem_bits)":
                    return "stream.WriteL(delta_q_rem_bits, ";

                case "L(n)":
                    return "stream.WriteL(n, ";

                case "L(paletteBits)":
                    return "stream.WriteL(paletteBits, ";

                case "L(SGRPROJ_PARAMS_BITS)":
                    return "stream.WriteL(SGRPROJ_PARAMS_BITS, ";

                case "le(TileSizeBytes)":
                    return "stream.WriteLeVar(TileSizeBytes, ";

                case "leb128()":
                    return "stream.WriteLeb128(";

                case "ns(32)":
                    return "stream.WriteUnsignedInt32(";

                case "ns(maxHeight)":
                    return "stream.WriteUnsignedInt(maxHeight,";

                case "ns(maxWidth)":
                    return "stream.WriteUnsignedInt(maxWidth,";

                case "NS(numSyms - mk)":
                case "ns(numSyms - mk)":
                    return "stream.WriteUnsignedInt(numSyms - mk,";

                case "NS(PaletteSizeUV)":
                    return "stream.WriteNS(PaletteSizeUV,";

                case "S()":
                    return "stream.WriteS(";

                case "su(32)":
                    return "stream.WriteSignedInt32(";

                case "su(32)[]":
                    return "stream.WriteSignedInt32(";

                case "su(32)[][]":
                    return "stream.WriteSignedInt32(";

                case "su(1+6)":
                    return "stream.WriteSignedIntVar(1+6,";

                case "su(1+bitsToRead)":
                    return "stream.WriteSignedIntVar(1+bitsToRead,";

                case "uvlc()":
                    return "stream.WriteUvlc(";

                default:
                    if (aomField.Type == null)
                    {
                        return $"Write{aomField.ClassType.ToPropertyCase()}{aomField.Parameter}";
                    }
                    throw new NotImplementedException();
            }
        }
    }
}
