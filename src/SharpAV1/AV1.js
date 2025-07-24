open_bitstream_unit( sz ) { 
    obu_header()
    if ( obu_has_size_field ) {
       obu_size leb128()
    } else {
       obu_size = sz - 1 - obu_extension_flag
    }
    startPosition = get_position()
    if ( obu_type != OBU_SEQUENCE_HEADER && obu_type != OBU_TEMPORAL_DELIMITER && OperatingPointIdc != 0 && obu_extension_flag == 1 ) {
       inTemporalLayer = (OperatingPointIdc >> temporal_id ) & 1
       inSpatialLayer = (OperatingPointIdc >> ( spatial_id + 8 ) ) & 1
       if ( !inTemporalLayer || ! inSpatialLayer ) {
          drop_obu()
       return
    }
 }
 if ( obu_type == OBU_SEQUENCE_HEADER )
    sequence_header_obu()
 else if ( obu_type == OBU_TEMPORAL_DELIMITER )
    temporal_delimiter_obu()
 else if ( obu_type == OBU_FRAME_HEADER )
    frame_header_obu()
 else if ( obu_type == OBU_REDUNDANT_FRAME_HEADER )
    frame_header_obu()
 else if ( obu_type == OBU_TILE_GROUP )
    tile_group_obu( obu_size )
 else if ( obu_type == OBU_METADATA )
    metadata_obu()
 else if ( obu_type == OBU_FRAME )
    frame_obu( obu_size )
 else if ( obu_type == OBU_TILE_LIST )
    tile_list_obu()
 else if ( obu_type == OBU_PADDING )
    padding_obu()
 else
    reserved_obu()
 currentPosition = get_position()
 payloadBits = currentPosition - startPosition
 if ( obu_size > 0 && obu_type != OBU_TILE_GROUP && obu_type != OBU_TILE_LIST && obu_type != OBU_FRAME ) {
    trailing_bits( obu_size * 8 - payloadBits )
 }
}

obu_header() { 
 obu_forbidden_bit f(1)
 obu_type f(4)
 obu_extension_flag f(1)
 obu_has_size_field f(1)
 obu_reserved_1bit f(1)
 if ( obu_extension_flag == 1 )
  obu_extension_header()
}

 obu_extension_header() { 
 temporal_id f(3)
 spatial_id f(2)
 extension_header_reserved_3bits f(3)
 }

trailing_bits( nbBits ) { 
 trailing_one_bit f(1)
 nbBits--
while ( nbBits > 0 ) {
 trailing_zero_bit f(1)
 nbBits--
}
}

byte_alignment() { 
 while ( get_position() & 7 )
 zero_bit f(1)
}

reserved_obu() { 
}

sequence_header_obu() { 
 seq_profile f(3)
 still_picture f(1)
 reduced_still_picture_header f(1)
 if ( reduced_still_picture_header ) {
 timing_info_present_flag = 0
 decoder_model_info_present_flag = 0
 initial_display_delay_present_flag = 0
 operating_points_cnt_minus_1 = 0
 operating_point_idc[ 0 ] = 0
 seq_level_idx[ 0 ] f(5)
 seq_tier[ 0 ] = 0
 decoder_model_present_for_this_op[ 0 ] = 0
 initial_display_delay_present_for_this_op[ 0 ] = 0
 } else {
 timing_info_present_flag f(1)
 if ( timing_info_present_flag ) {
 timing_info()
 decoder_model_info_present_flag f(1)
 if ( decoder_model_info_present_flag ) {
 decoder_model_info()
 }
 } else {
 decoder_model_info_present_flag = 0
 }
 initial_display_delay_present_flag f(1)
 operating_points_cnt_minus_1 f(5)
 for ( i = 0; i <= operating_points_cnt_minus_1; i++ ) {
 operating_point_idc[ i ] f(12)
 seq_level_idx[ i ] f(5)
 if ( seq_level_idx[ i ] > 7 ) {
 seq_tier[ i ] f(1)
 } else {
 seq_tier[ i ] = 0
 }
 if ( decoder_model_info_present_flag ) {
 decoder_model_present_for_this_op[ i ] f(1)
 if ( decoder_model_present_for_this_op[ i ] ) {
 operating_parameters_info( i )
 }
 } else {
 decoder_model_present_for_this_op[ i ] = 0
 }
 if ( initial_display_delay_present_flag ) {
 initial_display_delay_present_for_this_op[ i ] f(1)
 if ( initial_display_delay_present_for_this_op[ i ] ) {
 initial_display_delay_minus_1[ i ] f(4)
 }
 }
 }
 }
 operatingPoint = choose_operating_point()
 OperatingPointIdc = operating_point_idc[ operatingPoint ]
 frame_width_bits_minus_1 f(4)
 frame_height_bits_minus_1 f(4)
 n = frame_width_bits_minus_1 + 1
 max_frame_width_minus_1 f(n)
 n = frame_height_bits_minus_1 + 1
 max_frame_height_minus_1 f(n)
 if ( reduced_still_picture_header )
 frame_id_numbers_present_flag = 0
 else
 frame_id_numbers_present_flag f(1)
 if ( frame_id_numbers_present_flag ) {
 delta_frame_id_length_minus_2 f(4)
 additional_frame_id_length_minus_1 f(3)
 }
 use_128x128_superblock f(1)
 enable_filter_intra f(1)
 enable_intra_edge_filter f(1)
 if ( reduced_still_picture_header ) {
 enable_interintra_compound = 0
 enable_masked_compound = 0
 enable_warped_motion = 0
 enable_dual_filter = 0
 enable_order_hint = 0
 enable_jnt_comp = 0
 enable_ref_frame_mvs = 0
 seq_force_screen_content_tools = SELECT_SCREEN_CONTENT_TOOLS
 seq_force_integer_mv = SELECT_INTEGER_MV
 OrderHintBits = 0
 } else {
 enable_interintra_compound f(1)
 enable_masked_compound f(1)
 enable_warped_motion f(1)
 enable_dual_filter f(1)
 enable_order_hint f(1)
 if ( enable_order_hint ) {
 enable_jnt_comp f(1)
 enable_ref_frame_mvs f(1)
 } else {
 enable_jnt_comp = 0
 enable_ref_frame_mvs = 0
 }
 seq_choose_screen_content_tools f(1)
 if ( seq_choose_screen_content_tools ) {
seq_force_screen_content_tools = SELECT_SCREEN_CONTENT_TOOLS
 } else {
 seq_force_screen_content_tools f(1)
 }
 if ( seq_force_screen_content_tools > 0 ) {
 seq_choose_integer_mv f(1)
 if ( seq_choose_integer_mv ) {
 seq_force_integer_mv = SELECT_INTEGER_MV
 } else {
 seq_force_integer_mv f(1)
 }
 } else {
 seq_force_integer_mv = SELECT_INTEGER_MV
 }
 if ( enable_order_hint ) {
 order_hint_bits_minus_1 f(3)
 OrderHintBits = order_hint_bits_minus_1 + 1
 } else {
 OrderHintBits = 0
 }
 }
 enable_superres f(1)
 enable_cdef f(1)
 enable_restoration f(1)
 color_config()
 film_grain_params_present f(1)
}

timing_info() { 
 num_units_in_display_tick f(32)
 time_scale f(32)
 equal_picture_interval f(1)
 if ( equal_picture_interval )
 num_ticks_per_picture_minus_1 uvlc()
}

decoder_model_info() { 
 buffer_delay_length_minus_1 f(5)
 num_units_in_decoding_tick f(32)
 buffer_removal_time_length_minus_1 f(5)
 frame_presentation_time_length_minus_1 f(5)
}

operating_parameters_info( op ) { 
 n = buffer_delay_length_minus_1 + 1
 decoder_buffer_delay[ op ] f(n)
 encoder_buffer_delay[ op ] f(n)
 low_delay_mode_flag[ op ] f(1)
}

color_config() { 
 high_bitdepth f(1)
 if ( seq_profile == 2 && high_bitdepth ) {
 twelve_bit f(1)
 BitDepth = twelve_bit ? 12 : 10
 } else if ( seq_profile <= 2 ) {
 BitDepth = high_bitdepth ? 10 : 8
 }
 if ( seq_profile == 1 ) {
 mono_chrome = 0
 } else {
 mono_chrome f(1)
 }
 NumPlanes = mono_chrome ? 1 : 3
 color_description_present_flag f(1)
 if ( color_description_present_flag ) {
 color_primaries f(8)
 transfer_characteristics f(8)
 matrix_coefficients f(8)
 } else {
 color_primaries = CP_UNSPECIFIED
 transfer_characteristics = TC_UNSPECIFIED
 matrix_coefficients = MC_UNSPECIFIED
 }
 if ( mono_chrome ) {
 color_range f(1)
 subsampling_x = 1
 subsampling_y = 1
 chroma_sample_position = CSP_UNKNOWN
 separate_uv_delta_q = 0
 return
 } else if ( color_primaries == CP_BT_709 &&
 transfer_characteristics == TC_SRGB &&
 matrix_coefficients == MC_IDENTITY ) {
 color_range = 1
 subsampling_x = 0
 subsampling_y = 0
 } else {
 color_range f(1)
 if ( seq_profile == 0 ) {
 subsampling_x = 1
 subsampling_y = 1
 } else if ( seq_profile == 1 ) {
 subsampling_x = 0
 subsampling_y = 0
 } else {
 if ( BitDepth == 12 ) {
 subsampling_x f(1)
 if ( subsampling_x )
 subsampling_y f(1)
 else
 subsampling_y = 0
 } else {
 subsampling_x = 1
 subsampling_y = 0
 }
 }
 if ( subsampling_x && subsampling_y ) {
 chroma_sample_position f(2)
 }
 }
 separate_uv_delta_q f(1)
}

frame_header_obu() { 
 if ( SeenFrameHeader == 1 ) {
 frame_header_copy()
 } else {
 SeenFrameHeader = 1
 uncompressed_header()
 if ( show_existing_frame ) {
 decode_frame_wrapup()
 SeenFrameHeader = 0
 } else {
 TileNum = 0
 SeenFrameHeader = 1
 }
 }
 }

 uncompressed_header() { 
 if ( frame_id_numbers_present_flag ) {
    idLen = ( additional_frame_id_length_minus_1 + delta_frame_id_length_minus_2 + 3 )
 }
 allFrames = (1 << NUM_REF_FRAMES) - 1
 if ( reduced_still_picture_header ) {
    show_existing_frame = 0
    frame_type = KEY_FRAME
    FrameIsIntra = 1
    show_frame = 1
    showable_frame = 0
 } else {
    show_existing_frame f(1)
    if ( show_existing_frame == 1 ) {
       frame_to_show_map_idx f(3)
       /* if ( decoder_model_info_present_flag && !equal_picture_interval ) {
          temporal_point_info()
       }
       refresh_frame_flags = 0
       if ( frame_id_numbers_present_flag ) {
          display_frame_id f(idLen)
       }
       frame_type = RefFrameType[ frame_to_show_map_idx ]
       if ( frame_type == KEY_FRAME ) {
          refresh_frame_flags = allFrames
       }
       if ( film_grain_params_present ) {
          load_grain_params( frame_to_show_map_idx )
       } */
       
       return
    }
    frame_type f(2)
    FrameIsIntra = (frame_type == INTRA_ONLY_FRAME || frame_type == KEY_FRAME)
    show_frame f(1)
    if ( show_frame && decoder_model_info_present_flag && !equal_picture_interval ) {
       temporal_point_info()
    }
    if ( show_frame ) {
       showable_frame = frame_type != KEY_FRAME
    } else {
    showable_frame f(1)
 }
 if ( frame_type == SWITCH_FRAME || ( frame_type == KEY_FRAME && show_frame ) )
    error_resilient_mode = 1
 else
    error_resilient_mode f(1)
 }
 if ( frame_type == KEY_FRAME && show_frame ) {
    for ( i = 0; i < NUM_REF_FRAMES; i++ ) {
       RefValid[ i ] = 0
       RefOrderHint[ i ] = 0
    }
    for ( i = 0; i < REFS_PER_FRAME; i++ ) {
       OrderHints[ LAST_FRAME + i ] = 0
    }
 }
 disable_cdf_update f(1)
 if ( seq_force_screen_content_tools == SELECT_SCREEN_CONTENT_TOOLS ) {
    allow_screen_content_tools f(1)
 } else {
    allow_screen_content_tools = seq_force_screen_content_tools
 }
 if ( allow_screen_content_tools ) {
    if ( seq_force_integer_mv == SELECT_INTEGER_MV ) {
       force_integer_mv f(1)
    } else {
       force_integer_mv = seq_force_integer_mv
    }
 } else {
    force_integer_mv = 0
 }
 if ( FrameIsIntra ) {
    force_integer_mv = 1
 }
 if ( frame_id_numbers_present_flag ) {
    PrevFrameID = current_frame_id
    current_frame_id f(idLen)
    mark_ref_frames( idLen )
 } else {
    current_frame_id = 0
 }
 if ( frame_type == SWITCH_FRAME )
    frame_size_override_flag = 1
 else if ( reduced_still_picture_header )
    frame_size_override_flag = 0
 else
    frame_size_override_flag f(1)
 order_hint f(OrderHintBits)
 OrderHint = order_hint
 if ( FrameIsIntra || error_resilient_mode ) {
    primary_ref_frame = PRIMARY_REF_NONE
 } else {
    primary_ref_frame f(3)
 }
 if ( decoder_model_info_present_flag ) {
    buffer_removal_time_present_flag f(1)
    if ( buffer_removal_time_present_flag ) {
       for ( opNum = 0; opNum <= operating_points_cnt_minus_1; opNum++ ) {
          if ( decoder_model_present_for_this_op[ opNum ] ) {
             opPtIdc = operating_point_idc[ opNum ]
             inTemporalLayer = ( opPtIdc >> temporal_id ) & 1
             inSpatialLayer = ( opPtIdc >> ( spatial_id + 8 ) ) & 1
             if ( opPtIdc == 0 || ( inTemporalLayer && inSpatialLayer ) ) {
                n = buffer_removal_time_length_minus_1 + 1
                buffer_removal_time[ opNum ] f(n)
             }
          }
       }
    }
 }
 allow_high_precision_mv = 0
 use_ref_frame_mvs = 0
 allow_intrabc = 0
 if ( frame_type == SWITCH_FRAME || ( frame_type == KEY_FRAME && show_frame ) ) {
    refresh_frame_flags = allFrames
 } else {
    refresh_frame_flags f(8)
 }
 if ( !FrameIsIntra || refresh_frame_flags != allFrames ) {
    if ( error_resilient_mode && enable_order_hint ) {
       for ( i = 0; i < NUM_REF_FRAMES; i++) {
          ref_order_hint[ i ] f(OrderHintBits)
          if ( ref_order_hint[ i ] != RefOrderHint[ i ] ) {
             RefValid[ i ] = 0
          }
       }
    }
 }
 if (  FrameIsIntra ) {
    frame_size()
    render_size()
    if ( allow_screen_content_tools && UpscaledWidth == FrameWidth ) {
       allow_intrabc f(1)
    }
 } else {
 if ( !enable_order_hint ) {
    frame_refs_short_signaling = 0
 } else {
    frame_refs_short_signaling f(1)
    if ( frame_refs_short_signaling ) {
       last_frame_idx f(3)
       gold_frame_idx f(3)
       set_frame_refs()
    }
 }
 for ( i = 0; i < REFS_PER_FRAME; i++ ) {
 if ( !frame_refs_short_signaling )
 ref_frame_idx[ i ] f(3)
 if ( frame_id_numbers_present_flag ) {
 n = delta_frame_id_length_minus_2 + 2
 delta_frame_id_minus_1 f(n)
 DeltaFrameId = delta_frame_id_minus_1 + 1
 expectedFrameId[ i ] = ((current_frame_id + (1 << idLen) - DeltaFrameId ) % (1 << idLen))
 }
 }
 if ( frame_size_override_flag && !error_resilient_mode ) {
 frame_size_with_refs()
 } else {
 frame_size()
 render_size()
 }
 if ( force_integer_mv ) {
 allow_high_precision_mv = 0
 } else {
 allow_high_precision_mv f(1)
 }
 read_interpolation_filter()
 is_motion_mode_switchable f(1)
 if ( error_resilient_mode || !enable_ref_frame_mvs ) {
 use_ref_frame_mvs = 0
 } else {
 use_ref_frame_mvs f(1)
 }
 for ( i = 0; i < REFS_PER_FRAME; i++ ) {
 refFrame = LAST_FRAME + i
 hint = RefOrderHint[ ref_frame_idx[ i ] ]
 OrderHints[ refFrame ] = hint
 if ( !enable_order_hint ) {
 RefFrameSignBias[ refFrame ] = 0
 } else {
 RefFrameSignBias[ refFrame ] = get_relative_dist( hint, OrderHint) > 0
 }
 }
 }
 if ( reduced_still_picture_header || disable_cdf_update )
 disable_frame_end_update_cdf = 1
 else
 disable_frame_end_update_cdf f(1)
 if ( primary_ref_frame == PRIMARY_REF_NONE ) {
 init_non_coeff_cdfs()
 setup_past_independence()
 } else {
 load_cdfs( ref_frame_idx[ primary_ref_frame ] )
 load_previous()
 }
 if ( use_ref_frame_mvs == 1 )
 motion_field_estimation()
 tile_info()
 quantization_params()
 segmentation_params()
 delta_q_params()
 delta_lf_params()
 if ( primary_ref_frame == PRIMARY_REF_NONE ) {
 init_coeff_cdfs()
 } else {
 load_previous_segment_ids()
 }
 CodedLossless = 1
 for ( segmentId = 0; segmentId < MAX_SEGMENTS; segmentId++ ) {
 qindex = get_qindex( 1, segmentId )
 LosslessArray[ segmentId ] = qindex == 0 && DeltaQYDc == 0 && DeltaQUAc == 0 && DeltaQUDc == 0 && DeltaQVAc == 0 && DeltaQVDc == 0
 if ( !LosslessArray[ segmentId ] )
 CodedLossless = 0
 if ( using_qmatrix ) {
 if ( LosslessArray[ segmentId ] ) {
 SegQMLevel[ 0 ][ segmentId ] = 15
 SegQMLevel[ 1 ][ segmentId ] = 15
 SegQMLevel[ 2 ][ segmentId ] = 15
 } else {
 SegQMLevel[ 0 ][ segmentId ] = qm_y
 SegQMLevel[ 1 ][ segmentId ] = qm_u
 SegQMLevel[ 2 ][ segmentId ] = qm_v
 }
 }
 }
 AllLossless = CodedLossless && ( FrameWidth == UpscaledWidth )
 loop_filter_params()
 cdef_params()
 lr_params()
 read_tx_mode()
 frame_reference_mode()
 skip_mode_params()
 if ( FrameIsIntra || error_resilient_mode || !enable_warped_motion )
 allow_warped_motion = 0
 else
 allow_warped_motion f(1)
 reduced_tx_set f(1)
 global_motion_params()
 film_grain_params()
 }

 temporal_point_info() { 
 n = frame_presentation_time_length_minus_1 + 1
 frame_presentation_time f(n)
 }

 frame_size() { 
 if ( frame_size_override_flag ) {
 n = frame_width_bits_minus_1 + 1
 frame_width_minus_1 f(n)
 n = frame_height_bits_minus_1 + 1
 frame_height_minus_1 f(n)
 FrameWidth = frame_width_minus_1 + 1
 FrameHeight = frame_height_minus_1 + 1
 } else {
 FrameWidth = max_frame_width_minus_1 + 1
 FrameHeight = max_frame_height_minus_1 + 1
 }
 superres_params()
 compute_image_size()
 }

 render_size() { 
 render_and_frame_size_different f(1)
 if ( render_and_frame_size_different == 1 ) {
 render_width_minus_1 f(16)
 render_height_minus_1 f(16)
 RenderWidth = render_width_minus_1 + 1
 RenderHeight = render_height_minus_1 + 1
 } else {
 RenderWidth = UpscaledWidth
 RenderHeight = FrameHeight
 }
 }

 frame_size_with_refs() { 
 for ( i = 0; i < REFS_PER_FRAME; i++ ) {
 found_ref f(1)
 if ( found_ref == 1 ) {
 UpscaledWidth = RefUpscaledWidth[ ref_frame_idx[ i ] ]
 FrameWidth = UpscaledWidth
 FrameHeight = RefFrameHeight[ ref_frame_idx[ i ] ]
 RenderWidth = RefRenderWidth[ ref_frame_idx[ i ] ]
 RenderHeight = RefRenderHeight[ ref_frame_idx[ i ] ]
 break
 }
 }
 if ( found_ref == 0 ) {
 frame_size()
 render_size()
 } else {
 superres_params()
 compute_image_size()
 }
 }

 read_interpolation_filter() { 
 is_filter_switchable f(1)
 if ( is_filter_switchable == 1 ) {
 interpolation_filter = SWITCHABLE
 } else {
 interpolation_filter f(2)
 }
 }

 get_relative_dist( a, b ) { 
 if ( !enable_order_hint )
 return 0
 diff = a - b
 m = 1 << (OrderHintBits - 1)
 diff = (diff & (m - 1)) - (diff & m)
 return diff
}

tile_info () { 
 sbCols = use_128x128_superblock ? ( ( MiCols + 31 ) >> 5 ) : ( ( MiCols + 15 ) >> 4 )
 sbRows = use_128x128_superblock ? ( ( MiRows + 31 ) >> 5 ) : ( ( MiRows + 15 ) >> 4 )
 sbShift = use_128x128_superblock ? 5 : 4
 sbSize = sbShift + 2
 maxTileWidthSb = MAX_TILE_WIDTH >> sbSize
 maxTileAreaSb = MAX_TILE_AREA >> ( 2 * sbSize )
 minLog2TileCols = tile_log2(maxTileWidthSb, sbCols)
 maxLog2TileCols = tile_log2(1, Min(sbCols, MAX_TILE_COLS))
 maxLog2TileRows = tile_log2(1, Min(sbRows, MAX_TILE_ROWS))
 minLog2Tiles = Max(minLog2TileCols, tile_log2(maxTileAreaSb, sbRows * sbCols))
 uniform_tile_spacing_flag f(1)
 if ( uniform_tile_spacing_flag ) {
 TileColsLog2 = minLog2TileCols
 while ( TileColsLog2 < maxLog2TileCols ) {
 increment_tile_cols_log2 f(1)
 if ( increment_tile_cols_log2 == 1 )
 TileColsLog2++
 else
 break
 }
 tileWidthSb = (sbCols + (1 << TileColsLog2) - 1) >> TileColsLog2
 i = 0
 for ( startSb = 0; startSb < sbCols; startSb += tileWidthSb ) {
 MiColStarts[ i ] = startSb << sbShift
 i += 1
 }
 MiColStarts[i] = MiCols
 TileCols = i
 minLog2TileRows = Max( minLog2Tiles - TileColsLog2, 0)
 TileRowsLog2 = minLog2TileRows
 while ( TileRowsLog2 < maxLog2TileRows ) {
 increment_tile_rows_log2 f(1)
 if ( increment_tile_rows_log2 == 1 )
 TileRowsLog2++
 else
 break
 }
 tileHeightSb = (sbRows + (1 << TileRowsLog2) - 1) >> TileRowsLog2
 i = 0
 for ( startSb = 0; startSb < sbRows; startSb += tileHeightSb ) {
 MiRowStarts[ i ] = startSb << sbShift
 i += 1
 }
 MiRowStarts[i] = MiRows
 TileRows = i
 } else {
 widestTileSb = 0
 startSb = 0
 for ( i = 0; startSb < sbCols; i++ ) {
 MiColStarts[ i ] = startSb << sbShift
 maxWidth = Min(sbCols - startSb, maxTileWidthSb)
 width_in_sbs_minus_1 ns(maxWidth)
 sizeSb = width_in_sbs_minus_1 + 1
 widestTileSb = Max( sizeSb, widestTileSb )
 startSb += sizeSb
 }
 MiColStarts[i] = MiCols
 TileCols = i
 TileColsLog2 = tile_log2(1, TileCols)
 if ( minLog2Tiles > 0 )
 maxTileAreaSb = (sbRows * sbCols) >> (minLog2Tiles + 1)
 else
 maxTileAreaSb = sbRows * sbCols
 maxTileHeightSb = Max( maxTileAreaSb / widestTileSb, 1 )
 startSb = 0
 for ( i = 0; startSb < sbRows; i++ ) {
 MiRowStarts[ i ] = startSb << sbShift
 maxHeight = Min(sbRows - startSb, maxTileHeightSb)
 height_in_sbs_minus_1 ns(maxHeight)
 sizeSb = height_in_sbs_minus_1 + 1
 startSb += sizeSb
 }
 MiRowStarts[ i ] = MiRows
 TileRows = i
 TileRowsLog2 = tile_log2(1, TileRows)
 }
 if ( TileColsLog2 > 0 || TileRowsLog2 > 0 ) {
 context_update_tile_id f(TileRowsLog2+TileColsLog2)
 tile_size_bytes_minus_1 f(2)
 TileSizeBytes = tile_size_bytes_minus_1 + 1
 } else {
 context_update_tile_id = 0
 }
 }

tile_log2( blkSize, target ) { 
 for ( k = 0; (blkSize << k) < target; k++ ) {
 }
 return k
 }

 quantization_params() { 
 base_q_idx f(8)
 DeltaQYDc = read_delta_q()
 if ( NumPlanes > 1 ) {
 if ( separate_uv_delta_q )
 diff_uv_delta f(1)
 else
 diff_uv_delta = 0
 DeltaQUDc = read_delta_q()
 DeltaQUAc = read_delta_q()
 if ( diff_uv_delta ) {
 DeltaQVDc = read_delta_q()
 DeltaQVAc = read_delta_q()
 } else {
 DeltaQVDc = DeltaQUDc
 DeltaQVAc = DeltaQUAc
 }
 } else {
 DeltaQUDc = 0
 DeltaQUAc = 0
 DeltaQVDc = 0
 DeltaQVAc = 0
 }
 using_qmatrix f(1)
 if ( using_qmatrix ) {
 qm_y f(4)
 qm_u f(4)
 if ( !separate_uv_delta_q )
 qm_v = qm_u
 else
 qm_v f(4)
 }
 }

 read_delta_q() { 
 delta_coded f(1)
 if ( delta_coded ) {
 delta_q su(1+6)
 } else {
 delta_q = 0
 }
 return delta_q
 }

 segmentation_params() { 
 segmentation_enabled f(1)
 if ( segmentation_enabled == 1 ) {
 if ( primary_ref_frame == PRIMARY_REF_NONE ) {
 segmentation_update_map = 1
 segmentation_temporal_update = 0
 segmentation_update_data = 1
 } else {
 segmentation_update_map f(1)
 if ( segmentation_update_map == 1 )
 segmentation_temporal_update f(1)
 segmentation_update_data f(1)
 }
 if ( segmentation_update_data == 1 ) {
 for ( i = 0; i < MAX_SEGMENTS; i++ ) {
 for ( j = 0; j < SEG_LVL_MAX; j++ ) {
 feature_value = 0
 feature_enabled f(1)
 FeatureEnabled[ i ][ j ] = feature_enabled
 clippedValue = 0
 if ( feature_enabled == 1 ) {
 bitsToRead = Segmentation_Feature_Bits[ j ]
 limit = Segmentation_Feature_Max[ j ]
 if ( Segmentation_Feature_Signed[ j ] == 1 ) {
 feature_value su(1+bitsToRead)
 clippedValue = Clip3( -limit, limit, feature_value)
 } else {
 feature_value f(bitsToRead)
 clippedValue = Clip3( 0, limit, feature_value)
 }
 }
 FeatureData[ i ][ j ] = clippedValue
 }
 }
 }
 } else {
 for ( i = 0; i < MAX_SEGMENTS; i++ ) {
 for ( j = 0; j < SEG_LVL_MAX; j++ ) {
 FeatureEnabled[ i ][ j ] = 0
 FeatureData[ i ][ j ] = 0
 }
 }
 }
 SegIdPreSkip = 0
 LastActiveSegId = 0
 for ( i = 0; i < MAX_SEGMENTS; i++ ) {
 for ( j = 0; j < SEG_LVL_MAX; j++ ) {
 if ( FeatureEnabled[ i ][ j ] ) {
 LastActiveSegId = i
 if ( j >= SEG_LVL_REF_FRAME ) {
 SegIdPreSkip = 1
 }
 }
 }
 }
 }

delta_q_params() { 
 delta_q_res = 0
 delta_q_present = 0
 if ( base_q_idx > 0 ) {
 delta_q_present f(1)
 }
 if ( delta_q_present ) {
 delta_q_res f(2)
 }
 }

delta_lf_params() { 
 delta_lf_present = 0
 delta_lf_res = 0
 delta_lf_multi = 0
 if ( delta_q_present ) {
 if ( !allow_intrabc )
 delta_lf_present f(1)
 if ( delta_lf_present ) {
 delta_lf_res f(2)
 delta_lf_multi f(1)
 }
 }
 }

cdef_params() { 
 if ( CodedLossless || allow_intrabc || !enable_cdef) {
 cdef_bits = 0
 cdef_y_pri_strength[0] = 0
 cdef_y_sec_strength[0] = 0
 cdef_uv_pri_strength[0] = 0
 cdef_uv_sec_strength[0] = 0
 CdefDamping = 3
 return
 }
 cdef_damping_minus_3 f(2)
 CdefDamping = cdef_damping_minus_3 + 3
 cdef_bits f(2)
 for ( i = 0; i < (1 << cdef_bits); i++ ) {
 cdef_y_pri_strength[i] f(4)
 cdef_y_sec_strength[i] f(2)
 if ( cdef_y_sec_strength[i] == 3 )
 cdef_y_sec_strength[i] += 1
 if ( NumPlanes > 1 ) {
 cdef_uv_pri_strength[i] f(4)
 cdef_uv_sec_strength[i] f(2)
 if ( cdef_uv_sec_strength[i] == 3 )
 cdef_uv_sec_strength[i] += 1
 }
 }
 }

lr_params() { 
 if ( AllLossless || allow_intrabc || !enable_restoration ) {
 FrameRestorationType[0] = RESTORE_NONE
 FrameRestorationType[1] = RESTORE_NONE
 FrameRestorationType[2] = RESTORE_NONE
 UsesLr = 0
 return
 }
 UsesLr = 0
 usesChromaLr = 0
 for ( i = 0; i < NumPlanes; i++ ) {
 lr_type f(2)
 FrameRestorationType[i] = Remap_Lr_Type[lr_type]
 if ( FrameRestorationType[i] != RESTORE_NONE ) {
 UsesLr = 1
 if ( i > 0 ) {
 usesChromaLr = 1
 }
 }
 }
 if ( UsesLr ) {
 if ( use_128x128_superblock ) {
 lr_unit_shift f(1)
 lr_unit_shift++
 } else {
 lr_unit_shift f(1)
 if ( lr_unit_shift ) {
 lr_unit_extra_shift f(1)
 lr_unit_shift += lr_unit_extra_shift
 }
 }
 LoopRestorationSize[ 0 ] = RESTORATION_TILESIZE_MAX >> (2 - lr_unit_shift)
 if ( subsampling_x && subsampling_y && usesChromaLr ) {
 lr_uv_shift f(1)
 } else {
 lr_uv_shift = 0
 }
 LoopRestorationSize[ 1 ] = LoopRestorationSize[ 0 ] >> lr_uv_shift
 LoopRestorationSize[ 2 ] = LoopRestorationSize[ 0 ] >> lr_uv_shift
 }
 }

 loop_filter_params() { 
 if ( CodedLossless || allow_intrabc ) {
 loop_filter_level[ 0 ] = 0
 loop_filter_level[ 1 ] = 0
 loop_filter_ref_deltas[ INTRA_FRAME ] = 1
 loop_filter_ref_deltas[ LAST_FRAME ] = 0
 loop_filter_ref_deltas[ LAST2_FRAME ] = 0
 loop_filter_ref_deltas[ LAST3_FRAME ] = 0
 loop_filter_ref_deltas[ BWDREF_FRAME ] = 0
 loop_filter_ref_deltas[ GOLDEN_FRAME ] = -1
 loop_filter_ref_deltas[ ALTREF_FRAME ] = -1
 loop_filter_ref_deltas[ ALTREF2_FRAME ] = -1
 for ( i = 0; i < 2; i++ ) {
 loop_filter_mode_deltas[ i ] = 0
 }
 return
 }
 loop_filter_level[ 0 ] f(6)
 loop_filter_level[ 1 ] f(6)
 if ( NumPlanes > 1 ) {
 if ( loop_filter_level[ 0 ] || loop_filter_level[ 1 ] ) {
 loop_filter_level[ 2 ] f(6)
 loop_filter_level[ 3 ] f(6)
 }
 }
 loop_filter_sharpness f(3)
 loop_filter_delta_enabled f(1)
 if ( loop_filter_delta_enabled == 1 ) {
 loop_filter_delta_update f(1)
 if ( loop_filter_delta_update == 1 ) {
 for ( i = 0; i < TOTAL_REFS_PER_FRAME; i++ ) {
 update_ref_delta f(1)
 if ( update_ref_delta == 1 )
 loop_filter_ref_deltas[ i ] su(1+6)
 }
 for ( i = 0; i < 2; i++ ) {
 update_mode_delta f(1)
 if ( update_mode_delta == 1 )
 loop_filter_mode_deltas[ i ] su(1+6)
 }
 }
 }
 }

 read_tx_mode() { 
 if ( CodedLossless == 1 ) {
 TxMode = ONLY_4X4
 } else {
 tx_mode_select f(1)
 if ( tx_mode_select ) {
 TxMode = TX_MODE_SELECT
 } else {
 TxMode = TX_MODE_LARGEST
 }
 }
 }

 frame_reference_mode() { 
 if ( FrameIsIntra ) {
 reference_select = 0
 } else {
 reference_select f(1)
 }
 }

 skip_mode_params() { 
 if ( FrameIsIntra || !reference_select || !enable_order_hint ) {
 skipModeAllowed = 0
 } else {
 forwardIdx = -1
 backwardIdx = -1
 for ( i = 0; i < REFS_PER_FRAME; i++ ) {
 refHint = RefOrderHint[ ref_frame_idx[ i ] ]
 if ( get_relative_dist( refHint, OrderHint ) < 0 ) {
 if ( forwardIdx < 0 ||
 get_relative_dist( refHint, forwardHint) > 0 ) {
 forwardIdx = i
 forwardHint = refHint
 }
 } else if ( get_relative_dist( refHint, OrderHint) > 0 ) {
 if ( backwardIdx < 0 ||
 get_relative_dist( refHint, backwardHint) < 0 ) {
 backwardIdx = i
 backwardHint = refHint
 }
 }
 }
 if ( forwardIdx < 0 ) {
 skipModeAllowed = 0
 } else if ( backwardIdx >= 0 ) {
 skipModeAllowed = 1
 SkipModeFrame[ 0 ] = LAST_FRAME + Min(forwardIdx, backwardIdx)
 SkipModeFrame[ 1 ] = LAST_FRAME + Max(forwardIdx, backwardIdx)
 } else {
 secondForwardIdx = -1
 for ( i = 0; i < REFS_PER_FRAME; i++ ) {
 refHint = RefOrderHint[ ref_frame_idx[ i ] ]
 if ( get_relative_dist( refHint, forwardHint ) < 0 ) {
 if ( secondForwardIdx < 0 ||
 get_relative_dist( refHint, secondForwardHint ) > 0 ) {
 secondForwardIdx = i
 secondForwardHint = refHint
 }
 }
 }
 if ( secondForwardIdx < 0 ) {
 skipModeAllowed = 0
 } else {
 skipModeAllowed = 1
 SkipModeFrame[ 0 ] = LAST_FRAME + Min(forwardIdx, secondForwardIdx)
 SkipModeFrame[ 1 ] = LAST_FRAME + Max(forwardIdx, secondForwardIdx)
 }
 }
 }
 if ( skipModeAllowed ) {
 skip_mode_present f(1)
 } else {
 skip_mode_present = 0
 }
 }

 global_motion_params() { 
 for ( ref = LAST_FRAME; ref <= ALTREF_FRAME; ref++ ) {
 GmType[ ref ] = IDENTITY
 for ( i = 0; i < 6; i++ ) {
 gm_params[ ref ][ i ] = ( ( i % 3 == 2 ) ? 1 << WARPEDMODEL_PREC_BITS : 0 )
 }
 }
 if ( FrameIsIntra )
 return
 for ( ref = LAST_FRAME; ref <= ALTREF_FRAME; ref++ ) {
 is_global f(1)
 if ( is_global ) {
 is_rot_zoom f(1)
 if ( is_rot_zoom ) {
 type = ROTZOOM
 } else {
 is_translation f(1)
 type = is_translation ? TRANSLATION : AFFINE
 }
 } else {
 type = IDENTITY
 }
 GmType[ref] = type
 if ( type >= ROTZOOM ) {
 read_global_param(type, ref, 2)
 read_global_param(type, ref, 3)
 if ( type == AFFINE ) {
 read_global_param(type, ref, 4)
 read_global_param(type, ref, 5)
 } else {
 gm_params[ref][4] = -gm_params[ref][3]
 gm_params[ref][5] = gm_params[ref][2]
 }
 }
 if ( type >= TRANSLATION ) {
 read_global_param(type, ref, 0)
 read_global_param(type, ref, 1)
 }
 }
 }

read_global_param( type, ref, idx ) { 
 absBits = GM_ABS_ALPHA_BITS
 precBits = GM_ALPHA_PREC_BITS
 if ( idx < 2 ) {
 if ( type == TRANSLATION ) {
 absBits = GM_ABS_TRANS_ONLY_BITS - !allow_high_precision_mv
 precBits = GM_TRANS_ONLY_PREC_BITS - !allow_high_precision_mv
 } else {
 absBits = GM_ABS_TRANS_BITS
 precBits = GM_TRANS_PREC_BITS
 }
 }
 precDiff = WARPEDMODEL_PREC_BITS - precBits
 round = (idx % 3) == 2 ? (1 << WARPEDMODEL_PREC_BITS) : 0
 sub = (idx % 3) == 2 ? (1 << precBits) : 0
 mx = (1 << absBits)
 r = (PrevGmParams[ref][idx] >> precDiff) - sub
 gm_params[ref][idx] = (decode_signed_subexp_with_ref( -mx, mx + 1, r ) << precDiff) + round
 }

 film_grain_params() { 
 if ( !film_grain_params_present ||
 (!show_frame && !showable_frame) ) {
 reset_grain_params()
 return
 }
 apply_grain f(1)
 if ( !apply_grain ) {
 reset_grain_params()
 return
 }
 grain_seed f(16)
 if ( frame_type == INTER_FRAME )
 update_grain f(1)
 else
 update_grain = 1
 if ( !update_grain ) {
 film_grain_params_ref_idx f(3)
 tempGrainSeed = grain_seed
 load_grain_params( film_grain_params_ref_idx )
 grain_seed = tempGrainSeed
 return
 }
 num_y_points f(4)
 for ( i = 0; i < num_y_points; i++ ) {
 point_y_value[ i ] f(8)
 point_y_scaling[ i ] f(8)
 }
 if ( mono_chrome ) {
 chroma_scaling_from_luma = 0
 } else {
 chroma_scaling_from_luma f(1)
 }
 if ( mono_chrome || chroma_scaling_from_luma ||
 ( subsampling_x == 1 && subsampling_y == 1 &&
 num_y_points == 0 )
 ) {
 num_cb_points = 0
 num_cr_points = 0
 } else {
 num_cb_points f(4)
 for ( i = 0; i < num_cb_points; i++ ) {
 point_cb_value[ i ] f(8)
 point_cb_scaling[ i ] f(8)
 }
 num_cr_points f(4)
 for ( i = 0; i < num_cr_points; i++ ) {
 point_cr_value[ i ] f(8)
 point_cr_scaling[ i ] f(8)
 }
 }
 grain_scaling_minus_8 f(2)
 ar_coeff_lag f(2)
 numPosLuma = 2 * ar_coeff_lag * ( ar_coeff_lag + 1 )
 if ( num_y_points ) {
 numPosChroma = numPosLuma + 1
 for ( i = 0; i < numPosLuma; i++ )
 ar_coeffs_y_plus_128[ i ] f(8)
 } else {
 numPosChroma = numPosLuma
 }
 if ( chroma_scaling_from_luma || num_cb_points ) {
 for ( i = 0; i < numPosChroma; i++ )
 ar_coeffs_cb_plus_128[ i ] f(8)
 }
 if ( chroma_scaling_from_luma || num_cr_points ) {
 for ( i = 0; i < numPosChroma; i++ )
 ar_coeffs_cr_plus_128[ i ] f(8)
 }
 ar_coeff_shift_minus_6 f(2)
 grain_scale_shift f(2)
 if ( num_cb_points ) {
 cb_mult f(8)
 cb_luma_mult f(8)
 cb_offset f(9)
 }
 if ( num_cr_points ) {
 cr_mult f(8)
 cr_luma_mult f(8)
 cr_offset f(9)
 }
 overlap_flag f(1)
 clip_to_restricted_range f(1)
 }

superres_params() { 
 if ( enable_superres )
 use_superres f(1)
 else
 use_superres = 0
 if ( use_superres ) {
 coded_denom f(SUPERRES_DENOM_BITS)
 SuperresDenom = coded_denom + SUPERRES_DENOM_MIN
 } else {
 SuperresDenom = SUPERRES_NUM
 }
 UpscaledWidth = FrameWidth
 FrameWidth = (UpscaledWidth * SUPERRES_NUM + (SuperresDenom / 2)) / SuperresDenom
 }

 compute_image_size() { 
 MiCols = 2 * ( ( FrameWidth + 7 ) >> 3 )
 MiRows = 2 * ( ( FrameHeight + 7 ) >> 3 )
 }

 decode_signed_subexp_with_ref( low, high, r ) { 
 x = decode_unsigned_subexp_with_ref(high - low, r - low)
 return x + low
}

decode_unsigned_subexp_with_ref( mx, r ) { 
 v = decode_subexp( mx )
 if ( (r << 1) <= mx ) {
 return inverse_recenter(r, v)
 } else {
 return mx - 1 - inverse_recenter(mx - 1 - r, v)
 }
 }

decode_subexp( numSyms ) { 
 i = 0
 mk = 0
 k = 3
 while ( 1 ) {
 b2 = i ? k + i - 1 : k
 a = 1 << b2
 if ( numSyms <= mk + 3 * a ) {
 subexp_final_bits ns(numSyms - mk)
 return subexp_final_bits + mk
 } else {
 subexp_more_bits f(1)
 if ( subexp_more_bits ) {
 i++
 mk += a
 } else {
 subexp_bits f(b2)
 return subexp_bits + mk
 }
 }
 }
 }

inverse_recenter( r, v ) { 
 if ( v > 2 * r )
    return v
 else if ( v & 1 )
    return r - ((v + 1) >> 1)
 else
    return r + (v >> 1)
 }

temporal_delimiter_obu() { 
 SeenFrameHeader = 0
}

padding_obu() { 
 for ( i = 0; i < obu_padding_length; i++ )
 obu_padding_byte f(8)
}

metadata_obu() { 
 metadata_type leb128()
 if ( metadata_type == METADATA_TYPE_ITUT_T35 )
 metadata_itut_t35()
 else if ( metadata_type == METADATA_TYPE_HDR_CLL )
 metadata_hdr_cll()
 else if ( metadata_type == METADATA_TYPE_HDR_MDCV )
 metadata_hdr_mdcv()
 else if ( metadata_type == METADATA_TYPE_SCALABILITY )
 metadata_scalability()
 else if ( metadata_type == METADATA_TYPE_TIMECODE )
 metadata_timecode()
 }

metadata_itut_t35() { 
 itu_t_t35_country_code f(8)
 if ( itu_t_t35_country_code == 0xFF ) {
 itu_t_t35_country_code_extension_byte f(8)
 }
 itu_t_t35_payload_bytes
}

metadata_hdr_cll() { 
 max_cll f(16)
 max_fall f(16)
}

metadata_hdr_mdcv() { 
 for ( i = 0; i < 3; i++ ) {
 primary_chromaticity_x[ i ] f(16)
 primary_chromaticity_y[ i ] f(16)
 }
 white_point_chromaticity_x f(16)
 white_point_chromaticity_y f(16)
 luminance_max f(32)
 luminance_min f(32)
}

metadata_scalability() { 
 scalability_mode_idc f(8)
 if ( scalability_mode_idc == SCALABILITY_SS )
 scalability_structure()
}

scalability_structure() { 
 spatial_layers_cnt_minus_1 f(2)
 spatial_layer_dimensions_present_flag f(1)
 spatial_layer_description_present_flag f(1)
 temporal_group_description_present_flag f(1)
 scalability_structure_reserved_3bits f(3)
 if ( spatial_layer_dimensions_present_flag ) {
 for ( i = 0; i <= spatial_layers_cnt_minus_1 ; i++ ) {
  spatial_layer_max_width[ i ] f(16)
  spatial_layer_max_height[ i ] f(16)
 }
 }
 if ( spatial_layer_description_present_flag ) {
 for ( i = 0; i <= spatial_layers_cnt_minus_1; i++ )
  spatial_layer_ref_id[ i ] f(8)
 }
 if ( temporal_group_description_present_flag ) {
 temporal_group_size f(8)
 for ( i = 0; i < temporal_group_size; i++ ) {
 temporal_group_temporal_id[ i ] f(3)
 temporal_group_temporal_switching_up_point_flag[ i ] f(1)
 temporal_group_spatial_switching_up_point_flag[ i ] f(1)
 temporal_group_ref_cnt[ i ] f(3)
 for ( j = 0; j < temporal_group_ref_cnt[ i ]; j++ ) {
 temporal_group_ref_pic_diff[ i ][ j ] f(8)
 }
 }
 }
 }

metadata_timecode() { 
 counting_type f(5)
 full_timestamp_flag f(1)
 discontinuity_flag f(1)
 cnt_dropped_flag f(1)
 n_frames f(9)
 if ( full_timestamp_flag ) {
 seconds_value f(6)
 minutes_value f(6)
 hours_value f(5)
 } else {
 seconds_flag f(1)
 if ( seconds_flag ) {
 seconds_value f(6)
 minutes_flag f(1)
 if ( minutes_flag ) {
 minutes_value f(6)
 hours_flag f(1)
 if ( hours_flag ) {
 hours_value f(5)
 }
 }
 }
 }
 time_offset_length f(5)
 if ( time_offset_length > 0 ) {
 time_offset_value f(time_offset_length)
 }
 }

frame_obu( sz ) { 
 startBitPos = get_position()
 frame_header_obu()
 byte_alignment()
 endBitPos = get_position()
 headerBytes = (endBitPos - startBitPos) / 8
 sz -= headerBytes
 tile_group_obu( sz )
 }

 tile_group_obu( sz ) { 
 NumTiles = TileCols * TileRows
 startBitPos = get_position()
 tile_start_and_end_present_flag = 0
 if ( NumTiles > 1 )
 tile_start_and_end_present_flag f(1)
 if ( NumTiles == 1 || !tile_start_and_end_present_flag ) {
 tg_start = 0
 tg_end = NumTiles - 1
 } else {
 tileBits = TileColsLog2 + TileRowsLog2
 tg_start f(tileBits)
 tg_end f(tileBits)
 }
 byte_alignment()
 endBitPos = get_position()
 headerBytes = (endBitPos - startBitPos) / 8
 sz -= headerBytes
 /*for ( TileNum = tg_start; TileNum <= tg_end; TileNum++ ) {
 tileRow = TileNum / TileCols
 tileCol = TileNum % TileCols
 lastTile = TileNum == tg_end
 if ( lastTile ) {
 tileSize = sz
 } else {
 tile_size_minus_1 le(TileSizeBytes)
 tileSize = tile_size_minus_1 + 1
 sz -= tileSize + TileSizeBytes
 }
 MiRowStart = MiRowStarts[ tileRow ]
 MiRowEnd = MiRowStarts[ tileRow + 1 ]
 MiColStart = MiColStarts[ tileCol ]
 MiColEnd = MiColStarts[ tileCol + 1 ]
 CurrentQIndex = base_q_idx
 init_symbol( tileSize )
 decode_tile()
 exit_symbol()
 }*/
 skip_obu()

 if ( tg_end == NumTiles - 1 ) {
 /* if ( !disable_frame_end_update_cdf ) {
 frame_end_update_cdf()
 } */
 decode_frame_wrapup()
 SeenFrameHeader = 0
 }
 }

 tile_list_obu() {
 output_frame_width_in_tiles_minus_1 f(8)
 output_frame_height_in_tiles_minus_1 f(8)
 tile_count_minus_1 f(16)
 for ( tile = 0; tile <= tile_count_minus_1; tile++ )
 tile_list_entry()
 }

tile_list_entry() {
 anchor_frame_idx f(8)
 anchor_tile_row f(8)
 anchor_tile_col f(8)
 tile_data_size_minus_1 f(16)
 N = 8 * (tile_data_size_minus_1 + 1)
 coded_tile_data f(N)
 }
