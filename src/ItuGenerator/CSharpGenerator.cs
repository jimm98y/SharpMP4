using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ItuGenerator
{
    public enum MethodType
    {
        Read,
        Write
    }

    public class CSharpGenerator
    {
        public string GenerateParser(string type, IEnumerable<ItuClass> ituClasses)
        {
            string resultCode =
            $@"using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace Sharp{type}
{{
";
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

        private string GenerateClass(ItuClass ituClass)
        {
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
         public int ReadNextBits { get; set; }
";

            resultCode += GenerateCtor(ituClass);

            resultCode += $@"
         public ulong Read(ItuStream stream)
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
         public ulong Write(ItuStream stream)
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
                    resultCode += $"\r\n{type} {v.Name} = 0;";
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
                        else if(v.FieldArray[i] == ',')
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
                if (parent.Type == "for")
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

            m = m.Replace("H264Helpers.GetVariableCount(\"mantissa_focal_length_x\")", "(exponent_focal_length_x[ i ] == 0) ? (Math.Max( 0, prec_focal_length - 30 )) : (Math.Max( 0, exponent_focal_length_x[ i ] + prec_focal_length - 31))");
            m = m.Replace("H264Helpers.GetVariableCount(\"mantissa_focal_length_y\")", "(exponent_focal_length_y[ i ] == 0) ? (Math.Max( 0, prec_focal_length - 30 )) : (Math.Max( 0, exponent_focal_length_y[ i ] + prec_focal_length - 31))");
            m = m.Replace("H264Helpers.GetVariableCount(\"mantissa_principal_point_x\")", "(exponent_principal_point_x[ i ] == 0) ? (Math.Max( 0, prec_principal_point - 30 )) : (Math.Max( 0, exponent_principal_point_x[ i ] + prec_principal_point - 31))");
            m = m.Replace("H264Helpers.GetVariableCount(\"mantissa_principal_point_y\")", "(exponent_principal_point_y[ i ] == 0) ? (Math.Max( 0, prec_principal_point - 30 )) : (Math.Max( 0, exponent_principal_point_y[ i ] + prec_principal_point - 31))");
            m = m.Replace("H264Helpers.GetVariableCount(\"mantissa_skew_factor\")", "(exponent_skew_factor[ i ] == 0) ? (Math.Max( 0, prec_skew_factor - 30 )) : (Math.Max( 0, exponent_skew_factor[ i ] + prec_skew_factor - 31))");
            m = m.Replace("H264Helpers.GetVariableCount(\"mantissa_r\")", "(exponent_r[ i ][ j ][ k ]  == 0) ? (Math.Max( 0, prec_rotation_param - 30 )) : (Math.Max( 0, exponent_r[ i ][ j ][ k ] + prec_rotation_param - 31))");
            m = m.Replace("H264Helpers.GetVariableCount(\"mantissa_t\")", "(exponent_t[ i ][ j ] == 0) ? (Math.Max( 0, prec_translation_param - 30 )) : (Math.Max( 0, exponent_t[ i ][ j ] + prec_translation_param - 31))");
            m = m.Replace("H264Helpers.GetVariableCount(\"mantissa_ref_baseline\")", "(exponent_ref_baseline[ i ] == 0) ? (Math.Max( 0, prec_ref_baseline - 30 )) : (Math.Max( 0, exponent_ref_baseline[ i ] + prec_ref_baseline - 31))");
            m = m.Replace("H264Helpers.GetVariableCount(\"mantissa_ref_display_width\")", "(exponent_ref_display_width[ i ] == 0) ? (Math.Max( 0, prec_ref_display_width - 30 )) : (Math.Max( 0, exponent_ref_display_width[ i ] + prec_ref_display_width - 31))");
            m = m.Replace("H264Helpers.GetVariableCount(\"mantissa_ref_viewing_distance\")", "(exponent_ref_viewing_distance[ i ] == 0) ? (Math.Max( 0, prec_ref_viewing_dist - 30 )) : (Math.Max( 0, exponent_ref_viewing_distance[ i ] + prec_ref_viewing_dist - 31))");
            
            m = m.Replace("H264Helpers.GetVariableCount(\"man_gvd_z_near\")", "(man_len_gvd_z_near_minus1[ i ] + 1)");
            m = m.Replace("H264Helpers.GetVariableCount(\"man_gvd_z_far\")", "(man_len_gvd_z_far_minus1[ i ] + 1)");
            m = m.Replace("H264Helpers.GetVariableCount(\"man_gvd_focal_length_x\")", "(exp_gvd_focal_length_x[ i ] == 0) ? (Math.Max( 0, prec_gvd_focal_length - 30 )) : (Math.Max( 0, exp_gvd_focal_length_x[ i ] + prec_gvd_focal_length - 31))");
            m = m.Replace("H264Helpers.GetVariableCount(\"man_gvd_focal_length_y\")", "(exp_gvd_focal_length_y[ i ] == 0) ? (Math.Max( 0, prec_gvd_focal_length - 30 )) : (Math.Max( 0, exp_gvd_focal_length_y[ i ] + prec_gvd_focal_length - 31))");
            m = m.Replace("H264Helpers.GetVariableCount(\"man_gvd_principal_point_x\")", "(exp_gvd_principal_point_x[ i ] == 0) ? (Math.Max( 0, prec_gvd_principal_point - 30 )) : (Math.Max( 0, exp_gvd_principal_point_x[ i ] + prec_gvd_principal_point - 31))");
            m = m.Replace("H264Helpers.GetVariableCount(\"man_gvd_principal_point_y\")", "(exp_gvd_principal_point_y[ i ] == 0) ? (Math.Max( 0, prec_gvd_principal_point - 30 )) : (Math.Max( 0, exp_gvd_principal_point_y[ i ] + prec_gvd_principal_point - 31))");
            m = m.Replace("H264Helpers.GetVariableCount(\"man_gvd_r\")", "(exp_gvd_r[ i ][ j ][ k ] == 0) ? (Math.Max( 0, prec_gvd_rotation_param - 30 )) : (Math.Max( 0,  exp_gvd_r[ i ][ j ][ k ] + prec_gvd_rotation_param - 31))");
            m = m.Replace("H264Helpers.GetVariableCount(\"man_gvd_t_x\")", "(exp_gvd_t_x[ i ] == 0) ? (Math.Max( 0, prec_gvd_translation_param - 30 )) : (Math.Max( 0,  exp_gvd_t_x[ i ] + prec_gvd_translation_param - 31))");

            string typedef = (field as ItuField).FieldArray;

            string fieldComment = "";
            if (!string.IsNullOrEmpty((field as ItuField)?.Comment))
            {
                fieldComment = "//" + (field as ItuField).Comment;
            }

            string boxSize = "size += ";

            if (GetLoopNestingLevel(field) > 0)
            {
                //string suffix;
                //GetNestedInLoopSuffix(field, typedef, out suffix);
                //typedef += suffix;

                //if (methodType != MethodType.Size)
                //{
                //    m = FixNestedInLoopVariables(field, m, "(", ",");
                //    m = FixNestedInLoopVariables(field, m, ")", ","); // when casting
                //    m = FixNestedInLoopVariables(field, m, "", " ");
                //}
                //else
                //{
                //    m = FixNestedInLoopVariables(field, m, "", " ");
                //}
            }
            if (methodType == MethodType.Read)
                return $"{spacing}{boxSize}{m} out this.{name}{typedef}); {fieldComment}";
            else if (methodType == MethodType.Write)
                return $"{spacing}{boxSize}{m} this.{name}{typedef}); {fieldComment}";
            else
                return $"{spacing}{boxSize}{m}; // {name}";
        }

        private string BuildStatement(ItuClass b, ItuBlock parent, ItuField field, int level, MethodType methodType)
        {
            string fieldValue = field.Value;
            string fieldArray = field.FieldArray;

            if(!string.IsNullOrEmpty(fieldArray))
            {
                fieldArray = fieldArray.Replace("TotalCoeff", "H264Helpers.TotalCoeff");
                fieldArray = FixMissingFields(b, fieldArray);
            }

            if (!string.IsNullOrEmpty(fieldValue))
            {
                string[] ignored = new string[]
                {
                    "i16x16DClevel",
                    "i16x16AClevel",
                    "level4x4",
                    "level8x8"
                };
                foreach (var ignore in ignored)
                {
                    if (fieldValue.Contains(ignore))
                        return "";
                }

                fieldValue = fieldValue.Replace("<<", "<< (int)");
                fieldValue = fieldValue.Replace("more_rbsp_data()", "stream.MoreRbspData() ? (uint)1 : (uint)0");

                fieldValue = fieldValue.Replace("Abs(", "Math.Abs(");
                fieldValue = fieldValue.Replace("Min(", "Math.Min(");
                fieldValue = fieldValue.Replace("Max(", "Math.Max(");

                // TODO
                fieldValue = fieldValue.Replace("NextMbAddress", "H264Helpers.NextMbAddress");
                fieldValue = fieldValue.Replace("RLESkipContext", "H264Helpers.RLESkipContext");
                fieldValue = fieldValue.Replace("MbaffFrameFlag", "H264Helpers.GetMbaffFrameFlag()");
                fieldValue = fieldValue.Replace("mantissaPred + mantissa_diff", "(uint)(mantissaPred + mantissa_diff)");

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
                case "u(128)":
                    return "stream.WriteUnsignedInt(128, ";
                case "i(32)":
                    return "stream.WriteSignedInt(32, ";
                case "u(v)":
                    return $"stream.WriteUnsignedIntVariable(H264Helpers.GetVariableCount(\"{ituField.Name}\"), ";
                case "i(v)":
                    return $"stream.WriteSignedIntVariable(H264Helpers.GetVariableCount(\"{ituField.Name}\"), ";
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
                    return "stream.WriteSignedIntT(";
                case "se(v) | ae(v)":
                    return "stream.WriteSignedIntGolomb(";
                case "te(v) | ae(v)":
                    return "stream.WriteSignedIntGolomb(";
                case "b(8)":
                    return "stream.WriteBits(8, ";
                default:
                    if (ituField.Type == null)
                        return $"stream.WriteClass<{ituField.Name.ToPropertyCase()}>(";
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
                case "u(128)":
                    return "stream.ReadUnsignedInt(size, 128, ";
                case "i(32)":
                    return "stream.ReadSignedInt(size, 32, ";
                case "u(v)":
                    return $"stream.ReadUnsignedIntVariable(size, H264Helpers.GetVariableCount(\"{ituField.Name}\"), ";
                case "i(v)":
                    return $"stream.ReadSignedIntVariable(size, H264Helpers.GetVariableCount(\"{ituField.Name}\"), ";
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
                    return "stream.ReadSignedIntT(size, ";
                case "se(v) | ae(v)":
                    return "stream.ReadSignedIntGolomb(size, ";
                case "te(v) | ae(v)":
                    return "stream.ReadSignedIntGolomb(size, ";
                case "b(8)":
                    return "stream.ReadBits(size, 8, ";
                default:
                    if (ituField.Type == null)
                        return $"stream.ReadClass<{ituField.Name.ToPropertyCase()}>(size, () => new {ituField.Name.ToPropertyCase()}{FixMissingParameters(b, ituField.Parameter)}, ";
                    throw new NotImplementedException();
            }
        }

        private string FixMissingParameters(ItuClass b, string parameter)
        {
            parameter = parameter.Replace("ZNearSign", $"H264Helpers.GetArray2(\"ZNearSign\")");
            parameter = parameter.Replace("ZNearExp", $"H264Helpers.GetArray2(\"ZNearExp\")");
            parameter = parameter.Replace("ZNearMantissa", $"H264Helpers.GetArray2(\"ZNearMantissa\")");
            parameter = parameter.Replace("ZNearManLen", $"H264Helpers.GetArray2(\"ZNearManLen\")");

            parameter = parameter.Replace("ZFarSign", $"H264Helpers.GetArray2(\"ZFarSign\")");
            parameter = parameter.Replace("ZFarExp", $"H264Helpers.GetArray2(\"ZFarExp\")");
            parameter = parameter.Replace("ZFarMantissa", $"H264Helpers.GetArray2(\"ZFarMantissa\")");
            parameter = parameter.Replace("ZFarManLen", $"H264Helpers.GetArray2(\"ZFarManLen\")");

            parameter = parameter.Replace("DMinSign", $"H264Helpers.GetArray2(\"DMinSign\")");
            parameter = parameter.Replace("DMinExp", $"H264Helpers.GetArray2(\"DMinExp\")");
            parameter = parameter.Replace("DMinMantissa", $"H264Helpers.GetArray2(\"DMinMantissa\")");
            parameter = parameter.Replace("DMinManLen", $"H264Helpers.GetArray2(\"DMinManLen\")");

            parameter = parameter.Replace("DMaxSign", $"H264Helpers.GetArray2(\"DMaxSign\")");
            parameter = parameter.Replace("DMaxExp", $"H264Helpers.GetArray2(\"DMaxExp\")");
            parameter = parameter.Replace("DMaxMantissa", $"H264Helpers.GetArray2(\"DMaxMantissa\")");
            parameter = parameter.Replace("DMaxManLen", $"H264Helpers.GetArray2(\"DMaxManLen\")");
            
            parameter = parameter.Replace("NumDepthViews", $"H264Helpers.GetValue(\"NumDepthViews\")");

            parameter = parameter.Replace("ScalingList4x4[ i ]", "new uint[6 * 16]");
            parameter = parameter.Replace("UseDefaultScalingMatrix4x4Flag[ i ]", "0");

            parameter = parameter.Replace("ScalingList8x8[ i - 6 ]", "new uint[6 * 64]");
            parameter = parameter.Replace("UseDefaultScalingMatrix8x8Flag[ i - 6 ]", $"0");

            return parameter;
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
                { "u(10)",                      "uint" },
                { "u(16)",                      "uint" },
                { "u(20)",                      "uint" },
                { "u(24)",                      "uint" },
                { "u(32)",                      "uint" },
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
                { "st(v)",                      "int" },
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
                return field.Name.ToPropertyCase(); // type is a class

            Dictionary<string, string> map = GetCSharpTypeMapping();

            //Debug.WriteLine($"Field type: {field.Type}, opt: array: {field.FieldArray}");

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

        private string GetCtorParameterType(string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter))
                return "";

            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                { "NumBytesInNALunit",             "u(32)" },
                { "scalingLst",                    "u(32)[]" }, // TODO: remove this temporary fix
                { "sizeOfScalingList",             "u(32)" },
                { "useDefaultScalingMatrixFlag",   "u(32)" },
                { "mb_type",                       "ue(v) | ae(v)" },
                { "startIdx",                      "u(32)" },
                { "endIdx",                        "u(32)" },
                { "coeffLevel",                    "u(32)[]" },
                { "maxNumCoeff",                   "u(32)" },
                { "payloadType",                   "u(32)" },
                { "payloadSize",                   "u(32)" },
                { "outSign",                       "u(32)[,]" },
                { "outExp",                        "u(32)[,]" },
                { "outMantissa",                   "u(32)[,]" },
                { "outManLen",                     "u(32)[,]" },
                { "numViews",                      "u(32)" },
                { "predDirection",                 "u(32)" },
                { "index",                         "u(32)" },
                { "expLen",                        "u(32)" },
                { "i16x16DClevel",                 "u(32)[][]" },
                { "i16x16AClevel",                 "u(32)[][]" },
                { "level4x4",                      "u(32)[][]" },
                { "level8x8",                      "u(32)[][]" },
            };

            return map[parameter];
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
                condition = condition
                    .Replace("NumSubMbPart", "H264Helpers.NumSubMbPart")
                    .Replace("TotalCoeff", "H264Helpers.TotalCoeff")
                    .Replace("NumMbPart", "H264Helpers.NumMbPart");

                if(blockType == "if" || blockType == "else if" || blockType == "while" || blockType == "do")
                {
                    condition = FixCondition(b, condition, methodType);
                }
            }

            if (!string.IsNullOrEmpty(condition))
            {
                condition = condition.Replace("<<", "<< (int)");

                condition = FixMissingFields(b, condition);
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

                                // TODO: Fix this workaround
                                if (req.FieldArray.Contains("[ 0 ]") && variableName.Contains("["))
                                {
                                    variableName += "[0]";
                                }

                                if(variableName == "mvd_l1" || variableName == "mvd_l0")
                                {
                                    if(appendType == "[]")
                                        appendType += "[]";
                                }

                                if(variableType.Contains("_minus1[ c ]"))
                                    variableType = variableType.Replace("_minus1[ c ]", "_minus1[ c ] + 1");
                                else if(variableType.Contains("_minus1[ i ][ j ]"))
                                    variableType = variableType.Replace("_minus1[ i ][ j ]", "_minus1[ i ][ j ] + 1");
                                else if(variableType.Contains("_minus1[ i ]"))
                                    variableType = variableType.Replace("_minus1[ i ]", "_minus1[ i ] + 1");
                                else if (variableType.Contains("_minus1\")[ i ]"))
                                    variableType = variableType.Replace("_minus1\")[ i ]", "_minus1\")[ i ] + 1");
                                else if (variableType.Contains("_minus1\")"))
                                    variableType = variableType.Replace("_minus1\")", "_minus1\") + 1");
                                else if (variableType.Contains("_minus1"))
                                    variableType = variableType.Replace("_minus1", "_minus1 + 1");

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

            if (block.Type == "do")
                ret += $"\r\n{spacing}{blockType}\r\n{spacing}{{";
            else
                ret += $"\r\n{spacing}{blockType} {condition}\r\n{spacing}{{";

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

            //Debug.WriteLine($"---Condition parts: {string.Join(" ##### ", parts)}");

            condition = condition.Replace("slice_type  ==  B", "FrameTypes.IsB(slice_type)");
            condition = condition.Replace("slice_type  ==  P", "FrameTypes.IsP(slice_type)");
            condition = condition.Replace("slice_type  ==  I", "FrameTypes.IsI(slice_type)");
            condition = condition.Replace("slice_type  !=  I", "!FrameTypes.IsI(slice_type)");
            condition = condition.Replace("slice_type  ==  SP", "FrameTypes.IsSP(slice_type)");
            condition = condition.Replace("slice_type  ==  SI", "FrameTypes.IsSI(slice_type)");
            condition = condition.Replace("slice_type  !=  SI", "!FrameTypes.IsSI(slice_type)");
            condition = condition.Replace("slice_type  ==  EP", "FrameTypes.IsP(slice_type)");
            condition = condition.Replace("slice_type  ==  EB", "FrameTypes.IsB(slice_type)");
            condition = condition.Replace("slice_type  ==  EI", "FrameTypes.IsI(slice_type)");
            condition = condition.Replace("slice_type  !=  EI", "!FrameTypes.IsI(slice_type)");

            condition = condition.Replace("Extended_ISO", "H264Constants.Extended_ISO");
            condition = condition.Replace("Extended_SAR", "H264Constants.Extended_SAR");

            condition = condition.Replace("TrailingOnes", "H264Helpers.TrailingOnes");

            condition = condition.Replace("Abs(", "Math.Abs(");
            condition = condition.Replace("Min(", "Math.Min(");
            condition = condition.Replace("Max(", "Math.Max(");
            condition = condition.Replace("byte_aligned()", "stream.ByteAligned()");

            string prefix = methodType == MethodType.Read ? "Read" : "Write";
            condition = condition.Replace("more_rbsp_data()", $"stream.{prefix}MoreRbspData(this)");
            condition = condition.Replace("next_bits(", $"stream.{prefix}NextBits(this, ");

            condition = condition.Replace("more_rbsp_trailing_data()", "stream.MoreRbspTrailingData()");

            condition = condition.Replace("MbPartPredMode( mb_type, 0 )  ==  ", "MbTypes.MbPartPredMode( mb_type, 0 )  ==  MbPartPredModes.");
            condition = condition.Replace("MbPartPredMode( mb_type, 0 )  !=  ", "MbTypes.MbPartPredMode( mb_type, 0 )  !=  MbPartPredModes.");
            condition = condition.Replace("MbPartPredMode( mb_type, mbPartIdx )  !=  ", "MbTypes.MbPartPredMode(mb_type, mbPartIdx) != MbPartPredModes.");
            condition = condition.Replace("MbPartPredMode ( mb_type, mbPartIdx )  !=  ", "MbTypes.MbPartPredMode(mb_type, mbPartIdx) != MbPartPredModes.");
            condition = condition.Replace("SubMbPredMode( sub_mb_type[ mbPartIdx ] )  !=  ", "MbTypes.SubMbPredMode(sub_mb_type[mbPartIdx]) != MbPartPredModes.");
            condition = condition.Replace("mb_type  ==  ", "mb_type  ==  MbTypes.");
            condition = condition.Replace("mb_type  !=  ", "mb_type  !=  MbTypes.");
            condition = condition.Replace("sub_mb_type[ mbPartIdx ]  !=  ", "sub_mb_type[ mbPartIdx ]  !=  MbTypes.");

            // TODO
            condition = condition.Replace("InCropWindow", "H264Helpers.InCropWindow");

            return condition;
        }

        private static string FixMissingFields(ItuClass b, string condition)
        {
            Regex r = new Regex("\\b[a-z_][\\w_]+");
            var matches = r.Matches(condition).Select(x => x.Value).Distinct().ToArray();
            foreach (var match in matches)
            {
                if (b.FlattenedFields.FirstOrDefault(x => x.Name == match) == null && 
                    b.AddedFields.FirstOrDefault(x => x.Name == match) == null && 
                    b.RequiresDefinition.FirstOrDefault(x => x.Name == match) == null && 
                    match.Contains("_")
                    )
                {
                    if(condition.Substring(condition.IndexOf(match) + match.Length).Trim().StartsWith("["))
                        condition = condition.Replace(match, $"H264Helpers.GetArray(\"{match}\")");
                    else
                        condition = condition.Replace(match, $"H264Helpers.GetValue(\"{match}\")");
                }
            }

            string[] replacementsValue = new string[]
            {
                "CodedBlockPatternLuma", "CodedBlockPatternChroma", "MbWidthC", "MbHeightC", "SubWidthC", "SubHeightC",
                "NalHrdBpPresentFlag", "VclHrdBpPresentFlag", "CpbDpbDelaysPresentFlag", "NumClockTS", "PicSizeInMapUnits",
                "CurrMbAddr", "leftMbVSSkipped", "upMbVSSkipped", "predWeight0", "deltaFlag", "NumDepthViews", "IdrPicFlag",
                "ChromaArrayType", "AllViewsPairedFlag"
            };

            foreach (var match in replacementsValue)
            {
                if (b.FlattenedFields.FirstOrDefault(x => x.Name == match) == null &&
                    b.AddedFields.FirstOrDefault(x => x.Name == match) == null &&
                    b.RequiresDefinition.FirstOrDefault(x => x.Name == match) == null
                    )
                {
                    condition = condition.Replace(match, $"H264Helpers.GetValue(\"{match}\")");
                }
            }

            string[] replacementsArray = new string[]
            {
                "VspRefL1Flag","VspRefL0Flag"
            };

            foreach (var match in replacementsArray)
            {
                if (b.FlattenedFields.FirstOrDefault(x => x.Name == match) == null &&
                    b.AddedFields.FirstOrDefault(x => x.Name == match) == null &&
                    b.RequiresDefinition.FirstOrDefault(x => x.Name == match) == null
                    )
                {
                    condition = condition.Replace(match, $"H264Helpers.GetArray(\"{match}\")");
                }
            }

            condition = condition.Replace($"!H264Helpers.GetArray(\"VspRefL1Flag\")[ mbPartIdx ] != 0", "H264Helpers.GetArray(\"VspRefL1Flag\")[ mbPartIdx ] == 0");
            condition = condition.Replace($"!H264Helpers.GetArray(\"VspRefL0Flag\")[ mbPartIdx ] != 0", "H264Helpers.GetArray(\"VspRefL0Flag\")[ mbPartIdx ] == 0");
            condition = condition.Replace($"H264Helpers.GetValue(\"CodedBlockPatternChroma\") & 3 != 0", "(H264Helpers.GetValue(\"CodedBlockPatternChroma\") & 3) != 0");
            condition = condition.Replace($"H264Helpers.GetValue(\"CodedBlockPatternChroma\") & 2 != 0", "(H264Helpers.GetValue(\"CodedBlockPatternChroma\") & 2) != 0");
            condition = condition.Replace($"H264Helpers.GetValue(\"CodedBlockPatternLuma\") & ( 1 << (int) i8x8 )", "(H264Helpers.GetValue(\"CodedBlockPatternLuma\") & ( 1 << (int) i8x8 )) != 0");

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
                    if (parent.Condition.Contains("i =") || parent.Condition.Contains("i=") || parent.Condition.Contains("i ="))
                        ret.Insert(0, "[i]");
                    else
                        throw new Exception();
                }

                parent = parent.Parent;
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
                parent = parent.Parent;
            }

            foreach (var suffix in ret.ToArray())
            {
                if (!string.IsNullOrEmpty(currentSuffix) && currentSuffix.Replace(" ", "").Contains(suffix))
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
            string[] ctorParameterDefs = ctorParameters.Select(x => $"{(typeMappings.ContainsKey(GetCtorParameterType(x)) ? typeMappings[GetCtorParameterType(x)] : "")} {x}").ToArray();
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
            if(parameters.Length == 1 && string.IsNullOrEmpty(parameters[0]))
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
                resultCode += BuildField(field);
            }

            return resultCode;
        }

        private string BuildField(ItuField field)
        {
            string type = GetCSharpType(field);

            int nestingLevel = GetLoopNestingLevel(field);
            if (nestingLevel > 0)
            {
                AddRequiresAllocation(field);
            }

            string propertyName = GetFieldName(field).ToPropertyCase();
            return $"\t\tprivate {type} {field.Name.ToFirstLower()};\r\n\t\tpublic {type} {propertyName} {{ get {{ return {field.Name}; }} set {{ {field.Name} = value; }} }}\r\n";
        }

        public void AddRequiresAllocation(ItuField field)
        {
            ItuBlock parent = null;
            parent = field.Parent;
            while (parent != null)
            {
                if (parent.Type == "for")
                {
                    if (!string.IsNullOrEmpty(field.FieldArray))
                    {
                        // add the allocation above the first for in the hierarchy
                        parent.RequiresAllocation.Add(field);
                    }
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
                        Type = GetCtorParameterType(ctorParams[i])
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

                    string value = field.Value;
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
                            string variable = part.Substring(0, variableIndex).TrimStart(conditionChars).Trim();
                            if(variable == "NumDepthViews")
                            {
                                var f = new ItuField() { Name = variable, Type = "u(32)" };
                                AddAndResolveDuplicates(b, ret, f);
                                b.AddedFields.Add(f);
                            }
                            else if (b.RequiresDefinition.FirstOrDefault(x => x.Name == variable) == null && b.AddedFields.FirstOrDefault(x => x.Name == variable) == null)
                            {
                                b.RequiresDefinition.Add(new ItuField() { Name = variable, Type = "u(32)" });
                            }
                        }
                    }

                    if(block.Type == "do" || block.Type == "while")
                    {
                        if (block.Condition.Contains("i "))
                        {
                            if (b.RequiresDefinition.FirstOrDefault(x => x.Name == "i") == null)
                            {
                                b.RequiresDefinition.Add(new ItuField() { Name = "i", Type = "u(32)" });
                            }
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

                    var blockFields = FlattenFields(b, ((ItuBlock)blockifThenElse.BlockIf).Content, (ItuBlock)blockifThenElse.BlockIf);
                    foreach (var blockField in blockFields)
                    {
                        AddAndResolveDuplicates(b, ret, blockField);
                    }
                    ((ItuBlock)blockifThenElse.BlockIf).Parent = parent;

                    foreach (var blockelseif in blockifThenElse.BlockElseIf)
                    {
                        var blockElseIfFields = FlattenFields(b, ((ItuBlock)blockelseif).Content, (ItuBlock)blockelseif);
                        foreach (var blockField in blockElseIfFields)
                        {
                            AddAndResolveDuplicates(b, ret, blockField);
                        }
                        ((ItuBlock)blockelseif).Parent = parent;
                    }

                    if(blockifThenElse.BlockElse != null)
                    {
                        var blockElseFields = FlattenFields(b, ((ItuBlock)blockifThenElse.BlockElse).Content, (ItuBlock)blockifThenElse.BlockElse);
                        foreach (var blockElseField in blockElseFields)
                        {
                            AddAndResolveDuplicates(b, ret, blockElseField);
                        }
                        ((ItuBlock)blockifThenElse.BlockElse).Parent = parent;
                    }
                }
            }

            return ret.Values.ToList();
        }

        private void AddAndResolveDuplicates(ItuClass b, Dictionary<string, ItuField> ret, ItuField field)
        {
            if (string.IsNullOrEmpty(field.Type))
            {
                if (!string.IsNullOrEmpty(field.Increment))
                {
                    if (b.RequiresDefinition.FirstOrDefault(x => x.Name == field.Name) == null && b.AddedFields.FirstOrDefault(x => x.Name == field.Name) == null)
                    {
                        b.RequiresDefinition.Add(new ItuField() { Name = field.Name, Type = "u(32)", FieldArray = field.FieldArray });
                    }
                    return;
                }
                else if(!string.IsNullOrEmpty(field.Value) && b.AddedFields.FirstOrDefault(x => x.Name == field.Name) == null)
                {
                    if (b.RequiresDefinition.FirstOrDefault(x => x.Name == field.Name) == null)
                    {
                        if (field.Name == "ObjectBoundingBoxAvail")
                        {
                            b.RequiresDefinition.Add(new ItuField() { Name = field.Name, Type = "bool", FieldArray = field.FieldArray });
                        }
                        else if(field.Name == "levelVal" || field.Name == "levelCode" || field.Name == "coeffNum" || field.Name == "coeffLevel")
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
            //int index = 0;
            if (!ret.TryAdd(name, field))
            {
                //while (!ret.TryAdd($"{name}{index}", field))
                //{
                //    index++;
                //}

                //field.Name = $"{name}{index}";
            }
        }
    }
}
