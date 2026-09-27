get_ref_frames( checkRes ) {
/* AV2 specification v1.0.0, 7.7 Get ref frames process: its code, in order */
maxDisp = 0
for ( i = 0 ; i < NumRefFrames ; i ++ ) {
mapOrderHint [ i ] = -1
if ( first_slot_with_ref ( i ) && RefOrderHint [ i ] != RESTRICTED_OH &&
( ! IsBridge || i == bridge_frame_ref_idx ) &&
( AllowedFrames & ( 1 << i )) &&
TLayerDependencyMap [ obu_mlayer_id ][ obu_tlayer_id ][ RefTLayerId [ i ]] &&
MLayerDependencyMap [ obu_mlayer_id ][ RefMLayerId [ i ]] ) {
if ( valid_ref_frame_size ( checkRes , i ) ) {
mapOrderHint [ i ] = RefOrderHint [ i ]
}
mapBaseQIdx [ i ] = RefBaseQIdx [ i ]
maxDisp = Max ( maxDisp , RefOrderHint [ i ])
}
}
NRanked = 0
maxQ = 0
minQ = 0
for ( i = 0 ; i < NumRefFrames ; i ++ ) {
d = mapOrderHint [ i ]
if ( d != -1 ) {
q = mapBaseQIdx [ i ]
dispDiff = get_relative_dist ( OrderHint , d )
tDist = Abs ( dispDiff ) + obu_mlayer_id - RefMLayerId [ i ]
if ( maxDisp > OrderHint ) {
score = ( tDist << DIST_WEIGHT_BITS ) + q
} else {
score = Dist_Score_Lookup [ Min ( tDist , DECAY_DIST_CAP )] +
Max ( tDist - DECAY_DIST_CAP , 0 ) + q
}
refRatio = FloorLog2 ( RefFrameWidth [ i ] * RefFrameHeight [ i ] )
score -= refRatio << 5
if ( new_score_or_dist ( d , score , RefMLayerId [ i ])) {
ScoresIndex [ NRanked ] = i
ScoresScore [ NRanked ] = score
ScoresOrderHint [ NRanked ] = d
ScoresDistance [ NRanked ] = dispDiff
ScoresBaseQIdx [ NRanked ] = q
ScoresLayer [ NRanked ] = RefMLayerId [ i ]
if ( NRanked == 0 ) {
minQ = q
maxQ = q
} else {
minQ = Min ( q , minQ )
maxQ = Max ( q , maxQ )
}
NRanked += 1
}
}
}
if ( NRanked > REFS_PER_FRAME ) {
qThresh = ( maxQ + minQ + 1 ) / 2
unmappedIdx = get_unmapped_ref ( qThresh )
if ( unmappedIdx >= 0 ) {
ScoresScore [ unmappedIdx ] = 0x7fffffff
}
}
bubble_sort_ref_scores ()
NumTotalRefs = Min ( NRanked , ActiveNumRefFrames )
for ( i = 0 ; i < NumTotalRefs ; i ++ ) {
ref_frame_idx [ i ] = ScoresIndex [ i ]
}
if ( checkRes && ! IsBridge ) {
for ( i = 0 ; i < NumRefFrames ; i ++ ) {
if ( RefValid [ i ] && RefOrderHint [ i ] == RESTRICTED_OH &&
TLayerDependencyMap [ obu_mlayer_id ][ obu_tlayer_id ][ RefTLayerId [ i ]] &&
MLayerDependencyMap [ obu_mlayer_id ][ RefMLayerId [ i ]] &&
( AllowedFrames & ( 1 << i )) &&
NumTotalRefs < ActiveNumRefFrames ) {
ref_frame_idx [ NumTotalRefs ] = i
NumTotalRefs ++
}
}
}
}

get_past_future_cur_ref_lists( ) {
/* AV2 specification v1.0.0, 7.8 Get past future cur ref lists process: its code */
NumPastRefs = 0
NumFutureRefs = 0
numCurRefs = 0
FurthestFuture = NONE
ClosestPast = NONE
ClosestFuture = NONE
for ( i = 0 ; i < NumTotalRefs ; i ++ ) {
if ( RefOrderHint [ ref_frame_idx [ i ]] != RESTRICTED_OH ) {
if ( ScoresDistance [ i ] > 0 ) {
NumPastRefs ++
if ( ClosestPast == NONE ||
ScoresDistance [ i ] < ScoresDistance [ ClosestPast ] ) {
ClosestPast = i
}
} else if ( ScoresDistance [ i ] < 0 ) {
NumFutureRefs ++
if ( FurthestFuture == NONE ||
RefOrderHint [ ref_frame_idx [ FurthestFuture ]] <
RefOrderHint [ ref_frame_idx [ i ]] ) {
FurthestFuture = i
}
if ( ClosestFuture == NONE ||
RefOrderHint [ ref_frame_idx [ i ]] <
RefOrderHint [ ref_frame_idx [ ClosestFuture ]] ) {
ClosestFuture = i
}
} else {
curRefs [ numCurRefs ] = i
numCurRefs ++
}
}
}
SkipSegFrame = numCurRefs > 0 ? curRefs [ 0 ] : ClosestPast
if ( SkipSegFrame == NONE ) {
SkipSegFrame = 0
}
OrigClosestFuture = ClosestFuture
OrigClosestPast = ClosestPast
}

motion_field_estimation( ) {
/* AV2 specification v1.0.0, 7.9.1 Motion field estimation process: the code that chooses the references for TIP. */
/* The motion field it projects is the tile data's, and is left out. */
for ( i = 0 ; i < NumTotalRefs ; i ++ ) {
sortRef [ i ] = i
}
for ( i = 0 ; i < NumTotalRefs ; i ++ ) {
for ( j = i + 1 ; j < NumTotalRefs ; j ++ ) {
if ( get_relative_dist ( OrderHints [ sortRef [ j ] ],
OrderHints [ sortRef [ i ] ] ) < 0 ) {
tmp = sortRef [ i ]
sortRef [ i ] = sortRef [ j ]
sortRef [ j ] = tmp
}
}
}
curIdx = -1
for ( i = 0 ; i < NumTotalRefs ; i ++ ) {
if ( get_relative_dist ( OrderHints [ sortRef [ i ] ], OrderHint ) < 0 ) {
curIdx = i
} else {
break
}
}
for ( rf = 0 ; rf < NumTotalRefs ; rf ++ ) {
MotionFieldVisited [ rf ] = 0
MotionFieldDepth [ rf ] = -1
MotionFieldChecked [ rf ][ 0 ] = 0
MotionFieldChecked [ rf ][ 1 ] = 0
}
MotionFieldStackCount = 0
for ( rf = 0 ; rf < NumTotalRefs ; rf ++ ) {
if ( OrderHints [ rf ] != RESTRICTED_OH ) {
topo_sort_refs ( rf )
}
}
/* "If MotionFieldStackCount is less than 2, the process immediately terminates." */
if ( MotionFieldStackCount < 2 ) {
return
}
processCount = 0
if ( enable_tip &&
( ( NumFutureRefs > 0 && NumPastRefs > 0 ) || NumPastRefs >= 2 ) ) {
past = sortRef [ curIdx ]
if ( NumFutureRefs > 0 && NumPastRefs > 0 ) {
future = sortRef [ curIdx + 1 ]
} else {
future = sortRef [ curIdx - 1 ]
}
if ( MotionFieldDepth [ past ] > MotionFieldDepth [ future ] ) {
processCount = record_tip_projection ( past , 1 , future , processCount )
} else {
processCount = record_tip_projection ( future , 0 , past , processCount )
}
ClosestPast = past
ClosestFuture = future
} else {
ClosestPast = NONE
ClosestFuture = NONE
}
}

topo_sort_refs ( rf ) {
/* AV2 specification v1.0.0, 7.9.1 */
if ( MotionFieldVisited [ rf ] ) {
return
}
MotionFieldVisited [ rf ] = 1
refIdx = ref_frame_idx [ rf ]
if ( RefFrameType [ refIdx ] == INTER_FRAME ) {
for ( i = 0 ; i < RefNumTotalRefs [ refIdx ]; i ++ ) {
if ( SavedOrderHints [ refIdx ][ i ] != RESTRICTED_OH ) {
for ( j = 0 ; j < NumTotalRefs ; j ++ ) {
if ( OrderHints [ j ] == SavedOrderHints [ refIdx ][ i ] &&
! is_ref_overlay ( j ) ) {
topo_sort_refs ( j )
break
}
}
}
}
}
MotionFieldDepth [ rf ] = MotionFieldStackCount
MotionFieldStack [ MotionFieldStackCount ] = rf
MotionFieldStackCount ++
}

is_ref_overlay ( ref ) {
/* AV2 specification v1.0.0, 7.9.1 */
refIdx = ref_frame_idx [ ref ]
for ( i = 0 ; i < RefNumTotalRefs [ refIdx ]; i ++ ) {
if ( SavedOrderHints [ refIdx ][ i ] == RefOrderHint [ refIdx ]) {
return 1
}
}
return 0
}
