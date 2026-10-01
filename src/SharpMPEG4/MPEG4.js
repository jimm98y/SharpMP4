VisualObjectSequence() {
do {
visual_object_sequence_start_code 32 bslbf
profile_and_level_indication 8 uimsbf
if (profile_and_level_indication == 11100001-11101000) {
next_start_code_studio()
extension_and_user_data( 0 )
StudioVisualObject()
} else {
while ( next_bits()== user_data_start_code){
user_data()
}
VisualObject()
}
} while ( next_bits() != visual_object_sequence_end_code)
visual_object_sequence_end_code 32 bslbf
}

VisualObject() {
visual_object_start_code 32 bslbf
is_visual_object_identifier 1 uimsbf
if (is_visual_object_identifier) {
visual_object_verid 4 uimsbf
visual_object_priority 3 uimsbf
}
visual_object_type 4 uimsbf
if (visual_object_type == "video ID" || visual_object_type == "still texture ID") {
video_signal_type()
}
next_start_code()
while ( next_bits()== user_data_start_code){
user_data()
}
if (visual_object_type == "video ID") {
video_object_start_code 32 bslbf
VideoObjectLayer()
}
else if (visual_object_type == "still texture ID") {
StillTextureObject()
}
else if (visual_object_type == "mesh ID") {
MeshObject()
}
else if (visual_object_type == "FBA ID") {
FBAObject()
}
else if (visual_object_type == "3D mesh ID") {
3D_Mesh_Object()
}
if (next_bits() != "0000 0000 0000 0000 0000 0001")
next_start_code()
}

video_signal_type() {
video_signal_type 1 bslbf
if (video_signal_type) {
video_format 3 uimsbf
video_range 1 bslbf
colour_description 1 bslbf
if (colour_description) {
colour_primaries 8 uimsbf
transfer_characteristics 8 uimsbf
matrix_coefficients 8 uimsbf
}
}
}

user_data() {
user_data_start_code 32 bslbf
while( next_bits() != '0000 0000 0000 0000 0000 0001' ) {
user_data 8 uimsbf
}
}

VideoObjectLayer() {
if(next_bits() == video_object_layer_start_code) {
short_video_header = 0
video_object_layer_start_code 32 bslbf
random_accessible_vol 1 bslbf
video_object_type_indication 8 uimsbf
if ( video_object_type_indication == "Fine Granularity Scalable" ) {
fgs_layer_type 2 uimsbf
video_object_layer_priority 3 uimsbf
aspect_ratio_info 4 uimsbf
if (aspect_ratio_info == "extended_PAR") {
par_width 8 uimsbf
par_height 8 uimsbf
}
vol_control_parameters 1 bslbf
if (vol_control_parameters) {
chroma_format 2 uimsbf
low_delay 1 uimsbf
}
marker_bit 1 bslbf
vop_time_increment_resolution 16 uimsbf
marker_bit 1 bslbf
fixed_vop_rate 1 bslbf
if (fixed_vop_rate)
fixed_vop_time_increment 1-16 uimsbf
marker_bit 1 bslbf
video_object_layer_width 13 uimsbf
marker_bit 1 bslbf
video_object_layer_height 13 uimsbf
marker_bit 1 bslbf
interlaced 1 bslbf
if (fgs_layer_type =="FGST" || fgs_layer_type =="FGS_FGST")
fgs_ref_layer_id 4 uimsbf
if (fgs_layer_type =="FGS" || fgs_layer_type =="FGS_FGST") {
fgs_frequency_weighting_enable 1 bslbf
if ( fgs_frequency_weighting_enable ) {
load_fgs_frequency_weighting_matrix 1 bslbf
if (load_fgs_frequency_weighting_matrix)
fgs_frequency_weighting_matrix 3*[2-64] uimsbf
}
}
if (fgs_layer_type =="FGST" || fgs_layer_type =="FGS_FGST")
{
fgst_frequency_weighting_enable 1 bslbf
if ( fgst_frequency_weighting_enable ) {
load_fgst_frequency_weighting_matrix 1 bslbf
if (load_fgst_frequency_weighting_matrix)
fgst_frequency_weighting_matrix 3*[2-64] uimsbf
}
}
quarter_sample 1 bslbf
fgs_resync_marker_disable 1 bslbf
do {
if (nextbits_bytealigned() == group_of_vop_start_code)
Group_of_VideoObjectPlane()
FGSVideoObjectPlane()
} while((nextbits_bytealigned()==group_of_vop_start_code)||
(nextbits_bytealigned()==fgs_vop_start_code))
} else {
is_object_layer_identifier 1 uimsbf
if (is_object_layer_identifier) {
video_object_layer_verid 4 uimsbf
video_object_layer_priority 3 uimsbf
}
aspect_ratio_info 4 uimsbf
if (aspect_ratio_info == "extended_PAR") {
par_width 8 uimsbf
par_height 8 uimsbf
}
vol_control_parameters 1 bslbf
if (vol_control_parameters) {
chroma_format 2 uimsbf
low_delay 1 uimsbf
vbv_parameters 1 bslbf
if (vbv_parameters) {
first_half_bit_rate 15 uimsbf
marker_bit 1 bslbf
latter_half_bit_rate 15 uimsbf
marker_bit 1 bslbf
first_half_vbv_buffer_size 15 uimsbf
marker_bit 1 bslbf
latter_half_vbv_buffer_size 3 uimsbf
first_half_vbv_occupancy 11 uimsbf
marker_bit 1 bslbf
latter_half_vbv_occupancy 15 uimsbf
marker_bit 1 bslbf
}
}
video_object_layer_shape 2 uimsbf
    if (video_object_layer_shape == "grayscale"
&& video_object_layer_verid != '0001')
video_object_layer_shape_extension 4 uimsbf
marker_bit 1 bslbf
vop_time_increment_resolution 16 uimsbf
marker_bit 1 bslbf
fixed_vop_rate 1 bslbf
if (fixed_vop_rate)
fixed_vop_time_increment 1-16 uimsbf
if (video_object_layer_shape != "binary only") {
if (video_object_layer_shape == "rectangular") {
marker_bit 1 bslbf
video_object_layer_width 13 uimsbf
marker_bit 1 bslbf
video_object_layer_height 13 uimsbf
marker_bit 1 bslbf
}
interlaced 1 bslbf
obmc_disable 1 bslbf
    if (video_object_layer_verid == '0001')
sprite_enable 1 bslbf
else
sprite_enable 2 uimsbf
if (sprite_enable== "static" || sprite_enable == "GMC") {
if (sprite_enable != "GMC") {
sprite_width 13 uimsbf
marker_bit 1 bslbf
sprite_height 13 uimsbf
marker_bit 1 bslbf
sprite_left_coordinate 13 simsbf
marker_bit 1 bslbf
sprite_top_coordinate 13 simsbf
marker_bit 1 bslbf
}
no_of_sprite_warping_points 6 uimsbf
sprite_warping_accuracy 2 uimsbf
sprite_brightness_change 1 bslbf
if (sprite_enable != "GMC")
low_latency_sprite_enable 1 bslbf
}
    if (video_object_layer_verid != '0001' &&
video_object_layer_shape != "rectangular")
sadct_disable 1 bslbf
not_8_bit 1 bslbf
if (not_8_bit) {
quant_precision 4 uimsbf
bits_per_pixel 4 uimsbf
}
if (video_object_layer_shape=="grayscale") {
no_gray_quant_update 1 bslbf
composition_method 1 bslbf
linear_composition 1 bslbf
}
quant_type 1 bslbf
if (quant_type) {
load_intra_quant_mat 1 bslbf
if (load_intra_quant_mat)
intra_quant_mat 8*[2-64] uimsbf
load_nonintra_quant_mat 1 bslbf
if (load_nonintra_quant_mat)
nonintra_quant_mat 8*[2-64] uimsbf
if(video_object_layer_shape=="grayscale") {
for(i=0; i<aux_comp_count; i++) {
load_intra_quant_mat_grayscale 1 bslbf
if(load_intra_quant_mat_grayscale)
intra_quant_mat_grayscale[i] 8*[2-64] uimsbf
load_nonintra_quant_mat_grayscale 1 bslbf
if(load_nonintra_quant_mat_grayscale)
nonintra_quant_mat_grayscale[i] 8*[2-64] uimsbf
}
}
}
    if (video_object_layer_verid != '0001')
quarter_sample 1 bslbf
complexity_estimation_disable 1 bslbf
if (!complexity_estimation_disable)
define_vop_complexity_estimation_header()
resync_marker_disable 1 bslbf
data_partitioned 1 bslbf
if(data_partitioned)
reversible_vlc 1 bslbf
if(video_object_layer_verid != '0001') {
newpred_enable 1 bslbf
if (newpred_enable) {
requested_upstream_message_type 2 uimsbf
newpred_segment_type 1 bslbf
}
reduced_resolution_vop_enable 1 bslbf
}
scalability 1 bslbf
if (scalability) {
hierarchy_type 1 bslbf
ref_layer_id 4 uimsbf
ref_layer_sampling_direc 1 bslbf
hor_sampling_factor_n 5 uimsbf
hor_sampling_factor_m 5 uimsbf
vert_sampling_factor_n 5 uimsbf
vert_sampling_factor_m 5 uimsbf
enhancement_type 1 bslbf
    if (video_object_layer_shape == "binary" && hierarchy_type== '0') {
use_ref_shape 1 bslbf
use_ref_texture 1 bslbf
shape_hor_sampling_factor_n 5 uimsbf
shape_hor_sampling_factor_m 5 uimsbf
shape_vert_sampling_factor_n 5 uimsbf
shape_vert_sampling_factor_m 5 uimsbf
}
}
}
else {
if(video_object_layer_verid !="0001") {
scalability 1 bslbf
if(scalability) {
ref_layer_id 4 uimsbf
shape_hor_sampling_factor_n 5 uimsbf
shape_hor_sampling_factor_m 5 uimsbf
shape_vert_sampling_factor_n 5 uimsbf
shape_vert_sampling_factor_m 5 uimsbf
}
}
resync_marker_disable 1 bslbf
}
next_start_code()
while ( next_bits()== user_data_start_code){
user_data()
}
if (sprite_enable == "static" && !low_latency_sprite_enable)
VideoObjectPlane()
do {
if (next_bits() == group_of_vop_start_code)
Group_of_VideoObjectPlane()
VideoObjectPlane()
if ((preceding_vop_coding_type == "B" ||
preceding_vop_coding_type == "S" ||
video_object_layer_shape != "rectangular") &&
next_bits() == stuffing_start_code) {
stuffing_start_code 32 bslbf
while (next_bits() != '0000 0000 0000 0000 0000 0001')
stuffing_byte 8 bslbf
}
} while ((next_bits() == group_of_vop_start_code) ||
(next_bits() == vop_start_code))
}
} else {
short_video_header = 1
do {
video_plane_with_short_header()
} while(next_bits() == short_video_start_marker)
}
}

define_vop_complexity_estimation_header() {
estimation_method 2 uimsbf
    if (estimation_method =='00' || estimation_method == '01') {
shape_complexity_estimation_disable 1 bslbf
if (!shape_complexity_estimation_disable) {
opaque 1 bslbf
transparent 1 bslbf
intra_cae 1 bslbf
inter_cae 1 bslbf
no_update 1 bslbf
upsampling 1 bslbf
}
texture_complexity_estimation_set_1_disable 1 bslbf
if (!texture_complexity_estimation_set_1_disable) {
intra_blocks 1 bslbf
inter_blocks 1 bslbf
inter4v_blocks 1 bslbf
not_coded_blocks 1 bslbf
}
marker_bit 1 bslbf
texture_complexity_estimation_set_2_disable 1 bslbf
if (!texture_complexity_estimation_set_2_disable) {
dct_coefs 1 bslbf
dct_lines 1 bslbf
vlc_symbols 1 bslbf
vlc_bits 1 bslbf
}
motion_compensation_complexity_disable 1 bslbf
if (!motion_compensation_complexity_disable) {
apm 1 bslbf
npm 1 bslbf
interpolate_mc_q 1 bslbf
forw_back_mc_q 1 bslbf
halfpel2 1 bslbf
halfpel4 1 bslbf
}
marker_bit 1 bslbf
        if (estimation_method == '01') {
version2_complexity_estimation_disable 1 bslbf
if (!version2_complexity_estimation_disable) {
sadct 1 bslbf
quarterpel 1 bslbf
}
}
}
}

Group_of_VideoObjectPlane() {
group_of_vop_start_code 32 bslbf
time_code 18
closed_gov 1 bslbf
broken_link 1 bslbf
next_start_code()
while ( next_bits()== user_data_start_code){
user_data()
}
}

VideoObjectPlane() {
vop_start_code 32 bslbf
vop_coding_type 2 uimsbf
do {
modulo_time_base 1 bslbf
} while (modulo_time_base != '0')
marker_bit 1 bslbf
vop_time_increment 1-16 uimsbf
marker_bit 1 bslbf
vop_coded 1 bslbf
if (vop_coded == '0') {
next_start_code()
return()
}
if (newpred_enable) {
vop_id 4-15 uimsbf
vop_id_for_prediction_indication 1 bslbf
if (vop_id_for_prediction_indication)
vop_id_for_prediction 4-15 uimsbf
marker_bit 1 bslbf
}
if ((video_object_layer_shape != "binary only") &&
(vop_coding_type == "P" ||
(vop_coding_type == "S" && sprite_enable == "GMC")))
vop_rounding_type 1 bslbf
if ((reduced_resolution_vop_enable) &&
(video_object_layer_shape == "rectangular") &&
((vop_coding_type == "P") || (vop_coding_type == "I")))
vop_reduced_resolution 1 bslbf
if (video_object_layer_shape != "rectangular") {
if(!(sprite_enable == "static" && vop_coding_type == "I")) {
vop_width 13 uimsbf
marker_bit 1 bslbf
vop_height 13 uimsbf
marker_bit 1 bslbf
vop_horizontal_mc_spatial_ref 13 simsbf
marker_bit 1 bslbf
vop_vertical_mc_spatial_ref 13 simsbf
marker_bit 1 bslbf
}
if ((video_object_layer_shape != "binary only") &&
scalability && enhancement_type)
background_composition 1 bslbf
change_conv_ratio_disable 1 bslbf
vop_constant_alpha 1 bslbf
if (vop_constant_alpha)
vop_constant_alpha_value 8 bslbf
}
if (video_object_layer_shape != "binary only")
if (!complexity_estimation_disable)
read_vop_complexity_estimation_header()
if (video_object_layer_shape != "binary only") {
intra_dc_vlc_thr 3 uimsbf
if (interlaced) {
top_field_first 1 bslbf
alternate_vertical_scan_flag 1 bslbf
}
}
if ((sprite_enable =="static" || sprite_enable=="GMC") &&
vop_coding_type == "S") {
if (no_of_sprite_warping_points > 0)
sprite_trajectory()
if (sprite_brightness_change)
brightness_change_factor()
if(sprite_enable == "static") {
if (sprite_transmit_mode != "stop"
&& low_latency_sprite_enable) {
do {
sprite_transmit_mode 2 uimsbf
if ((sprite_transmit_mode == "piece") ||
(sprite_transmit_mode == "update"))
decode_sprite_piece()
} while (sprite_transmit_mode != "stop" &&
sprite_transmit_mode != "pause")
}
next_start_code()
return()
}
}
if (video_object_layer_shape != "binary only") {
vop_quant 3-9 uimsbf
if(video_object_layer_shape=="grayscale")
for(i=0; i<aux_comp_count; i++)
vop_alpha_quant[i] 6 uimsbf
if (vop_coding_type != "I")
vop_fcode_forward 3 uimsbf
if (vop_coding_type == "B")
vop_fcode_backward 3 uimsbf
if (!scalability) {
if (video_object_layer_shape != "rectangular"
&& vop_coding_type != "I")
vop_shape_coding_type 1 bslbf
motion_shape_texture()
while (nextbits_bytealigned() == resync_marker) {
video_packet_header()
motion_shape_texture()
}
}
else {
if (enhancement_type) {
load_backward_shape 1 bslbf
if (load_backward_shape) {
backward_shape_width 13 uimsbf
marker_bit 1 bslbf
backward_shape_height 13 uimsbf
marker_bit 1 bslbf
backward_shape_horizontal_mc_spatial_ref 13 simsbf
marker_bit 1 bslbf
backward_shape_vertical_mc_spatial_ref 13 simsbf
backward_shape()
load_forward_shape 1 bslbf
if (load_forward_shape) {
forward_shape_width 13 uimsbf
marker_bit 1 bslbf
forward_shape_height 13 uimsbf
marker_bit 1 bslbf
forward_shape_horizontal_mc_spatial_ref 13 simsbf
marker_bit 1 bslbf
forward_shape_vertical_mc_spatial_ref 13 simsbf
forward_shape()
}
}
}
ref_select_code 2 uimsbf
combined_motion_shape_texture()
}
}
else {
combined_motion_shape_texture()
while (nextbits_bytealigned() == resync_marker) {
video_packet_header()
combined_motion_shape_texture()
}
}
next_start_code()
}

read_vop_complexity_estimation_header() {
if (estimation_method=='00' || estimation_method=='01') {
if (vop_coding_type=="I") {
if (opaque) dcecs_opaque 8 uimsbf
if (transparent) dcecs_transparent 8 uimsbf
if (intra_cae) dcecs_intra_cae 8 uimsbf
if (inter_cae) dcecs_inter_cae 8 uimsbf
if (no_update) dcecs_no_update 8 uimsbf
if (upsampling) dcecs_upsampling 8 uimsbf
if (intra_blocks) dcecs_intra_blocks 8 uimsbf
if (not_coded_blocks) dcecs_not_coded_blocks 8 uimsbf
if (dct_coefs) dcecs_dct_coefs 8 uimsbf
if (dct_lines) dcecs_dct_lines 8 uimsbf
if (vlc_symbols) dcecs_vlc_symbols 8 uimsbf
if (vlc_bits) dcecs_vlc_bits 4 uimsbf
if (sadct) dcecs_sadct 8 uimsbf
}
if (vop_coding_type=="P") {
if (opaque) dcecs_opaque 8 uimsbf
if (transparent) dcecs_transparent 8 uimsbf
if (intra_cae) dcecs_intra_cae 8 uimsbf
if (inter_cae) dcecs_inter_cae 8 uimsbf
if (no_update) dcecs_no_update 8 uimsbf
if (upsampling) dcecs_upsampling 8 uimsbf
if (intra_blocks) dcecs_intra_blocks 8 uimsbf
if (not_coded) dcecs_not_coded_blocks 8 uimsbf
if (dct_coefs) dcecs_dct_coefs 8 uimsbf
if (dct_lines) dcecs_dct_lines 8 uimsbf
if (vlc_symbols) dcecs_vlc_symbols 8 uimsbf
if (vlc_bits) dcecs_vlc_bits 4 uimsbf
if (inter_blocks) dcecs_inter_blocks 8 uimsbf
if (inter4v_blocks) dcecs_inter4v_blocks 8 uimsbf
if (apm) dcecs_apm 8 uimsbf
if (npm) dcecs_npm 8 uimsbf
if (forw_back_mc_q) dcecs_forw_back_mc_q 8 uimsbf
if (halfpel2) dcecs_halfpel2 8 uimsbf
if (halfpel4) dcecs_halfpel4 8 uimsbf
if (sadct) dcecs_sadct 8 uimsbf
if (quarterpel) dcecs_quarterpel 8 uimsbf
}
if (vop_coding_type=="B") {
if (opaque) dcecs_opaque 8 uimsbf
if (transparent) dcecs_transparent 8 uimsbf
if (intra_cae) dcecs_intra_cae 8 uimsbf
if (inter_cae) dcecs_inter_cae 8 uimsbf
if (no_update) dcecs_no_update 8 uimsbf
if (upsampling) dcecs_upsampling 8 uimsbf
if (intra_blocks) dcecs_intra_blocks 8 uimsbf
if (not_coded_blocks) dcecs_not_coded_blocks 8 uimsbf
if (dct_coefs) dcecs_dct_coefs 8 uimsbf
if (dct_lines) dcecs_dct_lines 8 uimsbf
if (vlc_symbols) dcecs_vlc_symbols 8 uimsbf
if (vlc_bits) dcecs_vlc_bits 4 uimsbf
if (inter_blocks) dcecs_inter_blocks 8 uimsbf
if (inter4v_blocks) dcecs_inter4v_blocks 8 uimsbf
if (apm) dcecs_apm 8 uimsbf
if (npm) dcecs_npm 8 uimsbf
if (forw_back_mc_q) dcecs_forw_back_mc_q 8 uimsbf
if (halfpel2) dcecs_halfpel2 8 uimsbf
if (halfpel4) dcecs_halfpel4 8 uimsbf
if (interpolate_mc_q) dcecs_interpolate_mc_q 8 uimsbf
if (sadct) dcecs_sadct 8 uimsbf
if (quarterpel) dcecs_quarterpel 8 uimsbf
}
    if (vop_coding_type=='S'&& sprite_enable == "static") {
if (intra_blocks) dcecs_intra_blocks 8 uimsbf
if (not_coded_blocks) dcecs_not_coded_blocks 8 uimsbf
if (dct_coefs) dcecs_dct_coefs 8 uimsbf
if (dct_lines) dcecs_dct_lines 8 uimsbf
if (vlc_symbols) dcecs_vlc_symbols 8 uimsbf
if (vlc_bits) dcecs_vlc_bits 4 uimsbf
if (inter_blocks) dcecs_inter_blocks 8 uimsbf
if (inter4v_blocks) dcecs_inter4v_blocks 8 uimsbf
if (apm) dcecs_apm 8 uimsbf
if (npm) dcecs_npm 8 uimsbf
if (forw_back_mc_q) dcecs_forw_back_mc_q 8 uimsbf
if (halfpel2) dcecs_halfpel2 8 uimsbf
if (halfpel4) dcecs_halfpel4 8 uimsbf
if (interpolate_mc_q) dcecs_interpolate_mc_q 8 uimsbf
}
}
}

video_plane_with_short_header() {
short_video_start_marker 22 bslbf
temporal_reference 8 uimsbf
marker_bit 1 bslbf
zero_bit 1 bslbf
split_screen_indicator 1 bslbf
document_camera_indicator 1 bslbf
full_picture_freeze_release 1 bslbf
source_format 3 bslbf
picture_coding_type 1 bslbf
four_reserved_zero_bits 4 bslbf
vop_quant 5 uimsbf
zero_bit 1 bslbf
do{
pei 1 bslbf
if (pei == "1")
psupp 8 bslbf
} while (pei == "1")
gob_number = 0
for(i=0; i<num_gobs_in_vop; i++)
gob_layer()
if(next_bits() == short_video_end_marker)
short_video_end_marker 22 uimsbf
while(!bytealigned())
zero_bit 1 bslbf
}
