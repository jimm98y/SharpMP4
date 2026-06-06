open_bitstream_unit( sz ) {
obu_header()	
obuPayloadSize = sz - 1 - obu_header_extension_flag	
startPosition = get_position()	
load_xlayer_context( obu_xlayer_id )	
if ( obu_type == OBU_SEQUENCE_HEADER ) {	
sequence_header_obu()	
} else if ( obu_type == OBU_TEMPORAL_DELIMITER ) {	
FirstPictureInTU = 1	
temporal_delimiter_obu()	
} else if ( obu_type == OBU_MSDO ) {	
multistream_decoder_operation_obu()	
} else if ( obu_type == OBU_MULTI_FRAME_HEADER ) {	
multi_frame_header_obu()	
} else if ( is_sef() || is_tip_frame() || obu_type == OBU_BRIDGE_FRAME ) {	
frame_header( 1 )	
} else if ( obu_type == OBU_METADATA_SHORT ) {	
metadata_short_obu( obuPayloadSize )	
} else if ( obu_type == OBU_METADATA_GROUP ) {	
metadata_group_obu()	
} else if ( is_tile_group() ) {	
tile_group_obu( obuPayloadSize )	
} else if ( obu_type == OBU_LAYER_CONFIGURATION_RECORD ) {	
layer_config_record_obu()	
} else if ( obu_type == OBU_ATLAS_SEGMENT ) {	
atlas_segment_info_obu()	
} else if ( obu_type == OBU_OPERATING_POINT_SET ) {	
operating_point_set_obu()	
} else if ( obu_type == OBU_BUFFER_REMOVAL_TIMING ) {	
buffer_removal_timing_obu()	
} else if ( obu_type == OBU_QUANTIZATION_MATRIX ) {	
quantizer_matrix_obu()	
} else if ( obu_type == OBU_FILM_GRAIN ) {	
film_grain_obu()	
} else if ( obu_type == OBU_CONTENT_INTERPRETATION ) {	
content_interpretation_obu()	
} else if ( obu_type == OBU_PADDING ) {	
padding_obu()	
} else {	
reserved_obu()	
}	
usedArith = is_tile_group()	
currentPosition = get_position()	
parsedPayloadBits = currentPosition - startPosition	
remainingPayloadBits = obuPayloadSize * 8 - parsedPayloadBits	
if ( obuPayloadSize > 0 && !usedArith ) {	
if ( is_extensible_obu() ) {		
obu_extension_flag f(1)
if ( obu_extension_flag ) {	
obu_extension_data( remainingPayloadBits - 1 )	
} else {	
trailing_bits( remainingPayloadBits - 1 )	
}	
} else {	
trailing_bits( remainingPayloadBits )	
}	
}	
save_xlayer_context( obu_xlayer_id )	
}

obu_extension_data( sz ) {
for ( i = 0; i < sz; i++ ) {	
obu_extension_data_bit	f(1)
}	
}

obu_header() {
obu_header_extension_flag	f(1)
obu_type	f(5)
obu_tlayer_id	f(2)
if ( obu_header_extension_flag == 1 ) {	
obu_mlayer_id	f(3)
obu_xlayer_id	f(5)
} else {	
obu_mlayer_id = 0	
obu_xlayer_id = ( obu_type == OBU_MSDO || obu_type == OBU_TEMPORAL_DELIMITER ) ? GLOBAL_XLAYER_ID : 0	
}	
}

trailing_bits( nbBits ) {
trailing_one_bit	f(1)
nbBits--	
while ( nbBits > 0 ) {	
trailing_zero_bit	f(1)
nbBits--	
}	
}

byte_alignment() {
while ( get_position() & 7 ) {	
zero_bit	f(1)
}	
}

reserved_obu() {
}

sequence_header_obu() {
seq_header_id	uvlc()
seq_profile_idc	f(5)
single_picture_header_flag	f(1)
seq_level_idx	f(5)
if ( seq_level_idx > 3 && !single_picture_header_flag ) {	
seq_tier	f(1)
} else {	
seq_tier = 0	
}	
chroma_format_idc	uvlc()
bit_depth_idc	uvlc()
set_chroma_format_and_bit_depth()	
if ( single_picture_header_flag ) {	
seq_lcr_id = 0	
still_picture = 1	
max_tlayer_id = 0	
max_mlayer_id = 0	
SeqMaxMlayerCnt = 1	
monotonic_output_order_flag = 1	
} else {	
seq_lcr_id	f(3)
still_picture	f(1)
max_tlayer_id	f(2)
max_mlayer_id	f(3)
if ( max_mlayer_id > 0 ) {	
n = CeilLog2(max_mlayer_id + 1)	
seq_max_mlayer_cnt_minus_1	f(n)
SeqMaxMlayerCnt = seq_max_mlayer_cnt_minus_1 + 1	
} else {	
SeqMaxMlayerCnt = 1	
}	
monotonic_output_order_flag	f(1)
}	
frame_width_bits_minus_1	f(4)
frame_height_bits_minus_1	f(4)
n = frame_width_bits_minus_1 + 1	
max_frame_width_minus_1	f(n)
n = frame_height_bits_minus_1 + 1	
max_frame_height_minus_1	f(n)
seq_cropping_window_present_flag	f(1)
if ( seq_cropping_window_present_flag ) {	
seq_cropping_win_left_offset	uvlc()
seq_cropping_win_right_offset	uvlc()
seq_cropping_win_top_offset	uvlc()
seq_cropping_win_bottom_offset	uvlc()
} else {	
seq_cropping_win_left_offset = 0	
seq_cropping_win_right_offset = 0	
seq_cropping_win_top_offset = 0	
seq_cropping_win_bottom_offset = 0	
}	
if ( single_picture_header_flag ) {	
decoder_model_info_present_flag = 0	
} else {	
seq_initial_display_delay_present_flag	f(1)
if ( seq_initial_display_delay_present_flag ) {	
seq_initial_display_delay_minus_1	f(4)
}	
decoder_model_info_present_flag	f(1)
if ( decoder_model_info_present_flag ) {	
num_units_in_decoding_tick	f(32)
seq_decoder_model_info_present_flag	f(1)
if ( seq_decoder_model_info_present_flag ) {	
seq_decoder_model_info()	
}	
}	
}	
for ( mLayer = 0; mLayer < MAX_NUM_MLAYERS; mLayer++ ) {	
for ( currTLayer = 0; currTLayer < MAX_NUM_TLAYERS; currTLayer++ ) {	
for ( refTLayer = 0; refTLayer < MAX_NUM_TLAYERS; refTLayer++ ) {	
TLayerDependencyMap[ mLayer ][ currTLayer ][ refTLayer ] =	
refTLayer <= currTLayer && currTLayer <= max_tlayer_id && mLayer <= max_mlayer_id	
}	
}	
}	
for ( currLayer = 0; currLayer < MAX_NUM_MLAYERS; currLayer++ ) {	
for ( refLayer = 0; refLayer < MAX_NUM_MLAYERS; refLayer++ ) {	
MLayerDependencyMap[ currLayer ][ refLayer ] =	
refLayer <= currLayer && currLayer <= max_mlayer_id	
}	
}	
if ( max_mlayer_id > 0 ) {	
mlayer_dependency_present_flag	f(1)
if ( mlayer_dependency_present_flag ) {	
for ( currLayer = 1; currLayer <= max_mlayer_id; currLayer++ ) {	
for ( refLayer = currLayer; refLayer >= 0; refLayer-- ) {	
mlayer_dependency_map	f(1)
MLayerDependencyMap[ currLayer ][ refLayer ] =	
mlayer_dependency_map	
}	
}	
}	
}	
if ( max_tlayer_id > 0 ) {	
tlayer_dependency_present_flag	f(1)
if ( tlayer_dependency_present_flag ) {	
if ( max_mlayer_id > 0 ) {
multi_tlayer_dependency_map_present_flag	f(1)
}
else {
multi_tlayer_dependency_map_present_flag = 0	
}
for ( mLayer = 0; mLayer <= max_mlayer_id; mLayer++ ) {	
for ( currTLayer = 1; currTLayer <= max_tlayer_id; currTLayer++ ) {	
for ( refTLayer = currTLayer; refTLayer >= 0; refTLayer-- ) {	
if (multi_tlayer_dependency_map_present_flag > 0 ||	mLayer == 0) {	
tlayer_dependency_map	f(1)
TLayerDependencyMap[ mLayer ][ currTLayer ][ refTLayer ] =	tlayer_dependency_map	
} else {	
TLayerDependencyMap[ mLayer ][ currTLayer ][ refTLayer ] =	TLayerDependencyMap[ 0 ][ currTLayer ][ refTLayer ]	
}	
}	
}	
}	
}	
}	
for (mlayerId = 0; mlayerId < MAX_NUM_MLAYERS; mlayerId++) {	
for (refMlayer = 0; refMlayer < MAX_NUM_MLAYERS; refMlayer++) {	
MLayerPresenceMap[mlayerId][refMlayer] = 0	
if ( mlayerId == refMlayer || MLayerDependencyMap[mlayerId][refMlayer]) {	
MLayerPresenceMap[mlayerId][refMlayer] = 1	
for (depMLayerId = 0; depMLayerId < refMlayer; depMLayerId++) {	
MLayerPresenceMap[mlayerId][depMLayerId] |=	MLayerPresenceMap[refMlayer][depMLayerId]	
}	
}	
}	
}	
sequence_partition_config()	
sequence_segment_config()	
sequence_intra_config()	
sequence_inter_config()	
sequence_scc_config()	
sequence_transform_quant_entropy_config()	
sequence_filter_config()	
sequence_tile_config()	
film_grain_params_present	f(1)
save_sequence_header()	
}

sequence_tile_config() {
seq_tile_info_present_flag	f(1)
if ( seq_tile_info_present_flag ) {	
allow_tile_info_change	f(1)
seqSbSize = get_seq_sb_size()	
( SeqSbRowStarts, SeqSbRows, SeqTileRows, SeqTileRowsLog2,	SeqSbColStarts, SeqSbCols, SeqTileCols, SeqTileColsLog2, SeqUniformTileSpacingFlag, sbShift) = tile_params(	max_frame_width_minus_1 + 1, max_frame_height_minus_1 + 1,	seqSbSize, seqSbSize, 0 )	
}	
}

sequence_partition_config() {
use_256x256_superblock	f(1)
if ( !use_256x256_superblock ) {	
use_128x128_superblock	f(1)
}	
if ( Monochrome ) {	
enable_sdp = 0	
} else {	
enable_sdp	f(1)
}	
if ( enable_sdp && !single_picture_header_flag ) {	
enable_extended_sdp	f(1)
} else {	
enable_extended_sdp = 0	
}	
enable_ext_partitions	f(1)
if ( enable_ext_partitions ) {	
enable_uneven_4way_partitions	f(1)
} else {	
enable_uneven_4way_partitions = 0	
}	
reduce_pb_aspect_ratio	f(1)
if ( reduce_pb_aspect_ratio ) {	
max_pb_aspect_ratio_log2_minus_1	f(1)
MaxPbAspectRatio = 1 << (max_pb_aspect_ratio_log2_minus_1 + 1)	
} else {	
MaxPbAspectRatio = 8	
}	
}

sequence_segment_config() {
enable_ext_seg	f(1)
MaxSegments = enable_ext_seg ? 16 : 8	
seq_seg_info_present_flag	f(1)
if ( seq_seg_info_present_flag ) {	
seq_allow_seg_info_change	f(1)
( SeqFeatureEnabled, SeqFeatureData ) = seg_info( MaxSegments )	
}	
}

sequence_intra_config() {
enable_dip	f(1)
enable_intra_edge_filter	f(1)
enable_mrls	f(1)
enable_cfl_intra	f(1)
if ( Monochrome ) {	
cfl_ds_filter_index = 0	
} else {	
cfl_ds_filter_index	f(2)
}	
enable_mhccp	f(1)
enable_ibp	f(1)
}

sequence_inter_config() {
if ( single_picture_header_flag ) {	
for ( i = 0; i < MOTION_MODES; i++ ) {	
seq_enabled_motion_modes[ i ] = 0	
}	
enable_six_param_warp_delta = 0	
enable_masked_compound = 0	
enable_ref_frame_mvs = 0	
reduced_ref_frame_mvs_mode = 0	
OrderHintBits = 0	
enable_opfl_refine = REFINE_NONE	
enable_refmvbank	f(1)
disable_drl_reorder	f(1)
if ( disable_drl_reorder ) {	
DrlReorder = DRL_REORDER_DISABLED	
} else {	
constrain_drl_reorder	f(1)
DrlReorder = constrain_drl_reorder ?	
DRL_REORDER_CONSTRAINT : DRL_REORDER_ALWAYS	
}	
n = MAX_REF_BV_STACK_SIZE - 1	
seq_max_bvp_drl_bits_minus_1	ns(n)
allow_frame_max_bvp_drl_bits	f(1)
enable_bawp	f(1)
enable_mv_traj = 0	
enable_imp_msk_bld = 0	
NumRefFrames = 2	
long_term_frame_id_bits = 0	
} else {	
motionModeEnabled = 0	
for ( mode = INTERINTRA; mode < MOTION_MODES; mode++ ) {	
seq_enabled_motion_modes[ mode ]	f(1)
motionModeEnabled |= seq_enabled_motion_modes[ mode ]	
}	
if ( motionModeEnabled ) {	
seq_frame_motion_modes_present_flag	f(1)
} else {	
seq_frame_motion_modes_present_flag = 0	
}	
if ( seq_enabled_motion_modes[ DELTAWARP ] ) {	
enable_six_param_warp_delta	f(1)
} else {	
enable_six_param_warp_delta = 0	
}	
enable_masked_compound	f(1)
enable_ref_frame_mvs	f(1)
if ( enable_ref_frame_mvs ) {	
reduced_ref_frame_mvs_mode	f(1)
} else {	
reduced_ref_frame_mvs_mode = 0	
}	
order_hint_bits_minus_1	f(4)
OrderHintBits = order_hint_bits_minus_1 + 1	
enable_refmvbank	f(1)
disable_drl_reorder	f(1)
if ( disable_drl_reorder ) {	
DrlReorder = DRL_REORDER_DISABLED	
} else {	
constrain_drl_reorder	f(1)
DrlReorder = constrain_drl_reorder ? DRL_REORDER_CONSTRAINT :	
DRL_REORDER_ALWAYS	
}	
explicit_ref_frame_map	f(1)
explicit_num_ref_frames	f(1)
if ( explicit_num_ref_frames ) {	
num_ref_frames_minus_1	f(4)
NumRefFrames = num_ref_frames_minus_1 + 1	
} else {	
NumRefFrames = 8	
}	
ActiveNumRefFrames = Min( REFS_PER_FRAME, NumRefFrames )	
long_term_frame_id_bits	f(3)
n = MAX_REF_MV_STACK_SIZE - 1	
seq_max_drl_bits_minus_1	ns(n)
allow_frame_max_drl_bits	f(1)
n = MAX_REF_BV_STACK_SIZE - 1	
seq_max_bvp_drl_bits_minus_1	ns(n)
allow_frame_max_bvp_drl_bits	f(1)
num_same_ref_compound	f(2)
enable_tip	f(1)
if ( enable_tip ) {	
disable_tip_output	f(1)
EnableTipOutput = !disable_tip_output	
enable_tip_hole_fill	f(1)
} else {	
enable_tip_hole_fill = 0	
EnableTipOutput = 0	
}	
enable_mv_traj	f(1)
enable_bawp	f(1)
enable_cwp	f(1)
enable_imp_msk_bld	f(1)
enable_df_sub_pu	f(1)
if ( EnableTipOutput && enable_df_sub_pu ) {	
enable_tip_explicit_qp	f(1)
} else {	
enable_tip_explicit_qp = 0	
}	
enable_opfl_refine	f(2)
enable_refinemv	f(1)
if ( enable_tip && ( enable_opfl_refine != 0 || enable_refinemv ) ) {	
enable_tip_refinemv	f(1)
} else {	
enable_tip_refinemv = 0	
}	
enable_bru	f(1)
enable_adaptive_mvd	f(1)
enable_mvd_sign_derive	f(1)
enable_flex_mvres	f(1)
if ( single_picture_header_flag ) {	
enable_global_motion = 0	
} else {	
enable_global_motion	f(1)
}	
enable_short_refresh_frame_flags	f(1)
}	
}

sequence_scc_config() {
if ( single_picture_header_flag ) {	
seq_force_screen_content_tools = SELECT_SCREEN_CONTENT_TOOLS	
seq_force_integer_mv = SELECT_INTEGER_MV	
} else {	
seq_choose_screen_content_tools	f(1)
if ( seq_choose_screen_content_tools ) {	
seq_force_screen_content_tools = SELECT_SCREEN_CONTENT_TOOLS	
} else {	
seq_force_screen_content_tools	f(1)
}	
if ( seq_force_screen_content_tools > 0 ) {	
seq_choose_integer_mv	f(1)
if ( seq_choose_integer_mv ) {	
seq_force_integer_mv = SELECT_INTEGER_MV	
} else {	
seq_force_integer_mv	f(1)
}	
} else {	
seq_force_integer_mv = SELECT_INTEGER_MV	
}	
}	
}

sequence_transform_quant_entropy_config() {
enable_fsc	f(1)
if ( enable_fsc ) {	
enable_idtx_intra = 1	
} else {	
enable_idtx_intra	f(1)
}	
enable_intra_ist	f(1)
enable_inter_ist	f(1)
if ( Monochrome ) {	
enable_chroma_dctonly = 0	
} else {	
enable_chroma_dctonly	f(1)
}	
if ( !single_picture_header_flag ) {	
enable_inter_ddt	f(1)
}	
reduced_tx_part_set	f(1)
if ( Monochrome ) {	
enable_cctx = 0	
} else {	
enable_cctx	f(1)
}	
enable_tcq	f(1)
if ( enable_tcq && !single_picture_header_flag ) {	
choose_tcq_per_frame	f(1)
} else {	
choose_tcq_per_frame = 0	
}	
if ( enable_tcq && !choose_tcq_per_frame ) {	
enable_parity_hiding = 0	
} else {	
enable_parity_hiding	f(1)
}	
if ( single_picture_header_flag ) {	
enable_avg_cdf = 1	
avg_cdf_type = 1	
} else {	
enable_avg_cdf	f(1)
if ( enable_avg_cdf ) {	
avg_cdf_type	f(1)
}	
}	
if ( Monochrome ) {	
separate_uv_delta_q = 0	
} else {	
separate_uv_delta_q	f(1)
}	
BaseYDcDeltaQ = 0	
BaseUVDcDeltaQ = 0	
BaseUVAcDeltaQ = 0	
y_dc_delta_q_enabled = 0	
uv_dc_delta_q_enabled = 0	
uv_ac_delta_q_enabled = 0	
equal_ac_dc_q	f(1)
if ( !equal_ac_dc_q ) {	
base_y_dc_delta_q	f(5)
BaseYDcDeltaQ = DELTA_DCQUANT_MIN + base_y_dc_delta_q	
y_dc_delta_q_enabled	f(1)
}	
if ( !Monochrome ) {	
if ( !equal_ac_dc_q ) {	
base_uv_dc_delta_q	f(5)
BaseUVDcDeltaQ = DELTA_DCQUANT_MIN + base_uv_dc_delta_q	
uv_dc_delta_q_enabled	f(1)
}	
base_uv_ac_delta_q	f(5)
BaseUVAcDeltaQ = DELTA_DCQUANT_MIN + base_uv_ac_delta_q	
uv_ac_delta_q_enabled	f(1)
if ( equal_ac_dc_q ) {	
BaseUVDcDeltaQ = BaseUVAcDeltaQ	
}	
}	
}

seg_info( numSegments ) {
for ( i = 0; i < numSegments; i++ ) {	
for ( j = 0; j < SEG_LVL_MAX; j++ ) {	
feature_enabled	f(1)
enabled[ i ][ j ] = feature_enabled	
clippedValue = 0	
if ( feature_enabled == 1 ) {	
bitsToRead = Segmentation_Feature_Bits[ j ]	
limit = Segmentation_Feature_Max[ j ]	
if ( Segmentation_Feature_Signed[ j ] == 1 ) {	
n = 1 + bitsToRead	
feature_value	su(n)
clippedValue = Clip3( -limit, limit, feature_value)	
} else {	
feature_value	f(bitsToRead)
clippedValue = Clip3( 0, limit, feature_value)	
}	
}	
data[ i ][ j ] = clippedValue	
}	
}	
return (enabled, data)	
}

sequence_filter_config() {
disable_loopfilters_across_tiles	f(1)
enable_cdef	f(1)
enable_gdf	f(1)
if ( enable_gdf && get_seq_sb_size() == BLOCK_64X64 ) {	
gdf_unit_matches_sb_size	f(1)
} else {	
gdf_unit_matches_sb_size = 0	
}	
enable_restoration	f(1)
if ( enable_restoration ) {	
lr_tools_disable[ 0 ][ RESTORE_PC_WIENER ]	f(1)
lr_tools_disable[ 0 ][ RESTORE_WIENER_NONSEP ]	f(1)
lr_tools_disable[ 1 ][ RESTORE_PC_WIENER ] = 1	
lr_tools_uv_present	f(1)
if ( lr_tools_uv_present ) {	
lr_tools_disable[ 1 ][ RESTORE_WIENER_NONSEP ]	f(1)
} else {	
lr_tools_disable[ 1 ][ RESTORE_WIENER_NONSEP ] =	
lr_tools_disable[ 0 ][ RESTORE_WIENER_NONSEP ]	
}	
}	
enable_ccso	f(1)
if ( enable_ccso ) {	
ccso_unit_matches_sb_size	f(1)
} else {	
ccso_unit_matches_sb_size = 0	
}	
if ( single_picture_header_flag ) {	
CdefOnSkipTxfm = CDEF_ON_SKIP_TXFM_ADAPTIVE	
} else {	
cdef_on_skip_txfm_always_on	f(1)
if (cdef_on_skip_txfm_always_on) {	
CdefOnSkipTxfm = CDEF_ON_SKIP_TXFM_ALWAYS_ON	
} else {	
cdef_on_skip_txfm_disabled	f(1)
CdefOnSkipTxfm = cdef_on_skip_txfm_disabled ?	
CDEF_ON_SKIP_TXFM_DISABLED : CDEF_ON_SKIP_TXFM_ADAPTIVE	
}	
}	
df_par_bits_minus_2	f(2)
}

user_defined_qm( level, t, plane ) {
txSz = Fundamental_Tx_Size[ t ]	
w = Tx_Width[ txSz ]	
h = Tx_Height[ txSz ]	
if ( plane > 0 ) {	
qm_copy_from_previous_plane	f(1)
if ( qm_copy_from_previous_plane ) {	
for ( i = 0; i < h; i++ ) {	
for ( j = 0; j < w; j++ ) {	
UserQm[ level ][ t ][ plane ][ i ][ j ] =	
UserQm[ level ][ t ][ plane - 1 ][ i ][ j ]	
}	
}	
return	
}	
}	
if ( t == 0 ) {	
qm_8x8_is_symmetric	f(1)
} else if ( t == 2 ) {	
qm_4x8_is_transpose_of_8x4	f(1)
if ( qm_4x8_is_transpose_of_8x4 ) {	
for ( i = 0; i < h; i++ ) {	
for ( j = 0; j < w; j++ ) {	
UserQm[ level ][ t ][ plane ][ i ][ j ] =	
UserQm[ level ][ 1 ][ plane ][ j ][ i ]	
}	
}	
return	
}	
}	
scan = get_scan( txSz, TX_CLASS_2D )	
quant = 32	
coefRepeat = 0	
for ( c = 0; c < w * h; c++ ) {	
pos = scan[ c ]	
(row, col) = get_tx_row_col(pos, txSz)	
if ( t == 0 && qm_8x8_is_symmetric && col > row ) {	
quant = UserQm[ level ][ t ][ plane ][ col ][ row ]	
UserQm[ level ][ t ][ plane ][ row ][ col ] = quant	
} else if ( coefRepeat ) {	
UserQm[ level ][ t ][ plane ][ row ][ col ] = quant	
} else {	
quant_delta	svlc()
quant2 = (quant + quant_delta) & 255	
if ( quant2 == 0 ) {	
coefRepeat = 1	
} else {	
quant = quant2	
}	
UserQm[ level ][ t ][ plane ][ row ][ col ] = quant	
}	
}	
}

timing_info() {
num_units_in_display_tick	f(32)
time_scale	f(32)
equal_picture_interval	f(1)
if ( equal_picture_interval ) {	
num_ticks_per_picture_minus_1	uvlc()
}	
}

seq_decoder_model_info() {
decoder_buffer_delay	uvlc()
encoder_buffer_delay	uvlc()
low_delay_mode_flag	f(1)
}

temporal_delimiter_obu() {
SeenFrameHeader = 0	
for ( level = 0; level < 15; level++ ) {	
QmProtected[ level ] = 0	
}	
}

multistream_decoder_operation_obu() {
num_streams_minus_2	f(3)
multistream_profile_idc	f(5)
multistream_level_idx	f(5)
multistream_tier	f(1)
multistream_even_allocation_flag	f(1)
if ( !multistream_even_allocation_flag ) {	
multistream_large_picture_idc	f(3)
}	
for ( i = 0; i < num_streams_minus_2 + 2; i++ ) {	
sub_xlayer_id[ i ]	f(5)
sub_stream_max_profile[ i ]	f(5)
sub_stream_max_level[ i ]	f(5)
sub_stream_max_tier[ i ]	f(1)
}	
multistream_doh_constraint_flag	f(1)
}

multi_frame_header_obu() {
mfh_seq_header_id	uvlc()
mfh_id_minus_1	uvlc()
mfhId = mfh_id_minus_1 + 1	
MfhSeqHeaderId[ mfhId ] = mfh_seq_header_id	
MfhTLayerId[ mfhId ] = obu_tlayer_id	
MfhMLayerId[ mfhId ] = obu_mlayer_id	
mfh_frame_size_present_flag[ mfhId ]	f(1)
if ( mfh_frame_size_present_flag[ mfhId ] ) {	
mfh_frame_width_bits_minus_1	f(4)
mfh_frame_height_bits_minus_1	f(4)
n = mfh_frame_width_bits_minus_1 + 1	
mfh_frame_width_minus_1[ mfhId ]	f(n)
n = mfh_frame_height_bits_minus_1 + 1	
mfh_frame_height_minus_1[ mfhId ]	f(n)
}	
mfh_deblocking_filter_update[ mfhId ]	f(1)
if ( mfh_deblocking_filter_update[ mfhId ] ) {	
for ( i = 0; i < 4; i++ ) {	
mfh_apply_deblocking_filter[ mfhId ][ i ]	f(1)
}	
}	
mfh_seg_info_present_flag[ mfhId ]	f(1)
if ( mfh_seg_info_present_flag[ mfhId ] ) {	
mfh_ext_seg_flag[ mfhId ]	f(1)
mfh_allow_seg_info_change[ mfhId ]	f(1)
( MfhFeatureEnabled[mfhId], MfhFeatureData[mfhId] ) =	
seg_info( mfh_ext_seg_flag[ mfhId ] ? 16 : 8 )	
}	
}

layer_config_record_obu() {
if ( obu_xlayer_id == GLOBAL_XLAYER_ID ) {	
lcr_global_info()	
} else {	
lcr_local_info( obu_xlayer_id )	
}	
}

lcr_global_info() {
lcr_global_config_record_id	f(3)
lcr_xlayer_map	f(31)
LcrMaxNumXLayerCount = 0	
for ( i = 0; i < 31; i++ ) {	
if ( lcr_xlayer_map & ( 1 << i ) ) {	
LcrXLayerID[ LcrMaxNumXLayerCount ] = i	
LcrMaxNumXLayerCount ++	
}	
}	
lcr_aggregate_info_present_flag	f(1)
lcr_seq_profile_tier_level_info_present_flag	f(1)
lcr_global_payload_present_flag	f(1)
lcr_dependent_xlayers_flag	f(1)
lcr_global_atlas_id_present_flag	f(1)
lcr_global_purpose_id	f(7)
lcr_doh_constraint_flag	f(1)
lcr_enforce_tile_alignment_flag	f(1)
if ( lcr_global_atlas_id_present_flag ) {	
lcr_global_atlas_id	f(3)
} else {	
lcr_global_reserved_zero_3bits	f(3)
}	
lcr_global_reserved_zero_5bits	f(5)
if ( lcr_aggregate_info_present_flag ) {	
lcr_aggregate_info()	
}	
if ( lcr_seq_profile_tier_level_info_present_flag ) {	
for ( i = 0; i < LcrMaxNumXLayerCount; i++ ) {	
lcr_seq_profile_tier_level_info( LcrXLayerID[ i ] )	
}	
}	
if ( lcr_global_payload_present_flag ) {	
for ( i = 0; i < LcrMaxNumXLayerCount; i++) {	
lcr_data_size [ i ]	leb128()
lcr_global_payload( LcrXLayerID[ i ], lcr_data_size [ i ] )	
}	
}	
}

lcr_local_info( xlayerId ) {
lcr_global_id[ xlayerId ]	f(3)
lcr_local_id[ xlayerId ]	f(3)
lcr_profile_tier_level_info_present_flag[ xlayerId ]	f(1)
lcr_local_atlas_id_present_flag[ xlayerId ]	f(1)
if ( lcr_profile_tier_level_info_present_flag[ xlayerId ] ) {	
lcr_seq_profile_tier_level_info( xlayerId )	
}	
if ( lcr_local_atlas_id_present_flag[ xlayerId ] ) {	
lcr_local_atlas_id[ xlayerId ]	f(3)
} else {	
lcr_local_reserved_zero_3bits[ xlayerId ]	f(3)
}	
lcr_local_reserved_zero_5bits[ xlayerId ]	f(5)
lcr_xlayer_info( 0, xlayerId )	
}

lcr_aggregate_info() {
lcr_config_idc	f(6)
lcr_aggregate_level_idx	f(5)
lcr_max_tier_flag	f(1)
lcr_max_interop	f(4)
}

lcr_seq_profile_tier_level_info( i ) {
lcr_seq_profile_idc[ i ]	f(5)
lcr_max_level_idx[ i ]	f(5)
lcr_tier_flag[ i ]	f(1)
lcr_max_mlayer_count[ i ]	f(3)
lsptli_reserved_2bits	f(2)
}

lcr_global_payload( n, sz ) {
startPosition = get_position()	
if ( lcr_dependent_xlayers_flag && n > 0 ) {	
lcr_num_dependent_xlayer_map[ n ]	f(n)
}	
lcr_xlayer_info( 1 , n )	
currentPosition = get_position()	
parsedPayloadBits = currentPosition - startPosition	
RemainingLcrPayloadBits = sz * 8 - parsedPayloadBits	
for ( j = 0; j < RemainingLcrPayloadBits; j++ ) {	
lcr_remaining_payload_bit	f(1)
}	
}

lcr_xlayer_info( isGlobal, xId ) {
lcr_rep_info_present_flag[ isGlobal ][ xId ]	f(1)
lcr_xlayer_purpose_present_flag[ isGlobal ][ xId ]	f(1)
lcr_xlayer_color_info_present_flag[ isGlobal ][ xId ]	f(1)
lcr_embedded_layer_info_present_flag[ isGlobal ][ xId ]	f(1)
if ( lcr_rep_info_present_flag[ isGlobal ][ xId ] ) {	
lcr_rep_info( isGlobal, xId )	
}	
if( lcr_xlayer_purpose_present_flag[ isGlobal ][ xId ] ) {	
lcr_xlayer_purpose_id[ isGlobal ][ xId ]	f(7)
}	
if( lcr_xlayer_color_info_present_flag[ isGlobal ][ xId ] ) {	
lcr_xlayer_color_info( isGlobal, xId )	
}	
byte_alignment()	
if ( lcr_embedded_layer_info_present_flag[ isGlobal ][ xId ] ) {	
lcr_embedded_layer_info( isGlobal, xId )	
} else {	
if ( isGlobal && lcr_global_atlas_id_present_flag ) {	
lcr_xlayer_atlas_segment_id[ xId ]	f(8)
lcr_xlayer_priority_order[ xId ]	f(8)
lcr_xlayer_rendering_method[ xId ]	f(8)
}	
}	
}

lcr_rep_info( isGlobal, xId ) {
lcr_max_pic_width[ isGlobal ][ xId ]	uvlc()
lcr_max_pic_height[ isGlobal ][ xId ]	uvlc()
lcr_format_info_present_flag[ isGlobal ][ xId ]	f(1)
lcr_cropping_window_present_flag[ isGlobal ][ xId ]	f(1)
if ( lcr_format_info_present_flag[ isGlobal ][ xId ] ) {	
lcr_bit_depth_idc[ isGlobal ][ xId ]	uvlc()
lcr_chroma_format_idc[ isGlobal ][ xId ]	uvlc()
}	
if ( lcr_cropping_window_present_flag[ isGlobal ][ xId ] ) {	
lcr_cropping_win_left_offset [ isGlobal ][ xId ]	uvlc()
lcr_cropping_win_right_offset[ isGlobal ][ xId ]	uvlc()
lcr_cropping_win_top_offset [ isGlobal ][ xId ]	uvlc()
lcr_cropping_win_bottom_offset[ isGlobal ][ xId ]	uvlc()
}	
}

lcr_embedded_layer_info( isGlobal, xId ) {
lcr_mlayer_map[ isGlobal ][ xId ]	f(8)
for ( j = 0; j < 8; j++ ) {	
if ( lcr_mlayer_map[ isGlobal ][ xId ] & (1 << j) ) {	
n = MAX_NUM_TLAYERS	
lcr_tlayer_map[ isGlobal ][ xId ][ j ]	f(n)
atlasSegmentPresent = isGlobal ?	
lcr_global_atlas_id_present_flag :	
lcr_local_atlas_id_present_flag[ xId ]	
if ( atlasSegmentPresent ) {	
lcr_layer_atlas_segment_id[ isGlobal ][ xId ][ j ]	f(8)
lcr_priority_order[ isGlobal ][ xId ][ j ]	f(8)
lcr_rendering_method[ isGlobal ][ xId ][ j ]	f(8)
}	
lcr_layer_type[ isGlobal ][ xId ][ j ]	f(8)
if ( lcr_layer_type[ isGlobal ][ xId ][ j ] == AUX_LAYER ) {	
lcr_auxiliary_type[ isGlobal ][ xId ][ j ]	f(8)
}	
lcr_view_type[ isGlobal ][ xId ][ j ]	f(8)
if ( lcr_view_type[ isGlobal ][ xId ][ j ] == VIEW_EXPLICIT ) {	
lcr_view_id[ isGlobal ][ xId ][ j ]	f(8)
}	
if ( j > 0 ) {	
lcr_dependent_layer_map[ isGlobal ][ xId ][ j ]	f(j)
}	
lcr_same_sh_max_resolution_flag[ isGlobal ][ xId ][ j ]	f(1)
if ( !lcr_same_sh_max_resolution_flag[ isGlobal ][ xId ][ j ] ) {	
lcr_max_expected_width[ isGlobal ][ xId ][ j ]	uvlc()
lcr_max_expected_height[ isGlobal ][ xId ][ j ]	uvlc()
}	
byte_alignment()	
}	
}	
}

lcr_xlayer_color_info( isGlobal, xId ) {
layer_color_description_idc[ isGlobal ][ xId ]	rg(2)
if ( layer_color_description_idc[ isGlobal ][ xId ] == 0 ) {	
layer_color_primaries[ isGlobal ][ xId ]	f(8)
layer_transfer_characteristics[ isGlobal ][ xId ]	f(8)
layer_matrix_coefficients[ isGlobal ][ xId ]	f(8)
}	
layer_full_range_flag[ isGlobal ][ xId ]	f(1)
}

atlas_segment_info_obu() {
atlas_segment_id[ obu_xlayer_id ]	f(3)
xAId = atlas_segment_id[ obu_xlayer_id ]	
ats_atlas_segment_mode_idc[ xAId ]	uvlc()
if ( ats_atlas_segment_mode_idc[ xAId ] == ENHANCED_ATLAS ) {	
numSegments = ats_enhanced_atlas_info( xAId )	
} else if ( ats_atlas_segment_mode_idc[ xAId ] == BASIC_ATLAS ) {	
numSegments = ats_basic_info( xAId )	
} else if ( ats_atlas_segment_mode_idc[ xAId ] == SINGLE_ATLAS ) {	
numSegments = 1	
ats_nominal_width_minus_1[ xAId ]	uvlc()
ats_nominal_height_minus_1[ xAId ]	uvlc()
} else if ( ats_atlas_segment_mode_idc[ xAId ] == MULTISTREAM_ATLAS ) {	
numSegments = ats_multistream_info( obu_xlayer_id, xAId )	
} else if ( ats_atlas_segment_mode_idc[ xAId ] ==	
MULTISTREAM_ALPHA_ATLAS ) {	
numSegments = ats_multistream_with_alpha_info( obu_xlayer_id, xAId )	
}	
ats_label_segment_info( obu_xlayer_id, xAId, numSegments )	
}

ats_label_segment_info( xlayerId, xAId, numSegments ) {
ats_signaled_atlas_segment_ids_flag[ xlayerId ][ xAId ]	f(1)
if ( ats_signaled_atlas_segment_ids_flag[ xlayerId ][ xAId ] ) {	
for ( i = 0;i < numSegments; i++ ) {	
ats_atlas_segment_id[ xlayerId ][ xAId ][ i ]	f(8)
AtlasSegmentIDToIndex[ xlayerId ][ xAId ]	
[ ats_atlas_segment_id[ xlayerId ][ xAId ][ i ] ] = i	
AtlasSegmentIndexToID[ xlayerId ][ xAId ][ i ] =	
ats_atlas_segment_id[ xlayerId ][ xAId ][ i ]	
}	
} else {	
for ( i = 0;i < numSegments; i++ ) {	
ats_atlas_segment_id[ xlayerId ][ xAId ][ i ] = i	
AtlasSegmentIDToIndex[ xlayerId ][ xAId ][ i ] = i	
AtlasSegmentIndexToID[ xlayerId ][ xAId ][ i ] = i	
}	
}	
}

ats_enhanced_atlas_info( xAId ) {
ats_region_info( xAId )	
numSegments = ats_region_to_segment_mapping( xAId )	
return numSegments	
}

ats_region_info( xAId ) {
ats_num_region_columns_minus_1[ xAId ]	uvlc()
ats_num_region_rows_minus_1[ xAId ]	uvlc()
ats_uniform_spacing_flag[ xAId ]	f(1)
AtlasWidth = 0	
AtlasHeight = 0	
if ( !ats_uniform_spacing_flag[ xAId ] ) {	
for ( i = 0; i < ats_num_region_columns_minus_1[ xAId ] + 1;	
i++ ) {	
ats_column_width_minus_1[ xAId ][ i ]	uvlc()
AtlasWidth += (ats_column_width_minus_1[ xAId ][ i ] + 1)	
}	
for ( i = 0;i < ats_num_region_rows_minus_1[xAId] + 1; i++ ) {	
ats_row_height_minus_1[ xAId ][ i ]	uvlc()
AtlasHeight += (ats_row_height_minus_1[ xAId ][ i ] + 1)	
}	
} else {	
ats_region_width_minus_1[ xAId ]	uvlc()
ats_region_height_minus_1[ xAId ]	uvlc()
AtlasWidth =	
( ats_region_width_minus_1[ xAId ] + 1 ) *	( ats_num_region_columns_minus_1[ xAId ] + 1 )	
AtlasHeight =	
( ats_region_height_minus_1[ xAId ] + 1 ) *	( ats_num_region_rows_minus_1[ xAId ] + 1 )	
}	
NumRegionsInAtlas[ xAId ] =	
( ats_num_region_columns_minus_1[ xAId ] + 1) *	( ats_num_region_rows_minus_1[ xAId ] + 1 )	
}

ats_region_to_segment_mapping( xAId ) {
ats_single_region_per_atlas_segment_flag[ xAId ]	f(1)
if ( !ats_single_region_per_atlas_segment_flag[ xAId ] ) {	
ats_num_atlas_segments_minus_1[ xAId ]	uvlc()
for ( i = 0; i <= ats_num_atlas_segments_minus_1[ xAId ]; i++ ) {	
ats_top_left_region_column[ xAId ][ i ]	uvlc()
ats_top_left_region_row[ xAId ][ i ]	uvlc()
ats_bottom_right_region_column_off[ xAId ][ i ]	uvlc()
ats_bottom_right_region_row_off[ xAId ][ i ]	uvlc()
}	
} else {	
ats_num_atlas_segments_minus_1[ xAId ] =	
NumRegionsInAtlas[ xAId ] - 1	
}	
return ats_num_atlas_segments_minus_1[ xAId ] + 1	
}

ats_multistream_info( xlayerId, xAId ) {
ats_msi_width[ xlayerId ][ xAId ]	uvlc()
ats_msi_height[ xlayerId ][ xAId ]	uvlc()
AtlasWidth = ats_msi_width[ xlayerId ][ xAId ]	
AtlasHeight = ats_msi_height[ xlayerId ][ xAId ]	
ats_msi_num_atlas_segments_minus_1[ xlayerId ][ xAId ]	uvlc()
ats_msi_background_info_present_flag[ xlayerId ][ xAId ]	f(1)
if ( ats_msi_background_info_present_flag[ xlayerId ][ xAId ] ) {	
ats_msi_background_red_value[ xlayerId ][ xAId ]	f(8)
ats_msi_background_green_value[ xlayerId ][ xAId ]	f(8)
ats_msi_background_blue_value[ xlayerId ][ xAId ]	f(8)
}	
for (i=0;i<=ats_msi_num_atlas_segments_minus_1[ xlayerId ][ xAId ];i++) {	
ats_msi_input_stream_id[ xlayerId ][ xAId ][ i ]	f(5)
ats_msi_segment_top_left_pos_x[ xlayerId ][ xAId ][ i ]	uvlc()
ats_msi_segment_top_left_pos_y[ xlayerId ][ xAId ][ i ]	uvlc()
ats_msi_segment_width[ xlayerId ][ xAId ][ i ]	uvlc()
ats_msi_segment_height[ xlayerId ][ xAId ][ i ]	uvlc()
}	
return ats_msi_num_atlas_segments_minus_1[ xlayerId ][ xAId ] + 1	
}

ats_multistream_with_alpha_info( xlayerId, xAId ) {
ats_msi_width[ xlayerId ][ xAId ]	uvlc()
ats_msi_height[ xlayerId ][ xAId ]	uvlc()
AtlasWidth = ats_msi_width[ xlayerId ][ xAId ]	
AtlasHeight = ats_msi_height[ xlayerId ][ xAId ]	
ats_msi_num_atlas_segments_minus_1[ xlayerId ][ xAId ]	uvlc()
ats_msi_alpha_segments_present_flag[ xlayerId ][ xAId ]	f(1)
ats_msi_background_info_present_flag[ xlayerId ][ xAId ]	f(1)
if ( ats_msi_background_info_present_flag[ xlayerId ][ xAId ] ) {	
ats_msi_background_red_value[ xlayerId ][ xAId ]	f(8)
ats_msi_background_green_value[ xlayerId ][ xAId ]	f(8)
ats_msi_background_blue_value[ xlayerId ][ xAId ]	f(8)
}	
for (i=0;i<=ats_msi_num_atlas_segments_minus_1[ xlayerId ][ xAId ];i++) {	
ats_msi_input_stream_id[ xlayerId ][ xAId ][ i ]	f(5)
ats_msi_segment_top_left_pos_x[ xlayerId ][ xAId ][ i ]	uvlc()
ats_msi_segment_top_left_pos_y[ xlayerId ][ xAId ][ i ]	uvlc()
ats_msi_segment_width[ xlayerId ][ xAId ][ i ]	uvlc()
ats_msi_segment_height[ xlayerId ][ xAId ][ i ]	uvlc()
if ( ats_msi_alpha_segments_present_flag[ xlayerId ][ xAId ] &&	
i != ats_msi_num_atlas_segments_minus_1[ xlayerId ][ xAId ] ) {	
ats_msi_alpha_segment_flag[ xlayerId ][ xAId ][ i ]	f(1)
} else {	
ats_msi_alpha_segment_flag[ xlayerId ][ xAId ][ i ] = 0	
}	
}	
return ats_msi_num_atlas_segments_minus_1[ xlayerId ][ xAId ] + 1	
}

ats_basic_info( xAId ) {
ats_stream_id_present[ xAId ]	f(1)
ats_width[ xAId ]	uvlc()
ats_height[ xAId ]	uvlc()
ats_num_atlas_segments_minus_1[ xAId ]	uvlc()
AtlasWidth = ats_width[ xAId ]	
AtlasHeight = ats_height[ xAId ]	
for ( i = 0; i <= ats_num_atlas_segments_minus_1[ xAId ]; i++ ) {	
if (ats_stream_id_present[ xAId ]) {	
ats_input_stream_id[ xAId ][ i ]	f(5)
}	
ats_segment_top_left_pos_x[ xAId ][ i ]	uvlc()
ats_segment_top_left_pos_y[ xAId ][ i ]	uvlc()
ats_segment_width[ xAId ][ i ]	uvlc()
ats_segment_height[ xAId ][ i ]	uvlc()
}	
return ats_num_atlas_segments_minus_1[ xAId ] + 1	
}

operating_point_set_obu() {
ops_reset_flag[ obu_xlayer_id ]	f(1)
ops_id[ obu_xlayer_id ]	f(4)
opsID = ops_id[ obu_xlayer_id ]	
ops_cnt[ obu_xlayer_id ][ opsID ]	f(3)
if ( ops_cnt[ obu_xlayer_id ][ opsID ] > 0 ) {	
ops_priority[ obu_xlayer_id ][ opsID ]	f(4)
ops_intent[ obu_xlayer_id ][ opsID ]	f(7)
ops_intent_present_flag[ obu_xlayer_id ][ opsID ]	f(1)
ops_ptl_present_flag[ obu_xlayer_id ][ opsID ]	f(1)
ops_color_info_present_flag[ obu_xlayer_id ][ opsID ]	f(1)
if ( obu_xlayer_id == GLOBAL_XLAYER_ID ) {	
ops_mlayer_info_idc[ opsID ]	f(2)
} else {	
ops_reserved_2bits	f(2)
}	
for( i = 0; i < ops_cnt[ obu_xlayer_id ][ opsID ]; i++ ) {	
operating_point_payload( obu_xlayer_id, opsID, i )	
}	
}	
}

operating_point_payload( xId, opsID, i ) {
ops_data_size[ xId ][ opsID ][ i ]	leb128()
startPos = get_position()	
if ( ops_intent_present_flag[ xId ][ opsID ] ) {	
ops_op_intent[ xId ][ opsID ][ i ]	f(7)
}	
if ( ops_ptl_present_flag[ xId ][ opsID ] ) {	
if ( xId == GLOBAL_XLAYER_ID ) {	
ops_aggregate_info( opsID, i )	
} else {	
ops_seq_profile_tier_level_info( xId, opsID, i, xId )	
}	
}	
if ( ops_color_info_present_flag[ xId ][ opsID ] ) {	
ops_color_info( opsID, i )	
}	
ops_decoder_model_info_for_this_op_present_flag[ xId ][ opsID ][ i ]	f(1)
if ( ops_decoder_model_info_for_this_op_present_flag[ xId ][ opsID ][ i ] ) {	
ops_decoder_model_info( opsID, i )	
}	
ops_initial_display_delay_present_flag[ xId ][ opsID ][ i ]	f(1)
if ( ops_initial_display_delay_present_flag[ xId ][ opsID ][ i ] ) {	
ops_initial_display_delay_minus_1[ xId ][ opsID ][ i ]	f(4)
}	
if ( xId == GLOBAL_XLAYER_ID ) {	
ops_xlayer_map[ opsID ][ i ]	f(31)
k = 0	
for ( j = 0; j < 31; j++ ) {	
if ( ops_xlayer_map[ opsID ][ i ] & (1 << j) ) {	
OpsxLayerId[ xId ][ opsID ][ i ][ k ] = j	
k++	
if ( ops_ptl_present_flag[ xId ][ opsID ] ) {	
ops_seq_profile_tier_level_info( xId, opsID, i, j )	
}	
idc = ops_mlayer_info_idc[ opsID ]	
if ( idc == 1 ) {	
ops_mlayer_info( xId, opsID, i, j )	
} else if ( idc == 2 ) {	
ops_mlayer_explicit_info_flag[ opsID ][ i ][ j ]	f(1)
if ( ops_mlayer_explicit_info_flag[ opsID ][ i ][ j ] ) {	
ops_mlayer_info( xId, opsID, i, j )	
} else {	
ops_embedded_ops_id[ opsID ][ i ][ j ]	f(4)
ops_embedded_op_index[ opsID ][ i ][ j ]	f(3)
}	
}	
}	
}	
XCount[ xId ][ opsID ][ i ] = k	
} else {	
XCount[ xId ][ opsID ][ i ] = 1	
OpsxLayerId[ xId ][ opsID ][ i ][ 0 ] = xId	
ops_mlayer_info( xId, opsID, i, xId )	
}	
byte_alignment()	
opsBytes = (get_position() - startPos) >> 3	
}

ops_aggregate_info( opsID, i ) {
ops_config_idc[ opsID ][ i ]	f(6)
ops_aggregate_level_idx[ opsID ][ i ]	f(5)
ops_max_tier_flag[ opsID ][ i ]	f(1)
ops_max_interop[ opsID ][ i ]	f(4)
}

ops_seq_profile_tier_level_info( xId, opsID, i, j ) {
ops_seq_profile_idc[ xId ][ opsID ][ i ][ j ]	f(5)
ops_level_idx[ xId ][ opsID ][ i ][ j ]	f(5)
ops_tier_flag[ xId ][ opsID ][ i ][ j ]	f(1)
ops_mlayer_count[ xId ][ opsID ][ i ][ j ]	f(3)
ops_ptl_reserved_2bits	f(2)
}

ops_decoder_model_info( opsID, i ) {
ops_decoder_buffer_delay[ obu_xlayer_id ][ opsID ][ i ]	uvlc()
ops_encoder_buffer_delay[ obu_xlayer_id ][ opsID ][ i ]	uvlc()
ops_low_delay_mode_flag[ obu_xlayer_id ][ opsID ][ i ]	f(1)
}

ops_color_info( opsID, i ) {
ops_color_description_idc[ obu_xlayer_id ][ opsID ][ i ]	rg(2)
if ( ops_color_description_idc[ obu_xlayer_id ][ opsID ][ i ] == 0 ) {	
ops_color_primaries[ obu_xlayer_id ][ opsID ][ i ]	f(8)
ops_transfer_characteristics[ obu_xlayer_id ][ opsID ][ i ]	f(8)
ops_matrix_coefficients[ obu_xlayer_id ][ opsID ][ i ]	f(8)
}	
ops_full_range_flag[ obu_xlayer_id ][ opsID ][ i ]	f(1)
}

ops_mlayer_info( obuXLId, opsID, opIndex, xLId ) {
ops_mlayer_map[ obuXLId ][ opsID ][ opIndex ][ xLId ]	f(8)
mCount = 0	
for ( j = 0; j < 8; j++ ) {	
if (ops_mlayer_map[ obuXLId ][ opsID ][ opIndex ][ xLId ] & (1 << j)) {	
ops_tlayer_map[ obuXLId ][ opsID ][ opIndex ][ xLId ][ j ]	f(4)
tCount = 0	
for ( k = 0; k < 4; k++ ) {	
if ( ops_tlayer_map[ obuXLId ][ opsID ][ opIndex ][ xLId ][ j ]	
& (1 << k) ) {	
tCount++	
}	
}	
mCount++	
}	
}	
}

buffer_removal_timing_obu() {
br_ops_dependent_flag	f(1)
if ( br_ops_dependent_flag ) {	
br_ops_id	f(4)
br_ops_cnt[ br_ops_id ]	f(3)
for ( i = 0; i < br_ops_cnt[ br_ops_id ]; i++ ) {	
br_decoder_model_present_op_flag[ br_ops_id ][ i ]	f(1)
if ( br_decoder_model_present_op_flag[ br_ops_id ][ i ] ) {	
br_time_op[ br_ops_id ][ i ]	rg(4)
}	
}	
} else {	
br_time	rg(4)
}	
}

quantizer_matrix_obu() {
qm_bit_map	f(15)
qm_chroma_info_present_flag	f(1)
numPlanes = qm_chroma_info_present_flag ? 3 : 1	
if ( qm_bit_map == 0 ){	
for ( level = 0; level < NUM_CUSTOM_QMS; level++ ) {	
QmProtected[ level ] = 1	
QmNumPlanes[ level ] = numPlanes	
QmDataPresent[ level ] = 0	
QmMLayerId[ level ] = -1	
QmTLayerId[ level ] = -1	
}	
} else {	
for ( level = 0; level < 15; level++ ) {	
if ( qm_bit_map & (1 << level) ) {	
QmSeen[ level ] = 1	
QmProtected[ level ] = 1	
QmNumPlanes[ level ] = numPlanes	
QmMLayerId[ level ] = obu_mlayer_id	
QmTLayerId[ level ] = obu_tlayer_id	
QmDataPresent[ level ] = 1	
qm_is_default_flag	f(1)
if ( qm_is_default_flag ) {	
QmDataPresent[ level ] = 0	
} else {	
for ( t = 0; t < 3; t++ ){	
for ( plane = 0; plane < numPlanes; plane++ ) {	
user_defined_qm( level, t, plane )	
}	
}	
}	
}	
}	
}	
}

film_grain_obu() {
fgm_update_flags	f(8)
fgm_chroma_idc	uvlc()
if ( fgm_chroma_idc == CHROMA_FORMAT_420 ) {	
subX = 1	
subY = 1	
} else if ( fgm_chroma_idc == CHROMA_FORMAT_444 ) {	
subX = 0	
subY = 0	
} else if ( fgm_chroma_idc == CHROMA_FORMAT_422 ) {	
subX = 1	
subY = 0	
} else if ( fgm_chroma_idc == CHROMA_FORMAT_400 ) {	
subX = 1	
subY = 1	
}	
monochrome = fgm_chroma_idc == CHROMA_FORMAT_400	
for ( i = 0; i < MAX_FILM_GRAIN; i++ ) {	
if ( fgm_update_flags & (1 << i) ) {	
FilmGrainPresent[ i ] = 1	
film_grain_model( monochrome, subX, subY)	
save_grain_model( i )	
FgmTLayerId[ i ] = obu_tlayer_id	
FgmMLayerId[ i ] = obu_mlayer_id	
FgmChromaIdc[ i ] = fgm_chroma_idc	
}	
}	
}

content_interpretation_obu() {
ci_scan_type_idc	f(2)
ci_color_description_present_flag	f(1)
ci_chroma_sample_position_present_flag	f(1)
ci_aspect_ratio_info_present_flag	f(1)
ci_timing_info_present_flag	f(1)
ci_reserved_2bit	f(2)
ci_color_primaries = CP_UNSPECIFIED	
ci_transfer_characteristics = TC_UNSPECIFIED	
ci_matrix_coefficients = MC_UNSPECIFIED	
ci_full_range_flag = 0	
if ( ci_color_description_present_flag ) {	
ci_color_description_idc	rg(2)
if ( ci_color_description_idc == 0 ) {	
ci_color_primaries	f(8)
ci_transfer_characteristics	f(8)
ci_matrix_coefficients	f(8)
}	
ci_full_range_flag	f(1)
}	
if ( ci_chroma_sample_position_present_flag ) {	
ci_chroma_sample_position_top	uvlc()
if ( ci_scan_type_idc != 1 ) {	
ci_chroma_sample_position_bottom	uvlc()
} else {	
ci_chroma_sample_position_bottom = ci_chroma_sample_position_top	
}	
} else {	
ci_chroma_sample_position_top = CSP_UNSPECIFIED	
ci_chroma_sample_position_bottom = CSP_UNSPECIFIED	
}	
if ( ci_aspect_ratio_info_present_flag ) {	
ci_aspect_ratio_idc	f(8)
if ( ci_aspect_ratio_idc == 255 ) {	
ci_sar_width	uvlc()
ci_sar_height	uvlc()
} else {	
ci_sar_width = Aspect_Ratio_Width[ ci_aspect_ratio_idc ]	
ci_sar_height = Aspect_Ratio_Height[ ci_aspect_ratio_idc ]	
}	
}	
if ( ci_timing_info_present_flag ) {	
timing_info()	
}	
}

padding_obu() {
for ( i = 0; i < obu_padding_length; i++ ) {	
obu_padding_byte	f(8)
}	
}

metadata_unit( metadataPayloadSize ) {
startPosition = get_position()	
if ( metadata_type == METADATA_TYPE_ITUT_T35 ) {	
metadata_itut_t35( metadataPayloadSize )	
} else if ( metadata_type == METADATA_TYPE_HDR_CLL ) {	
metadata_hdr_cll()	
} else if ( metadata_type == METADATA_TYPE_HDR_MDCV ) {	
metadata_hdr_mdcv()	
} else if ( metadata_type == METADATA_TYPE_TIMECODE ) {	
metadata_timecode()	
} else if ( metadata_type == METADATA_TYPE_BANDING_HINTS ) {	
metadata_banding_hints()	
} else if ( metadata_type == METADATA_TYPE_ICC_PROFILE ) {	
metadata_icc_profile( metadataPayloadSize )	
} else if ( metadata_type == METADATA_TYPE_SCAN_TYPE ) {	
metadata_scan_type()	
} else if ( metadata_type == METADATA_TYPE_TEMPORAL_POINT_INFO ) {	
metadata_temporal_point_info()	
} else if ( metadata_type == METADATA_TYPE_DECODED_FRAME_HASH ) {	
metadata_decoded_frame_hash()	
} else if ( metadata_type == METADATA_TYPE_USER_DATA_UNREGISTERED ) {	
metadata_user_data_unregistered( metadataPayloadSize )	
}	
currentPosition = get_position()	
parsedPayloadBits = currentPosition - startPosition	
remainingMuPayloadBits = metadataPayloadSize * 8 - parsedPayloadBits	
for ( j = 0; j < remainingMuPayloadBits; j++ ) {	
metadata_unit_remaining_bit	f(1)
}	
}

metadata_short_obu( obuPayloadSize ) {
metadata_is_suffix	f(1)
metadata_necessity_idc = 0	
metadata_application_id = 0	
muh_layer_idc	f(3)
muh_cancel_flag	f(1)
muh_persistence_idc	f(3)
muh_priority = 0	
metadata_type	leb128()
if ( muh_cancel_flag ) {	
return	
}	
metadataPayloadSize = obuPayloadSize - 2 - Leb128Bytes	
metadata_unit( metadataPayloadSize )	
}

metadata_group_obu() {
metadata_is_suffix	f(1)
metadata_necessity_idc	f(2)
metadata_application_id	f(5)
metadata_unit_cnt_minus_1	leb128()
for ( i = 0; i <= metadata_unit_cnt_minus_1; i++ ) {	
metadata_type	leb128()
muh_header_size	f(7)
muh_cancel_flag	f(1)
headerRemainingBytes = muh_header_size	
if ( !muh_cancel_flag ) {	
muh_payload_size	leb128()
headerRemainingBytes -= Leb128Bytes	
muh_layer_idc	f(3)
muh_persistence_idc	f(3)
muh_priority	f(8)
muh_reserved_zero_2bits	f(2)
headerRemainingBytes -= 2	
if ( muh_layer_idc == LAYER_VALUES ) {	
if ( obu_xlayer_id == GLOBAL_XLAYER_ID ) {	
muh_xlayer_map	f(32)
headerRemainingBytes -= 4	
for ( n = 0; n < 31; n++ ) {	
if ( muh_xlayer_map & (0x1 << n) ) {	
muh_mlayer_map	f(8)
headerRemainingBytes -= 1	
}	
}	
} else {	
muh_mlayer_map	f(8)
headerRemainingBytes -= 1	
}	
}	
}	
for ( j = 0; j < headerRemainingBytes; j++ ) {	
muh_header_extension_byte	f(8)
}	
if ( !muh_cancel_flag ) {	
metadata_unit( muh_payload_size )	
}	
}	
}

metadata_itut_t35( metadataPayloadSize ) {
itu_t_t35_country_code	f(8)
t35PayloadSize = metadataPayloadSize - 1	
if ( itu_t_t35_country_code == 0xFF ) {	
itu_t_t35_country_code_extension_byte	f(8)
t35PayloadSize--	
}	
itu_t_t35_payload_bytes	le(t35PayloadSize)
}

metadata_hdr_cll() {
max_cll	f(16)
max_fall	f(16)
}

metadata_hdr_mdcv() {
for ( i = 0; i < 3; i++ ) {	
primary_chromaticity_x[ i ]	f(16)
primary_chromaticity_y[ i ]	f(16)
}	
white_point_chromaticity_x	f(16)
white_point_chromaticity_y	f(16)
luminance_max	f(32)
luminance_min	f(32)
}

metadata_timecode() {
counting_type	f(5)
full_timestamp_flag	f(1)
discontinuity_flag	f(1)
cnt_dropped_flag	f(1)
n_frames	f(9)
if ( full_timestamp_flag ) {	
seconds_value	f(6)
minutes_value	f(6)
hours_value	f(5)
} else {	
seconds_flag	f(1)
if ( seconds_flag ) {	
seconds_value	f(6)
minutes_flag	f(1)
if ( minutes_flag ) {	
minutes_value	f(6)
hours_flag	f(1)
if ( hours_flag ) {	
hours_value	f(5)
}	
}	
}	
}	
time_offset_length	f(5)
if ( time_offset_length > 0 ) {	
time_offset_value	f(time_offset_length)
}	
}

metadata_banding_hints() {
coding_banding_present_flag	f(1)
source_banding_present_flag	f(1)
if ( coding_banding_present_flag ) {	
banding_hints_flag	f(1)
if ( banding_hints_flag ) {	
three_color_components_flag	f(1)
numComponents = three_color_components_flag ? 3 : 1	
for ( plane = 0; plane < numComponents; plane++ ) {	
banding_in_component_present_flag	f(1)
if ( banding_in_component_present_flag ) {	
max_band_width_minus_4	f(6)
max_band_step_minus_1	f(4)
}	
}	
band_units_information_present_flag	f(1)
if ( band_units_information_present_flag ) {	
num_band_units_rows_minus_1	f(5)
num_band_units_cols_minus_1	f(5)
varying_size_band_units_flag	f(1)
if ( varying_size_band_units_flag ) {	
band_block_in_luma_samples	f(3)
for ( r = 0; r <= num_band_units_rows_minus_1; r++ ) {	
vert_size_in_band_blocks_minus_1	f(5)
}	
for ( c = 0; c <= num_band_units_cols_minus_1; c++ ) {	
horz_size_in_band_blocks_minus_1	f(5)
}	
}	
for ( r = 0; r <= num_band_units_rows_minus_1; r++ ) {	
for ( c = 0; c <= num_band_units_cols_minus_1; c++ ) {	
banding_in_band_unit_present_flag	f(1)
}	
}	
}	
}	
}	
}

metadata_icc_profile( metadataPayloadSize ) {
icc_profile_data_payload_bytes	le(metadataPayloadSize)
}

metadata_scan_type() {
mps_pic_struct_type	f(5)
mps_source_scan_type_idc	f(2)
mps_duplicate_flag	f(1)
}

metadata_temporal_point_info() {
frame_presentation_time	leb128()
}

metadata_decoded_frame_hash() {
hash_type	f(4)
per_plane	f(1)
has_grain	f(1)
is_monochrome	f(1)
reserved	f(1)
if ( per_plane ) {	
numPlanes = is_monochrome ? 1 : 3	
for ( i = 0; i < numPlanes; i++ ) {	
plane_hash[ i ]	le(16)
}	
} else {	
frame_hash	le(16)
}	
}

metadata_user_data_unregistered( metadataPayloadSize ) {
uuid_iso_iec_11578	f(128)
for( i = 16; i < metadataPayloadSize; i++ ) {	
user_data_payload_byte	f(8)
}	
}

frame_header( isFirst ) {
if ( isFirst ) {	
SeenFrameHeader = 1	
CountFrameHeaderForLevelConstraint = 1	
FrameSymbolCount = 0	
startBitPos = get_position()	
frame_header_info()	
NumFrameHeaderBits = get_position() - startBitPos	
FirstPictureInTU = 0	
if ( IsBridge ) {	
NumTiles = TileCols * TileRows	
tg_start = 0	
tg_end = NumTiles - 1	
tile_group_payload( 0 )	
} else if ( ShowExistingFrame ||	
TipFrameMode == TIP_FRAME_AS_OUTPUT ||	
bru_inactive ) {	
decode_frame_wrapup()	
SeenFrameHeader = 0	
CountFrameHeaderForLevelConstraint = 0	
} else {	
TileNum = 0	
}	
} else {	
CountFrameHeaderForLevelConstraint = 0	
frame_header_copy()	
}	
}

frame_header_copy() {
for ( i = 0; i < NumFrameHeaderBits; i++ ) {	
header_bit[ i ]	f(1)
}	
}

frame_header_info() {
keyFrame = obu_type == OBU_CLOSED_LOOP_KEY || obu_type == OBU_OPEN_LOOP_KEY	
IsRegular = ( obu_type == OBU_OPEN_LOOP_KEY ||	
obu_type == OBU_REGULAR_TILE_GROUP ||	
obu_type == OBU_REGULAR_TIP ||	
obu_type == OBU_REGULAR_SEF ||	
obu_type == OBU_SWITCH ||	
obu_type == OBU_RAS_FRAME ||	
obu_type == OBU_BRIDGE_FRAME )	
for ( i = 0; i < NUM_CUSTOM_QMS; i++ ) {	
QmSeen[ i ] = 0	
}	
startCVS = obu_type == OBU_CLOSED_LOOP_KEY && FirstPictureInTU	
if ( startCVS ) {	
OlkEncountered = 0	
for( i = 0; i < MAX_NUM_MLAYERS; i++ ) {	
OlkRefresh[ i ] = 0	
}	
flush_implicit_output_frames( 0 )	
}	
if ( OlkEncountered && IsRegular && FirstPictureInTU ) {	
flush_implicit_output_frames( 1 )	
OlkEncountered = 0	
allowedFrames = 0	
for ( i = 0; i < MAX_NUM_MLAYERS; i++ ) {	
allowedFrames |= OlkRefresh[ i ]	
OlkRefresh[ i ] = 0	
}	
for ( i = 0; i < NUM_REF_FRAMES; i++ ) {	
if ( ( allowedFrames & (1 << i) ) == 0 && RefLongTermId[ i ] == -1 )	
RefValid[ i ] = 0	
}	
}	
IsBridge = obu_type == OBU_BRIDGE_FRAME	
if ( IsBridge ) {	
cur_mfh_id = 0	
} else {	
cur_mfh_id	uvlc()
}	
if ( cur_mfh_id == 0 ) {	
seq_header_id_in_frame_header	uvlc()
load_sequence_header( seq_header_id_in_frame_header )	
mfh_deblocking_filter_update[ cur_mfh_id ] = 0	
} else {	
load_sequence_header( MfhSeqHeaderId[ cur_mfh_id ] )	
}	
if ( keyFrame ) {	
if ( seq_lcr_id != 0 ) {	
activate_layer_configuration_record( seq_lcr_id )	
}	
}	
if ( cur_mfh_id == 0 || !mfh_frame_size_present_flag[ cur_mfh_id ] ) {	
mfh_frame_width_minus_1[ cur_mfh_id ] = max_frame_width_minus_1	
mfh_frame_height_minus_1[ cur_mfh_id ] = max_frame_height_minus_1	
}	
if ( keyFrame && FirstPictureInTU ) {	
reset_qm()	
}	
if ( IsBridge ) {	
n = CeilLog2(NumRefFrames)	
bridge_frame_ref_idx	f(n)
}	
allFrames = (1 << NumRefFrames) - 1	
use_bru = 0	
bru_inactive = 0	
if ( single_picture_header_flag ) {	
ShowExistingFrame = 0	
FrameType = KEY_FRAME	
FrameIsIntra = 1	
immediate_output_frame = 1	
implicit_output_frame = 0	
} else {	
ShowExistingFrame = is_sef()	
if ( ShowExistingFrame == 1 ) {	
n = CeilLog2(NumRefFrames)	
frame_to_show_map_idx	f(n)
derive_sef_order_hint	f(1)
if ( derive_sef_order_hint == 0 ) {	
sef_order_hint	f(OrderHintBits)
OrderHintLsbs = sef_order_hint	
OrderHint = get_disp_order_hint()	
} else {	
OrderHint = RefOrderHint[ frame_to_show_map_idx ]	
}	
if ( IsRegular && OlkEncountered && !FirstPictureInTU ) {	
OlkTUOrderHint = derive_sef_order_hint ?	
RefOrderHint[ frame_to_show_map_idx ] :	
OrderHint	
}	
refresh_frame_flags = 0	
FrameType = RefFrameType[ frame_to_show_map_idx ]	
immediate_output_frame = 1	
film_grain_config()	
if ( derive_sef_order_hint ) {	
save_grain_params( frame_to_show_map_idx )	
}	
TipFrameMode = TIP_FRAME_DISABLED	
return	
}	
if ( IsBridge ) {	
FrameType = INTER_FRAME	
} else if ( obu_type == OBU_SWITCH || obu_type == OBU_RAS_FRAME ) {	
restricted_prediction_switch	f(1)
FrameType = SWITCH_FRAME	
} else if ( is_tip_frame() ) {	
FrameType = INTER_FRAME	
} else if ( obu_type == OBU_CLOSED_LOOP_KEY ||	
obu_type == OBU_OPEN_LOOP_KEY ) {	
FrameType = KEY_FRAME	
} else {	
frame_is_inter	f(1)
FrameType = frame_is_inter ? INTER_FRAME : INTRA_ONLY_FRAME	
}	
LongTermId = -1	
if ( FrameType == KEY_FRAME ) {	
long_term_id_plus_1	f(long_term_frame_id_bits)
LongTermId = long_term_id_plus_1 - 1	
}	
num_key_ref_frames = 0	
if ( (obu_type == OBU_RAS_FRAME || obu_type == OBU_OPEN_LOOP_KEY) &&	
long_term_frame_id_bits != 0) {	
num_key_ref_frames	f(3)
for ( i = 0; i < num_key_ref_frames; i++ ) {	
ref_long_term_id[ i ]	f(long_term_frame_id_bits)
}	
}	
if ( FrameType == SWITCH_FRAME && restricted_prediction_switch ) {	
for (i = 0; i < NUM_REF_FRAMES; i++) {	
if ( MLayerPresenceMap[RefMLayerId[i]][obu_mlayer_id] ) {	
if ( is_frame_eligible_for_output( i ) ) {	
output_frame_buffers( i )	
}	
RefOrderHint[ i ] = RESTRICTED_OH	
}	
}	
}	
if ( obu_type == OBU_RAS_FRAME ||	
(obu_type == OBU_SWITCH && restricted_prediction_switch) ) {	
reset_qm()	
}	
FrameIsIntra = (FrameType == INTRA_ONLY_FRAME ||	
FrameType == KEY_FRAME)	
if ( IsBridge || obu_type == OBU_OPEN_LOOP_KEY ) {	
immediate_output_frame = 0	
} else {	
immediate_output_frame	f(1)
}	
if ( IsBridge || immediate_output_frame || monotonic_output_order_flag ) {	
implicit_output_frame = 0	
} else {	
implicit_output_frame	f(1)
}	
}	
if ( use_256x256_superblock ) {	
SbSize = FrameIsIntra ? BLOCK_128X128 : BLOCK_256X256	
} else if ( use_128x128_superblock ) {	
SbSize = BLOCK_128X128	
} else {	
SbSize = BLOCK_64X64	
}	
if ( FrameType == KEY_FRAME && immediate_output_frame ) {	
for ( i = 0; i < REFS_PER_FRAME; i++ ) {	
OrderHints[ i ] = 0	
}	
}	
disable_cross_frame_cdf_init = 0	
if ( IsBridge ) {	
primary_ref_frame = PRIMARY_REF_NONE	
OrderHintLsbs = RefOrderHintLsbs[ bridge_frame_ref_idx ]	
OrderHint = RefOrderHint[ bridge_frame_ref_idx ]	
} else {	
if ( FrameType == SWITCH_FRAME ) {	
frame_size_override_flag = 1	
} else if ( single_picture_header_flag ) {	
frame_size_override_flag = 0	
} else {	
frame_size_override_flag	f(1)
}	
order_hint	f(OrderHintBits)
OrderHintLsbs = order_hint	
OrderHint = get_disp_order_hint()	
if ( FrameIsIntra || FrameType == SWITCH_FRAME ) {	
primary_ref_frame = PRIMARY_REF_NONE	
} else {	
signal_primary_ref_frame	f(1)
if ( !is_tip_frame() ) {	
disable_cross_frame_cdf_init	f(1)
}	
if ( signal_primary_ref_frame ) {	
primary_ref_frame	f(3)
} else {	
primary_ref_frame = PRIMARY_REF_CHOOSE	
}	
}	
}	
FrameMvPrecision = MV_PRECISION_ONE_PEL	
MvPrecision = FrameMvPrecision	
allow_high_precision_mv = 0	
use_ref_frame_mvs = 0	
allow_intrabc = 0	
allow_global_intrabc = 0	
allow_local_intrabc = 0	
allow_high_precision_mv = 0	
allow_df_sub_pu = 0	
if ( IsBridge ) {	
bridge_frame_overwrite_flag	f(1)
}	
if ( FrameType == KEY_FRAME ) {	
if ( obu_type == OBU_CLOSED_LOOP_KEY && max_mlayer_id == 0 ) {	
refresh_frame_flags = allFrames	
} else if ( enable_short_refresh_frame_flags ) {	
n = CeilLog2(NumRefFrames)	
frame_to_refresh	f(n)
refresh_frame_flags = 1 << frame_to_refresh	
} else {	
refresh_frame_flags	f(NumRefFrames)
}	
if ( obu_type == OBU_CLOSED_LOOP_KEY && FirstPictureInTU ) {	
for ( i = 0; i < NumRefFrames; i++ ) {	
RefValid[i] = 0	
}	
}	
if ( obu_type == OBU_CLOSED_LOOP_KEY ) {	
OlkEncountered = 0	
for( i = 0; i < MAX_NUM_MLAYERS; i++ ) {	
OlkRefresh[ i ] = 0	
}	
}	
if ( obu_type == OBU_OPEN_LOOP_KEY ) {	
OlkEncountered = 1	
OlkRefresh[ obu_mlayer_id ] = refresh_frame_flags	
if ( implicit_output_frame ) {	
OlkTUOrderHint = OrderHint	
}	
}	
} else if ( IsBridge && !bridge_frame_overwrite_flag ) {	
refresh_frame_flags = 1 << bridge_frame_ref_idx	
} else if ( obu_type == OBU_RAS_FRAME && max_mlayer_id == 0 ) {	
refresh_frame_flags = 0	
for ( i = 0; i < NumRefFrames; i++ ) {	
if ( !RefValid[i] || !long_term_id_in_use( RefLongTermId[i] ) ) {	
refresh_frame_flags |= (1 << i)	
}	
}	
} else if ( FrameType == SWITCH_FRAME ) {	
refresh_frame_flags	f(NumRefFrames)
} else if ( enable_short_refresh_frame_flags &&	
FrameType != SWITCH_FRAME &&	
FrameType != KEY_FRAME ) {	
has_refresh_frame_flags	f(1)
if ( has_refresh_frame_flags ) {	
n = CeilLog2(NumRefFrames)	
frame_to_refresh	f(n)
refresh_frame_flags = 1 << frame_to_refresh	
} else {	
refresh_frame_flags = 0	
}	
} else {	
refresh_frame_flags	f(NumRefFrames)
}	
AllowedFrames = -1	
if ( IsRegular && OlkEncountered && !FirstPictureInTU ) {	
AllowedFrames = 0	
for ( i = 0; i < MAX_NUM_MLAYERS; i++ ) {	
AllowedFrames |= OlkRefresh[ i ]	
}	
OlkRefresh[ obu_mlayer_id ] |= refresh_frame_flags	
if ( immediate_output_frame || implicit_output_frame ) {	
OlkTUOrderHint = OrderHint	
}	
}	
if ( FrameIsIntra ) {	
frame_size()	
screen_content_params()	
intrabc_params()	
NumTotalRefs = 0	
TipFrameMode = TIP_FRAME_DISABLED	
} else {	
if ( FrameType == SWITCH_FRAME || IsBridge ) {	
explicitRefFrameMap = 1	
} else if ( explicit_ref_frame_map ) {	
frame_explicit_ref_frame_map	f(1)
explicitRefFrameMap = frame_explicit_ref_frame_map	
} else {	
explicitRefFrameMap = 0	
}	
if ( IsBridge ) {	
NumTotalRefs = 1	
} else if ( explicitRefFrameMap ) {	
num_total_refs	f(3)
NumTotalRefs = num_total_refs	
} else {	
get_ref_frames( 0 )	
}	
for ( i = 0; i < NumTotalRefs; i++ ) {	
if ( IsBridge ) {	
ref_frame_idx[ i ] = bridge_frame_ref_idx	
} else if ( explicitRefFrameMap ) {	
n = CeilLog2(NumRefFrames)	
ref_frame_idx[ i ]	f(n)
}	
}	
if ( IsBridge ) {	
frame_size_with_bridge()	
} else if ( frame_size_override_flag && FrameType != SWITCH_FRAME ) {	
frame_size_with_refs()	
} else {	
frame_size()	
}	
if ( !explicitRefFrameMap ) {	
get_ref_frames( 1 )	
}	
NumSameRefCompound = Min(num_same_ref_compound, NumTotalRefs)	
if ( enable_bru && FrameType == INTER_FRAME && !is_tip_frame() &&	
!IsBridge ) {	
use_bru	f(1)
if ( use_bru ) {	
n = CeilLog2(NumTotalRefs)	
bru_ref	f(n)
bru_inactive	f(1)
}	
}	
if ( explicitRefFrameMap ) {	
for ( i = 0; i < NumTotalRefs; i++ ) {	
ScoresDistance[ i ] = get_relative_dist( OrderHint,	
RefOrderHint[ ref_frame_idx[ i ] ] )	
}	
}	
get_past_future_cur_ref_lists()	
if ( FrameType == SWITCH_FRAME || !enable_ref_frame_mvs ||	
IsBridge || bru_inactive ) {	
use_ref_frame_mvs = 0	
} else {	
use_ref_frame_mvs	f(1)
}	
if ( use_ref_frame_mvs && NumTotalRefs > 1 && SbSize != BLOCK_64X64 ) {	
tmvp_sample_step_minus_1	f(1)
ProjStep = tmvp_sample_step_minus_1 + 1	
} else {	
ProjStep = 1	
}	
for ( i = 0; i < NumTotalRefs; i++ ) {	
FrameDistance[ i ] = get_relative_dist( OrderHint,	
RefOrderHint[ ref_frame_idx[ i ] ] )	
if ( RefOrderHint[ ref_frame_idx[ i ] ] == RESTRICTED_OH ) {	
FrameDistance[ i ] = -FrameDistance[ i ]	
}	
}	
for ( i = 0; i < NumTotalRefs; i++ ) {	
refFrame = i	
hint = RefOrderHint[ ref_frame_idx[ i ] ]	
OrderHints[ refFrame ] = hint	
}	
if ( enable_tip &&	
(use_ref_frame_mvs && NumTotalRefs >= 2) &&	
!bru_inactive ) {	
TipInterpFilter = EIGHTTAP_SHARP	
TipGlobalMv[ 0 ] = 0	
TipGlobalMv[ 1 ] = 0	
if ( EnableTipOutput && is_tip_frame() ) {	
TipFrameMode = TIP_FRAME_AS_OUTPUT	
} else {	
tip_frame_mode	f(1)
TipFrameMode = tip_frame_mode	
}	
frame_opfl_refine_type()	
if ( TipFrameMode != TIP_FRAME_DISABLED &&	
enable_tip_hole_fill ) {	
allow_tip_hole_fill	f(1)
} else {	
allow_tip_hole_fill = 0	
}	
usesEqualWeight = enable_tip_refinemv &&	
NumFutureRefs > 0 && NumPastRefs > 0 &&	
( opfl_refine_type != REFINE_NONE || enable_refinemv )	
if ( TipFrameMode == TIP_FRAME_DISABLED || usesEqualWeight ) {	
tip_global_wtd_index = 0	
} else {	
tip_global_wtd_index	f(3)
}	
if ( TipFrameMode == TIP_FRAME_AS_OUTPUT ) {	
tip_mv_zero	f(1)
if ( !tip_mv_zero ) {	
tip_mv_row	f(4)
tip_mv_col	f(4)
if ( tip_mv_row != 0 ) {	
tip_mv_row_sign	f(1)
TipGlobalMv[ 0 ] = tip_mv_row_sign ?	
-tip_mv_row : tip_mv_row	
}	
if ( tip_mv_col != 0 ) {	
tip_mv_col_sign	f(1)
TipGlobalMv[ 1 ] = tip_mv_col_sign ?	
-tip_mv_col : tip_mv_col	
}	
}	
tip_sharp	f(1)
if ( tip_sharp ) {	
TipInterpFilter = EIGHTTAP_SHARP	
} else {	
tip_regular	f(1)
TipInterpFilter = tip_regular ? EIGHTTAP: EIGHTTAP_SMOOTH	
}	
}	
} else {	
TipFrameMode = TIP_FRAME_DISABLED	
if ( !bru_inactive && !IsBridge ) {	
frame_opfl_refine_type()	
}	
}	
if ( TipFrameMode != TIP_FRAME_AS_OUTPUT && !bru_inactive &&	
!IsBridge ) {	
screen_content_params()	
intrabc_params()	
max_drl_bits_minus_1 = seq_max_drl_bits_minus_1	
if ( allow_frame_max_drl_bits ) {	
change_drl	f(1)
if ( change_drl ) {	
n = MAX_REF_MV_STACK_SIZE - 2	
max_drl_bits_minus_1	ns(n)
if ( max_drl_bits_minus_1 >= seq_max_drl_bits_minus_1 ) {	
max_drl_bits_minus_1 += 1	
}	
}	
}	
if ( force_integer_mv ) {	
FrameMvPrecision = MV_PRECISION_ONE_PEL	
UsePerBlockMvPrecision = 0	
} else {	
use_qtr_precision_mv	f(1)
if ( use_qtr_precision_mv ) {	
FrameMvPrecision = MV_PRECISION_QUARTER_PEL	
} else {	
allow_high_precision_mv	f(1)
FrameMvPrecision = allow_high_precision_mv ?	
MV_PRECISION_EIGHTH_PEL : MV_PRECISION_HALF_PEL	
}	
UsePerBlockMvPrecision = enable_flex_mvres	
}	
MvPrecision = FrameMvPrecision	
read_interpolation_filter()	
for ( mode = INTERINTRA; mode < MOTION_MODES; mode++ ) {	
if ( !seq_frame_motion_modes_present_flag ) {	
frame_enabled_motion_modes[ mode ] =	
seq_enabled_motion_modes[ mode ]	
} else if ( seq_enabled_motion_modes[ mode ] ) {	
frame_enabled_motion_modes[ mode ]	f(1)
} else {	
frame_enabled_motion_modes[ mode ] = 0	
}	
}	
}	
}	
if ( TipFrameMode == TIP_FRAME_AS_OUTPUT ) {	
if ( enable_tip_explicit_qp ) {	
quantization_params()	
}	
if ( enable_df_sub_pu ) {	
allow_df_sub_pu	f(1)
}	
if ( allow_df_sub_pu ) {	
apply_deblocking_filter_tip	f(1)
} else {	
apply_deblocking_filter_tip = 0	
}	
}	
if ( TipFrameMode == TIP_FRAME_AS_OUTPUT || bru_inactive || IsBridge ) {	
for ( i = 0 ; i < 3; i++ ) {	
frame_filters_on[ i ] = 0	
}	
if ( bru_inactive || IsBridge ) {	
if ( IsBridge ) {	
tile_info()	
refIdx = bridge_frame_ref_idx	
} else {	
refIdx = ref_frame_idx[ bru_ref ]	
}	
base_q_idx = RefBaseQIdx[ refIdx ]	
DeltaQUAc = RefDeltaQUAc[ refIdx ]	
DeltaQVAc = RefDeltaQVAc[ refIdx ]	
set_primary_ref_frame_and_ctx( 0 )	
} else if ( apply_deblocking_filter_tip ) {	
tile_info()	
}	
film_grain_config()	
if ( bru_inactive || IsBridge ) {	
set_primary_ref_frame_and_ctx( 1 )	
}	
for (row = 0; row < MiRows; row++) {	
for (col = 0; col < MiCols; col++) {	
SegmentIds[ row ][ col ] = 0	
}	
}	
for ( ref = 0; ref < REFS_PER_FRAME; ref++ ) {	
for ( i = 0; i < 6; i++ ) {	
gm_params[ ref ][ i ] = Default_Warp_Params[ i ]	
}	
}	
} else {	
disable_cdf_update	f(1)
}	
if ( bru_inactive || IsBridge ) {	
apply_deblocking_filter[ 0 ] = 0	
apply_deblocking_filter[ 1 ] = 0	
cdef_frame_enable = 0	
for ( plane = 0; plane < NumPlanes; plane++ ) {	
ccso_planes[ plane ] = 0	
}	
FrameRestorationType[ 0 ] = RESTORE_NONE	
FrameRestorationType[ 1 ] = RESTORE_NONE	
FrameRestorationType[ 2 ] = RESTORE_NONE	
gdf_frame_enable = 0	
segmentation_enabled = 0	
for ( i = 0; i < MAX_SEGMENTS; i++ ) {	
for ( j = 0; j < SEG_LVL_MAX; j++ ) {	
FeatureEnabled[ i ][ j ] = 0	
FeatureData[ i ][ j ] = 0	
}	
}	
if ( primary_ref_frame == PRIMARY_REF_NONE ||	
disable_cross_frame_cdf_init) {	
init_coeff_cdfs()	
}	
return	
}	
if ( use_ref_frame_mvs == 1 ) {	
HasBothRefs = ClosestFuture != NONE && ClosestPast != NONE	
motion_field_estimation()	
if ( TipFrameMode == TIP_FRAME_AS_OUTPUT ) {	
if ( !enable_tip_explicit_qp ) {	
slot0 = ref_frame_idx[ ClosestPast ]	
slot1 = ref_frame_idx[ ClosestFuture ]	
base_q_idx = Round2(RefBaseQIdx[slot0] + RefBaseQIdx[slot1], 1)	
DeltaQUAc = Round2(RefDeltaQUAc[slot0] + RefDeltaQUAc[slot1], 1)	
DeltaQVAc = Round2(RefDeltaQVAc[slot0] + RefDeltaQVAc[slot1], 1)	
}	
set_primary_ref_frame_and_ctx( 1 )	
for (i = 0; i < MAX_SEGMENTS; i++) {	
for ( j = 0; j < SEG_LVL_MAX; j++ ) {	
FeatureData[ i ][ j ] = 0	
FeatureEnabled[ i ][ j ] = 0	
}	
}	
for (row = 0; row < MiRows; row++) {	
for (col = 0; col < MiCols; col++) {	
PrevSegmentIds[ row ][ col ] = 0	
}	
}	
for ( plane = 0; plane < 3; plane++ ) {	
ccso_planes[ plane ] = 0	
}	
if ( primary_ref_frame == PRIMARY_REF_NONE ||	
disable_cross_frame_cdf_init ) {	
init_coeff_cdfs()	
}	
}	
if ( TipFrameMode == TIP_FRAME_DISABLED ) {	
fill_tpl_mvs_sample_gap()	
}	
}	
if ( TipFrameMode != TIP_FRAME_DISABLED ) {	
setup_tip_motion_field()	
}	
if ( TipFrameMode == TIP_FRAME_AS_OUTPUT ) {	
return	
}	
tile_info()	
quantization_params()	
set_primary_ref_frame_and_ctx( 1 )	
segmentation_params()	
setup_qm_params()	
delta_q_params()	
if ( primary_ref_frame == PRIMARY_REF_NONE ||	
disable_cross_frame_cdf_init ) {	
init_coeff_cdfs()	
}	
if ( DerivedPrimaryRefFrame != PRIMARY_REF_NONE ) {	
load_previous_segment_ids()	
}	
CodedLossless = 1	
HasLosslessSegment = 0	
for ( segmentId = 0; segmentId < MaxSegments; segmentId++ ) {	
qindex = get_qindex( 1, segmentId )	
LosslessArray[ segmentId ] = qindex == 0 && delta_q_present == 0 &&	
DeltaQYDc + BaseYDcDeltaQ <= 0 &&	
DeltaQUDc + BaseUVDcDeltaQ <= 0 &&	
DeltaQVDc + BaseUVDcDeltaQ <= 0 &&	
DeltaQUAc + BaseUVAcDeltaQ <= 0 &&	
DeltaQVAc + BaseUVAcDeltaQ <= 0	
if ( LosslessArray[ segmentId ] ) {	
HasLosslessSegment = 1	
} else {	
CodedLossless = 0	
}	
if ( using_qmatrix ) {	
if ( LosslessArray[ segmentId ] ) {	
SegQMLevel[ 0 ][ segmentId ] = 15	
SegQMLevel[ 1 ][ segmentId ] = 15	
SegQMLevel[ 2 ][ segmentId ] = 15	
} else {	
qmNum = pic_qm_num_minus_1 + 1	
qmIndexBits = CeilLog2( qmNum )	
qm_index	f(qmIndexBits)
SegQMLevel[ 0 ][ segmentId ] = qm_y[ qm_index ]	
SegQMLevel[ 1 ][ segmentId ] = qm_u[ qm_index ]	
SegQMLevel[ 2 ][ segmentId ] = qm_v[ qm_index ]	
}	
}	
}	
if ( CodedLossless ) {	
allow_tcq = 0	
} else if ( choose_tcq_per_frame ) {	
allow_tcq	f(1)
} else {	
allow_tcq = enable_tcq	
}	
if ( CodedLossless || !enable_parity_hiding || allow_tcq ) {	
allow_parity_hiding = 0	
} else {	
allow_parity_hiding	f(1)
}	
deblocking_filter_params()	
gdf_params()	
cdef_params()	
lr_params()	
ccso_params()	
read_tx_mode()	
frame_reference_mode()	
skip_mode_params()	
if (!FrameIsIntra && enable_bawp) {	
allow_bawp	f(1)
} else {	
allow_bawp = 0	
}	
if ( !FrameIsIntra && frame_enabled_motion_modes[ DELTAWARP ] ) {	
allow_warpmv_mode	f(1)
} else {	
allow_warpmv_mode = 0	
}	
reduced_tx_set	f(2)
global_motion_params()	
film_grain_config()	
}

frame_opfl_refine_type() {
if ( TipFrameMode == TIP_FRAME_AS_OUTPUT ) {	
opfl_refine_type = ( !enable_tip_refinemv ||	
enable_opfl_refine == REFINE_NONE ) ?	
REFINE_NONE : REFINE_ALL	
} else if ( enable_opfl_refine == REFINE_AUTO ) {	
opfl_refine_type	f(1)
if ( opfl_refine_type != REFINE_SWITCHABLE ) {	
opfl_refine_all	f(1)
opfl_refine_type = opfl_refine_all ? REFINE_ALL : REFINE_NONE	
}	
} else {	
opfl_refine_type = enable_opfl_refine	
}	
}

screen_content_params() {
if ( seq_force_screen_content_tools == SELECT_SCREEN_CONTENT_TOOLS ) {	
allow_screen_content_tools	f(1)
} else {	
allow_screen_content_tools = seq_force_screen_content_tools	
}	
if ( allow_screen_content_tools ) {	
if ( seq_force_integer_mv == SELECT_INTEGER_MV ) {	
force_integer_mv	f(1)
} else {	
force_integer_mv = seq_force_integer_mv	
}	
} else {	
force_integer_mv = 0	
}	
}

intrabc_params() {
allow_intrabc	f(1)
if ( allow_intrabc ) {	
if ( FrameIsIntra ) {	
allow_global_intrabc	f(1)
if ( allow_global_intrabc ) {	
allow_local_intrabc	f(1)
} else {	
allow_local_intrabc = 1	
}	
} else {	
allow_global_intrabc = 0	
allow_local_intrabc = 1	
}	
max_bvp_drl_bits_minus_1 = seq_max_bvp_drl_bits_minus_1	
if ( allow_frame_max_bvp_drl_bits ) {	
change_bvp_drl	f(1)
if ( change_bvp_drl ) {	
max_bvp_drl_bits_minus_1	ns(2)
if ( max_bvp_drl_bits_minus_1 >=	
seq_max_bvp_drl_bits_minus_1 ) {	
max_bvp_drl_bits_minus_1 += 1	
}	
}	
}	
}	
}

frame_size() {
if ( frame_size_override_flag ) {	
n = frame_width_bits_minus_1 + 1	
frame_width_minus_1	f(n)
n = frame_height_bits_minus_1 + 1	
frame_height_minus_1	f(n)
FrameWidth = frame_width_minus_1 + 1	
FrameHeight = frame_height_minus_1 + 1	
} else {	
FrameWidth = mfh_frame_width_minus_1[ cur_mfh_id ] + 1	
FrameHeight = mfh_frame_height_minus_1[ cur_mfh_id ] + 1	
}	
compute_image_size()	
}

frame_size_with_bridge() {
n = frame_width_bits_minus_1 + 1	
bridge_frame_width_minus_1	f(n)
n = frame_height_bits_minus_1 + 1	
bridge_frame_height_minus_1	f(n)
FrameWidth = Min( RefFrameWidth[ bridge_frame_ref_idx ],	
bridge_frame_width_minus_1 + 1 )	
FrameHeight = Min( RefFrameHeight[ bridge_frame_ref_idx ],	
bridge_frame_height_minus_1 + 1 )	
compute_image_size()	
}

frame_size_with_refs() {
for ( i = 0; i < NumTotalRefs; i++ ) {	
found_ref	f(1)
if ( found_ref == 1 ) {	
FrameWidth = RefFrameWidth[ ref_frame_idx[ i ] ]	
FrameHeight = RefFrameHeight[ ref_frame_idx[ i ] ]	
break	
}	
}	
if ( NumTotalRefs == 0 || found_ref == 0 ) {	
frame_size()	
} else {	
compute_image_size()	
}	
}

read_interpolation_filter() {
is_filter_switchable	f(1)
if ( is_filter_switchable == 1 ) {	
interpolation_filter = SWITCHABLE	
} else {	
interpolation_filter	f(2)
}	
}

deblocking_filter_params() {
if ( CodedLossless ) {	
apply_deblocking_filter[ 0 ] = 0	
apply_deblocking_filter[ 1 ] = 0	
return	
}	
if ( enable_df_sub_pu && FrameType == INTER_FRAME ) {	
allow_df_sub_pu	f(1)
} else {	
allow_df_sub_pu = 0	
}	
if ( mfh_deblocking_filter_update[ cur_mfh_id ] ) {	
apply_deblocking_filter[ 0 ] = mfh_apply_deblocking_filter[ cur_mfh_id ][ 0 ]	
apply_deblocking_filter[ 1 ] = mfh_apply_deblocking_filter[ cur_mfh_id ][ 1 ]	
apply_deblocking_filter[ 2 ] = 0	
apply_deblocking_filter[ 3 ] = 0	
if ( NumPlanes > 1 ) {	
if ( apply_deblocking_filter[0] || apply_deblocking_filter[1] ) {	
apply_deblocking_filter[2] = mfh_apply_deblocking_filter[cur_mfh_id][2]	
apply_deblocking_filter[3] = mfh_apply_deblocking_filter[cur_mfh_id][3]	
}	
}	
} else {	
apply_deblocking_filter[ 0 ]	f(1)
apply_deblocking_filter[ 1 ]	f(1)
apply_deblocking_filter[ 2 ] = 0	
apply_deblocking_filter[ 3 ] = 0	
if ( NumPlanes > 1 ) {	
if ( apply_deblocking_filter[ 0 ] || apply_deblocking_filter[ 1 ] ) {	
apply_deblocking_filter[ 2 ]	f(1)
apply_deblocking_filter[ 3 ]	f(1)
}	
}	
}	
for ( i = 0; i < 4; i++ ) {	
if ( apply_deblocking_filter[ i ] ) {	
df_delta_q_present[ i ]	f(1)
if ( df_delta_q_present[ i ] ) {	
dfParBits = df_par_bits_minus_2 + 2	
df_delta_q[ i ]	f(dfParBits)
DfDeltaQ[ i ] = df_delta_q[ i ] - ( 1 << (dfParBits - 1) )	
} else {	
DfDeltaQ[ i ] = (i == 1) ? DfDeltaQ[ 0 ] : 0	
}	
} else {	
DfDeltaQ[ i ] = 0	
}	
}	
}

quantization_params() {
n = BitDepth == 8 ? 8 : 9	
base_q_idx	f(n)
DeltaQYDc = 0	
DeltaQUDc = 0	
DeltaQUAc = 0	
DeltaQVDc = 0	
DeltaQVAc = 0	
if ( TipFrameMode != TIP_FRAME_AS_OUTPUT && y_dc_delta_q_enabled ) {	
DeltaQYDc = read_delta_q()	
}	
if ( NumPlanes > 1 && (	
uv_ac_delta_q_enabled ||	
(TipFrameMode != TIP_FRAME_AS_OUTPUT && uv_dc_delta_q_enabled)	
) ) {	
if ( separate_uv_delta_q ) {	
diff_uv_delta	f(1)
} else {	
diff_uv_delta = 0	
}	
if ( TipFrameMode != TIP_FRAME_AS_OUTPUT && uv_dc_delta_q_enabled ) {	
DeltaQUDc = read_delta_q()	
}	
if ( uv_ac_delta_q_enabled ) {	
DeltaQUAc = read_delta_q()	
}	
if ( equal_ac_dc_q ) {	
DeltaQUDc = DeltaQUAc	
}	
if ( diff_uv_delta ) {	
if ( TipFrameMode != TIP_FRAME_AS_OUTPUT &&	
uv_dc_delta_q_enabled ) {	
DeltaQVDc = read_delta_q()	
}	
if ( uv_ac_delta_q_enabled ) {	
DeltaQVAc = read_delta_q()	
}	
if ( equal_ac_dc_q ) {	
DeltaQVDc = DeltaQVAc	
}	
} else {	
DeltaQVDc = DeltaQUDc	
DeltaQVAc = DeltaQUAc	
}	
}	
}

setup_qm_params() {
using_qmatrix	f(1)
if ( using_qmatrix ) {	
if ( segmentation_enabled ) {	
pic_qm_num_minus_1	f(2)
} else {	
pic_qm_num_minus_1 = 0	
}	
qmNum = pic_qm_num_minus_1 + 1	
for ( i = 0; i < qmNum; i++ ) {	
qm_y[ i ]	f(4)
if ( NumPlanes > 1 ) {	
qm_uv_same_as_y	f(1)
if ( qm_uv_same_as_y ) {	
qm_u[ i ] = qm_y [ i ]	
qm_v[ i ] = qm_y [ i ]	
} else {	
qm_u[ i ]	f(4)
if ( !separate_uv_delta_q ) {	
qm_v[ i ] = qm_u[ i ]	
} else {	
qm_v[ i ]	f(4)
}	
}	
}	
}	
}	
}

read_delta_q() {
delta_coded	f(1)
if ( delta_coded ) {	
delta_q	su(7)
} else {	
delta_q = 0	
}	
return delta_q	
}

segmentation_params() {
segmentation_enabled	f(1)
if ( segmentation_enabled == 1 ) {	
if ( cur_mfh_id > 0 && mfh_seg_info_present_flag[ cur_mfh_id ] ) {	
haveSegParams = mfh_ext_seg_flag[ cur_mfh_id ] == enable_ext_seg	
allowChange = haveSegParams && mfh_allow_seg_info_change[cur_mfh_id]	
mfhId = cur_mfh_id	
} else if ( seq_seg_info_present_flag ) {	
haveSegParams = 1	
allowChange = seq_allow_seg_info_change	
mfhId = 0	
} else {	
haveSegParams = 0	
allowChange = 0	
}	
if ( allowChange ) {	
reuse_seg_info	f(1)
} else {	
reuse_seg_info = haveSegParams	
}	
if ( reuse_seg_info ) {	
for ( i = 0; i < MAX_SEGMENTS; i++ ) {	
for ( j = 0; j < SEG_LVL_MAX; j++ ) {	
if ( mfhId == 0 ) {	
FeatureData[ i ][ j ] = SeqFeatureData[ i ][ j ]	
FeatureEnabled[ i ][ j ] = SeqFeatureEnabled[ i ][ j ]	
} else {	
FeatureData[ i ][ j ] =	
MfhFeatureData[ mfhId ][ i ][ j ]	
FeatureEnabled[ i ][ j ] =	
MfhFeatureEnabled[ mfhId ][ i ][ j ]	
}	
}	
}	
} else {	
(FeatureEnabled, FeatureData) = seg_info( MaxSegments )	
}	
if ( DerivedPrimaryRefFrame == PRIMARY_REF_NONE ) {	
segmentation_update_map = 1	
segmentation_temporal_update = 0	
} else {	
segmentation_update_map	f(1)
if ( segmentation_update_map == 1 && FrameType != SWITCH_FRAME ) {	
segmentation_temporal_update	f(1)
} else {	
segmentation_temporal_update = 0	
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
for ( i = 0; i < MaxSegments; i++ ) {	
for ( j = 0; j < SEG_LVL_MAX; j++ ) {	
if ( FeatureEnabled[ i ][ j ] ) {	
LastActiveSegId = i	
if ( j >= SEG_LVL_SKIP ) {	
SegIdPreSkip = 1	
}	
}	
}	
}	
}

tile_info () {
sb4x4 = Num_4x4_Blocks_Wide[ SbSize ]	
sbShift = Mi_Width_Log2[ SbSize ]	
sbCols = ( MiCols + sb4x4 - 1 ) >> sbShift	
sbRows = ( MiRows + sb4x4 - 1 ) >> sbShift	
if ( IsBridge ) {	
haveTileParams = 0	
} else {	
haveTileParams = seq_tile_info_present_flag	
}	
if ( haveTileParams &&	
( SeqUniformTileSpacingFlag ? (	
uniform_eligible( SeqTileRowsLog2, sbRows) &&	
uniform_eligible( SeqTileColsLog2, sbCols) ) :	
( SeqSbCols == sbCols && SeqSbRows == sbRows ) ) ) {	
if ( allow_tile_info_change ) {	
reuse_tile_info	f(1)
} else {	
reuse_tile_info = 1	
}	
} else {	
reuse_tile_info = 0	
}	
seqSbSize = get_seq_sb_size()	
if ( reuse_tile_info ) {	
( sbRowStarts, TileRows, TileRowsLog2, sbColStarts, TileCols,	
TileColsLog2, sbShift2) = reuse_tile_params(SeqUniformTileSpacingFlag,	
SeqSbRowStarts, SeqTileRows, SeqTileRowsLog2,	
SeqSbColStarts, SeqTileCols, SeqTileColsLog2, seqSbSize, SbSize )	
} else {	
( sbRowStarts, sbRows, TileRows, TileRowsLog2, sbColStarts, sbCols,	
TileCols, TileColsLog2, uniformSpacing, sbShift2) = tile_params(	
FrameWidth, FrameHeight, seqSbSize, SbSize, IsBridge )	
}	
for ( i = 0; i < TileCols; i++ ) {	
MiColStarts[ i ] = sbColStarts[ i ] << sbShift2	
}	
for ( i = 0; i < TileRows; i++ ) {	
MiRowStarts[ i ] = sbRowStarts[ i ] << sbShift2	
}	
MiColStarts[ TileCols ] = MiCols	
MiRowStarts[ TileRows ] = MiRows	
if ( (TileCols > 1 || TileRows > 1) && !IsBridge &&	
TipFrameMode != TIP_FRAME_AS_OUTPUT ) {	
if ( !enable_avg_cdf || !avg_cdf_type ) {	
n = TileRowsLog2 + TileColsLog2	
context_update_tile_id	f(n)
}	
tile_size_bytes_minus_1	f(2)
TileSizeBytes = tile_size_bytes_minus_1 + 1	
} else {	
context_update_tile_id = 0	
}	
}

tile_params( frameWidth, frameHeight, uniformSbSize, sbSize, isBridge ) {
miCols = 2 * ( ( frameWidth + 7 ) >> 3 )	
miRows = 2 * ( ( frameHeight + 7 ) >> 3 )	
sb4x4 = Num_4x4_Blocks_Wide[ sbSize ]	
sbShift = Mi_Width_Log2[ sbSize ]	
sbCols = ( miCols + sb4x4 - 1 ) >> sbShift	
sbRows = ( miRows + sb4x4 - 1 ) >> sbShift	
if ( seq_level_idx != 31 ) {	
maxTileWidthSb = ( Tile_Width_Scaling_Factor[seq_tier][seq_level_idx] *	
MAX_TILE_WIDTH ) >> (sbShift + 4)	
maxTileAreaSb = ( Tile_Area_Scaling_Factor[seq_tier][seq_level_idx] *	
MAX_TILE_AREA ) >> ( 2 * (sbShift + 2) + 2 )	
} else {	
maxTileWidthSb = sbCols	
maxTileAreaSb = sbCols * sbRows	
}	
minLog2TileCols = tile_log2(maxTileWidthSb, sbCols)	
maxLog2TileCols = tile_log2(1, Min(sbCols, MAX_TILE_COLS))	
maxLog2TileRows = tile_log2(1, Min(sbRows, MAX_TILE_ROWS))	
minLog2Tiles = Max( minLog2TileCols,	
tile_log2(maxTileAreaSb, sbRows * sbCols))	
if ( isBridge ) {	
uniform_tile_spacing_flag = 1	
} else {	
uniform_tile_spacing_flag	f(1)
}	
if ( uniform_tile_spacing_flag ) {	
sbShift = Mi_Width_Log2[ uniformSbSize ]	
tileColsLog2 = minLog2TileCols	
if ( !isBridge ) {	
while ( tileColsLog2 < maxLog2TileCols ) {	
increment_tile_cols_log2	f(1)
if ( increment_tile_cols_log2 == 1 ) {	
tileColsLog2 += 1	
} else {	
break	
}	
}	
}	
(sbColStarts, tileCols) = uniform_spacing( tileColsLog2, miCols,	
uniformSbSize )	
tileColsLog2 = tile_log2(1, tileCols)	
minLog2TileRows = Max( minLog2Tiles - tileColsLog2, 0)	
tileRowsLog2 = minLog2TileRows	
if ( !isBridge ) {	
while ( tileRowsLog2 < maxLog2TileRows ) {	
increment_tile_rows_log2	f(1)
if ( increment_tile_rows_log2 == 1 ) {	
tileRowsLog2++	
} else {	
break	
}	
}	
}	
(sbRowStarts, tileRows) = uniform_spacing( tileRowsLog2, miRows,	
uniformSbSize )	
} else {	
widestTileSb = 1	
startSb = 0	
for ( i = 0; startSb < sbCols; i++ ) {	
sbColStarts[ i ] = startSb	
n = Min(sbCols - startSb, maxTileWidthSb)	
width_in_sbs_minus_1	ns(n)
sizeSb = width_in_sbs_minus_1 + 1	
widestTileSb = Max( sizeSb, widestTileSb )	
startSb += sizeSb	
}	
tileCols = i	
tileColsLog2 = tile_log2(1, tileCols)	
if (minLog2Tiles > 0) {	
maxTileAreaSb = (sbRows * sbCols) >> (minLog2Tiles + 1)	
} else {	
maxTileAreaSb = sbRows * sbCols	
}	
maxTileHeightSb = Max( maxTileAreaSb / widestTileSb, 1 )	
startSb = 0	
for ( i = 0; startSb < sbRows; i++ ) {	
sbRowStarts[ i ] = startSb	
maxHeight = Min(sbRows - startSb, maxTileHeightSb)	
height_in_sbs_minus_1	ns(maxHeight)
sizeSb = height_in_sbs_minus_1 + 1	
startSb = startSb + sizeSb	
}	
tileRows = i	
}	
tileRowsLog2 = tile_log2(1, tileRows)	
return ( sbRowStarts, sbRows, tileRows, tileRowsLog2, sbColStarts, sbCols,	
tileCols, tileColsLog2, uniform_tile_spacing_flag, sbShift)	
}

delta_q_params() {
delta_q_res = 0	
delta_q_present = 0	
if ( base_q_idx > 0 ) {	
delta_q_present	f(1)
}	
if ( delta_q_present ) {	
delta_q_res	f(2)
}	
}

gdf_params() {
if ( CodedLossless || !enable_gdf ) {	
gdf_frame_enable = 0	
} else {	
if ( single_picture_header_flag ) {	
gdf_frame_enable = 1	
} else {	
gdf_frame_enable	f(1)
}	
if ( !gdf_frame_enable ) {	
return	
}	
gdfBlkSize = Max(Block_Width[ SbSize ],GDF_MIN_SIZE)	
if ( gdf_unit_matches_sb_size ) {	
gdfBlkSize = Block_Width[ SbSize ]	
} else if ( SbSize == BLOCK_64X64 ) {	
a = 0	
for ( i = 0; i < TileCols; i++ ) {	
a = a | MiColStarts[ i ]	
}	
for ( i = 0; i < TileRows; i++ ) {	
a = a | MiRowStarts[ i ]	
}	
if ( a & 16 ) {	
gdfBlkSize = 64	
}	
}	
GdfBlkSize = gdfBlkSize	
if ( MiCols * MI_SIZE > gdfBlkSize ||	
MiRows * MI_SIZE > gdfBlkSize ||	
( disable_loopfilters_across_tiles &&	
(TileRows > 1 || TileCols > 1) ) ) {	
gdf_per_block	f(1)
} else {	
gdf_per_block = 0	
}	
gdf_pic_qc_idx	f(2)
gdf_pic_scale_idx	f(2)
GdfPixScale = 1 + gdf_pic_scale_idx	
}	
}

cdef_params() {
if ( CodedLossless ||	
!enable_cdef ) {	
cdef_frame_enable = 0	
return	
}	
if ( single_picture_header_flag ) {	
cdef_frame_enable = 1	
} else {	
cdef_frame_enable	f(1)
}	
if ( !cdef_frame_enable ) {	
return	
}	
cdef_damping_minus_3	f(2)
CdefDamping = cdef_damping_minus_3 + 3	
cdef_strengths_minus_1	f(3)
CdefStrengths = cdef_strengths_minus_1 + 1	
if ( CdefOnSkipTxfm == CDEF_ON_SKIP_TXFM_ADAPTIVE ) {	
cdef_on_skip_txfm_frame_enable	f(1)
} else if ( CdefOnSkipTxfm == CDEF_ON_SKIP_TXFM_ALWAYS_ON ) {	
cdef_on_skip_txfm_frame_enable = 1	
} else {	
cdef_on_skip_txfm_frame_enable = 0	
}	
for ( i = 0; i < CdefStrengths; i++ ) {	
cdef_y_pri_zero	f(1)
if ( cdef_y_pri_zero ) {	
cdef_y_pri_strength[ i ] = 0	
} else {	
cdef_y_pri_strength[ i ]	f(4)
}	
cdef_y_sec_strength[ i ]	f(2)
if ( cdef_y_sec_strength[ i ] == 3 ) {	
cdef_y_sec_strength[ i ] += 1	
}	
if ( NumPlanes > 1 ) {	
cdef_uv_pri_zero	f(1)
if ( cdef_uv_pri_zero ) {	
cdef_uv_pri_strength[ i ] = 0	
} else {	
cdef_uv_pri_strength[ i ]	f(4)
}	
cdef_uv_sec_strength[ i ]	f(2)
if ( cdef_uv_sec_strength[ i ] == 3 ) {	
cdef_uv_sec_strength[ i ] += 1	
}	
}	
}	
}

lr_params() {
if ( CodedLossless || !enable_restoration ) {	
FrameRestorationType[ 0 ] = RESTORE_NONE	
FrameRestorationType[ 1 ] = RESTORE_NONE	
FrameRestorationType[ 2 ] = RESTORE_NONE	
UsesLr = 0	
for ( i = 0; i < 3; i++ ) {	
frame_filters_on[ i ] = 0	
}	
return	
}	
usesLumaLr = 0	
usesChromaLr = 0	
for ( plane = 0; plane < NumPlanes; plane++ ) {	
toolsCount = 1	
indexToTool[ 0 ] = RESTORE_NONE	
for ( i = 1; i < RESTORE_SWITCHABLE_TYPES; i++ ) {	
if ( !lr_tools_disable[ plane > 0 ][ i ] ) {	
indexToTool[ toolsCount ] = i	
toolsCount += 1	
}	
}	
indexToTool[ toolsCount ] = RESTORE_SWITCHABLE	
allowSwitchable = (toolsCount > 2)	
n = toolsCount + allowSwitchable	
tool_index	ns(n)
FrameRestorationType[ plane ] = indexToTool[ tool_index ]	
if ( FrameRestorationType[ plane ] != RESTORE_NONE ) {	
if ( plane == 0 ) {	
usesLumaLr = 1	
} else {	
usesChromaLr = 1	
}	
}	
r = FrameRestorationType[ plane ]	
if ( plane == 0 ) {	
NumFilterClasses = 1	
}	
frame_filters_on[ plane ] = 0	
temporal_pred_flag[ plane ] = 0	
if ( r == RESTORE_WIENER_NONSEP || r == RESTORE_SWITCHABLE ) {	
frame_filters_on[ plane ]	f(1)
if ( frame_filters_on[ plane ] ) {	
numRefFrames = (FrameIsIntra || FrameType == SWITCH_FRAME) ?	
0 : NumTotalRefs	
if ( numRefFrames > 0 ) {	
temporal_pred_flag[ plane ]	f(1)
}	
if ( temporal_pred_flag[ plane ] && numRefFrames > 1 ) {	
n = CeilLog2(numRefFrames)	
rst_ref_pic_idx	f(n)
} else {	
rst_ref_pic_idx = 0	
}	
if ( temporal_pred_flag[ plane ] ) {	
refIdx = ref_frame_idx[ rst_ref_pic_idx ]	
refPlane = plane	
if ( plane > 0 && !RefFrameFiltersOn[ refIdx ][ plane ] ) {	
refPlane = plane == 1 ? 2 : 1	
}	
if ( plane == 0 ) {	
NumFilterClasses = RefNumFilterClasses[ refIdx ]	
}	
for ( c = 0; c < WIENER_NS_CLASSES; c++ ) {	
for ( i = 0; i < WIENER_NS_CHROMA_COEFFS; i++ ) {	
FrameLrWienerNs[plane][c][i] =	
RefFrameLrWienerNs[refIdx][refPlane][c][i]	
}	
}	
}	
}	
if ( plane == 0 && frame_filters_on[ 0 ] ) {	
if ( temporal_pred_flag[ plane ] ) {	
num_filter_classes_idx =	
Encode_Num_Filter_Classes[ NumFilterClasses ]	
} else {	
num_filter_classes_idx	f(3)
NumFilterClasses =	
Decode_Num_Filter_Classes[ num_filter_classes_idx ]	
}	
qindex = base_q_idx	
index = get_filter_set_index(qindex)	
SubclassLookup =	
Pc_Wiener_Sub_Classify2[ index ][ num_filter_classes_idx ]	
}	
}	
}	
UsesLr = usesLumaLr || usesChromaLr	
LoopRestorationSize[ 0 ] = RESTORATION_TILESIZE_MAX >> 3	
LoopRestorationSize[ 1 ] = RESTORATION_TILESIZE_MAX >>	
( 3 + Max(SubsamplingX, SubsamplingY) )	
if ( usesLumaLr ) {	
lr_luma_use_half_size	f(1)
if ( lr_luma_use_half_size ) {	
shift = 1	
} else if ( SbSize == BLOCK_256X256 ) {	
shift = 0	
} else {	
lr_luma_use_max_size	f(1)
if ( lr_luma_use_max_size ) {	
shift = 0	
} else if ( SbSize == BLOCK_128X128 ) {	
shift = 2	
} else {	
lr_luma_use_quarter_size	f(1)
shift = lr_luma_use_quarter_size ? 2 : 3	
}	
}	
LoopRestorationSize[ 0 ] = RESTORATION_TILESIZE_MAX >> shift	
}	
if ( usesChromaLr ) {	
LoopRestorationSize[ 1 ] = RESTORATION_TILESIZE_MAX >>	
Max(SubsamplingX, SubsamplingY)	
lr_chroma_use_half_size	f(1)
if ( lr_chroma_use_half_size ) {	
shift = 1	
} else if ( SbSize == BLOCK_256X256 ) {	
shift = 0	
} else {	
lr_chroma_use_max_size	f(1)
if ( lr_chroma_use_max_size ) {	
shift = 0	
} else if ( SbSize == BLOCK_128X128 ) {	
shift = 2	
} else {	
lr_chroma_use_quarter_size	f(1)
shift = lr_chroma_use_quarter_size ? 2 : 3	
}	
}	
LoopRestorationSize[ 1 ] = LoopRestorationSize[ 1 ] >> shift	
}	
LoopRestorationSize[ 2 ] = LoopRestorationSize[ 1 ]	
for ( plane = 0; plane < NumPlanes; plane++ ) {	
if ( frame_filters_on[ plane ] && !temporal_pred_flag[ plane ] ) {	
read_wienerns_filter(plane, 0, 0, 1)	
}	
}	
}

ccso_params() {
for ( plane = 0; plane < NumPlanes; plane++ ) {	
ccso_planes[ plane ] = 0	
}	
if ( CodedLossless || !enable_ccso ) {	
return	
}	
a = 0	
for ( i = 0; i < TileCols; i++ ) {	
a = a | MiColStarts[ i ]	
}	
for ( i = 0; i < TileRows; i++ ) {	
a = a | MiRowStarts[ i ]	
}	
if ( ccso_unit_matches_sb_size ) {	
CcsoLumaSizeLog2 = Mi_Width_Log2[ SbSize ] + MI_SIZE_LOG2	
} else if ( (a & 63) == 0 ) {	
CcsoLumaSizeLog2 = 8	
} else if ( (a & 31) == 0 ) {	
CcsoLumaSizeLog2 = 7	
} else {	
CcsoLumaSizeLog2 = 6	
}	
if ( single_picture_header_flag ) {	
ccso_frame_flag = 1	
} else {	
ccso_frame_flag	f(1)
}	
if ( !ccso_frame_flag ) {	
return	
}	
for ( plane = 0; plane < NumPlanes; plane++ ) {	
ccso_planes[ plane ]	f(1)
if ( ccso_planes[ plane ] ) {	
if ( FrameIsIntra || FrameType == SWITCH_FRAME ) {	
reuse_ccso[ plane ] = 0	
sb_reuse_ccso[ plane ] = 0	
} else {	
reuse_ccso[ plane ]	f(1)
sb_reuse_ccso[ plane ]	f(1)
}	
if ( reuse_ccso[ plane ] || sb_reuse_ccso[ plane ] ) {	
n = CeilLog2(NumTotalRefs)	
ccso_ref_idx[ plane ]	f(n)
idx = ref_frame_idx[ ccso_ref_idx[ plane ] ]	
tmpCcsoLumaSizeLog2 = CcsoLumaSizeLog2	
load_ccso_params(idx, plane)	
CcsoLumaSizeLog2 = tmpCcsoLumaSizeLog2	
}	
}	
if ( ccso_planes[ plane ] && !reuse_ccso[ plane ] ) {	
ccso_bo_only[ plane ]	f(1)
ccso_scale_idx[ plane ]	f(2)
if ( ccso_bo_only[ plane ] ) {	
ccso_quant_idx[ plane ] = 0	
ccso_ext_filter[ plane ] = 0	
ccso_edge_clf[ plane ] = 0	
} else {	
ccso_quant_idx[ plane ]	f(2)
ccso_ext_filter[ plane ]	f(3)
quantStep = CCSO_Quant_Sz[ ccso_scale_idx[ plane ] ]	
[ ccso_quant_idx[ plane ] ]	
if ( quantStep == 0 ) {	
ccso_edge_clf[ plane ] = 0	
} else {	
ccso_edge_clf[ plane ]	f(1)
}	
}	
n = 2 + ccso_bo_only[ plane ]	
ccso_max_band_log2[ plane ]	f(n)
maxEdgeInterval = CCSO_INPUT_INTERVAL - ccso_edge_clf[ plane ]	
if ( ccso_bo_only[ plane ] ) {	
maxEdgeInterval = 1	
}	
maxBand = 1 << ccso_max_band_log2[ plane ]	
for ( d0 = 0; d0 < maxEdgeInterval; d0++ ) {	
for ( d1 = 0; d1 < maxEdgeInterval; d1++ ) {	
for ( band = 0; band < maxBand; band++ ) {	
ccso_offset_idx	tu(7)
offset = Ccso_Offset[ ccso_offset_idx ] *	
(ccso_scale_idx[ plane ] + 1)	
CcsoFilterOffset[ plane ][ band ][ d0 ][ d1 ] = offset	
}	
}	
}	
}	
}	
}

read_tx_mode() {
if ( CodedLossless == 1 ) {	
TxMode = ONLY_4X4	
} else {	
tx_mode_select	f(1)
if ( tx_mode_select ) {	
TxMode = TX_MODE_SELECT	
} else {	
TxMode = TX_MODE_LARGEST	
}	
}	
}

skip_mode_params() {
if ( FrameIsIntra || FrameType == SWITCH_FRAME ) {	
skipModeAllowed = 0	
} else {	
skipModeAllowed = 1	
SkipModeFrame[ 0 ] = 0	
SkipModeFrame[ 1 ] = NumTotalRefs > 1 ? 1 : 0	
if ( NumTotalRefs > 1 ) {	
curToRef0 = Abs(get_relative_dist(OrderHint,	
RefOrderHint[ ref_frame_idx[ 0 ] ]))	
curToRef1 = Abs(get_relative_dist(OrderHint,	
RefOrderHint[ ref_frame_idx[ 1 ] ]))	
if ( OrderHints[ 0 ] == RESTRICTED_OH ) {	
curToRef0 = 0	
}	
if ( OrderHints[ 1 ] == RESTRICTED_OH ) {	
curToRef1 = 0	
}	
if ( Abs(curToRef0 - curToRef1) > 1 ) {	
SkipModeFrame[ 1 ] = 0	
}	
}	
}	
if ( skipModeAllowed ) {	
skip_mode_present	f(1)
} else {	
skip_mode_present = 0	
}	
}

frame_reference_mode() {
if ( FrameIsIntra ) {	
reference_select = 0	
} else {	
reference_select	f(1)
}	
}

global_motion_params() {
for ( ref = 0; ref < REFS_PER_FRAME; ref++ ) {	
GmType[ ref ] = IDENTITY	
for ( i = 0; i < 6; i++ ) {	
gm_params[ ref ][ i ] = ( ( i % 3 == 2 ) ?	
1 << WARPEDMODEL_PREC_BITS : 0 )	
}	
}	
if ( FrameIsIntra || !enable_global_motion) {	
return	
}	
use_global_motion	f(1)
if ( !use_global_motion ) {	
return	
}	
for ( i = 0; i < 6; i++ ) {	
baseParams[ i ] = Default_Warp_Params[ i ]	
}	
baseDistance = 1	
if ( FrameType == SWITCH_FRAME ) {	
our_ref = NumTotalRefs	
} else {	
n = NumTotalRefs + 1	
our_ref	ns(n)
}	
if ( our_ref != NumTotalRefs ) {	
refIdx = ref_frame_idx[ our_ref ]	
if ( RefNumTotalRefs[ refIdx ] > 0 ) {	
n = RefNumTotalRefs[ refIdx ]	
their_ref	ns(n)
for ( i = 0; i < 6; i++ ) {	
baseParams[ i ] = SavedGmParams[ refIdx ][ their_ref ][ i ]	
}	
baseDistance = get_relative_dist(OrderHints[ our_ref ],	
SavedOrderHints[ refIdx ][ their_ref ])	
}	
}	
for ( ref = 0; ref < NumTotalRefs; ref++ ) {	
dist = get_relative_dist(OrderHint,OrderHints[ ref ])	
if ( dist == 0 || OrderHints[ ref ] == RESTRICTED_OH ) {	
for ( i = 0; i < 6; i++ ) {	
gm_params[ ref ][ i ] = Default_Warp_Params[ i ]	
}	
GmType[ ref ] = IDENTITY	
} else {	
for ( i = 0; i < 6; i++ ) {	
params = scale_warp_model(baseParams, baseDistance, dist)	
PrevGmParams[ ref ][ i ] = params[ i ]	
}	
is_global	f(1)
if ( is_global ) {	
is_rot_zoom	f(1)
if ( is_rot_zoom ) {	
type = ROTZOOM	
} else {	
type = AFFINE	
}	
} else {	
type = IDENTITY	
}	
GmType[ ref ] = type	
if ( type >= ROTZOOM ) {	
read_global_param(ref,2)	
read_global_param(ref,3)	
if ( type == AFFINE ) {	
read_global_param(ref,4)	
read_global_param(ref,5)	
} else {	
gm_params[ ref ][ 4 ] = -gm_params[ ref ][ 3 ]	
gm_params[ ref ][ 5 ] = gm_params[ ref ][ 2 ]	
}	
read_global_param(ref,0)	
read_global_param(ref,1)	
}	
}	
}	
}

read_global_param( ref, idx ) {
precBits = GM_ALPHA_PREC_BITS	
mx = GM_ALPHA_MAX	
if ( idx < 2 ) {	
precBits = GM_TRANS_PREC_BITS	
mx = GM_TRANS_MAX	
}	
precDiff = WARPEDMODEL_PREC_BITS - precBits	
round = (idx % 3) == 2 ? (1 << WARPEDMODEL_PREC_BITS) : 0	
sub = (idx % 3) == 2 ? (1 << precBits) : 0	
r = (PrevGmParams[ ref ][ idx ] >> precDiff) - sub	
gm_params[ ref ][ idx ] =	
(decode_signed_subexp_with_ref( -mx, mx + 1, r, 3 ) << precDiff) + round	
}

decode_signed_subexp_with_ref( low, high, r, k ) {
x = decode_unsigned_subexp_with_ref(high - low, r - low, k)	
return x + low	
}

decode_unsigned_subexp_with_ref( mx, r, k ) {
v = decode_subexp( mx, k )	
if ( (r << 1) <= mx ) {	
return inverse_recenter(r, v)	
} else {	
return mx - 1 - inverse_recenter(mx - 1 - r, v)	
}	
}


decode_subexp( numSyms, k ) {
i = 0	
mk = 0	
while ( 1 ) {	
b2 = i ? k + i - 1 : k	
a = 1 << b2	
if ( numSyms <= mk + 3 * a ) {	
n = numSyms - mk	
subexp_final_bits	ns(n)
return subexp_final_bits + mk	
} else {	
subexp_more_bits	f(1)
if ( subexp_more_bits ) {	
i++	
mk += a	
} else {	
subexp_bits	f(b2)
return subexp_bits + mk	
}	
}	
}	
}

film_grain_config() {
if ( !film_grain_params_present || ( !immediate_output_frame && !implicit_output_frame ) ) {	
apply_grain = 0	
} else if ( single_picture_header_flag ) {	
apply_grain = 1	
} else {	
apply_grain	f(1)
}	
if ( apply_grain ) {	
fgm_id	f(3)
load_grain_model( fgm_id )	
grain_seed	f(16)
}	
}

film_grain_model( monochrome, subX, subY ) {
if ( monochrome ) {	
chroma_scaling_from_luma = 0	
} else {	
chroma_scaling_from_luma	f(1)
}	
num_y_points	f(4)
if ( num_y_points > 0) {	
point_value_increment_bits_minus_1	f(3)
bitsIncr = point_value_increment_bits_minus_1 + 1	
point_scaling_bits_minus_5	f(2)
bitsScal = point_scaling_bits_minus_5 + 5	
}	
for ( i = 0; i < num_y_points; i++ ) {	
point_y_value[ i ]	f(bitsIncr)
if ( i > 0 ) {	
point_y_value[ i ] += point_y_value[ i - 1 ]	
}	
point_y_scaling[ i ]	f(bitsScal)
}	
if ( monochrome || chroma_scaling_from_luma ) {	
num_cb_points = 0	
num_cr_points = 0	
} else {	
num_cb_points	f(4)
if ( num_cb_points > 0 ) {	
point_value_increment_bits_minus_1	f(3)
bitsIncr = point_value_increment_bits_minus_1 + 1	
point_scaling_bits_minus_5	f(2)
bitsScal = point_scaling_bits_minus_5 + 5	
}	
for ( i = 0; i < num_cb_points; i++ ) {	
point_cb_value[ i ]	f(bitsIncr)
if ( i > 0 ) {	
point_cb_value[ i ] += point_cb_value[ i - 1 ]	
}	
point_cb_scaling[ i ]	f(bitsScal)
}	
num_cr_points	f(4)
if ( num_cr_points > 0 ) {	
point_value_increment_bits_minus_1	f(3)
bitsIncr = point_value_increment_bits_minus_1 + 1	
point_scaling_bits_minus_5	f(2)
bitsScal = point_scaling_bits_minus_5 + 5	
}	
for ( i = 0; i < num_cr_points; i++ ) {	
point_cr_value[ i ]	f(bitsIncr)
if ( i > 0 ) {	
point_cr_value[ i ] += point_cr_value[ i - 1 ]	
}	
point_cr_scaling[ i ]	f(bitsScal)
}	
}	
grain_scaling_minus_8	f(2)
ar_coeff_lag	f(2)
numPosLuma = 2 * ar_coeff_lag * ( ar_coeff_lag + 1 )	
if ( num_y_points ) {	
bits_per_ar_coeff_y_minus_5	f(2)
bitsCoef = bits_per_ar_coeff_y_minus_5 + 5	
numPosChroma = numPosLuma + 1	
for ( i = 0; i < numPosLuma; i++ ) {	
ar_coeffs_y[ i ]	f(bitsCoef)
ar_coeffs_y[ i ] -= (1 << (bitsCoef - 1))	
}	
} else {	
numPosChroma = numPosLuma	
}	
if ( chroma_scaling_from_luma || num_cb_points ) {	
bits_per_ar_coeff_cb_minus_5	f(2)
bitsCoef = bits_per_ar_coeff_cb_minus_5 + 5	
for ( i = 0; i < numPosChroma; i++ ) {	
ar_coeffs_cb[ i ]	f(bitsCoef)
ar_coeffs_cb[ i ] -= (1 << (bitsCoef - 1))	
}	
}	
if ( chroma_scaling_from_luma || num_cr_points ) {	
bits_per_ar_coeff_cr_minus_5	f(2)
bitsCoef = bits_per_ar_coeff_cr_minus_5 + 5	
for ( i = 0; i < numPosChroma; i++ ) {	
ar_coeffs_cr[ i ]	f(bitsCoef)
ar_coeffs_cr[ i ] -= (1 << (bitsCoef - 1))	
}	
}	
ar_coeff_shift_minus_6	f(2)
grain_scale_shift	f(2)
if ( num_cb_points ) {	
cb_mult	f(8)
cb_luma_mult	f(8)
cb_offset	f(9)
}	
if ( num_cr_points ) {	
cr_mult	f(8)
cr_luma_mult	f(8)
cr_offset	f(9)
}	
overlap_flag	f(1)
clip_to_restricted_range	f(1)
if ( clip_to_restricted_range ) {	
fg_mc_identity	f(1)
} else {	
fg_mc_identity = 0	
}	
film_grain_block_size	f(1)
}

tile_group_obu( sz ) {
startBitPos = get_position()	
is_first_tile_group	f(1)
if ( is_first_tile_group ) {	
frame_header_present_flag = 1	
} else {	
frame_header_present_flag	f(1)
}	
if ( frame_header_present_flag ) {	
frame_header( is_first_tile_group )	
}	
if ( bru_inactive ) {	
headerBits = get_position() - startBitPos	
remainingBits = sz * 8 - headerBits	
trailing_bits( remainingBits )	
return	
}	
NumTiles = TileCols * TileRows	
tile_start_and_end_present_flag = 0	
if ( NumTiles > 1 ) {	
tile_start_and_end_present_flag	f(1)
}	
if ( NumTiles == 1 || !tile_start_and_end_present_flag ) {	
tg_start = 0	
tg_end = NumTiles - 1	
} else {	
tileBits = TileColsLog2 + TileRowsLog2	
tg_start	f(tileBits)
tg_end	f(tileBits)
}	
if ( use_bru ) {	
if ( NumTiles > 1 ) {	
for ( TileNum = tg_start; TileNum <= tg_end; TileNum++ ) {	
tileRow = TileNum / TileCols	
tileCol = TileNum % TileCols	
bru_tile_active	f(1)
BruTileActives[ tileRow ][ tileCol ] = bru_tile_active	
}	
} else {	
BruTileActives[ 0 ][ 0 ] = 1	
}	
}	
byte_alignment()	
endBitPos = get_position()	
headerBytes = (endBitPos - startBitPos) / 8	
sz -= headerBytes	
tile_group_payload( sz )	
}

tile_group_payload( sz ) {
for ( TileNum = tg_start; TileNum <= tg_end; TileNum++ ) {	
tileRow = TileNum / TileCols	
tileCol = TileNum % TileCols	
lastTile = TileNum == tg_end	
if ( lastTile ) {	
tileSize = sz	
} else if ( !IsBridge ) {	
tile_size_minus_1	le(TileSizeBytes)
tileSize = tile_size_minus_1 + 1	
sz -= tileSize + TileSizeBytes	
}	
MiRowStart = MiRowStarts[ tileRow ]	
MiRowEnd = MiRowStarts[ tileRow + 1 ]	
MiColStart = MiColStarts[ tileCol ]	
MiColEnd = MiColStarts[ tileCol + 1 ]	
BruTileActive = use_bru ? BruTileActives[ tileRow ][ tileCol ] : 0	
align = Num_4x4_Blocks_High[ SbSize ]	
shift = Mi_Height_Log2[ SbSize ]	
for( r = MiRowStart; r < ((MiRowEnd + align - 1) >> shift) << shift;	
r++) {	
for( c = MiColStart; c < ((MiColEnd + align - 1) >> shift) << shift;	
c++) {	
IBCCoded[ r ][ c ] = 0	
}	
}	
CurrentQIndex = base_q_idx	
if ( !IsBridge ) {	
init_symbol( tileSize )	
}	
decode_tile()	
if ( !IsBridge ) {	
exit_symbol()	
}	
}	
if ( tg_end == NumTiles - 1 ) {	
if ( !IsBridge ) {	
frame_end_update_cdf()	
}	
decode_frame_wrapup()	
SeenFrameHeader = 0	
}	
}

