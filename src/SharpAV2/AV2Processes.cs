using SharpAVX;
using System;
using System.Collections.Generic;

namespace SharpAV2
{
    /// <summary>
    /// The functions and processes of the AV2 specification v1.0.0 that it gives in words rather than in
    /// code: the mathematical functions (4.7), the saving and loading of state between frames (6.x, 7.6,
    /// 7.23), and the decoding processes the syntax invokes. SharpAV2 reads the headers, not the tile
    /// data: the processes that act only on samples, motion vectors or CDFs do nothing here, and each says so.
    /// </summary>
    public partial class AV2Context
    {
        #region Mathematical functions (4.7)

        private static int Abs(int x) => x >= 0 ? x : -x;

        private static int Min(int x, int y) => x <= y ? x : y;

        private static int Max(int x, int y) => x >= y ? x : y;

        public static int Clip3(int x, int y, int z) => z < x ? x : z > y ? y : z;

        private int Clip1(int x) => Clip3(0, (1 << BitDepth) - 1, x);

        private int Round2Signed(int x, int n) => x >= 0 ? Round2(x, n) : -Round2(-x, n);

        #endregion

        #region Tables given in words

        private static int[] _blockWidth;
        private static int[] _blockHeight;

        /// <summary>Block_Width[ x ] is 4 * Num_4x4_Blocks_Wide[ x ] (the semantics of subSize).</summary>
        private static int[] Block_Width => _blockWidth ??= Array.ConvertAll(Num_4x4_Blocks_Wide, n => 4 * n);

        /// <summary>Block_Height[ x ] is 4 * Num_4x4_Blocks_High[ x ].</summary>
        private static int[] Block_Height => _blockHeight ??= Array.ConvertAll(Num_4x4_Blocks_High, n => 4 * n);

        /// <summary>Dist_Score_Lookup, 7.7.</summary>
        private static readonly int[] Dist_Score_Lookup = { 0, 64, 96, 112, 120, 124, 126 };

        /// <summary>Div_Lut, 7.13.3.22.</summary>
        private static readonly int[] Div_Lut =
        {
            512, 508, 504, 500, 496, 493, 489, 485, 482, 478, 475, 471, 468, 465, 462,
            458, 455, 452, 449, 446, 443, 440, 437, 434, 431, 428, 426, 423, 420, 417,
            415, 412, 410, 407, 405, 402, 400, 397, 395, 392, 390, 388, 386, 383, 381,
            379, 377, 374, 372, 370, 368, 366, 364, 362, 360, 358, 356, 354, 352, 350,
            349, 347, 345, 343, 341, 340, 338, 336, 334, 333, 331, 329, 328, 326, 324,
            323, 321, 320, 318, 317, 315, 314, 312, 311, 309, 308, 306, 305, 303, 302,
            301, 299, 298, 297, 295, 294, 293, 291, 290, 289, 287, 286, 285, 284, 282,
            281, 280, 279, 278, 277, 275, 274, 273, 272, 271, 270, 269, 267, 266, 265,
            264, 263, 262, 261, 260, 259, 258, 257, 256
        };

        #endregion

        #region Functions given in words

        private int get_position() => stream.GetPosition();

        /// <summary>The bytes the last leb128() read (4.11.6).</summary>
        private int Leb128Bytes => stream.Leb128Bytes;

        /// <summary>lookup_bitdepth and lookup_maxq: Table 6.3.</summary>
        private static int lookup_bitdepth(int bitDepthIdc) => bitDepthIdc == 0 ? 10 : bitDepthIdc == 1 ? 8 : throw new InvalidOperationException($"bit_depth_idc {bitDepthIdc} is reserved");

        private static int lookup_maxq(int bitDepthIdc) => bitDepthIdc == 0 ? MAXQ_10_BITS : bitDepthIdc == 1 ? MAXQ_8_BITS : throw new InvalidOperationException($"bit_depth_idc {bitDepthIdc} is reserved");

        /// <summary>Resolve divisor process, 7.13.3.22.</summary>
        private (int, int) resolve_divisor(int d)
        {
            int n = FloorLog2(Abs(d));
            int e = Abs(d) - (1 << n);
            int f = n > DIV_LUT_BITS ? Round2(e, n - DIV_LUT_BITS) : e << (DIV_LUT_BITS - n);
            int divShift = n + DIV_LUT_PREC_BITS;
            int divFactor = d < 0 ? -Div_Lut[f] : Div_Lut[f];
            return (divShift, divFactor);
        }

        /// <summary>get_qindex( ignoreDeltaQ, segmentId ), in the dequantization functions of 7.14.</summary>
        private int get_qindex(int ignoreDeltaQ, int segmentId)
        {
            if (SegFeatureActiveIdx(segmentId, SEG_LVL_ALT_Q) == 1)
            {
                int data = FeatureData[segmentId][SEG_LVL_ALT_Q];
                int qindex = base_q_idx + data;
                if (ignoreDeltaQ == 0 && delta_q_present == 1)
                    qindex = CurrentQIndex + data;
                return Clip3(0, MaxQ, qindex);
            }
            if (ignoreDeltaQ == 0 && delta_q_present == 1)
                return CurrentQIndex;
            return base_q_idx;
        }

        // obu_padding_length of the OBU being read or written; -1 until it is asked for.
        private int _paddingLength = -1;

        /// <summary>
        /// obu_padding_length (5.16): not coded, the payload before its trailing bits - as many bytes as
        /// were read, written. Found once: padding_obu asks for it after each byte it reads.
        /// </summary>
        private int obu_padding_length
        {
            get
            {
                if (_paddingLength < 0)
                    _paddingLength = _writing ? stream.Source?["obu_padding_byte"].Count ?? 0 : PaddingLength();
                return _paddingLength;
            }
        }

        private int PaddingLength()
        {
            {
                long remainingBits = ObuEndPosition - stream.GetPosition();
                var baseStream = stream.Bitstream.BaseStream;
                if (remainingBits <= 0 || stream.GetPosition() % 8 != 0 || !baseStream.CanSeek)
                    return 0;

                var rest = new byte[remainingBits / 8];
                long position = baseStream.Position;
                int read = 0;
                while (read < rest.Length)
                {
                    int count = baseStream.Read(rest, read, rest.Length - read);
                    if (count <= 0)
                        break;
                    read += count;
                }
                baseStream.Position = position;

                int trailing = read > 0 ? Array.FindLastIndex(rest, read - 1, read, b => b != 0) : -1;
                return Math.Max(0, trailing);
            }
        }

        /// <summary>
        /// MultiStreamDecoderMode (7.4.1): 1 once a temporal unit that is a random access point holds an
        /// MSDO OBU. Here: 1 from the first MSDO OBU read.
        /// </summary>
        private int MultiStreamDecoderMode { get; set; }

        /// <summary>
        /// The index of Ccso_Offset that, scaled, is the offset: what an encoder writes as ccso_offset_idx
        /// for a CcsoFilterOffset it chose (5.18.7.12). 0 for an offset that no index gives.
        /// </summary>
        private static int ccso_offset_index(int offset, int scale)
        {
            for (int i = 0; i < Ccso_Offset.Length; i++)
                if (Ccso_Offset[i] * scale == offset)
                    return i;
            return 0;
        }

        #endregion

        #region Reference frames (7.23)

        private int FrameCounter = -1;

        private AomArray<int> RefFrameType = new AomArray<int>();
        private AomArray<int> RefFrameWidth = new AomArray<int>();
        private AomArray<int> RefFrameHeight = new AomArray<int>();
        private AomArray<int> RefCropWidth = new AomArray<int>();
        private AomArray<int> RefCropHeight = new AomArray<int>();
        private AomArray<int> RefCropLeft = new AomArray<int>();
        private AomArray<int> RefCropTop = new AomArray<int>();
        private AomArray<int> RefMiCols = new AomArray<int>();
        private AomArray<int> RefMiRows = new AomArray<int>();
        private AomArray<int> RefSubsamplingX = new AomArray<int>();
        private AomArray<int> RefSubsamplingY = new AomArray<int>();
        private AomArray<int> RefLongTermId = new AomArray<int>();
        private AomArray<int> RefBitDepth = new AomArray<int>();
        private AomArray<int> RefNumPlanes = new AomArray<int>();
        private AomArray<int> RefFilmGrainPresent = new AomArray<int>();
        private AomArray<int> RefImplicitOutputFrame = new AomArray<int>();
        private AomArray<int> RefImmediateOutputFrame = new AomArray<int>();
        private AomArray<int> RefOrderHintLsbs = new AomArray<int>();
        private AomArray<int> RefBaseQIdx = new AomArray<int>();
        private AomArray<int> RefDeltaQUAc = new AomArray<int>();
        private AomArray<int> RefDeltaQVAc = new AomArray<int>();
        private AomArray<AomArray<int>> RefFrameFiltersOn = new AomArray<AomArray<int>>(() => new AomArray<int>());
        private AomArray<AomArray<AomArray<AomArray<int>>>> RefFrameLrWienerNs = new AomArray<AomArray<AomArray<AomArray<int>>>>(() => new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>())));
        private AomArray<int> RefNumFilterClasses = new AomArray<int>();
        private AomArray<int> RefCounter = new AomArray<int>();
        private AomArray<int> RefNumTotalRefs = new AomArray<int>();
        private AomArray<int> RefTLayerId = new AomArray<int>();
        private AomArray<int> RefMLayerId = new AomArray<int>();
        private AomArray<AomArray<int>> SavedOrderHints = new AomArray<AomArray<int>>(() => new AomArray<int>());
        private AomArray<AomArray<AomArray<int>>> SavedGmParams = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));

        /// <summary>
        /// Reference frame update process, 7.23, as far as the headers after it depend on it: what it keeps
        /// of the samples, motion vectors, segment ids and CDFs, SharpAV2 does not have.
        /// </summary>
        private void ReferenceFrameUpdate()
        {
            FrameCounter++;
            int first = 1;
            for (int i = 0; i < NUM_REF_FRAMES; i++)
            {
                if (((refresh_frame_flags >> i) & 1) == 0)
                    continue;

                // is_frame_eligible_for_output and output_frame_buffers output samples: none here.
                RefValid[i] = (FrameType == KEY_FRAME || FrameType == SWITCH_FRAME) ? first : 1;
                first = 0;
                RefFrameWidth[i] = FrameWidth;
                RefFrameHeight[i] = FrameHeight;
                RefCropWidth[i] = CropWidth;
                RefCropHeight[i] = CropHeight;
                RefCropLeft[i] = CropLeft;
                RefCropTop[i] = CropTop;
                RefMiCols[i] = MiCols;
                RefMiRows[i] = MiRows;
                RefFrameType[i] = FrameType;
                RefSubsamplingX[i] = SubsamplingX;
                RefSubsamplingY[i] = SubsamplingY;
                RefLongTermId[i] = LongTermId;
                RefBitDepth[i] = BitDepth;
                RefNumPlanes[i] = NumPlanes;
                RefFilmGrainPresent[i] = film_grain_params_present;
                RefImplicitOutputFrame[i] = implicit_output_frame;
                RefImmediateOutputFrame[i] = immediate_output_frame;
                RefOrderHint[i] = OrderHint;
                RefOrderHintLsbs[i] = OrderHintLsbs;
                RefBaseQIdx[i] = base_q_idx;
                RefDeltaQUAc[i] = DeltaQUAc;
                RefDeltaQVAc[i] = DeltaQVAc;
                RefFrameFiltersOn[i] = frame_filters_on.Clone();
                RefFrameLrWienerNs[i] = FrameLrWienerNs.Clone();
                RefNumFilterClasses[i] = NumFilterClasses;
                RefCounter[i] = FrameCounter;
                RefNumTotalRefs[i] = NumTotalRefs;
                RefTLayerId[i] = obu_tlayer_id;
                RefMLayerId[i] = obu_mlayer_id;
                for (int j = 0; j < REFS_PER_FRAME; j++)
                    SavedOrderHints[i][j] = OrderHints[j];
                SavedGmParams[i] = gm_params.Clone();

                if (film_grain_params_present != 0)
                {
                    load_grain_params(NUM_REF_FRAMES);
                    save_grain_params(i);
                }

                for (int plane = 0; plane <= 2; plane++)
                    save_ccso_params(i, plane);
            }
        }

        /// <summary>Decode frame wrapup process, 7.2, as far as the headers after it depend on it.</summary>
        private void decode_frame_wrapup()
        {
            // The frame level filters and segment ids act on samples: none here. The film grain is kept
            // under NUM_REF_FRAMES, then the reference frames updated; the frames it outputs are samples.
            save_grain_params(NUM_REF_FRAMES);
            ReferenceFrameUpdate();
        }

        #endregion

        #region Saved state

        // Sequence headers are not layer-specific (Table 6.1): kept for the whole stream, by seq_header_id.
        private readonly Dictionary<int, SequenceHeaderObuState> _sequenceHeaders = new Dictionary<int, SequenceHeaderObuState>();
        private Dictionary<int, (FilmGrainModelState, FilmGrainConfigState)> _grainParams = new Dictionary<int, (FilmGrainModelState, FilmGrainConfigState)>();
        private Dictionary<int, FilmGrainModelState> _grainModels = new Dictionary<int, FilmGrainModelState>();
        private Dictionary<(int, int), CcsoState> _ccsoParams = new Dictionary<(int, int), CcsoState>();

        /// <summary>save_sequence_header (6.4.1): all read in sequence_header_obu, under seq_header_id.</summary>
        private void save_sequence_header() => _sequenceHeaders[seq_header_id] = SaveSequenceHeaderObu();

        /// <summary>load_sequence_header( id ) (6.10): what save_sequence_header kept under id.</summary>
        private void load_sequence_header(int id)
        {
            if (!_sequenceHeaders.TryGetValue(id, out var saved))
                throw new InvalidOperationException($"No sequence header {id} has been read.");
            LoadSequenceHeaderObu(saved);
        }

        /// <summary>save_grain_params( i ) (7.23): all that can be read in film_grain_model and film_grain_config.</summary>
        private void save_grain_params(int i) => _grainParams[i] = (SaveFilmGrainModel(), SaveFilmGrainConfig());

        /// <summary>load_grain_params( idx ) (7.21.2): what save_grain_params kept under idx.</summary>
        private void load_grain_params(int idx)
        {
            if (_grainParams.TryGetValue(idx, out var saved))
            {
                LoadFilmGrainModel(saved.Item1);
                LoadFilmGrainConfig(saved.Item2);
            }
        }

        /// <summary>save_grain_model( i ) (6.13): all read in film_grain_model.</summary>
        private void save_grain_model(int i) => _grainModels[i] = SaveFilmGrainModel();

        /// <summary>load_grain_model( idx ) (6.17.10): what save_grain_model kept under idx.</summary>
        private void load_grain_model(int idx)
        {
            if (!_grainModels.TryGetValue(idx, out var saved))
                throw new InvalidOperationException($"No film grain model {idx} has been read.");
            LoadFilmGrainModel(saved);
        }

        /// <summary>What save_ccso_params( i, plane ) keeps (7.23): CcsoLumaSizeLog2, and of these their element for the plane.</summary>
        private sealed class CcsoState
        {
            public int CcsoLumaSizeLog2;
            public int ccso_planes;
            public int ccso_scale_idx;
            public int ccso_bo_only;
            public int ccso_quant_idx;
            public int ccso_ext_filter;
            public int ccso_max_band_log2;
            public int ccso_edge_clf;
            public AomArray<AomArray<AomArray<int>>> CcsoFilterOffset;
            // CcsoBlks[ plane ], which the list names too, is set in the tile data: not here.
        }

        private void save_ccso_params(int i, int plane)
        {
            _ccsoParams[(i, plane)] = new CcsoState
            {
                CcsoLumaSizeLog2 = CcsoLumaSizeLog2,
                ccso_planes = ccso_planes[plane],
                ccso_scale_idx = ccso_scale_idx[plane],
                ccso_bo_only = ccso_bo_only[plane],
                ccso_quant_idx = ccso_quant_idx[plane],
                ccso_ext_filter = ccso_ext_filter[plane],
                ccso_max_band_log2 = ccso_max_band_log2[plane],
                ccso_edge_clf = ccso_edge_clf[plane],
                CcsoFilterOffset = CcsoFilterOffset[plane].Clone(),
            };
        }

        /// <summary>load_ccso_params( i, plane ), 7.23: what save_ccso_params kept.</summary>
        private void load_ccso_params(int i, int plane)
        {
            if (!_ccsoParams.TryGetValue((i, plane), out var saved))
                return;
            CcsoLumaSizeLog2 = saved.CcsoLumaSizeLog2;
            ccso_planes[plane] = saved.ccso_planes;
            ccso_scale_idx[plane] = saved.ccso_scale_idx;
            ccso_bo_only[plane] = saved.ccso_bo_only;
            ccso_quant_idx[plane] = saved.ccso_quant_idx;
            ccso_ext_filter[plane] = saved.ccso_ext_filter;
            ccso_max_band_log2[plane] = saved.ccso_max_band_log2;
            ccso_edge_clf[plane] = saved.ccso_edge_clf;
            CcsoFilterOffset[plane] = saved.CcsoFilterOffset.Clone();
        }

        /// <summary>load_previous( ) (6.17.2): the global motion of the frame the current one is predicted from.</summary>
        private void load_previous()
        {
            int prevFrame = ref_frame_idx[DerivedPrimaryRefFrame];
            PrevGmParams = SavedGmParams[prevFrame].Clone();
        }

        /// <summary>setup_past_independence (6.17.2), less PrevSegmentIds, a segment map.</summary>
        private void setup_past_independence()
        {
            for (int i = 0; i < MAX_SEGMENTS; i++)
            {
                for (int j = 0; j < SEG_LVL_MAX; j++)
                {
                    FeatureData[i][j] = 0;
                    FeatureEnabled[i][j] = 0;
                }
            }
            for (int r = 0; r < REFS_PER_FRAME; r++)
            {
                for (int i = 0; i <= 5; i++)
                    PrevGmParams[r][i] = (i % 3 == 2) ? 1 << WARPEDMODEL_PREC_BITS : 0;
            }
            for (int plane = 0; plane <= 2; plane++)
                ccso_planes[plane] = 0;
        }

        /// <summary>activate_layer_configuration_record( id ) (6.10): which record applies, which the syntax does not read back.</summary>
        private void activate_layer_configuration_record(int id)
        {
            ActiveLayerConfigurationRecord = id;
        }

        /// <summary>The id activate_layer_configuration_record was last called with.</summary>
        public int ActiveLayerConfigurationRecord { get; private set; } = -1;

        #endregion

        #region Extended layer contexts (7.6)

        // The state of each extended layer but the one being read; the one being read is the live state.
        private readonly Dictionary<int, ContextState> _contexts = new Dictionary<int, ContextState>();
        private int _currentStream = -1;

        /// <summary>
        /// load_context( streamID ): the decoder state of the extended layer. The live state is the layer's
        /// last read, so it is swapped only when another layer's OBU comes.
        /// </summary>
        private void load_context(int streamID)
        {
            if (streamID == _currentStream)
                return;
            if (_currentStream >= 0)
                _contexts[_currentStream] = SaveContext();
            if (_contexts.TryGetValue(streamID, out var saved))
                LoadContext(saved);
            _currentStream = streamID;
        }

        /// <summary>save_context( streamID ): the live state is the layer's until another layer's is loaded.</summary>
        private void save_context(int streamID)
        {
            _currentStream = streamID;
        }

        // The state this part keeps, which a layer's context holds too.
        private sealed partial class ContextState
        {
            public int FrameCounter;
            public AomArray<int> RefFrameType, RefFrameWidth, RefFrameHeight, RefCropWidth, RefCropHeight, RefCropLeft, RefCropTop,
                RefMiCols, RefMiRows, RefSubsamplingX, RefSubsamplingY, RefLongTermId, RefBitDepth, RefNumPlanes, RefFilmGrainPresent,
                RefImplicitOutputFrame, RefImmediateOutputFrame, RefOrderHintLsbs, RefBaseQIdx, RefDeltaQUAc, RefDeltaQVAc,
                RefNumFilterClasses, RefCounter, RefNumTotalRefs, RefTLayerId, RefMLayerId;
            public AomArray<AomArray<int>> RefFrameFiltersOn, SavedOrderHints;
            public AomArray<AomArray<AomArray<AomArray<int>>>> RefFrameLrWienerNs;
            public AomArray<AomArray<AomArray<int>>> SavedGmParams;
            public Dictionary<int, (FilmGrainModelState, FilmGrainConfigState)> GrainParams;
            public Dictionary<int, FilmGrainModelState> GrainModels;
            public Dictionary<(int, int), CcsoState> CcsoParams;
        }

        partial void SaveContextExtra(ContextState state)
        {
            state.FrameCounter = FrameCounter;
            state.RefFrameType = RefFrameType.Clone(); state.RefFrameWidth = RefFrameWidth.Clone(); state.RefFrameHeight = RefFrameHeight.Clone();
            state.RefCropWidth = RefCropWidth.Clone(); state.RefCropHeight = RefCropHeight.Clone(); state.RefCropLeft = RefCropLeft.Clone(); state.RefCropTop = RefCropTop.Clone();
            state.RefMiCols = RefMiCols.Clone(); state.RefMiRows = RefMiRows.Clone(); state.RefSubsamplingX = RefSubsamplingX.Clone(); state.RefSubsamplingY = RefSubsamplingY.Clone();
            state.RefLongTermId = RefLongTermId.Clone(); state.RefBitDepth = RefBitDepth.Clone(); state.RefNumPlanes = RefNumPlanes.Clone(); state.RefFilmGrainPresent = RefFilmGrainPresent.Clone();
            state.RefImplicitOutputFrame = RefImplicitOutputFrame.Clone(); state.RefImmediateOutputFrame = RefImmediateOutputFrame.Clone(); state.RefOrderHintLsbs = RefOrderHintLsbs.Clone();
            state.RefBaseQIdx = RefBaseQIdx.Clone(); state.RefDeltaQUAc = RefDeltaQUAc.Clone(); state.RefDeltaQVAc = RefDeltaQVAc.Clone();
            state.RefNumFilterClasses = RefNumFilterClasses.Clone(); state.RefCounter = RefCounter.Clone(); state.RefNumTotalRefs = RefNumTotalRefs.Clone();
            state.RefTLayerId = RefTLayerId.Clone(); state.RefMLayerId = RefMLayerId.Clone();
            state.RefFrameFiltersOn = RefFrameFiltersOn.Clone(); state.SavedOrderHints = SavedOrderHints.Clone();
            state.RefFrameLrWienerNs = RefFrameLrWienerNs.Clone(); state.SavedGmParams = SavedGmParams.Clone();
            // What these hold is not changed once saved, so the dictionaries are copied, not their states.
            state.GrainParams = new Dictionary<int, (FilmGrainModelState, FilmGrainConfigState)>(_grainParams);
            state.GrainModels = new Dictionary<int, FilmGrainModelState>(_grainModels);
            state.CcsoParams = new Dictionary<(int, int), CcsoState>(_ccsoParams);
        }

        partial void LoadContextExtra(ContextState state)
        {
            FrameCounter = state.FrameCounter;
            RefFrameType = state.RefFrameType.Clone(); RefFrameWidth = state.RefFrameWidth.Clone(); RefFrameHeight = state.RefFrameHeight.Clone();
            RefCropWidth = state.RefCropWidth.Clone(); RefCropHeight = state.RefCropHeight.Clone(); RefCropLeft = state.RefCropLeft.Clone(); RefCropTop = state.RefCropTop.Clone();
            RefMiCols = state.RefMiCols.Clone(); RefMiRows = state.RefMiRows.Clone(); RefSubsamplingX = state.RefSubsamplingX.Clone(); RefSubsamplingY = state.RefSubsamplingY.Clone();
            RefLongTermId = state.RefLongTermId.Clone(); RefBitDepth = state.RefBitDepth.Clone(); RefNumPlanes = state.RefNumPlanes.Clone(); RefFilmGrainPresent = state.RefFilmGrainPresent.Clone();
            RefImplicitOutputFrame = state.RefImplicitOutputFrame.Clone(); RefImmediateOutputFrame = state.RefImmediateOutputFrame.Clone(); RefOrderHintLsbs = state.RefOrderHintLsbs.Clone();
            RefBaseQIdx = state.RefBaseQIdx.Clone(); RefDeltaQUAc = state.RefDeltaQUAc.Clone(); RefDeltaQVAc = state.RefDeltaQVAc.Clone();
            RefNumFilterClasses = state.RefNumFilterClasses.Clone(); RefCounter = state.RefCounter.Clone(); RefNumTotalRefs = state.RefNumTotalRefs.Clone();
            RefTLayerId = state.RefTLayerId.Clone(); RefMLayerId = state.RefMLayerId.Clone();
            RefFrameFiltersOn = state.RefFrameFiltersOn.Clone(); SavedOrderHints = state.SavedOrderHints.Clone();
            RefFrameLrWienerNs = state.RefFrameLrWienerNs.Clone(); SavedGmParams = state.SavedGmParams.Clone();
            _grainParams = new Dictionary<int, (FilmGrainModelState, FilmGrainConfigState)>(state.GrainParams);
            _grainModels = new Dictionary<int, FilmGrainModelState>(state.GrainModels);
            _ccsoParams = new Dictionary<(int, int), CcsoState>(state.CcsoParams);
        }

        #endregion

        #region Decoding processes (no-ops: SharpAV2 reads the headers)

        private long _tileEnd;

        private long[] tileStarts = new long[1];
        private int[] tileSizes = new int[1];

        /// <summary>
        /// How many tiles the last OBU read or written had - what Common Encryption would protect, tile by tile - their
        /// bytes found without decoding them (<see cref="TileStart"/>, <see cref="TileSize"/>). A bridge frame's tile
        /// groups, and those of an inactive BRU frame, have none.
        /// </summary>
        public int TileCount { get; private set; }

        /// <summary>Where the tile i of the last OBU starts, in bits from the start of the stream it was read from.</summary>
        public long TileStart(int i) => i < TileCount ? tileStarts[i] : throw new ArgumentOutOfRangeException(nameof(i));

        /// <summary>The size in bytes of the tile i of the last OBU: of what the OBU holds of it.</summary>
        public int TileSize(int i) => i < TileCount ? tileSizes[i] : throw new ArgumentOutOfRangeException(nameof(i));

        /// <summary>init_symbol( sz ) (8.2.2): the tile's sz bytes are skipped whole by exit_symbol, and where they lie noted.</summary>
        private void init_symbol(int sz)
        {
            long start = stream.GetPosition();
            _tileEnd = start + (long)sz * 8;

            if (TileCount == tileStarts.Length)
            {
                Array.Resize(ref tileStarts, TileCount * 2);
                Array.Resize(ref tileSizes, TileCount * 2);
            }
            tileStarts[TileCount] = start;
            tileSizes[TileCount] = (int)Math.Max(0, Math.Min((long)sz, (ObuEndPosition - start) / 8));
            TileCount++;
        }

        /// <summary>
        /// exit_symbol( ) (8.2.4): to the end of the tile. Its bytes are kept where the OBU is recorded, and
        /// written as they were.
        /// </summary>
        private void exit_symbol()
        {
            long left = _tileEnd - stream.GetPosition();
            if (left <= 0)
                return;
            if (_writing)
            {
                byte[] tile = stream.Pick("tile_data", (byte[])null, null)
                    ?? throw new InvalidOperationException("The tile data was not recorded: SharpAV2 writes tiles as they were read.");
                stream.WriteBytes((int)left, tile, "tile_data");
            }
            else if (stream.Record != null)
                stream.ReadBytes((int)left, out _, "tile_data");
            else
                stream.Skip(left);
        }

        /// <summary>decode_tile( ) (5.20.2.1): the tile's blocks, not read here.</summary>
        private void decode_tile() { }

        /// <summary>CDFs (init_coeff_cdfs, init_non_coeff_cdfs, load_cdfs, blend_cdfs, frame_end_update_cdf): the tile data's.</summary>
        private void init_coeff_cdfs() { }

        private void init_non_coeff_cdfs() { }

        private void load_cdfs(int ctx) { }

        private void blend_cdfs(int ctx) { }

        private void frame_end_update_cdf() { }

        /// <summary>
        /// Records a projection of the motion field (7.9.1), which is the tile data's: only its count is kept.
        /// </summary>
        private int record_tip_projection(int src, int dstSign, int dst, int processCount) => processCount + 1;

        private void setup_tip_motion_field() { }

        private void fill_tpl_mvs_sample_gap() { }

        /// <summary>load_previous_segment_ids (6.17.2): a segment map.</summary>
        private void load_previous_segment_ids() { }

        /// <summary>Output processes (7.21): the frames output are samples.</summary>
        private int is_frame_eligible_for_output(int i) => 0;

        private void output_frame_buffers(int i) { }

        private void flush_implicit_output_frames(int all) { }

        #endregion

    }
}
