using System;
using System.Collections.Generic;
using System.Numerics;

namespace SharpAV1
{

    public partial class AV1Context : IAomContext
    {
        public ObuHeader ObuHeader { get; set; }
		public ReservedObu ReservedObu { get; set; }
		public SequenceHeaderObu SequenceHeaderObu { get; set; }
		public TemporalDelimiterObu TemporalDelimiterObu { get; set; }
		public PaddingObu PaddingObu { get; set; }
		public MetadataObu MetadataObu { get; set; }
		public FrameHeaderObu FrameHeaderObu { get; set; }
		public FrameObu FrameObu { get; set; }
		public TileGroupObu TileGroupObu { get; set; }
		public TileListObu TileListObu { get; set; }

    }

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
    public class OpenBitstreamUnit : IAomSerializable
    {
		private uint sz;
		public uint Sz { get { return sz; } set { sz = value; } }
		private ObuHeader obu_header;
		public ObuHeader ObuHeader { get { return obu_header; } set { obu_header = value; } }
		private uint obu_size;
		public uint ObuSize { get { return obu_size; } set { obu_size = value; } }
		private DropObu drop_obu;
		public DropObu DropObu { get { return drop_obu; } set { drop_obu = value; } }
		private SequenceHeaderObu sequence_header_obu;
		public SequenceHeaderObu SequenceHeaderObu { get { return sequence_header_obu; } set { sequence_header_obu = value; } }
		private TemporalDelimiterObu temporal_delimiter_obu;
		public TemporalDelimiterObu TemporalDelimiterObu { get { return temporal_delimiter_obu; } set { temporal_delimiter_obu = value; } }
		private FrameHeaderObu frame_header_obu;
		public FrameHeaderObu FrameHeaderObu { get { return frame_header_obu; } set { frame_header_obu = value; } }
		private TileGroupObu tile_group_obu;
		public TileGroupObu TileGroupObu { get { return tile_group_obu; } set { tile_group_obu = value; } }
		private MetadataObu metadata_obu;
		public MetadataObu MetadataObu { get { return metadata_obu; } set { metadata_obu = value; } }
		private FrameObu frame_obu;
		public FrameObu FrameObu { get { return frame_obu; } set { frame_obu = value; } }
		private TileListObu tile_list_obu;
		public TileListObu TileListObu { get { return tile_list_obu; } set { tile_list_obu = value; } }
		private PaddingObu padding_obu;
		public PaddingObu PaddingObu { get { return padding_obu; } set { padding_obu = value; } }
		private ReservedObu reserved_obu;
		public ReservedObu ReservedObu { get { return reserved_obu; } set { reserved_obu = value; } }
		private TrailingBits trailing_bits;
		public TrailingBits TrailingBits { get { return trailing_bits; } set { trailing_bits = value; } }

         public OpenBitstreamUnit(uint sz)
         { 
			this.sz = sz;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint obu_size = 0;
			uint startPosition = 0;
			uint inTemporalLayer = 0;
			uint inSpatialLayer = 0;
			uint currentPosition = 0;
			uint payloadBits = 0;
			this.obu_header =  new ObuHeader() ;
			size +=  stream.ReadClass<ObuHeader>(size, context, this.obu_header, "obu_header"); 

			if ( obu_has_size_field != 0 )
			{
				size += stream.ReadLeb128(size,  out this.obu_size, "obu_size"); 
			}
			else 
			{
				obu_size= sz - 1 - obu_extension_flag;
			}
			startPosition= get_position();

			if ( obu_type != OBU_SEQUENCE_HEADER && obu_type != OBU_TEMPORAL_DELIMITER && OperatingPointIdc != 0 && obu_extension_flag == 1 )
			{
				inTemporalLayer= (OperatingPointIdc >> temporal_id ) & 1;
				inSpatialLayer= (OperatingPointIdc >> ( spatial_id + 8 ) ) & 1;

				if ( inTemporalLayer== 0 ||  inSpatialLayer== 0 )
				{
					this.drop_obu =  new DropObu() ;
					size +=  stream.ReadClass<DropObu>(size, context, this.drop_obu, "drop_obu"); 
return;
				}
			}

			if ( obu_type == OBU_SEQUENCE_HEADER )
			{
				this.sequence_header_obu =  new SequenceHeaderObu() ;
				size +=  stream.ReadClass<SequenceHeaderObu>(size, context, this.sequence_header_obu, "sequence_header_obu"); 
			}
			else if ( obu_type == OBU_TEMPORAL_DELIMITER )
			{
				this.temporal_delimiter_obu =  new TemporalDelimiterObu() ;
				size +=  stream.ReadClass<TemporalDelimiterObu>(size, context, this.temporal_delimiter_obu, "temporal_delimiter_obu"); 
			}
			else if ( obu_type == OBU_FRAME_HEADER )
			{
				this.frame_header_obu =  new FrameHeaderObu() ;
				size +=  stream.ReadClass<FrameHeaderObu>(size, context, this.frame_header_obu, "frame_header_obu"); 
			}
			else if ( obu_type == OBU_REDUNDANT_FRAME_HEADER )
			{
				this.frame_header_obu =  new FrameHeaderObu() ;
				size +=  stream.ReadClass<FrameHeaderObu>(size, context, this.frame_header_obu, "frame_header_obu"); 
			}
			else if ( obu_type == OBU_TILE_GROUP )
			{
				this.tile_group_obu =  new TileGroupObu( obu_size ) ;
				size +=  stream.ReadClass<TileGroupObu>(size, context, this.tile_group_obu, "tile_group_obu"); 
			}
			else if ( obu_type == OBU_METADATA )
			{
				this.metadata_obu =  new MetadataObu() ;
				size +=  stream.ReadClass<MetadataObu>(size, context, this.metadata_obu, "metadata_obu"); 
			}
			else if ( obu_type == OBU_FRAME )
			{
				this.frame_obu =  new FrameObu( obu_size ) ;
				size +=  stream.ReadClass<FrameObu>(size, context, this.frame_obu, "frame_obu"); 
			}
			else if ( obu_type == OBU_TILE_LIST )
			{
				this.tile_list_obu =  new TileListObu() ;
				size +=  stream.ReadClass<TileListObu>(size, context, this.tile_list_obu, "tile_list_obu"); 
			}
			else if ( obu_type == OBU_PADDING )
			{
				this.padding_obu =  new PaddingObu() ;
				size +=  stream.ReadClass<PaddingObu>(size, context, this.padding_obu, "padding_obu"); 
			}
			else 
			{
				this.reserved_obu =  new ReservedObu() ;
				size +=  stream.ReadClass<ReservedObu>(size, context, this.reserved_obu, "reserved_obu"); 
			}
			currentPosition= get_position();
			payloadBits= currentPosition - startPosition;

			if ( obu_size > 0 && obu_type != OBU_TILE_GROUP &&
    obu_type != OBU_TILE_LIST &&
    obu_type != OBU_FRAME )
			{
				this.trailing_bits =  new TrailingBits( obu_size * 8 - payloadBits ) ;
				size +=  stream.ReadClass<TrailingBits>(size, context, this.trailing_bits, "trailing_bits"); 
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint obu_size = 0;
			uint startPosition = 0;
			uint inTemporalLayer = 0;
			uint inSpatialLayer = 0;
			uint currentPosition = 0;
			uint payloadBits = 0;
			size += stream.WriteClass<ObuHeader>(context, this.obu_header, "obu_header"); 

			if ( obu_has_size_field != 0 )
			{
				size += stream.WriteLeb128( this.obu_size, "obu_size"); 
			}
			else 
			{
				obu_size= sz - 1 - obu_extension_flag;
			}
			startPosition= get_position();

			if ( obu_type != OBU_SEQUENCE_HEADER && obu_type != OBU_TEMPORAL_DELIMITER && OperatingPointIdc != 0 && obu_extension_flag == 1 )
			{
				inTemporalLayer= (OperatingPointIdc >> temporal_id ) & 1;
				inSpatialLayer= (OperatingPointIdc >> ( spatial_id + 8 ) ) & 1;

				if ( inTemporalLayer== 0 ||  inSpatialLayer== 0 )
				{
					size += stream.WriteClass<DropObu>(context, this.drop_obu, "drop_obu"); 
return;
				}
			}

			if ( obu_type == OBU_SEQUENCE_HEADER )
			{
				size += stream.WriteClass<SequenceHeaderObu>(context, this.sequence_header_obu, "sequence_header_obu"); 
			}
			else if ( obu_type == OBU_TEMPORAL_DELIMITER )
			{
				size += stream.WriteClass<TemporalDelimiterObu>(context, this.temporal_delimiter_obu, "temporal_delimiter_obu"); 
			}
			else if ( obu_type == OBU_FRAME_HEADER )
			{
				size += stream.WriteClass<FrameHeaderObu>(context, this.frame_header_obu, "frame_header_obu"); 
			}
			else if ( obu_type == OBU_REDUNDANT_FRAME_HEADER )
			{
				size += stream.WriteClass<FrameHeaderObu>(context, this.frame_header_obu, "frame_header_obu"); 
			}
			else if ( obu_type == OBU_TILE_GROUP )
			{
				size += stream.WriteClass<TileGroupObu>(context, this.tile_group_obu, "tile_group_obu"); 
			}
			else if ( obu_type == OBU_METADATA )
			{
				size += stream.WriteClass<MetadataObu>(context, this.metadata_obu, "metadata_obu"); 
			}
			else if ( obu_type == OBU_FRAME )
			{
				size += stream.WriteClass<FrameObu>(context, this.frame_obu, "frame_obu"); 
			}
			else if ( obu_type == OBU_TILE_LIST )
			{
				size += stream.WriteClass<TileListObu>(context, this.tile_list_obu, "tile_list_obu"); 
			}
			else if ( obu_type == OBU_PADDING )
			{
				size += stream.WriteClass<PaddingObu>(context, this.padding_obu, "padding_obu"); 
			}
			else 
			{
				size += stream.WriteClass<ReservedObu>(context, this.reserved_obu, "reserved_obu"); 
			}
			currentPosition= get_position();
			payloadBits= currentPosition - startPosition;

			if ( obu_size > 0 && obu_type != OBU_TILE_GROUP &&
    obu_type != OBU_TILE_LIST &&
    obu_type != OBU_FRAME )
			{
				size += stream.WriteClass<TrailingBits>(context, this.trailing_bits, "trailing_bits"); 
			}

            return size;
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
    public class ObuHeader : IAomSerializable
    {
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
		private ObuExtensionHeader obu_extension_header;
		public ObuExtensionHeader ObuExtensionHeader { get { return obu_extension_header; } set { obu_extension_header = value; } }

         public ObuHeader()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			size += stream.ReadFixed(size, 1, out this.obu_forbidden_bit, "obu_forbidden_bit"); 
			size += stream.ReadFixed(size, 4, out this.obu_type, "obu_type"); 
			size += stream.ReadFixed(size, 1, out this.obu_extension_flag, "obu_extension_flag"); 
			size += stream.ReadFixed(size, 1, out this.obu_has_size_field, "obu_has_size_field"); 
			size += stream.ReadFixed(size, 1, out this.obu_reserved_1bit, "obu_reserved_1bit"); 

			if ( obu_extension_flag == 1 )
			{
				this.obu_extension_header =  new ObuExtensionHeader() ;
				size +=  stream.ReadClass<ObuExtensionHeader>(size, context, this.obu_extension_header, "obu_extension_header"); 
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			size += stream.WriteFixed(1, this.obu_forbidden_bit, "obu_forbidden_bit"); 
			size += stream.WriteFixed(4, this.obu_type, "obu_type"); 
			size += stream.WriteFixed(1, this.obu_extension_flag, "obu_extension_flag"); 
			size += stream.WriteFixed(1, this.obu_has_size_field, "obu_has_size_field"); 
			size += stream.WriteFixed(1, this.obu_reserved_1bit, "obu_reserved_1bit"); 

			if ( obu_extension_flag == 1 )
			{
				size += stream.WriteClass<ObuExtensionHeader>(context, this.obu_extension_header, "obu_extension_header"); 
			}

            return size;
         }

    }

    /*



 obu_extension_header() { 
 temporal_id f(3)
 spatial_id f(2)
 extension_header_reserved_3bits f(3)
 }
    */
    public class ObuExtensionHeader : IAomSerializable
    {
		private uint temporal_id;
		public uint TemporalId { get { return temporal_id; } set { temporal_id = value; } }
		private uint spatial_id;
		public uint SpatialId { get { return spatial_id; } set { spatial_id = value; } }
		private uint extension_header_reserved_3bits;
		public uint ExtensionHeaderReserved3bits { get { return extension_header_reserved_3bits; } set { extension_header_reserved_3bits = value; } }

         public ObuExtensionHeader()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			size += stream.ReadFixed(size, 3, out this.temporal_id, "temporal_id"); 
			size += stream.ReadFixed(size, 2, out this.spatial_id, "spatial_id"); 
			size += stream.ReadFixed(size, 3, out this.extension_header_reserved_3bits, "extension_header_reserved_3bits"); 

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			size += stream.WriteFixed(3, this.temporal_id, "temporal_id"); 
			size += stream.WriteFixed(2, this.spatial_id, "spatial_id"); 
			size += stream.WriteFixed(3, this.extension_header_reserved_3bits, "extension_header_reserved_3bits"); 

            return size;
         }

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
    public class TrailingBits : IAomSerializable
    {
		private uint nbBits;
		public uint NbBits { get { return nbBits; } set { nbBits = value; } }
		private uint trailing_one_bit;
		public uint TrailingOneBit { get { return trailing_one_bit; } set { trailing_one_bit = value; } }
		private Dictionary<int, uint> trailing_zero_bit = new Dictionary<int, uint>();
		public Dictionary<int, uint> TrailingZeroBit { get { return trailing_zero_bit; } set { trailing_zero_bit = value; } }

         public TrailingBits(uint nbBits)
         { 
			this.nbBits = nbBits;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			int whileIndex = -1;
			size += stream.ReadFixed(size, 1, out this.trailing_one_bit, "trailing_one_bit"); 
			nbBits--;

			while ( nbBits > 0 )
			{
				whileIndex++;

				size += stream.ReadFixed(size, 1, whileIndex, this.trailing_zero_bit, "trailing_zero_bit"); 
				nbBits--;
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			int whileIndex = -1;
			size += stream.WriteFixed(1, this.trailing_one_bit, "trailing_one_bit"); 
			nbBits--;

			while ( nbBits > 0 )
			{
				whileIndex++;

				size += stream.WriteFixed(1, whileIndex, this.trailing_zero_bit, "trailing_zero_bit"); 
				nbBits--;
			}

            return size;
         }

    }

    /*



byte_alignment() { 
 while ( get_position() & 7 )
 zero_bit f(1)
}
    */
    public class ByteAlignment : IAomSerializable
    {
		private Dictionary<int, uint> zero_bit = new Dictionary<int, uint>();
		public Dictionary<int, uint> ZeroBit { get { return zero_bit; } set { zero_bit = value; } }

         public ByteAlignment()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			int whileIndex = -1;

			while ( get_position() & 7 )
			{
				whileIndex++;

				size += stream.ReadFixed(size, 1, whileIndex, this.zero_bit, "zero_bit"); 
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			int whileIndex = -1;

			while ( get_position() & 7 )
			{
				whileIndex++;

				size += stream.WriteFixed(1, whileIndex, this.zero_bit, "zero_bit"); 
			}

            return size;
         }

    }

    /*



reserved_obu() { 
}
    */
    public class ReservedObu : IAomSerializable
    {

         public ReservedObu()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;


            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;


            return size;
         }

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
    public class SequenceHeaderObu : IAomSerializable
    {
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
		private TimingInfo timing_info;
		public TimingInfo TimingInfo { get { return timing_info; } set { timing_info = value; } }
		private uint decoder_model_info_present_flag;
		public uint DecoderModelInfoPresentFlag { get { return decoder_model_info_present_flag; } set { decoder_model_info_present_flag = value; } }
		private DecoderModelInfo decoder_model_info;
		public DecoderModelInfo DecoderModelInfo { get { return decoder_model_info; } set { decoder_model_info = value; } }
		private uint initial_display_delay_present_flag;
		public uint InitialDisplayDelayPresentFlag { get { return initial_display_delay_present_flag; } set { initial_display_delay_present_flag = value; } }
		private uint operating_points_cnt_minus_1;
		public uint OperatingPointsCntMinus1 { get { return operating_points_cnt_minus_1; } set { operating_points_cnt_minus_1 = value; } }
		private OperatingPointIdc operating_point_idc;
		public OperatingPointIdc OperatingPointIdc { get { return operating_point_idc; } set { operating_point_idc = value; } }
		private uint[] seq_tier;
		public uint[] SeqTier { get { return seq_tier; } set { seq_tier = value; } }
		private uint[] decoder_model_present_for_this_op;
		public uint[] DecoderModelPresentForThisOp { get { return decoder_model_present_for_this_op; } set { decoder_model_present_for_this_op = value; } }
		private OperatingParametersInfo[] operating_parameters_info;
		public OperatingParametersInfo[] OperatingParametersInfo { get { return operating_parameters_info; } set { operating_parameters_info = value; } }
		private uint[] initial_display_delay_present_for_this_op;
		public uint[] InitialDisplayDelayPresentForThisOp { get { return initial_display_delay_present_for_this_op; } set { initial_display_delay_present_for_this_op = value; } }
		private InitialDisplayDelayMinus1 initial_display_delay_minus_1;
		public InitialDisplayDelayMinus1 InitialDisplayDelayMinus1 { get { return initial_display_delay_minus_1; } set { initial_display_delay_minus_1 = value; } }
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
		private ColorConfig color_config;
		public ColorConfig ColorConfig { get { return color_config; } set { color_config = value; } }
		private uint film_grain_params_present;
		public uint FilmGrainParamsPresent { get { return film_grain_params_present; } set { film_grain_params_present = value; } }

         public SequenceHeaderObu()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint timing_info_present_flag = 0;
			uint decoder_model_info_present_flag = 0;
			uint initial_display_delay_present_flag = 0;
			uint operating_points_cnt_minus_1 = 0;
			uint[][] operating_point_idc = null;
			uint[][] seq_tier = null;
			uint[][] decoder_model_present_for_this_op = null;
			uint[][] initial_display_delay_present_for_this_op = null;
			uint i = 0;
			uint operatingPoint = 0;
			uint OperatingPointIdc = 0;
			uint n = 0;
			uint frame_id_numbers_present_flag = 0;
			uint enable_interintra_compound = 0;
			uint enable_masked_compound = 0;
			uint enable_warped_motion = 0;
			uint enable_dual_filter = 0;
			uint enable_order_hint = 0;
			uint enable_jnt_comp = 0;
			uint enable_ref_frame_mvs = 0;
			uint seq_force_screen_content_tools = 0;
			uint seq_force_integer_mv = 0;
			uint OrderHintBits = 0;
			size += stream.ReadFixed(size, 3, out this.seq_profile, "seq_profile"); 
			size += stream.ReadFixed(size, 1, out this.still_picture, "still_picture"); 
			size += stream.ReadFixed(size, 1, out this.reduced_still_picture_header, "reduced_still_picture_header"); 

			if ( reduced_still_picture_header != 0 )
			{
				timing_info_present_flag= 0;
				decoder_model_info_present_flag= 0;
				initial_display_delay_present_flag= 0;
				operating_points_cnt_minus_1= 0;
				operating_point_idc[ 0 ]= 0;
				size += stream.ReadFixed(size, 5, out this.seq_level_idx[ 0 ], "seq_level_idx"); 
				seq_tier[ 0 ]= 0;
				decoder_model_present_for_this_op[ 0 ]= 0;
				initial_display_delay_present_for_this_op[ 0 ]= 0;
			}
			else 
			{
				size += stream.ReadFixed(size, 1, out this.timing_info_present_flag, "timing_info_present_flag"); 

				if ( timing_info_present_flag != 0 )
				{
					this.timing_info =  new TimingInfo() ;
					size +=  stream.ReadClass<TimingInfo>(size, context, this.timing_info, "timing_info"); 
					size += stream.ReadFixed(size, 1, out this.decoder_model_info_present_flag, "decoder_model_info_present_flag"); 

					if ( decoder_model_info_present_flag != 0 )
					{
						this.decoder_model_info =  new DecoderModelInfo() ;
						size +=  stream.ReadClass<DecoderModelInfo>(size, context, this.decoder_model_info, "decoder_model_info"); 
					}
				}
				else 
				{
					decoder_model_info_present_flag= 0;
				}
				size += stream.ReadFixed(size, 1, out this.initial_display_delay_present_flag, "initial_display_delay_present_flag"); 
				size += stream.ReadFixed(size, 5, out this.operating_points_cnt_minus_1, "operating_points_cnt_minus_1"); 

				this.operating_point_idc = new OperatingPointIdc[ operating_points_cnt_minus_1];
				this.seq_tier = new uint[ operating_points_cnt_minus_1];
				this.decoder_model_present_for_this_op = new uint[ operating_points_cnt_minus_1];
				this.operating_parameters_info = new OperatingParametersInfo[ operating_points_cnt_minus_1];
				this.initial_display_delay_present_for_this_op = new uint[ operating_points_cnt_minus_1];
				this.initial_display_delay_minus_1 = new InitialDisplayDelayMinus1[ operating_points_cnt_minus_1];
				for ( i = 0; i <= operating_points_cnt_minus_1; i++ )
				{
					this.operating_point_idc[ i ] =  new OperatingPointIdc() ;
					size +=  stream.ReadClass<OperatingPointIdc>(size, context, this.operating_point_idc[ i ], "operating_point_idc"); 
					size += stream.ReadFixed(size, 12, out this.operating_point_idc[ i ], "operating_point_idc"); 
					size += stream.ReadFixed(size, 5, out this.seq_level_idx[ i ], "seq_level_idx"); 

					if ( seq_level_idx[ i ] > 7 )
					{
						size += stream.ReadFixed(size, 1, out this.seq_tier[ i ], "seq_tier"); 
					}
					else 
					{
						seq_tier[ i ]= 0;
					}

					if ( decoder_model_info_present_flag != 0 )
					{
						size += stream.ReadFixed(size, 1, out this.decoder_model_present_for_this_op[ i ], "decoder_model_present_for_this_op"); 

						if ( decoder_model_present_for_this_op[ i ] != 0 )
						{
							this.operating_parameters_info[ i ] =  new OperatingParametersInfo( i ) ;
							size +=  stream.ReadClass<OperatingParametersInfo>(size, context, this.operating_parameters_info[ i ], "operating_parameters_info"); 
						}
					}
					else 
					{
						decoder_model_present_for_this_op[ i ]= 0;
					}

					if ( initial_display_delay_present_flag != 0 )
					{
						size += stream.ReadFixed(size, 1, out this.initial_display_delay_present_for_this_op[ i ], "initial_display_delay_present_for_this_op"); 

						if ( initial_display_delay_present_for_this_op[ i ] != 0 )
						{
							this.initial_display_delay_minus_1[ i ] =  new InitialDisplayDelayMinus1() ;
							size +=  stream.ReadClass<InitialDisplayDelayMinus1>(size, context, this.initial_display_delay_minus_1[ i ], "initial_display_delay_minus_1"); 
							size += stream.ReadFixed(size, 4, out this.initial_display_delay_minus_1[ i ], "initial_display_delay_minus_1"); 
						}
					}
				}
			}
			operatingPoint= choose_operating_point();
			OperatingPointIdc= operating_point_idc[ operatingPoint ];
			size += stream.ReadFixed(size, 4, out this.frame_width_bits_minus_1, "frame_width_bits_minus_1"); 
			size += stream.ReadFixed(size, 4, out this.frame_height_bits_minus_1, "frame_height_bits_minus_1"); 
			n= frame_width_bits_minus_1 + 1;
			size += stream.ReadVariable(size, n, out this.max_frame_width_minus_1, "max_frame_width_minus_1"); 
			n= frame_height_bits_minus_1 + 1;
			size += stream.ReadVariable(size, n, out this.max_frame_height_minus_1, "max_frame_height_minus_1"); 

			if ( reduced_still_picture_header != 0 )
			{
				frame_id_numbers_present_flag= 0;
			}
			else 
			{
				size += stream.ReadFixed(size, 1, out this.frame_id_numbers_present_flag, "frame_id_numbers_present_flag"); 
			}

			if ( frame_id_numbers_present_flag != 0 )
			{
				size += stream.ReadFixed(size, 4, out this.delta_frame_id_length_minus_2, "delta_frame_id_length_minus_2"); 
				size += stream.ReadFixed(size, 3, out this.additional_frame_id_length_minus_1, "additional_frame_id_length_minus_1"); 
			}
			size += stream.ReadFixed(size, 1, out this.use_128x128_superblock, "use_128x128_superblock"); 
			size += stream.ReadFixed(size, 1, out this.enable_filter_intra, "enable_filter_intra"); 
			size += stream.ReadFixed(size, 1, out this.enable_intra_edge_filter, "enable_intra_edge_filter"); 

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
				size += stream.ReadFixed(size, 1, out this.enable_interintra_compound, "enable_interintra_compound"); 
				size += stream.ReadFixed(size, 1, out this.enable_masked_compound, "enable_masked_compound"); 
				size += stream.ReadFixed(size, 1, out this.enable_warped_motion, "enable_warped_motion"); 
				size += stream.ReadFixed(size, 1, out this.enable_dual_filter, "enable_dual_filter"); 
				size += stream.ReadFixed(size, 1, out this.enable_order_hint, "enable_order_hint"); 

				if ( enable_order_hint != 0 )
				{
					size += stream.ReadFixed(size, 1, out this.enable_jnt_comp, "enable_jnt_comp"); 
					size += stream.ReadFixed(size, 1, out this.enable_ref_frame_mvs, "enable_ref_frame_mvs"); 
				}
				else 
				{
					enable_jnt_comp= 0;
					enable_ref_frame_mvs= 0;
				}
				size += stream.ReadFixed(size, 1, out this.seq_choose_screen_content_tools, "seq_choose_screen_content_tools"); 

				if ( seq_choose_screen_content_tools != 0 )
				{
					seq_force_screen_content_tools= SELECT_SCREEN_CONTENT_TOOLS;
				}
				else 
				{
					size += stream.ReadFixed(size, 1, out this.seq_force_screen_content_tools, "seq_force_screen_content_tools"); 
				}

				if ( seq_force_screen_content_tools > 0 )
				{
					size += stream.ReadFixed(size, 1, out this.seq_choose_integer_mv, "seq_choose_integer_mv"); 

					if ( seq_choose_integer_mv != 0 )
					{
						seq_force_integer_mv= SELECT_INTEGER_MV;
					}
					else 
					{
						size += stream.ReadFixed(size, 1, out this.seq_force_integer_mv, "seq_force_integer_mv"); 
					}
				}
				else 
				{
					seq_force_integer_mv= SELECT_INTEGER_MV;
				}

				if ( enable_order_hint != 0 )
				{
					size += stream.ReadFixed(size, 3, out this.order_hint_bits_minus_1, "order_hint_bits_minus_1"); 
					OrderHintBits= order_hint_bits_minus_1 + 1;
				}
				else 
				{
					OrderHintBits= 0;
				}
			}
			size += stream.ReadFixed(size, 1, out this.enable_superres, "enable_superres"); 
			size += stream.ReadFixed(size, 1, out this.enable_cdef, "enable_cdef"); 
			size += stream.ReadFixed(size, 1, out this.enable_restoration, "enable_restoration"); 
			this.color_config =  new ColorConfig() ;
			size +=  stream.ReadClass<ColorConfig>(size, context, this.color_config, "color_config"); 
			size += stream.ReadFixed(size, 1, out this.film_grain_params_present, "film_grain_params_present"); 

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint timing_info_present_flag = 0;
			uint decoder_model_info_present_flag = 0;
			uint initial_display_delay_present_flag = 0;
			uint operating_points_cnt_minus_1 = 0;
			uint[][] operating_point_idc = null;
			uint[][] seq_tier = null;
			uint[][] decoder_model_present_for_this_op = null;
			uint[][] initial_display_delay_present_for_this_op = null;
			uint i = 0;
			uint operatingPoint = 0;
			uint OperatingPointIdc = 0;
			uint n = 0;
			uint frame_id_numbers_present_flag = 0;
			uint enable_interintra_compound = 0;
			uint enable_masked_compound = 0;
			uint enable_warped_motion = 0;
			uint enable_dual_filter = 0;
			uint enable_order_hint = 0;
			uint enable_jnt_comp = 0;
			uint enable_ref_frame_mvs = 0;
			uint seq_force_screen_content_tools = 0;
			uint seq_force_integer_mv = 0;
			uint OrderHintBits = 0;
			size += stream.WriteFixed(3, this.seq_profile, "seq_profile"); 
			size += stream.WriteFixed(1, this.still_picture, "still_picture"); 
			size += stream.WriteFixed(1, this.reduced_still_picture_header, "reduced_still_picture_header"); 

			if ( reduced_still_picture_header != 0 )
			{
				timing_info_present_flag= 0;
				decoder_model_info_present_flag= 0;
				initial_display_delay_present_flag= 0;
				operating_points_cnt_minus_1= 0;
				operating_point_idc[ 0 ]= 0;
				size += stream.WriteFixed(5, this.seq_level_idx[ 0 ], "seq_level_idx"); 
				seq_tier[ 0 ]= 0;
				decoder_model_present_for_this_op[ 0 ]= 0;
				initial_display_delay_present_for_this_op[ 0 ]= 0;
			}
			else 
			{
				size += stream.WriteFixed(1, this.timing_info_present_flag, "timing_info_present_flag"); 

				if ( timing_info_present_flag != 0 )
				{
					size += stream.WriteClass<TimingInfo>(context, this.timing_info, "timing_info"); 
					size += stream.WriteFixed(1, this.decoder_model_info_present_flag, "decoder_model_info_present_flag"); 

					if ( decoder_model_info_present_flag != 0 )
					{
						size += stream.WriteClass<DecoderModelInfo>(context, this.decoder_model_info, "decoder_model_info"); 
					}
				}
				else 
				{
					decoder_model_info_present_flag= 0;
				}
				size += stream.WriteFixed(1, this.initial_display_delay_present_flag, "initial_display_delay_present_flag"); 
				size += stream.WriteFixed(5, this.operating_points_cnt_minus_1, "operating_points_cnt_minus_1"); 

				for ( i = 0; i <= operating_points_cnt_minus_1; i++ )
				{
					size += stream.WriteClass<OperatingPointIdc>(context, this.operating_point_idc[ i ], "operating_point_idc"); 
					size += stream.WriteFixed(12, this.operating_point_idc[ i ], "operating_point_idc"); 
					size += stream.WriteFixed(5, this.seq_level_idx[ i ], "seq_level_idx"); 

					if ( seq_level_idx[ i ] > 7 )
					{
						size += stream.WriteFixed(1, this.seq_tier[ i ], "seq_tier"); 
					}
					else 
					{
						seq_tier[ i ]= 0;
					}

					if ( decoder_model_info_present_flag != 0 )
					{
						size += stream.WriteFixed(1, this.decoder_model_present_for_this_op[ i ], "decoder_model_present_for_this_op"); 

						if ( decoder_model_present_for_this_op[ i ] != 0 )
						{
							size += stream.WriteClass<OperatingParametersInfo>(context, this.operating_parameters_info[ i ], "operating_parameters_info"); 
						}
					}
					else 
					{
						decoder_model_present_for_this_op[ i ]= 0;
					}

					if ( initial_display_delay_present_flag != 0 )
					{
						size += stream.WriteFixed(1, this.initial_display_delay_present_for_this_op[ i ], "initial_display_delay_present_for_this_op"); 

						if ( initial_display_delay_present_for_this_op[ i ] != 0 )
						{
							size += stream.WriteClass<InitialDisplayDelayMinus1>(context, this.initial_display_delay_minus_1[ i ], "initial_display_delay_minus_1"); 
							size += stream.WriteFixed(4, this.initial_display_delay_minus_1[ i ], "initial_display_delay_minus_1"); 
						}
					}
				}
			}
			operatingPoint= choose_operating_point();
			OperatingPointIdc= operating_point_idc[ operatingPoint ];
			size += stream.WriteFixed(4, this.frame_width_bits_minus_1, "frame_width_bits_minus_1"); 
			size += stream.WriteFixed(4, this.frame_height_bits_minus_1, "frame_height_bits_minus_1"); 
			n= frame_width_bits_minus_1 + 1;
			size += stream.WriteVariable(n, this.max_frame_width_minus_1, "max_frame_width_minus_1"); 
			n= frame_height_bits_minus_1 + 1;
			size += stream.WriteVariable(n, this.max_frame_height_minus_1, "max_frame_height_minus_1"); 

			if ( reduced_still_picture_header != 0 )
			{
				frame_id_numbers_present_flag= 0;
			}
			else 
			{
				size += stream.WriteFixed(1, this.frame_id_numbers_present_flag, "frame_id_numbers_present_flag"); 
			}

			if ( frame_id_numbers_present_flag != 0 )
			{
				size += stream.WriteFixed(4, this.delta_frame_id_length_minus_2, "delta_frame_id_length_minus_2"); 
				size += stream.WriteFixed(3, this.additional_frame_id_length_minus_1, "additional_frame_id_length_minus_1"); 
			}
			size += stream.WriteFixed(1, this.use_128x128_superblock, "use_128x128_superblock"); 
			size += stream.WriteFixed(1, this.enable_filter_intra, "enable_filter_intra"); 
			size += stream.WriteFixed(1, this.enable_intra_edge_filter, "enable_intra_edge_filter"); 

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
				size += stream.WriteFixed(1, this.enable_interintra_compound, "enable_interintra_compound"); 
				size += stream.WriteFixed(1, this.enable_masked_compound, "enable_masked_compound"); 
				size += stream.WriteFixed(1, this.enable_warped_motion, "enable_warped_motion"); 
				size += stream.WriteFixed(1, this.enable_dual_filter, "enable_dual_filter"); 
				size += stream.WriteFixed(1, this.enable_order_hint, "enable_order_hint"); 

				if ( enable_order_hint != 0 )
				{
					size += stream.WriteFixed(1, this.enable_jnt_comp, "enable_jnt_comp"); 
					size += stream.WriteFixed(1, this.enable_ref_frame_mvs, "enable_ref_frame_mvs"); 
				}
				else 
				{
					enable_jnt_comp= 0;
					enable_ref_frame_mvs= 0;
				}
				size += stream.WriteFixed(1, this.seq_choose_screen_content_tools, "seq_choose_screen_content_tools"); 

				if ( seq_choose_screen_content_tools != 0 )
				{
					seq_force_screen_content_tools= SELECT_SCREEN_CONTENT_TOOLS;
				}
				else 
				{
					size += stream.WriteFixed(1, this.seq_force_screen_content_tools, "seq_force_screen_content_tools"); 
				}

				if ( seq_force_screen_content_tools > 0 )
				{
					size += stream.WriteFixed(1, this.seq_choose_integer_mv, "seq_choose_integer_mv"); 

					if ( seq_choose_integer_mv != 0 )
					{
						seq_force_integer_mv= SELECT_INTEGER_MV;
					}
					else 
					{
						size += stream.WriteFixed(1, this.seq_force_integer_mv, "seq_force_integer_mv"); 
					}
				}
				else 
				{
					seq_force_integer_mv= SELECT_INTEGER_MV;
				}

				if ( enable_order_hint != 0 )
				{
					size += stream.WriteFixed(3, this.order_hint_bits_minus_1, "order_hint_bits_minus_1"); 
					OrderHintBits= order_hint_bits_minus_1 + 1;
				}
				else 
				{
					OrderHintBits= 0;
				}
			}
			size += stream.WriteFixed(1, this.enable_superres, "enable_superres"); 
			size += stream.WriteFixed(1, this.enable_cdef, "enable_cdef"); 
			size += stream.WriteFixed(1, this.enable_restoration, "enable_restoration"); 
			size += stream.WriteClass<ColorConfig>(context, this.color_config, "color_config"); 
			size += stream.WriteFixed(1, this.film_grain_params_present, "film_grain_params_present"); 

            return size;
         }

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
    public class ColorConfig : IAomSerializable
    {
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

         public ColorConfig()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint BitDepth = 0;
			uint mono_chrome = 0;
			uint NumPlanes = 0;
			uint color_primaries = 0;
			uint transfer_characteristics = 0;
			uint matrix_coefficients = 0;
			uint subsampling_x = 0;
			uint subsampling_y = 0;
			uint chroma_sample_position = 0;
			uint separate_uv_delta_q = 0;
			uint color_range = 0;
			size += stream.ReadFixed(size, 1, out this.high_bitdepth, "high_bitdepth"); 

			if ( seq_profile == 2 && high_bitdepth != 0 )
			{
				size += stream.ReadFixed(size, 1, out this.twelve_bit, "twelve_bit"); 
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
				size += stream.ReadFixed(size, 1, out this.mono_chrome, "mono_chrome"); 
			}
			NumPlanes= mono_chrome ? 1 : 3;
			size += stream.ReadFixed(size, 1, out this.color_description_present_flag, "color_description_present_flag"); 

			if ( color_description_present_flag != 0 )
			{
				size += stream.ReadFixed(size, 8, out this.color_primaries, "color_primaries"); 
				size += stream.ReadFixed(size, 8, out this.transfer_characteristics, "transfer_characteristics"); 
				size += stream.ReadFixed(size, 8, out this.matrix_coefficients, "matrix_coefficients"); 
			}
			else 
			{
				color_primaries= CP_UNSPECIFIED;
				transfer_characteristics= TC_UNSPECIFIED;
				matrix_coefficients= MC_UNSPECIFIED;
			}

			if ( mono_chrome != 0 )
			{
				size += stream.ReadFixed(size, 1, out this.color_range, "color_range"); 
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
				size += stream.ReadFixed(size, 1, out this.color_range, "color_range"); 

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
						size += stream.ReadFixed(size, 1, out this.subsampling_x, "subsampling_x"); 

						if ( subsampling_x != 0 )
						{
							size += stream.ReadFixed(size, 1, out this.subsampling_y, "subsampling_y"); 
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
					size += stream.ReadFixed(size, 2, out this.chroma_sample_position, "chroma_sample_position"); 
				}
			}
			size += stream.ReadFixed(size, 1, out this.separate_uv_delta_q, "separate_uv_delta_q"); 

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint BitDepth = 0;
			uint mono_chrome = 0;
			uint NumPlanes = 0;
			uint color_primaries = 0;
			uint transfer_characteristics = 0;
			uint matrix_coefficients = 0;
			uint subsampling_x = 0;
			uint subsampling_y = 0;
			uint chroma_sample_position = 0;
			uint separate_uv_delta_q = 0;
			uint color_range = 0;
			size += stream.WriteFixed(1, this.high_bitdepth, "high_bitdepth"); 

			if ( seq_profile == 2 && high_bitdepth != 0 )
			{
				size += stream.WriteFixed(1, this.twelve_bit, "twelve_bit"); 
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
				size += stream.WriteFixed(1, this.mono_chrome, "mono_chrome"); 
			}
			NumPlanes= mono_chrome ? 1 : 3;
			size += stream.WriteFixed(1, this.color_description_present_flag, "color_description_present_flag"); 

			if ( color_description_present_flag != 0 )
			{
				size += stream.WriteFixed(8, this.color_primaries, "color_primaries"); 
				size += stream.WriteFixed(8, this.transfer_characteristics, "transfer_characteristics"); 
				size += stream.WriteFixed(8, this.matrix_coefficients, "matrix_coefficients"); 
			}
			else 
			{
				color_primaries= CP_UNSPECIFIED;
				transfer_characteristics= TC_UNSPECIFIED;
				matrix_coefficients= MC_UNSPECIFIED;
			}

			if ( mono_chrome != 0 )
			{
				size += stream.WriteFixed(1, this.color_range, "color_range"); 
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
				size += stream.WriteFixed(1, this.color_range, "color_range"); 

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
						size += stream.WriteFixed(1, this.subsampling_x, "subsampling_x"); 

						if ( subsampling_x != 0 )
						{
							size += stream.WriteFixed(1, this.subsampling_y, "subsampling_y"); 
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
					size += stream.WriteFixed(2, this.chroma_sample_position, "chroma_sample_position"); 
				}
			}
			size += stream.WriteFixed(1, this.separate_uv_delta_q, "separate_uv_delta_q"); 

            return size;
         }

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
    public class TimingInfo : IAomSerializable
    {
		private uint num_units_in_display_tick;
		public uint NumUnitsInDisplayTick { get { return num_units_in_display_tick; } set { num_units_in_display_tick = value; } }
		private uint time_scale;
		public uint TimeScale { get { return time_scale; } set { time_scale = value; } }
		private uint equal_picture_interval;
		public uint EqualPictureInterval { get { return equal_picture_interval; } set { equal_picture_interval = value; } }
		private NumTicksPerPictureMinus1 num_ticks_per_picture_minus_1;
		public NumTicksPerPictureMinus1 NumTicksPerPictureMinus1 { get { return num_ticks_per_picture_minus_1; } set { num_ticks_per_picture_minus_1 = value; } }

         public TimingInfo()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			size += stream.ReadFixed(size, 32, out this.num_units_in_display_tick, "num_units_in_display_tick"); 
			size += stream.ReadFixed(size, 32, out this.time_scale, "time_scale"); 
			size += stream.ReadFixed(size, 1, out this.equal_picture_interval, "equal_picture_interval"); 

			if ( equal_picture_interval != 0 )
			{
				this.num_ticks_per_picture_minus_1 =  new NumTicksPerPictureMinus1() ;
				size +=  stream.ReadClass<NumTicksPerPictureMinus1>(size, context, this.num_ticks_per_picture_minus_1, "num_ticks_per_picture_minus_1"); 
			}
			size += stream.ReadUvlc(size,  out this.num_ticks_per_picture_minus_1, "num_ticks_per_picture_minus_1"); 

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			size += stream.WriteFixed(32, this.num_units_in_display_tick, "num_units_in_display_tick"); 
			size += stream.WriteFixed(32, this.time_scale, "time_scale"); 
			size += stream.WriteFixed(1, this.equal_picture_interval, "equal_picture_interval"); 

			if ( equal_picture_interval != 0 )
			{
				size += stream.WriteClass<NumTicksPerPictureMinus1>(context, this.num_ticks_per_picture_minus_1, "num_ticks_per_picture_minus_1"); 
			}
			size += stream.WriteUvlc( this.num_ticks_per_picture_minus_1, "num_ticks_per_picture_minus_1"); 

            return size;
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
    public class DecoderModelInfo : IAomSerializable
    {
		private uint buffer_delay_length_minus_1;
		public uint BufferDelayLengthMinus1 { get { return buffer_delay_length_minus_1; } set { buffer_delay_length_minus_1 = value; } }
		private uint num_units_in_decoding_tick;
		public uint NumUnitsInDecodingTick { get { return num_units_in_decoding_tick; } set { num_units_in_decoding_tick = value; } }
		private uint buffer_removal_time_length_minus_1;
		public uint BufferRemovalTimeLengthMinus1 { get { return buffer_removal_time_length_minus_1; } set { buffer_removal_time_length_minus_1 = value; } }
		private uint frame_presentation_time_length_minus_1;
		public uint FramePresentationTimeLengthMinus1 { get { return frame_presentation_time_length_minus_1; } set { frame_presentation_time_length_minus_1 = value; } }

         public DecoderModelInfo()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			size += stream.ReadFixed(size, 5, out this.buffer_delay_length_minus_1, "buffer_delay_length_minus_1"); 
			size += stream.ReadFixed(size, 32, out this.num_units_in_decoding_tick, "num_units_in_decoding_tick"); 
			size += stream.ReadFixed(size, 5, out this.buffer_removal_time_length_minus_1, "buffer_removal_time_length_minus_1"); 
			size += stream.ReadFixed(size, 5, out this.frame_presentation_time_length_minus_1, "frame_presentation_time_length_minus_1"); 

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			size += stream.WriteFixed(5, this.buffer_delay_length_minus_1, "buffer_delay_length_minus_1"); 
			size += stream.WriteFixed(32, this.num_units_in_decoding_tick, "num_units_in_decoding_tick"); 
			size += stream.WriteFixed(5, this.buffer_removal_time_length_minus_1, "buffer_removal_time_length_minus_1"); 
			size += stream.WriteFixed(5, this.frame_presentation_time_length_minus_1, "frame_presentation_time_length_minus_1"); 

            return size;
         }

    }

    /*



operating_parameters_info( op ) { 
 n = buffer_delay_length_minus_1 + 1
 decoder_buffer_delay[ op ] f(n)
 encoder_buffer_delay[ op ] f(n)
 low_delay_mode_flag[ op ] f(1)
}
    */
    public class OperatingParametersInfo : IAomSerializable
    {
		private uint op;
		public uint Op { get { return op; } set { op = value; } }
		private uint[] decoder_buffer_delay;
		public uint[] DecoderBufferDelay { get { return decoder_buffer_delay; } set { decoder_buffer_delay = value; } }
		private uint[] encoder_buffer_delay;
		public uint[] EncoderBufferDelay { get { return encoder_buffer_delay; } set { encoder_buffer_delay = value; } }
		private uint[] low_delay_mode_flag;
		public uint[] LowDelayModeFlag { get { return low_delay_mode_flag; } set { low_delay_mode_flag = value; } }

         public OperatingParametersInfo(uint op)
         { 
			this.op = op;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint n = 0;
			n= buffer_delay_length_minus_1 + 1;
			size += stream.ReadVariable(size, n, out this.decoder_buffer_delay[ op ], "decoder_buffer_delay"); 
			size += stream.ReadVariable(size, n, out this.encoder_buffer_delay[ op ], "encoder_buffer_delay"); 
			size += stream.ReadFixed(size, 1, out this.low_delay_mode_flag[ op ], "low_delay_mode_flag"); 

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint n = 0;
			n= buffer_delay_length_minus_1 + 1;
			size += stream.WriteVariable(n, this.decoder_buffer_delay[ op ], "decoder_buffer_delay"); 
			size += stream.WriteVariable(n, this.encoder_buffer_delay[ op ], "encoder_buffer_delay"); 
			size += stream.WriteFixed(1, this.low_delay_mode_flag[ op ], "low_delay_mode_flag"); 

            return size;
         }

    }

    /*



temporal_delimiter_obu() { 
 SeenFrameHeader = 0
}
    */
    public class TemporalDelimiterObu : IAomSerializable
    {

         public TemporalDelimiterObu()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint SeenFrameHeader = 0;
			SeenFrameHeader= 0;

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint SeenFrameHeader = 0;
			SeenFrameHeader= 0;

            return size;
         }

    }

    /*



padding_obu() { 
 for ( i = 0; i < obu_padding_length; i++ )
 obu_padding_byte f(8)
}
    */
    public class PaddingObu : IAomSerializable
    {
		private uint[] obu_padding_byte;
		public uint[] ObuPaddingByte { get { return obu_padding_byte; } set { obu_padding_byte = value; } }

         public PaddingObu()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint i = 0;

			this.obu_padding_byte = new uint[ obu_padding_length];
			for ( i = 0; i < obu_padding_length; i++ )
			{
				size += stream.ReadFixed(size, 8, out this.obu_padding_byte[ i ], "obu_padding_byte"); 
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint i = 0;

			for ( i = 0; i < obu_padding_length; i++ )
			{
				size += stream.WriteFixed(8, this.obu_padding_byte[ i ], "obu_padding_byte"); 
			}

            return size;
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
    public class MetadataObu : IAomSerializable
    {
		private uint metadata_type;
		public uint MetadataType { get { return metadata_type; } set { metadata_type = value; } }
		private MetadataItutT35 metadata_itut_t35;
		public MetadataItutT35 MetadataItutT35 { get { return metadata_itut_t35; } set { metadata_itut_t35 = value; } }
		private MetadataHdrCll metadata_hdr_cll;
		public MetadataHdrCll MetadataHdrCll { get { return metadata_hdr_cll; } set { metadata_hdr_cll = value; } }
		private MetadataHdrMdcv metadata_hdr_mdcv;
		public MetadataHdrMdcv MetadataHdrMdcv { get { return metadata_hdr_mdcv; } set { metadata_hdr_mdcv = value; } }
		private MetadataScalability metadata_scalability;
		public MetadataScalability MetadataScalability { get { return metadata_scalability; } set { metadata_scalability = value; } }
		private MetadataTimecode metadata_timecode;
		public MetadataTimecode MetadataTimecode { get { return metadata_timecode; } set { metadata_timecode = value; } }

         public MetadataObu()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			size += stream.ReadLeb128(size,  out this.metadata_type, "metadata_type"); 

			if ( metadata_type == METADATA_TYPE_ITUT_T35 )
			{
				this.metadata_itut_t35 =  new MetadataItutT35() ;
				size +=  stream.ReadClass<MetadataItutT35>(size, context, this.metadata_itut_t35, "metadata_itut_t35"); 
			}
			else if ( metadata_type == METADATA_TYPE_HDR_CLL )
			{
				this.metadata_hdr_cll =  new MetadataHdrCll() ;
				size +=  stream.ReadClass<MetadataHdrCll>(size, context, this.metadata_hdr_cll, "metadata_hdr_cll"); 
			}
			else if ( metadata_type == METADATA_TYPE_HDR_MDCV )
			{
				this.metadata_hdr_mdcv =  new MetadataHdrMdcv() ;
				size +=  stream.ReadClass<MetadataHdrMdcv>(size, context, this.metadata_hdr_mdcv, "metadata_hdr_mdcv"); 
			}
			else if ( metadata_type == METADATA_TYPE_SCALABILITY )
			{
				this.metadata_scalability =  new MetadataScalability() ;
				size +=  stream.ReadClass<MetadataScalability>(size, context, this.metadata_scalability, "metadata_scalability"); 
			}
			else if ( metadata_type == METADATA_TYPE_TIMECODE )
			{
				this.metadata_timecode =  new MetadataTimecode() ;
				size +=  stream.ReadClass<MetadataTimecode>(size, context, this.metadata_timecode, "metadata_timecode"); 
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			size += stream.WriteLeb128( this.metadata_type, "metadata_type"); 

			if ( metadata_type == METADATA_TYPE_ITUT_T35 )
			{
				size += stream.WriteClass<MetadataItutT35>(context, this.metadata_itut_t35, "metadata_itut_t35"); 
			}
			else if ( metadata_type == METADATA_TYPE_HDR_CLL )
			{
				size += stream.WriteClass<MetadataHdrCll>(context, this.metadata_hdr_cll, "metadata_hdr_cll"); 
			}
			else if ( metadata_type == METADATA_TYPE_HDR_MDCV )
			{
				size += stream.WriteClass<MetadataHdrMdcv>(context, this.metadata_hdr_mdcv, "metadata_hdr_mdcv"); 
			}
			else if ( metadata_type == METADATA_TYPE_SCALABILITY )
			{
				size += stream.WriteClass<MetadataScalability>(context, this.metadata_scalability, "metadata_scalability"); 
			}
			else if ( metadata_type == METADATA_TYPE_TIMECODE )
			{
				size += stream.WriteClass<MetadataTimecode>(context, this.metadata_timecode, "metadata_timecode"); 
			}

            return size;
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
    public class MetadataItutT35 : IAomSerializable
    {
		private uint itu_t_t35_country_code;
		public uint ItutT35CountryCode { get { return itu_t_t35_country_code; } set { itu_t_t35_country_code = value; } }
		private uint itu_t_t35_country_code_extension_byte;
		public uint ItutT35CountryCodeExtensionByte { get { return itu_t_t35_country_code_extension_byte; } set { itu_t_t35_country_code_extension_byte = value; } }
		private ItutT35PayloadBytes itu_t_t35_payload_bytes;
		public ItutT35PayloadBytes ItutT35PayloadBytes { get { return itu_t_t35_payload_bytes; } set { itu_t_t35_payload_bytes = value; } }

         public MetadataItutT35()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			size += stream.ReadFixed(size, 8, out this.itu_t_t35_country_code, "itu_t_t35_country_code"); 

			if ( itu_t_t35_country_code == 0xFF )
			{
				size += stream.ReadFixed(size, 8, out this.itu_t_t35_country_code_extension_byte, "itu_t_t35_country_code_extension_byte"); 
			}
			this.itu_t_t35_payload_bytes =  new ItutT35PayloadBytes() ;
			size +=  stream.ReadClass<ItutT35PayloadBytes>(size, context, this.itu_t_t35_payload_bytes, "itu_t_t35_payload_bytes"); 

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			size += stream.WriteFixed(8, this.itu_t_t35_country_code, "itu_t_t35_country_code"); 

			if ( itu_t_t35_country_code == 0xFF )
			{
				size += stream.WriteFixed(8, this.itu_t_t35_country_code_extension_byte, "itu_t_t35_country_code_extension_byte"); 
			}
			size += stream.WriteClass<ItutT35PayloadBytes>(context, this.itu_t_t35_payload_bytes, "itu_t_t35_payload_bytes"); 

            return size;
         }

    }

    /*



metadata_hdr_cll() { 
 max_cll f(16)
 max_fall f(16)
}
    */
    public class MetadataHdrCll : IAomSerializable
    {
		private uint max_cll;
		public uint MaxCll { get { return max_cll; } set { max_cll = value; } }
		private uint max_fall;
		public uint MaxFall { get { return max_fall; } set { max_fall = value; } }

         public MetadataHdrCll()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			size += stream.ReadFixed(size, 16, out this.max_cll, "max_cll"); 
			size += stream.ReadFixed(size, 16, out this.max_fall, "max_fall"); 

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			size += stream.WriteFixed(16, this.max_cll, "max_cll"); 
			size += stream.WriteFixed(16, this.max_fall, "max_fall"); 

            return size;
         }

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
    public class MetadataHdrMdcv : IAomSerializable
    {
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

         public MetadataHdrMdcv()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint i = 0;

			this.primary_chromaticity_x = new uint[ 3];
			this.primary_chromaticity_y = new uint[ 3];
			for ( i = 0; i < 3; i++ )
			{
				size += stream.ReadFixed(size, 16, out this.primary_chromaticity_x[ i ], "primary_chromaticity_x"); 
				size += stream.ReadFixed(size, 16, out this.primary_chromaticity_y[ i ], "primary_chromaticity_y"); 
			}
			size += stream.ReadFixed(size, 16, out this.white_point_chromaticity_x, "white_point_chromaticity_x"); 
			size += stream.ReadFixed(size, 16, out this.white_point_chromaticity_y, "white_point_chromaticity_y"); 
			size += stream.ReadFixed(size, 32, out this.luminance_max, "luminance_max"); 
			size += stream.ReadFixed(size, 32, out this.luminance_min, "luminance_min"); 

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint i = 0;

			for ( i = 0; i < 3; i++ )
			{
				size += stream.WriteFixed(16, this.primary_chromaticity_x[ i ], "primary_chromaticity_x"); 
				size += stream.WriteFixed(16, this.primary_chromaticity_y[ i ], "primary_chromaticity_y"); 
			}
			size += stream.WriteFixed(16, this.white_point_chromaticity_x, "white_point_chromaticity_x"); 
			size += stream.WriteFixed(16, this.white_point_chromaticity_y, "white_point_chromaticity_y"); 
			size += stream.WriteFixed(32, this.luminance_max, "luminance_max"); 
			size += stream.WriteFixed(32, this.luminance_min, "luminance_min"); 

            return size;
         }

    }

    /*



metadata_scalability() { 
 scalability_mode_idc f(8)
 if ( scalability_mode_idc == SCALABILITY_SS )
 scalability_structure()
}
    */
    public class MetadataScalability : IAomSerializable
    {
		private uint scalability_mode_idc;
		public uint ScalabilityModeIdc { get { return scalability_mode_idc; } set { scalability_mode_idc = value; } }
		private ScalabilityStructure scalability_structure;
		public ScalabilityStructure ScalabilityStructure { get { return scalability_structure; } set { scalability_structure = value; } }

         public MetadataScalability()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			size += stream.ReadFixed(size, 8, out this.scalability_mode_idc, "scalability_mode_idc"); 

			if ( scalability_mode_idc == SCALABILITY_SS )
			{
				this.scalability_structure =  new ScalabilityStructure() ;
				size +=  stream.ReadClass<ScalabilityStructure>(size, context, this.scalability_structure, "scalability_structure"); 
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			size += stream.WriteFixed(8, this.scalability_mode_idc, "scalability_mode_idc"); 

			if ( scalability_mode_idc == SCALABILITY_SS )
			{
				size += stream.WriteClass<ScalabilityStructure>(context, this.scalability_structure, "scalability_structure"); 
			}

            return size;
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
    public class ScalabilityStructure : IAomSerializable
    {
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

         public ScalabilityStructure()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint i = 0;
			uint j = 0;
			size += stream.ReadFixed(size, 2, out this.spatial_layers_cnt_minus_1, "spatial_layers_cnt_minus_1"); 
			size += stream.ReadFixed(size, 1, out this.spatial_layer_dimensions_present_flag, "spatial_layer_dimensions_present_flag"); 
			size += stream.ReadFixed(size, 1, out this.spatial_layer_description_present_flag, "spatial_layer_description_present_flag"); 
			size += stream.ReadFixed(size, 1, out this.temporal_group_description_present_flag, "temporal_group_description_present_flag"); 
			size += stream.ReadFixed(size, 3, out this.scalability_structure_reserved_3bits, "scalability_structure_reserved_3bits"); 

			if ( spatial_layer_dimensions_present_flag != 0 )
			{

				this.spatial_layer_max_width = new uint[ spatial_layers_cnt_minus_1 ];
				this.spatial_layer_max_height = new uint[ spatial_layers_cnt_minus_1 ];
				for ( i = 0; i <= spatial_layers_cnt_minus_1 ; i++ )
				{
					size += stream.ReadFixed(size, 16, out this.spatial_layer_max_width[ i ], "spatial_layer_max_width"); 
					size += stream.ReadFixed(size, 16, out this.spatial_layer_max_height[ i ], "spatial_layer_max_height"); 
				}
			}

			if ( spatial_layer_description_present_flag != 0 )
			{

				this.spatial_layer_ref_id = new uint[ spatial_layers_cnt_minus_1];
				for ( i = 0; i <= spatial_layers_cnt_minus_1; i++ )
				{
					size += stream.ReadFixed(size, 8, out this.spatial_layer_ref_id[ i ], "spatial_layer_ref_id"); 
				}
			}

			if ( temporal_group_description_present_flag != 0 )
			{
				size += stream.ReadFixed(size, 8, out this.temporal_group_size, "temporal_group_size"); 

				this.temporal_group_temporal_id = new uint[ temporal_group_size];
				this.temporal_group_temporal_switching_up_point_flag = new uint[ temporal_group_size];
				this.temporal_group_spatial_switching_up_point_flag = new uint[ temporal_group_size];
				this.temporal_group_ref_cnt = new uint[ temporal_group_size];
				this.temporal_group_ref_pic_diff = new uint[ temporal_group_size][];
				for ( i = 0; i < temporal_group_size; i++ )
				{
					size += stream.ReadFixed(size, 3, out this.temporal_group_temporal_id[ i ], "temporal_group_temporal_id"); 
					size += stream.ReadFixed(size, 1, out this.temporal_group_temporal_switching_up_point_flag[ i ], "temporal_group_temporal_switching_up_point_flag"); 
					size += stream.ReadFixed(size, 1, out this.temporal_group_spatial_switching_up_point_flag[ i ], "temporal_group_spatial_switching_up_point_flag"); 
					size += stream.ReadFixed(size, 3, out this.temporal_group_ref_cnt[ i ], "temporal_group_ref_cnt"); 

					this.temporal_group_ref_pic_diff[ i ] = new uint[ temporal_group_ref_cnt[ i ]];
					for ( j = 0; j < temporal_group_ref_cnt[ i ]; j++ )
					{
						size += stream.ReadFixed(size, 8, out this.temporal_group_ref_pic_diff[ i ][ j ], "temporal_group_ref_pic_diff"); 
					}
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint i = 0;
			uint j = 0;
			size += stream.WriteFixed(2, this.spatial_layers_cnt_minus_1, "spatial_layers_cnt_minus_1"); 
			size += stream.WriteFixed(1, this.spatial_layer_dimensions_present_flag, "spatial_layer_dimensions_present_flag"); 
			size += stream.WriteFixed(1, this.spatial_layer_description_present_flag, "spatial_layer_description_present_flag"); 
			size += stream.WriteFixed(1, this.temporal_group_description_present_flag, "temporal_group_description_present_flag"); 
			size += stream.WriteFixed(3, this.scalability_structure_reserved_3bits, "scalability_structure_reserved_3bits"); 

			if ( spatial_layer_dimensions_present_flag != 0 )
			{

				for ( i = 0; i <= spatial_layers_cnt_minus_1 ; i++ )
				{
					size += stream.WriteFixed(16, this.spatial_layer_max_width[ i ], "spatial_layer_max_width"); 
					size += stream.WriteFixed(16, this.spatial_layer_max_height[ i ], "spatial_layer_max_height"); 
				}
			}

			if ( spatial_layer_description_present_flag != 0 )
			{

				for ( i = 0; i <= spatial_layers_cnt_minus_1; i++ )
				{
					size += stream.WriteFixed(8, this.spatial_layer_ref_id[ i ], "spatial_layer_ref_id"); 
				}
			}

			if ( temporal_group_description_present_flag != 0 )
			{
				size += stream.WriteFixed(8, this.temporal_group_size, "temporal_group_size"); 

				for ( i = 0; i < temporal_group_size; i++ )
				{
					size += stream.WriteFixed(3, this.temporal_group_temporal_id[ i ], "temporal_group_temporal_id"); 
					size += stream.WriteFixed(1, this.temporal_group_temporal_switching_up_point_flag[ i ], "temporal_group_temporal_switching_up_point_flag"); 
					size += stream.WriteFixed(1, this.temporal_group_spatial_switching_up_point_flag[ i ], "temporal_group_spatial_switching_up_point_flag"); 
					size += stream.WriteFixed(3, this.temporal_group_ref_cnt[ i ], "temporal_group_ref_cnt"); 

					for ( j = 0; j < temporal_group_ref_cnt[ i ]; j++ )
					{
						size += stream.WriteFixed(8, this.temporal_group_ref_pic_diff[ i ][ j ], "temporal_group_ref_pic_diff"); 
					}
				}
			}

            return size;
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
    public class MetadataTimecode : IAomSerializable
    {
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

         public MetadataTimecode()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			size += stream.ReadFixed(size, 5, out this.counting_type, "counting_type"); 
			size += stream.ReadFixed(size, 1, out this.full_timestamp_flag, "full_timestamp_flag"); 
			size += stream.ReadFixed(size, 1, out this.discontinuity_flag, "discontinuity_flag"); 
			size += stream.ReadFixed(size, 1, out this.cnt_dropped_flag, "cnt_dropped_flag"); 
			size += stream.ReadFixed(size, 9, out this.n_frames, "n_frames"); 

			if ( full_timestamp_flag != 0 )
			{
				size += stream.ReadFixed(size, 6, out this.seconds_value, "seconds_value"); 
				size += stream.ReadFixed(size, 6, out this.minutes_value, "minutes_value"); 
				size += stream.ReadFixed(size, 5, out this.hours_value, "hours_value"); 
			}
			else 
			{
				size += stream.ReadFixed(size, 1, out this.seconds_flag, "seconds_flag"); 

				if ( seconds_flag != 0 )
				{
					size += stream.ReadFixed(size, 6, out this.seconds_value, "seconds_value"); 
					size += stream.ReadFixed(size, 1, out this.minutes_flag, "minutes_flag"); 

					if ( minutes_flag != 0 )
					{
						size += stream.ReadFixed(size, 6, out this.minutes_value, "minutes_value"); 
						size += stream.ReadFixed(size, 1, out this.hours_flag, "hours_flag"); 

						if ( hours_flag != 0 )
						{
							size += stream.ReadFixed(size, 5, out this.hours_value, "hours_value"); 
						}
					}
				}
			}
			size += stream.ReadFixed(size, 5, out this.time_offset_length, "time_offset_length"); 

			if ( time_offset_length > 0 )
			{
				size += stream.ReadVariable(size, time_offset_length, out this.time_offset_value, "time_offset_value"); 
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			size += stream.WriteFixed(5, this.counting_type, "counting_type"); 
			size += stream.WriteFixed(1, this.full_timestamp_flag, "full_timestamp_flag"); 
			size += stream.WriteFixed(1, this.discontinuity_flag, "discontinuity_flag"); 
			size += stream.WriteFixed(1, this.cnt_dropped_flag, "cnt_dropped_flag"); 
			size += stream.WriteFixed(9, this.n_frames, "n_frames"); 

			if ( full_timestamp_flag != 0 )
			{
				size += stream.WriteFixed(6, this.seconds_value, "seconds_value"); 
				size += stream.WriteFixed(6, this.minutes_value, "minutes_value"); 
				size += stream.WriteFixed(5, this.hours_value, "hours_value"); 
			}
			else 
			{
				size += stream.WriteFixed(1, this.seconds_flag, "seconds_flag"); 

				if ( seconds_flag != 0 )
				{
					size += stream.WriteFixed(6, this.seconds_value, "seconds_value"); 
					size += stream.WriteFixed(1, this.minutes_flag, "minutes_flag"); 

					if ( minutes_flag != 0 )
					{
						size += stream.WriteFixed(6, this.minutes_value, "minutes_value"); 
						size += stream.WriteFixed(1, this.hours_flag, "hours_flag"); 

						if ( hours_flag != 0 )
						{
							size += stream.WriteFixed(5, this.hours_value, "hours_value"); 
						}
					}
				}
			}
			size += stream.WriteFixed(5, this.time_offset_length, "time_offset_length"); 

			if ( time_offset_length > 0 )
			{
				size += stream.WriteVariable(time_offset_length, this.time_offset_value, "time_offset_value"); 
			}

            return size;
         }

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
    public class FrameHeaderObu : IAomSerializable
    {
		private FrameHeaderCopy frame_header_copy;
		public FrameHeaderCopy FrameHeaderCopy { get { return frame_header_copy; } set { frame_header_copy = value; } }
		private UncompressedHeader uncompressed_header;
		public UncompressedHeader UncompressedHeader { get { return uncompressed_header; } set { uncompressed_header = value; } }
		private DecodeFrameWrapup decode_frame_wrapup;
		public DecodeFrameWrapup DecodeFrameWrapup { get { return decode_frame_wrapup; } set { decode_frame_wrapup = value; } }

         public FrameHeaderObu()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint SeenFrameHeader = 0;
			uint TileNum = 0;

			if ( SeenFrameHeader == 1 )
			{
				this.frame_header_copy =  new FrameHeaderCopy() ;
				size +=  stream.ReadClass<FrameHeaderCopy>(size, context, this.frame_header_copy, "frame_header_copy"); 
			}
			else 
			{
				SeenFrameHeader= 1;
				this.uncompressed_header =  new UncompressedHeader() ;
				size +=  stream.ReadClass<UncompressedHeader>(size, context, this.uncompressed_header, "uncompressed_header"); 

				if ( show_existing_frame != 0 )
				{
					this.decode_frame_wrapup =  new DecodeFrameWrapup() ;
					size +=  stream.ReadClass<DecodeFrameWrapup>(size, context, this.decode_frame_wrapup, "decode_frame_wrapup"); 
					SeenFrameHeader= 0;
				}
				else 
				{
					TileNum= 0;
					SeenFrameHeader= 1;
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint SeenFrameHeader = 0;
			uint TileNum = 0;

			if ( SeenFrameHeader == 1 )
			{
				size += stream.WriteClass<FrameHeaderCopy>(context, this.frame_header_copy, "frame_header_copy"); 
			}
			else 
			{
				SeenFrameHeader= 1;
				size += stream.WriteClass<UncompressedHeader>(context, this.uncompressed_header, "uncompressed_header"); 

				if ( show_existing_frame != 0 )
				{
					size += stream.WriteClass<DecodeFrameWrapup>(context, this.decode_frame_wrapup, "decode_frame_wrapup"); 
					SeenFrameHeader= 0;
				}
				else 
				{
					TileNum= 0;
					SeenFrameHeader= 1;
				}
			}

            return size;
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
 LosslessArray[ segmentId ] = qindex == 0 && DeltaQYDc == 0 &&
 DeltaQUAc == 0 && DeltaQUDc == 0 &&
 DeltaQVAc == 0 && DeltaQVDc == 0
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
    public class UncompressedHeader : IAomSerializable
    {
		private uint show_existing_frame;
		public uint ShowExistingFrame { get { return show_existing_frame; } set { show_existing_frame = value; } }
		private uint frame_to_show_map_idx;
		public uint FrameToShowMapIdx { get { return frame_to_show_map_idx; } set { frame_to_show_map_idx = value; } }
		private TemporalPointInfo temporal_point_info;
		public TemporalPointInfo TemporalPointInfo { get { return temporal_point_info; } set { temporal_point_info = value; } }
		private uint display_frame_id;
		public uint DisplayFrameId { get { return display_frame_id; } set { display_frame_id = value; } }
		private LoadGrainParams load_grain_params;
		public LoadGrainParams LoadGrainParams { get { return load_grain_params; } set { load_grain_params = value; } }
		private uint frame_type;
		public uint FrameType { get { return frame_type; } set { frame_type = value; } }
		private ShowFrame show_frame;
		public ShowFrame ShowFrame { get { return show_frame; } set { show_frame = value; } }
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
		private CurrentFrameId current_frame_id;
		public CurrentFrameId CurrentFrameId { get { return current_frame_id; } set { current_frame_id = value; } }
		private MarkRefFrames mark_ref_frames;
		public MarkRefFrames MarkRefFrames { get { return mark_ref_frames; } set { mark_ref_frames = value; } }
		private uint frame_size_override_flag;
		public uint FrameSizeOverrideFlag { get { return frame_size_override_flag; } set { frame_size_override_flag = value; } }
		private OrderHint order_hint;
		public OrderHint OrderHint { get { return order_hint; } set { order_hint = value; } }
		private uint primary_ref_frame;
		public uint PrimaryRefFrame { get { return primary_ref_frame; } set { primary_ref_frame = value; } }
		private uint buffer_removal_time_present_flag;
		public uint BufferRemovalTimePresentFlag { get { return buffer_removal_time_present_flag; } set { buffer_removal_time_present_flag = value; } }
		private uint[] buffer_removal_time;
		public uint[] BufferRemovalTime { get { return buffer_removal_time; } set { buffer_removal_time = value; } }
		private uint refresh_frame_flags;
		public uint RefreshFrameFlags { get { return refresh_frame_flags; } set { refresh_frame_flags = value; } }
		private RefOrderHint ref_order_hint;
		public RefOrderHint RefOrderHint { get { return ref_order_hint; } set { ref_order_hint = value; } }
		private FrameSize frame_size;
		public FrameSize FrameSize { get { return frame_size; } set { frame_size = value; } }
		private RenderSize render_size;
		public RenderSize RenderSize { get { return render_size; } set { render_size = value; } }
		private uint allow_intrabc;
		public uint AllowIntrabc { get { return allow_intrabc; } set { allow_intrabc = value; } }
		private uint frame_refs_short_signaling;
		public uint FrameRefsShortSignaling { get { return frame_refs_short_signaling; } set { frame_refs_short_signaling = value; } }
		private uint last_frame_idx;
		public uint LastFrameIdx { get { return last_frame_idx; } set { last_frame_idx = value; } }
		private uint gold_frame_idx;
		public uint GoldFrameIdx { get { return gold_frame_idx; } set { gold_frame_idx = value; } }
		private SetFrameRefs set_frame_refs;
		public SetFrameRefs SetFrameRefs { get { return set_frame_refs; } set { set_frame_refs = value; } }
		private uint[] ref_frame_idx;
		public uint[] RefFrameIdx { get { return ref_frame_idx; } set { ref_frame_idx = value; } }
		private uint[] delta_frame_id_minus_1;
		public uint[] DeltaFrameIdMinus1 { get { return delta_frame_id_minus_1; } set { delta_frame_id_minus_1 = value; } }
		private FrameSizeWithRefs frame_size_with_refs;
		public FrameSizeWithRefs FrameSizeWithRefs { get { return frame_size_with_refs; } set { frame_size_with_refs = value; } }
		private uint allow_high_precision_mv;
		public uint AllowHighPrecisionMv { get { return allow_high_precision_mv; } set { allow_high_precision_mv = value; } }
		private ReadInterpolationFilter read_interpolation_filter;
		public ReadInterpolationFilter ReadInterpolationFilter { get { return read_interpolation_filter; } set { read_interpolation_filter = value; } }
		private uint is_motion_mode_switchable;
		public uint IsMotionModeSwitchable { get { return is_motion_mode_switchable; } set { is_motion_mode_switchable = value; } }
		private uint use_ref_frame_mvs;
		public uint UseRefFrameMvs { get { return use_ref_frame_mvs; } set { use_ref_frame_mvs = value; } }
		private uint disable_frame_end_update_cdf;
		public uint DisableFrameEndUpdateCdf { get { return disable_frame_end_update_cdf; } set { disable_frame_end_update_cdf = value; } }
		private InitNonCoeffCdfs init_non_coeff_cdfs;
		public InitNonCoeffCdfs InitNonCoeffCdfs { get { return init_non_coeff_cdfs; } set { init_non_coeff_cdfs = value; } }
		private SetupPastIndependence setup_past_independence;
		public SetupPastIndependence SetupPastIndependence { get { return setup_past_independence; } set { setup_past_independence = value; } }
		private LoadCdfs load_cdfs;
		public LoadCdfs LoadCdfs { get { return load_cdfs; } set { load_cdfs = value; } }
		private LoadPrevious load_previous;
		public LoadPrevious LoadPrevious { get { return load_previous; } set { load_previous = value; } }
		private MotionFieldEstimation motion_field_estimation;
		public MotionFieldEstimation MotionFieldEstimation { get { return motion_field_estimation; } set { motion_field_estimation = value; } }
		private TileInfo tile_info;
		public TileInfo TileInfo { get { return tile_info; } set { tile_info = value; } }
		private QuantizationParams quantization_params;
		public QuantizationParams QuantizationParams { get { return quantization_params; } set { quantization_params = value; } }
		private SegmentationParams segmentation_params;
		public SegmentationParams SegmentationParams { get { return segmentation_params; } set { segmentation_params = value; } }
		private DeltaqParams delta_q_params;
		public DeltaqParams DeltaqParams { get { return delta_q_params; } set { delta_q_params = value; } }
		private DeltaLfParams delta_lf_params;
		public DeltaLfParams DeltaLfParams { get { return delta_lf_params; } set { delta_lf_params = value; } }
		private InitCoeffCdfs init_coeff_cdfs;
		public InitCoeffCdfs InitCoeffCdfs { get { return init_coeff_cdfs; } set { init_coeff_cdfs = value; } }
		private LoadPreviousSegmentIds load_previous_segment_ids;
		public LoadPreviousSegmentIds LoadPreviousSegmentIds { get { return load_previous_segment_ids; } set { load_previous_segment_ids = value; } }
		private LoopFilterParams loop_filter_params;
		public LoopFilterParams LoopFilterParams { get { return loop_filter_params; } set { loop_filter_params = value; } }
		private CdefParams cdef_params;
		public CdefParams CdefParams { get { return cdef_params; } set { cdef_params = value; } }
		private LrParams lr_params;
		public LrParams LrParams { get { return lr_params; } set { lr_params = value; } }
		private ReadTxMode read_tx_mode;
		public ReadTxMode ReadTxMode { get { return read_tx_mode; } set { read_tx_mode = value; } }
		private FrameReferenceMode frame_reference_mode;
		public FrameReferenceMode FrameReferenceMode { get { return frame_reference_mode; } set { frame_reference_mode = value; } }
		private SkipModeParams skip_mode_params;
		public SkipModeParams SkipModeParams { get { return skip_mode_params; } set { skip_mode_params = value; } }
		private uint allow_warped_motion;
		public uint AllowWarpedMotion { get { return allow_warped_motion; } set { allow_warped_motion = value; } }
		private uint reduced_tx_set;
		public uint ReducedTxSet { get { return reduced_tx_set; } set { reduced_tx_set = value; } }
		private GlobalMotionParams global_motion_params;
		public GlobalMotionParams GlobalMotionParams { get { return global_motion_params; } set { global_motion_params = value; } }
		private FilmGrainParams film_grain_params;
		public FilmGrainParams FilmGrainParams { get { return film_grain_params; } set { film_grain_params = value; } }

         public UncompressedHeader()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint idLen = 0;
			uint allFrames = 0;
			uint show_existing_frame = 0;
			uint frame_type = 0;
			uint FrameIsIntra = 0;
			uint show_frame = 0;
			uint showable_frame = 0;
			uint refresh_frame_flags = 0;
			uint error_resilient_mode = 0;
			uint i = 0;
			uint[] RefValid = null;
			uint[] RefOrderHint = null;
			uint[] OrderHints = null;
			uint allow_screen_content_tools = 0;
			uint force_integer_mv = 0;
			uint PrevFrameID = 0;
			uint current_frame_id = 0;
			uint frame_size_override_flag = 0;
			uint OrderHint = 0;
			uint primary_ref_frame = 0;
			uint opNum = 0;
			uint opPtIdc = 0;
			uint inTemporalLayer = 0;
			uint inSpatialLayer = 0;
			uint n = 0;
			uint allow_high_precision_mv = 0;
			uint use_ref_frame_mvs = 0;
			uint allow_intrabc = 0;
			uint frame_refs_short_signaling = 0;
			uint DeltaFrameId = 0;
			uint[] expectedFrameId = null;
			uint refFrame = 0;
			uint hint = 0;
			uint[] RefFrameSignBias = null;
			uint disable_frame_end_update_cdf = 0;
			uint CodedLossless = 0;
			uint segmentId = 0;
			uint qindex = 0;
			uint[] LosslessArray = null;
			uint DeltaQUAc = 0;
			uint DeltaQVAc = 0;
			uint[][] SegQMLevel = null;
			uint AllLossless = 0;
			uint allow_warped_motion = 0;

			if ( frame_id_numbers_present_flag != 0 )
			{
				idLen= ( additional_frame_id_length_minus_1 + delta_frame_id_length_minus_2 + 3 );
			}
			allFrames= (1 << NUM_REF_FRAMES) - 1;

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
				size += stream.ReadFixed(size, 1, out this.show_existing_frame, "show_existing_frame"); 

				if ( show_existing_frame == 1 )
				{
					size += stream.ReadFixed(size, 3, out this.frame_to_show_map_idx, "frame_to_show_map_idx"); 

					if ( decoder_model_info_present_flag != 0 && equal_picture_interval== 0 )
					{
						this.temporal_point_info =  new TemporalPointInfo() ;
						size +=  stream.ReadClass<TemporalPointInfo>(size, context, this.temporal_point_info, "temporal_point_info"); 
					}
					refresh_frame_flags= 0;

					if ( frame_id_numbers_present_flag != 0 )
					{
						size += stream.ReadVariable(size, idLen, out this.display_frame_id, "display_frame_id"); 
					}
					frame_type= RefFrameType[ frame_to_show_map_idx ];

					if ( frame_type == KEY_FRAME )
					{
						refresh_frame_flags= allFrames;
					}

					if ( film_grain_params_present != 0 )
					{
						this.load_grain_params =  new LoadGrainParams( frame_to_show_map_idx ) ;
						size +=  stream.ReadClass<LoadGrainParams>(size, context, this.load_grain_params, "load_grain_params"); 
					}
return;
				}
				size += stream.ReadFixed(size, 2, out this.frame_type, "frame_type"); 
				FrameIsIntra= (frame_type == INTRA_ONLY_FRAME || frame_type == KEY_FRAME) ? (uint)1 : (uint)0;
				this.show_frame =  new ShowFrame() ;
				size +=  stream.ReadClass<ShowFrame>(size, context, this.show_frame, "show_frame"); 
				size += stream.ReadFixed(size, 1, out this.show_frame, "show_frame"); 

				if ( show_frame != 0 && decoder_model_info_present_flag != 0 && equal_picture_interval== 0 )
				{
					this.temporal_point_info =  new TemporalPointInfo() ;
					size +=  stream.ReadClass<TemporalPointInfo>(size, context, this.temporal_point_info, "temporal_point_info"); 
				}

				if ( show_frame != 0 )
				{
					showable_frame= frame_type != KEY_FRAME;
				}
				else 
				{
					size += stream.ReadFixed(size, 1, out this.showable_frame, "showable_frame"); 
				}

				if ( frame_type == SWITCH_FRAME || ( frame_type == KEY_FRAME && show_frame != 0 ) )
				{
					error_resilient_mode= 1;
				}
				else 
				{
					size += stream.ReadFixed(size, 1, out this.error_resilient_mode, "error_resilient_mode"); 
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
			size += stream.ReadFixed(size, 1, out this.disable_cdf_update, "disable_cdf_update"); 

			if ( seq_force_screen_content_tools == SELECT_SCREEN_CONTENT_TOOLS )
			{
				size += stream.ReadFixed(size, 1, out this.allow_screen_content_tools, "allow_screen_content_tools"); 
			}
			else 
			{
				allow_screen_content_tools= seq_force_screen_content_tools;
			}

			if ( allow_screen_content_tools != 0 )
			{

				if ( seq_force_integer_mv == SELECT_INTEGER_MV )
				{
					size += stream.ReadFixed(size, 1, out this.force_integer_mv, "force_integer_mv"); 
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
				this.current_frame_id =  new CurrentFrameId() ;
				size +=  stream.ReadClass<CurrentFrameId>(size, context, this.current_frame_id, "current_frame_id"); 
				size += stream.ReadVariable(size, idLen, out this.current_frame_id, "current_frame_id"); 
				this.mark_ref_frames =  new MarkRefFrames( idLen ) ;
				size +=  stream.ReadClass<MarkRefFrames>(size, context, this.mark_ref_frames, "mark_ref_frames"); 
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
				size += stream.ReadFixed(size, 1, out this.frame_size_override_flag, "frame_size_override_flag"); 
			}
			this.order_hint =  new OrderHint() ;
			size +=  stream.ReadClass<OrderHint>(size, context, this.order_hint, "order_hint"); 
			size += stream.ReadVariable(size, OrderHintBits, out this.order_hint, "order_hint"); 
			OrderHint= order_hint;

			if ( FrameIsIntra != 0 || error_resilient_mode != 0 )
			{
				primary_ref_frame= PRIMARY_REF_NONE;
			}
			else 
			{
				size += stream.ReadFixed(size, 3, out this.primary_ref_frame, "primary_ref_frame"); 
			}

			if ( decoder_model_info_present_flag != 0 )
			{
				size += stream.ReadFixed(size, 1, out this.buffer_removal_time_present_flag, "buffer_removal_time_present_flag"); 

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
								size += stream.ReadVariable(size, n, out this.buffer_removal_time[ opNum ], "buffer_removal_time"); 
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
				size += stream.ReadFixed(size, 8, out this.refresh_frame_flags, "refresh_frame_flags"); 
			}

			if ( FrameIsIntra== 0 || refresh_frame_flags != allFrames )
			{

				if ( error_resilient_mode != 0 && enable_order_hint != 0 )
				{

					this.ref_order_hint = new RefOrderHint[ NUM_REF_FRAMES];
					for ( i = 0; i < NUM_REF_FRAMES; i++)
					{
						this.ref_order_hint[ i ] =  new RefOrderHint() ;
						size +=  stream.ReadClass<RefOrderHint>(size, context, this.ref_order_hint[ i ], "ref_order_hint"); 
						size += stream.ReadVariable(size, OrderHintBits, out this.ref_order_hint[ i ], "ref_order_hint"); 

						if ( ref_order_hint[ i ] != RefOrderHint[ i ] )
						{
							RefValid[ i ]= 0;
						}
					}
				}
			}

			if (  FrameIsIntra != 0 )
			{
				this.frame_size =  new FrameSize() ;
				size +=  stream.ReadClass<FrameSize>(size, context, this.frame_size, "frame_size"); 
				this.render_size =  new RenderSize() ;
				size +=  stream.ReadClass<RenderSize>(size, context, this.render_size, "render_size"); 

				if ( allow_screen_content_tools != 0 && UpscaledWidth == FrameWidth )
				{
					size += stream.ReadFixed(size, 1, out this.allow_intrabc, "allow_intrabc"); 
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
					size += stream.ReadFixed(size, 1, out this.frame_refs_short_signaling, "frame_refs_short_signaling"); 

					if ( frame_refs_short_signaling != 0 )
					{
						size += stream.ReadFixed(size, 3, out this.last_frame_idx, "last_frame_idx"); 
						size += stream.ReadFixed(size, 3, out this.gold_frame_idx, "gold_frame_idx"); 
						this.set_frame_refs =  new SetFrameRefs() ;
						size +=  stream.ReadClass<SetFrameRefs>(size, context, this.set_frame_refs, "set_frame_refs"); 
					}
				}

				this.ref_frame_idx = new uint[ REFS_PER_FRAME];
				this.delta_frame_id_minus_1 = new uint[ REFS_PER_FRAME];
				for ( i = 0; i < REFS_PER_FRAME; i++ )
				{

					if ( frame_refs_short_signaling== 0 )
					{
						size += stream.ReadFixed(size, 3, out this.ref_frame_idx[ i ], "ref_frame_idx"); 
					}

					if ( frame_id_numbers_present_flag != 0 )
					{
						n= delta_frame_id_length_minus_2 + 2;
						size += stream.ReadVariable(size, n, out this.delta_frame_id_minus_1[ i ], "delta_frame_id_minus_1"); 
						DeltaFrameId= delta_frame_id_minus_1[i] + 1;
						expectedFrameId[ i ]= ((current_frame_id + (1 << idLen) - DeltaFrameId ) % (1 << idLen));
					}
				}

				if ( frame_size_override_flag != 0 && error_resilient_mode== 0 )
				{
					this.frame_size_with_refs =  new FrameSizeWithRefs() ;
					size +=  stream.ReadClass<FrameSizeWithRefs>(size, context, this.frame_size_with_refs, "frame_size_with_refs"); 
				}
				else 
				{
					this.frame_size =  new FrameSize() ;
					size +=  stream.ReadClass<FrameSize>(size, context, this.frame_size, "frame_size"); 
					this.render_size =  new RenderSize() ;
					size +=  stream.ReadClass<RenderSize>(size, context, this.render_size, "render_size"); 
				}

				if ( force_integer_mv != 0 )
				{
					allow_high_precision_mv= 0;
				}
				else 
				{
					size += stream.ReadFixed(size, 1, out this.allow_high_precision_mv, "allow_high_precision_mv"); 
				}
				this.read_interpolation_filter =  new ReadInterpolationFilter() ;
				size +=  stream.ReadClass<ReadInterpolationFilter>(size, context, this.read_interpolation_filter, "read_interpolation_filter"); 
				size += stream.ReadFixed(size, 1, out this.is_motion_mode_switchable, "is_motion_mode_switchable"); 

				if ( error_resilient_mode != 0 || enable_ref_frame_mvs== 0 )
				{
					use_ref_frame_mvs= 0;
				}
				else 
				{
					size += stream.ReadFixed(size, 1, out this.use_ref_frame_mvs, "use_ref_frame_mvs"); 
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
				size += stream.ReadFixed(size, 1, out this.disable_frame_end_update_cdf, "disable_frame_end_update_cdf"); 
			}

			if ( primary_ref_frame == PRIMARY_REF_NONE )
			{
				this.init_non_coeff_cdfs =  new InitNonCoeffCdfs() ;
				size +=  stream.ReadClass<InitNonCoeffCdfs>(size, context, this.init_non_coeff_cdfs, "init_non_coeff_cdfs"); 
				this.setup_past_independence =  new SetupPastIndependence() ;
				size +=  stream.ReadClass<SetupPastIndependence>(size, context, this.setup_past_independence, "setup_past_independence"); 
			}
			else 
			{
				this.load_cdfs =  new LoadCdfs( ref_frame_idx[ primary_ref_frame ] ) ;
				size +=  stream.ReadClass<LoadCdfs>(size, context, this.load_cdfs, "load_cdfs"); 
				this.load_previous =  new LoadPrevious() ;
				size +=  stream.ReadClass<LoadPrevious>(size, context, this.load_previous, "load_previous"); 
			}

			if ( use_ref_frame_mvs == 1 )
			{
				this.motion_field_estimation =  new MotionFieldEstimation() ;
				size +=  stream.ReadClass<MotionFieldEstimation>(size, context, this.motion_field_estimation, "motion_field_estimation"); 
			}
			this.tile_info =  new TileInfo() ;
			size +=  stream.ReadClass<TileInfo>(size, context, this.tile_info, "tile_info"); 
			this.quantization_params =  new QuantizationParams() ;
			size +=  stream.ReadClass<QuantizationParams>(size, context, this.quantization_params, "quantization_params"); 
			this.segmentation_params =  new SegmentationParams() ;
			size +=  stream.ReadClass<SegmentationParams>(size, context, this.segmentation_params, "segmentation_params"); 
			this.delta_q_params =  new DeltaqParams() ;
			size +=  stream.ReadClass<DeltaqParams>(size, context, this.delta_q_params, "delta_q_params"); 
			this.delta_lf_params =  new DeltaLfParams() ;
			size +=  stream.ReadClass<DeltaLfParams>(size, context, this.delta_lf_params, "delta_lf_params"); 

			if ( primary_ref_frame == PRIMARY_REF_NONE )
			{
				this.init_coeff_cdfs =  new InitCoeffCdfs() ;
				size +=  stream.ReadClass<InitCoeffCdfs>(size, context, this.init_coeff_cdfs, "init_coeff_cdfs"); 
			}
			else 
			{
				this.load_previous_segment_ids =  new LoadPreviousSegmentIds() ;
				size +=  stream.ReadClass<LoadPreviousSegmentIds>(size, context, this.load_previous_segment_ids, "load_previous_segment_ids"); 
			}
			CodedLossless= 1;

			for ( segmentId = 0; segmentId < MAX_SEGMENTS; segmentId++ )
			{
				qindex= get_qindex( 1, segmentId );
				LosslessArray[ segmentId ]= qindex == 0 && DeltaQYDc == 0 && ? (uint)1 : (uint)0;
				DeltaQUAc= = 0 && DeltaQUDc == 0 && ? (uint)1 : (uint)0;
				DeltaQVAc= = 0 && DeltaQVDc == 0 ? (uint)1 : (uint)0;

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
			AllLossless= CodedLossless && ( FrameWidth == UpscaledWidth ) ? (uint)1 : (uint)0;
			this.loop_filter_params =  new LoopFilterParams() ;
			size +=  stream.ReadClass<LoopFilterParams>(size, context, this.loop_filter_params, "loop_filter_params"); 
			this.cdef_params =  new CdefParams() ;
			size +=  stream.ReadClass<CdefParams>(size, context, this.cdef_params, "cdef_params"); 
			this.lr_params =  new LrParams() ;
			size +=  stream.ReadClass<LrParams>(size, context, this.lr_params, "lr_params"); 
			this.read_tx_mode =  new ReadTxMode() ;
			size +=  stream.ReadClass<ReadTxMode>(size, context, this.read_tx_mode, "read_tx_mode"); 
			this.frame_reference_mode =  new FrameReferenceMode() ;
			size +=  stream.ReadClass<FrameReferenceMode>(size, context, this.frame_reference_mode, "frame_reference_mode"); 
			this.skip_mode_params =  new SkipModeParams() ;
			size +=  stream.ReadClass<SkipModeParams>(size, context, this.skip_mode_params, "skip_mode_params"); 

			if ( FrameIsIntra != 0 || error_resilient_mode != 0 || enable_warped_motion== 0 )
			{
				allow_warped_motion= 0;
			}
			else 
			{
				size += stream.ReadFixed(size, 1, out this.allow_warped_motion, "allow_warped_motion"); 
			}
			size += stream.ReadFixed(size, 1, out this.reduced_tx_set, "reduced_tx_set"); 
			this.global_motion_params =  new GlobalMotionParams() ;
			size +=  stream.ReadClass<GlobalMotionParams>(size, context, this.global_motion_params, "global_motion_params"); 
			this.film_grain_params =  new FilmGrainParams() ;
			size +=  stream.ReadClass<FilmGrainParams>(size, context, this.film_grain_params, "film_grain_params"); 

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint idLen = 0;
			uint allFrames = 0;
			uint show_existing_frame = 0;
			uint frame_type = 0;
			uint FrameIsIntra = 0;
			uint show_frame = 0;
			uint showable_frame = 0;
			uint refresh_frame_flags = 0;
			uint error_resilient_mode = 0;
			uint i = 0;
			uint[] RefValid = null;
			uint[] RefOrderHint = null;
			uint[] OrderHints = null;
			uint allow_screen_content_tools = 0;
			uint force_integer_mv = 0;
			uint PrevFrameID = 0;
			uint current_frame_id = 0;
			uint frame_size_override_flag = 0;
			uint OrderHint = 0;
			uint primary_ref_frame = 0;
			uint opNum = 0;
			uint opPtIdc = 0;
			uint inTemporalLayer = 0;
			uint inSpatialLayer = 0;
			uint n = 0;
			uint allow_high_precision_mv = 0;
			uint use_ref_frame_mvs = 0;
			uint allow_intrabc = 0;
			uint frame_refs_short_signaling = 0;
			uint DeltaFrameId = 0;
			uint[] expectedFrameId = null;
			uint refFrame = 0;
			uint hint = 0;
			uint[] RefFrameSignBias = null;
			uint disable_frame_end_update_cdf = 0;
			uint CodedLossless = 0;
			uint segmentId = 0;
			uint qindex = 0;
			uint[] LosslessArray = null;
			uint DeltaQUAc = 0;
			uint DeltaQVAc = 0;
			uint[][] SegQMLevel = null;
			uint AllLossless = 0;
			uint allow_warped_motion = 0;

			if ( frame_id_numbers_present_flag != 0 )
			{
				idLen= ( additional_frame_id_length_minus_1 + delta_frame_id_length_minus_2 + 3 );
			}
			allFrames= (1 << NUM_REF_FRAMES) - 1;

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
				size += stream.WriteFixed(1, this.show_existing_frame, "show_existing_frame"); 

				if ( show_existing_frame == 1 )
				{
					size += stream.WriteFixed(3, this.frame_to_show_map_idx, "frame_to_show_map_idx"); 

					if ( decoder_model_info_present_flag != 0 && equal_picture_interval== 0 )
					{
						size += stream.WriteClass<TemporalPointInfo>(context, this.temporal_point_info, "temporal_point_info"); 
					}
					refresh_frame_flags= 0;

					if ( frame_id_numbers_present_flag != 0 )
					{
						size += stream.WriteVariable(idLen, this.display_frame_id, "display_frame_id"); 
					}
					frame_type= RefFrameType[ frame_to_show_map_idx ];

					if ( frame_type == KEY_FRAME )
					{
						refresh_frame_flags= allFrames;
					}

					if ( film_grain_params_present != 0 )
					{
						size += stream.WriteClass<LoadGrainParams>(context, this.load_grain_params, "load_grain_params"); 
					}
return;
				}
				size += stream.WriteFixed(2, this.frame_type, "frame_type"); 
				FrameIsIntra= (frame_type == INTRA_ONLY_FRAME || frame_type == KEY_FRAME) ? (uint)1 : (uint)0;
				size += stream.WriteClass<ShowFrame>(context, this.show_frame, "show_frame"); 
				size += stream.WriteFixed(1, this.show_frame, "show_frame"); 

				if ( show_frame != 0 && decoder_model_info_present_flag != 0 && equal_picture_interval== 0 )
				{
					size += stream.WriteClass<TemporalPointInfo>(context, this.temporal_point_info, "temporal_point_info"); 
				}

				if ( show_frame != 0 )
				{
					showable_frame= frame_type != KEY_FRAME;
				}
				else 
				{
					size += stream.WriteFixed(1, this.showable_frame, "showable_frame"); 
				}

				if ( frame_type == SWITCH_FRAME || ( frame_type == KEY_FRAME && show_frame != 0 ) )
				{
					error_resilient_mode= 1;
				}
				else 
				{
					size += stream.WriteFixed(1, this.error_resilient_mode, "error_resilient_mode"); 
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
			size += stream.WriteFixed(1, this.disable_cdf_update, "disable_cdf_update"); 

			if ( seq_force_screen_content_tools == SELECT_SCREEN_CONTENT_TOOLS )
			{
				size += stream.WriteFixed(1, this.allow_screen_content_tools, "allow_screen_content_tools"); 
			}
			else 
			{
				allow_screen_content_tools= seq_force_screen_content_tools;
			}

			if ( allow_screen_content_tools != 0 )
			{

				if ( seq_force_integer_mv == SELECT_INTEGER_MV )
				{
					size += stream.WriteFixed(1, this.force_integer_mv, "force_integer_mv"); 
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
				size += stream.WriteClass<CurrentFrameId>(context, this.current_frame_id, "current_frame_id"); 
				size += stream.WriteVariable(idLen, this.current_frame_id, "current_frame_id"); 
				size += stream.WriteClass<MarkRefFrames>(context, this.mark_ref_frames, "mark_ref_frames"); 
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
				size += stream.WriteFixed(1, this.frame_size_override_flag, "frame_size_override_flag"); 
			}
			size += stream.WriteClass<OrderHint>(context, this.order_hint, "order_hint"); 
			size += stream.WriteVariable(OrderHintBits, this.order_hint, "order_hint"); 
			OrderHint= order_hint;

			if ( FrameIsIntra != 0 || error_resilient_mode != 0 )
			{
				primary_ref_frame= PRIMARY_REF_NONE;
			}
			else 
			{
				size += stream.WriteFixed(3, this.primary_ref_frame, "primary_ref_frame"); 
			}

			if ( decoder_model_info_present_flag != 0 )
			{
				size += stream.WriteFixed(1, this.buffer_removal_time_present_flag, "buffer_removal_time_present_flag"); 

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
								size += stream.WriteVariable(n, this.buffer_removal_time[ opNum ], "buffer_removal_time"); 
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
				size += stream.WriteFixed(8, this.refresh_frame_flags, "refresh_frame_flags"); 
			}

			if ( FrameIsIntra== 0 || refresh_frame_flags != allFrames )
			{

				if ( error_resilient_mode != 0 && enable_order_hint != 0 )
				{

					for ( i = 0; i < NUM_REF_FRAMES; i++)
					{
						size += stream.WriteClass<RefOrderHint>(context, this.ref_order_hint[ i ], "ref_order_hint"); 
						size += stream.WriteVariable(OrderHintBits, this.ref_order_hint[ i ], "ref_order_hint"); 

						if ( ref_order_hint[ i ] != RefOrderHint[ i ] )
						{
							RefValid[ i ]= 0;
						}
					}
				}
			}

			if (  FrameIsIntra != 0 )
			{
				size += stream.WriteClass<FrameSize>(context, this.frame_size, "frame_size"); 
				size += stream.WriteClass<RenderSize>(context, this.render_size, "render_size"); 

				if ( allow_screen_content_tools != 0 && UpscaledWidth == FrameWidth )
				{
					size += stream.WriteFixed(1, this.allow_intrabc, "allow_intrabc"); 
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
					size += stream.WriteFixed(1, this.frame_refs_short_signaling, "frame_refs_short_signaling"); 

					if ( frame_refs_short_signaling != 0 )
					{
						size += stream.WriteFixed(3, this.last_frame_idx, "last_frame_idx"); 
						size += stream.WriteFixed(3, this.gold_frame_idx, "gold_frame_idx"); 
						size += stream.WriteClass<SetFrameRefs>(context, this.set_frame_refs, "set_frame_refs"); 
					}
				}

				for ( i = 0; i < REFS_PER_FRAME; i++ )
				{

					if ( frame_refs_short_signaling== 0 )
					{
						size += stream.WriteFixed(3, this.ref_frame_idx[ i ], "ref_frame_idx"); 
					}

					if ( frame_id_numbers_present_flag != 0 )
					{
						n= delta_frame_id_length_minus_2 + 2;
						size += stream.WriteVariable(n, this.delta_frame_id_minus_1[ i ], "delta_frame_id_minus_1"); 
						DeltaFrameId= delta_frame_id_minus_1[i] + 1;
						expectedFrameId[ i ]= ((current_frame_id + (1 << idLen) - DeltaFrameId ) % (1 << idLen));
					}
				}

				if ( frame_size_override_flag != 0 && error_resilient_mode== 0 )
				{
					size += stream.WriteClass<FrameSizeWithRefs>(context, this.frame_size_with_refs, "frame_size_with_refs"); 
				}
				else 
				{
					size += stream.WriteClass<FrameSize>(context, this.frame_size, "frame_size"); 
					size += stream.WriteClass<RenderSize>(context, this.render_size, "render_size"); 
				}

				if ( force_integer_mv != 0 )
				{
					allow_high_precision_mv= 0;
				}
				else 
				{
					size += stream.WriteFixed(1, this.allow_high_precision_mv, "allow_high_precision_mv"); 
				}
				size += stream.WriteClass<ReadInterpolationFilter>(context, this.read_interpolation_filter, "read_interpolation_filter"); 
				size += stream.WriteFixed(1, this.is_motion_mode_switchable, "is_motion_mode_switchable"); 

				if ( error_resilient_mode != 0 || enable_ref_frame_mvs== 0 )
				{
					use_ref_frame_mvs= 0;
				}
				else 
				{
					size += stream.WriteFixed(1, this.use_ref_frame_mvs, "use_ref_frame_mvs"); 
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
				size += stream.WriteFixed(1, this.disable_frame_end_update_cdf, "disable_frame_end_update_cdf"); 
			}

			if ( primary_ref_frame == PRIMARY_REF_NONE )
			{
				size += stream.WriteClass<InitNonCoeffCdfs>(context, this.init_non_coeff_cdfs, "init_non_coeff_cdfs"); 
				size += stream.WriteClass<SetupPastIndependence>(context, this.setup_past_independence, "setup_past_independence"); 
			}
			else 
			{
				size += stream.WriteClass<LoadCdfs>(context, this.load_cdfs, "load_cdfs"); 
				size += stream.WriteClass<LoadPrevious>(context, this.load_previous, "load_previous"); 
			}

			if ( use_ref_frame_mvs == 1 )
			{
				size += stream.WriteClass<MotionFieldEstimation>(context, this.motion_field_estimation, "motion_field_estimation"); 
			}
			size += stream.WriteClass<TileInfo>(context, this.tile_info, "tile_info"); 
			size += stream.WriteClass<QuantizationParams>(context, this.quantization_params, "quantization_params"); 
			size += stream.WriteClass<SegmentationParams>(context, this.segmentation_params, "segmentation_params"); 
			size += stream.WriteClass<DeltaqParams>(context, this.delta_q_params, "delta_q_params"); 
			size += stream.WriteClass<DeltaLfParams>(context, this.delta_lf_params, "delta_lf_params"); 

			if ( primary_ref_frame == PRIMARY_REF_NONE )
			{
				size += stream.WriteClass<InitCoeffCdfs>(context, this.init_coeff_cdfs, "init_coeff_cdfs"); 
			}
			else 
			{
				size += stream.WriteClass<LoadPreviousSegmentIds>(context, this.load_previous_segment_ids, "load_previous_segment_ids"); 
			}
			CodedLossless= 1;

			for ( segmentId = 0; segmentId < MAX_SEGMENTS; segmentId++ )
			{
				qindex= get_qindex( 1, segmentId );
				LosslessArray[ segmentId ]= qindex == 0 && DeltaQYDc == 0 && ? (uint)1 : (uint)0;
				DeltaQUAc= = 0 && DeltaQUDc == 0 && ? (uint)1 : (uint)0;
				DeltaQVAc= = 0 && DeltaQVDc == 0 ? (uint)1 : (uint)0;

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
			AllLossless= CodedLossless && ( FrameWidth == UpscaledWidth ) ? (uint)1 : (uint)0;
			size += stream.WriteClass<LoopFilterParams>(context, this.loop_filter_params, "loop_filter_params"); 
			size += stream.WriteClass<CdefParams>(context, this.cdef_params, "cdef_params"); 
			size += stream.WriteClass<LrParams>(context, this.lr_params, "lr_params"); 
			size += stream.WriteClass<ReadTxMode>(context, this.read_tx_mode, "read_tx_mode"); 
			size += stream.WriteClass<FrameReferenceMode>(context, this.frame_reference_mode, "frame_reference_mode"); 
			size += stream.WriteClass<SkipModeParams>(context, this.skip_mode_params, "skip_mode_params"); 

			if ( FrameIsIntra != 0 || error_resilient_mode != 0 || enable_warped_motion== 0 )
			{
				allow_warped_motion= 0;
			}
			else 
			{
				size += stream.WriteFixed(1, this.allow_warped_motion, "allow_warped_motion"); 
			}
			size += stream.WriteFixed(1, this.reduced_tx_set, "reduced_tx_set"); 
			size += stream.WriteClass<GlobalMotionParams>(context, this.global_motion_params, "global_motion_params"); 
			size += stream.WriteClass<FilmGrainParams>(context, this.film_grain_params, "film_grain_params"); 

            return size;
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
    public class GetRelativeDist : IAomSerializable
    {
		private uint a;
		public uint a { get { return a; } set { a = value; } }
		private uint b;
		public uint b { get { return b; } set { b = value; } }

         public GetRelativeDist(uint a, uint b)
         { 
			this.a = a;
			this.b = b;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint diff = 0;
			uint m = 0;

			if ( enable_order_hint== 0 )
			{
return 0;
			}
			diff= a - b;
			m= 1 << (OrderHintBits - 1);
			diff= (diff & (m - 1)) - (diff & m);
return diff;

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint diff = 0;
			uint m = 0;

			if ( enable_order_hint== 0 )
			{
return 0;
			}
			diff= a - b;
			m= 1 << (OrderHintBits - 1);
			diff= (diff & (m - 1)) - (diff & m);
return diff;

            return size;
         }

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
    public class MarkRefFrames : IAomSerializable
    {
		private uint idLen;
		public uint IdLen { get { return idLen; } set { idLen = value; } }

         public MarkRefFrames(uint idLen)
         { 
			this.idLen = idLen;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint diffLen = 0;
			uint i = 0;
			uint[] RefValid = null;
			diffLen= delta_frame_id_length_minus_2 + 2;

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

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint diffLen = 0;
			uint i = 0;
			uint[] RefValid = null;
			diffLen= delta_frame_id_length_minus_2 + 2;

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

            return size;
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
    public class FrameSize : IAomSerializable
    {
		private uint frame_width_minus_1;
		public uint FrameWidthMinus1 { get { return frame_width_minus_1; } set { frame_width_minus_1 = value; } }
		private uint frame_height_minus_1;
		public uint FrameHeightMinus1 { get { return frame_height_minus_1; } set { frame_height_minus_1 = value; } }
		private SuperresParams superres_params;
		public SuperresParams SuperresParams { get { return superres_params; } set { superres_params = value; } }
		private ComputeImageSize compute_image_size;
		public ComputeImageSize ComputeImageSize { get { return compute_image_size; } set { compute_image_size = value; } }

         public FrameSize()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint n = 0;
			uint FrameWidth = 0;
			uint FrameHeight = 0;

			if ( frame_size_override_flag != 0 )
			{
				n= frame_width_bits_minus_1 + 1;
				size += stream.ReadVariable(size, n, out this.frame_width_minus_1, "frame_width_minus_1"); 
				n= frame_height_bits_minus_1 + 1;
				size += stream.ReadVariable(size, n, out this.frame_height_minus_1, "frame_height_minus_1"); 
				FrameWidth= frame_width_minus_1 + 1;
				FrameHeight= frame_height_minus_1 + 1;
			}
			else 
			{
				FrameWidth= max_frame_width_minus_1 + 1;
				FrameHeight= max_frame_height_minus_1 + 1;
			}
			this.superres_params =  new SuperresParams() ;
			size +=  stream.ReadClass<SuperresParams>(size, context, this.superres_params, "superres_params"); 
			this.compute_image_size =  new ComputeImageSize() ;
			size +=  stream.ReadClass<ComputeImageSize>(size, context, this.compute_image_size, "compute_image_size"); 

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint n = 0;
			uint FrameWidth = 0;
			uint FrameHeight = 0;

			if ( frame_size_override_flag != 0 )
			{
				n= frame_width_bits_minus_1 + 1;
				size += stream.WriteVariable(n, this.frame_width_minus_1, "frame_width_minus_1"); 
				n= frame_height_bits_minus_1 + 1;
				size += stream.WriteVariable(n, this.frame_height_minus_1, "frame_height_minus_1"); 
				FrameWidth= frame_width_minus_1 + 1;
				FrameHeight= frame_height_minus_1 + 1;
			}
			else 
			{
				FrameWidth= max_frame_width_minus_1 + 1;
				FrameHeight= max_frame_height_minus_1 + 1;
			}
			size += stream.WriteClass<SuperresParams>(context, this.superres_params, "superres_params"); 
			size += stream.WriteClass<ComputeImageSize>(context, this.compute_image_size, "compute_image_size"); 

            return size;
         }

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
    public class RenderSize : IAomSerializable
    {
		private uint render_and_frame_size_different;
		public uint RenderAndFrameSizeDifferent { get { return render_and_frame_size_different; } set { render_and_frame_size_different = value; } }
		private uint render_width_minus_1;
		public uint RenderWidthMinus1 { get { return render_width_minus_1; } set { render_width_minus_1 = value; } }
		private uint render_height_minus_1;
		public uint RenderHeightMinus1 { get { return render_height_minus_1; } set { render_height_minus_1 = value; } }

         public RenderSize()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint RenderWidth = 0;
			uint RenderHeight = 0;
			size += stream.ReadFixed(size, 1, out this.render_and_frame_size_different, "render_and_frame_size_different"); 

			if ( render_and_frame_size_different == 1 )
			{
				size += stream.ReadFixed(size, 16, out this.render_width_minus_1, "render_width_minus_1"); 
				size += stream.ReadFixed(size, 16, out this.render_height_minus_1, "render_height_minus_1"); 
				RenderWidth= render_width_minus_1 + 1;
				RenderHeight= render_height_minus_1 + 1;
			}
			else 
			{
				RenderWidth= UpscaledWidth;
				RenderHeight= FrameHeight;
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint RenderWidth = 0;
			uint RenderHeight = 0;
			size += stream.WriteFixed(1, this.render_and_frame_size_different, "render_and_frame_size_different"); 

			if ( render_and_frame_size_different == 1 )
			{
				size += stream.WriteFixed(16, this.render_width_minus_1, "render_width_minus_1"); 
				size += stream.WriteFixed(16, this.render_height_minus_1, "render_height_minus_1"); 
				RenderWidth= render_width_minus_1 + 1;
				RenderHeight= render_height_minus_1 + 1;
			}
			else 
			{
				RenderWidth= UpscaledWidth;
				RenderHeight= FrameHeight;
			}

            return size;
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
    public class FrameSizeWithRefs : IAomSerializable
    {
		private uint[] found_ref;
		public uint[] FoundRef { get { return found_ref; } set { found_ref = value; } }
		private FrameSize frame_size;
		public FrameSize FrameSize { get { return frame_size; } set { frame_size = value; } }
		private RenderSize render_size;
		public RenderSize RenderSize { get { return render_size; } set { render_size = value; } }
		private SuperresParams superres_params;
		public SuperresParams SuperresParams { get { return superres_params; } set { superres_params = value; } }
		private ComputeImageSize compute_image_size;
		public ComputeImageSize ComputeImageSize { get { return compute_image_size; } set { compute_image_size = value; } }

         public FrameSizeWithRefs()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint i = 0;
			uint UpscaledWidth = 0;
			uint FrameWidth = 0;
			uint FrameHeight = 0;
			uint RenderWidth = 0;
			uint RenderHeight = 0;

			this.found_ref = new uint[ REFS_PER_FRAME];
			for ( i = 0; i < REFS_PER_FRAME; i++ )
			{
				size += stream.ReadFixed(size, 1, out this.found_ref[ i ], "found_ref"); 

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
				this.frame_size =  new FrameSize() ;
				size +=  stream.ReadClass<FrameSize>(size, context, this.frame_size, "frame_size"); 
				this.render_size =  new RenderSize() ;
				size +=  stream.ReadClass<RenderSize>(size, context, this.render_size, "render_size"); 
			}
			else 
			{
				this.superres_params =  new SuperresParams() ;
				size +=  stream.ReadClass<SuperresParams>(size, context, this.superres_params, "superres_params"); 
				this.compute_image_size =  new ComputeImageSize() ;
				size +=  stream.ReadClass<ComputeImageSize>(size, context, this.compute_image_size, "compute_image_size"); 
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint i = 0;
			uint UpscaledWidth = 0;
			uint FrameWidth = 0;
			uint FrameHeight = 0;
			uint RenderWidth = 0;
			uint RenderHeight = 0;

			for ( i = 0; i < REFS_PER_FRAME; i++ )
			{
				size += stream.WriteFixed(1, this.found_ref[ i ], "found_ref"); 

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
				size += stream.WriteClass<FrameSize>(context, this.frame_size, "frame_size"); 
				size += stream.WriteClass<RenderSize>(context, this.render_size, "render_size"); 
			}
			else 
			{
				size += stream.WriteClass<SuperresParams>(context, this.superres_params, "superres_params"); 
				size += stream.WriteClass<ComputeImageSize>(context, this.compute_image_size, "compute_image_size"); 
			}

            return size;
         }

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
    public class SuperresParams : IAomSerializable
    {
		private uint use_superres;
		public uint UseSuperres { get { return use_superres; } set { use_superres = value; } }
		private uint coded_denom;
		public uint CodedDenom { get { return coded_denom; } set { coded_denom = value; } }

         public SuperresParams()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint use_superres = 0;
			uint SuperresDenom = 0;
			uint UpscaledWidth = 0;
			uint FrameWidth = 0;

			if ( enable_superres != 0 )
			{
				size += stream.ReadFixed(size, 1, out this.use_superres, "use_superres"); 
			}
			else 
			{
				use_superres= 0;
			}

			if ( use_superres != 0 )
			{
				size += stream.ReadVariable(size, SUPERRES_DENOM_BITS, out this.coded_denom, "coded_denom"); 
				SuperresDenom= coded_denom + SUPERRES_DENOM_MIN;
			}
			else 
			{
				SuperresDenom= SUPERRES_NUM;
			}
			UpscaledWidth= FrameWidth;
			FrameWidth= (UpscaledWidth * SUPERRES_NUM + (SuperresDenom / 2)) / SuperresDenom;

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint use_superres = 0;
			uint SuperresDenom = 0;
			uint UpscaledWidth = 0;
			uint FrameWidth = 0;

			if ( enable_superres != 0 )
			{
				size += stream.WriteFixed(1, this.use_superres, "use_superres"); 
			}
			else 
			{
				use_superres= 0;
			}

			if ( use_superres != 0 )
			{
				size += stream.WriteVariable(SUPERRES_DENOM_BITS, this.coded_denom, "coded_denom"); 
				SuperresDenom= coded_denom + SUPERRES_DENOM_MIN;
			}
			else 
			{
				SuperresDenom= SUPERRES_NUM;
			}
			UpscaledWidth= FrameWidth;
			FrameWidth= (UpscaledWidth * SUPERRES_NUM + (SuperresDenom / 2)) / SuperresDenom;

            return size;
         }

    }

    /*



compute_image_size() { 
 MiCols = 2 * ( ( FrameWidth + 7 ) >> 3 )
 MiRows = 2 * ( ( FrameHeight + 7 ) >> 3 )
 }
    */
    public class ComputeImageSize : IAomSerializable
    {

         public ComputeImageSize()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint MiCols = 0;
			uint MiRows = 0;
			MiCols= 2 * ( ( FrameWidth + 7 ) >> 3 );
			MiRows= 2 * ( ( FrameHeight + 7 ) >> 3 );

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint MiCols = 0;
			uint MiRows = 0;
			MiCols= 2 * ( ( FrameWidth + 7 ) >> 3 );
			MiRows= 2 * ( ( FrameHeight + 7 ) >> 3 );

            return size;
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
    public class ReadInterpolationFilter : IAomSerializable
    {
		private uint is_filter_switchable;
		public uint IsFilterSwitchable { get { return is_filter_switchable; } set { is_filter_switchable = value; } }
		private uint interpolation_filter;
		public uint InterpolationFilter { get { return interpolation_filter; } set { interpolation_filter = value; } }

         public ReadInterpolationFilter()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint interpolation_filter = 0;
			size += stream.ReadFixed(size, 1, out this.is_filter_switchable, "is_filter_switchable"); 

			if ( is_filter_switchable == 1 )
			{
				interpolation_filter= SWITCHABLE;
			}
			else 
			{
				size += stream.ReadFixed(size, 2, out this.interpolation_filter, "interpolation_filter"); 
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint interpolation_filter = 0;
			size += stream.WriteFixed(1, this.is_filter_switchable, "is_filter_switchable"); 

			if ( is_filter_switchable == 1 )
			{
				interpolation_filter= SWITCHABLE;
			}
			else 
			{
				size += stream.WriteFixed(2, this.interpolation_filter, "interpolation_filter"); 
			}

            return size;
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
    public class LoopFilterParams : IAomSerializable
    {
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

         public LoopFilterParams()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint[] loop_filter_level = null;
			uint[] loop_filter_ref_deltas = null;
			uint i = 0;
			uint[] loop_filter_mode_deltas = null;

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
			size += stream.ReadFixed(size, 6, out this.loop_filter_level[ 0 ], "loop_filter_level"); 
			size += stream.ReadFixed(size, 6, out this.loop_filter_level[ 1 ], "loop_filter_level"); 

			if ( NumPlanes > 1 )
			{

				if ( loop_filter_level[ 0 ] != 0 || loop_filter_level[ 1 ] != 0 )
				{
					size += stream.ReadFixed(size, 6, out this.loop_filter_level[ 2 ], "loop_filter_level"); 
					size += stream.ReadFixed(size, 6, out this.loop_filter_level[ 3 ], "loop_filter_level"); 
				}
			}
			size += stream.ReadFixed(size, 3, out this.loop_filter_sharpness, "loop_filter_sharpness"); 
			size += stream.ReadFixed(size, 1, out this.loop_filter_delta_enabled, "loop_filter_delta_enabled"); 

			if ( loop_filter_delta_enabled == 1 )
			{
				size += stream.ReadFixed(size, 1, out this.loop_filter_delta_update, "loop_filter_delta_update"); 

				if ( loop_filter_delta_update == 1 )
				{

					this.update_ref_delta = new uint[ TOTAL_REFS_PER_FRAME];
					this.loop_filter_ref_deltas = new uint[ TOTAL_REFS_PER_FRAME];
					for ( i = 0; i < TOTAL_REFS_PER_FRAME; i++ )
					{
						size += stream.ReadFixed(size, 1, out this.update_ref_delta[ i ], "update_ref_delta"); 

						if ( update_ref_delta[i] == 1 )
						{
							size += stream.ReadSignedIntVar(size, 1+6, out this.loop_filter_ref_deltas[ i ], "loop_filter_ref_deltas"); 
						}
					}

					this.update_mode_delta = new uint[ 2];
					this.loop_filter_mode_deltas = new uint[ 2];
					for ( i = 0; i < 2; i++ )
					{
						size += stream.ReadFixed(size, 1, out this.update_mode_delta[ i ], "update_mode_delta"); 

						if ( update_mode_delta[i] == 1 )
						{
							size += stream.ReadSignedIntVar(size, 1+6, out this.loop_filter_mode_deltas[ i ], "loop_filter_mode_deltas"); 
						}
					}
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint[] loop_filter_level = null;
			uint[] loop_filter_ref_deltas = null;
			uint i = 0;
			uint[] loop_filter_mode_deltas = null;

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
			size += stream.WriteFixed(6, this.loop_filter_level[ 0 ], "loop_filter_level"); 
			size += stream.WriteFixed(6, this.loop_filter_level[ 1 ], "loop_filter_level"); 

			if ( NumPlanes > 1 )
			{

				if ( loop_filter_level[ 0 ] != 0 || loop_filter_level[ 1 ] != 0 )
				{
					size += stream.WriteFixed(6, this.loop_filter_level[ 2 ], "loop_filter_level"); 
					size += stream.WriteFixed(6, this.loop_filter_level[ 3 ], "loop_filter_level"); 
				}
			}
			size += stream.WriteFixed(3, this.loop_filter_sharpness, "loop_filter_sharpness"); 
			size += stream.WriteFixed(1, this.loop_filter_delta_enabled, "loop_filter_delta_enabled"); 

			if ( loop_filter_delta_enabled == 1 )
			{
				size += stream.WriteFixed(1, this.loop_filter_delta_update, "loop_filter_delta_update"); 

				if ( loop_filter_delta_update == 1 )
				{

					for ( i = 0; i < TOTAL_REFS_PER_FRAME; i++ )
					{
						size += stream.WriteFixed(1, this.update_ref_delta[ i ], "update_ref_delta"); 

						if ( update_ref_delta[i] == 1 )
						{
							size += stream.WriteSignedIntVar(1+6, this.loop_filter_ref_deltas[ i ], "loop_filter_ref_deltas"); 
						}
					}

					for ( i = 0; i < 2; i++ )
					{
						size += stream.WriteFixed(1, this.update_mode_delta[ i ], "update_mode_delta"); 

						if ( update_mode_delta[i] == 1 )
						{
							size += stream.WriteSignedIntVar(1+6, this.loop_filter_mode_deltas[ i ], "loop_filter_mode_deltas"); 
						}
					}
				}
			}

            return size;
         }

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
    public class QuantizationParams : IAomSerializable
    {
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

         public QuantizationParams()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint DeltaQYDc = 0;
			uint diff_uv_delta = 0;
			uint DeltaQUDc = 0;
			uint DeltaQUAc = 0;
			uint DeltaQVDc = 0;
			uint DeltaQVAc = 0;
			uint qm_v = 0;
			size += stream.ReadFixed(size, 8, out this.base_q_idx, "base_q_idx"); 
			DeltaQYDc= read_delta_q();

			if ( NumPlanes > 1 )
			{

				if ( separate_uv_delta_q != 0 )
				{
					size += stream.ReadFixed(size, 1, out this.diff_uv_delta, "diff_uv_delta"); 
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
			size += stream.ReadFixed(size, 1, out this.using_qmatrix, "using_qmatrix"); 

			if ( using_qmatrix != 0 )
			{
				size += stream.ReadFixed(size, 4, out this.qm_y, "qm_y"); 
				size += stream.ReadFixed(size, 4, out this.qm_u, "qm_u"); 

				if ( separate_uv_delta_q== 0 )
				{
					qm_v= qm_u;
				}
				else 
				{
					size += stream.ReadFixed(size, 4, out this.qm_v, "qm_v"); 
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint DeltaQYDc = 0;
			uint diff_uv_delta = 0;
			uint DeltaQUDc = 0;
			uint DeltaQUAc = 0;
			uint DeltaQVDc = 0;
			uint DeltaQVAc = 0;
			uint qm_v = 0;
			size += stream.WriteFixed(8, this.base_q_idx, "base_q_idx"); 
			DeltaQYDc= read_delta_q();

			if ( NumPlanes > 1 )
			{

				if ( separate_uv_delta_q != 0 )
				{
					size += stream.WriteFixed(1, this.diff_uv_delta, "diff_uv_delta"); 
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
			size += stream.WriteFixed(1, this.using_qmatrix, "using_qmatrix"); 

			if ( using_qmatrix != 0 )
			{
				size += stream.WriteFixed(4, this.qm_y, "qm_y"); 
				size += stream.WriteFixed(4, this.qm_u, "qm_u"); 

				if ( separate_uv_delta_q== 0 )
				{
					qm_v= qm_u;
				}
				else 
				{
					size += stream.WriteFixed(4, this.qm_v, "qm_v"); 
				}
			}

            return size;
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
    public class ReadDeltaq : IAomSerializable
    {
		private uint delta_coded;
		public uint DeltaCoded { get { return delta_coded; } set { delta_coded = value; } }
		private uint delta_q;
		public uint Deltaq { get { return delta_q; } set { delta_q = value; } }

         public ReadDeltaq()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint delta_q = 0;
			size += stream.ReadFixed(size, 1, out this.delta_coded, "delta_coded"); 

			if ( delta_coded != 0 )
			{
				size += stream.ReadSignedIntVar(size, 1+6, out this.delta_q, "delta_q"); 
			}
			else 
			{
				delta_q= 0;
			}
return delta_q;

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint delta_q = 0;
			size += stream.WriteFixed(1, this.delta_coded, "delta_coded"); 

			if ( delta_coded != 0 )
			{
				size += stream.WriteSignedIntVar(1+6, this.delta_q, "delta_q"); 
			}
			else 
			{
				delta_q= 0;
			}
return delta_q;

            return size;
         }

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
    public class SegmentationParams : IAomSerializable
    {
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
		private FeatureValue[][] feature_value;
		public FeatureValue[][] FeatureValue { get { return feature_value; } set { feature_value = value; } }

         public SegmentationParams()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint segmentation_update_map = 0;
			uint segmentation_temporal_update = 0;
			uint segmentation_update_data = 0;
			uint i = 0;
			uint j = 0;
			uint feature_value = 0;
			uint[][] FeatureEnabled = null;
			uint clippedValue = 0;
			uint bitsToRead = 0;
			uint limit = 0;
			uint[][] FeatureData = null;
			uint SegIdPreSkip = 0;
			uint LastActiveSegId = 0;
			size += stream.ReadFixed(size, 1, out this.segmentation_enabled, "segmentation_enabled"); 

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
					size += stream.ReadFixed(size, 1, out this.segmentation_update_map, "segmentation_update_map"); 

					if ( segmentation_update_map == 1 )
					{
						size += stream.ReadFixed(size, 1, out this.segmentation_temporal_update, "segmentation_temporal_update"); 
					}
					size += stream.ReadFixed(size, 1, out this.segmentation_update_data, "segmentation_update_data"); 
				}

				if ( segmentation_update_data == 1 )
				{

					this.feature_enabled = new uint[ MAX_SEGMENTS][];
					this.feature_value = new FeatureValue[ MAX_SEGMENTS][];
					for ( i = 0; i < MAX_SEGMENTS; i++ )
					{

						this.feature_enabled[ i ] = new uint[ SEG_LVL_MAX];
						this.feature_value[ i ] = new FeatureValue[ SEG_LVL_MAX];
						for ( j = 0; j < SEG_LVL_MAX; j++ )
						{
							feature_value= 0;
							size += stream.ReadFixed(size, 1, out this.feature_enabled[ i ][ j ], "feature_enabled"); 
							FeatureEnabled[ i ][ j ]= feature_enabled[i][j];
							clippedValue= 0;

							if ( feature_enabled[i][j] == 1 )
							{
								bitsToRead= Segmentation_Feature_Bits[ j ];
								limit= Segmentation_Feature_Max[ j ];

								if ( Segmentation_Feature_Signed[ j ] == 1 )
								{
									this.feature_value[ i ][ j ] =  new FeatureValue() ;
									size +=  stream.ReadClass<FeatureValue>(size, context, this.feature_value[ i ][ j ], "feature_value"); 
									size += stream.ReadSignedIntVar(size, 1+bitsToRead, out this.feature_value[ i ][ j ], "feature_value"); 
									clippedValue= Clip3( -limit, limit, feature_value[i][j]);
								}
								else 
								{
									this.feature_value[ i ][ j ] =  new FeatureValue() ;
									size +=  stream.ReadClass<FeatureValue>(size, context, this.feature_value[ i ][ j ], "feature_value"); 
									size += stream.ReadVariable(size, bitsToRead, out this.feature_value[ i ][ j ], "feature_value"); 
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
			SegIdPreSkip= 0;
			LastActiveSegId= 0;

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

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint segmentation_update_map = 0;
			uint segmentation_temporal_update = 0;
			uint segmentation_update_data = 0;
			uint i = 0;
			uint j = 0;
			uint feature_value = 0;
			uint[][] FeatureEnabled = null;
			uint clippedValue = 0;
			uint bitsToRead = 0;
			uint limit = 0;
			uint[][] FeatureData = null;
			uint SegIdPreSkip = 0;
			uint LastActiveSegId = 0;
			size += stream.WriteFixed(1, this.segmentation_enabled, "segmentation_enabled"); 

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
					size += stream.WriteFixed(1, this.segmentation_update_map, "segmentation_update_map"); 

					if ( segmentation_update_map == 1 )
					{
						size += stream.WriteFixed(1, this.segmentation_temporal_update, "segmentation_temporal_update"); 
					}
					size += stream.WriteFixed(1, this.segmentation_update_data, "segmentation_update_data"); 
				}

				if ( segmentation_update_data == 1 )
				{

					for ( i = 0; i < MAX_SEGMENTS; i++ )
					{

						for ( j = 0; j < SEG_LVL_MAX; j++ )
						{
							feature_value= 0;
							size += stream.WriteFixed(1, this.feature_enabled[ i ][ j ], "feature_enabled"); 
							FeatureEnabled[ i ][ j ]= feature_enabled[i][j];
							clippedValue= 0;

							if ( feature_enabled[i][j] == 1 )
							{
								bitsToRead= Segmentation_Feature_Bits[ j ];
								limit= Segmentation_Feature_Max[ j ];

								if ( Segmentation_Feature_Signed[ j ] == 1 )
								{
									size += stream.WriteClass<FeatureValue>(context, this.feature_value[ i ][ j ], "feature_value"); 
									size += stream.WriteSignedIntVar(1+bitsToRead, this.feature_value[ i ][ j ], "feature_value"); 
									clippedValue= Clip3( -limit, limit, feature_value[i][j]);
								}
								else 
								{
									size += stream.WriteClass<FeatureValue>(context, this.feature_value[ i ][ j ], "feature_value"); 
									size += stream.WriteVariable(bitsToRead, this.feature_value[ i ][ j ], "feature_value"); 
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
			SegIdPreSkip= 0;
			LastActiveSegId= 0;

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

            return size;
         }

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
    public class TileInfo : IAomSerializable
    {
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

         public TileInfo()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint sbCols = 0;
			uint sbRows = 0;
			uint sbShift = 0;
			uint sbSize = 0;
			uint maxTileWidthSb = 0;
			uint maxTileAreaSb = 0;
			uint minLog2TileCols = 0;
			uint maxLog2TileCols = 0;
			uint maxLog2TileRows = 0;
			uint minLog2Tiles = 0;
			uint TileColsLog2 = 0;
			int whileIndex = -1;
			uint tileWidthSb = 0;
			uint i = 0;
			uint startSb = 0;
			uint[] MiColStarts = null;
			uint TileCols = 0;
			uint minLog2TileRows = 0;
			uint TileRowsLog2 = 0;
			uint tileHeightSb = 0;
			uint[] MiRowStarts = null;
			uint TileRows = 0;
			uint widestTileSb = 0;
			uint maxWidth = 0;
			uint sizeSb = 0;
			uint maxTileHeightSb = 0;
			uint maxHeight = 0;
			uint TileSizeBytes = 0;
			uint context_update_tile_id = 0;
			sbCols= use_128x128_superblock ? ( ( MiCols + 31 ) >> 5 ) : ( ( MiCols + 15 ) >> 4 );
			sbRows= use_128x128_superblock ? ( ( MiRows + 31 ) >> 5 ) : ( ( MiRows + 15 ) >> 4 );
			sbShift= use_128x128_superblock ? 5 : 4;
			sbSize= sbShift + 2;
			maxTileWidthSb= MAX_TILE_WIDTH >> sbSize;
			maxTileAreaSb= MAX_TILE_AREA >> ( 2 * sbSize );
			minLog2TileCols= tile_log2(maxTileWidthSb, sbCols);
			maxLog2TileCols= tile_log2(1, Math.Min(sbCols, MAX_TILE_COLS));
			maxLog2TileRows= tile_log2(1, Math.Min(sbRows, MAX_TILE_ROWS));
			minLog2Tiles= Math.Max(minLog2TileCols, tile_log2(maxTileAreaSb, sbRows * sbCols));
			size += stream.ReadFixed(size, 1, out this.uniform_tile_spacing_flag, "uniform_tile_spacing_flag"); 

			if ( uniform_tile_spacing_flag != 0 )
			{
				TileColsLog2= minLog2TileCols;

				while ( TileColsLog2 < maxLog2TileCols )
				{
					whileIndex++;

					size += stream.ReadFixed(size, 1, whileIndex, this.increment_tile_cols_log2, "increment_tile_cols_log2"); 

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

					size += stream.ReadFixed(size, 1, whileIndex, this.increment_tile_rows_log2, "increment_tile_rows_log2"); 

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
					size += stream.ReadUnsignedInt(size, maxWidth, out this.width_in_sbs_minus_1[ i ], "width_in_sbs_minus_1"); 
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
					size += stream.ReadUnsignedInt(size, maxHeight, out this.height_in_sbs_minus_1[ i ], "height_in_sbs_minus_1"); 
					sizeSb= height_in_sbs_minus_1[i] + 1;
					startSb+= sizeSb;
				}
				MiRowStarts[ i ]= MiRows;
				TileRows= i;
				TileRowsLog2= tile_log2(1, TileRows);
			}

			if ( TileColsLog2 > 0 || TileRowsLog2 > 0 )
			{
				size += stream.ReadVariable(size, TileRowsLog2+TileColsLog2, out this.context_update_tile_id, "context_update_tile_id"); 
				size += stream.ReadFixed(size, 2, out this.tile_size_bytes_minus_1, "tile_size_bytes_minus_1"); 
				TileSizeBytes= tile_size_bytes_minus_1 + 1;
			}
			else 
			{
				context_update_tile_id= 0;
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint sbCols = 0;
			uint sbRows = 0;
			uint sbShift = 0;
			uint sbSize = 0;
			uint maxTileWidthSb = 0;
			uint maxTileAreaSb = 0;
			uint minLog2TileCols = 0;
			uint maxLog2TileCols = 0;
			uint maxLog2TileRows = 0;
			uint minLog2Tiles = 0;
			uint TileColsLog2 = 0;
			int whileIndex = -1;
			uint tileWidthSb = 0;
			uint i = 0;
			uint startSb = 0;
			uint[] MiColStarts = null;
			uint TileCols = 0;
			uint minLog2TileRows = 0;
			uint TileRowsLog2 = 0;
			uint tileHeightSb = 0;
			uint[] MiRowStarts = null;
			uint TileRows = 0;
			uint widestTileSb = 0;
			uint maxWidth = 0;
			uint sizeSb = 0;
			uint maxTileHeightSb = 0;
			uint maxHeight = 0;
			uint TileSizeBytes = 0;
			uint context_update_tile_id = 0;
			sbCols= use_128x128_superblock ? ( ( MiCols + 31 ) >> 5 ) : ( ( MiCols + 15 ) >> 4 );
			sbRows= use_128x128_superblock ? ( ( MiRows + 31 ) >> 5 ) : ( ( MiRows + 15 ) >> 4 );
			sbShift= use_128x128_superblock ? 5 : 4;
			sbSize= sbShift + 2;
			maxTileWidthSb= MAX_TILE_WIDTH >> sbSize;
			maxTileAreaSb= MAX_TILE_AREA >> ( 2 * sbSize );
			minLog2TileCols= tile_log2(maxTileWidthSb, sbCols);
			maxLog2TileCols= tile_log2(1, Math.Min(sbCols, MAX_TILE_COLS));
			maxLog2TileRows= tile_log2(1, Math.Min(sbRows, MAX_TILE_ROWS));
			minLog2Tiles= Math.Max(minLog2TileCols, tile_log2(maxTileAreaSb, sbRows * sbCols));
			size += stream.WriteFixed(1, this.uniform_tile_spacing_flag, "uniform_tile_spacing_flag"); 

			if ( uniform_tile_spacing_flag != 0 )
			{
				TileColsLog2= minLog2TileCols;

				while ( TileColsLog2 < maxLog2TileCols )
				{
					whileIndex++;

					size += stream.WriteFixed(1, whileIndex, this.increment_tile_cols_log2, "increment_tile_cols_log2"); 

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

					size += stream.WriteFixed(1, whileIndex, this.increment_tile_rows_log2, "increment_tile_rows_log2"); 

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
					size += stream.WriteUnsignedInt(maxWidth, this.width_in_sbs_minus_1[ i ], "width_in_sbs_minus_1"); 
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
					size += stream.WriteUnsignedInt(maxHeight, this.height_in_sbs_minus_1[ i ], "height_in_sbs_minus_1"); 
					sizeSb= height_in_sbs_minus_1[i] + 1;
					startSb+= sizeSb;
				}
				MiRowStarts[ i ]= MiRows;
				TileRows= i;
				TileRowsLog2= tile_log2(1, TileRows);
			}

			if ( TileColsLog2 > 0 || TileRowsLog2 > 0 )
			{
				size += stream.WriteVariable(TileRowsLog2+TileColsLog2, this.context_update_tile_id, "context_update_tile_id"); 
				size += stream.WriteFixed(2, this.tile_size_bytes_minus_1, "tile_size_bytes_minus_1"); 
				TileSizeBytes= tile_size_bytes_minus_1 + 1;
			}
			else 
			{
				context_update_tile_id= 0;
			}

            return size;
         }

    }

    /*



tile_log2( blkSize, target ) { 
 for ( k = 0; (blkSize << k) < target; k++ ) {
 }
 return k
 }
    */
    public class TileLog2 : IAomSerializable
    {
		private uint blkSize;
		public uint BlkSize { get { return blkSize; } set { blkSize = value; } }
		private uint target;
		public uint Target { get { return target; } set { target = value; } }

         public TileLog2(uint blkSize, uint target)
         { 
			this.blkSize = blkSize;
			this.target = target;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint k = 0;

			for ( k = 0; (blkSize << (int) k) < target; k++ )
			{
			}
return k;

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint k = 0;

			for ( k = 0; (blkSize << (int) k) < target; k++ )
			{
			}
return k;

            return size;
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
    public class DeltaqParams : IAomSerializable
    {
		private uint delta_q_present;
		public uint DeltaqPresent { get { return delta_q_present; } set { delta_q_present = value; } }
		private uint delta_q_res;
		public uint DeltaqRes { get { return delta_q_res; } set { delta_q_res = value; } }

         public DeltaqParams()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint delta_q_res = 0;
			uint delta_q_present = 0;
			delta_q_res= 0;
			delta_q_present= 0;

			if ( base_q_idx > 0 )
			{
				size += stream.ReadFixed(size, 1, out this.delta_q_present, "delta_q_present"); 
			}

			if ( delta_q_present != 0 )
			{
				size += stream.ReadFixed(size, 2, out this.delta_q_res, "delta_q_res"); 
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint delta_q_res = 0;
			uint delta_q_present = 0;
			delta_q_res= 0;
			delta_q_present= 0;

			if ( base_q_idx > 0 )
			{
				size += stream.WriteFixed(1, this.delta_q_present, "delta_q_present"); 
			}

			if ( delta_q_present != 0 )
			{
				size += stream.WriteFixed(2, this.delta_q_res, "delta_q_res"); 
			}

            return size;
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
    public class DeltaLfParams : IAomSerializable
    {
		private uint delta_lf_present;
		public uint DeltaLfPresent { get { return delta_lf_present; } set { delta_lf_present = value; } }
		private uint delta_lf_res;
		public uint DeltaLfRes { get { return delta_lf_res; } set { delta_lf_res = value; } }
		private uint delta_lf_multi;
		public uint DeltaLfMulti { get { return delta_lf_multi; } set { delta_lf_multi = value; } }

         public DeltaLfParams()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint delta_lf_present = 0;
			uint delta_lf_res = 0;
			uint delta_lf_multi = 0;
			delta_lf_present= 0;
			delta_lf_res= 0;
			delta_lf_multi= 0;

			if ( delta_q_present != 0 )
			{

				if ( allow_intrabc== 0 )
				{
					size += stream.ReadFixed(size, 1, out this.delta_lf_present, "delta_lf_present"); 
				}

				if ( delta_lf_present != 0 )
				{
					size += stream.ReadFixed(size, 2, out this.delta_lf_res, "delta_lf_res"); 
					size += stream.ReadFixed(size, 1, out this.delta_lf_multi, "delta_lf_multi"); 
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint delta_lf_present = 0;
			uint delta_lf_res = 0;
			uint delta_lf_multi = 0;
			delta_lf_present= 0;
			delta_lf_res= 0;
			delta_lf_multi= 0;

			if ( delta_q_present != 0 )
			{

				if ( allow_intrabc== 0 )
				{
					size += stream.WriteFixed(1, this.delta_lf_present, "delta_lf_present"); 
				}

				if ( delta_lf_present != 0 )
				{
					size += stream.WriteFixed(2, this.delta_lf_res, "delta_lf_res"); 
					size += stream.WriteFixed(1, this.delta_lf_multi, "delta_lf_multi"); 
				}
			}

            return size;
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
    public class CdefParams : IAomSerializable
    {
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

         public CdefParams()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint cdef_bits = 0;
			uint[] cdef_y_pri_strength = null;
			uint[] cdef_y_sec_strength = null;
			uint[] cdef_uv_pri_strength = null;
			uint[] cdef_uv_sec_strength = null;
			uint CdefDamping = 0;
			uint i = 0;

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
			size += stream.ReadFixed(size, 2, out this.cdef_damping_minus_3, "cdef_damping_minus_3"); 
			CdefDamping= cdef_damping_minus_3 + 3;
			size += stream.ReadFixed(size, 2, out this.cdef_bits, "cdef_bits"); 

			this.cdef_y_pri_strength = new uint[ (1 << (int) cdef_bits)];
			this.cdef_y_sec_strength = new uint[ (1 << (int) cdef_bits)];
			this.cdef_uv_pri_strength = new uint[ (1 << (int) cdef_bits)];
			this.cdef_uv_sec_strength = new uint[ (1 << (int) cdef_bits)];
			for ( i = 0; i < (1 << (int) cdef_bits); i++ )
			{
				size += stream.ReadFixed(size, 4, out this.cdef_y_pri_strength[i], "cdef_y_pri_strength"); 
				size += stream.ReadFixed(size, 2, out this.cdef_y_sec_strength[i], "cdef_y_sec_strength"); 

				if ( cdef_y_sec_strength[i] == 3 )
				{
					cdef_y_sec_strength[i]+= 1;
				}

				if ( NumPlanes > 1 )
				{
					size += stream.ReadFixed(size, 4, out this.cdef_uv_pri_strength[i], "cdef_uv_pri_strength"); 
					size += stream.ReadFixed(size, 2, out this.cdef_uv_sec_strength[i], "cdef_uv_sec_strength"); 

					if ( cdef_uv_sec_strength[i] == 3 )
					{
						cdef_uv_sec_strength[i]+= 1;
					}
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint cdef_bits = 0;
			uint[] cdef_y_pri_strength = null;
			uint[] cdef_y_sec_strength = null;
			uint[] cdef_uv_pri_strength = null;
			uint[] cdef_uv_sec_strength = null;
			uint CdefDamping = 0;
			uint i = 0;

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
			size += stream.WriteFixed(2, this.cdef_damping_minus_3, "cdef_damping_minus_3"); 
			CdefDamping= cdef_damping_minus_3 + 3;
			size += stream.WriteFixed(2, this.cdef_bits, "cdef_bits"); 

			for ( i = 0; i < (1 << (int) cdef_bits); i++ )
			{
				size += stream.WriteFixed(4, this.cdef_y_pri_strength[i], "cdef_y_pri_strength"); 
				size += stream.WriteFixed(2, this.cdef_y_sec_strength[i], "cdef_y_sec_strength"); 

				if ( cdef_y_sec_strength[i] == 3 )
				{
					cdef_y_sec_strength[i]+= 1;
				}

				if ( NumPlanes > 1 )
				{
					size += stream.WriteFixed(4, this.cdef_uv_pri_strength[i], "cdef_uv_pri_strength"); 
					size += stream.WriteFixed(2, this.cdef_uv_sec_strength[i], "cdef_uv_sec_strength"); 

					if ( cdef_uv_sec_strength[i] == 3 )
					{
						cdef_uv_sec_strength[i]+= 1;
					}
				}
			}

            return size;
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
    public class LrParams : IAomSerializable
    {
		private uint[] lr_type;
		public uint[] LrType { get { return lr_type; } set { lr_type = value; } }
		private uint lr_unit_shift;
		public uint LrUnitShift { get { return lr_unit_shift; } set { lr_unit_shift = value; } }
		private uint lr_unit_extra_shift;
		public uint LrUnitExtraShift { get { return lr_unit_extra_shift; } set { lr_unit_extra_shift = value; } }
		private uint lr_uv_shift;
		public uint LrUvShift { get { return lr_uv_shift; } set { lr_uv_shift = value; } }

         public LrParams()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint[] FrameRestorationType = null;
			uint UsesLr = 0;
			uint usesChromaLr = 0;
			uint i = 0;
			uint lr_unit_shift = 0;
			uint[] LoopRestorationSize = null;
			uint lr_uv_shift = 0;

			if ( AllLossless != 0 || allow_intrabc != 0 ||
 enable_restoration== 0 )
			{
				FrameRestorationType[0]= RESTORE_NONE;
				FrameRestorationType[1]= RESTORE_NONE;
				FrameRestorationType[2]= RESTORE_NONE;
				UsesLr= 0;
return;
			}
			UsesLr= 0;
			usesChromaLr= 0;

			this.lr_type = new uint[ NumPlanes];
			for ( i = 0; i < NumPlanes; i++ )
			{
				size += stream.ReadFixed(size, 2, out this.lr_type[ i ], "lr_type"); 
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
					size += stream.ReadFixed(size, 1, out this.lr_unit_shift, "lr_unit_shift"); 
					lr_unit_shift++;
				}
				else 
				{
					size += stream.ReadFixed(size, 1, out this.lr_unit_shift, "lr_unit_shift"); 

					if ( lr_unit_shift != 0 )
					{
						size += stream.ReadFixed(size, 1, out this.lr_unit_extra_shift, "lr_unit_extra_shift"); 
						lr_unit_shift+= lr_unit_extra_shift;
					}
				}
				LoopRestorationSize[ 0 ]= RESTORATION_TILESIZE_MAX >> (2 - lr_unit_shift);

				if ( subsampling_x != 0 && subsampling_y != 0 && usesChromaLr != 0 )
				{
					size += stream.ReadFixed(size, 1, out this.lr_uv_shift, "lr_uv_shift"); 
				}
				else 
				{
					lr_uv_shift= 0;
				}
				LoopRestorationSize[ 1 ]= LoopRestorationSize[ 0 ] >> lr_uv_shift;
				LoopRestorationSize[ 2 ]= LoopRestorationSize[ 0 ] >> lr_uv_shift;
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint[] FrameRestorationType = null;
			uint UsesLr = 0;
			uint usesChromaLr = 0;
			uint i = 0;
			uint lr_unit_shift = 0;
			uint[] LoopRestorationSize = null;
			uint lr_uv_shift = 0;

			if ( AllLossless != 0 || allow_intrabc != 0 ||
 enable_restoration== 0 )
			{
				FrameRestorationType[0]= RESTORE_NONE;
				FrameRestorationType[1]= RESTORE_NONE;
				FrameRestorationType[2]= RESTORE_NONE;
				UsesLr= 0;
return;
			}
			UsesLr= 0;
			usesChromaLr= 0;

			for ( i = 0; i < NumPlanes; i++ )
			{
				size += stream.WriteFixed(2, this.lr_type[ i ], "lr_type"); 
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
					size += stream.WriteFixed(1, this.lr_unit_shift, "lr_unit_shift"); 
					lr_unit_shift++;
				}
				else 
				{
					size += stream.WriteFixed(1, this.lr_unit_shift, "lr_unit_shift"); 

					if ( lr_unit_shift != 0 )
					{
						size += stream.WriteFixed(1, this.lr_unit_extra_shift, "lr_unit_extra_shift"); 
						lr_unit_shift+= lr_unit_extra_shift;
					}
				}
				LoopRestorationSize[ 0 ]= RESTORATION_TILESIZE_MAX >> (2 - lr_unit_shift);

				if ( subsampling_x != 0 && subsampling_y != 0 && usesChromaLr != 0 )
				{
					size += stream.WriteFixed(1, this.lr_uv_shift, "lr_uv_shift"); 
				}
				else 
				{
					lr_uv_shift= 0;
				}
				LoopRestorationSize[ 1 ]= LoopRestorationSize[ 0 ] >> lr_uv_shift;
				LoopRestorationSize[ 2 ]= LoopRestorationSize[ 0 ] >> lr_uv_shift;
			}

            return size;
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
    public class ReadTxMode : IAomSerializable
    {
		private uint tx_mode_select;
		public uint TxModeSelect { get { return tx_mode_select; } set { tx_mode_select = value; } }

         public ReadTxMode()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint TxMode = 0;

			if ( CodedLossless == 1 )
			{
				TxMode= ONLY_4X4;
			}
			else 
			{
				size += stream.ReadFixed(size, 1, out this.tx_mode_select, "tx_mode_select"); 

				if ( tx_mode_select != 0 )
				{
					TxMode= TX_MODE_SELECT;
				}
				else 
				{
					TxMode= TX_MODE_LARGEST;
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint TxMode = 0;

			if ( CodedLossless == 1 )
			{
				TxMode= ONLY_4X4;
			}
			else 
			{
				size += stream.WriteFixed(1, this.tx_mode_select, "tx_mode_select"); 

				if ( tx_mode_select != 0 )
				{
					TxMode= TX_MODE_SELECT;
				}
				else 
				{
					TxMode= TX_MODE_LARGEST;
				}
			}

            return size;
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
    public class SkipModeParams : IAomSerializable
    {
		private uint skip_mode_present;
		public uint SkipModePresent { get { return skip_mode_present; } set { skip_mode_present = value; } }

         public SkipModeParams()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint skipModeAllowed = 0;
			uint forwardIdx = 0;
			uint backwardIdx = 0;
			uint i = 0;
			uint refHint = 0;
			uint forwardHint = 0;
			uint backwardHint = 0;
			uint[] SkipModeFrame = null;
			uint secondForwardIdx = 0;
			uint secondForwardHint = 0;
			uint skip_mode_present = 0;

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
				size += stream.ReadFixed(size, 1, out this.skip_mode_present, "skip_mode_present"); 
			}
			else 
			{
				skip_mode_present= 0;
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint skipModeAllowed = 0;
			uint forwardIdx = 0;
			uint backwardIdx = 0;
			uint i = 0;
			uint refHint = 0;
			uint forwardHint = 0;
			uint backwardHint = 0;
			uint[] SkipModeFrame = null;
			uint secondForwardIdx = 0;
			uint secondForwardHint = 0;
			uint skip_mode_present = 0;

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
				size += stream.WriteFixed(1, this.skip_mode_present, "skip_mode_present"); 
			}
			else 
			{
				skip_mode_present= 0;
			}

            return size;
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
    public class FrameReferenceMode : IAomSerializable
    {
		private uint reference_select;
		public uint ReferenceSelect { get { return reference_select; } set { reference_select = value; } }

         public FrameReferenceMode()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint reference_select = 0;

			if ( FrameIsIntra != 0 )
			{
				reference_select= 0;
			}
			else 
			{
				size += stream.ReadFixed(size, 1, out this.reference_select, "reference_select"); 
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint reference_select = 0;

			if ( FrameIsIntra != 0 )
			{
				reference_select= 0;
			}
			else 
			{
				size += stream.WriteFixed(1, this.reference_select, "reference_select"); 
			}

            return size;
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
    public class GlobalMotionParams : IAomSerializable
    {
		private uint[] is_global;
		public uint[] IsGlobal { get { return is_global; } set { is_global = value; } }
		private uint[] is_rot_zoom;
		public uint[] IsRotZoom { get { return is_rot_zoom; } set { is_rot_zoom = value; } }
		private uint[] is_translation;
		public uint[] IsTranslation { get { return is_translation; } set { is_translation = value; } }
		private ReadGlobalParam[] read_global_param;
		public ReadGlobalParam[] ReadGlobalParam { get { return read_global_param; } set { read_global_param = value; } }
		private ReadGlobalParam[] read_global_param0;
		public ReadGlobalParam[] ReadGlobalParam0 { get { return read_global_param0; } set { read_global_param0 = value; } }
		private ReadGlobalParam[] read_global_param1;
		public ReadGlobalParam[] ReadGlobalParam1 { get { return read_global_param1; } set { read_global_param1 = value; } }
		private ReadGlobalParam0[] read_global_param00;
		public ReadGlobalParam0[] ReadGlobalParam00 { get { return read_global_param00; } set { read_global_param00 = value; } }
		private ReadGlobalParam[] read_global_param2;
		public ReadGlobalParam[] ReadGlobalParam2 { get { return read_global_param2; } set { read_global_param2 = value; } }
		private ReadGlobalParam0[] read_global_param01;
		public ReadGlobalParam0[] ReadGlobalParam01 { get { return read_global_param01; } set { read_global_param01 = value; } }

         public GlobalMotionParams()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint refc = 0;
			uint[] GmType = null;
			uint i = 0;
			int[][] gm_params = null;
			uint type = 0;

			for ( refc = LAST_FRAME; refc <= ALTREF_FRAME; refc++ )
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

			this.is_global = new uint[ ALTREF_FRAME];
			this.is_rot_zoom = new uint[ ALTREF_FRAME];
			this.is_translation = new uint[ ALTREF_FRAME];
			this.read_global_param = new ReadGlobalParam[ ALTREF_FRAME];
			this.read_global_param0 = new ReadGlobalParam[ ALTREF_FRAME];
			this.read_global_param1 = new ReadGlobalParam[ ALTREF_FRAME];
			this.read_global_param00 = new ReadGlobalParam0[ ALTREF_FRAME];
			this.read_global_param2 = new ReadGlobalParam[ ALTREF_FRAME];
			this.read_global_param01 = new ReadGlobalParam0[ ALTREF_FRAME];
			for ( refc = LAST_FRAME; refc <= ALTREF_FRAME; refc++ )
			{
				size += stream.ReadFixed(size, 1, out this.is_global[ refc ], "is_global"); 

				if ( is_global[c] != 0 )
				{
					size += stream.ReadFixed(size, 1, out this.is_rot_zoom[ refc ], "is_rot_zoom"); 

					if ( is_rot_zoom[c] != 0 )
					{
						type= ROTZOOM;
					}
					else 
					{
						size += stream.ReadFixed(size, 1, out this.is_translation[ refc ], "is_translation"); 
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
					this.read_global_param[ refc ] =  new ReadGlobalParam(type,  refc,  2) ;
					size +=  stream.ReadClass<ReadGlobalParam>(size, context, this.read_global_param[ refc ], "read_global_param"); 
					this.read_global_param0[ refc ] =  new ReadGlobalParam(type,  refc,  3) ;
					size +=  stream.ReadClass<ReadGlobalParam>(size, context, this.read_global_param0[ refc ], "read_global_param0"); 

					if ( type == AFFINE )
					{
						this.read_global_param1[ refc ] =  new ReadGlobalParam(type,  refc,  4) ;
						size +=  stream.ReadClass<ReadGlobalParam>(size, context, this.read_global_param1[ refc ], "read_global_param1"); 
						this.read_global_param00[ refc ] =  new ReadGlobalParam0(type,  refc,  5) ;
						size +=  stream.ReadClass<ReadGlobalParam0>(size, context, this.read_global_param00[ refc ], "read_global_param00"); 
					}
					else 
					{
						gm_params[refc][4]= -gm_params[refc][3];
						gm_params[refc][5]= gm_params[refc][2];
					}
				}

				if ( type >= TRANSLATION )
				{
					this.read_global_param2[ refc ] =  new ReadGlobalParam(type,  refc,  0) ;
					size +=  stream.ReadClass<ReadGlobalParam>(size, context, this.read_global_param2[ refc ], "read_global_param2"); 
					this.read_global_param01[ refc ] =  new ReadGlobalParam0(type,  refc,  1) ;
					size +=  stream.ReadClass<ReadGlobalParam0>(size, context, this.read_global_param01[ refc ], "read_global_param01"); 
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint refc = 0;
			uint[] GmType = null;
			uint i = 0;
			int[][] gm_params = null;
			uint type = 0;

			for ( refc = LAST_FRAME; refc <= ALTREF_FRAME; refc++ )
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

			for ( refc = LAST_FRAME; refc <= ALTREF_FRAME; refc++ )
			{
				size += stream.WriteFixed(1, this.is_global[ refc ], "is_global"); 

				if ( is_global[c] != 0 )
				{
					size += stream.WriteFixed(1, this.is_rot_zoom[ refc ], "is_rot_zoom"); 

					if ( is_rot_zoom[c] != 0 )
					{
						type= ROTZOOM;
					}
					else 
					{
						size += stream.WriteFixed(1, this.is_translation[ refc ], "is_translation"); 
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
					size += stream.WriteClass<ReadGlobalParam>(context, this.read_global_param[ refc ], "read_global_param"); 
					size += stream.WriteClass<ReadGlobalParam>(context, this.read_global_param0[ refc ], "read_global_param0"); 

					if ( type == AFFINE )
					{
						size += stream.WriteClass<ReadGlobalParam>(context, this.read_global_param1[ refc ], "read_global_param1"); 
						size += stream.WriteClass<ReadGlobalParam0>(context, this.read_global_param00[ refc ], "read_global_param00"); 
					}
					else 
					{
						gm_params[refc][4]= -gm_params[refc][3];
						gm_params[refc][5]= gm_params[refc][2];
					}
				}

				if ( type >= TRANSLATION )
				{
					size += stream.WriteClass<ReadGlobalParam>(context, this.read_global_param2[ refc ], "read_global_param2"); 
					size += stream.WriteClass<ReadGlobalParam0>(context, this.read_global_param01[ refc ], "read_global_param01"); 
				}
			}

            return size;
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
    public class ReadGlobalParam : IAomSerializable
    {
		private uint type;
		public uint Type { get { return type; } set { type = value; } }
		private uint refc;
		public uint Refc { get { return refc; } set { refc = value; } }
		private uint idx;
		public uint Idx { get { return idx; } set { idx = value; } }

         public ReadGlobalParam(uint type, uint refc, uint idx)
         { 
			this.type = type;
			this.refc = refc;
			this.idx = idx;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint absBits = 0;
			uint precBits = 0;
			uint precDiff = 0;
			uint round = 0;
			uint sub = 0;
			uint mx = 0;
			uint r = 0;
			int[][] gm_params = null;
			absBits= GM_ABS_ALPHA_BITS;
			precBits= GM_ALPHA_PREC_BITS;

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
			precDiff= WARPEDMODEL_PREC_BITS - precBits;
			round= (idx % 3) == 2 ? (1 << WARPEDMODEL_PREC_BITS) : 0;
			sub= (idx % 3) == 2 ? (1 << precBits) : 0;
			mx= (1 << absBits);
			r= (PrevGmParams[refc][idx] >> precDiff) - sub;
			gm_params[refc][idx]= (decode_signed_subexp_with_ref( -mx, mx + 1, r ) << precDiff) + round;

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint absBits = 0;
			uint precBits = 0;
			uint precDiff = 0;
			uint round = 0;
			uint sub = 0;
			uint mx = 0;
			uint r = 0;
			int[][] gm_params = null;
			absBits= GM_ABS_ALPHA_BITS;
			precBits= GM_ALPHA_PREC_BITS;

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
			precDiff= WARPEDMODEL_PREC_BITS - precBits;
			round= (idx % 3) == 2 ? (1 << WARPEDMODEL_PREC_BITS) : 0;
			sub= (idx % 3) == 2 ? (1 << precBits) : 0;
			mx= (1 << absBits);
			r= (PrevGmParams[refc][idx] >> precDiff) - sub;
			gm_params[refc][idx]= (decode_signed_subexp_with_ref( -mx, mx + 1, r ) << precDiff) + round;

            return size;
         }

    }

    /*



decode_signed_subexp_with_ref( low, high, r ) { 
 x = decode_unsigned_subexp_with_ref(high - low, r - low)
 return x + low
}
    */
    public class DecodeSignedSubexpWithRef : IAomSerializable
    {
		private uint low;
		public uint Low { get { return low; } set { low = value; } }
		private uint high;
		public uint High { get { return high; } set { high = value; } }
		private uint r;
		public uint r { get { return r; } set { r = value; } }

         public DecodeSignedSubexpWithRef(uint low, uint high, uint r)
         { 
			this.low = low;
			this.high = high;
			this.r = r;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint x = 0;
			x= decode_unsigned_subexp_with_ref(high - low, r - low);
return x + low;

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint x = 0;
			x= decode_unsigned_subexp_with_ref(high - low, r - low);
return x + low;

            return size;
         }

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
    public class DecodeUnsignedSubexpWithRef : IAomSerializable
    {
		private uint mx;
		public uint Mx { get { return mx; } set { mx = value; } }
		private uint r;
		public uint r { get { return r; } set { r = value; } }

         public DecodeUnsignedSubexpWithRef(uint mx, uint r)
         { 
			this.mx = mx;
			this.r = r;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint v = 0;
			v= decode_subexp( mx );

			if ( (r << (int) 1) <= mx )
			{
return inverse_recenter(r, v);
			}
			else 
			{
return mx - 1 - inverse_recenter(mx - 1 - r, v);
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint v = 0;
			v= decode_subexp( mx );

			if ( (r << (int) 1) <= mx )
			{
return inverse_recenter(r, v);
			}
			else 
			{
return mx - 1 - inverse_recenter(mx - 1 - r, v);
			}

            return size;
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
    public class DecodeSubexp : IAomSerializable
    {
		private uint numSyms;
		public uint NumSyms { get { return numSyms; } set { numSyms = value; } }
		private Dictionary<int, uint> subexp_final_bits = new Dictionary<int, uint>();
		public Dictionary<int, uint> SubexpFinalBits { get { return subexp_final_bits; } set { subexp_final_bits = value; } }
		private Dictionary<int, uint> subexp_more_bits = new Dictionary<int, uint>();
		public Dictionary<int, uint> SubexpMoreBits { get { return subexp_more_bits; } set { subexp_more_bits = value; } }
		private Dictionary<int, uint> subexp_bits = new Dictionary<int, uint>();
		public Dictionary<int, uint> SubexpBits { get { return subexp_bits; } set { subexp_bits = value; } }

         public DecodeSubexp(uint numSyms)
         { 
			this.numSyms = numSyms;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint i = 0;
			uint mk = 0;
			uint k = 0;
			int whileIndex = -1;
			uint b2 = 0;
			uint a = 0;
			i= 0;
			mk= 0;
			k= 3;

			while ( 1 != 0 )
			{
				whileIndex++;

				b2= i ? k + i - 1 : k;
				a= 1 << b2;

				if ( numSyms <= mk + 3 * a )
				{
					size += stream.ReadUnsignedInt(size, numSyms - mk, whileIndex, this.subexp_final_bits, "subexp_final_bits"); 
return subexp_final_bits + mk;
				}
				else 
				{
					size += stream.ReadFixed(size, 1, whileIndex, this.subexp_more_bits, "subexp_more_bits"); 

					if ( subexp_more_bits[whileIndex] != 0 )
					{
						i++;
						mk+= a;
					}
					else 
					{
						size += stream.ReadVariable(size, b2, whileIndex, this.subexp_bits, "subexp_bits"); 
return subexp_bits + mk;
					}
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint i = 0;
			uint mk = 0;
			uint k = 0;
			int whileIndex = -1;
			uint b2 = 0;
			uint a = 0;
			i= 0;
			mk= 0;
			k= 3;

			while ( 1 != 0 )
			{
				whileIndex++;

				b2= i ? k + i - 1 : k;
				a= 1 << b2;

				if ( numSyms <= mk + 3 * a )
				{
					size += stream.WriteUnsignedInt(numSyms - mk, whileIndex, this.subexp_final_bits, "subexp_final_bits"); 
return subexp_final_bits + mk;
				}
				else 
				{
					size += stream.WriteFixed(1, whileIndex, this.subexp_more_bits, "subexp_more_bits"); 

					if ( subexp_more_bits[whileIndex] != 0 )
					{
						i++;
						mk+= a;
					}
					else 
					{
						size += stream.WriteVariable(b2, whileIndex, this.subexp_bits, "subexp_bits"); 
return subexp_bits + mk;
					}
				}
			}

            return size;
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
    public class InverseRecenter : IAomSerializable
    {
		private uint r;
		public uint r { get { return r; } set { r = value; } }
		private uint v;
		public uint v { get { return v; } set { v = value; } }

         public InverseRecenter(uint r, uint v)
         { 
			this.r = r;
			this.v = v;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;


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

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;


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

            return size;
         }

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
    public class FilmGrainParams : IAomSerializable
    {
		private ResetGrainParams reset_grain_params;
		public ResetGrainParams ResetGrainParams { get { return reset_grain_params; } set { reset_grain_params = value; } }
		private uint apply_grain;
		public uint ApplyGrain { get { return apply_grain; } set { apply_grain = value; } }
		private uint grain_seed;
		public uint GrainSeed { get { return grain_seed; } set { grain_seed = value; } }
		private uint update_grain;
		public uint UpdateGrain { get { return update_grain; } set { update_grain = value; } }
		private uint film_grain_params_ref_idx;
		public uint FilmGrainParamsRefIdx { get { return film_grain_params_ref_idx; } set { film_grain_params_ref_idx = value; } }
		private LoadGrainParams load_grain_params;
		public LoadGrainParams LoadGrainParams { get { return load_grain_params; } set { load_grain_params = value; } }
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

         public FilmGrainParams()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint update_grain = 0;
			uint tempGrainSeed = 0;
			uint grain_seed = 0;
			uint i = 0;
			uint chroma_scaling_from_luma = 0;
			uint num_cb_points = 0;
			uint num_cr_points = 0;
			uint numPosLuma = 0;
			uint numPosChroma = 0;

			if ( film_grain_params_present== 0 ||
 (show_frame == 0 && showable_frame== 0) )
			{
				this.reset_grain_params =  new ResetGrainParams() ;
				size +=  stream.ReadClass<ResetGrainParams>(size, context, this.reset_grain_params, "reset_grain_params"); 
return;
			}
			size += stream.ReadFixed(size, 1, out this.apply_grain, "apply_grain"); 

			if ( apply_grain== 0 )
			{
				this.reset_grain_params =  new ResetGrainParams() ;
				size +=  stream.ReadClass<ResetGrainParams>(size, context, this.reset_grain_params, "reset_grain_params"); 
return;
			}
			size += stream.ReadFixed(size, 16, out this.grain_seed, "grain_seed"); 

			if ( frame_type == INTER_FRAME )
			{
				size += stream.ReadFixed(size, 1, out this.update_grain, "update_grain"); 
			}
			else 
			{
				update_grain= 1;
			}

			if ( update_grain== 0 )
			{
				size += stream.ReadFixed(size, 3, out this.film_grain_params_ref_idx, "film_grain_params_ref_idx"); 
				tempGrainSeed= grain_seed;
				this.load_grain_params =  new LoadGrainParams( film_grain_params_ref_idx ) ;
				size +=  stream.ReadClass<LoadGrainParams>(size, context, this.load_grain_params, "load_grain_params"); 
				grain_seed= tempGrainSeed;
return;
			}
			size += stream.ReadFixed(size, 4, out this.num_y_points, "num_y_points"); 

			this.point_y_value = new uint[ num_y_points];
			this.point_y_scaling = new uint[ num_y_points];
			for ( i = 0; i < num_y_points; i++ )
			{
				size += stream.ReadFixed(size, 8, out this.point_y_value[ i ], "point_y_value"); 
				size += stream.ReadFixed(size, 8, out this.point_y_scaling[ i ], "point_y_scaling"); 
			}

			if ( mono_chrome != 0 )
			{
				chroma_scaling_from_luma= 0;
			}
			else 
			{
				size += stream.ReadFixed(size, 1, out this.chroma_scaling_from_luma, "chroma_scaling_from_luma"); 
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
				size += stream.ReadFixed(size, 4, out this.num_cb_points, "num_cb_points"); 

				this.point_cb_value = new uint[ num_cb_points];
				this.point_cb_scaling = new uint[ num_cb_points];
				for ( i = 0; i < num_cb_points; i++ )
				{
					size += stream.ReadFixed(size, 8, out this.point_cb_value[ i ], "point_cb_value"); 
					size += stream.ReadFixed(size, 8, out this.point_cb_scaling[ i ], "point_cb_scaling"); 
				}
				size += stream.ReadFixed(size, 4, out this.num_cr_points, "num_cr_points"); 

				this.point_cr_value = new uint[ num_cr_points];
				this.point_cr_scaling = new uint[ num_cr_points];
				for ( i = 0; i < num_cr_points; i++ )
				{
					size += stream.ReadFixed(size, 8, out this.point_cr_value[ i ], "point_cr_value"); 
					size += stream.ReadFixed(size, 8, out this.point_cr_scaling[ i ], "point_cr_scaling"); 
				}
			}
			size += stream.ReadFixed(size, 2, out this.grain_scaling_minus_8, "grain_scaling_minus_8"); 
			size += stream.ReadFixed(size, 2, out this.ar_coeff_lag, "ar_coeff_lag"); 
			numPosLuma= 2 * ar_coeff_lag * ( ar_coeff_lag + 1 );

			if ( num_y_points != 0 )
			{
				numPosChroma= numPosLuma + 1;

				this.ar_coeffs_y_plus_128 = new uint[ numPosLuma];
				for ( i = 0; i < numPosLuma; i++ )
				{
					size += stream.ReadFixed(size, 8, out this.ar_coeffs_y_plus_128[ i ], "ar_coeffs_y_plus_128"); 
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
					size += stream.ReadFixed(size, 8, out this.ar_coeffs_cb_plus_128[ i ], "ar_coeffs_cb_plus_128"); 
				}
			}

			if ( chroma_scaling_from_luma != 0 || num_cr_points != 0 )
			{

				this.ar_coeffs_cr_plus_128 = new uint[ numPosChroma];
				for ( i = 0; i < numPosChroma; i++ )
				{
					size += stream.ReadFixed(size, 8, out this.ar_coeffs_cr_plus_128[ i ], "ar_coeffs_cr_plus_128"); 
				}
			}
			size += stream.ReadFixed(size, 2, out this.ar_coeff_shift_minus_6, "ar_coeff_shift_minus_6"); 
			size += stream.ReadFixed(size, 2, out this.grain_scale_shift, "grain_scale_shift"); 

			if ( num_cb_points != 0 )
			{
				size += stream.ReadFixed(size, 8, out this.cb_mult, "cb_mult"); 
				size += stream.ReadFixed(size, 8, out this.cb_luma_mult, "cb_luma_mult"); 
				size += stream.ReadFixed(size, 9, out this.cb_offset, "cb_offset"); 
			}

			if ( num_cr_points != 0 )
			{
				size += stream.ReadFixed(size, 8, out this.cr_mult, "cr_mult"); 
				size += stream.ReadFixed(size, 8, out this.cr_luma_mult, "cr_luma_mult"); 
				size += stream.ReadFixed(size, 9, out this.cr_offset, "cr_offset"); 
			}
			size += stream.ReadFixed(size, 1, out this.overlap_flag, "overlap_flag"); 
			size += stream.ReadFixed(size, 1, out this.clip_to_restricted_range, "clip_to_restricted_range"); 

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint update_grain = 0;
			uint tempGrainSeed = 0;
			uint grain_seed = 0;
			uint i = 0;
			uint chroma_scaling_from_luma = 0;
			uint num_cb_points = 0;
			uint num_cr_points = 0;
			uint numPosLuma = 0;
			uint numPosChroma = 0;

			if ( film_grain_params_present== 0 ||
 (show_frame == 0 && showable_frame== 0) )
			{
				size += stream.WriteClass<ResetGrainParams>(context, this.reset_grain_params, "reset_grain_params"); 
return;
			}
			size += stream.WriteFixed(1, this.apply_grain, "apply_grain"); 

			if ( apply_grain== 0 )
			{
				size += stream.WriteClass<ResetGrainParams>(context, this.reset_grain_params, "reset_grain_params"); 
return;
			}
			size += stream.WriteFixed(16, this.grain_seed, "grain_seed"); 

			if ( frame_type == INTER_FRAME )
			{
				size += stream.WriteFixed(1, this.update_grain, "update_grain"); 
			}
			else 
			{
				update_grain= 1;
			}

			if ( update_grain== 0 )
			{
				size += stream.WriteFixed(3, this.film_grain_params_ref_idx, "film_grain_params_ref_idx"); 
				tempGrainSeed= grain_seed;
				size += stream.WriteClass<LoadGrainParams>(context, this.load_grain_params, "load_grain_params"); 
				grain_seed= tempGrainSeed;
return;
			}
			size += stream.WriteFixed(4, this.num_y_points, "num_y_points"); 

			for ( i = 0; i < num_y_points; i++ )
			{
				size += stream.WriteFixed(8, this.point_y_value[ i ], "point_y_value"); 
				size += stream.WriteFixed(8, this.point_y_scaling[ i ], "point_y_scaling"); 
			}

			if ( mono_chrome != 0 )
			{
				chroma_scaling_from_luma= 0;
			}
			else 
			{
				size += stream.WriteFixed(1, this.chroma_scaling_from_luma, "chroma_scaling_from_luma"); 
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
				size += stream.WriteFixed(4, this.num_cb_points, "num_cb_points"); 

				for ( i = 0; i < num_cb_points; i++ )
				{
					size += stream.WriteFixed(8, this.point_cb_value[ i ], "point_cb_value"); 
					size += stream.WriteFixed(8, this.point_cb_scaling[ i ], "point_cb_scaling"); 
				}
				size += stream.WriteFixed(4, this.num_cr_points, "num_cr_points"); 

				for ( i = 0; i < num_cr_points; i++ )
				{
					size += stream.WriteFixed(8, this.point_cr_value[ i ], "point_cr_value"); 
					size += stream.WriteFixed(8, this.point_cr_scaling[ i ], "point_cr_scaling"); 
				}
			}
			size += stream.WriteFixed(2, this.grain_scaling_minus_8, "grain_scaling_minus_8"); 
			size += stream.WriteFixed(2, this.ar_coeff_lag, "ar_coeff_lag"); 
			numPosLuma= 2 * ar_coeff_lag * ( ar_coeff_lag + 1 );

			if ( num_y_points != 0 )
			{
				numPosChroma= numPosLuma + 1;

				for ( i = 0; i < numPosLuma; i++ )
				{
					size += stream.WriteFixed(8, this.ar_coeffs_y_plus_128[ i ], "ar_coeffs_y_plus_128"); 
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
					size += stream.WriteFixed(8, this.ar_coeffs_cb_plus_128[ i ], "ar_coeffs_cb_plus_128"); 
				}
			}

			if ( chroma_scaling_from_luma != 0 || num_cr_points != 0 )
			{

				for ( i = 0; i < numPosChroma; i++ )
				{
					size += stream.WriteFixed(8, this.ar_coeffs_cr_plus_128[ i ], "ar_coeffs_cr_plus_128"); 
				}
			}
			size += stream.WriteFixed(2, this.ar_coeff_shift_minus_6, "ar_coeff_shift_minus_6"); 
			size += stream.WriteFixed(2, this.grain_scale_shift, "grain_scale_shift"); 

			if ( num_cb_points != 0 )
			{
				size += stream.WriteFixed(8, this.cb_mult, "cb_mult"); 
				size += stream.WriteFixed(8, this.cb_luma_mult, "cb_luma_mult"); 
				size += stream.WriteFixed(9, this.cb_offset, "cb_offset"); 
			}

			if ( num_cr_points != 0 )
			{
				size += stream.WriteFixed(8, this.cr_mult, "cr_mult"); 
				size += stream.WriteFixed(8, this.cr_luma_mult, "cr_luma_mult"); 
				size += stream.WriteFixed(9, this.cr_offset, "cr_offset"); 
			}
			size += stream.WriteFixed(1, this.overlap_flag, "overlap_flag"); 
			size += stream.WriteFixed(1, this.clip_to_restricted_range, "clip_to_restricted_range"); 

            return size;
         }

    }

    /*



temporal_point_info() { 
 n = frame_presentation_time_length_minus_1 + 1
 frame_presentation_time f(n)
 }
    */
    public class TemporalPointInfo : IAomSerializable
    {
		private uint frame_presentation_time;
		public uint FramePresentationTime { get { return frame_presentation_time; } set { frame_presentation_time = value; } }

         public TemporalPointInfo()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint n = 0;
			n= frame_presentation_time_length_minus_1 + 1;
			size += stream.ReadVariable(size, n, out this.frame_presentation_time, "frame_presentation_time"); 

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint n = 0;
			n= frame_presentation_time_length_minus_1 + 1;
			size += stream.WriteVariable(n, this.frame_presentation_time, "frame_presentation_time"); 

            return size;
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
    public class FrameObu : IAomSerializable
    {
		private uint sz;
		public uint Sz { get { return sz; } set { sz = value; } }
		private FrameHeaderObu frame_header_obu;
		public FrameHeaderObu FrameHeaderObu { get { return frame_header_obu; } set { frame_header_obu = value; } }
		private ByteAlignment byte_alignment;
		public ByteAlignment ByteAlignment { get { return byte_alignment; } set { byte_alignment = value; } }
		private TileGroupObu tile_group_obu;
		public TileGroupObu TileGroupObu { get { return tile_group_obu; } set { tile_group_obu = value; } }

         public FrameObu(uint sz)
         { 
			this.sz = sz;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint startBitPos = 0;
			uint endBitPos = 0;
			uint headerBytes = 0;
			startBitPos= get_position();
			this.frame_header_obu =  new FrameHeaderObu() ;
			size +=  stream.ReadClass<FrameHeaderObu>(size, context, this.frame_header_obu, "frame_header_obu"); 
			this.byte_alignment =  new ByteAlignment() ;
			size +=  stream.ReadClass<ByteAlignment>(size, context, this.byte_alignment, "byte_alignment"); 
			endBitPos= get_position();
			headerBytes= (endBitPos - startBitPos) / 8;
			sz-= headerBytes;
			this.tile_group_obu =  new TileGroupObu( sz ) ;
			size +=  stream.ReadClass<TileGroupObu>(size, context, this.tile_group_obu, "tile_group_obu"); 

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint startBitPos = 0;
			uint endBitPos = 0;
			uint headerBytes = 0;
			startBitPos= get_position();
			size += stream.WriteClass<FrameHeaderObu>(context, this.frame_header_obu, "frame_header_obu"); 
			size += stream.WriteClass<ByteAlignment>(context, this.byte_alignment, "byte_alignment"); 
			endBitPos= get_position();
			headerBytes= (endBitPos - startBitPos) / 8;
			sz-= headerBytes;
			size += stream.WriteClass<TileGroupObu>(context, this.tile_group_obu, "tile_group_obu"); 

            return size;
         }

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
    public class TileGroupObu : IAomSerializable
    {
		private uint sz;
		public uint Sz { get { return sz; } set { sz = value; } }
		private uint tile_start_and_end_present_flag;
		public uint TileStartAndEndPresentFlag { get { return tile_start_and_end_present_flag; } set { tile_start_and_end_present_flag = value; } }
		private uint tg_start;
		public uint TgStart { get { return tg_start; } set { tg_start = value; } }
		private uint tg_end;
		public uint TgEnd { get { return tg_end; } set { tg_end = value; } }
		private ByteAlignment byte_alignment;
		public ByteAlignment ByteAlignment { get { return byte_alignment; } set { byte_alignment = value; } }
		private uint[] tile_size_minus_1;
		public uint[] TileSizeMinus1 { get { return tile_size_minus_1; } set { tile_size_minus_1 = value; } }
		private InitSymbol[] init_symbol;
		public InitSymbol[] InitSymbol { get { return init_symbol; } set { init_symbol = value; } }
		private DecodeTile[] decode_tile;
		public DecodeTile[] DecodeTile { get { return decode_tile; } set { decode_tile = value; } }
		private ExitSymbol[] exit_symbol;
		public ExitSymbol[] ExitSymbol { get { return exit_symbol; } set { exit_symbol = value; } }
		private FrameEndUpdateCdf frame_end_update_cdf;
		public FrameEndUpdateCdf FrameEndUpdateCdf { get { return frame_end_update_cdf; } set { frame_end_update_cdf = value; } }
		private DecodeFrameWrapup decode_frame_wrapup;
		public DecodeFrameWrapup DecodeFrameWrapup { get { return decode_frame_wrapup; } set { decode_frame_wrapup = value; } }

         public TileGroupObu(uint sz)
         { 
			this.sz = sz;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint NumTiles = 0;
			uint startBitPos = 0;
			uint tile_start_and_end_present_flag = 0;
			uint tg_start = 0;
			uint tg_end = 0;
			uint tileBits = 0;
			uint endBitPos = 0;
			uint headerBytes = 0;
			uint TileNum = 0;
			uint tileRow = 0;
			uint tileCol = 0;
			uint lastTile = 0;
			uint tileSize = 0;
			uint MiRowStart = 0;
			uint MiRowEnd = 0;
			uint MiColStart = 0;
			uint MiColEnd = 0;
			uint CurrentQIndex = 0;
			uint SeenFrameHeader = 0;
			NumTiles= TileCols * TileRows;
			startBitPos= get_position();
			tile_start_and_end_present_flag= 0;

			if ( NumTiles > 1 )
			{
				size += stream.ReadFixed(size, 1, out this.tile_start_and_end_present_flag, "tile_start_and_end_present_flag"); 
			}

			if ( NumTiles == 1 || tile_start_and_end_present_flag== 0 )
			{
				tg_start= 0;
				tg_end= NumTiles - 1;
			}
			else 
			{
				tileBits= TileColsLog2 + TileRowsLog2;
				size += stream.ReadVariable(size, tileBits, out this.tg_start, "tg_start"); 
				size += stream.ReadVariable(size, tileBits, out this.tg_end, "tg_end"); 
			}
			this.byte_alignment =  new ByteAlignment() ;
			size +=  stream.ReadClass<ByteAlignment>(size, context, this.byte_alignment, "byte_alignment"); 
			endBitPos= get_position();
			headerBytes= (endBitPos - startBitPos) / 8;
			sz-= headerBytes;

			this.tile_size_minus_1 = new uint[ tg_end];
			this.init_symbol = new InitSymbol[ tg_end];
			this.decode_tile = new DecodeTile[ tg_end];
			this.exit_symbol = new ExitSymbol[ tg_end];
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
					size += stream.ReadLeVar(size, TileSizeBytes, out this.tile_size_minus_1[ TileNum ], "tile_size_minus_1"); 
					tileSize= tile_size_minus_1[TileNum] + 1;
					sz-= tileSize + TileSizeBytes;
				}
				MiRowStart= MiRowStarts[ tileRow ];
				MiRowEnd= MiRowStarts[ tileRow + 1 ];
				MiColStart= MiColStarts[ tileCol ];
				MiColEnd= MiColStarts[ tileCol + 1 ];
				CurrentQIndex= base_q_idx;
				this.init_symbol[ TileNum ] =  new InitSymbol( tileSize ) ;
				size +=  stream.ReadClass<InitSymbol>(size, context, this.init_symbol[ TileNum ], "init_symbol"); 
				this.decode_tile[ TileNum ] =  new DecodeTile() ;
				size +=  stream.ReadClass<DecodeTile>(size, context, this.decode_tile[ TileNum ], "decode_tile"); 
				this.exit_symbol[ TileNum ] =  new ExitSymbol() ;
				size +=  stream.ReadClass<ExitSymbol>(size, context, this.exit_symbol[ TileNum ], "exit_symbol"); 
			}

			if ( tg_end == NumTiles - 1 )
			{

				if ( disable_frame_end_update_cdf== 0 )
				{
					this.frame_end_update_cdf =  new FrameEndUpdateCdf() ;
					size +=  stream.ReadClass<FrameEndUpdateCdf>(size, context, this.frame_end_update_cdf, "frame_end_update_cdf"); 
				}
				this.decode_frame_wrapup =  new DecodeFrameWrapup() ;
				size +=  stream.ReadClass<DecodeFrameWrapup>(size, context, this.decode_frame_wrapup, "decode_frame_wrapup"); 
				SeenFrameHeader= 0;
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint NumTiles = 0;
			uint startBitPos = 0;
			uint tile_start_and_end_present_flag = 0;
			uint tg_start = 0;
			uint tg_end = 0;
			uint tileBits = 0;
			uint endBitPos = 0;
			uint headerBytes = 0;
			uint TileNum = 0;
			uint tileRow = 0;
			uint tileCol = 0;
			uint lastTile = 0;
			uint tileSize = 0;
			uint MiRowStart = 0;
			uint MiRowEnd = 0;
			uint MiColStart = 0;
			uint MiColEnd = 0;
			uint CurrentQIndex = 0;
			uint SeenFrameHeader = 0;
			NumTiles= TileCols * TileRows;
			startBitPos= get_position();
			tile_start_and_end_present_flag= 0;

			if ( NumTiles > 1 )
			{
				size += stream.WriteFixed(1, this.tile_start_and_end_present_flag, "tile_start_and_end_present_flag"); 
			}

			if ( NumTiles == 1 || tile_start_and_end_present_flag== 0 )
			{
				tg_start= 0;
				tg_end= NumTiles - 1;
			}
			else 
			{
				tileBits= TileColsLog2 + TileRowsLog2;
				size += stream.WriteVariable(tileBits, this.tg_start, "tg_start"); 
				size += stream.WriteVariable(tileBits, this.tg_end, "tg_end"); 
			}
			size += stream.WriteClass<ByteAlignment>(context, this.byte_alignment, "byte_alignment"); 
			endBitPos= get_position();
			headerBytes= (endBitPos - startBitPos) / 8;
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
					size += stream.WriteLeVar(TileSizeBytes,  this.tile_size_minus_1[ TileNum ], "tile_size_minus_1"); 
					tileSize= tile_size_minus_1[TileNum] + 1;
					sz-= tileSize + TileSizeBytes;
				}
				MiRowStart= MiRowStarts[ tileRow ];
				MiRowEnd= MiRowStarts[ tileRow + 1 ];
				MiColStart= MiColStarts[ tileCol ];
				MiColEnd= MiColStarts[ tileCol + 1 ];
				CurrentQIndex= base_q_idx;
				size += stream.WriteClass<InitSymbol>(context, this.init_symbol[ TileNum ], "init_symbol"); 
				size += stream.WriteClass<DecodeTile>(context, this.decode_tile[ TileNum ], "decode_tile"); 
				size += stream.WriteClass<ExitSymbol>(context, this.exit_symbol[ TileNum ], "exit_symbol"); 
			}

			if ( tg_end == NumTiles - 1 )
			{

				if ( disable_frame_end_update_cdf== 0 )
				{
					size += stream.WriteClass<FrameEndUpdateCdf>(context, this.frame_end_update_cdf, "frame_end_update_cdf"); 
				}
				size += stream.WriteClass<DecodeFrameWrapup>(context, this.decode_frame_wrapup, "decode_frame_wrapup"); 
				SeenFrameHeader= 0;
			}

            return size;
         }

    }

    /*



decode_tile() { 
 clear_above_context()
 for ( i = 0; i < FRAME_LF_COUNT; i++ )
 DeltaLF[ i ] = 0
 for ( plane = 0; plane < NumPlanes; plane++ ) {
 for ( pass = 0; pass < 2; pass++ ) {
 RefSgrXqd[ plane ][ pass ] = Sgrproj_Xqd_Mid[ pass ]
 for ( i = 0; i < WIENER_COEFFS; i++ ) {
 RefLrWiener[ plane ][ pass ][ i ] = Wiener_Taps_Mid[ i ]
 }
 }
 }
 sbSize = use_128x128_superblock ? BLOCK_128X128 : BLOCK_64X64
 sbSize4 = Num_4x4_Blocks_Wide[ sbSize ]
 for ( r = MiRowStart; r < MiRowEnd; r += sbSize4 ) {
 clear_left_context()
 for ( c = MiColStart; c < MiColEnd; c += sbSize4 ) {
 ReadDeltas = delta_q_present
 clear_cdef( r, c )
 clear_block_decoded_flags( r, c, sbSize4 )
 read_lr( r, c, sbSize )
 decode_partition( r, c, sbSize )
 }
 }
 }
    */
    public class DecodeTile : IAomSerializable
    {
		private ClearAboveContext clear_above_context;
		public ClearAboveContext ClearAboveContext { get { return clear_above_context; } set { clear_above_context = value; } }
		private ClearLeftContext[] clear_left_context;
		public ClearLeftContext[] ClearLeftContext { get { return clear_left_context; } set { clear_left_context = value; } }
		private ClearCdef[][] clear_cdef;
		public ClearCdef[][] ClearCdef { get { return clear_cdef; } set { clear_cdef = value; } }
		private ClearBlockDecodedFlags[][] clear_block_decoded_flags;
		public ClearBlockDecodedFlags[][] ClearBlockDecodedFlags { get { return clear_block_decoded_flags; } set { clear_block_decoded_flags = value; } }
		private ReadLr[][] read_lr;
		public ReadLr[][] ReadLr { get { return read_lr; } set { read_lr = value; } }
		private DecodePartition[][] decode_partition;
		public DecodePartition[][] DecodePartition { get { return decode_partition; } set { decode_partition = value; } }

         public DecodeTile()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint i = 0;
			uint[] DeltaLF = null;
			uint plane = 0;
			uint pass = 0;
			uint[][] RefSgrXqd = null;
			uint[][][] RefLrWiener = null;
			uint sbSize = 0;
			uint sbSize4 = 0;
			uint r = 0;
			uint c = 0;
			uint ReadDeltas = 0;
			this.clear_above_context =  new ClearAboveContext() ;
			size +=  stream.ReadClass<ClearAboveContext>(size, context, this.clear_above_context, "clear_above_context"); 

			for ( i = 0; i < FRAME_LF_COUNT; i++ )
			{
				DeltaLF[ i ]= 0;
			}

			for ( plane = 0; plane < NumPlanes; plane++ )
			{

				for ( pass = 0; pass < 2; pass++ )
				{
					RefSgrXqd[ plane ][ pass ]= Sgrproj_Xqd_Mid[ pass ];

					for ( i = 0; i < WIENER_COEFFS; i++ )
					{
						RefLrWiener[ plane ][ pass ][ i ]= Wiener_Taps_Mid[ i ];
					}
				}
			}
			sbSize= use_128x128_superblock ? BLOCK_128X128 : BLOCK_64X64;
			sbSize4= Num_4x4_Blocks_Wide[ sbSize ];

			this.clear_left_context = new ClearLeftContext[ MiRowEnd];
			this.clear_cdef = new ClearCdef[ MiRowEnd][];
			this.clear_block_decoded_flags = new ClearBlockDecodedFlags[ MiRowEnd][];
			this.read_lr = new ReadLr[ MiRowEnd][];
			this.decode_partition = new DecodePartition[ MiRowEnd][];
			for ( r = MiRowStart; r < MiRowEnd; r += sbSize4 )
			{
				this.clear_left_context[ r ] =  new ClearLeftContext() ;
				size +=  stream.ReadClass<ClearLeftContext>(size, context, this.clear_left_context[ r ], "clear_left_context"); 

				this.clear_cdef[ r ] = new ClearCdef[ MiColEnd];
				this.clear_block_decoded_flags[ r ] = new ClearBlockDecodedFlags[ MiColEnd];
				this.read_lr[ r ] = new ReadLr[ MiColEnd];
				this.decode_partition[ r ] = new DecodePartition[ MiColEnd];
				for ( c = MiColStart; c < MiColEnd; c += sbSize4 )
				{
					ReadDeltas= delta_q_present;
					this.clear_cdef[ r ][ c ] =  new ClearCdef( r,  c ) ;
					size +=  stream.ReadClass<ClearCdef>(size, context, this.clear_cdef[ r ][ c ], "clear_cdef"); 
					this.clear_block_decoded_flags[ r ][ c ] =  new ClearBlockDecodedFlags( r,  c,  sbSize4 ) ;
					size +=  stream.ReadClass<ClearBlockDecodedFlags>(size, context, this.clear_block_decoded_flags[ r ][ c ], "clear_block_decoded_flags"); 
					this.read_lr[ r ][ c ] =  new ReadLr( r,  c,  sbSize ) ;
					size +=  stream.ReadClass<ReadLr>(size, context, this.read_lr[ r ][ c ], "read_lr"); 
					this.decode_partition[ r ][ c ] =  new DecodePartition( r,  c,  sbSize ) ;
					size +=  stream.ReadClass<DecodePartition>(size, context, this.decode_partition[ r ][ c ], "decode_partition"); 
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint i = 0;
			uint[] DeltaLF = null;
			uint plane = 0;
			uint pass = 0;
			uint[][] RefSgrXqd = null;
			uint[][][] RefLrWiener = null;
			uint sbSize = 0;
			uint sbSize4 = 0;
			uint r = 0;
			uint c = 0;
			uint ReadDeltas = 0;
			size += stream.WriteClass<ClearAboveContext>(context, this.clear_above_context, "clear_above_context"); 

			for ( i = 0; i < FRAME_LF_COUNT; i++ )
			{
				DeltaLF[ i ]= 0;
			}

			for ( plane = 0; plane < NumPlanes; plane++ )
			{

				for ( pass = 0; pass < 2; pass++ )
				{
					RefSgrXqd[ plane ][ pass ]= Sgrproj_Xqd_Mid[ pass ];

					for ( i = 0; i < WIENER_COEFFS; i++ )
					{
						RefLrWiener[ plane ][ pass ][ i ]= Wiener_Taps_Mid[ i ];
					}
				}
			}
			sbSize= use_128x128_superblock ? BLOCK_128X128 : BLOCK_64X64;
			sbSize4= Num_4x4_Blocks_Wide[ sbSize ];

			for ( r = MiRowStart; r < MiRowEnd; r += sbSize4 )
			{
				size += stream.WriteClass<ClearLeftContext>(context, this.clear_left_context[ r ], "clear_left_context"); 

				for ( c = MiColStart; c < MiColEnd; c += sbSize4 )
				{
					ReadDeltas= delta_q_present;
					size += stream.WriteClass<ClearCdef>(context, this.clear_cdef[ r ][ c ], "clear_cdef"); 
					size += stream.WriteClass<ClearBlockDecodedFlags>(context, this.clear_block_decoded_flags[ r ][ c ], "clear_block_decoded_flags"); 
					size += stream.WriteClass<ReadLr>(context, this.read_lr[ r ][ c ], "read_lr"); 
					size += stream.WriteClass<DecodePartition>(context, this.decode_partition[ r ][ c ], "decode_partition"); 
				}
			}

            return size;
         }

    }

    /*



clear_block_decoded_flags( r, c, sbSize4 ) { 
 for ( plane = 0; plane < NumPlanes; plane++ ) {
 subX = (plane > 0) ? subsampling_x : 0
 subY = (plane > 0) ? subsampling_y : 0
 sbWidth4 = ( MiColEnd - c ) >> subX
 sbHeight4 = ( MiRowEnd - r ) >> subY
 for ( y = -1; y <= ( sbSize4 >> subY ); y++ )
 for ( x = -1; x <= ( sbSize4 >> subX ); x++ ) {
 if ( y < 0 && x < sbWidth4 )
 BlockDecoded[ plane ][ y ][ x ] = 1
 else if ( x < 0 && y < sbHeight4 )
 BlockDecoded[ plane ][ y ][ x ] = 1
 else
 BlockDecoded[ plane ][ y ][ x ] = 0
 }
 BlockDecoded[ plane ][ sbSize4 >> subY ][ -1 ] = 0
 }
 }
    */
    public class ClearBlockDecodedFlags : IAomSerializable
    {
		private uint r;
		public uint r { get { return r; } set { r = value; } }
		private uint c;
		public uint c { get { return c; } set { c = value; } }
		private uint sbSize4;
		public uint SbSize4 { get { return sbSize4; } set { sbSize4 = value; } }

         public ClearBlockDecodedFlags(uint r, uint c, uint sbSize4)
         { 
			this.r = r;
			this.c = c;
			this.sbSize4 = sbSize4;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint plane = 0;
			uint subX = 0;
			uint subY = 0;
			uint sbWidth4 = 0;
			uint sbHeight4 = 0;
			uint y = 0;
			uint x = 0;
			uint[][][] BlockDecoded = null;

			for ( plane = 0; plane < NumPlanes; plane++ )
			{
				subX= (plane > 0) ? subsampling_x : 0;
				subY= (plane > 0) ? subsampling_y : 0;
				sbWidth4= ( MiColEnd - c ) >> subX;
				sbHeight4= ( MiRowEnd - r ) >> subY;

				for ( y = -1; y <= ( sbSize4 >> subY ); y++ )
				{

					for ( x = -1; x <= ( sbSize4 >> subX ); x++ )
					{

						if ( y < 0 && x < sbWidth4 )
						{
							BlockDecoded[ plane ][ y ][ x ]= 1;
						}
						else if ( x < 0 && y < sbHeight4 )
						{
							BlockDecoded[ plane ][ y ][ x ]= 1;
						}
						else 
						{
							BlockDecoded[ plane ][ y ][ x ]= 0;
						}
					}
				}
				BlockDecoded[ plane ][ sbSize4 >> subY ][ -1 ]= 0;
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint plane = 0;
			uint subX = 0;
			uint subY = 0;
			uint sbWidth4 = 0;
			uint sbHeight4 = 0;
			uint y = 0;
			uint x = 0;
			uint[][][] BlockDecoded = null;

			for ( plane = 0; plane < NumPlanes; plane++ )
			{
				subX= (plane > 0) ? subsampling_x : 0;
				subY= (plane > 0) ? subsampling_y : 0;
				sbWidth4= ( MiColEnd - c ) >> subX;
				sbHeight4= ( MiRowEnd - r ) >> subY;

				for ( y = -1; y <= ( sbSize4 >> subY ); y++ )
				{

					for ( x = -1; x <= ( sbSize4 >> subX ); x++ )
					{

						if ( y < 0 && x < sbWidth4 )
						{
							BlockDecoded[ plane ][ y ][ x ]= 1;
						}
						else if ( x < 0 && y < sbHeight4 )
						{
							BlockDecoded[ plane ][ y ][ x ]= 1;
						}
						else 
						{
							BlockDecoded[ plane ][ y ][ x ]= 0;
						}
					}
				}
				BlockDecoded[ plane ][ sbSize4 >> subY ][ -1 ]= 0;
			}

            return size;
         }

    }

    /*



decode_partition( r, c, bSize ) { 
 if ( r >= MiRows || c >= MiCols )
 return 0
 AvailU = is_inside( r - 1, c )
 AvailL = is_inside( r, c - 1 )
 num4x4 = Num_4x4_Blocks_Wide[ bSize ]
 halfBlock4x4 = num4x4 >> 1
 quarterBlock4x4 = halfBlock4x4 >> 1
 hasRows = ( r + halfBlock4x4 ) < MiRows
 hasCols = ( c + halfBlock4x4 ) < MiCols
 if ( bSize < BLOCK_8X8 ) {
 partition = PARTITION_NONE
 } else if ( hasRows && hasCols ) {
 partition partition S()
 } else if ( hasCols ) {
 split_or_horz split_or_horz S()
 partition = split_or_horz ? PARTITION_SPLIT : PARTITION_HORZ
 } else if ( hasRows ) {
 split_or_vert split_or_vert S()
 partition = split_or_vert ? PARTITION_SPLIT : PARTITION_VERT
 } else {
 partition = PARTITION_SPLIT
 }
 subSize = Partition_Subsize[ partition ][ bSize ]
 splitSize = Partition_Subsize[ PARTITION_SPLIT ][ bSize ]
 if ( partition == PARTITION_NONE ) {
 decode_block( r, c, subSize )
 } else if ( partition == PARTITION_HORZ ) {
 decode_block( r, c, subSize )
 if ( hasRows )
 decode_block( r + halfBlock4x4, c, subSize )
 } else if ( partition == PARTITION_VERT ) {
 decode_block( r, c, subSize )
 if ( hasCols )
 decode_block( r, c + halfBlock4x4, subSize )
 } else if ( partition == PARTITION_SPLIT ) {
 decode_partition( r, c, subSize )
 decode_partition( r, c + halfBlock4x4, subSize )
 decode_partition( r + halfBlock4x4, c, subSize )
 decode_partition( r + halfBlock4x4, c + halfBlock4x4, subSize )
 } else if ( partition == PARTITION_HORZ_A ) {
 decode_block( r, c, splitSize )
 decode_block( r, c + halfBlock4x4, splitSize )
 decode_block( r + halfBlock4x4, c, subSize )
 } else if ( partition == PARTITION_HORZ_B ) {
 decode_block( r, c, subSize )
 decode_block( r + halfBlock4x4, c, splitSize )
 decode_block( r + halfBlock4x4, c + halfBlock4x4, splitSize )
 } else if ( partition == PARTITION_VERT_A ) {
 decode_block( r, c, splitSize )
 decode_block( r + halfBlock4x4, c, splitSize )
 decode_block( r, c + halfBlock4x4, subSize )
 } else if ( partition == PARTITION_VERT_B ) {
 decode_block( r, c, subSize )
 decode_block( r, c + halfBlock4x4, splitSize )
 decode_block( r + halfBlock4x4, c + halfBlock4x4, splitSize )
 } else if ( partition == PARTITION_HORZ_4 ) {
 decode_block( r + quarterBlock4x4 * 0, c, subSize )
 decode_block( r + quarterBlock4x4 * 1, c, subSize )
 decode_block( r + quarterBlock4x4 * 2, c, subSize )
 if ( r + quarterBlock4x4 * 3 < MiRows )
 decode_block( r + quarterBlock4x4 * 3, c, subSize )
 } else {
 decode_block( r, c + quarterBlock4x4 * 0, subSize )
 decode_block( r, c + quarterBlock4x4 * 1, subSize )
 decode_block( r, c + quarterBlock4x4 * 2, subSize )
 if ( c + quarterBlock4x4 * 3 < MiCols )
 decode_block( r, c + quarterBlock4x4 * 3, subSize )
 }
 }
    */
    public class DecodePartition : IAomSerializable
    {
		private uint r;
		public uint r { get { return r; } set { r = value; } }
		private uint c;
		public uint c { get { return c; } set { c = value; } }
		private uint bSize;
		public uint BSize { get { return bSize; } set { bSize = value; } }
		private Partition partition;
		public Partition Partition { get { return partition; } set { partition = value; } }
		private SplitOrHorz split_or_horz;
		public SplitOrHorz SplitOrHorz { get { return split_or_horz; } set { split_or_horz = value; } }
		private SplitOrVert split_or_vert;
		public SplitOrVert SplitOrVert { get { return split_or_vert; } set { split_or_vert = value; } }
		private DecodeBlock decode_block;
		public DecodeBlock DecodeBlock { get { return decode_block; } set { decode_block = value; } }
		private DecodeBlock decode_block0;
		public DecodeBlock DecodeBlock0 { get { return decode_block0; } set { decode_block0 = value; } }
		private DecodeBlock0 decode_block00;
		public DecodeBlock0 DecodeBlock00 { get { return decode_block00; } set { decode_block00 = value; } }
		private DecodePartition decode_partition;
		public DecodePartition _DecodePartition { get { return decode_partition; } set { decode_partition = value; } }
		private DecodePartition decode_partition0;
		public DecodePartition DecodePartition0 { get { return decode_partition0; } set { decode_partition0 = value; } }
		private DecodePartition decode_partition1;
		public DecodePartition DecodePartition1 { get { return decode_partition1; } set { decode_partition1 = value; } }
		private DecodePartition decode_partition2;
		public DecodePartition DecodePartition2 { get { return decode_partition2; } set { decode_partition2 = value; } }
		private DecodeBlock decode_block1;
		public DecodeBlock DecodeBlock1 { get { return decode_block1; } set { decode_block1 = value; } }
		private DecodeBlock0 decode_block01;
		public DecodeBlock0 DecodeBlock01 { get { return decode_block01; } set { decode_block01 = value; } }
		private DecodeBlock1 decode_block10;
		public DecodeBlock1 DecodeBlock10 { get { return decode_block10; } set { decode_block10 = value; } }
		private DecodeBlock0 decode_block02;
		public DecodeBlock0 DecodeBlock02 { get { return decode_block02; } set { decode_block02 = value; } }
		private DecodeBlock1 decode_block11;
		public DecodeBlock1 DecodeBlock11 { get { return decode_block11; } set { decode_block11 = value; } }
		private DecodeBlock decode_block2;
		public DecodeBlock DecodeBlock2 { get { return decode_block2; } set { decode_block2 = value; } }
		private DecodeBlock0 decode_block03;
		public DecodeBlock0 DecodeBlock03 { get { return decode_block03; } set { decode_block03 = value; } }
		private DecodeBlock1 decode_block12;
		public DecodeBlock1 DecodeBlock12 { get { return decode_block12; } set { decode_block12 = value; } }
		private DecodeBlock0 decode_block04;
		public DecodeBlock0 DecodeBlock04 { get { return decode_block04; } set { decode_block04 = value; } }
		private DecodeBlock1 decode_block13;
		public DecodeBlock1 DecodeBlock13 { get { return decode_block13; } set { decode_block13 = value; } }
		private DecodeBlock decode_block3;
		public DecodeBlock DecodeBlock3 { get { return decode_block3; } set { decode_block3 = value; } }
		private DecodeBlock0 decode_block05;
		public DecodeBlock0 DecodeBlock05 { get { return decode_block05; } set { decode_block05 = value; } }
		private DecodeBlock1 decode_block14;
		public DecodeBlock1 DecodeBlock14 { get { return decode_block14; } set { decode_block14 = value; } }
		private DecodeBlock2 decode_block20;
		public DecodeBlock2 DecodeBlock20 { get { return decode_block20; } set { decode_block20 = value; } }
		private DecodeBlock decode_block4;
		public DecodeBlock DecodeBlock4 { get { return decode_block4; } set { decode_block4 = value; } }
		private DecodeBlock0 decode_block06;
		public DecodeBlock0 DecodeBlock06 { get { return decode_block06; } set { decode_block06 = value; } }
		private DecodeBlock1 decode_block15;
		public DecodeBlock1 DecodeBlock15 { get { return decode_block15; } set { decode_block15 = value; } }
		private DecodeBlock2 decode_block21;
		public DecodeBlock2 DecodeBlock21 { get { return decode_block21; } set { decode_block21 = value; } }

         public DecodePartition(uint r, uint c, uint bSize)
         { 
			this.r = r;
			this.c = c;
			this.bSize = bSize;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint AvailU = 0;
			uint AvailL = 0;
			uint num4x4 = 0;
			uint halfBlock4x4 = 0;
			uint quarterBlock4x4 = 0;
			uint hasRows = 0;
			uint hasCols = 0;
			uint partition = 0;
			uint subSize = 0;
			uint splitSize = 0;

			if ( r >= MiRows || c >= MiCols )
			{
return 0;
			}
			AvailU= is_inside( r - 1, c );
			AvailL= is_inside( r, c - 1 );
			num4x4= Num_4x4_Blocks_Wide[ bSize ];
			halfBlock4x4= num4x4 >> 1;
			quarterBlock4x4= halfBlock4x4 >> 1;
			hasRows= ( r + halfBlock4x4 ) < MiRows ? (uint)1 : (uint)0;
			hasCols= ( c + halfBlock4x4 ) < MiCols ? (uint)1 : (uint)0;

			if ( bSize < BLOCK_8X8 )
			{
				partition= PARTITION_NONE;
			}
			else if ( hasRows != 0 && hasCols != 0 )
			{
				this.partition =  new Partition() ;
				size +=  stream.ReadClass<Partition>(size, context, this.partition, "partition"); 
				size += stream.ReadS(size, out this.partition, "partition"); 
			}
			else if ( hasCols != 0 )
			{
				this.split_or_horz =  new SplitOrHorz() ;
				size +=  stream.ReadClass<SplitOrHorz>(size, context, this.split_or_horz, "split_or_horz"); 
				size += stream.ReadS(size, out this.split_or_horz, "split_or_horz"); 
				partition= split_or_horz ? PARTITION_SPLIT : PARTITION_HORZ;
			}
			else if ( hasRows != 0 )
			{
				this.split_or_vert =  new SplitOrVert() ;
				size +=  stream.ReadClass<SplitOrVert>(size, context, this.split_or_vert, "split_or_vert"); 
				size += stream.ReadS(size, out this.split_or_vert, "split_or_vert"); 
				partition= split_or_vert ? PARTITION_SPLIT : PARTITION_VERT;
			}
			else 
			{
				partition= PARTITION_SPLIT;
			}
			subSize= Partition_Subsize[ partition ][ bSize ];
			splitSize= Partition_Subsize[ PARTITION_SPLIT ][ bSize ];

			if ( partition == PARTITION_NONE )
			{
				this.decode_block =  new DecodeBlock( r,  c,  subSize ) ;
				size +=  stream.ReadClass<DecodeBlock>(size, context, this.decode_block, "decode_block"); 
			}
			else if ( partition == PARTITION_HORZ )
			{
				this.decode_block =  new DecodeBlock( r,  c,  subSize ) ;
				size +=  stream.ReadClass<DecodeBlock>(size, context, this.decode_block, "decode_block"); 

				if ( hasRows != 0 )
				{
					this.decode_block0 =  new DecodeBlock( r + halfBlock4x4,  c,  subSize ) ;
					size +=  stream.ReadClass<DecodeBlock>(size, context, this.decode_block0, "decode_block0"); 
				}
			}
			else if ( partition == PARTITION_VERT )
			{
				this.decode_block =  new DecodeBlock( r,  c,  subSize ) ;
				size +=  stream.ReadClass<DecodeBlock>(size, context, this.decode_block, "decode_block"); 

				if ( hasCols != 0 )
				{
					this.decode_block00 =  new DecodeBlock0( r,  c + halfBlock4x4,  subSize ) ;
					size +=  stream.ReadClass<DecodeBlock0>(size, context, this.decode_block00, "decode_block00"); 
				}
			}
			else if ( partition == PARTITION_SPLIT )
			{
				this.decode_partition =  new DecodePartition( r,  c,  subSize ) ;
				size +=  stream.ReadClass<DecodePartition>(size, context, this.decode_partition, "decode_partition"); 
				this.decode_partition0 =  new DecodePartition( r,  c + halfBlock4x4,  subSize ) ;
				size +=  stream.ReadClass<DecodePartition>(size, context, this.decode_partition0, "decode_partition0"); 
				this.decode_partition1 =  new DecodePartition( r + halfBlock4x4,  c,  subSize ) ;
				size +=  stream.ReadClass<DecodePartition>(size, context, this.decode_partition1, "decode_partition1"); 
				this.decode_partition2 =  new DecodePartition( r + halfBlock4x4,  c + halfBlock4x4,  subSize ) ;
				size +=  stream.ReadClass<DecodePartition>(size, context, this.decode_partition2, "decode_partition2"); 
			}
			else if ( partition == PARTITION_HORZ_A )
			{
				this.decode_block1 =  new DecodeBlock( r,  c,  splitSize ) ;
				size +=  stream.ReadClass<DecodeBlock>(size, context, this.decode_block1, "decode_block1"); 
				this.decode_block01 =  new DecodeBlock0( r,  c + halfBlock4x4,  splitSize ) ;
				size +=  stream.ReadClass<DecodeBlock0>(size, context, this.decode_block01, "decode_block01"); 
				this.decode_block10 =  new DecodeBlock1( r + halfBlock4x4,  c,  subSize ) ;
				size +=  stream.ReadClass<DecodeBlock1>(size, context, this.decode_block10, "decode_block10"); 
			}
			else if ( partition == PARTITION_HORZ_B )
			{
				this.decode_block =  new DecodeBlock( r,  c,  subSize ) ;
				size +=  stream.ReadClass<DecodeBlock>(size, context, this.decode_block, "decode_block"); 
				this.decode_block02 =  new DecodeBlock0( r + halfBlock4x4,  c,  splitSize ) ;
				size +=  stream.ReadClass<DecodeBlock0>(size, context, this.decode_block02, "decode_block02"); 
				this.decode_block11 =  new DecodeBlock1( r + halfBlock4x4,  c + halfBlock4x4,  splitSize ) ;
				size +=  stream.ReadClass<DecodeBlock1>(size, context, this.decode_block11, "decode_block11"); 
			}
			else if ( partition == PARTITION_VERT_A )
			{
				this.decode_block2 =  new DecodeBlock( r,  c,  splitSize ) ;
				size +=  stream.ReadClass<DecodeBlock>(size, context, this.decode_block2, "decode_block2"); 
				this.decode_block03 =  new DecodeBlock0( r + halfBlock4x4,  c,  splitSize ) ;
				size +=  stream.ReadClass<DecodeBlock0>(size, context, this.decode_block03, "decode_block03"); 
				this.decode_block12 =  new DecodeBlock1( r,  c + halfBlock4x4,  subSize ) ;
				size +=  stream.ReadClass<DecodeBlock1>(size, context, this.decode_block12, "decode_block12"); 
			}
			else if ( partition == PARTITION_VERT_B )
			{
				this.decode_block =  new DecodeBlock( r,  c,  subSize ) ;
				size +=  stream.ReadClass<DecodeBlock>(size, context, this.decode_block, "decode_block"); 
				this.decode_block04 =  new DecodeBlock0( r,  c + halfBlock4x4,  splitSize ) ;
				size +=  stream.ReadClass<DecodeBlock0>(size, context, this.decode_block04, "decode_block04"); 
				this.decode_block13 =  new DecodeBlock1( r + halfBlock4x4,  c + halfBlock4x4,  splitSize ) ;
				size +=  stream.ReadClass<DecodeBlock1>(size, context, this.decode_block13, "decode_block13"); 
			}
			else if ( partition == PARTITION_HORZ_4 )
			{
				this.decode_block3 =  new DecodeBlock( r + quarterBlock4x4 * 0,  c,  subSize ) ;
				size +=  stream.ReadClass<DecodeBlock>(size, context, this.decode_block3, "decode_block3"); 
				this.decode_block05 =  new DecodeBlock0( r + quarterBlock4x4 * 1,  c,  subSize ) ;
				size +=  stream.ReadClass<DecodeBlock0>(size, context, this.decode_block05, "decode_block05"); 
				this.decode_block14 =  new DecodeBlock1( r + quarterBlock4x4 * 2,  c,  subSize ) ;
				size +=  stream.ReadClass<DecodeBlock1>(size, context, this.decode_block14, "decode_block14"); 

				if ( r + quarterBlock4x4 * 3 < MiRows )
				{
					this.decode_block20 =  new DecodeBlock2( r + quarterBlock4x4 * 3,  c,  subSize ) ;
					size +=  stream.ReadClass<DecodeBlock2>(size, context, this.decode_block20, "decode_block20"); 
				}
			}
			else 
			{
				this.decode_block4 =  new DecodeBlock( r,  c + quarterBlock4x4 * 0,  subSize ) ;
				size +=  stream.ReadClass<DecodeBlock>(size, context, this.decode_block4, "decode_block4"); 
				this.decode_block06 =  new DecodeBlock0( r,  c + quarterBlock4x4 * 1,  subSize ) ;
				size +=  stream.ReadClass<DecodeBlock0>(size, context, this.decode_block06, "decode_block06"); 
				this.decode_block15 =  new DecodeBlock1( r,  c + quarterBlock4x4 * 2,  subSize ) ;
				size +=  stream.ReadClass<DecodeBlock1>(size, context, this.decode_block15, "decode_block15"); 

				if ( c + quarterBlock4x4 * 3 < MiCols )
				{
					this.decode_block21 =  new DecodeBlock2( r,  c + quarterBlock4x4 * 3,  subSize ) ;
					size +=  stream.ReadClass<DecodeBlock2>(size, context, this.decode_block21, "decode_block21"); 
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint AvailU = 0;
			uint AvailL = 0;
			uint num4x4 = 0;
			uint halfBlock4x4 = 0;
			uint quarterBlock4x4 = 0;
			uint hasRows = 0;
			uint hasCols = 0;
			uint partition = 0;
			uint subSize = 0;
			uint splitSize = 0;

			if ( r >= MiRows || c >= MiCols )
			{
return 0;
			}
			AvailU= is_inside( r - 1, c );
			AvailL= is_inside( r, c - 1 );
			num4x4= Num_4x4_Blocks_Wide[ bSize ];
			halfBlock4x4= num4x4 >> 1;
			quarterBlock4x4= halfBlock4x4 >> 1;
			hasRows= ( r + halfBlock4x4 ) < MiRows ? (uint)1 : (uint)0;
			hasCols= ( c + halfBlock4x4 ) < MiCols ? (uint)1 : (uint)0;

			if ( bSize < BLOCK_8X8 )
			{
				partition= PARTITION_NONE;
			}
			else if ( hasRows != 0 && hasCols != 0 )
			{
				size += stream.WriteClass<Partition>(context, this.partition, "partition"); 
				size += stream.WriteS( this.partition, "partition"); 
			}
			else if ( hasCols != 0 )
			{
				size += stream.WriteClass<SplitOrHorz>(context, this.split_or_horz, "split_or_horz"); 
				size += stream.WriteS( this.split_or_horz, "split_or_horz"); 
				partition= split_or_horz ? PARTITION_SPLIT : PARTITION_HORZ;
			}
			else if ( hasRows != 0 )
			{
				size += stream.WriteClass<SplitOrVert>(context, this.split_or_vert, "split_or_vert"); 
				size += stream.WriteS( this.split_or_vert, "split_or_vert"); 
				partition= split_or_vert ? PARTITION_SPLIT : PARTITION_VERT;
			}
			else 
			{
				partition= PARTITION_SPLIT;
			}
			subSize= Partition_Subsize[ partition ][ bSize ];
			splitSize= Partition_Subsize[ PARTITION_SPLIT ][ bSize ];

			if ( partition == PARTITION_NONE )
			{
				size += stream.WriteClass<DecodeBlock>(context, this.decode_block, "decode_block"); 
			}
			else if ( partition == PARTITION_HORZ )
			{
				size += stream.WriteClass<DecodeBlock>(context, this.decode_block, "decode_block"); 

				if ( hasRows != 0 )
				{
					size += stream.WriteClass<DecodeBlock>(context, this.decode_block0, "decode_block0"); 
				}
			}
			else if ( partition == PARTITION_VERT )
			{
				size += stream.WriteClass<DecodeBlock>(context, this.decode_block, "decode_block"); 

				if ( hasCols != 0 )
				{
					size += stream.WriteClass<DecodeBlock0>(context, this.decode_block00, "decode_block00"); 
				}
			}
			else if ( partition == PARTITION_SPLIT )
			{
				size += stream.WriteClass<DecodePartition>(context, this.decode_partition, "decode_partition"); 
				size += stream.WriteClass<DecodePartition>(context, this.decode_partition0, "decode_partition0"); 
				size += stream.WriteClass<DecodePartition>(context, this.decode_partition1, "decode_partition1"); 
				size += stream.WriteClass<DecodePartition>(context, this.decode_partition2, "decode_partition2"); 
			}
			else if ( partition == PARTITION_HORZ_A )
			{
				size += stream.WriteClass<DecodeBlock>(context, this.decode_block1, "decode_block1"); 
				size += stream.WriteClass<DecodeBlock0>(context, this.decode_block01, "decode_block01"); 
				size += stream.WriteClass<DecodeBlock1>(context, this.decode_block10, "decode_block10"); 
			}
			else if ( partition == PARTITION_HORZ_B )
			{
				size += stream.WriteClass<DecodeBlock>(context, this.decode_block, "decode_block"); 
				size += stream.WriteClass<DecodeBlock0>(context, this.decode_block02, "decode_block02"); 
				size += stream.WriteClass<DecodeBlock1>(context, this.decode_block11, "decode_block11"); 
			}
			else if ( partition == PARTITION_VERT_A )
			{
				size += stream.WriteClass<DecodeBlock>(context, this.decode_block2, "decode_block2"); 
				size += stream.WriteClass<DecodeBlock0>(context, this.decode_block03, "decode_block03"); 
				size += stream.WriteClass<DecodeBlock1>(context, this.decode_block12, "decode_block12"); 
			}
			else if ( partition == PARTITION_VERT_B )
			{
				size += stream.WriteClass<DecodeBlock>(context, this.decode_block, "decode_block"); 
				size += stream.WriteClass<DecodeBlock0>(context, this.decode_block04, "decode_block04"); 
				size += stream.WriteClass<DecodeBlock1>(context, this.decode_block13, "decode_block13"); 
			}
			else if ( partition == PARTITION_HORZ_4 )
			{
				size += stream.WriteClass<DecodeBlock>(context, this.decode_block3, "decode_block3"); 
				size += stream.WriteClass<DecodeBlock0>(context, this.decode_block05, "decode_block05"); 
				size += stream.WriteClass<DecodeBlock1>(context, this.decode_block14, "decode_block14"); 

				if ( r + quarterBlock4x4 * 3 < MiRows )
				{
					size += stream.WriteClass<DecodeBlock2>(context, this.decode_block20, "decode_block20"); 
				}
			}
			else 
			{
				size += stream.WriteClass<DecodeBlock>(context, this.decode_block4, "decode_block4"); 
				size += stream.WriteClass<DecodeBlock0>(context, this.decode_block06, "decode_block06"); 
				size += stream.WriteClass<DecodeBlock1>(context, this.decode_block15, "decode_block15"); 

				if ( c + quarterBlock4x4 * 3 < MiCols )
				{
					size += stream.WriteClass<DecodeBlock2>(context, this.decode_block21, "decode_block21"); 
				}
			}

            return size;
         }

    }

    /*



decode_block( r, c, subSize ) { 
 MiRow = r
 MiCol = c
 MiSize = subSize
 bw4 = Num_4x4_Blocks_Wide[ subSize ]
 bh4 = Num_4x4_Blocks_High[ subSize ]
 if ( bh4 == 1 && subsampling_y && (MiRow & 1) == 0 )
 HasChroma = 0
 else if ( bw4 == 1 && subsampling_x && (MiCol & 1) == 0 )
 HasChroma = 0
 else
 HasChroma = NumPlanes > 1
 AvailU = is_inside( r - 1, c )
 AvailL = is_inside( r, c - 1 )
 AvailUChroma = AvailU
 AvailLChroma = AvailL
 if ( HasChroma ) {
 if ( subsampling_y && bh4 == 1 )
 AvailUChroma = is_inside( r - 2, c )
 if ( subsampling_x && bw4 == 1 )
 AvailLChroma = is_inside( r, c - 2 )
 } else {
 AvailUChroma = 0
 AvailLChroma = 0
 }
 mode_info()
 palette_tokens()
 read_block_tx_size()
 if ( skip )
 reset_block_context( bw4, bh4 )
 isCompound = RefFrame[ 1 ] > INTRA_FRAME
 for ( y = 0; y < bh4; y++ ) {
 for ( x = 0; x < bw4; x++ ) {
 YModes [ r + y ][ c + x ] = YMode
 if ( RefFrame[ 0 ] == INTRA_FRAME && HasChroma )
 UVModes [ r + y ][ c + x ] = UVMode
 for ( refList = 0; refList < 2; refList++ )
 RefFrames[ r + y ][ c + x ][ refList ] = RefFrame[ refList ]
 if ( is_inter ) {
 if ( !use_intrabc ) {
 CompGroupIdxs[ r + y ][ c + x ] = comp_group_idx
 CompoundIdxs[ r + y ][ c + x ] = compound_idx
 }
 for ( dir = 0; dir < 2; dir++ ) {
 InterpFilters[ r + y ][ c + x ][ dir ] = interp_filter[ dir ]
 }
 for ( refList = 0; refList < 1 + isCompound; refList++ ) {
 Mvs[ r + y ][ c + x ][ refList ] = Mv[ refList ]
 }
 }
 }
 }
 compute_prediction()
 residual()
 for ( y = 0; y < bh4; y++ ) {
 for ( x = 0; x < bw4; x++ ) {
 IsInters[ r + y ][ c + x ] = is_inter
 SkipModes[ r + y ][ c + x ] = skip_mode
 Skips[ r + y ][ c + x ] = skip
 TxSizes[ r + y ][ c + x ] = TxSize
 MiSizes[ r + y ][ c + x ] = MiSize
 SegmentIds[ r + y ][ c + x ] = segment_id
 PaletteSizes[ 0 ][ r + y ][ c + x ] = PaletteSizeY
 PaletteSizes[ 1 ][ r + y ][ c + x ] = PaletteSizeUV
 for ( i = 0; i < PaletteSizeY; i++ )
 PaletteColors[ 0 ][ r + y ][ c + x ][ i ] = palette_colors_y[ i ]
 for ( i = 0; i < PaletteSizeUV; i++ )
 PaletteColors[ 1 ][ r + y ][ c + x ][ i ] = palette_colors_u[ i ]
 for ( i = 0; i < FRAME_LF_COUNT; i++ )
 DeltaLFs[ r + y ][ c + x ][ i ] = DeltaLF[ i ]
 }
 }
 }
    */
    public class DecodeBlock : IAomSerializable
    {
		private uint r;
		public uint r { get { return r; } set { r = value; } }
		private uint c;
		public uint c { get { return c; } set { c = value; } }
		private uint subSize;
		public uint SubSize { get { return subSize; } set { subSize = value; } }
		private ModeInfo mode_info;
		public ModeInfo ModeInfo { get { return mode_info; } set { mode_info = value; } }
		private PaletteTokens palette_tokens;
		public PaletteTokens PaletteTokens { get { return palette_tokens; } set { palette_tokens = value; } }
		private ReadBlockTxSize read_block_tx_size;
		public ReadBlockTxSize ReadBlockTxSize { get { return read_block_tx_size; } set { read_block_tx_size = value; } }
		private ResetBlockContext reset_block_context;
		public ResetBlockContext ResetBlockContext { get { return reset_block_context; } set { reset_block_context = value; } }
		private ComputePrediction compute_prediction;
		public ComputePrediction ComputePrediction { get { return compute_prediction; } set { compute_prediction = value; } }
		private Residual residual;
		public Residual Residual { get { return residual; } set { residual = value; } }

         public DecodeBlock(uint r, uint c, uint subSize)
         { 
			this.r = r;
			this.c = c;
			this.subSize = subSize;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint MiRow = 0;
			uint MiCol = 0;
			uint MiSize = 0;
			uint bw4 = 0;
			uint bh4 = 0;
			uint HasChroma = 0;
			uint AvailU = 0;
			uint AvailL = 0;
			uint AvailUChroma = 0;
			uint AvailLChroma = 0;
			uint isCompound = 0;
			uint y = 0;
			uint x = 0;
			uint[][] YModes = null;
			uint[][] UVModes = null;
			uint refList = 0;
			uint[][][] RefFrames = null;
			uint[][] CompGroupIdxs = null;
			uint[][] CompoundIdxs = null;
			uint dir = 0;
			uint[][][] InterpFilters = null;
			uint[][][] Mvs = null;
			uint[][] IsInters = null;
			uint[][] SkipModes = null;
			uint[][] Skips = null;
			uint[][] TxSizes = null;
			uint[][] MiSizes = null;
			uint[][] SegmentIds = null;
			uint[][][] PaletteSizes = null;
			uint i = 0;
			uint[][][][] PaletteColors = null;
			uint[][][] DeltaLFs = null;
			MiRow= r;
			MiCol= c;
			MiSize= subSize;
			bw4= Num_4x4_Blocks_Wide[ subSize ];
			bh4= Num_4x4_Blocks_High[ subSize ];

			if ( bh4 == 1 && subsampling_y != 0 && (MiRow & 1) == 0 )
			{
				HasChroma= 0;
			}
			else if ( bw4 == 1 && subsampling_x != 0 && (MiCol & 1) == 0 )
			{
				HasChroma= 0;
			}
			else 
			{
				HasChroma= NumPlanes > 1 ? (uint)1 : (uint)0;
			}
			AvailU= is_inside( r - 1, c );
			AvailL= is_inside( r, c - 1 );
			AvailUChroma= AvailU;
			AvailLChroma= AvailL;

			if ( HasChroma != 0 )
			{

				if ( subsampling_y != 0 && bh4 == 1 )
				{
					AvailUChroma= is_inside( r - 2, c );
				}

				if ( subsampling_x != 0 && bw4 == 1 )
				{
					AvailLChroma= is_inside( r, c - 2 );
				}
			}
			else 
			{
				AvailUChroma= 0;
				AvailLChroma= 0;
			}
			this.mode_info =  new ModeInfo() ;
			size +=  stream.ReadClass<ModeInfo>(size, context, this.mode_info, "mode_info"); 
			this.palette_tokens =  new PaletteTokens() ;
			size +=  stream.ReadClass<PaletteTokens>(size, context, this.palette_tokens, "palette_tokens"); 
			this.read_block_tx_size =  new ReadBlockTxSize() ;
			size +=  stream.ReadClass<ReadBlockTxSize>(size, context, this.read_block_tx_size, "read_block_tx_size"); 

			if ( skip != 0 )
			{
				this.reset_block_context =  new ResetBlockContext( bw4,  bh4 ) ;
				size +=  stream.ReadClass<ResetBlockContext>(size, context, this.reset_block_context, "reset_block_context"); 
			}
			isCompound= RefFrame[ 1 ] > AV1RefFrames.INTRA_FRAME ? (uint)1 : (uint)0;

			for ( y = 0; y < bh4; y++ )
			{

				for ( x = 0; x < bw4; x++ )
				{
					YModes[ r + y ][ c + x ]= YMode;

					if ( RefFrame[ 0 ] == INTRA_FRAME && HasChroma != 0 )
					{
						UVModes[ r + y ][ c + x ]= UVMode;
					}

					for ( refList = 0; refList < 2; refList++ )
					{
						RefFrames[ r + y ][ c + x ][ refList ]= RefFrame[ refList ];
					}

					if ( is_inter != 0 )
					{

						if ( use_intrabc== 0 )
						{
							CompGroupIdxs[ r + y ][ c + x ]= comp_group_idx;
							CompoundIdxs[ r + y ][ c + x ]= compound_idx;
						}

						for ( dir = 0; dir < 2; dir++ )
						{
							InterpFilters[ r + y ][ c + x ][ dir ]= interp_filter[ dir ];
						}

						for ( refList = 0; refList < 1 + isCompound; refList++ )
						{
							Mvs[ r + y ][ c + x ][ refList ]= Mv[ refList ];
						}
					}
				}
			}
			this.compute_prediction =  new ComputePrediction() ;
			size +=  stream.ReadClass<ComputePrediction>(size, context, this.compute_prediction, "compute_prediction"); 
			this.residual =  new Residual() ;
			size +=  stream.ReadClass<Residual>(size, context, this.residual, "residual"); 

			for ( y = 0; y < bh4; y++ )
			{

				for ( x = 0; x < bw4; x++ )
				{
					IsInters[ r + y ][ c + x ]= is_inter;
					SkipModes[ r + y ][ c + x ]= skip_mode;
					Skips[ r + y ][ c + x ]= skip;
					TxSizes[ r + y ][ c + x ]= TxSize;
					MiSizes[ r + y ][ c + x ]= MiSize;
					SegmentIds[ r + y ][ c + x ]= segment_id;
					PaletteSizes[ 0 ][ r + y ][ c + x ]= PaletteSizeY;
					PaletteSizes[ 1 ][ r + y ][ c + x ]= PaletteSizeUV;

					for ( i = 0; i < PaletteSizeY; i++ )
					{
						PaletteColors[ 0 ][ r + y ][ c + x ][ i ]= palette_colors_y[ i ];
					}

					for ( i = 0; i < PaletteSizeUV; i++ )
					{
						PaletteColors[ 1 ][ r + y ][ c + x ][ i ]= palette_colors_u[ i ];
					}

					for ( i = 0; i < FRAME_LF_COUNT; i++ )
					{
						DeltaLFs[ r + y ][ c + x ][ i ]= DeltaLF[ i ];
					}
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint MiRow = 0;
			uint MiCol = 0;
			uint MiSize = 0;
			uint bw4 = 0;
			uint bh4 = 0;
			uint HasChroma = 0;
			uint AvailU = 0;
			uint AvailL = 0;
			uint AvailUChroma = 0;
			uint AvailLChroma = 0;
			uint isCompound = 0;
			uint y = 0;
			uint x = 0;
			uint[][] YModes = null;
			uint[][] UVModes = null;
			uint refList = 0;
			uint[][][] RefFrames = null;
			uint[][] CompGroupIdxs = null;
			uint[][] CompoundIdxs = null;
			uint dir = 0;
			uint[][][] InterpFilters = null;
			uint[][][] Mvs = null;
			uint[][] IsInters = null;
			uint[][] SkipModes = null;
			uint[][] Skips = null;
			uint[][] TxSizes = null;
			uint[][] MiSizes = null;
			uint[][] SegmentIds = null;
			uint[][][] PaletteSizes = null;
			uint i = 0;
			uint[][][][] PaletteColors = null;
			uint[][][] DeltaLFs = null;
			MiRow= r;
			MiCol= c;
			MiSize= subSize;
			bw4= Num_4x4_Blocks_Wide[ subSize ];
			bh4= Num_4x4_Blocks_High[ subSize ];

			if ( bh4 == 1 && subsampling_y != 0 && (MiRow & 1) == 0 )
			{
				HasChroma= 0;
			}
			else if ( bw4 == 1 && subsampling_x != 0 && (MiCol & 1) == 0 )
			{
				HasChroma= 0;
			}
			else 
			{
				HasChroma= NumPlanes > 1 ? (uint)1 : (uint)0;
			}
			AvailU= is_inside( r - 1, c );
			AvailL= is_inside( r, c - 1 );
			AvailUChroma= AvailU;
			AvailLChroma= AvailL;

			if ( HasChroma != 0 )
			{

				if ( subsampling_y != 0 && bh4 == 1 )
				{
					AvailUChroma= is_inside( r - 2, c );
				}

				if ( subsampling_x != 0 && bw4 == 1 )
				{
					AvailLChroma= is_inside( r, c - 2 );
				}
			}
			else 
			{
				AvailUChroma= 0;
				AvailLChroma= 0;
			}
			size += stream.WriteClass<ModeInfo>(context, this.mode_info, "mode_info"); 
			size += stream.WriteClass<PaletteTokens>(context, this.palette_tokens, "palette_tokens"); 
			size += stream.WriteClass<ReadBlockTxSize>(context, this.read_block_tx_size, "read_block_tx_size"); 

			if ( skip != 0 )
			{
				size += stream.WriteClass<ResetBlockContext>(context, this.reset_block_context, "reset_block_context"); 
			}
			isCompound= RefFrame[ 1 ] > AV1RefFrames.INTRA_FRAME ? (uint)1 : (uint)0;

			for ( y = 0; y < bh4; y++ )
			{

				for ( x = 0; x < bw4; x++ )
				{
					YModes[ r + y ][ c + x ]= YMode;

					if ( RefFrame[ 0 ] == INTRA_FRAME && HasChroma != 0 )
					{
						UVModes[ r + y ][ c + x ]= UVMode;
					}

					for ( refList = 0; refList < 2; refList++ )
					{
						RefFrames[ r + y ][ c + x ][ refList ]= RefFrame[ refList ];
					}

					if ( is_inter != 0 )
					{

						if ( use_intrabc== 0 )
						{
							CompGroupIdxs[ r + y ][ c + x ]= comp_group_idx;
							CompoundIdxs[ r + y ][ c + x ]= compound_idx;
						}

						for ( dir = 0; dir < 2; dir++ )
						{
							InterpFilters[ r + y ][ c + x ][ dir ]= interp_filter[ dir ];
						}

						for ( refList = 0; refList < 1 + isCompound; refList++ )
						{
							Mvs[ r + y ][ c + x ][ refList ]= Mv[ refList ];
						}
					}
				}
			}
			size += stream.WriteClass<ComputePrediction>(context, this.compute_prediction, "compute_prediction"); 
			size += stream.WriteClass<Residual>(context, this.residual, "residual"); 

			for ( y = 0; y < bh4; y++ )
			{

				for ( x = 0; x < bw4; x++ )
				{
					IsInters[ r + y ][ c + x ]= is_inter;
					SkipModes[ r + y ][ c + x ]= skip_mode;
					Skips[ r + y ][ c + x ]= skip;
					TxSizes[ r + y ][ c + x ]= TxSize;
					MiSizes[ r + y ][ c + x ]= MiSize;
					SegmentIds[ r + y ][ c + x ]= segment_id;
					PaletteSizes[ 0 ][ r + y ][ c + x ]= PaletteSizeY;
					PaletteSizes[ 1 ][ r + y ][ c + x ]= PaletteSizeUV;

					for ( i = 0; i < PaletteSizeY; i++ )
					{
						PaletteColors[ 0 ][ r + y ][ c + x ][ i ]= palette_colors_y[ i ];
					}

					for ( i = 0; i < PaletteSizeUV; i++ )
					{
						PaletteColors[ 1 ][ r + y ][ c + x ][ i ]= palette_colors_u[ i ];
					}

					for ( i = 0; i < FRAME_LF_COUNT; i++ )
					{
						DeltaLFs[ r + y ][ c + x ][ i ]= DeltaLF[ i ];
					}
				}
			}

            return size;
         }

    }

    /*

 reset_block_context(bw4,bh4){
 for(plane==0;plane<1+2*HasChroma;plane++){
 subX=(plane>0)?subsampling_x:0
 subY=(plane>00)?subsampling_y:0
 for(i=MiCol>>subX;i<((MiCol+bw4)>>subX); i++){
 AboveLevelContext[plane][i]=0
 AboveDcContext[plane][i]=0
 }
 for (i=MiRow >> subY;i<((MiRow+bh4)>>subY); i++){
 LeftLevelContext[plane][i]=0
 LeftDcContext[plane][i]=0
 }
 }
 }
    */
    public class ResetBlockContext : IAomSerializable
    {
		private uint bw4;
		public uint Bw4 { get { return bw4; } set { bw4 = value; } }
		private uint bh4;
		public uint Bh4 { get { return bh4; } set { bh4 = value; } }

         public ResetBlockContext(uint bw4, uint bh4)
         { 
			this.bw4 = bw4;
			this.bh4 = bh4;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint plane = 0;
			uint subX = 0;
			uint subY = 0;
			uint i = 0;
			uint[][] AboveLevelContext = null;
			uint[][] AboveDcContext = null;
			uint[][] LeftLevelContext = null;
			uint[][] LeftDcContext = null;

			for (plane==0;plane<1+2*HasChroma;plane++)
			{
				subX= (plane>0)?subsampling_x:0;
				subY= (plane>00)?subsampling_y:0;

				for (i=MiCol>>subX;i<((MiCol+bw4)>>subX); i++)
				{
					AboveLevelContext[plane][i]= 0;
					AboveDcContext[plane][i]= 0;
				}

				for (i=MiRow >> subY;i<((MiRow+bh4)>>subY); i++)
				{
					LeftLevelContext[plane][i]= 0;
					LeftDcContext[plane][i]= 0;
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint plane = 0;
			uint subX = 0;
			uint subY = 0;
			uint i = 0;
			uint[][] AboveLevelContext = null;
			uint[][] AboveDcContext = null;
			uint[][] LeftLevelContext = null;
			uint[][] LeftDcContext = null;

			for (plane==0;plane<1+2*HasChroma;plane++)
			{
				subX= (plane>0)?subsampling_x:0;
				subY= (plane>00)?subsampling_y:0;

				for (i=MiCol>>subX;i<((MiCol+bw4)>>subX); i++)
				{
					AboveLevelContext[plane][i]= 0;
					AboveDcContext[plane][i]= 0;
				}

				for (i=MiRow >> subY;i<((MiRow+bh4)>>subY); i++)
				{
					LeftLevelContext[plane][i]= 0;
					LeftDcContext[plane][i]= 0;
				}
			}

            return size;
         }

    }

    /*



mode_info() { 
 if ( FrameIsIntra )
 intra_frame_mode_info()
 else
 inter_frame_mode_info()
 }
    */
    public class ModeInfo : IAomSerializable
    {
		private IntraFrameModeInfo intra_frame_mode_info;
		public IntraFrameModeInfo IntraFrameModeInfo { get { return intra_frame_mode_info; } set { intra_frame_mode_info = value; } }
		private InterFrameModeInfo inter_frame_mode_info;
		public InterFrameModeInfo InterFrameModeInfo { get { return inter_frame_mode_info; } set { inter_frame_mode_info = value; } }

         public ModeInfo()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;


			if ( FrameIsIntra != 0 )
			{
				this.intra_frame_mode_info =  new IntraFrameModeInfo() ;
				size +=  stream.ReadClass<IntraFrameModeInfo>(size, context, this.intra_frame_mode_info, "intra_frame_mode_info"); 
			}
			else 
			{
				this.inter_frame_mode_info =  new InterFrameModeInfo() ;
				size +=  stream.ReadClass<InterFrameModeInfo>(size, context, this.inter_frame_mode_info, "inter_frame_mode_info"); 
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;


			if ( FrameIsIntra != 0 )
			{
				size += stream.WriteClass<IntraFrameModeInfo>(context, this.intra_frame_mode_info, "intra_frame_mode_info"); 
			}
			else 
			{
				size += stream.WriteClass<InterFrameModeInfo>(context, this.inter_frame_mode_info, "inter_frame_mode_info"); 
			}

            return size;
         }

    }

    /*



intra_frame_mode_info() { 
 skip = 0
 if ( SegIdPreSkip )
 intra_segment_id()
 skip_mode = 0
 read_skip()
 if ( !SegIdPreSkip )
 intra_segment_id()
 read_cdef()
 read_delta_qindex()
 read_delta_lf()
 ReadDeltas = 0
 RefFrame[ 0 ] = INTRA_FRAME
 RefFrame[ 1 ] = NONE
 if ( allow_intrabc ) {
 use_intrabc S()
 } else {
 use_intrabc = 0
 }
 if ( use_intrabc ) {
 is_inter = 1
 YMode = DC_PRED
 UVMode = DC_PRED
 motion_mode = SIMPLE
 compound_type = COMPOUND_AVERAGE
 PaletteSizeY = 0
 PaletteSizeUV = 0
 interp_filter[ 0 ] = BILINEAR
 interp_filter[ 1 ] = BILINEAR
 find_mv_stack( 0 )
 assign_mv( 0 )
 } else {
 is_inter = 0
 intra_frame_y_mode S()
 YMode = intra_frame_y_mode
 intra_angle_info_y()
 if ( HasChroma ) {
 uv_mode S()
 UVMode = uv_mode
 if ( UVMode == UV_CFL_PRED ) {
 read_cfl_alphas()
 }
 intra_angle_info_uv()
 }
 PaletteSizeY = 0
 PaletteSizeUV = 0
 if ( MiSize >= BLOCK_8X8 &&
 Block_Width[ MiSize ] <= 64  &&
 Block_Height[ MiSize ] <= 64 &&
 allow_screen_content_tools ) {
 palette_mode_info()
 }
 filter_intra_mode_info()
 }
 }
    */
    public class IntraFrameModeInfo : IAomSerializable
    {
		private IntraSegmentId intra_segment_id;
		public IntraSegmentId IntraSegmentId { get { return intra_segment_id; } set { intra_segment_id = value; } }
		private ReadSkip read_skip;
		public ReadSkip ReadSkip { get { return read_skip; } set { read_skip = value; } }
		private ReadCdef read_cdef;
		public ReadCdef ReadCdef { get { return read_cdef; } set { read_cdef = value; } }
		private ReadDeltaQindex read_delta_qindex;
		public ReadDeltaQindex ReadDeltaQindex { get { return read_delta_qindex; } set { read_delta_qindex = value; } }
		private ReadDeltaLf read_delta_lf;
		public ReadDeltaLf ReadDeltaLf { get { return read_delta_lf; } set { read_delta_lf = value; } }
		private uint use_intrabc;
		public uint UseIntrabc { get { return use_intrabc; } set { use_intrabc = value; } }
		private FindMvStack find_mv_stack;
		public FindMvStack FindMvStack { get { return find_mv_stack; } set { find_mv_stack = value; } }
		private AssignMv assign_mv;
		public AssignMv AssignMv { get { return assign_mv; } set { assign_mv = value; } }
		private uint intra_frame_y_mode;
		public uint IntraFrameyMode { get { return intra_frame_y_mode; } set { intra_frame_y_mode = value; } }
		private IntraAngleInfoy intra_angle_info_y;
		public IntraAngleInfoy IntraAngleInfoy { get { return intra_angle_info_y; } set { intra_angle_info_y = value; } }
		private uint uv_mode;
		public uint UvMode { get { return uv_mode; } set { uv_mode = value; } }
		private ReadCflAlphas read_cfl_alphas;
		public ReadCflAlphas ReadCflAlphas { get { return read_cfl_alphas; } set { read_cfl_alphas = value; } }
		private IntraAngleInfoUv intra_angle_info_uv;
		public IntraAngleInfoUv IntraAngleInfoUv { get { return intra_angle_info_uv; } set { intra_angle_info_uv = value; } }
		private PaletteModeInfo palette_mode_info;
		public PaletteModeInfo PaletteModeInfo { get { return palette_mode_info; } set { palette_mode_info = value; } }
		private FilterIntraModeInfo filter_intra_mode_info;
		public FilterIntraModeInfo FilterIntraModeInfo { get { return filter_intra_mode_info; } set { filter_intra_mode_info = value; } }

         public IntraFrameModeInfo()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint skip = 0;
			uint skip_mode = 0;
			uint ReadDeltas = 0;
			uint[] RefFrame = null;
			uint use_intrabc = 0;
			uint is_inter = 0;
			uint YMode = 0;
			uint UVMode = 0;
			uint motion_mode = 0;
			uint compound_type = 0;
			uint PaletteSizeY = 0;
			uint PaletteSizeUV = 0;
			uint[] interp_filter = null;
			skip= 0;

			if ( SegIdPreSkip != 0 )
			{
				this.intra_segment_id =  new IntraSegmentId() ;
				size +=  stream.ReadClass<IntraSegmentId>(size, context, this.intra_segment_id, "intra_segment_id"); 
			}
			skip_mode= 0;
			this.read_skip =  new ReadSkip() ;
			size +=  stream.ReadClass<ReadSkip>(size, context, this.read_skip, "read_skip"); 

			if ( SegIdPreSkip== 0 )
			{
				this.intra_segment_id =  new IntraSegmentId() ;
				size +=  stream.ReadClass<IntraSegmentId>(size, context, this.intra_segment_id, "intra_segment_id"); 
			}
			this.read_cdef =  new ReadCdef() ;
			size +=  stream.ReadClass<ReadCdef>(size, context, this.read_cdef, "read_cdef"); 
			this.read_delta_qindex =  new ReadDeltaQindex() ;
			size +=  stream.ReadClass<ReadDeltaQindex>(size, context, this.read_delta_qindex, "read_delta_qindex"); 
			this.read_delta_lf =  new ReadDeltaLf() ;
			size +=  stream.ReadClass<ReadDeltaLf>(size, context, this.read_delta_lf, "read_delta_lf"); 
			ReadDeltas= 0;
			RefFrame[ 0 ]= AV1RefFrames.INTRA_FRAME;
			RefFrame[ 1 ]= NONE;

			if ( allow_intrabc != 0 )
			{
				size += stream.ReadS(size, out this.use_intrabc, "use_intrabc"); 
			}
			else 
			{
				use_intrabc= 0;
			}

			if ( use_intrabc != 0 )
			{
				is_inter= 1;
				YMode= DC_PRED;
				UVMode= DC_PRED;
				motion_mode= SIMPLE;
				compound_type= COMPOUND_AVERAGE;
				PaletteSizeY= 0;
				PaletteSizeUV= 0;
				interp_filter[ 0 ]= BILINEAR;
				interp_filter[ 1 ]= BILINEAR;
				this.find_mv_stack =  new FindMvStack( 0 ) ;
				size +=  stream.ReadClass<FindMvStack>(size, context, this.find_mv_stack, "find_mv_stack"); 
				this.assign_mv =  new AssignMv( 0 ) ;
				size +=  stream.ReadClass<AssignMv>(size, context, this.assign_mv, "assign_mv"); 
			}
			else 
			{
				is_inter= 0;
				size += stream.ReadS(size, out this.intra_frame_y_mode, "intra_frame_y_mode"); 
				YMode= intra_frame_y_mode;
				this.intra_angle_info_y =  new IntraAngleInfoy() ;
				size +=  stream.ReadClass<IntraAngleInfoy>(size, context, this.intra_angle_info_y, "intra_angle_info_y"); 

				if ( HasChroma != 0 )
				{
					size += stream.ReadS(size, out this.uv_mode, "uv_mode"); 
					UVMode= uv_mode;

					if ( UVMode == UV_CFL_PRED )
					{
						this.read_cfl_alphas =  new ReadCflAlphas() ;
						size +=  stream.ReadClass<ReadCflAlphas>(size, context, this.read_cfl_alphas, "read_cfl_alphas"); 
					}
					this.intra_angle_info_uv =  new IntraAngleInfoUv() ;
					size +=  stream.ReadClass<IntraAngleInfoUv>(size, context, this.intra_angle_info_uv, "intra_angle_info_uv"); 
				}
				PaletteSizeY= 0;
				PaletteSizeUV= 0;

				if ( MiSize >= BLOCK_8X8 &&
 Block_Width[ MiSize ] <= 64  &&
 Block_Height[ MiSize ] <= 64 &&
 allow_screen_content_tools != 0 )
				{
					this.palette_mode_info =  new PaletteModeInfo() ;
					size +=  stream.ReadClass<PaletteModeInfo>(size, context, this.palette_mode_info, "palette_mode_info"); 
				}
				this.filter_intra_mode_info =  new FilterIntraModeInfo() ;
				size +=  stream.ReadClass<FilterIntraModeInfo>(size, context, this.filter_intra_mode_info, "filter_intra_mode_info"); 
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint skip = 0;
			uint skip_mode = 0;
			uint ReadDeltas = 0;
			uint[] RefFrame = null;
			uint use_intrabc = 0;
			uint is_inter = 0;
			uint YMode = 0;
			uint UVMode = 0;
			uint motion_mode = 0;
			uint compound_type = 0;
			uint PaletteSizeY = 0;
			uint PaletteSizeUV = 0;
			uint[] interp_filter = null;
			skip= 0;

			if ( SegIdPreSkip != 0 )
			{
				size += stream.WriteClass<IntraSegmentId>(context, this.intra_segment_id, "intra_segment_id"); 
			}
			skip_mode= 0;
			size += stream.WriteClass<ReadSkip>(context, this.read_skip, "read_skip"); 

			if ( SegIdPreSkip== 0 )
			{
				size += stream.WriteClass<IntraSegmentId>(context, this.intra_segment_id, "intra_segment_id"); 
			}
			size += stream.WriteClass<ReadCdef>(context, this.read_cdef, "read_cdef"); 
			size += stream.WriteClass<ReadDeltaQindex>(context, this.read_delta_qindex, "read_delta_qindex"); 
			size += stream.WriteClass<ReadDeltaLf>(context, this.read_delta_lf, "read_delta_lf"); 
			ReadDeltas= 0;
			RefFrame[ 0 ]= AV1RefFrames.INTRA_FRAME;
			RefFrame[ 1 ]= NONE;

			if ( allow_intrabc != 0 )
			{
				size += stream.WriteS( this.use_intrabc, "use_intrabc"); 
			}
			else 
			{
				use_intrabc= 0;
			}

			if ( use_intrabc != 0 )
			{
				is_inter= 1;
				YMode= DC_PRED;
				UVMode= DC_PRED;
				motion_mode= SIMPLE;
				compound_type= COMPOUND_AVERAGE;
				PaletteSizeY= 0;
				PaletteSizeUV= 0;
				interp_filter[ 0 ]= BILINEAR;
				interp_filter[ 1 ]= BILINEAR;
				size += stream.WriteClass<FindMvStack>(context, this.find_mv_stack, "find_mv_stack"); 
				size += stream.WriteClass<AssignMv>(context, this.assign_mv, "assign_mv"); 
			}
			else 
			{
				is_inter= 0;
				size += stream.WriteS( this.intra_frame_y_mode, "intra_frame_y_mode"); 
				YMode= intra_frame_y_mode;
				size += stream.WriteClass<IntraAngleInfoy>(context, this.intra_angle_info_y, "intra_angle_info_y"); 

				if ( HasChroma != 0 )
				{
					size += stream.WriteS( this.uv_mode, "uv_mode"); 
					UVMode= uv_mode;

					if ( UVMode == UV_CFL_PRED )
					{
						size += stream.WriteClass<ReadCflAlphas>(context, this.read_cfl_alphas, "read_cfl_alphas"); 
					}
					size += stream.WriteClass<IntraAngleInfoUv>(context, this.intra_angle_info_uv, "intra_angle_info_uv"); 
				}
				PaletteSizeY= 0;
				PaletteSizeUV= 0;

				if ( MiSize >= BLOCK_8X8 &&
 Block_Width[ MiSize ] <= 64  &&
 Block_Height[ MiSize ] <= 64 &&
 allow_screen_content_tools != 0 )
				{
					size += stream.WriteClass<PaletteModeInfo>(context, this.palette_mode_info, "palette_mode_info"); 
				}
				size += stream.WriteClass<FilterIntraModeInfo>(context, this.filter_intra_mode_info, "filter_intra_mode_info"); 
			}

            return size;
         }

    }

    /*



intra_segment_id() { 
 if ( segmentation_enabled )
 read_segment_id()
 else
 segment_id = 0
 Lossless = LosslessArray[ segment_id ]
 }
    */
    public class IntraSegmentId : IAomSerializable
    {
		private ReadSegmentId read_segment_id;
		public ReadSegmentId ReadSegmentId { get { return read_segment_id; } set { read_segment_id = value; } }

         public IntraSegmentId()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint segment_id = 0;
			uint Lossless = 0;

			if ( segmentation_enabled != 0 )
			{
				this.read_segment_id =  new ReadSegmentId() ;
				size +=  stream.ReadClass<ReadSegmentId>(size, context, this.read_segment_id, "read_segment_id"); 
			}
			else 
			{
				segment_id= 0;
			}
			Lossless= LosslessArray[ segment_id ];

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint segment_id = 0;
			uint Lossless = 0;

			if ( segmentation_enabled != 0 )
			{
				size += stream.WriteClass<ReadSegmentId>(context, this.read_segment_id, "read_segment_id"); 
			}
			else 
			{
				segment_id= 0;
			}
			Lossless= LosslessArray[ segment_id ];

            return size;
         }

    }

    /*



read_segment_id() { 
 if ( AvailU && AvailL )
 prevUL = SegmentIds[ MiRow - 1 ][ MiCol - 1 ]
 else
 prevUL = -1
 if ( AvailU )
 prevU = SegmentIds[ MiRow - 1 ][ MiCol ]
 else
 prevU = -1
 if ( AvailL )
 prevL = SegmentIds[ MiRow ][ MiCol - 1 ]
 else
 prevL = -1
 if ( prevU == -1 )
 pred = (prevL == -1) ? 0 : prevL
 else if ( prevL == -1 )
 pred = prevU
 else
 pred = (prevUL == prevU) ? prevU : prevL
 if ( skip ) {
 segment_id = pred
 } else {
 segment_id S()
 segment_id = neg_deinterleave( segment_id, pred, LastActiveSegId + 1 )
 }
 }
    */
    public class ReadSegmentId : IAomSerializable
    {
		private uint segment_id;
		public uint SegmentId { get { return segment_id; } set { segment_id = value; } }

         public ReadSegmentId()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint prevUL = 0;
			uint prevU = 0;
			uint prevL = 0;
			uint pred = 0;
			uint segment_id = 0;

			if ( AvailU != 0 && AvailL != 0 )
			{
				prevUL= SegmentIds[ MiRow - 1 ][ MiCol - 1 ];
			}
			else 
			{
				prevUL= -1;
			}

			if ( AvailU != 0 )
			{
				prevU= SegmentIds[ MiRow - 1 ][ MiCol ];
			}
			else 
			{
				prevU= -1;
			}

			if ( AvailL != 0 )
			{
				prevL= SegmentIds[ MiRow ][ MiCol - 1 ];
			}
			else 
			{
				prevL= -1;
			}

			if ( prevU == -1 )
			{
				pred= (prevL == -1) ? 0 : prevL;
			}
			else if ( prevL == -1 )
			{
				pred= prevU;
			}
			else 
			{
				pred= (prevUL == prevU) ? prevU : prevL;
			}

			if ( skip != 0 )
			{
				segment_id= pred;
			}
			else 
			{
				size += stream.ReadS(size, out this.segment_id, "segment_id"); 
				segment_id= neg_deinterleave( segment_id, pred, LastActiveSegId + 1 );
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint prevUL = 0;
			uint prevU = 0;
			uint prevL = 0;
			uint pred = 0;
			uint segment_id = 0;

			if ( AvailU != 0 && AvailL != 0 )
			{
				prevUL= SegmentIds[ MiRow - 1 ][ MiCol - 1 ];
			}
			else 
			{
				prevUL= -1;
			}

			if ( AvailU != 0 )
			{
				prevU= SegmentIds[ MiRow - 1 ][ MiCol ];
			}
			else 
			{
				prevU= -1;
			}

			if ( AvailL != 0 )
			{
				prevL= SegmentIds[ MiRow ][ MiCol - 1 ];
			}
			else 
			{
				prevL= -1;
			}

			if ( prevU == -1 )
			{
				pred= (prevL == -1) ? 0 : prevL;
			}
			else if ( prevL == -1 )
			{
				pred= prevU;
			}
			else 
			{
				pred= (prevUL == prevU) ? prevU : prevL;
			}

			if ( skip != 0 )
			{
				segment_id= pred;
			}
			else 
			{
				size += stream.WriteS( this.segment_id, "segment_id"); 
				segment_id= neg_deinterleave( segment_id, pred, LastActiveSegId + 1 );
			}

            return size;
         }

    }

    /*




neg_deinterleave(diff, refc, max) {
 if (! refc )
 return diff
 if ( refc >= ( max - 1 ))
 return max - diff - 1
 if(2 * refc < max){
 if(diff <= 2 * refc ){
 if(diff & 1)
 return refc + ( (diff+1) >> 1)
 else 
 return refc - (diff >> 1)
 }
 return diff
 } else {
 if (diff <=2 * (max - refc - 1)){
 if (diff & 1)
 return refc + ( (diff + 1) >>1)
 else 
 return refc - (diff >> 1 )
 }
 return max - ( diff + 1 )
 }
 }
    */
    public class NegDeinterleave : IAomSerializable
    {
		private uint diff;
		public uint Diff { get { return diff; } set { diff = value; } }
		private uint refc;
		public uint Refc { get { return refc; } set { refc = value; } }
		private uint max;
		public uint Max { get { return max; } set { max = value; } }

         public NegDeinterleave(uint diff, uint refc, uint max)
         { 
			this.diff = diff;
			this.refc = refc;
			this.max = max;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;


			if ( refc== 0 )
			{
return diff;
			}

			if ( refc >= ( max - 1 ))
			{
return max - diff - 1;
			}

			if (2 * refc < max)
			{

				if (diff <= 2 * refc )
				{

					if (diff & 1 != 0)
					{
return refc + ( (diff+1) >> 1);
					}
					else 
					{
return refc - (diff >> 1);
					}
				}
return diff;
			}
			else 
			{

				if (diff <=2 * (max - refc - 1))
				{

					if (diff & 1 != 0)
					{
return refc + ( (diff + 1) >>1);
					}
					else 
					{
return refc - (diff >> 1 );
					}
				}
return max - ( diff + 1 );
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;


			if ( refc== 0 )
			{
return diff;
			}

			if ( refc >= ( max - 1 ))
			{
return max - diff - 1;
			}

			if (2 * refc < max)
			{

				if (diff <= 2 * refc )
				{

					if (diff & 1 != 0)
					{
return refc + ( (diff+1) >> 1);
					}
					else 
					{
return refc - (diff >> 1);
					}
				}
return diff;
			}
			else 
			{

				if (diff <=2 * (max - refc - 1))
				{

					if (diff & 1 != 0)
					{
return refc + ( (diff + 1) >>1);
					}
					else 
					{
return refc - (diff >> 1 );
					}
				}
return max - ( diff + 1 );
			}

            return size;
         }

    }

    /*




read_skip_mode() { 
 if ( seg_feature_active( SEG_LVL_SKIP ) ||
 seg_feature_active( SEG_LVL_REF_FRAME ) ||
 seg_feature_active( SEG_LVL_GLOBALMV ) ||
 !skip_mode_present ||
 Block_Width[ MiSize ] < 8 ||
 Block_Height[ MiSize ] < 8 ) {
 skip_mode = 0
 } else {
 skip_mode S()
 }
 }
    */
    public class ReadSkipMode : IAomSerializable
    {
		private uint skip_mode;
		public uint SkipMode { get { return skip_mode; } set { skip_mode = value; } }

         public ReadSkipMode()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint skip_mode = 0;

			if ( seg_feature_active( SEG_LVL_SKIP ) ||
 seg_feature_active( SEG_LVL_REF_FRAME ) ||
 seg_feature_active( SEG_LVL_GLOBALMV ) ||
 skip_mode_present== 0 ||
 Block_Width[ MiSize ] < 8 ||
 Block_Height[ MiSize ] < 8 )
			{
				skip_mode= 0;
			}
			else 
			{
				size += stream.ReadS(size, out this.skip_mode, "skip_mode"); 
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint skip_mode = 0;

			if ( seg_feature_active( SEG_LVL_SKIP ) ||
 seg_feature_active( SEG_LVL_REF_FRAME ) ||
 seg_feature_active( SEG_LVL_GLOBALMV ) ||
 skip_mode_present== 0 ||
 Block_Width[ MiSize ] < 8 ||
 Block_Height[ MiSize ] < 8 )
			{
				skip_mode= 0;
			}
			else 
			{
				size += stream.WriteS( this.skip_mode, "skip_mode"); 
			}

            return size;
         }

    }

    /*



read_skip() { 
 if ( SegIdPreSkip && seg_feature_active( SEG_LVL_SKIP ) ) {
 skip = 1
 } else {
 skip S()
 }
 }
    */
    public class ReadSkip : IAomSerializable
    {
		private uint skip;
		public uint Skip { get { return skip; } set { skip = value; } }

         public ReadSkip()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint skip = 0;

			if ( SegIdPreSkip != 0 && seg_feature_active( SEG_LVL_SKIP ) )
			{
				skip= 1;
			}
			else 
			{
				size += stream.ReadS(size, out this.skip, "skip"); 
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint skip = 0;

			if ( SegIdPreSkip != 0 && seg_feature_active( SEG_LVL_SKIP ) )
			{
				skip= 1;
			}
			else 
			{
				size += stream.WriteS( this.skip, "skip"); 
			}

            return size;
         }

    }

    /*



read_delta_qindex() { 
 sbSize = use_128x128_superblock ? BLOCK_128X128 : BLOCK_64X64
 if ( MiSize == sbSize && skip )
 return
 if ( ReadDeltas ) {
 delta_q_abs S()
 if ( delta_q_abs == DELTA_Q_SMALL ) {
 delta_q_rem_bits L(3)
 delta_q_rem_bits++
 delta_q_abs_bits L(delta_q_rem_bits)
 delta_q_abs = delta_q_abs_bits + (1 << delta_q_rem_bits) + 1
 }
 if ( delta_q_abs ) {
 delta_q_sign_bit L(1)
 reducedDeltaQIndex = delta_q_sign_bit ? -delta_q_abs : delta_q_abs
 CurrentQIndex = Clip3(1, 255, CurrentQIndex + (reducedDeltaQIndex << delta_q_res))
 }
 }
 }
    */
    public class ReadDeltaQindex : IAomSerializable
    {
		private uint delta_q_abs;
		public uint DeltaqAbs { get { return delta_q_abs; } set { delta_q_abs = value; } }
		private uint delta_q_rem_bits;
		public uint DeltaqRemBits { get { return delta_q_rem_bits; } set { delta_q_rem_bits = value; } }
		private uint delta_q_abs_bits;
		public uint DeltaqAbsBits { get { return delta_q_abs_bits; } set { delta_q_abs_bits = value; } }
		private uint delta_q_sign_bit;
		public uint DeltaqSignBit { get { return delta_q_sign_bit; } set { delta_q_sign_bit = value; } }

         public ReadDeltaQindex()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint sbSize = 0;
			uint delta_q_abs = 0;
			uint reducedDeltaQIndex = 0;
			uint CurrentQIndex = 0;
			sbSize= use_128x128_superblock ? BLOCK_128X128 : BLOCK_64X64;

			if ( MiSize == sbSize && skip != 0 )
			{
return;
			}

			if ( ReadDeltas != 0 )
			{
				size += stream.ReadS(size, out this.delta_q_abs, "delta_q_abs"); 

				if ( delta_q_abs == DELTA_Q_SMALL )
				{
					size += stream.ReadL(size, 3, out this.delta_q_rem_bits, "delta_q_rem_bits"); 
					delta_q_rem_bits++;
					size += stream.ReadL(size, delta_q_rem_bits, out this.delta_q_abs_bits, "delta_q_abs_bits"); 
					delta_q_abs= delta_q_abs_bits + (1 << delta_q_rem_bits) + 1;
				}

				if ( delta_q_abs != 0 )
				{
					size += stream.ReadL(size, 1, out this.delta_q_sign_bit, "delta_q_sign_bit"); 
					reducedDeltaQIndex= delta_q_sign_bit ? -delta_q_abs : delta_q_abs;
					CurrentQIndex= Clip3(1, 255, CurrentQIndex + (reducedDeltaQIndex << delta_q_res));
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint sbSize = 0;
			uint delta_q_abs = 0;
			uint reducedDeltaQIndex = 0;
			uint CurrentQIndex = 0;
			sbSize= use_128x128_superblock ? BLOCK_128X128 : BLOCK_64X64;

			if ( MiSize == sbSize && skip != 0 )
			{
return;
			}

			if ( ReadDeltas != 0 )
			{
				size += stream.WriteS( this.delta_q_abs, "delta_q_abs"); 

				if ( delta_q_abs == DELTA_Q_SMALL )
				{
					size += stream.WriteL(3,  this.delta_q_rem_bits, "delta_q_rem_bits"); 
					delta_q_rem_bits++;
					size += stream.WriteL(delta_q_rem_bits,  this.delta_q_abs_bits, "delta_q_abs_bits"); 
					delta_q_abs= delta_q_abs_bits + (1 << delta_q_rem_bits) + 1;
				}

				if ( delta_q_abs != 0 )
				{
					size += stream.WriteL(1,  this.delta_q_sign_bit, "delta_q_sign_bit"); 
					reducedDeltaQIndex= delta_q_sign_bit ? -delta_q_abs : delta_q_abs;
					CurrentQIndex= Clip3(1, 255, CurrentQIndex + (reducedDeltaQIndex << delta_q_res));
				}
			}

            return size;
         }

    }

    /*



read_delta_lf() { 
 sbSize = use_128x128_superblock ? BLOCK_128X128 : BLOCK_64X64
 if ( MiSize == sbSize && skip )
 return
 if ( ReadDeltas && delta_lf_present ) {
 frameLfCount = 1
 if ( delta_lf_multi ) {
 frameLfCount = ( NumPlanes > 1 ) ? FRAME_LF_COUNT : ( FRAME_LF_COUNT - 2 )
 }
 for ( i = 0; i < frameLfCount; i++ ) {
 delta_lf_abs S()
 if ( delta_lf_abs == DELTA_LF_SMALL ) {
 delta_lf_rem_bits L(3)
 n = delta_lf_rem_bits + 1
 delta_lf_abs_bits L(n)
 deltaLfAbs = delta_lf_abs_bits + ( 1 << n ) + 1
 } else {
 deltaLfAbs = delta_lf_abs
 }
 if ( deltaLfAbs ) {
 delta_lf_sign_bit L(1)
 reducedDeltaLfLevel = delta_lf_sign_bit ?-deltaLfAbs : deltaLfAbs
 DeltaLF[ i ] = Clip3( -MAX_LOOP_FILTER, MAX_LOOP_FILTER, DeltaLF[ i ] + (reducedDeltaLfLevel << delta_lf_res) )
 }
 }
 }
 }
    */
    public class ReadDeltaLf : IAomSerializable
    {
		private uint[] delta_lf_abs;
		public uint[] DeltaLfAbs { get { return delta_lf_abs; } set { delta_lf_abs = value; } }
		private uint[] delta_lf_rem_bits;
		public uint[] DeltaLfRemBits { get { return delta_lf_rem_bits; } set { delta_lf_rem_bits = value; } }
		private uint[] delta_lf_abs_bits;
		public uint[] DeltaLfAbsBits { get { return delta_lf_abs_bits; } set { delta_lf_abs_bits = value; } }
		private uint[] delta_lf_sign_bit;
		public uint[] DeltaLfSignBit { get { return delta_lf_sign_bit; } set { delta_lf_sign_bit = value; } }

         public ReadDeltaLf()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint sbSize = 0;
			uint frameLfCount = 0;
			uint i = 0;
			uint n = 0;
			uint deltaLfAbs = 0;
			uint reducedDeltaLfLevel = 0;
			uint[] DeltaLF = null;
			sbSize= use_128x128_superblock ? BLOCK_128X128 : BLOCK_64X64;

			if ( MiSize == sbSize && skip != 0 )
			{
return;
			}

			if ( ReadDeltas != 0 && delta_lf_present != 0 )
			{
				frameLfCount= 1;

				if ( delta_lf_multi != 0 )
				{
					frameLfCount= ( NumPlanes > 1 ) ? FRAME_LF_COUNT : ( FRAME_LF_COUNT - 2 );
				}

				this.delta_lf_abs = new uint[ frameLfCount];
				this.delta_lf_rem_bits = new uint[ frameLfCount];
				this.delta_lf_abs_bits = new uint[ frameLfCount];
				this.delta_lf_sign_bit = new uint[ frameLfCount];
				for ( i = 0; i < frameLfCount; i++ )
				{
					size += stream.ReadS(size, out this.delta_lf_abs[ i ], "delta_lf_abs"); 

					if ( delta_lf_abs[i] == DELTA_LF_SMALL )
					{
						size += stream.ReadL(size, 3, out this.delta_lf_rem_bits[ i ], "delta_lf_rem_bits"); 
						n= delta_lf_rem_bits[i] + 1;
						size += stream.ReadL(size, n, out this.delta_lf_abs_bits[ i ], "delta_lf_abs_bits"); 
						deltaLfAbs= delta_lf_abs[i]_bits[i] + ( 1 << n ) + 1;
					}
					else 
					{
						deltaLfAbs= delta_lf_abs[i];
					}

					if ( deltaLfAbs != 0 )
					{
						size += stream.ReadL(size, 1, out this.delta_lf_sign_bit[ i ], "delta_lf_sign_bit"); 
						reducedDeltaLfLevel= delta_lf_sign_bit[i] ?-deltaLfAbs : deltaLfAbs;
						DeltaLF[ i ]= Clip3( -MAX_LOOP_FILTER, MAX_LOOP_FILTER, DeltaLF[ i ] + (reducedDeltaLfLevel << delta_lf_res) );
					}
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint sbSize = 0;
			uint frameLfCount = 0;
			uint i = 0;
			uint n = 0;
			uint deltaLfAbs = 0;
			uint reducedDeltaLfLevel = 0;
			uint[] DeltaLF = null;
			sbSize= use_128x128_superblock ? BLOCK_128X128 : BLOCK_64X64;

			if ( MiSize == sbSize && skip != 0 )
			{
return;
			}

			if ( ReadDeltas != 0 && delta_lf_present != 0 )
			{
				frameLfCount= 1;

				if ( delta_lf_multi != 0 )
				{
					frameLfCount= ( NumPlanes > 1 ) ? FRAME_LF_COUNT : ( FRAME_LF_COUNT - 2 );
				}

				for ( i = 0; i < frameLfCount; i++ )
				{
					size += stream.WriteS( this.delta_lf_abs[ i ], "delta_lf_abs"); 

					if ( delta_lf_abs[i] == DELTA_LF_SMALL )
					{
						size += stream.WriteL(3,  this.delta_lf_rem_bits[ i ], "delta_lf_rem_bits"); 
						n= delta_lf_rem_bits[i] + 1;
						size += stream.WriteL(n,  this.delta_lf_abs_bits[ i ], "delta_lf_abs_bits"); 
						deltaLfAbs= delta_lf_abs[i]_bits[i] + ( 1 << n ) + 1;
					}
					else 
					{
						deltaLfAbs= delta_lf_abs[i];
					}

					if ( deltaLfAbs != 0 )
					{
						size += stream.WriteL(1,  this.delta_lf_sign_bit[ i ], "delta_lf_sign_bit"); 
						reducedDeltaLfLevel= delta_lf_sign_bit[i] ?-deltaLfAbs : deltaLfAbs;
						DeltaLF[ i ]= Clip3( -MAX_LOOP_FILTER, MAX_LOOP_FILTER, DeltaLF[ i ] + (reducedDeltaLfLevel << delta_lf_res) );
					}
				}
			}

            return size;
         }

    }

    /*



seg_feature_active_idx( idx, feature ) { 
 return segmentation_enabled && FeatureEnabled[ idx ][ feature ]
 }
    */
    public class SegFeatureActiveIdx : IAomSerializable
    {
		private uint idx;
		public uint Idx { get { return idx; } set { idx = value; } }
		private uint feature;
		public uint Feature { get { return feature; } set { feature = value; } }

         public SegFeatureActiveIdx(uint idx, uint feature)
         { 
			this.idx = idx;
			this.feature = feature;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

return segmentation_enabled && FeatureEnabled[ idx ][ feature ];

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

return segmentation_enabled && FeatureEnabled[ idx ][ feature ];

            return size;
         }

    }

    /*

 seg_feature_active( feature ) {
 return seg_feature_active_idx( segment_id, feature )
 }
    */
    public class SegFeatureActive : IAomSerializable
    {
		private uint feature;
		public uint Feature { get { return feature; } set { feature = value; } }

         public SegFeatureActive(uint feature)
         { 
			this.feature = feature;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

return seg_feature_active_idx( segment_id, feature );

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

return seg_feature_active_idx( segment_id, feature );

            return size;
         }

    }

    /*



 read_tx_size( allowSelect ) { 
 if ( Lossless ) {
 TxSize = TX_4X4
 return
 }
 maxRectTxSize = Max_Tx_Size_Rect[ MiSize ]
 maxTxDepth = Max_Tx_Depth[ MiSize ]
 TxSize = maxRectTxSize
 if ( MiSize > BLOCK_4X4 && allowSelect && TxMode == TX_MODE_SELECT ) {
 tx_depth S()
 for ( i = 0; i < tx_depth; i++ )
 TxSize = Split_Tx_Size[ TxSize ]
 }
 }
    */
    public class ReadTxSize : IAomSerializable
    {
		private uint allowSelect;
		public uint AllowSelect { get { return allowSelect; } set { allowSelect = value; } }
		private uint tx_depth;
		public uint TxDepth { get { return tx_depth; } set { tx_depth = value; } }

         public ReadTxSize(uint allowSelect)
         { 
			this.allowSelect = allowSelect;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint TxSize = 0;
			uint maxRectTxSize = 0;
			uint maxTxDepth = 0;
			uint i = 0;

			if ( Lossless != 0 )
			{
				TxSize= TX_4X4;
return;
			}
			maxRectTxSize= Max_Tx_Size_Rect[ MiSize ];
			maxTxDepth= Max_Tx_Depth[ MiSize ];
			TxSize= maxRectTxSize;

			if ( MiSize > BLOCK_4X4 && allowSelect != 0 && TxMode == TX_MODE_SELECT )
			{
				size += stream.ReadS(size, out this.tx_depth, "tx_depth"); 

				for ( i = 0; i < tx_depth; i++ )
				{
					TxSize= Split_Tx_Size[ TxSize ];
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint TxSize = 0;
			uint maxRectTxSize = 0;
			uint maxTxDepth = 0;
			uint i = 0;

			if ( Lossless != 0 )
			{
				TxSize= TX_4X4;
return;
			}
			maxRectTxSize= Max_Tx_Size_Rect[ MiSize ];
			maxTxDepth= Max_Tx_Depth[ MiSize ];
			TxSize= maxRectTxSize;

			if ( MiSize > BLOCK_4X4 && allowSelect != 0 && TxMode == TX_MODE_SELECT )
			{
				size += stream.WriteS( this.tx_depth, "tx_depth"); 

				for ( i = 0; i < tx_depth; i++ )
				{
					TxSize= Split_Tx_Size[ TxSize ];
				}
			}

            return size;
         }

    }

    /*



read_block_tx_size() { 
 bw4 = Num_4x4_Blocks_Wide[ MiSize ]
 bh4 = Num_4x4_Blocks_High[ MiSize ]
 if ( TxMode == TX_MODE_SELECT &&
 MiSize > BLOCK_4X4 && is_inter &&
 !skip && !Lossless ) {
 maxTxSz = Max_Tx_Size_Rect[ MiSize ]
 txW4 = Tx_Width[ maxTxSz ] / MI_SIZE
 txH4 = Tx_Height[ maxTxSz ] / MI_SIZE
 for ( row = MiRow; row < MiRow + bh4; row += txH4 )
 for ( col = MiCol; col < MiCol + bw4; col += txW4 )
 read_var_tx_size( row, col, maxTxSz, 0 )
 } else {
 read_tx_size(!skip || !is_inter)
 for ( row = MiRow; row < MiRow + bh4; row++ )
 for ( col = MiCol; col < MiCol + bw4; col++ )
 InterTxSizes[ row ][ col ] = TxSize
 }
 }
    */
    public class ReadBlockTxSize : IAomSerializable
    {
		private ReadVarTxSize[][] read_var_tx_size;
		public ReadVarTxSize[][] ReadVarTxSize { get { return read_var_tx_size; } set { read_var_tx_size = value; } }
		private ReadTxSize read_tx_size;
		public ReadTxSize ReadTxSize { get { return read_tx_size; } set { read_tx_size = value; } }

         public ReadBlockTxSize()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint bw4 = 0;
			uint bh4 = 0;
			uint maxTxSz = 0;
			uint txW4 = 0;
			uint txH4 = 0;
			uint row = 0;
			uint col = 0;
			uint[][] InterTxSizes = null;
			bw4= Num_4x4_Blocks_Wide[ MiSize ];
			bh4= Num_4x4_Blocks_High[ MiSize ];

			if ( TxMode == TX_MODE_SELECT &&
 MiSize > BLOCK_4X4 && is_inter != 0 &&
 skip== 0 && Lossless== 0 )
			{
				maxTxSz= Max_Tx_Size_Rect[ MiSize ];
				txW4= Tx_Width[ maxTxSz ] / MI_SIZE;
				txH4= Tx_Height[ maxTxSz ] / MI_SIZE;

				this.read_var_tx_size = new ReadVarTxSize[ MiRow + bh4][];
				for ( row = MiRow; row < MiRow + bh4; row += txH4 )
				{

					this.read_var_tx_size[ row ] = new ReadVarTxSize[ MiCol + bw4];
					for ( col = MiCol; col < MiCol + bw4; col += txW4 )
					{
						this.read_var_tx_size[ row ][ col ] =  new ReadVarTxSize( row,  col,  maxTxSz,  0 ) ;
						size +=  stream.ReadClass<ReadVarTxSize>(size, context, this.read_var_tx_size[ row ][ col ], "read_var_tx_size"); 
					}
				}
			}
			else 
			{
				this.read_tx_size =  new ReadTxSize(!skip || !is_inter) ;
				size +=  stream.ReadClass<ReadTxSize>(size, context, this.read_tx_size, "read_tx_size"); 

				for ( row = MiRow; row < MiRow + bh4; row++ )
				{

					for ( col = MiCol; col < MiCol + bw4; col++ )
					{
						InterTxSizes[ row ][ col ]= TxSize;
					}
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint bw4 = 0;
			uint bh4 = 0;
			uint maxTxSz = 0;
			uint txW4 = 0;
			uint txH4 = 0;
			uint row = 0;
			uint col = 0;
			uint[][] InterTxSizes = null;
			bw4= Num_4x4_Blocks_Wide[ MiSize ];
			bh4= Num_4x4_Blocks_High[ MiSize ];

			if ( TxMode == TX_MODE_SELECT &&
 MiSize > BLOCK_4X4 && is_inter != 0 &&
 skip== 0 && Lossless== 0 )
			{
				maxTxSz= Max_Tx_Size_Rect[ MiSize ];
				txW4= Tx_Width[ maxTxSz ] / MI_SIZE;
				txH4= Tx_Height[ maxTxSz ] / MI_SIZE;

				for ( row = MiRow; row < MiRow + bh4; row += txH4 )
				{

					for ( col = MiCol; col < MiCol + bw4; col += txW4 )
					{
						size += stream.WriteClass<ReadVarTxSize>(context, this.read_var_tx_size[ row ][ col ], "read_var_tx_size"); 
					}
				}
			}
			else 
			{
				size += stream.WriteClass<ReadTxSize>(context, this.read_tx_size, "read_tx_size"); 

				for ( row = MiRow; row < MiRow + bh4; row++ )
				{

					for ( col = MiCol; col < MiCol + bw4; col++ )
					{
						InterTxSizes[ row ][ col ]= TxSize;
					}
				}
			}

            return size;
         }

    }

    /*



read_var_tx_size( row, col, txSz, depth) { 
 if ( row >= MiRows || col >= MiCols )
 return
 if ( txSz == TX_4X4 || depth == MAX_VARTX_DEPTH ) {
 txfm_split = 0
 } else {
 txfm_split S()
 }
 w4 = Tx_Width[ txSz ] / MI_SIZE
 h4 = Tx_Height[ txSz ] / MI_SIZE
 if ( txfm_split ) {
 subTxSz = Split_Tx_Size[ txSz ]
 stepW = Tx_Width[ subTxSz ] / MI_SIZE
 stepH = Tx_Height[ subTxSz ] / MI_SIZE
 for ( i = 0; i < h4; i += stepH )
 for ( j = 0; j < w4; j += stepW )
 read_var_tx_size( row + i, col + j, subTxSz, depth+1)
 } else {
 for ( i = 0; i < h4; i++ )
 for ( j = 0; j < w4; j++ )
 InterTxSizes[ row + i ][ col + j ] = txSz
 TxSize = txSz
 }
 }
    */
    public class ReadVarTxSize : IAomSerializable
    {
		private uint row;
		public uint Row { get { return row; } set { row = value; } }
		private uint col;
		public uint Col { get { return col; } set { col = value; } }
		private uint txSz;
		public uint TxSz { get { return txSz; } set { txSz = value; } }
		private uint depth;
		public uint Depth { get { return depth; } set { depth = value; } }
		private uint txfm_split;
		public uint TxfmSplit { get { return txfm_split; } set { txfm_split = value; } }
		private ReadVarTxSize[][] read_var_tx_size;
		public ReadVarTxSize[][] _ReadVarTxSize { get { return read_var_tx_size; } set { read_var_tx_size = value; } }

         public ReadVarTxSize(uint row, uint col, uint txSz, uint depth)
         { 
			this.row = row;
			this.col = col;
			this.txSz = txSz;
			this.depth = depth;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint txfm_split = 0;
			uint w4 = 0;
			uint h4 = 0;
			uint subTxSz = 0;
			uint stepW = 0;
			uint stepH = 0;
			uint i = 0;
			uint j = 0;
			uint[][] InterTxSizes = null;
			uint TxSize = 0;

			if ( row >= MiRows || col >= MiCols )
			{
return;
			}

			if ( txSz == TX_4X4 || depth == MAX_VARTX_DEPTH )
			{
				txfm_split= 0;
			}
			else 
			{
				size += stream.ReadS(size, out this.txfm_split, "txfm_split"); 
			}
			w4= Tx_Width[ txSz ] / MI_SIZE;
			h4= Tx_Height[ txSz ] / MI_SIZE;

			if ( txfm_split != 0 )
			{
				subTxSz= Split_Tx_Size[ txSz ];
				stepW= Tx_Width[ subTxSz ] / MI_SIZE;
				stepH= Tx_Height[ subTxSz ] / MI_SIZE;

				this.read_var_tx_size = new ReadVarTxSize[ h4][];
				for ( i = 0; i < h4; i += stepH )
				{

					this.read_var_tx_size[ i ] = new ReadVarTxSize[ w4];
					for ( j = 0; j < w4; j += stepW )
					{
						this.read_var_tx_size[ i ][ j ] =  new ReadVarTxSize( row + i,  col + j,  subTxSz,  depth+1) ;
						size +=  stream.ReadClass<ReadVarTxSize>(size, context, this.read_var_tx_size[ i ][ j ], "read_var_tx_size"); 
					}
				}
			}
			else 
			{

				for ( i = 0; i < h4; i++ )
				{

					for ( j = 0; j < w4; j++ )
					{
						InterTxSizes[ row + i ][ col + j ]= txSz;
					}
				}
				TxSize= txSz;
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint txfm_split = 0;
			uint w4 = 0;
			uint h4 = 0;
			uint subTxSz = 0;
			uint stepW = 0;
			uint stepH = 0;
			uint i = 0;
			uint j = 0;
			uint[][] InterTxSizes = null;
			uint TxSize = 0;

			if ( row >= MiRows || col >= MiCols )
			{
return;
			}

			if ( txSz == TX_4X4 || depth == MAX_VARTX_DEPTH )
			{
				txfm_split= 0;
			}
			else 
			{
				size += stream.WriteS( this.txfm_split, "txfm_split"); 
			}
			w4= Tx_Width[ txSz ] / MI_SIZE;
			h4= Tx_Height[ txSz ] / MI_SIZE;

			if ( txfm_split != 0 )
			{
				subTxSz= Split_Tx_Size[ txSz ];
				stepW= Tx_Width[ subTxSz ] / MI_SIZE;
				stepH= Tx_Height[ subTxSz ] / MI_SIZE;

				for ( i = 0; i < h4; i += stepH )
				{

					for ( j = 0; j < w4; j += stepW )
					{
						size += stream.WriteClass<ReadVarTxSize>(context, this.read_var_tx_size[ i ][ j ], "read_var_tx_size"); 
					}
				}
			}
			else 
			{

				for ( i = 0; i < h4; i++ )
				{

					for ( j = 0; j < w4; j++ )
					{
						InterTxSizes[ row + i ][ col + j ]= txSz;
					}
				}
				TxSize= txSz;
			}

            return size;
         }

    }

    /*



inter_frame_mode_info() { 
 use_intrabc = 0
 LeftRefFrame[ 0 ] = AvailL ? RefFrames[ MiRow ][ MiCol-1 ][ 0 ] : INTRA_FRAME
 AboveRefFrame[ 0 ] = AvailU ? RefFrames[ MiRow-1 ][ MiCol ][ 0 ] : INTRA_FRAME
 LeftRefFrame[ 1 ] = AvailL ? RefFrames[ MiRow ][ MiCol-1 ][ 1 ] : NONE
 AboveRefFrame[ 1 ] = AvailU ? RefFrames[ MiRow-1 ][ MiCol ][ 1 ] : NONE
 LeftIntra = LeftRefFrame[ 0 ] <= INTRA_FRAME
 AboveIntra = AboveRefFrame[ 0 ] <= INTRA_FRAME
 LeftSingle = LeftRefFrame[ 1 ] <= INTRA_FRAME
 AboveSingle = AboveRefFrame[ 1 ] <= INTRA_FRAME
 skip = 0
 inter_segment_id( 1 )
 read_skip_mode()
 if ( skip_mode )
 skip = 1
 else
 read_skip()
 if ( !SegIdPreSkip )
 inter_segment_id( 0 )
 Lossless = LosslessArray[ segment_id ]
 read_cdef()
 read_delta_qindex()
 read_delta_lf()
 ReadDeltas = 0
 read_is_inter()
 if ( is_inter )
 inter_block_mode_info()
 else
 intra_block_mode_info()
 }
    */
    public class InterFrameModeInfo : IAomSerializable
    {
		private InterSegmentId inter_segment_id;
		public InterSegmentId InterSegmentId { get { return inter_segment_id; } set { inter_segment_id = value; } }
		private ReadSkipMode read_skip_mode;
		public ReadSkipMode ReadSkipMode { get { return read_skip_mode; } set { read_skip_mode = value; } }
		private ReadSkip read_skip;
		public ReadSkip ReadSkip { get { return read_skip; } set { read_skip = value; } }
		private InterSegmentId inter_segment_id0;
		public InterSegmentId InterSegmentId0 { get { return inter_segment_id0; } set { inter_segment_id0 = value; } }
		private ReadCdef read_cdef;
		public ReadCdef ReadCdef { get { return read_cdef; } set { read_cdef = value; } }
		private ReadDeltaQindex read_delta_qindex;
		public ReadDeltaQindex ReadDeltaQindex { get { return read_delta_qindex; } set { read_delta_qindex = value; } }
		private ReadDeltaLf read_delta_lf;
		public ReadDeltaLf ReadDeltaLf { get { return read_delta_lf; } set { read_delta_lf = value; } }
		private ReadIsInter read_is_inter;
		public ReadIsInter ReadIsInter { get { return read_is_inter; } set { read_is_inter = value; } }
		private InterBlockModeInfo inter_block_mode_info;
		public InterBlockModeInfo InterBlockModeInfo { get { return inter_block_mode_info; } set { inter_block_mode_info = value; } }
		private IntraBlockModeInfo intra_block_mode_info;
		public IntraBlockModeInfo IntraBlockModeInfo { get { return intra_block_mode_info; } set { intra_block_mode_info = value; } }

         public InterFrameModeInfo()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint use_intrabc = 0;
			uint[] LeftRefFrame = null;
			uint[] AboveRefFrame = null;
			uint LeftIntra = 0;
			uint AboveIntra = 0;
			uint LeftSingle = 0;
			uint AboveSingle = 0;
			uint skip = 0;
			uint Lossless = 0;
			uint ReadDeltas = 0;
			use_intrabc= 0;
			LeftRefFrame[ 0 ]= AvailL ? RefFrames[ MiRow ][ MiCol-1 ][ 0 ] : AV1RefFrames.INTRA_FRAME;
			AboveRefFrame[ 0 ]= AvailU ? RefFrames[ MiRow-1 ][ MiCol ][ 0 ] : AV1RefFrames.INTRA_FRAME;
			LeftRefFrame[ 1 ]= AvailL ? RefFrames[ MiRow ][ MiCol-1 ][ 1 ] : NONE;
			AboveRefFrame[ 1 ]= AvailU ? RefFrames[ MiRow-1 ][ MiCol ][ 1 ] : NONE;
			LeftIntra= LeftRefFrame[ 0 ] <= AV1RefFrames.INTRA_FRAME ? (uint)1 : (uint)0;
			AboveIntra= AboveRefFrame[ 0 ] <= AV1RefFrames.INTRA_FRAME ? (uint)1 : (uint)0;
			LeftSingle= LeftRefFrame[ 1 ] <= AV1RefFrames.INTRA_FRAME ? (uint)1 : (uint)0;
			AboveSingle= AboveRefFrame[ 1 ] <= AV1RefFrames.INTRA_FRAME ? (uint)1 : (uint)0;
			skip= 0;
			this.inter_segment_id =  new InterSegmentId( 1 ) ;
			size +=  stream.ReadClass<InterSegmentId>(size, context, this.inter_segment_id, "inter_segment_id"); 
			this.read_skip_mode =  new ReadSkipMode() ;
			size +=  stream.ReadClass<ReadSkipMode>(size, context, this.read_skip_mode, "read_skip_mode"); 

			if ( skip_mode != 0 )
			{
				skip= 1;
			}
			else 
			{
				this.read_skip =  new ReadSkip() ;
				size +=  stream.ReadClass<ReadSkip>(size, context, this.read_skip, "read_skip"); 
			}

			if ( SegIdPreSkip== 0 )
			{
				this.inter_segment_id0 =  new InterSegmentId( 0 ) ;
				size +=  stream.ReadClass<InterSegmentId>(size, context, this.inter_segment_id0, "inter_segment_id0"); 
			}
			Lossless= LosslessArray[ segment_id ];
			this.read_cdef =  new ReadCdef() ;
			size +=  stream.ReadClass<ReadCdef>(size, context, this.read_cdef, "read_cdef"); 
			this.read_delta_qindex =  new ReadDeltaQindex() ;
			size +=  stream.ReadClass<ReadDeltaQindex>(size, context, this.read_delta_qindex, "read_delta_qindex"); 
			this.read_delta_lf =  new ReadDeltaLf() ;
			size +=  stream.ReadClass<ReadDeltaLf>(size, context, this.read_delta_lf, "read_delta_lf"); 
			ReadDeltas= 0;
			this.read_is_inter =  new ReadIsInter() ;
			size +=  stream.ReadClass<ReadIsInter>(size, context, this.read_is_inter, "read_is_inter"); 

			if ( is_inter != 0 )
			{
				this.inter_block_mode_info =  new InterBlockModeInfo() ;
				size +=  stream.ReadClass<InterBlockModeInfo>(size, context, this.inter_block_mode_info, "inter_block_mode_info"); 
			}
			else 
			{
				this.intra_block_mode_info =  new IntraBlockModeInfo() ;
				size +=  stream.ReadClass<IntraBlockModeInfo>(size, context, this.intra_block_mode_info, "intra_block_mode_info"); 
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint use_intrabc = 0;
			uint[] LeftRefFrame = null;
			uint[] AboveRefFrame = null;
			uint LeftIntra = 0;
			uint AboveIntra = 0;
			uint LeftSingle = 0;
			uint AboveSingle = 0;
			uint skip = 0;
			uint Lossless = 0;
			uint ReadDeltas = 0;
			use_intrabc= 0;
			LeftRefFrame[ 0 ]= AvailL ? RefFrames[ MiRow ][ MiCol-1 ][ 0 ] : AV1RefFrames.INTRA_FRAME;
			AboveRefFrame[ 0 ]= AvailU ? RefFrames[ MiRow-1 ][ MiCol ][ 0 ] : AV1RefFrames.INTRA_FRAME;
			LeftRefFrame[ 1 ]= AvailL ? RefFrames[ MiRow ][ MiCol-1 ][ 1 ] : NONE;
			AboveRefFrame[ 1 ]= AvailU ? RefFrames[ MiRow-1 ][ MiCol ][ 1 ] : NONE;
			LeftIntra= LeftRefFrame[ 0 ] <= AV1RefFrames.INTRA_FRAME ? (uint)1 : (uint)0;
			AboveIntra= AboveRefFrame[ 0 ] <= AV1RefFrames.INTRA_FRAME ? (uint)1 : (uint)0;
			LeftSingle= LeftRefFrame[ 1 ] <= AV1RefFrames.INTRA_FRAME ? (uint)1 : (uint)0;
			AboveSingle= AboveRefFrame[ 1 ] <= AV1RefFrames.INTRA_FRAME ? (uint)1 : (uint)0;
			skip= 0;
			size += stream.WriteClass<InterSegmentId>(context, this.inter_segment_id, "inter_segment_id"); 
			size += stream.WriteClass<ReadSkipMode>(context, this.read_skip_mode, "read_skip_mode"); 

			if ( skip_mode != 0 )
			{
				skip= 1;
			}
			else 
			{
				size += stream.WriteClass<ReadSkip>(context, this.read_skip, "read_skip"); 
			}

			if ( SegIdPreSkip== 0 )
			{
				size += stream.WriteClass<InterSegmentId>(context, this.inter_segment_id0, "inter_segment_id0"); 
			}
			Lossless= LosslessArray[ segment_id ];
			size += stream.WriteClass<ReadCdef>(context, this.read_cdef, "read_cdef"); 
			size += stream.WriteClass<ReadDeltaQindex>(context, this.read_delta_qindex, "read_delta_qindex"); 
			size += stream.WriteClass<ReadDeltaLf>(context, this.read_delta_lf, "read_delta_lf"); 
			ReadDeltas= 0;
			size += stream.WriteClass<ReadIsInter>(context, this.read_is_inter, "read_is_inter"); 

			if ( is_inter != 0 )
			{
				size += stream.WriteClass<InterBlockModeInfo>(context, this.inter_block_mode_info, "inter_block_mode_info"); 
			}
			else 
			{
				size += stream.WriteClass<IntraBlockModeInfo>(context, this.intra_block_mode_info, "intra_block_mode_info"); 
			}

            return size;
         }

    }

    /*



inter_segment_id( preSkip ) { 
 if ( segmentation_enabled ) {
 predictedSegmentId = get_segment_id()
 if ( segmentation_update_map ) {
 if ( preSkip && !SegIdPreSkip ) {
 segment_id = 0
 return
 }
 if ( !preSkip ) {
 if ( skip ) {
 seg_id_predicted = 0
 for ( i = 0; i < Num_4x4_Blocks_Wide[ MiSize ]; i++ )
 AboveSegPredContext[ MiCol + i ] = seg_id_predicted
 for ( i = 0; i < Num_4x4_Blocks_High[ MiSize ]; i++ )
 LeftSegPredContext[ MiRow + i ] = seg_id_predicted
 read_segment_id()
 return
 }
 }
 if ( segmentation_temporal_update == 1 ) {
 seg_id_predicted S()
 if ( seg_id_predicted )
 segment_id = predictedSegmentId
 else
 read_segment_id()
 for ( i = 0; i < Num_4x4_Blocks_Wide[ MiSize ]; i++ )
 AboveSegPredContext[ MiCol + i ] = seg_id_predicted
 for ( i = 0; i < Num_4x4_Blocks_High[ MiSize ]; i++ )
 LeftSegPredContext[ MiRow + i ] = seg_id_predicted
 } else {
 read_segment_id()
 }
 } else {
 segment_id = predictedSegmentId
 }
 } else {
 segment_id = 0
 }
 }
    */
    public class InterSegmentId : IAomSerializable
    {
		private uint preSkip;
		public uint PreSkip { get { return preSkip; } set { preSkip = value; } }
		private ReadSegmentId read_segment_id;
		public ReadSegmentId ReadSegmentId { get { return read_segment_id; } set { read_segment_id = value; } }
		private uint seg_id_predicted;
		public uint SegIdPredicted { get { return seg_id_predicted; } set { seg_id_predicted = value; } }

         public InterSegmentId(uint preSkip)
         { 
			this.preSkip = preSkip;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint predictedSegmentId = 0;
			uint segment_id = 0;
			uint seg_id_predicted = 0;
			uint i = 0;
			uint[] AboveSegPredContext = null;
			uint[] LeftSegPredContext = null;

			if ( segmentation_enabled != 0 )
			{
				predictedSegmentId= get_segment_id();

				if ( segmentation_update_map != 0 )
				{

					if ( preSkip != 0 && SegIdPreSkip== 0 )
					{
						segment_id= 0;
return;
					}

					if ( preSkip== 0 )
					{

						if ( skip != 0 )
						{
							seg_id_predicted= 0;

							for ( i = 0; i < Num_4x4_Blocks_Wide[ MiSize ]; i++ )
							{
								AboveSegPredContext[ MiCol + i ]= seg_id_predicted;
							}

							for ( i = 0; i < Num_4x4_Blocks_High[ MiSize ]; i++ )
							{
								LeftSegPredContext[ MiRow + i ]= seg_id_predicted;
							}
							this.read_segment_id =  new ReadSegmentId() ;
							size +=  stream.ReadClass<ReadSegmentId>(size, context, this.read_segment_id, "read_segment_id"); 
return;
						}
					}

					if ( segmentation_temporal_update == 1 )
					{
						size += stream.ReadS(size, out this.seg_id_predicted, "seg_id_predicted"); 

						if ( seg_id_predicted != 0 )
						{
							segment_id= predictedSegmentId;
						}
						else 
						{
							this.read_segment_id =  new ReadSegmentId() ;
							size +=  stream.ReadClass<ReadSegmentId>(size, context, this.read_segment_id, "read_segment_id"); 
						}

						for ( i = 0; i < Num_4x4_Blocks_Wide[ MiSize ]; i++ )
						{
							AboveSegPredContext[ MiCol + i ]= seg_id_predicted;
						}

						for ( i = 0; i < Num_4x4_Blocks_High[ MiSize ]; i++ )
						{
							LeftSegPredContext[ MiRow + i ]= seg_id_predicted;
						}
					}
					else 
					{
						this.read_segment_id =  new ReadSegmentId() ;
						size +=  stream.ReadClass<ReadSegmentId>(size, context, this.read_segment_id, "read_segment_id"); 
					}
				}
				else 
				{
					segment_id= predictedSegmentId;
				}
			}
			else 
			{
				segment_id= 0;
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint predictedSegmentId = 0;
			uint segment_id = 0;
			uint seg_id_predicted = 0;
			uint i = 0;
			uint[] AboveSegPredContext = null;
			uint[] LeftSegPredContext = null;

			if ( segmentation_enabled != 0 )
			{
				predictedSegmentId= get_segment_id();

				if ( segmentation_update_map != 0 )
				{

					if ( preSkip != 0 && SegIdPreSkip== 0 )
					{
						segment_id= 0;
return;
					}

					if ( preSkip== 0 )
					{

						if ( skip != 0 )
						{
							seg_id_predicted= 0;

							for ( i = 0; i < Num_4x4_Blocks_Wide[ MiSize ]; i++ )
							{
								AboveSegPredContext[ MiCol + i ]= seg_id_predicted;
							}

							for ( i = 0; i < Num_4x4_Blocks_High[ MiSize ]; i++ )
							{
								LeftSegPredContext[ MiRow + i ]= seg_id_predicted;
							}
							size += stream.WriteClass<ReadSegmentId>(context, this.read_segment_id, "read_segment_id"); 
return;
						}
					}

					if ( segmentation_temporal_update == 1 )
					{
						size += stream.WriteS( this.seg_id_predicted, "seg_id_predicted"); 

						if ( seg_id_predicted != 0 )
						{
							segment_id= predictedSegmentId;
						}
						else 
						{
							size += stream.WriteClass<ReadSegmentId>(context, this.read_segment_id, "read_segment_id"); 
						}

						for ( i = 0; i < Num_4x4_Blocks_Wide[ MiSize ]; i++ )
						{
							AboveSegPredContext[ MiCol + i ]= seg_id_predicted;
						}

						for ( i = 0; i < Num_4x4_Blocks_High[ MiSize ]; i++ )
						{
							LeftSegPredContext[ MiRow + i ]= seg_id_predicted;
						}
					}
					else 
					{
						size += stream.WriteClass<ReadSegmentId>(context, this.read_segment_id, "read_segment_id"); 
					}
				}
				else 
				{
					segment_id= predictedSegmentId;
				}
			}
			else 
			{
				segment_id= 0;
			}

            return size;
         }

    }

    /*



read_is_inter() { 
 if ( skip_mode ) {
 is_inter = 1
 } else if ( seg_feature_active ( SEG_LVL_REF_FRAME ) ) {
 is_inter = FeatureData[ segment_id ][ SEG_LVL_REF_FRAME ] != INTRA_FRAME
 } else if ( seg_feature_active ( SEG_LVL_GLOBALMV ) ) {
 is_inter = 1
 } else {
 is_inter S()
 }
 }
    */
    public class ReadIsInter : IAomSerializable
    {
		private uint is_inter;
		public uint IsInter { get { return is_inter; } set { is_inter = value; } }

         public ReadIsInter()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint is_inter = 0;

			if ( skip_mode != 0 )
			{
				is_inter= 1;
			}
			else if ( seg_feature_active ( SEG_LVL_REF_FRAME ) )
			{
				is_inter= FeatureData[ segment_id ][ SEG_LVL_REF_FRAME ] != AV1RefFrames.INTRA_FRAME;
			}
			else if ( seg_feature_active ( SEG_LVL_GLOBALMV ) )
			{
				is_inter= 1;
			}
			else 
			{
				size += stream.ReadS(size, out this.is_inter, "is_inter"); 
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint is_inter = 0;

			if ( skip_mode != 0 )
			{
				is_inter= 1;
			}
			else if ( seg_feature_active ( SEG_LVL_REF_FRAME ) )
			{
				is_inter= FeatureData[ segment_id ][ SEG_LVL_REF_FRAME ] != AV1RefFrames.INTRA_FRAME;
			}
			else if ( seg_feature_active ( SEG_LVL_GLOBALMV ) )
			{
				is_inter= 1;
			}
			else 
			{
				size += stream.WriteS( this.is_inter, "is_inter"); 
			}

            return size;
         }

    }

    /*



get_segment_id() { 
 bw4 = Num_4x4_Blocks_Wide[ MiSize ]
 bh4 = Num_4x4_Blocks_High[ MiSize ]
 xMis = Min( MiCols - MiCol, bw4 )
 yMis = Min( MiRows - MiRow, bh4 )
 seg = 7
 for ( y = 0; y < yMis; y++ )
 for ( x = 0; x < xMis; x++ )
 seg = Min( seg, PrevSegmentIds[ MiRow + y ][ MiCol + x ] )
 return seg
 }
    */
    public class GetSegmentId : IAomSerializable
    {

         public GetSegmentId()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint bw4 = 0;
			uint bh4 = 0;
			uint xMis = 0;
			uint yMis = 0;
			uint seg = 0;
			uint y = 0;
			uint x = 0;
			bw4= Num_4x4_Blocks_Wide[ MiSize ];
			bh4= Num_4x4_Blocks_High[ MiSize ];
			xMis= Math.Min( MiCols - MiCol, bw4 );
			yMis= Math.Min( MiRows - MiRow, bh4 );
			seg= 7;

			for ( y = 0; y < yMis; y++ )
			{

				for ( x = 0; x < xMis; x++ )
				{
					seg= Math.Min( seg, PrevSegmentIds[ MiRow + y ][ MiCol + x ] );
				}
			}
return seg;

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint bw4 = 0;
			uint bh4 = 0;
			uint xMis = 0;
			uint yMis = 0;
			uint seg = 0;
			uint y = 0;
			uint x = 0;
			bw4= Num_4x4_Blocks_Wide[ MiSize ];
			bh4= Num_4x4_Blocks_High[ MiSize ];
			xMis= Math.Min( MiCols - MiCol, bw4 );
			yMis= Math.Min( MiRows - MiRow, bh4 );
			seg= 7;

			for ( y = 0; y < yMis; y++ )
			{

				for ( x = 0; x < xMis; x++ )
				{
					seg= Math.Min( seg, PrevSegmentIds[ MiRow + y ][ MiCol + x ] );
				}
			}
return seg;

            return size;
         }

    }

    /*



intra_block_mode_info() { 
 RefFrame[ 0 ] = INTRA_FRAME
 RefFrame[ 1 ] = NONE
 y_mode S()
 YMode = y_mode
 intra_angle_info_y()
 if ( HasChroma ) {
  uv_mode S()
 UVMode = uv_mode
 if ( UVMode == UV_CFL_PRED ) {
 read_cfl_alphas()
 }
 intra_angle_info_uv()
 }
 PaletteSizeY = 0
 PaletteSizeUV = 0
 if ( MiSize >= BLOCK_8X8 &&
 Block_Width[ MiSize ] <= 64  &&
 Block_Height[ MiSize ] <= 64 &&
 allow_screen_content_tools )
 palette_mode_info()
 filter_intra_mode_info()
 }
    */
    public class IntraBlockModeInfo : IAomSerializable
    {
		private uint y_mode;
		public uint yMode { get { return y_mode; } set { y_mode = value; } }
		private IntraAngleInfoy intra_angle_info_y;
		public IntraAngleInfoy IntraAngleInfoy { get { return intra_angle_info_y; } set { intra_angle_info_y = value; } }
		private uint uv_mode;
		public uint UvMode { get { return uv_mode; } set { uv_mode = value; } }
		private ReadCflAlphas read_cfl_alphas;
		public ReadCflAlphas ReadCflAlphas { get { return read_cfl_alphas; } set { read_cfl_alphas = value; } }
		private IntraAngleInfoUv intra_angle_info_uv;
		public IntraAngleInfoUv IntraAngleInfoUv { get { return intra_angle_info_uv; } set { intra_angle_info_uv = value; } }
		private PaletteModeInfo palette_mode_info;
		public PaletteModeInfo PaletteModeInfo { get { return palette_mode_info; } set { palette_mode_info = value; } }
		private FilterIntraModeInfo filter_intra_mode_info;
		public FilterIntraModeInfo FilterIntraModeInfo { get { return filter_intra_mode_info; } set { filter_intra_mode_info = value; } }

         public IntraBlockModeInfo()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint[] RefFrame = null;
			uint YMode = 0;
			uint UVMode = 0;
			uint PaletteSizeY = 0;
			uint PaletteSizeUV = 0;
			RefFrame[ 0 ]= AV1RefFrames.INTRA_FRAME;
			RefFrame[ 1 ]= NONE;
			size += stream.ReadS(size, out this.y_mode, "y_mode"); 
			YMode= y_mode;
			this.intra_angle_info_y =  new IntraAngleInfoy() ;
			size +=  stream.ReadClass<IntraAngleInfoy>(size, context, this.intra_angle_info_y, "intra_angle_info_y"); 

			if ( HasChroma != 0 )
			{
				size += stream.ReadS(size, out this.uv_mode, "uv_mode"); 
				UVMode= uv_mode;

				if ( UVMode == UV_CFL_PRED )
				{
					this.read_cfl_alphas =  new ReadCflAlphas() ;
					size +=  stream.ReadClass<ReadCflAlphas>(size, context, this.read_cfl_alphas, "read_cfl_alphas"); 
				}
				this.intra_angle_info_uv =  new IntraAngleInfoUv() ;
				size +=  stream.ReadClass<IntraAngleInfoUv>(size, context, this.intra_angle_info_uv, "intra_angle_info_uv"); 
			}
			PaletteSizeY= 0;
			PaletteSizeUV= 0;

			if ( MiSize >= BLOCK_8X8 &&
 Block_Width[ MiSize ] <= 64  &&
 Block_Height[ MiSize ] <= 64 &&
 allow_screen_content_tools != 0 )
			{
				this.palette_mode_info =  new PaletteModeInfo() ;
				size +=  stream.ReadClass<PaletteModeInfo>(size, context, this.palette_mode_info, "palette_mode_info"); 
			}
			this.filter_intra_mode_info =  new FilterIntraModeInfo() ;
			size +=  stream.ReadClass<FilterIntraModeInfo>(size, context, this.filter_intra_mode_info, "filter_intra_mode_info"); 

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint[] RefFrame = null;
			uint YMode = 0;
			uint UVMode = 0;
			uint PaletteSizeY = 0;
			uint PaletteSizeUV = 0;
			RefFrame[ 0 ]= AV1RefFrames.INTRA_FRAME;
			RefFrame[ 1 ]= NONE;
			size += stream.WriteS( this.y_mode, "y_mode"); 
			YMode= y_mode;
			size += stream.WriteClass<IntraAngleInfoy>(context, this.intra_angle_info_y, "intra_angle_info_y"); 

			if ( HasChroma != 0 )
			{
				size += stream.WriteS( this.uv_mode, "uv_mode"); 
				UVMode= uv_mode;

				if ( UVMode == UV_CFL_PRED )
				{
					size += stream.WriteClass<ReadCflAlphas>(context, this.read_cfl_alphas, "read_cfl_alphas"); 
				}
				size += stream.WriteClass<IntraAngleInfoUv>(context, this.intra_angle_info_uv, "intra_angle_info_uv"); 
			}
			PaletteSizeY= 0;
			PaletteSizeUV= 0;

			if ( MiSize >= BLOCK_8X8 &&
 Block_Width[ MiSize ] <= 64  &&
 Block_Height[ MiSize ] <= 64 &&
 allow_screen_content_tools != 0 )
			{
				size += stream.WriteClass<PaletteModeInfo>(context, this.palette_mode_info, "palette_mode_info"); 
			}
			size += stream.WriteClass<FilterIntraModeInfo>(context, this.filter_intra_mode_info, "filter_intra_mode_info"); 

            return size;
         }

    }

    /*



inter_block_mode_info() { 
 PaletteSizeY = 0
 PaletteSizeUV = 0
 read_ref_frames()
 isCompound = RefFrame[ 1 ] > INTRA_FRAME
 find_mv_stack( isCompound )
 if ( skip_mode ) {
 YMode = NEAREST_NEARESTMV
 } else if ( seg_feature_active( SEG_LVL_SKIP ) ||
 seg_feature_active( SEG_LVL_GLOBALMV ) ) {
 YMode = GLOBALMV
 } else if ( isCompound ) {
 compound_mode S()
 YMode = NEAREST_NEARESTMV + compound_mode
 } else {
 new_mv S()
 if ( new_mv == 0 ) {
 YMode = NEWMV
 } else {
 zero_mv S()
 if ( zero_mv == 0 ) {
 YMode = GLOBALMV
 } else {
 ref_mv S()
 YMode = (ref_mv == 0) ? NEARESTMV : NEARMV
 }
 }
 }
 RefMvIdx = 0
 if ( YMode == NEWMV || YMode == NEW_NEWMV ) {
 for ( idx = 0; idx < 2; idx++ ) {
 if ( NumMvFound > idx + 1 ) {
 drl_mode S()
 if ( drl_mode == 0 ) {
 RefMvIdx = idx
 break
 }
 RefMvIdx = idx + 1
 }
 }
 } else if ( has_nearmv() ) {
 RefMvIdx = 1
 for ( idx = 1; idx < 3; idx++ ) {
 if ( NumMvFound > idx + 1 ) {
 drl_mode S()
 if ( drl_mode == 0 ) {
 RefMvIdx = idx
 break
 }
 RefMvIdx = idx + 1
 }
 }
 }
 assign_mv( isCompound )
 read_interintra_mode( isCompound )
 read_motion_mode( isCompound )
 read_compound_type( isCompound )
 if ( interpolation_filter == SWITCHABLE ) {
 for ( dir = 0; dir < ( enable_dual_filter ? 2 : 1 ); dir++ ) {
 if ( needs_interp_filter() ) {
 interp_filter[ dir ] S()
 } else {
 interp_filter[ dir ] = EIGHTTAP
 }
 }
 if ( !enable_dual_filter )
 interp_filter[ 1 ] = interp_filter[ 0 ]
 } else {
 for ( dir = 0; dir < 2; dir++ )
 interp_filter[ dir ] = interpolation_filter
 }
 }
    */
    public class InterBlockModeInfo : IAomSerializable
    {
		private ReadRefFrames read_ref_frames;
		public ReadRefFrames ReadRefFrames { get { return read_ref_frames; } set { read_ref_frames = value; } }
		private FindMvStack find_mv_stack;
		public FindMvStack FindMvStack { get { return find_mv_stack; } set { find_mv_stack = value; } }
		private uint compound_mode;
		public uint CompoundMode { get { return compound_mode; } set { compound_mode = value; } }
		private uint new_mv;
		public uint NewMv { get { return new_mv; } set { new_mv = value; } }
		private uint zero_mv;
		public uint ZeroMv { get { return zero_mv; } set { zero_mv = value; } }
		private uint ref_mv;
		public uint RefMv { get { return ref_mv; } set { ref_mv = value; } }
		private uint[] drl_mode;
		public uint[] DrlMode { get { return drl_mode; } set { drl_mode = value; } }
		private AssignMv assign_mv;
		public AssignMv AssignMv { get { return assign_mv; } set { assign_mv = value; } }
		private ReadInterintraMode read_interintra_mode;
		public ReadInterintraMode ReadInterintraMode { get { return read_interintra_mode; } set { read_interintra_mode = value; } }
		private ReadMotionMode read_motion_mode;
		public ReadMotionMode ReadMotionMode { get { return read_motion_mode; } set { read_motion_mode = value; } }
		private ReadCompoundType read_compound_type;
		public ReadCompoundType ReadCompoundType { get { return read_compound_type; } set { read_compound_type = value; } }
		private uint[] interp_filter;
		public uint[] InterpFilter { get { return interp_filter; } set { interp_filter = value; } }

         public InterBlockModeInfo()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint PaletteSizeY = 0;
			uint PaletteSizeUV = 0;
			uint isCompound = 0;
			uint YMode = 0;
			uint RefMvIdx = 0;
			uint idx = 0;
			uint dir = 0;
			uint[] interp_filter = null;
			PaletteSizeY= 0;
			PaletteSizeUV= 0;
			this.read_ref_frames =  new ReadRefFrames() ;
			size +=  stream.ReadClass<ReadRefFrames>(size, context, this.read_ref_frames, "read_ref_frames"); 
			isCompound= RefFrame[ 1 ] > AV1RefFrames.INTRA_FRAME ? (uint)1 : (uint)0;
			this.find_mv_stack =  new FindMvStack( isCompound ) ;
			size +=  stream.ReadClass<FindMvStack>(size, context, this.find_mv_stack, "find_mv_stack"); 

			if ( skip_mode != 0 )
			{
				YMode= NEAREST_NEARESTMV;
			}
			else if ( seg_feature_active( SEG_LVL_SKIP ) ||
 seg_feature_active( SEG_LVL_GLOBALMV ) )
			{
				YMode= GLOBALMV;
			}
			else if ( isCompound != 0 )
			{
				size += stream.ReadS(size, out this.compound_mode, "compound_mode"); 
				YMode= NEAREST_NEARESTMV + compound_mode;
			}
			else 
			{
				size += stream.ReadS(size, out this.new_mv, "new_mv"); 

				if ( new_mv == 0 )
				{
					YMode= NEWMV;
				}
				else 
				{
					size += stream.ReadS(size, out this.zero_mv, "zero_mv"); 

					if ( zero_mv == 0 )
					{
						YMode= GLOBALMV;
					}
					else 
					{
						size += stream.ReadS(size, out this.ref_mv, "ref_mv"); 
						YMode= (ref_mv == 0) ? NEARESTMV : NEARMV;
					}
				}
			}
			RefMvIdx= 0;

			if ( YMode == NEWMV || YMode == NEW_NEWMV )
			{

				this.drl_mode = new uint[ 2];
				for ( idx = 0; idx < 2; idx++ )
				{

					if ( NumMvFound > idx + 1 )
					{
						size += stream.ReadS(size, out this.drl_mode[ idx ], "drl_mode"); 

						if ( drl_mode[x] == 0 )
						{
							RefMvIdx= idx;
break;
						}
						RefMvIdx= idx + 1;
					}
				}
			}
			else if ( has_nearmv() )
			{
				RefMvIdx= 1;

				for ( idx = 1; idx < 3; idx++ )
				{

					if ( NumMvFound > idx + 1 )
					{
						size += stream.ReadS(size, out this.drl_mode[ idx ], "drl_mode"); 

						if ( drl_mode[x] == 0 )
						{
							RefMvIdx= idx;
break;
						}
						RefMvIdx= idx + 1;
					}
				}
			}
			this.assign_mv =  new AssignMv( isCompound ) ;
			size +=  stream.ReadClass<AssignMv>(size, context, this.assign_mv, "assign_mv"); 
			this.read_interintra_mode =  new ReadInterintraMode( isCompound ) ;
			size +=  stream.ReadClass<ReadInterintraMode>(size, context, this.read_interintra_mode, "read_interintra_mode"); 
			this.read_motion_mode =  new ReadMotionMode( isCompound ) ;
			size +=  stream.ReadClass<ReadMotionMode>(size, context, this.read_motion_mode, "read_motion_mode"); 
			this.read_compound_type =  new ReadCompoundType( isCompound ) ;
			size +=  stream.ReadClass<ReadCompoundType>(size, context, this.read_compound_type, "read_compound_type"); 

			if ( interpolation_filter == SWITCHABLE )
			{

				this.interp_filter = new uint[ ( enable_dual_filter ? 2 : 1 )];
				for ( dir = 0; dir < ( enable_dual_filter ? 2 : 1 ); dir++ )
				{

					if ( needs_interp_filter() )
					{
						size += stream.ReadS(size, out this.interp_filter[ dir ], "interp_filter"); 
					}
					else 
					{
						interp_filter[ dir ]= EIGHTTAP;
					}
				}

				if ( enable_dual_filter== 0 )
				{
					interp_filter[ 1 ]= interp_filter[ 0 ];
				}
			}
			else 
			{

				for ( dir = 0; dir < 2; dir++ )
				{
					interp_filter[ dir ]= interpolation_filter;
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint PaletteSizeY = 0;
			uint PaletteSizeUV = 0;
			uint isCompound = 0;
			uint YMode = 0;
			uint RefMvIdx = 0;
			uint idx = 0;
			uint dir = 0;
			uint[] interp_filter = null;
			PaletteSizeY= 0;
			PaletteSizeUV= 0;
			size += stream.WriteClass<ReadRefFrames>(context, this.read_ref_frames, "read_ref_frames"); 
			isCompound= RefFrame[ 1 ] > AV1RefFrames.INTRA_FRAME ? (uint)1 : (uint)0;
			size += stream.WriteClass<FindMvStack>(context, this.find_mv_stack, "find_mv_stack"); 

			if ( skip_mode != 0 )
			{
				YMode= NEAREST_NEARESTMV;
			}
			else if ( seg_feature_active( SEG_LVL_SKIP ) ||
 seg_feature_active( SEG_LVL_GLOBALMV ) )
			{
				YMode= GLOBALMV;
			}
			else if ( isCompound != 0 )
			{
				size += stream.WriteS( this.compound_mode, "compound_mode"); 
				YMode= NEAREST_NEARESTMV + compound_mode;
			}
			else 
			{
				size += stream.WriteS( this.new_mv, "new_mv"); 

				if ( new_mv == 0 )
				{
					YMode= NEWMV;
				}
				else 
				{
					size += stream.WriteS( this.zero_mv, "zero_mv"); 

					if ( zero_mv == 0 )
					{
						YMode= GLOBALMV;
					}
					else 
					{
						size += stream.WriteS( this.ref_mv, "ref_mv"); 
						YMode= (ref_mv == 0) ? NEARESTMV : NEARMV;
					}
				}
			}
			RefMvIdx= 0;

			if ( YMode == NEWMV || YMode == NEW_NEWMV )
			{

				for ( idx = 0; idx < 2; idx++ )
				{

					if ( NumMvFound > idx + 1 )
					{
						size += stream.WriteS( this.drl_mode[ idx ], "drl_mode"); 

						if ( drl_mode[x] == 0 )
						{
							RefMvIdx= idx;
break;
						}
						RefMvIdx= idx + 1;
					}
				}
			}
			else if ( has_nearmv() )
			{
				RefMvIdx= 1;

				for ( idx = 1; idx < 3; idx++ )
				{

					if ( NumMvFound > idx + 1 )
					{
						size += stream.WriteS( this.drl_mode[ idx ], "drl_mode"); 

						if ( drl_mode[x] == 0 )
						{
							RefMvIdx= idx;
break;
						}
						RefMvIdx= idx + 1;
					}
				}
			}
			size += stream.WriteClass<AssignMv>(context, this.assign_mv, "assign_mv"); 
			size += stream.WriteClass<ReadInterintraMode>(context, this.read_interintra_mode, "read_interintra_mode"); 
			size += stream.WriteClass<ReadMotionMode>(context, this.read_motion_mode, "read_motion_mode"); 
			size += stream.WriteClass<ReadCompoundType>(context, this.read_compound_type, "read_compound_type"); 

			if ( interpolation_filter == SWITCHABLE )
			{

				for ( dir = 0; dir < ( enable_dual_filter ? 2 : 1 ); dir++ )
				{

					if ( needs_interp_filter() )
					{
						size += stream.WriteS( this.interp_filter[ dir ], "interp_filter"); 
					}
					else 
					{
						interp_filter[ dir ]= EIGHTTAP;
					}
				}

				if ( enable_dual_filter== 0 )
				{
					interp_filter[ 1 ]= interp_filter[ 0 ];
				}
			}
			else 
			{

				for ( dir = 0; dir < 2; dir++ )
				{
					interp_filter[ dir ]= interpolation_filter;
				}
			}

            return size;
         }

    }

    /*




has_nearmv() {
 return (YMode== NEARMV||YMode ==NEAR_NEARMV ||YMode ==NEAR_NEWMV ||YMode ==NEW_NEARMV)
 }
    */
    public class HasNearmv : IAomSerializable
    {

         public HasNearmv()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

return (YMode== NEARMV||YMode ==NEAR_NEARMV ||YMode ==NEAR_NEWMV ||YMode ==NEW_NEARMV);

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

return (YMode== NEARMV||YMode ==NEAR_NEARMV ||YMode ==NEAR_NEWMV ||YMode ==NEW_NEARMV);

            return size;
         }

    }

    /*




  needs_interp_filter(){
 large=(Min(Block_Width[MiSize], Block_Height[MiSize]) >=8)
 if (skip_mode ||motion_mode ==LOCALWARP){
 return 0
 } else if(large &&YMode ==GLOBALMV){
 return GmType[RefFrame[0]]==TRANSLATION
 } else if(large &&YMode ==GLOBAL_GLOBALMV){
 return GmType[RefFrame[0]] ==TRANSLATION ||GmType[RefFrame[1]] ==TRANSLATION
 } else{
 return 1
 }
 }
    */
    public class NeedsInterpFilter : IAomSerializable
    {

         public NeedsInterpFilter()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint large = 0;
			large= (Math.Min(Block_Width[MiSize], Block_Height[MiSize]) >=8) ? (uint)1 : (uint)0;

			if (skip_mode != 0 ||motion_mode ==LOCALWARP)
			{
return 0;
			}
			else if (large != 0 &&YMode ==GLOBALMV)
			{
return GmType[RefFrame[0]]==TRANSLATION;
			}
			else if (large != 0 &&YMode ==GLOBAL_GLOBALMV)
			{
return GmType[RefFrame[0]] ==TRANSLATION ||GmType[RefFrame[1]] ==TRANSLATION;
			}
			else 
			{
return 1;
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint large = 0;
			large= (Math.Min(Block_Width[MiSize], Block_Height[MiSize]) >=8) ? (uint)1 : (uint)0;

			if (skip_mode != 0 ||motion_mode ==LOCALWARP)
			{
return 0;
			}
			else if (large != 0 &&YMode ==GLOBALMV)
			{
return GmType[RefFrame[0]]==TRANSLATION;
			}
			else if (large != 0 &&YMode ==GLOBAL_GLOBALMV)
			{
return GmType[RefFrame[0]] ==TRANSLATION ||GmType[RefFrame[1]] ==TRANSLATION;
			}
			else 
			{
return 1;
			}

            return size;
         }

    }

    /*



  filter_intra_mode_info() { 
 use_filter_intra = 0
 if ( enable_filter_intra &&
 YMode == DC_PRED && PaletteSizeY == 0 &&
 Max( Block_Width[ MiSize ], Block_Height[ MiSize ] ) <= 32 ) {
 use_filter_intra S()
 if ( use_filter_intra ) {
 filter_intra_mode S()
 }
 }
 }
    */
    public class FilterIntraModeInfo : IAomSerializable
    {
		private uint use_filter_intra;
		public uint UseFilterIntra { get { return use_filter_intra; } set { use_filter_intra = value; } }
		private uint filter_intra_mode;
		public uint FilterIntraMode { get { return filter_intra_mode; } set { filter_intra_mode = value; } }

         public FilterIntraModeInfo()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint use_filter_intra = 0;
			use_filter_intra= 0;

			if ( enable_filter_intra != 0 &&
 YMode == DC_PRED && PaletteSizeY == 0 &&
 Math.Max( Block_Width[ MiSize ], Block_Height[ MiSize ] ) <= 32 )
			{
				size += stream.ReadS(size, out this.use_filter_intra, "use_filter_intra"); 

				if ( use_filter_intra != 0 )
				{
					size += stream.ReadS(size, out this.filter_intra_mode, "filter_intra_mode"); 
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint use_filter_intra = 0;
			use_filter_intra= 0;

			if ( enable_filter_intra != 0 &&
 YMode == DC_PRED && PaletteSizeY == 0 &&
 Math.Max( Block_Width[ MiSize ], Block_Height[ MiSize ] ) <= 32 )
			{
				size += stream.WriteS( this.use_filter_intra, "use_filter_intra"); 

				if ( use_filter_intra != 0 )
				{
					size += stream.WriteS( this.filter_intra_mode, "filter_intra_mode"); 
				}
			}

            return size;
         }

    }

    /*



  read_ref_frames() { 
 if ( skip_mode ) {
 RefFrame[ 0 ] = SkipModeFrame[ 0 ]
 RefFrame[ 1 ] = SkipModeFrame[ 1 ]
 } else if ( seg_feature_active( SEG_LVL_REF_FRAME ) ) {
 RefFrame[ 0 ] = FeatureData[ segment_id ][ SEG_LVL_REF_FRAME ]
 RefFrame[ 1 ] = NONE
 } else if ( seg_feature_active( SEG_LVL_SKIP ) ||
 seg_feature_active( SEG_LVL_GLOBALMV ) ) {
 RefFrame[ 0 ] = LAST_FRAME
 RefFrame[ 1 ] = NONE
 } else {
 bw4 = Num_4x4_Blocks_Wide[ MiSize ]
 bh4 = Num_4x4_Blocks_High[ MiSize ]
 if ( reference_select && ( Min( bw4, bh4 ) >= 2 ) )
  comp_mode S()
 else
 comp_mode = SINGLE_REFERENCE
 if ( comp_mode == COMPOUND_REFERENCE ) {
  comp_ref_type S()
 if ( comp_ref_type == UNIDIR_COMP_REFERENCE ) {
  uni_comp_ref S()
 if ( uni_comp_ref ) {
 RefFrame[0] = BWDREF_FRAME
 RefFrame[1] = ALTREF_FRAME
 } else {
  uni_comp_ref_p1 S()
 if ( uni_comp_ref_p1 ) {
  uni_comp_ref_p2 S()
 if ( uni_comp_ref_p2 ) {
 RefFrame[0] = LAST_FRAME
 RefFrame[1] = GOLDEN_FRAME
 } else {
 RefFrame[0] = LAST_FRAME
 RefFrame[1] = LAST3_FRAME
 }
 } else {
 RefFrame[0] = LAST_FRAME
 RefFrame[1] = LAST2_FRAME
 }
 }
 } else {
 comp_ref S()
 if ( comp_ref == 0 ) {
 comp_ref_p1 S()
 RefFrame[ 0 ] = comp_ref_p1 ? LAST2_FRAME : LAST_FRAME
 } else {
 comp_ref_p2 S()
 RefFrame[ 0 ] = comp_ref_p2 ? GOLDEN_FRAME : LAST3_FRAME
 }
 comp_bwdref S()
 if ( comp_bwdref == 0 ) {
 comp_bwdref_p1 S()
 RefFrame[ 1 ] = comp_bwdref_p1 ? ALTREF2_FRAME : BWDREF_FRAME
 } else {
 RefFrame[ 1 ] = ALTREF_FRAME
 }
 }
 } else {
 single_ref_p1 S()
 if ( single_ref_p1 ) {
 single_ref_p2 S()
 if ( single_ref_p2 == 0 ) {
 single_ref_p6 S()
 RefFrame[ 0 ] = single_ref_p6 ? ALTREF2_FRAME : BWDREF_FRAME
 } else {
 RefFrame[ 0 ] = ALTREF_FRAME
 }
 } else {
 single_ref_p3 S()
 if ( single_ref_p3 ) {
 single_ref_p5 S()
 RefFrame[ 0 ] = single_ref_p5 ? GOLDEN_FRAME : LAST3_FRAME
 } else {
 single_ref_p4 S()
 RefFrame[ 0 ] = single_ref_p4 ? LAST2_FRAME : LAST_FRAME
 }
 }
 RefFrame[ 1 ] = NONE
 }
 }
 }
    */
    public class ReadRefFrames : IAomSerializable
    {
		private uint comp_mode;
		public uint CompMode { get { return comp_mode; } set { comp_mode = value; } }
		private uint comp_ref_type;
		public uint CompRefType { get { return comp_ref_type; } set { comp_ref_type = value; } }
		private uint uni_comp_ref;
		public uint UniCompRef { get { return uni_comp_ref; } set { uni_comp_ref = value; } }
		private uint uni_comp_ref_p1;
		public uint UniCompRefP1 { get { return uni_comp_ref_p1; } set { uni_comp_ref_p1 = value; } }
		private uint uni_comp_ref_p2;
		public uint UniCompRefP2 { get { return uni_comp_ref_p2; } set { uni_comp_ref_p2 = value; } }
		private uint comp_ref;
		public uint CompRef { get { return comp_ref; } set { comp_ref = value; } }
		private uint comp_ref_p1;
		public uint CompRefP1 { get { return comp_ref_p1; } set { comp_ref_p1 = value; } }
		private uint comp_ref_p2;
		public uint CompRefP2 { get { return comp_ref_p2; } set { comp_ref_p2 = value; } }
		private uint comp_bwdref;
		public uint CompBwdref { get { return comp_bwdref; } set { comp_bwdref = value; } }
		private uint comp_bwdref_p1;
		public uint CompBwdrefP1 { get { return comp_bwdref_p1; } set { comp_bwdref_p1 = value; } }
		private uint single_ref_p1;
		public uint SingleRefP1 { get { return single_ref_p1; } set { single_ref_p1 = value; } }
		private uint single_ref_p2;
		public uint SingleRefP2 { get { return single_ref_p2; } set { single_ref_p2 = value; } }
		private uint single_ref_p6;
		public uint SingleRefP6 { get { return single_ref_p6; } set { single_ref_p6 = value; } }
		private uint single_ref_p3;
		public uint SingleRefP3 { get { return single_ref_p3; } set { single_ref_p3 = value; } }
		private uint single_ref_p5;
		public uint SingleRefP5 { get { return single_ref_p5; } set { single_ref_p5 = value; } }
		private uint single_ref_p4;
		public uint SingleRefP4 { get { return single_ref_p4; } set { single_ref_p4 = value; } }

         public ReadRefFrames()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint[] RefFrame = null;
			uint bw4 = 0;
			uint bh4 = 0;
			uint comp_mode = 0;

			if ( skip_mode != 0 )
			{
				RefFrame[ 0 ]= SkipModeFrame[ 0 ];
				RefFrame[ 1 ]= SkipModeFrame[ 1 ];
			}
			else if ( seg_feature_active( SEG_LVL_REF_FRAME ) )
			{
				RefFrame[ 0 ]= FeatureData[ segment_id ][ SEG_LVL_REF_FRAME ];
				RefFrame[ 1 ]= NONE;
			}
			else if ( seg_feature_active( SEG_LVL_SKIP ) ||
 seg_feature_active( SEG_LVL_GLOBALMV ) )
			{
				RefFrame[ 0 ]= AV1RefFrames.LAST_FRAME;
				RefFrame[ 1 ]= NONE;
			}
			else 
			{
				bw4= Num_4x4_Blocks_Wide[ MiSize ];
				bh4= Num_4x4_Blocks_High[ MiSize ];

				if ( reference_select != 0 && ( Math.Min( bw4, bh4 ) >= 2 ) )
				{
					size += stream.ReadS(size, out this.comp_mode, "comp_mode"); 
				}
				else 
				{
					comp_mode= SINGLE_REFERENCE;
				}

				if ( comp_mode == COMPOUND_REFERENCE )
				{
					size += stream.ReadS(size, out this.comp_ref_type, "comp_ref_type"); 

					if ( comp_ref_type == UNIDIR_COMP_REFERENCE )
					{
						size += stream.ReadS(size, out this.uni_comp_ref, "uni_comp_ref"); 

						if ( uni_comp_ref != 0 )
						{
							RefFrame[0]= AV1RefFrames.BWDREF_FRAME;
							RefFrame[1]= AV1RefFrames.ALTREF_FRAME;
						}
						else 
						{
							size += stream.ReadS(size, out this.uni_comp_ref_p1, "uni_comp_ref_p1"); 

							if ( uni_comp_ref_p1 != 0 )
							{
								size += stream.ReadS(size, out this.uni_comp_ref_p2, "uni_comp_ref_p2"); 

								if ( uni_comp_ref_p2 != 0 )
								{
									RefFrame[0]= AV1RefFrames.LAST_FRAME;
									RefFrame[1]= AV1RefFrames.GOLDEN_FRAME;
								}
								else 
								{
									RefFrame[0]= AV1RefFrames.LAST_FRAME;
									RefFrame[1]= AV1RefFrames.LAST3_FRAME;
								}
							}
							else 
							{
								RefFrame[0]= AV1RefFrames.LAST_FRAME;
								RefFrame[1]= AV1RefFrames.LAST2_FRAME;
							}
						}
					}
					else 
					{
						size += stream.ReadS(size, out this.comp_ref, "comp_ref"); 

						if ( comp_ref == 0 )
						{
							size += stream.ReadS(size, out this.comp_ref_p1, "comp_ref_p1"); 
							RefFrame[ 0 ]= comp_ref_p1 ? AV1RefFrames.LAST2_FRAME : AV1RefFrames.LAST_FRAME;
						}
						else 
						{
							size += stream.ReadS(size, out this.comp_ref_p2, "comp_ref_p2"); 
							RefFrame[ 0 ]= comp_ref_p2 ? AV1RefFrames.GOLDEN_FRAME : AV1RefFrames.LAST3_FRAME;
						}
						size += stream.ReadS(size, out this.comp_bwdref, "comp_bwdref"); 

						if ( comp_bwdref == 0 )
						{
							size += stream.ReadS(size, out this.comp_bwdref_p1, "comp_bwdref_p1"); 
							RefFrame[ 1 ]= comp_bwdref_p1 ? AV1RefFrames.ALTREF2_FRAME : AV1RefFrames.BWDREF_FRAME;
						}
						else 
						{
							RefFrame[ 1 ]= AV1RefFrames.ALTREF_FRAME;
						}
					}
				}
				else 
				{
					size += stream.ReadS(size, out this.single_ref_p1, "single_ref_p1"); 

					if ( single_ref_p1 != 0 )
					{
						size += stream.ReadS(size, out this.single_ref_p2, "single_ref_p2"); 

						if ( single_ref_p2 == 0 )
						{
							size += stream.ReadS(size, out this.single_ref_p6, "single_ref_p6"); 
							RefFrame[ 0 ]= single_ref_p6 ? AV1RefFrames.ALTREF2_FRAME : AV1RefFrames.BWDREF_FRAME;
						}
						else 
						{
							RefFrame[ 0 ]= AV1RefFrames.ALTREF_FRAME;
						}
					}
					else 
					{
						size += stream.ReadS(size, out this.single_ref_p3, "single_ref_p3"); 

						if ( single_ref_p3 != 0 )
						{
							size += stream.ReadS(size, out this.single_ref_p5, "single_ref_p5"); 
							RefFrame[ 0 ]= single_ref_p5 ? AV1RefFrames.GOLDEN_FRAME : AV1RefFrames.LAST3_FRAME;
						}
						else 
						{
							size += stream.ReadS(size, out this.single_ref_p4, "single_ref_p4"); 
							RefFrame[ 0 ]= single_ref_p4 ? AV1RefFrames.LAST2_FRAME : AV1RefFrames.LAST_FRAME;
						}
					}
					RefFrame[ 1 ]= NONE;
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint[] RefFrame = null;
			uint bw4 = 0;
			uint bh4 = 0;
			uint comp_mode = 0;

			if ( skip_mode != 0 )
			{
				RefFrame[ 0 ]= SkipModeFrame[ 0 ];
				RefFrame[ 1 ]= SkipModeFrame[ 1 ];
			}
			else if ( seg_feature_active( SEG_LVL_REF_FRAME ) )
			{
				RefFrame[ 0 ]= FeatureData[ segment_id ][ SEG_LVL_REF_FRAME ];
				RefFrame[ 1 ]= NONE;
			}
			else if ( seg_feature_active( SEG_LVL_SKIP ) ||
 seg_feature_active( SEG_LVL_GLOBALMV ) )
			{
				RefFrame[ 0 ]= AV1RefFrames.LAST_FRAME;
				RefFrame[ 1 ]= NONE;
			}
			else 
			{
				bw4= Num_4x4_Blocks_Wide[ MiSize ];
				bh4= Num_4x4_Blocks_High[ MiSize ];

				if ( reference_select != 0 && ( Math.Min( bw4, bh4 ) >= 2 ) )
				{
					size += stream.WriteS( this.comp_mode, "comp_mode"); 
				}
				else 
				{
					comp_mode= SINGLE_REFERENCE;
				}

				if ( comp_mode == COMPOUND_REFERENCE )
				{
					size += stream.WriteS( this.comp_ref_type, "comp_ref_type"); 

					if ( comp_ref_type == UNIDIR_COMP_REFERENCE )
					{
						size += stream.WriteS( this.uni_comp_ref, "uni_comp_ref"); 

						if ( uni_comp_ref != 0 )
						{
							RefFrame[0]= AV1RefFrames.BWDREF_FRAME;
							RefFrame[1]= AV1RefFrames.ALTREF_FRAME;
						}
						else 
						{
							size += stream.WriteS( this.uni_comp_ref_p1, "uni_comp_ref_p1"); 

							if ( uni_comp_ref_p1 != 0 )
							{
								size += stream.WriteS( this.uni_comp_ref_p2, "uni_comp_ref_p2"); 

								if ( uni_comp_ref_p2 != 0 )
								{
									RefFrame[0]= AV1RefFrames.LAST_FRAME;
									RefFrame[1]= AV1RefFrames.GOLDEN_FRAME;
								}
								else 
								{
									RefFrame[0]= AV1RefFrames.LAST_FRAME;
									RefFrame[1]= AV1RefFrames.LAST3_FRAME;
								}
							}
							else 
							{
								RefFrame[0]= AV1RefFrames.LAST_FRAME;
								RefFrame[1]= AV1RefFrames.LAST2_FRAME;
							}
						}
					}
					else 
					{
						size += stream.WriteS( this.comp_ref, "comp_ref"); 

						if ( comp_ref == 0 )
						{
							size += stream.WriteS( this.comp_ref_p1, "comp_ref_p1"); 
							RefFrame[ 0 ]= comp_ref_p1 ? AV1RefFrames.LAST2_FRAME : AV1RefFrames.LAST_FRAME;
						}
						else 
						{
							size += stream.WriteS( this.comp_ref_p2, "comp_ref_p2"); 
							RefFrame[ 0 ]= comp_ref_p2 ? AV1RefFrames.GOLDEN_FRAME : AV1RefFrames.LAST3_FRAME;
						}
						size += stream.WriteS( this.comp_bwdref, "comp_bwdref"); 

						if ( comp_bwdref == 0 )
						{
							size += stream.WriteS( this.comp_bwdref_p1, "comp_bwdref_p1"); 
							RefFrame[ 1 ]= comp_bwdref_p1 ? AV1RefFrames.ALTREF2_FRAME : AV1RefFrames.BWDREF_FRAME;
						}
						else 
						{
							RefFrame[ 1 ]= AV1RefFrames.ALTREF_FRAME;
						}
					}
				}
				else 
				{
					size += stream.WriteS( this.single_ref_p1, "single_ref_p1"); 

					if ( single_ref_p1 != 0 )
					{
						size += stream.WriteS( this.single_ref_p2, "single_ref_p2"); 

						if ( single_ref_p2 == 0 )
						{
							size += stream.WriteS( this.single_ref_p6, "single_ref_p6"); 
							RefFrame[ 0 ]= single_ref_p6 ? AV1RefFrames.ALTREF2_FRAME : AV1RefFrames.BWDREF_FRAME;
						}
						else 
						{
							RefFrame[ 0 ]= AV1RefFrames.ALTREF_FRAME;
						}
					}
					else 
					{
						size += stream.WriteS( this.single_ref_p3, "single_ref_p3"); 

						if ( single_ref_p3 != 0 )
						{
							size += stream.WriteS( this.single_ref_p5, "single_ref_p5"); 
							RefFrame[ 0 ]= single_ref_p5 ? AV1RefFrames.GOLDEN_FRAME : AV1RefFrames.LAST3_FRAME;
						}
						else 
						{
							size += stream.WriteS( this.single_ref_p4, "single_ref_p4"); 
							RefFrame[ 0 ]= single_ref_p4 ? AV1RefFrames.LAST2_FRAME : AV1RefFrames.LAST_FRAME;
						}
					}
					RefFrame[ 1 ]= NONE;
				}
			}

            return size;
         }

    }

    /*



  assign_mv( isCompound ) { 
 for ( i = 0; i < 1 + isCompound; i++ ) {
 if ( use_intrabc ) {
 compMode = NEWMV
 } else {
 compMode = get_mode( i )
 }
 if ( use_intrabc ) {
 PredMv[ 0 ] = RefStackMv[ 0 ][ 0 ]
 if ( PredMv[ 0 ][ 0 ] == 0 && PredMv[ 0 ][ 1 ] == 0 ) {
 PredMv[ 0 ] = RefStackMv[ 1 ][ 0 ]
 }
 if ( PredMv[ 0 ][ 0 ] == 0 && PredMv[ 0 ][ 1 ] == 0 ) {
 sbSize = use_128x128_superblock ? BLOCK_128X128 : BLOCK_64X64
 sbSize4 = Num_4x4_Blocks_High[ sbSize ]
 if ( MiRow - sbSize4 < MiRowStart ) {
 PredMv[ 0 ][ 0 ] = 0
 PredMv[ 0 ][ 1 ] = -(sbSize4 * MI_SIZE + INTRABC_DELAY_PIXELS) * 8
 } else {
 PredMv[ 0 ][ 0 ] = -(sbSize4 * MI_SIZE * 8)
 PredMv[ 0 ][ 1 ] = 0
 }
 }
 } else if ( compMode == GLOBALMV ) {
 PredMv[ i ] = GlobalMvs[ i ]
 } else {
 pos = ( compMode == NEARESTMV ) ? 0 : RefMvIdx
 if ( compMode == NEWMV && NumMvFound <= 1 )
 pos = 0
 PredMv[ i ] = RefStackMv[ pos ][ i ]
 }
 if ( compMode == NEWMV ) {
 read_mv( i )
 } else {
 Mv[ i ] = PredMv[ i ]
 }
 }
 }
    */
    public class AssignMv : IAomSerializable
    {
		private uint isCompound;
		public uint IsCompound { get { return isCompound; } set { isCompound = value; } }
		private ReadMv[] read_mv;
		public ReadMv[] ReadMv { get { return read_mv; } set { read_mv = value; } }

         public AssignMv(uint isCompound)
         { 
			this.isCompound = isCompound;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint i = 0;
			uint compMode = 0;
			uint[] PredMv = null;
			uint sbSize = 0;
			uint sbSize4 = 0;
			uint pos = 0;
			uint[] Mv = null;

			this.read_mv = new ReadMv[ 1 + isCompound];
			for ( i = 0; i < 1 + isCompound; i++ )
			{

				if ( use_intrabc != 0 )
				{
					compMode= NEWMV;
				}
				else 
				{
					compMode= get_mode( i );
				}

				if ( use_intrabc != 0 )
				{
					PredMv[ 0 ]= RefStackMv[ 0 ][ 0 ];

					if ( PredMv[ 0 ][ 0 ] == 0 && PredMv[ 0 ][ 1 ] == 0 )
					{
						PredMv[ 0 ]= RefStackMv[ 1 ][ 0 ];
					}

					if ( PredMv[ 0 ][ 0 ] == 0 && PredMv[ 0 ][ 1 ] == 0 )
					{
						sbSize= use_128x128_superblock ? BLOCK_128X128 : BLOCK_64X64;
						sbSize4= Num_4x4_Blocks_High[ sbSize ];

						if ( MiRow - sbSize4 < MiRowStart )
						{
							PredMv[ 0 ][ 0 ]= 0;
							PredMv[ 0 ][ 1 ]= -(sbSize4 * MI_SIZE + INTRABC_DELAY_PIXELS) * 8;
						}
						else 
						{
							PredMv[ 0 ][ 0 ]= -(sbSize4 * MI_SIZE * 8);
							PredMv[ 0 ][ 1 ]= 0;
						}
					}
				}
				else if ( compMode == GLOBALMV )
				{
					PredMv[ i ]= GlobalMvs[ i ];
				}
				else 
				{
					pos= ( compMode == NEARESTMV ) ? 0 : RefMvIdx;

					if ( compMode == NEWMV && NumMvFound <= 1 )
					{
						pos= 0;
					}
					PredMv[ i ]= RefStackMv[ pos ][ i ];
				}

				if ( compMode == NEWMV )
				{
					this.read_mv[ i ] =  new ReadMv( i ) ;
					size +=  stream.ReadClass<ReadMv>(size, context, this.read_mv[ i ], "read_mv"); 
				}
				else 
				{
					Mv[ i ]= PredMv[ i ];
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint i = 0;
			uint compMode = 0;
			uint[] PredMv = null;
			uint sbSize = 0;
			uint sbSize4 = 0;
			uint pos = 0;
			uint[] Mv = null;

			for ( i = 0; i < 1 + isCompound; i++ )
			{

				if ( use_intrabc != 0 )
				{
					compMode= NEWMV;
				}
				else 
				{
					compMode= get_mode( i );
				}

				if ( use_intrabc != 0 )
				{
					PredMv[ 0 ]= RefStackMv[ 0 ][ 0 ];

					if ( PredMv[ 0 ][ 0 ] == 0 && PredMv[ 0 ][ 1 ] == 0 )
					{
						PredMv[ 0 ]= RefStackMv[ 1 ][ 0 ];
					}

					if ( PredMv[ 0 ][ 0 ] == 0 && PredMv[ 0 ][ 1 ] == 0 )
					{
						sbSize= use_128x128_superblock ? BLOCK_128X128 : BLOCK_64X64;
						sbSize4= Num_4x4_Blocks_High[ sbSize ];

						if ( MiRow - sbSize4 < MiRowStart )
						{
							PredMv[ 0 ][ 0 ]= 0;
							PredMv[ 0 ][ 1 ]= -(sbSize4 * MI_SIZE + INTRABC_DELAY_PIXELS) * 8;
						}
						else 
						{
							PredMv[ 0 ][ 0 ]= -(sbSize4 * MI_SIZE * 8);
							PredMv[ 0 ][ 1 ]= 0;
						}
					}
				}
				else if ( compMode == GLOBALMV )
				{
					PredMv[ i ]= GlobalMvs[ i ];
				}
				else 
				{
					pos= ( compMode == NEARESTMV ) ? 0 : RefMvIdx;

					if ( compMode == NEWMV && NumMvFound <= 1 )
					{
						pos= 0;
					}
					PredMv[ i ]= RefStackMv[ pos ][ i ];
				}

				if ( compMode == NEWMV )
				{
					size += stream.WriteClass<ReadMv>(context, this.read_mv[ i ], "read_mv"); 
				}
				else 
				{
					Mv[ i ]= PredMv[ i ];
				}
			}

            return size;
         }

    }

    /*



  read_motion_mode( isCompound ) { 
 if ( skip_mode ) {
 motion_mode = SIMPLE
 return
 }
 if ( !is_motion_mode_switchable ) {
 motion_mode = SIMPLE
 return
 }
 if ( Min( Block_Width[ MiSize ],
 Block_Height[ MiSize ] ) < 8 ) {
 motion_mode = SIMPLE
 return
 }
 if ( !force_integer_mv &&
 ( YMode == GLOBALMV || YMode == GLOBAL_GLOBALMV ) ) {
 if ( GmType[ RefFrame[ 0 ] ] > TRANSLATION ) {
 motion_mode = SIMPLE
 return
 }
 }
 if ( isCompound || RefFrame[ 1 ] == INTRA_FRAME || !has_overlappable_candidates() ) {
 motion_mode = SIMPLE
 return
 }
 find_warp_samples()
 if ( force_integer_mv || NumSamples == 0 ||
 !allow_warped_motion || is_scaled( RefFrame[0] ) ) {
 use_obmc S()
 motion_mode = use_obmc ? OBMC : SIMPLE
 } else {
 motion_mode S()
 }
 }
    */
    public class ReadMotionMode : IAomSerializable
    {
		private uint isCompound;
		public uint IsCompound { get { return isCompound; } set { isCompound = value; } }
		private FindWarpSamples find_warp_samples;
		public FindWarpSamples FindWarpSamples { get { return find_warp_samples; } set { find_warp_samples = value; } }
		private uint use_obmc;
		public uint UseObmc { get { return use_obmc; } set { use_obmc = value; } }
		private uint motion_mode;
		public uint MotionMode { get { return motion_mode; } set { motion_mode = value; } }

         public ReadMotionMode(uint isCompound)
         { 
			this.isCompound = isCompound;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint motion_mode = 0;

			if ( skip_mode != 0 )
			{
				motion_mode= SIMPLE;
return;
			}

			if ( is_motion_mode_switchable== 0 )
			{
				motion_mode= SIMPLE;
return;
			}

			if ( Math.Min( Block_Width[ MiSize ],
 Block_Height[ MiSize ] ) < 8 )
			{
				motion_mode= SIMPLE;
return;
			}

			if ( force_integer_mv== 0 &&
 ( YMode == GLOBALMV || YMode == GLOBAL_GLOBALMV ) )
			{

				if ( GmType[ RefFrame[ 0 ] ] > TRANSLATION )
				{
					motion_mode= SIMPLE;
return;
				}
			}

			if ( isCompound != 0 || RefFrame[ 1 ] == INTRA_FRAME || !has_overlappable_candidates() )
			{
				motion_mode= SIMPLE;
return;
			}
			this.find_warp_samples =  new FindWarpSamples() ;
			size +=  stream.ReadClass<FindWarpSamples>(size, context, this.find_warp_samples, "find_warp_samples"); 

			if ( force_integer_mv != 0 || NumSamples == 0 ||
 allow_warped_motion== 0 || is_scaled( RefFrame[0] ) )
			{
				size += stream.ReadS(size, out this.use_obmc, "use_obmc"); 
				motion_mode= use_obmc ? OBMC : SIMPLE;
			}
			else 
			{
				size += stream.ReadS(size, out this.motion_mode, "motion_mode"); 
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint motion_mode = 0;

			if ( skip_mode != 0 )
			{
				motion_mode= SIMPLE;
return;
			}

			if ( is_motion_mode_switchable== 0 )
			{
				motion_mode= SIMPLE;
return;
			}

			if ( Math.Min( Block_Width[ MiSize ],
 Block_Height[ MiSize ] ) < 8 )
			{
				motion_mode= SIMPLE;
return;
			}

			if ( force_integer_mv== 0 &&
 ( YMode == GLOBALMV || YMode == GLOBAL_GLOBALMV ) )
			{

				if ( GmType[ RefFrame[ 0 ] ] > TRANSLATION )
				{
					motion_mode= SIMPLE;
return;
				}
			}

			if ( isCompound != 0 || RefFrame[ 1 ] == INTRA_FRAME || !has_overlappable_candidates() )
			{
				motion_mode= SIMPLE;
return;
			}
			size += stream.WriteClass<FindWarpSamples>(context, this.find_warp_samples, "find_warp_samples"); 

			if ( force_integer_mv != 0 || NumSamples == 0 ||
 allow_warped_motion== 0 || is_scaled( RefFrame[0] ) )
			{
				size += stream.WriteS( this.use_obmc, "use_obmc"); 
				motion_mode= use_obmc ? OBMC : SIMPLE;
			}
			else 
			{
				size += stream.WriteS( this.motion_mode, "motion_mode"); 
			}

            return size;
         }

    }

    /*


is_scaled(refFrame) {
 refIdx=ref_frame_idx[refFrame-LAST_FRAME]
 xScale=((RefUpscaledWidth[refIdx] <<REF_SCALE_SHIFT)+(FrameWidth/2))//FrameWidth
 yScale=((RefFrameHeight[refIdx] <<REF_SCALE_SHIFT)+(FrameHeight/2))//FrameHeight
 noScale=1 <<REF_SCALE_SHIFT
 return returnxScale !=noScale ||yScale !=noScale
 }
    */
    public class IsScaled : IAomSerializable
    {
		private uint refFrame;
		public uint RefFrame { get { return refFrame; } set { refFrame = value; } }

         public IsScaled(uint refFrame)
         { 
			this.refFrame = refFrame;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint refIdx = 0;
			uint xScale = 0;
			uint yScale = 0;
			uint noScale = 0;
			refIdx= ref_frame_idx[refFrame-AV1RefFrames.LAST_FRAME];
			xScale= ((RefUpscaledWidth[refIdx] <<REF_SCALE_SHIFT)+(FrameWidth/2))//FrameWidth;
			yScale= ((RefFrameHeight[refIdx] <<REF_SCALE_SHIFT)+(FrameHeight/2))//FrameHeight;
			noScale= 1 <<REF_SCALE_SHIFT;
return returnxScale !=noScale ||yScale !=noScale;

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint refIdx = 0;
			uint xScale = 0;
			uint yScale = 0;
			uint noScale = 0;
			refIdx= ref_frame_idx[refFrame-AV1RefFrames.LAST_FRAME];
			xScale= ((RefUpscaledWidth[refIdx] <<REF_SCALE_SHIFT)+(FrameWidth/2))//FrameWidth;
			yScale= ((RefFrameHeight[refIdx] <<REF_SCALE_SHIFT)+(FrameHeight/2))//FrameHeight;
			noScale= 1 <<REF_SCALE_SHIFT;
return returnxScale !=noScale ||yScale !=noScale;

            return size;
         }

    }

    /*



  read_interintra_mode( isCompound ) { 
 if ( !skip_mode && enable_interintra_compound && !isCompound &&
 MiSize >= BLOCK_8X8 && MiSize <= BLOCK_32X32) {
 interintra S()
 if ( interintra ) {
 interintra_mode S()
 RefFrame[1] = INTRA_FRAME
 AngleDeltaY = 0
 AngleDeltaUV = 0
 use_filter_intra = 0
 wedge_interintra S()
 if ( wedge_interintra ) {
 wedge_index S()
 wedge_sign = 0
 }
 }
 } else {
 interintra = 0
 }
 }
    */
    public class ReadInterintraMode : IAomSerializable
    {
		private uint isCompound;
		public uint IsCompound { get { return isCompound; } set { isCompound = value; } }
		private uint interintra;
		public uint Interintra { get { return interintra; } set { interintra = value; } }
		private uint interintra_mode;
		public uint InterintraMode { get { return interintra_mode; } set { interintra_mode = value; } }
		private uint wedge_interintra;
		public uint WedgeInterintra { get { return wedge_interintra; } set { wedge_interintra = value; } }
		private uint wedge_index;
		public uint WedgeIndex { get { return wedge_index; } set { wedge_index = value; } }

         public ReadInterintraMode(uint isCompound)
         { 
			this.isCompound = isCompound;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint[] RefFrame = null;
			uint AngleDeltaY = 0;
			uint AngleDeltaUV = 0;
			uint use_filter_intra = 0;
			uint wedge_sign = 0;
			uint interintra = 0;

			if ( skip_mode== 0 && enable_interintra_compound != 0 && isCompound== 0 &&
 MiSize >= BLOCK_8X8 && MiSize <= BLOCK_32X32)
			{
				size += stream.ReadS(size, out this.interintra, "interintra"); 

				if ( interintra != 0 )
				{
					size += stream.ReadS(size, out this.interintra_mode, "interintra_mode"); 
					RefFrame[1]= AV1RefFrames.INTRA_FRAME;
					AngleDeltaY= 0;
					AngleDeltaUV= 0;
					use_filter_intra= 0;
					size += stream.ReadS(size, out this.wedge_interintra, "wedge_interintra"); 

					if ( wedge_interintra != 0 )
					{
						size += stream.ReadS(size, out this.wedge_index, "wedge_index"); 
						wedge_sign= 0;
					}
				}
			}
			else 
			{
				interintra= 0;
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint[] RefFrame = null;
			uint AngleDeltaY = 0;
			uint AngleDeltaUV = 0;
			uint use_filter_intra = 0;
			uint wedge_sign = 0;
			uint interintra = 0;

			if ( skip_mode== 0 && enable_interintra_compound != 0 && isCompound== 0 &&
 MiSize >= BLOCK_8X8 && MiSize <= BLOCK_32X32)
			{
				size += stream.WriteS( this.interintra, "interintra"); 

				if ( interintra != 0 )
				{
					size += stream.WriteS( this.interintra_mode, "interintra_mode"); 
					RefFrame[1]= AV1RefFrames.INTRA_FRAME;
					AngleDeltaY= 0;
					AngleDeltaUV= 0;
					use_filter_intra= 0;
					size += stream.WriteS( this.wedge_interintra, "wedge_interintra"); 

					if ( wedge_interintra != 0 )
					{
						size += stream.WriteS( this.wedge_index, "wedge_index"); 
						wedge_sign= 0;
					}
				}
			}
			else 
			{
				interintra= 0;
			}

            return size;
         }

    }

    /*



  read_compound_type( isCompound ) { 
 comp_group_idx = 0
 compound_idx = 1
 if ( skip_mode ) {
 compound_type = COMPOUND_AVERAGE
 return
 }
 if ( isCompound ) {
 n = Wedge_Bits[ MiSize ]
 if ( enable_masked_compound ) {
 comp_group_idx S()
 }
 if ( comp_group_idx == 0 ) {
 if ( enable_jnt_comp ) {
 compound_idx S()
 compound_type = compound_idx ? COMPOUND_AVERAGE : COMPOUND_DISTANCE
 } else {
 compound_type = COMPOUND_AVERAGE
 }
 } else {
 if ( n == 0 ) {
 compound_type = COMPOUND_DIFFWTD
 } else {
 compound_type S()
 }
 }
 if ( compound_type == COMPOUND_WEDGE ) {
 wedge_index S()
 wedge_sign L(1)
 } else if ( compound_type == COMPOUND_DIFFWTD ) {
 mask_type L(1)
 }
 } else {
 if ( interintra ) {
 compound_type = wedge_interintra ? COMPOUND_WEDGE : COMPOUND_INTRA
 } else {
 compound_type = COMPOUND_AVERAGE
 }
 }
 }
    */
    public class ReadCompoundType : IAomSerializable
    {
		private uint isCompound;
		public uint IsCompound { get { return isCompound; } set { isCompound = value; } }
		private uint comp_group_idx;
		public uint CompGroupIdx { get { return comp_group_idx; } set { comp_group_idx = value; } }
		private uint compound_idx;
		public uint CompoundIdx { get { return compound_idx; } set { compound_idx = value; } }
		private uint compound_type;
		public uint CompoundType { get { return compound_type; } set { compound_type = value; } }
		private uint wedge_index;
		public uint WedgeIndex { get { return wedge_index; } set { wedge_index = value; } }
		private uint wedge_sign;
		public uint WedgeSign { get { return wedge_sign; } set { wedge_sign = value; } }
		private uint mask_type;
		public uint MaskType { get { return mask_type; } set { mask_type = value; } }

         public ReadCompoundType(uint isCompound)
         { 
			this.isCompound = isCompound;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint comp_group_idx = 0;
			uint compound_idx = 0;
			uint compound_type = 0;
			uint n = 0;
			comp_group_idx= 0;
			compound_idx= 1;

			if ( skip_mode != 0 )
			{
				compound_type= COMPOUND_AVERAGE;
return;
			}

			if ( isCompound != 0 )
			{
				n= Wedge_Bits[ MiSize ];

				if ( enable_masked_compound != 0 )
				{
					size += stream.ReadS(size, out this.comp_group_idx, "comp_group_idx"); 
				}

				if ( comp_group_idx == 0 )
				{

					if ( enable_jnt_comp != 0 )
					{
						size += stream.ReadS(size, out this.compound_idx, "compound_idx"); 
						compound_type= compound_idx ? COMPOUND_AVERAGE : COMPOUND_DISTANCE;
					}
					else 
					{
						compound_type= COMPOUND_AVERAGE;
					}
				}
				else 
				{

					if ( n == 0 )
					{
						compound_type= COMPOUND_DIFFWTD;
					}
					else 
					{
						size += stream.ReadS(size, out this.compound_type, "compound_type"); 
					}
				}

				if ( compound_type == COMPOUND_WEDGE )
				{
					size += stream.ReadS(size, out this.wedge_index, "wedge_index"); 
					size += stream.ReadL(size, 1, out this.wedge_sign, "wedge_sign"); 
				}
				else if ( compound_type == COMPOUND_DIFFWTD )
				{
					size += stream.ReadL(size, 1, out this.mask_type, "mask_type"); 
				}
			}
			else 
			{

				if ( interintra != 0 )
				{
					compound_type= wedge_interintra ? COMPOUND_WEDGE : COMPOUND_INTRA;
				}
				else 
				{
					compound_type= COMPOUND_AVERAGE;
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint comp_group_idx = 0;
			uint compound_idx = 0;
			uint compound_type = 0;
			uint n = 0;
			comp_group_idx= 0;
			compound_idx= 1;

			if ( skip_mode != 0 )
			{
				compound_type= COMPOUND_AVERAGE;
return;
			}

			if ( isCompound != 0 )
			{
				n= Wedge_Bits[ MiSize ];

				if ( enable_masked_compound != 0 )
				{
					size += stream.WriteS( this.comp_group_idx, "comp_group_idx"); 
				}

				if ( comp_group_idx == 0 )
				{

					if ( enable_jnt_comp != 0 )
					{
						size += stream.WriteS( this.compound_idx, "compound_idx"); 
						compound_type= compound_idx ? COMPOUND_AVERAGE : COMPOUND_DISTANCE;
					}
					else 
					{
						compound_type= COMPOUND_AVERAGE;
					}
				}
				else 
				{

					if ( n == 0 )
					{
						compound_type= COMPOUND_DIFFWTD;
					}
					else 
					{
						size += stream.WriteS( this.compound_type, "compound_type"); 
					}
				}

				if ( compound_type == COMPOUND_WEDGE )
				{
					size += stream.WriteS( this.wedge_index, "wedge_index"); 
					size += stream.WriteL(1,  this.wedge_sign, "wedge_sign"); 
				}
				else if ( compound_type == COMPOUND_DIFFWTD )
				{
					size += stream.WriteL(1,  this.mask_type, "mask_type"); 
				}
			}
			else 
			{

				if ( interintra != 0 )
				{
					compound_type= wedge_interintra ? COMPOUND_WEDGE : COMPOUND_INTRA;
				}
				else 
				{
					compound_type= COMPOUND_AVERAGE;
				}
			}

            return size;
         }

    }

    /*



 get_mode( refList ) { 
 if ( refList == 0 ) {
 if ( YMode < NEAREST_NEARESTMV )
 compMode = YMode
 else if ( YMode == NEW_NEWMV || YMode == NEW_NEARESTMV || YMode == NEW_NEARMV )
 compMode = NEWMV
 else if ( YMode == NEAREST_NEARESTMV || YMode == NEAREST_NEWMV )
 compMode = NEARESTMV
 else if ( YMode == NEAR_NEARMV || YMode == NEAR_NEWMV )
 compMode = NEARMV
 else
 compMode = GLOBALMV
 } else {
 if ( YMode == NEW_NEWMV || YMode == NEAREST_NEWMV || YMode == NEAR_NEWMV )
 compMode = NEWMV
 else if ( YMode == NEAREST_NEARESTMV || YMode == NEW_NEARESTMV )
 compMode = NEARESTMV
 else if ( YMode == NEAR_NEARMV || YMode == NEW_NEARMV )
 compMode = NEARMV
 else
 compMode = GLOBALMV
 }
 return compMode
 }
    */
    public class GetMode : IAomSerializable
    {
		private uint refList;
		public uint RefList { get { return refList; } set { refList = value; } }

         public GetMode(uint refList)
         { 
			this.refList = refList;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint compMode = 0;

			if ( refList == 0 )
			{

				if ( YMode < NEAREST_NEARESTMV )
				{
					compMode= YMode;
				}
				else if ( YMode == NEW_NEWMV || YMode == NEW_NEARESTMV || YMode == NEW_NEARMV )
				{
					compMode= NEWMV;
				}
				else if ( YMode == NEAREST_NEARESTMV || YMode == NEAREST_NEWMV )
				{
					compMode= NEARESTMV;
				}
				else if ( YMode == NEAR_NEARMV || YMode == NEAR_NEWMV )
				{
					compMode= NEARMV;
				}
				else 
				{
					compMode= GLOBALMV;
				}
			}
			else 
			{

				if ( YMode == NEW_NEWMV || YMode == NEAREST_NEWMV || YMode == NEAR_NEWMV )
				{
					compMode= NEWMV;
				}
				else if ( YMode == NEAREST_NEARESTMV || YMode == NEW_NEARESTMV )
				{
					compMode= NEARESTMV;
				}
				else if ( YMode == NEAR_NEARMV || YMode == NEW_NEARMV )
				{
					compMode= NEARMV;
				}
				else 
				{
					compMode= GLOBALMV;
				}
			}
return compMode;

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint compMode = 0;

			if ( refList == 0 )
			{

				if ( YMode < NEAREST_NEARESTMV )
				{
					compMode= YMode;
				}
				else if ( YMode == NEW_NEWMV || YMode == NEW_NEARESTMV || YMode == NEW_NEARMV )
				{
					compMode= NEWMV;
				}
				else if ( YMode == NEAREST_NEARESTMV || YMode == NEAREST_NEWMV )
				{
					compMode= NEARESTMV;
				}
				else if ( YMode == NEAR_NEARMV || YMode == NEAR_NEWMV )
				{
					compMode= NEARMV;
				}
				else 
				{
					compMode= GLOBALMV;
				}
			}
			else 
			{

				if ( YMode == NEW_NEWMV || YMode == NEAREST_NEWMV || YMode == NEAR_NEWMV )
				{
					compMode= NEWMV;
				}
				else if ( YMode == NEAREST_NEARESTMV || YMode == NEW_NEARESTMV )
				{
					compMode= NEARESTMV;
				}
				else if ( YMode == NEAR_NEARMV || YMode == NEW_NEARMV )
				{
					compMode= NEARMV;
				}
				else 
				{
					compMode= GLOBALMV;
				}
			}
return compMode;

            return size;
         }

    }

    /*



  read_mv( refc ) { 
 diffMv[ 0 ] = 0
 diffMv[ 1 ] = 0
 if ( use_intrabc ) {
 MvCtx = MV_INTRABC_CONTEXT
 } else {
 MvCtx = 0
 }
 mv_joint S()
 if ( mv_joint == MV_JOINT_HZVNZ || mv_joint == MV_JOINT_HNZVNZ )
 diffMv[ 0 ] = read_mv_component( 0 )
 if ( mv_joint == MV_JOINT_HNZVZ || mv_joint == MV_JOINT_HNZVNZ )
 diffMv[ 1 ] = read_mv_component( 1 )
 Mv[ refc ][ 0 ] = PredMv[ refc ][ 0 ] + diffMv[ 0 ]
 Mv[ refc ][ 1 ] = PredMv[ refc ][ 1 ] + diffMv[ 1 ]
 }
    */
    public class ReadMv : IAomSerializable
    {
		private uint refc;
		public uint Refc { get { return refc; } set { refc = value; } }
		private uint mv_joint;
		public uint MvJoint { get { return mv_joint; } set { mv_joint = value; } }

         public ReadMv(uint refc)
         { 
			this.refc = refc;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint[] diffMv = null;
			uint MvCtx = 0;
			uint[][] Mv = null;
			diffMv[ 0 ]= 0;
			diffMv[ 1 ]= 0;

			if ( use_intrabc != 0 )
			{
				MvCtx= MV_INTRABC_CONTEXT;
			}
			else 
			{
				MvCtx= 0;
			}
			size += stream.ReadS(size, out this.mv_joint, "mv_joint"); 

			if ( mv_joint == MV_JOINT_HZVNZ || mv_joint == MV_JOINT_HNZVNZ )
			{
				diffMv[ 0 ]= read_mv_component( 0 );
			}

			if ( mv_joint == MV_JOINT_HNZVZ || mv_joint == MV_JOINT_HNZVNZ )
			{
				diffMv[ 1 ]= read_mv_component( 1 );
			}
			Mv[ refc ][ 0 ]= PredMv[ refc ][ 0 ] + diffMv[ 0 ];
			Mv[ refc ][ 1 ]= PredMv[ refc ][ 1 ] + diffMv[ 1 ];

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint[] diffMv = null;
			uint MvCtx = 0;
			uint[][] Mv = null;
			diffMv[ 0 ]= 0;
			diffMv[ 1 ]= 0;

			if ( use_intrabc != 0 )
			{
				MvCtx= MV_INTRABC_CONTEXT;
			}
			else 
			{
				MvCtx= 0;
			}
			size += stream.WriteS( this.mv_joint, "mv_joint"); 

			if ( mv_joint == MV_JOINT_HZVNZ || mv_joint == MV_JOINT_HNZVNZ )
			{
				diffMv[ 0 ]= read_mv_component( 0 );
			}

			if ( mv_joint == MV_JOINT_HNZVZ || mv_joint == MV_JOINT_HNZVNZ )
			{
				diffMv[ 1 ]= read_mv_component( 1 );
			}
			Mv[ refc ][ 0 ]= PredMv[ refc ][ 0 ] + diffMv[ 0 ];
			Mv[ refc ][ 1 ]= PredMv[ refc ][ 1 ] + diffMv[ 1 ];

            return size;
         }

    }

    /*



  read_mv_component( comp ) { 
  mv_sign S()
  mv_class S()
 if ( mv_class == MV_CLASS_0 ) {
  mv_class0_bit S()
 if ( force_integer_mv )
 mv_class0_fr = 3
 else
  mv_class0_fr S()
 if ( allow_high_precision_mv )
  mv_class0_hp S()
 else
 mv_class0_hp = 1
 mag = ( ( mv_class0_bit << 3 ) | ( mv_class0_fr << 1 ) | mv_class0_hp ) + 1
 } else {
 d = 0
 for ( i = 0; i < mv_class; i++ ) {
  mv_bit S()
 d = d | mv_bit << i
 }
 mag = CLASS0_SIZE << ( mv_class + 2 )
 if ( force_integer_mv )
 mv_fr = 3
 else
  mv_fr S()
 if ( allow_high_precision_mv )
  mv_hp S()
 else
 mv_hp = 1
 mag += ( ( d << 3 ) | ( mv_fr << 1 ) | mv_hp ) + 1
 }
 return mv_sign ? -mag : mag
 }
    */
    public class ReadMvComponent : IAomSerializable
    {
		private uint comp;
		public uint Comp { get { return comp; } set { comp = value; } }
		private uint mv_sign;
		public uint MvSign { get { return mv_sign; } set { mv_sign = value; } }
		private uint mv_class;
		public uint MvClass { get { return mv_class; } set { mv_class = value; } }
		private uint mv_class0_bit;
		public uint MvClass0Bit { get { return mv_class0_bit; } set { mv_class0_bit = value; } }
		private uint mv_class0_fr;
		public uint MvClass0Fr { get { return mv_class0_fr; } set { mv_class0_fr = value; } }
		private uint mv_class0_hp;
		public uint MvClass0Hp { get { return mv_class0_hp; } set { mv_class0_hp = value; } }
		private uint[] mv_bit;
		public uint[] MvBit { get { return mv_bit; } set { mv_bit = value; } }
		private uint mv_fr;
		public uint MvFr { get { return mv_fr; } set { mv_fr = value; } }
		private uint mv_hp;
		public uint MvHp { get { return mv_hp; } set { mv_hp = value; } }

         public ReadMvComponent(uint comp)
         { 
			this.comp = comp;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint mv_class0_fr = 0;
			uint mv_class0_hp = 0;
			uint mag = 0;
			uint d = 0;
			uint i = 0;
			uint mv_fr = 0;
			uint mv_hp = 0;
			size += stream.ReadS(size, out this.mv_sign, "mv_sign"); 
			size += stream.ReadS(size, out this.mv_class, "mv_class"); 

			if ( mv_class == MV_CLASS_0 )
			{
				size += stream.ReadS(size, out this.mv_class0_bit, "mv_class0_bit"); 

				if ( force_integer_mv != 0 )
				{
					mv_class0_fr= 3;
				}
				else 
				{
					size += stream.ReadS(size, out this.mv_class0_fr, "mv_class0_fr"); 
				}

				if ( allow_high_precision_mv != 0 )
				{
					size += stream.ReadS(size, out this.mv_class0_hp, "mv_class0_hp"); 
				}
				else 
				{
					mv_class0_hp= 1;
				}
				mag= ( ( mv_class0_bit << 3 ) | ( mv_class0_fr << 1 ) | mv_class0_hp ) + 1;
			}
			else 
			{
				d= 0;

				this.mv_bit = new uint[ mv_class];
				for ( i = 0; i < mv_class; i++ )
				{
					size += stream.ReadS(size, out this.mv_bit[ i ], "mv_bit"); 
					d= d | mv_bit[i] << i;
				}
				mag= CLASS0_SIZE << ( mv_class + 2 );

				if ( force_integer_mv != 0 )
				{
					mv_fr= 3;
				}
				else 
				{
					size += stream.ReadS(size, out this.mv_fr, "mv_fr"); 
				}

				if ( allow_high_precision_mv != 0 )
				{
					size += stream.ReadS(size, out this.mv_hp, "mv_hp"); 
				}
				else 
				{
					mv_hp= 1;
				}
				mag+= ( ( d << 3 ) | ( mv_fr << 1 ) | mv_hp ) + 1;
			}
return mv_sign ? -mag : mag;

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint mv_class0_fr = 0;
			uint mv_class0_hp = 0;
			uint mag = 0;
			uint d = 0;
			uint i = 0;
			uint mv_fr = 0;
			uint mv_hp = 0;
			size += stream.WriteS( this.mv_sign, "mv_sign"); 
			size += stream.WriteS( this.mv_class, "mv_class"); 

			if ( mv_class == MV_CLASS_0 )
			{
				size += stream.WriteS( this.mv_class0_bit, "mv_class0_bit"); 

				if ( force_integer_mv != 0 )
				{
					mv_class0_fr= 3;
				}
				else 
				{
					size += stream.WriteS( this.mv_class0_fr, "mv_class0_fr"); 
				}

				if ( allow_high_precision_mv != 0 )
				{
					size += stream.WriteS( this.mv_class0_hp, "mv_class0_hp"); 
				}
				else 
				{
					mv_class0_hp= 1;
				}
				mag= ( ( mv_class0_bit << 3 ) | ( mv_class0_fr << 1 ) | mv_class0_hp ) + 1;
			}
			else 
			{
				d= 0;

				for ( i = 0; i < mv_class; i++ )
				{
					size += stream.WriteS( this.mv_bit[ i ], "mv_bit"); 
					d= d | mv_bit[i] << i;
				}
				mag= CLASS0_SIZE << ( mv_class + 2 );

				if ( force_integer_mv != 0 )
				{
					mv_fr= 3;
				}
				else 
				{
					size += stream.WriteS( this.mv_fr, "mv_fr"); 
				}

				if ( allow_high_precision_mv != 0 )
				{
					size += stream.WriteS( this.mv_hp, "mv_hp"); 
				}
				else 
				{
					mv_hp= 1;
				}
				mag+= ( ( d << 3 ) | ( mv_fr << 1 ) | mv_hp ) + 1;
			}
return mv_sign ? -mag : mag;

            return size;
         }

    }

    /*



  compute_prediction() { 
 sbMask = use_128x128_superblock ? 31 : 15
 subBlockMiRow = MiRow & sbMask
 subBlockMiCol = MiCol & sbMask
 for ( plane = 0; plane < 1 + HasChroma * 2; plane++ ) {
 planeSz = get_plane_residual_size( MiSize, plane )
 num4x4W = Num_4x4_Blocks_Wide[ planeSz ]
 num4x4H = Num_4x4_Blocks_High[ planeSz ]
 log2W = MI_SIZE_LOG2 + Mi_Width_Log2[ planeSz ]
 log2H = MI_SIZE_LOG2 + Mi_Height_Log2[ planeSz ]
 subX = (plane > 0) ? subsampling_x : 0
 subY = (plane > 0) ? subsampling_y : 0
 baseX = (MiCol >> subX) * MI_SIZE
 baseY = (MiRow >> subY) * MI_SIZE
 candRow = (MiRow >> subY) << subY
 candCol = (MiCol >> subX) << subX
 IsInterIntra = ( is_inter && RefFrame[ 1 ] == INTRA_FRAME )
 if ( IsInterIntra ) {
 if ( interintra_mode == II_DC_PRED ) mode = DC_PRED
 else if ( interintra_mode == II_V_PRED ) mode = V_PRED
 else if ( interintra_mode == II_H_PRED ) mode = H_PRED
 else mode = SMOOTH_PRED
 predict_intra( plane, baseX, baseY,
 plane == 0 ? AvailL : AvailLChroma,
 plane == 0 ? AvailU : AvailUChroma,
 BlockDecoded[ plane ]
 [ ( subBlockMiRow >> subY ) - 1 ]
 [ ( subBlockMiCol >> subX ) + num4x4W ],
 BlockDecoded[ plane ]
 [ ( subBlockMiRow >> subY ) + num4x4H ]
 [ ( subBlockMiCol >> subX ) - 1 ],
 mode,
 log2W, log2H )
 }
 if ( is_inter ) {
 predW = Block_Width[ MiSize ] >> subX
 predH = Block_Height[ MiSize ] >> subY
 someUseIntra = 0
 for ( r = 0; r < (num4x4H << subY); r++ )
 for ( c = 0; c < (num4x4W << subX); c++ )
 if ( RefFrames[ candRow + r ][ candCol + c ][ 0 ] == INTRA_FRAME )
 someUseIntra = 1
 if ( someUseIntra ) {
 predW = num4x4W * 4
 predH = num4x4H * 4
 candRow = MiRow
 candCol = MiCol
 }
 r = 0
 for ( y = 0; y < num4x4H * 4; y += predH ) {
 c = 0
 for ( x = 0; x < num4x4W * 4; x += predW ) {
 predict_inter( plane, baseX + x, baseY + y,
 predW, predH,
 candRow + r, candCol + c)
 c++
 }
 r++
 }
 }
 }
 }
    */
    public class ComputePrediction : IAomSerializable
    {
		private PredictIntra[] predict_intra;
		public PredictIntra[] PredictIntra { get { return predict_intra; } set { predict_intra = value; } }
		private PredictInter[][][] predict_inter;
		public PredictInter[][][] PredictInter { get { return predict_inter; } set { predict_inter = value; } }

         public ComputePrediction()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint sbMask = 0;
			uint subBlockMiRow = 0;
			uint subBlockMiCol = 0;
			uint plane = 0;
			uint planeSz = 0;
			uint num4x4W = 0;
			uint num4x4H = 0;
			uint log2W = 0;
			uint log2H = 0;
			uint subX = 0;
			uint subY = 0;
			uint baseX = 0;
			uint baseY = 0;
			uint candRow = 0;
			uint candCol = 0;
			uint IsInterIntra = 0;
			uint mode = 0;
			uint predW = 0;
			uint predH = 0;
			uint someUseIntra = 0;
			uint r = 0;
			uint c = 0;
			uint y = 0;
			uint x = 0;
			sbMask= use_128x128_superblock ? 31 : 15;
			subBlockMiRow= MiRow & sbMask;
			subBlockMiCol= MiCol & sbMask;

			this.predict_intra = new PredictIntra[ 1 + HasChroma * 2];
			this.predict_inter = new PredictInter[ 1 + HasChroma * 2][][];
			for ( plane = 0; plane < 1 + HasChroma * 2; plane++ )
			{
				planeSz= get_plane_residual_size( MiSize, plane );
				num4x4W= Num_4x4_Blocks_Wide[ planeSz ];
				num4x4H= Num_4x4_Blocks_High[ planeSz ];
				log2W= MI_SIZE_LOG2 + Mi_Width_Log2[ planeSz ];
				log2H= MI_SIZE_LOG2 + Mi_Height_Log2[ planeSz ];
				subX= (plane > 0) ? subsampling_x : 0;
				subY= (plane > 0) ? subsampling_y : 0;
				baseX= (MiCol >> subX) * MI_SIZE;
				baseY= (MiRow >> subY) * MI_SIZE;
				candRow= (MiRow >> subY) << subY;
				candCol= (MiCol >> subX) << subX;
				IsInterIntra= ( is_inter && RefFrame[ 1 ] == AV1RefFrames.INTRA_FRAME ) ? (uint)1 : (uint)0;

				if ( IsInterIntra != 0 )
				{

					if ( interintra_mode == II_DC_PRED )
					{
						mode= DC_PRED;
					}
					else if ( interintra_mode == II_V_PRED )
					{
						mode= V_PRED;
					}
					else if ( interintra_mode == II_H_PRED )
					{
						mode= H_PRED;
					}
					else 
					{
						mode= SMOOTH_PRED;
					}
					this.predict_intra[ plane ] =  new PredictIntra( plane,  baseX,  baseY,  plane == 0 ? AvailL : AvailLChroma,  plane == 0 ? AvailU : AvailUChroma,  BlockDecoded[ plane ] [ ( subBlockMiRow >> subY ) - 1 ] [ ( subBlockMiCol >> subX ) + num4x4W ],  BlockDecoded[ plane ] [ ( subBlockMiRow >> subY ) + num4x4H ] [ ( subBlockMiCol >> subX ) - 1 ],  mode,  log2W,  log2H ) ;
					size +=  stream.ReadClass<PredictIntra>(size, context, this.predict_intra[ plane ], "predict_intra"); 
				}

				if ( is_inter != 0 )
				{
					predW= Block_Width[ MiSize ] >> subX;
					predH= Block_Height[ MiSize ] >> subY;
					someUseIntra= 0;

					for ( r[plane] = 0; r[plane] < (num4x4H << (int) subY); r++ )
					{

						for ( c = 0; c < (num4x4W << (int) subX); c++ )
						{

							if ( RefFrames[ candRow + r[c] ][ candCol + c ][ 0 ] == INTRA_FRAME )
							{
								someUseIntra= 1;
							}
						}
					}

					if ( someUseIntra != 0 )
					{
						predW= num4x4W * 4;
						predH= num4x4H * 4;
						candRow= MiRow;
						candCol= MiCol;
					}
					r= 0;

					this.predict_inter[ plane ] = new PredictInter[ num4x4H * 4][];
					for ( y = 0; y < num4x4H * 4; y += predH )
					{
						c= 0;

						this.predict_inter[ plane ][ y ] = new PredictInter[ num4x4W * 4];
						for ( x = 0; x < num4x4W * 4; x += predW )
						{
							this.predict_inter[ plane ][ y ][ x ] =  new PredictInter( plane,  baseX + x,  baseY + y,  predW,  predH,  candRow + r,  candCol + c) ;
							size +=  stream.ReadClass<PredictInter>(size, context, this.predict_inter[ plane ][ y ][ x ], "predict_inter"); 
							c++;
						}
						r++;
					}
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint sbMask = 0;
			uint subBlockMiRow = 0;
			uint subBlockMiCol = 0;
			uint plane = 0;
			uint planeSz = 0;
			uint num4x4W = 0;
			uint num4x4H = 0;
			uint log2W = 0;
			uint log2H = 0;
			uint subX = 0;
			uint subY = 0;
			uint baseX = 0;
			uint baseY = 0;
			uint candRow = 0;
			uint candCol = 0;
			uint IsInterIntra = 0;
			uint mode = 0;
			uint predW = 0;
			uint predH = 0;
			uint someUseIntra = 0;
			uint r = 0;
			uint c = 0;
			uint y = 0;
			uint x = 0;
			sbMask= use_128x128_superblock ? 31 : 15;
			subBlockMiRow= MiRow & sbMask;
			subBlockMiCol= MiCol & sbMask;

			for ( plane = 0; plane < 1 + HasChroma * 2; plane++ )
			{
				planeSz= get_plane_residual_size( MiSize, plane );
				num4x4W= Num_4x4_Blocks_Wide[ planeSz ];
				num4x4H= Num_4x4_Blocks_High[ planeSz ];
				log2W= MI_SIZE_LOG2 + Mi_Width_Log2[ planeSz ];
				log2H= MI_SIZE_LOG2 + Mi_Height_Log2[ planeSz ];
				subX= (plane > 0) ? subsampling_x : 0;
				subY= (plane > 0) ? subsampling_y : 0;
				baseX= (MiCol >> subX) * MI_SIZE;
				baseY= (MiRow >> subY) * MI_SIZE;
				candRow= (MiRow >> subY) << subY;
				candCol= (MiCol >> subX) << subX;
				IsInterIntra= ( is_inter && RefFrame[ 1 ] == AV1RefFrames.INTRA_FRAME ) ? (uint)1 : (uint)0;

				if ( IsInterIntra != 0 )
				{

					if ( interintra_mode == II_DC_PRED )
					{
						mode= DC_PRED;
					}
					else if ( interintra_mode == II_V_PRED )
					{
						mode= V_PRED;
					}
					else if ( interintra_mode == II_H_PRED )
					{
						mode= H_PRED;
					}
					else 
					{
						mode= SMOOTH_PRED;
					}
					size += stream.WriteClass<PredictIntra>(context, this.predict_intra[ plane ], "predict_intra"); 
				}

				if ( is_inter != 0 )
				{
					predW= Block_Width[ MiSize ] >> subX;
					predH= Block_Height[ MiSize ] >> subY;
					someUseIntra= 0;

					for ( r[plane] = 0; r[plane] < (num4x4H << (int) subY); r++ )
					{

						for ( c = 0; c < (num4x4W << (int) subX); c++ )
						{

							if ( RefFrames[ candRow + r[c] ][ candCol + c ][ 0 ] == INTRA_FRAME )
							{
								someUseIntra= 1;
							}
						}
					}

					if ( someUseIntra != 0 )
					{
						predW= num4x4W * 4;
						predH= num4x4H * 4;
						candRow= MiRow;
						candCol= MiCol;
					}
					r= 0;

					for ( y = 0; y < num4x4H * 4; y += predH )
					{
						c= 0;

						for ( x = 0; x < num4x4W * 4; x += predW )
						{
							size += stream.WriteClass<PredictInter>(context, this.predict_inter[ plane ][ y ][ x ], "predict_inter"); 
							c++;
						}
						r++;
					}
				}
			}

            return size;
         }

    }

    /*



residual() { 
 sbMask = use_128x128_superblock ? 31 : 15
 widthChunks = Max( 1, Block_Width[ MiSize ] >> 6 )
 heightChunks = Max( 1, Block_Height[ MiSize ] >> 6 )
 miSizeChunk = ( widthChunks > 1 || heightChunks > 1 ) ? BLOCK_64X64 : MiSize
 for ( chunkY = 0; chunkY < heightChunks; chunkY++ ) {
 for ( chunkX = 0; chunkX < widthChunks; chunkX++ ) {
 miRowChunk = MiRow + ( chunkY << 4 )
 miColChunk = MiCol + ( chunkX << 4 )
 subBlockMiRow = miRowChunk & sbMask
 subBlockMiCol = miColChunk & sbMask
 for ( plane = 0; plane < 1 + HasChroma * 2; plane++ ) {
 txSz = Lossless ? TX_4X4 : get_tx_size( plane, TxSize )
 stepX = Tx_Width[ txSz ] >> 2
 stepY = Tx_Height[ txSz ] >> 2
 planeSz = get_plane_residual_size( miSizeChunk, plane )
 num4x4W = Num_4x4_Blocks_Wide[ planeSz ]
 num4x4H = Num_4x4_Blocks_High[ planeSz ]
 subX = (plane > 0) ? subsampling_x : 0
 subY = (plane > 0) ? subsampling_y : 0
 baseX = (miColChunk >> subX) * MI_SIZE
 baseY = (miRowChunk >> subY) * MI_SIZE
 if ( is_inter && !Lossless && !plane ) {
 transform_tree( baseX, baseY, num4x4W * 4, num4x4H * 4 )
 } else {
 baseXBlock = (MiCol >> subX) * MI_SIZE
 baseYBlock = (MiRow >> subY) * MI_SIZE
 for ( y = 0; y < num4x4H; y += stepY )
 for ( x = 0; x < num4x4W; x += stepX )
 transform_block( plane, baseXBlock, baseYBlock, txSz,
 x + ( ( chunkX << 4 ) >> subX ),
 y + ( ( chunkY << 4 ) >> subY ) )
 }
 }
 }
 }
 }
    */
    public class Residual : IAomSerializable
    {
		private TransformTree[][][] transform_tree;
		public TransformTree[][][] TransformTree { get { return transform_tree; } set { transform_tree = value; } }
		private TransformBlock[][][][][] transform_block;
		public TransformBlock[][][][][] TransformBlock { get { return transform_block; } set { transform_block = value; } }

         public Residual()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint sbMask = 0;
			uint widthChunks = 0;
			uint heightChunks = 0;
			uint miSizeChunk = 0;
			uint chunkY = 0;
			uint chunkX = 0;
			uint miRowChunk = 0;
			uint miColChunk = 0;
			uint subBlockMiRow = 0;
			uint subBlockMiCol = 0;
			uint plane = 0;
			uint txSz = 0;
			uint stepX = 0;
			uint stepY = 0;
			uint planeSz = 0;
			uint num4x4W = 0;
			uint num4x4H = 0;
			uint subX = 0;
			uint subY = 0;
			uint baseX = 0;
			uint baseY = 0;
			uint baseXBlock = 0;
			uint baseYBlock = 0;
			uint y = 0;
			uint x = 0;
			sbMask= use_128x128_superblock ? 31 : 15;
			widthChunks= Math.Max( 1, Block_Width[ MiSize ] >> 6 );
			heightChunks= Math.Max( 1, Block_Height[ MiSize ] >> 6 );
			miSizeChunk= ( widthChunks > 1 || heightChunks > 1 ) ? BLOCK_64X64 : MiSize;

			this.transform_tree = new TransformTree[ heightChunks][][];
			this.transform_block = new TransformBlock[ heightChunks][][][][];
			for ( chunkY = 0; chunkY < heightChunks; chunkY++ )
			{

				this.transform_tree[ chunkY ] = new TransformTree[ widthChunks][];
				this.transform_block[ chunkY ] = new TransformBlock[ widthChunks][][][];
				for ( chunkX = 0; chunkX < widthChunks; chunkX++ )
				{
					miRowChunk= MiRow + ( chunkY << 4 );
					miColChunk= MiCol + ( chunkX << 4 );
					subBlockMiRow= miRowChunk & sbMask;
					subBlockMiCol= miColChunk & sbMask;

					this.transform_tree[ chunkY ][ chunkX ] = new TransformTree[ 1 + HasChroma * 2];
					this.transform_block[ chunkY ][ chunkX ] = new TransformBlock[ 1 + HasChroma * 2][][];
					for ( plane = 0; plane < 1 + HasChroma * 2; plane++ )
					{
						txSz= Lossless ? TX_4X4 : get_tx_size( plane, TxSize );
						stepX= Tx_Width[ txSz ] >> 2;
						stepY= Tx_Height[ txSz ] >> 2;
						planeSz= get_plane_residual_size( miSizeChunk, plane );
						num4x4W= Num_4x4_Blocks_Wide[ planeSz ];
						num4x4H= Num_4x4_Blocks_High[ planeSz ];
						subX= (plane > 0) ? subsampling_x : 0;
						subY= (plane > 0) ? subsampling_y : 0;
						baseX= (miColChunk >> subX) * MI_SIZE;
						baseY= (miRowChunk >> subY) * MI_SIZE;

						if ( is_inter != 0 && Lossless== 0 && plane== 0 )
						{
							this.transform_tree[ chunkY ][ chunkX ][ plane ] =  new TransformTree( baseX,  baseY,  num4x4W * 4,  num4x4H * 4 ) ;
							size +=  stream.ReadClass<TransformTree>(size, context, this.transform_tree[ chunkY ][ chunkX ][ plane ], "transform_tree"); 
						}
						else 
						{
							baseXBlock= (MiCol >> subX) * MI_SIZE;
							baseYBlock= (MiRow >> subY) * MI_SIZE;

							this.transform_block[ chunkY ][ chunkX ][ plane ] = new TransformBlock[ num4x4H][];
							for ( y = 0; y < num4x4H; y += stepY )
							{

								this.transform_block[ chunkY ][ chunkX ][ plane ][ y ] = new TransformBlock[ num4x4W];
								for ( x = 0; x < num4x4W; x += stepX )
								{
									this.transform_block[ chunkY ][ chunkX ][ plane ][ y ][ x ] =  new TransformBlock( plane,  baseXBlock,  baseYBlock,  txSz,  x + ( ( chunkX << 4 ) >> subX ),  y + ( ( chunkY << 4 ) >> subY ) ) ;
									size +=  stream.ReadClass<TransformBlock>(size, context, this.transform_block[ chunkY ][ chunkX ][ plane ][ y ][ x ], "transform_block"); 
								}
							}
						}
					}
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint sbMask = 0;
			uint widthChunks = 0;
			uint heightChunks = 0;
			uint miSizeChunk = 0;
			uint chunkY = 0;
			uint chunkX = 0;
			uint miRowChunk = 0;
			uint miColChunk = 0;
			uint subBlockMiRow = 0;
			uint subBlockMiCol = 0;
			uint plane = 0;
			uint txSz = 0;
			uint stepX = 0;
			uint stepY = 0;
			uint planeSz = 0;
			uint num4x4W = 0;
			uint num4x4H = 0;
			uint subX = 0;
			uint subY = 0;
			uint baseX = 0;
			uint baseY = 0;
			uint baseXBlock = 0;
			uint baseYBlock = 0;
			uint y = 0;
			uint x = 0;
			sbMask= use_128x128_superblock ? 31 : 15;
			widthChunks= Math.Max( 1, Block_Width[ MiSize ] >> 6 );
			heightChunks= Math.Max( 1, Block_Height[ MiSize ] >> 6 );
			miSizeChunk= ( widthChunks > 1 || heightChunks > 1 ) ? BLOCK_64X64 : MiSize;

			for ( chunkY = 0; chunkY < heightChunks; chunkY++ )
			{

				for ( chunkX = 0; chunkX < widthChunks; chunkX++ )
				{
					miRowChunk= MiRow + ( chunkY << 4 );
					miColChunk= MiCol + ( chunkX << 4 );
					subBlockMiRow= miRowChunk & sbMask;
					subBlockMiCol= miColChunk & sbMask;

					for ( plane = 0; plane < 1 + HasChroma * 2; plane++ )
					{
						txSz= Lossless ? TX_4X4 : get_tx_size( plane, TxSize );
						stepX= Tx_Width[ txSz ] >> 2;
						stepY= Tx_Height[ txSz ] >> 2;
						planeSz= get_plane_residual_size( miSizeChunk, plane );
						num4x4W= Num_4x4_Blocks_Wide[ planeSz ];
						num4x4H= Num_4x4_Blocks_High[ planeSz ];
						subX= (plane > 0) ? subsampling_x : 0;
						subY= (plane > 0) ? subsampling_y : 0;
						baseX= (miColChunk >> subX) * MI_SIZE;
						baseY= (miRowChunk >> subY) * MI_SIZE;

						if ( is_inter != 0 && Lossless== 0 && plane== 0 )
						{
							size += stream.WriteClass<TransformTree>(context, this.transform_tree[ chunkY ][ chunkX ][ plane ], "transform_tree"); 
						}
						else 
						{
							baseXBlock= (MiCol >> subX) * MI_SIZE;
							baseYBlock= (MiRow >> subY) * MI_SIZE;

							for ( y = 0; y < num4x4H; y += stepY )
							{

								for ( x = 0; x < num4x4W; x += stepX )
								{
									size += stream.WriteClass<TransformBlock>(context, this.transform_block[ chunkY ][ chunkX ][ plane ][ y ][ x ], "transform_block"); 
								}
							}
						}
					}
				}
			}

            return size;
         }

    }

    /*



transform_block(plane, baseX, baseY, txSz, x, y) { 
 startX = baseX + 4 * x
 startY = baseY + 4 * y
 subX = (plane > 0) ? subsampling_x : 0
 subY = (plane > 0) ? subsampling_y : 0
 row = ( startY << subY ) >> MI_SIZE_LOG2
 col = ( startX << subX ) >> MI_SIZE_LOG2
 sbMask = use_128x128_superblock ? 31 : 15
 subBlockMiRow = row & sbMask
 subBlockMiCol = col & sbMask
 stepX = Tx_Width[ txSz ] >> MI_SIZE_LOG2
 stepY = Tx_Height[ txSz ] >> MI_SIZE_LOG2
 maxX = (MiCols * MI_SIZE) >> subX
 maxY = (MiRows * MI_SIZE) >> subY
 if ( startX >= maxX || startY >= maxY ) {
 return
 }
 if ( !is_inter ) {
 if ( ( ( plane == 0 ) && PaletteSizeY ) ||
 ( ( plane != 0 ) && PaletteSizeUV ) ) {
 predict_palette( plane, startX, startY, x, y, txSz )
 } else {
 isCfl = (plane > 0 && UVMode == UV_CFL_PRED)
 if ( plane == 0 ) {
 mode = YMode
 } else {
 mode = ( isCfl ) ? DC_PRED : UVMode
 }
 log2W = Tx_Width_Log2[ txSz ]
 log2H = Tx_Height_Log2[ txSz ]
 predict_intra( plane, startX, startY,
 ( plane == 0 ? AvailL : AvailLChroma ) || x > 0,
 ( plane == 0 ? AvailU : AvailUChroma ) || y > 0,
 BlockDecoded[ plane ]
 [ ( subBlockMiRow >> subY ) - 1 ]
 [ ( subBlockMiCol >> subX ) + stepX ],
 BlockDecoded[ plane ]
 [ ( subBlockMiRow >> subY ) + stepY ]
 [ ( subBlockMiCol >> subX ) - 1 ],
 mode,
 log2W, log2H )
 if ( isCfl ) {
 predict_chroma_from_luma( plane, startX, startY, txSz )
 }
 }
 if ( plane == 0 ) {
 MaxLumaW = startX + stepX * 4
 MaxLumaH = startY + stepY * 4
 }
 }
 if ( !skip ) {
 eob = coeffs( plane, startX, startY, txSz )
 if ( eob > 0 )
 reconstruct( plane, startX, startY, txSz )
 }
 for ( i = 0; i < stepY; i++ ) {
 for ( j = 0; j < stepX; j++ ) {
 LoopfilterTxSizes[ plane ]
 [ (row >> subY) + i ]
 [ (col >> subX) + j ] = txSz
 BlockDecoded[ plane ]
 [ ( subBlockMiRow >> subY ) + i ]
 [ ( subBlockMiCol >> subX ) + j ] = 1
 }
 }
 }
    */
    public class TransformBlock : IAomSerializable
    {
		private uint plane;
		public uint Plane { get { return plane; } set { plane = value; } }
		private uint baseX;
		public uint BaseX { get { return baseX; } set { baseX = value; } }
		private uint baseY;
		public uint BaseY { get { return baseY; } set { baseY = value; } }
		private uint txSz;
		public uint TxSz { get { return txSz; } set { txSz = value; } }
		private uint x;
		public uint x { get { return x; } set { x = value; } }
		private uint y;
		public uint y { get { return y; } set { y = value; } }
		private PredictPalette predict_palette;
		public PredictPalette PredictPalette { get { return predict_palette; } set { predict_palette = value; } }
		private PredictIntra predict_intra;
		public PredictIntra PredictIntra { get { return predict_intra; } set { predict_intra = value; } }
		private PredictChromaFromLuma predict_chroma_from_luma;
		public PredictChromaFromLuma PredictChromaFromLuma { get { return predict_chroma_from_luma; } set { predict_chroma_from_luma = value; } }
		private Reconstruct reconstruct;
		public Reconstruct Reconstruct { get { return reconstruct; } set { reconstruct = value; } }

         public TransformBlock(uint plane, uint baseX, uint baseY, uint txSz, uint x, uint y)
         { 
			this.plane = plane;
			this.baseX = baseX;
			this.baseY = baseY;
			this.txSz = txSz;
			this.x = x;
			this.y = y;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint startX = 0;
			uint startY = 0;
			uint subX = 0;
			uint subY = 0;
			uint row = 0;
			uint col = 0;
			uint sbMask = 0;
			uint subBlockMiRow = 0;
			uint subBlockMiCol = 0;
			uint stepX = 0;
			uint stepY = 0;
			uint maxX = 0;
			uint maxY = 0;
			uint isCfl = 0;
			uint mode = 0;
			uint log2W = 0;
			uint log2H = 0;
			uint MaxLumaW = 0;
			uint MaxLumaH = 0;
			uint eob = 0;
			uint i = 0;
			uint j = 0;
			uint[][][] LoopfilterTxSizes = null;
			uint[][][] BlockDecoded = null;
			startX= baseX + 4 * x;
			startY= baseY + 4 * y;
			subX= (plane > 0) ? subsampling_x : 0;
			subY= (plane > 0) ? subsampling_y : 0;
			row= ( startY << subY ) >> MI_SIZE_LOG2;
			col= ( startX << subX ) >> MI_SIZE_LOG2;
			sbMask= use_128x128_superblock ? 31 : 15;
			subBlockMiRow= row & sbMask;
			subBlockMiCol= col & sbMask;
			stepX= Tx_Width[ txSz ] >> MI_SIZE_LOG2;
			stepY= Tx_Height[ txSz ] >> MI_SIZE_LOG2;
			maxX= (MiCols * MI_SIZE) >> subX;
			maxY= (MiRows * MI_SIZE) >> subY;

			if ( startX >= maxX || startY >= maxY )
			{
return;
			}

			if ( is_inter== 0 )
			{

				if ( ( ( plane == 0 ) && PaletteSizeY != 0 ) ||
 ( ( plane != 0 ) && PaletteSizeUV != 0 ) )
				{
					this.predict_palette =  new PredictPalette( plane,  startX,  startY,  x,  y,  txSz ) ;
					size +=  stream.ReadClass<PredictPalette>(size, context, this.predict_palette, "predict_palette"); 
				}
				else 
				{
					isCfl= (plane > 0 && UVMode == UV_CFL_PRED) ? (uint)1 : (uint)0;

					if ( plane == 0 )
					{
						mode= YMode;
					}
					else 
					{
						mode= ( isCfl ) ? DC_PRED : UVMode;
					}
					log2W= Tx_Width_Log2[ txSz ];
					log2H= Tx_Height_Log2[ txSz ];
					this.predict_intra =  new PredictIntra( plane,  startX,  startY,  ( plane == 0 ? AvailL : AvailLChroma ) || x > 0,  ( plane == 0 ? AvailU : AvailUChroma ) || y > 0,  BlockDecoded[ plane ] [ ( subBlockMiRow >> subY ) - 1 ] [ ( subBlockMiCol >> subX ) + stepX ],  BlockDecoded[ plane ] [ ( subBlockMiRow >> subY ) + stepY ] [ ( subBlockMiCol >> subX ) - 1 ],  mode,  log2W,  log2H ) ;
					size +=  stream.ReadClass<PredictIntra>(size, context, this.predict_intra, "predict_intra"); 

					if ( isCfl != 0 )
					{
						this.predict_chroma_from_luma =  new PredictChromaFromLuma( plane,  startX,  startY,  txSz ) ;
						size +=  stream.ReadClass<PredictChromaFromLuma>(size, context, this.predict_chroma_from_luma, "predict_chroma_from_luma"); 
					}
				}

				if ( plane == 0 )
				{
					MaxLumaW= startX + stepX * 4;
					MaxLumaH= startY + stepY * 4;
				}
			}

			if ( skip== 0 )
			{
				eob= coeffs( plane, startX, startY, txSz );

				if ( eob > 0 )
				{
					this.reconstruct =  new Reconstruct( plane,  startX,  startY,  txSz ) ;
					size +=  stream.ReadClass<Reconstruct>(size, context, this.reconstruct, "reconstruct"); 
				}
			}

			for ( i = 0; i < stepY; i++ )
			{

				for ( j = 0; j < stepX; j++ )
				{
					LoopfilterTxSizes[ plane ][ (row >> subY) + i ][ (col >> subX) + j ]= txSz;
					BlockDecoded[ plane ][ ( subBlockMiRow >> subY ) + i ][ ( subBlockMiCol >> subX ) + j ]= 1;
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint startX = 0;
			uint startY = 0;
			uint subX = 0;
			uint subY = 0;
			uint row = 0;
			uint col = 0;
			uint sbMask = 0;
			uint subBlockMiRow = 0;
			uint subBlockMiCol = 0;
			uint stepX = 0;
			uint stepY = 0;
			uint maxX = 0;
			uint maxY = 0;
			uint isCfl = 0;
			uint mode = 0;
			uint log2W = 0;
			uint log2H = 0;
			uint MaxLumaW = 0;
			uint MaxLumaH = 0;
			uint eob = 0;
			uint i = 0;
			uint j = 0;
			uint[][][] LoopfilterTxSizes = null;
			uint[][][] BlockDecoded = null;
			startX= baseX + 4 * x;
			startY= baseY + 4 * y;
			subX= (plane > 0) ? subsampling_x : 0;
			subY= (plane > 0) ? subsampling_y : 0;
			row= ( startY << subY ) >> MI_SIZE_LOG2;
			col= ( startX << subX ) >> MI_SIZE_LOG2;
			sbMask= use_128x128_superblock ? 31 : 15;
			subBlockMiRow= row & sbMask;
			subBlockMiCol= col & sbMask;
			stepX= Tx_Width[ txSz ] >> MI_SIZE_LOG2;
			stepY= Tx_Height[ txSz ] >> MI_SIZE_LOG2;
			maxX= (MiCols * MI_SIZE) >> subX;
			maxY= (MiRows * MI_SIZE) >> subY;

			if ( startX >= maxX || startY >= maxY )
			{
return;
			}

			if ( is_inter== 0 )
			{

				if ( ( ( plane == 0 ) && PaletteSizeY != 0 ) ||
 ( ( plane != 0 ) && PaletteSizeUV != 0 ) )
				{
					size += stream.WriteClass<PredictPalette>(context, this.predict_palette, "predict_palette"); 
				}
				else 
				{
					isCfl= (plane > 0 && UVMode == UV_CFL_PRED) ? (uint)1 : (uint)0;

					if ( plane == 0 )
					{
						mode= YMode;
					}
					else 
					{
						mode= ( isCfl ) ? DC_PRED : UVMode;
					}
					log2W= Tx_Width_Log2[ txSz ];
					log2H= Tx_Height_Log2[ txSz ];
					size += stream.WriteClass<PredictIntra>(context, this.predict_intra, "predict_intra"); 

					if ( isCfl != 0 )
					{
						size += stream.WriteClass<PredictChromaFromLuma>(context, this.predict_chroma_from_luma, "predict_chroma_from_luma"); 
					}
				}

				if ( plane == 0 )
				{
					MaxLumaW= startX + stepX * 4;
					MaxLumaH= startY + stepY * 4;
				}
			}

			if ( skip== 0 )
			{
				eob= coeffs( plane, startX, startY, txSz );

				if ( eob > 0 )
				{
					size += stream.WriteClass<Reconstruct>(context, this.reconstruct, "reconstruct"); 
				}
			}

			for ( i = 0; i < stepY; i++ )
			{

				for ( j = 0; j < stepX; j++ )
				{
					LoopfilterTxSizes[ plane ][ (row >> subY) + i ][ (col >> subX) + j ]= txSz;
					BlockDecoded[ plane ][ ( subBlockMiRow >> subY ) + i ][ ( subBlockMiCol >> subX ) + j ]= 1;
				}
			}

            return size;
         }

    }

    /*



transform_tree( startX, startY, w, h ) { 
 maxX = MiCols * MI_SIZE
 maxY = MiRows * MI_SIZE
 if ( startX >= maxX || startY >= maxY ) {
 return
 }
 row = startY >> MI_SIZE_LOG2
 col = startX >> MI_SIZE_LOG2
 lumaTxSz = InterTxSizes[ row ][ col ]
 lumaW = Tx_Width[ lumaTxSz ]
 lumaH = Tx_Height[ lumaTxSz ]
 if ( w <= lumaW && h <= lumaH ) {
 txSz = find_tx_size( w, h )
 transform_block( 0, startX, startY, txSz, 0, 0 )
 } else {
 if ( w > h ) {
 transform_tree( startX, startY, w/2, h )
 transform_tree( startX + w / 2, startY, w/2, h )
 } else if ( w < h ) {
 transform_tree( startX, startY, w, h/2 )
 transform_tree( startX, startY + h/2, w, h/2 )
 } else {
 transform_tree( startX, startY, w/2, h/2 )
 transform_tree( startX + w/2, startY, w/2, h/2 )
 transform_tree( startX, startY + h/2, w/2, h/2 )
 transform_tree( startX + w/2, startY + h/2, w/2, h/2 )
 }
 }
 }
    */
    public class TransformTree : IAomSerializable
    {
		private uint startX;
		public uint StartX { get { return startX; } set { startX = value; } }
		private uint startY;
		public uint StartY { get { return startY; } set { startY = value; } }
		private uint w;
		public uint w { get { return w; } set { w = value; } }
		private uint h;
		public uint h { get { return h; } set { h = value; } }
		private TransformBlock transform_block;
		public TransformBlock TransformBlock { get { return transform_block; } set { transform_block = value; } }
		private TransformTree transform_tree;
		public TransformTree _TransformTree { get { return transform_tree; } set { transform_tree = value; } }
		private TransformTree transform_tree0;
		public TransformTree TransformTree0 { get { return transform_tree0; } set { transform_tree0 = value; } }
		private TransformTree transform_tree1;
		public TransformTree TransformTree1 { get { return transform_tree1; } set { transform_tree1 = value; } }
		private TransformTree0 transform_tree00;
		public TransformTree0 TransformTree00 { get { return transform_tree00; } set { transform_tree00 = value; } }
		private TransformTree transform_tree2;
		public TransformTree TransformTree2 { get { return transform_tree2; } set { transform_tree2 = value; } }
		private TransformTree0 transform_tree01;
		public TransformTree0 TransformTree01 { get { return transform_tree01; } set { transform_tree01 = value; } }
		private TransformTree1 transform_tree10;
		public TransformTree1 TransformTree10 { get { return transform_tree10; } set { transform_tree10 = value; } }
		private TransformTree2 transform_tree20;
		public TransformTree2 TransformTree20 { get { return transform_tree20; } set { transform_tree20 = value; } }

         public TransformTree(uint startX, uint startY, uint w, uint h)
         { 
			this.startX = startX;
			this.startY = startY;
			this.w = w;
			this.h = h;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint maxX = 0;
			uint maxY = 0;
			uint row = 0;
			uint col = 0;
			uint lumaTxSz = 0;
			uint lumaW = 0;
			uint lumaH = 0;
			uint txSz = 0;
			maxX= MiCols * MI_SIZE;
			maxY= MiRows * MI_SIZE;

			if ( startX >= maxX || startY >= maxY )
			{
return;
			}
			row= startY >> MI_SIZE_LOG2;
			col= startX >> MI_SIZE_LOG2;
			lumaTxSz= InterTxSizes[ row ][ col ];
			lumaW= Tx_Width[ lumaTxSz ];
			lumaH= Tx_Height[ lumaTxSz ];

			if ( w <= lumaW && h <= lumaH )
			{
				txSz= find_tx_size( w, h );
				this.transform_block =  new TransformBlock( 0,  startX,  startY,  txSz,  0,  0 ) ;
				size +=  stream.ReadClass<TransformBlock>(size, context, this.transform_block, "transform_block"); 
			}
			else 
			{

				if ( w > h )
				{
					this.transform_tree =  new TransformTree( startX,  startY,  w/2,  h ) ;
					size +=  stream.ReadClass<TransformTree>(size, context, this.transform_tree, "transform_tree"); 
					this.transform_tree0 =  new TransformTree( startX + w / 2,  startY,  w/2,  h ) ;
					size +=  stream.ReadClass<TransformTree>(size, context, this.transform_tree0, "transform_tree0"); 
				}
				else if ( w < h )
				{
					this.transform_tree1 =  new TransformTree( startX,  startY,  w,  h/2 ) ;
					size +=  stream.ReadClass<TransformTree>(size, context, this.transform_tree1, "transform_tree1"); 
					this.transform_tree00 =  new TransformTree0( startX,  startY + h/2,  w,  h/2 ) ;
					size +=  stream.ReadClass<TransformTree0>(size, context, this.transform_tree00, "transform_tree00"); 
				}
				else 
				{
					this.transform_tree2 =  new TransformTree( startX,  startY,  w/2,  h/2 ) ;
					size +=  stream.ReadClass<TransformTree>(size, context, this.transform_tree2, "transform_tree2"); 
					this.transform_tree01 =  new TransformTree0( startX + w/2,  startY,  w/2,  h/2 ) ;
					size +=  stream.ReadClass<TransformTree0>(size, context, this.transform_tree01, "transform_tree01"); 
					this.transform_tree10 =  new TransformTree1( startX,  startY + h/2,  w/2,  h/2 ) ;
					size +=  stream.ReadClass<TransformTree1>(size, context, this.transform_tree10, "transform_tree10"); 
					this.transform_tree20 =  new TransformTree2( startX + w/2,  startY + h/2,  w/2,  h/2 ) ;
					size +=  stream.ReadClass<TransformTree2>(size, context, this.transform_tree20, "transform_tree20"); 
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint maxX = 0;
			uint maxY = 0;
			uint row = 0;
			uint col = 0;
			uint lumaTxSz = 0;
			uint lumaW = 0;
			uint lumaH = 0;
			uint txSz = 0;
			maxX= MiCols * MI_SIZE;
			maxY= MiRows * MI_SIZE;

			if ( startX >= maxX || startY >= maxY )
			{
return;
			}
			row= startY >> MI_SIZE_LOG2;
			col= startX >> MI_SIZE_LOG2;
			lumaTxSz= InterTxSizes[ row ][ col ];
			lumaW= Tx_Width[ lumaTxSz ];
			lumaH= Tx_Height[ lumaTxSz ];

			if ( w <= lumaW && h <= lumaH )
			{
				txSz= find_tx_size( w, h );
				size += stream.WriteClass<TransformBlock>(context, this.transform_block, "transform_block"); 
			}
			else 
			{

				if ( w > h )
				{
					size += stream.WriteClass<TransformTree>(context, this.transform_tree, "transform_tree"); 
					size += stream.WriteClass<TransformTree>(context, this.transform_tree0, "transform_tree0"); 
				}
				else if ( w < h )
				{
					size += stream.WriteClass<TransformTree>(context, this.transform_tree1, "transform_tree1"); 
					size += stream.WriteClass<TransformTree0>(context, this.transform_tree00, "transform_tree00"); 
				}
				else 
				{
					size += stream.WriteClass<TransformTree>(context, this.transform_tree2, "transform_tree2"); 
					size += stream.WriteClass<TransformTree0>(context, this.transform_tree01, "transform_tree01"); 
					size += stream.WriteClass<TransformTree1>(context, this.transform_tree10, "transform_tree10"); 
					size += stream.WriteClass<TransformTree2>(context, this.transform_tree20, "transform_tree20"); 
				}
			}

            return size;
         }

    }

    /*




 find_tx_size(w,h){
 for(txSz=0;txSz<TX_SIZES_ALL;txSz++)
 if (Tx_Width[txSz]==w && Tx_Height[txSz] ==h)
 break
 return txSz
 }
    */
    public class FindTxSize : IAomSerializable
    {
		private uint w;
		public uint w { get { return w; } set { w = value; } }
		private uint h;
		public uint h { get { return h; } set { h = value; } }

         public FindTxSize(uint w, uint h)
         { 
			this.w = w;
			this.h = h;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint txSz = 0;

			for (txSz=0;txSz<TX_SIZES_ALL;txSz++)
			{

				if (Tx_Width[txSz]==w && Tx_Height[txSz] ==h)
				{
break;
				}
			}
return txSz;

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint txSz = 0;

			for (txSz=0;txSz<TX_SIZES_ALL;txSz++)
			{

				if (Tx_Width[txSz]==w && Tx_Height[txSz] ==h)
				{
break;
				}
			}
return txSz;

            return size;
         }

    }

    /*



get_tx_size( plane, txSz ) { 
 if ( plane == 0 )
 return txSz
  uvTx = Max_Tx_Size_Rect[ get_plane_residual_size( MiSize, plane ) ]
 if ( Tx_Width[ uvTx ] == 64 || Tx_Height[ uvTx ] == 64 ){
 if ( Tx_Width[ uvTx ] == 16 ) {
 return TX_16X32
 }
 if ( Tx_Height[ uvTx ] == 16 ) {
 return TX_32X16
 }
 return TX_32X32
 }
 return uvTx
 }
    */
    public class GetTxSize : IAomSerializable
    {
		private uint plane;
		public uint Plane { get { return plane; } set { plane = value; } }
		private uint txSz;
		public uint TxSz { get { return txSz; } set { txSz = value; } }

         public GetTxSize(uint plane, uint txSz)
         { 
			this.plane = plane;
			this.txSz = txSz;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint uvTx = 0;

			if ( plane == 0 )
			{
return txSz;
			}
			uvTx= Max_Tx_Size_Rect[ get_plane_residual_size( MiSize, plane ) ];

			if ( Tx_Width[ uvTx ] == 64 || Tx_Height[ uvTx ] == 64 )
			{

				if ( Tx_Width[ uvTx ] == 16 )
				{
return TX_16X32;
				}

				if ( Tx_Height[ uvTx ] == 16 )
				{
return TX_32X16;
				}
return TX_32X32;
			}
return uvTx;

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint uvTx = 0;

			if ( plane == 0 )
			{
return txSz;
			}
			uvTx= Max_Tx_Size_Rect[ get_plane_residual_size( MiSize, plane ) ];

			if ( Tx_Width[ uvTx ] == 64 || Tx_Height[ uvTx ] == 64 )
			{

				if ( Tx_Width[ uvTx ] == 16 )
				{
return TX_16X32;
				}

				if ( Tx_Height[ uvTx ] == 16 )
				{
return TX_32X16;
				}
return TX_32X32;
			}
return uvTx;

            return size;
         }

    }

    /*


get_plane_residual_size( subsize, plane ) {
 Type
 subx = plane > 0 ? subsampling_x : 0
 suby = plane > 0 ? subsampling_y : 0
 return Subsampled_Size[ subsize ][ subx ][ suby ]
 }
    */
    public class GetPlaneResidualSize : IAomSerializable
    {
		private uint subsize;
		public uint Subsize { get { return subsize; } set { subsize = value; } }
		private uint plane;
		public uint Plane { get { return plane; } set { plane = value; } }
		private Type type;
		public Type Type { get { return Type; } set { Type = value; } }

         public GetPlaneResidualSize(uint subsize, uint plane)
         { 
			this.subsize = subsize;
			this.plane = plane;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint subx = 0;
			uint suby = 0;
			this.Type =  new Type() ;
			size +=  stream.ReadClass<Type>(size, context, this.Type, "Type"); 
			subx= plane > 0 ? subsampling_x : 0;
			suby= plane > 0 ? subsampling_y : 0;
return Subsampled_Size[ subsize ][ subx ][ suby ];

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint subx = 0;
			uint suby = 0;
			size += stream.WriteClass<Type>(context, this.Type, "Type"); 
			subx= plane > 0 ? subsampling_x : 0;
			suby= plane > 0 ? subsampling_y : 0;
return Subsampled_Size[ subsize ][ subx ][ suby ];

            return size;
         }

    }

    /*



  coeffs( plane, startX, startY, txSz ) { 
 x4 = startX >> 2
 y4 = startY >> 2
 w4 = Tx_Width[ txSz ] >> 2
 h4 = Tx_Height[ txSz ] >> 2
 txSzCtx = ( Tx_Size_Sqr[txSz] + Tx_Size_Sqr_Up[txSz] + 1 ) >> 1
 ptype = plane > 0
 segEob = ( txSz == TX_16X64 || txSz == TX_64X16 ) ? 512 :
 Min( 1024, Tx_Width[ txSz ] * Tx_Height[ txSz ] )
 for ( c = 0; c < segEob; c++ )
 Quant[c] = 0
 for ( i = 0; i < 64; i++ )
 for ( j = 0; j < 64; j++ )
 Dequant[ i ][ j ] = 0
 eob = 0
 culLevel = 0
 dcCategory = 0
  all_zero S()
  if ( all_zero ) {
 c = 0
 if ( plane == 0 ) {
 for ( i = 0; i < w4; i++ ) {
 for ( j = 0; j < h4; j++ ) {
 TxTypes[ y4 + j ][ x4 + i ] = DCT_DCT
 }
 }
 }
 } else {
 if ( plane == 0 )
 transform_type( x4, y4, txSz )
 PlaneTxType = compute_tx_type( plane, txSz, x4, y4 )
 scan = get_scan( txSz )
 eobMultisize = Min( Tx_Width_Log2[ txSz ], 5) + Min( Tx_Height_Log2[ txSz ], 5) - 4
 if ( eobMultisize == 0 ) {
  eob_pt_16 S()
 eobPt = eob_pt_16 + 1
 } else if ( eobMultisize == 1 ) {
  eob_pt_32 S()
 eobPt = eob_pt_32 + 1
 } else if ( eobMultisize == 2 ) {
  eob_pt_64 S()
 eobPt = eob_pt_64 + 1
 } else if ( eobMultisize == 3 ) {
  eob_pt_128 S()
 eobPt = eob_pt_128 + 1
 } else if ( eobMultisize == 4 ) {
  eob_pt_256 S()
 eobPt = eob_pt_256 + 1
 } else if ( eobMultisize == 5 ) {
  eob_pt_512 S()
 eobPt = eob_pt_512 + 1
 } else {
  eob_pt_1024 S()
 eobPt = eob_pt_1024 + 1
 }
 eob = ( eobPt < 2 ) ? eobPt : ( ( 1 << ( eobPt - 2 ) ) + 1 )
 eobShift = Max( -1, eobPt - 3 )
 if ( eobShift >= 0 ) {
  eob_extra S()
 if ( eob_extra ) {
 eob += ( 1 << eobShift )
 }
 for ( i = 1; i < Max( 0, eobPt - 2 ); i++ ) {
 eobShift = Max( 0, eobPt - 2 ) - 1 - i
 eob_extra_bit  L(1)
 if ( eob_extra_bit ) {
 eob += ( 1 << eobShift )
 }
 }
 }
 for ( c = eob - 1; c >= 0; c-- ) {
 pos = scan[ c ]
 if ( c == ( eob - 1 ) ) {
  coeff_base_eob S()
 level = coeff_base_eob + 1
 } else {
  coeff_base S()
 level = coeff_base
 }
 if ( level > NUM_BASE_LEVELS ) {
 for ( idx = 0;
 idx < COEFF_BASE_RANGE / ( BR_CDF_SIZE - 1 );
 idx++ ) {
  coeff_br S()
 level += coeff_br
 if ( coeff_br < ( BR_CDF_SIZE - 1 ) )
 break
 }
 }
 Quant[ pos ] = level
 }
 for ( c = 0; c < eob; c++ ) {
 pos = scan[ c ]
 if ( Quant[ pos ] != 0 ) {
 if ( c == 0 ) {
  dc_sign S()
 sign = dc_sign
 } else {
  sign_bit L(1)
 sign = sign_bit
 }
 } else {
 sign = 0
 }
 if ( Quant[ pos ] >
 ( NUM_BASE_LEVELS + COEFF_BASE_RANGE ) ) {
 length = 0
 do {
 length++
  golomb_length_bit L(1)
 } while ( !golomb_length_bit )
 x = 1
 for ( i = length - 2; i >= 0; i-- ) {
  golomb_data_bit L(1)
 x = ( x << 1 ) | golomb_data_bit
 }
 Quant[ pos ] = x + COEFF_BASE_RANGE + NUM_BASE_LEVELS
 }
 if ( pos == 0 && Quant[ pos ] > 0 ) {
 dcCategory = sign ? 1 : 2
 }
 Quant[ pos ] = Quant[ pos ] & 0xFFFFF
 culLevel += Quant[ pos ]
 if ( sign )
 Quant[ pos ] = - Quant[ pos ]
 }
 culLevel = Min( 63, culLevel )
 }
 for ( i = 0; i < w4; i++ ) {
 AboveLevelContext[ plane ][ x4 + i ] = culLevel
 AboveDcContext[ plane ][ x4 + i ] = dcCategory
 }
 for ( i = 0; i < h4; i++ ) {
 LeftLevelContext[ plane ][ y4 + i ] = culLevel
 LeftDcContext[ plane ][ y4 + i ] = dcCategory
 }
 return eob
 }
    */
    public class Coeffs : IAomSerializable
    {
		private uint plane;
		public uint Plane { get { return plane; } set { plane = value; } }
		private uint startX;
		public uint StartX { get { return startX; } set { startX = value; } }
		private uint startY;
		public uint StartY { get { return startY; } set { startY = value; } }
		private uint txSz;
		public uint TxSz { get { return txSz; } set { txSz = value; } }
		private Min min;
		public Min Min { get { return Min; } set { Min = value; } }
		private uint all_zero;
		public uint AllZero { get { return all_zero; } set { all_zero = value; } }
		private TransformType transform_type;
		public TransformType TransformType { get { return transform_type; } set { transform_type = value; } }
		private uint eob_pt_16;
		public uint EobPt16 { get { return eob_pt_16; } set { eob_pt_16 = value; } }
		private uint eob_pt_32;
		public uint EobPt32 { get { return eob_pt_32; } set { eob_pt_32 = value; } }
		private uint eob_pt_64;
		public uint EobPt64 { get { return eob_pt_64; } set { eob_pt_64 = value; } }
		private uint eob_pt_128;
		public uint EobPt128 { get { return eob_pt_128; } set { eob_pt_128 = value; } }
		private uint eob_pt_256;
		public uint EobPt256 { get { return eob_pt_256; } set { eob_pt_256 = value; } }
		private uint eob_pt_512;
		public uint EobPt512 { get { return eob_pt_512; } set { eob_pt_512 = value; } }
		private uint eob_pt_1024;
		public uint EobPt1024 { get { return eob_pt_1024; } set { eob_pt_1024 = value; } }
		private uint eob_extra;
		public uint EobExtra { get { return eob_extra; } set { eob_extra = value; } }
		private uint[] eob_extra_bit;
		public uint[] EobExtraBit { get { return eob_extra_bit; } set { eob_extra_bit = value; } }
		private uint[] coeff_base_eob;
		public uint[] CoeffBaseEob { get { return coeff_base_eob; } set { coeff_base_eob = value; } }
		private uint[] coeff_base;
		public uint[] CoeffBase { get { return coeff_base; } set { coeff_base = value; } }
		private uint[][] coeff_br;
		public uint[][] CoeffBr { get { return coeff_br; } set { coeff_br = value; } }
		private uint[] dc_sign;
		public uint[] DcSign { get { return dc_sign; } set { dc_sign = value; } }
		private uint[] sign_bit;
		public uint[] SignBit { get { return sign_bit; } set { sign_bit = value; } }
		private Dictionary<int, uint> golomb_length_bit = new Dictionary<int, uint>();
		public Dictionary<int, uint> GolombLengthBit { get { return golomb_length_bit; } set { golomb_length_bit = value; } }
		private uint[][] golomb_data_bit;
		public uint[][] GolombDataBit { get { return golomb_data_bit; } set { golomb_data_bit = value; } }

         public Coeffs(uint plane, uint startX, uint startY, uint txSz)
         { 
			this.plane = plane;
			this.startX = startX;
			this.startY = startY;
			this.txSz = txSz;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint x4 = 0;
			uint y4 = 0;
			uint w4 = 0;
			uint h4 = 0;
			uint txSzCtx = 0;
			uint ptype = 0;
			uint segEob = 0;
			uint c = 0;
			uint[] Quant = null;
			uint i = 0;
			uint j = 0;
			uint[][] Dequant = null;
			uint eob = 0;
			uint culLevel = 0;
			uint dcCategory = 0;
			uint[][] TxTypes = null;
			uint PlaneTxType = 0;
			uint scan = 0;
			uint eobMultisize = 0;
			uint eobPt = 0;
			uint eobShift = 0;
			uint pos = 0;
			uint level = 0;
			uint idx = 0;
			uint sign = 0;
			uint length = 0;
			int whileIndex = -1;
			uint x = 0;
			uint[][] AboveLevelContext = null;
			uint[][] AboveDcContext = null;
			uint[][] LeftLevelContext = null;
			uint[][] LeftDcContext = null;
			x4= startX >> 2;
			y4= startY >> 2;
			w4= Tx_Width[ txSz ] >> 2;
			h4= Tx_Height[ txSz ] >> 2;
			txSzCtx= ( Tx_Size_Sqr[txSz] + Tx_Size_Sqr_Up[txSz] + 1 ) >> 1;
			ptype= plane > 0 ? (uint)1 : (uint)0;
			segEob= ( txSz == TX_16X64 || txSz == TX_64X16 ) ? 512 :;
			this.Min =  new Min( 1024,  Tx_Width[ txSz ] * Tx_Height[ txSz ] ) ;
			size +=  stream.ReadClass<Min>(size, context, this.Min, "Min"); 

			for ( c = 0; c < segEob; c++ )
			{
				Quant[c]= 0;
			}

			for ( i = 0; i < 64; i++ )
			{

				for ( j = 0; j < 64; j++ )
				{
					Dequant[ i ][ j ]= 0;
				}
			}
			eob= 0;
			culLevel= 0;
			dcCategory= 0;
			size += stream.ReadS(size, out this.all_zero, "all_zero"); 

			if ( all_zero != 0 )
			{
				c= 0;

				if ( plane == 0 )
				{

					for ( i = 0; i < w4; i++ )
					{

						for ( j = 0; j < h4; j++ )
						{
							TxTypes[ y4 + j ][ x4 + i ]= DCT_DCT;
						}
					}
				}
			}
			else 
			{

				if ( plane == 0 )
				{
					this.transform_type =  new TransformType( x4,  y4,  txSz ) ;
					size +=  stream.ReadClass<TransformType>(size, context, this.transform_type, "transform_type"); 
				}
				PlaneTxType= compute_tx_type( plane, txSz, x4, y4 );
				scan= get_scan( txSz );
				eobMultisize= Math.Min( Tx_Width_Log2[ txSz ], 5) + Math.Min( Tx_Height_Log2[ txSz ], 5) - 4;

				if ( eobMultisize == 0 )
				{
					size += stream.ReadS(size, out this.eob_pt_16, "eob_pt_16"); 
					eobPt= eob_pt_16 + 1;
				}
				else if ( eobMultisize == 1 )
				{
					size += stream.ReadS(size, out this.eob_pt_32, "eob_pt_32"); 
					eobPt= eob_pt_32 + 1;
				}
				else if ( eobMultisize == 2 )
				{
					size += stream.ReadS(size, out this.eob_pt_64, "eob_pt_64"); 
					eobPt= eob_pt_64 + 1;
				}
				else if ( eobMultisize == 3 )
				{
					size += stream.ReadS(size, out this.eob_pt_128, "eob_pt_128"); 
					eobPt= eob_pt_128 + 1;
				}
				else if ( eobMultisize == 4 )
				{
					size += stream.ReadS(size, out this.eob_pt_256, "eob_pt_256"); 
					eobPt= eob_pt_256 + 1;
				}
				else if ( eobMultisize == 5 )
				{
					size += stream.ReadS(size, out this.eob_pt_512, "eob_pt_512"); 
					eobPt= eob_pt_512 + 1;
				}
				else 
				{
					size += stream.ReadS(size, out this.eob_pt_1024, "eob_pt_1024"); 
					eobPt= eob_pt_1024 + 1;
				}
				eob= ( eobPt < 2 ) ? eobPt : ( ( 1 << ( eobPt - 2 ) ) + 1 );
				eobShift= Math.Max( -1, eobPt - 3 );

				if ( eobShift >= 0 )
				{
					size += stream.ReadS(size, out this.eob_extra, "eob_extra"); 

					if ( eob_extra != 0 )
					{
						eob+= ( 1 << eobShift );
					}

					this.eob_extra_bit = new uint[ Math.Max( 0, eobPt - 2 )];
					for ( i = 1; i < Math.Max( 0, eobPt - 2 ); i++ )
					{
						eobShift= Math.Max( 0, eobPt - 2 ) - 1 - i;
						size += stream.ReadL(size, 1, out this.eob_extra_bit[ i ], "eob_extra_bit"); 

						if ( eob_extra_bit[i] != 0 )
						{
							eob+= ( 1 << eobShift );
						}
					}
				}

				this.coeff_base_eob = new uint[ 0];
				this.coeff_base = new uint[ 0];
				this.coeff_br = new uint[ 0][];
				for ( c = eob - 1; c >= 0; c-- )
				{
					pos= scan[ c ];

					if ( c == ( eob - 1 ) )
					{
						size += stream.ReadS(size, out this.coeff_base_eob[ c ], "coeff_base_eob"); 
						level= coeff_base_eob[c] + 1;
					}
					else 
					{
						size += stream.ReadS(size, out this.coeff_base[ c ], "coeff_base"); 
						level= coeff_base[c];
					}

					if ( level > NUM_BASE_LEVELS )
					{

						this.coeff_br[ c ] = new uint[ COEFF_BASE_RANGE / ( BR_CDF_SIZE - 1 )];
						for ( idx = 0;
 idx < COEFF_BASE_RANGE / ( BR_CDF_SIZE - 1 );
 idx++ )
						{
							size += stream.ReadS(size, out this.coeff_br[ c ][ idx ], "coeff_br"); 
							level+= coeff_br[c][x];

							if ( coeff_br[c][x] < ( BR_CDF_SIZE - 1 ) )
							{
break;
							}
						}
					}
					Quant[ pos ]= level;
				}

				this.dc_sign = new uint[ eob];
				this.sign_bit = new uint[ eob];
				this.golomb_data_bit = new uint[ eob][];
				for ( c = 0; c < eob; c++ )
				{
					pos= scan[ c ];

					if ( Quant[ pos ] != 0 )
					{

						if ( c == 0 )
						{
							size += stream.ReadS(size, out this.dc_sign[ c ], "dc_sign"); 
							sign= dc_sign[c];
						}
						else 
						{
							size += stream.ReadL(size, 1, out this.sign_bit[ c ], "sign_bit"); 
							sign= sign_bit[c];
						}
					}
					else 
					{
						sign= 0;
					}

					if ( Quant[ pos ] >
 ( NUM_BASE_LEVELS + COEFF_BASE_RANGE ) )
					{
						length= 0;

						do
						{
							whileIndex++;

							length++;
							size += stream.ReadL(size, 1, whileIndex, this.golomb_length_bit, "golomb_length_bit"); 
						} while ( golomb_length_bit[whileIndex][c]== 0 );
						x= 1;

						this.golomb_data_bit[ c ] = new uint[ 0];
						for ( i = length[c] - 2; i >= 0; i-- )
						{
							size += stream.ReadL(size, 1, out this.golomb_data_bit[ c ][ i ], "golomb_data_bit"); 
							x= ( x << 1 ) | golomb_data_bit[c][i];
						}
						Quant[ pos ]= x + COEFF_BASE_RANGE + NUM_BASE_LEVELS;
					}

					if ( pos == 0 && Quant[ pos ] > 0 )
					{
						dcCategory= sign ? 1 : 2;
					}
					Quant[ pos ]= Quant[ pos ] & 0xFFFFF;
					culLevel+= Quant[ pos ];

					if ( sign != 0 )
					{
						Quant[ pos ]= - Quant[ pos ];
					}
				}
				culLevel= Math.Min( 63, culLevel );
			}

			for ( i = 0; i < w4; i++ )
			{
				AboveLevelContext[ plane ][ x4 + i ]= culLevel;
				AboveDcContext[ plane ][ x4 + i ]= dcCategory;
			}

			for ( i = 0; i < h4; i++ )
			{
				LeftLevelContext[ plane ][ y4 + i ]= culLevel;
				LeftDcContext[ plane ][ y4 + i ]= dcCategory;
			}
return eob;

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint x4 = 0;
			uint y4 = 0;
			uint w4 = 0;
			uint h4 = 0;
			uint txSzCtx = 0;
			uint ptype = 0;
			uint segEob = 0;
			uint c = 0;
			uint[] Quant = null;
			uint i = 0;
			uint j = 0;
			uint[][] Dequant = null;
			uint eob = 0;
			uint culLevel = 0;
			uint dcCategory = 0;
			uint[][] TxTypes = null;
			uint PlaneTxType = 0;
			uint scan = 0;
			uint eobMultisize = 0;
			uint eobPt = 0;
			uint eobShift = 0;
			uint pos = 0;
			uint level = 0;
			uint idx = 0;
			uint sign = 0;
			uint length = 0;
			int whileIndex = -1;
			uint x = 0;
			uint[][] AboveLevelContext = null;
			uint[][] AboveDcContext = null;
			uint[][] LeftLevelContext = null;
			uint[][] LeftDcContext = null;
			x4= startX >> 2;
			y4= startY >> 2;
			w4= Tx_Width[ txSz ] >> 2;
			h4= Tx_Height[ txSz ] >> 2;
			txSzCtx= ( Tx_Size_Sqr[txSz] + Tx_Size_Sqr_Up[txSz] + 1 ) >> 1;
			ptype= plane > 0 ? (uint)1 : (uint)0;
			segEob= ( txSz == TX_16X64 || txSz == TX_64X16 ) ? 512 :;
			size += stream.WriteClass<Min>(context, this.Min, "Min"); 

			for ( c = 0; c < segEob; c++ )
			{
				Quant[c]= 0;
			}

			for ( i = 0; i < 64; i++ )
			{

				for ( j = 0; j < 64; j++ )
				{
					Dequant[ i ][ j ]= 0;
				}
			}
			eob= 0;
			culLevel= 0;
			dcCategory= 0;
			size += stream.WriteS( this.all_zero, "all_zero"); 

			if ( all_zero != 0 )
			{
				c= 0;

				if ( plane == 0 )
				{

					for ( i = 0; i < w4; i++ )
					{

						for ( j = 0; j < h4; j++ )
						{
							TxTypes[ y4 + j ][ x4 + i ]= DCT_DCT;
						}
					}
				}
			}
			else 
			{

				if ( plane == 0 )
				{
					size += stream.WriteClass<TransformType>(context, this.transform_type, "transform_type"); 
				}
				PlaneTxType= compute_tx_type( plane, txSz, x4, y4 );
				scan= get_scan( txSz );
				eobMultisize= Math.Min( Tx_Width_Log2[ txSz ], 5) + Math.Min( Tx_Height_Log2[ txSz ], 5) - 4;

				if ( eobMultisize == 0 )
				{
					size += stream.WriteS( this.eob_pt_16, "eob_pt_16"); 
					eobPt= eob_pt_16 + 1;
				}
				else if ( eobMultisize == 1 )
				{
					size += stream.WriteS( this.eob_pt_32, "eob_pt_32"); 
					eobPt= eob_pt_32 + 1;
				}
				else if ( eobMultisize == 2 )
				{
					size += stream.WriteS( this.eob_pt_64, "eob_pt_64"); 
					eobPt= eob_pt_64 + 1;
				}
				else if ( eobMultisize == 3 )
				{
					size += stream.WriteS( this.eob_pt_128, "eob_pt_128"); 
					eobPt= eob_pt_128 + 1;
				}
				else if ( eobMultisize == 4 )
				{
					size += stream.WriteS( this.eob_pt_256, "eob_pt_256"); 
					eobPt= eob_pt_256 + 1;
				}
				else if ( eobMultisize == 5 )
				{
					size += stream.WriteS( this.eob_pt_512, "eob_pt_512"); 
					eobPt= eob_pt_512 + 1;
				}
				else 
				{
					size += stream.WriteS( this.eob_pt_1024, "eob_pt_1024"); 
					eobPt= eob_pt_1024 + 1;
				}
				eob= ( eobPt < 2 ) ? eobPt : ( ( 1 << ( eobPt - 2 ) ) + 1 );
				eobShift= Math.Max( -1, eobPt - 3 );

				if ( eobShift >= 0 )
				{
					size += stream.WriteS( this.eob_extra, "eob_extra"); 

					if ( eob_extra != 0 )
					{
						eob+= ( 1 << eobShift );
					}

					for ( i = 1; i < Math.Max( 0, eobPt - 2 ); i++ )
					{
						eobShift= Math.Max( 0, eobPt - 2 ) - 1 - i;
						size += stream.WriteL(1,  this.eob_extra_bit[ i ], "eob_extra_bit"); 

						if ( eob_extra_bit[i] != 0 )
						{
							eob+= ( 1 << eobShift );
						}
					}
				}

				for ( c = eob - 1; c >= 0; c-- )
				{
					pos= scan[ c ];

					if ( c == ( eob - 1 ) )
					{
						size += stream.WriteS( this.coeff_base_eob[ c ], "coeff_base_eob"); 
						level= coeff_base_eob[c] + 1;
					}
					else 
					{
						size += stream.WriteS( this.coeff_base[ c ], "coeff_base"); 
						level= coeff_base[c];
					}

					if ( level > NUM_BASE_LEVELS )
					{

						for ( idx = 0;
 idx < COEFF_BASE_RANGE / ( BR_CDF_SIZE - 1 );
 idx++ )
						{
							size += stream.WriteS( this.coeff_br[ c ][ idx ], "coeff_br"); 
							level+= coeff_br[c][x];

							if ( coeff_br[c][x] < ( BR_CDF_SIZE - 1 ) )
							{
break;
							}
						}
					}
					Quant[ pos ]= level;
				}

				for ( c = 0; c < eob; c++ )
				{
					pos= scan[ c ];

					if ( Quant[ pos ] != 0 )
					{

						if ( c == 0 )
						{
							size += stream.WriteS( this.dc_sign[ c ], "dc_sign"); 
							sign= dc_sign[c];
						}
						else 
						{
							size += stream.WriteL(1,  this.sign_bit[ c ], "sign_bit"); 
							sign= sign_bit[c];
						}
					}
					else 
					{
						sign= 0;
					}

					if ( Quant[ pos ] >
 ( NUM_BASE_LEVELS + COEFF_BASE_RANGE ) )
					{
						length= 0;

						do
						{
							whileIndex++;

							length++;
							size += stream.WriteL(1,  whileIndex, this.golomb_length_bit, "golomb_length_bit"); 
						} while ( golomb_length_bit[whileIndex][c]== 0 );
						x= 1;

						for ( i = length[c] - 2; i >= 0; i-- )
						{
							size += stream.WriteL(1,  this.golomb_data_bit[ c ][ i ], "golomb_data_bit"); 
							x= ( x << 1 ) | golomb_data_bit[c][i];
						}
						Quant[ pos ]= x + COEFF_BASE_RANGE + NUM_BASE_LEVELS;
					}

					if ( pos == 0 && Quant[ pos ] > 0 )
					{
						dcCategory= sign ? 1 : 2;
					}
					Quant[ pos ]= Quant[ pos ] & 0xFFFFF;
					culLevel+= Quant[ pos ];

					if ( sign != 0 )
					{
						Quant[ pos ]= - Quant[ pos ];
					}
				}
				culLevel= Math.Min( 63, culLevel );
			}

			for ( i = 0; i < w4; i++ )
			{
				AboveLevelContext[ plane ][ x4 + i ]= culLevel;
				AboveDcContext[ plane ][ x4 + i ]= dcCategory;
			}

			for ( i = 0; i < h4; i++ )
			{
				LeftLevelContext[ plane ][ y4 + i ]= culLevel;
				LeftDcContext[ plane ][ y4 + i ]= dcCategory;
			}
return eob;

            return size;
         }

    }

    /*



 compute_tx_type( plane, txSz, blockX, blockY ) { 
 txSzSqrUp = Tx_Size_Sqr_Up[ txSz ]
 if ( Lossless || txSzSqrUp > TX_32X32 )
 return DCT_DCT
 txSet = get_tx_set( txSz )
 if ( plane == 0 ) {
 return TxTypes[ blockY ][ blockX ]
 }
 if ( is_inter ) {
 x4 = Max( MiCol, blockX << subsampling_x )
 y4 = Max( MiRow, blockY << subsampling_y )
 txType = TxTypes[ y4 ][ x4 ]
 if ( !is_tx_type_in_set( txSet, txType ) )
 return DCT_DCT
 return txType
 }
 txType = Mode_To_Txfm[ UVMode ]
 if ( !is_tx_type_in_set( txSet, txType ) )
 return DCT_DCT
 return txType
 }
    */
    public class ComputeTxType : IAomSerializable
    {
		private uint plane;
		public uint Plane { get { return plane; } set { plane = value; } }
		private uint txSz;
		public uint TxSz { get { return txSz; } set { txSz = value; } }
		private uint blockX;
		public uint BlockX { get { return blockX; } set { blockX = value; } }
		private uint blockY;
		public uint BlockY { get { return blockY; } set { blockY = value; } }

         public ComputeTxType(uint plane, uint txSz, uint blockX, uint blockY)
         { 
			this.plane = plane;
			this.txSz = txSz;
			this.blockX = blockX;
			this.blockY = blockY;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint txSzSqrUp = 0;
			uint txSet = 0;
			uint x4 = 0;
			uint y4 = 0;
			uint txType = 0;
			txSzSqrUp= Tx_Size_Sqr_Up[ txSz ];

			if ( Lossless != 0 || txSzSqrUp > TX_32X32 )
			{
return DCT_DCT;
			}
			txSet= get_tx_set( txSz );

			if ( plane == 0 )
			{
return TxTypes[ blockY ][ blockX ];
			}

			if ( is_inter != 0 )
			{
				x4= Math.Max( MiCol, blockX << subsampling_x );
				y4= Math.Max( MiRow, blockY << subsampling_y );
				txType= TxTypes[ y4 ][ x4 ];

				if ( !is_tx_type_in_set( txSet, txType ) )
				{
return DCT_DCT;
				}
return txType;
			}
			txType= Mode_To_Txfm[ UVMode ];

			if ( !is_tx_type_in_set( txSet, txType ) )
			{
return DCT_DCT;
			}
return txType;

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint txSzSqrUp = 0;
			uint txSet = 0;
			uint x4 = 0;
			uint y4 = 0;
			uint txType = 0;
			txSzSqrUp= Tx_Size_Sqr_Up[ txSz ];

			if ( Lossless != 0 || txSzSqrUp > TX_32X32 )
			{
return DCT_DCT;
			}
			txSet= get_tx_set( txSz );

			if ( plane == 0 )
			{
return TxTypes[ blockY ][ blockX ];
			}

			if ( is_inter != 0 )
			{
				x4= Math.Max( MiCol, blockX << subsampling_x );
				y4= Math.Max( MiRow, blockY << subsampling_y );
				txType= TxTypes[ y4 ][ x4 ];

				if ( !is_tx_type_in_set( txSet, txType ) )
				{
return DCT_DCT;
				}
return txType;
			}
			txType= Mode_To_Txfm[ UVMode ];

			if ( !is_tx_type_in_set( txSet, txType ) )
			{
return DCT_DCT;
			}
return txType;

            return size;
         }

    }

    /*

 is_tx_type_in_set( txSet, txType ) {
 return is_inter ? Tx_Type_In_Set_Inter[ txSet ][ txType ] : Tx_Type_In_Set_Intra[ txSet ][ txType ]
 }
    */
    public class IsTxTypeInSet : IAomSerializable
    {
		private uint txSet;
		public uint TxSet { get { return txSet; } set { txSet = value; } }
		private uint txType;
		public uint TxType { get { return txType; } set { txType = value; } }

         public IsTxTypeInSet(uint txSet, uint txType)
         { 
			this.txSet = txSet;
			this.txType = txType;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

return is_inter ? Tx_Type_In_Set_Inter[ txSet ][ txType ] : Tx_Type_In_Set_Intra[ txSet ][ txType ];

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

return is_inter ? Tx_Type_In_Set_Inter[ txSet ][ txType ] : Tx_Type_In_Set_Intra[ txSet ][ txType ];

            return size;
         }

    }

    /*



  get_mrow_scan( txSz ) { 
 if ( txSz == TX_4X4 )
 return Mrow_Scan_4x4
 else if ( txSz == TX_4X8 )
 return Mrow_Scan_4x8
 else if ( txSz == TX_8X4 )
 return Mrow_Scan_8x4
 else if ( txSz == TX_8X8 )
 return Mrow_Scan_8x8
 else if ( txSz == TX_8X16 )
 return Mrow_Scan_8x16
 else if ( txSz == TX_16X8 )
 return Mrow_Scan_16x8
 else if ( txSz == TX_16X16 )
 return Mrow_Scan_16x16
 else if ( txSz == TX_4X16 )
 return Mrow_Scan_4x16
 return Mrow_Scan_16x4
 }
    */
    public class GetMrowScan : IAomSerializable
    {
		private uint txSz;
		public uint TxSz { get { return txSz; } set { txSz = value; } }

         public GetMrowScan(uint txSz)
         { 
			this.txSz = txSz;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;


			if ( txSz == TX_4X4 )
			{
return Mrow_Scan_4x4;
			}
			else if ( txSz == TX_4X8 )
			{
return Mrow_Scan_4x8;
			}
			else if ( txSz == TX_8X4 )
			{
return Mrow_Scan_8x4;
			}
			else if ( txSz == TX_8X8 )
			{
return Mrow_Scan_8x8;
			}
			else if ( txSz == TX_8X16 )
			{
return Mrow_Scan_8x16;
			}
			else if ( txSz == TX_16X8 )
			{
return Mrow_Scan_16x8;
			}
			else if ( txSz == TX_16X16 )
			{
return Mrow_Scan_16x16;
			}
			else if ( txSz == TX_4X16 )
			{
return Mrow_Scan_4x16;
			}
return Mrow_Scan_16x4;

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;


			if ( txSz == TX_4X4 )
			{
return Mrow_Scan_4x4;
			}
			else if ( txSz == TX_4X8 )
			{
return Mrow_Scan_4x8;
			}
			else if ( txSz == TX_8X4 )
			{
return Mrow_Scan_8x4;
			}
			else if ( txSz == TX_8X8 )
			{
return Mrow_Scan_8x8;
			}
			else if ( txSz == TX_8X16 )
			{
return Mrow_Scan_8x16;
			}
			else if ( txSz == TX_16X8 )
			{
return Mrow_Scan_16x8;
			}
			else if ( txSz == TX_16X16 )
			{
return Mrow_Scan_16x16;
			}
			else if ( txSz == TX_4X16 )
			{
return Mrow_Scan_4x16;
			}
return Mrow_Scan_16x4;

            return size;
         }

    }

    /*

 get_mcol_scan( txSz ) {
 if ( txSz == TX_4X4 )
 return Mcol_Scan_4x4
 else if ( txSz == TX_4X8 )
 return Mcol_Scan_4x8
 else if ( txSz == TX_8X4 )
 return Mcol_Scan_8x4
 else if ( txSz == TX_8X8 )
 return Mcol_Scan_8x8
 else if ( txSz == TX_8X16 )
 return Mcol_Scan_8x16
 else if ( txSz == TX_16X8 )
 return Mcol_Scan_16x8
 else if ( txSz == TX_16X16 )
 return Mcol_Scan_16x16
 else if ( txSz == TX_4X16 )
 return Mcol_Scan_4x16
 return Mcol_Scan_16x4
 }
    */
    public class GetMcolScan : IAomSerializable
    {
		private uint txSz;
		public uint TxSz { get { return txSz; } set { txSz = value; } }

         public GetMcolScan(uint txSz)
         { 
			this.txSz = txSz;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;


			if ( txSz == TX_4X4 )
			{
return Mcol_Scan_4x4;
			}
			else if ( txSz == TX_4X8 )
			{
return Mcol_Scan_4x8;
			}
			else if ( txSz == TX_8X4 )
			{
return Mcol_Scan_8x4;
			}
			else if ( txSz == TX_8X8 )
			{
return Mcol_Scan_8x8;
			}
			else if ( txSz == TX_8X16 )
			{
return Mcol_Scan_8x16;
			}
			else if ( txSz == TX_16X8 )
			{
return Mcol_Scan_16x8;
			}
			else if ( txSz == TX_16X16 )
			{
return Mcol_Scan_16x16;
			}
			else if ( txSz == TX_4X16 )
			{
return Mcol_Scan_4x16;
			}
return Mcol_Scan_16x4;

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;


			if ( txSz == TX_4X4 )
			{
return Mcol_Scan_4x4;
			}
			else if ( txSz == TX_4X8 )
			{
return Mcol_Scan_4x8;
			}
			else if ( txSz == TX_8X4 )
			{
return Mcol_Scan_8x4;
			}
			else if ( txSz == TX_8X8 )
			{
return Mcol_Scan_8x8;
			}
			else if ( txSz == TX_8X16 )
			{
return Mcol_Scan_8x16;
			}
			else if ( txSz == TX_16X8 )
			{
return Mcol_Scan_16x8;
			}
			else if ( txSz == TX_16X16 )
			{
return Mcol_Scan_16x16;
			}
			else if ( txSz == TX_4X16 )
			{
return Mcol_Scan_4x16;
			}
return Mcol_Scan_16x4;

            return size;
         }

    }

    /*

 get_default_scan( txSz ) {
 if ( txSz == TX_4X4 )
 return Default_Scan_4x4
 else if ( txSz == TX_4X8 )
 return Default_Scan_4x8
 else if ( txSz == TX_8X4 )
 return Default_Scan_8x4
 else if ( txSz == TX_8X8 )
 return Default_Scan_8x8
 else if ( txSz == TX_8X16 )
 return Default_Scan_8x16
 else if ( txSz == TX_16X8 )
 return Default_Scan_16x8
 else if ( txSz == TX_16X16 )
 return Default_Scan_16x16
 else if ( txSz == TX_16X32 )
 return Default_Scan_16x32
 else if ( txSz == TX_32X16 )
 return Default_Scan_32x16
 else if ( txSz == TX_4X16 )
 return Default_Scan_4x16
 else if ( txSz == TX_16X4 )
 return Default_Scan_16x4
 else if ( txSz == TX_8X32 )
 return Default_Scan_8x32
 else if ( txSz == TX_32X8 )
 return Default_Scan_32x8
 return Default_Scan_32x32
 }
    */
    public class GetDefaultScan : IAomSerializable
    {
		private uint txSz;
		public uint TxSz { get { return txSz; } set { txSz = value; } }

         public GetDefaultScan(uint txSz)
         { 
			this.txSz = txSz;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;


			if ( txSz == TX_4X4 )
			{
return Default_Scan_4x4;
			}
			else if ( txSz == TX_4X8 )
			{
return Default_Scan_4x8;
			}
			else if ( txSz == TX_8X4 )
			{
return Default_Scan_8x4;
			}
			else if ( txSz == TX_8X8 )
			{
return Default_Scan_8x8;
			}
			else if ( txSz == TX_8X16 )
			{
return Default_Scan_8x16;
			}
			else if ( txSz == TX_16X8 )
			{
return Default_Scan_16x8;
			}
			else if ( txSz == TX_16X16 )
			{
return Default_Scan_16x16;
			}
			else if ( txSz == TX_16X32 )
			{
return Default_Scan_16x32;
			}
			else if ( txSz == TX_32X16 )
			{
return Default_Scan_32x16;
			}
			else if ( txSz == TX_4X16 )
			{
return Default_Scan_4x16;
			}
			else if ( txSz == TX_16X4 )
			{
return Default_Scan_16x4;
			}
			else if ( txSz == TX_8X32 )
			{
return Default_Scan_8x32;
			}
			else if ( txSz == TX_32X8 )
			{
return Default_Scan_32x8;
			}
return Default_Scan_32x32;

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;


			if ( txSz == TX_4X4 )
			{
return Default_Scan_4x4;
			}
			else if ( txSz == TX_4X8 )
			{
return Default_Scan_4x8;
			}
			else if ( txSz == TX_8X4 )
			{
return Default_Scan_8x4;
			}
			else if ( txSz == TX_8X8 )
			{
return Default_Scan_8x8;
			}
			else if ( txSz == TX_8X16 )
			{
return Default_Scan_8x16;
			}
			else if ( txSz == TX_16X8 )
			{
return Default_Scan_16x8;
			}
			else if ( txSz == TX_16X16 )
			{
return Default_Scan_16x16;
			}
			else if ( txSz == TX_16X32 )
			{
return Default_Scan_16x32;
			}
			else if ( txSz == TX_32X16 )
			{
return Default_Scan_32x16;
			}
			else if ( txSz == TX_4X16 )
			{
return Default_Scan_4x16;
			}
			else if ( txSz == TX_16X4 )
			{
return Default_Scan_16x4;
			}
			else if ( txSz == TX_8X32 )
			{
return Default_Scan_8x32;
			}
			else if ( txSz == TX_32X8 )
			{
return Default_Scan_32x8;
			}
return Default_Scan_32x32;

            return size;
         }

    }

    /*

 get_scan( txSz ) {
 if ( txSz == TX_16X64 ) {
 return Default_Scan_16x32
 }
 if ( txSz == TX_64X16 ) {
 return Default_Scan_32x16
 }
 if ( Tx_Size_Sqr_Up[ txSz ] == TX_64X64 ) {
 return Default_Scan_32x32
 }
 if ( PlaneTxType == IDTX ) {
 return get_default_scan( txSz )
 }
 preferRow = ( PlaneTxType == V_DCT || PlaneTxType == V_ADST || PlaneTxType == V_FLIPADST )
 preferCol = ( PlaneTxType == H_DCT || PlaneTxType == H_ADST || PlaneTxType == H_FLIPADST )
 if ( preferRow ) {
 return get_mrow_scan( txSz )
 } else if ( preferCol ) {
 return get_mcol_scan( txSz )
 }
 return get_default_scan( txSz )
 }
    */
    public class GetScan : IAomSerializable
    {
		private uint txSz;
		public uint TxSz { get { return txSz; } set { txSz = value; } }

         public GetScan(uint txSz)
         { 
			this.txSz = txSz;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint preferRow = 0;
			uint preferCol = 0;

			if ( txSz == TX_16X64 )
			{
return Default_Scan_16x32;
			}

			if ( txSz == TX_64X16 )
			{
return Default_Scan_32x16;
			}

			if ( Tx_Size_Sqr_Up[ txSz ] == TX_64X64 )
			{
return Default_Scan_32x32;
			}

			if ( PlaneTxType == IDTX )
			{
return get_default_scan( txSz );
			}
			preferRow= ( PlaneTxType == V_DCT || PlaneTxType == V_ADST || PlaneTxType == V_FLIPADST ) ? (uint)1 : (uint)0;
			preferCol= ( PlaneTxType == H_DCT || PlaneTxType == H_ADST || PlaneTxType == H_FLIPADST ) ? (uint)1 : (uint)0;

			if ( preferRow != 0 )
			{
return get_mrow_scan( txSz );
			}
			else if ( preferCol != 0 )
			{
return get_mcol_scan( txSz );
			}
return get_default_scan( txSz );

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint preferRow = 0;
			uint preferCol = 0;

			if ( txSz == TX_16X64 )
			{
return Default_Scan_16x32;
			}

			if ( txSz == TX_64X16 )
			{
return Default_Scan_32x16;
			}

			if ( Tx_Size_Sqr_Up[ txSz ] == TX_64X64 )
			{
return Default_Scan_32x32;
			}

			if ( PlaneTxType == IDTX )
			{
return get_default_scan( txSz );
			}
			preferRow= ( PlaneTxType == V_DCT || PlaneTxType == V_ADST || PlaneTxType == V_FLIPADST ) ? (uint)1 : (uint)0;
			preferCol= ( PlaneTxType == H_DCT || PlaneTxType == H_ADST || PlaneTxType == H_FLIPADST ) ? (uint)1 : (uint)0;

			if ( preferRow != 0 )
			{
return get_mrow_scan( txSz );
			}
			else if ( preferCol != 0 )
			{
return get_mcol_scan( txSz );
			}
return get_default_scan( txSz );

            return size;
         }

    }

    /*



  intra_angle_info_y() { 
 AngleDeltaY = 0
 if ( MiSize >= BLOCK_8X8 ) {
 if ( is_directional_mode( YMode ) ) {
 angle_delta_y S()
 AngleDeltaY = angle_delta_y - MAX_ANGLE_DELTA
 }
 }
 }
    */
    public class IntraAngleInfoy : IAomSerializable
    {
		private uint angle_delta_y;
		public uint AngleDeltay { get { return angle_delta_y; } set { angle_delta_y = value; } }

         public IntraAngleInfoy()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint AngleDeltaY = 0;
			AngleDeltaY= 0;

			if ( MiSize >= BLOCK_8X8 )
			{

				if ( is_directional_mode( YMode ) )
				{
					size += stream.ReadS(size, out this.angle_delta_y, "angle_delta_y"); 
					AngleDeltaY= angle_delta_y - MAX_ANGLE_DELTA;
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint AngleDeltaY = 0;
			AngleDeltaY= 0;

			if ( MiSize >= BLOCK_8X8 )
			{

				if ( is_directional_mode( YMode ) )
				{
					size += stream.WriteS( this.angle_delta_y, "angle_delta_y"); 
					AngleDeltaY= angle_delta_y - MAX_ANGLE_DELTA;
				}
			}

            return size;
         }

    }

    /*



 intra_angle_info_uv() { 
 AngleDeltaUV = 0
 if ( MiSize >= BLOCK_8X8 ) {
 if ( is_directional_mode( UVMode ) ) {
 angle_delta_uv S()
 AngleDeltaUV = angle_delta_uv - MAX_ANGLE_DELTA
 }
 }
 }
    */
    public class IntraAngleInfoUv : IAomSerializable
    {
		private uint angle_delta_uv;
		public uint AngleDeltaUv { get { return angle_delta_uv; } set { angle_delta_uv = value; } }

         public IntraAngleInfoUv()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint AngleDeltaUV = 0;
			AngleDeltaUV= 0;

			if ( MiSize >= BLOCK_8X8 )
			{

				if ( is_directional_mode( UVMode ) )
				{
					size += stream.ReadS(size, out this.angle_delta_uv, "angle_delta_uv"); 
					AngleDeltaUV= angle_delta_uv - MAX_ANGLE_DELTA;
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint AngleDeltaUV = 0;
			AngleDeltaUV= 0;

			if ( MiSize >= BLOCK_8X8 )
			{

				if ( is_directional_mode( UVMode ) )
				{
					size += stream.WriteS( this.angle_delta_uv, "angle_delta_uv"); 
					AngleDeltaUV= angle_delta_uv - MAX_ANGLE_DELTA;
				}
			}

            return size;
         }

    }

    /*


 is_directional_mode( mode ) { 
 if ( ( mode >= V_PRED ) && ( mode <= D67_PRED ) ) {
 return 1
 }
 return 0
 }
    */
    public class IsDirectionalMode : IAomSerializable
    {
		private uint mode;
		public uint Mode { get { return mode; } set { mode = value; } }

         public IsDirectionalMode(uint mode)
         { 
			this.mode = mode;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;


			if ( ( mode >= V_PRED ) && ( mode <= D67_PRED ) )
			{
return 1;
			}
return 0;

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;


			if ( ( mode >= V_PRED ) && ( mode <= D67_PRED ) )
			{
return 1;
			}
return 0;

            return size;
         }

    }

    /*



 read_cfl_alphas() { 
 cfl_alpha_signs cfl_alpha_signs S()
 signU = (cfl_alpha_signs + 1 ) / 3
 signV = (cfl_alpha_signs + 1 ) % 3
 if ( signU != CFL_SIGN_ZERO ) {
 cfl_alpha_u cfl_alpha_u S()
 CflAlphaU = 1 + cfl_alpha_u
 if ( signU == CFL_SIGN_NEG )
 CflAlphaU = -CflAlphaU
 } else {
 CflAlphaU = 0
 }
 if ( signV != CFL_SIGN_ZERO ) {
 cfl_alpha_v S()
 CflAlphaV = 1 + cfl_alpha_v
 if ( signV == CFL_SIGN_NEG )
 CflAlphaV = -CflAlphaV
 } else {
 CflAlphaV = 0
 }
 }
    */
    public class ReadCflAlphas : IAomSerializable
    {
		private CflAlphaSigns cfl_alpha_signs;
		public CflAlphaSigns CflAlphaSigns { get { return cfl_alpha_signs; } set { cfl_alpha_signs = value; } }
		private CflAlphau cfl_alpha_u;
		public CflAlphau CflAlphau { get { return cfl_alpha_u; } set { cfl_alpha_u = value; } }
		private uint cfl_alpha_v;
		public uint CflAlphav { get { return cfl_alpha_v; } set { cfl_alpha_v = value; } }

         public ReadCflAlphas()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint signU = 0;
			uint signV = 0;
			uint CflAlphaU = 0;
			uint CflAlphaV = 0;
			this.cfl_alpha_signs =  new CflAlphaSigns() ;
			size +=  stream.ReadClass<CflAlphaSigns>(size, context, this.cfl_alpha_signs, "cfl_alpha_signs"); 
			size += stream.ReadS(size, out this.cfl_alpha_signs, "cfl_alpha_signs"); 
			signU= (cfl_alpha_signs + 1 ) / 3;
			signV= (cfl_alpha_signs + 1 ) % 3;

			if ( signU != CFL_SIGN_ZERO )
			{
				this.cfl_alpha_u =  new CflAlphau() ;
				size +=  stream.ReadClass<CflAlphau>(size, context, this.cfl_alpha_u, "cfl_alpha_u"); 
				size += stream.ReadS(size, out this.cfl_alpha_u, "cfl_alpha_u"); 
				CflAlphaU= 1 + cfl_alpha_u;

				if ( signU == CFL_SIGN_NEG )
				{
					CflAlphaU= -CflAlphaU;
				}
			}
			else 
			{
				CflAlphaU= 0;
			}

			if ( signV != CFL_SIGN_ZERO )
			{
				size += stream.ReadS(size, out this.cfl_alpha_v, "cfl_alpha_v"); 
				CflAlphaV= 1 + cfl_alpha_v;

				if ( signV == CFL_SIGN_NEG )
				{
					CflAlphaV= -CflAlphaV;
				}
			}
			else 
			{
				CflAlphaV= 0;
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint signU = 0;
			uint signV = 0;
			uint CflAlphaU = 0;
			uint CflAlphaV = 0;
			size += stream.WriteClass<CflAlphaSigns>(context, this.cfl_alpha_signs, "cfl_alpha_signs"); 
			size += stream.WriteS( this.cfl_alpha_signs, "cfl_alpha_signs"); 
			signU= (cfl_alpha_signs + 1 ) / 3;
			signV= (cfl_alpha_signs + 1 ) % 3;

			if ( signU != CFL_SIGN_ZERO )
			{
				size += stream.WriteClass<CflAlphau>(context, this.cfl_alpha_u, "cfl_alpha_u"); 
				size += stream.WriteS( this.cfl_alpha_u, "cfl_alpha_u"); 
				CflAlphaU= 1 + cfl_alpha_u;

				if ( signU == CFL_SIGN_NEG )
				{
					CflAlphaU= -CflAlphaU;
				}
			}
			else 
			{
				CflAlphaU= 0;
			}

			if ( signV != CFL_SIGN_ZERO )
			{
				size += stream.WriteS( this.cfl_alpha_v, "cfl_alpha_v"); 
				CflAlphaV= 1 + cfl_alpha_v;

				if ( signV == CFL_SIGN_NEG )
				{
					CflAlphaV= -CflAlphaV;
				}
			}
			else 
			{
				CflAlphaV= 0;
			}

            return size;
         }

    }

    /*



palette_mode_info() { 
 bsizeCtx = Mi_Width_Log2[ MiSize ] + Mi_Height_Log2[ MiSize ] - 2
 if ( YMode == DC_PRED ) {
  has_palette_y S()
 if ( has_palette_y ) {
  palette_size_y_minus_2 S()
 PaletteSizeY = palette_size_y_minus_2 + 2
 cacheN = get_palette_cache( 0 )
 idx = 0
 for ( i = 0; i < cacheN && idx < PaletteSizeY; i++ ) {
  use_palette_color_cache_y L(1)
 if ( use_palette_color_cache_y ) {
 palette_colors_y[ idx ] = PaletteCache[ i ]
 idx++
 }
 }
 if ( idx < PaletteSizeY ) {
 palette_colors_y[ idx ] L(BitDepth)
 idx++
 }
 if ( idx < PaletteSizeY ) {
 minBits = BitDepth - 3
  palette_num_extra_bits_y L(2)
 paletteBits = minBits + palette_num_extra_bits_y
 }
 while ( idx < PaletteSizeY ) {
 palette_delta_y  L(paletteBits)
 palette_delta_y++
 palette_colors_y[ idx ] = Clip1( palette_colors_y[ idx - 1 ] + palette_delta_y )
 range = ( 1 << BitDepth ) - palette_colors_y[ idx ] - 1
 paletteBits = Min( paletteBits, CeilLog2( range ) )
 idx++
 }
 sort( palette_colors_y, 0, PaletteSizeY - 1 )
 }
 }
 if ( HasChroma && UVMode == DC_PRED ) {
  has_palette_uv S()
 if ( has_palette_uv ) {
 palette_size_uv_minus_2 S()
 PaletteSizeUV = palette_size_uv_minus_2 + 2
 cacheN = get_palette_cache( 1 )
 idx = 0
 for ( i = 0; i < cacheN && idx < PaletteSizeUV; i++ ) {
 use_palette_color_cache_u use_palette_color_cache_u L(1)
 if ( use_palette_color_cache_u ) {
 palette_colors_u[ idx ] = PaletteCache[ i ]
 idx++
 }
 }
 if ( idx < PaletteSizeUV ) {
  palette_colors_u[ idx ] L(BitDepth)
 idx++
 }
 if ( idx < PaletteSizeUV ) {
 minBits = BitDepth - 3
 palette_num_extra_bits_u L(2)
 paletteBits = minBits + palette_num_extra_bits_u
 }
 while ( idx < PaletteSizeUV ) {
 palette_delta_u palette_delta_u L(paletteBits)
 palette_colors_u[ idx ] =
 Clip1( palette_colors_u[ idx - 1 ] + palette_delta_u )
 range = ( 1 << BitDepth ) - palette_colors_u[ idx ]
 paletteBits = Min( paletteBits, CeilLog2( range ) )
 idx++
 }
 sort( palette_colors_u, 0, PaletteSizeUV - 1 )
 delta_encode_palette_colors_v L(1)
 if ( delta_encode_palette_colors_v ) {
 minBits = BitDepth - 4
 maxVal = 1 << BitDepth
 palette_num_extra_bits_v L(2)
 paletteBits = minBits + palette_num_extra_bits_v
 palette_colors_v[ 0 ] L(BitDepth)
 for ( idx = 1; idx < PaletteSizeUV; idx++ ) {
 palette_delta_v L(paletteBits)
 if ( palette_delta_v ) {
 palette_delta_sign_bit_v L(1)
 if ( palette_delta_sign_bit_v ) {
 palette_delta_v = -palette_delta_v
 }
 }
 val = palette_colors_v[ idx - 1 ] + palette_delta_v
 if ( val < 0 ) val += maxVal
 if ( val >= maxVal ) val -= maxVal
 palette_colors_v[ idx ] = Clip1( val )
 }
 } else {
 for ( idx = 0; idx < PaletteSizeUV; idx++ ) {
  palette_colors_v[ idx ] L(BitDepth)
 }
 }
 }
 }
 }
    */
    public class PaletteModeInfo : IAomSerializable
    {
		private uint has_palette_y;
		public uint HasPalettey { get { return has_palette_y; } set { has_palette_y = value; } }
		private uint palette_size_y_minus_2;
		public uint PaletteSizeyMinus2 { get { return palette_size_y_minus_2; } set { palette_size_y_minus_2 = value; } }
		private uint[] use_palette_color_cache_y;
		public uint[] UsePaletteColorCachey { get { return use_palette_color_cache_y; } set { use_palette_color_cache_y = value; } }
		private uint[] palette_colors_y;
		public uint[] PaletteColorsy { get { return palette_colors_y; } set { palette_colors_y = value; } }
		private uint palette_num_extra_bits_y;
		public uint PaletteNumExtraBitsy { get { return palette_num_extra_bits_y; } set { palette_num_extra_bits_y = value; } }
		private Dictionary<int, uint> palette_delta_y = new Dictionary<int, uint>();
		public Dictionary<int, uint> PaletteDeltay { get { return palette_delta_y; } set { palette_delta_y = value; } }
		private Sort sort;
		public Sort Sort { get { return sort; } set { sort = value; } }
		private uint has_palette_uv;
		public uint HasPaletteUv { get { return has_palette_uv; } set { has_palette_uv = value; } }
		private uint palette_size_uv_minus_2;
		public uint PaletteSizeUvMinus2 { get { return palette_size_uv_minus_2; } set { palette_size_uv_minus_2 = value; } }
		private UsePaletteColorCacheu[] use_palette_color_cache_u;
		public UsePaletteColorCacheu[] UsePaletteColorCacheu { get { return use_palette_color_cache_u; } set { use_palette_color_cache_u = value; } }
		private uint[] palette_colors_u;
		public uint[] PaletteColorsu { get { return palette_colors_u; } set { palette_colors_u = value; } }
		private uint palette_num_extra_bits_u;
		public uint PaletteNumExtraBitsu { get { return palette_num_extra_bits_u; } set { palette_num_extra_bits_u = value; } }
		private Dictionary<int, PaletteDeltau> palette_delta_u = new Dictionary<int, PaletteDeltau>();
		public Dictionary<int, PaletteDeltau> PaletteDeltau { get { return palette_delta_u; } set { palette_delta_u = value; } }
		private Sort sort0;
		public Sort Sort0 { get { return sort0; } set { sort0 = value; } }
		private uint delta_encode_palette_colors_v;
		public uint DeltaEncodePaletteColorsv { get { return delta_encode_palette_colors_v; } set { delta_encode_palette_colors_v = value; } }
		private uint palette_num_extra_bits_v;
		public uint PaletteNumExtraBitsv { get { return palette_num_extra_bits_v; } set { palette_num_extra_bits_v = value; } }
		private uint[] palette_colors_v;
		public uint[] PaletteColorsv { get { return palette_colors_v; } set { palette_colors_v = value; } }
		private uint[] palette_delta_v;
		public uint[] PaletteDeltav { get { return palette_delta_v; } set { palette_delta_v = value; } }
		private uint[] palette_delta_sign_bit_v;
		public uint[] PaletteDeltaSignBitv { get { return palette_delta_sign_bit_v; } set { palette_delta_sign_bit_v = value; } }

         public PaletteModeInfo()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint bsizeCtx = 0;
			uint PaletteSizeY = 0;
			uint cacheN = 0;
			uint idx = 0;
			uint i = 0;
			uint[] palette_colors_y = null;
			uint minBits = 0;
			uint paletteBits = 0;
			int whileIndex = -1;
			uint range = 0;
			uint PaletteSizeUV = 0;
			uint[] palette_colors_u = null;
			uint maxVal = 0;
			uint palette_delta_v = 0;
			uint val = 0;
			uint[] palette_colors_v = null;
			bsizeCtx= Mi_Width_Log2[ MiSize ] + Mi_Height_Log2[ MiSize ] - 2;

			if ( YMode == DC_PRED )
			{
				size += stream.ReadS(size, out this.has_palette_y, "has_palette_y"); 

				if ( has_palette_y != 0 )
				{
					size += stream.ReadS(size, out this.palette_size_y_minus_2, "palette_size_y_minus_2"); 
					PaletteSizeY= palette_size_y_minus_2 + 2;
					cacheN= get_palette_cache( 0 );
					idx= 0;

					this.use_palette_color_cache_y = new uint[ cacheN && idx < PaletteSizeY];
					for ( i = 0; i < cacheN && idx < PaletteSizeY; i++ )
					{
						size += stream.ReadL(size, 1, out this.use_palette_color_cache_y[ i ], "use_palette_color_cache_y"); 

						if ( use_palette_color_cache_y[i] != 0 )
						{
							palette_colors_y[ idx ]= PaletteCache[ i ];
							idx++;
						}
					}

					if ( idx < PaletteSizeY )
					{
						size += stream.ReadL(size, BitDepth, out this.palette_colors_y[ idx ], "palette_colors_y"); 
						idx++;
					}

					if ( idx < PaletteSizeY )
					{
						minBits= BitDepth - 3;
						size += stream.ReadL(size, 2, out this.palette_num_extra_bits_y, "palette_num_extra_bits_y"); 
						paletteBits= minBits + palette_num_extra_bits_y;
					}

					while ( idx[whileIndex] < PaletteSizeY )
					{
						whileIndex++;

						size += stream.ReadL(size, paletteBits, whileIndex, this.palette_delta_y, "palette_delta_y"); 
						palette_delta_y++;
						palette_colors_y[ idx ]= Clip1( palette_colors_y[ idx[whileIndex] - 1 ] + palette_delta_y[whileIndex] );
						range= ( 1 << BitDepth ) - palette_colors_y[ idx[whileIndex] ] - 1;
						paletteBits= Math.Min( paletteBits, CeilLog2( range ) );
						idx++;
					}
					this.sort =  new Sort( palette_colors_y,  0,  PaletteSizeY - 1 ) ;
					size +=  stream.ReadClass<Sort>(size, context, this.sort, "sort"); 
				}
			}

			if ( HasChroma != 0 && UVMode == DC_PRED )
			{
				size += stream.ReadS(size, out this.has_palette_uv, "has_palette_uv"); 

				if ( has_palette_uv != 0 )
				{
					size += stream.ReadS(size, out this.palette_size_uv_minus_2, "palette_size_uv_minus_2"); 
					PaletteSizeUV= palette_size_uv_minus_2 + 2;
					cacheN= get_palette_cache( 1 );
					idx= 0;

					this.use_palette_color_cache_u = new UsePaletteColorCacheu[ cacheN && idx < PaletteSizeUV];
					for ( i = 0; i < cacheN && idx < PaletteSizeUV; i++ )
					{
						this.use_palette_color_cache_u[ i ] =  new UsePaletteColorCacheu() ;
						size +=  stream.ReadClass<UsePaletteColorCacheu>(size, context, this.use_palette_color_cache_u[ i ], "use_palette_color_cache_u"); 
						size += stream.ReadL(size, 1, out this.use_palette_color_cache_u[ i ], "use_palette_color_cache_u"); 

						if ( use_palette_color_cache_u[i] != 0 )
						{
							palette_colors_u[ idx ]= PaletteCache[ i ];
							idx++;
						}
					}

					if ( idx < PaletteSizeUV )
					{
						size += stream.ReadL(size, BitDepth, out this.palette_colors_u[ idx ], "palette_colors_u"); 
						idx++;
					}

					if ( idx < PaletteSizeUV )
					{
						minBits= BitDepth - 3;
						size += stream.ReadL(size, 2, out this.palette_num_extra_bits_u, "palette_num_extra_bits_u"); 
						paletteBits= minBits + palette_num_extra_bits_u;
					}

					while ( idx[whileIndex] < PaletteSizeUV )
					{
						whileIndex++;

						this.palette_delta_u.Add(whileIndex,  new PaletteDeltau() );
						size +=  stream.ReadClass<PaletteDeltau>(size, context, this.palette_delta_u[whileIndex], "palette_delta_u"); 
						size += stream.ReadL(size, paletteBits, whileIndex, this.palette_delta_u, "palette_delta_u"); 
						palette_colors_u[ idx ]= Clip1( palette_colors_u[ idx[whileIndex] - 1 ] + palette_delta_u[whileIndex] );
						range= ( 1 << BitDepth ) - palette_colors_u[ idx[whileIndex] ];
						paletteBits= Math.Min( paletteBits, CeilLog2( range ) );
						idx++;
					}
					this.sort0 =  new Sort( palette_colors_u,  0,  PaletteSizeUV - 1 ) ;
					size +=  stream.ReadClass<Sort>(size, context, this.sort0, "sort0"); 
					size += stream.ReadL(size, 1, out this.delta_encode_palette_colors_v, "delta_encode_palette_colors_v"); 

					if ( delta_encode_palette_colors_v != 0 )
					{
						minBits= BitDepth - 4;
						maxVal= 1 << BitDepth;
						size += stream.ReadL(size, 2, out this.palette_num_extra_bits_v, "palette_num_extra_bits_v"); 
						paletteBits= minBits + palette_num_extra_bits_v;
						size += stream.ReadL(size, BitDepth, out this.palette_colors_v[ 0 ], "palette_colors_v"); 

						this.palette_delta_v = new uint[ PaletteSizeUV];
						this.palette_delta_sign_bit_v = new uint[ PaletteSizeUV];
						for ( idx = 1; idx < PaletteSizeUV; idx++ )
						{
							size += stream.ReadL(size, paletteBits, out this.palette_delta_v[ idx ], "palette_delta_v"); 

							if ( palette_delta_v[x] != 0 )
							{
								size += stream.ReadL(size, 1, out this.palette_delta_sign_bit_v[ idx ], "palette_delta_sign_bit_v"); 

								if ( palette_delta_sign_bit_v[x] != 0 )
								{
									palette_delta_v= -palette_delta_v[x];
								}
							}
							val= palette_colors_v[ idx - 1 ] + palette_delta_v[x];

							if ( val < 0 )
							{
								val+= maxVal;
							}

							if ( val >= maxVal )
							{
								val-= maxVal;
							}
							palette_colors_v[ idx ]= Clip1( val );
						}
					}
					else 
					{

						for ( idx = 0; idx < PaletteSizeUV; idx++ )
						{
							size += stream.ReadL(size, BitDepth, out this.palette_colors_v[ idx ], "palette_colors_v"); 
						}
					}
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint bsizeCtx = 0;
			uint PaletteSizeY = 0;
			uint cacheN = 0;
			uint idx = 0;
			uint i = 0;
			uint[] palette_colors_y = null;
			uint minBits = 0;
			uint paletteBits = 0;
			int whileIndex = -1;
			uint range = 0;
			uint PaletteSizeUV = 0;
			uint[] palette_colors_u = null;
			uint maxVal = 0;
			uint palette_delta_v = 0;
			uint val = 0;
			uint[] palette_colors_v = null;
			bsizeCtx= Mi_Width_Log2[ MiSize ] + Mi_Height_Log2[ MiSize ] - 2;

			if ( YMode == DC_PRED )
			{
				size += stream.WriteS( this.has_palette_y, "has_palette_y"); 

				if ( has_palette_y != 0 )
				{
					size += stream.WriteS( this.palette_size_y_minus_2, "palette_size_y_minus_2"); 
					PaletteSizeY= palette_size_y_minus_2 + 2;
					cacheN= get_palette_cache( 0 );
					idx= 0;

					for ( i = 0; i < cacheN && idx < PaletteSizeY; i++ )
					{
						size += stream.WriteL(1,  this.use_palette_color_cache_y[ i ], "use_palette_color_cache_y"); 

						if ( use_palette_color_cache_y[i] != 0 )
						{
							palette_colors_y[ idx ]= PaletteCache[ i ];
							idx++;
						}
					}

					if ( idx < PaletteSizeY )
					{
						size += stream.WriteL(BitDepth,  this.palette_colors_y[ idx ], "palette_colors_y"); 
						idx++;
					}

					if ( idx < PaletteSizeY )
					{
						minBits= BitDepth - 3;
						size += stream.WriteL(2,  this.palette_num_extra_bits_y, "palette_num_extra_bits_y"); 
						paletteBits= minBits + palette_num_extra_bits_y;
					}

					while ( idx[whileIndex] < PaletteSizeY )
					{
						whileIndex++;

						size += stream.WriteL(paletteBits,  whileIndex, this.palette_delta_y, "palette_delta_y"); 
						palette_delta_y++;
						palette_colors_y[ idx ]= Clip1( palette_colors_y[ idx[whileIndex] - 1 ] + palette_delta_y[whileIndex] );
						range= ( 1 << BitDepth ) - palette_colors_y[ idx[whileIndex] ] - 1;
						paletteBits= Math.Min( paletteBits, CeilLog2( range ) );
						idx++;
					}
					size += stream.WriteClass<Sort>(context, this.sort, "sort"); 
				}
			}

			if ( HasChroma != 0 && UVMode == DC_PRED )
			{
				size += stream.WriteS( this.has_palette_uv, "has_palette_uv"); 

				if ( has_palette_uv != 0 )
				{
					size += stream.WriteS( this.palette_size_uv_minus_2, "palette_size_uv_minus_2"); 
					PaletteSizeUV= palette_size_uv_minus_2 + 2;
					cacheN= get_palette_cache( 1 );
					idx= 0;

					for ( i = 0; i < cacheN && idx < PaletteSizeUV; i++ )
					{
						size += stream.WriteClass<UsePaletteColorCacheu>(context, this.use_palette_color_cache_u[ i ], "use_palette_color_cache_u"); 
						size += stream.WriteL(1,  this.use_palette_color_cache_u[ i ], "use_palette_color_cache_u"); 

						if ( use_palette_color_cache_u[i] != 0 )
						{
							palette_colors_u[ idx ]= PaletteCache[ i ];
							idx++;
						}
					}

					if ( idx < PaletteSizeUV )
					{
						size += stream.WriteL(BitDepth,  this.palette_colors_u[ idx ], "palette_colors_u"); 
						idx++;
					}

					if ( idx < PaletteSizeUV )
					{
						minBits= BitDepth - 3;
						size += stream.WriteL(2,  this.palette_num_extra_bits_u, "palette_num_extra_bits_u"); 
						paletteBits= minBits + palette_num_extra_bits_u;
					}

					while ( idx[whileIndex] < PaletteSizeUV )
					{
						whileIndex++;

						size += stream.WriteClass<PaletteDeltau>(context, whileIndex, this.palette_delta_u, "palette_delta_u"); 
						size += stream.WriteL(paletteBits,  whileIndex, this.palette_delta_u, "palette_delta_u"); 
						palette_colors_u[ idx ]= Clip1( palette_colors_u[ idx[whileIndex] - 1 ] + palette_delta_u[whileIndex] );
						range= ( 1 << BitDepth ) - palette_colors_u[ idx[whileIndex] ];
						paletteBits= Math.Min( paletteBits, CeilLog2( range ) );
						idx++;
					}
					size += stream.WriteClass<Sort>(context, this.sort0, "sort0"); 
					size += stream.WriteL(1,  this.delta_encode_palette_colors_v, "delta_encode_palette_colors_v"); 

					if ( delta_encode_palette_colors_v != 0 )
					{
						minBits= BitDepth - 4;
						maxVal= 1 << BitDepth;
						size += stream.WriteL(2,  this.palette_num_extra_bits_v, "palette_num_extra_bits_v"); 
						paletteBits= minBits + palette_num_extra_bits_v;
						size += stream.WriteL(BitDepth,  this.palette_colors_v[ 0 ], "palette_colors_v"); 

						for ( idx = 1; idx < PaletteSizeUV; idx++ )
						{
							size += stream.WriteL(paletteBits,  this.palette_delta_v[ idx ], "palette_delta_v"); 

							if ( palette_delta_v[x] != 0 )
							{
								size += stream.WriteL(1,  this.palette_delta_sign_bit_v[ idx ], "palette_delta_sign_bit_v"); 

								if ( palette_delta_sign_bit_v[x] != 0 )
								{
									palette_delta_v= -palette_delta_v[x];
								}
							}
							val= palette_colors_v[ idx - 1 ] + palette_delta_v[x];

							if ( val < 0 )
							{
								val+= maxVal;
							}

							if ( val >= maxVal )
							{
								val-= maxVal;
							}
							palette_colors_v[ idx ]= Clip1( val );
						}
					}
					else 
					{

						for ( idx = 0; idx < PaletteSizeUV; idx++ )
						{
							size += stream.WriteL(BitDepth,  this.palette_colors_v[ idx ], "palette_colors_v"); 
						}
					}
				}
			}

            return size;
         }

    }

    /*



get_palette_cache( plane ) { 
 aboveN = 0
 if ( ( MiRow * MI_SIZE ) % 64 ) {
 aboveN = PaletteSizes[ plane ][ MiRow - 1 ][ MiCol ]
 }
 leftN = 0
 if ( AvailL ) {
 leftN = PaletteSizes[ plane ][ MiRow ][ MiCol - 1 ]
 }
 aboveIdx = 0
 leftIdx = 0
 n = 0
 while ( aboveIdx < aboveN  && leftIdx < leftN ) {
 aboveC = PaletteColors[ plane ][ MiRow - 1 ][ MiCol ][ aboveIdx ]
 leftC = PaletteColors[ plane ][ MiRow ][ MiCol - 1 ][ leftIdx ]
 if ( leftC < aboveC ) {
 if ( n == 0 || leftC != PaletteCache[ n - 1 ] ) {
 PaletteCache[ n ] = leftC
 n++
 }
 leftIdx++
 } else {
 if ( n == 0 || aboveC != PaletteCache[ n - 1 ] ) {
 PaletteCache[ n ] = aboveC
 n++
 }
 aboveIdx++
 if ( leftC == aboveC ) {
 leftIdx++
 }
 }
 }
 while ( aboveIdx < aboveN ) {
 val = PaletteColors[ plane ][ MiRow - 1 ][ MiCol ][ aboveIdx ]
 aboveIdx++
 if ( n == 0 || val != PaletteCache[ n - 1 ] ) {
 PaletteCache[ n ] = val
 n++
 }
 }
 while ( leftIdx < leftN ) {
 val = PaletteColors[ plane ][ MiRow ][ MiCol - 1 ][ leftIdx ]
 leftIdx++
 if ( n == 0 || val != PaletteCache[ n - 1 ] ) {
 PaletteCache[ n ] = val
 n++
 }
 }
 return n
 }
    */
    public class GetPaletteCache : IAomSerializable
    {
		private uint plane;
		public uint Plane { get { return plane; } set { plane = value; } }

         public GetPaletteCache(uint plane)
         { 
			this.plane = plane;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint aboveN = 0;
			uint leftN = 0;
			uint aboveIdx = 0;
			uint leftIdx = 0;
			uint n = 0;
			int whileIndex = -1;
			uint aboveC = 0;
			uint leftC = 0;
			uint[] PaletteCache = null;
			uint val = 0;
			aboveN= 0;

			if ( ( MiRow * MI_SIZE ) % 64 != 0 )
			{
				aboveN= PaletteSizes[ plane ][ MiRow - 1 ][ MiCol ];
			}
			leftN= 0;

			if ( AvailL != 0 )
			{
				leftN= PaletteSizes[ plane ][ MiRow ][ MiCol - 1 ];
			}
			aboveIdx= 0;
			leftIdx= 0;
			n= 0;

			while ( aboveIdx < aboveN  && leftIdx < leftN )
			{
				whileIndex++;

				aboveC= PaletteColors[ plane ][ MiRow - 1 ][ MiCol ][ aboveIdx ];
				leftC= PaletteColors[ plane ][ MiRow ][ MiCol - 1 ][ leftIdx ];

				if ( leftC < aboveC )
				{

					if ( n == 0 || leftC != PaletteCache[ n - 1 ] )
					{
						PaletteCache[ n ]= leftC;
						n++;
					}
					leftIdx++;
				}
				else 
				{

					if ( n == 0 || aboveC != PaletteCache[ n - 1 ] )
					{
						PaletteCache[ n ]= aboveC;
						n++;
					}
					aboveIdx++;

					if ( leftC == aboveC )
					{
						leftIdx++;
					}
				}
			}

			while ( aboveIdx < aboveN )
			{
				whileIndex++;

				val= PaletteColors[ plane ][ MiRow - 1 ][ MiCol ][ aboveIdx[whileIndex] ];
				aboveIdx++;

				if ( n == 0 || val != PaletteCache[ n - 1 ] )
				{
					PaletteCache[ n ]= val;
					n++;
				}
			}

			while ( leftIdx < leftN )
			{
				whileIndex++;

				val= PaletteColors[ plane ][ MiRow ][ MiCol - 1 ][ leftIdx[whileIndex] ];
				leftIdx++;

				if ( n == 0 || val != PaletteCache[ n - 1 ] )
				{
					PaletteCache[ n ]= val;
					n++;
				}
			}
return n;

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint aboveN = 0;
			uint leftN = 0;
			uint aboveIdx = 0;
			uint leftIdx = 0;
			uint n = 0;
			int whileIndex = -1;
			uint aboveC = 0;
			uint leftC = 0;
			uint[] PaletteCache = null;
			uint val = 0;
			aboveN= 0;

			if ( ( MiRow * MI_SIZE ) % 64 != 0 )
			{
				aboveN= PaletteSizes[ plane ][ MiRow - 1 ][ MiCol ];
			}
			leftN= 0;

			if ( AvailL != 0 )
			{
				leftN= PaletteSizes[ plane ][ MiRow ][ MiCol - 1 ];
			}
			aboveIdx= 0;
			leftIdx= 0;
			n= 0;

			while ( aboveIdx < aboveN  && leftIdx < leftN )
			{
				whileIndex++;

				aboveC= PaletteColors[ plane ][ MiRow - 1 ][ MiCol ][ aboveIdx ];
				leftC= PaletteColors[ plane ][ MiRow ][ MiCol - 1 ][ leftIdx ];

				if ( leftC < aboveC )
				{

					if ( n == 0 || leftC != PaletteCache[ n - 1 ] )
					{
						PaletteCache[ n ]= leftC;
						n++;
					}
					leftIdx++;
				}
				else 
				{

					if ( n == 0 || aboveC != PaletteCache[ n - 1 ] )
					{
						PaletteCache[ n ]= aboveC;
						n++;
					}
					aboveIdx++;

					if ( leftC == aboveC )
					{
						leftIdx++;
					}
				}
			}

			while ( aboveIdx < aboveN )
			{
				whileIndex++;

				val= PaletteColors[ plane ][ MiRow - 1 ][ MiCol ][ aboveIdx[whileIndex] ];
				aboveIdx++;

				if ( n == 0 || val != PaletteCache[ n - 1 ] )
				{
					PaletteCache[ n ]= val;
					n++;
				}
			}

			while ( leftIdx < leftN )
			{
				whileIndex++;

				val= PaletteColors[ plane ][ MiRow ][ MiCol - 1 ][ leftIdx[whileIndex] ];
				leftIdx++;

				if ( n == 0 || val != PaletteCache[ n - 1 ] )
				{
					PaletteCache[ n ]= val;
					n++;
				}
			}
return n;

            return size;
         }

    }

    /*



transform_type( x4, y4, txSz ) { 
 set = get_tx_set( txSz )
 if ( set > 0 &&
 ( segmentation_enabled ? get_qindex( 1, segment_id ) : base_q_idx ) > 0 ) {
 if ( is_inter ) {
  inter_tx_type S()
 if ( set == TX_SET_INTER_1 )
 TxType = Tx_Type_Inter_Inv_Set1[ inter_tx_type ]
 else if ( set == TX_SET_INTER_2 )
 TxType = Tx_Type_Inter_Inv_Set2[ inter_tx_type ]
 else
 TxType = Tx_Type_Inter_Inv_Set3[ inter_tx_type ]
 } else {
  intra_tx_type S()
 if ( set == TX_SET_INTRA_1 )
 TxType = Tx_Type_Intra_Inv_Set1[ intra_tx_type ]
 else
 TxType = Tx_Type_Intra_Inv_Set2[ intra_tx_type ]
 }
 } else {
 TxType = DCT_DCT
 }
 for ( i = 0; i < ( Tx_Width[ txSz ] >> 2 ); i++ ) {
 for ( j = 0; j < ( Tx_Height[ txSz ] >> 2 ); j++ ) {
 TxTypes[ y4 + j ][ x4 + i ] = TxType
 }
 }
 }
    */
    public class TransformType : IAomSerializable
    {
		private uint x4;
		public uint X4 { get { return x4; } set { x4 = value; } }
		private uint y4;
		public uint Y4 { get { return y4; } set { y4 = value; } }
		private uint txSz;
		public uint TxSz { get { return txSz; } set { txSz = value; } }
		private uint inter_tx_type;
		public uint InterTxType { get { return inter_tx_type; } set { inter_tx_type = value; } }
		private uint intra_tx_type;
		public uint IntraTxType { get { return intra_tx_type; } set { intra_tx_type = value; } }

         public TransformType(uint x4, uint y4, uint txSz)
         { 
			this.x4 = x4;
			this.y4 = y4;
			this.txSz = txSz;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint set = 0;
			uint TxType = 0;
			uint i = 0;
			uint j = 0;
			uint[][] TxTypes = null;
			set= get_tx_set( txSz );

			if ( set > 0 &&
 ( segmentation_enabled ? get_qindex( 1, segment_id ) : base_q_idx ) > 0 )
			{

				if ( is_inter != 0 )
				{
					size += stream.ReadS(size, out this.inter_tx_type, "inter_tx_type"); 

					if ( set == TX_SET_INTER_1 )
					{
						TxType= Tx_Type_Inter_Inv_Set1[ inter_tx_type ];
					}
					else if ( set == TX_SET_INTER_2 )
					{
						TxType= Tx_Type_Inter_Inv_Set2[ inter_tx_type ];
					}
					else 
					{
						TxType= Tx_Type_Inter_Inv_Set3[ inter_tx_type ];
					}
				}
				else 
				{
					size += stream.ReadS(size, out this.intra_tx_type, "intra_tx_type"); 

					if ( set == TX_SET_INTRA_1 )
					{
						TxType= Tx_Type_Intra_Inv_Set1[ intra_tx_type ];
					}
					else 
					{
						TxType= Tx_Type_Intra_Inv_Set2[ intra_tx_type ];
					}
				}
			}
			else 
			{
				TxType= DCT_DCT;
			}

			for ( i = 0; i < ( Tx_Width[ txSz ] >> 2 ); i++ )
			{

				for ( j = 0; j < ( Tx_Height[ txSz ] >> 2 ); j++ )
				{
					TxTypes[ y4 + j ][ x4 + i ]= TxType;
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint set = 0;
			uint TxType = 0;
			uint i = 0;
			uint j = 0;
			uint[][] TxTypes = null;
			set= get_tx_set( txSz );

			if ( set > 0 &&
 ( segmentation_enabled ? get_qindex( 1, segment_id ) : base_q_idx ) > 0 )
			{

				if ( is_inter != 0 )
				{
					size += stream.WriteS( this.inter_tx_type, "inter_tx_type"); 

					if ( set == TX_SET_INTER_1 )
					{
						TxType= Tx_Type_Inter_Inv_Set1[ inter_tx_type ];
					}
					else if ( set == TX_SET_INTER_2 )
					{
						TxType= Tx_Type_Inter_Inv_Set2[ inter_tx_type ];
					}
					else 
					{
						TxType= Tx_Type_Inter_Inv_Set3[ inter_tx_type ];
					}
				}
				else 
				{
					size += stream.WriteS( this.intra_tx_type, "intra_tx_type"); 

					if ( set == TX_SET_INTRA_1 )
					{
						TxType= Tx_Type_Intra_Inv_Set1[ intra_tx_type ];
					}
					else 
					{
						TxType= Tx_Type_Intra_Inv_Set2[ intra_tx_type ];
					}
				}
			}
			else 
			{
				TxType= DCT_DCT;
			}

			for ( i = 0; i < ( Tx_Width[ txSz ] >> 2 ); i++ )
			{

				for ( j = 0; j < ( Tx_Height[ txSz ] >> 2 ); j++ )
				{
					TxTypes[ y4 + j ][ x4 + i ]= TxType;
				}
			}

            return size;
         }

    }

    /*



get_tx_set( txSz ) { 
 txSzSqr = Tx_Size_Sqr[ txSz ]
 txSzSqrUp = Tx_Size_Sqr_Up[ txSz ]
 if ( txSzSqrUp > TX_32X32 )
 return TX_SET_DCTONLY
 if ( is_inter ) {
 if ( reduced_tx_set || txSzSqrUp == TX_32X32 ) return TX_SET_INTER_3
 else if ( txSzSqr == TX_16X16 ) return TX_SET_INTER_2
 return TX_SET_INTER_1
 } else {
 if ( txSzSqrUp == TX_32X32 ) return TX_SET_DCTONLY
 else if ( reduced_tx_set ) return TX_SET_INTRA_2
 else if ( txSzSqr == TX_16X16 ) return TX_SET_INTRA_2
 return TX_SET_INTRA_1
 }
 }
    */
    public class GetTxSet : IAomSerializable
    {
		private uint txSz;
		public uint TxSz { get { return txSz; } set { txSz = value; } }

         public GetTxSet(uint txSz)
         { 
			this.txSz = txSz;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint txSzSqr = 0;
			uint txSzSqrUp = 0;
			txSzSqr= Tx_Size_Sqr[ txSz ];
			txSzSqrUp= Tx_Size_Sqr_Up[ txSz ];

			if ( txSzSqrUp > TX_32X32 )
			{
return TX_SET_DCTONLY;
			}

			if ( is_inter != 0 )
			{

				if ( reduced_tx_set != 0 || txSzSqrUp == TX_32X32 )
				{
return TX_SET_INTER_3;
				}
				else if ( txSzSqr == TX_16X16 )
				{
return TX_SET_INTER_2;
				}
return TX_SET_INTER_1;
			}
			else 
			{

				if ( txSzSqrUp == TX_32X32 )
				{
return TX_SET_DCTONLY;
				}
				else if ( reduced_tx_set != 0 )
				{
return TX_SET_INTRA_2;
				}
				else if ( txSzSqr == TX_16X16 )
				{
return TX_SET_INTRA_2;
				}
return TX_SET_INTRA_1;
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint txSzSqr = 0;
			uint txSzSqrUp = 0;
			txSzSqr= Tx_Size_Sqr[ txSz ];
			txSzSqrUp= Tx_Size_Sqr_Up[ txSz ];

			if ( txSzSqrUp > TX_32X32 )
			{
return TX_SET_DCTONLY;
			}

			if ( is_inter != 0 )
			{

				if ( reduced_tx_set != 0 || txSzSqrUp == TX_32X32 )
				{
return TX_SET_INTER_3;
				}
				else if ( txSzSqr == TX_16X16 )
				{
return TX_SET_INTER_2;
				}
return TX_SET_INTER_1;
			}
			else 
			{

				if ( txSzSqrUp == TX_32X32 )
				{
return TX_SET_DCTONLY;
				}
				else if ( reduced_tx_set != 0 )
				{
return TX_SET_INTRA_2;
				}
				else if ( txSzSqr == TX_16X16 )
				{
return TX_SET_INTRA_2;
				}
return TX_SET_INTRA_1;
			}

            return size;
         }

    }

    /*



palette_tokens() { 
 blockHeight = Block_Height[ MiSize ]
 blockWidth = Block_Width[ MiSize ]
 onscreenHeight = Min( blockHeight, (MiRows - MiRow) * MI_SIZE )
 onscreenWidth = Min( blockWidth, (MiCols - MiCol) * MI_SIZE )
 if ( PaletteSizeY ) {
  color_index_map_y NS(PaletteSizeY)
 ColorMapY[0][0] = color_index_map_y
 for ( i = 1; i < onscreenHeight + onscreenWidth - 1; i++ ) {
 for ( j = Min( i, onscreenWidth - 1 );
 j >= Max( 0, i - onscreenHeight + 1 ); j-- ) {
 get_palette_color_context(
 ColorMapY, ( i - j ), j, PaletteSizeY )
  palette_color_idx_y S()
 ColorMapY[ i - j ][ j ] = ColorOrder[ palette_color_idx_y ]
 }
 }
 for ( i = 0; i < onscreenHeight; i++ ) {
 for ( j = onscreenWidth; j < blockWidth; j++ ) {
 ColorMapY[ i ][ j ] = ColorMapY[ i ][ onscreenWidth - 1 ]
 }
 }
 for ( i = onscreenHeight; i < blockHeight; i++ ) {
 for ( j = 0; j < blockWidth; j++ ) {
 ColorMapY[ i ][ j ] = ColorMapY[ onscreenHeight - 1 ][ j ]
 }
 }
 }
 if ( PaletteSizeUV ) {
  color_index_map_uv NS(PaletteSizeUV)
 ColorMapUV[0][0] = color_index_map_uv
 blockHeight = blockHeight >> subsampling_y
 blockWidth = blockWidth >> subsampling_x
 onscreenHeight = onscreenHeight >> subsampling_y
 onscreenWidth = onscreenWidth >> subsampling_x
 if ( blockWidth < 4 ) {
 blockWidth += 2
 onscreenWidth += 2
 }
 if ( blockHeight < 4 ) {
 blockHeight += 2
 onscreenHeight += 2
 }
 for ( i = 1; i < onscreenHeight + onscreenWidth - 1; i++ ) {
 for ( j = Min( i, onscreenWidth - 1 );
 j >= Max( 0, i - onscreenHeight + 1 ); j-- ) {
 get_palette_color_context(
 ColorMapUV, ( i - j ), j, PaletteSizeUV )
  palette_color_idx_uv S()
 ColorMapUV[ i - j ][ j ] = ColorOrder[ palette_color_idx_uv ]
 }
 }
 for ( i = 0; i < onscreenHeight; i++ ) {
 for ( j = onscreenWidth; j < blockWidth; j++ ) {
 ColorMapUV[ i ][ j ] = ColorMapUV[ i ][ onscreenWidth - 1 ]
 }
 }
 for ( i = onscreenHeight; i < blockHeight; i++ ) {
 for ( j = 0; j < blockWidth; j++ ) {
 ColorMapUV[ i ][ j ] = ColorMapUV[ onscreenHeight - 1 ][ j ]
 }
 }
 }
 }
    */
    public class PaletteTokens : IAomSerializable
    {
		private ColorIndexMapy color_index_map_y;
		public ColorIndexMapy ColorIndexMapy { get { return color_index_map_y; } set { color_index_map_y = value; } }
		private NS nS;
		public NS NS { get { return NS; } set { NS = value; } }
		private GetPaletteColorContext[][] get_palette_color_context;
		public GetPaletteColorContext[][] GetPaletteColorContext { get { return get_palette_color_context; } set { get_palette_color_context = value; } }
		private uint[][] palette_color_idx_y;
		public uint[][] PaletteColorIdxy { get { return palette_color_idx_y; } set { palette_color_idx_y = value; } }
		private uint color_index_map_uv;
		public uint ColorIndexMapUv { get { return color_index_map_uv; } set { color_index_map_uv = value; } }
		private GetPaletteColorContext[][] get_palette_color_context0;
		public GetPaletteColorContext[][] GetPaletteColorContext0 { get { return get_palette_color_context0; } set { get_palette_color_context0 = value; } }
		private uint[][] palette_color_idx_uv;
		public uint[][] PaletteColorIdxUv { get { return palette_color_idx_uv; } set { palette_color_idx_uv = value; } }

         public PaletteTokens()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint blockHeight = 0;
			uint blockWidth = 0;
			uint onscreenHeight = 0;
			uint onscreenWidth = 0;
			uint[][] ColorMapY = null;
			uint i = 0;
			uint j = 0;
			uint[][] ColorMapUV = null;
			blockHeight= Block_Height[ MiSize ];
			blockWidth= Block_Width[ MiSize ];
			onscreenHeight= Math.Min( blockHeight, (MiRows - MiRow) * MI_SIZE );
			onscreenWidth= Math.Min( blockWidth, (MiCols - MiCol) * MI_SIZE );

			if ( PaletteSizeY != 0 )
			{
				this.color_index_map_y =  new ColorIndexMapy() ;
				size +=  stream.ReadClass<ColorIndexMapy>(size, context, this.color_index_map_y, "color_index_map_y"); 
				this.NS =  new NS(PaletteSizeY) ;
				size +=  stream.ReadClass<NS>(size, context, this.NS, "NS"); 
				ColorMapY[0][0]= color_index_map_y;

				this.get_palette_color_context = new GetPaletteColorContext[ onscreenHeight + onscreenWidth - 1][];
				this.palette_color_idx_y = new uint[ onscreenHeight + onscreenWidth - 1][];
				for ( i = 1; i < onscreenHeight + onscreenWidth - 1; i++ )
				{

					this.get_palette_color_context[ i ] = new GetPaletteColorContext[ Math.Max( 0, i - onscreenHeight + 1 )];
					this.palette_color_idx_y[ i ] = new uint[ Math.Max( 0, i - onscreenHeight + 1 )];
					for ( j = Math.Min( i, onscreenWidth - 1 );
 j >= Math.Max( 0, i - onscreenHeight + 1 ); j-- )
					{
						this.get_palette_color_context[ i ][ j ] =  new GetPaletteColorContext( ColorMapY,  ( i - j ),  j,  PaletteSizeY ) ;
						size +=  stream.ReadClass<GetPaletteColorContext>(size, context, this.get_palette_color_context[ i ][ j ], "get_palette_color_context"); 
						size += stream.ReadS(size, out this.palette_color_idx_y[ i ][ j ], "palette_color_idx_y"); 
						ColorMapY[ i - j ][ j ]= ColorOrder[ palette_color_idx_y[i][j] ];
					}
				}

				for ( i = 0; i < onscreenHeight; i++ )
				{

					for ( j = onscreenWidth; j < blockWidth; j++ )
					{
						ColorMapY[ i ][ j ]= ColorMapY[ i ][ onscreenWidth - 1 ];
					}
				}

				for ( i = onscreenHeight; i < blockHeight; i++ )
				{

					for ( j = 0; j < blockWidth; j++ )
					{
						ColorMapY[ i ][ j ]= ColorMapY[ onscreenHeight - 1 ][ j ];
					}
				}
			}

			if ( PaletteSizeUV != 0 )
			{
				size += stream.ReadNS(size, PaletteSizeUV, out this.color_index_map_uv, "color_index_map_uv"); 
				ColorMapUV[0][0]= color_index_map_uv;
				blockHeight= blockHeight >> subsampling_y;
				blockWidth= blockWidth >> subsampling_x;
				onscreenHeight= onscreenHeight >> subsampling_y;
				onscreenWidth= onscreenWidth >> subsampling_x;

				if ( blockWidth < 4 )
				{
					blockWidth+= 2;
					onscreenWidth+= 2;
				}

				if ( blockHeight < 4 )
				{
					blockHeight+= 2;
					onscreenHeight+= 2;
				}

				this.get_palette_color_context0 = new GetPaletteColorContext[ onscreenHeight + onscreenWidth - 1][];
				this.palette_color_idx_uv = new uint[ onscreenHeight + onscreenWidth - 1][];
				for ( i = 1; i < onscreenHeight + onscreenWidth - 1; i++ )
				{

					this.get_palette_color_context0[ i ] = new GetPaletteColorContext[ Math.Max( 0, i - onscreenHeight + 1 )];
					this.palette_color_idx_uv[ i ] = new uint[ Math.Max( 0, i - onscreenHeight + 1 )];
					for ( j = Math.Min( i, onscreenWidth - 1 );
 j >= Math.Max( 0, i - onscreenHeight + 1 ); j-- )
					{
						this.get_palette_color_context0[ i ][ j ] =  new GetPaletteColorContext( ColorMapUV,  ( i - j ),  j,  PaletteSizeUV ) ;
						size +=  stream.ReadClass<GetPaletteColorContext>(size, context, this.get_palette_color_context0[ i ][ j ], "get_palette_color_context0"); 
						size += stream.ReadS(size, out this.palette_color_idx_uv[ i ][ j ], "palette_color_idx_uv"); 
						ColorMapUV[ i - j ][ j ]= ColorOrder[ palette_color_idx_uv[i][j] ];
					}
				}

				for ( i = 0; i < onscreenHeight; i++ )
				{

					for ( j = onscreenWidth; j < blockWidth; j++ )
					{
						ColorMapUV[ i ][ j ]= ColorMapUV[ i ][ onscreenWidth - 1 ];
					}
				}

				for ( i = onscreenHeight; i < blockHeight; i++ )
				{

					for ( j = 0; j < blockWidth; j++ )
					{
						ColorMapUV[ i ][ j ]= ColorMapUV[ onscreenHeight - 1 ][ j ];
					}
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint blockHeight = 0;
			uint blockWidth = 0;
			uint onscreenHeight = 0;
			uint onscreenWidth = 0;
			uint[][] ColorMapY = null;
			uint i = 0;
			uint j = 0;
			uint[][] ColorMapUV = null;
			blockHeight= Block_Height[ MiSize ];
			blockWidth= Block_Width[ MiSize ];
			onscreenHeight= Math.Min( blockHeight, (MiRows - MiRow) * MI_SIZE );
			onscreenWidth= Math.Min( blockWidth, (MiCols - MiCol) * MI_SIZE );

			if ( PaletteSizeY != 0 )
			{
				size += stream.WriteClass<ColorIndexMapy>(context, this.color_index_map_y, "color_index_map_y"); 
				size += stream.WriteClass<NS>(context, this.NS, "NS"); 
				ColorMapY[0][0]= color_index_map_y;

				for ( i = 1; i < onscreenHeight + onscreenWidth - 1; i++ )
				{

					for ( j = Math.Min( i, onscreenWidth - 1 );
 j >= Math.Max( 0, i - onscreenHeight + 1 ); j-- )
					{
						size += stream.WriteClass<GetPaletteColorContext>(context, this.get_palette_color_context[ i ][ j ], "get_palette_color_context"); 
						size += stream.WriteS( this.palette_color_idx_y[ i ][ j ], "palette_color_idx_y"); 
						ColorMapY[ i - j ][ j ]= ColorOrder[ palette_color_idx_y[i][j] ];
					}
				}

				for ( i = 0; i < onscreenHeight; i++ )
				{

					for ( j = onscreenWidth; j < blockWidth; j++ )
					{
						ColorMapY[ i ][ j ]= ColorMapY[ i ][ onscreenWidth - 1 ];
					}
				}

				for ( i = onscreenHeight; i < blockHeight; i++ )
				{

					for ( j = 0; j < blockWidth; j++ )
					{
						ColorMapY[ i ][ j ]= ColorMapY[ onscreenHeight - 1 ][ j ];
					}
				}
			}

			if ( PaletteSizeUV != 0 )
			{
				size += stream.WriteNS(PaletteSizeUV, this.color_index_map_uv, "color_index_map_uv"); 
				ColorMapUV[0][0]= color_index_map_uv;
				blockHeight= blockHeight >> subsampling_y;
				blockWidth= blockWidth >> subsampling_x;
				onscreenHeight= onscreenHeight >> subsampling_y;
				onscreenWidth= onscreenWidth >> subsampling_x;

				if ( blockWidth < 4 )
				{
					blockWidth+= 2;
					onscreenWidth+= 2;
				}

				if ( blockHeight < 4 )
				{
					blockHeight+= 2;
					onscreenHeight+= 2;
				}

				for ( i = 1; i < onscreenHeight + onscreenWidth - 1; i++ )
				{

					for ( j = Math.Min( i, onscreenWidth - 1 );
 j >= Math.Max( 0, i - onscreenHeight + 1 ); j-- )
					{
						size += stream.WriteClass<GetPaletteColorContext>(context, this.get_palette_color_context0[ i ][ j ], "get_palette_color_context0"); 
						size += stream.WriteS( this.palette_color_idx_uv[ i ][ j ], "palette_color_idx_uv"); 
						ColorMapUV[ i - j ][ j ]= ColorOrder[ palette_color_idx_uv[i][j] ];
					}
				}

				for ( i = 0; i < onscreenHeight; i++ )
				{

					for ( j = onscreenWidth; j < blockWidth; j++ )
					{
						ColorMapUV[ i ][ j ]= ColorMapUV[ i ][ onscreenWidth - 1 ];
					}
				}

				for ( i = onscreenHeight; i < blockHeight; i++ )
				{

					for ( j = 0; j < blockWidth; j++ )
					{
						ColorMapUV[ i ][ j ]= ColorMapUV[ onscreenHeight - 1 ][ j ];
					}
				}
			}

            return size;
         }

    }

    /*



get_palette_color_context( colorMap, r, c, n ) { 
 for ( i = 0; i < PALETTE_COLORS; i++ ) {
 scores[ i ] = 0
 ColorOrder[i] = i
 }
 if ( c > 0 ) {
 neighbor = colorMap[ r ][ c - 1 ]
 scores[ neighbor ] += 2
 }
 if ( ( r > 0 ) && ( c > 0 ) ) {
 neighbor = colorMap[ r - 1 ][ c - 1 ]
 scores[ neighbor ] += 1
 }
 if ( r > 0 ) {
 neighbor = colorMap[ r - 1 ][ c ]
 scores[ neighbor ] += 2
 }
 for ( i = 0; i < PALETTE_NUM_NEIGHBORS; i++ ) {
 maxScore = scores[ i ]
 maxIdx = i
 for ( j = i + 1; j < n; j++ ) {
 if ( scores[ j ] > maxScore ) {
 maxScore = scores[ j ]
 maxIdx = j
 }
 }
 if ( maxIdx != i ) {
 maxScore = scores[ maxIdx ]
 maxColorOrder = ColorOrder[ maxIdx ]
 for ( k = maxIdx; k > i; k-- ) {
 scores[ k ] = scores[ k - 1 ]
 ColorOrder[ k ] = ColorOrder[ k - 1 ]
 }
 scores[ i ] = maxScore
 ColorOrder[ i ] = maxColorOrder
 }
 }
 ColorContextHash = 0
 for ( i = 0; i < PALETTE_NUM_NEIGHBORS; i++ ) {
 ColorContextHash += scores[ i ] * Palette_Color_Hash_Multipliers[ i ]
 }
 }
    */
    public class GetPaletteColorContext : IAomSerializable
    {
		private uint colorMap;
		public uint ColorMap { get { return colorMap; } set { colorMap = value; } }
		private uint r;
		public uint r { get { return r; } set { r = value; } }
		private uint c;
		public uint c { get { return c; } set { c = value; } }
		private uint n;
		public uint n { get { return n; } set { n = value; } }

         public GetPaletteColorContext(uint colorMap, uint r, uint c, uint n)
         { 
			this.colorMap = colorMap;
			this.r = r;
			this.c = c;
			this.n = n;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint i = 0;
			uint[] scores = null;
			uint[] ColorOrder = null;
			uint neighbor = 0;
			uint maxScore = 0;
			uint maxIdx = 0;
			uint j = 0;
			uint maxColorOrder = 0;
			uint k = 0;
			uint ColorContextHash = 0;

			for ( i = 0; i < PALETTE_COLORS; i++ )
			{
				scores[ i ]= 0;
				ColorOrder[i]= i;
			}

			if ( c > 0 )
			{
				neighbor= colorMap[ r ][ c - 1 ];
				scores[ neighbor ]+= 2;
			}

			if ( ( r > 0 ) && ( c > 0 ) )
			{
				neighbor= colorMap[ r - 1 ][ c - 1 ];
				scores[ neighbor ]+= 1;
			}

			if ( r > 0 )
			{
				neighbor= colorMap[ r - 1 ][ c ];
				scores[ neighbor ]+= 2;
			}

			for ( i = 0; i < PALETTE_NUM_NEIGHBORS; i++ )
			{
				maxScore= scores[ i ];
				maxIdx= i;

				for ( j = i + 1; j < n; j++ )
				{

					if ( scores[ j ] > maxScore )
					{
						maxScore= scores[ j ];
						maxIdx= j;
					}
				}

				if ( maxIdx != i )
				{
					maxScore= scores[ maxIdx ];
					maxColorOrder= ColorOrder[ maxIdx ];

					for ( k = maxIdx; k > i; k-- )
					{
						scores[ k ]= scores[ k - 1 ];
						ColorOrder[ k ]= ColorOrder[ k - 1 ];
					}
					scores[ i ]= maxScore;
					ColorOrder[ i ]= maxColorOrder;
				}
			}
			ColorContextHash= 0;

			for ( i = 0; i < PALETTE_NUM_NEIGHBORS; i++ )
			{
				ColorContextHash+= scores[ i ] * Palette_Color_Hash_Multipliers[ i ];
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint i = 0;
			uint[] scores = null;
			uint[] ColorOrder = null;
			uint neighbor = 0;
			uint maxScore = 0;
			uint maxIdx = 0;
			uint j = 0;
			uint maxColorOrder = 0;
			uint k = 0;
			uint ColorContextHash = 0;

			for ( i = 0; i < PALETTE_COLORS; i++ )
			{
				scores[ i ]= 0;
				ColorOrder[i]= i;
			}

			if ( c > 0 )
			{
				neighbor= colorMap[ r ][ c - 1 ];
				scores[ neighbor ]+= 2;
			}

			if ( ( r > 0 ) && ( c > 0 ) )
			{
				neighbor= colorMap[ r - 1 ][ c - 1 ];
				scores[ neighbor ]+= 1;
			}

			if ( r > 0 )
			{
				neighbor= colorMap[ r - 1 ][ c ];
				scores[ neighbor ]+= 2;
			}

			for ( i = 0; i < PALETTE_NUM_NEIGHBORS; i++ )
			{
				maxScore= scores[ i ];
				maxIdx= i;

				for ( j = i + 1; j < n; j++ )
				{

					if ( scores[ j ] > maxScore )
					{
						maxScore= scores[ j ];
						maxIdx= j;
					}
				}

				if ( maxIdx != i )
				{
					maxScore= scores[ maxIdx ];
					maxColorOrder= ColorOrder[ maxIdx ];

					for ( k = maxIdx; k > i; k-- )
					{
						scores[ k ]= scores[ k - 1 ];
						ColorOrder[ k ]= ColorOrder[ k - 1 ];
					}
					scores[ i ]= maxScore;
					ColorOrder[ i ]= maxColorOrder;
				}
			}
			ColorContextHash= 0;

			for ( i = 0; i < PALETTE_NUM_NEIGHBORS; i++ )
			{
				ColorContextHash+= scores[ i ] * Palette_Color_Hash_Multipliers[ i ];
			}

            return size;
         }

    }

    /*



is_inside( candidateR, candidateC ) { 
 return ( candidateC >= MiColStart && candidateC < MiColEnd && candidateR >= MiRowStart && candidateR < MiRowEnd )
 }
    */
    public class IsInside : IAomSerializable
    {
		private uint candidateR;
		public uint CandidateR { get { return candidateR; } set { candidateR = value; } }
		private uint candidateC;
		public uint CandidateC { get { return candidateC; } set { candidateC = value; } }

         public IsInside(uint candidateR, uint candidateC)
         { 
			this.candidateR = candidateR;
			this.candidateC = candidateC;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

return ( candidateC >= MiColStart && candidateC < MiColEnd && candidateR >= MiRowStart && candidateR < MiRowEnd );

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

return ( candidateC >= MiColStart && candidateC < MiColEnd && candidateR >= MiRowStart && candidateR < MiRowEnd );

            return size;
         }

    }

    /*



is_inside_filter_region( candidateR, candidateC ) { 
 colStart = 0
 colEnd = MiCols
 rowStart = 0
 rowEnd = MiRows
 return (candidateC >= colStart && candidateC < colEnd && candidateR >= rowStart && candidateR < rowEnd)
 }
    */
    public class IsInsideFilterRegion : IAomSerializable
    {
		private uint candidateR;
		public uint CandidateR { get { return candidateR; } set { candidateR = value; } }
		private uint candidateC;
		public uint CandidateC { get { return candidateC; } set { candidateC = value; } }

         public IsInsideFilterRegion(uint candidateR, uint candidateC)
         { 
			this.candidateR = candidateR;
			this.candidateC = candidateC;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint colStart = 0;
			uint colEnd = 0;
			uint rowStart = 0;
			uint rowEnd = 0;
			colStart= 0;
			colEnd= MiCols;
			rowStart= 0;
			rowEnd= MiRows;
return (candidateC >= colStart && candidateC < colEnd && candidateR >= rowStart && candidateR < rowEnd);

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint colStart = 0;
			uint colEnd = 0;
			uint rowStart = 0;
			uint rowEnd = 0;
			colStart= 0;
			colEnd= MiCols;
			rowStart= 0;
			rowEnd= MiRows;
return (candidateC >= colStart && candidateC < colEnd && candidateR >= rowStart && candidateR < rowEnd);

            return size;
         }

    }

    /*



clamp_mv_row( mvec, border ) { 
 bh4 = Num_4x4_Blocks_High[ MiSize ]
 mbToTopEdge = -((MiRow * MI_SIZE) * 8)
 mbToBottomEdge = ((MiRows - bh4 - MiRow) * MI_SIZE) * 8
 return Clip3( mbToTopEdge - border, mbToBottomEdge + border, mvec )
 }
    */
    public class ClampMvRow : IAomSerializable
    {
		private uint mvec;
		public uint Mvec { get { return mvec; } set { mvec = value; } }
		private uint border;
		public uint Border { get { return border; } set { border = value; } }

         public ClampMvRow(uint mvec, uint border)
         { 
			this.mvec = mvec;
			this.border = border;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint bh4 = 0;
			uint mbToTopEdge = 0;
			uint mbToBottomEdge = 0;
			bh4= Num_4x4_Blocks_High[ MiSize ];
			mbToTopEdge= -((MiRow * MI_SIZE) * 8);
			mbToBottomEdge= ((MiRows - bh4 - MiRow) * MI_SIZE) * 8;
return Clip3( mbToTopEdge - border, mbToBottomEdge + border, mvec );

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint bh4 = 0;
			uint mbToTopEdge = 0;
			uint mbToBottomEdge = 0;
			bh4= Num_4x4_Blocks_High[ MiSize ];
			mbToTopEdge= -((MiRow * MI_SIZE) * 8);
			mbToBottomEdge= ((MiRows - bh4 - MiRow) * MI_SIZE) * 8;
return Clip3( mbToTopEdge - border, mbToBottomEdge + border, mvec );

            return size;
         }

    }

    /*



clamp_mv_col( mvec, border ) { 
 bw4 = Num_4x4_Blocks_Wide[ MiSize ]
 mbToLeftEdge = -((MiCol * MI_SIZE) * 8)
 mbToRightEdge = ((MiCols - bw4 - MiCol) * MI_SIZE) * 8
 return Clip3( mbToLeftEdge - border, mbToRightEdge + border, mvec )
 }
    */
    public class ClampMvCol : IAomSerializable
    {
		private uint mvec;
		public uint Mvec { get { return mvec; } set { mvec = value; } }
		private uint border;
		public uint Border { get { return border; } set { border = value; } }

         public ClampMvCol(uint mvec, uint border)
         { 
			this.mvec = mvec;
			this.border = border;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint bw4 = 0;
			uint mbToLeftEdge = 0;
			uint mbToRightEdge = 0;
			bw4= Num_4x4_Blocks_Wide[ MiSize ];
			mbToLeftEdge= -((MiCol * MI_SIZE) * 8);
			mbToRightEdge= ((MiCols - bw4 - MiCol) * MI_SIZE) * 8;
return Clip3( mbToLeftEdge - border, mbToRightEdge + border, mvec );

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint bw4 = 0;
			uint mbToLeftEdge = 0;
			uint mbToRightEdge = 0;
			bw4= Num_4x4_Blocks_Wide[ MiSize ];
			mbToLeftEdge= -((MiCol * MI_SIZE) * 8);
			mbToRightEdge= ((MiCols - bw4 - MiCol) * MI_SIZE) * 8;
return Clip3( mbToLeftEdge - border, mbToRightEdge + border, mvec );

            return size;
         }

    }

    /*



clear_cdef( r, c ) { 
 cdef_idx[ r ][ c ] = -1
 if ( use_128x128_superblock ) {
 cdefSize4 = Num_4x4_Blocks_Wide[ BLOCK_64X64 ]
 cdef_idx[ r ][ c + cdefSize4 ] = -1
 cdef_idx[ r + cdefSize4][ c ] = -1
 cdef_idx[ r + cdefSize4][ c + cdefSize4 ] = -1
 }
 }
    */
    public class ClearCdef : IAomSerializable
    {
		private uint r;
		public uint r { get { return r; } set { r = value; } }
		private uint c;
		public uint c { get { return c; } set { c = value; } }

         public ClearCdef(uint r, uint c)
         { 
			this.r = r;
			this.c = c;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint[][] cdef_idx = null;
			uint cdefSize4 = 0;
			cdef_idx[ r ][ c ]= -1;

			if ( use_128x128_superblock != 0 )
			{
				cdefSize4= Num_4x4_Blocks_Wide[ BLOCK_64X64 ];
				cdef_idx[ r ][ c + cdefSize4 ]= -1;
				cdef_idx[ r + cdefSize4][ c ]= -1;
				cdef_idx[ r + cdefSize4][ c + cdefSize4 ]= -1;
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint[][] cdef_idx = null;
			uint cdefSize4 = 0;
			cdef_idx[ r ][ c ]= -1;

			if ( use_128x128_superblock != 0 )
			{
				cdefSize4= Num_4x4_Blocks_Wide[ BLOCK_64X64 ];
				cdef_idx[ r ][ c + cdefSize4 ]= -1;
				cdef_idx[ r + cdefSize4][ c ]= -1;
				cdef_idx[ r + cdefSize4][ c + cdefSize4 ]= -1;
			}

            return size;
         }

    }

    /*



read_cdef() { 
 if ( skip || CodedLossless || !enable_cdef || allow_intrabc) {
 return
 }
 cdefSize4 = Num_4x4_Blocks_Wide[ BLOCK_64X64 ]
 cdefMask4 = ~(cdefSize4 - 1)
 r = MiRow & cdefMask4
 c = MiCol & cdefMask4
 if ( cdef_idx[ r ][ c ] == -1 ) {
  cdef_idx[ r ][ c ] L(cdef_bits)
 w4 = Num_4x4_Blocks_Wide[ MiSize ]
 h4 = Num_4x4_Blocks_High[ MiSize ]
 for ( i = r; i < r + h4 ; i += cdefSize4 ) {
 for ( j = c; j < c + w4 ; j += cdefSize4 ) {
 cdef_idx[ i ][ j ] = cdef_idx[ r ][ c ]
 }
 }
 }
 }
    */
    public class ReadCdef : IAomSerializable
    {
		private uint[][] cdef_idx;
		public uint[][] CdefIdx { get { return cdef_idx; } set { cdef_idx = value; } }

         public ReadCdef()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint cdefSize4 = 0;
			uint cdefMask4 = 0;
			uint r = 0;
			uint c = 0;
			uint w4 = 0;
			uint h4 = 0;
			uint i = 0;
			uint j = 0;
			uint[][] cdef_idx = null;

			if ( skip != 0 || CodedLossless != 0 || enable_cdef== 0 || allow_intrabc != 0)
			{
return;
			}
			cdefSize4= Num_4x4_Blocks_Wide[ BLOCK_64X64 ];
			cdefMask4= ~(cdefSize4 - 1);
			r= MiRow & cdefMask4;
			c= MiCol & cdefMask4;

			if ( cdef_idx[ r ][ c ] == -1 )
			{
				size += stream.ReadL(size, cdef_bits, out this.cdef_idx[ r ][ c ], "cdef_idx"); 
				w4= Num_4x4_Blocks_Wide[ MiSize ];
				h4= Num_4x4_Blocks_High[ MiSize ];

				for ( i = r; i < r + h4 ; i += cdefSize4 )
				{

					for ( j = c; j < c + w4 ; j += cdefSize4 )
					{
						cdef_idx[ i ][ j ]= cdef_idx[ r ][ c ];
					}
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint cdefSize4 = 0;
			uint cdefMask4 = 0;
			uint r = 0;
			uint c = 0;
			uint w4 = 0;
			uint h4 = 0;
			uint i = 0;
			uint j = 0;
			uint[][] cdef_idx = null;

			if ( skip != 0 || CodedLossless != 0 || enable_cdef== 0 || allow_intrabc != 0)
			{
return;
			}
			cdefSize4= Num_4x4_Blocks_Wide[ BLOCK_64X64 ];
			cdefMask4= ~(cdefSize4 - 1);
			r= MiRow & cdefMask4;
			c= MiCol & cdefMask4;

			if ( cdef_idx[ r ][ c ] == -1 )
			{
				size += stream.WriteL(cdef_bits,  this.cdef_idx[ r ][ c ], "cdef_idx"); 
				w4= Num_4x4_Blocks_Wide[ MiSize ];
				h4= Num_4x4_Blocks_High[ MiSize ];

				for ( i = r; i < r + h4 ; i += cdefSize4 )
				{

					for ( j = c; j < c + w4 ; j += cdefSize4 )
					{
						cdef_idx[ i ][ j ]= cdef_idx[ r ][ c ];
					}
				}
			}

            return size;
         }

    }

    /*



read_lr( r, c, bSize ) { 
 if ( allow_intrabc ) {
 return
 }
 w = Num_4x4_Blocks_Wide[ bSize ]
 h = Num_4x4_Blocks_High[ bSize ]
 for ( plane = 0; plane < NumPlanes; plane++ ) {
 if ( FrameRestorationType[ plane ] != RESTORE_NONE ) {
 subX = (plane == 0) ? 0 : subsampling_x
 subY = (plane == 0) ? 0 : subsampling_y
 unitSize = LoopRestorationSize[ plane ]
 unitRows = count_units_in_frame( unitSize, Round2( FrameHeight, subY) )
 unitCols = count_units_in_frame( unitSize, Round2( UpscaledWidth, subX) )
 unitRowStart = ( r * ( MI_SIZE >> subY) + unitSize - 1 ) / unitSize
 unitRowEnd = Min( unitRows, ( (r + h) * ( MI_SIZE >> subY) + unitSize - 1 ) / unitSize)
 if ( use_superres ) {
 numerator = (MI_SIZE >> subX) * SuperresDenom
 denominator = unitSize * SUPERRES_NUM
 } else {
 numerator = MI_SIZE >> subX
 denominator = unitSize
 }
 unitColStart = ( c * numerator + denominator - 1 ) / denominator
 unitColEnd = Min( unitCols, ( (c + w) * numerator + denominator - 1 ) / denominator)
 for ( unitRow = unitRowStart; unitRow < unitRowEnd; unitRow++ ) {
 for ( unitCol = unitColStart; unitCol < unitColEnd; unitCol++ ) {
 read_lr_unit(plane, unitRow, unitCol)
 }
 }
 }
 }
 }
    */
    public class ReadLr : IAomSerializable
    {
		private uint r;
		public uint r { get { return r; } set { r = value; } }
		private uint c;
		public uint c { get { return c; } set { c = value; } }
		private uint bSize;
		public uint BSize { get { return bSize; } set { bSize = value; } }
		private ReadLrUnit[][][] read_lr_unit;
		public ReadLrUnit[][][] ReadLrUnit { get { return read_lr_unit; } set { read_lr_unit = value; } }

         public ReadLr(uint r, uint c, uint bSize)
         { 
			this.r = r;
			this.c = c;
			this.bSize = bSize;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint w = 0;
			uint h = 0;
			uint plane = 0;
			uint subX = 0;
			uint subY = 0;
			uint unitSize = 0;
			uint unitRows = 0;
			uint unitCols = 0;
			uint unitRowStart = 0;
			uint unitRowEnd = 0;
			uint numerator = 0;
			uint denominator = 0;
			uint unitColStart = 0;
			uint unitColEnd = 0;
			uint unitRow = 0;
			uint unitCol = 0;

			if ( allow_intrabc != 0 )
			{
return;
			}
			w= Num_4x4_Blocks_Wide[ bSize ];
			h= Num_4x4_Blocks_High[ bSize ];

			this.read_lr_unit = new ReadLrUnit[ NumPlanes][][];
			for ( plane = 0; plane < NumPlanes; plane++ )
			{

				if ( FrameRestorationType[ plane ] != RESTORE_NONE )
				{
					subX= (plane == 0) ? 0 : subsampling_x;
					subY= (plane == 0) ? 0 : subsampling_y;
					unitSize= LoopRestorationSize[ plane ];
					unitRows= count_units_in_frame( unitSize, Round2( FrameHeight, subY) );
					unitCols= count_units_in_frame( unitSize, Round2( UpscaledWidth, subX) );
					unitRowStart= ( r * ( MI_SIZE >> subY) + unitSize - 1 ) / unitSize;
					unitRowEnd= Math.Min( unitRows, ( (r + h) * ( MI_SIZE >> subY) + unitSize - 1 ) / unitSize);

					if ( use_superres != 0 )
					{
						numerator= (MI_SIZE >> subX) * SuperresDenom;
						denominator= unitSize * SUPERRES_NUM;
					}
					else 
					{
						numerator= MI_SIZE >> subX;
						denominator= unitSize;
					}
					unitColStart= ( c * numerator + denominator - 1 ) / denominator;
					unitColEnd= Math.Min( unitCols, ( (c + w) * numerator + denominator - 1 ) / denominator);

					this.read_lr_unit[ plane ] = new ReadLrUnit[ unitRowEnd][];
					for ( unitRow = unitRowStart; unitRow < unitRowEnd; unitRow++ )
					{

						this.read_lr_unit[ plane ][ unitRow ] = new ReadLrUnit[ unitColEnd];
						for ( unitCol = unitColStart; unitCol < unitColEnd; unitCol++ )
						{
							this.read_lr_unit[ plane ][ unitRow ][ unitCol ] =  new ReadLrUnit(plane,  unitRow,  unitCol) ;
							size +=  stream.ReadClass<ReadLrUnit>(size, context, this.read_lr_unit[ plane ][ unitRow ][ unitCol ], "read_lr_unit"); 
						}
					}
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint w = 0;
			uint h = 0;
			uint plane = 0;
			uint subX = 0;
			uint subY = 0;
			uint unitSize = 0;
			uint unitRows = 0;
			uint unitCols = 0;
			uint unitRowStart = 0;
			uint unitRowEnd = 0;
			uint numerator = 0;
			uint denominator = 0;
			uint unitColStart = 0;
			uint unitColEnd = 0;
			uint unitRow = 0;
			uint unitCol = 0;

			if ( allow_intrabc != 0 )
			{
return;
			}
			w= Num_4x4_Blocks_Wide[ bSize ];
			h= Num_4x4_Blocks_High[ bSize ];

			for ( plane = 0; plane < NumPlanes; plane++ )
			{

				if ( FrameRestorationType[ plane ] != RESTORE_NONE )
				{
					subX= (plane == 0) ? 0 : subsampling_x;
					subY= (plane == 0) ? 0 : subsampling_y;
					unitSize= LoopRestorationSize[ plane ];
					unitRows= count_units_in_frame( unitSize, Round2( FrameHeight, subY) );
					unitCols= count_units_in_frame( unitSize, Round2( UpscaledWidth, subX) );
					unitRowStart= ( r * ( MI_SIZE >> subY) + unitSize - 1 ) / unitSize;
					unitRowEnd= Math.Min( unitRows, ( (r + h) * ( MI_SIZE >> subY) + unitSize - 1 ) / unitSize);

					if ( use_superres != 0 )
					{
						numerator= (MI_SIZE >> subX) * SuperresDenom;
						denominator= unitSize * SUPERRES_NUM;
					}
					else 
					{
						numerator= MI_SIZE >> subX;
						denominator= unitSize;
					}
					unitColStart= ( c * numerator + denominator - 1 ) / denominator;
					unitColEnd= Math.Min( unitCols, ( (c + w) * numerator + denominator - 1 ) / denominator);

					for ( unitRow = unitRowStart; unitRow < unitRowEnd; unitRow++ )
					{

						for ( unitCol = unitColStart; unitCol < unitColEnd; unitCol++ )
						{
							size += stream.WriteClass<ReadLrUnit>(context, this.read_lr_unit[ plane ][ unitRow ][ unitCol ], "read_lr_unit"); 
						}
					}
				}
			}

            return size;
         }

    }

    /*



read_lr_unit(plane, unitRow, unitCol) { 
 if ( FrameRestorationType[ plane ] == RESTORE_WIENER ) {
 use_wiener  S()
 restoration_type = use_wiener ? RESTORE_WIENER : RESTORE_NONE
 } else if ( FrameRestorationType[ plane ] == RESTORE_SGRPROJ ) {
 use_sgrproj  S()
 restoration_type = use_sgrproj ? RESTORE_SGRPROJ : RESTORE_NONE
 } else {
 restoration_type  S()
 }
 LrType[ plane ][ unitRow ][ unitCol ] = restoration_type
 if ( restoration_type == RESTORE_WIENER ) {
 for ( pass = 0; pass < 2; pass++ ) {
 if ( plane ) {
 firstCoeff = 1
 LrWiener[ plane ]
 [ unitRow ][ unitCol ][ pass ][0] = 0
 } else {
 firstCoeff = 0
 }
 for ( j = firstCoeff; j < 3; j++ ) {
 min = Wiener_Taps_Min[ j ]
 max = Wiener_Taps_Max[ j ]
 k = Wiener_Taps_K[ j ]
 v = decode_signed_subexp_with_ref_bool( min, max + 1, k, RefLrWiener[ plane ][ pass ][ j ] )
 LrWiener[ plane ]
 [ unitRow ][ unitCol ][ pass ][ j ] = v
 RefLrWiener[ plane ][ pass ][ j ] = v
 }
 }
 } else if ( restoration_type == RESTORE_SGRPROJ ) {
 lr_sgr_set L(SGRPROJ_PARAMS_BITS)
 LrSgrSet[ plane ][ unitRow ][ unitCol ] = lr_sgr_set
 for ( i = 0; i < 2; i++ ) {
 radius = Sgr_Params[ lr_sgr_set ][ i * 2 ]
 min = Sgrproj_Xqd_Min[i]
 max = Sgrproj_Xqd_Max[i]
 if ( radius ) {
 v = decode_signed_subexp_with_ref_bool( min, max + 1, SGRPROJ_PRJ_SUBEXP_K, RefSgrXqd[ plane ][ i ])
 } else {
 v = 0
 if ( i == 1 ) {
 v = Clip3( min, max, (1 << SGRPROJ_PRJ_BITS) RefSgrXqd[ plane ][ 0 ] )
 }
 }
 LrSgrXqd[ plane ][ unitRow ][ unitCol ][ i ] = v
 RefSgrXqd[ plane ][ i ] = v
 }
 }
 }
    */
    public class ReadLrUnit : IAomSerializable
    {
		private uint plane;
		public uint Plane { get { return plane; } set { plane = value; } }
		private uint unitRow;
		public uint UnitRow { get { return unitRow; } set { unitRow = value; } }
		private uint unitCol;
		public uint UnitCol { get { return unitCol; } set { unitCol = value; } }
		private uint use_wiener;
		public uint UseWiener { get { return use_wiener; } set { use_wiener = value; } }
		private uint use_sgrproj;
		public uint UseSgrproj { get { return use_sgrproj; } set { use_sgrproj = value; } }
		private uint restoration_type;
		public uint RestorationType { get { return restoration_type; } set { restoration_type = value; } }
		private uint lr_sgr_set;
		public uint LrSgrSet { get { return lr_sgr_set; } set { lr_sgr_set = value; } }

         public ReadLrUnit(uint plane, uint unitRow, uint unitCol)
         { 
			this.plane = plane;
			this.unitRow = unitRow;
			this.unitCol = unitCol;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint restoration_type = 0;
			uint[][][] LrType = null;
			uint pass = 0;
			uint firstCoeff = 0;
			uint[][][][][] LrWiener = null;
			uint j = 0;
			uint min = 0;
			uint max = 0;
			uint k = 0;
			uint v = 0;
			uint[][][] RefLrWiener = null;
			uint[][][] LrSgrSet = null;
			uint i = 0;
			uint radius = 0;
			uint[][][][] LrSgrXqd = null;
			uint[][] RefSgrXqd = null;

			if ( FrameRestorationType[ plane ] == RESTORE_WIENER )
			{
				size += stream.ReadS(size, out this.use_wiener, "use_wiener"); 
				restoration_type= use_wiener ? RESTORE_WIENER : RESTORE_NONE;
			}
			else if ( FrameRestorationType[ plane ] == RESTORE_SGRPROJ )
			{
				size += stream.ReadS(size, out this.use_sgrproj, "use_sgrproj"); 
				restoration_type= use_sgrproj ? RESTORE_SGRPROJ : RESTORE_NONE;
			}
			else 
			{
				size += stream.ReadS(size, out this.restoration_type, "restoration_type"); 
			}
			LrType[ plane ][ unitRow ][ unitCol ]= restoration_type;

			if ( restoration_type == RESTORE_WIENER )
			{

				for ( pass = 0; pass < 2; pass++ )
				{

					if ( plane != 0 )
					{
						firstCoeff= 1;
						LrWiener[ plane ][ unitRow ][ unitCol ][ pass ][0]= 0;
					}
					else 
					{
						firstCoeff= 0;
					}

					for ( j = firstCoeff; j < 3; j++ )
					{
						min= Wiener_Taps_Min[ j ];
						max= Wiener_Taps_Max[ j ];
						k= Wiener_Taps_K[ j ];
						v= decode_signed_subexp_with_ref_bool( min, max + 1, k, RefLrWiener[ plane ][ pass ][ j ] );
						LrWiener[ plane ][ unitRow ][ unitCol ][ pass ][ j ]= v;
						RefLrWiener[ plane ][ pass ][ j ]= v;
					}
				}
			}
			else if ( restoration_type == RESTORE_SGRPROJ )
			{
				size += stream.ReadL(size, SGRPROJ_PARAMS_BITS, out this.lr_sgr_set, "lr_sgr_set"); 
				LrSgrSet[ plane ][ unitRow ][ unitCol ]= lr_sgr_set;

				for ( i = 0; i < 2; i++ )
				{
					radius= Sgr_Params[ lr_sgr_set ][ i * 2 ];
					min= Sgrproj_Xqd_Min[i];
					max= Sgrproj_Xqd_Max[i];

					if ( radius != 0 )
					{
						v= decode_signed_subexp_with_ref_bool( min, max + 1, SGRPROJ_PRJ_SUBEXP_K, RefSgrXqd[ plane ][ i ]);
					}
					else 
					{
						v= 0;

						if ( i == 1 )
						{
							v= Clip3( min, max, (1 << SGRPROJ_PRJ_BITS) RefSgrXqd[ plane ][ 0 ] );
						}
					}
					LrSgrXqd[ plane ][ unitRow ][ unitCol ][ i ]= v;
					RefSgrXqd[ plane ][ i ]= v;
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint restoration_type = 0;
			uint[][][] LrType = null;
			uint pass = 0;
			uint firstCoeff = 0;
			uint[][][][][] LrWiener = null;
			uint j = 0;
			uint min = 0;
			uint max = 0;
			uint k = 0;
			uint v = 0;
			uint[][][] RefLrWiener = null;
			uint[][][] LrSgrSet = null;
			uint i = 0;
			uint radius = 0;
			uint[][][][] LrSgrXqd = null;
			uint[][] RefSgrXqd = null;

			if ( FrameRestorationType[ plane ] == RESTORE_WIENER )
			{
				size += stream.WriteS( this.use_wiener, "use_wiener"); 
				restoration_type= use_wiener ? RESTORE_WIENER : RESTORE_NONE;
			}
			else if ( FrameRestorationType[ plane ] == RESTORE_SGRPROJ )
			{
				size += stream.WriteS( this.use_sgrproj, "use_sgrproj"); 
				restoration_type= use_sgrproj ? RESTORE_SGRPROJ : RESTORE_NONE;
			}
			else 
			{
				size += stream.WriteS( this.restoration_type, "restoration_type"); 
			}
			LrType[ plane ][ unitRow ][ unitCol ]= restoration_type;

			if ( restoration_type == RESTORE_WIENER )
			{

				for ( pass = 0; pass < 2; pass++ )
				{

					if ( plane != 0 )
					{
						firstCoeff= 1;
						LrWiener[ plane ][ unitRow ][ unitCol ][ pass ][0]= 0;
					}
					else 
					{
						firstCoeff= 0;
					}

					for ( j = firstCoeff; j < 3; j++ )
					{
						min= Wiener_Taps_Min[ j ];
						max= Wiener_Taps_Max[ j ];
						k= Wiener_Taps_K[ j ];
						v= decode_signed_subexp_with_ref_bool( min, max + 1, k, RefLrWiener[ plane ][ pass ][ j ] );
						LrWiener[ plane ][ unitRow ][ unitCol ][ pass ][ j ]= v;
						RefLrWiener[ plane ][ pass ][ j ]= v;
					}
				}
			}
			else if ( restoration_type == RESTORE_SGRPROJ )
			{
				size += stream.WriteL(SGRPROJ_PARAMS_BITS,  this.lr_sgr_set, "lr_sgr_set"); 
				LrSgrSet[ plane ][ unitRow ][ unitCol ]= lr_sgr_set;

				for ( i = 0; i < 2; i++ )
				{
					radius= Sgr_Params[ lr_sgr_set ][ i * 2 ];
					min= Sgrproj_Xqd_Min[i];
					max= Sgrproj_Xqd_Max[i];

					if ( radius != 0 )
					{
						v= decode_signed_subexp_with_ref_bool( min, max + 1, SGRPROJ_PRJ_SUBEXP_K, RefSgrXqd[ plane ][ i ]);
					}
					else 
					{
						v= 0;

						if ( i == 1 )
						{
							v= Clip3( min, max, (1 << SGRPROJ_PRJ_BITS) RefSgrXqd[ plane ][ 0 ] );
						}
					}
					LrSgrXqd[ plane ][ unitRow ][ unitCol ][ i ]= v;
					RefSgrXqd[ plane ][ i ]= v;
				}
			}

            return size;
         }

    }

    /*



decode_signed_subexp_with_ref_bool( low, high, k, r ) { 
 x = decode_unsigned_subexp_with_ref_bool(high - low, k, r - low)
 return x + low
 }
    */
    public class DecodeSignedSubexpWithRefBool : IAomSerializable
    {
		private uint low;
		public uint Low { get { return low; } set { low = value; } }
		private uint high;
		public uint High { get { return high; } set { high = value; } }
		private uint k;
		public uint k { get { return k; } set { k = value; } }
		private uint r;
		public uint r { get { return r; } set { r = value; } }

         public DecodeSignedSubexpWithRefBool(uint low, uint high, uint k, uint r)
         { 
			this.low = low;
			this.high = high;
			this.k = k;
			this.r = r;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint x = 0;
			x= decode_unsigned_subexp_with_ref_bool(high - low, k, r - low);
return x + low;

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint x = 0;
			x= decode_unsigned_subexp_with_ref_bool(high - low, k, r - low);
return x + low;

            return size;
         }

    }

    /*

 decode_unsigned_subexp_with_ref_bool( mx, k, r ) {
 v = decode_subexp_bool( mx, k )
 if ( (r << 1) <= mx ) {
 return inverse_recenter(r, v)
 } else {
 return mx - 1 - inverse_recenter(mx - 1 - r, v)
 }
 }
    */
    public class DecodeUnsignedSubexpWithRefBool : IAomSerializable
    {
		private uint mx;
		public uint Mx { get { return mx; } set { mx = value; } }
		private uint k;
		public uint k { get { return k; } set { k = value; } }
		private uint r;
		public uint r { get { return r; } set { r = value; } }

         public DecodeUnsignedSubexpWithRefBool(uint mx, uint k, uint r)
         { 
			this.mx = mx;
			this.k = k;
			this.r = r;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint v = 0;
			v= decode_subexp_bool( mx, k );

			if ( (r << (int) 1) <= mx )
			{
return inverse_recenter(r, v);
			}
			else 
			{
return mx - 1 - inverse_recenter(mx - 1 - r, v);
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint v = 0;
			v= decode_subexp_bool( mx, k );

			if ( (r << (int) 1) <= mx )
			{
return inverse_recenter(r, v);
			}
			else 
			{
return mx - 1 - inverse_recenter(mx - 1 - r, v);
			}

            return size;
         }

    }

    /*

 decode_subexp_bool( numSyms, k ) {
 i = 0
 mk = 0
 while ( 1 ) {
 b2 = i ? k + i - 1 : k
 a = 1 << b2
 if ( numSyms <= mk + 3 * a ) {
 subexp_unif_bools  NS(numSyms - mk)
 return subexp_unif_bools + mk
 } else {
 subexp_more_bools  L(1)
 if ( subexp_more_bools ) {
 i++
 mk += a
 } else {
 subexp_bools  L(b2)
 return subexp_bools + mk
 }
 }
 }
 }
    */
    public class DecodeSubexpBool : IAomSerializable
    {
		private uint numSyms;
		public uint NumSyms { get { return numSyms; } set { numSyms = value; } }
		private uint k;
		public uint k { get { return k; } set { k = value; } }
		private Dictionary<int, uint> subexp_unif_bools = new Dictionary<int, uint>();
		public Dictionary<int, uint> SubexpUnifBools { get { return subexp_unif_bools; } set { subexp_unif_bools = value; } }
		private Dictionary<int, uint> subexp_more_bools = new Dictionary<int, uint>();
		public Dictionary<int, uint> SubexpMoreBools { get { return subexp_more_bools; } set { subexp_more_bools = value; } }
		private Dictionary<int, uint> subexp_bools = new Dictionary<int, uint>();
		public Dictionary<int, uint> SubexpBools { get { return subexp_bools; } set { subexp_bools = value; } }

         public DecodeSubexpBool(uint numSyms, uint k)
         { 
			this.numSyms = numSyms;
			this.k = k;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint i = 0;
			uint mk = 0;
			int whileIndex = -1;
			uint b2 = 0;
			uint a = 0;
			i= 0;
			mk= 0;

			while ( 1 != 0 )
			{
				whileIndex++;

				b2= i ? k + i - 1 : k;
				a= 1 << b2;

				if ( numSyms <= mk + 3 * a )
				{
					size += stream.ReadUnsignedInt(size, numSyms - mk, whileIndex, this.subexp_unif_bools, "subexp_unif_bools"); 
return subexp_unif_bools + mk;
				}
				else 
				{
					size += stream.ReadL(size, 1, whileIndex, this.subexp_more_bools, "subexp_more_bools"); 

					if ( subexp_more_bools[whileIndex] != 0 )
					{
						i++;
						mk+= a;
					}
					else 
					{
						size += stream.ReadL(size, b2, whileIndex, this.subexp_bools, "subexp_bools"); 
return subexp_bools + mk;
					}
				}
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint i = 0;
			uint mk = 0;
			int whileIndex = -1;
			uint b2 = 0;
			uint a = 0;
			i= 0;
			mk= 0;

			while ( 1 != 0 )
			{
				whileIndex++;

				b2= i ? k + i - 1 : k;
				a= 1 << b2;

				if ( numSyms <= mk + 3 * a )
				{
					size += stream.WriteUnsignedInt(numSyms - mk, whileIndex, this.subexp_unif_bools, "subexp_unif_bools"); 
return subexp_unif_bools + mk;
				}
				else 
				{
					size += stream.WriteL(1,  whileIndex, this.subexp_more_bools, "subexp_more_bools"); 

					if ( subexp_more_bools[whileIndex] != 0 )
					{
						i++;
						mk+= a;
					}
					else 
					{
						size += stream.WriteL(b2,  whileIndex, this.subexp_bools, "subexp_bools"); 
return subexp_bools + mk;
					}
				}
			}

            return size;
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
    public class TileListObu : IAomSerializable
    {
		private uint output_frame_width_in_tiles_minus_1;
		public uint OutputFrameWidthInTilesMinus1 { get { return output_frame_width_in_tiles_minus_1; } set { output_frame_width_in_tiles_minus_1 = value; } }
		private uint output_frame_height_in_tiles_minus_1;
		public uint OutputFrameHeightInTilesMinus1 { get { return output_frame_height_in_tiles_minus_1; } set { output_frame_height_in_tiles_minus_1 = value; } }
		private uint tile_count_minus_1;
		public uint TileCountMinus1 { get { return tile_count_minus_1; } set { tile_count_minus_1 = value; } }
		private TileListEntry[] tile_list_entry;
		public TileListEntry[] TileListEntry { get { return tile_list_entry; } set { tile_list_entry = value; } }

         public TileListObu()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint tile = 0;
			size += stream.ReadFixed(size, 8, out this.output_frame_width_in_tiles_minus_1, "output_frame_width_in_tiles_minus_1"); 
			size += stream.ReadFixed(size, 8, out this.output_frame_height_in_tiles_minus_1, "output_frame_height_in_tiles_minus_1"); 
			size += stream.ReadFixed(size, 16, out this.tile_count_minus_1, "tile_count_minus_1"); 

			this.tile_list_entry = new TileListEntry[ tile_count_minus_1];
			for ( tile = 0; tile <= tile_count_minus_1; tile++ )
			{
				this.tile_list_entry[ tile ] =  new TileListEntry() ;
				size +=  stream.ReadClass<TileListEntry>(size, context, this.tile_list_entry[ tile ], "tile_list_entry"); 
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint tile = 0;
			size += stream.WriteFixed(8, this.output_frame_width_in_tiles_minus_1, "output_frame_width_in_tiles_minus_1"); 
			size += stream.WriteFixed(8, this.output_frame_height_in_tiles_minus_1, "output_frame_height_in_tiles_minus_1"); 
			size += stream.WriteFixed(16, this.tile_count_minus_1, "tile_count_minus_1"); 

			for ( tile = 0; tile <= tile_count_minus_1; tile++ )
			{
				size += stream.WriteClass<TileListEntry>(context, this.tile_list_entry[ tile ], "tile_list_entry"); 
			}

            return size;
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
    public class TileListEntry : IAomSerializable
    {
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

         public TileListEntry()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint N = 0;
			size += stream.ReadFixed(size, 8, out this.anchor_frame_idx, "anchor_frame_idx"); 
			size += stream.ReadFixed(size, 8, out this.anchor_tile_row, "anchor_tile_row"); 
			size += stream.ReadFixed(size, 8, out this.anchor_tile_col, "anchor_tile_col"); 
			size += stream.ReadFixed(size, 16, out this.tile_data_size_minus_1, "tile_data_size_minus_1"); 
			N= 8 * (tile_data_size_minus_1 + 1);
			size += stream.ReadVariable(size, N, out this.coded_tile_data, "coded_tile_data"); 

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			uint N = 0;
			size += stream.WriteFixed(8, this.anchor_frame_idx, "anchor_frame_idx"); 
			size += stream.WriteFixed(8, this.anchor_tile_row, "anchor_tile_row"); 
			size += stream.WriteFixed(8, this.anchor_tile_col, "anchor_tile_col"); 
			size += stream.WriteFixed(16, this.tile_data_size_minus_1, "tile_data_size_minus_1"); 
			N= 8 * (tile_data_size_minus_1 + 1);
			size += stream.WriteVariable(N, this.coded_tile_data, "coded_tile_data"); 

            return size;
         }

    }

    /*



bitstream() { 
 while ( more_data_in_bitstream() ) {
  temporal_unit_size leb128()
 temporal_unit( temporal_unit_size )
 }
 }
    */
    public class Bitstream : IAomSerializable
    {
		private Dictionary<int, uint> temporal_unit_size = new Dictionary<int, uint>();
		public Dictionary<int, uint> TemporalUnitSize { get { return temporal_unit_size; } set { temporal_unit_size = value; } }
		private Dictionary<int, TemporalUnit> temporal_unit = new Dictionary<int, TemporalUnit>();
		public Dictionary<int, TemporalUnit> TemporalUnit { get { return temporal_unit; } set { temporal_unit = value; } }

         public Bitstream()
         { 

         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			int whileIndex = -1;

			while ( more_data_in_bitstream() )
			{
				whileIndex++;

				size += stream.ReadLeb128(size,  whileIndex, this.temporal_unit_size, "temporal_unit_size"); 
				this.temporal_unit.Add(whileIndex,  new TemporalUnit( temporal_unit_size ) );
				size +=  stream.ReadClass<TemporalUnit>(size, context, this.temporal_unit[whileIndex], "temporal_unit"); 
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			int whileIndex = -1;

			while ( more_data_in_bitstream() )
			{
				whileIndex++;

				size += stream.WriteLeb128( whileIndex, this.temporal_unit_size, "temporal_unit_size"); 
				size += stream.WriteClass<TemporalUnit>(context, whileIndex, this.temporal_unit, "temporal_unit"); 
			}

            return size;
         }

    }

    /*



temporal_unit( sz ) { 
 while ( sz > 0 ) {
  frame_unit_size leb128()
 sz -= Leb128Bytes
 frame_unit( frame_unit_size )
 sz -= frame_unit_size
 }
 }
    */
    public class TemporalUnit : IAomSerializable
    {
		private uint sz;
		public uint Sz { get { return sz; } set { sz = value; } }
		private Dictionary<int, uint> frame_unit_size = new Dictionary<int, uint>();
		public Dictionary<int, uint> FrameUnitSize { get { return frame_unit_size; } set { frame_unit_size = value; } }
		private Dictionary<int, FrameUnit> frame_unit = new Dictionary<int, FrameUnit>();
		public Dictionary<int, FrameUnit> FrameUnit { get { return frame_unit; } set { frame_unit = value; } }

         public TemporalUnit(uint sz)
         { 
			this.sz = sz;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			int whileIndex = -1;

			while ( sz > 0 )
			{
				whileIndex++;

				size += stream.ReadLeb128(size,  whileIndex, this.frame_unit_size, "frame_unit_size"); 
				sz-= Leb128Bytes;
				this.frame_unit.Add(whileIndex,  new FrameUnit( frame_unit_size ) );
				size +=  stream.ReadClass<FrameUnit>(size, context, this.frame_unit[whileIndex], "frame_unit"); 
				sz-= frame_unit[whileIndex]_size[whileIndex];
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			int whileIndex = -1;

			while ( sz > 0 )
			{
				whileIndex++;

				size += stream.WriteLeb128( whileIndex, this.frame_unit_size, "frame_unit_size"); 
				sz-= Leb128Bytes;
				size += stream.WriteClass<FrameUnit>(context, whileIndex, this.frame_unit, "frame_unit"); 
				sz-= frame_unit[whileIndex]_size[whileIndex];
			}

            return size;
         }

    }

    /*



frame_unit( sz ) { 
 while ( sz > 0 ) {
  obu_length leb128()
 sz -= Leb128Bytes
 open_bitstream_unit( obu_length )
 sz -= obu_length
 }
 }
    */
    public class FrameUnit : IAomSerializable
    {
		private uint sz;
		public uint Sz { get { return sz; } set { sz = value; } }
		private Dictionary<int, uint> obu_length = new Dictionary<int, uint>();
		public Dictionary<int, uint> ObuLength { get { return obu_length; } set { obu_length = value; } }
		private Dictionary<int, OpenBitstreamUnit> open_bitstream_unit = new Dictionary<int, OpenBitstreamUnit>();
		public Dictionary<int, OpenBitstreamUnit> OpenBitstreamUnit { get { return open_bitstream_unit; } set { open_bitstream_unit = value; } }

         public FrameUnit(uint sz)
         { 
			this.sz = sz;
         }

         public ulong Read(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			int whileIndex = -1;

			while ( sz > 0 )
			{
				whileIndex++;

				size += stream.ReadLeb128(size,  whileIndex, this.obu_length, "obu_length"); 
				sz-= Leb128Bytes;
				this.open_bitstream_unit.Add(whileIndex,  new OpenBitstreamUnit( obu_length ) );
				size +=  stream.ReadClass<OpenBitstreamUnit>(size, context, this.open_bitstream_unit[whileIndex], "open_bitstream_unit"); 
				sz-= obu_length[whileIndex];
			}

            return size;
         }

         public ulong Write(IAomContext context, AomStream stream)
         {
            ulong size = 0;

			int whileIndex = -1;

			while ( sz > 0 )
			{
				whileIndex++;

				size += stream.WriteLeb128( whileIndex, this.obu_length, "obu_length"); 
				sz-= Leb128Bytes;
				size += stream.WriteClass<OpenBitstreamUnit>(context, whileIndex, this.open_bitstream_unit, "open_bitstream_unit"); 
				sz-= obu_length[whileIndex];
			}

            return size;
         }

    }

}
