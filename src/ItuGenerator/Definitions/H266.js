nal_unit( NumBytesInNalUnit ) { 
nal_unit_header()  
/*NumBytesInRbsp = 0  */
/*for( i = 2; i < NumBytesInNalUnit; i++ )  */
/*if( i + 2 < NumBytesInNalUnit  &&  next_bits( 24 )  ==  0x000003 ) {  */
/*rbsp_byte[ NumBytesInRbsp++ ] b(8) */
/*rbsp_byte[ NumBytesInRbsp++ ] b(8) */
/*i  +=  2  */
/*emulation_prevention_three_byte*/  /* equal to 0x03 *//* f(8) */
/*} else  */
/*rbsp_byte[ NumBytesInRbsp++ ] b(8) */
}

nal_unit_header() {  
 forbidden_zero_bit f(1) 
 nuh_reserved_zero_bit u(1) 
 nuh_layer_id u(6) 
 nal_unit_type u(5) 
 nuh_temporal_id_plus1 u(3) 
}  

decoding_capability_information_rbsp() {  
 dci_reserved_zero_4bits u(4) 
 dci_num_ptls_minus1 u(4) 
 for( i = 0; i  <=  dci_num_ptls_minus1; i++ )  
  profile_tier_level( 1, 0 )  
 dci_extension_flag u(1) 
 if( dci_extension_flag )  
  while( more_rbsp_data() )  
   dci_extension_data_flag u(1) 
 rbsp_trailing_bits()  
}  

operating_point_information_rbsp() {  
 opi_ols_info_present_flag u(1) 
 opi_htid_info_present_flag u(1) 
 if( opi_ols_info_present_flag )  
  opi_ols_idx ue(v) 
 if( opi_htid_info_present_flag )  
  opi_htid_plus1 u(3) 
 opi_extension_flag u(1) 
 if( opi_extension_flag )  
  while( more_rbsp_data() )  
   opi_extension_data_flag u(1) 
 rbsp_trailing_bits()  
}  

video_parameter_set_rbsp() {  
 vps_video_parameter_set_id u(4)
  vps_max_layers_minus1 u(6) 
 vps_max_sublayers_minus1 u(3) 
 if( vps_max_layers_minus1 > 0  &&  vps_max_sublayers_minus1 > 0 )  
  vps_default_ptl_dpb_hrd_max_tid_flag u(1) 
 if( vps_max_layers_minus1 > 0 )  
  vps_all_independent_layers_flag u(1) 
 for( i = 0; i  <=  vps_max_layers_minus1; i++ ) {  
  vps_layer_id[ i ] u(6) 
  if( i > 0  &&  !vps_all_independent_layers_flag ) {  
   vps_independent_layer_flag[ i ] u(1) 
   if( !vps_independent_layer_flag[ i ] ) {  
    vps_max_tid_ref_present_flag[ i ] u(1) 
    for( j = 0; j < i; j++ ) {  
     vps_direct_ref_layer_flag[ i ][ j ] u(1) 
     if( vps_max_tid_ref_present_flag[ i ]  &&  vps_direct_ref_layer_flag[ i ][ j ] )  
      vps_max_tid_il_ref_pics_plus1[ i ][ j ] u(3) 
    }  
   }  
  }  
 }  
 if( vps_max_layers_minus1 > 0 ) {  
  if( vps_all_independent_layers_flag )  
   vps_each_layer_is_an_ols_flag u(1) 
  if( !vps_each_layer_is_an_ols_flag ) {  
   if( !vps_all_independent_layers_flag )  
    vps_ols_mode_idc u(2) 
   if( vps_ols_mode_idc  ==  2 ) {  
    vps_num_output_layer_sets_minus2 u(8) 
    for( i = 1; i  <=  vps_num_output_layer_sets_minus2 + 1; i ++ )  
     for( j = 0; j  <=  vps_max_layers_minus1; j++ )  
      vps_ols_output_layer_flag[ i ][ j ] u(1) 
   }  
  }  
  vps_num_ptls_minus1 u(8) 
 }  
 for( i = 0; i  <=  vps_num_ptls_minus1; i++ ) {  
  if( i > 0 )  
   vps_pt_present_flag[ i ] u(1) 
  if( !vps_default_ptl_dpb_hrd_max_tid_flag )  
   vps_ptl_max_tid[ i ] u(3) 
 }  
 while( !byte_aligned() )  
  vps_ptl_alignment_zero_bit  /* equal to 0 */ f(1) 
 for( i = 0; i  <=  vps_num_ptls_minus1; i++ )  
  profile_tier_level( vps_pt_present_flag[ i ], vps_ptl_max_tid[ i ] )  
 for( i = 0; i < TotalNumOlss; i++ )  
  if( vps_num_ptls_minus1 > 0  &&  vps_num_ptls_minus1 + 1  !=  TotalNumOlss )  
   vps_ols_ptl_idx[ i ] u(8) 
    if( !vps_each_layer_is_an_ols_flag ) {  
  vps_num_dpb_params_minus1 ue(v) 
  if( vps_max_sublayers_minus1 > 0 )  
   vps_sublayer_dpb_params_present_flag u(1) 
  for( i = 0; i < VpsNumDpbParams; i++ ) {  
   if( !vps_default_ptl_dpb_hrd_max_tid_flag )  
    vps_dpb_max_tid[ i ] u(3) 
   dpb_parameters( vps_dpb_max_tid[ i ], 
     vps_sublayer_dpb_params_present_flag ) 
 
  }  
  for( i = 0; i < NumMultiLayerOlss; i++ ) {  
   vps_ols_dpb_pic_width[ i ] ue(v) 
   vps_ols_dpb_pic_height[ i ] ue(v) 
   vps_ols_dpb_chroma_format[ i ] u(2) 
   vps_ols_dpb_bitdepth_minus8[ i ] ue(v) 
   if( VpsNumDpbParams > 1  &&  VpsNumDpbParams  !=  NumMultiLayerOlss )  
    vps_ols_dpb_params_idx[ i ] ue(v) 
  }  
  vps_timing_hrd_params_present_flag u(1) 
  if( vps_timing_hrd_params_present_flag ) {  
   general_timing_hrd_parameters()  
   if( vps_max_sublayers_minus1 > 0 )  
    vps_sublayer_cpb_params_present_flag u(1) 
   vps_num_ols_timing_hrd_params_minus1 ue(v) 
   for( i = 0; i  <=  vps_num_ols_timing_hrd_params_minus1; i++ ) {  
    if( !vps_default_ptl_dpb_hrd_max_tid_flag )  
     vps_hrd_max_tid[ i ] u(3) 
    firstSubLayer = vps_sublayer_cpb_params_present_flag ? 0 : vps_hrd_max_tid[ i ]  
    ols_timing_hrd_parameters( firstSubLayer, vps_hrd_max_tid[ i ] )  
   }  
   if( vps_num_ols_timing_hrd_params_minus1 > 0  && 
     vps_num_ols_timing_hrd_params_minus1 + 1  !=  NumMultiLayerOlss ) 
 
    for( i = 0; i < NumMultiLayerOlss; i++ )  
     vps_ols_timing_hrd_idx[ i ] ue(v) 
  }  
 }  
 vps_extension_flag u(1) 
 if( vps_extension_flag )  
  while( more_rbsp_data() )  
   vps_extension_data_flag u(1) 
 rbsp_trailing_bits()  
} 

seq_parameter_set_rbsp() {  
 sps_seq_parameter_set_id u(4) 
 sps_video_parameter_set_id u(4)
 sps_max_sublayers_minus1 u(3) 
 sps_chroma_format_idc u(2) 
 sps_log2_ctu_size_minus5 u(2) 
 sps_ptl_dpb_hrd_params_present_flag u(1) 
 if( sps_ptl_dpb_hrd_params_present_flag )  
  profile_tier_level( 1, sps_max_sublayers_minus1 )  
 sps_gdr_enabled_flag u(1) 
 sps_ref_pic_resampling_enabled_flag u(1) 
 if( sps_ref_pic_resampling_enabled_flag )  
  sps_res_change_in_clvs_allowed_flag u(1) 
 sps_pic_width_max_in_luma_samples ue(v) 
 sps_pic_height_max_in_luma_samples ue(v) 
 sps_conformance_window_flag u(1) 
 if( sps_conformance_window_flag ) {  
  sps_conf_win_left_offset ue(v) 
  sps_conf_win_right_offset ue(v) 
  sps_conf_win_top_offset ue(v) 
  sps_conf_win_bottom_offset ue(v) 
 }  
 sps_subpic_info_present_flag u(1) 
 if( sps_subpic_info_present_flag ) {  
  sps_num_subpics_minus1 ue(v) 
  if( sps_num_subpics_minus1 > 0 ) {  
   sps_independent_subpics_flag u(1) 
   sps_subpic_same_size_flag u(1) 
  }  
  for( i = 0; sps_num_subpics_minus1 > 0  &&  i  <=  sps_num_subpics_minus1; i++ ) {  
   if( !sps_subpic_same_size_flag  ||  i  ==  0 ) {  
    if( i > 0  &&  sps_pic_width_max_in_luma_samples > CtbSizeY )  
     sps_subpic_ctu_top_left_x[ i ] u(v) 
    if( i > 0  &&  sps_pic_height_max_in_luma_samples > CtbSizeY )  
     sps_subpic_ctu_top_left_y[ i ] u(v) 
    if( i < sps_num_subpics_minus1  && 
      sps_pic_width_max_in_luma_samples > CtbSizeY ) 
 
     sps_subpic_width_minus1[ i ] u(v) 
    if( i < sps_num_subpics_minus1  && 
      sps_pic_height_max_in_luma_samples > CtbSizeY ) 
 
     sps_subpic_height_minus1[ i ] u(v) 
   }  
   if( !sps_independent_subpics_flag) {  
    sps_subpic_treated_as_pic_flag[ i ] u(1) 
    sps_loop_filter_across_subpic_enabled_flag[ i ] u(1) 
   }  
  }  
  sps_subpic_id_len_minus1 ue(v) 
  sps_subpic_id_mapping_explicitly_signalled_flag u(1) 
  if( sps_subpic_id_mapping_explicitly_signalled_flag ) {  
   sps_subpic_id_mapping_present_flag u(1) 
   if( sps_subpic_id_mapping_present_flag )  
    for( i = 0; i  <=   sps_num_subpics_minus1; i++ )  
     sps_subpic_id[ i ] u(v) 
  }  
 }  
 sps_bitdepth_minus8 ue(v) 
 sps_entropy_coding_sync_enabled_flag u(1) 
 sps_entry_point_offsets_present_flag u(1) 
 sps_log2_max_pic_order_cnt_lsb_minus4 u(4) 
 sps_poc_msb_cycle_flag u(1) 
 if( sps_poc_msb_cycle_flag )  
  sps_poc_msb_cycle_len_minus1 ue(v) 
 sps_num_extra_ph_bytes u(2) 
 for( i = 0; i < (sps_num_extra_ph_bytes * 8 ); i++ )  
  sps_extra_ph_bit_present_flag[ i ] u(1) 
 sps_num_extra_sh_bytes u(2) 
 for( i = 0; i < (sps_num_extra_sh_bytes * 8 ); i++ )  
  sps_extra_sh_bit_present_flag[ i ] u(1) 
 if( sps_ptl_dpb_hrd_params_present_flag ) {  
  if( sps_max_sublayers_minus1 > 0 )  
   sps_sublayer_dpb_params_flag u(1) 
  dpb_parameters( sps_max_sublayers_minus1, sps_sublayer_dpb_params_flag )  
 }  
 sps_log2_min_luma_coding_block_size_minus2 ue(v) 
 sps_partition_constraints_override_enabled_flag u(1) 
 sps_log2_diff_min_qt_min_cb_intra_slice_luma ue(v) 
 sps_max_mtt_hierarchy_depth_intra_slice_luma ue(v) 
 if( sps_max_mtt_hierarchy_depth_intra_slice_luma  !=  0 ) {  
  sps_log2_diff_max_bt_min_qt_intra_slice_luma ue(v) 
  sps_log2_diff_max_tt_min_qt_intra_slice_luma ue(v) 
 }  
 if( sps_chroma_format_idc  !=  0 )  
  sps_qtbtt_dual_tree_intra_flag u(1) 
 if( sps_qtbtt_dual_tree_intra_flag ) {  
  sps_log2_diff_min_qt_min_cb_intra_slice_chroma ue(v) 
  sps_max_mtt_hierarchy_depth_intra_slice_chroma ue(v) 
  if( sps_max_mtt_hierarchy_depth_intra_slice_chroma  !=  0 ) {  
   sps_log2_diff_max_bt_min_qt_intra_slice_chroma ue(v) 
   sps_log2_diff_max_tt_min_qt_intra_slice_chroma ue(v) 
  }  
 }  
 sps_log2_diff_min_qt_min_cb_inter_slice ue(v) 
 sps_max_mtt_hierarchy_depth_inter_slice ue(v) 
 if( sps_max_mtt_hierarchy_depth_inter_slice  !=  0 ) {  
  sps_log2_diff_max_bt_min_qt_inter_slice ue(v) 
  sps_log2_diff_max_tt_min_qt_inter_slice ue(v) 
 }  
 if( CtbSizeY > 32 )  
  sps_max_luma_transform_size_64_flag u(1) 
 sps_transform_skip_enabled_flag u(1) 
 if( sps_transform_skip_enabled_flag ) {  
  sps_log2_transform_skip_max_size_minus2 ue(v) 
  sps_bdpcm_enabled_flag u(1) 
 }  
 sps_mts_enabled_flag u(1) 
 if( sps_mts_enabled_flag ) {  
  sps_explicit_mts_intra_enabled_flag u(1) 
  sps_explicit_mts_inter_enabled_flag u(1) 
 }  
 sps_lfnst_enabled_flag u(1) 
 if( sps_chroma_format_idc  !=  0 ) {  
  sps_joint_cbcr_enabled_flag u(1) 
  sps_same_qp_table_for_chroma_flag u(1) 
  numQpTables = sps_same_qp_table_for_chroma_flag ? 1 : ( sps_joint_cbcr_enabled_flag ? 3 : 2 ) 
 
  for( i = 0; i < numQpTables; i++ ) {  
   sps_qp_table_start_minus26[ i ] se(v) 
   sps_num_points_in_qp_table_minus1[ i ] ue(v) 
   for( j = 0; j  <=  sps_num_points_in_qp_table_minus1[ i ]; j++ ) {  
    sps_delta_qp_in_val_minus1[ i ][ j ] ue(v) 
    sps_delta_qp_diff_val[ i ][ j ] ue(v) 
   }  
  }  
 }  
 sps_sao_enabled_flag u(1) 
 sps_alf_enabled_flag u(1) 
 if( sps_alf_enabled_flag  &&  sps_chroma_format_idc  !=  0 )  
  sps_ccalf_enabled_flag u(1) 
 sps_lmcs_enabled_flag u(1) 
 sps_weighted_pred_flag u(1) 
 sps_weighted_bipred_flag u(1) 
 sps_long_term_ref_pics_flag u(1) 
 if( sps_video_parameter_set_id > 0 )  
  sps_inter_layer_prediction_enabled_flag u(1) 
 sps_idr_rpl_present_flag u(1) 
 sps_rpl1_same_as_rpl0_flag u(1) 
 for( i = 0; i < ( sps_rpl1_same_as_rpl0_flag ? 1 : 2 ); i++ ) {  
  sps_num_ref_pic_lists[ i ] ue(v) 
  for( j = 0; j < sps_num_ref_pic_lists[ i ]; j++)  
   ref_pic_list_struct( i, j )  
 }  
 sps_ref_wraparound_enabled_flag u(1) 
 sps_temporal_mvp_enabled_flag u(1) 
 if( sps_temporal_mvp_enabled_flag )  
  sps_sbtmvp_enabled_flag u(1)
 sps_amvr_enabled_flag u(1) 
 sps_bdof_enabled_flag u(1) 
 if( sps_bdof_enabled_flag )  
  sps_bdof_control_present_in_ph_flag u(1) 
 sps_smvd_enabled_flag u(1) 
 sps_dmvr_enabled_flag u(1) 
 if( sps_dmvr_enabled_flag)  
  sps_dmvr_control_present_in_ph_flag u(1) 
 sps_mmvd_enabled_flag u(1) 
 if( sps_mmvd_enabled_flag )  
  sps_mmvd_fullpel_only_enabled_flag u(1) 
 sps_six_minus_max_num_merge_cand ue(v) 
 sps_sbt_enabled_flag u(1) 
 sps_affine_enabled_flag u(1) 
 if( sps_affine_enabled_flag ) {  
  sps_five_minus_max_num_subblock_merge_cand ue(v) 
  sps_6param_affine_enabled_flag u(1) 
  if( sps_amvr_enabled_flag )  
   sps_affine_amvr_enabled_flag u(1) 
  sps_affine_prof_enabled_flag u(1) 
  if( sps_affine_prof_enabled_flag )  
   sps_prof_control_present_in_ph_flag u(1) 
 }  
 sps_bcw_enabled_flag u(1) 
 sps_ciip_enabled_flag u(1) 
 if( MaxNumMergeCand  >=  2 ) {  
  sps_gpm_enabled_flag u(1) 
  if( sps_gpm_enabled_flag  &&  MaxNumMergeCand  >=  3 )  
   sps_max_num_merge_cand_minus_max_num_gpm_cand ue(v) 
 }  
 sps_log2_parallel_merge_level_minus2 ue(v) 
 sps_isp_enabled_flag u(1) 
 sps_mrl_enabled_flag u(1) 
 sps_mip_enabled_flag u(1) 
 if( sps_chroma_format_idc  !=  0 )  
  sps_cclm_enabled_flag u(1) 
 if( sps_chroma_format_idc  ==  1 ) {  
  sps_chroma_horizontal_collocated_flag u(1) 
  sps_chroma_vertical_collocated_flag u(1) 
 }  
 sps_palette_enabled_flag u(1) 
 if( sps_chroma_format_idc  ==  3  &&  !sps_max_luma_transform_size_64_flag )  
  sps_act_enabled_flag u(1) 
 if( sps_transform_skip_enabled_flag  ||  sps_palette_enabled_flag )  
  sps_min_qp_prime_ts ue(v) 
 sps_ibc_enabled_flag u(1) 
 if( sps_ibc_enabled_flag )  
  sps_six_minus_max_num_ibc_merge_cand ue(v) 
 sps_ladf_enabled_flag u(1) 
 if( sps_ladf_enabled_flag ) {  
  sps_num_ladf_intervals_minus2 u(2) 
  sps_ladf_lowest_interval_qp_offset se(v) 
  for( i = 0; i < sps_num_ladf_intervals_minus2 + 1; i++ ) {  
   sps_ladf_qp_offset[ i ] se(v) 
   sps_ladf_delta_threshold_minus1[ i ] ue(v) 
  }  
 }  
 sps_explicit_scaling_list_enabled_flag u(1) 
 if( sps_lfnst_enabled_flag  &&  sps_explicit_scaling_list_enabled_flag )  
  sps_scaling_matrix_for_lfnst_disabled_flag u(1) 
 if( sps_act_enabled_flag  &&  sps_explicit_scaling_list_enabled_flag )  
  sps_scaling_matrix_for_alternative_colour_space_disabled_flag u(1) 
 if( sps_scaling_matrix_for_alternative_colour_space_disabled_flag )  
  sps_scaling_matrix_designated_colour_space_flag u(1) 
 sps_dep_quant_enabled_flag u(1) 
 sps_sign_data_hiding_enabled_flag u(1) 
 sps_virtual_boundaries_enabled_flag u(1) 
 if( sps_virtual_boundaries_enabled_flag ) {  
  sps_virtual_boundaries_present_flag u(1) 
  if( sps_virtual_boundaries_present_flag ) {  
   sps_num_ver_virtual_boundaries ue(v) 
   for( i = 0; i < sps_num_ver_virtual_boundaries; i++ )  
    sps_virtual_boundary_pos_x_minus1[ i ] ue(v) 
   sps_num_hor_virtual_boundaries ue(v) 
   for( i = 0; i < sps_num_hor_virtual_boundaries; i++ )  
    sps_virtual_boundary_pos_y_minus1[ i ] ue(v) 
  }  
 }  
 if( sps_ptl_dpb_hrd_params_present_flag ) {  
  sps_timing_hrd_params_present_flag u(1) 
  if( sps_timing_hrd_params_present_flag ) {  
   general_timing_hrd_parameters()  
   if( sps_max_sublayers_minus1 > 0 )  
    sps_sublayer_cpb_params_present_flag u(1) 
   firstSubLayer = sps_sublayer_cpb_params_present_flag ? 0 : sps_max_sublayers_minus1 
 
   ols_timing_hrd_parameters( firstSubLayer, sps_max_sublayers_minus1 )  
  }  
 }  
 sps_field_seq_flag u(1) 
 sps_vui_parameters_present_flag u(1) 
 if( sps_vui_parameters_present_flag ) {  
  sps_vui_payload_size_minus1 ue(v) 
  while( !byte_aligned() )  
   sps_vui_alignment_zero_bit f(1) 
  vui_payload( sps_vui_payload_size_minus1 + 1 )  
 }  
 sps_extension_flag u(1) 
 if( sps_extension_flag )  
  while( more_rbsp_data() )  
   sps_extension_data_flag u(1) 
 rbsp_trailing_bits()  
}  

pic_parameter_set_rbsp() {  
 pps_pic_parameter_set_id u(6) 
 pps_seq_parameter_set_id u(4) 
 pps_mixed_nalu_types_in_pic_flag u(1) 
 pps_pic_width_in_luma_samples ue(v) 
 pps_pic_height_in_luma_samples ue(v) 
 pps_conformance_window_flag u(1) 
 if( pps_conformance_window_flag ) {  
  pps_conf_win_left_offset ue(v) 
  pps_conf_win_right_offset ue(v) 
  pps_conf_win_top_offset ue(v) 
  pps_conf_win_bottom_offset ue(v) 
 }  
 pps_scaling_window_explicit_signalling_flag u(1) 
 if( pps_scaling_window_explicit_signalling_flag ) {  
  pps_scaling_win_left_offset se(v) 
  pps_scaling_win_right_offset se(v) 
  pps_scaling_win_top_offset se(v) 
  pps_scaling_win_bottom_offset se(v) 
 }  
 pps_output_flag_present_flag u(1) 
 pps_no_pic_partition_flag u(1) 
 pps_subpic_id_mapping_present_flag u(1) 
 /*
 if( pps_subpic_id_mapping_present_flag ) {  
  if( !pps_no_pic_partition_flag )  
   pps_num_subpics_minus1 ue(v) 
  pps_subpic_id_len_minus1 ue(v) 
  for( i = 0; i  <=  pps_num_subpics_minus1; i++ )  
   pps_subpic_id[ i ] u(v) 
    }  
 if( !pps_no_pic_partition_flag ) {  
  pps_log2_ctu_size_minus5 u(2) 
  pps_num_exp_tile_columns_minus1 ue(v) 
  pps_num_exp_tile_rows_minus1 ue(v) 
  for( i = 0; i  <=  pps_num_exp_tile_columns_minus1; i++ )  
     pps_tile_column_width_minus1[ i ] ue(v) 
  for( i = 0; i  <=  pps_num_exp_tile_rows_minus1; i++ )  
   pps_tile_row_height_minus1[ i ] ue(v) 
  if( NumTilesInPic > 1 ) {  
   pps_loop_filter_across_tiles_enabled_flag u(1) 
   pps_rect_slice_flag u(1) 
  }  
  if( pps_rect_slice_flag )  
   pps_single_slice_per_subpic_flag u(1) 
  if( pps_rect_slice_flag  &&  !pps_single_slice_per_subpic_flag ) {  
   pps_num_slices_in_pic_minus1 ue(v) 
   if( pps_num_slices_in_pic_minus1 > 1 )  
    pps_tile_idx_delta_present_flag u(1) 
   for( i = 0; i < pps_num_slices_in_pic_minus1; i++ ) {  
    if( SliceTopLeftTileIdx[ i ] % NumTileColumns  !=  NumTileColumns - 1 )  
     pps_slice_width_in_tiles_minus1[ i ] ue(v) 
    if( SliceTopLeftTileIdx[ i ] / NumTileColumns  !=  NumTileRows - 1  && 
      ( pps_tile_idx_delta_present_flag  || 
      SliceTopLeftTileIdx[ i ] % NumTileColumns  ==  0 ) ) 
 
     pps_slice_height_in_tiles_minus1[ i ] ue(v) 
    if( pps_slice_width_in_tiles_minus1[ i ]  ==  0  && 
      pps_slice_height_in_tiles_minus1[ i ]  ==  0  && 
      RowHeightVal[ SliceTopLeftTileIdx[ i ] / NumTileColumns ] > 1 ) { 
 
     pps_num_exp_slices_in_tile[ i ] ue(v) 
     for( j = 0; j < pps_num_exp_slices_in_tile[ i ]; j++ )  
      pps_exp_slice_height_in_ctus_minus1[ i ][ j ] ue(v) 
     i  +=  NumSlicesInTile[ i ] - 1  
    }  
    if( pps_tile_idx_delta_present_flag  &&  i < pps_num_slices_in_pic_minus1 )  
     pps_tile_idx_delta_val[ i ] se(v) 
   }  
  }  
  if( !pps_rect_slice_flag  ||  pps_single_slice_per_subpic_flag  || 
    pps_num_slices_in_pic_minus1 > 0 ) 
 
   pps_loop_filter_across_slices_enabled_flag u(1) 
 }  
 pps_cabac_init_present_flag u(1) 
 for( i = 0; i < 2; i++ )  
  pps_num_ref_idx_default_active_minus1[ i ] ue(v) 
 pps_rpl1_idx_present_flag u(1) 
 pps_weighted_pred_flag u(1) 
 pps_weighted_bipred_flag u(1) 
 pps_ref_wraparound_enabled_flag u(1) 
 if( pps_ref_wraparound_enabled_flag )  
  pps_pic_width_minus_wraparound_offset ue(v) 
 pps_init_qp_minus26 se(v) 
 pps_cu_qp_delta_enabled_flag u(1) 
 pps_chroma_tool_offsets_present_flag u(1) 
 if( pps_chroma_tool_offsets_present_flag ) {  
   pps_cb_qp_offset se(v) 
  pps_cr_qp_offset se(v) 
  pps_joint_cbcr_qp_offset_present_flag u(1) 
  if( pps_joint_cbcr_qp_offset_present_flag )  
   pps_joint_cbcr_qp_offset_value se(v) 
  pps_slice_chroma_qp_offsets_present_flag u(1) 
  pps_cu_chroma_qp_offset_list_enabled_flag u(1) 
  if( pps_cu_chroma_qp_offset_list_enabled_flag ) {  
   pps_chroma_qp_offset_list_len_minus1 ue(v) 
   for( i = 0; i  <=  pps_chroma_qp_offset_list_len_minus1; i++ ) {  
    pps_cb_qp_offset_list[ i ] se(v) 
    pps_cr_qp_offset_list[ i ] se(v) 
    if( pps_joint_cbcr_qp_offset_present_flag )  
     pps_joint_cbcr_qp_offset_list[ i ] se(v) 
   }  
  }  
 }  
 pps_deblocking_filter_control_present_flag u(1) 
 if( pps_deblocking_filter_control_present_flag ) {  
  pps_deblocking_filter_override_enabled_flag u(1) 
  pps_deblocking_filter_disabled_flag u(1) 
  if( !pps_no_pic_partition_flag  &&  pps_deblocking_filter_override_enabled_flag )  
   pps_dbf_info_in_ph_flag u(1) 
  if( !pps_deblocking_filter_disabled_flag ) {  
   pps_luma_beta_offset_div2 se(v) 
   pps_luma_tc_offset_div2 se(v) 
   if( pps_chroma_tool_offsets_present_flag ) {  
    pps_cb_beta_offset_div2 se(v) 
    pps_cb_tc_offset_div2 se(v) 
    pps_cr_beta_offset_div2 se(v) 
    pps_cr_tc_offset_div2 se(v) 
   }  
  }  
 }  
 if( !pps_no_pic_partition_flag ) {  
  pps_rpl_info_in_ph_flag u(1) 
  pps_sao_info_in_ph_flag u(1) 
  pps_alf_info_in_ph_flag u(1) 
  if( ( pps_weighted_pred_flag  ||  pps_weighted_bipred_flag )  && 
    pps_rpl_info_in_ph_flag ) 
 
   pps_wp_info_in_ph_flag u(1) 
  pps_qp_delta_info_in_ph_flag u(1) 
 }  
 pps_picture_header_extension_present_flag u(1) 
 pps_slice_header_extension_present_flag u(1) 
 pps_extension_flag u(1) 
 if( pps_extension_flag )  
  while( more_rbsp_data() )  
   pps_extension_data_flag u(1) 
    rbsp_trailing_bits()  
    */
}  

adaptation_parameter_set_rbsp() {  
 aps_params_type u(3) 
 aps_adaptation_parameter_set_id u(5) 
 aps_chroma_present_flag u(1) 
 if( aps_params_type  ==  ALF_APS )  
  alf_data()  
 else if( aps_params_type  ==  LMCS_APS )  
  lmcs_data()  
 /* else if( aps_params_type  ==  SCALING_APS ) */
 /* scaling_list_data()  */
 /*aps_extension_flag u(1) */
 /*if( aps_extension_flag )  */
  /*while( more_rbsp_data() )  */
  /* aps_extension_data_flag u(1) */
 /*rbsp_trailing_bits()  */
} 

picture_header_rbsp() {  
 picture_header_structure()  
 rbsp_trailing_bits()  
}  

picture_header_structure() {  
 ph_gdr_or_irap_pic_flag u(1) 
 ph_non_ref_pic_flag u(1) 
 if( ph_gdr_or_irap_pic_flag )  
  ph_gdr_pic_flag u(1) 
 ph_inter_slice_allowed_flag u(1) 
 if( ph_inter_slice_allowed_flag )  
  ph_intra_slice_allowed_flag u(1) 
 ph_pic_parameter_set_id ue(v) 
 ph_pic_order_cnt_lsb u(v) 
 if( ph_gdr_pic_flag )  
  ph_recovery_poc_cnt ue(v) 
 for( i = 0; i < NumExtraPhBits; i++ )  
  ph_extra_bit[ i ] u(1) 
 if( sps_poc_msb_cycle_flag ) {  
   ph_poc_msb_cycle_present_flag u(1) 
  if( ph_poc_msb_cycle_present_flag )  
   ph_poc_msb_cycle_val u(v) 
    }  
 /*
 if( sps_alf_enabled_flag  &&  pps_alf_info_in_ph_flag ) {  
  ph_alf_enabled_flag u(1) 
  if( ph_alf_enabled_flag ) {  
   ph_num_alf_aps_ids_luma u(3) 
   for( i = 0; i < ph_num_alf_aps_ids_luma; i++ )  
    ph_alf_aps_id_luma[ i ] u(3) 
   if( sps_chroma_format_idc  !=  0 ) {  
    ph_alf_cb_enabled_flag u(1) 
    ph_alf_cr_enabled_flag u(1) 
   }  
   if( ph_alf_cb_enabled_flag  ||  ph_alf_cr_enabled_flag )  
    ph_alf_aps_id_chroma u(3) 
   if( sps_ccalf_enabled_flag ) {  
    ph_alf_cc_cb_enabled_flag u(1) 
    if( ph_alf_cc_cb_enabled_flag )  
     ph_alf_cc_cb_aps_id u(3) 
    ph_alf_cc_cr_enabled_flag u(1) 
    if( ph_alf_cc_cr_enabled_flag )  
     ph_alf_cc_cr_aps_id u(3) 
   }  
  }  
 }  
 if( sps_lmcs_enabled_flag ) {  
  ph_lmcs_enabled_flag u(1) 
  if( ph_lmcs_enabled_flag ) {  
   ph_lmcs_aps_id u(2) 
   if( sps_chroma_format_idc  !=  0 )  
    ph_chroma_residual_scale_flag u(1) 
  }  
 }  
 if( sps_explicit_scaling_list_enabled_flag ) {  
  ph_explicit_scaling_list_enabled_flag u(1) 
  if( ph_explicit_scaling_list_enabled_flag )  
   ph_scaling_list_aps_id u(3) 
 }  
 if( sps_virtual_boundaries_enabled_flag  &&  !sps_virtual_boundaries_present_flag ) {  
  ph_virtual_boundaries_present_flag u(1) 
  if( ph_virtual_boundaries_present_flag ) {  
   ph_num_ver_virtual_boundaries ue(v) 
   for( i = 0; i < ph_num_ver_virtual_boundaries; i++ )  
    ph_virtual_boundary_pos_x_minus1[ i ] ue(v) 
   ph_num_hor_virtual_boundaries ue(v) 
   for( i = 0; i < ph_num_hor_virtual_boundaries; i++ )  
    ph_virtual_boundary_pos_y_minus1[ i ] ue(v) 
      }  
 }  
 if( pps_output_flag_present_flag  &&  !ph_non_ref_pic_flag )  
  ph_pic_output_flag u(1) 
 if( pps_rpl_info_in_ph_flag )  
  ref_pic_lists()  
 if( sps_partition_constraints_override_enabled_flag )  
  ph_partition_constraints_override_flag u(1) 
 if( ph_intra_slice_allowed_flag ) {  
  if( ph_partition_constraints_override_flag ) {  
   ph_log2_diff_min_qt_min_cb_intra_slice_luma ue(v) 
   ph_max_mtt_hierarchy_depth_intra_slice_luma ue(v) 
   if( ph_max_mtt_hierarchy_depth_intra_slice_luma  !=  0 ) {  
    ph_log2_diff_max_bt_min_qt_intra_slice_luma ue(v) 
    ph_log2_diff_max_tt_min_qt_intra_slice_luma ue(v) 
   }  
   if( sps_qtbtt_dual_tree_intra_flag ) {   
    ph_log2_diff_min_qt_min_cb_intra_slice_chroma ue(v) 
    ph_max_mtt_hierarchy_depth_intra_slice_chroma ue(v) 
    if( ph_max_mtt_hierarchy_depth_intra_slice_chroma  !=  0 ) {  
     ph_log2_diff_max_bt_min_qt_intra_slice_chroma ue(v) 
     ph_log2_diff_max_tt_min_qt_intra_slice_chroma ue(v) 
    }  
   }  
  }  
  if( pps_cu_qp_delta_enabled_flag )  
   ph_cu_qp_delta_subdiv_intra_slice ue(v) 
  if( pps_cu_chroma_qp_offset_list_enabled_flag )  
   ph_cu_chroma_qp_offset_subdiv_intra_slice ue(v) 
 }  
 if( ph_inter_slice_allowed_flag ) {  
  if( ph_partition_constraints_override_flag ) {  
   ph_log2_diff_min_qt_min_cb_inter_slice ue(v) 
   ph_max_mtt_hierarchy_depth_inter_slice ue(v) 
   if( ph_max_mtt_hierarchy_depth_inter_slice  !=  0 ) {  
    ph_log2_diff_max_bt_min_qt_inter_slice ue(v) 
    ph_log2_diff_max_tt_min_qt_inter_slice ue(v) 
   }  
  }  
  if( pps_cu_qp_delta_enabled_flag )  
   ph_cu_qp_delta_subdiv_inter_slice ue(v) 
  if( pps_cu_chroma_qp_offset_list_enabled_flag )  
   ph_cu_chroma_qp_offset_subdiv_inter_slice ue(v) 
  if( sps_temporal_mvp_enabled_flag ) {  
   ph_temporal_mvp_enabled_flag u(1) 
   if( ph_temporal_mvp_enabled_flag  &&  pps_rpl_info_in_ph_flag ) {  
    if( num_ref_entries[ 1 ][ RplsIdx[ 1 ] ] > 0 )  
     ph_collocated_from_l0_flag u(1) 
         if( ( ph_collocated_from_l0_flag  && 
      num_ref_entries[ 0 ][ RplsIdx[ 0 ] ] > 1 )  || 
      ( !ph_collocated_from_l0_flag  && 
      num_ref_entries[ 1 ][ RplsIdx[ 1 ] ] > 1 ) ) 
 
     ph_collocated_ref_idx ue(v) 
   }  
  }  
  if( sps_mmvd_fullpel_only_enabled_flag )  
   ph_mmvd_fullpel_only_flag u(1) 
  presenceFlag = 0  
  if( !pps_rpl_info_in_ph_flag )*/ /* This condition is intentionally not merged into the next, 
    to avoid possible interpretation of RplsIdx[ i ] not having a specified value. */ 
 /*
   presenceFlag = 1  
  else if( num_ref_entries[ 1 ][ RplsIdx[ 1 ] ] > 0 )  
   presenceFlag = 1  
  if( presenceFlag ) {  
   ph_mvd_l1_zero_flag u(1) 
   if( sps_bdof_control_present_in_ph_flag )  
    ph_bdof_disabled_flag u(1) 
   if( sps_dmvr_control_present_in_ph_flag )  
    ph_dmvr_disabled_flag u(1) 
  }  
  if( sps_prof_control_present_in_ph_flag )  
   ph_prof_disabled_flag u(1) 
  if( ( pps_weighted_pred_flag  ||  pps_weighted_bipred_flag )  && 
    pps_wp_info_in_ph_flag ) 
 
   pred_weight_table()  
 }  
 if( pps_qp_delta_info_in_ph_flag )  
  ph_qp_delta se(v) 
 if( sps_joint_cbcr_enabled_flag )  
  ph_joint_cbcr_sign_flag u(1) 
 if( sps_sao_enabled_flag  &&  pps_sao_info_in_ph_flag ) {  
  ph_sao_luma_enabled_flag u(1) 
  if( sps_chroma_format_idc  !=  0 )  
   ph_sao_chroma_enabled_flag u(1) 
 }  
 if( pps_dbf_info_in_ph_flag ) {  
  ph_deblocking_params_present_flag u(1) 
  if( ph_deblocking_params_present_flag ) {  
   if( !pps_deblocking_filter_disabled_flag )  
    ph_deblocking_filter_disabled_flag u(1) 
   if( !ph_deblocking_filter_disabled_flag ) {  
    ph_luma_beta_offset_div2 se(v) 
    ph_luma_tc_offset_div2 se(v) 
    if( pps_chroma_tool_offsets_present_flag ) {  
     ph_cb_beta_offset_div2 se(v) 
     ph_cb_tc_offset_div2 se(v) 
     ph_cr_beta_offset_div2 se(v) 

     ph_cr_tc_offset_div2 se(v)
    }  
   }  
  }  
}  
if( pps_picture_header_extension_present_flag ) {  
ph_extension_length ue(v) 
for( i = 0; i < ph_extension_length; i++)   
ph_extension_data_byte[ i ] u(8) 
}  
*/
}  

sei_rbsp() { 
do  
sei_message()  
while( more_rbsp_data() )  
rbsp_trailing_bits()  
} 

access_unit_delimiter_rbsp() {  
aud_irap_or_gdr_flag u(1) 
aud_pic_type u(3) 
rbsp_trailing_bits()  
}  

end_of_seq_rbsp() { 
}  

end_of_bitstream_rbsp() { 
}

filler_data_rbsp() { 
while( next_bits( 8 )  ==  0xFF )  
fd_ff_byte  /* equal to 0xFF */ f(8)
rbsp_trailing_bits()  
}  

rbsp_trailing_bits() { 
rbsp_stop_one_bit  /* equal to 1 */ f(1) 
while( !byte_aligned() )  
rbsp_alignment_zero_bit  /* equal to 0 */ f(1)
}  

byte_alignment() { 
byte_alignment_bit_equal_to_one  /* equal to 1 */ f(1) 
while( !byte_aligned() )  
byte_alignment_bit_equal_to_zero  /* equal to 0 */ f(1)
}  

alf_data() { 
alf_luma_filter_signal_flag  u(1)
if( aps_chroma_present_flag ) {  
alf_chroma_filter_signal_flag u(1)
alf_cc_cb_filter_signal_flag u(1)
alf_cc_cr_filter_signal_flag u(1)
}  

 if( alf_luma_filter_signal_flag ) {  
  alf_luma_clip_flag u(1) 
  alf_luma_num_filters_signalled_minus1 ue(v) 
  if( alf_luma_num_filters_signalled_minus1 > 0 )  
   for( filtIdx = 0; filtIdx < NumAlfFilters; filtIdx++ )  
    alf_luma_coeff_delta_idx[ filtIdx ] u(v) 
  for( sfIdx = 0; sfIdx  <=  alf_luma_num_filters_signalled_minus1; sfIdx++ )  
   for( j = 0; j < 12; j++ ) {  
    alf_luma_coeff_abs[ sfIdx ][ j ] ue(v) 
    if( alf_luma_coeff_abs[ sfIdx ][ j ] )  
     alf_luma_coeff_sign[ sfIdx ][ j ] u(1) 
   }  
  if( alf_luma_clip_flag )  
   for( sfIdx = 0; sfIdx  <=  alf_luma_num_filters_signalled_minus1; sfIdx++ )  
    for( j = 0; j < 12; j++ )  
     alf_luma_clip_idx[ sfIdx ][ j ] u(2) 
 }  
 if( alf_chroma_filter_signal_flag ) {  
  alf_chroma_clip_flag u(1) 
  alf_chroma_num_alt_filters_minus1 ue(v) 
  for( altIdx = 0; altIdx  <=  alf_chroma_num_alt_filters_minus1; altIdx++ ) {  
   for( j = 0; j < 6; j++ ) {  
    alf_chroma_coeff_abs[ altIdx ][ j ] ue(v) 
    if( alf_chroma_coeff_abs[ altIdx ][ j ] > 0 )  
     alf_chroma_coeff_sign[ altIdx ][ j ] u(1) 
   }  
   if( alf_chroma_clip_flag )  
    for( j = 0; j < 6; j++ )  
     alf_chroma_clip_idx[ altIdx ][ j ] u(2) 
  }  
 }  
 if( alf_cc_cb_filter_signal_flag ) {  
  alf_cc_cb_filters_signalled_minus1 ue(v) 
  for( k = 0; k < alf_cc_cb_filters_signalled_minus1 + 1; k++ ) {  
   for( j = 0; j < 7; j++ ) {  
    alf_cc_cb_mapped_coeff_abs[ k ][ j ] u(3) 
    if( alf_cc_cb_mapped_coeff_abs[ k ][ j ] )  
     alf_cc_cb_coeff_sign[ k ][ j ] u(1) 
   }  
  }  
 }  
 if( alf_cc_cr_filter_signal_flag ) {  
  alf_cc_cr_filters_signalled_minus1 ue(v) 
  for( k = 0; k < alf_cc_cr_filters_signalled_minus1 + 1; k++ ) {  
   for( j = 0; j < 7; j++ ) {  
    alf_cc_cr_mapped_coeff_abs[ k ][ j ] u(3) 
     if( alf_cc_cr_mapped_coeff_abs[ k ][ j ] )  
     alf_cc_cr_coeff_sign[ k ][ j ] u(1)
   }  
  }  
 }  
} 

lmcs_data() {  
 lmcs_min_bin_idx ue(v) 
 lmcs_delta_max_bin_idx ue(v) 
 lmcs_delta_cw_prec_minus1 ue(v) 
 for( i = lmcs_min_bin_idx; i  <=  LmcsMaxBinIdx; i++ ) {  
  lmcs_delta_abs_cw[ i ] u(v) 
  if( lmcs_delta_abs_cw[ i ] > 0 )   
   lmcs_delta_sign_cw_flag[ i ] u(1) 
 }  
 if( aps_chroma_present_flag ) {  
  lmcs_delta_abs_crs u(3) 
  if( lmcs_delta_abs_crs > 0 )  
   lmcs_delta_sign_crs_flag u(1) 
 }  
}  

vui_payload( payloadSize ) {  
 vui_parameters( payloadSize ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 */  
 if( more_data_in_payload() ) {  
  if( payload_extension_present() )  
   vui_reserved_payload_extension_data u(v) 
  vui_payload_bit_equal_to_one /* equal to 1 */ f(1) 
  while( !byte_aligned() )  
   vui_payload_bit_equal_to_zero /* equal to 0 */ f(1) 
 }  
}  

vui_parameters(payloadSize) {
    vui_progressive_source_flag u(1)
    vui_interlaced_source_flag u(1)
    vui_non_packed_constraint_flag u(1)
    vui_non_projected_constraint_flag u(1)
    vui_aspect_ratio_info_present_flag u(1)
    if (vui_aspect_ratio_info_present_flag) {
        vui_aspect_ratio_constant_flag u(1)
        vui_aspect_ratio_idc u(8)
        if (vui_aspect_ratio_idc == 255) {
            vui_sar_width u(16)
            vui_sar_height u(16)
        }
    }
    vui_overscan_info_present_flag u(1)
    if (vui_overscan_info_present_flag)  
  vui_overscan_appropriate_flag u(1) 
 vui_colour_description_present_flag u(1)
    if (vui_colour_description_present_flag) {  
  vui_colour_primaries u(8) 
  vui_transfer_characteristics u(8) 
  vui_matrix_coeffs u(8) 
  vui_full_range_flag u(1)
    }  
 vui_chroma_loc_info_present_flag u(1)
    if (vui_chroma_loc_info_present_flag) {
        if (vui_progressive_source_flag && !vui_interlaced_source_flag)  
   vui_chroma_sample_loc_type_frame ue(v) 
  else {  
   vui_chroma_sample_loc_type_top_field ue(v) 
   vui_chroma_sample_loc_type_bottom_field ue(v)
        }
    }
}  

profile_tier_level( profileTierPresentFlag, MaxNumSubLayersMinus1 ) {  
 if( profileTierPresentFlag ) {  
  general_profile_idc u(7) 
  general_tier_flag u(1) 
 }  
 general_level_idc u(8) 
 ptl_frame_only_constraint_flag u(1) 
 ptl_multilayer_enabled_flag u(1) 
 if( profileTierPresentFlag )  
  general_constraints_info()  
 for( i = MaxNumSubLayersMinus1 - 1; i  >=  0; i-- )  
  ptl_sublayer_level_present_flag[ i ] u(1) 
 while( !byte_aligned() )  
  ptl_reserved_zero_bit u(1) 
 for( i = MaxNumSubLayersMinus1 - 1; i  >=  0; i-- )  
  if( ptl_sublayer_level_present_flag[ i ] )  
   sublayer_level_idc[ i ] u(8) 
 if( profileTierPresentFlag ) {  
  ptl_num_sub_profiles u(8) 
  for( i = 0; i < ptl_num_sub_profiles; i++ )  
   general_sub_profile_idc[ i ] u(32) 
 }  
 }  

general_constraints_info() {  
 gci_present_flag u(1) 
 if( gci_present_flag ) {  
 /* general */  
  gci_intra_only_constraint_flag u(1) 
  gci_all_layers_independent_constraint_flag u(1) 
  gci_one_au_only_constraint_flag  u(1) 
 /* picture format */  
  gci_sixteen_minus_max_bitdepth_constraint_idc u(4) 
  gci_three_minus_max_chroma_format_constraint_idc u(2) 
 /* NAL unit type related */  
  gci_no_mixed_nalu_types_in_pic_constraint_flag u(1) 
  gci_no_trail_constraint_flag u(1) 
  gci_no_stsa_constraint_flag u(1) 
  gci_no_rasl_constraint_flag u(1) 
  gci_no_radl_constraint_flag u(1) 
  gci_no_idr_constraint_flag u(1) 
  gci_no_cra_constraint_flag u(1) 
  gci_no_gdr_constraint_flag u(1) 
  gci_no_aps_constraint_flag u(1) 
  gci_no_idr_rpl_constraint_flag u(1) 
 /* tile, slice, subpicture partitioning */  
  gci_one_tile_per_pic_constraint_flag u(1) 
  gci_pic_header_in_slice_header_constraint_flag u(1) 
  gci_one_slice_per_pic_constraint_flag u(1) 
  gci_no_rectangular_slice_constraint_flag u(1) 
  gci_one_slice_per_subpic_constraint_flag u(1) 
  gci_no_subpic_info_constraint_flag u(1) 
 /* CTU and block partitioning */  
  gci_three_minus_max_log2_ctu_size_constraint_idc u(2) 
  gci_no_partition_constraints_override_constraint_flag u(1) 
  gci_no_mtt_constraint_flag u(1) 
  gci_no_qtbtt_dual_tree_intra_constraint_flag u(1) 
 /* intra */  
  gci_no_palette_constraint_flag u(1) 
  gci_no_ibc_constraint_flag u(1) 
  gci_no_isp_constraint_flag u(1) 
  gci_no_mrl_constraint_flag u(1) 
  gci_no_mip_constraint_flag u(1) 
  gci_no_cclm_constraint_flag u(1) 
 /* inter */  
  gci_no_ref_pic_resampling_constraint_flag u(1) 
  gci_no_res_change_in_clvs_constraint_flag u(1)
    gci_no_weighted_prediction_constraint_flag u(1) 
  gci_no_ref_wraparound_constraint_flag u(1) 
  gci_no_temporal_mvp_constraint_flag u(1) 
  gci_no_sbtmvp_constraint_flag  u(1) 
  gci_no_amvr_constraint_flag u(1) 
  gci_no_bdof_constraint_flag u(1) 
  gci_no_smvd_constraint_flag u(1) 
  gci_no_dmvr_constraint_flag u(1) 
  gci_no_mmvd_constraint_flag u(1) 
  gci_no_affine_motion_constraint_flag u(1) 
  gci_no_prof_constraint_flag u(1) 
  gci_no_bcw_constraint_flag u(1) 
  gci_no_ciip_constraint_flag u(1) 
  gci_no_gpm_constraint_flag u(1) 
 /* transform, quantization, residual */  
  gci_no_luma_transform_size_64_constraint_flag u(1) 
  gci_no_transform_skip_constraint_flag u(1) 
  gci_no_bdpcm_constraint_flag u(1) 
  gci_no_mts_constraint_flag u(1) 
  gci_no_lfnst_constraint_flag u(1) 
  gci_no_joint_cbcr_constraint_flag u(1) 
  gci_no_sbt_constraint_flag u(1) 
  gci_no_act_constraint_flag u(1) 
  gci_no_explicit_scaling_list_constraint_flag u(1) 
  gci_no_dep_quant_constraint_flag u(1) 
  gci_no_sign_data_hiding_constraint_flag u(1) 
  gci_no_cu_qp_delta_constraint_flag u(1) 
  gci_no_chroma_qp_offset_constraint_flag u(1) 
 /* loop filter */  
  gci_no_sao_constraint_flag u(1) 
  gci_no_alf_constraint_flag u(1) 
  gci_no_ccalf_constraint_flag u(1) 
  gci_no_lmcs_constraint_flag u(1) 
  gci_no_ladf_constraint_flag u(1) 
  gci_no_virtual_boundaries_constraint_flag u(1) 
  gci_num_reserved_bits u(8) 
  for( i = 0; i < gci_num_reserved_bits; i++ )  
   gci_reserved_zero_bit[ i ] u(1) 
 }  
 while( !byte_aligned() )  
  gci_alignment_zero_bit f(1) 
} 

dpb_parameters( MaxSubLayersMinus1, subLayerInfoFlag ) {  
 for( i = ( subLayerInfoFlag ? 0 : MaxSubLayersMinus1 ); 
   i  <=  MaxSubLayersMinus1; i++ ) { 
   dpb_max_dec_pic_buffering_minus1[ i ] ue(v) 
  dpb_max_num_reorder_pics[ i ] ue(v) 
  dpb_max_latency_increase_plus1[ i ] ue(v) 
 }  
}  

general_timing_hrd_parameters() {  
 num_units_in_tick u(32) 
 time_scale u(32) 
 general_nal_hrd_params_present_flag u(1) 
 general_vcl_hrd_params_present_flag u(1) 
 if( general_nal_hrd_params_present_flag  ||  general_vcl_hrd_params_present_flag ) {  
  general_same_pic_timing_in_all_ols_flag u(1) 
  general_du_hrd_params_present_flag u(1) 
  if( general_du_hrd_params_present_flag )  
   tick_divisor_minus2 u(8) 
  bit_rate_scale u(4) 
  cpb_size_scale u(4) 
  if( general_du_hrd_params_present_flag )  
   cpb_size_du_scale u(4) 
  hrd_cpb_cnt_minus1 ue(v) 
 }  
}  

ols_timing_hrd_parameters( firstSubLayer, MaxSubLayersVal ) {  
 for( i = firstSubLayer; i  <=  MaxSubLayersVal; i++ ) {  
  fixed_pic_rate_general_flag[ i ] u(1) 
  if( !fixed_pic_rate_general_flag[ i ] )  
   fixed_pic_rate_within_cvs_flag[ i ] u(1) 
  if( fixed_pic_rate_within_cvs_flag[ i ] )  
   elemental_duration_in_tc_minus1[ i ] ue(v) 
  else if( ( general_nal_hrd_params_present_flag  || 
    general_vcl_hrd_params_present_flag )  &&  hrd_cpb_cnt_minus1  ==  0 ) 
 
   low_delay_hrd_flag[ i ] u(1) 
  if( general_nal_hrd_params_present_flag )  
   sublayer_hrd_parameters( i )  
  if( general_vcl_hrd_params_present_flag )  
   sublayer_hrd_parameters( i )  
 }  
}  

sublayer_hrd_parameters( subLayerId ) {  
 for( j = 0; j  <=  hrd_cpb_cnt_minus1; j++ ) {  
  bit_rate_value_minus1[ subLayerId ][ j ] ue(v) 
  cpb_size_value_minus1[ subLayerId ][ j ] ue(v) 
  if( general_du_hrd_params_present_flag ) {  
   cpb_size_du_value_minus1[ subLayerId ][ j ] ue(v) 
   bit_rate_du_value_minus1[ subLayerId ][ j ] ue(v) 
  }  
  cbr_flag[ subLayerId ][ j ] u(1) 
 }  
}  

sei_message() {  
 payloadType = 0  
 do {  
  payload_type_byte u(8) 
  payloadType  +=  payload_type_byte  
 } while( payload_type_byte  ==  0xFF )  
 payloadSize = 0  
 do {  
  payload_size_byte u(8) 
  payloadSize  +=  payload_size_byte  
 } while( payload_size_byte  ==  0xFF )  
 sei_payload( payloadType, payloadSize )  
}  

ref_pic_list_struct( listIdx, rplsIdx ) {  
 num_ref_entries[ listIdx ][ rplsIdx ] ue(v) 
 if( sps_long_term_ref_pics_flag  &&  rplsIdx < sps_num_ref_pic_lists[ listIdx ]  && 
   num_ref_entries[ listIdx ][ rplsIdx ] > 0 ) 
 
  ltrp_in_header_flag[ listIdx ][ rplsIdx ] u(1) 
 for( i = 0, j = 0; i < num_ref_entries[ listIdx ][ rplsIdx ]; i++) {  
  if( sps_inter_layer_prediction_enabled_flag )  
   inter_layer_ref_pic_flag[ listIdx ][ rplsIdx ][ i ] u(1) 
  if( !inter_layer_ref_pic_flag[ listIdx ][ rplsIdx ][ i ] ) {  
   if( sps_long_term_ref_pics_flag )  
          st_ref_pic_flag[ listIdx ][ rplsIdx ][ i ] u(1) 
   else
      st_ref_pic_flag[ listIdx ][ rplsIdx ][ i ] = 1 /* LukasV added default */
   if( st_ref_pic_flag[ listIdx ][ rplsIdx ][ i ] ) {  
    abs_delta_poc_st[ listIdx ][ rplsIdx ][ i ] ue(v) 
    if( AbsDeltaPocSt[ listIdx ][ rplsIdx ][ i ] > 0 )  
     strp_entry_sign_flag[ listIdx ][ rplsIdx ][ i ] u(1) 
   } else if( !ltrp_in_header_flag[ listIdx ][ rplsIdx ] )  
    rpls_poc_lsb_lt[ listIdx ][ rplsIdx ][ j++ ] u(v) 
      } else  
   ilrp_idx[ listIdx ][ rplsIdx ][ i ] ue(v) 
 }  
}  

sei_payload( payloadType, payloadSize ) {  
 if( nal_unit_type  ==  PREFIX_SEI_NUT )  
  if( payloadType  ==  0 )  
   buffering_period( payloadSize )  
  else if( payloadType  ==  1 )  
   pic_timing( payloadSize )  
  else if( payloadType  ==  3 )  
   filler_payload( payloadSize ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 */  
  else if( payloadType  ==  4 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 */  
   user_data_registered_itu_t_t35( payloadSize )  
  else if( payloadType  ==  5 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 */  
   user_data_unregistered( payloadSize )  
  else if( payloadType  ==  19 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 */  
   film_grain_characteristics( payloadSize )  
  else if( payloadType  ==  45 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 */  
   frame_packing_arrangement( payloadSize )  
  else if( payloadType  ==  129 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 */  
   parameter_sets_inclusion_indication( payloadSize )  
  else if( payloadType  ==  130 )  
   decoding_unit_info( payloadSize )  
  else if( payloadType  ==  133 )  
   scalable_nesting( payloadSize )  
  else if( payloadType  ==  137 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 */  
   mastering_display_colour_volume( payloadSize )  
  else if( payloadType  ==  144 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 */  
   content_light_level_info( payloadSize )  
  else if( payloadType  ==  145 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 */  
   dependent_rap_indication( payloadSize )  
  else if( payloadType  ==  147 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 */  
   alternative_transfer_characteristics( payloadSize )  
  else if( payloadType  ==  148 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 */  
   ambient_viewing_environment( payloadSize )  
  else if( payloadType  ==  149 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 */  
   content_colour_volume( payloadSize )  
  else if( payloadType  ==  150 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 */  
   equirectangular_projection( payloadSize )  
  else if( payloadType  ==  153 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 */  
   generalized_cubemap_projection( payloadSize )  
  else if( payloadType  ==  154 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 */  
   sphere_rotation( payloadSize )  
  else if( payloadType  ==  155 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 */  
   regionwise_packing( payloadSize )  
  else if( payloadType  ==  156 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 */  
   omni_viewport( payloadSize )  
  else if( payloadType  ==  168 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 */  
   frame_field_info( payloadSize )  
  else if( payloadType  ==  203 )  
   subpic_level_info( payloadSize )  
  else if( payloadType  ==  204 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 */  
   sample_aspect_ratio_info( payloadSize )  
  else                                             /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 */  
   reserved_message( payloadSize )  
 else /* nal_unit_type  ==  SUFFIX_SEI_NUT */  
  if( payloadType  ==  3 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 */  
   filler_payload( payloadSize )  
  else if( payloadType  ==  132 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 */  
   decoded_picture_hash( payloadSize )  
  else if( payloadType  ==  133 )  
   scalable_nesting( payloadSize )  
  else                                     /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 */  
   reserved_message( payloadSize )  
 if( more_data_in_payload() ) {  
  if( payload_extension_present() )  
   sei_reserved_payload_extension_data u(v) 
  sei_payload_bit_equal_to_one /* equal to 1 */ f(1) 
  while( !byte_aligned() )  
   sei_payload_bit_equal_to_zero /* equal to 0 */ f(1) 
 }  
} 

filler_payload(payloadSize) {
    for (k = 0; k < payloadSize; k++)
        ff_byte  /* equal to 0xFF */ f(8)
}

user_data_registered_itu_t_t35(payloadSize) {
    itu_t_t35_country_code b(8)
    if (itu_t_t35_country_code != 0xFF)
        i = 1
    else {        
        itu_t_t35_country_code_extension_byte b(8)
        i = 2
    }
    do {
        itu_t_t35_payload_byte b(8)
        i++
    } while (i < payloadSize)
}

user_data_unregistered(payloadSize) {
    uuid_iso_iec_11578 u(128)
    for (i = 16; i < payloadSize; i++)  
        user_data_payload_byte b(8)
}

film_grain_characteristics(payloadSize) {
 fg_characteristics_cancel_flag u(1)
    if (!fg_characteristics_cancel_flag) {  
  fg_model_id u(2) 
  fg_separate_colour_description_present_flag u(1)
        if (fg_separate_colour_description_present_flag) {  
   fg_bit_depth_luma_minus8 u(3) 
   fg_bit_depth_chroma_minus8 u(3) 
   fg_full_range_flag u(1) 
   fg_colour_primaries u(8) 
   fg_transfer_characteristics u(8) 
   fg_matrix_coeffs u(8)
        }  
  fg_blending_mode_id u(2) 
  fg_log2_scale_factor u(4)
        for (c = 0; c < 3; c++)
            fg_comp_model_present_flag[c] u(1)
        for (c = 0; c < 3; c++)
            if (fg_comp_model_present_flag[c]) {
                fg_num_intensity_intervals_minus1[c] u(8)
                fg_num_model_values_minus1[c] u(3)
                for (i = 0; i <= fg_num_intensity_intervals_minus1[c]; i++) {
                    fg_intensity_interval_lower_bound[c][i] u(8)
                    fg_intensity_interval_upper_bound[c][i] u(8)
                    for (j = 0; j <= fg_num_model_values_minus1[c]; j++)
                        fg_comp_model_value[c][i][j] se(v)
                }
            }  
  fg_characteristics_persistence_flag u(1)
    }
} 

frame_packing_arrangement(payloadSize) {
 fp_arrangement_id ue(v) 
 fp_arrangement_cancel_flag u(1)
    if (!fp_arrangement_cancel_flag) {  
  fp_arrangement_type u(7) 
  fp_quincunx_sampling_flag u(1) 
  fp_content_interpretation_type u(6) 
  fp_spatial_flipping_flag u(1) 
  fp_frame0_flipped_flag u(1) 
  fp_field_views_flag u(1) 
  fp_current_frame_is_frame0_flag u(1) 
  fp_frame0_self_contained_flag u(1) 
  fp_frame1_self_contained_flag u(1)
        if (!fp_quincunx_sampling_flag && fp_arrangement_type != 5) {  
   fp_frame0_grid_position_x u(4) 
   fp_frame0_grid_position_y u(4) 
   fp_frame1_grid_position_x u(4) 
   fp_frame1_grid_position_y u(4)
        }  
  fp_arrangement_reserved_byte u(8) 
  fp_arrangement_persistence_flag u(1)
    }  
 fp_upsampled_aspect_ratio_flag u(1)
}  

mastering_display_colour_volume(payloadSize) {
    for (c = 0; c < 3; c++) {
        mdcv_display_primaries_x[c] u(16)
        mdcv_display_primaries_y[c] u(16)
    }
    mdcv_white_point_x u(16)
    mdcv_white_point_y u(16)
    mdcv_max_display_mastering_luminance u(32)
    mdcv_min_display_mastering_luminance u(32)
}

content_light_level_info(payloadSize) {
    clli_max_content_light_level u(16)
    clli_max_pic_average_light_level u(16)
}

dependent_rap_indication(payloadSize) { 
} 

alternative_transfer_characteristics(payloadSize) {
    preferred_transfer_characteristics u(8)
}

ambient_viewing_environment(payloadSize) {
    ambient_illuminance u(32)  
    ambient_light_x u(16) 
    ambient_light_y u(16)
}

content_colour_volume(payloadSize) {
 ccv_cancel_flag u(1)
    if (!ccv_cancel_flag) {  
  ccv_persistence_flag u(1) 
  ccv_primaries_present_flag u(1) 
  ccv_min_luminance_value_present_flag u(1) 
  ccv_max_luminance_value_present_flag u(1) 
  ccv_avg_luminance_value_present_flag u(1) 
  ccv_reserved_zero_2bits u(2)
        if (ccv_primaries_present_flag)
            for (c = 0; c < 3; c++) {
                ccv_primaries_x[c] i(32)
                ccv_primaries_y[c] i(32)
            }
        if (ccv_min_luminance_value_present_flag)  
   ccv_min_luminance_value u(32)
        if (ccv_max_luminance_value_present_flag)  
   ccv_max_luminance_value u(32)
        if (ccv_avg_luminance_value_present_flag)  
   ccv_avg_luminance_value u(32)
    }
}  

equirectangular_projection(payloadSize) {
    erp_cancel_flag u(1)
    if (!erp_cancel_flag) {
        erp_persistence_flag u(1)
        erp_guard_band_flag u(1)
        erp_reserved_zero_2bits u(2)
        if (erp_guard_band_flag == 1) {
            erp_guard_band_type u(3)
            erp_left_guard_band_width u(8)
            erp_right_guard_band_width u(8)
        }
    }
}

generalized_cubemap_projection(payloadSize) {
    gcmp_cancel_flag u(1)
    if (!gcmp_cancel_flag) {       
        gcmp_persistence_flag u(1)
        gcmp_packing_type u(3) 
        gcmp_mapping_function_type u(2)
        for (i = 0; i < ((gcmp_packing_type == 4 || gcmp_packing_type  == 5) ? 5 : 6); i++ ) {

            gcmp_face_index[i] u(3)
            gcmp_face_rotation[i] u(2)
            if (gcmp_mapping_function_type == 2) {
                gcmp_function_coeff_u[i] u(7)
                gcmp_function_u_affected_by_v_flag[i] u(1)
                gcmp_function_coeff_v[i] u(7)
                gcmp_function_v_affected_by_u_flag[i] u(1)
            }
        }  
  gcmp_guard_band_flag u(1)
        if (gcmp_guard_band_flag) {  
   gcmp_guard_band_type u(3) 
   gcmp_guard_band_boundary_exterior_flag u(1) 
   gcmp_guard_band_samples_minus1 u(4)
        }
    }
}  

regionwise_packing(payloadSize) {
    rwp_cancel_flag  u(1)
    if (!rwp_cancel_flag) {
        rwp_persistence_flag  u(1)
        rwp_constituent_picture_matching_flag  u(1)
        rwp_reserved_zero_5bits  u(5)
        rwp_num_packed_regions  u(8)
        rwp_proj_picture_width u(32)
        rwp_proj_picture_height u(32)
         rwp_packed_picture_width u(16) 
  rwp_packed_picture_height u(16)
        for (i = 0; i < rwp_num_packed_regions; i++) {
            rwp_reserved_zero_4bits[i] u(4)
            rwp_transform_type[i] u(3)
            rwp_guard_band_flag[i] u(1)
            rwp_proj_region_width[i] u(32)
            rwp_proj_region_height[i] u(32)
            rwp_proj_region_top[i] u(32)
            rwp_proj_region_left[i] u(32)
            rwp_packed_region_width[i] u(16)
            rwp_packed_region_height[i] u(16)
            rwp_packed_region_top[i] u(16)
            rwp_packed_region_left[i] u(16)
            if (rwp_guard_band_flag[i]) {
                rwp_left_guard_band_width[i] u(8)
                rwp_right_guard_band_width[i] u(8)
                rwp_top_guard_band_height[i] u(8)
                rwp_bottom_guard_band_height[i] u(8)
                rwp_guard_band_not_used_for_pred_flag[i] u(1)
                for (j = 0; j < 4; j++)
                    rwp_guard_band_type[i][j] u(3)
                rwp_guard_band_reserved_zero_3bits[i] u(3)
            }
        }
    }
} 

omni_viewport(payloadSize) {
 omni_viewport_id u(10) 
 omni_viewport_cancel_flag u(1)
    if (!omni_viewport_cancel_flag) {  
  omni_viewport_persistence_flag u(1) 
  omni_viewport_cnt_minus1 u(4)
        for (i = 0; i <= omni_viewport_cnt_minus1; i++) {
            omni_viewport_azimuth_centre[i] i(32)
            omni_viewport_elevation_centre[i] i(32)
            omni_viewport_tilt_centre[i] i(32)
            omni_viewport_hor_range[i] u(32)
            omni_viewport_ver_range[i] u(32)
        }
    }
} 

frame_field_info(payloadSize) {
    ffi_field_pic_flag  u(1)
    if (ffi_field_pic_flag) {
        ffi_bottom_field_flag u(1)
        ffi_pairing_indicated_flag u(1)
        
        if (ffi_pairing_indicated_flag)
            ffi_paired_with_next_field_flag u(1)
     } else {
        
        ffi_display_fields_from_frame_flag u(1)
        if (ffi_display_fields_from_frame_flag)
            ffi_top_field_first_flag u(1)
        ffi_display_elemental_periods_minus1 u(8)
    }
    ffi_source_scan_type u(2)
    ffi_duplicate_flag u(1)
} 

sample_aspect_ratio_info(payloadSize) {
    sari_cancel_flag u(1)
    if (!sari_cancel_flag) {
        sari_persistence_flag u(1)
        sari_aspect_ratio_idc u(8)
        if (sari_aspect_ratio_idc == 255) {
            sari_sar_width u(16) 
            sari_sar_height u(16) 
        }
    }
}

annotated_regions(payloadSize) {
    ar_cancel_flag u(1)
    if (!ar_cancel_flag) {      
        ar_not_optimized_for_viewing_flag u(1)
        ar_true_motion_flag u(1)
        ar_occluded_object_flag u(1)
        ar_partial_object_flag_present_flag u(1)
        ar_object_label_present_flag u(1)
        ar_object_confidence_info_present_flag u(1)
        if (ar_object_confidence_info_present_flag)
            ar_object_confidence_length_minus1 u(4)
        if (ar_object_label_present_flag) {  
            ar_object_label_language_present_flag u(1)
            if (ar_object_label_language_present_flag) {
                while (!byte_aligned())  
                    ar_bit_equal_to_zero /* equal to 0 */ f(1) 
                ar_object_label_language st(v)
            }  
            ar_num_label_updates ue(v)
            for (i = 0; i < ar_num_label_updates; i++) {
                ar_label_idx[i] ue(v) 
                ar_label_cancel_flag u(1)
                LabelAssigned[ar_label_idx[i]] = !ar_label_cancel_flag
                if (!ar_label_cancel_flag) {
                    while (!byte_aligned())  
                        ar_bit_equal_to_zero /* equal to 0 */ f(1)
                    ar_label[ar_label_idx[i]] st(v)
                }
            }
        }  
        ar_num_object_updates ue(v)
        for (i = 0; i < ar_num_object_updates; i++) {
            ar_object_idx[i] ue(v) 
            ar_object_cancel_flag u(1)
            ObjectTracked[ar_object_idx[i]] = !ar_object_cancel_flag
            if (!ar_object_cancel_flag) {
                if (ar_object_label_present_flag) {  
                    ar_object_label_update_flag u(1)
                    if (ar_object_label_update_flag)
                        ar_object_label_idx[ar_object_idx[i]] ue(v)
                }  
                ar_bounding_box_update_flag u(1)
                if (ar_bounding_box_update_flag) {  
                    ar_bounding_box_cancel_flag u(1)
                    ObjectBoundingBoxAvail[ar_object_idx[i]] =
                        !ar_bounding_box_cancel_flag

                    if (!ar_bounding_box_cancel_flag) {
                        ar_bounding_box_top[ar_object_idx[i]] u(16)
                        ar_bounding_box_left[ar_object_idx[i]] u(16)
                        ar_bounding_box_width[ar_object_idx[i]] u(16)
                        ar_bounding_box_height[ar_object_idx[i]] u(16)
                        if (ar_partial_object_flag_present_flag)
                            ar_partial_object_flag[ar_object_idx[i]] u(1)
                        if (ar_object_confidence_info_present_flag)
                            ar_object_confidence[ar_object_idx[i]] u(v)
                    }
                }
            }
        }
    }
}

scalability_dimension_info(payloadSize) {
 sdi_max_layers_minus1 u(6) 
 sdi_multiview_info_flag u(1) 
 sdi_auxiliary_info_flag u(1)
    if (sdi_multiview_info_flag || sdi_auxiliary_info_flag) {
        if (sdi_multiview_info_flag)  
            sdi_view_id_len_minus1 u(4)
        for (i = 0; i <= sdi_max_layers_minus1; i++) {
            sdi_layer_id[i] u(6)
            if (sdi_multiview_info_flag)
                sdi_view_id_val[i] u(v)
            if (sdi_auxiliary_info_flag)
                sdi_aux_id[i] u(8)
            if (sdi_aux_id[i] > 0) {
                sdi_num_associated_primary_layers_minus1[i] u(6)
                for (j = 0; j <= sdi_num_associated_primary_layers_minus1[i]; j++)
                    sdi_associated_primary_layer_idx[i][j] u(6)
            }
        }
    }
}  

multiview_acquisition_info(payloadSize) {
    intrinsic_param_flag u(1)
    extrinsic_param_flag u(1)
    num_views_minus1 ue(v)
    if (intrinsic_param_flag) {
        intrinsic_params_equal_flag u(1)
        prec_focal_length ue(v) 
  prec_principal_point ue(v) 
  prec_skew_factor ue(v)
        for (i = 0; i <= intrinsic_params_equal_flag ? 0 : num_views_minus1; i++) {
            sign_focal_length_x[i] u(1)
            exponent_focal_length_x[i] u(6)
            mantissa_focal_length_x[i] u(v)
            sign_focal_length_y[i] u(1)
            exponent_focal_length_y[i] u(6)
            mantissa_focal_length_y[i] u(v)
            sign_principal_point_x[i] u(1)
            exponent_principal_point_x[i] u(6)
            mantissa_principal_point_x[i] u(v)
            sign_principal_point_y[i] u(1)
            exponent_principal_point_y[i] u(6)
            mantissa_principal_point_y[i] u(v)
            sign_skew_factor[i] u(1)
            exponent_skew_factor[i] u(6)
            mantissa_skew_factor[i] u(v)
        }
    }
    if (extrinsic_param_flag) {  
  prec_rotation_param ue(v) 
  prec_translation_param ue(v)
        for (i = 0; i <= num_views_minus1; i++)
            for (j = 0; j < 3; j++) { /* row */
                for (k = 0; k < 3; k++) { /* column */
                    sign_r[i][j][k] u(1)
                    exponent_r[i][j][k] u(6)
                    mantissa_r[i][j][k] u(v)
                }
                sign_t[i][j] u(1)
                exponent_t[i][j] u(6)
                mantissa_t[i][j] u(v)
            }
    }
}

multiview_view_position(payloadSize) {
    num_views_minus1 ue(v)
    for (i = 0; i <= num_views_minus1; i++ )
        view_position[i] ue(v)
}

depth_representation_info(payloadSize) {
    z_near_flag u(1)
    z_far_flag u(1)
    d_min_flag u(1)
    d_max_flag u(1)
    depth_representation_type ue(v) 
    
    if (d_min_flag || d_max_flag)
        disparity_ref_view_id ue(v) 
    if (z_near_flag)
        depth_rep_info_element(ZNearSign, ZNearExp, ZNearMantissa, ZNearManLen)
    if (z_far_flag)
        depth_rep_info_element(ZFarSign, ZFarExp, ZFarMantissa, ZFarManLen)
    if (d_min_flag)
        depth_rep_info_element(DMinSign, DMinExp, DMinMantissa, DMinManLen)
    if (d_max_flag)
        depth_rep_info_element(DMaxSign, DMaxExp, DMaxMantissa, DMaxManLen)
    if (depth_representation_type == 3) {
        depth_nonlinear_representation_num_minus1 ue(v)
        for (i = 1; i <= depth_nonlinear_representation_num_minus1 + 1; i++)
            depth_nonlinear_representation_model[i]  ue(v)
    }
}  

depth_rep_info_element(OutSign, OutExp, OutMantissa, OutManLen) {
    da_sign_flag u(1)
    da_exponent u(7)
    da_mantissa_len_minus1 u(5)
    da_mantissa u(v)  
}

alpha_channel_info(payloadSize) {
    alpha_channel_cancel_flag u(1)
    if (!alpha_channel_cancel_flag) {
        alpha_channel_use_idc u(3)
        alpha_channel_bit_depth_minus8 u(3)
        alpha_transparent_value u(v)
        alpha_opaque_value u(v)
        alpha_channel_incr_flag u(1)
        alpha_channel_clip_flag u(1)
        if (alpha_channel_clip_flag)
            alpha_channel_clip_type_flag u(1)
    }
}  

display_orientation(payloadSize) {
    display_orientation_cancel_flag u(1)
    if (!display_orientation_cancel_flag) {
        display_orientation_persistence_flag u(1)
        display_orientation_transform_type u(3)
        display_orientation_reserved_zero_3bits u(3)
    }
}

colour_transform_info(payloadSize) {
    colour_transform_id ue(v)
    colour_transform_cancel_flag u(1)      
    if (!colour_transform_cancel_flag) {
        colour_transform_persistence_flag u(1)
        colour_transform_video_signal_info_present_flag u(1)
        if (colour_transform_video_signal_info_present_flag) {
            colour_transform_full_range_flag u(1)
            colour_tranform_primaries u(8)
            colour_transform_transfer_function u(8)
            colour_transform_matrix_coefficients u(8)
        }
        colour_transform_bit_depth_minus8 u(4)
        colour_transform_log2_number_of_points_per_lut_minus1 u(3)
        colour_transform_cross_component_flag u(1)
        if (colour_transform_cross_component_flag )
            colour_transform_cross_comp_inferred_flag u(1)
        for (i = 0; i < colourTransformSize; i++)
            colour_transf_lut[0][i] u(v)
        if (colour_transform_cross_component_flag == 0 ||
            colour_transform_cross_comp_inferred_flag == 0) {
            colour_transform_lut2_present_flag u(1)
            for (i = 0; i < colourTransformSize; i++)
                colour_transf_lut[1][i] u(1)
            if (colour_transform_lut2_present_flag)
                for (i = 0; i < colourTransformSize; i++)
                    colour_transf_lut[2][i] u(v)
        } else { 
            colour_transform_chroma_offset u(v)
        }
    }
}  

shutter_interval_info(payloadSize) {
    sii_time_scale  u(32)
    sii_fixed_shutter_interval_within_clvs_flag u(1)
    if (sii_fixed_shutter_interval_within_clvs_flag)  
        sii_num_units_in_shutter_interval u(32)
    else {
        sii_max_sub_layers_minus1 u(3)
        for (i = 0; i <= sii_max_sub_layers_minus1; i++)
            sii_sub_layer_num_units_in_shutter_interval[i] u(32)
    }
}

phase_indication(payloadSize) {
    pi_hor_phase_num u(8)
    pi_hor_phase_den_minus1 u(8)
    pi_ver_phase_num u(8)
    pi_ver_phase_den_minus1 u(8)
}

reserved_message(payloadSize) {
    for (i = 0; i < payloadSize; i++)
        reserved_message_payload_byte u(8)    
}

buffering_period( payloadSize ) {  
 bp_nal_hrd_params_present_flag u(1) 
 bp_vcl_hrd_params_present_flag u(1) 
 bp_cpb_initial_removal_delay_length_minus1 u(5) 
 bp_cpb_removal_delay_length_minus1 u(5) 
 bp_dpb_output_delay_length_minus1 u(5) 
 bp_du_hrd_params_present_flag u(1) 
 if( bp_du_hrd_params_present_flag ) {  
  bp_du_cpb_removal_delay_increment_length_minus1 u(5) 
  bp_dpb_output_delay_du_length_minus1 u(5) 
  bp_du_cpb_params_in_pic_timing_sei_flag u(1) 
  bp_du_dpb_params_in_pic_timing_sei_flag u(1) 
 }  
 bp_concatenation_flag u(1) 
 bp_additional_concatenation_info_present_flag u(1) 
 if( bp_additional_concatenation_info_present_flag )  
  bp_max_initial_removal_delay_for_concatenation u(v) 
 bp_cpb_removal_delay_delta_minus1 u(v) 
 bp_max_sublayers_minus1 u(3) 
 if( bp_max_sublayers_minus1 > 0 )  
  bp_cpb_removal_delay_deltas_present_flag u(1) 
 if( bp_cpb_removal_delay_deltas_present_flag ) {  
  bp_num_cpb_removal_delay_deltas_minus1 ue(v) 
  for( i = 0; i  <=  bp_num_cpb_removal_delay_deltas_minus1; i++ )  
   bp_cpb_removal_delay_delta_val[ i ] u(v) 
 }  
 bp_cpb_cnt_minus1 ue(v) 
 if( bp_max_sublayers_minus1 > 0 )  
  bp_sublayer_initial_cpb_removal_delay_present_flag u(1) 
 for( i = ( bp_sublayer_initial_cpb_removal_delay_present_flag ? 
   0 : bp_max_sublayers_minus1 ); i  <=  bp_max_sublayers_minus1; i++ ) { 
 
  if( bp_nal_hrd_params_present_flag )  
   for( j = 0; j < bp_cpb_cnt_minus1 + 1; j++ ) {  
    bp_nal_initial_cpb_removal_delay[ i ][ j ] u(v) 
    bp_nal_initial_cpb_removal_offset[ i ][ j ] u(v) 
    if( bp_du_hrd_params_present_flag) {  
     bp_nal_initial_alt_cpb_removal_delay[ i ][ j ] u(v) 
     bp_nal_initial_alt_cpb_removal_offset[ i ][ j ] u(v) 
    }  
   }  
  if( bp_vcl_hrd_params_present_flag )  
     for( j = 0; j < bp_cpb_cnt_minus1 + 1; j++ ) {  
    bp_vcl_initial_cpb_removal_delay[ i ][ j ] u(v) 
    bp_vcl_initial_cpb_removal_offset[ i ][ j ] u(v) 
    if( bp_du_hrd_params_present_flag ) {  
     bp_vcl_initial_alt_cpb_removal_delay[ i ][ j ] u(v) 
     bp_vcl_initial_alt_cpb_removal_offset[ i ][ j ] u(v) 
    }  
  }  
 }  
 if( bp_max_sublayers_minus1 > 0 )  
  bp_sublayer_dpb_output_offsets_present_flag u(1) 
 if( bp_sublayer_dpb_output_offsets_present_flag )  
  for( i = 0; i < bp_max_sublayers_minus1; i++ )  
   bp_dpb_output_tid_offset[ i ] ue(v) 
 bp_alt_cpb_params_present_flag u(1) 
 if( bp_alt_cpb_params_present_flag )  
  bp_use_alt_cpb_params_flag u(1) 
} 

pic_timing( payloadSize ) {  
 pt_cpb_removal_delay_minus1[ bp_max_sublayers_minus1 ] u(v) 
 for( i = TemporalId; i < bp_max_sublayers_minus1; i++ ) {  
  pt_sublayer_delays_present_flag[ i ] u(1) 
  if( pt_sublayer_delays_present_flag[ i ] ) {  
   if( bp_cpb_removal_delay_deltas_present_flag )  
    pt_cpb_removal_delay_delta_enabled_flag[ i ] u(1) 
   if( pt_cpb_removal_delay_delta_enabled_flag[ i ] ) {  
    if( bp_num_cpb_removal_delay_deltas_minus1 > 0 )  
         pt_cpb_removal_delay_delta_idx[ i ] u(v) 
   } else  
    pt_cpb_removal_delay_minus1[ i ] u(v) 
  }  
 }  
 pt_dpb_output_delay u(v) 
 if( bp_alt_cpb_params_present_flag ) {  
  pt_cpb_alt_timing_info_present_flag u(1) 
  if( pt_cpb_alt_timing_info_present_flag ) {  
   if( bp_nal_hrd_params_present_flag ) {  
    for( i = ( bp_sublayer_initial_cpb_removal_delay_present_flag ? 0 : 
      bp_max_sublayers_minus1 ); i  <=  bp_max_sublayers_minus1; i++ ) { 
 
     for( j = 0; j < bp_cpb_cnt_minus1 + 1; j++ ) {  
      pt_nal_cpb_alt_initial_removal_delay_delta[ i ][ j ] u(v) 
      pt_nal_cpb_alt_initial_removal_offset_delta[ i ][ j ] u(v) 
     }  
     pt_nal_cpb_delay_offset[ i ] u(v) 
     pt_nal_dpb_delay_offset[ i ] u(v) 
    }  
   }  
   if( bp_vcl_hrd_params_present_flag ) {  
    for( i = ( bp_sublayer_initial_cpb_removal_delay_present_flag ? 0 : 
      bp_max_sublayers_minus1 ); i  <=  bp_max_sublayers_minus1; i++ ) { 
 
     for( j = 0; j < bp_cpb_cnt_minus1 + 1; j++ ) {  
      pt_vcl_cpb_alt_initial_removal_delay_delta[ i ][ j ] u(v) 
      pt_vcl_cpb_alt_initial_removal_offset_delta[ i ][ j ] u(v) 
     }  
     pt_vcl_cpb_delay_offset[ i ] u(v) 
     pt_vcl_dpb_delay_offset[ i ] u(v) 
    }  
   }  
  }  
 }  
 if( bp_du_hrd_params_present_flag  && 
   bp_du_dpb_params_in_pic_timing_sei_flag ) 
 
  pt_dpb_output_du_delay u(v) 
 if( bp_du_hrd_params_present_flag  && 
   bp_du_cpb_params_in_pic_timing_sei_flag ) { 
 
  pt_num_decoding_units_minus1 ue(v) 
  if( pt_num_decoding_units_minus1 > 0 ) {  
   pt_du_common_cpb_removal_delay_flag u(1) 
   if( pt_du_common_cpb_removal_delay_flag )  
    for( i = TemporalId; i  <=  bp_max_sublayers_minus1; i++ )  
     if( pt_sublayer_delays_present_flag[ i ] )  
      pt_du_common_cpb_removal_delay_increment_minus1[ i ] u(v) 
   for( i = 0; i  <=  pt_num_decoding_units_minus1; i++ ) {  
    pt_num_nalus_in_du_minus1[ i ] ue(v) 
    if( !pt_du_common_cpb_removal_delay_flag  && 
      i < pt_num_decoding_units_minus1 ) 
      for( j = TemporalId; j  <=  bp_max_sublayers_minus1; j++ )  
      if( pt_sublayer_delays_present_flag[ j ] )  
       pt_du_cpb_removal_delay_increment_minus1[ i ][ j ] u(v) 
   }  
  }  
 }  
 if( bp_additional_concatenation_info_present_flag )  
  pt_delay_for_concatenation_ensured_flag u(1) 
 pt_display_elemental_periods_minus1 u(8) 
}  


decoding_unit_info( payloadSize ) { 
dui_decoding_unit_idx  ue(v)
if( !bp_du_cpb_params_in_pic_timing_sei_flag )  
for( i = TemporalId; i  <=  bp_max_sublayers_minus1; i++ ) {  
if( i < bp_max_sublayers_minus1 )  
dui_sublayer_delays_present_flag[ i ]  u(1) 
if( dui_sublayer_delays_present_flag[ i ] )  
dui_du_cpb_removal_delay_increment[ i ] u(v) 
}  
if( !bp_du_dpb_params_in_pic_timing_sei_flag )  
dui_dpb_output_du_delay_present_flag u(1) 
if( dui_dpb_output_du_delay_present_flag )  
dui_dpb_output_du_delay u(v) 
}  
scalable_nesting( payloadSize ) { 
sn_ols_flag  u(1) 
sn_subpic_flag u(1) 
sn_num_olss_minus1 ue(v) 
if( sn_ols_flag ) {  
for( i = 0; i  <=  sn_num_olss_minus1; i++ )  
sn_ols_idx_delta_minus1[ i ] ue(v) 
} else { 
sn_all_layers_flag u(1) 
if( !sn_all_layers_flag ) {  
sn_num_layers_minus1 ue(v) 
for( i = 1; i  <=  sn_num_layers_minus1; i++ )  
sn_layer_id[ i ] u(6) 
}  
}  
if( sn_subpic_flag ) {  
sn_num_subpics_minus1 ue(v) 
sn_subpic_id_len_minus1 ue(v) 
for( i = 0; i  <=  sn_num_subpics_minus1; i++ )  
sn_subpic_id[ i ] u(v) 
}  
sn_num_seis_minus1 ue(v) 
while( !byte_aligned() )  
sn_zero_bit /* equal to 0 */ u(1) 
for( i = 0; i  <=  sn_num_seis_minus1; i++ )  
sei_message()  
}  

subpic_level_info( payloadSize ) { 
sli_num_ref_levels_minus1  u(3) 
sli_cbr_constraint_flag u(1) 
sli_explicit_fraction_present_flag u(1) 

if( sli_explicit_fraction_present_flag )  

sli_num_subpics_minus1 ue(v) 
sli_max_sublayers_minus1 u(3) 
sli_sublayer_info_present_flag u(1) 

while( !byte_aligned() )  

sli_alignment_zero_bit f(1) 
for( k = sli_sublayer_info_present_flag ? 0 : sli_max_sublayers_minus1; k  <=  sli_max_sublayers_minus1; k++ ) 
for( i = 0; i  <=  sli_num_ref_levels_minus1; i++ ) {  
sli_non_subpic_layers_fraction[ i ][ k ] u(8) 
sli_ref_level_idc[ i ][ k ] u(8) 
if( sli_explicit_fraction_present_flag )  
for( j = 0; j  <=  sli_num_subpics_minus1; j++ )  
sli_ref_level_fraction_minus1[ i ][ j ][ k ] u(8) 
}  
}  

parameter_sets_inclusion_indication(payloadSize) {
    psii_self_contained_clvs_flag  u(1)
}

sphere_rotation(payloadSize) { 
    sphere_rotation_cancel_flag u(1)
    if (!sphere_rotation_cancel_flag) {  
      sphere_rotation_persistence_flag u(1) 
      sphere_rotation_reserved_zero_6bits u(6) 
      yaw_rotation i(32) 
      pitch_rotation i(32) 
      roll_rotation i(32)
    }
}

decoded_picture_hash(payloadSize) {
 dph_sei_hash_type u(8) 
 dph_sei_single_component_flag u(1) 
 dph_sei_reserved_zero_7bits u(7)
    for (cIdx = 0; cIdx < (dph_sei_single_component_flag ? 1 : 3); cIdx++)
        if (dph_sei_hash_type == 0)
            for (i = 0; i < 16; i++)
                dph_sei_picture_md5[cIdx][i] b(8) 
  else if (dph_sei_hash_type == 1)
        dph_sei_picture_crc[cIdx] u(16) 
  else if (dph_sei_hash_type == 2)
        dph_sei_picture_checksum[cIdx] u(32)
}


