using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AomGenerator.CSharp
{
    /// <summary>
    /// AV1 (AV1 Bitstream &amp; Decoding Process Specification, AOMedia): its constants are those of the
    /// AV1Constants, AV1ObuTypes and like classes, named as the specification names them; its functions are
    /// in the hand-written part of AV1Context.
    /// </summary>
    public class CSharpGeneratorAV1 : CSharpGeneratorAom
    {
        public override IEnumerable<string> StaticUsings => new[]
        {
            "SharpAV1.AV1Constants", "SharpAV1.AV1RefFrames", "SharpAV1.AV1ObuTypes", "SharpAV1.AV1ColorPrimaries",
            "SharpAV1.AV1TransferCharacteristics", "SharpAV1.AV1MatrixCoefficients", "SharpAV1.AV1ChromaSamplePosition",
            "SharpAV1.AV1FrameTypes", "SharpAV1.AV1MetadataType", "SharpAV1.AV1FrameRestorationType",
            "SharpAV1.AV1ScalabilityModeIdc", "SharpAV1.AV1TxModes", "SharpAV1.AV1InterpolationFilter",
        };

        /// <summary>
        /// Lower case variables are their process's (4.8), but for these: set_frame_refs's (7.8), which the
        /// find_* functions of the same process use, and startPosition, where the payload starts, which the
        /// hand-written part reads to find where it ends.
        /// </summary>
        public override ISet<string> KeptVariables => new HashSet<string> { "usedFrame", "shiftedOrderHints", "curFrameHint", "startPosition" };

        /// <summary>Where SharpAV1 reads other than a decoder does, or the specification leaves to the text.</summary>
        protected override string Patch(string definitions)
        {
            // A caller may read every layer, where a decoder drops the OBUs outside the operating point it chose (7.1).
            definitions = definitions.Replace("OperatingPointIdc != 0 && obu_extension_flag == 1", "OperatingPointIdc != 0 && AllLayers == 0 && obu_extension_flag == 1");

            // obu_padding_length is what the OBU has left before its trailing bits (5.9.2), which the syntax
            // does not say how to work out; left 0, the padding was read as trailing bits.
            definitions = Regex.Replace(definitions, @"padding_obu\(\) \{ *(\r?\n)", m => $"padding_obu() {{{m.Groups[1].Value} obu_padding_length = PayloadBytesBeforeTrailingBits(){m.Groups[1].Value}", RegexOptions.None);

            // A hook where the frame header ends, for the reader to act on it there.
            definitions = new Regex(@"(\buncompressed_header\(\))(\s+)(if \( show_existing_frame \) \{)").Replace(
                definitions, m => $"{m.Groups[1].Value}{m.Groups[2].Value}FrameHeaderDone(){m.Groups[2].Value}{m.Groups[3].Value}", 1);

            // A metadata type the syntax does not know - registered later, user private, reserved - is to be
            // ignored (6.7.1): its payload is read as bytes. With no branch for it, the payload was read as
            // trailing bits.
            definitions = new Regex(@"metadata_timecode\(\)(\r?\n)(\s*)\}").Replace(
                definitions, m => $"metadata_timecode(){m.Groups[1].Value} else{m.Groups[1].Value} MetadataUnknownPayload(){m.Groups[1].Value}{m.Groups[2].Value}}}", 1);

            // The T.35 payload runs to the trailing bits; the syntax gives it no length.
            definitions = Regex.Replace(definitions, @"^(\s*)itu_t_t35_payload_bytes\s*$", "$1ItutT35PayloadBytes()", RegexOptions.Multiline);

            // A conforming frame has at most MAX_TILE_COLS and MAX_TILE_ROWS tiles; one that codes a tile a
            // superblock past that is read up to the limit, as ffmpeg reads it, not past the end of the tile lists.
            definitions = definitions.Replace("for ( i = 0; startSb < sbCols; i++ )", "for ( i = 0; startSb < sbCols && i < MAX_TILE_COLS; i++ )");
            definitions = definitions.Replace("for ( i = 0; startSb < sbRows; i++ )", "for ( i = 0; startSb < sbRows && i < MAX_TILE_ROWS; i++ )");

            // A frame no superblock wide - one sized after a reference a broken stream never coded - has no
            // widest tile: taken as 1, the division does not throw.
            definitions = definitions.Replace("maxTileAreaSb / widestTileSb", "maxTileAreaSb / Max( widestTileSb, 1 )");

            return definitions;
        }

        /// <summary>
        /// The elements whose value an encoder chooses from its state, as libaom's does (av1/encoder/bitstream.c),
        /// where the specification's statements do not say how.
        /// </summary>
        public override string GetInverse(AomField element)
        {
            switch (element.Name)
            {
                // 1 at the first reference the frame's sizes are (write_frame_size_with_refs)
                case "found_ref":
                    return "({state}.UpscaledWidth == RefUpscaledWidth[ref_frame_idx[i]] && {state}.FrameHeight == RefFrameHeight[ref_frame_idx[i]]"
                        + " && {state}.RenderWidth == RefRenderWidth[ref_frame_idx[i]] && {state}.RenderHeight == RefRenderHeight[ref_frame_idx[i]] ? 1 : 0)";
                // The tiles' log2, counted up to
                case "increment_tile_cols_log2":
                    return "({state}.TileColsLog2 > TileColsLog2 ? 1 : 0)";
                case "increment_tile_rows_log2":
                    return "({state}.TileRowsLog2 > TileRowsLog2 ? 1 : 0)";
                // Each tile's size, from where the tiles start: the last runs to the frame's last superblock
                case "width_in_sbs_minus_1":
                    return "((i + 1 < {state}.TileCols ? {state}.MiColStarts[i + 1] >> sbShift : sbCols) - ({state}.MiColStarts[i] >> sbShift) - 1)";
                case "height_in_sbs_minus_1":
                    return "((i + 1 < {state}.TileRows ? {state}.MiRowStarts[i + 1] >> sbShift : sbRows) - ({state}.MiRowStarts[i] >> sbShift) - 1)";
                // The distance back to the frame id of each reference, which expectedFrameId must be (6.8.2)
                case "delta_frame_id_minus_1":
                    return "((({state}.current_frame_id + (1 << idLen) - RefFrameId[ref_frame_idx[i]]) % (1 << idLen)) - 1)";
                // The global motion type
                case "is_global":
                    return "({state}.GmType[refc] != IDENTITY ? 1 : 0)";
                case "is_rot_zoom":
                    return "({state}.GmType[refc] == ROTZOOM ? 1 : 0)";
                case "is_translation":
                    return "({state}.GmType[refc] == TRANSLATION ? 1 : 0)";
                // The value kept, clipped as it was read
                case "feature_value":
                    return "{state}.FeatureData[i][j]";
                // The restoration unit's size: a shift of one more with superblocks of 128, else of the
                // shift coded, and its extra bit
                case "lr_unit_shift":
                    return "(use_128x128_superblock != 0 ? {state}.lr_unit_shift - 1 : ({state}.lr_unit_shift > 0 ? 1 : 0))";
                case "lr_unit_extra_shift":
                    return "({state}.lr_unit_shift - 1)";
                default:
                    return null;
            }
        }

        public override string GetFieldType(AomField field)
        {
            // uvlc() reads up to 2^32 - 1 (4.10.3), which an int does not hold
            if (field.Type == "uvlc()")
                return "uint";
            // The tile list's tile data, as it is
            if (field.Type == "f(N)")
                return "byte[]";
            return base.GetFieldType(field);
        }

        public override string GetReadMethod(AomField field)
        {
            if (field.Type == "f(N)")
                return "stream.ReadBytes(N,";
            // Only obu_size's length is kept - it says where the payload starts. Every leb128 used to set
            // it, so a metadata_type after it made the OBU header out by the difference in their lengths.
            if (field.Name == "obu_size")
                return "obu_size_len = (int)stream.ReadLeb128(";
            return base.GetReadMethod(field);
        }
    }
}
