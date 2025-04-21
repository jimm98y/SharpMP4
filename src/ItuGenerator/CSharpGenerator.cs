﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace ItuGenerator
{
    public enum MethodType
    {
        Read,
        Write
    }

    public interface ICustomGenerator
    {
        string PreprocessDefinitionsFile(string definitions);
        string GetFieldDefaultValue(ItuField field);
        void FixClassParameters(ItuClass ituClass);
        string ReplaceParameter(string parameter);
        string FixMissingParameters(ItuClass b, string parameter, string classType);
        string FixCondition(string condition, MethodType methodType);
        string FixStatement(string fieldValue);
        string GetCtorParameterType(string parameter);
        string FixFieldValue(string fieldValue);
        void FixMethodAllocation(string name, ref string method, ref string typedef);
    }

    public class CSharpGenerator
    {
        private ICustomGenerator specificGenerator = null;

        public CSharpGenerator(ICustomGenerator specificGenerator)
        {
            this.specificGenerator = specificGenerator ?? throw new ArgumentNullException(nameof(specificGenerator));
        }

        public string GenerateParser(string type, IEnumerable<ItuClass> ituClasses)
        {
            string resultCode =
            $@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using SharpH26X;

namespace Sharp{type}
{{
";
            resultCode += GenerateContext(type, ituClasses);

            foreach (var ituClass in ituClasses)
            {
                resultCode += GenerateClass(ituClass);
            }

            resultCode +=
                @"
}
";
            return resultCode;
        }

        private string GenerateContext(string type, IEnumerable<ItuClass> ituClasses)
        {
            string ret = @$"
    public class {type}Context : IItuContext
    {{
            public NalUnit NalHeader {{ get; set; }}
";
            var rbsp = ituClasses.Where(x => x.ClassName.EndsWith("rbsp")).ToArray();
            foreach (var cls in rbsp)
            {
                ret += $"\t\tpublic {cls.ClassName.ToPropertyCase()} {cls.ClassName.ToPropertyCase()} {{ get; set; }}\r\n";
            }

            ret += @$"
    }}
";
            return ret;
        }

        private string GenerateClass(ItuClass ituClass)
        {
            specificGenerator.FixClassParameters(ituClass);

            string resultCode = @$"
    /*
{ituClass.Syntax.Replace("*/", "*//*")}
    */
    public class {ituClass.ClassName.ToPropertyCase()} : IItuSerializable
    {{
";
            resultCode += GenerateFields(ituClass);

            resultCode += @"
         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }
";

            resultCode += GenerateCtor(ituClass);

            resultCode += $@"
         public ulong Read(IItuContext context, ItuStream stream)
         {{
             ulong size = 0;
";
            resultCode += BuildRequiredVariables(ituClass);

            foreach (var field in ituClass.Fields)
            {
                resultCode += "\r\n" + BuildMethod(ituClass, null, field, 3, MethodType.Read);
            }
            ituClass.AddedFields.Clear();
            resultCode += $@"

             return size;
         }}
";

            resultCode += $@"
         public ulong Write(IItuContext context, ItuStream stream)
         {{
             ulong size = 0;
";
            resultCode += BuildRequiredVariables(ituClass);

            foreach (var field in ituClass.Fields)
            {
                resultCode += "\r\n" + BuildMethod(ituClass, null, field, 3, MethodType.Write);
            }
            ituClass.AddedFields.Clear();
            resultCode += $@"

             return size;
         }}
";

            resultCode += $@"
    }}
";
            return resultCode;
        }
                
        private static string BuildRequiredVariables(ItuClass ituClass)
        {
            string resultCode = "";

            foreach (var v in ituClass.RequiresDefinition)
            {
                string type = GetCSharpTypeMapping()[v.Type];
                if (string.IsNullOrEmpty(v.FieldArray))
                {
                    string value = "= 0";
                    if(!string.IsNullOrWhiteSpace(v.Value))
                    {
                        value = v.Value;
                    }

                    resultCode += $"\r\n{type} {v.Name} {value};";
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
                    resultCode += $"\r\n{type}{array} {v.Name} = null;"; // TODO: size
                }
            }

            return resultCode;
        }

        public int GetLoopNestingLevel(ItuCode code)
        {
            int ret = 0;
            var field = code as ItuField;
            var block = code as ItuBlock;
            ItuBlock parent = null;

            if (field != null)
                parent = field.Parent;

            if (block != null)
                parent = block.Parent;

            while (parent != null)
            {
                if (parent.Type == "for" || parent.Type == "do" || parent.Type == "while")
                    ret++;
                parent = parent.Parent;
            }

            return ret;
        }

        private string BuildMethod(ItuClass b, ItuBlock parent, ItuCode field, int level, MethodType methodType)
        {
            string spacing = GetSpacing(level);
            var block = field as ItuBlock;
            if (block != null)
            {
                return BuildBlock(b, parent, block, level, methodType);
            }

            var blockIf = field as ItuBlockIfThenElse;
            if (blockIf != null)
            {
                string ret = BuildBlock(b, parent, (ItuBlock)blockIf.BlockIf, level, methodType);
                foreach(var elseBlock in blockIf.BlockElseIf)
                {
                    ret += BuildBlock(b, parent, (ItuBlock)elseBlock, level, methodType);
                }
                if(blockIf.BlockElse != null)
                {
                    ret += BuildBlock(b, parent, (ItuBlock)blockIf.BlockElse, level, methodType);
                }
                return ret;
            }

            var comment = field as ItuComment;
            if (comment != null)
            {
                return BuildComment(b, comment, level, methodType);
            }

            if ((field as ItuField).Type == null && (!string.IsNullOrWhiteSpace((field as ItuField).Value) || !string.IsNullOrWhiteSpace((field as ItuField).Increment)))
            {
                return BuildStatement(b, parent, field as ItuField, level, methodType);
            }
           
            string name = GetFieldName(field as ItuField);
            string m = methodType == MethodType.Read ? GetReadMethod(b, field as ItuField) : GetWriteMethod(field as ItuField);

            string typedef = (field as ItuField).FieldArray;

            string fieldComment = "";
            if (!string.IsNullOrEmpty((field as ItuField)?.Comment))
            {
                fieldComment = "//" + (field as ItuField).Comment;
            }

            string boxSize = "size += ";

            if (GetLoopNestingLevel(field) > 0)
            {
                if (name != "ar_bit_equal_to_zero" && !(field as ItuField).MakeList)
                {
                    string suffix;
                    GetNestedInLoopSuffix(field, typedef, out suffix);
                    typedef += suffix;

                    m = FixNestedInLoopVariables(field, m, "(", ",");
                    m = FixNestedInLoopVariables(field, m, ")", ","); // when casting
                    m = FixNestedInLoopVariables(field, m, "", " ");
                }
            }

            if (m.Contains("###value###") && methodType == MethodType.Read)
            {
                specificGenerator.FixMethodAllocation(name, ref m, ref typedef);

                if ((field as ItuField).MakeList)
                {
                    // special case, create class first, then read it
                    m = m.Replace("###value###", $"{spacing}this.{name}{typedef}.Add(whileIndex, ");
                    m = m.Replace("###size###", $");\r\n{spacing}{boxSize}");
                    return $"{m} this.{name}{typedef}[whileIndex]); {fieldComment}";
                }
                else
                {
                    // special case, create class first, then read it
                    m = m.Replace("###value###", $"{spacing}this.{name}{typedef} = ");
                    m = m.Replace("###size###", $";\r\n{spacing}{boxSize}");
                    return $"{m} this.{name}{typedef}); {fieldComment}";
                }
            }
            else
            {
                if ((field as ItuField).MakeList)
                {
                    if (methodType == MethodType.Read)
                        return $"{spacing}{boxSize}{m} whileIndex, this.{name}{typedef}); {fieldComment}";
                    else if (methodType == MethodType.Write)
                        return $"{spacing}{boxSize}{m} whileIndex, this.{name}{typedef}); {fieldComment}";
                    else
                        throw new NotSupportedException();
                }
                else
                {
                    if (methodType == MethodType.Read)
                        return $"{spacing}{boxSize}{m} out this.{name}{typedef}); {fieldComment}";
                    else if (methodType == MethodType.Write)
                        return $"{spacing}{boxSize}{m} this.{name}{typedef}); {fieldComment}";
                    else
                        throw new NotSupportedException();
                }
            }
        }

        private string BuildStatement(ItuClass b, ItuBlock parent, ItuField field, int level, MethodType methodType)
        {
            string fieldValue = field.Value;
            string fieldArray = field.FieldArray;

            if (!string.IsNullOrEmpty(fieldValue))
            {
                fieldValue = specificGenerator.FixStatement(fieldValue);
                fieldValue = specificGenerator.FixFieldValue(fieldValue);

                string trimmed = fieldValue.TrimStart(new char[] { ' ', '=' });
                if (trimmed.StartsWith('!'))
                {
                    fieldValue = $"= {trimmed.Substring(1)} != 0";
                }

                if (!fieldValue.Contains("||") && !fieldValue.Contains("&&"))
                    fieldValue = fieldValue.Replace("_flag    != 0", "_flag");

                if (fieldValue.Contains("flag") && !fieldValue.Contains(")"))
                    fieldValue = fieldValue.Replace("||", "|").Replace("&&", "&");

                if ((fieldValue.Contains("==") || fieldValue.Contains(">") || fieldValue.Contains("<")) && !fieldValue.Contains("?") && !fieldValue.Contains("<<") && !fieldValue.Contains(">>"))
                    fieldValue += " ? (uint)1 : (uint)0";

                if (fieldValue.Contains("+ 256"))
                    fieldValue = "= (uint)" + fieldValue.TrimStart(' ', '=');

                fieldValue = FixMissingFields(b, fieldValue);
                fieldValue = FixNestedInLoopVariables(field, fieldValue);
            }

            if (b.FlattenedFields.FirstOrDefault(x => x.Name == field.Name) != null || parent != null)
            {
                return $"{GetSpacing(level)}{field.Name}{fieldArray}{field.Increment}{fieldValue};";
            }
            else
            {
                if (b.AddedFields.FirstOrDefault(x => x.Name == field.Name) == null && b.RequiresDefinition.FirstOrDefault(x => x.Name == field.Name) == null)
                {
                    b.AddedFields.Add(new ItuField() { Name = field.Name, Value = fieldValue });
                    return $"{GetSpacing(level)}uint {field.Name}{fieldArray}{fieldValue};";
                }
                else
                {
                    return $"{GetSpacing(level)}{field.Name}{fieldArray}{field.Increment}{fieldValue};";
                }
            }
        }

        private string GetWriteMethod(ItuField ituField)
        {
            switch (ituField.Type)
            {
                case "f(1)":
                    return "stream.WriteFixed(1, ";
                case "f(8)":
                    return "stream.WriteFixed(8, ";
                case "f(16)":
                    return "stream.WriteFixed(16, ";
                case "u(1)":
                    return "stream.WriteUnsignedInt(1, ";
                case "u(1) | ae(v)":
                    return "stream.WriteUnsignedInt(1, ";
                case "u(2)":
                    return "stream.WriteUnsignedInt(2, ";
                case "u(3)":
                    return "stream.WriteUnsignedInt(3, ";
                case "u(3) | ae(v)":
                    return "stream.WriteUnsignedInt(3, ";
                case "u(4)":
                    return "stream.WriteUnsignedInt(4, ";
                case "u(5)":
                    return "stream.WriteUnsignedInt(5, ";
                case "u(6)":
                    return "stream.WriteUnsignedInt(6, ";
                case "u(7)":
                    return "stream.WriteUnsignedInt(7, ";
                case "u(8)":
                    return "stream.WriteUnsignedInt(8, ";
                case "u(9)":
                    return "stream.WriteUnsignedInt(9, ";
                case "u(10)":
                    return "stream.WriteUnsignedInt(10, ";
                case "u(16)":
                    return "stream.WriteUnsignedInt(16, ";
                case "u(20)":
                    return "stream.WriteUnsignedInt(20, ";
                case "u(24)":
                    return "stream.WriteUnsignedInt(24, ";
                case "u(32)":
                    return "stream.WriteUnsignedInt(32, ";
                case "u(33)":
                    return "stream.WriteUnsignedInt(33, ";
                case "u(34)":
                    return "stream.WriteUnsignedInt(34, ";
                case "u(35)":
                    return "stream.WriteUnsignedInt(35, ";
                case "u(43)":
                    return "stream.WriteUnsignedInt(43, ";
                case "u(128)":
                    return "stream.WriteUnsignedInt(128, ";
                case "i(32)":
                    return "stream.WriteSignedInt(32, ";
                case "u(v)":
                    return $"stream.WriteUnsignedIntVariable({specificGenerator.ReplaceParameter(ituField.Name)}, ";
                case "i(v)":
                    return $"stream.WriteSignedIntVariable({specificGenerator.ReplaceParameter(ituField.Name)}, ";
                case "ue(v)":
                    return "stream.WriteUnsignedIntGolomb(";
                case "ae(v)":
                    return "stream.WriteUnsignedIntGolomb(";
                case "ce(v)":
                    return "stream.WriteUnsignedIntGolomb(";
                case "ue(v) | ae(v)":
                    return "stream.WriteUnsignedIntGolomb(";
                case "me(v) | ae(v)":
                    return "stream.WriteUnsignedIntGolomb(";
                case "se(v)":
                    return "stream.WriteSignedIntGolomb(";
                case "st(v)":
                    return "stream.WriteUtf8String(";
                case "se(v) | ae(v)":
                    return "stream.WriteSignedIntGolomb(";
                case "te(v) | ae(v)":
                    return "stream.WriteSignedIntGolomb(";
                case "b(8)":
                    return "stream.WriteBits(8, ";
                default:
                    if (ituField.Type == null)
                        return $"stream.WriteClass<{ituField.ClassType.ToPropertyCase()}>(context, ";
                    throw new NotImplementedException();
            }
        }

        private string GetReadMethod(ItuClass b, ItuField ituField)
        {
            switch(ituField.Type)
            {
                case "f(1)":
                    return "stream.ReadFixed(size, 1, ";
                case "f(8)":
                    return "stream.ReadFixed(size, 8, ";
                case "f(16)":
                    return "stream.ReadFixed(size, 16, ";
                case "u(1)":
                    return "stream.ReadUnsignedInt(size, 1, ";               
                case "u(1) | ae(v)":
                    return "stream.ReadUnsignedInt(size, 1, ";               
                case "u(2)":
                    return "stream.ReadUnsignedInt(size, 2, ";
                case "u(3)":
                    return "stream.ReadUnsignedInt(size, 3, ";
                case "u(3) | ae(v)":
                    return "stream.ReadUnsignedInt(size, 3, ";
                case "u(4)":
                    return "stream.ReadUnsignedInt(size, 4, ";
                case "u(5)":
                    return "stream.ReadUnsignedInt(size, 5, ";
                case "u(6)":
                    return "stream.ReadUnsignedInt(size, 6, ";
                case "u(7)":
                    return "stream.ReadUnsignedInt(size, 7, ";
                case "u(8)":
                    return "stream.ReadUnsignedInt(size, 8, ";
                case "u(9)":
                    return "stream.ReadUnsignedInt(size, 9, ";
                case "u(10)":
                    return "stream.ReadUnsignedInt(size, 10, ";
                case "u(16)":
                    return "stream.ReadUnsignedInt(size, 16, ";
                case "u(20)":
                    return "stream.ReadUnsignedInt(size, 20, ";
                case "u(24)":
                    return "stream.ReadUnsignedInt(size, 24, ";
                case "u(32)":
                    return "stream.ReadUnsignedInt(size, 32, ";
                case "u(33)":
                    return "stream.ReadUnsignedInt(size, 33, ";
                case "u(34)":
                    return "stream.ReadUnsignedInt(size, 34, ";
                case "u(35)":
                    return "stream.ReadUnsignedInt(size, 35, ";
                case "u(43)":
                    return "stream.ReadUnsignedInt(size, 43, ";
                case "u(128)":
                    return "stream.ReadUnsignedInt(size, 128, ";
                case "i(32)":
                    return "stream.ReadSignedInt(size, 32, ";
                case "u(v)":
                    return $"stream.ReadUnsignedIntVariable(size, {specificGenerator.ReplaceParameter(ituField.Name)}, ";
                case "i(v)":
                    return $"stream.ReadSignedIntVariable(size, {specificGenerator.ReplaceParameter(ituField.Name)}, ";
                case "ue(v)":
                    return "stream.ReadUnsignedIntGolomb(size, ";
                case "ae(v)":
                    return "stream.ReadUnsignedIntGolomb(size, ";
                case "ce(v)":
                    return "stream.ReadUnsignedIntGolomb(size, ";
                case "ue(v) | ae(v)":
                    return "stream.ReadUnsignedIntGolomb(size, ";
                case "me(v) | ae(v)":
                    return "stream.ReadUnsignedIntGolomb(size, ";
                case "se(v)":
                    return "stream.ReadSignedIntGolomb(size, ";
                case "st(v)":
                    return "stream.ReadUtf8String(size, ";
                case "se(v) | ae(v)":
                    return "stream.ReadSignedIntGolomb(size, ";
                case "te(v) | ae(v)":
                    return "stream.ReadSignedIntGolomb(size, ";
                case "b(8)":
                    return "stream.ReadBits(size, 8, ";
                default:
                    if (ituField.Type == null)
                        return $"###value### new {ituField.ClassType.ToPropertyCase()}{specificGenerator.FixMissingParameters(b, ituField.Parameter, ituField.ClassType)} ###size### stream.ReadClass<{ituField.ClassType.ToPropertyCase()}>(size, context, ";
                    throw new NotImplementedException();
            }
        }      

        private static Dictionary<string, string> GetCSharpTypeMapping()
        {
            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                { "f(1)",                       "uint" },
                { "f(8)",                       "uint" },
                { "f(16)",                      "uint" },
                { "u(1)",                       "byte" },
                { "u(1) | ae(v)",               "uint" },
                { "u(2)",                       "uint" },
                { "u(3)",                       "uint" },
                { "u(3) | ae(v)",               "uint" },
                { "u(4)",                       "uint" },
                { "u(5)",                       "uint" },
                { "u(6)",                       "uint" },
                { "u(7)",                       "uint" },
                { "u(8)",                       "uint" },
                { "u(9)",                       "uint" },
                { "u(10)",                      "uint" },
                { "u(16)",                      "uint" },
                { "u(20)",                      "uint" },
                { "u(24)",                      "uint" },
                { "u(32)",                      "uint" },
                { "u(33)",                      "ulong" },
                { "u(34)",                      "ulong" },
                { "u(35)",                      "ulong" },
                { "u(43)",                      "ulong" },
                { "u(128)",                     "BigInteger" },
                { "u(v)",                       "uint" },
                { "ue(v)",                      "uint" },
                { "ae(v)",                      "uint" },
                { "ce(v)",                      "uint" },
                { "ue(v) | ae(v)",              "uint" },
                { "me(v) | ae(v)",              "uint" },
                { "i(32)",                      "int" },
                { "i(v)",                       "int" },
                { "se(v)",                      "int" },
                { "st(v)",                      "byte[]" },
                { "se(v) | ae(v)",              "int" },
                { "te(v) | ae(v)",              "int" },
                { "b(8)",                       "byte" },
                { "u(32)[]",                    "uint[]" },
                { "u(32)[][]",                  "int[][]" },
                { "u(32)[,]",                   "uint[,]" },

                // added
                { "bool",                       "bool" },
                { "i(64)",                      "long" },
            };
            return map;
        }

        private string GetCSharpType(ItuField field)
        {
            if (string.IsNullOrWhiteSpace(field.Type))
            {
                if (!string.IsNullOrEmpty(field.ClassType))
                    return field.ClassType.ToPropertyCase();
                else
                    return field.Name.ToPropertyCase();
            }

            Dictionary<string, string> map = GetCSharpTypeMapping();

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

        private string BuildComment(ItuClass b, ItuComment comment, int level, MethodType methodType)
        {
            return $"/* {comment.Comment} */\r\n";
        }

        private string BuildBlock(ItuClass b, ItuBlock parent, ItuBlock block, int level, MethodType methodType)
        {
            string spacing = GetSpacing(level);
            string ret = "";

            string condition = block.Condition;
            string blockType = block.Type;

            if (!string.IsNullOrEmpty(condition))
            {
                if(blockType == "if" || blockType == "else if" || blockType == "while" || blockType == "do")
                {
                    condition = FixCondition(b, condition, methodType);
                }
            }

            if (!string.IsNullOrEmpty(condition))
            {
                condition = condition.Replace("<<", "<< (int)");

                condition = FixMissingFields(b, condition);

                int nestedLevel = GetLoopNestingLevel(block);
                condition = FixNestedInLoopVariables(block, condition, "", ";");
                condition = FixNestedInLoopVariables(block, condition, "", "=");
                condition = FixNestedInLoopVariables(block, condition, "", " ");
                condition = FixNestedInLoopVariables(block, condition, "", ")");
                condition = FixNestedInLoopVariables(block, condition, "", ",");
            }

            if (methodType == MethodType.Read)
            {
                if (block.RequiresAllocation.Count > 0)
                {
                    if (block.Type == "for")
                    {
                        string[] parts = condition.Substring(1, condition.Length - 2).Split(';');

                        var conditionChars = new char[] { '<', '=', '>', '!' };
                        int variableIndex = parts[1].IndexOfAny(conditionChars);
                        string variable = parts[1].Substring(variableIndex).TrimStart(conditionChars);

                        if (!string.IsNullOrWhiteSpace(variable))
                        {
                            foreach (var req in block.RequiresAllocation)
                            {
                                string suffix;
                                int blockSuffixLevel = GetNestedInLoopSuffix(block, "", out suffix);
                                int fieldSuffixLevel = GetNestedInLoopSuffix(req, "", out _);

                                string appendType = "";
                                if (fieldSuffixLevel - blockSuffixLevel > 1)
                                {
                                    int count = fieldSuffixLevel - blockSuffixLevel - 1;

                                    for (int i = 0; i < count; i++)
                                    {
                                        appendType += "[]";
                                    }
                                }

                                string variableType = GetCSharpType(req);
                                int indexesTypeDef = req.FieldArray.Count(x => x == '[');
                                int indexesType = variableType.Count(x => x == '[');
                                string variableName = GetFieldName(req) + suffix;

                                if (variableType.Contains("[]"))
                                {
                                    int diff = (indexesType - indexesTypeDef);
                                    variableType = variableType.Replace("[]", "");
                                    variableType = $"{variableType}[{variable}]";
                                    for (int i = 0; i < diff; i++)
                                    {
                                        variableType += "[]";
                                    }
                                }
                                else
                                {
                                    variableType = variableType + $"[{variable}]";
                                }

                                if(variableType.Contains("_minus1[ c ]"))
                                    variableType = variableType.Replace("_minus1[ c ]", "_minus1[ c ] + 1");
                                else if(variableType.Contains("_minus1[ i ][ j ]"))
                                    variableType = variableType.Replace("_minus1[ i ][ j ]", "_minus1[ i ][ j ] + 1");
                                else if(variableType.Contains("_minus1[ i ]"))
                                    variableType = variableType.Replace("_minus1[ i ]", "_minus1[ i ] + 1");
                                else if (variableType.Contains("_minus1"))
                                    variableType = variableType.Replace("_minus1", "_minus1 + 1");
                                else if (variableType.Contains("Minus1[ i ]"))
                                    variableType = variableType.Replace("Minus1[ i ]", "Minus1[ i ] + 1");
                                else if (variableType.Contains("Minus1(context)"))
                                    variableType = variableType.Replace("Minus1(context)", "Minus1(context) + 1");
                                else if (variableType.Contains("Minus1"))
                                    variableType = variableType.Replace("Minus1", "Minus1 + 1");

                                if (variableName == "ar_label")
                                {
                                    appendType += "[]"; // TODO fix this workaround
                                }

                                ret += $"\r\n{spacing}this.{variableName} = new {variableType}{appendType};";
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("allocation for block other than for");
                    }
                }
            }

            // TODO: for now, this makes sure we have no collisions in case 2 do/while loops modify the same variable
            //if (blockType == "do" || blockType == "while")
            //    ret += $"\r\n{spacing}whileIndex = -1;\r\n";

            if (block.Type == "do")
                ret += $"\r\n{spacing}{blockType}\r\n{spacing}{{";
            else
                ret += $"\r\n{spacing}{blockType} {condition}\r\n{spacing}{{";

            if (blockType == "do" || blockType == "while")
                ret += $"{spacing}whileIndex++;\r\n";

            foreach (var field in block.Content)
            {
                ret += "\r\n" + BuildMethod(b, block, field, level + 1, methodType);
            }

            ret += $"\r\n{spacing}}}";

            if(block.Type == "do")
                ret += $"while {condition};";

            return ret;
        }

        private string FixCondition(ItuClass b, string condition, MethodType methodType)
        {
            string[] parts = condition.Substring(1, condition.Length - 2).Split(new string[] { "||", "&&" }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].StartsWith('!'))
                {
                    string trimmed = parts[i].Trim(new char[] { ' ', '(', ')' });
                    if (!trimmed.Contains("(") && !parts[i].Contains("()")) // if (more_rbsp_data())
                    {
                        // we don't have bool anymore, so in this case it's easy fix
                        condition = condition.Replace(parts[i].TrimEnd(')'), parts[i].TrimEnd(')').Substring(1, parts[i].TrimEnd(')').Length - 1) + "== 0");
                    }
                }
                else if (!parts[i].Contains('=') && !parts[i].Contains('>') && !parts[i].Contains('<'))
                {
                    string trimmed = parts[i].Trim(new char[] { ' ', '(', ')' });
                    if (!trimmed.Contains("(") && !parts[i].Contains("()")) // if (more_rbsp_data())
                    {
                        condition = condition.Replace(trimmed, trimmed + " != 0");
                    }
                }
            }

            condition = specificGenerator.FixCondition(condition, methodType);

            return condition;
        }

        private string FixMissingFields(ItuClass b, string condition)
        {
            Regex r = new Regex("\\b[a-zA-Z_][\\w_]+");
            var matches = r.Matches(condition).Select(x => x.Value).Distinct().ToArray();
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

        public string FixNestedInLoopVariables(ItuCode code, string condition, string prefix = "", string suffix = "")
        {
            if (string.IsNullOrEmpty(condition))
                return condition;

            List<string> ret = new List<string>();
            ItuBlock parent = null;
            var field = code as ItuField;
            if (field != null)
                parent = field.Parent;
            var block = code as ItuBlock;
            if (block != null)
                parent = block.Parent;

            while (parent != null)
            {
                if (parent.Type == "for")
                {
                    if (parent.Condition.Contains("i =") || parent.Condition.Contains("i=") || parent.Condition.Contains("i =") || parent.Condition.Contains("i++"))
                        ret.Insert(0, "[i]");
                    else if (parent.Condition.Contains("j =") || parent.Condition.Contains("j=") || parent.Condition.Contains("j ="))
                        ret.Insert(0, "[j]");
                    else if (parent.Condition.Contains("k =") || parent.Condition.Contains("k=") || parent.Condition.Contains("k ="))
                        ret.Insert(0, "[k]");
                    else if (parent.Condition.Contains("n =") || parent.Condition.Contains("n=") || parent.Condition.Contains("n ="))
                        ret.Insert(0, "[n]");
                    else if (parent.Condition.Contains("c =") || parent.Condition.Contains("c=") || parent.Condition.Contains("c ="))
                        ret.Insert(0, "[c]");
                    else if (parent.Condition.Contains("cx =") || parent.Condition.Contains("cx=") || parent.Condition.Contains("cx ="))
                        ret.Insert(0, "[cx]");
                    else if (parent.Condition.Contains("cy =") || parent.Condition.Contains("cy=") || parent.Condition.Contains("cy ="))
                        ret.Insert(0, "[cy]");
                    else if (parent.Condition.Contains("iGroup =") || parent.Condition.Contains("iGroup=") || parent.Condition.Contains("iGroup ="))
                        ret.Insert(0, "[iGroup]");
                    else if (parent.Condition.Contains("SchedSelIdx =") || parent.Condition.Contains("SchedSelIdx=") || parent.Condition.Contains("SchedSelIdx ="))
                        ret.Insert(0, "[SchedSelIdx]");
                    else if (parent.Condition.Contains("layer =") || parent.Condition.Contains("layer=") || parent.Condition.Contains("layer ="))
                        ret.Insert(0, "[layer]");
                    else if (parent.Condition.Contains("colour_component =") || parent.Condition.Contains("colour_component=") || parent.Condition.Contains("colour_component ="))
                        ret.Insert(0, "[colour_component]");
                    // H265:                   
                    else if (parent.Condition.Contains("comp =") || parent.Condition.Contains("comp=") || parent.Condition.Contains("comp ="))
                        ret.Insert(0, "[comp]");
                    else if (parent.Condition.Contains("sizeId =") || parent.Condition.Contains("sizeId=") || parent.Condition.Contains("sizeId ="))
                        ret.Insert(0, "[sizeId]");
                    else if (parent.Condition.Contains("matrixId =") || parent.Condition.Contains("matrixId=") || parent.Condition.Contains("matrixId ="))
                        ret.Insert(0, "[matrixId]");
                    else if (parent.Condition.Contains("cIdx =") || parent.Condition.Contains("cIdx=") || parent.Condition.Contains("cIdx ="))
                        ret.Insert(0, "[cIdx]");
                    else if (parent.Condition.Contains("h =") || parent.Condition.Contains("h=") || parent.Condition.Contains("h ="))
                        ret.Insert(0, "[h]");
                    else if (parent.Condition.Contains("r =") || parent.Condition.Contains("r=") || parent.Condition.Contains("r ="))
                        ret.Insert(0, "[r]");
                    else if (parent.Condition.Contains("t =") || parent.Condition.Contains("t=") || parent.Condition.Contains("t ="))
                        ret.Insert(0, "[t]");
                    else if (parent.Condition.Contains("m =") || parent.Condition.Contains("m=") || parent.Condition.Contains("m ="))
                        ret.Insert(0, "[m]");
                    else if (parent.Condition.Contains("d =") || parent.Condition.Contains("d=") || parent.Condition.Contains("d ="))
                        ret.Insert(0, "[d]");
                    else
                        throw new Exception();
                }
                else if (parent.Type == "do" || parent.Type == "while")
                {
                    ret.Insert(0, "[whileIndex]");
                }

                parent = parent.Parent;
            }

            if (block != null && (block.Type == "do" || block.Type == "while"))
            {
                ret.Insert(0, "[whileIndex]");
            }

            if (field != null)
                parent = field.Parent;
            if (block != null)
                parent = block.Parent;

            int level = 0;
            while (parent != null)
            {
                string str = string.Concat(ret.Skip(level));

                if (parent.Type == "for")
                {
                    level++;
                }
                else if(parent.Type == "do" || parent.Type == "while")
                {
                    level++;
                }

                foreach (var f in parent.Content)
                {
                    if (f is ItuField ff)
                    {
                        if (string.IsNullOrWhiteSpace(ff.Name) || !string.IsNullOrEmpty(ff.Value))
                            continue;
                        if (condition.Contains(prefix + ff.Name + suffix) && !condition.Contains(prefix + ff.Name + "["))
                        {
                            condition = condition.Replace(prefix + ff.Name + suffix, prefix + ff.Name + str + suffix);
                        }
                    }
                    else if (f is ItuBlock bb)
                    {
                        foreach (var fff in bb.Content)
                        {
                            if (fff is ItuField ffff)
                            {
                                if (string.IsNullOrWhiteSpace(ffff.Name) || !string.IsNullOrEmpty(ffff.Value))
                                    continue;
                                if (condition.Contains(prefix + ffff.Name + suffix) && !condition.Contains(prefix + ffff.Name + "["))
                                {
                                    condition = condition.Replace(prefix + ffff.Name + suffix, prefix + ffff.Name + str + suffix);
                                }
                            }
                        }
                    }
                }

                parent = parent.Parent;
            }

            return condition;
        }

        private int GetNestedInLoopSuffix(ItuCode code, string currentSuffix, out string result)
        {
            List<string> ret = new List<string>();
            ItuBlock parent = null;
            var field = code as ItuField;
            if (field != null)
                parent = field.Parent;
            var block = code as ItuBlock;
            if (block != null)
                parent = block.Parent;

            while (parent != null)
            {
                if (parent.Type == "for")
                {
                    ret.Insert(0, $"[{parent.Condition.Substring(1, parent.Condition.Length - 2).Split(';').First().Split('=').First()}]");
                }
                else if(parent.Type == "do" || parent.Type == "while")
                {
                    ret.Insert(0, $"[whileIndex]");
                }
                
                parent = parent.Parent;
            }

            foreach (var suffix in ret.ToArray())
            {
                if (!string.IsNullOrEmpty(currentSuffix) && currentSuffix.Replace(" ", "").Replace("-2", "").Contains(suffix.Replace(" ", "")))
                    ret.Remove(suffix);
            }

            result = string.Concat(ret);
            return ret.Count;
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

        private string GenerateCtor(ItuClass ituClass)
        {
            string[] ctorParameters = GetCtorParameters(ituClass);
            var typeMappings = GetCSharpTypeMapping();
            string[] ctorParameterDefs = ctorParameters.Select(x => $"{(typeMappings.ContainsKey(specificGenerator.GetCtorParameterType(x)) ? typeMappings[specificGenerator.GetCtorParameterType(x)] : "")} {x}").ToArray();
            string ituClassParameters = $"({string.Join(", ", ctorParameterDefs)})";

            string ctorInitializer = string.Join("\r\n", ctorParameters.Select(x => $"\t\t\tthis.{x.ToFirstLower()} = {x};"));

            string resultCode = $@"
         public {ituClass.ClassName.ToPropertyCase()}{ituClassParameters}
         {{ 
{ctorInitializer}
         }}
";
            return resultCode;
        }

        private string[] GetCtorParameters(ItuClass ituClass)
        {
            var parameters = ituClass.ClassParameter.Substring(1, ituClass.ClassParameter.Length - 2).Split(',').Select(x => x.Trim()).ToArray();
            if (parameters.Length == 1 && string.IsNullOrEmpty(parameters[0]))
            {
                return new string[] { };
            }
            return parameters;
        }

        private string GenerateFields(ItuClass ituClass)
        {
            string resultCode = "";
            ituClass.FlattenedFields = FlattenFields(ituClass, ituClass.Fields);

            foreach(var field in ituClass.FlattenedFields)
            {
                resultCode += BuildField(ituClass, field);
            }

            return resultCode;
        }

        private string BuildField(ItuClass ituClass, ItuField field)
        {
            string type = GetCSharpType(field);
            if(ituClass.AddedFields != null && ituClass.AddedFields.FirstOrDefault(x => x.Name == field.Name) != null)
            {
                // NumOutputLayerSets - adding a calculated field as a property
                if (string.IsNullOrEmpty(field.Type))
                    type = GetCSharpTypeMapping()[ituClass.AddedFields.FirstOrDefault(x => x.Name == field.Name).Type];
            }

            string defaultInitializer = specificGenerator.GetFieldDefaultValue(field);
            string initializer = string.IsNullOrEmpty(defaultInitializer) ? "" : $"= {defaultInitializer}";

            if (field.MakeList)
            {
                type = $"Dictionary<int, {type}>";
                initializer = $" = new {type}()";
            }
            else
            {
                int nestingLevel = GetLoopNestingLevel(field);
                if (nestingLevel > 0)
                {
                    nestingLevel = GetNestedInLoopSuffix(field, field.FieldArray, out _);

                    AddRequiresAllocation(field);

                    if (nestingLevel > 0)
                    {
                        // change the type
                        for (int i = 0; i < nestingLevel; i++)
                        {
                            type += "[]";
                            initializer = "";
                        }
                    }
                }
            }

            string propertyName = GetFieldName(field).ToPropertyCase();
            if(propertyName == ituClass.ClassName.ToPropertyCase())
            {
                propertyName = $"_{propertyName}";
            }

            return $"\t\tprivate {type} {field.Name.ToFirstLower()}{initializer};\r\n\t\tpublic {type} {propertyName} {{ get {{ return {field.Name}; }} set {{ {field.Name} = value; }} }}\r\n";
        }

        public void AddRequiresAllocation(ItuField field)
        {
            ItuBlock parent = null;
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

        private string GetFieldName(ItuField field)
        {
            return field.Name;
        }

        private List<ItuField> FlattenFields(ItuClass b, IEnumerable<ItuCode> fields, ItuBlock parent = null)
        {
            Dictionary<string, ItuField> ret = new Dictionary<string, ItuField>();

            if (parent == null)
            {
                // add also ctor params as fields
                string[] ctorParams = GetCtorParameters(b);
                for (int i = 0; i < ctorParams.Length; i++)
                {
                    var f = new ItuField()
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
                if (code is ItuField field)
                {
                    field.Parent = parent; // keep track of parent blocks

                    var p = parent;
                    while (p != null)
                    {
                        if (p.Type == "do" || p.Type == "while")
                        {
                            field.MakeList = true;
                            Debug.WriteLine($"Field {field.Name} is a list");
                        }
                        p = p.Parent;
                    }

                    AddAndResolveDuplicates(b, ret, field);
                }
                else if (code is ItuBlock block)
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
                            if(variable == "NumDepthViews")
                            {
                                // h264
                                var f = new ItuField() { Name = variable, Type = "u(32)" };
                                AddAndResolveDuplicates(b, ret, f);
                                b.AddedFields.Add(f);
                            }
                            else if (b.RequiresDefinition.FirstOrDefault(x => x.Name == variable) == null && b.AddedFields.FirstOrDefault(x => x.Name == variable) == null)
                            {
                                // h265
                                if (variable == "matrixId")
                                {
                                    b.RequiresDefinition.Add(new ItuField() { Name = variable, Type = "i(32)" });
                                }
                                else
                                {
                                    b.RequiresDefinition.Add(new ItuField() { Name = variable, Type = "u(32)" });
                                }
                            }
                        }
                    }

                    if(block.Type == "do" || block.Type == "while")
                    {
                        if (b.RequiresDefinition.FirstOrDefault(x => x.Name == "whileIndex") == null)
                        {
                            var f = new ItuField() { Name = "whileIndex", Type = "i(32)", Value = "= -1" };
                            b.RequiresDefinition.Add(f);
                        }
                    }

                    var blockFields = FlattenFields(b, block.Content, block);
                    foreach (var blockField in blockFields)
                    {
                        AddAndResolveDuplicates(b, ret, blockField);
                    }
                }
                else if (code is ItuBlockIfThenElse blockifThenElse)
                {
                    blockifThenElse.Parent = parent; // keep track of parent blocks
                    ((ItuBlock)blockifThenElse.BlockIf).Parent = parent;
                    if((ItuBlock)blockifThenElse.BlockElse != null) ((ItuBlock)blockifThenElse.BlockElse).Parent = parent;

                    var blockFields = FlattenFields(b, ((ItuBlock)blockifThenElse.BlockIf).Content, (ItuBlock)blockifThenElse.BlockIf);
                    foreach (var blockField in blockFields)
                    {
                        AddAndResolveDuplicates(b, ret, blockField);
                    }

                    foreach (var blockelseif in blockifThenElse.BlockElseIf)
                    {
                        ((ItuBlock)blockelseif).Parent = parent;
                        var blockElseIfFields = FlattenFields(b, ((ItuBlock)blockelseif).Content, (ItuBlock)blockelseif);
                        foreach (var blockField in blockElseIfFields)
                        {
                            AddAndResolveDuplicates(b, ret, blockField);
                        }
                    }

                    if(blockifThenElse.BlockElse != null)
                    {
                        var blockElseFields = FlattenFields(b, ((ItuBlock)blockifThenElse.BlockElse).Content, (ItuBlock)blockifThenElse.BlockElse);
                        foreach (var blockElseField in blockElseFields)
                        {
                            AddAndResolveDuplicates(b, ret, blockElseField);
                        }
                    }
                }
            }

            return ret.Values.ToList();
        }

        private void AddAndResolveDuplicates(ItuClass b, Dictionary<string, ItuField> ret, ItuField field)
        {
            if (string.IsNullOrEmpty(field.Type))
            {
                if (field.Name == "NumOutputLayerSets")
                {
                    if (b.AddedFields.FirstOrDefault(x => x.Name == field.Name) == null)
                    {
                        // h265
                        var f = new ItuField() { Name = field.Name, Type = "u(32)" };
                        b.AddedFields.Add(f);
                    }
                }
                else if (!string.IsNullOrEmpty(field.Increment))
                {
                    if (b.RequiresDefinition.FirstOrDefault(x => x.Name == field.Name) == null && b.AddedFields.FirstOrDefault(x => x.Name == field.Name) == null)
                    {
                        b.RequiresDefinition.Add(new ItuField() { Name = field.Name, Type = "u(32)", FieldArray = field.FieldArray });
                    }
                    return;
                }
                else if (!string.IsNullOrEmpty(field.Value) && b.AddedFields.FirstOrDefault(x => x.Name == field.Name) == null)
                {
                    if (b.RequiresDefinition.FirstOrDefault(x => x.Name == field.Name) == null)
                    {
                        if (field.Name == "ObjectBoundingBoxAvail")
                        {
                            b.RequiresDefinition.Add(new ItuField() { Name = field.Name, Type = "bool", FieldArray = field.FieldArray });
                        }
                        else if (field.Name == "levelVal" || field.Name == "levelCode" || field.Name == "coeffNum" || field.Name == "coeffLevel"
                            // h265
                            || field.Name == "numComps")
                        {
                            b.RequiresDefinition.Add(new ItuField() { Name = field.Name, Type = "i(64)", FieldArray = field.FieldArray });
                        }
                        else
                        {
                            if (field.Value.TrimEnd().EndsWith("-1"))
                                b.RequiresDefinition.Add(new ItuField() { Name = field.Name, Type = "i(32)", FieldArray = field.FieldArray });
                            else
                                b.RequiresDefinition.Add(new ItuField() { Name = field.Name, Type = "u(32)", FieldArray = field.FieldArray });
                        }
                    }
                    else
                    {
                        if (field.Value.TrimEnd().EndsWith("-1"))
                            b.RequiresDefinition.FirstOrDefault(x => x.Name == field.Name).Type = "i(32)";
                    }
                    return;
                }
            }            

            string name = field.Name;
            int index = 0;
            if (!ret.TryAdd(name, field))
            {
                if (string.IsNullOrEmpty(field.Type))
                {
                    if (field.Parameter != ret[name].Parameter) // we have to duplicate these
                    {
                        while (!ret.TryAdd($"{name}{index}", field))
                        {
                            index++;
                        }

                        field.ClassType = field.Name;
                        field.Name = $"{name}{index}";
                    }
                    else if(!name.StartsWith("out") && name != "NumDepthViews" && name != "NumOutputLayerSets")
                    {
                        // just log a warning for now
                        Debug.WriteLine($"--Field {field.Name} already exists in {b.ClassName} class. Type: {field.Type}, Value: {field.Value}");
                    }
                }
            }
        }
    }
}
