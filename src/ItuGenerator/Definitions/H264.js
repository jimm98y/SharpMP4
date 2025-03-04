nal_unit( NumBytesInNALunit ) { 
 forbidden_zero_bit All f(1) 
 nal_ref_idc All u(2) 
 nal_unit_type All u(5) 
 NumBytesInRBSP = 0   
 nalUnitHeaderBytes = 1   
 if( nal_unit_type  ==  14  ||  nal_unit_type  ==  20  ||  nal_unit_type  ==  21 ) { 
  
  if( nal_unit_type !=  21 )   
   svc_extension_flag All u(1) 
  else   
   avc_3d_extension_flag All u(1) 
  if( svc_extension_flag ) {   
   nal_unit_header_svc_extension() /* specified in Annex G */ All  
   nalUnitHeaderBytes += 3   
  } else if( avc_3d_extension_flag ) {   
   nal_unit_header_3davc_extension() /* specified in Annex J */   
   nalUnitHeaderBytes += 2   
  } else {   
   nal_unit_header_mvc_extension() /* specified in Annex H */ All  
   nalUnitHeaderBytes += 3   
  }   
 }   
 for( i = nalUnitHeaderBytes; i < NumBytesInNALunit; i++ ) {   
  if( i + 2 < NumBytesInNALunit && next_bits( 24 )  ==  0x000003 ) {   
   rbsp_byte[ NumBytesInRBSP++ ] All b(8) 
   rbsp_byte[ NumBytesInRBSP++ ] All b(8) 
   i += 2   
   emulation_prevention_three_byte  /* equal to 0x03 */ All f(8) 
  } else   
   rbsp_byte[ NumBytesInRBSP++ ] All b(8) 
 }   
}  


seq_parameter_set_rbsp() { 
 seq_parameter_set_data() 0  
 rbsp_trailing_bits() 0  
} 


seq_parameter_set_data() { 
 profile_idc 0 u(8) 
 constraint_set0_flag 0 u(1) 
 constraint_set1_flag 0 u(1) 
 constraint_set2_flag 0 u(1) 
 constraint_set3_flag 0 u(1) 
 constraint_set4_flag 0 u(1) 
 constraint_set5_flag 0 u(1) 
 reserved_zero_2bits  /* equal to 0 */ 0 u(2) 
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

scaling_list( scalingList, sizeOfScalingList, useDefaultScalingMatrixFlag ) { 
 lastScale = 8   
 nextScale = 8   
 for( j = 0; j < sizeOfScalingList; j++ ) {   
  if( nextScale != 0 ) {   
   delta_scale 0 | 1 se(v) 
   nextScale = ( lastScale + delta_scale + 256 ) % 256   
   useDefaultScalingMatrixFlag = ( j  ==  0 && nextScale  ==  0 )   
  }   
  scalingList[ j ] = ( nextScale  ==  0 ) ? lastScale : nextScale   
  lastScale = scalingList[ j ]   
 }   
}

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

subset_seq_parameter_set_rbsp() { 
 seq_parameter_set_data() 0  
 if( profile_idc  ==  83  ||  profile_idc  ==  86 ) {   
  seq_parameter_set_svc_extension()  /* specified in Annex G */ 0  
  svc_vui_parameters_present_flag 0 u(1) 
  if( svc_vui_parameters_present_flag  ==  1 )   
   svc_vui_parameters_extension()  /* specified in Annex G */ 0  
 } else if( profile_idc  ==  118  ||  profile_idc  ==  128  || 
  profile_idc  ==  134  ) { 
  
  bit_equal_to_one  /* equal to 1 */ 0 f(1) 
  seq_parameter_set_mvc_extension()  /* specified in Annex H */ 0  
  mvc_vui_parameters_present_flag 0 u(1) 
  if( mvc_vui_parameters_present_flag  ==  1 )   
   mvc_vui_parameters_extension()  /* specified in Annex H */ 0  
 } else if( profile_idc  ==  138 ||  profile_idc  ==  135 ) {   
  bit_equal_to_one  /* equal to 1 */ 0 f(1) 
  seq_parameter_set_mvcd_extension()  /* specified in Annex I */   
 } else if( profile_idc  ==  139 ) {   
  bit_equal_to_one  /* equal to 1 */ 0 f(1) 
  seq_parameter_set_mvcd_extension()  /* specified in Annex I */ 0  
  seq_parameter_set_3davc_extension() /* specified in Annex J */ 0  
 }   
 additional_extension2_flag 0 u(1) 
 if( additional_extension2_flag  ==  1 )   
  while( more_rbsp_data() )   
   additional_extension2_data_flag 0 u(1) 
 rbsp_trailing_bits() 0  
}


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

sei_rbsp() {
    do   
    {
        sei_message() 5
    } while( more_rbsp_data() )   
  rbsp_trailing_bits() 5  
}  

sei_message() { 
  payloadType = 0   
  while( next_bits( 8 )  ==  0xFF ) {   
    ff_byte  /* equal to 0xFF */ 5 f(8) 
    payloadType += 255   
  }   
  last_payload_type_byte 5 u(8) 
  payloadType += last_payload_type_byte   
  payloadSize = 0   
  while( next_bits( 8 )  ==  0xFF ) {   
    ff_byte  /* equal to 0xFF */ 5 f(8) 
    payloadSize += 255   
  }   
  last_payload_size_byte 5 u(8) 
  payloadSize += last_payload_size_byte   
  sei_payload( payloadType, payloadSize ) 5  
}  

access_unit_delimiter_rbsp() { 
 primary_pic_type 6 u(3) 
 rbsp_trailing_bits() 6  
}   

end_of_seq_rbsp() { 

}  

end_of_stream_rbsp() { 
}   

filler_data_rbsp() { 
  while( next_bits( 8 )  ==  0xFF )   
    ff_byte  /* equal to 0xFF */ 9 f(8)
  rbsp_trailing_bits() 9 
}

slice_layer_without_partitioning_rbsp() { 
  slice_header() 2 
  slice_data()  /* all categories of slice_data() syntax */ 2 | 3 | 4
  rbsp_slice_trailing_bits() 2
}   

slice_data_partition_a_layer_rbsp() { 
  slice_header() 2
  slice_id All ue(v) 
  slice_data()  /* only category 2 parts of slice_data() syntax */ 2
  rbsp_slice_trailing_bits() 2
}

slice_data_partition_b_layer_rbsp() {
 slice_id All ue(v) 
 if( separate_colour_plane_flag  ==  1 )   
  colour_plane_id All u(2) 
 if( redundant_pic_cnt_present_flag )   
  redundant_pic_cnt All ue(v) 
 slice_data()  /* only category 3 parts of slice_data() syntax */ 3  
 rbsp_slice_trailing_bits() 3  
}  

slice_data_partition_c_layer_rbsp() { 
 slice_id All ue(v) 
 if( separate_colour_plane_flag  ==  1 )   
  colour_plane_id All u(2) 
 if( redundant_pic_cnt_present_flag )   
  redundant_pic_cnt All ue(v) 
 slice_data()  /* only category 4 parts of slice_data() syntax */ 4  
 rbsp_slice_trailing_bits() 4  
}  

rbsp_slice_trailing_bits() { 
 rbsp_trailing_bits() All  
 if( entropy_coding_mode_flag )   
  while( more_rbsp_trailing_data() )   
   cabac_zero_word  /* equal to 0x0000 */ All f(16) 
}   

rbsp_trailing_bits() { 
 rbsp_stop_one_bit  /* equal to 1 */ All f(1) 
 while( !byte_aligned() )   
  rbsp_alignment_zero_bit  /* equal to 0 */ All f(1) 
}   

prefix_nal_unit_rbsp() {
 if( svc_extension_flag )   
  prefix_nal_unit_svc()  /* specified in Annex G */ 2  
}  

slice_layer_extension_rbsp() { 
 if( svc_extension_flag ) {   
  slice_header_in_scalable_extension()  /* specified in Annex G */ 2  
  if( !slice_skip_flag )   
   slice_data_in_scalable_extension()  /* specified in Annex G */ 2 | 3 | 4  
 } else if( avc_3d_extension_flag ) {   
  slice_header_in_3davc_extension()  /* specified in Annex J */ 2  
  slice_data_in_3davc_extension()  /* specified in Annex J */ 2 | 3 | 4  
 } else {   
  slice_header() 2  
  slice_data() 2 | 3 | 4  
 }   
 rbsp_slice_trailing_bits() 2  
}  

slice_header() { 
 first_mb_in_slice 2 ue(v) 
 slice_type 2 ue(v) 
 pic_parameter_set_id 2 ue(v) 
 if( separate_colour_plane_flag  ==  1 )   
  colour_plane_id 2 u(2) 
 frame_num 2 u(v) 
 if( !frame_mbs_only_flag ) {   
  field_pic_flag 2 u(1) 
  if( field_pic_flag )   
   bottom_field_flag 2 u(1) 
 }   
 if( IdrPicFlag )   
  idr_pic_id 2 ue(v) 
 if( pic_order_cnt_type  ==  0 ) {   
  pic_order_cnt_lsb 2 u(v) 
  if( bottom_field_pic_order_in_frame_present_flag &&  !field_pic_flag )   
   delta_pic_order_cnt_bottom 2 se(v) 
 }   
 if( pic_order_cnt_type == 1 && !delta_pic_order_always_zero_flag ) {   
  delta_pic_order_cnt[ 0 ] 2 se(v)
    if( bottom_field_pic_order_in_frame_present_flag  &&  !field_pic_flag )   
   delta_pic_order_cnt[ 1 ] 2 se(v) 
 }   
 if( redundant_pic_cnt_present_flag )   
  redundant_pic_cnt 2 ue(v) 
 if( slice_type  ==  B )   
  direct_spatial_mv_pred_flag 2 u(1) 
 if( slice_type  ==  P  ||  slice_type  ==  SP  ||  slice_type  ==  B ) {   
  num_ref_idx_active_override_flag 2 u(1) 
  if( num_ref_idx_active_override_flag ) {   
   num_ref_idx_l0_active_minus1 2 ue(v) 
   if( slice_type  ==  B )   
    num_ref_idx_l1_active_minus1 2 ue(v) 
  }   
 }   
 if( nal_unit_type  ==  20  ||  nal_unit_type  ==  21 )   
  ref_pic_list_mvc_modification()  /* specified in Annex H */ 2  
 else   
  ref_pic_list_modification() 2  
 if( ( weighted_pred_flag  &&  ( slice_type  ==  P  ||  slice_type  ==  SP ) )  || 
  ( weighted_bipred_idc  ==  1  &&  slice_type  ==  B ) ) 
  
  pred_weight_table() 2  
 if( nal_ref_idc != 0 )   
  dec_ref_pic_marking() 2  
 if( entropy_coding_mode_flag  &&  slice_type  !=  I  &&  slice_type  !=  SI )   
  cabac_init_idc 2 ue(v) 
 slice_qp_delta 2 se(v) 
 if( slice_type  ==  SP  ||  slice_type  ==  SI ) {   
  if( slice_type  ==  SP )   
   sp_for_switch_flag 2 u(1) 
  slice_qs_delta 2 se(v) 
 }   
 if( deblocking_filter_control_present_flag ) {   
  disable_deblocking_filter_idc 2 ue(v) 
  if( disable_deblocking_filter_idc  !=  1 ) {   
   slice_alpha_c0_offset_div2 2 se(v) 
   slice_beta_offset_div2 2 se(v) 
  }   
 }   
 if( num_slice_groups_minus1 > 0  && 
  slice_group_map_type >= 3  &&  slice_group_map_type <= 5) 
  
  slice_group_change_cycle 2 u(v) 
}   

slice_data() { 
 if( entropy_coding_mode_flag )   
  while( !byte_aligned() )   
   cabac_alignment_one_bit 2 f(1) 
 CurrMbAddr = first_mb_in_slice * ( 1 + MbaffFrameFlag )   
 moreDataFlag = 1   
 prevMbSkipped = 0   
 do {   
  if( slice_type  !=  I  &&  slice_type  !=  SI )   
   if( !entropy_coding_mode_flag ) {   
    mb_skip_run 2 ue(v) 
    prevMbSkipped = ( mb_skip_run > 0 )   
    for( i=0; i<mb_skip_run; i++ )   
     CurrMbAddr = NextMbAddress( CurrMbAddr )   
    if( mb_skip_run > 0 )   
     moreDataFlag = more_rbsp_data()   
   } else {   
    mb_skip_flag 2 ae(v) 
    moreDataFlag = !mb_skip_flag   
   }   
  if( moreDataFlag ) {   
   if( MbaffFrameFlag && ( CurrMbAddr % 2  ==  0  || 
    ( CurrMbAddr % 2  ==  1  &&  prevMbSkipped ) ) ) 
  
    mb_field_decoding_flag 2 u(1) | ae(v) 
   macroblock_layer() 2 | 3 | 4  
  }   
  if( !entropy_coding_mode_flag )   
   moreDataFlag = more_rbsp_data()   
  else {   
   if( slice_type  !=  I  &&  slice_type  !=  SI )   
    prevMbSkipped = mb_skip_flag   
   if( MbaffFrameFlag  &&  CurrMbAddr % 2  ==  0 )   
    moreDataFlag = 1   
   else {   
    end_of_slice_flag 2 ae(v) 
    moreDataFlag = !end_of_slice_flag   
   }   
  }   
  CurrMbAddr = NextMbAddress( CurrMbAddr )   
 } while( moreDataFlag )   
} 

ref_pic_list_modification() { 
 if( slice_type % 5  !=  2  &&  slice_type % 5  !=  4 ) {    
  ref_pic_list_modification_flag_l0 2 u(1) 
  if( ref_pic_list_modification_flag_l0 )   
   do {   
    modification_of_pic_nums_idc 2 ue(v) 
    if( modification_of_pic_nums_idc  ==  0  || 
     modification_of_pic_nums_idc  ==  1 ) 
  
     abs_diff_pic_num_minus1 2 ue(v) 
    else if( modification_of_pic_nums_idc  ==  2 )   
     long_term_pic_num 2 ue(v) 
   } while( modification_of_pic_nums_idc  !=  3 )   
 }   
 if( slice_type % 5  ==  1 ) {    
  ref_pic_list_modification_flag_l1 2 u(1) 
  if( ref_pic_list_modification_flag_l1 )   
   do {   
    modification_of_pic_nums_idc 2 ue(v) 
    if( modification_of_pic_nums_idc  ==  0  || 
     modification_of_pic_nums_idc  ==  1 ) 
  
     abs_diff_pic_num_minus1 2 ue(v) 
    else if( modification_of_pic_nums_idc  ==  2 )   
     long_term_pic_num 2 ue(v) 
   } while( modification_of_pic_nums_idc  !=  3 )   
 }   
}  

pred_weight_table() { 
 luma_log2_weight_denom 2 ue(v) 
 if( ChromaArrayType  !=  0 )   
  chroma_log2_weight_denom 2 ue(v) 
 for( i = 0; i <= num_ref_idx_l0_active_minus1; i++ ) {   
  luma_weight_l0_flag 2 u(1) 
  if( luma_weight_l0_flag ) {   
   luma_weight_l0[ i ] 2 se(v) 
   luma_offset_l0[ i ] 2 se(v) 
  }   
  if( ChromaArrayType  !=  0 ) {   
   chroma_weight_l0_flag 2 u(1) 
   if( chroma_weight_l0_flag )   
    for( j =0; j < 2; j++ ) {   
     chroma_weight_l0[ i ][ j ] 2 se(v) 
     chroma_offset_l0[ i ][ j ] 2 se(v) 
    }   
  }   
 }   
 if( slice_type % 5  ==  1 )    
  for( i = 0; i <= num_ref_idx_l1_active_minus1; i++ ) {   
   luma_weight_l1_flag 2 u(1) 
   if( luma_weight_l1_flag ) {   
    luma_weight_l1[ i ] 2 se(v) 
    luma_offset_l1[ i ] 2 se(v) 
   }   
   if( ChromaArrayType  !=  0 ) {   
    chroma_weight_l1_flag 2 u(1) 
    if( chroma_weight_l1_flag )   
     for( j = 0; j < 2; j++ ) {   
      chroma_weight_l1[ i ][ j ] 2 se(v) 
      chroma_offset_l1[ i ][ j ] 2 se(v) 
     }   
   }   
  }   
}  

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




macroblock_layer() { 
 mb_type 2 ue(v) | ae(v) 
 if( mb_type  ==  I_PCM ) {   
  while( !byte_aligned() )   
   pcm_alignment_zero_bit 3 f(1) 
  for( i = 0; i < 256; i++ )   
   pcm_sample_luma[ i ] 3 u(v) 
  for( i = 0; i < 2 * MbWidthC * MbHeightC; i++ )   
   pcm_sample_chroma[ i ] 3 u(v) 
 } else {   
  noSubMbPartSizeLessThan8x8Flag = 1   
  if( mb_type  !=  I_NxN  && 
   MbPartPredMode( mb_type, 0 )  !=  Intra_16x16  && 
   NumMbPart( mb_type )  ==  4 ) { 
  
   sub_mb_pred( mb_type ) 2  
   for( mbPartIdx = 0; mbPartIdx < 4; mbPartIdx++ )   
    if( sub_mb_type[ mbPartIdx ]  !=  B_Direct_8x8 ) {   
     if( NumSubMbPart( sub_mb_type[ mbPartIdx ] )  >  1 )   
      noSubMbPartSizeLessThan8x8Flag = 0   
    } else if( !direct_8x8_inference_flag )   
     noSubMbPartSizeLessThan8x8Flag = 0   
  } else {   
   if( transform_8x8_mode_flag  &&  mb_type  ==  I_NxN )   
    transform_size_8x8_flag 2 u(1) | ae(v) 
   mb_pred( mb_type ) 2  
  }   
  if( MbPartPredMode( mb_type, 0 )  !=  Intra_16x16 ) {   
   coded_block_pattern 2 me(v) | ae(v) 
   if( CodedBlockPatternLuma > 0  && 
     transform_8x8_mode_flag  &&  mb_type  !=  I_NxN  && 
     noSubMbPartSizeLessThan8x8Flag  && 
     ( mb_type  !=  B_Direct_16x16  ||  direct_8x8_inference_flag ) ) 
  
    transform_size_8x8_flag 2 u(1) | ae(v) 
  }   
  if( CodedBlockPatternLuma > 0  ||  CodedBlockPatternChroma > 0  || 
   MbPartPredMode( mb_type, 0 )  ==  Intra_16x16 ) { 
  
   mb_qp_delta 2 se(v) | ae(v) 
   residual( 0, 15 ) 3 | 4  
  }   
 }   
}  

mb_pred( mb_type ) {
 if( MbPartPredMode( mb_type, 0 )  ==  Intra_4x4  ||   
  MbPartPredMode( mb_type, 0 )  ==  Intra_8x8  ||   
  MbPartPredMode( mb_type, 0 )  ==  Intra_16x16 ) { 
  
  if( MbPartPredMode( mb_type, 0 )  ==  Intra_4x4 )   
   for( luma4x4BlkIdx=0; luma4x4BlkIdx<16; luma4x4BlkIdx++ ) {   
    prev_intra4x4_pred_mode_flag[ luma4x4BlkIdx ] 2 u(1) | ae(v) 
    if( !prev_intra4x4_pred_mode_flag[ luma4x4BlkIdx ] )   
     rem_intra4x4_pred_mode[ luma4x4BlkIdx ] 2 u(3) | ae(v) 
   }   
  if( MbPartPredMode( mb_type, 0 )  ==  Intra_8x8 )   
   for( luma8x8BlkIdx=0; luma8x8BlkIdx<4; luma8x8BlkIdx++ ) {   
    prev_intra8x8_pred_mode_flag[ luma8x8BlkIdx ] 2 u(1) | ae(v) 
    if( !prev_intra8x8_pred_mode_flag[ luma8x8BlkIdx ] )   
     rem_intra8x8_pred_mode[ luma8x8BlkIdx ] 2 u(3) | ae(v) 
   }   
  if( ChromaArrayType  ==  1  ||  ChromaArrayType  ==  2 )   
   intra_chroma_pred_mode 2 ue(v) | ae(v) 
 } else if( MbPartPredMode( mb_type, 0 )  !=  Direct ) {   
  for( mbPartIdx = 0; mbPartIdx < NumMbPart( mb_type ); mbPartIdx++ )   
   if( ( num_ref_idx_l0_active_minus1 > 0  || 
     mb_field_decoding_flag  !=  field_pic_flag ) &&   
    MbPartPredMode( mb_type, mbPartIdx )  !=  Pred_L1 ) 
  
    ref_idx_l0[ mbPartIdx ] 2 te(v) | ae(v) 
  for( mbPartIdx = 0; mbPartIdx < NumMbPart( mb_type ); mbPartIdx++ )   
   if( ( num_ref_idx_l1_active_minus1  >  0  || 
     mb_field_decoding_flag  !=  field_pic_flag ) &&   
    MbPartPredMode( mb_type, mbPartIdx )  !=  Pred_L0 ) 
  
    ref_idx_l1[ mbPartIdx ] 2 te(v) | ae(v) 
  for( mbPartIdx = 0; mbPartIdx < NumMbPart( mb_type ); mbPartIdx++ )   
   if( MbPartPredMode ( mb_type, mbPartIdx )  !=  Pred_L1 )   
    for( compIdx = 0; compIdx < 2; compIdx++ )   
     mvd_l0[ mbPartIdx ][ 0 ][ compIdx ] 2 se(v) | ae(v) 
  for( mbPartIdx = 0; mbPartIdx < NumMbPart( mb_type ); mbPartIdx++ )   
   if( MbPartPredMode( mb_type, mbPartIdx )  !=  Pred_L0 )   
    for( compIdx = 0; compIdx < 2; compIdx++ )   
     mvd_l1[ mbPartIdx ][ 0 ][ compIdx ] 2 se(v) | ae(v) 
 }   
}   

sub_mb_pred( mb_type ) { 
 for( mbPartIdx = 0; mbPartIdx < 4; mbPartIdx++ )   
  sub_mb_type[ mbPartIdx ] 2 ue(v) | ae(v) 
 for( mbPartIdx = 0; mbPartIdx < 4; mbPartIdx++ )   
  if( ( num_ref_idx_l0_active_minus1  >  0  ||   
    mb_field_decoding_flag  !=  field_pic_flag ) && 
   mb_type  !=  P_8x8ref0  && 
   sub_mb_type[ mbPartIdx ]  !=  B_Direct_8x8  && 
   SubMbPredMode( sub_mb_type[ mbPartIdx ] )  !=  Pred_L1 ) 
  
   ref_idx_l0[ mbPartIdx ] 2 te(v) | ae(v) 
 for( mbPartIdx = 0; mbPartIdx < 4; mbPartIdx++ )   
  if( ( num_ref_idx_l1_active_minus1  >  0  ||   
    mb_field_decoding_flag  !=  field_pic_flag ) && 
     sub_mb_type[ mbPartIdx ]  !=  B_Direct_8x8  && 
     SubMbPredMode( sub_mb_type[ mbPartIdx ] )  !=  Pred_L0 ) 
  
   ref_idx_l1[ mbPartIdx ] 2 te(v) | ae(v) 
 for( mbPartIdx = 0; mbPartIdx < 4; mbPartIdx++ )   
  if( sub_mb_type[ mbPartIdx ]  !=  B_Direct_8x8  && 
   SubMbPredMode( sub_mb_type[ mbPartIdx ] )  !=  Pred_L1 ) 
  
   for( subMbPartIdx = 0;  
       subMbPartIdx < NumSubMbPart( sub_mb_type[ mbPartIdx ] ); 
       subMbPartIdx++ ) 
  
    for( compIdx = 0; compIdx < 2; compIdx++ )   
     mvd_l0[ mbPartIdx ][ subMbPartIdx ][ compIdx ] 2 se(v) | ae(v) 
 for( mbPartIdx = 0; mbPartIdx < 4; mbPartIdx++ )   
  if( sub_mb_type[ mbPartIdx ]  !=  B_Direct_8x8  && 
   SubMbPredMode( sub_mb_type[ mbPartIdx ] )  !=  Pred_L0 ) 
  
   for( subMbPartIdx = 0;  
       subMbPartIdx < NumSubMbPart( sub_mb_type[ mbPartIdx ] ); 
       subMbPartIdx++ ) 
  
    for( compIdx = 0; compIdx < 2; compIdx++ )   
     mvd_l1[ mbPartIdx ][ subMbPartIdx ][ compIdx ] 2 se(v) | ae(v) 
}   

residual( startIdx, endIdx ) {
 if( !entropy_coding_mode_flag )   
  residual_block = residual_block_cavlc   
 else   
  residual_block = residual_block_cabac   
 residual_luma( i16x16DClevel, i16x16AClevel, level4x4, level8x8, startIdx, endIdx ) 3 | 4  
 Intra16x16DCLevel = i16x16DClevel   
 Intra16x16ACLevel = i16x16AClevel   
 LumaLevel4x4 = level4x4   
 LumaLevel8x8 = level8x8   
 if( ChromaArrayType  ==  1  || ChromaArrayType  ==  2 ) {   
  NumC8x8 = 4 / ( SubWidthC * SubHeightC )   
  for( iCbCr = 0; iCbCr < 2; iCbCr++ )   
   if( ( CodedBlockPatternChroma & 3 )  &&  startIdx  ==  0 )   
    residual_block(ChromaDCLevel[iCbCr], 0, 4 * NumC8x8 - 1, 4 * NumC8x8) /* chroma DC residual present */  3 | 4  
   else   
    for( i = 0; i < 4 * NumC8x8; i++ )   
     ChromaDCLevel[ iCbCr ][ i ] = 0   
  for( iCbCr = 0; iCbCr < 2; iCbCr++ )   
   for( i8x8 = 0; i8x8 < NumC8x8; i8x8++ )   
    for( i4x4 = 0; i4x4 < 4; i4x4++ )   
     if( CodedBlockPatternChroma & 2 )   
       residual_block(ChromaACLevel[iCbCr][i8x8 * 4 + i4x4], Max(0, startIdx - 1), endIdx - 1, 15) /* chroma AC residual present */  3 | 4  
     else   
      for( i = 0; i < 15; i++ )   
       ChromaACLevel[ iCbCr ][ i8x8 * 4 + i4x4 ][ i ] = 0   
 } else if( ChromaArrayType  ==  3 ) {   
  residual_luma( i16x16DClevel, i16x16AClevel, level4x4, level8x8, startIdx, endIdx ) 3 | 4  
  CbIntra16x16DCLevel = i16x16DClevel   
  CbIntra16x16ACLevel = i16x16AClevel   
  CbLevel4x4 = level4x4   
  CbLevel8x8 = level8x8   
  residual_luma( i16x16DClevel, i16x16AClevel, level4x4, level8x8, startIdx, endIdx ) 3 | 4  
  CrIntra16x16DCLevel = i16x16DClevel   
  CrIntra16x16ACLevel = i16x16AClevel   
  CrLevel4x4 = level4x4   
  CrLevel8x8 = level8x8   
  }  
}

residual_luma( i16x16DClevel, i16x16AClevel, level4x4, level8x8, startIdx, endIdx ) { 
 if( startIdx  ==  0  &&  MbPartPredMode( mb_type, 0 )  ==  Intra_16x16 )   
  residual_block( i16x16DClevel, 0, 15, 16 ) 3  
 for( i8x8 = 0; i8x8 < 4; i8x8++ )   
  if( !transform_size_8x8_flag  ||  !entropy_coding_mode_flag )   
   for( i4x4 = 0; i4x4 < 4; i4x4++ ) {   
    if( CodedBlockPatternLuma & ( 1 << i8x8 ) )   
     if( MbPartPredMode( mb_type, 0 )  ==  Intra_16x16 )   
      residual_block( i16x16AClevel[ i8x8 * 4 + i4x4 ], Max( 0, startIdx - 1 ), endIdx - 1, 15 ) 3  
     else   
      residual_block( level4x4[ i8x8 * 4 + i4x4 ], startIdx, endIdx, 16 ) 3 | 4  
    else if( MbPartPredMode( mb_type, 0 )  ==  Intra_16x16 )   
     for( i = 0; i < 15; i++ )   
      i16x16AClevel[ i8x8 * 4 + i4x4 ][ i ] = 0   
    else   
     for( i = 0; i < 16; i++ )   
      level4x4[ i8x8 * 4 + i4x4 ][ i ] = 0   
    if( !entropy_coding_mode_flag && transform_size_8x8_flag )   
     for( i = 0; i < 16; i++ )   
      level8x8[ i8x8 ][ 4 * i + i4x4 ] = level4x4[ i8x8 * 4 + i4x4 ][ i ]   
   }   
  else if( CodedBlockPatternLuma & ( 1 << i8x8 ) )   
   residual_block( level8x8[ i8x8 ], 4 * startIdx, 4 * endIdx + 3, 64 ) 3 | 4  
  else   
   for( i = 0; i < 64; i++ )   
    level8x8[ i8x8 ][ i ] = 0   
}  

residual_block_cavlc( coeffLevel, startIdx, endIdx, maxNumCoeff ) { 
 for( i = 0; i < maxNumCoeff; i++ )   
  coeffLevel[ i ] = 0   
 coeff_token 3 | 4 ce(v) 
 if( TotalCoeff( coeff_token ) > 0 ) {   
  if( TotalCoeff( coeff_token ) > 10  &&  TrailingOnes( coeff_token ) < 3 )   
   suffixLength = 1   
  else   
   suffixLength = 0   
  for( i = 0; i < TotalCoeff( coeff_token ); i++ )   
   if( i < TrailingOnes( coeff_token ) ) {   
    trailing_ones_sign_flag 3 | 4 u(1) 
    levelVal[ i ] = 1 - 2 * trailing_ones_sign_flag   
   } else {  
  level_prefix 3 | 4 ce(v)
                levelCode = (Min(15, level_prefix) << suffixLength)
                if (suffixLength > 0 || level_prefix >= 14) {   
     level_suffix 3 | 4 u(v)
                    levelCode += level_suffix
                }
                if (level_prefix >= 15 && suffixLength  ==  0 )
                levelCode += 15
                if (level_prefix >= 16 )
                levelCode += (1 << (level_prefix - 3 ) ) - 4096
                if (i == TrailingOnes(coeff_token) &&
                    TrailingOnes(coeff_token) < 3)

                    levelCode += 2
                if (levelCode % 2  == 0 )
                levelVal[i] = (levelCode + 2) >> 1   
    else
                levelVal[i] = ( -levelCode - 1 ) >> 1
                if (suffixLength == 0)
                    suffixLength = 1
                if (Abs(levelVal[i]) > (3 << (suffixLength - 1 ) )  &&
                    suffixLength < 6 )

                suffixLength++
            }
        if (TotalCoeff(coeff_token) < endIdx - startIdx + 1 ) {   
   total_zeros 3 | 4 ce(v)
            zerosLeft = total_zeros
        } else
        zerosLeft = 0
        for (i = 0; i < TotalCoeff(coeff_token) - 1; i++ ) {
            if (zerosLeft > 0) {   
    run_before 3 | 4 ce(v)
                runVal[i] = run_before
            } else
                runVal[i] = 0
            zerosLeft = zerosLeft - runVal[i]
        }
        runVal[TotalCoeff(coeff_token) - 1 ] = zerosLeft
        coeffNum = -1
        for (i = TotalCoeff(coeff_token) - 1; i >= 0; i-- ) {
            coeffNum += runVal[i] + 1
            coeffLevel[startIdx + coeffNum] = levelVal[i]
        }
    }
}

residual_block_cabac( coeffLevel, startIdx, endIdx, maxNumCoeff ) { 
 if( maxNumCoeff  !=  64  ||  ChromaArrayType  ==  3 )   
  coded_block_flag 3 | 4 ae(v) 
 for( i = 0; i < maxNumCoeff; i++ )   
  coeffLevel[ i ] = 0   
 if( coded_block_flag ) {   
  numCoeff = endIdx + 1   
  i = startIdx   
  while( i < numCoeff - 1 )  {   
   significant_coeff_flag[ i ] 3 | 4 ae(v) 
   if( significant_coeff_flag[ i ] ) {   
    last_significant_coeff_flag[ i ] 3 | 4 ae(v) 
    if( last_significant_coeff_flag[ i ] )    
     numCoeff = i + 1   
   }    
   i++   
  }   
  coeff_abs_level_minus1[ numCoeff - 1 ] 3 | 4 ae(v) 
  coeff_sign_flag[ numCoeff - 1 ] 3 | 4 ae(v) 
  coeffLevel[ numCoeff - 1 ] = ( coeff_abs_level_minus1[ numCoeff - 1 ] + 1 ) * ( 1 - 2 * coeff_sign_flag[ numCoeff - 1 ] ) 
  for( i = numCoeff - 2; i >= startIdx; i-- )   
   if( significant_coeff_flag[ i ] ) {   
    coeff_abs_level_minus1[ i ] 3 | 4 ae(v) 
    coeff_sign_flag[ i ] 3 | 4 ae(v) 
    coeffLevel[ i ] = ( coeff_abs_level_minus1[ i ] + 1 ) * ( 1 - 2 * coeff_sign_flag[ i ] ) 
   }   
 }   
}   

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
scalability_info( payloadSize )  /* specified in Annex G */ 5
else if( payloadType  ==  25 )   
sub_pic_scalable_layer( payloadSize )  /* specified in Annex G */ 5
else if( payloadType  ==  26 )   
non_required_layer_rep( payloadSize )  /* specified in Annex G */ 5
else if( payloadType  ==  27 )   
priority_layer_info( payloadSize )  /* specified in Annex G */ 5
else if( payloadType  ==  28 )   
layers_not_present( payloadSize )  /* specified in Annex G */ 5
else if( payloadType  ==  29 )   
layer_dependency_change( payloadSize )  /* specified in Annex G */ 5
else if( payloadType  ==  30 )   
scalable_nesting( payloadSize )  /* specified in Annex G */ 5
else if( payloadType  ==  31 )   
base_layer_temporal_hrd( payloadSize )  /* specified in Annex G */ 5
else if( payloadType  ==  32 )   
quality_layer_integrity_check( payloadSize )  /* specified in Annex G */ 5
else if( payloadType  ==  33 )   
redundant_pic_property( payloadSize )  /* specified in Annex G */ 5
else if( payloadType  ==  34 )   
tl0_dep_rep_index( payloadSize )  /* specified in Annex G */ 5
else if( payloadType  ==  35 )   
tl_switching_point( payloadSize )  /* specified in Annex G */ 5
else if( payloadType  ==  36 )   
parallel_decoding_info( payloadSize )  /* specified in Annex H */ 5
else if( payloadType  ==  37 )   
 mvc_scalable_nesting( payloadSize )  /* specified in Annex H */ 5  
else if( payloadType  ==  38 )   
 view_scalability_info( payloadSize )  /* specified in Annex H */ 5  
else if( payloadType  ==  39 )   
 multiview_scene_info( payloadSize )  /* specified in Annex H */ 5  
else if( payloadType  ==  40 )   
 multiview_acquisition_info( payloadSize )  /* specified in Annex H */ 5  
else if( payloadType  ==  41 )   
 non_required_view_component( payloadSize )  /* specified in Annex H */ 5  
else if( payloadType  ==  42 )   
 view_dependency_change( payloadSize )  /* specified in Annex H */ 5  
else if( payloadType  ==  43 )   
 operation_point_not_present( payloadSize )  /* specified in Annex H */ 5  
else if( payloadType  ==  44 )   
 base_view_temporal_hrd( payloadSize )  /* specified in Annex H */ 5  
else if( payloadType  ==  45 )   
 frame_packing_arrangement( payloadSize ) 5  
else if( payloadType  ==  46 )   
 multiview_view_position( payloadSize )  /* specified in Annex H */ 5  
else if( payloadType  ==  47 )   
 display_orientation( payloadSize ) 5  
else if( payloadType  ==  48 )   
 mvcd_scalable_nesting( payloadSize )  /* specified in Annex I */ 5  
else if( payloadType  ==  49 )   
 mvcd_view_scalability_info( payloadSize )  /* specified in Annex I */ 5  
else if( payloadType  ==  50 )   
 depth_representation_info( payloadSize )  /* specified in Annex I */ 5  
else if( payloadType  ==  51 )   
 three_dimensional_reference_displays_info( payloadSize )  /* specified in Annex I */ 5
else if( payloadType  ==  52 )   
 depth_timing( payloadSize )  /* specified in Annex I */ 5  
else if( payloadType  ==  53 )   
 depth_sampling_info( payloadSize )  /* specified in Annex I */ 5  
else if( payloadType  ==  54 )   
 constrained_depth_parameter_set_identifier( payloadSize ) /* specified in Annex J */ 5  
else if( payloadType  ==  56 )   
 green_metadata( payloadSize )  /* specified in ISO/IEC 23001-11 */ 5  
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
alternative_depth_info( payloadSize )  /* specified in Annex I */ 5
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
bit_equal_to_one  /* equal to 1 */ 5 f(1) 
while( !byte_aligned() ) 
bit_equal_to_zero  /* equal to 0 */ 5 f(1)
}   
}  

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
   seconds_value /* 0..59 */ 5 u(6) 
   minutes_value /* 0..59 */ 5 u(6) 
   hours_value /* 0..23 */ 5 u(5) 
   } else {   
   seconds_flag 5 u(1) 
   if( seconds_flag ) {   
    seconds_value /* range 0..59 */ 5 u(6) 
      minutes_flag 5 u(1) 
      if( minutes_flag ) {   
       minutes_value /* 0..59 */ 5 u(6) 
       hours_flag 5 u(1) 
       if( hours_flag )   
        hours_value /* 0..23 */ 5 u(5) 
      }   
     }   
    }   
    if( time_offset_length > 0 )   
     time_offset 5 i(v) 
   }   
  }   
 }   
}   

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

filler_payload( payloadSize ) {  
 for( k = 0; k < payloadSize; k++ )   
  ff_byte  /* equal to 0xFF */ 5 f(8) 
}  

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

user_data_unregistered( payloadSize ) {  
 uuid_iso_iec_11578 5 u(128) 
 for( i = 16; i < payloadSize; i++ )   
  user_data_payload_byte 5 b(8) 
}   

recovery_point( payloadSize ) {  
 recovery_frame_cnt 5 ue(v) 
 exact_match_flag 5 u(1) 
 broken_link_flag 5 u(1) 
 changing_slice_group_idc 5 u(2) 
}  

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

scene_info( payloadSize ) {  
 scene_info_present_flag 5 u(1) 
 if( scene_info_present_flag ) {   
  scene_id 5 ue(v) 
  scene_transition_type 5 ue(v) 
  if( scene_transition_type > 3 )   
   second_scene_id 5 ue(v) 
 }   
}   

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

sub_seq_layer_characteristics( payloadSize ) {  
 num_sub_seq_layers_minus1 5 ue(v) 
 for( layer = 0; layer <= num_sub_seq_layers_minus1; layer++ ) {   
  accurate_statistics_flag 5 u(1) 
  average_bit_rate 5 u(16) 
  average_frame_rate 5 u(16) 
 }   
}   

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

full_frame_freeze( payloadSize ) { 
  full_frame_freeze_repetition_period 5 ue(v)
}   

full_frame_freeze_release( payloadSize ) { 
}  

full_frame_snapshot( payloadSize ) { 
  snapshot_id 5 ue(v)
}   

progressive_refinement_segment_start( payloadSize ) { 
  progressive_refinement_id 5 ue(v)
  num_refinement_steps_minus1 5 ue(v) 
}   

progressive_refinement_segment_end( payloadSize ) { 
  progressive_refinement_id 5 ue(v)
}   

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

deblocking_filter_display_preference( payloadSize ) {  
 deblocking_display_preference_cancel_flag 5 u(1) 
 if( !deblocking_display_preference_cancel_flag ) {   
  display_prior_to_deblocking_preferred_flag 5 u(1) 
  dec_frame_buffering_constraint_flag 5 u(1) 
  deblocking_display_preference_repetition_period 5 ue(v) 
 }   
}   

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

content_light_level_info( payloadSize ) {  
 max_content_light_level 5 u(16) 
 max_pic_average_light_level 5 u(16) 
}   

alternative_transfer_characteristics( payloadSize ) {  
 preferred_transfer_characteristics 5 u(8) 
}   

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

ambient_viewing_environment( payloadSize ) {  
 ambient_illuminance 5 u(32) 
 ambient_light_x 5 u(16) 
 ambient_light_y 5 u(16) 
}   

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

cubemap_projection( payloadSize ) {  
 cmp_cancel_flag 5 u(1) 
 if( !cmp_cancel_flag )   
  cmp_persistence_flag 5 u(1) 
}   

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

sei_manifest( payloadSize ) {  
 manifest_num_sei_msg_types 5 u(16) 
 for( i = 0; i < manifest_num_sei_msg_types; i++ ) {   
  manifest_sei_payload_type[ i ] 5 u(16) 
  manifest_sei_description[ i ] 5 u(8) 
 }   
}   

sei_prefix_indication( payloadSize ) {  
 prefix_sei_payload_type 5 u(16) 
 num_sei_prefix_indications_minus1 5 u(8) 
 for( i = 0; i  <=  num_sei_prefix_indications_minus1; i++ ) {   
  num_bits_in_prefix_indication_minus1[ i ] 5 u(16) 
  for( j = 0; j  <=  num_bits_in_prefix_indication_minus1[ i ]; j++ )   
   sei_prefix_data_bit[ i ][ j ] 5 u(1) 
  while( !byte_aligned() )   
   byte_alignment_bit_equal_to_one /* equal to 1 */ 5 f(1) 
 }   
}   

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
     ar_bit_equal_to_zero /* equal to 0 */ 5 f(1) 
    ar_object_label_language 5 st(v) 
   }   
   ar_num_label_updates 5 ue(v) 
   for( i = 0; i < ar_num_label_updates; i++ ) {   
    ar_label_idx[ i ] 5 ue(v) 
    ar_label_cancel_flag 5 u(1) 
    LabelAssigned[ ar_label_idx[ i ] ] = !ar_label_cancel_flag   
    if( !ar_label_cancel_flag ) {   
     while( !byte_aligned() )   
      ar_bit_equal_to_zero /* equal to 0 */ 5 f(1) 
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

reserved_sei_message( payloadSize ) {  
 for( i = 0; i < payloadSize; i++ )   
  reserved_sei_message_payload_byte 5 b(8) 
}   


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

prefix_nal_unit_svc() {  
 if( nal_ref_idc  !=  0 ) {   
  store_ref_base_pic_flag 2 u(1) 
  if( ( use_ref_base_pic_flag  ||  store_ref_base_pic_flag )  && 
       !idr_flag ) 
  
   dec_ref_base_pic_marking() 2  
  additional_prefix_nal_unit_extension_flag 2 u(1) 
  if( additional_prefix_nal_unit_extension_flag  ==  1 )   
   while( more_rbsp_data() )   
    additional_prefix_nal_unit_extension_data_flag 2 u(1) 
  rbsp_trailing_bits() 2  
 } else if( more_rbsp_data() ) {   
  while( more_rbsp_data() )   
   additional_prefix_nal_unit_extension_data_flag 2 u(1) 
  rbsp_trailing_bits() 2  
 }   
}   

slice_header_in_scalable_extension() {  
 first_mb_in_slice 2 ue(v) 
 slice_type 2 ue(v) 
 pic_parameter_set_id 2 ue(v) 
 if( separate_colour_plane_flag  ==  1 )   
  colour_plane_id 2 u(2) 
 frame_num 2 u(v) 
 if( !frame_mbs_only_flag ) {   
  field_pic_flag 2 u(1) 
  if( field_pic_flag )   
   bottom_field_flag 2 u(1) 
 }   
 if( idr_flag  ==  1 )   
  idr_pic_id 2 ue(v) 
 if( pic_order_cnt_type  ==  0 ) {   
  pic_order_cnt_lsb 2 u(v) 
  if( bottom_field_pic_order_in_frame_present_flag  &&  !field_pic_flag )   
   delta_pic_order_cnt_bottom 2 se(v) 
 }   
 if( pic_order_cnt_type  ==  1  &&  !delta_pic_order_always_zero_flag ) {   
  delta_pic_order_cnt[ 0 ] 2 se(v) 
  if( bottom_field_pic_order_in_frame_present_flag  &&  !field_pic_flag )   
   delta_pic_order_cnt[ 1 ] 2 se(v) 
 }   
 if( redundant_pic_cnt_present_flag )   
  redundant_pic_cnt 2 ue(v) 
 if( quality_id  ==  0 ) {   
  if( slice_type  ==  EB )   
   direct_spatial_mv_pred_flag 2 u(1) 
  if( slice_type  ==  EP  ||  slice_type  ==  EB ) {   
   num_ref_idx_active_override_flag 2 u(1) 
   if( num_ref_idx_active_override_flag ) {   
    num_ref_idx_l0_active_minus1 2 ue(v) 
    if( slice_type  ==  EB )   
     num_ref_idx_l1_active_minus1 2 ue(v) 
   }   
  }   
  ref_pic_list_modification() 2  
  if( ( weighted_pred_flag  &&  slice_type == EP  )  || 
   ( weighted_bipred_idc  ==  1  &&  slice_type  ==  EB ) ) { 
  
   if( !no_inter_layer_pred_flag )   
    base_pred_weight_table_flag 2 u(1) 
   if( no_inter_layer_pred_flag  ||  !base_pred_weight_table_flag )   
    pred_weight_table() 2  
  }   
  if( nal_ref_idc  !=  0 ) {   
   dec_ref_pic_marking() 2  
   if( !slice_header_restriction_flag ) {   
    store_ref_base_pic_flag 2 u(1) 
    if( ( use_ref_base_pic_flag  ||  store_ref_base_pic_flag )  && 
          !idr_flag )  
  
     dec_ref_base_pic_marking() 2  
   }   
  }   
 }   
 if( entropy_coding_mode_flag  &&  slice_type  !=  EI )  
   cabac_init_idc 2 ue(v) 
 slice_qp_delta 2 se(v) 
 if( deblocking_filter_control_present_flag ) {   
  disable_deblocking_filter_idc 2 ue(v) 
  if( disable_deblocking_filter_idc  !=  1 ) {   
   slice_alpha_c0_offset_div2 2 se(v) 
   slice_beta_offset_div2 2 se(v) 
  }   
 }   
 if( num_slice_groups_minus1 > 0  && 
  slice_group_map_type  >=  3  &&  slice_group_map_type  <=  5 ) 
  
  slice_group_change_cycle 2 u(v) 
 if( !no_inter_layer_pred_flag  &&  quality_id  ==  0 ) {   
  ref_layer_dq_id 2 ue(v) 
  if( inter_layer_deblocking_filter_control_present_flag ) {   
   disable_inter_layer_deblocking_filter_idc 2 ue(v) 
   if( disable_inter_layer_deblocking_filter_idc  !=  1 ) {   
    inter_layer_slice_alpha_c0_offset_div2 2 se(v) 
    inter_layer_slice_beta_offset_div2 2 se(v) 
   }   
  }   
  constrained_intra_resampling_flag 2 u(1) 
  if( extended_spatial_scalability_idc  ==  2 ) {   
   if( ChromaArrayType > 0 ) {   
    ref_layer_chroma_phase_x_plus1_flag 2 u(1) 
    ref_layer_chroma_phase_y_plus1 2 u(2) 
   }   
   scaled_ref_layer_left_offset 2 se(v) 
   scaled_ref_layer_top_offset 2 se(v) 
   scaled_ref_layer_right_offset 2 se(v) 
   scaled_ref_layer_bottom_offset 2 se(v) 
  }   
 }   
 if( !no_inter_layer_pred_flag ) {   
  slice_skip_flag 2 u(1) 
  if( slice_skip_flag )   
   num_mbs_in_slice_minus1 2 ue(v) 
  else {   
   adaptive_base_mode_flag 2 u(1) 
   if( !adaptive_base_mode_flag )   
    default_base_mode_flag 2 u(1) 
   if( !default_base_mode_flag ) {   
    adaptive_motion_prediction_flag 2 u(1) 
    if( !adaptive_motion_prediction_flag )   
     default_motion_prediction_flag 2 u(1) 
   }   
   adaptive_residual_prediction_flag 2 u(1) 
   if( !adaptive_residual_prediction_flag )   
    default_residual_prediction_flag 2 u(1)
      }   
  if( adaptive_tcoeff_level_prediction_flag )   
   tcoeff_level_prediction_flag 2 u(1) 
 }   
 if( !slice_header_restriction_flag  &&  !slice_skip_flag ) {   
  scan_idx_start 2 u(4) 
  scan_idx_end 2 u(4) 
 }   
}  

dec_ref_base_pic_marking() {  
 adaptive_ref_base_pic_marking_mode_flag 2 u(1) 
 if( adaptive_ref_base_pic_marking_mode_flag )   
  do {   
   memory_management_base_control_operation 2 ue(v) 
   if( memory_management_base_control_operation  ==  1 )   
    difference_of_base_pic_nums_minus1 2 ue(v) 
   if( memory_management_base_control_operation  ==  2  )   
    long_term_base_pic_num 2 ue(v) 
  } while( memory_management_base_control_operation  !=  0 )   
}  

slice_data_in_scalable_extension() {  
 if( entropy_coding_mode_flag)   
  while( !byte_aligned() )   
   cabac_alignment_one_bit 2 f(1) 
 CurrMbAddr = first_mb_in_slice * ( 1 + MbaffFrameFlag )   
 moreDataFlag = 1   
 prevMbSkipped = 0   
 do {   
  if( slice_type  !=  EI )   
   if( !entropy_coding_mode_flag ) {   
    mb_skip_run 2 ue(v) 
    prevMbSkipped = ( mb_skip_run > 0 )   
    for( i = 0; i < mb_skip_run; i++ )   
     CurrMbAddr = NextMbAddress( CurrMbAddr )   
    if( mb_skip_run > 0 )   
     moreDataFlag = more_rbsp_data()   
   } else {   
    mb_skip_flag 2 ae(v) 
    moreDataFlag = !mb_skip_flag   
   }   
  if( moreDataFlag )  {   
   if( MbaffFrameFlag  &&  ( ( CurrMbAddr % 2 )  ==  0  || 
    ( ( CurrMbAddr % 2 )  ==  1  &&  prevMbSkipped ) ) ) 
  
    mb_field_decoding_flag 2 u(1) | ae(v) 
   macroblock_layer_in_scalable_extension() 2 | 3 | 4  
  }   
  if( !entropy_coding_mode_flag )   
   moreDataFlag = more_rbsp_data()   
  else {   
   if( slice_type  !=  EI )   
    prevMbSkipped = mb_skip_flag   
   if( MbaffFrameFlag  &&  ( CurrMbAddr % 2 )  ==  0 )   
    moreDataFlag = 1   
   else {   
    end_of_slice_flag 2 ae(v) 
    moreDataFlag = !end_of_slice_flag   
   }   
  }   
  CurrMbAddr = NextMbAddress( CurrMbAddr )   
 } while( moreDataFlag )   
}  

macroblock_layer_in_scalable_extension() {  
 if( InCropWindow( CurrMbAddr )  &&  adaptive_base_mode_flag )   
  base_mode_flag 2 u(1) | ae(v) 
 if( !base_mode_flag)   
  mb_type 2 ue(v) | ae(v) 
 if( mb_type  ==  I_PCM ) {   
  while( !byte_aligned() )   
   pcm_alignment_zero_bit 3 f(1) 
  for( i = 0; i < 256; i++ )   
   pcm_sample_luma[ i ] 3 u(v) 
  for( i = 0; i < 2 * MbWidthC * MbHeightC; i++ )   
   pcm_sample_chroma[ i ] 3 u(v) 
 } else {   
  if( !base_mode_flag )  {   
   noSubMbPartSizeLessThan8x8Flag = 1   
   if( mb_type  !=  I_NxN  && 
    MbPartPredMode( mb_type, 0 )  !=  Intra_16x16  && 
    NumMbPart( mb_type )  ==  4 ) { 
  
    sub_mb_pred_in_scalable_extension( mb_type ) 2  
    for( mbPartIdx = 0; mbPartIdx < 4; mbPartIdx++ )   
     if( sub_mb_type[ mbPartIdx ]  !=  B_Direct_8x8 ) {   
      if( NumSubMbPart( sub_mb_type[ mbPartIdx ] ) > 1 )   
       noSubMbPartSizeLessThan8x8Flag = 0   
     } else if( !direct_8x8_inference_flag )   
      noSubMbPartSizeLessThan8x8Flag = 0   
   } else {   
    if( transform_8x8_mode_flag  &&  mb_type  ==  I_NxN )   
     transform_size_8x8_flag 2 u(1) | ae(v) 
    mb_pred_in_scalable_extension( mb_type ) 2  
   }   
  }   
    if( adaptive_residual_prediction_flag  &&  slice_type  !=  EI  && 
   InCropWindow( CurrMbAddr )  && 
   ( base_mode_flag  || 
     ( MbPartPredMode( mb_type, 0 )  !=  Intra_16x16  && 
    MbPartPredMode( mb_type, 0 )  !=  Intra_8x8  && 
    MbPartPredMode( mb_type, 0 )  !=  Intra_4x4 ) ) ) 
  
   residual_prediction_flag 2 u(1) | ae(v) 
  if( scan_idx_end >= scan_idx_start ) {   
   if( base_mode_flag  || 
    MbPartPredMode( mb_type, 0 )  !=  Intra_16x16 ) { 
  
    coded_block_pattern 2 me(v) | ae(v) 
    if( CodedBlockPatternLuma > 0  && 
      transform_8x8_mode_flag  && 
     ( base_mode_flag  || 
      ( mb_type  !=  I_NxN  && 
        noSubMbPartSizeLessThan8x8Flag  && 
        ( mb_type  !=  B_Direct_16x16  || 
          direct_8x8_inference_flag ) ) ) ) 
  
     transform_size_8x8_flag 2 u(1) | ae(v) 
   }   
   if( CodedBlockPatternLuma > 0  || 
     CodedBlockPatternChroma > 0  || 
     MbPartPredMode( mb_type, 0 )  ==  Intra_16x16 )  { 
  
    mb_qp_delta 2 se(v) | ae(v) 
    residual( scan_idx_start, scan_idx_end ) 3 | 4  
   }   
  }   
 }   
}   

mb_pred_in_scalable_extension( mb_type ) {  
 if( MbPartPredMode( mb_type, 0 )  ==  Intra_4x4  ||   
  MbPartPredMode( mb_type, 0 )  ==  Intra_8x8  ||   
  MbPartPredMode( mb_type, 0 )  ==  Intra_16x16 ) { 
  
  if( MbPartPredMode( mb_type, 0 )  ==  Intra_4x4 )   
   for( luma4x4BlkIdx = 0; luma4x4BlkIdx < 16; luma4x4BlkIdx++ ) {   
    prev_intra4x4_pred_mode_flag[ luma4x4BlkIdx ] 2 u(1) | ae(v) 
    if( !prev_intra4x4_pred_mode_flag[ luma4x4BlkIdx ] )   
     rem_intra4x4_pred_mode[ luma4x4BlkIdx ] 2 u(3) | ae(v) 
   }   
  if( MbPartPredMode( mb_type, 0 )  ==  Intra_8x8 )   
   for( luma8x8BlkIdx = 0; luma8x8BlkIdx < 4; luma8x8BlkIdx++ ) {   
    prev_intra8x8_pred_mode_flag[ luma8x8BlkIdx ] 2 u(1) | ae(v) 
    if( !prev_intra8x8_pred_mode_flag[ luma8x8BlkIdx ] )   
     rem_intra8x8_pred_mode[ luma8x8BlkIdx ] 2 u(3) | ae(v) 
   }   
  if( ChromaArrayType  !=  0 )   
   intra_chroma_pred_mode 2 ue(v) | ae(v) 
 } else if( MbPartPredMode( mb_type, 0 )  !=  Direct ) {   
  if( InCropWindow( CurrMbAddr )  && 
              adaptive_motion_prediction_flag ) { 
    for( mbPartIdx = 0; mbPartIdx < NumMbPart( mb_type ); mbPartIdx++ )   
    if( MbPartPredMode( mb_type, mbPartIdx )  !=  Pred_L1 )   
     motion_prediction_flag_l0[ mbPartIdx ] 2 u(1) | ae(v) 
   for( mbPartIdx = 0; mbPartIdx < NumMbPart( mb_type ); mbPartIdx++ )   
    if( MbPartPredMode( mb_type, mbPartIdx )  !=  Pred_L0 )   
     motion_prediction_flag_l1[ mbPartIdx ] 2 u(1) | ae(v) 
  }   
  for( mbPartIdx = 0; mbPartIdx < NumMbPart( mb_type ); mbPartIdx++ )   
   if( ( num_ref_idx_l0_active_minus1 > 0  || 
       mb_field_decoding_flag  !=  field_pic_flag )  && 
    MbPartPredMode( mb_type, mbPartIdx )  !=  Pred_L1  && 
    !motion_prediction_flag_l0[ mbPartIdx ]  ) 
  
    ref_idx_l0[ mbPartIdx ] 2 te(v) | ae(v) 
  for( mbPartIdx = 0; mbPartIdx < NumMbPart( mb_type ); mbPartIdx++ )   
   if( ( num_ref_idx_l1_active_minus1 > 0  || 
       mb_field_decoding_flag  !=  field_pic_flag )  && 
    MbPartPredMode( mb_type, mbPartIdx )  !=  Pred_L0  && 
    !motion_prediction_flag_l1[ mbPartIdx ] ) 
  
    ref_idx_l1[ mbPartIdx ] 2 te(v) | ae(v) 
  for( mbPartIdx = 0; mbPartIdx < NumMbPart( mb_type ); mbPartIdx++ )   
   if( MbPartPredMode ( mb_type, mbPartIdx )  !=  Pred_L1 )   
    for( compIdx = 0; compIdx < 2; compIdx++ )   
     mvd_l0[ mbPartIdx ][ 0 ][ compIdx ] 2 se(v) | ae(v) 
  for( mbPartIdx = 0; mbPartIdx < NumMbPart( mb_type ); mbPartIdx++ )   
   if( MbPartPredMode( mb_type, mbPartIdx )  !=  Pred_L0 )   
    for( compIdx = 0; compIdx < 2; compIdx++ )   
     mvd_l1[ mbPartIdx ][ 0 ][ compIdx ] 2 se(v) | ae(v) 
 }   
}  

sub_mb_pred_in_scalable_extension( mb_type ) {  
 for( mbPartIdx = 0; mbPartIdx < 4; mbPartIdx++ )     
  sub_mb_type[ mbPartIdx ] 2 ue(v) | ae(v) 
 if( InCropWindow( CurrMbAddr )  &&  adaptive_motion_prediction_flag ) {   
  for( mbPartIdx = 0; mbPartIdx < 4; mbPartIdx++ )   
   if( SubMbPredMode( sub_mb_type[ mbPartIdx ] )  !=  Direct  && 
    SubMbPredMode( sub_mb_type[ mbPartIdx ] )  !=  Pred_L1 ) 
  
    motion_prediction_flag_l0[ mbPartIdx ] 2 u(1) | ae(v) 
  for( mbPartIdx = 0; mbPartIdx < 4; mbPartIdx++ )   
   if( SubMbPredMode( sub_mb_type[ mbPartIdx ] )  !=  Direct  && 
    SubMbPredMode( sub_mb_type[ mbPartIdx ] )  !=  Pred_L0 ) 
  
    motion_prediction_flag_l1[ mbPartIdx ] 2 u(1) | ae(v) 
 }   
 for( mbPartIdx = 0; mbPartIdx < 4; mbPartIdx++ )     
  if( ( num_ref_idx_l0_active_minus1 > 0  || 
      mb_field_decoding_flag  !=  field_pic_flag )  && 
   mb_type  !=  P_8x8ref0  && 
   sub_mb_type[ mbPartIdx ]  !=  B_Direct_8x8  && 
   SubMbPredMode( sub_mb_type[ mbPartIdx ] )  !=  Pred_L1  && 
   !motion_prediction_flag_l0[ mbPartIdx ] ) 
  
   ref_idx_l0[ mbPartIdx ] 2 te(v) | ae(v) 
 for( mbPartIdx = 0; mbPartIdx < 4; mbPartIdx++ )     
  if( ( num_ref_idx_l1_active_minus1 > 0  || 
      mb_field_decoding_flag  !=  field_pic_flag )  && 
   sub_mb_type[ mbPartIdx ]  !=  B_Direct_8x8  && 
   SubMbPredMode( sub_mb_type[ mbPartIdx ] )  !=  Pred_L0  && 
   !motion_prediction_flag_l1[ mbPartIdx ] ) 
  
   ref_idx_l1[ mbPartIdx ] 2 te(v) | ae(v) 
 for( mbPartIdx = 0; mbPartIdx < 4; mbPartIdx++ )     
  if( sub_mb_type[ mbPartIdx ]  !=  B_Direct_8x8  && 
   SubMbPredMode( sub_mb_type[ mbPartIdx ] )  !=  Pred_L1 ) 
  
   for( subMbPartIdx = 0;  
       subMbPartIdx < NumSubMbPart( sub_mb_type[ mbPartIdx ] ); 
       subMbPartIdx++ ) 
  
    for( compIdx = 0; compIdx < 2; compIdx++ )   
     mvd_l0[ mbPartIdx ][ subMbPartIdx ][ compIdx ] 2 se(v) | ae(v) 
 for( mbPartIdx = 0; mbPartIdx < 4; mbPartIdx++ )     
  if( sub_mb_type[ mbPartIdx ]  !=  B_Direct_8x8  && 
   SubMbPredMode( sub_mb_type[ mbPartIdx ] )  !=  Pred_L0 ) 
  
   for( subMbPartIdx = 0;  
       subMbPartIdx < NumSubMbPart( sub_mb_type[ mbPartIdx ] ); 
       subMbPartIdx++ ) 
  
    for( compIdx = 0; compIdx < 2; compIdx++ )   
     mvd_l1[ mbPartIdx ][ subMbPartIdx ][ compIdx ] 2 se(v) | ae(v) 
}  

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

sub_pic_scalable_layer( payloadSize ) {  
 layer_id 5 ue(v) 
}

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

priority_layer_info( payloadSize ) {  
 pr_dependency_id 5 u(3) 
 num_priority_ids 5 u(4) 
 for( i = 0; i < num_priority_ids; i++ ) {   
  alt_priority_id[ i ] 5 u(6) 
 }   
}   

layers_not_present( payloadSize ) {  
 num_layers 5 ue(v) 
 for( i = 0; i < num_layers; i++ ) {   
  layer_id[ i ] 5 ue(v) 
 }   
}   

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
  sei_nesting_zero_bit /* equal to 0 */ 5 f(1) 
 do   
  sei_message() 5  
 while( more_rbsp_data() )   
}   

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

quality_layer_integrity_check( payloadSize ) {  
 num_info_entries_minus1 5 ue(v) 
 for( i = 0; i <= num_info_entries_minus1; i++ ) {   
  entry_dependency_id[ i ] 5 u(3) 
  quality_layer_crc[ i ] 5 u(16) 
 }   
}   

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

tl0_dep_rep_index( payloadSize ) {  
 tl0_dep_rep_idx 5 u(8) 
 effective_idr_pic_id 5 u(16) 
}   

tl_switching_point( payloadSize ) { 
delta_frame_num 5 se(v)
}   

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

nal_unit_header_mvc_extension() { 
non_idr_flag All u(1) 
priority_id All u(6) 
view_id All u(10) 
temporal_id All u(3) 
anchor_pic_flag All u(1) 
inter_view_flag All u(1) 
reserved_one_bit All u(1) 
}   

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

ref_pic_list_mvc_modification() {  
 if( slice_type % 5  !=  2  &&  slice_type % 5  !=  4 ) {    
  ref_pic_list_modification_flag_l0 2 u(1) 
  if( ref_pic_list_modification_flag_l0 )   
   do {   
    modification_of_pic_nums_idc 2 ue(v) 
    if( modification_of_pic_nums_idc  ==  0  || 
     modification_of_pic_nums_idc  ==  1 ) 
  
     abs_diff_pic_num_minus1 2 ue(v) 
    else if( modification_of_pic_nums_idc  ==  2 )   
     long_term_pic_num 2 ue(v) 
    else if( modification_of_pic_nums_idc  ==  4  || 
       modification_of_pic_nums_idc  ==  5 ) 
  
      abs_diff_view_idx_minus1 2 ue(v) 
   } while( modification_of_pic_nums_idc  !=  3 )   
 }   
 if( slice_type % 5  ==  1 ) {    
  ref_pic_list_modification_flag_l1 2 u(1) 
  if( ref_pic_list_modification_flag_l1 )   
   do {   
    modification_of_pic_nums_idc 2 ue(v) 
    if( modification_of_pic_nums_idc  ==  0  || 
     modification_of_pic_nums_idc  ==  1 ) 
  
     abs_diff_pic_num_minus1 2 ue(v) 
    else if( modification_of_pic_nums_idc  ==  2 )   
     long_term_pic_num 2 ue(v) 
    else if( modification_of_pic_nums_idc  ==  4  || 
       modification_of_pic_nums_idc  ==  5 ) 
  
     abs_diff_view_idx_minus1 2 ue(v) 
   } while( modification_of_pic_nums_idc  !=  3 )   
 }   
}   

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
   sei_nesting_zero_bit /* equal to 0 */ 5 f(1) 
 sei_message() 5  
}   


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

multiview_scene_info( payloadSize ) {  
 max_disparity 5 ue(v) 
}  

multiview_acquisition_info( payloadSize ) {  
 num_views_minus1  ue(v) 
 intrinsic_param_flag 5 u(1) 
 extrinsic_param_flag 5 u(1) 
if (intrinsic_param_flag ) {   
  intrinsic_params_equal_flag 5 u(1) 
  prec_focal_length 5 ue(v) 
  prec_principal_point 5 ue(v) 
  prec_skew_factor 5 ue(v) 
  for( i = 0; i <= intrinsic_params_equal_flag ? 0 : num_views_minus1; 
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
   for( j = 1; j <= 3; j++ ) { /* row */   
    for( k = 1; k <= 3; k++ ) { /* column */   
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

non_required_view_component( payloadSize ) {  
 num_info_entries_minus1 5 ue(v) 
 for( i = 0; i <= num_info_entries_minus1; i++ ) {   
  view_order_index[ i ] 5 ue(v) 
  num_non_required_view_components_minus1[ i ] 5 ue(v) 
  for( j = 0; j <= num_non_required_view_components_minus1[ i ]; j++ )    
   index_delta_minus1[ i ][ j ] 5 ue(v) 
 }   
}   

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

operation_point_not_present( payloadSize ) {  
 num_operation_points 5 ue(v) 
 for( k = 0; k < num_operation_points; k++ )    
  operation_point_not_present_id[ k ] 5 ue(v) 
}   

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

multiview_view_position( payloadSize ) { 
num_views_minus1 5 ue(v)
for( i = 0; i <= num_views_minus1; i++ )   
view_position[ i ] 5 ue(v)
multiview_view_position_extension_flag 5 u(1)
}   

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

mvcd_op_view_info() {  
 view_info_depth_view_present_flag 5 u(1) 
 if( view_info_depth_view_present_flag )   
  mvcd_depth_view_flag 5 u(1) 
 view_info_texture_view_present_flag 5 u(1) 
 if( view_info_texture_view_present_flag )   
  mvcd_texture_view_flag 5 u(1) 
}  

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
  sei_nesting_zero_bit /* equal to 0 */ 5 f(1) 
 sei_message() 5  
}   

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

depth_representation_sei_element( outSign, outExp, outMantissa, 
 outManLen ) { 
 da_sign_flag 5 u(1) 
 da_exponent 5 u(7) 
 da_mantissa_len_minus1 5 u(5) 
 da_mantissa 5 u(v) 
}   

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

depth_timing( payloadSize ) {  
 per_view_depth_timing_flag 5 u(1) 
 if( per_view_depth_timing_flag )   
  for( i = 0; i < NumDepthViews; i++ )   
   depth_timing_offset()   
 else   
  depth_timing_offset()   
}   

depth_timing_offset() {  
 offset_len_minus1 5 u(5) 
 depth_disp_delay_offset_fp 5 u(v) 
 depth_disp_delay_offset_dp 5 u(6) 
}  

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
    for( j = 0; j < 3; j++ ) /* row */   
     for( k = 0; k < 3; k++ ) { /* column */   
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

depth_grid_position() { 
depth_grid_pos_x_fp 5 u(20) 
depth_grid_pos_x_dp 5 u(4) 
depth_grid_pos_x_sign_flag 5 u(1) 
depth_grid_pos_y_fp 5 u(20) 
depth_grid_pos_y_dp 5 u(4) 
depth_grid_pos_y_sign_flag 5 u(1) 
}   

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

nal_unit_header_3davc_extension() {  
 view_idx All u(8) 
 depth_flag All u(1) 
 non_idr_flag All u(1) 
 temporal_id All u(3) 
 anchor_pic_flag All u(1) 
 inter_view_flag All u(1) 
}  

seq_parameter_set_3davc_extension() {  
 if( NumDepthViews > 0 ) {   
  3dv_acquisition_idc 0 ue(v) 
  for( i = 0; i < NumDepthViews; i++ )   
   view_id_3dv[ i ] 0 ue(v) 
  if( 3dv_acquisition_idc ) {   
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

depth_ranges( numViews, predDirection, index ) {  
 z_near_flag 11 u(1) 
 z_far_flag 11 u(1) 
 if( z_near_flag )   
  3dv_acquisition_element( numViews, 0, predDirection, 7, index, 
   ZNearSign, ZNearExp, ZNearMantissa, ZNearManLen ) 
  
 if( z_far_flag )   
  3dv_acquisition_element( numViews, 0, predDirection, 7, index, 
   ZFarSign, ZFarExp, ZFarMantissa, ZFarManLen ) 
  
}  

3dv_acquisition_element( numViews, predDirection, expLen, index, outSign, 
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
   outManLen[ index, i ] = manLen = mantissa_len_minus1 + 1   
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

vsp_param( numViews, predDirection, index ) {  
 for( i = 0; i < numViews; i++ )   
  for( j = 0; j < i; j++ ) {   
   disparity_diff_wji[ j ][ i ] 0 ue(v) 
   disparity_diff_oji[ j ][ i ] 0 ue(v) 
   disparity_diff_wij[ i ][ j ] 0 ue(v) 
   disparity_diff_oij[ i ][ j ] 0 ue(v) 
  }   
}   

slice_header_in_3davc_extension() {  
 first_mb_in_slice 2 ue(v) 
 slice_type 2 ue(v) 
 pic_parameter_set_id 2 ue(v) 
 if( avc_3d_extension_flag  &&  slice_header_prediction_flag  !=  0 ) {   
  pre_slice_header_src 2 u(2) 
  if( slice_type  ==  P  ||  slice_type  ==  SP ||  slice_type  ==  B ) { 
   pre_ref_lists_src 2 u(2) 
   if( !pre_ref_lists_src ) {   
    num_ref_idx_active_override_flag 2 u(1) 
    if( num_ref_idx_active_override_flag ) {   
    num_ref_idx_l0_active_minus1 2 ue(v) 
    if( slice_type  ==  B ) 
     num_ref_idx_l1_active_minus1 2 ue(v) 
    }   
    ref_pic_list_mvc_modification()  /* specified in Annex H */ 2  
   }   
  }   
  if( ( weighted_pred_flag  &&  ( slice_type  ==  P  ||  slice_type  ==  SP ) )  ||  ( weighted_bipred_idc  ==  1  &&  slice_type  ==  B ) ) { 
   pre_pred_weight_table_src 2 u(2) 
   if( !pre_pred_weight_table_src )   
    pred_weight_table() 2  
  }
  if( nal_ref_idc != 0 ) {   
   pre_dec_ref_pic_marking_src 2 u(2) 
   if( !pre_dec_ref_pic_marking_src )   
    dec_ref_pic_marking() 2  
  }   
  slice_qp_delta 2 se(v) 
 } else {   
  if( separate_colour_plane_flag  ==  1 )   
   colour_plane_id 2 u(2) 
  frame_num 2 u(v) 
  if( !frame_mbs_only_flag ) {   
   field_pic_flag 2 u(1) 
   if( field_pic_flag )   
    bottom_field_flag 2 u(1) 
  }   
  if( IdrPicFlag )   
   idr_pic_id 2 ue(v) 
  if( pic_order_cnt_type  ==  0 ) {   
   pic_order_cnt_lsb 2 u(v) 
   if( bottom_field_pic_order_in_frame_present_flag &&  !field_pic_flag )   
    delta_pic_order_cnt_bottom 2 se(v) 
  }   
  if( pic_order_cnt_type == 1 && !delta_pic_order_always_zero_flag ) {   
   delta_pic_order_cnt[ 0 ] 2 se(v) 
   if( bottom_field_pic_order_in_frame_present_flag && !field_pic_flag )   
    delta_pic_order_cnt[ 1 ] 2 se(v) 
  }   
  if( redundant_pic_cnt_present_flag )   
   redundant_pic_cnt 2 ue(v) 
  if( slice_type  ==  B )   
   direct_spatial_mv_pred_flag 2 u(1) 
  if( slice_type  ==  P  ||  slice_type  ==  SP  ||  slice_type  ==  B ) {   
   num_ref_idx_active_override_flag 2 u(1) 
   if( num_ref_idx_active_override_flag ) {   
    num_ref_idx_l0_active_minus1 2 ue(v) 
    if( slice_type  ==  B )   
     num_ref_idx_l1_active_minus1 2 ue(v) 
   }   
  }   
  if( nal_unit_type  ==  20  ||  nal_unit_type  ==  21 )   
   ref_pic_list_mvc_modification()  /* specified in Annex H */ 2  
  else   
   ref_pic_list_modification() 2  
  if( ( weighted_pred_flag  &&  ( slice_type  ==  P  || slice_type  ==  SP ) )  ||  ( weighted_bipred_idc  ==  1  &&  slice_type  ==  B ) ) 
   pred_weight_table() 2  
  if( nal_ref_idc != 0 )   
   dec_ref_pic_marking() 2  
  if( entropy_coding_mode_flag  &&  slice_type  !=  I  && slice_type  !=  SI ) 
   cabac_init_idc 2 ue(v) 
  slice_qp_delta 2 se(v) 
  if( slice_type  ==  SP  ||  slice_type  ==  SI ) {   
   if( slice_type  ==  SP )   
    sp_for_switch_flag 2 u(1) 
   slice_qs_delta 2 se(v) 
  }   
  if( deblocking_filter_control_present_flag ) {   
   disable_deblocking_filter_idc 2 ue(v) 
   if( disable_deblocking_filter_idc  !=  1 ) {   
    slice_alpha_c0_offset_div2 2 se(v) 
    slice_beta_offset_div2 2 se(v) 
   }   
  }   
  if( num_slice_groups_minus1 > 0  && slice_group_map_type >= 3  &&  slice_group_map_type <= 5) 
     slice_group_change_cycle 2 u(v) 
  if( nal_unit_type ==  21  && ( slice_type  !=  I  && slice_type  !=  SI ) ) { 
   if( DepthFlag )   
    depth_weighted_pred_flag 2 u(1) 
   else if( avc_3d_extension_flag  ) {   
    dmvp_flag 2 u(1) 
    if( seq_view_synthesis_flag )   
     slice_vsp_flag 2 u(1) 
   }   
   if( 3dv_acquisition_idc != 1  && ( depth_weighted_pred_flag  ||  dmvp_flag ) ) 
    dps_id 2 ue(v) 
  }   
 }   
}  

slice_data_in_3davc_extension() {  
 if( entropy_coding_mode_flag )   
  while( !byte_aligned() )   
   cabac_alignment_one_bit 2 f(1) 
 CurrMbAddr = first_mb_in_slice * ( 1 + MbaffFrameFlag )   
 moreDataFlag = 1   
 prevMbSkipped = 0   
 RunLength = 0   
 do {   
  if( slice_type  !=  I  &&  slice_type  !=  SI )   
   if( !entropy_coding_mode_flag ) {   
    mb_skip_run 2 ue(v) 
    prevMbSkipped = ( mb_skip_run > 0 )   
    for( i=0; i<mb_skip_run; i++ )   
     CurrMbAddr = NextMbAddress( CurrMbAddr )   
    if( nal_unit_type  ==  21  &&  !DepthFlag  &&  
     mb_skip_run > 0  &&  VspRefExist )   
     mb_skip_type_flag 2 u(1) 
    if( mb_skip_run > 0 )   
     moreDataFlag = more_rbsp_data()   
   } else {   
    if( nal_unit_type  ==  21  &&  !DepthFlag  && VspRefExist  &&  leftMbVSSkipped  && upMbVSSkipped ) { 
     mb_vsskip_flag 2 ae(v) 
     moreDataFlag = !mb_vsskip_flag   
     if( !mb_vsskip_flag ) {   
      mb_skip_flag 2 ae(v) 
      moreDataFlag = !mb_skip_flag   
     }   
     RunLength = 0  
    } else {   
     rleCtx = RLESkipContext()   
     if( rleCtx  &&  !RunLength ) {   
      mb_skip_run_type 2 ae(v) 
      RunLength = 16   
     } else if( !rleCtx  &&  RunLength )   
      RunLength = 0   
     if( rleCtx  &&  mb_skip_run_type )   
      RunLength -= 1   
     else   
      mb_skip_flag 2 ae(v) 
     if( rleCtx  &&  !mb_skip_flag )   
      RunLength = 0   
     moreDataFlag = !mb_skip_flag   
     if( nal_unit_type  ==  21  &&  !DepthFlag  && VspRefExist  &&  !mb_skip_flag ) { 
      mb_vsskip_flag 2 ae(v) 
      moreDataFlag = !mb_vsskip_flag   
     }   
    }   
    if(alc_sps_enable_flag  &&  nal_unit_type  ==  21 &&  slice_type  ==  P  &&  !DepthFlag  && !mb_vsskip_flag  &&  mb_skip_flag  ==  1 ) 
     mb_alc_skip_flag 2 ae(v) 
   }   
  if( moreDataFlag ) {   
   if( MbaffFrameFlag && ( CurrMbAddr % 2  ==  0  || ( CurrMbAddr % 2  ==  1  &&  prevMbSkipped ) ) ) 
    mb_field_decoding_flag 2 u(1) | ae(v) 
   macroblock_layer_in_3davc_extension() 2 | 3 | 4  
  }   
  if( !entropy_coding_mode_flag )   
   moreDataFlag = more_rbsp_data()   
  else {   
   if( slice_type  !=  I  &&  slice_type  !=  SI ) 
    prevMbSkipped = mb_skip_flag  ||  mb_vsskip_flag   
   if( MbaffFrameFlag  &&  CurrMbAddr % 2  ==  0 )   
    moreDataFlag = 1   
   else {   
    end_of_slice_flag 2 ae(v) 
    moreDataFlag = !end_of_slice_flag   
   }   
  }   
  CurrMbAddr = NextMbAddress( CurrMbAddr )   
 } while( moreDataFlag )   
}  

macroblock_layer_in_3davc_extension() {  
 mb_type 2 ue(v) | ae(v) 
 if( nal_unit_type  == 21  &&  !DepthFlag   
  &&  slice_type  ==  B  
  &&  direct_spatial_mv_pred_flag  &&  VspRefExist 
  &&  mb_type  ==  B_Direct_16x16 ) 
  
  mb_direct_type_flag 2 u(1) | ae(v) 
 if( alc_sps_enable_flag  &&  nal_unit_type  ==  21  && 
  slice_type  ==  P  &&  !DepthFlag  && 
  ( mb_type  ==  P_L0_16x16  ||   
   mb_type  ==  P_L0_L0_16x8  || 
   mb_type  ==  P_L0_L0_8x16 ) ) 
  
  mb_alc_flag 2 u(1) | ae(v) 
 if( mb_type  ==  I_PCM ) {   
  while( !byte_aligned() )   
   pcm_alignment_zero_bit 3 f(1) 
  for( i = 0; i < 256; i++ )   
   pcm_sample_luma[ i ] 3 u(v) 
  for( i = 0; i < 2 * MbWidthC * MbHeightC; i++ )   
   pcm_sample_chroma[ i ] 3 u(v) 
 } else {   
  noSubMbPartSizeLessThan8x8Flag = 1   
  if( mb_type  !=  I_NxN  && 
   MbPartPredMode( mb_type, 0 )  !=  Intra_16x16  && 
   NumMbPart( mb_type )  ==  4 ) { 
  
   sub_mb_pred_in_3davc_extension( mb_type ) 2  
   for( mbPartIdx = 0; mbPartIdx < 4; mbPartIdx++ )   
    if( sub_mb_type[ mbPartIdx ]  !=  B_Direct_8x8 ) {   
     if( NumSubMbPart( sub_mb_type[ mbPartIdx ] )  >  1 )   
      noSubMbPartSizeLessThan8x8Flag = 0   
    } else if( !direct_8x8_inference_flag )   
     noSubMbPartSizeLessThan8x8Flag = 0   
  } else {   
   if( transform_8x8_mode_flag  &&  mb_type  ==  I_NxN )   
    transform_size_8x8_flag 2 u(1) | ae(v)
      mb_pred_in_3davc_extension( mb_type ) 2  
  }   
  if( MbPartPredMode( mb_type, 0 )  !=  Intra_16x16 ) {   
   coded_block_pattern 2 me(v) | ae(v) 
   if( ( CodedBlockPatternLuma > 0 || mb_alc_flag == 1 ) && 
     transform_8x8_mode_flag  &&  mb_type  !=  I_NxN  && 
     noSubMbPartSizeLessThan8x8Flag  && 
     ( mb_type  !=  B_Direct_16x16  || 
       direct_8x8_inference_flag ) ) 
  
    transform_size_8x8_flag 2 u(1) | ae(v) 
  }   
  if( CodedBlockPatternLuma > 0  ||   
   CodedBlockPatternChroma > 0  || 
   MbPartPredMode( mb_type, 0 )  ==  Intra_16x16 ) { 
  
   mb_qp_delta 2 se(v) | ae(v) 
   residual( 0, 15 ) 3 | 4  
  }   
 }   
}  

mb_pred_in_3davc_extension( mb_type ) {  
 if( MbPartPredMode( mb_type, 0 )  ==  Intra_4x4  ||   
  MbPartPredMode( mb_type, 0 )  ==  Intra_8x8  ||   
  MbPartPredMode( mb_type, 0 )  ==  Intra_16x16 ) { 
  
  if( MbPartPredMode( mb_type, 0 )  ==  Intra_4x4 )   
   for( luma4x4BlkIdx=0; luma4x4BlkIdx<16; luma4x4BlkIdx++ ) {   
    prev_intra4x4_pred_mode_flag[ luma4x4BlkIdx ] 2 u(1) | ae(v) 
    if( !prev_intra4x4_pred_mode_flag[ luma4x4BlkIdx ] )   
     rem_intra4x4_pred_mode[ luma4x4BlkIdx ] 2 u(3) | ae(v) 
   }   
  if( MbPartPredMode( mb_type, 0 )  ==  Intra_8x8 )   
   for( luma8x8BlkIdx=0; luma8x8BlkIdx<4; luma8x8BlkIdx++ ) {   
    prev_intra8x8_pred_mode_flag[ luma8x8BlkIdx ] 2 u(1) | ae(v) 
    if( !prev_intra8x8_pred_mode_flag[ luma8x8BlkIdx ] )   
     rem_intra8x8_pred_mode[ luma8x8BlkIdx ] 2 u(3) | ae(v) 
   }   
  if( ChromaArrayType  ==  1  ||  ChromaArrayType  ==  2 )   
   intra_chroma_pred_mode 2 ue(v) | ae(v) 
 } else if( MbPartPredMode( mb_type, 0 )  !=  Direct ) {   
  for( mbPartIdx = 0; mbPartIdx < NumMbPart( mb_type ); mbPartIdx++ )   
   if( ( num_ref_idx_l0_active_minus1 > 0  || 
     mb_field_decoding_flag  !=  field_pic_flag ) &&   
    MbPartPredMode( mb_type, mbPartIdx )  !=  Pred_L1  && 
    mb_alc_flag  ==  0 ) { 
  
    ref_idx_l0[ mbPartIdx ] 2 te(v) | ae(v) 
    if( VspRefL0Flag[ mbPartIdx ]  &&  slice_vsp_flag )   
     bvsp_flag_l0[ mbPartIdx ] 2 u(1) | ae(v) 
   }   
  for( mbPartIdx = 0; mbPartIdx < NumMbPart( mb_type ); mbPartIdx++ )  
    if( ( num_ref_idx_l1_active_minus1  >  0  || 
     mb_field_decoding_flag  !=  field_pic_flag ) &&   
    MbPartPredMode( mb_type, mbPartIdx )  !=  Pred_L0 ) { 
  
    ref_idx_l1[ mbPartIdx ] 2 te(v) | ae(v) 
    if( VspRefL1Flag[ mbPartIdx ]  &&  slice_vsp_flag )   
     bvsp_flag_l1[ mbPartIdx ] 2 u(1) | ae(v) 
   }   
  for( mbPartIdx = 0; mbPartIdx < NumMbPart( mb_type ); mbPartIdx++ )   
   if( MbPartPredMode ( mb_type, mbPartIdx )  !=  Pred_L1  && 
    ( !VspRefL0Flag[ mbPartIdx ]  ||  !bvsp_flag_l0[ mbPartIdx ] ) ) 
  
    for( compIdx = 0; compIdx < 2; compIdx++ )   
     mvd_l0[ mbPartIdx ][ 0 ][ compIdx ] 2 se(v) | ae(v) 
  for( mbPartIdx = 0; mbPartIdx < NumMbPart( mb_type ); mbPartIdx++ )   
   if( MbPartPredMode( mb_type, mbPartIdx )  !=  Pred_L0  && 
    ( !VspRefL1Flag[ mbPartIdx ]  ||  !bvsp_flag_l1[ mbPartIdx ] ) ) 
  
    for( compIdx = 0; compIdx < 2; compIdx++ )   
     mvd_l1[ mbPartIdx ][ 0 ][ compIdx ] 2 se(v) | ae(v) 
 }   
}  

sub_mb_pred_in_3davc_extension( mb_type ) {  
 for( mbPartIdx = 0; mbPartIdx < 4; mbPartIdx++ )   
  sub_mb_type[ mbPartIdx ] 2 ue(v) | ae(v) 
 for( mbPartIdx = 0; mbPartIdx < 4; mbPartIdx++ )   
  if( ( num_ref_idx_l0_active_minus1  >  0  || mb_field_decoding_flag  !=  field_pic_flag ) && mb_type  !=  P_8x8ref0  && sub_mb_type[ mbPartIdx ]  !=  B_Direct_8x8  && SubMbPredMode( sub_mb_type[ mbPartIdx ] )  !=  Pred_L1 &&  mb_alc_flag  ==  0 ) { 
   ref_idx_l0[ mbPartIdx ] 2 te(v) | ae(v) 
   if( VspRefL0Flag[ mbPartIdx ]  &&  slice_vsp_flag )   
    bvsp_flag_l0[ mbPartIdx ] 2 u(1) | ae(v) 
  }   
 for( mbPartIdx = 0; mbPartIdx < 4; mbPartIdx++ )   
  if( ( num_ref_idx_l1_active_minus1  >  0  ||   
    mb_field_decoding_flag  !=  field_pic_flag ) && 
     sub_mb_type[ mbPartIdx ]  !=  B_Direct_8x8  && 
     SubMbPredMode( sub_mb_type[ mbPartIdx ] )  !=  Pred_L0 ) { 
  
   ref_idx_l1[ mbPartIdx ] 2 te(v) | ae(v) 
   if( VspRefL1Flag[ mbPartIdx ]  &&  slice_vsp_flag )   
    bvsp_flag_l1[ mbPartIdx ] 2 u(1) | ae(v) 
  }   
 for( mbPartIdx = 0; mbPartIdx < 4; mbPartIdx++ )   
  if( sub_mb_type[ mbPartIdx ]  !=  B_Direct_8x8  && SubMbPredMode( sub_mb_type[ mbPartIdx ] )  !=  Pred_L1 && ( !VspRefL0Flag[ mbPartIdx ]  ||  !bvsp_flag_l0[ mbPartIdx ] ) ) 
   for( subMbPartIdx = 0; subMbPartIdx < NumSubMbPart( sub_mb_type[ mbPartIdx ] ); subMbPartIdx++ ) 
     for( compIdx = 0; compIdx < 2; compIdx++ )   
       mvd_l0[ mbPartIdx ][ subMbPartIdx ][ compIdx ] 2 se(v) | ae(v)
 for( mbPartIdx = 0; mbPartIdx < 4; mbPartIdx++ )
   if( sub_mb_type[ mbPartIdx ]  !=  B_Direct_8x8  && SubMbPredMode( sub_mb_type[ mbPartIdx ] )  !=  Pred_L0 && ( !VspRefL1Flag[ mbPartIdx ]  ||  !bvsp_flag_l1[ mbPartIdx ] ) ) 
    for( subMbPartIdx = 0; subMbPartIdx < NumSubMbPart( sub_mb_type[ mbPartIdx ] ); subMbPartIdx++ ) 
     for( compIdx = 0; compIdx < 2; compIdx++ )   
       mvd_l1[ mbPartIdx ][ subMbPartIdx ][ compIdx ] 2 se(v) | ae(v)
}   
