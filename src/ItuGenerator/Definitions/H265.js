nal_unit( NumBytesInNalUnit ) { 
  nal_unit_header()  
  /*NumBytesInRbsp = 0 */ 
  /*for( i = 2; i < NumBytesInNalUnit; i++ )  */
  /* if( i + 2 < NumBytesInNalUnit  &&  next_bits( 24 ) == 0x000003 ) {  */
  /*   rbsp_byte[ NumBytesInRbsp++ ] b(8) */
  /*   rbsp_byte[ NumBytesInRbsp++ ] b(8) */
  /*   i +=  2  */
  /*   emulation_prevention_three_byte  *//* equal to 0x03 *//* f(8) */
  /*  } else  */
  /*    rbsp_byte[ NumBytesInRbsp++ ] b(8) */
} 

nal_unit_header() { 
  forbidden_zero_bit f(1)
  nal_unit_type u(6)
  nuh_layer_id u(6)
  nuh_temporal_id_plus1 u(3)
} 

video_parameter_set_rbsp() {  
 vps_video_parameter_set_id u(4) 
 vps_base_layer_internal_flag u(1) 
 vps_base_layer_available_flag u(1) 
 vps_max_layers_minus1 u(6) 
 vps_max_sub_layers_minus1 u(3) 
 vps_temporal_id_nesting_flag u(1) 
 vps_reserved_0xffff_16bits u(16)
    profile_tier_level(1, vps_max_sub_layers_minus1)  
 vps_sub_layer_ordering_info_present_flag u(1)
    for (i = (vps_sub_layer_ordering_info_present_flag ? 0 : vps_max_sub_layers_minus1);
        i <= vps_max_sub_layers_minus1; i++) {

        vps_max_dec_pic_buffering_minus1[i] ue(v)
        vps_max_num_reorder_pics[i] ue(v)
        vps_max_latency_increase_plus1[i] ue(v)
    }  
 vps_max_layer_id u(6) 
 vps_num_layer_sets_minus1 ue(v)
    for (i = 1; i <= vps_num_layer_sets_minus1; i++)
        for (j = 0; j <= vps_max_layer_id; j++)
            layer_id_included_flag[i][j] u(1) 
 vps_timing_info_present_flag u(1)
    if (vps_timing_info_present_flag) {  
  vps_num_units_in_tick u(32) 
  vps_time_scale u(32) 
  vps_poc_proportional_to_timing_flag u(1)
        if (vps_poc_proportional_to_timing_flag)  
   vps_num_ticks_poc_diff_one_minus1 ue(v) 
  vps_num_hrd_parameters ue(v)
        for (i = 0; i < vps_num_hrd_parameters; i++) {
            hrd_layer_set_idx[i] ue(v)
            if (i > 0)
                cprms_present_flag[i] u(1)
            hrd_parameters(cprms_present_flag[i], vps_max_sub_layers_minus1)
        }
    }  
 vps_extension_flag u(1)
    if (vps_extension_flag)
        while (more_rbsp_data())  
   vps_extension_data_flag u(1)
    rbsp_trailing_bits()
}

seq_parameter_set_rbsp() {  
 sps_video_parameter_set_id u(4) 
 sps_max_sub_layers_minus1 u(3) 
 sps_temporal_id_nesting_flag u(1)
    profile_tier_level(1, sps_max_sub_layers_minus1)  
 sps_seq_parameter_set_id ue(v) 
 chroma_format_idc ue(v)
    if (chroma_format_idc == 3)  
  separate_colour_plane_flag u(1) 
 pic_width_in_luma_samples ue(v) 
 pic_height_in_luma_samples ue(v) 
 conformance_window_flag u(1)
    if (conformance_window_flag) {  
  conf_win_left_offset ue(v) 
  conf_win_right_offset ue(v) 
  conf_win_top_offset ue(v) 
  conf_win_bottom_offset ue(v)
    }  
 bit_depth_luma_minus8 ue(v) 
 bit_depth_chroma_minus8 ue(v) 
 log2_max_pic_order_cnt_lsb_minus4 ue(v) 
 sps_sub_layer_ordering_info_present_flag u(1)
    for (i = (sps_sub_layer_ordering_info_present_flag ? 0 : sps_max_sub_layers_minus1);
        i <= sps_max_sub_layers_minus1; i++) {

        sps_max_dec_pic_buffering_minus1[i] ue(v)
        sps_max_num_reorder_pics[i] ue(v)
        sps_max_latency_increase_plus1[i] ue(v)
    }  
 log2_min_luma_coding_block_size_minus3 ue(v) 
 log2_diff_max_min_luma_coding_block_size ue(v) 
 log2_min_luma_transform_block_size_minus2 ue(v) 
 log2_diff_max_min_luma_transform_block_size ue(v) 
 max_transform_hierarchy_depth_inter ue(v) 
 max_transform_hierarchy_depth_intra ue(v) 
 scaling_list_enabled_flag u(1)
    if (scaling_list_enabled_flag) {  
  sps_scaling_list_data_present_flag u(1)
        if (sps_scaling_list_data_present_flag)
            scaling_list_data()
    }  
 amp_enabled_flag u(1) 
 sample_adaptive_offset_enabled_flag u(1) 
 pcm_enabled_flag u(1)
    if (pcm_enabled_flag) {  
  pcm_sample_bit_depth_luma_minus1 u(4) 
  pcm_sample_bit_depth_chroma_minus1 u(4) 
   log2_min_pcm_luma_coding_block_size_minus3 ue(v) 
  log2_diff_max_min_pcm_luma_coding_block_size ue(v) 
  pcm_loop_filter_disabled_flag u(1)
    }  
 num_short_term_ref_pic_sets ue(v)
    for (i = 0; i < num_short_term_ref_pic_sets; i++)
        st_ref_pic_set(i)  
 long_term_ref_pics_present_flag u(1)
    if (long_term_ref_pics_present_flag) {  
  num_long_term_ref_pics_sps ue(v)
        for (i = 0; i < num_long_term_ref_pics_sps; i++) {
            lt_ref_pic_poc_lsb_sps[i] u(v)
            used_by_curr_pic_lt_sps_flag[i] u(1)
        }
    }  
 sps_temporal_mvp_enabled_flag u(1) 
 strong_intra_smoothing_enabled_flag u(1) 
 vui_parameters_present_flag u(1)
    if (vui_parameters_present_flag)
        vui_parameters()  
 sps_extension_present_flag u(1)
    if (sps_extension_present_flag) {  
  sps_range_extension_flag u(1) 
  sps_multilayer_extension_flag u(1) 
  sps_3d_extension_flag u(1) 
  sps_scc_extension_flag u(1) 
  sps_extension_4bits u(4)
    }
    if (sps_range_extension_flag)
        sps_range_extension()
    if (sps_multilayer_extension_flag)
        sps_multilayer_extension()  /* specified in Annex F */
    if (sps_3d_extension_flag)
        sps_3d_extension()  /* specified in Annex I */
    if (sps_scc_extension_flag)
        sps_scc_extension()
    if (sps_extension_4bits)
        while (more_rbsp_data())  
   sps_extension_data_flag u(1)
    rbsp_trailing_bits()
}  

sps_range_extension() {  
 transform_skip_rotation_enabled_flag u(1) 
 transform_skip_context_enabled_flag u(1) 
 implicit_rdpcm_enabled_flag u(1) 
 explicit_rdpcm_enabled_flag u(1) 
 extended_precision_processing_flag u(1) 
 intra_smoothing_disabled_flag u(1) 
 high_precision_offsets_enabled_flag u(1) 
 persistent_rice_adaptation_enabled_flag u(1) 
 cabac_bypass_alignment_enabled_flag u(1) 
}  

sps_scc_extension() {  
 sps_curr_pic_ref_enabled_flag u(1) 
 palette_mode_enabled_flag u(1) 
 if( palette_mode_enabled_flag ) {  
  palette_max_size ue(v) 
  delta_palette_max_predictor_size ue(v) 
  sps_palette_predictor_initializers_present_flag u(1) 
  if( sps_palette_predictor_initializers_present_flag ) {  
   sps_num_palette_predictor_initializers_minus1 ue(v) 
   numComps = ( chroma_format_idc == 0 ) ? 1 : 3  
   for( comp = 0; comp < numComps; comp++ )  
    for( i = 0; i <= sps_num_palette_predictor_initializers_minus1; i++ )  
     sps_palette_predictor_initializer[ comp ][ i ] u(v) 
  }  
 }  
 motion_vector_resolution_control_idc u(2) 
 intra_boundary_filtering_disabled_flag u(1) 
}  

pic_parameter_set_rbsp() { 
 pps_pic_parameter_set_id ue(v) 
 pps_seq_parameter_set_id ue(v) 
 dependent_slice_segments_enabled_flag u(1) 
 output_flag_present_flag u(1) 
 num_extra_slice_header_bits u(3) 
 sign_data_hiding_enabled_flag u(1) 
 cabac_init_present_flag u(1) 
  num_ref_idx_l0_default_active_minus1 ue(v) 
 num_ref_idx_l1_default_active_minus1 ue(v) 
 init_qp_minus26 se(v) 
 constrained_intra_pred_flag u(1) 
 transform_skip_enabled_flag u(1) 
 cu_qp_delta_enabled_flag u(1) 
 if( cu_qp_delta_enabled_flag )  
  diff_cu_qp_delta_depth ue(v) 
 pps_cb_qp_offset se(v) 
 pps_cr_qp_offset se(v) 
 pps_slice_chroma_qp_offsets_present_flag u(1) 
 weighted_pred_flag u(1) 
 weighted_bipred_flag u(1) 
 transquant_bypass_enabled_flag u(1) 
 tiles_enabled_flag u(1) 
 entropy_coding_sync_enabled_flag u(1) 
 if( tiles_enabled_flag ) {  
  num_tile_columns_minus1 ue(v) 
  num_tile_rows_minus1 ue(v) 
  uniform_spacing_flag u(1) 
  if( !uniform_spacing_flag ) {  
   for( i = 0; i < num_tile_columns_minus1; i++ )  
    column_width_minus1[ i ] ue(v) 
   for( i = 0; i < num_tile_rows_minus1; i++ )  
    row_height_minus1[ i ] ue(v) 
  }  
  loop_filter_across_tiles_enabled_flag u(1) 
 }  
 pps_loop_filter_across_slices_enabled_flag u(1) 
 deblocking_filter_control_present_flag u(1) 
 if( deblocking_filter_control_present_flag ) {  
  deblocking_filter_override_enabled_flag u(1) 
  pps_deblocking_filter_disabled_flag u(1) 
  if( !pps_deblocking_filter_disabled_flag ) {  
   pps_beta_offset_div2 se(v) 
   pps_tc_offset_div2 se(v) 
  }  
 }  
 pps_scaling_list_data_present_flag u(1) 
 if( pps_scaling_list_data_present_flag )  
  scaling_list_data()  
 lists_modification_present_flag u(1) 
 log2_parallel_merge_level_minus2 ue(v) 
 slice_segment_header_extension_present_flag u(1) 
 pps_extension_present_flag u(1) 
 if( pps_extension_present_flag ) {  
  pps_range_extension_flag u(1) 
  pps_multilayer_extension_flag u(1)
    pps_3d_extension_flag u(1) 
  pps_scc_extension_flag u(1) 
  pps_extension_4bits u(4) 
 }  
 if( pps_range_extension_flag )  
  pps_range_extension()  
 if( pps_multilayer_extension_flag )  
  pps_multilayer_extension()  /* specified in Annex F */  
 if( pps_3d_extension_flag )  
  pps_3d_extension()  /* specified in Annex I */  
 if( pps_scc_extension_flag )  
  pps_scc_extension()  
 if( pps_extension_4bits )  
  while( more_rbsp_data() )  
   pps_extension_data_flag u(1) 
 rbsp_trailing_bits()  
} 

pps_range_extension() { 
 if( transform_skip_enabled_flag )  
  log2_max_transform_skip_block_size_minus2 ue(v) 
 cross_component_prediction_enabled_flag u(1) 
 chroma_qp_offset_list_enabled_flag u(1) 
 if( chroma_qp_offset_list_enabled_flag ) {  
  diff_cu_chroma_qp_offset_depth ue(v) 
  chroma_qp_offset_list_len_minus1 ue(v) 
  for( i = 0; i <= chroma_qp_offset_list_len_minus1; i++ ) {  
   cb_qp_offset_list[ i ] se(v) 
   cr_qp_offset_list[ i ] se(v) 
  }  
 }  
 log2_sao_offset_scale_luma ue(v) 
 log2_sao_offset_scale_chroma ue(v) 
}  

pps_scc_extension() { 
 pps_curr_pic_ref_enabled_flag u(1) 
 residual_adaptive_colour_transform_enabled_flag u(1) 
 if( residual_adaptive_colour_transform_enabled_flag ) {  
  pps_slice_act_qp_offsets_present_flag u(1) 
  pps_act_y_qp_offset_plus5 se(v) 
  pps_act_cb_qp_offset_plus5 se(v) 
  pps_act_cr_qp_offset_plus3 se(v) 
 }  
 pps_palette_predictor_initializers_present_flag u(1) 
 if( pps_palette_predictor_initializers_present_flag ) {  
  pps_num_palette_predictor_initializers ue(v) 
  if( pps_num_palette_predictor_initializers > 0 ) {  
   monochrome_palette_flag u(1) 
   luma_bit_depth_entry_minus8 ue(v) 
   if( !monochrome_palette_flag )  
    chroma_bit_depth_entry_minus8 ue(v) 
   numComps = monochrome_palette_flag ? 1 : 3  
   for( comp = 0; comp < numComps; comp++ )  
    for( i = 0; i < pps_num_palette_predictor_initializers; i++ )  
     pps_palette_predictor_initializer[ comp ][ i ] u(v) 
  }  
 }  
}  

sei_rbsp() { 
do  
sei_message()  
while( more_rbsp_data() )  
rbsp_trailing_bits()  
}  

access_unit_delimiter_rbsp() { 
 pic_type u(3) 
 rbsp_trailing_bits()  
}  

end_of_seq_rbsp() { 
} 

end_of_bitstream_rbsp() { 
}  

filler_data_rbsp() { 
while( next_bits( 8 ) == 0xFF )  
ff_byte  /* equal to 0xFF */ f(8) 
rbsp_trailing_bits()  
}  

slice_segment_layer_rbsp() { 
slice_segment_header()  
/*slice_segment_data()  */
/* rbsp_slice_segment_trailing_bits()   */
}  

rbsp_trailing_bits() { 
rbsp_stop_one_bit  /* equal to 1 */ f(1) 
while( !byte_aligned() )  
rbsp_alignment_zero_bit  /* equal to 0 */ f(1) 
} 

byte_alignment() { 
 alignment_bit_equal_to_one  /* equal to 1 */ f(1) 
 while( !byte_aligned() )  
  alignment_bit_equal_to_zero  /* equal to 0 */ f(1) 
}  

profile_tier_level( profilePresentFlag, maxNumSubLayersMinus1 ) { 
 if( profilePresentFlag ) {  
  general_profile_space u(2) 
  general_tier_flag u(1) 
  general_profile_idc u(5) 
  for( j = 0; j < 32; j++ )  
   general_profile_compatibility_flag[ j ] u(1) 
  general_progressive_source_flag u(1) 
  general_interlaced_source_flag u(1) 
  general_non_packed_constraint_flag u(1) 
  general_frame_only_constraint_flag u(1) 
  if( general_profile_idc == 4  ||  general_profile_compatibility_flag[ 4 ]  || 
   general_profile_idc == 5  ||  general_profile_compatibility_flag[ 5 ]  || 
   general_profile_idc == 6  ||  general_profile_compatibility_flag[ 6 ]  || 
   general_profile_idc == 7  ||  general_profile_compatibility_flag[ 7 ]  || 
   general_profile_idc == 8  ||  general_profile_compatibility_flag[ 8 ]  || 
   general_profile_idc == 9  ||  general_profile_compatibility_flag[ 9 ]  || 
   general_profile_idc == 10  ||  general_profile_compatibility_flag[ 10 ] ) { 
   /* The number of bits in this syntax structure is not affected by this condition */ 
 
   general_max_12bit_constraint_flag u(1) 
   general_max_10bit_constraint_flag u(1) 
   general_max_8bit_constraint_flag u(1) 
   general_max_422chroma_constraint_flag u(1) 
   general_max_420chroma_constraint_flag u(1) 
   general_max_monochrome_constraint_flag u(1) 
   general_intra_constraint_flag u(1) 
   general_one_picture_only_constraint_flag u(1) 
   general_lower_bit_rate_constraint_flag u(1) 
   if( general_profile_idc == 5 ||  general_profile_compatibility_flag[ 5 ]  || 
    general_profile_idc == 9  ||  general_profile_compatibility_flag[ 9 ]  || 
    general_profile_idc == 10  ||  general_profile_compatibility_flag[ 10 ] ) { 
 
    general_max_14bit_constraint_flag u(1) 
    general_reserved_zero_33bits u(33) 
   } else  
    general_reserved_zero_34bits u(34) 
  } else if( general_profile_idc == 2  ||  general_profile_compatibility_flag[ 2 ] ) {  
   general_reserved_zero_7bits u(7) 
   general_one_picture_only_constraint_flag u(1) 
   general_reserved_zero_35bits u(35) 
  } else  
     general_reserved_zero_43bits u(43) 
  if( ( general_profile_idc >= 1  &&  general_profile_idc <= 5 )  || 
    general_profile_idc == 9  || 
    general_profile_compatibility_flag[ 1 ]  ||  general_profile_compatibility_flag[ 2 ]  || 
    general_profile_compatibility_flag[ 3 ]  ||  general_profile_compatibility_flag[ 4 ]  || 
    general_profile_compatibility_flag[ 5 ]  ||  general_profile_compatibility_flag[ 9 ] ) 
   /* The number of bits in this syntax structure is not affected by this condition */ 
   general_inbld_flag u(1) 
  else  
   general_reserved_zero_bit u(1) 
 }  
 general_level_idc u(8) 
 for( i = 0; i < maxNumSubLayersMinus1; i++ ) {  
  sub_layer_profile_present_flag[ i ] u(1) 
  sub_layer_level_present_flag[ i ] u(1) 
 }  
 if( maxNumSubLayersMinus1 > 0 )  
  for( i = maxNumSubLayersMinus1; i < 8; i++ )  
   reserved_zero_2bits[ i ] u(2) 
 for( i = 0; i < maxNumSubLayersMinus1; i++ ) {  
  if( sub_layer_profile_present_flag[ i ] ) {  
   sub_layer_profile_space[ i ] u(2) 
   sub_layer_tier_flag[ i ] u(1) 
   sub_layer_profile_idc[ i ] u(5) 
   for( j = 0; j < 32; j++ )  
    sub_layer_profile_compatibility_flag[ i ][ j ] u(1) 
   sub_layer_progressive_source_flag[ i ] u(1) 
   sub_layer_interlaced_source_flag[ i ] u(1) 
   sub_layer_non_packed_constraint_flag[ i ] u(1) 
   sub_layer_frame_only_constraint_flag[ i ] u(1) 
   if( sub_layer_profile_idc[ i ] == 4  ||  sub_layer_profile_compatibility_flag[ i ][ 4 ]  || 
    sub_layer_profile_idc[ i ] == 5  ||  sub_layer_profile_compatibility_flag[ i ][ 5 ]  || 
    sub_layer_profile_idc[ i ] == 6  ||  sub_layer_profile_compatibility_flag[ i ][ 6 ]  || 
    sub_layer_profile_idc[ i ] == 7  ||  sub_layer_profile_compatibility_flag[ i ][ 7 ]  || 
    sub_layer_profile_idc[ i ] == 8  ||  sub_layer_profile_compatibility_flag[ i ][ 8 ]  || 
    sub_layer_profile_idc[ i ] == 9  ||  sub_layer_profile_compatibility_flag[ i ][ 9 ]  || 
    sub_layer_profile_idc[ i ] == 10  ||  sub_layer_profile_compatibility_flag[ i ][ 10 ] 
    ) { 
    /* The number of bits in this syntax structure is not affected by this condition */ 
 
    sub_layer_max_12bit_constraint_flag[ i ] u(1) 
    sub_layer_max_10bit_constraint_flag[ i ] u(1) 
    sub_layer_max_8bit_constraint_flag[ i ] u(1) 
    sub_layer_max_422chroma_constraint_flag[ i ] u(1) 
    sub_layer_max_420chroma_constraint_flag[ i ] u(1) 
    sub_layer_max_monochrome_constraint_flag[ i ] u(1) 
    sub_layer_intra_constraint_flag[ i ] u(1) 
    sub_layer_one_picture_only_constraint_flag[ i ] u(1) 
    sub_layer_lower_bit_rate_constraint_flag[ i ] u(1)
       if( sub_layer_profile_idc[ i ] == 5 || 
     sub_layer_profile_compatibility_flag[ i ][ 5 ] ) { 
 
     sub_layer_max_14bit_constraint_flag[ i ] u(1) 
     sub_layer_reserved_zero_33bits[ i ] u(33) 
    } else  
     sub_layer_reserved_zero_34bits[ i ] u(34) 
   } else if( sub_layer_profile_idc[ i ] == 2  || 
       sub_layer_profile_compatibility_flag[ i ][ 2 ] ) { 
 
    sub_layer_reserved_zero_7bits[ i ] u(7) 
    sub_layer_one_picture_only_constraint_flag[ i ] u(1) 
    sub_layer_reserved_zero_35bits[ i ] u(35) 
   } else  
    sub_layer_reserved_zero_43bits[ i ] u(43) 
   if( ( sub_layer_profile_idc[ i ] >= 1  &&  sub_layer_profile_idc[ i ] <= 5 )  || 
     sub_layer_profile_idc[ i ] == 9  || 
     sub_layer_profile_compatibility_flag[ i ][ 1 ]  || 
     sub_layer_profile_compatibility_flag[ i ][ 2 ]  || 
     sub_layer_profile_compatibility_flag[ i ][ 3 ]  || 
     sub_layer_profile_compatibility_flag[ i ][ 4 ]  || 
     sub_layer_profile_compatibility_flag[ i ][ 5 ]  || 
     sub_layer_profile_compatibility_flag[ i ][ 9 ] ) 
    /* The number of bits in this syntax structure is not affected by this condition */ 
 
    sub_layer_inbld_flag[ i ] u(1) 
   else  
    sub_layer_reserved_zero_bit[ i ] u(1) 
  }  
  if( sub_layer_level_present_flag[ i ] )  
   sub_layer_level_idc[ i ] u(8) 
 }  
}  

scaling_list_data() { 
 for( sizeId = 0; sizeId < 4; sizeId++ )  
  for( matrixId = 0; matrixId < 6; matrixId  +=  ( sizeId == 3 ) ? 3 : 1 ) {  
   scaling_list_pred_mode_flag[ sizeId ][ matrixId ] u(1) 
   if( !scaling_list_pred_mode_flag[ sizeId ][ matrixId ] )  
    scaling_list_pred_matrix_id_delta[ sizeId ][ matrixId ] ue(v) 
   else {  
    nextCoef = 8  
    coefNum = Min( 64, ( 1  <<  ( 4 + ( sizeId  <<  1 ) ) ) )  
    if( sizeId > 1 ) {  
     scaling_list_dc_coef_minus8[ sizeId - 2 ][ matrixId ] se(v) 
     nextCoef = scaling_list_dc_coef_minus8[ sizeId - 2 ][ matrixId ] + 8  
    }  
    for( i = 0; i < coefNum; i++ ) {  
     scaling_list_delta_coef se(v) 
     nextCoef = ( nextCoef + scaling_list_delta_coef + 256 ) % 256  
     ScalingList[ sizeId ][ matrixId ][ i ] = nextCoef  
    }  
   }  
  }  
} 

sei_message() {  
 payloadType = 0  
 while( next_bits( 8 ) == 0xFF ) {  
  ff_byte  /* equal to 0xFF */ f(8) 
  payloadType  +=  255  
 }  
 last_payload_type_byte u(8) 
 payloadType  +=  last_payload_type_byte  
 payloadSize = 0  
 while( next_bits( 8 ) == 0xFF ) {  
  ff_byte  /* equal to 0xFF */ f(8) 
  payloadSize  +=  255  
 }  
 last_payload_size_byte u(8) 
 payloadSize  +=  last_payload_size_byte  
 sei_payload( payloadType, payloadSize )  
}  

ref_pic_lists_modification() { 
ref_pic_list_modification_flag_l0  u(1)
if( ref_pic_list_modification_flag_l0 )  
for( i = 0; i <= num_ref_idx_l0_active_minus1; i++ )  
list_entry_l0[ i ] u(v) 
if( slice_type == B ) {   
ref_pic_list_modification_flag_l1 u(1)
if( ref_pic_list_modification_flag_l1 )  
for( i = 0; i <= num_ref_idx_l1_active_minus1; i++ )  
list_entry_l1[ i ] u(v) 
}  
} 

pred_weight_table() {  
 luma_log2_weight_denom ue(v) 
 if( ChromaArrayType != 0 )  
  delta_chroma_log2_weight_denom se(v) 
 for( i = 0; i <= num_ref_idx_l0_active_minus1; i++ )  
  if( ( pic_layer_id( RefPicList0[ i ] ) != nuh_layer_id )  || 
   ( PicOrderCnt( RefPicList0[ i ] ) != PicOrderCnt( CurrPic ) ) ) 
 
   luma_weight_l0_flag[ i ] u(1) 
 if( ChromaArrayType != 0 )  
  for( i = 0; i <= num_ref_idx_l0_active_minus1; i++ )  
   if( ( pic_layer_id( RefPicList0[ i ] ) != nuh_layer_id )  || 
    ( PicOrderCnt(RefPicList0[ i ]) != PicOrderCnt( CurrPic ) ) ) 
 
    chroma_weight_l0_flag[ i ] u(1) 
 for( i = 0; i <= num_ref_idx_l0_active_minus1; i++ ) {  
  if( luma_weight_l0_flag[ i ] ) {  
   delta_luma_weight_l0[ i ] se(v) 
   luma_offset_l0[ i ] se(v) 
  }  
  if( chroma_weight_l0_flag[ i ] )  
   for( j = 0; j < 2; j++ ) {  
    delta_chroma_weight_l0[ i ][ j ] se(v) 
    delta_chroma_offset_l0[ i ][ j ] se(v) 
   }  
 }  
 if( slice_type == B ) {  
  for( i = 0; i <= num_ref_idx_l1_active_minus1; i++ )  
   if( ( pic_layer_id( RefPicList0[ i ] ) != nuh_layer_id )  || 
    ( PicOrderCnt(RefPicList1[ i ]) != PicOrderCnt( CurrPic ) ) ) 
 
    luma_weight_l1_flag[ i ] u(1) 
  if( ChromaArrayType != 0 )  
   for( i = 0; i <= num_ref_idx_l1_active_minus1; i++ )  
    if( ( pic_layer_id( RefPicList0[ i ] ) != nuh_layer_id )  || 
     ( PicOrderCnt(RefPicList1[ i ]) != PicOrderCnt( CurrPic ) ) ) 
 
     chroma_weight_l1_flag[ i ] u(1) 
  for( i = 0; i <= num_ref_idx_l1_active_minus1; i++ ) {  
   if( luma_weight_l1_flag[ i ] ) {  
    delta_luma_weight_l1[ i ] se(v) 
    luma_offset_l1[ i ] se(v) 
   }  
   if( chroma_weight_l1_flag[ i ] )  
    for( j = 0; j < 2; j++ ) {  
     delta_chroma_weight_l1[ i ][ j ] se(v) 
     delta_chroma_offset_l1[ i ][ j ] se(v) 
    }  
  }  
 }  
}  

st_ref_pic_set( stRpsIdx ) {  
 if( stRpsIdx != 0 )  
  inter_ref_pic_set_prediction_flag u(1) 
 if( inter_ref_pic_set_prediction_flag ) {  
  if( stRpsIdx == num_short_term_ref_pic_sets )  
   delta_idx_minus1 ue(v) 
  delta_rps_sign u(1) 
  abs_delta_rps_minus1 ue(v) 
  for( j = 0; j <= NumDeltaPocs[ RefRpsIdx ]; j++ ) {  
   used_by_curr_pic_flag[ j ] u(1) 
   if( !used_by_curr_pic_flag[ j ] )  
    use_delta_flag[ j ] u(1) 
  }  
 } else {  
  num_negative_pics ue(v) 
  num_positive_pics ue(v) 
  for( i = 0; i < num_negative_pics; i++ ) {  
   delta_poc_s0_minus1[ i ] ue(v) 
   used_by_curr_pic_s0_flag[ i ] u(1) 
  }  
  for( i = 0; i < num_positive_pics; i++ ) {  
   delta_poc_s1_minus1[ i ] ue(v) 
   used_by_curr_pic_s1_flag[ i ] u(1) 
  }  
 }  
}  

sei_payload( payloadType, payloadSize ) {  
 if( nal_unit_type == PREFIX_SEI_NUT )  
  if( payloadType == 0 )  
   buffering_period( payloadSize )  
  else if( payloadType == 1 )  
   pic_timing( payloadSize )  
  else if( payloadType == 2 )  
   pan_scan_rect( payloadSize )  
  else if( payloadType == 3 )  
   filler_payload( payloadSize )  
  else if( payloadType == 4 )  
   user_data_registered_itu_t_t35( payloadSize )  
  else if( payloadType == 5 )  
   user_data_unregistered( payloadSize )  
  else if( payloadType == 6 )  
   recovery_point( payloadSize )  
  else if( payloadType == 9 )  
   scene_info( payloadSize )  
  else if( payloadType == 15 )  
   picture_snapshot( payloadSize )  
  else if( payloadType == 16 )  
   progressive_refinement_segment_start( payloadSize )  
  else if( payloadType == 17 )  
   progressive_refinement_segment_end( payloadSize )  
  else if( payloadType == 19 )  
   film_grain_characteristics( payloadSize )  
  else if( payloadType == 22 )  
   post_filter_hint( payloadSize )  
  else if( payloadType == 23 )  
   tone_mapping_info( payloadSize )  
  else if( payloadType == 45 ) 
     frame_packing_arrangement( payloadSize )  
  else if( payloadType == 47 )  
   display_orientation( payloadSize )  
  else if( payloadType == 56 )  
   green_metadata( payloadSize ) /* specified in ISO/IEC 23001-11 */  
  else if( payloadType == 128 )  
   structure_of_pictures_info( payloadSize )  
  else if( payloadType == 129 )  
   active_parameter_sets( payloadSize )  
  else if( payloadType == 130 )  
   decoding_unit_info( payloadSize )  
  else if( payloadType == 131 )  
   temporal_sub_layer_zero_idx( payloadSize )  
  else if( payloadType == 133 )  
   scalable_nesting( payloadSize )  
  else if( payloadType == 134 )  
   region_refresh_info( payloadSize )  
  else if( payloadType == 135 )  
   no_display( payloadSize )  
  else if( payloadType == 136 )  
   time_code( payloadSize )  
  else if( payloadType == 137 )  
   mastering_display_colour_volume( payloadSize )  
  else if( payloadType == 138 )  
   segmented_rect_frame_packing_arrangement( payloadSize )  
  else if( payloadType == 139 )  
   temporal_motion_constrained_tile_sets( payloadSize )  
  else if( payloadType == 140 )  
   chroma_resampling_filter_hint( payloadSize )  
  else if( payloadType == 141 )  
   knee_function_info( payloadSize )  
  else if( payloadType == 142 )  
   colour_remapping_info( payloadSize )  
  else if( payloadType == 143 )  
   deinterlaced_field_identification( payloadSize )  
  else if( payloadType == 144 )  
   content_light_level_info( payloadSize )  
  else if( payloadType == 145 )  
   dependent_rap_indication( payloadSize )  
  else if( payloadType == 146 )  
   coded_region_completion( payloadSize )  
  else if( payloadType == 147 )  
   alternative_transfer_characteristics( payloadSize )  
  else if( payloadType == 148 )  
   ambient_viewing_environment( payloadSize )  
  else if( payloadType == 149 )  
   content_colour_volume( payloadSize )  
  else if( payloadType == 150 )  
   equirectangular_projection( payloadSize )  
  else if( payloadType == 151 )  
   cubemap_projection( payloadSize )  
  else if( payloadType == 154 )  
   sphere_rotation( payloadSize ) 
     else if( payloadType == 155 )  
   regionwise_packing( payloadSize )  
  else if( payloadType == 156 )  
   omni_viewport( payloadSize )  
  else if( payloadType == 157 )  
   regional_nesting( payloadSize )  
  else if( payloadType == 158 )  
   mcts_extraction_info_sets( payloadSize )  
  else if( payloadType == 159 )  
   mcts_extraction_info_nesting( payloadSize )  
  else if( payloadType == 160 )  
   layers_not_present( payloadSize )  /* specified in Annex F */  
  else if( payloadType == 161 )  
   inter_layer_constrained_tile_sets( payloadSize )  /* specified in Annex F */  
  else if( payloadType == 162 )  
   bsp_nesting( payloadSize )  /* specified in Annex F */  
  else if( payloadType == 163 )  
   bsp_initial_arrival_time( payloadSize )  /* specified in Annex F */  
  else if( payloadType == 164 )  
   sub_bitstream_property( payloadSize )  /* specified in Annex F */  
  else if( payloadType == 165 )  
   alpha_channel_info( payloadSize )  /* specified in Annex F */  
  else if( payloadType == 166 )  
   overlay_info( payloadSize )  /* specified in Annex F */  
  else if( payloadType == 167 )  
   temporal_mv_prediction_constraints( payloadSize )  /* specified in Annex F */  
  else if( payloadType == 168 )  
   frame_field_info( payloadSize )  /* specified in Annex F */  
  else if( payloadType == 176 )  
   three_dimensional_reference_displays_info( payloadSize )  /* specified in Annex G */  
  else if( payloadType == 177 )  
   depth_representation_info( payloadSize )  /* specified in Annex G */  
  else if( payloadType == 178 )  
   multiview_scene_info( payloadSize )  /* specified in Annex G */  
  else if( payloadType == 179 )  
   multiview_acquisition_info( payloadSize )  /* specified in Annex G */  
  else if( payloadType == 180 )  
   multiview_view_position( payloadSize )  /* specified in Annex G */  
  else if( payloadType == 181 )  
   alternative_depth_info( payloadSize )  /* specified in Annex I */  
  else  
   reserved_sei_message( payloadSize )  
 else /* nal_unit_type == SUFFIX_SEI_NUT */  
  if( payloadType == 3 )  
   filler_payload( payloadSize )  
  else if( payloadType == 4 )  
   user_data_registered_itu_t_t35( payloadSize )  
  else if( payloadType == 5 )  
   user_data_unregistered( payloadSize )  
  else if( payloadType == 17 )  
   progressive_refinement_segment_end( payloadSize )  
  else if( payloadType == 22 )  
   post_filter_hint( payloadSize ) 
     else if( payloadType == 132 )  
   decoded_picture_hash( payloadSize )  
  else if( payloadType == 146 )  
   coded_region_completion( payloadSize )  
  else  
   reserved_sei_message( payloadSize )  
 if( more_data_in_payload() ) {  
  if( payload_extension_present() )  
   reserved_payload_extension_data u(v) 
  payload_bit_equal_to_one /* equal to 1 */ f(1) 
  while( !byte_aligned() )  
   payload_bit_equal_to_zero /* equal to 0 */ f(1) 
 }  
} 

buffering_period( payloadSize ) {  
 bp_seq_parameter_set_id ue(v) 
 if( !sub_pic_hrd_params_present_flag )  
  irap_cpb_params_present_flag u(1) 
 if( irap_cpb_params_present_flag ) {  
  cpb_delay_offset u(v) 
  dpb_delay_offset u(v) 
 }  
 concatenation_flag u(1) 
 au_cpb_removal_delay_delta_minus1 u(v) 
 if( NalHrdBpPresentFlag ) {  
  for( i = 0; i < CpbCnt; i++ ) {  
   nal_initial_cpb_removal_delay[ i ] u(v) 
   nal_initial_cpb_removal_offset[ i ] u(v) 
   if( sub_pic_hrd_params_present_flag  ||  irap_cpb_params_present_flag ) {  
    nal_initial_alt_cpb_removal_delay[ i ] u(v) 
    nal_initial_alt_cpb_removal_offset[ i ] u(v) 
   }  
  }  
 }  
 if( VclHrdBpPresentFlag ) {  
  for( i = 0; i < CpbCnt; i++ ) {  
   vcl_initial_cpb_removal_delay[ i ] u(v) 
   vcl_initial_cpb_removal_offset[ i ] u(v) 
   if( sub_pic_hrd_params_present_flag  ||  irap_cpb_params_present_flag ) {  
    vcl_initial_alt_cpb_removal_delay[ i ] u(v) 
    vcl_initial_alt_cpb_removal_offset[ i ] u(v) 
   }  
  }  
 }  
 if( payload_extension_present() )  
  use_alt_cpb_params_flag u(1) 
}  

pic_timing( payloadSize ) {  
 if( frame_field_info_present_flag ) {  
  pic_struct u(4) 
  source_scan_type u(2) 
  duplicate_flag u(1) 
 }  
 if( CpbDpbDelaysPresentFlag ) {  
  au_cpb_removal_delay_minus1 u(v) 
  pic_dpb_output_delay u(v) 
  if( sub_pic_hrd_params_present_flag )  
   pic_dpb_output_du_delay u(v) 
  if( sub_pic_hrd_params_present_flag  && 
    sub_pic_cpb_params_in_pic_timing_sei_flag ) { 
 
   num_decoding_units_minus1 ue(v) 
   du_common_cpb_removal_delay_flag u(1) 
   if( du_common_cpb_removal_delay_flag )  
    du_common_cpb_removal_delay_increment_minus1 u(v) 
   for( i = 0; i <= num_decoding_units_minus1; i++ ) {  
    num_nalus_in_du_minus1[ i ] ue(v) 
    if( !du_common_cpb_removal_delay_flag  &&  i < num_decoding_units_minus1 )  
     du_cpb_removal_delay_increment_minus1[ i ] u(v) 
   }  
  }  
 }  
} 

pan_scan_rect( payloadSize ) {  
 pan_scan_rect_id ue(v) 
 pan_scan_rect_cancel_flag u(1) 
 if( !pan_scan_rect_cancel_flag ) {  
  pan_scan_cnt_minus1 ue(v) 
  for( i = 0; i <= pan_scan_cnt_minus1; i++ ) {  
   pan_scan_rect_left_offset[ i ] se(v) 
   pan_scan_rect_right_offset[ i ] se(v) 
   pan_scan_rect_top_offset[ i ] se(v) 
   pan_scan_rect_bottom_offset[ i ] se(v) 
  }  
  pan_scan_rect_persistence_flag u(1) 
 }  
} 

filler_payload( payloadSize ) {  
 for( k = 0; k < payloadSize; k++)  
  ff_byte  /* equal to 0xFF */ f(8) 
} 

user_data_registered_itu_t_t35( payloadSize ) {  
 itu_t_t35_country_code b(8) 
 if( itu_t_t35_country_code != 0xFF )  
  i = 1  
 else {  
  itu_t_t35_country_code_extension_byte b(8) 
  i = 2  
 }  
 do {  
  itu_t_t35_payload_byte b(8) 
  i++  
 } while( i < payloadSize )  
} 

user_data_unregistered( payloadSize ) {  
 uuid_iso_iec_11578 u(128) 
 for( i = 16; i < payloadSize; i++ )  
  user_data_payload_byte b(8) 
} 

recovery_point( payloadSize ) {  
 recovery_poc_cnt se(v) 
 exact_match_flag u(1) 
 broken_link_flag u(1) 
} 

scene_info( payloadSize ) { 
scene_info_present_flag  u(1)
if( scene_info_present_flag ) {  
prev_scene_id_valid_flag u(1)
scene_id ue(v) 
scene_transition_type ue(v)
if( scene_transition_type > 3 ) 
  second_scene_id ue(v) 
}  
} 

picture_snapshot( payloadSize ) { 
 snapshot_id ue(v) 
} 

progressive_refinement_segment_start( payloadSize ) { 
  progressive_refinement_id ue(v) 
  pic_order_cnt_delta ue(v) 
} 

progressive_refinement_segment_end( payloadSize ) { 
  progressive_refinement_id ue(v) 
} 

film_grain_characteristics( payloadSize ) {  
 film_grain_characteristics_cancel_flag u(1) 
 if( !film_grain_characteristics_cancel_flag ) {  
  film_grain_model_id u(2) 
  separate_colour_description_present_flag u(1) 
  if( separate_colour_description_present_flag ) {  
   film_grain_bit_depth_luma_minus8 u(3) 
   film_grain_bit_depth_chroma_minus8 u(3) 
   film_grain_full_range_flag u(1) 
   film_grain_colour_primaries u(8) 
   film_grain_transfer_characteristics u(8) 
   film_grain_matrix_coeffs u(8) 
  }  
  blending_mode_id u(2) 
  log2_scale_factor u(4) 
  for( c = 0; c < 3; c++ )  
   comp_model_present_flag[ c ] u(1) 
  for( c = 0; c < 3; c++ )  
   if( comp_model_present_flag[ c ] ) {  
    num_intensity_intervals_minus1[ c ] u(8) 
    num_model_values_minus1[ c ] u(3) 
    for( i = 0; i <= num_intensity_intervals_minus1[ c ]; i++ ) {  
     intensity_interval_lower_bound[ c ][ i ] u(8) 
     intensity_interval_upper_bound[ c ][ i ] u(8) 
     for( j = 0; j <= num_model_values_minus1[ c ]; j++ )  
      comp_model_value[ c ][ i ][ j ] se(v) 
    }  
   }  
 film_grain_characteristics_persistence_flag u(1) 
 }  
}  

post_filter_hint( payloadSize ) {  
 filter_hint_size_y ue(v) 
 filter_hint_size_x ue(v) 
 filter_hint_type u(2) 
 for( cIdx = 0; cIdx < ( chroma_format_idc == 0 ? 1 : 3 ); cIdx++ )  
  for( cy = 0; cy < filter_hint_size_y; cy++ )  
   for( cx = 0; cx < filter_hint_size_x; cx++ )  
    filter_hint_value[ cIdx ][ cy ][ cx ] se(v) 
}  

tone_mapping_info( payloadSize ) {  
 tone_map_id ue(v) 
 tone_map_cancel_flag u(1) 
 if( !tone_map_cancel_flag ) {  
  tone_map_persistence_flag u(1) 
  coded_data_bit_depth u(8) 
  target_bit_depth u(8) 
  tone_map_model_id ue(v) 
  if( tone_map_model_id == 0 ) {  
   min_value u(32) 
   max_value u(32) 
  } else if( tone_map_model_id == 1 ) {  
   sigmoid_midpoint u(32) 
   sigmoid_width u(32) 
  } else if( tone_map_model_id == 2 )  
   for( i = 0; i < ( 1  <<  target_bit_depth ); i++ )  
    start_of_coded_interval[ i ] u(v) 
  else if( tone_map_model_id == 3 ) {  
   num_pivots u(16) 
   for( i = 0; i < num_pivots; i++ ) {  
    coded_pivot_value[ i ] u(v) 
    target_pivot_value[ i ] u(v) 
   }  
  } else if( tone_map_model_id == 4 ) {  
   camera_iso_speed_idc u(8) 
   if( camera_iso_speed_idc == EXTENDED_ISO )  
    camera_iso_speed_value u(32) 
   exposure_idx_idc u(8) 
   if( exposure_idx_idc == EXTENDED_ISO )  
    exposure_idx_value u(32) 
   exposure_compensation_value_sign_flag u(1) 
   exposure_compensation_value_numerator u(16) 
   exposure_compensation_value_denom_idc u(16) 
   ref_screen_luminance_white u(32) 
   extended_range_white_level u(32) 
   nominal_black_level_code_value u(16) 
   nominal_white_level_code_value u(16) 
   extended_white_level_code_value u(16) 
  }  
 }  
}  

frame_packing_arrangement( payloadSize ) {  
 frame_packing_arrangement_id ue(v) 
 frame_packing_arrangement_cancel_flag u(1) 
 if( !frame_packing_arrangement_cancel_flag ) {  
  frame_packing_arrangement_type u(7) 
  quincunx_sampling_flag u(1) 
  content_interpretation_type u(6) 
  spatial_flipping_flag u(1) 
  frame0_flipped_flag u(1) 
  field_views_flag u(1) 
  current_frame_is_frame0_flag u(1) 
  frame0_self_contained_flag u(1) 
  frame1_self_contained_flag u(1) 
  if( !quincunx_sampling_flag  &&  frame_packing_arrangement_type != 5 ) {  
   frame0_grid_position_x u(4) 
   frame0_grid_position_y u(4) 
   frame1_grid_position_x u(4) 
   frame1_grid_position_y u(4) 
  }  
  frame_packing_arrangement_reserved_byte u(8) 
  frame_packing_arrangement_persistence_flag u(1) 
 }  
 upsampled_aspect_ratio_flag u(1) 
}  

display_orientation( payloadSize ) {  
 display_orientation_cancel_flag u(1) 
 if( !display_orientation_cancel_flag ) {  
  hor_flip u(1) 
  ver_flip u(1) 
  anticlockwise_rotation u(16) 
  display_orientation_persistence_flag u(1) 
 }  
} 

structure_of_pictures_info( payloadSize ) {  
 sop_seq_parameter_set_id ue(v) 
 num_entries_in_sop_minus1 ue(v) 
 for( i = 0; i <= num_entries_in_sop_minus1; i++ ) {  
  sop_vcl_nut[ i ] u(6) 
  sop_temporal_id[ i ] u(3) 
  if( sop_vcl_nut[ i ] != IDR_W_RADL  &&  sop_vcl_nut[ i ] != IDR_N_LP )  
   sop_short_term_rps_idx[ i ] ue(v) 
  if( i > 0 )  
   sop_poc_delta[ i ] se(v) 
 }  
} 

decoded_picture_hash( payloadSize ) {  
 hash_type u(8) 
 for( cIdx = 0; cIdx < ( chroma_format_idc == 0 ? 1 : 3 ); cIdx++ )  
  if( hash_type == 0 )  
   for( i = 0; i < 16; i++)  
    picture_md5[ cIdx ][ i ] b(8) 
  else if( hash_type == 1 )  
   picture_crc[ cIdx ] u(16) 
  else if( hash_type == 2 )  
   picture_checksum[ cIdx ] u(32) 
} 

active_parameter_sets( payloadSize ) {  
 active_video_parameter_set_id u(4) 
 self_contained_cvs_flag u(1) 
 no_parameter_set_update_flag u(1) 
 num_sps_ids_minus1 ue(v) 
 for( i = 0; i <= num_sps_ids_minus1; i++ )  
  active_seq_parameter_set_id[ i ] ue(v) 
 for( i = vps_base_layer_internal_flag; i <= MaxLayersMinus1; i++ )  
  layer_sps_idx[ i ] ue(v) 
} 

decoding_unit_info( payloadSize ) {  
 decoding_unit_idx ue(v) 
 if( !sub_pic_cpb_params_in_pic_timing_sei_flag )  
  du_spt_cpb_removal_delay_increment u(v) 
 dpb_output_du_delay_present_flag u(1) 
 if( dpb_output_du_delay_present_flag )  
  pic_spt_dpb_output_du_delay u(v) 
}  

temporal_sub_layer_zero_idx( payloadSize ) {  
 temporal_sub_layer_zero_idx u(8) 
 irap_pic_id u(8) 
}  

scalable_nesting( payloadSize ) {  
 bitstream_subset_flag u(1) 
 nesting_op_flag u(1) 
 if( nesting_op_flag ) {  
  default_op_flag u(1) 
  nesting_num_ops_minus1 ue(v) 
  for( i = default_op_flag; i <= nesting_num_ops_minus1; i++ ) {  
   nesting_max_temporal_id_plus1[ i ] u(3) 
   nesting_op_idx[ i ] ue(v) 
  }  
 } else {  
  all_layers_flag u(1) 
  if( !all_layers_flag ) {  
   nesting_no_op_max_temporal_id_plus1 u(3) 
   nesting_num_layers_minus1 ue(v) 
   for( i = 0; i <= nesting_num_layers_minus1; i++ )  
    nesting_layer_id[ i ] u(6) 
  }  
 }  
 while( !byte_aligned() )  
  nesting_zero_bit /* equal to 0 */ u(1) 
 do  
  sei_message()  
 while( more_rbsp_data() )  
} 

region_refresh_info( payloadSize ) {  
 refreshed_region_flag u(1) 
}

no_display( payloadSize ) {  
}

time_code( payloadSize ) {  
 num_clock_ts u(2) 
 for( i = 0; i < num_clock_ts; i++ ) {  
  clock_timestamp_flag[ i ] u(1) 
  if( clock_timestamp_flag[ i ] ) {  
   units_field_based_flag[ i ] u(1) 
   counting_type[ i ] u(5) 
   full_timestamp_flag[ i ] u(1) 
   discontinuity_flag[ i ] u(1) 
   cnt_dropped_flag[ i ] u(1) 
   n_frames[ i ] u(9) 
   if( full_timestamp_flag[ i ] ) {  
    seconds_value[ i ] u(6) /* 0..59 */
    minutes_value[ i ] u(6) /* 0..59 */
    hours_value[ i ] u(5) /* 0..23 */
   } else {  
    seconds_flag[ i ] u(1) 
    if( seconds_flag[ i ] ) {  
     seconds_value[ i ] u(6) /* 0..59 */
     minutes_flag[ i ] u(1) 
     if( minutes_flag[ i ] ) {  
      minutes_value[ i ]  u(6) /* 0..59 */
      hours_flag[ i ] u(1) 
      if( hours_flag[ i ] )  
       hours_value[ i ]  u(5) /* 0..23 */
     }  
    }  
   }  
   time_offset_length[ i ] u(5) 
   if( time_offset_length[ i ] > 0 )  
    time_offset_value[ i ] i(v) 
  }  
 }  
}  

mastering_display_colour_volume( payloadSize ) { 
for( c = 0; c < 3; c++ ) {  
display_primaries_x[ c ] u(16) 
display_primaries_y[ c ] u(16) 
}  
white_point_x u(16) 
white_point_y u(16) 
max_display_mastering_luminance u(32) 
min_display_mastering_luminance u(32) 
}

segmented_rect_frame_packing_arrangement( payloadSize ) { 
segmented_rect_frame_packing_arrangement_cancel_flag  u(1) 
if( !segmented_rect_frame_packing_arrangement_cancel_flag ) {  
segmented_rect_content_interpretation_type u(2) 
segmented_rect_frame_packing_arrangement_persistence_flag u(1) 
}  
}

temporal_motion_constrained_tile_sets( payloadSize ) {  
 mc_all_tiles_exact_sample_value_match_flag u(1) 
 each_tile_one_tile_set_flag u(1) 
 if( !each_tile_one_tile_set_flag ) {  
  limited_tile_set_display_flag u(1) 
  num_sets_in_message_minus1 ue(v) 
  for( i = 0; i <= num_sets_in_message_minus1; i++ ) {  
   mcts_id[ i ] ue(v) 
   if( limited_tile_set_display_flag )  
    display_tile_set_flag[ i ] u(1) 
   num_tile_rects_in_set_minus1[ i ] ue(v) 
   for( j = 0; j <= num_tile_rects_in_set_minus1[ i ]; j++ ) {  
    top_left_tile_idx[ i ][ j ] ue(v) 
    bottom_right_tile_idx[ i ][ j ] ue(v) 
   }  
   if( !mc_all_tiles_exact_sample_value_match_flag )  
    mc_exact_sample_value_match_flag[ i ] u(1) 
   mcts_tier_level_idc_present_flag[ i ] u(1) 
   if( mcts_tier_level_idc_present_flag[ i ] ) {  
    mcts_tier_flag[ i ] u(1) 
    mcts_level_idc[ i ] u(8) 
   }  
  }  
 } else {  
  max_mcs_tier_level_idc_present_flag u(1) 
     if (max_mcs_tier_level_idc_present_flag ) {  
   mcts_max_tier_flag u(1) 
   mcts_max_level_idc u(8) 
  }  
 }  
}  

chroma_resampling_filter_hint( payloadSize ) {  
 ver_chroma_filter_idc u(8) 
 hor_chroma_filter_idc u(8) 
 ver_filtering_field_processing_flag u(1) 
 if( ver_chroma_filter_idc == 1  ||  hor_chroma_filter_idc == 1 ) {  
  target_format_idc ue(v) 
  if( ver_chroma_filter_idc == 1 ) {  
   num_vertical_filters ue(v) 
   for( i = 0; i < num_vertical_filters; i++ ) {  
    ver_tap_length_minus1[ i ] ue(v) 
    for( j = 0; j <= ver_tap_length_minus1[ i ]; j++ )  
     ver_filter_coeff[ i ][ j ] se(v) 
   }  
  }  
  if( hor_chroma_filter_idc == 1 ) {  
   num_horizontal_filters ue(v) 
   for( i = 0; i < num_horizontal_filters; i++ ) {  
    hor_tap_length_minus1[ i ] ue(v) 
    for( j = 0; j <= hor_tap_length_minus1[ i ]; j++ )  
     hor_filter_coeff[ i ][ j ] se(v) 
   }  
  }  
 }  
}  

knee_function_info( payloadSize ) {  
 knee_function_id ue(v) 
 knee_function_cancel_flag u(1) 
 if( !knee_function_cancel_flag ) {  
  knee_function_persistence_flag u(1) 
  input_d_range u(32) 
  input_disp_luminance u(32) 
  output_d_range u(32) 
  output_disp_luminance u(32) 
  num_knee_points_minus1 ue(v) 
  for( i = 0; i <= num_knee_points_minus1; i++ ) {  
   input_knee_point[ i ] u(10) 
   output_knee_point[ i ] u(10) 
  }  
 }  
} 

colour_remapping_info( payloadSize ) {  
 colour_remap_id ue(v) 
 colour_remap_cancel_flag u(1) 
 if( !colour_remap_cancel_flag ) {  
  colour_remap_persistence_flag u(1) 
  colour_remap_video_signal_info_present_flag u(1) 
  if( colour_remap_video_signal_info_present_flag ) {  
   colour_remap_full_range_flag u(1) 
   colour_remap_primaries u(8) 
   colour_remap_transfer_function u(8) 
   colour_remap_matrix_coefficients u(8) 
  }  
  colour_remap_input_bit_depth u(8) 
  colour_remap_output_bit_depth u(8) 
  for( c = 0; c < 3; c++ ) {  
   pre_lut_num_val_minus1[ c ] u(8) 
   if( pre_lut_num_val_minus1[ c ] > 0 )  
    for( i = 0; i <= pre_lut_num_val_minus1[ c ]; i++ ) {  
     pre_lut_coded_value[ c ][ i ] u(v) 
     pre_lut_target_value[ c ][ i ] u(v) 
    }  
  }  
  colour_remap_matrix_present_flag u(1) 
  if( colour_remap_matrix_present_flag ) {  
   log2_matrix_denom u(4) 
   for( c = 0; c < 3; c++ )  
    for( i = 0; i < 3; i++ )  
     colour_remap_coeffs[ c ][ i ] se(v) 
  }  
  for( c = 0; c < 3; c++ ) {  
   post_lut_num_val_minus1[ c ] u(8) 
   if( post_lut_num_val_minus1[ c ] > 0 )  
    for( i = 0; i <= post_lut_num_val_minus1[ c ]; i++ ) {  
     post_lut_coded_value[ c ][ i ] u(v) 
     post_lut_target_value[ c ][ i ] u(v) 
    }  
  }  
 }  
} 

deinterlaced_field_identification( payloadSize ) { 
deinterlaced_picture_source_parity_flag  u(1) 
}

content_light_level_info( payloadSize ) { 
max_content_light_level  u(16) 
max_pic_average_light_level u(16) 
}

dependent_rap_indication( payloadSize ) { 
}

coded_region_completion( payloadSize ) { 
next_segment_address ue(v) 
if( next_segment_address > 0 )  
independent_slice_segment_flag u(1) 
}

alternative_transfer_characteristics ( payloadSize ) { 
preferred_transfer_characteristics u(8) 
}

ambient_viewing_environment( payloadSize ) { 
ambient_illuminance  u(32) 
ambient_light_x u(16) 
ambient_light_y u(16) 
}

content_colour_volume( payloadSize ) {  
 ccv_cancel_flag u(1) 
 if( !ccv_cancel_flag ) {  
  ccv_persistence_flag u(1) 
  ccv_primaries_present_flag u(1) 
  ccv_min_luminance_value_present_flag u(1) 
  ccv_max_luminance_value_present_flag u(1) 
  ccv_avg_luminance_value_present_flag u(1) 
  ccv_reserved_zero_2bits u(2) 
  if( ccv_primaries_present_flag )  
   for( c = 0; c < 3; c++ ) {  
    ccv_primaries_x[ c ] i(32) 
    ccv_primaries_y[ c ] i(32) 
   }  
  if( ccv_min_luminance_value_present_flag )  
   ccv_min_luminance_value u(32) 
  if( ccv_max_luminance_value_present_flag )  
   ccv_max_luminance_value u(32) 
  if( ccv_avg_luminance_value_present_flag )  
   ccv_avg_luminance_value u(32) 
 }  
} 

equirectangular_projection( payloadSize ) {  
 erp_cancel_flag u(1) 
 if( !erp_cancel_flag ) {  
  erp_persistence_flag u(1) 
  erp_guard_band_flag u(1) 
  erp_reserved_zero_2bits u(2) 
  if( erp_guard_band_flag == 1 ) {  
   erp_guard_band_type u(3) 
   erp_left_guard_band_width u(8) 
   erp_right_guard_band_width u(8) 
  }  
 }  
} 

cubemap_projection( payloadSize ) { 
cmp_cancel_flag u(1) 
if( !cmp_cancel_flag )  
cmp_persistence_flag u(1) 
}  

sphere_rotation( payloadSize ) { 
sphere_rotation_cancel_flag  u(1) 
if( !sphere_rotation_cancel_flag ) {  
sphere_rotation_persistence_flag u(1) 
sphere_rotation_reserved_zero_6bits u(6) 
yaw_rotation i(32) 
pitch_rotation i(32) 
roll_rotation i(32) 
}  
} 

regionwise_packing( payloadSize ) {  
 rwp_cancel_flag u(1) 
 if( !rwp_cancel_flag ) {  
  rwp_persistence_flag u(1) 
  constituent_picture_matching_flag u(1) 
  rwp_reserved_zero_5bits u(5) 
  num_packed_regions u(8) 
  proj_picture_width u(32) 
  proj_picture_height u(32) 
  packed_picture_width u(16) 
  packed_picture_height u(16) 
  for( i = 0; i < num_packed_regions; i++ ) {  
   rwp_reserved_zero_4bits[ i ] u(4) 
   rwp_transform_type[ i ] u(3) 
   rwp_guard_band_flag[ i ] u(1) 
   proj_region_width[ i ] u(32) 
   proj_region_height[ i ] u(32) 
   proj_region_top[ i ] u(32) 
   proj_region_left[ i ] u(32) 
   packed_region_width[ i ] u(16) 
   packed_region_height[ i ] u(16) 
   packed_region_top[ i ] u(16) 
   packed_region_left[ i ] u(16) 
   if( rwp_guard_band_flag[ i ] ) {  
    rwp_left_guard_band_width[ i ] u(8) 
    rwp_right_guard_band_width[ i ] u(8) 
    rwp_top_guard_band_height[ i ] u(8) 
    rwp_bottom_guard_band_height[ i ] u(8) 
    rwp_guard_band_not_used_for_pred_flag[ i ] u(1) 
    for( j = 0; j < 4; j++ )  
     rwp_guard_band_type[ i ][ j ] u(3) 
    rwp_guard_band_reserved_zero_3bits[ i ] u(3) 
   }  
  }  
 }  
} 

omni_viewport( payloadSize ) {  
 omni_viewport_id u(10) 
 omni_viewport_cancel_flag u(1) 
 if( !omni_viewport_cancel_flag ) {  
  omni_viewport_persistence_flag u(1) 
  omni_viewport_cnt_minus1 u(4) 
  for( i = 0; i <= omni_viewport_cnt_minus1; i++ ) {  
   omni_viewport_azimuth_centre[ i ] i(32) 
   omni_viewport_elevation_centre[ i ] i(32) 
   omni_viewport_tilt_centre[ i ] i(32) 
   omni_viewport_hor_range[ i ] u(32) 
   omni_viewport_ver_range[ i ] u(32) 
  }  
 }  
} 

regional_nesting( payloadSize ) {  
 regional_nesting_id u(16) 
 regional_nesting_num_rect_regions u(8) 
 for( i = 0; i < regional_nesting_num_rect_regions; i++ ) {  
  regional_nesting_rect_region_id[ i ] u(8) 
  regional_nesting_rect_left_offset[ i ] u(16) 
  regional_nesting_rect_right_offset[ i ] u(16) 
  regional_nesting_rect_top_offset[ i ] u(16) 
  regional_nesting_rect_bottom_offset[ i ] u(16) 
 }  
 num_sei_messages_in_regional_nesting_minus1 u(8) 
 for( i = 0; i <= num_sei_messages_in_regional_nesting_minus1; i++ ) {  
  num_regions_for_sei_message[ i ] u(8) 
  for(j = 0; j < num_regions_for_sei_message[ i ]; j++ )  
   regional_nesting_sei_region_idx[ i ][ j ] u(8) 
  sei_message()  
 }  
} 

mcts_extraction_info_sets( payloadSize ) {  
 num_info_sets_minus1 ue(v) 
 for( i = 0; i <= num_info_sets_minus1; i++ ) {  
  num_mcts_sets_minus1[ i ] ue(v) 
  for( j = 0; j <= num_mcts_sets_minus1[ i ]; j++ ) {  
   num_mcts_in_set_minus1[ i ][ j ] ue(v) 
   for( k = 0; k <= num_mcts_in_set_minus1[ i ][ j ]; k++ )  
    idx_of_mcts_in_set[ i ][ j ][ k ] ue(v) 
  }  
  slice_reordering_enabled_flag[ i ] u(1) 
  if( slice_reordering_enabled_flag[ i ] ) {  
   num_slice_segments_minus1[ i ] ue(v) 
   for( j = 0; j <= num_slice_segments_minus1[ i ]; j++ )  
    output_slice_segment_address[ i ][ j ] u(v) 
  }  
  num_vps_in_info_set_minus1[ i ] ue(v) 
  for( j = 0; j <= num_vps_in_info_set_minus1[ i ]; j++ )  
   vps_rbsp_data_length[ i ][ j ] ue(v) 
  num_sps_in_info_set_minus1[ i ] ue(v) 
  for( j = 0; j <= num_sps_in_info_set_minus1[ i ]; j++ )  
   sps_rbsp_data_length[ i ][ j ] ue(v) 
  num_pps_in_info_set_minus1[ i ] ue(v) 
  for( j = 0; j <= num_pps_in_info_set_minus1[ i ]; j++ ) {  
   pps_nuh_temporal_id_plus1[ i ][ j ] u(3) 
   pps_rbsp_data_length[ i ][ j ] ue(v) 
  }  
  while( !byte_aligned() )  
   mcts_alignment_bit_equal_to_zero f(1) 
  for( j = 0; j <= num_vps_in_info_set_minus1[ i ]; j++ )  
   for( k = 0; k <= vps_rbsp_data_length[ i ][ j ]; k++ )  
    vps_rbsp_data_byte[ i ][ j ][ k ] u(8) 
  for( j = 0; j <= num_sps_in_info_set_minus1[ i ]; j++ )  
   for( k = 0; k <= sps_rbsp_data_length[ i ][ j ]; k++ )  
    sps_rbsp_data_byte[ i ][ j ][ k ] u(8) 
  for( j = 0; j <= num_pps_in_info_set_minus1[ i ]; j++ )  
   for( k = 0; k <= pps_rbsp_data_length[ i ][ j ]; k++ )  
    pps_rbsp_data_byte[ i ][ j ][ k ] u(8) 
 }  
}  

mcts_extraction_info_nesting( payloadSize ) { 
all_mcts_flag  u(1) 
if( !all_mcts_flag ) {  
num_associated_mcts_minus1 ue(v) 
for( i = 0; i <= num_associated_mcts_minus1; i++ )  
idx_of_associated_mcts[ i ] ue(v) 
}  
num_sei_messages_in_mcts_extraction_nesting_minus1 ue(v) 
while( !byte_aligned() )  
mcts_nesting_zero_bit /* equal to 0 */ u(1)
for( i = 0; i <= num_sei_messages_in_mcts_extraction_nesting_minus1; i++ )  
sei_message()  
}  

reserved_sei_message( payloadSize ) { 
for( i = 0; i < payloadSize; i++ )  
reserved_sei_message_payload_byte b(8) 
}

vui_parameters() {  
 aspect_ratio_info_present_flag u(1) 
 if( aspect_ratio_info_present_flag ) {  
  aspect_ratio_idc u(8) 
  if( aspect_ratio_idc == EXTENDED_SAR ) {  
   sar_width u(16) 
   sar_height u(16) 
  }  
 }  
 overscan_info_present_flag u(1) 
 if( overscan_info_present_flag )  
  overscan_appropriate_flag u(1) 
 video_signal_type_present_flag u(1) 
 if( video_signal_type_present_flag ) {  
  video_format u(3) 
  video_full_range_flag u(1) 
  colour_description_present_flag u(1) 
  if( colour_description_present_flag ) {  
   colour_primaries u(8) 
   transfer_characteristics u(8) 
   matrix_coeffs u(8) 
  }  
 }  
 chroma_loc_info_present_flag u(1) 
 if( chroma_loc_info_present_flag ) {  
  chroma_sample_loc_type_top_field ue(v) 
  chroma_sample_loc_type_bottom_field ue(v)
   }  
 neutral_chroma_indication_flag u(1) 
 field_seq_flag u(1) 
 frame_field_info_present_flag u(1) 
 default_display_window_flag u(1) 
 if( default_display_window_flag ) {  
  def_disp_win_left_offset ue(v) 
  def_disp_win_right_offset ue(v) 
  def_disp_win_top_offset ue(v) 
  def_disp_win_bottom_offset ue(v) 
 }  
 vui_timing_info_present_flag u(1) 
 if( vui_timing_info_present_flag ) {  
  vui_num_units_in_tick u(32) 
  vui_time_scale u(32) 
  vui_poc_proportional_to_timing_flag u(1) 
  if( vui_poc_proportional_to_timing_flag )  
   vui_num_ticks_poc_diff_one_minus1 ue(v) 
  vui_hrd_parameters_present_flag u(1) 
  if( vui_hrd_parameters_present_flag )  
   hrd_parameters( 1, sps_max_sub_layers_minus1 )  
 }  
 bitstream_restriction_flag u(1) 
 if( bitstream_restriction_flag ) {  
  tiles_fixed_structure_flag u(1) 
  motion_vectors_over_pic_boundaries_flag u(1) 
  restricted_ref_pic_lists_flag u(1) 
  min_spatial_segmentation_idc ue(v) 
  max_bytes_per_pic_denom ue(v) 
  max_bits_per_min_cu_denom ue(v) 
  log2_max_mv_length_horizontal ue(v) 
  log2_max_mv_length_vertical ue(v) 
 }  
} 

hrd_parameters( commonInfPresentFlag, maxNumSubLayersMinus1 ) {  
 if( commonInfPresentFlag ) {  
  nal_hrd_parameters_present_flag u(1) 
  vcl_hrd_parameters_present_flag u(1) 
  if( nal_hrd_parameters_present_flag  ||  vcl_hrd_parameters_present_flag ) {  
   sub_pic_hrd_params_present_flag u(1) 
   if( sub_pic_hrd_params_present_flag ) {  
    tick_divisor_minus2 u(8) 
    du_cpb_removal_delay_increment_length_minus1 u(5) 
    sub_pic_cpb_params_in_pic_timing_sei_flag u(1) 
    dpb_output_delay_du_length_minus1 u(5) 
   }  
   bit_rate_scale u(4) 
   cpb_size_scale u(4) 
   if( sub_pic_hrd_params_present_flag )  
    cpb_size_du_scale u(4) 
   initial_cpb_removal_delay_length_minus1 u(5) 
   au_cpb_removal_delay_length_minus1 u(5) 
   dpb_output_delay_length_minus1 u(5) 
  }  
 }  
 for( i = 0; i <= maxNumSubLayersMinus1; i++ ) {  
  fixed_pic_rate_general_flag[ i ] u(1) 
  if( !fixed_pic_rate_general_flag[ i ] )  
   fixed_pic_rate_within_cvs_flag[ i ] u(1) 
  if( fixed_pic_rate_within_cvs_flag[ i ] )  
   elemental_duration_in_tc_minus1[ i ] ue(v) 
  else  
   low_delay_hrd_flag[ i ] u(1) 
  if( !low_delay_hrd_flag[ i ] )  
   cpb_cnt_minus1[ i ] ue(v) 
  if( nal_hrd_parameters_present_flag )  
   sub_layer_hrd_parameters( i )  
  if( vcl_hrd_parameters_present_flag )  
   sub_layer_hrd_parameters( i )  
 }  
} 

sub_layer_hrd_parameters( subLayerId ) { 
for( i = 0; i < CpbCnt; i++ ) {  
bit_rate_value_minus1[ i ] ue(v)
cpb_size_value_minus1[ i ]  ue(v) 
if( sub_pic_hrd_params_present_flag ) {  
cpb_size_du_value_minus1[ i ] ue(v)
bit_rate_du_value_minus1[ i ] ue(v)
}  
cbr_flag[ i ] u(1)
}  
}  

vps_extension() {  
 if( vps_max_layers_minus1 > 0  &&  vps_base_layer_internal_flag )  
  profile_tier_level( 0, vps_max_sub_layers_minus1 )  
 splitting_flag u(1) 
 for( i = 0, NumScalabilityTypes = 0; i < 16; i++ ) {  
  scalability_mask_flag[ i ] u(1) 
  NumScalabilityTypes  +=  scalability_mask_flag[ i ]  
 }  
 for( j = 0; j < ( NumScalabilityTypes - splitting_flag ); j++ )  
  dimension_id_len_minus1[ j ] u(3) 
 vps_nuh_layer_id_present_flag u(1) 
 for( i = 1; i <= MaxLayersMinus1; i++ ) {  
  if( vps_nuh_layer_id_present_flag )   
   layer_id_in_nuh[ i ] u(6) 
  if( !splitting_flag )  
     for( j = 0; j < NumScalabilityTypes; j++ )  
    dimension_id[ i ][ j ] u(v) 
 }  
 view_id_len u(4) 
 if( view_id_len > 0 )  
  for( i = 0; i < NumViews; i++ )  
   view_id_val[ i ] u(v) 
 for( i = 1; i <= MaxLayersMinus1; i++ )  
  for( j = 0; j < i; j++ )  
   direct_dependency_flag[ i ][ j ] u(1) 
 if( NumIndependentLayers > 1 )   
  num_add_layer_sets ue(v) 
 for( i = 0; i < num_add_layer_sets; i++ )   
  for( j = 1; j < NumIndependentLayers; j++ )  
   highest_layer_idx_plus1[ i ][ j ] u(v) 
 vps_sub_layers_max_minus1_present_flag u(1) 
 if( vps_sub_layers_max_minus1_present_flag )  
  for( i = 0; i <= MaxLayersMinus1; i++ )  
   sub_layers_vps_max_minus1[ i ] u(3) 
 max_tid_ref_present_flag u(1) 
 if( max_tid_ref_present_flag )  
  for( i = 0; i < MaxLayersMinus1; i++ )  
   for( j = i + 1; j <= MaxLayersMinus1; j++ )  
    if( direct_dependency_flag[ j ][ i ] )  
     max_tid_il_ref_pics_plus1[ i ][ j ] u(3) 
 default_ref_layers_active_flag u(1) 
 vps_num_profile_tier_level_minus1 ue(v) 
 for( i = vps_base_layer_internal_flag ? 2 : 1; 
          i <= vps_num_profile_tier_level_minus1; i++ ) { 
 
  vps_profile_present_flag[ i ] u(1) 
  profile_tier_level( vps_profile_present_flag[ i ], vps_max_sub_layers_minus1 )  
 }  
 if( NumLayerSets > 1 ) {  
  num_add_olss ue(v) 
  default_output_layer_idc u(2) 
 }  
 NumOutputLayerSets = num_add_olss + NumLayerSets  
 for( i = 1; i < NumOutputLayerSets; i++ ) {  
  if( NumLayerSets > 2  &&  i >= NumLayerSets )  
   layer_set_idx_for_ols_minus1[ i ] u(v) 
  if( i > vps_num_layer_sets_minus1  ||  defaultOutputLayerIdc == 2 )  
   for( j = 0; j < NumLayersInIdList[ OlsIdxToLsIdx[ i ] ]; j++ )  
    output_layer_flag[ i ][ j ] u(1) 
  for( j = 0; j < NumLayersInIdList[ OlsIdxToLsIdx[ i ] ]; j++ )  
   if( NecessaryLayerFlag[ i ][ j ]  &&  vps_num_profile_tier_level_minus1 > 0 )  
    profile_tier_level_idx[ i ][ j ] u(v) 
  if( NumOutputLayersInOutputLayerSet[ i ] == 1 
   &&  NumDirectRefLayers[ OlsHighestOutputLayerId[ i ] ] > 0 ) 
 
   alt_output_layer_flag[ i ] u(1) 
    }  
 vps_num_rep_formats_minus1 ue(v) 
 for( i = 0; i <= vps_num_rep_formats_minus1; i++ )  
  rep_format()  
 if( vps_num_rep_formats_minus1 > 0 )  
  rep_format_idx_present_flag u(1) 
 if( rep_format_idx_present_flag )   
  for( i = vps_base_layer_internal_flag ? 1 : 0; i <= MaxLayersMinus1; i++ )  
   vps_rep_format_idx[ i ] u(v) 
 max_one_active_ref_layer_flag u(1) 
 vps_poc_lsb_aligned_flag u(1) 
 for( i = 1; i <= MaxLayersMinus1; i++ )  
  if( NumDirectRefLayers[ layer_id_in_nuh[ i ] ] == 0 )  
   poc_lsb_not_present_flag[ i ] u(1) 
 dpb_size()  
 direct_dep_type_len_minus2 ue(v) 
 direct_dependency_all_layers_flag u(1) 
 if( direct_dependency_all_layers_flag )  
  direct_dependency_all_layers_type u(v) 
 else {  
  for( i = vps_base_layer_internal_flag ? 1 : 2; i <= MaxLayersMinus1; i++ )  
   for( j = vps_base_layer_internal_flag ? 0 : 1; j < i; j++ )  
    if( direct_dependency_flag[ i ][ j ] )  
     direct_dependency_type[ i ][ j ] u(v) 
 }  
 vps_non_vui_extension_length ue(v) 
 for( i = 1; i <= vps_non_vui_extension_length; i++ )  
  vps_non_vui_extension_data_byte u(8) 
 vps_vui_present_flag u(1) 
 if( vps_vui_present_flag ) {  
  while( !byte_aligned() )  
   vps_vui_alignment_bit_equal_to_one u(1) 
  vps_vui()  
 }  
}  

rep_format() {  
 pic_width_vps_in_luma_samples u(16) 
 pic_height_vps_in_luma_samples u(16) 
 chroma_and_bit_depth_vps_present_flag u(1) 
 if( chroma_and_bit_depth_vps_present_flag ) {  
  chroma_format_vps_idc u(2) 
  if( chroma_format_vps_idc == 3 )  
   separate_colour_plane_vps_flag u(1) 
  bit_depth_vps_luma_minus8 u(4) 
  bit_depth_vps_chroma_minus8 u(4) 
 }  
 conformance_window_vps_flag u(1) 
 if( conformance_window_vps_flag ) {  
  conf_win_vps_left_offset ue(v) 
  conf_win_vps_right_offset ue(v) 
  conf_win_vps_top_offset ue(v) 
  conf_win_vps_bottom_offset ue(v) 
 }  
}  

dpb_size() {  
 for( i = 1; i < NumOutputLayerSets; i++ ) {  
  currLsIdx = OlsIdxToLsIdx[ i ]  
  sub_layer_flag_info_present_flag[ i ] u(1) 
  for( j = 0; j <= MaxSubLayersInLayerSetMinus1[ currLsIdx ]; j++ ) {  
   if( j > 0  &&  sub_layer_flag_info_present_flag[ i ] )  
    sub_layer_dpb_info_present_flag[ i ][ j ] u(1) 
   if( sub_layer_dpb_info_present_flag[ i ][ j ] ) {  
    for( k = 0; k < NumLayersInIdList[ currLsIdx ]; k++ )  
     if( NecessaryLayerFlag[ i ][ k ]  &&  ( vps_base_layer_internal_flag  || 
      ( LayerSetLayerIdList[ currLsIdx ][ k ] != 0 ) ) ) 
 
      max_vps_dec_pic_buffering_minus1[ i ][ k ][ j ] ue(v) 
    max_vps_num_reorder_pics[ i ][ j ] ue(v) 
    max_vps_latency_increase_plus1[ i ][ j ] ue(v) 
   }  
  }  
 }  
} 

vps_vui() {  
 cross_layer_pic_type_aligned_flag u(1) 
 if( !cross_layer_pic_type_aligned_flag ) 
   cross_layer_irap_aligned_flag u(1) 
 if( cross_layer_irap_aligned_flag )  
  all_layers_idr_aligned_flag u(1) 
 bit_rate_present_vps_flag u(1) 
 pic_rate_present_vps_flag u(1) 
 if( bit_rate_present_vps_flag  ||  pic_rate_present_vps_flag )  
  for( i = vps_base_layer_internal_flag ? 0 : 1; i < NumLayerSets; i++ )  
   for( j = 0; j <= MaxSubLayersInLayerSetMinus1[ i ]; j++ ) {  
    if( bit_rate_present_vps_flag )  
     bit_rate_present_flag[ i ][ j ] u(1) 
    if( pic_rate_present_vps_flag )  
     pic_rate_present_flag[ i ][ j ] u(1) 
    if( bit_rate_present_flag[ i ][ j ] ) {  
     avg_bit_rate[ i ][ j ] u(16) 
     max_bit_rate[ i ][ j ] u(16) 
    }  
    if( pic_rate_present_flag[ i ][ j ] ) {  
     constant_pic_rate_idc[ i ][ j ] u(2) 
     avg_pic_rate[ i ][ j ] u(16) 
    }  
   }  
 video_signal_info_idx_present_flag u(1) 
 if( video_signal_info_idx_present_flag )  
  vps_num_video_signal_info_minus1 u(4) 
 for( i = 0; i <= vps_num_video_signal_info_minus1; i++ )  
  video_signal_info()  
 if( video_signal_info_idx_present_flag  &&  vps_num_video_signal_info_minus1 > 0 )  
  for( i = vps_base_layer_internal_flag ? 0 : 1; i <= MaxLayersMinus1; i++ )  
   vps_video_signal_info_idx[ i ] u(4) 
 tiles_not_in_use_flag u(1) 
 if( !tiles_not_in_use_flag ) {  
  for( i = vps_base_layer_internal_flag ? 0 : 1; i <= MaxLayersMinus1; i++ ) {  
   tiles_in_use_flag[ i ] u(1) 
   if( tiles_in_use_flag[ i ] )  
    loop_filter_not_across_tiles_flag[ i ] u(1) 
  }  
  for( i = vps_base_layer_internal_flag ? 1 : 2; i <= MaxLayersMinus1; i++ )  
   for( j = 0; j < NumDirectRefLayers[ layer_id_in_nuh[ i ] ]; j++ ) {  
    layerIdx = LayerIdxInVps[ IdDirectRefLayer[ layer_id_in_nuh[ i ] ][ j ] ]  
    if( tiles_in_use_flag[ i ]  &&  tiles_in_use_flag[ layerIdx ] )  
     tile_boundaries_aligned_flag[ i ][ j ] u(1) 
   }  
 }  
 wpp_not_in_use_flag u(1) 
 if( !wpp_not_in_use_flag )  
  for( i = vps_base_layer_internal_flag ? 0 : 1; i <= MaxLayersMinus1; i++ )  
   wpp_in_use_flag[ i ] u(1) 
 single_layer_for_non_irap_flag u(1)
  higher_layer_irap_skip_flag u(1) 
 ilp_restricted_ref_layers_flag u(1) 
 if( ilp_restricted_ref_layers_flag )  
  for( i = 1; i <= MaxLayersMinus1; i++ )  
   for( j = 0; j < NumDirectRefLayers[ layer_id_in_nuh[ i ] ]; j++ )  
    if( vps_base_layer_internal_flag  || 
        IdDirectRefLayer[ layer_id_in_nuh[ i ] ][ j ] > 0 ) { 
 
     min_spatial_segment_offset_plus1[ i ][ j ] ue(v) 
     if( min_spatial_segment_offset_plus1[ i ][ j ] > 0 ) {  
      ctu_based_offset_enabled_flag[ i ][ j ] u(1) 
      if( ctu_based_offset_enabled_flag[ i ][ j ] )  
       min_horizontal_ctu_offset_plus1[ i ][ j ] ue(v) 
     }  
    }  
 vps_vui_bsp_hrd_present_flag u(1) 
 if( vps_vui_bsp_hrd_present_flag )  
  vps_vui_bsp_hrd_params()  
 for( i = 1; i <= MaxLayersMinus1; i++ )  
  if( NumDirectRefLayers[ layer_id_in_nuh[ i ] ] == 0 )  
   base_layer_parameter_set_compatibility_flag[ i ] u(1) 
}  

video_signal_info() {  
 video_vps_format u(3) 
 video_full_range_vps_flag u(1) 
 colour_primaries_vps u(8) 
 transfer_characteristics_vps u(8) 
 matrix_coeffs_vps u(8) 
}  

vps_vui_bsp_hrd_params() {  
 vps_num_add_hrd_params ue(v) 
 for( i = vps_num_hrd_parameters; i < vps_num_hrd_parameters + 
                  vps_num_add_hrd_params; i++ ) { 
 
  if( i > 0 )  
   cprms_add_present_flag[ i ] u(1) 
  num_sub_layer_hrd_minus1[ i ] ue(v) 
  hrd_parameters( cprms_add_present_flag[ i ], num_sub_layer_hrd_minus1[ i ] )  
 }  
 if( vps_num_hrd_parameters + vps_num_add_hrd_params > 0 )  
  for( h = 1; h < NumOutputLayerSets; h++ ) {  
   num_signalled_partitioning_schemes[ h ] ue(v) 
   for( j = 1; j < num_signalled_partitioning_schemes[ h ] + 1; j++ ) {  
    num_partitions_in_scheme_minus1[ h ][ j ] ue(v) 
    for( k = 0; k <= num_partitions_in_scheme_minus1[ h ][ j ]; k++ )  
     for( r = 0; r < NumLayersInIdList[ OlsIdxToLsIdx[ h ] ]; r++ )  
      layer_included_in_partition_flag[ h ][ j ][ k ][ r ] u(1) 
   }  
   for( i = 0; i < num_signalled_partitioning_schemes[ h ] + 1; i++ )  
    for( t = 0; t <= MaxSubLayersInLayerSetMinus1[ OlsIdxToLsIdx[ h ] ]; t++ ) {  
     num_bsp_schedules_minus1[ h ][ i ][ t ] ue(v) 
     for( j = 0; j <= num_bsp_schedules_minus1[ h ][ i ][ t ]; j++ )  
      for( k = 0; k <= num_partitions_in_scheme_minus1[ h ][ i ]; k++ ) {  
       if( vps_num_hrd_parameters + vps_num_add_hrd_params > 1 )  
        bsp_hrd_idx[ h ][ i ][ t ][ j ][ k ] u(v) 
       bsp_sched_idx[ h ][ i ][ t ][ j ][ k ] ue(v) 
      }  
    }  
  }  
} 

seq_parameter_set_annex_f_rbsp() { 
 sps_video_parameter_set_id u(4) 
 if( nuh_layer_id == 0 )  
  sps_max_sub_layers_minus1 u(3) 
 else  
  sps_ext_or_max_sub_layers_minus1 u(3) 
 MultiLayerExtSpsFlag =  ( nuh_layer_id != 0  &&  sps_ext_or_max_sub_layers_minus1 == 7 ) 
 if( !MultiLayerExtSpsFlag ) {  
  sps_temporal_id_nesting_flag u(1) 
  profile_tier_level( 1, sps_max_sub_layers_minus1 )  
 }
  sps_seq_parameter_set_id ue(v) 
 if( MultiLayerExtSpsFlag ) {  
  update_rep_format_flag u(1) 
  if( update_rep_format_flag )  
   sps_rep_format_idx u(8) 
 } else {  
  chroma_format_idc ue(v) 
  if( chroma_format_idc == 3 )  
   separate_colour_plane_flag u(1) 
  pic_width_in_luma_samples ue(v) 
  pic_height_in_luma_samples ue(v) 
  conformance_window_flag u(1) 
  if( conformance_window_flag ) {  
   conf_win_left_offset ue(v) 
   conf_win_right_offset ue(v) 
   conf_win_top_offset ue(v) 
   conf_win_bottom_offset ue(v) 
  }  
  bit_depth_luma_minus8 ue(v) 
  bit_depth_chroma_minus8 ue(v) 
 }  
 log2_max_pic_order_cnt_lsb_minus4 ue(v) 
 if( !MultiLayerExtSpsFlag ) {  
  sps_sub_layer_ordering_info_present_flag u(1) 
  for( i = ( sps_sub_layer_ordering_info_present_flag ? 0 : sps_max_sub_layers_minus1 ); 
    i <= sps_max_sub_layers_minus1; i++ ) { 
 
   sps_max_dec_pic_buffering_minus1[ i ] ue(v) 
   sps_max_num_reorder_pics[ i ] ue(v) 
   sps_max_latency_increase_plus1[ i ] ue(v) 
  }  
 }  
 log2_min_luma_coding_block_size_minus3 ue(v) 
 log2_diff_max_min_luma_coding_block_size ue(v) 
 log2_min_luma_transform_block_size_minus2 ue(v) 
 log2_diff_max_min_luma_transform_block_size ue(v) 
 max_transform_hierarchy_depth_inter ue(v) 
 max_transform_hierarchy_depth_intra ue(v) 
 scaling_list_enabled_flag u(1) 
 if( scaling_list_enabled_flag ) {  
  if( MultiLayerExtSpsFlag )  
   sps_infer_scaling_list_flag u(1) 
  if( sps_infer_scaling_list_flag )  
   sps_scaling_list_ref_layer_id u(6) 
  else {  
   sps_scaling_list_data_present_flag u(1) 
   if( sps_scaling_list_data_present_flag )  
    scaling_list_data()  
  }  
 } 
  amp_enabled_flag u(1) 
 sample_adaptive_offset_enabled_flag u(1) 
 pcm_enabled_flag u(1) 
 if( pcm_enabled_flag ) {  
  pcm_sample_bit_depth_luma_minus1 u(4) 
  pcm_sample_bit_depth_chroma_minus1 u(4) 
  log2_min_pcm_luma_coding_block_size_minus3 ue(v) 
  log2_diff_max_min_pcm_luma_coding_block_size ue(v) 
  pcm_loop_filter_disabled_flag u(1) 
 }  
 num_short_term_ref_pic_sets ue(v) 
 for( i = 0; i < num_short_term_ref_pic_sets; i++ )  
  st_ref_pic_set( i )  
 long_term_ref_pics_present_flag u(1) 
 if( long_term_ref_pics_present_flag ) {  
  num_long_term_ref_pics_sps ue(v) 
  for( i = 0; i < num_long_term_ref_pics_sps; i++ ) {  
   lt_ref_pic_poc_lsb_sps[ i ] u(v) 
   used_by_curr_pic_lt_sps_flag[ i ] u(1) 
  }  
 }  
 sps_temporal_mvp_enabled_flag u(1) 
 strong_intra_smoothing_enabled_flag u(1) 
 vui_parameters_present_flag u(1) 
 if( vui_parameters_present_flag )  
  vui_parameters()  
 sps_extension_present_flag u(1) 
 if( sps_extension_present_flag ) {  
  sps_range_extension_flag u(1) 
  sps_multilayer_extension_flag u(1) 
  sps_3d_extension_flag u(1) 
  sps_scc_extension_flag u(1) 
  sps_extension_4bits u(5) 
 }  
 if( sps_range_extension_flag )  
  sps_range_extension()  
 if( sps_multilayer_extension_flag )  
  sps_multilayer_extension()  
 if( sps_3d_extension_flag )  
  sps_3d_extension()  /* specified in Annex I */  
 if( sps_scc_extension_flag )  
  sps_scc_extension()  
 if( sps_extension_4bits )  
  while( more_rbsp_data() )  
   sps_extension_data_flag u(1) 
 rbsp_trailing_bits()  
}  

sps_multilayer_extension() { 
inter_view_mv_vert_constraint_flag  u(1) 
}

pps_multilayer_extension() {  
 poc_reset_info_present_flag u(1) 
 pps_infer_scaling_list_flag u(1) 
 if( pps_infer_scaling_list_flag )  
  pps_scaling_list_ref_layer_id u(6) 
 num_ref_loc_offsets ue(v) 
 for( i = 0; i < num_ref_loc_offsets; i++ ) {   
  ref_loc_offset_layer_id[ i ] u(6) 
  scaled_ref_layer_offset_present_flag[ i ] u(1) 
  if( scaled_ref_layer_offset_present_flag[ i ] ) {  
   scaled_ref_layer_left_offset[ ref_loc_offset_layer_id[ i ] ] se(v) 
   scaled_ref_layer_top_offset[ ref_loc_offset_layer_id[ i ] ] se(v) 
   scaled_ref_layer_right_offset[ ref_loc_offset_layer_id[ i ] ] se(v) 
   scaled_ref_layer_bottom_offset[ ref_loc_offset_layer_id[ i ] ] se(v) 
  }  
  ref_region_offset_present_flag[ i ] u(1) 
  if( ref_region_offset_present_flag[ i ] ) {  
   ref_region_left_offset[ ref_loc_offset_layer_id[ i ] ] se(v) 
   ref_region_top_offset[ ref_loc_offset_layer_id[ i ] ] se(v) 
   ref_region_right_offset[ ref_loc_offset_layer_id[ i ] ] se(v) 
   ref_region_bottom_offset[ ref_loc_offset_layer_id[ i ] ] se(v) 
  }  
  resample_phase_set_present_flag[ i ] u(1) 
  if( resample_phase_set_present_flag[ i ] ) {  
   phase_hor_luma[ ref_loc_offset_layer_id[ i ] ] ue(v) 
   phase_ver_luma[ ref_loc_offset_layer_id[ i ] ] ue(v) 
   phase_hor_chroma_plus8[ ref_loc_offset_layer_id[ i ] ] ue(v) 
   phase_ver_chroma_plus8[ ref_loc_offset_layer_id[ i ] ] ue(v) 
  }  
 }  
 colour_mapping_enabled_flag u(1) 
 if( colour_mapping_enabled_flag )  
  colour_mapping_table()  
} 

colour_mapping_table() {  
 num_cm_ref_layers_minus1 ue(v) 
 for( i = 0; i <= num_cm_ref_layers_minus1; i++ )  
  cm_ref_layer_id[ i ] u(6) 
 cm_octant_depth u(2) 
 cm_y_part_num_log2 u(2) 
 luma_bit_depth_cm_input_minus8 ue(v) 
 chroma_bit_depth_cm_input_minus8 ue(v) 
 luma_bit_depth_cm_output_minus8 ue(v) 
 chroma_bit_depth_cm_output_minus8 ue(v) 
 cm_res_quant_bits u(2) 
 cm_delta_flc_bits_minus1 u(2) 
 if( cm_octant_depth == 1 ) {  
  cm_adapt_threshold_u_delta se(v) 
  cm_adapt_threshold_v_delta se(v) 
 }  
 colour_mapping_octants( 0, 0, 0, 0, 1  <<  cm_octant_depth )  
}  

colour_mapping_octants( inpDepth, idxY, idxCb, idxCr, inpLength ) {   
 if( inpDepth < cm_octant_depth )  
  split_octant_flag u(1) 
 if( split_octant_flag )  
  for( k = 0; k < 2; k++ )  
   for( m = 0; m < 2; m++ )  
    for( n = 0; n < 2; n++ )  
     colour_mapping_octants( inpDepth + 1, idxY + PartNumY * k * inpLength / 2, 
      idxCb + m * inpLength / 2, idxCr + n * inpLength / 2, inpLength / 2 ) 
 
 else  
  for( i = 0; i < PartNumY; i++ ) {  
   idxShiftY = idxY + ( i << ( cm_octant_depth - inpDepth ) )  
   for( j = 0; j < 4; j++ ) {  
    coded_res_flag[ idxShiftY ][ idxCb ][ idxCr ][ j ] u(1) 
    if( coded_res_flag[ idxShiftY ][ idxCb ][ idxCr ][ j ] )  
     for( c = 0; c < 3; c++ ) {  
      res_coeff_q[ idxShiftY ][ idxCb ][ idxCr ][ j ][ c ] ue(v) 
      res_coeff_r[ idxShiftY ][ idxCb ][ idxCr ][ j ][ c ] u(v) 
      if( res_coeff_q[ idxShiftY ][ idxCb ][ idxCr ][ j ][ c ]  ||  
        res_coeff_r[ idxShiftY ][ idxCb ][ idxCr ][ j ][ c ] ) 
 
       res_coeff_s[ idxShiftY ][ idxCb ][ idxCr ][ j ][ c ] u(1) 
     }  
   }  
  }  
}  

layers_not_present( payloadSize ) {  
 lnp_sei_active_vps_id u(4) 
 for( i = 0; i <= MaxLayersMinus1; i++ )   
  layer_not_present_flag[ i ] u(1) 
}   

inter_layer_constrained_tile_sets( payloadSize ) {  
 il_all_tiles_exact_sample_value_match_flag u(1) 
 il_one_tile_per_tile_set_flag u(1) 
 if( !il_one_tile_per_tile_set_flag ) {  
  il_num_sets_in_message_minus1 ue(v) 
  if( il_num_sets_in_message_minus1 )  
   skipped_tile_set_present_flag u(1) 
  numSignificantSets = il_num_sets_in_message_minus1 - skipped_tile_set_present_flag + 1 
  for( i = 0; i < numSignificantSets; i++ ) {  
   ilcts_id[ i ] ue(v) 
   il_num_tile_rects_in_set_minus1[ i ] ue(v) 
   for( j = 0; j <= il_num_tile_rects_in_set_minus1[ i ]; j++ ) {  
    il_top_left_tile_idx[ i ][ j ] ue(v) 
    il_bottom_right_tile_idx[ i ][ j ] ue(v) 
   }  
   ilc_idc[ i ] u(2) 
   if ( !il_all_tiles_exact_sample_value_match_flag )  
    il_exact_sample_value_match_flag[ i ] u(1) 
  }  
 } else  
  all_tiles_ilc_idc u(2) 
}  

bsp_nesting( payloadSize ) {  
 sei_ols_idx ue(v) 
 sei_partitioning_scheme_idx ue(v) 
 bsp_idx ue(v) 
 num_seis_in_bsp_minus1 ue(v) 
 while( !byte_aligned() )  
  bsp_nesting_zero_bit /* equal to 0 */ u(1) 
 for( i = 0; i <= num_seis_in_bsp_minus1; i++ )  
  sei_message()  
}  

bsp_initial_arrival_time( payloadSize ) {  
 psIdx = sei_partitioning_scheme_idx  
 if( nalInitialArrivalDelayPresent )  
  for( i = 0; i < BspSchedCnt[ sei_ols_idx ][ psIdx ][ MaxTemporalId[ 0 ] ]; i++ )  
   nal_initial_arrival_delay[ i ] u(v) 
 if( vclInitialArrivalDelayPresent )  
  for( i = 0; i < BspSchedCnt[ sei_ols_idx ][ psIdx ][ MaxTemporalId[ 0 ] ]; i++ )  
   vcl_initial_arrival_delay[ i ] u(v) 
}  

sub_bitstream_property( payloadSize ) {  
 sb_property_active_vps_id u(4) 
 num_additional_sub_streams_minus1 ue(v) 
 for( i = 0; i <= num_additional_sub_streams_minus1; i++ ) {  
  sub_bitstream_mode[ i ] u(2) 
  ols_idx_to_vps[ i ] ue(v) 
  highest_sublayer_id[ i ] u(3) 
  avg_sb_property_bit_rate[ i ] u(16) 
  max_sb_property_bit_rate[ i ] u(16) 
 }  
} 

alpha_channel_info( payloadSize ) { 
alpha_channel_cancel_flag u(1)  
if( !alpha_channel_cancel_flag ) {  

alpha_channel_use_idc u(3) 
alpha_channel_bit_depth_minus8 u(3) 
alpha_transparent_value u(v) 

alpha_opaque_value u(v)
alpha_channel_incr_flag u(1) 
alpha_channel_clip_flag u(1) 

if( alpha_channel_clip_flag )  
alpha_channel_clip_type_flag u(1) 
}  
}  

overlay_info( payloadSize ) {  
 overlay_info_cancel_flag u(1) 
 if( !overlay_info_cancel_flag ) {  
  overlay_content_aux_id_minus128 ue(v) 
  overlay_label_aux_id_minus128 ue(v) 
  overlay_alpha_aux_id_minus128 ue(v) 
  overlay_element_label_value_length_minus8 ue(v) 
  num_overlays_minus1 ue(v) 
  for( i = 0; i <= num_overlays_minus1; i++ ) {  
   overlay_idx[ i ] ue(v) 
   language_overlay_present_flag[ i ] u(1) 
   overlay_content_layer_id[ i ] u(6) 
   overlay_label_present_flag[ i ] u(1) 
   if( overlay_label_present_flag[ i ] )   
    overlay_label_layer_id[ i ] u(6) 
   overlay_alpha_present_flag[ i ] u(1) 
   if( overlay_alpha_present_flag[ i ] )  
    overlay_alpha_layer_id[ i ] u(6) 
   if( overlay_label_present_flag[ i ] ) {  
    num_overlay_elements_minus1[ i ] ue(v) 
    for( j = 0; j <= num_overlay_elements_minus1[ i ]; j++ ) {  
     overlay_element_label_min[ i ][ j ] u(v) 
     overlay_element_label_max[ i ][ j ] u(v) 
    }  
   }  
  }  
  while( !byte_aligned() )  
   overlay_zero_bit /* equal to 0 */ f(1) 
  for( i = 0; i <= num_overlays_minus1; i++ ) {  
   if( language_overlay_present_flag[ i ] )  
    overlay_language[ i ] st(v) 
   overlay_name[ i ] st(v) 
   if( overlay_label_present_flag[ i ] )  
    for( j = 0; j <= num_overlay_elements_minus1[ i ]; j++ )  
     overlay_element_name[ i ][ j ] st(v) 
  }  
  overlay_info_persistence_flag u(1) 
 }  
}  

temporal_mv_prediction_constraints( payloadSize ) { 
prev_pics_not_used_flag  u(1) 
no_intra_layer_col_pic_flag u(1) 
}  

frame_field_info( payloadSize ) { 
ffinfo_pic_struct u(4)
ffinfo_source_scan_type u(2) 
ffinfo_duplicate_flag u(1) 
}  


three_dimensional_reference_displays_info( payloadSize ) { 
prec_ref_display_width  ue(v) 
ref_viewing_distance_flag u(1) 


if( ref_viewing_distance_flag )  
prec_ref_viewing_dist ue(v) 
num_ref_displays_minus1 ue(v) 

for( i = 0; i <= num_ref_displays_minus1; i++ ) {  
left_view_id[ i ] ue(v) 
right_view_id[ i ] ue(v) 
exponent_ref_display_width[ i ] u(6) 
mantissa_ref_display_width[ i ] u(v) 
if( ref_viewing_distance_flag ) {  
exponent_ref_viewing_distance[ i ] u(6) 
mantissa_ref_viewing_distance[ i ] u(v) 
}  
additional_shift_present_flag[ i ] u(1) 
if( additional_shift_present_flag[ i ] )  
num_sample_shift_plus512[ i ] u(10) 
}  
three_dimensional_reference_displays_extension_flag u(1) 
}  

depth_representation_info( payloadSize ) {  
 z_near_flag u(1) 
 z_far_flag u(1) 
 d_min_flag u(1) 
 d_max_flag u(1) 
 depth_representation_type ue(v) 
 if( d_min_flag  ||  d_max_flag )   
  disparity_ref_view_id ue(v) 
 if( z_near_flag )  
  depth_rep_info_element( ZNearSign, ZNearExp, ZNearMantissa, ZNearManLen )  
 if( z_far_flag )  
  depth_rep_info_element( ZFarSign, ZFarExp, ZFarMantissa, ZFarManLen )  
 if( d_min_flag )  
  depth_rep_info_element( DMinSign, DMinExp, DMinMantissa, DMinManLen )  
 if( d_max_flag )  
  depth_rep_info_element( DMaxSign, DMaxExp, DMaxMantissa, DMaxManLen )  
 if( depth_representation_type == 3 ) {  
  depth_nonlinear_representation_num_minus1 ue(v) 
  for( i = 1; i <= depth_nonlinear_representation_num_minus1 + 1; i++ )  
   depth_nonlinear_representation_model[ i ] u(8)
 }  
} 

depth_rep_info_element( OutSign, OutExp, OutMantissa, OutManLen ) {  
 da_sign_flag u(1) 
 da_exponent u(7) 
 da_mantissa_len_minus1 u(5) 
 da_mantissa u(v) 
}

multiview_scene_info( payloadSize ) {  
 min_disparity se(v) 
 max_disparity_range ue(v) 
}

multiview_acquisition_info( payloadSize ) {  
 intrinsic_param_flag u(1) 
 extrinsic_param_flag u(1) 
 if( intrinsic_param_flag ) {  
  intrinsic_params_equal_flag u(1) 
  prec_focal_length ue(v) 
  prec_principal_point ue(v) 
  prec_skew_factor ue(v) 
  for( i = 0; i <= intrinsic_params_equal_flag ? 0 : numViewsMinus1; i++ ) {  
   sign_focal_length_x[ i ] u(1) 
   exponent_focal_length_x[ i ] u(6) 
   mantissa_focal_length_x[ i ] u(v) 
   sign_focal_length_y[ i ] u(1) 
   exponent_focal_length_y[ i ] u(6) 
   mantissa_focal_length_y[ i ] u(v) 
   sign_principal_point_x[ i ] u(1) 
   exponent_principal_point_x[ i ] u(6) 
   mantissa_principal_point_x[ i ] u(v) 
   sign_principal_point_y[ i ] u(1) 
   exponent_principal_point_y[ i ] u(6) 
   mantissa_principal_point_y[ i ] u(v) 
   sign_skew_factor[ i ] u(1) 
   exponent_skew_factor[ i ] u(6) 
   mantissa_skew_factor[ i ] u(v) 
  }  
 }  
 if( extrinsic_param_flag ) {  
  prec_rotation_param ue(v) 
  prec_translation_param ue(v) 
  for( i = 0; i <= numViewsMinus1; i++ )  
   for( j = 0; j < 3; j++ ) { /* row */  
    for( k = 0; k < 3; k++ ) { /* column */  
     sign_r[ i ][ j ][ k ] u(1) 
     exponent_r[ i ][ j ][ k ] u(6) 
     mantissa_r[ i ][ j ][ k ] u(v) 
    }  
    sign_t[ i ][ j ] u(1) 
    exponent_t[ i ][ j ] u(6) 
    mantissa_t[ i ][ j ] u(v) 
   }  
 }  
} 

multiview_view_position( payloadSize ) { 
num_views_minus1  ue(v)
for( i = 0; i <= num_views_minus1; i++ )  
view_position[ i ] ue(v) 
}

video_parameter_set_annex_f_rbsp() { 
vps_video_parameter_set_id  u(4) 
vps_base_layer_internal_flag u(1) 
vps_base_layer_available_flag u(1) 
vps_max_layers_minus1 u(6) 
vps_max_sub_layers_minus1 u(3) 
vps_temporal_id_nesting_flag u(1) 
vps_reserved_0xffff_16bits u(16) 
profile_tier_level( 1, vps_max_sub_layers_minus1 )  
vps_sub_layer_ordering_info_present_flag  u(1)
for( i = ( vps_sub_layer_ordering_info_present_flag ? 0 : vps_max_sub_layers_minus1 ); i<= vps_max_sub_layers_minus1; i++ ) { 
vps_max_dec_pic_buffering_minus1[ i ] ue(v) 
vps_max_num_reorder_pics[ i ] ue(v) 
vps_max_latency_increase_plus1[ i ] ue(v) 
}  
vps_max_layer_id u(6)
vps_num_layer_sets_minus1 ue(v) 
for( i = 1; i <= vps_num_layer_sets_minus1; i++ )  
for( j = 0; j <= vps_max_layer_id; j++ )  
layer_id_included_flag[ i ][ j ] u(1)
 vps_timing_info_present_flag u(1) 
 if( vps_timing_info_present_flag ) {  
  vps_num_units_in_tick u(32) 
  vps_time_scale u(32) 
  vps_poc_proportional_to_timing_flag u(1) 
  if( vps_poc_proportional_to_timing_flag )  
   vps_num_ticks_poc_diff_one_minus1 ue(v) 
  vps_num_hrd_parameters ue(v) 
  for( i = 0; i < vps_num_hrd_parameters; i++ ) {  
   hrd_layer_set_idx[ i ] ue(v) 
   if( i > 0 )  
    cprms_present_flag[ i ] u(1) 
   hrd_parameters( cprms_present_flag[ i ], vps_max_sub_layers_minus1 )  
  }  
 }  
 vps_extension_flag u(1) 
 if( vps_extension_flag ) {  
  while( !byte_aligned() )  
   vps_extension_alignment_bit_equal_to_one u(1) 
  vps_extension()  
  vps_extension2_flag u(1) 
  if( vps_extension2_flag ) {  
   vps_3d_extension_flag u(1) 
   if( vps_3d_extension_flag ) {  
    while( !byte_aligned() )  
     vps_3d_extension_alignment_bit_equal_to_one u(1) 
    vps_3d_extension()  
   }  
   vps_extension3_flag u(1) 
   if( vps_extension3_flag )  
    while( more_rbsp_data() )  
     vps_extension_data_flag u(1) 
  }  
 }  
 rbsp_trailing_bits()  
} 

vps_3d_extension() {  
 cp_precision ue(v) 
 for( n = 1; n < NumViews; n++ ) {  
  i = ViewOIdxList[ n ]  
  num_cp[ i ] u(6) 
  if( num_cp[ i ] > 0 ) {  
   cp_in_slice_segment_header_flag[ i ] u(1) 
   for( m = 0; m < num_cp[ i ]; m++ ) {  
    cp_ref_voi[ i ][ m ] ue(v) 
    if( !cp_in_slice_segment_header_flag[ i ] ) {  
     j = cp_ref_voi[ i ][ m ]  
     vps_cp_scale[ i ][ j ] se(v) 
     vps_cp_off[ i ][ j ] se(v) 
     vps_cp_inv_scale_plus_scale[ i ][ j ] se(v) 
     vps_cp_inv_off_plus_off[ i ][ j ] se(v) 
    }  
   }  
  }  
 }  
}  

sps_3d_extension() {  
 for( d = 0; d <= 1; d++ ) {  
  iv_di_mc_enabled_flag[ d ] u(1) 
  iv_mv_scal_enabled_flag[ d ] u(1) 
  if( d == 0 ) {  
   log2_ivmc_sub_pb_size_minus3[ d ] ue(v) 
      iv_res_pred_enabled_flag[ d ] u(1) 
   depth_ref_enabled_flag[ d ] u(1) 
   vsp_mc_enabled_flag[ d ] u(1) 
   dbbp_enabled_flag[ d ] u(1) 
  } else {  
   tex_mc_enabled_flag[ d ] u(1) 
   log2_texmc_sub_pb_size_minus3[ d ] ue(v) 
   intra_contour_enabled_flag[ d ] u(1) 
   intra_dc_only_wedge_enabled_flag[ d ] u(1) 
   cqt_cu_part_pred_enabled_flag[ d ] u(1) 
   inter_dc_only_enabled_flag[ d ] u(1) 
   skip_intra_enabled_flag[ d ] u(1) 
  }  
 }  
}  

pps_3d_extension() {  
 dlts_present_flag u(1) 
 if( dlts_present_flag ) {  
  pps_depth_layers_minus1 u(6) 
  pps_bit_depth_for_depth_layers_minus8 u(4) 
  for( i = 0; i <= pps_depth_layers_minus1; i++ ) {  
   dlt_flag[ i ] u(1) 
   if( dlt_flag[ i ] ) {  
    dlt_pred_flag[ i ] u(1) 
    if( !dlt_pred_flag[ i ] )  
     dlt_val_flags_present_flag[ i ] u(1) 
    if( dlt_val_flags_present_flag[ i ] )  
     for( j = 0; j <= depthMaxValue; j++ )  
           dlt_value_flag[ i ][ j ] u(1) 
    else  
     delta_dlt( i )  
   }  
  }  
 }  
}  

delta_dlt( i ) {  
 num_val_delta_dlt u(v) 
 if( num_val_delta_dlt > 0 ) {  
  if( num_val_delta_dlt > 1 )  
   max_diff u(v) 
  if( num_val_delta_dlt > 2  &&  max_diff > 0 )  
   min_diff_minus1 u(v) 
  delta_dlt_val0 u(v) 
  if( max_diff > ( min_diff_minus1 + 1 ) )  
   for( k = 1; k < num_val_delta_dlt; k++ )   
    delta_val_diff_minus_min[ k ] u(v) 
 }  
} 

slice_segment_header() {  
 first_slice_segment_in_pic_flag u(1) 
 if( nal_unit_type >= BLA_W_LP  &&  nal_unit_type <= RSV_IRAP_VCL23 )  
  no_output_of_prior_pics_flag u(1) 
 slice_pic_parameter_set_id ue(v) 
 if( !first_slice_segment_in_pic_flag ) {  
  if( dependent_slice_segments_enabled_flag )  
   dependent_slice_segment_flag u(1) 
  slice_segment_address u(v) 
 }  
 if( !dependent_slice_segment_flag ) {  
  i = 0  
  if( num_extra_slice_header_bits > i ) {  
   i++  
   discardable_flag u(1) 
  }  
  if( num_extra_slice_header_bits > i ) {  
   i++  
   cross_layer_bla_flag u(1) 
  }  
  for(; i < num_extra_slice_header_bits; i++ )  
   slice_reserved_flag[ i ] u(1) 
  slice_type ue(v) 
  if( output_flag_present_flag )  
   pic_output_flag u(1) 
  if( separate_colour_plane_flag == 1 )  
   colour_plane_id u(2) 
  if( ( nuh_layer_id > 0  &&   
    !poc_lsb_not_present_flag[ LayerIdxInVps[ nuh_layer_id ] ] )  || 
    ( nal_unit_type != IDR_W_RADL  &&  nal_unit_type != IDR_N_LP ) ) 
 
   slice_pic_order_cnt_lsb u(v) 
  if( nal_unit_type != IDR_W_RADL  &&  nal_unit_type != IDR_N_LP ) {  
   short_term_ref_pic_set_sps_flag u(1) 
   if( !short_term_ref_pic_set_sps_flag )  
    st_ref_pic_set( num_short_term_ref_pic_sets )  
   else if( num_short_term_ref_pic_sets > 1 )  
    short_term_ref_pic_set_idx u(v) 
   if( long_term_ref_pics_present_flag ) { 
       if( num_long_term_ref_pics_sps > 0 )  
     num_long_term_sps ue(v) 
    num_long_term_pics ue(v) 
    for( i = 0; i < num_long_term_sps + num_long_term_pics; i++ ) {  
     if( i < num_long_term_sps ) {  
      if( num_long_term_ref_pics_sps > 1 )  
       lt_idx_sps[ i ] u(v) 
     } else {  
      poc_lsb_lt[ i ] u(v) 
      used_by_curr_pic_lt_flag[ i ] u(1) 
     }  
     delta_poc_msb_present_flag[ i ] u(1) 
     if( delta_poc_msb_present_flag[ i ] )  
      delta_poc_msb_cycle_lt[ i ] ue(v) 
    }  
   }  
   if( sps_temporal_mvp_enabled_flag )  
    slice_temporal_mvp_enabled_flag u(1) 
  }  
  if( nuh_layer_id > 0  &&  !default_ref_layers_active_flag  &&   
      NumRefListLayers[ nuh_layer_id ] > 0 ) {  
 
   inter_layer_pred_enabled_flag u(1) 
   if( inter_layer_pred_enabled_flag  &&  NumRefListLayers[ nuh_layer_id ] > 1) {  
    if( !max_one_active_ref_layer_flag )  
     num_inter_layer_ref_pics_minus1 u(v) 
    if( NumActiveRefLayerPics != NumRefListLayers[ nuh_layer_id ] )  
     for( i = 0; i < NumActiveRefLayerPics; i++ )   
      inter_layer_pred_layer_idc[ i ] u(v) 
   }  
  }  
  if( inCmpPredAvailFlag )  
   in_comp_pred_flag u(1) 
  if( sample_adaptive_offset_enabled_flag ) {  
   slice_sao_luma_flag u(1) 
   if( ChromaArrayType != 0 )  
    slice_sao_chroma_flag u(1) 
  }  
  if( slice_type == P  ||  slice_type == B ) {  
   num_ref_idx_active_override_flag u(1) 
   if( num_ref_idx_active_override_flag ) {  
    num_ref_idx_l0_active_minus1 ue(v) 
    if( slice_type == B )  
     num_ref_idx_l1_active_minus1 ue(v) 
   }  
   if( lists_modification_present_flag  &&  NumPicTotalCurr > 1 )  
    ref_pic_lists_modification()  
   if( slice_type == B )  
    mvd_l1_zero_flag u(1) 
   if( cabac_init_present_flag ) 
       cabac_init_flag u(1) 
   if( slice_temporal_mvp_enabled_flag ) {  
    if( slice_type == B )  
     collocated_from_l0_flag u(1) 
    if( ( collocated_from_l0_flag &&  num_ref_idx_l0_active_minus1 > 0 ) || ( !collocated_from_l0_flag && num_ref_idx_l1_active_minus1 > 0)) 
     collocated_ref_idx ue(v) 
   }  
   if( ( weighted_pred_flag  &&  slice_type == P )  || 
     ( weighted_bipred_flag  &&  slice_type == B ) ) 
 
    pred_weight_table()  
   else if( !DepthFlag  &&  NumRefListLayers[ nuh_layer_id ] > 0 ) {  
    slice_ic_enabled_flag u(1) 
    if( slice_ic_enabled_flag )  
     slice_ic_disabled_merge_zero_idx_flag u(1) 
   }  
   five_minus_max_num_merge_cand ue(v) 
  }  
  slice_qp_delta se(v) 
  if( pps_slice_chroma_qp_offsets_present_flag ) {  
   slice_cb_qp_offset se(v) 
   slice_cr_qp_offset se(v) 
  }  
  if( chroma_qp_offset_list_enabled_flag )  
   cu_chroma_qp_offset_enabled_flag u(1) 
  if( deblocking_filter_override_enabled_flag )  
   deblocking_filter_override_flag u(1) 
  if( deblocking_filter_override_flag ) {  
   slice_deblocking_filter_disabled_flag u(1) 
   if( !slice_deblocking_filter_disabled_flag ) {  
    slice_beta_offset_div2 se(v) 
    slice_tc_offset_div2 se(v) 
   }  
  }  
  if( pps_loop_filter_across_slices_enabled_flag  &&   
   ( slice_sao_luma_flag  ||  slice_sao_chroma_flag  || 
    !slice_deblocking_filter_disabled_flag ) ) 
 
   slice_loop_filter_across_slices_enabled_flag u(1) 
  if( cp_in_slice_segment_header_flag[ ViewIdx ] )  
   for( m = 0; m < num_cp[ ViewIdx ]; m++ ) {  
    j = cp_ref_voi[ ViewIdx ][ m ]  
    cp_scale[ j ] se(v) 
    cp_off[ j ] se(v) 
    cp_inv_scale_plus_scale[ j ] se(v) 
    cp_inv_off_plus_off[ j ] se(v) 
   }  
 }  
 if( tiles_enabled_flag  ||  entropy_coding_sync_enabled_flag ) {  
  num_entry_point_offsets ue(v) 
    if( num_entry_point_offsets > 0 ) {  
   offset_len_minus1 ue(v) 
   for( i = 0; i < num_entry_point_offsets; i++ )  
    entry_point_offset_minus1[ i ] u(v) 
  }  
 }  
 if( slice_segment_header_extension_present_flag ) {  
  slice_segment_header_extension_length ue(v) 
  if( poc_reset_info_present_flag )  
   poc_reset_idc u(2) 
  if( poc_reset_idc != 0 )  
   poc_reset_period_id u(6) 
  if( poc_reset_idc == 3 ) {  
   full_poc_reset_flag u(1) 
   poc_lsb_val u(v) 
  }  
  if( !PocMsbValRequiredFlag  &&  vps_poc_lsb_aligned_flag )  
   poc_msb_cycle_val_present_flag u(1) 
  if( poc_msb_cycle_val_present_flag )  
   poc_msb_cycle_val ue(v) 
  while( more_data_in_slice_segment_header_extension() )  
   slice_segment_header_extension_data_bit u(1) 
 }  
 byte_alignment()  
}  

alternative_depth_info( payloadSize ) {  
 alternative_depth_info_cancel_flag u(1) 
 if( alternative_depth_info_cancel_flag == 0 ) {  
  depth_type u(2) 
  if( depth_type == 0 ) {  
   num_constituent_views_gvd_minus1 ue(v) 
   depth_present_gvd_flag u(1) 
   z_gvd_flag u(1) 
   intrinsic_param_gvd_flag u(1) 
   rotation_gvd_flag u(1) 
   translation_gvd_flag u(1) 
   if( z_gvd_flag )   
    for( i = 0; i <= num_constituent_views_gvd_minus1 + 1; i++ ) {  
     sign_gvd_z_near_flag[ i ] u(1) 
     exp_gvd_z_near[ i ] u(7) 
          man_len_gvd_z_near_minus1[ i ] u(5) 
     man_gvd_z_near[ i ] u(v) 
     sign_gvd_z_far_flag[ i ] u(1) 
     exp_gvd_z_far[ i ] u(7) 
     man_len_gvd_z_far_minus1[ i ] u(5) 
     man_gvd_z_far[ i ] u(v) 
    }  
   if( intrinsic_param_gvd_flag ) {  
    prec_gvd_focal_length ue(v) 
    prec_gvd_principal_point ue(v) 
   }  
   if( rotation_gvd_flag )  
    prec_gvd_rotation_param ue(v) 
   if( translation_gvd_flag )  
    prec_gvd_translation_param ue(v) 
   for( i = 0; i <= num_constituent_views_gvd_minus1 + 1; i++ ) {  
    if( intrinsic_param_gvd_flag ) {  
     sign_gvd_focal_length_x[ i ] u(1) 
     exp_gvd_focal_length_x[ i ] u(6) 
     man_gvd_focal_length_x[ i ] u(v) 
     sign_gvd_focal_length_y[ i ] u(1) 
     exp_gvd_focal_length_y[ i ] u(6) 
     man_gvd_focal_length_y[ i ] u(v) 
     sign_gvd_principal_point_x[ i ] u(1) 
     exp_gvd_principal_point_x[ i ] u(6) 
     man_gvd_principal_point_x[ i ] u(v) 
     sign_gvd_principal_point_y[ i ] u(1) 
     exp_gvd_principal_point_y[ i ] u(6) 
     man_gvd_principal_point_y[ i ] u(v) 
    }  
    if( rotation_gvd_flag )   
     for( j = 0; j < 3; j++ ) /* row */  
      for( k = 0; k < 3; k++ ) { /* column */  
       sign_gvd_r[ i ][ j ][ k ] u(1) 
       exp_gvd_r[ i ][ j ][ k ] u(6) 
       man_gvd_r[ i ][ j ][ k ] u(v) 
      }  
    if( translation_gvd_flag ) {  
     sign_gvd_t_x[ i ] u(1) 
     exp_gvd_t_x[ i ] u(6) 
     man_gvd_t_x[ i ] u(v) 
    }  
   }  
  }  
  if( depth_type == 1 ) {  
   min_offset_x_int se(v) 
   min_offset_x_frac u(8) 
   max_offset_x_int se(v)
      max_offset_x_frac u(8) 
   offset_y_present_flag u(1) 
   if( offset_y_present_flag ){  
    min_offset_y_int se(v) 
    min_offset_y_frac u(8) 
    max_offset_y_int se(v) 
    max_offset_y_frac u(8) 
   }  
   warp_map_size_present_flag u(1) 
   if( warp_map_size_present_flag ) {  
    warp_map_width_minus2  ue(v) 
    warp_map_height_minus2 ue(v) 
   }  
  }  
 }  
}  

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
