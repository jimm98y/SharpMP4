
using System;
using System.Linq;

namespace SharpH264
{
    public class FrameTypes
    {
        // mod 5
        public const uint P = 0; // 5
        public const uint B = 1; // 6
        public const uint I = 2; // 7
        public const uint SP = 3; // 8
        public const uint SI = 4; // 9

        public static bool IsP(uint value) { return value % 5 == P; }
        public static bool IsB(uint value) { return value % 5 == B; }
        public static bool IsI(uint value) { return value % 5 == I; }
        public static bool IsSP(uint value) { return value % 5 == SP; }
        public static bool IsSI(uint value) { return value % 5 == SI; }
    }

    public class H264Constants
    {
        public const uint Extended_ISO = 255;
        public const uint Extended_SAR = 255;
    }

    public class H264Helpers
    {
        private static NalUnit _nalHeader;
        private static SeqParameterSetRbsp _sps;
        private static PicParameterSetRbsp _pps;
        private static SubsetSeqParameterSetRbsp _subsetSps;
        private static SeqParameterSetExtensionRbsp _spsExtension;
        private static DepthParameterSetRbsp _dps;
        private static AccessUnitDelimiterRbsp _au;
        private static SeiRbsp _sei;

        public static void SetSeqParameterSet(SeqParameterSetRbsp sps)
        {
            _sps = sps;
        }
        
        public static void SetPicParameterSet(PicParameterSetRbsp pps)
        {
            _pps = pps;
        }

        public static void SetSubsetSeqParameterSet(SubsetSeqParameterSetRbsp ssps)
        {
            _subsetSps = ssps;
        }

        public static void SetDepthParameterSet(DepthParameterSetRbsp dpps)
        {
            _dps = dpps;
        }

        public static void SetNalUnit(NalUnit nal)
        {
            _nalHeader = nal;
        }

        public static void SetSei(SeiRbsp sei)
        {
            _sei = sei;
        }

        public static void SetSeqParameterSetExtension(SeqParameterSetExtensionRbsp spsex)
        {
            _spsExtension = spsex;
        }

        public static void SetAccessUnitDelimiter(AccessUnitDelimiterRbsp au)
        {
            _au = au;
        }

        public static uint GetValue(string field)
        {
            switch (field)
            {
                case "AllViewsPairedFlag":
                    {
                        // The variable AllViewsPairedFlag is derived as follows:
                        // AllViewsPairedFlag = 1 for( i = 1; i <= num_views_minus1; i++ ) AllViewsPairedFlag = ( AllViewsPairedFlag && depth_view_present_flag[ i ] && texture_view_present_flag[ i ] )
                        uint allViewsPairedFlag = 1;
                        for (int i = 1; i <= GetValue("num_views_minus1"); i++)
                            allViewsPairedFlag = (uint)((allViewsPairedFlag != 0 && GetArray("depth_view_present_flag")[i] != 0 && GetArray("texture_view_present_flag")[i] != 0) ? 1 : 0);
                        return allViewsPairedFlag;
                    }
                case "ChromaArrayType":
                    // Depending on the value of separate_colour_plane_flag, the value of the variable ChromaArrayType is assigned as follows:
                    // – If separate_colour_plane_flag is equal to 0, ChromaArrayType is set equal to chroma_format_idc.
                    //– Otherwise (separate_colour_plane_flag is equal to 1), ChromaArrayType is set equal to 0. */
                    return _sps.SeqParameterSetData.SeparateColourPlaneFlag == 0 ? _sps.SeqParameterSetData.ChromaFormatIdc : 0;
                case "IdrPicFlag":
                    // IdrPicFlag = ( ( nal_unit_type  ==  5 )  ?  1  :  0 ) 
                    return (uint)((_nalHeader.NalUnitType == 5) ? 1 : 0);                    
                case "NalHrdBpPresentFlag":
                    return _sps.SeqParameterSetData.VuiParameters.NalHrdParametersPresentFlag;                
                case "VclHrdBpPresentFlag":
                    return _sps.SeqParameterSetData.VuiParameters.VclHrdParametersPresentFlag;
                case "cpb_cnt_minus1":
                    return _sps.SeqParameterSetData.VuiParameters.HrdParameters.CpbCntMinus1;
                case "initial_cpb_removal_delay_length_minus1":
                    return _sps.SeqParameterSetData.VuiParameters.HrdParameters.InitialCpbRemovalDelayLengthMinus1;
                case "profile_idc":
                    return _sps.SeqParameterSetData.ProfileIdc;
                case "chroma_format_idc":
                    return _sps.SeqParameterSetData.ChromaFormatIdc;
                case "CpbDpbDelaysPresentFlag":
                    return (uint)(_sps.SeqParameterSetData.VuiParameters.NalHrdParametersPresentFlag == 1 || _sps.SeqParameterSetData.VuiParameters.VclHrdParametersPresentFlag == 1 ? 1 : 0);
                case "pic_struct_present_flag":
                    return _sps.SeqParameterSetData.VuiParameters.PicStructPresentFlag;
                case "NumClockTS":
                    switch(_sei.SeiMessage.SeiPayload.PicTiming.PicStruct)
                    {
                        case 0:
                        case 1:
                        case 2:
                            return 1;
                        case 3:
                        case 4:
                            return 2;
                        case 5:
                        case 6:
                            return 3;
                        case 7:
                            return 2;
                        case 8:
                            return 3;
                        default:
                            throw new NotSupportedException();
                    }
                case "NumDepthViews":
                    return _subsetSps.SeqParameterSetMvcdExtension.NumDepthViews;
                case "PicSizeInMapUnits":
                    uint picWidthInMbs = _sps.SeqParameterSetData.PicWidthInMbsMinus1 + 1;
                    uint picHeightInMapUnits = _sps.SeqParameterSetData.PicHeightInMapUnitsMinus1 + 1;
                    return picWidthInMbs * picHeightInMapUnits;
                case "time_offset_length":
                    return _sps.SeqParameterSetData.VuiParameters.HrdParameters.TimeOffsetLength;
                case "frame_mbs_only_flag":
                    return _sps.SeqParameterSetData.FrameMbsOnlyFlag;
                case "num_slice_groups_minus1":
                    return _pps.NumSliceGroupsMinus1;
                case "num_views_minus1":
                    return _subsetSps.SeqParameterSetMvcExtension.NumViewsMinus1;
                case "anchor_pic_flag":
                    return _nalHeader.NalUnitHeaderMvcExtension.AnchorPicFlag;
                case "ref_dps_id0":
                    return _dps.RefDpsId0;
                case "predWeight0":
                    return _dps.PredWeight0;
                case "deltaFlag":
                    return 0; // TODO: unknown
                case "ref_dps_id1":
                    return _dps.RefDpsId1;

                // variable bit values
                case "bit_depth_aux_minus8":
                    return _spsExtension.BitDepthAuxMinus8;
                case "cpb_removal_delay_length_minus1":
                    return _sps.SeqParameterSetData.VuiParameters.HrdParameters.CpbRemovalDelayLengthMinus1;
                case "dpb_output_delay_length_minus1":
                    return _sps.SeqParameterSetData.VuiParameters.HrdParameters.DpbOutputDelayLengthMinus1;
                case "coded_data_bit_depth":
                    return _sei.SeiMessage.SeiPayload.ToneMappingInfo.CodedDataBitDepth;
                case "colour_remap_input_bit_depth":
                    return _sei.SeiMessage.SeiPayload.ColourRemappingInfo.ColourRemapInputBitDepth;
                case "colour_remap_output_bit_depth":
                    return _sei.SeiMessage.SeiPayload.ColourRemappingInfo.ColourRemapOutputBitDepth;
                case "ar_object_confidence_length_minus1":
                    return _sei.SeiMessage.SeiPayload.AnnotatedRegions.ArObjectConfidenceLengthMinus1;
            }

            throw new NotImplementedException();
        }

        public static uint[] GetArray(string field)
        {
            switch(field)
            {
                case "num_anchor_refs_l0":
                    return _subsetSps.SeqParameterSetMvcExtension.NumAnchorRefsL0;
                case "num_anchor_refs_l1":
                    return _subsetSps.SeqParameterSetMvcExtension.NumAnchorRefsL1;
                case "num_non_anchor_refs_l0":
                    return _subsetSps.SeqParameterSetMvcExtension.NumNonAnchorRefsL0;
                case "num_non_anchor_refs_l1":
                    return _subsetSps.SeqParameterSetMvcExtension.NumNonAnchorRefsL1;
                case "num_init_pic_parameter_set_minus1":
                    return _sei.SeiMessage.SeiPayload.MvcdViewScalabilityInfo.NumPicParameterSetMinus1; // looks like there is a typo...
                case "additional_shift_present":
                    return _sei.SeiMessage.SeiPayload.ThreeDimensionalReferenceDisplaysInfo.AdditionalShiftPresentFlag.Select(x => (uint)x).ToArray(); // TODO: looks like a typo
                case "texture_view_present_flag":
                    return _subsetSps.SeqParameterSetMvcdExtension.TextureViewPresentFlag.Select(x => (uint)x).ToArray();
                default:
                    throw new NotImplementedException();
            }
        }

        // TODO: SeiMessage support for multiple messages
        public static uint GetVariableCount(string field)
        {
            switch (field)
            {
                case "initial_cpb_removal_delay":
                case "initial_cpb_removal_delay_offset":
                    return GetValue("initial_cpb_removal_delay_length_minus1") + 1;
                case "alpha_opaque_value":
                case "alpha_transparent_value":
                    return GetValue("bit_depth_aux_minus8") + 9;
                case "slice_group_id":
                    return (uint)Math.Ceiling(Math.Log2(GetValue("num_slice_groups_minus1") + 1));
                case "cpb_removal_delay":
                    return GetValue("cpb_removal_delay_length_minus1") + 1;
                case "dpb_output_delay":
                    return GetValue("dpb_output_delay_length_minus1") + 1;
                case "time_offset":
                    return GetValue("time_offset_length");
                case "start_of_coded_interval":
                case "coded_pivot_value":
                case "target_pivot_value":
                    return ((GetValue("coded_data_bit_depth") + 7) >> 3) << 3;
                case "pre_lut_coded_value":
                case "pre_lut_target_value":
                    return ((GetValue("colour_remap_input_bit_depth") + 7) >> 3) << 3;
                case "post_lut_coded_value":
                case "post_lut_target_value":
                    return ((GetValue("colour_remap_output_bit_depth") + 7) >> 3) << 3;
                case "ar_object_confidence":
                    return GetValue("ar_object_confidence_length_minus1") + 1;
                case "mantissa_focal_length_x":
                    /*
                    The length of the mantissa_focal_length_x[ i ] syntax element in units of bits is variable and determined as follows: 
                    – If exponent_focal_length_x[ i ] == 0, the length is Max( 0, prec_focal_length − 30 ). 
                    – Otherwise (0 < exponent_focal_length_x[ i ] < 63), the length is Max( 0, exponent_focal_length_x[ i ] + prec_focal_length − 31 ).
                     */
                    throw new NotSupportedException();
                case "mantissa_focal_length_y":
                    throw new NotSupportedException();
                case "mantissa_principal_point_x":
                    throw new NotSupportedException();
                case "mantissa_principal_point_y":
                    throw new NotSupportedException();
                case "mantissa_skew_factor":
                    throw new NotSupportedException();
                case "mantissa_r":
                    throw new NotSupportedException();
                case "mantissa_t":
                    throw new NotSupportedException();
                case "da_mantissa":
                    throw new NotSupportedException();
                case "mantissa_ref_baseline":
                    throw new NotSupportedException();
                case "mantissa_ref_display_width":
                    throw new NotSupportedException();
                case "mantissa_ref_viewing_distance":
                    throw new NotSupportedException();
                case "depth_disp_delay_offset_fp":
                    throw new NotSupportedException();
                case "man_gvd_z_near":
                    throw new NotSupportedException();
                case "man_gvd_z_far":
                    throw new NotSupportedException();
                case "man_gvd_focal_length_x":
                    throw new NotSupportedException();
                case "man_gvd_focal_length_y":
                    throw new NotSupportedException();
                case "man_gvd_principal_point_x":
                    throw new NotSupportedException();
                case "man_gvd_principal_point_y":
                    throw new NotSupportedException();
                case "man_gvd_r":
                    throw new NotSupportedException();
                case "man_gvd_t_x":
                    throw new NotSupportedException();
                case "exponent0":
                    throw new NotSupportedException();
                case "mantissa0":
                    throw new NotSupportedException();
                case "exponent1":
                    throw new NotSupportedException();
                default:
                    throw new NotImplementedException();
            }
        }

        public static uint[,] GetArray2(string field)
        {
            switch(field)
            {
                case "ZNearSign":
                    throw new NotImplementedException();
                case "ZNearExp":
                    throw new NotImplementedException();
                case "ZNearMantissa":
                    throw new NotImplementedException();
                case "ZNearManLen":
                    throw new NotImplementedException();
                case "ZFarSign":
                    throw new NotImplementedException();
                case "ZFarExp":
                    throw new NotImplementedException();
                case "ZFarMantissa":
                    throw new NotImplementedException();
                case "ZFarManLen":
                    throw new NotImplementedException();
                case "DMinSign":
                    throw new NotImplementedException();
                case "DMinExp":
                    throw new NotImplementedException();
                case "DMinMantissa":
                    throw new NotImplementedException();
                case "DMinManLen":
                    throw new NotImplementedException();
                case "DMaxSign":
                    throw new NotImplementedException();
                case "DMaxExp":
                    throw new NotImplementedException();
                case "DMaxMantissa":
                    throw new NotImplementedException();
                case "DMaxManLen":
                    throw new NotImplementedException();

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
