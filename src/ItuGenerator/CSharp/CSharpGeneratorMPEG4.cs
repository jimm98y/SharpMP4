using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ItuGenerator.CSharp
{
    /// <summary>
    /// ISO/IEC 14496-2 (MPEG-4 Visual), whose syntax tables are MPEG's, as H.262's are: each element with its number of
    /// bits and a mnemonic, values named as the semantics name them. The tables nest the stream - the visual object
    /// sequence holds the visual objects, a video object layer its planes - where it is read here unit by unit, each from
    /// its start code to the next: what MPEG4.js has is cut into those units here, and what one unit says of the next is
    /// in the hand-written part of MPEG4Context.
    /// </summary>
    public class CSharpGeneratorMPEG4 : ICustomGenerator
    {
        public string ContextClass => "MPEG4Context";

        // The elements of the video object layer the planes of it are read by, which the context keeps
        private static readonly string[] LayerElements =
        {
            "video_object_layer_shape", "sprite_enable", "newpred_enable", "reduced_resolution_vop_enable", "scalability",
            "enhancement_type", "complexity_estimation_disable", "interlaced", "no_of_sprite_warping_points",
            "sprite_brightness_change", "low_latency_sprite_enable",
        };

        // The elements of define_vop_complexity_estimation_header( ) read_vop_complexity_estimation_header( ) is read by
        private static readonly string[] ComplexityElements =
        {
            "estimation_method", "opaque", "transparent", "intra_cae", "inter_cae", "no_update", "upsampling", "intra_blocks", "inter_blocks",
            "inter4v_blocks", "not_coded_blocks", "dct_coefs", "dct_lines", "vlc_symbols", "vlc_bits", "apm", "npm",
            "interpolate_mc_q", "forw_back_mc_q", "halfpel2", "halfpel4", "sadct", "quarterpel",
        };

        // Values named as the semantics name them: visual_object_type's
        // (6.3.2), and Tables 6-11, 6-12, 6-14, 6-16, 6-19 and 6-24
        private static readonly Dictionary<string, Dictionary<string, int>> NamedValues = new Dictionary<string, Dictionary<string, int>>
        {
            { "visual_object_type", new Dictionary<string, int> { { "video ID", 1 }, { "still texture ID", 2 }, { "mesh ID", 3 }, { "FBA ID", 4 }, { "3D mesh ID", 5 } } },
            { "video_object_type_indication", new Dictionary<string, int> { { "Fine Granularity Scalable", 0x12 } } },
            { "fgs_layer_type", new Dictionary<string, int> { { "FGS", 1 }, { "FGST", 2 }, { "FGS_FGST", 3 } } },
            { "aspect_ratio_info", new Dictionary<string, int> { { "extended_PAR", 15 } } },
            { "video_object_layer_shape", new Dictionary<string, int> { { "rectangular", 0 }, { "binary", 1 }, { "binary only", 2 }, { "grayscale", 3 } } },
            { "sprite_enable", new Dictionary<string, int> { { "static", 1 }, { "GMC", 2 } } },
            { "vop_coding_type", new Dictionary<string, int> { { "I", 0 }, { "P", 1 }, { "B", 2 }, { "S", 3 } } },
        };

        public string PreprocessDefinitionsFile(string definitions)
        {
            definitions = definitions.Replace("\r\n", "\n");

            // Typos of the text: of a name, a keyword, a mnemonic, a quote
            definitions = Fix(definitions, "not_8_ bit", "not_8_bit");
            definitions = Fix(definitions, "backward_shape_ height", "backward_shape_height");
            definitions = Fix(definitions, "short_video _end_marker", "short_video_end_marker");
            definitions = Fix(definitions, "iIf (!motion_compensation_complexity_disable)", "if (!motion_compensation_complexity_disable)");
            definitions = definitions.Replace(" blsbf\n", " bslbf\n");
            definitions = Fix(definitions, "shape_complexity_estimation_disable 1\nif (!shape_complexity_estimation_disable) { bslbf",
                "shape_complexity_estimation_disable 1 bslbf\nif (!shape_complexity_estimation_disable) {");
            definitions = Fix(definitions, "\"still texture\nID\"", "\"still texture ID\"");
            definitions = Fix(definitions, "`'binary only`'", "\"binary only\"");
            definitions = Fix(definitions, "\" binary only\"", "\"binary only\"");
            definitions = definitions.Replace("`", "'");

            // The units: each read on its own, from its start code to the next, the user data after a header one of them
            definitions = definitions.Replace("next_start_code()\nwhile ( next_bits()== user_data_start_code){\nuser_data()\n}\n", "next_start_code()\n");

            // The visual object sequence: its start code and profile, to the next start code - the studio profiles' visual
            // objects (6.2.13) not read - and its end code a unit of its own
            definitions = Fix(definitions,
                "VisualObjectSequence() {\ndo {\nvisual_object_sequence_start_code 32 bslbf\nprofile_and_level_indication 8 uimsbf\n" +
                "if (profile_and_level_indication == 11100001-11101000) {\nnext_start_code_studio()\nextension_and_user_data( 0 )\nStudioVisualObject()\n" +
                "} else {\nwhile ( next_bits()== user_data_start_code){\nuser_data()\n}\nVisualObject()\n}\n" +
                "} while ( next_bits() != visual_object_sequence_end_code)\nvisual_object_sequence_end_code 32 bslbf\n}",
                "VisualObjectSequence() {\nvisual_object_sequence_start_code 32 bslbf\nprofile_and_level_indication 8 uimsbf\nnext_start_code()\n}\n\n" +
                "VisualObjectSequenceEnd() {\nvisual_object_sequence_end_code 32 bslbf\nnext_start_code()\n}");

            // The visual object to the end of its header; the video object's start code, a unit of its own; the other objects
            // not read
            definitions = Fix(definitions,
                "if (visual_object_type == \"video ID\") {\nvideo_object_start_code 32 bslbf\nVideoObjectLayer()\n}\n" +
                "else if (visual_object_type == \"still texture ID\") {\nStillTextureObject()\n}\n" +
                "else if (visual_object_type == \"mesh ID\") {\nMeshObject()\n}\n" +
                "else if (visual_object_type == \"FBA ID\") {\nFBAObject()\n}\n" +
                "else if (visual_object_type == \"3D mesh ID\") {\n3D_Mesh_Object()\n}\n" +
                "if (next_bits() != \"0000 0000 0000 0000 0000 0001\")\nnext_start_code()\n}",
                "}\n\nVideoObject() {\nvideo_object_start_code 32 bslbf\nnext_start_code()\n}");

            // The video object layer: its header, to the next start code. Of a short video header, there are no start codes
            // and no video object layer: its planes are read on their own (MPEG4Context)
            definitions = Fix(definitions, "VideoObjectLayer() {\nif(next_bits() == video_object_layer_start_code) {\nshort_video_header = 0\n", "VideoObjectLayer() {\n");
            definitions = Fix(definitions, "} else {\nshort_video_header = 1\ndo {\nvideo_plane_with_short_header()\n} while(next_bits() == short_video_start_marker)\n}\n}\n", "");
            definitions = Fix(definitions,
                "do {\nif (nextbits_bytealigned() == group_of_vop_start_code)\nGroup_of_VideoObjectPlane()\nFGSVideoObjectPlane()\n" +
                "} while((nextbits_bytealigned()==group_of_vop_start_code)||\n(nextbits_bytealigned()==fgs_vop_start_code))",
                "next_start_code()");
            definitions = Fix(definitions,
                "next_start_code()\nif (sprite_enable == \"static\" && !low_latency_sprite_enable)\nVideoObjectPlane()\ndo {\n" +
                "if (next_bits() == group_of_vop_start_code)\nGroup_of_VideoObjectPlane()\nVideoObjectPlane()\n" +
                "if ((preceding_vop_coding_type == \"B\" ||\npreceding_vop_coding_type == \"S\" ||\nvideo_object_layer_shape != \"rectangular\") &&\n" +
                "next_bits() == stuffing_start_code) {\nstuffing_start_code 32 bslbf\nwhile (next_bits() != '0000 0000 0000 0000 0000 0001')\nstuffing_byte 8 bslbf\n}\n" +
                "} while ((next_bits() == group_of_vop_start_code) ||\n(next_bits() == vop_start_code))\n}",
                "next_start_code()\n}\n}\n\nStuffing() {\nstuffing_start_code 32 bslbf\nwhile (next_bits() != '0000 0000 0000 0000 0000 0001')\nstuffing_byte 8 bslbf\n}");

            // The plane: its header, the rest of it - its macroblocks, the video packets of them - taken as it is
            definitions = Fix(definitions, "if (vop_coded == '0') {\nnext_start_code()\nreturn()\n}\n", "if (vop_coded == '0') {\nnext_start_code()\n}\nelse {\n");
            definitions = Fix(definitions, "motion_shape_texture()\nwhile (nextbits_bytealigned() == resync_marker) {\nvideo_packet_header()\nmotion_shape_texture()\n}", "vop_data()");
            definitions = Fix(definitions, "combined_motion_shape_texture()\nwhile (nextbits_bytealigned() == resync_marker) {\nvideo_packet_header()\ncombined_motion_shape_texture()\n}", "vop_data()");
            definitions = Fix(definitions, "ref_select_code 2 uimsbf\ncombined_motion_shape_texture()", "ref_select_code 2 uimsbf\nvop_data()");
            definitions = Fix(definitions, "}\nnext_start_code()\n}\n\nread_vop_complexity_estimation_header() {", "}\n}\n}\n\nread_vop_complexity_estimation_header() {");
            // A plane of a static sprite: its pieces, of macroblocks, not read - such a plane is kept as its bytes (MPEG4Context)
            definitions = Fix(definitions,
                "if(sprite_enable == \"static\") {\nif (sprite_transmit_mode != \"stop\"\n&& low_latency_sprite_enable) {\ndo {\nsprite_transmit_mode 2 uimsbf\n" +
                "if ((sprite_transmit_mode == \"piece\") ||\n(sprite_transmit_mode == \"update\"))\ndecode_sprite_piece()\n" +
                "} while (sprite_transmit_mode != \"stop\" &&\nsprite_transmit_mode != \"pause\")\n}\nnext_start_code()\nreturn()\n}",
                "if(sprite_enable == \"static\") {\nstatic_sprite_vop()\n}");
            definitions = Fix(definitions, "do {\nmodulo_time_base 1 bslbf\n} while (modulo_time_base != '0')", "do {\nmodulo_time_base 1 bslbf\n} while (more_modulo_time_base)");

            // The plane of a short video header (6.2.5.2): its header, its GOBs taken as they are
            definitions = Fix(definitions,
                "do{\npei 1 bslbf\nif (pei == \"1\")\npsupp 8 bslbf\n} while (pei == \"1\")\ngob_number = 0\nfor(i=0; i<num_gobs_in_vop; i++)\ngob_layer()\n" +
                "if(next_bits() == short_video_end_marker)\nshort_video_end_marker 22 uimsbf\nwhile(!bytealigned())\nzero_bit 1 bslbf\n}",
                "do{\npei 1 bslbf\nif (pei_set)\npsupp 8 bslbf\n} while (pei_set)\nvop_data()\n}");

            // Bytes to the next start code - or to the end of the unit: those of the element read in the loop
            definitions = Regex.Replace(definitions, @"while\s*\(\s*next_bits\(\)\s*!=\s*'0000 0000 0000 0000 0000 0001'\s*\)(\s*\{?\s*\n\s*)(\w+)",
                "while ( before_start_code( $2 ) )$1$2");

            // A quantisation matrix (6.3.3): of so many bits a value, to the value of 0 or the 64th
            definitions = Regex.Replace(definitions, @"^(\w+)(\[i\])?\s+(\d+)\*\[2-64\]\s+uimsbf\s*$",
                m => m.Groups[2].Success ? $"{m.Groups[1].Value}( i )" : $"{m.Groups[1].Value}()", RegexOptions.Multiline);

            // The elements of a number of bits of the stream's: their size here
            definitions = Regex.Replace(definitions, @"^(\w+)\s+\d+-\d+\s+(uimsbf|bslbf)\s*$", "$1 u(v)", RegexOptions.Multiline);

            // An element's number of bits and mnemonic: bslbf and uimsbf unsigned, simsbf two's complement
            definitions = Regex.Replace(definitions, @"^(.*?\S)\s+(\d+)\s+(uimsbf|bslbf)\s*$", "$1 u($2)", RegexOptions.Multiline);
            definitions = Regex.Replace(definitions, @"^(.*?\S)\s+(\d+)\s+simsbf\s*$", "$1 i($2)", RegexOptions.Multiline);
            // time_code, whose row gives no mnemonic (6.3.4: hours, minutes, a marker bit, seconds)
            definitions = Regex.Replace(definitions, @"^(\s*\w+)\s+(\d+)\s*$", "$1 u($2)", RegexOptions.Multiline);

            // Values named as the semantics name them, and bits written as a string
            definitions = Regex.Replace(definitions, @"(\w+)\s*(==|!=)\s*""([^""]+)""", m =>
            {
                string element = m.Groups[1].Value;
                string name = m.Groups[3].Value;
                if (NamedValues.TryGetValue(element, out var values) && values.TryGetValue(name, out int value))
                    return $"{element} {m.Groups[2].Value} {value}";
                if (Regex.IsMatch(name, "^[01]+$"))
                    return $"{element} {m.Groups[2].Value} {Convert.ToInt32(name, 2)}";
                throw new InvalidOperationException($"No value of {element} named {name}");
            });
            definitions = Regex.Replace(definitions, @"(\w+)\s*(==|!=)\s*'([01]+)'", m => $"{m.Groups[1].Value} {m.Groups[2].Value} {Convert.ToInt32(m.Groups[3].Value, 2)}");
            definitions = Regex.Replace(definitions, @"vop_coding_type\s*==\s*'S'", "vop_coding_type == 3");

            // The marker bits of a structure, each of a name of its own: written as each was read, of 0 where a stream has it so
            definitions = NumberMarkerBits(definitions);
            return definitions.Replace("\n", "\r\n");
        }

        // Each marker_bit of a structure after its first: marker_bit_2, marker_bit_3, ...
        private static string NumberMarkerBits(string definitions)
        {
            var lines = definitions.Split('\n');
            int count = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (Regex.IsMatch(lines[i], @"^\w+\(.*\)\s*\{\s*$"))
                    count = 0;
                else if (lines[i] == "marker_bit u(1)" && ++count > 1)
                    lines[i] = $"marker_bit_{count} u(1)";
            }
            return string.Join("\n", lines);
        }

        /// <summary>A replacement the text must have the place of: where it has not, the text is not the one this was made for.</summary>
        private static string Fix(string definitions, string text, string replacement)
        {
            if (!definitions.Contains(text))
                throw new InvalidOperationException($"MPEG4.js has no '{text}'");
            return definitions.Replace(text, replacement);
        }

        public string FixCondition(string condition, MethodType methodType)
        {
            bool read = methodType == MethodType.Read;

            // written, as many as were read: of the property, which makes the dictionary of none
            condition = Regex.Replace(condition, @"before_start_code\(\s*(\w+)\s*\)", m =>
                read ? "!ituContext.AtStartCode(stream)" : $"whileIndex + 1 < this.{PropertyOf(m.Groups[1].Value)}.Count");
            condition = Regex.Replace(condition, @"\bmore_modulo_time_base\b",
                read ? "(this.modulo_time_base[whileIndex] == 1 ? 1 : 0)" : "(whileIndex + 1 < this.ModuloTimeBase.Count ? 1 : 0)");
            condition = Regex.Replace(condition, @"\bpei_set\b", "(this.pei[whileIndex] == 1 ? 1 : 0)");

            // What the units before say: the layer's version, the layer the planes are of, its complexity estimation, the
            // plane's type to the structures it reads
            condition = Regex.Replace(condition, @"\bvideo_object_layer_verid\b", "ituContext.VideoObjectLayerVerid");
            condition = Regex.Replace(condition, @"\bvop_coding_type\b", "ituContext.VopCodingType");
            foreach (string element in LayerElements)
                condition = Regex.Replace(condition, $@"\b{element}\b", $"ituContext.VideoObjectLayer.{element.ToPropertyCase()}");
            foreach (string element in ComplexityElements)
                condition = Regex.Replace(condition, $@"(?<!\.)\b{element}\b", $"ituContext.ComplexityEstimation.{element.ToPropertyCase()}");
            // dcecs_not_coded_blocks of a P-VOP, by not_coded, as the text has it
            condition = Regex.Replace(condition, @"(?<![\.\w])not_coded(?!\w)", "ituContext.ComplexityEstimation.NotCodedBlocks");
            condition = Regex.Replace(condition, @"\baux_comp_count\b", "ituContext.AuxCompCount");
            return condition;
        }

        // The property of an element: user_data's, of the structure user_data( ), is _UserData, a member not being named as its class
        private static string PropertyOf(string element) => element == "user_data" ? "_UserData" : element.ToPropertyCase();

        public string GetVariableSize(string parameter)
        {
            switch (parameter)
            {
                // 6.3.3: the bits vop_time_increment_resolution takes, at least 1
                case "fixed_vop_time_increment":
                case "vop_time_increment":
                    return "ituContext.VopTimeIncrementBits";
                // 6.3.5: the bits vop_time_increment takes and 3, at most 15
                case "vop_id":
                case "vop_id_for_prediction":
                    return "ituContext.VopIdBits";
                // 6.3.5: quant_precision, or 5
                case "vop_quant":
                    return "ituContext.QuantBits";
                default:
                    throw new NotImplementedException(parameter);
            }
        }

        public string GetDerivedVariables(string field)
        {
            switch (field)
            {
                // the layer being read, of whose elements its conditions are
                case "video_object_layer_start_code":
                    return "ituContext.OnVideoObjectLayer(this);";
                case "visual_object_start_code":
                    return "ituContext.OnVisualObject(this);";
                case "is_object_layer_identifier":
                    return "ituContext.OnObjectLayerIdentifier(is_object_layer_identifier);";
                case "video_object_layer_verid":
                    return "ituContext.VideoObjectLayerVerid = video_object_layer_verid;";
                case "visual_object_verid":
                    return "ituContext.VisualObjectVerid = visual_object_verid;";
                // the complexity estimation the layer's planes are read by
                case "estimation_method":
                    return "ituContext.ComplexityEstimation = this;";
                case "vop_coding_type":
                    return "ituContext.VopCodingType = vop_coding_type;";
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
