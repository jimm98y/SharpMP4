frame( sz ) { 
 startBitPos = get_position( )  
 uncompressed_header( )  
 trailing_bits( )  
 if ( header_size_in_bytes == 0 )  {  
  while ( get_position( ) < startBitPos + 8 * sz)  
   padding_bit f(1) 
  return  
 }  
 load_probs( frame_context_idx )  
 load_probs2( frame_context_idx )  
 clear_counts( )  
 init_bool( header_size_in_bytes )  
 compressed_header( )  
 exit_bool( )  
 endBitPos = get_position( )  
 headerBytes = (endBitPos - startBitPos) / 8  
 decode_tiles( sz - headerBytes )  
 refresh_probs( )  
}

trailing_bits() {
    while (get_position() & 7)  
  zero_bit f(1) 
}

uncompressed_header() {
 frame_marker f(2) 
 profile_low_bit f(1) 
 profile_high_bit f(1)
    Profile = (profile_high_bit << 1) + profile_low_bit
    if (Profile == 3)  
  reserved_zero f(1) 
 show_existing_frame f(1)
    if (show_existing_frame == 1) {  
  frame_to_show_map_idx f(3)
        header_size_in_bytes = 0
        refresh_frame_flags = 0
        loop_filter_level = 0
        return
    }
    LastFrameType = frame_type  
 frame_type f(1) 
 show_frame f(1) 
 error_resilient_mode f(1)
    if (frame_type == KEY_FRAME) {
        frame_sync_code()
        color_config()
        frame_size()
        render_size()
        refresh_frame_flags = 0xFF
        FrameIsIntra = 1
    } else {
        if (show_frame == 0) {  
   intra_only f(1)
        } else {
            intra_only = 0
        }
        FrameIsIntra = intra_only
        if (error_resilient_mode == 0) {  
   reset_frame_context f(2)
        } else {
            reset_frame_context = 0
        }
        if (intra_only == 1) {
            frame_sync_code()
            if (Profile > 0) {
                color_config()
            } else {
                color_space = CS_BT_601
                subsampling_x = 1
                subsampling_y = 1
                BitDepth = 8
            }  
   refresh_frame_flags f(8)
            frame_size()
            render_size()
        } else {  
   refresh_frame_flags f(8)
            for (i = 0; i < 3; i++) {
                ref_frame_idx[i] f(3)
                ref_frame_sign_bias[LAST_FRAME + i] f(1)
            }
            frame_size_with_refs()  
   allow_high_precision_mv f(1)
            read_interpolation_filter()
        }
    }
    if (error_resilient_mode == 0) {  
  refresh_frame_context f(1) 
  frame_parallel_decoding_mode f(1)
    } else {
        refresh_frame_context = 0
        frame_parallel_decoding_mode = 1
    }  
 frame_context_idx f(2)
    if (FrameIsIntra || error_resilient_mode) {
        setup_past_independence()
        if (frame_type == KEY_FRAME || error_resilient_mode == 1
            || reset_frame_context == 3) {

            for (i = 0; i < 4; i++) {
                save_probs(i)
            }
        } else if (reset_frame_context == 2) {
            save_probs(frame_context_idx)
        }
        frame_context_idx = 0
    }
    loop_filter_params()
    quantization_params()
    segmentation_params()
    tile_info()
 header_size_in_bytes f(16)
}

frame_sync_code() {
 frame_sync_byte_0 f(8) 
 frame_sync_byte_1 f(8) 
 frame_sync_byte_2 f(8) 
}

color_config() {
    if (Profile >= 2) {  
  ten_or_twelve_bit f(1)
        BitDepth = ten_or_twelve_bit ? 12 : 10
    } else {
        BitDepth = 8
    }  
 color_space f(3)
    if (color_space != CS_RGB) {  
  color_range f(1)
        if (Profile == 1 || Profile == 3) {  
   subsampling_x f(1) 
   subsampling_y f(1) 
   reserved_zero f(1)
        } else {
            subsampling_x = 1
            subsampling_y = 1
        }
    } else {
        color_range = 1
        if (Profile == 1 || Profile == 3) {
            subsampling_x = 0
            subsampling_y = 0  
   reserved_zero f(1)
        }  
 }  
}

frame_size() { 
 frame_width_minus_1  f(16) 
 frame_height_minus_1  f(16)
    FrameWidth = frame_width_minus_1 + 1
    FrameHeight = frame_height_minus_1 + 1 
 compute_image_size( )  
}

render_size() {
 render_and_frame_size_different f(1)
    if (render_and_frame_size_different == 1) {  
  render_width_minus_1 f(16) 
  render_height_minus_1 f(16)
        renderWidth = render_width_minus_1 + 1
        renderHeight = render_height_minus_1 + 1
    } else {
        renderWidth = FrameWidth
        renderHeight = FrameHeight  
 }  
}

frame_size_with_refs() {
    for (i = 0; i < 3; i++) {  
  found_ref f(1)
        if (found_ref == 1) {
            FrameWidth = RefFrameWidth[ref_frame_idx[i]]
            FrameHeight = RefFrameHeight[ref_frame_idx[i]]
            break
        }
    }
    if (found_ref == 0)
        frame_size()
    else
        compute_image_size()  
 render_size( )  
}

compute_image_size() {
    MiCols = (FrameWidth + 7) >> 3
    MiRows = (FrameHeight + 7) >> 3
    Sb64Cols = (MiCols + 7) >> 3  
 Sb64Rows = (MiRows + 7) >> 3  
}

read_interpolation_filter() { 
 is_filter_switchable f(1)
    if (is_filter_switchable == 1) {
        interpolation_filter = SWITCHABLE
    } else {  
  raw_interpolation_filter f(2)
        interpolation_filter = literal_to_type[raw_interpolation_filter]  
 }  
}

loop_filter_params() {
 loop_filter_level f(6) 
 loop_filter_sharpness f(3) 
 loop_filter_delta_enabled f(1)
    if (loop_filter_delta_enabled == 1) {  
  loop_filter_delta_update f(1)
        if (loop_filter_delta_update == 1) {
            for (i = 0; i < 4; i++) {  
    update_ref_delta f(1)
                if (update_ref_delta == 1)
                    loop_filter_ref_deltas[i] s(6)
            }
            for (i = 0; i < 2; i++) {  
    update_mode_delta f(1)
                if (update_mode_delta == 1)
                    loop_filter_mode_deltas[i] s(6)
            }
        }
 }
}

quantization_params() {
 base_q_idx f(8)
    delta_q_y_dc = read_delta_q()
    delta_q_uv_dc = read_delta_q()
    delta_q_uv_ac = read_delta_q()
    Lossless = base_q_idx == 0 && delta_q_y_dc == 0 && delta_q_uv_dc == 0 && delta_q_uv_ac == 0 
}

read_delta_q() {
 delta_coded f(1)
    if (delta_coded) {  
  delta_q s(4)
    } else {
        delta_q = 0
    }  
 return delta_q  
}

segmentation_params() {
 segmentation_enabled f(1)
    if (segmentation_enabled == 1) {  
  segmentation_update_map f(1)
        if (segmentation_update_map == 1) {
            for (i = 0; i < 7; i++)
                segmentation_tree_probs[i] = read_prob()  
   segmentation_temporal_update f(1)
            for (i = 0; i < 3; i++)
                segmentation_pred_prob[i] = segmentation_temporal_update ?
                    read_prob() : 255


        }  
  segmentation_update_data f(1)
        if (segmentation_update_data == 1) {  
   segmentation_abs_or_delta_update f(1)
            for (i = 0; i < MAX_SEGMENTS; i++) {
                for (j = 0; j < SEG_LVL_MAX; j++) {
                    feature_value = 0  
     feature_enabled f(1)
                    FeatureEnabled[i][j] = feature_enabled
                    if (feature_enabled == 1) {
                        bits_to_read = segmentation_feature_bits[j]  
      feature_value f(bits_to_read)
                        if (segmentation_feature_signed[j] == 1) {  
       feature_sign f(1)
                            if (feature_sign == 1)
                                feature_value *= -1
                        }
                    }
                    FeatureData[i][j] = feature_value
                }
            }
        }

 }
}

read_prob() {
 prob_coded f(1)
    if (prob_coded) {  
  prob f(8)
    } else {
        prob = 255
    }  
 return prob  
}

tile_info() {
    minLog2TileCols = calc_min_log2_tile_cols()
    maxLog2TileCols = calc_max_log2_tile_cols()
    tile_cols_log2 = minLog2TileCols
    while (tile_cols_log2 < maxLog2TileCols) {  
  increment_tile_cols_log2 f(1)
        if (increment_tile_cols_log2 == 1)
            tile_cols_log2++
        else
            break
    }  
 tile_rows_log2 f(1)
    if (tile_rows_log2 == 1) {  
  increment_tile_rows_log2 f(1)
        tile_rows_log2 += increment_tile_rows_log2  
 }  
}

calc_min_log2_tile_cols() {
    minLog2 = 0
    while ((MAX_TILE_WIDTH_B64 << minLog2) < Sb64Cols)
        minLog2++
 return minLog2  
}

calc_max_log2_tile_cols() {
    maxLog2 = 1
    while ((Sb64Cols >> maxLog2) >= MIN_TILE_WIDTH_B64)
        maxLog2++  
 return maxLog2 - 1  
}

compressed_header() {
    read_tx_mode()
    if (tx_mode == TX_MODE_SELECT) {
        tx_mode_probs()
    }
    read_coef_probs()
    read_skip_prob()
    if (FrameIsIntra == 0) {
        read_inter_mode_probs()
        if (interpolation_filter == SWITCHABLE)
            read_interp_filter_probs()
        read_is_inter_probs()
        frame_reference_mode()
        frame_reference_mode_probs()
        read_y_mode_probs()
        read_partition_probs()
        mv_probs()  
 }  
}

read_tx_mode() {
    if (Lossless == 1) {
        tx_mode = ONLY_4X4
    } else {  
  tx_mode L(2)
        if (tx_mode == ALLOW_32X32) {  
   tx_mode_select L(1)
            tx_mode += tx_mode_select
        }  
 }  
}

tx_mode_probs() {
    for (i = 0; i < TX_SIZE_CONTEXTS; i++)
        for (j = 0; j < TX_SIZES - 3; j++)
            tx_probs_8x8[i][j] = diff_update_prob(tx_probs_8x8[i][j])
    for (i = 0; i < TX_SIZE_CONTEXTS; i++)
        for (j = 0; j < TX_SIZES - 2; j++)
            tx_probs_16x16[i][j] = diff_update_prob(tx_probs_16x16[i][j])
    for (i = 0; i < TX_SIZE_CONTEXTS; i++)
        for (j = 0; j < TX_SIZES - 1; j++)
   tx_probs_32x32[ i ][ j ] = diff_update_prob( tx_probs_32x32[ i ][ j ] )
}

diff_update_prob(prob) {
 update_prob B(252)
    if (update_prob == 1) {
        deltaProb = decode_term_subexp()
        prob = inv_remap_prob(deltaProb, prob)
    }  
 return prob  
}

decode_term_subexp() {
 bit L(1)
    if (bit == 0) {  
  sub_exp_val L(4)
        return sub_exp_val
    }  
 bit L(1)
    if (bit == 0) {  
  sub_exp_val_minus_16 L(4)
        return sub_exp_val_minus_16 + 16
    }  
 bit L(1)
    if (bit == 0) {  
  sub_exp_val_minus_32 L(5)
        return sub_exp_val_minus_32 + 32
    }  
 v L(7)
    if (v < 65)
        return v + 64  
 bit L(1)
return (v << 1) - 1 + bit  
}

inv_remap_prob(deltaProb, prob) {
    m = prob
    v = deltaProb
    v = inv_map_table[v]
    m--
    if ((m << 1) <= 255)
        m = 1 + inv_recenter_nonneg(v, m)
    else
        m = 255 - inv_recenter_nonneg(v, 255 - 1 - m)
return m  
}

inv_recenter_nonneg(v, m) {
    if (v > 2 * m)
        return v
    if (v & 1)
        return m - ((v + 1) >> 1)  
return m + (v >> 1)
}

read_coef_probs() {
    maxTxSize = tx_mode_to_biggest_tx_size[tx_mode]
    for (txSz = TX_4X4; txSz <= maxTxSize; txSz++) {  
  update_probs L(1)
        if (update_probs == 1)
            for (i = 0; i < 2; i++)
                for (j = 0; j < 2; j++)
                    for (k = 0; k < 6; k++) {
                        maxL = (k == 0) ? 3 : 6
                        for (l = 0; l < maxL; l++)
                            for (m = 0; m < 3; m++)
                                coef_probs[txSz][i][j][k][l][m] =
                                    diff_update_prob(coef_probs[txSz][i][j][k][l][m])

                    }  
 }  
}

read_skip_prob() {
    for (i = 0; i < SKIP_CONTEXTS; i++)  
  skip_prob[ i ]  = diff_update_prob( skip_prob[ i ] )  
}

read_inter_mode_probs() {
    for (i = 0; i < INTER_MODE_CONTEXTS; i++)
        for (j = 0; j < INTER_MODES - 1; j++)  
   inter_mode_probs[ i ][ j ] = diff_update_prob( inter_mode_probs[ i ][ j ] )  
}

read_interp_filter_probs() {
    for (j = 0; j < INTERP_FILTER_CONTEXTS; j++)
        for (i = 0; i < SWITCHABLE_FILTERS - 1; i++)  
   interp_filter_probs[ j ][ i ] = diff_update_prob( interp_filter_probs[ j ][ i ] )  
}

read_is_inter_probs() {
    for (i = 0; i < IS_INTER_CONTEXTS; i++)
 is_inter_prob[ i ] = diff_update_prob( is_inter_prob[ i ] )  
}

frame_reference_mode() {
    compoundReferenceAllowed = 0
    for (i = 1; i < REFS_PER_FRAME; i++)
        if (ref_frame_sign_bias[i + 1] != ref_frame_sign_bias[1])
            compoundReferenceAllowed = 1
    if (compoundReferenceAllowed == 1) {  
  non_single_reference L(1)
        if (non_single_reference == 0) {
            reference_mode = SINGLE_REFERENCE
        } else {  
   reference_select L(1)
            if (reference_select == 0)
                reference_mode = COMPOUND_REFERENCE
            else
                reference_mode = REFERENCE_MODE_SELECT
            setup_compound_reference_mode()
        }
    } else {
        reference_mode = SINGLE_REFERENCE
 }
}

frame_reference_mode_probs() {
    if (reference_mode == REFERENCE_MODE_SELECT) {
        for (i = 0; i < COMP_MODE_CONTEXTS; i++)
            comp_mode_prob[i] = diff_update_prob(comp_mode_prob[i])
    }
    if (reference_mode != COMPOUND_REFERENCE) {
        for (i = 0; i < REF_CONTEXTS; i++) {
            single_ref_prob[i][0] = diff_update_prob(single_ref_prob[i][0])
            single_ref_prob[i][1] = diff_update_prob(single_ref_prob[i][1])
        }
    }
    if (reference_mode != SINGLE_REFERENCE) {
        for (i = 0; i < REF_CONTEXTS; i++)
            comp_ref_prob[i] = diff_update_prob(comp_ref_prob[i])  
 }  
}

read_y_mode_probs() {
    for (i = 0; i < BLOCK_SIZE_GROUPS; i++)
        for (j = 0; j < INTRA_MODES - 1; j++)  
   y_mode_probs[ i ][ j ] = diff_update_prob( y_mode_probs[ i ][ j ] )  
}

read_partition_probs() {
    for (i = 0; i < PARTITION_CONTEXTS; i++)
        for (j = 0; j < PARTITION_TYPES - 1; j++)
   partition_probs[ i ][ j ] = diff_update_prob( partition_probs[ i ][ j ] )
}

mv_probs() {
    for (j = 0; j < MV_JOINTS - 1; j++)
        mv_joint_probs[j] = update_mv_prob(mv_joint_probs[j])
    for (i = 0; i < 2; i++) {
        mv_sign_prob[i] = update_mv_prob(mv_sign_prob[i])
        for (j = 0; j < MV_CLASSES - 1; j++)
            mv_class_probs[i][j] = update_mv_prob(mv_class_probs[i][j])
        mv_class0_bit_prob[i] = update_mv_prob(mv_class0_bit_prob[i])
        for (j = 0; j < MV_OFFSET_BITS; j++)
            mv_bits_prob[i][j] = update_mv_prob(mv_bits_prob[i][j])
    }
    for (i = 0; i < 2; i++) {
        for (j = 0; j < CLASS0_SIZE; j++)
            for (k = 0; k < MV_FR_SIZE - 1; k++)
                mv_class0_fr_probs[i][j][k] = update_mv_prob(mv_class0_fr_probs[i][j][k])
        for (k = 0; k < MV_FR_SIZE - 1; k++)
            mv_fr_probs[i][k] = update_mv_prob(mv_fr_probs[i][k])
    }
    if (allow_high_precision_mv) {
        for (i = 0; i < 2; i++) {
            mv_class0_hp_prob[i] = update_mv_prob(mv_class0_hp_prob[i])
            mv_hp_prob[i] = update_mv_prob(mv_hp_prob[i])
        }  
 }  
}

update_mv_prob(prob) { 
 update_mv_prob B(252)
    if (update_mv_prob == 1) {  
  mv_prob L(7)
        prob = (mv_prob << 1) | 1
    }
 return prob
}

setup_compound_reference_mode() {
    if (ref_frame_sign_bias[LAST_FRAME] ==
        ref_frame_sign_bias[GOLDEN_FRAME]) {

        CompFixedRef = ALTREF_FRAME
        CompVarRef[0] = LAST_FRAME
        CompVarRef[1] = GOLDEN_FRAME
    } else if (ref_frame_sign_bias[LAST_FRAME] ==
        ref_frame_sign_bias[ALTREF_FRAME]) {

        CompFixedRef = GOLDEN_FRAME
        CompVarRef[0] = LAST_FRAME
        CompVarRef[1] = ALTREF_FRAME
    } else {
        CompFixedRef = LAST_FRAME
        CompVarRef[0] = GOLDEN_FRAME
        CompVarRef[1] = ALTREF_FRAME  
 }  
}

superframe( sz ) {
    for( i = 0; i < NumFrames; i++ )
        frame( frame_sizes[ i ] )
    superframe_index( )
}

superframe_index( ) {
    superframe_header( )
    for( i = 0; i < NumFrames; i++ )
        frame_sizes[ i ] f(SzBytes)
    superframe_header( )
}

superframe_header( ) {
    superframe_marker f(3)
    bytes_per_framesize_minus_1 f(2)
    frames_in_superframe_minus_1 f(3)
}
