using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ItuGenerator.CSharp
{
    /// <summary>
    /// Apple ProRes (SMPTE RDD 36:2022), whose syntax tables are written as the ITU's are, each element with its descriptor.
    /// A frame is read to its slices, which are kept as they are: what the tables derive - the size of a picture of a
    /// field, its macroblocks and slices, the stuffing - and what a header may have past its fields, which its size counts
    /// and decoders skip, are in the hand-written part of ProResContext.
    /// </summary>
    public class CSharpGeneratorProRes : ICustomGenerator
    {
        public string ContextClass => "ProResContext";

        public string PreprocessDefinitionsFile(string definitions)
        {
            definitions = definitions.Replace("\r\n", "\n");

            // A fixed pattern of bits, of a value: as an unsigned number of as many bits
            definitions = Regex.Replace(definitions, @"\bf\((\d+)\)", "u($1)");

            // Which picture of the frame a picture is, as the number the class can take: 0 the first, 1 the second
            definitions = Fix(definitions, "picture(\"first\")", "picture( 0 )");
            definitions = Fix(definitions, "picture(\"second\")", "picture( 1 )");

            // The slices, which are not read: the rest of the picture, taken as it is
            definitions = Fix(definitions,
                "slice_table()\nfor (i = 0; i < height_in_mb; i++)\nfor (j = 0; j < number_of_slices_per_mb_row; j++)\nslice(i, j)\n}",
                "slice_table()\nslice_data()\n}");

            // What a header has past its fields, which its size counts and decoders skip: kept, so it is written as it was
            definitions = Fix(definitions, "chroma_quantization_matrix[v][u] u(8)\n}\n}", "chroma_quantization_matrix[v][u] u(8)\n}\nframe_header_remainder()\n}");
            definitions = Fix(definitions, "log2_desired_slice_size_in_mb u(2)\nreserved u(4)\n}", "log2_desired_slice_size_in_mb u(2)\nreserved u(4)\npicture_header_remainder()\n}");

            definitions = definitions.Replace("slice_table () {", "slice_table() {");

            // The reserved fields of a header, each of a name of its own: written as each was read - Apple's encoder sets
            // some - where one field of them all wrote the last read in every place
            definitions = NumberReservedFields(definitions);
            return definitions.Replace("\n", "\r\n");
        }

        // Each reserved field of a structure after its first: reserved_2, reserved_3, ...
        private static string NumberReservedFields(string definitions)
        {
            var lines = definitions.Split('\n');
            int count = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (Regex.IsMatch(lines[i], @"^\w+\s*\(.*\)\s*\{\s*$"))
                    count = 0;
                else if (Regex.IsMatch(lines[i], @"^reserved u\(\d+\)$") && ++count > 1)
                    lines[i] = lines[i].Replace("reserved ", $"reserved_{count} ");
            }
            return string.Join("\n", lines);
        }

        /// <summary>A replacement the text must have the place of: where it has not, the text is not the one this was made for.</summary>
        private static string Fix(string definitions, string text, string replacement)
        {
            if (!definitions.Contains(text))
                throw new InvalidOperationException($"ProRes.js has no '{text}'");
            return definitions.Replace(text, replacement);
        }

        public string FixCondition(string condition, MethodType methodType)
        {
            // what the frame header said, and what the semantics derive of it (RDD 36 6.1.2, 6.2)
            condition = Regex.Replace(condition, @"\binterlace_mode\b", "ituContext.InterlaceMode");
            condition = Regex.Replace(condition, @"\bstuffing_size\b", "ituContext.StuffingSize");
            condition = Regex.Replace(condition, @"\bheight_in_mb\b", "ituContext.HeightInMb");
            condition = Regex.Replace(condition, @"\bnumber_of_slices_per_mb_row\b", "ituContext.NumberOfSlicesPerMbRow");
            return condition;
        }

        public string GetDerivedVariables(string field)
        {
            switch (field)
            {
                case "frame_size":
                    return "ituContext.OnFrame(frame_size);";
                case "frame_header_size":
                    return "ituContext.FrameHeaderSize = frame_header_size;";
                case "horizontal_size":
                    return "ituContext.HorizontalSize = horizontal_size;";
                case "vertical_size":
                    return "ituContext.VerticalSize = vertical_size;";
                case "interlace_mode":
                    return "ituContext.InterlaceMode = interlace_mode;";
                case "load_chroma_quantization_matrix":
                    return "ituContext.OnQuantizationMatrices(load_luma_quantization_matrix, load_chroma_quantization_matrix);";
                case "picture_header_size":
                    return "ituContext.OnPictureHeader(picture_header_size);";
                case "picture_size":
                    return "ituContext.OnPictureSize(picture_size);";
                case "log2_desired_slice_size_in_mb":
                    return "ituContext.Log2DesiredSliceSizeInMb = log2_desired_slice_size_in_mb;";
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
