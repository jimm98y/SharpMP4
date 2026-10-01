namespace SharpVP9
{
    /// <summary>
    /// The constants of the VP9 Bitstream &amp; Decoding Process Specification v0.7, named as it names them: those of section 3,
    /// then the values its semantics name (7.2).
    /// </summary>
    public static class VP9Constants
    {
        // 3 Symbols (and abbreviated terms)
        public const int REFS_PER_FRAME = 3;
        public const int MV_FR_SIZE = 4;
        public const int MVREF_NEIGHBOURS = 8;
        public const int BLOCK_SIZE_GROUPS = 4;
        public const int BLOCK_SIZES = 13;
        public const int BLOCK_INVALID = 14;
        public const int PARTITION_CONTEXTS = 16;
        public const int MI_SIZE = 8;
        public const int MIN_TILE_WIDTH_B64 = 4;
        public const int MAX_TILE_WIDTH_B64 = 64;
        public const int MAX_MV_REF_CANDIDATES = 2;
        public const int NUM_REF_FRAMES = 8;
        public const int MAX_REF_FRAMES = 4;
        public const int IS_INTER_CONTEXTS = 4;
        public const int COMP_MODE_CONTEXTS = 5;
        public const int REF_CONTEXTS = 5;
        public const int MAX_SEGMENTS = 8;
        public const int SEG_LVL_ALT_Q = 0;
        public const int SEG_LVL_ALT_L = 1;
        public const int SEG_LVL_REF_FRAME = 2;
        public const int SEG_LVL_SKIP = 3;
        public const int SEG_LVL_MAX = 4;
        public const int BLOCK_TYPES = 2;
        public const int REF_TYPES = 2;
        public const int COEF_BANDS = 6;
        public const int PREV_COEF_CONTEXTS = 6;
        public const int UNCONSTRAINED_NODES = 3;
        public const int TX_SIZE_CONTEXTS = 2;
        public const int SWITCHABLE_FILTERS = 3;
        public const int INTERP_FILTER_CONTEXTS = 4;
        public const int SKIP_CONTEXTS = 3;
        public const int PARTITION_TYPES = 4;
        public const int TX_SIZES = 4;
        public const int TX_MODES = 5;
        public const int DCT_DCT = 0;
        public const int ADST_DCT = 1;
        public const int DCT_ADST = 2;
        public const int ADST_ADST = 3;
        public const int MB_MODE_COUNT = 14;
        public const int INTRA_MODES = 10;
        public const int INTER_MODES = 4;
        public const int INTER_MODE_CONTEXTS = 7;
        public const int MV_JOINTS = 4;
        public const int MV_CLASSES = 11;
        public const int CLASS0_SIZE = 2;
        public const int MV_OFFSET_BITS = 10;
        public const int MAX_PROB = 255;
        public const int MAX_MODE_LF_DELTAS = 2;
        public const int COMPANDED_MVREF_THRESH = 8;
        public const int MAX_LOOP_FILTER = 63;
        public const int REF_SCALE_SHIFT = 14;
        public const int SUBPEL_BITS = 4;
        public const int SUBPEL_SHIFTS = 16;
        public const int SUBPEL_MASK = 15;
        public const int MV_BORDER = 128;
        public const int INTERP_EXTEND = 4;
        public const int BORDERINPIXELS = 160;
        public const int MAX_UPDATE_FACTOR = 128;
        public const int COUNT_SAT = 20;
        public const int BOTH_ZERO = 0;
        public const int BOTH_PREDICTED = 2;
        public const int NEW_PLUS_NON_INTRA = 3;
        public const int BOTH_NEW = 4;
        public const int INTRA_PLUS_NON_INTRA = 5;
        public const int BOTH_INTRA = 6;
        public const int INVALID_CASE = 9;

        // frame_type (7.2)
        public const int KEY_FRAME = 0;
        public const int NON_KEY_FRAME = 1;

        // color_space (7.2.2)
        public const int CS_UNKNOWN = 0;
        public const int CS_BT_601 = 1;
        public const int CS_BT_709 = 2;
        public const int CS_SMPTE_170 = 3;
        public const int CS_SMPTE_240 = 4;
        public const int CS_BT_2020 = 5;
        public const int CS_RESERVED = 6;
        public const int CS_RGB = 7;

        // interpolation_filter (7.2.7)
        public const int EIGHTTAP = 0;
        public const int EIGHTTAP_SMOOTH = 1;
        public const int EIGHTTAP_SHARP = 2;
        public const int BILINEAR = 3;
        public const int SWITCHABLE = 4;

        // tx_mode (7.3.1)
        public const int ONLY_4X4 = 0;
        public const int ALLOW_8X8 = 1;
        public const int ALLOW_16X16 = 2;
        public const int ALLOW_32X32 = 3;
        public const int TX_MODE_SELECT = 4;

        // reference_mode (7.3.6)
        public const int SINGLE_REFERENCE = 0;
        public const int COMPOUND_REFERENCE = 1;
        public const int REFERENCE_MODE_SELECT = 2;

        // tx_size (7.4.8)
        public const int TX_4X4 = 0;
        public const int TX_8X8 = 1;
        public const int TX_16X16 = 2;
        public const int TX_32X32 = 3;

        // Reference frames (7.4.12)
        public const int INTRA_FRAME = 0;
        public const int LAST_FRAME = 1;
        public const int GOLDEN_FRAME = 2;
        public const int ALTREF_FRAME = 3;
    }
}
