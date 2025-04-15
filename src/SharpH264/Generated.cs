using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace SharpH264
{

    /*
nal_unit( NumBytesInNALunit ) { 
 forbidden_zero_bit All f(1) 
 nal_ref_idc All u(2) 
 nal_unit_type All u(5) 
 /* NumBytesInRBSP = 0 *//*
 nalUnitHeaderBytes = 1   
 if( nal_unit_type  ==  14  ||  nal_unit_type  ==  20  ||  nal_unit_type  ==  21 ) { 
  
  if( nal_unit_type !=  21 )   
   svc_extension_flag All u(1) 
  else   
   avc_3d_extension_flag All u(1) 
  if( svc_extension_flag ) {   
   nal_unit_header_svc_extension() /* specified in Annex G *//* All  
   nalUnitHeaderBytes += 3   
  } else if( avc_3d_extension_flag ) {   
   nal_unit_header_3davc_extension() /* specified in Annex J *//*   
   nalUnitHeaderBytes += 2   
  } else {   
   nal_unit_header_mvc_extension() /* specified in Annex H *//* All  
   nalUnitHeaderBytes += 3   
  }   
 }   
 /*for( i = nalUnitHeaderBytes; i < NumBytesInNALunit; i++ ) {   
  if( i + 2 < NumBytesInNALunit && next_bits( 24 )  ==  0x000003 ) {   
   rbsp_byte[ NumBytesInRBSP++ ] All b(8) 
   rbsp_byte[ NumBytesInRBSP++ ] All b(8) 
   i += 2   
   emulation_prevention_three_byte  *//*/* equal to 0x03 *//*/* All f(8) 
  } else   
   rbsp_byte[ NumBytesInRBSP++ ] All b(8) 
 }*//* 
}
    */
    public class NalUnit : IItuSerializable
    {
        private uint numBytesInNALunit;
        public uint NumBytesInNALunit { get { return numBytesInNALunit; } set { numBytesInNALunit = value; } }
        private uint forbidden_zero_bit;
        public uint ForbiddenZeroBit { get { return forbidden_zero_bit; } set { forbidden_zero_bit = value; } }
        private uint nal_ref_idc;
        public uint NalRefIdc { get { return nal_ref_idc; } set { nal_ref_idc = value; } }
        private uint nal_unit_type;
        public uint NalUnitType { get { return nal_unit_type; } set { nal_unit_type = value; } }
        private byte svc_extension_flag;
        public byte SvcExtensionFlag { get { return svc_extension_flag; } set { svc_extension_flag = value; } }
        private byte avc_3d_extension_flag;
        public byte Avc3dExtensionFlag { get { return avc_3d_extension_flag; } set { avc_3d_extension_flag = value; } }
        private NalUnitHeaderSvcExtension nal_unit_header_svc_extension;
        public NalUnitHeaderSvcExtension NalUnitHeaderSvcExtension { get { return nal_unit_header_svc_extension; } set { nal_unit_header_svc_extension = value; } }
        private NalUnitHeader3davcExtension nal_unit_header_3davc_extension;
        public NalUnitHeader3davcExtension NalUnitHeader3davcExtension { get { return nal_unit_header_3davc_extension; } set { nal_unit_header_3davc_extension = value; } }
        private NalUnitHeaderMvcExtension nal_unit_header_mvc_extension;
        public NalUnitHeaderMvcExtension NalUnitHeaderMvcExtension { get { return nal_unit_header_mvc_extension; } set { nal_unit_header_mvc_extension = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public NalUnit(uint NumBytesInNALunit)
        {
            this.numBytesInNALunit = NumBytesInNALunit;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint nalUnitHeaderBytes = 0;
            size += stream.ReadFixed(size, 1, out this.forbidden_zero_bit);
            size += stream.ReadUnsignedInt(size, 2, out this.nal_ref_idc);
            size += stream.ReadUnsignedInt(size, 5, out this.nal_unit_type);
            /*  NumBytesInRBSP = 0  */

            nalUnitHeaderBytes = 1;

            if (nal_unit_type == 14 || nal_unit_type == 20 || nal_unit_type == 21)
            {

                if (nal_unit_type != 21)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.svc_extension_flag);
                }
                else
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.avc_3d_extension_flag);
                }

                if (svc_extension_flag != 0)
                {
                    size += stream.ReadClass<NalUnitHeaderSvcExtension>(size, context, () => new NalUnitHeaderSvcExtension(), out this.nal_unit_header_svc_extension); // specified in Annex G 
                    nalUnitHeaderBytes += 3;
                }
                else if (avc_3d_extension_flag != 0)
                {
                    size += stream.ReadClass<NalUnitHeader3davcExtension>(size, context, () => new NalUnitHeader3davcExtension(), out this.nal_unit_header_3davc_extension); // specified in Annex J 
                    nalUnitHeaderBytes += 2;
                }
                else
                {
                    size += stream.ReadClass<NalUnitHeaderMvcExtension>(size, context, () => new NalUnitHeaderMvcExtension(), out this.nal_unit_header_mvc_extension); // specified in Annex H 
                    nalUnitHeaderBytes += 3;
                }
            }
            /* for( i = nalUnitHeaderBytes; i < NumBytesInNALunit; i++ ) {   
              if( i + 2 < NumBytesInNALunit && next_bits( 24 )  ==  0x000003 ) {   
               rbsp_byte[ NumBytesInRBSP++ ] All b(8) 
               rbsp_byte[ NumBytesInRBSP++ ] All b(8) 
               i += 2   
               emulation_prevention_three_byte   */

            /*  equal to 0x03  */

            /*  All f(8) 
              } else   
               rbsp_byte[ NumBytesInRBSP++ ] All b(8) 
             } */


            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint nalUnitHeaderBytes = 0;
            size += stream.WriteFixed(1, this.forbidden_zero_bit);
            size += stream.WriteUnsignedInt(2, this.nal_ref_idc);
            size += stream.WriteUnsignedInt(5, this.nal_unit_type);
            /*  NumBytesInRBSP = 0  */

            nalUnitHeaderBytes = 1;

            if (nal_unit_type == 14 || nal_unit_type == 20 || nal_unit_type == 21)
            {

                if (nal_unit_type != 21)
                {
                    size += stream.WriteUnsignedInt(1, this.svc_extension_flag);
                }
                else
                {
                    size += stream.WriteUnsignedInt(1, this.avc_3d_extension_flag);
                }

                if (svc_extension_flag != 0)
                {
                    size += stream.WriteClass<NalUnitHeaderSvcExtension>(context, this.nal_unit_header_svc_extension); // specified in Annex G 
                    nalUnitHeaderBytes += 3;
                }
                else if (avc_3d_extension_flag != 0)
                {
                    size += stream.WriteClass<NalUnitHeader3davcExtension>(context, this.nal_unit_header_3davc_extension); // specified in Annex J 
                    nalUnitHeaderBytes += 2;
                }
                else
                {
                    size += stream.WriteClass<NalUnitHeaderMvcExtension>(context, this.nal_unit_header_mvc_extension); // specified in Annex H 
                    nalUnitHeaderBytes += 3;
                }
            }
            /* for( i = nalUnitHeaderBytes; i < NumBytesInNALunit; i++ ) {   
              if( i + 2 < NumBytesInNALunit && next_bits( 24 )  ==  0x000003 ) {   
               rbsp_byte[ NumBytesInRBSP++ ] All b(8) 
               rbsp_byte[ NumBytesInRBSP++ ] All b(8) 
               i += 2   
               emulation_prevention_three_byte   */

            /*  equal to 0x03  */

            /*  All f(8) 
              } else   
               rbsp_byte[ NumBytesInRBSP++ ] All b(8) 
             } */


            return size;
        }

    }

    /*
  

seq_parameter_set_rbsp() { 
 seq_parameter_set_data() 0  
 rbsp_trailing_bits() 0  
}
    */
    public class SeqParameterSetRbsp : IItuSerializable
    {
        private SeqParameterSetData seq_parameter_set_data;
        public SeqParameterSetData SeqParameterSetData { get { return seq_parameter_set_data; } set { seq_parameter_set_data = value; } }
        private RbspTrailingBits rbsp_trailing_bits;
        public RbspTrailingBits RbspTrailingBits { get { return rbsp_trailing_bits; } set { rbsp_trailing_bits = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SeqParameterSetRbsp()
        {

        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadClass<SeqParameterSetData>(size, context, () => new SeqParameterSetData(), out this.seq_parameter_set_data);
            size += stream.ReadClass<RbspTrailingBits>(size, context, () => new RbspTrailingBits(), out this.rbsp_trailing_bits);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteClass<SeqParameterSetData>(context, this.seq_parameter_set_data);
            size += stream.WriteClass<RbspTrailingBits>(context, this.rbsp_trailing_bits);

            return size;
        }

    }

    /*
 

seq_parameter_set_data() { 
 profile_idc 0 u(8) 
 constraint_set0_flag 0 u(1) 
 constraint_set1_flag 0 u(1) 
 constraint_set2_flag 0 u(1) 
 constraint_set3_flag 0 u(1) 
 constraint_set4_flag 0 u(1) 
 constraint_set5_flag 0 u(1) 
 reserved_zero_2bits  /* equal to 0 *//* 0 u(2) 
 level_idc 0 u(8) 
 seq_parameter_set_id 0 ue(v) 
 if( profile_idc  ==  100  ||  profile_idc  ==  110  || 
  profile_idc  ==  122  ||  profile_idc  ==  244  ||  profile_idc  ==  44  || 
  profile_idc  ==  83  ||  profile_idc  ==  86  ||  profile_idc  ==  118  || 
  profile_idc  ==  128  ||  profile_idc  ==  138  ||  profile_idc  ==  139  || 
  profile_idc  ==  134  ||  profile_idc  ==  135 ) { 
  
  chroma_format_idc 0 ue(v) 
  if( chroma_format_idc  ==  3 )   
   separate_colour_plane_flag 0 u(1) 
  bit_depth_luma_minus8 0 ue(v) 
  bit_depth_chroma_minus8 0 ue(v) 
  qpprime_y_zero_transform_bypass_flag 0 u(1) 
  seq_scaling_matrix_present_flag 0 u(1) 
  if( seq_scaling_matrix_present_flag )   
   for( i = 0; i < ( ( chroma_format_idc  !=  3 ) ? 8 : 12 ); i++ ) {   
    seq_scaling_list_present_flag[ i ] 0 u(1) 
    if( seq_scaling_list_present_flag[ i ] )   
     if( i < 6 )    
      scaling_list( ScalingList4x4[ i ], 16, UseDefaultScalingMatrix4x4Flag[ i ] ) 0  
     else   
      scaling_list( ScalingList8x8[ i - 6 ], 64, UseDefaultScalingMatrix8x8Flag[ i - 6 ] ) 0  
   }   
 }   
 
 log2_max_frame_num_minus4 0 ue(v) 
 pic_order_cnt_type 0 ue(v) 
 if( pic_order_cnt_type  ==  0 )   
  log2_max_pic_order_cnt_lsb_minus4 0 ue(v) 
 else if( pic_order_cnt_type  ==  1 ) {   
  delta_pic_order_always_zero_flag 0 u(1) 
  offset_for_non_ref_pic 0 se(v) 
  offset_for_top_to_bottom_field 0 se(v) 
  num_ref_frames_in_pic_order_cnt_cycle 0 ue(v) 
  for( i = 0; i < num_ref_frames_in_pic_order_cnt_cycle; i++ )   
   offset_for_ref_frame[ i ] 0 se(v) 
 }   
 max_num_ref_frames 0 ue(v) 
 gaps_in_frame_num_value_allowed_flag 0 u(1) 
 pic_width_in_mbs_minus1 0 ue(v) 
 pic_height_in_map_units_minus1 0 ue(v) 
 frame_mbs_only_flag 0 u(1) 
 if( !frame_mbs_only_flag )   
  mb_adaptive_frame_field_flag 0 u(1) 
 direct_8x8_inference_flag 0 u(1) 
 frame_cropping_flag 0 u(1) 
 if( frame_cropping_flag ) {   
  frame_crop_left_offset 0 ue(v) 
  frame_crop_right_offset 0 ue(v) 
  frame_crop_top_offset 0 ue(v) 
  frame_crop_bottom_offset 0 ue(v) 
 }   
 vui_parameters_present_flag 0 u(1) 
 if( vui_parameters_present_flag )   
  vui_parameters() 0  
}
    */
    public class SeqParameterSetData : IItuSerializable
    {
        private uint profile_idc;
        public uint ProfileIdc { get { return profile_idc; } set { profile_idc = value; } }
        private byte constraint_set0_flag;
        public byte ConstraintSet0Flag { get { return constraint_set0_flag; } set { constraint_set0_flag = value; } }
        private byte constraint_set1_flag;
        public byte ConstraintSet1Flag { get { return constraint_set1_flag; } set { constraint_set1_flag = value; } }
        private byte constraint_set2_flag;
        public byte ConstraintSet2Flag { get { return constraint_set2_flag; } set { constraint_set2_flag = value; } }
        private byte constraint_set3_flag;
        public byte ConstraintSet3Flag { get { return constraint_set3_flag; } set { constraint_set3_flag = value; } }
        private byte constraint_set4_flag;
        public byte ConstraintSet4Flag { get { return constraint_set4_flag; } set { constraint_set4_flag = value; } }
        private byte constraint_set5_flag;
        public byte ConstraintSet5Flag { get { return constraint_set5_flag; } set { constraint_set5_flag = value; } }
        private uint reserved_zero_2bits;
        public uint ReservedZero2bits { get { return reserved_zero_2bits; } set { reserved_zero_2bits = value; } }
        private uint level_idc;
        public uint LevelIdc { get { return level_idc; } set { level_idc = value; } }
        private uint seq_parameter_set_id;
        public uint SeqParameterSetId { get { return seq_parameter_set_id; } set { seq_parameter_set_id = value; } }
        private uint chroma_format_idc;
        public uint ChromaFormatIdc { get { return chroma_format_idc; } set { chroma_format_idc = value; } }
        private byte separate_colour_plane_flag;
        public byte SeparateColourPlaneFlag { get { return separate_colour_plane_flag; } set { separate_colour_plane_flag = value; } }
        private uint bit_depth_luma_minus8;
        public uint BitDepthLumaMinus8 { get { return bit_depth_luma_minus8; } set { bit_depth_luma_minus8 = value; } }
        private uint bit_depth_chroma_minus8;
        public uint BitDepthChromaMinus8 { get { return bit_depth_chroma_minus8; } set { bit_depth_chroma_minus8 = value; } }
        private byte qpprime_y_zero_transform_bypass_flag;
        public byte QpprimeyZeroTransformBypassFlag { get { return qpprime_y_zero_transform_bypass_flag; } set { qpprime_y_zero_transform_bypass_flag = value; } }
        private byte seq_scaling_matrix_present_flag;
        public byte SeqScalingMatrixPresentFlag { get { return seq_scaling_matrix_present_flag; } set { seq_scaling_matrix_present_flag = value; } }
        private byte[] seq_scaling_list_present_flag;
        public byte[] SeqScalingListPresentFlag { get { return seq_scaling_list_present_flag; } set { seq_scaling_list_present_flag = value; } }
        private ScalingList[] scaling_list;
        public ScalingList[] ScalingList { get { return scaling_list; } set { scaling_list = value; } }
        private uint log2_max_frame_num_minus4;
        public uint Log2MaxFrameNumMinus4 { get { return log2_max_frame_num_minus4; } set { log2_max_frame_num_minus4 = value; } }
        private uint pic_order_cnt_type;
        public uint PicOrderCntType { get { return pic_order_cnt_type; } set { pic_order_cnt_type = value; } }
        private uint log2_max_pic_order_cnt_lsb_minus4;
        public uint Log2MaxPicOrderCntLsbMinus4 { get { return log2_max_pic_order_cnt_lsb_minus4; } set { log2_max_pic_order_cnt_lsb_minus4 = value; } }
        private byte delta_pic_order_always_zero_flag;
        public byte DeltaPicOrderAlwaysZeroFlag { get { return delta_pic_order_always_zero_flag; } set { delta_pic_order_always_zero_flag = value; } }
        private int offset_for_non_ref_pic;
        public int OffsetForNonRefPic { get { return offset_for_non_ref_pic; } set { offset_for_non_ref_pic = value; } }
        private int offset_for_top_to_bottom_field;
        public int OffsetForTopToBottomField { get { return offset_for_top_to_bottom_field; } set { offset_for_top_to_bottom_field = value; } }
        private uint num_ref_frames_in_pic_order_cnt_cycle;
        public uint NumRefFramesInPicOrderCntCycle { get { return num_ref_frames_in_pic_order_cnt_cycle; } set { num_ref_frames_in_pic_order_cnt_cycle = value; } }
        private int[] offset_for_ref_frame;
        public int[] OffsetForRefFrame { get { return offset_for_ref_frame; } set { offset_for_ref_frame = value; } }
        private uint max_num_ref_frames;
        public uint MaxNumRefFrames { get { return max_num_ref_frames; } set { max_num_ref_frames = value; } }
        private byte gaps_in_frame_num_value_allowed_flag;
        public byte GapsInFrameNumValueAllowedFlag { get { return gaps_in_frame_num_value_allowed_flag; } set { gaps_in_frame_num_value_allowed_flag = value; } }
        private uint pic_width_in_mbs_minus1;
        public uint PicWidthInMbsMinus1 { get { return pic_width_in_mbs_minus1; } set { pic_width_in_mbs_minus1 = value; } }
        private uint pic_height_in_map_units_minus1;
        public uint PicHeightInMapUnitsMinus1 { get { return pic_height_in_map_units_minus1; } set { pic_height_in_map_units_minus1 = value; } }
        private byte frame_mbs_only_flag;
        public byte FrameMbsOnlyFlag { get { return frame_mbs_only_flag; } set { frame_mbs_only_flag = value; } }
        private byte mb_adaptive_frame_field_flag;
        public byte MbAdaptiveFrameFieldFlag { get { return mb_adaptive_frame_field_flag; } set { mb_adaptive_frame_field_flag = value; } }
        private byte direct_8x8_inference_flag;
        public byte Direct8x8InferenceFlag { get { return direct_8x8_inference_flag; } set { direct_8x8_inference_flag = value; } }
        private byte frame_cropping_flag;
        public byte FrameCroppingFlag { get { return frame_cropping_flag; } set { frame_cropping_flag = value; } }
        private uint frame_crop_left_offset;
        public uint FrameCropLeftOffset { get { return frame_crop_left_offset; } set { frame_crop_left_offset = value; } }
        private uint frame_crop_right_offset;
        public uint FrameCropRightOffset { get { return frame_crop_right_offset; } set { frame_crop_right_offset = value; } }
        private uint frame_crop_top_offset;
        public uint FrameCropTopOffset { get { return frame_crop_top_offset; } set { frame_crop_top_offset = value; } }
        private uint frame_crop_bottom_offset;
        public uint FrameCropBottomOffset { get { return frame_crop_bottom_offset; } set { frame_crop_bottom_offset = value; } }
        private byte vui_parameters_present_flag;
        public byte VuiParametersPresentFlag { get { return vui_parameters_present_flag; } set { vui_parameters_present_flag = value; } }
        private VuiParameters vui_parameters;
        public VuiParameters VuiParameters { get { return vui_parameters; } set { vui_parameters = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SeqParameterSetData()
        {

        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedInt(size, 8, out this.profile_idc);
            size += stream.ReadUnsignedInt(size, 1, out this.constraint_set0_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.constraint_set1_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.constraint_set2_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.constraint_set3_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.constraint_set4_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.constraint_set5_flag);
            size += stream.ReadUnsignedInt(size, 2, out this.reserved_zero_2bits); // equal to 0 
            size += stream.ReadUnsignedInt(size, 8, out this.level_idc);
            size += stream.ReadUnsignedIntGolomb(size, out this.seq_parameter_set_id);

            if (profile_idc == 100 || profile_idc == 110 ||
  profile_idc == 122 || profile_idc == 244 || profile_idc == 44 ||
  profile_idc == 83 || profile_idc == 86 || profile_idc == 118 ||
  profile_idc == 128 || profile_idc == 138 || profile_idc == 139 ||
  profile_idc == 134 || profile_idc == 135)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.chroma_format_idc);

                if (chroma_format_idc == 3)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.separate_colour_plane_flag);
                }
                size += stream.ReadUnsignedIntGolomb(size, out this.bit_depth_luma_minus8);
                size += stream.ReadUnsignedIntGolomb(size, out this.bit_depth_chroma_minus8);
                size += stream.ReadUnsignedInt(size, 1, out this.qpprime_y_zero_transform_bypass_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.seq_scaling_matrix_present_flag);

                if (seq_scaling_matrix_present_flag != 0)
                {

                    this.seq_scaling_list_present_flag = new byte[((chroma_format_idc != 3) ? 8 : 12)];
                    this.scaling_list = new ScalingList[((chroma_format_idc != 3) ? 8 : 12)];
                    for (i = 0; i < ((chroma_format_idc != 3) ? 8 : 12); i++)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.seq_scaling_list_present_flag[i]);

                        if (seq_scaling_list_present_flag[i] != 0)
                        {

                            if (i < 6)
                            {
                                size += stream.ReadClass<ScalingList>(size, context, () => new ScalingList(new uint[6 * 16], 16, 0), out this.scaling_list[i]);
                            }
                            else
                            {
                                size += stream.ReadClass<ScalingList>(size, context, () => new ScalingList(new uint[6 * 64], 64, 0), out this.scaling_list[i]);
                            }
                        }
                    }
                }
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.log2_max_frame_num_minus4);
            size += stream.ReadUnsignedIntGolomb(size, out this.pic_order_cnt_type);

            if (pic_order_cnt_type == 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.log2_max_pic_order_cnt_lsb_minus4);
            }
            else if (pic_order_cnt_type == 1)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.delta_pic_order_always_zero_flag);
                size += stream.ReadSignedIntGolomb(size, out this.offset_for_non_ref_pic);
                size += stream.ReadSignedIntGolomb(size, out this.offset_for_top_to_bottom_field);
                size += stream.ReadUnsignedIntGolomb(size, out this.num_ref_frames_in_pic_order_cnt_cycle);

                this.offset_for_ref_frame = new int[num_ref_frames_in_pic_order_cnt_cycle];
                for (i = 0; i < num_ref_frames_in_pic_order_cnt_cycle; i++)
                {
                    size += stream.ReadSignedIntGolomb(size, out this.offset_for_ref_frame[i]);
                }
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.max_num_ref_frames);
            size += stream.ReadUnsignedInt(size, 1, out this.gaps_in_frame_num_value_allowed_flag);
            size += stream.ReadUnsignedIntGolomb(size, out this.pic_width_in_mbs_minus1);
            size += stream.ReadUnsignedIntGolomb(size, out this.pic_height_in_map_units_minus1);
            size += stream.ReadUnsignedInt(size, 1, out this.frame_mbs_only_flag);

            if (frame_mbs_only_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.mb_adaptive_frame_field_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.direct_8x8_inference_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.frame_cropping_flag);

            if (frame_cropping_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.frame_crop_left_offset);
                size += stream.ReadUnsignedIntGolomb(size, out this.frame_crop_right_offset);
                size += stream.ReadUnsignedIntGolomb(size, out this.frame_crop_top_offset);
                size += stream.ReadUnsignedIntGolomb(size, out this.frame_crop_bottom_offset);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.vui_parameters_present_flag);

            if (vui_parameters_present_flag != 0)
            {
                size += stream.ReadClass<VuiParameters>(size, context, () => new VuiParameters(), out this.vui_parameters);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedInt(8, this.profile_idc);
            size += stream.WriteUnsignedInt(1, this.constraint_set0_flag);
            size += stream.WriteUnsignedInt(1, this.constraint_set1_flag);
            size += stream.WriteUnsignedInt(1, this.constraint_set2_flag);
            size += stream.WriteUnsignedInt(1, this.constraint_set3_flag);
            size += stream.WriteUnsignedInt(1, this.constraint_set4_flag);
            size += stream.WriteUnsignedInt(1, this.constraint_set5_flag);
            size += stream.WriteUnsignedInt(2, this.reserved_zero_2bits); // equal to 0 
            size += stream.WriteUnsignedInt(8, this.level_idc);
            size += stream.WriteUnsignedIntGolomb(this.seq_parameter_set_id);

            if (profile_idc == 100 || profile_idc == 110 ||
  profile_idc == 122 || profile_idc == 244 || profile_idc == 44 ||
  profile_idc == 83 || profile_idc == 86 || profile_idc == 118 ||
  profile_idc == 128 || profile_idc == 138 || profile_idc == 139 ||
  profile_idc == 134 || profile_idc == 135)
            {
                size += stream.WriteUnsignedIntGolomb(this.chroma_format_idc);

                if (chroma_format_idc == 3)
                {
                    size += stream.WriteUnsignedInt(1, this.separate_colour_plane_flag);
                }
                size += stream.WriteUnsignedIntGolomb(this.bit_depth_luma_minus8);
                size += stream.WriteUnsignedIntGolomb(this.bit_depth_chroma_minus8);
                size += stream.WriteUnsignedInt(1, this.qpprime_y_zero_transform_bypass_flag);
                size += stream.WriteUnsignedInt(1, this.seq_scaling_matrix_present_flag);

                if (seq_scaling_matrix_present_flag != 0)
                {

                    for (i = 0; i < ((chroma_format_idc != 3) ? 8 : 12); i++)
                    {
                        size += stream.WriteUnsignedInt(1, this.seq_scaling_list_present_flag[i]);

                        if (seq_scaling_list_present_flag[i] != 0)
                        {

                            if (i < 6)
                            {
                                size += stream.WriteClass<ScalingList>(context, this.scaling_list[i]);
                            }
                            else
                            {
                                size += stream.WriteClass<ScalingList>(context, this.scaling_list[i]);
                            }
                        }
                    }
                }
            }
            size += stream.WriteUnsignedIntGolomb(this.log2_max_frame_num_minus4);
            size += stream.WriteUnsignedIntGolomb(this.pic_order_cnt_type);

            if (pic_order_cnt_type == 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.log2_max_pic_order_cnt_lsb_minus4);
            }
            else if (pic_order_cnt_type == 1)
            {
                size += stream.WriteUnsignedInt(1, this.delta_pic_order_always_zero_flag);
                size += stream.WriteSignedIntGolomb(this.offset_for_non_ref_pic);
                size += stream.WriteSignedIntGolomb(this.offset_for_top_to_bottom_field);
                size += stream.WriteUnsignedIntGolomb(this.num_ref_frames_in_pic_order_cnt_cycle);

                for (i = 0; i < num_ref_frames_in_pic_order_cnt_cycle; i++)
                {
                    size += stream.WriteSignedIntGolomb(this.offset_for_ref_frame[i]);
                }
            }
            size += stream.WriteUnsignedIntGolomb(this.max_num_ref_frames);
            size += stream.WriteUnsignedInt(1, this.gaps_in_frame_num_value_allowed_flag);
            size += stream.WriteUnsignedIntGolomb(this.pic_width_in_mbs_minus1);
            size += stream.WriteUnsignedIntGolomb(this.pic_height_in_map_units_minus1);
            size += stream.WriteUnsignedInt(1, this.frame_mbs_only_flag);

            if (frame_mbs_only_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.mb_adaptive_frame_field_flag);
            }
            size += stream.WriteUnsignedInt(1, this.direct_8x8_inference_flag);
            size += stream.WriteUnsignedInt(1, this.frame_cropping_flag);

            if (frame_cropping_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.frame_crop_left_offset);
                size += stream.WriteUnsignedIntGolomb(this.frame_crop_right_offset);
                size += stream.WriteUnsignedIntGolomb(this.frame_crop_top_offset);
                size += stream.WriteUnsignedIntGolomb(this.frame_crop_bottom_offset);
            }
            size += stream.WriteUnsignedInt(1, this.vui_parameters_present_flag);

            if (vui_parameters_present_flag != 0)
            {
                size += stream.WriteClass<VuiParameters>(context, this.vui_parameters);
            }

            return size;
        }

    }

    /*


scaling_list( scalingLst, sizeOfScalingList, useDefaultScalingMatrixFlag ) { 
 lastScale = 8   
 nextScale = 8   
 for( j = 0; j < sizeOfScalingList; j++ ) {   
  if( nextScale != 0 ) {   
   delta_scale 0 | 1 se(v) 
   nextScale = ( lastScale + delta_scale + 256 ) % 256   
   useDefaultScalingMatrixFlag = ( j  ==  0 && nextScale  ==  0 )   
  }   
  scalingLst[ j ] = ( nextScale  ==  0 ) ? lastScale : nextScale   
  lastScale = scalingLst[ j ]   
 }   
}
    */
    public class ScalingList : IItuSerializable
    {
        private uint[] scalingLst;
        public uint[] ScalingLst { get { return scalingLst; } set { scalingLst = value; } }
        private uint sizeOfScalingList;
        public uint SizeOfScalingList { get { return sizeOfScalingList; } set { sizeOfScalingList = value; } }
        private uint useDefaultScalingMatrixFlag;
        public uint UseDefaultScalingMatrixFlag { get { return useDefaultScalingMatrixFlag; } set { useDefaultScalingMatrixFlag = value; } }
        private int[] delta_scale;
        public int[] DeltaScale { get { return delta_scale; } set { delta_scale = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ScalingList(uint[] scalingLst, uint sizeOfScalingList, uint useDefaultScalingMatrixFlag)
        {
            this.scalingLst = scalingLst;
            this.sizeOfScalingList = sizeOfScalingList;
            this.useDefaultScalingMatrixFlag = useDefaultScalingMatrixFlag;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint lastScale = 0;
            uint nextScale = 0;
            uint j = 0;
            lastScale = 8;
            nextScale = 8;

            this.delta_scale = new int[sizeOfScalingList];
            for (j = 0; j < sizeOfScalingList; j++)
            {

                if (nextScale != 0)
                {
                    size += stream.ReadSignedIntGolomb(size, out this.delta_scale[j]);
                    nextScale = (uint)(lastScale + delta_scale[j] + 256) % 256;
                    useDefaultScalingMatrixFlag = (j == 0 && nextScale == 0) ? (uint)1 : (uint)0;
                }
                scalingLst[j] = (nextScale == 0) ? lastScale : nextScale;
                lastScale = scalingLst[j];
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint lastScale = 0;
            uint nextScale = 0;
            uint j = 0;
            lastScale = 8;
            nextScale = 8;

            for (j = 0; j < sizeOfScalingList; j++)
            {

                if (nextScale != 0)
                {
                    size += stream.WriteSignedIntGolomb(this.delta_scale[j]);
                    nextScale = (uint)(lastScale + delta_scale[j] + 256) % 256;
                    useDefaultScalingMatrixFlag = (j == 0 && nextScale == 0) ? (uint)1 : (uint)0;
                }
                scalingLst[j] = (nextScale == 0) ? lastScale : nextScale;
                lastScale = scalingLst[j];
            }

            return size;
        }

    }

    /*


seq_parameter_set_extension_rbsp() { 
 seq_parameter_set_id 10 ue(v) 
 aux_format_idc 10 ue(v) 
 if( aux_format_idc  !=  0 ) {   
  bit_depth_aux_minus8 10 ue(v) 
  alpha_incr_flag 10 u(1) 
  alpha_opaque_value 10 u(v) 
  alpha_transparent_value 10 u(v) 
 }   
 additional_extension_flag 10 u(1) 
 rbsp_trailing_bits() 10  
}
    */
    public class SeqParameterSetExtensionRbsp : IItuSerializable
    {
        private uint seq_parameter_set_id;
        public uint SeqParameterSetId { get { return seq_parameter_set_id; } set { seq_parameter_set_id = value; } }
        private uint aux_format_idc;
        public uint AuxFormatIdc { get { return aux_format_idc; } set { aux_format_idc = value; } }
        private uint bit_depth_aux_minus8;
        public uint BitDepthAuxMinus8 { get { return bit_depth_aux_minus8; } set { bit_depth_aux_minus8 = value; } }
        private byte alpha_incr_flag;
        public byte AlphaIncrFlag { get { return alpha_incr_flag; } set { alpha_incr_flag = value; } }
        private uint alpha_opaque_value;
        public uint AlphaOpaqueValue { get { return alpha_opaque_value; } set { alpha_opaque_value = value; } }
        private uint alpha_transparent_value;
        public uint AlphaTransparentValue { get { return alpha_transparent_value; } set { alpha_transparent_value = value; } }
        private byte additional_extension_flag;
        public byte AdditionalExtensionFlag { get { return additional_extension_flag; } set { additional_extension_flag = value; } }
        private RbspTrailingBits rbsp_trailing_bits;
        public RbspTrailingBits RbspTrailingBits { get { return rbsp_trailing_bits; } set { rbsp_trailing_bits = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SeqParameterSetExtensionRbsp()
        {

        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedIntGolomb(size, out this.seq_parameter_set_id);
            size += stream.ReadUnsignedIntGolomb(size, out this.aux_format_idc);

            if (aux_format_idc != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.bit_depth_aux_minus8);
                size += stream.ReadUnsignedInt(size, 1, out this.alpha_incr_flag);
                size += stream.ReadUnsignedIntVariable(size, (context.SpsExtension.BitDepthAuxMinus8 + 9), out this.alpha_opaque_value);
                size += stream.ReadUnsignedIntVariable(size, (context.SpsExtension.BitDepthAuxMinus8 + 9), out this.alpha_transparent_value);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.additional_extension_flag);
            size += stream.ReadClass<RbspTrailingBits>(size, context, () => new RbspTrailingBits(), out this.rbsp_trailing_bits);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedIntGolomb(this.seq_parameter_set_id);
            size += stream.WriteUnsignedIntGolomb(this.aux_format_idc);

            if (aux_format_idc != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.bit_depth_aux_minus8);
                size += stream.WriteUnsignedInt(1, this.alpha_incr_flag);
                size += stream.WriteUnsignedIntVariable((context.SpsExtension.BitDepthAuxMinus8 + 9), this.alpha_opaque_value);
                size += stream.WriteUnsignedIntVariable((context.SpsExtension.BitDepthAuxMinus8 + 9), this.alpha_transparent_value);
            }
            size += stream.WriteUnsignedInt(1, this.additional_extension_flag);
            size += stream.WriteClass<RbspTrailingBits>(context, this.rbsp_trailing_bits);

            return size;
        }

    }

    /*


subset_seq_parameter_set_rbsp() { 
 seq_parameter_set_data() 0  
 if( profile_idc  ==  83  ||  profile_idc  ==  86 ) {   
  seq_parameter_set_svc_extension()  /* specified in Annex G *//* 0  
  svc_vui_parameters_present_flag 0 u(1) 
  if( svc_vui_parameters_present_flag  ==  1 )   
   svc_vui_parameters_extension()  /* specified in Annex G *//* 0  
 } else if( profile_idc  ==  118  ||  profile_idc  ==  128  || 
  profile_idc  ==  134  ) { 
  
  bit_equal_to_one  /* equal to 1 *//* 0 f(1) 
  seq_parameter_set_mvc_extension()  /* specified in Annex H *//* 0  
  mvc_vui_parameters_present_flag 0 u(1) 
  if( mvc_vui_parameters_present_flag  ==  1 )   
   mvc_vui_parameters_extension()  /* specified in Annex H *//* 0  
 } else if( profile_idc  ==  138 ||  profile_idc  ==  135 ) {   
  bit_equal_to_one  /* equal to 1 *//* 0 f(1) 
  seq_parameter_set_mvcd_extension()  /* specified in Annex I *//*   
 } else if( profile_idc  ==  139 ) {   
  bit_equal_to_one  /* equal to 1 *//* 0 f(1) 
  seq_parameter_set_mvcd_extension()  /* specified in Annex I *//* 0  
  seq_parameter_set_3davc_extension() /* specified in Annex J *//* 0  
 }   
 additional_extension2_flag 0 u(1) 
 if( additional_extension2_flag  ==  1 )   
  while( more_rbsp_data() )   
   additional_extension2_data_flag 0 u(1) 
 rbsp_trailing_bits() 0  
}
    */
    public class SubsetSeqParameterSetRbsp : IItuSerializable
    {
        private SeqParameterSetData seq_parameter_set_data;
        public SeqParameterSetData SeqParameterSetData { get { return seq_parameter_set_data; } set { seq_parameter_set_data = value; } }
        private SeqParameterSetSvcExtension seq_parameter_set_svc_extension;
        public SeqParameterSetSvcExtension SeqParameterSetSvcExtension { get { return seq_parameter_set_svc_extension; } set { seq_parameter_set_svc_extension = value; } }
        private byte svc_vui_parameters_present_flag;
        public byte SvcVuiParametersPresentFlag { get { return svc_vui_parameters_present_flag; } set { svc_vui_parameters_present_flag = value; } }
        private SvcVuiParametersExtension svc_vui_parameters_extension;
        public SvcVuiParametersExtension SvcVuiParametersExtension { get { return svc_vui_parameters_extension; } set { svc_vui_parameters_extension = value; } }
        private uint bit_equal_to_one;
        public uint BitEqualToOne { get { return bit_equal_to_one; } set { bit_equal_to_one = value; } }
        private SeqParameterSetMvcExtension seq_parameter_set_mvc_extension;
        public SeqParameterSetMvcExtension SeqParameterSetMvcExtension { get { return seq_parameter_set_mvc_extension; } set { seq_parameter_set_mvc_extension = value; } }
        private byte mvc_vui_parameters_present_flag;
        public byte MvcVuiParametersPresentFlag { get { return mvc_vui_parameters_present_flag; } set { mvc_vui_parameters_present_flag = value; } }
        private MvcVuiParametersExtension mvc_vui_parameters_extension;
        public MvcVuiParametersExtension MvcVuiParametersExtension { get { return mvc_vui_parameters_extension; } set { mvc_vui_parameters_extension = value; } }
        private SeqParameterSetMvcdExtension seq_parameter_set_mvcd_extension;
        public SeqParameterSetMvcdExtension SeqParameterSetMvcdExtension { get { return seq_parameter_set_mvcd_extension; } set { seq_parameter_set_mvcd_extension = value; } }
        private SeqParameterSet3davcExtension seq_parameter_set_3davc_extension;
        public SeqParameterSet3davcExtension SeqParameterSet3davcExtension { get { return seq_parameter_set_3davc_extension; } set { seq_parameter_set_3davc_extension = value; } }
        private byte additional_extension2_flag;
        public byte AdditionalExtension2Flag { get { return additional_extension2_flag; } set { additional_extension2_flag = value; } }
        private byte additional_extension2_data_flag;
        public byte AdditionalExtension2DataFlag { get { return additional_extension2_data_flag; } set { additional_extension2_data_flag = value; } }
        private RbspTrailingBits rbsp_trailing_bits;
        public RbspTrailingBits RbspTrailingBits { get { return rbsp_trailing_bits; } set { rbsp_trailing_bits = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SubsetSeqParameterSetRbsp()
        {

        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadClass<SeqParameterSetData>(size, context, () => new SeqParameterSetData(), out this.seq_parameter_set_data);

            if (context.Sps.SeqParameterSetData.ProfileIdc == 83 || context.Sps.SeqParameterSetData.ProfileIdc == 86)
            {
                size += stream.ReadClass<SeqParameterSetSvcExtension>(size, context, () => new SeqParameterSetSvcExtension(), out this.seq_parameter_set_svc_extension); // specified in Annex G 
                size += stream.ReadUnsignedInt(size, 1, out this.svc_vui_parameters_present_flag);

                if (svc_vui_parameters_present_flag == 1)
                {
                    size += stream.ReadClass<SvcVuiParametersExtension>(size, context, () => new SvcVuiParametersExtension(), out this.svc_vui_parameters_extension); // specified in Annex G 
                }
            }
            else if (context.Sps.SeqParameterSetData.ProfileIdc == 118 || context.Sps.SeqParameterSetData.ProfileIdc == 128 ||
  context.Sps.SeqParameterSetData.ProfileIdc == 134)
            {
                size += stream.ReadFixed(size, 1, out this.bit_equal_to_one); // equal to 1 
                size += stream.ReadClass<SeqParameterSetMvcExtension>(size, context, () => new SeqParameterSetMvcExtension(), out this.seq_parameter_set_mvc_extension); // specified in Annex H 
                size += stream.ReadUnsignedInt(size, 1, out this.mvc_vui_parameters_present_flag);

                if (mvc_vui_parameters_present_flag == 1)
                {
                    size += stream.ReadClass<MvcVuiParametersExtension>(size, context, () => new MvcVuiParametersExtension(), out this.mvc_vui_parameters_extension); // specified in Annex H 
                }
            }
            else if (context.Sps.SeqParameterSetData.ProfileIdc == 138 || context.Sps.SeqParameterSetData.ProfileIdc == 135)
            {
                size += stream.ReadFixed(size, 1, out this.bit_equal_to_one); // equal to 1 
                size += stream.ReadClass<SeqParameterSetMvcdExtension>(size, context, () => new SeqParameterSetMvcdExtension(), out this.seq_parameter_set_mvcd_extension); // specified in Annex I 
            }
            else if (context.Sps.SeqParameterSetData.ProfileIdc == 139)
            {
                size += stream.ReadFixed(size, 1, out this.bit_equal_to_one); // equal to 1 
                size += stream.ReadClass<SeqParameterSetMvcdExtension>(size, context, () => new SeqParameterSetMvcdExtension(), out this.seq_parameter_set_mvcd_extension); // specified in Annex I 
                size += stream.ReadClass<SeqParameterSet3davcExtension>(size, context, () => new SeqParameterSet3davcExtension(), out this.seq_parameter_set_3davc_extension); // specified in Annex J 
            }
            size += stream.ReadUnsignedInt(size, 1, out this.additional_extension2_flag);

            if (additional_extension2_flag == 1)
            {

                while (stream.ReadMoreRbspData(this))
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.additional_extension2_data_flag);
                }
            }
            size += stream.ReadClass<RbspTrailingBits>(size, context, () => new RbspTrailingBits(), out this.rbsp_trailing_bits);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteClass<SeqParameterSetData>(context, this.seq_parameter_set_data);

            if (context.Sps.SeqParameterSetData.ProfileIdc == 83 || context.Sps.SeqParameterSetData.ProfileIdc == 86)
            {
                size += stream.WriteClass<SeqParameterSetSvcExtension>(context, this.seq_parameter_set_svc_extension); // specified in Annex G 
                size += stream.WriteUnsignedInt(1, this.svc_vui_parameters_present_flag);

                if (svc_vui_parameters_present_flag == 1)
                {
                    size += stream.WriteClass<SvcVuiParametersExtension>(context, this.svc_vui_parameters_extension); // specified in Annex G 
                }
            }
            else if (context.Sps.SeqParameterSetData.ProfileIdc == 118 || context.Sps.SeqParameterSetData.ProfileIdc == 128 ||
  context.Sps.SeqParameterSetData.ProfileIdc == 134)
            {
                size += stream.WriteFixed(1, this.bit_equal_to_one); // equal to 1 
                size += stream.WriteClass<SeqParameterSetMvcExtension>(context, this.seq_parameter_set_mvc_extension); // specified in Annex H 
                size += stream.WriteUnsignedInt(1, this.mvc_vui_parameters_present_flag);

                if (mvc_vui_parameters_present_flag == 1)
                {
                    size += stream.WriteClass<MvcVuiParametersExtension>(context, this.mvc_vui_parameters_extension); // specified in Annex H 
                }
            }
            else if (context.Sps.SeqParameterSetData.ProfileIdc == 138 || context.Sps.SeqParameterSetData.ProfileIdc == 135)
            {
                size += stream.WriteFixed(1, this.bit_equal_to_one); // equal to 1 
                size += stream.WriteClass<SeqParameterSetMvcdExtension>(context, this.seq_parameter_set_mvcd_extension); // specified in Annex I 
            }
            else if (context.Sps.SeqParameterSetData.ProfileIdc == 139)
            {
                size += stream.WriteFixed(1, this.bit_equal_to_one); // equal to 1 
                size += stream.WriteClass<SeqParameterSetMvcdExtension>(context, this.seq_parameter_set_mvcd_extension); // specified in Annex I 
                size += stream.WriteClass<SeqParameterSet3davcExtension>(context, this.seq_parameter_set_3davc_extension); // specified in Annex J 
            }
            size += stream.WriteUnsignedInt(1, this.additional_extension2_flag);

            if (additional_extension2_flag == 1)
            {

                while (stream.WriteMoreRbspData(this))
                {
                    size += stream.WriteUnsignedInt(1, this.additional_extension2_data_flag);
                }
            }
            size += stream.WriteClass<RbspTrailingBits>(context, this.rbsp_trailing_bits);

            return size;
        }

    }

    /*



pic_parameter_set_rbsp() { 
 pic_parameter_set_id 1 ue(v) 
 seq_parameter_set_id 1 ue(v) 
 entropy_coding_mode_flag 1 u(1) 
 bottom_field_pic_order_in_frame_present_flag 1 u(1) 
 num_slice_groups_minus1 1 ue(v) 
 if( num_slice_groups_minus1 > 0 ) {   
  slice_group_map_type 1 ue(v) 
  if( slice_group_map_type  ==  0 )   
   for( iGroup = 0; iGroup <= num_slice_groups_minus1; iGroup++ )   
    run_length_minus1[ iGroup ] 1 ue(v) 
  else if( slice_group_map_type  ==  2 )   
   for( iGroup = 0; iGroup < num_slice_groups_minus1; iGroup++ ) {   
    top_left[ iGroup ] 1 ue(v) 
    bottom_right[ iGroup ] 1 ue(v) 
   }   
  else if(  slice_group_map_type  ==  3  ||   
     slice_group_map_type  ==  4  ||   
     slice_group_map_type  ==  5 ) { 
  
   slice_group_change_direction_flag 1 u(1) 
   slice_group_change_rate_minus1 1 ue(v) 
  } else if( slice_group_map_type  ==  6 ) {   
   pic_size_in_map_units_minus1 1 ue(v) 
   for( i = 0; i <= pic_size_in_map_units_minus1; i++ )   
    slice_group_id[ i ] 1 u(v) 
  }   
 }   
 num_ref_idx_l0_default_active_minus1 1 ue(v) 
 num_ref_idx_l1_default_active_minus1 1 ue(v) 
 weighted_pred_flag 1 u(1) 
 weighted_bipred_idc 1 u(2) 
 pic_init_qp_minus26 1 se(v) 
 pic_init_qs_minus26 1 se(v) 
 chroma_qp_index_offset 1 se(v) 
 deblocking_filter_control_present_flag 1 u(1) 
 constrained_intra_pred_flag 1 u(1) 
 redundant_pic_cnt_present_flag 1 u(1) 
 if( more_rbsp_data() ) {   
  transform_8x8_mode_flag 1 u(1) 
  pic_scaling_matrix_present_flag 1 u(1) 
  if( pic_scaling_matrix_present_flag )   
   for( i = 0; i < 6 + 
     ( ( chroma_format_idc  !=  3 ) ? 2 : 6 ) * transform_8x8_mode_flag; 
     i++ ) { 
  
    pic_scaling_list_present_flag[ i ] 1 u(1) 
    if( pic_scaling_list_present_flag[ i ] )   
     if( i < 6 )    
     scaling_list( ScalingList4x4[ i ], 16, UseDefaultScalingMatrix4x4Flag[ i ] ) 1  
     else   
      scaling_list( ScalingList8x8[ i - 6 ], 64, UseDefaultScalingMatrix8x8Flag[ i - 6 ] ) 1  
   }   
  second_chroma_qp_index_offset 1 se(v) 
 }   
 rbsp_trailing_bits() 1  
}
    */
    public class PicParameterSetRbsp : IItuSerializable
    {
        private uint pic_parameter_set_id;
        public uint PicParameterSetId { get { return pic_parameter_set_id; } set { pic_parameter_set_id = value; } }
        private uint seq_parameter_set_id;
        public uint SeqParameterSetId { get { return seq_parameter_set_id; } set { seq_parameter_set_id = value; } }
        private byte entropy_coding_mode_flag;
        public byte EntropyCodingModeFlag { get { return entropy_coding_mode_flag; } set { entropy_coding_mode_flag = value; } }
        private byte bottom_field_pic_order_in_frame_present_flag;
        public byte BottomFieldPicOrderInFramePresentFlag { get { return bottom_field_pic_order_in_frame_present_flag; } set { bottom_field_pic_order_in_frame_present_flag = value; } }
        private uint num_slice_groups_minus1;
        public uint NumSliceGroupsMinus1 { get { return num_slice_groups_minus1; } set { num_slice_groups_minus1 = value; } }
        private uint slice_group_map_type;
        public uint SliceGroupMapType { get { return slice_group_map_type; } set { slice_group_map_type = value; } }
        private uint[] run_length_minus1;
        public uint[] RunLengthMinus1 { get { return run_length_minus1; } set { run_length_minus1 = value; } }
        private uint[] top_left;
        public uint[] TopLeft { get { return top_left; } set { top_left = value; } }
        private uint[] bottom_right;
        public uint[] BottomRight { get { return bottom_right; } set { bottom_right = value; } }
        private byte slice_group_change_direction_flag;
        public byte SliceGroupChangeDirectionFlag { get { return slice_group_change_direction_flag; } set { slice_group_change_direction_flag = value; } }
        private uint slice_group_change_rate_minus1;
        public uint SliceGroupChangeRateMinus1 { get { return slice_group_change_rate_minus1; } set { slice_group_change_rate_minus1 = value; } }
        private uint pic_size_in_map_units_minus1;
        public uint PicSizeInMapUnitsMinus1 { get { return pic_size_in_map_units_minus1; } set { pic_size_in_map_units_minus1 = value; } }
        private uint[] slice_group_id;
        public uint[] SliceGroupId { get { return slice_group_id; } set { slice_group_id = value; } }
        private uint num_ref_idx_l0_default_active_minus1;
        public uint NumRefIdxL0DefaultActiveMinus1 { get { return num_ref_idx_l0_default_active_minus1; } set { num_ref_idx_l0_default_active_minus1 = value; } }
        private uint num_ref_idx_l1_default_active_minus1;
        public uint NumRefIdxL1DefaultActiveMinus1 { get { return num_ref_idx_l1_default_active_minus1; } set { num_ref_idx_l1_default_active_minus1 = value; } }
        private byte weighted_pred_flag;
        public byte WeightedPredFlag { get { return weighted_pred_flag; } set { weighted_pred_flag = value; } }
        private uint weighted_bipred_idc;
        public uint WeightedBipredIdc { get { return weighted_bipred_idc; } set { weighted_bipred_idc = value; } }
        private int pic_init_qp_minus26;
        public int PicInitQpMinus26 { get { return pic_init_qp_minus26; } set { pic_init_qp_minus26 = value; } }
        private int pic_init_qs_minus26;
        public int PicInitQsMinus26 { get { return pic_init_qs_minus26; } set { pic_init_qs_minus26 = value; } }
        private int chroma_qp_index_offset;
        public int ChromaQpIndexOffset { get { return chroma_qp_index_offset; } set { chroma_qp_index_offset = value; } }
        private byte deblocking_filter_control_present_flag;
        public byte DeblockingFilterControlPresentFlag { get { return deblocking_filter_control_present_flag; } set { deblocking_filter_control_present_flag = value; } }
        private byte constrained_intra_pred_flag;
        public byte ConstrainedIntraPredFlag { get { return constrained_intra_pred_flag; } set { constrained_intra_pred_flag = value; } }
        private byte redundant_pic_cnt_present_flag;
        public byte RedundantPicCntPresentFlag { get { return redundant_pic_cnt_present_flag; } set { redundant_pic_cnt_present_flag = value; } }
        private byte transform_8x8_mode_flag;
        public byte Transform8x8ModeFlag { get { return transform_8x8_mode_flag; } set { transform_8x8_mode_flag = value; } }
        private byte pic_scaling_matrix_present_flag;
        public byte PicScalingMatrixPresentFlag { get { return pic_scaling_matrix_present_flag; } set { pic_scaling_matrix_present_flag = value; } }
        private byte[] pic_scaling_list_present_flag;
        public byte[] PicScalingListPresentFlag { get { return pic_scaling_list_present_flag; } set { pic_scaling_list_present_flag = value; } }
        private ScalingList[] scaling_list;
        public ScalingList[] ScalingList { get { return scaling_list; } set { scaling_list = value; } }
        private int second_chroma_qp_index_offset;
        public int SecondChromaQpIndexOffset { get { return second_chroma_qp_index_offset; } set { second_chroma_qp_index_offset = value; } }
        private RbspTrailingBits rbsp_trailing_bits;
        public RbspTrailingBits RbspTrailingBits { get { return rbsp_trailing_bits; } set { rbsp_trailing_bits = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public PicParameterSetRbsp()
        {

        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint iGroup = 0;
            uint i = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.pic_parameter_set_id);
            size += stream.ReadUnsignedIntGolomb(size, out this.seq_parameter_set_id);
            size += stream.ReadUnsignedInt(size, 1, out this.entropy_coding_mode_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.bottom_field_pic_order_in_frame_present_flag);
            size += stream.ReadUnsignedIntGolomb(size, out this.num_slice_groups_minus1);

            if (num_slice_groups_minus1 > 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.slice_group_map_type);

                if (slice_group_map_type == 0)
                {

                    this.run_length_minus1 = new uint[num_slice_groups_minus1 + 1];
                    for (iGroup = 0; iGroup <= num_slice_groups_minus1; iGroup++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.run_length_minus1[iGroup]);
                    }
                }
                else if (slice_group_map_type == 2)
                {

                    this.top_left = new uint[num_slice_groups_minus1 + 1];
                    this.bottom_right = new uint[num_slice_groups_minus1 + 1];
                    for (iGroup = 0; iGroup < num_slice_groups_minus1; iGroup++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.top_left[iGroup]);
                        size += stream.ReadUnsignedIntGolomb(size, out this.bottom_right[iGroup]);
                    }
                }
                else if (slice_group_map_type == 3 ||
     slice_group_map_type == 4 ||
     slice_group_map_type == 5)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.slice_group_change_direction_flag);
                    size += stream.ReadUnsignedIntGolomb(size, out this.slice_group_change_rate_minus1);
                }
                else if (slice_group_map_type == 6)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.pic_size_in_map_units_minus1);

                    this.slice_group_id = new uint[pic_size_in_map_units_minus1 + 1];
                    for (i = 0; i <= pic_size_in_map_units_minus1; i++)
                    {
                        size += stream.ReadUnsignedIntVariable(size, (uint)Math.Ceiling(Math.Log2(context.Pps.NumSliceGroupsMinus1 + 1)), out this.slice_group_id[i]);
                    }
                }
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.num_ref_idx_l0_default_active_minus1);
            size += stream.ReadUnsignedIntGolomb(size, out this.num_ref_idx_l1_default_active_minus1);
            size += stream.ReadUnsignedInt(size, 1, out this.weighted_pred_flag);
            size += stream.ReadUnsignedInt(size, 2, out this.weighted_bipred_idc);
            size += stream.ReadSignedIntGolomb(size, out this.pic_init_qp_minus26);
            size += stream.ReadSignedIntGolomb(size, out this.pic_init_qs_minus26);
            size += stream.ReadSignedIntGolomb(size, out this.chroma_qp_index_offset);
            size += stream.ReadUnsignedInt(size, 1, out this.deblocking_filter_control_present_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.constrained_intra_pred_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.redundant_pic_cnt_present_flag);

            if (stream.ReadMoreRbspData(this))
            {
                size += stream.ReadUnsignedInt(size, 1, out this.transform_8x8_mode_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.pic_scaling_matrix_present_flag);

                if (pic_scaling_matrix_present_flag != 0)
                {

                    this.pic_scaling_list_present_flag = new byte[6 +
     ((context.Sps.SeqParameterSetData.ChromaFormatIdc != 3) ? 2 : 6) * transform_8x8_mode_flag];
                    this.scaling_list = new ScalingList[6 +
     ((context.Sps.SeqParameterSetData.ChromaFormatIdc != 3) ? 2 : 6) * transform_8x8_mode_flag];
                    for (i = 0; i < 6 +
     ((context.Sps.SeqParameterSetData.ChromaFormatIdc != 3) ? 2 : 6) * transform_8x8_mode_flag;
     i++)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.pic_scaling_list_present_flag[i]);

                        if (pic_scaling_list_present_flag[i] != 0)
                        {

                            if (i < 6)
                            {
                                size += stream.ReadClass<ScalingList>(size, context, () => new ScalingList(new uint[6 * 16], 16, 0), out this.scaling_list[i]);
                            }
                            else
                            {
                                size += stream.ReadClass<ScalingList>(size, context, () => new ScalingList(new uint[6 * 64], 64, 0), out this.scaling_list[i]);
                            }
                        }
                    }
                }
                size += stream.ReadSignedIntGolomb(size, out this.second_chroma_qp_index_offset);
            }
            size += stream.ReadClass<RbspTrailingBits>(size, context, () => new RbspTrailingBits(), out this.rbsp_trailing_bits);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint iGroup = 0;
            uint i = 0;
            size += stream.WriteUnsignedIntGolomb(this.pic_parameter_set_id);
            size += stream.WriteUnsignedIntGolomb(this.seq_parameter_set_id);
            size += stream.WriteUnsignedInt(1, this.entropy_coding_mode_flag);
            size += stream.WriteUnsignedInt(1, this.bottom_field_pic_order_in_frame_present_flag);
            size += stream.WriteUnsignedIntGolomb(this.num_slice_groups_minus1);

            if (num_slice_groups_minus1 > 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.slice_group_map_type);

                if (slice_group_map_type == 0)
                {

                    for (iGroup = 0; iGroup <= num_slice_groups_minus1; iGroup++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.run_length_minus1[iGroup]);
                    }
                }
                else if (slice_group_map_type == 2)
                {

                    for (iGroup = 0; iGroup < num_slice_groups_minus1; iGroup++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.top_left[iGroup]);
                        size += stream.WriteUnsignedIntGolomb(this.bottom_right[iGroup]);
                    }
                }
                else if (slice_group_map_type == 3 ||
     slice_group_map_type == 4 ||
     slice_group_map_type == 5)
                {
                    size += stream.WriteUnsignedInt(1, this.slice_group_change_direction_flag);
                    size += stream.WriteUnsignedIntGolomb(this.slice_group_change_rate_minus1);
                }
                else if (slice_group_map_type == 6)
                {
                    size += stream.WriteUnsignedIntGolomb(this.pic_size_in_map_units_minus1);

                    for (i = 0; i <= pic_size_in_map_units_minus1; i++)
                    {
                        size += stream.WriteUnsignedIntVariable((uint)Math.Ceiling(Math.Log2(context.Pps.NumSliceGroupsMinus1 + 1)), this.slice_group_id[i]);
                    }
                }
            }
            size += stream.WriteUnsignedIntGolomb(this.num_ref_idx_l0_default_active_minus1);
            size += stream.WriteUnsignedIntGolomb(this.num_ref_idx_l1_default_active_minus1);
            size += stream.WriteUnsignedInt(1, this.weighted_pred_flag);
            size += stream.WriteUnsignedInt(2, this.weighted_bipred_idc);
            size += stream.WriteSignedIntGolomb(this.pic_init_qp_minus26);
            size += stream.WriteSignedIntGolomb(this.pic_init_qs_minus26);
            size += stream.WriteSignedIntGolomb(this.chroma_qp_index_offset);
            size += stream.WriteUnsignedInt(1, this.deblocking_filter_control_present_flag);
            size += stream.WriteUnsignedInt(1, this.constrained_intra_pred_flag);
            size += stream.WriteUnsignedInt(1, this.redundant_pic_cnt_present_flag);

            if (stream.WriteMoreRbspData(this))
            {
                size += stream.WriteUnsignedInt(1, this.transform_8x8_mode_flag);
                size += stream.WriteUnsignedInt(1, this.pic_scaling_matrix_present_flag);

                if (pic_scaling_matrix_present_flag != 0)
                {

                    for (i = 0; i < 6 +
     ((context.Sps.SeqParameterSetData.ChromaFormatIdc != 3) ? 2 : 6) * transform_8x8_mode_flag;
     i++)
                    {
                        size += stream.WriteUnsignedInt(1, this.pic_scaling_list_present_flag[i]);

                        if (pic_scaling_list_present_flag[i] != 0)
                        {

                            if (i < 6)
                            {
                                size += stream.WriteClass<ScalingList>(context, this.scaling_list[i]);
                            }
                            else
                            {
                                size += stream.WriteClass<ScalingList>(context, this.scaling_list[i]);
                            }
                        }
                    }
                }
                size += stream.WriteSignedIntGolomb(this.second_chroma_qp_index_offset);
            }
            size += stream.WriteClass<RbspTrailingBits>(context, this.rbsp_trailing_bits);

            return size;
        }

    }

    /*
  

sei_rbsp() {
    do   
    {
        sei_message() 5
    } while( more_rbsp_data() )   
  rbsp_trailing_bits() 5  
}
    */
    public class SeiRbsp : IItuSerializable
    {
        private SeiMessage sei_message;
        public SeiMessage SeiMessage { get { return sei_message; } set { sei_message = value; } }
        private RbspTrailingBits rbsp_trailing_bits;
        public RbspTrailingBits RbspTrailingBits { get { return rbsp_trailing_bits; } set { rbsp_trailing_bits = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SeiRbsp()
        {

        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;


            do
            {
                size += stream.ReadClass<SeiMessage>(size, context, () => new SeiMessage(), out this.sei_message);
            } while (stream.ReadMoreRbspData(this));
            size += stream.ReadClass<RbspTrailingBits>(size, context, () => new RbspTrailingBits(), out this.rbsp_trailing_bits);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;


            do
            {
                size += stream.WriteClass<SeiMessage>(context, this.sei_message);
            } while (stream.WriteMoreRbspData(this));
            size += stream.WriteClass<RbspTrailingBits>(context, this.rbsp_trailing_bits);

            return size;
        }

    }

    /*
  

sei_message() { 
  payloadType = 0   
  while( next_bits( 8 )  ==  0xFF ) {   
    ff_byte  /* equal to 0xFF *//* 5 f(8) 
    payloadType += 255   
  }   
  last_payload_type_byte 5 u(8) 
  payloadType += last_payload_type_byte   
  payloadSize = 0   
  while( next_bits( 8 )  ==  0xFF ) {   
    ff_byte  /* equal to 0xFF *//* 5 f(8) 
    payloadSize += 255   
  }   
  last_payload_size_byte 5 u(8) 
  payloadSize += last_payload_size_byte   
  sei_payload( payloadType, payloadSize ) 5  
}
    */
    public class SeiMessage : IItuSerializable
    {
        private uint ff_byte;
        public uint FfByte { get { return ff_byte; } set { ff_byte = value; } }
        private uint last_payload_type_byte;
        public uint LastPayloadTypeByte { get { return last_payload_type_byte; } set { last_payload_type_byte = value; } }
        private uint last_payload_size_byte;
        public uint LastPayloadSizeByte { get { return last_payload_size_byte; } set { last_payload_size_byte = value; } }
        private SeiPayload sei_payload;
        public SeiPayload SeiPayload { get { return sei_payload; } set { sei_payload = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SeiMessage()
        {

        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint payloadType = 0;
            uint payloadSize = 0;
            payloadType = 0;

            while (stream.ReadNextBits(this, 8) == 0xFF)
            {
                size += stream.ReadFixed(size, 8, out this.ff_byte); // equal to 0xFF 
                payloadType += 255;
            }
            size += stream.ReadUnsignedInt(size, 8, out this.last_payload_type_byte);
            payloadType += last_payload_type_byte;
            payloadSize = 0;

            while (stream.ReadNextBits(this, 8) == 0xFF)
            {
                size += stream.ReadFixed(size, 8, out this.ff_byte); // equal to 0xFF 
                payloadSize += 255;
            }
            size += stream.ReadUnsignedInt(size, 8, out this.last_payload_size_byte);
            payloadSize += last_payload_size_byte;
            size += stream.ReadClass<SeiPayload>(size, context, () => new SeiPayload(payloadType, payloadSize), out this.sei_payload);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint payloadType = 0;
            uint payloadSize = 0;
            payloadType = 0;

            while (stream.WriteNextBits(this, 8) == 0xFF)
            {
                size += stream.WriteFixed(8, this.ff_byte); // equal to 0xFF 
                payloadType += 255;
            }
            size += stream.WriteUnsignedInt(8, this.last_payload_type_byte);
            payloadType += last_payload_type_byte;
            payloadSize = 0;

            while (stream.WriteNextBits(this, 8) == 0xFF)
            {
                size += stream.WriteFixed(8, this.ff_byte); // equal to 0xFF 
                payloadSize += 255;
            }
            size += stream.WriteUnsignedInt(8, this.last_payload_size_byte);
            payloadSize += last_payload_size_byte;
            size += stream.WriteClass<SeiPayload>(context, this.sei_payload);

            return size;
        }

    }

    /*
  

access_unit_delimiter_rbsp() { 
 primary_pic_type 6 u(3) 
 rbsp_trailing_bits() 6  
}
    */
    public class AccessUnitDelimiterRbsp : IItuSerializable
    {
        private uint primary_pic_type;
        public uint PrimaryPicType { get { return primary_pic_type; } set { primary_pic_type = value; } }
        private RbspTrailingBits rbsp_trailing_bits;
        public RbspTrailingBits RbspTrailingBits { get { return rbsp_trailing_bits; } set { rbsp_trailing_bits = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public AccessUnitDelimiterRbsp()
        {

        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 3, out this.primary_pic_type);
            size += stream.ReadClass<RbspTrailingBits>(size, context, () => new RbspTrailingBits(), out this.rbsp_trailing_bits);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(3, this.primary_pic_type);
            size += stream.WriteClass<RbspTrailingBits>(context, this.rbsp_trailing_bits);

            return size;
        }

    }

    /*
    

rbsp_trailing_bits() { 
 rbsp_stop_one_bit  /* equal to 1 *//* All f(1) 
 while( !byte_aligned() )   
  rbsp_alignment_zero_bit  /* equal to 0 *//* All f(1) 
}
    */
    public class RbspTrailingBits : IItuSerializable
    {
        private uint rbsp_stop_one_bit;
        public uint RbspStopOneBit { get { return rbsp_stop_one_bit; } set { rbsp_stop_one_bit = value; } }
        private uint rbsp_alignment_zero_bit;
        public uint RbspAlignmentZeroBit { get { return rbsp_alignment_zero_bit; } set { rbsp_alignment_zero_bit = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public RbspTrailingBits()
        {

        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadFixed(size, 1, out this.rbsp_stop_one_bit); // equal to 1 

            while (!stream.ByteAligned())
            {
                size += stream.ReadFixed(size, 1, out this.rbsp_alignment_zero_bit); // equal to 0 
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteFixed(1, this.rbsp_stop_one_bit); // equal to 1 

            while (!stream.ByteAligned())
            {
                size += stream.WriteFixed(1, this.rbsp_alignment_zero_bit); // equal to 0 
            }

            return size;
        }

    }

    /*
   

dec_ref_pic_marking() { 
 if( IdrPicFlag ) {    
  no_output_of_prior_pics_flag 2 | 5 u(1) 
  long_term_reference_flag 2 | 5 u(1) 
 } else {   
  adaptive_ref_pic_marking_mode_flag 2 | 5 u(1) 
  if( adaptive_ref_pic_marking_mode_flag )   
   do {   
    memory_management_control_operation 2 | 5 ue(v) 
    if( memory_management_control_operation  ==  1  || 
     memory_management_control_operation  ==  3 ) 
  
     difference_of_pic_nums_minus1 2 | 5 ue(v) 
    if(memory_management_control_operation  ==  2  )   
     long_term_pic_num 2 | 5 ue(v) 
     if( memory_management_control_operation  ==  3  || 
     memory_management_control_operation  ==  6 ) 
  
     long_term_frame_idx 2 | 5 ue(v) 
    if( memory_management_control_operation  ==  4 )   
     max_long_term_frame_idx_plus1 2 | 5 ue(v) 
   } while( memory_management_control_operation  !=  0 )   
 }   
}
    */
    public class DecRefPicMarking : IItuSerializable
    {
        private byte no_output_of_prior_pics_flag;
        public byte NoOutputOfPriorPicsFlag { get { return no_output_of_prior_pics_flag; } set { no_output_of_prior_pics_flag = value; } }
        private byte long_term_reference_flag;
        public byte LongTermReferenceFlag { get { return long_term_reference_flag; } set { long_term_reference_flag = value; } }
        private byte adaptive_ref_pic_marking_mode_flag;
        public byte AdaptiveRefPicMarkingModeFlag { get { return adaptive_ref_pic_marking_mode_flag; } set { adaptive_ref_pic_marking_mode_flag = value; } }
        private uint memory_management_control_operation;
        public uint MemoryManagementControlOperation { get { return memory_management_control_operation; } set { memory_management_control_operation = value; } }
        private uint difference_of_pic_nums_minus1;
        public uint DifferenceOfPicNumsMinus1 { get { return difference_of_pic_nums_minus1; } set { difference_of_pic_nums_minus1 = value; } }
        private uint long_term_pic_num;
        public uint LongTermPicNum { get { return long_term_pic_num; } set { long_term_pic_num = value; } }
        private uint long_term_frame_idx;
        public uint LongTermFrameIdx { get { return long_term_frame_idx; } set { long_term_frame_idx = value; } }
        private uint max_long_term_frame_idx_plus1;
        public uint MaxLongTermFrameIdxPlus1 { get { return max_long_term_frame_idx_plus1; } set { max_long_term_frame_idx_plus1 = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public DecRefPicMarking()
        {

        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;


            if ((uint)((context.NalHeader.NalUnitType == 5) ? 1 : 0) != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.no_output_of_prior_pics_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.long_term_reference_flag);
            }
            else
            {
                size += stream.ReadUnsignedInt(size, 1, out this.adaptive_ref_pic_marking_mode_flag);

                if (adaptive_ref_pic_marking_mode_flag != 0)
                {

                    do
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.memory_management_control_operation);

                        if (memory_management_control_operation == 1 ||
     memory_management_control_operation == 3)
                        {
                            size += stream.ReadUnsignedIntGolomb(size, out this.difference_of_pic_nums_minus1);
                        }

                        if (memory_management_control_operation == 2)
                        {
                            size += stream.ReadUnsignedIntGolomb(size, out this.long_term_pic_num);
                        }

                        if (memory_management_control_operation == 3 ||
     memory_management_control_operation == 6)
                        {
                            size += stream.ReadUnsignedIntGolomb(size, out this.long_term_frame_idx);
                        }

                        if (memory_management_control_operation == 4)
                        {
                            size += stream.ReadUnsignedIntGolomb(size, out this.max_long_term_frame_idx_plus1);
                        }
                    } while (memory_management_control_operation != 0);
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;


            if ((uint)((context.NalHeader.NalUnitType == 5) ? 1 : 0) != 0)
            {
                size += stream.WriteUnsignedInt(1, this.no_output_of_prior_pics_flag);
                size += stream.WriteUnsignedInt(1, this.long_term_reference_flag);
            }
            else
            {
                size += stream.WriteUnsignedInt(1, this.adaptive_ref_pic_marking_mode_flag);

                if (adaptive_ref_pic_marking_mode_flag != 0)
                {

                    do
                    {
                        size += stream.WriteUnsignedIntGolomb(this.memory_management_control_operation);

                        if (memory_management_control_operation == 1 ||
     memory_management_control_operation == 3)
                        {
                            size += stream.WriteUnsignedIntGolomb(this.difference_of_pic_nums_minus1);
                        }

                        if (memory_management_control_operation == 2)
                        {
                            size += stream.WriteUnsignedIntGolomb(this.long_term_pic_num);
                        }

                        if (memory_management_control_operation == 3 ||
     memory_management_control_operation == 6)
                        {
                            size += stream.WriteUnsignedIntGolomb(this.long_term_frame_idx);
                        }

                        if (memory_management_control_operation == 4)
                        {
                            size += stream.WriteUnsignedIntGolomb(this.max_long_term_frame_idx_plus1);
                        }
                    } while (memory_management_control_operation != 0);
                }
            }

            return size;
        }

    }

    /*
 

sei_payload( payloadType, payloadSize ) { 
if( payloadType  ==  0 )   
 
buffering_period( payloadSize ) 5
else if( payloadType  ==  1 )   
pic_timing( payloadSize ) 5
else if( payloadType  ==  2 ) 
pan_scan_rect( payloadSize )  5  
else if( payloadType  ==  3 )  
filler_payload( payloadSize ) 5  
else if( payloadType  ==  4 )   
user_data_registered_itu_t_t35( payloadSize ) 5  
else if( payloadType  ==  5 )   
user_data_unregistered( payloadSize ) 5  
else if( payloadType  ==  6 )   
recovery_point( payloadSize ) 5
else if( payloadType  ==  7 )   
dec_ref_pic_marking_repetition( payloadSize ) 5
else if( payloadType  ==  8 )  
spare_pic( payloadSize ) 5
else if( payloadType  ==  9 )   
scene_info( payloadSize ) 5
else if( payloadType  ==  10 )   
sub_seq_info( payloadSize ) 5
else if( payloadType  ==  11 )   
sub_seq_layer_characteristics( payloadSize ) 5
else if( payloadType  ==  12 )   
sub_seq_characteristics( payloadSize ) 5
else if( payloadType  ==  13 )   
full_frame_freeze( payloadSize ) 5
else if( payloadType  ==  14 )   
full_frame_freeze_release( payloadSize ) 5
else if( payloadType  ==  15 )   
full_frame_snapshot( payloadSize ) 5
else if( payloadType  ==  16 )   
progressive_refinement_segment_start( payloadSize ) 5
else if( payloadType  ==  17 )   
progressive_refinement_segment_end( payloadSize ) 5
else if( payloadType  ==  18 )   
motion_constrained_slice_group_set( payloadSize ) 5
else if( payloadType  ==  19 )   
film_grain_characteristics( payloadSize ) 5
else if( payloadType  ==  20 )   
deblocking_filter_display_preference( payloadSize ) 5
else if( payloadType  ==  21 )   
stereo_video_info( payloadSize ) 5
else if( payloadType  ==  22 )   
post_filter_hint( payloadSize ) 5
else if( payloadType  ==  23 )   
tone_mapping_info( payloadSize ) 5
else if( payloadType  ==  24 )   
scalability_info( payloadSize )  /* specified in Annex G *//* 5
else if( payloadType  ==  25 )   
sub_pic_scalable_layer( payloadSize )  /* specified in Annex G *//* 5
else if( payloadType  ==  26 )   
non_required_layer_rep( payloadSize )  /* specified in Annex G *//* 5
else if( payloadType  ==  27 )   
priority_layer_info( payloadSize )  /* specified in Annex G *//* 5
else if( payloadType  ==  28 )   
layers_not_present( payloadSize )  /* specified in Annex G *//* 5
else if( payloadType  ==  29 )   
layer_dependency_change( payloadSize )  /* specified in Annex G *//* 5
else if( payloadType  ==  30 )   
scalable_nesting( payloadSize )  /* specified in Annex G *//* 5
else if( payloadType  ==  31 )   
base_layer_temporal_hrd( payloadSize )  /* specified in Annex G *//* 5
else if( payloadType  ==  32 )   
quality_layer_integrity_check( payloadSize )  /* specified in Annex G *//* 5
else if( payloadType  ==  33 )   
redundant_pic_property( payloadSize )  /* specified in Annex G *//* 5
else if( payloadType  ==  34 )   
tl0_dep_rep_index( payloadSize )  /* specified in Annex G *//* 5
else if( payloadType  ==  35 )   
tl_switching_point( payloadSize )  /* specified in Annex G *//* 5
else if( payloadType  ==  36 )   
parallel_decoding_info( payloadSize )  /* specified in Annex H *//* 5
else if( payloadType  ==  37 )   
 mvc_scalable_nesting( payloadSize )  /* specified in Annex H *//* 5  
else if( payloadType  ==  38 )   
 view_scalability_info( payloadSize )  /* specified in Annex H *//* 5  
else if( payloadType  ==  39 )   
 multiview_scene_info( payloadSize )  /* specified in Annex H *//* 5  
else if( payloadType  ==  40 )   
 multiview_acquisition_info( payloadSize )  /* specified in Annex H *//* 5  
else if( payloadType  ==  41 )   
 non_required_view_component( payloadSize )  /* specified in Annex H *//* 5  
else if( payloadType  ==  42 )   
 view_dependency_change( payloadSize )  /* specified in Annex H *//* 5  
else if( payloadType  ==  43 )   
 operation_point_not_present( payloadSize )  /* specified in Annex H *//* 5  
else if( payloadType  ==  44 )   
 base_view_temporal_hrd( payloadSize )  /* specified in Annex H *//* 5  
else if( payloadType  ==  45 )   
 frame_packing_arrangement( payloadSize ) 5  
else if( payloadType  ==  46 )   
 multiview_view_position( payloadSize )  /* specified in Annex H *//* 5  
else if( payloadType  ==  47 )   
 display_orientation( payloadSize ) 5  
else if( payloadType  ==  48 )   
 mvcd_scalable_nesting( payloadSize )  /* specified in Annex I *//* 5  
else if( payloadType  ==  49 )   
 mvcd_view_scalability_info( payloadSize )  /* specified in Annex I *//* 5  
else if( payloadType  ==  50 )   
 depth_representation_info( payloadSize )  /* specified in Annex I *//* 5  
else if( payloadType  ==  51 )   
 three_dimensional_reference_displays_info( payloadSize )  /* specified in Annex I *//* 5
else if( payloadType  ==  52 )   
 depth_timing( payloadSize )  /* specified in Annex I *//* 5  
else if( payloadType  ==  53 )   
 depth_sampling_info( payloadSize )  /* specified in Annex I *//* 5  
else if( payloadType  ==  54 )   
 constrained_depth_parameter_set_identifier( payloadSize ) /* specified in Annex J *//* 5  
else if( payloadType  ==  56 )   
 green_metadata( payloadSize )  /* specified in ISO/IEC 23001-11 *//* 5  
else if( payloadType  ==  137 )   
 mastering_display_colour_volume( payloadSize ) 5  
else if( payloadType  ==  142 )   
 colour_remapping_info( payloadSize ) 5  
else if( payloadType  ==  144 )   
 content_light_level_info( payloadSize ) 5  
else if( payloadType  ==  147 )   
 alternative_transfer_characteristics( payloadSize ) 5  
else if( payloadType  ==  148 )   
ambient_viewing_environment( payloadSize ) 5  
else if( payloadType  ==  149 )   
content_colour_volume( payloadSize ) 
else if( payloadType  ==  150 )   
equirectangular_projection( payloadSize ) 5
else if( payloadType  ==  151 )   
cubemap_projection( payloadSize ) 5
else if( payloadType  ==  154 )   
sphere_rotation( payloadSize ) 5
else if( payloadType  ==  155 )   
regionwise_packing( payloadSize ) 5
else if( payloadType  ==  156 )   
omni_viewport( payloadSize ) 5
else if( payloadType  ==  181 )   
alternative_depth_info( payloadSize )  /* specified in Annex I *//* 5
else if( payloadType  ==  200 )   
sei_manifest( payloadSize ) 5
else if( payloadType  ==  201 )   
sei_prefix_indication( payloadSize ) 5
else if( payloadType  ==  202 )   
annotated_regions( payloadSize ) 5
else if( payloadType  ==  205 )   
shutter_interval_info( payloadSize ) 5
else   
reserved_sei_message( payloadSize ) 5
if( !byte_aligned() ) {   
bit_equal_to_one  /* equal to 1 *//* 5 f(1) 
while( !byte_aligned() ) 
bit_equal_to_zero  /* equal to 0 *//* 5 f(1)
}   
}
    */
    public class SeiPayload : IItuSerializable
    {
        private uint payloadType;
        public uint PayloadType { get { return payloadType; } set { payloadType = value; } }
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private BufferingPeriod buffering_period;
        public BufferingPeriod BufferingPeriod { get { return buffering_period; } set { buffering_period = value; } }
        private PicTiming pic_timing;
        public PicTiming PicTiming { get { return pic_timing; } set { pic_timing = value; } }
        private PanScanRect pan_scan_rect;
        public PanScanRect PanScanRect { get { return pan_scan_rect; } set { pan_scan_rect = value; } }
        private FillerPayload filler_payload;
        public FillerPayload FillerPayload { get { return filler_payload; } set { filler_payload = value; } }
        private UserDataRegisteredItutT35 user_data_registered_itu_t_t35;
        public UserDataRegisteredItutT35 UserDataRegisteredItutT35 { get { return user_data_registered_itu_t_t35; } set { user_data_registered_itu_t_t35 = value; } }
        private UserDataUnregistered user_data_unregistered;
        public UserDataUnregistered UserDataUnregistered { get { return user_data_unregistered; } set { user_data_unregistered = value; } }
        private RecoveryPoint recovery_point;
        public RecoveryPoint RecoveryPoint { get { return recovery_point; } set { recovery_point = value; } }
        private DecRefPicMarkingRepetition dec_ref_pic_marking_repetition;
        public DecRefPicMarkingRepetition DecRefPicMarkingRepetition { get { return dec_ref_pic_marking_repetition; } set { dec_ref_pic_marking_repetition = value; } }
        private SparePic spare_pic;
        public SparePic SparePic { get { return spare_pic; } set { spare_pic = value; } }
        private SceneInfo scene_info;
        public SceneInfo SceneInfo { get { return scene_info; } set { scene_info = value; } }
        private SubSeqInfo sub_seq_info;
        public SubSeqInfo SubSeqInfo { get { return sub_seq_info; } set { sub_seq_info = value; } }
        private SubSeqLayerCharacteristics sub_seq_layer_characteristics;
        public SubSeqLayerCharacteristics SubSeqLayerCharacteristics { get { return sub_seq_layer_characteristics; } set { sub_seq_layer_characteristics = value; } }
        private SubSeqCharacteristics sub_seq_characteristics;
        public SubSeqCharacteristics SubSeqCharacteristics { get { return sub_seq_characteristics; } set { sub_seq_characteristics = value; } }
        private FullFrameFreeze full_frame_freeze;
        public FullFrameFreeze FullFrameFreeze { get { return full_frame_freeze; } set { full_frame_freeze = value; } }
        private FullFrameFreezeRelease full_frame_freeze_release;
        public FullFrameFreezeRelease FullFrameFreezeRelease { get { return full_frame_freeze_release; } set { full_frame_freeze_release = value; } }
        private FullFrameSnapshot full_frame_snapshot;
        public FullFrameSnapshot FullFrameSnapshot { get { return full_frame_snapshot; } set { full_frame_snapshot = value; } }
        private ProgressiveRefinementSegmentStart progressive_refinement_segment_start;
        public ProgressiveRefinementSegmentStart ProgressiveRefinementSegmentStart { get { return progressive_refinement_segment_start; } set { progressive_refinement_segment_start = value; } }
        private ProgressiveRefinementSegmentEnd progressive_refinement_segment_end;
        public ProgressiveRefinementSegmentEnd ProgressiveRefinementSegmentEnd { get { return progressive_refinement_segment_end; } set { progressive_refinement_segment_end = value; } }
        private MotionConstrainedSliceGroupSet motion_constrained_slice_group_set;
        public MotionConstrainedSliceGroupSet MotionConstrainedSliceGroupSet { get { return motion_constrained_slice_group_set; } set { motion_constrained_slice_group_set = value; } }
        private FilmGrainCharacteristics film_grain_characteristics;
        public FilmGrainCharacteristics FilmGrainCharacteristics { get { return film_grain_characteristics; } set { film_grain_characteristics = value; } }
        private DeblockingFilterDisplayPreference deblocking_filter_display_preference;
        public DeblockingFilterDisplayPreference DeblockingFilterDisplayPreference { get { return deblocking_filter_display_preference; } set { deblocking_filter_display_preference = value; } }
        private StereoVideoInfo stereo_video_info;
        public StereoVideoInfo StereoVideoInfo { get { return stereo_video_info; } set { stereo_video_info = value; } }
        private PostFilterHint post_filter_hint;
        public PostFilterHint PostFilterHint { get { return post_filter_hint; } set { post_filter_hint = value; } }
        private ToneMappingInfo tone_mapping_info;
        public ToneMappingInfo ToneMappingInfo { get { return tone_mapping_info; } set { tone_mapping_info = value; } }
        private ScalabilityInfo scalability_info;
        public ScalabilityInfo ScalabilityInfo { get { return scalability_info; } set { scalability_info = value; } }
        private SubPicScalableLayer sub_pic_scalable_layer;
        public SubPicScalableLayer SubPicScalableLayer { get { return sub_pic_scalable_layer; } set { sub_pic_scalable_layer = value; } }
        private NonRequiredLayerRep non_required_layer_rep;
        public NonRequiredLayerRep NonRequiredLayerRep { get { return non_required_layer_rep; } set { non_required_layer_rep = value; } }
        private PriorityLayerInfo priority_layer_info;
        public PriorityLayerInfo PriorityLayerInfo { get { return priority_layer_info; } set { priority_layer_info = value; } }
        private LayersNotPresent layers_not_present;
        public LayersNotPresent LayersNotPresent { get { return layers_not_present; } set { layers_not_present = value; } }
        private LayerDependencyChange layer_dependency_change;
        public LayerDependencyChange LayerDependencyChange { get { return layer_dependency_change; } set { layer_dependency_change = value; } }
        private ScalableNesting scalable_nesting;
        public ScalableNesting ScalableNesting { get { return scalable_nesting; } set { scalable_nesting = value; } }
        private BaseLayerTemporalHrd base_layer_temporal_hrd;
        public BaseLayerTemporalHrd BaseLayerTemporalHrd { get { return base_layer_temporal_hrd; } set { base_layer_temporal_hrd = value; } }
        private QualityLayerIntegrityCheck quality_layer_integrity_check;
        public QualityLayerIntegrityCheck QualityLayerIntegrityCheck { get { return quality_layer_integrity_check; } set { quality_layer_integrity_check = value; } }
        private RedundantPicProperty redundant_pic_property;
        public RedundantPicProperty RedundantPicProperty { get { return redundant_pic_property; } set { redundant_pic_property = value; } }
        private Tl0DepRepIndex tl0_dep_rep_index;
        public Tl0DepRepIndex Tl0DepRepIndex { get { return tl0_dep_rep_index; } set { tl0_dep_rep_index = value; } }
        private TlSwitchingPoint tl_switching_point;
        public TlSwitchingPoint TlSwitchingPoint { get { return tl_switching_point; } set { tl_switching_point = value; } }
        private ParallelDecodingInfo parallel_decoding_info;
        public ParallelDecodingInfo ParallelDecodingInfo { get { return parallel_decoding_info; } set { parallel_decoding_info = value; } }
        private MvcScalableNesting mvc_scalable_nesting;
        public MvcScalableNesting MvcScalableNesting { get { return mvc_scalable_nesting; } set { mvc_scalable_nesting = value; } }
        private ViewScalabilityInfo view_scalability_info;
        public ViewScalabilityInfo ViewScalabilityInfo { get { return view_scalability_info; } set { view_scalability_info = value; } }
        private MultiviewSceneInfo multiview_scene_info;
        public MultiviewSceneInfo MultiviewSceneInfo { get { return multiview_scene_info; } set { multiview_scene_info = value; } }
        private MultiviewAcquisitionInfo multiview_acquisition_info;
        public MultiviewAcquisitionInfo MultiviewAcquisitionInfo { get { return multiview_acquisition_info; } set { multiview_acquisition_info = value; } }
        private NonRequiredViewComponent non_required_view_component;
        public NonRequiredViewComponent NonRequiredViewComponent { get { return non_required_view_component; } set { non_required_view_component = value; } }
        private ViewDependencyChange view_dependency_change;
        public ViewDependencyChange ViewDependencyChange { get { return view_dependency_change; } set { view_dependency_change = value; } }
        private OperationPointNotPresent operation_point_not_present;
        public OperationPointNotPresent OperationPointNotPresent { get { return operation_point_not_present; } set { operation_point_not_present = value; } }
        private BaseViewTemporalHrd base_view_temporal_hrd;
        public BaseViewTemporalHrd BaseViewTemporalHrd { get { return base_view_temporal_hrd; } set { base_view_temporal_hrd = value; } }
        private FramePackingArrangement frame_packing_arrangement;
        public FramePackingArrangement FramePackingArrangement { get { return frame_packing_arrangement; } set { frame_packing_arrangement = value; } }
        private MultiviewViewPosition multiview_view_position;
        public MultiviewViewPosition MultiviewViewPosition { get { return multiview_view_position; } set { multiview_view_position = value; } }
        private DisplayOrientation display_orientation;
        public DisplayOrientation DisplayOrientation { get { return display_orientation; } set { display_orientation = value; } }
        private MvcdScalableNesting mvcd_scalable_nesting;
        public MvcdScalableNesting MvcdScalableNesting { get { return mvcd_scalable_nesting; } set { mvcd_scalable_nesting = value; } }
        private MvcdViewScalabilityInfo mvcd_view_scalability_info;
        public MvcdViewScalabilityInfo MvcdViewScalabilityInfo { get { return mvcd_view_scalability_info; } set { mvcd_view_scalability_info = value; } }
        private DepthRepresentationInfo depth_representation_info;
        public DepthRepresentationInfo DepthRepresentationInfo { get { return depth_representation_info; } set { depth_representation_info = value; } }
        private ThreeDimensionalReferenceDisplaysInfo three_dimensional_reference_displays_info;
        public ThreeDimensionalReferenceDisplaysInfo ThreeDimensionalReferenceDisplaysInfo { get { return three_dimensional_reference_displays_info; } set { three_dimensional_reference_displays_info = value; } }
        private DepthTiming depth_timing;
        public DepthTiming DepthTiming { get { return depth_timing; } set { depth_timing = value; } }
        private DepthSamplingInfo depth_sampling_info;
        public DepthSamplingInfo DepthSamplingInfo { get { return depth_sampling_info; } set { depth_sampling_info = value; } }
        private ConstrainedDepthParameterSetIdentifier constrained_depth_parameter_set_identifier;
        public ConstrainedDepthParameterSetIdentifier ConstrainedDepthParameterSetIdentifier { get { return constrained_depth_parameter_set_identifier; } set { constrained_depth_parameter_set_identifier = value; } }
        private GreenMetadata green_metadata;
        public GreenMetadata GreenMetadata { get { return green_metadata; } set { green_metadata = value; } }
        private MasteringDisplayColourVolume mastering_display_colour_volume;
        public MasteringDisplayColourVolume MasteringDisplayColourVolume { get { return mastering_display_colour_volume; } set { mastering_display_colour_volume = value; } }
        private ColourRemappingInfo colour_remapping_info;
        public ColourRemappingInfo ColourRemappingInfo { get { return colour_remapping_info; } set { colour_remapping_info = value; } }
        private ContentLightLevelInfo content_light_level_info;
        public ContentLightLevelInfo ContentLightLevelInfo { get { return content_light_level_info; } set { content_light_level_info = value; } }
        private AlternativeTransferCharacteristics alternative_transfer_characteristics;
        public AlternativeTransferCharacteristics AlternativeTransferCharacteristics { get { return alternative_transfer_characteristics; } set { alternative_transfer_characteristics = value; } }
        private AmbientViewingEnvironment ambient_viewing_environment;
        public AmbientViewingEnvironment AmbientViewingEnvironment { get { return ambient_viewing_environment; } set { ambient_viewing_environment = value; } }
        private ContentColourVolume content_colour_volume;
        public ContentColourVolume ContentColourVolume { get { return content_colour_volume; } set { content_colour_volume = value; } }
        private EquirectangularProjection equirectangular_projection;
        public EquirectangularProjection EquirectangularProjection { get { return equirectangular_projection; } set { equirectangular_projection = value; } }
        private CubemapProjection cubemap_projection;
        public CubemapProjection CubemapProjection { get { return cubemap_projection; } set { cubemap_projection = value; } }
        private SphereRotation sphere_rotation;
        public SphereRotation SphereRotation { get { return sphere_rotation; } set { sphere_rotation = value; } }
        private RegionwisePacking regionwise_packing;
        public RegionwisePacking RegionwisePacking { get { return regionwise_packing; } set { regionwise_packing = value; } }
        private OmniViewport omni_viewport;
        public OmniViewport OmniViewport { get { return omni_viewport; } set { omni_viewport = value; } }
        private AlternativeDepthInfo alternative_depth_info;
        public AlternativeDepthInfo AlternativeDepthInfo { get { return alternative_depth_info; } set { alternative_depth_info = value; } }
        private SeiManifest sei_manifest;
        public SeiManifest SeiManifest { get { return sei_manifest; } set { sei_manifest = value; } }
        private SeiPrefixIndication sei_prefix_indication;
        public SeiPrefixIndication SeiPrefixIndication { get { return sei_prefix_indication; } set { sei_prefix_indication = value; } }
        private AnnotatedRegions annotated_regions;
        public AnnotatedRegions AnnotatedRegions { get { return annotated_regions; } set { annotated_regions = value; } }
        private ShutterIntervalInfo shutter_interval_info;
        public ShutterIntervalInfo ShutterIntervalInfo { get { return shutter_interval_info; } set { shutter_interval_info = value; } }
        private ReservedSeiMessage reserved_sei_message;
        public ReservedSeiMessage ReservedSeiMessage { get { return reserved_sei_message; } set { reserved_sei_message = value; } }
        private uint bit_equal_to_one;
        public uint BitEqualToOne { get { return bit_equal_to_one; } set { bit_equal_to_one = value; } }
        private uint bit_equal_to_zero;
        public uint BitEqualToZero { get { return bit_equal_to_zero; } set { bit_equal_to_zero = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SeiPayload(uint payloadType, uint payloadSize)
        {
            this.payloadType = payloadType;
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;


            if (payloadType == 0)
            {
                size += stream.ReadClass<BufferingPeriod>(size, context, () => new BufferingPeriod(payloadSize), out this.buffering_period);
            }
            else if (payloadType == 1)
            {
                size += stream.ReadClass<PicTiming>(size, context, () => new PicTiming(payloadSize), out this.pic_timing);
            }
            else if (payloadType == 2)
            {
                size += stream.ReadClass<PanScanRect>(size, context, () => new PanScanRect(payloadSize), out this.pan_scan_rect);
            }
            else if (payloadType == 3)
            {
                size += stream.ReadClass<FillerPayload>(size, context, () => new FillerPayload(payloadSize), out this.filler_payload);
            }
            else if (payloadType == 4)
            {
                size += stream.ReadClass<UserDataRegisteredItutT35>(size, context, () => new UserDataRegisteredItutT35(payloadSize), out this.user_data_registered_itu_t_t35);
            }
            else if (payloadType == 5)
            {
                size += stream.ReadClass<UserDataUnregistered>(size, context, () => new UserDataUnregistered(payloadSize), out this.user_data_unregistered);
            }
            else if (payloadType == 6)
            {
                size += stream.ReadClass<RecoveryPoint>(size, context, () => new RecoveryPoint(payloadSize), out this.recovery_point);
            }
            else if (payloadType == 7)
            {
                size += stream.ReadClass<DecRefPicMarkingRepetition>(size, context, () => new DecRefPicMarkingRepetition(payloadSize), out this.dec_ref_pic_marking_repetition);
            }
            else if (payloadType == 8)
            {
                size += stream.ReadClass<SparePic>(size, context, () => new SparePic(payloadSize), out this.spare_pic);
            }
            else if (payloadType == 9)
            {
                size += stream.ReadClass<SceneInfo>(size, context, () => new SceneInfo(payloadSize), out this.scene_info);
            }
            else if (payloadType == 10)
            {
                size += stream.ReadClass<SubSeqInfo>(size, context, () => new SubSeqInfo(payloadSize), out this.sub_seq_info);
            }
            else if (payloadType == 11)
            {
                size += stream.ReadClass<SubSeqLayerCharacteristics>(size, context, () => new SubSeqLayerCharacteristics(payloadSize), out this.sub_seq_layer_characteristics);
            }
            else if (payloadType == 12)
            {
                size += stream.ReadClass<SubSeqCharacteristics>(size, context, () => new SubSeqCharacteristics(payloadSize), out this.sub_seq_characteristics);
            }
            else if (payloadType == 13)
            {
                size += stream.ReadClass<FullFrameFreeze>(size, context, () => new FullFrameFreeze(payloadSize), out this.full_frame_freeze);
            }
            else if (payloadType == 14)
            {
                size += stream.ReadClass<FullFrameFreezeRelease>(size, context, () => new FullFrameFreezeRelease(payloadSize), out this.full_frame_freeze_release);
            }
            else if (payloadType == 15)
            {
                size += stream.ReadClass<FullFrameSnapshot>(size, context, () => new FullFrameSnapshot(payloadSize), out this.full_frame_snapshot);
            }
            else if (payloadType == 16)
            {
                size += stream.ReadClass<ProgressiveRefinementSegmentStart>(size, context, () => new ProgressiveRefinementSegmentStart(payloadSize), out this.progressive_refinement_segment_start);
            }
            else if (payloadType == 17)
            {
                size += stream.ReadClass<ProgressiveRefinementSegmentEnd>(size, context, () => new ProgressiveRefinementSegmentEnd(payloadSize), out this.progressive_refinement_segment_end);
            }
            else if (payloadType == 18)
            {
                size += stream.ReadClass<MotionConstrainedSliceGroupSet>(size, context, () => new MotionConstrainedSliceGroupSet(payloadSize), out this.motion_constrained_slice_group_set);
            }
            else if (payloadType == 19)
            {
                size += stream.ReadClass<FilmGrainCharacteristics>(size, context, () => new FilmGrainCharacteristics(payloadSize), out this.film_grain_characteristics);
            }
            else if (payloadType == 20)
            {
                size += stream.ReadClass<DeblockingFilterDisplayPreference>(size, context, () => new DeblockingFilterDisplayPreference(payloadSize), out this.deblocking_filter_display_preference);
            }
            else if (payloadType == 21)
            {
                size += stream.ReadClass<StereoVideoInfo>(size, context, () => new StereoVideoInfo(payloadSize), out this.stereo_video_info);
            }
            else if (payloadType == 22)
            {
                size += stream.ReadClass<PostFilterHint>(size, context, () => new PostFilterHint(payloadSize), out this.post_filter_hint);
            }
            else if (payloadType == 23)
            {
                size += stream.ReadClass<ToneMappingInfo>(size, context, () => new ToneMappingInfo(payloadSize), out this.tone_mapping_info);
            }
            else if (payloadType == 24)
            {
                size += stream.ReadClass<ScalabilityInfo>(size, context, () => new ScalabilityInfo(payloadSize), out this.scalability_info); // specified in Annex G 
            }
            else if (payloadType == 25)
            {
                size += stream.ReadClass<SubPicScalableLayer>(size, context, () => new SubPicScalableLayer(payloadSize), out this.sub_pic_scalable_layer); // specified in Annex G 
            }
            else if (payloadType == 26)
            {
                size += stream.ReadClass<NonRequiredLayerRep>(size, context, () => new NonRequiredLayerRep(payloadSize), out this.non_required_layer_rep); // specified in Annex G 
            }
            else if (payloadType == 27)
            {
                size += stream.ReadClass<PriorityLayerInfo>(size, context, () => new PriorityLayerInfo(payloadSize), out this.priority_layer_info); // specified in Annex G 
            }
            else if (payloadType == 28)
            {
                size += stream.ReadClass<LayersNotPresent>(size, context, () => new LayersNotPresent(payloadSize), out this.layers_not_present); // specified in Annex G 
            }
            else if (payloadType == 29)
            {
                size += stream.ReadClass<LayerDependencyChange>(size, context, () => new LayerDependencyChange(payloadSize), out this.layer_dependency_change); // specified in Annex G 
            }
            else if (payloadType == 30)
            {
                size += stream.ReadClass<ScalableNesting>(size, context, () => new ScalableNesting(payloadSize), out this.scalable_nesting); // specified in Annex G 
            }
            else if (payloadType == 31)
            {
                size += stream.ReadClass<BaseLayerTemporalHrd>(size, context, () => new BaseLayerTemporalHrd(payloadSize), out this.base_layer_temporal_hrd); // specified in Annex G 
            }
            else if (payloadType == 32)
            {
                size += stream.ReadClass<QualityLayerIntegrityCheck>(size, context, () => new QualityLayerIntegrityCheck(payloadSize), out this.quality_layer_integrity_check); // specified in Annex G 
            }
            else if (payloadType == 33)
            {
                size += stream.ReadClass<RedundantPicProperty>(size, context, () => new RedundantPicProperty(payloadSize), out this.redundant_pic_property); // specified in Annex G 
            }
            else if (payloadType == 34)
            {
                size += stream.ReadClass<Tl0DepRepIndex>(size, context, () => new Tl0DepRepIndex(payloadSize), out this.tl0_dep_rep_index); // specified in Annex G 
            }
            else if (payloadType == 35)
            {
                size += stream.ReadClass<TlSwitchingPoint>(size, context, () => new TlSwitchingPoint(payloadSize), out this.tl_switching_point); // specified in Annex G 
            }
            else if (payloadType == 36)
            {
                size += stream.ReadClass<ParallelDecodingInfo>(size, context, () => new ParallelDecodingInfo(payloadSize), out this.parallel_decoding_info); // specified in Annex H 
            }
            else if (payloadType == 37)
            {
                size += stream.ReadClass<MvcScalableNesting>(size, context, () => new MvcScalableNesting(payloadSize), out this.mvc_scalable_nesting); // specified in Annex H 
            }
            else if (payloadType == 38)
            {
                size += stream.ReadClass<ViewScalabilityInfo>(size, context, () => new ViewScalabilityInfo(payloadSize), out this.view_scalability_info); // specified in Annex H 
            }
            else if (payloadType == 39)
            {
                size += stream.ReadClass<MultiviewSceneInfo>(size, context, () => new MultiviewSceneInfo(payloadSize), out this.multiview_scene_info); // specified in Annex H 
            }
            else if (payloadType == 40)
            {
                size += stream.ReadClass<MultiviewAcquisitionInfo>(size, context, () => new MultiviewAcquisitionInfo(payloadSize), out this.multiview_acquisition_info); // specified in Annex H 
            }
            else if (payloadType == 41)
            {
                size += stream.ReadClass<NonRequiredViewComponent>(size, context, () => new NonRequiredViewComponent(payloadSize), out this.non_required_view_component); // specified in Annex H 
            }
            else if (payloadType == 42)
            {
                size += stream.ReadClass<ViewDependencyChange>(size, context, () => new ViewDependencyChange(payloadSize), out this.view_dependency_change); // specified in Annex H 
            }
            else if (payloadType == 43)
            {
                size += stream.ReadClass<OperationPointNotPresent>(size, context, () => new OperationPointNotPresent(payloadSize), out this.operation_point_not_present); // specified in Annex H 
            }
            else if (payloadType == 44)
            {
                size += stream.ReadClass<BaseViewTemporalHrd>(size, context, () => new BaseViewTemporalHrd(payloadSize), out this.base_view_temporal_hrd); // specified in Annex H 
            }
            else if (payloadType == 45)
            {
                size += stream.ReadClass<FramePackingArrangement>(size, context, () => new FramePackingArrangement(payloadSize), out this.frame_packing_arrangement);
            }
            else if (payloadType == 46)
            {
                size += stream.ReadClass<MultiviewViewPosition>(size, context, () => new MultiviewViewPosition(payloadSize), out this.multiview_view_position); // specified in Annex H 
            }
            else if (payloadType == 47)
            {
                size += stream.ReadClass<DisplayOrientation>(size, context, () => new DisplayOrientation(payloadSize), out this.display_orientation);
            }
            else if (payloadType == 48)
            {
                size += stream.ReadClass<MvcdScalableNesting>(size, context, () => new MvcdScalableNesting(payloadSize), out this.mvcd_scalable_nesting); // specified in Annex I 
            }
            else if (payloadType == 49)
            {
                size += stream.ReadClass<MvcdViewScalabilityInfo>(size, context, () => new MvcdViewScalabilityInfo(payloadSize), out this.mvcd_view_scalability_info); // specified in Annex I 
            }
            else if (payloadType == 50)
            {
                size += stream.ReadClass<DepthRepresentationInfo>(size, context, () => new DepthRepresentationInfo(payloadSize), out this.depth_representation_info); // specified in Annex I 
            }
            else if (payloadType == 51)
            {
                size += stream.ReadClass<ThreeDimensionalReferenceDisplaysInfo>(size, context, () => new ThreeDimensionalReferenceDisplaysInfo(payloadSize), out this.three_dimensional_reference_displays_info); // specified in Annex I 
            }
            else if (payloadType == 52)
            {
                size += stream.ReadClass<DepthTiming>(size, context, () => new DepthTiming(payloadSize), out this.depth_timing); // specified in Annex I 
            }
            else if (payloadType == 53)
            {
                size += stream.ReadClass<DepthSamplingInfo>(size, context, () => new DepthSamplingInfo(payloadSize), out this.depth_sampling_info); // specified in Annex I 
            }
            else if (payloadType == 54)
            {
                size += stream.ReadClass<ConstrainedDepthParameterSetIdentifier>(size, context, () => new ConstrainedDepthParameterSetIdentifier(payloadSize), out this.constrained_depth_parameter_set_identifier); // specified in Annex J 
            }
            else if (payloadType == 56)
            {
                size += stream.ReadClass<GreenMetadata>(size, context, () => new GreenMetadata(payloadSize), out this.green_metadata); // specified in ISO/IEC 23001-11 
            }
            else if (payloadType == 137)
            {
                size += stream.ReadClass<MasteringDisplayColourVolume>(size, context, () => new MasteringDisplayColourVolume(payloadSize), out this.mastering_display_colour_volume);
            }
            else if (payloadType == 142)
            {
                size += stream.ReadClass<ColourRemappingInfo>(size, context, () => new ColourRemappingInfo(payloadSize), out this.colour_remapping_info);
            }
            else if (payloadType == 144)
            {
                size += stream.ReadClass<ContentLightLevelInfo>(size, context, () => new ContentLightLevelInfo(payloadSize), out this.content_light_level_info);
            }
            else if (payloadType == 147)
            {
                size += stream.ReadClass<AlternativeTransferCharacteristics>(size, context, () => new AlternativeTransferCharacteristics(payloadSize), out this.alternative_transfer_characteristics);
            }
            else if (payloadType == 148)
            {
                size += stream.ReadClass<AmbientViewingEnvironment>(size, context, () => new AmbientViewingEnvironment(payloadSize), out this.ambient_viewing_environment);
            }
            else if (payloadType == 149)
            {
                size += stream.ReadClass<ContentColourVolume>(size, context, () => new ContentColourVolume(payloadSize), out this.content_colour_volume);
            }
            else if (payloadType == 150)
            {
                size += stream.ReadClass<EquirectangularProjection>(size, context, () => new EquirectangularProjection(payloadSize), out this.equirectangular_projection);
            }
            else if (payloadType == 151)
            {
                size += stream.ReadClass<CubemapProjection>(size, context, () => new CubemapProjection(payloadSize), out this.cubemap_projection);
            }
            else if (payloadType == 154)
            {
                size += stream.ReadClass<SphereRotation>(size, context, () => new SphereRotation(payloadSize), out this.sphere_rotation);
            }
            else if (payloadType == 155)
            {
                size += stream.ReadClass<RegionwisePacking>(size, context, () => new RegionwisePacking(payloadSize), out this.regionwise_packing);
            }
            else if (payloadType == 156)
            {
                size += stream.ReadClass<OmniViewport>(size, context, () => new OmniViewport(payloadSize), out this.omni_viewport);
            }
            else if (payloadType == 181)
            {
                size += stream.ReadClass<AlternativeDepthInfo>(size, context, () => new AlternativeDepthInfo(payloadSize), out this.alternative_depth_info); // specified in Annex I 
            }
            else if (payloadType == 200)
            {
                size += stream.ReadClass<SeiManifest>(size, context, () => new SeiManifest(payloadSize), out this.sei_manifest);
            }
            else if (payloadType == 201)
            {
                size += stream.ReadClass<SeiPrefixIndication>(size, context, () => new SeiPrefixIndication(payloadSize), out this.sei_prefix_indication);
            }
            else if (payloadType == 202)
            {
                size += stream.ReadClass<AnnotatedRegions>(size, context, () => new AnnotatedRegions(payloadSize), out this.annotated_regions);
            }
            else if (payloadType == 205)
            {
                size += stream.ReadClass<ShutterIntervalInfo>(size, context, () => new ShutterIntervalInfo(payloadSize), out this.shutter_interval_info);
            }
            else
            {
                size += stream.ReadClass<ReservedSeiMessage>(size, context, () => new ReservedSeiMessage(payloadSize), out this.reserved_sei_message);
            }

            if (!stream.ByteAligned())
            {
                size += stream.ReadFixed(size, 1, out this.bit_equal_to_one); // equal to 1 

                while (!stream.ByteAligned())
                {
                    size += stream.ReadFixed(size, 1, out this.bit_equal_to_zero); // equal to 0 
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;


            if (payloadType == 0)
            {
                size += stream.WriteClass<BufferingPeriod>(context, this.buffering_period);
            }
            else if (payloadType == 1)
            {
                size += stream.WriteClass<PicTiming>(context, this.pic_timing);
            }
            else if (payloadType == 2)
            {
                size += stream.WriteClass<PanScanRect>(context, this.pan_scan_rect);
            }
            else if (payloadType == 3)
            {
                size += stream.WriteClass<FillerPayload>(context, this.filler_payload);
            }
            else if (payloadType == 4)
            {
                size += stream.WriteClass<UserDataRegisteredItutT35>(context, this.user_data_registered_itu_t_t35);
            }
            else if (payloadType == 5)
            {
                size += stream.WriteClass<UserDataUnregistered>(context, this.user_data_unregistered);
            }
            else if (payloadType == 6)
            {
                size += stream.WriteClass<RecoveryPoint>(context, this.recovery_point);
            }
            else if (payloadType == 7)
            {
                size += stream.WriteClass<DecRefPicMarkingRepetition>(context, this.dec_ref_pic_marking_repetition);
            }
            else if (payloadType == 8)
            {
                size += stream.WriteClass<SparePic>(context, this.spare_pic);
            }
            else if (payloadType == 9)
            {
                size += stream.WriteClass<SceneInfo>(context, this.scene_info);
            }
            else if (payloadType == 10)
            {
                size += stream.WriteClass<SubSeqInfo>(context, this.sub_seq_info);
            }
            else if (payloadType == 11)
            {
                size += stream.WriteClass<SubSeqLayerCharacteristics>(context, this.sub_seq_layer_characteristics);
            }
            else if (payloadType == 12)
            {
                size += stream.WriteClass<SubSeqCharacteristics>(context, this.sub_seq_characteristics);
            }
            else if (payloadType == 13)
            {
                size += stream.WriteClass<FullFrameFreeze>(context, this.full_frame_freeze);
            }
            else if (payloadType == 14)
            {
                size += stream.WriteClass<FullFrameFreezeRelease>(context, this.full_frame_freeze_release);
            }
            else if (payloadType == 15)
            {
                size += stream.WriteClass<FullFrameSnapshot>(context, this.full_frame_snapshot);
            }
            else if (payloadType == 16)
            {
                size += stream.WriteClass<ProgressiveRefinementSegmentStart>(context, this.progressive_refinement_segment_start);
            }
            else if (payloadType == 17)
            {
                size += stream.WriteClass<ProgressiveRefinementSegmentEnd>(context, this.progressive_refinement_segment_end);
            }
            else if (payloadType == 18)
            {
                size += stream.WriteClass<MotionConstrainedSliceGroupSet>(context, this.motion_constrained_slice_group_set);
            }
            else if (payloadType == 19)
            {
                size += stream.WriteClass<FilmGrainCharacteristics>(context, this.film_grain_characteristics);
            }
            else if (payloadType == 20)
            {
                size += stream.WriteClass<DeblockingFilterDisplayPreference>(context, this.deblocking_filter_display_preference);
            }
            else if (payloadType == 21)
            {
                size += stream.WriteClass<StereoVideoInfo>(context, this.stereo_video_info);
            }
            else if (payloadType == 22)
            {
                size += stream.WriteClass<PostFilterHint>(context, this.post_filter_hint);
            }
            else if (payloadType == 23)
            {
                size += stream.WriteClass<ToneMappingInfo>(context, this.tone_mapping_info);
            }
            else if (payloadType == 24)
            {
                size += stream.WriteClass<ScalabilityInfo>(context, this.scalability_info); // specified in Annex G 
            }
            else if (payloadType == 25)
            {
                size += stream.WriteClass<SubPicScalableLayer>(context, this.sub_pic_scalable_layer); // specified in Annex G 
            }
            else if (payloadType == 26)
            {
                size += stream.WriteClass<NonRequiredLayerRep>(context, this.non_required_layer_rep); // specified in Annex G 
            }
            else if (payloadType == 27)
            {
                size += stream.WriteClass<PriorityLayerInfo>(context, this.priority_layer_info); // specified in Annex G 
            }
            else if (payloadType == 28)
            {
                size += stream.WriteClass<LayersNotPresent>(context, this.layers_not_present); // specified in Annex G 
            }
            else if (payloadType == 29)
            {
                size += stream.WriteClass<LayerDependencyChange>(context, this.layer_dependency_change); // specified in Annex G 
            }
            else if (payloadType == 30)
            {
                size += stream.WriteClass<ScalableNesting>(context, this.scalable_nesting); // specified in Annex G 
            }
            else if (payloadType == 31)
            {
                size += stream.WriteClass<BaseLayerTemporalHrd>(context, this.base_layer_temporal_hrd); // specified in Annex G 
            }
            else if (payloadType == 32)
            {
                size += stream.WriteClass<QualityLayerIntegrityCheck>(context, this.quality_layer_integrity_check); // specified in Annex G 
            }
            else if (payloadType == 33)
            {
                size += stream.WriteClass<RedundantPicProperty>(context, this.redundant_pic_property); // specified in Annex G 
            }
            else if (payloadType == 34)
            {
                size += stream.WriteClass<Tl0DepRepIndex>(context, this.tl0_dep_rep_index); // specified in Annex G 
            }
            else if (payloadType == 35)
            {
                size += stream.WriteClass<TlSwitchingPoint>(context, this.tl_switching_point); // specified in Annex G 
            }
            else if (payloadType == 36)
            {
                size += stream.WriteClass<ParallelDecodingInfo>(context, this.parallel_decoding_info); // specified in Annex H 
            }
            else if (payloadType == 37)
            {
                size += stream.WriteClass<MvcScalableNesting>(context, this.mvc_scalable_nesting); // specified in Annex H 
            }
            else if (payloadType == 38)
            {
                size += stream.WriteClass<ViewScalabilityInfo>(context, this.view_scalability_info); // specified in Annex H 
            }
            else if (payloadType == 39)
            {
                size += stream.WriteClass<MultiviewSceneInfo>(context, this.multiview_scene_info); // specified in Annex H 
            }
            else if (payloadType == 40)
            {
                size += stream.WriteClass<MultiviewAcquisitionInfo>(context, this.multiview_acquisition_info); // specified in Annex H 
            }
            else if (payloadType == 41)
            {
                size += stream.WriteClass<NonRequiredViewComponent>(context, this.non_required_view_component); // specified in Annex H 
            }
            else if (payloadType == 42)
            {
                size += stream.WriteClass<ViewDependencyChange>(context, this.view_dependency_change); // specified in Annex H 
            }
            else if (payloadType == 43)
            {
                size += stream.WriteClass<OperationPointNotPresent>(context, this.operation_point_not_present); // specified in Annex H 
            }
            else if (payloadType == 44)
            {
                size += stream.WriteClass<BaseViewTemporalHrd>(context, this.base_view_temporal_hrd); // specified in Annex H 
            }
            else if (payloadType == 45)
            {
                size += stream.WriteClass<FramePackingArrangement>(context, this.frame_packing_arrangement);
            }
            else if (payloadType == 46)
            {
                size += stream.WriteClass<MultiviewViewPosition>(context, this.multiview_view_position); // specified in Annex H 
            }
            else if (payloadType == 47)
            {
                size += stream.WriteClass<DisplayOrientation>(context, this.display_orientation);
            }
            else if (payloadType == 48)
            {
                size += stream.WriteClass<MvcdScalableNesting>(context, this.mvcd_scalable_nesting); // specified in Annex I 
            }
            else if (payloadType == 49)
            {
                size += stream.WriteClass<MvcdViewScalabilityInfo>(context, this.mvcd_view_scalability_info); // specified in Annex I 
            }
            else if (payloadType == 50)
            {
                size += stream.WriteClass<DepthRepresentationInfo>(context, this.depth_representation_info); // specified in Annex I 
            }
            else if (payloadType == 51)
            {
                size += stream.WriteClass<ThreeDimensionalReferenceDisplaysInfo>(context, this.three_dimensional_reference_displays_info); // specified in Annex I 
            }
            else if (payloadType == 52)
            {
                size += stream.WriteClass<DepthTiming>(context, this.depth_timing); // specified in Annex I 
            }
            else if (payloadType == 53)
            {
                size += stream.WriteClass<DepthSamplingInfo>(context, this.depth_sampling_info); // specified in Annex I 
            }
            else if (payloadType == 54)
            {
                size += stream.WriteClass<ConstrainedDepthParameterSetIdentifier>(context, this.constrained_depth_parameter_set_identifier); // specified in Annex J 
            }
            else if (payloadType == 56)
            {
                size += stream.WriteClass<GreenMetadata>(context, this.green_metadata); // specified in ISO/IEC 23001-11 
            }
            else if (payloadType == 137)
            {
                size += stream.WriteClass<MasteringDisplayColourVolume>(context, this.mastering_display_colour_volume);
            }
            else if (payloadType == 142)
            {
                size += stream.WriteClass<ColourRemappingInfo>(context, this.colour_remapping_info);
            }
            else if (payloadType == 144)
            {
                size += stream.WriteClass<ContentLightLevelInfo>(context, this.content_light_level_info);
            }
            else if (payloadType == 147)
            {
                size += stream.WriteClass<AlternativeTransferCharacteristics>(context, this.alternative_transfer_characteristics);
            }
            else if (payloadType == 148)
            {
                size += stream.WriteClass<AmbientViewingEnvironment>(context, this.ambient_viewing_environment);
            }
            else if (payloadType == 149)
            {
                size += stream.WriteClass<ContentColourVolume>(context, this.content_colour_volume);
            }
            else if (payloadType == 150)
            {
                size += stream.WriteClass<EquirectangularProjection>(context, this.equirectangular_projection);
            }
            else if (payloadType == 151)
            {
                size += stream.WriteClass<CubemapProjection>(context, this.cubemap_projection);
            }
            else if (payloadType == 154)
            {
                size += stream.WriteClass<SphereRotation>(context, this.sphere_rotation);
            }
            else if (payloadType == 155)
            {
                size += stream.WriteClass<RegionwisePacking>(context, this.regionwise_packing);
            }
            else if (payloadType == 156)
            {
                size += stream.WriteClass<OmniViewport>(context, this.omni_viewport);
            }
            else if (payloadType == 181)
            {
                size += stream.WriteClass<AlternativeDepthInfo>(context, this.alternative_depth_info); // specified in Annex I 
            }
            else if (payloadType == 200)
            {
                size += stream.WriteClass<SeiManifest>(context, this.sei_manifest);
            }
            else if (payloadType == 201)
            {
                size += stream.WriteClass<SeiPrefixIndication>(context, this.sei_prefix_indication);
            }
            else if (payloadType == 202)
            {
                size += stream.WriteClass<AnnotatedRegions>(context, this.annotated_regions);
            }
            else if (payloadType == 205)
            {
                size += stream.WriteClass<ShutterIntervalInfo>(context, this.shutter_interval_info);
            }
            else
            {
                size += stream.WriteClass<ReservedSeiMessage>(context, this.reserved_sei_message);
            }

            if (!stream.ByteAligned())
            {
                size += stream.WriteFixed(1, this.bit_equal_to_one); // equal to 1 

                while (!stream.ByteAligned())
                {
                    size += stream.WriteFixed(1, this.bit_equal_to_zero); // equal to 0 
                }
            }

            return size;
        }

    }

    /*
  

buffering_period( payloadSize ) {  
 seq_parameter_set_id 5 ue(v) 
 if( NalHrdBpPresentFlag )   
  for( SchedSelIdx = 0; SchedSelIdx <= cpb_cnt_minus1; SchedSelIdx++ ) {   
   initial_cpb_removal_delay[ SchedSelIdx ] 5 u(v) 
   initial_cpb_removal_delay_offset[ SchedSelIdx ] 5 u(v) 
  }   
 if( VclHrdBpPresentFlag )   
  for( SchedSelIdx = 0; SchedSelIdx <= cpb_cnt_minus1; SchedSelIdx++ ) {   
   initial_cpb_removal_delay[ SchedSelIdx ] 5 u(v) 
   initial_cpb_removal_delay_offset[ SchedSelIdx ] 5 u(v) 
  }   
}
    */
    public class BufferingPeriod : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint seq_parameter_set_id;
        public uint SeqParameterSetId { get { return seq_parameter_set_id; } set { seq_parameter_set_id = value; } }
        private uint[] initial_cpb_removal_delay;
        public uint[] InitialCpbRemovalDelay { get { return initial_cpb_removal_delay; } set { initial_cpb_removal_delay = value; } }
        private uint[] initial_cpb_removal_delay_offset;
        public uint[] InitialCpbRemovalDelayOffset { get { return initial_cpb_removal_delay_offset; } set { initial_cpb_removal_delay_offset = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public BufferingPeriod(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint SchedSelIdx = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.seq_parameter_set_id);

            if (context.Sps.SeqParameterSetData.VuiParameters.NalHrdParametersPresentFlag != 0)
            {

                this.initial_cpb_removal_delay = new uint[context.Sps.SeqParameterSetData.VuiParameters.HrdParameters.CpbCntMinus1 + 1];
                this.initial_cpb_removal_delay_offset = new uint[context.Sps.SeqParameterSetData.VuiParameters.HrdParameters.CpbCntMinus1 + 1];
                for (SchedSelIdx = 0; SchedSelIdx <= context.Sps.SeqParameterSetData.VuiParameters.HrdParameters.CpbCntMinus1; SchedSelIdx++)
                {
                    size += stream.ReadUnsignedIntVariable(size, (context.Sps.SeqParameterSetData.VuiParameters.HrdParameters.InitialCpbRemovalDelayLengthMinus1 + 1), out this.initial_cpb_removal_delay[SchedSelIdx]);
                    size += stream.ReadUnsignedIntVariable(size, (context.Sps.SeqParameterSetData.VuiParameters.HrdParameters.InitialCpbRemovalDelayLengthMinus1 + 1), out this.initial_cpb_removal_delay_offset[SchedSelIdx]);
                }
            }

            if (context.Sps.SeqParameterSetData.VuiParameters.VclHrdParametersPresentFlag != 0)
            {

                for (SchedSelIdx = 0; SchedSelIdx <= context.Sps.SeqParameterSetData.VuiParameters.HrdParameters.CpbCntMinus1; SchedSelIdx++)
                {
                    size += stream.ReadUnsignedIntVariable(size, (context.Sps.SeqParameterSetData.VuiParameters.HrdParameters.InitialCpbRemovalDelayLengthMinus1 + 1), out this.initial_cpb_removal_delay[SchedSelIdx]);
                    size += stream.ReadUnsignedIntVariable(size, (context.Sps.SeqParameterSetData.VuiParameters.HrdParameters.InitialCpbRemovalDelayLengthMinus1 + 1), out this.initial_cpb_removal_delay_offset[SchedSelIdx]);
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint SchedSelIdx = 0;
            size += stream.WriteUnsignedIntGolomb(this.seq_parameter_set_id);

            if (context.Sps.SeqParameterSetData.VuiParameters.NalHrdParametersPresentFlag != 0)
            {

                for (SchedSelIdx = 0; SchedSelIdx <= context.Sps.SeqParameterSetData.VuiParameters.HrdParameters.CpbCntMinus1; SchedSelIdx++)
                {
                    size += stream.WriteUnsignedIntVariable((context.Sps.SeqParameterSetData.VuiParameters.HrdParameters.InitialCpbRemovalDelayLengthMinus1 + 1), this.initial_cpb_removal_delay[SchedSelIdx]);
                    size += stream.WriteUnsignedIntVariable((context.Sps.SeqParameterSetData.VuiParameters.HrdParameters.InitialCpbRemovalDelayLengthMinus1 + 1), this.initial_cpb_removal_delay_offset[SchedSelIdx]);
                }
            }

            if (context.Sps.SeqParameterSetData.VuiParameters.VclHrdParametersPresentFlag != 0)
            {

                for (SchedSelIdx = 0; SchedSelIdx <= context.Sps.SeqParameterSetData.VuiParameters.HrdParameters.CpbCntMinus1; SchedSelIdx++)
                {
                    size += stream.WriteUnsignedIntVariable((context.Sps.SeqParameterSetData.VuiParameters.HrdParameters.InitialCpbRemovalDelayLengthMinus1 + 1), this.initial_cpb_removal_delay[SchedSelIdx]);
                    size += stream.WriteUnsignedIntVariable((context.Sps.SeqParameterSetData.VuiParameters.HrdParameters.InitialCpbRemovalDelayLengthMinus1 + 1), this.initial_cpb_removal_delay_offset[SchedSelIdx]);
                }
            }

            return size;
        }

    }

    /*
 

pic_timing( payloadSize ) {  
 if( CpbDpbDelaysPresentFlag ) {   
  cpb_removal_delay 5 u(v) 
  dpb_output_delay 5 u(v) 
 }   
 if( pic_struct_present_flag ) {   
  pic_struct 5 u(4) 
  for( i = 0; i < NumClockTS; i++ ) {   
   clock_timestamp_flag[ i ] 5 u(1) 
   if( clock_timestamp_flag[ i ] ) {   
   ct_type 5 u(2) 
   nuit_field_based_flag 5 u(1) 
   counting_type 5 u(5) 
   full_timestamp_flag 5 u(1) 
   discontinuity_flag 5 u(1) 
   cnt_dropped_flag 5 u(1) 
   n_frames 5 u(8) 
   if( full_timestamp_flag ) {   
   seconds_value /* 0..59 *//* 5 u(6) 
   minutes_value /* 0..59 *//* 5 u(6) 
   hours_value /* 0..23 *//* 5 u(5) 
   } else {   
   seconds_flag 5 u(1) 
   if( seconds_flag ) {   
    seconds_value /* range 0..59 *//* 5 u(6) 
      minutes_flag 5 u(1) 
      if( minutes_flag ) {   
       minutes_value /* 0..59 *//* 5 u(6) 
       hours_flag 5 u(1) 
       if( hours_flag )   
        hours_value /* 0..23 *//* 5 u(5) 
      }   
     }   
    }   
    if( time_offset_length > 0 )   
     time_offset 5 i(v) 
   }   
  }   
 }   
}
    */
    public class PicTiming : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint cpb_removal_delay;
        public uint CpbRemovalDelay { get { return cpb_removal_delay; } set { cpb_removal_delay = value; } }
        private uint dpb_output_delay;
        public uint DpbOutputDelay { get { return dpb_output_delay; } set { dpb_output_delay = value; } }
        private uint pic_struct;
        public uint PicStruct { get { return pic_struct; } set { pic_struct = value; } }
        private byte[] clock_timestamp_flag;
        public byte[] ClockTimestampFlag { get { return clock_timestamp_flag; } set { clock_timestamp_flag = value; } }
        private uint[] ct_type;
        public uint[] CtType { get { return ct_type; } set { ct_type = value; } }
        private byte[] nuit_field_based_flag;
        public byte[] NuitFieldBasedFlag { get { return nuit_field_based_flag; } set { nuit_field_based_flag = value; } }
        private uint[] counting_type;
        public uint[] CountingType { get { return counting_type; } set { counting_type = value; } }
        private byte[] full_timestamp_flag;
        public byte[] FullTimestampFlag { get { return full_timestamp_flag; } set { full_timestamp_flag = value; } }
        private byte[] discontinuity_flag;
        public byte[] DiscontinuityFlag { get { return discontinuity_flag; } set { discontinuity_flag = value; } }
        private byte[] cnt_dropped_flag;
        public byte[] CntDroppedFlag { get { return cnt_dropped_flag; } set { cnt_dropped_flag = value; } }
        private uint[] n_frames;
        public uint[] nFrames { get { return n_frames; } set { n_frames = value; } }
        private uint[] seconds_value;
        public uint[] SecondsValue { get { return seconds_value; } set { seconds_value = value; } }
        private uint[] minutes_value;
        public uint[] MinutesValue { get { return minutes_value; } set { minutes_value = value; } }
        private uint[] hours_value;
        public uint[] HoursValue { get { return hours_value; } set { hours_value = value; } }
        private byte[] seconds_flag;
        public byte[] SecondsFlag { get { return seconds_flag; } set { seconds_flag = value; } }
        private byte[] minutes_flag;
        public byte[] MinutesFlag { get { return minutes_flag; } set { minutes_flag = value; } }
        private byte[] hours_flag;
        public byte[] HoursFlag { get { return hours_flag; } set { hours_flag = value; } }
        private int[] time_offset;
        public int[] TimeOffset { get { return time_offset; } set { time_offset = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public PicTiming(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;

            if ((uint)(context.Sps.SeqParameterSetData.VuiParameters.NalHrdParametersPresentFlag == 1 || context.Sps.SeqParameterSetData.VuiParameters.VclHrdParametersPresentFlag == 1 ? 1 : 0) != 0)
            {
                size += stream.ReadUnsignedIntVariable(size, (context.Sps.SeqParameterSetData.VuiParameters.HrdParameters.CpbRemovalDelayLengthMinus1 + 1), out this.cpb_removal_delay);
                size += stream.ReadUnsignedIntVariable(size, (context.Sps.SeqParameterSetData.VuiParameters.HrdParameters.DpbOutputDelayLengthMinus1 + 1), out this.dpb_output_delay);
            }

            if (context.Sps.SeqParameterSetData.VuiParameters.PicStructPresentFlag != 0)
            {
                size += stream.ReadUnsignedInt(size, 4, out this.pic_struct);

                this.clock_timestamp_flag = new byte[this.pic_struct switch
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
                }];
                this.ct_type = new uint[this.pic_struct switch
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
                }];
                this.nuit_field_based_flag = new byte[this.pic_struct switch
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
                }];
                this.counting_type = new uint[this.pic_struct switch
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
                }];
                this.full_timestamp_flag = new byte[this.pic_struct switch
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
                }];
                this.discontinuity_flag = new byte[this.pic_struct switch
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
                }];
                this.cnt_dropped_flag = new byte[this.pic_struct switch
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
                }];
                this.n_frames = new uint[this.pic_struct switch
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
                }];
                this.seconds_value = new uint[this.pic_struct switch
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
                }];
                this.minutes_value = new uint[this.pic_struct switch
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
                }];
                this.hours_value = new uint[this.pic_struct switch
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
                }];
                this.seconds_flag = new byte[this.pic_struct switch
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
                }];
                this.minutes_flag = new byte[this.pic_struct switch
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
                }];
                this.hours_flag = new byte[this.pic_struct switch
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
                }];
                this.time_offset = new int[this.pic_struct switch
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
                }];
                for (i = 0; i < this.pic_struct switch
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
                }; i++)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.clock_timestamp_flag[i]);

                    if (clock_timestamp_flag[i] != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 2, out this.ct_type[i]);
                        size += stream.ReadUnsignedInt(size, 1, out this.nuit_field_based_flag[i]);
                        size += stream.ReadUnsignedInt(size, 5, out this.counting_type[i]);
                        size += stream.ReadUnsignedInt(size, 1, out this.full_timestamp_flag[i]);
                        size += stream.ReadUnsignedInt(size, 1, out this.discontinuity_flag[i]);
                        size += stream.ReadUnsignedInt(size, 1, out this.cnt_dropped_flag[i]);
                        size += stream.ReadUnsignedInt(size, 8, out this.n_frames[i]);

                        if (full_timestamp_flag[i] != 0)
                        {
                            size += stream.ReadUnsignedInt(size, 6, out this.seconds_value[i]); // 0..59 
                            size += stream.ReadUnsignedInt(size, 6, out this.minutes_value[i]); // 0..59 
                            size += stream.ReadUnsignedInt(size, 5, out this.hours_value[i]); // 0..23 
                        }
                        else
                        {
                            size += stream.ReadUnsignedInt(size, 1, out this.seconds_flag[i]);

                            if (seconds_flag[i] != 0)
                            {
                                size += stream.ReadUnsignedInt(size, 6, out this.seconds_value[i]); // range 0..59 
                                size += stream.ReadUnsignedInt(size, 1, out this.minutes_flag[i]);

                                if (minutes_flag[i] != 0)
                                {
                                    size += stream.ReadUnsignedInt(size, 6, out this.minutes_value[i]); // 0..59 
                                    size += stream.ReadUnsignedInt(size, 1, out this.hours_flag[i]);

                                    if (hours_flag[i] != 0)
                                    {
                                        size += stream.ReadUnsignedInt(size, 5, out this.hours_value[i]); // 0..23 
                                    }
                                }
                            }
                        }

                        if (context.Sps.SeqParameterSetData.VuiParameters.HrdParameters.TimeOffsetLength > 0)
                        {
                            size += stream.ReadSignedIntVariable(size, context.Sps.SeqParameterSetData.VuiParameters.HrdParameters.TimeOffsetLength, out this.time_offset[i]);
                        }
                    }
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;

            if ((uint)(context.Sps.SeqParameterSetData.VuiParameters.NalHrdParametersPresentFlag == 1 || context.Sps.SeqParameterSetData.VuiParameters.VclHrdParametersPresentFlag == 1 ? 1 : 0) != 0)
            {
                size += stream.WriteUnsignedIntVariable((context.Sps.SeqParameterSetData.VuiParameters.HrdParameters.CpbRemovalDelayLengthMinus1 + 1), this.cpb_removal_delay);
                size += stream.WriteUnsignedIntVariable((context.Sps.SeqParameterSetData.VuiParameters.HrdParameters.DpbOutputDelayLengthMinus1 + 1), this.dpb_output_delay);
            }

            if (context.Sps.SeqParameterSetData.VuiParameters.PicStructPresentFlag != 0)
            {
                size += stream.WriteUnsignedInt(4, this.pic_struct);

                for (i = 0; i < this.pic_struct switch
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
                }; i++)
                {
                    size += stream.WriteUnsignedInt(1, this.clock_timestamp_flag[i]);

                    if (clock_timestamp_flag[i] != 0)
                    {
                        size += stream.WriteUnsignedInt(2, this.ct_type[i]);
                        size += stream.WriteUnsignedInt(1, this.nuit_field_based_flag[i]);
                        size += stream.WriteUnsignedInt(5, this.counting_type[i]);
                        size += stream.WriteUnsignedInt(1, this.full_timestamp_flag[i]);
                        size += stream.WriteUnsignedInt(1, this.discontinuity_flag[i]);
                        size += stream.WriteUnsignedInt(1, this.cnt_dropped_flag[i]);
                        size += stream.WriteUnsignedInt(8, this.n_frames[i]);

                        if (full_timestamp_flag[i] != 0)
                        {
                            size += stream.WriteUnsignedInt(6, this.seconds_value[i]); // 0..59 
                            size += stream.WriteUnsignedInt(6, this.minutes_value[i]); // 0..59 
                            size += stream.WriteUnsignedInt(5, this.hours_value[i]); // 0..23 
                        }
                        else
                        {
                            size += stream.WriteUnsignedInt(1, this.seconds_flag[i]);

                            if (seconds_flag[i] != 0)
                            {
                                size += stream.WriteUnsignedInt(6, this.seconds_value[i]); // range 0..59 
                                size += stream.WriteUnsignedInt(1, this.minutes_flag[i]);

                                if (minutes_flag[i] != 0)
                                {
                                    size += stream.WriteUnsignedInt(6, this.minutes_value[i]); // 0..59 
                                    size += stream.WriteUnsignedInt(1, this.hours_flag[i]);

                                    if (hours_flag[i] != 0)
                                    {
                                        size += stream.WriteUnsignedInt(5, this.hours_value[i]); // 0..23 
                                    }
                                }
                            }
                        }

                        if (context.Sps.SeqParameterSetData.VuiParameters.HrdParameters.TimeOffsetLength > 0)
                        {
                            size += stream.WriteSignedIntVariable(context.Sps.SeqParameterSetData.VuiParameters.HrdParameters.TimeOffsetLength, this.time_offset[i]);
                        }
                    }
                }
            }

            return size;
        }

    }

    /*
   

pan_scan_rect( payloadSize ) {  
pan_scan_rect_id 5 ue(v) 
pan_scan_rect_cancel_flag 5 u(1) 
if( !pan_scan_rect_cancel_flag ) {   
 pan_scan_cnt_minus1 5 ue(v) 
 for( i = 0; i <= pan_scan_cnt_minus1; i++ ) {   
  pan_scan_rect_left_offset[ i ] 5 se(v) 
  pan_scan_rect_right_offset[ i ] 5 se(v) 
  pan_scan_rect_top_offset[ i ] 5 se(v) 
  pan_scan_rect_bottom_offset[ i ] 5 se(v) 
 }   
 pan_scan_rect_repetition_period 5 ue(v) 
}   
}
    */
    public class PanScanRect : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint pan_scan_rect_id;
        public uint PanScanRectId { get { return pan_scan_rect_id; } set { pan_scan_rect_id = value; } }
        private byte pan_scan_rect_cancel_flag;
        public byte PanScanRectCancelFlag { get { return pan_scan_rect_cancel_flag; } set { pan_scan_rect_cancel_flag = value; } }
        private uint pan_scan_cnt_minus1;
        public uint PanScanCntMinus1 { get { return pan_scan_cnt_minus1; } set { pan_scan_cnt_minus1 = value; } }
        private int[] pan_scan_rect_left_offset;
        public int[] PanScanRectLeftOffset { get { return pan_scan_rect_left_offset; } set { pan_scan_rect_left_offset = value; } }
        private int[] pan_scan_rect_right_offset;
        public int[] PanScanRectRightOffset { get { return pan_scan_rect_right_offset; } set { pan_scan_rect_right_offset = value; } }
        private int[] pan_scan_rect_top_offset;
        public int[] PanScanRectTopOffset { get { return pan_scan_rect_top_offset; } set { pan_scan_rect_top_offset = value; } }
        private int[] pan_scan_rect_bottom_offset;
        public int[] PanScanRectBottomOffset { get { return pan_scan_rect_bottom_offset; } set { pan_scan_rect_bottom_offset = value; } }
        private uint pan_scan_rect_repetition_period;
        public uint PanScanRectRepetitionPeriod { get { return pan_scan_rect_repetition_period; } set { pan_scan_rect_repetition_period = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public PanScanRect(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.pan_scan_rect_id);
            size += stream.ReadUnsignedInt(size, 1, out this.pan_scan_rect_cancel_flag);

            if (pan_scan_rect_cancel_flag == 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.pan_scan_cnt_minus1);

                this.pan_scan_rect_left_offset = new int[pan_scan_cnt_minus1 + 1];
                this.pan_scan_rect_right_offset = new int[pan_scan_cnt_minus1 + 1];
                this.pan_scan_rect_top_offset = new int[pan_scan_cnt_minus1 + 1];
                this.pan_scan_rect_bottom_offset = new int[pan_scan_cnt_minus1 + 1];
                for (i = 0; i <= pan_scan_cnt_minus1; i++)
                {
                    size += stream.ReadSignedIntGolomb(size, out this.pan_scan_rect_left_offset[i]);
                    size += stream.ReadSignedIntGolomb(size, out this.pan_scan_rect_right_offset[i]);
                    size += stream.ReadSignedIntGolomb(size, out this.pan_scan_rect_top_offset[i]);
                    size += stream.ReadSignedIntGolomb(size, out this.pan_scan_rect_bottom_offset[i]);
                }
                size += stream.ReadUnsignedIntGolomb(size, out this.pan_scan_rect_repetition_period);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedIntGolomb(this.pan_scan_rect_id);
            size += stream.WriteUnsignedInt(1, this.pan_scan_rect_cancel_flag);

            if (pan_scan_rect_cancel_flag == 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.pan_scan_cnt_minus1);

                for (i = 0; i <= pan_scan_cnt_minus1; i++)
                {
                    size += stream.WriteSignedIntGolomb(this.pan_scan_rect_left_offset[i]);
                    size += stream.WriteSignedIntGolomb(this.pan_scan_rect_right_offset[i]);
                    size += stream.WriteSignedIntGolomb(this.pan_scan_rect_top_offset[i]);
                    size += stream.WriteSignedIntGolomb(this.pan_scan_rect_bottom_offset[i]);
                }
                size += stream.WriteUnsignedIntGolomb(this.pan_scan_rect_repetition_period);
            }

            return size;
        }

    }

    /*
   

filler_payload( payloadSize ) {  
 for( k = 0; k < payloadSize; k++ )   
  ff_byte  /* equal to 0xFF *//* 5 f(8) 
}
    */
    public class FillerPayload : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint[] ff_byte;
        public uint[] FfByte { get { return ff_byte; } set { ff_byte = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public FillerPayload(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint k = 0;

            this.ff_byte = new uint[payloadSize];
            for (k = 0; k < payloadSize; k++)
            {
                size += stream.ReadFixed(size, 8, out this.ff_byte[k]); // equal to 0xFF 
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint k = 0;

            for (k = 0; k < payloadSize; k++)
            {
                size += stream.WriteFixed(8, this.ff_byte[k]); // equal to 0xFF 
            }

            return size;
        }

    }

    /*
  

user_data_registered_itu_t_t35( payloadSize ) {  
 itu_t_t35_country_code 5 b(8) 
 if( itu_t_t35_country_code  !=  0xFF )   
  i = 1   
 else {   
  itu_t_t35_country_code_extension_byte 5 b(8) 
  i = 2   
 }   
 do {   
  itu_t_t35_payload_byte 5 b(8) 
  i++   
 } while( i < payloadSize )   
}
    */
    public class UserDataRegisteredItutT35 : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte itu_t_t35_country_code;
        public byte ItutT35CountryCode { get { return itu_t_t35_country_code; } set { itu_t_t35_country_code = value; } }
        private byte itu_t_t35_country_code_extension_byte;
        public byte ItutT35CountryCodeExtensionByte { get { return itu_t_t35_country_code_extension_byte; } set { itu_t_t35_country_code_extension_byte = value; } }
        private byte itu_t_t35_payload_byte;
        public byte ItutT35PayloadByte { get { return itu_t_t35_payload_byte; } set { itu_t_t35_payload_byte = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public UserDataRegisteredItutT35(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadBits(size, 8, out this.itu_t_t35_country_code);

            if (itu_t_t35_country_code != 0xFF)
            {
                i = 1;
            }
            else
            {
                size += stream.ReadBits(size, 8, out this.itu_t_t35_country_code_extension_byte);
                i = 2;
            }

            do
            {
                size += stream.ReadBits(size, 8, out this.itu_t_t35_payload_byte);
                i++;
            } while (i < payloadSize);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteBits(8, this.itu_t_t35_country_code);

            if (itu_t_t35_country_code != 0xFF)
            {
                i = 1;
            }
            else
            {
                size += stream.WriteBits(8, this.itu_t_t35_country_code_extension_byte);
                i = 2;
            }

            do
            {
                size += stream.WriteBits(8, this.itu_t_t35_payload_byte);
                i++;
            } while (i < payloadSize);

            return size;
        }

    }

    /*
  

user_data_unregistered( payloadSize ) {  
 uuid_iso_iec_11578 5 u(128) 
 for( i = 16; i < payloadSize; i++ )   
  user_data_payload_byte 5 b(8) 
}
    */
    public class UserDataUnregistered : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private BigInteger uuid_iso_iec_11578;
        public BigInteger UuidIsoIec11578 { get { return uuid_iso_iec_11578; } set { uuid_iso_iec_11578 = value; } }
        private byte[] user_data_payload_byte;
        public byte[] UserDataPayloadByte { get { return user_data_payload_byte; } set { user_data_payload_byte = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public UserDataUnregistered(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedInt(size, 128, out this.uuid_iso_iec_11578);

            this.user_data_payload_byte = new byte[payloadSize];
            for (i = 16; i < payloadSize; i++)
            {
                size += stream.ReadBits(size, 8, out this.user_data_payload_byte[i]);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedInt(128, this.uuid_iso_iec_11578);

            for (i = 16; i < payloadSize; i++)
            {
                size += stream.WriteBits(8, this.user_data_payload_byte[i]);
            }

            return size;
        }

    }

    /*
   

recovery_point( payloadSize ) {  
 recovery_frame_cnt 5 ue(v) 
 exact_match_flag 5 u(1) 
 broken_link_flag 5 u(1) 
 changing_slice_group_idc 5 u(2) 
}
    */
    public class RecoveryPoint : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint recovery_frame_cnt;
        public uint RecoveryFrameCnt { get { return recovery_frame_cnt; } set { recovery_frame_cnt = value; } }
        private byte exact_match_flag;
        public byte ExactMatchFlag { get { return exact_match_flag; } set { exact_match_flag = value; } }
        private byte broken_link_flag;
        public byte BrokenLinkFlag { get { return broken_link_flag; } set { broken_link_flag = value; } }
        private uint changing_slice_group_idc;
        public uint ChangingSliceGroupIdc { get { return changing_slice_group_idc; } set { changing_slice_group_idc = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public RecoveryPoint(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedIntGolomb(size, out this.recovery_frame_cnt);
            size += stream.ReadUnsignedInt(size, 1, out this.exact_match_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.broken_link_flag);
            size += stream.ReadUnsignedInt(size, 2, out this.changing_slice_group_idc);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedIntGolomb(this.recovery_frame_cnt);
            size += stream.WriteUnsignedInt(1, this.exact_match_flag);
            size += stream.WriteUnsignedInt(1, this.broken_link_flag);
            size += stream.WriteUnsignedInt(2, this.changing_slice_group_idc);

            return size;
        }

    }

    /*
  

dec_ref_pic_marking_repetition( payloadSize ) {  
 original_idr_flag 5 u(1) 
 original_frame_num 5 ue(v) 
 if( !frame_mbs_only_flag ) {   
  original_field_pic_flag 5 u(1) 
  if( original_field_pic_flag )   
   original_bottom_field_flag 5 u(1) 
 }   
 dec_ref_pic_marking() 5  
}
    */
    public class DecRefPicMarkingRepetition : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte original_idr_flag;
        public byte OriginalIdrFlag { get { return original_idr_flag; } set { original_idr_flag = value; } }
        private uint original_frame_num;
        public uint OriginalFrameNum { get { return original_frame_num; } set { original_frame_num = value; } }
        private byte original_field_pic_flag;
        public byte OriginalFieldPicFlag { get { return original_field_pic_flag; } set { original_field_pic_flag = value; } }
        private byte original_bottom_field_flag;
        public byte OriginalBottomFieldFlag { get { return original_bottom_field_flag; } set { original_bottom_field_flag = value; } }
        private DecRefPicMarking dec_ref_pic_marking;
        public DecRefPicMarking DecRefPicMarking { get { return dec_ref_pic_marking; } set { dec_ref_pic_marking = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public DecRefPicMarkingRepetition(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.original_idr_flag);
            size += stream.ReadUnsignedIntGolomb(size, out this.original_frame_num);

            if (context.Sps.SeqParameterSetData.FrameMbsOnlyFlag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.original_field_pic_flag);

                if (original_field_pic_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.original_bottom_field_flag);
                }
            }
            size += stream.ReadClass<DecRefPicMarking>(size, context, () => new DecRefPicMarking(), out this.dec_ref_pic_marking);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.original_idr_flag);
            size += stream.WriteUnsignedIntGolomb(this.original_frame_num);

            if (context.Sps.SeqParameterSetData.FrameMbsOnlyFlag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.original_field_pic_flag);

                if (original_field_pic_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.original_bottom_field_flag);
                }
            }
            size += stream.WriteClass<DecRefPicMarking>(context, this.dec_ref_pic_marking);

            return size;
        }

    }

    /*
  

spare_pic( payloadSize ) {  
 target_frame_num 5 ue(v) 
 spare_field_flag 5 u(1) 
 if( spare_field_flag )   
  target_bottom_field_flag 5 u(1) 
 num_spare_pics_minus1 5 ue(v) 
 for( i = 0; i < num_spare_pics_minus1 + 1; i++ ) {   
  delta_spare_frame_num[ i ] 5 ue(v) 
  if( spare_field_flag )   
   spare_bottom_field_flag[ i ] 5 u(1) 
  spare_area_idc[ i ] 5 ue(v) 
  if( spare_area_idc[ i ]  ==  1 )   
   for( j = 0; j < PicSizeInMapUnits; j++ )    
    spare_unit_flag[ i ][ j ] 5 u(1) 
  else if( spare_area_idc[ i ]  ==  2 ) {   
   mapUnitCnt = 0   
   for( j=0; mapUnitCnt < PicSizeInMapUnits; j++ ) {   
    zero_run_length[ i ][ j ] 5 ue(v) 
    mapUnitCnt += zero_run_length[ i ][ j ] + 1   
   }   
  }   
 }    
}
    */
    public class SparePic : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint target_frame_num;
        public uint TargetFrameNum { get { return target_frame_num; } set { target_frame_num = value; } }
        private byte spare_field_flag;
        public byte SpareFieldFlag { get { return spare_field_flag; } set { spare_field_flag = value; } }
        private byte target_bottom_field_flag;
        public byte TargetBottomFieldFlag { get { return target_bottom_field_flag; } set { target_bottom_field_flag = value; } }
        private uint num_spare_pics_minus1;
        public uint NumSparePicsMinus1 { get { return num_spare_pics_minus1; } set { num_spare_pics_minus1 = value; } }
        private uint[] delta_spare_frame_num;
        public uint[] DeltaSpareFrameNum { get { return delta_spare_frame_num; } set { delta_spare_frame_num = value; } }
        private byte[] spare_bottom_field_flag;
        public byte[] SpareBottomFieldFlag { get { return spare_bottom_field_flag; } set { spare_bottom_field_flag = value; } }
        private uint[] spare_area_idc;
        public uint[] SpareAreaIdc { get { return spare_area_idc; } set { spare_area_idc = value; } }
        private byte[][] spare_unit_flag;
        public byte[][] SpareUnitFlag { get { return spare_unit_flag; } set { spare_unit_flag = value; } }
        private uint[][] zero_run_length;
        public uint[][] ZeroRunLength { get { return zero_run_length; } set { zero_run_length = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SparePic(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            uint mapUnitCnt = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.target_frame_num);
            size += stream.ReadUnsignedInt(size, 1, out this.spare_field_flag);

            if (spare_field_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.target_bottom_field_flag);
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.num_spare_pics_minus1);

            this.delta_spare_frame_num = new uint[num_spare_pics_minus1 + 1 + 1];
            this.spare_bottom_field_flag = new byte[num_spare_pics_minus1 + 1 + 1];
            this.spare_area_idc = new uint[num_spare_pics_minus1 + 1 + 1];
            this.spare_unit_flag = new byte[num_spare_pics_minus1 + 1 + 1][];
            this.zero_run_length = new uint[num_spare_pics_minus1 + 1 + 1][];
            for (i = 0; i < num_spare_pics_minus1 + 1; i++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.delta_spare_frame_num[i]);

                if (spare_field_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.spare_bottom_field_flag[i]);
                }
                size += stream.ReadUnsignedIntGolomb(size, out this.spare_area_idc[i]);

                if (spare_area_idc[i] == 1)
                {

                    this.spare_unit_flag[i] = new byte[(context.Sps.SeqParameterSetData.PicWidthInMbsMinus1 + 1 + 1) * (context.Sps.SeqParameterSetData.PicHeightInMapUnitsMinus1 + 1 + 1)];
                    for (j = 0; j < (context.Sps.SeqParameterSetData.PicWidthInMbsMinus1 + 1) * (context.Sps.SeqParameterSetData.PicHeightInMapUnitsMinus1 + 1); j++)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.spare_unit_flag[i][j]);
                    }
                }
                else if (spare_area_idc[i] == 2)
                {
                    mapUnitCnt = 0;

                    this.zero_run_length[i] = new uint[(context.Sps.SeqParameterSetData.PicWidthInMbsMinus1 + 1 + 1) * (context.Sps.SeqParameterSetData.PicHeightInMapUnitsMinus1 + 1 + 1)];
                    for (j = 0; mapUnitCnt < (context.Sps.SeqParameterSetData.PicWidthInMbsMinus1 + 1) * (context.Sps.SeqParameterSetData.PicHeightInMapUnitsMinus1 + 1); j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.zero_run_length[i][j]);
                        mapUnitCnt += zero_run_length[i][j] + 1;
                    }
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            uint mapUnitCnt = 0;
            size += stream.WriteUnsignedIntGolomb(this.target_frame_num);
            size += stream.WriteUnsignedInt(1, this.spare_field_flag);

            if (spare_field_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.target_bottom_field_flag);
            }
            size += stream.WriteUnsignedIntGolomb(this.num_spare_pics_minus1);

            for (i = 0; i < num_spare_pics_minus1 + 1; i++)
            {
                size += stream.WriteUnsignedIntGolomb(this.delta_spare_frame_num[i]);

                if (spare_field_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.spare_bottom_field_flag[i]);
                }
                size += stream.WriteUnsignedIntGolomb(this.spare_area_idc[i]);

                if (spare_area_idc[i] == 1)
                {

                    for (j = 0; j < (context.Sps.SeqParameterSetData.PicWidthInMbsMinus1 + 1) * (context.Sps.SeqParameterSetData.PicHeightInMapUnitsMinus1 + 1); j++)
                    {
                        size += stream.WriteUnsignedInt(1, this.spare_unit_flag[i][j]);
                    }
                }
                else if (spare_area_idc[i] == 2)
                {
                    mapUnitCnt = 0;

                    for (j = 0; mapUnitCnt < (context.Sps.SeqParameterSetData.PicWidthInMbsMinus1 + 1) * (context.Sps.SeqParameterSetData.PicHeightInMapUnitsMinus1 + 1); j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.zero_run_length[i][j]);
                        mapUnitCnt += zero_run_length[i][j] + 1;
                    }
                }
            }

            return size;
        }

    }

    /*
   

scene_info( payloadSize ) {  
 scene_info_present_flag 5 u(1) 
 if( scene_info_present_flag ) {   
  scene_id 5 ue(v) 
  scene_transition_type 5 ue(v) 
  if( scene_transition_type > 3 )   
   second_scene_id 5 ue(v) 
 }   
}
    */
    public class SceneInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte scene_info_present_flag;
        public byte SceneInfoPresentFlag { get { return scene_info_present_flag; } set { scene_info_present_flag = value; } }
        private uint scene_id;
        public uint SceneId { get { return scene_id; } set { scene_id = value; } }
        private uint scene_transition_type;
        public uint SceneTransitionType { get { return scene_transition_type; } set { scene_transition_type = value; } }
        private uint second_scene_id;
        public uint SecondSceneId { get { return second_scene_id; } set { second_scene_id = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SceneInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.scene_info_present_flag);

            if (scene_info_present_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.scene_id);
                size += stream.ReadUnsignedIntGolomb(size, out this.scene_transition_type);

                if (scene_transition_type > 3)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.second_scene_id);
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.scene_info_present_flag);

            if (scene_info_present_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.scene_id);
                size += stream.WriteUnsignedIntGolomb(this.scene_transition_type);

                if (scene_transition_type > 3)
                {
                    size += stream.WriteUnsignedIntGolomb(this.second_scene_id);
                }
            }

            return size;
        }

    }

    /*
   

sub_seq_info( payloadSize ) {  
 sub_seq_layer_num 5 ue(v) 
 sub_seq_id 5 ue(v) 
 first_ref_pic_flag 5 u(1) 
 leading_non_ref_pic_flag 5 u(1) 
 last_pic_flag 5 u(1) 
 sub_seq_frame_num_flag 5 u(1) 
 if( sub_seq_frame_num_flag )   
  sub_seq_frame_num 5 ue(v) 
}
    */
    public class SubSeqInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint sub_seq_layer_num;
        public uint SubSeqLayerNum { get { return sub_seq_layer_num; } set { sub_seq_layer_num = value; } }
        private uint sub_seq_id;
        public uint SubSeqId { get { return sub_seq_id; } set { sub_seq_id = value; } }
        private byte first_ref_pic_flag;
        public byte FirstRefPicFlag { get { return first_ref_pic_flag; } set { first_ref_pic_flag = value; } }
        private byte leading_non_ref_pic_flag;
        public byte LeadingNonRefPicFlag { get { return leading_non_ref_pic_flag; } set { leading_non_ref_pic_flag = value; } }
        private byte last_pic_flag;
        public byte LastPicFlag { get { return last_pic_flag; } set { last_pic_flag = value; } }
        private byte sub_seq_frame_num_flag;
        public byte SubSeqFrameNumFlag { get { return sub_seq_frame_num_flag; } set { sub_seq_frame_num_flag = value; } }
        private uint sub_seq_frame_num;
        public uint SubSeqFrameNum { get { return sub_seq_frame_num; } set { sub_seq_frame_num = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SubSeqInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedIntGolomb(size, out this.sub_seq_layer_num);
            size += stream.ReadUnsignedIntGolomb(size, out this.sub_seq_id);
            size += stream.ReadUnsignedInt(size, 1, out this.first_ref_pic_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.leading_non_ref_pic_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.last_pic_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.sub_seq_frame_num_flag);

            if (sub_seq_frame_num_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.sub_seq_frame_num);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedIntGolomb(this.sub_seq_layer_num);
            size += stream.WriteUnsignedIntGolomb(this.sub_seq_id);
            size += stream.WriteUnsignedInt(1, this.first_ref_pic_flag);
            size += stream.WriteUnsignedInt(1, this.leading_non_ref_pic_flag);
            size += stream.WriteUnsignedInt(1, this.last_pic_flag);
            size += stream.WriteUnsignedInt(1, this.sub_seq_frame_num_flag);

            if (sub_seq_frame_num_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.sub_seq_frame_num);
            }

            return size;
        }

    }

    /*
  

sub_seq_layer_characteristics( payloadSize ) {  
 num_sub_seq_layers_minus1 5 ue(v) 
 for( layer = 0; layer <= num_sub_seq_layers_minus1; layer++ ) {   
  accurate_statistics_flag 5 u(1) 
  average_bit_rate 5 u(16) 
  average_frame_rate 5 u(16) 
 }   
}
    */
    public class SubSeqLayerCharacteristics : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint num_sub_seq_layers_minus1;
        public uint NumSubSeqLayersMinus1 { get { return num_sub_seq_layers_minus1; } set { num_sub_seq_layers_minus1 = value; } }
        private byte[] accurate_statistics_flag;
        public byte[] AccurateStatisticsFlag { get { return accurate_statistics_flag; } set { accurate_statistics_flag = value; } }
        private uint[] average_bit_rate;
        public uint[] AverageBitRate { get { return average_bit_rate; } set { average_bit_rate = value; } }
        private uint[] average_frame_rate;
        public uint[] AverageFrameRate { get { return average_frame_rate; } set { average_frame_rate = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SubSeqLayerCharacteristics(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint layer = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.num_sub_seq_layers_minus1);

            this.accurate_statistics_flag = new byte[num_sub_seq_layers_minus1 + 1];
            this.average_bit_rate = new uint[num_sub_seq_layers_minus1 + 1];
            this.average_frame_rate = new uint[num_sub_seq_layers_minus1 + 1];
            for (layer = 0; layer <= num_sub_seq_layers_minus1; layer++)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.accurate_statistics_flag[layer]);
                size += stream.ReadUnsignedInt(size, 16, out this.average_bit_rate[layer]);
                size += stream.ReadUnsignedInt(size, 16, out this.average_frame_rate[layer]);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint layer = 0;
            size += stream.WriteUnsignedIntGolomb(this.num_sub_seq_layers_minus1);

            for (layer = 0; layer <= num_sub_seq_layers_minus1; layer++)
            {
                size += stream.WriteUnsignedInt(1, this.accurate_statistics_flag[layer]);
                size += stream.WriteUnsignedInt(16, this.average_bit_rate[layer]);
                size += stream.WriteUnsignedInt(16, this.average_frame_rate[layer]);
            }

            return size;
        }

    }

    /*
   

sub_seq_characteristics( payloadSize ) {  
 sub_seq_layer_num 5 ue(v) 
 sub_seq_id 5 ue(v) 
 duration_flag 5 u(1) 
 if( duration_flag)   
  sub_seq_duration 5 u(32) 
 average_rate_flag 5 u(1) 
 if( average_rate_flag ) {   
  accurate_statistics_flag 5 u(1) 
  average_bit_rate 5 u(16) 
  average_frame_rate 5 u(16) 
 }   
 num_referenced_subseqs 5 ue(v) 
 for( n = 0; n < num_referenced_subseqs; n++ ) {   
  ref_sub_seq_layer_num 5 ue(v) 
  ref_sub_seq_id 5 ue(v) 
  ref_sub_seq_direction 5 u(1) 
 }   
}
    */
    public class SubSeqCharacteristics : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint sub_seq_layer_num;
        public uint SubSeqLayerNum { get { return sub_seq_layer_num; } set { sub_seq_layer_num = value; } }
        private uint sub_seq_id;
        public uint SubSeqId { get { return sub_seq_id; } set { sub_seq_id = value; } }
        private byte duration_flag;
        public byte DurationFlag { get { return duration_flag; } set { duration_flag = value; } }
        private uint sub_seq_duration;
        public uint SubSeqDuration { get { return sub_seq_duration; } set { sub_seq_duration = value; } }
        private byte average_rate_flag;
        public byte AverageRateFlag { get { return average_rate_flag; } set { average_rate_flag = value; } }
        private byte accurate_statistics_flag;
        public byte AccurateStatisticsFlag { get { return accurate_statistics_flag; } set { accurate_statistics_flag = value; } }
        private uint average_bit_rate;
        public uint AverageBitRate { get { return average_bit_rate; } set { average_bit_rate = value; } }
        private uint average_frame_rate;
        public uint AverageFrameRate { get { return average_frame_rate; } set { average_frame_rate = value; } }
        private uint num_referenced_subseqs;
        public uint NumReferencedSubseqs { get { return num_referenced_subseqs; } set { num_referenced_subseqs = value; } }
        private uint[] ref_sub_seq_layer_num;
        public uint[] RefSubSeqLayerNum { get { return ref_sub_seq_layer_num; } set { ref_sub_seq_layer_num = value; } }
        private uint[] ref_sub_seq_id;
        public uint[] RefSubSeqId { get { return ref_sub_seq_id; } set { ref_sub_seq_id = value; } }
        private byte[] ref_sub_seq_direction;
        public byte[] RefSubSeqDirection { get { return ref_sub_seq_direction; } set { ref_sub_seq_direction = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SubSeqCharacteristics(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint n = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.sub_seq_layer_num);
            size += stream.ReadUnsignedIntGolomb(size, out this.sub_seq_id);
            size += stream.ReadUnsignedInt(size, 1, out this.duration_flag);

            if (duration_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 32, out this.sub_seq_duration);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.average_rate_flag);

            if (average_rate_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.accurate_statistics_flag);
                size += stream.ReadUnsignedInt(size, 16, out this.average_bit_rate);
                size += stream.ReadUnsignedInt(size, 16, out this.average_frame_rate);
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.num_referenced_subseqs);

            this.ref_sub_seq_layer_num = new uint[num_referenced_subseqs];
            this.ref_sub_seq_id = new uint[num_referenced_subseqs];
            this.ref_sub_seq_direction = new byte[num_referenced_subseqs];
            for (n = 0; n < num_referenced_subseqs; n++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.ref_sub_seq_layer_num[n]);
                size += stream.ReadUnsignedIntGolomb(size, out this.ref_sub_seq_id[n]);
                size += stream.ReadUnsignedInt(size, 1, out this.ref_sub_seq_direction[n]);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint n = 0;
            size += stream.WriteUnsignedIntGolomb(this.sub_seq_layer_num);
            size += stream.WriteUnsignedIntGolomb(this.sub_seq_id);
            size += stream.WriteUnsignedInt(1, this.duration_flag);

            if (duration_flag != 0)
            {
                size += stream.WriteUnsignedInt(32, this.sub_seq_duration);
            }
            size += stream.WriteUnsignedInt(1, this.average_rate_flag);

            if (average_rate_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.accurate_statistics_flag);
                size += stream.WriteUnsignedInt(16, this.average_bit_rate);
                size += stream.WriteUnsignedInt(16, this.average_frame_rate);
            }
            size += stream.WriteUnsignedIntGolomb(this.num_referenced_subseqs);

            for (n = 0; n < num_referenced_subseqs; n++)
            {
                size += stream.WriteUnsignedIntGolomb(this.ref_sub_seq_layer_num[n]);
                size += stream.WriteUnsignedIntGolomb(this.ref_sub_seq_id[n]);
                size += stream.WriteUnsignedInt(1, this.ref_sub_seq_direction[n]);
            }

            return size;
        }

    }

    /*
   

full_frame_freeze( payloadSize ) { 
  full_frame_freeze_repetition_period 5 ue(v)
}
    */
    public class FullFrameFreeze : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint full_frame_freeze_repetition_period;
        public uint FullFrameFreezeRepetitionPeriod { get { return full_frame_freeze_repetition_period; } set { full_frame_freeze_repetition_period = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public FullFrameFreeze(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedIntGolomb(size, out this.full_frame_freeze_repetition_period);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedIntGolomb(this.full_frame_freeze_repetition_period);

            return size;
        }

    }

    /*
   

full_frame_freeze_release( payloadSize ) { 
}
    */
    public class FullFrameFreezeRelease : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public FullFrameFreezeRelease(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;


            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;


            return size;
        }

    }

    /*
  

full_frame_snapshot( payloadSize ) { 
  snapshot_id 5 ue(v)
}
    */
    public class FullFrameSnapshot : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint snapshot_id;
        public uint SnapshotId { get { return snapshot_id; } set { snapshot_id = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public FullFrameSnapshot(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedIntGolomb(size, out this.snapshot_id);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedIntGolomb(this.snapshot_id);

            return size;
        }

    }

    /*
   

progressive_refinement_segment_start( payloadSize ) { 
  progressive_refinement_id 5 ue(v)
  num_refinement_steps_minus1 5 ue(v) 
}
    */
    public class ProgressiveRefinementSegmentStart : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint progressive_refinement_id;
        public uint ProgressiveRefinementId { get { return progressive_refinement_id; } set { progressive_refinement_id = value; } }
        private uint num_refinement_steps_minus1;
        public uint NumRefinementStepsMinus1 { get { return num_refinement_steps_minus1; } set { num_refinement_steps_minus1 = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ProgressiveRefinementSegmentStart(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedIntGolomb(size, out this.progressive_refinement_id);
            size += stream.ReadUnsignedIntGolomb(size, out this.num_refinement_steps_minus1);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedIntGolomb(this.progressive_refinement_id);
            size += stream.WriteUnsignedIntGolomb(this.num_refinement_steps_minus1);

            return size;
        }

    }

    /*
   

progressive_refinement_segment_end( payloadSize ) { 
  progressive_refinement_id 5 ue(v)
}
    */
    public class ProgressiveRefinementSegmentEnd : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint progressive_refinement_id;
        public uint ProgressiveRefinementId { get { return progressive_refinement_id; } set { progressive_refinement_id = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ProgressiveRefinementSegmentEnd(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedIntGolomb(size, out this.progressive_refinement_id);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedIntGolomb(this.progressive_refinement_id);

            return size;
        }

    }

    /*
   

motion_constrained_slice_group_set( payloadSize ) { 
   num_slice_groups_in_set_minus1 5 ue(v)
   if( num_slice_groups_minus1 > 0 )  
      for( i = 0; i <= num_slice_groups_in_set_minus1; i++ )   
         slice_group_id[ i ] 5 u(v) 
   exact_sample_value_match_flag 5 u(1) 
   pan_scan_rect_flag 5 u(1) 

   if( pan_scan_rect_flag )   
      pan_scan_rect_id 5 ue(v)
}
    */
    public class MotionConstrainedSliceGroupSet : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint num_slice_groups_in_set_minus1;
        public uint NumSliceGroupsInSetMinus1 { get { return num_slice_groups_in_set_minus1; } set { num_slice_groups_in_set_minus1 = value; } }
        private uint[] slice_group_id;
        public uint[] SliceGroupId { get { return slice_group_id; } set { slice_group_id = value; } }
        private byte exact_sample_value_match_flag;
        public byte ExactSampleValueMatchFlag { get { return exact_sample_value_match_flag; } set { exact_sample_value_match_flag = value; } }
        private byte pan_scan_rect_flag;
        public byte PanScanRectFlag { get { return pan_scan_rect_flag; } set { pan_scan_rect_flag = value; } }
        private uint pan_scan_rect_id;
        public uint PanScanRectId { get { return pan_scan_rect_id; } set { pan_scan_rect_id = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public MotionConstrainedSliceGroupSet(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.num_slice_groups_in_set_minus1);

            if (context.Pps.NumSliceGroupsMinus1 > 0)
            {

                this.slice_group_id = new uint[num_slice_groups_in_set_minus1 + 1];
                for (i = 0; i <= num_slice_groups_in_set_minus1; i++)
                {
                    size += stream.ReadUnsignedIntVariable(size, (uint)Math.Ceiling(Math.Log2(context.Pps.NumSliceGroupsMinus1 + 1)), out this.slice_group_id[i]);
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.exact_sample_value_match_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.pan_scan_rect_flag);

            if (pan_scan_rect_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.pan_scan_rect_id);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedIntGolomb(this.num_slice_groups_in_set_minus1);

            if (context.Pps.NumSliceGroupsMinus1 > 0)
            {

                for (i = 0; i <= num_slice_groups_in_set_minus1; i++)
                {
                    size += stream.WriteUnsignedIntVariable((uint)Math.Ceiling(Math.Log2(context.Pps.NumSliceGroupsMinus1 + 1)), this.slice_group_id[i]);
                }
            }
            size += stream.WriteUnsignedInt(1, this.exact_sample_value_match_flag);
            size += stream.WriteUnsignedInt(1, this.pan_scan_rect_flag);

            if (pan_scan_rect_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.pan_scan_rect_id);
            }

            return size;
        }

    }

    /*
   

film_grain_characteristics( payloadSize ) {  
 film_grain_characteristics_cancel_flag 5 u(1) 
 if( !film_grain_characteristics_cancel_flag ) {   
  film_grain_model_id 5 u(2) 
  separate_colour_description_present_flag 5 u(1) 
  if( separate_colour_description_present_flag ) {   
   film_grain_bit_depth_luma_minus8 5 u(3) 
   film_grain_bit_depth_chroma_minus8 5 u(3) 
   film_grain_full_range_flag 5 u(1) 
   film_grain_colour_primaries 5 u(8) 
   film_grain_transfer_characteristics 5 u(8) 
   film_grain_matrix_coefficients 5 u(8) 
  }   
  blending_mode_id 5 u(2) 
  log2_scale_factor 5 u(4) 
  for( c = 0; c < 3; c++ )   
   comp_model_present_flag[ c ] 5 u(1) 
  for( c = 0; c < 3; c++ )   
   if( comp_model_present_flag[ c ] ) {   
    num_intensity_intervals_minus1[ c ] 5 u(8) 
    num_model_values_minus1[ c ] 5 u(3) 
    for( i = 0; i <= num_intensity_intervals_minus1[ c ]; i++ ) {   
     intensity_interval_lower_bound[ c ][ i ]  5 u(8) 
     intensity_interval_upper_bound[ c ][ i ] 5 u(8) 
     for( j = 0; j <= num_model_values_minus1[ c ]; j++ )   
      comp_model_value[ c ][ i ][ j ] 5 se(v) 
    }   
   }   
 film_grain_characteristics_repetition_period 5 ue(v) 
 }   
}
    */
    public class FilmGrainCharacteristics : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte film_grain_characteristics_cancel_flag;
        public byte FilmGrainCharacteristicsCancelFlag { get { return film_grain_characteristics_cancel_flag; } set { film_grain_characteristics_cancel_flag = value; } }
        private uint film_grain_model_id;
        public uint FilmGrainModelId { get { return film_grain_model_id; } set { film_grain_model_id = value; } }
        private byte separate_colour_description_present_flag;
        public byte SeparateColourDescriptionPresentFlag { get { return separate_colour_description_present_flag; } set { separate_colour_description_present_flag = value; } }
        private uint film_grain_bit_depth_luma_minus8;
        public uint FilmGrainBitDepthLumaMinus8 { get { return film_grain_bit_depth_luma_minus8; } set { film_grain_bit_depth_luma_minus8 = value; } }
        private uint film_grain_bit_depth_chroma_minus8;
        public uint FilmGrainBitDepthChromaMinus8 { get { return film_grain_bit_depth_chroma_minus8; } set { film_grain_bit_depth_chroma_minus8 = value; } }
        private byte film_grain_full_range_flag;
        public byte FilmGrainFullRangeFlag { get { return film_grain_full_range_flag; } set { film_grain_full_range_flag = value; } }
        private uint film_grain_colour_primaries;
        public uint FilmGrainColourPrimaries { get { return film_grain_colour_primaries; } set { film_grain_colour_primaries = value; } }
        private uint film_grain_transfer_characteristics;
        public uint FilmGrainTransferCharacteristics { get { return film_grain_transfer_characteristics; } set { film_grain_transfer_characteristics = value; } }
        private uint film_grain_matrix_coefficients;
        public uint FilmGrainMatrixCoefficients { get { return film_grain_matrix_coefficients; } set { film_grain_matrix_coefficients = value; } }
        private uint blending_mode_id;
        public uint BlendingModeId { get { return blending_mode_id; } set { blending_mode_id = value; } }
        private uint log2_scale_factor;
        public uint Log2ScaleFactor { get { return log2_scale_factor; } set { log2_scale_factor = value; } }
        private byte[] comp_model_present_flag;
        public byte[] CompModelPresentFlag { get { return comp_model_present_flag; } set { comp_model_present_flag = value; } }
        private uint[] num_intensity_intervals_minus1;
        public uint[] NumIntensityIntervalsMinus1 { get { return num_intensity_intervals_minus1; } set { num_intensity_intervals_minus1 = value; } }
        private uint[] num_model_values_minus1;
        public uint[] NumModelValuesMinus1 { get { return num_model_values_minus1; } set { num_model_values_minus1 = value; } }
        private uint[][] intensity_interval_lower_bound;
        public uint[][] IntensityIntervalLowerBound { get { return intensity_interval_lower_bound; } set { intensity_interval_lower_bound = value; } }
        private uint[][] intensity_interval_upper_bound;
        public uint[][] IntensityIntervalUpperBound { get { return intensity_interval_upper_bound; } set { intensity_interval_upper_bound = value; } }
        private int[][][] comp_model_value;
        public int[][][] CompModelValue { get { return comp_model_value; } set { comp_model_value = value; } }
        private uint film_grain_characteristics_repetition_period;
        public uint FilmGrainCharacteristicsRepetitionPeriod { get { return film_grain_characteristics_repetition_period; } set { film_grain_characteristics_repetition_period = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public FilmGrainCharacteristics(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint c = 0;
            uint i = 0;
            uint j = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.film_grain_characteristics_cancel_flag);

            if (film_grain_characteristics_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 2, out this.film_grain_model_id);
                size += stream.ReadUnsignedInt(size, 1, out this.separate_colour_description_present_flag);

                if (separate_colour_description_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 3, out this.film_grain_bit_depth_luma_minus8);
                    size += stream.ReadUnsignedInt(size, 3, out this.film_grain_bit_depth_chroma_minus8);
                    size += stream.ReadUnsignedInt(size, 1, out this.film_grain_full_range_flag);
                    size += stream.ReadUnsignedInt(size, 8, out this.film_grain_colour_primaries);
                    size += stream.ReadUnsignedInt(size, 8, out this.film_grain_transfer_characteristics);
                    size += stream.ReadUnsignedInt(size, 8, out this.film_grain_matrix_coefficients);
                }
                size += stream.ReadUnsignedInt(size, 2, out this.blending_mode_id);
                size += stream.ReadUnsignedInt(size, 4, out this.log2_scale_factor);

                this.comp_model_present_flag = new byte[3];
                for (c = 0; c < 3; c++)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.comp_model_present_flag[c]);
                }

                this.num_intensity_intervals_minus1 = new uint[3];
                this.num_model_values_minus1 = new uint[3];
                this.intensity_interval_lower_bound = new uint[3][];
                this.intensity_interval_upper_bound = new uint[3][];
                this.comp_model_value = new int[3][][];
                for (c = 0; c < 3; c++)
                {

                    if (comp_model_present_flag[c] != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 8, out this.num_intensity_intervals_minus1[c]);
                        size += stream.ReadUnsignedInt(size, 3, out this.num_model_values_minus1[c]);

                        this.intensity_interval_lower_bound[c] = new uint[num_intensity_intervals_minus1[c] + 1];
                        this.intensity_interval_upper_bound[c] = new uint[num_intensity_intervals_minus1[c] + 1];
                        this.comp_model_value[c] = new int[num_intensity_intervals_minus1[c] + 1][];
                        for (i = 0; i <= num_intensity_intervals_minus1[c]; i++)
                        {
                            size += stream.ReadUnsignedInt(size, 8, out this.intensity_interval_lower_bound[c][i]);
                            size += stream.ReadUnsignedInt(size, 8, out this.intensity_interval_upper_bound[c][i]);

                            this.comp_model_value[c][i] = new int[num_model_values_minus1[c] + 1];
                            for (j = 0; j <= num_model_values_minus1[c]; j++)
                            {
                                size += stream.ReadSignedIntGolomb(size, out this.comp_model_value[c][i][j]);
                            }
                        }
                    }
                }
                size += stream.ReadUnsignedIntGolomb(size, out this.film_grain_characteristics_repetition_period);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint c = 0;
            uint i = 0;
            uint j = 0;
            size += stream.WriteUnsignedInt(1, this.film_grain_characteristics_cancel_flag);

            if (film_grain_characteristics_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(2, this.film_grain_model_id);
                size += stream.WriteUnsignedInt(1, this.separate_colour_description_present_flag);

                if (separate_colour_description_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(3, this.film_grain_bit_depth_luma_minus8);
                    size += stream.WriteUnsignedInt(3, this.film_grain_bit_depth_chroma_minus8);
                    size += stream.WriteUnsignedInt(1, this.film_grain_full_range_flag);
                    size += stream.WriteUnsignedInt(8, this.film_grain_colour_primaries);
                    size += stream.WriteUnsignedInt(8, this.film_grain_transfer_characteristics);
                    size += stream.WriteUnsignedInt(8, this.film_grain_matrix_coefficients);
                }
                size += stream.WriteUnsignedInt(2, this.blending_mode_id);
                size += stream.WriteUnsignedInt(4, this.log2_scale_factor);

                for (c = 0; c < 3; c++)
                {
                    size += stream.WriteUnsignedInt(1, this.comp_model_present_flag[c]);
                }

                for (c = 0; c < 3; c++)
                {

                    if (comp_model_present_flag[c] != 0)
                    {
                        size += stream.WriteUnsignedInt(8, this.num_intensity_intervals_minus1[c]);
                        size += stream.WriteUnsignedInt(3, this.num_model_values_minus1[c]);

                        for (i = 0; i <= num_intensity_intervals_minus1[c]; i++)
                        {
                            size += stream.WriteUnsignedInt(8, this.intensity_interval_lower_bound[c][i]);
                            size += stream.WriteUnsignedInt(8, this.intensity_interval_upper_bound[c][i]);

                            for (j = 0; j <= num_model_values_minus1[c]; j++)
                            {
                                size += stream.WriteSignedIntGolomb(this.comp_model_value[c][i][j]);
                            }
                        }
                    }
                }
                size += stream.WriteUnsignedIntGolomb(this.film_grain_characteristics_repetition_period);
            }

            return size;
        }

    }

    /*
  

deblocking_filter_display_preference( payloadSize ) {  
 deblocking_display_preference_cancel_flag 5 u(1) 
 if( !deblocking_display_preference_cancel_flag ) {   
  display_prior_to_deblocking_preferred_flag 5 u(1) 
  dec_frame_buffering_constraint_flag 5 u(1) 
  deblocking_display_preference_repetition_period 5 ue(v) 
 }   
}
    */
    public class DeblockingFilterDisplayPreference : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte deblocking_display_preference_cancel_flag;
        public byte DeblockingDisplayPreferenceCancelFlag { get { return deblocking_display_preference_cancel_flag; } set { deblocking_display_preference_cancel_flag = value; } }
        private byte display_prior_to_deblocking_preferred_flag;
        public byte DisplayPriorToDeblockingPreferredFlag { get { return display_prior_to_deblocking_preferred_flag; } set { display_prior_to_deblocking_preferred_flag = value; } }
        private byte dec_frame_buffering_constraint_flag;
        public byte DecFrameBufferingConstraintFlag { get { return dec_frame_buffering_constraint_flag; } set { dec_frame_buffering_constraint_flag = value; } }
        private uint deblocking_display_preference_repetition_period;
        public uint DeblockingDisplayPreferenceRepetitionPeriod { get { return deblocking_display_preference_repetition_period; } set { deblocking_display_preference_repetition_period = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public DeblockingFilterDisplayPreference(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.deblocking_display_preference_cancel_flag);

            if (deblocking_display_preference_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.display_prior_to_deblocking_preferred_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.dec_frame_buffering_constraint_flag);
                size += stream.ReadUnsignedIntGolomb(size, out this.deblocking_display_preference_repetition_period);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.deblocking_display_preference_cancel_flag);

            if (deblocking_display_preference_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.display_prior_to_deblocking_preferred_flag);
                size += stream.WriteUnsignedInt(1, this.dec_frame_buffering_constraint_flag);
                size += stream.WriteUnsignedIntGolomb(this.deblocking_display_preference_repetition_period);
            }

            return size;
        }

    }

    /*
   

stereo_video_info( payloadSize ) {  
 field_views_flag 5 u(1) 
 if( field_views_flag )   
  top_field_is_left_view_flag 5 u(1) 
 else {   
  current_frame_is_left_view_flag 5 u(1) 
  next_frame_is_second_view_flag 5 u(1) 
 }   
 left_view_self_contained_flag 5 u(1) 
 right_view_self_contained_flag 5 u(1) 
}
    */
    public class StereoVideoInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte field_views_flag;
        public byte FieldViewsFlag { get { return field_views_flag; } set { field_views_flag = value; } }
        private byte top_field_is_left_view_flag;
        public byte TopFieldIsLeftViewFlag { get { return top_field_is_left_view_flag; } set { top_field_is_left_view_flag = value; } }
        private byte current_frame_is_left_view_flag;
        public byte CurrentFrameIsLeftViewFlag { get { return current_frame_is_left_view_flag; } set { current_frame_is_left_view_flag = value; } }
        private byte next_frame_is_second_view_flag;
        public byte NextFrameIsSecondViewFlag { get { return next_frame_is_second_view_flag; } set { next_frame_is_second_view_flag = value; } }
        private byte left_view_self_contained_flag;
        public byte LeftViewSelfContainedFlag { get { return left_view_self_contained_flag; } set { left_view_self_contained_flag = value; } }
        private byte right_view_self_contained_flag;
        public byte RightViewSelfContainedFlag { get { return right_view_self_contained_flag; } set { right_view_self_contained_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public StereoVideoInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.field_views_flag);

            if (field_views_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.top_field_is_left_view_flag);
            }
            else
            {
                size += stream.ReadUnsignedInt(size, 1, out this.current_frame_is_left_view_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.next_frame_is_second_view_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.left_view_self_contained_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.right_view_self_contained_flag);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.field_views_flag);

            if (field_views_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.top_field_is_left_view_flag);
            }
            else
            {
                size += stream.WriteUnsignedInt(1, this.current_frame_is_left_view_flag);
                size += stream.WriteUnsignedInt(1, this.next_frame_is_second_view_flag);
            }
            size += stream.WriteUnsignedInt(1, this.left_view_self_contained_flag);
            size += stream.WriteUnsignedInt(1, this.right_view_self_contained_flag);

            return size;
        }

    }

    /*
  

post_filter_hint( payloadSize ) {  
 filter_hint_size_y 5 ue(v) 
 filter_hint_size_x 5 ue(v) 
 filter_hint_type 5 u(2) 
 for( colour_component = 0; colour_component < 3; colour_component ++ )   
  for( cy = 0; cy < filter_hint_size_y; cy ++ )   
   for( cx = 0; cx < filter_hint_size_x; cx ++ )   
    filter_hint[ colour_component ][ cy ][ cx ] 5 se(v) 
 additional_extension_flag 5 u(1) 
}
    */
    public class PostFilterHint : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint filter_hint_size_y;
        public uint FilterHintSizey { get { return filter_hint_size_y; } set { filter_hint_size_y = value; } }
        private uint filter_hint_size_x;
        public uint FilterHintSizex { get { return filter_hint_size_x; } set { filter_hint_size_x = value; } }
        private uint filter_hint_type;
        public uint FilterHintType { get { return filter_hint_type; } set { filter_hint_type = value; } }
        private int[][][] filter_hint;
        public int[][][] FilterHint { get { return filter_hint; } set { filter_hint = value; } }
        private byte additional_extension_flag;
        public byte AdditionalExtensionFlag { get { return additional_extension_flag; } set { additional_extension_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public PostFilterHint(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint colour_component = 0;
            uint cy = 0;
            uint cx = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.filter_hint_size_y);
            size += stream.ReadUnsignedIntGolomb(size, out this.filter_hint_size_x);
            size += stream.ReadUnsignedInt(size, 2, out this.filter_hint_type);

            this.filter_hint = new int[3][][];
            for (colour_component = 0; colour_component < 3; colour_component++)
            {

                this.filter_hint[colour_component] = new int[filter_hint_size_y][];
                for (cy = 0; cy < filter_hint_size_y; cy++)
                {

                    this.filter_hint[colour_component][cy] = new int[filter_hint_size_x];
                    for (cx = 0; cx < filter_hint_size_x; cx++)
                    {
                        size += stream.ReadSignedIntGolomb(size, out this.filter_hint[colour_component][cy][cx]);
                    }
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.additional_extension_flag);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint colour_component = 0;
            uint cy = 0;
            uint cx = 0;
            size += stream.WriteUnsignedIntGolomb(this.filter_hint_size_y);
            size += stream.WriteUnsignedIntGolomb(this.filter_hint_size_x);
            size += stream.WriteUnsignedInt(2, this.filter_hint_type);

            for (colour_component = 0; colour_component < 3; colour_component++)
            {

                for (cy = 0; cy < filter_hint_size_y; cy++)
                {

                    for (cx = 0; cx < filter_hint_size_x; cx++)
                    {
                        size += stream.WriteSignedIntGolomb(this.filter_hint[colour_component][cy][cx]);
                    }
                }
            }
            size += stream.WriteUnsignedInt(1, this.additional_extension_flag);

            return size;
        }

    }

    /*
  

tone_mapping_info( payloadSize ) {  
 tone_map_id 5 ue(v) 
 tone_map_cancel_flag 5 u(1) 
 if( !tone_map_cancel_flag ) {   
  tone_map_repetition_period 5 ue(v) 
  coded_data_bit_depth 5 u(8) 
  target_bit_depth 5 u(8) 
  tone_map_model_id 5 ue(v) 
  if( tone_map_model_id  ==  0 ) {   
   min_value 5 u(32) 
   max_value 5 u(32) 
  }   
  if( tone_map_model_id  ==  1 ) {   
   sigmoid_midpoint 5 u(32) 
   sigmoid_width 5 u(32) 
  }   
  if( tone_map_model_id  ==  2 )   
   for( i = 0; i < ( 1 << target_bit_depth ); i++ )   
    start_of_coded_interval[ i ] 5 u(v) 
  if( tone_map_model_id  ==  3 ) {   
   num_pivots 5 u(16) 
   for( i=0; i < num_pivots; i++ ) {   
    coded_pivot_value[ i ] 5 u(v) 
    target_pivot_value[ i ] 5 u(v) 
   }   
  }   
  if( tone_map_model_id  ==  4 ) {   
   camera_iso_speed_idc 5 u(8) 
   if( camera_iso_speed_idc ==  Extended_ISO )   
    camera_iso_speed_value 5 u(32) 
   exposure_index_idc 5 u(8) 
   if( exposure_index_idc  ==  Extended_ISO )   
    exposure_index_value 5 u(32) 
   exposure_compensation_value_sign_flag 5 u(1) 
   exposure_compensation_value_numerator 5 u(16) 
   exposure_compensation_value_denom_idc 5 u(16) 
   ref_screen_luminance_white 5 u(32) 
   extended_range_white_level 5 u(32) 
   nominal_black_level_luma_code_value 5 u(16) 
   nominal_white_level_luma_code_value 5 u(16) 
   extended_white_level_luma_code_value 5 u(16) 
  }   
 }   
}
    */
    public class ToneMappingInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint tone_map_id;
        public uint ToneMapId { get { return tone_map_id; } set { tone_map_id = value; } }
        private byte tone_map_cancel_flag;
        public byte ToneMapCancelFlag { get { return tone_map_cancel_flag; } set { tone_map_cancel_flag = value; } }
        private uint tone_map_repetition_period;
        public uint ToneMapRepetitionPeriod { get { return tone_map_repetition_period; } set { tone_map_repetition_period = value; } }
        private uint coded_data_bit_depth;
        public uint CodedDataBitDepth { get { return coded_data_bit_depth; } set { coded_data_bit_depth = value; } }
        private uint target_bit_depth;
        public uint TargetBitDepth { get { return target_bit_depth; } set { target_bit_depth = value; } }
        private uint tone_map_model_id;
        public uint ToneMapModelId { get { return tone_map_model_id; } set { tone_map_model_id = value; } }
        private uint min_value;
        public uint MinValue { get { return min_value; } set { min_value = value; } }
        private uint max_value;
        public uint MaxValue { get { return max_value; } set { max_value = value; } }
        private uint sigmoid_midpoint;
        public uint SigmoidMidpoint { get { return sigmoid_midpoint; } set { sigmoid_midpoint = value; } }
        private uint sigmoid_width;
        public uint SigmoidWidth { get { return sigmoid_width; } set { sigmoid_width = value; } }
        private uint[] start_of_coded_interval;
        public uint[] StartOfCodedInterval { get { return start_of_coded_interval; } set { start_of_coded_interval = value; } }
        private uint num_pivots;
        public uint NumPivots { get { return num_pivots; } set { num_pivots = value; } }
        private uint[] coded_pivot_value;
        public uint[] CodedPivotValue { get { return coded_pivot_value; } set { coded_pivot_value = value; } }
        private uint[] target_pivot_value;
        public uint[] TargetPivotValue { get { return target_pivot_value; } set { target_pivot_value = value; } }
        private uint camera_iso_speed_idc;
        public uint CameraIsoSpeedIdc { get { return camera_iso_speed_idc; } set { camera_iso_speed_idc = value; } }
        private uint camera_iso_speed_value;
        public uint CameraIsoSpeedValue { get { return camera_iso_speed_value; } set { camera_iso_speed_value = value; } }
        private uint exposure_index_idc;
        public uint ExposureIndexIdc { get { return exposure_index_idc; } set { exposure_index_idc = value; } }
        private uint exposure_index_value;
        public uint ExposureIndexValue { get { return exposure_index_value; } set { exposure_index_value = value; } }
        private byte exposure_compensation_value_sign_flag;
        public byte ExposureCompensationValueSignFlag { get { return exposure_compensation_value_sign_flag; } set { exposure_compensation_value_sign_flag = value; } }
        private uint exposure_compensation_value_numerator;
        public uint ExposureCompensationValueNumerator { get { return exposure_compensation_value_numerator; } set { exposure_compensation_value_numerator = value; } }
        private uint exposure_compensation_value_denom_idc;
        public uint ExposureCompensationValueDenomIdc { get { return exposure_compensation_value_denom_idc; } set { exposure_compensation_value_denom_idc = value; } }
        private uint ref_screen_luminance_white;
        public uint RefScreenLuminanceWhite { get { return ref_screen_luminance_white; } set { ref_screen_luminance_white = value; } }
        private uint extended_range_white_level;
        public uint ExtendedRangeWhiteLevel { get { return extended_range_white_level; } set { extended_range_white_level = value; } }
        private uint nominal_black_level_luma_code_value;
        public uint NominalBlackLevelLumaCodeValue { get { return nominal_black_level_luma_code_value; } set { nominal_black_level_luma_code_value = value; } }
        private uint nominal_white_level_luma_code_value;
        public uint NominalWhiteLevelLumaCodeValue { get { return nominal_white_level_luma_code_value; } set { nominal_white_level_luma_code_value = value; } }
        private uint extended_white_level_luma_code_value;
        public uint ExtendedWhiteLevelLumaCodeValue { get { return extended_white_level_luma_code_value; } set { extended_white_level_luma_code_value = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ToneMappingInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.tone_map_id);
            size += stream.ReadUnsignedInt(size, 1, out this.tone_map_cancel_flag);

            if (tone_map_cancel_flag == 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.tone_map_repetition_period);
                size += stream.ReadUnsignedInt(size, 8, out this.coded_data_bit_depth);
                size += stream.ReadUnsignedInt(size, 8, out this.target_bit_depth);
                size += stream.ReadUnsignedIntGolomb(size, out this.tone_map_model_id);

                if (tone_map_model_id == 0)
                {
                    size += stream.ReadUnsignedInt(size, 32, out this.min_value);
                    size += stream.ReadUnsignedInt(size, 32, out this.max_value);
                }

                if (tone_map_model_id == 1)
                {
                    size += stream.ReadUnsignedInt(size, 32, out this.sigmoid_midpoint);
                    size += stream.ReadUnsignedInt(size, 32, out this.sigmoid_width);
                }

                if (tone_map_model_id == 2)
                {

                    this.start_of_coded_interval = new uint[(1 << (int)target_bit_depth)];
                    for (i = 0; i < (1 << (int)target_bit_depth); i++)
                    {
                        size += stream.ReadUnsignedIntVariable(size, (((context.Sei.SeiMessage.SeiPayload.ToneMappingInfo.CodedDataBitDepth + 7) >> 3) << 3), out this.start_of_coded_interval[i]);
                    }
                }

                if (tone_map_model_id == 3)
                {
                    size += stream.ReadUnsignedInt(size, 16, out this.num_pivots);

                    this.coded_pivot_value = new uint[num_pivots];
                    this.target_pivot_value = new uint[num_pivots];
                    for (i = 0; i < num_pivots; i++)
                    {
                        size += stream.ReadUnsignedIntVariable(size, (((context.Sei.SeiMessage.SeiPayload.ToneMappingInfo.CodedDataBitDepth + 7) >> 3) << 3), out this.coded_pivot_value[i]);
                        size += stream.ReadUnsignedIntVariable(size, (((context.Sei.SeiMessage.SeiPayload.ToneMappingInfo.CodedDataBitDepth + 7) >> 3) << 3), out this.target_pivot_value[i]);
                    }
                }

                if (tone_map_model_id == 4)
                {
                    size += stream.ReadUnsignedInt(size, 8, out this.camera_iso_speed_idc);

                    if (camera_iso_speed_idc == H264Constants.Extended_ISO)
                    {
                        size += stream.ReadUnsignedInt(size, 32, out this.camera_iso_speed_value);
                    }
                    size += stream.ReadUnsignedInt(size, 8, out this.exposure_index_idc);

                    if (exposure_index_idc == H264Constants.Extended_ISO)
                    {
                        size += stream.ReadUnsignedInt(size, 32, out this.exposure_index_value);
                    }
                    size += stream.ReadUnsignedInt(size, 1, out this.exposure_compensation_value_sign_flag);
                    size += stream.ReadUnsignedInt(size, 16, out this.exposure_compensation_value_numerator);
                    size += stream.ReadUnsignedInt(size, 16, out this.exposure_compensation_value_denom_idc);
                    size += stream.ReadUnsignedInt(size, 32, out this.ref_screen_luminance_white);
                    size += stream.ReadUnsignedInt(size, 32, out this.extended_range_white_level);
                    size += stream.ReadUnsignedInt(size, 16, out this.nominal_black_level_luma_code_value);
                    size += stream.ReadUnsignedInt(size, 16, out this.nominal_white_level_luma_code_value);
                    size += stream.ReadUnsignedInt(size, 16, out this.extended_white_level_luma_code_value);
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedIntGolomb(this.tone_map_id);
            size += stream.WriteUnsignedInt(1, this.tone_map_cancel_flag);

            if (tone_map_cancel_flag == 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.tone_map_repetition_period);
                size += stream.WriteUnsignedInt(8, this.coded_data_bit_depth);
                size += stream.WriteUnsignedInt(8, this.target_bit_depth);
                size += stream.WriteUnsignedIntGolomb(this.tone_map_model_id);

                if (tone_map_model_id == 0)
                {
                    size += stream.WriteUnsignedInt(32, this.min_value);
                    size += stream.WriteUnsignedInt(32, this.max_value);
                }

                if (tone_map_model_id == 1)
                {
                    size += stream.WriteUnsignedInt(32, this.sigmoid_midpoint);
                    size += stream.WriteUnsignedInt(32, this.sigmoid_width);
                }

                if (tone_map_model_id == 2)
                {

                    for (i = 0; i < (1 << (int)target_bit_depth); i++)
                    {
                        size += stream.WriteUnsignedIntVariable((((context.Sei.SeiMessage.SeiPayload.ToneMappingInfo.CodedDataBitDepth + 7) >> 3) << 3), this.start_of_coded_interval[i]);
                    }
                }

                if (tone_map_model_id == 3)
                {
                    size += stream.WriteUnsignedInt(16, this.num_pivots);

                    for (i = 0; i < num_pivots; i++)
                    {
                        size += stream.WriteUnsignedIntVariable((((context.Sei.SeiMessage.SeiPayload.ToneMappingInfo.CodedDataBitDepth + 7) >> 3) << 3), this.coded_pivot_value[i]);
                        size += stream.WriteUnsignedIntVariable((((context.Sei.SeiMessage.SeiPayload.ToneMappingInfo.CodedDataBitDepth + 7) >> 3) << 3), this.target_pivot_value[i]);
                    }
                }

                if (tone_map_model_id == 4)
                {
                    size += stream.WriteUnsignedInt(8, this.camera_iso_speed_idc);

                    if (camera_iso_speed_idc == H264Constants.Extended_ISO)
                    {
                        size += stream.WriteUnsignedInt(32, this.camera_iso_speed_value);
                    }
                    size += stream.WriteUnsignedInt(8, this.exposure_index_idc);

                    if (exposure_index_idc == H264Constants.Extended_ISO)
                    {
                        size += stream.WriteUnsignedInt(32, this.exposure_index_value);
                    }
                    size += stream.WriteUnsignedInt(1, this.exposure_compensation_value_sign_flag);
                    size += stream.WriteUnsignedInt(16, this.exposure_compensation_value_numerator);
                    size += stream.WriteUnsignedInt(16, this.exposure_compensation_value_denom_idc);
                    size += stream.WriteUnsignedInt(32, this.ref_screen_luminance_white);
                    size += stream.WriteUnsignedInt(32, this.extended_range_white_level);
                    size += stream.WriteUnsignedInt(16, this.nominal_black_level_luma_code_value);
                    size += stream.WriteUnsignedInt(16, this.nominal_white_level_luma_code_value);
                    size += stream.WriteUnsignedInt(16, this.extended_white_level_luma_code_value);
                }
            }

            return size;
        }

    }

    /*
   

frame_packing_arrangement( payloadSize ) {  
 frame_packing_arrangement_id 5 ue(v) 
 frame_packing_arrangement_cancel_flag 5 u(1) 
 if( !frame_packing_arrangement_cancel_flag ) {   
  frame_packing_arrangement_type 5 u(7) 
  quincunx_sampling_flag 5 u(1) 
  content_interpretation_type 5 u(6) 
  spatial_flipping_flag 5 u(1) 
  frame0_flipped_flag 5 u(1) 
  field_views_flag 5 u(1) 
  current_frame_is_frame0_flag 5 u(1) 
  frame0_self_contained_flag 5 u(1) 
  frame1_self_contained_flag 5 u(1) 
  if( !quincunx_sampling_flag  && 
   frame_packing_arrangement_type  !=  5 ) { 
  
   frame0_grid_position_x 5 u(4) 
   frame0_grid_position_y 5 u(4) 
   frame1_grid_position_x 5 u(4) 
   frame1_grid_position_y 5 u(4) 
  }   
  frame_packing_arrangement_reserved_byte 5 u(8) 
  frame_packing_arrangement_repetition_period 5 ue(v) 
 }   
 frame_packing_arrangement_extension_flag 5 u(1) 
}
    */
    public class FramePackingArrangement : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint frame_packing_arrangement_id;
        public uint FramePackingArrangementId { get { return frame_packing_arrangement_id; } set { frame_packing_arrangement_id = value; } }
        private byte frame_packing_arrangement_cancel_flag;
        public byte FramePackingArrangementCancelFlag { get { return frame_packing_arrangement_cancel_flag; } set { frame_packing_arrangement_cancel_flag = value; } }
        private uint frame_packing_arrangement_type;
        public uint FramePackingArrangementType { get { return frame_packing_arrangement_type; } set { frame_packing_arrangement_type = value; } }
        private byte quincunx_sampling_flag;
        public byte QuincunxSamplingFlag { get { return quincunx_sampling_flag; } set { quincunx_sampling_flag = value; } }
        private uint content_interpretation_type;
        public uint ContentInterpretationType { get { return content_interpretation_type; } set { content_interpretation_type = value; } }
        private byte spatial_flipping_flag;
        public byte SpatialFlippingFlag { get { return spatial_flipping_flag; } set { spatial_flipping_flag = value; } }
        private byte frame0_flipped_flag;
        public byte Frame0FlippedFlag { get { return frame0_flipped_flag; } set { frame0_flipped_flag = value; } }
        private byte field_views_flag;
        public byte FieldViewsFlag { get { return field_views_flag; } set { field_views_flag = value; } }
        private byte current_frame_is_frame0_flag;
        public byte CurrentFrameIsFrame0Flag { get { return current_frame_is_frame0_flag; } set { current_frame_is_frame0_flag = value; } }
        private byte frame0_self_contained_flag;
        public byte Frame0SelfContainedFlag { get { return frame0_self_contained_flag; } set { frame0_self_contained_flag = value; } }
        private byte frame1_self_contained_flag;
        public byte Frame1SelfContainedFlag { get { return frame1_self_contained_flag; } set { frame1_self_contained_flag = value; } }
        private uint frame0_grid_position_x;
        public uint Frame0GridPositionx { get { return frame0_grid_position_x; } set { frame0_grid_position_x = value; } }
        private uint frame0_grid_position_y;
        public uint Frame0GridPositiony { get { return frame0_grid_position_y; } set { frame0_grid_position_y = value; } }
        private uint frame1_grid_position_x;
        public uint Frame1GridPositionx { get { return frame1_grid_position_x; } set { frame1_grid_position_x = value; } }
        private uint frame1_grid_position_y;
        public uint Frame1GridPositiony { get { return frame1_grid_position_y; } set { frame1_grid_position_y = value; } }
        private uint frame_packing_arrangement_reserved_byte;
        public uint FramePackingArrangementReservedByte { get { return frame_packing_arrangement_reserved_byte; } set { frame_packing_arrangement_reserved_byte = value; } }
        private uint frame_packing_arrangement_repetition_period;
        public uint FramePackingArrangementRepetitionPeriod { get { return frame_packing_arrangement_repetition_period; } set { frame_packing_arrangement_repetition_period = value; } }
        private byte frame_packing_arrangement_extension_flag;
        public byte FramePackingArrangementExtensionFlag { get { return frame_packing_arrangement_extension_flag; } set { frame_packing_arrangement_extension_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public FramePackingArrangement(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedIntGolomb(size, out this.frame_packing_arrangement_id);
            size += stream.ReadUnsignedInt(size, 1, out this.frame_packing_arrangement_cancel_flag);

            if (frame_packing_arrangement_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 7, out this.frame_packing_arrangement_type);
                size += stream.ReadUnsignedInt(size, 1, out this.quincunx_sampling_flag);
                size += stream.ReadUnsignedInt(size, 6, out this.content_interpretation_type);
                size += stream.ReadUnsignedInt(size, 1, out this.spatial_flipping_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.frame0_flipped_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.field_views_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.current_frame_is_frame0_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.frame0_self_contained_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.frame1_self_contained_flag);

                if (quincunx_sampling_flag == 0 &&
   frame_packing_arrangement_type != 5)
                {
                    size += stream.ReadUnsignedInt(size, 4, out this.frame0_grid_position_x);
                    size += stream.ReadUnsignedInt(size, 4, out this.frame0_grid_position_y);
                    size += stream.ReadUnsignedInt(size, 4, out this.frame1_grid_position_x);
                    size += stream.ReadUnsignedInt(size, 4, out this.frame1_grid_position_y);
                }
                size += stream.ReadUnsignedInt(size, 8, out this.frame_packing_arrangement_reserved_byte);
                size += stream.ReadUnsignedIntGolomb(size, out this.frame_packing_arrangement_repetition_period);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.frame_packing_arrangement_extension_flag);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedIntGolomb(this.frame_packing_arrangement_id);
            size += stream.WriteUnsignedInt(1, this.frame_packing_arrangement_cancel_flag);

            if (frame_packing_arrangement_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(7, this.frame_packing_arrangement_type);
                size += stream.WriteUnsignedInt(1, this.quincunx_sampling_flag);
                size += stream.WriteUnsignedInt(6, this.content_interpretation_type);
                size += stream.WriteUnsignedInt(1, this.spatial_flipping_flag);
                size += stream.WriteUnsignedInt(1, this.frame0_flipped_flag);
                size += stream.WriteUnsignedInt(1, this.field_views_flag);
                size += stream.WriteUnsignedInt(1, this.current_frame_is_frame0_flag);
                size += stream.WriteUnsignedInt(1, this.frame0_self_contained_flag);
                size += stream.WriteUnsignedInt(1, this.frame1_self_contained_flag);

                if (quincunx_sampling_flag == 0 &&
   frame_packing_arrangement_type != 5)
                {
                    size += stream.WriteUnsignedInt(4, this.frame0_grid_position_x);
                    size += stream.WriteUnsignedInt(4, this.frame0_grid_position_y);
                    size += stream.WriteUnsignedInt(4, this.frame1_grid_position_x);
                    size += stream.WriteUnsignedInt(4, this.frame1_grid_position_y);
                }
                size += stream.WriteUnsignedInt(8, this.frame_packing_arrangement_reserved_byte);
                size += stream.WriteUnsignedIntGolomb(this.frame_packing_arrangement_repetition_period);
            }
            size += stream.WriteUnsignedInt(1, this.frame_packing_arrangement_extension_flag);

            return size;
        }

    }

    /*
  

display_orientation( payloadSize ) { 
 display_orientation_cancel_flag 5 u(1) 
 if( !display_orientation_cancel_flag ) {   
  hor_flip 5 u(1) 
  ver_flip 5 u(1) 
  anticlockwise_rotation 5 u(16) 
  display_orientation_repetition_period 5 ue(v) 
  display_orientation_extension_flag 5 u(1) 
 }   
}
    */
    public class DisplayOrientation : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte display_orientation_cancel_flag;
        public byte DisplayOrientationCancelFlag { get { return display_orientation_cancel_flag; } set { display_orientation_cancel_flag = value; } }
        private byte hor_flip;
        public byte HorFlip { get { return hor_flip; } set { hor_flip = value; } }
        private byte ver_flip;
        public byte VerFlip { get { return ver_flip; } set { ver_flip = value; } }
        private uint anticlockwise_rotation;
        public uint AnticlockwiseRotation { get { return anticlockwise_rotation; } set { anticlockwise_rotation = value; } }
        private uint display_orientation_repetition_period;
        public uint DisplayOrientationRepetitionPeriod { get { return display_orientation_repetition_period; } set { display_orientation_repetition_period = value; } }
        private byte display_orientation_extension_flag;
        public byte DisplayOrientationExtensionFlag { get { return display_orientation_extension_flag; } set { display_orientation_extension_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public DisplayOrientation(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.display_orientation_cancel_flag);

            if (display_orientation_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.hor_flip);
                size += stream.ReadUnsignedInt(size, 1, out this.ver_flip);
                size += stream.ReadUnsignedInt(size, 16, out this.anticlockwise_rotation);
                size += stream.ReadUnsignedIntGolomb(size, out this.display_orientation_repetition_period);
                size += stream.ReadUnsignedInt(size, 1, out this.display_orientation_extension_flag);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.display_orientation_cancel_flag);

            if (display_orientation_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.hor_flip);
                size += stream.WriteUnsignedInt(1, this.ver_flip);
                size += stream.WriteUnsignedInt(16, this.anticlockwise_rotation);
                size += stream.WriteUnsignedIntGolomb(this.display_orientation_repetition_period);
                size += stream.WriteUnsignedInt(1, this.display_orientation_extension_flag);
            }

            return size;
        }

    }

    /*
   

   mastering_display_colour_volume( payloadSize ) { 
   for( c = 0; c < 3; c++ ) {    
      display_primaries_x[ c ] 5 u(16) 
      display_primaries_y[ c ] 5 u(16) 
   }   
   white_point_x 5 u(16) 
   white_point_y 5 u(16) 

   max_display_mastering_luminance 5 u(32) 
   min_display_mastering_luminance 5 u(32) 
}
    */
    public class MasteringDisplayColourVolume : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint[] display_primaries_x;
        public uint[] DisplayPrimariesx { get { return display_primaries_x; } set { display_primaries_x = value; } }
        private uint[] display_primaries_y;
        public uint[] DisplayPrimariesy { get { return display_primaries_y; } set { display_primaries_y = value; } }
        private uint white_point_x;
        public uint WhitePointx { get { return white_point_x; } set { white_point_x = value; } }
        private uint white_point_y;
        public uint WhitePointy { get { return white_point_y; } set { white_point_y = value; } }
        private uint max_display_mastering_luminance;
        public uint MaxDisplayMasteringLuminance { get { return max_display_mastering_luminance; } set { max_display_mastering_luminance = value; } }
        private uint min_display_mastering_luminance;
        public uint MinDisplayMasteringLuminance { get { return min_display_mastering_luminance; } set { min_display_mastering_luminance = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public MasteringDisplayColourVolume(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint c = 0;

            this.display_primaries_x = new uint[3];
            this.display_primaries_y = new uint[3];
            for (c = 0; c < 3; c++)
            {
                size += stream.ReadUnsignedInt(size, 16, out this.display_primaries_x[c]);
                size += stream.ReadUnsignedInt(size, 16, out this.display_primaries_y[c]);
            }
            size += stream.ReadUnsignedInt(size, 16, out this.white_point_x);
            size += stream.ReadUnsignedInt(size, 16, out this.white_point_y);
            size += stream.ReadUnsignedInt(size, 32, out this.max_display_mastering_luminance);
            size += stream.ReadUnsignedInt(size, 32, out this.min_display_mastering_luminance);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint c = 0;

            for (c = 0; c < 3; c++)
            {
                size += stream.WriteUnsignedInt(16, this.display_primaries_x[c]);
                size += stream.WriteUnsignedInt(16, this.display_primaries_y[c]);
            }
            size += stream.WriteUnsignedInt(16, this.white_point_x);
            size += stream.WriteUnsignedInt(16, this.white_point_y);
            size += stream.WriteUnsignedInt(32, this.max_display_mastering_luminance);
            size += stream.WriteUnsignedInt(32, this.min_display_mastering_luminance);

            return size;
        }

    }

    /*


colour_remapping_info( payloadSize ) {  
 colour_remap_id 5 ue(v) 
 colour_remap_cancel_flag 5 u(1) 
 if( !colour_remap_cancel_flag ) {   
  colour_remap_repetition_period 5 ue(v) 
  colour_remap_video_signal_info_present_flag 5 u(1) 
  if( colour_remap_video_signal_info_present_flag ) {   
   colour_remap_full_range_flag 5 u(1) 
   colour_remap_primaries 5 u(8) 
   colour_remap_transfer_function 5 u(8) 
   colour_remap_matrix_coefficients 5 u(8) 
  }   
  colour_remap_input_bit_depth 5 u(8) 
  colour_remap_output_bit_depth 5 u(8) 
  for( c = 0; c < 3; c++ ) {   
   pre_lut_num_val_minus1[ c ] 5 u(8) 
   if( pre_lut_num_val_minus1[ c ] > 0 )   
    for( i = 0; i  <=  pre_lut_num_val_minus1[ c ]; i++ ) {   
     pre_lut_coded_value[ c ][ i ] 5 u(v) 
     pre_lut_target_value[ c ][ i ] 5 u(v) 
    }   
  }   
  colour_remap_matrix_present_flag 5 u(1) 
  if( colour_remap_matrix_present_flag ) {   
   log2_matrix_denom 5 u(4) 
   for( c = 0; c < 3; c++ )   
    for( i = 0; i < 3; i++ )   
     colour_remap_coeffs[ c ][ i ] 5 se(v) 
  }   
  for( c = 0; c < 3; c++ ) {   
   post_lut_num_val_minus1[ c ] 5 u(8) 
   if( post_lut_num_val_minus1[ c ] > 0 )   
    for( i = 0; i  <=  post_lut_num_val_minus1[ c ]; i++ ) {   
     post_lut_coded_value[ c ][ i ] 5 u(v) 
     post_lut_target_value[ c ][ i ] 5 u(v) 
    }   
  }   
 }   
}
    */
    public class ColourRemappingInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint colour_remap_id;
        public uint ColourRemapId { get { return colour_remap_id; } set { colour_remap_id = value; } }
        private byte colour_remap_cancel_flag;
        public byte ColourRemapCancelFlag { get { return colour_remap_cancel_flag; } set { colour_remap_cancel_flag = value; } }
        private uint colour_remap_repetition_period;
        public uint ColourRemapRepetitionPeriod { get { return colour_remap_repetition_period; } set { colour_remap_repetition_period = value; } }
        private byte colour_remap_video_signal_info_present_flag;
        public byte ColourRemapVideoSignalInfoPresentFlag { get { return colour_remap_video_signal_info_present_flag; } set { colour_remap_video_signal_info_present_flag = value; } }
        private byte colour_remap_full_range_flag;
        public byte ColourRemapFullRangeFlag { get { return colour_remap_full_range_flag; } set { colour_remap_full_range_flag = value; } }
        private uint colour_remap_primaries;
        public uint ColourRemapPrimaries { get { return colour_remap_primaries; } set { colour_remap_primaries = value; } }
        private uint colour_remap_transfer_function;
        public uint ColourRemapTransferFunction { get { return colour_remap_transfer_function; } set { colour_remap_transfer_function = value; } }
        private uint colour_remap_matrix_coefficients;
        public uint ColourRemapMatrixCoefficients { get { return colour_remap_matrix_coefficients; } set { colour_remap_matrix_coefficients = value; } }
        private uint colour_remap_input_bit_depth;
        public uint ColourRemapInputBitDepth { get { return colour_remap_input_bit_depth; } set { colour_remap_input_bit_depth = value; } }
        private uint colour_remap_output_bit_depth;
        public uint ColourRemapOutputBitDepth { get { return colour_remap_output_bit_depth; } set { colour_remap_output_bit_depth = value; } }
        private uint[] pre_lut_num_val_minus1;
        public uint[] PreLutNumValMinus1 { get { return pre_lut_num_val_minus1; } set { pre_lut_num_val_minus1 = value; } }
        private uint[][] pre_lut_coded_value;
        public uint[][] PreLutCodedValue { get { return pre_lut_coded_value; } set { pre_lut_coded_value = value; } }
        private uint[][] pre_lut_target_value;
        public uint[][] PreLutTargetValue { get { return pre_lut_target_value; } set { pre_lut_target_value = value; } }
        private byte colour_remap_matrix_present_flag;
        public byte ColourRemapMatrixPresentFlag { get { return colour_remap_matrix_present_flag; } set { colour_remap_matrix_present_flag = value; } }
        private uint log2_matrix_denom;
        public uint Log2MatrixDenom { get { return log2_matrix_denom; } set { log2_matrix_denom = value; } }
        private int[][] colour_remap_coeffs;
        public int[][] ColourRemapCoeffs { get { return colour_remap_coeffs; } set { colour_remap_coeffs = value; } }
        private uint[] post_lut_num_val_minus1;
        public uint[] PostLutNumValMinus1 { get { return post_lut_num_val_minus1; } set { post_lut_num_val_minus1 = value; } }
        private uint[][] post_lut_coded_value;
        public uint[][] PostLutCodedValue { get { return post_lut_coded_value; } set { post_lut_coded_value = value; } }
        private uint[][] post_lut_target_value;
        public uint[][] PostLutTargetValue { get { return post_lut_target_value; } set { post_lut_target_value = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ColourRemappingInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint c = 0;
            uint i = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.colour_remap_id);
            size += stream.ReadUnsignedInt(size, 1, out this.colour_remap_cancel_flag);

            if (colour_remap_cancel_flag == 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.colour_remap_repetition_period);
                size += stream.ReadUnsignedInt(size, 1, out this.colour_remap_video_signal_info_present_flag);

                if (colour_remap_video_signal_info_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.colour_remap_full_range_flag);
                    size += stream.ReadUnsignedInt(size, 8, out this.colour_remap_primaries);
                    size += stream.ReadUnsignedInt(size, 8, out this.colour_remap_transfer_function);
                    size += stream.ReadUnsignedInt(size, 8, out this.colour_remap_matrix_coefficients);
                }
                size += stream.ReadUnsignedInt(size, 8, out this.colour_remap_input_bit_depth);
                size += stream.ReadUnsignedInt(size, 8, out this.colour_remap_output_bit_depth);

                this.pre_lut_num_val_minus1 = new uint[3];
                this.pre_lut_coded_value = new uint[3][];
                this.pre_lut_target_value = new uint[3][];
                for (c = 0; c < 3; c++)
                {
                    size += stream.ReadUnsignedInt(size, 8, out this.pre_lut_num_val_minus1[c]);

                    if (pre_lut_num_val_minus1[c] > 0)
                    {

                        this.pre_lut_coded_value[c] = new uint[pre_lut_num_val_minus1[c] + 1];
                        this.pre_lut_target_value[c] = new uint[pre_lut_num_val_minus1[c] + 1];
                        for (i = 0; i <= pre_lut_num_val_minus1[c]; i++)
                        {
                            size += stream.ReadUnsignedIntVariable(size, (((context.Sei.SeiMessage.SeiPayload.ColourRemappingInfo.ColourRemapInputBitDepth + 7) >> 3) << 3), out this.pre_lut_coded_value[c][i]);
                            size += stream.ReadUnsignedIntVariable(size, (((context.Sei.SeiMessage.SeiPayload.ColourRemappingInfo.ColourRemapInputBitDepth + 7) >> 3) << 3), out this.pre_lut_target_value[c][i]);
                        }
                    }
                }
                size += stream.ReadUnsignedInt(size, 1, out this.colour_remap_matrix_present_flag);

                if (colour_remap_matrix_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 4, out this.log2_matrix_denom);

                    this.colour_remap_coeffs = new int[3][];
                    for (c = 0; c < 3; c++)
                    {

                        this.colour_remap_coeffs[c] = new int[3];
                        for (i = 0; i < 3; i++)
                        {
                            size += stream.ReadSignedIntGolomb(size, out this.colour_remap_coeffs[c][i]);
                        }
                    }
                }

                this.post_lut_num_val_minus1 = new uint[3];
                this.post_lut_coded_value = new uint[3][];
                this.post_lut_target_value = new uint[3][];
                for (c = 0; c < 3; c++)
                {
                    size += stream.ReadUnsignedInt(size, 8, out this.post_lut_num_val_minus1[c]);

                    if (post_lut_num_val_minus1[c] > 0)
                    {

                        this.post_lut_coded_value[c] = new uint[post_lut_num_val_minus1[c] + 1];
                        this.post_lut_target_value[c] = new uint[post_lut_num_val_minus1[c] + 1];
                        for (i = 0; i <= post_lut_num_val_minus1[c]; i++)
                        {
                            size += stream.ReadUnsignedIntVariable(size, (((context.Sei.SeiMessage.SeiPayload.ColourRemappingInfo.ColourRemapOutputBitDepth + 7) >> 3) << 3), out this.post_lut_coded_value[c][i]);
                            size += stream.ReadUnsignedIntVariable(size, (((context.Sei.SeiMessage.SeiPayload.ColourRemappingInfo.ColourRemapOutputBitDepth + 7) >> 3) << 3), out this.post_lut_target_value[c][i]);
                        }
                    }
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint c = 0;
            uint i = 0;
            size += stream.WriteUnsignedIntGolomb(this.colour_remap_id);
            size += stream.WriteUnsignedInt(1, this.colour_remap_cancel_flag);

            if (colour_remap_cancel_flag == 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.colour_remap_repetition_period);
                size += stream.WriteUnsignedInt(1, this.colour_remap_video_signal_info_present_flag);

                if (colour_remap_video_signal_info_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.colour_remap_full_range_flag);
                    size += stream.WriteUnsignedInt(8, this.colour_remap_primaries);
                    size += stream.WriteUnsignedInt(8, this.colour_remap_transfer_function);
                    size += stream.WriteUnsignedInt(8, this.colour_remap_matrix_coefficients);
                }
                size += stream.WriteUnsignedInt(8, this.colour_remap_input_bit_depth);
                size += stream.WriteUnsignedInt(8, this.colour_remap_output_bit_depth);

                for (c = 0; c < 3; c++)
                {
                    size += stream.WriteUnsignedInt(8, this.pre_lut_num_val_minus1[c]);

                    if (pre_lut_num_val_minus1[c] > 0)
                    {

                        for (i = 0; i <= pre_lut_num_val_minus1[c]; i++)
                        {
                            size += stream.WriteUnsignedIntVariable((((context.Sei.SeiMessage.SeiPayload.ColourRemappingInfo.ColourRemapInputBitDepth + 7) >> 3) << 3), this.pre_lut_coded_value[c][i]);
                            size += stream.WriteUnsignedIntVariable((((context.Sei.SeiMessage.SeiPayload.ColourRemappingInfo.ColourRemapInputBitDepth + 7) >> 3) << 3), this.pre_lut_target_value[c][i]);
                        }
                    }
                }
                size += stream.WriteUnsignedInt(1, this.colour_remap_matrix_present_flag);

                if (colour_remap_matrix_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(4, this.log2_matrix_denom);

                    for (c = 0; c < 3; c++)
                    {

                        for (i = 0; i < 3; i++)
                        {
                            size += stream.WriteSignedIntGolomb(this.colour_remap_coeffs[c][i]);
                        }
                    }
                }

                for (c = 0; c < 3; c++)
                {
                    size += stream.WriteUnsignedInt(8, this.post_lut_num_val_minus1[c]);

                    if (post_lut_num_val_minus1[c] > 0)
                    {

                        for (i = 0; i <= post_lut_num_val_minus1[c]; i++)
                        {
                            size += stream.WriteUnsignedIntVariable((((context.Sei.SeiMessage.SeiPayload.ColourRemappingInfo.ColourRemapOutputBitDepth + 7) >> 3) << 3), this.post_lut_coded_value[c][i]);
                            size += stream.WriteUnsignedIntVariable((((context.Sei.SeiMessage.SeiPayload.ColourRemappingInfo.ColourRemapOutputBitDepth + 7) >> 3) << 3), this.post_lut_target_value[c][i]);
                        }
                    }
                }
            }

            return size;
        }

    }

    /*
   

content_light_level_info( payloadSize ) {  
 max_content_light_level 5 u(16) 
 max_pic_average_light_level 5 u(16) 
}
    */
    public class ContentLightLevelInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint max_content_light_level;
        public uint MaxContentLightLevel { get { return max_content_light_level; } set { max_content_light_level = value; } }
        private uint max_pic_average_light_level;
        public uint MaxPicAverageLightLevel { get { return max_pic_average_light_level; } set { max_pic_average_light_level = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ContentLightLevelInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 16, out this.max_content_light_level);
            size += stream.ReadUnsignedInt(size, 16, out this.max_pic_average_light_level);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(16, this.max_content_light_level);
            size += stream.WriteUnsignedInt(16, this.max_pic_average_light_level);

            return size;
        }

    }

    /*
   

alternative_transfer_characteristics( payloadSize ) {  
 preferred_transfer_characteristics 5 u(8) 
}
    */
    public class AlternativeTransferCharacteristics : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint preferred_transfer_characteristics;
        public uint PreferredTransferCharacteristics { get { return preferred_transfer_characteristics; } set { preferred_transfer_characteristics = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public AlternativeTransferCharacteristics(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 8, out this.preferred_transfer_characteristics);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(8, this.preferred_transfer_characteristics);

            return size;
        }

    }

    /*
   

content_colour_volume( payloadSize ) {  
 ccv_cancel_flag 5 u(1) 
 if( !ccv_cancel_flag ) {   
  ccv_persistence_flag 5 u(1) 
  ccv_primaries_present_flag 5 u(1) 
  ccv_min_luminance_value_present_flag 5 u(1) 
  ccv_max_luminance_value_present_flag 5 u(1) 
  ccv_avg_luminance_value_present_flag 5 u(1) 
  ccv_reserved_zero_2bits 5 u(2) 
  if( ccv_primaries_present_flag )   
   for( c = 0; c < 3; c++ ) {   
    ccv_primaries_x[ c ] 5 i(32) 
    ccv_primaries_y[ c ] 5 i(32) 
   }   
  if( ccv_min_luminance_value_present_flag )   
   ccv_min_luminance_value 5 u(32) 
  if( ccv_max_luminance_value_present_flag )   
   ccv_max_luminance_value 5 u(32) 
  if( ccv_avg_luminance_value_present_flag )   
   ccv_avg_luminance_value 5 u(32) 
 }   
}
    */
    public class ContentColourVolume : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte ccv_cancel_flag;
        public byte CcvCancelFlag { get { return ccv_cancel_flag; } set { ccv_cancel_flag = value; } }
        private byte ccv_persistence_flag;
        public byte CcvPersistenceFlag { get { return ccv_persistence_flag; } set { ccv_persistence_flag = value; } }
        private byte ccv_primaries_present_flag;
        public byte CcvPrimariesPresentFlag { get { return ccv_primaries_present_flag; } set { ccv_primaries_present_flag = value; } }
        private byte ccv_min_luminance_value_present_flag;
        public byte CcvMinLuminanceValuePresentFlag { get { return ccv_min_luminance_value_present_flag; } set { ccv_min_luminance_value_present_flag = value; } }
        private byte ccv_max_luminance_value_present_flag;
        public byte CcvMaxLuminanceValuePresentFlag { get { return ccv_max_luminance_value_present_flag; } set { ccv_max_luminance_value_present_flag = value; } }
        private byte ccv_avg_luminance_value_present_flag;
        public byte CcvAvgLuminanceValuePresentFlag { get { return ccv_avg_luminance_value_present_flag; } set { ccv_avg_luminance_value_present_flag = value; } }
        private uint ccv_reserved_zero_2bits;
        public uint CcvReservedZero2bits { get { return ccv_reserved_zero_2bits; } set { ccv_reserved_zero_2bits = value; } }
        private int[] ccv_primaries_x;
        public int[] CcvPrimariesx { get { return ccv_primaries_x; } set { ccv_primaries_x = value; } }
        private int[] ccv_primaries_y;
        public int[] CcvPrimariesy { get { return ccv_primaries_y; } set { ccv_primaries_y = value; } }
        private uint ccv_min_luminance_value;
        public uint CcvMinLuminanceValue { get { return ccv_min_luminance_value; } set { ccv_min_luminance_value = value; } }
        private uint ccv_max_luminance_value;
        public uint CcvMaxLuminanceValue { get { return ccv_max_luminance_value; } set { ccv_max_luminance_value = value; } }
        private uint ccv_avg_luminance_value;
        public uint CcvAvgLuminanceValue { get { return ccv_avg_luminance_value; } set { ccv_avg_luminance_value = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ContentColourVolume(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint c = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.ccv_cancel_flag);

            if (ccv_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.ccv_persistence_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.ccv_primaries_present_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.ccv_min_luminance_value_present_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.ccv_max_luminance_value_present_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.ccv_avg_luminance_value_present_flag);
                size += stream.ReadUnsignedInt(size, 2, out this.ccv_reserved_zero_2bits);

                if (ccv_primaries_present_flag != 0)
                {

                    this.ccv_primaries_x = new int[3];
                    this.ccv_primaries_y = new int[3];
                    for (c = 0; c < 3; c++)
                    {
                        size += stream.ReadSignedInt(size, 32, out this.ccv_primaries_x[c]);
                        size += stream.ReadSignedInt(size, 32, out this.ccv_primaries_y[c]);
                    }
                }

                if (ccv_min_luminance_value_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 32, out this.ccv_min_luminance_value);
                }

                if (ccv_max_luminance_value_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 32, out this.ccv_max_luminance_value);
                }

                if (ccv_avg_luminance_value_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 32, out this.ccv_avg_luminance_value);
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint c = 0;
            size += stream.WriteUnsignedInt(1, this.ccv_cancel_flag);

            if (ccv_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.ccv_persistence_flag);
                size += stream.WriteUnsignedInt(1, this.ccv_primaries_present_flag);
                size += stream.WriteUnsignedInt(1, this.ccv_min_luminance_value_present_flag);
                size += stream.WriteUnsignedInt(1, this.ccv_max_luminance_value_present_flag);
                size += stream.WriteUnsignedInt(1, this.ccv_avg_luminance_value_present_flag);
                size += stream.WriteUnsignedInt(2, this.ccv_reserved_zero_2bits);

                if (ccv_primaries_present_flag != 0)
                {

                    for (c = 0; c < 3; c++)
                    {
                        size += stream.WriteSignedInt(32, this.ccv_primaries_x[c]);
                        size += stream.WriteSignedInt(32, this.ccv_primaries_y[c]);
                    }
                }

                if (ccv_min_luminance_value_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(32, this.ccv_min_luminance_value);
                }

                if (ccv_max_luminance_value_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(32, this.ccv_max_luminance_value);
                }

                if (ccv_avg_luminance_value_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(32, this.ccv_avg_luminance_value);
                }
            }

            return size;
        }

    }

    /*
   

ambient_viewing_environment( payloadSize ) {  
 ambient_illuminance 5 u(32) 
 ambient_light_x 5 u(16) 
 ambient_light_y 5 u(16) 
}
    */
    public class AmbientViewingEnvironment : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint ambient_illuminance;
        public uint AmbientIlluminance { get { return ambient_illuminance; } set { ambient_illuminance = value; } }
        private uint ambient_light_x;
        public uint AmbientLightx { get { return ambient_light_x; } set { ambient_light_x = value; } }
        private uint ambient_light_y;
        public uint AmbientLighty { get { return ambient_light_y; } set { ambient_light_y = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public AmbientViewingEnvironment(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 32, out this.ambient_illuminance);
            size += stream.ReadUnsignedInt(size, 16, out this.ambient_light_x);
            size += stream.ReadUnsignedInt(size, 16, out this.ambient_light_y);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(32, this.ambient_illuminance);
            size += stream.WriteUnsignedInt(16, this.ambient_light_x);
            size += stream.WriteUnsignedInt(16, this.ambient_light_y);

            return size;
        }

    }

    /*
   

equirectangular_projection( payloadSize ) {  
 erp_cancel_flag 5 u(1) 
 if( !erp_cancel_flag ) {
  erp_persistence_flag 5 u(1) 
  erp_padding_flag 5 u(1) 
  erp_reserved_zero_2bits 5 u(2) 
  if( erp_padding_flag  ==  1 ) {   
   gp_erp_type 5 u(3) 
   left_gb_erp_width 5 u(8) 
   right_gb_erp_width 5 u(8) 
  }   
 }   
}
    */
    public class EquirectangularProjection : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte erp_cancel_flag;
        public byte ErpCancelFlag { get { return erp_cancel_flag; } set { erp_cancel_flag = value; } }
        private byte erp_persistence_flag;
        public byte ErpPersistenceFlag { get { return erp_persistence_flag; } set { erp_persistence_flag = value; } }
        private byte erp_padding_flag;
        public byte ErpPaddingFlag { get { return erp_padding_flag; } set { erp_padding_flag = value; } }
        private uint erp_reserved_zero_2bits;
        public uint ErpReservedZero2bits { get { return erp_reserved_zero_2bits; } set { erp_reserved_zero_2bits = value; } }
        private uint gp_erp_type;
        public uint GpErpType { get { return gp_erp_type; } set { gp_erp_type = value; } }
        private uint left_gb_erp_width;
        public uint LeftGbErpWidth { get { return left_gb_erp_width; } set { left_gb_erp_width = value; } }
        private uint right_gb_erp_width;
        public uint RightGbErpWidth { get { return right_gb_erp_width; } set { right_gb_erp_width = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public EquirectangularProjection(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.erp_cancel_flag);

            if (erp_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.erp_persistence_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.erp_padding_flag);
                size += stream.ReadUnsignedInt(size, 2, out this.erp_reserved_zero_2bits);

                if (erp_padding_flag == 1)
                {
                    size += stream.ReadUnsignedInt(size, 3, out this.gp_erp_type);
                    size += stream.ReadUnsignedInt(size, 8, out this.left_gb_erp_width);
                    size += stream.ReadUnsignedInt(size, 8, out this.right_gb_erp_width);
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.erp_cancel_flag);

            if (erp_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.erp_persistence_flag);
                size += stream.WriteUnsignedInt(1, this.erp_padding_flag);
                size += stream.WriteUnsignedInt(2, this.erp_reserved_zero_2bits);

                if (erp_padding_flag == 1)
                {
                    size += stream.WriteUnsignedInt(3, this.gp_erp_type);
                    size += stream.WriteUnsignedInt(8, this.left_gb_erp_width);
                    size += stream.WriteUnsignedInt(8, this.right_gb_erp_width);
                }
            }

            return size;
        }

    }

    /*
   

cubemap_projection( payloadSize ) {  
 cmp_cancel_flag 5 u(1) 
 if( !cmp_cancel_flag )   
  cmp_persistence_flag 5 u(1) 
}
    */
    public class CubemapProjection : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte cmp_cancel_flag;
        public byte CmpCancelFlag { get { return cmp_cancel_flag; } set { cmp_cancel_flag = value; } }
        private byte cmp_persistence_flag;
        public byte CmpPersistenceFlag { get { return cmp_persistence_flag; } set { cmp_persistence_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public CubemapProjection(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.cmp_cancel_flag);

            if (cmp_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.cmp_persistence_flag);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.cmp_cancel_flag);

            if (cmp_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.cmp_persistence_flag);
            }

            return size;
        }

    }

    /*
   

sphere_rotation( payloadSize ) {  
 sphere_rotation_cancel_flag 5 u(1) 
 if( !sphere_rotation_cancel_flag ) {   
  sphere_rotation_persistence_flag 5 u(1) 
  sphere_rotation_reserved_zero_6bits 5 u(6) 
  yaw_rotation 5 i(32) 
  pitch_rotation 5 i(32) 
  roll_rotation 5 i(32) 
 }   
}
    */
    public class SphereRotation : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte sphere_rotation_cancel_flag;
        public byte SphereRotationCancelFlag { get { return sphere_rotation_cancel_flag; } set { sphere_rotation_cancel_flag = value; } }
        private byte sphere_rotation_persistence_flag;
        public byte SphereRotationPersistenceFlag { get { return sphere_rotation_persistence_flag; } set { sphere_rotation_persistence_flag = value; } }
        private uint sphere_rotation_reserved_zero_6bits;
        public uint SphereRotationReservedZero6bits { get { return sphere_rotation_reserved_zero_6bits; } set { sphere_rotation_reserved_zero_6bits = value; } }
        private int yaw_rotation;
        public int YawRotation { get { return yaw_rotation; } set { yaw_rotation = value; } }
        private int pitch_rotation;
        public int PitchRotation { get { return pitch_rotation; } set { pitch_rotation = value; } }
        private int roll_rotation;
        public int RollRotation { get { return roll_rotation; } set { roll_rotation = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SphereRotation(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.sphere_rotation_cancel_flag);

            if (sphere_rotation_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sphere_rotation_persistence_flag);
                size += stream.ReadUnsignedInt(size, 6, out this.sphere_rotation_reserved_zero_6bits);
                size += stream.ReadSignedInt(size, 32, out this.yaw_rotation);
                size += stream.ReadSignedInt(size, 32, out this.pitch_rotation);
                size += stream.ReadSignedInt(size, 32, out this.roll_rotation);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.sphere_rotation_cancel_flag);

            if (sphere_rotation_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.sphere_rotation_persistence_flag);
                size += stream.WriteUnsignedInt(6, this.sphere_rotation_reserved_zero_6bits);
                size += stream.WriteSignedInt(32, this.yaw_rotation);
                size += stream.WriteSignedInt(32, this.pitch_rotation);
                size += stream.WriteSignedInt(32, this.roll_rotation);
            }

            return size;
        }

    }

    /*
   

regionwise_packing( payloadSize ) {  
 rwp_cancel_flag 5 u(1) 
 if( !rwp_cancel_flag ) {   
  rwp_persistence_flag 5 u(1) 
  constituent_picture_matching_flag 5 u(1) 
  rwp_reserved_zero_5bits 5 u(5) 
  num_packed_regions 5 u(8) 
  proj_picture_width 5 u(32) 
  proj_picture_height 5 u(32) 
  packed_picture_width 5 u(16) 
  packed_picture_height 5 u(16) 
  for( i = 0; i < num_packed_regions; i++ ) {   
   rwp_reserved_zero_4bits[ i ] 5 u(4) 
   transform_type[ i ] 5 u(3) 
   guard_band_flag[ i ] 5 u(1) 
   proj_region_width[ i ] 5 u(32) 
   proj_region_height[ i ] 5 u(32) 
   proj_region_top[ i ] 5 u(32) 
   proj_region_left[ i ] 5 u(32) 
   packed_region_width[ i ] 5 u(16) 
   packed_region_height[ i ] 5 u(16) 
   packed_region_top[ i ] 5 u(16) 
   packed_region_left[ i ] 5 u(16) 
   if( guard_band_flag[ i ] ) {   
    left_gb_width[ i ] 5 u(8) 
    right_gb_width[ i ] 5 u(8) 
    top_gb_height[ i ] 5 u(8) 
    bottom_gb_height[ i ] 5 u(8) 
    gb_not_used_for_pred_flag[ i ] 5 u(1) 
    for( j = 0; j < 4; j++ )   
     gb_type[ i ][ j ] 5 u(3) 
    rwp_gb_reserved_zero_3bits[ i ] 5 u(3) 
   }   
  }   
 }   
}
    */
    public class RegionwisePacking : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte rwp_cancel_flag;
        public byte RwpCancelFlag { get { return rwp_cancel_flag; } set { rwp_cancel_flag = value; } }
        private byte rwp_persistence_flag;
        public byte RwpPersistenceFlag { get { return rwp_persistence_flag; } set { rwp_persistence_flag = value; } }
        private byte constituent_picture_matching_flag;
        public byte ConstituentPictureMatchingFlag { get { return constituent_picture_matching_flag; } set { constituent_picture_matching_flag = value; } }
        private uint rwp_reserved_zero_5bits;
        public uint RwpReservedZero5bits { get { return rwp_reserved_zero_5bits; } set { rwp_reserved_zero_5bits = value; } }
        private uint num_packed_regions;
        public uint NumPackedRegions { get { return num_packed_regions; } set { num_packed_regions = value; } }
        private uint proj_picture_width;
        public uint ProjPictureWidth { get { return proj_picture_width; } set { proj_picture_width = value; } }
        private uint proj_picture_height;
        public uint ProjPictureHeight { get { return proj_picture_height; } set { proj_picture_height = value; } }
        private uint packed_picture_width;
        public uint PackedPictureWidth { get { return packed_picture_width; } set { packed_picture_width = value; } }
        private uint packed_picture_height;
        public uint PackedPictureHeight { get { return packed_picture_height; } set { packed_picture_height = value; } }
        private uint[] rwp_reserved_zero_4bits;
        public uint[] RwpReservedZero4bits { get { return rwp_reserved_zero_4bits; } set { rwp_reserved_zero_4bits = value; } }
        private uint[] transform_type;
        public uint[] TransformType { get { return transform_type; } set { transform_type = value; } }
        private byte[] guard_band_flag;
        public byte[] GuardBandFlag { get { return guard_band_flag; } set { guard_band_flag = value; } }
        private uint[] proj_region_width;
        public uint[] ProjRegionWidth { get { return proj_region_width; } set { proj_region_width = value; } }
        private uint[] proj_region_height;
        public uint[] ProjRegionHeight { get { return proj_region_height; } set { proj_region_height = value; } }
        private uint[] proj_region_top;
        public uint[] ProjRegionTop { get { return proj_region_top; } set { proj_region_top = value; } }
        private uint[] proj_region_left;
        public uint[] ProjRegionLeft { get { return proj_region_left; } set { proj_region_left = value; } }
        private uint[] packed_region_width;
        public uint[] PackedRegionWidth { get { return packed_region_width; } set { packed_region_width = value; } }
        private uint[] packed_region_height;
        public uint[] PackedRegionHeight { get { return packed_region_height; } set { packed_region_height = value; } }
        private uint[] packed_region_top;
        public uint[] PackedRegionTop { get { return packed_region_top; } set { packed_region_top = value; } }
        private uint[] packed_region_left;
        public uint[] PackedRegionLeft { get { return packed_region_left; } set { packed_region_left = value; } }
        private uint[] left_gb_width;
        public uint[] LeftGbWidth { get { return left_gb_width; } set { left_gb_width = value; } }
        private uint[] right_gb_width;
        public uint[] RightGbWidth { get { return right_gb_width; } set { right_gb_width = value; } }
        private uint[] top_gb_height;
        public uint[] TopGbHeight { get { return top_gb_height; } set { top_gb_height = value; } }
        private uint[] bottom_gb_height;
        public uint[] BottomGbHeight { get { return bottom_gb_height; } set { bottom_gb_height = value; } }
        private byte[] gb_not_used_for_pred_flag;
        public byte[] GbNotUsedForPredFlag { get { return gb_not_used_for_pred_flag; } set { gb_not_used_for_pred_flag = value; } }
        private uint[][] gb_type;
        public uint[][] GbType { get { return gb_type; } set { gb_type = value; } }
        private uint[] rwp_gb_reserved_zero_3bits;
        public uint[] RwpGbReservedZero3bits { get { return rwp_gb_reserved_zero_3bits; } set { rwp_gb_reserved_zero_3bits = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public RegionwisePacking(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.rwp_cancel_flag);

            if (rwp_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.rwp_persistence_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.constituent_picture_matching_flag);
                size += stream.ReadUnsignedInt(size, 5, out this.rwp_reserved_zero_5bits);
                size += stream.ReadUnsignedInt(size, 8, out this.num_packed_regions);
                size += stream.ReadUnsignedInt(size, 32, out this.proj_picture_width);
                size += stream.ReadUnsignedInt(size, 32, out this.proj_picture_height);
                size += stream.ReadUnsignedInt(size, 16, out this.packed_picture_width);
                size += stream.ReadUnsignedInt(size, 16, out this.packed_picture_height);

                this.rwp_reserved_zero_4bits = new uint[num_packed_regions];
                this.transform_type = new uint[num_packed_regions];
                this.guard_band_flag = new byte[num_packed_regions];
                this.proj_region_width = new uint[num_packed_regions];
                this.proj_region_height = new uint[num_packed_regions];
                this.proj_region_top = new uint[num_packed_regions];
                this.proj_region_left = new uint[num_packed_regions];
                this.packed_region_width = new uint[num_packed_regions];
                this.packed_region_height = new uint[num_packed_regions];
                this.packed_region_top = new uint[num_packed_regions];
                this.packed_region_left = new uint[num_packed_regions];
                this.left_gb_width = new uint[num_packed_regions];
                this.right_gb_width = new uint[num_packed_regions];
                this.top_gb_height = new uint[num_packed_regions];
                this.bottom_gb_height = new uint[num_packed_regions];
                this.gb_not_used_for_pred_flag = new byte[num_packed_regions];
                this.gb_type = new uint[num_packed_regions][];
                this.rwp_gb_reserved_zero_3bits = new uint[num_packed_regions];
                for (i = 0; i < num_packed_regions; i++)
                {
                    size += stream.ReadUnsignedInt(size, 4, out this.rwp_reserved_zero_4bits[i]);
                    size += stream.ReadUnsignedInt(size, 3, out this.transform_type[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.guard_band_flag[i]);
                    size += stream.ReadUnsignedInt(size, 32, out this.proj_region_width[i]);
                    size += stream.ReadUnsignedInt(size, 32, out this.proj_region_height[i]);
                    size += stream.ReadUnsignedInt(size, 32, out this.proj_region_top[i]);
                    size += stream.ReadUnsignedInt(size, 32, out this.proj_region_left[i]);
                    size += stream.ReadUnsignedInt(size, 16, out this.packed_region_width[i]);
                    size += stream.ReadUnsignedInt(size, 16, out this.packed_region_height[i]);
                    size += stream.ReadUnsignedInt(size, 16, out this.packed_region_top[i]);
                    size += stream.ReadUnsignedInt(size, 16, out this.packed_region_left[i]);

                    if (guard_band_flag[i] != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 8, out this.left_gb_width[i]);
                        size += stream.ReadUnsignedInt(size, 8, out this.right_gb_width[i]);
                        size += stream.ReadUnsignedInt(size, 8, out this.top_gb_height[i]);
                        size += stream.ReadUnsignedInt(size, 8, out this.bottom_gb_height[i]);
                        size += stream.ReadUnsignedInt(size, 1, out this.gb_not_used_for_pred_flag[i]);

                        this.gb_type[i] = new uint[4];
                        for (j = 0; j < 4; j++)
                        {
                            size += stream.ReadUnsignedInt(size, 3, out this.gb_type[i][j]);
                        }
                        size += stream.ReadUnsignedInt(size, 3, out this.rwp_gb_reserved_zero_3bits[i]);
                    }
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.WriteUnsignedInt(1, this.rwp_cancel_flag);

            if (rwp_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.rwp_persistence_flag);
                size += stream.WriteUnsignedInt(1, this.constituent_picture_matching_flag);
                size += stream.WriteUnsignedInt(5, this.rwp_reserved_zero_5bits);
                size += stream.WriteUnsignedInt(8, this.num_packed_regions);
                size += stream.WriteUnsignedInt(32, this.proj_picture_width);
                size += stream.WriteUnsignedInt(32, this.proj_picture_height);
                size += stream.WriteUnsignedInt(16, this.packed_picture_width);
                size += stream.WriteUnsignedInt(16, this.packed_picture_height);

                for (i = 0; i < num_packed_regions; i++)
                {
                    size += stream.WriteUnsignedInt(4, this.rwp_reserved_zero_4bits[i]);
                    size += stream.WriteUnsignedInt(3, this.transform_type[i]);
                    size += stream.WriteUnsignedInt(1, this.guard_band_flag[i]);
                    size += stream.WriteUnsignedInt(32, this.proj_region_width[i]);
                    size += stream.WriteUnsignedInt(32, this.proj_region_height[i]);
                    size += stream.WriteUnsignedInt(32, this.proj_region_top[i]);
                    size += stream.WriteUnsignedInt(32, this.proj_region_left[i]);
                    size += stream.WriteUnsignedInt(16, this.packed_region_width[i]);
                    size += stream.WriteUnsignedInt(16, this.packed_region_height[i]);
                    size += stream.WriteUnsignedInt(16, this.packed_region_top[i]);
                    size += stream.WriteUnsignedInt(16, this.packed_region_left[i]);

                    if (guard_band_flag[i] != 0)
                    {
                        size += stream.WriteUnsignedInt(8, this.left_gb_width[i]);
                        size += stream.WriteUnsignedInt(8, this.right_gb_width[i]);
                        size += stream.WriteUnsignedInt(8, this.top_gb_height[i]);
                        size += stream.WriteUnsignedInt(8, this.bottom_gb_height[i]);
                        size += stream.WriteUnsignedInt(1, this.gb_not_used_for_pred_flag[i]);

                        for (j = 0; j < 4; j++)
                        {
                            size += stream.WriteUnsignedInt(3, this.gb_type[i][j]);
                        }
                        size += stream.WriteUnsignedInt(3, this.rwp_gb_reserved_zero_3bits[i]);
                    }
                }
            }

            return size;
        }

    }

    /*
   

omni_viewport( payloadSize ) {  
 omni_viewport_id 5 u(10) 
 omni_viewport_cancel_flag 5 u(1) 
 if( !omni_viewport_cancel_flag ) {   
  omni_viewport_persistence_flag 5 u(1) 
  omni_viewport_cnt_minus1 5 u(4) 
  for( i = 0; i  <=  omni_viewport_cnt_minus1; i++ ) {   
   omni_viewport_azimuth_centre[ i ] 5 i(32) 
   omni_viewport_elevation_centre[ i ] 5 i(32) 
   omni_viewport_tilt_centre[ i ] 5 i(32) 
   omni_viewport_hor_range[ i ] 5 u(32) 
   omni_viewport_ver_range[ i ] 5 u(32) 
  }   
 }   
}
    */
    public class OmniViewport : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint omni_viewport_id;
        public uint OmniViewportId { get { return omni_viewport_id; } set { omni_viewport_id = value; } }
        private byte omni_viewport_cancel_flag;
        public byte OmniViewportCancelFlag { get { return omni_viewport_cancel_flag; } set { omni_viewport_cancel_flag = value; } }
        private byte omni_viewport_persistence_flag;
        public byte OmniViewportPersistenceFlag { get { return omni_viewport_persistence_flag; } set { omni_viewport_persistence_flag = value; } }
        private uint omni_viewport_cnt_minus1;
        public uint OmniViewportCntMinus1 { get { return omni_viewport_cnt_minus1; } set { omni_viewport_cnt_minus1 = value; } }
        private int[] omni_viewport_azimuth_centre;
        public int[] OmniViewportAzimuthCentre { get { return omni_viewport_azimuth_centre; } set { omni_viewport_azimuth_centre = value; } }
        private int[] omni_viewport_elevation_centre;
        public int[] OmniViewportElevationCentre { get { return omni_viewport_elevation_centre; } set { omni_viewport_elevation_centre = value; } }
        private int[] omni_viewport_tilt_centre;
        public int[] OmniViewportTiltCentre { get { return omni_viewport_tilt_centre; } set { omni_viewport_tilt_centre = value; } }
        private uint[] omni_viewport_hor_range;
        public uint[] OmniViewportHorRange { get { return omni_viewport_hor_range; } set { omni_viewport_hor_range = value; } }
        private uint[] omni_viewport_ver_range;
        public uint[] OmniViewportVerRange { get { return omni_viewport_ver_range; } set { omni_viewport_ver_range = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public OmniViewport(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedInt(size, 10, out this.omni_viewport_id);
            size += stream.ReadUnsignedInt(size, 1, out this.omni_viewport_cancel_flag);

            if (omni_viewport_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.omni_viewport_persistence_flag);
                size += stream.ReadUnsignedInt(size, 4, out this.omni_viewport_cnt_minus1);

                this.omni_viewport_azimuth_centre = new int[omni_viewport_cnt_minus1 + 1];
                this.omni_viewport_elevation_centre = new int[omni_viewport_cnt_minus1 + 1];
                this.omni_viewport_tilt_centre = new int[omni_viewport_cnt_minus1 + 1];
                this.omni_viewport_hor_range = new uint[omni_viewport_cnt_minus1 + 1];
                this.omni_viewport_ver_range = new uint[omni_viewport_cnt_minus1 + 1];
                for (i = 0; i <= omni_viewport_cnt_minus1; i++)
                {
                    size += stream.ReadSignedInt(size, 32, out this.omni_viewport_azimuth_centre[i]);
                    size += stream.ReadSignedInt(size, 32, out this.omni_viewport_elevation_centre[i]);
                    size += stream.ReadSignedInt(size, 32, out this.omni_viewport_tilt_centre[i]);
                    size += stream.ReadUnsignedInt(size, 32, out this.omni_viewport_hor_range[i]);
                    size += stream.ReadUnsignedInt(size, 32, out this.omni_viewport_ver_range[i]);
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedInt(10, this.omni_viewport_id);
            size += stream.WriteUnsignedInt(1, this.omni_viewport_cancel_flag);

            if (omni_viewport_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.omni_viewport_persistence_flag);
                size += stream.WriteUnsignedInt(4, this.omni_viewport_cnt_minus1);

                for (i = 0; i <= omni_viewport_cnt_minus1; i++)
                {
                    size += stream.WriteSignedInt(32, this.omni_viewport_azimuth_centre[i]);
                    size += stream.WriteSignedInt(32, this.omni_viewport_elevation_centre[i]);
                    size += stream.WriteSignedInt(32, this.omni_viewport_tilt_centre[i]);
                    size += stream.WriteUnsignedInt(32, this.omni_viewport_hor_range[i]);
                    size += stream.WriteUnsignedInt(32, this.omni_viewport_ver_range[i]);
                }
            }

            return size;
        }

    }

    /*
   

sei_manifest( payloadSize ) {  
 manifest_num_sei_msg_types 5 u(16) 
 for( i = 0; i < manifest_num_sei_msg_types; i++ ) {   
  manifest_sei_payload_type[ i ] 5 u(16) 
  manifest_sei_description[ i ] 5 u(8) 
 }   
}
    */
    public class SeiManifest : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint manifest_num_sei_msg_types;
        public uint ManifestNumSeiMsgTypes { get { return manifest_num_sei_msg_types; } set { manifest_num_sei_msg_types = value; } }
        private uint[] manifest_sei_payload_type;
        public uint[] ManifestSeiPayloadType { get { return manifest_sei_payload_type; } set { manifest_sei_payload_type = value; } }
        private uint[] manifest_sei_description;
        public uint[] ManifestSeiDescription { get { return manifest_sei_description; } set { manifest_sei_description = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SeiManifest(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedInt(size, 16, out this.manifest_num_sei_msg_types);

            this.manifest_sei_payload_type = new uint[manifest_num_sei_msg_types];
            this.manifest_sei_description = new uint[manifest_num_sei_msg_types];
            for (i = 0; i < manifest_num_sei_msg_types; i++)
            {
                size += stream.ReadUnsignedInt(size, 16, out this.manifest_sei_payload_type[i]);
                size += stream.ReadUnsignedInt(size, 8, out this.manifest_sei_description[i]);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedInt(16, this.manifest_num_sei_msg_types);

            for (i = 0; i < manifest_num_sei_msg_types; i++)
            {
                size += stream.WriteUnsignedInt(16, this.manifest_sei_payload_type[i]);
                size += stream.WriteUnsignedInt(8, this.manifest_sei_description[i]);
            }

            return size;
        }

    }

    /*
   

sei_prefix_indication( payloadSize ) {  
 prefix_sei_payload_type 5 u(16) 
 num_sei_prefix_indications_minus1 5 u(8) 
 for( i = 0; i  <=  num_sei_prefix_indications_minus1; i++ ) {   
  num_bits_in_prefix_indication_minus1[ i ] 5 u(16) 
  for( j = 0; j  <=  num_bits_in_prefix_indication_minus1[ i ]; j++ )   
   sei_prefix_data_bit[ i ][ j ] 5 u(1) 
  while( !byte_aligned() )   
   byte_alignment_bit_equal_to_one /* equal to 1 *//* 5 f(1) 
 }   
}
    */
    public class SeiPrefixIndication : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint prefix_sei_payload_type;
        public uint PrefixSeiPayloadType { get { return prefix_sei_payload_type; } set { prefix_sei_payload_type = value; } }
        private uint num_sei_prefix_indications_minus1;
        public uint NumSeiPrefixIndicationsMinus1 { get { return num_sei_prefix_indications_minus1; } set { num_sei_prefix_indications_minus1 = value; } }
        private uint[] num_bits_in_prefix_indication_minus1;
        public uint[] NumBitsInPrefixIndicationMinus1 { get { return num_bits_in_prefix_indication_minus1; } set { num_bits_in_prefix_indication_minus1 = value; } }
        private byte[][] sei_prefix_data_bit;
        public byte[][] SeiPrefixDataBit { get { return sei_prefix_data_bit; } set { sei_prefix_data_bit = value; } }
        private uint[] byte_alignment_bit_equal_to_one;
        public uint[] ByteAlignmentBitEqualToOne { get { return byte_alignment_bit_equal_to_one; } set { byte_alignment_bit_equal_to_one = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SeiPrefixIndication(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.ReadUnsignedInt(size, 16, out this.prefix_sei_payload_type);
            size += stream.ReadUnsignedInt(size, 8, out this.num_sei_prefix_indications_minus1);

            this.num_bits_in_prefix_indication_minus1 = new uint[num_sei_prefix_indications_minus1 + 1];
            this.sei_prefix_data_bit = new byte[num_sei_prefix_indications_minus1 + 1][];
            this.byte_alignment_bit_equal_to_one = new uint[num_sei_prefix_indications_minus1 + 1];
            for (i = 0; i <= num_sei_prefix_indications_minus1; i++)
            {
                size += stream.ReadUnsignedInt(size, 16, out this.num_bits_in_prefix_indication_minus1[i]);

                this.sei_prefix_data_bit[i] = new byte[num_bits_in_prefix_indication_minus1[i] + 1];
                for (j = 0; j <= num_bits_in_prefix_indication_minus1[i]; j++)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.sei_prefix_data_bit[i][j]);
                }

                while (!stream.ByteAligned())
                {
                    size += stream.ReadFixed(size, 1, out this.byte_alignment_bit_equal_to_one[i]); // equal to 1 
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.WriteUnsignedInt(16, this.prefix_sei_payload_type);
            size += stream.WriteUnsignedInt(8, this.num_sei_prefix_indications_minus1);

            for (i = 0; i <= num_sei_prefix_indications_minus1; i++)
            {
                size += stream.WriteUnsignedInt(16, this.num_bits_in_prefix_indication_minus1[i]);

                for (j = 0; j <= num_bits_in_prefix_indication_minus1[i]; j++)
                {
                    size += stream.WriteUnsignedInt(1, this.sei_prefix_data_bit[i][j]);
                }

                while (!stream.ByteAligned())
                {
                    size += stream.WriteFixed(1, this.byte_alignment_bit_equal_to_one[i]); // equal to 1 
                }
            }

            return size;
        }

    }

    /*
   

annotated_regions( payloadSize ) {  
 ar_cancel_flag 5 u(1) 
 if( !ar_cancel_flag ) {   
  ar_not_optimized_for_viewing_flag 5 u(1) 
  ar_true_motion_flag 5 u(1) 
  ar_occluded_object_flag 5 u(1) 
  ar_partial_object_flag_present_flag 5 u(1) 
  ar_object_label_present_flag 5 u(1) 
  ar_object_confidence_info_present_flag 5 u(1) 
  if( ar_object_confidence_info_present_flag )   
   ar_object_confidence_length_minus1 5 u(4) 
  if( ar_object_label_present_flag ) {   
   ar_object_label_language_present_flag 5 u(1) 
   if( ar_object_label_language_present_flag ) {   
    while( !byte_aligned() )   
     ar_bit_equal_to_zero /* equal to 0 *//* 5 f(1) 
    ar_object_label_language 5 st(v) 
   }   
   ar_num_label_updates 5 ue(v) 
   for( i = 0; i < ar_num_label_updates; i++ ) {   
    ar_label_idx[ i ] 5 ue(v) 
    ar_label_cancel_flag 5 u(1) 
    LabelAssigned[ ar_label_idx[ i ] ] = !ar_label_cancel_flag   
    if( !ar_label_cancel_flag ) {   
     while( !byte_aligned() )   
      ar_bit_equal_to_zero /* equal to 0 *//* 5 f(1) 
     ar_label[ ar_label_idx[ i ] ] 5 st(v) 
    }   
   }   
  }   
  ar_num_object_updates 5 ue(v) 
  for( i = 0; i < ar_num_object_updates; i++ ) {   
   ar_object_idx[ i ] 5 ue(v) 
   ar_object_cancel_flag 5 u(1) 
   ObjectTracked[ ar_object_idx[ i ] ] = !ar_object_cancel_flag   
   if( !ar_object_cancel_flag ) {   
    if( ar_object_label_present_flag ) {   
     ar_object_label_update_flag 5 u(1) 
     if( ar_object_label_update_flag )   
      ar_object_label_idx[ ar_object_idx[ i ] ] 5 ue(v) 
    }   
    ar_bounding_box_update_flag 5 u(1) 
    if( ar_bounding_box_update_flag ) {   
     ar_bounding_box_cancel_flag 5 u(1) 
     ObjectBoundingBoxAvail[ ar_object_idx[ i ] ] = 
       !ar_bounding_box_cancel_flag 
    if( !ar_bounding_box_cancel_flag ) {   
      ar_bounding_box_top[ ar_object_idx[ i ] ] 5 u(16) 
      ar_bounding_box_left[ ar_object_idx[ i ] ] 5 u(16) 
      ar_bounding_box_width[ ar_object_idx[ i ] ] 5 u(16) 
      ar_bounding_box_height[ ar_object_idx[ i ] ] 5 u(16) 
      if( ar_partial_object_flag_present_flag )   
       ar_partial_object_flag[ ar_object_idx[ i ] ] 5 u(1) 
      if( ar_object_confidence_info_present_flag )   
       ar_object_confidence[ ar_object_idx[ i ] ] 5 u(v) 
     }   
    }   
   }   
  }   
 }   
}
    */
    public class AnnotatedRegions : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte ar_cancel_flag;
        public byte ArCancelFlag { get { return ar_cancel_flag; } set { ar_cancel_flag = value; } }
        private byte ar_not_optimized_for_viewing_flag;
        public byte ArNotOptimizedForViewingFlag { get { return ar_not_optimized_for_viewing_flag; } set { ar_not_optimized_for_viewing_flag = value; } }
        private byte ar_true_motion_flag;
        public byte ArTrueMotionFlag { get { return ar_true_motion_flag; } set { ar_true_motion_flag = value; } }
        private byte ar_occluded_object_flag;
        public byte ArOccludedObjectFlag { get { return ar_occluded_object_flag; } set { ar_occluded_object_flag = value; } }
        private byte ar_partial_object_flag_present_flag;
        public byte ArPartialObjectFlagPresentFlag { get { return ar_partial_object_flag_present_flag; } set { ar_partial_object_flag_present_flag = value; } }
        private byte ar_object_label_present_flag;
        public byte ArObjectLabelPresentFlag { get { return ar_object_label_present_flag; } set { ar_object_label_present_flag = value; } }
        private byte ar_object_confidence_info_present_flag;
        public byte ArObjectConfidenceInfoPresentFlag { get { return ar_object_confidence_info_present_flag; } set { ar_object_confidence_info_present_flag = value; } }
        private uint ar_object_confidence_length_minus1;
        public uint ArObjectConfidenceLengthMinus1 { get { return ar_object_confidence_length_minus1; } set { ar_object_confidence_length_minus1 = value; } }
        private byte ar_object_label_language_present_flag;
        public byte ArObjectLabelLanguagePresentFlag { get { return ar_object_label_language_present_flag; } set { ar_object_label_language_present_flag = value; } }
        private uint ar_bit_equal_to_zero;
        public uint ArBitEqualToZero { get { return ar_bit_equal_to_zero; } set { ar_bit_equal_to_zero = value; } }
        private int ar_object_label_language;
        public int ArObjectLabelLanguage { get { return ar_object_label_language; } set { ar_object_label_language = value; } }
        private uint ar_num_label_updates;
        public uint ArNumLabelUpdates { get { return ar_num_label_updates; } set { ar_num_label_updates = value; } }
        private uint[] ar_label_idx;
        public uint[] ArLabelIdx { get { return ar_label_idx; } set { ar_label_idx = value; } }
        private byte[] ar_label_cancel_flag;
        public byte[] ArLabelCancelFlag { get { return ar_label_cancel_flag; } set { ar_label_cancel_flag = value; } }
        private int[] ar_label;
        public int[] ArLabel { get { return ar_label; } set { ar_label = value; } }
        private uint ar_num_object_updates;
        public uint ArNumObjectUpdates { get { return ar_num_object_updates; } set { ar_num_object_updates = value; } }
        private uint[] ar_object_idx;
        public uint[] ArObjectIdx { get { return ar_object_idx; } set { ar_object_idx = value; } }
        private byte[] ar_object_cancel_flag;
        public byte[] ArObjectCancelFlag { get { return ar_object_cancel_flag; } set { ar_object_cancel_flag = value; } }
        private byte[] ar_object_label_update_flag;
        public byte[] ArObjectLabelUpdateFlag { get { return ar_object_label_update_flag; } set { ar_object_label_update_flag = value; } }
        private uint[] ar_object_label_idx;
        public uint[] ArObjectLabelIdx { get { return ar_object_label_idx; } set { ar_object_label_idx = value; } }
        private byte[] ar_bounding_box_update_flag;
        public byte[] ArBoundingBoxUpdateFlag { get { return ar_bounding_box_update_flag; } set { ar_bounding_box_update_flag = value; } }
        private byte[] ar_bounding_box_cancel_flag;
        public byte[] ArBoundingBoxCancelFlag { get { return ar_bounding_box_cancel_flag; } set { ar_bounding_box_cancel_flag = value; } }
        private uint[] ar_bounding_box_top;
        public uint[] ArBoundingBoxTop { get { return ar_bounding_box_top; } set { ar_bounding_box_top = value; } }
        private uint[] ar_bounding_box_left;
        public uint[] ArBoundingBoxLeft { get { return ar_bounding_box_left; } set { ar_bounding_box_left = value; } }
        private uint[] ar_bounding_box_width;
        public uint[] ArBoundingBoxWidth { get { return ar_bounding_box_width; } set { ar_bounding_box_width = value; } }
        private uint[] ar_bounding_box_height;
        public uint[] ArBoundingBoxHeight { get { return ar_bounding_box_height; } set { ar_bounding_box_height = value; } }
        private byte[] ar_partial_object_flag;
        public byte[] ArPartialObjectFlag { get { return ar_partial_object_flag; } set { ar_partial_object_flag = value; } }
        private uint[] ar_object_confidence;
        public uint[] ArObjectConfidence { get { return ar_object_confidence; } set { ar_object_confidence = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public AnnotatedRegions(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint[] LabelAssigned = null;
            uint[] ObjectTracked = null;
            bool[] ObjectBoundingBoxAvail = null;
            size += stream.ReadUnsignedInt(size, 1, out this.ar_cancel_flag);

            if (ar_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.ar_not_optimized_for_viewing_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.ar_true_motion_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.ar_occluded_object_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.ar_partial_object_flag_present_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.ar_object_label_present_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.ar_object_confidence_info_present_flag);

                if (ar_object_confidence_info_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 4, out this.ar_object_confidence_length_minus1);
                }

                if (ar_object_label_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.ar_object_label_language_present_flag);

                    if (ar_object_label_language_present_flag != 0)
                    {

                        while (!stream.ByteAligned())
                        {
                            size += stream.ReadFixed(size, 1, out this.ar_bit_equal_to_zero); // equal to 0 
                        }
                        size += stream.ReadSignedIntT(size, out this.ar_object_label_language);
                    }
                    size += stream.ReadUnsignedIntGolomb(size, out this.ar_num_label_updates);

                    this.ar_label_idx = new uint[ar_num_label_updates];
                    this.ar_label_cancel_flag = new byte[ar_num_label_updates];
                    this.ar_label = new int[ar_num_label_updates];
                    for (i = 0; i < ar_num_label_updates; i++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.ar_label_idx[i]);
                        size += stream.ReadUnsignedInt(size, 1, out this.ar_label_cancel_flag[i]);
                        LabelAssigned[ar_label_idx[i]] = ar_label_cancel_flag[i];

                        if (ar_label_cancel_flag[i] == 0)
                        {

                            while (!stream.ByteAligned())
                            {
                                size += stream.ReadFixed(size, 1, out this.ar_bit_equal_to_zero); // equal to 0 
                            }
                            size += stream.ReadSignedIntT(size, out this.ar_label[ar_label_idx[i]]);
                        }
                    }
                }
                size += stream.ReadUnsignedIntGolomb(size, out this.ar_num_object_updates);

                this.ar_object_idx = new uint[ar_num_object_updates];
                this.ar_object_cancel_flag = new byte[ar_num_object_updates];
                this.ar_object_label_update_flag = new byte[ar_num_object_updates];
                this.ar_object_label_idx = new uint[ar_num_object_updates];
                this.ar_bounding_box_update_flag = new byte[ar_num_object_updates];
                this.ar_bounding_box_cancel_flag = new byte[ar_num_object_updates];
                this.ar_bounding_box_top = new uint[ar_num_object_updates];
                this.ar_bounding_box_left = new uint[ar_num_object_updates];
                this.ar_bounding_box_width = new uint[ar_num_object_updates];
                this.ar_bounding_box_height = new uint[ar_num_object_updates];
                this.ar_partial_object_flag = new byte[ar_num_object_updates];
                this.ar_object_confidence = new uint[ar_num_object_updates];
                for (i = 0; i < ar_num_object_updates; i++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.ar_object_idx[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.ar_object_cancel_flag[i]);
                    ObjectTracked[ar_object_idx[i]] = ar_object_cancel_flag[i];

                    if (ar_object_cancel_flag[i] == 0)
                    {

                        if (ar_object_label_present_flag != 0)
                        {
                            size += stream.ReadUnsignedInt(size, 1, out this.ar_object_label_update_flag[i]);

                            if (ar_object_label_update_flag[i] != 0)
                            {
                                size += stream.ReadUnsignedIntGolomb(size, out this.ar_object_label_idx[ar_object_idx[i]]);
                            }
                        }
                        size += stream.ReadUnsignedInt(size, 1, out this.ar_bounding_box_update_flag[i]);

                        if (ar_bounding_box_update_flag[i] != 0)
                        {
                            size += stream.ReadUnsignedInt(size, 1, out this.ar_bounding_box_cancel_flag[i]);
                            ObjectBoundingBoxAvail[ar_object_idx[i]] = ar_bounding_box_cancel_flag[i] != 0;

                            if (ar_bounding_box_cancel_flag[i] == 0)
                            {
                                size += stream.ReadUnsignedInt(size, 16, out this.ar_bounding_box_top[ar_object_idx[i]]);
                                size += stream.ReadUnsignedInt(size, 16, out this.ar_bounding_box_left[ar_object_idx[i]]);
                                size += stream.ReadUnsignedInt(size, 16, out this.ar_bounding_box_width[ar_object_idx[i]]);
                                size += stream.ReadUnsignedInt(size, 16, out this.ar_bounding_box_height[ar_object_idx[i]]);

                                if (ar_partial_object_flag_present_flag != 0)
                                {
                                    size += stream.ReadUnsignedInt(size, 1, out this.ar_partial_object_flag[ar_object_idx[i]]);
                                }

                                if (ar_object_confidence_info_present_flag != 0)
                                {
                                    size += stream.ReadUnsignedIntVariable(size, (context.Sei.SeiMessage.SeiPayload.AnnotatedRegions.ArObjectConfidenceLengthMinus1 + 1), out this.ar_object_confidence[ar_object_idx[i]]);
                                }
                            }
                        }
                    }
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint[] LabelAssigned = null;
            uint[] ObjectTracked = null;
            bool[] ObjectBoundingBoxAvail = null;
            size += stream.WriteUnsignedInt(1, this.ar_cancel_flag);

            if (ar_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.ar_not_optimized_for_viewing_flag);
                size += stream.WriteUnsignedInt(1, this.ar_true_motion_flag);
                size += stream.WriteUnsignedInt(1, this.ar_occluded_object_flag);
                size += stream.WriteUnsignedInt(1, this.ar_partial_object_flag_present_flag);
                size += stream.WriteUnsignedInt(1, this.ar_object_label_present_flag);
                size += stream.WriteUnsignedInt(1, this.ar_object_confidence_info_present_flag);

                if (ar_object_confidence_info_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(4, this.ar_object_confidence_length_minus1);
                }

                if (ar_object_label_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.ar_object_label_language_present_flag);

                    if (ar_object_label_language_present_flag != 0)
                    {

                        while (!stream.ByteAligned())
                        {
                            size += stream.WriteFixed(1, this.ar_bit_equal_to_zero); // equal to 0 
                        }
                        size += stream.WriteSignedIntT(this.ar_object_label_language);
                    }
                    size += stream.WriteUnsignedIntGolomb(this.ar_num_label_updates);

                    for (i = 0; i < ar_num_label_updates; i++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.ar_label_idx[i]);
                        size += stream.WriteUnsignedInt(1, this.ar_label_cancel_flag[i]);
                        LabelAssigned[ar_label_idx[i]] = ar_label_cancel_flag[i];

                        if (ar_label_cancel_flag[i] == 0)
                        {

                            while (!stream.ByteAligned())
                            {
                                size += stream.WriteFixed(1, this.ar_bit_equal_to_zero); // equal to 0 
                            }
                            size += stream.WriteSignedIntT(this.ar_label[ar_label_idx[i]]);
                        }
                    }
                }
                size += stream.WriteUnsignedIntGolomb(this.ar_num_object_updates);

                for (i = 0; i < ar_num_object_updates; i++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.ar_object_idx[i]);
                    size += stream.WriteUnsignedInt(1, this.ar_object_cancel_flag[i]);
                    ObjectTracked[ar_object_idx[i]] = ar_object_cancel_flag[i];

                    if (ar_object_cancel_flag[i] == 0)
                    {

                        if (ar_object_label_present_flag != 0)
                        {
                            size += stream.WriteUnsignedInt(1, this.ar_object_label_update_flag[i]);

                            if (ar_object_label_update_flag[i] != 0)
                            {
                                size += stream.WriteUnsignedIntGolomb(this.ar_object_label_idx[ar_object_idx[i]]);
                            }
                        }
                        size += stream.WriteUnsignedInt(1, this.ar_bounding_box_update_flag[i]);

                        if (ar_bounding_box_update_flag[i] != 0)
                        {
                            size += stream.WriteUnsignedInt(1, this.ar_bounding_box_cancel_flag[i]);
                            ObjectBoundingBoxAvail[ar_object_idx[i]] = ar_bounding_box_cancel_flag[i] != 0;

                            if (ar_bounding_box_cancel_flag[i] == 0)
                            {
                                size += stream.WriteUnsignedInt(16, this.ar_bounding_box_top[ar_object_idx[i]]);
                                size += stream.WriteUnsignedInt(16, this.ar_bounding_box_left[ar_object_idx[i]]);
                                size += stream.WriteUnsignedInt(16, this.ar_bounding_box_width[ar_object_idx[i]]);
                                size += stream.WriteUnsignedInt(16, this.ar_bounding_box_height[ar_object_idx[i]]);

                                if (ar_partial_object_flag_present_flag != 0)
                                {
                                    size += stream.WriteUnsignedInt(1, this.ar_partial_object_flag[ar_object_idx[i]]);
                                }

                                if (ar_object_confidence_info_present_flag != 0)
                                {
                                    size += stream.WriteUnsignedIntVariable((context.Sei.SeiMessage.SeiPayload.AnnotatedRegions.ArObjectConfidenceLengthMinus1 + 1), this.ar_object_confidence[ar_object_idx[i]]);
                                }
                            }
                        }
                    }
                }
            }

            return size;
        }

    }

    /*
  


shutter_interval_info( payloadSize ) {  
 sii_sub_layer_idx 5 ue(v) 
 if( sii_sub_layer_idx  ==  0 ) {
  shutter_interval_info_present_flag 5 u(1) 
  if( shutter_interval_info_present_flag ) {
   sii_time_scale 5 u(32) 
   fixed_shutter_interval_within_cvs_flag 5 u(1) 
   if( fixed_shutter_interval_within_cvs_flag )   
    sii_num_units_in_shutter_interval 5 u(32) 
   else {   
    sii_max_sub_layers_minus1 5 u(3) 
    for( i = 0; i  <=  sii_max_sub_layers_minus1; i++ )   
     sub_layer_num_units_in_shutter_interval[ i ] 5 u(32) 
   }   
  }   
 }   
}
    */
    public class ShutterIntervalInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint sii_sub_layer_idx;
        public uint SiiSubLayerIdx { get { return sii_sub_layer_idx; } set { sii_sub_layer_idx = value; } }
        private byte shutter_interval_info_present_flag;
        public byte ShutterIntervalInfoPresentFlag { get { return shutter_interval_info_present_flag; } set { shutter_interval_info_present_flag = value; } }
        private uint sii_time_scale;
        public uint SiiTimeScale { get { return sii_time_scale; } set { sii_time_scale = value; } }
        private byte fixed_shutter_interval_within_cvs_flag;
        public byte FixedShutterIntervalWithinCvsFlag { get { return fixed_shutter_interval_within_cvs_flag; } set { fixed_shutter_interval_within_cvs_flag = value; } }
        private uint sii_num_units_in_shutter_interval;
        public uint SiiNumUnitsInShutterInterval { get { return sii_num_units_in_shutter_interval; } set { sii_num_units_in_shutter_interval = value; } }
        private uint sii_max_sub_layers_minus1;
        public uint SiiMaxSubLayersMinus1 { get { return sii_max_sub_layers_minus1; } set { sii_max_sub_layers_minus1 = value; } }
        private uint[] sub_layer_num_units_in_shutter_interval;
        public uint[] SubLayerNumUnitsInShutterInterval { get { return sub_layer_num_units_in_shutter_interval; } set { sub_layer_num_units_in_shutter_interval = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ShutterIntervalInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.sii_sub_layer_idx);

            if (sii_sub_layer_idx == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.shutter_interval_info_present_flag);

                if (shutter_interval_info_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 32, out this.sii_time_scale);
                    size += stream.ReadUnsignedInt(size, 1, out this.fixed_shutter_interval_within_cvs_flag);

                    if (fixed_shutter_interval_within_cvs_flag != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 32, out this.sii_num_units_in_shutter_interval);
                    }
                    else
                    {
                        size += stream.ReadUnsignedInt(size, 3, out this.sii_max_sub_layers_minus1);

                        this.sub_layer_num_units_in_shutter_interval = new uint[sii_max_sub_layers_minus1 + 1];
                        for (i = 0; i <= sii_max_sub_layers_minus1; i++)
                        {
                            size += stream.ReadUnsignedInt(size, 32, out this.sub_layer_num_units_in_shutter_interval[i]);
                        }
                    }
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedIntGolomb(this.sii_sub_layer_idx);

            if (sii_sub_layer_idx == 0)
            {
                size += stream.WriteUnsignedInt(1, this.shutter_interval_info_present_flag);

                if (shutter_interval_info_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(32, this.sii_time_scale);
                    size += stream.WriteUnsignedInt(1, this.fixed_shutter_interval_within_cvs_flag);

                    if (fixed_shutter_interval_within_cvs_flag != 0)
                    {
                        size += stream.WriteUnsignedInt(32, this.sii_num_units_in_shutter_interval);
                    }
                    else
                    {
                        size += stream.WriteUnsignedInt(3, this.sii_max_sub_layers_minus1);

                        for (i = 0; i <= sii_max_sub_layers_minus1; i++)
                        {
                            size += stream.WriteUnsignedInt(32, this.sub_layer_num_units_in_shutter_interval[i]);
                        }
                    }
                }
            }

            return size;
        }

    }

    /*
  

reserved_sei_message( payloadSize ) {  
 for( i = 0; i < payloadSize; i++ )   
  reserved_sei_message_payload_byte 5 b(8) 
}
    */
    public class ReservedSeiMessage : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte[] reserved_sei_message_payload_byte;
        public byte[] ReservedSeiMessagePayloadByte { get { return reserved_sei_message_payload_byte; } set { reserved_sei_message_payload_byte = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ReservedSeiMessage(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;

            this.reserved_sei_message_payload_byte = new byte[payloadSize];
            for (i = 0; i < payloadSize; i++)
            {
                size += stream.ReadBits(size, 8, out this.reserved_sei_message_payload_byte[i]);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;

            for (i = 0; i < payloadSize; i++)
            {
                size += stream.WriteBits(8, this.reserved_sei_message_payload_byte[i]);
            }

            return size;
        }

    }

    /*
   


vui_parameters() { 
 aspect_ratio_info_present_flag 0 u(1) 
 if( aspect_ratio_info_present_flag ) {   
  aspect_ratio_idc 0 u(8) 
  if( aspect_ratio_idc  ==  Extended_SAR ) {   
   sar_width 0 u(16) 
   sar_height 0 u(16) 
  }   
 }   
 overscan_info_present_flag 0 u(1) 
 if( overscan_info_present_flag )   
  overscan_appropriate_flag 0 u(1) 
 video_signal_type_present_flag 0 u(1) 
 if( video_signal_type_present_flag ) {   
  video_format 0 u(3) 
  video_full_range_flag 0 u(1) 
  colour_description_present_flag 0 u(1) 
  if( colour_description_present_flag ) {   
   colour_primaries 0 u(8) 
   transfer_characteristics 0 u(8) 
   matrix_coefficients 0 u(8) 
  }   
 }   
 chroma_loc_info_present_flag 0 u(1) 
 if( chroma_loc_info_present_flag ) {   
  chroma_sample_loc_type_top_field 0 ue(v) 
  chroma_sample_loc_type_bottom_field 0 ue(v) 
 }
  timing_info_present_flag 0 u(1) 
 if( timing_info_present_flag ) {   
  num_units_in_tick 0 u(32) 
  time_scale 0 u(32) 
  fixed_frame_rate_flag 0 u(1) 
 }   
 nal_hrd_parameters_present_flag 0 u(1) 
 if( nal_hrd_parameters_present_flag )   
  hrd_parameters() 0  
 vcl_hrd_parameters_present_flag 0 u(1) 
 if( vcl_hrd_parameters_present_flag )   
  hrd_parameters() 0  
 if( nal_hrd_parameters_present_flag  ||  vcl_hrd_parameters_present_flag )   
  low_delay_hrd_flag 0 u(1) 
 pic_struct_present_flag  0 u(1) 
 bitstream_restriction_flag 0 u(1) 
 if( bitstream_restriction_flag ) {   
  motion_vectors_over_pic_boundaries_flag 0 u(1) 
  max_bytes_per_pic_denom 0 ue(v) 
  max_bits_per_mb_denom 0 ue(v) 
  log2_max_mv_length_horizontal 0 ue(v) 
  log2_max_mv_length_vertical 0 ue(v) 
  max_num_reorder_frames 0 ue(v) 
  max_dec_frame_buffering 0 ue(v) 
 }   
}
    */
    public class VuiParameters : IItuSerializable
    {
        private byte aspect_ratio_info_present_flag;
        public byte AspectRatioInfoPresentFlag { get { return aspect_ratio_info_present_flag; } set { aspect_ratio_info_present_flag = value; } }
        private uint aspect_ratio_idc;
        public uint AspectRatioIdc { get { return aspect_ratio_idc; } set { aspect_ratio_idc = value; } }
        private uint sar_width;
        public uint SarWidth { get { return sar_width; } set { sar_width = value; } }
        private uint sar_height;
        public uint SarHeight { get { return sar_height; } set { sar_height = value; } }
        private byte overscan_info_present_flag;
        public byte OverscanInfoPresentFlag { get { return overscan_info_present_flag; } set { overscan_info_present_flag = value; } }
        private byte overscan_appropriate_flag;
        public byte OverscanAppropriateFlag { get { return overscan_appropriate_flag; } set { overscan_appropriate_flag = value; } }
        private byte video_signal_type_present_flag;
        public byte VideoSignalTypePresentFlag { get { return video_signal_type_present_flag; } set { video_signal_type_present_flag = value; } }
        private uint video_format;
        public uint VideoFormat { get { return video_format; } set { video_format = value; } }
        private byte video_full_range_flag;
        public byte VideoFullRangeFlag { get { return video_full_range_flag; } set { video_full_range_flag = value; } }
        private byte colour_description_present_flag;
        public byte ColourDescriptionPresentFlag { get { return colour_description_present_flag; } set { colour_description_present_flag = value; } }
        private uint colour_primaries;
        public uint ColourPrimaries { get { return colour_primaries; } set { colour_primaries = value; } }
        private uint transfer_characteristics;
        public uint TransferCharacteristics { get { return transfer_characteristics; } set { transfer_characteristics = value; } }
        private uint matrix_coefficients;
        public uint MatrixCoefficients { get { return matrix_coefficients; } set { matrix_coefficients = value; } }
        private byte chroma_loc_info_present_flag;
        public byte ChromaLocInfoPresentFlag { get { return chroma_loc_info_present_flag; } set { chroma_loc_info_present_flag = value; } }
        private uint chroma_sample_loc_type_top_field;
        public uint ChromaSampleLocTypeTopField { get { return chroma_sample_loc_type_top_field; } set { chroma_sample_loc_type_top_field = value; } }
        private uint chroma_sample_loc_type_bottom_field;
        public uint ChromaSampleLocTypeBottomField { get { return chroma_sample_loc_type_bottom_field; } set { chroma_sample_loc_type_bottom_field = value; } }
        private byte timing_info_present_flag;
        public byte TimingInfoPresentFlag { get { return timing_info_present_flag; } set { timing_info_present_flag = value; } }
        private uint num_units_in_tick;
        public uint NumUnitsInTick { get { return num_units_in_tick; } set { num_units_in_tick = value; } }
        private uint time_scale;
        public uint TimeScale { get { return time_scale; } set { time_scale = value; } }
        private byte fixed_frame_rate_flag;
        public byte FixedFrameRateFlag { get { return fixed_frame_rate_flag; } set { fixed_frame_rate_flag = value; } }
        private byte nal_hrd_parameters_present_flag;
        public byte NalHrdParametersPresentFlag { get { return nal_hrd_parameters_present_flag; } set { nal_hrd_parameters_present_flag = value; } }
        private HrdParameters hrd_parameters;
        public HrdParameters HrdParameters { get { return hrd_parameters; } set { hrd_parameters = value; } }
        private byte vcl_hrd_parameters_present_flag;
        public byte VclHrdParametersPresentFlag { get { return vcl_hrd_parameters_present_flag; } set { vcl_hrd_parameters_present_flag = value; } }
        private byte low_delay_hrd_flag;
        public byte LowDelayHrdFlag { get { return low_delay_hrd_flag; } set { low_delay_hrd_flag = value; } }
        private byte pic_struct_present_flag;
        public byte PicStructPresentFlag { get { return pic_struct_present_flag; } set { pic_struct_present_flag = value; } }
        private byte bitstream_restriction_flag;
        public byte BitstreamRestrictionFlag { get { return bitstream_restriction_flag; } set { bitstream_restriction_flag = value; } }
        private byte motion_vectors_over_pic_boundaries_flag;
        public byte MotionVectorsOverPicBoundariesFlag { get { return motion_vectors_over_pic_boundaries_flag; } set { motion_vectors_over_pic_boundaries_flag = value; } }
        private uint max_bytes_per_pic_denom;
        public uint MaxBytesPerPicDenom { get { return max_bytes_per_pic_denom; } set { max_bytes_per_pic_denom = value; } }
        private uint max_bits_per_mb_denom;
        public uint MaxBitsPerMbDenom { get { return max_bits_per_mb_denom; } set { max_bits_per_mb_denom = value; } }
        private uint log2_max_mv_length_horizontal;
        public uint Log2MaxMvLengthHorizontal { get { return log2_max_mv_length_horizontal; } set { log2_max_mv_length_horizontal = value; } }
        private uint log2_max_mv_length_vertical;
        public uint Log2MaxMvLengthVertical { get { return log2_max_mv_length_vertical; } set { log2_max_mv_length_vertical = value; } }
        private uint max_num_reorder_frames;
        public uint MaxNumReorderFrames { get { return max_num_reorder_frames; } set { max_num_reorder_frames = value; } }
        private uint max_dec_frame_buffering;
        public uint MaxDecFrameBuffering { get { return max_dec_frame_buffering; } set { max_dec_frame_buffering = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public VuiParameters()
        {

        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.aspect_ratio_info_present_flag);

            if (aspect_ratio_info_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 8, out this.aspect_ratio_idc);

                if (aspect_ratio_idc == H264Constants.Extended_SAR)
                {
                    size += stream.ReadUnsignedInt(size, 16, out this.sar_width);
                    size += stream.ReadUnsignedInt(size, 16, out this.sar_height);
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.overscan_info_present_flag);

            if (overscan_info_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.overscan_appropriate_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.video_signal_type_present_flag);

            if (video_signal_type_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 3, out this.video_format);
                size += stream.ReadUnsignedInt(size, 1, out this.video_full_range_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.colour_description_present_flag);

                if (colour_description_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 8, out this.colour_primaries);
                    size += stream.ReadUnsignedInt(size, 8, out this.transfer_characteristics);
                    size += stream.ReadUnsignedInt(size, 8, out this.matrix_coefficients);
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.chroma_loc_info_present_flag);

            if (chroma_loc_info_present_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.chroma_sample_loc_type_top_field);
                size += stream.ReadUnsignedIntGolomb(size, out this.chroma_sample_loc_type_bottom_field);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.timing_info_present_flag);

            if (timing_info_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 32, out this.num_units_in_tick);
                size += stream.ReadUnsignedInt(size, 32, out this.time_scale);
                size += stream.ReadUnsignedInt(size, 1, out this.fixed_frame_rate_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.nal_hrd_parameters_present_flag);

            if (nal_hrd_parameters_present_flag != 0)
            {
                size += stream.ReadClass<HrdParameters>(size, context, () => new HrdParameters(), out this.hrd_parameters);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.vcl_hrd_parameters_present_flag);

            if (vcl_hrd_parameters_present_flag != 0)
            {
                size += stream.ReadClass<HrdParameters>(size, context, () => new HrdParameters(), out this.hrd_parameters);
            }

            if (nal_hrd_parameters_present_flag != 0 || vcl_hrd_parameters_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.low_delay_hrd_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.pic_struct_present_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.bitstream_restriction_flag);

            if (bitstream_restriction_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.motion_vectors_over_pic_boundaries_flag);
                size += stream.ReadUnsignedIntGolomb(size, out this.max_bytes_per_pic_denom);
                size += stream.ReadUnsignedIntGolomb(size, out this.max_bits_per_mb_denom);
                size += stream.ReadUnsignedIntGolomb(size, out this.log2_max_mv_length_horizontal);
                size += stream.ReadUnsignedIntGolomb(size, out this.log2_max_mv_length_vertical);
                size += stream.ReadUnsignedIntGolomb(size, out this.max_num_reorder_frames);
                size += stream.ReadUnsignedIntGolomb(size, out this.max_dec_frame_buffering);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.aspect_ratio_info_present_flag);

            if (aspect_ratio_info_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(8, this.aspect_ratio_idc);

                if (aspect_ratio_idc == H264Constants.Extended_SAR)
                {
                    size += stream.WriteUnsignedInt(16, this.sar_width);
                    size += stream.WriteUnsignedInt(16, this.sar_height);
                }
            }
            size += stream.WriteUnsignedInt(1, this.overscan_info_present_flag);

            if (overscan_info_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.overscan_appropriate_flag);
            }
            size += stream.WriteUnsignedInt(1, this.video_signal_type_present_flag);

            if (video_signal_type_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(3, this.video_format);
                size += stream.WriteUnsignedInt(1, this.video_full_range_flag);
                size += stream.WriteUnsignedInt(1, this.colour_description_present_flag);

                if (colour_description_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(8, this.colour_primaries);
                    size += stream.WriteUnsignedInt(8, this.transfer_characteristics);
                    size += stream.WriteUnsignedInt(8, this.matrix_coefficients);
                }
            }
            size += stream.WriteUnsignedInt(1, this.chroma_loc_info_present_flag);

            if (chroma_loc_info_present_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.chroma_sample_loc_type_top_field);
                size += stream.WriteUnsignedIntGolomb(this.chroma_sample_loc_type_bottom_field);
            }
            size += stream.WriteUnsignedInt(1, this.timing_info_present_flag);

            if (timing_info_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(32, this.num_units_in_tick);
                size += stream.WriteUnsignedInt(32, this.time_scale);
                size += stream.WriteUnsignedInt(1, this.fixed_frame_rate_flag);
            }
            size += stream.WriteUnsignedInt(1, this.nal_hrd_parameters_present_flag);

            if (nal_hrd_parameters_present_flag != 0)
            {
                size += stream.WriteClass<HrdParameters>(context, this.hrd_parameters);
            }
            size += stream.WriteUnsignedInt(1, this.vcl_hrd_parameters_present_flag);

            if (vcl_hrd_parameters_present_flag != 0)
            {
                size += stream.WriteClass<HrdParameters>(context, this.hrd_parameters);
            }

            if (nal_hrd_parameters_present_flag != 0 || vcl_hrd_parameters_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.low_delay_hrd_flag);
            }
            size += stream.WriteUnsignedInt(1, this.pic_struct_present_flag);
            size += stream.WriteUnsignedInt(1, this.bitstream_restriction_flag);

            if (bitstream_restriction_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.motion_vectors_over_pic_boundaries_flag);
                size += stream.WriteUnsignedIntGolomb(this.max_bytes_per_pic_denom);
                size += stream.WriteUnsignedIntGolomb(this.max_bits_per_mb_denom);
                size += stream.WriteUnsignedIntGolomb(this.log2_max_mv_length_horizontal);
                size += stream.WriteUnsignedIntGolomb(this.log2_max_mv_length_vertical);
                size += stream.WriteUnsignedIntGolomb(this.max_num_reorder_frames);
                size += stream.WriteUnsignedIntGolomb(this.max_dec_frame_buffering);
            }

            return size;
        }

    }

    /*
   


hrd_parameters() { 
 cpb_cnt_minus1 0 | 5 ue(v) 
 bit_rate_scale 0 | 5 u(4) 
 cpb_size_scale 0 | 5 u(4)
    for (SchedSelIdx = 0; SchedSelIdx <= cpb_cnt_minus1; SchedSelIdx++) {
        bit_rate_value_minus1[SchedSelIdx] 0 | 5 ue(v)
        cpb_size_value_minus1[SchedSelIdx] 0 | 5 ue(v)
        cbr_flag[SchedSelIdx] 0 | 5 u(1)
    }   
 initial_cpb_removal_delay_length_minus1 0 | 5 u(5) 
 cpb_removal_delay_length_minus1 0 | 5 u(5) 
 dpb_output_delay_length_minus1 0 | 5 u(5) 
 time_offset_length 0 | 5 u(5)
}
    */
    public class HrdParameters : IItuSerializable
    {
        private uint cpb_cnt_minus1;
        public uint CpbCntMinus1 { get { return cpb_cnt_minus1; } set { cpb_cnt_minus1 = value; } }
        private uint bit_rate_scale;
        public uint BitRateScale { get { return bit_rate_scale; } set { bit_rate_scale = value; } }
        private uint cpb_size_scale;
        public uint CpbSizeScale { get { return cpb_size_scale; } set { cpb_size_scale = value; } }
        private uint[] bit_rate_value_minus1;
        public uint[] BitRateValueMinus1 { get { return bit_rate_value_minus1; } set { bit_rate_value_minus1 = value; } }
        private uint[] cpb_size_value_minus1;
        public uint[] CpbSizeValueMinus1 { get { return cpb_size_value_minus1; } set { cpb_size_value_minus1 = value; } }
        private byte[] cbr_flag;
        public byte[] CbrFlag { get { return cbr_flag; } set { cbr_flag = value; } }
        private uint initial_cpb_removal_delay_length_minus1;
        public uint InitialCpbRemovalDelayLengthMinus1 { get { return initial_cpb_removal_delay_length_minus1; } set { initial_cpb_removal_delay_length_minus1 = value; } }
        private uint cpb_removal_delay_length_minus1;
        public uint CpbRemovalDelayLengthMinus1 { get { return cpb_removal_delay_length_minus1; } set { cpb_removal_delay_length_minus1 = value; } }
        private uint dpb_output_delay_length_minus1;
        public uint DpbOutputDelayLengthMinus1 { get { return dpb_output_delay_length_minus1; } set { dpb_output_delay_length_minus1 = value; } }
        private uint time_offset_length;
        public uint TimeOffsetLength { get { return time_offset_length; } set { time_offset_length = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public HrdParameters()
        {

        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint SchedSelIdx = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.cpb_cnt_minus1);
            size += stream.ReadUnsignedInt(size, 4, out this.bit_rate_scale);
            size += stream.ReadUnsignedInt(size, 4, out this.cpb_size_scale);

            this.bit_rate_value_minus1 = new uint[cpb_cnt_minus1 + 1];
            this.cpb_size_value_minus1 = new uint[cpb_cnt_minus1 + 1];
            this.cbr_flag = new byte[cpb_cnt_minus1 + 1];
            for (SchedSelIdx = 0; SchedSelIdx <= cpb_cnt_minus1; SchedSelIdx++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.bit_rate_value_minus1[SchedSelIdx]);
                size += stream.ReadUnsignedIntGolomb(size, out this.cpb_size_value_minus1[SchedSelIdx]);
                size += stream.ReadUnsignedInt(size, 1, out this.cbr_flag[SchedSelIdx]);
            }
            size += stream.ReadUnsignedInt(size, 5, out this.initial_cpb_removal_delay_length_minus1);
            size += stream.ReadUnsignedInt(size, 5, out this.cpb_removal_delay_length_minus1);
            size += stream.ReadUnsignedInt(size, 5, out this.dpb_output_delay_length_minus1);
            size += stream.ReadUnsignedInt(size, 5, out this.time_offset_length);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint SchedSelIdx = 0;
            size += stream.WriteUnsignedIntGolomb(this.cpb_cnt_minus1);
            size += stream.WriteUnsignedInt(4, this.bit_rate_scale);
            size += stream.WriteUnsignedInt(4, this.cpb_size_scale);

            for (SchedSelIdx = 0; SchedSelIdx <= cpb_cnt_minus1; SchedSelIdx++)
            {
                size += stream.WriteUnsignedIntGolomb(this.bit_rate_value_minus1[SchedSelIdx]);
                size += stream.WriteUnsignedIntGolomb(this.cpb_size_value_minus1[SchedSelIdx]);
                size += stream.WriteUnsignedInt(1, this.cbr_flag[SchedSelIdx]);
            }
            size += stream.WriteUnsignedInt(5, this.initial_cpb_removal_delay_length_minus1);
            size += stream.WriteUnsignedInt(5, this.cpb_removal_delay_length_minus1);
            size += stream.WriteUnsignedInt(5, this.dpb_output_delay_length_minus1);
            size += stream.WriteUnsignedInt(5, this.time_offset_length);

            return size;
        }

    }

    /*
 



nal_unit_header_svc_extension() { 
idr_flag  All u(1) 
priority_id All u(6) 
no_inter_layer_pred_flag All u(1) 
dependency_id All u(3) 
quality_id All u(4) 
temporal_id All u(3) 
use_ref_base_pic_flag All u(1) 
discardable_flag All u(1) 
output_flag All u(1) 
reserved_three_2bits All u(2) 
}
    */
    public class NalUnitHeaderSvcExtension : IItuSerializable
    {
        private byte idr_flag;
        public byte IdrFlag { get { return idr_flag; } set { idr_flag = value; } }
        private uint priority_id;
        public uint PriorityId { get { return priority_id; } set { priority_id = value; } }
        private byte no_inter_layer_pred_flag;
        public byte NoInterLayerPredFlag { get { return no_inter_layer_pred_flag; } set { no_inter_layer_pred_flag = value; } }
        private uint dependency_id;
        public uint DependencyId { get { return dependency_id; } set { dependency_id = value; } }
        private uint quality_id;
        public uint QualityId { get { return quality_id; } set { quality_id = value; } }
        private uint temporal_id;
        public uint TemporalId { get { return temporal_id; } set { temporal_id = value; } }
        private byte use_ref_base_pic_flag;
        public byte UseRefBasePicFlag { get { return use_ref_base_pic_flag; } set { use_ref_base_pic_flag = value; } }
        private byte discardable_flag;
        public byte DiscardableFlag { get { return discardable_flag; } set { discardable_flag = value; } }
        private byte output_flag;
        public byte OutputFlag { get { return output_flag; } set { output_flag = value; } }
        private uint reserved_three_2bits;
        public uint ReservedThree2bits { get { return reserved_three_2bits; } set { reserved_three_2bits = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public NalUnitHeaderSvcExtension()
        {

        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.idr_flag);
            size += stream.ReadUnsignedInt(size, 6, out this.priority_id);
            size += stream.ReadUnsignedInt(size, 1, out this.no_inter_layer_pred_flag);
            size += stream.ReadUnsignedInt(size, 3, out this.dependency_id);
            size += stream.ReadUnsignedInt(size, 4, out this.quality_id);
            size += stream.ReadUnsignedInt(size, 3, out this.temporal_id);
            size += stream.ReadUnsignedInt(size, 1, out this.use_ref_base_pic_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.discardable_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.output_flag);
            size += stream.ReadUnsignedInt(size, 2, out this.reserved_three_2bits);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.idr_flag);
            size += stream.WriteUnsignedInt(6, this.priority_id);
            size += stream.WriteUnsignedInt(1, this.no_inter_layer_pred_flag);
            size += stream.WriteUnsignedInt(3, this.dependency_id);
            size += stream.WriteUnsignedInt(4, this.quality_id);
            size += stream.WriteUnsignedInt(3, this.temporal_id);
            size += stream.WriteUnsignedInt(1, this.use_ref_base_pic_flag);
            size += stream.WriteUnsignedInt(1, this.discardable_flag);
            size += stream.WriteUnsignedInt(1, this.output_flag);
            size += stream.WriteUnsignedInt(2, this.reserved_three_2bits);

            return size;
        }

    }

    /*
   

seq_parameter_set_svc_extension() {  
 inter_layer_deblocking_filter_control_present_flag 0 u(1) 
 extended_spatial_scalability_idc 0 u(2) 
 if( ChromaArrayType  ==  1  ||  ChromaArrayType  ==  2 )   
  chroma_phase_x_plus1_flag 0 u(1) 
 if( ChromaArrayType  ==  1 )   
  chroma_phase_y_plus1 0 u(2) 
 if( extended_spatial_scalability_idc  ==  1 ) {   
  if( ChromaArrayType > 0 ) {   
   seq_ref_layer_chroma_phase_x_plus1_flag 0 u(1) 
   seq_ref_layer_chroma_phase_y_plus1 0 u(2) 
  }   
  seq_scaled_ref_layer_left_offset 0 se(v) 
  seq_scaled_ref_layer_top_offset 0 se(v) 
  seq_scaled_ref_layer_right_offset 0 se(v) 
  seq_scaled_ref_layer_bottom_offset 0 se(v) 
 }   
 seq_tcoeff_level_prediction_flag 0 u(1) 
 if( seq_tcoeff_level_prediction_flag ) {   
  adaptive_tcoeff_level_prediction_flag 0 u(1) 
 }   
 slice_header_restriction_flag 0 u(1) 
}
    */
    public class SeqParameterSetSvcExtension : IItuSerializable
    {
        private byte inter_layer_deblocking_filter_control_present_flag;
        public byte InterLayerDeblockingFilterControlPresentFlag { get { return inter_layer_deblocking_filter_control_present_flag; } set { inter_layer_deblocking_filter_control_present_flag = value; } }
        private uint extended_spatial_scalability_idc;
        public uint ExtendedSpatialScalabilityIdc { get { return extended_spatial_scalability_idc; } set { extended_spatial_scalability_idc = value; } }
        private byte chroma_phase_x_plus1_flag;
        public byte ChromaPhasexPlus1Flag { get { return chroma_phase_x_plus1_flag; } set { chroma_phase_x_plus1_flag = value; } }
        private uint chroma_phase_y_plus1;
        public uint ChromaPhaseyPlus1 { get { return chroma_phase_y_plus1; } set { chroma_phase_y_plus1 = value; } }
        private byte seq_ref_layer_chroma_phase_x_plus1_flag;
        public byte SeqRefLayerChromaPhasexPlus1Flag { get { return seq_ref_layer_chroma_phase_x_plus1_flag; } set { seq_ref_layer_chroma_phase_x_plus1_flag = value; } }
        private uint seq_ref_layer_chroma_phase_y_plus1;
        public uint SeqRefLayerChromaPhaseyPlus1 { get { return seq_ref_layer_chroma_phase_y_plus1; } set { seq_ref_layer_chroma_phase_y_plus1 = value; } }
        private int seq_scaled_ref_layer_left_offset;
        public int SeqScaledRefLayerLeftOffset { get { return seq_scaled_ref_layer_left_offset; } set { seq_scaled_ref_layer_left_offset = value; } }
        private int seq_scaled_ref_layer_top_offset;
        public int SeqScaledRefLayerTopOffset { get { return seq_scaled_ref_layer_top_offset; } set { seq_scaled_ref_layer_top_offset = value; } }
        private int seq_scaled_ref_layer_right_offset;
        public int SeqScaledRefLayerRightOffset { get { return seq_scaled_ref_layer_right_offset; } set { seq_scaled_ref_layer_right_offset = value; } }
        private int seq_scaled_ref_layer_bottom_offset;
        public int SeqScaledRefLayerBottomOffset { get { return seq_scaled_ref_layer_bottom_offset; } set { seq_scaled_ref_layer_bottom_offset = value; } }
        private byte seq_tcoeff_level_prediction_flag;
        public byte SeqTcoeffLevelPredictionFlag { get { return seq_tcoeff_level_prediction_flag; } set { seq_tcoeff_level_prediction_flag = value; } }
        private byte adaptive_tcoeff_level_prediction_flag;
        public byte AdaptiveTcoeffLevelPredictionFlag { get { return adaptive_tcoeff_level_prediction_flag; } set { adaptive_tcoeff_level_prediction_flag = value; } }
        private byte slice_header_restriction_flag;
        public byte SliceHeaderRestrictionFlag { get { return slice_header_restriction_flag; } set { slice_header_restriction_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SeqParameterSetSvcExtension()
        {

        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.inter_layer_deblocking_filter_control_present_flag);
            size += stream.ReadUnsignedInt(size, 2, out this.extended_spatial_scalability_idc);

            if ((context.Sps.SeqParameterSetData.SeparateColourPlaneFlag == 0 ? context.Sps.SeqParameterSetData.ChromaFormatIdc : 0) == 1 || (context.Sps.SeqParameterSetData.SeparateColourPlaneFlag == 0 ? context.Sps.SeqParameterSetData.ChromaFormatIdc : 0) == 2)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.chroma_phase_x_plus1_flag);
            }

            if ((context.Sps.SeqParameterSetData.SeparateColourPlaneFlag == 0 ? context.Sps.SeqParameterSetData.ChromaFormatIdc : 0) == 1)
            {
                size += stream.ReadUnsignedInt(size, 2, out this.chroma_phase_y_plus1);
            }

            if (extended_spatial_scalability_idc == 1)
            {

                if ((context.Sps.SeqParameterSetData.SeparateColourPlaneFlag == 0 ? context.Sps.SeqParameterSetData.ChromaFormatIdc : 0) > 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.seq_ref_layer_chroma_phase_x_plus1_flag);
                    size += stream.ReadUnsignedInt(size, 2, out this.seq_ref_layer_chroma_phase_y_plus1);
                }
                size += stream.ReadSignedIntGolomb(size, out this.seq_scaled_ref_layer_left_offset);
                size += stream.ReadSignedIntGolomb(size, out this.seq_scaled_ref_layer_top_offset);
                size += stream.ReadSignedIntGolomb(size, out this.seq_scaled_ref_layer_right_offset);
                size += stream.ReadSignedIntGolomb(size, out this.seq_scaled_ref_layer_bottom_offset);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.seq_tcoeff_level_prediction_flag);

            if (seq_tcoeff_level_prediction_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.adaptive_tcoeff_level_prediction_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.slice_header_restriction_flag);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.inter_layer_deblocking_filter_control_present_flag);
            size += stream.WriteUnsignedInt(2, this.extended_spatial_scalability_idc);

            if ((context.Sps.SeqParameterSetData.SeparateColourPlaneFlag == 0 ? context.Sps.SeqParameterSetData.ChromaFormatIdc : 0) == 1 || (context.Sps.SeqParameterSetData.SeparateColourPlaneFlag == 0 ? context.Sps.SeqParameterSetData.ChromaFormatIdc : 0) == 2)
            {
                size += stream.WriteUnsignedInt(1, this.chroma_phase_x_plus1_flag);
            }

            if ((context.Sps.SeqParameterSetData.SeparateColourPlaneFlag == 0 ? context.Sps.SeqParameterSetData.ChromaFormatIdc : 0) == 1)
            {
                size += stream.WriteUnsignedInt(2, this.chroma_phase_y_plus1);
            }

            if (extended_spatial_scalability_idc == 1)
            {

                if ((context.Sps.SeqParameterSetData.SeparateColourPlaneFlag == 0 ? context.Sps.SeqParameterSetData.ChromaFormatIdc : 0) > 0)
                {
                    size += stream.WriteUnsignedInt(1, this.seq_ref_layer_chroma_phase_x_plus1_flag);
                    size += stream.WriteUnsignedInt(2, this.seq_ref_layer_chroma_phase_y_plus1);
                }
                size += stream.WriteSignedIntGolomb(this.seq_scaled_ref_layer_left_offset);
                size += stream.WriteSignedIntGolomb(this.seq_scaled_ref_layer_top_offset);
                size += stream.WriteSignedIntGolomb(this.seq_scaled_ref_layer_right_offset);
                size += stream.WriteSignedIntGolomb(this.seq_scaled_ref_layer_bottom_offset);
            }
            size += stream.WriteUnsignedInt(1, this.seq_tcoeff_level_prediction_flag);

            if (seq_tcoeff_level_prediction_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.adaptive_tcoeff_level_prediction_flag);
            }
            size += stream.WriteUnsignedInt(1, this.slice_header_restriction_flag);

            return size;
        }

    }

    /*
   

scalability_info( payloadSize ) {  
 temporal_id_nesting_flag 5 u(1) 
 priority_layer_info_present_flag 5 u(1) 
 priority_id_setting_flag 5 u(1) 
 num_layers_minus1 5 ue(v) 
 for( i = 0; i <= num_layers_minus1; i++ ) {   
  layer_id[ i ] 5 ue(v) 
  priority_id[ i ] 5 u(6) 
  discardable_flag[ i ] 5 u(1) 
  dependency_id[ i ] 5 u(3) 
  quality_id[ i ] 5 u(4) 
  temporal_id[ i ] 5 u(3) 
  sub_pic_layer_flag[ i ] 5 u(1) 
  sub_region_layer_flag[ i ] 5 u(1) 
  iroi_division_info_present_flag[ i ] 5 u(1) 
  profile_level_info_present_flag[ i ] 5 u(1) 
  bitrate_info_present_flag[ i ] 5 u(1) 
  frm_rate_info_present_flag[ i ] 5 u(1) 
  frm_size_info_present_flag[ i ] 5 u(1) 
  layer_dependency_info_present_flag[ i ] 5 u(1) 
  parameter_sets_info_present_flag[ i ] 5 u(1) 
  bitstream_restriction_info_present_flag[ i ] 5 u(1) 
  exact_inter_layer_pred_flag[ i ] 5 u(1) 
  if( sub_pic_layer_flag[ i ]  ||  iroi_division_info_present_flag[ i ] )   
   exact_sample_value_match_flag[ i ] 5 u(1) 
  layer_conversion_flag[ i ] 5 u(1) 
  layer_output_flag[ i ] 5 u(1) 
  if( profile_level_info_present_flag[ i ] )   
   layer_profile_level_idc[ i ] 5 u(24) 
  if( bitrate_info_present_flag[ i ] ) {   
   avg_bitrate[ i ] 5 u(16) 
   max_bitrate_layer[ i ] 5 u(16) 
   max_bitrate_layer_representation[ i ] 5 u(16) 
   max_bitrate_calc_window[ i ] 5 u(16) 
  }   
  if( frm_rate_info_present_flag[ i ] ) {   
   constant_frm_rate_idc[ i ] 5 u(2) 
   avg_frm_rate[ i ] 5 u(16) 
  }   
  if( frm_size_info_present_flag[ i ]  ||   
   iroi_division_info_present_flag[ i ] ) { 
  
   frm_width_in_mbs_minus1[ i ] 5 ue(v) 
   frm_height_in_mbs_minus1[ i ] 5 ue(v) 
  }   
  if( sub_region_layer_flag[ i ] ) {   
   base_region_layer_id[ i ] 5 ue(v) 
      dynamic_rect_flag[ i ] 5 u(1) 
   if( !dynamic_rect_flag[ i ] ) {   
    horizontal_offset[ i ] 5 u(16) 
    vertical_offset[ i ] 5 u(16) 
    region_width[ i ] 5 u(16) 
    region_height[ i ] 5 u(16) 
   }   
  }   
  if( sub_pic_layer_flag[ i ] )    
   roi_id[ i ] 5 ue(v) 
  if( iroi_division_info_present_flag[ i ] ) {   
   iroi_grid_flag[ i ] 5 u(1) 
   if( iroi_grid_flag[ i ] ) {   
    grid_width_in_mbs_minus1[ i ] 5 ue(v) 
    grid_height_in_mbs_minus1[ i ] 5 ue(v) 
   } else {   
    num_rois_minus1[ i ] 5 ue(v) 
    for(j = 0; j <= num_rois_minus1[ i ]; j++ ) {   
     first_mb_in_roi[ i ][ j ] 5 ue(v) 
     roi_width_in_mbs_minus1[ i ][ j ] 5 ue(v) 
     roi_height_in_mbs_minus1[ i ][ j ] 5 ue(v) 
    }   
   }   
  }   
  if( layer_dependency_info_present_flag[ i ] ) {   
   num_directly_dependent_layers[ i ] 5 ue(v) 
   for( j = 0; j < num_directly_dependent_layers[ i ]; j++ )   
    directly_dependent_layer_id_delta_minus1[ i ][ j ] 5 ue(v) 
  } else   
   layer_dependency_info_src_layer_id_delta[ i ] 5 ue(v) 
  if( parameter_sets_info_present_flag[ i ] ) {   
   num_seq_parameter_sets[ i ] 5 ue(v) 
   for( j = 0; j < num_seq_parameter_sets[ i ]; j++ )   
    seq_parameter_set_id_delta[ i ][ j ] 5 ue(v) 
   num_subset_seq_parameter_sets[ i ] 5 ue(v) 
   for( j = 0; j < num_subset_seq_parameter_sets[ i ]; j++ )   
    subset_seq_parameter_set_id_delta[ i ][ j ] 5 ue(v) 
   num_pic_parameter_sets_minus1[ i ] 5 ue(v) 
   for( j = 0; j <= num_pic_parameter_sets_minus1[ i ]; j++ )   
    pic_parameter_set_id_delta[ i ][ j ] 5 ue(v) 
  } else   
   parameter_sets_info_src_layer_id_delta[ i ] 5 ue(v) 
  if( bitstream_restriction_info_present_flag[ i ] ) {   
   motion_vectors_over_pic_boundaries_flag[ i ] 5 u(1) 
   max_bytes_per_pic_denom[ i ] 5 ue(v) 
   max_bits_per_mb_denom[ i ] 5 ue(v) 
   log2_max_mv_length_horizontal[ i ] 5 ue(v) 
   log2_max_mv_length_vertical[ i ] 5 ue(v) 
      max_num_reorder_frames[ i ] 5 ue(v) 
   max_dec_frame_buffering[ i ] 5 ue(v) 
  }   
  if( layer_conversion_flag[ i ] ) {   
   conversion_type_idc[ i ] 5 ue(v) 
   for( j=0; j < 2; j++ ) {   
    rewriting_info_flag[ i ][ j ] 5 u(1) 
    if( rewriting_info_flag[ i ][ j ] ) {   
     rewriting_profile_level_idc[ i ][ j ] 5 u(24) 
     rewriting_avg_bitrate[ i ][ j ] 5 u(16) 
     rewriting_max_bitrate[ i ][ j ] 5 u(16) 
    }   
   }   
  }   
 }   
 if( priority_layer_info_present_flag ) {   
  pr_num_dIds_minus1 5 ue(v) 
  for( i = 0; i <= pr_num_dIds_minus1; i++ ) {   
   pr_dependency_id[ i ] 5 u(3) 
   pr_num_minus1[ i ] 5 ue(v) 
   for( j = 0; j <= pr_num_minus1[ i ]; j++ ) {   
    pr_id[ i ][ j ] 5 ue(v) 
    pr_profile_level_idc[ i ][ j ] 5 u(24) 
    pr_avg_bitrate[ i ][ j ] 5 u(16) 
    pr_max_bitrate[ i ][ j ] 5 u(16) 
   }   
  }   
 }   
 if( priority_id_setting_flag ) {   
  PriorityIdSettingUriIdx = 0   
  do    
   priority_id_setting_uri[ PriorityIdSettingUriIdx ] 5 b(8) 
  while( priority_id_setting_uri[ PriorityIdSettingUriIdx++ ]  !=  0 )   
 }   
}
    */
    public class ScalabilityInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte temporal_id_nesting_flag;
        public byte TemporalIdNestingFlag { get { return temporal_id_nesting_flag; } set { temporal_id_nesting_flag = value; } }
        private byte priority_layer_info_present_flag;
        public byte PriorityLayerInfoPresentFlag { get { return priority_layer_info_present_flag; } set { priority_layer_info_present_flag = value; } }
        private byte priority_id_setting_flag;
        public byte PriorityIdSettingFlag { get { return priority_id_setting_flag; } set { priority_id_setting_flag = value; } }
        private uint num_layers_minus1;
        public uint NumLayersMinus1 { get { return num_layers_minus1; } set { num_layers_minus1 = value; } }
        private uint[] layer_id;
        public uint[] LayerId { get { return layer_id; } set { layer_id = value; } }
        private uint[] priority_id;
        public uint[] PriorityId { get { return priority_id; } set { priority_id = value; } }
        private byte[] discardable_flag;
        public byte[] DiscardableFlag { get { return discardable_flag; } set { discardable_flag = value; } }
        private uint[] dependency_id;
        public uint[] DependencyId { get { return dependency_id; } set { dependency_id = value; } }
        private uint[] quality_id;
        public uint[] QualityId { get { return quality_id; } set { quality_id = value; } }
        private uint[] temporal_id;
        public uint[] TemporalId { get { return temporal_id; } set { temporal_id = value; } }
        private byte[] sub_pic_layer_flag;
        public byte[] SubPicLayerFlag { get { return sub_pic_layer_flag; } set { sub_pic_layer_flag = value; } }
        private byte[] sub_region_layer_flag;
        public byte[] SubRegionLayerFlag { get { return sub_region_layer_flag; } set { sub_region_layer_flag = value; } }
        private byte[] iroi_division_info_present_flag;
        public byte[] IroiDivisionInfoPresentFlag { get { return iroi_division_info_present_flag; } set { iroi_division_info_present_flag = value; } }
        private byte[] profile_level_info_present_flag;
        public byte[] ProfileLevelInfoPresentFlag { get { return profile_level_info_present_flag; } set { profile_level_info_present_flag = value; } }
        private byte[] bitrate_info_present_flag;
        public byte[] BitrateInfoPresentFlag { get { return bitrate_info_present_flag; } set { bitrate_info_present_flag = value; } }
        private byte[] frm_rate_info_present_flag;
        public byte[] FrmRateInfoPresentFlag { get { return frm_rate_info_present_flag; } set { frm_rate_info_present_flag = value; } }
        private byte[] frm_size_info_present_flag;
        public byte[] FrmSizeInfoPresentFlag { get { return frm_size_info_present_flag; } set { frm_size_info_present_flag = value; } }
        private byte[] layer_dependency_info_present_flag;
        public byte[] LayerDependencyInfoPresentFlag { get { return layer_dependency_info_present_flag; } set { layer_dependency_info_present_flag = value; } }
        private byte[] parameter_sets_info_present_flag;
        public byte[] ParameterSetsInfoPresentFlag { get { return parameter_sets_info_present_flag; } set { parameter_sets_info_present_flag = value; } }
        private byte[] bitstream_restriction_info_present_flag;
        public byte[] BitstreamRestrictionInfoPresentFlag { get { return bitstream_restriction_info_present_flag; } set { bitstream_restriction_info_present_flag = value; } }
        private byte[] exact_inter_layer_pred_flag;
        public byte[] ExactInterLayerPredFlag { get { return exact_inter_layer_pred_flag; } set { exact_inter_layer_pred_flag = value; } }
        private byte[] exact_sample_value_match_flag;
        public byte[] ExactSampleValueMatchFlag { get { return exact_sample_value_match_flag; } set { exact_sample_value_match_flag = value; } }
        private byte[] layer_conversion_flag;
        public byte[] LayerConversionFlag { get { return layer_conversion_flag; } set { layer_conversion_flag = value; } }
        private byte[] layer_output_flag;
        public byte[] LayerOutputFlag { get { return layer_output_flag; } set { layer_output_flag = value; } }
        private uint[] layer_profile_level_idc;
        public uint[] LayerProfileLevelIdc { get { return layer_profile_level_idc; } set { layer_profile_level_idc = value; } }
        private uint[] avg_bitrate;
        public uint[] AvgBitrate { get { return avg_bitrate; } set { avg_bitrate = value; } }
        private uint[] max_bitrate_layer;
        public uint[] MaxBitrateLayer { get { return max_bitrate_layer; } set { max_bitrate_layer = value; } }
        private uint[] max_bitrate_layer_representation;
        public uint[] MaxBitrateLayerRepresentation { get { return max_bitrate_layer_representation; } set { max_bitrate_layer_representation = value; } }
        private uint[] max_bitrate_calc_window;
        public uint[] MaxBitrateCalcWindow { get { return max_bitrate_calc_window; } set { max_bitrate_calc_window = value; } }
        private uint[] constant_frm_rate_idc;
        public uint[] ConstantFrmRateIdc { get { return constant_frm_rate_idc; } set { constant_frm_rate_idc = value; } }
        private uint[] avg_frm_rate;
        public uint[] AvgFrmRate { get { return avg_frm_rate; } set { avg_frm_rate = value; } }
        private uint[] frm_width_in_mbs_minus1;
        public uint[] FrmWidthInMbsMinus1 { get { return frm_width_in_mbs_minus1; } set { frm_width_in_mbs_minus1 = value; } }
        private uint[] frm_height_in_mbs_minus1;
        public uint[] FrmHeightInMbsMinus1 { get { return frm_height_in_mbs_minus1; } set { frm_height_in_mbs_minus1 = value; } }
        private uint[] base_region_layer_id;
        public uint[] BaseRegionLayerId { get { return base_region_layer_id; } set { base_region_layer_id = value; } }
        private byte[] dynamic_rect_flag;
        public byte[] DynamicRectFlag { get { return dynamic_rect_flag; } set { dynamic_rect_flag = value; } }
        private uint[] horizontal_offset;
        public uint[] HorizontalOffset { get { return horizontal_offset; } set { horizontal_offset = value; } }
        private uint[] vertical_offset;
        public uint[] VerticalOffset { get { return vertical_offset; } set { vertical_offset = value; } }
        private uint[] region_width;
        public uint[] RegionWidth { get { return region_width; } set { region_width = value; } }
        private uint[] region_height;
        public uint[] RegionHeight { get { return region_height; } set { region_height = value; } }
        private uint[] roi_id;
        public uint[] RoiId { get { return roi_id; } set { roi_id = value; } }
        private byte[] iroi_grid_flag;
        public byte[] IroiGridFlag { get { return iroi_grid_flag; } set { iroi_grid_flag = value; } }
        private uint[] grid_width_in_mbs_minus1;
        public uint[] GridWidthInMbsMinus1 { get { return grid_width_in_mbs_minus1; } set { grid_width_in_mbs_minus1 = value; } }
        private uint[] grid_height_in_mbs_minus1;
        public uint[] GridHeightInMbsMinus1 { get { return grid_height_in_mbs_minus1; } set { grid_height_in_mbs_minus1 = value; } }
        private uint[] num_rois_minus1;
        public uint[] NumRoisMinus1 { get { return num_rois_minus1; } set { num_rois_minus1 = value; } }
        private uint[][] first_mb_in_roi;
        public uint[][] FirstMbInRoi { get { return first_mb_in_roi; } set { first_mb_in_roi = value; } }
        private uint[][] roi_width_in_mbs_minus1;
        public uint[][] RoiWidthInMbsMinus1 { get { return roi_width_in_mbs_minus1; } set { roi_width_in_mbs_minus1 = value; } }
        private uint[][] roi_height_in_mbs_minus1;
        public uint[][] RoiHeightInMbsMinus1 { get { return roi_height_in_mbs_minus1; } set { roi_height_in_mbs_minus1 = value; } }
        private uint[] num_directly_dependent_layers;
        public uint[] NumDirectlyDependentLayers { get { return num_directly_dependent_layers; } set { num_directly_dependent_layers = value; } }
        private uint[][] directly_dependent_layer_id_delta_minus1;
        public uint[][] DirectlyDependentLayerIdDeltaMinus1 { get { return directly_dependent_layer_id_delta_minus1; } set { directly_dependent_layer_id_delta_minus1 = value; } }
        private uint[] layer_dependency_info_src_layer_id_delta;
        public uint[] LayerDependencyInfoSrcLayerIdDelta { get { return layer_dependency_info_src_layer_id_delta; } set { layer_dependency_info_src_layer_id_delta = value; } }
        private uint[] num_seq_parameter_sets;
        public uint[] NumSeqParameterSets { get { return num_seq_parameter_sets; } set { num_seq_parameter_sets = value; } }
        private uint[][] seq_parameter_set_id_delta;
        public uint[][] SeqParameterSetIdDelta { get { return seq_parameter_set_id_delta; } set { seq_parameter_set_id_delta = value; } }
        private uint[] num_subset_seq_parameter_sets;
        public uint[] NumSubsetSeqParameterSets { get { return num_subset_seq_parameter_sets; } set { num_subset_seq_parameter_sets = value; } }
        private uint[][] subset_seq_parameter_set_id_delta;
        public uint[][] SubsetSeqParameterSetIdDelta { get { return subset_seq_parameter_set_id_delta; } set { subset_seq_parameter_set_id_delta = value; } }
        private uint[] num_pic_parameter_sets_minus1;
        public uint[] NumPicParameterSetsMinus1 { get { return num_pic_parameter_sets_minus1; } set { num_pic_parameter_sets_minus1 = value; } }
        private uint[][] pic_parameter_set_id_delta;
        public uint[][] PicParameterSetIdDelta { get { return pic_parameter_set_id_delta; } set { pic_parameter_set_id_delta = value; } }
        private uint[] parameter_sets_info_src_layer_id_delta;
        public uint[] ParameterSetsInfoSrcLayerIdDelta { get { return parameter_sets_info_src_layer_id_delta; } set { parameter_sets_info_src_layer_id_delta = value; } }
        private byte[] motion_vectors_over_pic_boundaries_flag;
        public byte[] MotionVectorsOverPicBoundariesFlag { get { return motion_vectors_over_pic_boundaries_flag; } set { motion_vectors_over_pic_boundaries_flag = value; } }
        private uint[] max_bytes_per_pic_denom;
        public uint[] MaxBytesPerPicDenom { get { return max_bytes_per_pic_denom; } set { max_bytes_per_pic_denom = value; } }
        private uint[] max_bits_per_mb_denom;
        public uint[] MaxBitsPerMbDenom { get { return max_bits_per_mb_denom; } set { max_bits_per_mb_denom = value; } }
        private uint[] log2_max_mv_length_horizontal;
        public uint[] Log2MaxMvLengthHorizontal { get { return log2_max_mv_length_horizontal; } set { log2_max_mv_length_horizontal = value; } }
        private uint[] log2_max_mv_length_vertical;
        public uint[] Log2MaxMvLengthVertical { get { return log2_max_mv_length_vertical; } set { log2_max_mv_length_vertical = value; } }
        private uint[] max_num_reorder_frames;
        public uint[] MaxNumReorderFrames { get { return max_num_reorder_frames; } set { max_num_reorder_frames = value; } }
        private uint[] max_dec_frame_buffering;
        public uint[] MaxDecFrameBuffering { get { return max_dec_frame_buffering; } set { max_dec_frame_buffering = value; } }
        private uint[] conversion_type_idc;
        public uint[] ConversionTypeIdc { get { return conversion_type_idc; } set { conversion_type_idc = value; } }
        private byte[][] rewriting_info_flag;
        public byte[][] RewritingInfoFlag { get { return rewriting_info_flag; } set { rewriting_info_flag = value; } }
        private uint[][] rewriting_profile_level_idc;
        public uint[][] RewritingProfileLevelIdc { get { return rewriting_profile_level_idc; } set { rewriting_profile_level_idc = value; } }
        private uint[][] rewriting_avg_bitrate;
        public uint[][] RewritingAvgBitrate { get { return rewriting_avg_bitrate; } set { rewriting_avg_bitrate = value; } }
        private uint[][] rewriting_max_bitrate;
        public uint[][] RewritingMaxBitrate { get { return rewriting_max_bitrate; } set { rewriting_max_bitrate = value; } }
        private uint pr_num_dIds_minus1;
        public uint PrNumDIdsMinus1 { get { return pr_num_dIds_minus1; } set { pr_num_dIds_minus1 = value; } }
        private uint[] pr_dependency_id;
        public uint[] PrDependencyId { get { return pr_dependency_id; } set { pr_dependency_id = value; } }
        private uint[] pr_num_minus1;
        public uint[] PrNumMinus1 { get { return pr_num_minus1; } set { pr_num_minus1 = value; } }
        private uint[][] pr_id;
        public uint[][] PrId { get { return pr_id; } set { pr_id = value; } }
        private uint[][] pr_profile_level_idc;
        public uint[][] PrProfileLevelIdc { get { return pr_profile_level_idc; } set { pr_profile_level_idc = value; } }
        private uint[][] pr_avg_bitrate;
        public uint[][] PrAvgBitrate { get { return pr_avg_bitrate; } set { pr_avg_bitrate = value; } }
        private uint[][] pr_max_bitrate;
        public uint[][] PrMaxBitrate { get { return pr_max_bitrate; } set { pr_max_bitrate = value; } }
        private byte[] priority_id_setting_uri;
        public byte[] PriorityIdSettingUri { get { return priority_id_setting_uri; } set { priority_id_setting_uri = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ScalabilityInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            uint PriorityIdSettingUriIdx = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.temporal_id_nesting_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.priority_layer_info_present_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.priority_id_setting_flag);
            size += stream.ReadUnsignedIntGolomb(size, out this.num_layers_minus1);

            this.layer_id = new uint[num_layers_minus1 + 1];
            this.priority_id = new uint[num_layers_minus1 + 1];
            this.discardable_flag = new byte[num_layers_minus1 + 1];
            this.dependency_id = new uint[num_layers_minus1 + 1];
            this.quality_id = new uint[num_layers_minus1 + 1];
            this.temporal_id = new uint[num_layers_minus1 + 1];
            this.sub_pic_layer_flag = new byte[num_layers_minus1 + 1];
            this.sub_region_layer_flag = new byte[num_layers_minus1 + 1];
            this.iroi_division_info_present_flag = new byte[num_layers_minus1 + 1];
            this.profile_level_info_present_flag = new byte[num_layers_minus1 + 1];
            this.bitrate_info_present_flag = new byte[num_layers_minus1 + 1];
            this.frm_rate_info_present_flag = new byte[num_layers_minus1 + 1];
            this.frm_size_info_present_flag = new byte[num_layers_minus1 + 1];
            this.layer_dependency_info_present_flag = new byte[num_layers_minus1 + 1];
            this.parameter_sets_info_present_flag = new byte[num_layers_minus1 + 1];
            this.bitstream_restriction_info_present_flag = new byte[num_layers_minus1 + 1];
            this.exact_inter_layer_pred_flag = new byte[num_layers_minus1 + 1];
            this.exact_sample_value_match_flag = new byte[num_layers_minus1 + 1];
            this.layer_conversion_flag = new byte[num_layers_minus1 + 1];
            this.layer_output_flag = new byte[num_layers_minus1 + 1];
            this.layer_profile_level_idc = new uint[num_layers_minus1 + 1];
            this.avg_bitrate = new uint[num_layers_minus1 + 1];
            this.max_bitrate_layer = new uint[num_layers_minus1 + 1];
            this.max_bitrate_layer_representation = new uint[num_layers_minus1 + 1];
            this.max_bitrate_calc_window = new uint[num_layers_minus1 + 1];
            this.constant_frm_rate_idc = new uint[num_layers_minus1 + 1];
            this.avg_frm_rate = new uint[num_layers_minus1 + 1];
            this.frm_width_in_mbs_minus1 = new uint[num_layers_minus1 + 1];
            this.frm_height_in_mbs_minus1 = new uint[num_layers_minus1 + 1];
            this.base_region_layer_id = new uint[num_layers_minus1 + 1];
            this.dynamic_rect_flag = new byte[num_layers_minus1 + 1];
            this.horizontal_offset = new uint[num_layers_minus1 + 1];
            this.vertical_offset = new uint[num_layers_minus1 + 1];
            this.region_width = new uint[num_layers_minus1 + 1];
            this.region_height = new uint[num_layers_minus1 + 1];
            this.roi_id = new uint[num_layers_minus1 + 1];
            this.iroi_grid_flag = new byte[num_layers_minus1 + 1];
            this.grid_width_in_mbs_minus1 = new uint[num_layers_minus1 + 1];
            this.grid_height_in_mbs_minus1 = new uint[num_layers_minus1 + 1];
            this.num_rois_minus1 = new uint[num_layers_minus1 + 1];
            this.first_mb_in_roi = new uint[num_layers_minus1 + 1][];
            this.roi_width_in_mbs_minus1 = new uint[num_layers_minus1 + 1][];
            this.roi_height_in_mbs_minus1 = new uint[num_layers_minus1 + 1][];
            this.num_directly_dependent_layers = new uint[num_layers_minus1 + 1];
            this.directly_dependent_layer_id_delta_minus1 = new uint[num_layers_minus1 + 1][];
            this.layer_dependency_info_src_layer_id_delta = new uint[num_layers_minus1 + 1];
            this.num_seq_parameter_sets = new uint[num_layers_minus1 + 1];
            this.seq_parameter_set_id_delta = new uint[num_layers_minus1 + 1][];
            this.num_subset_seq_parameter_sets = new uint[num_layers_minus1 + 1];
            this.subset_seq_parameter_set_id_delta = new uint[num_layers_minus1 + 1][];
            this.num_pic_parameter_sets_minus1 = new uint[num_layers_minus1 + 1];
            this.pic_parameter_set_id_delta = new uint[num_layers_minus1 + 1][];
            this.parameter_sets_info_src_layer_id_delta = new uint[num_layers_minus1 + 1];
            this.motion_vectors_over_pic_boundaries_flag = new byte[num_layers_minus1 + 1];
            this.max_bytes_per_pic_denom = new uint[num_layers_minus1 + 1];
            this.max_bits_per_mb_denom = new uint[num_layers_minus1 + 1];
            this.log2_max_mv_length_horizontal = new uint[num_layers_minus1 + 1];
            this.log2_max_mv_length_vertical = new uint[num_layers_minus1 + 1];
            this.max_num_reorder_frames = new uint[num_layers_minus1 + 1];
            this.max_dec_frame_buffering = new uint[num_layers_minus1 + 1];
            this.conversion_type_idc = new uint[num_layers_minus1 + 1];
            this.rewriting_info_flag = new byte[num_layers_minus1 + 1][];
            this.rewriting_profile_level_idc = new uint[num_layers_minus1 + 1][];
            this.rewriting_avg_bitrate = new uint[num_layers_minus1 + 1][];
            this.rewriting_max_bitrate = new uint[num_layers_minus1 + 1][];
            for (i = 0; i <= num_layers_minus1; i++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.layer_id[i]);
                size += stream.ReadUnsignedInt(size, 6, out this.priority_id[i]);
                size += stream.ReadUnsignedInt(size, 1, out this.discardable_flag[i]);
                size += stream.ReadUnsignedInt(size, 3, out this.dependency_id[i]);
                size += stream.ReadUnsignedInt(size, 4, out this.quality_id[i]);
                size += stream.ReadUnsignedInt(size, 3, out this.temporal_id[i]);
                size += stream.ReadUnsignedInt(size, 1, out this.sub_pic_layer_flag[i]);
                size += stream.ReadUnsignedInt(size, 1, out this.sub_region_layer_flag[i]);
                size += stream.ReadUnsignedInt(size, 1, out this.iroi_division_info_present_flag[i]);
                size += stream.ReadUnsignedInt(size, 1, out this.profile_level_info_present_flag[i]);
                size += stream.ReadUnsignedInt(size, 1, out this.bitrate_info_present_flag[i]);
                size += stream.ReadUnsignedInt(size, 1, out this.frm_rate_info_present_flag[i]);
                size += stream.ReadUnsignedInt(size, 1, out this.frm_size_info_present_flag[i]);
                size += stream.ReadUnsignedInt(size, 1, out this.layer_dependency_info_present_flag[i]);
                size += stream.ReadUnsignedInt(size, 1, out this.parameter_sets_info_present_flag[i]);
                size += stream.ReadUnsignedInt(size, 1, out this.bitstream_restriction_info_present_flag[i]);
                size += stream.ReadUnsignedInt(size, 1, out this.exact_inter_layer_pred_flag[i]);

                if (sub_pic_layer_flag[i] != 0 || iroi_division_info_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.exact_sample_value_match_flag[i]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.layer_conversion_flag[i]);
                size += stream.ReadUnsignedInt(size, 1, out this.layer_output_flag[i]);

                if (profile_level_info_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 24, out this.layer_profile_level_idc[i]);
                }

                if (bitrate_info_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 16, out this.avg_bitrate[i]);
                    size += stream.ReadUnsignedInt(size, 16, out this.max_bitrate_layer[i]);
                    size += stream.ReadUnsignedInt(size, 16, out this.max_bitrate_layer_representation[i]);
                    size += stream.ReadUnsignedInt(size, 16, out this.max_bitrate_calc_window[i]);
                }

                if (frm_rate_info_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 2, out this.constant_frm_rate_idc[i]);
                    size += stream.ReadUnsignedInt(size, 16, out this.avg_frm_rate[i]);
                }

                if (frm_size_info_present_flag[i] != 0 ||
   iroi_division_info_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.frm_width_in_mbs_minus1[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.frm_height_in_mbs_minus1[i]);
                }

                if (sub_region_layer_flag[i] != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.base_region_layer_id[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.dynamic_rect_flag[i]);

                    if (dynamic_rect_flag[i] == 0)
                    {
                        size += stream.ReadUnsignedInt(size, 16, out this.horizontal_offset[i]);
                        size += stream.ReadUnsignedInt(size, 16, out this.vertical_offset[i]);
                        size += stream.ReadUnsignedInt(size, 16, out this.region_width[i]);
                        size += stream.ReadUnsignedInt(size, 16, out this.region_height[i]);
                    }
                }

                if (sub_pic_layer_flag[i] != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.roi_id[i]);
                }

                if (iroi_division_info_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.iroi_grid_flag[i]);

                    if (iroi_grid_flag[i] != 0)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.grid_width_in_mbs_minus1[i]);
                        size += stream.ReadUnsignedIntGolomb(size, out this.grid_height_in_mbs_minus1[i]);
                    }
                    else
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.num_rois_minus1[i]);

                        this.first_mb_in_roi[i] = new uint[num_rois_minus1[i] + 1];
                        this.roi_width_in_mbs_minus1[i] = new uint[num_rois_minus1[i] + 1];
                        this.roi_height_in_mbs_minus1[i] = new uint[num_rois_minus1[i] + 1];
                        for (j = 0; j <= num_rois_minus1[i]; j++)
                        {
                            size += stream.ReadUnsignedIntGolomb(size, out this.first_mb_in_roi[i][j]);
                            size += stream.ReadUnsignedIntGolomb(size, out this.roi_width_in_mbs_minus1[i][j]);
                            size += stream.ReadUnsignedIntGolomb(size, out this.roi_height_in_mbs_minus1[i][j]);
                        }
                    }
                }

                if (layer_dependency_info_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_directly_dependent_layers[i]);

                    this.directly_dependent_layer_id_delta_minus1[i] = new uint[num_directly_dependent_layers[i]];
                    for (j = 0; j < num_directly_dependent_layers[i]; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.directly_dependent_layer_id_delta_minus1[i][j]);
                    }
                }
                else
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.layer_dependency_info_src_layer_id_delta[i]);
                }

                if (parameter_sets_info_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_seq_parameter_sets[i]);

                    this.seq_parameter_set_id_delta[i] = new uint[num_seq_parameter_sets[i]];
                    for (j = 0; j < num_seq_parameter_sets[i]; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.seq_parameter_set_id_delta[i][j]);
                    }
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_subset_seq_parameter_sets[i]);

                    this.subset_seq_parameter_set_id_delta[i] = new uint[num_subset_seq_parameter_sets[i]];
                    for (j = 0; j < num_subset_seq_parameter_sets[i]; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.subset_seq_parameter_set_id_delta[i][j]);
                    }
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_pic_parameter_sets_minus1[i]);

                    this.pic_parameter_set_id_delta[i] = new uint[num_pic_parameter_sets_minus1[i] + 1];
                    for (j = 0; j <= num_pic_parameter_sets_minus1[i]; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.pic_parameter_set_id_delta[i][j]);
                    }
                }
                else
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.parameter_sets_info_src_layer_id_delta[i]);
                }

                if (bitstream_restriction_info_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.motion_vectors_over_pic_boundaries_flag[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.max_bytes_per_pic_denom[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.max_bits_per_mb_denom[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.log2_max_mv_length_horizontal[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.log2_max_mv_length_vertical[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.max_num_reorder_frames[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.max_dec_frame_buffering[i]);
                }

                if (layer_conversion_flag[i] != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.conversion_type_idc[i]);

                    this.rewriting_info_flag[i] = new byte[2];
                    this.rewriting_profile_level_idc[i] = new uint[2];
                    this.rewriting_avg_bitrate[i] = new uint[2];
                    this.rewriting_max_bitrate[i] = new uint[2];
                    for (j = 0; j < 2; j++)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.rewriting_info_flag[i][j]);

                        if (rewriting_info_flag[i][j] != 0)
                        {
                            size += stream.ReadUnsignedInt(size, 24, out this.rewriting_profile_level_idc[i][j]);
                            size += stream.ReadUnsignedInt(size, 16, out this.rewriting_avg_bitrate[i][j]);
                            size += stream.ReadUnsignedInt(size, 16, out this.rewriting_max_bitrate[i][j]);
                        }
                    }
                }
            }

            if (priority_layer_info_present_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.pr_num_dIds_minus1);

                this.pr_dependency_id = new uint[pr_num_dIds_minus1 + 1];
                this.pr_num_minus1 = new uint[pr_num_dIds_minus1 + 1];
                this.pr_id = new uint[pr_num_dIds_minus1 + 1][];
                this.pr_profile_level_idc = new uint[pr_num_dIds_minus1 + 1][];
                this.pr_avg_bitrate = new uint[pr_num_dIds_minus1 + 1][];
                this.pr_max_bitrate = new uint[pr_num_dIds_minus1 + 1][];
                for (i = 0; i <= pr_num_dIds_minus1; i++)
                {
                    size += stream.ReadUnsignedInt(size, 3, out this.pr_dependency_id[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.pr_num_minus1[i]);

                    this.pr_id[i] = new uint[pr_num_minus1[i] + 1];
                    this.pr_profile_level_idc[i] = new uint[pr_num_minus1[i] + 1];
                    this.pr_avg_bitrate[i] = new uint[pr_num_minus1[i] + 1];
                    this.pr_max_bitrate[i] = new uint[pr_num_minus1[i] + 1];
                    for (j = 0; j <= pr_num_minus1[i]; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.pr_id[i][j]);
                        size += stream.ReadUnsignedInt(size, 24, out this.pr_profile_level_idc[i][j]);
                        size += stream.ReadUnsignedInt(size, 16, out this.pr_avg_bitrate[i][j]);
                        size += stream.ReadUnsignedInt(size, 16, out this.pr_max_bitrate[i][j]);
                    }
                }
            }

            if (priority_id_setting_flag != 0)
            {
                PriorityIdSettingUriIdx = 0;

                do
                {
                    size += stream.ReadBits(size, 8, out this.priority_id_setting_uri[PriorityIdSettingUriIdx]);
                } while (priority_id_setting_uri[PriorityIdSettingUriIdx++] != 0);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            uint PriorityIdSettingUriIdx = 0;
            size += stream.WriteUnsignedInt(1, this.temporal_id_nesting_flag);
            size += stream.WriteUnsignedInt(1, this.priority_layer_info_present_flag);
            size += stream.WriteUnsignedInt(1, this.priority_id_setting_flag);
            size += stream.WriteUnsignedIntGolomb(this.num_layers_minus1);

            for (i = 0; i <= num_layers_minus1; i++)
            {
                size += stream.WriteUnsignedIntGolomb(this.layer_id[i]);
                size += stream.WriteUnsignedInt(6, this.priority_id[i]);
                size += stream.WriteUnsignedInt(1, this.discardable_flag[i]);
                size += stream.WriteUnsignedInt(3, this.dependency_id[i]);
                size += stream.WriteUnsignedInt(4, this.quality_id[i]);
                size += stream.WriteUnsignedInt(3, this.temporal_id[i]);
                size += stream.WriteUnsignedInt(1, this.sub_pic_layer_flag[i]);
                size += stream.WriteUnsignedInt(1, this.sub_region_layer_flag[i]);
                size += stream.WriteUnsignedInt(1, this.iroi_division_info_present_flag[i]);
                size += stream.WriteUnsignedInt(1, this.profile_level_info_present_flag[i]);
                size += stream.WriteUnsignedInt(1, this.bitrate_info_present_flag[i]);
                size += stream.WriteUnsignedInt(1, this.frm_rate_info_present_flag[i]);
                size += stream.WriteUnsignedInt(1, this.frm_size_info_present_flag[i]);
                size += stream.WriteUnsignedInt(1, this.layer_dependency_info_present_flag[i]);
                size += stream.WriteUnsignedInt(1, this.parameter_sets_info_present_flag[i]);
                size += stream.WriteUnsignedInt(1, this.bitstream_restriction_info_present_flag[i]);
                size += stream.WriteUnsignedInt(1, this.exact_inter_layer_pred_flag[i]);

                if (sub_pic_layer_flag[i] != 0 || iroi_division_info_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.exact_sample_value_match_flag[i]);
                }
                size += stream.WriteUnsignedInt(1, this.layer_conversion_flag[i]);
                size += stream.WriteUnsignedInt(1, this.layer_output_flag[i]);

                if (profile_level_info_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(24, this.layer_profile_level_idc[i]);
                }

                if (bitrate_info_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(16, this.avg_bitrate[i]);
                    size += stream.WriteUnsignedInt(16, this.max_bitrate_layer[i]);
                    size += stream.WriteUnsignedInt(16, this.max_bitrate_layer_representation[i]);
                    size += stream.WriteUnsignedInt(16, this.max_bitrate_calc_window[i]);
                }

                if (frm_rate_info_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(2, this.constant_frm_rate_idc[i]);
                    size += stream.WriteUnsignedInt(16, this.avg_frm_rate[i]);
                }

                if (frm_size_info_present_flag[i] != 0 ||
   iroi_division_info_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.frm_width_in_mbs_minus1[i]);
                    size += stream.WriteUnsignedIntGolomb(this.frm_height_in_mbs_minus1[i]);
                }

                if (sub_region_layer_flag[i] != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.base_region_layer_id[i]);
                    size += stream.WriteUnsignedInt(1, this.dynamic_rect_flag[i]);

                    if (dynamic_rect_flag[i] == 0)
                    {
                        size += stream.WriteUnsignedInt(16, this.horizontal_offset[i]);
                        size += stream.WriteUnsignedInt(16, this.vertical_offset[i]);
                        size += stream.WriteUnsignedInt(16, this.region_width[i]);
                        size += stream.WriteUnsignedInt(16, this.region_height[i]);
                    }
                }

                if (sub_pic_layer_flag[i] != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.roi_id[i]);
                }

                if (iroi_division_info_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.iroi_grid_flag[i]);

                    if (iroi_grid_flag[i] != 0)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.grid_width_in_mbs_minus1[i]);
                        size += stream.WriteUnsignedIntGolomb(this.grid_height_in_mbs_minus1[i]);
                    }
                    else
                    {
                        size += stream.WriteUnsignedIntGolomb(this.num_rois_minus1[i]);

                        for (j = 0; j <= num_rois_minus1[i]; j++)
                        {
                            size += stream.WriteUnsignedIntGolomb(this.first_mb_in_roi[i][j]);
                            size += stream.WriteUnsignedIntGolomb(this.roi_width_in_mbs_minus1[i][j]);
                            size += stream.WriteUnsignedIntGolomb(this.roi_height_in_mbs_minus1[i][j]);
                        }
                    }
                }

                if (layer_dependency_info_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.num_directly_dependent_layers[i]);

                    for (j = 0; j < num_directly_dependent_layers[i]; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.directly_dependent_layer_id_delta_minus1[i][j]);
                    }
                }
                else
                {
                    size += stream.WriteUnsignedIntGolomb(this.layer_dependency_info_src_layer_id_delta[i]);
                }

                if (parameter_sets_info_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.num_seq_parameter_sets[i]);

                    for (j = 0; j < num_seq_parameter_sets[i]; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.seq_parameter_set_id_delta[i][j]);
                    }
                    size += stream.WriteUnsignedIntGolomb(this.num_subset_seq_parameter_sets[i]);

                    for (j = 0; j < num_subset_seq_parameter_sets[i]; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.subset_seq_parameter_set_id_delta[i][j]);
                    }
                    size += stream.WriteUnsignedIntGolomb(this.num_pic_parameter_sets_minus1[i]);

                    for (j = 0; j <= num_pic_parameter_sets_minus1[i]; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.pic_parameter_set_id_delta[i][j]);
                    }
                }
                else
                {
                    size += stream.WriteUnsignedIntGolomb(this.parameter_sets_info_src_layer_id_delta[i]);
                }

                if (bitstream_restriction_info_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.motion_vectors_over_pic_boundaries_flag[i]);
                    size += stream.WriteUnsignedIntGolomb(this.max_bytes_per_pic_denom[i]);
                    size += stream.WriteUnsignedIntGolomb(this.max_bits_per_mb_denom[i]);
                    size += stream.WriteUnsignedIntGolomb(this.log2_max_mv_length_horizontal[i]);
                    size += stream.WriteUnsignedIntGolomb(this.log2_max_mv_length_vertical[i]);
                    size += stream.WriteUnsignedIntGolomb(this.max_num_reorder_frames[i]);
                    size += stream.WriteUnsignedIntGolomb(this.max_dec_frame_buffering[i]);
                }

                if (layer_conversion_flag[i] != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.conversion_type_idc[i]);

                    for (j = 0; j < 2; j++)
                    {
                        size += stream.WriteUnsignedInt(1, this.rewriting_info_flag[i][j]);

                        if (rewriting_info_flag[i][j] != 0)
                        {
                            size += stream.WriteUnsignedInt(24, this.rewriting_profile_level_idc[i][j]);
                            size += stream.WriteUnsignedInt(16, this.rewriting_avg_bitrate[i][j]);
                            size += stream.WriteUnsignedInt(16, this.rewriting_max_bitrate[i][j]);
                        }
                    }
                }
            }

            if (priority_layer_info_present_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.pr_num_dIds_minus1);

                for (i = 0; i <= pr_num_dIds_minus1; i++)
                {
                    size += stream.WriteUnsignedInt(3, this.pr_dependency_id[i]);
                    size += stream.WriteUnsignedIntGolomb(this.pr_num_minus1[i]);

                    for (j = 0; j <= pr_num_minus1[i]; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.pr_id[i][j]);
                        size += stream.WriteUnsignedInt(24, this.pr_profile_level_idc[i][j]);
                        size += stream.WriteUnsignedInt(16, this.pr_avg_bitrate[i][j]);
                        size += stream.WriteUnsignedInt(16, this.pr_max_bitrate[i][j]);
                    }
                }
            }

            if (priority_id_setting_flag != 0)
            {
                PriorityIdSettingUriIdx = 0;

                do
                {
                    size += stream.WriteBits(8, this.priority_id_setting_uri[PriorityIdSettingUriIdx]);
                } while (priority_id_setting_uri[PriorityIdSettingUriIdx++] != 0);
            }

            return size;
        }

    }

    /*
  

sub_pic_scalable_layer( payloadSize ) {  
 layer_id 5 ue(v) 
}
    */
    public class SubPicScalableLayer : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint layer_id;
        public uint LayerId { get { return layer_id; } set { layer_id = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SubPicScalableLayer(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedIntGolomb(size, out this.layer_id);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedIntGolomb(this.layer_id);

            return size;
        }

    }

    /*


non_required_layer_rep( payloadSize ) {  
 num_info_entries_minus1 5 ue(v) 
 for( i = 0; i <= num_info_entries_minus1; i++ ) {   
  entry_dependency_id[ i ] 5 u(3) 
  num_non_required_layer_reps_minus1[ i ] 5 ue(v) 
  for( j = 0; j <= num_non_required_layer_reps_minus1[ i ]; j++ ) {   
   non_required_layer_rep_dependency_id[ i ][ j ] 5 u(3) 
   non_required_layer_rep_quality_id[ i ][ j ] 5 u(4) 
  }   
 }   
}
    */
    public class NonRequiredLayerRep : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint num_info_entries_minus1;
        public uint NumInfoEntriesMinus1 { get { return num_info_entries_minus1; } set { num_info_entries_minus1 = value; } }
        private uint[] entry_dependency_id;
        public uint[] EntryDependencyId { get { return entry_dependency_id; } set { entry_dependency_id = value; } }
        private uint[] num_non_required_layer_reps_minus1;
        public uint[] NumNonRequiredLayerRepsMinus1 { get { return num_non_required_layer_reps_minus1; } set { num_non_required_layer_reps_minus1 = value; } }
        private uint[][] non_required_layer_rep_dependency_id;
        public uint[][] NonRequiredLayerRepDependencyId { get { return non_required_layer_rep_dependency_id; } set { non_required_layer_rep_dependency_id = value; } }
        private uint[][] non_required_layer_rep_quality_id;
        public uint[][] NonRequiredLayerRepQualityId { get { return non_required_layer_rep_quality_id; } set { non_required_layer_rep_quality_id = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public NonRequiredLayerRep(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.num_info_entries_minus1);

            this.entry_dependency_id = new uint[num_info_entries_minus1 + 1];
            this.num_non_required_layer_reps_minus1 = new uint[num_info_entries_minus1 + 1];
            this.non_required_layer_rep_dependency_id = new uint[num_info_entries_minus1 + 1][];
            this.non_required_layer_rep_quality_id = new uint[num_info_entries_minus1 + 1][];
            for (i = 0; i <= num_info_entries_minus1; i++)
            {
                size += stream.ReadUnsignedInt(size, 3, out this.entry_dependency_id[i]);
                size += stream.ReadUnsignedIntGolomb(size, out this.num_non_required_layer_reps_minus1[i]);

                this.non_required_layer_rep_dependency_id[i] = new uint[num_non_required_layer_reps_minus1[i] + 1];
                this.non_required_layer_rep_quality_id[i] = new uint[num_non_required_layer_reps_minus1[i] + 1];
                for (j = 0; j <= num_non_required_layer_reps_minus1[i]; j++)
                {
                    size += stream.ReadUnsignedInt(size, 3, out this.non_required_layer_rep_dependency_id[i][j]);
                    size += stream.ReadUnsignedInt(size, 4, out this.non_required_layer_rep_quality_id[i][j]);
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.WriteUnsignedIntGolomb(this.num_info_entries_minus1);

            for (i = 0; i <= num_info_entries_minus1; i++)
            {
                size += stream.WriteUnsignedInt(3, this.entry_dependency_id[i]);
                size += stream.WriteUnsignedIntGolomb(this.num_non_required_layer_reps_minus1[i]);

                for (j = 0; j <= num_non_required_layer_reps_minus1[i]; j++)
                {
                    size += stream.WriteUnsignedInt(3, this.non_required_layer_rep_dependency_id[i][j]);
                    size += stream.WriteUnsignedInt(4, this.non_required_layer_rep_quality_id[i][j]);
                }
            }

            return size;
        }

    }

    /*
   

priority_layer_info( payloadSize ) {  
 pr_dependency_id 5 u(3) 
 num_priority_ids 5 u(4) 
 for( i = 0; i < num_priority_ids; i++ ) {   
  alt_priority_id[ i ] 5 u(6) 
 }   
}
    */
    public class PriorityLayerInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint pr_dependency_id;
        public uint PrDependencyId { get { return pr_dependency_id; } set { pr_dependency_id = value; } }
        private uint num_priority_ids;
        public uint NumPriorityIds { get { return num_priority_ids; } set { num_priority_ids = value; } }
        private uint[] alt_priority_id;
        public uint[] AltPriorityId { get { return alt_priority_id; } set { alt_priority_id = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public PriorityLayerInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedInt(size, 3, out this.pr_dependency_id);
            size += stream.ReadUnsignedInt(size, 4, out this.num_priority_ids);

            this.alt_priority_id = new uint[num_priority_ids];
            for (i = 0; i < num_priority_ids; i++)
            {
                size += stream.ReadUnsignedInt(size, 6, out this.alt_priority_id[i]);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedInt(3, this.pr_dependency_id);
            size += stream.WriteUnsignedInt(4, this.num_priority_ids);

            for (i = 0; i < num_priority_ids; i++)
            {
                size += stream.WriteUnsignedInt(6, this.alt_priority_id[i]);
            }

            return size;
        }

    }

    /*
   

layers_not_present( payloadSize ) {  
 num_layers 5 ue(v) 
 for( i = 0; i < num_layers; i++ ) {   
  layer_id[ i ] 5 ue(v) 
 }   
}
    */
    public class LayersNotPresent : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint num_layers;
        public uint NumLayers { get { return num_layers; } set { num_layers = value; } }
        private uint[] layer_id;
        public uint[] LayerId { get { return layer_id; } set { layer_id = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public LayersNotPresent(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.num_layers);

            this.layer_id = new uint[num_layers];
            for (i = 0; i < num_layers; i++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.layer_id[i]);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedIntGolomb(this.num_layers);

            for (i = 0; i < num_layers; i++)
            {
                size += stream.WriteUnsignedIntGolomb(this.layer_id[i]);
            }

            return size;
        }

    }

    /*
   

layer_dependency_change( payloadSize ) {  
 num_layers_minus1 5 ue(v) 
 for( i = 0; i <= num_layers_minus1; i++ ) {   
  layer_id[ i ] 5 ue(v) 
  layer_dependency_info_present_flag[ i ] 5 u(1) 
  if( layer_dependency_info_present_flag[ i ] ) {   
   num_directly_dependent_layers[ i ] 5 ue(v) 
   for( j = 0; j < num_directly_dependent_layers[ i ]; j++ )   
    directly_dependent_layer_id_delta_minus1[ i ][ j ] 5 ue(v) 
  } else {   
   layer_dependency_info_src_layer_id_delta_minus1[ i ] 5 ue(v) 
  }   
   }   
}
    */
    public class LayerDependencyChange : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint num_layers_minus1;
        public uint NumLayersMinus1 { get { return num_layers_minus1; } set { num_layers_minus1 = value; } }
        private uint[] layer_id;
        public uint[] LayerId { get { return layer_id; } set { layer_id = value; } }
        private byte[] layer_dependency_info_present_flag;
        public byte[] LayerDependencyInfoPresentFlag { get { return layer_dependency_info_present_flag; } set { layer_dependency_info_present_flag = value; } }
        private uint[] num_directly_dependent_layers;
        public uint[] NumDirectlyDependentLayers { get { return num_directly_dependent_layers; } set { num_directly_dependent_layers = value; } }
        private uint[][] directly_dependent_layer_id_delta_minus1;
        public uint[][] DirectlyDependentLayerIdDeltaMinus1 { get { return directly_dependent_layer_id_delta_minus1; } set { directly_dependent_layer_id_delta_minus1 = value; } }
        private uint[] layer_dependency_info_src_layer_id_delta_minus1;
        public uint[] LayerDependencyInfoSrcLayerIdDeltaMinus1 { get { return layer_dependency_info_src_layer_id_delta_minus1; } set { layer_dependency_info_src_layer_id_delta_minus1 = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public LayerDependencyChange(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.num_layers_minus1);

            this.layer_id = new uint[num_layers_minus1 + 1];
            this.layer_dependency_info_present_flag = new byte[num_layers_minus1 + 1];
            this.num_directly_dependent_layers = new uint[num_layers_minus1 + 1];
            this.directly_dependent_layer_id_delta_minus1 = new uint[num_layers_minus1 + 1][];
            this.layer_dependency_info_src_layer_id_delta_minus1 = new uint[num_layers_minus1 + 1];
            for (i = 0; i <= num_layers_minus1; i++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.layer_id[i]);
                size += stream.ReadUnsignedInt(size, 1, out this.layer_dependency_info_present_flag[i]);

                if (layer_dependency_info_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_directly_dependent_layers[i]);

                    this.directly_dependent_layer_id_delta_minus1[i] = new uint[num_directly_dependent_layers[i]];
                    for (j = 0; j < num_directly_dependent_layers[i]; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.directly_dependent_layer_id_delta_minus1[i][j]);
                    }
                }
                else
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.layer_dependency_info_src_layer_id_delta_minus1[i]);
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.WriteUnsignedIntGolomb(this.num_layers_minus1);

            for (i = 0; i <= num_layers_minus1; i++)
            {
                size += stream.WriteUnsignedIntGolomb(this.layer_id[i]);
                size += stream.WriteUnsignedInt(1, this.layer_dependency_info_present_flag[i]);

                if (layer_dependency_info_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.num_directly_dependent_layers[i]);

                    for (j = 0; j < num_directly_dependent_layers[i]; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.directly_dependent_layer_id_delta_minus1[i][j]);
                    }
                }
                else
                {
                    size += stream.WriteUnsignedIntGolomb(this.layer_dependency_info_src_layer_id_delta_minus1[i]);
                }
            }

            return size;
        }

    }

    /*
    

scalable_nesting( payloadSize ) {  
 all_layer_representations_in_au_flag 5 u(1) 
 if( all_layer_representations_in_au_flag  ==  0) {   
  num_layer_representations_minus1 5 ue(v) 
  for( i = 0; i <= num_layer_representations_minus1; i++ ) {   
   sei_dependency_id[ i ] 5 u(3) 
   sei_quality_id[ i ] 5 u(4) 
  }   
  sei_temporal_id 5 u(3) 
 }   
 while( !byte_aligned() )   
  sei_nesting_zero_bit /* equal to 0 *//* 5 f(1) 
 do   
  sei_message() 5  
 while( more_rbsp_data() )   
}
    */
    public class ScalableNesting : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte all_layer_representations_in_au_flag;
        public byte AllLayerRepresentationsInAuFlag { get { return all_layer_representations_in_au_flag; } set { all_layer_representations_in_au_flag = value; } }
        private uint num_layer_representations_minus1;
        public uint NumLayerRepresentationsMinus1 { get { return num_layer_representations_minus1; } set { num_layer_representations_minus1 = value; } }
        private uint[] sei_dependency_id;
        public uint[] SeiDependencyId { get { return sei_dependency_id; } set { sei_dependency_id = value; } }
        private uint[] sei_quality_id;
        public uint[] SeiQualityId { get { return sei_quality_id; } set { sei_quality_id = value; } }
        private uint sei_temporal_id;
        public uint SeiTemporalId { get { return sei_temporal_id; } set { sei_temporal_id = value; } }
        private uint sei_nesting_zero_bit;
        public uint SeiNestingZeroBit { get { return sei_nesting_zero_bit; } set { sei_nesting_zero_bit = value; } }
        private SeiMessage sei_message;
        public SeiMessage SeiMessage { get { return sei_message; } set { sei_message = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ScalableNesting(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.all_layer_representations_in_au_flag);

            if (all_layer_representations_in_au_flag == 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.num_layer_representations_minus1);

                this.sei_dependency_id = new uint[num_layer_representations_minus1 + 1];
                this.sei_quality_id = new uint[num_layer_representations_minus1 + 1];
                for (i = 0; i <= num_layer_representations_minus1; i++)
                {
                    size += stream.ReadUnsignedInt(size, 3, out this.sei_dependency_id[i]);
                    size += stream.ReadUnsignedInt(size, 4, out this.sei_quality_id[i]);
                }
                size += stream.ReadUnsignedInt(size, 3, out this.sei_temporal_id);
            }

            while (!stream.ByteAligned())
            {
                size += stream.ReadFixed(size, 1, out this.sei_nesting_zero_bit); // equal to 0 
            }

            do
            {
                size += stream.ReadClass<SeiMessage>(size, context, () => new SeiMessage(), out this.sei_message);
            } while (stream.ReadMoreRbspData(this));

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedInt(1, this.all_layer_representations_in_au_flag);

            if (all_layer_representations_in_au_flag == 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.num_layer_representations_minus1);

                for (i = 0; i <= num_layer_representations_minus1; i++)
                {
                    size += stream.WriteUnsignedInt(3, this.sei_dependency_id[i]);
                    size += stream.WriteUnsignedInt(4, this.sei_quality_id[i]);
                }
                size += stream.WriteUnsignedInt(3, this.sei_temporal_id);
            }

            while (!stream.ByteAligned())
            {
                size += stream.WriteFixed(1, this.sei_nesting_zero_bit); // equal to 0 
            }

            do
            {
                size += stream.WriteClass<SeiMessage>(context, this.sei_message);
            } while (stream.WriteMoreRbspData(this));

            return size;
        }

    }

    /*
   

base_layer_temporal_hrd( payloadSize ) {  
 num_of_temporal_layers_in_base_layer_minus1 5 ue(v) 
 for( i = 0; i <= num_of_temporal_layers_in_base_layer_minus1; i++ ) {   
  sei_temporal_id[ i ] 5 u(3) 
  sei_timing_info_present_flag[ i ] 5 u(1) 
  if( sei_timing_info_present_flag[ i ] ) {   
   sei_num_units_in_tick[ i ] 5 u(32) 
   sei_time_scale[ i ] 5 u(32) 
   sei_fixed_frame_rate_flag[ i ] 5 u(1) 
  }   
  sei_nal_hrd_parameters_present_flag[ i ] 5 u(1) 
  if( sei_nal_hrd_parameters_present_flag[ i ] )   
   hrd_parameters() 5  
  sei_vcl_hrd_parameters_present_flag[ i ] 5 u(1) 
  if( sei_vcl_hrd_parameters_present_flag[ i ] )   
   hrd_parameters() 5  
  if( sei_nal_hrd_parameters_present_flag[ i ]  ||   
   sei_vcl_hrd_parameters_present_flag[ i ] ) 
  
   sei_low_delay_hrd_flag[ i ] 5 u(1) 
  sei_pic_struct_present_flag[ i ] 5 u(1) 
 }   
}
    */
    public class BaseLayerTemporalHrd : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint num_of_temporal_layers_in_base_layer_minus1;
        public uint NumOfTemporalLayersInBaseLayerMinus1 { get { return num_of_temporal_layers_in_base_layer_minus1; } set { num_of_temporal_layers_in_base_layer_minus1 = value; } }
        private uint[] sei_temporal_id;
        public uint[] SeiTemporalId { get { return sei_temporal_id; } set { sei_temporal_id = value; } }
        private byte[] sei_timing_info_present_flag;
        public byte[] SeiTimingInfoPresentFlag { get { return sei_timing_info_present_flag; } set { sei_timing_info_present_flag = value; } }
        private uint[] sei_num_units_in_tick;
        public uint[] SeiNumUnitsInTick { get { return sei_num_units_in_tick; } set { sei_num_units_in_tick = value; } }
        private uint[] sei_time_scale;
        public uint[] SeiTimeScale { get { return sei_time_scale; } set { sei_time_scale = value; } }
        private byte[] sei_fixed_frame_rate_flag;
        public byte[] SeiFixedFrameRateFlag { get { return sei_fixed_frame_rate_flag; } set { sei_fixed_frame_rate_flag = value; } }
        private byte[] sei_nal_hrd_parameters_present_flag;
        public byte[] SeiNalHrdParametersPresentFlag { get { return sei_nal_hrd_parameters_present_flag; } set { sei_nal_hrd_parameters_present_flag = value; } }
        private HrdParameters[] hrd_parameters;
        public HrdParameters[] HrdParameters { get { return hrd_parameters; } set { hrd_parameters = value; } }
        private byte[] sei_vcl_hrd_parameters_present_flag;
        public byte[] SeiVclHrdParametersPresentFlag { get { return sei_vcl_hrd_parameters_present_flag; } set { sei_vcl_hrd_parameters_present_flag = value; } }
        private byte[] sei_low_delay_hrd_flag;
        public byte[] SeiLowDelayHrdFlag { get { return sei_low_delay_hrd_flag; } set { sei_low_delay_hrd_flag = value; } }
        private byte[] sei_pic_struct_present_flag;
        public byte[] SeiPicStructPresentFlag { get { return sei_pic_struct_present_flag; } set { sei_pic_struct_present_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public BaseLayerTemporalHrd(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.num_of_temporal_layers_in_base_layer_minus1);

            this.sei_temporal_id = new uint[num_of_temporal_layers_in_base_layer_minus1 + 1];
            this.sei_timing_info_present_flag = new byte[num_of_temporal_layers_in_base_layer_minus1 + 1];
            this.sei_num_units_in_tick = new uint[num_of_temporal_layers_in_base_layer_minus1 + 1];
            this.sei_time_scale = new uint[num_of_temporal_layers_in_base_layer_minus1 + 1];
            this.sei_fixed_frame_rate_flag = new byte[num_of_temporal_layers_in_base_layer_minus1 + 1];
            this.sei_nal_hrd_parameters_present_flag = new byte[num_of_temporal_layers_in_base_layer_minus1 + 1];
            this.hrd_parameters = new HrdParameters[num_of_temporal_layers_in_base_layer_minus1 + 1];
            this.sei_vcl_hrd_parameters_present_flag = new byte[num_of_temporal_layers_in_base_layer_minus1 + 1];
            this.sei_low_delay_hrd_flag = new byte[num_of_temporal_layers_in_base_layer_minus1 + 1];
            this.sei_pic_struct_present_flag = new byte[num_of_temporal_layers_in_base_layer_minus1 + 1];
            for (i = 0; i <= num_of_temporal_layers_in_base_layer_minus1; i++)
            {
                size += stream.ReadUnsignedInt(size, 3, out this.sei_temporal_id[i]);
                size += stream.ReadUnsignedInt(size, 1, out this.sei_timing_info_present_flag[i]);

                if (sei_timing_info_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 32, out this.sei_num_units_in_tick[i]);
                    size += stream.ReadUnsignedInt(size, 32, out this.sei_time_scale[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.sei_fixed_frame_rate_flag[i]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.sei_nal_hrd_parameters_present_flag[i]);

                if (sei_nal_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.ReadClass<HrdParameters>(size, context, () => new HrdParameters(), out this.hrd_parameters[i]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.sei_vcl_hrd_parameters_present_flag[i]);

                if (sei_vcl_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.ReadClass<HrdParameters>(size, context, () => new HrdParameters(), out this.hrd_parameters[i]);
                }

                if (sei_nal_hrd_parameters_present_flag[i] != 0 ||
   sei_vcl_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.sei_low_delay_hrd_flag[i]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.sei_pic_struct_present_flag[i]);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedIntGolomb(this.num_of_temporal_layers_in_base_layer_minus1);

            for (i = 0; i <= num_of_temporal_layers_in_base_layer_minus1; i++)
            {
                size += stream.WriteUnsignedInt(3, this.sei_temporal_id[i]);
                size += stream.WriteUnsignedInt(1, this.sei_timing_info_present_flag[i]);

                if (sei_timing_info_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(32, this.sei_num_units_in_tick[i]);
                    size += stream.WriteUnsignedInt(32, this.sei_time_scale[i]);
                    size += stream.WriteUnsignedInt(1, this.sei_fixed_frame_rate_flag[i]);
                }
                size += stream.WriteUnsignedInt(1, this.sei_nal_hrd_parameters_present_flag[i]);

                if (sei_nal_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.WriteClass<HrdParameters>(context, this.hrd_parameters[i]);
                }
                size += stream.WriteUnsignedInt(1, this.sei_vcl_hrd_parameters_present_flag[i]);

                if (sei_vcl_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.WriteClass<HrdParameters>(context, this.hrd_parameters[i]);
                }

                if (sei_nal_hrd_parameters_present_flag[i] != 0 ||
   sei_vcl_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.sei_low_delay_hrd_flag[i]);
                }
                size += stream.WriteUnsignedInt(1, this.sei_pic_struct_present_flag[i]);
            }

            return size;
        }

    }

    /*
  

quality_layer_integrity_check( payloadSize ) {  
 num_info_entries_minus1 5 ue(v) 
 for( i = 0; i <= num_info_entries_minus1; i++ ) {   
  entry_dependency_id[ i ] 5 u(3) 
  quality_layer_crc[ i ] 5 u(16) 
 }   
}
    */
    public class QualityLayerIntegrityCheck : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint num_info_entries_minus1;
        public uint NumInfoEntriesMinus1 { get { return num_info_entries_minus1; } set { num_info_entries_minus1 = value; } }
        private uint[] entry_dependency_id;
        public uint[] EntryDependencyId { get { return entry_dependency_id; } set { entry_dependency_id = value; } }
        private uint[] quality_layer_crc;
        public uint[] QualityLayerCrc { get { return quality_layer_crc; } set { quality_layer_crc = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public QualityLayerIntegrityCheck(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.num_info_entries_minus1);

            this.entry_dependency_id = new uint[num_info_entries_minus1 + 1];
            this.quality_layer_crc = new uint[num_info_entries_minus1 + 1];
            for (i = 0; i <= num_info_entries_minus1; i++)
            {
                size += stream.ReadUnsignedInt(size, 3, out this.entry_dependency_id[i]);
                size += stream.ReadUnsignedInt(size, 16, out this.quality_layer_crc[i]);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedIntGolomb(this.num_info_entries_minus1);

            for (i = 0; i <= num_info_entries_minus1; i++)
            {
                size += stream.WriteUnsignedInt(3, this.entry_dependency_id[i]);
                size += stream.WriteUnsignedInt(16, this.quality_layer_crc[i]);
            }

            return size;
        }

    }

    /*
   

redundant_pic_property( payloadSize ) {  
 num_dIds_minus1 5 ue(v) 
 for( i = 0; i <= num_dIds_minus1; i++ ) {   
  dependency_id[ i ] 5 u(3) 
  num_qIds_minus1[ i ] 5 ue(v) 
  for( j = 0; j <= num_qIds_minus1[ i ]; j++ ) {   
   quality_id[ i ][ j ] 5 u(4) 
   num_redundant_pics_minus1[ i ][ j ] 5 ue(v) 
   for( k = 0; k <= num_redundant_pics_minus1[ i ][ j ]; k++ ) {   
    redundant_pic_cnt_minus1[ i ][ j ][ k ] 5 ue(v) 
    pic_match_flag[ i ][ j ][ k ] 5 u(1) 
    if( !pic_match_flag[ i ][ j ][ k ]) {   
     mb_type_match_flag[ i ][ j ][ k ] 5 u(1) 
     motion_match_flag[ i ][ j ][ k ] 5 u(1) 
     residual_match_flag[ i ][ j ][ k ] 5 u(1) 
     intra_samples_match_flag[ i ][ j ][ k ] 5 u(1) 
    }   
   }   
  }   
 }   
}
    */
    public class RedundantPicProperty : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint num_dIds_minus1;
        public uint NumDIdsMinus1 { get { return num_dIds_minus1; } set { num_dIds_minus1 = value; } }
        private uint[] dependency_id;
        public uint[] DependencyId { get { return dependency_id; } set { dependency_id = value; } }
        private uint[] num_qIds_minus1;
        public uint[] NumQIdsMinus1 { get { return num_qIds_minus1; } set { num_qIds_minus1 = value; } }
        private uint[][] quality_id;
        public uint[][] QualityId { get { return quality_id; } set { quality_id = value; } }
        private uint[][] num_redundant_pics_minus1;
        public uint[][] NumRedundantPicsMinus1 { get { return num_redundant_pics_minus1; } set { num_redundant_pics_minus1 = value; } }
        private uint[][][] redundant_pic_cnt_minus1;
        public uint[][][] RedundantPicCntMinus1 { get { return redundant_pic_cnt_minus1; } set { redundant_pic_cnt_minus1 = value; } }
        private byte[][][] pic_match_flag;
        public byte[][][] PicMatchFlag { get { return pic_match_flag; } set { pic_match_flag = value; } }
        private byte[][][] mb_type_match_flag;
        public byte[][][] MbTypeMatchFlag { get { return mb_type_match_flag; } set { mb_type_match_flag = value; } }
        private byte[][][] motion_match_flag;
        public byte[][][] MotionMatchFlag { get { return motion_match_flag; } set { motion_match_flag = value; } }
        private byte[][][] residual_match_flag;
        public byte[][][] ResidualMatchFlag { get { return residual_match_flag; } set { residual_match_flag = value; } }
        private byte[][][] intra_samples_match_flag;
        public byte[][][] IntraSamplesMatchFlag { get { return intra_samples_match_flag; } set { intra_samples_match_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public RedundantPicProperty(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            uint k = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.num_dIds_minus1);

            this.dependency_id = new uint[num_dIds_minus1 + 1];
            this.num_qIds_minus1 = new uint[num_dIds_minus1 + 1];
            this.quality_id = new uint[num_dIds_minus1 + 1][];
            this.num_redundant_pics_minus1 = new uint[num_dIds_minus1 + 1][];
            this.redundant_pic_cnt_minus1 = new uint[num_dIds_minus1 + 1][][];
            this.pic_match_flag = new byte[num_dIds_minus1 + 1][][];
            this.mb_type_match_flag = new byte[num_dIds_minus1 + 1][][];
            this.motion_match_flag = new byte[num_dIds_minus1 + 1][][];
            this.residual_match_flag = new byte[num_dIds_minus1 + 1][][];
            this.intra_samples_match_flag = new byte[num_dIds_minus1 + 1][][];
            for (i = 0; i <= num_dIds_minus1; i++)
            {
                size += stream.ReadUnsignedInt(size, 3, out this.dependency_id[i]);
                size += stream.ReadUnsignedIntGolomb(size, out this.num_qIds_minus1[i]);

                this.quality_id[i] = new uint[num_qIds_minus1[i] + 1];
                this.num_redundant_pics_minus1[i] = new uint[num_qIds_minus1[i] + 1];
                this.redundant_pic_cnt_minus1[i] = new uint[num_qIds_minus1[i] + 1][];
                this.pic_match_flag[i] = new byte[num_qIds_minus1[i] + 1][];
                this.mb_type_match_flag[i] = new byte[num_qIds_minus1[i] + 1][];
                this.motion_match_flag[i] = new byte[num_qIds_minus1[i] + 1][];
                this.residual_match_flag[i] = new byte[num_qIds_minus1[i] + 1][];
                this.intra_samples_match_flag[i] = new byte[num_qIds_minus1[i] + 1][];
                for (j = 0; j <= num_qIds_minus1[i]; j++)
                {
                    size += stream.ReadUnsignedInt(size, 4, out this.quality_id[i][j]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_redundant_pics_minus1[i][j]);

                    this.redundant_pic_cnt_minus1[i][j] = new uint[num_redundant_pics_minus1[i][j] + 1];
                    this.pic_match_flag[i][j] = new byte[num_redundant_pics_minus1[i][j] + 1];
                    this.mb_type_match_flag[i][j] = new byte[num_redundant_pics_minus1[i][j] + 1];
                    this.motion_match_flag[i][j] = new byte[num_redundant_pics_minus1[i][j] + 1];
                    this.residual_match_flag[i][j] = new byte[num_redundant_pics_minus1[i][j] + 1];
                    this.intra_samples_match_flag[i][j] = new byte[num_redundant_pics_minus1[i][j] + 1];
                    for (k = 0; k <= num_redundant_pics_minus1[i][j]; k++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.redundant_pic_cnt_minus1[i][j][k]);
                        size += stream.ReadUnsignedInt(size, 1, out this.pic_match_flag[i][j][k]);

                        if (pic_match_flag[i][j][k] == 0)
                        {
                            size += stream.ReadUnsignedInt(size, 1, out this.mb_type_match_flag[i][j][k]);
                            size += stream.ReadUnsignedInt(size, 1, out this.motion_match_flag[i][j][k]);
                            size += stream.ReadUnsignedInt(size, 1, out this.residual_match_flag[i][j][k]);
                            size += stream.ReadUnsignedInt(size, 1, out this.intra_samples_match_flag[i][j][k]);
                        }
                    }
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            uint k = 0;
            size += stream.WriteUnsignedIntGolomb(this.num_dIds_minus1);

            for (i = 0; i <= num_dIds_minus1; i++)
            {
                size += stream.WriteUnsignedInt(3, this.dependency_id[i]);
                size += stream.WriteUnsignedIntGolomb(this.num_qIds_minus1[i]);

                for (j = 0; j <= num_qIds_minus1[i]; j++)
                {
                    size += stream.WriteUnsignedInt(4, this.quality_id[i][j]);
                    size += stream.WriteUnsignedIntGolomb(this.num_redundant_pics_minus1[i][j]);

                    for (k = 0; k <= num_redundant_pics_minus1[i][j]; k++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.redundant_pic_cnt_minus1[i][j][k]);
                        size += stream.WriteUnsignedInt(1, this.pic_match_flag[i][j][k]);

                        if (pic_match_flag[i][j][k] == 0)
                        {
                            size += stream.WriteUnsignedInt(1, this.mb_type_match_flag[i][j][k]);
                            size += stream.WriteUnsignedInt(1, this.motion_match_flag[i][j][k]);
                            size += stream.WriteUnsignedInt(1, this.residual_match_flag[i][j][k]);
                            size += stream.WriteUnsignedInt(1, this.intra_samples_match_flag[i][j][k]);
                        }
                    }
                }
            }

            return size;
        }

    }

    /*
  

tl0_dep_rep_index( payloadSize ) {  
 tl0_dep_rep_idx 5 u(8) 
 effective_idr_pic_id 5 u(16) 
}
    */
    public class Tl0DepRepIndex : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint tl0_dep_rep_idx;
        public uint Tl0DepRepIdx { get { return tl0_dep_rep_idx; } set { tl0_dep_rep_idx = value; } }
        private uint effective_idr_pic_id;
        public uint EffectiveIdrPicId { get { return effective_idr_pic_id; } set { effective_idr_pic_id = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public Tl0DepRepIndex(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 8, out this.tl0_dep_rep_idx);
            size += stream.ReadUnsignedInt(size, 16, out this.effective_idr_pic_id);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(8, this.tl0_dep_rep_idx);
            size += stream.WriteUnsignedInt(16, this.effective_idr_pic_id);

            return size;
        }

    }

    /*
   

tl_switching_point( payloadSize ) { 
delta_frame_num 5 se(v)
}
    */
    public class TlSwitchingPoint : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private int delta_frame_num;
        public int DeltaFrameNum { get { return delta_frame_num; } set { delta_frame_num = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public TlSwitchingPoint(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadSignedIntGolomb(size, out this.delta_frame_num);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteSignedIntGolomb(this.delta_frame_num);

            return size;
        }

    }

    /*
   

svc_vui_parameters_extension() {  
 vui_ext_num_entries_minus1 0 ue(v) 
 for( i = 0; i <= vui_ext_num_entries_minus1; i++ ) {   
  vui_ext_dependency_id[ i ] 0 u(3) 
  vui_ext_quality_id[ i ] 0 u(4) 
  vui_ext_temporal_id[ i ] 0 u(3) 
  vui_ext_timing_info_present_flag[ i ] 0 u(1) 
  if( vui_ext_timing_info_present_flag[ i ] ) {   
   vui_ext_num_units_in_tick[ i ] 0 u(32) 
   vui_ext_time_scale[ i ] 0 u(32) 
   vui_ext_fixed_frame_rate_flag[ i ] 0 u(1) 
  }   
  vui_ext_nal_hrd_parameters_present_flag[ i ] 0 u(1) 
  if( vui_ext_nal_hrd_parameters_present_flag[ i ] )   
   hrd_parameters() 0  
  vui_ext_vcl_hrd_parameters_present_flag[ i ] 0 u(1) 
  if( vui_ext_vcl_hrd_parameters_present_flag[ i ] )   
   hrd_parameters() 0  
  if( vui_ext_nal_hrd_parameters_present_flag[ i ]  ||   
   vui_ext_vcl_hrd_parameters_present_flag[ i ] ) 
  
   vui_ext_low_delay_hrd_flag[ i ] 0 u(1) 
  vui_ext_pic_struct_present_flag[ i ]  0 u(1) 
 }   
}
    */
    public class SvcVuiParametersExtension : IItuSerializable
    {
        private uint vui_ext_num_entries_minus1;
        public uint VuiExtNumEntriesMinus1 { get { return vui_ext_num_entries_minus1; } set { vui_ext_num_entries_minus1 = value; } }
        private uint[] vui_ext_dependency_id;
        public uint[] VuiExtDependencyId { get { return vui_ext_dependency_id; } set { vui_ext_dependency_id = value; } }
        private uint[] vui_ext_quality_id;
        public uint[] VuiExtQualityId { get { return vui_ext_quality_id; } set { vui_ext_quality_id = value; } }
        private uint[] vui_ext_temporal_id;
        public uint[] VuiExtTemporalId { get { return vui_ext_temporal_id; } set { vui_ext_temporal_id = value; } }
        private byte[] vui_ext_timing_info_present_flag;
        public byte[] VuiExtTimingInfoPresentFlag { get { return vui_ext_timing_info_present_flag; } set { vui_ext_timing_info_present_flag = value; } }
        private uint[] vui_ext_num_units_in_tick;
        public uint[] VuiExtNumUnitsInTick { get { return vui_ext_num_units_in_tick; } set { vui_ext_num_units_in_tick = value; } }
        private uint[] vui_ext_time_scale;
        public uint[] VuiExtTimeScale { get { return vui_ext_time_scale; } set { vui_ext_time_scale = value; } }
        private byte[] vui_ext_fixed_frame_rate_flag;
        public byte[] VuiExtFixedFrameRateFlag { get { return vui_ext_fixed_frame_rate_flag; } set { vui_ext_fixed_frame_rate_flag = value; } }
        private byte[] vui_ext_nal_hrd_parameters_present_flag;
        public byte[] VuiExtNalHrdParametersPresentFlag { get { return vui_ext_nal_hrd_parameters_present_flag; } set { vui_ext_nal_hrd_parameters_present_flag = value; } }
        private HrdParameters[] hrd_parameters;
        public HrdParameters[] HrdParameters { get { return hrd_parameters; } set { hrd_parameters = value; } }
        private byte[] vui_ext_vcl_hrd_parameters_present_flag;
        public byte[] VuiExtVclHrdParametersPresentFlag { get { return vui_ext_vcl_hrd_parameters_present_flag; } set { vui_ext_vcl_hrd_parameters_present_flag = value; } }
        private byte[] vui_ext_low_delay_hrd_flag;
        public byte[] VuiExtLowDelayHrdFlag { get { return vui_ext_low_delay_hrd_flag; } set { vui_ext_low_delay_hrd_flag = value; } }
        private byte[] vui_ext_pic_struct_present_flag;
        public byte[] VuiExtPicStructPresentFlag { get { return vui_ext_pic_struct_present_flag; } set { vui_ext_pic_struct_present_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SvcVuiParametersExtension()
        {

        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.vui_ext_num_entries_minus1);

            this.vui_ext_dependency_id = new uint[vui_ext_num_entries_minus1 + 1];
            this.vui_ext_quality_id = new uint[vui_ext_num_entries_minus1 + 1];
            this.vui_ext_temporal_id = new uint[vui_ext_num_entries_minus1 + 1];
            this.vui_ext_timing_info_present_flag = new byte[vui_ext_num_entries_minus1 + 1];
            this.vui_ext_num_units_in_tick = new uint[vui_ext_num_entries_minus1 + 1];
            this.vui_ext_time_scale = new uint[vui_ext_num_entries_minus1 + 1];
            this.vui_ext_fixed_frame_rate_flag = new byte[vui_ext_num_entries_minus1 + 1];
            this.vui_ext_nal_hrd_parameters_present_flag = new byte[vui_ext_num_entries_minus1 + 1];
            this.hrd_parameters = new HrdParameters[vui_ext_num_entries_minus1 + 1];
            this.vui_ext_vcl_hrd_parameters_present_flag = new byte[vui_ext_num_entries_minus1 + 1];
            this.vui_ext_low_delay_hrd_flag = new byte[vui_ext_num_entries_minus1 + 1];
            this.vui_ext_pic_struct_present_flag = new byte[vui_ext_num_entries_minus1 + 1];
            for (i = 0; i <= vui_ext_num_entries_minus1; i++)
            {
                size += stream.ReadUnsignedInt(size, 3, out this.vui_ext_dependency_id[i]);
                size += stream.ReadUnsignedInt(size, 4, out this.vui_ext_quality_id[i]);
                size += stream.ReadUnsignedInt(size, 3, out this.vui_ext_temporal_id[i]);
                size += stream.ReadUnsignedInt(size, 1, out this.vui_ext_timing_info_present_flag[i]);

                if (vui_ext_timing_info_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 32, out this.vui_ext_num_units_in_tick[i]);
                    size += stream.ReadUnsignedInt(size, 32, out this.vui_ext_time_scale[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.vui_ext_fixed_frame_rate_flag[i]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.vui_ext_nal_hrd_parameters_present_flag[i]);

                if (vui_ext_nal_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.ReadClass<HrdParameters>(size, context, () => new HrdParameters(), out this.hrd_parameters[i]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.vui_ext_vcl_hrd_parameters_present_flag[i]);

                if (vui_ext_vcl_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.ReadClass<HrdParameters>(size, context, () => new HrdParameters(), out this.hrd_parameters[i]);
                }

                if (vui_ext_nal_hrd_parameters_present_flag[i] != 0 ||
   vui_ext_vcl_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.vui_ext_low_delay_hrd_flag[i]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.vui_ext_pic_struct_present_flag[i]);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedIntGolomb(this.vui_ext_num_entries_minus1);

            for (i = 0; i <= vui_ext_num_entries_minus1; i++)
            {
                size += stream.WriteUnsignedInt(3, this.vui_ext_dependency_id[i]);
                size += stream.WriteUnsignedInt(4, this.vui_ext_quality_id[i]);
                size += stream.WriteUnsignedInt(3, this.vui_ext_temporal_id[i]);
                size += stream.WriteUnsignedInt(1, this.vui_ext_timing_info_present_flag[i]);

                if (vui_ext_timing_info_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(32, this.vui_ext_num_units_in_tick[i]);
                    size += stream.WriteUnsignedInt(32, this.vui_ext_time_scale[i]);
                    size += stream.WriteUnsignedInt(1, this.vui_ext_fixed_frame_rate_flag[i]);
                }
                size += stream.WriteUnsignedInt(1, this.vui_ext_nal_hrd_parameters_present_flag[i]);

                if (vui_ext_nal_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.WriteClass<HrdParameters>(context, this.hrd_parameters[i]);
                }
                size += stream.WriteUnsignedInt(1, this.vui_ext_vcl_hrd_parameters_present_flag[i]);

                if (vui_ext_vcl_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.WriteClass<HrdParameters>(context, this.hrd_parameters[i]);
                }

                if (vui_ext_nal_hrd_parameters_present_flag[i] != 0 ||
   vui_ext_vcl_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.vui_ext_low_delay_hrd_flag[i]);
                }
                size += stream.WriteUnsignedInt(1, this.vui_ext_pic_struct_present_flag[i]);
            }

            return size;
        }

    }

    /*
   

nal_unit_header_mvc_extension() { 
non_idr_flag All u(1) 
priority_id All u(6) 
view_id All u(10) 
temporal_id All u(3) 
anchor_pic_flag All u(1) 
inter_view_flag All u(1) 
reserved_one_bit All u(1) 
}
    */
    public class NalUnitHeaderMvcExtension : IItuSerializable
    {
        private byte non_idr_flag;
        public byte NonIdrFlag { get { return non_idr_flag; } set { non_idr_flag = value; } }
        private uint priority_id;
        public uint PriorityId { get { return priority_id; } set { priority_id = value; } }
        private uint view_id;
        public uint ViewId { get { return view_id; } set { view_id = value; } }
        private uint temporal_id;
        public uint TemporalId { get { return temporal_id; } set { temporal_id = value; } }
        private byte anchor_pic_flag;
        public byte AnchorPicFlag { get { return anchor_pic_flag; } set { anchor_pic_flag = value; } }
        private byte inter_view_flag;
        public byte InterViewFlag { get { return inter_view_flag; } set { inter_view_flag = value; } }
        private byte reserved_one_bit;
        public byte ReservedOneBit { get { return reserved_one_bit; } set { reserved_one_bit = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public NalUnitHeaderMvcExtension()
        {

        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.non_idr_flag);
            size += stream.ReadUnsignedInt(size, 6, out this.priority_id);
            size += stream.ReadUnsignedInt(size, 10, out this.view_id);
            size += stream.ReadUnsignedInt(size, 3, out this.temporal_id);
            size += stream.ReadUnsignedInt(size, 1, out this.anchor_pic_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.inter_view_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.reserved_one_bit);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.non_idr_flag);
            size += stream.WriteUnsignedInt(6, this.priority_id);
            size += stream.WriteUnsignedInt(10, this.view_id);
            size += stream.WriteUnsignedInt(3, this.temporal_id);
            size += stream.WriteUnsignedInt(1, this.anchor_pic_flag);
            size += stream.WriteUnsignedInt(1, this.inter_view_flag);
            size += stream.WriteUnsignedInt(1, this.reserved_one_bit);

            return size;
        }

    }

    /*
   

seq_parameter_set_mvc_extension() {  
 num_views_minus1 0 ue(v) 
 for( i = 0; i <= num_views_minus1; i++ )    
  view_id[ i ] 0 ue(v) 
 for( i = 1; i <= num_views_minus1; i++ ) {   
  num_anchor_refs_l0[ i ] 0 ue(v) 
  for( j = 0; j < num_anchor_refs_l0[ i ]; j++ )   
   anchor_ref_l0[ i ][ j ] 0 ue(v) 
  num_anchor_refs_l1[ i ] 0 ue(v) 
  for( j = 0; j < num_anchor_refs_l1[ i ]; j++ )   
   anchor_ref_l1[ i ][ j ] 0 ue(v) 
 }   
 for( i = 1; i <= num_views_minus1; i++ ) {   
  num_non_anchor_refs_l0[ i ] 0 ue(v) 
  for( j = 0; j < num_non_anchor_refs_l0[ i ]; j++ )   
   non_anchor_ref_l0[ i ][ j ] 0 ue(v) 
  num_non_anchor_refs_l1[ i ] 0 ue(v) 
  for( j = 0; j < num_non_anchor_refs_l1[ i ]; j++ )   
   non_anchor_ref_l1[ i ][ j ] 0 ue(v) 
 }   
 num_level_values_signalled_minus1 0 ue(v) 
 for( i = 0; i <= num_level_values_signalled_minus1; i++ ) {   
  level_idc[ i ] 0 u(8) 
  num_applicable_ops_minus1[ i ] 0 ue(v) 
  for( j = 0; j <= num_applicable_ops_minus1[ i ]; j++ ) {   
   applicable_op_temporal_id[ i ][ j ] 0 u(3) 
   applicable_op_num_target_views_minus1[ i ][ j ] 0 ue(v) 
   for( k = 0; k <= applicable_op_num_target_views_minus1[ i ][ j ]; k++ )   
    applicable_op_target_view_id[ i ][ j ][ k ] 0 ue(v) 
   applicable_op_num_views_minus1[ i ][ j ] 0 ue(v) 
  }   
 }   
 if( profile_idc  ==  134 ) {   
  mfc_format_idc 0 u(6) 
  if( mfc_format_idc  ==  0  ||  mfc_format_idc  ==  1 ) {   
   default_grid_position_flag 0 u(1) 
   if( !default_grid_position_flag ) {   
    view0_grid_position_x 0 u(4) 
    view0_grid_position_y 0 u(4) 
    view1_grid_position_x 0 u(4) 
    view1_grid_position_y 0 u(4) 
   }   
  }   
  rpu_filter_enabled_flag 0 u(1) 
  if( !frame_mbs_only_flag )    
   rpu_field_processing_flag 0 u(1) 
 }   
}
    */
    public class SeqParameterSetMvcExtension : IItuSerializable
    {
        private uint num_views_minus1;
        public uint NumViewsMinus1 { get { return num_views_minus1; } set { num_views_minus1 = value; } }
        private uint[] view_id;
        public uint[] ViewId { get { return view_id; } set { view_id = value; } }
        private uint[] num_anchor_refs_l0;
        public uint[] NumAnchorRefsL0 { get { return num_anchor_refs_l0; } set { num_anchor_refs_l0 = value; } }
        private uint[][] anchor_ref_l0;
        public uint[][] AnchorRefL0 { get { return anchor_ref_l0; } set { anchor_ref_l0 = value; } }
        private uint[] num_anchor_refs_l1;
        public uint[] NumAnchorRefsL1 { get { return num_anchor_refs_l1; } set { num_anchor_refs_l1 = value; } }
        private uint[][] anchor_ref_l1;
        public uint[][] AnchorRefL1 { get { return anchor_ref_l1; } set { anchor_ref_l1 = value; } }
        private uint[] num_non_anchor_refs_l0;
        public uint[] NumNonAnchorRefsL0 { get { return num_non_anchor_refs_l0; } set { num_non_anchor_refs_l0 = value; } }
        private uint[][] non_anchor_ref_l0;
        public uint[][] NonAnchorRefL0 { get { return non_anchor_ref_l0; } set { non_anchor_ref_l0 = value; } }
        private uint[] num_non_anchor_refs_l1;
        public uint[] NumNonAnchorRefsL1 { get { return num_non_anchor_refs_l1; } set { num_non_anchor_refs_l1 = value; } }
        private uint[][] non_anchor_ref_l1;
        public uint[][] NonAnchorRefL1 { get { return non_anchor_ref_l1; } set { non_anchor_ref_l1 = value; } }
        private uint num_level_values_signalled_minus1;
        public uint NumLevelValuesSignalledMinus1 { get { return num_level_values_signalled_minus1; } set { num_level_values_signalled_minus1 = value; } }
        private uint[] level_idc;
        public uint[] LevelIdc { get { return level_idc; } set { level_idc = value; } }
        private uint[] num_applicable_ops_minus1;
        public uint[] NumApplicableOpsMinus1 { get { return num_applicable_ops_minus1; } set { num_applicable_ops_minus1 = value; } }
        private uint[][] applicable_op_temporal_id;
        public uint[][] ApplicableOpTemporalId { get { return applicable_op_temporal_id; } set { applicable_op_temporal_id = value; } }
        private uint[][] applicable_op_num_target_views_minus1;
        public uint[][] ApplicableOpNumTargetViewsMinus1 { get { return applicable_op_num_target_views_minus1; } set { applicable_op_num_target_views_minus1 = value; } }
        private uint[][][] applicable_op_target_view_id;
        public uint[][][] ApplicableOpTargetViewId { get { return applicable_op_target_view_id; } set { applicable_op_target_view_id = value; } }
        private uint[][] applicable_op_num_views_minus1;
        public uint[][] ApplicableOpNumViewsMinus1 { get { return applicable_op_num_views_minus1; } set { applicable_op_num_views_minus1 = value; } }
        private uint mfc_format_idc;
        public uint MfcFormatIdc { get { return mfc_format_idc; } set { mfc_format_idc = value; } }
        private byte default_grid_position_flag;
        public byte DefaultGridPositionFlag { get { return default_grid_position_flag; } set { default_grid_position_flag = value; } }
        private uint view0_grid_position_x;
        public uint View0GridPositionx { get { return view0_grid_position_x; } set { view0_grid_position_x = value; } }
        private uint view0_grid_position_y;
        public uint View0GridPositiony { get { return view0_grid_position_y; } set { view0_grid_position_y = value; } }
        private uint view1_grid_position_x;
        public uint View1GridPositionx { get { return view1_grid_position_x; } set { view1_grid_position_x = value; } }
        private uint view1_grid_position_y;
        public uint View1GridPositiony { get { return view1_grid_position_y; } set { view1_grid_position_y = value; } }
        private byte rpu_filter_enabled_flag;
        public byte RpuFilterEnabledFlag { get { return rpu_filter_enabled_flag; } set { rpu_filter_enabled_flag = value; } }
        private byte rpu_field_processing_flag;
        public byte RpuFieldProcessingFlag { get { return rpu_field_processing_flag; } set { rpu_field_processing_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SeqParameterSetMvcExtension()
        {

        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            uint k = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.num_views_minus1);

            this.view_id = new uint[num_views_minus1 + 1];
            for (i = 0; i <= num_views_minus1; i++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.view_id[i]);
            }

            this.num_anchor_refs_l0 = new uint[num_views_minus1 + 1];
            this.anchor_ref_l0 = new uint[num_views_minus1 + 1][];
            this.num_anchor_refs_l1 = new uint[num_views_minus1 + 1];
            this.anchor_ref_l1 = new uint[num_views_minus1 + 1][];
            for (i = 1; i <= num_views_minus1; i++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.num_anchor_refs_l0[i]);

                this.anchor_ref_l0[i] = new uint[num_anchor_refs_l0[i]];
                for (j = 0; j < num_anchor_refs_l0[i]; j++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.anchor_ref_l0[i][j]);
                }
                size += stream.ReadUnsignedIntGolomb(size, out this.num_anchor_refs_l1[i]);

                this.anchor_ref_l1[i] = new uint[num_anchor_refs_l1[i]];
                for (j = 0; j < num_anchor_refs_l1[i]; j++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.anchor_ref_l1[i][j]);
                }
            }

            this.num_non_anchor_refs_l0 = new uint[num_views_minus1 + 1];
            this.non_anchor_ref_l0 = new uint[num_views_minus1 + 1][];
            this.num_non_anchor_refs_l1 = new uint[num_views_minus1 + 1];
            this.non_anchor_ref_l1 = new uint[num_views_minus1 + 1][];
            for (i = 1; i <= num_views_minus1; i++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.num_non_anchor_refs_l0[i]);

                this.non_anchor_ref_l0[i] = new uint[num_non_anchor_refs_l0[i]];
                for (j = 0; j < num_non_anchor_refs_l0[i]; j++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.non_anchor_ref_l0[i][j]);
                }
                size += stream.ReadUnsignedIntGolomb(size, out this.num_non_anchor_refs_l1[i]);

                this.non_anchor_ref_l1[i] = new uint[num_non_anchor_refs_l1[i]];
                for (j = 0; j < num_non_anchor_refs_l1[i]; j++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.non_anchor_ref_l1[i][j]);
                }
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.num_level_values_signalled_minus1);

            this.level_idc = new uint[num_level_values_signalled_minus1 + 1];
            this.num_applicable_ops_minus1 = new uint[num_level_values_signalled_minus1 + 1];
            this.applicable_op_temporal_id = new uint[num_level_values_signalled_minus1 + 1][];
            this.applicable_op_num_target_views_minus1 = new uint[num_level_values_signalled_minus1 + 1][];
            this.applicable_op_target_view_id = new uint[num_level_values_signalled_minus1 + 1][][];
            this.applicable_op_num_views_minus1 = new uint[num_level_values_signalled_minus1 + 1][];
            for (i = 0; i <= num_level_values_signalled_minus1; i++)
            {
                size += stream.ReadUnsignedInt(size, 8, out this.level_idc[i]);
                size += stream.ReadUnsignedIntGolomb(size, out this.num_applicable_ops_minus1[i]);

                this.applicable_op_temporal_id[i] = new uint[num_applicable_ops_minus1[i] + 1];
                this.applicable_op_num_target_views_minus1[i] = new uint[num_applicable_ops_minus1[i] + 1];
                this.applicable_op_target_view_id[i] = new uint[num_applicable_ops_minus1[i] + 1][];
                this.applicable_op_num_views_minus1[i] = new uint[num_applicable_ops_minus1[i] + 1];
                for (j = 0; j <= num_applicable_ops_minus1[i]; j++)
                {
                    size += stream.ReadUnsignedInt(size, 3, out this.applicable_op_temporal_id[i][j]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.applicable_op_num_target_views_minus1[i][j]);

                    this.applicable_op_target_view_id[i][j] = new uint[applicable_op_num_target_views_minus1[i][j] + 1];
                    for (k = 0; k <= applicable_op_num_target_views_minus1[i][j]; k++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.applicable_op_target_view_id[i][j][k]);
                    }
                    size += stream.ReadUnsignedIntGolomb(size, out this.applicable_op_num_views_minus1[i][j]);
                }
            }

            if (context.Sps.SeqParameterSetData.ProfileIdc == 134)
            {
                size += stream.ReadUnsignedInt(size, 6, out this.mfc_format_idc);

                if (mfc_format_idc == 0 || mfc_format_idc == 1)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.default_grid_position_flag);

                    if (default_grid_position_flag == 0)
                    {
                        size += stream.ReadUnsignedInt(size, 4, out this.view0_grid_position_x);
                        size += stream.ReadUnsignedInt(size, 4, out this.view0_grid_position_y);
                        size += stream.ReadUnsignedInt(size, 4, out this.view1_grid_position_x);
                        size += stream.ReadUnsignedInt(size, 4, out this.view1_grid_position_y);
                    }
                }
                size += stream.ReadUnsignedInt(size, 1, out this.rpu_filter_enabled_flag);

                if (context.Sps.SeqParameterSetData.FrameMbsOnlyFlag == 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.rpu_field_processing_flag);
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            uint k = 0;
            size += stream.WriteUnsignedIntGolomb(this.num_views_minus1);

            for (i = 0; i <= num_views_minus1; i++)
            {
                size += stream.WriteUnsignedIntGolomb(this.view_id[i]);
            }

            for (i = 1; i <= num_views_minus1; i++)
            {
                size += stream.WriteUnsignedIntGolomb(this.num_anchor_refs_l0[i]);

                for (j = 0; j < num_anchor_refs_l0[i]; j++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.anchor_ref_l0[i][j]);
                }
                size += stream.WriteUnsignedIntGolomb(this.num_anchor_refs_l1[i]);

                for (j = 0; j < num_anchor_refs_l1[i]; j++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.anchor_ref_l1[i][j]);
                }
            }

            for (i = 1; i <= num_views_minus1; i++)
            {
                size += stream.WriteUnsignedIntGolomb(this.num_non_anchor_refs_l0[i]);

                for (j = 0; j < num_non_anchor_refs_l0[i]; j++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.non_anchor_ref_l0[i][j]);
                }
                size += stream.WriteUnsignedIntGolomb(this.num_non_anchor_refs_l1[i]);

                for (j = 0; j < num_non_anchor_refs_l1[i]; j++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.non_anchor_ref_l1[i][j]);
                }
            }
            size += stream.WriteUnsignedIntGolomb(this.num_level_values_signalled_minus1);

            for (i = 0; i <= num_level_values_signalled_minus1; i++)
            {
                size += stream.WriteUnsignedInt(8, this.level_idc[i]);
                size += stream.WriteUnsignedIntGolomb(this.num_applicable_ops_minus1[i]);

                for (j = 0; j <= num_applicable_ops_minus1[i]; j++)
                {
                    size += stream.WriteUnsignedInt(3, this.applicable_op_temporal_id[i][j]);
                    size += stream.WriteUnsignedIntGolomb(this.applicable_op_num_target_views_minus1[i][j]);

                    for (k = 0; k <= applicable_op_num_target_views_minus1[i][j]; k++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.applicable_op_target_view_id[i][j][k]);
                    }
                    size += stream.WriteUnsignedIntGolomb(this.applicable_op_num_views_minus1[i][j]);
                }
            }

            if (context.Sps.SeqParameterSetData.ProfileIdc == 134)
            {
                size += stream.WriteUnsignedInt(6, this.mfc_format_idc);

                if (mfc_format_idc == 0 || mfc_format_idc == 1)
                {
                    size += stream.WriteUnsignedInt(1, this.default_grid_position_flag);

                    if (default_grid_position_flag == 0)
                    {
                        size += stream.WriteUnsignedInt(4, this.view0_grid_position_x);
                        size += stream.WriteUnsignedInt(4, this.view0_grid_position_y);
                        size += stream.WriteUnsignedInt(4, this.view1_grid_position_x);
                        size += stream.WriteUnsignedInt(4, this.view1_grid_position_y);
                    }
                }
                size += stream.WriteUnsignedInt(1, this.rpu_filter_enabled_flag);

                if (context.Sps.SeqParameterSetData.FrameMbsOnlyFlag == 0)
                {
                    size += stream.WriteUnsignedInt(1, this.rpu_field_processing_flag);
                }
            }

            return size;
        }

    }

    /*
  

parallel_decoding_info( payloadSize ) {  
 seq_parameter_set_id 5 ue(v) 
 for( i = 1; i <= num_views_minus1; i++ ) {   
  if( anchor_pic_flag ) {   
   for( j = 0; j <= num_anchor_refs_l0[ i ]; j++ )    
    pdi_init_delay_anchor_minus2_l0[ i ][ j ] 5 ue(v) 
   for( j = 0; j <= num_anchor_refs_l1[ i ]; j++ )    
    pdi_init_delay_anchor_minus2_l1[ i ][ j ] 5 ue(v) 
  }   
  else {   
   for( j = 0; j <= num_non_anchor_refs_l0[ i ]; j++ )    
    pdi_init_delay_non_anchor_minus2_l0[ i ][ j ] 5 ue(v) 
   for( j = 0; j <= num_non_anchor_refs_l1[ i ]; j++ )    
    pdi_init_delay_non_anchor_minus2_l1[ i ][ j ] 5 ue(v) 
  }   
 }   
}
    */
    public class ParallelDecodingInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint seq_parameter_set_id;
        public uint SeqParameterSetId { get { return seq_parameter_set_id; } set { seq_parameter_set_id = value; } }
        private uint[][] pdi_init_delay_anchor_minus2_l0;
        public uint[][] PdiInitDelayAnchorMinus2L0 { get { return pdi_init_delay_anchor_minus2_l0; } set { pdi_init_delay_anchor_minus2_l0 = value; } }
        private uint[][] pdi_init_delay_anchor_minus2_l1;
        public uint[][] PdiInitDelayAnchorMinus2L1 { get { return pdi_init_delay_anchor_minus2_l1; } set { pdi_init_delay_anchor_minus2_l1 = value; } }
        private uint[][] pdi_init_delay_non_anchor_minus2_l0;
        public uint[][] PdiInitDelayNonAnchorMinus2L0 { get { return pdi_init_delay_non_anchor_minus2_l0; } set { pdi_init_delay_non_anchor_minus2_l0 = value; } }
        private uint[][] pdi_init_delay_non_anchor_minus2_l1;
        public uint[][] PdiInitDelayNonAnchorMinus2L1 { get { return pdi_init_delay_non_anchor_minus2_l1; } set { pdi_init_delay_non_anchor_minus2_l1 = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ParallelDecodingInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.seq_parameter_set_id);

            this.pdi_init_delay_anchor_minus2_l0 = new uint[context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1 + 1][];
            this.pdi_init_delay_anchor_minus2_l1 = new uint[context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1 + 1][];
            this.pdi_init_delay_non_anchor_minus2_l0 = new uint[context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1 + 1][];
            this.pdi_init_delay_non_anchor_minus2_l1 = new uint[context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1 + 1][];
            for (i = 1; i <= context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1; i++)
            {

                if (context.NalHeader.NalUnitHeaderMvcExtension.AnchorPicFlag != 0)
                {

                    this.pdi_init_delay_anchor_minus2_l0[i] = new uint[context.SubsetSps.SeqParameterSetMvcExtension.NumAnchorRefsL0[i]];
                    for (j = 0; j <= context.SubsetSps.SeqParameterSetMvcExtension.NumAnchorRefsL0[i]; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.pdi_init_delay_anchor_minus2_l0[i][j]);
                    }

                    this.pdi_init_delay_anchor_minus2_l1[i] = new uint[context.SubsetSps.SeqParameterSetMvcExtension.NumAnchorRefsL1[i]];
                    for (j = 0; j <= context.SubsetSps.SeqParameterSetMvcExtension.NumAnchorRefsL1[i]; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.pdi_init_delay_anchor_minus2_l1[i][j]);
                    }
                }
                else
                {

                    this.pdi_init_delay_non_anchor_minus2_l0[i] = new uint[context.SubsetSps.SeqParameterSetMvcExtension.NumNonAnchorRefsL0[i]];
                    for (j = 0; j <= context.SubsetSps.SeqParameterSetMvcExtension.NumNonAnchorRefsL0[i]; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.pdi_init_delay_non_anchor_minus2_l0[i][j]);
                    }

                    this.pdi_init_delay_non_anchor_minus2_l1[i] = new uint[context.SubsetSps.SeqParameterSetMvcExtension.NumNonAnchorRefsL1[i]];
                    for (j = 0; j <= context.SubsetSps.SeqParameterSetMvcExtension.NumNonAnchorRefsL1[i]; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.pdi_init_delay_non_anchor_minus2_l1[i][j]);
                    }
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.WriteUnsignedIntGolomb(this.seq_parameter_set_id);

            for (i = 1; i <= context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1; i++)
            {

                if (context.NalHeader.NalUnitHeaderMvcExtension.AnchorPicFlag != 0)
                {

                    for (j = 0; j <= context.SubsetSps.SeqParameterSetMvcExtension.NumAnchorRefsL0[i]; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.pdi_init_delay_anchor_minus2_l0[i][j]);
                    }

                    for (j = 0; j <= context.SubsetSps.SeqParameterSetMvcExtension.NumAnchorRefsL1[i]; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.pdi_init_delay_anchor_minus2_l1[i][j]);
                    }
                }
                else
                {

                    for (j = 0; j <= context.SubsetSps.SeqParameterSetMvcExtension.NumNonAnchorRefsL0[i]; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.pdi_init_delay_non_anchor_minus2_l0[i][j]);
                    }

                    for (j = 0; j <= context.SubsetSps.SeqParameterSetMvcExtension.NumNonAnchorRefsL1[i]; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.pdi_init_delay_non_anchor_minus2_l1[i][j]);
                    }
                }
            }

            return size;
        }

    }

    /*
   

mvc_scalable_nesting( payloadSize ) {  
 operation_point_flag 5 u(1) 
 if( !operation_point_flag ) {   
  all_view_components_in_au_flag 5 u(1) 
  if( !all_view_components_in_au_flag ) {   
   num_view_components_minus1 5 ue(v) 
   for( i = 0; i <= num_view_components_minus1; i++ )   
    sei_view_id[ i ] 5 u(10) 
  }   
 } else {   
  num_view_components_op_minus1 5 ue(v) 
  for( i = 0; i <= num_view_components_op_minus1; i++ )   
   sei_op_view_id[ i ] 5 u(10) 
  sei_op_temporal_id 5 u(3) 
 }   
 while( !byte_aligned() )  
   sei_nesting_zero_bit /* equal to 0 *//* 5 f(1) 
 sei_message() 5  
}
    */
    public class MvcScalableNesting : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte operation_point_flag;
        public byte OperationPointFlag { get { return operation_point_flag; } set { operation_point_flag = value; } }
        private byte all_view_components_in_au_flag;
        public byte AllViewComponentsInAuFlag { get { return all_view_components_in_au_flag; } set { all_view_components_in_au_flag = value; } }
        private uint num_view_components_minus1;
        public uint NumViewComponentsMinus1 { get { return num_view_components_minus1; } set { num_view_components_minus1 = value; } }
        private uint[] sei_view_id;
        public uint[] SeiViewId { get { return sei_view_id; } set { sei_view_id = value; } }
        private uint num_view_components_op_minus1;
        public uint NumViewComponentsOpMinus1 { get { return num_view_components_op_minus1; } set { num_view_components_op_minus1 = value; } }
        private uint[] sei_op_view_id;
        public uint[] SeiOpViewId { get { return sei_op_view_id; } set { sei_op_view_id = value; } }
        private uint sei_op_temporal_id;
        public uint SeiOpTemporalId { get { return sei_op_temporal_id; } set { sei_op_temporal_id = value; } }
        private uint sei_nesting_zero_bit;
        public uint SeiNestingZeroBit { get { return sei_nesting_zero_bit; } set { sei_nesting_zero_bit = value; } }
        private SeiMessage sei_message;
        public SeiMessage SeiMessage { get { return sei_message; } set { sei_message = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public MvcScalableNesting(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.operation_point_flag);

            if (operation_point_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.all_view_components_in_au_flag);

                if (all_view_components_in_au_flag == 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_view_components_minus1);

                    this.sei_view_id = new uint[num_view_components_minus1 + 1];
                    for (i = 0; i <= num_view_components_minus1; i++)
                    {
                        size += stream.ReadUnsignedInt(size, 10, out this.sei_view_id[i]);
                    }
                }
            }
            else
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.num_view_components_op_minus1);

                this.sei_op_view_id = new uint[num_view_components_op_minus1 + 1];
                for (i = 0; i <= num_view_components_op_minus1; i++)
                {
                    size += stream.ReadUnsignedInt(size, 10, out this.sei_op_view_id[i]);
                }
                size += stream.ReadUnsignedInt(size, 3, out this.sei_op_temporal_id);
            }

            while (!stream.ByteAligned())
            {
                size += stream.ReadFixed(size, 1, out this.sei_nesting_zero_bit); // equal to 0 
            }
            size += stream.ReadClass<SeiMessage>(size, context, () => new SeiMessage(), out this.sei_message);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedInt(1, this.operation_point_flag);

            if (operation_point_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.all_view_components_in_au_flag);

                if (all_view_components_in_au_flag == 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.num_view_components_minus1);

                    for (i = 0; i <= num_view_components_minus1; i++)
                    {
                        size += stream.WriteUnsignedInt(10, this.sei_view_id[i]);
                    }
                }
            }
            else
            {
                size += stream.WriteUnsignedIntGolomb(this.num_view_components_op_minus1);

                for (i = 0; i <= num_view_components_op_minus1; i++)
                {
                    size += stream.WriteUnsignedInt(10, this.sei_op_view_id[i]);
                }
                size += stream.WriteUnsignedInt(3, this.sei_op_temporal_id);
            }

            while (!stream.ByteAligned())
            {
                size += stream.WriteFixed(1, this.sei_nesting_zero_bit); // equal to 0 
            }
            size += stream.WriteClass<SeiMessage>(context, this.sei_message);

            return size;
        }

    }

    /*
   


view_scalability_info( payloadSize ) {  
 num_operation_points_minus1 5 ue(v) 
 for( i = 0; i <= num_operation_points_minus1; i++ ) {   
  operation_point_id[ i ] 5 ue(v) 
  priority_id[ i ] 5 u(5) 
  temporal_id[ i ] 5 u(3) 
  num_target_output_views_minus1[ i ] 5 ue(v) 
  for( j = 0; j <= num_target_output_views_minus1[ i ]; j++ )    
   view_id[ i ][ j ] 5 ue(v) 
  profile_level_info_present_flag[ i ] 5 u(1) 
  bitrate_info_present_flag[ i ] 5 u(1) 
  frm_rate_info_present_flag[ i ] 5 u(1) 
  if( !num_target_output_views_minus1[ i ] )    
   view_dependency_info_present_flag[ i ] 5 u(1) 
  parameter_sets_info_present_flag[ i ] 5 u(1) 
  bitstream_restriction_info_present_flag[ i ] 5 u(1) 
  if( profile_level_info_present_flag[ i ] )   
   op_profile_level_idc[ i ]  5 u(24) 
  if( bitrate_info_present_flag[ i ] ) {   
   avg_bitrate[ i ] 5 u(16) 
   max_bitrate[ i ] 5 u(16) 
   max_bitrate_calc_window[ i ] 5 u(16) 
  }   
  if( frm_rate_info_present_flag[ i ] ) {   
   constant_frm_rate_idc[ i ] 5 u(2) 
   avg_frm_rate[ i ] 5 u(16) 
  }   
  if( view_dependency_info_present_flag[ i ] ) {   
   num_directly_dependent_views[ i ] 5 ue(v) 
   for( j = 0; j < num_directly_dependent_views[ i ]; j++ )   
    directly_dependent_view_id[ i ][ j ] 5 ue(v) 
  } else   
   view_dependency_info_src_op_id[ i ] 5 ue(v) 
  if( parameter_sets_info_present_flag[ i ] ) {   
   num_seq_parameter_sets[ i ] 5 ue(v) 
   for( j = 0; j < num_seq_parameter_sets[ i ]; j++ )   
    seq_parameter_set_id_delta[ i ][ j ] 5 ue(v) 
   num_subset_seq_parameter_sets[ i ] 5 ue(v) 
   for( j = 0; j < num_subset_seq_parameter_sets[ i ]; j++ )   
    subset_seq_parameter_set_id_delta[ i ][ j ] 5 ue(v) 
   num_pic_parameter_sets_minus1[ i ] 5 ue(v)
      for( j = 0; j <= num_pic_parameter_sets_minus1[ i ]; j++ )   
    pic_parameter_set_id_delta[ i ][ j ] 5 ue(v) 
  } else   
   parameter_sets_info_src_op_id[ i ] 5 ue(v) 
  if( bitstream_restriction_info_present_flag[ i ] ) {   
   motion_vectors_over_pic_boundaries_flag[ i ] 5 u(1) 
   max_bytes_per_pic_denom[ i ] 5 ue(v) 
   max_bits_per_mb_denom[ i ] 5 ue(v) 
   log2_max_mv_length_horizontal[ i ] 5 ue(v) 
   log2_max_mv_length_vertical[ i ] 5 ue(v) 
   max_num_reorder_frames[ i ] 5 ue(v) 
   max_dec_frame_buffering[ i ] 5 ue(v) 
  }   
 }   
}
    */
    public class ViewScalabilityInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint num_operation_points_minus1;
        public uint NumOperationPointsMinus1 { get { return num_operation_points_minus1; } set { num_operation_points_minus1 = value; } }
        private uint[] operation_point_id;
        public uint[] OperationPointId { get { return operation_point_id; } set { operation_point_id = value; } }
        private uint[] priority_id;
        public uint[] PriorityId { get { return priority_id; } set { priority_id = value; } }
        private uint[] temporal_id;
        public uint[] TemporalId { get { return temporal_id; } set { temporal_id = value; } }
        private uint[] num_target_output_views_minus1;
        public uint[] NumTargetOutputViewsMinus1 { get { return num_target_output_views_minus1; } set { num_target_output_views_minus1 = value; } }
        private uint[][] view_id;
        public uint[][] ViewId { get { return view_id; } set { view_id = value; } }
        private byte[] profile_level_info_present_flag;
        public byte[] ProfileLevelInfoPresentFlag { get { return profile_level_info_present_flag; } set { profile_level_info_present_flag = value; } }
        private byte[] bitrate_info_present_flag;
        public byte[] BitrateInfoPresentFlag { get { return bitrate_info_present_flag; } set { bitrate_info_present_flag = value; } }
        private byte[] frm_rate_info_present_flag;
        public byte[] FrmRateInfoPresentFlag { get { return frm_rate_info_present_flag; } set { frm_rate_info_present_flag = value; } }
        private byte[] view_dependency_info_present_flag;
        public byte[] ViewDependencyInfoPresentFlag { get { return view_dependency_info_present_flag; } set { view_dependency_info_present_flag = value; } }
        private byte[] parameter_sets_info_present_flag;
        public byte[] ParameterSetsInfoPresentFlag { get { return parameter_sets_info_present_flag; } set { parameter_sets_info_present_flag = value; } }
        private byte[] bitstream_restriction_info_present_flag;
        public byte[] BitstreamRestrictionInfoPresentFlag { get { return bitstream_restriction_info_present_flag; } set { bitstream_restriction_info_present_flag = value; } }
        private uint[] op_profile_level_idc;
        public uint[] OpProfileLevelIdc { get { return op_profile_level_idc; } set { op_profile_level_idc = value; } }
        private uint[] avg_bitrate;
        public uint[] AvgBitrate { get { return avg_bitrate; } set { avg_bitrate = value; } }
        private uint[] max_bitrate;
        public uint[] MaxBitrate { get { return max_bitrate; } set { max_bitrate = value; } }
        private uint[] max_bitrate_calc_window;
        public uint[] MaxBitrateCalcWindow { get { return max_bitrate_calc_window; } set { max_bitrate_calc_window = value; } }
        private uint[] constant_frm_rate_idc;
        public uint[] ConstantFrmRateIdc { get { return constant_frm_rate_idc; } set { constant_frm_rate_idc = value; } }
        private uint[] avg_frm_rate;
        public uint[] AvgFrmRate { get { return avg_frm_rate; } set { avg_frm_rate = value; } }
        private uint[] num_directly_dependent_views;
        public uint[] NumDirectlyDependentViews { get { return num_directly_dependent_views; } set { num_directly_dependent_views = value; } }
        private uint[][] directly_dependent_view_id;
        public uint[][] DirectlyDependentViewId { get { return directly_dependent_view_id; } set { directly_dependent_view_id = value; } }
        private uint[] view_dependency_info_src_op_id;
        public uint[] ViewDependencyInfoSrcOpId { get { return view_dependency_info_src_op_id; } set { view_dependency_info_src_op_id = value; } }
        private uint[] num_seq_parameter_sets;
        public uint[] NumSeqParameterSets { get { return num_seq_parameter_sets; } set { num_seq_parameter_sets = value; } }
        private uint[][] seq_parameter_set_id_delta;
        public uint[][] SeqParameterSetIdDelta { get { return seq_parameter_set_id_delta; } set { seq_parameter_set_id_delta = value; } }
        private uint[] num_subset_seq_parameter_sets;
        public uint[] NumSubsetSeqParameterSets { get { return num_subset_seq_parameter_sets; } set { num_subset_seq_parameter_sets = value; } }
        private uint[][] subset_seq_parameter_set_id_delta;
        public uint[][] SubsetSeqParameterSetIdDelta { get { return subset_seq_parameter_set_id_delta; } set { subset_seq_parameter_set_id_delta = value; } }
        private uint[] num_pic_parameter_sets_minus1;
        public uint[] NumPicParameterSetsMinus1 { get { return num_pic_parameter_sets_minus1; } set { num_pic_parameter_sets_minus1 = value; } }
        private uint[][] pic_parameter_set_id_delta;
        public uint[][] PicParameterSetIdDelta { get { return pic_parameter_set_id_delta; } set { pic_parameter_set_id_delta = value; } }
        private uint[] parameter_sets_info_src_op_id;
        public uint[] ParameterSetsInfoSrcOpId { get { return parameter_sets_info_src_op_id; } set { parameter_sets_info_src_op_id = value; } }
        private byte[] motion_vectors_over_pic_boundaries_flag;
        public byte[] MotionVectorsOverPicBoundariesFlag { get { return motion_vectors_over_pic_boundaries_flag; } set { motion_vectors_over_pic_boundaries_flag = value; } }
        private uint[] max_bytes_per_pic_denom;
        public uint[] MaxBytesPerPicDenom { get { return max_bytes_per_pic_denom; } set { max_bytes_per_pic_denom = value; } }
        private uint[] max_bits_per_mb_denom;
        public uint[] MaxBitsPerMbDenom { get { return max_bits_per_mb_denom; } set { max_bits_per_mb_denom = value; } }
        private uint[] log2_max_mv_length_horizontal;
        public uint[] Log2MaxMvLengthHorizontal { get { return log2_max_mv_length_horizontal; } set { log2_max_mv_length_horizontal = value; } }
        private uint[] log2_max_mv_length_vertical;
        public uint[] Log2MaxMvLengthVertical { get { return log2_max_mv_length_vertical; } set { log2_max_mv_length_vertical = value; } }
        private uint[] max_num_reorder_frames;
        public uint[] MaxNumReorderFrames { get { return max_num_reorder_frames; } set { max_num_reorder_frames = value; } }
        private uint[] max_dec_frame_buffering;
        public uint[] MaxDecFrameBuffering { get { return max_dec_frame_buffering; } set { max_dec_frame_buffering = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ViewScalabilityInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.num_operation_points_minus1);

            this.operation_point_id = new uint[num_operation_points_minus1 + 1];
            this.priority_id = new uint[num_operation_points_minus1 + 1];
            this.temporal_id = new uint[num_operation_points_minus1 + 1];
            this.num_target_output_views_minus1 = new uint[num_operation_points_minus1 + 1];
            this.view_id = new uint[num_operation_points_minus1 + 1][];
            this.profile_level_info_present_flag = new byte[num_operation_points_minus1 + 1];
            this.bitrate_info_present_flag = new byte[num_operation_points_minus1 + 1];
            this.frm_rate_info_present_flag = new byte[num_operation_points_minus1 + 1];
            this.view_dependency_info_present_flag = new byte[num_operation_points_minus1 + 1];
            this.parameter_sets_info_present_flag = new byte[num_operation_points_minus1 + 1];
            this.bitstream_restriction_info_present_flag = new byte[num_operation_points_minus1 + 1];
            this.op_profile_level_idc = new uint[num_operation_points_minus1 + 1];
            this.avg_bitrate = new uint[num_operation_points_minus1 + 1];
            this.max_bitrate = new uint[num_operation_points_minus1 + 1];
            this.max_bitrate_calc_window = new uint[num_operation_points_minus1 + 1];
            this.constant_frm_rate_idc = new uint[num_operation_points_minus1 + 1];
            this.avg_frm_rate = new uint[num_operation_points_minus1 + 1];
            this.num_directly_dependent_views = new uint[num_operation_points_minus1 + 1];
            this.directly_dependent_view_id = new uint[num_operation_points_minus1 + 1][];
            this.view_dependency_info_src_op_id = new uint[num_operation_points_minus1 + 1];
            this.num_seq_parameter_sets = new uint[num_operation_points_minus1 + 1];
            this.seq_parameter_set_id_delta = new uint[num_operation_points_minus1 + 1][];
            this.num_subset_seq_parameter_sets = new uint[num_operation_points_minus1 + 1];
            this.subset_seq_parameter_set_id_delta = new uint[num_operation_points_minus1 + 1][];
            this.num_pic_parameter_sets_minus1 = new uint[num_operation_points_minus1 + 1];
            this.pic_parameter_set_id_delta = new uint[num_operation_points_minus1 + 1][];
            this.parameter_sets_info_src_op_id = new uint[num_operation_points_minus1 + 1];
            this.motion_vectors_over_pic_boundaries_flag = new byte[num_operation_points_minus1 + 1];
            this.max_bytes_per_pic_denom = new uint[num_operation_points_minus1 + 1];
            this.max_bits_per_mb_denom = new uint[num_operation_points_minus1 + 1];
            this.log2_max_mv_length_horizontal = new uint[num_operation_points_minus1 + 1];
            this.log2_max_mv_length_vertical = new uint[num_operation_points_minus1 + 1];
            this.max_num_reorder_frames = new uint[num_operation_points_minus1 + 1];
            this.max_dec_frame_buffering = new uint[num_operation_points_minus1 + 1];
            for (i = 0; i <= num_operation_points_minus1; i++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.operation_point_id[i]);
                size += stream.ReadUnsignedInt(size, 5, out this.priority_id[i]);
                size += stream.ReadUnsignedInt(size, 3, out this.temporal_id[i]);
                size += stream.ReadUnsignedIntGolomb(size, out this.num_target_output_views_minus1[i]);

                this.view_id[i] = new uint[num_target_output_views_minus1[i] + 1];
                for (j = 0; j <= num_target_output_views_minus1[i]; j++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.view_id[i][j]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.profile_level_info_present_flag[i]);
                size += stream.ReadUnsignedInt(size, 1, out this.bitrate_info_present_flag[i]);
                size += stream.ReadUnsignedInt(size, 1, out this.frm_rate_info_present_flag[i]);

                if (num_target_output_views_minus1[i] == 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.view_dependency_info_present_flag[i]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.parameter_sets_info_present_flag[i]);
                size += stream.ReadUnsignedInt(size, 1, out this.bitstream_restriction_info_present_flag[i]);

                if (profile_level_info_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 24, out this.op_profile_level_idc[i]);
                }

                if (bitrate_info_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 16, out this.avg_bitrate[i]);
                    size += stream.ReadUnsignedInt(size, 16, out this.max_bitrate[i]);
                    size += stream.ReadUnsignedInt(size, 16, out this.max_bitrate_calc_window[i]);
                }

                if (frm_rate_info_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 2, out this.constant_frm_rate_idc[i]);
                    size += stream.ReadUnsignedInt(size, 16, out this.avg_frm_rate[i]);
                }

                if (view_dependency_info_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_directly_dependent_views[i]);

                    this.directly_dependent_view_id[i] = new uint[num_directly_dependent_views[i]];
                    for (j = 0; j < num_directly_dependent_views[i]; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.directly_dependent_view_id[i][j]);
                    }
                }
                else
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.view_dependency_info_src_op_id[i]);
                }

                if (parameter_sets_info_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_seq_parameter_sets[i]);

                    this.seq_parameter_set_id_delta[i] = new uint[num_seq_parameter_sets[i]];
                    for (j = 0; j < num_seq_parameter_sets[i]; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.seq_parameter_set_id_delta[i][j]);
                    }
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_subset_seq_parameter_sets[i]);

                    this.subset_seq_parameter_set_id_delta[i] = new uint[num_subset_seq_parameter_sets[i]];
                    for (j = 0; j < num_subset_seq_parameter_sets[i]; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.subset_seq_parameter_set_id_delta[i][j]);
                    }
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_pic_parameter_sets_minus1[i]);

                    this.pic_parameter_set_id_delta[i] = new uint[num_pic_parameter_sets_minus1[i] + 1];
                    for (j = 0; j <= num_pic_parameter_sets_minus1[i]; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.pic_parameter_set_id_delta[i][j]);
                    }
                }
                else
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.parameter_sets_info_src_op_id[i]);
                }

                if (bitstream_restriction_info_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.motion_vectors_over_pic_boundaries_flag[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.max_bytes_per_pic_denom[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.max_bits_per_mb_denom[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.log2_max_mv_length_horizontal[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.log2_max_mv_length_vertical[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.max_num_reorder_frames[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.max_dec_frame_buffering[i]);
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.WriteUnsignedIntGolomb(this.num_operation_points_minus1);

            for (i = 0; i <= num_operation_points_minus1; i++)
            {
                size += stream.WriteUnsignedIntGolomb(this.operation_point_id[i]);
                size += stream.WriteUnsignedInt(5, this.priority_id[i]);
                size += stream.WriteUnsignedInt(3, this.temporal_id[i]);
                size += stream.WriteUnsignedIntGolomb(this.num_target_output_views_minus1[i]);

                for (j = 0; j <= num_target_output_views_minus1[i]; j++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.view_id[i][j]);
                }
                size += stream.WriteUnsignedInt(1, this.profile_level_info_present_flag[i]);
                size += stream.WriteUnsignedInt(1, this.bitrate_info_present_flag[i]);
                size += stream.WriteUnsignedInt(1, this.frm_rate_info_present_flag[i]);

                if (num_target_output_views_minus1[i] == 0)
                {
                    size += stream.WriteUnsignedInt(1, this.view_dependency_info_present_flag[i]);
                }
                size += stream.WriteUnsignedInt(1, this.parameter_sets_info_present_flag[i]);
                size += stream.WriteUnsignedInt(1, this.bitstream_restriction_info_present_flag[i]);

                if (profile_level_info_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(24, this.op_profile_level_idc[i]);
                }

                if (bitrate_info_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(16, this.avg_bitrate[i]);
                    size += stream.WriteUnsignedInt(16, this.max_bitrate[i]);
                    size += stream.WriteUnsignedInt(16, this.max_bitrate_calc_window[i]);
                }

                if (frm_rate_info_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(2, this.constant_frm_rate_idc[i]);
                    size += stream.WriteUnsignedInt(16, this.avg_frm_rate[i]);
                }

                if (view_dependency_info_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.num_directly_dependent_views[i]);

                    for (j = 0; j < num_directly_dependent_views[i]; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.directly_dependent_view_id[i][j]);
                    }
                }
                else
                {
                    size += stream.WriteUnsignedIntGolomb(this.view_dependency_info_src_op_id[i]);
                }

                if (parameter_sets_info_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.num_seq_parameter_sets[i]);

                    for (j = 0; j < num_seq_parameter_sets[i]; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.seq_parameter_set_id_delta[i][j]);
                    }
                    size += stream.WriteUnsignedIntGolomb(this.num_subset_seq_parameter_sets[i]);

                    for (j = 0; j < num_subset_seq_parameter_sets[i]; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.subset_seq_parameter_set_id_delta[i][j]);
                    }
                    size += stream.WriteUnsignedIntGolomb(this.num_pic_parameter_sets_minus1[i]);

                    for (j = 0; j <= num_pic_parameter_sets_minus1[i]; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.pic_parameter_set_id_delta[i][j]);
                    }
                }
                else
                {
                    size += stream.WriteUnsignedIntGolomb(this.parameter_sets_info_src_op_id[i]);
                }

                if (bitstream_restriction_info_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.motion_vectors_over_pic_boundaries_flag[i]);
                    size += stream.WriteUnsignedIntGolomb(this.max_bytes_per_pic_denom[i]);
                    size += stream.WriteUnsignedIntGolomb(this.max_bits_per_mb_denom[i]);
                    size += stream.WriteUnsignedIntGolomb(this.log2_max_mv_length_horizontal[i]);
                    size += stream.WriteUnsignedIntGolomb(this.log2_max_mv_length_vertical[i]);
                    size += stream.WriteUnsignedIntGolomb(this.max_num_reorder_frames[i]);
                    size += stream.WriteUnsignedIntGolomb(this.max_dec_frame_buffering[i]);
                }
            }

            return size;
        }

    }

    /*
  

multiview_scene_info( payloadSize ) {  
 max_disparity 5 ue(v) 
}
    */
    public class MultiviewSceneInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint max_disparity;
        public uint MaxDisparity { get { return max_disparity; } set { max_disparity = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public MultiviewSceneInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedIntGolomb(size, out this.max_disparity);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedIntGolomb(this.max_disparity);

            return size;
        }

    }

    /*
  

multiview_acquisition_info( payloadSize ) {  
 num_views_minus1  ue(v) 
 intrinsic_param_flag 5 u(1) 
 extrinsic_param_flag 5 u(1) 
if (intrinsic_param_flag ) {   
  intrinsic_params_equal_flag 5 u(1) 
  prec_focal_length 5 ue(v) 
  prec_principal_point 5 ue(v) 
  prec_skew_factor 5 ue(v) 
  for( i = 0; i <= (intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1); 
   i++ ) { 
  
   sign_focal_length_x[ i ] 5 u(1) 
   exponent_focal_length_x[ i ] 5 u(6) 
   mantissa_focal_length_x[ i ]  5 u(v) 
   sign_focal_length_y[ i ] 5 u(1) 
   exponent_focal_length_y[ i ] 5 u(6) 
   mantissa_focal_length_y[ i ] 5 u(v) 
   sign_principal_point_x[ i ] 5 u(1) 
   exponent_principal_point_x[ i ] 5 u(6) 
   mantissa_principal_point_x[ i ] 5 u(v) 
   sign_principal_point_y[ i ] 5 u(1) 
   exponent_principal_point_y[ i ] 5 u(6) 
   mantissa_principal_point_y[ i ]  5 u(v)
      sign_skew_factor[ i ] 5 u(1) 
   exponent_skew_factor[ i ] 5 u(6) 
   mantissa_skew_factor[ i ] 5 u(v) 
  }   
 }   
 if( extrinsic_param_flag ) {   
  prec_rotation_param 5 ue(v) 
  prec_translation_param 5 ue(v) 
  for( i = 0; i <= num_views_minus1; i++ ) {   
   for( j = 1; j <= 3; j++ ) { /* row *//*   
    for( k = 1; k <= 3; k++ ) { /* column *//*   
     sign_r[ i ][ j ][ k ] 5 u(1) 
     exponent_r[ i ][ j ][ k ] 5 u(6) 
     mantissa_r[ i ][ j ][ k ] 5 u(v) 
    }   
    sign_t[ i ][ j ] 5 u(1) 
    exponent_t[ i ][ j ] 5 u(6) 
    mantissa_t[ i ][ j ] 5 u(v) 
   }   
  }   
 }   
}
    */
    public class MultiviewAcquisitionInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint num_views_minus1;
        public uint NumViewsMinus1 { get { return num_views_minus1; } set { num_views_minus1 = value; } }
        private byte intrinsic_param_flag;
        public byte IntrinsicParamFlag { get { return intrinsic_param_flag; } set { intrinsic_param_flag = value; } }
        private byte extrinsic_param_flag;
        public byte ExtrinsicParamFlag { get { return extrinsic_param_flag; } set { extrinsic_param_flag = value; } }
        private byte intrinsic_params_equal_flag;
        public byte IntrinsicParamsEqualFlag { get { return intrinsic_params_equal_flag; } set { intrinsic_params_equal_flag = value; } }
        private uint prec_focal_length;
        public uint PrecFocalLength { get { return prec_focal_length; } set { prec_focal_length = value; } }
        private uint prec_principal_point;
        public uint PrecPrincipalPoint { get { return prec_principal_point; } set { prec_principal_point = value; } }
        private uint prec_skew_factor;
        public uint PrecSkewFactor { get { return prec_skew_factor; } set { prec_skew_factor = value; } }
        private byte[] sign_focal_length_x;
        public byte[] SignFocalLengthx { get { return sign_focal_length_x; } set { sign_focal_length_x = value; } }
        private uint[] exponent_focal_length_x;
        public uint[] ExponentFocalLengthx { get { return exponent_focal_length_x; } set { exponent_focal_length_x = value; } }
        private uint[] mantissa_focal_length_x;
        public uint[] MantissaFocalLengthx { get { return mantissa_focal_length_x; } set { mantissa_focal_length_x = value; } }
        private byte[] sign_focal_length_y;
        public byte[] SignFocalLengthy { get { return sign_focal_length_y; } set { sign_focal_length_y = value; } }
        private uint[] exponent_focal_length_y;
        public uint[] ExponentFocalLengthy { get { return exponent_focal_length_y; } set { exponent_focal_length_y = value; } }
        private uint[] mantissa_focal_length_y;
        public uint[] MantissaFocalLengthy { get { return mantissa_focal_length_y; } set { mantissa_focal_length_y = value; } }
        private byte[] sign_principal_point_x;
        public byte[] SignPrincipalPointx { get { return sign_principal_point_x; } set { sign_principal_point_x = value; } }
        private uint[] exponent_principal_point_x;
        public uint[] ExponentPrincipalPointx { get { return exponent_principal_point_x; } set { exponent_principal_point_x = value; } }
        private uint[] mantissa_principal_point_x;
        public uint[] MantissaPrincipalPointx { get { return mantissa_principal_point_x; } set { mantissa_principal_point_x = value; } }
        private byte[] sign_principal_point_y;
        public byte[] SignPrincipalPointy { get { return sign_principal_point_y; } set { sign_principal_point_y = value; } }
        private uint[] exponent_principal_point_y;
        public uint[] ExponentPrincipalPointy { get { return exponent_principal_point_y; } set { exponent_principal_point_y = value; } }
        private uint[] mantissa_principal_point_y;
        public uint[] MantissaPrincipalPointy { get { return mantissa_principal_point_y; } set { mantissa_principal_point_y = value; } }
        private byte[] sign_skew_factor;
        public byte[] SignSkewFactor { get { return sign_skew_factor; } set { sign_skew_factor = value; } }
        private uint[] exponent_skew_factor;
        public uint[] ExponentSkewFactor { get { return exponent_skew_factor; } set { exponent_skew_factor = value; } }
        private uint[] mantissa_skew_factor;
        public uint[] MantissaSkewFactor { get { return mantissa_skew_factor; } set { mantissa_skew_factor = value; } }
        private uint prec_rotation_param;
        public uint PrecRotationParam { get { return prec_rotation_param; } set { prec_rotation_param = value; } }
        private uint prec_translation_param;
        public uint PrecTranslationParam { get { return prec_translation_param; } set { prec_translation_param = value; } }
        private byte[][][] sign_r;
        public byte[][][] Signr { get { return sign_r; } set { sign_r = value; } }
        private uint[][][] exponent_r;
        public uint[][][] Exponentr { get { return exponent_r; } set { exponent_r = value; } }
        private uint[][][] mantissa_r;
        public uint[][][] Mantissar { get { return mantissa_r; } set { mantissa_r = value; } }
        private byte[][] sign_t;
        public byte[][] Signt { get { return sign_t; } set { sign_t = value; } }
        private uint[][] exponent_t;
        public uint[][] Exponentt { get { return exponent_t; } set { exponent_t = value; } }
        private uint[][] mantissa_t;
        public uint[][] Mantissat { get { return mantissa_t; } set { mantissa_t = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public MultiviewAcquisitionInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            uint k = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.num_views_minus1);
            size += stream.ReadUnsignedInt(size, 1, out this.intrinsic_param_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.extrinsic_param_flag);

            if (intrinsic_param_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.intrinsic_params_equal_flag);
                size += stream.ReadUnsignedIntGolomb(size, out this.prec_focal_length);
                size += stream.ReadUnsignedIntGolomb(size, out this.prec_principal_point);
                size += stream.ReadUnsignedIntGolomb(size, out this.prec_skew_factor);

                this.sign_focal_length_x = new byte[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.exponent_focal_length_x = new uint[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.mantissa_focal_length_x = new uint[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.sign_focal_length_y = new byte[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.exponent_focal_length_y = new uint[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.mantissa_focal_length_y = new uint[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.sign_principal_point_x = new byte[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.exponent_principal_point_x = new uint[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.mantissa_principal_point_x = new uint[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.sign_principal_point_y = new byte[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.exponent_principal_point_y = new uint[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.mantissa_principal_point_y = new uint[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.sign_skew_factor = new byte[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.exponent_skew_factor = new uint[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.mantissa_skew_factor = new uint[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                for (i = 0; i <= (intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1);
   i++)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.sign_focal_length_x[i]);
                    size += stream.ReadUnsignedInt(size, 6, out this.exponent_focal_length_x[i]);
                    size += stream.ReadUnsignedIntVariable(size, (exponent_focal_length_x[i] == 0) ? (Math.Max(0, prec_focal_length - 30)) : (Math.Max(0, exponent_focal_length_x[i] + prec_focal_length - 31)), out this.mantissa_focal_length_x[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.sign_focal_length_y[i]);
                    size += stream.ReadUnsignedInt(size, 6, out this.exponent_focal_length_y[i]);
                    size += stream.ReadUnsignedIntVariable(size, (exponent_focal_length_y[i] == 0) ? (Math.Max(0, prec_focal_length - 30)) : (Math.Max(0, exponent_focal_length_y[i] + prec_focal_length - 31)), out this.mantissa_focal_length_y[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.sign_principal_point_x[i]);
                    size += stream.ReadUnsignedInt(size, 6, out this.exponent_principal_point_x[i]);
                    size += stream.ReadUnsignedIntVariable(size, (exponent_principal_point_x[i] == 0) ? (Math.Max(0, prec_principal_point - 30)) : (Math.Max(0, exponent_principal_point_x[i] + prec_principal_point - 31)), out this.mantissa_principal_point_x[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.sign_principal_point_y[i]);
                    size += stream.ReadUnsignedInt(size, 6, out this.exponent_principal_point_y[i]);
                    size += stream.ReadUnsignedIntVariable(size, (exponent_principal_point_y[i] == 0) ? (Math.Max(0, prec_principal_point - 30)) : (Math.Max(0, exponent_principal_point_y[i] + prec_principal_point - 31)), out this.mantissa_principal_point_y[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.sign_skew_factor[i]);
                    size += stream.ReadUnsignedInt(size, 6, out this.exponent_skew_factor[i]);
                    size += stream.ReadUnsignedIntVariable(size, (exponent_skew_factor[i] == 0) ? (Math.Max(0, prec_skew_factor - 30)) : (Math.Max(0, exponent_skew_factor[i] + prec_skew_factor - 31)), out this.mantissa_skew_factor[i]);
                }
            }

            if (extrinsic_param_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.prec_rotation_param);
                size += stream.ReadUnsignedIntGolomb(size, out this.prec_translation_param);

                this.sign_r = new byte[num_views_minus1 + 1][][];
                this.exponent_r = new uint[num_views_minus1 + 1][][];
                this.mantissa_r = new uint[num_views_minus1 + 1][][];
                this.sign_t = new byte[num_views_minus1 + 1][];
                this.exponent_t = new uint[num_views_minus1 + 1][];
                this.mantissa_t = new uint[num_views_minus1 + 1][];
                for (i = 0; i <= num_views_minus1; i++)
                {

                    this.sign_r[i] = new byte[3][];
                    this.exponent_r[i] = new uint[3][];
                    this.mantissa_r[i] = new uint[3][];
                    this.sign_t[i] = new byte[3];
                    this.exponent_t[i] = new uint[3];
                    this.mantissa_t[i] = new uint[3];
                    for (j = 1; j <= 3; j++)
                    {
                        /*  row  */


                        this.sign_r[i][j] = new byte[3];
                        this.exponent_r[i][j] = new uint[3];
                        this.mantissa_r[i][j] = new uint[3];
                        for (k = 1; k <= 3; k++)
                        {
                            /*  column  */

                            size += stream.ReadUnsignedInt(size, 1, out this.sign_r[i][j][k]);
                            size += stream.ReadUnsignedInt(size, 6, out this.exponent_r[i][j][k]);
                            size += stream.ReadUnsignedIntVariable(size, (exponent_r[i][j][k] == 0) ? (Math.Max(0, prec_rotation_param - 30)) : (Math.Max(0, exponent_r[i][j][k] + prec_rotation_param - 31)), out this.mantissa_r[i][j][k]);
                        }
                        size += stream.ReadUnsignedInt(size, 1, out this.sign_t[i][j]);
                        size += stream.ReadUnsignedInt(size, 6, out this.exponent_t[i][j]);
                        size += stream.ReadUnsignedIntVariable(size, (exponent_t[i][j] == 0) ? (Math.Max(0, prec_translation_param - 30)) : (Math.Max(0, exponent_t[i][j] + prec_translation_param - 31)), out this.mantissa_t[i][j]);
                    }
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            uint k = 0;
            size += stream.WriteUnsignedIntGolomb(this.num_views_minus1);
            size += stream.WriteUnsignedInt(1, this.intrinsic_param_flag);
            size += stream.WriteUnsignedInt(1, this.extrinsic_param_flag);

            if (intrinsic_param_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.intrinsic_params_equal_flag);
                size += stream.WriteUnsignedIntGolomb(this.prec_focal_length);
                size += stream.WriteUnsignedIntGolomb(this.prec_principal_point);
                size += stream.WriteUnsignedIntGolomb(this.prec_skew_factor);

                for (i = 0; i <= (intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1);
   i++)
                {
                    size += stream.WriteUnsignedInt(1, this.sign_focal_length_x[i]);
                    size += stream.WriteUnsignedInt(6, this.exponent_focal_length_x[i]);
                    size += stream.WriteUnsignedIntVariable((exponent_focal_length_x[i] == 0) ? (Math.Max(0, prec_focal_length - 30)) : (Math.Max(0, exponent_focal_length_x[i] + prec_focal_length - 31)), this.mantissa_focal_length_x[i]);
                    size += stream.WriteUnsignedInt(1, this.sign_focal_length_y[i]);
                    size += stream.WriteUnsignedInt(6, this.exponent_focal_length_y[i]);
                    size += stream.WriteUnsignedIntVariable((exponent_focal_length_y[i] == 0) ? (Math.Max(0, prec_focal_length - 30)) : (Math.Max(0, exponent_focal_length_y[i] + prec_focal_length - 31)), this.mantissa_focal_length_y[i]);
                    size += stream.WriteUnsignedInt(1, this.sign_principal_point_x[i]);
                    size += stream.WriteUnsignedInt(6, this.exponent_principal_point_x[i]);
                    size += stream.WriteUnsignedIntVariable((exponent_principal_point_x[i] == 0) ? (Math.Max(0, prec_principal_point - 30)) : (Math.Max(0, exponent_principal_point_x[i] + prec_principal_point - 31)), this.mantissa_principal_point_x[i]);
                    size += stream.WriteUnsignedInt(1, this.sign_principal_point_y[i]);
                    size += stream.WriteUnsignedInt(6, this.exponent_principal_point_y[i]);
                    size += stream.WriteUnsignedIntVariable((exponent_principal_point_y[i] == 0) ? (Math.Max(0, prec_principal_point - 30)) : (Math.Max(0, exponent_principal_point_y[i] + prec_principal_point - 31)), this.mantissa_principal_point_y[i]);
                    size += stream.WriteUnsignedInt(1, this.sign_skew_factor[i]);
                    size += stream.WriteUnsignedInt(6, this.exponent_skew_factor[i]);
                    size += stream.WriteUnsignedIntVariable((exponent_skew_factor[i] == 0) ? (Math.Max(0, prec_skew_factor - 30)) : (Math.Max(0, exponent_skew_factor[i] + prec_skew_factor - 31)), this.mantissa_skew_factor[i]);
                }
            }

            if (extrinsic_param_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.prec_rotation_param);
                size += stream.WriteUnsignedIntGolomb(this.prec_translation_param);

                for (i = 0; i <= num_views_minus1; i++)
                {

                    for (j = 1; j <= 3; j++)
                    {
                        /*  row  */


                        for (k = 1; k <= 3; k++)
                        {
                            /*  column  */

                            size += stream.WriteUnsignedInt(1, this.sign_r[i][j][k]);
                            size += stream.WriteUnsignedInt(6, this.exponent_r[i][j][k]);
                            size += stream.WriteUnsignedIntVariable((exponent_r[i][j][k] == 0) ? (Math.Max(0, prec_rotation_param - 30)) : (Math.Max(0, exponent_r[i][j][k] + prec_rotation_param - 31)), this.mantissa_r[i][j][k]);
                        }
                        size += stream.WriteUnsignedInt(1, this.sign_t[i][j]);
                        size += stream.WriteUnsignedInt(6, this.exponent_t[i][j]);
                        size += stream.WriteUnsignedIntVariable((exponent_t[i][j] == 0) ? (Math.Max(0, prec_translation_param - 30)) : (Math.Max(0, exponent_t[i][j] + prec_translation_param - 31)), this.mantissa_t[i][j]);
                    }
                }
            }

            return size;
        }

    }

    /*
  

non_required_view_component( payloadSize ) {  
 num_info_entries_minus1 5 ue(v) 
 for( i = 0; i <= num_info_entries_minus1; i++ ) {   
  view_order_index[ i ] 5 ue(v) 
  num_non_required_view_components_minus1[ i ] 5 ue(v) 
  for( j = 0; j <= num_non_required_view_components_minus1[ i ]; j++ )    
   index_delta_minus1[ i ][ j ] 5 ue(v) 
 }   
}
    */
    public class NonRequiredViewComponent : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint num_info_entries_minus1;
        public uint NumInfoEntriesMinus1 { get { return num_info_entries_minus1; } set { num_info_entries_minus1 = value; } }
        private uint[] view_order_index;
        public uint[] ViewOrderIndex { get { return view_order_index; } set { view_order_index = value; } }
        private uint[] num_non_required_view_components_minus1;
        public uint[] NumNonRequiredViewComponentsMinus1 { get { return num_non_required_view_components_minus1; } set { num_non_required_view_components_minus1 = value; } }
        private uint[][] index_delta_minus1;
        public uint[][] IndexDeltaMinus1 { get { return index_delta_minus1; } set { index_delta_minus1 = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public NonRequiredViewComponent(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.num_info_entries_minus1);

            this.view_order_index = new uint[num_info_entries_minus1 + 1];
            this.num_non_required_view_components_minus1 = new uint[num_info_entries_minus1 + 1];
            this.index_delta_minus1 = new uint[num_info_entries_minus1 + 1][];
            for (i = 0; i <= num_info_entries_minus1; i++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.view_order_index[i]);
                size += stream.ReadUnsignedIntGolomb(size, out this.num_non_required_view_components_minus1[i]);

                this.index_delta_minus1[i] = new uint[num_non_required_view_components_minus1[i] + 1];
                for (j = 0; j <= num_non_required_view_components_minus1[i]; j++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.index_delta_minus1[i][j]);
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.WriteUnsignedIntGolomb(this.num_info_entries_minus1);

            for (i = 0; i <= num_info_entries_minus1; i++)
            {
                size += stream.WriteUnsignedIntGolomb(this.view_order_index[i]);
                size += stream.WriteUnsignedIntGolomb(this.num_non_required_view_components_minus1[i]);

                for (j = 0; j <= num_non_required_view_components_minus1[i]; j++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.index_delta_minus1[i][j]);
                }
            }

            return size;
        }

    }

    /*
   

view_dependency_change( payloadSize ) {  
 seq_parameter_set_id 5 ue(v) 
 anchor_update_flag 5 u(1) 
 non_anchor_update_flag 5 u(1) 
 if( anchor_update_flag )   
  for( i = 1; i <= num_views_minus1; i++ ) {   
   for( j = 0; j < num_anchor_refs_l0[ i ]; j++ )   
    anchor_ref_l0_flag[ i ][ j ] 5 u(1) 
   for( j = 0; j < num_anchor_refs_l1[ i ]; j++ )   
    anchor_ref_l1_flag[ i ][ j ] 5 u(1) 
  }   
 if( non_anchor_update_flag )   
  for( i = 1; i <= num_views_minus1; i++ ) {   
   for( j = 0; j < num_non_anchor_refs_l0[ i ]; j++ )   
    non_anchor_ref_l0_flag[ i ][ j ] 5 u(1) 
   for( j = 0; j < num_non_anchor_refs_l1[ i ]; j++ )   
    non_anchor_ref_l1_flag[ i ][ j ] 5 u(1) 
  }   
}
    */
    public class ViewDependencyChange : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint seq_parameter_set_id;
        public uint SeqParameterSetId { get { return seq_parameter_set_id; } set { seq_parameter_set_id = value; } }
        private byte anchor_update_flag;
        public byte AnchorUpdateFlag { get { return anchor_update_flag; } set { anchor_update_flag = value; } }
        private byte non_anchor_update_flag;
        public byte NonAnchorUpdateFlag { get { return non_anchor_update_flag; } set { non_anchor_update_flag = value; } }
        private byte[][] anchor_ref_l0_flag;
        public byte[][] AnchorRefL0Flag { get { return anchor_ref_l0_flag; } set { anchor_ref_l0_flag = value; } }
        private byte[][] anchor_ref_l1_flag;
        public byte[][] AnchorRefL1Flag { get { return anchor_ref_l1_flag; } set { anchor_ref_l1_flag = value; } }
        private byte[][] non_anchor_ref_l0_flag;
        public byte[][] NonAnchorRefL0Flag { get { return non_anchor_ref_l0_flag; } set { non_anchor_ref_l0_flag = value; } }
        private byte[][] non_anchor_ref_l1_flag;
        public byte[][] NonAnchorRefL1Flag { get { return non_anchor_ref_l1_flag; } set { non_anchor_ref_l1_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ViewDependencyChange(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.seq_parameter_set_id);
            size += stream.ReadUnsignedInt(size, 1, out this.anchor_update_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.non_anchor_update_flag);

            if (anchor_update_flag != 0)
            {

                this.anchor_ref_l0_flag = new byte[context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1 + 1][];
                this.anchor_ref_l1_flag = new byte[context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1 + 1][];
                for (i = 1; i <= context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1; i++)
                {

                    this.anchor_ref_l0_flag[i] = new byte[context.SubsetSps.SeqParameterSetMvcExtension.NumAnchorRefsL0[i]];
                    for (j = 0; j < context.SubsetSps.SeqParameterSetMvcExtension.NumAnchorRefsL0[i]; j++)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.anchor_ref_l0_flag[i][j]);
                    }

                    this.anchor_ref_l1_flag[i] = new byte[context.SubsetSps.SeqParameterSetMvcExtension.NumAnchorRefsL1[i]];
                    for (j = 0; j < context.SubsetSps.SeqParameterSetMvcExtension.NumAnchorRefsL1[i]; j++)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.anchor_ref_l1_flag[i][j]);
                    }
                }
            }

            if (non_anchor_update_flag != 0)
            {

                this.non_anchor_ref_l0_flag = new byte[context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1 + 1][];
                this.non_anchor_ref_l1_flag = new byte[context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1 + 1][];
                for (i = 1; i <= context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1; i++)
                {

                    this.non_anchor_ref_l0_flag[i] = new byte[context.SubsetSps.SeqParameterSetMvcExtension.NumNonAnchorRefsL0[i]];
                    for (j = 0; j < context.SubsetSps.SeqParameterSetMvcExtension.NumNonAnchorRefsL0[i]; j++)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.non_anchor_ref_l0_flag[i][j]);
                    }

                    this.non_anchor_ref_l1_flag[i] = new byte[context.SubsetSps.SeqParameterSetMvcExtension.NumNonAnchorRefsL1[i]];
                    for (j = 0; j < context.SubsetSps.SeqParameterSetMvcExtension.NumNonAnchorRefsL1[i]; j++)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.non_anchor_ref_l1_flag[i][j]);
                    }
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.WriteUnsignedIntGolomb(this.seq_parameter_set_id);
            size += stream.WriteUnsignedInt(1, this.anchor_update_flag);
            size += stream.WriteUnsignedInt(1, this.non_anchor_update_flag);

            if (anchor_update_flag != 0)
            {

                for (i = 1; i <= context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1; i++)
                {

                    for (j = 0; j < context.SubsetSps.SeqParameterSetMvcExtension.NumAnchorRefsL0[i]; j++)
                    {
                        size += stream.WriteUnsignedInt(1, this.anchor_ref_l0_flag[i][j]);
                    }

                    for (j = 0; j < context.SubsetSps.SeqParameterSetMvcExtension.NumAnchorRefsL1[i]; j++)
                    {
                        size += stream.WriteUnsignedInt(1, this.anchor_ref_l1_flag[i][j]);
                    }
                }
            }

            if (non_anchor_update_flag != 0)
            {

                for (i = 1; i <= context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1; i++)
                {

                    for (j = 0; j < context.SubsetSps.SeqParameterSetMvcExtension.NumNonAnchorRefsL0[i]; j++)
                    {
                        size += stream.WriteUnsignedInt(1, this.non_anchor_ref_l0_flag[i][j]);
                    }

                    for (j = 0; j < context.SubsetSps.SeqParameterSetMvcExtension.NumNonAnchorRefsL1[i]; j++)
                    {
                        size += stream.WriteUnsignedInt(1, this.non_anchor_ref_l1_flag[i][j]);
                    }
                }
            }

            return size;
        }

    }

    /*
  

operation_point_not_present( payloadSize ) {  
 num_operation_points 5 ue(v) 
 for( k = 0; k < num_operation_points; k++ )    
  operation_point_not_present_id[ k ] 5 ue(v) 
}
    */
    public class OperationPointNotPresent : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint num_operation_points;
        public uint NumOperationPoints { get { return num_operation_points; } set { num_operation_points = value; } }
        private uint[] operation_point_not_present_id;
        public uint[] OperationPointNotPresentId { get { return operation_point_not_present_id; } set { operation_point_not_present_id = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public OperationPointNotPresent(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint k = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.num_operation_points);

            this.operation_point_not_present_id = new uint[num_operation_points];
            for (k = 0; k < num_operation_points; k++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.operation_point_not_present_id[k]);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint k = 0;
            size += stream.WriteUnsignedIntGolomb(this.num_operation_points);

            for (k = 0; k < num_operation_points; k++)
            {
                size += stream.WriteUnsignedIntGolomb(this.operation_point_not_present_id[k]);
            }

            return size;
        }

    }

    /*
   

base_view_temporal_hrd( payloadSize ) {  
 num_of_temporal_layers_in_base_view_minus1 5 ue(v) 
 for( i = 0; i <= num_of_temporal_layers_in_base_view_minus1; i++ ) {   
  sei_mvc_temporal_id[ i ] 5 u(3) 
  sei_mvc_timing_info_present_flag[ i ] 5 u(1) 
  if( sei_mvc_timing_info_present_flag[ i ] ) {   
   sei_mvc_num_units_in_tick[ i ] 5 u(32) 
   sei_mvc_time_scale[ i ] 5 u(32) 
   sei_mvc_fixed_frame_rate_flag[ i ] 5 u(1) 
  }   
  sei_mvc_nal_hrd_parameters_present_flag[ i ] 5 u(1) 
  if( sei_mvc_nal_hrd_parameters_present_flag[ i ] )   
   hrd_parameters() 5  
  sei_mvc_vcl_hrd_parameters_present_flag[ i ] 5 u(1) 
  if( sei_mvc_vcl_hrd_parameters_present_flag[ i ] )   
  hrd_parameters() 5
if( sei_mvc_nal_hrd_parameters_present_flag[ i ]  ||   
sei_mvc_vcl_hrd_parameters_present_flag[ i ] ) 
sei_mvc_low_delay_hrd_flag[ i ] 5 u(1) 
sei_mvc_pic_struct_present_flag[ i ] 5 u(1) 
}   
}
    */
    public class BaseViewTemporalHrd : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint num_of_temporal_layers_in_base_view_minus1;
        public uint NumOfTemporalLayersInBaseViewMinus1 { get { return num_of_temporal_layers_in_base_view_minus1; } set { num_of_temporal_layers_in_base_view_minus1 = value; } }
        private uint[] sei_mvc_temporal_id;
        public uint[] SeiMvcTemporalId { get { return sei_mvc_temporal_id; } set { sei_mvc_temporal_id = value; } }
        private byte[] sei_mvc_timing_info_present_flag;
        public byte[] SeiMvcTimingInfoPresentFlag { get { return sei_mvc_timing_info_present_flag; } set { sei_mvc_timing_info_present_flag = value; } }
        private uint[] sei_mvc_num_units_in_tick;
        public uint[] SeiMvcNumUnitsInTick { get { return sei_mvc_num_units_in_tick; } set { sei_mvc_num_units_in_tick = value; } }
        private uint[] sei_mvc_time_scale;
        public uint[] SeiMvcTimeScale { get { return sei_mvc_time_scale; } set { sei_mvc_time_scale = value; } }
        private byte[] sei_mvc_fixed_frame_rate_flag;
        public byte[] SeiMvcFixedFrameRateFlag { get { return sei_mvc_fixed_frame_rate_flag; } set { sei_mvc_fixed_frame_rate_flag = value; } }
        private byte[] sei_mvc_nal_hrd_parameters_present_flag;
        public byte[] SeiMvcNalHrdParametersPresentFlag { get { return sei_mvc_nal_hrd_parameters_present_flag; } set { sei_mvc_nal_hrd_parameters_present_flag = value; } }
        private HrdParameters[] hrd_parameters;
        public HrdParameters[] HrdParameters { get { return hrd_parameters; } set { hrd_parameters = value; } }
        private byte[] sei_mvc_vcl_hrd_parameters_present_flag;
        public byte[] SeiMvcVclHrdParametersPresentFlag { get { return sei_mvc_vcl_hrd_parameters_present_flag; } set { sei_mvc_vcl_hrd_parameters_present_flag = value; } }
        private byte[] sei_mvc_low_delay_hrd_flag;
        public byte[] SeiMvcLowDelayHrdFlag { get { return sei_mvc_low_delay_hrd_flag; } set { sei_mvc_low_delay_hrd_flag = value; } }
        private byte[] sei_mvc_pic_struct_present_flag;
        public byte[] SeiMvcPicStructPresentFlag { get { return sei_mvc_pic_struct_present_flag; } set { sei_mvc_pic_struct_present_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public BaseViewTemporalHrd(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.num_of_temporal_layers_in_base_view_minus1);

            this.sei_mvc_temporal_id = new uint[num_of_temporal_layers_in_base_view_minus1 + 1];
            this.sei_mvc_timing_info_present_flag = new byte[num_of_temporal_layers_in_base_view_minus1 + 1];
            this.sei_mvc_num_units_in_tick = new uint[num_of_temporal_layers_in_base_view_minus1 + 1];
            this.sei_mvc_time_scale = new uint[num_of_temporal_layers_in_base_view_minus1 + 1];
            this.sei_mvc_fixed_frame_rate_flag = new byte[num_of_temporal_layers_in_base_view_minus1 + 1];
            this.sei_mvc_nal_hrd_parameters_present_flag = new byte[num_of_temporal_layers_in_base_view_minus1 + 1];
            this.hrd_parameters = new HrdParameters[num_of_temporal_layers_in_base_view_minus1 + 1];
            this.sei_mvc_vcl_hrd_parameters_present_flag = new byte[num_of_temporal_layers_in_base_view_minus1 + 1];
            this.sei_mvc_low_delay_hrd_flag = new byte[num_of_temporal_layers_in_base_view_minus1 + 1];
            this.sei_mvc_pic_struct_present_flag = new byte[num_of_temporal_layers_in_base_view_minus1 + 1];
            for (i = 0; i <= num_of_temporal_layers_in_base_view_minus1; i++)
            {
                size += stream.ReadUnsignedInt(size, 3, out this.sei_mvc_temporal_id[i]);
                size += stream.ReadUnsignedInt(size, 1, out this.sei_mvc_timing_info_present_flag[i]);

                if (sei_mvc_timing_info_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 32, out this.sei_mvc_num_units_in_tick[i]);
                    size += stream.ReadUnsignedInt(size, 32, out this.sei_mvc_time_scale[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.sei_mvc_fixed_frame_rate_flag[i]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.sei_mvc_nal_hrd_parameters_present_flag[i]);

                if (sei_mvc_nal_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.ReadClass<HrdParameters>(size, context, () => new HrdParameters(), out this.hrd_parameters[i]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.sei_mvc_vcl_hrd_parameters_present_flag[i]);

                if (sei_mvc_vcl_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.ReadClass<HrdParameters>(size, context, () => new HrdParameters(), out this.hrd_parameters[i]);
                }

                if (sei_mvc_nal_hrd_parameters_present_flag[i] != 0 ||
sei_mvc_vcl_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.sei_mvc_low_delay_hrd_flag[i]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.sei_mvc_pic_struct_present_flag[i]);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedIntGolomb(this.num_of_temporal_layers_in_base_view_minus1);

            for (i = 0; i <= num_of_temporal_layers_in_base_view_minus1; i++)
            {
                size += stream.WriteUnsignedInt(3, this.sei_mvc_temporal_id[i]);
                size += stream.WriteUnsignedInt(1, this.sei_mvc_timing_info_present_flag[i]);

                if (sei_mvc_timing_info_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(32, this.sei_mvc_num_units_in_tick[i]);
                    size += stream.WriteUnsignedInt(32, this.sei_mvc_time_scale[i]);
                    size += stream.WriteUnsignedInt(1, this.sei_mvc_fixed_frame_rate_flag[i]);
                }
                size += stream.WriteUnsignedInt(1, this.sei_mvc_nal_hrd_parameters_present_flag[i]);

                if (sei_mvc_nal_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.WriteClass<HrdParameters>(context, this.hrd_parameters[i]);
                }
                size += stream.WriteUnsignedInt(1, this.sei_mvc_vcl_hrd_parameters_present_flag[i]);

                if (sei_mvc_vcl_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.WriteClass<HrdParameters>(context, this.hrd_parameters[i]);
                }

                if (sei_mvc_nal_hrd_parameters_present_flag[i] != 0 ||
sei_mvc_vcl_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.sei_mvc_low_delay_hrd_flag[i]);
                }
                size += stream.WriteUnsignedInt(1, this.sei_mvc_pic_struct_present_flag[i]);
            }

            return size;
        }

    }

    /*
  

multiview_view_position( payloadSize ) { 
num_views_minus1 5 ue(v)
for( i = 0; i <= num_views_minus1; i++ )   
view_position[ i ] 5 ue(v)
multiview_view_position_extension_flag 5 u(1)
}
    */
    public class MultiviewViewPosition : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint num_views_minus1;
        public uint NumViewsMinus1 { get { return num_views_minus1; } set { num_views_minus1 = value; } }
        private uint[] view_position;
        public uint[] ViewPosition { get { return view_position; } set { view_position = value; } }
        private byte multiview_view_position_extension_flag;
        public byte MultiviewViewPositionExtensionFlag { get { return multiview_view_position_extension_flag; } set { multiview_view_position_extension_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public MultiviewViewPosition(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.num_views_minus1);

            this.view_position = new uint[num_views_minus1 + 1];
            for (i = 0; i <= num_views_minus1; i++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.view_position[i]);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.multiview_view_position_extension_flag);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedIntGolomb(this.num_views_minus1);

            for (i = 0; i <= num_views_minus1; i++)
            {
                size += stream.WriteUnsignedIntGolomb(this.view_position[i]);
            }
            size += stream.WriteUnsignedInt(1, this.multiview_view_position_extension_flag);

            return size;
        }

    }

    /*
   

mvc_vui_parameters_extension() {  
 vui_mvc_num_ops_minus1 0 ue(v) 
 for( i = 0; i <= vui_mvc_num_ops_minus1; i++ ) {   
  vui_mvc_temporal_id[ i ] 0 u(3) 
  vui_mvc_num_target_output_views_minus1[ i ] 5 ue(v) 
  for( j = 0; j <= vui_mvc_num_target_output_views_minus1[ i ]; j++ )    
   vui_mvc_view_id[ i ][ j ] 5 ue(v) 
  vui_mvc_timing_info_present_flag[ i ] 0 u(1) 
  if( vui_mvc_timing_info_present_flag[ i ] ) {   
   vui_mvc_num_units_in_tick[ i ] 0 u(32) 
   vui_mvc_time_scale[ i ] 0 u(32) 
   vui_mvc_fixed_frame_rate_flag[ i ] 0 u(1) 
  }   
  vui_mvc_nal_hrd_parameters_present_flag[ i ] 0 u(1) 
  if( vui_mvc_nal_hrd_parameters_present_flag[ i ] )   
   hrd_parameters() 0  
  vui_mvc_vcl_hrd_parameters_present_flag[ i ] 0 u(1) 
  if( vui_mvc_vcl_hrd_parameters_present_flag[ i ] )   
   hrd_parameters() 0  
  if( vui_mvc_nal_hrd_parameters_present_flag[ i ]  ||   
   vui_mvc_vcl_hrd_parameters_present_flag[ i ] ) 
  
   vui_mvc_low_delay_hrd_flag[ i ] 0 u(1) 
   vui_mvc_pic_struct_present_flag[ i ]  0 u(1)
}   
}
    */
    public class MvcVuiParametersExtension : IItuSerializable
    {
        private uint vui_mvc_num_ops_minus1;
        public uint VuiMvcNumOpsMinus1 { get { return vui_mvc_num_ops_minus1; } set { vui_mvc_num_ops_minus1 = value; } }
        private uint[] vui_mvc_temporal_id;
        public uint[] VuiMvcTemporalId { get { return vui_mvc_temporal_id; } set { vui_mvc_temporal_id = value; } }
        private uint[] vui_mvc_num_target_output_views_minus1;
        public uint[] VuiMvcNumTargetOutputViewsMinus1 { get { return vui_mvc_num_target_output_views_minus1; } set { vui_mvc_num_target_output_views_minus1 = value; } }
        private uint[][] vui_mvc_view_id;
        public uint[][] VuiMvcViewId { get { return vui_mvc_view_id; } set { vui_mvc_view_id = value; } }
        private byte[] vui_mvc_timing_info_present_flag;
        public byte[] VuiMvcTimingInfoPresentFlag { get { return vui_mvc_timing_info_present_flag; } set { vui_mvc_timing_info_present_flag = value; } }
        private uint[] vui_mvc_num_units_in_tick;
        public uint[] VuiMvcNumUnitsInTick { get { return vui_mvc_num_units_in_tick; } set { vui_mvc_num_units_in_tick = value; } }
        private uint[] vui_mvc_time_scale;
        public uint[] VuiMvcTimeScale { get { return vui_mvc_time_scale; } set { vui_mvc_time_scale = value; } }
        private byte[] vui_mvc_fixed_frame_rate_flag;
        public byte[] VuiMvcFixedFrameRateFlag { get { return vui_mvc_fixed_frame_rate_flag; } set { vui_mvc_fixed_frame_rate_flag = value; } }
        private byte[] vui_mvc_nal_hrd_parameters_present_flag;
        public byte[] VuiMvcNalHrdParametersPresentFlag { get { return vui_mvc_nal_hrd_parameters_present_flag; } set { vui_mvc_nal_hrd_parameters_present_flag = value; } }
        private HrdParameters[] hrd_parameters;
        public HrdParameters[] HrdParameters { get { return hrd_parameters; } set { hrd_parameters = value; } }
        private byte[] vui_mvc_vcl_hrd_parameters_present_flag;
        public byte[] VuiMvcVclHrdParametersPresentFlag { get { return vui_mvc_vcl_hrd_parameters_present_flag; } set { vui_mvc_vcl_hrd_parameters_present_flag = value; } }
        private byte[] vui_mvc_low_delay_hrd_flag;
        public byte[] VuiMvcLowDelayHrdFlag { get { return vui_mvc_low_delay_hrd_flag; } set { vui_mvc_low_delay_hrd_flag = value; } }
        private byte[] vui_mvc_pic_struct_present_flag;
        public byte[] VuiMvcPicStructPresentFlag { get { return vui_mvc_pic_struct_present_flag; } set { vui_mvc_pic_struct_present_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public MvcVuiParametersExtension()
        {

        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.vui_mvc_num_ops_minus1);

            this.vui_mvc_temporal_id = new uint[vui_mvc_num_ops_minus1 + 1];
            this.vui_mvc_num_target_output_views_minus1 = new uint[vui_mvc_num_ops_minus1 + 1];
            this.vui_mvc_view_id = new uint[vui_mvc_num_ops_minus1 + 1][];
            this.vui_mvc_timing_info_present_flag = new byte[vui_mvc_num_ops_minus1 + 1];
            this.vui_mvc_num_units_in_tick = new uint[vui_mvc_num_ops_minus1 + 1];
            this.vui_mvc_time_scale = new uint[vui_mvc_num_ops_minus1 + 1];
            this.vui_mvc_fixed_frame_rate_flag = new byte[vui_mvc_num_ops_minus1 + 1];
            this.vui_mvc_nal_hrd_parameters_present_flag = new byte[vui_mvc_num_ops_minus1 + 1];
            this.hrd_parameters = new HrdParameters[vui_mvc_num_ops_minus1 + 1];
            this.vui_mvc_vcl_hrd_parameters_present_flag = new byte[vui_mvc_num_ops_minus1 + 1];
            this.vui_mvc_low_delay_hrd_flag = new byte[vui_mvc_num_ops_minus1 + 1];
            this.vui_mvc_pic_struct_present_flag = new byte[vui_mvc_num_ops_minus1 + 1];
            for (i = 0; i <= vui_mvc_num_ops_minus1; i++)
            {
                size += stream.ReadUnsignedInt(size, 3, out this.vui_mvc_temporal_id[i]);
                size += stream.ReadUnsignedIntGolomb(size, out this.vui_mvc_num_target_output_views_minus1[i]);

                this.vui_mvc_view_id[i] = new uint[vui_mvc_num_target_output_views_minus1[i] + 1];
                for (j = 0; j <= vui_mvc_num_target_output_views_minus1[i]; j++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.vui_mvc_view_id[i][j]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.vui_mvc_timing_info_present_flag[i]);

                if (vui_mvc_timing_info_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 32, out this.vui_mvc_num_units_in_tick[i]);
                    size += stream.ReadUnsignedInt(size, 32, out this.vui_mvc_time_scale[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.vui_mvc_fixed_frame_rate_flag[i]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.vui_mvc_nal_hrd_parameters_present_flag[i]);

                if (vui_mvc_nal_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.ReadClass<HrdParameters>(size, context, () => new HrdParameters(), out this.hrd_parameters[i]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.vui_mvc_vcl_hrd_parameters_present_flag[i]);

                if (vui_mvc_vcl_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.ReadClass<HrdParameters>(size, context, () => new HrdParameters(), out this.hrd_parameters[i]);
                }

                if (vui_mvc_nal_hrd_parameters_present_flag[i] != 0 ||
   vui_mvc_vcl_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.vui_mvc_low_delay_hrd_flag[i]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.vui_mvc_pic_struct_present_flag[i]);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.WriteUnsignedIntGolomb(this.vui_mvc_num_ops_minus1);

            for (i = 0; i <= vui_mvc_num_ops_minus1; i++)
            {
                size += stream.WriteUnsignedInt(3, this.vui_mvc_temporal_id[i]);
                size += stream.WriteUnsignedIntGolomb(this.vui_mvc_num_target_output_views_minus1[i]);

                for (j = 0; j <= vui_mvc_num_target_output_views_minus1[i]; j++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.vui_mvc_view_id[i][j]);
                }
                size += stream.WriteUnsignedInt(1, this.vui_mvc_timing_info_present_flag[i]);

                if (vui_mvc_timing_info_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(32, this.vui_mvc_num_units_in_tick[i]);
                    size += stream.WriteUnsignedInt(32, this.vui_mvc_time_scale[i]);
                    size += stream.WriteUnsignedInt(1, this.vui_mvc_fixed_frame_rate_flag[i]);
                }
                size += stream.WriteUnsignedInt(1, this.vui_mvc_nal_hrd_parameters_present_flag[i]);

                if (vui_mvc_nal_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.WriteClass<HrdParameters>(context, this.hrd_parameters[i]);
                }
                size += stream.WriteUnsignedInt(1, this.vui_mvc_vcl_hrd_parameters_present_flag[i]);

                if (vui_mvc_vcl_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.WriteClass<HrdParameters>(context, this.hrd_parameters[i]);
                }

                if (vui_mvc_nal_hrd_parameters_present_flag[i] != 0 ||
   vui_mvc_vcl_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.vui_mvc_low_delay_hrd_flag[i]);
                }
                size += stream.WriteUnsignedInt(1, this.vui_mvc_pic_struct_present_flag[i]);
            }

            return size;
        }

    }

    /*
   

seq_parameter_set_mvcd_extension() {  
 num_views_minus1 0 ue(v) 
 for( i = 0, NumDepthViews = 0; i <= num_views_minus1; i++ ) {   
  view_id[ i ] 0 ue(v) 
  depth_view_present_flag[ i ] 0 u(1) 
  DepthViewId[ NumDepthViews ] = view_id[ i ]   
  NumDepthViews += depth_view_present_flag[ i ]   
  texture_view_present_flag[ i ] 0 u(1) 
 }   
 for( i = 1; i <= num_views_minus1; i++ )   
  if( depth_view_present_flag[ i ] ) {   
   num_anchor_refs_l0[ i ] 0 ue(v) 
   for( j = 0; j < num_anchor_refs_l0[ i ]; j++ )   
    anchor_ref_l0[ i ][ j ] 0 ue(v) 
   num_anchor_refs_l1[ i ] 0 ue(v) 
   for( j = 0; j < num_anchor_refs_l1[ i ]; j++ )   
    anchor_ref_l1[ i ][ j ] 0 ue(v) 
  }   
 for( i = 1; i <= num_views_minus1; i++ )   
  if( depth_view_present_flag[ i ] ) {   
   num_non_anchor_refs_l0[ i ] 0 ue(v) 
   for( j = 0; j < num_non_anchor_refs_l0[ i ]; j++ )   
    non_anchor_ref_l0[ i ][ j ] 0 ue(v) 
   num_non_anchor_refs_l1[ i ] 0 ue(v) 
   for( j = 0; j < num_non_anchor_refs_l1[ i ]; j++ )   
    non_anchor_ref_l1[ i ][ j ] 0 ue(v) 
  }   
 num_level_values_signalled_minus1 0 ue(v) 
 for( i = 0; i <= num_level_values_signalled_minus1; i++ ) {   
  level_idc[ i ] 0 u(8) 
  num_applicable_ops_minus1[ i ] 0 ue(v) 
  for( j = 0; j <= num_applicable_ops_minus1[ i ]; j++ ) {   
   applicable_op_temporal_id[ i ][ j ] 0 u(3) 
   applicable_op_num_target_views_minus1[ i ][ j ] 0 ue(v) 
   for( k = 0; k <= applicable_op_num_target_views_minus1[ i ][ j ]; 
    k++ ) { 
  
    applicable_op_target_view_id[ i ][ j ][ k ] 0 ue(v) 
    applicable_op_depth_flag[ i ][ j ][ k ] 0 u(1) 
    applicable_op_texture_flag[ i ][ j ][ k ] 0 u(1) 
   }   
   applicable_op_num_texture_views_minus1[ i ][ j ] 0 ue(v) 
   applicable_op_num_depth_views[ i ][ j ] 0 ue(v) 
  }   
 }   
 mvcd_vui_parameters_present_flag 0 u(1) 
 if( mvcd_vui_parameters_present_flag  ==  1 )   
  mvcd_vui_parameters_extension()   
 texture_vui_parameters_present_flag 0 u(1) 
 if( texture_vui_parameters_present_flag  ==  1 )   
  mvc_vui_parameters_extension() 0  
}
    */
    public class SeqParameterSetMvcdExtension : IItuSerializable
    {
        private uint num_views_minus1;
        public uint NumViewsMinus1 { get { return num_views_minus1; } set { num_views_minus1 = value; } }
        private uint numDepthViews;
        public uint NumDepthViews { get { return NumDepthViews; } set { NumDepthViews = value; } }
        private uint[] view_id;
        public uint[] ViewId { get { return view_id; } set { view_id = value; } }
        private byte[] depth_view_present_flag;
        public byte[] DepthViewPresentFlag { get { return depth_view_present_flag; } set { depth_view_present_flag = value; } }
        private byte[] texture_view_present_flag;
        public byte[] TextureViewPresentFlag { get { return texture_view_present_flag; } set { texture_view_present_flag = value; } }
        private uint[] num_anchor_refs_l0;
        public uint[] NumAnchorRefsL0 { get { return num_anchor_refs_l0; } set { num_anchor_refs_l0 = value; } }
        private uint[][] anchor_ref_l0;
        public uint[][] AnchorRefL0 { get { return anchor_ref_l0; } set { anchor_ref_l0 = value; } }
        private uint[] num_anchor_refs_l1;
        public uint[] NumAnchorRefsL1 { get { return num_anchor_refs_l1; } set { num_anchor_refs_l1 = value; } }
        private uint[][] anchor_ref_l1;
        public uint[][] AnchorRefL1 { get { return anchor_ref_l1; } set { anchor_ref_l1 = value; } }
        private uint[] num_non_anchor_refs_l0;
        public uint[] NumNonAnchorRefsL0 { get { return num_non_anchor_refs_l0; } set { num_non_anchor_refs_l0 = value; } }
        private uint[][] non_anchor_ref_l0;
        public uint[][] NonAnchorRefL0 { get { return non_anchor_ref_l0; } set { non_anchor_ref_l0 = value; } }
        private uint[] num_non_anchor_refs_l1;
        public uint[] NumNonAnchorRefsL1 { get { return num_non_anchor_refs_l1; } set { num_non_anchor_refs_l1 = value; } }
        private uint[][] non_anchor_ref_l1;
        public uint[][] NonAnchorRefL1 { get { return non_anchor_ref_l1; } set { non_anchor_ref_l1 = value; } }
        private uint num_level_values_signalled_minus1;
        public uint NumLevelValuesSignalledMinus1 { get { return num_level_values_signalled_minus1; } set { num_level_values_signalled_minus1 = value; } }
        private uint[] level_idc;
        public uint[] LevelIdc { get { return level_idc; } set { level_idc = value; } }
        private uint[] num_applicable_ops_minus1;
        public uint[] NumApplicableOpsMinus1 { get { return num_applicable_ops_minus1; } set { num_applicable_ops_minus1 = value; } }
        private uint[][] applicable_op_temporal_id;
        public uint[][] ApplicableOpTemporalId { get { return applicable_op_temporal_id; } set { applicable_op_temporal_id = value; } }
        private uint[][] applicable_op_num_target_views_minus1;
        public uint[][] ApplicableOpNumTargetViewsMinus1 { get { return applicable_op_num_target_views_minus1; } set { applicable_op_num_target_views_minus1 = value; } }
        private uint[][][] applicable_op_target_view_id;
        public uint[][][] ApplicableOpTargetViewId { get { return applicable_op_target_view_id; } set { applicable_op_target_view_id = value; } }
        private byte[][][] applicable_op_depth_flag;
        public byte[][][] ApplicableOpDepthFlag { get { return applicable_op_depth_flag; } set { applicable_op_depth_flag = value; } }
        private byte[][][] applicable_op_texture_flag;
        public byte[][][] ApplicableOpTextureFlag { get { return applicable_op_texture_flag; } set { applicable_op_texture_flag = value; } }
        private uint[][] applicable_op_num_texture_views_minus1;
        public uint[][] ApplicableOpNumTextureViewsMinus1 { get { return applicable_op_num_texture_views_minus1; } set { applicable_op_num_texture_views_minus1 = value; } }
        private uint[][] applicable_op_num_depth_views;
        public uint[][] ApplicableOpNumDepthViews { get { return applicable_op_num_depth_views; } set { applicable_op_num_depth_views = value; } }
        private byte mvcd_vui_parameters_present_flag;
        public byte MvcdVuiParametersPresentFlag { get { return mvcd_vui_parameters_present_flag; } set { mvcd_vui_parameters_present_flag = value; } }
        private MvcdVuiParametersExtension mvcd_vui_parameters_extension;
        public MvcdVuiParametersExtension MvcdVuiParametersExtension { get { return mvcd_vui_parameters_extension; } set { mvcd_vui_parameters_extension = value; } }
        private byte texture_vui_parameters_present_flag;
        public byte TextureVuiParametersPresentFlag { get { return texture_vui_parameters_present_flag; } set { texture_vui_parameters_present_flag = value; } }
        private MvcVuiParametersExtension mvc_vui_parameters_extension;
        public MvcVuiParametersExtension MvcVuiParametersExtension { get { return mvc_vui_parameters_extension; } set { mvc_vui_parameters_extension = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SeqParameterSetMvcdExtension()
        {

        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint[] DepthViewId = null;
            uint j = 0;
            uint k = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.num_views_minus1);

            this.view_id = new uint[num_views_minus1 + 1];
            this.depth_view_present_flag = new byte[num_views_minus1 + 1];
            this.texture_view_present_flag = new byte[num_views_minus1 + 1];
            for (i = 0, NumDepthViews = 0; i <= num_views_minus1; i++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.view_id[i]);
                size += stream.ReadUnsignedInt(size, 1, out this.depth_view_present_flag[i]);
                DepthViewId[NumDepthViews] = view_id[i];
                NumDepthViews += depth_view_present_flag[i];
                size += stream.ReadUnsignedInt(size, 1, out this.texture_view_present_flag[i]);
            }

            this.num_anchor_refs_l0 = new uint[num_views_minus1 + 1];
            this.anchor_ref_l0 = new uint[num_views_minus1 + 1][];
            this.num_anchor_refs_l1 = new uint[num_views_minus1 + 1];
            this.anchor_ref_l1 = new uint[num_views_minus1 + 1][];
            for (i = 1; i <= num_views_minus1; i++)
            {

                if (depth_view_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_anchor_refs_l0[i]);

                    this.anchor_ref_l0[i] = new uint[num_anchor_refs_l0[i]];
                    for (j = 0; j < num_anchor_refs_l0[i]; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.anchor_ref_l0[i][j]);
                    }
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_anchor_refs_l1[i]);

                    this.anchor_ref_l1[i] = new uint[num_anchor_refs_l1[i]];
                    for (j = 0; j < num_anchor_refs_l1[i]; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.anchor_ref_l1[i][j]);
                    }
                }
            }

            this.num_non_anchor_refs_l0 = new uint[num_views_minus1 + 1];
            this.non_anchor_ref_l0 = new uint[num_views_minus1 + 1][];
            this.num_non_anchor_refs_l1 = new uint[num_views_minus1 + 1];
            this.non_anchor_ref_l1 = new uint[num_views_minus1 + 1][];
            for (i = 1; i <= num_views_minus1; i++)
            {

                if (depth_view_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_non_anchor_refs_l0[i]);

                    this.non_anchor_ref_l0[i] = new uint[num_non_anchor_refs_l0[i]];
                    for (j = 0; j < num_non_anchor_refs_l0[i]; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.non_anchor_ref_l0[i][j]);
                    }
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_non_anchor_refs_l1[i]);

                    this.non_anchor_ref_l1[i] = new uint[num_non_anchor_refs_l1[i]];
                    for (j = 0; j < num_non_anchor_refs_l1[i]; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.non_anchor_ref_l1[i][j]);
                    }
                }
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.num_level_values_signalled_minus1);

            this.level_idc = new uint[num_level_values_signalled_minus1 + 1];
            this.num_applicable_ops_minus1 = new uint[num_level_values_signalled_minus1 + 1];
            this.applicable_op_temporal_id = new uint[num_level_values_signalled_minus1 + 1][];
            this.applicable_op_num_target_views_minus1 = new uint[num_level_values_signalled_minus1 + 1][];
            this.applicable_op_target_view_id = new uint[num_level_values_signalled_minus1 + 1][][];
            this.applicable_op_depth_flag = new byte[num_level_values_signalled_minus1 + 1][][];
            this.applicable_op_texture_flag = new byte[num_level_values_signalled_minus1 + 1][][];
            this.applicable_op_num_texture_views_minus1 = new uint[num_level_values_signalled_minus1 + 1][];
            this.applicable_op_num_depth_views = new uint[num_level_values_signalled_minus1 + 1][];
            for (i = 0; i <= num_level_values_signalled_minus1; i++)
            {
                size += stream.ReadUnsignedInt(size, 8, out this.level_idc[i]);
                size += stream.ReadUnsignedIntGolomb(size, out this.num_applicable_ops_minus1[i]);

                this.applicable_op_temporal_id[i] = new uint[num_applicable_ops_minus1[i] + 1];
                this.applicable_op_num_target_views_minus1[i] = new uint[num_applicable_ops_minus1[i] + 1];
                this.applicable_op_target_view_id[i] = new uint[num_applicable_ops_minus1[i] + 1][];
                this.applicable_op_depth_flag[i] = new byte[num_applicable_ops_minus1[i] + 1][];
                this.applicable_op_texture_flag[i] = new byte[num_applicable_ops_minus1[i] + 1][];
                this.applicable_op_num_texture_views_minus1[i] = new uint[num_applicable_ops_minus1[i] + 1];
                this.applicable_op_num_depth_views[i] = new uint[num_applicable_ops_minus1[i] + 1];
                for (j = 0; j <= num_applicable_ops_minus1[i]; j++)
                {
                    size += stream.ReadUnsignedInt(size, 3, out this.applicable_op_temporal_id[i][j]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.applicable_op_num_target_views_minus1[i][j]);

                    this.applicable_op_target_view_id[i][j] = new uint[applicable_op_num_target_views_minus1[i][j] + 1];
                    this.applicable_op_depth_flag[i][j] = new byte[applicable_op_num_target_views_minus1[i][j] + 1];
                    this.applicable_op_texture_flag[i][j] = new byte[applicable_op_num_target_views_minus1[i][j] + 1];
                    for (k = 0; k <= applicable_op_num_target_views_minus1[i][j];
    k++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.applicable_op_target_view_id[i][j][k]);
                        size += stream.ReadUnsignedInt(size, 1, out this.applicable_op_depth_flag[i][j][k]);
                        size += stream.ReadUnsignedInt(size, 1, out this.applicable_op_texture_flag[i][j][k]);
                    }
                    size += stream.ReadUnsignedIntGolomb(size, out this.applicable_op_num_texture_views_minus1[i][j]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.applicable_op_num_depth_views[i][j]);
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.mvcd_vui_parameters_present_flag);

            if (mvcd_vui_parameters_present_flag == 1)
            {
                size += stream.ReadClass<MvcdVuiParametersExtension>(size, context, () => new MvcdVuiParametersExtension(), out this.mvcd_vui_parameters_extension);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.texture_vui_parameters_present_flag);

            if (texture_vui_parameters_present_flag == 1)
            {
                size += stream.ReadClass<MvcVuiParametersExtension>(size, context, () => new MvcVuiParametersExtension(), out this.mvc_vui_parameters_extension);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint[] DepthViewId = null;
            uint j = 0;
            uint k = 0;
            size += stream.WriteUnsignedIntGolomb(this.num_views_minus1);

            for (i = 0, NumDepthViews = 0; i <= num_views_minus1; i++)
            {
                size += stream.WriteUnsignedIntGolomb(this.view_id[i]);
                size += stream.WriteUnsignedInt(1, this.depth_view_present_flag[i]);
                DepthViewId[NumDepthViews] = view_id[i];
                NumDepthViews += depth_view_present_flag[i];
                size += stream.WriteUnsignedInt(1, this.texture_view_present_flag[i]);
            }

            for (i = 1; i <= num_views_minus1; i++)
            {

                if (depth_view_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.num_anchor_refs_l0[i]);

                    for (j = 0; j < num_anchor_refs_l0[i]; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.anchor_ref_l0[i][j]);
                    }
                    size += stream.WriteUnsignedIntGolomb(this.num_anchor_refs_l1[i]);

                    for (j = 0; j < num_anchor_refs_l1[i]; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.anchor_ref_l1[i][j]);
                    }
                }
            }

            for (i = 1; i <= num_views_minus1; i++)
            {

                if (depth_view_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.num_non_anchor_refs_l0[i]);

                    for (j = 0; j < num_non_anchor_refs_l0[i]; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.non_anchor_ref_l0[i][j]);
                    }
                    size += stream.WriteUnsignedIntGolomb(this.num_non_anchor_refs_l1[i]);

                    for (j = 0; j < num_non_anchor_refs_l1[i]; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.non_anchor_ref_l1[i][j]);
                    }
                }
            }
            size += stream.WriteUnsignedIntGolomb(this.num_level_values_signalled_minus1);

            for (i = 0; i <= num_level_values_signalled_minus1; i++)
            {
                size += stream.WriteUnsignedInt(8, this.level_idc[i]);
                size += stream.WriteUnsignedIntGolomb(this.num_applicable_ops_minus1[i]);

                for (j = 0; j <= num_applicable_ops_minus1[i]; j++)
                {
                    size += stream.WriteUnsignedInt(3, this.applicable_op_temporal_id[i][j]);
                    size += stream.WriteUnsignedIntGolomb(this.applicable_op_num_target_views_minus1[i][j]);

                    for (k = 0; k <= applicable_op_num_target_views_minus1[i][j];
    k++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.applicable_op_target_view_id[i][j][k]);
                        size += stream.WriteUnsignedInt(1, this.applicable_op_depth_flag[i][j][k]);
                        size += stream.WriteUnsignedInt(1, this.applicable_op_texture_flag[i][j][k]);
                    }
                    size += stream.WriteUnsignedIntGolomb(this.applicable_op_num_texture_views_minus1[i][j]);
                    size += stream.WriteUnsignedIntGolomb(this.applicable_op_num_depth_views[i][j]);
                }
            }
            size += stream.WriteUnsignedInt(1, this.mvcd_vui_parameters_present_flag);

            if (mvcd_vui_parameters_present_flag == 1)
            {
                size += stream.WriteClass<MvcdVuiParametersExtension>(context, this.mvcd_vui_parameters_extension);
            }
            size += stream.WriteUnsignedInt(1, this.texture_vui_parameters_present_flag);

            if (texture_vui_parameters_present_flag == 1)
            {
                size += stream.WriteClass<MvcVuiParametersExtension>(context, this.mvc_vui_parameters_extension);
            }

            return size;
        }

    }

    /*
  

mvcd_view_scalability_info( payloadSize ) { 
num_operation_points_minus1 5 ue(v) 
for( i = 0; i <= num_operation_points_minus1; i++ ) {   
operation_point_id[ i ] 5 ue(v) 
priority_id[ i ] 5 u(5) 
 temporal_id[ i ] 5 u(3) 
  num_target_output_views_minus1[ i ] 5 ue(v) 
  for( j = 0; j <= num_target_output_views_minus1[ i ]; j++ ) {   
   view_id[ i ][ j ] 5 ue(v) 
   mvcd_op_view_info()   
  }   
  profile_level_info_present_flag[ i ] 5 u(1) 
  bitrate_info_present_flag[ i ] 5 u(1) 
  frm_rate_info_present_flag[ i ] 5 u(1) 
  if( !num_target_output_views_minus1[ i ] )    
   view_dependency_info_present_flag[ i ] 5 u(1) 
  parameter_sets_info_present_flag[ i ] 5 u(1) 
  bitstream_restriction_info_present_flag[ i ] 5 u(1) 
  if( profile_level_info_present_flag[ i ] )   
   op_profile_level_idc[ i ]  5 u(24) 
  if( bitrate_info_present_flag[ i ] ) {   
   avg_bitrate[ i ] 5 u(16) 
   max_bitrate[ i ] 5 u(16) 
   max_bitrate_calc_window[ i ] 5 u(16) 
  }   
  if( frm_rate_info_present_flag[ i ] ) {   
   constant_frm_rate_idc[ i ] 5 u(2) 
   avg_frm_rate[ i ] 5 u(16) 
  }   
  if( view_dependency_info_present_flag[ i ] ) {   
   num_directly_dependent_views[ i ] 5 ue(v) 
   for( j = 0; j < num_directly_dependent_views[ i ]; j++ ) {   
    directly_dependent_view_id[ i ][ j ] 5 ue(v) 
    mvcd_op_view_info()   
   }   
  } else   
   view_dependency_info_src_op_id[ i ] 5 ue(v) 
  if( parameter_sets_info_present_flag[ i ] ) {   
   num_seq_parameter_set_minus1[ i ] 5 ue(v) 
   for( j = 0; j <= num_seq_parameter_set_minus1[ i ]; j++ )   
    seq_parameter_set_id_delta[ i ][ j ] 5 ue(v) 
   num_subset_seq_parameter_set_minus1[ i ] 5 ue(v) 
   for( j = 0; j <= num_subset_seq_parameter_set_minus1[ i ]; j++ )   
    subset_seq_parameter_set_id_delta[ i ][ j ] 5 ue(v) 
   num_pic_parameter_set_minus1[ i ] 5 ue(v) 
   for( j = 0; j <= num_init_pic_parameter_set_minus1[ i ]; j++ )   
    pic_parameter_set_id_delta[ i ][ j ] 5 ue(v) 
  } else   
   parameter_sets_info_src_op_id[ i ] 5 ue(v)
    if( bitstream_restriction_info_present_flag[ i ] ) {   
   motion_vectors_over_pic_boundaries_flag[ i ] 5 u(1) 
   max_bytes_per_pic_denom[ i ] 5 ue(v) 
   max_bits_per_mb_denom[ i ] 5 ue(v) 
   log2_max_mv_length_horizontal[ i ] 5 ue(v) 
   log2_max_mv_length_vertical[ i ] 5 ue(v) 
   num_reorder_frames[ i ] 5 ue(v) 
   max_dec_frame_buffering[ i ] 5 ue(v) 
  }   
 }   
}
    */
    public class MvcdViewScalabilityInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint num_operation_points_minus1;
        public uint NumOperationPointsMinus1 { get { return num_operation_points_minus1; } set { num_operation_points_minus1 = value; } }
        private uint[] operation_point_id;
        public uint[] OperationPointId { get { return operation_point_id; } set { operation_point_id = value; } }
        private uint[] priority_id;
        public uint[] PriorityId { get { return priority_id; } set { priority_id = value; } }
        private uint[] temporal_id;
        public uint[] TemporalId { get { return temporal_id; } set { temporal_id = value; } }
        private uint[] num_target_output_views_minus1;
        public uint[] NumTargetOutputViewsMinus1 { get { return num_target_output_views_minus1; } set { num_target_output_views_minus1 = value; } }
        private uint[][] view_id;
        public uint[][] ViewId { get { return view_id; } set { view_id = value; } }
        private MvcdOpViewInfo[][] mvcd_op_view_info;
        public MvcdOpViewInfo[][] MvcdOpViewInfo { get { return mvcd_op_view_info; } set { mvcd_op_view_info = value; } }
        private byte[] profile_level_info_present_flag;
        public byte[] ProfileLevelInfoPresentFlag { get { return profile_level_info_present_flag; } set { profile_level_info_present_flag = value; } }
        private byte[] bitrate_info_present_flag;
        public byte[] BitrateInfoPresentFlag { get { return bitrate_info_present_flag; } set { bitrate_info_present_flag = value; } }
        private byte[] frm_rate_info_present_flag;
        public byte[] FrmRateInfoPresentFlag { get { return frm_rate_info_present_flag; } set { frm_rate_info_present_flag = value; } }
        private byte[] view_dependency_info_present_flag;
        public byte[] ViewDependencyInfoPresentFlag { get { return view_dependency_info_present_flag; } set { view_dependency_info_present_flag = value; } }
        private byte[] parameter_sets_info_present_flag;
        public byte[] ParameterSetsInfoPresentFlag { get { return parameter_sets_info_present_flag; } set { parameter_sets_info_present_flag = value; } }
        private byte[] bitstream_restriction_info_present_flag;
        public byte[] BitstreamRestrictionInfoPresentFlag { get { return bitstream_restriction_info_present_flag; } set { bitstream_restriction_info_present_flag = value; } }
        private uint[] op_profile_level_idc;
        public uint[] OpProfileLevelIdc { get { return op_profile_level_idc; } set { op_profile_level_idc = value; } }
        private uint[] avg_bitrate;
        public uint[] AvgBitrate { get { return avg_bitrate; } set { avg_bitrate = value; } }
        private uint[] max_bitrate;
        public uint[] MaxBitrate { get { return max_bitrate; } set { max_bitrate = value; } }
        private uint[] max_bitrate_calc_window;
        public uint[] MaxBitrateCalcWindow { get { return max_bitrate_calc_window; } set { max_bitrate_calc_window = value; } }
        private uint[] constant_frm_rate_idc;
        public uint[] ConstantFrmRateIdc { get { return constant_frm_rate_idc; } set { constant_frm_rate_idc = value; } }
        private uint[] avg_frm_rate;
        public uint[] AvgFrmRate { get { return avg_frm_rate; } set { avg_frm_rate = value; } }
        private uint[] num_directly_dependent_views;
        public uint[] NumDirectlyDependentViews { get { return num_directly_dependent_views; } set { num_directly_dependent_views = value; } }
        private uint[][] directly_dependent_view_id;
        public uint[][] DirectlyDependentViewId { get { return directly_dependent_view_id; } set { directly_dependent_view_id = value; } }
        private uint[] view_dependency_info_src_op_id;
        public uint[] ViewDependencyInfoSrcOpId { get { return view_dependency_info_src_op_id; } set { view_dependency_info_src_op_id = value; } }
        private uint[] num_seq_parameter_set_minus1;
        public uint[] NumSeqParameterSetMinus1 { get { return num_seq_parameter_set_minus1; } set { num_seq_parameter_set_minus1 = value; } }
        private uint[][] seq_parameter_set_id_delta;
        public uint[][] SeqParameterSetIdDelta { get { return seq_parameter_set_id_delta; } set { seq_parameter_set_id_delta = value; } }
        private uint[] num_subset_seq_parameter_set_minus1;
        public uint[] NumSubsetSeqParameterSetMinus1 { get { return num_subset_seq_parameter_set_minus1; } set { num_subset_seq_parameter_set_minus1 = value; } }
        private uint[][] subset_seq_parameter_set_id_delta;
        public uint[][] SubsetSeqParameterSetIdDelta { get { return subset_seq_parameter_set_id_delta; } set { subset_seq_parameter_set_id_delta = value; } }
        private uint[] num_pic_parameter_set_minus1;
        public uint[] NumPicParameterSetMinus1 { get { return num_pic_parameter_set_minus1; } set { num_pic_parameter_set_minus1 = value; } }
        private uint[][] pic_parameter_set_id_delta;
        public uint[][] PicParameterSetIdDelta { get { return pic_parameter_set_id_delta; } set { pic_parameter_set_id_delta = value; } }
        private uint[] parameter_sets_info_src_op_id;
        public uint[] ParameterSetsInfoSrcOpId { get { return parameter_sets_info_src_op_id; } set { parameter_sets_info_src_op_id = value; } }
        private byte[] motion_vectors_over_pic_boundaries_flag;
        public byte[] MotionVectorsOverPicBoundariesFlag { get { return motion_vectors_over_pic_boundaries_flag; } set { motion_vectors_over_pic_boundaries_flag = value; } }
        private uint[] max_bytes_per_pic_denom;
        public uint[] MaxBytesPerPicDenom { get { return max_bytes_per_pic_denom; } set { max_bytes_per_pic_denom = value; } }
        private uint[] max_bits_per_mb_denom;
        public uint[] MaxBitsPerMbDenom { get { return max_bits_per_mb_denom; } set { max_bits_per_mb_denom = value; } }
        private uint[] log2_max_mv_length_horizontal;
        public uint[] Log2MaxMvLengthHorizontal { get { return log2_max_mv_length_horizontal; } set { log2_max_mv_length_horizontal = value; } }
        private uint[] log2_max_mv_length_vertical;
        public uint[] Log2MaxMvLengthVertical { get { return log2_max_mv_length_vertical; } set { log2_max_mv_length_vertical = value; } }
        private uint[] num_reorder_frames;
        public uint[] NumReorderFrames { get { return num_reorder_frames; } set { num_reorder_frames = value; } }
        private uint[] max_dec_frame_buffering;
        public uint[] MaxDecFrameBuffering { get { return max_dec_frame_buffering; } set { max_dec_frame_buffering = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public MvcdViewScalabilityInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.num_operation_points_minus1);

            this.operation_point_id = new uint[num_operation_points_minus1 + 1];
            this.priority_id = new uint[num_operation_points_minus1 + 1];
            this.temporal_id = new uint[num_operation_points_minus1 + 1];
            this.num_target_output_views_minus1 = new uint[num_operation_points_minus1 + 1];
            this.view_id = new uint[num_operation_points_minus1 + 1][];
            this.mvcd_op_view_info = new MvcdOpViewInfo[num_operation_points_minus1 + 1][];
            this.profile_level_info_present_flag = new byte[num_operation_points_minus1 + 1];
            this.bitrate_info_present_flag = new byte[num_operation_points_minus1 + 1];
            this.frm_rate_info_present_flag = new byte[num_operation_points_minus1 + 1];
            this.view_dependency_info_present_flag = new byte[num_operation_points_minus1 + 1];
            this.parameter_sets_info_present_flag = new byte[num_operation_points_minus1 + 1];
            this.bitstream_restriction_info_present_flag = new byte[num_operation_points_minus1 + 1];
            this.op_profile_level_idc = new uint[num_operation_points_minus1 + 1];
            this.avg_bitrate = new uint[num_operation_points_minus1 + 1];
            this.max_bitrate = new uint[num_operation_points_minus1 + 1];
            this.max_bitrate_calc_window = new uint[num_operation_points_minus1 + 1];
            this.constant_frm_rate_idc = new uint[num_operation_points_minus1 + 1];
            this.avg_frm_rate = new uint[num_operation_points_minus1 + 1];
            this.num_directly_dependent_views = new uint[num_operation_points_minus1 + 1];
            this.directly_dependent_view_id = new uint[num_operation_points_minus1 + 1][];
            this.view_dependency_info_src_op_id = new uint[num_operation_points_minus1 + 1];
            this.num_seq_parameter_set_minus1 = new uint[num_operation_points_minus1 + 1];
            this.seq_parameter_set_id_delta = new uint[num_operation_points_minus1 + 1][];
            this.num_subset_seq_parameter_set_minus1 = new uint[num_operation_points_minus1 + 1];
            this.subset_seq_parameter_set_id_delta = new uint[num_operation_points_minus1 + 1][];
            this.num_pic_parameter_set_minus1 = new uint[num_operation_points_minus1 + 1];
            this.pic_parameter_set_id_delta = new uint[num_operation_points_minus1 + 1][];
            this.parameter_sets_info_src_op_id = new uint[num_operation_points_minus1 + 1];
            this.motion_vectors_over_pic_boundaries_flag = new byte[num_operation_points_minus1 + 1];
            this.max_bytes_per_pic_denom = new uint[num_operation_points_minus1 + 1];
            this.max_bits_per_mb_denom = new uint[num_operation_points_minus1 + 1];
            this.log2_max_mv_length_horizontal = new uint[num_operation_points_minus1 + 1];
            this.log2_max_mv_length_vertical = new uint[num_operation_points_minus1 + 1];
            this.num_reorder_frames = new uint[num_operation_points_minus1 + 1];
            this.max_dec_frame_buffering = new uint[num_operation_points_minus1 + 1];
            for (i = 0; i <= num_operation_points_minus1; i++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.operation_point_id[i]);
                size += stream.ReadUnsignedInt(size, 5, out this.priority_id[i]);
                size += stream.ReadUnsignedInt(size, 3, out this.temporal_id[i]);
                size += stream.ReadUnsignedIntGolomb(size, out this.num_target_output_views_minus1[i]);

                this.view_id[i] = new uint[num_target_output_views_minus1[i] + 1];
                this.mvcd_op_view_info[i] = new MvcdOpViewInfo[num_target_output_views_minus1[i] + 1];
                for (j = 0; j <= num_target_output_views_minus1[i]; j++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.view_id[i][j]);
                    size += stream.ReadClass<MvcdOpViewInfo>(size, context, () => new MvcdOpViewInfo(), out this.mvcd_op_view_info[i][j]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.profile_level_info_present_flag[i]);
                size += stream.ReadUnsignedInt(size, 1, out this.bitrate_info_present_flag[i]);
                size += stream.ReadUnsignedInt(size, 1, out this.frm_rate_info_present_flag[i]);

                if (num_target_output_views_minus1[i] == 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.view_dependency_info_present_flag[i]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.parameter_sets_info_present_flag[i]);
                size += stream.ReadUnsignedInt(size, 1, out this.bitstream_restriction_info_present_flag[i]);

                if (profile_level_info_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 24, out this.op_profile_level_idc[i]);
                }

                if (bitrate_info_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 16, out this.avg_bitrate[i]);
                    size += stream.ReadUnsignedInt(size, 16, out this.max_bitrate[i]);
                    size += stream.ReadUnsignedInt(size, 16, out this.max_bitrate_calc_window[i]);
                }

                if (frm_rate_info_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 2, out this.constant_frm_rate_idc[i]);
                    size += stream.ReadUnsignedInt(size, 16, out this.avg_frm_rate[i]);
                }

                if (view_dependency_info_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_directly_dependent_views[i]);

                    this.directly_dependent_view_id[i] = new uint[num_directly_dependent_views[i]];
                    for (j = 0; j < num_directly_dependent_views[i]; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.directly_dependent_view_id[i][j]);
                        size += stream.ReadClass<MvcdOpViewInfo>(size, context, () => new MvcdOpViewInfo(), out this.mvcd_op_view_info[i][j]);
                    }
                }
                else
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.view_dependency_info_src_op_id[i]);
                }

                if (parameter_sets_info_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_seq_parameter_set_minus1[i]);

                    this.seq_parameter_set_id_delta[i] = new uint[num_seq_parameter_set_minus1[i] + 1];
                    for (j = 0; j <= num_seq_parameter_set_minus1[i]; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.seq_parameter_set_id_delta[i][j]);
                    }
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_subset_seq_parameter_set_minus1[i]);

                    this.subset_seq_parameter_set_id_delta[i] = new uint[num_subset_seq_parameter_set_minus1[i] + 1];
                    for (j = 0; j <= num_subset_seq_parameter_set_minus1[i]; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.subset_seq_parameter_set_id_delta[i][j]);
                    }
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_pic_parameter_set_minus1[i]);

                    this.pic_parameter_set_id_delta[i] = new uint[context.Sei.SeiMessage.SeiPayload.MvcdViewScalabilityInfo.NumPicParameterSetMinus1[i] + 1];
                    for (j = 0; j <= context.Sei.SeiMessage.SeiPayload.MvcdViewScalabilityInfo.NumPicParameterSetMinus1[i]; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.pic_parameter_set_id_delta[i][j]);
                    }
                }
                else
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.parameter_sets_info_src_op_id[i]);
                }

                if (bitstream_restriction_info_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.motion_vectors_over_pic_boundaries_flag[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.max_bytes_per_pic_denom[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.max_bits_per_mb_denom[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.log2_max_mv_length_horizontal[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.log2_max_mv_length_vertical[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_reorder_frames[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.max_dec_frame_buffering[i]);
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.WriteUnsignedIntGolomb(this.num_operation_points_minus1);

            for (i = 0; i <= num_operation_points_minus1; i++)
            {
                size += stream.WriteUnsignedIntGolomb(this.operation_point_id[i]);
                size += stream.WriteUnsignedInt(5, this.priority_id[i]);
                size += stream.WriteUnsignedInt(3, this.temporal_id[i]);
                size += stream.WriteUnsignedIntGolomb(this.num_target_output_views_minus1[i]);

                for (j = 0; j <= num_target_output_views_minus1[i]; j++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.view_id[i][j]);
                    size += stream.WriteClass<MvcdOpViewInfo>(context, this.mvcd_op_view_info[i][j]);
                }
                size += stream.WriteUnsignedInt(1, this.profile_level_info_present_flag[i]);
                size += stream.WriteUnsignedInt(1, this.bitrate_info_present_flag[i]);
                size += stream.WriteUnsignedInt(1, this.frm_rate_info_present_flag[i]);

                if (num_target_output_views_minus1[i] == 0)
                {
                    size += stream.WriteUnsignedInt(1, this.view_dependency_info_present_flag[i]);
                }
                size += stream.WriteUnsignedInt(1, this.parameter_sets_info_present_flag[i]);
                size += stream.WriteUnsignedInt(1, this.bitstream_restriction_info_present_flag[i]);

                if (profile_level_info_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(24, this.op_profile_level_idc[i]);
                }

                if (bitrate_info_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(16, this.avg_bitrate[i]);
                    size += stream.WriteUnsignedInt(16, this.max_bitrate[i]);
                    size += stream.WriteUnsignedInt(16, this.max_bitrate_calc_window[i]);
                }

                if (frm_rate_info_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(2, this.constant_frm_rate_idc[i]);
                    size += stream.WriteUnsignedInt(16, this.avg_frm_rate[i]);
                }

                if (view_dependency_info_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.num_directly_dependent_views[i]);

                    for (j = 0; j < num_directly_dependent_views[i]; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.directly_dependent_view_id[i][j]);
                        size += stream.WriteClass<MvcdOpViewInfo>(context, this.mvcd_op_view_info[i][j]);
                    }
                }
                else
                {
                    size += stream.WriteUnsignedIntGolomb(this.view_dependency_info_src_op_id[i]);
                }

                if (parameter_sets_info_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.num_seq_parameter_set_minus1[i]);

                    for (j = 0; j <= num_seq_parameter_set_minus1[i]; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.seq_parameter_set_id_delta[i][j]);
                    }
                    size += stream.WriteUnsignedIntGolomb(this.num_subset_seq_parameter_set_minus1[i]);

                    for (j = 0; j <= num_subset_seq_parameter_set_minus1[i]; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.subset_seq_parameter_set_id_delta[i][j]);
                    }
                    size += stream.WriteUnsignedIntGolomb(this.num_pic_parameter_set_minus1[i]);

                    for (j = 0; j <= context.Sei.SeiMessage.SeiPayload.MvcdViewScalabilityInfo.NumPicParameterSetMinus1[i]; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.pic_parameter_set_id_delta[i][j]);
                    }
                }
                else
                {
                    size += stream.WriteUnsignedIntGolomb(this.parameter_sets_info_src_op_id[i]);
                }

                if (bitstream_restriction_info_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.motion_vectors_over_pic_boundaries_flag[i]);
                    size += stream.WriteUnsignedIntGolomb(this.max_bytes_per_pic_denom[i]);
                    size += stream.WriteUnsignedIntGolomb(this.max_bits_per_mb_denom[i]);
                    size += stream.WriteUnsignedIntGolomb(this.log2_max_mv_length_horizontal[i]);
                    size += stream.WriteUnsignedIntGolomb(this.log2_max_mv_length_vertical[i]);
                    size += stream.WriteUnsignedIntGolomb(this.num_reorder_frames[i]);
                    size += stream.WriteUnsignedIntGolomb(this.max_dec_frame_buffering[i]);
                }
            }

            return size;
        }

    }

    /*
   

mvcd_op_view_info() {  
 view_info_depth_view_present_flag 5 u(1) 
 if( view_info_depth_view_present_flag )   
  mvcd_depth_view_flag 5 u(1) 
 view_info_texture_view_present_flag 5 u(1) 
 if( view_info_texture_view_present_flag )   
  mvcd_texture_view_flag 5 u(1) 
}
    */
    public class MvcdOpViewInfo : IItuSerializable
    {
        private byte view_info_depth_view_present_flag;
        public byte ViewInfoDepthViewPresentFlag { get { return view_info_depth_view_present_flag; } set { view_info_depth_view_present_flag = value; } }
        private byte mvcd_depth_view_flag;
        public byte MvcdDepthViewFlag { get { return mvcd_depth_view_flag; } set { mvcd_depth_view_flag = value; } }
        private byte view_info_texture_view_present_flag;
        public byte ViewInfoTextureViewPresentFlag { get { return view_info_texture_view_present_flag; } set { view_info_texture_view_present_flag = value; } }
        private byte mvcd_texture_view_flag;
        public byte MvcdTextureViewFlag { get { return mvcd_texture_view_flag; } set { mvcd_texture_view_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public MvcdOpViewInfo()
        {

        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.view_info_depth_view_present_flag);

            if (view_info_depth_view_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.mvcd_depth_view_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.view_info_texture_view_present_flag);

            if (view_info_texture_view_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.mvcd_texture_view_flag);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.view_info_depth_view_present_flag);

            if (view_info_depth_view_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.mvcd_depth_view_flag);
            }
            size += stream.WriteUnsignedInt(1, this.view_info_texture_view_present_flag);

            if (view_info_texture_view_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.mvcd_texture_view_flag);
            }

            return size;
        }

    }

    /*
  

mvcd_scalable_nesting( payloadSize ) {  
 operation_point_flag 5 u(1) 
 if( !operation_point_flag ) {   
  all_view_components_in_au_flag 5 u(1) 
  if( !all_view_components_in_au_flag ) {   
   num_view_components_minus1 5 ue(v) 
   for( i = 0; i <= num_view_components_minus1; i++ ) {   
    sei_view_id[ i ] 5 u(10) 
    sei_view_applicability_flag[ i ] 5 u(1) 
   }   
  }   
 } else {   
  sei_op_texture_only_flag 5 u(1) 
  num_view_components_op_minus1 5 ue(v) 
  for( i = 0; i <= num_view_components_op_minus1; i++ ) {   
   sei_op_view_id[ i ] 5 u(10) 
   if( !sei_op_texture_only_flag ) {   
    sei_op_depth_flag[ i ] 5 u(1) 
    sei_op_texture_flag[ i ] 5 u(1) 
   }   
  }   
  sei_op_temporal_id 5 u(3) 
 }   
 while( !byte_aligned() )   
  sei_nesting_zero_bit /* equal to 0 *//* 5 f(1) 
 sei_message() 5  
}
    */
    public class MvcdScalableNesting : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte operation_point_flag;
        public byte OperationPointFlag { get { return operation_point_flag; } set { operation_point_flag = value; } }
        private byte all_view_components_in_au_flag;
        public byte AllViewComponentsInAuFlag { get { return all_view_components_in_au_flag; } set { all_view_components_in_au_flag = value; } }
        private uint num_view_components_minus1;
        public uint NumViewComponentsMinus1 { get { return num_view_components_minus1; } set { num_view_components_minus1 = value; } }
        private uint[] sei_view_id;
        public uint[] SeiViewId { get { return sei_view_id; } set { sei_view_id = value; } }
        private byte[] sei_view_applicability_flag;
        public byte[] SeiViewApplicabilityFlag { get { return sei_view_applicability_flag; } set { sei_view_applicability_flag = value; } }
        private byte sei_op_texture_only_flag;
        public byte SeiOpTextureOnlyFlag { get { return sei_op_texture_only_flag; } set { sei_op_texture_only_flag = value; } }
        private uint num_view_components_op_minus1;
        public uint NumViewComponentsOpMinus1 { get { return num_view_components_op_minus1; } set { num_view_components_op_minus1 = value; } }
        private uint[] sei_op_view_id;
        public uint[] SeiOpViewId { get { return sei_op_view_id; } set { sei_op_view_id = value; } }
        private byte[] sei_op_depth_flag;
        public byte[] SeiOpDepthFlag { get { return sei_op_depth_flag; } set { sei_op_depth_flag = value; } }
        private byte[] sei_op_texture_flag;
        public byte[] SeiOpTextureFlag { get { return sei_op_texture_flag; } set { sei_op_texture_flag = value; } }
        private uint sei_op_temporal_id;
        public uint SeiOpTemporalId { get { return sei_op_temporal_id; } set { sei_op_temporal_id = value; } }
        private uint sei_nesting_zero_bit;
        public uint SeiNestingZeroBit { get { return sei_nesting_zero_bit; } set { sei_nesting_zero_bit = value; } }
        private SeiMessage sei_message;
        public SeiMessage SeiMessage { get { return sei_message; } set { sei_message = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public MvcdScalableNesting(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.operation_point_flag);

            if (operation_point_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.all_view_components_in_au_flag);

                if (all_view_components_in_au_flag == 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_view_components_minus1);

                    this.sei_view_id = new uint[num_view_components_minus1 + 1];
                    this.sei_view_applicability_flag = new byte[num_view_components_minus1 + 1];
                    for (i = 0; i <= num_view_components_minus1; i++)
                    {
                        size += stream.ReadUnsignedInt(size, 10, out this.sei_view_id[i]);
                        size += stream.ReadUnsignedInt(size, 1, out this.sei_view_applicability_flag[i]);
                    }
                }
            }
            else
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sei_op_texture_only_flag);
                size += stream.ReadUnsignedIntGolomb(size, out this.num_view_components_op_minus1);

                this.sei_op_view_id = new uint[num_view_components_op_minus1 + 1];
                this.sei_op_depth_flag = new byte[num_view_components_op_minus1 + 1];
                this.sei_op_texture_flag = new byte[num_view_components_op_minus1 + 1];
                for (i = 0; i <= num_view_components_op_minus1; i++)
                {
                    size += stream.ReadUnsignedInt(size, 10, out this.sei_op_view_id[i]);

                    if (sei_op_texture_only_flag == 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.sei_op_depth_flag[i]);
                        size += stream.ReadUnsignedInt(size, 1, out this.sei_op_texture_flag[i]);
                    }
                }
                size += stream.ReadUnsignedInt(size, 3, out this.sei_op_temporal_id);
            }

            while (!stream.ByteAligned())
            {
                size += stream.ReadFixed(size, 1, out this.sei_nesting_zero_bit); // equal to 0 
            }
            size += stream.ReadClass<SeiMessage>(size, context, () => new SeiMessage(), out this.sei_message);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedInt(1, this.operation_point_flag);

            if (operation_point_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.all_view_components_in_au_flag);

                if (all_view_components_in_au_flag == 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.num_view_components_minus1);

                    for (i = 0; i <= num_view_components_minus1; i++)
                    {
                        size += stream.WriteUnsignedInt(10, this.sei_view_id[i]);
                        size += stream.WriteUnsignedInt(1, this.sei_view_applicability_flag[i]);
                    }
                }
            }
            else
            {
                size += stream.WriteUnsignedInt(1, this.sei_op_texture_only_flag);
                size += stream.WriteUnsignedIntGolomb(this.num_view_components_op_minus1);

                for (i = 0; i <= num_view_components_op_minus1; i++)
                {
                    size += stream.WriteUnsignedInt(10, this.sei_op_view_id[i]);

                    if (sei_op_texture_only_flag == 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.sei_op_depth_flag[i]);
                        size += stream.WriteUnsignedInt(1, this.sei_op_texture_flag[i]);
                    }
                }
                size += stream.WriteUnsignedInt(3, this.sei_op_temporal_id);
            }

            while (!stream.ByteAligned())
            {
                size += stream.WriteFixed(1, this.sei_nesting_zero_bit); // equal to 0 
            }
            size += stream.WriteClass<SeiMessage>(context, this.sei_message);

            return size;
        }

    }

    /*
   

depth_representation_info( payloadSize ) {  
 all_views_equal_flag 5 u(1) 
 if( all_views_equal_flag  ==  0 ) {   
  num_views_minus1 5 ue(v) 
  numViews = num_views_minus1 + 1   
 } else   
  numViews = 1   
 z_near_flag 5 u(1) 
 z_far_flag 5 u(1) 
 if( z_near_flag  ||  z_far_flag ) {   
  z_axis_equal_flag 5 u(1) 
  if( z_axis_equal_flag )   
   common_z_axis_reference_view 5 ue(v) 
 }   
 d_min_flag 5 u(1) 
 d_max_flag 5 u(1) 
 depth_representation_type 5 ue(v) 
 for( i = 0; i < numViews; i++ ) {   
  depth_info_view_id[ i ] 5 ue(v) 
  if( ( z_near_flag  ||  z_far_flag )  &&  ( z_axis_equal_flag  ==  0 ) )   
   z_axis_reference_view[ i ] 5 ue(v) 
  if( d_min_flag  ||  d_max_flag )   
   disparity_reference_view[ i ] 5 ue(v) 
  if( z_near_flag )   
   depth_representation_sei_element( ZNearSign, ZNearExp, 
             ZNearMantissa, ZNearManLen ) 
  
  if( z_far_flag )   
   depth_representation_sei_element( ZFarSign, ZFarExp, 
             ZFarMantissa, ZFarManLen ) 
  
  if( d_min_flag )   
   depth_representation_sei_element( DMinSign, DMinExp, 
             DMinMantissa, DMinManLen ) 
  
  if( d_max_flag )   
   depth_representation_sei_element( DMaxSign, DMaxExp, 
             DMaxMantissa, DMaxManLen ) 
  
 }   
 if( depth_representation_type  ==  3 ) {   
  depth_nonlinear_representation_num_minus1 5 ue(v) 
  for( i = 1; i <= depth_nonlinear_representation_num_minus1 + 1; i++ )   
   depth_nonlinear_representation_model[ i ] 5 ue(v) 
 }   
}
    */
    public class DepthRepresentationInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte all_views_equal_flag;
        public byte AllViewsEqualFlag { get { return all_views_equal_flag; } set { all_views_equal_flag = value; } }
        private uint num_views_minus1;
        public uint NumViewsMinus1 { get { return num_views_minus1; } set { num_views_minus1 = value; } }
        private byte z_near_flag;
        public byte zNearFlag { get { return z_near_flag; } set { z_near_flag = value; } }
        private byte z_far_flag;
        public byte zFarFlag { get { return z_far_flag; } set { z_far_flag = value; } }
        private byte z_axis_equal_flag;
        public byte zAxisEqualFlag { get { return z_axis_equal_flag; } set { z_axis_equal_flag = value; } }
        private uint common_z_axis_reference_view;
        public uint CommonzAxisReferenceView { get { return common_z_axis_reference_view; } set { common_z_axis_reference_view = value; } }
        private byte d_min_flag;
        public byte dMinFlag { get { return d_min_flag; } set { d_min_flag = value; } }
        private byte d_max_flag;
        public byte dMaxFlag { get { return d_max_flag; } set { d_max_flag = value; } }
        private uint depth_representation_type;
        public uint DepthRepresentationType { get { return depth_representation_type; } set { depth_representation_type = value; } }
        private uint[] depth_info_view_id;
        public uint[] DepthInfoViewId { get { return depth_info_view_id; } set { depth_info_view_id = value; } }
        private uint[] z_axis_reference_view;
        public uint[] zAxisReferenceView { get { return z_axis_reference_view; } set { z_axis_reference_view = value; } }
        private uint[] disparity_reference_view;
        public uint[] DisparityReferenceView { get { return disparity_reference_view; } set { disparity_reference_view = value; } }
        private DepthRepresentationSeiElement[] depth_representation_sei_element;
        public DepthRepresentationSeiElement[] DepthRepresentationSeiElement { get { return depth_representation_sei_element; } set { depth_representation_sei_element = value; } }
        private uint depth_nonlinear_representation_num_minus1;
        public uint DepthNonlinearRepresentationNumMinus1 { get { return depth_nonlinear_representation_num_minus1; } set { depth_nonlinear_representation_num_minus1 = value; } }
        private uint[] depth_nonlinear_representation_model;
        public uint[] DepthNonlinearRepresentationModel { get { return depth_nonlinear_representation_model; } set { depth_nonlinear_representation_model = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public DepthRepresentationInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint numViews = 0;
            uint i = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.all_views_equal_flag);

            if (all_views_equal_flag == 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.num_views_minus1);
                numViews = num_views_minus1 + 1;
            }
            else
            {
                numViews = 1;
            }
            size += stream.ReadUnsignedInt(size, 1, out this.z_near_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.z_far_flag);

            if (z_near_flag != 0 || z_far_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.z_axis_equal_flag);

                if (z_axis_equal_flag != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.common_z_axis_reference_view);
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.d_min_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.d_max_flag);
            size += stream.ReadUnsignedIntGolomb(size, out this.depth_representation_type);

            this.depth_info_view_id = new uint[numViews];
            this.z_axis_reference_view = new uint[numViews];
            this.disparity_reference_view = new uint[numViews];
            this.depth_representation_sei_element = new DepthRepresentationSeiElement[numViews];
            for (i = 0; i < numViews; i++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.depth_info_view_id[i]);

                if ((z_near_flag != 0 || z_far_flag != 0) && (z_axis_equal_flag == 0))
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.z_axis_reference_view[i]);
                }

                if (d_min_flag != 0 || d_max_flag != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.disparity_reference_view[i]);
                }

                if (z_near_flag != 0)
                {
                    size += stream.ReadClass<DepthRepresentationSeiElement>(size, context, () => new DepthRepresentationSeiElement(H264Helpers.GetArray2("ZNearSign"), H264Helpers.GetArray2("ZNearExp"),
             H264Helpers.GetArray2("ZNearMantissa"), H264Helpers.GetArray2("ZNearManLen")), out this.depth_representation_sei_element[i]);
                }

                if (z_far_flag != 0)
                {
                    size += stream.ReadClass<DepthRepresentationSeiElement>(size, context, () => new DepthRepresentationSeiElement(H264Helpers.GetArray2("ZFarSign"), H264Helpers.GetArray2("ZFarExp"),
             H264Helpers.GetArray2("ZFarMantissa"), H264Helpers.GetArray2("ZFarManLen")), out this.depth_representation_sei_element[i]);
                }

                if (d_min_flag != 0)
                {
                    size += stream.ReadClass<DepthRepresentationSeiElement>(size, context, () => new DepthRepresentationSeiElement(H264Helpers.GetArray2("DMinSign"), H264Helpers.GetArray2("DMinExp"),
             H264Helpers.GetArray2("DMinMantissa"), H264Helpers.GetArray2("DMinManLen")), out this.depth_representation_sei_element[i]);
                }

                if (d_max_flag != 0)
                {
                    size += stream.ReadClass<DepthRepresentationSeiElement>(size, context, () => new DepthRepresentationSeiElement(H264Helpers.GetArray2("DMaxSign"), H264Helpers.GetArray2("DMaxExp"),
             H264Helpers.GetArray2("DMaxMantissa"), H264Helpers.GetArray2("DMaxManLen")), out this.depth_representation_sei_element[i]);
                }
            }

            if (depth_representation_type == 3)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.depth_nonlinear_representation_num_minus1);

                this.depth_nonlinear_representation_model = new uint[depth_nonlinear_representation_num_minus1 + 1 + 1];
                for (i = 1; i <= depth_nonlinear_representation_num_minus1 + 1; i++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.depth_nonlinear_representation_model[i]);
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint numViews = 0;
            uint i = 0;
            size += stream.WriteUnsignedInt(1, this.all_views_equal_flag);

            if (all_views_equal_flag == 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.num_views_minus1);
                numViews = num_views_minus1 + 1;
            }
            else
            {
                numViews = 1;
            }
            size += stream.WriteUnsignedInt(1, this.z_near_flag);
            size += stream.WriteUnsignedInt(1, this.z_far_flag);

            if (z_near_flag != 0 || z_far_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.z_axis_equal_flag);

                if (z_axis_equal_flag != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.common_z_axis_reference_view);
                }
            }
            size += stream.WriteUnsignedInt(1, this.d_min_flag);
            size += stream.WriteUnsignedInt(1, this.d_max_flag);
            size += stream.WriteUnsignedIntGolomb(this.depth_representation_type);

            for (i = 0; i < numViews; i++)
            {
                size += stream.WriteUnsignedIntGolomb(this.depth_info_view_id[i]);

                if ((z_near_flag != 0 || z_far_flag != 0) && (z_axis_equal_flag == 0))
                {
                    size += stream.WriteUnsignedIntGolomb(this.z_axis_reference_view[i]);
                }

                if (d_min_flag != 0 || d_max_flag != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.disparity_reference_view[i]);
                }

                if (z_near_flag != 0)
                {
                    size += stream.WriteClass<DepthRepresentationSeiElement>(context, this.depth_representation_sei_element[i]);
                }

                if (z_far_flag != 0)
                {
                    size += stream.WriteClass<DepthRepresentationSeiElement>(context, this.depth_representation_sei_element[i]);
                }

                if (d_min_flag != 0)
                {
                    size += stream.WriteClass<DepthRepresentationSeiElement>(context, this.depth_representation_sei_element[i]);
                }

                if (d_max_flag != 0)
                {
                    size += stream.WriteClass<DepthRepresentationSeiElement>(context, this.depth_representation_sei_element[i]);
                }
            }

            if (depth_representation_type == 3)
            {
                size += stream.WriteUnsignedIntGolomb(this.depth_nonlinear_representation_num_minus1);

                for (i = 1; i <= depth_nonlinear_representation_num_minus1 + 1; i++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.depth_nonlinear_representation_model[i]);
                }
            }

            return size;
        }

    }

    /*
   

depth_representation_sei_element( outSign, outExp, outMantissa, 
 outManLen ) { 
 da_sign_flag 5 u(1) 
 da_exponent 5 u(7) 
 da_mantissa_len_minus1 5 u(5) 
 da_mantissa 5 u(v) 
}
    */
    public class DepthRepresentationSeiElement : IItuSerializable
    {
        private uint[,] outSign;
        public uint[,] OutSign { get { return outSign; } set { outSign = value; } }
        private uint[,] outExp;
        public uint[,] OutExp { get { return outExp; } set { outExp = value; } }
        private uint[,] outMantissa;
        public uint[,] OutMantissa { get { return outMantissa; } set { outMantissa = value; } }
        private uint[,] outManLen;
        public uint[,] OutManLen { get { return outManLen; } set { outManLen = value; } }
        private byte da_sign_flag;
        public byte DaSignFlag { get { return da_sign_flag; } set { da_sign_flag = value; } }
        private uint da_exponent;
        public uint DaExponent { get { return da_exponent; } set { da_exponent = value; } }
        private uint da_mantissa_len_minus1;
        public uint DaMantissaLenMinus1 { get { return da_mantissa_len_minus1; } set { da_mantissa_len_minus1 = value; } }
        private uint da_mantissa;
        public uint DaMantissa { get { return da_mantissa; } set { da_mantissa = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public DepthRepresentationSeiElement(uint[,] outSign, uint[,] outExp, uint[,] outMantissa, uint[,] outManLen)
        {
            this.outSign = outSign;
            this.outExp = outExp;
            this.outMantissa = outMantissa;
            this.outManLen = outManLen;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.da_sign_flag);
            size += stream.ReadUnsignedInt(size, 7, out this.da_exponent);
            size += stream.ReadUnsignedInt(size, 5, out this.da_mantissa_len_minus1);
            size += stream.ReadUnsignedIntVariable(size, (this.da_mantissa_len_minus1 + 1), out this.da_mantissa);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.da_sign_flag);
            size += stream.WriteUnsignedInt(7, this.da_exponent);
            size += stream.WriteUnsignedInt(5, this.da_mantissa_len_minus1);
            size += stream.WriteUnsignedIntVariable((this.da_mantissa_len_minus1 + 1), this.da_mantissa);

            return size;
        }

    }

    /*
   

three_dimensional_reference_displays_info( payloadSize ) {  
 prec_ref_baseline 5 ue(v) 
 prec_ref_display_width 5 ue(v) 
 ref_viewing_distance_flag 5 u(1) 
 if( ref_viewing_distance_flag )   
  prec_ref_viewing_dist 5 ue(v) 
 num_ref_displays_minus1 5 ue(v) 
 numRefDisplays  = num_ref_displays_minus1 + 1   
 for( i = 0; i < numRefDisplays; i++ ) {   
  exponent_ref_baseline[ i ] 5 u(6) 
  mantissa_ref_baseline[ i ] 5 u(v) 
  exponent_ref_display_width[ i ] 5 u(6) 
  mantissa_ref_display_width[ i ] 5 u(v) 
  if( ref_viewing_distance_flag ) {   
   exponent_ref_viewing_distance[ i ] 5 u(6) 
   mantissa_ref_viewing_distance[ i ] 5 u(v) 
  }   
  additional_shift_present_flag[ i ] 5 u(1) 
  if( additional_shift_present[ i ] )   
   num_sample_shift_plus512[ i ] 5 u(10) 
 }   
 three_dimensional_reference_displays_extension_flag 5 u(1) 
}
    */
    public class ThreeDimensionalReferenceDisplaysInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint prec_ref_baseline;
        public uint PrecRefBaseline { get { return prec_ref_baseline; } set { prec_ref_baseline = value; } }
        private uint prec_ref_display_width;
        public uint PrecRefDisplayWidth { get { return prec_ref_display_width; } set { prec_ref_display_width = value; } }
        private byte ref_viewing_distance_flag;
        public byte RefViewingDistanceFlag { get { return ref_viewing_distance_flag; } set { ref_viewing_distance_flag = value; } }
        private uint prec_ref_viewing_dist;
        public uint PrecRefViewingDist { get { return prec_ref_viewing_dist; } set { prec_ref_viewing_dist = value; } }
        private uint num_ref_displays_minus1;
        public uint NumRefDisplaysMinus1 { get { return num_ref_displays_minus1; } set { num_ref_displays_minus1 = value; } }
        private uint[] exponent_ref_baseline;
        public uint[] ExponentRefBaseline { get { return exponent_ref_baseline; } set { exponent_ref_baseline = value; } }
        private uint[] mantissa_ref_baseline;
        public uint[] MantissaRefBaseline { get { return mantissa_ref_baseline; } set { mantissa_ref_baseline = value; } }
        private uint[] exponent_ref_display_width;
        public uint[] ExponentRefDisplayWidth { get { return exponent_ref_display_width; } set { exponent_ref_display_width = value; } }
        private uint[] mantissa_ref_display_width;
        public uint[] MantissaRefDisplayWidth { get { return mantissa_ref_display_width; } set { mantissa_ref_display_width = value; } }
        private uint[] exponent_ref_viewing_distance;
        public uint[] ExponentRefViewingDistance { get { return exponent_ref_viewing_distance; } set { exponent_ref_viewing_distance = value; } }
        private uint[] mantissa_ref_viewing_distance;
        public uint[] MantissaRefViewingDistance { get { return mantissa_ref_viewing_distance; } set { mantissa_ref_viewing_distance = value; } }
        private byte[] additional_shift_present_flag;
        public byte[] AdditionalShiftPresentFlag { get { return additional_shift_present_flag; } set { additional_shift_present_flag = value; } }
        private uint[] num_sample_shift_plus512;
        public uint[] NumSampleShiftPlus512 { get { return num_sample_shift_plus512; } set { num_sample_shift_plus512 = value; } }
        private byte three_dimensional_reference_displays_extension_flag;
        public byte ThreeDimensionalReferenceDisplaysExtensionFlag { get { return three_dimensional_reference_displays_extension_flag; } set { three_dimensional_reference_displays_extension_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ThreeDimensionalReferenceDisplaysInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint numRefDisplays = 0;
            uint i = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.prec_ref_baseline);
            size += stream.ReadUnsignedIntGolomb(size, out this.prec_ref_display_width);
            size += stream.ReadUnsignedInt(size, 1, out this.ref_viewing_distance_flag);

            if (ref_viewing_distance_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.prec_ref_viewing_dist);
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.num_ref_displays_minus1);
            numRefDisplays = num_ref_displays_minus1 + 1;

            this.exponent_ref_baseline = new uint[numRefDisplays];
            this.mantissa_ref_baseline = new uint[numRefDisplays];
            this.exponent_ref_display_width = new uint[numRefDisplays];
            this.mantissa_ref_display_width = new uint[numRefDisplays];
            this.exponent_ref_viewing_distance = new uint[numRefDisplays];
            this.mantissa_ref_viewing_distance = new uint[numRefDisplays];
            this.additional_shift_present_flag = new byte[numRefDisplays];
            this.num_sample_shift_plus512 = new uint[numRefDisplays];
            for (i = 0; i < numRefDisplays; i++)
            {
                size += stream.ReadUnsignedInt(size, 6, out this.exponent_ref_baseline[i]);
                size += stream.ReadUnsignedIntVariable(size, (exponent_ref_baseline[i] == 0) ? (Math.Max(0, prec_ref_baseline - 30)) : (Math.Max(0, exponent_ref_baseline[i] + prec_ref_baseline - 31)), out this.mantissa_ref_baseline[i]);
                size += stream.ReadUnsignedInt(size, 6, out this.exponent_ref_display_width[i]);
                size += stream.ReadUnsignedIntVariable(size, (exponent_ref_display_width[i] == 0) ? (Math.Max(0, prec_ref_display_width - 30)) : (Math.Max(0, exponent_ref_display_width[i] + prec_ref_display_width - 31)), out this.mantissa_ref_display_width[i]);

                if (ref_viewing_distance_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 6, out this.exponent_ref_viewing_distance[i]);
                    size += stream.ReadUnsignedIntVariable(size, (exponent_ref_viewing_distance[i] == 0) ? (Math.Max(0, prec_ref_viewing_dist - 30)) : (Math.Max(0, exponent_ref_viewing_distance[i] + prec_ref_viewing_dist - 31)), out this.mantissa_ref_viewing_distance[i]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.additional_shift_present_flag[i]);

                if (context.Sei.SeiMessage.SeiPayload.ThreeDimensionalReferenceDisplaysInfo.AdditionalShiftPresentFlag.Select(x => (uint)x).ToArray()[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 10, out this.num_sample_shift_plus512[i]);
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.three_dimensional_reference_displays_extension_flag);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint numRefDisplays = 0;
            uint i = 0;
            size += stream.WriteUnsignedIntGolomb(this.prec_ref_baseline);
            size += stream.WriteUnsignedIntGolomb(this.prec_ref_display_width);
            size += stream.WriteUnsignedInt(1, this.ref_viewing_distance_flag);

            if (ref_viewing_distance_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.prec_ref_viewing_dist);
            }
            size += stream.WriteUnsignedIntGolomb(this.num_ref_displays_minus1);
            numRefDisplays = num_ref_displays_minus1 + 1;

            for (i = 0; i < numRefDisplays; i++)
            {
                size += stream.WriteUnsignedInt(6, this.exponent_ref_baseline[i]);
                size += stream.WriteUnsignedIntVariable((exponent_ref_baseline[i] == 0) ? (Math.Max(0, prec_ref_baseline - 30)) : (Math.Max(0, exponent_ref_baseline[i] + prec_ref_baseline - 31)), this.mantissa_ref_baseline[i]);
                size += stream.WriteUnsignedInt(6, this.exponent_ref_display_width[i]);
                size += stream.WriteUnsignedIntVariable((exponent_ref_display_width[i] == 0) ? (Math.Max(0, prec_ref_display_width - 30)) : (Math.Max(0, exponent_ref_display_width[i] + prec_ref_display_width - 31)), this.mantissa_ref_display_width[i]);

                if (ref_viewing_distance_flag != 0)
                {
                    size += stream.WriteUnsignedInt(6, this.exponent_ref_viewing_distance[i]);
                    size += stream.WriteUnsignedIntVariable((exponent_ref_viewing_distance[i] == 0) ? (Math.Max(0, prec_ref_viewing_dist - 30)) : (Math.Max(0, exponent_ref_viewing_distance[i] + prec_ref_viewing_dist - 31)), this.mantissa_ref_viewing_distance[i]);
                }
                size += stream.WriteUnsignedInt(1, this.additional_shift_present_flag[i]);

                if (context.Sei.SeiMessage.SeiPayload.ThreeDimensionalReferenceDisplaysInfo.AdditionalShiftPresentFlag.Select(x => (uint)x).ToArray()[i] != 0)
                {
                    size += stream.WriteUnsignedInt(10, this.num_sample_shift_plus512[i]);
                }
            }
            size += stream.WriteUnsignedInt(1, this.three_dimensional_reference_displays_extension_flag);

            return size;
        }

    }

    /*
  

depth_timing( payloadSize ) {  
 per_view_depth_timing_flag 5 u(1) 
 if( per_view_depth_timing_flag )   
  for( i = 0; i < NumDepthViews; i++ )   
   depth_timing_offset()   
 else   
  depth_timing_offset()   
}
    */
    public class DepthTiming : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte per_view_depth_timing_flag;
        public byte PerViewDepthTimingFlag { get { return per_view_depth_timing_flag; } set { per_view_depth_timing_flag = value; } }
        private DepthTimingOffset[] depth_timing_offset;
        public DepthTimingOffset[] DepthTimingOffset { get { return depth_timing_offset; } set { depth_timing_offset = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public DepthTiming(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.per_view_depth_timing_flag);

            if (per_view_depth_timing_flag != 0)
            {

                this.depth_timing_offset = new DepthTimingOffset[context.SubsetSps.SeqParameterSetMvcdExtension.NumDepthViews];
                for (i = 0; i < context.SubsetSps.SeqParameterSetMvcdExtension.NumDepthViews; i++)
                {
                    size += stream.ReadClass<DepthTimingOffset>(size, context, () => new DepthTimingOffset(), out this.depth_timing_offset[i]);
                }
            }
            else
            {
                size += stream.ReadClass<DepthTimingOffset>(size, context, () => new DepthTimingOffset(), out this.depth_timing_offset);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedInt(1, this.per_view_depth_timing_flag);

            if (per_view_depth_timing_flag != 0)
            {

                for (i = 0; i < context.SubsetSps.SeqParameterSetMvcdExtension.NumDepthViews; i++)
                {
                    size += stream.WriteClass<DepthTimingOffset>(context, this.depth_timing_offset[i]);
                }
            }
            else
            {
                size += stream.WriteClass<DepthTimingOffset>(context, this.depth_timing_offset);
            }

            return size;
        }

    }

    /*
   

depth_timing_offset() {  
 offset_len_minus1 5 u(5) 
 depth_disp_delay_offset_fp 5 u(v) 
 depth_disp_delay_offset_dp 5 u(6) 
}
    */
    public class DepthTimingOffset : IItuSerializable
    {
        private uint offset_len_minus1;
        public uint OffsetLenMinus1 { get { return offset_len_minus1; } set { offset_len_minus1 = value; } }
        private uint depth_disp_delay_offset_fp;
        public uint DepthDispDelayOffsetFp { get { return depth_disp_delay_offset_fp; } set { depth_disp_delay_offset_fp = value; } }
        private uint depth_disp_delay_offset_dp;
        public uint DepthDispDelayOffsetDp { get { return depth_disp_delay_offset_dp; } set { depth_disp_delay_offset_dp = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public DepthTimingOffset()
        {

        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 5, out this.offset_len_minus1);
            size += stream.ReadUnsignedIntVariable(size, (context.Sei.SeiMessage.SeiPayload.DepthTiming.DepthTimingOffset.First().OffsetLenMinus1 + 1), out this.depth_disp_delay_offset_fp);
            size += stream.ReadUnsignedInt(size, 6, out this.depth_disp_delay_offset_dp);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(5, this.offset_len_minus1);
            size += stream.WriteUnsignedIntVariable((context.Sei.SeiMessage.SeiPayload.DepthTiming.DepthTimingOffset.First().OffsetLenMinus1 + 1), this.depth_disp_delay_offset_fp);
            size += stream.WriteUnsignedInt(6, this.depth_disp_delay_offset_dp);

            return size;
        }

    }

    /*
  

alternative_depth_info( payloadSize ) {  
 depth_type 5 ue(v) 
 if( depth_type  ==  0 ) {   
  num_constituent_views_gvd_minus1 5 ue(v) 
  depth_present_gvd_flag 5 u(1) 
  z_gvd_flag 5 u(1) 
  intrinsic_param_gvd_flag 5 u(1) 
  rotation_gvd_flag 5 u(1) 
  translation_gvd_flag 5 u(1) 
  if( z_gvd_flag )    
   for( i = 0; i  <=  num_constituent_views_gvd_minus1 + 1; i++ ) {   
    sign_gvd_z_near_flag[ i ] 5 u(1) 
    exp_gvd_z_near[ i ] 5 u(7) 
    man_len_gvd_z_near_minus1[ i ] 5 u(5) 
    man_gvd_z_near[ i ] 5 u(v) 
    sign_gvd_z_far_flag[ i ] 5 u(1) 
    exp_gvd_z_far[ i ] 5 u(7) 
    man_len_gvd_z_far_minus1[ i ] 5 u(5) 
    man_gvd_z_far[ i ] 5 u(v) 
   }   
  if( intrinsic_param_gvd_flag ) {   
   prec_gvd_focal_length 5 ue(v) 
   prec_gvd_principal_point 5 ue(v) 
  }   
  if( rotation_gvd_flag )   
   prec_gvd_rotation_param 5 ue(v) 
  if( translation_gvd_flag )   
   prec_gvd_translation_param 5 ue(v) 
  for( i = 0; i  <=  num_constituent_views_gvd_minus1 + 1; i++ ) {   
   if( intrinsic_param_gvd_flag ) {   
    sign_gvd_focal_length_x[ i ] 5 u(1) 
    exp_gvd_focal_length_x[ i ] 5 u(6) 
    man_gvd_focal_length_x[ i ] 5 u(v) 
    sign_gvd_focal_length_y[ i ] 5 u(1) 
    exp_gvd_focal_length_y[ i ] 5 u(6) 
    man_gvd_focal_length_y[ i ] 5 u(v) 
    sign_gvd_principal_point_x[ i ] 5 u(1) 
    exp_gvd_principal_point_x[ i ] 5 u(6)
        man_gvd_principal_point_x[ i ] 5 u(v) 
    sign_gvd_principal_point_y[ i ] 5 u(1) 
    exp_gvd_principal_point_y[ i ] 5 u(6) 
    man_gvd_principal_point_y[ i ] 5 u(v) 
   }   
   if( rotation_gvd_flag )   
    for( j = 0; j < 3; j++ ) /* row *//*   
     for( k = 0; k < 3; k++ ) { /* column *//*   
      sign_gvd_r[ i ][ j ][ k ] 5 u(1) 
      exp_gvd_r[ i ][ j ][ k ] 5 u(6) 
      man_gvd_r[ i ][ j ][ k ] 5 u(v) 
    }   
   if( translation_gvd_flag ) {   
    sign_gvd_t_x[ i ] 5 u(1) 
    exp_gvd_t_x[ i ] 5 u(6) 
    man_gvd_t_x[ i ] 5 u(v) 
   }   
  }   
 }   
}
    */
    public class AlternativeDepthInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint depth_type;
        public uint DepthType { get { return depth_type; } set { depth_type = value; } }
        private uint num_constituent_views_gvd_minus1;
        public uint NumConstituentViewsGvdMinus1 { get { return num_constituent_views_gvd_minus1; } set { num_constituent_views_gvd_minus1 = value; } }
        private byte depth_present_gvd_flag;
        public byte DepthPresentGvdFlag { get { return depth_present_gvd_flag; } set { depth_present_gvd_flag = value; } }
        private byte z_gvd_flag;
        public byte zGvdFlag { get { return z_gvd_flag; } set { z_gvd_flag = value; } }
        private byte intrinsic_param_gvd_flag;
        public byte IntrinsicParamGvdFlag { get { return intrinsic_param_gvd_flag; } set { intrinsic_param_gvd_flag = value; } }
        private byte rotation_gvd_flag;
        public byte RotationGvdFlag { get { return rotation_gvd_flag; } set { rotation_gvd_flag = value; } }
        private byte translation_gvd_flag;
        public byte TranslationGvdFlag { get { return translation_gvd_flag; } set { translation_gvd_flag = value; } }
        private byte[] sign_gvd_z_near_flag;
        public byte[] SignGvdzNearFlag { get { return sign_gvd_z_near_flag; } set { sign_gvd_z_near_flag = value; } }
        private uint[] exp_gvd_z_near;
        public uint[] ExpGvdzNear { get { return exp_gvd_z_near; } set { exp_gvd_z_near = value; } }
        private uint[] man_len_gvd_z_near_minus1;
        public uint[] ManLenGvdzNearMinus1 { get { return man_len_gvd_z_near_minus1; } set { man_len_gvd_z_near_minus1 = value; } }
        private uint[] man_gvd_z_near;
        public uint[] ManGvdzNear { get { return man_gvd_z_near; } set { man_gvd_z_near = value; } }
        private byte[] sign_gvd_z_far_flag;
        public byte[] SignGvdzFarFlag { get { return sign_gvd_z_far_flag; } set { sign_gvd_z_far_flag = value; } }
        private uint[] exp_gvd_z_far;
        public uint[] ExpGvdzFar { get { return exp_gvd_z_far; } set { exp_gvd_z_far = value; } }
        private uint[] man_len_gvd_z_far_minus1;
        public uint[] ManLenGvdzFarMinus1 { get { return man_len_gvd_z_far_minus1; } set { man_len_gvd_z_far_minus1 = value; } }
        private uint[] man_gvd_z_far;
        public uint[] ManGvdzFar { get { return man_gvd_z_far; } set { man_gvd_z_far = value; } }
        private uint prec_gvd_focal_length;
        public uint PrecGvdFocalLength { get { return prec_gvd_focal_length; } set { prec_gvd_focal_length = value; } }
        private uint prec_gvd_principal_point;
        public uint PrecGvdPrincipalPoint { get { return prec_gvd_principal_point; } set { prec_gvd_principal_point = value; } }
        private uint prec_gvd_rotation_param;
        public uint PrecGvdRotationParam { get { return prec_gvd_rotation_param; } set { prec_gvd_rotation_param = value; } }
        private uint prec_gvd_translation_param;
        public uint PrecGvdTranslationParam { get { return prec_gvd_translation_param; } set { prec_gvd_translation_param = value; } }
        private byte[] sign_gvd_focal_length_x;
        public byte[] SignGvdFocalLengthx { get { return sign_gvd_focal_length_x; } set { sign_gvd_focal_length_x = value; } }
        private uint[] exp_gvd_focal_length_x;
        public uint[] ExpGvdFocalLengthx { get { return exp_gvd_focal_length_x; } set { exp_gvd_focal_length_x = value; } }
        private uint[] man_gvd_focal_length_x;
        public uint[] ManGvdFocalLengthx { get { return man_gvd_focal_length_x; } set { man_gvd_focal_length_x = value; } }
        private byte[] sign_gvd_focal_length_y;
        public byte[] SignGvdFocalLengthy { get { return sign_gvd_focal_length_y; } set { sign_gvd_focal_length_y = value; } }
        private uint[] exp_gvd_focal_length_y;
        public uint[] ExpGvdFocalLengthy { get { return exp_gvd_focal_length_y; } set { exp_gvd_focal_length_y = value; } }
        private uint[] man_gvd_focal_length_y;
        public uint[] ManGvdFocalLengthy { get { return man_gvd_focal_length_y; } set { man_gvd_focal_length_y = value; } }
        private byte[] sign_gvd_principal_point_x;
        public byte[] SignGvdPrincipalPointx { get { return sign_gvd_principal_point_x; } set { sign_gvd_principal_point_x = value; } }
        private uint[] exp_gvd_principal_point_x;
        public uint[] ExpGvdPrincipalPointx { get { return exp_gvd_principal_point_x; } set { exp_gvd_principal_point_x = value; } }
        private uint[] man_gvd_principal_point_x;
        public uint[] ManGvdPrincipalPointx { get { return man_gvd_principal_point_x; } set { man_gvd_principal_point_x = value; } }
        private byte[] sign_gvd_principal_point_y;
        public byte[] SignGvdPrincipalPointy { get { return sign_gvd_principal_point_y; } set { sign_gvd_principal_point_y = value; } }
        private uint[] exp_gvd_principal_point_y;
        public uint[] ExpGvdPrincipalPointy { get { return exp_gvd_principal_point_y; } set { exp_gvd_principal_point_y = value; } }
        private uint[] man_gvd_principal_point_y;
        public uint[] ManGvdPrincipalPointy { get { return man_gvd_principal_point_y; } set { man_gvd_principal_point_y = value; } }
        private byte[][][] sign_gvd_r;
        public byte[][][] SignGvdr { get { return sign_gvd_r; } set { sign_gvd_r = value; } }
        private uint[][][] exp_gvd_r;
        public uint[][][] ExpGvdr { get { return exp_gvd_r; } set { exp_gvd_r = value; } }
        private uint[][][] man_gvd_r;
        public uint[][][] ManGvdr { get { return man_gvd_r; } set { man_gvd_r = value; } }
        private byte[] sign_gvd_t_x;
        public byte[] SignGvdtx { get { return sign_gvd_t_x; } set { sign_gvd_t_x = value; } }
        private uint[] exp_gvd_t_x;
        public uint[] ExpGvdtx { get { return exp_gvd_t_x; } set { exp_gvd_t_x = value; } }
        private uint[] man_gvd_t_x;
        public uint[] ManGvdtx { get { return man_gvd_t_x; } set { man_gvd_t_x = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public AlternativeDepthInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            uint k = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.depth_type);

            if (depth_type == 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.num_constituent_views_gvd_minus1);
                size += stream.ReadUnsignedInt(size, 1, out this.depth_present_gvd_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.z_gvd_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.intrinsic_param_gvd_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.rotation_gvd_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.translation_gvd_flag);

                if (z_gvd_flag != 0)
                {

                    this.sign_gvd_z_near_flag = new byte[num_constituent_views_gvd_minus1 + 1 + 1];
                    this.exp_gvd_z_near = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                    this.man_len_gvd_z_near_minus1 = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                    this.man_gvd_z_near = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                    this.sign_gvd_z_far_flag = new byte[num_constituent_views_gvd_minus1 + 1 + 1];
                    this.exp_gvd_z_far = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                    this.man_len_gvd_z_far_minus1 = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                    this.man_gvd_z_far = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                    for (i = 0; i <= num_constituent_views_gvd_minus1 + 1; i++)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.sign_gvd_z_near_flag[i]);
                        size += stream.ReadUnsignedInt(size, 7, out this.exp_gvd_z_near[i]);
                        size += stream.ReadUnsignedInt(size, 5, out this.man_len_gvd_z_near_minus1[i]);
                        size += stream.ReadUnsignedIntVariable(size, (man_len_gvd_z_near_minus1[i] + 1), out this.man_gvd_z_near[i]);
                        size += stream.ReadUnsignedInt(size, 1, out this.sign_gvd_z_far_flag[i]);
                        size += stream.ReadUnsignedInt(size, 7, out this.exp_gvd_z_far[i]);
                        size += stream.ReadUnsignedInt(size, 5, out this.man_len_gvd_z_far_minus1[i]);
                        size += stream.ReadUnsignedIntVariable(size, (man_len_gvd_z_far_minus1[i] + 1), out this.man_gvd_z_far[i]);
                    }
                }

                if (intrinsic_param_gvd_flag != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.prec_gvd_focal_length);
                    size += stream.ReadUnsignedIntGolomb(size, out this.prec_gvd_principal_point);
                }

                if (rotation_gvd_flag != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.prec_gvd_rotation_param);
                }

                if (translation_gvd_flag != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.prec_gvd_translation_param);
                }

                this.sign_gvd_focal_length_x = new byte[num_constituent_views_gvd_minus1 + 1 + 1];
                this.exp_gvd_focal_length_x = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                this.man_gvd_focal_length_x = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                this.sign_gvd_focal_length_y = new byte[num_constituent_views_gvd_minus1 + 1 + 1];
                this.exp_gvd_focal_length_y = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                this.man_gvd_focal_length_y = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                this.sign_gvd_principal_point_x = new byte[num_constituent_views_gvd_minus1 + 1 + 1];
                this.exp_gvd_principal_point_x = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                this.man_gvd_principal_point_x = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                this.sign_gvd_principal_point_y = new byte[num_constituent_views_gvd_minus1 + 1 + 1];
                this.exp_gvd_principal_point_y = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                this.man_gvd_principal_point_y = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                this.sign_gvd_r = new byte[num_constituent_views_gvd_minus1 + 1 + 1][][];
                this.exp_gvd_r = new uint[num_constituent_views_gvd_minus1 + 1 + 1][][];
                this.man_gvd_r = new uint[num_constituent_views_gvd_minus1 + 1 + 1][][];
                this.sign_gvd_t_x = new byte[num_constituent_views_gvd_minus1 + 1 + 1];
                this.exp_gvd_t_x = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                this.man_gvd_t_x = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                for (i = 0; i <= num_constituent_views_gvd_minus1 + 1; i++)
                {

                    if (intrinsic_param_gvd_flag != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.sign_gvd_focal_length_x[i]);
                        size += stream.ReadUnsignedInt(size, 6, out this.exp_gvd_focal_length_x[i]);
                        size += stream.ReadUnsignedIntVariable(size, (exp_gvd_focal_length_x[i] == 0) ? (Math.Max(0, prec_gvd_focal_length - 30)) : (Math.Max(0, exp_gvd_focal_length_x[i] + prec_gvd_focal_length - 31)), out this.man_gvd_focal_length_x[i]);
                        size += stream.ReadUnsignedInt(size, 1, out this.sign_gvd_focal_length_y[i]);
                        size += stream.ReadUnsignedInt(size, 6, out this.exp_gvd_focal_length_y[i]);
                        size += stream.ReadUnsignedIntVariable(size, (exp_gvd_focal_length_y[i] == 0) ? (Math.Max(0, prec_gvd_focal_length - 30)) : (Math.Max(0, exp_gvd_focal_length_y[i] + prec_gvd_focal_length - 31)), out this.man_gvd_focal_length_y[i]);
                        size += stream.ReadUnsignedInt(size, 1, out this.sign_gvd_principal_point_x[i]);
                        size += stream.ReadUnsignedInt(size, 6, out this.exp_gvd_principal_point_x[i]);
                        size += stream.ReadUnsignedIntVariable(size, (exp_gvd_principal_point_x[i] == 0) ? (Math.Max(0, prec_gvd_principal_point - 30)) : (Math.Max(0, exp_gvd_principal_point_x[i] + prec_gvd_principal_point - 31)), out this.man_gvd_principal_point_x[i]);
                        size += stream.ReadUnsignedInt(size, 1, out this.sign_gvd_principal_point_y[i]);
                        size += stream.ReadUnsignedInt(size, 6, out this.exp_gvd_principal_point_y[i]);
                        size += stream.ReadUnsignedIntVariable(size, (exp_gvd_principal_point_y[i] == 0) ? (Math.Max(0, prec_gvd_principal_point - 30)) : (Math.Max(0, exp_gvd_principal_point_y[i] + prec_gvd_principal_point - 31)), out this.man_gvd_principal_point_y[i]);
                    }

                    if (rotation_gvd_flag != 0)
                    {

                        this.sign_gvd_r[i] = new byte[3][];
                        this.exp_gvd_r[i] = new uint[3][];
                        this.man_gvd_r[i] = new uint[3][];
                        for (j = 0; j < 3; j++)
                        {

                            this.sign_gvd_r[i][j] = new byte[3];
                            this.exp_gvd_r[i][j] = new uint[3];
                            this.man_gvd_r[i][j] = new uint[3];
                            for (k = 0; k < 3; k++)
                            {
                                /*  column  */

                                size += stream.ReadUnsignedInt(size, 1, out this.sign_gvd_r[i][j][k]);
                                size += stream.ReadUnsignedInt(size, 6, out this.exp_gvd_r[i][j][k]);
                                size += stream.ReadUnsignedIntVariable(size, (exp_gvd_r[i][j][k] == 0) ? (Math.Max(0, prec_gvd_rotation_param - 30)) : (Math.Max(0, exp_gvd_r[i][j][k] + prec_gvd_rotation_param - 31)), out this.man_gvd_r[i][j][k]);
                            }
                        }
                    }

                    if (translation_gvd_flag != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.sign_gvd_t_x[i]);
                        size += stream.ReadUnsignedInt(size, 6, out this.exp_gvd_t_x[i]);
                        size += stream.ReadUnsignedIntVariable(size, (exp_gvd_t_x[i] == 0) ? (Math.Max(0, prec_gvd_translation_param - 30)) : (Math.Max(0, exp_gvd_t_x[i] + prec_gvd_translation_param - 31)), out this.man_gvd_t_x[i]);
                    }
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            uint k = 0;
            size += stream.WriteUnsignedIntGolomb(this.depth_type);

            if (depth_type == 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.num_constituent_views_gvd_minus1);
                size += stream.WriteUnsignedInt(1, this.depth_present_gvd_flag);
                size += stream.WriteUnsignedInt(1, this.z_gvd_flag);
                size += stream.WriteUnsignedInt(1, this.intrinsic_param_gvd_flag);
                size += stream.WriteUnsignedInt(1, this.rotation_gvd_flag);
                size += stream.WriteUnsignedInt(1, this.translation_gvd_flag);

                if (z_gvd_flag != 0)
                {

                    for (i = 0; i <= num_constituent_views_gvd_minus1 + 1; i++)
                    {
                        size += stream.WriteUnsignedInt(1, this.sign_gvd_z_near_flag[i]);
                        size += stream.WriteUnsignedInt(7, this.exp_gvd_z_near[i]);
                        size += stream.WriteUnsignedInt(5, this.man_len_gvd_z_near_minus1[i]);
                        size += stream.WriteUnsignedIntVariable((man_len_gvd_z_near_minus1[i] + 1), this.man_gvd_z_near[i]);
                        size += stream.WriteUnsignedInt(1, this.sign_gvd_z_far_flag[i]);
                        size += stream.WriteUnsignedInt(7, this.exp_gvd_z_far[i]);
                        size += stream.WriteUnsignedInt(5, this.man_len_gvd_z_far_minus1[i]);
                        size += stream.WriteUnsignedIntVariable((man_len_gvd_z_far_minus1[i] + 1), this.man_gvd_z_far[i]);
                    }
                }

                if (intrinsic_param_gvd_flag != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.prec_gvd_focal_length);
                    size += stream.WriteUnsignedIntGolomb(this.prec_gvd_principal_point);
                }

                if (rotation_gvd_flag != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.prec_gvd_rotation_param);
                }

                if (translation_gvd_flag != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.prec_gvd_translation_param);
                }

                for (i = 0; i <= num_constituent_views_gvd_minus1 + 1; i++)
                {

                    if (intrinsic_param_gvd_flag != 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.sign_gvd_focal_length_x[i]);
                        size += stream.WriteUnsignedInt(6, this.exp_gvd_focal_length_x[i]);
                        size += stream.WriteUnsignedIntVariable((exp_gvd_focal_length_x[i] == 0) ? (Math.Max(0, prec_gvd_focal_length - 30)) : (Math.Max(0, exp_gvd_focal_length_x[i] + prec_gvd_focal_length - 31)), this.man_gvd_focal_length_x[i]);
                        size += stream.WriteUnsignedInt(1, this.sign_gvd_focal_length_y[i]);
                        size += stream.WriteUnsignedInt(6, this.exp_gvd_focal_length_y[i]);
                        size += stream.WriteUnsignedIntVariable((exp_gvd_focal_length_y[i] == 0) ? (Math.Max(0, prec_gvd_focal_length - 30)) : (Math.Max(0, exp_gvd_focal_length_y[i] + prec_gvd_focal_length - 31)), this.man_gvd_focal_length_y[i]);
                        size += stream.WriteUnsignedInt(1, this.sign_gvd_principal_point_x[i]);
                        size += stream.WriteUnsignedInt(6, this.exp_gvd_principal_point_x[i]);
                        size += stream.WriteUnsignedIntVariable((exp_gvd_principal_point_x[i] == 0) ? (Math.Max(0, prec_gvd_principal_point - 30)) : (Math.Max(0, exp_gvd_principal_point_x[i] + prec_gvd_principal_point - 31)), this.man_gvd_principal_point_x[i]);
                        size += stream.WriteUnsignedInt(1, this.sign_gvd_principal_point_y[i]);
                        size += stream.WriteUnsignedInt(6, this.exp_gvd_principal_point_y[i]);
                        size += stream.WriteUnsignedIntVariable((exp_gvd_principal_point_y[i] == 0) ? (Math.Max(0, prec_gvd_principal_point - 30)) : (Math.Max(0, exp_gvd_principal_point_y[i] + prec_gvd_principal_point - 31)), this.man_gvd_principal_point_y[i]);
                    }

                    if (rotation_gvd_flag != 0)
                    {

                        for (j = 0; j < 3; j++)
                        {

                            for (k = 0; k < 3; k++)
                            {
                                /*  column  */

                                size += stream.WriteUnsignedInt(1, this.sign_gvd_r[i][j][k]);
                                size += stream.WriteUnsignedInt(6, this.exp_gvd_r[i][j][k]);
                                size += stream.WriteUnsignedIntVariable((exp_gvd_r[i][j][k] == 0) ? (Math.Max(0, prec_gvd_rotation_param - 30)) : (Math.Max(0, exp_gvd_r[i][j][k] + prec_gvd_rotation_param - 31)), this.man_gvd_r[i][j][k]);
                            }
                        }
                    }

                    if (translation_gvd_flag != 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.sign_gvd_t_x[i]);
                        size += stream.WriteUnsignedInt(6, this.exp_gvd_t_x[i]);
                        size += stream.WriteUnsignedIntVariable((exp_gvd_t_x[i] == 0) ? (Math.Max(0, prec_gvd_translation_param - 30)) : (Math.Max(0, exp_gvd_t_x[i] + prec_gvd_translation_param - 31)), this.man_gvd_t_x[i]);
                    }
                }
            }

            return size;
        }

    }

    /*
  

depth_sampling_info( payloadSize ) {  
 dttsr_x_mul 5 u(16) 
 dttsr_x_dp 5 u(4) 
 dttsr_y_mul 5 u(16) 
 dttsr_y_dp 5 u(4) 
 per_view_depth_grid_pos_flag 5 u(1) 
 if( per_view_depth_grid_pos_flag ) {   
  num_video_plus_depth_views_minus1 5 ue(v) 
  for( i = 0; i <= num_video_plus_depth_views_minus1; i++ ) {   
   depth_grid_view_id[ i ] 5 ue(v) 
   depth_grid_position()   
  }   
 } else   
  depth_grid_position()   
}
    */
    public class DepthSamplingInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint dttsr_x_mul;
        public uint DttsrxMul { get { return dttsr_x_mul; } set { dttsr_x_mul = value; } }
        private uint dttsr_x_dp;
        public uint DttsrxDp { get { return dttsr_x_dp; } set { dttsr_x_dp = value; } }
        private uint dttsr_y_mul;
        public uint DttsryMul { get { return dttsr_y_mul; } set { dttsr_y_mul = value; } }
        private uint dttsr_y_dp;
        public uint DttsryDp { get { return dttsr_y_dp; } set { dttsr_y_dp = value; } }
        private byte per_view_depth_grid_pos_flag;
        public byte PerViewDepthGridPosFlag { get { return per_view_depth_grid_pos_flag; } set { per_view_depth_grid_pos_flag = value; } }
        private uint num_video_plus_depth_views_minus1;
        public uint NumVideoPlusDepthViewsMinus1 { get { return num_video_plus_depth_views_minus1; } set { num_video_plus_depth_views_minus1 = value; } }
        private uint[] depth_grid_view_id;
        public uint[] DepthGridViewId { get { return depth_grid_view_id; } set { depth_grid_view_id = value; } }
        private DepthGridPosition[] depth_grid_position;
        public DepthGridPosition[] DepthGridPosition { get { return depth_grid_position; } set { depth_grid_position = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public DepthSamplingInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedInt(size, 16, out this.dttsr_x_mul);
            size += stream.ReadUnsignedInt(size, 4, out this.dttsr_x_dp);
            size += stream.ReadUnsignedInt(size, 16, out this.dttsr_y_mul);
            size += stream.ReadUnsignedInt(size, 4, out this.dttsr_y_dp);
            size += stream.ReadUnsignedInt(size, 1, out this.per_view_depth_grid_pos_flag);

            if (per_view_depth_grid_pos_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.num_video_plus_depth_views_minus1);

                this.depth_grid_view_id = new uint[num_video_plus_depth_views_minus1 + 1];
                this.depth_grid_position = new DepthGridPosition[num_video_plus_depth_views_minus1 + 1];
                for (i = 0; i <= num_video_plus_depth_views_minus1; i++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.depth_grid_view_id[i]);
                    size += stream.ReadClass<DepthGridPosition>(size, context, () => new DepthGridPosition(), out this.depth_grid_position[i]);
                }
            }
            else
            {
                size += stream.ReadClass<DepthGridPosition>(size, context, () => new DepthGridPosition(), out this.depth_grid_position);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedInt(16, this.dttsr_x_mul);
            size += stream.WriteUnsignedInt(4, this.dttsr_x_dp);
            size += stream.WriteUnsignedInt(16, this.dttsr_y_mul);
            size += stream.WriteUnsignedInt(4, this.dttsr_y_dp);
            size += stream.WriteUnsignedInt(1, this.per_view_depth_grid_pos_flag);

            if (per_view_depth_grid_pos_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.num_video_plus_depth_views_minus1);

                for (i = 0; i <= num_video_plus_depth_views_minus1; i++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.depth_grid_view_id[i]);
                    size += stream.WriteClass<DepthGridPosition>(context, this.depth_grid_position[i]);
                }
            }
            else
            {
                size += stream.WriteClass<DepthGridPosition>(context, this.depth_grid_position);
            }

            return size;
        }

    }

    /*
  

constrained_depth_parameter_set_identifier(payloadSize) {
    max_dps_id 5  ue(v)
    max_dps_id_diff  5  ue(v)
}
    */
    public class ConstrainedDepthParameterSetIdentifier : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint max_dps_id;
        public uint MaxDpsId { get { return max_dps_id; } set { max_dps_id = value; } }
        private uint max_dps_id_diff;
        public uint MaxDpsIdDiff { get { return max_dps_id_diff; } set { max_dps_id_diff = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ConstrainedDepthParameterSetIdentifier(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedIntGolomb(size, out this.max_dps_id);
            size += stream.ReadUnsignedIntGolomb(size, out this.max_dps_id_diff);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedIntGolomb(this.max_dps_id);
            size += stream.WriteUnsignedIntGolomb(this.max_dps_id_diff);

            return size;
        }

    }

    /*


depth_grid_position() { 
depth_grid_pos_x_fp 5 u(20) 
depth_grid_pos_x_dp 5 u(4) 
depth_grid_pos_x_sign_flag 5 u(1) 
depth_grid_pos_y_fp 5 u(20) 
depth_grid_pos_y_dp 5 u(4) 
depth_grid_pos_y_sign_flag 5 u(1) 
}
    */
    public class DepthGridPosition : IItuSerializable
    {
        private uint depth_grid_pos_x_fp;
        public uint DepthGridPosxFp { get { return depth_grid_pos_x_fp; } set { depth_grid_pos_x_fp = value; } }
        private uint depth_grid_pos_x_dp;
        public uint DepthGridPosxDp { get { return depth_grid_pos_x_dp; } set { depth_grid_pos_x_dp = value; } }
        private byte depth_grid_pos_x_sign_flag;
        public byte DepthGridPosxSignFlag { get { return depth_grid_pos_x_sign_flag; } set { depth_grid_pos_x_sign_flag = value; } }
        private uint depth_grid_pos_y_fp;
        public uint DepthGridPosyFp { get { return depth_grid_pos_y_fp; } set { depth_grid_pos_y_fp = value; } }
        private uint depth_grid_pos_y_dp;
        public uint DepthGridPosyDp { get { return depth_grid_pos_y_dp; } set { depth_grid_pos_y_dp = value; } }
        private byte depth_grid_pos_y_sign_flag;
        public byte DepthGridPosySignFlag { get { return depth_grid_pos_y_sign_flag; } set { depth_grid_pos_y_sign_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public DepthGridPosition()
        {

        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 20, out this.depth_grid_pos_x_fp);
            size += stream.ReadUnsignedInt(size, 4, out this.depth_grid_pos_x_dp);
            size += stream.ReadUnsignedInt(size, 1, out this.depth_grid_pos_x_sign_flag);
            size += stream.ReadUnsignedInt(size, 20, out this.depth_grid_pos_y_fp);
            size += stream.ReadUnsignedInt(size, 4, out this.depth_grid_pos_y_dp);
            size += stream.ReadUnsignedInt(size, 1, out this.depth_grid_pos_y_sign_flag);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(20, this.depth_grid_pos_x_fp);
            size += stream.WriteUnsignedInt(4, this.depth_grid_pos_x_dp);
            size += stream.WriteUnsignedInt(1, this.depth_grid_pos_x_sign_flag);
            size += stream.WriteUnsignedInt(20, this.depth_grid_pos_y_fp);
            size += stream.WriteUnsignedInt(4, this.depth_grid_pos_y_dp);
            size += stream.WriteUnsignedInt(1, this.depth_grid_pos_y_sign_flag);

            return size;
        }

    }

    /*
   

mvcd_vui_parameters_extension() {  
 vui_mvcd_num_ops_minus1 0 ue(v) 
 for( i = 0; i <= vui_mvcd_num_ops_minus1; i++ ) {   
  vui_mvcd_temporal_id[ i ] 0 u(3) 
  vui_mvcd_num_target_output_views_minus1[ i ] 0 ue(v) 
  for( j = 0; j <= vui_mvcd_num_target_output_views_minus1[ i ]; j++ ) {   
   vui_mvcd_view_id[ i ][ j ] 0 ue(v) 
   vui_mvcd_depth_flag[ i ][ j ] 0 u(1) 
   vui_mvcd_texture_flag[ i ][ j ] 0 u(1) 
  }   
  vui_mvcd_timing_info_present_flag[ i ] 0 u(1) 
  if( vui_mvcd_timing_info_present_flag[ i ] ) {   
   vui_mvcd_num_units_in_tick[ i ] 0 u(32) 
   vui_mvcd_time_scale[ i ] 0 u(32) 
   vui_mvcd_fixed_frame_rate_flag[ i ] 0 u(1) 
  }   
  vui_mvcd_nal_hrd_parameters_present_flag[ i ] 0 u(1) 
  if( vui_mvcd_nal_hrd_parameters_present_flag[ i ] )   
   hrd_parameters() 0  
  vui_mvcd_vcl_hrd_parameters_present_flag[ i ] 0 u(1) 
  if( vui_mvcd_vcl_hrd_parameters_present_flag[ i ] )   
   hrd_parameters() 0  
  if( vui_mvcd_nal_hrd_parameters_present_flag[ i ]  ||   
   vui_mvcd_vcl_hrd_parameters_present_flag[ i ] ) 
  
   vui_mvcd_low_delay_hrd_flag[ i ] 0 u(1) 
  vui_mvcd_pic_struct_present_flag[ i ]  0 u(1) 
 }   
}
    */
    public class MvcdVuiParametersExtension : IItuSerializable
    {
        private uint vui_mvcd_num_ops_minus1;
        public uint VuiMvcdNumOpsMinus1 { get { return vui_mvcd_num_ops_minus1; } set { vui_mvcd_num_ops_minus1 = value; } }
        private uint[] vui_mvcd_temporal_id;
        public uint[] VuiMvcdTemporalId { get { return vui_mvcd_temporal_id; } set { vui_mvcd_temporal_id = value; } }
        private uint[] vui_mvcd_num_target_output_views_minus1;
        public uint[] VuiMvcdNumTargetOutputViewsMinus1 { get { return vui_mvcd_num_target_output_views_minus1; } set { vui_mvcd_num_target_output_views_minus1 = value; } }
        private uint[][] vui_mvcd_view_id;
        public uint[][] VuiMvcdViewId { get { return vui_mvcd_view_id; } set { vui_mvcd_view_id = value; } }
        private byte[][] vui_mvcd_depth_flag;
        public byte[][] VuiMvcdDepthFlag { get { return vui_mvcd_depth_flag; } set { vui_mvcd_depth_flag = value; } }
        private byte[][] vui_mvcd_texture_flag;
        public byte[][] VuiMvcdTextureFlag { get { return vui_mvcd_texture_flag; } set { vui_mvcd_texture_flag = value; } }
        private byte[] vui_mvcd_timing_info_present_flag;
        public byte[] VuiMvcdTimingInfoPresentFlag { get { return vui_mvcd_timing_info_present_flag; } set { vui_mvcd_timing_info_present_flag = value; } }
        private uint[] vui_mvcd_num_units_in_tick;
        public uint[] VuiMvcdNumUnitsInTick { get { return vui_mvcd_num_units_in_tick; } set { vui_mvcd_num_units_in_tick = value; } }
        private uint[] vui_mvcd_time_scale;
        public uint[] VuiMvcdTimeScale { get { return vui_mvcd_time_scale; } set { vui_mvcd_time_scale = value; } }
        private byte[] vui_mvcd_fixed_frame_rate_flag;
        public byte[] VuiMvcdFixedFrameRateFlag { get { return vui_mvcd_fixed_frame_rate_flag; } set { vui_mvcd_fixed_frame_rate_flag = value; } }
        private byte[] vui_mvcd_nal_hrd_parameters_present_flag;
        public byte[] VuiMvcdNalHrdParametersPresentFlag { get { return vui_mvcd_nal_hrd_parameters_present_flag; } set { vui_mvcd_nal_hrd_parameters_present_flag = value; } }
        private HrdParameters[] hrd_parameters;
        public HrdParameters[] HrdParameters { get { return hrd_parameters; } set { hrd_parameters = value; } }
        private byte[] vui_mvcd_vcl_hrd_parameters_present_flag;
        public byte[] VuiMvcdVclHrdParametersPresentFlag { get { return vui_mvcd_vcl_hrd_parameters_present_flag; } set { vui_mvcd_vcl_hrd_parameters_present_flag = value; } }
        private byte[] vui_mvcd_low_delay_hrd_flag;
        public byte[] VuiMvcdLowDelayHrdFlag { get { return vui_mvcd_low_delay_hrd_flag; } set { vui_mvcd_low_delay_hrd_flag = value; } }
        private byte[] vui_mvcd_pic_struct_present_flag;
        public byte[] VuiMvcdPicStructPresentFlag { get { return vui_mvcd_pic_struct_present_flag; } set { vui_mvcd_pic_struct_present_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public MvcdVuiParametersExtension()
        {

        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.vui_mvcd_num_ops_minus1);

            this.vui_mvcd_temporal_id = new uint[vui_mvcd_num_ops_minus1 + 1];
            this.vui_mvcd_num_target_output_views_minus1 = new uint[vui_mvcd_num_ops_minus1 + 1];
            this.vui_mvcd_view_id = new uint[vui_mvcd_num_ops_minus1 + 1][];
            this.vui_mvcd_depth_flag = new byte[vui_mvcd_num_ops_minus1 + 1][];
            this.vui_mvcd_texture_flag = new byte[vui_mvcd_num_ops_minus1 + 1][];
            this.vui_mvcd_timing_info_present_flag = new byte[vui_mvcd_num_ops_minus1 + 1];
            this.vui_mvcd_num_units_in_tick = new uint[vui_mvcd_num_ops_minus1 + 1];
            this.vui_mvcd_time_scale = new uint[vui_mvcd_num_ops_minus1 + 1];
            this.vui_mvcd_fixed_frame_rate_flag = new byte[vui_mvcd_num_ops_minus1 + 1];
            this.vui_mvcd_nal_hrd_parameters_present_flag = new byte[vui_mvcd_num_ops_minus1 + 1];
            this.hrd_parameters = new HrdParameters[vui_mvcd_num_ops_minus1 + 1];
            this.vui_mvcd_vcl_hrd_parameters_present_flag = new byte[vui_mvcd_num_ops_minus1 + 1];
            this.vui_mvcd_low_delay_hrd_flag = new byte[vui_mvcd_num_ops_minus1 + 1];
            this.vui_mvcd_pic_struct_present_flag = new byte[vui_mvcd_num_ops_minus1 + 1];
            for (i = 0; i <= vui_mvcd_num_ops_minus1; i++)
            {
                size += stream.ReadUnsignedInt(size, 3, out this.vui_mvcd_temporal_id[i]);
                size += stream.ReadUnsignedIntGolomb(size, out this.vui_mvcd_num_target_output_views_minus1[i]);

                this.vui_mvcd_view_id[i] = new uint[vui_mvcd_num_target_output_views_minus1[i] + 1];
                this.vui_mvcd_depth_flag[i] = new byte[vui_mvcd_num_target_output_views_minus1[i] + 1];
                this.vui_mvcd_texture_flag[i] = new byte[vui_mvcd_num_target_output_views_minus1[i] + 1];
                for (j = 0; j <= vui_mvcd_num_target_output_views_minus1[i]; j++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.vui_mvcd_view_id[i][j]);
                    size += stream.ReadUnsignedInt(size, 1, out this.vui_mvcd_depth_flag[i][j]);
                    size += stream.ReadUnsignedInt(size, 1, out this.vui_mvcd_texture_flag[i][j]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.vui_mvcd_timing_info_present_flag[i]);

                if (vui_mvcd_timing_info_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 32, out this.vui_mvcd_num_units_in_tick[i]);
                    size += stream.ReadUnsignedInt(size, 32, out this.vui_mvcd_time_scale[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.vui_mvcd_fixed_frame_rate_flag[i]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.vui_mvcd_nal_hrd_parameters_present_flag[i]);

                if (vui_mvcd_nal_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.ReadClass<HrdParameters>(size, context, () => new HrdParameters(), out this.hrd_parameters[i]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.vui_mvcd_vcl_hrd_parameters_present_flag[i]);

                if (vui_mvcd_vcl_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.ReadClass<HrdParameters>(size, context, () => new HrdParameters(), out this.hrd_parameters[i]);
                }

                if (vui_mvcd_nal_hrd_parameters_present_flag[i] != 0 ||
   vui_mvcd_vcl_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.vui_mvcd_low_delay_hrd_flag[i]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.vui_mvcd_pic_struct_present_flag[i]);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.WriteUnsignedIntGolomb(this.vui_mvcd_num_ops_minus1);

            for (i = 0; i <= vui_mvcd_num_ops_minus1; i++)
            {
                size += stream.WriteUnsignedInt(3, this.vui_mvcd_temporal_id[i]);
                size += stream.WriteUnsignedIntGolomb(this.vui_mvcd_num_target_output_views_minus1[i]);

                for (j = 0; j <= vui_mvcd_num_target_output_views_minus1[i]; j++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.vui_mvcd_view_id[i][j]);
                    size += stream.WriteUnsignedInt(1, this.vui_mvcd_depth_flag[i][j]);
                    size += stream.WriteUnsignedInt(1, this.vui_mvcd_texture_flag[i][j]);
                }
                size += stream.WriteUnsignedInt(1, this.vui_mvcd_timing_info_present_flag[i]);

                if (vui_mvcd_timing_info_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(32, this.vui_mvcd_num_units_in_tick[i]);
                    size += stream.WriteUnsignedInt(32, this.vui_mvcd_time_scale[i]);
                    size += stream.WriteUnsignedInt(1, this.vui_mvcd_fixed_frame_rate_flag[i]);
                }
                size += stream.WriteUnsignedInt(1, this.vui_mvcd_nal_hrd_parameters_present_flag[i]);

                if (vui_mvcd_nal_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.WriteClass<HrdParameters>(context, this.hrd_parameters[i]);
                }
                size += stream.WriteUnsignedInt(1, this.vui_mvcd_vcl_hrd_parameters_present_flag[i]);

                if (vui_mvcd_vcl_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.WriteClass<HrdParameters>(context, this.hrd_parameters[i]);
                }

                if (vui_mvcd_nal_hrd_parameters_present_flag[i] != 0 ||
   vui_mvcd_vcl_hrd_parameters_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.vui_mvcd_low_delay_hrd_flag[i]);
                }
                size += stream.WriteUnsignedInt(1, this.vui_mvcd_pic_struct_present_flag[i]);
            }

            return size;
        }

    }

    /*
   

nal_unit_header_3davc_extension() {  
 view_idx All u(8) 
 depth_flag All u(1) 
 non_idr_flag All u(1) 
 temporal_id All u(3) 
 anchor_pic_flag All u(1) 
 inter_view_flag All u(1) 
}
    */
    public class NalUnitHeader3davcExtension : IItuSerializable
    {
        private uint view_idx;
        public uint ViewIdx { get { return view_idx; } set { view_idx = value; } }
        private byte depth_flag;
        public byte DepthFlag { get { return depth_flag; } set { depth_flag = value; } }
        private byte non_idr_flag;
        public byte NonIdrFlag { get { return non_idr_flag; } set { non_idr_flag = value; } }
        private uint temporal_id;
        public uint TemporalId { get { return temporal_id; } set { temporal_id = value; } }
        private byte anchor_pic_flag;
        public byte AnchorPicFlag { get { return anchor_pic_flag; } set { anchor_pic_flag = value; } }
        private byte inter_view_flag;
        public byte InterViewFlag { get { return inter_view_flag; } set { inter_view_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public NalUnitHeader3davcExtension()
        {

        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 8, out this.view_idx);
            size += stream.ReadUnsignedInt(size, 1, out this.depth_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.non_idr_flag);
            size += stream.ReadUnsignedInt(size, 3, out this.temporal_id);
            size += stream.ReadUnsignedInt(size, 1, out this.anchor_pic_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.inter_view_flag);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(8, this.view_idx);
            size += stream.WriteUnsignedInt(1, this.depth_flag);
            size += stream.WriteUnsignedInt(1, this.non_idr_flag);
            size += stream.WriteUnsignedInt(3, this.temporal_id);
            size += stream.WriteUnsignedInt(1, this.anchor_pic_flag);
            size += stream.WriteUnsignedInt(1, this.inter_view_flag);

            return size;
        }

    }

    /*
  

seq_parameter_set_3davc_extension() {  
 if( NumDepthViews > 0 ) {   
  three_dv_acquisition_idc 0 ue(v) 
  for( i = 0; i < NumDepthViews; i++ )   
   view_id_3dv[ i ] 0 ue(v) 
  if( three_dv_acquisition_idc ) {   
   depth_ranges( NumDepthViews, 2, 0 )   
   vsp_param( NumDepthViews, 2, 0  )   
  }   
  reduced_resolution_flag 0 u(1) 
  if( reduced_resolution_flag ) {   
   depth_pic_width_in_mbs_minus1 0 ue(v) 
   depth_pic_height_in_map_units_minus1 0 ue(v) 
   depth_hor_mult_minus1 0 ue(v) 
   depth_ver_mult_minus1 0 ue(v) 
   depth_hor_rsh 0 ue(v) 
   depth_ver_rsh 0 ue(v) 
  }   
  depth_frame_cropping_flag 0 u(1) 
  if( depth_frame_cropping_flag ) {   
   depth_frame_crop_left_offset 0 ue(v) 
   depth_frame_crop_right_offset 0 ue(v) 
   depth_frame_crop_top_offset 0 ue(v) 
   depth_frame_crop_bottom_offset 0 ue(v) 
  }   
  grid_pos_num_views 0 ue(v) 
  for( i = 0; i < grid_pos_num_views; i++ ) {   
   grid_pos_view_id[ i ] 0 ue(v) 
   grid_pos_x[ grid_pos_view_id[ i ] ] 0 se(v) 
   grid_pos_y[ grid_pos_view_id[ i ] ] 0 se(v) 
  }   
  slice_header_prediction_flag 0 u(1)
    seq_view_synthesis_flag 0 u(1) 
 }   
 alc_sps_enable_flag 0 u(1) 
 enable_rle_skip_flag 0 u(1) 
 if( !AllViewsPairedFlag ) {   
  for( i = 1; i <= num_views_minus1; i++ )   
   if( texture_view_present_flag[ i ] ) {   
    num_anchor_refs_l0[ i ] 0 ue(v) 
    for( j = 0; j < num_anchor_refs_l0[ i ]; j++ )   
     anchor_ref_l0[ i ][ j ] 0 ue(v) 
    num_anchor_refs_l1[ i ] 0 ue(v) 
    for( j = 0; j < num_anchor_refs_l1[ i ]; j++ )   
     anchor_ref_l1[ i ][ j ] 0 ue(v) 
   }   
  for( i = 1; i <= num_views_minus1; i++ )   
   if( texture_view_present_flag[ i ] ) {   
    num_non_anchor_refs_l0[ i ] 0 ue(v) 
    for( j = 0; j < num_non_anchor_refs_l0[ i ]; j++ )   
     non_anchor_ref_l0[ i ][ j ] 0 ue(v) 
    num_non_anchor_refs_l1[ i ] 0 ue(v) 
    for( j = 0; j < num_non_anchor_refs_l1[ i ]; j++ )   
     non_anchor_ref_l1[ i ][ j ] 0 ue(v) 
   }   
 }   
}
    */
    public class SeqParameterSet3davcExtension : IItuSerializable
    {
        private uint three_dv_acquisition_idc;
        public uint ThreeDvAcquisitionIdc { get { return three_dv_acquisition_idc; } set { three_dv_acquisition_idc = value; } }
        private uint[] view_id_3dv;
        public uint[] ViewId3dv { get { return view_id_3dv; } set { view_id_3dv = value; } }
        private DepthRanges depth_ranges;
        public DepthRanges DepthRanges { get { return depth_ranges; } set { depth_ranges = value; } }
        private VspParam vsp_param;
        public VspParam VspParam { get { return vsp_param; } set { vsp_param = value; } }
        private byte reduced_resolution_flag;
        public byte ReducedResolutionFlag { get { return reduced_resolution_flag; } set { reduced_resolution_flag = value; } }
        private uint depth_pic_width_in_mbs_minus1;
        public uint DepthPicWidthInMbsMinus1 { get { return depth_pic_width_in_mbs_minus1; } set { depth_pic_width_in_mbs_minus1 = value; } }
        private uint depth_pic_height_in_map_units_minus1;
        public uint DepthPicHeightInMapUnitsMinus1 { get { return depth_pic_height_in_map_units_minus1; } set { depth_pic_height_in_map_units_minus1 = value; } }
        private uint depth_hor_mult_minus1;
        public uint DepthHorMultMinus1 { get { return depth_hor_mult_minus1; } set { depth_hor_mult_minus1 = value; } }
        private uint depth_ver_mult_minus1;
        public uint DepthVerMultMinus1 { get { return depth_ver_mult_minus1; } set { depth_ver_mult_minus1 = value; } }
        private uint depth_hor_rsh;
        public uint DepthHorRsh { get { return depth_hor_rsh; } set { depth_hor_rsh = value; } }
        private uint depth_ver_rsh;
        public uint DepthVerRsh { get { return depth_ver_rsh; } set { depth_ver_rsh = value; } }
        private byte depth_frame_cropping_flag;
        public byte DepthFrameCroppingFlag { get { return depth_frame_cropping_flag; } set { depth_frame_cropping_flag = value; } }
        private uint depth_frame_crop_left_offset;
        public uint DepthFrameCropLeftOffset { get { return depth_frame_crop_left_offset; } set { depth_frame_crop_left_offset = value; } }
        private uint depth_frame_crop_right_offset;
        public uint DepthFrameCropRightOffset { get { return depth_frame_crop_right_offset; } set { depth_frame_crop_right_offset = value; } }
        private uint depth_frame_crop_top_offset;
        public uint DepthFrameCropTopOffset { get { return depth_frame_crop_top_offset; } set { depth_frame_crop_top_offset = value; } }
        private uint depth_frame_crop_bottom_offset;
        public uint DepthFrameCropBottomOffset { get { return depth_frame_crop_bottom_offset; } set { depth_frame_crop_bottom_offset = value; } }
        private uint grid_pos_num_views;
        public uint GridPosNumViews { get { return grid_pos_num_views; } set { grid_pos_num_views = value; } }
        private uint[] grid_pos_view_id;
        public uint[] GridPosViewId { get { return grid_pos_view_id; } set { grid_pos_view_id = value; } }
        private int[] grid_pos_x;
        public int[] GridPosx { get { return grid_pos_x; } set { grid_pos_x = value; } }
        private int[] grid_pos_y;
        public int[] GridPosy { get { return grid_pos_y; } set { grid_pos_y = value; } }
        private byte slice_header_prediction_flag;
        public byte SliceHeaderPredictionFlag { get { return slice_header_prediction_flag; } set { slice_header_prediction_flag = value; } }
        private byte seq_view_synthesis_flag;
        public byte SeqViewSynthesisFlag { get { return seq_view_synthesis_flag; } set { seq_view_synthesis_flag = value; } }
        private byte alc_sps_enable_flag;
        public byte AlcSpsEnableFlag { get { return alc_sps_enable_flag; } set { alc_sps_enable_flag = value; } }
        private byte enable_rle_skip_flag;
        public byte EnableRleSkipFlag { get { return enable_rle_skip_flag; } set { enable_rle_skip_flag = value; } }
        private uint[] num_anchor_refs_l0;
        public uint[] NumAnchorRefsL0 { get { return num_anchor_refs_l0; } set { num_anchor_refs_l0 = value; } }
        private uint[][] anchor_ref_l0;
        public uint[][] AnchorRefL0 { get { return anchor_ref_l0; } set { anchor_ref_l0 = value; } }
        private uint[] num_anchor_refs_l1;
        public uint[] NumAnchorRefsL1 { get { return num_anchor_refs_l1; } set { num_anchor_refs_l1 = value; } }
        private uint[][] anchor_ref_l1;
        public uint[][] AnchorRefL1 { get { return anchor_ref_l1; } set { anchor_ref_l1 = value; } }
        private uint[] num_non_anchor_refs_l0;
        public uint[] NumNonAnchorRefsL0 { get { return num_non_anchor_refs_l0; } set { num_non_anchor_refs_l0 = value; } }
        private uint[][] non_anchor_ref_l0;
        public uint[][] NonAnchorRefL0 { get { return non_anchor_ref_l0; } set { non_anchor_ref_l0 = value; } }
        private uint[] num_non_anchor_refs_l1;
        public uint[] NumNonAnchorRefsL1 { get { return num_non_anchor_refs_l1; } set { num_non_anchor_refs_l1 = value; } }
        private uint[][] non_anchor_ref_l1;
        public uint[][] NonAnchorRefL1 { get { return non_anchor_ref_l1; } set { non_anchor_ref_l1 = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SeqParameterSet3davcExtension()
        {

        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;

            if (context.SubsetSps.SeqParameterSetMvcdExtension.NumDepthViews > 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.three_dv_acquisition_idc);

                this.view_id_3dv = new uint[context.SubsetSps.SeqParameterSetMvcdExtension.NumDepthViews];
                for (i = 0; i < context.SubsetSps.SeqParameterSetMvcdExtension.NumDepthViews; i++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.view_id_3dv[i]);
                }

                if (three_dv_acquisition_idc != 0)
                {
                    size += stream.ReadClass<DepthRanges>(size, context, () => new DepthRanges(context.SubsetSps.SeqParameterSetMvcdExtension.NumDepthViews, 2, 0), out this.depth_ranges);
                    size += stream.ReadClass<VspParam>(size, context, () => new VspParam(context.SubsetSps.SeqParameterSetMvcdExtension.NumDepthViews, 2, 0), out this.vsp_param);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.reduced_resolution_flag);

                if (reduced_resolution_flag != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.depth_pic_width_in_mbs_minus1);
                    size += stream.ReadUnsignedIntGolomb(size, out this.depth_pic_height_in_map_units_minus1);
                    size += stream.ReadUnsignedIntGolomb(size, out this.depth_hor_mult_minus1);
                    size += stream.ReadUnsignedIntGolomb(size, out this.depth_ver_mult_minus1);
                    size += stream.ReadUnsignedIntGolomb(size, out this.depth_hor_rsh);
                    size += stream.ReadUnsignedIntGolomb(size, out this.depth_ver_rsh);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.depth_frame_cropping_flag);

                if (depth_frame_cropping_flag != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.depth_frame_crop_left_offset);
                    size += stream.ReadUnsignedIntGolomb(size, out this.depth_frame_crop_right_offset);
                    size += stream.ReadUnsignedIntGolomb(size, out this.depth_frame_crop_top_offset);
                    size += stream.ReadUnsignedIntGolomb(size, out this.depth_frame_crop_bottom_offset);
                }
                size += stream.ReadUnsignedIntGolomb(size, out this.grid_pos_num_views);

                this.grid_pos_view_id = new uint[grid_pos_num_views];
                this.grid_pos_x = new int[grid_pos_num_views];
                this.grid_pos_y = new int[grid_pos_num_views];
                for (i = 0; i < grid_pos_num_views; i++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.grid_pos_view_id[i]);
                    size += stream.ReadSignedIntGolomb(size, out this.grid_pos_x[grid_pos_view_id[i]]);
                    size += stream.ReadSignedIntGolomb(size, out this.grid_pos_y[grid_pos_view_id[i]]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.slice_header_prediction_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.seq_view_synthesis_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.alc_sps_enable_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.enable_rle_skip_flag);

            if (((Func<uint>)(() =>
            {
                uint AllViewsPairedFlag = 1;
                for (int i = 1; i <= context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1; i++)
                    AllViewsPairedFlag = (uint)((AllViewsPairedFlag != 0 && context.SubsetSps.SeqParameterSetMvcdExtension.DepthViewPresentFlag[i] != 0 && context.SubsetSps.SeqParameterSetMvcdExtension.TextureViewPresentFlag[i] != 0) ? 1 : 0);
                return AllViewsPairedFlag;
            })).Invoke() == 0)
            {

                this.num_anchor_refs_l0 = new uint[context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1 + 1];
                this.anchor_ref_l0 = new uint[context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1 + 1][];
                this.num_anchor_refs_l1 = new uint[context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1 + 1];
                this.anchor_ref_l1 = new uint[context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1 + 1][];
                for (i = 1; i <= context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1; i++)
                {

                    if (context.SubsetSps.SeqParameterSetMvcdExtension.TextureViewPresentFlag.Select(x => (uint)x).ToArray()[i] != 0)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.num_anchor_refs_l0[i]);

                        this.anchor_ref_l0[i] = new uint[num_anchor_refs_l0[i]];
                        for (j = 0; j < num_anchor_refs_l0[i]; j++)
                        {
                            size += stream.ReadUnsignedIntGolomb(size, out this.anchor_ref_l0[i][j]);
                        }
                        size += stream.ReadUnsignedIntGolomb(size, out this.num_anchor_refs_l1[i]);

                        this.anchor_ref_l1[i] = new uint[num_anchor_refs_l1[i]];
                        for (j = 0; j < num_anchor_refs_l1[i]; j++)
                        {
                            size += stream.ReadUnsignedIntGolomb(size, out this.anchor_ref_l1[i][j]);
                        }
                    }
                }

                this.num_non_anchor_refs_l0 = new uint[context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1 + 1];
                this.non_anchor_ref_l0 = new uint[context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1 + 1][];
                this.num_non_anchor_refs_l1 = new uint[context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1 + 1];
                this.non_anchor_ref_l1 = new uint[context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1 + 1][];
                for (i = 1; i <= context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1; i++)
                {

                    if (context.SubsetSps.SeqParameterSetMvcdExtension.TextureViewPresentFlag.Select(x => (uint)x).ToArray()[i] != 0)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.num_non_anchor_refs_l0[i]);

                        this.non_anchor_ref_l0[i] = new uint[num_non_anchor_refs_l0[i]];
                        for (j = 0; j < num_non_anchor_refs_l0[i]; j++)
                        {
                            size += stream.ReadUnsignedIntGolomb(size, out this.non_anchor_ref_l0[i][j]);
                        }
                        size += stream.ReadUnsignedIntGolomb(size, out this.num_non_anchor_refs_l1[i]);

                        this.non_anchor_ref_l1[i] = new uint[num_non_anchor_refs_l1[i]];
                        for (j = 0; j < num_non_anchor_refs_l1[i]; j++)
                        {
                            size += stream.ReadUnsignedIntGolomb(size, out this.non_anchor_ref_l1[i][j]);
                        }
                    }
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;

            if (context.SubsetSps.SeqParameterSetMvcdExtension.NumDepthViews > 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.three_dv_acquisition_idc);

                for (i = 0; i < context.SubsetSps.SeqParameterSetMvcdExtension.NumDepthViews; i++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.view_id_3dv[i]);
                }

                if (three_dv_acquisition_idc != 0)
                {
                    size += stream.WriteClass<DepthRanges>(context, this.depth_ranges);
                    size += stream.WriteClass<VspParam>(context, this.vsp_param);
                }
                size += stream.WriteUnsignedInt(1, this.reduced_resolution_flag);

                if (reduced_resolution_flag != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.depth_pic_width_in_mbs_minus1);
                    size += stream.WriteUnsignedIntGolomb(this.depth_pic_height_in_map_units_minus1);
                    size += stream.WriteUnsignedIntGolomb(this.depth_hor_mult_minus1);
                    size += stream.WriteUnsignedIntGolomb(this.depth_ver_mult_minus1);
                    size += stream.WriteUnsignedIntGolomb(this.depth_hor_rsh);
                    size += stream.WriteUnsignedIntGolomb(this.depth_ver_rsh);
                }
                size += stream.WriteUnsignedInt(1, this.depth_frame_cropping_flag);

                if (depth_frame_cropping_flag != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.depth_frame_crop_left_offset);
                    size += stream.WriteUnsignedIntGolomb(this.depth_frame_crop_right_offset);
                    size += stream.WriteUnsignedIntGolomb(this.depth_frame_crop_top_offset);
                    size += stream.WriteUnsignedIntGolomb(this.depth_frame_crop_bottom_offset);
                }
                size += stream.WriteUnsignedIntGolomb(this.grid_pos_num_views);

                for (i = 0; i < grid_pos_num_views; i++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.grid_pos_view_id[i]);
                    size += stream.WriteSignedIntGolomb(this.grid_pos_x[grid_pos_view_id[i]]);
                    size += stream.WriteSignedIntGolomb(this.grid_pos_y[grid_pos_view_id[i]]);
                }
                size += stream.WriteUnsignedInt(1, this.slice_header_prediction_flag);
                size += stream.WriteUnsignedInt(1, this.seq_view_synthesis_flag);
            }
            size += stream.WriteUnsignedInt(1, this.alc_sps_enable_flag);
            size += stream.WriteUnsignedInt(1, this.enable_rle_skip_flag);

            if (((Func<uint>)(() =>
            {
                uint AllViewsPairedFlag = 1;
                for (int i = 1; i <= context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1; i++)
                    AllViewsPairedFlag = (uint)((AllViewsPairedFlag != 0 && context.SubsetSps.SeqParameterSetMvcdExtension.DepthViewPresentFlag[i] != 0 && context.SubsetSps.SeqParameterSetMvcdExtension.TextureViewPresentFlag[i] != 0) ? 1 : 0);
                return AllViewsPairedFlag;
            })).Invoke() == 0)
            {

                for (i = 1; i <= context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1; i++)
                {

                    if (context.SubsetSps.SeqParameterSetMvcdExtension.TextureViewPresentFlag.Select(x => (uint)x).ToArray()[i] != 0)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.num_anchor_refs_l0[i]);

                        for (j = 0; j < num_anchor_refs_l0[i]; j++)
                        {
                            size += stream.WriteUnsignedIntGolomb(this.anchor_ref_l0[i][j]);
                        }
                        size += stream.WriteUnsignedIntGolomb(this.num_anchor_refs_l1[i]);

                        for (j = 0; j < num_anchor_refs_l1[i]; j++)
                        {
                            size += stream.WriteUnsignedIntGolomb(this.anchor_ref_l1[i][j]);
                        }
                    }
                }

                for (i = 1; i <= context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1; i++)
                {

                    if (context.SubsetSps.SeqParameterSetMvcdExtension.TextureViewPresentFlag.Select(x => (uint)x).ToArray()[i] != 0)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.num_non_anchor_refs_l0[i]);

                        for (j = 0; j < num_non_anchor_refs_l0[i]; j++)
                        {
                            size += stream.WriteUnsignedIntGolomb(this.non_anchor_ref_l0[i][j]);
                        }
                        size += stream.WriteUnsignedIntGolomb(this.num_non_anchor_refs_l1[i]);

                        for (j = 0; j < num_non_anchor_refs_l1[i]; j++)
                        {
                            size += stream.WriteUnsignedIntGolomb(this.non_anchor_ref_l1[i][j]);
                        }
                    }
                }
            }

            return size;
        }

    }

    /*
  

depth_parameter_set_rbsp() {  
 depth_parameter_set_id 11 ue(v) 
 pred_direction 11 ue(v) 
 if( pred_direction  ==  0  ||  pred_direction  ==  1 ) {   
  ref_dps_id0 11 ue(v) 
  predWeight0 = 64   
 }   
 if( pred_direction  ==  0 ) {   
  ref_dps_id1 11 ue(v) 
  pred_weight0 11 u(6) 
  predWeight0 = pred_weight0   
 }   
 num_depth_views_minus1 11 ue(v) 
 depth_ranges( num_depth_views_minus1 + 1, pred_direction,  
  depth_parameter_set_id ) 
  
 vsp_param_flag 11 u(1) 
 if( vsp_param_flag )   
  vsp_param( num_depth_views_minus1 + 1, pred_direction,  
  depth_parameter_set_id ) 
  
 depth_param_additional_extension_flag 11 u(1) 
 nonlinear_depth_representation_num 11 ue(v) 
 for( i = 1; i <= nonlinear_depth_representation_num; i++ )   
  nonlinear_depth_representation_model[ i ] 11 ue(v) 
 if(depth_param_additional_extension_flag  ==  1 )   
  while( more_rbsp_data() )   
   depth_param_additional_extension_data_flag 11 u(1) 
 rbsp_trailing_bits()   
}
    */
    public class DepthParameterSetRbsp : IItuSerializable
    {
        private uint depth_parameter_set_id;
        public uint DepthParameterSetId { get { return depth_parameter_set_id; } set { depth_parameter_set_id = value; } }
        private uint pred_direction;
        public uint PredDirection { get { return pred_direction; } set { pred_direction = value; } }
        private uint ref_dps_id0;
        public uint RefDpsId0 { get { return ref_dps_id0; } set { ref_dps_id0 = value; } }
        private uint ref_dps_id1;
        public uint RefDpsId1 { get { return ref_dps_id1; } set { ref_dps_id1 = value; } }
        private uint pred_weight0;
        public uint PredWeight0 { get { return pred_weight0; } set { pred_weight0 = value; } }
        private uint num_depth_views_minus1;
        public uint NumDepthViewsMinus1 { get { return num_depth_views_minus1; } set { num_depth_views_minus1 = value; } }
        private DepthRanges depth_ranges;
        public DepthRanges DepthRanges { get { return depth_ranges; } set { depth_ranges = value; } }
        private byte vsp_param_flag;
        public byte VspParamFlag { get { return vsp_param_flag; } set { vsp_param_flag = value; } }
        private VspParam vsp_param;
        public VspParam VspParam { get { return vsp_param; } set { vsp_param = value; } }
        private byte depth_param_additional_extension_flag;
        public byte DepthParamAdditionalExtensionFlag { get { return depth_param_additional_extension_flag; } set { depth_param_additional_extension_flag = value; } }
        private uint nonlinear_depth_representation_num;
        public uint NonlinearDepthRepresentationNum { get { return nonlinear_depth_representation_num; } set { nonlinear_depth_representation_num = value; } }
        private uint[] nonlinear_depth_representation_model;
        public uint[] NonlinearDepthRepresentationModel { get { return nonlinear_depth_representation_model; } set { nonlinear_depth_representation_model = value; } }
        private byte depth_param_additional_extension_data_flag;
        public byte DepthParamAdditionalExtensionDataFlag { get { return depth_param_additional_extension_data_flag; } set { depth_param_additional_extension_data_flag = value; } }
        private RbspTrailingBits rbsp_trailing_bits;
        public RbspTrailingBits RbspTrailingBits { get { return rbsp_trailing_bits; } set { rbsp_trailing_bits = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public DepthParameterSetRbsp()
        {

        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint predWeight0 = 0;
            uint i = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.depth_parameter_set_id);
            size += stream.ReadUnsignedIntGolomb(size, out this.pred_direction);

            if (pred_direction == 0 || pred_direction == 1)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.ref_dps_id0);
                predWeight0 = 64;
            }

            if (pred_direction == 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.ref_dps_id1);
                size += stream.ReadUnsignedInt(size, 6, out this.pred_weight0);
                predWeight0 = pred_weight0;
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.num_depth_views_minus1);
            size += stream.ReadClass<DepthRanges>(size, context, () => new DepthRanges(num_depth_views_minus1 + 1, pred_direction,
  depth_parameter_set_id), out this.depth_ranges);
            size += stream.ReadUnsignedInt(size, 1, out this.vsp_param_flag);

            if (vsp_param_flag != 0)
            {
                size += stream.ReadClass<VspParam>(size, context, () => new VspParam(num_depth_views_minus1 + 1, pred_direction,
  depth_parameter_set_id), out this.vsp_param);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.depth_param_additional_extension_flag);
            size += stream.ReadUnsignedIntGolomb(size, out this.nonlinear_depth_representation_num);

            this.nonlinear_depth_representation_model = new uint[nonlinear_depth_representation_num];
            for (i = 1; i <= nonlinear_depth_representation_num; i++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.nonlinear_depth_representation_model[i]);
            }

            if (depth_param_additional_extension_flag == 1)
            {

                while (stream.ReadMoreRbspData(this))
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.depth_param_additional_extension_data_flag);
                }
            }
            size += stream.ReadClass<RbspTrailingBits>(size, context, () => new RbspTrailingBits(), out this.rbsp_trailing_bits);

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint predWeight0 = 0;
            uint i = 0;
            size += stream.WriteUnsignedIntGolomb(this.depth_parameter_set_id);
            size += stream.WriteUnsignedIntGolomb(this.pred_direction);

            if (pred_direction == 0 || pred_direction == 1)
            {
                size += stream.WriteUnsignedIntGolomb(this.ref_dps_id0);
                predWeight0 = 64;
            }

            if (pred_direction == 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.ref_dps_id1);
                size += stream.WriteUnsignedInt(6, this.pred_weight0);
                predWeight0 = pred_weight0;
            }
            size += stream.WriteUnsignedIntGolomb(this.num_depth_views_minus1);
            size += stream.WriteClass<DepthRanges>(context, this.depth_ranges);
            size += stream.WriteUnsignedInt(1, this.vsp_param_flag);

            if (vsp_param_flag != 0)
            {
                size += stream.WriteClass<VspParam>(context, this.vsp_param);
            }
            size += stream.WriteUnsignedInt(1, this.depth_param_additional_extension_flag);
            size += stream.WriteUnsignedIntGolomb(this.nonlinear_depth_representation_num);

            for (i = 1; i <= nonlinear_depth_representation_num; i++)
            {
                size += stream.WriteUnsignedIntGolomb(this.nonlinear_depth_representation_model[i]);
            }

            if (depth_param_additional_extension_flag == 1)
            {

                while (stream.WriteMoreRbspData(this))
                {
                    size += stream.WriteUnsignedInt(1, this.depth_param_additional_extension_data_flag);
                }
            }
            size += stream.WriteClass<RbspTrailingBits>(context, this.rbsp_trailing_bits);

            return size;
        }

    }

    /*
  

depth_ranges( numViews, predDirection, index ) {  
 z_near_flag 11 u(1) 
 z_far_flag 11 u(1) 
 if( z_near_flag )   
  three_dv_acquisition_element( numViews, predDirection, 7, index, 
   ZNearSign, ZNearExp, ZNearMantissa, ZNearManLen ) 
  
 if( z_far_flag )   
  three_dv_acquisition_element( numViews, predDirection, 7, index, 
   ZFarSign, ZFarExp, ZFarMantissa, ZFarManLen ) 
  
}
    */
    public class DepthRanges : IItuSerializable
    {
        private uint numViews;
        public uint NumViews { get { return numViews; } set { numViews = value; } }
        private uint predDirection;
        public uint PredDirection { get { return predDirection; } set { predDirection = value; } }
        private uint index;
        public uint Index { get { return index; } set { index = value; } }
        private byte z_near_flag;
        public byte zNearFlag { get { return z_near_flag; } set { z_near_flag = value; } }
        private byte z_far_flag;
        public byte zFarFlag { get { return z_far_flag; } set { z_far_flag = value; } }
        private ThreeDvAcquisitionElement three_dv_acquisition_element;
        public ThreeDvAcquisitionElement ThreeDvAcquisitionElement { get { return three_dv_acquisition_element; } set { three_dv_acquisition_element = value; } }
        private ThreeDvAcquisitionElement three_dv_acquisition_element0;
        public ThreeDvAcquisitionElement ThreeDvAcquisitionElement0 { get { return three_dv_acquisition_element0; } set { three_dv_acquisition_element0 = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public DepthRanges(uint numViews, uint predDirection, uint index)
        {
            this.numViews = numViews;
            this.predDirection = predDirection;
            this.index = index;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.z_near_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.z_far_flag);

            if (z_near_flag != 0)
            {
                size += stream.ReadClass<ThreeDvAcquisitionElement>(size, context, () => new ThreeDvAcquisitionElement(numViews, predDirection, 7, index,
   H264Helpers.GetArray2("ZNearSign"), H264Helpers.GetArray2("ZNearExp"), H264Helpers.GetArray2("ZNearMantissa"), H264Helpers.GetArray2("ZNearManLen")), out this.three_dv_acquisition_element);
            }

            if (z_far_flag != 0)
            {
                size += stream.ReadClass<ThreeDvAcquisitionElement>(size, context, () => new ThreeDvAcquisitionElement(numViews, predDirection, 7, index,
   H264Helpers.GetArray2("ZFarSign"), H264Helpers.GetArray2("ZFarExp"), H264Helpers.GetArray2("ZFarMantissa"), H264Helpers.GetArray2("ZFarManLen")), out this.three_dv_acquisition_element0);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.z_near_flag);
            size += stream.WriteUnsignedInt(1, this.z_far_flag);

            if (z_near_flag != 0)
            {
                size += stream.WriteClass<ThreeDvAcquisitionElement>(context, this.three_dv_acquisition_element);
            }

            if (z_far_flag != 0)
            {
                size += stream.WriteClass<ThreeDvAcquisitionElement>(context, this.three_dv_acquisition_element0);
            }

            return size;
        }

    }

    /*
  

three_dv_acquisition_element( numViews, predDirection, expLen, index, outSign, 
 outExp, outMantissa, outManLen ) { 
 
 if( numViews > 1 )   
  element_equal_flag 11 u(1) 
 if( element_equal_flag  ==  0 )   
  numValues = numViews   
 else   
  numValues = 1   
 for( i = 0; i < numValues; i++ ) {   
  if( predDirection  ==  2  &&  i  ==  0 ) {   
   mantissa_len_minus1 11 u(5) 
   outManLen[ index, i ] = mantissa_len_minus1 + 1   
  }   
  if( predDirection  ==  2 ) {   
   sign0 11 u(1) 
   outSign[ index, i ] = sign0   
   exponent0 11 u(v) 
   outExp[ index, i ] = exponent0   
   mantissa0 11 u(v) 
   outMantissa[ index, i ] = mantissa0   
  } else {   
   skip_flag 11 u(1) 
   if( skip_flag  ==  0 ) {   
    sign1 11 u(1) 
    outSign[ index, i ] = sign1   
    exponent_skip_flag 11 u(1) 
    if( exponent_skip_flag  ==  0 ) {   
     exponent1 11 u(v) 
     outExp[ index, i ] = exponent1   
    } else   
     outExp[ index, i ] = outExp[ ref_dps_id0, i ]   
    mantissa_diff 11 se(v) 
    if( predDirection  ==  0 )   
     mantissaPred = (( OutMantissa[ ref_dps_id0, i ] * predWeight0 + outMantissa[ ref_dps_id1, i ] * ( 64-predWeight0 ) + 32 ) >> 6 ) 
     else   
     mantissaPred = outMantissa[ ref_dps_id0, i ]   
    outMantissa[ index, i ] = mantissaPred + mantissa_diff   
    outManLen[ index, i ] = outManLen[ ref_dps_id0, i ]   
   } else {   
    outSign[ index, i ] = outSign[ ref_dps_id0, i ]   
    outExp[ index, i ] = outExp[ ref_dps_id0, i ]   
    outMantissa[ index, i ] = outMantissa[ ref_dps_id0, i ]   
    outManLen[ index, i ] = outManLen[ ref_dps_id0, i ]   
   }   
  }   
 }   
 if( element_equal_flag  ==  1 ) {   
  for( i = 1; i < num_views_minus1 + 1 - deltaFlag; i++ ) {   
   outSign[ index, i ] = outSign[ index, 0 ]   
   outExp[ index, i ] = outExp[ index, 0 ]   
   outMantissa[ index, i ] = outMantissa[ index, 0 ]   
   outManLen[ index, i ] = outManLen[ index, 0 ]   
  }   
 }   
}
    */
    public class ThreeDvAcquisitionElement : IItuSerializable
    {
        private uint numViews;
        public uint NumViews { get { return numViews; } set { numViews = value; } }
        private uint predDirection;
        public uint PredDirection { get { return predDirection; } set { predDirection = value; } }
        private uint expLen;
        public uint ExpLen { get { return expLen; } set { expLen = value; } }
        private uint index;
        public uint Index { get { return index; } set { index = value; } }
        private uint[,] outSign;
        public uint[,] OutSign { get { return outSign; } set { outSign = value; } }
        private uint[,] outExp;
        public uint[,] OutExp { get { return outExp; } set { outExp = value; } }
        private uint[,] outMantissa;
        public uint[,] OutMantissa { get { return outMantissa; } set { outMantissa = value; } }
        private uint[,] outManLen;
        public uint[,] OutManLen { get { return outManLen; } set { outManLen = value; } }
        private byte element_equal_flag;
        public byte ElementEqualFlag { get { return element_equal_flag; } set { element_equal_flag = value; } }
        private uint[] mantissa_len_minus1;
        public uint[] MantissaLenMinus1 { get { return mantissa_len_minus1; } set { mantissa_len_minus1 = value; } }
        private byte[] sign0;
        public byte[] Sign0 { get { return sign0; } set { sign0 = value; } }
        private uint[] exponent0;
        public uint[] Exponent0 { get { return exponent0; } set { exponent0 = value; } }
        private uint[] mantissa0;
        public uint[] Mantissa0 { get { return mantissa0; } set { mantissa0 = value; } }
        private byte[] skip_flag;
        public byte[] SkipFlag { get { return skip_flag; } set { skip_flag = value; } }
        private byte[] sign1;
        public byte[] Sign1 { get { return sign1; } set { sign1 = value; } }
        private byte[] exponent_skip_flag;
        public byte[] ExponentSkipFlag { get { return exponent_skip_flag; } set { exponent_skip_flag = value; } }
        private uint[] exponent1;
        public uint[] Exponent1 { get { return exponent1; } set { exponent1 = value; } }
        private int[] mantissa_diff;
        public int[] MantissaDiff { get { return mantissa_diff; } set { mantissa_diff = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ThreeDvAcquisitionElement(uint numViews, uint predDirection, uint expLen, uint index, uint[,] outSign, uint[,] outExp, uint[,] outMantissa, uint[,] outManLen)
        {
            this.numViews = numViews;
            this.predDirection = predDirection;
            this.expLen = expLen;
            this.index = index;
            this.outSign = outSign;
            this.outExp = outExp;
            this.outMantissa = outMantissa;
            this.outManLen = outManLen;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint numValues = 0;
            uint i = 0;
            uint mantissaPred = 0;

            if (numViews > 1)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.element_equal_flag);
            }

            if (element_equal_flag == 0)
            {
                numValues = numViews;
            }
            else
            {
                numValues = 1;
            }

            this.mantissa_len_minus1 = new uint[numValues];
            this.sign0 = new byte[numValues];
            this.exponent0 = new uint[numValues];
            this.mantissa0 = new uint[numValues];
            this.skip_flag = new byte[numValues];
            this.sign1 = new byte[numValues];
            this.exponent_skip_flag = new byte[numValues];
            this.exponent1 = new uint[numValues];
            this.mantissa_diff = new int[numValues];
            for (i = 0; i < numValues; i++)
            {

                if (predDirection == 2 && i == 0)
                {
                    size += stream.ReadUnsignedInt(size, 5, out this.mantissa_len_minus1[i]);
                    outManLen[index, i] = mantissa_len_minus1[i] + 1;
                }

                if (predDirection == 2)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.sign0[i]);
                    outSign[index, i] = sign0[i];
                    size += stream.ReadUnsignedIntVariable(size, context.SubsetSps.SeqParameterSet3davcExtension.DepthRanges.ThreeDvAcquisitionElement.ExpLen, out this.exponent0[i]);
                    outExp[index, i] = exponent0[i];
                    size += stream.ReadUnsignedIntVariable(size, (context.SubsetSps.SeqParameterSet3davcExtension.DepthRanges.ThreeDvAcquisitionElement.MantissaLenMinus1[i] + 1), out this.mantissa0[i]);
                    outMantissa[index, i] = mantissa0[i];
                }
                else
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.skip_flag[i]);

                    if (skip_flag[i] == 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.sign1[i]);
                        outSign[index, i] = sign1[i];
                        size += stream.ReadUnsignedInt(size, 1, out this.exponent_skip_flag[i]);

                        if (exponent_skip_flag[i] == 0)
                        {
                            size += stream.ReadUnsignedIntVariable(size, context.SubsetSps.SeqParameterSet3davcExtension.DepthRanges.ThreeDvAcquisitionElement.ExpLen, out this.exponent1[i]);
                            outExp[index, i] = exponent1[i];
                        }
                        else
                        {
                            outExp[index, i] = outExp[context.Dps.RefDpsId0, i];
                        }
                        size += stream.ReadSignedIntGolomb(size, out this.mantissa_diff[i]);

                        if (predDirection == 0)
                        {
                            mantissaPred = ((OutMantissa[context.Dps.RefDpsId0, i] * context.Dps.PredWeight0 + outMantissa[context.Dps.RefDpsId1, i] * (64 - context.Dps.PredWeight0) + 32) >> 6);
                        }
                        else
                        {
                            mantissaPred = outMantissa[context.Dps.RefDpsId0, i];
                        }
                        outMantissa[index, i] = (uint)(mantissaPred + mantissa_diff[i]);
                        outManLen[index, i] = outManLen[context.Dps.RefDpsId0, i];
                    }
                    else
                    {
                        outSign[index, i] = outSign[context.Dps.RefDpsId0, i];
                        outExp[index, i] = outExp[context.Dps.RefDpsId0, i];
                        outMantissa[index, i] = outMantissa[context.Dps.RefDpsId0, i];
                        outManLen[index, i] = outManLen[context.Dps.RefDpsId0, i];
                    }
                }
            }

            if (element_equal_flag == 1)
            {

                for (i = 1; i < context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1 + 1 - 0; i++)
                {
                    outSign[index, i] = outSign[index, 0];
                    outExp[index, i] = outExp[index, 0];
                    outMantissa[index, i] = outMantissa[index, 0];
                    outManLen[index, i] = outManLen[index, 0];
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint numValues = 0;
            uint i = 0;
            uint mantissaPred = 0;

            if (numViews > 1)
            {
                size += stream.WriteUnsignedInt(1, this.element_equal_flag);
            }

            if (element_equal_flag == 0)
            {
                numValues = numViews;
            }
            else
            {
                numValues = 1;
            }

            for (i = 0; i < numValues; i++)
            {

                if (predDirection == 2 && i == 0)
                {
                    size += stream.WriteUnsignedInt(5, this.mantissa_len_minus1[i]);
                    outManLen[index, i] = mantissa_len_minus1[i] + 1;
                }

                if (predDirection == 2)
                {
                    size += stream.WriteUnsignedInt(1, this.sign0[i]);
                    outSign[index, i] = sign0[i];
                    size += stream.WriteUnsignedIntVariable(context.SubsetSps.SeqParameterSet3davcExtension.DepthRanges.ThreeDvAcquisitionElement.ExpLen, this.exponent0[i]);
                    outExp[index, i] = exponent0[i];
                    size += stream.WriteUnsignedIntVariable((context.SubsetSps.SeqParameterSet3davcExtension.DepthRanges.ThreeDvAcquisitionElement.MantissaLenMinus1[i] + 1), this.mantissa0[i]);
                    outMantissa[index, i] = mantissa0[i];
                }
                else
                {
                    size += stream.WriteUnsignedInt(1, this.skip_flag[i]);

                    if (skip_flag[i] == 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.sign1[i]);
                        outSign[index, i] = sign1[i];
                        size += stream.WriteUnsignedInt(1, this.exponent_skip_flag[i]);

                        if (exponent_skip_flag[i] == 0)
                        {
                            size += stream.WriteUnsignedIntVariable(context.SubsetSps.SeqParameterSet3davcExtension.DepthRanges.ThreeDvAcquisitionElement.ExpLen, this.exponent1[i]);
                            outExp[index, i] = exponent1[i];
                        }
                        else
                        {
                            outExp[index, i] = outExp[context.Dps.RefDpsId0, i];
                        }
                        size += stream.WriteSignedIntGolomb(this.mantissa_diff[i]);

                        if (predDirection == 0)
                        {
                            mantissaPred = ((OutMantissa[context.Dps.RefDpsId0, i] * context.Dps.PredWeight0 + outMantissa[context.Dps.RefDpsId1, i] * (64 - context.Dps.PredWeight0) + 32) >> 6);
                        }
                        else
                        {
                            mantissaPred = outMantissa[context.Dps.RefDpsId0, i];
                        }
                        outMantissa[index, i] = (uint)(mantissaPred + mantissa_diff[i]);
                        outManLen[index, i] = outManLen[context.Dps.RefDpsId0, i];
                    }
                    else
                    {
                        outSign[index, i] = outSign[context.Dps.RefDpsId0, i];
                        outExp[index, i] = outExp[context.Dps.RefDpsId0, i];
                        outMantissa[index, i] = outMantissa[context.Dps.RefDpsId0, i];
                        outManLen[index, i] = outManLen[context.Dps.RefDpsId0, i];
                    }
                }
            }

            if (element_equal_flag == 1)
            {

                for (i = 1; i < context.SubsetSps.SeqParameterSetMvcExtension.NumViewsMinus1 + 1 - 0; i++)
                {
                    outSign[index, i] = outSign[index, 0];
                    outExp[index, i] = outExp[index, 0];
                    outMantissa[index, i] = outMantissa[index, 0];
                    outManLen[index, i] = outManLen[index, 0];
                }
            }

            return size;
        }

    }

    /*
  

vsp_param( numViews, predDirection, index ) {  
 for( i = 0; i < numViews; i++ )   
  for( j = 0; j < i; j++ ) {   
   disparity_diff_wji[ j ][ i ] 0 ue(v) 
   disparity_diff_oji[ j ][ i ] 0 ue(v) 
   disparity_diff_wij[ i ][ j ] 0 ue(v) 
   disparity_diff_oij[ i ][ j ] 0 ue(v) 
  }   
}
    */
    public class VspParam : IItuSerializable
    {
        private uint numViews;
        public uint NumViews { get { return numViews; } set { numViews = value; } }
        private uint predDirection;
        public uint PredDirection { get { return predDirection; } set { predDirection = value; } }
        private uint index;
        public uint Index { get { return index; } set { index = value; } }
        private uint[][] disparity_diff_wji;
        public uint[][] DisparityDiffWji { get { return disparity_diff_wji; } set { disparity_diff_wji = value; } }
        private uint[][] disparity_diff_oji;
        public uint[][] DisparityDiffOji { get { return disparity_diff_oji; } set { disparity_diff_oji = value; } }
        private uint[][] disparity_diff_wij;
        public uint[][] DisparityDiffWij { get { return disparity_diff_wij; } set { disparity_diff_wij = value; } }
        private uint[][] disparity_diff_oij;
        public uint[][] DisparityDiffOij { get { return disparity_diff_oij; } set { disparity_diff_oij = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public VspParam(uint numViews, uint predDirection, uint index)
        {
            this.numViews = numViews;
            this.predDirection = predDirection;
            this.index = index;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;

            this.disparity_diff_wji = new uint[numViews][];
            this.disparity_diff_oji = new uint[numViews][];
            this.disparity_diff_wij = new uint[numViews][];
            this.disparity_diff_oij = new uint[numViews][];
            for (i = 0; i < numViews; i++)
            {

                this.disparity_diff_wji[i] = new uint[i];
                this.disparity_diff_oji[i] = new uint[i];
                this.disparity_diff_wij[i] = new uint[i];
                this.disparity_diff_oij[i] = new uint[i];
                for (j = 0; j < i; j++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.disparity_diff_wji[j][i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.disparity_diff_oji[j][i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.disparity_diff_wij[i][j]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.disparity_diff_oij[i][j]);
                }
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;

            for (i = 0; i < numViews; i++)
            {

                for (j = 0; j < i; j++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.disparity_diff_wji[j][i]);
                    size += stream.WriteUnsignedIntGolomb(this.disparity_diff_oji[j][i]);
                    size += stream.WriteUnsignedIntGolomb(this.disparity_diff_wij[i][j]);
                    size += stream.WriteUnsignedIntGolomb(this.disparity_diff_oij[i][j]);
                }
            }

            return size;
        }

    }

    /*
   

green_metadata(payloadSize) {
    green_metadata_type 5 u(8)
    if (green_metadata_type == 0) {
        period_type 5 u(8)

        if (period_type == 2) {
            num_seconds 5 u(16)
        }
        else if (period_type == 3) {
            num_pictures 5 u(16)
        }

        percent_non_zero_macroblocks 5 u(8)
        percent_intra_coded_macroblocks 5 u(8)
        percent_six_tap_filtering 5 u(8)
        percent_alpha_point_deblocking_instance 5 u(8)
    }
    else if (green_metadata_type == 1) {
        xsd_metric_type 5 u(8)
        xsd_metric_value 5 u(16)
    }
}
    */
    public class GreenMetadata : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint green_metadata_type;
        public uint GreenMetadataType { get { return green_metadata_type; } set { green_metadata_type = value; } }
        private uint period_type;
        public uint PeriodType { get { return period_type; } set { period_type = value; } }
        private uint num_seconds;
        public uint NumSeconds { get { return num_seconds; } set { num_seconds = value; } }
        private uint num_pictures;
        public uint NumPictures { get { return num_pictures; } set { num_pictures = value; } }
        private uint percent_non_zero_macroblocks;
        public uint PercentNonZeroMacroblocks { get { return percent_non_zero_macroblocks; } set { percent_non_zero_macroblocks = value; } }
        private uint percent_intra_coded_macroblocks;
        public uint PercentIntraCodedMacroblocks { get { return percent_intra_coded_macroblocks; } set { percent_intra_coded_macroblocks = value; } }
        private uint percent_six_tap_filtering;
        public uint PercentSixTapFiltering { get { return percent_six_tap_filtering; } set { percent_six_tap_filtering = value; } }
        private uint percent_alpha_point_deblocking_instance;
        public uint PercentAlphaPointDeblockingInstance { get { return percent_alpha_point_deblocking_instance; } set { percent_alpha_point_deblocking_instance = value; } }
        private uint xsd_metric_type;
        public uint XsdMetricType { get { return xsd_metric_type; } set { xsd_metric_type = value; } }
        private uint xsd_metric_value;
        public uint XsdMetricValue { get { return xsd_metric_value; } set { xsd_metric_value = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public GreenMetadata(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 8, out this.green_metadata_type);

            if (green_metadata_type == 0)
            {
                size += stream.ReadUnsignedInt(size, 8, out this.period_type);

                if (period_type == 2)
                {
                    size += stream.ReadUnsignedInt(size, 16, out this.num_seconds);
                }
                else if (period_type == 3)
                {
                    size += stream.ReadUnsignedInt(size, 16, out this.num_pictures);
                }
                size += stream.ReadUnsignedInt(size, 8, out this.percent_non_zero_macroblocks);
                size += stream.ReadUnsignedInt(size, 8, out this.percent_intra_coded_macroblocks);
                size += stream.ReadUnsignedInt(size, 8, out this.percent_six_tap_filtering);
                size += stream.ReadUnsignedInt(size, 8, out this.percent_alpha_point_deblocking_instance);
            }
            else if (green_metadata_type == 1)
            {
                size += stream.ReadUnsignedInt(size, 8, out this.xsd_metric_type);
                size += stream.ReadUnsignedInt(size, 16, out this.xsd_metric_value);
            }

            return size;
        }

        public ulong Write(H264Context context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(8, this.green_metadata_type);

            if (green_metadata_type == 0)
            {
                size += stream.WriteUnsignedInt(8, this.period_type);

                if (period_type == 2)
                {
                    size += stream.WriteUnsignedInt(16, this.num_seconds);
                }
                else if (period_type == 3)
                {
                    size += stream.WriteUnsignedInt(16, this.num_pictures);
                }
                size += stream.WriteUnsignedInt(8, this.percent_non_zero_macroblocks);
                size += stream.WriteUnsignedInt(8, this.percent_intra_coded_macroblocks);
                size += stream.WriteUnsignedInt(8, this.percent_six_tap_filtering);
                size += stream.WriteUnsignedInt(8, this.percent_alpha_point_deblocking_instance);
            }
            else if (green_metadata_type == 1)
            {
                size += stream.WriteUnsignedInt(8, this.xsd_metric_type);
                size += stream.WriteUnsignedInt(16, this.xsd_metric_value);
            }

            return size;
        }

    }

}
