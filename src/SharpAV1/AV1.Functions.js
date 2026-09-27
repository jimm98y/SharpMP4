set_frame_refs() {
/* AV1 Bitstream & Decoding Process Specification, 7.8 Set frame refs process: its code, in order */
for ( i = 0; i < REFS_PER_FRAME; i++ )
  ref_frame_idx[ i ] = -1
ref_frame_idx[ LAST_FRAME - LAST_FRAME ] = last_frame_idx
ref_frame_idx[ GOLDEN_FRAME - LAST_FRAME ] = gold_frame_idx
for ( i = 0; i < NUM_REF_FRAMES; i++ )
  usedFrame[ i ] = 0
usedFrame[ last_frame_idx ] = 1
usedFrame[ gold_frame_idx ] = 1
/* "A variable curFrameHint is set equal to 1 << (OrderHintBits - 1)." */
curFrameHint = 1 << (OrderHintBits - 1)
for ( i = 0; i < NUM_REF_FRAMES; i++ )
  shiftedOrderHints[ i ] = curFrameHint + get_relative_dist( RefOrderHint[ i ], OrderHint )
/* "The variable lastOrderHint ... is set equal to shiftedOrderHints[ last_frame_idx ]." */
lastOrderHint = shiftedOrderHints[ last_frame_idx ]
/* "The variable goldOrderHint ... is set equal to shiftedOrderHints[ gold_frame_idx ]." */
goldOrderHint = shiftedOrderHints[ gold_frame_idx ]
ref = find_latest_backward()
if ( ref >= 0 ) {
  ref_frame_idx[ ALTREF_FRAME - LAST_FRAME ] = ref
  usedFrame[ ref ] = 1
}
ref = find_earliest_backward()
if ( ref >= 0 ) {
  ref_frame_idx[ BWDREF_FRAME - LAST_FRAME ] = ref
  usedFrame[ ref ] = 1
}
ref = find_earliest_backward()
if ( ref >= 0 ) {
  ref_frame_idx[ ALTREF2_FRAME - LAST_FRAME ] = ref
  usedFrame[ ref ] = 1
}
for ( i = 0; i < REFS_PER_FRAME - 2; i++ ) {
  refFrame = Ref_Frame_List[ i ]
  if ( ref_frame_idx[ refFrame - LAST_FRAME ] < 0 ) {
    ref = find_latest_forward()
    if ( ref >= 0 ) {
      ref_frame_idx[ refFrame - LAST_FRAME ] = ref
      usedFrame[ ref ] = 1
    }
  }
}
ref = -1
for ( i = 0; i < NUM_REF_FRAMES; i++ ) {
  hint = shiftedOrderHints[ i ]
  if ( ref < 0 || hint < earliestOrderHint ) {
    ref = i
    earliestOrderHint = hint
  }
}
for ( i = 0; i < REFS_PER_FRAME; i++ ) {
  if ( ref_frame_idx[ i ] < 0 ) {
    ref_frame_idx[ i ] = ref
  }
}
}

find_latest_backward() {
/* AV1 Bitstream & Decoding Process Specification, 7.8 Set frame refs process */
  ref = -1
  for ( i = 0; i < NUM_REF_FRAMES; i++ ) {
    hint = shiftedOrderHints[ i ]
    if ( !usedFrame[ i ] &&
         hint >= curFrameHint &&
         ( ref < 0 || hint >= latestOrderHint ) ) {
      ref = i
      latestOrderHint = hint
    }
  }
  return ref
}

find_earliest_backward() {
/* AV1 Bitstream & Decoding Process Specification, 7.8 Set frame refs process */
  ref = -1
  for ( i = 0; i < NUM_REF_FRAMES; i++ ) {
    hint = shiftedOrderHints[ i ]
    if ( !usedFrame[ i ] &&
         hint >= curFrameHint &&
         ( ref < 0 || hint < earliestOrderHint ) ) {
      ref = i
      earliestOrderHint = hint
    }
  }
  return ref
}

find_latest_forward() {
/* AV1 Bitstream & Decoding Process Specification, 7.8 Set frame refs process */
  ref = -1
  for ( i = 0; i < NUM_REF_FRAMES; i++ ) {
    hint = shiftedOrderHints[ i ]
    if ( !usedFrame[ i ] &&
         hint < curFrameHint &&
         ( ref < 0 || hint >= latestOrderHint ) ) {
      ref = i
      latestOrderHint = hint
    }
  }
  return ref
}

mark_ref_frames( idLen ) {
/* AV1 Bitstream & Decoding Process Specification, 5.9.4 Reference frame marking function */
    diffLen = delta_frame_id_length_minus_2 + 2
    for ( i = 0; i < NUM_REF_FRAMES; i++ ) {
        if ( current_frame_id > ( 1 << diffLen ) ) {
            if ( RefFrameId[ i ] > current_frame_id ||
                 RefFrameId[ i ] < ( current_frame_id - ( 1 << diffLen ) ) )
                RefValid[ i ] = 0
        } else {
            if ( RefFrameId[ i ] > current_frame_id &&
                 RefFrameId[ i ] < ( ( 1 << idLen ) +
                                     current_frame_id -
                                     ( 1 << diffLen ) ) )
                RefValid[ i ] = 0
        }
    }
}
