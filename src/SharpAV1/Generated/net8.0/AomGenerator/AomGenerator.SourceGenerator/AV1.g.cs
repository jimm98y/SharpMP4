using System;
using System.Collections.Generic;
using System.Numerics;

namespace SharpAV1
{

    public partial class AV1Context : IAomContext
    {
        AomStream stream = null;
        private void ReadDropObu() { };
        private void WriteDropObu() { };

    /*
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
 if ( obu_size > 0 && obu_type != OBU_TILE_GROUP &&
    obu_type != OBU_TILE_LIST &&
    obu_type != OBU_FRAME ) {
    trailing_bits( obu_size * 8 - payloadBits )
 }
}
    */
		private uint sz;
		public uint Sz { get { return sz; } set { sz = value; } }
		private uint obu_size;
		public uint ObuSize { get { return obu_size; } set { obu_size = value; } }

         public void ReadOpenBitstreamUnit(uint sz)
         {

			ReadObuHeader(); 

			if ( obu_has_size_field != 0 )
			{
				stream.ReadLeb128( out this.obu_size, "obu_size"); 
			}
			else 
			{
				obu_size= sz - 1 - obu_extension_flag;
			}
			uint startPosition= stream.GetPosition();

			if ( obu_type != AV1ObuTypes.OBU_SEQUENCE_HEADER && obu_type != AV1ObuTypes.OBU_TEMPORAL_DELIMITER && OperatingPointIdc != 0 && obu_extension_flag == 1 )
			{
				inTemporalLayer= (OperatingPointIdc >> temporal_id ) & 1;
				inSpatialLayer= (OperatingPointIdc >> ( spatial_id + 8 ) ) & 1;

				if ( inTemporalLayer== 0 ||  inSpatialLayer== 0 )
				{
					ReadDropObu(); 
return;
				}
			}

			if ( obu_type == AV1ObuTypes.OBU_SEQUENCE_HEADER )
			{
				ReadSequenceHeaderObu(); 
			}
			else if ( obu_type == AV1ObuTypes.OBU_TEMPORAL_DELIMITER )
			{
				ReadTemporalDelimiterObu(); 
			}
			else if ( obu_type == AV1ObuTypes.OBU_FRAME_HEADER )
			{
				ReadFrameHeaderObu(); 
			}
			else if ( obu_type == AV1ObuTypes.OBU_REDUNDANT_FRAME_HEADER )
			{
				ReadFrameHeaderObu(); 
			}
			else if ( obu_type == AV1ObuTypes.OBU_TILE_GROUP )
			{
				ReadTileGroupObu( obu_size ); 
			}
			else if ( obu_type == AV1ObuTypes.OBU_METADATA )
			{
				ReadMetadataObu(); 
			}
			else if ( obu_type == AV1ObuTypes.OBU_FRAME )
			{
				ReadFrameObu( obu_size ); 
			}
			else if ( obu_type == AV1ObuTypes.OBU_TILE_LIST )
			{
				ReadTileListObu(); 
			}
			else if ( obu_type == AV1ObuTypes.OBU_PADDING )
			{
				ReadPaddingObu(); 
			}
			else 
			{
				ReadReservedObu(); 
			}
			uint currentPosition= stream.GetPosition();
			uint payloadBits= currentPosition - startPosition;

			if ( obu_size > 0 && obu_type != AV1ObuTypes.OBU_TILE_GROUP &&
    obu_type != AV1ObuTypes.OBU_TILE_LIST &&
    obu_type != AV1ObuTypes.OBU_FRAME )
			{
				ReadTrailingBits( obu_size * 8 - payloadBits ); 
			}
         }

         public void WriteOpenBitstreamUnit(uint sz)
         {

			WriteObuHeader(); 

			if ( obu_has_size_field != 0 )
			{
				stream.WriteLeb128( this.obu_size, "obu_size"); 
			}
			else 
			{
				obu_size= sz - 1 - obu_extension_flag;
			}
			uint startPosition= stream.GetPosition();

			if ( obu_type != AV1ObuTypes.OBU_SEQUENCE_HEADER && obu_type != AV1ObuTypes.OBU_TEMPORAL_DELIMITER && OperatingPointIdc != 0 && obu_extension_flag == 1 )
			{
				inTemporalLayer= (OperatingPointIdc >> temporal_id ) & 1;
				inSpatialLayer= (OperatingPointIdc >> ( spatial_id + 8 ) ) & 1;

				if ( inTemporalLayer== 0 ||  inSpatialLayer== 0 )
				{
					WriteDropObu(); 
return;
				}
			}

			if ( obu_type == AV1ObuTypes.OBU_SEQUENCE_HEADER )
			{
				WriteSequenceHeaderObu(); 
			}
			else if ( obu_type == AV1ObuTypes.OBU_TEMPORAL_DELIMITER )
			{
				WriteTemporalDelimiterObu(); 
			}
			else if ( obu_type == AV1ObuTypes.OBU_FRAME_HEADER )
			{
				WriteFrameHeaderObu(); 
			}
			else if ( obu_type == AV1ObuTypes.OBU_REDUNDANT_FRAME_HEADER )
			{
				WriteFrameHeaderObu(); 
			}
			else if ( obu_type == AV1ObuTypes.OBU_TILE_GROUP )
			{
				WriteTileGroupObu( obu_size ); 
			}
			else if ( obu_type == AV1ObuTypes.OBU_METADATA )
			{
				WriteMetadataObu(); 
			}
			else if ( obu_type == AV1ObuTypes.OBU_FRAME )
			{
				WriteFrameObu( obu_size ); 
			}
			else if ( obu_type == AV1ObuTypes.OBU_TILE_LIST )
			{
				WriteTileListObu(); 
			}
			else if ( obu_type == AV1ObuTypes.OBU_PADDING )
			{
				WritePaddingObu(); 
			}
			else 
			{
				WriteReservedObu(); 
			}
			uint currentPosition= stream.GetPosition();
			uint payloadBits= currentPosition - startPosition;

			if ( obu_size > 0 && obu_type != AV1ObuTypes.OBU_TILE_GROUP &&
    obu_type != AV1ObuTypes.OBU_TILE_LIST &&
    obu_type != AV1ObuTypes.OBU_FRAME )
			{
				WriteTrailingBits( obu_size * 8 - payloadBits ); 
			}
         }

    /*


obu_header() { 
 obu_forbidden_bit f(1)
 obu_type f(4)
 obu_extension_flag f(1)
 obu_has_size_field f(1)
 obu_reserved_1bit f(1)
 if ( obu_extension_flag == 1 )
  obu_extension_header()
}
    */
		private uint obu_forbidden_bit;
		public uint ObuForbiddenBit { get { return obu_forbidden_bit; } set { obu_forbidden_bit = value; } }
		private uint obu_type;
		public uint ObuType { get { return obu_type; } set { obu_type = value; } }
		private uint obu_extension_flag;
		public uint ObuExtensionFlag { get { return obu_extension_flag; } set { obu_extension_flag = value; } }
		private uint obu_has_size_field;
		public uint ObuHasSizeField { get { return obu_has_size_field; } set { obu_has_size_field = value; } }
		private uint obu_reserved_1bit;
		public uint ObuReserved1bit { get { return obu_reserved_1bit; } set { obu_reserved_1bit = value; } }

         public void ReadObuHeader()
         {

			stream.ReadFixed(1, out this.obu_forbidden_bit, "obu_forbidden_bit"); 
			stream.ReadFixed(4, out this.obu_type, "obu_type"); 
			stream.ReadFixed(1, out this.obu_extension_flag, "obu_extension_flag"); 
			stream.ReadFixed(1, out this.obu_has_size_field, "obu_has_size_field"); 
			stream.ReadFixed(1, out this.obu_reserved_1bit, "obu_reserved_1bit"); 

			if ( obu_extension_flag == 1 )
			{
				ReadObuExtensionHeader(); 
			}
         }

         public void WriteObuHeader()
         {

			stream.WriteFixed(1, this.obu_forbidden_bit, "obu_forbidden_bit"); 
			stream.WriteFixed(4, this.obu_type, "obu_type"); 
			stream.WriteFixed(1, this.obu_extension_flag, "obu_extension_flag"); 
			stream.WriteFixed(1, this.obu_has_size_field, "obu_has_size_field"); 
			stream.WriteFixed(1, this.obu_reserved_1bit, "obu_reserved_1bit"); 

			if ( obu_extension_flag == 1 )
			{
				WriteObuExtensionHeader(); 
			}
         }

    /*



 obu_extension_header() { 
 temporal_id f(3)
 spatial_id f(2)
 extension_header_reserved_3bits f(3)
 }
    */
		private uint temporal_id;
		public uint TemporalId { get { return temporal_id; } set { temporal_id = value; } }
		private uint spatial_id;
		public uint SpatialId { get { return spatial_id; } set { spatial_id = value; } }
		private uint extension_header_reserved_3bits;
		public uint ExtensionHeaderReserved3bits { get { return extension_header_reserved_3bits; } set { extension_header_reserved_3bits = value; } }

         public void ReadObuExtensionHeader()
         {

			stream.ReadFixed(3, out this.temporal_id, "temporal_id"); 
			stream.ReadFixed(2, out this.spatial_id, "spatial_id"); 
			stream.ReadFixed(3, out this.extension_header_reserved_3bits, "extension_header_reserved_3bits"); 
         }

         public void WriteObuExtensionHeader()
         {

			stream.WriteFixed(3, this.temporal_id, "temporal_id"); 
			stream.WriteFixed(2, this.spatial_id, "spatial_id"); 
			stream.WriteFixed(3, this.extension_header_reserved_3bits, "extension_header_reserved_3bits"); 
         }

    /*



trailing_bits( nbBits ) { 
 trailing_one_bit f(1)
 nbBits--
while ( nbBits > 0 ) {
 trailing_zero_bit f(1)
 nbBits--
}
}
    */
		private uint nbBits;
		public uint NbBits { get { return nbBits; } set { nbBits = value; } }
		private uint trailing_one_bit;
		public uint TrailingOneBit { get { return trailing_one_bit; } set { trailing_one_bit = value; } }
		private Dictionary<int, uint> trailing_zero_bit = new Dictionary<int, uint>();
		public Dictionary<int, uint> TrailingZeroBit { get { return trailing_zero_bit; } set { trailing_zero_bit = value; } }

			int whileIndex = -1;
         public void ReadTrailingBits(uint nbBits)
         {

			stream.ReadFixed(1, out this.trailing_one_bit, "trailing_one_bit"); 
			nbBits--;

			while ( nbBits > 0 )
			{
				whileIndex++;

				stream.ReadFixed(1, out this.trailing_zero_bit, "trailing_zero_bit"); 
				nbBits--;
			}
         }

         public void WriteTrailingBits(uint nbBits)
         {

			stream.WriteFixed(1, this.trailing_one_bit, "trailing_one_bit"); 
			nbBits--;

			while ( nbBits > 0 )
			{
				whileIndex++;

				stream.WriteFixed(1, this.trailing_zero_bit, "trailing_zero_bit"); 
				nbBits--;
			}
         }

    /*



byte_alignment() { 
 while ( get_position() & 7 )
 zero_bit f(1)
}
    */
		private Dictionary<int, uint> zero_bit = new Dictionary<int, uint>();
		public Dictionary<int, uint> ZeroBit { get { return zero_bit; } set { zero_bit = value; } }

			int whileIndex = -1;
         public void ReadByteAlignment()
         {


			while ( get_position() & 7 )
			{
				whileIndex++;

				stream.ReadFixed(1, out this.zero_bit, "zero_bit"); 
			}
         }

         public void WriteByteAlignment()
         {


			while ( get_position() & 7 )
			{
				whileIndex++;

				stream.WriteFixed(1, this.zero_bit, "zero_bit"); 
			}
         }

    /*



reserved_obu() { 
}
    */

         public void ReadReservedObu()
         {

         }

         public void WriteReservedObu()
         {

         }

    /*



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
 operating_point_idc[ i ] operating_point_idc[ i ] f(12)
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
 initial_display_delay_minus_1[ i ] initial_display_delay_minus_1[ i ] f(4)
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
    */
		private uint seq_profile;
		public uint SeqProfile { get { return seq_profile; } set { seq_profile = value; } }
		private uint still_picture;
		public uint StillPicture { get { return still_picture; } set { still_picture = value; } }
		private uint reduced_still_picture_header;
		public uint ReducedStillPictureHeader { get { return reduced_still_picture_header; } set { reduced_still_picture_header = value; } }
		private uint[] seq_level_idx;
		public uint[] SeqLevelIdx { get { return seq_level_idx; } set { seq_level_idx = value; } }
		private uint timing_info_present_flag;
		public uint TimingInfoPresentFlag { get { return timing_info_present_flag; } set { timing_info_present_flag = value; } }
		private uint decoder_model_info_present_flag;
		public uint DecoderModelInfoPresentFlag { get { return decoder_model_info_present_flag; } set { decoder_model_info_present_flag = value; } }
		private uint initial_display_delay_present_flag;
		public uint InitialDisplayDelayPresentFlag { get { return initial_display_delay_present_flag; } set { initial_display_delay_present_flag = value; } }
		private uint operating_points_cnt_minus_1;
		public uint OperatingPointsCntMinus1 { get { return operating_points_cnt_minus_1; } set { operating_points_cnt_minus_1 = value; } }
		private uint[] operating_point_idc;
		public uint[] OperatingPointIdc { get { return operating_point_idc; } set { operating_point_idc = value; } }
		private uint[] seq_tier;
		public uint[] SeqTier { get { return seq_tier; } set { seq_tier = value; } }
		private uint[] decoder_model_present_for_this_op;
		public uint[] DecoderModelPresentForThisOp { get { return decoder_model_present_for_this_op; } set { decoder_model_present_for_this_op = value; } }
		private uint[] initial_display_delay_present_for_this_op;
		public uint[] InitialDisplayDelayPresentForThisOp { get { return initial_display_delay_present_for_this_op; } set { initial_display_delay_present_for_this_op = value; } }
		private uint[] initial_display_delay_minus_1;
		public uint[] InitialDisplayDelayMinus1 { get { return initial_display_delay_minus_1; } set { initial_display_delay_minus_1 = value; } }
		private uint frame_width_bits_minus_1;
		public uint FrameWidthBitsMinus1 { get { return frame_width_bits_minus_1; } set { frame_width_bits_minus_1 = value; } }
		private uint frame_height_bits_minus_1;
		public uint FrameHeightBitsMinus1 { get { return frame_height_bits_minus_1; } set { frame_height_bits_minus_1 = value; } }
		private uint max_frame_width_minus_1;
		public uint MaxFrameWidthMinus1 { get { return max_frame_width_minus_1; } set { max_frame_width_minus_1 = value; } }
		private uint max_frame_height_minus_1;
		public uint MaxFrameHeightMinus1 { get { return max_frame_height_minus_1; } set { max_frame_height_minus_1 = value; } }
		private uint frame_id_numbers_present_flag;
		public uint FrameIdNumbersPresentFlag { get { return frame_id_numbers_present_flag; } set { frame_id_numbers_present_flag = value; } }
		private uint delta_frame_id_length_minus_2;
		public uint DeltaFrameIdLengthMinus2 { get { return delta_frame_id_length_minus_2; } set { delta_frame_id_length_minus_2 = value; } }
		private uint additional_frame_id_length_minus_1;
		public uint AdditionalFrameIdLengthMinus1 { get { return additional_frame_id_length_minus_1; } set { additional_frame_id_length_minus_1 = value; } }
		private uint use_128x128_superblock;
		public uint Use128x128Superblock { get { return use_128x128_superblock; } set { use_128x128_superblock = value; } }
		private uint enable_filter_intra;
		public uint EnableFilterIntra { get { return enable_filter_intra; } set { enable_filter_intra = value; } }
		private uint enable_intra_edge_filter;
		public uint EnableIntraEdgeFilter { get { return enable_intra_edge_filter; } set { enable_intra_edge_filter = value; } }
		private uint enable_interintra_compound;
		public uint EnableInterintraCompound { get { return enable_interintra_compound; } set { enable_interintra_compound = value; } }
		private uint enable_masked_compound;
		public uint EnableMaskedCompound { get { return enable_masked_compound; } set { enable_masked_compound = value; } }
		private uint enable_warped_motion;
		public uint EnableWarpedMotion { get { return enable_warped_motion; } set { enable_warped_motion = value; } }
		private uint enable_dual_filter;
		public uint EnableDualFilter { get { return enable_dual_filter; } set { enable_dual_filter = value; } }
		private uint enable_order_hint;
		public uint EnableOrderHint { get { return enable_order_hint; } set { enable_order_hint = value; } }
		private uint enable_jnt_comp;
		public uint EnableJntComp { get { return enable_jnt_comp; } set { enable_jnt_comp = value; } }
		private uint enable_ref_frame_mvs;
		public uint EnableRefFrameMvs { get { return enable_ref_frame_mvs; } set { enable_ref_frame_mvs = value; } }
		private uint seq_choose_screen_content_tools;
		public uint SeqChooseScreenContentTools { get { return seq_choose_screen_content_tools; } set { seq_choose_screen_content_tools = value; } }
		private uint seq_force_screen_content_tools;
		public uint SeqForceScreenContentTools { get { return seq_force_screen_content_tools; } set { seq_force_screen_content_tools = value; } }
		private uint seq_choose_integer_mv;
		public uint SeqChooseIntegerMv { get { return seq_choose_integer_mv; } set { seq_choose_integer_mv = value; } }
		private uint seq_force_integer_mv;
		public uint SeqForceIntegerMv { get { return seq_force_integer_mv; } set { seq_force_integer_mv = value; } }
		private uint order_hint_bits_minus_1;
		public uint OrderHintBitsMinus1 { get { return order_hint_bits_minus_1; } set { order_hint_bits_minus_1 = value; } }
		private uint enable_superres;
		public uint EnableSuperres { get { return enable_superres; } set { enable_superres = value; } }
		private uint enable_cdef;
		public uint EnableCdef { get { return enable_cdef; } set { enable_cdef = value; } }
		private uint enable_restoration;
		public uint EnableRestoration { get { return enable_restoration; } set { enable_restoration = value; } }
		private uint film_grain_params_present;
		public uint FilmGrainParamsPresent { get { return film_grain_params_present; } set { film_grain_params_present = value; } }

			uint i = 0;
         public void ReadSequenceHeaderObu()
         {

			stream.ReadFixed(3, out this.seq_profile, "seq_profile"); 
			stream.ReadFixed(1, out this.still_picture, "still_picture"); 
			stream.ReadFixed(1, out this.reduced_still_picture_header, "reduced_still_picture_header"); 

			if ( reduced_still_picture_header != 0 )
			{
				timing_info_present_flag= 0;
				decoder_model_info_present_flag= 0;
				initial_display_delay_present_flag= 0;
				operating_points_cnt_minus_1= 0;
				operating_point_idc[ 0 ]= 0;
				stream.ReadFixed(5, out this.seq_level_idx[ 0 ], "seq_level_idx"); 
				seq_tier[ 0 ]= 0;
				decoder_model_present_for_this_op[ 0 ]= 0;
				initial_display_delay_present_for_this_op[ 0 ]= 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.timing_info_present_flag, "timing_info_present_flag"); 

				if ( timing_info_present_flag != 0 )
				{
					ReadTimingInfo(); 
					stream.ReadFixed(1, out this.decoder_model_info_present_flag, "decoder_model_info_present_flag"); 

					if ( decoder_model_info_present_flag != 0 )
					{
						ReadDecoderModelInfo(); 
					}
				}
				else 
				{
					decoder_model_info_present_flag= 0;
				}
				stream.ReadFixed(1, out this.initial_display_delay_present_flag, "initial_display_delay_present_flag"); 
				stream.ReadFixed(5, out this.operating_points_cnt_minus_1, "operating_points_cnt_minus_1"); 

				this.operating_point_idc = new uint[ operating_points_cnt_minus_1];
				this.seq_tier = new uint[ operating_points_cnt_minus_1];
				this.decoder_model_present_for_this_op = new uint[ operating_points_cnt_minus_1];
				this.initial_display_delay_present_for_this_op = new uint[ operating_points_cnt_minus_1];
				this.initial_display_delay_minus_1 = new uint[ operating_points_cnt_minus_1];
				for ( i = 0; i <= operating_points_cnt_minus_1; i++ )
				{
					ReadOperatingPointIdc; 
					stream.ReadFixed(12, out this.operating_point_idc[ i ], "operating_point_idc"); 
					stream.ReadFixed(5, out this.seq_level_idx[ i ], "seq_level_idx"); 

					if ( seq_level_idx[ i ] > 7 )
					{
						stream.ReadFixed(1, out this.seq_tier[ i ], "seq_tier"); 
					}
					else 
					{
						seq_tier[ i ]= 0;
					}

					if ( decoder_model_info_present_flag != 0 )
					{
						stream.ReadFixed(1, out this.decoder_model_present_for_this_op[ i ], "decoder_model_present_for_this_op"); 

						if ( decoder_model_present_for_this_op[ i ] != 0 )
						{
							ReadOperatingParametersInfo( i ); 
						}
					}
					else 
					{
						decoder_model_present_for_this_op[ i ]= 0;
					}

					if ( initial_display_delay_present_flag != 0 )
					{
						stream.ReadFixed(1, out this.initial_display_delay_present_for_this_op[ i ], "initial_display_delay_present_for_this_op"); 

						if ( initial_display_delay_present_for_this_op[ i ] != 0 )
						{
							ReadInitialDisplayDelayMinus1; 
							stream.ReadFixed(4, out this.initial_display_delay_minus_1[ i ], "initial_display_delay_minus_1"); 
						}
					}
				}
			}
			uint operatingPoint= choose_operating_point();
			uint OperatingPointIdc= operating_point_idc[ operatingPoint ];
			stream.ReadFixed(4, out this.frame_width_bits_minus_1, "frame_width_bits_minus_1"); 
			stream.ReadFixed(4, out this.frame_height_bits_minus_1, "frame_height_bits_minus_1"); 
			uint n= frame_width_bits_minus_1 + 1;
			stream.ReadVariable(n, out this.max_frame_width_minus_1, "max_frame_width_minus_1"); 
			n= frame_height_bits_minus_1 + 1;
			stream.ReadVariable(n, out this.max_frame_height_minus_1, "max_frame_height_minus_1"); 

			if ( reduced_still_picture_header != 0 )
			{
				frame_id_numbers_present_flag= 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.frame_id_numbers_present_flag, "frame_id_numbers_present_flag"); 
			}

			if ( frame_id_numbers_present_flag != 0 )
			{
				stream.ReadFixed(4, out this.delta_frame_id_length_minus_2, "delta_frame_id_length_minus_2"); 
				stream.ReadFixed(3, out this.additional_frame_id_length_minus_1, "additional_frame_id_length_minus_1"); 
			}
			stream.ReadFixed(1, out this.use_128x128_superblock, "use_128x128_superblock"); 
			stream.ReadFixed(1, out this.enable_filter_intra, "enable_filter_intra"); 
			stream.ReadFixed(1, out this.enable_intra_edge_filter, "enable_intra_edge_filter"); 

			if ( reduced_still_picture_header != 0 )
			{
				enable_interintra_compound= 0;
				enable_masked_compound= 0;
				enable_warped_motion= 0;
				enable_dual_filter= 0;
				enable_order_hint= 0;
				enable_jnt_comp= 0;
				enable_ref_frame_mvs= 0;
				seq_force_screen_content_tools= SELECT_SCREEN_CONTENT_TOOLS;
				seq_force_integer_mv= SELECT_INTEGER_MV;
				OrderHintBits= 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.enable_interintra_compound, "enable_interintra_compound"); 
				stream.ReadFixed(1, out this.enable_masked_compound, "enable_masked_compound"); 
				stream.ReadFixed(1, out this.enable_warped_motion, "enable_warped_motion"); 
				stream.ReadFixed(1, out this.enable_dual_filter, "enable_dual_filter"); 
				stream.ReadFixed(1, out this.enable_order_hint, "enable_order_hint"); 

				if ( enable_order_hint != 0 )
				{
					stream.ReadFixed(1, out this.enable_jnt_comp, "enable_jnt_comp"); 
					stream.ReadFixed(1, out this.enable_ref_frame_mvs, "enable_ref_frame_mvs"); 
				}
				else 
				{
					enable_jnt_comp= 0;
					enable_ref_frame_mvs= 0;
				}
				stream.ReadFixed(1, out this.seq_choose_screen_content_tools, "seq_choose_screen_content_tools"); 

				if ( seq_choose_screen_content_tools != 0 )
				{
					seq_force_screen_content_tools= SELECT_SCREEN_CONTENT_TOOLS;
				}
				else 
				{
					stream.ReadFixed(1, out this.seq_force_screen_content_tools, "seq_force_screen_content_tools"); 
				}

				if ( seq_force_screen_content_tools > 0 )
				{
					stream.ReadFixed(1, out this.seq_choose_integer_mv, "seq_choose_integer_mv"); 

					if ( seq_choose_integer_mv != 0 )
					{
						seq_force_integer_mv= SELECT_INTEGER_MV;
					}
					else 
					{
						stream.ReadFixed(1, out this.seq_force_integer_mv, "seq_force_integer_mv"); 
					}
				}
				else 
				{
					seq_force_integer_mv= SELECT_INTEGER_MV;
				}

				if ( enable_order_hint != 0 )
				{
					stream.ReadFixed(3, out this.order_hint_bits_minus_1, "order_hint_bits_minus_1"); 
					OrderHintBits= order_hint_bits_minus_1 + 1;
				}
				else 
				{
					OrderHintBits= 0;
				}
			}
			stream.ReadFixed(1, out this.enable_superres, "enable_superres"); 
			stream.ReadFixed(1, out this.enable_cdef, "enable_cdef"); 
			stream.ReadFixed(1, out this.enable_restoration, "enable_restoration"); 
			ReadColorConfig(); 
			stream.ReadFixed(1, out this.film_grain_params_present, "film_grain_params_present"); 
         }

         public void WriteSequenceHeaderObu()
         {

			stream.WriteFixed(3, this.seq_profile, "seq_profile"); 
			stream.WriteFixed(1, this.still_picture, "still_picture"); 
			stream.WriteFixed(1, this.reduced_still_picture_header, "reduced_still_picture_header"); 

			if ( reduced_still_picture_header != 0 )
			{
				timing_info_present_flag= 0;
				decoder_model_info_present_flag= 0;
				initial_display_delay_present_flag= 0;
				operating_points_cnt_minus_1= 0;
				operating_point_idc[ 0 ]= 0;
				stream.WriteFixed(5, this.seq_level_idx[ 0 ], "seq_level_idx"); 
				seq_tier[ 0 ]= 0;
				decoder_model_present_for_this_op[ 0 ]= 0;
				initial_display_delay_present_for_this_op[ 0 ]= 0;
			}
			else 
			{
				stream.WriteFixed(1, this.timing_info_present_flag, "timing_info_present_flag"); 

				if ( timing_info_present_flag != 0 )
				{
					WriteTimingInfo(); 
					stream.WriteFixed(1, this.decoder_model_info_present_flag, "decoder_model_info_present_flag"); 

					if ( decoder_model_info_present_flag != 0 )
					{
						WriteDecoderModelInfo(); 
					}
				}
				else 
				{
					decoder_model_info_present_flag= 0;
				}
				stream.WriteFixed(1, this.initial_display_delay_present_flag, "initial_display_delay_present_flag"); 
				stream.WriteFixed(5, this.operating_points_cnt_minus_1, "operating_points_cnt_minus_1"); 

				for ( i = 0; i <= operating_points_cnt_minus_1; i++ )
				{
					WriteOperatingPointIdc; 
					stream.WriteFixed(12, this.operating_point_idc[ i ], "operating_point_idc"); 
					stream.WriteFixed(5, this.seq_level_idx[ i ], "seq_level_idx"); 

					if ( seq_level_idx[ i ] > 7 )
					{
						stream.WriteFixed(1, this.seq_tier[ i ], "seq_tier"); 
					}
					else 
					{
						seq_tier[ i ]= 0;
					}

					if ( decoder_model_info_present_flag != 0 )
					{
						stream.WriteFixed(1, this.decoder_model_present_for_this_op[ i ], "decoder_model_present_for_this_op"); 

						if ( decoder_model_present_for_this_op[ i ] != 0 )
						{
							WriteOperatingParametersInfo( i ); 
						}
					}
					else 
					{
						decoder_model_present_for_this_op[ i ]= 0;
					}

					if ( initial_display_delay_present_flag != 0 )
					{
						stream.WriteFixed(1, this.initial_display_delay_present_for_this_op[ i ], "initial_display_delay_present_for_this_op"); 

						if ( initial_display_delay_present_for_this_op[ i ] != 0 )
						{
							WriteInitialDisplayDelayMinus1; 
							stream.WriteFixed(4, this.initial_display_delay_minus_1[ i ], "initial_display_delay_minus_1"); 
						}
					}
				}
			}
			uint operatingPoint= choose_operating_point();
			uint OperatingPointIdc= operating_point_idc[ operatingPoint ];
			stream.WriteFixed(4, this.frame_width_bits_minus_1, "frame_width_bits_minus_1"); 
			stream.WriteFixed(4, this.frame_height_bits_minus_1, "frame_height_bits_minus_1"); 
			uint n= frame_width_bits_minus_1 + 1;
			stream.WriteVariable(n, this.max_frame_width_minus_1, "max_frame_width_minus_1"); 
			n= frame_height_bits_minus_1 + 1;
			stream.WriteVariable(n, this.max_frame_height_minus_1, "max_frame_height_minus_1"); 

			if ( reduced_still_picture_header != 0 )
			{
				frame_id_numbers_present_flag= 0;
			}
			else 
			{
				stream.WriteFixed(1, this.frame_id_numbers_present_flag, "frame_id_numbers_present_flag"); 
			}

			if ( frame_id_numbers_present_flag != 0 )
			{
				stream.WriteFixed(4, this.delta_frame_id_length_minus_2, "delta_frame_id_length_minus_2"); 
				stream.WriteFixed(3, this.additional_frame_id_length_minus_1, "additional_frame_id_length_minus_1"); 
			}
			stream.WriteFixed(1, this.use_128x128_superblock, "use_128x128_superblock"); 
			stream.WriteFixed(1, this.enable_filter_intra, "enable_filter_intra"); 
			stream.WriteFixed(1, this.enable_intra_edge_filter, "enable_intra_edge_filter"); 

			if ( reduced_still_picture_header != 0 )
			{
				enable_interintra_compound= 0;
				enable_masked_compound= 0;
				enable_warped_motion= 0;
				enable_dual_filter= 0;
				enable_order_hint= 0;
				enable_jnt_comp= 0;
				enable_ref_frame_mvs= 0;
				seq_force_screen_content_tools= SELECT_SCREEN_CONTENT_TOOLS;
				seq_force_integer_mv= SELECT_INTEGER_MV;
				OrderHintBits= 0;
			}
			else 
			{
				stream.WriteFixed(1, this.enable_interintra_compound, "enable_interintra_compound"); 
				stream.WriteFixed(1, this.enable_masked_compound, "enable_masked_compound"); 
				stream.WriteFixed(1, this.enable_warped_motion, "enable_warped_motion"); 
				stream.WriteFixed(1, this.enable_dual_filter, "enable_dual_filter"); 
				stream.WriteFixed(1, this.enable_order_hint, "enable_order_hint"); 

				if ( enable_order_hint != 0 )
				{
					stream.WriteFixed(1, this.enable_jnt_comp, "enable_jnt_comp"); 
					stream.WriteFixed(1, this.enable_ref_frame_mvs, "enable_ref_frame_mvs"); 
				}
				else 
				{
					enable_jnt_comp= 0;
					enable_ref_frame_mvs= 0;
				}
				stream.WriteFixed(1, this.seq_choose_screen_content_tools, "seq_choose_screen_content_tools"); 

				if ( seq_choose_screen_content_tools != 0 )
				{
					seq_force_screen_content_tools= SELECT_SCREEN_CONTENT_TOOLS;
				}
				else 
				{
					stream.WriteFixed(1, this.seq_force_screen_content_tools, "seq_force_screen_content_tools"); 
				}

				if ( seq_force_screen_content_tools > 0 )
				{
					stream.WriteFixed(1, this.seq_choose_integer_mv, "seq_choose_integer_mv"); 

					if ( seq_choose_integer_mv != 0 )
					{
						seq_force_integer_mv= SELECT_INTEGER_MV;
					}
					else 
					{
						stream.WriteFixed(1, this.seq_force_integer_mv, "seq_force_integer_mv"); 
					}
				}
				else 
				{
					seq_force_integer_mv= SELECT_INTEGER_MV;
				}

				if ( enable_order_hint != 0 )
				{
					stream.WriteFixed(3, this.order_hint_bits_minus_1, "order_hint_bits_minus_1"); 
					OrderHintBits= order_hint_bits_minus_1 + 1;
				}
				else 
				{
					OrderHintBits= 0;
				}
			}
			stream.WriteFixed(1, this.enable_superres, "enable_superres"); 
			stream.WriteFixed(1, this.enable_cdef, "enable_cdef"); 
			stream.WriteFixed(1, this.enable_restoration, "enable_restoration"); 
			WriteColorConfig(); 
			stream.WriteFixed(1, this.film_grain_params_present, "film_grain_params_present"); 
         }

    /*


timing_info() { 
 num_units_in_display_tick f(32)
 time_scale f(32)
 equal_picture_interval f(1)
 if ( equal_picture_interval )
 num_ticks_per_picture_minus_1 num_ticks_per_picture_minus_1 uvlc()
}
    */
		private uint num_units_in_display_tick;
		public uint NumUnitsInDisplayTick { get { return num_units_in_display_tick; } set { num_units_in_display_tick = value; } }
		private uint time_scale;
		public uint TimeScale { get { return time_scale; } set { time_scale = value; } }
		private uint equal_picture_interval;
		public uint EqualPictureInterval { get { return equal_picture_interval; } set { equal_picture_interval = value; } }
		private uint num_ticks_per_picture_minus_1;
		public uint NumTicksPerPictureMinus1 { get { return num_ticks_per_picture_minus_1; } set { num_ticks_per_picture_minus_1 = value; } }

         public void ReadTimingInfo()
         {

			stream.ReadFixed(32, out this.num_units_in_display_tick, "num_units_in_display_tick"); 
			stream.ReadFixed(32, out this.time_scale, "time_scale"); 
			stream.ReadFixed(1, out this.equal_picture_interval, "equal_picture_interval"); 

			if ( equal_picture_interval != 0 )
			{
				ReadNumTicksPerPictureMinus1; 
			}
			stream.ReadUvlc( out this.num_ticks_per_picture_minus_1, "num_ticks_per_picture_minus_1"); 
         }

         public void WriteTimingInfo()
         {

			stream.WriteFixed(32, this.num_units_in_display_tick, "num_units_in_display_tick"); 
			stream.WriteFixed(32, this.time_scale, "time_scale"); 
			stream.WriteFixed(1, this.equal_picture_interval, "equal_picture_interval"); 

			if ( equal_picture_interval != 0 )
			{
				WriteNumTicksPerPictureMinus1; 
			}
			stream.WriteUvlc( this.num_ticks_per_picture_minus_1, "num_ticks_per_picture_minus_1"); 
         }

    /*


decoder_model_info() { 
 buffer_delay_length_minus_1 f(5)
 num_units_in_decoding_tick f(32)
 buffer_removal_time_length_minus_1 f(5)
 frame_presentation_time_length_minus_1 f(5)
}
    */
		private uint buffer_delay_length_minus_1;
		public uint BufferDelayLengthMinus1 { get { return buffer_delay_length_minus_1; } set { buffer_delay_length_minus_1 = value; } }
		private uint num_units_in_decoding_tick;
		public uint NumUnitsInDecodingTick { get { return num_units_in_decoding_tick; } set { num_units_in_decoding_tick = value; } }
		private uint buffer_removal_time_length_minus_1;
		public uint BufferRemovalTimeLengthMinus1 { get { return buffer_removal_time_length_minus_1; } set { buffer_removal_time_length_minus_1 = value; } }
		private uint frame_presentation_time_length_minus_1;
		public uint FramePresentationTimeLengthMinus1 { get { return frame_presentation_time_length_minus_1; } set { frame_presentation_time_length_minus_1 = value; } }

         public void ReadDecoderModelInfo()
         {

			stream.ReadFixed(5, out this.buffer_delay_length_minus_1, "buffer_delay_length_minus_1"); 
			stream.ReadFixed(32, out this.num_units_in_decoding_tick, "num_units_in_decoding_tick"); 
			stream.ReadFixed(5, out this.buffer_removal_time_length_minus_1, "buffer_removal_time_length_minus_1"); 
			stream.ReadFixed(5, out this.frame_presentation_time_length_minus_1, "frame_presentation_time_length_minus_1"); 
         }

         public void WriteDecoderModelInfo()
         {

			stream.WriteFixed(5, this.buffer_delay_length_minus_1, "buffer_delay_length_minus_1"); 
			stream.WriteFixed(32, this.num_units_in_decoding_tick, "num_units_in_decoding_tick"); 
			stream.WriteFixed(5, this.buffer_removal_time_length_minus_1, "buffer_removal_time_length_minus_1"); 
			stream.WriteFixed(5, this.frame_presentation_time_length_minus_1, "frame_presentation_time_length_minus_1"); 
         }

    /*


operating_parameters_info( op ) { 
 n = buffer_delay_length_minus_1 + 1
 decoder_buffer_delay[ op ] f(n)
 encoder_buffer_delay[ op ] f(n)
 low_delay_mode_flag[ op ] f(1)
}
    */
		private uint op;
		public uint Op { get { return op; } set { op = value; } }
		private uint[] decoder_buffer_delay;
		public uint[] DecoderBufferDelay { get { return decoder_buffer_delay; } set { decoder_buffer_delay = value; } }
		private uint[] encoder_buffer_delay;
		public uint[] EncoderBufferDelay { get { return encoder_buffer_delay; } set { encoder_buffer_delay = value; } }
		private uint[] low_delay_mode_flag;
		public uint[] LowDelayModeFlag { get { return low_delay_mode_flag; } set { low_delay_mode_flag = value; } }

         public void ReadOperatingParametersInfo(uint op)
         {

			uint n= buffer_delay_length_minus_1 + 1;
			stream.ReadVariable(n, out this.decoder_buffer_delay[ op ], "decoder_buffer_delay"); 
			stream.ReadVariable(n, out this.encoder_buffer_delay[ op ], "encoder_buffer_delay"); 
			stream.ReadFixed(1, out this.low_delay_mode_flag[ op ], "low_delay_mode_flag"); 
         }

         public void WriteOperatingParametersInfo(uint op)
         {

			uint n= buffer_delay_length_minus_1 + 1;
			stream.WriteVariable(n, this.decoder_buffer_delay[ op ], "decoder_buffer_delay"); 
			stream.WriteVariable(n, this.encoder_buffer_delay[ op ], "encoder_buffer_delay"); 
			stream.WriteFixed(1, this.low_delay_mode_flag[ op ], "low_delay_mode_flag"); 
         }

    /*


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
    */
		private uint high_bitdepth;
		public uint HighBitdepth { get { return high_bitdepth; } set { high_bitdepth = value; } }
		private uint twelve_bit;
		public uint TwelveBit { get { return twelve_bit; } set { twelve_bit = value; } }
		private uint mono_chrome;
		public uint MonoChrome { get { return mono_chrome; } set { mono_chrome = value; } }
		private uint color_description_present_flag;
		public uint ColorDescriptionPresentFlag { get { return color_description_present_flag; } set { color_description_present_flag = value; } }
		private uint color_primaries;
		public uint ColorPrimaries { get { return color_primaries; } set { color_primaries = value; } }
		private uint transfer_characteristics;
		public uint TransferCharacteristics { get { return transfer_characteristics; } set { transfer_characteristics = value; } }
		private uint matrix_coefficients;
		public uint MatrixCoefficients { get { return matrix_coefficients; } set { matrix_coefficients = value; } }
		private uint color_range;
		public uint ColorRange { get { return color_range; } set { color_range = value; } }
		private uint subsampling_x;
		public uint Subsamplingx { get { return subsampling_x; } set { subsampling_x = value; } }
		private uint subsampling_y;
		public uint Subsamplingy { get { return subsampling_y; } set { subsampling_y = value; } }
		private uint chroma_sample_position;
		public uint ChromaSamplePosition { get { return chroma_sample_position; } set { chroma_sample_position = value; } }
		private uint separate_uv_delta_q;
		public uint SeparateUvDeltaq { get { return separate_uv_delta_q; } set { separate_uv_delta_q = value; } }

         public void ReadColorConfig()
         {

			stream.ReadFixed(1, out this.high_bitdepth, "high_bitdepth"); 

			if ( seq_profile == 2 && high_bitdepth != 0 )
			{
				stream.ReadFixed(1, out this.twelve_bit, "twelve_bit"); 
				BitDepth= twelve_bit ? 12 : 10;
			}
			else if ( seq_profile <= 2 )
			{
				BitDepth= high_bitdepth ? 10 : 8;
			}

			if ( seq_profile == 1 )
			{
				mono_chrome= 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.mono_chrome, "mono_chrome"); 
			}
			uint NumPlanes= mono_chrome ? 1 : 3;
			stream.ReadFixed(1, out this.color_description_present_flag, "color_description_present_flag"); 

			if ( color_description_present_flag != 0 )
			{
				stream.ReadFixed(8, out this.color_primaries, "color_primaries"); 
				stream.ReadFixed(8, out this.transfer_characteristics, "transfer_characteristics"); 
				stream.ReadFixed(8, out this.matrix_coefficients, "matrix_coefficients"); 
			}
			else 
			{
				color_primaries= CP_UNSPECIFIED;
				transfer_characteristics= TC_UNSPECIFIED;
				matrix_coefficients= MC_UNSPECIFIED;
			}

			if ( mono_chrome != 0 )
			{
				stream.ReadFixed(1, out this.color_range, "color_range"); 
				subsampling_x= 1;
				subsampling_y= 1;
				chroma_sample_position= CSP_UNKNOWN;
				separate_uv_delta_q= 0;
return;
			}
			else if ( color_primaries == CP_BT_709 &&
 transfer_characteristics == TC_SRGB &&
 matrix_coefficients == MC_IDENTITY )
			{
				color_range= 1;
				subsampling_x= 0;
				subsampling_y= 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.color_range, "color_range"); 

				if ( seq_profile == 0 )
				{
					subsampling_x= 1;
					subsampling_y= 1;
				}
				else if ( seq_profile == 1 )
				{
					subsampling_x= 0;
					subsampling_y= 0;
				}
				else 
				{

					if ( BitDepth == 12 )
					{
						stream.ReadFixed(1, out this.subsampling_x, "subsampling_x"); 

						if ( subsampling_x != 0 )
						{
							stream.ReadFixed(1, out this.subsampling_y, "subsampling_y"); 
						}
						else 
						{
							subsampling_y= 0;
						}
					}
					else 
					{
						subsampling_x= 1;
						subsampling_y= 0;
					}
				}

				if ( subsampling_x != 0 && subsampling_y != 0 )
				{
					stream.ReadFixed(2, out this.chroma_sample_position, "chroma_sample_position"); 
				}
			}
			stream.ReadFixed(1, out this.separate_uv_delta_q, "separate_uv_delta_q"); 
         }

         public void WriteColorConfig()
         {

			stream.WriteFixed(1, this.high_bitdepth, "high_bitdepth"); 

			if ( seq_profile == 2 && high_bitdepth != 0 )
			{
				stream.WriteFixed(1, this.twelve_bit, "twelve_bit"); 
				BitDepth= twelve_bit ? 12 : 10;
			}
			else if ( seq_profile <= 2 )
			{
				BitDepth= high_bitdepth ? 10 : 8;
			}

			if ( seq_profile == 1 )
			{
				mono_chrome= 0;
			}
			else 
			{
				stream.WriteFixed(1, this.mono_chrome, "mono_chrome"); 
			}
			uint NumPlanes= mono_chrome ? 1 : 3;
			stream.WriteFixed(1, this.color_description_present_flag, "color_description_present_flag"); 

			if ( color_description_present_flag != 0 )
			{
				stream.WriteFixed(8, this.color_primaries, "color_primaries"); 
				stream.WriteFixed(8, this.transfer_characteristics, "transfer_characteristics"); 
				stream.WriteFixed(8, this.matrix_coefficients, "matrix_coefficients"); 
			}
			else 
			{
				color_primaries= CP_UNSPECIFIED;
				transfer_characteristics= TC_UNSPECIFIED;
				matrix_coefficients= MC_UNSPECIFIED;
			}

			if ( mono_chrome != 0 )
			{
				stream.WriteFixed(1, this.color_range, "color_range"); 
				subsampling_x= 1;
				subsampling_y= 1;
				chroma_sample_position= CSP_UNKNOWN;
				separate_uv_delta_q= 0;
return;
			}
			else if ( color_primaries == CP_BT_709 &&
 transfer_characteristics == TC_SRGB &&
 matrix_coefficients == MC_IDENTITY )
			{
				color_range= 1;
				subsampling_x= 0;
				subsampling_y= 0;
			}
			else 
			{
				stream.WriteFixed(1, this.color_range, "color_range"); 

				if ( seq_profile == 0 )
				{
					subsampling_x= 1;
					subsampling_y= 1;
				}
				else if ( seq_profile == 1 )
				{
					subsampling_x= 0;
					subsampling_y= 0;
				}
				else 
				{

					if ( BitDepth == 12 )
					{
						stream.WriteFixed(1, this.subsampling_x, "subsampling_x"); 

						if ( subsampling_x != 0 )
						{
							stream.WriteFixed(1, this.subsampling_y, "subsampling_y"); 
						}
						else 
						{
							subsampling_y= 0;
						}
					}
					else 
					{
						subsampling_x= 1;
						subsampling_y= 0;
					}
				}

				if ( subsampling_x != 0 && subsampling_y != 0 )
				{
					stream.WriteFixed(2, this.chroma_sample_position, "chroma_sample_position"); 
				}
			}
			stream.WriteFixed(1, this.separate_uv_delta_q, "separate_uv_delta_q"); 
         }

    /*


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
    */

         public void ReadFrameHeaderObu()
         {


			if ( SeenFrameHeader == 1 )
			{
				ReadFrameHeaderCopy(); 
			}
			else 
			{
				SeenFrameHeader= 1;
				ReadUncompressedHeader(); 

				if ( show_existing_frame != 0 )
				{
					ReadDecodeFrameWrapup(); 
					SeenFrameHeader= 0;
				}
				else 
				{
					TileNum= 0;
					SeenFrameHeader= 1;
				}
			}
         }

         public void WriteFrameHeaderObu()
         {


			if ( SeenFrameHeader == 1 )
			{
				WriteFrameHeaderCopy(); 
			}
			else 
			{
				SeenFrameHeader= 1;
				WriteUncompressedHeader(); 

				if ( show_existing_frame != 0 )
				{
					WriteDecodeFrameWrapup(); 
					SeenFrameHeader= 0;
				}
				else 
				{
					TileNum= 0;
					SeenFrameHeader= 1;
				}
			}
         }

    /*


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
 if ( decoder_model_info_present_flag && !equal_picture_interval ) {
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
 }
 return
 }
 frame_type f(2)
 FrameIsIntra = (frame_type == INTRA_ONLY_FRAME || frame_type == KEY_FRAME)
 show_frame show_frame f(1)
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
 current_frame_id current_frame_id f(idLen)
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
 order_hint order_hint f(OrderHintBits)
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
 if ( frame_type == SWITCH_FRAME ||
 ( frame_type == KEY_FRAME && show_frame ) ) {
 refresh_frame_flags = allFrames
 } else {
 refresh_frame_flags f(8)
 }
 if ( !FrameIsIntra || refresh_frame_flags != allFrames ) {
 if ( error_resilient_mode && enable_order_hint ) {
 for ( i = 0; i < NUM_REF_FRAMES; i++) {
 ref_order_hint[ i ] ref_order_hint[ i ] f(OrderHintBits)
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
    */
		private uint show_existing_frame;
		public uint ShowExistingFrame { get { return show_existing_frame; } set { show_existing_frame = value; } }
		private uint frame_to_show_map_idx;
		public uint FrameToShowMapIdx { get { return frame_to_show_map_idx; } set { frame_to_show_map_idx = value; } }
		private uint display_frame_id;
		public uint DisplayFrameId { get { return display_frame_id; } set { display_frame_id = value; } }
		private uint frame_type;
		public uint FrameType { get { return frame_type; } set { frame_type = value; } }
		private uint show_frame;
		public uint ShowFrame { get { return show_frame; } set { show_frame = value; } }
		private uint showable_frame;
		public uint ShowableFrame { get { return showable_frame; } set { showable_frame = value; } }
		private uint error_resilient_mode;
		public uint ErrorResilientMode { get { return error_resilient_mode; } set { error_resilient_mode = value; } }
		private uint disable_cdf_update;
		public uint DisableCdfUpdate { get { return disable_cdf_update; } set { disable_cdf_update = value; } }
		private uint allow_screen_content_tools;
		public uint AllowScreenContentTools { get { return allow_screen_content_tools; } set { allow_screen_content_tools = value; } }
		private uint force_integer_mv;
		public uint ForceIntegerMv { get { return force_integer_mv; } set { force_integer_mv = value; } }
		private uint current_frame_id;
		public uint CurrentFrameId { get { return current_frame_id; } set { current_frame_id = value; } }
		private uint frame_size_override_flag;
		public uint FrameSizeOverrideFlag { get { return frame_size_override_flag; } set { frame_size_override_flag = value; } }
		private uint order_hint;
		public uint OrderHint { get { return order_hint; } set { order_hint = value; } }
		private uint primary_ref_frame;
		public uint PrimaryRefFrame { get { return primary_ref_frame; } set { primary_ref_frame = value; } }
		private uint buffer_removal_time_present_flag;
		public uint BufferRemovalTimePresentFlag { get { return buffer_removal_time_present_flag; } set { buffer_removal_time_present_flag = value; } }
		private uint[] buffer_removal_time;
		public uint[] BufferRemovalTime { get { return buffer_removal_time; } set { buffer_removal_time = value; } }
		private uint refresh_frame_flags;
		public uint RefreshFrameFlags { get { return refresh_frame_flags; } set { refresh_frame_flags = value; } }
		private uint[] ref_order_hint;
		public uint[] RefOrderHint { get { return ref_order_hint; } set { ref_order_hint = value; } }
		private uint allow_intrabc;
		public uint AllowIntrabc { get { return allow_intrabc; } set { allow_intrabc = value; } }
		private uint frame_refs_short_signaling;
		public uint FrameRefsShortSignaling { get { return frame_refs_short_signaling; } set { frame_refs_short_signaling = value; } }
		private uint last_frame_idx;
		public uint LastFrameIdx { get { return last_frame_idx; } set { last_frame_idx = value; } }
		private uint gold_frame_idx;
		public uint GoldFrameIdx { get { return gold_frame_idx; } set { gold_frame_idx = value; } }
		private uint[] ref_frame_idx;
		public uint[] RefFrameIdx { get { return ref_frame_idx; } set { ref_frame_idx = value; } }
		private uint[] delta_frame_id_minus_1;
		public uint[] DeltaFrameIdMinus1 { get { return delta_frame_id_minus_1; } set { delta_frame_id_minus_1 = value; } }
		private uint allow_high_precision_mv;
		public uint AllowHighPrecisionMv { get { return allow_high_precision_mv; } set { allow_high_precision_mv = value; } }
		private uint is_motion_mode_switchable;
		public uint IsMotionModeSwitchable { get { return is_motion_mode_switchable; } set { is_motion_mode_switchable = value; } }
		private uint use_ref_frame_mvs;
		public uint UseRefFrameMvs { get { return use_ref_frame_mvs; } set { use_ref_frame_mvs = value; } }
		private uint disable_frame_end_update_cdf;
		public uint DisableFrameEndUpdateCdf { get { return disable_frame_end_update_cdf; } set { disable_frame_end_update_cdf = value; } }
		private uint allow_warped_motion;
		public uint AllowWarpedMotion { get { return allow_warped_motion; } set { allow_warped_motion = value; } }
		private uint reduced_tx_set;
		public uint ReducedTxSet { get { return reduced_tx_set; } set { reduced_tx_set = value; } }

			uint i = 0;
			uint opNum = 0;
			uint segmentId = 0;
         public void ReadUncompressedHeader()
         {


			if ( frame_id_numbers_present_flag != 0 )
			{
				idLen= ( additional_frame_id_length_minus_1 + delta_frame_id_length_minus_2 + 3 );
			}
			uint allFrames= (1 << NUM_REF_FRAMES) - 1;

			if ( reduced_still_picture_header != 0 )
			{
				show_existing_frame= 0;
				frame_type= KEY_FRAME;
				FrameIsIntra= 1;
				show_frame= 1;
				showable_frame= 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.show_existing_frame, "show_existing_frame"); 

				if ( show_existing_frame == 1 )
				{
					stream.ReadFixed(3, out this.frame_to_show_map_idx, "frame_to_show_map_idx"); 

					if ( decoder_model_info_present_flag != 0 && equal_picture_interval== 0 )
					{
						ReadTemporalPointInfo(); 
					}
					refresh_frame_flags= 0;

					if ( frame_id_numbers_present_flag != 0 )
					{
						stream.ReadVariable(idLen, out this.display_frame_id, "display_frame_id"); 
					}
					frame_type= RefFrameType[ frame_to_show_map_idx ];

					if ( frame_type == KEY_FRAME )
					{
						refresh_frame_flags= allFrames;
					}

					if ( film_grain_params_present != 0 )
					{
						ReadLoadGrainParams( frame_to_show_map_idx ); 
					}
return;
				}
				stream.ReadFixed(2, out this.frame_type, "frame_type"); 
				FrameIsIntra= (frame_type == INTRA_ONLY_FRAME || frame_type == KEY_FRAME) ? (uint)1 : (uint)0;
				ReadShowFrame; 
				stream.ReadFixed(1, out this.show_frame, "show_frame"); 

				if ( show_frame != 0 && decoder_model_info_present_flag != 0 && equal_picture_interval== 0 )
				{
					ReadTemporalPointInfo(); 
				}

				if ( show_frame != 0 )
				{
					showable_frame= frame_type != KEY_FRAME;
				}
				else 
				{
					stream.ReadFixed(1, out this.showable_frame, "showable_frame"); 
				}

				if ( frame_type == SWITCH_FRAME || ( frame_type == KEY_FRAME && show_frame != 0 ) )
				{
					error_resilient_mode= 1;
				}
				else 
				{
					stream.ReadFixed(1, out this.error_resilient_mode, "error_resilient_mode"); 
				}
			}

			if ( frame_type == KEY_FRAME && show_frame != 0 )
			{

				for ( i = 0; i < NUM_REF_FRAMES; i++ )
				{
					RefValid[ i ]= 0;
					RefOrderHint[ i ]= 0;
				}

				for ( i = 0; i < REFS_PER_FRAME; i++ )
				{
					OrderHints[ AV1RefFrames.LAST_FRAME + i ]= 0;
				}
			}
			stream.ReadFixed(1, out this.disable_cdf_update, "disable_cdf_update"); 

			if ( seq_force_screen_content_tools == SELECT_SCREEN_CONTENT_TOOLS )
			{
				stream.ReadFixed(1, out this.allow_screen_content_tools, "allow_screen_content_tools"); 
			}
			else 
			{
				allow_screen_content_tools= seq_force_screen_content_tools;
			}

			if ( allow_screen_content_tools != 0 )
			{

				if ( seq_force_integer_mv == SELECT_INTEGER_MV )
				{
					stream.ReadFixed(1, out this.force_integer_mv, "force_integer_mv"); 
				}
				else 
				{
					force_integer_mv= seq_force_integer_mv;
				}
			}
			else 
			{
				force_integer_mv= 0;
			}

			if ( FrameIsIntra != 0 )
			{
				force_integer_mv= 1;
			}

			if ( frame_id_numbers_present_flag != 0 )
			{
				PrevFrameID= current_frame_id;
				ReadCurrentFrameId; 
				stream.ReadVariable(idLen, out this.current_frame_id, "current_frame_id"); 
				ReadMarkRefFrames( idLen ); 
			}
			else 
			{
				current_frame_id= 0;
			}

			if ( frame_type == SWITCH_FRAME )
			{
				frame_size_override_flag= 1;
			}
			else if ( reduced_still_picture_header != 0 )
			{
				frame_size_override_flag= 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.frame_size_override_flag, "frame_size_override_flag"); 
			}
			ReadOrderHint; 
			stream.ReadVariable(OrderHintBits, out this.order_hint, "order_hint"); 
			uint OrderHint= order_hint;

			if ( FrameIsIntra != 0 || error_resilient_mode != 0 )
			{
				primary_ref_frame= PRIMARY_REF_NONE;
			}
			else 
			{
				stream.ReadFixed(3, out this.primary_ref_frame, "primary_ref_frame"); 
			}

			if ( decoder_model_info_present_flag != 0 )
			{
				stream.ReadFixed(1, out this.buffer_removal_time_present_flag, "buffer_removal_time_present_flag"); 

				if ( buffer_removal_time_present_flag != 0 )
				{

					this.buffer_removal_time = new uint[ operating_points_cnt_minus_1];
					for ( opNum = 0; opNum <= operating_points_cnt_minus_1; opNum++ )
					{

						if ( decoder_model_present_for_this_op[ opNum ] != 0 )
						{
							opPtIdc= operating_point_idc[ opNum ];
							inTemporalLayer= ( opPtIdc >> temporal_id ) & 1;
							inSpatialLayer= ( opPtIdc >> ( spatial_id + 8 ) ) & 1;

							if ( opPtIdc == 0 || ( inTemporalLayer != 0 && inSpatialLayer != 0 ) )
							{
								n= buffer_removal_time[opNum]_length_minus_1 + 1;
								stream.ReadVariable(n, out this.buffer_removal_time[ opNum ], "buffer_removal_time"); 
							}
						}
					}
				}
			}
			allow_high_precision_mv= 0;
			use_ref_frame_mvs= 0;
			allow_intrabc= 0;

			if ( frame_type == SWITCH_FRAME ||
 ( frame_type == KEY_FRAME && show_frame != 0 ) )
			{
				refresh_frame_flags= allFrames;
			}
			else 
			{
				stream.ReadFixed(8, out this.refresh_frame_flags, "refresh_frame_flags"); 
			}

			if ( FrameIsIntra== 0 || refresh_frame_flags != allFrames )
			{

				if ( error_resilient_mode != 0 && enable_order_hint != 0 )
				{

					this.ref_order_hint = new uint[ NUM_REF_FRAMES];
					for ( i = 0; i < NUM_REF_FRAMES; i++)
					{
						ReadRefOrderHint; 
						stream.ReadVariable(OrderHintBits, out this.ref_order_hint[ i ], "ref_order_hint"); 

						if ( ref_order_hint[ i ] != RefOrderHint[ i ] )
						{
							RefValid[ i ]= 0;
						}
					}
				}
			}

			if (  FrameIsIntra != 0 )
			{
				ReadFrameSize(); 
				ReadRenderSize(); 

				if ( allow_screen_content_tools != 0 && UpscaledWidth == FrameWidth )
				{
					stream.ReadFixed(1, out this.allow_intrabc, "allow_intrabc"); 
				}
			}
			else 
			{

				if ( enable_order_hint== 0 )
				{
					frame_refs_short_signaling= 0;
				}
				else 
				{
					stream.ReadFixed(1, out this.frame_refs_short_signaling, "frame_refs_short_signaling"); 

					if ( frame_refs_short_signaling != 0 )
					{
						stream.ReadFixed(3, out this.last_frame_idx, "last_frame_idx"); 
						stream.ReadFixed(3, out this.gold_frame_idx, "gold_frame_idx"); 
						ReadSetFrameRefs(); 
					}
				}

				this.ref_frame_idx = new uint[ REFS_PER_FRAME];
				this.delta_frame_id_minus_1 = new uint[ REFS_PER_FRAME];
				for ( i = 0; i < REFS_PER_FRAME; i++ )
				{

					if ( frame_refs_short_signaling== 0 )
					{
						stream.ReadFixed(3, out this.ref_frame_idx[ i ], "ref_frame_idx"); 
					}

					if ( frame_id_numbers_present_flag != 0 )
					{
						n= delta_frame_id_length_minus_2 + 2;
						stream.ReadVariable(n, out this.delta_frame_id_minus_1[ i ], "delta_frame_id_minus_1"); 
						DeltaFrameId= delta_frame_id_minus_1[i] + 1;
						expectedFrameId[ i ]= ((current_frame_id + (1 << idLen) - DeltaFrameId ) % (1 << idLen));
					}
				}

				if ( frame_size_override_flag != 0 && error_resilient_mode== 0 )
				{
					ReadFrameSizeWithRefs(); 
				}
				else 
				{
					ReadFrameSize(); 
					ReadRenderSize(); 
				}

				if ( force_integer_mv != 0 )
				{
					allow_high_precision_mv= 0;
				}
				else 
				{
					stream.ReadFixed(1, out this.allow_high_precision_mv, "allow_high_precision_mv"); 
				}
				ReadReadInterpolationFilter(); 
				stream.ReadFixed(1, out this.is_motion_mode_switchable, "is_motion_mode_switchable"); 

				if ( error_resilient_mode != 0 || enable_ref_frame_mvs== 0 )
				{
					use_ref_frame_mvs= 0;
				}
				else 
				{
					stream.ReadFixed(1, out this.use_ref_frame_mvs, "use_ref_frame_mvs"); 
				}

				for ( i = 0; i < REFS_PER_FRAME; i++ )
				{
					refFrame= AV1RefFrames.LAST_FRAME + i;
					hint= RefOrderHint[ ref_frame_idx[ i ] ];
					OrderHints[ refFrame ]= hint;

					if ( enable_order_hint== 0 )
					{
						RefFrameSignBias[ refFrame ]= 0;
					}
					else 
					{
						RefFrameSignBias[ refFrame ]= get_relative_dist( hint, OrderHint) > 0 ? (uint)1 : (uint)0;
					}
				}
			}

			if ( reduced_still_picture_header != 0 || disable_cdf_update != 0 )
			{
				disable_frame_end_update_cdf= 1;
			}
			else 
			{
				stream.ReadFixed(1, out this.disable_frame_end_update_cdf, "disable_frame_end_update_cdf"); 
			}

			if ( primary_ref_frame == PRIMARY_REF_NONE )
			{
				ReadInitNonCoeffCdfs(); 
				ReadSetupPastIndependence(); 
			}
			else 
			{
				ReadLoadCdfs( ref_frame_idx[ primary_ref_frame ] ); 
				ReadLoadPrevious(); 
			}

			if ( use_ref_frame_mvs == 1 )
			{
				ReadMotionFieldEstimation(); 
			}
			ReadTileInfo(); 
			ReadQuantizationParams(); 
			ReadSegmentationParams(); 
			ReadDeltaqParams(); 
			ReadDeltaLfParams(); 

			if ( primary_ref_frame == PRIMARY_REF_NONE )
			{
				ReadInitCoeffCdfs(); 
			}
			else 
			{
				ReadLoadPreviousSegmentIds(); 
			}
			uint CodedLossless= 1;

			for ( segmentId = 0; segmentId < MAX_SEGMENTS; segmentId++ )
			{
				qindex= get_qindex( 1, segmentId );
				LosslessArray[ segmentId ]= qindex == 0 && DeltaQYDc == 0 && DeltaQUAc == 0 && DeltaQUDc == 0 && DeltaQVAc == 0 && DeltaQVDc == 0 ? (uint)1 : (uint)0;

				if ( LosslessArray[ segmentId ]== 0 )
				{
					CodedLossless= 0;
				}

				if ( using_qmatrix != 0 )
				{

					if ( LosslessArray[ segmentId ] != 0 )
					{
						SegQMLevel[ 0 ][ segmentId ]= 15;
						SegQMLevel[ 1 ][ segmentId ]= 15;
						SegQMLevel[ 2 ][ segmentId ]= 15;
					}
					else 
					{
						SegQMLevel[ 0 ][ segmentId ]= qm_y;
						SegQMLevel[ 1 ][ segmentId ]= qm_u;
						SegQMLevel[ 2 ][ segmentId ]= qm_v;
					}
				}
			}
			uint AllLossless= CodedLossless && ( FrameWidth == UpscaledWidth ) ? (uint)1 : (uint)0;
			ReadLoopFilterParams(); 
			ReadCdefParams(); 
			ReadLrParams(); 
			ReadReadTxMode(); 
			ReadFrameReferenceMode(); 
			ReadSkipModeParams(); 

			if ( FrameIsIntra != 0 || error_resilient_mode != 0 || enable_warped_motion== 0 )
			{
				allow_warped_motion= 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.allow_warped_motion, "allow_warped_motion"); 
			}
			stream.ReadFixed(1, out this.reduced_tx_set, "reduced_tx_set"); 
			ReadGlobalMotionParams(); 
			ReadFilmGrainParams(); 
         }

         public void WriteUncompressedHeader()
         {


			if ( frame_id_numbers_present_flag != 0 )
			{
				idLen= ( additional_frame_id_length_minus_1 + delta_frame_id_length_minus_2 + 3 );
			}
			uint allFrames= (1 << NUM_REF_FRAMES) - 1;

			if ( reduced_still_picture_header != 0 )
			{
				show_existing_frame= 0;
				frame_type= KEY_FRAME;
				FrameIsIntra= 1;
				show_frame= 1;
				showable_frame= 0;
			}
			else 
			{
				stream.WriteFixed(1, this.show_existing_frame, "show_existing_frame"); 

				if ( show_existing_frame == 1 )
				{
					stream.WriteFixed(3, this.frame_to_show_map_idx, "frame_to_show_map_idx"); 

					if ( decoder_model_info_present_flag != 0 && equal_picture_interval== 0 )
					{
						WriteTemporalPointInfo(); 
					}
					refresh_frame_flags= 0;

					if ( frame_id_numbers_present_flag != 0 )
					{
						stream.WriteVariable(idLen, this.display_frame_id, "display_frame_id"); 
					}
					frame_type= RefFrameType[ frame_to_show_map_idx ];

					if ( frame_type == KEY_FRAME )
					{
						refresh_frame_flags= allFrames;
					}

					if ( film_grain_params_present != 0 )
					{
						WriteLoadGrainParams( frame_to_show_map_idx ); 
					}
return;
				}
				stream.WriteFixed(2, this.frame_type, "frame_type"); 
				FrameIsIntra= (frame_type == INTRA_ONLY_FRAME || frame_type == KEY_FRAME) ? (uint)1 : (uint)0;
				WriteShowFrame; 
				stream.WriteFixed(1, this.show_frame, "show_frame"); 

				if ( show_frame != 0 && decoder_model_info_present_flag != 0 && equal_picture_interval== 0 )
				{
					WriteTemporalPointInfo(); 
				}

				if ( show_frame != 0 )
				{
					showable_frame= frame_type != KEY_FRAME;
				}
				else 
				{
					stream.WriteFixed(1, this.showable_frame, "showable_frame"); 
				}

				if ( frame_type == SWITCH_FRAME || ( frame_type == KEY_FRAME && show_frame != 0 ) )
				{
					error_resilient_mode= 1;
				}
				else 
				{
					stream.WriteFixed(1, this.error_resilient_mode, "error_resilient_mode"); 
				}
			}

			if ( frame_type == KEY_FRAME && show_frame != 0 )
			{

				for ( i = 0; i < NUM_REF_FRAMES; i++ )
				{
					RefValid[ i ]= 0;
					RefOrderHint[ i ]= 0;
				}

				for ( i = 0; i < REFS_PER_FRAME; i++ )
				{
					OrderHints[ AV1RefFrames.LAST_FRAME + i ]= 0;
				}
			}
			stream.WriteFixed(1, this.disable_cdf_update, "disable_cdf_update"); 

			if ( seq_force_screen_content_tools == SELECT_SCREEN_CONTENT_TOOLS )
			{
				stream.WriteFixed(1, this.allow_screen_content_tools, "allow_screen_content_tools"); 
			}
			else 
			{
				allow_screen_content_tools= seq_force_screen_content_tools;
			}

			if ( allow_screen_content_tools != 0 )
			{

				if ( seq_force_integer_mv == SELECT_INTEGER_MV )
				{
					stream.WriteFixed(1, this.force_integer_mv, "force_integer_mv"); 
				}
				else 
				{
					force_integer_mv= seq_force_integer_mv;
				}
			}
			else 
			{
				force_integer_mv= 0;
			}

			if ( FrameIsIntra != 0 )
			{
				force_integer_mv= 1;
			}

			if ( frame_id_numbers_present_flag != 0 )
			{
				PrevFrameID= current_frame_id;
				WriteCurrentFrameId; 
				stream.WriteVariable(idLen, this.current_frame_id, "current_frame_id"); 
				WriteMarkRefFrames( idLen ); 
			}
			else 
			{
				current_frame_id= 0;
			}

			if ( frame_type == SWITCH_FRAME )
			{
				frame_size_override_flag= 1;
			}
			else if ( reduced_still_picture_header != 0 )
			{
				frame_size_override_flag= 0;
			}
			else 
			{
				stream.WriteFixed(1, this.frame_size_override_flag, "frame_size_override_flag"); 
			}
			WriteOrderHint; 
			stream.WriteVariable(OrderHintBits, this.order_hint, "order_hint"); 
			uint OrderHint= order_hint;

			if ( FrameIsIntra != 0 || error_resilient_mode != 0 )
			{
				primary_ref_frame= PRIMARY_REF_NONE;
			}
			else 
			{
				stream.WriteFixed(3, this.primary_ref_frame, "primary_ref_frame"); 
			}

			if ( decoder_model_info_present_flag != 0 )
			{
				stream.WriteFixed(1, this.buffer_removal_time_present_flag, "buffer_removal_time_present_flag"); 

				if ( buffer_removal_time_present_flag != 0 )
				{

					for ( opNum = 0; opNum <= operating_points_cnt_minus_1; opNum++ )
					{

						if ( decoder_model_present_for_this_op[ opNum ] != 0 )
						{
							opPtIdc= operating_point_idc[ opNum ];
							inTemporalLayer= ( opPtIdc >> temporal_id ) & 1;
							inSpatialLayer= ( opPtIdc >> ( spatial_id + 8 ) ) & 1;

							if ( opPtIdc == 0 || ( inTemporalLayer != 0 && inSpatialLayer != 0 ) )
							{
								n= buffer_removal_time[opNum]_length_minus_1 + 1;
								stream.WriteVariable(n, this.buffer_removal_time[ opNum ], "buffer_removal_time"); 
							}
						}
					}
				}
			}
			allow_high_precision_mv= 0;
			use_ref_frame_mvs= 0;
			allow_intrabc= 0;

			if ( frame_type == SWITCH_FRAME ||
 ( frame_type == KEY_FRAME && show_frame != 0 ) )
			{
				refresh_frame_flags= allFrames;
			}
			else 
			{
				stream.WriteFixed(8, this.refresh_frame_flags, "refresh_frame_flags"); 
			}

			if ( FrameIsIntra== 0 || refresh_frame_flags != allFrames )
			{

				if ( error_resilient_mode != 0 && enable_order_hint != 0 )
				{

					for ( i = 0; i < NUM_REF_FRAMES; i++)
					{
						WriteRefOrderHint; 
						stream.WriteVariable(OrderHintBits, this.ref_order_hint[ i ], "ref_order_hint"); 

						if ( ref_order_hint[ i ] != RefOrderHint[ i ] )
						{
							RefValid[ i ]= 0;
						}
					}
				}
			}

			if (  FrameIsIntra != 0 )
			{
				WriteFrameSize(); 
				WriteRenderSize(); 

				if ( allow_screen_content_tools != 0 && UpscaledWidth == FrameWidth )
				{
					stream.WriteFixed(1, this.allow_intrabc, "allow_intrabc"); 
				}
			}
			else 
			{

				if ( enable_order_hint== 0 )
				{
					frame_refs_short_signaling= 0;
				}
				else 
				{
					stream.WriteFixed(1, this.frame_refs_short_signaling, "frame_refs_short_signaling"); 

					if ( frame_refs_short_signaling != 0 )
					{
						stream.WriteFixed(3, this.last_frame_idx, "last_frame_idx"); 
						stream.WriteFixed(3, this.gold_frame_idx, "gold_frame_idx"); 
						WriteSetFrameRefs(); 
					}
				}

				for ( i = 0; i < REFS_PER_FRAME; i++ )
				{

					if ( frame_refs_short_signaling== 0 )
					{
						stream.WriteFixed(3, this.ref_frame_idx[ i ], "ref_frame_idx"); 
					}

					if ( frame_id_numbers_present_flag != 0 )
					{
						n= delta_frame_id_length_minus_2 + 2;
						stream.WriteVariable(n, this.delta_frame_id_minus_1[ i ], "delta_frame_id_minus_1"); 
						DeltaFrameId= delta_frame_id_minus_1[i] + 1;
						expectedFrameId[ i ]= ((current_frame_id + (1 << idLen) - DeltaFrameId ) % (1 << idLen));
					}
				}

				if ( frame_size_override_flag != 0 && error_resilient_mode== 0 )
				{
					WriteFrameSizeWithRefs(); 
				}
				else 
				{
					WriteFrameSize(); 
					WriteRenderSize(); 
				}

				if ( force_integer_mv != 0 )
				{
					allow_high_precision_mv= 0;
				}
				else 
				{
					stream.WriteFixed(1, this.allow_high_precision_mv, "allow_high_precision_mv"); 
				}
				WriteReadInterpolationFilter(); 
				stream.WriteFixed(1, this.is_motion_mode_switchable, "is_motion_mode_switchable"); 

				if ( error_resilient_mode != 0 || enable_ref_frame_mvs== 0 )
				{
					use_ref_frame_mvs= 0;
				}
				else 
				{
					stream.WriteFixed(1, this.use_ref_frame_mvs, "use_ref_frame_mvs"); 
				}

				for ( i = 0; i < REFS_PER_FRAME; i++ )
				{
					refFrame= AV1RefFrames.LAST_FRAME + i;
					hint= RefOrderHint[ ref_frame_idx[ i ] ];
					OrderHints[ refFrame ]= hint;

					if ( enable_order_hint== 0 )
					{
						RefFrameSignBias[ refFrame ]= 0;
					}
					else 
					{
						RefFrameSignBias[ refFrame ]= get_relative_dist( hint, OrderHint) > 0 ? (uint)1 : (uint)0;
					}
				}
			}

			if ( reduced_still_picture_header != 0 || disable_cdf_update != 0 )
			{
				disable_frame_end_update_cdf= 1;
			}
			else 
			{
				stream.WriteFixed(1, this.disable_frame_end_update_cdf, "disable_frame_end_update_cdf"); 
			}

			if ( primary_ref_frame == PRIMARY_REF_NONE )
			{
				WriteInitNonCoeffCdfs(); 
				WriteSetupPastIndependence(); 
			}
			else 
			{
				WriteLoadCdfs( ref_frame_idx[ primary_ref_frame ] ); 
				WriteLoadPrevious(); 
			}

			if ( use_ref_frame_mvs == 1 )
			{
				WriteMotionFieldEstimation(); 
			}
			WriteTileInfo(); 
			WriteQuantizationParams(); 
			WriteSegmentationParams(); 
			WriteDeltaqParams(); 
			WriteDeltaLfParams(); 

			if ( primary_ref_frame == PRIMARY_REF_NONE )
			{
				WriteInitCoeffCdfs(); 
			}
			else 
			{
				WriteLoadPreviousSegmentIds(); 
			}
			uint CodedLossless= 1;

			for ( segmentId = 0; segmentId < MAX_SEGMENTS; segmentId++ )
			{
				qindex= get_qindex( 1, segmentId );
				LosslessArray[ segmentId ]= qindex == 0 && DeltaQYDc == 0 && DeltaQUAc == 0 && DeltaQUDc == 0 && DeltaQVAc == 0 && DeltaQVDc == 0 ? (uint)1 : (uint)0;

				if ( LosslessArray[ segmentId ]== 0 )
				{
					CodedLossless= 0;
				}

				if ( using_qmatrix != 0 )
				{

					if ( LosslessArray[ segmentId ] != 0 )
					{
						SegQMLevel[ 0 ][ segmentId ]= 15;
						SegQMLevel[ 1 ][ segmentId ]= 15;
						SegQMLevel[ 2 ][ segmentId ]= 15;
					}
					else 
					{
						SegQMLevel[ 0 ][ segmentId ]= qm_y;
						SegQMLevel[ 1 ][ segmentId ]= qm_u;
						SegQMLevel[ 2 ][ segmentId ]= qm_v;
					}
				}
			}
			uint AllLossless= CodedLossless && ( FrameWidth == UpscaledWidth ) ? (uint)1 : (uint)0;
			WriteLoopFilterParams(); 
			WriteCdefParams(); 
			WriteLrParams(); 
			WriteReadTxMode(); 
			WriteFrameReferenceMode(); 
			WriteSkipModeParams(); 

			if ( FrameIsIntra != 0 || error_resilient_mode != 0 || enable_warped_motion== 0 )
			{
				allow_warped_motion= 0;
			}
			else 
			{
				stream.WriteFixed(1, this.allow_warped_motion, "allow_warped_motion"); 
			}
			stream.WriteFixed(1, this.reduced_tx_set, "reduced_tx_set"); 
			WriteGlobalMotionParams(); 
			WriteFilmGrainParams(); 
         }

    /*


 temporal_point_info() { 
 n = frame_presentation_time_length_minus_1 + 1
 frame_presentation_time f(n)
 }
    */
		private uint frame_presentation_time;
		public uint FramePresentationTime { get { return frame_presentation_time; } set { frame_presentation_time = value; } }

         public void ReadTemporalPointInfo()
         {

			uint n= frame_presentation_time_length_minus_1 + 1;
			stream.ReadVariable(n, out this.frame_presentation_time, "frame_presentation_time"); 
         }

         public void WriteTemporalPointInfo()
         {

			uint n= frame_presentation_time_length_minus_1 + 1;
			stream.WriteVariable(n, this.frame_presentation_time, "frame_presentation_time"); 
         }

    /*


 mark_ref_frames( idLen ) { 
 diffLen = delta_frame_id_length_minus_2 + 2
 for ( i = 0; i < NUM_REF_FRAMES; i++ ) {
 if ( current_frame_id > ( 1 << diffLen ) ) {
 if ( RefFrameId[ i ] > current_frame_id ||
 RefFrameId[ i ] < ( current_frame_id - ( 1 << diffLen ) ) )
 RefValid[ i ] = 0
 } else {
 if ( RefFrameId[ i ] > current_frame_id &&
 RefFrameId[ i ] < ( ( 1 << idLen ) +
 current_frame_id 
( 1 << diffLen ) ) )
 RefValid[ i ] = 0
 }
 }
 }
    */
		private uint idLen;
		public uint IdLen { get { return idLen; } set { idLen = value; } }

			uint i = 0;
         public void ReadMarkRefFrames(uint idLen)
         {

			uint diffLen= delta_frame_id_length_minus_2 + 2;

			for ( i = 0; i < NUM_REF_FRAMES; i++ )
			{

				if ( current_frame_id > ( 1 << (int) diffLen ) )
				{

					if ( RefFrameId[ i ] > current_frame_id ||
 RefFrameId[ i ] < ( current_frame_id - ( 1 << (int) diffLen ) ) )
					{
						RefValid[ i ]= 0;
					}
				}
				else 
				{

					if ( RefFrameId[ i ] > current_frame_id &&
 RefFrameId[ i ] < ( ( 1 << (int) idLen ) +
 current_frame_id 
( 1 << (int) diffLen ) ) )
					{
						RefValid[ i ]= 0;
					}
				}
			}
         }

         public void WriteMarkRefFrames(uint idLen)
         {

			uint diffLen= delta_frame_id_length_minus_2 + 2;

			for ( i = 0; i < NUM_REF_FRAMES; i++ )
			{

				if ( current_frame_id > ( 1 << (int) diffLen ) )
				{

					if ( RefFrameId[ i ] > current_frame_id ||
 RefFrameId[ i ] < ( current_frame_id - ( 1 << (int) diffLen ) ) )
					{
						RefValid[ i ]= 0;
					}
				}
				else 
				{

					if ( RefFrameId[ i ] > current_frame_id &&
 RefFrameId[ i ] < ( ( 1 << (int) idLen ) +
 current_frame_id 
( 1 << (int) diffLen ) ) )
					{
						RefValid[ i ]= 0;
					}
				}
			}
         }

    /*


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
    */
		private uint frame_width_minus_1;
		public uint FrameWidthMinus1 { get { return frame_width_minus_1; } set { frame_width_minus_1 = value; } }
		private uint frame_height_minus_1;
		public uint FrameHeightMinus1 { get { return frame_height_minus_1; } set { frame_height_minus_1 = value; } }

         public void ReadFrameSize()
         {


			if ( frame_size_override_flag != 0 )
			{
				n= frame_width_bits_minus_1 + 1;
				stream.ReadVariable(n, out this.frame_width_minus_1, "frame_width_minus_1"); 
				n= frame_height_bits_minus_1 + 1;
				stream.ReadVariable(n, out this.frame_height_minus_1, "frame_height_minus_1"); 
				FrameWidth= frame_width_minus_1 + 1;
				FrameHeight= frame_height_minus_1 + 1;
			}
			else 
			{
				FrameWidth= max_frame_width_minus_1 + 1;
				FrameHeight= max_frame_height_minus_1 + 1;
			}
			ReadSuperresParams(); 
			ReadComputeImageSize(); 
         }

         public void WriteFrameSize()
         {


			if ( frame_size_override_flag != 0 )
			{
				n= frame_width_bits_minus_1 + 1;
				stream.WriteVariable(n, this.frame_width_minus_1, "frame_width_minus_1"); 
				n= frame_height_bits_minus_1 + 1;
				stream.WriteVariable(n, this.frame_height_minus_1, "frame_height_minus_1"); 
				FrameWidth= frame_width_minus_1 + 1;
				FrameHeight= frame_height_minus_1 + 1;
			}
			else 
			{
				FrameWidth= max_frame_width_minus_1 + 1;
				FrameHeight= max_frame_height_minus_1 + 1;
			}
			WriteSuperresParams(); 
			WriteComputeImageSize(); 
         }

    /*


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
    */
		private uint render_and_frame_size_different;
		public uint RenderAndFrameSizeDifferent { get { return render_and_frame_size_different; } set { render_and_frame_size_different = value; } }
		private uint render_width_minus_1;
		public uint RenderWidthMinus1 { get { return render_width_minus_1; } set { render_width_minus_1 = value; } }
		private uint render_height_minus_1;
		public uint RenderHeightMinus1 { get { return render_height_minus_1; } set { render_height_minus_1 = value; } }

         public void ReadRenderSize()
         {

			stream.ReadFixed(1, out this.render_and_frame_size_different, "render_and_frame_size_different"); 

			if ( render_and_frame_size_different == 1 )
			{
				stream.ReadFixed(16, out this.render_width_minus_1, "render_width_minus_1"); 
				stream.ReadFixed(16, out this.render_height_minus_1, "render_height_minus_1"); 
				RenderWidth= render_width_minus_1 + 1;
				RenderHeight= render_height_minus_1 + 1;
			}
			else 
			{
				RenderWidth= UpscaledWidth;
				RenderHeight= FrameHeight;
			}
         }

         public void WriteRenderSize()
         {

			stream.WriteFixed(1, this.render_and_frame_size_different, "render_and_frame_size_different"); 

			if ( render_and_frame_size_different == 1 )
			{
				stream.WriteFixed(16, this.render_width_minus_1, "render_width_minus_1"); 
				stream.WriteFixed(16, this.render_height_minus_1, "render_height_minus_1"); 
				RenderWidth= render_width_minus_1 + 1;
				RenderHeight= render_height_minus_1 + 1;
			}
			else 
			{
				RenderWidth= UpscaledWidth;
				RenderHeight= FrameHeight;
			}
         }

    /*


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
    */
		private uint[] found_ref;
		public uint[] FoundRef { get { return found_ref; } set { found_ref = value; } }

			uint i = 0;
         public void ReadFrameSizeWithRefs()
         {


			this.found_ref = new uint[ REFS_PER_FRAME];
			for ( i = 0; i < REFS_PER_FRAME; i++ )
			{
				stream.ReadFixed(1, out this.found_ref[ i ], "found_ref"); 

				if ( found_ref[i] == 1 )
				{
					UpscaledWidth= RefUpscaledWidth[ ref_frame_idx[ i ] ];
					FrameWidth= UpscaledWidth;
					FrameHeight= RefFrameHeight[ ref_frame_idx[ i ] ];
					RenderWidth= RefRenderWidth[ ref_frame_idx[ i ] ];
					RenderHeight= RefRenderHeight[ ref_frame_idx[ i ] ];
break;
				}
			}

			if ( found_ref == 0 )
			{
				ReadFrameSize(); 
				ReadRenderSize(); 
			}
			else 
			{
				ReadSuperresParams(); 
				ReadComputeImageSize(); 
			}
         }

         public void WriteFrameSizeWithRefs()
         {


			for ( i = 0; i < REFS_PER_FRAME; i++ )
			{
				stream.WriteFixed(1, this.found_ref[ i ], "found_ref"); 

				if ( found_ref[i] == 1 )
				{
					UpscaledWidth= RefUpscaledWidth[ ref_frame_idx[ i ] ];
					FrameWidth= UpscaledWidth;
					FrameHeight= RefFrameHeight[ ref_frame_idx[ i ] ];
					RenderWidth= RefRenderWidth[ ref_frame_idx[ i ] ];
					RenderHeight= RefRenderHeight[ ref_frame_idx[ i ] ];
break;
				}
			}

			if ( found_ref == 0 )
			{
				WriteFrameSize(); 
				WriteRenderSize(); 
			}
			else 
			{
				WriteSuperresParams(); 
				WriteComputeImageSize(); 
			}
         }

    /*


 read_interpolation_filter() { 
 is_filter_switchable f(1)
 if ( is_filter_switchable == 1 ) {
 interpolation_filter = SWITCHABLE
 } else {
 interpolation_filter f(2)
 }
 }
    */
		private uint is_filter_switchable;
		public uint IsFilterSwitchable { get { return is_filter_switchable; } set { is_filter_switchable = value; } }
		private uint interpolation_filter;
		public uint InterpolationFilter { get { return interpolation_filter; } set { interpolation_filter = value; } }

         public void ReadReadInterpolationFilter()
         {

			stream.ReadFixed(1, out this.is_filter_switchable, "is_filter_switchable"); 

			if ( is_filter_switchable == 1 )
			{
				interpolation_filter= SWITCHABLE;
			}
			else 
			{
				stream.ReadFixed(2, out this.interpolation_filter, "interpolation_filter"); 
			}
         }

         public void WriteReadInterpolationFilter()
         {

			stream.WriteFixed(1, this.is_filter_switchable, "is_filter_switchable"); 

			if ( is_filter_switchable == 1 )
			{
				interpolation_filter= SWITCHABLE;
			}
			else 
			{
				stream.WriteFixed(2, this.interpolation_filter, "interpolation_filter"); 
			}
         }

    /*


 get_relative_dist( a, b ) { 
 if ( !enable_order_hint )
 return 0
 diff = a - b
 m = 1 << (OrderHintBits - 1)
 diff = (diff & (m - 1)) - (diff & m)
 return diff
}
    */
		private uint a;
		public uint a { get { return a; } set { a = value; } }
		private uint b;
		public uint b { get { return b; } set { b = value; } }

         public void ReadGetRelativeDist(uint a, uint b)
         {


			if ( enable_order_hint== 0 )
			{
return 0;
			}
			uint diff= a - b;
			uint m= 1 << (OrderHintBits - 1);
			diff= (diff & (m - 1)) - (diff & m);
return diff;
         }

         public void WriteGetRelativeDist(uint a, uint b)
         {


			if ( enable_order_hint== 0 )
			{
return 0;
			}
			uint diff= a - b;
			uint m= 1 << (OrderHintBits - 1);
			diff= (diff & (m - 1)) - (diff & m);
return diff;
         }

    /*


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
    */
		private uint uniform_tile_spacing_flag;
		public uint UniformTileSpacingFlag { get { return uniform_tile_spacing_flag; } set { uniform_tile_spacing_flag = value; } }
		private Dictionary<int, uint> increment_tile_cols_log2 = new Dictionary<int, uint>();
		public Dictionary<int, uint> IncrementTileColsLog2 { get { return increment_tile_cols_log2; } set { increment_tile_cols_log2 = value; } }
		private Dictionary<int, uint> increment_tile_rows_log2 = new Dictionary<int, uint>();
		public Dictionary<int, uint> IncrementTileRowsLog2 { get { return increment_tile_rows_log2; } set { increment_tile_rows_log2 = value; } }
		private uint[] width_in_sbs_minus_1;
		public uint[] WidthInSbsMinus1 { get { return width_in_sbs_minus_1; } set { width_in_sbs_minus_1 = value; } }
		private uint[] height_in_sbs_minus_1;
		public uint[] HeightInSbsMinus1 { get { return height_in_sbs_minus_1; } set { height_in_sbs_minus_1 = value; } }
		private uint context_update_tile_id;
		public uint ContextUpdateTileId { get { return context_update_tile_id; } set { context_update_tile_id = value; } }
		private uint tile_size_bytes_minus_1;
		public uint TileSizeBytesMinus1 { get { return tile_size_bytes_minus_1; } set { tile_size_bytes_minus_1 = value; } }

			int whileIndex = -1;
			uint startSb = 0;
			uint i = 0;
         public void ReadTileInfo()
         {

			uint sbCols= use_128x128_superblock ? ( ( MiCols + 31 ) >> 5 ) : ( ( MiCols + 15 ) >> 4 );
			uint sbRows= use_128x128_superblock ? ( ( MiRows + 31 ) >> 5 ) : ( ( MiRows + 15 ) >> 4 );
			uint sbShift= use_128x128_superblock ? 5 : 4;
			uint sbSize= sbShift + 2;
			uint maxTileWidthSb= MAX_TILE_WIDTH >> sbSize;
			uint maxTileAreaSb= MAX_TILE_AREA >> ( 2 * sbSize );
			uint minLog2TileCols= tile_log2(maxTileWidthSb, sbCols);
			uint maxLog2TileCols= tile_log2(1, Math.Min(sbCols, MAX_TILE_COLS));
			uint maxLog2TileRows= tile_log2(1, Math.Min(sbRows, MAX_TILE_ROWS));
			uint minLog2Tiles= Math.Max(minLog2TileCols, tile_log2(maxTileAreaSb, sbRows * sbCols));
			stream.ReadFixed(1, out this.uniform_tile_spacing_flag, "uniform_tile_spacing_flag"); 

			if ( uniform_tile_spacing_flag != 0 )
			{
				TileColsLog2= minLog2TileCols;

				while ( TileColsLog2 < maxLog2TileCols )
				{
					whileIndex++;

					stream.ReadFixed(1, out this.increment_tile_cols_log2, "increment_tile_cols_log2"); 

					if ( increment_tile_cols_log2[whileIndex] == 1 )
					{
						TileColsLog2++;
					}
					else 
					{
break;
					}
				}
				tileWidthSb= (sbCols + (1 << TileColsLog2) - 1) >> TileColsLog2;
				i= 0;

				for ( startSb = 0; startSb < sbCols; startSb += tileWidthSb )
				{
					MiColStarts[ i ]= startSb << sbShift;
					i+= 1;
				}
				MiColStarts[i]= MiCols;
				TileCols= i;
				minLog2TileRows= Math.Max( minLog2Tiles - TileColsLog2, 0);
				TileRowsLog2= minLog2TileRows;

				while ( TileRowsLog2 < maxLog2TileRows )
				{
					whileIndex++;

					stream.ReadFixed(1, out this.increment_tile_rows_log2, "increment_tile_rows_log2"); 

					if ( increment_tile_rows_log2[whileIndex] == 1 )
					{
						TileRowsLog2++;
					}
					else 
					{
break;
					}
				}
				tileHeightSb= (sbRows + (1 << TileRowsLog2) - 1) >> TileRowsLog2;
				i= 0;

				for ( startSb = 0; startSb < sbRows; startSb += tileHeightSb )
				{
					MiRowStarts[ i ]= startSb << sbShift;
					i+= 1;
				}
				MiRowStarts[i]= MiRows;
				TileRows= i;
			}
			else 
			{
				widestTileSb= 0;
				startSb= 0;

				this.width_in_sbs_minus_1 = new uint[ sbCols];
				for ( i = 0; startSb < sbCols; i++ )
				{
					MiColStarts[ i ]= startSb << sbShift;
					maxWidth= Math.Min(sbCols - startSb, maxTileWidthSb);
					stream.ReadUnsignedInt(maxWidth, out this.width_in_sbs_minus_1[ i ], "width_in_sbs_minus_1"); 
					sizeSb= width_in_sbs_minus_1[i] + 1;
					widestTileSb= Math.Max( sizeSb, widestTileSb );
					startSb+= sizeSb;
				}
				MiColStarts[i]= MiCols;
				TileCols= i;
				TileColsLog2= tile_log2(1, TileCols);

				if ( minLog2Tiles > 0 )
				{
					maxTileAreaSb= (sbRows * sbCols) >> (minLog2Tiles + 1);
				}
				else 
				{
					maxTileAreaSb= sbRows * sbCols;
				}
				maxTileHeightSb= Math.Max( maxTileAreaSb / widestTileSb, 1 );
				startSb= 0;

				this.height_in_sbs_minus_1 = new uint[ sbRows];
				for ( i = 0; startSb < sbRows; i++ )
				{
					MiRowStarts[ i ]= startSb << sbShift;
					maxHeight= Math.Min(sbRows - startSb, maxTileHeightSb);
					stream.ReadUnsignedInt(maxHeight, out this.height_in_sbs_minus_1[ i ], "height_in_sbs_minus_1"); 
					sizeSb= height_in_sbs_minus_1[i] + 1;
					startSb+= sizeSb;
				}
				MiRowStarts[ i ]= MiRows;
				TileRows= i;
				TileRowsLog2= tile_log2(1, TileRows);
			}

			if ( TileColsLog2 > 0 || TileRowsLog2 > 0 )
			{
				stream.ReadVariable(TileRowsLog2+TileColsLog2, out this.context_update_tile_id, "context_update_tile_id"); 
				stream.ReadFixed(2, out this.tile_size_bytes_minus_1, "tile_size_bytes_minus_1"); 
				TileSizeBytes= tile_size_bytes_minus_1 + 1;
			}
			else 
			{
				context_update_tile_id= 0;
			}
         }

         public void WriteTileInfo()
         {

			uint sbCols= use_128x128_superblock ? ( ( MiCols + 31 ) >> 5 ) : ( ( MiCols + 15 ) >> 4 );
			uint sbRows= use_128x128_superblock ? ( ( MiRows + 31 ) >> 5 ) : ( ( MiRows + 15 ) >> 4 );
			uint sbShift= use_128x128_superblock ? 5 : 4;
			uint sbSize= sbShift + 2;
			uint maxTileWidthSb= MAX_TILE_WIDTH >> sbSize;
			uint maxTileAreaSb= MAX_TILE_AREA >> ( 2 * sbSize );
			uint minLog2TileCols= tile_log2(maxTileWidthSb, sbCols);
			uint maxLog2TileCols= tile_log2(1, Math.Min(sbCols, MAX_TILE_COLS));
			uint maxLog2TileRows= tile_log2(1, Math.Min(sbRows, MAX_TILE_ROWS));
			uint minLog2Tiles= Math.Max(minLog2TileCols, tile_log2(maxTileAreaSb, sbRows * sbCols));
			stream.WriteFixed(1, this.uniform_tile_spacing_flag, "uniform_tile_spacing_flag"); 

			if ( uniform_tile_spacing_flag != 0 )
			{
				TileColsLog2= minLog2TileCols;

				while ( TileColsLog2 < maxLog2TileCols )
				{
					whileIndex++;

					stream.WriteFixed(1, this.increment_tile_cols_log2, "increment_tile_cols_log2"); 

					if ( increment_tile_cols_log2[whileIndex] == 1 )
					{
						TileColsLog2++;
					}
					else 
					{
break;
					}
				}
				tileWidthSb= (sbCols + (1 << TileColsLog2) - 1) >> TileColsLog2;
				i= 0;

				for ( startSb = 0; startSb < sbCols; startSb += tileWidthSb )
				{
					MiColStarts[ i ]= startSb << sbShift;
					i+= 1;
				}
				MiColStarts[i]= MiCols;
				TileCols= i;
				minLog2TileRows= Math.Max( minLog2Tiles - TileColsLog2, 0);
				TileRowsLog2= minLog2TileRows;

				while ( TileRowsLog2 < maxLog2TileRows )
				{
					whileIndex++;

					stream.WriteFixed(1, this.increment_tile_rows_log2, "increment_tile_rows_log2"); 

					if ( increment_tile_rows_log2[whileIndex] == 1 )
					{
						TileRowsLog2++;
					}
					else 
					{
break;
					}
				}
				tileHeightSb= (sbRows + (1 << TileRowsLog2) - 1) >> TileRowsLog2;
				i= 0;

				for ( startSb = 0; startSb < sbRows; startSb += tileHeightSb )
				{
					MiRowStarts[ i ]= startSb << sbShift;
					i+= 1;
				}
				MiRowStarts[i]= MiRows;
				TileRows= i;
			}
			else 
			{
				widestTileSb= 0;
				startSb= 0;

				for ( i = 0; startSb < sbCols; i++ )
				{
					MiColStarts[ i ]= startSb << sbShift;
					maxWidth= Math.Min(sbCols - startSb, maxTileWidthSb);
					stream.WriteUnsignedInt(maxWidth, this.width_in_sbs_minus_1[ i ], "width_in_sbs_minus_1"); 
					sizeSb= width_in_sbs_minus_1[i] + 1;
					widestTileSb= Math.Max( sizeSb, widestTileSb );
					startSb+= sizeSb;
				}
				MiColStarts[i]= MiCols;
				TileCols= i;
				TileColsLog2= tile_log2(1, TileCols);

				if ( minLog2Tiles > 0 )
				{
					maxTileAreaSb= (sbRows * sbCols) >> (minLog2Tiles + 1);
				}
				else 
				{
					maxTileAreaSb= sbRows * sbCols;
				}
				maxTileHeightSb= Math.Max( maxTileAreaSb / widestTileSb, 1 );
				startSb= 0;

				for ( i = 0; startSb < sbRows; i++ )
				{
					MiRowStarts[ i ]= startSb << sbShift;
					maxHeight= Math.Min(sbRows - startSb, maxTileHeightSb);
					stream.WriteUnsignedInt(maxHeight, this.height_in_sbs_minus_1[ i ], "height_in_sbs_minus_1"); 
					sizeSb= height_in_sbs_minus_1[i] + 1;
					startSb+= sizeSb;
				}
				MiRowStarts[ i ]= MiRows;
				TileRows= i;
				TileRowsLog2= tile_log2(1, TileRows);
			}

			if ( TileColsLog2 > 0 || TileRowsLog2 > 0 )
			{
				stream.WriteVariable(TileRowsLog2+TileColsLog2, this.context_update_tile_id, "context_update_tile_id"); 
				stream.WriteFixed(2, this.tile_size_bytes_minus_1, "tile_size_bytes_minus_1"); 
				TileSizeBytes= tile_size_bytes_minus_1 + 1;
			}
			else 
			{
				context_update_tile_id= 0;
			}
         }

    /*



tile_log2( blkSize, target ) { 
 for ( k = 0; (blkSize << k) < target; k++ ) {
 }
 return k
 }
    */
		private uint blkSize;
		public uint BlkSize { get { return blkSize; } set { blkSize = value; } }
		private uint target;
		public uint Target { get { return target; } set { target = value; } }

			uint k = 0;
         public void ReadTileLog2(uint blkSize, uint target)
         {


			for ( k = 0; (blkSize << (int) k) < target; k++ )
			{
			}
return k;
         }

         public void WriteTileLog2(uint blkSize, uint target)
         {


			for ( k = 0; (blkSize << (int) k) < target; k++ )
			{
			}
return k;
         }

    /*


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
    */
		private uint base_q_idx;
		public uint BaseqIdx { get { return base_q_idx; } set { base_q_idx = value; } }
		private uint diff_uv_delta;
		public uint DiffUvDelta { get { return diff_uv_delta; } set { diff_uv_delta = value; } }
		private uint using_qmatrix;
		public uint UsingQmatrix { get { return using_qmatrix; } set { using_qmatrix = value; } }
		private uint qm_y;
		public uint Qmy { get { return qm_y; } set { qm_y = value; } }
		private uint qm_u;
		public uint Qmu { get { return qm_u; } set { qm_u = value; } }
		private uint qm_v;
		public uint Qmv { get { return qm_v; } set { qm_v = value; } }

         public void ReadQuantizationParams()
         {

			stream.ReadFixed(8, out this.base_q_idx, "base_q_idx"); 
			uint DeltaQYDc= read_delta_q();

			if ( NumPlanes > 1 )
			{

				if ( separate_uv_delta_q != 0 )
				{
					stream.ReadFixed(1, out this.diff_uv_delta, "diff_uv_delta"); 
				}
				else 
				{
					diff_uv_delta= 0;
				}
				DeltaQUDc= read_delta_q();
				DeltaQUAc= read_delta_q();

				if ( diff_uv_delta != 0 )
				{
					DeltaQVDc= read_delta_q();
					DeltaQVAc= read_delta_q();
				}
				else 
				{
					DeltaQVDc= DeltaQUDc;
					DeltaQVAc= DeltaQUAc;
				}
			}
			else 
			{
				DeltaQUDc= 0;
				DeltaQUAc= 0;
				DeltaQVDc= 0;
				DeltaQVAc= 0;
			}
			stream.ReadFixed(1, out this.using_qmatrix, "using_qmatrix"); 

			if ( using_qmatrix != 0 )
			{
				stream.ReadFixed(4, out this.qm_y, "qm_y"); 
				stream.ReadFixed(4, out this.qm_u, "qm_u"); 

				if ( separate_uv_delta_q== 0 )
				{
					qm_v= qm_u;
				}
				else 
				{
					stream.ReadFixed(4, out this.qm_v, "qm_v"); 
				}
			}
         }

         public void WriteQuantizationParams()
         {

			stream.WriteFixed(8, this.base_q_idx, "base_q_idx"); 
			uint DeltaQYDc= read_delta_q();

			if ( NumPlanes > 1 )
			{

				if ( separate_uv_delta_q != 0 )
				{
					stream.WriteFixed(1, this.diff_uv_delta, "diff_uv_delta"); 
				}
				else 
				{
					diff_uv_delta= 0;
				}
				DeltaQUDc= read_delta_q();
				DeltaQUAc= read_delta_q();

				if ( diff_uv_delta != 0 )
				{
					DeltaQVDc= read_delta_q();
					DeltaQVAc= read_delta_q();
				}
				else 
				{
					DeltaQVDc= DeltaQUDc;
					DeltaQVAc= DeltaQUAc;
				}
			}
			else 
			{
				DeltaQUDc= 0;
				DeltaQUAc= 0;
				DeltaQVDc= 0;
				DeltaQVAc= 0;
			}
			stream.WriteFixed(1, this.using_qmatrix, "using_qmatrix"); 

			if ( using_qmatrix != 0 )
			{
				stream.WriteFixed(4, this.qm_y, "qm_y"); 
				stream.WriteFixed(4, this.qm_u, "qm_u"); 

				if ( separate_uv_delta_q== 0 )
				{
					qm_v= qm_u;
				}
				else 
				{
					stream.WriteFixed(4, this.qm_v, "qm_v"); 
				}
			}
         }

    /*


 read_delta_q() { 
 delta_coded f(1)
 if ( delta_coded ) {
 delta_q su(1+6)
 } else {
 delta_q = 0
 }
 return delta_q
 }
    */
		private uint delta_coded;
		public uint DeltaCoded { get { return delta_coded; } set { delta_coded = value; } }
		private uint delta_q;
		public uint Deltaq { get { return delta_q; } set { delta_q = value; } }

         public void ReadReadDeltaq()
         {

			stream.ReadFixed(1, out this.delta_coded, "delta_coded"); 

			if ( delta_coded != 0 )
			{
				stream.ReadSignedIntVar(1+6, out this.delta_q, "delta_q"); 
			}
			else 
			{
				delta_q= 0;
			}
return delta_q;
         }

         public void WriteReadDeltaq()
         {

			stream.WriteFixed(1, this.delta_coded, "delta_coded"); 

			if ( delta_coded != 0 )
			{
				stream.WriteSignedIntVar(1+6, this.delta_q, "delta_q"); 
			}
			else 
			{
				delta_q= 0;
			}
return delta_q;
         }

    /*


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
 feature_value feature_value su(1+bitsToRead)
 clippedValue = Clip3( -limit, limit, feature_value)
 } else {
 feature_value feature_value f(bitsToRead)
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
    */
		private uint segmentation_enabled;
		public uint SegmentationEnabled { get { return segmentation_enabled; } set { segmentation_enabled = value; } }
		private uint segmentation_update_map;
		public uint SegmentationUpdateMap { get { return segmentation_update_map; } set { segmentation_update_map = value; } }
		private uint segmentation_temporal_update;
		public uint SegmentationTemporalUpdate { get { return segmentation_temporal_update; } set { segmentation_temporal_update = value; } }
		private uint segmentation_update_data;
		public uint SegmentationUpdateData { get { return segmentation_update_data; } set { segmentation_update_data = value; } }
		private uint[][] feature_enabled;
		public uint[][] FeatureEnabled { get { return feature_enabled; } set { feature_enabled = value; } }
		private uint[][] feature_value;
		public uint[][] FeatureValue { get { return feature_value; } set { feature_value = value; } }

			uint i = 0;
			uint j = 0;
         public void ReadSegmentationParams()
         {

			stream.ReadFixed(1, out this.segmentation_enabled, "segmentation_enabled"); 

			if ( segmentation_enabled == 1 )
			{

				if ( primary_ref_frame == PRIMARY_REF_NONE )
				{
					segmentation_update_map= 1;
					segmentation_temporal_update= 0;
					segmentation_update_data= 1;
				}
				else 
				{
					stream.ReadFixed(1, out this.segmentation_update_map, "segmentation_update_map"); 

					if ( segmentation_update_map == 1 )
					{
						stream.ReadFixed(1, out this.segmentation_temporal_update, "segmentation_temporal_update"); 
					}
					stream.ReadFixed(1, out this.segmentation_update_data, "segmentation_update_data"); 
				}

				if ( segmentation_update_data == 1 )
				{

					this.feature_enabled = new uint[ MAX_SEGMENTS][];
					this.feature_value = new uint[ MAX_SEGMENTS][];
					for ( i = 0; i < MAX_SEGMENTS; i++ )
					{

						this.feature_enabled[ i ] = new uint[ SEG_LVL_MAX];
						this.feature_value[ i ] = new uint[ SEG_LVL_MAX];
						for ( j = 0; j < SEG_LVL_MAX; j++ )
						{
							feature_value= 0;
							stream.ReadFixed(1, out this.feature_enabled[ i ][ j ], "feature_enabled"); 
							FeatureEnabled[ i ][ j ]= feature_enabled[i][j];
							clippedValue= 0;

							if ( feature_enabled[i][j] == 1 )
							{
								bitsToRead= Segmentation_Feature_Bits[ j ];
								limit= Segmentation_Feature_Max[ j ];

								if ( Segmentation_Feature_Signed[ j ] == 1 )
								{
									ReadFeatureValue; 
									stream.ReadSignedIntVar(1+bitsToRead, out this.feature_value[ i ][ j ], "feature_value"); 
									clippedValue= Clip3( -limit, limit, feature_value[i][j]);
								}
								else 
								{
									ReadFeatureValue; 
									stream.ReadVariable(bitsToRead, out this.feature_value[ i ][ j ], "feature_value"); 
									clippedValue= Clip3( 0, limit, feature_value[i][j]);
								}
							}
							FeatureData[ i ][ j ]= clippedValue;
						}
					}
				}
			}
			else 
			{

				for ( i = 0; i < MAX_SEGMENTS; i++ )
				{

					for ( j = 0; j < SEG_LVL_MAX; j++ )
					{
						FeatureEnabled[ i ][ j ]= 0;
						FeatureData[ i ][ j ]= 0;
					}
				}
			}
			uint SegIdPreSkip= 0;
			uint LastActiveSegId= 0;

			for ( i = 0; i < MAX_SEGMENTS; i++ )
			{

				for ( j = 0; j < SEG_LVL_MAX; j++ )
				{

					if ( FeatureEnabled[ i ][ j ] != 0 )
					{
						LastActiveSegId= i;

						if ( j >= SEG_LVL_REF_FRAME )
						{
							SegIdPreSkip= 1;
						}
					}
				}
			}
         }

         public void WriteSegmentationParams()
         {

			stream.WriteFixed(1, this.segmentation_enabled, "segmentation_enabled"); 

			if ( segmentation_enabled == 1 )
			{

				if ( primary_ref_frame == PRIMARY_REF_NONE )
				{
					segmentation_update_map= 1;
					segmentation_temporal_update= 0;
					segmentation_update_data= 1;
				}
				else 
				{
					stream.WriteFixed(1, this.segmentation_update_map, "segmentation_update_map"); 

					if ( segmentation_update_map == 1 )
					{
						stream.WriteFixed(1, this.segmentation_temporal_update, "segmentation_temporal_update"); 
					}
					stream.WriteFixed(1, this.segmentation_update_data, "segmentation_update_data"); 
				}

				if ( segmentation_update_data == 1 )
				{

					for ( i = 0; i < MAX_SEGMENTS; i++ )
					{

						for ( j = 0; j < SEG_LVL_MAX; j++ )
						{
							feature_value= 0;
							stream.WriteFixed(1, this.feature_enabled[ i ][ j ], "feature_enabled"); 
							FeatureEnabled[ i ][ j ]= feature_enabled[i][j];
							clippedValue= 0;

							if ( feature_enabled[i][j] == 1 )
							{
								bitsToRead= Segmentation_Feature_Bits[ j ];
								limit= Segmentation_Feature_Max[ j ];

								if ( Segmentation_Feature_Signed[ j ] == 1 )
								{
									WriteFeatureValue; 
									stream.WriteSignedIntVar(1+bitsToRead, this.feature_value[ i ][ j ], "feature_value"); 
									clippedValue= Clip3( -limit, limit, feature_value[i][j]);
								}
								else 
								{
									WriteFeatureValue; 
									stream.WriteVariable(bitsToRead, this.feature_value[ i ][ j ], "feature_value"); 
									clippedValue= Clip3( 0, limit, feature_value[i][j]);
								}
							}
							FeatureData[ i ][ j ]= clippedValue;
						}
					}
				}
			}
			else 
			{

				for ( i = 0; i < MAX_SEGMENTS; i++ )
				{

					for ( j = 0; j < SEG_LVL_MAX; j++ )
					{
						FeatureEnabled[ i ][ j ]= 0;
						FeatureData[ i ][ j ]= 0;
					}
				}
			}
			uint SegIdPreSkip= 0;
			uint LastActiveSegId= 0;

			for ( i = 0; i < MAX_SEGMENTS; i++ )
			{

				for ( j = 0; j < SEG_LVL_MAX; j++ )
				{

					if ( FeatureEnabled[ i ][ j ] != 0 )
					{
						LastActiveSegId= i;

						if ( j >= SEG_LVL_REF_FRAME )
						{
							SegIdPreSkip= 1;
						}
					}
				}
			}
         }

    /*


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
    */
		private uint delta_q_present;
		public uint DeltaqPresent { get { return delta_q_present; } set { delta_q_present = value; } }
		private uint delta_q_res;
		public uint DeltaqRes { get { return delta_q_res; } set { delta_q_res = value; } }

         public void ReadDeltaqParams()
         {

			delta_q_res= 0;
			delta_q_present= 0;

			if ( base_q_idx > 0 )
			{
				stream.ReadFixed(1, out this.delta_q_present, "delta_q_present"); 
			}

			if ( delta_q_present != 0 )
			{
				stream.ReadFixed(2, out this.delta_q_res, "delta_q_res"); 
			}
         }

         public void WriteDeltaqParams()
         {

			delta_q_res= 0;
			delta_q_present= 0;

			if ( base_q_idx > 0 )
			{
				stream.WriteFixed(1, this.delta_q_present, "delta_q_present"); 
			}

			if ( delta_q_present != 0 )
			{
				stream.WriteFixed(2, this.delta_q_res, "delta_q_res"); 
			}
         }

    /*


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
    */
		private uint delta_lf_present;
		public uint DeltaLfPresent { get { return delta_lf_present; } set { delta_lf_present = value; } }
		private uint delta_lf_res;
		public uint DeltaLfRes { get { return delta_lf_res; } set { delta_lf_res = value; } }
		private uint delta_lf_multi;
		public uint DeltaLfMulti { get { return delta_lf_multi; } set { delta_lf_multi = value; } }

         public void ReadDeltaLfParams()
         {

			delta_lf_present= 0;
			delta_lf_res= 0;
			delta_lf_multi= 0;

			if ( delta_q_present != 0 )
			{

				if ( allow_intrabc== 0 )
				{
					stream.ReadFixed(1, out this.delta_lf_present, "delta_lf_present"); 
				}

				if ( delta_lf_present != 0 )
				{
					stream.ReadFixed(2, out this.delta_lf_res, "delta_lf_res"); 
					stream.ReadFixed(1, out this.delta_lf_multi, "delta_lf_multi"); 
				}
			}
         }

         public void WriteDeltaLfParams()
         {

			delta_lf_present= 0;
			delta_lf_res= 0;
			delta_lf_multi= 0;

			if ( delta_q_present != 0 )
			{

				if ( allow_intrabc== 0 )
				{
					stream.WriteFixed(1, this.delta_lf_present, "delta_lf_present"); 
				}

				if ( delta_lf_present != 0 )
				{
					stream.WriteFixed(2, this.delta_lf_res, "delta_lf_res"); 
					stream.WriteFixed(1, this.delta_lf_multi, "delta_lf_multi"); 
				}
			}
         }

    /*


cdef_params() { 
 if ( CodedLossless || allow_intrabc ||
 !enable_cdef) {
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
    */
		private uint cdef_damping_minus_3;
		public uint CdefDampingMinus3 { get { return cdef_damping_minus_3; } set { cdef_damping_minus_3 = value; } }
		private uint cdef_bits;
		public uint CdefBits { get { return cdef_bits; } set { cdef_bits = value; } }
		private uint[] cdef_y_pri_strength;
		public uint[] CdefyPriStrength { get { return cdef_y_pri_strength; } set { cdef_y_pri_strength = value; } }
		private uint[] cdef_y_sec_strength;
		public uint[] CdefySecStrength { get { return cdef_y_sec_strength; } set { cdef_y_sec_strength = value; } }
		private uint[] cdef_uv_pri_strength;
		public uint[] CdefUvPriStrength { get { return cdef_uv_pri_strength; } set { cdef_uv_pri_strength = value; } }
		private uint[] cdef_uv_sec_strength;
		public uint[] CdefUvSecStrength { get { return cdef_uv_sec_strength; } set { cdef_uv_sec_strength = value; } }

			uint i = 0;
         public void ReadCdefParams()
         {


			if ( CodedLossless != 0 || allow_intrabc != 0 ||
 enable_cdef== 0)
			{
				cdef_bits= 0;
				cdef_y_pri_strength[0]= 0;
				cdef_y_sec_strength[0]= 0;
				cdef_uv_pri_strength[0]= 0;
				cdef_uv_sec_strength[0]= 0;
				CdefDamping= 3;
return;
			}
			stream.ReadFixed(2, out this.cdef_damping_minus_3, "cdef_damping_minus_3"); 
			uint CdefDamping= cdef_damping_minus_3 + 3;
			stream.ReadFixed(2, out this.cdef_bits, "cdef_bits"); 

			this.cdef_y_pri_strength = new uint[ (1 << (int) cdef_bits)];
			this.cdef_y_sec_strength = new uint[ (1 << (int) cdef_bits)];
			this.cdef_uv_pri_strength = new uint[ (1 << (int) cdef_bits)];
			this.cdef_uv_sec_strength = new uint[ (1 << (int) cdef_bits)];
			for ( i = 0; i < (1 << (int) cdef_bits); i++ )
			{
				stream.ReadFixed(4, out this.cdef_y_pri_strength[i], "cdef_y_pri_strength"); 
				stream.ReadFixed(2, out this.cdef_y_sec_strength[i], "cdef_y_sec_strength"); 

				if ( cdef_y_sec_strength[i] == 3 )
				{
					cdef_y_sec_strength[i]+= 1;
				}

				if ( NumPlanes > 1 )
				{
					stream.ReadFixed(4, out this.cdef_uv_pri_strength[i], "cdef_uv_pri_strength"); 
					stream.ReadFixed(2, out this.cdef_uv_sec_strength[i], "cdef_uv_sec_strength"); 

					if ( cdef_uv_sec_strength[i] == 3 )
					{
						cdef_uv_sec_strength[i]+= 1;
					}
				}
			}
         }

         public void WriteCdefParams()
         {


			if ( CodedLossless != 0 || allow_intrabc != 0 ||
 enable_cdef== 0)
			{
				cdef_bits= 0;
				cdef_y_pri_strength[0]= 0;
				cdef_y_sec_strength[0]= 0;
				cdef_uv_pri_strength[0]= 0;
				cdef_uv_sec_strength[0]= 0;
				CdefDamping= 3;
return;
			}
			stream.WriteFixed(2, this.cdef_damping_minus_3, "cdef_damping_minus_3"); 
			uint CdefDamping= cdef_damping_minus_3 + 3;
			stream.WriteFixed(2, this.cdef_bits, "cdef_bits"); 

			for ( i = 0; i < (1 << (int) cdef_bits); i++ )
			{
				stream.WriteFixed(4, this.cdef_y_pri_strength[i], "cdef_y_pri_strength"); 
				stream.WriteFixed(2, this.cdef_y_sec_strength[i], "cdef_y_sec_strength"); 

				if ( cdef_y_sec_strength[i] == 3 )
				{
					cdef_y_sec_strength[i]+= 1;
				}

				if ( NumPlanes > 1 )
				{
					stream.WriteFixed(4, this.cdef_uv_pri_strength[i], "cdef_uv_pri_strength"); 
					stream.WriteFixed(2, this.cdef_uv_sec_strength[i], "cdef_uv_sec_strength"); 

					if ( cdef_uv_sec_strength[i] == 3 )
					{
						cdef_uv_sec_strength[i]+= 1;
					}
				}
			}
         }

    /*



lr_params() { 
 if ( AllLossless || allow_intrabc ||
 !enable_restoration ) {
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
    */
		private uint[] lr_type;
		public uint[] LrType { get { return lr_type; } set { lr_type = value; } }
		private uint lr_unit_shift;
		public uint LrUnitShift { get { return lr_unit_shift; } set { lr_unit_shift = value; } }
		private uint lr_unit_extra_shift;
		public uint LrUnitExtraShift { get { return lr_unit_extra_shift; } set { lr_unit_extra_shift = value; } }
		private uint lr_uv_shift;
		public uint LrUvShift { get { return lr_uv_shift; } set { lr_uv_shift = value; } }

			uint i = 0;
         public void ReadLrParams()
         {


			if ( AllLossless != 0 || allow_intrabc != 0 ||
 enable_restoration== 0 )
			{
				FrameRestorationType[0]= RESTORE_NONE;
				FrameRestorationType[1]= RESTORE_NONE;
				FrameRestorationType[2]= RESTORE_NONE;
				UsesLr= 0;
return;
			}
			uint UsesLr= 0;
			uint usesChromaLr= 0;

			this.lr_type = new uint[ NumPlanes];
			for ( i = 0; i < NumPlanes; i++ )
			{
				stream.ReadFixed(2, out this.lr_type[ i ], "lr_type"); 
				FrameRestorationType[i]= Remap_Lr_Type[lr_type[i]];

				if ( FrameRestorationType[i] != RESTORE_NONE )
				{
					UsesLr= 1;

					if ( i > 0 )
					{
						usesChromaLr= 1;
					}
				}
			}

			if ( UsesLr != 0 )
			{

				if ( use_128x128_superblock != 0 )
				{
					stream.ReadFixed(1, out this.lr_unit_shift, "lr_unit_shift"); 
					lr_unit_shift++;
				}
				else 
				{
					stream.ReadFixed(1, out this.lr_unit_shift, "lr_unit_shift"); 

					if ( lr_unit_shift != 0 )
					{
						stream.ReadFixed(1, out this.lr_unit_extra_shift, "lr_unit_extra_shift"); 
						lr_unit_shift+= lr_unit_extra_shift;
					}
				}
				LoopRestorationSize[ 0 ]= RESTORATION_TILESIZE_MAX >> (2 - lr_unit_shift);

				if ( subsampling_x != 0 && subsampling_y != 0 && usesChromaLr != 0 )
				{
					stream.ReadFixed(1, out this.lr_uv_shift, "lr_uv_shift"); 
				}
				else 
				{
					lr_uv_shift= 0;
				}
				LoopRestorationSize[ 1 ]= LoopRestorationSize[ 0 ] >> lr_uv_shift;
				LoopRestorationSize[ 2 ]= LoopRestorationSize[ 0 ] >> lr_uv_shift;
			}
         }

         public void WriteLrParams()
         {


			if ( AllLossless != 0 || allow_intrabc != 0 ||
 enable_restoration== 0 )
			{
				FrameRestorationType[0]= RESTORE_NONE;
				FrameRestorationType[1]= RESTORE_NONE;
				FrameRestorationType[2]= RESTORE_NONE;
				UsesLr= 0;
return;
			}
			uint UsesLr= 0;
			uint usesChromaLr= 0;

			for ( i = 0; i < NumPlanes; i++ )
			{
				stream.WriteFixed(2, this.lr_type[ i ], "lr_type"); 
				FrameRestorationType[i]= Remap_Lr_Type[lr_type[i]];

				if ( FrameRestorationType[i] != RESTORE_NONE )
				{
					UsesLr= 1;

					if ( i > 0 )
					{
						usesChromaLr= 1;
					}
				}
			}

			if ( UsesLr != 0 )
			{

				if ( use_128x128_superblock != 0 )
				{
					stream.WriteFixed(1, this.lr_unit_shift, "lr_unit_shift"); 
					lr_unit_shift++;
				}
				else 
				{
					stream.WriteFixed(1, this.lr_unit_shift, "lr_unit_shift"); 

					if ( lr_unit_shift != 0 )
					{
						stream.WriteFixed(1, this.lr_unit_extra_shift, "lr_unit_extra_shift"); 
						lr_unit_shift+= lr_unit_extra_shift;
					}
				}
				LoopRestorationSize[ 0 ]= RESTORATION_TILESIZE_MAX >> (2 - lr_unit_shift);

				if ( subsampling_x != 0 && subsampling_y != 0 && usesChromaLr != 0 )
				{
					stream.WriteFixed(1, this.lr_uv_shift, "lr_uv_shift"); 
				}
				else 
				{
					lr_uv_shift= 0;
				}
				LoopRestorationSize[ 1 ]= LoopRestorationSize[ 0 ] >> lr_uv_shift;
				LoopRestorationSize[ 2 ]= LoopRestorationSize[ 0 ] >> lr_uv_shift;
			}
         }

    /*


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
    */
		private uint[] loop_filter_level;
		public uint[] LoopFilterLevel { get { return loop_filter_level; } set { loop_filter_level = value; } }
		private uint loop_filter_sharpness;
		public uint LoopFilterSharpness { get { return loop_filter_sharpness; } set { loop_filter_sharpness = value; } }
		private uint loop_filter_delta_enabled;
		public uint LoopFilterDeltaEnabled { get { return loop_filter_delta_enabled; } set { loop_filter_delta_enabled = value; } }
		private uint loop_filter_delta_update;
		public uint LoopFilterDeltaUpdate { get { return loop_filter_delta_update; } set { loop_filter_delta_update = value; } }
		private uint[] update_ref_delta;
		public uint[] UpdateRefDelta { get { return update_ref_delta; } set { update_ref_delta = value; } }
		private uint[] loop_filter_ref_deltas;
		public uint[] LoopFilterRefDeltas { get { return loop_filter_ref_deltas; } set { loop_filter_ref_deltas = value; } }
		private uint[] update_mode_delta;
		public uint[] UpdateModeDelta { get { return update_mode_delta; } set { update_mode_delta = value; } }
		private uint[] loop_filter_mode_deltas;
		public uint[] LoopFilterModeDeltas { get { return loop_filter_mode_deltas; } set { loop_filter_mode_deltas = value; } }

			uint i = 0;
         public void ReadLoopFilterParams()
         {


			if ( CodedLossless != 0 || allow_intrabc != 0 )
			{
				loop_filter_level[ 0 ]= 0;
				loop_filter_level[ 1 ]= 0;
				loop_filter_ref_deltas[ AV1RefFrames.INTRA_FRAME ]= 1;
				loop_filter_ref_deltas[ AV1RefFrames.LAST_FRAME ]= 0;
				loop_filter_ref_deltas[ AV1RefFrames.LAST2_FRAME ]= 0;
				loop_filter_ref_deltas[ AV1RefFrames.LAST3_FRAME ]= 0;
				loop_filter_ref_deltas[ AV1RefFrames.BWDREF_FRAME ]= 0;
				loop_filter_ref_deltas[ AV1RefFrames.GOLDEN_FRAME ]= -1;
				loop_filter_ref_deltas[ AV1RefFrames.ALTREF_FRAME ]= -1;
				loop_filter_ref_deltas[ AV1RefFrames.ALTREF2_FRAME ]= -1;

				for ( i = 0; i < 2; i++ )
				{
					loop_filter_mode_deltas[ i ]= 0;
				}
return;
			}
			stream.ReadFixed(6, out this.loop_filter_level[ 0 ], "loop_filter_level"); 
			stream.ReadFixed(6, out this.loop_filter_level[ 1 ], "loop_filter_level"); 

			if ( NumPlanes > 1 )
			{

				if ( loop_filter_level[ 0 ] != 0 || loop_filter_level[ 1 ] != 0 )
				{
					stream.ReadFixed(6, out this.loop_filter_level[ 2 ], "loop_filter_level"); 
					stream.ReadFixed(6, out this.loop_filter_level[ 3 ], "loop_filter_level"); 
				}
			}
			stream.ReadFixed(3, out this.loop_filter_sharpness, "loop_filter_sharpness"); 
			stream.ReadFixed(1, out this.loop_filter_delta_enabled, "loop_filter_delta_enabled"); 

			if ( loop_filter_delta_enabled == 1 )
			{
				stream.ReadFixed(1, out this.loop_filter_delta_update, "loop_filter_delta_update"); 

				if ( loop_filter_delta_update == 1 )
				{

					this.update_ref_delta = new uint[ TOTAL_REFS_PER_FRAME];
					this.loop_filter_ref_deltas = new uint[ TOTAL_REFS_PER_FRAME];
					for ( i = 0; i < TOTAL_REFS_PER_FRAME; i++ )
					{
						stream.ReadFixed(1, out this.update_ref_delta[ i ], "update_ref_delta"); 

						if ( update_ref_delta[i] == 1 )
						{
							stream.ReadSignedIntVar(1+6, out this.loop_filter_ref_deltas[ i ], "loop_filter_ref_deltas"); 
						}
					}

					this.update_mode_delta = new uint[ 2];
					this.loop_filter_mode_deltas = new uint[ 2];
					for ( i = 0; i < 2; i++ )
					{
						stream.ReadFixed(1, out this.update_mode_delta[ i ], "update_mode_delta"); 

						if ( update_mode_delta[i] == 1 )
						{
							stream.ReadSignedIntVar(1+6, out this.loop_filter_mode_deltas[ i ], "loop_filter_mode_deltas"); 
						}
					}
				}
			}
         }

         public void WriteLoopFilterParams()
         {


			if ( CodedLossless != 0 || allow_intrabc != 0 )
			{
				loop_filter_level[ 0 ]= 0;
				loop_filter_level[ 1 ]= 0;
				loop_filter_ref_deltas[ AV1RefFrames.INTRA_FRAME ]= 1;
				loop_filter_ref_deltas[ AV1RefFrames.LAST_FRAME ]= 0;
				loop_filter_ref_deltas[ AV1RefFrames.LAST2_FRAME ]= 0;
				loop_filter_ref_deltas[ AV1RefFrames.LAST3_FRAME ]= 0;
				loop_filter_ref_deltas[ AV1RefFrames.BWDREF_FRAME ]= 0;
				loop_filter_ref_deltas[ AV1RefFrames.GOLDEN_FRAME ]= -1;
				loop_filter_ref_deltas[ AV1RefFrames.ALTREF_FRAME ]= -1;
				loop_filter_ref_deltas[ AV1RefFrames.ALTREF2_FRAME ]= -1;

				for ( i = 0; i < 2; i++ )
				{
					loop_filter_mode_deltas[ i ]= 0;
				}
return;
			}
			stream.WriteFixed(6, this.loop_filter_level[ 0 ], "loop_filter_level"); 
			stream.WriteFixed(6, this.loop_filter_level[ 1 ], "loop_filter_level"); 

			if ( NumPlanes > 1 )
			{

				if ( loop_filter_level[ 0 ] != 0 || loop_filter_level[ 1 ] != 0 )
				{
					stream.WriteFixed(6, this.loop_filter_level[ 2 ], "loop_filter_level"); 
					stream.WriteFixed(6, this.loop_filter_level[ 3 ], "loop_filter_level"); 
				}
			}
			stream.WriteFixed(3, this.loop_filter_sharpness, "loop_filter_sharpness"); 
			stream.WriteFixed(1, this.loop_filter_delta_enabled, "loop_filter_delta_enabled"); 

			if ( loop_filter_delta_enabled == 1 )
			{
				stream.WriteFixed(1, this.loop_filter_delta_update, "loop_filter_delta_update"); 

				if ( loop_filter_delta_update == 1 )
				{

					for ( i = 0; i < TOTAL_REFS_PER_FRAME; i++ )
					{
						stream.WriteFixed(1, this.update_ref_delta[ i ], "update_ref_delta"); 

						if ( update_ref_delta[i] == 1 )
						{
							stream.WriteSignedIntVar(1+6, this.loop_filter_ref_deltas[ i ], "loop_filter_ref_deltas"); 
						}
					}

					for ( i = 0; i < 2; i++ )
					{
						stream.WriteFixed(1, this.update_mode_delta[ i ], "update_mode_delta"); 

						if ( update_mode_delta[i] == 1 )
						{
							stream.WriteSignedIntVar(1+6, this.loop_filter_mode_deltas[ i ], "loop_filter_mode_deltas"); 
						}
					}
				}
			}
         }

    /*


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
    */
		private uint tx_mode_select;
		public uint TxModeSelect { get { return tx_mode_select; } set { tx_mode_select = value; } }

         public void ReadReadTxMode()
         {


			if ( CodedLossless == 1 )
			{
				TxMode= ONLY_4X4;
			}
			else 
			{
				stream.ReadFixed(1, out this.tx_mode_select, "tx_mode_select"); 

				if ( tx_mode_select != 0 )
				{
					TxMode= TX_MODE_SELECT;
				}
				else 
				{
					TxMode= TX_MODE_LARGEST;
				}
			}
         }

         public void WriteReadTxMode()
         {


			if ( CodedLossless == 1 )
			{
				TxMode= ONLY_4X4;
			}
			else 
			{
				stream.WriteFixed(1, this.tx_mode_select, "tx_mode_select"); 

				if ( tx_mode_select != 0 )
				{
					TxMode= TX_MODE_SELECT;
				}
				else 
				{
					TxMode= TX_MODE_LARGEST;
				}
			}
         }

    /*


 frame_reference_mode() { 
 if ( FrameIsIntra ) {
 reference_select = 0
 } else {
 reference_select f(1)
 }
 }
    */
		private uint reference_select;
		public uint ReferenceSelect { get { return reference_select; } set { reference_select = value; } }

         public void ReadFrameReferenceMode()
         {


			if ( FrameIsIntra != 0 )
			{
				reference_select= 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.reference_select, "reference_select"); 
			}
         }

         public void WriteFrameReferenceMode()
         {


			if ( FrameIsIntra != 0 )
			{
				reference_select= 0;
			}
			else 
			{
				stream.WriteFixed(1, this.reference_select, "reference_select"); 
			}
         }

    /*


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
    */
		private uint skip_mode_present;
		public uint SkipModePresent { get { return skip_mode_present; } set { skip_mode_present = value; } }

			uint i = 0;
         public void ReadSkipModeParams()
         {


			if ( FrameIsIntra != 0 || reference_select== 0 || enable_order_hint== 0 )
			{
				skipModeAllowed= 0;
			}
			else 
			{
				forwardIdx= -1;
				backwardIdx= -1;

				for ( i = 0; i < REFS_PER_FRAME; i++ )
				{
					refHint= RefOrderHint[ ref_frame_idx[ i ] ];

					if ( get_relative_dist( refHint, OrderHint ) < 0 )
					{

						if ( forwardIdx < 0 ||
 get_relative_dist( refHint, forwardHint) > 0 )
						{
							forwardIdx= i;
							forwardHint= refHint;
						}
					}
					else if ( get_relative_dist( refHint, OrderHint) > 0 )
					{

						if ( backwardIdx < 0 ||
 get_relative_dist( refHint, backwardHint) < 0 )
						{
							backwardIdx= i;
							backwardHint= refHint;
						}
					}
				}

				if ( forwardIdx < 0 )
				{
					skipModeAllowed= 0;
				}
				else if ( backwardIdx >= 0 )
				{
					skipModeAllowed= 1;
					SkipModeFrame[ 0 ]= AV1RefFrames.LAST_FRAME + Math.Min(forwardIdx, backwardIdx);
					SkipModeFrame[ 1 ]= AV1RefFrames.LAST_FRAME + Math.Max(forwardIdx, backwardIdx);
				}
				else 
				{
					secondForwardIdx= -1;

					for ( i = 0; i < REFS_PER_FRAME; i++ )
					{
						refHint= RefOrderHint[ ref_frame_idx[ i ] ];

						if ( get_relative_dist( refHint, forwardHint ) < 0 )
						{

							if ( secondForwardIdx < 0 ||
 get_relative_dist( refHint, secondForwardHint ) > 0 )
							{
								secondForwardIdx= i;
								secondForwardHint= refHint;
							}
						}
					}

					if ( secondForwardIdx < 0 )
					{
						skipModeAllowed= 0;
					}
					else 
					{
						skipModeAllowed= 1;
						SkipModeFrame[ 0 ]= AV1RefFrames.LAST_FRAME + Math.Min(forwardIdx, secondForwardIdx);
						SkipModeFrame[ 1 ]= AV1RefFrames.LAST_FRAME + Math.Max(forwardIdx, secondForwardIdx);
					}
				}
			}

			if ( skipModeAllowed != 0 )
			{
				stream.ReadFixed(1, out this.skip_mode_present, "skip_mode_present"); 
			}
			else 
			{
				skip_mode_present= 0;
			}
         }

         public void WriteSkipModeParams()
         {


			if ( FrameIsIntra != 0 || reference_select== 0 || enable_order_hint== 0 )
			{
				skipModeAllowed= 0;
			}
			else 
			{
				forwardIdx= -1;
				backwardIdx= -1;

				for ( i = 0; i < REFS_PER_FRAME; i++ )
				{
					refHint= RefOrderHint[ ref_frame_idx[ i ] ];

					if ( get_relative_dist( refHint, OrderHint ) < 0 )
					{

						if ( forwardIdx < 0 ||
 get_relative_dist( refHint, forwardHint) > 0 )
						{
							forwardIdx= i;
							forwardHint= refHint;
						}
					}
					else if ( get_relative_dist( refHint, OrderHint) > 0 )
					{

						if ( backwardIdx < 0 ||
 get_relative_dist( refHint, backwardHint) < 0 )
						{
							backwardIdx= i;
							backwardHint= refHint;
						}
					}
				}

				if ( forwardIdx < 0 )
				{
					skipModeAllowed= 0;
				}
				else if ( backwardIdx >= 0 )
				{
					skipModeAllowed= 1;
					SkipModeFrame[ 0 ]= AV1RefFrames.LAST_FRAME + Math.Min(forwardIdx, backwardIdx);
					SkipModeFrame[ 1 ]= AV1RefFrames.LAST_FRAME + Math.Max(forwardIdx, backwardIdx);
				}
				else 
				{
					secondForwardIdx= -1;

					for ( i = 0; i < REFS_PER_FRAME; i++ )
					{
						refHint= RefOrderHint[ ref_frame_idx[ i ] ];

						if ( get_relative_dist( refHint, forwardHint ) < 0 )
						{

							if ( secondForwardIdx < 0 ||
 get_relative_dist( refHint, secondForwardHint ) > 0 )
							{
								secondForwardIdx= i;
								secondForwardHint= refHint;
							}
						}
					}

					if ( secondForwardIdx < 0 )
					{
						skipModeAllowed= 0;
					}
					else 
					{
						skipModeAllowed= 1;
						SkipModeFrame[ 0 ]= AV1RefFrames.LAST_FRAME + Math.Min(forwardIdx, secondForwardIdx);
						SkipModeFrame[ 1 ]= AV1RefFrames.LAST_FRAME + Math.Max(forwardIdx, secondForwardIdx);
					}
				}
			}

			if ( skipModeAllowed != 0 )
			{
				stream.WriteFixed(1, this.skip_mode_present, "skip_mode_present"); 
			}
			else 
			{
				skip_mode_present= 0;
			}
         }

    /*


 global_motion_params() { 
 for ( refc = LAST_FRAME; refc <= ALTREF_FRAME; refc++ ) {
 GmType[ refc ] = IDENTITY
 for ( i = 0; i < 6; i++ ) {
 gm_params[ refc ][ i ] = ( ( i % 3 == 2 ) ? 1 << WARPEDMODEL_PREC_BITS : 0 )
 }
 }
 if ( FrameIsIntra )
 return
 for ( refc = LAST_FRAME; refc <= ALTREF_FRAME; refc++ ) {
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
 GmType[refc] = type
 if ( type >= ROTZOOM ) {
 read_global_param(type, refc, 2)
 read_global_param(type, refc, 3)
 if ( type == AFFINE ) {
 read_global_param(type, refc, 4)
 read_global_param(type, refc, 5)
 } else {
 gm_params[refc][4] = -gm_params[refc][3]
 gm_params[refc][5] = gm_params[refc][2]
 }
 }
 if ( type >= TRANSLATION ) {
 read_global_param(type, refc, 0)
 read_global_param(type, refc, 1)
 }
 }
 }
    */
		private uint[] is_global;
		public uint[] IsGlobal { get { return is_global; } set { is_global = value; } }
		private uint[] is_rot_zoom;
		public uint[] IsRotZoom { get { return is_rot_zoom; } set { is_rot_zoom = value; } }
		private uint[] is_translation;
		public uint[] IsTranslation { get { return is_translation; } set { is_translation = value; } }

			uint refc = 0;
			uint i = 0;
         public void ReadGlobalMotionParams()
         {


			for ( refc = AV1RefFrames.LAST_FRAME; refc <= AV1RefFrames.ALTREF_FRAME; refc++ )
			{
				GmType[ refc ]= IDENTITY;

				for ( i = 0; i < 6; i++ )
				{
					gm_params[ refc ][ i ]= ( ( i % 3 == 2 ) ? 1 << WARPEDMODEL_PREC_BITS : 0 );
				}
			}

			if ( FrameIsIntra != 0 )
			{
return;
			}

			this.is_global = new uint[ AV1RefFrames.ALTREF_FRAME];
			this.is_rot_zoom = new uint[ AV1RefFrames.ALTREF_FRAME];
			this.is_translation = new uint[ AV1RefFrames.ALTREF_FRAME];
			for ( refc = AV1RefFrames.LAST_FRAME; refc <= AV1RefFrames.ALTREF_FRAME; refc++ )
			{
				stream.ReadFixed(1, out this.is_global[ refc ], "is_global"); 

				if ( is_global[c] != 0 )
				{
					stream.ReadFixed(1, out this.is_rot_zoom[ refc ], "is_rot_zoom"); 

					if ( is_rot_zoom[c] != 0 )
					{
						type= ROTZOOM;
					}
					else 
					{
						stream.ReadFixed(1, out this.is_translation[ refc ], "is_translation"); 
						type= is_translation[c] ? TRANSLATION : AFFINE;
					}
				}
				else 
				{
					type= IDENTITY;
				}
				GmType[refc]= type;

				if ( type >= ROTZOOM )
				{
					ReadReadGlobalParam(type, refc, 2); 
					ReadReadGlobalParam(type, refc, 3); 

					if ( type == AFFINE )
					{
						ReadReadGlobalParam(type, refc, 4); 
						ReadReadGlobalParam(type, refc, 5); 
					}
					else 
					{
						gm_params[refc][4]= -gm_params[refc][3];
						gm_params[refc][5]= gm_params[refc][2];
					}
				}

				if ( type >= TRANSLATION )
				{
					ReadReadGlobalParam(type, refc, 0); 
					ReadReadGlobalParam(type, refc, 1); 
				}
			}
         }

         public void WriteGlobalMotionParams()
         {


			for ( refc = AV1RefFrames.LAST_FRAME; refc <= AV1RefFrames.ALTREF_FRAME; refc++ )
			{
				GmType[ refc ]= IDENTITY;

				for ( i = 0; i < 6; i++ )
				{
					gm_params[ refc ][ i ]= ( ( i % 3 == 2 ) ? 1 << WARPEDMODEL_PREC_BITS : 0 );
				}
			}

			if ( FrameIsIntra != 0 )
			{
return;
			}

			for ( refc = AV1RefFrames.LAST_FRAME; refc <= AV1RefFrames.ALTREF_FRAME; refc++ )
			{
				stream.WriteFixed(1, this.is_global[ refc ], "is_global"); 

				if ( is_global[c] != 0 )
				{
					stream.WriteFixed(1, this.is_rot_zoom[ refc ], "is_rot_zoom"); 

					if ( is_rot_zoom[c] != 0 )
					{
						type= ROTZOOM;
					}
					else 
					{
						stream.WriteFixed(1, this.is_translation[ refc ], "is_translation"); 
						type= is_translation[c] ? TRANSLATION : AFFINE;
					}
				}
				else 
				{
					type= IDENTITY;
				}
				GmType[refc]= type;

				if ( type >= ROTZOOM )
				{
					WriteReadGlobalParam(type, refc, 2); 
					WriteReadGlobalParam(type, refc, 3); 

					if ( type == AFFINE )
					{
						WriteReadGlobalParam(type, refc, 4); 
						WriteReadGlobalParam(type, refc, 5); 
					}
					else 
					{
						gm_params[refc][4]= -gm_params[refc][3];
						gm_params[refc][5]= gm_params[refc][2];
					}
				}

				if ( type >= TRANSLATION )
				{
					WriteReadGlobalParam(type, refc, 0); 
					WriteReadGlobalParam(type, refc, 1); 
				}
			}
         }

    /*


read_global_param( type, refc, idx ) { 
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
 r = (PrevGmParams[refc][idx] >> precDiff) - sub
 gm_params[refc][idx] = (decode_signed_subexp_with_ref( -mx, mx + 1, r ) << precDiff) + round
 }
    */
		private uint type;
		public uint Type { get { return type; } set { type = value; } }
		private uint refc;
		public uint Refc { get { return refc; } set { refc = value; } }
		private uint idx;
		public uint Idx { get { return idx; } set { idx = value; } }

         public void ReadReadGlobalParam(uint type, uint refc, uint idx)
         {

			uint absBits= GM_ABS_ALPHA_BITS;
			uint precBits= GM_ALPHA_PREC_BITS;

			if ( idx < 2 )
			{

				if ( type == TRANSLATION )
				{
					absBits= GM_ABS_TRANS_ONLY_BITS - !allow_high_precision_mv;
					precBits= GM_TRANS_ONLY_PREC_BITS - !allow_high_precision_mv;
				}
				else 
				{
					absBits= GM_ABS_TRANS_BITS;
					precBits= GM_TRANS_PREC_BITS;
				}
			}
			uint precDiff= WARPEDMODEL_PREC_BITS - precBits;
			uint round= (idx % 3) == 2 ? (1 << WARPEDMODEL_PREC_BITS) : 0;
			uint sub= (idx % 3) == 2 ? (1 << precBits) : 0;
			uint mx= (1 << absBits);
			uint r= (PrevGmParams[refc][idx] >> precDiff) - sub;
			uint gm_params[refc][idx]= (decode_signed_subexp_with_ref( -mx, mx + 1, r ) << precDiff) + round;
         }

         public void WriteReadGlobalParam(uint type, uint refc, uint idx)
         {

			uint absBits= GM_ABS_ALPHA_BITS;
			uint precBits= GM_ALPHA_PREC_BITS;

			if ( idx < 2 )
			{

				if ( type == TRANSLATION )
				{
					absBits= GM_ABS_TRANS_ONLY_BITS - !allow_high_precision_mv;
					precBits= GM_TRANS_ONLY_PREC_BITS - !allow_high_precision_mv;
				}
				else 
				{
					absBits= GM_ABS_TRANS_BITS;
					precBits= GM_TRANS_PREC_BITS;
				}
			}
			uint precDiff= WARPEDMODEL_PREC_BITS - precBits;
			uint round= (idx % 3) == 2 ? (1 << WARPEDMODEL_PREC_BITS) : 0;
			uint sub= (idx % 3) == 2 ? (1 << precBits) : 0;
			uint mx= (1 << absBits);
			uint r= (PrevGmParams[refc][idx] >> precDiff) - sub;
			uint gm_params[refc][idx]= (decode_signed_subexp_with_ref( -mx, mx + 1, r ) << precDiff) + round;
         }

    /*


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
    */
		private uint apply_grain;
		public uint ApplyGrain { get { return apply_grain; } set { apply_grain = value; } }
		private uint grain_seed;
		public uint GrainSeed { get { return grain_seed; } set { grain_seed = value; } }
		private uint update_grain;
		public uint UpdateGrain { get { return update_grain; } set { update_grain = value; } }
		private uint film_grain_params_ref_idx;
		public uint FilmGrainParamsRefIdx { get { return film_grain_params_ref_idx; } set { film_grain_params_ref_idx = value; } }
		private uint num_y_points;
		public uint NumyPoints { get { return num_y_points; } set { num_y_points = value; } }
		private uint[] point_y_value;
		public uint[] PointyValue { get { return point_y_value; } set { point_y_value = value; } }
		private uint[] point_y_scaling;
		public uint[] PointyScaling { get { return point_y_scaling; } set { point_y_scaling = value; } }
		private uint chroma_scaling_from_luma;
		public uint ChromaScalingFromLuma { get { return chroma_scaling_from_luma; } set { chroma_scaling_from_luma = value; } }
		private uint num_cb_points;
		public uint NumCbPoints { get { return num_cb_points; } set { num_cb_points = value; } }
		private uint[] point_cb_value;
		public uint[] PointCbValue { get { return point_cb_value; } set { point_cb_value = value; } }
		private uint[] point_cb_scaling;
		public uint[] PointCbScaling { get { return point_cb_scaling; } set { point_cb_scaling = value; } }
		private uint num_cr_points;
		public uint NumCrPoints { get { return num_cr_points; } set { num_cr_points = value; } }
		private uint[] point_cr_value;
		public uint[] PointCrValue { get { return point_cr_value; } set { point_cr_value = value; } }
		private uint[] point_cr_scaling;
		public uint[] PointCrScaling { get { return point_cr_scaling; } set { point_cr_scaling = value; } }
		private uint grain_scaling_minus_8;
		public uint GrainScalingMinus8 { get { return grain_scaling_minus_8; } set { grain_scaling_minus_8 = value; } }
		private uint ar_coeff_lag;
		public uint ArCoeffLag { get { return ar_coeff_lag; } set { ar_coeff_lag = value; } }
		private uint[] ar_coeffs_y_plus_128;
		public uint[] ArCoeffsyPlus128 { get { return ar_coeffs_y_plus_128; } set { ar_coeffs_y_plus_128 = value; } }
		private uint[] ar_coeffs_cb_plus_128;
		public uint[] ArCoeffsCbPlus128 { get { return ar_coeffs_cb_plus_128; } set { ar_coeffs_cb_plus_128 = value; } }
		private uint[] ar_coeffs_cr_plus_128;
		public uint[] ArCoeffsCrPlus128 { get { return ar_coeffs_cr_plus_128; } set { ar_coeffs_cr_plus_128 = value; } }
		private uint ar_coeff_shift_minus_6;
		public uint ArCoeffShiftMinus6 { get { return ar_coeff_shift_minus_6; } set { ar_coeff_shift_minus_6 = value; } }
		private uint grain_scale_shift;
		public uint GrainScaleShift { get { return grain_scale_shift; } set { grain_scale_shift = value; } }
		private uint cb_mult;
		public uint CbMult { get { return cb_mult; } set { cb_mult = value; } }
		private uint cb_luma_mult;
		public uint CbLumaMult { get { return cb_luma_mult; } set { cb_luma_mult = value; } }
		private uint cb_offset;
		public uint CbOffset { get { return cb_offset; } set { cb_offset = value; } }
		private uint cr_mult;
		public uint CrMult { get { return cr_mult; } set { cr_mult = value; } }
		private uint cr_luma_mult;
		public uint CrLumaMult { get { return cr_luma_mult; } set { cr_luma_mult = value; } }
		private uint cr_offset;
		public uint CrOffset { get { return cr_offset; } set { cr_offset = value; } }
		private uint overlap_flag;
		public uint OverlapFlag { get { return overlap_flag; } set { overlap_flag = value; } }
		private uint clip_to_restricted_range;
		public uint ClipToRestrictedRange { get { return clip_to_restricted_range; } set { clip_to_restricted_range = value; } }

			uint i = 0;
         public void ReadFilmGrainParams()
         {


			if ( film_grain_params_present== 0 ||
 (show_frame == 0 && showable_frame== 0) )
			{
				ReadResetGrainParams(); 
return;
			}
			stream.ReadFixed(1, out this.apply_grain, "apply_grain"); 

			if ( apply_grain== 0 )
			{
				ReadResetGrainParams(); 
return;
			}
			stream.ReadFixed(16, out this.grain_seed, "grain_seed"); 

			if ( frame_type == INTER_FRAME )
			{
				stream.ReadFixed(1, out this.update_grain, "update_grain"); 
			}
			else 
			{
				update_grain= 1;
			}

			if ( update_grain== 0 )
			{
				stream.ReadFixed(3, out this.film_grain_params_ref_idx, "film_grain_params_ref_idx"); 
				tempGrainSeed= grain_seed;
				ReadLoadGrainParams( film_grain_params_ref_idx ); 
				grain_seed= tempGrainSeed;
return;
			}
			stream.ReadFixed(4, out this.num_y_points, "num_y_points"); 

			this.point_y_value = new uint[ num_y_points];
			this.point_y_scaling = new uint[ num_y_points];
			for ( i = 0; i < num_y_points; i++ )
			{
				stream.ReadFixed(8, out this.point_y_value[ i ], "point_y_value"); 
				stream.ReadFixed(8, out this.point_y_scaling[ i ], "point_y_scaling"); 
			}

			if ( mono_chrome != 0 )
			{
				chroma_scaling_from_luma= 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.chroma_scaling_from_luma, "chroma_scaling_from_luma"); 
			}

			if ( mono_chrome != 0 || chroma_scaling_from_luma != 0 ||
 ( subsampling_x == 1 && subsampling_y == 1 &&
 num_y_points == 0 )
 )
			{
				num_cb_points= 0;
				num_cr_points= 0;
			}
			else 
			{
				stream.ReadFixed(4, out this.num_cb_points, "num_cb_points"); 

				this.point_cb_value = new uint[ num_cb_points];
				this.point_cb_scaling = new uint[ num_cb_points];
				for ( i = 0; i < num_cb_points; i++ )
				{
					stream.ReadFixed(8, out this.point_cb_value[ i ], "point_cb_value"); 
					stream.ReadFixed(8, out this.point_cb_scaling[ i ], "point_cb_scaling"); 
				}
				stream.ReadFixed(4, out this.num_cr_points, "num_cr_points"); 

				this.point_cr_value = new uint[ num_cr_points];
				this.point_cr_scaling = new uint[ num_cr_points];
				for ( i = 0; i < num_cr_points; i++ )
				{
					stream.ReadFixed(8, out this.point_cr_value[ i ], "point_cr_value"); 
					stream.ReadFixed(8, out this.point_cr_scaling[ i ], "point_cr_scaling"); 
				}
			}
			stream.ReadFixed(2, out this.grain_scaling_minus_8, "grain_scaling_minus_8"); 
			stream.ReadFixed(2, out this.ar_coeff_lag, "ar_coeff_lag"); 
			uint numPosLuma= 2 * ar_coeff_lag * ( ar_coeff_lag + 1 );

			if ( num_y_points != 0 )
			{
				numPosChroma= numPosLuma + 1;

				this.ar_coeffs_y_plus_128 = new uint[ numPosLuma];
				for ( i = 0; i < numPosLuma; i++ )
				{
					stream.ReadFixed(8, out this.ar_coeffs_y_plus_128[ i ], "ar_coeffs_y_plus_128"); 
				}
			}
			else 
			{
				numPosChroma= numPosLuma;
			}

			if ( chroma_scaling_from_luma != 0 || num_cb_points != 0 )
			{

				this.ar_coeffs_cb_plus_128 = new uint[ numPosChroma];
				for ( i = 0; i < numPosChroma; i++ )
				{
					stream.ReadFixed(8, out this.ar_coeffs_cb_plus_128[ i ], "ar_coeffs_cb_plus_128"); 
				}
			}

			if ( chroma_scaling_from_luma != 0 || num_cr_points != 0 )
			{

				this.ar_coeffs_cr_plus_128 = new uint[ numPosChroma];
				for ( i = 0; i < numPosChroma; i++ )
				{
					stream.ReadFixed(8, out this.ar_coeffs_cr_plus_128[ i ], "ar_coeffs_cr_plus_128"); 
				}
			}
			stream.ReadFixed(2, out this.ar_coeff_shift_minus_6, "ar_coeff_shift_minus_6"); 
			stream.ReadFixed(2, out this.grain_scale_shift, "grain_scale_shift"); 

			if ( num_cb_points != 0 )
			{
				stream.ReadFixed(8, out this.cb_mult, "cb_mult"); 
				stream.ReadFixed(8, out this.cb_luma_mult, "cb_luma_mult"); 
				stream.ReadFixed(9, out this.cb_offset, "cb_offset"); 
			}

			if ( num_cr_points != 0 )
			{
				stream.ReadFixed(8, out this.cr_mult, "cr_mult"); 
				stream.ReadFixed(8, out this.cr_luma_mult, "cr_luma_mult"); 
				stream.ReadFixed(9, out this.cr_offset, "cr_offset"); 
			}
			stream.ReadFixed(1, out this.overlap_flag, "overlap_flag"); 
			stream.ReadFixed(1, out this.clip_to_restricted_range, "clip_to_restricted_range"); 
         }

         public void WriteFilmGrainParams()
         {


			if ( film_grain_params_present== 0 ||
 (show_frame == 0 && showable_frame== 0) )
			{
				WriteResetGrainParams(); 
return;
			}
			stream.WriteFixed(1, this.apply_grain, "apply_grain"); 

			if ( apply_grain== 0 )
			{
				WriteResetGrainParams(); 
return;
			}
			stream.WriteFixed(16, this.grain_seed, "grain_seed"); 

			if ( frame_type == INTER_FRAME )
			{
				stream.WriteFixed(1, this.update_grain, "update_grain"); 
			}
			else 
			{
				update_grain= 1;
			}

			if ( update_grain== 0 )
			{
				stream.WriteFixed(3, this.film_grain_params_ref_idx, "film_grain_params_ref_idx"); 
				tempGrainSeed= grain_seed;
				WriteLoadGrainParams( film_grain_params_ref_idx ); 
				grain_seed= tempGrainSeed;
return;
			}
			stream.WriteFixed(4, this.num_y_points, "num_y_points"); 

			for ( i = 0; i < num_y_points; i++ )
			{
				stream.WriteFixed(8, this.point_y_value[ i ], "point_y_value"); 
				stream.WriteFixed(8, this.point_y_scaling[ i ], "point_y_scaling"); 
			}

			if ( mono_chrome != 0 )
			{
				chroma_scaling_from_luma= 0;
			}
			else 
			{
				stream.WriteFixed(1, this.chroma_scaling_from_luma, "chroma_scaling_from_luma"); 
			}

			if ( mono_chrome != 0 || chroma_scaling_from_luma != 0 ||
 ( subsampling_x == 1 && subsampling_y == 1 &&
 num_y_points == 0 )
 )
			{
				num_cb_points= 0;
				num_cr_points= 0;
			}
			else 
			{
				stream.WriteFixed(4, this.num_cb_points, "num_cb_points"); 

				for ( i = 0; i < num_cb_points; i++ )
				{
					stream.WriteFixed(8, this.point_cb_value[ i ], "point_cb_value"); 
					stream.WriteFixed(8, this.point_cb_scaling[ i ], "point_cb_scaling"); 
				}
				stream.WriteFixed(4, this.num_cr_points, "num_cr_points"); 

				for ( i = 0; i < num_cr_points; i++ )
				{
					stream.WriteFixed(8, this.point_cr_value[ i ], "point_cr_value"); 
					stream.WriteFixed(8, this.point_cr_scaling[ i ], "point_cr_scaling"); 
				}
			}
			stream.WriteFixed(2, this.grain_scaling_minus_8, "grain_scaling_minus_8"); 
			stream.WriteFixed(2, this.ar_coeff_lag, "ar_coeff_lag"); 
			uint numPosLuma= 2 * ar_coeff_lag * ( ar_coeff_lag + 1 );

			if ( num_y_points != 0 )
			{
				numPosChroma= numPosLuma + 1;

				for ( i = 0; i < numPosLuma; i++ )
				{
					stream.WriteFixed(8, this.ar_coeffs_y_plus_128[ i ], "ar_coeffs_y_plus_128"); 
				}
			}
			else 
			{
				numPosChroma= numPosLuma;
			}

			if ( chroma_scaling_from_luma != 0 || num_cb_points != 0 )
			{

				for ( i = 0; i < numPosChroma; i++ )
				{
					stream.WriteFixed(8, this.ar_coeffs_cb_plus_128[ i ], "ar_coeffs_cb_plus_128"); 
				}
			}

			if ( chroma_scaling_from_luma != 0 || num_cr_points != 0 )
			{

				for ( i = 0; i < numPosChroma; i++ )
				{
					stream.WriteFixed(8, this.ar_coeffs_cr_plus_128[ i ], "ar_coeffs_cr_plus_128"); 
				}
			}
			stream.WriteFixed(2, this.ar_coeff_shift_minus_6, "ar_coeff_shift_minus_6"); 
			stream.WriteFixed(2, this.grain_scale_shift, "grain_scale_shift"); 

			if ( num_cb_points != 0 )
			{
				stream.WriteFixed(8, this.cb_mult, "cb_mult"); 
				stream.WriteFixed(8, this.cb_luma_mult, "cb_luma_mult"); 
				stream.WriteFixed(9, this.cb_offset, "cb_offset"); 
			}

			if ( num_cr_points != 0 )
			{
				stream.WriteFixed(8, this.cr_mult, "cr_mult"); 
				stream.WriteFixed(8, this.cr_luma_mult, "cr_luma_mult"); 
				stream.WriteFixed(9, this.cr_offset, "cr_offset"); 
			}
			stream.WriteFixed(1, this.overlap_flag, "overlap_flag"); 
			stream.WriteFixed(1, this.clip_to_restricted_range, "clip_to_restricted_range"); 
         }

    /*


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
    */
		private uint use_superres;
		public uint UseSuperres { get { return use_superres; } set { use_superres = value; } }
		private uint coded_denom;
		public uint CodedDenom { get { return coded_denom; } set { coded_denom = value; } }

         public void ReadSuperresParams()
         {


			if ( enable_superres != 0 )
			{
				stream.ReadFixed(1, out this.use_superres, "use_superres"); 
			}
			else 
			{
				use_superres= 0;
			}

			if ( use_superres != 0 )
			{
				stream.ReadVariable(SUPERRES_DENOM_BITS, out this.coded_denom, "coded_denom"); 
				SuperresDenom= coded_denom + SUPERRES_DENOM_MIN;
			}
			else 
			{
				SuperresDenom= SUPERRES_NUM;
			}
			uint UpscaledWidth= FrameWidth;
			uint FrameWidth= (UpscaledWidth * SUPERRES_NUM + (SuperresDenom / 2)) / SuperresDenom;
         }

         public void WriteSuperresParams()
         {


			if ( enable_superres != 0 )
			{
				stream.WriteFixed(1, this.use_superres, "use_superres"); 
			}
			else 
			{
				use_superres= 0;
			}

			if ( use_superres != 0 )
			{
				stream.WriteVariable(SUPERRES_DENOM_BITS, this.coded_denom, "coded_denom"); 
				SuperresDenom= coded_denom + SUPERRES_DENOM_MIN;
			}
			else 
			{
				SuperresDenom= SUPERRES_NUM;
			}
			uint UpscaledWidth= FrameWidth;
			uint FrameWidth= (UpscaledWidth * SUPERRES_NUM + (SuperresDenom / 2)) / SuperresDenom;
         }

    /*


 compute_image_size() { 
 MiCols = 2 * ( ( FrameWidth + 7 ) >> 3 )
 MiRows = 2 * ( ( FrameHeight + 7 ) >> 3 )
 }
    */

         public void ReadComputeImageSize()
         {

			uint MiCols= 2 * ( ( FrameWidth + 7 ) >> 3 );
			uint MiRows= 2 * ( ( FrameHeight + 7 ) >> 3 );
         }

         public void WriteComputeImageSize()
         {

			uint MiCols= 2 * ( ( FrameWidth + 7 ) >> 3 );
			uint MiRows= 2 * ( ( FrameHeight + 7 ) >> 3 );
         }

    /*


 decode_signed_subexp_with_ref( low, high, r ) { 
 x = decode_unsigned_subexp_with_ref(high - low, r - low)
 return x + low
}
    */
		private uint low;
		public uint Low { get { return low; } set { low = value; } }
		private uint high;
		public uint High { get { return high; } set { high = value; } }
		private uint r;
		public uint r { get { return r; } set { r = value; } }

         public void ReadDecodeSignedSubexpWithRef(uint low, uint high, uint r)
         {

			uint x= decode_unsigned_subexp_with_ref(high - low, r - low);
return x + low;
         }

         public void WriteDecodeSignedSubexpWithRef(uint low, uint high, uint r)
         {

			uint x= decode_unsigned_subexp_with_ref(high - low, r - low);
return x + low;
         }

    /*


decode_unsigned_subexp_with_ref( mx, r ) { 
 v = decode_subexp( mx )
 if ( (r << 1) <= mx ) {
 return inverse_recenter(r, v)
 } else {
 return mx - 1 - inverse_recenter(mx - 1 - r, v)
 }
 }
    */
		private uint mx;
		public uint Mx { get { return mx; } set { mx = value; } }
		private uint r;
		public uint r { get { return r; } set { r = value; } }

         public void ReadDecodeUnsignedSubexpWithRef(uint mx, uint r)
         {

			uint v= decode_subexp( mx );

			if ( (r << (int) 1) <= mx )
			{
return inverse_recenter(r, v);
			}
			else 
			{
return mx - 1 - inverse_recenter(mx - 1 - r, v);
			}
         }

         public void WriteDecodeUnsignedSubexpWithRef(uint mx, uint r)
         {

			uint v= decode_subexp( mx );

			if ( (r << (int) 1) <= mx )
			{
return inverse_recenter(r, v);
			}
			else 
			{
return mx - 1 - inverse_recenter(mx - 1 - r, v);
			}
         }

    /*


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
    */
		private uint numSyms;
		public uint NumSyms { get { return numSyms; } set { numSyms = value; } }
		private Dictionary<int, uint> subexp_final_bits = new Dictionary<int, uint>();
		public Dictionary<int, uint> SubexpFinalBits { get { return subexp_final_bits; } set { subexp_final_bits = value; } }
		private Dictionary<int, uint> subexp_more_bits = new Dictionary<int, uint>();
		public Dictionary<int, uint> SubexpMoreBits { get { return subexp_more_bits; } set { subexp_more_bits = value; } }
		private Dictionary<int, uint> subexp_bits = new Dictionary<int, uint>();
		public Dictionary<int, uint> SubexpBits { get { return subexp_bits; } set { subexp_bits = value; } }

			int whileIndex = -1;
         public void ReadDecodeSubexp(uint numSyms)
         {

			uint i= 0;
			uint mk= 0;
			uint k= 3;

			while ( 1 != 0 )
			{
				whileIndex++;

				b2= i ? k + i - 1 : k;
				a= 1 << b2;

				if ( numSyms <= mk + 3 * a )
				{
					stream.ReadUnsignedInt(numSyms - mk, out this.subexp_final_bits, "subexp_final_bits"); 
return subexp_final_bits + mk;
				}
				else 
				{
					stream.ReadFixed(1, out this.subexp_more_bits, "subexp_more_bits"); 

					if ( subexp_more_bits[whileIndex] != 0 )
					{
						i++;
						mk+= a;
					}
					else 
					{
						stream.ReadVariable(b2, out this.subexp_bits, "subexp_bits"); 
return subexp_bits + mk;
					}
				}
			}
         }

         public void WriteDecodeSubexp(uint numSyms)
         {

			uint i= 0;
			uint mk= 0;
			uint k= 3;

			while ( 1 != 0 )
			{
				whileIndex++;

				b2= i ? k + i - 1 : k;
				a= 1 << b2;

				if ( numSyms <= mk + 3 * a )
				{
					stream.WriteUnsignedInt(numSyms - mk, this.subexp_final_bits, "subexp_final_bits"); 
return subexp_final_bits + mk;
				}
				else 
				{
					stream.WriteFixed(1, this.subexp_more_bits, "subexp_more_bits"); 

					if ( subexp_more_bits[whileIndex] != 0 )
					{
						i++;
						mk+= a;
					}
					else 
					{
						stream.WriteVariable(b2, this.subexp_bits, "subexp_bits"); 
return subexp_bits + mk;
					}
				}
			}
         }

    /*


inverse_recenter( r, v ) { 
 if ( v > 2 * r )
    return v
 else if ( v & 1 )
    return r - ((v + 1) >> 1)
 else
    return r + (v >> 1)
 }
    */
		private uint r;
		public uint r { get { return r; } set { r = value; } }
		private uint v;
		public uint v { get { return v; } set { v = value; } }

         public void ReadInverseRecenter(uint r, uint v)
         {


			if ( v > 2 * r )
			{
return v;
			}
			else if ( v & 1 != 0 )
			{
return r - ((v + 1) >> 1);
			}
			else 
			{
return r + (v >> 1);
			}
         }

         public void WriteInverseRecenter(uint r, uint v)
         {


			if ( v > 2 * r )
			{
return v;
			}
			else if ( v & 1 != 0 )
			{
return r - ((v + 1) >> 1);
			}
			else 
			{
return r + (v >> 1);
			}
         }

    /*


temporal_delimiter_obu() { 
 SeenFrameHeader = 0
}
    */

         public void ReadTemporalDelimiterObu()
         {

			uint SeenFrameHeader= 0;
         }

         public void WriteTemporalDelimiterObu()
         {

			uint SeenFrameHeader= 0;
         }

    /*


padding_obu() { 
 for ( i = 0; i < obu_padding_length; i++ )
 obu_padding_byte f(8)
}
    */
		private uint[] obu_padding_byte;
		public uint[] ObuPaddingByte { get { return obu_padding_byte; } set { obu_padding_byte = value; } }

			uint i = 0;
         public void ReadPaddingObu()
         {


			this.obu_padding_byte = new uint[ obu_padding_length];
			for ( i = 0; i < obu_padding_length; i++ )
			{
				stream.ReadFixed(8, out this.obu_padding_byte[ i ], "obu_padding_byte"); 
			}
         }

         public void WritePaddingObu()
         {


			for ( i = 0; i < obu_padding_length; i++ )
			{
				stream.WriteFixed(8, this.obu_padding_byte[ i ], "obu_padding_byte"); 
			}
         }

    /*


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
    */
		private uint metadata_type;
		public uint MetadataType { get { return metadata_type; } set { metadata_type = value; } }

         public void ReadMetadataObu()
         {

			stream.ReadLeb128( out this.metadata_type, "metadata_type"); 

			if ( metadata_type == METADATA_TYPE_ITUT_T35 )
			{
				ReadMetadataItutT35(); 
			}
			else if ( metadata_type == METADATA_TYPE_HDR_CLL )
			{
				ReadMetadataHdrCll(); 
			}
			else if ( metadata_type == METADATA_TYPE_HDR_MDCV )
			{
				ReadMetadataHdrMdcv(); 
			}
			else if ( metadata_type == METADATA_TYPE_SCALABILITY )
			{
				ReadMetadataScalability(); 
			}
			else if ( metadata_type == METADATA_TYPE_TIMECODE )
			{
				ReadMetadataTimecode(); 
			}
         }

         public void WriteMetadataObu()
         {

			stream.WriteLeb128( this.metadata_type, "metadata_type"); 

			if ( metadata_type == METADATA_TYPE_ITUT_T35 )
			{
				WriteMetadataItutT35(); 
			}
			else if ( metadata_type == METADATA_TYPE_HDR_CLL )
			{
				WriteMetadataHdrCll(); 
			}
			else if ( metadata_type == METADATA_TYPE_HDR_MDCV )
			{
				WriteMetadataHdrMdcv(); 
			}
			else if ( metadata_type == METADATA_TYPE_SCALABILITY )
			{
				WriteMetadataScalability(); 
			}
			else if ( metadata_type == METADATA_TYPE_TIMECODE )
			{
				WriteMetadataTimecode(); 
			}
         }

    /*



metadata_itut_t35() { 
 itu_t_t35_country_code f(8)
 if ( itu_t_t35_country_code == 0xFF ) {
 itu_t_t35_country_code_extension_byte f(8)
 }
 itu_t_t35_payload_bytes
}
    */
		private uint itu_t_t35_country_code;
		public uint ItutT35CountryCode { get { return itu_t_t35_country_code; } set { itu_t_t35_country_code = value; } }
		private uint itu_t_t35_country_code_extension_byte;
		public uint ItutT35CountryCodeExtensionByte { get { return itu_t_t35_country_code_extension_byte; } set { itu_t_t35_country_code_extension_byte = value; } }

         public void ReadMetadataItutT35()
         {

			stream.ReadFixed(8, out this.itu_t_t35_country_code, "itu_t_t35_country_code"); 

			if ( itu_t_t35_country_code == 0xFF )
			{
				stream.ReadFixed(8, out this.itu_t_t35_country_code_extension_byte, "itu_t_t35_country_code_extension_byte"); 
			}
			ReadItutT35PayloadBytes; 
         }

         public void WriteMetadataItutT35()
         {

			stream.WriteFixed(8, this.itu_t_t35_country_code, "itu_t_t35_country_code"); 

			if ( itu_t_t35_country_code == 0xFF )
			{
				stream.WriteFixed(8, this.itu_t_t35_country_code_extension_byte, "itu_t_t35_country_code_extension_byte"); 
			}
			WriteItutT35PayloadBytes; 
         }

    /*



metadata_hdr_cll() { 
 max_cll f(16)
 max_fall f(16)
}
    */
		private uint max_cll;
		public uint MaxCll { get { return max_cll; } set { max_cll = value; } }
		private uint max_fall;
		public uint MaxFall { get { return max_fall; } set { max_fall = value; } }

         public void ReadMetadataHdrCll()
         {

			stream.ReadFixed(16, out this.max_cll, "max_cll"); 
			stream.ReadFixed(16, out this.max_fall, "max_fall"); 
         }

         public void WriteMetadataHdrCll()
         {

			stream.WriteFixed(16, this.max_cll, "max_cll"); 
			stream.WriteFixed(16, this.max_fall, "max_fall"); 
         }

    /*



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
    */
		private uint[] primary_chromaticity_x;
		public uint[] PrimaryChromaticityx { get { return primary_chromaticity_x; } set { primary_chromaticity_x = value; } }
		private uint[] primary_chromaticity_y;
		public uint[] PrimaryChromaticityy { get { return primary_chromaticity_y; } set { primary_chromaticity_y = value; } }
		private uint white_point_chromaticity_x;
		public uint WhitePointChromaticityx { get { return white_point_chromaticity_x; } set { white_point_chromaticity_x = value; } }
		private uint white_point_chromaticity_y;
		public uint WhitePointChromaticityy { get { return white_point_chromaticity_y; } set { white_point_chromaticity_y = value; } }
		private uint luminance_max;
		public uint LuminanceMax { get { return luminance_max; } set { luminance_max = value; } }
		private uint luminance_min;
		public uint LuminanceMin { get { return luminance_min; } set { luminance_min = value; } }

			uint i = 0;
         public void ReadMetadataHdrMdcv()
         {


			this.primary_chromaticity_x = new uint[ 3];
			this.primary_chromaticity_y = new uint[ 3];
			for ( i = 0; i < 3; i++ )
			{
				stream.ReadFixed(16, out this.primary_chromaticity_x[ i ], "primary_chromaticity_x"); 
				stream.ReadFixed(16, out this.primary_chromaticity_y[ i ], "primary_chromaticity_y"); 
			}
			stream.ReadFixed(16, out this.white_point_chromaticity_x, "white_point_chromaticity_x"); 
			stream.ReadFixed(16, out this.white_point_chromaticity_y, "white_point_chromaticity_y"); 
			stream.ReadFixed(32, out this.luminance_max, "luminance_max"); 
			stream.ReadFixed(32, out this.luminance_min, "luminance_min"); 
         }

         public void WriteMetadataHdrMdcv()
         {


			for ( i = 0; i < 3; i++ )
			{
				stream.WriteFixed(16, this.primary_chromaticity_x[ i ], "primary_chromaticity_x"); 
				stream.WriteFixed(16, this.primary_chromaticity_y[ i ], "primary_chromaticity_y"); 
			}
			stream.WriteFixed(16, this.white_point_chromaticity_x, "white_point_chromaticity_x"); 
			stream.WriteFixed(16, this.white_point_chromaticity_y, "white_point_chromaticity_y"); 
			stream.WriteFixed(32, this.luminance_max, "luminance_max"); 
			stream.WriteFixed(32, this.luminance_min, "luminance_min"); 
         }

    /*



metadata_scalability() { 
 scalability_mode_idc f(8)
 if ( scalability_mode_idc == SCALABILITY_SS )
 scalability_structure()
}
    */
		private uint scalability_mode_idc;
		public uint ScalabilityModeIdc { get { return scalability_mode_idc; } set { scalability_mode_idc = value; } }

         public void ReadMetadataScalability()
         {

			stream.ReadFixed(8, out this.scalability_mode_idc, "scalability_mode_idc"); 

			if ( scalability_mode_idc == SCALABILITY_SS )
			{
				ReadScalabilityStructure(); 
			}
         }

         public void WriteMetadataScalability()
         {

			stream.WriteFixed(8, this.scalability_mode_idc, "scalability_mode_idc"); 

			if ( scalability_mode_idc == SCALABILITY_SS )
			{
				WriteScalabilityStructure(); 
			}
         }

    /*



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
    */
		private uint spatial_layers_cnt_minus_1;
		public uint SpatialLayersCntMinus1 { get { return spatial_layers_cnt_minus_1; } set { spatial_layers_cnt_minus_1 = value; } }
		private uint spatial_layer_dimensions_present_flag;
		public uint SpatialLayerDimensionsPresentFlag { get { return spatial_layer_dimensions_present_flag; } set { spatial_layer_dimensions_present_flag = value; } }
		private uint spatial_layer_description_present_flag;
		public uint SpatialLayerDescriptionPresentFlag { get { return spatial_layer_description_present_flag; } set { spatial_layer_description_present_flag = value; } }
		private uint temporal_group_description_present_flag;
		public uint TemporalGroupDescriptionPresentFlag { get { return temporal_group_description_present_flag; } set { temporal_group_description_present_flag = value; } }
		private uint scalability_structure_reserved_3bits;
		public uint ScalabilityStructureReserved3bits { get { return scalability_structure_reserved_3bits; } set { scalability_structure_reserved_3bits = value; } }
		private uint[] spatial_layer_max_width;
		public uint[] SpatialLayerMaxWidth { get { return spatial_layer_max_width; } set { spatial_layer_max_width = value; } }
		private uint[] spatial_layer_max_height;
		public uint[] SpatialLayerMaxHeight { get { return spatial_layer_max_height; } set { spatial_layer_max_height = value; } }
		private uint[] spatial_layer_ref_id;
		public uint[] SpatialLayerRefId { get { return spatial_layer_ref_id; } set { spatial_layer_ref_id = value; } }
		private uint temporal_group_size;
		public uint TemporalGroupSize { get { return temporal_group_size; } set { temporal_group_size = value; } }
		private uint[] temporal_group_temporal_id;
		public uint[] TemporalGroupTemporalId { get { return temporal_group_temporal_id; } set { temporal_group_temporal_id = value; } }
		private uint[] temporal_group_temporal_switching_up_point_flag;
		public uint[] TemporalGroupTemporalSwitchingUpPointFlag { get { return temporal_group_temporal_switching_up_point_flag; } set { temporal_group_temporal_switching_up_point_flag = value; } }
		private uint[] temporal_group_spatial_switching_up_point_flag;
		public uint[] TemporalGroupSpatialSwitchingUpPointFlag { get { return temporal_group_spatial_switching_up_point_flag; } set { temporal_group_spatial_switching_up_point_flag = value; } }
		private uint[] temporal_group_ref_cnt;
		public uint[] TemporalGroupRefCnt { get { return temporal_group_ref_cnt; } set { temporal_group_ref_cnt = value; } }
		private uint[][] temporal_group_ref_pic_diff;
		public uint[][] TemporalGroupRefPicDiff { get { return temporal_group_ref_pic_diff; } set { temporal_group_ref_pic_diff = value; } }

			uint i = 0;
			uint j = 0;
         public void ReadScalabilityStructure()
         {

			stream.ReadFixed(2, out this.spatial_layers_cnt_minus_1, "spatial_layers_cnt_minus_1"); 
			stream.ReadFixed(1, out this.spatial_layer_dimensions_present_flag, "spatial_layer_dimensions_present_flag"); 
			stream.ReadFixed(1, out this.spatial_layer_description_present_flag, "spatial_layer_description_present_flag"); 
			stream.ReadFixed(1, out this.temporal_group_description_present_flag, "temporal_group_description_present_flag"); 
			stream.ReadFixed(3, out this.scalability_structure_reserved_3bits, "scalability_structure_reserved_3bits"); 

			if ( spatial_layer_dimensions_present_flag != 0 )
			{

				this.spatial_layer_max_width = new uint[ spatial_layers_cnt_minus_1 ];
				this.spatial_layer_max_height = new uint[ spatial_layers_cnt_minus_1 ];
				for ( i = 0; i <= spatial_layers_cnt_minus_1 ; i++ )
				{
					stream.ReadFixed(16, out this.spatial_layer_max_width[ i ], "spatial_layer_max_width"); 
					stream.ReadFixed(16, out this.spatial_layer_max_height[ i ], "spatial_layer_max_height"); 
				}
			}

			if ( spatial_layer_description_present_flag != 0 )
			{

				this.spatial_layer_ref_id = new uint[ spatial_layers_cnt_minus_1];
				for ( i = 0; i <= spatial_layers_cnt_minus_1; i++ )
				{
					stream.ReadFixed(8, out this.spatial_layer_ref_id[ i ], "spatial_layer_ref_id"); 
				}
			}

			if ( temporal_group_description_present_flag != 0 )
			{
				stream.ReadFixed(8, out this.temporal_group_size, "temporal_group_size"); 

				this.temporal_group_temporal_id = new uint[ temporal_group_size];
				this.temporal_group_temporal_switching_up_point_flag = new uint[ temporal_group_size];
				this.temporal_group_spatial_switching_up_point_flag = new uint[ temporal_group_size];
				this.temporal_group_ref_cnt = new uint[ temporal_group_size];
				this.temporal_group_ref_pic_diff = new uint[ temporal_group_size][];
				for ( i = 0; i < temporal_group_size; i++ )
				{
					stream.ReadFixed(3, out this.temporal_group_temporal_id[ i ], "temporal_group_temporal_id"); 
					stream.ReadFixed(1, out this.temporal_group_temporal_switching_up_point_flag[ i ], "temporal_group_temporal_switching_up_point_flag"); 
					stream.ReadFixed(1, out this.temporal_group_spatial_switching_up_point_flag[ i ], "temporal_group_spatial_switching_up_point_flag"); 
					stream.ReadFixed(3, out this.temporal_group_ref_cnt[ i ], "temporal_group_ref_cnt"); 

					this.temporal_group_ref_pic_diff[ i ] = new uint[ temporal_group_ref_cnt[ i ]];
					for ( j = 0; j < temporal_group_ref_cnt[ i ]; j++ )
					{
						stream.ReadFixed(8, out this.temporal_group_ref_pic_diff[ i ][ j ], "temporal_group_ref_pic_diff"); 
					}
				}
			}
         }

         public void WriteScalabilityStructure()
         {

			stream.WriteFixed(2, this.spatial_layers_cnt_minus_1, "spatial_layers_cnt_minus_1"); 
			stream.WriteFixed(1, this.spatial_layer_dimensions_present_flag, "spatial_layer_dimensions_present_flag"); 
			stream.WriteFixed(1, this.spatial_layer_description_present_flag, "spatial_layer_description_present_flag"); 
			stream.WriteFixed(1, this.temporal_group_description_present_flag, "temporal_group_description_present_flag"); 
			stream.WriteFixed(3, this.scalability_structure_reserved_3bits, "scalability_structure_reserved_3bits"); 

			if ( spatial_layer_dimensions_present_flag != 0 )
			{

				for ( i = 0; i <= spatial_layers_cnt_minus_1 ; i++ )
				{
					stream.WriteFixed(16, this.spatial_layer_max_width[ i ], "spatial_layer_max_width"); 
					stream.WriteFixed(16, this.spatial_layer_max_height[ i ], "spatial_layer_max_height"); 
				}
			}

			if ( spatial_layer_description_present_flag != 0 )
			{

				for ( i = 0; i <= spatial_layers_cnt_minus_1; i++ )
				{
					stream.WriteFixed(8, this.spatial_layer_ref_id[ i ], "spatial_layer_ref_id"); 
				}
			}

			if ( temporal_group_description_present_flag != 0 )
			{
				stream.WriteFixed(8, this.temporal_group_size, "temporal_group_size"); 

				for ( i = 0; i < temporal_group_size; i++ )
				{
					stream.WriteFixed(3, this.temporal_group_temporal_id[ i ], "temporal_group_temporal_id"); 
					stream.WriteFixed(1, this.temporal_group_temporal_switching_up_point_flag[ i ], "temporal_group_temporal_switching_up_point_flag"); 
					stream.WriteFixed(1, this.temporal_group_spatial_switching_up_point_flag[ i ], "temporal_group_spatial_switching_up_point_flag"); 
					stream.WriteFixed(3, this.temporal_group_ref_cnt[ i ], "temporal_group_ref_cnt"); 

					for ( j = 0; j < temporal_group_ref_cnt[ i ]; j++ )
					{
						stream.WriteFixed(8, this.temporal_group_ref_pic_diff[ i ][ j ], "temporal_group_ref_pic_diff"); 
					}
				}
			}
         }

    /*



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
    */
		private uint counting_type;
		public uint CountingType { get { return counting_type; } set { counting_type = value; } }
		private uint full_timestamp_flag;
		public uint FullTimestampFlag { get { return full_timestamp_flag; } set { full_timestamp_flag = value; } }
		private uint discontinuity_flag;
		public uint DiscontinuityFlag { get { return discontinuity_flag; } set { discontinuity_flag = value; } }
		private uint cnt_dropped_flag;
		public uint CntDroppedFlag { get { return cnt_dropped_flag; } set { cnt_dropped_flag = value; } }
		private uint n_frames;
		public uint nFrames { get { return n_frames; } set { n_frames = value; } }
		private uint seconds_value;
		public uint SecondsValue { get { return seconds_value; } set { seconds_value = value; } }
		private uint minutes_value;
		public uint MinutesValue { get { return minutes_value; } set { minutes_value = value; } }
		private uint hours_value;
		public uint HoursValue { get { return hours_value; } set { hours_value = value; } }
		private uint seconds_flag;
		public uint SecondsFlag { get { return seconds_flag; } set { seconds_flag = value; } }
		private uint minutes_flag;
		public uint MinutesFlag { get { return minutes_flag; } set { minutes_flag = value; } }
		private uint hours_flag;
		public uint HoursFlag { get { return hours_flag; } set { hours_flag = value; } }
		private uint time_offset_length;
		public uint TimeOffsetLength { get { return time_offset_length; } set { time_offset_length = value; } }
		private uint time_offset_value;
		public uint TimeOffsetValue { get { return time_offset_value; } set { time_offset_value = value; } }

         public void ReadMetadataTimecode()
         {

			stream.ReadFixed(5, out this.counting_type, "counting_type"); 
			stream.ReadFixed(1, out this.full_timestamp_flag, "full_timestamp_flag"); 
			stream.ReadFixed(1, out this.discontinuity_flag, "discontinuity_flag"); 
			stream.ReadFixed(1, out this.cnt_dropped_flag, "cnt_dropped_flag"); 
			stream.ReadFixed(9, out this.n_frames, "n_frames"); 

			if ( full_timestamp_flag != 0 )
			{
				stream.ReadFixed(6, out this.seconds_value, "seconds_value"); 
				stream.ReadFixed(6, out this.minutes_value, "minutes_value"); 
				stream.ReadFixed(5, out this.hours_value, "hours_value"); 
			}
			else 
			{
				stream.ReadFixed(1, out this.seconds_flag, "seconds_flag"); 

				if ( seconds_flag != 0 )
				{
					stream.ReadFixed(6, out this.seconds_value, "seconds_value"); 
					stream.ReadFixed(1, out this.minutes_flag, "minutes_flag"); 

					if ( minutes_flag != 0 )
					{
						stream.ReadFixed(6, out this.minutes_value, "minutes_value"); 
						stream.ReadFixed(1, out this.hours_flag, "hours_flag"); 

						if ( hours_flag != 0 )
						{
							stream.ReadFixed(5, out this.hours_value, "hours_value"); 
						}
					}
				}
			}
			stream.ReadFixed(5, out this.time_offset_length, "time_offset_length"); 

			if ( time_offset_length > 0 )
			{
				stream.ReadVariable(time_offset_length, out this.time_offset_value, "time_offset_value"); 
			}
         }

         public void WriteMetadataTimecode()
         {

			stream.WriteFixed(5, this.counting_type, "counting_type"); 
			stream.WriteFixed(1, this.full_timestamp_flag, "full_timestamp_flag"); 
			stream.WriteFixed(1, this.discontinuity_flag, "discontinuity_flag"); 
			stream.WriteFixed(1, this.cnt_dropped_flag, "cnt_dropped_flag"); 
			stream.WriteFixed(9, this.n_frames, "n_frames"); 

			if ( full_timestamp_flag != 0 )
			{
				stream.WriteFixed(6, this.seconds_value, "seconds_value"); 
				stream.WriteFixed(6, this.minutes_value, "minutes_value"); 
				stream.WriteFixed(5, this.hours_value, "hours_value"); 
			}
			else 
			{
				stream.WriteFixed(1, this.seconds_flag, "seconds_flag"); 

				if ( seconds_flag != 0 )
				{
					stream.WriteFixed(6, this.seconds_value, "seconds_value"); 
					stream.WriteFixed(1, this.minutes_flag, "minutes_flag"); 

					if ( minutes_flag != 0 )
					{
						stream.WriteFixed(6, this.minutes_value, "minutes_value"); 
						stream.WriteFixed(1, this.hours_flag, "hours_flag"); 

						if ( hours_flag != 0 )
						{
							stream.WriteFixed(5, this.hours_value, "hours_value"); 
						}
					}
				}
			}
			stream.WriteFixed(5, this.time_offset_length, "time_offset_length"); 

			if ( time_offset_length > 0 )
			{
				stream.WriteVariable(time_offset_length, this.time_offset_value, "time_offset_value"); 
			}
         }

    /*


 frame_obu( sz ) { 
 startBitPos = get_position()
 frame_header_obu()
 byte_alignment()
 endBitPos = get_position()
 headerBytes = (endBitPos - startBitPos) / 8
 sz -= headerBytes
 tile_group_obu( sz )
 }
    */
		private uint sz;
		public uint Sz { get { return sz; } set { sz = value; } }

         public void ReadFrameObu(uint sz)
         {

			uint startBitPos= stream.GetPosition();
			ReadFrameHeaderObu(); 
			ReadByteAlignment(); 
			uint endBitPos= stream.GetPosition();
			uint headerBytes= (endBitPos - startBitPos) / 8;
			sz-= headerBytes;
			ReadTileGroupObu( sz ); 
         }

         public void WriteFrameObu(uint sz)
         {

			uint startBitPos= stream.GetPosition();
			WriteFrameHeaderObu(); 
			WriteByteAlignment(); 
			uint endBitPos= stream.GetPosition();
			uint headerBytes= (endBitPos - startBitPos) / 8;
			sz-= headerBytes;
			WriteTileGroupObu( sz ); 
         }

    /*


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
 for ( TileNum = tg_start; TileNum <= tg_end; TileNum++ ) {
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
 }
 if ( tg_end == NumTiles - 1 ) {
 if ( !disable_frame_end_update_cdf ) {
 frame_end_update_cdf()
 }
 decode_frame_wrapup()
 SeenFrameHeader = 0
 }
 }
    */
		private uint sz;
		public uint Sz { get { return sz; } set { sz = value; } }
		private uint tile_start_and_end_present_flag;
		public uint TileStartAndEndPresentFlag { get { return tile_start_and_end_present_flag; } set { tile_start_and_end_present_flag = value; } }
		private uint tg_start;
		public uint TgStart { get { return tg_start; } set { tg_start = value; } }
		private uint tg_end;
		public uint TgEnd { get { return tg_end; } set { tg_end = value; } }
		private uint[] tile_size_minus_1;
		public uint[] TileSizeMinus1 { get { return tile_size_minus_1; } set { tile_size_minus_1 = value; } }

			uint TileNum = 0;
         public void ReadTileGroupObu(uint sz)
         {

			uint NumTiles= TileCols * TileRows;
			uint startBitPos= stream.GetPosition();
			tile_start_and_end_present_flag= 0;

			if ( NumTiles > 1 )
			{
				stream.ReadFixed(1, out this.tile_start_and_end_present_flag, "tile_start_and_end_present_flag"); 
			}

			if ( NumTiles == 1 || tile_start_and_end_present_flag== 0 )
			{
				tg_start= 0;
				tg_end= NumTiles - 1;
			}
			else 
			{
				tileBits= TileColsLog2 + TileRowsLog2;
				stream.ReadVariable(tileBits, out this.tg_start, "tg_start"); 
				stream.ReadVariable(tileBits, out this.tg_end, "tg_end"); 
			}
			ReadByteAlignment(); 
			uint endBitPos= stream.GetPosition();
			uint headerBytes= (endBitPos - startBitPos) / 8;
			sz-= headerBytes;

			this.tile_size_minus_1 = new uint[ tg_end];
			for ( TileNum = tg_start; TileNum <= tg_end; TileNum++ )
			{
				tileRow= TileNum / TileCols;
				tileCol= TileNum % TileCols;
				lastTile= TileNum == tg_end ? (uint)1 : (uint)0;

				if ( lastTile != 0 )
				{
					tileSize= sz;
				}
				else 
				{
					stream.ReadLeVar(TileSizeBytes, out this.tile_size_minus_1[ TileNum ], "tile_size_minus_1"); 
					tileSize= tile_size_minus_1[TileNum] + 1;
					sz-= tileSize + TileSizeBytes;
				}
				MiRowStart= MiRowStarts[ tileRow ];
				MiRowEnd= MiRowStarts[ tileRow + 1 ];
				MiColStart= MiColStarts[ tileCol ];
				MiColEnd= MiColStarts[ tileCol + 1 ];
				CurrentQIndex= base_q_idx;
				ReadInitSymbol( tileSize ); 
				ReadDecodeTile(); 
				ReadExitSymbol(); 
			}

			if ( tg_end == NumTiles - 1 )
			{

				if ( disable_frame_end_update_cdf== 0 )
				{
					ReadFrameEndUpdateCdf(); 
				}
				ReadDecodeFrameWrapup(); 
				SeenFrameHeader= 0;
			}
         }

         public void WriteTileGroupObu(uint sz)
         {

			uint NumTiles= TileCols * TileRows;
			uint startBitPos= stream.GetPosition();
			tile_start_and_end_present_flag= 0;

			if ( NumTiles > 1 )
			{
				stream.WriteFixed(1, this.tile_start_and_end_present_flag, "tile_start_and_end_present_flag"); 
			}

			if ( NumTiles == 1 || tile_start_and_end_present_flag== 0 )
			{
				tg_start= 0;
				tg_end= NumTiles - 1;
			}
			else 
			{
				tileBits= TileColsLog2 + TileRowsLog2;
				stream.WriteVariable(tileBits, this.tg_start, "tg_start"); 
				stream.WriteVariable(tileBits, this.tg_end, "tg_end"); 
			}
			WriteByteAlignment(); 
			uint endBitPos= stream.GetPosition();
			uint headerBytes= (endBitPos - startBitPos) / 8;
			sz-= headerBytes;

			for ( TileNum = tg_start; TileNum <= tg_end; TileNum++ )
			{
				tileRow= TileNum / TileCols;
				tileCol= TileNum % TileCols;
				lastTile= TileNum == tg_end ? (uint)1 : (uint)0;

				if ( lastTile != 0 )
				{
					tileSize= sz;
				}
				else 
				{
					stream.WriteLeVar(TileSizeBytes,  this.tile_size_minus_1[ TileNum ], "tile_size_minus_1"); 
					tileSize= tile_size_minus_1[TileNum] + 1;
					sz-= tileSize + TileSizeBytes;
				}
				MiRowStart= MiRowStarts[ tileRow ];
				MiRowEnd= MiRowStarts[ tileRow + 1 ];
				MiColStart= MiColStarts[ tileCol ];
				MiColEnd= MiColStarts[ tileCol + 1 ];
				CurrentQIndex= base_q_idx;
				WriteInitSymbol( tileSize ); 
				WriteDecodeTile(); 
				WriteExitSymbol(); 
			}

			if ( tg_end == NumTiles - 1 )
			{

				if ( disable_frame_end_update_cdf== 0 )
				{
					WriteFrameEndUpdateCdf(); 
				}
				WriteDecodeFrameWrapup(); 
				SeenFrameHeader= 0;
			}
         }

    /*


 tile_list_obu() {
 output_frame_width_in_tiles_minus_1 f(8)
 output_frame_height_in_tiles_minus_1 f(8)
 tile_count_minus_1 f(16)
 for ( tile = 0; tile <= tile_count_minus_1; tile++ )
 tile_list_entry()
 }
    */
		private uint output_frame_width_in_tiles_minus_1;
		public uint OutputFrameWidthInTilesMinus1 { get { return output_frame_width_in_tiles_minus_1; } set { output_frame_width_in_tiles_minus_1 = value; } }
		private uint output_frame_height_in_tiles_minus_1;
		public uint OutputFrameHeightInTilesMinus1 { get { return output_frame_height_in_tiles_minus_1; } set { output_frame_height_in_tiles_minus_1 = value; } }
		private uint tile_count_minus_1;
		public uint TileCountMinus1 { get { return tile_count_minus_1; } set { tile_count_minus_1 = value; } }

			uint tile = 0;
         public void ReadTileListObu()
         {

			stream.ReadFixed(8, out this.output_frame_width_in_tiles_minus_1, "output_frame_width_in_tiles_minus_1"); 
			stream.ReadFixed(8, out this.output_frame_height_in_tiles_minus_1, "output_frame_height_in_tiles_minus_1"); 
			stream.ReadFixed(16, out this.tile_count_minus_1, "tile_count_minus_1"); 

			for ( tile = 0; tile <= tile_count_minus_1; tile++ )
			{
				ReadTileListEntry(); 
			}
         }

         public void WriteTileListObu()
         {

			stream.WriteFixed(8, this.output_frame_width_in_tiles_minus_1, "output_frame_width_in_tiles_minus_1"); 
			stream.WriteFixed(8, this.output_frame_height_in_tiles_minus_1, "output_frame_height_in_tiles_minus_1"); 
			stream.WriteFixed(16, this.tile_count_minus_1, "tile_count_minus_1"); 

			for ( tile = 0; tile <= tile_count_minus_1; tile++ )
			{
				WriteTileListEntry(); 
			}
         }

    /*



tile_list_entry() {
 anchor_frame_idx f(8)
 anchor_tile_row f(8)
 anchor_tile_col f(8)
 tile_data_size_minus_1 f(16)
 N = 8 * (tile_data_size_minus_1 + 1)
 coded_tile_data f(N)
 }
    */
		private uint anchor_frame_idx;
		public uint AnchorFrameIdx { get { return anchor_frame_idx; } set { anchor_frame_idx = value; } }
		private uint anchor_tile_row;
		public uint AnchorTileRow { get { return anchor_tile_row; } set { anchor_tile_row = value; } }
		private uint anchor_tile_col;
		public uint AnchorTileCol { get { return anchor_tile_col; } set { anchor_tile_col = value; } }
		private uint tile_data_size_minus_1;
		public uint TileDataSizeMinus1 { get { return tile_data_size_minus_1; } set { tile_data_size_minus_1 = value; } }
		private uint coded_tile_data;
		public uint CodedTileData { get { return coded_tile_data; } set { coded_tile_data = value; } }

         public void ReadTileListEntry()
         {

			stream.ReadFixed(8, out this.anchor_frame_idx, "anchor_frame_idx"); 
			stream.ReadFixed(8, out this.anchor_tile_row, "anchor_tile_row"); 
			stream.ReadFixed(8, out this.anchor_tile_col, "anchor_tile_col"); 
			stream.ReadFixed(16, out this.tile_data_size_minus_1, "tile_data_size_minus_1"); 
			uint N= 8 * (tile_data_size_minus_1 + 1);
			stream.ReadVariable(N, out this.coded_tile_data, "coded_tile_data"); 
         }

         public void WriteTileListEntry()
         {

			stream.WriteFixed(8, this.anchor_frame_idx, "anchor_frame_idx"); 
			stream.WriteFixed(8, this.anchor_tile_row, "anchor_tile_row"); 
			stream.WriteFixed(8, this.anchor_tile_col, "anchor_tile_col"); 
			stream.WriteFixed(16, this.tile_data_size_minus_1, "tile_data_size_minus_1"); 
			uint N= 8 * (tile_data_size_minus_1 + 1);
			stream.WriteVariable(N, this.coded_tile_data, "coded_tile_data"); 
         }

        }
    }
