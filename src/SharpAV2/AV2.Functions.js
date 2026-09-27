compute_image_size ( ) {
/* AV2 specification v1.0.0, 5.18.4.4 Compute image size function */
MiCols = 2 * ( ( FrameWidth + 7 ) >> 3 )
MiRows = 2 * ( ( FrameHeight + 7 ) >> 3 )
maxFrameWidth = max_frame_width_minus_1 + 1
maxFrameHeight = max_frame_height_minus_1 + 1
CropLeft = ( seq_cropping_win_left_offset * FrameWidth ) / maxFrameWidth
cropRight = FrameWidth - (( seq_cropping_win_right_offset * FrameWidth ) /
maxFrameWidth )
CropTop = ( seq_cropping_win_top_offset * FrameHeight ) / maxFrameHeight
cropBottom = FrameHeight - (( seq_cropping_win_bottom_offset * FrameHeight ) /
maxFrameHeight )
CropWidth = cropRight - CropLeft
CropHeight = cropBottom - CropTop
}

get_disp_order_hint ( ) {
/* AV2 specification v1.0.0, 5.18.2 Frame header info syntax */
if ( obu_type == OBU_CLOSED_LOOP_KEY ||
( ! is_sef () && FrameType == SWITCH_FRAME &&
restricted_prediction_switch ) ) {
return OrderHintLsbs
}
maxDisp = get_max_disp_order_hint ( 1 )
dispOrderHint = OrderHintLsbs
offset = maxDisp - (( 1 << OrderHintBits ) >> 1 ) - OrderHintLsbs
if ( offset >= 0 ) {
dispOrderHint += (( offset >> OrderHintBits ) + 1 ) << OrderHintBits
}
return dispOrderHint
}

get_relative_dist ( a , b ) {
/* AV2 specification v1.0.0, 5.18.3.1 Get relative distance function */
if ( a == RESTRICTED_OH && b == RESTRICTED_OH ) {
return 0
} else if ( a == RESTRICTED_OH ) {
return 127
} else if ( b == RESTRICTED_OH ) {
return -127
} else {
return Clip3 ( -127 , 127 , a - b )
}
}

get_seq_sb_size () {
/* AV2 specification v1.0.0, 5.18.7.6 Get sequence superblock size function */
if ( use_256x256_superblock ) {
return BLOCK_256X256
} else if ( use_128x128_superblock ) {
return BLOCK_128X128
} else {
return BLOCK_64X64
}
}

inverse_recenter ( r , v ) {
/* AV2 specification v1.0.0, 5.18.9.6 Inverse recenter function */
if ( v > 2 * r ) {
return v
} else if ( v & 1 ) {
return r - (( v + 1 ) >> 1 )
} else {
return r + ( v >> 1 )
}
}

is_extensible_obu () {
/* AV2 specification v1.0.0, 5.2.1 General OBU syntax */
return obu_type == OBU_SEQUENCE_HEADER ||
obu_type == OBU_MULTI_FRAME_HEADER ||
obu_type == OBU_LAYER_CONFIGURATION_RECORD ||
obu_type == OBU_CONTENT_INTERPRETATION ||
obu_type == OBU_OPERATING_POINT_SET ||
obu_type == OBU_ATLAS_SEGMENT
}

is_sef () {
/* AV2 specification v1.0.0, 5.2.1 General OBU syntax */
return obu_type == OBU_LEADING_SEF || obu_type == OBU_REGULAR_SEF
}

is_tile_group () {
/* AV2 specification v1.0.0, 5.2.1 General OBU syntax */
return obu_type == OBU_LEADING_TILE_GROUP ||
obu_type == OBU_REGULAR_TILE_GROUP ||
obu_type == OBU_CLOSED_LOOP_KEY ||
obu_type == OBU_OPEN_LOOP_KEY ||
obu_type == OBU_SWITCH ||
obu_type == OBU_RAS_FRAME
}

is_tip_frame () {
/* AV2 specification v1.0.0, 5.2.1 General OBU syntax */
return obu_type == OBU_LEADING_TIP || obu_type == OBU_REGULAR_TIP
}

load_xlayer_context ( obu_xlayer_id ) {
/* AV2 specification v1.0.0, 7.6 Extended layer context management */
if ( obu_xlayer_id == GLOBAL_XLAYER_ID )
return
if ( MultiStreamDecoderMode ) {
for ( i = 0 ; i < num_streams_minus_2 + 2 ; i ++ ) {
if ( sub_xlayer_id [ i ] == obu_xlayer_id ) {
streamID = i
break
}
}
} else {
streamID = obu_xlayer_id
}
load_context ( streamID )
}

long_term_id_in_use ( longTermId ) {
/* AV2 specification v1.0.0, 5.18.2 Frame header info syntax */
for ( j = 0 ; j < num_key_ref_frames ; j ++ ) {
if ( longTermId == ref_long_term_id [ j ] ) {
return 1
}
}
return 0
}

reuse_tile_params ( uniformSpacing , sbRowStarts , tileRows , tileRowsLog2 , sbColStarts , tileCols , tileColsLog2 , seqSbSize , sbSize ) {
/* AV2 specification v1.0.0, 5.18.7.4 Reuse tile params function */
if ( uniformSpacing ) {
sbShift = Mi_Width_Log2 [ seqSbSize ]
( unifSbColStarts , tileCols ) = uniform_spacing ( tileColsLog2 , MiCols ,
seqSbSize )
( unifSbRowStarts , tileRows ) = uniform_spacing ( tileRowsLog2 , MiRows ,
seqSbSize )
tileColsLog2 = tile_log2 ( 1 , tileCols )
tileRowsLog2 = tile_log2 ( 1 , tileRows )
return ( unifSbRowStarts , tileRows , tileRowsLog2 , unifSbColStarts ,
tileCols , tileColsLog2 , sbShift )
} else {
sbShift = Mi_Width_Log2 [ sbSize ]
tileColsLog2 = tile_log2 ( 1 , tileCols )
tileRowsLog2 = tile_log2 ( 1 , tileRows )
return ( sbRowStarts , tileRows , tileRowsLog2 , sbColStarts , tileCols ,
tileColsLog2 , sbShift )
}
}

save_xlayer_context ( obu_xlayer_id ) {
/* AV2 specification v1.0.0, 7.6 Extended layer context management */
if ( obu_xlayer_id == GLOBAL_XLAYER_ID )
return
if ( MultiStreamDecoderMode ) {
for ( i = 0 ; i < num_streams_minus_2 + 2 ; i ++ ) {
if ( sub_xlayer_id [ i ] == obu_xlayer_id ) {
streamID = i
break
}
}
} else {
streamID = obu_xlayer_id
}
save_context ( streamID )
}

scale_warp_model ( baseParams , baseDistance , dist ) {
/* AV2 specification v1.0.0, 5.18.9.1 Global motion params syntax */
if ( baseDistance == 0 ) {
return Default_Warp_Params
}
if ( baseDistance < 0 ) {
baseDistance = - baseDistance
dist = - dist
}
for ( i = 0 ; i < 6 ; i ++ ) {
center = Default_Warp_Params [ i ]
limit = ( 1 << 22 ) - 1
input = Clip3 ( - limit , limit , baseParams [ i ] - center )
( divShift , divFactor ) = resolve_divisor ( baseDistance )
scaled = Round2Signed ( input * divFactor , divShift )
output = Round2Signed ( scaled * dist , Param_Shift [ i ] )
output = Clip3 ( Param_Min [ i ], Param_Max [ i ], output ) << Param_Shift [ i ]
params [ i ] = center + output
}
return params
}

set_chroma_format_and_bit_depth ( ) {
/* AV2 specification v1.0.0, 6.4.1 General sequence header OBU semantics */
if ( chroma_format_idc == CHROMA_FORMAT_420 ) {
SubsamplingX = 1
SubsamplingY = 1
} else if ( chroma_format_idc == CHROMA_FORMAT_444 ) {
SubsamplingX = 0
SubsamplingY = 0
} else if ( chroma_format_idc == CHROMA_FORMAT_422 ) {
SubsamplingX = 1
SubsamplingY = 0
} else if ( chroma_format_idc == CHROMA_FORMAT_400 ) {
SubsamplingX = 1
SubsamplingY = 1
}
BitDepth = lookup_bitdepth ( bit_depth_idc )
MaxQ = lookup_maxq ( bit_depth_idc )
Monochrome = chroma_format_idc == CHROMA_FORMAT_400
NumPlanes = Monochrome ? 1 : 3
}

set_primary_ref_frame_and_ctx ( loadCdfs ) {
/* AV2 specification v1.0.0, 5.18.2 Frame header info syntax */
( DerivedPrimaryRefFrame , derivedSecondaryRefFrame ) =
choose_primary_secondary_ref_frame ()
if ( primary_ref_frame == PRIMARY_REF_CHOOSE ) {
primary_ref_frame = DerivedPrimaryRefFrame
}
if ( DerivedPrimaryRefFrame == PRIMARY_REF_NONE ||
primary_ref_frame == PRIMARY_REF_NONE ) {
primary_ref_frame = PRIMARY_REF_NONE
DerivedPrimaryRefFrame = PRIMARY_REF_NONE
disable_cross_frame_cdf_init = 1
}
if ( ! loadCdfs ) {
return
}
if ( primary_ref_frame == PRIMARY_REF_NONE ||
disable_cross_frame_cdf_init ) {
init_non_coeff_cdfs ( )
} else {
load_cdfs ( ref_frame_idx [ primary_ref_frame ] )
if ( TipFrameMode != TIP_FRAME_AS_OUTPUT ) {
blendFrame = ( primary_ref_frame == DerivedPrimaryRefFrame ) ?
derivedSecondaryRefFrame : DerivedPrimaryRefFrame
if ( enable_avg_cdf && ! avg_cdf_type &&
blendFrame != PRIMARY_REF_NONE &&
! bru_inactive ) {
blend_cdfs ( ref_frame_idx [ blendFrame ] )
}
}
}
if ( DerivedPrimaryRefFrame == PRIMARY_REF_NONE ) {
setup_past_independence ( )
} else {
load_previous ( )
}
}

tile_log2 ( blkSize , target ) {
/* AV2 specification v1.0.0, 5.18.7.7 Tile size calculation function */
for ( k = 0 ; ( blkSize << k ) < target ; k ++ ) {
}
return k
}

uniform_eligible ( tileLog2 , sbNum ) {
/* AV2 specification v1.0.0, 5.18.7.2 Tile info syntax */
tileNum = 1 << tileLog2
tileWidth = ( sbNum + tileNum - 1 ) >> tileLog2
lastTileWidth = sbNum - ( tileNum - 1 ) * tileWidth
return tileWidth >= 1 && lastTileWidth >= 1
}

uniform_spacing ( tileLog2 , mis , sbSize ) {
/* AV2 specification v1.0.0, 5.18.7.5 Uniform spacing function */
sb4x4 = Num_4x4_Blocks_Wide [ sbSize ]
sbShift = Mi_Width_Log2 [ sbSize ]
sbs = ( mis + sb4x4 - 1 ) >> sbShift
fullSbs = mis >> sbShift
tileSb = fullSbs >> tileLog2
if ( tileSb == 0 ) {
extraSbs = sbs
} else {
extraSbs = fullSbs - ( tileSb << tileLog2 )
}
startSb = 0
for ( i = 0 ; i < ( 1 << tileLog2 ) && startSb < sbs ; i ++ ) {
sbStarts [ i ] = startSb
startSb += tileSb
if ( i < extraSbs ) {
startSb += 1
}
}
return ( sbStarts , i )
}

get_tx_row_col ( pos , txSz ) {
/* AV2 specification v1.0.0, 5.20.7.27 Coefficients syntax */
adjTxSz = Adjusted_Tx_Size [ txSz ]
bwl = Tx_Width_Log2 [ adjTxSz ]
row = pos >> bwl
col = pos - ( row << bwl )
return ( row , col )
}

get_scan ( txSz , txClass ) {
/* AV2 specification v1.0.0, 5.20.7.30 Get scan function */
w = Min ( Tx_Width [ txSz ], 32 )
h = Min ( Tx_Height [ txSz ], 32 )
if ( txClass == TX_CLASS_VERT ) {
c = 0
for ( y = 0 ; y < h ; y ++ ) {
for ( x = 0 ; x < w ; x ++ ) {
out [ c ] = y * w + x
c += 1
}
}
} else if ( txClass == TX_CLASS_HORIZ ) {
c = 0
for ( x = 0 ; x < w ; x ++ ) {
for ( y = 0 ; y < h ; y ++ ) {
out [ c ] = y * w + x
c += 1
}
}
} else {
x = 0
y = 0
for ( c = 0 ; c < w * h ; c ++ ) {
out [ c ] = y * w + x
x += 1
y -= 1
if ( y < 0 || x >= w ) {
x += 1
s = Min ( x , h - 1 - y )
x -= s
y += s
}
}
}
return out
}

get_filter_set_index ( base_qindex ) {
/* AV2 specification v1.0.0, 5.18.7.11 Loop restoration params syntax */
if ( base_qindex < 130 ) {
return 0
} else if ( base_qindex < 190 ) {
return 1
} else if ( base_qindex < 220 ) {
return 2
} else {
return 3
}
}

read_wienerns_filter( plane, unitRow, unitCol, readFrameFilters ) {
/* AV2 specification v1.0.0, 5.20.10.6 Read Wiener NS syntax */
numClasses = 1
if ( frame_filters_on[ plane ] ) {
if ( !readFrameFilters ) {
return
}
(numClasses, numRefFilters, _, _, _) = search_frame_filters( plane, -1 )
nopcw = lr_tools_disable[ 0 ][ RESTORE_PC_WIENER ]
groupCounts[ 0 ] = numClasses
groupCounts[ 1 ] = numRefFilters
groupCounts[ 2 ] = (plane > 0 || nopcw) ?
0 : 64 - numClasses - numRefFilters
for ( i = 0; i < 3; i++ ) {
groupHits[ i ] = 0
}
groupBase[ 0 ] = 0
for ( i = 1; i < 3; i++ ) {
groupBase[ i ] = groupBase[ i - 1 ] + groupCounts[ i - 1 ]
}
for ( c = 0 ; c < numClasses; c++ ) {
groupCounts[ 0 ] = c + 1
if ( c == 0 ) {
predGroup = (groupCounts[ 1 ] > 2) ?
1 : predict_group( groupCounts )
} else {
predGroup = predict_group( groupHits )
}
numZeros = 0
altGroup = 0
for ( i = 0; i < 3; i++ ) {
if ( i != predGroup ) {
if ( groupCounts[ i ] == 0 ) {
numZeros += 1
} else {
altGroup = i
}
}
}
if ( numZeros == 2 ) {
use_alt_group = 0
} else {
use_alt_group
f(1)
}
if ( use_alt_group ) {
if ( numZeros == 1 ) {
group = altGroup
} else {
group_bit
f(1)
group = predGroup <= group_bit ? group_bit + 1 : group_bit
}
} else {
group = predGroup
}
n = groupCounts[ group ]
ref = groupBase[ group ] + (n >> 1)
if ( n == 1 ) {
matchIndices[ c ] = groupBase[ group ]
} else {
matchIndices[ c ] = decode_signed_subexp_with_ref(
groupBase[ group ],
groupBase[ group ] + n, ref, 4)
}
groupHits[ group ]++
}
}
for ( c = 0 ; c < numClasses ; c++ ) {
if ( readFrameFilters ) {
merged_param
f(1)
} else {
merged_param
L(1)
}
merged[ c ] = merged_param
if ( readFrameFilters ) {
refBank[ c ] = 0
} else {
for ( k = 0; k < WienerNsBankSize[ plane ][ c ] - 1; k++ ) {
use_bank
L(1)
if (use_bank) {
break
}
}
refBank[ c ] = (WienerNsPtr[ plane ][ c ] - k + LR_BANK_SIZE) %
LR_BANK_SIZE
}
}
for ( c = 0 ; c < numClasses ; c++ ) {
if ( frame_filters_on[ plane ] ) {
fill_first_slot_of_bank_with_filter_match( c, plane,
matchIndices[ c ] )
}
nCoeffs = plane > 0 ? WIENER_NS_CHROMA_COEFFS :
WIENER_NS_LUMA_COEFFS
if ( merged[ c ] ) {
if ( WienerNsBankSize[ plane ][ c ] == 0 ) {
WienerNsBankSize[ plane ][ c ] = 1
}
} else {
if ( WienerNsBankSize[ plane ][ c ] < LR_BANK_SIZE ) {
WienerNsPtr[ plane ][ c ] = WienerNsBankSize[ plane ][ c ]
WienerNsBankSize[ plane ][ c ] += 1
} else {
WienerNsPtr[ plane ][ c ] = (WienerNsPtr[ plane ][ c ] + 1) %
LR_BANK_SIZE
}
numSubsets = plane == 0 ? 4 : 3
for ( subset = 0; subset < numSubsets - 1; subset++ ) {
if ( readFrameFilters ) {
wiener_ns_length
f(1)
} else {
wiener_ns_length
S()
}
if ( wiener_ns_length == 0 ) {
break
}
}
if ( plane > 0 && subset > 0 ) {
if ( readFrameFilters ) {
wiener_ns_uv_sym
f(1)
} else {
wiener_ns_uv_sym
S()
}
} else {
wiener_ns_uv_sym = 0
}
}
for ( j = 0; j < nCoeffs; j++ ) {
min = Wiener_Ns_Taps_Min[ plane!=0 ][ j ]
k = Wiener_Ns_Taps_K[ plane!=0 ][ j ]
v = RefLrWienerNs[ plane ][ c ][ refBank[ c ] ][ j ]
if ( !merged[ c ] ) {
if ( Wiener_Ns_Taps_Present[ plane!=0 ][ subset ][ j ] ) {
if ( readFrameFilters ) {
v = decode_signed_subexp_with_ref( min, min + (1 << k),
v, k - 3 )
} else {
v = decode_signed_4part( min, k, v )
}
} else {
v = 0
}
}
if ( readFrameFilters ) {
FrameLrWienerNs[ plane ][ c ][ j ] = v
if ( !merged[ c ] && plane > 0 &&
j >= WIENER_NS_SHORT_COEFFS && wiener_ns_uv_sym ) {
FrameLrWienerNs[ plane ][ c ][ j + 1 ] = v
j++
}
} else {
LrWienerNs[ plane ][ unitRow ][ unitCol ][ j ] = v
if ( !merged[ c ] ) {
RefLrWienerNs[ plane ][ c ]
[ WienerNsPtr[ plane ][ c ] ][ j ] = v
}
if ( !merged[ c ] && plane > 0 &&
j >= WIENER_NS_SHORT_COEFFS && wiener_ns_uv_sym ) {
LrWienerNs[ plane ][ unitRow ][ unitCol ][ j + 1 ] = v
RefLrWienerNs[ plane ][ c ]
[ WienerNsPtr[ plane ][ c ] ][ j + 1 ] = v
j++
}
}
}
}
}

reset_qm () {
/* AV2 specification v1.0.0, 5.18.2 Frame header info syntax */
for ( level = 0 ; level < 15 ; level ++ ) {
if ( obu_type == OBU_SWITCH || obu_type == OBU_RAS_FRAME ) {
needsReset = QmMLayerId [ level ] == -1 ||
MLayerPresenceMap [ QmMLayerId [ level ]][ obu_mlayer_id ]
} else {
needsReset = 1
}
if ( ! QmProtected [ level ] && needsReset ) {
QmDataPresent [ level ] = 0
QmNumPlanes [ level ] = NumPlanes
QmMLayerId [ level ] = -1
QmTLayerId [ level ] = -1
}
}
}

CeilLog2 ( x ) {
/* AV2 specification v1.0.0, 4.8 Mathematical functions */
if ( x < 2 )
return 0
i = 1
p = 2
while ( p < x ) {
i ++
p = p << 1
}
return i
}

choose_primary_secondary_ref_frame () {
/* AV2 specification v1.0.0, 5.18.2 Frame header info syntax */
if ( FrameIsIntra || FrameType == SWITCH_FRAME ) {
return ( PRIMARY_REF_NONE , PRIMARY_REF_NONE )
}
primary = PRIMARY_REF_NONE
primaryQpDiff = 512
secondary = PRIMARY_REF_NONE
secondaryQpDiff = 512
primaryD = 0
secondaryD = 0
primaryRatio = 0
secondaryRatio = 0
for ( i = 0 ; i < NumTotalRefs ; i ++ ) {
idx = ref_frame_idx [ i ]
if ( RefFrameType [ idx ] == INTER_FRAME && first_slot_with_ref ( idx ) &&
RefOrderHint [ idx ] != RESTRICTED_OH ) {
q = RefBaseQIdx [ idx ]
d = RefOrderHint [ idx ]
dRatio = FloorLog2 ( RefFrameWidth [ idx ] * RefFrameHeight [ idx ] )
qpDiff = Abs ( q - base_q_idx )
if ( ( qpDiff < primaryQpDiff ) ||
( qpDiff == primaryQpDiff &&
is_ref_better ( d , primaryD , dRatio , primaryRatio )) ) {
secondary = primary
secondaryQpDiff = primaryQpDiff
secondaryD = primaryD
secondaryRatio = primaryRatio
primary = i
primaryQpDiff = qpDiff
primaryD = d
primaryRatio = dRatio
} else if (( qpDiff < secondaryQpDiff ) ||
( qpDiff == secondaryQpDiff &&
is_ref_better ( d , secondaryD , dRatio , secondaryRatio ))) {
secondary = i
secondaryQpDiff = qpDiff
secondaryD = d
secondaryRatio = dRatio
}
}
}
if ( signal_primary_ref_frame ) {
if ( primary_ref_frame == PRIMARY_REF_NONE ) {
primary = PRIMARY_REF_NONE
secondary = PRIMARY_REF_NONE
} else if ( primary_ref_frame != primary ) {
if ( secondary == PRIMARY_REF_NONE ||
secondary == primary_ref_frame ) {
secondary = primary
}
primary = primary_ref_frame
}
}
return ( primary , secondary )
}

decode_signed_4part ( low , k , r ) {
/* AV2 specification v1.0.0, 5.20.10.6 Read Wiener NS syntax */
rOffset = r - low
xOffset = decode_unsigned_4part ( k , rOffset )
x = xOffset + low
return x
}

fill_first_slot_of_bank_with_filter_match ( c , plane , m ) {
/* AV2 specification v1.0.0, 5.20.10.6 Read Wiener NS syntax */
WienerNsPtr [ plane ][ c ] = 0
WienerNsBankSize [ plane ][ c ] = 1
( numClasses , numRefFilters , matchIdx , matchCls , matchPlane ) =
search_frame_filters ( plane , m )
for ( j = 0 ; j < ( ( plane > 0 ) ? WIENER_NS_CHROMA_COEFFS :
WIENER_NS_LUMA_COEFFS ); j ++ ) {
if ( m == 0 ) {
v = 0
} else if ( m < numClasses ) {
oldCls = m - 1
v = FrameLrWienerNs [ plane ][ oldCls ][ j ]
} else if ( m < numClasses + numRefFilters ) {
v = RefFrameLrWienerNs [ matchIdx ][ matchPlane ][ matchCls ][ j ]
} else {
v = get_translated_pc_wiener ( m - NumFilterClasses - numRefFilters , j )
}
RefLrWienerNs [ plane ][ c ][ 0 ][ j ] = v
}
}

get_max_disp_order_hint ( onlyShowable ) {
/* AV2 specification v1.0.0, 5.18.2 Frame header info syntax */
maxDisp = 0
for ( i = 0 ; i < NumRefFrames ; i ++ ) {
if ( RefValid [ i ] &&
TLayerDependencyMap [ obu_mlayer_id ][ obu_tlayer_id ][ RefTLayerId [ i ]] &&
MLayerDependencyMap [ obu_mlayer_id ][ RefMLayerId [ i ]] &&
( ! onlyShowable || RefImplicitOutputFrame [ i ] ||
RefImmediateOutputFrame [ i ] ) ) {
maxDisp = Max ( maxDisp , RefOrderHint [ i ])
}
}
return maxDisp
}

predict_group ( counts ) {
/* AV2 specification v1.0.0, 5.20.10.6 Read Wiener NS syntax */
pred = 0
for ( i = 1 ; i <= 2 ; i ++ ) {
if ( counts [ i ] > counts [ pred ] ) {
pred = i
}
}
return pred
}

Round2 ( x , n ) {
/* AV2 specification v1.0.0, 4.8 Mathematical functions */
if ( n == 0 )
return x
return ( x + ( 1 << ( n - 1 )) ) >> n
}

search_frame_filters ( plane , target ) {
/* AV2 specification v1.0.0, 5.20.10.6 Read Wiener NS syntax */
nopcw = lr_tools_disable [ 0 ][ RESTORE_PC_WIENER ]
minPcWiener = ( plane > 0 || nopcw ) ? 0 : 16
numClasses = ( plane == 0 ) ? NumFilterClasses : 1
maxRefFilters = ( nopcw ? 16 : 64 ) - numClasses - minPcWiener
numRefFilters = 0
numCheckPlanes = plane > 0 ? 2 : 1
matchIdx = 0
matchCls = 0
matchPlane = plane
for ( ref = 0 ; ref < NumTotalRefs ; ref ++ ) {
if ( FrameType != SWITCH_FRAME && OrderHints [ ref ] != RESTRICTED_OH ) {
idx = ref_frame_idx [ ref ]
for ( check = 0 ; check < numCheckPlanes ; check ++ ) {
if ( check == 0 ) {
checkPlane = plane
} else {
checkPlane = plane == 1 ? 2 : 1
}
if ( RefFrameFiltersOn [ idx ][ checkPlane ] ) {
numRefClasses = ( plane == 0 ) ?
RefNumFilterClasses [ idx ] : 1
for ( i = 0 ; i < numRefClasses ; i ++ ) {
if ( numRefFilters < maxRefFilters ) {
if ( numRefFilters + numClasses == target ) {
matchIdx = idx
matchCls = i
matchPlane = checkPlane
}
numRefFilters += 1
}
}
}
}
}
}
return ( numClasses , numRefFilters , matchIdx , matchCls , matchPlane )
}

decode_unsigned_4part ( k , r ) {
/* AV2 specification v1.0.0, 5.20.10.6 Read Wiener NS syntax */
mx = 1 << k
v = decode_4part ( 6 - k )
if (( r << 1 ) <= mx ) {
offset = inverse_recenter ( r , v )
} else {
offset = mx - 1 - inverse_recenter ( mx - 1 - r , v )
}
return offset
}

first_slot_with_ref ( i ) {
/* AV2 specification v1.0.0, 7.7 Get ref frames process */
if ( ! RefValid [ i ] ) {
return 0
}
for ( j = 0 ; j < i ; j ++ ) {
if ( RefValid [ j ] && RefCounter [ j ] == RefCounter [ i ] ) {
return 0
}
}
return 1
}

FloorLog2 ( x ) {
/* AV2 specification v1.0.0, 4.8 Mathematical functions */
s = 0
while ( x != 0 ) {
x = x >> 1
s ++
}
return s - 1
}

get_translated_pc_wiener ( m , j ) {
/* AV2 specification v1.0.0, 5.20.10.6 Read Wiener NS syntax */
if ( j >= 12 ) {
return 0
}
filt = Shuffled_Index [ m ]
coeff = Round2Signed ( Pc_Wiener_Filters [ 0 ][ filt ][ j ],
PC_WIENER_PREC_BITS - WIENER_NS_PREC_BITS )
min = Wiener_Ns_Taps_Min [ 0 ][ j ]
max = min + ( 1 << Wiener_Ns_Taps_K [ 0 ][ j ] ) - 1
return Clip3 ( min , max , coeff )
}

is_ref_better ( refDisp , bestDisp , refRatio , bestRatio ) {
/* AV2 specification v1.0.0, 5.18.2 Frame header info syntax */
d0 = Abs ( get_relative_dist ( OrderHint , refDisp )) - ( refRatio << 1 )
d1 = Abs ( get_relative_dist ( OrderHint , bestDisp )) - ( bestRatio << 1 )
if ( d0 < d1 ) {
return 1
}
if ( d0 == d1 && get_relative_dist ( refDisp , bestDisp ) > 0 ) {
return 1
}
return 0
}

bubble_sort_ref_scores ( ) {
/* AV2 specification v1.0.0, 7.7 Get ref frames process */
for ( i = NRanked - 1 ; i > 0 ; i -- ) {
for ( j = 0 ; j < i ; j ++ ) {
if ( ScoresScore [ j ] > ScoresScore [ j + 1 ]) {
index = ScoresIndex [ j ]
score = ScoresScore [ j ]
displayOrder = ScoresOrderHint [ j ]
distance = ScoresDistance [ j ]
baseQIdx = ScoresBaseQIdx [ j ]
ScoresIndex [ j ] = ScoresIndex [ j + 1 ]
ScoresScore [ j ] = ScoresScore [ j + 1 ]
ScoresOrderHint [ j ] = ScoresOrderHint [ j + 1 ]
ScoresDistance [ j ] = ScoresDistance [ j + 1 ]
ScoresBaseQIdx [ j ] = ScoresBaseQIdx [ j + 1 ]
ScoresIndex [ j + 1 ] = index
ScoresScore [ j + 1 ] = score
ScoresOrderHint [ j + 1 ] = displayOrder
ScoresDistance [ j + 1 ] = distance
ScoresBaseQIdx [ j + 1 ] = baseQIdx
}
}
}
}

get_unmapped_ref ( qThresh ) {
/* AV2 specification v1.0.0, 7.7 Get ref frames process */
nPast = 0
nFuture = 0
maxPastDistance = 0
maxFutureDistance = 0
pastIdx = 0
futureIdx = 0
for ( i = 0 ; i < NRanked ; i ++ ) {
if ( ScoresBaseQIdx [ i ] >= qThresh ) {
d = ScoresDistance [ i ]
if ( d > 0 ) {
if ( d > maxPastDistance ) {
maxPastDistance = d
pastIdx = i
}
nPast ++
} else if ( d < 0 ) {
if ( - d > maxFutureDistance ) {
maxFutureDistance = - d
futureIdx = i
}
nFuture ++
}
}
}
if ( nPast > nFuture ) {
return pastIdx
}
if ( nPast < nFuture ) {
return futureIdx
}
if ( nPast > 0 ) {
return maxPastDistance >= maxFutureDistance ? pastIdx : futureIdx
}
return -1
}

new_score_or_dist ( d , score , mLayer ) {
/* AV2 specification v1.0.0, 7.7 Get ref frames process */
for ( i = 0 ; i < NRanked ; i ++ ) {
if ( ScoresOrderHint [ i ] == d &&
ScoresScore [ i ] == score &&
mLayer == ScoresLayer [ i ] ) {
return 0
}
}
return 1
}

valid_ref_frame_size ( checkRes , slot ) {
/* AV2 specification v1.0.0, 7.7 Get ref frames process */
if ( ! checkRes )
return 1
return ( 2 * FrameWidth >= RefFrameWidth [ slot ] &&
2 * FrameHeight >= RefFrameHeight [ slot ] &&
FrameWidth <= 16 * RefFrameWidth [ slot ] &&
FrameHeight <= 16 * RefFrameHeight [ slot ] )
}

seg_feature_active_idx ( idx , feature ) {
/* AV2 specification v1.0.0, 5.20.5.12 Segmentation feature active function */
return segmentation_enabled && FeatureEnabled [ idx ][ feature ]
}
