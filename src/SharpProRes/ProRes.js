frame() {
frame_size u(32)
frame_identifier f(32)
frame_header()
picture("first")
if (interlace_mode == 1 || interlace_mode == 2)
picture("second")
if (stuffing_size > 0)
stuffing()
}

frame_header() {
frame_header_size u(16)
reserved u(8)
bitstream_version u(8)
encoder_identifier f(32)
horizontal_size u(16)
vertical_size u(16)
chroma_format u(2)
reserved u(2)
interlace_mode u(2)
reserved u(2)
aspect_ratio_information u(4)
frame_rate_code u(4)
color_primaries u(8)
transfer_characteristic u(8)
matrix_coefficients u(8)
reserved u(4)
alpha_channel_type u(4)
reserved u(14)
load_luma_quantization_matrix u(1)
load_chroma_quantization_matrix u(1)
if (load_luma_quantization_matrix) {
for (v = 0; v < 8; v++)
for (u = 0; u < 8; u++)
luma_quantization_matrix[v][u] u(8)
}
if (load_chroma_quantization_matrix) {
for (v = 0; v < 8; v++)
for (u = 0; u < 8; u++)
chroma_quantization_matrix[v][u] u(8)
}
}

stuffing() {
for (m = 0; m < stuffing_size; m++)
zero_byte /* Equal to 0x00 */ f(8)
}

picture(temporalOrder) {
picture_header()
slice_table()
for (i = 0; i < height_in_mb; i++)
for (j = 0; j < number_of_slices_per_mb_row; j++)
slice(i, j)
}

picture_header() {
picture_header_size u(5)
reserved u(3)
picture_size u(32)
deprecated_number_of_slices u(16)
reserved u(2)
log2_desired_slice_size_in_mb u(2)
reserved u(4)
}

slice_table () {
for (i = 0; i < height_in_mb; i++)
for (j = 0; j < number_of_slices_per_mb_row; j++)
coded_size_of_slice[i][j] u(16)
}
