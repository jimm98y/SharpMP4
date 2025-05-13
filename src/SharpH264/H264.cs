using System;

namespace SharpH264
{
    public class H264Constants
    {
        public const uint Extended_ISO = 255;
        public const uint Extended_SAR = 255;
    }

    public class H264FrameTypes
    {
        public const uint P = 0; // 5
        public const uint B = 1; // 6
        public const uint I = 2; // 7
        public const uint SP = 3; // 8
        public const uint SI = 4; // 9

        // mod 5
        public static bool IsP(ulong value) { return value % 5 == P; }
        public static bool IsB(ulong value) { return value % 5 == B; }
        public static bool IsI(ulong value) { return value % 5 == I; }
        public static bool IsSP(ulong value) { return value % 5 == SP; }
        public static bool IsSI(ulong value) { return value % 5 == SI; }
    }

    public class H264NALTypes
    {
        public const uint UNSPECIFIED0 = 0;                 // Unspecified
        public const uint SLICE = 1;                        // Slice of non-IDR picture
        public const uint DPA = 2;                          // Slice data partition A 
        public const uint DPB = 3;                          // Slice data partition B
        public const uint DPC = 4;                          // Slice data partition C
        public const uint IDR_SLICE = 5;                    // Immediate decoder refresh (IDR) slice
        public const uint SEI = 6;                          // Supplemental enhancement information (SEI)
        public const uint SPS = 7;                          // Sequence parameter set
        public const uint PPS = 8;                          // Picture parameter set
        public const uint AUD = 9;                          // Access unit delimiter
        public const uint END_OF_SEQUENCE = 10;             // End of sequence
        public const uint END_OF_STREAM = 11;               // End of stream
        public const uint FILLER_DATA = 12;                 // Filler data
        public const uint SPS_EXT = 13;                     // SPS extension
        public const uint PREFIX_NAL = 14;                  // Prefix NAL unit
        public const uint SUBSET_SPS = 15;                  // Subset sequence parameter set
        public const uint DPS = 16;                         // Depth parameter set
        public const uint RESERVED0 = 17;
        public const uint RESERVED1 = 18;
        public const uint SLICE_NOPARTITIONING = 19;        // Slice of an auxiliary coded picture without partitioning
        public const uint SLICE_EXT = 20;                   // Slice extension
        public const uint SLICE_EXT_VIEW_COMPONENT = 21;    // Slice extension for depth view component or 3D-AVC texture view component
        public const uint RESERVED2 = 22;
        public const uint RESERVED3 = 23;
        public const uint UNSPECIFIED1 = 23;
        public const uint UNSPECIFIED2 = 24;
        public const uint UNSPECIFIED3 = 25;
        public const uint UNSPECIFIED4 = 26;
        public const uint UNSPECIFIED5 = 27;
        public const uint UNSPECIFIED6 = 28;
        public const uint UNSPECIFIED7 = 28;
        public const uint UNSPECIFIED8 = 30;
        public const uint UNSPECIFIED9 = 31;
    }

    public partial class H264Context
    {
        public SeiPayload SeiPayload { get; set; }

        public int NumClockTS { get; set; }
        public uint AllViewsPairedFlag { get; set; } = 1;
        public ulong ChromaArrayType { get; set; } = 1; // default value of chroma_format_idc when not present
        public uint IdrPicFlag { get; private set; }
        public int DepthFlag { get; private set; }
        public uint CpbDpbDelaysPresentFlag { get; private set; }
        public ulong PicHeightInMapUnits { get; private set; }
        public ulong PicSizeInMapUnits { get; private set; }
        public ulong PicWidthInMbs { get; private set; }
        public int deltaFlag { get; private set; }

        // TODO: artificially added because there are many different slice_header types - unify?
        public uint TimeOffsetLength { get; set; } = 24;
        public ulong SliceType { get; set; }
        public ulong NumRefIdxL1ActiveMinus1 { get; set; }
        public ulong NumRefIdxL0ActiveMinus1 { get; set; }

        public void SetSeiPayload(SeiPayload payload)
        {
            if (SeiPayload == null)
            {
                SeiPayload = payload;
            }

            if (payload.AlternativeDepthInfo != null)
                SeiPayload.AlternativeDepthInfo = payload.AlternativeDepthInfo;
            if (payload.AlternativeTransferCharacteristics != null)
                SeiPayload.AlternativeTransferCharacteristics = payload.AlternativeTransferCharacteristics;
            if (payload.AmbientViewingEnvironment != null)
                SeiPayload.AmbientViewingEnvironment = payload.AmbientViewingEnvironment;
            if (payload.AnnotatedRegions != null)
                SeiPayload.AnnotatedRegions = payload.AnnotatedRegions;
            if (payload.BaseLayerTemporalHrd != null)
                SeiPayload.BaseLayerTemporalHrd = payload.BaseLayerTemporalHrd;
            if (payload.BaseViewTemporalHrd != null)
                SeiPayload.BaseViewTemporalHrd = payload.BaseViewTemporalHrd;
            if (payload.BufferingPeriod != null)
                SeiPayload.BufferingPeriod = payload.BufferingPeriod;
            if (payload.ColourRemappingInfo != null)
                SeiPayload.ColourRemappingInfo = payload.ColourRemappingInfo;
            if (payload.ConstrainedDepthParameterSetIdentifier != null)
                SeiPayload.ConstrainedDepthParameterSetIdentifier = payload.ConstrainedDepthParameterSetIdentifier;
            if (payload.ContentColourVolume != null)
                SeiPayload.ContentColourVolume = payload.ContentColourVolume;
            if (payload.ContentLightLevelInfo != null)
                SeiPayload.ContentLightLevelInfo = payload.ContentLightLevelInfo;
            if (payload.CubemapProjection != null)
                SeiPayload.CubemapProjection = payload.CubemapProjection;
            if (payload.DeblockingFilterDisplayPreference != null)
                SeiPayload.DeblockingFilterDisplayPreference = payload.DeblockingFilterDisplayPreference;
            if (payload.DecRefPicMarkingRepetition != null)
                SeiPayload.DecRefPicMarkingRepetition = payload.DecRefPicMarkingRepetition;
            if (payload.DepthRepresentationInfo != null)
                SeiPayload.DepthRepresentationInfo = payload.DepthRepresentationInfo;
            if (payload.DepthSamplingInfo != null)
                SeiPayload.DepthSamplingInfo = payload.DepthSamplingInfo;
            if (payload.DepthTiming != null)
                SeiPayload.DepthTiming = payload.DepthTiming;
            if (payload.DisplayOrientation != null)
                SeiPayload.DisplayOrientation = payload.DisplayOrientation;
            if (payload.EquirectangularProjection != null)
                SeiPayload.EquirectangularProjection = payload.EquirectangularProjection;
            if (payload.FillerPayload != null)
                SeiPayload.FillerPayload = payload.FillerPayload;
            if (payload.FilmGrainCharacteristics != null)
                SeiPayload.FilmGrainCharacteristics = payload.FilmGrainCharacteristics;
            if (payload.FramePackingArrangement != null)
                SeiPayload.FramePackingArrangement = payload.FramePackingArrangement;
            if (payload.FullFrameFreeze != null)
                SeiPayload.FullFrameFreeze = payload.FullFrameFreeze;
            if (payload.FullFrameFreezeRelease != null)
                SeiPayload.FullFrameFreezeRelease = payload.FullFrameFreezeRelease;
            if (payload.FullFrameSnapshot != null)
                SeiPayload.FullFrameSnapshot = payload.FullFrameSnapshot;
            if (payload.GreenMetadata != null)
                SeiPayload.GreenMetadata = payload.GreenMetadata;
            if (payload.LayerDependencyChange != null)
                SeiPayload.LayerDependencyChange = payload.LayerDependencyChange;
            if (payload.LayersNotPresent != null)
                SeiPayload.LayersNotPresent = payload.LayersNotPresent;
            if (payload.MasteringDisplayColourVolume != null)
                SeiPayload.MasteringDisplayColourVolume = payload.MasteringDisplayColourVolume;
            if (payload.MotionConstrainedSliceGroupSet != null)
                SeiPayload.MotionConstrainedSliceGroupSet = payload.MotionConstrainedSliceGroupSet;
            if (payload.MultiviewAcquisitionInfo != null)
                SeiPayload.MultiviewAcquisitionInfo = payload.MultiviewAcquisitionInfo;
            if (payload.MultiviewSceneInfo != null)
                SeiPayload.MultiviewSceneInfo = payload.MultiviewSceneInfo;
            if (payload.MultiviewViewPosition != null)
                SeiPayload.MultiviewViewPosition = payload.MultiviewViewPosition;
            if (payload.MvcdScalableNesting != null)
                SeiPayload.MvcdScalableNesting = payload.MvcdScalableNesting;
            if (payload.MvcdViewScalabilityInfo != null)
                SeiPayload.MvcdViewScalabilityInfo = payload.MvcdViewScalabilityInfo;
            if (payload.MvcScalableNesting != null)
                SeiPayload.MvcScalableNesting = payload.MvcScalableNesting;
            if (payload.NonRequiredLayerRep != null)
                SeiPayload.NonRequiredLayerRep = payload.NonRequiredLayerRep;
            if (payload.NonRequiredViewComponent != null)
                SeiPayload.NonRequiredViewComponent = payload.NonRequiredViewComponent;
            if (payload.OmniViewport != null)
                SeiPayload.OmniViewport = payload.OmniViewport;
            if (payload.OperationPointNotPresent != null)
                SeiPayload.OperationPointNotPresent = payload.OperationPointNotPresent;
            if (payload.PanScanRect != null)
                SeiPayload.PanScanRect = payload.PanScanRect;
            if (payload.ParallelDecodingInfo != null)
                SeiPayload.ParallelDecodingInfo = payload.ParallelDecodingInfo;
            if (payload.PicTiming != null)
                SeiPayload.PicTiming = payload.PicTiming;
            if (payload.PostFilterHint != null)
                SeiPayload.PostFilterHint = payload.PostFilterHint;
            if (payload.PriorityLayerInfo != null)
                SeiPayload.PriorityLayerInfo = payload.PriorityLayerInfo;
            if (payload.ProgressiveRefinementSegmentEnd != null)
                SeiPayload.ProgressiveRefinementSegmentEnd = payload.ProgressiveRefinementSegmentEnd;
            if (payload.ProgressiveRefinementSegmentStart != null)
                SeiPayload.ProgressiveRefinementSegmentStart = payload.ProgressiveRefinementSegmentStart;
            if (payload.QualityLayerIntegrityCheck != null)
                SeiPayload.QualityLayerIntegrityCheck = payload.QualityLayerIntegrityCheck;
            if (payload.RecoveryPoint != null)
                SeiPayload.RecoveryPoint = payload.RecoveryPoint;
            if (payload.RedundantPicProperty != null)
                SeiPayload.RedundantPicProperty = payload.RedundantPicProperty;
            if (payload.RegionwisePacking != null)
                SeiPayload.RegionwisePacking = payload.RegionwisePacking;
            if (payload.ReservedSeiMessage != null)
                SeiPayload.ReservedSeiMessage = payload.ReservedSeiMessage;
            if (payload.ScalabilityInfo != null)
                SeiPayload.ScalabilityInfo = payload.ScalabilityInfo;
            if (payload.ScalableNesting != null)
                SeiPayload.ScalableNesting = payload.ScalableNesting;
            if (payload.SceneInfo != null)
                SeiPayload.SceneInfo = payload.SceneInfo;
            if (payload.SeiManifest != null)
                SeiPayload.SeiManifest = payload.SeiManifest;
            if (payload.SeiPrefixIndication != null)
                SeiPayload.SeiPrefixIndication = payload.SeiPrefixIndication;
            if (payload.ShutterIntervalInfo != null)
                SeiPayload.ShutterIntervalInfo = payload.ShutterIntervalInfo;
            if (payload.SparePic != null)
                SeiPayload.SparePic = payload.SparePic;
            if (payload.SphereRotation != null)
                SeiPayload.SphereRotation = payload.SphereRotation;
            if (payload.StereoVideoInfo != null)
                SeiPayload.StereoVideoInfo = payload.StereoVideoInfo;
            if (payload.SubPicScalableLayer != null)
                SeiPayload.SubPicScalableLayer = payload.SubPicScalableLayer;
            if (payload.SubSeqCharacteristics != null)
                SeiPayload.SubSeqCharacteristics = payload.SubSeqCharacteristics;
            if (payload.SubSeqInfo != null)
                SeiPayload.SubSeqInfo = payload.SubSeqInfo;
            if (payload.SubSeqLayerCharacteristics != null)
                SeiPayload.SubSeqLayerCharacteristics = payload.SubSeqLayerCharacteristics;
            if (payload.ThreeDimensionalReferenceDisplaysInfo != null)
                SeiPayload.ThreeDimensionalReferenceDisplaysInfo = payload.ThreeDimensionalReferenceDisplaysInfo;
            if (payload.Tl0DepRepIndex != null)
                SeiPayload.Tl0DepRepIndex = payload.Tl0DepRepIndex;
            if (payload.TlSwitchingPoint != null)
                SeiPayload.TlSwitchingPoint = payload.TlSwitchingPoint;
            if (payload.ToneMappingInfo != null)
                SeiPayload.ToneMappingInfo = payload.ToneMappingInfo;
            if (payload.UserDataRegisteredItutT35 != null)
                SeiPayload.UserDataRegisteredItutT35 = payload.UserDataRegisteredItutT35;
            if (payload.UserDataUnregistered != null)
                SeiPayload.UserDataUnregistered = payload.UserDataUnregistered;
            if (payload.ViewDependencyChange != null)
                SeiPayload.ViewDependencyChange = payload.ViewDependencyChange;
            if (payload.ViewScalabilityInfo != null)
                SeiPayload.ViewScalabilityInfo = payload.ViewScalabilityInfo;
        }

        public void OnPicStruct()
        {
            var pic_struct = SeiPayload.PicTiming.PicStruct;

            NumClockTS = pic_struct switch
            {
                0u => 1,
                1u => 1,
                2u => 1,
                3u => 2,
                4u => 2,
                5u => 3,
                6u => 3,
                7u => 2,
                8u => 3,
                _ => throw new NotSupportedException()
            };
        }

        public void OnEnableRleSkipFlag()
        {
            AllViewsPairedFlag = 1;
            for (int i = 1; i <= (int)SubsetSeqParameterSetRbsp.SeqParameterSetMvcExtension.NumViewsMinus1; i++)
                AllViewsPairedFlag = (uint)((AllViewsPairedFlag != 0 && SubsetSeqParameterSetRbsp.SeqParameterSetMvcdExtension.DepthViewPresentFlag[i] != 0 && SubsetSeqParameterSetRbsp.SeqParameterSetMvcdExtension.TextureViewPresentFlag[i] != 0) ? 1 : 0);
        }

        public void OnSeparateColourPlaneFlag()
        {
            var separate_colour_plane_flag = SeqParameterSetRbsp.SeqParameterSetData.SeparateColourPlaneFlag;
            var chroma_format_idc = SeqParameterSetRbsp.SeqParameterSetData.ChromaFormatIdc;

            if (separate_colour_plane_flag == 0)
                ChromaArrayType = chroma_format_idc;
            else
                ChromaArrayType = 0;
        }

        public void OnNalUnitType()
        {
            var nal_unit_type = NalHeader.NalUnitType;

            IdrPicFlag = (uint)((nal_unit_type == 5) ? 1 : 0);
        }

        public void OnAvc3dExtensionFlag()
        {
            var nal_unit_type = NalHeader.NalUnitType;
            var avc_3d_extension_flag = NalHeader.Avc3dExtensionFlag;
            var depth_flag = NalHeader.NalUnitHeader3davcExtension.DepthFlag;

            DepthFlag = (nal_unit_type != 21) ? 0 : (avc_3d_extension_flag != 0 ? depth_flag : 1);
        }

        public void OnVclHrdParametersPresentFlag()
        {
            var nal_hrd_parameters_present_flag = SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.NalHrdParametersPresentFlag;
            var vcl_hrd_parameters_present_flag = SeqParameterSetRbsp.SeqParameterSetData.VuiParameters.VclHrdParametersPresentFlag;
            CpbDpbDelaysPresentFlag = (uint)(nal_hrd_parameters_present_flag == 1 || vcl_hrd_parameters_present_flag == 1 ? 1 : 0);
        }

        public void OnPicWidthInMbsMinus1()
        {
            var pic_width_in_mbs_minus1 = SeqParameterSetRbsp.SeqParameterSetData.PicWidthInMbsMinus1;
            PicWidthInMbs = pic_width_in_mbs_minus1 + 1;
            //PicWidthInSamplesL = PicWidthInMbs * 16;
            //PicWidthInSamplesC = PicWidthInMbs * MbWidthC;
        }

        public void OnPicHeightInMapUnitsMinus1()
        {
            var pic_height_in_map_units_minus1 = SeqParameterSetRbsp.SeqParameterSetData.PicHeightInMapUnitsMinus1;
            PicHeightInMapUnits = pic_height_in_map_units_minus1 + 1;
            PicSizeInMapUnits = PicWidthInMbs * PicHeightInMapUnits;
        }

        public void OnTimeOffsetLength(uint time_offset_length)
        {
            TimeOffsetLength = time_offset_length;
        }
        
        public void OnSliceType(ulong slice_type)
        {
            SliceType = slice_type;
        }

        public void OnNumRefIdxL0ActiveMinus1(ulong num_ref_idx_l0_active_minus1)
        {
            NumRefIdxL0ActiveMinus1 = num_ref_idx_l0_active_minus1;
        }

        public void OnNumRefIdxL1ActiveMinus1(ulong num_ref_idx_l1_active_minus1)
        {
            NumRefIdxL1ActiveMinus1 = num_ref_idx_l1_active_minus1;
        }

        public void OnNumRefIdxActiveOverrideFlag(uint num_ref_idx_active_override_flag)
        {
            var num_ref_idx_l0_default_active_minus1 = PicParameterSetRbsp.NumRefIdxL0DefaultActiveMinus1;
            if (num_ref_idx_active_override_flag == 0)
            {
                NumRefIdxL0ActiveMinus1 = num_ref_idx_l0_default_active_minus1;
            }
        }

        public void OnPicTiming(PicTiming pic_timing)
        {
            SeiPayload.PicTiming = pic_timing;
        }

        public void OnChromaFormatIdc()
        {
            OnSeparateColourPlaneFlag();
        }
    }
}
