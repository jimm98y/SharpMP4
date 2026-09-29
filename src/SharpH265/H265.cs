using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpH265
{
    public class H265Constants
    {
        public const uint EXTENDED_ISO = 255;
        public const uint EXTENDED_SAR = 255;
    }

    public class H265FrameTypes
    {
        public const ulong B = 0;
        public const ulong P = 1;
        public const ulong I = 2;

        public static bool IsB(ulong value) { return value == B; }
        public static bool IsP(ulong value) { return value == P; }
        public static bool IsI(ulong value) { return value == I; }
    }

    public class H265NALTypes
    {
        public const uint TRAIL_N = 0;                 // Coded slice segment of a non-TSA, non-STSA trailing picture
        public const uint TRAIL_R = 1;                 // Coded slice segment of a non-TSA, non-STSA trailing picture
        public const uint TSA_N = 2;                   // Coded slice segment of a TSA picture
        public const uint TSA_R = 3;                   // Coded slice segment of a TSA picture
        public const uint STSA_N = 4;                  // Coded slice segment of a STSA picture
        public const uint STSA_R = 5;                  // Coded slice segment of a STSA picture
        public const uint RADL_N = 6;                  // Coded slice segment of a RADL picture
        public const uint RADL_R = 7;                  // Coded slice segment of a RADL picture
        public const uint RASL_N = 8;                  // Coded slice segment of a RASL picture
        public const uint RASL_R = 9;                  // Coded slice segment of a RASL picture

        public const uint RSV_VCL_N10 = 10;            // Reserved non-IRAP SLNR VCL NAL unit types
        public const uint RSV_VCL_R11 = 11;            // Reserved non-IRAP sub-layer reference VCL NAL unit types
        public const uint RSV_VCL_N12 = 12;            // Reserved non-IRAP SLNR VCL NAL unit types
        public const uint RSV_VCL_R13 = 13;            // Reserved non-IRAP sub-layer reference VCL NAL unit types
        public const uint RSV_VCL_N14 = 14;            // Reserved non-IRAP SLNR VCL NAL unit types
        public const uint RSV_VCL_R15 = 15;            // Reserved non-IRAP sub-layer reference VCL NAL unit types

        public const uint BLA_W_LP = 16;               // Coded slice segment of a BLA picture
        public const uint BLA_W_RADL = 17;             // Coded slice segment of a BLA picture
        public const uint BLA_N_LP = 18;               // Coded slice segment of a BLA picture
        public const uint IDR_W_RADL = 19;             // Coded slice segment of an IDR picture
        public const uint IDR_N_LP = 20;               // Coded slice segment of an IDR picture
        public const uint CRA_NUT = 21;                // Coded slice segment of a CRA picture

        public const uint RSV_IRAP_VCL22 = 22;         // Reserved IRAP VCL NAL unit types
        public const uint RSV_IRAP_VCL23 = 23;         // Reserved IRAP VCL NAL unit types
        public const uint RSV_VCL24 = 24;              // Reserved VCL NAL unit types
        public const uint RSV_VCL25 = 25;              // Reserved VCL NAL unit types
        public const uint RSV_VCL26 = 26;              // Reserved VCL NAL unit types
        public const uint RSV_VCL27 = 27;              // Reserved VCL NAL unit types
        public const uint RSV_VCL28 = 28;              // Reserved VCL NAL unit types
        public const uint RSV_VCL29 = 29;              // Reserved VCL NAL unit types
        public const uint RSV_VCL30 = 30;              // Reserved VCL NAL unit types
        public const uint RSV_VCL31 = 31;              // Reserved VCL NAL unit types

        public const uint VPS_NUT = 32;                // Video parameter set
        public const uint SPS_NUT = 33;                // Sequence parameter set
        public const uint PPS_NUT = 34;                // Picture parameter set
        public const uint AUD_NUT = 35;                // Access unit delimiter
        public const uint EOS_NUT = 36;                // End of sequence
        public const uint EOB_NUT = 37;                // End of stream
        public const uint FD_NUT = 38;                 // Filler data
        public const uint PREFIX_SEI_NUT = 39;         // Prefix SEI
        public const uint SUFFIX_SEI_NUT = 40;         // Suffix SEI

        public const uint RSV_NVCL41 = 41;             // Reserved non-VCL NAL unit types
        public const uint RSV_NVCL42 = 42;             // Reserved non-VCL NAL unit types
        public const uint RSV_NVCL43 = 43;             // Reserved non-VCL NAL unit types
        public const uint RSV_NVCL44 = 44;             // Reserved non-VCL NAL unit types
        public const uint RSV_NVCL45 = 45;             // Reserved non-VCL NAL unit types
        public const uint RSV_NVCL46 = 46;             // Reserved non-VCL NAL unit types
        public const uint RSV_NVCL47 = 47;             // Reserved non-VCL NAL unit types
        public const uint UNSPEC48 = 48;               // Unspecified NAL unit types
        public const uint UNSPEC49 = 49;               // Unspecified NAL unit types
        public const uint UNSPEC50 = 50;               // Unspecified NAL unit types
        public const uint UNSPEC51 = 51;               // Unspecified NAL unit types
        public const uint UNSPEC52 = 52;               // Unspecified NAL unit types
        public const uint UNSPEC53 = 53;               // Unspecified NAL unit types
        public const uint UNSPEC54 = 54;               // Unspecified NAL unit types
        public const uint UNSPEC55 = 55;               // Unspecified NAL unit types
        public const uint UNSPEC56 = 56;               // Unspecified NAL unit types
        public const uint UNSPEC57 = 57;               // Unspecified NAL unit types
        public const uint UNSPEC58 = 58;               // Unspecified NAL unit types
        public const uint UNSPEC59 = 59;               // Unspecified NAL unit types
        public const uint UNSPEC60 = 60;               // Unspecified NAL unit types
        public const uint UNSPEC61 = 61;               // Unspecified NAL unit types
        public const uint UNSPEC62 = 62;               // Unspecified NAL unit types
        public const uint UNSPEC63 = 63;               // Unspecified NAL unit types
    }

    public partial class H265Context
    {
        public SeiPayload SeiPayload { get; set; }
        public Dictionary<ulong, SeqParameterSetRbsp> SeqParameterSets { get; } = new Dictionary<ulong, SeqParameterSetRbsp>();
        public Dictionary<ulong, PicParameterSetRbsp> PicParameterSets { get; } = new Dictionary<ulong, PicParameterSetRbsp>();

        public ulong[][][] BspSchedCnt { get; set; }
        public int[][] IdDirectRefLayer { get; set; }
        public int[][] LayerSetLayerIdList { get; set; }
        public uint[][] CpPresentFlag { get; set; }
        public int[][] DependencyFlag { get; set; }
        public int[][] IdRefLayer { get; set; }
        public int[][] IdPredictedLayer { get; set; }
        public int[][] TreePartitionLayerIdList { get; set; }
        public int[][] ScalabilityId { get; set; }
        public uint[][] OutputLayerFlag { get; set; }
        public uint[][] NecessaryLayerFlag { get; set; }
        public int[][] IdRefListLayer { get; set; }
        public uint[][] ViewCompLayerPresentFlag { get; set; }
        public int[][] ViewCompLayerId { get; set; }
        public uint[][] UsedByCurrPicS0 { get; set; }
        public uint[][] UsedByCurrPicS1 { get; set; }
        public int[][] DeltaPocS0 { get; set; }
        public int[][] DeltaPocS1 { get; set; }
        public int[] NumLayersInIdList { get; set; }
        public int[] MaxSubLayersInLayerSetMinus1 { get; set; }
        public uint[] NumDirectRefLayers { get; set; }
        public uint[] NumLayersInTreePartition { get; set; }
        public int[] layerIdInListFlag { get; set; }
        public uint[] NumRefLayers { get; set; }
        public uint[] NumPredictedLayers { get; set; }
        public int[] DependencyId { get; set; }
        public int[] AuxId { get; set; }
        public int[] DepthLayerFlag { get; set; } = new int[] { 0 };
        public int[] ViewOrderIdx { get; set; }
        public ulong[] OlsIdxToLsIdx { get; set; }
        public uint[] NumOutputLayersInOutputLayerSet { get; set; }
        public uint[] OlsHighestOutputLayerId { get; set; }
        public uint[] NumNecessaryLayers { get; set; }
        public uint[] LayerIdxInVps { get; set; }
        public uint[] NumRefListLayers { get; set; } = new uint[] { 0 };
        public uint[] ViewOIdxList { get; set; }
        public ulong[] NumNegativePics { get; set; }
        public ulong[] NumPositivePics { get; set; }
        public ulong[] NumDeltaPocs { get; set; }
        public uint[] MaxTemporalId { get; set; }
        public uint MaxLayersMinus1 { get; set; }
        public ulong CpbCnt { get; set; }
        public uint NumViews { get; set; }
        public uint NumIndependentLayers { get; set; }
        public ulong NumLayerSets { get; set; }
        public ulong FirstAddLayerSetIdx { get; set; }
        public ulong LastAddLayerSetIdx { get; set; }
        public int PicSizeInCtbsY { get; set; }
        public int MinCbLog2SizeY { get; set; }
        public int CtbLog2SizeY { get; set; }
        public int MinCbSizeY { get; set; }
        public int CtbSizeY { get; set; }
        public int PicWidthInMinCbsY { get; set; }
        public int PicWidthInCtbsY { get; set; }
        public int PicHeightInMinCbsY { get; set; }
        public int PicHeightInCtbsY { get; set; }
        public int PicSizeInMinCbsY { get; set; }
        public int PicSizeInSamplesY { get; set; }
        public int PicWidthInSamplesC { get; set; }
        public int PicHeightInSamplesC { get; set; }
        public int SubWidthC { get; set; }
        public int SubHeightC { get; set; }
        public uint VclInitialArrivalDelayPresent { get; set; }
        public uint NalInitialArrivalDelayPresent { get; set; }
        public ulong RefRpsIdx { get; set; }
        public ulong NumActiveRefLayerPics { get; set; }
        public uint[] refLayerPicIdc { get; set; }
        public uint TemporalId { get; set; }

        /// <summary>
        /// NumPicTotalCurr (ITU-T H.265 equation 7-57, extended by F.7.4.7.2).
        /// </summary>
        /// <remarks>
        /// Computed on demand rather than cached. The slice segment header consults it to decide
        /// whether ref_pic_lists_modification() is present, and that test happens *before*
        /// ref_pic_lists_modification() is parsed - which is the only place the old cached value
        /// was ever assigned. So the test used to read a stale value (0 on the first slice) and
        /// silently skipped ref_pic_lists_modification(), desynchronising the rest of the header.
        /// </remarks>
        public uint NumPicTotalCurr
        {
            get
            {
                uint count = 0;
                var header = SliceSegmentLayerRbsp?.SliceSegmentHeader;

                // Prefer the current picture's own st_ref_pic_set over the NumNegativePics /
                // NumPositivePics context arrays. Those are only refreshed from inside the
                // per-entry loops, so a set with zero entries in one direction leaves the
                // previous picture's count in place.
                var rps = ActiveStRefPicSet();
                if (rps != null && rps.InterRefPicSetPredictionFlag == 0)
                {
                    for (int i = 0; i < (int)rps.NumNegativePics; i++)
                        if (rps.UsedByCurrPicS0Flag[i] != 0)
                            count++;
                    for (int i = 0; i < (int)rps.NumPositivePics; i++)
                        if (rps.UsedByCurrPicS1Flag[i] != 0)
                            count++;
                }
                else
                {
                    // A predicted set has no entries of its own to count: its variables are
                    // worked out from the set it is predicted from. CurrRpsIdx (7-54 context) is
                    // worked out here: it was set only where short_term_ref_pic_set_idx is coded,
                    // so a slice coding its own set counted the SPS set of the slice before.
                    if (header != null)
                        CurrRpsIdx = header.ShortTermRefPicSetSpsFlag != 0 ? header.ShortTermRefPicSetIdx : SeqParameterSetRbsp.NumShortTermRefPicSets;
                    EnsureStRefPicSet(CurrRpsIdx);

                    if (NumNegativePics != null && UsedByCurrPicS0 != null &&
                        CurrRpsIdx < (ulong)NumNegativePics.Length && UsedByCurrPicS0[CurrRpsIdx] != null)
                    {
                        for (int i = 0; i < (int)NumNegativePics[CurrRpsIdx] && i < UsedByCurrPicS0[CurrRpsIdx].Length; i++)
                            if (UsedByCurrPicS0[CurrRpsIdx][i] != 0)
                                count++;
                    }

                    if (NumPositivePics != null && UsedByCurrPicS1 != null &&
                        CurrRpsIdx < (ulong)NumPositivePics.Length && UsedByCurrPicS1[CurrRpsIdx] != null)
                    {
                        for (int i = 0; i < (int)NumPositivePics[CurrRpsIdx] && i < UsedByCurrPicS1[CurrRpsIdx].Length; i++)
                            if (UsedByCurrPicS1[CurrRpsIdx][i] != 0)
                                count++;
                    }
                }

                // UsedByCurrPicLt (7-52), from the header: it was set only where
                // used_by_curr_pic_lt_flag is coded, so the entries taken from the SPS went
                // uncounted, and list_entry_lX was read a bit short.
                if (header != null)
                {
                    int numLt = (int)(header.NumLongTermSps + header.NumLongTermPics);
                    for (int i = 0; i < numLt; i++)
                    {
                        uint used;
                        if (i < (int)header.NumLongTermSps)
                        {
                            // lt_idx_sps[ i ] is 0 where not coded.
                            ulong idx = header.LtIdxSps != null && i < header.LtIdxSps.Length ? header.LtIdxSps[i] : 0;
                            var spsFlags = SeqParameterSetRbsp.UsedByCurrPicLtSpsFlag;
                            used = spsFlags != null && idx < (ulong)spsFlags.Length ? spsFlags[idx] : 0u;
                        }
                        else
                        {
                            used = header.UsedByCurrPicLtFlag != null && i < header.UsedByCurrPicLtFlag.Length ? header.UsedByCurrPicLtFlag[i] : 0u;
                        }
                        if (used != 0)
                            count++;
                    }
                }

                if (PicParameterSetRbsp?.PpsSccExtension != null &&
                    PicParameterSetRbsp.PpsSccExtension.PpsCurrPicRefEnabledFlag != 0)
                {
                    count++;
                }

                // F.7.4.7.2: multi-layer streams add the active inter-layer reference pictures.
                DeriveNumActiveRefLayerPics();
                count += (uint)NumActiveRefLayerPics;

                return count;
            }
        }

        /// <summary>
        /// The short-term reference picture set in force for the current slice, whether it was
        /// signalled inline or selected from the active SPS.
        /// </summary>
        private StRefPicSet ActiveStRefPicSet()
        {
            var header = SliceSegmentLayerRbsp?.SliceSegmentHeader;
            if (header == null)
                return null;

            if (header.ShortTermRefPicSetSpsFlag == 0)
                return header.StRefPicSet;

            var sets = SeqParameterSetRbsp?.StRefPicSet;
            if (sets == null || header.ShortTermRefPicSetIdx >= (ulong)sets.Length)
                return null;

            return sets[header.ShortTermRefPicSetIdx];
        }

        public ulong[] PocLsbLt { get; private set; }
        public uint[] UsedByCurrPicLt { get; set; }
        public ulong CurrRpsIdx { get; set; }

        /// <summary>
        /// True when the bitstream carries the 3D-HEVC (Annex I) extensions.
        /// </summary>
        /// <remarks>
        /// slice_ic_enabled_flag is an Annex I element. Plain MV-HEVC (Annex F) streams, such as
        /// Apple spatial video, must not have it parsed - reading it costs one bit and corrupts
        /// the remainder of the slice segment header.
        /// </remarks>
        public int Is3dExtension =>
            VideoParameterSetRbsp?.Vps3dExtension != null ? 1 : 0;

        public int inCmpPredAvailFlag { get; set; }
        public int DepthFlag { get; set; }
        public int ViewIdx { get; set; }
        public int[] curCmpLIds { get; set; }
        public int[] RefPicLayerId { get; set; }
        public int[] inCmpRefViewIdcs { get; set; }
        public int cpAvailableFlag { get; set; }
        public int allRefCmpLayersAvailFlag { get; set; }
        private ulong numCurCmpLIds { get; set; }
        private int refCmpCurLIdAvailFlag { get; set; }

        public void SetSeiPayload(SeiPayload payload)
        {
            if (SeiPayload == null)
            {
                SeiPayload = payload;
            }

            if (payload.ActiveParameterSets != null)
                SeiPayload.ActiveParameterSets = payload.ActiveParameterSets;
            if (payload.AlphaChannelInfo != null)
                SeiPayload.AlphaChannelInfo = payload.AlphaChannelInfo;
            if (payload.AlternativeDepthInfo != null)
                SeiPayload.AlternativeDepthInfo = payload.AlternativeDepthInfo;
            if (payload.AlternativeTransferCharacteristics != null)
                SeiPayload.AlternativeTransferCharacteristics = payload.AlternativeTransferCharacteristics;
            if (payload.AmbientViewingEnvironment != null)
                SeiPayload.AmbientViewingEnvironment = payload.AmbientViewingEnvironment;
            if (payload.BspInitialArrivalTime != null)
                SeiPayload.BspInitialArrivalTime = payload.BspInitialArrivalTime;
            if (payload.BspNesting != null)
                SeiPayload.BspNesting = payload.BspNesting;
            if (payload.BufferingPeriod != null)
                SeiPayload.BufferingPeriod = payload.BufferingPeriod;
            if (payload.ChromaResamplingFilterHint != null)
                SeiPayload.ChromaResamplingFilterHint = payload.ChromaResamplingFilterHint;
            if (payload.CodedRegionCompletion != null)
                SeiPayload.CodedRegionCompletion = payload.CodedRegionCompletion;
            if (payload.ColourRemappingInfo != null)
                SeiPayload.ColourRemappingInfo = payload.ColourRemappingInfo;
            if (payload.ContentColourVolume != null)
                SeiPayload.ContentColourVolume = payload.ContentColourVolume;
            if (payload.ContentLightLevelInfo != null)
                SeiPayload.ContentLightLevelInfo = payload.ContentLightLevelInfo;
            if (payload.CubemapProjection != null)
                SeiPayload.CubemapProjection = payload.CubemapProjection;
            if (payload.DecodedPictureHash != null)
                SeiPayload.DecodedPictureHash = payload.DecodedPictureHash;
            if (payload.DecodingUnitInfo != null)
                SeiPayload.DecodingUnitInfo = payload.DecodingUnitInfo;
            if (payload.DeinterlacedFieldIdentification != null)
                SeiPayload.DeinterlacedFieldIdentification = payload.DeinterlacedFieldIdentification;
            if (payload.DependentRapIndication != null)
                SeiPayload.DependentRapIndication = payload.DependentRapIndication;
            if (payload.DepthRepresentationInfo != null)
                SeiPayload.DepthRepresentationInfo = payload.DepthRepresentationInfo;
            if (payload.DisplayOrientation != null)
                SeiPayload.DisplayOrientation = payload.DisplayOrientation;
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
            if (payload.GreenMetadata != null)
                SeiPayload.GreenMetadata = payload.GreenMetadata;
            if (payload.InterLayerConstrainedTileSets != null)
                SeiPayload.InterLayerConstrainedTileSets = payload.InterLayerConstrainedTileSets;
            if (payload.KneeFunctionInfo != null)
                SeiPayload.KneeFunctionInfo = payload.KneeFunctionInfo;
            if (payload.LayersNotPresent != null)
                SeiPayload.LayersNotPresent = payload.LayersNotPresent;
            if (payload.MasteringDisplayColourVolume != null)
                SeiPayload.MasteringDisplayColourVolume = payload.MasteringDisplayColourVolume;
            if (payload.MctsExtractionInfoNesting != null)
                SeiPayload.MctsExtractionInfoNesting = payload.MctsExtractionInfoNesting;
            if (payload.MctsExtractionInfoSets != null)
                SeiPayload.MctsExtractionInfoSets = payload.MctsExtractionInfoSets;
            if (payload.MultiviewAcquisitionInfo != null)
                SeiPayload.MultiviewAcquisitionInfo = payload.MultiviewAcquisitionInfo;
            if (payload.MultiviewSceneInfo != null)
                SeiPayload.MultiviewSceneInfo = payload.MultiviewSceneInfo;
            if (payload.MultiviewViewPosition != null)
                SeiPayload.MultiviewViewPosition = payload.MultiviewViewPosition;
            if (payload.NoDisplay != null)
                SeiPayload.NoDisplay = payload.NoDisplay;
            if (payload.OmniViewport != null)
                SeiPayload.OmniViewport = payload.OmniViewport;
            if (payload.OverlayInfo != null)
                SeiPayload.OverlayInfo = payload.OverlayInfo;
            if (payload.PanScanRect != null)
                SeiPayload.PanScanRect = payload.PanScanRect;
            if (payload.PicTiming != null)
                SeiPayload.PicTiming = payload.PicTiming;
            if (payload.PictureSnapshot != null)
                SeiPayload.PictureSnapshot = payload.PictureSnapshot;
            if (payload.PostFilterHint != null)
                SeiPayload.PostFilterHint = payload.PostFilterHint;
            if (payload.ProgressiveRefinementSegmentEnd != null)
                SeiPayload.ProgressiveRefinementSegmentEnd = payload.ProgressiveRefinementSegmentEnd;
            if (payload.ProgressiveRefinementSegmentStart != null)
                SeiPayload.ProgressiveRefinementSegmentStart = payload.ProgressiveRefinementSegmentStart;
            if (payload.RecoveryPoint != null)
                SeiPayload.RecoveryPoint = payload.RecoveryPoint;
            if (payload.RegionalNesting != null)
                SeiPayload.RegionalNesting = payload.RegionalNesting;
            if (payload.RegionRefreshInfo != null)
                SeiPayload.RegionRefreshInfo = payload.RegionRefreshInfo;
            if (payload.RegionwisePacking != null)
                SeiPayload.RegionwisePacking = payload.RegionwisePacking;
            if (payload.ReservedSeiMessage != null)
                SeiPayload.ReservedSeiMessage = payload.ReservedSeiMessage;
            if (payload.ScalableNesting != null)
                SeiPayload.ScalableNesting = payload.ScalableNesting;
            if (payload.SceneInfo != null)
                SeiPayload.SceneInfo = payload.SceneInfo;
            if (payload.SegmentedRectFramePackingArrangement != null)
                SeiPayload.SegmentedRectFramePackingArrangement = payload.SegmentedRectFramePackingArrangement;
            if (payload.SphereRotation != null)
                SeiPayload.SphereRotation = payload.SphereRotation;
            if (payload.StructureOfPicturesInfo != null)
                SeiPayload.StructureOfPicturesInfo = payload.StructureOfPicturesInfo;
            if (payload.SubBitstreamProperty != null)
                SeiPayload.SubBitstreamProperty = payload.SubBitstreamProperty;
            if (payload.TemporalMotionConstrainedTileSets != null)
                SeiPayload.TemporalMotionConstrainedTileSets = payload.TemporalMotionConstrainedTileSets;
            if (payload.TemporalMvPredictionConstraints != null)
                SeiPayload.TemporalMvPredictionConstraints = payload.TemporalMvPredictionConstraints;
            if (payload.TemporalSubLayerZeroIdx != null)
                SeiPayload.TemporalSubLayerZeroIdx = payload.TemporalSubLayerZeroIdx;
            if (payload.ThreeDimensionalReferenceDisplaysInfo != null)
                SeiPayload.ThreeDimensionalReferenceDisplaysInfo = payload.ThreeDimensionalReferenceDisplaysInfo;
            if (payload.TimeCode != null)
                SeiPayload.TimeCode = payload.TimeCode;
            if (payload.ToneMappingInfo != null)
                SeiPayload.ToneMappingInfo = payload.ToneMappingInfo;
            if (payload.UserDataRegisteredItutT35 != null)
                SeiPayload.UserDataRegisteredItutT35 = payload.UserDataRegisteredItutT35;
            if (payload.UserDataUnregistered != null)
                SeiPayload.UserDataUnregistered = payload.UserDataUnregistered;
        }

        /// <summary>
        /// What a P or B slice leaves out and the spec infers (7.4.7.1): the active reference counts
        /// come from the picture parameter set unless the slice overrides them, and
        /// collocated_from_l0_flag, coded for B slices only, is 1. Later elements are conditioned on
        /// all three - collocated_ref_idx on the count of the list it names, the weight tables and
        /// list modifications on both - so leaving them 0 read a different header.
        /// </summary>
        public void OnNumRefIdxActiveOverrideFlag(SliceSegmentHeader header)
        {
            if (header.NumRefIdxActiveOverrideFlag == 0 && PicParameterSetRbsp != null)
            {
                header.NumRefIdxL0ActiveMinus1 = PicParameterSetRbsp.NumRefIdxL0DefaultActiveMinus1;
                header.NumRefIdxL1ActiveMinus1 = PicParameterSetRbsp.NumRefIdxL1DefaultActiveMinus1;
            }

            // Read over for a B slice further on.
            header.CollocatedFromL0Flag = 1;
        }

        /// <summary>
        /// Called just before a predicted set reads its entries, of which there are one more than
        /// NumDeltaPocs of the set it is predicted from - so that set has to be worked out by then,
        /// however it was coded itself.
        /// </summary>
        public void OnAbsDeltaRpsMinus1(ulong stRpsIdx, StRefPicSet set)
        {
            // delta_idx_minus1 is coded only for a slice's own set; elsewhere it is inferred to be 0.
            RefRpsIdx = stRpsIdx - (set.DeltaIdxMinus1 + 1); // 7-59
            EnsureStRefPicSet(RefRpsIdx);
        }

        /// <summary>
        /// The set each index of the arrays below was worked out from. A set is worked out again
        /// when a different one takes its index: another sequence parameter set's, or another
        /// slice's own.
        /// </summary>
        private StRefPicSet[] _stRefPicSetDerivedFrom = new StRefPicSet[0];

        /// <summary>The set at an index: the sequence parameter set's, then the slice's own after them.</summary>
        private StRefPicSet StRefPicSetAt(ulong stRpsIdx)
        {
            var sps = SeqParameterSetRbsp;
            if (sps?.StRefPicSet != null && stRpsIdx < sps.NumShortTermRefPicSets && stRpsIdx < (ulong)sps.StRefPicSet.Length)
                return sps.StRefPicSet[stRpsIdx];

            var header = SliceSegmentLayerRbsp?.SliceSegmentHeader;
            return header != null && sps != null && stRpsIdx == sps.NumShortTermRefPicSets ? header.StRefPicSet : null;
        }

        /// <summary>Works out a set's variables, unless they are already this set's.</summary>
        private void EnsureStRefPicSet(ulong stRpsIdx)
        {
            var set = StRefPicSetAt(stRpsIdx);
            if (set == null)
                return;

            if (stRpsIdx < (ulong)_stRefPicSetDerivedFrom.Length && ReferenceEquals(_stRefPicSetDerivedFrom[stRpsIdx], set))
                return;

            DeriveStRefPicSet(stRpsIdx, set);
        }

        /// <summary>
        /// NumNegativePics, NumPositivePics, DeltaPocS0/S1, UsedByCurrPicS0/S1 and NumDeltaPocs of one
        /// set (7-61 to 7-71). A coded set lists its pictures; a predicted one takes those of the set
        /// it is predicted from, shifted by deltaRps, keeping the ones use_delta_flag says to. Only
        /// the coded kind used to be worked out, and only from inside the loops that read its
        /// entries - so a predicted set, or an empty one, left whatever an earlier set had put there.
        /// </summary>
        private void DeriveStRefPicSet(ulong stRpsIdx, StRefPicSet set)
        {
            // collected in the context's own buffers, then copied to arrays of the exact size - the arrays already
            // there where they are, as a slice's own set is worked out again for every slice at the same index
            var s0 = new Entries(_s0Delta, _s0Used);
            var s1 = new Entries(_s1Delta, _s1Used);

            if (set.InterRefPicSetPredictionFlag == 0)
            {
                // Worked out as the entries are read, so an array not read yet may not be there.
                int negative = Math.Min((int)set.NumNegativePics, Math.Min(set.DeltaPocS0Minus1?.Length ?? 0, set.UsedByCurrPicS0Flag?.Length ?? 0));
                int positive = Math.Min((int)set.NumPositivePics, Math.Min(set.DeltaPocS1Minus1?.Length ?? 0, set.UsedByCurrPicS1Flag?.Length ?? 0));

                int poc = 0;
                for (int i = 0; i < negative; i++)
                {
                    poc -= (int)set.DeltaPocS0Minus1[i] + 1; // 7-67, 7-69
                    s0.Add((poc, set.UsedByCurrPicS0Flag[i])); // 7-65
                }

                poc = 0;
                for (int i = 0; i < positive; i++)
                {
                    poc += (int)set.DeltaPocS1Minus1[i] + 1; // 7-68, 7-70
                    s1.Add((poc, set.UsedByCurrPicS1Flag[i])); // 7-66
                }
            }
            else
            {
                ulong refIdx = stRpsIdx - (set.DeltaIdxMinus1 + 1); // 7-59
                EnsureStRefPicSet(refIdx);

                int deltaRps = (1 - 2 * set.DeltaRpsSign) * ((int)set.AbsDeltaRpsMinus1 + 1); // 7-60
                int refNegative = (int)ValueAt(NumNegativePics, refIdx);
                int refPositive = (int)ValueAt(NumPositivePics, refIdx);
                int refAll = refNegative + refPositive;
                int[] refS0 = ValueAt(DeltaPocS0, refIdx) ?? Array.Empty<int>();
                int[] refS1 = ValueAt(DeltaPocS1, refIdx) ?? Array.Empty<int>();

                uint Used(int j) => set.UsedByCurrPicFlag[j];

                // use_delta_flag is inferred to be 1 where it is not coded: where the picture is used.
                bool UseDelta(int j) => set.UsedByCurrPicFlag[j] != 0 || set.UseDeltaFlag[j] != 0;

                // 7-61
                for (int j = refPositive - 1; j >= 0; j--)
                {
                    int dPoc = refS1[j] + deltaRps;
                    if (dPoc < 0 && UseDelta(refNegative + j))
                        s0.Add((dPoc, Used(refNegative + j)));
                }

                if (deltaRps < 0 && UseDelta(refAll))
                    s0.Add((deltaRps, Used(refAll)));

                for (int j = 0; j < refNegative; j++)
                {
                    int dPoc = refS0[j] + deltaRps;
                    if (dPoc < 0 && UseDelta(j))
                        s0.Add((dPoc, Used(j)));
                }

                // 7-62
                for (int j = refNegative - 1; j >= 0; j--)
                {
                    int dPoc = refS0[j] + deltaRps;
                    if (dPoc > 0 && UseDelta(j))
                        s1.Add((dPoc, Used(j)));
                }

                if (deltaRps > 0 && UseDelta(refAll))
                    s1.Add((deltaRps, Used(refAll)));

                for (int j = 0; j < refPositive; j++)
                {
                    int dPoc = refS1[j] + deltaRps;
                    if (dPoc > 0 && UseDelta(refNegative + j))
                        s1.Add((dPoc, Used(refNegative + j)));
                }
            }

            int size = (int)stRpsIdx + 1;
            NumNegativePics = Grown(NumNegativePics, size);
            NumPositivePics = Grown(NumPositivePics, size);
            NumDeltaPocs = Grown(NumDeltaPocs, size);
            DeltaPocS0 = Grown(DeltaPocS0, size);
            DeltaPocS1 = Grown(DeltaPocS1, size);
            UsedByCurrPicS0 = Grown(UsedByCurrPicS0, size);
            UsedByCurrPicS1 = Grown(UsedByCurrPicS1, size);
            _stRefPicSetDerivedFrom = Grown(_stRefPicSetDerivedFrom, size);

            NumNegativePics[stRpsIdx] = (ulong)s0.Count;
            NumPositivePics[stRpsIdx] = (ulong)s1.Count;
            NumDeltaPocs[stRpsIdx] = (ulong)(s0.Count + s1.Count); // 7-71
            DeltaPocS0[stRpsIdx] = Copied(DeltaPocS0[stRpsIdx], s0.Delta, s0.Count);
            DeltaPocS1[stRpsIdx] = Copied(DeltaPocS1[stRpsIdx], s1.Delta, s1.Count);
            UsedByCurrPicS0[stRpsIdx] = Copied(UsedByCurrPicS0[stRpsIdx], s0.Used, s0.Count);
            UsedByCurrPicS1[stRpsIdx] = Copied(UsedByCurrPicS1[stRpsIdx], s1.Used, s1.Count);
            _stRefPicSetDerivedFrom[stRpsIdx] = set;

            // kept for the next set, as large as the largest so far
            (_s0Delta, _s0Used, _s1Delta, _s1Used) = (s0.Delta, s0.Used, s1.Delta, s1.Used);
        }

        private int[] _s0Delta = new int[16], _s1Delta = new int[16];
        private uint[] _s0Used = new uint[16], _s1Used = new uint[16];

        /// <summary>A set's pictures of one direction as they are worked out: into buffers that grow where they must.</summary>
        private struct Entries
        {
            public int[] Delta;
            public uint[] Used;
            public int Count;

            public Entries(int[] delta, uint[] used)
            {
                Delta = delta;
                Used = used;
                Count = 0;
            }

            public void Add((int Delta, uint Used) entry)
            {
                if (Count == Delta.Length)
                {
                    Array.Resize(ref Delta, Count * 2);
                    Array.Resize(ref Used, Count * 2);
                }
                Delta[Count] = entry.Delta;
                Used[Count] = entry.Used;
                Count++;
            }
        }

        /// <summary>The first so many of a buffer, in the array given where it is of that size already, else in a new one.</summary>
        private static T[] Copied<T>(T[] existing, T[] buffer, int count)
        {
            var array = existing != null && existing.Length == count ? existing : (count == 0 ? Array.Empty<T>() : new T[count]);
            Array.Copy(buffer, array, count);
            return array;
        }

        private static T ValueAt<T>(T[] array, ulong index) =>
            array != null && index < (ulong)array.Length ? array[index] : default(T);

        private static T[] Grown<T>(T[] array, int size)
        {
            if (array != null && array.Length >= size)
                return array;

            var grown = new T[size];
            if (array != null)
                Array.Copy(array, grown, array.Length);
            return grown;
        }

        public void OnNestingMaxTemporalIdPlus1(uint i)
        {
            var nesting_max_temporal_id_plus1 = SeiPayload.ScalableNesting.NestingMaxTemporalIdPlus1;
            MaxTemporalId[i] = nesting_max_temporal_id_plus1[i] - 1;
        }

        /// <summary>
        /// Called for each entry of a coded set as it is read; the set is worked out whole each time,
        /// so it is complete once its last entry is in. Growing the arrays here used to replace them
        /// with new ones, dropping every earlier set's values.
        /// </summary>
        public void OnUsedByCurrPicS0Flag(uint i, ulong stRpsIdx, StRefPicSet st_ref_pic_set)
        {
            DeriveStRefPicSet(stRpsIdx, st_ref_pic_set);
        }

        /// <summary>
        /// Called for each entry of a coded set as it is read; the set is worked out whole each time,
        /// so it is complete once its last entry is in. Growing the arrays here used to replace them
        /// with new ones, dropping every earlier set's values.
        /// </summary>
        public void OnUsedByCurrPicS1Flag(uint i, ulong stRpsIdx, StRefPicSet st_ref_pic_set)
        {
            DeriveStRefPicSet(stRpsIdx, st_ref_pic_set);
        }

        public void OnNumBspSchedulesMinus1(uint h, uint i, uint t)
        {
            var num_signalled_partitioning_schemes = VideoParameterSetRbsp.VpsExtension.VpsVui.VpsVuiBspHrdParams.NumSignalledPartitioningSchemes[h];

            if (BspSchedCnt == null ||
                BspSchedCnt.Length < (int)(num_signalled_partitioning_schemes + 1) ||
                BspSchedCnt[h].Length < (int)(num_signalled_partitioning_schemes + 1))
            {
                BspSchedCnt = new ulong[num_signalled_partitioning_schemes + 1][][];
                for (int j = 0; j < (int)num_signalled_partitioning_schemes + 1; j++)
                {
                    BspSchedCnt[j] = new ulong[num_signalled_partitioning_schemes + 1][];
                    for (int k = 0; k < (int)num_signalled_partitioning_schemes + 1; k++)
                    {
                        BspSchedCnt[j][k] = new ulong[MaxSubLayersInLayerSetMinus1[OlsIdxToLsIdx[h]] + 1];
                    }
                }
            }

            var num_bsp_schedules_minus1 = VideoParameterSetRbsp.VpsExtension.VpsVui.VpsVuiBspHrdParams.NumBspSchedulesMinus1;
            BspSchedCnt[h][i][t] = num_bsp_schedules_minus1[h][i][t] + 1;
        }

        public void OnNalHrdParametersPresentFlag(uint value)
        {
            NalInitialArrivalDelayPresent = value;
        }

        public void OnVclHrdParametersPresentFlag(uint value)
        {
            VclInitialArrivalDelayPresent = value;
        }

        /// <summary>How many values nuh_layer_id can take, and so how many layers a VPS can describe.</summary>
        private const int LayerIds = 64;

        /// <summary>How many values a view order index can take: it is a dimension_id, of up to 8 bits.</summary>
        private const int ViewOrderIdxs = 256;

        /// <summary>
        /// Called as a VPS reads vps_num_layer_sets_minus1: a new VPS, so everything worked out from
        /// the last one goes, and layer set 0 - never coded, only the base layer - is set up.
        /// </summary>
        /// <remarks>
        /// The layer variables used to be allocated once, sized for the first VPS, and indexed by
        /// layer count where the spec indexes them by nuh_layer_id, which can be anything up to 63;
        /// a stream whose layers were not numbered 0, 1, 2... or with more layer sets than layers
        /// ran off their end.
        /// </remarks>
        /// <summary>
        /// cross_layer_irap_aligned_flag is coded only when cross_layer_pic_type_aligned_flag is 0:
        /// pictures of one type across layers are IRAPs across layers too, and it is inferred to be
        /// vps_vui_present_flag, 1 here (F.7.4.3.1.4). Left 0, all_layers_idr_aligned_flag after it
        /// went unread, and the rest of the VUI a bit early.
        /// </summary>
        public void OnCrossLayerPicTypeAlignedFlag(VpsVui vui)
        {
            if (vui.CrossLayerPicTypeAlignedFlag != 0)
                vui.CrossLayerIrapAlignedFlag = 1;
        }

        public void OnVpsNumLayerSetsMinus1()
        {
            var vps_num_layer_sets_minus1 = VideoParameterSetRbsp.VpsNumLayerSetsMinus1;

            NumLayerSets = vps_num_layer_sets_minus1 + 1;
            FirstAddLayerSetIdx = 0;
            LastAddLayerSetIdx = 0;
            NumViews = 1;
            NumIndependentLayers = 1;

            NumLayersInIdList = new int[NumLayerSets];
            LayerSetLayerIdList = new int[NumLayerSets][];
            for (int k = 0; k < LayerSetLayerIdList.Length; k++)
                LayerSetLayerIdList[k] = new int[LayerIds];
            NumLayersInIdList[0] = 1;
            LayerSetLayerIdList[0][0] = 0;

            LayerIdxInVps = new uint[LayerIds];
            ScalabilityId = null;
            DepthLayerFlag = new int[LayerIds];
            ViewOrderIdx = new int[LayerIds];
            DependencyId = new int[LayerIds];
            AuxId = new int[LayerIds];
            DependencyFlag = null;
            IdDirectRefLayer = null;
            IdRefLayer = null;
            IdPredictedLayer = null;
            NumDirectRefLayers = new uint[LayerIds];
            NumRefLayers = new uint[LayerIds];
            NumPredictedLayers = new uint[LayerIds];
            TreePartitionLayerIdList = null;
            NumLayersInTreePartition = null;
            MaxSubLayersInLayerSetMinus1 = null;
            OlsIdxToLsIdx = null;
            OutputLayerFlag = null;
            NumOutputLayersInOutputLayerSet = null;
            OlsHighestOutputLayerId = null;
            NecessaryLayerFlag = null;
            NumNecessaryLayers = null;
            ViewOIdxList = null;
            NumRefListLayers = new uint[LayerIds];
            IdRefListLayer = null;
            ViewCompLayerPresentFlag = null;
            ViewCompLayerId = null;
            CpPresentFlag = null;
            BspSchedCnt = null;
        }

        public void OnLayerIDIncludedFlag(uint i, uint j)
        {
            var layer_id_included_flag = VideoParameterSetRbsp.LayerIdIncludedFlag;
            var vps_max_layer_id = VideoParameterSetRbsp.VpsMaxLayerId;

            int n = 0;
            for (int m = 0; m <= vps_max_layer_id; m++)
                if (layer_id_included_flag[i][m] != 0)
                    LayerSetLayerIdList[i][n++] = m;
            NumLayersInIdList[i] = n;
        }

        public void OnLayerSetIdxForOlsMinus1(uint i, ulong NumOutputLayerSets) // F-11
        {
            var layer_set_idx_for_ols_minus1 = VideoParameterSetRbsp.VpsExtension.LayerSetIdxForOlsMinus1;

            if (OlsIdxToLsIdx == null || OlsIdxToLsIdx.Length < (int)NumOutputLayerSets)
            {
                OlsIdxToLsIdx = new ulong[NumOutputLayerSets];
            }

            OlsIdxToLsIdx[i] = (i < NumLayerSets) ? i : (layer_set_idx_for_ols_minus1[i] + 1);

            // output_layer_flag is only coded for additional output layer sets, or when
            // default_output_layer_idc is 2. Otherwise the output layers are inferred, and so is
            // everything that follows from them - NecessaryLayerFlag first, which the
            // profile_tier_level_idx loop right after this reads. The derivation normally runs as
            // each flag is read, so when none are, it has to be run here.
            var vps_num_layer_sets_minus1 = VideoParameterSetRbsp.VpsNumLayerSetsMinus1;
            var defaultOutputLayerIdc = Math.Min(VideoParameterSetRbsp.VpsExtension.DefaultOutputLayerIdc, 2);
            if (!(i > vps_num_layer_sets_minus1 || defaultOutputLayerIdc == 2))
                OnOutputLayerFlag(i, 0);
        }

        public void OnOutputLayerFlag(uint ii, uint jj)
        {
            var NumOutputLayerSets = VideoParameterSetRbsp.VpsExtension.NumOutputLayerSets;
            var default_output_layer_idc = VideoParameterSetRbsp.VpsExtension.DefaultOutputLayerIdc;
            var output_layer_flag = VideoParameterSetRbsp.VpsExtension.OutputLayerFlag;
            var vps_num_layer_sets_minus1 = VideoParameterSetRbsp.VpsNumLayerSetsMinus1;

            var defaultOutputLayerIdc = Math.Min(default_output_layer_idc, 2);

            // Every output layer set has a row - the additional ones too, past the layer sets - as
            // long as its layer set has layers. Sized by the layer sets alone, and once, by whichever
            // set a row was mapped to before its own mapping was read, they ran out. Every call
            // works every row out again, so one resized loses nothing.
            if (OutputLayerFlag == null || OutputLayerFlag.Length < (int)NumOutputLayerSets)
                OutputLayerFlag = new uint[NumOutputLayerSets][];
            if (NecessaryLayerFlag == null || NecessaryLayerFlag.Length < (int)NumOutputLayerSets)
                NecessaryLayerFlag = new uint[NumOutputLayerSets][];
            for (int i = 0; i < (int)NumOutputLayerSets; i++)
            {
                int layers = NumLayersInIdList[OlsIdxToLsIdx[i]];
                if (OutputLayerFlag[i]?.Length != layers)
                    OutputLayerFlag[i] = new uint[layers];
                if (NecessaryLayerFlag[i]?.Length != layers)
                    NecessaryLayerFlag[i] = new uint[layers];
            }
            if (NumOutputLayersInOutputLayerSet == null || NumOutputLayersInOutputLayerSet.Length < (int)NumOutputLayerSets)
                NumOutputLayersInOutputLayerSet = new uint[NumOutputLayerSets];
            if (OlsHighestOutputLayerId == null || OlsHighestOutputLayerId.Length < (int)NumOutputLayerSets)
                OlsHighestOutputLayerId = new uint[NumOutputLayerSets];
            if (NumNecessaryLayers == null || NumNecessaryLayers.Length < (int)NumOutputLayerSets)
                NumNecessaryLayers = new uint[NumOutputLayerSets];

            if (defaultOutputLayerIdc == 0 || defaultOutputLayerIdc == 1)
            {
                for (int i = 0; i <= (int)vps_num_layer_sets_minus1; i++)
                {
                    int nuhLayerIdA = LayerSetLayerIdList[OlsIdxToLsIdx[i]][0]; // highest value in LayerSetLayerIdList[OlsIdxToLsIdx[i]]
                    for (int k = 0; k < NumLayersInIdList[OlsIdxToLsIdx[i]]; k++)
                    {
                        if (LayerSetLayerIdList[OlsIdxToLsIdx[i]][k] > nuhLayerIdA)
                            nuhLayerIdA = LayerSetLayerIdList[OlsIdxToLsIdx[i]][k];
                    }

                    // Every layer of the set: with default_output_layer_idc 0 all of them are output
                    // layers, with 1 only the highest - which is the last, so stopping one short left
                    // it out in both cases, and with it everything NecessaryLayerFlag decides.
                    for (int j = 0; j < NumLayersInIdList[OlsIdxToLsIdx[i]]; j++)
                    {
                        if (defaultOutputLayerIdc == 0 ||
                             LayerSetLayerIdList[OlsIdxToLsIdx[i]][j] == nuhLayerIdA)
                            OutputLayerFlag[i][j] = 1;
                        else
                            OutputLayerFlag[i][j] = 0;
                    }
                }
            }

            for (uint i = ((defaultOutputLayerIdc == 2) ? 0 : ((uint)vps_num_layer_sets_minus1 + 1)); i <= NumOutputLayerSets - 1; i++)
            {
                // Set 0 holds only the base layer, which is always output; its flags are never
                // coded. Later sets' flags may not have been read yet, since this runs as each
                // one is.
                if (i == 0)
                {
                    OutputLayerFlag[0][0] = 1;
                    continue;
                }
                if (output_layer_flag?[i] == null)
                    continue;

                for (int j = 0; j <= NumLayersInIdList[OlsIdxToLsIdx[i]] - 1; j++)
                {
                    OutputLayerFlag[i][j] = output_layer_flag[i][j];
                }
            }

            // F.7.4.3.1.1 counts the output layers of every output layer set, not only those whose
            // flags were coded. alt_output_layer_flag is present when a set has exactly one, so an
            // uncounted set left it out of the syntax.
            for (uint i = 0; i <= NumOutputLayerSets - 1; i++)
            {
                NumOutputLayersInOutputLayerSet[i] = 0;
                for (int j = 0; j < NumLayersInIdList[OlsIdxToLsIdx[i]]; j++)
                {
                    NumOutputLayersInOutputLayerSet[i] += OutputLayerFlag[i][j];
                    if (OutputLayerFlag[i][j] != 0)
                        OlsHighestOutputLayerId[i] = (uint)LayerSetLayerIdList[OlsIdxToLsIdx[i]][j];
                }
            }

            for (int olsIdx = 0; olsIdx < (int)NumOutputLayerSets; olsIdx++)
            {
                ulong lsIdx = OlsIdxToLsIdx[olsIdx];
                for (int lsLayerIdx = 0; lsLayerIdx < NumLayersInIdList[lsIdx]; lsLayerIdx++)
                    NecessaryLayerFlag[olsIdx][lsLayerIdx] = 0;
                for (int lsLayerIdx = 0; lsLayerIdx < NumLayersInIdList[lsIdx]; lsLayerIdx++)
                    if (OutputLayerFlag[olsIdx][lsLayerIdx] != 0)
                    {
                        NecessaryLayerFlag[olsIdx][lsLayerIdx] = 1;
                        int currLayerId = LayerSetLayerIdList[lsIdx][lsLayerIdx];
                        for (int rLsLayerIdx = 0; rLsLayerIdx < lsLayerIdx; rLsLayerIdx++)
                        {
                            int refLayerId = LayerSetLayerIdList[lsIdx][rLsLayerIdx];
                            if (DependencyFlag[LayerIdxInVps[currLayerId]][LayerIdxInVps[refLayerId]] != 0)
                                NecessaryLayerFlag[olsIdx][rLsLayerIdx] = 1;
                        }
                    }

                // The count follows the marking loop rather than sitting inside it. Nested, it
                // reused the marking loop's variable, ran it to the end after the first output
                // layer, and so no layer after that one was ever marked necessary.
                NumNecessaryLayers[olsIdx] = 0;
                for (int lsLayerIdx = 0; lsLayerIdx < NumLayersInIdList[lsIdx]; lsLayerIdx++)
                    NumNecessaryLayers[olsIdx] += NecessaryLayerFlag[olsIdx][lsLayerIdx];
            }
        }

        public void OnLog2DiffMaxMinLumaCodingBlockSize()
        {
            DerivePictureSizes(SeqParameterSetRbsp);
        }

        /// <summary>
        /// The picture size variables (7-10 to 7-22), worked out from one sequence parameter set.
        /// </summary>
        /// <remarks>
        /// They belong to the set a slice activates, not to the last one parsed, so they are worked
        /// out again whenever a slice activates one - see <see cref="SetSlicePicParameterSetId"/>.
        /// A stream with a set per layer, or a parser handed several, otherwise read a slice's
        /// slice_segment_address against another picture's size: its width is Ceil(Log2(
        /// PicSizeInCtbsY)), and a first slice has no address, so only the second slice of a
        /// picture went wrong, and everything after it in the header with it.
        /// </remarks>
        private void DerivePictureSizes(SeqParameterSetRbsp sps)
        {
            if (sps == null)
                return;

            var separate_colour_plane_flag = sps.SeparateColourPlaneFlag;
            var chroma_format_idc = sps.ChromaFormatIdc;
            var log2_min_luma_coding_block_size_minus3 = sps.Log2MinLumaCodingBlockSizeMinus3;
            var log2_diff_max_min_luma_coding_block_size = sps.Log2DiffMaxMinLumaCodingBlockSize;
            var pic_width_in_luma_samples = sps.PicWidthInLumaSamples;
            var pic_height_in_luma_samples = sps.PicHeightInLumaSamples;

            if (separate_colour_plane_flag == 0)
            {
                if (chroma_format_idc == 0)
                {
                    SubWidthC = 1;
                    SubHeightC = 1;
                }
                else if (chroma_format_idc == 1)
                {
                    SubWidthC = 2;
                    SubHeightC = 2;
                }
                else if (chroma_format_idc == 2)
                {
                    SubWidthC = 2;
                    SubHeightC = 1;
                }
                else if (chroma_format_idc == 3)
                {
                    SubWidthC = 1;
                    SubHeightC = 1;
                }
            }
            else
            {
                SubWidthC = 1;
                SubHeightC = 1;
            }

            MinCbLog2SizeY = (int)log2_min_luma_coding_block_size_minus3 + 3; // 7-10
            CtbLog2SizeY = MinCbLog2SizeY + (int)log2_diff_max_min_luma_coding_block_size; // 7-11
            MinCbSizeY = 1 << MinCbLog2SizeY; // 7-12
            CtbSizeY = 1 << CtbLog2SizeY; // 7-13
            PicWidthInMinCbsY = (int)pic_width_in_luma_samples / MinCbSizeY; // 7-14
            PicWidthInCtbsY = (int)Math.Ceiling((double)pic_width_in_luma_samples / CtbSizeY); // 7-15
            PicHeightInMinCbsY = (int)pic_height_in_luma_samples / MinCbSizeY; // 7-16
            PicHeightInCtbsY = (int)Math.Ceiling((double)pic_height_in_luma_samples / CtbSizeY); // 7-17
            PicSizeInMinCbsY = PicWidthInMinCbsY * PicHeightInMinCbsY; // 7-18
            PicSizeInCtbsY = PicWidthInCtbsY * PicHeightInCtbsY; // 7-19
            PicSizeInSamplesY = (int)pic_width_in_luma_samples * (int)pic_height_in_luma_samples; // 7-20
            PicWidthInSamplesC = (int)pic_width_in_luma_samples / SubWidthC; // 7-21
            PicHeightInSamplesC = (int)pic_height_in_luma_samples / SubHeightC; // 7-22
        }

        /// <summary>
        /// Called as max_tid_ref_present_flag is read, the element after the sub-layer counts:
        /// MaxSubLayersInLayerSetMinus1 (F-10), which dpb_size() loops on.
        /// </summary>
        /// <remarks>
        /// This hung on sub_layers_vps_max_minus1, which is coded only when
        /// vps_sub_layers_max_minus1_present_flag is 1 and otherwise inferred to be
        /// vps_max_sub_layers_minus1 - so it never ran for most streams, and when it did, it ran
        /// before the layers after the one just read had their counts.
        /// </remarks>
        public void OnMaxTidRefPresentFlag()
        {
            var extension = VideoParameterSetRbsp.VpsExtension;
            if (extension.VpsSubLayersMaxMinus1PresentFlag == 0)
            {
                extension.SubLayersVpsMaxMinus1 = new uint[MaxLayersMinus1 + 1];
                for (int i = 0; i <= MaxLayersMinus1; i++)
                    extension.SubLayersVpsMaxMinus1[i] = VideoParameterSetRbsp.VpsMaxSubLayersMinus1;
            }

            var sub_layers_vps_max_minus1 = extension.SubLayersVpsMaxMinus1;

            MaxSubLayersInLayerSetMinus1 = new int[NumLayerSets];
            for (int i = 0; i < (int)NumLayerSets; i++)
            {
                uint maxSlMinus1 = 0;
                for (int k = 0; k < NumLayersInIdList[i]; k++)
                {
                    int lId = LayerSetLayerIdList[i][k];
                    maxSlMinus1 = Math.Max(maxSlMinus1, sub_layers_vps_max_minus1[LayerIdxInVps[lId]]);
                }
                MaxSubLayersInLayerSetMinus1[i] = (int)maxSlMinus1;
            }
        }

        public void OnVpsMaxLayersMinus1()
        {
            var vps_max_layers_minus1 = VideoParameterSetRbsp.VpsMaxLayersMinus1;
            MaxLayersMinus1 = Math.Min(62, vps_max_layers_minus1);
        }

        public void OnCpbCntMinus1(uint i)
        {
            var cpb_cnt_minus1 = SeqParameterSetRbsp.VuiParameters.HrdParameters.CpbCntMinus1;
            CpbCnt = cpb_cnt_minus1[i] + 1; // E.3.3
        }

        public void OnNumAddLayerSets()
        {
            var vps_num_layer_sets_minus1 = VideoParameterSetRbsp.VpsNumLayerSetsMinus1;
            var num_add_layer_sets = VideoParameterSetRbsp.VpsExtension.NumAddLayerSets;

            NumLayerSets = vps_num_layer_sets_minus1 + 1 + num_add_layer_sets; // F-7
            if (num_add_layer_sets > 0)
            {
                // F-8
                FirstAddLayerSetIdx = vps_num_layer_sets_minus1 + 1;
                LastAddLayerSetIdx = FirstAddLayerSetIdx + num_add_layer_sets - 1;
            }

            // The additional layer sets follow the coded ones in the same lists.
            NumLayersInIdList = Grown(NumLayersInIdList, (int)NumLayerSets);
            LayerSetLayerIdList = Grown(LayerSetLayerIdList, (int)NumLayerSets);
            for (int k = 0; k < LayerSetLayerIdList.Length; k++)
                LayerSetLayerIdList[k] ??= new int[LayerIds];
        }

        public void OnHighestLayerIdxPlus1(uint i) // F-9
        {
            var num_add_layer_sets = VideoParameterSetRbsp.VpsExtension.NumAddLayerSets;
            var vps_num_layer_sets_minus1 = VideoParameterSetRbsp.VpsNumLayerSetsMinus1;
            var highest_layer_idx_plus1 = VideoParameterSetRbsp.VpsExtension.HighestLayerIdxPlus1;

            int layerNum = 0;
            uint lsIdx = (uint)vps_num_layer_sets_minus1 + 1 + i;
            for (int treeIdx = 1; treeIdx < NumIndependentLayers; treeIdx++)
            {
                for (int layerCnt = 0; layerCnt < (int)highest_layer_idx_plus1[i][treeIdx]; layerCnt++)
                {
                    LayerSetLayerIdList[lsIdx][layerNum++] = TreePartitionLayerIdList[treeIdx][layerCnt];
                }
            }
            NumLayersInIdList[lsIdx] = layerNum;
        }

        /// <summary>
        /// Called as view_id_len is read, the element after the layers' identifiers: everything
        /// that follows from those (F-1 to F-3), which view_id_val loops on and the dependencies
        /// right after index by.
        /// </summary>
        /// <remarks>
        /// This ran as each dimension_id was read, over every layer, so it read the dimension_id of
        /// layers not yet read. And dimension_id is coded only without splitting_flag,
        /// layer_id_in_nuh only with vps_nuh_layer_id_present_flag: neither's inference was made,
        /// so with splitting_flag nothing here ran at all, and without layer_id_in_nuh neither did
        /// LayerIdxInVps.
        /// </remarks>
        public void OnViewIdLen()
        {
            var extension = VideoParameterSetRbsp.VpsExtension;
            var layer_id_in_nuh = extension.LayerIdInNuh;
            var dimension_id = extension.DimensionId;
            var scalability_mask_flag = extension.ScalabilityMaskFlag;
            var splitting_flag = extension.SplittingFlag;

            int numScalabilityTypes = 0;
            for (int smIdx = 0; smIdx < 16; smIdx++)
                numScalabilityTypes += scalability_mask_flag[smIdx];

            // layer_id_in_nuh[i] is i when not coded, and layer 0's never is.
            for (int i = 0; i <= MaxLayersMinus1; i++)
            {
                if (i == 0 || extension.VpsNuhLayerIdPresentFlag == 0)
                    layer_id_in_nuh[i] = (uint)i;
                LayerIdxInVps[layer_id_in_nuh[i]] = (uint)i;
            }

            // With splitting_flag the dimensions are bit fields of nuh_layer_id, the last taking
            // what the others leave of its 6 bits (F-1); without, layer 0's are 0.
            var dimBitOffset = new int[numScalabilityTypes + 1];
            for (int j = 1; j < numScalabilityTypes; j++)
                dimBitOffset[j] = dimBitOffset[j - 1] + (int)extension.DimensionIdLenMinus1[j - 1] + 1;
            dimBitOffset[numScalabilityTypes] = 6;

            for (int i = 0; i <= MaxLayersMinus1; i++)
            {
                if (splitting_flag == 0 && i > 0)
                    continue;

                dimension_id[i] = new ulong[numScalabilityTypes];
                for (int j = 0; j < numScalabilityTypes && splitting_flag != 0; j++)
                    dimension_id[i][j] = (ulong)((layer_id_in_nuh[i] & ((1u << dimBitOffset[j + 1]) - 1)) >> dimBitOffset[j]);
            }

            // F-3
            ScalabilityId = new int[MaxLayersMinus1 + 1][];
            NumViews = 1;
            for (int i = 0; i <= MaxLayersMinus1; i++)
            {
                uint lId = layer_id_in_nuh[i];
                ScalabilityId[i] = new int[16];
                for (int smIdx = 0, j = 0; smIdx < 16; smIdx++)
                {
                    if (scalability_mask_flag[smIdx] != 0)
                        ScalabilityId[i][smIdx] = (int)dimension_id[i][j++];
                    else
                        ScalabilityId[i][smIdx] = 0;
                }
                DepthLayerFlag[lId] = ScalabilityId[i][0];
                ViewOrderIdx[lId] = ScalabilityId[i][1];
                DependencyId[lId] = ScalabilityId[i][2];
                AuxId[lId] = ScalabilityId[i][3];
                if (i > 0)
                {
                    uint newViewFlag = 1;
                    for (int j = 0; j < i; j++)
                        if (ViewOrderIdx[lId] == ViewOrderIdx[layer_id_in_nuh[j]])
                            newViewFlag = 0;
                    NumViews += newViewFlag;
                }
            }

            // With a single layer no direct_dependency_flag is read, and the dependencies are
            // worked out as the last one is; this covers that case.
            DeriveDependencies();
        }

        public void OnDirectDependencyType()
        {
            var layer_id_in_nuh = VideoParameterSetRbsp.VpsExtension.LayerIdInNuh;

            if (ViewOIdxList == null || ViewOIdxList.Length < MaxLayersMinus1 + 1)
                ViewOIdxList = new uint[MaxLayersMinus1 + 1];
            // Indexed by nuh_layer_id, and the view components by view order index - a dimension_id,
            // up to 8 bits - not by how many layers or views there are.
            if (NumRefListLayers == null || NumRefListLayers.Length < LayerIds)
                NumRefListLayers = new uint[LayerIds];
            if (IdRefListLayer == null)
            {
                IdRefListLayer = new int[LayerIds][];
                for (int i = 0; i < LayerIds; i++)
                    IdRefListLayer[i] = new int[MaxLayersMinus1 + 1];
            }
            if (ViewCompLayerPresentFlag == null)
            {
                ViewCompLayerPresentFlag = new uint[ViewOrderIdxs][];
                for (int i = 0; i < ViewOrderIdxs; i++)
                    ViewCompLayerPresentFlag[i] = new uint[2];
            }
            if (ViewCompLayerId == null)
            {
                ViewCompLayerId = new int[ViewOrderIdxs][];
                for (int i = 0; i < ViewOrderIdxs; i++)
                    ViewCompLayerId[i] = new int[2];
            }

            // I-7
            int idx = 0;
            ViewOIdxList[idx++] = 0;
            for (int i = 1; i <= MaxLayersMinus1; i++)
            {
                uint lId = layer_id_in_nuh[i];
                int newViewFlag = 1;
                for (int j = 0; j < i; j++)
                    if (ViewOrderIdx[layer_id_in_nuh[i]] == ViewOrderIdx[layer_id_in_nuh[j]])
                        newViewFlag = 0;
                if (newViewFlag != 0)
                    ViewOIdxList[idx++] = (uint)ViewOrderIdx[lId];
            }

            // I-8
            for (int i = 0; i <= MaxLayersMinus1; i++)
            {
                uint iNuhLId = layer_id_in_nuh[i];
                NumRefListLayers[iNuhLId] = 0;
                for (int j = 0; j < NumDirectRefLayers[iNuhLId]; j++)
                {
                    int jNuhLId = IdDirectRefLayer[iNuhLId][j];
                    if (DepthLayerFlag[iNuhLId] == DepthLayerFlag[jNuhLId])
                        IdRefListLayer[iNuhLId][NumRefListLayers[iNuhLId]++] = jNuhLId;
                }
            }

            // I-9
            for (int depFlag = 0; depFlag <= 1; depFlag++)
            {
                for (int i = 0; i < NumViews; i++)
                {
                    uint iViewOIdx = ViewOIdxList[i];
                    int layerId = -1;
                    for (int j = 0; j <= MaxLayersMinus1; j++)
                    {
                        int jNuhLId = (int)layer_id_in_nuh[j];
                        if (DepthLayerFlag[jNuhLId] == depFlag && ViewOrderIdx[jNuhLId] == iViewOIdx && DependencyId[jNuhLId] == 0 && AuxId[jNuhLId] == 0)
                            layerId = jNuhLId;

                    }
                    ViewCompLayerPresentFlag[iViewOIdx][depFlag] = (layerId != -1) ? 1u : 0u;
                    ViewCompLayerId[iViewOIdx][depFlag] = layerId;
                }
            }
        }

        /// <summary>
        /// Called as each direct_dependency_flag is read; the dependencies are worked out once the
        /// last is in.
        /// </summary>
        /// <remarks>
        /// Worked out at every flag, this read the rows of layers whose flags had not been read yet,
        /// which do not exist until then.
        /// </remarks>
        public void OnDirectDependencyFlag(uint i, uint j)
        {
            if (i == MaxLayersMinus1 && j == i - 1)
                DeriveDependencies();
        }

        /// <summary>
        /// What follows from the dependency flags (F-4 to F-6): which layers each depends on,
        /// directly or not, which depend on it, and the trees of layers the independent ones head,
        /// down to NumIndependentLayers, on which num_add_layer_sets is conditioned.
        /// </summary>
        private void DeriveDependencies()
        {
            var direct_dependency_flag = VideoParameterSetRbsp.VpsExtension.DirectDependencyFlag;
            var layer_id_in_nuh = VideoParameterSetRbsp.VpsExtension.LayerIdInNuh;
            int layers = (int)MaxLayersMinus1 + 1;

            // Flags not coded - j >= i - or not read yet are 0.
            int Direct(int i, int j) => direct_dependency_flag?[i] != null && j < direct_dependency_flag[i].Length ? direct_dependency_flag[i][j] : 0;

            // F-4
            DependencyFlag = new int[layers][];
            for (int i = 0; i < layers; i++)
            {
                DependencyFlag[i] = new int[layers];
                for (int j = 0; j < layers; j++)
                {
                    DependencyFlag[i][j] = Direct(i, j);
                    for (int k = 0; k < i; k++)
                        if (Direct(i, k) != 0 && DependencyFlag[k][j] != 0)
                            DependencyFlag[i][j] = 1;
                }
            }

            // F-5, indexed by nuh_layer_id
            IdDirectRefLayer = new int[LayerIds][];
            IdRefLayer = new int[LayerIds][];
            IdPredictedLayer = new int[LayerIds][];
            NumDirectRefLayers = new uint[LayerIds];
            NumRefLayers = new uint[LayerIds];
            NumPredictedLayers = new uint[LayerIds];
            for (int i = 0; i < layers; i++)
            {
                int iNuhLId = (int)layer_id_in_nuh[i];
                IdDirectRefLayer[iNuhLId] = new int[layers];
                IdRefLayer[iNuhLId] = new int[layers];
                IdPredictedLayer[iNuhLId] = new int[layers];

                int d = 0, r = 0, p = 0;
                for (int j = 0; j < layers; j++)
                {
                    int jNuhLid = (int)layer_id_in_nuh[j];
                    if (Direct(i, j) != 0)
                        IdDirectRefLayer[iNuhLId][d++] = jNuhLid;
                    if (DependencyFlag[i][j] != 0)
                        IdRefLayer[iNuhLId][r++] = jNuhLid;
                    if (DependencyFlag[j][i] != 0)
                        IdPredictedLayer[iNuhLId][p++] = jNuhLid;
                }
                NumDirectRefLayers[iNuhLId] = (uint)d;
                NumRefLayers[iNuhLId] = (uint)r;
                NumPredictedLayers[iNuhLId] = (uint)p;
            }

            // F-6. A tree lists the layer heading it first and the layers predicted from it after,
            // so those start at 1; starting at 0 wrote the first over its head and counted a layer
            // short, which is the width highest_layer_idx_plus1 is read at.
            layerIdInListFlag = new int[LayerIds];
            TreePartitionLayerIdList = new int[layers][];
            NumLayersInTreePartition = new uint[layers];
            uint trees = 0;
            for (int i = 0; i < layers; i++)
            {
                int iNuhLId = (int)layer_id_in_nuh[i];
                if (NumDirectRefLayers[iNuhLId] == 0)
                {
                    TreePartitionLayerIdList[trees] = new int[layers];
                    TreePartitionLayerIdList[trees][0] = iNuhLId;
                    uint h = 1;
                    for (int j = 0; j < NumPredictedLayers[iNuhLId]; j++)
                    {
                        int predLId = IdPredictedLayer[iNuhLId][j];
                        if (layerIdInListFlag[predLId] == 0)
                        {
                            TreePartitionLayerIdList[trees][h++] = predLId;
                            layerIdInListFlag[predLId] = 1;
                        }
                    }
                    NumLayersInTreePartition[trees++] = h;
                }
            }
            NumIndependentLayers = trees;
        }

        public void OnCpRefVoi() // I-12
        {
            var num_cp = VideoParameterSetRbsp.Vps3dExtension.NumCp;
            var cp_ref_voi = VideoParameterSetRbsp.Vps3dExtension.CpRefVoi;

            // Both indices are view order indices, not counts of views.
            if (CpPresentFlag == null)
                CpPresentFlag = new uint[ViewOrderIdxs][];

            for (int n = 1; n < NumViews; n++)
            {
                uint i = ViewOIdxList[n];
                CpPresentFlag[i] ??= new uint[ViewOrderIdxs];
                for (int m = 0; m < num_cp[i]; m++)
                    CpPresentFlag[i][cp_ref_voi[i][m]] = 1;
            }
        }

        public void OnNumInterLayerRefPicsMinus1()
        {
            DeriveNumActiveRefLayerPics();
        }

        /// <summary>
        /// Derives NumActiveRefLayerPics (ITU-T H.265 F.7.4.7.1).
        /// </summary>
        /// <remarks>
        /// This used to run only from <see cref="OnNumInterLayerRefPicsMinus1"/>, i.e. only when
        /// num_inter_layer_ref_pics_minus1 is actually present in the slice header. When
        /// default_ref_layers_active_flag is 1 the whole inter-layer block is absent from the
        /// slice header, so the derivation never ran and NumActiveRefLayerPics stayed 0.
        /// Apple MV-HEVC (spatial video) encodes exactly that way, which then made
        /// NumPicTotalCurr too small and dropped ref_pic_lists_modification() from the parse.
        /// </remarks>
        public void DeriveNumActiveRefLayerPics()
        {
            if (NalHeader?.NalUnitHeader == null || VideoParameterSetRbsp?.VpsExtension == null)
            {
                NumActiveRefLayerPics = 0;
                return;
            }

            var nuh_layer_id = NalHeader.NalUnitHeader.NuhLayerId;
            var sub_layers_vps_max_minus1 = VideoParameterSetRbsp.VpsExtension.SubLayersVpsMaxMinus1;
            var max_tid_il_ref_pics_plus1 = VideoParameterSetRbsp.VpsExtension.MaxTidIlRefPicsPlus1;
            var default_ref_layers_active_flag = VideoParameterSetRbsp.VpsExtension.DefaultRefLayersActiveFlag;
            var inter_layer_pred_enabled_flag = SliceSegmentLayerRbsp?.SliceSegmentHeader == null ? (byte)0 : SliceSegmentLayerRbsp.SliceSegmentHeader.InterLayerPredEnabledFlag;
            var max_one_active_ref_layer_flag = VideoParameterSetRbsp.VpsExtension.MaxOneActiveRefLayerFlag;
            var num_inter_layer_ref_pics_minus1 = SliceSegmentLayerRbsp?.SliceSegmentHeader == null ? 0 : SliceSegmentLayerRbsp.SliceSegmentHeader.NumInterLayerRefPicsMinus1;

            if (refLayerPicIdc == null || refLayerPicIdc.Length < MaxLayersMinus1 + 1)
            {
                refLayerPicIdc = new uint[MaxLayersMinus1 + 1];
            }

            // F.7.4.3.1.1: when vps_sub_layers_max_minus1_present_flag is 0, sub_layers_vps_max_minus1[i]
            // is inferred to be vps_max_sub_layers_minus1; when max_tid_ref_present_flag is 0,
            // max_tid_il_ref_pics_plus1[i][j] is inferred to be 7. Both arrays are then absent.
            uint SubLayersVpsMaxMinus1(uint i) =>
                sub_layers_vps_max_minus1 != null && i < sub_layers_vps_max_minus1.Length
                    ? sub_layers_vps_max_minus1[i]
                    : VideoParameterSetRbsp.VpsMaxSubLayersMinus1;

            uint MaxTidIlRefPicsPlus1(uint i, uint k) =>
                max_tid_il_ref_pics_plus1 != null && i < max_tid_il_ref_pics_plus1.Length &&
                max_tid_il_ref_pics_plus1[i] != null && k < max_tid_il_ref_pics_plus1[i].Length
                    ? max_tid_il_ref_pics_plus1[i][k]
                    : 7u;

            // The layers of the reference picture lists (I.7.4.7.1): the direct reference layers,
            // less, in 3D-HEVC, those of the other component - a depth layer's texture - which
            // refLayerPicIdc indexes IdRefListLayer for. Counted over all direct reference layers,
            // a depth slice read list_entry_lX a bit too wide.
            uint numRefListLayers = NumRefListLayers != null && nuh_layer_id < NumRefListLayers.Length ? NumRefListLayers[nuh_layer_id] : (uint)NumDirectRefLayers[nuh_layer_id];
            int RefListLayer(uint i) => NumRefListLayers != null && nuh_layer_id < NumRefListLayers.Length ? IdRefListLayer[nuh_layer_id][i] : IdDirectRefLayer[nuh_layer_id][i];

            uint j = 0;
            for (uint i = 0; i < numRefListLayers; i++)
            {
                uint refLayerIdx = LayerIdxInVps[RefListLayer(i)];
                if (SubLayersVpsMaxMinus1(refLayerIdx) >= TemporalId &&
                    (TemporalId == 0 || MaxTidIlRefPicsPlus1(refLayerIdx, LayerIdxInVps[nuh_layer_id]) > TemporalId))
                    refLayerPicIdc[j++] = i;
            }
            uint numRefLayerPics = j;

            if (nuh_layer_id == 0 || numRefLayerPics == 0)
                NumActiveRefLayerPics = 0;
            else if (default_ref_layers_active_flag != 0)
                NumActiveRefLayerPics = numRefLayerPics;
            else if (inter_layer_pred_enabled_flag == 0)
                NumActiveRefLayerPics = 0;
            else if (max_one_active_ref_layer_flag != 0 || numRefListLayers == 1)
                NumActiveRefLayerPics = 1;
            else
                NumActiveRefLayerPics = num_inter_layer_ref_pics_minus1 + 1;
        }

        public void OnNuhTemporalIdPlus1()
        {
            var nuh_temporal_id_plus1 = NalHeader.NalUnitHeader.NuhTemporalIdPlus1;
            TemporalId = nuh_temporal_id_plus1 - 1;
        }

        public void OnListEntryL0()
        {
            // NumPicTotalCurr is now derived on demand, see the property.
        }

        public void OnUsedByCurrPicLtFlag(uint i)
        {
            var num_long_term_sps = SliceSegmentLayerRbsp.SliceSegmentHeader.NumLongTermSps;
            var num_long_term_pics = SliceSegmentLayerRbsp.SliceSegmentHeader.NumLongTermPics;
            var lt_ref_pic_poc_lsb_sps = SeqParameterSetRbsp.LtRefPicPocLsbSps;
            var lt_idx_sps = SliceSegmentLayerRbsp.SliceSegmentHeader.LtIdxSps;
            var used_by_curr_pic_lt_sps_flag = SeqParameterSetRbsp.UsedByCurrPicLtSpsFlag;
            var poc_lsb_lt = SliceSegmentLayerRbsp.SliceSegmentHeader.PocLsbLt;
            var used_by_curr_pic_lt_flag = SliceSegmentLayerRbsp.SliceSegmentHeader.UsedByCurrPicLtFlag;

            if(PocLsbLt == null || PocLsbLt.Length < (int)(num_long_term_sps + num_long_term_pics))
            {
                PocLsbLt = new ulong[num_long_term_sps + num_long_term_pics];
            }
            if(UsedByCurrPicLt == null || UsedByCurrPicLt.Length < (int)(num_long_term_sps + num_long_term_pics))
            {
                UsedByCurrPicLt = new uint[num_long_term_sps + num_long_term_pics];
            }

            if (i < num_long_term_sps)
            {
                PocLsbLt[i] = lt_ref_pic_poc_lsb_sps[lt_idx_sps[i]];
                UsedByCurrPicLt[i] = used_by_curr_pic_lt_sps_flag[lt_idx_sps[i]];
            }
            else
            {
                PocLsbLt[i] = poc_lsb_lt[i];
                UsedByCurrPicLt[i] = used_by_curr_pic_lt_flag[i];
            }
        }

        public void OnShortTermRefPicSetIdx()
        {
            var short_term_ref_pic_set_sps_flag = SliceSegmentLayerRbsp.SliceSegmentHeader.ShortTermRefPicSetSpsFlag;
            var short_term_ref_pic_set_idx = SliceSegmentLayerRbsp.SliceSegmentHeader.ShortTermRefPicSetIdx;
            var num_short_term_ref_pic_sets = SeqParameterSetRbsp.NumShortTermRefPicSets;

            if (short_term_ref_pic_set_sps_flag == 1)
            {
                CurrRpsIdx = short_term_ref_pic_set_idx;
            }
            else
            {
                CurrRpsIdx = num_short_term_ref_pic_sets;
            }
        }

        public void OnInterLayerPredLayerIdc()
        {
            var nuh_layer_id = NalHeader.NalUnitHeader.NuhLayerId;
            var inter_layer_pred_layer_idc = SliceSegmentLayerRbsp.SliceSegmentHeader.InterLayerPredLayerIdc;

            if(RefPicLayerId == null || RefPicLayerId.Length < MaxLayersMinus1 + 1)
            {
                RefPicLayerId = new int[MaxLayersMinus1 + 1];
            }

            for (int i = 0; i < (int)NumActiveRefLayerPics; i++)
                RefPicLayerId[i] = IdRefListLayer[nuh_layer_id][inter_layer_pred_layer_idc[i]];
        }

        /// <summary>
        /// The arrays of one colour mapping octant's residuals, which colour_mapping_octants()
        /// indexes by where the octant sits in the whole table - [ idxShiftY ][ idxCb ][ idxCr ] -
        /// then by vertex and, for the coefficients, by colour component. Each octant got arrays
        /// as long as its own loops instead, allocated at the loop's index and read at the
        /// table's, so the first residual flag read threw.
        /// </summary>
        public T[][][][] ColourMappingOctantGrid<T>(int vertices) =>
            OctantGrid(() => new T[vertices]);

        /// <inheritdoc cref="ColourMappingOctantGrid{T}(int)"/>
        public T[][][][][] ColourMappingOctantGrid<T>(int vertices, int components) =>
            OctantGrid(() =>
            {
                var perVertex = new T[vertices][];
                for (int j = 0; j < vertices; j++)
                    perVertex[j] = new T[components];
                return perVertex;
            });

        private TOctant[][][] OctantGrid<TOctant>(Func<TOctant> octant)
        {
            var table = PicParameterSetRbsp.PpsMultilayerExtension.ColourMappingTable;
            int side = 1 << (int)table.CmOctantDepth;
            int lumaSide = (1 << (int)table.CmyPartNumLog2) * side;

            var grid = new TOctant[lumaSide][][];
            for (int y = 0; y < lumaSide; y++)
            {
                grid[y] = new TOctant[side][];
                for (int cb = 0; cb < side; cb++)
                {
                    grid[y][cb] = new TOctant[side];
                    for (int cr = 0; cr < side; cr++)
                        grid[y][cb][cr] = octant();
                }
            }
            return grid;
        }

        /// <summary>
        /// ScalingList[ sizeId ][ matrixId ][ i ] (7.4.5): four sizes of six matrices of up to 64
        /// coefficients, which scaling_list_data() fills as it reads.
        /// </summary>
        public static uint[][][] NewScalingList()
        {
            var list = new uint[4][][];
            for (int sizeId = 0; sizeId < 4; sizeId++)
            {
                list[sizeId] = new uint[6][];
                for (int matrixId = 0; matrixId < 6; matrixId++)
                    list[sizeId][matrixId] = new uint[64];
            }
            return list;
        }

        public void OnSliceType()
        {
            if (SeqParameterSetRbsp.Sps3dExtension == null)
                return;

            // I.7.4.7.1, the part the slice's layer decides alone.
            var nuh_layer_id = NalHeader.NalUnitHeader.NuhLayerId;
            DepthFlag = DepthLayerFlag[nuh_layer_id];
            ViewIdx = ViewOrderIdx[nuh_layer_id];
        }

        public void OnInterLayerPredEnabledFlag()
        {
            // Needed straight away, for whether inter_layer_pred_layer_idc is coded - and
            // num_inter_layer_ref_pics_minus1, which would work it out again, is not always.
            DeriveNumActiveRefLayerPics();
        }

        /// <summary>
        /// inCmpPredAvailFlag (I.7.4.7.1), worked out where in_comp_pred_flag is conditioned on it.
        /// </summary>
        /// <remarks>
        /// It depends on RefPicLayerId, the layers the slice predicts from, which the slice header
        /// codes after slice_type. Worked out at slice_type, as it was, it read the previous slice's
        /// layers - or none, and threw. The loop also stopped one layer short, and a layer whose
        /// sub-layers reached exactly as high as the slice's own did not count as available.
        /// </remarks>
        public int DeriveInCmpPredAvailFlag()
        {
            inCmpPredAvailFlag = 0;
            var sps3d = SeqParameterSetRbsp.Sps3dExtension;
            if (sps3d == null || VideoParameterSetRbsp?.VpsExtension == null)
                return inCmpPredAvailFlag;

            var nuh_layer_id = NalHeader.NalUnitHeader.NuhLayerId;
            var extension = VideoParameterSetRbsp.VpsExtension;
            var header = SliceSegmentLayerRbsp.SliceSegmentHeader;

            // RefPicLayerId (F.7.4.7.1). inter_layer_pred_layer_idc is coded only when not every
            // reference layer is active; otherwise the active ones are the first, in order - or,
            // with default_ref_layers_active_flag, those refLayerPicIdc lists.
            DeriveNumActiveRefLayerPics();
            RefPicLayerId = new int[NumActiveRefLayerPics];
            for (int i = 0; i < (int)NumActiveRefLayerPics; i++)
            {
                ulong idc =
                    header.InterLayerPredLayerIdc != null && i < header.InterLayerPredLayerIdc.Length && extension.DefaultRefLayersActiveFlag == 0 ? header.InterLayerPredLayerIdc[i] :
                    extension.DefaultRefLayersActiveFlag != 0 ? refLayerPicIdc[i] :
                    (ulong)i;
                RefPicLayerId[i] = IdRefListLayer[nuh_layer_id][idc];
            }

            uint Direct(uint i, uint j) =>
                extension.DirectDependencyFlag?[i] != null && j < extension.DirectDependencyFlag[i].Length ? extension.DirectDependencyFlag[i][j] : 0u;
            uint MaxTidIlRefPicsPlus1(uint i, uint j) =>
                extension.MaxTidIlRefPicsPlus1?[i] != null && j < extension.MaxTidIlRefPicsPlus1[i].Length ? extension.MaxTidIlRefPicsPlus1[i][j] : 7u;
            uint CpPresent(int i, int j) => CpPresentFlag?[i] != null ? CpPresentFlag[i][j] : 0u;

            curCmpLIds = DepthFlag != 0 ? new int[] { (int)nuh_layer_id } : RefPicLayerId;
            numCurCmpLIds = DepthFlag != 0 ? 1 : NumActiveRefLayerPics;

            cpAvailableFlag = 1;
            allRefCmpLayersAvailFlag = 1;
            inCmpRefViewIdcs = new int[numCurCmpLIds];

            uint layerIdx = LayerIdxInVps[nuh_layer_id];
            int otherComponent = DepthFlag == 0 ? 1 : 0;
            for (int i = 0; i < (int)numCurCmpLIds; i++)
            {
                inCmpRefViewIdcs[i] = ViewOrderIdx[curCmpLIds[i]];
                if (CpPresent(ViewIdx, inCmpRefViewIdcs[i]) == 0)
                    cpAvailableFlag = 0;

                refCmpCurLIdAvailFlag = 0;
                if (ViewCompLayerPresentFlag[inCmpRefViewIdcs[i]][otherComponent] == 1)
                {
                    uint j = LayerIdxInVps[ViewCompLayerId[inCmpRefViewIdcs[i]][otherComponent]];
                    if (Direct(layerIdx, j) == 1 &&
                        extension.SubLayersVpsMaxMinus1[j] >= TemporalId &&
                        (TemporalId == 0 || MaxTidIlRefPicsPlus1(j, layerIdx) > TemporalId))
                        refCmpCurLIdAvailFlag = 1;
                }
                if (refCmpCurLIdAvailFlag == 0)
                    allRefCmpLayersAvailFlag = 0;
            }

            if (allRefCmpLayersAvailFlag != 0)
            {
                if (DepthFlag == 0)
                    inCmpPredAvailFlag = (sps3d.VspMcEnabledFlag[DepthFlag] != 0 || sps3d.DbbpEnabledFlag[DepthFlag] != 0 || sps3d.DepthRefEnabledFlag[DepthFlag] != 0) ? 1 : 0;
                else
                    inCmpPredAvailFlag = (sps3d.IntraContourEnabledFlag[DepthFlag] != 0 || sps3d.CqtCuPartPredEnabledFlag[DepthFlag] != 0 || sps3d.TexMcEnabledFlag[DepthFlag] != 0) ? 1 : 0;
            }

            return inCmpPredAvailFlag;
        }

        /// <summary>
        /// Registers the sequence parameter set being parsed. One sent again under the same id
        /// replaces the one there, as the spec has it: that is how a stream changes resolution, or
        /// two streams are joined. Keeping the first read every later slice against the old set.
        /// </summary>
        public void SetSpsSeqParameterSetId(ulong sps_seq_parameter_set_id)
        {
            if (SeqParameterSetRbsp == null)
                return;

            SeqParameterSets[SeqParameterSetRbsp.SpsSeqParameterSetId] = SeqParameterSetRbsp;
        }

        /// <summary>
        /// A PPS may come before the SPS it names (VPSSPSPPS_A, RPS_C): nothing in its syntax
        /// takes from the SPS, and the slice that activates it activates the SPS as well. It
        /// threw on a PPS without an SPS before it.
        /// </summary>
        /// <summary>
        /// fixed_pic_rate_within_cvs_flag is coded only when fixed_pic_rate_general_flag is 0, and
        /// is 1 otherwise (E.3.2): left 0, low_delay_hrd_flag was read where
        /// elemental_duration_in_tc_minus1 is.
        /// </summary>
        public void OnFixedPicRateGeneralFlag(HrdParameters hrd, uint i)
        {
            if (hrd.FixedPicRateGeneralFlag[i] != 0)
                hrd.FixedPicRateWithinCvsFlag[i] = 1;
        }

        /// <summary>The SPS a slice of each layer last activated.</summary>
        public SeqParameterSetRbsp[] ActiveSeqParameterSets { get; } = new SeqParameterSetRbsp[64];

        /// <summary>
        /// chroma_format_idc of the picture a decoded picture hash is for: that of the SPS active in
        /// the SEI's layer. It was taken from the SPS parsed last, which in a multi-layer stream is
        /// often another layer's - one that takes its format from the VPS, and so read as 4:0:0.
        /// </summary>
        public ulong ChromaFormatIdcOfSeiLayer()
        {
            var sps = ActiveSeqParameterSets[NalHeader?.NalUnitHeader?.NuhLayerId ?? 0] ?? SeqParameterSetRbsp;
            return sps?.ChromaFormatIdc ?? 1;
        }

        public void SetPpsSeqParameterSetId(ulong pps_seq_parameter_set_id)
        {
            if (SeqParameterSets.TryGetValue(pps_seq_parameter_set_id, out var sps))
                SeqParameterSetRbsp = sps;
            else if (SeqParameterSetRbsp == null || SeqParameterSetRbsp.SpsSeqParameterSetId != pps_seq_parameter_set_id)
                return;

            ResolveMultiLayerRepFormat(SeqParameterSetRbsp);
        }

        /// <summary>
        /// Copies the VPS rep_format() into an SPS that did not code the picture format itself
        /// (ITU-T H.265 F.7.4.3.2.1, MultiLayerExtSpsFlag equal to 1).
        /// </summary>
        /// <remarks>
        /// Such an SPS omits chroma_format_idc, the picture size, the bit depths and the
        /// conformance window, inheriting them from the VPS instead. Without this the fields stay
        /// zero, so ChromaArrayType reads as 0 and slice_sao_chroma_flag is dropped from the
        /// slice header parse - one bit short, corrupting everything after it.
        /// </remarks>
        public void ResolveMultiLayerRepFormat(SeqParameterSetRbsp sps)
        {
            // A conforming SPS that codes its own format always has a non-zero width.
            if (sps == null || sps.PicWidthInLumaSamples != 0)
                return;

            var ext = VideoParameterSetRbsp?.VpsExtension;
            var repFormats = ext?.RepFormat;
            if (repFormats == null || repFormats.Length == 0)
                return;

            ulong idx = 0;
            if (sps.UpdateRepFormatFlag != 0)
            {
                idx = sps.SpsRepFormatIdx;
            }
            else if (ext.VpsRepFormatIdx != null && NalHeader?.NalUnitHeader != null)
            {
                var layerIdx = LayerIdxInVps == null ? 0 : LayerIdxInVps[NalHeader.NalUnitHeader.NuhLayerId];
                if (layerIdx < ext.VpsRepFormatIdx.Length)
                    idx = ext.VpsRepFormatIdx[layerIdx];
            }

            if (idx >= (ulong)repFormats.Length || repFormats[idx] == null)
                idx = 0;
            var rep = repFormats[idx];
            if (rep == null)
                return;

            sps.ChromaFormatIdc = rep.ChromaFormatVpsIdc;
            sps.SeparateColourPlaneFlag = rep.SeparateColourPlaneVpsFlag;
            sps.PicWidthInLumaSamples = rep.PicWidthVpsInLumaSamples;
            sps.PicHeightInLumaSamples = rep.PicHeightVpsInLumaSamples;
            sps.BitDepthLumaMinus8 = rep.BitDepthVpsLumaMinus8;
            sps.BitDepthChromaMinus8 = rep.BitDepthVpsChromaMinus8;
            sps.ConformanceWindowFlag = rep.ConformanceWindowVpsFlag;
            sps.ConfWinLeftOffset = rep.ConfWinVpsLeftOffset;
            sps.ConfWinRightOffset = rep.ConfWinVpsRightOffset;
            sps.ConfWinTopOffset = rep.ConfWinVpsTopOffset;
            sps.ConfWinBottomOffset = rep.ConfWinVpsBottomOffset;
        }

        /// <summary>Registers the picture parameter set being parsed, replacing one of the same id.</summary>
        public void SetPpsPicParameterSetId(ulong pps_pic_parameter_set_id)
        {
            if (PicParameterSetRbsp == null)
                return;

            PicParameterSets[PicParameterSetRbsp.PpsPicParameterSetId] = PicParameterSetRbsp;
        }
        
        /// <summary>
        /// Activates the picture parameter set a slice names, and the sequence parameter set that
        /// names, every time rather than only when the id changes: whatever was parsed since the
        /// last slice - another layer's sets, or a later set under the same id - the sets in force
        /// are the ones this slice names. Skipping it when the picture parameter set id repeated
        /// left the last sequence parameter set parsed in force.
        /// </summary>
        public void SetSlicePicParameterSetId(ulong slice_pic_parameter_set_id)
        {
            if (PicParameterSets.TryGetValue(slice_pic_parameter_set_id, out var pps))
                PicParameterSetRbsp = pps;
            else if (PicParameterSetRbsp == null || PicParameterSetRbsp.PpsPicParameterSetId != slice_pic_parameter_set_id)
                throw new Exception($"PicParameterSet with id {slice_pic_parameter_set_id} not found.");

            ulong spsId = PicParameterSetRbsp.PpsSeqParameterSetId;
            if (SeqParameterSets.TryGetValue(spsId, out var sps))
                SeqParameterSetRbsp = sps;
            else if (SeqParameterSetRbsp == null || SeqParameterSetRbsp.SpsSeqParameterSetId != spsId)
                throw new Exception($"SeqParameterSet with id {spsId} not found.");

            ResolveMultiLayerRepFormat(SeqParameterSetRbsp);
            DerivePictureSizes(SeqParameterSetRbsp);
            ActiveSeqParameterSets[NalHeader?.NalUnitHeader?.NuhLayerId ?? 0] = SeqParameterSetRbsp;
        }
    }
}
