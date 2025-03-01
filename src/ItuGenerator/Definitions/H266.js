nal_unit( NumBytesInNalUnit ) { 
nal_unit_header()  
NumBytesInRbsp = 0  
for( i = 2; i < NumBytesInNalUnit; i++ )  
if( i + 2 < NumBytesInNalUnit  &&  next_bits( 24 )  ==  0x000003 ) {  
rbsp_byte[ NumBytesInRbsp++ ] b(8) 
rbsp_byte[ NumBytesInRbsp++ ] b(8) 
i  +=  2  
emulation_prevention_three_byte  /* equal to 0x03 */ f(8) 
} else  
rbsp_byte[ NumBytesInRbsp++ ] b(8) 
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
   firstSubLayer = sps_sublayer_cpb_params_present_flag ? 0 : 
     sps_max_sublayers_minus1 
 
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
}  

adaptation_parameter_set_rbsp() {  
 aps_params_type u(3) 
 aps_adaptation_parameter_set_id u(5) 
 aps_chroma_present_flag u(1) 
 if( aps_params_type  ==  ALF_APS )  
  alf_data()  
 else if( aps_params_type  ==  LMCS_APS )  
  lmcs_data()  
 else if( aps_params_type  ==  SCALING_APS )  
  scaling_list_data()  
 aps_extension_flag u(1) 
 if( aps_extension_flag )  
  while( more_rbsp_data() )  
   aps_extension_data_flag u(1) 
 rbsp_trailing_bits()  
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
  if( !pps_rpl_info_in_ph_flag ) /* This condition is intentionally not merged into the next, 
    to avoid possible interpretation of RplsIdx[ i ] not having a specified value. */ 
 
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

slice_layer_rbsp() { 
slice_header()  
slice_data()  
rbsp_slice_trailing_bits()  
}  

rbsp_slice_trailing_bits() {  
rbsp_trailing_bits()  
while( more_rbsp_trailing_data() )  
rbsp_cabac_zero_word  /* equal to 0x0000 */ f(16) 
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

scaling_list_data() {  
 for( id = 0; id < 28; id ++ ) {  
  matrixSize = id < 2 ? 2 : ( id < 8 ? 4 : 8 )  
  if( aps_chroma_present_flag  ||  id % 3  ==  2  ||  id  ==  27 ) {  
   scaling_list_copy_mode_flag[ id ] u(1) 
   if( !scaling_list_copy_mode_flag[ id ] )  
    scaling_list_pred_mode_flag[ id ] u(1) 
   if( ( scaling_list_copy_mode_flag[ id ]  ||  scaling_list_pred_mode_flag[ id ] )  && 
     id  !=  0  &&  id  !=  2  &&  id  !=  8 ) 
 
    scaling_list_pred_id_delta[ id ] ue(v) 
   if( !scaling_list_copy_mode_flag[ id ] ) {  
    nextCoef = 0  
    if( id > 13 ) {  
     scaling_list_dc_coef[ id - 14 ] se(v) 
     nextCoef  +=  scaling_list_dc_coef[ id - 14 ]  
    }  
    for( i = 0; i < matrixSize * matrixSize; i++ ) {  
     x = DiagScanOrder[ 3 ][ 3 ][ i ][ 0 ]  
     y = DiagScanOrder[ 3 ][ 3 ][ i ][ 1 ]  
     if( !( id > 25  &&  x  >=  4  &&  y  >=  4 ) ) {  
      scaling_list_delta_coef[ id ][ i ] se(v) 
      nextCoef  +=  scaling_list_delta_coef[ id ][ i ]  
           }  
     ScalingList[ id ][ i ] = nextCoef  
    }  
   }  
  }  
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

slice_header() {  
 sh_picture_header_in_slice_header_flag u(1) 
 if( sh_picture_header_in_slice_header_flag )  
  picture_header_structure()  
 if( sps_subpic_info_present_flag )  
  sh_subpic_id u(v) 
 if( ( pps_rect_slice_flag  &&  NumSlicesInSubpic[ CurrSubpicIdx ] > 1 )  || 
   ( !pps_rect_slice_flag  &&  NumTilesInPic > 1 ) ) 
 
  sh_slice_address u(v) 
 for( i = 0; i < NumExtraShBits; i++ )  
  sh_extra_bit[ i ] u(1) 
 if( !pps_rect_slice_flag  &&  NumTilesInPic - sh_slice_address > 1 )  
  sh_num_tiles_in_slice_minus1 ue(v) 
 if( ph_inter_slice_allowed_flag )  
  sh_slice_type ue(v) 
   if( nal_unit_type  ==  IDR_W_RADL  ||  nal_unit_type  ==  IDR_N_LP  || 
   nal_unit_type  ==    CRA_NUT  ||  nal_unit_type  ==  GDR_NUT ) 
 
  sh_no_output_of_prior_pics_flag u(1) 
 if( sps_alf_enabled_flag  &&  !pps_alf_info_in_ph_flag ) {  
  sh_alf_enabled_flag u(1) 
  if( sh_alf_enabled_flag ) {  
   sh_num_alf_aps_ids_luma u(3) 
   for( i = 0; i < sh_num_alf_aps_ids_luma; i++ )  
    sh_alf_aps_id_luma[ i ] u(3) 
   if( sps_chroma_format_idc  !=  0 ) {  
    sh_alf_cb_enabled_flag u(1) 
    sh_alf_cr_enabled_flag u(1) 
   }  
   if( sh_alf_cb_enabled_flag  ||  sh_alf_cr_enabled_flag )  
    sh_alf_aps_id_chroma u(3) 
   if( sps_ccalf_enabled_flag ) {  
    sh_alf_cc_cb_enabled_flag u(1) 
    if( sh_alf_cc_cb_enabled_flag )  
     sh_alf_cc_cb_aps_id u(3) 
    sh_alf_cc_cr_enabled_flag u(1) 
    if( sh_alf_cc_cr_enabled_flag )  
     sh_alf_cc_cr_aps_id u(3) 
   }  
  }  
 }  
 if( ph_lmcs_enabled_flag  &&  !sh_picture_header_in_slice_header_flag )  
  sh_lmcs_used_flag u(1) 
 if( ph_explicit_scaling_list_enabled_flag  &&  !sh_picture_header_in_slice_header_flag )  
  sh_explicit_scaling_list_used_flag u(1) 
 if( !pps_rpl_info_in_ph_flag  &&  ( ( nal_unit_type  !=  IDR_W_RADL  && 
   nal_unit_type  !=  IDR_N_LP )  ||  sps_idr_rpl_present_flag ) ) 
 
  ref_pic_lists()  
 if( ( sh_slice_type  !=  I  &&  num_ref_entries[ 0 ][ RplsIdx[ 0 ] ] > 1 )  || 
   ( sh_slice_type  ==  B  &&  num_ref_entries[ 1 ][ RplsIdx[ 1 ] ] > 1 ) ) { 
 
  sh_num_ref_idx_active_override_flag u(1) 
  if( sh_num_ref_idx_active_override_flag )  
   for( i = 0; i < ( sh_slice_type  ==  B ? 2: 1 ); i++ )  
    if( num_ref_entries[ i ][ RplsIdx[ i ] ] > 1 )  
     sh_num_ref_idx_active_minus1[ i ] ue(v) 
 }  
 if( sh_slice_type  !=  I ) {  
  if( pps_cabac_init_present_flag )  
   sh_cabac_init_flag u(1) 
  if( ph_temporal_mvp_enabled_flag  &&  !pps_rpl_info_in_ph_flag ) {  
   if( sh_slice_type  ==  B )  
    sh_collocated_from_l0_flag u(1) 
   if( ( sh_collocated_from_l0_flag  &&  NumRefIdxActive[ 0 ] > 1 )  || 
     ( ! sh_collocated_from_l0_flag  &&  NumRefIdxActive[ 1 ] > 1 ) ) 
 
    sh_collocated_ref_idx ue(v) 
      }  
  if( !pps_wp_info_in_ph_flag  && 
    ( ( pps_weighted_pred_flag  &&  sh_slice_type  ==  P )  || 
    ( pps_weighted_bipred_flag  &&  sh_slice_type  ==  B ) ) ) 
 
   pred_weight_table()  
 }   
 if( !pps_qp_delta_info_in_ph_flag )  
  sh_qp_delta se(v) 
 if( pps_slice_chroma_qp_offsets_present_flag ) {  
  sh_cb_qp_offset se(v) 
  sh_cr_qp_offset se(v) 
  if( sps_joint_cbcr_enabled_flag )  
   sh_joint_cbcr_qp_offset se(v) 
 }  
 if( pps_cu_chroma_qp_offset_list_enabled_flag )  
  sh_cu_chroma_qp_offset_enabled_flag u(1) 
 if( sps_sao_enabled_flag  &&  !pps_sao_info_in_ph_flag ) {  
  sh_sao_luma_used_flag u(1) 
  if( sps_chroma_format_idc  !=  0 )  
   sh_sao_chroma_used_flag u(1) 
 }  
 if( pps_deblocking_filter_override_enabled_flag  &&  !pps_dbf_info_in_ph_flag )  
  sh_deblocking_params_present_flag u(1) 
 if( sh_deblocking_params_present_flag ) {  
  if( !pps_deblocking_filter_disabled_flag )  
   sh_deblocking_filter_disabled_flag u(1) 
  if( !sh_deblocking_filter_disabled_flag ) {  
   sh_luma_beta_offset_div2 se(v) 
   sh_luma_tc_offset_div2 se(v) 
   if( pps_chroma_tool_offsets_present_flag ) {  
    sh_cb_beta_offset_div2 se(v) 
    sh_cb_tc_offset_div2 se(v) 
    sh_cr_beta_offset_div2 se(v) 
    sh_cr_tc_offset_div2 se(v) 
   }  
  }  
 }  
 if( sps_dep_quant_enabled_flag )  
  sh_dep_quant_used_flag u(1) 
 if( sps_sign_data_hiding_enabled_flag  &&  !sh_dep_quant_used_flag )  
  sh_sign_data_hiding_used_flag u(1) 
 if( sps_transform_skip_enabled_flag  &&  !sh_dep_quant_used_flag  && 
   !sh_sign_data_hiding_used_flag ) 
 
  sh_ts_residual_coding_disabled_flag u(1) 
 if( pps_slice_header_extension_present_flag ) {  
  sh_slice_header_extension_length ue(v) 
  for( i = 0; i < sh_slice_header_extension_length; i++)   
   sh_slice_header_extension_data_byte[ i ] u(8) 
 }  
  if( NumEntryPoints > 0 ) {  
  sh_entry_offset_len_minus1 ue(v) 
  for( i = 0; i < NumEntryPoints; i++ )  
   sh_entry_point_offset_minus1[ i ] u(v) 
 }  
 byte_alignment()  
}  

pred_weight_table() {  
 luma_log2_weight_denom ue(v) 
 if( sps_chroma_format_idc  !=  0 )  
  delta_chroma_log2_weight_denom se(v) 
 if( pps_wp_info_in_ph_flag )  
  num_l0_weights ue(v) 
 for( i = 0; i < NumWeightsL0; i++ )  
  luma_weight_l0_flag[ i ] u(1) 
 if( sps_chroma_format_idc  !=  0 )  
  for( i = 0; i < NumWeightsL0; i++ )  
   chroma_weight_l0_flag[ i ] u(1) 
 for( i = 0; i < NumWeightsL0; i++ ) {  
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
 if(  pps_weighted_bipred_flag  &&  pps_wp_info_in_ph_flag  && 
   num_ref_entries[ 1 ][ RplsIdx[ 1 ] ] > 0 ) 
 
  num_l1_weights ue(v) 
 for( i = 0; i < NumWeightsL1; i++ )  
  luma_weight_l1_flag[ i ] u(1) 
 if( sps_chroma_format_idc  !=  0 )  
  for( i = 0; i < NumWeightsL1; i++ )  
   chroma_weight_l1_flag[ i ] u(1) 
 for( i = 0; i < NumWeightsL1; i++ ) {  
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

ref_pic_lists() {  
 for( i = 0; i < 2; i++ ) {  
  if( sps_num_ref_pic_lists[ i ] > 0  && 
     ( i  ==  0  ||  ( i  ==  1  &&  pps_rpl1_idx_present_flag ) ) ) 
 
   rpl_sps_flag[ i ] u(1) 
  if( rpl_sps_flag[ i ] ) {  
   if( sps_num_ref_pic_lists[ i ] > 1  && 
      ( i  ==  0  ||  ( i  ==  1  &&  pps_rpl1_idx_present_flag ) ) ) 
 
    rpl_idx[ i ] u(v) 
  } else  
   ref_pic_list_struct( i, sps_num_ref_pic_lists[ i ] )  
  for( j = 0; j < NumLtrpEntries[ i ][ RplsIdx[ i ] ]; j++ ) {  
   if( ltrp_in_header_flag[ i ][ RplsIdx[ i ] ] )  
    poc_lsb_lt[ i ][ j ] u(v) 
   delta_poc_msb_cycle_present_flag[ i ][ j ] u(1) 
   if( delta_poc_msb_cycle_present_flag[ i ][ j ] )  
    delta_poc_msb_cycle_lt[ i ][ j ] ue(v) 
  }  
 }  
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

slice_data() {  
 FirstCtbRowInSlice = 1  
 for( i = 0; i < NumCtusInCurrSlice; i++ ) {  
  CtbAddrInRs = CtbAddrInCurrSlice[ i ]  
  CtbAddrX = ( CtbAddrInRs % PicWidthInCtbsY )  
  CtbAddrY = ( CtbAddrInRs / PicWidthInCtbsY )  
  if( CtbAddrX  ==  CtbToTileColBd[ CtbAddrX ] ) {  
   NumHmvpCand = 0  
   NumHmvpIbcCand = 0  
   ResetIbcBuf = 1  
  }  
  coding_tree_unit()  
  if( i  ==  NumCtusInCurrSlice - 1 )  
   end_of_slice_one_bit  /* equal to 1 */ ae(v) 
  else if( CtbAddrX  ==  CtbToTileColBd[ CtbAddrX + 1 ] - 1 ) {  
   if( CtbAddrY  ==  CtbToTileRowBd[ CtbAddrY + 1 ] - 1 ) {  
    end_of_tile_one_bit  /* equal to 1 */ ae(v) 
    byte_alignment()  
   } else if( sps_entropy_coding_sync_enabled_flag ) {  
    end_of_subset_one_bit  /* equal to 1 */ ae(v) 
    byte_alignment()  
   }  
   FirstCtbRowInSlice = 0  
  }  
 }  
}  

coding_tree_unit() {  
 xCtb = CtbAddrX  <<  CtbLog2SizeY  
 yCtb = CtbAddrY  <<  CtbLog2SizeY  
 if( sh_sao_luma_used_flag  ||  sh_sao_chroma_used_flag )  
  sao( CtbAddrX, CtbAddrY )  
 if( sh_alf_enabled_flag ){  
  alf_ctb_flag[ 0 ][ CtbAddrX ][ CtbAddrY ] ae(v) 
  if( alf_ctb_flag[ 0 ][ CtbAddrX ][ CtbAddrY ] ) {  
   if( sh_num_alf_aps_ids_luma > 0 )  
       alf_use_aps_flag ae(v) 
   if( alf_use_aps_flag ) {  
    if( sh_num_alf_aps_ids_luma > 1 )  
     alf_luma_prev_filter_idx ae(v) 
   } else  
    alf_luma_fixed_filter_idx ae(v) 
  }  
  if( sh_alf_cb_enabled_flag ) {  
   alf_ctb_flag[ 1 ][ CtbAddrX ][ CtbAddrY ] ae(v) 
   if( alf_ctb_flag[ 1 ][ CtbAddrX ][ CtbAddrY ] 
    &&  alf_chroma_num_alt_filters_minus1 > 0 ) 
 
    alf_ctb_filter_alt_idx[ 0 ][ CtbAddrX ][ CtbAddrY ] ae(v) 
  }  
  if( sh_alf_cr_enabled_flag ) {  
   alf_ctb_flag[ 2 ][ CtbAddrX ][ CtbAddrY ] ae(v) 
   if( alf_ctb_flag[ 2 ][ CtbAddrX ][ CtbAddrY ] 
    &&  alf_chroma_num_alt_filters_minus1 > 0 ) 
 
    alf_ctb_filter_alt_idx[ 1 ][ CtbAddrX ][ CtbAddrY ] ae(v) 
  }  
 }  
 if( sh_alf_cc_cb_enabled_flag )  
  alf_ctb_cc_cb_idc[ CtbAddrX ][ CtbAddrY ] ae(v) 
 if( sh_alf_cc_cr_enabled_flag )  
  alf_ctb_cc_cr_idc[ CtbAddrX ][ CtbAddrY ] ae(v) 
 if( sh_slice_type  ==  I  &&  sps_qtbtt_dual_tree_intra_flag )  
  dual_tree_implicit_qt_split( xCtb, yCtb, CtbSizeY, 0 )  
 else  
  coding_tree( xCtb, yCtb, CtbSizeY, CtbSizeY, 1, 1, 0, 0, 0, 0, 0, 
       SINGLE_TREE, MODE_TYPE_ALL ) 
 
} 

dual_tree_implicit_qt_split( x0, y0, cbSize, cqtDepth ) {  
 cbSubdiv = 2 * cqtDepth  
 if( cbSize > 64 ) {  
  if( pps_cu_qp_delta_enabled_flag  &&  cbSubdiv  <=  CuQpDeltaSubdiv ) {  
   IsCuQpDeltaCoded = 0  
   CuQpDeltaVal = 0  
   CuQgTopLeftX = x0  
   CuQgTopLeftY = y0  
  }  
  if( sh_cu_chroma_qp_offset_enabled_flag  && 
    cbSubdiv  <=  CuChromaQpOffsetSubdiv ) { 
 
   IsCuChromaQpOffsetCoded = 0  
   CuQpOffsetCb = 0  
   CuQpOffsetCr = 0  
   CuQpOffsetCbCr = 0  
  }  
  x1 = x0 + ( cbSize / 2 )  
  y1 = y0 + ( cbSize / 2 ) 
    dual_tree_implicit_qt_split( x0, y0, cbSize / 2, cqtDepth + 1 )  
  if( x1 < pps_pic_width_in_luma_samples )  
   dual_tree_implicit_qt_split( x1, y0, cbSize / 2, cqtDepth + 1 )  
  if( y1 < pps_pic_height_in_luma_samples )  
   dual_tree_implicit_qt_split( x0, y1, cbSize / 2, cqtDepth + 1 )  
  if( x1 < pps_pic_width_in_luma_samples  &&  y1 < pps_pic_height_in_luma_samples )  
   dual_tree_implicit_qt_split( x1, y1, cbSize / 2, cqtDepth + 1 )  
 } else {  
  coding_tree( x0, y0, cbSize, cbSize, 1, 0, cbSubdiv, cqtDepth, 0, 0, 0, 
       DUAL_TREE_LUMA, MODE_TYPE_ALL ) 
 
  coding_tree( x0, y0, cbSize, cbSize, 0, 1, cbSubdiv, cqtDepth, 0, 0, 0, 
       DUAL_TREE_CHROMA, MODE_TYPE_ALL ) 
 
 }  
} 

sao( rx, ry ) {  
 if( rx > 0 ) {  
  leftCtbAvailable = rx  !=  CtbToTileColBd[ rx ]  
  if( leftCtbAvailable )  
   sao_merge_left_flag ae(v) 
 }  
 if( ry > 0  &&  !sao_merge_left_flag ) {  
  upCtbAvailable = ry  !=  CtbToTileRowBd[ ry ]  && !FirstCtbRowInSlice  
  if( upCtbAvailable )  
   sao_merge_up_flag ae(v) 
 }  
 if( !sao_merge_up_flag  &&  !sao_merge_left_flag )  
  for( cIdx = 0; cIdx < ( sps_chroma_format_idc  !=  0 ? 3 : 1 ); cIdx++ )  
   if( ( sh_sao_luma_used_flag  &&  cIdx  ==  0 )  || 
    ( sh_sao_chroma_used_flag  &&  cIdx > 0 ) ) { 
 
    if( cIdx  ==  0 )  
     sao_type_idx_luma ae(v) 
    else if( cIdx  ==  1 )  
     sao_type_idx_chroma ae(v) 
    if( SaoTypeIdx[ cIdx ][ rx ][ ry ]  !=  0 ) {  
     for( i = 0; i < 4; i++ )  
      sao_offset_abs[ cIdx ][ rx ][ ry ][ i ] ae(v) 
     if( SaoTypeIdx[ cIdx ][ rx ][ ry ]  ==  1 ) {  
      for( i = 0; i < 4; i++ )  
       if( sao_offset_abs[ cIdx ][ rx ][ ry ][ i ]  !=  0 )  
        sao_offset_sign_flag[ cIdx ][ rx ][ ry ][ i ] ae(v) 
      sao_band_position[ cIdx ][ rx ][ ry ] ae(v) 
     } else {  
      if( cIdx  ==  0 )  
       sao_eo_class_luma ae(v) 
      if( cIdx  ==  1 )  
             sao_eo_class_chroma ae(v) 
     }  
    }  
   }  
}  

coding_tree( x0, y0, cbWidth, cbHeight, qgOnY, qgOnC, cbSubdiv, cqtDepth, mttDepth, depthOffset,  
      partIdx, treeTypeCurr, modeTypeCurr ) { 
 
 if( ( allowSplitBtVer  ||  allowSplitBtHor  ||  allowSplitTtVer  ||  allowSplitTtHor  || 
   allowSplitQt )  &&   ( x0 + cbWidth  <=  pps_pic_width_in_luma_samples )  && 
   ( y0 + cbHeight  <=  pps_pic_height_in_luma_samples ) ) 
 
  split_cu_flag ae(v) 
 if( pps_cu_qp_delta_enabled_flag  &&  qgOnY  &&  cbSubdiv  <=  CuQpDeltaSubdiv ) {  
  IsCuQpDeltaCoded = 0  
  CuQpDeltaVal = 0  
  CuQgTopLeftX = x0  
  CuQgTopLeftY = y0  
 }  
 if( sh_cu_chroma_qp_offset_enabled_flag  &&  qgOnC  && 
   cbSubdiv  <=  CuChromaQpOffsetSubdiv ) { 
 
  IsCuChromaQpOffsetCoded = 0  
  CuQpOffsetCb = 0  
  CuQpOffsetCr = 0  
  CuQpOffsetCbCr = 0  
 }  
 if( split_cu_flag ) {  
  if( ( allowSplitBtVer  ||  allowSplitBtHor  ||  allowSplitTtVer  ||  allowSplitTtHor )  && 
    allowSplitQt ) 
 
   split_qt_flag ae(v) 
  if( !split_qt_flag ) {  
   if( ( allowSplitBtHor  ||  allowSplitTtHor )  &&  ( allowSplitBtVer  ||  allowSplitTtVer ) )  
    mtt_split_cu_vertical_flag ae(v) 
   if( ( allowSplitBtVer  &&  allowSplitTtVer  &&  mtt_split_cu_vertical_flag )  || 
     ( allowSplitBtHor  &&  allowSplitTtHor  &&  !mtt_split_cu_vertical_flag ) ) 
 
    mtt_split_cu_binary_flag ae(v) 
  }  
  if( ModeTypeCondition  ==  1 )  
   modeType = MODE_TYPE_INTRA  
  else if( ModeTypeCondition  ==  2 ) {  
   non_inter_flag ae(v) 
   modeType = non_inter_flag ? MODE_TYPE_INTRA : MODE_TYPE_INTER  
  } else  
   modeType = modeTypeCurr  
  treeType = ( modeType  ==  MODE_TYPE_INTRA ) ? DUAL_TREE_LUMA : treeTypeCurr  
  if( !split_qt_flag ) {  
   if( MttSplitMode[ x0 ][ y0 ][ mttDepth ]  ==  SPLIT_BT_VER ) {  
    depthOffset  +=  ( x0 + cbWidth > pps_pic_width_in_luma_samples ) ? 1 : 0 
        x1 = x0 + ( cbWidth / 2 )  
    coding_tree( x0, y0, cbWidth / 2, cbHeight, qgOnY, qgOnC, cbSubdiv + 1, 
          cqtDepth, mttDepth + 1, depthOffset, 0, treeType, modeType ) 
 
    if( x1 < pps_pic_width_in_luma_samples )  
     coding_tree( x1, y0, cbWidth / 2, cbHeight, qgOnY, qgOnC, cbSubdiv + 1,  
          cqtDepth, mttDepth + 1, depthOffset, 1, treeType, modeType ) 
 
   } else if( MttSplitMode[ x0 ][ y0 ][ mttDepth ]  ==  SPLIT_BT_HOR ) {  
    depthOffset  +=  ( y0 + cbHeight > pps_pic_height_in_luma_samples ) ? 1 : 0  
    y1 = y0 + ( cbHeight / 2 )  
    coding_tree( x0, y0, cbWidth, cbHeight / 2, qgOnY, qgOnC, cbSubdiv + 1, 
          cqtDepth, mttDepth + 1, depthOffset, 0, treeType, modeType ) 
 
    if( y1 < pps_pic_height_in_luma_samples )  
     coding_tree( x0, y1, cbWidth, cbHeight / 2, qgOnY, qgOnC, cbSubdiv + 1, 
          cqtDepth, mttDepth + 1, depthOffset, 1, treeType, modeType ) 
 
   } else if( MttSplitMode[ x0 ][ y0 ][ mttDepth ]  ==  SPLIT_TT_VER ) {  
    x1 = x0 + ( cbWidth / 4 )  
    x2 = x0 + ( 3 * cbWidth / 4 )  
    qgNextOnY = qgOnY  &&  ( cbSubdiv + 2  <=  CuQpDeltaSubdiv )  
    qgNextOnC = qgOnC  &&  ( cbSubdiv + 2  <=  CuChromaQpOffsetSubdiv )  
    coding_tree( x0, y0, cbWidth / 4, cbHeight, qgNextOnY, qgNextOnC, cbSubdiv + 2, 
          cqtDepth, mttDepth + 1, depthOffset, 0, treeType, modeType ) 
 
    coding_tree( x1, y0, cbWidth / 2, cbHeight, qgNextOnY, qgNextOnC, cbSubdiv + 1, 
          cqtDepth, mttDepth + 1, depthOffset, 1, treeType, modeType ) 
 
    coding_tree( x2, y0, cbWidth / 4, cbHeight, qgNextOnY, qgNextOnC, cbSubdiv + 2, 
          cqtDepth, mttDepth + 1, depthOffset, 2, treeType, modeType ) 
 
   } else { /* SPLIT_TT_HOR */  
    y1 = y0 + ( cbHeight / 4 )  
    y2 = y0 + ( 3 * cbHeight / 4 )  
    qgNextOnY = qgOnY  &&  ( cbSubdiv + 2  <=  CuQpDeltaSubdiv )  
    qgNextOnC = qgOnC  &&  ( cbSubdiv + 2  <=  CuChromaQpOffsetSubdiv )  
    coding_tree( x0, y0, cbWidth, cbHeight / 4, qgNextOnY, qgNextOnC, cbSubdiv + 2, 
          cqtDepth, mttDepth + 1, depthOffset, 0, treeType, modeType ) 
 
    coding_tree( x0, y1, cbWidth, cbHeight / 2, qgNextOnY, qgNextOnC, cbSubdiv + 1, 
          cqtDepth, mttDepth + 1, depthOffset, 1, treeType, modeType ) 
 
    coding_tree( x0, y2, cbWidth, cbHeight / 4, qgNextOnY, qgNextOnC, cbSubdiv + 2, 
          cqtDepth, mttDepth + 1, depthOffset, 2, treeType, modeType ) 
 
   }  
  } else {  
   x1 = x0 + ( cbWidth / 2 )  
   y1 = y0 + ( cbHeight / 2 )  
   coding_tree( x0, y0, cbWidth / 2, cbHeight / 2, qgOnY, qgOnC, cbSubdiv + 2, 
         cqtDepth + 1, 0, 0, 0, treeType, modeType ) 
 
   if( x1 < pps_pic_width_in_luma_samples )  
    coding_tree( x1, y0, cbWidth / 2, cbHeight / 2, qgOnY, qgOnC, cbSubdiv + 2, 
         cqtDepth + 1, 0, 0, 1, treeType, modeType ) 
 
   if( y1 < pps_pic_height_in_luma_samples )  
    coding_tree( x0, y1, cbWidth / 2, cbHeight / 2, qgOnY, qgOnC, cbSubdiv + 2, 
         cqtDepth + 1, 0, 0, 2, treeType, modeType ) 
 
   if( y1 < pps_pic_height_in_luma_samples  &&  x1 < pps_pic_width_in_luma_samples )  
    coding_tree( x1, y1, cbWidth / 2, cbHeight / 2, qgOnY, qgOnC, cbSubdiv + 2, 
         cqtDepth + 1, 0, 0, 3, treeType, modeType ) 
 
  }  
  if( modeTypeCurr == MODE_TYPE_ALL && modeType == MODE_TYPE_INTRA )  
   coding_tree( x0, y0, cbWidth, cbHeight, 0, qgOnC, cbSubdiv, cqtDepth, mttDepth, 0, 0, 
          DUAL_TREE_CHROMA, modeType ) 
 
 } else  
  coding_unit( x0, y0, cbWidth, cbHeight, cqtDepth, treeTypeCurr, modeTypeCurr )  
}  

coding_unit( x0, y0, cbWidth, cbHeight, cqtDepth, treeType, modeType ) {  
 if( sh_slice_type  ==  I  &&  ( cbWidth > 64  ||  cbHeight > 64 ) )  
  modeType = MODE_TYPE_INTRA  
 chType = treeType == DUAL_TREE_CHROMA ? 1 : 0  
 if( sh_slice_type  !=  I  ||  sps_ibc_enabled_flag ) {  
  if( treeType  !=  DUAL_TREE_CHROMA  && 
    ( ( !( cbWidth  ==  4  &&  cbHeight  ==  4 )  && 
    modeType  !=  MODE_TYPE_INTRA )  || 
    ( sps_ibc_enabled_flag  &&  cbWidth  <=  64  &&  cbHeight  <=  64 ) ) ) 
 
   cu_skip_flag[ x0 ][ y0 ] ae(v) 
  if( cu_skip_flag[ x0 ][ y0 ]  ==  0  &&  sh_slice_type  !=  I  && 
    !( cbWidth  ==  4  &&  cbHeight  ==  4 )  &&  modeType  ==  MODE_TYPE_ALL ) 
 
   pred_mode_flag ae(v) 
  if( ( ( sh_slice_type  ==  I  &&  cu_skip_flag[ x0 ][ y0 ] ==0 )  || 
    ( sh_slice_type  !=  I  &&  ( CuPredMode[ chType ][ x0 ][ y0 ] !=  MODE_INTRA  || 
    ( ( ( cbWidth  ==  4  &&  cbHeight  ==  4 )  ||  modeType  == MODE_TYPE_INTRA ) 
     &&  cu_skip_flag[ x0 ][ y0 ]  ==  0 ) ) ) )  && 
    cbWidth  <=  64  &&  cbHeight  <= 64  &&  modeType  !=  MODE_TYPE_INTER  && 
    sps_ibc_enabled_flag  &&  treeType  !=  DUAL_TREE_CHROMA ) 
 
   pred_mode_ibc_flag ae(v) 
 }  
 if( CuPredMode[ chType ][ x0 ][ y0 ]  ==  MODE_INTRA  &&  sps_palette_enabled_flag  && 
   cbWidth  <=  64  &&  cbHeight  <=  64  &&  cu_skip_flag[ x0 ][ y0 ]  ==  0  && 
   modeType  !=  MODE_TYPE_INTER  &&  ( ( cbWidth * cbHeight ) >  
   ( treeType  !=  DUAL_TREE_CHROMA ? 16 : 16 * SubWidthC * SubHeightC ) )  && 
   ( modeType  !=  MODE_TYPE_INTRA  ||  treeType  !=  DUAL_TREE_CHROMA ) ) 
 
  pred_mode_plt_flag ae(v) 
 if( CuPredMode[ chType ][ x0 ][ y0 ]  ==  MODE_INTRA  &&  sps_act_enabled_flag  && 
   treeType  ==  SINGLE_TREE ) 
 
  cu_act_enabled_flag ae(v) 
 if( CuPredMode[ chType ][ x0 ][ y0 ]  ==  MODE_INTRA  || 
   CuPredMode[ chType ][ x0 ][ y0 ]  ==  MODE_PLT ) { 
 
  if( treeType  ==  SINGLE_TREE  ||  treeType  ==  DUAL_TREE_LUMA ) {  
   if( pred_mode_plt_flag )  
    palette_coding( x0, y0, cbWidth, cbHeight, treeType )  
   else {  
    if( sps_bdpcm_enabled_flag  && 
      cbWidth  <=  MaxTsSize  &&  cbHeight  <=  MaxTsSize ) 
 
     intra_bdpcm_luma_flag ae(v) 
    if( intra_bdpcm_luma_flag )  
     intra_bdpcm_luma_dir_flag ae(v) 
    else {  
         if( sps_mip_enabled_flag )  
      intra_mip_flag ae(v) 
     if( intra_mip_flag ) {  
      intra_mip_transposed_flag[ x0 ][ y0 ] ae(v) 
      intra_mip_mode[ x0 ][ y0 ] ae(v) 
     } else {  
      if( sps_mrl_enabled_flag  &&  ( ( y0 % CtbSizeY ) > 0 ) )  
       intra_luma_ref_idx ae(v) 
      if( sps_isp_enabled_flag  &&  intra_luma_ref_idx  ==  0  && 
        ( cbWidth  <=  MaxTbSizeY  &&  cbHeight  <=  MaxTbSizeY )  && 
        ( cbWidth * cbHeight > MinTbSizeY * MinTbSizeY )  && 
        !cu_act_enabled_flag ) 
 
       intra_subpartitions_mode_flag ae(v) 
      if( intra_subpartitions_mode_flag  ==  1 )  
       intra_subpartitions_split_flag ae(v) 
      if( intra_luma_ref_idx  ==  0 )  
       intra_luma_mpm_flag[ x0 ][ y0 ] ae(v) 
      if( intra_luma_mpm_flag[ x0 ][ y0 ] ) {  
       if( intra_luma_ref_idx  ==  0 )  
        intra_luma_not_planar_flag[ x0 ][ y0 ] ae(v) 
       if( intra_luma_not_planar_flag[ x0 ][ y0 ] )  
        intra_luma_mpm_idx[ x0 ][ y0 ] ae(v) 
      } else  
       intra_luma_mpm_remainder[ x0 ][ y0 ] ae(v) 
     }  
    }  
   }  
  }  
  if( ( treeType  ==  SINGLE_TREE  ||  treeType  ==  DUAL_TREE_CHROMA ) && 
    sps_chroma_format_idc  !=  0 ) { 
 
   if( pred_mode_plt_flag   &&  treeType  ==  DUAL_TREE_CHROMA )  
    palette_coding( x0, y0, cbWidth / SubWidthC, cbHeight / SubHeightC, treeType )  
   else if( !pred_mode_plt_flag ) {  
    if( !cu_act_enabled_flag ) {  
     if( cbWidth / SubWidthC  <=  MaxTsSize  &&  cbHeight / SubHeightC  <=  MaxTsSize 
       &&  sps_bdpcm_enabled_flag ) 
 
      intra_bdpcm_chroma_flag ae(v) 
     if( intra_bdpcm_chroma_flag )  
      intra_bdpcm_chroma_dir_flag ae(v) 
     else {  
      if( CclmEnabled )  
       cclm_mode_flag ae(v) 
      if( cclm_mode_flag )  
       cclm_mode_idx ae(v) 
      else  
       intra_chroma_pred_mode ae(v) 
     }  
    }  
   }  
     }  
 } else if( treeType  !=  DUAL_TREE_CHROMA ) { /* MODE_INTER or MODE_IBC */  
  if( cu_skip_flag[ x0 ][ y0 ]  ==  0 )  
   general_merge_flag[ x0 ][ y0 ] ae(v) 
  if( general_merge_flag[ x0 ][ y0 ] )  
   merge_data( x0, y0, cbWidth, cbHeight, chType )  
  else if( CuPredMode[ chType ][ x0 ][ y0 ]  ==  MODE_IBC ) {  
   mvd_coding( x0, y0, 0, 0 )  
   if( MaxNumIbcMergeCand > 1 )  
    mvp_l0_flag[ x0 ][ y0 ] ae(v) 
   if( sps_amvr_enabled_flag  && 
     ( MvdL0[ x0 ][ y0 ][ 0 ]  !=  0  ||  MvdL0[ x0 ][ y0 ][ 1 ]  !=  0 ) ) 
 
    amvr_precision_idx[ x0 ][ y0 ] ae(v) 
  } else {  
   if( sh_slice_type  ==  B )  
    inter_pred_idc[ x0 ][ y0 ] ae(v) 
   if( sps_affine_enabled_flag  &&  cbWidth  >=  16  &&  cbHeight  >=  16 ) {  
    inter_affine_flag[ x0 ][ y0 ] ae(v) 
    if( sps_6param_affine_enabled_flag  &&  inter_affine_flag[ x0 ][ y0 ] )  
     cu_affine_type_flag[ x0 ][ y0 ] ae(v) 
   }  
   if( sps_smvd_enabled_flag  &&  !ph_mvd_l1_zero_flag  && 
     inter_pred_idc[ x0 ][ y0 ]  ==  PRED_BI  && 
     !inter_affine_flag[ x0 ][ y0 ]  &&  RefIdxSymL0 > -1  &&  RefIdxSymL1 > -1 ) 
 
    sym_mvd_flag[ x0 ][ y0 ] ae(v) 
   if( inter_pred_idc[ x0 ][ y0 ]  !=  PRED_L1 ) {  
    if( NumRefIdxActive[ 0 ] > 1  &&  !sym_mvd_flag[ x0 ][ y0 ] )  
     ref_idx_l0[ x0 ][ y0 ] ae(v) 
    mvd_coding( x0, y0, 0, 0 )  
    if( MotionModelIdc[ x0 ][ y0 ] > 0 )  
     mvd_coding( x0, y0, 0, 1 )  
    if(MotionModelIdc[ x0 ][ y0 ] > 1 )  
     mvd_coding( x0, y0, 0, 2 )  
    mvp_l0_flag[ x0 ][ y0 ] ae(v) 
   } else {  
    MvdL0[ x0 ][ y0 ][ 0 ] = 0  
    MvdL0[ x0 ][ y0 ][ 1 ] = 0  
   }  
   if( inter_pred_idc[ x0 ][ y0 ]  !=  PRED_L0 ) {  
    if( NumRefIdxActive[ 1 ] > 1  &&  !sym_mvd_flag[ x0 ][ y0 ] )  
     ref_idx_l1[ x0 ][ y0 ] ae(v) 
    if( ph_mvd_l1_zero_flag  &&  inter_pred_idc[ x0 ][ y0 ]  ==  PRED_BI ) {  
     MvdL1[ x0 ][ y0 ][ 0 ] = 0  
     MvdL1[ x0 ][ y0 ][ 1 ] = 0  
     MvdCpL1[ x0 ][ y0 ][ 0 ][ 0 ] = 0  
     MvdCpL1[ x0 ][ y0 ][ 0 ][ 1 ] = 0  
     MvdCpL1[ x0 ][ y0 ][ 1 ][ 0 ] = 0  
     MvdCpL1[ x0 ][ y0 ][ 1 ][ 1 ] = 0  
     MvdCpL1[ x0 ][ y0 ][ 2 ][ 0 ] = 0  
          MvdCpL1[ x0 ][ y0 ][ 2 ][ 1 ] = 0  
    } else {  
     if( sym_mvd_flag[ x0 ][ y0 ] ) {  
      MvdL1[ x0 ][ y0 ][ 0 ] = -MvdL0[ x0 ][ y0 ][ 0 ]  
      MvdL1[ x0 ][ y0 ][ 1 ] = -MvdL0[ x0 ][ y0 ][ 1 ]  
     } else  
      mvd_coding( x0, y0, 1, 0 )  
     if( MotionModelIdc[ x0 ][ y0 ] > 0 )  
      mvd_coding( x0, y0, 1, 1 )  
     if(MotionModelIdc[ x0 ][ y0 ] > 1 )  
      mvd_coding( x0, y0, 1, 2 )  
    }  
    mvp_l1_flag[ x0 ][ y0 ] ae(v) 
   } else {  
    MvdL1[ x0 ][ y0 ][ 0 ] = 0  
    MvdL1[ x0 ][ y0 ][ 1 ] = 0  
   }  
   if( ( sps_amvr_enabled_flag  &&  inter_affine_flag[ x0 ][ y0 ]  ==  0  && 
     ( MvdL0[ x0 ][ y0 ][ 0 ]  !=  0  ||  MvdL0[ x0 ][ y0 ][ 1 ]  !=  0  || 
     MvdL1[ x0 ][ y0 ][ 0 ]  !=  0  ||  MvdL1[ x0 ][ y0 ][ 1 ]  !=  0 ) )  || 
     ( sps_affine_amvr_enabled_flag  &&  inter_affine_flag[ x0 ][ y0 ]  ==  1  && 
     ( MvdCpL0[ x0 ][ y0 ][ 0 ][ 0 ]  !=  0  ||  MvdCpL0[ x0 ][ y0 ][ 0 ][ 1 ]  !=  0  || 
     MvdCpL1[ x0 ][ y0 ][ 0 ][ 0 ]  !=  0  ||  MvdCpL1[ x0 ][ y0 ][ 0 ][ 1 ]  !=  0  || 
     MvdCpL0[ x0 ][ y0 ][ 1 ][ 0 ]  !=  0  ||  MvdCpL0[ x0 ][ y0 ][ 1 ][ 1 ]  !=  0  || 
      MvdCpL1[ x0 ][ y0 ][ 1 ][ 0 ]  !=  0  ||  MvdCpL1[ x0 ][ y0 ][ 1 ][ 1 ]  !=  0  || 
     MvdCpL0[ x0 ][ y0 ][ 2 ][ 0 ]  !=  0  ||  MvdCpL0[ x0 ][ y0 ][ 2 ][ 1 ]  !=  0  || 
     MvdCpL1[ x0 ][ y0 ][ 2 ][ 0 ]  !=  0  ||  MvdCpL1[ x0 ][ y0 ][ 2 ][ 1 ]  !=  0 ) ) ) { 
 
    amvr_flag[ x0 ][ y0 ] ae(v) 
    if( amvr_flag[ x0 ][ y0 ] )  
     amvr_precision_idx[ x0 ][ y0 ] ae(v) 
   }  
   if( sps_bcw_enabled_flag  &&  inter_pred_idc[ x0 ][ y0 ]  ==  PRED_BI  && 
     luma_weight_l0_flag[ ref_idx_l0 [ x0 ][ y0 ] ]  ==  0  && 
     luma_weight_l1_flag[ ref_idx_l1 [ x0 ][ y0 ] ]  ==  0  && 
     chroma_weight_l0_flag[ ref_idx_l0 [ x0 ][ y0 ] ]  ==  0  && 
     chroma_weight_l1_flag[ ref_idx_l1 [ x0 ][ y0 ] ]  ==  0  && 
     cbWidth * cbHeight  >=  256 ) 
 
    bcw_idx[ x0 ][ y0 ] ae(v) 
  }  
 }  
 if( CuPredMode[ chType ][ x0 ][ y0 ]  !=  MODE_INTRA  && !pred_mode_plt_flag  && 
   general_merge_flag[ x0 ][ y0 ]  ==  0 ) 
 
  cu_coded_flag ae(v) 
 if( cu_coded_flag ) {  
  if( CuPredMode[ chType ][ x0 ][ y0 ]  ==  MODE_INTER  &&  sps_sbt_enabled_flag  &&   
   !ciip_flag[ x0 ][ y0 ]  &&  cbWidth  <=  MaxTbSizeY  &&  cbHeight  <=  MaxTbSizeY ) { 
 
   allowSbtVerH = cbWidth  >=  8  
   allowSbtVerQ = cbWidth  >=  16  
   allowSbtHorH = cbHeight  >=  8  
   allowSbtHorQ = cbHeight  >=  16  
   if( allowSbtVerH  ||  allowSbtHorH )  
    cu_sbt_flag ae(v) 
   if( cu_sbt_flag ) {  
       if( ( allowSbtVerH  ||  allowSbtHorH )  &&  ( allowSbtVerQ  ||  allowSbtHorQ ) )  
     cu_sbt_quad_flag ae(v) 
    if( ( cu_sbt_quad_flag  &&  allowSbtVerQ  &&  allowSbtHorQ )  || 
      ( !cu_sbt_quad_flag  &&  allowSbtVerH  &&  allowSbtHorH ) ) 
 
     cu_sbt_horizontal_flag ae(v) 
    cu_sbt_pos_flag ae(v) 
   }  
  }  
  if( sps_act_enabled_flag  &&  CuPredMode[ chType ][ x0 ][ y0 ]  !=  MODE_INTRA  && 
    treeType  ==  SINGLE_TREE ) 
 
   cu_act_enabled_flag ae(v) 
  LfnstDcOnly = 1  
  LfnstZeroOutSigCoeffFlag = 1  
  MtsDcOnly = 1  
  MtsZeroOutSigCoeffFlag = 1  
  transform_tree( x0, y0, cbWidth, cbHeight, treeType, chType )  
  lfnstWidth = ( treeType  ==  DUAL_TREE_CHROMA ) ? cbWidth / SubWidthC : ( ( IntraSubPartitionsSplitType  ==  ISP_VER_SPLIT ) ? cbWidth / NumIntraSubPartitions : cbWidth ) 
 
  lfnstHeight = ( treeType  ==  DUAL_TREE_CHROMA ) ? cbHeight / SubHeightC : ( ( IntraSubPartitionsSplitType  ==  ISP_HOR_SPLIT) ? cbHeight / NumIntraSubPartitions : cbHeight ) 
 
  lfnstNotTsFlag = ( treeType  ==  DUAL_TREE_CHROMA  ||  !tu_y_coded_flag[ x0 ][ y0 ]  || transform_skip_flag[ x0 ][ y0 ][ 0 ]  ==  0 )  && ( treeType  ==  DUAL_TREE_LUMA  || ( ( !tu_cb_coded_flag[ x0 ][ y0 ]  ||  transform_skip_flag[ x0 ][ y0 ][ 1 ]  ==  0 )  && ( !tu_cr_coded_flag[ x0 ][ y0 ]  ||  transform_skip_flag[ x0 ][ y0 ][ 2 ]  ==  0 ) ) ) 
 
  if( Min( lfnstWidth, lfnstHeight )  >=  4  &&  sps_lfnst_enabled_flag  ==  1  && 
    CuPredMode[ chType ][ x0 ][ y0 ]  ==  MODE_INTRA  &&  lfnstNotTsFlag  ==  1  && 
    ( treeType  ==  DUAL_TREE_CHROMA  ||  !IntraMipFlag[ x0 ][ y0 ]  || 
     Min( lfnstWidth, lfnstHeight )  >=  16 )   && 
    Max( cbWidth, cbHeight )  <=  MaxTbSizeY) { 
 
   if( ( IntraSubPartitionsSplitType  !=  ISP_NO_SPLIT  ||  LfnstDcOnly  ==  0 )  && 
     LfnstZeroOutSigCoeffFlag  ==  1 ) 
 
    lfnst_idx ae(v) 
  }  
  if( treeType  !=  DUAL_TREE_CHROMA  &&  lfnst_idx  ==  0  && 
    transform_skip_flag[ x0 ][ y0 ][ 0 ]  ==  0  &&  Max( cbWidth, cbHeight )  <=  32  && 
    IntraSubPartitionsSplitType  ==  ISP_NO_SPLIT  &&  cu_sbt_flag  ==  0  && 
    MtsZeroOutSigCoeffFlag  ==  1  &&  MtsDcOnly  ==  0 ) { 
 
   if( ( ( CuPredMode[ chType ][ x0 ][ y0 ]  ==  MODE_INTER  && 
     sps_explicit_mts_inter_enabled_flag )  || 
     ( CuPredMode[ chType ][ x0 ][ y0 ]  ==  MODE_INTRA  && 
     sps_explicit_mts_intra_enabled_flag ) ) ) 
 
    mts_idx ae(v) 
  }  
 }  
}  

palette_coding( x0, y0, cbWidth, cbHeight, treeType ) {  
 startComp = ( treeType  ==  DUAL_TREE_CHROMA ) ? 1 : 0 
  numComps = ( treeType  ==  SINGLE_TREE ) ? ( sps_chroma_format_idc  ==  0 ? 1 : 3 ) :  ( treeType  ==  DUAL_TREE_CHROMA ) ? 2 : 1 
 
 maxNumPaletteEntries = ( treeType == SINGLE_TREE ) ? 31 : 15  
 palettePredictionFinished = 0  
 NumPredictedPaletteEntries = 0  
 for( predictorEntryIdx = 0; predictorEntryIdx < PredictorPaletteSize[ startComp ]  &&  !palettePredictionFinished  && NumPredictedPaletteEntries < maxNumPaletteEntries; predictorEntryIdx++ ) { 
  palette_predictor_run ae(v) 
  if( palette_predictor_run  !=  1 ) {  
   if( palette_predictor_run > 1 )   
    predictorEntryIdx  +=  palette_predictor_run - 1  
   PalettePredictorEntryReuseFlags[ predictorEntryIdx ] = 1  
   NumPredictedPaletteEntries++  
  } else  
   palettePredictionFinished = 1  
 }  
 if( NumPredictedPaletteEntries < maxNumPaletteEntries )  
  num_signalled_palette_entries ae(v) 
 for( cIdx = startComp; cIdx < ( startComp + numComps ); cIdx++ )  
  for( i = 0; i < num_signalled_palette_entries; i++ )   
   new_palette_entries[ cIdx ][ i ] ae(v) 
 if( CurrentPaletteSize[ startComp ] > 0 )  
  palette_escape_val_present_flag ae(v) 
 if( MaxPaletteIndex > 0 ) {  
  adjust = 0  
  palette_transpose_flag ae(v) 
 }  
 if( treeType  !=  DUAL_TREE_CHROMA  &&  palette_escape_val_present_flag )  
  if( pps_cu_qp_delta_enabled_flag  &&  !IsCuQpDeltaCoded ) {  
   cu_qp_delta_abs ae(v) 
   if( cu_qp_delta_abs )  
    cu_qp_delta_sign_flag ae(v) 
  }  
 if( treeType  !=  DUAL_TREE_LUMA  &&  palette_escape_val_present_flag )  
  if( sh_cu_chroma_qp_offset_enabled_flag  &&  !IsCuChromaQpOffsetCoded ) {  
   cu_chroma_qp_offset_flag ae(v) 
   if( cu_chroma_qp_offset_flag  &&  pps_chroma_qp_offset_list_len_minus1 > 0 )  
    cu_chroma_qp_offset_idx ae(v) 
  }  
 PreviousRunPosition = 0  
 PreviousRunType = 0  
 for( subSetId = 0; subSetId  <=  ( cbWidth * cbHeight - 1 ) / 16; subSetId++ ) {  
  minSubPos = subSetId * 16  
  if( minSubPos + 16 > cbWidth * cbHeight)  
   maxSubPos = cbWidth * cbHeight  
  else   
   maxSubPos = minSubPos + 16  
  RunCopyMap[ x0 ][ y0 ] = 0  
  PaletteScanPos = minSubPos  
  log2CbWidth = Log2( cbWidth )  
  log2CbHeight = Log2( cbHeight )  
  while( PaletteScanPos < maxSubPos ) {  
   xC = x0 + TraverseScanOrder[ log2CbWidth ][ log2CbHeight ][ PaletteScanPos ][ 0 ]  
   yC = y0 + TraverseScanOrder[ log2CbWidth ][ log2CbHeight ][ PaletteScanPos ][ 1 ]  
   if( PaletteScanPos > 0 ) {  
    xcPrev = x0 + TraverseScanOrder[ log2CbWidth ][ log2CbHeight ][ PaletteScanPos - 1 ][ 0 ]  
    ycPrev = y0 + TraverseScanOrder[ log2CbWidth ][ log2CbHeight ][ PaletteScanPos - 1 ][ 1 ]  
   }  
   if( MaxPaletteIndex > 0  &&  PaletteScanPos > 0 ) {  
    run_copy_flag ae(v) 
    RunCopyMap[ xC ][ yC ] = run_copy_flag  
   }  
   CopyAboveIndicesFlag[ xC ][ yC ] = 0  
   if( MaxPaletteIndex > 0  &&  !RunCopyMap[ xC ][ yC ] ) {  
    if( ( ( !palette_transpose_flag  &&  yC > y0 )  ||  ( palette_transpose_flag  &&  xC > x0 ) ) &&  CopyAboveIndicesFlag[ xcPrev ][ ycPrev ]  ==  0  &&  PaletteScanPos > 0 ) { 
     copy_above_palette_indices_flag ae(v) 
     CopyAboveIndicesFlag[ xC ][ yC ] = copy_above_palette_indices_flag  
    }  
    PreviousRunType = CopyAboveIndicesFlag[ xC ][ yC ]  
    PreviousRunPosition = PaletteScanPos  
   }
   else if (PaletteScanPos > 0)  
    CopyAboveIndicesFlag[ xC ][ yC ] = CopyAboveIndicesFlag[ xcPrev ][ ycPrev ]  
   PaletteScanPos++  
  }  
  PaletteScanPos = minSubPos  
  while( PaletteScanPos < maxSubPos ) {  
   xC = x0 + TraverseScanOrder[ log2CbWidth ][ log2CbHeight ][ PaletteScanPos ][ 0 ]  
   yC = y0 + TraverseScanOrder[ log2CbWidth ][ log2CbHeight ][ PaletteScanPos ][ 1 ]  
   if( PaletteScanPos > 0 ) {  
    xcPrev =x0 +  TraverseScanOrder[ log2CbWidth ][ log2CbHeight ][ PaletteScanPos - 1 ][ 0 ]  
    ycPrev = y0 +  TraverseScanOrder[ log2CbWidth ][ log2CbHeight ][ PaletteScanPos - 1 ][ 1 ]  
   }  
   if( MaxPaletteIndex > 0  &&  !RunCopyMap[ xC ][ yC ]  && CopyAboveIndicesFlag[ xC ][ yC ]  ==  0 ) { 
 
    if( MaxPaletteIndex - adjust > 0 )  
     palette_idx_idc ae(v) 
    adjust = 1  
   }  
   if( !RunCopyMap[ xC ][ yC ]  &&  CopyAboveIndicesFlag[ xC ][ yC ]  ==  0 )  
    CurrPaletteIndex = palette_idx_idc  
   if( CopyAboveIndicesFlag[ xC ][ yC ]  ==  0 )  
    PaletteIndexMap[ xC ][ yC ] = CurrPaletteIndex  
   else if( !palette_transpose_flag )  
       PaletteIndexMap[ xC ][ yC ] = PaletteIndexMap[ xC ][ yC - 1 ]  
   else  
    PaletteIndexMap[ xC ][ yC ] = PaletteIndexMap[ xC - 1 ][ yC ]  
   PaletteScanPos++  
  }  
  if( palette_escape_val_present_flag ) {  
   for( cIdx = startComp; cIdx < ( startComp + numComps ); cIdx++ ) {  
    for( sPos = minSubPos; sPos < maxSubPos; sPos++ ) {  
     xC = x0 + TraverseScanOrder[ log2CbWidth ][ log2CbHeight ][ sPos ][ 0 ]  
     yC = y0 + TraverseScanOrder[ log2CbWidth ][ log2CbHeight ][ sPos ][ 1 ]  
     if( !( treeType == SINGLE_TREE  &&  cIdx  !=  0  &&  ( xC % SubWidthC  !=  0  ||  yC % SubHeightC  !=  0 ) ) ) { 
      if( PaletteIndexMap[ cIdx ][ xC ][ yC ]  ==  MaxPaletteIndex ) {  
       palette_escape_val ae(v) 
       PaletteEscapeVal[ cIdx ][ xC ][ yC ] = palette_escape_val  
      }  
     }  
    }  
   }  
  }  
 }  
}  

merge_data( x0, y0, cbWidth, cbHeight, chType ) {  
 if( CuPredMode[ chType ][ x0 ][ y0 ]  ==  MODE_IBC ) {  
  if( MaxNumIbcMergeCand > 1 )  
   merge_idx[ x0 ][ y0 ] ae(v) 
 } else {  
  if( MaxNumSubblockMergeCand > 0  &&  cbWidth  >=  8  &&  cbHeight  >=  8 )  
   merge_subblock_flag[ x0 ][ y0 ] ae(v) 
  if( merge_subblock_flag[ x0 ][ y0 ]  ==  1 ) {  
   if( MaxNumSubblockMergeCand > 1 )  
    merge_subblock_idx[ x0 ][ y0 ] ae(v) 
  } else {  
   if( cbWidth < 128  &&  cbHeight < 128  && 
     ( ( sps_ciip_enabled_flag  &&  cu_skip_flag[ x0 ][ y0 ]  ==  0  && 
      ( cbWidth * cbHeight )  >=  64 )  || 
     ( sps_gpm_enabled_flag  && 
     sh_slice_type  ==  B  &&  cbWidth  >=  8  &&  cbHeight  >=  8  && 
     cbWidth < ( 8 * cbHeight )  &&  cbHeight < ( 8 * cbWidth ) ) ) ) 
 
    regular_merge_flag[ x0 ][ y0 ] ae(v) 
   if( regular_merge_flag[ x0 ][ y0 ]  ==  1 ) {  
    if( sps_mmvd_enabled_flag )  
     mmvd_merge_flag[ x0 ][ y0 ] ae(v) 
    if( mmvd_merge_flag[ x0 ][ y0 ]  ==  1 ) {  
     if( MaxNumMergeCand > 1 )  
      mmvd_cand_flag[ x0 ][ y0 ] ae(v)
           mmvd_distance_idx[ x0 ][ y0 ] ae(v) 
     mmvd_direction_idx[ x0 ][ y0 ] ae(v) 
    } else if( MaxNumMergeCand > 1 )  
     merge_idx[ x0 ][ y0 ] ae(v) 
   } else {  
    if( sps_ciip_enabled_flag  &&  sps_gpm_enabled_flag  && 
     sh_slice_type  ==  B  && 
     cu_skip_flag[ x0 ][ y0 ]  ==  0  &&  cbWidth  >=  8  &&  cbHeight  >=  8  && 
     cbWidth < ( 8 * cbHeight )  &&  cbHeight < ( 8 * cbWidth )  && 
     cbWidth < 128  &&  cbHeight < 128 ) 
 
     ciip_flag[ x0 ][ y0 ] ae(v) 
    if( ciip_flag[ x0 ][ y0 ]  &&  MaxNumMergeCand > 1 )  
     merge_idx[ x0 ][ y0 ] ae(v) 
    if( !ciip_flag[ x0 ][ y0 ] ) {  
     merge_gpm_partition_idx[ x0 ][ y0 ] ae(v) 
     merge_gpm_idx0[ x0 ][ y0 ] ae(v) 
     if( MaxNumGpmMergeCand > 2 )  
      merge_gpm_idx1[ x0 ][ y0 ] ae(v) 
    }  
   }  
  }  
 }  
}  

mvd_coding( x0, y0, refList, cpIdx ) {  
 abs_mvd_greater0_flag[ 0 ] ae(v) 
 abs_mvd_greater0_flag[ 1 ] ae(v) 
 if( abs_mvd_greater0_flag[ 0 ] )  
  abs_mvd_greater1_flag[ 0 ] ae(v) 
 if( abs_mvd_greater0_flag[ 1 ] )  
  abs_mvd_greater1_flag[ 1 ] ae(v) 
 if( abs_mvd_greater0_flag[ 0 ] ) {  
  if( abs_mvd_greater1_flag[ 0 ] )  
   abs_mvd_minus2[ 0 ] ae(v) 
  mvd_sign_flag[ 0 ] ae(v) 
 }  
 if( abs_mvd_greater0_flag[ 1 ] ) {  
  if( abs_mvd_greater1_flag[ 1 ] )  
   abs_mvd_minus2[ 1 ] ae(v) 
  mvd_sign_flag[ 1 ] ae(v) 
 }  
}  

transform_tree( x0, y0, tbWidth, tbHeight, treeType, chType ) {  
 InferTuCbfLuma = 1  
 if( IntraSubPartitionsSplitType  ==  ISP_NO_SPLIT  &&  !cu_sbt_flag ) {  
  if( tbWidth > MaxTbSizeY  ||  tbHeight > MaxTbSizeY ) {  
   verSplitFirst = ( tbWidth > MaxTbSizeY && tbWidth > tbHeight ) ? 1 : 0  
   trafoWidth = verSplitFirst ? ( tbWidth / 2 ) : tbWidth  
   trafoHeight = !verSplitFirst ? ( tbHeight / 2 ) : tbHeight  
   transform_tree( x0, y0, trafoWidth, trafoHeight, treeType, chType )  
   if( verSplitFirst )  
    transform_tree( x0 + trafoWidth, y0, trafoWidth, trafoHeight, treeType, chType )  
   else  
    transform_tree( x0, y0 + trafoHeight, trafoWidth, trafoHeight, treeType, chType )  
  } else {  
   transform_unit( x0, y0, tbWidth, tbHeight, treeType, 0, chType )  
  }  
 } else if( cu_sbt_flag ) {  
  if( !cu_sbt_horizontal_flag ) {  
   trafoWidth = tbWidth * SbtNumFourthsTb0 / 4  
   transform_unit( x0, y0, trafoWidth, tbHeight, treeType, 0, 0 )  
   transform_unit( x0 + trafoWidth, y0, tbWidth - trafoWidth, tbHeight, treeType, 1, 0 )  
  } else {  
   trafoHeight = tbHeight * SbtNumFourthsTb0 / 4  
   transform_unit( x0, y0, tbWidth, trafoHeight, treeType, 0, 0 )  
   transform_unit( x0, y0 + trafoHeight, tbWidth, tbHeight - trafoHeight, treeType, 1, 0 )  
  }  
  } else if( IntraSubPartitionsSplitType  ==  ISP_HOR_SPLIT ) {  
  trafoHeight = tbHeight / NumIntraSubPartitions  
  for( partIdx = 0; partIdx < NumIntraSubPartitions; partIdx++ )  
   transform_unit( x0, y0 + trafoHeight * partIdx, tbWidth, trafoHeight, treeType, partIdx, 0 )  
 } else if( IntraSubPartitionsSplitType  ==  ISP_VER_SPLIT ) {  
  trafoWidth = tbWidth / NumIntraSubPartitions  
  for( partIdx = 0; partIdx < NumIntraSubPartitions; partIdx++ )  
   transform_unit( x0 + trafoWidth * partIdx, y0, trafoWidth, tbHeight, treeType, partIdx, 0 )  
 }  
} 

transform_unit( x0, y0, tbWidth, tbHeight, treeType, subTuIndex, chType ) {  
 if( IntraSubPartitionsSplitType  !=  ISP_NO_SPLIT  && 
   treeType  ==  SINGLE_TREE  &&  subTuIndex  ==  NumIntraSubPartitions - 1 ) { 
 
  xC = CbPosX[ chType ][ x0 ][ y0 ]  
  yC = CbPosY[ chType ][ x0 ][ y0 ]  
  wC = CbWidth[ chType ][ x0 ][ y0 ] / SubWidthC  
  hC = CbHeight[ chType ][ x0 ][ y0 ] / SubHeightC  
 } else {  
   xC = x0  
  yC = y0  
  wC = tbWidth / SubWidthC  
  hC = tbHeight / SubHeightC  
 }  
 chromaAvailable = treeType  !=  DUAL_TREE_LUMA  &&  sps_chroma_format_idc  !=  0  && ( IntraSubPartitionsSplitType  ==  ISP_NO_SPLIT   ||  ( IntraSubPartitionsSplitType  !=  ISP_NO_SPLIT  &&  subTuIndex  ==  NumIntraSubPartitions - 1 ) ) 
 
 if( ( treeType  ==  SINGLE_TREE  ||  treeType  ==  DUAL_TREE_CHROMA )  && 
   sps_chroma_format_idc  !=  0  &&   
   ( ( IntraSubPartitionsSplitType  ==  ISP_NO_SPLIT  &&  !( cu_sbt_flag  && 
   ( ( subTuIndex  == 0  &&  cu_sbt_pos_flag )  || 
   ( subTuIndex  == 1  &&  !cu_sbt_pos_flag ) ) ) )  || 
   ( IntraSubPartitionsSplitType  !=  ISP_NO_SPLIT  && 
   ( subTuIndex  ==  NumIntraSubPartitions - 1 ) ) ) ) { 
 
  tu_cb_coded_flag[ xC ][ yC ] ae(v) 
  tu_cr_coded_flag[ xC ][ yC ] ae(v) 
 }  
 if( treeType  ==  SINGLE_TREE  ||  treeType  ==  DUAL_TREE_LUMA ) {  
  if( ( IntraSubPartitionsSplitType  ==  ISP_NO_SPLIT  &&  !( cu_sbt_flag  && 
    ( ( subTuIndex  == 0  &&  cu_sbt_pos_flag )  || 
    ( subTuIndex  == 1  &&  !cu_sbt_pos_flag ) ) )  && 
    ( ( CuPredMode[ chType ][ x0 ][ y0 ]  ==  MODE_INTRA  &&   
    !cu_act_enabled_flag[ x0 ][ y0 ] )  || 
    ( chromaAvailable  &&  ( tu_cb_coded_flag[ xC ][ yC ]  || 
    tu_cr_coded_flag[ xC ][ yC ] ) )  || 
    CbWidth[ chType ][ x0 ][ y0 ] > MaxTbSizeY  || 
    CbHeight[ chType ][ x0 ][ y0 ] > MaxTbSizeY ) )  || 
    ( IntraSubPartitionsSplitType  !=  ISP_NO_SPLIT && 
    ( subTuIndex < NumIntraSubPartitions - 1  ||  !InferTuCbfLuma ) ) ) 
 
   tu_y_coded_flag[ x0 ][ y0 ] ae(v) 
  if(IntraSubPartitionsSplitType  !=  ISP_NO_SPLIT )  
   InferTuCbfLuma  = InferTuCbfLuma  &&  !tu_y_coded_flag[ x0 ][ y0 ]  
 }  
 if( ( CbWidth[ chType ][ x0 ][ y0 ] > 64  ||  CbHeight[ chType ][ x0 ][ y0 ] > 64  || 
   tu_y_coded_flag[ x0 ][ y0 ]  ||  ( chromaAvailable  &&  ( tu_cb_coded_flag[ xC ][ yC ]  || 
   tu_cr_coded_flag[ xC ][ yC ] ) ) )  &&  treeType  !=  DUAL_TREE_CHROMA  && 
   pps_cu_qp_delta_enabled_flag  &&  !IsCuQpDeltaCoded ) { 
 
  cu_qp_delta_abs ae(v) 
  if( cu_qp_delta_abs )  
   cu_qp_delta_sign_flag ae(v) 
 }  
 if( ( CbWidth[ chType ][ x0 ][ y0 ] > 64  ||  CbHeight[ chType ][ x0 ][ y0 ] > 64  || 
   ( chromaAvailable  &&  ( tu_cb_coded_flag[ xC ][ yC ]  || 
   tu_cr_coded_flag[ xC ][ yC ] ) ) )  && 
   treeType  !=  DUAL_TREE_LUMA  &&  sh_cu_chroma_qp_offset_enabled_flag  && 
   !IsCuChromaQpOffsetCoded ) { 
 
  cu_chroma_qp_offset_flag ae(v) 
  if( cu_chroma_qp_offset_flag  &&  pps_chroma_qp_offset_list_len_minus1 > 0 )  
   cu_chroma_qp_offset_idx ae(v) 
 }  
 if( sps_joint_cbcr_enabled_flag  &&  ( ( CuPredMode[ chType ][ x0 ][ y0 ]  ==  MODE_INTRA 
   &&  ( tu_cb_coded_flag[ xC ][ yC ]  ||  tu_cr_coded_flag[ xC ][ yC ] ) )  || 
   ( tu_cb_coded_flag[ xC ][ yC ]  &&  tu_cr_coded_flag[ xC ][ yC ] ) )  && 
   chromaAvailable ) 
 
  tu_joint_cbcr_residual_flag[ xC ][ yC ] ae(v) 
   if( tu_y_coded_flag[ x0 ][ y0 ]  &&  treeType  !=  DUAL_TREE_CHROMA ) {  
  if( sps_transform_skip_enabled_flag  &&  !BdpcmFlag[ x0 ][ y0 ][ 0 ]  && 
    tbWidth  <=  MaxTsSize  &&  tbHeight  <=  MaxTsSize  && 
    ( IntraSubPartitionsSplitType  ==  ISP_NO_SPLIT )  &&  !cu_sbt_flag ) 
 
   transform_skip_flag[ x0 ][ y0 ][ 0 ] ae(v) 
  if( !transform_skip_flag[ x0 ][ y0 ][ 0 ]  ||  sh_ts_residual_coding_disabled_flag )  
   residual_coding( x0, y0, Log2( tbWidth ), Log2( tbHeight ), 0 )  
  else  
   residual_ts_coding( x0, y0, Log2( tbWidth ), Log2( tbHeight ), 0 )  
 }  
 if( tu_cb_coded_flag[ xC ][ yC ]  &&  treeType  !=  DUAL_TREE_LUMA ) {  
  if( sps_transform_skip_enabled_flag  &&  !BdpcmFlag[ x0 ][ y0 ][ 1 ]  && 
    wC  <=  MaxTsSize  &&  hC  <=  MaxTsSize  &&  !cu_sbt_flag ) 
 
   transform_skip_flag[ xC ][ yC ][ 1 ] ae(v) 
  if( !transform_skip_flag[ xC ][ yC ][ 1 ]  ||  sh_ts_residual_coding_disabled_flag )  
   residual_coding( xC, yC, Log2( wC ), Log2( hC ), 1 )  
  else  
   residual_ts_coding( xC, yC, Log2( wC ), Log2( hC ), 1 )  
 }  
 if( tu_cr_coded_flag[ xC ][ yC ]  &&  treeType  !=  DUAL_TREE_LUMA  && 
   !( tu_cb_coded_flag[ xC ][ yC ]  &&  tu_joint_cbcr_residual_flag[ xC ][ yC ] ) ) { 
 
  if( sps_transform_skip_enabled_flag  &&  !BdpcmFlag[ x0 ][ y0 ][ 2 ]  && 
    wC  <=  MaxTsSize  &&  hC  <=  MaxTsSize  &&  !cu_sbt_flag ) 
 
   transform_skip_flag[ xC ][ yC ][ 2 ] ae(v) 
  if( !transform_skip_flag[ xC ][ yC ][ 2 ]  ||  sh_ts_residual_coding_disabled_flag )  
   residual_coding( xC, yC, Log2( wC ), Log2( hC ), 2 )  
  else  
   residual_ts_coding( xC, yC, Log2( wC ), Log2( hC ), 2 )  
 }  
} 

residual_coding( x0, y0, log2TbWidth, log2TbHeight, cIdx ) {  
 if( sps_mts_enabled_flag  &&  cu_sbt_flag  &&  cIdx  ==  0  && 
   log2TbWidth  ==  5  &&  log2TbHeight < 6 ) 
 
  log2ZoTbWidth = 4  
 else  
  log2ZoTbWidth = Min( log2TbWidth, 5 )  
 if( sps_mts_enabled_flag  &&  cu_sbt_flag  &&  cIdx  ==  0  &&   
   log2TbWidth < 6  &&  log2TbHeight  ==  5 ) 
 
  log2ZoTbHeight = 4  
 else  
  log2ZoTbHeight = Min( log2TbHeight, 5 )  
 if( log2TbWidth > 0 )  
  last_sig_coeff_x_prefix ae(v) 
 if( log2TbHeight > 0 )  
  last_sig_coeff_y_prefix ae(v) 
 if( last_sig_coeff_x_prefix > 3 )  
  last_sig_coeff_x_suffix ae(v) 
 if( last_sig_coeff_y_prefix > 3 )  
   last_sig_coeff_y_suffix ae(v) 
 log2TbWidth = log2ZoTbWidth  
 log2TbHeight = log2ZoTbHeight  
 remBinsPass1 = ( ( 1  <<  ( log2TbWidth + log2TbHeight ) ) * 7 )  >>  2  
 log2SbW = ( Min( log2TbWidth, log2TbHeight ) < 2 ? 1 : 2 )  
 log2SbH = log2SbW  
 if( log2TbWidth + log2TbHeight > 3 )  
  if( log2TbWidth < 2 ) {  
   log2SbW = log2TbWidth  
   log2SbH = 4 - log2SbW  
  } else if( log2TbHeight < 2 ) {  
   log2SbH = log2TbHeight  
   log2SbW = 4 - log2SbH  
  }  
 numSbCoeff = 1  <<  ( log2SbW + log2SbH )  
 lastScanPos = numSbCoeff  
 lastSubBlock = ( 1  <<  ( log2TbWidth + log2TbHeight - ( log2SbW + log2SbH ) ) ) - 1  
 do {  
  if( lastScanPos  ==  0 ) {  
   lastScanPos = numSbCoeff  
   lastSubBlock--  
  }  
  lastScanPos--  
  xS = DiagScanOrder[ log2TbWidth - log2SbW ][ log2TbHeight - log2SbH ][ lastSubBlock ][ 0 ] 
  yS = DiagScanOrder[ log2TbWidth - log2SbW ][ log2TbHeight - log2SbH ][ lastSubBlock ][ 1 ] 
  xC = ( xS  <<  log2SbW ) + DiagScanOrder[ log2SbW ][ log2SbH ][ lastScanPos ][ 0 ]  
  yC = ( yS  <<  log2SbH ) + DiagScanOrder[ log2SbW ][ log2SbH ][ lastScanPos ][ 1 ]  
 } while( ( xC  !=  LastSignificantCoeffX )  ||  ( yC  !=  LastSignificantCoeffY ) )  
 if( lastSubBlock  ==  0  &&  log2TbWidth  >=  2  &&  log2TbHeight  >=  2  && !transform_skip_flag[ x0 ][ y0 ][ cIdx ]  &&  lastScanPos > 0 )  
  LfnstDcOnly = 0   
 if( ( lastSubBlock > 0  &&  log2TbWidth  >=  2  &&  log2TbHeight  >=  2 )  || ( lastScanPos > 7  &&  ( log2TbWidth  ==  2  ||  log2TbWidth  ==  3 )  && log2TbWidth  ==  log2TbHeight ) )  
  LfnstZeroOutSigCoeffFlag = 0  
 if( ( lastSubBlock > 0  ||  lastScanPos > 0 )  &&  cIdx == 0 )  
  MtsDcOnly = 0  
 QState = 0  
 for( i = lastSubBlock; i  >=  0; i-- ) {  
  startQStateSb = QState  
  xS = DiagScanOrder[ log2TbWidth - log2SbW ][ log2TbHeight - log2SbH ][ i ][ 0 ] 
  yS = DiagScanOrder[ log2TbWidth - log2SbW ][ log2TbHeight - log2SbH ][ i ][ 1 ] 
  inferSbDcSigCoeffFlag = 0  
  if( i < lastSubBlock  &&  i > 0 ) {  
   sb_coded_flag[ xS ][ yS ] ae(v) 
   inferSbDcSigCoeffFlag = 1  
  }  
  if( sb_coded_flag[ xS ][ yS ]  &&  ( xS > 3  ||  yS > 3 )  &&  cIdx  ==  0 )  
   MtsZeroOutSigCoeffFlag = 0  
  firstSigScanPosSb = numSbCoeff  
  lastSigScanPosSb = -1  
  firstPosMode0 = ( i  ==  lastSubBlock ? lastScanPos : numSbCoeff - 1 )  
  firstPosMode1 = firstPosMode0  
  for( n = firstPosMode0; n  >=  0  &&  remBinsPass1  >=  4; n-- ) {  
   xC = ( xS  <<  log2SbW ) + DiagScanOrder[ log2SbW ][ log2SbH ][ n ][ 0 ]   
   yC = ( yS  <<  log2SbH ) + DiagScanOrder[ log2SbW ][ log2SbH ][ n ][ 1 ]  
   if( sb_coded_flag[ xS ][ yS ]  &&  ( n > 0  ||  !inferSbDcSigCoeffFlag )  && 
     ( xC  !=  LastSignificantCoeffX  ||  yC  !=  LastSignificantCoeffY ) ) { 
 
    sig_coeff_flag[ xC ][ yC ] ae(v) 
    remBinsPass1--  
    if( sig_coeff_flag[ xC ][ yC ] )  
     inferSbDcSigCoeffFlag = 0  
   }  
   if( sig_coeff_flag[ xC ][ yC ] ) {  
    abs_level_gtx_flag[ n ][ 0 ] ae(v) 
    remBinsPass1--  
    if( abs_level_gtx_flag[ n ][ 0 ] ) {  
     par_level_flag[ n ] ae(v) 
     remBinsPass1--  
     abs_level_gtx_flag[ n ][ 1 ] ae(v) 
     remBinsPass1--  
    }  
    if( lastSigScanPosSb  ==  -1 )  
     lastSigScanPosSb = n  
    firstSigScanPosSb = n  
   }  
   AbsLevelPass1[ xC ][ yC ] = sig_coeff_flag[ xC ][ yC ] + par_level_flag[ n ] + abs_level_gtx_flag[ n ][ 0 ] + 2 * abs_level_gtx_flag[ n ][ 1 ] 
 
   if( sh_dep_quant_used_flag )  
    QState = QStateTransTable[ QState ][ AbsLevelPass1[ xC ][ yC ] & 1 ]  
   firstPosMode1 = n - 1  
  }  
  for( n = firstPosMode0; n > firstPosMode1; n-- ) {  
   xC = ( xS  <<  log2SbW ) + DiagScanOrder[ log2SbW ][ log2SbH ][ n ][ 0 ]  
   yC = ( yS  <<  log2SbH ) + DiagScanOrder[ log2SbW ][ log2SbH ][ n ][ 1 ]  
   if( abs_level_gtx_flag[ n ][ 1 ] )  
    abs_remainder[ n ] ae(v) 
   AbsLevel[ xC ][ yC ] = AbsLevelPass1[ xC ][ yC ] +2 * abs_remainder[ n ]  
  }  
  for( n = firstPosMode1; n  >=  0; n-- ) {  
   xC = ( xS  <<  log2SbW ) + DiagScanOrder[ log2SbW ][ log2SbH ][ n ][ 0 ]  
   yC = ( yS  <<  log2SbH ) + DiagScanOrder[ log2SbW ][ log2SbH ][ n ][ 1 ]  
   if( sb_coded_flag[ xS ][ yS ] )  
    dec_abs_level[ n ] ae(v) 
   if( AbsLevel[ xC ][ yC ] > 0 ) {  
       if( lastSigScanPosSb  ==  -1 )  
     lastSigScanPosSb = n  
    firstSigScanPosSb = n  
   }  
   if( sh_dep_quant_used_flag )  
    QState = QStateTransTable[ QState ][ AbsLevel[ xC ][ yC ] & 1 ]  
  }  
  signHiddenFlag = sh_sign_data_hiding_used_flag  && ( lastSigScanPosSb - firstSigScanPosSb > 3 ? 1 : 0 ) 
 
  for( n = numSbCoeff - 1; n  >=  0; n-- ) {  
   xC = ( xS  <<  log2SbW ) + DiagScanOrder[ log2SbW ][ log2SbH ][ n ][ 0 ]   
   yC = ( yS  <<  log2SbH ) + DiagScanOrder[ log2SbW ][ log2SbH ][ n ][ 1 ]  
   if( ( AbsLevel[ xC ][ yC ] > 0 )  && ( !signHiddenFlag  ||  ( n  !=  firstSigScanPosSb ) ) ) 
    coeff_sign_flag[ n ] ae(v) 
  }  
  if( sh_dep_quant_used_flag ) {  
   QState = startQStateSb  
   for( n = numSbCoeff - 1; n  >=  0; n-- ) {  
    xC = ( xS  <<  log2SbW ) + DiagScanOrder[ log2SbW ][ log2SbH ][ n ][ 0 ]  
    yC = ( yS  <<  log2SbH ) + DiagScanOrder[ log2SbW ][ log2SbH ][ n ][ 1 ]  
    if( AbsLevel[ xC ][ yC ] > 0 )  
     TransCoeffLevel[ x0 ][ y0 ][ cIdx ][ xC ][ yC ] = ( 2 * AbsLevel[ xC ][ yC ] - ( QState > 1 ? 1 : 0 ) ) * ( 1 - 2 * coeff_sign_flag[ n ] ) 
    QState = QStateTransTable[ QState ][ AbsLevel[ xC ][ yC ] & 1 ]  
   }  
  } else {  
   sumAbsLevel = 0  
   for( n = numSbCoeff - 1; n  >=  0; n-- ) {  
    xC = ( xS  <<  log2SbW ) + DiagScanOrder[ log2SbW ][ log2SbH ][ n ][ 0 ]  
    yC = ( yS  <<  log2SbH ) + DiagScanOrder[ log2SbW ][ log2SbH ][ n ][ 1 ]  
    if( AbsLevel[ xC ][ yC ] > 0 ) {  
     TransCoeffLevel[ x0 ][ y0 ][ cIdx ][ xC ][ yC ]  = 
       AbsLevel[ xC ][ yC ] * ( 1 - 2 * coeff_sign_flag[ n ] )  
     if( signHiddenFlag ) {  
      sumAbsLevel  +=  AbsLevel[ xC ][ yC ]  
      if( n  ==  firstSigScanPosSb  &&  sumAbsLevel % 2  ==  1 )  
       TransCoeffLevel[ x0 ][ y0 ][ cIdx ][ xC ][ yC ]  = -TransCoeffLevel[ x0 ][ y0 ][ cIdx ][ xC ][ yC ]  
     }  
    }  
   }  
  }  
 }  
}  

residual_ts_coding( x0, y0, log2TbWidth, log2TbHeight, cIdx ) {  
 log2SbW = ( Min( log2TbWidth, log2TbHeight ) < 2 ? 1 : 2 ) 
  log2SbH = log2SbW  
 if( log2TbWidth + log2TbHeight > 3 )  
  if( log2TbWidth < 2 ) {  
   log2SbW = log2TbWidth  
   log2SbH = 4 - log2SbW  
  } else if( log2TbHeight < 2 ) {  
   log2SbH = log2TbHeight  
   log2SbW = 4 - log2SbH  
  }  
 numSbCoeff = 1  <<  ( log2SbW + log2SbH )  
 lastSubBlock = ( 1  <<  ( log2TbWidth + log2TbHeight - ( log2SbW + log2SbH ) ) ) - 1  
 inferSbCbf = 1  
 RemCcbs = ( ( 1  <<  ( log2TbWidth + log2TbHeight ) ) * 7 )  >>  2  
 for( i =0; i  <=  lastSubBlock; i++ ) {  
  xS = DiagScanOrder[ log2TbWidth - log2SbW ][ log2TbHeight - log2SbH ][ i ][ 0 ]  
  yS = DiagScanOrder[ log2TbWidth - log2SbW ][ log2TbHeight - log2SbH ][ i ][ 1 ]  
  if( i  !=  lastSubBlock  ||  !inferSbCbf )  
   sb_coded_flag[ xS ][ yS ] ae(v) 
  if( sb_coded_flag[ xS ][ yS ]  &&  i < lastSubBlock )  
   inferSbCbf = 0  
 /* First scan pass */  
  inferSbSigCoeffFlag = 1  
  lastScanPosPass1 = -1  
  for( n = 0; n  <=  numSbCoeff - 1  &&  RemCcbs  >=  4; n++ ) {  
   xC = ( xS  <<  log2SbW ) + DiagScanOrder[ log2SbW ][ log2SbH ][ n ][ 0 ]  
   yC = ( yS  <<  log2SbH ) + DiagScanOrder[ log2SbW ][ log2SbH ][ n ][ 1 ]  
   lastScanPosPass1 = n  
   if( sb_coded_flag[ xS ][ yS ]  && ( n  !=  numSbCoeff - 1  ||  !inferSbSigCoeffFlag ) ) { 
 
    sig_coeff_flag[ xC ][ yC ] ae(v) 
    RemCcbs--  
    if( sig_coeff_flag[ xC ][ yC ] )  
     inferSbSigCoeffFlag = 0  
   }  
   CoeffSignLevel[ xC ][ yC ] = 0  
   if( sig_coeff_flag[ xC ][ yC ] ) {  
    coeff_sign_flag[ n ] ae(v) 
    RemCcbs--  
    CoeffSignLevel[ xC ][ yC ] = ( coeff_sign_flag[ n ] > 0 ? -1 : 1 )  
    abs_level_gtx_flag[ n ][ 0 ] ae(v) 
    RemCcbs--  
    if( abs_level_gtx_flag[ n ][ 0 ] ) {  
     par_level_flag[ n ] ae(v) 
     RemCcbs--  
    }  
   }  
   AbsLevelPass1[ xC ][ yC ] = 
     sig_coeff_flag[ xC ][ yC ] + par_level_flag[ n ] + abs_level_gtx_flag[ n ][ 0 ] 
 
  }  
   /* Greater than X scan pass (numGtXFlags=5) */  
  lastScanPosPass2 = -1  
  for( n = 0; n  <=  numSbCoeff - 1  &&  RemCcbs  >=  4; n++ ) {  
   xC = ( xS  <<  log2SbW ) + DiagScanOrder[ log2SbW ][ log2SbH ][ n ][ 0 ]  
   yC = ( yS  <<  log2SbH ) + DiagScanOrder[ log2SbW ][ log2SbH ][ n ][ 1 ]  
   AbsLevelPass2[ xC ][ yC ] = AbsLevelPass1[ xC ][ yC ]  
   for( j = 1; j < 5; j++ ) {  
    if( abs_level_gtx_flag[ n ][ j - 1 ] ) {  
     abs_level_gtx_flag[ n ][ j ] ae(v) 
     RemCcbs--  
    }  
    AbsLevelPass2[ xC ][ yC ]  +=  2 * abs_level_gtx_flag[ n ][ j ]  
   }  
   lastScanPosPass2 = n  
  }  
 /* remainder scan pass */  
  for( n = 0; n  <=  numSbCoeff - 1; n++ ) {  
   xC = ( xS  <<  log2SbW ) + DiagScanOrder[ log2SbW ][ log2SbH ][ n ][ 0 ]  
   yC = ( yS  <<  log2SbH ) + DiagScanOrder[ log2SbW ][ log2SbH ][ n ][ 1 ]  
   if( ( n  <=  lastScanPosPass2  &&  AbsLevelPass2[ xC ][ yC ]  >=  10 )  || 
     ( n > lastScanPosPass2  &&  n  <=  lastScanPosPass1  && 
     AbsLevelPass1[ xC ][ yC ]  >=  2 )  || 
     ( n > lastScanPosPass1  &&  sb_coded_flag[ xS ][ yS ] ) ) 
 
    abs_remainder[ n ] ae(v) 
   if( n  <=  lastScanPosPass2 )  
    AbsLevel[ xC ][ yC ] = AbsLevelPass2[ xC ][ yC ] + 2 * abs_remainder[ n ]  
   else if(n  <=  lastScanPosPass1 )  
    AbsLevel[ xC ][ yC ] = AbsLevelPass1[ xC ][ yC ] + 2 * abs_remainder[ n ]  
   else { /* bypass */  
    AbsLevel[ xC ][ yC ] = abs_remainder[ n ]  
    if( abs_remainder[ n ] )  
     coeff_sign_flag[ n ] ae(v) 
   }  
   if( BdpcmFlag[ x0 ][ y0 ][ cIdx ]  ==  0  &&  n  <=  lastScanPosPass1 ) {  
    absLeftCoeff = xC > 0 ? AbsLevel[ xC - 1 ][ yC ] : 0  
    absAboveCoeff  = yC > 0 ? AbsLevel[ xC ][ yC - 1 ] : 0  
    predCoeff = Max( absLeftCoeff, absAboveCoeff )  
    if( AbsLevel[ xC ][ yC ]  ==  1  &&  predCoeff > 0 )  
     AbsLevel[ xC ][ yC ] = predCoeff  
    else if( AbsLevel[ xC ][ yC ] > 0  && AbsLevel[ xC ][ yC ]  <=  predCoeff )  
     AbsLevel[ xC ][ yC ]--  
   }  
   TransCoeffLevel[ x0 ][ y0 ][ cIdx ][ xC ][ yC ] = ( 1 - 2 * coeff_sign_flag[ n ] ) * AbsLevel[ xC ][ yC ] 
 
  }  
 }  
} 

byte_stream_nal_unit( NumBytesInNalUnit ) { 
while( next_bits( 24 )  !=  0x000001  &&  next_bits( 32 )  !=  0x00000001 )   
leading_zero_8bits  /* equal to 0x00 */ f(8) 
if( next_bits( 24 )  !=  0x000001 )  f(8) 
zero_byte  /* equal to 0x00 */ 
start_code_prefix_one_3bytes  /* equal to 0x000001 */ f(24) 
nal_unit( NumBytesInNalUnit )  
while( more_data_in_byte_stream()  &&  next_bits( 24 )  !=  0x000001  && 
next_bits( 32 )  !=  0x00000001 ) 
trailing_zero_8bits  /* equal to 0x00 */ f(8)
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
  if( payloadType  ==  132 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 */  
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
