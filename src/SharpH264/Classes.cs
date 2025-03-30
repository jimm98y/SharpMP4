
using System;

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
        public static uint GetValue(string field)
        {
            //SeqParameterSetRbsp sps = null;

            switch (field)
            {
                case "AllViewsPairedFlag":
                    // The variable AllViewsPairedFlag is derived as follows:
                    // AllViewsPairedFlag = 1 for( i = 1; i <= num_views_minus1; i++ ) AllViewsPairedFlag = ( AllViewsPairedFlag && depth_view_present_flag[ i ] && (J-9) texture_view_present_flag[ i ] )
                    return 1;

                case "ChromaArrayType":
                    // Depending on the value of separate_colour_plane_flag, the value of the variable ChromaArrayType is assigned as follows:
                    // – If separate_colour_plane_flag is equal to 0, ChromaArrayType is set equal to chroma_format_idc.
                    //– Otherwise (separate_colour_plane_flag is equal to 1), ChromaArrayType is set equal to 0. */
                    return 1;

                case "IdrPicFlag":
                    return 1;

                case "NalHrdBpPresentFlag":
                    // TODO from vui: nal_hrd_parameters_present_flag
                    //return sps.SeqParameterSetData.VuiParameters.NalHrdParametersPresentFlag;
                    return 1;
                
                case "VclHrdBpPresentFlag":
                    // TODO from vui: vcl_hrd_parameters_present_flag
                    //return sps.SeqParameterSetData.VuiParameters.VclHrdParametersPresentFlag;
                    return 0;

                case "cpb_cnt_minus1":
                    // TODO from vui: cpb_cnt_minus1
                    return 0;

                case "initial_cpb_removal_delay_length_minus1":
                    return 23;

                case "profile_idc":
                    throw new NotSupportedException();
                case "chroma_format_idc":
                    throw new NotSupportedException();
                case "CpbDpbDelaysPresentFlag":
                    throw new NotSupportedException();
                case "pic_struct_present_flag":
                    throw new NotSupportedException();
                case "NumClockTS":
                    throw new NotSupportedException();
                case "time_offset_length":
                    throw new NotSupportedException();
                case "frame_mbs_only_flag":
                    throw new NotSupportedException();
                case "PicSizeInMapUnits":
                    throw new NotSupportedException();
                case "num_slice_groups_minus1":
                    throw new NotSupportedException();
                case "num_views_minus1":
                    throw new NotSupportedException();
                case "anchor_pic_flag":
                    throw new NotSupportedException();
                case "NumDepthViews":
                    throw new NotSupportedException();
                case "ref_dps_id0":
                    throw new NotSupportedException();
                case "predWeight0":
                    throw new NotSupportedException();
                case "deltaFlag":
                    throw new NotSupportedException();
                case "ref_dps_id1":
                    throw new NotSupportedException();
            }

            throw new NotImplementedException();
        }
        public static uint[] GetArray(string field)
        {
            switch(field)
            {
                case "num_anchor_refs_l0":
                    throw new NotSupportedException();
                case "num_anchor_refs_l1":
                    throw new NotSupportedException();
                case "num_non_anchor_refs_l0":
                    throw new NotSupportedException();
                case "num_non_anchor_refs_l1":
                    throw new NotSupportedException();
                case "num_init_pic_parameter_set_minus1":
                    throw new NotSupportedException();
                case "additional_shift_present":
                    throw new NotSupportedException();
                case "texture_view_present_flag":
                    throw new NotSupportedException();
                default:
                    throw new NotImplementedException();
            }
        }

        public static uint GetVariableCount(string field)
        {
            switch (field)
            {
                case "initial_cpb_removal_delay":
                case "initial_cpb_removal_delay_offset":
                    return GetValue("initial_cpb_removal_delay_length_minus1") + 1;

                case "alpha_opaque_value":
                    throw new NotSupportedException();
                case "alpha_transparent_value":
                    throw new NotSupportedException();
                case "slice_group_id":
                    throw new NotSupportedException();
                case "cpb_removal_delay":
                    throw new NotSupportedException();
                case "dpb_output_delay":
                    throw new NotSupportedException();
                case "time_offset":
                    throw new NotSupportedException();
                case "start_of_coded_interval":
                    throw new NotSupportedException();
                case "coded_pivot_value":
                    throw new NotSupportedException();
                case "target_pivot_value":
                    throw new NotSupportedException();
                case "pre_lut_coded_value":
                    throw new NotSupportedException();
                case "pre_lut_target_value":
                    throw new NotSupportedException();
                case "post_lut_coded_value":
                    throw new NotSupportedException();
                case "post_lut_target_value":
                    throw new NotSupportedException();
                case "ar_object_confidence":
                    throw new NotSupportedException();
                case "mantissa_focal_length_x":
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
            throw new NotImplementedException();
        }
    }
}
