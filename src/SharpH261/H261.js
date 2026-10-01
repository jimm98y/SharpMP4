picture_layer() {
 /* Transcribed from ITU-T Rec. H.261 (03/93), 4.2.1 and Figure 5, which give the picture layer as prose: not the
    Recommendation's text. Each element is named after its abbreviation, the bits of PTYPE after what they are. */
 psc u(20) /* 4.2.1.1 */
 tr u(5) /* 4.2.1.2 */
 split_screen_indicator u(1) /* 4.2.1.3: PTYPE bit 1 */
 document_camera_indicator u(1) /* bit 2 */
 freeze_picture_release u(1) /* bit 3 */
 source_format u(1) /* bit 4: 0 QCIF, 1 CIF */
 hi_res u(1) /* bit 5: the still image mode of Annex D, on where 0 */
 ptype_spare_bit u(1) /* bit 6 */
 pei u(1) /* 4.2.1.4 */
 if( pei == 1 ) {
  do {
   pspare u(8) /* 4.2.1.5 */
   more_pei u(1)
  } while( more_pei_set )
 }
 picture_data() /* the GOBs (4.2.2), to the next picture start code */
}
