// Generated from the AV2 Bitstream & Decoding Process Specification v1.0.0 (https://av2.aomedia.org/v1.0.0/):
// Table 3.1 (Additional constants used in the specification), and the tables of section 6 and beyond that
// name a syntax element's values ("Name of obu_type", "Name of FrameType" and the like). The names are the
// specification's, so that the generated code reads as it does.
namespace SharpAV2
{
    public partial class AV2Context
    {
        /// <summary>Inverse transform rows with ADST and columns with ADST</summary>
        public const int ADST_ADST = 3;
        /// <summary>Inverse transform rows with DCT and columns with ADST</summary>
        public const int ADST_DCT = 1;
        /// <summary>Inverse transform rows with FLIPADST and columns with ADST</summary>
        public const int ADST_FLIPADST = 7;
        /// <summary>Warp model is a general affine transform</summary>
        public const int AFFINE = 2;
        /// <summary>Number of degrees of step-per-unit increase in AngleDeltaY or AngleDeltaUV .</summary>
        public const int ANGLE_STEP = 3;
        /// <summary>Number of parameter banks for motion vectors</summary>
        public const int BANK_REFS_PER_FRAME = 9;
        /// <summary>Number of contexts for explicit_bawp</summary>
        public const int BAWP_SCALES_CTX_COUNT = 3;
        /// <summary>A blend weight used in smooth intra prediction</summary>
        public const int BLEND_WEIGHT_MAX = 32;
        /// <summary>Sentinel value to mark partition choices that are not allowed</summary>
        public const int BLOCK_INVALID = 29;
        /// <summary>Number of different block sizes used</summary>
        public const int BLOCK_SIZES = 29;
        /// <summary>Number of contexts when decoding y_mode</summary>
        public const int BLOCK_SIZE_GROUPS = 4;
        /// <summary>Number of values for coeff_br</summary>
        public const int BR_CDF_SIZE = 4;
        /// <summary>Maximum number of bands allowed in CCSO</summary>
        public const int CCSO_BAND_NUM = 64;
        /// <summary>Number of contexts when decoding ccso_blk</summary>
        public const int CCSO_CONTEXT = 4;
        /// <summary>Number of classes for CCSO</summary>
        public const int CCSO_INPUT_INTERVAL = 3;
        /// <summary>Base 2 logarithm of size of CCSO blocks (measured in luma samples)</summary>
        public const int CCSO_LUMA_SIZE_LOG2 = 8;
        /// <summary>Precision bits used during cross component transform</summary>
        public const int CCTX_PREC_BITS = 8;
        /// <summary>Number of values for cctx_type</summary>
        public const int CCTX_TYPES = 7;
        /// <summary>Value indicating CDEF has a frame level enabled for whether it is used on skipped transform blocks</summary>
        public const int CDEF_ON_SKIP_TXFM_ADAPTIVE = 2;
        /// <summary>Value indicating CDEF is enabled on skipped transform blocks</summary>
        public const int CDEF_ON_SKIP_TXFM_ALWAYS_ON = 1;
        /// <summary>Value indicating CDEF is disabled on skipped transform block s</summary>
        public const int CDEF_ON_SKIP_TXFM_DISABLED = 0;
        /// <summary>Number of contexts for cdef_index0</summary>
        public const int CDEF_STRENGTH_INDEX0_CTX = 4;
        /// <summary>Number of values for cfl_alpha_u and cfl_alpha_v</summary>
        public const int CFL_ALPHABET_SIZE = 8;
        /// <summary>Number of contexts for cfl_alpha_u and cfl_alpha_v</summary>
        public const int CFL_ALPHA_CONTEXTS = 6;
        /// <summary>Number of contexts for is_cfl</summary>
        public const int CFL_CONTEXTS = 3;
        /// <summary>Number of values for cfl_alpha_signs</summary>
        public const int CFL_JOINT_SIGNS = 8;
        /// <summary>Number of values for uv_mode</summary>
        public const int CHROMA_MODE_COUNT = 8;
        /// <summary>Number of contexts for coeff_base when the parity is hidden</summary>
        public const int COEFF_BASE_PH_CONTEXTS = 5;
        /// <summary>Number of values for coeff_br ( coeff_br extends the range of coeff_base )</summary>
        public const int COEFF_BASE_RANGE = 3;
        /// <summary>Number of selectable context types for the coeffs( ) syntax structure</summary>
        public const int COEFF_CDF_Q_CTXS = 4;
        /// <summary>Number of values for compound_mode</summary>
        public const int COMPOUND_MODES = 7;
        /// <summary>Number of contexts for compound_mode</summary>
        public const int COMPOUND_MODE_CONTEXTS = 5;
        /// <summary>Number of values for compound_type</summary>
        public const int COMPOUND_TYPES = 2;
        /// <summary>Number of contexts for comp_group_idx</summary>
        public const int COMP_GROUP_IDX_CONTEXTS = 12;
        /// <summary>Number of contexts for comp_mode</summary>
        public const int COMP_INTER_CONTEXTS = 5;
        /// <summary>Value for CwpIdx that corresponds to equal weighting for two inter references</summary>
        public const int CWP_EQUAL = 8;
        /// <summary>Length of Q_First array</summary>
        public const int DBL_REG_DECIS_LEN = 9;
        /// <summary>Inverse transform rows with ADST and columns with DCT</summary>
        public const int DCT_ADST = 2;
        /// <summary>Inverse transform rows with DCT and columns with DCT</summary>
        public const int DCT_DCT = 0;
        /// <summary>Inverse transform rows with FLIPADST and columns with DCT</summary>
        public const int DCT_FLIPADST = 5;
        /// <summary>Number of contexts for dc_sign</summary>
        public const int DC_SIGN_CONTEXTS = 3;
        /// <summary>Number of groups of contexts for dc_sign (corresponding to whether the sign is hidden or not)</summary>
        public const int DC_SIGN_GROUPS = 2;
        /// <summary>Maximum distance that can use array Dist_Score_Lookup</summary>
        public const int DECAY_DIST_CAP = 6;
        /// <summary>Use delta warp motion compensation</summary>
        public const int DELTAWARP = 3;
        /// <summary>Number of bits for base_y_dc_delta_q , base_uv_dc_delta_q , and base_uv_ac_delta_q</summary>
        public const int DELTA_DCQUANT_BITS = 5;
        /// <summary>Maximum value for BaseYDcDeltaQ and BaseUVDcDeltaQ</summary>
        public const int DELTA_DCQUANT_MAX = (1 << (DELTA_DCQUANT_BITS - 2));
        /// <summary>Minimum value for BaseYDcDeltaQ and BaseUVDcDeltaQ</summary>
        public const int DELTA_DCQUANT_MIN = (DELTA_DCQUANT_MAX - (1 << DELTA_DCQUANT_BITS) + 1);
        /// <summary>Value indicating alternative encoding of quantizer index delta values</summary>
        public const int DELTA_Q_SMALL = 7;
        /// <summary>Scale factor for DfDeltaQ</summary>
        public const int DF_DELTA_SCALE = 8;
        /// <summary>Shift used in deblocking filter</summary>
        public const int DF_SHIFT = 8;
        /// <summary>Number of contexts for use_dip</summary>
        public const int DIP_CTXS = 3;
        /// <summary>Number of directional intra modes</summary>
        public const int DIRECTIONAL_MODES_COUNT = 56;
        /// <summary>Number of order hint bits</summary>
        public const int DISPLAY_ORDER_HINT_BITS = 30;
        /// <summary>Scaling used in scoring reference frames</summary>
        public const int DIST_WEIGHT_BITS = 6;
        /// <summary>Number of fractional bits for lookup in divisor lookup table</summary>
        public const int DIV_LUT_BITS = 7;
        /// <summary>Number of entries in divisor lookup table</summary>
        public const int DIV_LUT_NUM = 129;
        /// <summary>Number of fractional bits of entries in divisor lookup table</summary>
        public const int DIV_LUT_PREC_BITS = 9;
        /// <summary>Number of bits used in get_division_scale_shift</summary>
        public const int DIV_PREC_BITS = 14;
        /// <summary>Number of regions used in get_division_scale_shift</summary>
        public const int DIV_PREC_BITS_POW2 = 8;
        /// <summary>Base 2 logarithm of regions used in get_division_scale_shift</summary>
        public const int DIV_SLOT_BITS = 3;
        /// <summary>Number of contexts for drl_mode</summary>
        public const int DRL_MODE_CONTEXTS = 5;
        /// <summary>Number of bits to reduce CDF precision during arithmetic coding</summary>
        public const int EC_PROB_SHIFT = 7;
        /// <summary>Number of contexts for EOB -related syntax elements</summary>
        public const int EOB_PLANE_CTXS = 3;
        /// <summary>Use extended warp motion compensation</summary>
        public const int EXTENDWARP = 4;
        /// <summary>Number of partition types</summary>
        public const int EXT_PARTITION_TYPES = 10;
        /// <summary>Number of size classes (each size class has a different choice of transform types)</summary>
        public const int EXT_TX_SIZES = 4;
        /// <summary>Number of phases for extended warp filtering</summary>
        public const int EXT_WARP_PHASES = 64;
        /// <summary>Base 2 logarithm of number of phases for extended warp filtering</summary>
        public const int EXT_WARP_PHASES_LOG2 = 6;
        /// <summary>Difference between bits used for the warp model and bits needed to specify the phase for extended warp filtering</summary>
        public const int EXT_WARP_ROUND_BITS = WARPEDMODEL_PREC_BITS - EXT_WARP_PHASES_LOG2;
        /// <summary>Number of taps in extended warp filtering</summary>
        public const int EXT_WARP_TAPS = 6;
        /// <summary>Number of bits used in Wiener filter coefficients</summary>
        public const int FILTER_BITS = 7;
        /// <summary>Number of values coded via the first intra mode set</summary>
        public const int FIRST_MODE_COUNT = 13;
        /// <summary>Inverse transform rows with ADST and columns with FLIPADST</summary>
        public const int FLIPADST_ADST = 8;
        /// <summary>Inverse transform rows with DCT and columns with FLIPADST</summary>
        public const int FLIPADST_DCT = 4;
        /// <summary>Inverse transform rows with FLIPADST and columns with FLIPADST</summary>
        public const int FLIPADST_FLIPADST = 6;
        /// <summary>Number of block size groups in context for fsc_mode</summary>
        public const int FSC_BSIZE_CONTEXTS = 6;
        /// <summary>Max width/height for blocks to use forward skip coding</summary>
        public const int FSC_MAX = 32;
        /// <summary>Number of values of fsc_mode</summary>
        public const int FSC_MODES = 2;
        /// <summary>Number of contexts for fsc_mode</summary>
        public const int FSC_MODE_CONTEXTS = 4;
        /// <summary>Number of transform size context groups for forward skip coding</summary>
        public const int FSC_TX_SIZE_CONTEXTS = 3;
        /// <summary>GDF first diagonal direction</summary>
        public const int GDF_DIAG0 = 2;
        /// <summary>GDF second diagonal direction</summary>
        public const int GDF_DIAG1 = 3;
        /// <summary>GDF horizontal direction</summary>
        public const int GDF_HOR = 1;
        /// <summary>Minimum size of GDF blocks when gdf_unit_matches_sb_size is equal to 0</summary>
        public const int GDF_MIN_SIZE = 128;
        /// <summary>GDF vertical direction</summary>
        public const int GDF_VER = 0;
        /// <summary>Value for xlayer_id that indicates global scope</summary>
        public const int GLOBAL_XLAYER_ID = 31;
        /// <summary>Number of bits encoded for non-translational components of global motion models</summary>
        public const int GM_ABS_ALPHA_BITS = 9;
        /// <summary>Number of bits encoded for translational components of global motion models, if part of a ROTZOOM or AFFINE model</summary>
        public const int GM_ABS_TRANS_BITS = 14;
        /// <summary>Maximum non-translational value</summary>
        public const int GM_ALPHA_MAX = (1 << GM_ABS_ALPHA_BITS) - 1;
        /// <summary>Minimum non-translational value</summary>
        public const int GM_ALPHA_MIN = -GM_ALPHA_MAX;
        /// <summary>Number of fractional bits for sending non-translational warp model coefficients</summary>
        public const int GM_ALPHA_PREC_BITS = 10;
        /// <summary>Difference between warped model and non-translational precision</summary>
        public const int GM_ALPHA_PREC_DIFF = WARPEDMODEL_PREC_BITS - GM_ALPHA_PREC_BITS;
        /// <summary>Maximum translational value</summary>
        public const int GM_TRANS_MAX = (1 << GM_ABS_TRANS_BITS) - 1;
        /// <summary>Minimum translational value</summary>
        public const int GM_TRANS_MIN = -GM_TRANS_MAX;
        /// <summary>Difference between warped model and motion vector precision</summary>
        public const int GM_TRANS_ONLY_PREC_DIFF = WARPEDMODEL_PREC_BITS - 3;
        /// <summary>Number of fractional bits for sending translational warp model coefficients</summary>
        public const int GM_TRANS_PREC_BITS = 3;
        /// <summary>Difference between warped model and translational precision</summary>
        public const int GM_TRANS_PREC_DIFF = WARPEDMODEL_PREC_BITS - GM_TRANS_PREC_BITS;
        /// <summary>Inverse transform rows with ADST and columns with identity</summary>
        public const int H_ADST = 13;
        /// <summary>Inverse transform rows with DCT and columns with identity</summary>
        public const int H_DCT = 11;
        /// <summary>Inverse transform rows with FLIPADST and columns with identity</summary>
        public const int H_FLIPADST = 15;
        /// <summary>Number of wedge angles when wedge_angle_dir is equal to 0</summary>
        public const int H_WEDGE_ANGLES = 10;
        /// <summary>Size of buffer used in local intra block copy</summary>
        public const int IBC_BUFFER_SIZE = 64;
        /// <summary>Base 2 logarithm of size of buffer used in local intra block copy</summary>
        public const int IBC_BUFFER_SIZE_LOG2 = 6;
        /// <summary>Number of buffers used in local intra block copy</summary>
        public const int IBC_NUM_BUFFERS = 4;
        /// <summary>Sum of weights used in IBP</summary>
        public const int IBP_WEIGHT_MAX = 128;
        /// <summary>Scaling shift for IBP process</summary>
        public const int IBP_WEIGHT_SHIFT = 7;
        /// <summary>Size of weights used in IBP</summary>
        public const int IBP_WEIGHT_SIZE = 1 << IBP_WEIGHT_SIZE_LOG2;
        /// <summary>Base 2 logarithm of size of weights used in IBP</summary>
        public const int IBP_WEIGHT_SIZE_LOG2 = 4;
        /// <summary>Warp model is just an identity transform</summary>
        public const int IDENTITY = 0;
        /// <summary>Inverse transform rows with identity and columns with identity</summary>
        public const int IDTX = 9;
        /// <summary>Number of contexts per transform size group for coeff_br_idtx</summary>
        public const int IDTX_LEVEL_CONTEXTS = 7;
        /// <summary>Number of contexts per transform size group for idtx_sign</summary>
        public const int IDTX_SIGN_CONTEXTS = 9;
        /// <summary>Number of contexts per transform size group for coeff_base_idtx</summary>
        public const int IDTX_SIG_COEF_CONTEXTS = 7;
        /// <summary>Largest number representable with 32-bit signed integer</summary>
        public const int INT32MAX = int.MaxValue; // ( 1 << 31 ) - 1
        /// <summary>Smallest number representable with 32-bit signed integer</summary>
        public const int INT32MIN = int.MinValue; // -( 1 << 31 )
        /// <summary>Use inter intra motion compensation</summary>
        public const int INTERINTRA = 1;
        /// <summary>Number of inter intra modes</summary>
        public const int INTERINTRA_MODES = 4;
        /// <summary>Number of values for interp_filter</summary>
        public const int INTERP_FILTERS = 3;
        /// <summary>Number of contexts for interp_filter</summary>
        public const int INTERP_FILTER_CONTEXTS = 16;
        /// <summary>Number of contexts for region_type</summary>
        public const int INTER_SDP_BSIZE_GROUP = 4;
        /// <summary>Maximum size for switching partitioning scheme</summary>
        public const int INTER_SDP_MAX_BLOCK_SIZE = 64;
        /// <summary>Number of contexts for use_intrabc</summary>
        public const int INTRABC_CONTEXTS = 3;
        /// <summary>Number of horizontal luma samples before intra block copy can be used</summary>
        public const int INTRABC_DELAY_PIXELS = 256;
        /// <summary>Number of 64 by 64 blocks before intra block copy can be used</summary>
        public const int INTRABC_DELAY_SB64 = 4;
        /// <summary>Number of filter kernels for the intra edge filter</summary>
        public const int INTRA_EDGE_KERNELS = 3;
        /// <summary>Number of kernel taps for the intra edge filter</summary>
        public const int INTRA_EDGE_TAPS = 5;
        /// <summary>Number of values for y_mode</summary>
        public const int INTRA_MODES = 13;
        /// <summary>Number of values for y_mode_set</summary>
        public const int INTRA_MODE_SETS = 4;
        /// <summary>Value for region_type that indicates intra coding</summary>
        public const int INTRA_REGION = 0;
        /// <summary>Number of values for intra_tx_type</summary>
        public const int INTRA_TX_TYPES = 7;
        /// <summary>Height of matrix used in 4x4 secondary transform</summary>
        public const int IST_4X4_HEIGHT = 8;
        /// <summary>Width of matrix used in 4x4 secondary transform</summary>
        public const int IST_4X4_WIDTH = 16;
        /// <summary>Height of matrix used in 8x8 secondary transform</summary>
        public const int IST_8X8_HEIGHT = 32;
        /// <summary>Reduced height of matrix used in special case of 8x8 secondary transform</summary>
        public const int IST_8X8_HEIGHT_RED = 20;
        /// <summary>Width of matrix used in 8x8 secondary transform</summary>
        public const int IST_8X8_WIDTH = 48;
        /// <summary>Number of directional groups in secondary transform kernels</summary>
        public const int IST_DIR_SIZE = 7;
        /// <summary>Number of different sets of secondary transforms for ADST</summary>
        public const int IST_REDUCE_SET_SIZE_ADST_ADST = 4;
        /// <summary>Number of different sets of 4x4 secondary transforms</summary>
        public const int IST_SET_SIZE_4X4 = 14;
        /// <summary>Number of different sets of 8x8 secondary transforms</summary>
        public const int IST_SET_SIZE_8X8 = 11;
        /// <summary>Number of contexts for is_inter</summary>
        public const int IS_INTER_CONTEXTS = 4;
        /// <summary>Number of values for jmvd_scale_mode when use_amvd is equal to 1</summary>
        public const int JOINT_AMVD_SCALE_FACTOR_CNT = 3;
        /// <summary>Number of values for jmvd_scale_mode when use_amvd is equal to 0</summary>
        public const int JOINT_NEWMV_SCALE_FACTOR_CNT = 5;
        /// <summary>Largest number of samples used when computing a local warp</summary>
        public const int LEAST_SQUARES_SAMPLES_MAX = 8;
        /// <summary>Number of contexts for coeff_br for high frequency luma coefficients</summary>
        public const int LEVEL_CONTEXTS = 7;
        /// <summary>Number of contexts for coeff_br for high frequency chroma coefficients</summary>
        public const int LEVEL_CONTEXTS_UV = 4;
        /// <summary>Number of values for coeff_base for low frequency coefficients</summary>
        public const int LF_BASE_SYMBOLS = 6;
        /// <summary>Number of contexts for coeff_br for low frequency luma coefficients</summary>
        public const int LF_LEVEL_CONTEXTS = 14;
        /// <summary>Base level threshold for low frequency coefficients for deciding to read coeff_br</summary>
        public const int LF_NUM_BASE_LEVELS = LF_BASE_SYMBOLS - 2;
        /// <summary>Number of contexts for coeff_base for low frequency luma coefficients</summary>
        public const int LF_SIG_COEF_CONTEXTS = LF_SIG_COEF_CONTEXTS_2D + LF_SIG_COEF_CONTEXTS_1D;
        /// <summary>Number of contexts for 1d luma transform class</summary>
        public const int LF_SIG_COEF_CONTEXTS_1D = 12;
        /// <summary>Number of contexts for 1d chroma transform class</summary>
        public const int LF_SIG_COEF_CONTEXTS_1D_UV = 4;
        /// <summary>Number of contexts for 2d luma transform class</summary>
        public const int LF_SIG_COEF_CONTEXTS_2D = 21;
        /// <summary>Number of contexts for 2d chroma transform class</summary>
        public const int LF_SIG_COEF_CONTEXTS_2D_UV = 8;
        /// <summary>Number of contexts for coeff_base for low frequency chroma coefficients</summary>
        public const int LF_SIG_COEF_CONTEXTS_UV = LF_SIG_COEF_CONTEXTS_2D_UV + LF_SIG_COEF_CONTEXTS_1D_UV;
        /// <summary>Use local warp motion compensation</summary>
        public const int LOCALWARP = 2;
        /// <summary>Size of coefficient cache used for loop restoration</summary>
        public const int LR_BANK_SIZE = 4;
        /// <summary>Largest motion vector difference to include in local warp computation</summary>
        public const int LS_MV_MAX = 256;
        /// <summary>Size of MasterMask array</summary>
        public const int MASK_MASTER_SIZE = 128;
        /// <summary>Maximum quantizer when bit depth is 8</summary>
        public const int MAXQ_8_BITS = 255;
        /// <summary>Maximum quantizer when bit depth is 10</summary>
        public const int MAXQ_10_BITS = MAXQ_8_BITS + 2 * MAXQ_OFFSET;
        /// <summary>Maximum quantizer irrespective of the bit depth</summary>
        public const int MAXQ_BITS = MAXQ_8_BITS + 4 * MAXQ_OFFSET;
        /// <summary>Increase in allowed quantizer for each increase in bit depth</summary>
        public const int MAXQ_OFFSET = 24;
        /// <summary>Number of values for amvd_index</summary>
        public const int MAX_AMVD_INDEX = 8;
        /// <summary>Maximum magnitude of AngleDeltaY and AngleDeltaUV</summary>
        public const int MAX_ANGLE_DELTA = 3;
        /// <summary>Maximum number of Atlas region columns</summary>
        public const int MAX_ATLAS_COLS = 64;
        /// <summary>Maximum number of Atlas region rows</summary>
        public const int MAX_ATLAS_ROWS = 64;
        /// <summary>The maximum value for coeff_base and coeff_br combined</summary>
        public const int MAX_BASE_BR_RANGE = COEFF_BASE_RANGE + NUM_BASE_LEVELS + 1;
        /// <summary>Maximum times col_mv_greater can be coded per motion vector</summary>
        public const int MAX_COL_TRUNCATED_UNARY_VAL = 2;
        /// <summary>Number of values for CwpIdx</summary>
        public const int MAX_CWP_NUM = 5;
        /// <summary>Maximum distance from edge for samples used in the deblocking filter</summary>
        public const int MAX_DBL_FLT_LEN = 8;
        /// <summary>Used to limit the number of derived motion vector pruning operations</summary>
        public const int MAX_DR_PR_NUM = 2;
        /// <summary>Maximum number of motion vectors in the derived stack</summary>
        public const int MAX_DR_STACK_SIZE = 4;
        /// <summary>Maximum number of film grain configurations</summary>
        public const int MAX_FILM_GRAIN = 8;
        /// <summary>Maximum distance when computing weighted prediction</summary>
        public const int MAX_FRAME_DISTANCE = 31;
        /// <summary>Maximum number of loop restoration tools to switch between</summary>
        public const int MAX_LR_FLEX_SWITCHABLE_BITS = 3;
        /// <summary>Maximum bits in least squares calculations</summary>
        public const int MAX_LS_BITS = 26;
        /// <summary>Maximum number of multi-frame headers</summary>
        public const int MAX_MFH_NUM = 16;
        /// <summary>Maximum number of Atlas segments</summary>
        public const int MAX_NUM_ATLAS_SEGMENTS = 256;
        /// <summary>Maximum number of embedded layer s</summary>
        public const int MAX_NUM_MLAYERS = 8;
        /// <summary>Maximum number of temporal layer s</summary>
        public const int MAX_NUM_TLAYERS = 4;
        /// <summary>Used to limit the number of motion vector pruning operations</summary>
        public const int MAX_PR_NUM = 16;
        /// <summary>Maximum number of motion vectors in the stack for intra block copy</summary>
        public const int MAX_REF_BV_STACK_SIZE = 4;
        /// <summary>Maximum number of motion vectors in the stack</summary>
        public const int MAX_REF_MV_STACK_SIZE = 6;
        /// <summary>Maximum number of accesses to the bank of motion vectors per superblock</summary>
        public const int MAX_RMB_SB_HITS = 64;
        /// <summary>Number of segments allowed in segmentation map</summary>
        public const int MAX_SEGMENTS = 16;
        /// <summary>Maximum number of sequence headers</summary>
        public const int MAX_SEQ_NUM = 16;
        /// <summary>Length of Side_Thresholds array</summary>
        public const int MAX_SIDE_TABLE = 296;
        /// <summary>Maximum area of a tile in units of luma samples</summary>
        public const int MAX_TILE_AREA = 4096 * 2304;
        /// <summary>Maximum number of tile columns</summary>
        public const int MAX_TILE_COLS = 64;
        /// <summary>Maximum number of tile rows</summary>
        public const int MAX_TILE_ROWS = 64;
        /// <summary>Maximum width of a tile in units of luma samples</summary>
        public const int MAX_TILE_WIDTH = 4096;
        /// <summary>Maximum number of warp reference candidates</summary>
        public const int MAX_WARP_REF_CANDIDATES = 4;
        /// <summary>Maximum number of accesses to the warp parameter bank per superblock</summary>
        public const int MAX_WARP_SB_HITS = 64;
        /// <summary>Stack size for motion field motion vectors</summary>
        public const int MFMV_STACK_SIZE = 4;
        /// <summary>Number of bits used in MHCCP</summary>
        public const int MHCCP_BITS = 16;
        /// <summary>Value for region_type that indicates mixed intra coding and inter coding</summary>
        public const int MIXED_REGION = 1;
        /// <summary>Smallest size of a mode info block in luma samples</summary>
        public const int MI_SIZE = 4;
        /// <summary>Base 2 logarithm of smallest size of a mode info block</summary>
        public const int MI_SIZE_LOG2 = 2;
        /// <summary>Number of values for y_mode_index</summary>
        public const int MODE_INDEX_COUNT = 8;
        /// <summary>Number of values for y_mode_offset</summary>
        public const int MODE_OFFSET_COUNT = 6;
        /// <summary>Number of values for motion modes</summary>
        public const int MOTION_MODES = 5;
        /// <summary>Number of contexts for mrl_index</summary>
        public const int MRL_INDEX_CONTEXTS = 3;
        /// <summary>Value used when clipping motion vectors</summary>
        public const int MV_BORDER = 128;
        /// <summary>Number of contexts for decoding motion vectors including one for intra block copy</summary>
        public const int MV_CONTEXTS = 2;
        /// <summary>Motion vector context used for intra block copy</summary>
        public const int MV_INTRABC_CONTEXT = 1;
        /// <summary>Number of bits for motion vectors (not including sign bit)</summary>
        public const int MV_IN_USE_BITS = 16;
        /// <summary>Number of values for mv_joint</summary>
        public const int MV_JOINTS = 4;
        /// <summary>Exclusive lower bound on motion vectors</summary>
        public const int MV_LOW = -(1 << MV_IN_USE_BITS);
        /// <summary>Number of bits for motion vectors from optical flow</summary>
        public const int MV_REFINE_PREC_BITS = 4;
        /// <summary>Exclusive upper bound on motion vectors</summary>
        public const int MV_UPP = (1 << MV_IN_USE_BITS);
        /// <summary>Number of non-directional intra modes</summary>
        public const int NON_DIRECTIONAL_MODES_COUNT = 5;
        /// <summary>Number of quantizer base levels</summary>
        public const int NUM_BASE_LEVELS = 2;
        /// <summary>Number of contexts for col_mv_greater</summary>
        public const int NUM_CTX_COL_MV_GTX = 2;
        /// <summary>Number of contexts for col_mv_index</summary>
        public const int NUM_CTX_COL_MV_INDEX = 4;
        /// <summary>Maximum number of quantization matrices that can be present</summary>
        public const int NUM_CUSTOM_QMS = 15;
        /// <summary>Number of adaptation rates</summary>
        public const int NUM_PARA_COMBINATIONS = 125;
        /// <summary>Number of time intervals for computing adaptation rates</summary>
        public const int NUM_PARA_INTERVALS = 3;
        /// <summary>Number of filters in pixel-classified Wiener filtering</summary>
        public const int NUM_PC_WIENER_FILTERS = 64;
        /// <summary>Number of classes in pixel-classified Wiener filtering</summary>
        public const int NUM_PC_WIENER_LUT_CLASSES = 256;
        /// <summary>Number of types of rectangle</summary>
        public const int NUM_RECT_PARTS = 2;
        /// <summary>Number of frames that can be stored for future reference</summary>
        public const int NUM_REF_FRAMES = 16;
        /// <summary>Number of samples used in chroma from luma prediction</summary>
        public const int NUM_REF_SAM_CFL = 8;
        /// <summary>Number of uneven partition types</summary>
        public const int NUM_UNEVEN_4WAY_PARTS = 2;
        /// <summary>Number of distances for the wedge mask process</summary>
        public const int NUM_WEDGE_DIST = 4;
        /// <summary>Size of unit used in gradient computation</summary>
        public const int OPFL_GRAD_UNIT = 16;
        /// <summary>Base 2 logarithm of size of unit used in gradient computation</summary>
        public const int OPFL_GRAD_UNIT_LOG2 = 4;
        /// <summary>Maximum adjustment for motion vectors from optical flow</summary>
        public const int OPFL_MV_DELTA_LIMIT = 1 << MV_REFINE_PREC_BITS;
        /// <summary>Number of values for palette_color</summary>
        public const int PALETTE_COLORS = 8;
        /// <summary>Number of values for color contexts</summary>
        public const int PALETTE_COLOR_CONTEXTS = 5;
        /// <summary>Number of mappings between color context hash and color context</summary>
        public const int PALETTE_MAX_COLOR_CONTEXT_HASH = 8;
        /// <summary>Number of neighbors considered within palette computation</summary>
        public const int PALETTE_NUM_NEIGHBORS = 3;
        /// <summary>Number of values for identity row contexts</summary>
        public const int PALETTE_ROW_FLAG_CONTEXTS = 4;
        /// <summary>Number of values for palette_size</summary>
        public const int PALETTE_SIZES = 7;
        /// <summary>Number of contexts when decoding partition syntax elements</summary>
        public const int PARTITION_CONTEXTS = 64;
        /// <summary>Maximum number of partitions for a block (luma and chroma can have different partitions)</summary>
        public const int PARTITION_STRUCTURE_NUM = 2;
        /// <summary>Number of coefficients in pixel-classified Wiener filtering</summary>
        public const int PC_WIENER_COEFFS = 13;
        /// <summary>Number of lagging taps in pixel-classified Wiener filtering</summary>
        public const int PC_WIENER_LAG = 4;
        /// <summary>Number of leading taps in pixel-classified Wiener filtering</summary>
        public const int PC_WIENER_LEAD = 1;
        /// <summary>Number of features for pixel-classified Wiener filtering</summary>
        public const int PC_WIENER_NUM_FEATURES = 4;
        /// <summary>Bit precision for pixel-classified Wiener filtering</summary>
        public const int PC_WIENER_PREC_BITS = 7;
        /// <summary>Bit precision for pixel-classified features</summary>
        public const int PC_WIENER_PREC_FEATURE = 14;
        /// <summary>Number of taps in pixel-classified Wiener filtering</summary>
        public const int PC_WIENER_TAPS = PC_WIENER_COEFFS * 2 - 1;
        /// <summary>Number of non-zero coefficients that will allow the parity to be hidden</summary>
        public const int PHTHRESH = 4;
        /// <summary>Number of different plane types ( luma or chroma )</summary>
        public const int PLANE_TYPES = 2;
        /// <summary>Value of primary_ref_frame , indicating that the primary reference frame is chosen automatically from the available reference frames</summary>
        public const int PRIMARY_REF_CHOOSE = 8;
        /// <summary>Value of primary_ref_frame , indicating that there is no primary reference frame</summary>
        public const int PRIMARY_REF_NONE = 7;
        /// <summary>Number of bits to discard from quantizer before application</summary>
        public const int QUANT_TABLE_BITS = 3;
        /// <summary>Block is split with a horizontal cut</summary>
        public const int RECT_HORZ = 0;
        /// <summary>Block cannot be split into rectangles</summary>
        public const int RECT_INVALID = 2;
        /// <summary>Block is split with a vertical cut</summary>
        public const int RECT_VERT = 1;
        /// <summary>Number of contexts for use_refinemv</summary>
        public const int REFINEMV_CONTEXTS = 24;
        /// <summary>Largest reference MV component that can be saved</summary>
        public const int REFMVS_LIMIT = ( 1 << 11 ) - 1;
        /// <summary>Number of reference frame s that can be used for inter prediction</summary>
        public const int REFS_PER_FRAME = 7;
        /// <summary>Number of contexts for single_ref</summary>
        public const int REF_CONTEXTS = 3;
        /// <summary>Size of the parameter bank for motion vectors</summary>
        public const int REF_MV_BANK_SIZE = 4;
        /// <summary>Number of bits of precision when scaling reference frames</summary>
        public const int REF_SCALE_SHIFT = 14;
        /// <summary>Maximum size of a loop restoration tile</summary>
        public const int RESTORATION_TILESIZE_MAX = 512;
        /// <summary>Number of switchable loop restoration types</summary>
        public const int RESTORE_SWITCHABLE_TYPES = RESTORE_SWITCHABLE;
        /// <summary>Sentinel order hint to mark restricted reference frames</summary>
        public const int RESTRICTED_OH = -1;
        /// <summary>Warp model is a rotation + symmetric zoom + translation</summary>
        public const int ROTZOOM = 1;
        /// <summary>Number of bits of precision when computing inter prediction locations</summary>
        public const int SCALE_SUBPEL_BITS = 10;
        /// <summary>Number of values for y_second_mode</summary>
        public const int SECOND_MODE_COUNT = 16;
        /// <summary>Number of contexts for segment_id</summary>
        public const int SEGMENT_ID_CONTEXTS = 3;
        /// <summary>Number of contexts for segment_id_predicted</summary>
        public const int SEGMENT_ID_PREDICTED_CONTEXTS = 3;
        /// <summary>Index for quantizer segment feature</summary>
        public const int SEG_LVL_ALT_Q = 0;
        /// <summary>Index for global mv feature</summary>
        public const int SEG_LVL_GLOBALMV = 2;
        /// <summary>Number of segment features</summary>
        public const int SEG_LVL_MAX = 3;
        /// <summary>Index for skip segment feature</summary>
        public const int SEG_LVL_SKIP = 1;
        /// <summary>Value that indicates the force_integer_mv syntax element is coded</summary>
        public const int SELECT_INTEGER_MV = 2;
        /// <summary>Value that indicates the allow_screen_content_tools syntax element is coded</summary>
        public const int SELECT_SCREEN_CONTENT_TOOLS = 2;
        /// <summary>Number of contexts for coeff_base for luma</summary>
        public const int SIG_COEF_CONTEXTS = 20;
        /// <summary>Number of contexts for coeff_base_bob</summary>
        public const int SIG_COEF_CONTEXTS_BOB = 3;
        /// <summary>Number of contexts for coeff_base_eob</summary>
        public const int SIG_COEF_CONTEXTS_EOB = 4;
        /// <summary>Number of contexts for coeff_base for chroma</summary>
        public const int SIG_COEF_CONTEXTS_UV = 12;
        /// <summary>Maximum number of context samples to be used in determining the context index for coeff_base and coeff_base_eob .</summary>
        public const int SIG_REF_DIFF_OFFSET_NUM = 5;
        /// <summary>Use translation or global motion compensation</summary>
        public const int SIMPLE = 0;
        /// <summary>Number of contexts for single_mode</summary>
        public const int SINGLE_MODE_CONTEXTS = 5;
        /// <summary>Number of contexts for decoding skip</summary>
        public const int SKIP_CONTEXTS = 6;
        /// <summary>Number of contexts for decoding skip_mode</summary>
        public const int SKIP_MODE_CONTEXTS = 3;
        /// <summary>Number of contexts for do_square_split syntax element</summary>
        public const int SQUARE_SPLIT_CONTEXTS = 8;
        /// <summary>Number of secondary transform types</summary>
        public const int STX_TYPES = 4;
        /// <summary>Number of bits of precision when choosing an inter prediction filter kernel</summary>
        public const int SUBPEL_BITS = 4;
        /// <summary>( 1 << SUBPEL_BITS ) - 1</summary>
        public const int SUBPEL_MASK = 15;
        /// <summary>Number of contexts for tip_mode</summary>
        public const int TIP_CONTEXTS = 3;
        /// <summary>Stack size for motion field motion vectors related to TIP</summary>
        public const int TIP_MFMV_STACK_SIZE = 3;
        /// <summary>Number of different angle deltas</summary>
        public const int TOTAL_ANGLE_DELTA_COUNT = 7;
        /// <summary>Number of contexts for all_zero per group</summary>
        public const int TXB_SKIP_CONTEXTS = 10;
        /// <summary>Number of groups of transform split types</summary>
        public const int TXFM_SPLIT_GROUP = 9;
        /// <summary>Transform class for transform types performing non-identity transforms in both directions</summary>
        public const int TX_CLASS_2D = 0;
        /// <summary>Transform class for transforms performing only a horizontal non-identity transform</summary>
        public const int TX_CLASS_HORIZ = 1;
        /// <summary>Transform class for transforms performing only a vertical non-identity transform</summary>
        public const int TX_CLASS_VERT = 2;
        /// <summary>Number of contexts for tx_partition_type</summary>
        public const int TX_PARTITION_TYPE_NUM = 7;
        /// <summary>Number of values (not equal to BLOCK_INVALID ) in the output range of Size_To_Tx_Type_Group_Vert_And_Horz</summary>
        public const int TX_PARTITION_TYPE_NUM_VERT_AND_HORZ = 14;
        /// <summary>Number of values (not equal to BLOCK_INVALID ) in the output range of Size_To_Tx_Type_Group_Vert_Or_Horz</summary>
        public const int TX_PARTITION_TYPE_NUM_VERT_OR_HORZ = 3;
        /// <summary>Number of inter transform set types</summary>
        public const int TX_SET_TYPES_INTER = 9;
        /// <summary>Number of intra transform set types</summary>
        public const int TX_SET_TYPES_INTRA = 7;
        /// <summary>Number of square transform sizes</summary>
        public const int TX_SIZES = 5;
        /// <summary>Number of transform sizes (including non-square sizes)</summary>
        public const int TX_SIZES_ALL = 25;
        /// <summary>Number of inverse transform types</summary>
        public const int TX_TYPES = 16;
        /// <summary>Number of values for uv_mode when chroma from luma is allowed</summary>
        public const int UV_INTRA_MODES_CFL_ALLOWED = 14;
        /// <summary>Number of values for uv_mode when chroma from luma is not allowed</summary>
        public const int UV_INTRA_MODES_CFL_NOT_ALLOWED = 13;
        /// <summary>Number of contexts for uv_mode</summary>
        public const int UV_MODE_CONTEXTS = 2;
        /// <summary>Inverse transform rows with identity and columns with ADST</summary>
        public const int V_ADST = 12;
        /// <summary>Inverse transform rows with identity and columns with DCT</summary>
        public const int V_DCT = 10;
        /// <summary>Inverse transform rows with identity and columns with FLIPADST</summary>
        public const int V_FLIPADST = 14;
        /// <summary>Number of contexts for all_zero for the V plane</summary>
        public const int V_TXB_SKIP_CONTEXTS = 12;
        /// <summary>Threshold used in WAIP</summary>
        public const int WAIP_WH_RATIO_2_THRES = 61;
        /// <summary>Threshold used in WAIP</summary>
        public const int WAIP_WH_RATIO_4_THRES = 73;
        /// <summary>Threshold used in WAIP</summary>
        public const int WAIP_WH_RATIO_8_THRES = 82;
        /// <summary>Threshold used in WAIP</summary>
        public const int WAIP_WH_RATIO_16_THRES = 86;
        /// <summary>Number of extra bits of precision in warped filtering</summary>
        public const int WARPEDDIFF_PREC_BITS = 10;
        /// <summary>Internal precision of warped motion models</summary>
        public const int WARPEDMODEL_PREC_BITS = 16;
        /// <summary>Clamping value used for translation components of warp</summary>
        public const int WARPEDMODEL_TRANS_CLAMP = 1 << 27;
        /// <summary>Number of phases used in warped filtering</summary>
        public const int WARPEDPIXEL_PREC_SHIFTS = 1 << 6;
        /// <summary>Number of contexts when decoding is_warp</summary>
        public const int WARPMV_MODE_CONTEXT = 5;
        /// <summary>Number of contexts when decoding use_local_warp</summary>
        public const int WARP_CAUSAL_MODE_CTX = 4;
        /// <summary>Number of values for warp_delta_param_high</summary>
        public const int WARP_DELTA_NUM_SYMBOLS_HIGH = 8;
        /// <summary>Number of values for warp_delta_param_low</summary>
        public const int WARP_DELTA_NUM_SYMBOLS_LOW = 8;
        /// <summary>Shift to apply to warp_delta_param</summary>
        public const int WARP_DELTA_STEP_BITS = 10;
        /// <summary>Size of the parameter bank for warp</summary>
        public const int WARP_PARAM_BANK_SIZE = 4;
        /// <summary>Rounding bitwidth for the parameters to the shear process</summary>
        public const int WARP_PARAM_REDUCE_BITS = 6;
        /// <summary>Number of angles for the wedge mask process</summary>
        public const int WEDGE_ANGLES = 20;
        /// <summary>Size of table lookup in the wedge mask process</summary>
        public const int WEDGE_BLD_LUT_SIZE = 32;
        /// <summary>Value indicating a sharp boundary</summary>
        public const int WEDGE_BOUNDARY_SHARP = 0;
        /// <summary>Value indicating a smooth boundary</summary>
        public const int WEDGE_BOUNDARY_SMOOTH = 1;
        /// <summary>Number of different boundary types</summary>
        public const int WEDGE_BOUNDARY_TYPES = 2;
        /// <summary>Number of types of wedge</summary>
        public const int WEDGE_TYPES = 68;
        /// <summary>Number of Wiener filter coefficients to read</summary>
        public const int WIENER_COEFFS = 3;
        /// <summary>Number of chroma non-separable Wiener filter coefficients</summary>
        public const int WIENER_NS_CHROMA_COEFFS = 18;
        /// <summary>Number of classes of non-separable Wiener filter coefficients</summary>
        public const int WIENER_NS_CLASSES = 16;
        /// <summary>Number of luma non-separable Wiener filter coefficients</summary>
        public const int WIENER_NS_LUMA_COEFFS = 16;
        /// <summary>Number of planes of non-separable Wiener filter coefficients</summary>
        public const int WIENER_NS_PLANES = 3;
        /// <summary>Number of bits used in non-separable Wiener filter coefficients</summary>
        public const int WIENER_NS_PREC_BITS = 7;
        /// <summary>Number of short non-separable Wiener filter coefficients</summary>
        public const int WIENER_NS_SHORT_COEFFS = 6;
        /// <summary>Number of chroma non-separable Wiener filter taps</summary>
        public const int WIENER_NS_TAPS_UV = 12;
        /// <summary>Number of luma non-separable Wiener filter taps</summary>
        public const int WIENER_NS_TAPS_Y = 32;
        /// <summary>Number of contexts for y_mode_index and y_second_mode ↑ Back to Table of Contents 4. Conventions</summary>
        public const int Y_MODE_CONTEXTS = 3;
        /// <summary>Table 7.1: 1D transform type values and names</summary>
        public const int ADST = 2;
        /// <summary>metadata_necessity_idc | Name | Description</summary>
        public const int ADVISORY = 2;
        /// <summary>Table 6.9: Auxiliary type values for LCR embedded layers</summary>
        public const int ALPHA_AUX = 0;
        /// <summary>Table 6.8: Layer type values for LCR embedded layers</summary>
        public const int AUX_LAYER = 1;
        /// <summary>Table 6.11: Specifies the representation description and coding of the atlas segments</summary>
        public const int BASIC_ATLAS = 1;
        /// <summary>muh_persistence_idc | Name | Description</summary>
        public const int BASIC_PERSISTENCE = 1;
        /// <summary>interpolation_filter | Name of interpolation_filter</summary>
        public const int BILINEAR = 3;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_128X128 = 15;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_128X256 = 16;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_128X64 = 14;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_16X16 = 6;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_16X32 = 7;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_16X4 = 20;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_16X64 = 23;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_16X8 = 5;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_256X128 = 17;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_256X256 = 18;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_32X16 = 8;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_32X32 = 9;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_32X4 = 26;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_32X64 = 10;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_32X8 = 22;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_4X16 = 19;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_4X32 = 25;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_4X4 = 0;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_4X8 = 1;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_64X128 = 13;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_64X16 = 24;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_64X32 = 11;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_64X64 = 12;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_64X8 = 28;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_8X16 = 4;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_8X32 = 21;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_8X4 = 2;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_8X64 = 27;
        /// <summary>Table 6.22: subSize values for different partition types</summary>
        public const int BLOCK_8X8 = 3;
        /// <summary>Table 6.21: bru_mode values and interpretations</summary>
        public const int BRU_ACTIVE = 2;
        /// <summary>Table 6.21: bru_mode values and interpretations</summary>
        public const int BRU_INACTIVE = 0;
        /// <summary>Table 6.21: bru_mode values and interpretations</summary>
        public const int BRU_SUPPORT = 1;
        /// <summary>cctx_type | Name of cctx_type</summary>
        public const int CCTX_30 = 2;
        /// <summary>cctx_type | Name of cctx_type</summary>
        public const int CCTX_45 = 1;
        /// <summary>cctx_type | Name of cctx_type</summary>
        public const int CCTX_60 = 3;
        /// <summary>cctx_type | Name of cctx_type</summary>
        public const int CCTX_MINUS30 = 5;
        /// <summary>cctx_type | Name of cctx_type</summary>
        public const int CCTX_MINUS45 = 4;
        /// <summary>cctx_type | Name of cctx_type</summary>
        public const int CCTX_MINUS60 = 6;
        /// <summary>cctx_type | Name of cctx_type</summary>
        public const int CCTX_NONE = 0;
        /// <summary>Table 6.26: cfl_index values and names</summary>
        public const int CFL_DERIVED_ALPHA = 1;
        /// <summary>Table 6.26: cfl_index values and names</summary>
        public const int CFL_EXPLICIT = 0;
        /// <summary>Table 6.26: cfl_index values and names</summary>
        public const int CFL_MULTI = 2;
        /// <summary>Table 6.2: Chroma format indicator values</summary>
        public const int CHROMA_FORMAT_400 = 1;
        /// <summary>Table 6.2: Chroma format indicator values</summary>
        public const int CHROMA_FORMAT_420 = 0;
        /// <summary>Table 6.2: Chroma format indicator values</summary>
        public const int CHROMA_FORMAT_422 = 3;
        /// <summary>Table 6.2: Chroma format indicator values</summary>
        public const int CHROMA_FORMAT_444 = 2;
        /// <summary>TreeType | Name of TreeType</summary>
        public const int CHROMA_PART = 2;
        /// <summary>compound_type | Name of compound_type</summary>
        public const int COMPOUND_AVERAGE = 2;
        /// <summary>compound_type | Name of compound_type</summary>
        public const int COMPOUND_DIFFWTD = 1;
        /// <summary>compound_type | Name of compound_type</summary>
        public const int COMPOUND_INTRA = 3;
        /// <summary>comp_mode | Name of comp_mode</summary>
        public const int COMPOUND_REFERENCE = 1;
        /// <summary>compound_type | Name of compound_type</summary>
        public const int COMPOUND_WEDGE = 0;
        /// <summary>Table 6.14: ops_color_primaries values and names</summary>
        public const int CP_BT_2020 = 9;
        /// <summary>Table 6.14: ops_color_primaries values and names</summary>
        public const int CP_BT_470_B_G = 5;
        /// <summary>Table 6.14: ops_color_primaries values and names</summary>
        public const int CP_BT_470_M = 4;
        /// <summary>Table 6.14: ops_color_primaries values and names</summary>
        public const int CP_BT_601 = 6;
        /// <summary>Table 6.14: ops_color_primaries values and names</summary>
        public const int CP_BT_709 = 1;
        /// <summary>Table 6.14: ops_color_primaries values and names</summary>
        public const int CP_EBU_3213 = 22;
        /// <summary>Table 6.14: ops_color_primaries values and names</summary>
        public const int CP_GENERIC_FILM = 8;
        /// <summary>Table 6.14: ops_color_primaries values and names</summary>
        public const int CP_SMPTE_240 = 7;
        /// <summary>Table 6.14: ops_color_primaries values and names</summary>
        public const int CP_SMPTE_431 = 11;
        /// <summary>Table 6.14: ops_color_primaries values and names</summary>
        public const int CP_SMPTE_432 = 12;
        /// <summary>Table 6.14: ops_color_primaries values and names</summary>
        public const int CP_UNSPECIFIED = 2;
        /// <summary>Table 6.14: ops_color_primaries values and names</summary>
        public const int CP_XYZ = 10;
        /// <summary>ci_chroma_sample_position_(top/bottom) | Name of chroma sample position | Meaning for 4:2:2 (offsets from (0,0) luma sample) | Meaning for 4:2:0 (offsets from (0,0) luma sample)</summary>
        public const int CSP_BOTTOM = 5;
        /// <summary>ci_chroma_sample_position_(top/bottom) | Name of chroma sample position | Meaning for 4:2:2 (offsets from (0,0) luma sample) | Meaning for 4:2:0 (offsets from (0,0) luma sample)</summary>
        public const int CSP_BOTTOMLEFT = 4;
        /// <summary>ci_chroma_sample_position_(top/bottom) | Name of chroma sample position | Meaning for 4:2:2 (offsets from (0,0) luma sample) | Meaning for 4:2:0 (offsets from (0,0) luma sample)</summary>
        public const int CSP_CENTER = 1;
        /// <summary>ci_chroma_sample_position_(top/bottom) | Name of chroma sample position | Meaning for 4:2:2 (offsets from (0,0) luma sample) | Meaning for 4:2:0 (offsets from (0,0) luma sample)</summary>
        public const int CSP_LEFT = 0;
        /// <summary>ci_chroma_sample_position_(top/bottom) | Name of chroma sample position | Meaning for 4:2:2 (offsets from (0,0) luma sample) | Meaning for 4:2:0 (offsets from (0,0) luma sample)</summary>
        public const int CSP_TOP = 3;
        /// <summary>ci_chroma_sample_position_(top/bottom) | Name of chroma sample position | Meaning for 4:2:2 (offsets from (0,0) luma sample) | Meaning for 4:2:0 (offsets from (0,0) luma sample)</summary>
        public const int CSP_TOPLEFT = 2;
        /// <summary>ci_chroma_sample_position_(top/bottom) | Name of chroma sample position | Meaning for 4:2:2 (offsets from (0,0) luma sample) | Meaning for 4:2:0 (offsets from (0,0) luma sample)</summary>
        public const int CSP_UNSPECIFIED = 6;
        /// <summary>YMode | Name of YMode</summary>
        public const int D113_PRED = 5;
        /// <summary>YMode | Name of YMode</summary>
        public const int D135_PRED = 4;
        /// <summary>YMode | Name of YMode</summary>
        public const int D157_PRED = 6;
        /// <summary>YMode | Name of YMode</summary>
        public const int D203_PRED = 7;
        /// <summary>YMode | Name of YMode</summary>
        public const int D45_PRED = 3;
        /// <summary>YMode | Name of YMode</summary>
        public const int D67_PRED = 8;
        /// <summary>YMode | Name of YMode</summary>
        public const int DC_PRED = 0;
        /// <summary>Table 7.1: 1D transform type values and names</summary>
        public const int DDTX = 4;
        /// <summary>Table 6.9: Auxiliary type values for LCR embedded layers</summary>
        public const int DEPTH_AUX = 1;
        /// <summary>Table 6.4: DrlReorder values and names</summary>
        public const int DRL_REORDER_ALWAYS = 2;
        /// <summary>Table 6.4: DrlReorder values and names</summary>
        public const int DRL_REORDER_CONSTRAINT = 1;
        /// <summary>Table 6.4: DrlReorder values and names</summary>
        public const int DRL_REORDER_DISABLED = 0;
        /// <summary>interpolation_filter | Name of interpolation_filter</summary>
        public const int EIGHTTAP = 0;
        /// <summary>interpolation_filter | Name of interpolation_filter</summary>
        public const int EIGHTTAP_SHARP = 2;
        /// <summary>interpolation_filter | Name of interpolation_filter</summary>
        public const int EIGHTTAP_SMOOTH = 1;
        /// <summary>Table 6.11: Specifies the representation description and coding of the atlas segments</summary>
        public const int ENHANCED_ATLAS = 0;
        /// <summary>muh_persistence_idc | Name | Description</summary>
        public const int ENHANCED_PERSISTENCE = 3;
        /// <summary>Table 7.1: 1D transform type values and names</summary>
        public const int FDDT = 5;
        /// <summary>Table 7.1: 1D transform type values and names</summary>
        public const int FDST = 3;
        /// <summary>Table 6.9: Auxiliary type values for LCR embedded layers</summary>
        public const int GAIN_MAP_AUX = 3;
        /// <summary>YMode | Name of YMode</summary>
        public const int GLOBALMV = 15;
        /// <summary>YMode | Name of YMode</summary>
        public const int GLOBAL_GLOBALMV = 22;
        /// <summary>muh_persistence_idc | Name | Description</summary>
        public const int GLOBAL_PERSISTENCE = 0;
        /// <summary>YMode | Name of YMode</summary>
        public const int H_PRED = 2;
        /// <summary>Table 6.24: interintra_mode values and names</summary>
        public const int II_DC_PRED = 0;
        /// <summary>Table 6.24: interintra_mode values and names</summary>
        public const int II_H_PRED = 2;
        /// <summary>Table 6.24: interintra_mode values and names</summary>
        public const int II_SMOOTH_PRED = 3;
        /// <summary>Table 6.24: interintra_mode values and names</summary>
        public const int II_V_PRED = 1;
        /// <summary>FrameType | Name of FrameType</summary>
        public const int INTER_FRAME = 1;
        /// <summary>RefFrame[ 0 ] | Name of ref_frame</summary>
        public const int INTRA_FRAME = 8;
        /// <summary>FrameType | Name of FrameType</summary>
        public const int INTRA_ONLY_FRAME = 2;
        /// <summary>YMode | Name of YMode</summary>
        public const int JOINT_NEWMV = 24;
        /// <summary>FrameType | Name of FrameType</summary>
        public const int KEY_FRAME = 0;
        /// <summary>muh_layer_idc | Name | Description</summary>
        public const int LAYER_CURRENT = 2;
        /// <summary>muh_layer_idc | Name | Description</summary>
        public const int LAYER_GLOBAL = 1;
        /// <summary>muh_layer_idc | Name | Description</summary>
        public const int LAYER_UNSPECIFIED = 0;
        /// <summary>muh_layer_idc | Name | Description</summary>
        public const int LAYER_VALUES = 3;
        /// <summary>TreeType | Name of TreeType</summary>
        public const int LUMA_PART = 1;
        /// <summary>Table 6.15: ops_matrix_coefficients values and names</summary>
        public const int MC_BT_2020_CL = 10;
        /// <summary>Table 6.15: ops_matrix_coefficients values and names</summary>
        public const int MC_BT_2020_NCL = 9;
        /// <summary>Table 6.15: ops_matrix_coefficients values and names</summary>
        public const int MC_BT_470_B_G = 5;
        /// <summary>Table 6.15: ops_matrix_coefficients values and names</summary>
        public const int MC_BT_601 = 6;
        /// <summary>Table 6.15: ops_matrix_coefficients values and names</summary>
        public const int MC_BT_709 = 1;
        /// <summary>Table 6.15: ops_matrix_coefficients values and names</summary>
        public const int MC_CHROMAT_CL = 13;
        /// <summary>Table 6.15: ops_matrix_coefficients values and names</summary>
        public const int MC_CHROMAT_NCL = 12;
        /// <summary>Table 6.15: ops_matrix_coefficients values and names</summary>
        public const int MC_FCC = 4;
        /// <summary>Table 6.15: ops_matrix_coefficients values and names</summary>
        public const int MC_ICTCP = 14;
        /// <summary>Table 6.15: ops_matrix_coefficients values and names</summary>
        public const int MC_IDENTITY = 0;
        /// <summary>Table 6.15: ops_matrix_coefficients values and names</summary>
        public const int MC_IPT_C2 = 15;
        /// <summary>Table 6.15: ops_matrix_coefficients values and names</summary>
        public const int MC_RESERVED_3 = 3;
        /// <summary>Table 6.15: ops_matrix_coefficients values and names</summary>
        public const int MC_SMPTE_2085 = 11;
        /// <summary>Table 6.15: ops_matrix_coefficients values and names</summary>
        public const int MC_SMPTE_240 = 7;
        /// <summary>Table 6.15: ops_matrix_coefficients values and names</summary>
        public const int MC_SMPTE_YCGCO = 8;
        /// <summary>Table 6.15: ops_matrix_coefficients values and names</summary>
        public const int MC_UNSPECIFIED = 2;
        /// <summary>Table 6.15: ops_matrix_coefficients values and names</summary>
        public const int MC_YCGCO_RE = 16;
        /// <summary>Table 6.15: ops_matrix_coefficients values and names</summary>
        public const int MC_YCGCO_RO = 17;
        /// <summary>Table 6.17: metadata_type values and layer-specific status</summary>
        public const int METADATA_TYPE_BANDING_HINTS = 6;
        /// <summary>Table 6.17: metadata_type values and layer-specific status</summary>
        public const int METADATA_TYPE_DECODED_FRAME_HASH = 5;
        /// <summary>Table 6.17: metadata_type values and layer-specific status</summary>
        public const int METADATA_TYPE_HDR_CLL = 1;
        /// <summary>Table 6.17: metadata_type values and layer-specific status</summary>
        public const int METADATA_TYPE_HDR_MDCV = 2;
        /// <summary>Table 6.17: metadata_type values and layer-specific status</summary>
        public const int METADATA_TYPE_ICC_PROFILE = 7;
        /// <summary>Table 6.17: metadata_type values and layer-specific status</summary>
        public const int METADATA_TYPE_ITUT_T35 = 3;
        /// <summary>Table 6.17: metadata_type values and layer-specific status</summary>
        public const int METADATA_TYPE_SCAN_TYPE = 8;
        /// <summary>Table 6.17: metadata_type values and layer-specific status</summary>
        public const int METADATA_TYPE_TEMPORAL_POINT_INFO = 9;
        /// <summary>Table 6.17: metadata_type values and layer-specific status</summary>
        public const int METADATA_TYPE_TIMECODE = 4;
        /// <summary>Table 6.17: metadata_type values and layer-specific status</summary>
        public const int METADATA_TYPE_USER_DATA_UNREGISTERED = 10;
        /// <summary>metadata_necessity_idc | Name | Description</summary>
        public const int MIXED = 3;
        /// <summary>Table 6.16: metadata_application_id values and descriptions</summary>
        public const int MOBILE = 2;
        /// <summary>Table 6.16: metadata_application_id values and descriptions</summary>
        public const int MOBILE_OR_TV = 1;
        /// <summary>Table 6.11: Specifies the representation description and coding of the atlas segments</summary>
        public const int MULTISTREAM_ALPHA_ATLAS = 4;
        /// <summary>Table 6.11: Specifies the representation description and coding of the atlas segments</summary>
        public const int MULTISTREAM_ATLAS = 3;
        /// <summary>mv_joint | Name of mv_joint | Changes row | Changes col</summary>
        public const int MV_JOINT_HNZVNZ = 3;
        /// <summary>mv_joint | Name of mv_joint | Changes row | Changes col</summary>
        public const int MV_JOINT_HNZVZ = 1;
        /// <summary>mv_joint | Name of mv_joint | Changes row | Changes col</summary>
        public const int MV_JOINT_HZVNZ = 2;
        /// <summary>mv_joint | Name of mv_joint | Changes row | Changes col</summary>
        public const int MV_JOINT_ZERO = 0;
        /// <summary>Table 6.19: FrameMvPrecision values and names</summary>
        public const int MV_PRECISION_EIGHTH_PEL = 6;
        /// <summary>Table 6.19: FrameMvPrecision values and names</summary>
        public const int MV_PRECISION_EIGHT_PEL = 0;
        /// <summary>Table 6.19: FrameMvPrecision values and names</summary>
        public const int MV_PRECISION_FOUR_PEL = 1;
        /// <summary>Table 6.19: FrameMvPrecision values and names</summary>
        public const int MV_PRECISION_HALF_PEL = 4;
        /// <summary>Table 6.19: FrameMvPrecision values and names</summary>
        public const int MV_PRECISION_ONE_PEL = 3;
        /// <summary>Table 6.19: FrameMvPrecision values and names</summary>
        public const int MV_PRECISION_QUARTER_PEL = 5;
        /// <summary>Table 6.19: FrameMvPrecision values and names</summary>
        public const int MV_PRECISION_TWO_PEL = 2;
        /// <summary>YMode | Name of YMode</summary>
        public const int NEARMV = 14;
        /// <summary>YMode | Name of YMode</summary>
        public const int NEAR_NEARMV = 19;
        /// <summary>YMode | Name of YMode</summary>
        public const int NEAR_NEWMV = 20;
        /// <summary>metadata_necessity_idc | Name | Description</summary>
        public const int NECESSARY = 1;
        /// <summary>YMode | Name of YMode</summary>
        public const int NEWMV = 16;
        /// <summary>YMode | Name of YMode</summary>
        public const int NEW_NEARMV = 21;
        /// <summary>YMode | Name of YMode</summary>
        public const int NEW_NEWMV = 23;
        /// <summary>RefFrame[ 1 ] | Name of ref_frame</summary>
        public const int NONE = -1;
        /// <summary>muh_persistence_idc | Name | Description</summary>
        public const int NO_PERSISTENCE = 2;
        /// <summary>Table 6.19: FrameMvPrecision values and names</summary>
        public const int NUM_MV_PRECISIONS = 7;
        /// <summary>Table 6.1: OBU types and their layer-specific status</summary>
        public const int OBU_ATLAS_SEGMENT = 17;
        /// <summary>Table 6.1: OBU types and their layer-specific status</summary>
        public const int OBU_BRIDGE_FRAME = 19;
        /// <summary>Table 6.1: OBU types and their layer-specific status</summary>
        public const int OBU_BUFFER_REMOVAL_TIMING = 15;
        /// <summary>Table 6.1: OBU types and their layer-specific status</summary>
        public const int OBU_CLOSED_LOOP_KEY = 4;
        /// <summary>Table 6.1: OBU types and their layer-specific status</summary>
        public const int OBU_CONTENT_INTERPRETATION = 24;
        /// <summary>Table 6.1: OBU types and their layer-specific status</summary>
        public const int OBU_FILM_GRAIN = 23;
        /// <summary>Table 6.1: OBU types and their layer-specific status</summary>
        public const int OBU_LAYER_CONFIGURATION_RECORD = 16;
        /// <summary>Table 6.1: OBU types and their layer-specific status</summary>
        public const int OBU_LEADING_SEF = 11;
        /// <summary>Table 6.1: OBU types and their layer-specific status</summary>
        public const int OBU_LEADING_TILE_GROUP = 6;
        /// <summary>Table 6.1: OBU types and their layer-specific status</summary>
        public const int OBU_LEADING_TIP = 13;
        /// <summary>Table 6.1: OBU types and their layer-specific status</summary>
        public const int OBU_METADATA_GROUP = 9;
        /// <summary>Table 6.1: OBU types and their layer-specific status</summary>
        public const int OBU_METADATA_SHORT = 8;
        /// <summary>Table 6.1: OBU types and their layer-specific status</summary>
        public const int OBU_MSDO = 20;
        /// <summary>Table 6.1: OBU types and their layer-specific status</summary>
        public const int OBU_MULTI_FRAME_HEADER = 3;
        /// <summary>Table 6.1: OBU types and their layer-specific status</summary>
        public const int OBU_OPEN_LOOP_KEY = 5;
        /// <summary>Table 6.1: OBU types and their layer-specific status</summary>
        public const int OBU_OPERATING_POINT_SET = 18;
        /// <summary>Table 6.1: OBU types and their layer-specific status</summary>
        public const int OBU_PADDING = 25;
        /// <summary>Table 6.1: OBU types and their layer-specific status</summary>
        public const int OBU_QUANTIZATION_MATRIX = 22;
        /// <summary>Table 6.1: OBU types and their layer-specific status</summary>
        public const int OBU_RAS_FRAME = 21;
        /// <summary>Table 6.1: OBU types and their layer-specific status</summary>
        public const int OBU_REGULAR_SEF = 12;
        /// <summary>Table 6.1: OBU types and their layer-specific status</summary>
        public const int OBU_REGULAR_TILE_GROUP = 7;
        /// <summary>Table 6.1: OBU types and their layer-specific status</summary>
        public const int OBU_REGULAR_TIP = 14;
        /// <summary>Table 6.1: OBU types and their layer-specific status</summary>
        public const int OBU_SEQUENCE_HEADER = 1;
        /// <summary>Table 6.1: OBU types and their layer-specific status</summary>
        public const int OBU_SWITCH = 10;
        /// <summary>Table 6.1: OBU types and their layer-specific status</summary>
        public const int OBU_TEMPORAL_DELIMITER = 2;
        /// <summary>TxMode | Name of TxMode</summary>
        public const int ONLY_4X4 = 0;
        /// <summary>Table 6.12: ops_intent values and labels</summary>
        public const int OPSI_GAIN_MAP = 5;
        /// <summary>Table 6.12: ops_intent values and labels</summary>
        public const int OPSI_MULTIVIEW = 6;
        /// <summary>Table 6.12: ops_intent values and labels</summary>
        public const int OPSI_SCALABILITY = 1;
        /// <summary>Table 6.12: ops_intent values and labels</summary>
        public const int OPSI_STEREO = 2;
        /// <summary>Table 6.12: ops_intent values and labels</summary>
        public const int OPSI_TEXTURE_ALPHA = 3;
        /// <summary>Table 6.12: ops_intent values and labels</summary>
        public const int OPSI_TEXTURE_DEPTH = 4;
        /// <summary>Table 6.12: ops_intent values and labels</summary>
        public const int OPSI_UNSPECIFIED = 0;
        /// <summary>YMode | Name of YMode</summary>
        public const int PAETH_PRED = 12;
        /// <summary>partition | Name of partition</summary>
        public const int PARTITION_HORZ = 1;
        /// <summary>partition | Name of partition</summary>
        public const int PARTITION_HORZ_3 = 3;
        /// <summary>partition | Name of partition</summary>
        public const int PARTITION_HORZ_4A = 5;
        /// <summary>partition | Name of partition</summary>
        public const int PARTITION_HORZ_4B = 6;
        /// <summary>partition | Name of partition</summary>
        public const int PARTITION_NONE = 0;
        /// <summary>partition | Name of partition</summary>
        public const int PARTITION_SPLIT = 9;
        /// <summary>partition | Name of partition</summary>
        public const int PARTITION_VERT = 2;
        /// <summary>partition | Name of partition</summary>
        public const int PARTITION_VERT_3 = 4;
        /// <summary>partition | Name of partition</summary>
        public const int PARTITION_VERT_4A = 7;
        /// <summary>partition | Name of partition</summary>
        public const int PARTITION_VERT_4B = 8;
        /// <summary>Table 6.5: Optical flow signaling modes</summary>
        public const int REFINE_ALL = 2;
        /// <summary>Table 6.5: Optical flow signaling modes</summary>
        public const int REFINE_AUTO = 3;
        /// <summary>Table 6.5: Optical flow signaling modes</summary>
        public const int REFINE_NONE = 0;
        /// <summary>Table 6.5: Optical flow signaling modes</summary>
        public const int REFINE_SWITCHABLE = 1;
        /// <summary>FrameRestorationType | Name of FrameRestorationType</summary>
        public const int RESTORE_NONE = 0;
        /// <summary>FrameRestorationType | Name of FrameRestorationType</summary>
        public const int RESTORE_PC_WIENER = 1;
        /// <summary>FrameRestorationType | Name of FrameRestorationType</summary>
        public const int RESTORE_SWITCHABLE = 3;
        /// <summary>FrameRestorationType | Name of FrameRestorationType</summary>
        public const int RESTORE_WIENER_NONSEP = 2;
        /// <summary>Table 6.9: Auxiliary type values for LCR embedded layers</summary>
        public const int SEGMENTATION_AUX = 2;
        /// <summary>TreeType | Name of TreeType</summary>
        public const int SHARED_PART = 0;
        /// <summary>Table 6.11: Specifies the representation description and coding of the atlas segments</summary>
        public const int SINGLE_ATLAS = 2;
        /// <summary>comp_mode | Name of comp_mode</summary>
        public const int SINGLE_REFERENCE = 0;
        /// <summary>YMode | Name of YMode</summary>
        public const int SMOOTH_H_PRED = 11;
        /// <summary>YMode | Name of YMode</summary>
        public const int SMOOTH_PRED = 9;
        /// <summary>YMode | Name of YMode</summary>
        public const int SMOOTH_V_PRED = 10;
        /// <summary>interpolation_filter | Name of interpolation_filter</summary>
        public const int SWITCHABLE = 4;
        /// <summary>FrameType | Name of FrameType</summary>
        public const int SWITCH_FRAME = 3;
        /// <summary>ops_transfer_characteristics | Name of transfer characteristics | Description</summary>
        public const int TC_BT_1361 = 12;
        /// <summary>ops_transfer_characteristics | Name of transfer characteristics | Description</summary>
        public const int TC_BT_2020_10_BIT = 14;
        /// <summary>ops_transfer_characteristics | Name of transfer characteristics | Description</summary>
        public const int TC_BT_470_B_G = 5;
        /// <summary>ops_transfer_characteristics | Name of transfer characteristics | Description</summary>
        public const int TC_BT_470_M = 4;
        /// <summary>ops_transfer_characteristics | Name of transfer characteristics | Description</summary>
        public const int TC_BT_601 = 6;
        /// <summary>ops_transfer_characteristics | Name of transfer characteristics | Description</summary>
        public const int TC_BT_709 = 1;
        /// <summary>ops_transfer_characteristics | Name of transfer characteristics | Description</summary>
        public const int TC_HLG = 18;
        /// <summary>ops_transfer_characteristics | Name of transfer characteristics | Description</summary>
        public const int TC_IEC_61966 = 11;
        /// <summary>ops_transfer_characteristics | Name of transfer characteristics | Description</summary>
        public const int TC_LINEAR = 8;
        /// <summary>ops_transfer_characteristics | Name of transfer characteristics | Description</summary>
        public const int TC_LOG_100 = 9;
        /// <summary>ops_transfer_characteristics | Name of transfer characteristics | Description</summary>
        public const int TC_LOG_100_SQRT10 = 10;
        /// <summary>ops_transfer_characteristics | Name of transfer characteristics | Description</summary>
        public const int TC_RESERVED_0 = 0;
        /// <summary>ops_transfer_characteristics | Name of transfer characteristics | Description</summary>
        public const int TC_RESERVED_3 = 3;
        /// <summary>ops_transfer_characteristics | Name of transfer characteristics | Description</summary>
        public const int TC_SMPTE_2084 = 16;
        /// <summary>ops_transfer_characteristics | Name of transfer characteristics | Description</summary>
        public const int TC_SMPTE_240 = 7;
        /// <summary>ops_transfer_characteristics | Name of transfer characteristics | Description</summary>
        public const int TC_SMPTE_428 = 17;
        /// <summary>ops_transfer_characteristics | Name of transfer characteristics | Description</summary>
        public const int TC_SRGB = 13;
        /// <summary>ops_transfer_characteristics | Name of transfer characteristics | Description</summary>
        public const int TC_UNSPECIFIED = 2;
        /// <summary>Table 6.8: Layer type values for LCR embedded layers</summary>
        public const int TEXTURE_LAYER = 0;
        /// <summary>RefFrame[ 0 ] | Name of ref_frame</summary>
        public const int TIP_FRAME = 7;
        /// <summary>Table 6.20: TipFrameMode values and names</summary>
        public const int TIP_FRAME_AS_OUTPUT = 2;
        /// <summary>Table 6.20: TipFrameMode values and names</summary>
        public const int TIP_FRAME_AS_REF = 1;
        /// <summary>Table 6.20: TipFrameMode values and names</summary>
        public const int TIP_FRAME_DISABLED = 0;
        /// <summary>TxSize | Name of TxSize</summary>
        public const int TX_16X16 = 2;
        /// <summary>TxSize | Name of TxSize</summary>
        public const int TX_16X32 = 9;
        /// <summary>TxSize | Name of TxSize</summary>
        public const int TX_16X4 = 14;
        /// <summary>TxSize | Name of TxSize</summary>
        public const int TX_16X64 = 17;
        /// <summary>TxSize | Name of TxSize</summary>
        public const int TX_16X8 = 8;
        /// <summary>TxSize | Name of TxSize</summary>
        public const int TX_32X16 = 10;
        /// <summary>TxSize | Name of TxSize</summary>
        public const int TX_32X32 = 3;
        /// <summary>TxSize | Name of TxSize</summary>
        public const int TX_32X4 = 20;
        /// <summary>TxSize | Name of TxSize</summary>
        public const int TX_32X64 = 11;
        /// <summary>TxSize | Name of TxSize</summary>
        public const int TX_32X8 = 16;
        /// <summary>TxSize | Name of TxSize</summary>
        public const int TX_4X16 = 13;
        /// <summary>TxSize | Name of TxSize</summary>
        public const int TX_4X32 = 19;
        /// <summary>TxSize | Name of TxSize</summary>
        public const int TX_4X4 = 0;
        /// <summary>TxSize | Name of TxSize</summary>
        public const int TX_4X64 = 23;
        /// <summary>TxSize | Name of TxSize</summary>
        public const int TX_4X8 = 5;
        /// <summary>TxSize | Name of TxSize</summary>
        public const int TX_64X16 = 18;
        /// <summary>TxSize | Name of TxSize</summary>
        public const int TX_64X32 = 12;
        /// <summary>TxSize | Name of TxSize</summary>
        public const int TX_64X4 = 24;
        /// <summary>TxSize | Name of TxSize</summary>
        public const int TX_64X64 = 4;
        /// <summary>TxSize | Name of TxSize</summary>
        public const int TX_64X8 = 22;
        /// <summary>TxSize | Name of TxSize</summary>
        public const int TX_8X16 = 7;
        /// <summary>TxSize | Name of TxSize</summary>
        public const int TX_8X32 = 15;
        /// <summary>TxSize | Name of TxSize</summary>
        public const int TX_8X4 = 6;
        /// <summary>TxSize | Name of TxSize</summary>
        public const int TX_8X64 = 21;
        /// <summary>TxSize | Name of TxSize</summary>
        public const int TX_8X8 = 1;
        /// <summary>TxSize | Name of TxSize</summary>
        public const int TX_INVALID = 255;
        /// <summary>TxMode | Name of TxMode</summary>
        public const int TX_MODE_LARGEST = 1;
        /// <summary>TxMode | Name of TxMode</summary>
        public const int TX_MODE_SELECT = 2;
        /// <summary>Table 6.23: txPartition values and names</summary>
        public const int TX_PARTITION_HORZ = 2;
        /// <summary>Table 6.23: txPartition values and names</summary>
        public const int TX_PARTITION_HORZ4 = 4;
        /// <summary>Table 6.23: txPartition values and names</summary>
        public const int TX_PARTITION_HORZ5 = 6;
        /// <summary>Table 6.23: txPartition values and names</summary>
        public const int TX_PARTITION_NONE = 0;
        /// <summary>Table 6.23: txPartition values and names</summary>
        public const int TX_PARTITION_SPLIT = 1;
        /// <summary>Table 6.23: txPartition values and names</summary>
        public const int TX_PARTITION_VERT = 3;
        /// <summary>Table 6.23: txPartition values and names</summary>
        public const int TX_PARTITION_VERT4 = 5;
        /// <summary>Table 6.23: txPartition values and names</summary>
        public const int TX_PARTITION_VERT5 = 7;
        /// <summary>is_inter | set | Name of transform set</summary>
        public const int TX_SET_DCTONLY = 0;
        /// <summary>is_inter | set | Name of transform set</summary>
        public const int TX_SET_DCT_IDTX = 1;
        /// <summary>is_inter | set | Name of transform set</summary>
        public const int TX_SET_DCT_IDTX_IDDCT = 1;
        /// <summary>is_inter | set | Name of transform set</summary>
        public const int TX_SET_HIGH_32 = 4;
        /// <summary>is_inter | set | Name of transform set</summary>
        public const int TX_SET_HIGH_64 = 2;
        /// <summary>is_inter | set | Name of transform set</summary>
        public const int TX_SET_INTER_1 = 1;
        /// <summary>is_inter | set | Name of transform set</summary>
        public const int TX_SET_INTER_2 = 1;
        /// <summary>is_inter | set | Name of transform set</summary>
        public const int TX_SET_INTRA_1 = 0;
        /// <summary>is_inter | set | Name of transform set</summary>
        public const int TX_SET_INTRA_2 = 0;
        /// <summary>is_inter | set | Name of transform set</summary>
        public const int TX_SET_WIDE_32 = 3;
        /// <summary>is_inter | set | Name of transform set</summary>
        public const int TX_SET_WIDE_64 = 1;
        /// <summary>metadata_necessity_idc | Name | Description</summary>
        public const int UNDEFINED = 0;
        /// <summary>mask_type | Name of mask_type</summary>
        public const int UNIFORM_45 = 0;
        /// <summary>mask_type | Name of mask_type</summary>
        public const int UNIFORM_45_INV = 1;
        /// <summary>Table 6.16: metadata_application_id values and descriptions</summary>
        public const int UNSPECIFIED = 0;
        /// <summary>UVMode | Name of UVMode</summary>
        public const int UV_CFL_PRED = 13;
        /// <summary>Table 6.10: View type values for LCR embedded layers</summary>
        public const int VIEW_CENTER = 1;
        /// <summary>Table 6.10: View type values for LCR embedded layers</summary>
        public const int VIEW_EXPLICIT = 4;
        /// <summary>Table 6.10: View type values for LCR embedded layers</summary>
        public const int VIEW_LEFT = 2;
        /// <summary>Table 6.10: View type values for LCR embedded layers</summary>
        public const int VIEW_RIGHT = 3;
        /// <summary>Table 6.10: View type values for LCR embedded layers</summary>
        public const int VIEW_UNSPECIFIED = 0;
        /// <summary>YMode | Name of YMode</summary>
        public const int V_PRED = 1;
        /// <summary>YMode | Name of YMode</summary>
        public const int WARPMV = 17;
        /// <summary>YMode | Name of YMode</summary>
        public const int WARP_NEWMV = 18;
        /// <summary>Table 6.16: metadata_application_id values and descriptions</summary>
        public const int WEARABLE = 5;
        /// <summary>Table 6.25: wedgeAngle values and names</summary>
        public const int WEDGE_0 = 0;
        /// <summary>Table 6.25: wedgeAngle values and names</summary>
        public const int WEDGE_117 = 6;
        /// <summary>Table 6.25: wedgeAngle values and names</summary>
        public const int WEDGE_135 = 7;
        /// <summary>Table 6.25: wedgeAngle values and names</summary>
        public const int WEDGE_14 = 1;
        /// <summary>Table 6.25: wedgeAngle values and names</summary>
        public const int WEDGE_153 = 8;
        /// <summary>Table 6.25: wedgeAngle values and names</summary>
        public const int WEDGE_166 = 9;
        /// <summary>Table 6.25: wedgeAngle values and names</summary>
        public const int WEDGE_180 = 10;
        /// <summary>Table 6.25: wedgeAngle values and names</summary>
        public const int WEDGE_194 = 11;
        /// <summary>Table 6.25: wedgeAngle values and names</summary>
        public const int WEDGE_207 = 12;
        /// <summary>Table 6.25: wedgeAngle values and names</summary>
        public const int WEDGE_225 = 13;
        /// <summary>Table 6.25: wedgeAngle values and names</summary>
        public const int WEDGE_243 = 14;
        /// <summary>Table 6.25: wedgeAngle values and names</summary>
        public const int WEDGE_27 = 2;
        /// <summary>Table 6.25: wedgeAngle values and names</summary>
        public const int WEDGE_270 = 15;
        /// <summary>Table 6.25: wedgeAngle values and names</summary>
        public const int WEDGE_297 = 16;
        /// <summary>Table 6.25: wedgeAngle values and names</summary>
        public const int WEDGE_315 = 17;
        /// <summary>Table 6.25: wedgeAngle values and names</summary>
        public const int WEDGE_333 = 18;
        /// <summary>Table 6.25: wedgeAngle values and names</summary>
        public const int WEDGE_346 = 19;
        /// <summary>Table 6.25: wedgeAngle values and names</summary>
        public const int WEDGE_45 = 3;
        /// <summary>Table 6.25: wedgeAngle values and names</summary>
        public const int WEDGE_63 = 4;
        /// <summary>Table 6.25: wedgeAngle values and names</summary>
        public const int WEDGE_90 = 5;
    }
}
