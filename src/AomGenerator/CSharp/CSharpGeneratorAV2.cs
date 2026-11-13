// SharpMP4
// Copyright (C) 2026 Lukas Volf
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
// SOFTWARE.

using System.Collections.Generic;

namespace AomGenerator.CSharp
{
    /// <summary>
    /// AV2 (AV2 Bitstream &amp; Decoding Process Specification, AOMedia): its constants, tables and functions
    /// are in the hand-written part of AV2Context, under the specification's names.
    /// </summary>
    public class CSharpGeneratorAV2 : CSharpGeneratorAom
    {
        /// <summary>
        /// What save_sequence_header, save_grain_params and save_grain_model keep: "all the syntax
        /// elements read in" these (6.4.1, 7.23, 6.13); and, as "*", the whole context, which
        /// save_context keeps for an extended layer (7.6).
        /// </summary>
        public override IEnumerable<string> FieldSets => new[] { "sequence_header_obu", "film_grain_model", "film_grain_config", "*" };

        // Tables whose rows the syntax takes whole: their dimensions are all_tables.h's, not the syntax's.
        protected override IDictionary<string, int> DimensionOverrides => new Dictionary<string, int> { { "Pc_Wiener_Sub_Classify2", 3 } };

        /// <summary>
        /// The elements whose value an encoder chooses from its state, as AVM does (av2/encoder/bitstream.c),
        /// where the specification's statements do not say how: 1 at the first reference the frame's size
        /// is (write_frame_size_with_refs), the tiles' log2 counted up to (tile_params, for the sequence
        /// header or the frame), and the offset looked up (ccso's).
        /// </summary>
        public override string GetInverse(AomField element)
        {
            switch (element.Name)
            {
                case "found_ref":
                    return "({state}.FrameWidth == RefFrameWidth[ref_frame_idx[i]] && {state}.FrameHeight == RefFrameHeight[ref_frame_idx[i]] ? 1 : 0)";
                case "increment_tile_cols_log2":
                    return "((obu_type == OBU_SEQUENCE_HEADER ? {state}.SeqTileColsLog2 : {state}.TileColsLog2) > tileColsLog2 ? 1 : 0)";
                case "increment_tile_rows_log2":
                    return "((obu_type == OBU_SEQUENCE_HEADER ? {state}.SeqTileRowsLog2 : {state}.TileRowsLog2) > tileRowsLog2 ? 1 : 0)";
                case "ccso_offset_idx":
                    return "ccso_offset_index({state}.CcsoFilterOffset[plane][band][d0][d1], ccso_scale_idx[plane] + 1)";
                default:
                    return null;
            }
        }
    }
}
