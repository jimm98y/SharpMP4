using System.Collections.Generic;

namespace AomGenerator.CSharp
{
    public class CSharpGeneratorAV1 : ICustomGenerator
    {
        public string AppendMethod(AomCode field, string spacing, string retm)
        {
            switch ((field as AomField).Name)
            {
                case "operating_points_cnt_minus_1":
                        return retm + "operating_point_idc = new int[operating_points_cnt_minus_1 + 1];\r\n" +
                            "\t\t\t\tseq_level_idx = new int[operating_points_cnt_minus_1 + 1];\r\n" +
                            "\t\t\t\tseq_tier = new int[operating_points_cnt_minus_1 + 1];\r\n" +
                            "\t\t\t\tdecoder_model_present_for_this_op = new int[operating_points_cnt_minus_1 + 1];\r\n" +
                            "\t\t\t\tinitial_display_delay_present_for_this_op = new int[operating_points_cnt_minus_1 + 1];\r\n" +
                            "\t\t\t\tinitial_display_delay_minus_1 = new int[operating_points_cnt_minus_1 + 1];\r\n" +
                            "\t\t\t\tbuffer_removal_time = new int[operating_points_cnt_minus_1 + 1];\r\n" +
                            "\t\t\t\tdecoder_buffer_delay = new int[operating_points_cnt_minus_1 + 1];\r\n" +
                            "\t\t\t\tencoder_buffer_delay = new int[operating_points_cnt_minus_1 + 1];\r\n" +
                            "\t\t\t\tlow_delay_mode_flag = new int[operating_points_cnt_minus_1 + 1];\r\n ";

                case "spatial_layers_cnt_minus_1":
                        return retm + "spatial_layer_max_width = new int[spatial_layers_cnt_minus_1 + 1];\r\n" +
                        "\t\t\t\tspatial_layer_max_height = new int[spatial_layers_cnt_minus_1 + 1];\r\n" +
                        "\t\t\t\tspatial_layer_ref_id = new int[spatial_layers_cnt_minus_1 + 1];\r\n ";

                case "temporal_group_size":
                        return retm + "temporal_group_temporal_id = new int[temporal_group_size];\r\n" +
                        "\t\t\t\ttemporal_group_temporal_switching_up_point_flag = new int[temporal_group_size];\r\n" +
                        "\t\t\t\ttemporal_group_spatial_switching_up_point_flag = new int[temporal_group_size];\r\n" +
                        "\t\t\t\ttemporal_group_ref_cnt = new int[temporal_group_size];\r\n ";

                case "temporal_group_ref_cnt":
                        return retm + "temporal_group_ref_pic_diff = new int[temporal_group_size][];\r\n" +
                        "\t\t\t\tfor(int k = 0; k < temporal_group_size; k++) { \r\n" +
                        "\t\t\t\t\t temporal_group_ref_pic_diff[k] = new int[temporal_group_ref_cnt[ i ]]; \r\n" +
                        "\t\t\t\t }\r\n";

                default:
                    return retm;
            }
        }

        public string FixCondition(string value)
        {
            return FixStatement(value);
        }

        public string FixStatement(string value)
        {
            value = value.Replace("Min(", "Math.Min(");
            value = value.Replace("Max(", "Math.Max(");
            value = value.Replace(" >> ", " >> (int)");

            value = value.Replace(" NONE ", " AV1RefFrames.NONE ");
            value = value.Replace("INTRA_FRAME", "AV1RefFrames.INTRA_FRAME");
            value = value.Replace("LAST_FRAME", "AV1RefFrames.LAST_FRAME");
            value = value.Replace("LAST2_FRAME", "AV1RefFrames.LAST2_FRAME");
            value = value.Replace("LAST3_FRAME", "AV1RefFrames.LAST3_FRAME");
            value = value.Replace("GOLDEN_FRAME", "AV1RefFrames.GOLDEN_FRAME");
            value = value.Replace("BWDREF_FRAME", "AV1RefFrames.BWDREF_FRAME");
            value = value.Replace("ALTREF2_FRAME", "AV1RefFrames.ALTREF2_FRAME");
            value = value.Replace("ALTREF_FRAME", "AV1RefFrames.ALTREF_FRAME");
            value = value.Replace("OBU_SEQUENCE_HEADER", "AV1ObuTypes.OBU_SEQUENCE_HEADER");
            value = value.Replace("OBU_TEMPORAL_DELIMITER", "AV1ObuTypes.OBU_TEMPORAL_DELIMITER");
            //value = value.Replace("OBU_FRAME_HEADER", "AV1ObuTypes.OBU_FRAME_HEADER");
            value = value.Replace("OBU_TILE_GROUP", "AV1ObuTypes.OBU_TILE_GROUP");
            value = value.Replace("OBU_METADATA", "AV1ObuTypes.OBU_METADATA");
            value = value.Replace("OBU_FRAME", "AV1ObuTypes.OBU_FRAME");
            value = value.Replace("OBU_REDUNDANT_FRAME_HEADER", "AV1ObuTypes.OBU_REDUNDANT_FRAME_HEADER");
            value = value.Replace("OBU_TILE_LIST", "AV1ObuTypes.OBU_TILE_LIST");
            value = value.Replace("OBU_PADDING", "AV1ObuTypes.OBU_PADDING");
            
            value = value.Replace("get_position", "stream.GetPosition");

            value = value.Replace(" REFS_PER_FRAME", " AV1Constants.REFS_PER_FRAME");
            value = value.Replace("TOTAL_REFS_PER_FRAME", "AV1Constants.TOTAL_REFS_PER_FRAME");
            value = value.Replace("BLOCK_SIZE_GROUPS", "AV1Constants.BLOCK_SIZE_GROUPS");
            value = value.Replace("BLOCK_SIZES", "AV1Constants.BLOCK_SIZES");
            value = value.Replace("BLOCK_INVALID", "AV1Constants.BLOCK_INVALID");
            value = value.Replace("MAX_SB_SIZE", "AV1Constants.MAX_SB_SIZE");
            value = value.Replace("MI_SIZE", "AV1Constants.MI_SIZE");
            value = value.Replace("MI_SIZE_LOG2", "AV1Constants.MI_SIZE_LOG2");
            value = value.Replace("MAX_TILE_WIDTH", "AV1Constants.MAX_TILE_WIDTH");
            value = value.Replace("MAX_TILE_AREA", "AV1Constants.MAX_TILE_AREA");
            value = value.Replace("MAX_TILE_ROWS", "AV1Constants.MAX_TILE_ROWS");
            value = value.Replace("MAX_TILE_COLS", "AV1Constants.MAX_TILE_COLS");
            value = value.Replace("INTRABC_DELAY_PIXELS", "AV1Constants.INTRABC_DELAY_PIXELS");
            value = value.Replace("INTRABC_DELAY_SB64", "AV1Constants.INTRABC_DELAY_SB64");
            value = value.Replace("NUM_REF_FRAMES", "AV1Constants.NUM_REF_FRAMES");
            value = value.Replace("IS_INTER_CONTEXTS", "AV1Constants.IS_INTER_CONTEXTS");
            value = value.Replace("REF_CONTEXTS", "AV1Constants.REF_CONTEXTS");
            value = value.Replace("MAX_SEGMENTS", "AV1Constants.MAX_SEGMENTS");
            value = value.Replace("SEGMENT_ID_CONTEXTS", "AV1Constants.SEGMENT_ID_CONTEXTS");
            value = value.Replace("SEG_LVL_ALT_Q", "AV1Constants.SEG_LVL_ALT_Q");
            value = value.Replace("SEG_LVL_ALT_LF_Y_V", "AV1Constants.SEG_LVL_ALT_LF_Y_V");
            value = value.Replace("SEG_LVL_REF_FRAME", "AV1Constants.SEG_LVL_REF_FRAME");
            value = value.Replace("SEG_LVL_SKIP", "AV1Constants.SEG_LVL_SKIP");
            value = value.Replace("SEG_LVL_GLOBALMV", "AV1Constants.SEG_LVL_GLOBALMV");
            value = value.Replace("SEG_LVL_MAX", "AV1Constants.SEG_LVL_MAX");
            value = value.Replace("TX_SIZE_CONTEXTS", "AV1Constants.TX_SIZE_CONTEXTS");
            value = value.Replace("INTERP_FILTERS", "AV1Constants.INTERP_FILTERS");
            value = value.Replace("INTERP_FILTER_CONTEXTS", "AV1Constants.INTERP_FILTER_CONTEXTS");
            value = value.Replace("SKIP_MODE_CONTEXTS", "AV1Constants.SKIP_MODE_CONTEXTS");
            value = value.Replace("SKIP_CONTEXTS", "AV1Constants.SKIP_CONTEXTS");
            value = value.Replace("PARTITION_CONTEXTS", "AV1Constants.PARTITION_CONTEXTS");
            value = value.Replace("TX_SIZES", "AV1Constants.TX_SIZES");
            value = value.Replace("TX_SIZES_ALL", "AV1Constants.TX_SIZES_ALL");
            value = value.Replace("TX_MODES", "AV1Constants.TX_MODES");
            value = value.Replace("DCT_DCT", "AV1Constants.DCT_DCT");
            value = value.Replace("ADST_DCT", "AV1Constants.ADST_DCT");
            value = value.Replace("DCT_ADST", "AV1Constants.DCT_ADST");
            value = value.Replace("ADST_ADST", "AV1Constants.ADST_ADST");
            value = value.Replace("FLIPADST_DCT", "AV1Constants.FLIPADST_DCT");
            value = value.Replace("DCT_FLIPADST", "AV1Constants.DCT_FLIPADST");
            value = value.Replace("FLIPADST_FLIPADST", "AV1Constants.FLIPADST_FLIPADST");
            value = value.Replace("ADST_FLIPADST", "AV1Constants.ADST_FLIPADST");
            value = value.Replace("FLIPADST_ADST", "AV1Constants.FLIPADST_ADST");
            value = value.Replace("IDTX", "AV1Constants.IDTX");
            value = value.Replace("V_DCT", "AV1Constants.V_DCT");
            value = value.Replace("H_DCT", "AV1Constants.H_DCT");
            value = value.Replace("V_ADST", "AV1Constants.V_ADST");
            value = value.Replace("H_ADST", "AV1Constants.H_ADST");
            value = value.Replace("V_FLIPADST", "AV1Constants.V_FLIPADST");
            value = value.Replace("H_FLIPADST", "AV1Constants.H_FLIPADST");
            value = value.Replace("TX_TYPES", "AV1Constants.TX_TYPES");
            value = value.Replace("MB_MODE_COUNT", "AV1Constants.MB_MODE_COUNT");
            value = value.Replace("INTRA_MODES", "AV1Constants.INTRA_MODES");
            value = value.Replace("UV_INTRA_MODES_CFL_NOT_ALLOWED", "AV1Constants.UV_INTRA_MODES_CFL_NOT_ALLOWED");
            value = value.Replace("UV_INTRA_MODES_CFL_ALLOWED", "AV1Constants.UV_INTRA_MODES_CFL_ALLOWED");
            value = value.Replace("COMPOUND_MODES", "AV1Constants.COMPOUND_MODES");
            value = value.Replace("COMPOUND_MODE_CONTEXTS", "AV1Constants.COMPOUND_MODE_CONTEXTS");
            value = value.Replace("COMP_NEWMV_CTXS", "AV1Constants.COMP_NEWMV_CTXS");
            value = value.Replace("NEW_MV_CONTEXTS", "AV1Constants.NEW_MV_CONTEXTS");
            value = value.Replace("ZERO_MV_CONTEXTS", "AV1Constants.ZERO_MV_CONTEXTS");
            value = value.Replace("REF_MV_CONTEXTS", "AV1Constants.REF_MV_CONTEXTS");
            value = value.Replace("DRL_MODE_CONTEXTS", "AV1Constants.DRL_MODE_CONTEXTS");
            value = value.Replace("MV_CONTEXTS", "AV1Constants.MV_CONTEXTS");
            value = value.Replace("MV_INTRABC_CONTEXT", "AV1Constants.MV_INTRABC_CONTEXT");
            value = value.Replace("MV_JOINTS", "AV1Constants.MV_JOINTS");
            value = value.Replace("MV_CLASSES", "AV1Constants.MV_CLASSES");
            value = value.Replace("CLASS0_SIZE", "AV1Constants.CLASS0_SIZE");
            value = value.Replace("MV_OFFSET_BITS", "AV1Constants.MV_OFFSET_BITS");
            value = value.Replace("MAX_LOOP_FILTER", "AV1Constants.MAX_LOOP_FILTER");
            value = value.Replace("REF_SCALE_SHIFT", "AV1Constants.REF_SCALE_SHIFT");
            value = value.Replace("SUBPEL_BITS", "AV1Constants.SUBPEL_BITS");
            value = value.Replace("SUBPEL_MASK", "AV1Constants.SUBPEL_MASK");
            value = value.Replace("SCALE_SUBPEL_BITS", "AV1Constants.SCALE_SUBPEL_BITS");
            value = value.Replace("MV_BORDER", "AV1Constants.MV_BORDER");
            value = value.Replace("PALETTE_COLOR_CONTEXTS", "AV1Constants.PALETTE_COLOR_CONTEXTS");
            value = value.Replace("PALETTE_MAX_COLOR_CONTEXT_HASH", "AV1Constants.PALETTE_MAX_COLOR_CONTEXT_HASH");
            value = value.Replace("PALETTE_BLOCK_SIZE_CONTEXTS", "AV1Constants.PALETTE_BLOCK_SIZE_CONTEXTS");
            value = value.Replace("PALETTE_Y_MODE_CONTEXTS", "AV1Constants.PALETTE_Y_MODE_CONTEXTS");
            value = value.Replace("PALETTE_UV_MODE_CONTEXTS", "AV1Constants.PALETTE_UV_MODE_CONTEXTS");
            value = value.Replace("PALETTE_SIZES", "AV1Constants.PALETTE_SIZES");
            value = value.Replace("PALETTE_COLORS", "AV1Constants.PALETTE_COLORS");
            value = value.Replace("PALETTE_NUM_NEIGHBORS", "AV1Constants.PALETTE_NUM_NEIGHBORS");
            value = value.Replace("DELTA_Q_SMALL", "AV1Constants.DELTA_Q_SMALL");
            value = value.Replace("DELTA_LF_SMALL", "AV1Constants.DELTA_LF_SMALL");
            value = value.Replace("QM_TOTAL_SIZE", "AV1Constants.QM_TOTAL_SIZE");
            value = value.Replace("MAX_ANGLE_DELTA", "AV1Constants.MAX_ANGLE_DELTA");
            value = value.Replace("DIRECTIONAL_MODES", "AV1Constants.DIRECTIONAL_MODES");
            value = value.Replace("ANGLE_STEP", "AV1Constants.ANGLE_STEP");
            value = value.Replace("TX_SET_TYPES_INTRA", "AV1Constants.TX_SET_TYPES_INTRA");
            value = value.Replace("TX_SET_TYPES_INTER", "AV1Constants.TX_SET_TYPES_INTER");
            value = value.Replace("WARPEDMODEL_PREC_BITS", "AV1Constants.WARPEDMODEL_PREC_BITS");
            value = value.Replace(" IDENTITY", " AV1Constants.IDENTITY");
            value = value.Replace("TRANSLATION", "AV1Constants.TRANSLATION");
            value = value.Replace("ROTZOOM", "AV1Constants.ROTZOOM");
            value = value.Replace("AFFINE", "AV1Constants.AFFINE");
            value = value.Replace("GM_ABS_TRANS_BITS", "AV1Constants.GM_ABS_TRANS_BITS");
            value = value.Replace("GM_ABS_TRANS_ONLY_BITS", "AV1Constants.GM_ABS_TRANS_ONLY_BITS");
            value = value.Replace("GM_ABS_ALPHA_BITS", "AV1Constants.GM_ABS_ALPHA_BITS");
            value = value.Replace("DIV_LUT_PREC_BITS", "AV1Constants.DIV_LUT_PREC_BITS");
            value = value.Replace("DIV_LUT_BITS", "AV1Constants.DIV_LUT_BITS");
            value = value.Replace("DIV_LUT_NUM", "AV1Constants.DIV_LUT_NUM");
            value = value.Replace("MOTION_MODES", "AV1Constants.MOTION_MODES");
            value = value.Replace("SIMPLE", "AV1Constants.SIMPLE");
            value = value.Replace("OBMC", "AV1Constants.OBMC");
            value = value.Replace("LOCALWARP", "AV1Constants.LOCALWARP");
            value = value.Replace("LEAST_SQUARES_SAMPLES_MAX", "AV1Constants.LEAST_SQUARES_SAMPLES_MAX");
            value = value.Replace("LS_MV_MAX", "AV1Constants.LS_MV_MAX");
            value = value.Replace("WARPEDMODEL_TRANS_CLAMP", "AV1Constants.WARPEDMODEL_TRANS_CLAMP");
            value = value.Replace("WARPEDMODEL_NONDIAGAFFINE_CLAMP", "AV1Constants.WARPEDMODEL_NONDIAGAFFINE_CLAMP");
            value = value.Replace("WARPEDPIXEL_PREC_SHIFTS", "AV1Constants.WARPEDPIXEL_PREC_SHIFTS");
            value = value.Replace("WARPEDDIFF_PREC_BITS", "AV1Constants.WARPEDDIFF_PREC_BITS");
            value = value.Replace("GM_ALPHA_PREC_BITS", "AV1Constants.GM_ALPHA_PREC_BITS");
            value = value.Replace("GM_TRANS_PREC_BITS", "AV1Constants.GM_TRANS_PREC_BITS");
            value = value.Replace("GM_TRANS_ONLY_PREC_BITS", "AV1Constants.GM_TRANS_ONLY_PREC_BITS");
            value = value.Replace("INTERINTRA_MODES", "AV1Constants.INTERINTRA_MODES");
            value = value.Replace("MASK_MASTER_SIZE", "AV1Constants.MASK_MASTER_SIZE");
            value = value.Replace("SEGMENT_ID_PREDICTED_CONTEXTS", "AV1Constants.SEGMENT_ID_PREDICTED_CONTEXTS");
            value = value.Replace("FWD_REFS", "AV1Constants.FWD_REFS");
            value = value.Replace("BWD_REFS", "AV1Constants.BWD_REFS");
            value = value.Replace("SINGLE_REFS", "AV1Constants.SINGLE_REFS");
            value = value.Replace("UNIDIR_COMP_REFS", "AV1Constants.UNIDIR_COMP_REFS");
            value = value.Replace("COMPOUND_TYPES", "AV1Constants.COMPOUND_TYPES");
            value = value.Replace("CFL_JOINT_SIGNS", "AV1Constants.CFL_JOINT_SIGNS");
            value = value.Replace("CFL_ALPHABET_SIZE", "AV1Constants.CFL_ALPHABET_SIZE");
            value = value.Replace("COMP_REF_TYPE_CONTEXTS", "AV1Constants.COMP_REF_TYPE_CONTEXTS");
            value = value.Replace("INTRA_MODE_CONTEXTS", "AV1Constants.INTRA_MODE_CONTEXTS");
            value = value.Replace("COMP_GROUP_IDX_CONTEXTS", "AV1Constants.COMP_GROUP_IDX_CONTEXTS");
            value = value.Replace("COMPOUND_IDX_CONTEXTS", "AV1Constants.COMPOUND_IDX_CONTEXTS");
            value = value.Replace("INTRA_EDGE_KERNELS", "AV1Constants.INTRA_EDGE_KERNELS");
            value = value.Replace("INTRA_EDGE_TAPS", "AV1Constants.INTRA_EDGE_TAPS");
            value = value.Replace("FRAME_LF_COUNT", "AV1Constants.FRAME_LF_COUNT");
            value = value.Replace("MAX_VARTX_DEPTH", "AV1Constants.MAX_VARTX_DEPTH");
            value = value.Replace("TXFM_PARTITION_CONTEXTS", "AV1Constants.TXFM_PARTITION_CONTEXTS");
            value = value.Replace("REF_CAT_LEVEL", "AV1Constants.REF_CAT_LEVEL");
            value = value.Replace("MAX_REF_MV_STACK_SIZE", "AV1Constants.MAX_REF_MV_STACK_SIZE");
            value = value.Replace("MFMV_STACK_SIZE", "AV1Constants.MFMV_STACK_SIZE");
            value = value.Replace("MAX_TX_DEPTH", "AV1Constants.MAX_TX_DEPTH");
            value = value.Replace("WEDGE_TYPES", "AV1Constants.WEDGE_TYPES");
            value = value.Replace("FILTER_BITS", "AV1Constants.FILTER_BITS");
            value = value.Replace("WIENER_COEFFS", "AV1Constants.WIENER_COEFFS");
            value = value.Replace("SGRPROJ_PARAMS_BITS", "AV1Constants.SGRPROJ_PARAMS_BITS");
            value = value.Replace("SGRPROJ_PRJ_SUBEXP_K", "AV1Constants.SGRPROJ_PRJ_SUBEXP_K");
            value = value.Replace("SGRPROJ_PRJ_BITS", "AV1Constants.SGRPROJ_PRJ_BITS");
            value = value.Replace("SGRPROJ_RST_BITS", "AV1Constants.SGRPROJ_RST_BITS");
            value = value.Replace("SGRPROJ_MTABLE_BITS", "AV1Constants.SGRPROJ_MTABLE_BITS");
            value = value.Replace("SGRPROJ_RECIP_BITS", "AV1Constants.SGRPROJ_RECIP_BITS");
            value = value.Replace("SGRPROJ_SGR_BITS", "AV1Constants.SGRPROJ_SGR_BITS");
            value = value.Replace("EC_PROB_SHIFT", "AV1Constants.EC_PROB_SHIFT");
            value = value.Replace("EC_MIN_PROB", "AV1Constants.EC_MIN_PROB");
            value = value.Replace("SELECT_SCREEN_CONTENT_TOOLS", "AV1Constants.SELECT_SCREEN_CONTENT_TOOLS");
            value = value.Replace("SELECT_INTEGER_MV", "AV1Constants.SELECT_INTEGER_MV");
            value = value.Replace("RESTORATION_TILESIZE_MAX", "AV1Constants.RESTORATION_TILESIZE_MAX");
            value = value.Replace("MAX_FRAME_DISTANCE", "AV1Constants.MAX_FRAME_DISTANCE");
            value = value.Replace("MAX_OFFSET_WIDTH", "AV1Constants.MAX_OFFSET_WIDTH");
            value = value.Replace("MAX_OFFSET_HEIGHT", "AV1Constants.MAX_OFFSET_HEIGHT");
            value = value.Replace("WARP_PARAM_REDUCE_BITS", "AV1Constants.WARP_PARAM_REDUCE_BITS");
            value = value.Replace("NUM_BASE_LEVELS", "AV1Constants.NUM_BASE_LEVELS");
            value = value.Replace("COEFF_BASE_RANGE", "AV1Constants.COEFF_BASE_RANGE");
            value = value.Replace("BR_CDF_SIZE", "AV1Constants.BR_CDF_SIZE");
            value = value.Replace("SIG_COEF_CONTEXTS_EOB", "AV1Constants.SIG_COEF_CONTEXTS_EOB");
            value = value.Replace("SIG_COEF_CONTEXTS_2D", "AV1Constants.SIG_COEF_CONTEXTS_2D");
            value = value.Replace("SIG_COEF_CONTEXTS", "AV1Constants.SIG_COEF_CONTEXTS");
            value = value.Replace("SIG_REF_DIFF_OFFSET_NUM", "AV1Constants.SIG_REF_DIFF_OFFSET_NUM");
            value = value.Replace("SUPERRES_NUM", "AV1Constants.SUPERRES_NUM");
            value = value.Replace("SUPERRES_DENOM_MIN", "AV1Constants.SUPERRES_DENOM_MIN");
            value = value.Replace("SUPERRES_DENOM_BITS", "AV1Constants.SUPERRES_DENOM_BITS");
            value = value.Replace("SUPERRES_FILTER_BITS", "AV1Constants.SUPERRES_FILTER_BITS");
            value = value.Replace("SUPERRES_FILTER_SHIFTS", "AV1Constants.SUPERRES_FILTER_SHIFTS");
            value = value.Replace("SUPERRES_FILTER_TAPS", "AV1Constants.SUPERRES_FILTER_TAPS");
            value = value.Replace("SUPERRES_FILTER_OFFSET", "AV1Constants.SUPERRES_FILTER_OFFSET");
            value = value.Replace("SUPERRES_SCALE_BITS", "AV1Constants.SUPERRES_SCALE_BITS");
            value = value.Replace("SUPERRES_SCALE_MASK", "AV1Constants.SUPERRES_SCALE_MASK");
            value = value.Replace("SUPERRES_EXTRA_BITS", "AV1Constants.SUPERRES_EXTRA_BITS");
            value = value.Replace("TXB_SKIP_CONTEXTS", "AV1Constants.TXB_SKIP_CONTEXTS");
            value = value.Replace("EOB_COEF_CONTEXTS", "AV1Constants.EOB_COEF_CONTEXTS");
            value = value.Replace("DC_SIGN_CONTEXTS", "AV1Constants.DC_SIGN_CONTEXTS");
            value = value.Replace("LEVEL_CONTEXTS", "AV1Constants.LEVEL_CONTEXTS");
            value = value.Replace("TX_CLASS_2D", "AV1Constants.TX_CLASS_2D");
            value = value.Replace("TX_CLASS_HORIZ", "AV1Constants.TX_CLASS_HORIZ");
            value = value.Replace("TX_CLASS_VERT", "AV1Constants.TX_CLASS_VERT");
            value = value.Replace("REFMVS_LIMIT", "AV1Constants.REFMVS_LIMIT");
            value = value.Replace("INTRA_FILTER_SCALE_BITS", "AV1Constants.INTRA_FILTER_SCALE_BITS");
            value = value.Replace("INTRA_FILTER_MODES", "AV1Constants.INTRA_FILTER_MODES");
            value = value.Replace("COEFF_CDF_Q_CTXS", "AV1Constants.COEFF_CDF_Q_CTXS");
            value = value.Replace("PRIMARY_REF_NONE", "AV1Constants.PRIMARY_REF_NONE");
            value = value.Replace("BUFFER_POOL_MAX_SIZE", "AV1Constants.BUFFER_POOL_MAX_SIZE");

            value = value.Replace("CP_BT_709", "AV1ColorPrimaries.CP_BT_709");
            value = value.Replace("CP_UNSPECIFIED", "AV1ColorPrimaries.CP_UNSPECIFIED");
            value = value.Replace("CP_BT_470_M", "AV1ColorPrimaries.CP_BT_470_M");
            value = value.Replace("CP_BT_470_B_G", "AV1ColorPrimaries.CP_BT_470_B_G");
            value = value.Replace("CP_BT_601", "AV1ColorPrimaries.CP_BT_601");
            value = value.Replace("CP_SMPTE_240", "AV1ColorPrimaries.CP_SMPTE_240");
            value = value.Replace("CP_GENERIC_FILM", "AV1ColorPrimaries.CP_GENERIC_FILM");
            value = value.Replace("CP_BT_2020", "AV1ColorPrimaries.CP_BT_2020");
            value = value.Replace("CP_XYZ", "AV1ColorPrimaries.CP_XYZ");
            value = value.Replace("CP_SMPTE_431", "AV1ColorPrimaries.CP_SMPTE_431");
            value = value.Replace("CP_SMPTE_432", "AV1ColorPrimaries.CP_SMPTE_432");
            value = value.Replace("CP_EBU_3213", "AV1ColorPrimaries.CP_EBU_3213");

            value = value.Replace("TC_RESERVED_0", "AV1TransferCharacteristics.TC_RESERVED_0");
            value = value.Replace("TC_BT_709", "AV1TransferCharacteristics.TC_BT_709");
            value = value.Replace("TC_UNSPECIFIED", "AV1TransferCharacteristics.TC_UNSPECIFIED");
            value = value.Replace("TC_RESERVED_3", "AV1TransferCharacteristics.TC_RESERVED_3");
            value = value.Replace("TC_BT_470_M", "AV1TransferCharacteristics.TC_BT_470_M");
            value = value.Replace("TC_BT_470_B_G", "AV1TransferCharacteristics.TC_BT_470_B_G");
            value = value.Replace("TC_BT_601", "AV1TransferCharacteristics.TC_BT_601");
            value = value.Replace("TC_SMPTE_240", "AV1TransferCharacteristics.TC_SMPTE_240");
            value = value.Replace("TC_LINEAR", "AV1TransferCharacteristics.TC_LINEAR");
            value = value.Replace("TC_LOG_100", "AV1TransferCharacteristics.TC_LOG_100");
            value = value.Replace("TC_LOG_100_SQRT10", "AV1TransferCharacteristics.TC_LOG_100_SQRT10");
            value = value.Replace("TC_IEC_61966", "AV1TransferCharacteristics.TC_IEC_61966");
            value = value.Replace("TC_BT_1361", "AV1TransferCharacteristics.TC_BT_1361");
            value = value.Replace("TC_SRGB", "AV1TransferCharacteristics.TC_SRGB");
            value = value.Replace("TC_BT_2020_10_BIT", "AV1TransferCharacteristics.TC_BT_2020_10_BIT");
            value = value.Replace("TC_BT_2020_12_BIT", "AV1TransferCharacteristics.TC_BT_2020_12_BIT");
            value = value.Replace("TC_SMPTE_2084", "AV1TransferCharacteristics.TC_SMPTE_2084");
            value = value.Replace("TC_SMPTE_428", "AV1TransferCharacteristics.TC_SMPTE_428");
            value = value.Replace("TC_HLG", "AV1TransferCharacteristics.TC_HLG");

            value = value.Replace("MC_IDENTITY", "AV1MatrixCoefficients.MC_IDENTITY");
            value = value.Replace("MC_BT_709", "AV1MatrixCoefficients.MC_BT_709");
            value = value.Replace("MC_UNSPECIFIED", "AV1MatrixCoefficients.MC_UNSPECIFIED");
            value = value.Replace("MC_RESERVED_3", "AV1MatrixCoefficients.MC_RESERVED_3");
            value = value.Replace("MC_FCC", "AV1MatrixCoefficients.MC_FCC");
            value = value.Replace("MC_BT_470_B_G", "AV1MatrixCoefficients.MC_BT_470_B_G");
            value = value.Replace("MC_BT_601", "AV1MatrixCoefficients.MC_BT_601");
            value = value.Replace("MC_SMPTE_240", "AV1MatrixCoefficients.MC_SMPTE_240");
            value = value.Replace("MC_SMPTE_YCGCO", "AV1MatrixCoefficients.MC_SMPTE_YCGCO");
            value = value.Replace("MC_BT_2020_NCL", "AV1MatrixCoefficients.MC_BT_2020_NCL");
            value = value.Replace("MC_BT_2020_CL", "AV1MatrixCoefficients.MC_BT_2020_CL");
            value = value.Replace("MC_SMPTE_2085", "AV1MatrixCoefficients.MC_SMPTE_2085");
            value = value.Replace("MC_CHROMAT_NCL", "AV1MatrixCoefficients.MC_CHROMAT_NCL");
            value = value.Replace("MC_CHROMAT_CL", "AV1MatrixCoefficients.MC_CHROMAT_CL");
            value = value.Replace("MC_ICTCP", "AV1MatrixCoefficients.MC_ICTCP");

            value = value.Replace("CSP_UNKNOWN", "AV1ChromaSamplePosition.CSP_UNKNOWN");
            value = value.Replace("CSP_VERTICAL", "AV1ChromaSamplePosition.CSP_VERTICAL");
            value = value.Replace("CSP_COLOCATED", "AV1ChromaSamplePosition.CSP_COLOCATED");
            value = value.Replace("CSP_RESERVED", "AV1ChromaSamplePosition.CSP_RESERVED");

            value = value.Replace("KEY_FRAME", "AV1FrameTypes.KEY_FRAME");
            value = value.Replace("INTER_FRAME", "AV1FrameTypes.INTER_FRAME");
            value = value.Replace("INTRA_ONLY_FRAME", "AV1FrameTypes.INTRA_ONLY_FRAME");
            value = value.Replace("SWITCH_FRAME", "AV1FrameTypes.SWITCH_FRAME");

            value = value.Replace("METADATA_TYPE_HDR_CLL", "AV1MetadataType.METADATA_TYPE_HDR_CLL");
            value = value.Replace("METADATA_TYPE_HDR_MDCV", "AV1MetadataType.METADATA_TYPE_HDR_MDCV");
            value = value.Replace("METADATA_TYPE_SCALABILITY", "AV1MetadataType.METADATA_TYPE_SCALABILITY");
            value = value.Replace("METADATA_TYPE_ITUT_T35", "AV1MetadataType.METADATA_TYPE_ITUT_T35");
            value = value.Replace("METADATA_TYPE_TIMECODE", "AV1MetadataType.METADATA_TYPE_TIMECODE");
            
            value = value.Replace("RESTORE_NONE", "AV1FrameRestorationType.RESTORE_NONE");
            value = value.Replace("RESTORE_SWITCHABLE", "AV1FrameRestorationType.RESTORE_SWITCHABLE");
            value = value.Replace("RESTORE_WIENER", "AV1FrameRestorationType.RESTORE_WIENER");
            value = value.Replace("RESTORE_SGRPROJ", "AV1FrameRestorationType.RESTORE_SGRPROJ");

            value = value.Replace("SCALABILITY_L4T7_KEY_SHIFT", "AV1ScalabilityModeIdc.SCALABILITY_L4T7_KEY_SHIFT");
            value = value.Replace("SCALABILITY_L1T3", "AV1ScalabilityModeIdc.SCALABILITY_L1T3");
            value = value.Replace("SCALABILITY_L2T1", "AV1ScalabilityModeIdc.SCALABILITY_L2T1");
            value = value.Replace("SCALABILITY_L2T2", "AV1ScalabilityModeIdc.SCALABILITY_L2T2");
            value = value.Replace("SCALABILITY_L2T3", "AV1ScalabilityModeIdc.SCALABILITY_L2T3");
            value = value.Replace("SCALABILITY_S2T1", "AV1ScalabilityModeIdc.SCALABILITY_S2T1");
            value = value.Replace("SCALABILITY_S2T2", "AV1ScalabilityModeIdc.SCALABILITY_S2T2");
            value = value.Replace("SCALABILITY_S2T3", "AV1ScalabilityModeIdc.SCALABILITY_S2T3");
            value = value.Replace("SCALABILITY_L2T1h", "AV1ScalabilityModeIdc.SCALABILITY_L2T1h");
            value = value.Replace("SCALABILITY_L2T2h", "AV1ScalabilityModeIdc.SCALABILITY_L2T2h");
            value = value.Replace("SCALABILITY_L2T3h", "AV1ScalabilityModeIdc.SCALABILITY_L2T3h");
            value = value.Replace("SCALABILITY_S2T1h", "AV1ScalabilityModeIdc.SCALABILITY_S2T1h");
            value = value.Replace("SCALABILITY_S2T2h", "AV1ScalabilityModeIdc.SCALABILITY_S2T2h");
            value = value.Replace("SCALABILITY_S2T3h", "AV1ScalabilityModeIdc.SCALABILITY_S2T3h");
            value = value.Replace("SCALABILITY_SS", "AV1ScalabilityModeIdc.SCALABILITY_SS");
            value = value.Replace("SCALABILITY_L3T1", "AV1ScalabilityModeIdc.SCALABILITY_L3T1");
            value = value.Replace("SCALABILITY_L3T2", "AV1ScalabilityModeIdc.SCALABILITY_L3T2");
            value = value.Replace("SCALABILITY_L3T3", "AV1ScalabilityModeIdc.SCALABILITY_L3T3");
            value = value.Replace("SCALABILITY_S3T1", "AV1ScalabilityModeIdc.SCALABILITY_S3T1");
            value = value.Replace("SCALABILITY_S3T2", "AV1ScalabilityModeIdc.SCALABILITY_S3T2");
            value = value.Replace("SCALABILITY_S3T3", "AV1ScalabilityModeIdc.SCALABILITY_S3T3");
            value = value.Replace("SCALABILITY_L3T2_KEY", "AV1ScalabilityModeIdc.SCALABILITY_L3T2_KEY");
            value = value.Replace("SCALABILITY_L3T3_KEY", "AV1ScalabilityModeIdc.SCALABILITY_L3T3_KEY");
            value = value.Replace("SCALABILITY_L4T5_KEY", "AV1ScalabilityModeIdc.SCALABILITY_L4T5_KEY");
            value = value.Replace("SCALABILITY_L4T7_KEY", "AV1ScalabilityModeIdc.SCALABILITY_L4T7_KEY");
            value = value.Replace("SCALABILITY_L3T2_KEY_SHIFT", "AV1ScalabilityModeIdc.SCALABILITY_L3T2_KEY_SHIFT");
            value = value.Replace("SCALABILITY_L3T3_KEY_SHIFT", "AV1ScalabilityModeIdc.SCALABILITY_L3T3_KEY_SHIFT");
            value = value.Replace("SCALABILITY_L4T5_KEY_SHIFT", "AV1ScalabilityModeIdc.SCALABILITY_L4T5_KEY_SHIFT");
            value = value.Replace("SCALABILITY_L4T7_KEY_SHIFT", "AV1ScalabilityModeIdc.SCALABILITY_L4T7_KEY_SHIFT");
            
            value = value.Replace("ONLY_4X4", "AV1TxModes.ONLY_4X4");
            value = value.Replace("TX_MODE_LARGEST", "AV1TxModes.TX_MODE_LARGEST");
            value = value.Replace("TX_MODE_SELECT", "AV1TxModes.TX_MODE_SELECT");

            value = value.Replace(" EIGHTTAP", " AV1InterpolationFilter.EIGHTTAP");
            value = value.Replace("EIGHTTAP_SMOOTH", "AV1InterpolationFilter.EIGHTTAP_SMOOTH");
            value = value.Replace("EIGHTTAP_SHARP", "AV1InterpolationFilter.EIGHTTAP_SHARP");
            value = value.Replace("BILINEAR", "AV1InterpolationFilter.BILINEAR");
            value = value.Replace("SWITCHABLE", "AV1InterpolationFilter.SWITCHABLE");

            value = value.Replace("choose_operating_point", "ChooseOperatingPoint");
            value = value.Replace("get_qindex", "GetQIndex");
            value = value.Replace("mark_ref_frames", "AomStream.MarkRefFrames");

            value = value.Replace("tile_log2", "TileLog2");
            value = value.Replace("read_delta_q", "ReadDeltaq");
            value = value.Replace("inverse_recenter", "InverseRecenter");
            value = value.Replace("get_relative_dist", "GetRelativeDist");
            value = value.Replace("decode_unsigned_subexp_with_ref", "DecodeUnsignedSubexpWithRef");
            value = value.Replace("decode_signed_subexp_with_ref", "DecodeSignedSubexpWithRef");
            value = value.Replace("decode_subexp", "DecodeSubexp");

            return value;
        }

        public string GetCtorParameterType(string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter))
                return "";

            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                { "sz", "su(32)" },
                { "nbBits", "su(64)" },
                { "op", "su(32)" },
                { "a", "su(32)" },
                { "b", "su(32)" },
                { "idLen", "su(32)" },
                { "blkSize", "su(32)" },
                { "target", "su(32)" },
                { "type", "su(32)" },
                { "refc", "su(32)" },
                { "idx", "su(32)" },
                { "low", "su(32)" },
                { "high", "su(32)" },
                { "r", "su(32)" },
                { "mx", "su(32)" },
                { "numSyms", "su(32)" },
                { "v", "su(32)" },
                { "c", "su(32)" },
                { "sbSize4", "su(32)" },
                { "bSize", "su(32)" },
                { "subSize", "su(32)" },
                { "bw4", "su(32)" },
                { "bh4", "su(32)" },
                { "diff", "su(32)" },
                { "max", "su(32)" },
                { "feature", "su(32)" },
                { "allowSelect", "su(32)" },
                { "row", "su(32)" },
                { "col", "su(32)" },
                { "txSz", "su(32)" },
                { "depth", "su(32)" },
                { "preSkip", "su(32)" },
                { "isCompound", "su(32)" },
                { "refFrame", "su(32)" },
                { "refList", "su(32)" },
                { "comp", "su(32)" },
                { "plane", "su(32)" },
                { "baseX", "su(32)" },
                { "baseY", "su(32)" },
                { "x", "su(32)" },
                { "y", "su(32)" },
                { "startX", "su(32)" },
                { "startY", "su(32)" },
                { "w", "su(32)" },
                { "h", "su(32)" },
                { "subsize", "su(32)" },
                { "blockX", "su(32)" },
                { "blockY", "su(32)" },
                { "txSet", "su(32)" },
                { "txType", "su(32)" },
                { "mode", "su(32)" },
                { "x4", "su(32)" },
                { "y4", "su(32)" },
                { "colorMap", "su(32)" },
                { "n", "su(32)" },
                { "candidateR", "su(32)" },
                { "candidateC", "su(32)" },
                { "mvec", "su(32)" },
                { "border", "su(32)" },
                { "unitRow", "su(32)" },
                { "unitCol", "su(32)" },
                { "k", "su(32)" },
            };

            return map[parameter];
        }

        public string GetFieldDefaultValue(AomField field)
        {
            switch (field.Name)
            {
                case "operating_point_idc":
                case "seq_level_idx":
                case "seq_tier":
                case "decoder_model_present_for_this_op":
                case "initial_display_delay_present_for_this_op":
                case "initial_display_delay_minus_1":
                case "decoder_buffer_delay":
                case "encoder_buffer_delay":
                case "low_delay_mode_flag":
                case "buffer_removal_time":
                    return "new int[1]";

                case "RefValid":
                case "RefOrderHint":
                case "ref_order_hint":
                    return "new int[AV1Constants.NUM_REF_FRAMES]";
                
                case "OrderHints":
                    return "new int[AV1RefFrames.LAST_FRAME + AV1Constants.REFS_PER_FRAME]";

                case "MiColStarts":
                    return "new int[AV1Constants.MAX_TILE_COLS]";

                case "MiRowStarts":
                    return "new int[AV1Constants.MAX_TILE_ROWS]";

                case "FeatureEnabled":
                case "FeatureData":
                    return "new int[AV1Constants.MAX_SEGMENTS][] { new int[AV1Constants.SEG_LVL_MAX],new int[AV1Constants.SEG_LVL_MAX],new int[AV1Constants.SEG_LVL_MAX],new int[AV1Constants.SEG_LVL_MAX],new int[AV1Constants.SEG_LVL_MAX],new int[AV1Constants.SEG_LVL_MAX],new int[AV1Constants.SEG_LVL_MAX],new int[AV1Constants.SEG_LVL_MAX] }";

                case "LosslessArray":
                    return "new int[AV1Constants.MAX_SEGMENTS]";

                case "loop_filter_level":
                    return "new int[4]";

                case "cdef_y_pri_strength":
                case "cdef_y_sec_strength":
                case "cdef_uv_pri_strength":
                case "cdef_uv_sec_strength":
                    return "new int[1]";

                case "FrameRestorationType":
                    return "new int[3]";

                case "Remap_Lr_Type":
                    return "new int[1]";

                case "GmType":
                    return "new int[AV1RefFrames.ALTREF_FRAME + 1]";

                case "gm_params":
                    return "new int[AV1RefFrames.ALTREF_FRAME + 1][] { new int[6],new int[6],new int[6],new int[6],new int[6],new int[6],new int[6],new int[6] }";

                case "expectedFrameId":
                case "ref_frame_idx":
                    return "new int[AV1Constants.REFS_PER_FRAME]";

                case "RefFrameSignBias":
                    return "new int[AV1Constants.REFS_PER_FRAME + AV1RefFrames.LAST_FRAME]";

                case "LoopRestorationSize":
                    return "new int[4]";

                case "loop_filter_ref_deltas":
                    return "new int[8]";

                case "loop_filter_mode_deltas":
                    return "new int[2]";

                case "SegQMLevel":
                    return "new int[3][] { new int[8],new int[8],new int[8] }";

                case "SkipModeFrame":
                    return "new int[2]";

                case "point_y_value":
                case "point_y_scaling":
                    return "new int[14]";

                case "point_cb_value":
                case "point_cb_scaling":
                    return "new int[10]";

                case "point_cr_value":
                case "point_cr_scaling":
                    return "new int[10]";

                case "ar_coeffs_y_plus_128":
                    return "new int[24]";

                case "ar_coeffs_cb_plus_128":
                case "ar_coeffs_cr_plus_128":
                    return "new int[25]";

                case "primary_chromaticity_x":
                case "primary_chromaticity_y":
                    return "new int[3]";

                default:
                    return "";
            }
        }

        public string PreprocessDefinitionsFile(string definitions)
        {
            // rename ref -> refc to avoid C# conflicts with built-in ref keyword
            definitions = definitions.Replace(" ref ", " refc ");
            definitions = definitions.Replace("[ref]", "[refc]");
            definitions = definitions.Replace(", ref,", ", refc,");
            definitions = definitions.Replace("ref++", "refc++");

            // TODO: ternary operator support
            definitions = definitions.Replace("twelve_bit ?", "twelve_bit != 0 ?");
            definitions = definitions.Replace("high_bitdepth ?", "high_bitdepth != 0 ?");
            definitions = definitions.Replace("mono_chrome ?", "mono_chrome != 0 ?");
            definitions = definitions.Replace("use_128x128_superblock ?", "use_128x128_superblock != 0 ?");
            definitions = definitions.Replace("is_translation ?", "is_translation != 0 ?");
            definitions = definitions.Replace(" i ?", " i != 0 ?");
            
            definitions = definitions.Replace("!allow_high_precision_mv", "(allow_high_precision_mv == 0 ? 1 : 0)");
            definitions = definitions.Replace("get_position() & 7", "(get_position() & 7) > 0");
            definitions = definitions.Replace(" v & 1 ", "( v & 1 ) != 0");
            definitions = definitions.Replace("return subexp_final_bits", "return (int)subexp_final_bits");
            definitions = definitions.Replace("itu_t_t35_payload_bytes", "itu_t_t35_payload_bytes()");
            
            definitions = definitions.Replace("height_in_sbs_minus_1 + 1", "(int)(height_in_sbs_minus_1 + 1)");
            definitions = definitions.Replace("width_in_sbs_minus_1 + 1", "(int)(width_in_sbs_minus_1 + 1)");

            definitions = definitions.Replace("CodedLossless && ( FrameWidth == UpscaledWidth )", "(CodedLossless != 0 && ( FrameWidth == UpscaledWidth )) ? 1 : 0");
            definitions = definitions.Replace("TileNum == tg_end", "(TileNum == tg_end) ? 1 : 0");
            definitions = definitions.Replace("qindex == 0 && DeltaQYDc == 0 && DeltaQUAc == 0 && DeltaQUDc == 0 && DeltaQVAc == 0 && DeltaQVDc == 0", "(qindex == 0 && DeltaQYDc == 0 && DeltaQUAc == 0 && DeltaQUDc == 0 && DeltaQVAc == 0 && DeltaQVDc == 0) ? 1 : 0");
            definitions = definitions.Replace("frame_type != KEY_FRAME", "(frame_type != KEY_FRAME) ? 1 : 0");
            definitions = definitions.Replace("get_relative_dist( hint, OrderHint) > 0", "(get_relative_dist( hint, OrderHint) > 0) ? 1 : 0");
            definitions = definitions.Replace("frame_type == INTRA_ONLY_FRAME || frame_type == KEY_FRAME", "(frame_type == INTRA_ONLY_FRAME || frame_type == KEY_FRAME) ? 1 : 0");

            return definitions;
        }
    }
}
