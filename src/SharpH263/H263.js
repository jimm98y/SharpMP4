picture_layer() {
 /* Transcribed from ITU-T Rec. H.263 (01/2005), 5.1 and Figures 7 and 8, which give the picture layer as prose: not
    the Recommendation's text. Each element is named after its abbreviation, the bits of PTYPE and PLUSPTYPE after
    what they are. */
 psc u(22) /* 5.1.1 */
 tr u(8) /* 5.1.2 */
 ptype_start_code_emulation_bit u(1) /* 5.1.3: PTYPE bit 1, always 1 */
 ptype_h261_distinction_bit u(1) /* bit 2, always 0 */
 split_screen_indicator u(1) /* bit 3 */
 document_camera_indicator u(1) /* bit 4 */
 full_picture_freeze_release u(1) /* bit 5 */
 source_format u(3) /* bits 6-8 */
 if( source_format != 7 ) {
  picture_coding_type u(1) /* bit 9 */
  unrestricted_motion_vector_mode u(1) /* bit 10, Annex D */
  syntax_based_arithmetic_coding_mode u(1) /* bit 11, Annex E */
  advanced_prediction_mode u(1) /* bit 12, Annex F */
  pb_frames_mode u(1) /* bit 13, Annex G */
 } else {
  /* PLUSPTYPE, 5.1.4 */
  ufep u(3) /* 5.1.4.1 */
  if( ufep == 1 ) {
   /* OPPTYPE, 5.1.4.2 */
   opptype_source_format u(3) /* bits 1-3 */
   custom_pcf u(1) /* bit 4 */
   umv_mode u(1) /* bit 5, Annex D */
   sac_mode u(1) /* bit 6, Annex E */
   ap_mode u(1) /* bit 7, Annex F */
   aic_mode u(1) /* bit 8, Annex I */
   df_mode u(1) /* bit 9, Annex J */
   ss_mode u(1) /* bit 10, Annex K */
   rps_mode u(1) /* bit 11, Annex N */
   isd_mode u(1) /* bit 12, Annex R */
   aiv_mode u(1) /* bit 13, Annex S */
   mq_mode u(1) /* bit 14, Annex T */
   opptype_start_code_emulation_bit u(1) /* bit 15, always 1 */
   opptype_reserved_bits u(3) /* bits 16-18, 0 */
  }
  /* MPPTYPE, 5.1.4.3 */
  picture_type_code u(3) /* bits 1-3 */
  rpr_mode u(1) /* bit 4, Annex P */
  rru_mode u(1) /* bit 5, Annex Q */
  rounding_type u(1) /* bit 6 */
  mpptype_reserved_bits u(2) /* bits 7-8, 0 */
  mpptype_start_code_emulation_bit u(1) /* bit 9, always 1 */
  cpm u(1) /* 5.1.20: after PLUSPTYPE where it is present (5.1.4.7) */
  if( cpm == 1 )
   psbi u(2) /* 5.1.21 */
  if( ufep == 1 && opptype_source_format == 6 ) {
   /* CPFMT, 5.1.5 */
   pixel_aspect_ratio_code u(4) /* bits 1-4 */
   picture_width_indication u(9) /* bits 5-13 */
   cpfmt_start_code_emulation_bit u(1) /* bit 14, always 1 */
   picture_height_indication u(9) /* bits 15-23 */
   if( pixel_aspect_ratio_code == 15 ) {
    /* EPAR, 5.1.6 */
    par_width u(8)
    par_height u(8)
   }
  }
  if( ufep == 1 && custom_pcf == 1 ) {
   /* CPCFC, 5.1.7 */
   clock_conversion_code u(1)
   clock_divisor u(7)
  }
  if( custom_pcf_in_use )
   etr u(2) /* 5.1.8 */
  if( ufep == 1 && umv_mode == 1 ) {
   /* UUI, 5.1.9: "1", or "01" */
   uui u(1)
   if( uui == 0 )
    uui_unlimited u(1)
  }
  if( ufep == 1 && ss_mode == 1 ) {
   /* SSS, 5.1.10 */
   rectangular_slices u(1)
   arbitrary_slice_ordering u(1)
  }
  if( scalability_in_use ) {
   elnum u(4) /* 5.1.11 */
   if( ufep == 1 )
    rlnum u(4) /* 5.1.12 */
  }
  if( rps_in_use ) {
   if( ufep == 1 )
    rpsmf u(3) /* 5.1.13 */
   trpi u(1) /* 5.1.14 */
   if( trpi == 1 )
    trp u(10) /* 5.1.15 */
   /* BCI, 5.1.16: "1", a BCM following, or "01" */
   bci u(1)
   if( bci == 1 )
    bcm() /* 5.1.17, N.4.2 */
   else
    bci_end u(1)
  }
  if( rpr_mode == 1 )
   rprp() /* 5.1.18, P.2 */
 }
 pquant u(5) /* 5.1.19 */
 if( source_format != 7 ) {
  cpm u(1) /* 5.1.20: after PQUANT where PLUSPTYPE is not present */
  if( cpm == 1 )
   psbi u(2)
 }
 if( pb_frame ) {
  trb u(v) /* 5.1.22: 3 bits, or 5 with a custom picture clock frequency */
  dbquant u(2) /* 5.1.23 */
 }
 pei u(1) /* 5.1.24 */
 if( pei == 1 ) {
  do {
   psupp u(8) /* 5.1.25 */
   more_pei u(1)
  } while( more_pei_set )
 }
 picture_data() /* the GOBs or slices, ESTUF, EOS and PSTUF to the next picture start code */
}
