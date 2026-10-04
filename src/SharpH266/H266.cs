using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpH26X;

namespace SharpH266
{
    public class H266Constants
    {
        public const int ALF_APS = 0; // ALF parameters
        public const int LMCS_APS = 1; // LMCS parameters
        public const int SCALING_APS = 2; // ScalingList parameters
    }

    public class H266FrameTypes
    {
        public const ulong B = 0;
        public const ulong P = 1;
        public const ulong I = 2;

        public static bool IsB(ulong value) { return value == B; }
        public static bool IsP(ulong value) { return value == P; }
        public static bool IsI(ulong value) { return value == I; }
    }

    public class H266NALTypes
    {          
        public const uint TRAIL_NUT = 0;                     // Coded slice of a trailing picture or subpicture
        public const uint STSA_NUT = 1;                      // Coded slice of an STSA picture or subpicture
        public const uint RADL_NUT = 2;                      // Coded slice of a random access picture or subpicture
        public const uint RASL_NUT = 3;                      // Coded slice of a random access picture or subpicture
        
        public const uint RSV_VCL_4 = 4;                     // Reserved for future use
        public const uint RSV_VCL_5 = 5;                     // Reserved for future use
        public const uint RSV_VCL_6 = 6;                     // Reserved for future use

        public const uint IDR_W_RADL = 7;                    // Coded slice of an IDR picture or subpicture
        public const uint IDR_N_LP = 8;                      // Coded slice of an IDR picture or subpicture
        public const uint CRA_NUT = 9;                       // Coded slice of a CRA picture or subpicture
        public const uint GDR_NUT = 10;                      // Coded slice of a GDR picture or subpicture
                                                             // 
        public const uint RSV_IRAP_11 = 11;                  // Reserved IRAP VCL NAL unit
        public const uint OPI_NUT = 12;                      // Operating Point Information
        public const uint DCI_NUT = 13;                      // Decoding Capability Information
        public const uint VPS_NUT = 14;                      // Video parameter set
        public const uint SPS_NUT = 15;                      // Sequence parameter set
        public const uint PPS_NUT = 16;                      // Picture parameter set
        public const uint PREFIX_APS_NUT = 17;               // Adaptation Parameter Set
        public const uint SUFFIX_APS_NUT = 18;               // Adaptation Parameter Set
        public const uint PH_NUT = 19;                       // Picture Header
        public const uint AUD_NUT = 20;                      // AU Delimiter
        public const uint EOS_NUT = 21;                      // End of Sequence
        public const uint EOB_NUT = 22;                      // End of Bitstream
        public const uint PREFIX_SEI_NUT = 23;               // Supplemental enhancement information
        public const uint SUFFIX_SEI_NUT = 24;               // Supplemental enhancement information
        public const uint FD_NUT = 25;                       // Filler Data

        public const uint RSV_NVCL_26 = 26;                  // Reserved non-VCL NAL unit types
        public const uint RSV_NVCL_27 = 27;                  // Reserved non-VCL NAL unit types

        public const uint UNSPEC_28 = 28;                    // Unspecified
        public const uint UNSPEC_29 = 29;                    // Unspecified
        public const uint UNSPEC_30 = 30;                    // Unspecified
        public const uint UNSPEC_31 = 31;                    // Unspecified
    }

    public partial class H266Context
    {
        /// <summary>
        /// DiagScanOrder[ log2BlockWidth ][ log2BlockHeight ][ sPos ][ sComp ] (6.5.2): the up-right
        /// diagonal scan of each block size, x then y.
        /// </summary>
        public static readonly int[][][][] DiagScanOrder = BuildDiagScanOrder();

        private static int[][][][] BuildDiagScanOrder()
        {
            var order = new int[6][][][];
            for (int log2W = 0; log2W < 6; log2W++)
            {
                order[log2W] = new int[6][][];
                for (int log2H = 0; log2H < 6; log2H++)
                {
                    int width = 1 << log2W, height = 1 << log2H;
                    var scan = new int[width * height][];
                    int i = 0, x = 0, y = 0;
                    bool stop = false;
                    while (!stop)
                    {
                        while (y >= 0)
                        {
                            if (x < width && y < height)
                                scan[i++] = [x, y];
                            y--;
                            x++;
                        }
                        y = x;
                        x = 0;
                        if (i >= width * height)
                            stop = true;
                    }
                    order[log2W][log2H] = scan;
                }
            }
            return order;
        }

        /// <summary>ScalingList[ id ][ i ]: 28 lists of up to 64 coefficients.</summary>
        public static uint[][] NewScalingList()
        {
            var list = new uint[28][];
            for (int id = 0; id < 28; id++)
                list[id] = new uint[64];
            return list;
        }

        internal byte[][] cbr_flag;
        internal ulong[][] bit_rate_du_value_minus1;
        internal ulong[][] cpb_size_du_value_minus1;
        internal ulong[][] bit_rate_value_minus1;
        internal ulong[][] cpb_size_value_minus1;
        internal ulong[][] num_ref_entries;
        internal byte[][] ltrp_in_header_flag;
        internal byte[][][] inter_layer_ref_pic_flag;
        internal byte[][][] st_ref_pic_flag;
        internal ulong[][][] abs_delta_poc_st;
        internal byte[][][] strp_entry_sign_flag;
        internal ulong[][][] rpls_poc_lsb_lt;
        internal ulong[][][] ilrp_idx;

        public SeiPayload SeiPayload { get; set; }
        public GeneralTimingHrdParameters GeneralTimingHrdParameters { get; set; }
        public Dictionary<ulong, SeqParameterSetRbsp> SeqParameterSets { get; } = new Dictionary<ulong, SeqParameterSetRbsp>();
        public Dictionary<ulong, PicParameterSetRbsp> PicParameterSets { get; } = new Dictionary<ulong, PicParameterSetRbsp>();

        public uint olsModeIdc { get; set; }
        public uint TotalNumOlss { get; set; }
        public ulong VpsNumDpbParams { get; set; }
        public uint[] NumOutputLayersInOls { get; set; }
        public uint[][] OutputLayerIdInOls { get; set; }
        public uint[][] layerIncludedInOlsFlag { get; set; }
        public uint[][] NumSubLayersInLayerInOLS { get; set; }
        public uint[] LayerUsedAsOutputLayerFlag { get; set; }
        public int[][] OutputLayerIdx { get; set; }
        public uint[][] LayerIdInOls { get; set; }
        public ulong NumMultiLayerOlss { get; set; }
        public ulong[] MultiLayerOlsIdx { get; set; }
        public uint[] LayerUsedAsRefLayerFlag { get; set; }
        public int[] NumDirectRefLayers { get; set; }
        public int[] NumRefLayers { get; set; }
        public int[] GeneralLayerIdx { get; set; }
        public int[][] DirectRefLayerIdx { get; set; }
        public int[][] ReferenceLayerIdx { get; set; }
        public uint[] NumLayersInOls { get; set; }
        public uint CtbLog2SizeY { get; set; }
        public uint CtbSizeY { get; set; }
        public ulong MaxNumMergeCand { get; set; }
        public int NumExtraPhBits { get; set; }
        public int NumAlfFilters { get; set; } = 25; // a constant (7.4.3.18); it was set only when chroma filters were signalled
        public ulong LmcsMaxBinIdx { get; set; }
        public uint PicWidthInCtbsY { get; set; }
        public uint PicHeightInCtbsY { get; set; }
        public uint PicSizeInCtbsY { get; set; }
        public ulong PicWidthInMinCbsY { get; set; }
        public ulong PicHeightInMinCbsY { get; set; }
        public ulong PicSizeInMinCbsY { get; set; }
        public ulong PicSizeInSamplesY { get; set; }
        public ulong PicWidthInSamplesC { get; set; }
        public ulong PicHeightInSamplesC { get; set; }
        public uint SubWidthC { get; set; }
        public uint SubHeightC { get; set; }
        public ulong MinCbLog2SizeY { get; set; }
        public uint MinCbSizeY { get; set; }
        public uint IbcBufWidthY { get; set; }
        public uint IbcBufWidthC { get; set; }
        public uint VSize { get; set; }
        public uint CtbWidthC { get; set; }
        public uint CtbHeightC { get; set; }
        public int NumTileColumns { get; set; }
        public int NumTileRows { get; set; }
        public int NumTilesInPic { get; set; }
        public ulong[] ColWidthVal { get; set; }
        public ulong[] RowHeightVal { get; set; }
        public ulong[] TileColBdVal { get; set; }
        public ulong[] TileRowBdVal { get; set; }
        public ulong[] CtbToTileColBd { get; set; }
        public uint[] ctbToTileColIdx { get; set; }
        public ulong[] CtbToTileRowBd { get; set; }
        public uint[] ctbToTileRowIdx { get; set; }
        public uint[] SubpicWidthInTiles { get; set; }
        public uint[] SubpicHeightInTiles { get; set; }
        public uint[] subpicHeightLessThanOneTileFlag { get; set; }
        public uint[] NumCtusInSlice { get; set; }
        public uint[] SliceTopLeftTileIdx { get; set; }
        public ulong[] sliceWidthInTiles { get; set; }
        public ulong[] sliceHeightInTiles { get; set; }
        public uint[] NumSlicesInTile { get; set; }
        public ulong[] sliceHeightInCtus { get; set; }
        public ulong[][] CtbAddrInSlice { get; set; }
        public uint[] NumSlicesInSubpic { get; set; }
        public int[] SubpicIdxForSlice { get; set; }
        public uint[] SubpicLevelSliceIdx { get; set; }
        /// <summary>
        /// NumLtrpEntries[ listIdx ][ rplsIdx ] (7.4.10): the long-term entries of each reference
        /// picture list structure, counted from the tables as they are now.
        /// </summary>
        public uint[][] NumLtrpEntries
        {
            get
            {
                var counts = new uint[2][];
                for (int listIdx = 0; listIdx < 2; listIdx++)
                {
                    var entries = num_ref_entries?[listIdx];
                    counts[listIdx] = new uint[entries?.Length ?? 0];
                    for (int rplsIdx = 0; rplsIdx < counts[listIdx].Length; rplsIdx++)
                    {
                        var interLayer = inter_layer_ref_pic_flag?[listIdx]?[rplsIdx];
                        var shortTerm = st_ref_pic_flag?[listIdx]?[rplsIdx];
                        for (ulong i = 0; i < entries[rplsIdx]; i++)
                        {
                            bool isInterLayer = interLayer != null && interLayer[i] != 0;
                            bool isShortTerm = shortTerm != null && shortTerm[i] != 0;
                            if (!isInterLayer && !isShortTerm)
                                counts[listIdx][rplsIdx]++;
                        }
                    }
                }
                return counts;
            }
        }
        public ulong[] RplsIdx { get; set; } = new ulong[2];
        public ulong NumWeightsL0 { get; set; }
        public ulong[] NumRefIdxActive { get; set; }
        public int CurrSubpicIdx { get; set; }
        public ulong[] SubpicIdVal { get; set; }
        public int NumExtraShBits { get; set; }
        public int NumEntryPoints { get; set; }
        public uint NumCtusInCurrSlice { get; set; }
        public ulong[] CtbAddrInCurrSlice { get; set; }
        public ulong NumWeightsL1 { get; set; }
        public ulong[][][] AbsDeltaPocSt { get; set; }

        public void SetGeneralTimingHrdParameters(GeneralTimingHrdParameters generalTimingHrdParameters)
        {
            GeneralTimingHrdParameters = generalTimingHrdParameters;
        }

        public void SetSeiPayload(SeiPayload payload)
        {
            if (SeiPayload == null)
            {
                SeiPayload = payload;
            }

            if (payload.AlternativeTransferCharacteristics != null)
                SeiPayload.AlternativeTransferCharacteristics = payload.AlternativeTransferCharacteristics;
            if (payload.AmbientViewingEnvironment != null)
                SeiPayload.AmbientViewingEnvironment = payload.AmbientViewingEnvironment;
            if (payload.BufferingPeriod != null)
                SeiPayload.BufferingPeriod = payload.BufferingPeriod;
            if (payload.ContentColourVolume != null)
                SeiPayload.ContentColourVolume = payload.ContentColourVolume;
            if (payload.ContentLightLevelInfo != null)
                SeiPayload.ContentLightLevelInfo = payload.ContentLightLevelInfo;
            if (payload.DecodedPictureHash != null)
                SeiPayload.DecodedPictureHash = payload.DecodedPictureHash;
            if (payload.DecodingUnitInfo != null)
                SeiPayload.DecodingUnitInfo = payload.DecodingUnitInfo;
            if (payload.DependentRapIndication != null)
                SeiPayload.DependentRapIndication = payload.DependentRapIndication;
            if (payload.EquirectangularProjection != null)
                SeiPayload.EquirectangularProjection = payload.EquirectangularProjection;
            if (payload.FillerPayload != null)
                SeiPayload.FillerPayload = payload.FillerPayload;
            if (payload.FilmGrainCharacteristics != null)
                SeiPayload.FilmGrainCharacteristics = payload.FilmGrainCharacteristics;
            if (payload.FrameFieldInfo != null)
                SeiPayload.FrameFieldInfo = payload.FrameFieldInfo;
            if (payload.FramePackingArrangement != null)
                SeiPayload.FramePackingArrangement = payload.FramePackingArrangement;
            if (payload.GeneralizedCubemapProjection != null)
                SeiPayload.GeneralizedCubemapProjection = payload.GeneralizedCubemapProjection;
            if (payload.MasteringDisplayColourVolume != null)
                SeiPayload.MasteringDisplayColourVolume = payload.MasteringDisplayColourVolume;
            if (payload.OmniViewport != null)
                SeiPayload.OmniViewport = payload.OmniViewport;
            if (payload.ParameterSetsInclusionIndication != null)
                SeiPayload.ParameterSetsInclusionIndication = payload.ParameterSetsInclusionIndication;
            if (payload.PicTiming != null)
                SeiPayload.PicTiming = payload.PicTiming;
            if (payload.RegionwisePacking != null)
                SeiPayload.RegionwisePacking = payload.RegionwisePacking;
            if (payload.ReservedMessage != null)
                SeiPayload.ReservedMessage = payload.ReservedMessage;
            if (payload.SampleAspectRatioInfo != null)
                SeiPayload.SampleAspectRatioInfo = payload.SampleAspectRatioInfo;
            if (payload.ScalableNesting != null)
                SeiPayload.ScalableNesting = payload.ScalableNesting;
            if (payload.SphereRotation != null)
                SeiPayload.SphereRotation = payload.SphereRotation;
            if (payload.SubpicLevelInfo != null)
                SeiPayload.SubpicLevelInfo = payload.SubpicLevelInfo;
            if (payload.UserDataRegisteredItutT35 != null)
                SeiPayload.UserDataRegisteredItutT35 = payload.UserDataRegisteredItutT35;
            if (payload.UserDataUnregistered != null)
                SeiPayload.UserDataUnregistered = payload.UserDataUnregistered;
        }

        /// <summary>
        /// Several VPS flags are coded only in some VPSs, and inferred otherwise (7.4.3.3):
        /// vps_default_ptl_dpb_hrd_max_tid_flag and vps_all_independent_layers_flag are 1,
        /// vps_each_layer_is_an_ols_flag is 1 with one layer, 0 else, and vps_ols_mode_idc is 2.
        /// Left 0, a VPS with one sublayer read vps_ptl_max_tid that was not there, and one with
        /// independent layers but no OLS per layer skipped its output layer sets.
        /// </summary>
        public void OnVpsMaxLayersMinus1(VideoParameterSetRbsp vps)
        {
            vps.VpsDefaultPtlDpbHrdMaxTidFlag = 1;
            vps.VpsAllIndependentLayersFlag = 1;
            vps.VpsEachLayerIsAnOlsFlag = (byte)(vps.VpsMaxLayersMinus1 == 0 ? 1 : 0);
            vps.VpsOlsModeIdc = 2;
        }

        /// <summary>
        /// TotalNumOlss (7-36), worked out where it is used: it was set only when
        /// vps_num_output_layer_sets_minus2 was coded, and kept the last VPS's value otherwise.
        /// </summary>
        public uint DeriveTotalNumOlss()
        {
            var vps = VideoParameterSetRbsp;
            if (vps == null)
                return 1;

            olsModeIdc = vps.VpsEachLayerIsAnOlsFlag == 0 ? vps.VpsOlsModeIdc : 4;
            if (olsModeIdc == 4 || olsModeIdc == 0 || olsModeIdc == 1)
                TotalNumOlss = vps.VpsMaxLayersMinus1 + 1;
            else if (olsModeIdc == 2)
                TotalNumOlss = vps.VpsNumOutputLayerSetsMinus2 + 2;
            return TotalNumOlss;
        }

        /// <summary>
        /// Whether an SEI message can be read within its payloadSize, read ahead. A buffering period
        /// that signals VCL parameters but carries only the NAL ones (HRD_B_2) ran past the NAL
        /// unit, and the pic timing after it had no buffering period to take its lengths from.
        /// </summary>
        public bool FitsItsPayload(ItuStream stream, ulong payloadSize, IItuSerializable message)
        {
            using var ahead = stream.Lookahead();
            try
            {
                return message.Read(this, ahead) <= payloadSize * 8;
            }
            catch (Exception)
            {
                return false; // read past the end, or into values nothing could hold
            }
        }

        public void OnVpsNumDpbParamsMinus1()
        {
            var vps_each_layer_is_an_ols_flag = VideoParameterSetRbsp.VpsEachLayerIsAnOlsFlag;
            var vps_num_dpb_params_minus1 = VideoParameterSetRbsp.VpsNumDpbParamsMinus1;

            if (vps_each_layer_is_an_ols_flag != 0)
                VpsNumDpbParams = 0;
            else
                VpsNumDpbParams = vps_num_dpb_params_minus1 + 1;
        }

        public void OnVpsDirectRefLayerFlag()
        {
            var vps_max_layers_minus1 = VideoParameterSetRbsp.VpsMaxLayersMinus1;
            var vps_direct_ref_layer_flag = VideoParameterSetRbsp.VpsDirectRefLayerFlag;
            var vps_layer_id = VideoParameterSetRbsp.VpsLayerId;

            // Worked out at every flag, so the rows of the layers not read yet are 0 so far.
            uint Direct(int i, int j) =>
                vps_direct_ref_layer_flag?[i] != null && j < vps_direct_ref_layer_flag[i].Length ? vps_direct_ref_layer_flag[i][j] : 0u;

            uint[][] dependencyFlag = new uint[vps_max_layers_minus1 + 1][];
            for (int i = 0; i < dependencyFlag.Length; i++)
            {
                dependencyFlag[i] = new uint[vps_max_layers_minus1 + 1];
            }
            if (DirectRefLayerIdx == null || DirectRefLayerIdx.Length < vps_max_layers_minus1 + 1)
            {
                DirectRefLayerIdx = new int[vps_max_layers_minus1 + 1][];
                for (int i = 0; i < DirectRefLayerIdx.Length; i++)
                {
                    DirectRefLayerIdx[i] = new int[vps_max_layers_minus1 + 1];
                }
            }
            if (ReferenceLayerIdx == null || ReferenceLayerIdx.Length < vps_max_layers_minus1 + 1)
            {
                ReferenceLayerIdx = new int[vps_max_layers_minus1 + 1][];
                for (int i = 0; i < ReferenceLayerIdx.Length; i++)
                {
                    ReferenceLayerIdx[i] = new int[vps_max_layers_minus1 + 1];
                }
            }

            if (LayerUsedAsRefLayerFlag == null || LayerUsedAsRefLayerFlag.Length < vps_max_layers_minus1 + 1)
                LayerUsedAsRefLayerFlag = new uint[vps_max_layers_minus1 + 1];
            if (NumDirectRefLayers == null || NumDirectRefLayers.Length < vps_max_layers_minus1 + 1)
                NumDirectRefLayers = new int[vps_max_layers_minus1 + 1];
            if (NumRefLayers == null || NumRefLayers.Length < vps_max_layers_minus1 + 1)
                NumRefLayers = new int[vps_max_layers_minus1 + 1];
            // Indexed by nuh_layer_id, up to 63, not by the count of layers.
            if (GeneralLayerIdx == null || GeneralLayerIdx.Length < 64)
                GeneralLayerIdx = new int[64];

            for (int i = 0; i <= vps_max_layers_minus1; i++)
            {
                for (int j = 0; j <= vps_max_layers_minus1; j++)
                {
                    dependencyFlag[i][j] = Direct(i, j);
                    for (int k = 0; k < i; k++)
                        if (Direct(i, k) != 0 && dependencyFlag[k][j] != 0)
                            dependencyFlag[i][j] = 1;
                }
                LayerUsedAsRefLayerFlag[i] = 0;
            }

            for (int i = 0; i <= vps_max_layers_minus1; i++)
            {
                int d = 0, r = 0;
                for (int j = 0; j <= vps_max_layers_minus1; j++)
                {
                    if (Direct(i, j) != 0)
                    {
                        DirectRefLayerIdx[i][d++] = j;
                        LayerUsedAsRefLayerFlag[j] = 1;
                    }
                    if (dependencyFlag[i][j] != 0)
                        ReferenceLayerIdx[i][r++] = j;
                }
                NumDirectRefLayers[i] = d;
                NumRefLayers[i] = r;
            }

            for (int i = 0; i <= vps_max_layers_minus1; i++)
                GeneralLayerIdx[vps_layer_id[i]] = i;
        }

        /// <summary>
        /// The output layer sets (7-37 to 7-41), worked out where NumMultiLayerOlss is used, after
        /// every element they take is read. They were worked out at each vps_ols_output_layer_flag,
        /// coded only when vps_ols_mode_idc is 2, and before the profile indices they take.
        /// </summary>
        /// <summary>
        /// Reject what a conforming stream cannot contain, as ffmpeg's reader does, rather than
        /// read on: an output layer set without an output layer, and so the VPS it is in, and the
        /// parameter sets and pictures that depend on a set rejected. Off by default.
        /// </summary>
        public bool Strict { get; set; }

        private readonly HashSet<ulong> rejectedVps = new HashSet<ulong>();

        /// <summary>
        /// With <see cref="Strict"/>, the checks on the output layer sets once they are worked out:
        /// NumOutputLayersInOls[ i ] is at least 1 for every OLS (7.4.3.3), and a VPS without an
        /// OLS per layer has an OLS of more than one layer.
        /// </summary>
        private void CheckOls(VideoParameterSetRbsp vps, uint total)
        {
            for (int i = 1; i < total; i++)
            {
                if (NumOutputLayersInOls[i] == 0)
                {
                    rejectedVps.Add(vps.VpsVideoParameterSetId);
                    throw new InvalidDataException($"Output layer set {i} has no output layer.");
                }
            }

            if (vps.VpsEachLayerIsAnOlsFlag == 0 && NumMultiLayerOlss == 0)
            {
                rejectedVps.Add(vps.VpsVideoParameterSetId);
                throw new InvalidDataException("No output layer set has more than one layer.");
            }
        }

        /// <summary>With <see cref="Strict"/>, an SPS naming a VPS that was rejected is rejected too.</summary>
        public void OnSpsVideoParameterSetId(ulong sps_video_parameter_set_id)
        {
            if (Strict && rejectedVps.Contains(sps_video_parameter_set_id))
            {
                SeqParameterSets.Remove(SeqParameterSetRbsp.SpsSeqParameterSetId);
                throw new InvalidDataException($"VPS {sps_video_parameter_set_id} is not available.");
            }
        }

        public ulong DeriveOls()
        {
            var vps = VideoParameterSetRbsp;
            if (vps == null)
                return NumMultiLayerOlss = 0;

            uint total = DeriveTotalNumOlss();
            uint layers = vps.VpsMaxLayersMinus1 + 1;
            var vps_layer_id = vps.VpsLayerId;
            var vps_each_layer_is_an_ols_flag = vps.VpsEachLayerIsAnOlsFlag;
            var vps_ols_output_layer_flag = vps.VpsOlsOutputLayerFlag;
            OnVpsDirectRefLayerFlag();

            // Inferred where not coded: vps_ptl_max_tid and vps_max_tid_il_ref_pics_plus1 (7.4.3.3)
            // from vps_max_sublayers_minus1, vps_ols_ptl_idx from the number of PTLs.
            uint PtlIdx(int i) =>
                vps.VpsNumPtlsMinus1 + 1 == total ? (uint)i :
                vps.VpsNumPtlsMinus1 == 0 || vps.VpsOlsPtlIdx == null || i >= vps.VpsOlsPtlIdx.Length ? 0 : vps.VpsOlsPtlIdx[i];
            uint SubLayers(int i)
            {
                uint idx = PtlIdx(i);
                if (vps.VpsDefaultPtlDpbHrdMaxTidFlag != 0 || vps.VpsPtlMaxTid == null || idx >= vps.VpsPtlMaxTid.Length)
                    return vps.VpsMaxSublayersMinus1 + 1;
                return vps.VpsPtlMaxTid[idx] + 1;
            }
            uint Direct(int m, int k) =>
                vps.VpsDirectRefLayerFlag?[m] != null && k < vps.VpsDirectRefLayerFlag[m].Length ? vps.VpsDirectRefLayerFlag[m][k] : 0u;
            uint MaxTidIlRef(int m, int k) =>
                vps.VpsMaxTidRefPresentFlag?[m] == 1 && vps.VpsMaxTidIlRefPicsPlus1?[m] != null && k < vps.VpsMaxTidIlRefPicsPlus1[m].Length ?
                    vps.VpsMaxTidIlRefPicsPlus1[m][k] : vps.VpsMaxSublayersMinus1 + 1;
            uint OutputFlag(int i, int k) =>
                vps_ols_output_layer_flag?[i] != null && k < vps_ols_output_layer_flag[i].Length ? vps_ols_output_layer_flag[i][k] : 0u;

            uint[][] Rows(uint count) => Enumerable.Range(0, (int)count).Select(_ => new uint[layers]).ToArray();
            NumOutputLayersInOls = new uint[total];
            OutputLayerIdInOls = Rows(total);
            NumSubLayersInLayerInOLS = Rows(total);
            layerIncludedInOlsFlag = Rows(total);
            OutputLayerIdx = Enumerable.Range(0, (int)total).Select(_ => new int[layers]).ToArray();
            LayerIdInOls = Rows(total);
            LayerUsedAsOutputLayerFlag = new uint[layers];
            MultiLayerOlsIdx = new ulong[total];
            NumLayersInOls = new uint[total];

            NumOutputLayersInOls[0] = 1;
            OutputLayerIdInOls[0][0] = vps_layer_id[0];
            NumSubLayersInLayerInOLS[0][0] = SubLayers(0);
            LayerUsedAsOutputLayerFlag[0] = 1;
            for (int i = 1; i < layers; i++)
            {
                if (olsModeIdc == 4 || olsModeIdc < 2)
                    LayerUsedAsOutputLayerFlag[i] = 1;
                else if (olsModeIdc == 2)
                    LayerUsedAsOutputLayerFlag[i] = 0;
            }
            for (int i = 1; i < total; i++)
            {
                if (olsModeIdc == 4 || olsModeIdc == 0)
                {
                    NumOutputLayersInOls[i] = 1;
                    OutputLayerIdInOls[i][0] = vps_layer_id[i];
                    if (vps_each_layer_is_an_ols_flag != 0)
                        NumSubLayersInLayerInOLS[i][0] = SubLayers(i);
                    else
                    {
                        NumSubLayersInLayerInOLS[i][i] = SubLayers(i);
                        for (int k = i - 1; k >= 0; k--)
                        {
                            NumSubLayersInLayerInOLS[i][k] = 0;
                            for (int m = k + 1; m <= i; m++)
                            {
                                uint maxSublayerNeeded = Math.Min(NumSubLayersInLayerInOLS[i][m], MaxTidIlRef(m, k));
                                if (Direct(m, k) != 0 && NumSubLayersInLayerInOLS[i][k] < maxSublayerNeeded)
                                    NumSubLayersInLayerInOLS[i][k] = maxSublayerNeeded;
                            }
                        }
                    }
                }
                else if (olsModeIdc == 1)
                {
                    NumOutputLayersInOls[i] = (uint)(i + 1);
                    for (int j = 0; j < NumOutputLayersInOls[i]; j++)
                    {
                        OutputLayerIdInOls[i][j] = vps_layer_id[j];
                        NumSubLayersInLayerInOLS[i][j] = SubLayers(i);
                    }
                }
                else if (olsModeIdc == 2)
                {
                    int highestIncludedLayer = 0;
                    int j = 0;
                    for (int k = 0; k < layers; k++)
                    {
                        if (OutputFlag(i, k) != 0)
                        {
                            layerIncludedInOlsFlag[i][k] = 1;
                            highestIncludedLayer = k;
                            LayerUsedAsOutputLayerFlag[k] = 1;
                            OutputLayerIdx[i][j] = k;
                            OutputLayerIdInOls[i][j++] = vps_layer_id[k];
                            NumSubLayersInLayerInOLS[i][k] = SubLayers(i);
                        }
                    }
                    NumOutputLayersInOls[i] = (uint)j;
                    for (j = 0; j < NumOutputLayersInOls[i]; j++)
                    {
                        int idx = OutputLayerIdx[i][j];
                        for (int k = 0; k < NumRefLayers[idx]; k++)
                            layerIncludedInOlsFlag[i][ReferenceLayerIdx[idx][k]] = 1;
                    }
                    for (int k = highestIncludedLayer - 1; k >= 0; k--)
                        if (layerIncludedInOlsFlag[i][k] != 0 && OutputFlag(i, k) == 0)
                            for (int m = k + 1; m <= highestIncludedLayer; m++)
                            {
                                uint maxSublayerNeeded = Math.Min(NumSubLayersInLayerInOLS[i][m], MaxTidIlRef(m, k));
                                if (Direct(m, k) != 0 && layerIncludedInOlsFlag[i][m] != 0 &&
                                   NumSubLayersInLayerInOLS[i][k] < maxSublayerNeeded)
                                    NumSubLayersInLayerInOLS[i][k] = maxSublayerNeeded;
                            }
                }
            }

            NumLayersInOls[0] = 1;
            LayerIdInOls[0][0] = vps_layer_id[0];
            NumMultiLayerOlss = 0;
            for (int i = 1; i < total; i++)
            {
                if (vps_each_layer_is_an_ols_flag != 0)
                {
                    NumLayersInOls[i] = 1;
                    LayerIdInOls[i][0] = vps_layer_id[i];
                }
                else if (olsModeIdc == 0 || olsModeIdc == 1)
                {
                    NumLayersInOls[i] = (uint)(i + 1);
                    for (int j = 0; j < NumLayersInOls[i]; j++)
                        LayerIdInOls[i][j] = vps_layer_id[j];
                }
                else if (olsModeIdc == 2)
                {
                    int j = 0;
                    for (int k = 0; k < layers; k++)
                        if (layerIncludedInOlsFlag[i][k] != 0)
                            LayerIdInOls[i][j++] = vps_layer_id[k];
                    NumLayersInOls[i] = (uint)j;
                }
                if (NumLayersInOls[i] > 1)
                {
                    MultiLayerOlsIdx[i] = NumMultiLayerOlss;
                    NumMultiLayerOlss++;
                }
            }

            if (Strict)
                CheckOls(vps, total);
            return NumMultiLayerOlss;
        }

        public void OnSpsLog2CtuSizeMinus5()
        {
            var sps_log2_ctu_size_minus5 = SeqParameterSetRbsp.SpsLog2CtuSizeMinus5;
            CtbLog2SizeY = sps_log2_ctu_size_minus5 + 5;
            CtbSizeY = (uint)(1 << (int)CtbLog2SizeY);
        }

        public void OnSpsSixMinusMaxNumMergeCand()
        {
            var sps_six_minus_max_num_merge_cand = SeqParameterSetRbsp.SpsSixMinusMaxNumMergeCand;
            MaxNumMergeCand = 6 - sps_six_minus_max_num_merge_cand;
        }

        public void OnSpsExtraPhBitPresentFlag()
        {
            var sps_num_extra_ph_bytes = SeqParameterSetRbsp.SpsNumExtraPhBytes;
            var sps_extra_ph_bit_present_flag = SeqParameterSetRbsp.SpsExtraPhBitPresentFlag;
            NumExtraPhBits = 0;
            for (int i = 0; i < (sps_num_extra_ph_bytes * 8); i++)
                if (sps_extra_ph_bit_present_flag[i] != 0)
                    NumExtraPhBits++;
        }

        /// <summary>
        /// fixed_pic_rate_within_cvs_flag is coded only when fixed_pic_rate_general_flag is 0, and
        /// is 1 otherwise (7.4.6.2): left 0, low_delay_hrd_flag was read where
        /// elemental_duration_in_tc_minus1 is.
        /// </summary>
        public void OnFixedPicRateGeneralFlag(OlsTimingHrdParameters hrd, uint i)
        {
            if (hrd.FixedPicRateGeneralFlag[i] != 0)
                hrd.FixedPicRateWithinCvsFlag[i] = 1;
        }

        public void OnAlfChromaFilterSignalFlag()
        {
            NumAlfFilters = 25;
        }

        public void OnLmcsDeltaMaxBinIdx()
        {
            var lmcs_delta_max_bin_idx = AdaptationParameterSetRbsp.LmcsData.LmcsDeltaMaxBinIdx;
            LmcsMaxBinIdx = 15 - lmcs_delta_max_bin_idx;
        }

        public void OnSpsLog2MinLumaCodingBlockSizeMinus2()
        {
            var sps_log2_min_luma_coding_block_size_minus2 = SeqParameterSetRbsp.SpsLog2MinLumaCodingBlockSizeMinus2;
            var sps_chroma_format_idc = SeqParameterSetRbsp.SpsChromaFormatIdc;

            SubWidthC = sps_chroma_format_idc == 1 || sps_chroma_format_idc == 2 ? 2u : 1u;
            SubHeightC = sps_chroma_format_idc == 1 ? 2u : 1u;

            MinCbLog2SizeY = sps_log2_min_luma_coding_block_size_minus2 + 2; // 43
            MinCbSizeY = (uint)(1 << (int)MinCbLog2SizeY); // 44
            IbcBufWidthY = 256 * 128 / CtbSizeY; // 45
            IbcBufWidthC = IbcBufWidthY / SubWidthC; // 46
            VSize = (uint)Math.Min(64, CtbSizeY); // 47

            if (sps_chroma_format_idc == 0)
            {
                CtbWidthC = 0;
                CtbHeightC = 0;
            }
            else
            {
                CtbWidthC = CtbSizeY / SubWidthC;
                CtbHeightC = CtbSizeY / SubHeightC;
            }
        }

        public void OnPpsPicHeightInLumaSamples()
        {
            var pps_pic_width_in_luma_samples = PicParameterSetRbsp.PpsPicWidthInLumaSamples;
            var pps_pic_height_in_luma_samples = PicParameterSetRbsp.PpsPicHeightInLumaSamples;

            PicWidthInCtbsY = (uint)Math.Ceiling(pps_pic_width_in_luma_samples / (double)CtbSizeY); // 64
            PicHeightInCtbsY = (uint)Math.Ceiling(pps_pic_height_in_luma_samples / (double)CtbSizeY); // 65
            PicSizeInCtbsY = PicWidthInCtbsY * PicHeightInCtbsY; // 66
            PicWidthInMinCbsY = pps_pic_width_in_luma_samples / MinCbSizeY; // 67
            PicHeightInMinCbsY = pps_pic_height_in_luma_samples / MinCbSizeY; // 68
            PicSizeInMinCbsY = PicWidthInMinCbsY * PicHeightInMinCbsY; // 69
            PicSizeInSamplesY = pps_pic_width_in_luma_samples * pps_pic_height_in_luma_samples; // 70
            PicWidthInSamplesC = pps_pic_width_in_luma_samples / SubWidthC; // 71
            PicHeightInSamplesC = pps_pic_height_in_luma_samples / SubHeightC; // 72
        }

        /// <summary>
        /// Called as each pps_tile_row_height_minus1 is read; the tiles (6.5.1) are worked out once
        /// the last is in, since NumTilesInPic conditions what follows.
        /// </summary>
        /// <remarks>
        /// This worked everything out at every row - the slices too, from the parts of the PPS not
        /// read yet - and sized the column and row lists by the ones coded, so the uniform ones that
        /// follow them ran past the end.
        /// </remarks>
        public void OnPpsTileRowHeightMinus1(uint i)
        {
            if (i == PicParameterSetRbsp.PpsNumExpTileRowsMinus1)
            {
                DeriveTiles();

                // With a single tile pps_rect_slice_flag is not coded, and is 1 (7.4.3.5).
                if (NumTilesInPic <= 1)
                    PicParameterSetRbsp.PpsRectSliceFlag = 1;
            }
        }

        /// <summary>
        /// Called as the PPS reads pps_cabac_init_present_flag, the first element after its
        /// partitioning: a picture not partitioned is a single tile, and with every slice read the
        /// slices can be laid out.
        /// </summary>
        public void OnPpsCabacInitPresentFlag()
        {
            if (PicParameterSetRbsp.PpsNoPicPartitionFlag != 0)
            {
                // Not partitioned: one tile, and one slice to each subpicture (7.4.3.5).
                DeriveTiles();
                PicParameterSetRbsp.PpsRectSliceFlag = 1;
                PicParameterSetRbsp.PpsSingleSlicePerSubpicFlag = 1;
            }
            DeriveSlices();
        }

        /// <summary>The tile columns and rows (6.5.1, 14 to 20).</summary>
        private void DeriveTiles()
        {
            var pps = PicParameterSetRbsp;
            bool partitioned = pps.PpsNoPicPartitionFlag == 0;

            // (14), (15): the sizes coded, then as many of the last as fit, then what is left.
            // A picture not partitioned is one column and one row.
            ulong[] Split(ulong total, ulong[] codedMinus1, ulong lastCoded, out int count)
            {
                var sizes = new ulong[total + 1];
                ulong remaining = total;
                count = 0;
                if (partitioned)
                {
                    for (ulong k = 0; k <= lastCoded; k++)
                    {
                        sizes[count] = codedMinus1[k] + 1;
                        remaining -= Math.Min(remaining, sizes[count]);
                        count++;
                    }
                    ulong uniform = codedMinus1[lastCoded] + 1;
                    while (remaining >= uniform)
                    {
                        sizes[count++] = uniform;
                        remaining -= uniform;
                    }
                }
                if (remaining > 0)
                    sizes[count++] = remaining;
                return sizes;
            }

            ColWidthVal = Split(PicWidthInCtbsY, pps.PpsTileColumnWidthMinus1, pps.PpsNumExpTileColumnsMinus1, out int columns);
            RowHeightVal = Split(PicHeightInCtbsY, pps.PpsTileRowHeightMinus1, pps.PpsNumExpTileRowsMinus1, out int rows);
            NumTileColumns = columns;
            NumTileRows = rows;
            NumTilesInPic = NumTileColumns * NumTileRows;

            // (16), (17)
            TileColBdVal = new ulong[NumTileColumns + 1];
            for (int i = 0; i < NumTileColumns; i++)
                TileColBdVal[i + 1] = TileColBdVal[i] + ColWidthVal[i];
            TileRowBdVal = new ulong[NumTileRows + 1];
            for (int j = 0; j < NumTileRows; j++)
                TileRowBdVal[j + 1] = TileRowBdVal[j] + RowHeightVal[j];

            // (18), (19)
            CtbToTileColBd = new ulong[PicWidthInCtbsY + 1];
            ctbToTileColIdx = new uint[PicWidthInCtbsY + 1];
            for (uint ctbAddrX = 0, tileX = 0; ctbAddrX <= PicWidthInCtbsY; ctbAddrX++)
            {
                if (tileX < NumTileColumns && ctbAddrX == TileColBdVal[tileX + 1])
                    tileX++;
                CtbToTileColBd[ctbAddrX] = TileColBdVal[tileX];
                ctbToTileColIdx[ctbAddrX] = tileX;
            }
            CtbToTileRowBd = new ulong[PicHeightInCtbsY + 1];
            ctbToTileRowIdx = new uint[PicHeightInCtbsY + 1];
            for (uint ctbAddrY = 0, tileY = 0; ctbAddrY <= PicHeightInCtbsY; ctbAddrY++)
            {
                if (tileY < NumTileRows && ctbAddrY == TileRowBdVal[tileY + 1])
                    tileY++;
                CtbToTileRowBd[ctbAddrY] = TileRowBdVal[tileY];
                ctbToTileRowIdx[ctbAddrY] = tileY;
            }

            // (20)
            int subpics = NumSubpics;
            SubpicWidthInTiles = new uint[subpics];
            SubpicHeightInTiles = new uint[subpics];
            subpicHeightLessThanOneTileFlag = new uint[subpics];
            for (int i = 0; i < subpics; i++)
            {
                ulong leftX = SubpicLeft(i);
                ulong rightX = Math.Min(leftX + SubpicWidthMinus1(i), PicWidthInCtbsY - 1);
                SubpicWidthInTiles[i] = ctbToTileColIdx[rightX] + 1 - ctbToTileColIdx[leftX];
                ulong topY = SubpicTop(i);
                ulong bottomY = Math.Min(topY + SubpicHeightMinus1(i), PicHeightInCtbsY - 1);
                SubpicHeightInTiles[i] = ctbToTileRowIdx[bottomY] + 1 - ctbToTileRowIdx[topY];
                subpicHeightLessThanOneTileFlag[i] =
                    SubpicHeightInTiles[i] == 1 && SubpicHeightMinus1(i) + 1 < RowHeightVal[ctbToTileRowIdx[topY]] ? 1u : 0u;
            }
        }

        // The subpictures: without sps_subpic_info_present_flag, one covering the picture.
        private int NumSubpics => SeqParameterSetRbsp.SpsSubpicInfoPresentFlag != 0 ? (int)SeqParameterSetRbsp.SpsNumSubpicsMinus1 + 1 : 1;

        // The position and size of subpicture i, as coded or inferred (7.4.3.4): what is not coded
        // reaches to the right and bottom of the picture, or with sps_subpic_same_size_flag repeats
        // subpicture 0 in a grid. They were taken as 0 where not coded, so the last subpicture was
        // one CTU in size and held none of the slices below its first row.
        private ulong SubpicLeft(int i) => SubpicPosition(i, true);
        private ulong SubpicTop(int i) => SubpicPosition(i, false);
        private ulong SubpicWidthMinus1(int i) => SubpicSize(i, true);
        private ulong SubpicHeightMinus1(int i) => SubpicSize(i, false);

        private ulong SubpicPosition(int i, bool x)
        {
            var sps = SeqParameterSetRbsp;
            if (sps.SpsSubpicInfoPresentFlag == 0)
                return 0;
            bool sameSize = sps.SpsNumSubpicsMinus1 > 0 && sps.SpsSubpicSameSizeFlag != 0;
            var coded = x ? sps.SpsSubpicCtuTopLeftx : sps.SpsSubpicCtuTopLefty;
            if ((!sameSize || i == 0) && i > 0 && SpsPicSize(x) > CtbSizeY && coded != null && i < coded.Length)
                return coded[i];
            if (!sameSize || i == 0)
                return 0;
            ulong numSubpicCols = SpsPicSizeInCtbs(true) / (SubpicSize(0, true) + 1);
            return x ? (ulong)i % numSubpicCols * (SubpicSize(0, true) + 1) : (ulong)i / numSubpicCols * (SubpicSize(0, false) + 1);
        }

        private ulong SubpicSize(int i, bool x)
        {
            var sps = SeqParameterSetRbsp;
            if (sps.SpsSubpicInfoPresentFlag == 0)
                return (x ? PicWidthInCtbsY : PicHeightInCtbsY) - 1;
            bool sameSize = sps.SpsNumSubpicsMinus1 > 0 && sps.SpsSubpicSameSizeFlag != 0;
            var coded = x ? sps.SpsSubpicWidthMinus1 : sps.SpsSubpicHeightMinus1;
            if ((!sameSize || i == 0) && i < (int)sps.SpsNumSubpicsMinus1 && SpsPicSize(x) > CtbSizeY && coded != null && i < coded.Length)
                return coded[i];
            if (!sameSize || i == 0)
                return SpsPicSizeInCtbs(x) - SubpicPosition(i, x) - 1;
            return SubpicSize(0, x);
        }

        private ulong SpsPicSize(bool x) =>
            x ? SeqParameterSetRbsp.SpsPicWidthMaxInLumaSamples : SeqParameterSetRbsp.SpsPicHeightMaxInLumaSamples;

        // tmpWidthVal and tmpHeightVal (7.4.3.4).
        private ulong SpsPicSizeInCtbs(bool x) => (SpsPicSize(x) + CtbSizeY - 1) / CtbSizeY;

        /// <summary>
        /// The rectangular slices as far as the PPS has coded them, for the slice loop that reads
        /// them: which tile each starts at is worked out from the slices before it (6.5.1, 21).
        /// It was taken from the layout worked out at the tile rows, before any slice was read.
        /// </summary>
        public H266Context DeriveRectSlices(PicParameterSetRbsp pps)
        {
            LayOutRectSlices(pps, addCtbs: false);
            return this;
        }

        /// <summary>
        /// Whether rectangular slice i is one of several in a tile, which the PPS then codes the
        /// heights of - with slice i's size inferred first where the PPS left it out (7.4.3.5): a
        /// width of one tile, and the height of the slice before it unless in the last row. Left 0,
        /// a slice inheriting a taller height was read as sitting inside one tile.
        /// </summary>
        public bool OnPpsSliceSize(PicParameterSetRbsp pps, uint i)
        {
            LayOutRectSlices(pps, addCtbs: false);

            uint topLeft = SliceTopLeftTileIdx[i];
            int tileX = (int)(topLeft % (uint)NumTileColumns);
            int tileY = (int)(topLeft / (uint)NumTileColumns);
            bool widthCoded = tileX != NumTileColumns - 1;
            bool heightCoded = tileY != NumTileRows - 1 && (pps.PpsTileIdxDeltaPresentFlag != 0 || tileX == 0);

            if (!widthCoded)
                pps.PpsSliceWidthInTilesMinus1[i] = 0;
            if (!heightCoded)
                pps.PpsSliceHeightInTilesMinus1[i] = tileY == NumTileRows - 1 || i == 0 ? 0 : pps.PpsSliceHeightInTilesMinus1[i - 1];

            return pps.PpsSliceWidthInTilesMinus1[i] == 0 && pps.PpsSliceHeightInTilesMinus1[i] == 0 && RowHeightVal[tileY] > 1;
        }

        /// <summary>The slices of the picture and the CTUs in each (6.5.1, 21 to 23).</summary>
        private void DeriveSlices()
        {
            var pps = PicParameterSetRbsp;
            if (pps.PpsRectSliceFlag == 0)
                return; // each slice header says where its slice is

            if (pps.PpsSingleSlicePerSubpicFlag != 0)
            {
                // pps_num_slices_in_pic_minus1 is inferred to be sps_num_subpics_minus1.
                int subpics = NumSubpics;
                NumCtusInSlice = new uint[subpics];
                CtbAddrInSlice = new ulong[subpics][];

                if (SeqParameterSetRbsp.SpsSubpicInfoPresentFlag == 0)
                {
                    for (int j = 0; j < NumTileRows; j++)
                        for (int i = 0; i < NumTileColumns; i++)
                            AddCtbsToSlice(0, TileColBdVal[i], TileColBdVal[i + 1], TileRowBdVal[j], TileRowBdVal[j + 1]);
                }
                else
                {
                    for (int i = 0; i < subpics; i++)
                    {
                        if (subpicHeightLessThanOneTileFlag[i] != 0)
                        {
                            AddCtbsToSlice(i, SubpicLeft(i), SubpicLeft(i) + SubpicWidthMinus1(i) + 1,
                                SubpicTop(i), SubpicTop(i) + SubpicHeightMinus1(i) + 1);
                        }
                        else
                        {
                            uint tileX = ctbToTileColIdx[SubpicLeft(i)];
                            uint tileY = ctbToTileRowIdx[SubpicTop(i)];
                            for (uint j = 0; j < SubpicHeightInTiles[i]; j++)
                                for (uint k = 0; k < SubpicWidthInTiles[i]; k++)
                                    AddCtbsToSlice(i, TileColBdVal[tileX + k], TileColBdVal[tileX + k + 1],
                                        TileRowBdVal[tileY + j], TileRowBdVal[tileY + j + 1]);
                        }
                    }
                }
            }
            else
            {
                LayOutRectSlices(pps, addCtbs: true);
            }

            // (23)
            int slices = NumCtusInSlice.Length;
            NumSlicesInSubpic = new uint[NumSubpics];
            SubpicIdxForSlice = new int[slices];
            SubpicLevelSliceIdx = new uint[slices];
            for (int i = 0; i < NumSubpics; i++)
            {
                for (int j = 0; j < slices; j++)
                {
                    if (NumCtusInSlice[j] == 0)
                        continue;
                    ulong posX = CtbAddrInSlice[j][0] % PicWidthInCtbsY;
                    ulong posY = CtbAddrInSlice[j][0] / PicWidthInCtbsY;
                    if (posX >= SubpicLeft(i) && posX < SubpicLeft(i) + SubpicWidthMinus1(i) + 1 &&
                        posY >= SubpicTop(i) && posY < SubpicTop(i) + SubpicHeightMinus1(i) + 1)
                    {
                        SubpicIdxForSlice[j] = i;
                        SubpicLevelSliceIdx[j] = NumSlicesInSubpic[i];
                        NumSlicesInSubpic[i]++;
                    }
                }
            }
        }

        /// <summary>
        /// (21) for rectangular slices the PPS codes one by one, over what it has read so far: the
        /// slices not read yet read as zeros, which leaves the ones before them as they are.
        /// </summary>
        private void LayOutRectSlices(PicParameterSetRbsp pps, bool addCtbs)
        {
            int slices = (int)pps.PpsNumSlicesInPicMinus1 + 1;
            SliceTopLeftTileIdx = new uint[slices];
            sliceWidthInTiles = new ulong[slices];
            sliceHeightInTiles = new ulong[slices];
            NumSlicesInTile = new uint[slices];
            sliceHeightInCtus = new ulong[slices];
            NumCtusInSlice = new uint[slices];
            CtbAddrInSlice = new ulong[slices][];

            ulong Coded(ulong[] values, int i) => values != null && i < values.Length ? values[i] : 0;

            int tileIdx = 0;
            for (int i = 0; i < slices; i++)
            {
                if (tileIdx < 0 || tileIdx >= NumTilesInPic)
                    break; // past what has been read

                SliceTopLeftTileIdx[i] = (uint)tileIdx;
                int tileX = tileIdx % NumTileColumns;
                int tileY = tileIdx / NumTileColumns;
                if (i < slices - 1)
                {
                    sliceWidthInTiles[i] = Coded(pps.PpsSliceWidthInTilesMinus1, i) + 1;
                    sliceHeightInTiles[i] = Coded(pps.PpsSliceHeightInTilesMinus1, i) + 1;
                }
                else
                {
                    sliceWidthInTiles[i] = (ulong)(NumTileColumns - tileX);
                    sliceHeightInTiles[i] = (ulong)(NumTileRows - tileY);
                    NumSlicesInTile[i] = 1;
                }

                if (sliceWidthInTiles[i] == 1 && sliceHeightInTiles[i] == 1)
                {
                    ulong expSlices = Coded(pps.PpsNumExpSlicesInTile, i);
                    if (expSlices == 0)
                    {
                        NumSlicesInTile[i] = 1;
                        sliceHeightInCtus[i] = RowHeightVal[tileY];
                    }
                    else
                    {
                        ulong remainingHeightInCtbsY = RowHeightVal[tileY];
                        var heights = pps.PpsExpSliceHeightInCtusMinus1?[i];
                        int j;
                        for (j = 0; j < (int)expSlices && i + j < slices; j++)
                        {
                            sliceHeightInCtus[i + j] = (heights != null && j < heights.Length ? heights[j] : 0) + 1;
                            remainingHeightInCtbsY -= Math.Min(remainingHeightInCtbsY, sliceHeightInCtus[i + j]);
                        }
                        ulong uniformSliceHeight = sliceHeightInCtus[i + j - 1];
                        while (remainingHeightInCtbsY >= uniformSliceHeight && i + j < slices)
                        {
                            sliceHeightInCtus[i + j] = uniformSliceHeight;
                            remainingHeightInCtbsY -= uniformSliceHeight;
                            j++;
                        }
                        if (remainingHeightInCtbsY > 0 && i + j < slices)
                        {
                            sliceHeightInCtus[i + j] = remainingHeightInCtbsY;
                            j++;
                        }
                        NumSlicesInTile[i] = (uint)j;
                    }

                    ulong ctbY = TileRowBdVal[tileY];
                    for (int j = 0; j < NumSlicesInTile[i]; j++)
                    {
                        if (addCtbs)
                            AddCtbsToSlice(i + j, TileColBdVal[tileX], TileColBdVal[tileX + 1], ctbY, ctbY + sliceHeightInCtus[i + j]);
                        ctbY += sliceHeightInCtus[i + j];
                        sliceWidthInTiles[i + j] = 1;
                        sliceHeightInTiles[i + j] = 1;
                    }
                    i += (int)NumSlicesInTile[i] - 1;
                }
                else if (addCtbs)
                {
                    for (ulong j = 0; j < sliceHeightInTiles[i] && tileY + (int)j < NumTileRows; j++)
                        for (ulong k = 0; k < sliceWidthInTiles[i] && tileX + (int)k < NumTileColumns; k++)
                            AddCtbsToSlice(i, TileColBdVal[tileX + (int)k], TileColBdVal[tileX + (int)k + 1],
                                TileRowBdVal[tileY + (int)j], TileRowBdVal[tileY + (int)j + 1]);
                }

                if (i < slices - 1)
                {
                    if (pps.PpsTileIdxDeltaPresentFlag != 0)
                    {
                        tileIdx += (int)(pps.PpsTileIdxDeltaVal != null && i < pps.PpsTileIdxDeltaVal.Length ? pps.PpsTileIdxDeltaVal[i] : 0);
                    }
                    else
                    {
                        tileIdx += (int)sliceWidthInTiles[i];
                        if (tileIdx % NumTileColumns == 0)
                            tileIdx += ((int)sliceHeightInTiles[i] - 1) * NumTileColumns;
                    }
                }
            }
        }

        public void AddCtbsToSlice(int sliceIdx, ulong startX, ulong stopX, ulong startY, ulong stopY)
        {
            var addresses = CtbAddrInSlice[sliceIdx] ??= new ulong[16];
            for (ulong ctbY = startY; ctbY < stopY; ctbY++)
            {
                for (ulong ctbX = startX; ctbX < stopX; ctbX++)
                {
                    if (NumCtusInSlice[sliceIdx] == addresses.Length)
                        CtbAddrInSlice[sliceIdx] = addresses = Grown(addresses, addresses.Length * 2);
                    addresses[NumCtusInSlice[sliceIdx]] = ctbY * PicWidthInCtbsY + ctbX;
                    NumCtusInSlice[sliceIdx]++;
                }
            }
        }

        private static T[] Grown<T>(T[] array, int size)
        {
            var grown = new T[size];
            Array.Copy(array, grown, array.Length);
            return grown;
        }

        /// <summary>
        /// Called as a slice header starts, read or written: sh_subpic_id, when not coded, is of the
        /// only subpicture.
        /// </summary>
        public void OnShPictureHeaderInSliceHeaderFlag(SliceHeader header)
        {
            CurrSubpicIdx = 0;
        }

        /// <summary>
        /// What a slice header leaves out and the spec infers, set as it starts to be read so that
        /// what is read overwrites it: sh_slice_type is I when the picture allows no inter slices
        /// (7.4.8), sh_num_ref_idx_active_override_flag is 1, and sh_collocated_from_l0_flag,
        /// coded for B slices only, is 1. Left 0 - B, and no override - intra slices read the
        /// reference counts of a B slice, and P slices looked for their collocated picture in the
        /// wrong list. Only as it is read: written, it would write over the values coded.
        /// </summary>
        public void InferSliceHeader(SliceHeader header)
        {
            header.ShSliceType = H266FrameTypes.I;
            header.ShNumRefIdxActiveOverrideFlag = 1;
            header.ShCollocatedFromL0Flag = 1;
        }

        /// <summary>
        /// The picture header of the picture being read, whether it came in a picture header NAL
        /// unit or in the slice header. The slice header's references to it went through the slice
        /// header's own, which is null when the picture header came on its own - so every slice of
        /// such a picture threw.
        /// </summary>
        public PictureHeaderStructure PictureHeader { get; set; }

        /// <summary>
        /// Called as a picture header starts, read or written: it becomes the one in force.
        /// </summary>
        public void OnPhGdrOrIrapPicFlag(PictureHeaderStructure header)
        {
            PictureHeader = header;
        }

        /// <summary>
        /// What a picture header leaves out and the spec infers, set as it starts to be read:
        /// ph_collocated_from_l0_flag, coded only when list 1 has entries, is 1 otherwise, and
        /// ph_intra_slice_allowed_flag, coded only when inter slices are allowed, is 1 otherwise
        /// (7.4.3.8) - left 0, an intra-only picture's intra slice parameters went unread. Only as it
        /// is read: written, it would write over the values coded.
        /// </summary>
        public void InferPictureHeader(PictureHeaderStructure header)
        {
            header.PhCollocatedFromL0Flag = 1;
            header.PhIntraSliceAllowedFlag = 1;
        }

        /// <summary>
        /// Called as the SPS reads sps_ref_wraparound_enabled_flag, the element after its reference
        /// picture list structures. With sps_rpl1_same_as_rpl0_flag only list 0's are coded, and
        /// list 1's are inferred to be the same (7.4.3.4): without that, list 1 had no count, and
        /// every slice predicting from it read past the end of one.
        /// </summary>
        public void OnSpsRefWraparoundEnabledFlag()
        {
            var sps = SeqParameterSetRbsp;
            if (sps.SpsRpl1SameAsRpl0Flag == 0)
                return;

            ulong lists = sps.SpsNumRefPicLists[0];
            sps.SpsNumRefPicLists = new[] { lists, lists };

            // The structures themselves are shared - read once, never changed - but list 1 keeps
            // its own slot past them, for the one a picture or slice header codes.
            T[] Copy<T>(T[] list0)
            {
                var list1 = new T[lists + 1];
                if (list0 != null)
                    Array.Copy(list0, list1, (int)Math.Min(lists, (ulong)list0.Length));
                return list1;
            }

            // the tables are there once the SPS is read; one with no structures, written, has none yet
            num_ref_entries ??= new ulong[2][];
            inter_layer_ref_pic_flag ??= new byte[2][][];
            st_ref_pic_flag ??= new byte[2][][];
            abs_delta_poc_st ??= new ulong[2][][];
            strp_entry_sign_flag ??= new byte[2][][];
            rpls_poc_lsb_lt ??= new ulong[2][][];
            ilrp_idx ??= new ulong[2][][];
            ltrp_in_header_flag ??= new byte[2][];

            num_ref_entries[1] = Copy(num_ref_entries[0]);
            inter_layer_ref_pic_flag[1] = Copy(inter_layer_ref_pic_flag[0]);
            st_ref_pic_flag[1] = Copy(st_ref_pic_flag[0]);
            abs_delta_poc_st[1] = Copy(abs_delta_poc_st[0]);
            strp_entry_sign_flag[1] = Copy(strp_entry_sign_flag[0]);
            rpls_poc_lsb_lt[1] = Copy(rpls_poc_lsb_lt[0]);
            ilrp_idx[1] = Copy(ilrp_idx[0]);
            ltrp_in_header_flag[1] = Copy(ltrp_in_header_flag[0]);
        }

        /// <summary>
        /// rpl_sps_flag[ i ], as ref_pic_lists() tests it: coded or inferred (7.4.9). It is coded
        /// only when the SPS has structures to pick from, and for list 1 only with
        /// pps_rpl1_idx_present_flag; otherwise list 1 does as list 0 did. rpl_idx is inferred
        /// alongside it, and RplsIdx worked out - both used to be only when rpl_idx was coded, so
        /// a list taken from the SPS without an index, or coded in the header, was looked up at
        /// whatever index the previous slice left.
        /// </summary>
        public byte InferRplSpsFlag(RefPicLists lists, uint i)
        {
            var sps_num_ref_pic_lists = SeqParameterSetRbsp.SpsNumRefPicLists;
            var pps_rpl1_idx_present_flag = PicParameterSetRbsp.PpsRpl1IdxPresentFlag;
            bool rpl1FromRpl0 = i == 1 && pps_rpl1_idx_present_flag == 0;

            bool coded = sps_num_ref_pic_lists[i] > 0 && !rpl1FromRpl0;
            if (!coded)
                lists.RplSpsFlag[i] = sps_num_ref_pic_lists[i] == 0 ? (byte)0 : (rpl1FromRpl0 ? lists.RplSpsFlag[0] : (byte)0);

            if (lists.RplSpsFlag[i] != 0)
            {
                // Inferred only where it is not coded: written, the coded one was written over before it was written.
                // Where it is coded, a read puts it in after this, by OnRplIdx.
                bool idxCoded = sps_num_ref_pic_lists[i] > 1 && !rpl1FromRpl0;
                if (!idxCoded)
                    lists.RplIdx[i] = rpl1FromRpl0 && sps_num_ref_pic_lists[1] > 1 ? lists.RplIdx[0] : 0;
                RplsIdx[i] = lists.RplIdx[i];
            }
            else
            {
                RplsIdx[i] = sps_num_ref_pic_lists[i];
            }

            return lists.RplSpsFlag[i];
        }

        /// <summary>
        /// NumRefIdxActive (7.4.8), worked out where it is used. It was worked out only as
        /// sh_num_ref_idx_active_minus1 was read, which it is not when the override is off or a
        /// list has a single entry, so the collocated picture's index was read, or not, by the
        /// counts of an earlier slice.
        /// </summary>
        public ulong[] DeriveNumRefIdxActive()
        {
            var header = SliceLayerRbsp.SliceHeader;
            var pps_num_ref_idx_default_active_minus1 = PicParameterSetRbsp.PpsNumRefIdxDefaultActiveMinus1;

            if (NumRefIdxActive == null || NumRefIdxActive.Length < 2)
                NumRefIdxActive = new ulong[2];

            for (int i = 0; i < 2; i++)
            {
                if (header.ShSliceType == H266FrameTypes.B || (header.ShSliceType == H266FrameTypes.P && i == 0))
                {
                    if (header.ShNumRefIdxActiveOverrideFlag != 0)
                    {
                        // sh_num_ref_idx_active_minus1 is 0 where not coded.
                        var minus1 = header.ShNumRefIdxActiveMinus1;
                        NumRefIdxActive[i] = (minus1 != null && i < minus1.Length ? minus1[i] : 0) + 1;
                    }
                    else
                    {
                        ulong entries = num_ref_entries[i][RplsIdx[i]];
                        NumRefIdxActive[i] = Math.Min(entries, pps_num_ref_idx_default_active_minus1[i] + 1);
                    }
                }
                else
                {
                    NumRefIdxActive[i] = 0;
                }
            }

            return NumRefIdxActive;
        }

        public void OnRplIdx(RefPicLists refPicLists, uint i)
        {
            var rpl_sps_flag = refPicLists.RplSpsFlag;
            var rpl_idx = refPicLists.RplIdx;
            var sps_num_ref_pic_lists = SeqParameterSetRbsp.SpsNumRefPicLists;

            RplsIdx[i] = rpl_sps_flag[i] != 0 ? rpl_idx[i] : sps_num_ref_pic_lists[i];
        }

        /// <summary>NumWeightsL0 (148), worked out where pred_weight_table() loops on it.</summary>
        public ulong DeriveNumWeightsL0(PredWeightTable table)
        {
            NumWeightsL0 = PicParameterSetRbsp.PpsWpInfoInPhFlag != 0 ? table.NumL0Weights : DeriveNumRefIdxActive()[0];
            return NumWeightsL0;
        }

        public void OnShNumRefIdxActiveMinus1()
        {
            DeriveNumRefIdxActive();
        }

        public void OnPpsSubpicId()
        {
            var sps_num_subpics_minus1 = SeqParameterSetRbsp.SpsNumSubpicsMinus1;
            var sps_subpic_id_mapping_explicitly_signalled_flag = SeqParameterSetRbsp.SpsSubpicIdMappingExplicitlySignalledFlag;
            var pps_subpic_id_mapping_present_flag = PicParameterSetRbsp.PpsSubpicIdMappingPresentFlag;
            var pps_subpic_id = PicParameterSetRbsp.PpsSubpicId;
            var sps_subpic_id = SeqParameterSetRbsp.SpsSubpicId;

            if (SubpicIdVal == null || SubpicIdVal.Length < (int)sps_num_subpics_minus1 + 1)
                SubpicIdVal = new ulong[sps_num_subpics_minus1 + 1];

            for (uint i = 0; i <= sps_num_subpics_minus1; i++)
                if (sps_subpic_id_mapping_explicitly_signalled_flag != 0)
                    SubpicIdVal[i] = pps_subpic_id_mapping_present_flag != 0 ? pps_subpic_id[i] : sps_subpic_id[i];
                else
                    SubpicIdVal[i] = i;
        }

        public void OnShSubpicId(ulong sh_subpic_id)
        {
            CurrSubpicIdx = Array.IndexOf(SubpicIdVal, sh_subpic_id);
        }

        public void OnSpsExtraShBitPresentFlag()
        {
            var sps_num_extra_sh_bytes = SeqParameterSetRbsp.SpsNumExtraShBytes;
            var sps_extra_sh_bit_present_flag = SeqParameterSetRbsp.SpsExtraShBitPresentFlag;

            NumExtraShBits = 0;
            for (int i = 0; i < (sps_num_extra_sh_bytes * 8); i++)
                if (sps_extra_sh_bit_present_flag[i] != 0)
                    NumExtraShBits++;
        }

        /// <summary>
        /// NumEntryPoints (7.4.8), worked out where it is tested from the CTUs of the current slice.
        /// Both were worked out only as optional elements were read - the slice header extension,
        /// sh_num_tiles_in_slice_minus1 - so most slices read the entry points of another.
        /// </summary>
        public int DeriveNumEntryPoints()
        {
            DeriveCurrSlice();

            NumEntryPoints = 0;
            if (SeqParameterSetRbsp.SpsEntryPointOffsetsPresentFlag != 0)
            {
                for (int i = 1; i < NumCtusInCurrSlice; i++)
                {
                    ulong ctbAddrX = CtbAddrInCurrSlice[i] % PicWidthInCtbsY;
                    ulong ctbAddrY = CtbAddrInCurrSlice[i] / PicWidthInCtbsY;
                    ulong prevCtbAddrX = CtbAddrInCurrSlice[i - 1] % PicWidthInCtbsY;
                    ulong prevCtbAddrY = CtbAddrInCurrSlice[i - 1] / PicWidthInCtbsY;
                    if (CtbToTileRowBd[ctbAddrY] != CtbToTileRowBd[prevCtbAddrY] ||
                        CtbToTileColBd[ctbAddrX] != CtbToTileColBd[prevCtbAddrX] ||
                        (ctbAddrY != prevCtbAddrY && SeqParameterSetRbsp.SpsEntropyCodingSyncEnabledFlag != 0))
                        NumEntryPoints++;
                }
            }

            return NumEntryPoints;
        }

        /// <summary>The CTUs of the current slice (7.4.8).</summary>
        private void DeriveCurrSlice()
        {
            var header = SliceLayerRbsp.SliceHeader;

            if (PicParameterSetRbsp.PpsRectSliceFlag != 0)
            {
                ulong picLevelSliceIdx = header.ShSliceAddress;
                for (int j = 0; j < CurrSubpicIdx; j++)
                    picLevelSliceIdx += NumSlicesInSubpic[j];
                NumCtusInCurrSlice = NumCtusInSlice[picLevelSliceIdx];
                CtbAddrInCurrSlice = CtbAddrInSlice[picLevelSliceIdx] ?? [];
            }
            else
            {
                // sh_num_tiles_in_slice_minus1 is 0 where not coded.
                var addresses = new List<ulong>();
                for (int tileIdx = (int)header.ShSliceAddress; tileIdx <= (int)(header.ShSliceAddress + header.ShNumTilesInSliceMinus1) && tileIdx < NumTilesInPic; tileIdx++)
                {
                    int tileX = tileIdx % NumTileColumns;
                    int tileY = tileIdx / NumTileColumns;
                    for (ulong ctbY = TileRowBdVal[tileY]; ctbY < TileRowBdVal[tileY + 1]; ctbY++)
                        for (ulong ctbX = TileColBdVal[tileX]; ctbX < TileColBdVal[tileX + 1]; ctbX++)
                            addresses.Add(ctbY * PicWidthInCtbsY + ctbX);
                }
                CtbAddrInCurrSlice = addresses.ToArray();
                NumCtusInCurrSlice = (uint)addresses.Count;
            }
        }

        /// <summary>
        /// NumWeightsL1 (149), worked out where pred_weight_table() loops on it. It was worked out
        /// only as num_l1_weights was read, which it is only with the weights in the picture header,
        /// and from the header's own reference picture list, which a list taken from the SPS lacks.
        /// </summary>
        public ulong DeriveNumWeightsL1(PredWeightTable table)
        {
            var pps = PicParameterSetRbsp;
            if (pps.PpsWeightedBipredFlag == 0 || (pps.PpsWpInfoInPhFlag != 0 && num_ref_entries[1][RplsIdx[1]] == 0))
                NumWeightsL1 = 0;
            else if (pps.PpsWpInfoInPhFlag != 0)
                NumWeightsL1 = table.NumL1Weights;
            else
                NumWeightsL1 = DeriveNumRefIdxActive()[1];
            return NumWeightsL1;
        }

        public void OnAbsDeltaPocSt(uint listIdx, ulong rplsIdx, uint i, RefPicListStruct refPicListStruct)
        {
            var sps_weighted_pred_flag = SeqParameterSetRbsp.SpsWeightedPredFlag;
            var sps_weighted_bipred_flag = SeqParameterSetRbsp.SpsWeightedBipredFlag;
            var sps_num_ref_pic_lists = SeqParameterSetRbsp.SpsNumRefPicLists;
            var num_ref_entries = refPicListStruct.NumRefEntries;

            if (AbsDeltaPocSt == null || AbsDeltaPocSt.Length < sps_num_ref_pic_lists.Length)
            {
                AbsDeltaPocSt = new ulong[sps_num_ref_pic_lists.Length][][];
            }

            if (AbsDeltaPocSt[listIdx] == null)
                AbsDeltaPocSt[listIdx] = new ulong[sps_num_ref_pic_lists[listIdx] + 1][];

            if(AbsDeltaPocSt[listIdx][rplsIdx] == null || AbsDeltaPocSt[listIdx][rplsIdx].Length < (int)num_ref_entries[listIdx][rplsIdx])
                AbsDeltaPocSt[listIdx][rplsIdx] = new ulong[num_ref_entries[listIdx][rplsIdx]];

            // the structure's own, which a context that has only written has no table of
            var abs_delta_poc_st = refPicListStruct.AbsDeltaPocSt;
            if ((sps_weighted_pred_flag != 0 || sps_weighted_bipred_flag != 0) && i != 0)
                AbsDeltaPocSt[listIdx][rplsIdx][i] = abs_delta_poc_st[listIdx][rplsIdx][i];
            else
                AbsDeltaPocSt[listIdx][rplsIdx][i] = abs_delta_poc_st[listIdx][rplsIdx][i] + 1;
        }

        /// <summary>
        /// A reference picture list structure, as it is read or written: its entry goes into the tables the picture and
        /// slice headers look lists up in, at [ listIdx ][ rplsIdx ]. Called as its count and its ltrp_in_header_flag are
        /// known - the rest of it are arrays, which the tables then share.
        /// </summary>
        public void OnRefPicListStruct(RefPicListStruct rpl)
        {
            uint listIdx = rpl.ListIdx;
            ulong rplsIdx = rpl.RplsIdx;

            Ensure(ref num_ref_entries, listIdx, rplsIdx)[rplsIdx] = rpl.NumRefEntries[listIdx][rplsIdx];
            Ensure(ref ltrp_in_header_flag, listIdx, rplsIdx)[rplsIdx] = rpl.LtrpInHeaderFlag[listIdx][rplsIdx];
            Ensure(ref inter_layer_ref_pic_flag, listIdx, rplsIdx)[rplsIdx] = rpl.InterLayerRefPicFlag[listIdx][rplsIdx];
            Ensure(ref st_ref_pic_flag, listIdx, rplsIdx)[rplsIdx] = rpl.StRefPicFlag[listIdx][rplsIdx];
            Ensure(ref abs_delta_poc_st, listIdx, rplsIdx)[rplsIdx] = rpl.AbsDeltaPocSt[listIdx][rplsIdx];
            Ensure(ref strp_entry_sign_flag, listIdx, rplsIdx)[rplsIdx] = rpl.StrpEntrySignFlag[listIdx][rplsIdx];
            Ensure(ref rpls_poc_lsb_lt, listIdx, rplsIdx)[rplsIdx] = rpl.RplsPocLsbLt[listIdx][rplsIdx];
            Ensure(ref ilrp_idx, listIdx, rplsIdx)[rplsIdx] = rpl.IlrpIdx[listIdx][rplsIdx];
        }

        /// <summary>The list's row of a table, there and long enough for rplsIdx: the SPS sizes them as it is read, a context that has only written as they are needed.</summary>
        private static T[] Ensure<T>(ref T[][] table, uint listIdx, ulong rplsIdx)
        {
            table ??= new T[2][];
            T[] row = table[listIdx];
            if (row == null || (ulong)row.Length <= rplsIdx)
            {
                var grown = new T[rplsIdx + 1];
                if (row != null)
                    Array.Copy(row, grown, row.Length);
                table[listIdx] = row = grown;
            }
            return row;
        }
    
        /// <summary>
        /// A sequence parameter set registers itself under its id. One sent again under the same id
        /// replaces the one there; keeping the first read later pictures against the old one.
        /// </summary>
        public void SetSpsSeqParameterSetId(ulong sps_seq_parameter_set_id)
        {
            if (SeqParameterSetRbsp != null)
                SeqParameterSets[sps_seq_parameter_set_id] = SeqParameterSetRbsp;
        }

        /// <summary>A PPS reads the rest of itself against the SPS it names.</summary>
        public void SetPpsSeqParameterSetId(ulong pps_seq_parameter_set_id)
        {
            if (!SeqParameterSets.TryGetValue(pps_seq_parameter_set_id, out var sps))
            {
                // Registered as its id was read: a PPS read no further is not there to refer to.
                if (Strict)
                    PicParameterSets.Remove(PicParameterSetRbsp.PpsPicParameterSetId);
                throw new Exception($"SeqParameterSet with id {pps_seq_parameter_set_id} not found.");
            }

            SeqParameterSetRbsp = sps;
            DeriveFromSps();
        }

        /// <summary>A PPS registers itself under its id, replacing one sent before under it.</summary>
        public void SetPpsPicParameterSetId(ulong pps_pic_parameter_set_id)
        {
            if (PicParameterSetRbsp != null)
                PicParameterSets[pps_pic_parameter_set_id] = PicParameterSetRbsp;
        }

        /// <summary>
        /// A picture header activates the PPS it names, and that the SPS, and what the syntax takes
        /// from them is worked out again. It switched only when the id differed from the set last
        /// parsed, and kept what had been worked out from that one - the picture size in CTUs, the
        /// tiles and slices, the extra header bits - so a picture was read against another's.
        /// </summary>
        public void SetPhPicParameterSetId(ulong ph_pic_parameter_set_id)
        {
            if (!PicParameterSets.TryGetValue(ph_pic_parameter_set_id, out var pps))
                throw new Exception($"PicParameterSet with id {ph_pic_parameter_set_id} not found.");
            if (!SeqParameterSets.TryGetValue(pps.PpsSeqParameterSetId, out var sps))
                throw new Exception($"SeqParameterSet with id {pps.PpsSeqParameterSetId} not found.");

            PicParameterSetRbsp = pps;
            SeqParameterSetRbsp = sps;
            DeriveFromSps();
            OnPpsPicHeightInLumaSamples();
            if (pps.PpsNoPicPartitionFlag == 0 && pps.PpsTileRowHeightMinus1 != null)
                OnPpsTileRowHeightMinus1((uint)pps.PpsNumExpTileRowsMinus1);
            OnPpsCabacInitPresentFlag();
            if (sps.SpsSubpicIdMappingExplicitlySignalledFlag != 0 || sps.SpsSubpicInfoPresentFlag != 0)
                OnPpsSubpicId();
        }

        /// <summary>What the syntax takes from the SPS in force.</summary>
        private void DeriveFromSps()
        {
            OnSpsLog2CtuSizeMinus5();
            OnSpsLog2MinLumaCodingBlockSizeMinus2();
            OnSpsSixMinusMaxNumMergeCand();
            OnSpsExtraPhBitPresentFlag();
            OnSpsExtraShBitPresentFlag();
        }
    }
}
