using System;
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

    public class CSharpGenerator
    {
        public string GenerateParser(string type, IEnumerable<ItuClass> ituClasses)
        {
            string resultCode =
            $@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

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
            if(ituClass.ClassName == "depth_representation_sei_element")
            {
                ituClass.ClassParameter = "()"; // remove all "out" parameters
            }

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
                if(name == "depth_timing_offset" && string.IsNullOrWhiteSpace(typedef))
                {
                    m = $"this.depth_timing_offset = new DepthTimingOffset[1];\r\n" + m;
                    typedef = "[ 0 ]";
                }
                else if(name == "depth_grid_position" && string.IsNullOrWhiteSpace(typedef))
                {
                    m = $"this.depth_grid_position = new DepthGridPosition[1];\r\n" + m;
                    typedef = "[ 0 ]";
                }

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
                fieldValue = fieldValue.Replace("<<", "<< (int)");

                fieldValue = fieldValue.Replace("Abs(", "Math.Abs(");
                fieldValue = fieldValue.Replace("Min(", "Math.Min(");
                fieldValue = fieldValue.Replace("Max(", "Math.Max(");

                // TODO
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
                    return $"stream.WriteUnsignedIntVariable({ReplaceParameter(ituField.Name)}, ";
                case "i(v)":
                    return $"stream.WriteSignedIntVariable({ReplaceParameter(ituField.Name)}, ";
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
                    return $"stream.ReadUnsignedIntVariable(size, {ReplaceParameter(ituField.Name)}, ";
                case "i(v)":
                    return $"stream.ReadSignedIntVariable(size, {ReplaceParameter(ituField.Name)}, ";
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
                        return $"###value### new {ituField.ClassType.ToPropertyCase()}{FixMissingParameters(b, ituField.Parameter, ituField.ClassType)} ###size### stream.ReadClass<{ituField.ClassType.ToPropertyCase()}>(size, context, ";
                    throw new NotImplementedException();
            }
        }

        private string ReplaceParameter(string parameter)
        {
            switch(parameter)
            {
                case "AllViewsPairedFlag":
                    return "((Func<uint>)(() =>\r\n            {\r\n                uint AllViewsPairedFlag = 1;\r\n                for (int i = 1; i <= ((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetMvcExtension.NumViewsMinus1; i++)\r\n                    AllViewsPairedFlag = (uint)((AllViewsPairedFlag != 0 && ((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetMvcdExtension.DepthViewPresentFlag[i] != 0 && ((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetMvcdExtension.TextureViewPresentFlag[i] != 0) ? 1 : 0);\r\n                return AllViewsPairedFlag;\r\n            })).Invoke()";
                case "cpb_cnt_minus1":
                    return "((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.HrdParameters.CpbCntMinus1";
                case "ChromaArrayType":
                    return "(((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.SeparateColourPlaneFlag == 0 ? ((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.ChromaFormatIdc : 0)";
                case "IdrPicFlag":
                    return "(uint)((((H264Context)context).NalHeader.NalUnitType == 5) ? 1 : 0)";
                case "NalHrdBpPresentFlag":
                    return "((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.NalHrdParametersPresentFlag";
                case "VclHrdBpPresentFlag":
                    return "((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.VclHrdParametersPresentFlag";
                case "initial_cpb_removal_delay_length_minus1":
                    return "((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.HrdParameters.InitialCpbRemovalDelayLengthMinus1";
                case "profile_idc":
                    return "((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.ProfileIdc";
                case "chroma_format_idc":
                    return "((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.ChromaFormatIdc";
                case "CpbDpbDelaysPresentFlag":
                    return "(uint)(((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.NalHrdParametersPresentFlag == 1 || ((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.VclHrdParametersPresentFlag == 1 ? 1 : 0)";
                case "pic_struct_present_flag":
                    return "((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.PicStructPresentFlag";
                case "NumDepthViews":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetMvcdExtension.NumDepthViews";
                case "PicSizeInMapUnits":
                    return "(((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.PicWidthInMbsMinus1 + 1) * (((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.PicHeightInMapUnitsMinus1 + 1)";
                case "time_offset_length":
                    return "((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.HrdParameters.TimeOffsetLength";
                case "frame_mbs_only_flag":
                    return "((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.FrameMbsOnlyFlag";
                case "num_slice_groups_minus1":
                    return "((H264Context)context).PicParameterSetRbsp.NumSliceGroupsMinus1";
                case "num_views_minus1":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetMvcExtension.NumViewsMinus1";
                case "anchor_pic_flag":
                    return "((H264Context)context).NalHeader.NalUnitHeaderMvcExtension.AnchorPicFlag";
                case "ref_dps_id0":
                    return "((H264Context)context).DepthParameterSetRbsp.RefDpsId0";
                case "predWeight0":
                    return "((H264Context)context).DepthParameterSetRbsp.PredWeight0";
                case "deltaFlag":
                    return "0"; 
                case "ref_dps_id1":
                    return "((H264Context)context).DepthParameterSetRbsp.RefDpsId1";
                case "bit_depth_aux_minus8":
                    return "((H264Context)context).SpsExtension.BitDepthAuxMinus8";
                case "cpb_removal_delay_length_minus1":
                    return "((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.HrdParameters.CpbRemovalDelayLengthMinus1";
                case "dpb_output_delay_length_minus1":
                    return "((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.HrdParameters.DpbOutputDelayLengthMinus1";
                case "coded_data_bit_depth":
                    return "((H264Context)context).SeiRbsp.SeiMessage.Last().Value.SeiPayload.ToneMappingInfo.CodedDataBitDepth";
                case "colour_remap_input_bit_depth":
                    return "((H264Context)context).SeiRbsp.SeiMessage.Last().Value.SeiPayload.ColourRemappingInfo.ColourRemapInputBitDepth";
                case "colour_remap_output_bit_depth":
                    return "((H264Context)context).SeiRbsp.SeiMessage.Last().Value.SeiPayload.ColourRemappingInfo.ColourRemapOutputBitDepth";
                case "ar_object_confidence_length_minus1":
                    return "((H264Context)context).SeiRbsp.SeiMessage.Last().Value.SeiPayload.AnnotatedRegions.ArObjectConfidenceLengthMinus1";
                case "expLen":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSet3davcExtension.DepthRanges.ThreeDvAcquisitionElement.ExpLen";
                case "num_anchor_refs_l0":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetMvcExtension.NumAnchorRefsL0";
                case "num_anchor_refs_l1":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetMvcExtension.NumAnchorRefsL1";
                case "num_non_anchor_refs_l0":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetMvcExtension.NumNonAnchorRefsL0";
                case "num_non_anchor_refs_l1":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetMvcExtension.NumNonAnchorRefsL1";
                case "num_init_pic_parameter_set_minus1":
                    return "((H264Context)context).SeiRbsp.SeiMessage.Last().Value.SeiPayload.MvcdViewScalabilityInfo.NumPicParameterSetMinus1"; // looks like there is a typo...
                case "additional_shift_present":
                    return "((H264Context)context).SeiRbsp.SeiMessage.Last().Value.SeiPayload.ThreeDimensionalReferenceDisplaysInfo.AdditionalShiftPresentFlag.Select(x => (uint)x).ToArray()"; // TODO: looks like a typo
                case "texture_view_present_flag":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetMvcdExtension.TextureViewPresentFlag.Select(x => (uint)x).ToArray()";
                case "initial_cpb_removal_delay":
                case "initial_cpb_removal_delay_offset":
                    return "(((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.HrdParameters.InitialCpbRemovalDelayLengthMinus1 + 1)";
                case "alpha_opaque_value":
                case "alpha_transparent_value":
                    return "(this.bit_depth_aux_minus8 + 9)";
                case "slice_group_id":
                    return "(uint)Math.Ceiling(Math.Log2(((H264Context)context).PicParameterSetRbsp.NumSliceGroupsMinus1 + 1))";
                case "cpb_removal_delay":
                    return "(((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.HrdParameters.CpbRemovalDelayLengthMinus1 + 1)";
                case "dpb_output_delay":
                    return "(((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.HrdParameters.DpbOutputDelayLengthMinus1 + 1)";
                case "time_offset":
                    return "((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.HrdParameters.TimeOffsetLength";
                case "start_of_coded_interval":
                case "coded_pivot_value":
                case "target_pivot_value":
                    return "(((((H264Context)context).SeiRbsp.SeiMessage.Last().Value.SeiPayload.ToneMappingInfo.CodedDataBitDepth + 7) >> 3) << 3)";
                case "pre_lut_coded_value":
                case "pre_lut_target_value":
                    return "(((((H264Context)context).SeiRbsp.SeiMessage.Last().Value.SeiPayload.ColourRemappingInfo.ColourRemapInputBitDepth + 7) >> 3) << 3)";
                case "post_lut_coded_value":
                case "post_lut_target_value":
                    return "(((((H264Context)context).SeiRbsp.SeiMessage.Last().Value.SeiPayload.ColourRemappingInfo.ColourRemapOutputBitDepth + 7) >> 3) << 3)";
                case "ar_object_confidence":
                    return "(((H264Context)context).SeiRbsp.SeiMessage.Last().Value.SeiPayload.AnnotatedRegions.ArObjectConfidenceLengthMinus1 + 1)";
                case "da_mantissa":
                    return "(this.da_mantissa_len_minus1 + 1)";
                case "exponent0":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSet3davcExtension.DepthRanges.ThreeDvAcquisitionElement.ExpLen";
                case "mantissa0":
                    return "(((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSet3davcExtension.DepthRanges.ThreeDvAcquisitionElement.MantissaLenMinus1[i] + 1)";
                case "exponent1":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSet3davcExtension.DepthRanges.ThreeDvAcquisitionElement.ExpLen";
                case "mantissa_focal_length_x":
                    return "(exponent_focal_length_x[ i ] == 0) ? (Math.Max( 0, prec_focal_length - 30 )) : (Math.Max( 0, exponent_focal_length_x[ i ] + prec_focal_length - 31))";
                case "mantissa_focal_length_y":
                    return "(exponent_focal_length_y[ i ] == 0) ? (Math.Max( 0, prec_focal_length - 30 )) : (Math.Max( 0, exponent_focal_length_y[ i ] + prec_focal_length - 31))";
                case "mantissa_principal_point_x": 
                    return "(exponent_principal_point_x[ i ] == 0) ? (Math.Max( 0, prec_principal_point - 30 )) : (Math.Max( 0, exponent_principal_point_x[ i ] + prec_principal_point - 31))";
                case "mantissa_principal_point_y": 
                    return "(exponent_principal_point_y[ i ] == 0) ? (Math.Max( 0, prec_principal_point - 30 )) : (Math.Max( 0, exponent_principal_point_y[ i ] + prec_principal_point - 31))";
                case "mantissa_skew_factor": 
                    return "(exponent_skew_factor[ i ] == 0) ? (Math.Max( 0, prec_skew_factor - 30 )) : (Math.Max( 0, exponent_skew_factor[ i ] + prec_skew_factor - 31))";
                case "mantissa_r": 
                    return "(exponent_r[ i ][ j ][ k ]  == 0) ? (Math.Max( 0, prec_rotation_param - 30 )) : (Math.Max( 0, exponent_r[ i ][ j ][ k ] + prec_rotation_param - 31))";
                case "mantissa_t": 
                    return "(exponent_t[ i ][ j ] == 0) ? (Math.Max( 0, prec_translation_param - 30 )) : (Math.Max( 0, exponent_t[ i ][ j ] + prec_translation_param - 31))";
                case "mantissa_ref_baseline": 
                    return "(exponent_ref_baseline[ i ] == 0) ? (Math.Max( 0, prec_ref_baseline - 30 )) : (Math.Max( 0, exponent_ref_baseline[ i ] + prec_ref_baseline - 31))";
                case "mantissa_ref_display_width": 
                    return "(exponent_ref_display_width[ i ] == 0) ? (Math.Max( 0, prec_ref_display_width - 30 )) : (Math.Max( 0, exponent_ref_display_width[ i ] + prec_ref_display_width - 31))";
                case "mantissa_ref_viewing_distance": 
                    return "(exponent_ref_viewing_distance[ i ] == 0) ? (Math.Max( 0, prec_ref_viewing_dist - 30 )) : (Math.Max( 0, exponent_ref_viewing_distance[ i ] + prec_ref_viewing_dist - 31))";
                case "man_gvd_z_near": 
                    return "(man_len_gvd_z_near_minus1[ i ] + 1)";
                case "man_gvd_z_far": 
                    return "(man_len_gvd_z_far_minus1[ i ] + 1)";
                case "man_gvd_focal_length_x": 
                    return "(exp_gvd_focal_length_x[ i ] == 0) ? (Math.Max( 0, prec_gvd_focal_length - 30 )) : (Math.Max( 0, exp_gvd_focal_length_x[ i ] + prec_gvd_focal_length - 31))";
                case "man_gvd_focal_length_y": 
                    return "(exp_gvd_focal_length_y[ i ] == 0) ? (Math.Max( 0, prec_gvd_focal_length - 30 )) : (Math.Max( 0, exp_gvd_focal_length_y[ i ] + prec_gvd_focal_length - 31))";
                case "man_gvd_principal_point_x": 
                    return "(exp_gvd_principal_point_x[ i ] == 0) ? (Math.Max( 0, prec_gvd_principal_point - 30 )) : (Math.Max( 0, exp_gvd_principal_point_x[ i ] + prec_gvd_principal_point - 31))";
                case "man_gvd_principal_point_y": 
                    return "(exp_gvd_principal_point_y[ i ] == 0) ? (Math.Max( 0, prec_gvd_principal_point - 30 )) : (Math.Max( 0, exp_gvd_principal_point_y[ i ] + prec_gvd_principal_point - 31))";
                case "man_gvd_r": 
                    return "(exp_gvd_r[ i ][ j ][ k ] == 0) ? (Math.Max( 0, prec_gvd_rotation_param - 30 )) : (Math.Max( 0,  exp_gvd_r[ i ][ j ][ k ] + prec_gvd_rotation_param - 31))";
                case "man_gvd_t_x": 
                    return "(exp_gvd_t_x[ i ] == 0) ? (Math.Max( 0, prec_gvd_translation_param - 30 )) : (Math.Max( 0,  exp_gvd_t_x[ i ] + prec_gvd_translation_param - 31))";
                case "MbWidthC":
                    return "(((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.ChromaFormatIdc == 0 ? 0 : 16 / ((((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.ChromaFormatIdc == 1 || ((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.ChromaFormatIdc == 2) ? 2 : 1))";
                case "MbHeightC":
                    return "(((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.ChromaFormatIdc == 0 ? 0 : 16 / ((((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.ChromaFormatIdc == 2 || ((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.ChromaFormatIdc == 3) ? 1 : 2))";
                case "SubWidthC":
                    return "((((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.ChromaFormatIdc == 1 || ((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.ChromaFormatIdc == 2) ? 2 : 1)";
                case "SubHeightC":
                    return "((((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.ChromaFormatIdc == 2 || ((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.ChromaFormatIdc == 3) ? 1 : 2)";
                case "separate_colour_plane_flag":
                    return "((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.SeparateColourPlaneFlag";
                case "redundant_pic_cnt_present_flag":
                    return "((H264Context)context).PicParameterSetRbsp.RedundantPicCntPresentFlag";
                case "entropy_coding_mode_flag":
                    return "((H264Context)context).PicParameterSetRbsp.EntropyCodingModeFlag";
                case "svc_extension_flag":
                    return "((H264Context)context).NalHeader.SvcExtensionFlag";
                case "avc_3d_extension_flag":
                    return "((H264Context)context).NalHeader.Avc3dExtensionFlag";
                case "frame_num":
                    return "(((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.Log2MaxFrameNumMinus4 + 4)";
                case "pic_order_cnt_type":
                    return "((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.PicOrderCntType";
                case "pic_order_cnt_lsb":
                    return "(((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.Log2MaxPicOrderCntLsbMinus4 + 4)";
                case "bottom_field_pic_order_in_frame_present_flag":
                    return "((H264Context)context).PicParameterSetRbsp.BottomFieldPicOrderInFramePresentFlag";
                case "delta_pic_order_always_zero_flag":
                    return "((H264Context)context).SeqParameterSetRbsp.SeqParameterSetData.DeltaPicOrderAlwaysZeroFlag";
                case "nal_unit_type":
                    return "((H264Context)context).NalHeader.NalUnitType";
                case "weighted_pred_flag":
                    return "((H264Context)context).PicParameterSetRbsp.WeightedPredFlag";
                case "weighted_bipred_idc":
                    return "((H264Context)context).PicParameterSetRbsp.WeightedBipredIdc";
                case "nal_ref_idc":
                    return "((H264Context)context).NalHeader.NalRefIdc";
                case "deblocking_filter_control_present_flag":
                    return "((H264Context)context).PicParameterSetRbsp.DeblockingFilterControlPresentFlag";
                case "slice_group_map_type":
                    return "((H264Context)context).PicParameterSetRbsp.SliceGroupMapType";
                case "slice_group_change_cycle":
                    return "((H264Context)context).SliceLayerWithoutPartitioningRbsp.SliceHeader.SliceGroupChangeCycle";
                case "slice_type":
                    return "((H264Context)context).SliceLayerWithoutPartitioningRbsp.SliceHeader.SliceType";
                case "num_ref_idx_l0_active_minus1":
                    return "((H264Context)context).SliceLayerWithoutPartitioningRbsp.SliceHeader.NumRefIdxL0ActiveMinus1";
                case "num_ref_idx_l1_active_minus1":
                    return "((H264Context)context).SliceLayerWithoutPartitioningRbsp.SliceHeader.NumRefIdxL1ActiveMinus1";
                case "use_ref_base_pic_flag":
                    return "((H264Context)context).NalHeader.NalUnitHeaderSvcExtension.UseRefBasePicFlag";
                case "idr_flag":
                    return "((H264Context)context).NalHeader.NalUnitHeaderSvcExtension.IdrFlag";
                case "quality_id":
                    return "((H264Context)context).NalHeader.NalUnitHeaderSvcExtension.QualityId";
                case "no_inter_layer_pred_flag":
                    return "((H264Context)context).NalHeader.NalUnitHeaderSvcExtension.NoInterLayerPredFlag";
                case "slice_header_restriction_flag":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetSvcExtension.SliceHeaderRestrictionFlag";
                case "inter_layer_deblocking_filter_control_present_flag":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetSvcExtension.InterLayerDeblockingFilterControlPresentFlag";
                case "extended_spatial_scalability_idc":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetSvcExtension.ExtendedSpatialScalabilityIdc";
                case "adaptive_tcoeff_level_prediction_flag":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSetSvcExtension.AdaptiveTcoeffLevelPredictionFlag";
                case "slice_header_prediction_flag":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSet3davcExtension.SliceHeaderPredictionFlag";
                case "seq_view_synthesis_flag":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSet3davcExtension.SeqViewSynthesisFlag";
                case "three_dv_acquisition_idc":
                    return "((H264Context)context).SubsetSeqParameterSetRbsp.SeqParameterSet3davcExtension.ThreeDvAcquisitionIdc";
                case "DepthFlag":
                    return "( ((H264Context)context).NalHeader.NalUnitType  !=  21 ) ? false : ( ((H264Context)context).NalHeader.Avc3dExtensionFlag != 0 ? ((H264Context)context).NalHeader.NalUnitHeader3davcExtension.DepthFlag : 1 )";
                case "depth_disp_delay_offset_fp":
                    return "(this.offset_len_minus1 + 1)";

                default:
                    throw new NotImplementedException(parameter);
            }
        }

        private string FixMissingParameters(ItuClass b, string parameter, string classType)
        {
            parameter = parameter.Replace("NumDepthViews", ReplaceParameter("NumDepthViews"));

            parameter = parameter.Replace("ScalingList4x4[ i ]", "new uint[6 * 16]");
            parameter = parameter.Replace("UseDefaultScalingMatrix4x4Flag[ i ]", "0");

            parameter = parameter.Replace("ScalingList8x8[ i - 6 ]", "new uint[6 * 64]");
            parameter = parameter.Replace("UseDefaultScalingMatrix8x8Flag[ i - 6 ]", $"0");

            if (classType == "depth_representation_sei_element")
                parameter = "()";

            parameter = parameter.Replace("ZNearSign, ZNearExp, ZNearMantissa, ZNearManLen", "new uint[index, numViews], new uint[index, numViews], new uint[index, numViews], new uint[index, numViews]");
            parameter = parameter.Replace("ZFarSign, ZFarExp, ZFarMantissa, ZFarManLen", "new uint[index, numViews], new uint[index, numViews], new uint[index, numViews], new uint[index, numViews]");

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

            if (blockType == "do" || blockType == "while")
                ret += $"\r\n{spacing}whileIndex = -1;\r\n";

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

            condition = condition.Replace("slice_type == B", "H264FrameTypes.IsB(slice_type)");
            condition = condition.Replace("slice_type == P", "H264FrameTypes.IsP(slice_type)");
            condition = condition.Replace("slice_type == I", "H264FrameTypes.IsI(slice_type)");
            condition = condition.Replace("slice_type != I", "!H264FrameTypes.IsI(slice_type)");
            condition = condition.Replace("slice_type == SP", "H264FrameTypes.IsSP(slice_type)");
            condition = condition.Replace("slice_type == SI", "H264FrameTypes.IsSI(slice_type)");
            condition = condition.Replace("slice_type != SI", "!H264FrameTypes.IsSI(slice_type)");
            condition = condition.Replace("slice_type == EP", "H264FrameTypes.IsP(slice_type)");
            condition = condition.Replace("slice_type == EB", "H264FrameTypes.IsB(slice_type)");
            condition = condition.Replace("slice_type == EI", "H264FrameTypes.IsI(slice_type)");
            condition = condition.Replace("slice_type != EI", "!H264FrameTypes.IsI(slice_type)");

            condition = condition.Replace("Extended_ISO", "H264Constants.Extended_ISO");
            condition = condition.Replace("Extended_SAR", "H264Constants.Extended_SAR");

            condition = condition.Replace("Abs(", "Math.Abs(");
            condition = condition.Replace("Min(", "Math.Min(");
            condition = condition.Replace("Max(", "Math.Max(");
            condition = condition.Replace("byte_aligned()", "stream.ByteAligned()");

            string prefix = methodType == MethodType.Read ? "Read" : "Write";
            condition = condition.Replace("more_rbsp_data()", $"stream.{prefix}MoreRbspData(this)");
            condition = condition.Replace("next_bits(", $"stream.{prefix}NextBits(this, ");

            condition = condition.Replace("more_rbsp_trailing_data()", "stream.MoreRbspTrailingData()");

            return condition;
        }

        private string FixMissingFields(ItuClass b, string condition)
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
                    condition = condition.Replace(match, ReplaceParameter(match.ToString()));
                }
            }

            string[] replacementsValue = new string[]
            {
                "NalHrdBpPresentFlag",
                "VclHrdBpPresentFlag",
                "CpbDpbDelaysPresentFlag",
                "PicSizeInMapUnits",
                "predWeight0",
                "deltaFlag",
                "NumDepthViews",
                "IdrPicFlag",
                "ChromaArrayType",
                "AllViewsPairedFlag", 
                "DepthFlag"
            };

            foreach (var match in replacementsValue)
            {
                if (b.FlattenedFields.FirstOrDefault(x => x.Name == match) == null &&
                    b.AddedFields.FirstOrDefault(x => x.Name == match) == null &&
                    b.RequiresDefinition.FirstOrDefault(x => x.Name == match) == null
                    )
                {
                    condition = condition.Replace(match, ReplaceParameter(match.ToString()));
                }
            }

            condition = condition.Replace("NumClockTS", "this.pic_struct switch\r\n{\r\n0u => 1,\r\n1u => 1,\r\n2u => 1,\r\n3u => 2,\r\n4u => 2,\r\n5u => 3,\r\n6u => 3,\r\n7u => 2,\r\n8u => 3,\r\n_ => throw new NotSupportedException()\r\n}");

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
                if (!string.IsNullOrEmpty(currentSuffix) && currentSuffix.Replace(" ", "").Contains(suffix.Replace(" ", "")))
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
                resultCode += BuildField(field);
            }

            return resultCode;
        }

        private string BuildField(ItuField field)
        {
            string type = GetCSharpType(field);
            string initializer = "";

            if (field.MakeList)
            {
                type = $"Dictionary<int, {type}>";
                initializer = $" = new {type}()";
            }

            int nestingLevel = GetLoopNestingLevel(field);
            if (nestingLevel > 0)
            {
                nestingLevel = GetNestedInLoopSuffix(field, field.FieldArray, out _);

                if (!field.MakeList)
                {
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
            int index = 0;
            if (!ret.TryAdd(name, field))
            {
                if (string.IsNullOrEmpty(field.Type))
                {
                    if (name == "three_dv_acquisition_element") // we have to duplicate these
                    {
                        while (!ret.TryAdd($"{name}{index}", field))
                        {
                            index++;
                        }

                        field.ClassType = field.Name;
                        field.Name = $"{name}{index}";
                    }
                    else if(!name.StartsWith("out") && name != "NumDepthViews")
                    {
                        // just log a warning for now
                        Debug.WriteLine($"--Field {field.Name} already exists in {b.ClassName} class. Type: {field.Type}, Value: {field.Value}");
                    }
                }
            }
        }
    }
}
