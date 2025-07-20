using System;
using System.Collections.Generic;
using System.Numerics;

namespace SharpAV1
{

    public partial class AV1Context : IAomContext
    {

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
		private int sz;
		private int obu_header;
		private int obu_size;
		private int startPosition;
		private int inTemporalLayer;
		private int inSpatialLayer;
		private int drop_obu;
		private int sequence_header_obu;
		private int temporal_delimiter_obu;
		private int frame_header_obu;
		private int tile_group_obu;
		private int metadata_obu;
		private int frame_obu;
		private int tile_list_obu;
		private int padding_obu;
		private int reserved_obu;
		private int currentPosition;
		private int payloadBits;
		private int trailing_bits;

         public void ReadOpenBitstreamUnit(int sz)
         {

			ReadObuHeader(); 

			if ( obu_has_size_field != 0 )
			{
				obu_size_len = (int)stream.ReadLeb128( out this.obu_size, "obu_size"); 
			}
			else 
			{
				obu_size= sz - 1 - obu_extension_flag;
			}
			startPosition= stream.GetPosition();

			if ( obu_type != AV1ObuTypes.OBU_SEQUENCE_HEADER && obu_type != AV1ObuTypes.OBU_TEMPORAL_DELIMITER && OperatingPointIdc != 0 && obu_extension_flag == 1 )
			{
				inTemporalLayer= (OperatingPointIdc >> (int)temporal_id ) & 1;
				inSpatialLayer= (OperatingPointIdc >> (int)( spatial_id + 8 ) ) & 1;

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
			currentPosition= stream.GetPosition();
			payloadBits= currentPosition - startPosition;

			if ( obu_size > 0 && obu_type != AV1ObuTypes.OBU_TILE_GROUP &&
    obu_type != AV1ObuTypes.OBU_TILE_LIST &&
    obu_type != AV1ObuTypes.OBU_FRAME )
			{
				ReadTrailingBits( obu_size * 8 - payloadBits ); 
			}
         }

         public void WriteOpenBitstreamUnit(int sz)
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
			startPosition= stream.GetPosition();

			if ( obu_type != AV1ObuTypes.OBU_SEQUENCE_HEADER && obu_type != AV1ObuTypes.OBU_TEMPORAL_DELIMITER && OperatingPointIdc != 0 && obu_extension_flag == 1 )
			{
				inTemporalLayer= (OperatingPointIdc >> (int)temporal_id ) & 1;
				inSpatialLayer= (OperatingPointIdc >> (int)( spatial_id + 8 ) ) & 1;

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
			currentPosition= stream.GetPosition();
			payloadBits= currentPosition - startPosition;

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
		private int obu_forbidden_bit;
		private int obu_type;
		private int obu_extension_flag;
		private int obu_has_size_field;
		private int obu_reserved_1bit;
		private int obu_extension_header;

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
		private int temporal_id;
		private int spatial_id;
		private int extension_header_reserved_3bits;

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
		private long nbBits;
		private int trailing_one_bit;
		private int trailing_zero_bit;

         public void ReadTrailingBits(long nbBits)
         {

			stream.ReadFixed(1, out this.trailing_one_bit, "trailing_one_bit"); 
			nbBits--;

			while ( nbBits > 0 )
			{
				stream.ReadFixed(1, out this.trailing_zero_bit, "trailing_zero_bit"); 
				nbBits--;
			}
         }

         public void WriteTrailingBits(long nbBits)
         {

			stream.WriteFixed(1, this.trailing_one_bit, "trailing_one_bit"); 
			nbBits--;

			while ( nbBits > 0 )
			{
				stream.WriteFixed(1, this.trailing_zero_bit, "trailing_zero_bit"); 
				nbBits--;
			}
         }

    /*



byte_alignment() { 
 while ( (get_position() & 7) > 0 )
 zero_bit f(1)
}
    */
		private int zero_bit;

         public void ReadByteAlignment()
         {


			while ( (stream.GetPosition() & 7) > 0 )
			{
				stream.ReadFixed(1, out this.zero_bit, "zero_bit"); 
			}
         }

         public void WriteByteAlignment()
         {


			while ( (stream.GetPosition() & 7) > 0 )
			{
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
 operating_point_idc[ i ] f(12)
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
 initial_display_delay_minus_1[ i ] f(4)
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
		private int seq_profile;
		private int still_picture;
		private int reduced_still_picture_header;
		private int timing_info_present_flag;
		private int decoder_model_info_present_flag;
		private int initial_display_delay_present_flag;
		private int operating_points_cnt_minus_1;
		private int[] operating_point_idc= new int[1];
		private int[] seq_level_idx= new int[1];
		private int[] seq_tier= new int[1];
		private int[] decoder_model_present_for_this_op= new int[1];
		private int[] initial_display_delay_present_for_this_op= new int[1];
		private int timing_info;
		private int decoder_model_info;
		private int operating_parameters_info;
		private int[] initial_display_delay_minus_1;
		private int operatingPoint;
		private int OperatingPointIdc;
		private int frame_width_bits_minus_1;
		private int frame_height_bits_minus_1;
		private int n;
		private int max_frame_width_minus_1;
		private int max_frame_height_minus_1;
		private int frame_id_numbers_present_flag;
		private int delta_frame_id_length_minus_2;
		private int additional_frame_id_length_minus_1;
		private int use_128x128_superblock;
		private int enable_filter_intra;
		private int enable_intra_edge_filter;
		private int enable_interintra_compound;
		private int enable_masked_compound;
		private int enable_warped_motion;
		private int enable_dual_filter;
		private int enable_order_hint;
		private int enable_jnt_comp;
		private int enable_ref_frame_mvs;
		private int seq_force_screen_content_tools;
		private int seq_force_integer_mv;
		private int OrderHintBits;
		private int seq_choose_screen_content_tools;
		private int seq_choose_integer_mv;
		private int order_hint_bits_minus_1;
		private int enable_superres;
		private int enable_cdef;
		private int enable_restoration;
		private int color_config;
		private int film_grain_params_present;

			int i = 0;
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
				stream.ReadFixed(5, out this.operating_points_cnt_minus_1, "operating_points_cnt_minus_1"); operating_point_idc = new int[operating_points_cnt_minus_1 + 1];
seq_level_idx = new int[operating_points_cnt_minus_1 + 1];
seq_tier = new int[operating_points_cnt_minus_1 + 1];
decoder_model_present_for_this_op = new int[operating_points_cnt_minus_1 + 1];
initial_display_delay_present_for_this_op = new int[operating_points_cnt_minus_1 + 1];
initial_display_delay_minus_1 = new int[operating_points_cnt_minus_1 + 1];


				for ( i = 0; i <= operating_points_cnt_minus_1; i++ )
				{
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
							stream.ReadFixed(4, out this.initial_display_delay_minus_1[ i ], "initial_display_delay_minus_1"); 
						}
					}
				}
			}
			operatingPoint= ChooseOperatingPoint();
			OperatingPointIdc= operating_point_idc[ operatingPoint ];
			stream.ReadFixed(4, out this.frame_width_bits_minus_1, "frame_width_bits_minus_1"); 
			stream.ReadFixed(4, out this.frame_height_bits_minus_1, "frame_height_bits_minus_1"); 
			n= frame_width_bits_minus_1 + 1;
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
				seq_force_screen_content_tools= AV1Constants.SELECT_SCREEN_CONTENT_TOOLS;
				seq_force_integer_mv= AV1Constants.SELECT_INTEGER_MV;
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
					seq_force_screen_content_tools= AV1Constants.SELECT_SCREEN_CONTENT_TOOLS;
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
						seq_force_integer_mv= AV1Constants.SELECT_INTEGER_MV;
					}
					else 
					{
						stream.ReadFixed(1, out this.seq_force_integer_mv, "seq_force_integer_mv"); 
					}
				}
				else 
				{
					seq_force_integer_mv= AV1Constants.SELECT_INTEGER_MV;
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
				stream.WriteFixed(5, this.operating_points_cnt_minus_1, "operating_points_cnt_minus_1"); operating_point_idc = new int[operating_points_cnt_minus_1 + 1];
seq_level_idx = new int[operating_points_cnt_minus_1 + 1];
seq_tier = new int[operating_points_cnt_minus_1 + 1];
decoder_model_present_for_this_op = new int[operating_points_cnt_minus_1 + 1];
initial_display_delay_present_for_this_op = new int[operating_points_cnt_minus_1 + 1];
initial_display_delay_minus_1 = new int[operating_points_cnt_minus_1 + 1];


				for ( i = 0; i <= operating_points_cnt_minus_1; i++ )
				{
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
							stream.WriteFixed(4, this.initial_display_delay_minus_1[ i ], "initial_display_delay_minus_1"); 
						}
					}
				}
			}
			operatingPoint= ChooseOperatingPoint();
			OperatingPointIdc= operating_point_idc[ operatingPoint ];
			stream.WriteFixed(4, this.frame_width_bits_minus_1, "frame_width_bits_minus_1"); 
			stream.WriteFixed(4, this.frame_height_bits_minus_1, "frame_height_bits_minus_1"); 
			n= frame_width_bits_minus_1 + 1;
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
				seq_force_screen_content_tools= AV1Constants.SELECT_SCREEN_CONTENT_TOOLS;
				seq_force_integer_mv= AV1Constants.SELECT_INTEGER_MV;
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
					seq_force_screen_content_tools= AV1Constants.SELECT_SCREEN_CONTENT_TOOLS;
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
						seq_force_integer_mv= AV1Constants.SELECT_INTEGER_MV;
					}
					else 
					{
						stream.WriteFixed(1, this.seq_force_integer_mv, "seq_force_integer_mv"); 
					}
				}
				else 
				{
					seq_force_integer_mv= AV1Constants.SELECT_INTEGER_MV;
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
 num_ticks_per_picture_minus_1 uvlc()
}
    */
		private int num_units_in_display_tick;
		private int time_scale;
		private int equal_picture_interval;
		private uint num_ticks_per_picture_minus_1;

         public void ReadTimingInfo()
         {

			stream.ReadFixed(32, out this.num_units_in_display_tick, "num_units_in_display_tick"); 
			stream.ReadFixed(32, out this.time_scale, "time_scale"); 
			stream.ReadFixed(1, out this.equal_picture_interval, "equal_picture_interval"); 

			if ( equal_picture_interval != 0 )
			{
				stream.ReadUvlc( out this.num_ticks_per_picture_minus_1, "num_ticks_per_picture_minus_1"); 
			}
         }

         public void WriteTimingInfo()
         {

			stream.WriteFixed(32, this.num_units_in_display_tick, "num_units_in_display_tick"); 
			stream.WriteFixed(32, this.time_scale, "time_scale"); 
			stream.WriteFixed(1, this.equal_picture_interval, "equal_picture_interval"); 

			if ( equal_picture_interval != 0 )
			{
				stream.WriteUvlc( this.num_ticks_per_picture_minus_1, "num_ticks_per_picture_minus_1"); 
			}
         }

    /*


decoder_model_info() { 
 buffer_delay_length_minus_1 f(5)
 num_units_in_decoding_tick f(32)
 buffer_removal_time_length_minus_1 f(5)
 frame_presentation_time_length_minus_1 f(5)
}
    */
		private int buffer_delay_length_minus_1;
		private int num_units_in_decoding_tick;
		private int buffer_removal_time_length_minus_1;
		private int frame_presentation_time_length_minus_1;

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
		private int op;
		private int[] decoder_buffer_delay;
		private int[] encoder_buffer_delay;
		private int[] low_delay_mode_flag;

         public void ReadOperatingParametersInfo(int op)
         {

			n= buffer_delay_length_minus_1 + 1;
			stream.ReadVariable(n, out this.decoder_buffer_delay[ op ], "decoder_buffer_delay"); 
			stream.ReadVariable(n, out this.encoder_buffer_delay[ op ], "encoder_buffer_delay"); 
			stream.ReadFixed(1, out this.low_delay_mode_flag[ op ], "low_delay_mode_flag"); 
         }

         public void WriteOperatingParametersInfo(int op)
         {

			n= buffer_delay_length_minus_1 + 1;
			stream.WriteVariable(n, this.decoder_buffer_delay[ op ], "decoder_buffer_delay"); 
			stream.WriteVariable(n, this.encoder_buffer_delay[ op ], "encoder_buffer_delay"); 
			stream.WriteFixed(1, this.low_delay_mode_flag[ op ], "low_delay_mode_flag"); 
         }

    /*


color_config() { 
 high_bitdepth f(1)
 if ( seq_profile == 2 && high_bitdepth ) {
 twelve_bit f(1)
 BitDepth = twelve_bit != 0 ? 12 : 10
 } else if ( seq_profile <= 2 ) {
 BitDepth = high_bitdepth != 0 ? 10 : 8
 }
 if ( seq_profile == 1 ) {
 mono_chrome = 0
 } else {
 mono_chrome f(1)
 }
 NumPlanes = mono_chrome != 0 ? 1 : 3
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
		private int high_bitdepth;
		private int twelve_bit;
		private int BitDepth;
		private int mono_chrome;
		private int NumPlanes;
		private int color_description_present_flag;
		private int color_primaries;
		private int transfer_characteristics;
		private int matrix_coefficients;
		private int color_range;
		private int subsampling_x;
		private int subsampling_y;
		private int chroma_sample_position;
		private int separate_uv_delta_q;

         public void ReadColorConfig()
         {

			stream.ReadFixed(1, out this.high_bitdepth, "high_bitdepth"); 

			if ( seq_profile == 2 && high_bitdepth != 0 )
			{
				stream.ReadFixed(1, out this.twelve_bit, "twelve_bit"); 
				BitDepth= twelve_bit != 0 ? 12 : 10;
			}
			else if ( seq_profile <= 2 )
			{
				BitDepth= high_bitdepth != 0 ? 10 : 8;
			}

			if ( seq_profile == 1 )
			{
				mono_chrome= 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.mono_chrome, "mono_chrome"); 
			}
			NumPlanes= mono_chrome != 0 ? 1 : 3;
			stream.ReadFixed(1, out this.color_description_present_flag, "color_description_present_flag"); 

			if ( color_description_present_flag != 0 )
			{
				stream.ReadFixed(8, out this.color_primaries, "color_primaries"); 
				stream.ReadFixed(8, out this.transfer_characteristics, "transfer_characteristics"); 
				stream.ReadFixed(8, out this.matrix_coefficients, "matrix_coefficients"); 
			}
			else 
			{
				color_primaries= AV1ColorPrimaries.CP_UNSPECIFIED;
				transfer_characteristics= AV1TransferCharacteristics.TC_UNSPECIFIED;
				matrix_coefficients= AV1MatrixCoefficients.MC_UNSPECIFIED;
			}

			if ( mono_chrome != 0 )
			{
				stream.ReadFixed(1, out this.color_range, "color_range"); 
				subsampling_x= 1;
				subsampling_y= 1;
				chroma_sample_position= AV1ChromaSamplePosition.CSP_UNKNOWN;
				separate_uv_delta_q= 0;
				return;
			}
			else if ( color_primaries == AV1ColorPrimaries.CP_BT_709 &&
 transfer_characteristics == AV1TransferCharacteristics.TC_SRGB &&
 matrix_coefficients == AV1MatrixCoefficients.MC_IDENTITY )
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
				BitDepth= twelve_bit != 0 ? 12 : 10;
			}
			else if ( seq_profile <= 2 )
			{
				BitDepth= high_bitdepth != 0 ? 10 : 8;
			}

			if ( seq_profile == 1 )
			{
				mono_chrome= 0;
			}
			else 
			{
				stream.WriteFixed(1, this.mono_chrome, "mono_chrome"); 
			}
			NumPlanes= mono_chrome != 0 ? 1 : 3;
			stream.WriteFixed(1, this.color_description_present_flag, "color_description_present_flag"); 

			if ( color_description_present_flag != 0 )
			{
				stream.WriteFixed(8, this.color_primaries, "color_primaries"); 
				stream.WriteFixed(8, this.transfer_characteristics, "transfer_characteristics"); 
				stream.WriteFixed(8, this.matrix_coefficients, "matrix_coefficients"); 
			}
			else 
			{
				color_primaries= AV1ColorPrimaries.CP_UNSPECIFIED;
				transfer_characteristics= AV1TransferCharacteristics.TC_UNSPECIFIED;
				matrix_coefficients= AV1MatrixCoefficients.MC_UNSPECIFIED;
			}

			if ( mono_chrome != 0 )
			{
				stream.WriteFixed(1, this.color_range, "color_range"); 
				subsampling_x= 1;
				subsampling_y= 1;
				chroma_sample_position= AV1ChromaSamplePosition.CSP_UNKNOWN;
				separate_uv_delta_q= 0;
				return;
			}
			else if ( color_primaries == AV1ColorPrimaries.CP_BT_709 &&
 transfer_characteristics == AV1TransferCharacteristics.TC_SRGB &&
 matrix_coefficients == AV1MatrixCoefficients.MC_IDENTITY )
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
		private int frame_header_copy;
		private int SeenFrameHeader;
		private int uncompressed_header;
		private int decode_frame_wrapup;
		private int TileNum;

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
       /* if ( decoder_model_info_present_flag && !equal_picture_interval ) {
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
       } *//*
       
       return
    }
    frame_type f(2)
    FrameIsIntra = ((frame_type == INTRA_ONLY_FRAME || frame_type == KEY_FRAME) ? 1 : 0)
    show_frame f(1)
    if ( show_frame && decoder_model_info_present_flag && !equal_picture_interval ) {
       temporal_point_info()
    }
    if ( show_frame ) {
       showable_frame = (frame_type != KEY_FRAME) ? 1 : 0
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
    current_frame_id f(idLen)
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
 order_hint f(OrderHintBits)
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
 if ( frame_type == SWITCH_FRAME || ( frame_type == KEY_FRAME && show_frame ) ) {
    refresh_frame_flags = allFrames
 } else {
    refresh_frame_flags f(8)
 }
 if ( !FrameIsIntra || refresh_frame_flags != allFrames ) {
    if ( error_resilient_mode && enable_order_hint ) {
       for ( i = 0; i < NUM_REF_FRAMES; i++) {
          ref_order_hint[ i ] f(OrderHintBits)
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
 RefFrameSignBias[ refFrame ] = (get_relative_dist( hint, OrderHint) > 0) ? 1 : 0
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
 LosslessArray[ segmentId ] = (qindex == 0 && DeltaQYDc == 0 && DeltaQUAc == 0 && DeltaQUDc == 0 && DeltaQVAc == 0 && DeltaQVDc == 0) ? 1 : 0
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
 AllLossless = (CodedLossless != 0 && ( FrameWidth == UpscaledWidth )) ? 1 : 0
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
		private int idLen;
		private int allFrames;
		private int show_existing_frame;
		private int frame_type;
		private int FrameIsIntra;
		private int show_frame;
		private int showable_frame;
		private int frame_to_show_map_idx;
		private int temporal_point_info;
		private int error_resilient_mode;
		private int[] RefValid= new int[AV1Constants.NUM_REF_FRAMES];
		private int[] RefOrderHint= new int[AV1Constants.NUM_REF_FRAMES];
		private int[] OrderHints= new int[AV1RefFrames.LAST_FRAME + AV1Constants.REFS_PER_FRAME];
		private int disable_cdf_update;
		private int allow_screen_content_tools;
		private int force_integer_mv;
		private int PrevFrameID;
		private int current_frame_id;
		private int mark_ref_frames;
		private int frame_size_override_flag;
		private int order_hint;
		private int OrderHint;
		private int primary_ref_frame;
		private int buffer_removal_time_present_flag;
		private int opPtIdc;
		private int[] buffer_removal_time;
		private int allow_high_precision_mv;
		private int use_ref_frame_mvs;
		private int allow_intrabc;
		private int refresh_frame_flags;
		private int[] ref_order_hint;
		private int frame_size;
		private int render_size;
		private int frame_refs_short_signaling;
		private int last_frame_idx;
		private int gold_frame_idx;
		private int set_frame_refs;
		private int[] ref_frame_idx= new int[AV1Constants.REFS_PER_FRAME];
		private int delta_frame_id_minus_1;
		private int DeltaFrameId;
		private int[] expectedFrameId;
		private int frame_size_with_refs;
		private int read_interpolation_filter;
		private int is_motion_mode_switchable;
		private int refFrame;
		private int hint;
		private int[] RefFrameSignBias= new int[AV1Constants.REFS_PER_FRAME + AV1RefFrames.LAST_FRAME];
		private int disable_frame_end_update_cdf;
		private int init_non_coeff_cdfs;
		private int setup_past_independence;
		private int load_cdfs;
		private int load_previous;
		private int motion_field_estimation;
		private int tile_info;
		private int quantization_params;
		private int segmentation_params;
		private int delta_q_params;
		private int delta_lf_params;
		private int init_coeff_cdfs;
		private int load_previous_segment_ids;
		private int CodedLossless;
		private int qindex;
		private int[] LosslessArray= new int[AV1Constants.MAX_SEGMENTS];
		private int[][] SegQMLevel;
		private int AllLossless;
		private int loop_filter_params;
		private int cdef_params;
		private int lr_params;
		private int read_tx_mode;
		private int frame_reference_mode;
		private int skip_mode_params;
		private int allow_warped_motion;
		private int reduced_tx_set;
		private int global_motion_params;
		private int film_grain_params;

			int opNum = 0;
			int segmentId = 0;
         public void ReadUncompressedHeader()
         {


			if ( frame_id_numbers_present_flag != 0 )
			{
				idLen= ( additional_frame_id_length_minus_1 + delta_frame_id_length_minus_2 + 3 );
			}
			allFrames= (1 << AV1Constants.NUM_REF_FRAMES) - 1;

			if ( reduced_still_picture_header != 0 )
			{
				show_existing_frame= 0;
				frame_type= AV1FrameTypes.KEY_FRAME;
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
/*  if ( decoder_model_info_present_flag && !equal_picture_interval ) {
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
       }  */

					return;
				}
				stream.ReadFixed(2, out this.frame_type, "frame_type"); 
				FrameIsIntra= ((frame_type == AV1FrameTypes.INTRA_ONLY_FRAME || frame_type == AV1FrameTypes.KEY_FRAME) ? 1 : 0);
				stream.ReadFixed(1, out this.show_frame, "show_frame"); 

				if ( show_frame != 0 && decoder_model_info_present_flag != 0 && equal_picture_interval== 0 )
				{
					ReadTemporalPointInfo(); 
				}

				if ( show_frame != 0 )
				{
					showable_frame= (frame_type != AV1FrameTypes.KEY_FRAME) ? 1 : 0;
				}
				else 
				{
					stream.ReadFixed(1, out this.showable_frame, "showable_frame"); 
				}

				if ( frame_type == AV1FrameTypes.SWITCH_FRAME || ( frame_type == AV1FrameTypes.KEY_FRAME && show_frame != 0 ) )
				{
					error_resilient_mode= 1;
				}
				else 
				{
					stream.ReadFixed(1, out this.error_resilient_mode, "error_resilient_mode"); 
				}
			}

			if ( frame_type == AV1FrameTypes.KEY_FRAME && show_frame != 0 )
			{

				for ( i = 0; i < AV1Constants.NUM_REF_FRAMES; i++ )
				{
					RefValid[ i ]= 0;
					RefOrderHint[ i ]= 0;
				}

				for ( i = 0; i < AV1Constants.REFS_PER_FRAME; i++ )
				{
					OrderHints[ AV1RefFrames.LAST_FRAME + i ]= 0;
				}
			}
			stream.ReadFixed(1, out this.disable_cdf_update, "disable_cdf_update"); 

			if ( seq_force_screen_content_tools == AV1Constants.SELECT_SCREEN_CONTENT_TOOLS )
			{
				stream.ReadFixed(1, out this.allow_screen_content_tools, "allow_screen_content_tools"); 
			}
			else 
			{
				allow_screen_content_tools= seq_force_screen_content_tools;
			}

			if ( allow_screen_content_tools != 0 )
			{

				if ( seq_force_integer_mv == AV1Constants.SELECT_INTEGER_MV )
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
				stream.ReadVariable(idLen, out this.current_frame_id, "current_frame_id"); 
				ReadMarkRefFrames( idLen ); 
			}
			else 
			{
				current_frame_id= 0;
			}

			if ( frame_type == AV1FrameTypes.SWITCH_FRAME )
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
			stream.ReadVariable(OrderHintBits, out this.order_hint, "order_hint"); 
			OrderHint= order_hint;

			if ( FrameIsIntra != 0 || error_resilient_mode != 0 )
			{
				primary_ref_frame= AV1Constants.PRIMARY_REF_NONE;
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

					for ( opNum = 0; opNum <= operating_points_cnt_minus_1; opNum++ )
					{

						if ( decoder_model_present_for_this_op[ opNum ] != 0 )
						{
							opPtIdc= operating_point_idc[ opNum ];
							inTemporalLayer= ( opPtIdc >> (int)temporal_id ) & 1;
							inSpatialLayer= ( opPtIdc >> (int)( spatial_id + 8 ) ) & 1;

							if ( opPtIdc == 0 || ( inTemporalLayer != 0 && inSpatialLayer != 0 ) )
							{
								n= buffer_removal_time_length_minus_1 + 1;
								stream.ReadVariable(n, out this.buffer_removal_time[ opNum ], "buffer_removal_time"); 
							}
						}
					}
				}
			}
			allow_high_precision_mv= 0;
			use_ref_frame_mvs= 0;
			allow_intrabc= 0;

			if ( frame_type == AV1FrameTypes.SWITCH_FRAME || ( frame_type == AV1FrameTypes.KEY_FRAME && show_frame != 0 ) )
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

					for ( i = 0; i < AV1Constants.NUM_REF_FRAMES; i++)
					{
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

				for ( i = 0; i < AV1Constants.REFS_PER_FRAME; i++ )
				{

					if ( frame_refs_short_signaling== 0 )
					{
						stream.ReadFixed(3, out this.ref_frame_idx[ i ], "ref_frame_idx"); 
					}

					if ( frame_id_numbers_present_flag != 0 )
					{
						n= delta_frame_id_length_minus_2 + 2;
						stream.ReadVariable(n, out this.delta_frame_id_minus_1, "delta_frame_id_minus_1"); 
						DeltaFrameId= delta_frame_id_minus_1 + 1;
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

				for ( i = 0; i < AV1Constants.REFS_PER_FRAME; i++ )
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
						RefFrameSignBias[ refFrame ]= (ReadGetRelativeDist( hint, OrderHint) > 0) ? 1 : 0;
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

			if ( primary_ref_frame == AV1Constants.PRIMARY_REF_NONE )
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

			if ( primary_ref_frame == AV1Constants.PRIMARY_REF_NONE )
			{
				ReadInitCoeffCdfs(); 
			}
			else 
			{
				ReadLoadPreviousSegmentIds(); 
			}
			CodedLossless= 1;

			for ( segmentId = 0; segmentId < AV1Constants.MAX_SEGMENTS; segmentId++ )
			{
				qindex= GetQIndex( 1, segmentId );
				LosslessArray[ segmentId ]= (qindex == 0 && DeltaQYDc == 0 && DeltaQUAc == 0 && DeltaQUDc == 0 && DeltaQVAc == 0 && DeltaQVDc == 0) ? 1 : 0;

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
			AllLossless= (CodedLossless != 0 && ( FrameWidth == UpscaledWidth )) ? 1 : 0;
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
			allFrames= (1 << AV1Constants.NUM_REF_FRAMES) - 1;

			if ( reduced_still_picture_header != 0 )
			{
				show_existing_frame= 0;
				frame_type= AV1FrameTypes.KEY_FRAME;
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
/*  if ( decoder_model_info_present_flag && !equal_picture_interval ) {
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
       }  */

					return;
				}
				stream.WriteFixed(2, this.frame_type, "frame_type"); 
				FrameIsIntra= ((frame_type == AV1FrameTypes.INTRA_ONLY_FRAME || frame_type == AV1FrameTypes.KEY_FRAME) ? 1 : 0);
				stream.WriteFixed(1, this.show_frame, "show_frame"); 

				if ( show_frame != 0 && decoder_model_info_present_flag != 0 && equal_picture_interval== 0 )
				{
					WriteTemporalPointInfo(); 
				}

				if ( show_frame != 0 )
				{
					showable_frame= (frame_type != AV1FrameTypes.KEY_FRAME) ? 1 : 0;
				}
				else 
				{
					stream.WriteFixed(1, this.showable_frame, "showable_frame"); 
				}

				if ( frame_type == AV1FrameTypes.SWITCH_FRAME || ( frame_type == AV1FrameTypes.KEY_FRAME && show_frame != 0 ) )
				{
					error_resilient_mode= 1;
				}
				else 
				{
					stream.WriteFixed(1, this.error_resilient_mode, "error_resilient_mode"); 
				}
			}

			if ( frame_type == AV1FrameTypes.KEY_FRAME && show_frame != 0 )
			{

				for ( i = 0; i < AV1Constants.NUM_REF_FRAMES; i++ )
				{
					RefValid[ i ]= 0;
					RefOrderHint[ i ]= 0;
				}

				for ( i = 0; i < AV1Constants.REFS_PER_FRAME; i++ )
				{
					OrderHints[ AV1RefFrames.LAST_FRAME + i ]= 0;
				}
			}
			stream.WriteFixed(1, this.disable_cdf_update, "disable_cdf_update"); 

			if ( seq_force_screen_content_tools == AV1Constants.SELECT_SCREEN_CONTENT_TOOLS )
			{
				stream.WriteFixed(1, this.allow_screen_content_tools, "allow_screen_content_tools"); 
			}
			else 
			{
				allow_screen_content_tools= seq_force_screen_content_tools;
			}

			if ( allow_screen_content_tools != 0 )
			{

				if ( seq_force_integer_mv == AV1Constants.SELECT_INTEGER_MV )
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
				stream.WriteVariable(idLen, this.current_frame_id, "current_frame_id"); 
				WriteMarkRefFrames( idLen ); 
			}
			else 
			{
				current_frame_id= 0;
			}

			if ( frame_type == AV1FrameTypes.SWITCH_FRAME )
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
			stream.WriteVariable(OrderHintBits, this.order_hint, "order_hint"); 
			OrderHint= order_hint;

			if ( FrameIsIntra != 0 || error_resilient_mode != 0 )
			{
				primary_ref_frame= AV1Constants.PRIMARY_REF_NONE;
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
							inTemporalLayer= ( opPtIdc >> (int)temporal_id ) & 1;
							inSpatialLayer= ( opPtIdc >> (int)( spatial_id + 8 ) ) & 1;

							if ( opPtIdc == 0 || ( inTemporalLayer != 0 && inSpatialLayer != 0 ) )
							{
								n= buffer_removal_time_length_minus_1 + 1;
								stream.WriteVariable(n, this.buffer_removal_time[ opNum ], "buffer_removal_time"); 
							}
						}
					}
				}
			}
			allow_high_precision_mv= 0;
			use_ref_frame_mvs= 0;
			allow_intrabc= 0;

			if ( frame_type == AV1FrameTypes.SWITCH_FRAME || ( frame_type == AV1FrameTypes.KEY_FRAME && show_frame != 0 ) )
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

					for ( i = 0; i < AV1Constants.NUM_REF_FRAMES; i++)
					{
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

				for ( i = 0; i < AV1Constants.REFS_PER_FRAME; i++ )
				{

					if ( frame_refs_short_signaling== 0 )
					{
						stream.WriteFixed(3, this.ref_frame_idx[ i ], "ref_frame_idx"); 
					}

					if ( frame_id_numbers_present_flag != 0 )
					{
						n= delta_frame_id_length_minus_2 + 2;
						stream.WriteVariable(n, this.delta_frame_id_minus_1, "delta_frame_id_minus_1"); 
						DeltaFrameId= delta_frame_id_minus_1 + 1;
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

				for ( i = 0; i < AV1Constants.REFS_PER_FRAME; i++ )
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
						RefFrameSignBias[ refFrame ]= (WriteGetRelativeDist( hint, OrderHint) > 0) ? 1 : 0;
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

			if ( primary_ref_frame == AV1Constants.PRIMARY_REF_NONE )
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

			if ( primary_ref_frame == AV1Constants.PRIMARY_REF_NONE )
			{
				WriteInitCoeffCdfs(); 
			}
			else 
			{
				WriteLoadPreviousSegmentIds(); 
			}
			CodedLossless= 1;

			for ( segmentId = 0; segmentId < AV1Constants.MAX_SEGMENTS; segmentId++ )
			{
				qindex= GetQIndex( 1, segmentId );
				LosslessArray[ segmentId ]= (qindex == 0 && DeltaQYDc == 0 && DeltaQUAc == 0 && DeltaQUDc == 0 && DeltaQVAc == 0 && DeltaQVDc == 0) ? 1 : 0;

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
			AllLossless= (CodedLossless != 0 && ( FrameWidth == UpscaledWidth )) ? 1 : 0;
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
		private int frame_presentation_time;

         public void ReadTemporalPointInfo()
         {

			n= frame_presentation_time_length_minus_1 + 1;
			stream.ReadVariable(n, out this.frame_presentation_time, "frame_presentation_time"); 
         }

         public void WriteTemporalPointInfo()
         {

			n= frame_presentation_time_length_minus_1 + 1;
			stream.WriteVariable(n, this.frame_presentation_time, "frame_presentation_time"); 
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
		private int frame_width_minus_1;
		private int frame_height_minus_1;
		private int FrameWidth;
		private int FrameHeight;
		private int superres_params;
		private int compute_image_size;

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
		private int render_and_frame_size_different;
		private int render_width_minus_1;
		private int render_height_minus_1;
		private int RenderWidth;
		private int RenderHeight;

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
		private int found_ref;
		private int UpscaledWidth;

         public void ReadFrameSizeWithRefs()
         {


			for ( i = 0; i < AV1Constants.REFS_PER_FRAME; i++ )
			{
				stream.ReadFixed(1, out this.found_ref, "found_ref"); 

				if ( found_ref == 1 )
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


			for ( i = 0; i < AV1Constants.REFS_PER_FRAME; i++ )
			{
				stream.WriteFixed(1, this.found_ref, "found_ref"); 

				if ( found_ref == 1 )
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
		private int is_filter_switchable;
		private int interpolation_filter;

         public void ReadReadInterpolationFilter()
         {

			stream.ReadFixed(1, out this.is_filter_switchable, "is_filter_switchable"); 

			if ( is_filter_switchable == 1 )
			{
				interpolation_filter= AV1InterpolationFilter.SWITCHABLE;
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
				interpolation_filter= AV1InterpolationFilter.SWITCHABLE;
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
		private int a;
		private int b;
		private int diff;
		private int m;

         public int ReadGetRelativeDist(int a, int b)
         {


			if ( enable_order_hint== 0 )
			{
				return 0;
			}
			diff= a - b;
			m= 1 << (OrderHintBits - 1);
			diff= (diff & (m - 1)) - (diff & m);
			return diff;
         }

         public int WriteGetRelativeDist(int a, int b)
         {


			if ( enable_order_hint== 0 )
			{
				return 0;
			}
			diff= a - b;
			m= 1 << (OrderHintBits - 1);
			diff= (diff & (m - 1)) - (diff & m);
			return diff;
         }

    /*


tile_info () { 
 sbCols = use_128x128_superblock != 0 ? ( ( MiCols + 31 ) >> 5 ) : ( ( MiCols + 15 ) >> 4 )
 sbRows = use_128x128_superblock != 0 ? ( ( MiRows + 31 ) >> 5 ) : ( ( MiRows + 15 ) >> 4 )
 sbShift = use_128x128_superblock != 0 ? 5 : 4
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
 sizeSb = (int)(width_in_sbs_minus_1 + 1)
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
 sizeSb = (int)(height_in_sbs_minus_1 + 1)
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
		private int sbCols;
		private int sbRows;
		private int sbShift;
		private int sbSize;
		private int maxTileWidthSb;
		private int maxTileAreaSb;
		private int minLog2TileCols;
		private int maxLog2TileCols;
		private int maxLog2TileRows;
		private int minLog2Tiles;
		private int uniform_tile_spacing_flag;
		private int TileColsLog2;
		private int increment_tile_cols_log2;
		private int tileWidthSb;
		private int[] MiColStarts= new int[AV1Constants.MAX_TILE_COLS];
		private int TileCols;
		private int minLog2TileRows;
		private int TileRowsLog2;
		private int increment_tile_rows_log2;
		private int tileHeightSb;
		private int[] MiRowStarts= new int[AV1Constants.MAX_TILE_ROWS];
		private int TileRows;
		private int widestTileSb;
		private int startSb;
		private int maxWidth;
		private uint width_in_sbs_minus_1;
		private int sizeSb;
		private int maxTileHeightSb;
		private int maxHeight;
		private uint height_in_sbs_minus_1;
		private int context_update_tile_id;
		private int tile_size_bytes_minus_1;
		private int TileSizeBytes;

         public void ReadTileInfo()
         {

			sbCols= use_128x128_superblock != 0 ? ( ( MiCols + 31 ) >> (int)5 ) : ( ( MiCols + 15 ) >> (int)4 );
			sbRows= use_128x128_superblock != 0 ? ( ( MiRows + 31 ) >> (int)5 ) : ( ( MiRows + 15 ) >> (int)4 );
			sbShift= use_128x128_superblock != 0 ? 5 : 4;
			sbSize= sbShift + 2;
			maxTileWidthSb= AV1Constants.MAX_TILE_WIDTH >> (int)sbSize;
			maxTileAreaSb= AV1Constants.MAX_TILE_AREA >> (int)( 2 * sbSize );
			minLog2TileCols= ReadTileLog2(maxTileWidthSb, sbCols);
			maxLog2TileCols= ReadTileLog2(1, Math.Min(sbCols, AV1Constants.MAX_TILE_COLS));
			maxLog2TileRows= ReadTileLog2(1, Math.Min(sbRows, AV1Constants.MAX_TILE_ROWS));
			minLog2Tiles= Math.Max(minLog2TileCols, ReadTileLog2(maxTileAreaSb, sbRows * sbCols));
			stream.ReadFixed(1, out this.uniform_tile_spacing_flag, "uniform_tile_spacing_flag"); 

			if ( uniform_tile_spacing_flag != 0 )
			{
				TileColsLog2= minLog2TileCols;

				while ( TileColsLog2 < maxLog2TileCols )
				{
					stream.ReadFixed(1, out this.increment_tile_cols_log2, "increment_tile_cols_log2"); 

					if ( increment_tile_cols_log2 == 1 )
					{
						TileColsLog2++;
					}
					else 
					{
						break;
					}
				}
				tileWidthSb= (sbCols + (1 << TileColsLog2) - 1) >> (int)TileColsLog2;
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
					stream.ReadFixed(1, out this.increment_tile_rows_log2, "increment_tile_rows_log2"); 

					if ( increment_tile_rows_log2 == 1 )
					{
						TileRowsLog2++;
					}
					else 
					{
						break;
					}
				}
				tileHeightSb= (sbRows + (1 << TileRowsLog2) - 1) >> (int)TileRowsLog2;
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
					stream.Read_ns(maxWidth, out this.width_in_sbs_minus_1, "width_in_sbs_minus_1"); 
					sizeSb= (int)(width_in_sbs_minus_1 + 1);
					widestTileSb= Math.Max( sizeSb, widestTileSb );
					startSb+= sizeSb;
				}
				MiColStarts[i]= MiCols;
				TileCols= i;
				TileColsLog2= ReadTileLog2(1, TileCols);

				if ( minLog2Tiles > 0 )
				{
					maxTileAreaSb= (sbRows * sbCols) >> (int)(minLog2Tiles + 1);
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
					stream.Read_ns(maxHeight, out this.height_in_sbs_minus_1, "height_in_sbs_minus_1"); 
					sizeSb= (int)(height_in_sbs_minus_1 + 1);
					startSb+= sizeSb;
				}
				MiRowStarts[ i ]= MiRows;
				TileRows= i;
				TileRowsLog2= ReadTileLog2(1, TileRows);
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

			sbCols= use_128x128_superblock != 0 ? ( ( MiCols + 31 ) >> (int)5 ) : ( ( MiCols + 15 ) >> (int)4 );
			sbRows= use_128x128_superblock != 0 ? ( ( MiRows + 31 ) >> (int)5 ) : ( ( MiRows + 15 ) >> (int)4 );
			sbShift= use_128x128_superblock != 0 ? 5 : 4;
			sbSize= sbShift + 2;
			maxTileWidthSb= AV1Constants.MAX_TILE_WIDTH >> (int)sbSize;
			maxTileAreaSb= AV1Constants.MAX_TILE_AREA >> (int)( 2 * sbSize );
			minLog2TileCols= WriteTileLog2(maxTileWidthSb, sbCols);
			maxLog2TileCols= WriteTileLog2(1, Math.Min(sbCols, AV1Constants.MAX_TILE_COLS));
			maxLog2TileRows= WriteTileLog2(1, Math.Min(sbRows, AV1Constants.MAX_TILE_ROWS));
			minLog2Tiles= Math.Max(minLog2TileCols, WriteTileLog2(maxTileAreaSb, sbRows * sbCols));
			stream.WriteFixed(1, this.uniform_tile_spacing_flag, "uniform_tile_spacing_flag"); 

			if ( uniform_tile_spacing_flag != 0 )
			{
				TileColsLog2= minLog2TileCols;

				while ( TileColsLog2 < maxLog2TileCols )
				{
					stream.WriteFixed(1, this.increment_tile_cols_log2, "increment_tile_cols_log2"); 

					if ( increment_tile_cols_log2 == 1 )
					{
						TileColsLog2++;
					}
					else 
					{
						break;
					}
				}
				tileWidthSb= (sbCols + (1 << TileColsLog2) - 1) >> (int)TileColsLog2;
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
					stream.WriteFixed(1, this.increment_tile_rows_log2, "increment_tile_rows_log2"); 

					if ( increment_tile_rows_log2 == 1 )
					{
						TileRowsLog2++;
					}
					else 
					{
						break;
					}
				}
				tileHeightSb= (sbRows + (1 << TileRowsLog2) - 1) >> (int)TileRowsLog2;
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
					stream.Write_ns(maxWidth, this.width_in_sbs_minus_1, "width_in_sbs_minus_1"); 
					sizeSb= (int)(width_in_sbs_minus_1 + 1);
					widestTileSb= Math.Max( sizeSb, widestTileSb );
					startSb+= sizeSb;
				}
				MiColStarts[i]= MiCols;
				TileCols= i;
				TileColsLog2= WriteTileLog2(1, TileCols);

				if ( minLog2Tiles > 0 )
				{
					maxTileAreaSb= (sbRows * sbCols) >> (int)(minLog2Tiles + 1);
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
					stream.Write_ns(maxHeight, this.height_in_sbs_minus_1, "height_in_sbs_minus_1"); 
					sizeSb= (int)(height_in_sbs_minus_1 + 1);
					startSb+= sizeSb;
				}
				MiRowStarts[ i ]= MiRows;
				TileRows= i;
				TileRowsLog2= WriteTileLog2(1, TileRows);
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
		private int blkSize;
		private int target;

			int k = 0;
         public int ReadTileLog2(int blkSize, int target)
         {


			for ( k = 0; (blkSize << (int) k) < target; k++ )
			{
			}
			return k;
         }

         public int WriteTileLog2(int blkSize, int target)
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
		private int base_q_idx;
		private int DeltaQYDc;
		private int diff_uv_delta;
		private int DeltaQUDc;
		private int DeltaQUAc;
		private int DeltaQVDc;
		private int DeltaQVAc;
		private int using_qmatrix;
		private int qm_y;
		private int qm_u;
		private int qm_v;

         public void ReadQuantizationParams()
         {

			stream.ReadFixed(8, out this.base_q_idx, "base_q_idx"); 
			DeltaQYDc= ReadReadDeltaq();

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
				DeltaQUDc= ReadReadDeltaq();
				DeltaQUAc= ReadReadDeltaq();

				if ( diff_uv_delta != 0 )
				{
					DeltaQVDc= ReadReadDeltaq();
					DeltaQVAc= ReadReadDeltaq();
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
			DeltaQYDc= WriteReadDeltaq();

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
				DeltaQUDc= WriteReadDeltaq();
				DeltaQUAc= WriteReadDeltaq();

				if ( diff_uv_delta != 0 )
				{
					DeltaQVDc= WriteReadDeltaq();
					DeltaQVAc= WriteReadDeltaq();
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
		private int delta_coded;
		private int delta_q;

         public int ReadReadDeltaq()
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

         public int WriteReadDeltaq()
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
 feature_value su(1+bitsToRead)
 clippedValue = Clip3( -limit, limit, feature_value)
 } else {
 feature_value f(bitsToRead)
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
		private int segmentation_enabled;
		private int segmentation_update_map;
		private int segmentation_temporal_update;
		private int segmentation_update_data;
		private int feature_value;
		private int feature_enabled;
		private int[][] FeatureEnabled= new int[AV1Constants.MAX_SEGMENTS][] { new int[AV1Constants.SEG_LVL_MAX],new int[AV1Constants.SEG_LVL_MAX],new int[AV1Constants.SEG_LVL_MAX],new int[AV1Constants.SEG_LVL_MAX],new int[AV1Constants.SEG_LVL_MAX],new int[AV1Constants.SEG_LVL_MAX],new int[AV1Constants.SEG_LVL_MAX],new int[AV1Constants.SEG_LVL_MAX] };
		private int clippedValue;
		private int bitsToRead;
		private int limit;
		private int[][] FeatureData= new int[AV1Constants.MAX_SEGMENTS][] { new int[AV1Constants.SEG_LVL_MAX],new int[AV1Constants.SEG_LVL_MAX],new int[AV1Constants.SEG_LVL_MAX],new int[AV1Constants.SEG_LVL_MAX],new int[AV1Constants.SEG_LVL_MAX],new int[AV1Constants.SEG_LVL_MAX],new int[AV1Constants.SEG_LVL_MAX],new int[AV1Constants.SEG_LVL_MAX] };
		private int SegIdPreSkip;
		private int LastActiveSegId;

			int j = 0;
         public void ReadSegmentationParams()
         {

			stream.ReadFixed(1, out this.segmentation_enabled, "segmentation_enabled"); 

			if ( segmentation_enabled == 1 )
			{

				if ( primary_ref_frame == AV1Constants.PRIMARY_REF_NONE )
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

					for ( i = 0; i < AV1Constants.MAX_SEGMENTS; i++ )
					{

						for ( j = 0; j < AV1Constants.SEG_LVL_MAX; j++ )
						{
							feature_value= 0;
							stream.ReadFixed(1, out this.feature_enabled, "feature_enabled"); 
							FeatureEnabled[ i ][ j ]= feature_enabled;
							clippedValue= 0;

							if ( feature_enabled == 1 )
							{
								bitsToRead= Segmentation_Feature_Bits[ j ];
								limit= Segmentation_Feature_Max[ j ];

								if ( Segmentation_Feature_Signed[ j ] == 1 )
								{
									stream.ReadSignedIntVar(1+bitsToRead, out this.feature_value, "feature_value"); 
									clippedValue= AomStream.Clip3( -limit, limit, feature_value);
								}
								else 
								{
									stream.ReadVariable(bitsToRead, out this.feature_value, "feature_value"); 
									clippedValue= AomStream.Clip3( 0, limit, feature_value);
								}
							}
							FeatureData[ i ][ j ]= clippedValue;
						}
					}
				}
			}
			else 
			{

				for ( i = 0; i < AV1Constants.MAX_SEGMENTS; i++ )
				{

					for ( j = 0; j < AV1Constants.SEG_LVL_MAX; j++ )
					{
						FeatureEnabled[ i ][ j ]= 0;
						FeatureData[ i ][ j ]= 0;
					}
				}
			}
			SegIdPreSkip= 0;
			LastActiveSegId= 0;

			for ( i = 0; i < AV1Constants.MAX_SEGMENTS; i++ )
			{

				for ( j = 0; j < AV1Constants.SEG_LVL_MAX; j++ )
				{

					if ( FeatureEnabled[ i ][ j ] != 0 )
					{
						LastActiveSegId= i;

						if ( j >= AV1Constants.SEG_LVL_REF_FRAME )
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

				if ( primary_ref_frame == AV1Constants.PRIMARY_REF_NONE )
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

					for ( i = 0; i < AV1Constants.MAX_SEGMENTS; i++ )
					{

						for ( j = 0; j < AV1Constants.SEG_LVL_MAX; j++ )
						{
							feature_value= 0;
							stream.WriteFixed(1, this.feature_enabled, "feature_enabled"); 
							FeatureEnabled[ i ][ j ]= feature_enabled;
							clippedValue= 0;

							if ( feature_enabled == 1 )
							{
								bitsToRead= Segmentation_Feature_Bits[ j ];
								limit= Segmentation_Feature_Max[ j ];

								if ( Segmentation_Feature_Signed[ j ] == 1 )
								{
									stream.WriteSignedIntVar(1+bitsToRead, this.feature_value, "feature_value"); 
									clippedValue= AomStream.Clip3( -limit, limit, feature_value);
								}
								else 
								{
									stream.WriteVariable(bitsToRead, this.feature_value, "feature_value"); 
									clippedValue= AomStream.Clip3( 0, limit, feature_value);
								}
							}
							FeatureData[ i ][ j ]= clippedValue;
						}
					}
				}
			}
			else 
			{

				for ( i = 0; i < AV1Constants.MAX_SEGMENTS; i++ )
				{

					for ( j = 0; j < AV1Constants.SEG_LVL_MAX; j++ )
					{
						FeatureEnabled[ i ][ j ]= 0;
						FeatureData[ i ][ j ]= 0;
					}
				}
			}
			SegIdPreSkip= 0;
			LastActiveSegId= 0;

			for ( i = 0; i < AV1Constants.MAX_SEGMENTS; i++ )
			{

				for ( j = 0; j < AV1Constants.SEG_LVL_MAX; j++ )
				{

					if ( FeatureEnabled[ i ][ j ] != 0 )
					{
						LastActiveSegId= i;

						if ( j >= AV1Constants.SEG_LVL_REF_FRAME )
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
		private int delta_q_res;
		private int delta_q_present;

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
		private int delta_lf_present;
		private int delta_lf_res;
		private int delta_lf_multi;

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
		private int cdef_bits;
		private int[] cdef_y_pri_strength= new int[1];
		private int[] cdef_y_sec_strength= new int[1];
		private int[] cdef_uv_pri_strength= new int[1];
		private int[] cdef_uv_sec_strength= new int[1];
		private int CdefDamping;
		private int cdef_damping_minus_3;

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
			CdefDamping= cdef_damping_minus_3 + 3;
			stream.ReadFixed(2, out this.cdef_bits, "cdef_bits"); 

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
			CdefDamping= cdef_damping_minus_3 + 3;
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
		private int[] FrameRestorationType= new int[3];
		private int UsesLr;
		private int usesChromaLr;
		private int lr_type;
		private int lr_unit_shift;
		private int lr_unit_extra_shift;
		private int[] LoopRestorationSize= new int[4];
		private int lr_uv_shift;

         public void ReadLrParams()
         {


			if ( AllLossless != 0 || allow_intrabc != 0 ||
 enable_restoration== 0 )
			{
				FrameRestorationType[0]= AV1FrameRestorationType.RESTORE_NONE;
				FrameRestorationType[1]= AV1FrameRestorationType.RESTORE_NONE;
				FrameRestorationType[2]= AV1FrameRestorationType.RESTORE_NONE;
				UsesLr= 0;
				return;
			}
			UsesLr= 0;
			usesChromaLr= 0;

			for ( i = 0; i < NumPlanes; i++ )
			{
				stream.ReadFixed(2, out this.lr_type, "lr_type"); 
				FrameRestorationType[i]= Remap_Lr_Type[lr_type];

				if ( FrameRestorationType[i] != AV1FrameRestorationType.RESTORE_NONE )
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
				LoopRestorationSize[ 0 ]= AV1Constants.RESTORATION_TILESIZE_MAX >> (int)(2 - lr_unit_shift);

				if ( subsampling_x != 0 && subsampling_y != 0 && usesChromaLr != 0 )
				{
					stream.ReadFixed(1, out this.lr_uv_shift, "lr_uv_shift"); 
				}
				else 
				{
					lr_uv_shift= 0;
				}
				LoopRestorationSize[ 1 ]= LoopRestorationSize[ 0 ] >> (int)lr_uv_shift;
				LoopRestorationSize[ 2 ]= LoopRestorationSize[ 0 ] >> (int)lr_uv_shift;
			}
         }

         public void WriteLrParams()
         {


			if ( AllLossless != 0 || allow_intrabc != 0 ||
 enable_restoration== 0 )
			{
				FrameRestorationType[0]= AV1FrameRestorationType.RESTORE_NONE;
				FrameRestorationType[1]= AV1FrameRestorationType.RESTORE_NONE;
				FrameRestorationType[2]= AV1FrameRestorationType.RESTORE_NONE;
				UsesLr= 0;
				return;
			}
			UsesLr= 0;
			usesChromaLr= 0;

			for ( i = 0; i < NumPlanes; i++ )
			{
				stream.WriteFixed(2, this.lr_type, "lr_type"); 
				FrameRestorationType[i]= Remap_Lr_Type[lr_type];

				if ( FrameRestorationType[i] != AV1FrameRestorationType.RESTORE_NONE )
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
				LoopRestorationSize[ 0 ]= AV1Constants.RESTORATION_TILESIZE_MAX >> (int)(2 - lr_unit_shift);

				if ( subsampling_x != 0 && subsampling_y != 0 && usesChromaLr != 0 )
				{
					stream.WriteFixed(1, this.lr_uv_shift, "lr_uv_shift"); 
				}
				else 
				{
					lr_uv_shift= 0;
				}
				LoopRestorationSize[ 1 ]= LoopRestorationSize[ 0 ] >> (int)lr_uv_shift;
				LoopRestorationSize[ 2 ]= LoopRestorationSize[ 0 ] >> (int)lr_uv_shift;
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
		private int[] loop_filter_level= new int[4];
		private int[] loop_filter_ref_deltas= new int[8];
		private int[] loop_filter_mode_deltas= new int[2];
		private int loop_filter_sharpness;
		private int loop_filter_delta_enabled;
		private int loop_filter_delta_update;
		private int update_ref_delta;
		private int update_mode_delta;

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

					for ( i = 0; i < AV1Constants.TOTAL_REFS_PER_FRAME; i++ )
					{
						stream.ReadFixed(1, out this.update_ref_delta, "update_ref_delta"); 

						if ( update_ref_delta == 1 )
						{
							stream.ReadSignedIntVar(1+6, out this.loop_filter_ref_deltas[ i ], "loop_filter_ref_deltas"); 
						}
					}

					for ( i = 0; i < 2; i++ )
					{
						stream.ReadFixed(1, out this.update_mode_delta, "update_mode_delta"); 

						if ( update_mode_delta == 1 )
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

					for ( i = 0; i < AV1Constants.TOTAL_REFS_PER_FRAME; i++ )
					{
						stream.WriteFixed(1, this.update_ref_delta, "update_ref_delta"); 

						if ( update_ref_delta == 1 )
						{
							stream.WriteSignedIntVar(1+6, this.loop_filter_ref_deltas[ i ], "loop_filter_ref_deltas"); 
						}
					}

					for ( i = 0; i < 2; i++ )
					{
						stream.WriteFixed(1, this.update_mode_delta, "update_mode_delta"); 

						if ( update_mode_delta == 1 )
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
		private int TxMode;
		private int tx_mode_select;

         public void ReadReadTxMode()
         {


			if ( CodedLossless == 1 )
			{
				TxMode= AV1TxModes.ONLY_4X4;
			}
			else 
			{
				stream.ReadFixed(1, out this.tx_mode_select, "tx_mode_select"); 

				if ( tx_mode_select != 0 )
				{
					TxMode= AV1TxModes.TX_MODE_SELECT;
				}
				else 
				{
					TxMode= AV1TxModes.TX_MODE_LARGEST;
				}
			}
         }

         public void WriteReadTxMode()
         {


			if ( CodedLossless == 1 )
			{
				TxMode= AV1TxModes.ONLY_4X4;
			}
			else 
			{
				stream.WriteFixed(1, this.tx_mode_select, "tx_mode_select"); 

				if ( tx_mode_select != 0 )
				{
					TxMode= AV1TxModes.TX_MODE_SELECT;
				}
				else 
				{
					TxMode= AV1TxModes.TX_MODE_LARGEST;
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
		private int reference_select;

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
		private int skipModeAllowed;
		private int forwardIdx;
		private int backwardIdx;
		private int refHint;
		private int forwardHint;
		private int backwardHint;
		private int[] SkipModeFrame;
		private int secondForwardIdx;
		private int secondForwardHint;
		private int skip_mode_present;

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

				for ( i = 0; i < AV1Constants.REFS_PER_FRAME; i++ )
				{
					refHint= RefOrderHint[ ref_frame_idx[ i ] ];

					if ( ReadGetRelativeDist( refHint, OrderHint ) < 0 )
					{

						if ( forwardIdx < 0 ||
 ReadGetRelativeDist( refHint, forwardHint) > 0 )
						{
							forwardIdx= i;
							forwardHint= refHint;
						}
					}
					else if ( ReadGetRelativeDist( refHint, OrderHint) > 0 )
					{

						if ( backwardIdx < 0 ||
 ReadGetRelativeDist( refHint, backwardHint) < 0 )
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

					for ( i = 0; i < AV1Constants.REFS_PER_FRAME; i++ )
					{
						refHint= RefOrderHint[ ref_frame_idx[ i ] ];

						if ( ReadGetRelativeDist( refHint, forwardHint ) < 0 )
						{

							if ( secondForwardIdx < 0 ||
 ReadGetRelativeDist( refHint, secondForwardHint ) > 0 )
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

				for ( i = 0; i < AV1Constants.REFS_PER_FRAME; i++ )
				{
					refHint= RefOrderHint[ ref_frame_idx[ i ] ];

					if ( WriteGetRelativeDist( refHint, OrderHint ) < 0 )
					{

						if ( forwardIdx < 0 ||
 WriteGetRelativeDist( refHint, forwardHint) > 0 )
						{
							forwardIdx= i;
							forwardHint= refHint;
						}
					}
					else if ( WriteGetRelativeDist( refHint, OrderHint) > 0 )
					{

						if ( backwardIdx < 0 ||
 WriteGetRelativeDist( refHint, backwardHint) < 0 )
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

					for ( i = 0; i < AV1Constants.REFS_PER_FRAME; i++ )
					{
						refHint= RefOrderHint[ ref_frame_idx[ i ] ];

						if ( WriteGetRelativeDist( refHint, forwardHint ) < 0 )
						{

							if ( secondForwardIdx < 0 ||
 WriteGetRelativeDist( refHint, secondForwardHint ) > 0 )
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
 type = is_translation != 0 ? TRANSLATION : AFFINE
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
		private int[] GmType= new int[AV1RefFrames.ALTREF_FRAME + 1];
		private int[][] gm_params= new int[AV1RefFrames.ALTREF_FRAME + 1][] { new int[6],new int[6],new int[6],new int[6],new int[6],new int[6],new int[6],new int[6] };
		private int is_global;
		private int is_rot_zoom;
		private int type;
		private int is_translation;
		private int read_global_param;

			int refc = 0;
         public void ReadGlobalMotionParams()
         {


			for ( refc = AV1RefFrames.LAST_FRAME; refc <= AV1RefFrames.ALTREF_FRAME; refc++ )
			{
				GmType[ refc ]= AV1Constants.IDENTITY;

				for ( i = 0; i < 6; i++ )
				{
					gm_params[ refc ][ i ]= ( ( i % 3 == 2 ) ? 1 << AV1Constants.WARPEDMODEL_PREC_BITS : 0 );
				}
			}

			if ( FrameIsIntra != 0 )
			{
				return;
			}

			for ( refc = AV1RefFrames.LAST_FRAME; refc <= AV1RefFrames.ALTREF_FRAME; refc++ )
			{
				stream.ReadFixed(1, out this.is_global, "is_global"); 

				if ( is_global != 0 )
				{
					stream.ReadFixed(1, out this.is_rot_zoom, "is_rot_zoom"); 

					if ( is_rot_zoom != 0 )
					{
						type= AV1Constants.ROTZOOM;
					}
					else 
					{
						stream.ReadFixed(1, out this.is_translation, "is_translation"); 
						type= is_translation != 0 ? AV1Constants.TRANSLATION : AV1Constants.AFFINE;
					}
				}
				else 
				{
					type= AV1Constants.IDENTITY;
				}
				GmType[refc]= type;

				if ( type >= AV1Constants.ROTZOOM )
				{
					ReadReadGlobalParam(type, refc, 2); 
					ReadReadGlobalParam(type, refc, 3); 

					if ( type == AV1Constants.AFFINE )
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

				if ( type >= AV1Constants.TRANSLATION )
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
				GmType[ refc ]= AV1Constants.IDENTITY;

				for ( i = 0; i < 6; i++ )
				{
					gm_params[ refc ][ i ]= ( ( i % 3 == 2 ) ? 1 << AV1Constants.WARPEDMODEL_PREC_BITS : 0 );
				}
			}

			if ( FrameIsIntra != 0 )
			{
				return;
			}

			for ( refc = AV1RefFrames.LAST_FRAME; refc <= AV1RefFrames.ALTREF_FRAME; refc++ )
			{
				stream.WriteFixed(1, this.is_global, "is_global"); 

				if ( is_global != 0 )
				{
					stream.WriteFixed(1, this.is_rot_zoom, "is_rot_zoom"); 

					if ( is_rot_zoom != 0 )
					{
						type= AV1Constants.ROTZOOM;
					}
					else 
					{
						stream.WriteFixed(1, this.is_translation, "is_translation"); 
						type= is_translation != 0 ? AV1Constants.TRANSLATION : AV1Constants.AFFINE;
					}
				}
				else 
				{
					type= AV1Constants.IDENTITY;
				}
				GmType[refc]= type;

				if ( type >= AV1Constants.ROTZOOM )
				{
					WriteReadGlobalParam(type, refc, 2); 
					WriteReadGlobalParam(type, refc, 3); 

					if ( type == AV1Constants.AFFINE )
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

				if ( type >= AV1Constants.TRANSLATION )
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
 absBits = GM_ABS_TRANS_ONLY_BITS - (allow_high_precision_mv == 0 ? 1 : 0)
 precBits = GM_TRANS_ONLY_PREC_BITS - (allow_high_precision_mv == 0 ? 1 : 0)
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
		private int idx;
		private int absBits;
		private int precBits;
		private int precDiff;
		private int round;
		private int sub;
		private int mx;
		private int r;

         public void ReadReadGlobalParam(int type, int refc, int idx)
         {

			absBits= AV1Constants.GM_ABS_ALPHA_BITS;
			precBits= AV1Constants.GM_ALPHA_PREC_BITS;

			if ( idx < 2 )
			{

				if ( type == AV1Constants.TRANSLATION )
				{
					absBits= AV1Constants.GM_ABS_TRANS_ONLY_BITS - (allow_high_precision_mv == 0 ? 1 : 0);
					precBits= AV1Constants.GM_TRANS_ONLY_PREC_BITS - (allow_high_precision_mv == 0 ? 1 : 0);
				}
				else 
				{
					absBits= AV1Constants.GM_ABS_TRANS_BITS;
					precBits= AV1Constants.GM_TRANS_PREC_BITS;
				}
			}
			precDiff= AV1Constants.WARPEDMODEL_PREC_BITS - precBits;
			round= (idx % 3) == 2 ? (1 << AV1Constants.WARPEDMODEL_PREC_BITS) : 0;
			sub= (idx % 3) == 2 ? (1 << precBits) : 0;
			mx= (1 << absBits);
			r= (PrevGmParams[refc][idx] >> (int)precDiff) - sub;
			gm_params[refc][idx]= (ReadDecodeSignedSubexpWithRef( -mx, mx + 1, r ) << precDiff) + round;
         }

         public void WriteReadGlobalParam(int type, int refc, int idx)
         {

			absBits= AV1Constants.GM_ABS_ALPHA_BITS;
			precBits= AV1Constants.GM_ALPHA_PREC_BITS;

			if ( idx < 2 )
			{

				if ( type == AV1Constants.TRANSLATION )
				{
					absBits= AV1Constants.GM_ABS_TRANS_ONLY_BITS - (allow_high_precision_mv == 0 ? 1 : 0);
					precBits= AV1Constants.GM_TRANS_ONLY_PREC_BITS - (allow_high_precision_mv == 0 ? 1 : 0);
				}
				else 
				{
					absBits= AV1Constants.GM_ABS_TRANS_BITS;
					precBits= AV1Constants.GM_TRANS_PREC_BITS;
				}
			}
			precDiff= AV1Constants.WARPEDMODEL_PREC_BITS - precBits;
			round= (idx % 3) == 2 ? (1 << AV1Constants.WARPEDMODEL_PREC_BITS) : 0;
			sub= (idx % 3) == 2 ? (1 << precBits) : 0;
			mx= (1 << absBits);
			r= (PrevGmParams[refc][idx] >> (int)precDiff) - sub;
			gm_params[refc][idx]= (WriteDecodeSignedSubexpWithRef( -mx, mx + 1, r ) << precDiff) + round;
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
		private int reset_grain_params;
		private int apply_grain;
		private int grain_seed;
		private int update_grain;
		private int film_grain_params_ref_idx;
		private int tempGrainSeed;
		private int load_grain_params;
		private int num_y_points;
		private int[] point_y_value;
		private int[] point_y_scaling;
		private int chroma_scaling_from_luma;
		private int num_cb_points;
		private int num_cr_points;
		private int[] point_cb_value;
		private int[] point_cb_scaling;
		private int[] point_cr_value;
		private int[] point_cr_scaling;
		private int grain_scaling_minus_8;
		private int ar_coeff_lag;
		private int numPosLuma;
		private int numPosChroma;
		private int[] ar_coeffs_y_plus_128;
		private int[] ar_coeffs_cb_plus_128;
		private int[] ar_coeffs_cr_plus_128;
		private int ar_coeff_shift_minus_6;
		private int grain_scale_shift;
		private int cb_mult;
		private int cb_luma_mult;
		private int cb_offset;
		private int cr_mult;
		private int cr_luma_mult;
		private int cr_offset;
		private int overlap_flag;
		private int clip_to_restricted_range;

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

			if ( frame_type == AV1FrameTypes.INTER_FRAME )
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

				for ( i = 0; i < num_cb_points; i++ )
				{
					stream.ReadFixed(8, out this.point_cb_value[ i ], "point_cb_value"); 
					stream.ReadFixed(8, out this.point_cb_scaling[ i ], "point_cb_scaling"); 
				}
				stream.ReadFixed(4, out this.num_cr_points, "num_cr_points"); 

				for ( i = 0; i < num_cr_points; i++ )
				{
					stream.ReadFixed(8, out this.point_cr_value[ i ], "point_cr_value"); 
					stream.ReadFixed(8, out this.point_cr_scaling[ i ], "point_cr_scaling"); 
				}
			}
			stream.ReadFixed(2, out this.grain_scaling_minus_8, "grain_scaling_minus_8"); 
			stream.ReadFixed(2, out this.ar_coeff_lag, "ar_coeff_lag"); 
			numPosLuma= 2 * ar_coeff_lag * ( ar_coeff_lag + 1 );

			if ( num_y_points != 0 )
			{
				numPosChroma= numPosLuma + 1;

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

				for ( i = 0; i < numPosChroma; i++ )
				{
					stream.ReadFixed(8, out this.ar_coeffs_cb_plus_128[ i ], "ar_coeffs_cb_plus_128"); 
				}
			}

			if ( chroma_scaling_from_luma != 0 || num_cr_points != 0 )
			{

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

			if ( frame_type == AV1FrameTypes.INTER_FRAME )
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
			numPosLuma= 2 * ar_coeff_lag * ( ar_coeff_lag + 1 );

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
		private int use_superres;
		private int coded_denom;
		private int SuperresDenom;

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
				stream.ReadVariable(AV1Constants.SUPERRES_DENOM_BITS, out this.coded_denom, "coded_denom"); 
				SuperresDenom= coded_denom + AV1Constants.SUPERRES_DENOM_MIN;
			}
			else 
			{
				SuperresDenom= AV1Constants.SUPERRES_NUM;
			}
			UpscaledWidth= FrameWidth;
			FrameWidth= (UpscaledWidth * AV1Constants.SUPERRES_NUM + (SuperresDenom / 2)) / SuperresDenom;
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
				stream.WriteVariable(AV1Constants.SUPERRES_DENOM_BITS, this.coded_denom, "coded_denom"); 
				SuperresDenom= coded_denom + AV1Constants.SUPERRES_DENOM_MIN;
			}
			else 
			{
				SuperresDenom= AV1Constants.SUPERRES_NUM;
			}
			UpscaledWidth= FrameWidth;
			FrameWidth= (UpscaledWidth * AV1Constants.SUPERRES_NUM + (SuperresDenom / 2)) / SuperresDenom;
         }

    /*


 compute_image_size() { 
 MiCols = 2 * ( ( FrameWidth + 7 ) >> 3 )
 MiRows = 2 * ( ( FrameHeight + 7 ) >> 3 )
 }
    */
		private int MiCols;
		private int MiRows;

         public void ReadComputeImageSize()
         {

			MiCols= 2 * ( ( FrameWidth + 7 ) >> (int)3 );
			MiRows= 2 * ( ( FrameHeight + 7 ) >> (int)3 );
         }

         public void WriteComputeImageSize()
         {

			MiCols= 2 * ( ( FrameWidth + 7 ) >> (int)3 );
			MiRows= 2 * ( ( FrameHeight + 7 ) >> (int)3 );
         }

    /*


 decode_signed_subexp_with_ref( low, high, r ) { 
 x = decode_unsigned_subexp_with_ref(high - low, r - low)
 return x + low
}
    */
		private int low;
		private int high;
		private int x;

         public int ReadDecodeSignedSubexpWithRef(int low, int high, int r)
         {

			x= ReadDecodeUnsignedSubexpWithRef(high - low, r - low);
			return x + low;
         }

         public int WriteDecodeSignedSubexpWithRef(int low, int high, int r)
         {

			x= WriteDecodeUnsignedSubexpWithRef(high - low, r - low);
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
		private int v;

         public int ReadDecodeUnsignedSubexpWithRef(int mx, int r)
         {

			v= ReadDecodeSubexp( mx );

			if ( (r << (int) 1) <= mx )
			{
				return ReadInverseRecenter(r, v);
			}
			else 
			{
				return mx - 1 - ReadInverseRecenter(mx - 1 - r, v);
			}
         }

         public int WriteDecodeUnsignedSubexpWithRef(int mx, int r)
         {

			v= WriteDecodeSubexp( mx );

			if ( (r << (int) 1) <= mx )
			{
				return WriteInverseRecenter(r, v);
			}
			else 
			{
				return mx - 1 - WriteInverseRecenter(mx - 1 - r, v);
			}
         }

    /*


decode_subexp( numSyms ) { 
 i = 0
 mk = 0
 k = 3
 while ( 1 ) {
 b2 = i != 0 ? k + i - 1 : k
 a = 1 << b2
 if ( numSyms <= mk + 3 * a ) {
 subexp_final_bits ns(numSyms - mk)
 return (int)subexp_final_bits + mk
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
		private int numSyms;
		private int mk;
		private int b2;
		private uint subexp_final_bits;
		private int subexp_more_bits;
		private int subexp_bits;

         public int ReadDecodeSubexp(int numSyms)
         {

			i= 0;
			mk= 0;
			k= 3;

			while ( 1 != 0 )
			{
				b2= i != 0 ? k + i - 1 : k;
				a= 1 << b2;

				if ( numSyms <= mk + 3 * a )
				{
					stream.Read_ns(numSyms - mk, out this.subexp_final_bits, "subexp_final_bits"); 
					return (int)subexp_final_bits + mk;
				}
				else 
				{
					stream.ReadFixed(1, out this.subexp_more_bits, "subexp_more_bits"); 

					if ( subexp_more_bits != 0 )
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

         public int WriteDecodeSubexp(int numSyms)
         {

			i= 0;
			mk= 0;
			k= 3;

			while ( 1 != 0 )
			{
				b2= i != 0 ? k + i - 1 : k;
				a= 1 << b2;

				if ( numSyms <= mk + 3 * a )
				{
					stream.Write_ns(numSyms - mk, this.subexp_final_bits, "subexp_final_bits"); 
					return (int)subexp_final_bits + mk;
				}
				else 
				{
					stream.WriteFixed(1, this.subexp_more_bits, "subexp_more_bits"); 

					if ( subexp_more_bits != 0 )
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
 else if (( v & 1 ) != 0)
    return r - ((v + 1) >> 1)
 else
    return r + (v >> 1)
 }
    */

         public int ReadInverseRecenter(int r, int v)
         {


			if ( v > 2 * r )
			{
				return v;
			}
			else if (( v & 1 ) != 0)
			{
				return r - ((v + 1) >> (int)1);
			}
			else 
			{
				return r + (v >> (int)1);
			}
         }

         public int WriteInverseRecenter(int r, int v)
         {


			if ( v > 2 * r )
			{
				return v;
			}
			else if (( v & 1 ) != 0)
			{
				return r - ((v + 1) >> (int)1);
			}
			else 
			{
				return r + (v >> (int)1);
			}
         }

    /*


temporal_delimiter_obu() { 
 SeenFrameHeader = 0
}
    */

         public void ReadTemporalDelimiterObu()
         {

			SeenFrameHeader= 0;
         }

         public void WriteTemporalDelimiterObu()
         {

			SeenFrameHeader= 0;
         }

    /*


padding_obu() { 
 for ( i = 0; i < obu_padding_length; i++ )
 obu_padding_byte f(8)
}
    */
		private int obu_padding_byte;

         public void ReadPaddingObu()
         {


			for ( i = 0; i < obu_padding_length; i++ )
			{
				stream.ReadFixed(8, out this.obu_padding_byte, "obu_padding_byte"); 
			}
         }

         public void WritePaddingObu()
         {


			for ( i = 0; i < obu_padding_length; i++ )
			{
				stream.WriteFixed(8, this.obu_padding_byte, "obu_padding_byte"); 
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
		private int metadata_type;
		private int metadata_itut_t35;
		private int metadata_hdr_cll;
		private int metadata_hdr_mdcv;
		private int metadata_scalability;
		private int metadata_timecode;

         public void ReadMetadataObu()
         {

			obu_size_len = (int)stream.ReadLeb128( out this.metadata_type, "metadata_type"); 

			if ( metadata_type == AV1MetadataType.METADATA_TYPE_ITUT_T35 )
			{
				ReadMetadataItutT35(); 
			}
			else if ( metadata_type == AV1MetadataType.METADATA_TYPE_HDR_CLL )
			{
				ReadMetadataHdrCll(); 
			}
			else if ( metadata_type == AV1MetadataType.METADATA_TYPE_HDR_MDCV )
			{
				ReadMetadataHdrMdcv(); 
			}
			else if ( metadata_type == AV1MetadataType.METADATA_TYPE_SCALABILITY )
			{
				ReadMetadataScalability(); 
			}
			else if ( metadata_type == AV1MetadataType.METADATA_TYPE_TIMECODE )
			{
				ReadMetadataTimecode(); 
			}
         }

         public void WriteMetadataObu()
         {

			stream.WriteLeb128( this.metadata_type, "metadata_type"); 

			if ( metadata_type == AV1MetadataType.METADATA_TYPE_ITUT_T35 )
			{
				WriteMetadataItutT35(); 
			}
			else if ( metadata_type == AV1MetadataType.METADATA_TYPE_HDR_CLL )
			{
				WriteMetadataHdrCll(); 
			}
			else if ( metadata_type == AV1MetadataType.METADATA_TYPE_HDR_MDCV )
			{
				WriteMetadataHdrMdcv(); 
			}
			else if ( metadata_type == AV1MetadataType.METADATA_TYPE_SCALABILITY )
			{
				WriteMetadataScalability(); 
			}
			else if ( metadata_type == AV1MetadataType.METADATA_TYPE_TIMECODE )
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
 itu_t_t35_payload_bytes()
}
    */
		private int itu_t_t35_country_code;
		private int itu_t_t35_country_code_extension_byte;
		private int itu_t_t35_payload_bytes;

         public void ReadMetadataItutT35()
         {

			stream.ReadFixed(8, out this.itu_t_t35_country_code, "itu_t_t35_country_code"); 

			if ( itu_t_t35_country_code == 0xFF )
			{
				stream.ReadFixed(8, out this.itu_t_t35_country_code_extension_byte, "itu_t_t35_country_code_extension_byte"); 
			}
			ReadItutT35PayloadBytes(); 
         }

         public void WriteMetadataItutT35()
         {

			stream.WriteFixed(8, this.itu_t_t35_country_code, "itu_t_t35_country_code"); 

			if ( itu_t_t35_country_code == 0xFF )
			{
				stream.WriteFixed(8, this.itu_t_t35_country_code_extension_byte, "itu_t_t35_country_code_extension_byte"); 
			}
			WriteItutT35PayloadBytes(); 
         }

    /*



metadata_hdr_cll() { 
 max_cll f(16)
 max_fall f(16)
}
    */
		private int max_cll;
		private int max_fall;

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
		private int[] primary_chromaticity_x;
		private int[] primary_chromaticity_y;
		private int white_point_chromaticity_x;
		private int white_point_chromaticity_y;
		private int luminance_max;
		private int luminance_min;

         public void ReadMetadataHdrMdcv()
         {


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
		private int scalability_mode_idc;
		private int scalability_structure;

         public void ReadMetadataScalability()
         {

			stream.ReadFixed(8, out this.scalability_mode_idc, "scalability_mode_idc"); 

			if ( scalability_mode_idc == AV1ScalabilityModeIdc.SCALABILITY_SS )
			{
				ReadScalabilityStructure(); 
			}
         }

         public void WriteMetadataScalability()
         {

			stream.WriteFixed(8, this.scalability_mode_idc, "scalability_mode_idc"); 

			if ( scalability_mode_idc == AV1ScalabilityModeIdc.SCALABILITY_SS )
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
		private int spatial_layers_cnt_minus_1;
		private int spatial_layer_dimensions_present_flag;
		private int spatial_layer_description_present_flag;
		private int temporal_group_description_present_flag;
		private int scalability_structure_reserved_3bits;
		private int[] spatial_layer_max_width;
		private int[] spatial_layer_max_height;
		private int[] spatial_layer_ref_id;
		private int temporal_group_size;
		private int[] temporal_group_temporal_id;
		private int[] temporal_group_temporal_switching_up_point_flag;
		private int[] temporal_group_spatial_switching_up_point_flag;
		private int[] temporal_group_ref_cnt;
		private int[][] temporal_group_ref_pic_diff;

         public void ReadScalabilityStructure()
         {

			stream.ReadFixed(2, out this.spatial_layers_cnt_minus_1, "spatial_layers_cnt_minus_1"); 
			stream.ReadFixed(1, out this.spatial_layer_dimensions_present_flag, "spatial_layer_dimensions_present_flag"); 
			stream.ReadFixed(1, out this.spatial_layer_description_present_flag, "spatial_layer_description_present_flag"); 
			stream.ReadFixed(1, out this.temporal_group_description_present_flag, "temporal_group_description_present_flag"); 
			stream.ReadFixed(3, out this.scalability_structure_reserved_3bits, "scalability_structure_reserved_3bits"); 

			if ( spatial_layer_dimensions_present_flag != 0 )
			{

				for ( i = 0; i <= spatial_layers_cnt_minus_1 ; i++ )
				{
					stream.ReadFixed(16, out this.spatial_layer_max_width[ i ], "spatial_layer_max_width"); 
					stream.ReadFixed(16, out this.spatial_layer_max_height[ i ], "spatial_layer_max_height"); 
				}
			}

			if ( spatial_layer_description_present_flag != 0 )
			{

				for ( i = 0; i <= spatial_layers_cnt_minus_1; i++ )
				{
					stream.ReadFixed(8, out this.spatial_layer_ref_id[ i ], "spatial_layer_ref_id"); 
				}
			}

			if ( temporal_group_description_present_flag != 0 )
			{
				stream.ReadFixed(8, out this.temporal_group_size, "temporal_group_size"); 

				for ( i = 0; i < temporal_group_size; i++ )
				{
					stream.ReadFixed(3, out this.temporal_group_temporal_id[ i ], "temporal_group_temporal_id"); 
					stream.ReadFixed(1, out this.temporal_group_temporal_switching_up_point_flag[ i ], "temporal_group_temporal_switching_up_point_flag"); 
					stream.ReadFixed(1, out this.temporal_group_spatial_switching_up_point_flag[ i ], "temporal_group_spatial_switching_up_point_flag"); 
					stream.ReadFixed(3, out this.temporal_group_ref_cnt[ i ], "temporal_group_ref_cnt"); 

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
		private int counting_type;
		private int full_timestamp_flag;
		private int discontinuity_flag;
		private int cnt_dropped_flag;
		private int n_frames;
		private int seconds_value;
		private int minutes_value;
		private int hours_value;
		private int seconds_flag;
		private int minutes_flag;
		private int hours_flag;
		private int time_offset_length;
		private int time_offset_value;

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
		private int startBitPos;
		private int byte_alignment;
		private int endBitPos;
		private int headerBytes;

         public void ReadFrameObu(int sz)
         {

			startBitPos= stream.GetPosition();
			ReadFrameHeaderObu(); 
			ReadByteAlignment(); 
			endBitPos= stream.GetPosition();
			headerBytes= (endBitPos - startBitPos) / 8;
			sz-= headerBytes;
			ReadTileGroupObu( sz ); 
         }

         public void WriteFrameObu(int sz)
         {

			startBitPos= stream.GetPosition();
			WriteFrameHeaderObu(); 
			WriteByteAlignment(); 
			endBitPos= stream.GetPosition();
			headerBytes= (endBitPos - startBitPos) / 8;
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
 /*for ( TileNum = tg_start; TileNum <= tg_end; TileNum++ ) {
 tileRow = TileNum / TileCols
 tileCol = TileNum % TileCols
 lastTile = (TileNum == tg_end) ? 1 : 0
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
 }*//*
 skip_obu()

 if ( tg_end == NumTiles - 1 ) {
 /* if ( !disable_frame_end_update_cdf ) {
 frame_end_update_cdf()
 } *//*
 decode_frame_wrapup()
 SeenFrameHeader = 0
 }
 }
    */
		private int NumTiles;
		private int tile_start_and_end_present_flag;
		private int tg_start;
		private int tg_end;
		private int tileBits;
		private int skip_obu;

         public void ReadTileGroupObu(int sz)
         {

			NumTiles= TileCols * TileRows;
			startBitPos= stream.GetPosition();
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
			endBitPos= stream.GetPosition();
			headerBytes= (endBitPos - startBitPos) / 8;
			sz-= headerBytes;
			ReadSkipObu(); 

			if ( tg_end == NumTiles - 1 )
			{
/*  if ( !disable_frame_end_update_cdf ) {
 frame_end_update_cdf()
 }  */

				ReadDecodeFrameWrapup(); 
				SeenFrameHeader= 0;
			}
         }

         public void WriteTileGroupObu(int sz)
         {

			NumTiles= TileCols * TileRows;
			startBitPos= stream.GetPosition();
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
			endBitPos= stream.GetPosition();
			headerBytes= (endBitPos - startBitPos) / 8;
			sz-= headerBytes;
			WriteSkipObu(); 

			if ( tg_end == NumTiles - 1 )
			{
/*  if ( !disable_frame_end_update_cdf ) {
 frame_end_update_cdf()
 }  */

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
		private int output_frame_width_in_tiles_minus_1;
		private int output_frame_height_in_tiles_minus_1;
		private int tile_count_minus_1;
		private int tile_list_entry;

			int tile = 0;
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
		private int anchor_frame_idx;
		private int anchor_tile_row;
		private int anchor_tile_col;
		private int tile_data_size_minus_1;
		private int N;
		private byte[] coded_tile_data;

         public void ReadTileListEntry()
         {

			stream.ReadFixed(8, out this.anchor_frame_idx, "anchor_frame_idx"); 
			stream.ReadFixed(8, out this.anchor_tile_row, "anchor_tile_row"); 
			stream.ReadFixed(8, out this.anchor_tile_col, "anchor_tile_col"); 
			stream.ReadFixed(16, out this.tile_data_size_minus_1, "tile_data_size_minus_1"); 
			N= 8 * (tile_data_size_minus_1 + 1);
			stream.ReadBytes(N, out this.coded_tile_data, "coded_tile_data"); 
         }

         public void WriteTileListEntry()
         {

			stream.WriteFixed(8, this.anchor_frame_idx, "anchor_frame_idx"); 
			stream.WriteFixed(8, this.anchor_tile_row, "anchor_tile_row"); 
			stream.WriteFixed(8, this.anchor_tile_col, "anchor_tile_col"); 
			stream.WriteFixed(16, this.tile_data_size_minus_1, "tile_data_size_minus_1"); 
			N= 8 * (tile_data_size_minus_1 + 1);
			stream.WriteBytes(N, this.coded_tile_data, "coded_tile_data"); 
         }

        }
    }
