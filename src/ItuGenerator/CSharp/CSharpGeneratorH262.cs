using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ItuGenerator.CSharp
{
    /// <summary>
    /// H.262 | ISO/IEC 13818-2 (MPEG-2 video), whose syntax tables are MPEG's: each element with its number of bits and a
    /// mnemonic, where H.26x's have a descriptor; conditions on what comes next written as its name or its bits. What the
    /// generator reads otherwise is made of them here, and the rest - the stream's units, what they say of each other - is
    /// in the hand-written part of H262Context.
    /// </summary>
    public class CSharpGeneratorH262 : ICustomGenerator
    {
        public string ContextClass => "H262Context";

        // Table 6-2: the extension_start_code_identifier of each extension, by the structure it starts
        private static readonly Dictionary<string, int> ExtensionIds = new Dictionary<string, int>
        {
            { "Sequence Display Extension ID", 2 },
            { "Quant Matrix Extension ID", 3 },
            { "Copyright Extension ID", 4 },
            { "Sequence Scalable Extension ID", 5 },
            { "Picture Display Extension ID", 7 },
            { "Picture Spatial Scalable Extension ID", 9 },
            { "Picture Temporal Scalable Extension ID", 10 },
            { "Camera Parameters Extension ID", 11 },
            { "ITU-T Extension ID", 12 },
        };

        private static readonly Dictionary<string, string> ExtensionStructures = new Dictionary<string, string>
        {
            { "Sequence Display Extension ID", "sequence_display_extension" },
            { "Quant Matrix Extension ID", "quant_matrix_extension" },
            { "Copyright Extension ID", "copyright_extension" },
            { "Sequence Scalable Extension ID", "sequence_scalable_extension" },
            { "Picture Display Extension ID", "picture_display_extension" },
            { "Picture Spatial Scalable Extension ID", "picture_spatial_scalable_extension" },
            { "Picture Temporal Scalable Extension ID", "picture_temporal_scalable_extension" },
            { "Camera Parameters Extension ID", "camera_parameters_extension" },
            { "ITU-T Extension ID", "itu_t_extension" },
        };

        public string PreprocessDefinitionsFile(string definitions)
        {
            definitions = definitions.Replace("\r\n", "\n");

            // Names C# cannot take
            definitions = definitions.Replace("ITU-T_extension", "itu_t_extension").Replace("ITU-T_data", "itu_t_data");

            // The extra bit of 0 that ends the loop of those of 1, of a name of its own: the loop's are kept by their index
            definitions = definitions.Replace("extra_bit_picture/* with the value '0' */", "last_extra_bit_picture /* with the value '0' */")
                                     .Replace("extra_bit_slice /* with the value '0' */", "last_extra_bit_slice /* with the value '0' */");

            // A structure called with a width: content_description_data() of picture_header, its 8 bits with every 9th one
            definitions = Regex.Replace(definitions, @"^(\s*\w+\(\s*\))\s*(/\*.*?\*/)?\s*\d+\s+(uimsbf|bslbf)\s*$", "$1 $2", RegexOptions.Multiline);

            // An array of so many elements of so many bits: 'intra_quantiser_matrix[64] 8*64 uimsbf'
            definitions = Regex.Replace(definitions, @"^(\s*)(\w+)\[(\d+)\]\s*(\d+)\s*\*\s*(\d+)\s+(uimsbf|bslbf)\s*$",
                m => $"{m.Groups[1].Value}for ( i = 0; i < {m.Groups[3].Value}; i++ )\n{m.Groups[1].Value}{m.Groups[2].Value}[ i ] u({m.Groups[4].Value})", RegexOptions.Multiline);

            // An element's number of bits and mnemonic: bslbf and uimsbf - and uimbsf, as slice_picture_id has it - unsigned,
            // simsbf two's complement
            definitions = Regex.Replace(definitions, @"^(.*?\S)\s+(\d+)\s+(uimsbf|bslbf|uimbsf)\s*$", "$1 u($2)", RegexOptions.Multiline);
            definitions = Regex.Replace(definitions, @"^(.*?\S)\s+(\d+)\s+simsbf\s*$", "$1 i($2)", RegexOptions.Multiline);

            // The lower bits of an element split by marker bits, whose row gives no mnemonic: 'camera_position_x_lower 16'
            definitions = Regex.Replace(definitions, @"^(\s*\w+)\s+(\d+)\s*$", "$1 u($2)", RegexOptions.Multiline);

            // The marker bits of a loop, of a name of their own: the loop's are kept by their index, where the structure has
            // others of one
            definitions = Regex.Replace(definitions, @"for \([^)]*\) \{[^{}]*\}", m => m.Value.Replace("\nmarker_bit ", "\nloop_marker_bit "));

            // A condition broken over two lines: 'nextbits()' / '== "Sequence Scalable Extension ID"'
            definitions = Regex.Replace(definitions, @"nextbits\(\)\s*\n\s*==", "nextbits() ==");

            // extension_data( i ): i, which structure the extension follows, as a name the class can take as a property
            definitions = definitions.Replace("extension_data( i ) {", "extension_data( after ) {")
                                     .Replace("if (i == 0) { /* follows sequence_extension() */", "if (after == 0) { /* follows sequence_extension() */")
                                     .Replace("if (i == 2) { /* follows picture_coding_extension() */", "if (after == 2) { /* follows picture_coding_extension() */");

            // Each extension unit is read on its own (H262Context), so extension_data() reads one, not all that follow
            definitions = definitions.Replace("while ( nextbits()== extension_start_code ) {", "if ( one_extension ) {");

            // Which extension follows, by its identifier - written, by which there is
            foreach (var id in ExtensionIds.Keys)
                definitions = definitions.Replace($"nextbits() == \"{id}\"", $"extension_follows( {ExtensionStructures[id]} )")
                                         .Replace($"nextbits()== \"{id}\"", $"extension_follows( {ExtensionStructures[id]} )");

            // Bytes to the next start code - or to the end of the unit: those of the element read in the loop
            definitions = Regex.Replace(definitions, @"while\s*\(\s*nextbits\(\)\s*!=\s*'0000 0000 0000 0000 0000 0001'\s*\)(\s*\{?\s*\n\s*)(\w+)",
                "while ( before_start_code( $2 ) )$1$2");

            // Bits of 1 before more of the header: those of the element read in the loop
            definitions = Regex.Replace(definitions, @"while\s*\(\s*nextbits\(\)\s*==\s*'1'\s*\)(\s*\{?\s*\n\s*)(\w+)",
                "while ( next_bit_one( $2 ) )$1$2");
            definitions = definitions.Replace("if (nextbits() == '1') {\nslice_extension_flag", "if ( next_bit_one( slice_extension_flag ) ) {\nslice_extension_flag");

            // The slice's macroblocks, which are not read: the rest of the unit, taken as it is
            definitions = Regex.Replace(definitions, @"do \{\s*macroblock\(\)\s*\} while \(nextbits\(\) != '000 0000 0000 0000 0000 0000'\)", "slice_data()");

            definitions = definitions.Replace("<sequence_scalable_extension() is present in the bitstream>", "sequence_scalable_extension_present");

            // Values named as the semantics name them
            definitions = definitions
                .Replace("scalable_mode == \"spatial scalability\"", "scalable_mode == 1")
                .Replace("scalable_mode == \"temporal scalability\"", "scalable_mode == 3")
                .Replace("scalable_mode == 'data partitioning'", "scalable_mode == 0")
                .Replace("data_type == \"Padding Bytes\"", "data_type == 1")
                .Replace("data_type == \"Capture Timecode\"", "data_type == 2")
                .Replace("data_type == \"Additional Pan-Scan Parameters\"", "data_type == 3")
                .Replace("data_type == \"Active Region Window\"", "data_type == 4")
                .Replace("data_type == \"Coded Picture Length\"", "data_type == 5")
                .Replace("timecode_type == '11'", "timecode_type == 3")
                .Replace("display_size_present == '1'", "display_size_present == 1");

            return definitions.Replace("\n", "\r\n");
        }

        public string FixCondition(string condition, MethodType methodType)
        {
            bool read = methodType == MethodType.Read;

            condition = condition.Replace("one_extension", "1");
            condition = Regex.Replace(condition, @"extension_follows\(\s*(\w+)\s*\)", m =>
            {
                string structure = m.Groups[1].Value;
                int id = 0;
                foreach (var pair in ExtensionStructures)
                    if (pair.Value == structure)
                        id = ExtensionIds[pair.Key];
                return read ? $"ituContext.NextBits(stream, 4) == {id}" : $"this.{structure} != null";
            });
            // written, as many as were read: of the property, which makes the dictionary of none
            condition = Regex.Replace(condition, @"before_start_code\(\s*(\w+)\s*\)", m =>
                read ? "!ituContext.AtStartCode(stream)" : $"whileIndex + 1 < this.{PropertyOf(m.Groups[1].Value)}.Count");
            condition = Regex.Replace(condition, @"next_bit_one\(\s*(\w+)\s*\)", m =>
                m.Groups[1].Value == "slice_extension_flag"
                    ? (read ? "ituContext.NextBits(stream, 1) == 1" : "this.slice_extension_flag == 1")
                    : (read ? "ituContext.NextBits(stream, 1) == 1" : $"whileIndex + 1 < this.{PropertyOf(m.Groups[1].Value)}.Count"));

            // What other units say
            condition = Regex.Replace(condition, @"\bsequence_scalable_extension_present\b", "(ituContext.SequenceScalableExtension != null ? 1 : 0)");
            // of a slice, the sequence scalable extension's: data partitioning (Table 6-10)
            condition = condition.Replace("scalable_mode == 0", "ituContext.SequenceScalableExtension.ScalableMode == 0");
            condition = Regex.Replace(condition, @"\bvertical_size\b", "ituContext.VerticalSize");
            condition = Regex.Replace(condition, @"\bnumber_of_frame_centre_offsets\b", "ituContext.NumberOfFrameCentreOffsets");
            condition = Regex.Replace(condition, @"\bcounting_type\b", "ituContext.CountingType");
            condition = Regex.Replace(condition, @"\bdata_length\b", "ituContext.DataLength");
            condition = Regex.Replace(condition, @"\bdata_type\b", "ituContext.DataType");
            return condition;
        }

        // The property of an element: user_data's, of the structure user_data( ), is _UserData, a member not being named as its class
        private static string PropertyOf(string element) => element == "user_data" ? "_UserData" : element.ToPropertyCase();

        public string GetDerivedVariables(string field)
        {
            switch (field)
            {
                // what a structure after them in the unit reads, which they are not a part of
                case "counting_type":
                    return "ituContext.CountingType = counting_type;";
                case "data_length":
                    return "ituContext.DataLength = data_length;";
                case "data_type_lower":
                    return "ituContext.DataType = (data_type_upper << 8) | data_type_lower;";
                default:
                    return "";
            }
        }

        public string ReplaceParameter(string parameter) => parameter;

        public string GetCtorParameterType(string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter))
                return "";
            // extension_data( i ): which extensions may follow - 0 after the sequence extension, 2 after the picture coding extension
            return "u(32)";
        }

        public string GetParameterType(string parameter) => string.IsNullOrWhiteSpace(parameter) ? "" : "u(32)";

        public string AppendMethod(ItuCode field, MethodType methodType, string spacing, string retm) => retm;
        // f_code[ s ][ t ] of the picture coding extension: forward and backward, horizontal and vertical
        public string GetFieldDefaultValue(ItuField field) => field.Name == "f_code" ? "new uint[][] { new uint[2], new uint[2] }" : "";
        public void FixClassParameters(ItuClass ituClass) { }
        public string GetVariableSize(string parameter) => throw new NotImplementedException(parameter);
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
