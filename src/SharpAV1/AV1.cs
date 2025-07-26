using System;
using System.Collections.Generic;
using System.IO;

namespace SharpAV1
{
    public interface IAomContext : IAomSerializable
    {
        int ObuSizeLen { get; }
        byte[] LastObuFrameHeader { get; set; }
    }

    public interface IAomSerializable
    {
        void Read(AomStream stream, int size);
        void Write(AomStream stream, int size);
    }

    public partial class AV1Context
    {
        private AomStream stream;

        private int obu_padding_length = 0;
        private int obu_size_len = 0;
        private int prevFrame;
        private Dictionary<string, Queue<int>> _cachedIncrementValues = new Dictionary<string, Queue<int>>();


        public byte[] LastObuFrameHeader { get; set; }
        public int ObuSizeLen { get { return obu_size_len; } }

        public int[][] PrevGmParams { get; set; } = new int[8][] { new int[6], new int[6], new int[6], new int[6], new int[6], new int[6], new int[6], new int[6] };
        public int[][][] SavedGmParams { get; set; } = null;
        public int[] RefFrameHeight { get; set; } = new int[AV1Constants.NUM_REF_FRAMES];
        public int[] RefFrameType { get; set; } = new int[AV1Constants.NUM_REF_FRAMES];
        public int[] RefRenderWidth { get; set; } = new int[AV1Constants.NUM_REF_FRAMES];
        public int[] RefRenderHeight { get; set; } = new int[AV1Constants.NUM_REF_FRAMES];
        public int[] RefUpscaledWidth { get; set; } = new int[AV1Constants.NUM_REF_FRAMES];
        public int[] Remap_Lr_Type { get; set; } = new int[] { AV1FrameRestorationType.RESTORE_NONE, AV1FrameRestorationType.RESTORE_SWITCHABLE, AV1FrameRestorationType.RESTORE_WIENER, AV1FrameRestorationType.RESTORE_SGRPROJ };
        public int[] Segmentation_Feature_Bits { get; set; } = new int[AV1Constants.SEG_LVL_MAX];
        public int[] Segmentation_Feature_Max { get; set; } = new int[AV1Constants.SEG_LVL_MAX];
        public int[] Segmentation_Feature_Signed { get; set; } = new int[AV1Constants.SEG_LVL_MAX];

        public void Read(AomStream stream, int size)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
            ReadOpenBitstreamUnit(size);
        }

        public void Write(AomStream stream, int size)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
            WriteOpenBitstreamUnit(size);
        }

        private int GetQIndex(int ignoreDeltaQ, int segmentId)
        {
            // This is a stub for the quantizer index retrieval logic.
            // In a complete implementation, this would retrieve the quantizer index based on the segment ID.
            return ignoreDeltaQ;
        }

        private void ReadDropObu() { /* nothing */ }
        private void WriteDropObu() { /* nothing */ }
        private void ReadSetFrameRefs() { /* nothing */ }
        private void WriteSetFrameRefs() { /* nothing */ }
        private void ReadResetGrainParams() { /* nothing */ }
        private void WriteResetGrainParams() { /* nothing */ }
        private void ReadLoadGrainParams(int p) { /* nothing */ }
        private void WriteLoadGrainParams(int p) { /* nothing */ }
        private void WriteInitNonCoeffCdfs() { /* nothing */ }
        private void WriteSetupPastIndependence() { /* nothing */ }
        private void WriteLoadCdfs(int value) { /* nothing */ }
        private void WriteLoadPrevious() { /* nothing */ }
        private void WriteMotionFieldEstimation() { /* nothing */ }
        private void WriteInitCoeffCdfs() { /* nothing */ }
        private void WriteLoadPreviousSegmentIds() { /* nothing */ }
        private void ReadInitNonCoeffCdfs() { /* nothing */ }
        private void ReadSetupPastIndependence() 
        {
            for (int r = AV1RefFrames.LAST_FRAME; r <= AV1RefFrames.ALTREF_FRAME; r++)
            {
                for (int ri = 0; ri <= 5; ri++)
                {
                    PrevGmParams[r][ri] = ((ri % 3 == 2) ? (1 << AV1Constants.WARPEDMODEL_PREC_BITS) : 0);
                }
            }
        }
        private void ReadLoadCdfs(int value) { /* nothing */ }
        private void ReadLoadPrevious() 
        {
            prevFrame = ref_frame_idx[primary_ref_frame];
            PrevGmParams = SavedGmParams[prevFrame];
            // TODO
        }
        private void ReadMotionFieldEstimation() { /* nothing */ }
        private void ReadInitCoeffCdfs() { /* nothing */ }
        private void ReadLoadPreviousSegmentIds() { /* nothing */ }
        private void ReadMarkRefFrames(int idLen) { /* nothing */ }
        private void WriteMarkRefFrames(int idLen) { /* nothing */ }
        private void ReadItutT35PayloadBytes() { /* nothing */ }
        private void WriteItutT35PayloadBytes() { /* nothing */ }
        private void ReadSkipObu() 
        {
            long totalObuSizeBits = obu_size << 3;
            int currentBits = stream.GetPosition() - startPosition;
            stream.Skip(totalObuSizeBits - currentBits);
        }
        private void WriteSkipObu() { /* nothing */ }
        private void ReadFrameHeaderCopy() 
        {
            using (var aomStream = new AomStream(new MemoryStream(LastObuFrameHeader)))
            {
                var oldStream = this.stream;
                var oldSeenFrameHeader = SeenFrameHeader;
                var oldObuType = _ObuType;
                SeenFrameHeader = 0;
                this.stream = aomStream;
                ReadFrameHeaderObu();
                SeenFrameHeader = oldSeenFrameHeader;
                _ObuType = oldObuType;
                this.stream = oldStream;
            }
        }
        private void WriteFrameHeaderCopy() { /* nothing */ }
        private void ReadDecodeFrameWrapup() 
        {
            for (int i = 0; i < AV1Constants.NUM_REF_FRAMES; i++)
            {
                if (((refresh_frame_flags >> i) & 1) == 1)
                {
                    RefValid[i] = 1;
                    RefUpscaledWidth[i] = UpscaledWidth;
                    RefFrameHeight[i] = FrameHeight;
                    RefRenderWidth[i] = RenderWidth;
                    RefRenderHeight[i] = RenderHeight;
                    RefFrameType[i] = frame_type;

                    if(SavedGmParams == null)
                    {
                        SavedGmParams = new int[AV1Constants.NUM_REF_FRAMES][][];
                        for (int j = 0; j < SavedGmParams.Length; j++)
                        {
                            SavedGmParams[j] = new int[AV1Constants.TOTAL_REFS_PER_FRAME][];
                            for (int k = 0; k < SavedGmParams[j].Length; k++)
                            {
                                SavedGmParams[j][k] = new int[6];
                            }
                        }
                    }

                    for (int ri = AV1RefFrames.LAST_FRAME; ri < AV1RefFrames.ALTREF_FRAME; ri++)
                    {
                        for (int j = 0; j <= 5; j++)
                        {
                            SavedGmParams[i][ri][j] = gm_params[ri][j];
                        }
                    }

                    // TODO
                }
            }            
        }

        private void WriteDecodeFrameWrapup() { /* nothing */ }
        private int ChooseOperatingPoint()
        {
            return 0;
        }
    }

    public static class AV1RefFrames
    {
        public const int NONE = -1;
        public const int INTRA_FRAME = 0;
        public const int LAST_FRAME = 1;
        public const int LAST2_FRAME = 2;
        public const int LAST3_FRAME = 3;
        public const int GOLDEN_FRAME = 4;
        public const int BWDREF_FRAME = 5;
        public const int ALTREF2_FRAME = 6;
        public const int ALTREF_FRAME = 7;
    }

    public static class AV1ObuTypes
    {
        public const int OBU_RESERVED_0 = 0;
        public const int OBU_SEQUENCE_HEADER = 1;
        public const int OBU_TEMPORAL_DELIMITER = 2;
        public const int OBU_FRAME_HEADER = 3;
        public const int OBU_TILE_GROUP = 4;
        public const int OBU_METADATA = 5;
        public const int OBU_FRAME = 6;
        public const int OBU_REDUNDANT_FRAME_HEADER = 7;
        public const int OBU_TILE_LIST = 8;
        public const int OBU_RESERVED_9 = 9;
        public const int OBU_RESERVED_10 = 10;
        public const int OBU_RESERVED_11 = 11;
        public const int OBU_RESERVED_12 = 12;
        public const int OBU_RESERVED_13 = 13;
        public const int OBU_RESERVED_14 = 14;
        public const int OBU_PADDING = 15;
    }

    public static class AV1Constants
    {
        public const int REFS_PER_FRAME = 7;                // Number of reference frames that can be used for inter prediction
        public const int TOTAL_REFS_PER_FRAME = 8;          // Number of reference frame types (including intra type)
        public const int BLOCK_SIZE_GROUPS = 4;             // Number of contexts when decoding y_mode
        public const int BLOCK_SIZES = 22;                  // Number of different block sizes used
        public const int BLOCK_INVALID = 22;                // Sentinel value to mark partition choices that are not allowed
        public const int MAX_SB_SIZE = 128;                 // Maximum size of a superblock in luma samples
        public const int MI_SIZE = 4;                       // Smallest size of a mode info block in luma samples
        public const int MI_SIZE_LOG2 = 2;                  // Base 2 logarithm of smallest size of a mode info block
        public const int MAX_TILE_WIDTH = 4096;             // Maximum width of a tile in units of luma samples
        public const int MAX_TILE_AREA = 4096 * 2304;       // Maximum area of a tile in units of luma samples
        public const int MAX_TILE_ROWS = 64;                // Maximum number of tile rows
        public const int MAX_TILE_COLS = 64;                // Maximum number of tile columns
        public const int INTRABC_DELAY_PIXELS = 256;        // Number of horizontal luma samples before intra block copy can be used
        public const int INTRABC_DELAY_SB64 = 4;            // Number of 64 by 64 blocks before intra block copy can be used
        public const int NUM_REF_FRAMES = 8;                // Number of frames that can be stored for future reference
        public const int IS_INTER_CONTEXTS = 4;             // Number of contexts for is_inter
        public const int REF_CONTEXTS = 3;                  // Number of contexts for single_ref , comp_ref , comp_bwdref , uni_comp_ref , uni_comp_ref_p1 and uni_comp_ref_p2
        public const int MAX_SEGMENTS = 8;                  // Number of segments allowed in segmentation map
        public const int SEGMENT_ID_CONTEXTS = 3;           // Number of contexts for segment_id
        public const int SEG_LVL_ALT_Q = 0;                 // Index for quantizer segment feature
        public const int SEG_LVL_ALT_LF_Y_V = 1;            // Index for vertical luma loop filter segment feature
        public const int SEG_LVL_REF_FRAME = 5;             // Index for reference frame segment feature
        public const int SEG_LVL_SKIP = 6;                  // Index for skip segment feature
        public const int SEG_LVL_GLOBALMV = 7;              // Index for global mv feature
        public const int SEG_LVL_MAX = 8;                   // Number of segment features
        public const int PLANE_TYPES = 2;                   // Number of different plane types(luma or chroma)
        public const int TX_SIZE_CONTEXTS = 3;              // Number of contexts for transform size
        public const int INTERP_FILTERS = 3;                // Number of values for interp_filter
        public const int INTERP_FILTER_CONTEXTS = 16;       // Number of contexts for interp_filter
        public const int SKIP_MODE_CONTEXTS = 3;            // Number of contexts for decoding skip_mode
        public const int SKIP_CONTEXTS = 3;                 // Number of contexts for decoding skip
        public const int PARTITION_CONTEXTS = 4;            // Number of contexts when decoding partition
        public const int TX_SIZES = 5;                      // Number of square transform sizes
        public const int TX_SIZES_ALL = 19;                 // Number of transform sizes(including non-square sizes)
        public const int TX_MODES = 3;                      // Number of values for tx_mode
        public const int DCT_DCT = 0;                       // Inverse transform rows with DCT and columns with DCT
        public const int ADST_DCT = 1;                      // Inverse transform rows with DCT and columns with ADST
        public const int DCT_ADST = 2;                      // Inverse transform rows with ADST and columns with DCT
        public const int ADST_ADST = 3;                     // Inverse transform rows with ADST and columns with ADST
        public const int FLIPADST_DCT = 4;                  // Inverse transform rows with DCT and columns with FLIPADST
        public const int DCT_FLIPADST = 5;                  // Inverse transform rows with FLIPADST and columns with DCT
        public const int FLIPADST_FLIPADST = 6;             // Inverse transform rows with FLIPADST and columns with FLIPADST
        public const int ADST_FLIPADST = 7;                 // Inverse transform rows with FLIPADST and columns with ADST
        public const int FLIPADST_ADST = 8;                 // Inverse transform rows with ADST and columns with FLIPADST
        public const int IDTX = 9;                          // Inverse transform rows with identity and columns with identity
        public const int V_DCT = 10;                        // Inverse transform rows with identity and columns with DCT
        public const int H_DCT = 11;                        // Inverse transform rows with DCT and columns with identity
        public const int V_ADST = 12;                       // Inverse transform rows with identity and columns with ADST
        public const int H_ADST = 13;                       // Inverse transform rows with ADST and columns with identity
        public const int V_FLIPADST = 14;                   // Inverse transform rows with identity and columns with FLIPADST
        public const int H_FLIPADST = 15;                   // Inverse transform rows with FLIPADST and columns with identity
        public const int TX_TYPES = 16;                     // Number of inverse transform types
        public const int MB_MODE_COUNT = 17;                // Number of values for YMode
        public const int INTRA_MODES = 13;                  // Number of values for y_mode
        public const int UV_INTRA_MODES_CFL_NOT_ALLOWED = 13; // Number of values for uv_mode when chroma from luma is not allowed
        public const int UV_INTRA_MODES_CFL_ALLOWED = 14;   // Number of values for uv_mode when chroma from luma is allowed
        public const int COMPOUND_MODES = 8;                // Number of values for compound_mode
        public const int COMPOUND_MODE_CONTEXTS = 8;        // Number of contexts for compound_mode
        public const int COMP_NEWMV_CTXS = 5;               // Number of new mv values used when constructing context for compound_mode
        public const int NEW_MV_CONTEXTS = 6;               // Number of contexts for new_mv
        public const int ZERO_MV_CONTEXTS = 2;              // Number of contexts for zero_mv
        public const int REF_MV_CONTEXTS = 6;               // Number of contexts for ref_mv
        public const int DRL_MODE_CONTEXTS = 3;             // Number of contexts for drl_mode
        public const int MV_CONTEXTS = 2;                   // Number of contexts for decoding motion vectors including one for intra block copy
        public const int MV_INTRABC_CONTEXT = 1;            // Motion vector context used for intra block copy
        public const int MV_JOINTS = 4;                     // Number of values for mv_joint
        public const int MV_CLASSES = 11;                   // Number of values for mv_class
        public const int CLASS0_SIZE = 2;                   // Number of values for mv_class0_bit
        public const int MV_OFFSET_BITS = 10;               // Maximum number of bits for decoding motion vectors
        public const int MAX_LOOP_FILTER = 63;              // Maximum value used for loop filtering
        public const int REF_SCALE_SHIFT = 14;              // Number of bits of precision when scaling reference frames
        public const int SUBPEL_BITS = 4;                   // Number of bits of precision when choosing an inter prediction filter kernel
        public const int SUBPEL_MASK = 15;                  // ( 1 << SUBPEL_BITS ) - 1
        public const int SCALE_SUBPEL_BITS = 10;            // Number of bits of precision when computing inter prediction locations
        public const int MV_BORDER = 128;                   // Value used when clipping motion vectors
        public const int PALETTE_COLOR_CONTEXTS = 5;        // Number of values for color contexts
        public const int PALETTE_MAX_COLOR_CONTEXT_HASH = 8; // Number of mappings between color context hash and color context
        public const int PALETTE_BLOCK_SIZE_CONTEXTS = 7; // Number of values for palette block size
        public const int PALETTE_Y_MODE_CONTEXTS = 3; // Number of values for palette Y plane mode contexts
        public const int PALETTE_UV_MODE_CONTEXTS = 2; // Number of values for palette U and V plane mode contexts
        public const int PALETTE_SIZES = 7; // Number of values for palette_size
        public const int PALETTE_COLORS = 8; // Number of values for palette_color
        public const int PALETTE_NUM_NEIGHBORS = 3; // Number of neighbors considered within palette computation
        public const int DELTA_Q_SMALL = 3; // Value indicating alternative encoding of quantizer index delta values
        public const int DELTA_LF_SMALL = 3; // Value indicating alternative encoding of loop filter delta values
        public const int QM_TOTAL_SIZE = 3344; // Number of values in the quantizer matrix
        public const int MAX_ANGLE_DELTA = 3; // Maximum magnitude of AngleDeltaY and AngleDeltaUV
        public const int DIRECTIONAL_MODES = 8; // Number of directional intra modes
        public const int ANGLE_STEP = 3; // Number of degrees of step per unit increase in AngleDeltaY or AngleDeltaUV.
        public const int TX_SET_TYPES_INTRA = 3; // Number of intra transform set types
        public const int TX_SET_TYPES_INTER = 4; // Number of inter transform set types
        public const int WARPEDMODEL_PREC_BITS = 16; // Internal precision of warped motion models
        public const int IDENTITY = 0; // Warp model is just an identity transform
        public const int TRANSLATION = 1; // Warp model is a pure translation
        public const int ROTZOOM = 2; // Warp model is a rotation + symmetric zoom + translation
        public const int AFFINE = 3; // Warp model is a general affine transform
        public const int GM_ABS_TRANS_BITS = 12; // Number of bits encoded for translational components of global motion models, if part of a ROTZOOM or AFFINE model
        public const int GM_ABS_TRANS_ONLY_BITS = 9; // Number of bits encoded for translational components of global motion models, if part of a TRANSLATION model
        public const int GM_ABS_ALPHA_BITS = 12; // Number of bits encoded for non-translational components of global motion models
        public const int DIV_LUT_PREC_BITS = 14; // Number of fractional bits of entries in divisor lookup table
        public const int DIV_LUT_BITS = 8; // Number of fractional bits for lookup in divisor lookup table
        public const int DIV_LUT_NUM = 257; // Number of entries in divisor lookup table
        public const int MOTION_MODES = 3; // Number of values for motion modes
        public const int SIMPLE = 0; // Use translation or global motion compensation
        public const int OBMC = 1; // Use overlapped block motion compensation
        public const int LOCALWARP = 2; // Use local warp motion compensation
        public const int LEAST_SQUARES_SAMPLES_MAX = 8; // Largest number of samples used when computing a local warp
        public const int LS_MV_MAX = 256; // Largest motion vector difference to include in local warp computation
        public const int WARPEDMODEL_TRANS_CLAMP = 1 << 23; // Clamping value used for translation components of warp
        public const int WARPEDMODEL_NONDIAGAFFINE_CLAMP = 1 << 13; // Clamping value used for matrix components of warp
        public const int WARPEDPIXEL_PREC_SHIFTS = 1 << 6; // Number of phases used in warped filtering
        public const int WARPEDDIFF_PREC_BITS = 10; // Number of extra bits of precision in warped filtering
        public const int GM_ALPHA_PREC_BITS = 15; // Number of fractional bits for sending non translational warp model coefficients
        public const int GM_TRANS_PREC_BITS = 6; // Number of fractional bits for sending translational warp model coefficients
        public const int GM_TRANS_ONLY_PREC_BITS = 3; // Number of fractional bits used for pure translational warps
        public const int INTERINTRA_MODES = 4; // Number of inter intra modes
        public const int MASK_MASTER_SIZE = 64; // Size of MasterMask array
        public const int SEGMENT_ID_PREDICTED_CONTEXTS = 3; // Number of contexts for segment_id_predicted
        //public const int IS_INTER_CONTEXTS = 4; // Number of contexts for is_inter
        //public const int SKIP_CONTEXTS = 3; // Number of contexts for skip
        public const int FWD_REFS = 4; // Number of syntax elements for forward reference frames
        public const int BWD_REFS = 3; // Number of syntax elements for backward reference frames
        public const int SINGLE_REFS = 7; // Number of syntax elements for single reference frames
        public const int UNIDIR_COMP_REFS = 4; // Number of syntax elements for unidirectional compound reference frames
        public const int COMPOUND_TYPES = 2; // Number of values for compound_type
        public const int CFL_JOINT_SIGNS = 8; // Number of values for cfl_alpha_signs
        public const int CFL_ALPHABET_SIZE = 16; // Number of values for cfl_alpha_u and cfl_alpha_v
        public const int COMP_INTER_CONTEXTS = 5; // Number of contexts for comp_mode
        public const int COMP_REF_TYPE_CONTEXTS = 5; // Number of contexts for comp_ref_type
        public const int CFL_ALPHA_CONTEXTS = 6; // Number of contexts for cfl_alpha_u and cfl_alpha_v
        public const int INTRA_MODE_CONTEXTS = 5; // Number of each of left and above contexts for intra_frame_y_mode
        public const int COMP_GROUP_IDX_CONTEXTS = 6; // Number of contexts for comp_group_idx
        public const int COMPOUND_IDX_CONTEXTS = 6; // Number of contexts for compound_idx
        public const int INTRA_EDGE_KERNELS = 3; // Number of filter kernels for the intra edge filter
        public const int INTRA_EDGE_TAPS = 5; // Number of kernel taps for the intra edge filter
        public const int FRAME_LF_COUNT = 4; // Number of loop filter strength values
        public const int MAX_VARTX_DEPTH = 2; // Maximum depth for variable transform trees
        public const int TXFM_PARTITION_CONTEXTS = 21; // Number of contexts for txfm_split
        public const int REF_CAT_LEVEL = 640; // Bonus weight for close motion vectors
        public const int MAX_REF_MV_STACK_SIZE = 8; // Maximum number of motion vectors in the stack
        public const int MFMV_STACK_SIZE = 3; // Stack size for motion field motion vectors
        public const int MAX_TX_DEPTH = 2; // Maximum times the transform can be split
        public const int WEDGE_TYPES = 16; // Number of directions for the wedge mask process
        public const int FILTER_BITS = 7; // Number of bits used in Wiener filter coefficients
        public const int WIENER_COEFFS = 3; // Number of Wiener filter coefficients to read
        public const int SGRPROJ_PARAMS_BITS = 4; // Number of bits needed to specify self guided filter set
        public const int SGRPROJ_PRJ_SUBEXP_K = 4; // Controls how self guided deltas are read
        public const int SGRPROJ_PRJ_BITS = 7; // Precision bits during self guided restoration
        public const int SGRPROJ_RST_BITS = 4; // Restoration precision bits generated higher than source before projection
        public const int SGRPROJ_MTABLE_BITS = 20; // Precision of mtable division table
        public const int SGRPROJ_RECIP_BITS = 12; // Precision of division by n table
        public const int SGRPROJ_SGR_BITS = 8; // Internal precision bits for core selfguided_restoration
        public const int EC_PROB_SHIFT = 6; // Number of bits to reduce CDF precision during arithmetic coding
        public const int EC_MIN_PROB = 4; // Minimum probability assigned to each symbol during arithmetic coding
        public const int SELECT_SCREEN_CONTENT_TOOLS = 2; // Value that indicates the allow_screen_content_tools syntax element is coded
        public const int SELECT_INTEGER_MV = 2; // Value that indicates the force_integer_mv syntax element is coded
        public const int RESTORATION_TILESIZE_MAX = 256; // Maximum size of a loop restoration tile
        public const int MAX_FRAME_DISTANCE = 31; // Maximum distance when computing weighted prediction
        public const int MAX_OFFSET_WIDTH = 8; // Maximum horizontal offset of a projected motion vector
        public const int MAX_OFFSET_HEIGHT = 0; // Maximum vertical offset of a projected motion vector
        public const int WARP_PARAM_REDUCE_BITS = 6; // Rounding bitwidth for the parameters to the shear process
        public const int NUM_BASE_LEVELS = 2; // Number of quantizer base levels
        public const int COEFF_BASE_RANGE = 12; // The quantizer range above NUM_BASE_LEVELS above which the Exp-Golomb coding process is activated
        public const int BR_CDF_SIZE = 4; // Number of values for coeff_br
        public const int SIG_COEF_CONTEXTS_EOB = 4; // Number of contexts for coeff_base_eob
        public const int SIG_COEF_CONTEXTS_2D = 26; // Context offset for coeff_base for horizontal-only or vertical-only transforms.
        public const int SIG_COEF_CONTEXTS = 42; // Number of contexts for coeff_base
        public const int SIG_REF_DIFF_OFFSET_NUM = 5; // Maximum number of context samples to be used in determining the context index for coeff_base and coeff_base_eob.
        public const int SUPERRES_NUM = 8; // Numerator for upscaling ratio
        public const int SUPERRES_DENOM_MIN = 9; // Smallest denominator for upscaling ratio
        public const int SUPERRES_DENOM_BITS = 3; // Number of bits sent to specify denominator of upscaling ratio
        public const int SUPERRES_FILTER_BITS = 6; // Number of bits of fractional precision for upscaling filter selection 
        public const int SUPERRES_FILTER_SHIFTS = 1 << SUPERRES_FILTER_BITS; // Number of phases of upscaling filters
        public const int SUPERRES_FILTER_TAPS = 8; // Number of taps of upscaling filters
        public const int SUPERRES_FILTER_OFFSET = 3; // Sample offset for upscaling filters
        public const int SUPERRES_SCALE_BITS = 14; // Number of fractional bits for computing position in upscaling
        public const int SUPERRES_SCALE_MASK = (1 << 14) - 1; // Mask for computing position in upscaling
        public const int SUPERRES_EXTRA_BITS = 8; // Difference in precision between SUPERRES_SCALE_BITS and SUPERRES_FILTER_BITS
        public const int TXB_SKIP_CONTEXTS = 13; // Number of contexts for all_zero
        public const int EOB_COEF_CONTEXTS = 9; // Number of contexts for eob_extra
        public const int DC_SIGN_CONTEXTS = 3; // Number of contexts for dc_sign
        public const int LEVEL_CONTEXTS = 21; // Number of contexts for coeff_br
        public const int TX_CLASS_2D = 0; // Transform class for transform types performing non-identity transforms in both directions
        public const int TX_CLASS_HORIZ = 1; // Transform class for transforms performing only a horizontal non-identity transform
        public const int TX_CLASS_VERT = 2; // Transform class for transforms performing only a vertical non-identity transform
        public const int REFMVS_LIMIT = (1 << 12) - 1; // Largest reference MV component that can be saved
        public const int INTRA_FILTER_SCALE_BITS = 4; // Scaling shift for intra filtering process
        public const int INTRA_FILTER_MODES = 5; // Number of types of intra filtering
        public const int COEFF_CDF_Q_CTXS = 4; // Number of selectable context types for the coeff() syntax structure
        public const int PRIMARY_REF_NONE = 7; // Value of primary_ref_frame indicating that there is no primary reference frame
        public const int BUFFER_POOL_MAX_SIZE = 10; // Number of frames in buffer pool
    }

    public static class AV1ColorPrimaries
    {
        public const int CP_BT_709 = 1;
        public const int CP_UNSPECIFIED = 2;
        public const int CP_BT_470_M = 4;
        public const int CP_BT_470_B_G = 5;
        public const int CP_BT_601 = 6;
        public const int CP_SMPTE_240 = 7;
        public const int CP_GENERIC_FILM = 8;
        public const int CP_BT_2020 = 9;
        public const int CP_XYZ = 10;
        public const int CP_SMPTE_431 = 11;
        public const int CP_SMPTE_432 = 12;
        public const int CP_EBU_3213 = 22;
    }

    public static class AV1TransferCharacteristics
    {
        public const int TC_RESERVED_0 = 0;
        public const int TC_BT_709 = 1;
        public const int TC_UNSPECIFIED = 2;
        public const int TC_RESERVED_3 = 3;
        public const int TC_BT_470_M = 4;
        public const int TC_BT_470_B_G = 5;
        public const int TC_BT_601 = 6;
        public const int TC_SMPTE_240 = 7;
        public const int TC_LINEAR = 8;
        public const int TC_LOG_100 = 9;
        public const int TC_LOG_100_SQRT10 = 10;
        public const int TC_IEC_61966 = 11;
        public const int TC_BT_1361 = 12;
        public const int TC_SRGB = 13;
        public const int TC_BT_2020_10_BIT = 14;
        public const int TC_BT_2020_12_BIT = 15;
        public const int TC_SMPTE_2084 = 16;
        public const int TC_SMPTE_428 = 17;
        public const int TC_HLG = 18;
    }

    public static class AV1MatrixCoefficients
    {
        public const int MC_IDENTITY = 0;
        public const int MC_BT_709 = 1;
        public const int MC_UNSPECIFIED = 2;
        public const int MC_RESERVED_3 = 3;
        public const int MC_FCC = 4;
        public const int MC_BT_470_B_G = 5;
        public const int MC_BT_601 = 6;
        public const int MC_SMPTE_240 = 7;
        public const int MC_SMPTE_YCGCO = 8;
        public const int MC_BT_2020_NCL = 9;
        public const int MC_BT_2020_CL = 10;
        public const int MC_SMPTE_2085 = 11;
        public const int MC_CHROMAT_NCL = 12;
        public const int MC_CHROMAT_CL = 13;
        public const int MC_ICTCP = 14;
    }

    public static class AV1ChromaSamplePosition
    {
        public const int CSP_UNKNOWN = 0;
        public const int CSP_VERTICAL = 1;
        public const int CSP_COLOCATED = 2;
        public const int CSP_RESERVED = 3;
    }

    public static class AV1FrameTypes
    {
        public const int KEY_FRAME = 0;
        public const int INTER_FRAME = 1;
        public const int INTRA_ONLY_FRAME = 2;
        public const int SWITCH_FRAME = 3;
    }

    public static class AV1MetadataType
    {
        public const int METADATA_TYPE_HDR_CLL = 1;
        public const int METADATA_TYPE_HDR_MDCV = 2;
        public const int METADATA_TYPE_SCALABILITY = 3;
        public const int METADATA_TYPE_ITUT_T35 = 4;
        public const int METADATA_TYPE_TIMECODE = 5;
    }

    public static class AV1FrameRestorationType
    {
        public const int RESTORE_NONE = 0;
        public const int RESTORE_SWITCHABLE = 3;
        public const int RESTORE_WIENER = 1;
        public const int RESTORE_SGRPROJ = 2;
    }

    public static class AV1ScalabilityModeIdc
    {
        public const int SCALABILITY_L1T2 = 0;
        public const int SCALABILITY_L1T3 = 1;
        public const int SCALABILITY_L2T1 = 2;
        public const int SCALABILITY_L2T2 = 3;
        public const int SCALABILITY_L2T3 = 4;
        public const int SCALABILITY_S2T1 = 5;
        public const int SCALABILITY_S2T2 = 6;
        public const int SCALABILITY_S2T3 = 7;
        public const int SCALABILITY_L2T1h = 8;
        public const int SCALABILITY_L2T2h = 9;
        public const int SCALABILITY_L2T3h = 10;
        public const int SCALABILITY_S2T1h = 11;
        public const int SCALABILITY_S2T2h = 12;
        public const int SCALABILITY_S2T3h = 13;
        public const int SCALABILITY_SS = 14;
        public const int SCALABILITY_L3T1 = 15;
        public const int SCALABILITY_L3T2 = 16;
        public const int SCALABILITY_L3T3 = 17;
        public const int SCALABILITY_S3T1 = 18;
        public const int SCALABILITY_S3T2 = 19;
        public const int SCALABILITY_S3T3 = 20;
        public const int SCALABILITY_L3T2_KEY = 21;
        public const int SCALABILITY_L3T3_KEY = 22;
        public const int SCALABILITY_L4T5_KEY = 23;
        public const int SCALABILITY_L4T7_KEY = 24;
        public const int SCALABILITY_L3T2_KEY_SHIFT = 25;
        public const int SCALABILITY_L3T3_KEY_SHIFT = 26;
        public const int SCALABILITY_L4T5_KEY_SHIFT = 27;
        public const int SCALABILITY_L4T7_KEY_SHIFT = 28;
    }

    public static class AV1TxModes
    {
        public const int ONLY_4X4 = 0;
        public const int TX_MODE_LARGEST = 1;
        public const int TX_MODE_SELECT = 2;
    }

    public static class AV1InterpolationFilter
    {
        public const int EIGHTTAP = 0;
        public const int EIGHTTAP_SMOOTH = 1;
        public const int EIGHTTAP_SHARP = 2;
        public const int BILINEAR = 3;
        public const int SWITCHABLE = 4;
    }
}
