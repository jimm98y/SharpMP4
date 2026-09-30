using System.Collections.Generic;

namespace AomGenerator.CSharp
{
    /// <summary>
    /// VP9 (VP9 Bitstream &amp; Decoding Process Specification v0.7): its constants are those of VP9Constants, named as
    /// the specification names them; its tables and functions are in the hand-written part of VP9Context. Its compressed
    /// header is coded with the Boolean coder (9.2), which AomStream reads and writes: B(p) and L(n).
    /// </summary>
    public class CSharpGeneratorVP9 : CSharpGeneratorAom
    {
        public override IEnumerable<string> StaticUsings => new[] { "SharpVP9.VP9Constants" };

        /// <summary>
        /// Lower case variables are their structure's, but for the render size, which says how the frame is shown, and the
        /// hand-written part reads.
        /// </summary>
        public override ISet<string> KeptVariables => new HashSet<string> { "renderWidth", "renderHeight" };

        /// <summary>
        /// The probability tables the compressed header updates, which are not followed (see VP9Context), so not kept with
        /// the context: a copy of the state for every frame read copied hundreds of arrays of them. Writing does not read them.
        /// </summary>
        public override ISet<string> UnsavedFields => new HashSet<string>
        {
            "tx_probs_8x8", "tx_probs_16x16", "tx_probs_32x32", "coef_probs", "skip_prob", "inter_mode_probs", "interp_filter_probs",
            "is_inter_prob", "comp_mode_prob", "single_ref_prob", "comp_ref_prob", "y_mode_probs", "partition_probs",
            "mv_joint_probs", "mv_sign_prob", "mv_class_probs", "mv_class0_bit_prob", "mv_bits_prob", "mv_class0_fr_probs",
            "mv_fr_probs", "mv_class0_hp_prob", "mv_hp_prob",
        };

        /// <summary>
        /// The elements whose value an encoder chooses from its state, as libvpx does (vp9/encoder/vp9_bitstream.c), where
        /// the statements after them do not say how: 1 at the first reference the frame's size is (write_frame_size_with_refs),
        /// the tiles' log2 counted up to (write_tile_info), and the parts of what one element was read into by two.
        /// </summary>
        public override string GetInverse(AomField element)
        {
            switch (element.Name)
            {
                case "found_ref":
                    return "({state}.FrameWidth == RefFrameWidth[ref_frame_idx[i]] && {state}.FrameHeight == RefFrameHeight[ref_frame_idx[i]] ? 1 : 0)";
                case "increment_tile_cols_log2":
                    return "({state}.tile_cols_log2 > tile_cols_log2 ? 1 : 0)";
                case "tile_rows_log2":
                    return "({state}.tile_rows_log2 > 0 ? 1 : 0)";
                case "increment_tile_rows_log2":
                    return "({state}.tile_rows_log2 > 1 ? 1 : 0)";
                case "tx_mode":
                    return "Min({state}.tx_mode, ALLOW_32X32)";
                case "tx_mode_select":
                    return "({state}.tx_mode == TX_MODE_SELECT ? 1 : 0)";
                case "feature_value":
                    return "Abs({state}.FeatureData[i][j])";
                case "feature_sign":
                    return "({state}.FeatureData[i][j] < 0 ? 1 : 0)";
                default:
                    return null;
            }
        }

        public override string GetReadMethod(AomField field)
        {
            string type = field.Type;
            string argument;
            // B.2.1 gives the frame sizes as f(SzBytes), but they are SzBytes bytes, little-endian, as libvpx and ffmpeg
            // read them (vpx_dsp's mem_get_le16, 24 and 32; vp9_superframe_split's AV_RL)
            if (field.Name == "frame_sizes")
                return $"stream.ReadLe({Expressions.Int(Argument(type, "f"))},";
            if ((argument = Argument(type, "s")) != null)
                return $"stream.ReadSignMagnitude({Expressions.Int(argument)},";
            if ((argument = Argument(type, "B")) != null)
                return $"stream.ReadBool({Expressions.Int(argument)},";
            if ((argument = Argument(type, "L")) != null)
                return $"stream.ReadLiteral({Expressions.Int(argument)},";
            return base.GetReadMethod(field);
        }
    }
}
