sequence_header() {
sequence_header_code 32 bslbf
horizontal_size_value 12 uimsbf
vertical_size_value 12 uimsbf
aspect_ratio_information 4 uimsbf
frame_rate_code 4 uimsbf
bit_rate_value 18 uimsbf
marker_bit 1 bslbf
vbv_buffer_size_value 10 uimsbf
constrained_parameters_flag 1 bslbf
load_intra_quantiser_matrix 1 uimsbf
if ( load_intra_quantiser_matrix )
intra_quantiser_matrix[64] 8*64 uimsbf
load_non_intra_quantiser_matrix 1 uimsbf
if ( load_non_intra_quantiser_matrix )
non_intra_quantiser_matrix[64] 8*64 uimsbf
next_start_code()
}

extension_data( i ) {
while ( nextbits()== extension_start_code ) {
extension_start_code 32 bslbf
if (i == 0) { /* follows sequence_extension() */
if ( nextbits()== "Sequence Display Extension ID" )
sequence_display_extension()
else if ( nextbits()
== "Sequence Scalable Extension ID" )
sequence_scalable_extension()
else
while ( nextbits() != '0000 0000 0000 0000 0000 0001' )
reserved_extension_data_byte 8 uimsbf
}
/* NOTE – i never takes the value 1 because extension_data()
never follows a group_of_pictures_header() */
if (i == 2) { /* follows picture_coding_extension() */
if ( nextbits() == "Quant Matrix Extension ID" )
quant_matrix_extension()
else if ( nextbits() == "Copyright Extension ID" )
copyright_extension()
else if ( nextbits() == "Picture Display Extension ID" )
picture_display_extension()
else if ( nextbits()
== "Picture Spatial Scalable Extension ID" )
picture_spatial_scalable_extension()
else if ( nextbits()
== "Picture Temporal Scalable Extension ID" )
picture_temporal_scalable_extension()
else if ( nextbits()
== "Camera Parameters Extension ID" )
camera_parameters_extension()
else if ( nextbits()
== "ITU-T Extension ID" )
ITU-T_extension()
else
while ( nextbits() != '0000 0000 0000 0000 0000 0001' )
reserved_extension_data_byte 8 uimsbf
}
}
}

user_data() {
user_data_start_code 32 bslbf
while( nextbits() != '0000 0000 0000 0000 0000 0001' ) {
user_data 8 uimsbf
}
next_start_code()
}

sequence_extension() {
extension_start_code 32 bslbf
extension_start_code_identifier 4 uimsbf
profile_and_level_indication 8 uimsbf
progressive_sequence 1 uimsbf
chroma_format 2 uimsbf
horizontal_size_extension 2 uimsbf
vertical_size_extension 2 uimsbf
bit_rate_extension 12 uimsbf
marker_bit 1 bslbf
vbv_buffer_size_extension 8 uimsbf
low_delay 1 uimsbf
frame_rate_extension_n 2 uimsbf
frame_rate_extension_d 5 uimsbf
next_start_code()
}

sequence_display_extension() {
extension_start_code_identifier 4 uimsbf
video_format 3 uimsbf
colour_description 1 uimsbf
if ( colour_description ) {
colour_primaries 8 uimsbf
transfer_characteristics 8 uimsbf
matrix_coefficients 8 uimsbf
}
display_horizontal_size 14 uimsbf
marker_bit 1 bslbf
display_vertical_size 14 uimsbf
next_start_code()
}

sequence_scalable_extension() {
extension_start_code_identifier 4 uimsbf
scalable_mode 2 uimsbf
layer_id 4 uimsbf
if (scalable_mode == "spatial scalability") {
lower_layer_prediction_horizontal_size 14 uimsbf
marker_bit 1 bslbf
lower_layer_prediction_vertical_size 14 uimsbf
horizontal_subsampling_factor_m 5 uimsbf
horizontal_subsampling_factor_n 5 uimsbf
vertical_subsampling_factor_m 5 uimsbf
vertical_subsampling_factor_n 5 uimsbf
}
if ( scalable_mode == "temporal scalability" ) {
picture_mux_enable 1 uimsbf
if ( picture_mux_enable )
mux_to_progressive_sequence 1 uimsbf
picture_mux_order 3 uimsbf
picture_mux_factor 3 uimsbf
}
next_start_code()
}

group_of_pictures_header() {
group_start_code 32 bslbf
time_code 25 uimsbf
closed_gop 1 uimsbf
broken_link 1 uimsbf
next_start_code()
}

picture_header() {
picture_start_code 32 bslbf
temporal_reference 10 uimsbf
picture_coding_type 3 uimsbf
vbv_delay 16 uimsbf
if ( picture_coding_type == 2 || picture_coding_type == 3) {
full_pel_forward_vector 1 bslbf
forward_f_code 3 bslbf
}
if ( picture_coding_type == 3 ) {
full_pel_backward_vector 1 bslbf
backward_f_code 3 bslbf
}
while ( nextbits() == '1' ) {
extra_bit_picture /* with the value '1' */ 1 uimsbf
content_description_data() /* with every 9th bit having the value '1' */ 8 uimsbf
}
extra_bit_picture/* with the value '0' */ 1 uimsbf
next_start_code()
}

picture_coding_extension() {
extension_start_code 32 bslbf
extension_start_code_identifier 4 uimsbf
f_code[0][0] /* forward horizontal */ 4 uimsbf
f_code[0][1] /* forward vertical */ 4 uimsbf
f_code[1][0] /* backward horizontal */ 4 uimsbf
f_code[1][1] /* backward vertical */ 4 uimsbf
intra_dc_precision 2 uimsbf
picture_structure 2 uimsbf
top_field_first 1 uimsbf
frame_pred_frame_dct 1 uimsbf
concealment_motion_vectors 1 uimsbf
q_scale_type 1 uimsbf
intra_vlc_format 1 uimsbf
alternate_scan 1 uimsbf
repeat_first_field 1 uimsbf
chroma_420_type 1 uimsbf
progressive_frame 1 uimsbf
composite_display_flag 1 uimsbf
if ( composite_display_flag ) {
v_axis 1 uimsbf
field_sequence 3 uimsbf
sub_carrier 1 uimsbf
burst_amplitude 7 uimsbf
sub_carrier_phase 8 uimsbf
}
next_start_code()
}

quant_matrix_extension() {
extension_start_code_identifier 4 uimsbf
load_intra_quantiser_matrix 1 uimsbf
if ( load_intra_quantiser_matrix )
intra_quantiser_matrix[64] 8 * 64 uimsbf
load_non_intra_quantiser_matrix 1 uimsbf
if ( load_non_intra_quantiser_matrix )
non_intra_quantiser_matrix[64] 8 * 64 uimsbf
load_chroma_intra_quantiser_matrix 1 uimsbf
if ( load_chroma_intra_quantiser_matrix )
chroma_intra_quantiser_matrix[64] 8 * 64 uimsbf
load_chroma_non_intra_quantiser_matrix 1 uimsbf
if ( load_chroma_non_intra_quantiser_matrix )
chroma_non_intra_quantiser_matrix[64] 8 * 64 uimsbf
next_start_code()
}

picture_display_extension() {
extension_start_code_identifier 4 uimsbf
for ( i = 0; i < number_of_frame_centre_offsets; i ++ ) {
frame_centre_horizontal_offset 16 simsbf
marker_bit 1 bslbf
frame_centre_vertical_offset 16 simsbf
marker_bit 1 bslbf
}
next_start_code()
}

picture_temporal_scalable_extension() {
extension_start_code_identifier 4 uimsbf
reference_select_code 2 uimsbf
forward_temporal_reference 10 uimsbf
marker_bit 1 bslbf
backward_temporal_reference 10 uimsbf
next_start_code()
}

picture_spatial_scalable_extension() {
extension_start_code_identifier 4 uimsbf
lower_layer_temporal_reference 10 uimsbf
marker_bit 1 bslbf
lower_layer_horizontal_offset 15 simsbf
marker_bit 1 bslbf
lower_layer_vertical_offset 15 simsbf
spatial_temporal_weight_code_table_index 2 uimsbf
lower_layer_progressive_frame 1 uimsbf
lower_layer_deinterlaced_field_select 1 uimsbf
next_start_code()
}

copyright_extension() {
extension_start_code_identifier 4 uimsbf
copyright_flag 1 uimsbf
copyright_identifier 8 uimsbf
original_or_copy 1 uimsbf
reserved 7 bslbf
marker_bit 1 bslbf
copyright_number_1 20 uimsbf
marker_bit 1 bslbf
copyright_number_2 22 uimsbf
marker_bit 1 bslbf
copyright_number_3 22 uimsbf
next_start_code()
}

camera_parameters_extension() {
extension_start_code_identifier 4 uimsbf
reserved 1 uimsbf
camera_id 7 simsbf
marker_bit 1 bslbf
height_of_image_device 22 uimsbf
marker_bit 1 bslbf
focal_length 22 uimsbf
marker_bit 1 bslbf
f_number 22 uimsbf
marker_bit 1 bslbf
vertical_angle_of_view 22 uimsbf
marker_bit 1 bslbf
camera_position_x_upper 16 simsbf
marker_bit 1 bslbf
camera_position_x_lower 16
marker_bit 1 bslbf
camera_position_y_upper 16 simsbf
marker_bit 1 bslbf
camera_position_y_lower 16
marker_bit 1 bslbf
camera_position_z_upper 16 simsbf
marker_bit 1 bslbf
camera_position_z_lower 16
marker_bit 1 bslbf
camera_direction_x 22 simsbf
marker_bit 1 bslbf
camera_direction_y 22 simsbf
marker_bit 1 bslbf
camera_direction_z 22 simsbf
marker_bit 1 bslbf
image_plane_vertical_x 22 simsbf
marker_bit 1 bslbf
image_plane_vertical_y 22 simsbf
marker_bit 1 bslbf
image_plane_vertical_z 22 simsbf
marker_bit 1 bslbf
reserved 32 bslbf
next_start_code()
}

ITU-T_extension() {
extension_start_code_identifier 4 uimsbf
while( nextbits() != '0000 0000 0000 0000 0000 0001' ) {
ITU-T_data 1 uimsbf
}
next_start_code()
}

content_description_data() {
data_type_upper 8 uimsbf
marker_bit 1 bslbf
data_type_lower 8
marker_bit 1 bslbf
data_length 8 uimsbf
if ( data_type == "Padding Bytes" )
padding_bytes()
else if ( data_type == "Capture Timecode" )
capture_timecode()
else if ( data_type == "Additional Pan-Scan Parameters" )
additional_pan_scan_parameters()
else if ( data_type == "Active Region Window" )
active_region_window()
else if ( data_type == "Coded Picture Length" )
coded_picture_length()
else
for ( i = 0; i < data_length; i ++ ) {
marker_bit 1 bslbf
reserved_content_description_data 8 uimsbf
}
}

padding_bytes() {
for ( i = 0; i < data_length; i ++ ) {
marker_bit 1 bslbf
padding_byte 8 bslbf
}
}

capture_timecode() {
marker_bit 1 bslbf
timecode_type 2 uimsbf
counting_type 3 uimsbf
reserved_bit 1 uimsbf
reserved_bit 1 uimsbf
reserved_bit 1 uimsbf
if ( counting_type != 0 ) {
marker_bit 1 bslbf
nframes_conversion_code 1 uimsbf
clock_divisor 7 uimsbf
marker_bit 1 bslbf
nframes_multiplier_upper 8 uimsbf
marker_bit 1 bslbf
nframes_multiplier_lower 8
}
frame_or_field_capture_timestamp()
if ( timecode_type == '11' )
frame_or_field_capture_timestamp()
}

frame_or_field_capture_timestamp() {
if ( counting_type != 0 ) {
marker_bit 1 bslbf
nframes 8 uimsbf
}
marker_bit 1 bslbf
time_discontinuity 1 uimsbf
prior_count_dropped 1 uimsbf
time_offset_part_a 6 simsbf
marker_bit 1 bslbf
time_offset_part_b 8
marker_bit 1 bslbf
time_offset_part_c 8
marker_bit 1 bslbf
time_offset_part_d 8
marker_bit 1 bslbf
units_of_seconds 4 uimsbf
tens_of_seconds 4 uimsbf
marker_bit 1 bslbf
units_of_minutes 4 uimsbf
tens_of_minutes 4 uimsbf
marker_bit 1 bslbf
units_of_hours 4 uimsbf
tens_of_hours 4 uimsbf
}

additional_pan_scan_parameters() {
marker_bit 1 bslbf
aspect_ratio_information 4 uimsbf
reserved_bit 1 bslbf
reserved_bit 1 bslbf
reserved_bit 1 bslbf
display_size_present 1 bslbf
if (display_size_present == '1' ) {
marker_bit 1 bslbf
reserved_bit 1 bslbf
reserved_bit 1 bslbf
display_horizontal_size_upper 6 uimsbf
marker_bit 1 bslbf
display_horizontal_size_lower 8
marker_bit 1 bslbf
reserved_bit 1 bslbf
reserved_bit 1 bslbf
display_vertical_size_upper 6 uimsbf
marker_bit 1 bslbf
display_vertical_size_lower 8
}
for ( i = 0; i < number_of_frame_centre_offsets; i ++ ) {
marker_bit 1 bslbf
frame_centre_horizontal_offset_upper 8 simsbf
marker_bit 1 bslbf
frame_centre_horizontal_offset_lower 8
marker_bit 1 bslbf
frame_centre_vertical_offset_upper 8 simsbf
marker_bit 1 bslbf
frame_centre_vertical_offset_lower 8
}
}

active_region_window() {
marker_bit 1 bslbf
top_left_x_upper 8 uimsbf
marker_bit 1 bslbf
top_left_x_lower 8
marker_bit 1 bslbf
top_left_y_upper 8 uimsbf
marker_bit 1 bslbf
top_left_y_lower 8
marker_bit 1 bslbf
active_horizontal_size_upper 8 uimsbf
marker_bit 1 bslbf
active_horizontal_size_lower 8
marker_bit 1 bslbf
active_vertical_size_upper 8 uimsbf
marker_bit 1 bslbf
active_vertical_size_lower 8
}

coded_picture_length() {
marker_bit 1 bslbf
picture_byte_count_part_a 8 uimsbf
marker_bit 1 bslbf
picture_byte_count_part_b 8
marker_bit 1 bslbf
picture_byte_count_part_c 8
marker_bit 1 bslbf
picture_byte_count_part_d 8
}

slice() {
slice_start_code 32 bslbf
if (vertical_size > 2800)
slice_vertical_position_extension 3 uimsbf
if (<sequence_scalable_extension() is present in the bitstream>) {
if (scalable_mode == 'data partitioning')
priority_breakpoint 7 uimsbf
}
quantiser_scale_code 5 uimsbf
if (nextbits() == '1') {
slice_extension_flag 1 bslbf
intra_slice 1 uimsbf
slice_picture_id_enable 1 uimsbf
slice_picture_id 6 uimbsf
while (nextbits() == '1') {
extra_bit_slice /* with the value '1' */ 1 uimsbf
extra_information_slice 8 uimsbf
}
}
extra_bit_slice /* with the value '0' */ 1 uimsbf
do {
macroblock()
} while (nextbits() != '000 0000 0000 0000 0000 0000')
next_start_code()
}
