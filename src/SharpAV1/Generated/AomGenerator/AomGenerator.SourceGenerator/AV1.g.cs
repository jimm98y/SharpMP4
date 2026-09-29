using System;
using System.Collections.Generic;
using System.Numerics;
using SharpAVX;
using static SharpAV1.AV1Constants;
using static SharpAV1.AV1RefFrames;
using static SharpAV1.AV1ObuTypes;
using static SharpAV1.AV1ColorPrimaries;
using static SharpAV1.AV1TransferCharacteristics;
using static SharpAV1.AV1MatrixCoefficients;
using static SharpAV1.AV1ChromaSamplePosition;
using static SharpAV1.AV1FrameTypes;
using static SharpAV1.AV1MetadataType;
using static SharpAV1.AV1FrameRestorationType;
using static SharpAV1.AV1ScalabilityModeIdc;
using static SharpAV1.AV1TxModes;
using static SharpAV1.AV1InterpolationFilter;

namespace SharpAV1
{

    public partial class AV1Context : IAomContext
    {
        /// <summary>
        /// Writing, the state the OBU's syntax elements were read into, and the same state as it was
        /// changed: an element whose value the two give alike is written as it was read.
        /// </summary>
        private AV1Context _original;
        private AV1Context _edited;

    /*
open_bitstream_unit( sz ) { 
    obu_header()
    if ( obu_has_size_field ) {
       obu_size leb128()
    } else {
       obu_size = sz - 1 - obu_extension_flag
    }
    startPosition = get_position()
    if ( obu_type != OBU_SEQUENCE_HEADER && obu_type != OBU_TEMPORAL_DELIMITER && OperatingPointIdc != 0 && AllLayers == 0 && obu_extension_flag == 1 ) {
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
 if ( obu_size > 0 && obu_type != OBU_TILE_GROUP && obu_type != OBU_TILE_LIST && obu_type != OBU_FRAME ) {
    trailing_bits( obu_size * 8 - payloadBits )
 }
}
    */
		private int sz;
		public int _Sz { get { return sz; } set { sz = value; } }
		private int obu_size;
		public int _ObuSize { get { return obu_size; } set { obu_size = value; } }
		private int startPosition;
		public int _StartPosition { get { return startPosition; } set { startPosition = value; } }

        private void OpenBitstreamUnit(int sz)
        {
			int inTemporalLayer = 0;
			int inSpatialLayer = 0;
			int currentPosition = 0;
			int payloadBits = 0;
			ObuHeader(); 

			if ((obu_has_size_field != 0))
			{
				obu_size_len = (int)stream.ReadLeb128( out this.obu_size, "obu_size"); 
			}
			else 
			{
				obu_size = ((sz - 1) - obu_extension_flag);
			}
			startPosition = get_position();

			if ((((((obu_type != OBU_SEQUENCE_HEADER) && (obu_type != OBU_TEMPORAL_DELIMITER)) && (OperatingPointIdc != 0)) && (AllLayers == 0)) && (obu_extension_flag == 1)))
			{
				inTemporalLayer = ((OperatingPointIdc >> temporal_id) & 1);
				inSpatialLayer = ((OperatingPointIdc >> (spatial_id + 8)) & 1);

				if ((!(inTemporalLayer != 0) || !(inSpatialLayer != 0)))
				{
					drop_obu(); 
					return;
				}
			}

			if ((obu_type == OBU_SEQUENCE_HEADER))
			{
				SequenceHeaderObu(); 
			}
			else if ((obu_type == OBU_TEMPORAL_DELIMITER))
			{
				TemporalDelimiterObu(); 
			}
			else if ((obu_type == OBU_FRAME_HEADER))
			{
				FrameHeaderObu(); 
			}
			else if ((obu_type == OBU_REDUNDANT_FRAME_HEADER))
			{
				FrameHeaderObu(); 
			}
			else if ((obu_type == OBU_TILE_GROUP))
			{
				TileGroupObu(obu_size); 
			}
			else if ((obu_type == OBU_METADATA))
			{
				MetadataObu(); 
			}
			else if ((obu_type == OBU_FRAME))
			{
				FrameObu(obu_size); 
			}
			else if ((obu_type == OBU_TILE_LIST))
			{
				TileListObu(); 
			}
			else if ((obu_type == OBU_PADDING))
			{
				PaddingObu(); 
			}
			else 
			{
				ReservedObu(); 
			}
			currentPosition = get_position();
			payloadBits = (currentPosition - startPosition);

			if (((((obu_size > 0) && (obu_type != OBU_TILE_GROUP)) && (obu_type != OBU_TILE_LIST)) && (obu_type != OBU_FRAME)))
			{
				TrailingBits(((obu_size * 8) - payloadBits)); 
			}
        }

        private void WriteOpenBitstreamUnit(int sz)
        {
			int inTemporalLayer = 0;
			int inSpatialLayer = 0;
			int currentPosition = 0;
			int payloadBits = 0;
			WriteObuHeader(); 

			if ((obu_has_size_field != 0))
			{
				this.obu_size = stream.Pick("obu_size", _original != null ? _original.obu_size : this.obu_size, _edited != null ? _edited.obu_size : _original != null ? _original.obu_size : this.obu_size);
				obu_size_len = (int)stream.WriteLeb128( this.obu_size, "obu_size"); 
			}
			else 
			{
				obu_size = ((sz - 1) - obu_extension_flag);
			}
			startPosition = get_position();

			if ((((((obu_type != OBU_SEQUENCE_HEADER) && (obu_type != OBU_TEMPORAL_DELIMITER)) && (OperatingPointIdc != 0)) && (AllLayers == 0)) && (obu_extension_flag == 1)))
			{
				inTemporalLayer = ((OperatingPointIdc >> temporal_id) & 1);
				inSpatialLayer = ((OperatingPointIdc >> (spatial_id + 8)) & 1);

				if ((!(inTemporalLayer != 0) || !(inSpatialLayer != 0)))
				{
					drop_obu(); 
					return;
				}
			}

			if ((obu_type == OBU_SEQUENCE_HEADER))
			{
				WriteSequenceHeaderObu(); 
			}
			else if ((obu_type == OBU_TEMPORAL_DELIMITER))
			{
				TemporalDelimiterObu(); 
			}
			else if ((obu_type == OBU_FRAME_HEADER))
			{
				WriteFrameHeaderObu(); 
			}
			else if ((obu_type == OBU_REDUNDANT_FRAME_HEADER))
			{
				WriteFrameHeaderObu(); 
			}
			else if ((obu_type == OBU_TILE_GROUP))
			{
				WriteTileGroupObu(obu_size); 
			}
			else if ((obu_type == OBU_METADATA))
			{
				WriteMetadataObu(); 
			}
			else if ((obu_type == OBU_FRAME))
			{
				WriteFrameObu(obu_size); 
			}
			else if ((obu_type == OBU_TILE_LIST))
			{
				WriteTileListObu(); 
			}
			else if ((obu_type == OBU_PADDING))
			{
				WritePaddingObu(); 
			}
			else 
			{
				ReservedObu(); 
			}
			currentPosition = get_position();
			payloadBits = (currentPosition - startPosition);

			if (((((obu_size > 0) && (obu_type != OBU_TILE_GROUP)) && (obu_type != OBU_TILE_LIST)) && (obu_type != OBU_FRAME)))
			{
				WriteTrailingBits(((obu_size * 8) - payloadBits)); 
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
		public int _ObuForbiddenBit { get { return obu_forbidden_bit; } set { obu_forbidden_bit = value; } }
		private int obu_type;
		public int _ObuType { get { return obu_type; } set { obu_type = value; } }
		private int obu_extension_flag;
		public int _ObuExtensionFlag { get { return obu_extension_flag; } set { obu_extension_flag = value; } }
		private int obu_has_size_field;
		public int _ObuHasSizeField { get { return obu_has_size_field; } set { obu_has_size_field = value; } }
		private int obu_reserved_1bit;
		public int _ObuReserved1bit { get { return obu_reserved_1bit; } set { obu_reserved_1bit = value; } }

        private void ObuHeader()
        {
			stream.ReadFixed(1, out this.obu_forbidden_bit, "obu_forbidden_bit"); 
			stream.ReadFixed(4, out this.obu_type, "obu_type"); 
			stream.ReadFixed(1, out this.obu_extension_flag, "obu_extension_flag"); 
			stream.ReadFixed(1, out this.obu_has_size_field, "obu_has_size_field"); 
			stream.ReadFixed(1, out this.obu_reserved_1bit, "obu_reserved_1bit"); 

			if ((obu_extension_flag == 1))
			{
				ObuExtensionHeader(); 
			}
        }

        private void WriteObuHeader()
        {
			this.obu_forbidden_bit = stream.Pick("obu_forbidden_bit", _original != null ? _original.obu_forbidden_bit : this.obu_forbidden_bit, _edited != null ? _edited.obu_forbidden_bit : _original != null ? _original.obu_forbidden_bit : this.obu_forbidden_bit);
			stream.WriteFixed(1, this.obu_forbidden_bit, "obu_forbidden_bit"); 
			this.obu_type = stream.Pick("obu_type", _original != null ? _original.obu_type : this.obu_type, _edited != null ? _edited.obu_type : _original != null ? _original.obu_type : this.obu_type);
			stream.WriteFixed(4, this.obu_type, "obu_type"); 
			this.obu_extension_flag = stream.Pick("obu_extension_flag", _original != null ? _original.obu_extension_flag : this.obu_extension_flag, _edited != null ? _edited.obu_extension_flag : _original != null ? _original.obu_extension_flag : this.obu_extension_flag);
			stream.WriteFixed(1, this.obu_extension_flag, "obu_extension_flag"); 
			this.obu_has_size_field = stream.Pick("obu_has_size_field", _original != null ? _original.obu_has_size_field : this.obu_has_size_field, _edited != null ? _edited.obu_has_size_field : _original != null ? _original.obu_has_size_field : this.obu_has_size_field);
			stream.WriteFixed(1, this.obu_has_size_field, "obu_has_size_field"); 
			this.obu_reserved_1bit = stream.Pick("obu_reserved_1bit", _original != null ? _original.obu_reserved_1bit : this.obu_reserved_1bit, _edited != null ? _edited.obu_reserved_1bit : _original != null ? _original.obu_reserved_1bit : this.obu_reserved_1bit);
			stream.WriteFixed(1, this.obu_reserved_1bit, "obu_reserved_1bit"); 

			if ((obu_extension_flag == 1))
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
		public int _TemporalId { get { return temporal_id; } set { temporal_id = value; } }
		private int spatial_id;
		public int _SpatialId { get { return spatial_id; } set { spatial_id = value; } }
		private int extension_header_reserved_3bits;
		public int _ExtensionHeaderReserved3bits { get { return extension_header_reserved_3bits; } set { extension_header_reserved_3bits = value; } }

        private void ObuExtensionHeader()
        {
			stream.ReadFixed(3, out this.temporal_id, "temporal_id"); 
			stream.ReadFixed(2, out this.spatial_id, "spatial_id"); 
			stream.ReadFixed(3, out this.extension_header_reserved_3bits, "extension_header_reserved_3bits"); 
        }

        private void WriteObuExtensionHeader()
        {
			this.temporal_id = stream.Pick("temporal_id", _original != null ? _original.temporal_id : this.temporal_id, _edited != null ? _edited.temporal_id : _original != null ? _original.temporal_id : this.temporal_id);
			stream.WriteFixed(3, this.temporal_id, "temporal_id"); 
			this.spatial_id = stream.Pick("spatial_id", _original != null ? _original.spatial_id : this.spatial_id, _edited != null ? _edited.spatial_id : _original != null ? _original.spatial_id : this.spatial_id);
			stream.WriteFixed(2, this.spatial_id, "spatial_id"); 
			this.extension_header_reserved_3bits = stream.Pick("extension_header_reserved_3bits", _original != null ? _original.extension_header_reserved_3bits : this.extension_header_reserved_3bits, _edited != null ? _edited.extension_header_reserved_3bits : _original != null ? _original.extension_header_reserved_3bits : this.extension_header_reserved_3bits);
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
		public long _NbBits { get { return nbBits; } set { nbBits = value; } }
		private int trailing_one_bit;
		public int _TrailingOneBit { get { return trailing_one_bit; } set { trailing_one_bit = value; } }
		private int trailing_zero_bit;
		public int _TrailingZeroBit { get { return trailing_zero_bit; } set { trailing_zero_bit = value; } }

        private void TrailingBits(long nbBits)
        {
			stream.ReadFixed(1, out this.trailing_one_bit, "trailing_one_bit"); 
			nbBits--;

			while ((nbBits > 0))
			{
				stream.ReadFixed(1, out this.trailing_zero_bit, "trailing_zero_bit"); 
				nbBits--;
			}
        }

        private void WriteTrailingBits(long nbBits)
        {
			this.trailing_one_bit = stream.Pick("trailing_one_bit", _original != null ? _original.trailing_one_bit : this.trailing_one_bit, _edited != null ? _edited.trailing_one_bit : _original != null ? _original.trailing_one_bit : this.trailing_one_bit);
			stream.WriteFixed(1, this.trailing_one_bit, "trailing_one_bit"); 
			nbBits--;

			while ((nbBits > 0))
			{
				this.trailing_zero_bit = stream.Pick("trailing_zero_bit", _original != null ? _original.trailing_zero_bit : this.trailing_zero_bit, _edited != null ? _edited.trailing_zero_bit : _original != null ? _original.trailing_zero_bit : this.trailing_zero_bit);
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
		private int zero_bit;
		public int _ZeroBit { get { return zero_bit; } set { zero_bit = value; } }

        private void ByteAlignment()
        {

			while (((get_position() & 7) != 0))
			{
				stream.ReadFixed(1, out this.zero_bit, "zero_bit"); 
			}
        }

        private void WriteByteAlignment()
        {

			while (((get_position() & 7) != 0))
			{
				this.zero_bit = stream.Pick("zero_bit", _original != null ? _original.zero_bit : this.zero_bit, _edited != null ? _edited.zero_bit : _original != null ? _original.zero_bit : this.zero_bit);
				stream.WriteFixed(1, this.zero_bit, "zero_bit"); 
			}
        }

    /*
reserved_obu() { 
}
    */

        private void ReservedObu()
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
		public int _SeqProfile { get { return seq_profile; } set { seq_profile = value; } }
		private int still_picture;
		public int _StillPicture { get { return still_picture; } set { still_picture = value; } }
		private int reduced_still_picture_header;
		public int _ReducedStillPictureHeader { get { return reduced_still_picture_header; } set { reduced_still_picture_header = value; } }
		private int timing_info_present_flag;
		public int _TimingInfoPresentFlag { get { return timing_info_present_flag; } set { timing_info_present_flag = value; } }
		private int decoder_model_info_present_flag;
		public int _DecoderModelInfoPresentFlag { get { return decoder_model_info_present_flag; } set { decoder_model_info_present_flag = value; } }
		private int initial_display_delay_present_flag;
		public int _InitialDisplayDelayPresentFlag { get { return initial_display_delay_present_flag; } set { initial_display_delay_present_flag = value; } }
		private int operating_points_cnt_minus_1;
		public int _OperatingPointsCntMinus1 { get { return operating_points_cnt_minus_1; } set { operating_points_cnt_minus_1 = value; } }
		private AomArray<int> operating_point_idc = new AomArray<int>();
		public AomArray<int> _OperatingPointIdc { get { return operating_point_idc; } set { operating_point_idc = value; } }
		private AomArray<int> seq_level_idx = new AomArray<int>();
		public AomArray<int> _SeqLevelIdx { get { return seq_level_idx; } set { seq_level_idx = value; } }
		private AomArray<int> seq_tier = new AomArray<int>();
		public AomArray<int> _SeqTier { get { return seq_tier; } set { seq_tier = value; } }
		private AomArray<int> decoder_model_present_for_this_op = new AomArray<int>();
		public AomArray<int> _DecoderModelPresentForThisOp { get { return decoder_model_present_for_this_op; } set { decoder_model_present_for_this_op = value; } }
		private AomArray<int> initial_display_delay_present_for_this_op = new AomArray<int>();
		public AomArray<int> _InitialDisplayDelayPresentForThisOp { get { return initial_display_delay_present_for_this_op; } set { initial_display_delay_present_for_this_op = value; } }
		private AomArray<int> initial_display_delay_minus_1 = new AomArray<int>();
		public AomArray<int> _InitialDisplayDelayMinus1 { get { return initial_display_delay_minus_1; } set { initial_display_delay_minus_1 = value; } }
		private int OperatingPointIdc;
		public int __OperatingPointIdc { get { return OperatingPointIdc; } set { OperatingPointIdc = value; } }
		private int frame_width_bits_minus_1;
		public int _FrameWidthBitsMinus1 { get { return frame_width_bits_minus_1; } set { frame_width_bits_minus_1 = value; } }
		private int frame_height_bits_minus_1;
		public int _FrameHeightBitsMinus1 { get { return frame_height_bits_minus_1; } set { frame_height_bits_minus_1 = value; } }
		private int max_frame_width_minus_1;
		public int _MaxFrameWidthMinus1 { get { return max_frame_width_minus_1; } set { max_frame_width_minus_1 = value; } }
		private int max_frame_height_minus_1;
		public int _MaxFrameHeightMinus1 { get { return max_frame_height_minus_1; } set { max_frame_height_minus_1 = value; } }
		private int frame_id_numbers_present_flag;
		public int _FrameIdNumbersPresentFlag { get { return frame_id_numbers_present_flag; } set { frame_id_numbers_present_flag = value; } }
		private int delta_frame_id_length_minus_2;
		public int _DeltaFrameIdLengthMinus2 { get { return delta_frame_id_length_minus_2; } set { delta_frame_id_length_minus_2 = value; } }
		private int additional_frame_id_length_minus_1;
		public int _AdditionalFrameIdLengthMinus1 { get { return additional_frame_id_length_minus_1; } set { additional_frame_id_length_minus_1 = value; } }
		private int use_128x128_superblock;
		public int _Use128x128Superblock { get { return use_128x128_superblock; } set { use_128x128_superblock = value; } }
		private int enable_filter_intra;
		public int _EnableFilterIntra { get { return enable_filter_intra; } set { enable_filter_intra = value; } }
		private int enable_intra_edge_filter;
		public int _EnableIntraEdgeFilter { get { return enable_intra_edge_filter; } set { enable_intra_edge_filter = value; } }
		private int enable_interintra_compound;
		public int _EnableInterintraCompound { get { return enable_interintra_compound; } set { enable_interintra_compound = value; } }
		private int enable_masked_compound;
		public int _EnableMaskedCompound { get { return enable_masked_compound; } set { enable_masked_compound = value; } }
		private int enable_warped_motion;
		public int _EnableWarpedMotion { get { return enable_warped_motion; } set { enable_warped_motion = value; } }
		private int enable_dual_filter;
		public int _EnableDualFilter { get { return enable_dual_filter; } set { enable_dual_filter = value; } }
		private int enable_order_hint;
		public int _EnableOrderHint { get { return enable_order_hint; } set { enable_order_hint = value; } }
		private int enable_jnt_comp;
		public int _EnableJntComp { get { return enable_jnt_comp; } set { enable_jnt_comp = value; } }
		private int enable_ref_frame_mvs;
		public int _EnableRefFrameMvs { get { return enable_ref_frame_mvs; } set { enable_ref_frame_mvs = value; } }
		private int seq_force_screen_content_tools;
		public int _SeqForceScreenContentTools { get { return seq_force_screen_content_tools; } set { seq_force_screen_content_tools = value; } }
		private int seq_force_integer_mv;
		public int _SeqForceIntegerMv { get { return seq_force_integer_mv; } set { seq_force_integer_mv = value; } }
		private int OrderHintBits;
		public int _OrderHintBits { get { return OrderHintBits; } set { OrderHintBits = value; } }
		private int seq_choose_screen_content_tools;
		public int _SeqChooseScreenContentTools { get { return seq_choose_screen_content_tools; } set { seq_choose_screen_content_tools = value; } }
		private int seq_choose_integer_mv;
		public int _SeqChooseIntegerMv { get { return seq_choose_integer_mv; } set { seq_choose_integer_mv = value; } }
		private int order_hint_bits_minus_1;
		public int _OrderHintBitsMinus1 { get { return order_hint_bits_minus_1; } set { order_hint_bits_minus_1 = value; } }
		private int enable_superres;
		public int _EnableSuperres { get { return enable_superres; } set { enable_superres = value; } }
		private int enable_cdef;
		public int _EnableCdef { get { return enable_cdef; } set { enable_cdef = value; } }
		private int enable_restoration;
		public int _EnableRestoration { get { return enable_restoration; } set { enable_restoration = value; } }
		private int film_grain_params_present;
		public int _FilmGrainParamsPresent { get { return film_grain_params_present; } set { film_grain_params_present = value; } }
		private int i = 0;

        private void SequenceHeaderObu()
        {
			int i = 0;
			int operatingPoint = 0;
			int n = 0;
			stream.ReadFixed(3, out this.seq_profile, "seq_profile"); 
			stream.ReadFixed(1, out this.still_picture, "still_picture"); 
			stream.ReadFixed(1, out this.reduced_still_picture_header, "reduced_still_picture_header"); 

			if ((reduced_still_picture_header != 0))
			{
				timing_info_present_flag = 0;
				decoder_model_info_present_flag = 0;
				initial_display_delay_present_flag = 0;
				operating_points_cnt_minus_1 = 0;
				operating_point_idc[0] = 0;
				stream.ReadFixed(5, out this.seq_level_idx[0], "seq_level_idx"); 
				seq_tier[0] = 0;
				decoder_model_present_for_this_op[0] = 0;
				initial_display_delay_present_for_this_op[0] = 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.timing_info_present_flag, "timing_info_present_flag"); 

				if ((timing_info_present_flag != 0))
				{
					TimingInfo(); 
					stream.ReadFixed(1, out this.decoder_model_info_present_flag, "decoder_model_info_present_flag"); 

					if ((decoder_model_info_present_flag != 0))
					{
						DecoderModelInfo(); 
					}
				}
				else 
				{
					decoder_model_info_present_flag = 0;
				}
				stream.ReadFixed(1, out this.initial_display_delay_present_flag, "initial_display_delay_present_flag"); 
				stream.ReadFixed(5, out this.operating_points_cnt_minus_1, "operating_points_cnt_minus_1"); 

				for (i = 0; (i <= operating_points_cnt_minus_1); i++)
				{
					stream.ReadFixed(12, out this.operating_point_idc[i], "operating_point_idc"); 
					stream.ReadFixed(5, out this.seq_level_idx[i], "seq_level_idx"); 

					if ((seq_level_idx[i] > 7))
					{
						stream.ReadFixed(1, out this.seq_tier[i], "seq_tier"); 
					}
					else 
					{
						seq_tier[i] = 0;
					}

					if ((decoder_model_info_present_flag != 0))
					{
						stream.ReadFixed(1, out this.decoder_model_present_for_this_op[i], "decoder_model_present_for_this_op"); 

						if ((decoder_model_present_for_this_op[i] != 0))
						{
							OperatingParametersInfo(i); 
						}
					}
					else 
					{
						decoder_model_present_for_this_op[i] = 0;
					}

					if ((initial_display_delay_present_flag != 0))
					{
						stream.ReadFixed(1, out this.initial_display_delay_present_for_this_op[i], "initial_display_delay_present_for_this_op"); 

						if ((initial_display_delay_present_for_this_op[i] != 0))
						{
							stream.ReadFixed(4, out this.initial_display_delay_minus_1[i], "initial_display_delay_minus_1"); 
						}
					}
				}
			}
			operatingPoint = choose_operating_point();
			OperatingPointIdc = operating_point_idc[operatingPoint];
			stream.ReadFixed(4, out this.frame_width_bits_minus_1, "frame_width_bits_minus_1"); 
			stream.ReadFixed(4, out this.frame_height_bits_minus_1, "frame_height_bits_minus_1"); 
			n = (frame_width_bits_minus_1 + 1);
			stream.ReadVariable(n, out this.max_frame_width_minus_1, "max_frame_width_minus_1"); 
			n = (frame_height_bits_minus_1 + 1);
			stream.ReadVariable(n, out this.max_frame_height_minus_1, "max_frame_height_minus_1"); 

			if ((reduced_still_picture_header != 0))
			{
				frame_id_numbers_present_flag = 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.frame_id_numbers_present_flag, "frame_id_numbers_present_flag"); 
			}

			if ((frame_id_numbers_present_flag != 0))
			{
				stream.ReadFixed(4, out this.delta_frame_id_length_minus_2, "delta_frame_id_length_minus_2"); 
				stream.ReadFixed(3, out this.additional_frame_id_length_minus_1, "additional_frame_id_length_minus_1"); 
			}
			stream.ReadFixed(1, out this.use_128x128_superblock, "use_128x128_superblock"); 
			stream.ReadFixed(1, out this.enable_filter_intra, "enable_filter_intra"); 
			stream.ReadFixed(1, out this.enable_intra_edge_filter, "enable_intra_edge_filter"); 

			if ((reduced_still_picture_header != 0))
			{
				enable_interintra_compound = 0;
				enable_masked_compound = 0;
				enable_warped_motion = 0;
				enable_dual_filter = 0;
				enable_order_hint = 0;
				enable_jnt_comp = 0;
				enable_ref_frame_mvs = 0;
				seq_force_screen_content_tools = SELECT_SCREEN_CONTENT_TOOLS;
				seq_force_integer_mv = SELECT_INTEGER_MV;
				OrderHintBits = 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.enable_interintra_compound, "enable_interintra_compound"); 
				stream.ReadFixed(1, out this.enable_masked_compound, "enable_masked_compound"); 
				stream.ReadFixed(1, out this.enable_warped_motion, "enable_warped_motion"); 
				stream.ReadFixed(1, out this.enable_dual_filter, "enable_dual_filter"); 
				stream.ReadFixed(1, out this.enable_order_hint, "enable_order_hint"); 

				if ((enable_order_hint != 0))
				{
					stream.ReadFixed(1, out this.enable_jnt_comp, "enable_jnt_comp"); 
					stream.ReadFixed(1, out this.enable_ref_frame_mvs, "enable_ref_frame_mvs"); 
				}
				else 
				{
					enable_jnt_comp = 0;
					enable_ref_frame_mvs = 0;
				}
				stream.ReadFixed(1, out this.seq_choose_screen_content_tools, "seq_choose_screen_content_tools"); 

				if ((seq_choose_screen_content_tools != 0))
				{
					seq_force_screen_content_tools = SELECT_SCREEN_CONTENT_TOOLS;
				}
				else 
				{
					stream.ReadFixed(1, out this.seq_force_screen_content_tools, "seq_force_screen_content_tools"); 
				}

				if ((seq_force_screen_content_tools > 0))
				{
					stream.ReadFixed(1, out this.seq_choose_integer_mv, "seq_choose_integer_mv"); 

					if ((seq_choose_integer_mv != 0))
					{
						seq_force_integer_mv = SELECT_INTEGER_MV;
					}
					else 
					{
						stream.ReadFixed(1, out this.seq_force_integer_mv, "seq_force_integer_mv"); 
					}
				}
				else 
				{
					seq_force_integer_mv = SELECT_INTEGER_MV;
				}

				if ((enable_order_hint != 0))
				{
					stream.ReadFixed(3, out this.order_hint_bits_minus_1, "order_hint_bits_minus_1"); 
					OrderHintBits = (order_hint_bits_minus_1 + 1);
				}
				else 
				{
					OrderHintBits = 0;
				}
			}
			stream.ReadFixed(1, out this.enable_superres, "enable_superres"); 
			stream.ReadFixed(1, out this.enable_cdef, "enable_cdef"); 
			stream.ReadFixed(1, out this.enable_restoration, "enable_restoration"); 
			ColorConfig(); 
			stream.ReadFixed(1, out this.film_grain_params_present, "film_grain_params_present"); 
        }

        private void WriteSequenceHeaderObu()
        {
			int i = 0;
			int operatingPoint = 0;
			int n = 0;
			this.seq_profile = stream.Pick("seq_profile", _original != null ? _original.seq_profile : this.seq_profile, _edited != null ? _edited.seq_profile : _original != null ? _original.seq_profile : this.seq_profile);
			stream.WriteFixed(3, this.seq_profile, "seq_profile"); 
			this.still_picture = stream.Pick("still_picture", _original != null ? _original.still_picture : this.still_picture, _edited != null ? _edited.still_picture : _original != null ? _original.still_picture : this.still_picture);
			stream.WriteFixed(1, this.still_picture, "still_picture"); 
			this.reduced_still_picture_header = stream.Pick("reduced_still_picture_header", _original != null ? _original.reduced_still_picture_header : this.reduced_still_picture_header, _edited != null ? _edited.reduced_still_picture_header : _original != null ? _original.reduced_still_picture_header : this.reduced_still_picture_header);
			stream.WriteFixed(1, this.reduced_still_picture_header, "reduced_still_picture_header"); 

			if ((reduced_still_picture_header != 0))
			{
				timing_info_present_flag = 0;
				decoder_model_info_present_flag = 0;
				initial_display_delay_present_flag = 0;
				operating_points_cnt_minus_1 = 0;
				operating_point_idc[0] = 0;
				this.seq_level_idx[0] = stream.Pick("seq_level_idx", _original != null ? _original.seq_level_idx[0] : this.seq_level_idx[0], _edited != null ? _edited.seq_level_idx[0] : _original != null ? _original.seq_level_idx[0] : this.seq_level_idx[0]);
				stream.WriteFixed(5, this.seq_level_idx[0], "seq_level_idx"); 
				seq_tier[0] = 0;
				decoder_model_present_for_this_op[0] = 0;
				initial_display_delay_present_for_this_op[0] = 0;
			}
			else 
			{
				this.timing_info_present_flag = stream.Pick("timing_info_present_flag", _original != null ? _original.timing_info_present_flag : this.timing_info_present_flag, _edited != null ? _edited.timing_info_present_flag : _original != null ? _original.timing_info_present_flag : this.timing_info_present_flag);
				stream.WriteFixed(1, this.timing_info_present_flag, "timing_info_present_flag"); 

				if ((timing_info_present_flag != 0))
				{
					WriteTimingInfo(); 
					this.decoder_model_info_present_flag = stream.Pick("decoder_model_info_present_flag", _original != null ? _original.decoder_model_info_present_flag : this.decoder_model_info_present_flag, _edited != null ? _edited.decoder_model_info_present_flag : _original != null ? _original.decoder_model_info_present_flag : this.decoder_model_info_present_flag);
					stream.WriteFixed(1, this.decoder_model_info_present_flag, "decoder_model_info_present_flag"); 

					if ((decoder_model_info_present_flag != 0))
					{
						WriteDecoderModelInfo(); 
					}
				}
				else 
				{
					decoder_model_info_present_flag = 0;
				}
				this.initial_display_delay_present_flag = stream.Pick("initial_display_delay_present_flag", _original != null ? _original.initial_display_delay_present_flag : this.initial_display_delay_present_flag, _edited != null ? _edited.initial_display_delay_present_flag : _original != null ? _original.initial_display_delay_present_flag : this.initial_display_delay_present_flag);
				stream.WriteFixed(1, this.initial_display_delay_present_flag, "initial_display_delay_present_flag"); 
				this.operating_points_cnt_minus_1 = stream.Pick("operating_points_cnt_minus_1", _original != null ? _original.operating_points_cnt_minus_1 : this.operating_points_cnt_minus_1, _edited != null ? _edited.operating_points_cnt_minus_1 : _original != null ? _original.operating_points_cnt_minus_1 : this.operating_points_cnt_minus_1);
				stream.WriteFixed(5, this.operating_points_cnt_minus_1, "operating_points_cnt_minus_1"); 

				for (i = 0; (i <= operating_points_cnt_minus_1); i++)
				{
					this.operating_point_idc[i] = stream.Pick("operating_point_idc", _original != null ? _original.operating_point_idc[i] : this.operating_point_idc[i], _edited != null ? _edited.operating_point_idc[i] : _original != null ? _original.operating_point_idc[i] : this.operating_point_idc[i]);
					stream.WriteFixed(12, this.operating_point_idc[i], "operating_point_idc"); 
					this.seq_level_idx[i] = stream.Pick("seq_level_idx", _original != null ? _original.seq_level_idx[i] : this.seq_level_idx[i], _edited != null ? _edited.seq_level_idx[i] : _original != null ? _original.seq_level_idx[i] : this.seq_level_idx[i]);
					stream.WriteFixed(5, this.seq_level_idx[i], "seq_level_idx"); 

					if ((seq_level_idx[i] > 7))
					{
						this.seq_tier[i] = stream.Pick("seq_tier", _original != null ? _original.seq_tier[i] : this.seq_tier[i], _edited != null ? _edited.seq_tier[i] : _original != null ? _original.seq_tier[i] : this.seq_tier[i]);
						stream.WriteFixed(1, this.seq_tier[i], "seq_tier"); 
					}
					else 
					{
						seq_tier[i] = 0;
					}

					if ((decoder_model_info_present_flag != 0))
					{
						this.decoder_model_present_for_this_op[i] = stream.Pick("decoder_model_present_for_this_op", _original != null ? _original.decoder_model_present_for_this_op[i] : this.decoder_model_present_for_this_op[i], _edited != null ? _edited.decoder_model_present_for_this_op[i] : _original != null ? _original.decoder_model_present_for_this_op[i] : this.decoder_model_present_for_this_op[i]);
						stream.WriteFixed(1, this.decoder_model_present_for_this_op[i], "decoder_model_present_for_this_op"); 

						if ((decoder_model_present_for_this_op[i] != 0))
						{
							WriteOperatingParametersInfo(i); 
						}
					}
					else 
					{
						decoder_model_present_for_this_op[i] = 0;
					}

					if ((initial_display_delay_present_flag != 0))
					{
						this.initial_display_delay_present_for_this_op[i] = stream.Pick("initial_display_delay_present_for_this_op", _original != null ? _original.initial_display_delay_present_for_this_op[i] : this.initial_display_delay_present_for_this_op[i], _edited != null ? _edited.initial_display_delay_present_for_this_op[i] : _original != null ? _original.initial_display_delay_present_for_this_op[i] : this.initial_display_delay_present_for_this_op[i]);
						stream.WriteFixed(1, this.initial_display_delay_present_for_this_op[i], "initial_display_delay_present_for_this_op"); 

						if ((initial_display_delay_present_for_this_op[i] != 0))
						{
							this.initial_display_delay_minus_1[i] = stream.Pick("initial_display_delay_minus_1", _original != null ? _original.initial_display_delay_minus_1[i] : this.initial_display_delay_minus_1[i], _edited != null ? _edited.initial_display_delay_minus_1[i] : _original != null ? _original.initial_display_delay_minus_1[i] : this.initial_display_delay_minus_1[i]);
							stream.WriteFixed(4, this.initial_display_delay_minus_1[i], "initial_display_delay_minus_1"); 
						}
					}
				}
			}
			operatingPoint = choose_operating_point();
			OperatingPointIdc = operating_point_idc[operatingPoint];
			this.frame_width_bits_minus_1 = stream.Pick("frame_width_bits_minus_1", _original != null ? _original.frame_width_bits_minus_1 : this.frame_width_bits_minus_1, _edited != null ? _edited.frame_width_bits_minus_1 : _original != null ? _original.frame_width_bits_minus_1 : this.frame_width_bits_minus_1);
			stream.WriteFixed(4, this.frame_width_bits_minus_1, "frame_width_bits_minus_1"); 
			this.frame_height_bits_minus_1 = stream.Pick("frame_height_bits_minus_1", _original != null ? _original.frame_height_bits_minus_1 : this.frame_height_bits_minus_1, _edited != null ? _edited.frame_height_bits_minus_1 : _original != null ? _original.frame_height_bits_minus_1 : this.frame_height_bits_minus_1);
			stream.WriteFixed(4, this.frame_height_bits_minus_1, "frame_height_bits_minus_1"); 
			n = (frame_width_bits_minus_1 + 1);
			this.max_frame_width_minus_1 = stream.Pick("max_frame_width_minus_1", _original != null ? _original.max_frame_width_minus_1 : this.max_frame_width_minus_1, _edited != null ? _edited.max_frame_width_minus_1 : _original != null ? _original.max_frame_width_minus_1 : this.max_frame_width_minus_1);
			stream.WriteVariable(n, this.max_frame_width_minus_1, "max_frame_width_minus_1"); 
			n = (frame_height_bits_minus_1 + 1);
			this.max_frame_height_minus_1 = stream.Pick("max_frame_height_minus_1", _original != null ? _original.max_frame_height_minus_1 : this.max_frame_height_minus_1, _edited != null ? _edited.max_frame_height_minus_1 : _original != null ? _original.max_frame_height_minus_1 : this.max_frame_height_minus_1);
			stream.WriteVariable(n, this.max_frame_height_minus_1, "max_frame_height_minus_1"); 

			if ((reduced_still_picture_header != 0))
			{
				frame_id_numbers_present_flag = 0;
			}
			else 
			{
				this.frame_id_numbers_present_flag = stream.Pick("frame_id_numbers_present_flag", _original != null ? _original.frame_id_numbers_present_flag : this.frame_id_numbers_present_flag, _edited != null ? _edited.frame_id_numbers_present_flag : _original != null ? _original.frame_id_numbers_present_flag : this.frame_id_numbers_present_flag);
				stream.WriteFixed(1, this.frame_id_numbers_present_flag, "frame_id_numbers_present_flag"); 
			}

			if ((frame_id_numbers_present_flag != 0))
			{
				this.delta_frame_id_length_minus_2 = stream.Pick("delta_frame_id_length_minus_2", _original != null ? _original.delta_frame_id_length_minus_2 : this.delta_frame_id_length_minus_2, _edited != null ? _edited.delta_frame_id_length_minus_2 : _original != null ? _original.delta_frame_id_length_minus_2 : this.delta_frame_id_length_minus_2);
				stream.WriteFixed(4, this.delta_frame_id_length_minus_2, "delta_frame_id_length_minus_2"); 
				this.additional_frame_id_length_minus_1 = stream.Pick("additional_frame_id_length_minus_1", _original != null ? _original.additional_frame_id_length_minus_1 : this.additional_frame_id_length_minus_1, _edited != null ? _edited.additional_frame_id_length_minus_1 : _original != null ? _original.additional_frame_id_length_minus_1 : this.additional_frame_id_length_minus_1);
				stream.WriteFixed(3, this.additional_frame_id_length_minus_1, "additional_frame_id_length_minus_1"); 
			}
			this.use_128x128_superblock = stream.Pick("use_128x128_superblock", _original != null ? _original.use_128x128_superblock : this.use_128x128_superblock, _edited != null ? _edited.use_128x128_superblock : _original != null ? _original.use_128x128_superblock : this.use_128x128_superblock);
			stream.WriteFixed(1, this.use_128x128_superblock, "use_128x128_superblock"); 
			this.enable_filter_intra = stream.Pick("enable_filter_intra", _original != null ? _original.enable_filter_intra : this.enable_filter_intra, _edited != null ? _edited.enable_filter_intra : _original != null ? _original.enable_filter_intra : this.enable_filter_intra);
			stream.WriteFixed(1, this.enable_filter_intra, "enable_filter_intra"); 
			this.enable_intra_edge_filter = stream.Pick("enable_intra_edge_filter", _original != null ? _original.enable_intra_edge_filter : this.enable_intra_edge_filter, _edited != null ? _edited.enable_intra_edge_filter : _original != null ? _original.enable_intra_edge_filter : this.enable_intra_edge_filter);
			stream.WriteFixed(1, this.enable_intra_edge_filter, "enable_intra_edge_filter"); 

			if ((reduced_still_picture_header != 0))
			{
				enable_interintra_compound = 0;
				enable_masked_compound = 0;
				enable_warped_motion = 0;
				enable_dual_filter = 0;
				enable_order_hint = 0;
				enable_jnt_comp = 0;
				enable_ref_frame_mvs = 0;
				seq_force_screen_content_tools = SELECT_SCREEN_CONTENT_TOOLS;
				seq_force_integer_mv = SELECT_INTEGER_MV;
				OrderHintBits = 0;
			}
			else 
			{
				this.enable_interintra_compound = stream.Pick("enable_interintra_compound", _original != null ? _original.enable_interintra_compound : this.enable_interintra_compound, _edited != null ? _edited.enable_interintra_compound : _original != null ? _original.enable_interintra_compound : this.enable_interintra_compound);
				stream.WriteFixed(1, this.enable_interintra_compound, "enable_interintra_compound"); 
				this.enable_masked_compound = stream.Pick("enable_masked_compound", _original != null ? _original.enable_masked_compound : this.enable_masked_compound, _edited != null ? _edited.enable_masked_compound : _original != null ? _original.enable_masked_compound : this.enable_masked_compound);
				stream.WriteFixed(1, this.enable_masked_compound, "enable_masked_compound"); 
				this.enable_warped_motion = stream.Pick("enable_warped_motion", _original != null ? _original.enable_warped_motion : this.enable_warped_motion, _edited != null ? _edited.enable_warped_motion : _original != null ? _original.enable_warped_motion : this.enable_warped_motion);
				stream.WriteFixed(1, this.enable_warped_motion, "enable_warped_motion"); 
				this.enable_dual_filter = stream.Pick("enable_dual_filter", _original != null ? _original.enable_dual_filter : this.enable_dual_filter, _edited != null ? _edited.enable_dual_filter : _original != null ? _original.enable_dual_filter : this.enable_dual_filter);
				stream.WriteFixed(1, this.enable_dual_filter, "enable_dual_filter"); 
				this.enable_order_hint = stream.Pick("enable_order_hint", _original != null ? _original.enable_order_hint : this.enable_order_hint, _edited != null ? _edited.enable_order_hint : _original != null ? _original.enable_order_hint : this.enable_order_hint);
				stream.WriteFixed(1, this.enable_order_hint, "enable_order_hint"); 

				if ((enable_order_hint != 0))
				{
					this.enable_jnt_comp = stream.Pick("enable_jnt_comp", _original != null ? _original.enable_jnt_comp : this.enable_jnt_comp, _edited != null ? _edited.enable_jnt_comp : _original != null ? _original.enable_jnt_comp : this.enable_jnt_comp);
					stream.WriteFixed(1, this.enable_jnt_comp, "enable_jnt_comp"); 
					this.enable_ref_frame_mvs = stream.Pick("enable_ref_frame_mvs", _original != null ? _original.enable_ref_frame_mvs : this.enable_ref_frame_mvs, _edited != null ? _edited.enable_ref_frame_mvs : _original != null ? _original.enable_ref_frame_mvs : this.enable_ref_frame_mvs);
					stream.WriteFixed(1, this.enable_ref_frame_mvs, "enable_ref_frame_mvs"); 
				}
				else 
				{
					enable_jnt_comp = 0;
					enable_ref_frame_mvs = 0;
				}
				this.seq_choose_screen_content_tools = stream.Pick("seq_choose_screen_content_tools", _original != null ? (_original.seq_force_screen_content_tools == SELECT_SCREEN_CONTENT_TOOLS ? 1 : 0) : this.seq_choose_screen_content_tools, _edited != null ? (_edited.seq_force_screen_content_tools == SELECT_SCREEN_CONTENT_TOOLS ? 1 : 0) : _original != null ? (_original.seq_force_screen_content_tools == SELECT_SCREEN_CONTENT_TOOLS ? 1 : 0) : this.seq_choose_screen_content_tools);
				stream.WriteFixed(1, this.seq_choose_screen_content_tools, "seq_choose_screen_content_tools"); 

				if ((seq_choose_screen_content_tools != 0))
				{
					seq_force_screen_content_tools = SELECT_SCREEN_CONTENT_TOOLS;
				}
				else 
				{
					this.seq_force_screen_content_tools = stream.Pick("seq_force_screen_content_tools", _original != null ? _original.seq_force_screen_content_tools : this.seq_force_screen_content_tools, _edited != null ? _edited.seq_force_screen_content_tools : _original != null ? _original.seq_force_screen_content_tools : this.seq_force_screen_content_tools);
					stream.WriteFixed(1, this.seq_force_screen_content_tools, "seq_force_screen_content_tools"); 
				}

				if ((seq_force_screen_content_tools > 0))
				{
					this.seq_choose_integer_mv = stream.Pick("seq_choose_integer_mv", _original != null ? (_original.seq_force_integer_mv == SELECT_INTEGER_MV ? 1 : 0) : this.seq_choose_integer_mv, _edited != null ? (_edited.seq_force_integer_mv == SELECT_INTEGER_MV ? 1 : 0) : _original != null ? (_original.seq_force_integer_mv == SELECT_INTEGER_MV ? 1 : 0) : this.seq_choose_integer_mv);
					stream.WriteFixed(1, this.seq_choose_integer_mv, "seq_choose_integer_mv"); 

					if ((seq_choose_integer_mv != 0))
					{
						seq_force_integer_mv = SELECT_INTEGER_MV;
					}
					else 
					{
						this.seq_force_integer_mv = stream.Pick("seq_force_integer_mv", _original != null ? _original.seq_force_integer_mv : this.seq_force_integer_mv, _edited != null ? _edited.seq_force_integer_mv : _original != null ? _original.seq_force_integer_mv : this.seq_force_integer_mv);
						stream.WriteFixed(1, this.seq_force_integer_mv, "seq_force_integer_mv"); 
					}
				}
				else 
				{
					seq_force_integer_mv = SELECT_INTEGER_MV;
				}

				if ((enable_order_hint != 0))
				{
					this.order_hint_bits_minus_1 = stream.Pick("order_hint_bits_minus_1", _original != null ? _original.order_hint_bits_minus_1 : this.order_hint_bits_minus_1, _edited != null ? _edited.order_hint_bits_minus_1 : _original != null ? _original.order_hint_bits_minus_1 : this.order_hint_bits_minus_1);
					stream.WriteFixed(3, this.order_hint_bits_minus_1, "order_hint_bits_minus_1"); 
					OrderHintBits = (order_hint_bits_minus_1 + 1);
				}
				else 
				{
					OrderHintBits = 0;
				}
			}
			this.enable_superres = stream.Pick("enable_superres", _original != null ? _original.enable_superres : this.enable_superres, _edited != null ? _edited.enable_superres : _original != null ? _original.enable_superres : this.enable_superres);
			stream.WriteFixed(1, this.enable_superres, "enable_superres"); 
			this.enable_cdef = stream.Pick("enable_cdef", _original != null ? _original.enable_cdef : this.enable_cdef, _edited != null ? _edited.enable_cdef : _original != null ? _original.enable_cdef : this.enable_cdef);
			stream.WriteFixed(1, this.enable_cdef, "enable_cdef"); 
			this.enable_restoration = stream.Pick("enable_restoration", _original != null ? _original.enable_restoration : this.enable_restoration, _edited != null ? _edited.enable_restoration : _original != null ? _original.enable_restoration : this.enable_restoration);
			stream.WriteFixed(1, this.enable_restoration, "enable_restoration"); 
			WriteColorConfig(); 
			this.film_grain_params_present = stream.Pick("film_grain_params_present", _original != null ? _original.film_grain_params_present : this.film_grain_params_present, _edited != null ? _edited.film_grain_params_present : _original != null ? _original.film_grain_params_present : this.film_grain_params_present);
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
		public int _NumUnitsInDisplayTick { get { return num_units_in_display_tick; } set { num_units_in_display_tick = value; } }
		private int time_scale;
		public int _TimeScale { get { return time_scale; } set { time_scale = value; } }
		private int equal_picture_interval;
		public int _EqualPictureInterval { get { return equal_picture_interval; } set { equal_picture_interval = value; } }
		private uint num_ticks_per_picture_minus_1;
		public uint _NumTicksPerPictureMinus1 { get { return num_ticks_per_picture_minus_1; } set { num_ticks_per_picture_minus_1 = value; } }

        private void TimingInfo()
        {
			stream.ReadFixed(32, out this.num_units_in_display_tick, "num_units_in_display_tick"); 
			stream.ReadFixed(32, out this.time_scale, "time_scale"); 
			stream.ReadFixed(1, out this.equal_picture_interval, "equal_picture_interval"); 

			if ((equal_picture_interval != 0))
			{
				stream.ReadUvlc( out this.num_ticks_per_picture_minus_1, "num_ticks_per_picture_minus_1"); 
			}
        }

        private void WriteTimingInfo()
        {
			this.num_units_in_display_tick = stream.Pick("num_units_in_display_tick", _original != null ? _original.num_units_in_display_tick : this.num_units_in_display_tick, _edited != null ? _edited.num_units_in_display_tick : _original != null ? _original.num_units_in_display_tick : this.num_units_in_display_tick);
			stream.WriteFixed(32, this.num_units_in_display_tick, "num_units_in_display_tick"); 
			this.time_scale = stream.Pick("time_scale", _original != null ? _original.time_scale : this.time_scale, _edited != null ? _edited.time_scale : _original != null ? _original.time_scale : this.time_scale);
			stream.WriteFixed(32, this.time_scale, "time_scale"); 
			this.equal_picture_interval = stream.Pick("equal_picture_interval", _original != null ? _original.equal_picture_interval : this.equal_picture_interval, _edited != null ? _edited.equal_picture_interval : _original != null ? _original.equal_picture_interval : this.equal_picture_interval);
			stream.WriteFixed(1, this.equal_picture_interval, "equal_picture_interval"); 

			if ((equal_picture_interval != 0))
			{
				this.num_ticks_per_picture_minus_1 = stream.Pick("num_ticks_per_picture_minus_1", _original != null ? _original.num_ticks_per_picture_minus_1 : this.num_ticks_per_picture_minus_1, _edited != null ? _edited.num_ticks_per_picture_minus_1 : _original != null ? _original.num_ticks_per_picture_minus_1 : this.num_ticks_per_picture_minus_1);
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
		public int _BufferDelayLengthMinus1 { get { return buffer_delay_length_minus_1; } set { buffer_delay_length_minus_1 = value; } }
		private int num_units_in_decoding_tick;
		public int _NumUnitsInDecodingTick { get { return num_units_in_decoding_tick; } set { num_units_in_decoding_tick = value; } }
		private int buffer_removal_time_length_minus_1;
		public int _BufferRemovalTimeLengthMinus1 { get { return buffer_removal_time_length_minus_1; } set { buffer_removal_time_length_minus_1 = value; } }
		private int frame_presentation_time_length_minus_1;
		public int _FramePresentationTimeLengthMinus1 { get { return frame_presentation_time_length_minus_1; } set { frame_presentation_time_length_minus_1 = value; } }

        private void DecoderModelInfo()
        {
			stream.ReadFixed(5, out this.buffer_delay_length_minus_1, "buffer_delay_length_minus_1"); 
			stream.ReadFixed(32, out this.num_units_in_decoding_tick, "num_units_in_decoding_tick"); 
			stream.ReadFixed(5, out this.buffer_removal_time_length_minus_1, "buffer_removal_time_length_minus_1"); 
			stream.ReadFixed(5, out this.frame_presentation_time_length_minus_1, "frame_presentation_time_length_minus_1"); 
        }

        private void WriteDecoderModelInfo()
        {
			this.buffer_delay_length_minus_1 = stream.Pick("buffer_delay_length_minus_1", _original != null ? _original.buffer_delay_length_minus_1 : this.buffer_delay_length_minus_1, _edited != null ? _edited.buffer_delay_length_minus_1 : _original != null ? _original.buffer_delay_length_minus_1 : this.buffer_delay_length_minus_1);
			stream.WriteFixed(5, this.buffer_delay_length_minus_1, "buffer_delay_length_minus_1"); 
			this.num_units_in_decoding_tick = stream.Pick("num_units_in_decoding_tick", _original != null ? _original.num_units_in_decoding_tick : this.num_units_in_decoding_tick, _edited != null ? _edited.num_units_in_decoding_tick : _original != null ? _original.num_units_in_decoding_tick : this.num_units_in_decoding_tick);
			stream.WriteFixed(32, this.num_units_in_decoding_tick, "num_units_in_decoding_tick"); 
			this.buffer_removal_time_length_minus_1 = stream.Pick("buffer_removal_time_length_minus_1", _original != null ? _original.buffer_removal_time_length_minus_1 : this.buffer_removal_time_length_minus_1, _edited != null ? _edited.buffer_removal_time_length_minus_1 : _original != null ? _original.buffer_removal_time_length_minus_1 : this.buffer_removal_time_length_minus_1);
			stream.WriteFixed(5, this.buffer_removal_time_length_minus_1, "buffer_removal_time_length_minus_1"); 
			this.frame_presentation_time_length_minus_1 = stream.Pick("frame_presentation_time_length_minus_1", _original != null ? _original.frame_presentation_time_length_minus_1 : this.frame_presentation_time_length_minus_1, _edited != null ? _edited.frame_presentation_time_length_minus_1 : _original != null ? _original.frame_presentation_time_length_minus_1 : this.frame_presentation_time_length_minus_1);
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
		public int _Op { get { return op; } set { op = value; } }
		private AomArray<int> decoder_buffer_delay = new AomArray<int>();
		public AomArray<int> _DecoderBufferDelay { get { return decoder_buffer_delay; } set { decoder_buffer_delay = value; } }
		private AomArray<int> encoder_buffer_delay = new AomArray<int>();
		public AomArray<int> _EncoderBufferDelay { get { return encoder_buffer_delay; } set { encoder_buffer_delay = value; } }
		private AomArray<int> low_delay_mode_flag = new AomArray<int>();
		public AomArray<int> _LowDelayModeFlag { get { return low_delay_mode_flag; } set { low_delay_mode_flag = value; } }

        private void OperatingParametersInfo(int op)
        {
			int n = 0;
			n = (buffer_delay_length_minus_1 + 1);
			stream.ReadVariable(n, out this.decoder_buffer_delay[op], "decoder_buffer_delay"); 
			stream.ReadVariable(n, out this.encoder_buffer_delay[op], "encoder_buffer_delay"); 
			stream.ReadFixed(1, out this.low_delay_mode_flag[op], "low_delay_mode_flag"); 
        }

        private void WriteOperatingParametersInfo(int op)
        {
			int n = 0;
			n = (buffer_delay_length_minus_1 + 1);
			this.decoder_buffer_delay[op] = stream.Pick("decoder_buffer_delay", _original != null ? _original.decoder_buffer_delay[op] : this.decoder_buffer_delay[op], _edited != null ? _edited.decoder_buffer_delay[op] : _original != null ? _original.decoder_buffer_delay[op] : this.decoder_buffer_delay[op]);
			stream.WriteVariable(n, this.decoder_buffer_delay[op], "decoder_buffer_delay"); 
			this.encoder_buffer_delay[op] = stream.Pick("encoder_buffer_delay", _original != null ? _original.encoder_buffer_delay[op] : this.encoder_buffer_delay[op], _edited != null ? _edited.encoder_buffer_delay[op] : _original != null ? _original.encoder_buffer_delay[op] : this.encoder_buffer_delay[op]);
			stream.WriteVariable(n, this.encoder_buffer_delay[op], "encoder_buffer_delay"); 
			this.low_delay_mode_flag[op] = stream.Pick("low_delay_mode_flag", _original != null ? _original.low_delay_mode_flag[op] : this.low_delay_mode_flag[op], _edited != null ? _edited.low_delay_mode_flag[op] : _original != null ? _original.low_delay_mode_flag[op] : this.low_delay_mode_flag[op]);
			stream.WriteFixed(1, this.low_delay_mode_flag[op], "low_delay_mode_flag"); 
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
		private int high_bitdepth;
		public int _HighBitdepth { get { return high_bitdepth; } set { high_bitdepth = value; } }
		private int twelve_bit;
		public int _TwelveBit { get { return twelve_bit; } set { twelve_bit = value; } }
		private int BitDepth;
		public int _BitDepth { get { return BitDepth; } set { BitDepth = value; } }
		private int mono_chrome;
		public int _MonoChrome { get { return mono_chrome; } set { mono_chrome = value; } }
		private int NumPlanes;
		public int _NumPlanes { get { return NumPlanes; } set { NumPlanes = value; } }
		private int color_description_present_flag;
		public int _ColorDescriptionPresentFlag { get { return color_description_present_flag; } set { color_description_present_flag = value; } }
		private int color_primaries;
		public int _ColorPrimaries { get { return color_primaries; } set { color_primaries = value; } }
		private int transfer_characteristics;
		public int _TransferCharacteristics { get { return transfer_characteristics; } set { transfer_characteristics = value; } }
		private int matrix_coefficients;
		public int _MatrixCoefficients { get { return matrix_coefficients; } set { matrix_coefficients = value; } }
		private int color_range;
		public int _ColorRange { get { return color_range; } set { color_range = value; } }
		private int subsampling_x;
		public int _Subsamplingx { get { return subsampling_x; } set { subsampling_x = value; } }
		private int subsampling_y;
		public int _Subsamplingy { get { return subsampling_y; } set { subsampling_y = value; } }
		private int chroma_sample_position;
		public int _ChromaSamplePosition { get { return chroma_sample_position; } set { chroma_sample_position = value; } }
		private int separate_uv_delta_q;
		public int _SeparateUvDeltaq { get { return separate_uv_delta_q; } set { separate_uv_delta_q = value; } }

        private void ColorConfig()
        {
			stream.ReadFixed(1, out this.high_bitdepth, "high_bitdepth"); 

			if (((seq_profile == 2) && (high_bitdepth != 0)))
			{
				stream.ReadFixed(1, out this.twelve_bit, "twelve_bit"); 
				BitDepth = ((twelve_bit != 0) ? 12 : 10);
			}
			else if ((seq_profile <= 2))
			{
				BitDepth = ((high_bitdepth != 0) ? 10 : 8);
			}

			if ((seq_profile == 1))
			{
				mono_chrome = 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.mono_chrome, "mono_chrome"); 
			}
			NumPlanes = ((mono_chrome != 0) ? 1 : 3);
			stream.ReadFixed(1, out this.color_description_present_flag, "color_description_present_flag"); 

			if ((color_description_present_flag != 0))
			{
				stream.ReadFixed(8, out this.color_primaries, "color_primaries"); 
				stream.ReadFixed(8, out this.transfer_characteristics, "transfer_characteristics"); 
				stream.ReadFixed(8, out this.matrix_coefficients, "matrix_coefficients"); 
			}
			else 
			{
				color_primaries = CP_UNSPECIFIED;
				transfer_characteristics = TC_UNSPECIFIED;
				matrix_coefficients = MC_UNSPECIFIED;
			}

			if ((mono_chrome != 0))
			{
				stream.ReadFixed(1, out this.color_range, "color_range"); 
				subsampling_x = 1;
				subsampling_y = 1;
				chroma_sample_position = CSP_UNKNOWN;
				separate_uv_delta_q = 0;
				return;
			}
			else if ((((color_primaries == CP_BT_709) && (transfer_characteristics == TC_SRGB)) && (matrix_coefficients == MC_IDENTITY)))
			{
				color_range = 1;
				subsampling_x = 0;
				subsampling_y = 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.color_range, "color_range"); 

				if ((seq_profile == 0))
				{
					subsampling_x = 1;
					subsampling_y = 1;
				}
				else if ((seq_profile == 1))
				{
					subsampling_x = 0;
					subsampling_y = 0;
				}
				else 
				{

					if ((BitDepth == 12))
					{
						stream.ReadFixed(1, out this.subsampling_x, "subsampling_x"); 

						if ((subsampling_x != 0))
						{
							stream.ReadFixed(1, out this.subsampling_y, "subsampling_y"); 
						}
						else 
						{
							subsampling_y = 0;
						}
					}
					else 
					{
						subsampling_x = 1;
						subsampling_y = 0;
					}
				}

				if (((subsampling_x != 0) && (subsampling_y != 0)))
				{
					stream.ReadFixed(2, out this.chroma_sample_position, "chroma_sample_position"); 
				}
			}
			stream.ReadFixed(1, out this.separate_uv_delta_q, "separate_uv_delta_q"); 
        }

        private void WriteColorConfig()
        {
			this.high_bitdepth = stream.Pick("high_bitdepth", _original != null ? _original.high_bitdepth : this.high_bitdepth, _edited != null ? _edited.high_bitdepth : _original != null ? _original.high_bitdepth : this.high_bitdepth);
			stream.WriteFixed(1, this.high_bitdepth, "high_bitdepth"); 

			if (((seq_profile == 2) && (high_bitdepth != 0)))
			{
				this.twelve_bit = stream.Pick("twelve_bit", _original != null ? _original.twelve_bit : this.twelve_bit, _edited != null ? _edited.twelve_bit : _original != null ? _original.twelve_bit : this.twelve_bit);
				stream.WriteFixed(1, this.twelve_bit, "twelve_bit"); 
				BitDepth = ((twelve_bit != 0) ? 12 : 10);
			}
			else if ((seq_profile <= 2))
			{
				BitDepth = ((high_bitdepth != 0) ? 10 : 8);
			}

			if ((seq_profile == 1))
			{
				mono_chrome = 0;
			}
			else 
			{
				this.mono_chrome = stream.Pick("mono_chrome", _original != null ? _original.mono_chrome : this.mono_chrome, _edited != null ? _edited.mono_chrome : _original != null ? _original.mono_chrome : this.mono_chrome);
				stream.WriteFixed(1, this.mono_chrome, "mono_chrome"); 
			}
			NumPlanes = ((mono_chrome != 0) ? 1 : 3);
			this.color_description_present_flag = stream.Pick("color_description_present_flag", _original != null ? _original.color_description_present_flag : this.color_description_present_flag, _edited != null ? _edited.color_description_present_flag : _original != null ? _original.color_description_present_flag : this.color_description_present_flag);
			stream.WriteFixed(1, this.color_description_present_flag, "color_description_present_flag"); 

			if ((color_description_present_flag != 0))
			{
				this.color_primaries = stream.Pick("color_primaries", _original != null ? _original.color_primaries : this.color_primaries, _edited != null ? _edited.color_primaries : _original != null ? _original.color_primaries : this.color_primaries);
				stream.WriteFixed(8, this.color_primaries, "color_primaries"); 
				this.transfer_characteristics = stream.Pick("transfer_characteristics", _original != null ? _original.transfer_characteristics : this.transfer_characteristics, _edited != null ? _edited.transfer_characteristics : _original != null ? _original.transfer_characteristics : this.transfer_characteristics);
				stream.WriteFixed(8, this.transfer_characteristics, "transfer_characteristics"); 
				this.matrix_coefficients = stream.Pick("matrix_coefficients", _original != null ? _original.matrix_coefficients : this.matrix_coefficients, _edited != null ? _edited.matrix_coefficients : _original != null ? _original.matrix_coefficients : this.matrix_coefficients);
				stream.WriteFixed(8, this.matrix_coefficients, "matrix_coefficients"); 
			}
			else 
			{
				color_primaries = CP_UNSPECIFIED;
				transfer_characteristics = TC_UNSPECIFIED;
				matrix_coefficients = MC_UNSPECIFIED;
			}

			if ((mono_chrome != 0))
			{
				this.color_range = stream.Pick("color_range", _original != null ? _original.color_range : this.color_range, _edited != null ? _edited.color_range : _original != null ? _original.color_range : this.color_range);
				stream.WriteFixed(1, this.color_range, "color_range"); 
				subsampling_x = 1;
				subsampling_y = 1;
				chroma_sample_position = CSP_UNKNOWN;
				separate_uv_delta_q = 0;
				return;
			}
			else if ((((color_primaries == CP_BT_709) && (transfer_characteristics == TC_SRGB)) && (matrix_coefficients == MC_IDENTITY)))
			{
				color_range = 1;
				subsampling_x = 0;
				subsampling_y = 0;
			}
			else 
			{
				this.color_range = stream.Pick("color_range", _original != null ? _original.color_range : this.color_range, _edited != null ? _edited.color_range : _original != null ? _original.color_range : this.color_range);
				stream.WriteFixed(1, this.color_range, "color_range"); 

				if ((seq_profile == 0))
				{
					subsampling_x = 1;
					subsampling_y = 1;
				}
				else if ((seq_profile == 1))
				{
					subsampling_x = 0;
					subsampling_y = 0;
				}
				else 
				{

					if ((BitDepth == 12))
					{
						this.subsampling_x = stream.Pick("subsampling_x", _original != null ? _original.subsampling_x : this.subsampling_x, _edited != null ? _edited.subsampling_x : _original != null ? _original.subsampling_x : this.subsampling_x);
						stream.WriteFixed(1, this.subsampling_x, "subsampling_x"); 

						if ((subsampling_x != 0))
						{
							this.subsampling_y = stream.Pick("subsampling_y", _original != null ? _original.subsampling_y : this.subsampling_y, _edited != null ? _edited.subsampling_y : _original != null ? _original.subsampling_y : this.subsampling_y);
							stream.WriteFixed(1, this.subsampling_y, "subsampling_y"); 
						}
						else 
						{
							subsampling_y = 0;
						}
					}
					else 
					{
						subsampling_x = 1;
						subsampling_y = 0;
					}
				}

				if (((subsampling_x != 0) && (subsampling_y != 0)))
				{
					this.chroma_sample_position = stream.Pick("chroma_sample_position", _original != null ? _original.chroma_sample_position : this.chroma_sample_position, _edited != null ? _edited.chroma_sample_position : _original != null ? _original.chroma_sample_position : this.chroma_sample_position);
					stream.WriteFixed(2, this.chroma_sample_position, "chroma_sample_position"); 
				}
			}
			this.separate_uv_delta_q = stream.Pick("separate_uv_delta_q", _original != null ? _original.separate_uv_delta_q : this.separate_uv_delta_q, _edited != null ? _edited.separate_uv_delta_q : _original != null ? _original.separate_uv_delta_q : this.separate_uv_delta_q);
			stream.WriteFixed(1, this.separate_uv_delta_q, "separate_uv_delta_q"); 
        }

    /*
frame_header_obu() { 
 if ( SeenFrameHeader == 1 ) {
 frame_header_copy()
 } else {
 SeenFrameHeader = 1
 uncompressed_header()
 FrameHeaderDone()
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
		private int SeenFrameHeader;
		public int _SeenFrameHeader { get { return SeenFrameHeader; } set { SeenFrameHeader = value; } }
		private int TileNum;
		public int _TileNum { get { return TileNum; } set { TileNum = value; } }

        private void FrameHeaderObu()
        {

			if ((SeenFrameHeader == 1))
			{
				frame_header_copy(); 
			}
			else 
			{
				SeenFrameHeader = 1;
				UncompressedHeader(); 
				FrameHeaderDone(); 

				if ((show_existing_frame != 0))
				{
					decode_frame_wrapup(); 
					SeenFrameHeader = 0;
				}
				else 
				{
					TileNum = 0;
					SeenFrameHeader = 1;
				}
			}
        }

        private void WriteFrameHeaderObu()
        {

			if ((SeenFrameHeader == 1))
			{
				frame_header_copy(); 
			}
			else 
			{
				SeenFrameHeader = 1;
				WriteUncompressedHeader(); 
				FrameHeaderDone(); 

				if ((show_existing_frame != 0))
				{
					decode_frame_wrapup(); 
					SeenFrameHeader = 0;
				}
				else 
				{
					TileNum = 0;
					SeenFrameHeader = 1;
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
    show_frame f(1)
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
		private int show_existing_frame;
		public int _ShowExistingFrame { get { return show_existing_frame; } set { show_existing_frame = value; } }
		private int frame_type;
		public int _FrameType { get { return frame_type; } set { frame_type = value; } }
		private int FrameIsIntra;
		public int _FrameIsIntra { get { return FrameIsIntra; } set { FrameIsIntra = value; } }
		private int show_frame;
		public int _ShowFrame { get { return show_frame; } set { show_frame = value; } }
		private int showable_frame;
		public int _ShowableFrame { get { return showable_frame; } set { showable_frame = value; } }
		private int frame_to_show_map_idx;
		public int _FrameToShowMapIdx { get { return frame_to_show_map_idx; } set { frame_to_show_map_idx = value; } }
		private int refresh_frame_flags;
		public int _RefreshFrameFlags { get { return refresh_frame_flags; } set { refresh_frame_flags = value; } }
		private int display_frame_id;
		public int _DisplayFrameId { get { return display_frame_id; } set { display_frame_id = value; } }
		private int error_resilient_mode;
		public int _ErrorResilientMode { get { return error_resilient_mode; } set { error_resilient_mode = value; } }
		private AomArray<int> RefValid = new AomArray<int>();
		public AomArray<int> _RefValid { get { return RefValid; } set { RefValid = value; } }
		private AomArray<int> RefOrderHint = new AomArray<int>();
		public AomArray<int> _RefOrderHint { get { return RefOrderHint; } set { RefOrderHint = value; } }
		private AomArray<int> OrderHints = new AomArray<int>();
		public AomArray<int> _OrderHints { get { return OrderHints; } set { OrderHints = value; } }
		private int disable_cdf_update;
		public int _DisableCdfUpdate { get { return disable_cdf_update; } set { disable_cdf_update = value; } }
		private int allow_screen_content_tools;
		public int _AllowScreenContentTools { get { return allow_screen_content_tools; } set { allow_screen_content_tools = value; } }
		private int force_integer_mv;
		public int _ForceIntegerMv { get { return force_integer_mv; } set { force_integer_mv = value; } }
		private int PrevFrameID;
		public int _PrevFrameID { get { return PrevFrameID; } set { PrevFrameID = value; } }
		private int current_frame_id;
		public int _CurrentFrameId { get { return current_frame_id; } set { current_frame_id = value; } }
		private int frame_size_override_flag;
		public int _FrameSizeOverrideFlag { get { return frame_size_override_flag; } set { frame_size_override_flag = value; } }
		private int order_hint;
		public int _OrderHint { get { return order_hint; } set { order_hint = value; } }
		private int OrderHint;
		public int __OrderHint { get { return OrderHint; } set { OrderHint = value; } }
		private int primary_ref_frame;
		public int _PrimaryRefFrame { get { return primary_ref_frame; } set { primary_ref_frame = value; } }
		private int buffer_removal_time_present_flag;
		public int _BufferRemovalTimePresentFlag { get { return buffer_removal_time_present_flag; } set { buffer_removal_time_present_flag = value; } }
		private AomArray<int> buffer_removal_time = new AomArray<int>();
		public AomArray<int> _BufferRemovalTime { get { return buffer_removal_time; } set { buffer_removal_time = value; } }
		private int allow_high_precision_mv;
		public int _AllowHighPrecisionMv { get { return allow_high_precision_mv; } set { allow_high_precision_mv = value; } }
		private int use_ref_frame_mvs;
		public int _UseRefFrameMvs { get { return use_ref_frame_mvs; } set { use_ref_frame_mvs = value; } }
		private int allow_intrabc;
		public int _AllowIntrabc { get { return allow_intrabc; } set { allow_intrabc = value; } }
		private AomArray<int> ref_order_hint = new AomArray<int>();
		public AomArray<int> __RefOrderHint { get { return ref_order_hint; } set { ref_order_hint = value; } }
		private int frame_refs_short_signaling;
		public int _FrameRefsShortSignaling { get { return frame_refs_short_signaling; } set { frame_refs_short_signaling = value; } }
		private int last_frame_idx;
		public int _LastFrameIdx { get { return last_frame_idx; } set { last_frame_idx = value; } }
		private int gold_frame_idx;
		public int _GoldFrameIdx { get { return gold_frame_idx; } set { gold_frame_idx = value; } }
		private AomArray<int> ref_frame_idx = new AomArray<int>();
		public AomArray<int> _RefFrameIdx { get { return ref_frame_idx; } set { ref_frame_idx = value; } }
		private int delta_frame_id_minus_1;
		public int _DeltaFrameIdMinus1 { get { return delta_frame_id_minus_1; } set { delta_frame_id_minus_1 = value; } }
		private int DeltaFrameId;
		public int _DeltaFrameId { get { return DeltaFrameId; } set { DeltaFrameId = value; } }
		private int is_motion_mode_switchable;
		public int _IsMotionModeSwitchable { get { return is_motion_mode_switchable; } set { is_motion_mode_switchable = value; } }
		private AomArray<int> RefFrameSignBias = new AomArray<int>();
		public AomArray<int> _RefFrameSignBias { get { return RefFrameSignBias; } set { RefFrameSignBias = value; } }
		private int disable_frame_end_update_cdf;
		public int _DisableFrameEndUpdateCdf { get { return disable_frame_end_update_cdf; } set { disable_frame_end_update_cdf = value; } }
		private int CodedLossless;
		public int _CodedLossless { get { return CodedLossless; } set { CodedLossless = value; } }
		private AomArray<int> LosslessArray = new AomArray<int>();
		public AomArray<int> _LosslessArray { get { return LosslessArray; } set { LosslessArray = value; } }
		private AomArray<AomArray<int>> SegQMLevel = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _SegQMLevel { get { return SegQMLevel; } set { SegQMLevel = value; } }
		private int AllLossless;
		public int _AllLossless { get { return AllLossless; } set { AllLossless = value; } }
		private int allow_warped_motion;
		public int _AllowWarpedMotion { get { return allow_warped_motion; } set { allow_warped_motion = value; } }
		private int reduced_tx_set;
		public int _ReducedTxSet { get { return reduced_tx_set; } set { reduced_tx_set = value; } }
		private int opNum = 0;
		private int segmentId = 0;

        private void UncompressedHeader()
        {
			int i = 0;
			int opNum = 0;
			int segmentId = 0;
			int idLen = 0;
			int allFrames = 0;
			int opPtIdc = 0;
			int inTemporalLayer = 0;
			int inSpatialLayer = 0;
			int n = 0;
			AomArray<int> expectedFrameId = new AomArray<int>();
			int refFrame = 0;
			int hint = 0;
			int qindex = 0;

			if ((frame_id_numbers_present_flag != 0))
			{
				idLen = ((additional_frame_id_length_minus_1 + delta_frame_id_length_minus_2) + 3);
			}
			allFrames = ((1 << NUM_REF_FRAMES) - 1);

			if ((reduced_still_picture_header != 0))
			{
				show_existing_frame = 0;
				frame_type = KEY_FRAME;
				FrameIsIntra = 1;
				show_frame = 1;
				showable_frame = 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.show_existing_frame, "show_existing_frame"); 

				if ((show_existing_frame == 1))
				{
					stream.ReadFixed(3, out this.frame_to_show_map_idx, "frame_to_show_map_idx"); 

					if (((decoder_model_info_present_flag != 0) && !(equal_picture_interval != 0)))
					{
						TemporalPointInfo(); 
					}
					refresh_frame_flags = 0;

					if ((frame_id_numbers_present_flag != 0))
					{
						stream.ReadVariable(idLen, out this.display_frame_id, "display_frame_id"); 
					}
					frame_type = RefFrameType[frame_to_show_map_idx];

					if ((frame_type == KEY_FRAME))
					{
						refresh_frame_flags = allFrames;
					}

					if ((film_grain_params_present != 0))
					{
						load_grain_params(frame_to_show_map_idx); 
					}
					return;
				}
				stream.ReadFixed(2, out this.frame_type, "frame_type"); 
				FrameIsIntra = (((frame_type == INTRA_ONLY_FRAME) || (frame_type == KEY_FRAME)) ? 1 : 0);
				stream.ReadFixed(1, out this.show_frame, "show_frame"); 

				if ((((show_frame != 0) && (decoder_model_info_present_flag != 0)) && !(equal_picture_interval != 0)))
				{
					TemporalPointInfo(); 
				}

				if ((show_frame != 0))
				{
					showable_frame = ((frame_type != KEY_FRAME) ? 1 : 0);
				}
				else 
				{
					stream.ReadFixed(1, out this.showable_frame, "showable_frame"); 
				}

				if (((frame_type == SWITCH_FRAME) || ((frame_type == KEY_FRAME) && (show_frame != 0))))
				{
					error_resilient_mode = 1;
				}
				else 
				{
					stream.ReadFixed(1, out this.error_resilient_mode, "error_resilient_mode"); 
				}
			}

			if (((frame_type == KEY_FRAME) && (show_frame != 0)))
			{

				for (i = 0; (i < NUM_REF_FRAMES); i++)
				{
					RefValid[i] = 0;
					RefOrderHint[i] = 0;
				}

				for (i = 0; (i < REFS_PER_FRAME); i++)
				{
					OrderHints[(LAST_FRAME + i)] = 0;
				}
			}
			stream.ReadFixed(1, out this.disable_cdf_update, "disable_cdf_update"); 

			if ((seq_force_screen_content_tools == SELECT_SCREEN_CONTENT_TOOLS))
			{
				stream.ReadFixed(1, out this.allow_screen_content_tools, "allow_screen_content_tools"); 
			}
			else 
			{
				allow_screen_content_tools = seq_force_screen_content_tools;
			}

			if ((allow_screen_content_tools != 0))
			{

				if ((seq_force_integer_mv == SELECT_INTEGER_MV))
				{
					stream.ReadFixed(1, out this.force_integer_mv, "force_integer_mv"); 
				}
				else 
				{
					force_integer_mv = seq_force_integer_mv;
				}
			}
			else 
			{
				force_integer_mv = 0;
			}

			if ((FrameIsIntra != 0))
			{
				force_integer_mv = 1;
			}

			if ((frame_id_numbers_present_flag != 0))
			{
				PrevFrameID = current_frame_id;
				stream.ReadVariable(idLen, out this.current_frame_id, "current_frame_id"); 
				MarkRefFrames(idLen); 
			}
			else 
			{
				current_frame_id = 0;
			}

			if ((frame_type == SWITCH_FRAME))
			{
				frame_size_override_flag = 1;
			}
			else if ((reduced_still_picture_header != 0))
			{
				frame_size_override_flag = 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.frame_size_override_flag, "frame_size_override_flag"); 
			}
			stream.ReadVariable(OrderHintBits, out this.order_hint, "order_hint"); 
			OrderHint = order_hint;

			if (((FrameIsIntra != 0) || (error_resilient_mode != 0)))
			{
				primary_ref_frame = PRIMARY_REF_NONE;
			}
			else 
			{
				stream.ReadFixed(3, out this.primary_ref_frame, "primary_ref_frame"); 
			}

			if ((decoder_model_info_present_flag != 0))
			{
				stream.ReadFixed(1, out this.buffer_removal_time_present_flag, "buffer_removal_time_present_flag"); 

				if ((buffer_removal_time_present_flag != 0))
				{

					for (opNum = 0; (opNum <= operating_points_cnt_minus_1); opNum++)
					{

						if ((decoder_model_present_for_this_op[opNum] != 0))
						{
							opPtIdc = operating_point_idc[opNum];
							inTemporalLayer = ((opPtIdc >> temporal_id) & 1);
							inSpatialLayer = ((opPtIdc >> (spatial_id + 8)) & 1);

							if (((opPtIdc == 0) || ((inTemporalLayer != 0) && (inSpatialLayer != 0))))
							{
								n = (buffer_removal_time_length_minus_1 + 1);
								stream.ReadVariable(n, out this.buffer_removal_time[opNum], "buffer_removal_time"); 
							}
						}
					}
				}
			}
			allow_high_precision_mv = 0;
			use_ref_frame_mvs = 0;
			allow_intrabc = 0;

			if (((frame_type == SWITCH_FRAME) || ((frame_type == KEY_FRAME) && (show_frame != 0))))
			{
				refresh_frame_flags = allFrames;
			}
			else 
			{
				stream.ReadFixed(8, out this.refresh_frame_flags, "refresh_frame_flags"); 
			}

			if ((!(FrameIsIntra != 0) || (refresh_frame_flags != allFrames)))
			{

				if (((error_resilient_mode != 0) && (enable_order_hint != 0)))
				{

					for (i = 0; (i < NUM_REF_FRAMES); i++)
					{
						stream.ReadVariable(OrderHintBits, out this.ref_order_hint[i], "ref_order_hint"); 

						if ((ref_order_hint[i] != RefOrderHint[i]))
						{
							RefValid[i] = 0;
						}
					}
				}
			}

			if ((FrameIsIntra != 0))
			{
				FrameSize(); 
				RenderSize(); 

				if (((allow_screen_content_tools != 0) && (UpscaledWidth == FrameWidth)))
				{
					stream.ReadFixed(1, out this.allow_intrabc, "allow_intrabc"); 
				}
			}
			else 
			{

				if (!(enable_order_hint != 0))
				{
					frame_refs_short_signaling = 0;
				}
				else 
				{
					stream.ReadFixed(1, out this.frame_refs_short_signaling, "frame_refs_short_signaling"); 

					if ((frame_refs_short_signaling != 0))
					{
						stream.ReadFixed(3, out this.last_frame_idx, "last_frame_idx"); 
						stream.ReadFixed(3, out this.gold_frame_idx, "gold_frame_idx"); 
						SetFrameRefs(); 
					}
				}

				for (i = 0; (i < REFS_PER_FRAME); i++)
				{

					if (!(frame_refs_short_signaling != 0))
					{
						stream.ReadFixed(3, out this.ref_frame_idx[i], "ref_frame_idx"); 
					}

					if ((frame_id_numbers_present_flag != 0))
					{
						n = (delta_frame_id_length_minus_2 + 2);
						stream.ReadVariable(n, out this.delta_frame_id_minus_1, "delta_frame_id_minus_1"); 
						DeltaFrameId = (delta_frame_id_minus_1 + 1);
						expectedFrameId[i] = (((current_frame_id + (1 << idLen)) - DeltaFrameId) % (1 << idLen));
					}
				}

				if (((frame_size_override_flag != 0) && !(error_resilient_mode != 0)))
				{
					FrameSizeWithRefs(); 
				}
				else 
				{
					FrameSize(); 
					RenderSize(); 
				}

				if ((force_integer_mv != 0))
				{
					allow_high_precision_mv = 0;
				}
				else 
				{
					stream.ReadFixed(1, out this.allow_high_precision_mv, "allow_high_precision_mv"); 
				}
				ReadInterpolationFilter(); 
				stream.ReadFixed(1, out this.is_motion_mode_switchable, "is_motion_mode_switchable"); 

				if (((error_resilient_mode != 0) || !(enable_ref_frame_mvs != 0)))
				{
					use_ref_frame_mvs = 0;
				}
				else 
				{
					stream.ReadFixed(1, out this.use_ref_frame_mvs, "use_ref_frame_mvs"); 
				}

				for (i = 0; (i < REFS_PER_FRAME); i++)
				{
					refFrame = (LAST_FRAME + i);
					hint = RefOrderHint[ref_frame_idx[i]];
					OrderHints[refFrame] = hint;

					if (!(enable_order_hint != 0))
					{
						RefFrameSignBias[refFrame] = 0;
					}
					else 
					{
						RefFrameSignBias[refFrame] = ((GetRelativeDist(hint, OrderHint) > 0) ? 1 : 0);
					}
				}
			}

			if (((reduced_still_picture_header != 0) || (disable_cdf_update != 0)))
			{
				disable_frame_end_update_cdf = 1;
			}
			else 
			{
				stream.ReadFixed(1, out this.disable_frame_end_update_cdf, "disable_frame_end_update_cdf"); 
			}

			if ((primary_ref_frame == PRIMARY_REF_NONE))
			{
				init_non_coeff_cdfs(); 
				setup_past_independence(); 
			}
			else 
			{
				load_cdfs(ref_frame_idx[primary_ref_frame]); 
				load_previous(); 
			}

			if ((use_ref_frame_mvs == 1))
			{
				motion_field_estimation(); 
			}
			TileInfo(); 
			QuantizationParams(); 
			SegmentationParams(); 
			DeltaqParams(); 
			DeltaLfParams(); 

			if ((primary_ref_frame == PRIMARY_REF_NONE))
			{
				init_coeff_cdfs(); 
			}
			else 
			{
				load_previous_segment_ids(); 
			}
			CodedLossless = 1;

			for (segmentId = 0; (segmentId < MAX_SEGMENTS); segmentId++)
			{
				qindex = get_qindex(1, segmentId);
				LosslessArray[segmentId] = (((((((qindex == 0) && (DeltaQYDc == 0)) && (DeltaQUAc == 0)) && (DeltaQUDc == 0)) && (DeltaQVAc == 0)) && (DeltaQVDc == 0)) ? 1 : 0);

				if (!(LosslessArray[segmentId] != 0))
				{
					CodedLossless = 0;
				}

				if ((using_qmatrix != 0))
				{

					if ((LosslessArray[segmentId] != 0))
					{
						SegQMLevel[0][segmentId] = 15;
						SegQMLevel[1][segmentId] = 15;
						SegQMLevel[2][segmentId] = 15;
					}
					else 
					{
						SegQMLevel[0][segmentId] = qm_y;
						SegQMLevel[1][segmentId] = qm_u;
						SegQMLevel[2][segmentId] = qm_v;
					}
				}
			}
			AllLossless = (((CodedLossless != 0) && (FrameWidth == UpscaledWidth)) ? 1 : 0);
			LoopFilterParams(); 
			CdefParams(); 
			LrParams(); 
			ReadTxMode(); 
			FrameReferenceMode(); 
			SkipModeParams(); 

			if ((((FrameIsIntra != 0) || (error_resilient_mode != 0)) || !(enable_warped_motion != 0)))
			{
				allow_warped_motion = 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.allow_warped_motion, "allow_warped_motion"); 
			}
			stream.ReadFixed(1, out this.reduced_tx_set, "reduced_tx_set"); 
			GlobalMotionParams(); 
			FilmGrainParams(); 
        }

        private void WriteUncompressedHeader()
        {
			int i = 0;
			int opNum = 0;
			int segmentId = 0;
			int idLen = 0;
			int allFrames = 0;
			int opPtIdc = 0;
			int inTemporalLayer = 0;
			int inSpatialLayer = 0;
			int n = 0;
			AomArray<int> expectedFrameId = new AomArray<int>();
			int refFrame = 0;
			int hint = 0;
			int qindex = 0;

			if ((frame_id_numbers_present_flag != 0))
			{
				idLen = ((additional_frame_id_length_minus_1 + delta_frame_id_length_minus_2) + 3);
			}
			allFrames = ((1 << NUM_REF_FRAMES) - 1);

			if ((reduced_still_picture_header != 0))
			{
				show_existing_frame = 0;
				frame_type = KEY_FRAME;
				FrameIsIntra = 1;
				show_frame = 1;
				showable_frame = 0;
			}
			else 
			{
				this.show_existing_frame = stream.Pick("show_existing_frame", _original != null ? _original.show_existing_frame : this.show_existing_frame, _edited != null ? _edited.show_existing_frame : _original != null ? _original.show_existing_frame : this.show_existing_frame);
				stream.WriteFixed(1, this.show_existing_frame, "show_existing_frame"); 

				if ((show_existing_frame == 1))
				{
					this.frame_to_show_map_idx = stream.Pick("frame_to_show_map_idx", _original != null ? _original.frame_to_show_map_idx : this.frame_to_show_map_idx, _edited != null ? _edited.frame_to_show_map_idx : _original != null ? _original.frame_to_show_map_idx : this.frame_to_show_map_idx);
					stream.WriteFixed(3, this.frame_to_show_map_idx, "frame_to_show_map_idx"); 

					if (((decoder_model_info_present_flag != 0) && !(equal_picture_interval != 0)))
					{
						WriteTemporalPointInfo(); 
					}
					refresh_frame_flags = 0;

					if ((frame_id_numbers_present_flag != 0))
					{
						this.display_frame_id = stream.Pick("display_frame_id", _original != null ? _original.display_frame_id : this.display_frame_id, _edited != null ? _edited.display_frame_id : _original != null ? _original.display_frame_id : this.display_frame_id);
						stream.WriteVariable(idLen, this.display_frame_id, "display_frame_id"); 
					}
					frame_type = RefFrameType[frame_to_show_map_idx];

					if ((frame_type == KEY_FRAME))
					{
						refresh_frame_flags = allFrames;
					}

					if ((film_grain_params_present != 0))
					{
						load_grain_params(frame_to_show_map_idx); 
					}
					return;
				}
				this.frame_type = stream.Pick("frame_type", _original != null ? _original.frame_type : this.frame_type, _edited != null ? _edited.frame_type : _original != null ? _original.frame_type : this.frame_type);
				stream.WriteFixed(2, this.frame_type, "frame_type"); 
				FrameIsIntra = (((frame_type == INTRA_ONLY_FRAME) || (frame_type == KEY_FRAME)) ? 1 : 0);
				this.show_frame = stream.Pick("show_frame", _original != null ? _original.show_frame : this.show_frame, _edited != null ? _edited.show_frame : _original != null ? _original.show_frame : this.show_frame);
				stream.WriteFixed(1, this.show_frame, "show_frame"); 

				if ((((show_frame != 0) && (decoder_model_info_present_flag != 0)) && !(equal_picture_interval != 0)))
				{
					WriteTemporalPointInfo(); 
				}

				if ((show_frame != 0))
				{
					showable_frame = ((frame_type != KEY_FRAME) ? 1 : 0);
				}
				else 
				{
					this.showable_frame = stream.Pick("showable_frame", _original != null ? _original.showable_frame : this.showable_frame, _edited != null ? _edited.showable_frame : _original != null ? _original.showable_frame : this.showable_frame);
					stream.WriteFixed(1, this.showable_frame, "showable_frame"); 
				}

				if (((frame_type == SWITCH_FRAME) || ((frame_type == KEY_FRAME) && (show_frame != 0))))
				{
					error_resilient_mode = 1;
				}
				else 
				{
					this.error_resilient_mode = stream.Pick("error_resilient_mode", _original != null ? _original.error_resilient_mode : this.error_resilient_mode, _edited != null ? _edited.error_resilient_mode : _original != null ? _original.error_resilient_mode : this.error_resilient_mode);
					stream.WriteFixed(1, this.error_resilient_mode, "error_resilient_mode"); 
				}
			}

			if (((frame_type == KEY_FRAME) && (show_frame != 0)))
			{

				for (i = 0; (i < NUM_REF_FRAMES); i++)
				{
					RefValid[i] = 0;
					RefOrderHint[i] = 0;
				}

				for (i = 0; (i < REFS_PER_FRAME); i++)
				{
					OrderHints[(LAST_FRAME + i)] = 0;
				}
			}
			this.disable_cdf_update = stream.Pick("disable_cdf_update", _original != null ? _original.disable_cdf_update : this.disable_cdf_update, _edited != null ? _edited.disable_cdf_update : _original != null ? _original.disable_cdf_update : this.disable_cdf_update);
			stream.WriteFixed(1, this.disable_cdf_update, "disable_cdf_update"); 

			if ((seq_force_screen_content_tools == SELECT_SCREEN_CONTENT_TOOLS))
			{
				this.allow_screen_content_tools = stream.Pick("allow_screen_content_tools", _original != null ? _original.allow_screen_content_tools : this.allow_screen_content_tools, _edited != null ? _edited.allow_screen_content_tools : _original != null ? _original.allow_screen_content_tools : this.allow_screen_content_tools);
				stream.WriteFixed(1, this.allow_screen_content_tools, "allow_screen_content_tools"); 
			}
			else 
			{
				allow_screen_content_tools = seq_force_screen_content_tools;
			}

			if ((allow_screen_content_tools != 0))
			{

				if ((seq_force_integer_mv == SELECT_INTEGER_MV))
				{
					this.force_integer_mv = stream.Pick("force_integer_mv", _original != null ? _original.force_integer_mv : this.force_integer_mv, _edited != null ? _edited.force_integer_mv : _original != null ? _original.force_integer_mv : this.force_integer_mv);
					stream.WriteFixed(1, this.force_integer_mv, "force_integer_mv"); 
				}
				else 
				{
					force_integer_mv = seq_force_integer_mv;
				}
			}
			else 
			{
				force_integer_mv = 0;
			}

			if ((FrameIsIntra != 0))
			{
				force_integer_mv = 1;
			}

			if ((frame_id_numbers_present_flag != 0))
			{
				PrevFrameID = current_frame_id;
				this.current_frame_id = stream.Pick("current_frame_id", _original != null ? _original.current_frame_id : this.current_frame_id, _edited != null ? _edited.current_frame_id : _original != null ? _original.current_frame_id : this.current_frame_id);
				stream.WriteVariable(idLen, this.current_frame_id, "current_frame_id"); 
				MarkRefFrames(idLen); 
			}
			else 
			{
				current_frame_id = 0;
			}

			if ((frame_type == SWITCH_FRAME))
			{
				frame_size_override_flag = 1;
			}
			else if ((reduced_still_picture_header != 0))
			{
				frame_size_override_flag = 0;
			}
			else 
			{
				this.frame_size_override_flag = stream.Pick("frame_size_override_flag", _original != null ? _original.frame_size_override_flag : this.frame_size_override_flag, _edited != null ? _edited.frame_size_override_flag : _original != null ? _original.frame_size_override_flag : this.frame_size_override_flag);
				stream.WriteFixed(1, this.frame_size_override_flag, "frame_size_override_flag"); 
			}
			this.order_hint = stream.Pick("order_hint", _original != null ? _original.OrderHint : this.order_hint, _edited != null ? _edited.OrderHint : _original != null ? _original.OrderHint : this.order_hint);
			stream.WriteVariable(OrderHintBits, this.order_hint, "order_hint"); 
			OrderHint = order_hint;

			if (((FrameIsIntra != 0) || (error_resilient_mode != 0)))
			{
				primary_ref_frame = PRIMARY_REF_NONE;
			}
			else 
			{
				this.primary_ref_frame = stream.Pick("primary_ref_frame", _original != null ? _original.primary_ref_frame : this.primary_ref_frame, _edited != null ? _edited.primary_ref_frame : _original != null ? _original.primary_ref_frame : this.primary_ref_frame);
				stream.WriteFixed(3, this.primary_ref_frame, "primary_ref_frame"); 
			}

			if ((decoder_model_info_present_flag != 0))
			{
				this.buffer_removal_time_present_flag = stream.Pick("buffer_removal_time_present_flag", _original != null ? _original.buffer_removal_time_present_flag : this.buffer_removal_time_present_flag, _edited != null ? _edited.buffer_removal_time_present_flag : _original != null ? _original.buffer_removal_time_present_flag : this.buffer_removal_time_present_flag);
				stream.WriteFixed(1, this.buffer_removal_time_present_flag, "buffer_removal_time_present_flag"); 

				if ((buffer_removal_time_present_flag != 0))
				{

					for (opNum = 0; (opNum <= operating_points_cnt_minus_1); opNum++)
					{

						if ((decoder_model_present_for_this_op[opNum] != 0))
						{
							opPtIdc = operating_point_idc[opNum];
							inTemporalLayer = ((opPtIdc >> temporal_id) & 1);
							inSpatialLayer = ((opPtIdc >> (spatial_id + 8)) & 1);

							if (((opPtIdc == 0) || ((inTemporalLayer != 0) && (inSpatialLayer != 0))))
							{
								n = (buffer_removal_time_length_minus_1 + 1);
								this.buffer_removal_time[opNum] = stream.Pick("buffer_removal_time", _original != null ? _original.buffer_removal_time[opNum] : this.buffer_removal_time[opNum], _edited != null ? _edited.buffer_removal_time[opNum] : _original != null ? _original.buffer_removal_time[opNum] : this.buffer_removal_time[opNum]);
								stream.WriteVariable(n, this.buffer_removal_time[opNum], "buffer_removal_time"); 
							}
						}
					}
				}
			}
			allow_high_precision_mv = 0;
			use_ref_frame_mvs = 0;
			allow_intrabc = 0;

			if (((frame_type == SWITCH_FRAME) || ((frame_type == KEY_FRAME) && (show_frame != 0))))
			{
				refresh_frame_flags = allFrames;
			}
			else 
			{
				this.refresh_frame_flags = stream.Pick("refresh_frame_flags", _original != null ? _original.refresh_frame_flags : this.refresh_frame_flags, _edited != null ? _edited.refresh_frame_flags : _original != null ? _original.refresh_frame_flags : this.refresh_frame_flags);
				stream.WriteFixed(8, this.refresh_frame_flags, "refresh_frame_flags"); 
			}

			if ((!(FrameIsIntra != 0) || (refresh_frame_flags != allFrames)))
			{

				if (((error_resilient_mode != 0) && (enable_order_hint != 0)))
				{

					for (i = 0; (i < NUM_REF_FRAMES); i++)
					{
						this.ref_order_hint[i] = stream.Pick("ref_order_hint", _original != null ? _original.ref_order_hint[i] : this.ref_order_hint[i], _edited != null ? _edited.ref_order_hint[i] : _original != null ? _original.ref_order_hint[i] : this.ref_order_hint[i]);
						stream.WriteVariable(OrderHintBits, this.ref_order_hint[i], "ref_order_hint"); 

						if ((ref_order_hint[i] != RefOrderHint[i]))
						{
							RefValid[i] = 0;
						}
					}
				}
			}

			if ((FrameIsIntra != 0))
			{
				WriteFrameSize(); 
				WriteRenderSize(); 

				if (((allow_screen_content_tools != 0) && (UpscaledWidth == FrameWidth)))
				{
					this.allow_intrabc = stream.Pick("allow_intrabc", _original != null ? _original.allow_intrabc : this.allow_intrabc, _edited != null ? _edited.allow_intrabc : _original != null ? _original.allow_intrabc : this.allow_intrabc);
					stream.WriteFixed(1, this.allow_intrabc, "allow_intrabc"); 
				}
			}
			else 
			{

				if (!(enable_order_hint != 0))
				{
					frame_refs_short_signaling = 0;
				}
				else 
				{
					this.frame_refs_short_signaling = stream.Pick("frame_refs_short_signaling", _original != null ? _original.frame_refs_short_signaling : this.frame_refs_short_signaling, _edited != null ? _edited.frame_refs_short_signaling : _original != null ? _original.frame_refs_short_signaling : this.frame_refs_short_signaling);
					stream.WriteFixed(1, this.frame_refs_short_signaling, "frame_refs_short_signaling"); 

					if ((frame_refs_short_signaling != 0))
					{
						this.last_frame_idx = stream.Pick("last_frame_idx", _original != null ? _original.last_frame_idx : this.last_frame_idx, _edited != null ? _edited.last_frame_idx : _original != null ? _original.last_frame_idx : this.last_frame_idx);
						stream.WriteFixed(3, this.last_frame_idx, "last_frame_idx"); 
						this.gold_frame_idx = stream.Pick("gold_frame_idx", _original != null ? _original.gold_frame_idx : this.gold_frame_idx, _edited != null ? _edited.gold_frame_idx : _original != null ? _original.gold_frame_idx : this.gold_frame_idx);
						stream.WriteFixed(3, this.gold_frame_idx, "gold_frame_idx"); 
						SetFrameRefs(); 
					}
				}

				for (i = 0; (i < REFS_PER_FRAME); i++)
				{

					if (!(frame_refs_short_signaling != 0))
					{
						this.ref_frame_idx[i] = stream.Pick("ref_frame_idx", _original != null ? _original.ref_frame_idx[i] : this.ref_frame_idx[i], _edited != null ? _edited.ref_frame_idx[i] : _original != null ? _original.ref_frame_idx[i] : this.ref_frame_idx[i]);
						stream.WriteFixed(3, this.ref_frame_idx[i], "ref_frame_idx"); 
					}

					if ((frame_id_numbers_present_flag != 0))
					{
						n = (delta_frame_id_length_minus_2 + 2);
						this.delta_frame_id_minus_1 = stream.Pick("delta_frame_id_minus_1", _original != null ? (((_original.current_frame_id + (1 << idLen) - RefFrameId[ref_frame_idx[i]]) % (1 << idLen)) - 1) : this.delta_frame_id_minus_1, _edited != null ? (((_edited.current_frame_id + (1 << idLen) - RefFrameId[ref_frame_idx[i]]) % (1 << idLen)) - 1) : _original != null ? (((_original.current_frame_id + (1 << idLen) - RefFrameId[ref_frame_idx[i]]) % (1 << idLen)) - 1) : this.delta_frame_id_minus_1);
						stream.WriteVariable(n, this.delta_frame_id_minus_1, "delta_frame_id_minus_1"); 
						DeltaFrameId = (delta_frame_id_minus_1 + 1);
						expectedFrameId[i] = (((current_frame_id + (1 << idLen)) - DeltaFrameId) % (1 << idLen));
					}
				}

				if (((frame_size_override_flag != 0) && !(error_resilient_mode != 0)))
				{
					WriteFrameSizeWithRefs(); 
				}
				else 
				{
					WriteFrameSize(); 
					WriteRenderSize(); 
				}

				if ((force_integer_mv != 0))
				{
					allow_high_precision_mv = 0;
				}
				else 
				{
					this.allow_high_precision_mv = stream.Pick("allow_high_precision_mv", _original != null ? _original.allow_high_precision_mv : this.allow_high_precision_mv, _edited != null ? _edited.allow_high_precision_mv : _original != null ? _original.allow_high_precision_mv : this.allow_high_precision_mv);
					stream.WriteFixed(1, this.allow_high_precision_mv, "allow_high_precision_mv"); 
				}
				WriteReadInterpolationFilter(); 
				this.is_motion_mode_switchable = stream.Pick("is_motion_mode_switchable", _original != null ? _original.is_motion_mode_switchable : this.is_motion_mode_switchable, _edited != null ? _edited.is_motion_mode_switchable : _original != null ? _original.is_motion_mode_switchable : this.is_motion_mode_switchable);
				stream.WriteFixed(1, this.is_motion_mode_switchable, "is_motion_mode_switchable"); 

				if (((error_resilient_mode != 0) || !(enable_ref_frame_mvs != 0)))
				{
					use_ref_frame_mvs = 0;
				}
				else 
				{
					this.use_ref_frame_mvs = stream.Pick("use_ref_frame_mvs", _original != null ? _original.use_ref_frame_mvs : this.use_ref_frame_mvs, _edited != null ? _edited.use_ref_frame_mvs : _original != null ? _original.use_ref_frame_mvs : this.use_ref_frame_mvs);
					stream.WriteFixed(1, this.use_ref_frame_mvs, "use_ref_frame_mvs"); 
				}

				for (i = 0; (i < REFS_PER_FRAME); i++)
				{
					refFrame = (LAST_FRAME + i);
					hint = RefOrderHint[ref_frame_idx[i]];
					OrderHints[refFrame] = hint;

					if (!(enable_order_hint != 0))
					{
						RefFrameSignBias[refFrame] = 0;
					}
					else 
					{
						RefFrameSignBias[refFrame] = ((GetRelativeDist(hint, OrderHint) > 0) ? 1 : 0);
					}
				}
			}

			if (((reduced_still_picture_header != 0) || (disable_cdf_update != 0)))
			{
				disable_frame_end_update_cdf = 1;
			}
			else 
			{
				this.disable_frame_end_update_cdf = stream.Pick("disable_frame_end_update_cdf", _original != null ? _original.disable_frame_end_update_cdf : this.disable_frame_end_update_cdf, _edited != null ? _edited.disable_frame_end_update_cdf : _original != null ? _original.disable_frame_end_update_cdf : this.disable_frame_end_update_cdf);
				stream.WriteFixed(1, this.disable_frame_end_update_cdf, "disable_frame_end_update_cdf"); 
			}

			if ((primary_ref_frame == PRIMARY_REF_NONE))
			{
				init_non_coeff_cdfs(); 
				setup_past_independence(); 
			}
			else 
			{
				load_cdfs(ref_frame_idx[primary_ref_frame]); 
				load_previous(); 
			}

			if ((use_ref_frame_mvs == 1))
			{
				motion_field_estimation(); 
			}
			WriteTileInfo(); 
			WriteQuantizationParams(); 
			WriteSegmentationParams(); 
			WriteDeltaqParams(); 
			WriteDeltaLfParams(); 

			if ((primary_ref_frame == PRIMARY_REF_NONE))
			{
				init_coeff_cdfs(); 
			}
			else 
			{
				load_previous_segment_ids(); 
			}
			CodedLossless = 1;

			for (segmentId = 0; (segmentId < MAX_SEGMENTS); segmentId++)
			{
				qindex = get_qindex(1, segmentId);
				LosslessArray[segmentId] = (((((((qindex == 0) && (DeltaQYDc == 0)) && (DeltaQUAc == 0)) && (DeltaQUDc == 0)) && (DeltaQVAc == 0)) && (DeltaQVDc == 0)) ? 1 : 0);

				if (!(LosslessArray[segmentId] != 0))
				{
					CodedLossless = 0;
				}

				if ((using_qmatrix != 0))
				{

					if ((LosslessArray[segmentId] != 0))
					{
						SegQMLevel[0][segmentId] = 15;
						SegQMLevel[1][segmentId] = 15;
						SegQMLevel[2][segmentId] = 15;
					}
					else 
					{
						SegQMLevel[0][segmentId] = qm_y;
						SegQMLevel[1][segmentId] = qm_u;
						SegQMLevel[2][segmentId] = qm_v;
					}
				}
			}
			AllLossless = (((CodedLossless != 0) && (FrameWidth == UpscaledWidth)) ? 1 : 0);
			WriteLoopFilterParams(); 
			WriteCdefParams(); 
			WriteLrParams(); 
			WriteReadTxMode(); 
			WriteFrameReferenceMode(); 
			WriteSkipModeParams(); 

			if ((((FrameIsIntra != 0) || (error_resilient_mode != 0)) || !(enable_warped_motion != 0)))
			{
				allow_warped_motion = 0;
			}
			else 
			{
				this.allow_warped_motion = stream.Pick("allow_warped_motion", _original != null ? _original.allow_warped_motion : this.allow_warped_motion, _edited != null ? _edited.allow_warped_motion : _original != null ? _original.allow_warped_motion : this.allow_warped_motion);
				stream.WriteFixed(1, this.allow_warped_motion, "allow_warped_motion"); 
			}
			this.reduced_tx_set = stream.Pick("reduced_tx_set", _original != null ? _original.reduced_tx_set : this.reduced_tx_set, _edited != null ? _edited.reduced_tx_set : _original != null ? _original.reduced_tx_set : this.reduced_tx_set);
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
		public int _FramePresentationTime { get { return frame_presentation_time; } set { frame_presentation_time = value; } }

        private void TemporalPointInfo()
        {
			int n = 0;
			n = (frame_presentation_time_length_minus_1 + 1);
			stream.ReadVariable(n, out this.frame_presentation_time, "frame_presentation_time"); 
        }

        private void WriteTemporalPointInfo()
        {
			int n = 0;
			n = (frame_presentation_time_length_minus_1 + 1);
			this.frame_presentation_time = stream.Pick("frame_presentation_time", _original != null ? _original.frame_presentation_time : this.frame_presentation_time, _edited != null ? _edited.frame_presentation_time : _original != null ? _original.frame_presentation_time : this.frame_presentation_time);
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
		public int _FrameWidthMinus1 { get { return frame_width_minus_1; } set { frame_width_minus_1 = value; } }
		private int frame_height_minus_1;
		public int _FrameHeightMinus1 { get { return frame_height_minus_1; } set { frame_height_minus_1 = value; } }
		private int FrameWidth;
		public int _FrameWidth { get { return FrameWidth; } set { FrameWidth = value; } }
		private int FrameHeight;
		public int _FrameHeight { get { return FrameHeight; } set { FrameHeight = value; } }

        private void FrameSize()
        {
			int n = 0;

			if ((frame_size_override_flag != 0))
			{
				n = (frame_width_bits_minus_1 + 1);
				stream.ReadVariable(n, out this.frame_width_minus_1, "frame_width_minus_1"); 
				n = (frame_height_bits_minus_1 + 1);
				stream.ReadVariable(n, out this.frame_height_minus_1, "frame_height_minus_1"); 
				FrameWidth = (frame_width_minus_1 + 1);
				FrameHeight = (frame_height_minus_1 + 1);
			}
			else 
			{
				FrameWidth = (max_frame_width_minus_1 + 1);
				FrameHeight = (max_frame_height_minus_1 + 1);
			}
			SuperresParams(); 
			ComputeImageSize(); 
        }

        private void WriteFrameSize()
        {
			int n = 0;

			if ((frame_size_override_flag != 0))
			{
				n = (frame_width_bits_minus_1 + 1);
				this.frame_width_minus_1 = stream.Pick("frame_width_minus_1", _original != null ? _original.frame_width_minus_1 : this.frame_width_minus_1, _edited != null ? _edited.frame_width_minus_1 : _original != null ? _original.frame_width_minus_1 : this.frame_width_minus_1);
				stream.WriteVariable(n, this.frame_width_minus_1, "frame_width_minus_1"); 
				n = (frame_height_bits_minus_1 + 1);
				this.frame_height_minus_1 = stream.Pick("frame_height_minus_1", _original != null ? _original.frame_height_minus_1 : this.frame_height_minus_1, _edited != null ? _edited.frame_height_minus_1 : _original != null ? _original.frame_height_minus_1 : this.frame_height_minus_1);
				stream.WriteVariable(n, this.frame_height_minus_1, "frame_height_minus_1"); 
				FrameWidth = (frame_width_minus_1 + 1);
				FrameHeight = (frame_height_minus_1 + 1);
			}
			else 
			{
				FrameWidth = (max_frame_width_minus_1 + 1);
				FrameHeight = (max_frame_height_minus_1 + 1);
			}
			WriteSuperresParams(); 
			ComputeImageSize(); 
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
		public int _RenderAndFrameSizeDifferent { get { return render_and_frame_size_different; } set { render_and_frame_size_different = value; } }
		private int render_width_minus_1;
		public int _RenderWidthMinus1 { get { return render_width_minus_1; } set { render_width_minus_1 = value; } }
		private int render_height_minus_1;
		public int _RenderHeightMinus1 { get { return render_height_minus_1; } set { render_height_minus_1 = value; } }
		private int RenderWidth;
		public int _RenderWidth { get { return RenderWidth; } set { RenderWidth = value; } }
		private int RenderHeight;
		public int _RenderHeight { get { return RenderHeight; } set { RenderHeight = value; } }

        private void RenderSize()
        {
			stream.ReadFixed(1, out this.render_and_frame_size_different, "render_and_frame_size_different"); 

			if ((render_and_frame_size_different == 1))
			{
				stream.ReadFixed(16, out this.render_width_minus_1, "render_width_minus_1"); 
				stream.ReadFixed(16, out this.render_height_minus_1, "render_height_minus_1"); 
				RenderWidth = (render_width_minus_1 + 1);
				RenderHeight = (render_height_minus_1 + 1);
			}
			else 
			{
				RenderWidth = UpscaledWidth;
				RenderHeight = FrameHeight;
			}
        }

        private void WriteRenderSize()
        {
			this.render_and_frame_size_different = stream.Pick("render_and_frame_size_different", _original != null ? _original.render_and_frame_size_different : this.render_and_frame_size_different, _edited != null ? _edited.render_and_frame_size_different : _original != null ? _original.render_and_frame_size_different : this.render_and_frame_size_different);
			stream.WriteFixed(1, this.render_and_frame_size_different, "render_and_frame_size_different"); 

			if ((render_and_frame_size_different == 1))
			{
				this.render_width_minus_1 = stream.Pick("render_width_minus_1", _original != null ? _original.render_width_minus_1 : this.render_width_minus_1, _edited != null ? _edited.render_width_minus_1 : _original != null ? _original.render_width_minus_1 : this.render_width_minus_1);
				stream.WriteFixed(16, this.render_width_minus_1, "render_width_minus_1"); 
				this.render_height_minus_1 = stream.Pick("render_height_minus_1", _original != null ? _original.render_height_minus_1 : this.render_height_minus_1, _edited != null ? _edited.render_height_minus_1 : _original != null ? _original.render_height_minus_1 : this.render_height_minus_1);
				stream.WriteFixed(16, this.render_height_minus_1, "render_height_minus_1"); 
				RenderWidth = (render_width_minus_1 + 1);
				RenderHeight = (render_height_minus_1 + 1);
			}
			else 
			{
				RenderWidth = UpscaledWidth;
				RenderHeight = FrameHeight;
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
		public int _FoundRef { get { return found_ref; } set { found_ref = value; } }
		private int UpscaledWidth;
		public int _UpscaledWidth { get { return UpscaledWidth; } set { UpscaledWidth = value; } }

        private void FrameSizeWithRefs()
        {
			int i = 0;

			for (i = 0; (i < REFS_PER_FRAME); i++)
			{
				stream.ReadFixed(1, out this.found_ref, "found_ref"); 

				if ((found_ref == 1))
				{
					UpscaledWidth = RefUpscaledWidth[ref_frame_idx[i]];
					FrameWidth = UpscaledWidth;
					FrameHeight = RefFrameHeight[ref_frame_idx[i]];
					RenderWidth = RefRenderWidth[ref_frame_idx[i]];
					RenderHeight = RefRenderHeight[ref_frame_idx[i]];
					break;
				}
			}

			if ((found_ref == 0))
			{
				FrameSize(); 
				RenderSize(); 
			}
			else 
			{
				SuperresParams(); 
				ComputeImageSize(); 
			}
        }

        private void WriteFrameSizeWithRefs()
        {
			int i = 0;

			for (i = 0; (i < REFS_PER_FRAME); i++)
			{
				this.found_ref = stream.Pick("found_ref", _original != null ? (_original.UpscaledWidth == RefUpscaledWidth[ref_frame_idx[i]] && _original.FrameHeight == RefFrameHeight[ref_frame_idx[i]] && _original.RenderWidth == RefRenderWidth[ref_frame_idx[i]] && _original.RenderHeight == RefRenderHeight[ref_frame_idx[i]] ? 1 : 0) : this.found_ref, _edited != null ? (_edited.UpscaledWidth == RefUpscaledWidth[ref_frame_idx[i]] && _edited.FrameHeight == RefFrameHeight[ref_frame_idx[i]] && _edited.RenderWidth == RefRenderWidth[ref_frame_idx[i]] && _edited.RenderHeight == RefRenderHeight[ref_frame_idx[i]] ? 1 : 0) : _original != null ? (_original.UpscaledWidth == RefUpscaledWidth[ref_frame_idx[i]] && _original.FrameHeight == RefFrameHeight[ref_frame_idx[i]] && _original.RenderWidth == RefRenderWidth[ref_frame_idx[i]] && _original.RenderHeight == RefRenderHeight[ref_frame_idx[i]] ? 1 : 0) : this.found_ref);
				stream.WriteFixed(1, this.found_ref, "found_ref"); 

				if ((found_ref == 1))
				{
					UpscaledWidth = RefUpscaledWidth[ref_frame_idx[i]];
					FrameWidth = UpscaledWidth;
					FrameHeight = RefFrameHeight[ref_frame_idx[i]];
					RenderWidth = RefRenderWidth[ref_frame_idx[i]];
					RenderHeight = RefRenderHeight[ref_frame_idx[i]];
					break;
				}
			}

			if ((found_ref == 0))
			{
				WriteFrameSize(); 
				WriteRenderSize(); 
			}
			else 
			{
				WriteSuperresParams(); 
				ComputeImageSize(); 
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
		public int _IsFilterSwitchable { get { return is_filter_switchable; } set { is_filter_switchable = value; } }
		private int interpolation_filter;
		public int _InterpolationFilter { get { return interpolation_filter; } set { interpolation_filter = value; } }

        private void ReadInterpolationFilter()
        {
			stream.ReadFixed(1, out this.is_filter_switchable, "is_filter_switchable"); 

			if ((is_filter_switchable == 1))
			{
				interpolation_filter = SWITCHABLE;
			}
			else 
			{
				stream.ReadFixed(2, out this.interpolation_filter, "interpolation_filter"); 
			}
        }

        private void WriteReadInterpolationFilter()
        {
			this.is_filter_switchable = stream.Pick("is_filter_switchable", _original != null ? _original.is_filter_switchable : this.is_filter_switchable, _edited != null ? _edited.is_filter_switchable : _original != null ? _original.is_filter_switchable : this.is_filter_switchable);
			stream.WriteFixed(1, this.is_filter_switchable, "is_filter_switchable"); 

			if ((is_filter_switchable == 1))
			{
				interpolation_filter = SWITCHABLE;
			}
			else 
			{
				this.interpolation_filter = stream.Pick("interpolation_filter", _original != null ? _original.interpolation_filter : this.interpolation_filter, _edited != null ? _edited.interpolation_filter : _original != null ? _original.interpolation_filter : this.interpolation_filter);
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

        private int GetRelativeDist(int a, int b)
        {
			int diff = 0;
			int m = 0;

			if (!(enable_order_hint != 0))
			{
				return 0;
			}
			diff = (a - b);
			m = (1 << (OrderHintBits - 1));
			diff = ((diff & (m - 1)) - (diff & m));
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
 for ( i = 0; startSb < sbCols && i < MAX_TILE_COLS; i++ ) {
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
 maxTileHeightSb = Max( maxTileAreaSb / Max( widestTileSb, 1 ), 1 )
 startSb = 0
 for ( i = 0; startSb < sbRows && i < MAX_TILE_ROWS; i++ ) {
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
		private int uniform_tile_spacing_flag;
		public int _UniformTileSpacingFlag { get { return uniform_tile_spacing_flag; } set { uniform_tile_spacing_flag = value; } }
		private int TileColsLog2;
		public int _TileColsLog2 { get { return TileColsLog2; } set { TileColsLog2 = value; } }
		private int increment_tile_cols_log2;
		public int _IncrementTileColsLog2 { get { return increment_tile_cols_log2; } set { increment_tile_cols_log2 = value; } }
		private AomArray<int> MiColStarts = new AomArray<int>();
		public AomArray<int> _MiColStarts { get { return MiColStarts; } set { MiColStarts = value; } }
		private int TileCols;
		public int _TileCols { get { return TileCols; } set { TileCols = value; } }
		private int TileRowsLog2;
		public int _TileRowsLog2 { get { return TileRowsLog2; } set { TileRowsLog2 = value; } }
		private int increment_tile_rows_log2;
		public int _IncrementTileRowsLog2 { get { return increment_tile_rows_log2; } set { increment_tile_rows_log2 = value; } }
		private AomArray<int> MiRowStarts = new AomArray<int>();
		public AomArray<int> _MiRowStarts { get { return MiRowStarts; } set { MiRowStarts = value; } }
		private int TileRows;
		public int _TileRows { get { return TileRows; } set { TileRows = value; } }
		private int width_in_sbs_minus_1;
		public int _WidthInSbsMinus1 { get { return width_in_sbs_minus_1; } set { width_in_sbs_minus_1 = value; } }
		private int height_in_sbs_minus_1;
		public int _HeightInSbsMinus1 { get { return height_in_sbs_minus_1; } set { height_in_sbs_minus_1 = value; } }
		private int context_update_tile_id;
		public int _ContextUpdateTileId { get { return context_update_tile_id; } set { context_update_tile_id = value; } }
		private int tile_size_bytes_minus_1;
		public int _TileSizeBytesMinus1 { get { return tile_size_bytes_minus_1; } set { tile_size_bytes_minus_1 = value; } }
		private int TileSizeBytes;
		public int _TileSizeBytes { get { return TileSizeBytes; } set { TileSizeBytes = value; } }
		private int startSb = 0;

        private void TileInfo()
        {
			int startSb = 0;
			int i = 0;
			int sbCols = 0;
			int sbRows = 0;
			int sbShift = 0;
			int sbSize = 0;
			int maxTileWidthSb = 0;
			int maxTileAreaSb = 0;
			int minLog2TileCols = 0;
			int maxLog2TileCols = 0;
			int maxLog2TileRows = 0;
			int minLog2Tiles = 0;
			int tileWidthSb = 0;
			int minLog2TileRows = 0;
			int tileHeightSb = 0;
			int widestTileSb = 0;
			int maxWidth = 0;
			int sizeSb = 0;
			int maxTileHeightSb = 0;
			int maxHeight = 0;
			sbCols = ((use_128x128_superblock != 0) ? ((MiCols + 31) >> 5) : ((MiCols + 15) >> 4));
			sbRows = ((use_128x128_superblock != 0) ? ((MiRows + 31) >> 5) : ((MiRows + 15) >> 4));
			sbShift = ((use_128x128_superblock != 0) ? 5 : 4);
			sbSize = (sbShift + 2);
			maxTileWidthSb = (MAX_TILE_WIDTH >> sbSize);
			maxTileAreaSb = (MAX_TILE_AREA >> (2 * sbSize));
			minLog2TileCols = TileLog2(maxTileWidthSb, sbCols);
			maxLog2TileCols = TileLog2(1, Min(sbCols, MAX_TILE_COLS));
			maxLog2TileRows = TileLog2(1, Min(sbRows, MAX_TILE_ROWS));
			minLog2Tiles = Max(minLog2TileCols, TileLog2(maxTileAreaSb, (sbRows * sbCols)));
			stream.ReadFixed(1, out this.uniform_tile_spacing_flag, "uniform_tile_spacing_flag"); 

			if ((uniform_tile_spacing_flag != 0))
			{
				TileColsLog2 = minLog2TileCols;

				while ((TileColsLog2 < maxLog2TileCols))
				{
					stream.ReadFixed(1, out this.increment_tile_cols_log2, "increment_tile_cols_log2"); 

					if ((increment_tile_cols_log2 == 1))
					{
						TileColsLog2++;
					}
					else 
					{
						break;
					}
				}
				tileWidthSb = (((sbCols + (1 << TileColsLog2)) - 1) >> TileColsLog2);
				i = 0;

				for (startSb = 0; (startSb < sbCols); startSb += tileWidthSb)
				{
					MiColStarts[i] = (startSb << sbShift);
					i += 1;
				}
				MiColStarts[i] = MiCols;
				TileCols = i;
				minLog2TileRows = Max((minLog2Tiles - TileColsLog2), 0);
				TileRowsLog2 = minLog2TileRows;

				while ((TileRowsLog2 < maxLog2TileRows))
				{
					stream.ReadFixed(1, out this.increment_tile_rows_log2, "increment_tile_rows_log2"); 

					if ((increment_tile_rows_log2 == 1))
					{
						TileRowsLog2++;
					}
					else 
					{
						break;
					}
				}
				tileHeightSb = (((sbRows + (1 << TileRowsLog2)) - 1) >> TileRowsLog2);
				i = 0;

				for (startSb = 0; (startSb < sbRows); startSb += tileHeightSb)
				{
					MiRowStarts[i] = (startSb << sbShift);
					i += 1;
				}
				MiRowStarts[i] = MiRows;
				TileRows = i;
			}
			else 
			{
				widestTileSb = 0;
				startSb = 0;

				for (i = 0; ((startSb < sbCols) && (i < MAX_TILE_COLS)); i++)
				{
					MiColStarts[i] = (startSb << sbShift);
					maxWidth = Min((sbCols - startSb), maxTileWidthSb);
					stream.Read_ns(maxWidth, out this.width_in_sbs_minus_1, "width_in_sbs_minus_1"); 
					sizeSb = (width_in_sbs_minus_1 + 1);
					widestTileSb = Max(sizeSb, widestTileSb);
					startSb += sizeSb;
				}
				MiColStarts[i] = MiCols;
				TileCols = i;
				TileColsLog2 = TileLog2(1, TileCols);

				if ((minLog2Tiles > 0))
				{
					maxTileAreaSb = ((sbRows * sbCols) >> (minLog2Tiles + 1));
				}
				else 
				{
					maxTileAreaSb = (sbRows * sbCols);
				}
				maxTileHeightSb = Max((maxTileAreaSb / Max(widestTileSb, 1)), 1);
				startSb = 0;

				for (i = 0; ((startSb < sbRows) && (i < MAX_TILE_ROWS)); i++)
				{
					MiRowStarts[i] = (startSb << sbShift);
					maxHeight = Min((sbRows - startSb), maxTileHeightSb);
					stream.Read_ns(maxHeight, out this.height_in_sbs_minus_1, "height_in_sbs_minus_1"); 
					sizeSb = (height_in_sbs_minus_1 + 1);
					startSb += sizeSb;
				}
				MiRowStarts[i] = MiRows;
				TileRows = i;
				TileRowsLog2 = TileLog2(1, TileRows);
			}

			if (((TileColsLog2 > 0) || (TileRowsLog2 > 0)))
			{
				stream.ReadVariable((TileRowsLog2 + TileColsLog2), out this.context_update_tile_id, "context_update_tile_id"); 
				stream.ReadFixed(2, out this.tile_size_bytes_minus_1, "tile_size_bytes_minus_1"); 
				TileSizeBytes = (tile_size_bytes_minus_1 + 1);
			}
			else 
			{
				context_update_tile_id = 0;
			}
        }

        private void WriteTileInfo()
        {
			int startSb = 0;
			int i = 0;
			int sbCols = 0;
			int sbRows = 0;
			int sbShift = 0;
			int sbSize = 0;
			int maxTileWidthSb = 0;
			int maxTileAreaSb = 0;
			int minLog2TileCols = 0;
			int maxLog2TileCols = 0;
			int maxLog2TileRows = 0;
			int minLog2Tiles = 0;
			int tileWidthSb = 0;
			int minLog2TileRows = 0;
			int tileHeightSb = 0;
			int widestTileSb = 0;
			int maxWidth = 0;
			int sizeSb = 0;
			int maxTileHeightSb = 0;
			int maxHeight = 0;
			sbCols = ((use_128x128_superblock != 0) ? ((MiCols + 31) >> 5) : ((MiCols + 15) >> 4));
			sbRows = ((use_128x128_superblock != 0) ? ((MiRows + 31) >> 5) : ((MiRows + 15) >> 4));
			sbShift = ((use_128x128_superblock != 0) ? 5 : 4);
			sbSize = (sbShift + 2);
			maxTileWidthSb = (MAX_TILE_WIDTH >> sbSize);
			maxTileAreaSb = (MAX_TILE_AREA >> (2 * sbSize));
			minLog2TileCols = TileLog2(maxTileWidthSb, sbCols);
			maxLog2TileCols = TileLog2(1, Min(sbCols, MAX_TILE_COLS));
			maxLog2TileRows = TileLog2(1, Min(sbRows, MAX_TILE_ROWS));
			minLog2Tiles = Max(minLog2TileCols, TileLog2(maxTileAreaSb, (sbRows * sbCols)));
			this.uniform_tile_spacing_flag = stream.Pick("uniform_tile_spacing_flag", _original != null ? _original.uniform_tile_spacing_flag : this.uniform_tile_spacing_flag, _edited != null ? _edited.uniform_tile_spacing_flag : _original != null ? _original.uniform_tile_spacing_flag : this.uniform_tile_spacing_flag);
			stream.WriteFixed(1, this.uniform_tile_spacing_flag, "uniform_tile_spacing_flag"); 

			if ((uniform_tile_spacing_flag != 0))
			{
				TileColsLog2 = minLog2TileCols;

				while ((TileColsLog2 < maxLog2TileCols))
				{
					this.increment_tile_cols_log2 = stream.Pick("increment_tile_cols_log2", _original != null ? (_original.TileColsLog2 > TileColsLog2 ? 1 : 0) : this.increment_tile_cols_log2, _edited != null ? (_edited.TileColsLog2 > TileColsLog2 ? 1 : 0) : _original != null ? (_original.TileColsLog2 > TileColsLog2 ? 1 : 0) : this.increment_tile_cols_log2);
					stream.WriteFixed(1, this.increment_tile_cols_log2, "increment_tile_cols_log2"); 

					if ((increment_tile_cols_log2 == 1))
					{
						TileColsLog2++;
					}
					else 
					{
						break;
					}
				}
				tileWidthSb = (((sbCols + (1 << TileColsLog2)) - 1) >> TileColsLog2);
				i = 0;

				for (startSb = 0; (startSb < sbCols); startSb += tileWidthSb)
				{
					MiColStarts[i] = (startSb << sbShift);
					i += 1;
				}
				MiColStarts[i] = MiCols;
				TileCols = i;
				minLog2TileRows = Max((minLog2Tiles - TileColsLog2), 0);
				TileRowsLog2 = minLog2TileRows;

				while ((TileRowsLog2 < maxLog2TileRows))
				{
					this.increment_tile_rows_log2 = stream.Pick("increment_tile_rows_log2", _original != null ? (_original.TileRowsLog2 > TileRowsLog2 ? 1 : 0) : this.increment_tile_rows_log2, _edited != null ? (_edited.TileRowsLog2 > TileRowsLog2 ? 1 : 0) : _original != null ? (_original.TileRowsLog2 > TileRowsLog2 ? 1 : 0) : this.increment_tile_rows_log2);
					stream.WriteFixed(1, this.increment_tile_rows_log2, "increment_tile_rows_log2"); 

					if ((increment_tile_rows_log2 == 1))
					{
						TileRowsLog2++;
					}
					else 
					{
						break;
					}
				}
				tileHeightSb = (((sbRows + (1 << TileRowsLog2)) - 1) >> TileRowsLog2);
				i = 0;

				for (startSb = 0; (startSb < sbRows); startSb += tileHeightSb)
				{
					MiRowStarts[i] = (startSb << sbShift);
					i += 1;
				}
				MiRowStarts[i] = MiRows;
				TileRows = i;
			}
			else 
			{
				widestTileSb = 0;
				startSb = 0;

				for (i = 0; ((startSb < sbCols) && (i < MAX_TILE_COLS)); i++)
				{
					MiColStarts[i] = (startSb << sbShift);
					maxWidth = Min((sbCols - startSb), maxTileWidthSb);
					this.width_in_sbs_minus_1 = stream.Pick("width_in_sbs_minus_1", _original != null ? ((i + 1 < _original.TileCols ? _original.MiColStarts[i + 1] >> sbShift : sbCols) - (_original.MiColStarts[i] >> sbShift) - 1) : this.width_in_sbs_minus_1, _edited != null ? ((i + 1 < _edited.TileCols ? _edited.MiColStarts[i + 1] >> sbShift : sbCols) - (_edited.MiColStarts[i] >> sbShift) - 1) : _original != null ? ((i + 1 < _original.TileCols ? _original.MiColStarts[i + 1] >> sbShift : sbCols) - (_original.MiColStarts[i] >> sbShift) - 1) : this.width_in_sbs_minus_1);
					stream.Write_ns(maxWidth, this.width_in_sbs_minus_1, "width_in_sbs_minus_1"); 
					sizeSb = (width_in_sbs_minus_1 + 1);
					widestTileSb = Max(sizeSb, widestTileSb);
					startSb += sizeSb;
				}
				MiColStarts[i] = MiCols;
				TileCols = i;
				TileColsLog2 = TileLog2(1, TileCols);

				if ((minLog2Tiles > 0))
				{
					maxTileAreaSb = ((sbRows * sbCols) >> (minLog2Tiles + 1));
				}
				else 
				{
					maxTileAreaSb = (sbRows * sbCols);
				}
				maxTileHeightSb = Max((maxTileAreaSb / Max(widestTileSb, 1)), 1);
				startSb = 0;

				for (i = 0; ((startSb < sbRows) && (i < MAX_TILE_ROWS)); i++)
				{
					MiRowStarts[i] = (startSb << sbShift);
					maxHeight = Min((sbRows - startSb), maxTileHeightSb);
					this.height_in_sbs_minus_1 = stream.Pick("height_in_sbs_minus_1", _original != null ? ((i + 1 < _original.TileRows ? _original.MiRowStarts[i + 1] >> sbShift : sbRows) - (_original.MiRowStarts[i] >> sbShift) - 1) : this.height_in_sbs_minus_1, _edited != null ? ((i + 1 < _edited.TileRows ? _edited.MiRowStarts[i + 1] >> sbShift : sbRows) - (_edited.MiRowStarts[i] >> sbShift) - 1) : _original != null ? ((i + 1 < _original.TileRows ? _original.MiRowStarts[i + 1] >> sbShift : sbRows) - (_original.MiRowStarts[i] >> sbShift) - 1) : this.height_in_sbs_minus_1);
					stream.Write_ns(maxHeight, this.height_in_sbs_minus_1, "height_in_sbs_minus_1"); 
					sizeSb = (height_in_sbs_minus_1 + 1);
					startSb += sizeSb;
				}
				MiRowStarts[i] = MiRows;
				TileRows = i;
				TileRowsLog2 = TileLog2(1, TileRows);
			}

			if (((TileColsLog2 > 0) || (TileRowsLog2 > 0)))
			{
				this.context_update_tile_id = stream.Pick("context_update_tile_id", _original != null ? _original.context_update_tile_id : this.context_update_tile_id, _edited != null ? _edited.context_update_tile_id : _original != null ? _original.context_update_tile_id : this.context_update_tile_id);
				stream.WriteVariable((TileRowsLog2 + TileColsLog2), this.context_update_tile_id, "context_update_tile_id"); 
				this.tile_size_bytes_minus_1 = stream.Pick("tile_size_bytes_minus_1", _original != null ? _original.tile_size_bytes_minus_1 : this.tile_size_bytes_minus_1, _edited != null ? _edited.tile_size_bytes_minus_1 : _original != null ? _original.tile_size_bytes_minus_1 : this.tile_size_bytes_minus_1);
				stream.WriteFixed(2, this.tile_size_bytes_minus_1, "tile_size_bytes_minus_1"); 
				TileSizeBytes = (tile_size_bytes_minus_1 + 1);
			}
			else 
			{
				context_update_tile_id = 0;
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
		public int _BlkSize { get { return blkSize; } set { blkSize = value; } }
		private int target;
		public int _Target { get { return target; } set { target = value; } }
		private int k = 0;

        private int TileLog2(int blkSize, int target)
        {
			int k = 0;

			for (k = 0; ((blkSize << k) < target); k++)
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
		public int _BaseqIdx { get { return base_q_idx; } set { base_q_idx = value; } }
		private int DeltaQYDc;
		public int _DeltaQYDc { get { return DeltaQYDc; } set { DeltaQYDc = value; } }
		private int diff_uv_delta;
		public int _DiffUvDelta { get { return diff_uv_delta; } set { diff_uv_delta = value; } }
		private int DeltaQUDc;
		public int _DeltaQUDc { get { return DeltaQUDc; } set { DeltaQUDc = value; } }
		private int DeltaQUAc;
		public int _DeltaQUAc { get { return DeltaQUAc; } set { DeltaQUAc = value; } }
		private int DeltaQVDc;
		public int _DeltaQVDc { get { return DeltaQVDc; } set { DeltaQVDc = value; } }
		private int DeltaQVAc;
		public int _DeltaQVAc { get { return DeltaQVAc; } set { DeltaQVAc = value; } }
		private int using_qmatrix;
		public int _UsingQmatrix { get { return using_qmatrix; } set { using_qmatrix = value; } }
		private int qm_y;
		public int _Qmy { get { return qm_y; } set { qm_y = value; } }
		private int qm_u;
		public int _Qmu { get { return qm_u; } set { qm_u = value; } }
		private int qm_v;
		public int _Qmv { get { return qm_v; } set { qm_v = value; } }

        private void QuantizationParams()
        {
			stream.ReadFixed(8, out this.base_q_idx, "base_q_idx"); 
			DeltaQYDc = ReadDeltaq();

			if ((NumPlanes > 1))
			{

				if ((separate_uv_delta_q != 0))
				{
					stream.ReadFixed(1, out this.diff_uv_delta, "diff_uv_delta"); 
				}
				else 
				{
					diff_uv_delta = 0;
				}
				DeltaQUDc = ReadDeltaq();
				DeltaQUAc = ReadDeltaq();

				if ((diff_uv_delta != 0))
				{
					DeltaQVDc = ReadDeltaq();
					DeltaQVAc = ReadDeltaq();
				}
				else 
				{
					DeltaQVDc = DeltaQUDc;
					DeltaQVAc = DeltaQUAc;
				}
			}
			else 
			{
				DeltaQUDc = 0;
				DeltaQUAc = 0;
				DeltaQVDc = 0;
				DeltaQVAc = 0;
			}
			stream.ReadFixed(1, out this.using_qmatrix, "using_qmatrix"); 

			if ((using_qmatrix != 0))
			{
				stream.ReadFixed(4, out this.qm_y, "qm_y"); 
				stream.ReadFixed(4, out this.qm_u, "qm_u"); 

				if (!(separate_uv_delta_q != 0))
				{
					qm_v = qm_u;
				}
				else 
				{
					stream.ReadFixed(4, out this.qm_v, "qm_v"); 
				}
			}
        }

        private void WriteQuantizationParams()
        {
			this.base_q_idx = stream.Pick("base_q_idx", _original != null ? _original.base_q_idx : this.base_q_idx, _edited != null ? _edited.base_q_idx : _original != null ? _original.base_q_idx : this.base_q_idx);
			stream.WriteFixed(8, this.base_q_idx, "base_q_idx"); 
			DeltaQYDc = WriteReadDeltaq();

			if ((NumPlanes > 1))
			{

				if ((separate_uv_delta_q != 0))
				{
					this.diff_uv_delta = stream.Pick("diff_uv_delta", _original != null ? _original.diff_uv_delta : this.diff_uv_delta, _edited != null ? _edited.diff_uv_delta : _original != null ? _original.diff_uv_delta : this.diff_uv_delta);
					stream.WriteFixed(1, this.diff_uv_delta, "diff_uv_delta"); 
				}
				else 
				{
					diff_uv_delta = 0;
				}
				DeltaQUDc = WriteReadDeltaq();
				DeltaQUAc = WriteReadDeltaq();

				if ((diff_uv_delta != 0))
				{
					DeltaQVDc = WriteReadDeltaq();
					DeltaQVAc = WriteReadDeltaq();
				}
				else 
				{
					DeltaQVDc = DeltaQUDc;
					DeltaQVAc = DeltaQUAc;
				}
			}
			else 
			{
				DeltaQUDc = 0;
				DeltaQUAc = 0;
				DeltaQVDc = 0;
				DeltaQVAc = 0;
			}
			this.using_qmatrix = stream.Pick("using_qmatrix", _original != null ? _original.using_qmatrix : this.using_qmatrix, _edited != null ? _edited.using_qmatrix : _original != null ? _original.using_qmatrix : this.using_qmatrix);
			stream.WriteFixed(1, this.using_qmatrix, "using_qmatrix"); 

			if ((using_qmatrix != 0))
			{
				this.qm_y = stream.Pick("qm_y", _original != null ? _original.qm_y : this.qm_y, _edited != null ? _edited.qm_y : _original != null ? _original.qm_y : this.qm_y);
				stream.WriteFixed(4, this.qm_y, "qm_y"); 
				this.qm_u = stream.Pick("qm_u", _original != null ? _original.qm_u : this.qm_u, _edited != null ? _edited.qm_u : _original != null ? _original.qm_u : this.qm_u);
				stream.WriteFixed(4, this.qm_u, "qm_u"); 

				if (!(separate_uv_delta_q != 0))
				{
					qm_v = qm_u;
				}
				else 
				{
					this.qm_v = stream.Pick("qm_v", _original != null ? _original.qm_v : this.qm_v, _edited != null ? _edited.qm_v : _original != null ? _original.qm_v : this.qm_v);
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
		public int _DeltaCoded { get { return delta_coded; } set { delta_coded = value; } }
		private int delta_q;
		public int _Deltaq { get { return delta_q; } set { delta_q = value; } }

        private int ReadDeltaq()
        {
			stream.ReadFixed(1, out this.delta_coded, "delta_coded"); 

			if ((delta_coded != 0))
			{
				stream.ReadSignedIntVar((1 + 6), out this.delta_q, "delta_q"); 
			}
			else 
			{
				delta_q = 0;
			}
			return delta_q;
        }

        private int WriteReadDeltaq()
        {
			this.delta_coded = stream.Pick("delta_coded", _original != null ? _original.delta_coded : this.delta_coded, _edited != null ? _edited.delta_coded : _original != null ? _original.delta_coded : this.delta_coded);
			stream.WriteFixed(1, this.delta_coded, "delta_coded"); 

			if ((delta_coded != 0))
			{
				this.delta_q = stream.Pick("delta_q", _original != null ? _original.delta_q : this.delta_q, _edited != null ? _edited.delta_q : _original != null ? _original.delta_q : this.delta_q);
				stream.WriteSignedIntVar((1 + 6), this.delta_q, "delta_q"); 
			}
			else 
			{
				delta_q = 0;
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
		public int _SegmentationEnabled { get { return segmentation_enabled; } set { segmentation_enabled = value; } }
		private int segmentation_update_map;
		public int _SegmentationUpdateMap { get { return segmentation_update_map; } set { segmentation_update_map = value; } }
		private int segmentation_temporal_update;
		public int _SegmentationTemporalUpdate { get { return segmentation_temporal_update; } set { segmentation_temporal_update = value; } }
		private int segmentation_update_data;
		public int _SegmentationUpdateData { get { return segmentation_update_data; } set { segmentation_update_data = value; } }
		private int feature_value;
		public int _FeatureValue { get { return feature_value; } set { feature_value = value; } }
		private int feature_enabled;
		public int _FeatureEnabled { get { return feature_enabled; } set { feature_enabled = value; } }
		private AomArray<AomArray<int>> FeatureEnabled = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> __FeatureEnabled { get { return FeatureEnabled; } set { FeatureEnabled = value; } }
		private AomArray<AomArray<int>> FeatureData = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _FeatureData { get { return FeatureData; } set { FeatureData = value; } }
		private int SegIdPreSkip;
		public int _SegIdPreSkip { get { return SegIdPreSkip; } set { SegIdPreSkip = value; } }
		private int LastActiveSegId;
		public int _LastActiveSegId { get { return LastActiveSegId; } set { LastActiveSegId = value; } }
		private int j = 0;

        private void SegmentationParams()
        {
			int i = 0;
			int j = 0;
			int clippedValue = 0;
			int bitsToRead = 0;
			int limit = 0;
			stream.ReadFixed(1, out this.segmentation_enabled, "segmentation_enabled"); 

			if ((segmentation_enabled == 1))
			{

				if ((primary_ref_frame == PRIMARY_REF_NONE))
				{
					segmentation_update_map = 1;
					segmentation_temporal_update = 0;
					segmentation_update_data = 1;
				}
				else 
				{
					stream.ReadFixed(1, out this.segmentation_update_map, "segmentation_update_map"); 

					if ((segmentation_update_map == 1))
					{
						stream.ReadFixed(1, out this.segmentation_temporal_update, "segmentation_temporal_update"); 
					}
					stream.ReadFixed(1, out this.segmentation_update_data, "segmentation_update_data"); 
				}

				if ((segmentation_update_data == 1))
				{

					for (i = 0; (i < MAX_SEGMENTS); i++)
					{

						for (j = 0; (j < SEG_LVL_MAX); j++)
						{
							feature_value = 0;
							stream.ReadFixed(1, out this.feature_enabled, "feature_enabled"); 
							FeatureEnabled[i][j] = feature_enabled;
							clippedValue = 0;

							if ((feature_enabled == 1))
							{
								bitsToRead = Segmentation_Feature_Bits[j];
								limit = Segmentation_Feature_Max[j];

								if ((Segmentation_Feature_Signed[j] == 1))
								{
									stream.ReadSignedIntVar((1 + bitsToRead), out this.feature_value, "feature_value"); 
									clippedValue = Clip3(-limit, limit, feature_value);
								}
								else 
								{
									stream.ReadVariable(bitsToRead, out this.feature_value, "feature_value"); 
									clippedValue = Clip3(0, limit, feature_value);
								}
							}
							FeatureData[i][j] = clippedValue;
						}
					}
				}
			}
			else 
			{

				for (i = 0; (i < MAX_SEGMENTS); i++)
				{

					for (j = 0; (j < SEG_LVL_MAX); j++)
					{
						FeatureEnabled[i][j] = 0;
						FeatureData[i][j] = 0;
					}
				}
			}
			SegIdPreSkip = 0;
			LastActiveSegId = 0;

			for (i = 0; (i < MAX_SEGMENTS); i++)
			{

				for (j = 0; (j < SEG_LVL_MAX); j++)
				{

					if ((FeatureEnabled[i][j] != 0))
					{
						LastActiveSegId = i;

						if ((j >= SEG_LVL_REF_FRAME))
						{
							SegIdPreSkip = 1;
						}
					}
				}
			}
        }

        private void WriteSegmentationParams()
        {
			int i = 0;
			int j = 0;
			int clippedValue = 0;
			int bitsToRead = 0;
			int limit = 0;
			this.segmentation_enabled = stream.Pick("segmentation_enabled", _original != null ? _original.segmentation_enabled : this.segmentation_enabled, _edited != null ? _edited.segmentation_enabled : _original != null ? _original.segmentation_enabled : this.segmentation_enabled);
			stream.WriteFixed(1, this.segmentation_enabled, "segmentation_enabled"); 

			if ((segmentation_enabled == 1))
			{

				if ((primary_ref_frame == PRIMARY_REF_NONE))
				{
					segmentation_update_map = 1;
					segmentation_temporal_update = 0;
					segmentation_update_data = 1;
				}
				else 
				{
					this.segmentation_update_map = stream.Pick("segmentation_update_map", _original != null ? _original.segmentation_update_map : this.segmentation_update_map, _edited != null ? _edited.segmentation_update_map : _original != null ? _original.segmentation_update_map : this.segmentation_update_map);
					stream.WriteFixed(1, this.segmentation_update_map, "segmentation_update_map"); 

					if ((segmentation_update_map == 1))
					{
						this.segmentation_temporal_update = stream.Pick("segmentation_temporal_update", _original != null ? _original.segmentation_temporal_update : this.segmentation_temporal_update, _edited != null ? _edited.segmentation_temporal_update : _original != null ? _original.segmentation_temporal_update : this.segmentation_temporal_update);
						stream.WriteFixed(1, this.segmentation_temporal_update, "segmentation_temporal_update"); 
					}
					this.segmentation_update_data = stream.Pick("segmentation_update_data", _original != null ? _original.segmentation_update_data : this.segmentation_update_data, _edited != null ? _edited.segmentation_update_data : _original != null ? _original.segmentation_update_data : this.segmentation_update_data);
					stream.WriteFixed(1, this.segmentation_update_data, "segmentation_update_data"); 
				}

				if ((segmentation_update_data == 1))
				{

					for (i = 0; (i < MAX_SEGMENTS); i++)
					{

						for (j = 0; (j < SEG_LVL_MAX); j++)
						{
							feature_value = 0;
							this.feature_enabled = stream.Pick("feature_enabled", _original != null ? _original.FeatureEnabled[i][j] : this.feature_enabled, _edited != null ? _edited.FeatureEnabled[i][j] : _original != null ? _original.FeatureEnabled[i][j] : this.feature_enabled);
							stream.WriteFixed(1, this.feature_enabled, "feature_enabled"); 
							FeatureEnabled[i][j] = feature_enabled;
							clippedValue = 0;

							if ((feature_enabled == 1))
							{
								bitsToRead = Segmentation_Feature_Bits[j];
								limit = Segmentation_Feature_Max[j];

								if ((Segmentation_Feature_Signed[j] == 1))
								{
									this.feature_value = stream.Pick("feature_value", _original != null ? _original.FeatureData[i][j] : this.feature_value, _edited != null ? _edited.FeatureData[i][j] : _original != null ? _original.FeatureData[i][j] : this.feature_value);
									stream.WriteSignedIntVar((1 + bitsToRead), this.feature_value, "feature_value"); 
									clippedValue = Clip3(-limit, limit, feature_value);
								}
								else 
								{
									this.feature_value = stream.Pick("feature_value", _original != null ? _original.FeatureData[i][j] : this.feature_value, _edited != null ? _edited.FeatureData[i][j] : _original != null ? _original.FeatureData[i][j] : this.feature_value);
									stream.WriteVariable(bitsToRead, this.feature_value, "feature_value"); 
									clippedValue = Clip3(0, limit, feature_value);
								}
							}
							FeatureData[i][j] = clippedValue;
						}
					}
				}
			}
			else 
			{

				for (i = 0; (i < MAX_SEGMENTS); i++)
				{

					for (j = 0; (j < SEG_LVL_MAX); j++)
					{
						FeatureEnabled[i][j] = 0;
						FeatureData[i][j] = 0;
					}
				}
			}
			SegIdPreSkip = 0;
			LastActiveSegId = 0;

			for (i = 0; (i < MAX_SEGMENTS); i++)
			{

				for (j = 0; (j < SEG_LVL_MAX); j++)
				{

					if ((FeatureEnabled[i][j] != 0))
					{
						LastActiveSegId = i;

						if ((j >= SEG_LVL_REF_FRAME))
						{
							SegIdPreSkip = 1;
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
		public int _DeltaqRes { get { return delta_q_res; } set { delta_q_res = value; } }
		private int delta_q_present;
		public int _DeltaqPresent { get { return delta_q_present; } set { delta_q_present = value; } }

        private void DeltaqParams()
        {
			delta_q_res = 0;
			delta_q_present = 0;

			if ((base_q_idx > 0))
			{
				stream.ReadFixed(1, out this.delta_q_present, "delta_q_present"); 
			}

			if ((delta_q_present != 0))
			{
				stream.ReadFixed(2, out this.delta_q_res, "delta_q_res"); 
			}
        }

        private void WriteDeltaqParams()
        {
			delta_q_res = 0;
			delta_q_present = 0;

			if ((base_q_idx > 0))
			{
				this.delta_q_present = stream.Pick("delta_q_present", _original != null ? _original.delta_q_present : this.delta_q_present, _edited != null ? _edited.delta_q_present : _original != null ? _original.delta_q_present : this.delta_q_present);
				stream.WriteFixed(1, this.delta_q_present, "delta_q_present"); 
			}

			if ((delta_q_present != 0))
			{
				this.delta_q_res = stream.Pick("delta_q_res", _original != null ? _original.delta_q_res : this.delta_q_res, _edited != null ? _edited.delta_q_res : _original != null ? _original.delta_q_res : this.delta_q_res);
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
		public int _DeltaLfPresent { get { return delta_lf_present; } set { delta_lf_present = value; } }
		private int delta_lf_res;
		public int _DeltaLfRes { get { return delta_lf_res; } set { delta_lf_res = value; } }
		private int delta_lf_multi;
		public int _DeltaLfMulti { get { return delta_lf_multi; } set { delta_lf_multi = value; } }

        private void DeltaLfParams()
        {
			delta_lf_present = 0;
			delta_lf_res = 0;
			delta_lf_multi = 0;

			if ((delta_q_present != 0))
			{

				if (!(allow_intrabc != 0))
				{
					stream.ReadFixed(1, out this.delta_lf_present, "delta_lf_present"); 
				}

				if ((delta_lf_present != 0))
				{
					stream.ReadFixed(2, out this.delta_lf_res, "delta_lf_res"); 
					stream.ReadFixed(1, out this.delta_lf_multi, "delta_lf_multi"); 
				}
			}
        }

        private void WriteDeltaLfParams()
        {
			delta_lf_present = 0;
			delta_lf_res = 0;
			delta_lf_multi = 0;

			if ((delta_q_present != 0))
			{

				if (!(allow_intrabc != 0))
				{
					this.delta_lf_present = stream.Pick("delta_lf_present", _original != null ? _original.delta_lf_present : this.delta_lf_present, _edited != null ? _edited.delta_lf_present : _original != null ? _original.delta_lf_present : this.delta_lf_present);
					stream.WriteFixed(1, this.delta_lf_present, "delta_lf_present"); 
				}

				if ((delta_lf_present != 0))
				{
					this.delta_lf_res = stream.Pick("delta_lf_res", _original != null ? _original.delta_lf_res : this.delta_lf_res, _edited != null ? _edited.delta_lf_res : _original != null ? _original.delta_lf_res : this.delta_lf_res);
					stream.WriteFixed(2, this.delta_lf_res, "delta_lf_res"); 
					this.delta_lf_multi = stream.Pick("delta_lf_multi", _original != null ? _original.delta_lf_multi : this.delta_lf_multi, _edited != null ? _edited.delta_lf_multi : _original != null ? _original.delta_lf_multi : this.delta_lf_multi);
					stream.WriteFixed(1, this.delta_lf_multi, "delta_lf_multi"); 
				}
			}
        }

    /*
cdef_params() { 
 if ( CodedLossless || allow_intrabc || !enable_cdef) {
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
		public int _CdefBits { get { return cdef_bits; } set { cdef_bits = value; } }
		private AomArray<int> cdef_y_pri_strength = new AomArray<int>();
		public AomArray<int> _CdefyPriStrength { get { return cdef_y_pri_strength; } set { cdef_y_pri_strength = value; } }
		private AomArray<int> cdef_y_sec_strength = new AomArray<int>();
		public AomArray<int> _CdefySecStrength { get { return cdef_y_sec_strength; } set { cdef_y_sec_strength = value; } }
		private AomArray<int> cdef_uv_pri_strength = new AomArray<int>();
		public AomArray<int> _CdefUvPriStrength { get { return cdef_uv_pri_strength; } set { cdef_uv_pri_strength = value; } }
		private AomArray<int> cdef_uv_sec_strength = new AomArray<int>();
		public AomArray<int> _CdefUvSecStrength { get { return cdef_uv_sec_strength; } set { cdef_uv_sec_strength = value; } }
		private int CdefDamping;
		public int _CdefDamping { get { return CdefDamping; } set { CdefDamping = value; } }
		private int cdef_damping_minus_3;
		public int _CdefDampingMinus3 { get { return cdef_damping_minus_3; } set { cdef_damping_minus_3 = value; } }

        private void CdefParams()
        {
			int i = 0;

			if ((((CodedLossless != 0) || (allow_intrabc != 0)) || !(enable_cdef != 0)))
			{
				cdef_bits = 0;
				cdef_y_pri_strength[0] = 0;
				cdef_y_sec_strength[0] = 0;
				cdef_uv_pri_strength[0] = 0;
				cdef_uv_sec_strength[0] = 0;
				CdefDamping = 3;
				return;
			}
			stream.ReadFixed(2, out this.cdef_damping_minus_3, "cdef_damping_minus_3"); 
			CdefDamping = (cdef_damping_minus_3 + 3);
			stream.ReadFixed(2, out this.cdef_bits, "cdef_bits"); 

			for (i = 0; (i < (1 << cdef_bits)); i++)
			{
				stream.ReadFixed(4, out this.cdef_y_pri_strength[i], "cdef_y_pri_strength"); 
				stream.ReadFixed(2, out this.cdef_y_sec_strength[i], "cdef_y_sec_strength"); 

				if ((cdef_y_sec_strength[i] == 3))
				{
					cdef_y_sec_strength[i] += 1;
				}

				if ((NumPlanes > 1))
				{
					stream.ReadFixed(4, out this.cdef_uv_pri_strength[i], "cdef_uv_pri_strength"); 
					stream.ReadFixed(2, out this.cdef_uv_sec_strength[i], "cdef_uv_sec_strength"); 

					if ((cdef_uv_sec_strength[i] == 3))
					{
						cdef_uv_sec_strength[i] += 1;
					}
				}
			}
        }

        private void WriteCdefParams()
        {
			int i = 0;

			if ((((CodedLossless != 0) || (allow_intrabc != 0)) || !(enable_cdef != 0)))
			{
				cdef_bits = 0;
				cdef_y_pri_strength[0] = 0;
				cdef_y_sec_strength[0] = 0;
				cdef_uv_pri_strength[0] = 0;
				cdef_uv_sec_strength[0] = 0;
				CdefDamping = 3;
				return;
			}
			this.cdef_damping_minus_3 = stream.Pick("cdef_damping_minus_3", _original != null ? _original.cdef_damping_minus_3 : this.cdef_damping_minus_3, _edited != null ? _edited.cdef_damping_minus_3 : _original != null ? _original.cdef_damping_minus_3 : this.cdef_damping_minus_3);
			stream.WriteFixed(2, this.cdef_damping_minus_3, "cdef_damping_minus_3"); 
			CdefDamping = (cdef_damping_minus_3 + 3);
			this.cdef_bits = stream.Pick("cdef_bits", _original != null ? _original.cdef_bits : this.cdef_bits, _edited != null ? _edited.cdef_bits : _original != null ? _original.cdef_bits : this.cdef_bits);
			stream.WriteFixed(2, this.cdef_bits, "cdef_bits"); 

			for (i = 0; (i < (1 << cdef_bits)); i++)
			{
				this.cdef_y_pri_strength[i] = stream.Pick("cdef_y_pri_strength", _original != null ? _original.cdef_y_pri_strength[i] : this.cdef_y_pri_strength[i], _edited != null ? _edited.cdef_y_pri_strength[i] : _original != null ? _original.cdef_y_pri_strength[i] : this.cdef_y_pri_strength[i]);
				stream.WriteFixed(4, this.cdef_y_pri_strength[i], "cdef_y_pri_strength"); 
				this.cdef_y_sec_strength[i] = stream.Pick("cdef_y_sec_strength", _original != null ? (_original.cdef_y_sec_strength[i] == 4 ? 3 : _original.cdef_y_sec_strength[i]) : this.cdef_y_sec_strength[i], _edited != null ? (_edited.cdef_y_sec_strength[i] == 4 ? 3 : _edited.cdef_y_sec_strength[i]) : _original != null ? (_original.cdef_y_sec_strength[i] == 4 ? 3 : _original.cdef_y_sec_strength[i]) : this.cdef_y_sec_strength[i]);
				stream.WriteFixed(2, this.cdef_y_sec_strength[i], "cdef_y_sec_strength"); 

				if ((cdef_y_sec_strength[i] == 3))
				{
					cdef_y_sec_strength[i] += 1;
				}

				if ((NumPlanes > 1))
				{
					this.cdef_uv_pri_strength[i] = stream.Pick("cdef_uv_pri_strength", _original != null ? _original.cdef_uv_pri_strength[i] : this.cdef_uv_pri_strength[i], _edited != null ? _edited.cdef_uv_pri_strength[i] : _original != null ? _original.cdef_uv_pri_strength[i] : this.cdef_uv_pri_strength[i]);
					stream.WriteFixed(4, this.cdef_uv_pri_strength[i], "cdef_uv_pri_strength"); 
					this.cdef_uv_sec_strength[i] = stream.Pick("cdef_uv_sec_strength", _original != null ? (_original.cdef_uv_sec_strength[i] == 4 ? 3 : _original.cdef_uv_sec_strength[i]) : this.cdef_uv_sec_strength[i], _edited != null ? (_edited.cdef_uv_sec_strength[i] == 4 ? 3 : _edited.cdef_uv_sec_strength[i]) : _original != null ? (_original.cdef_uv_sec_strength[i] == 4 ? 3 : _original.cdef_uv_sec_strength[i]) : this.cdef_uv_sec_strength[i]);
					stream.WriteFixed(2, this.cdef_uv_sec_strength[i], "cdef_uv_sec_strength"); 

					if ((cdef_uv_sec_strength[i] == 3))
					{
						cdef_uv_sec_strength[i] += 1;
					}
				}
			}
        }

    /*
lr_params() { 
 if ( AllLossless || allow_intrabc || !enable_restoration ) {
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
		private AomArray<int> FrameRestorationType = new AomArray<int>();
		public AomArray<int> _FrameRestorationType { get { return FrameRestorationType; } set { FrameRestorationType = value; } }
		private int UsesLr;
		public int _UsesLr { get { return UsesLr; } set { UsesLr = value; } }
		private int lr_type;
		public int _LrType { get { return lr_type; } set { lr_type = value; } }
		private int lr_unit_shift;
		public int _LrUnitShift { get { return lr_unit_shift; } set { lr_unit_shift = value; } }
		private int lr_unit_extra_shift;
		public int _LrUnitExtraShift { get { return lr_unit_extra_shift; } set { lr_unit_extra_shift = value; } }
		private AomArray<int> LoopRestorationSize = new AomArray<int>();
		public AomArray<int> _LoopRestorationSize { get { return LoopRestorationSize; } set { LoopRestorationSize = value; } }
		private int lr_uv_shift;
		public int _LrUvShift { get { return lr_uv_shift; } set { lr_uv_shift = value; } }

        private void LrParams()
        {
			int i = 0;
			int usesChromaLr = 0;

			if ((((AllLossless != 0) || (allow_intrabc != 0)) || !(enable_restoration != 0)))
			{
				FrameRestorationType[0] = RESTORE_NONE;
				FrameRestorationType[1] = RESTORE_NONE;
				FrameRestorationType[2] = RESTORE_NONE;
				UsesLr = 0;
				return;
			}
			UsesLr = 0;
			usesChromaLr = 0;

			for (i = 0; (i < NumPlanes); i++)
			{
				stream.ReadFixed(2, out this.lr_type, "lr_type"); 
				FrameRestorationType[i] = Remap_Lr_Type[lr_type];

				if ((FrameRestorationType[i] != RESTORE_NONE))
				{
					UsesLr = 1;

					if ((i > 0))
					{
						usesChromaLr = 1;
					}
				}
			}

			if ((UsesLr != 0))
			{

				if ((use_128x128_superblock != 0))
				{
					stream.ReadFixed(1, out this.lr_unit_shift, "lr_unit_shift"); 
					lr_unit_shift++;
				}
				else 
				{
					stream.ReadFixed(1, out this.lr_unit_shift, "lr_unit_shift"); 

					if ((lr_unit_shift != 0))
					{
						stream.ReadFixed(1, out this.lr_unit_extra_shift, "lr_unit_extra_shift"); 
						lr_unit_shift += lr_unit_extra_shift;
					}
				}
				LoopRestorationSize[0] = (RESTORATION_TILESIZE_MAX >> (2 - lr_unit_shift));

				if ((((subsampling_x != 0) && (subsampling_y != 0)) && (usesChromaLr != 0)))
				{
					stream.ReadFixed(1, out this.lr_uv_shift, "lr_uv_shift"); 
				}
				else 
				{
					lr_uv_shift = 0;
				}
				LoopRestorationSize[1] = (LoopRestorationSize[0] >> lr_uv_shift);
				LoopRestorationSize[2] = (LoopRestorationSize[0] >> lr_uv_shift);
			}
        }

        private void WriteLrParams()
        {
			int i = 0;
			int usesChromaLr = 0;

			if ((((AllLossless != 0) || (allow_intrabc != 0)) || !(enable_restoration != 0)))
			{
				FrameRestorationType[0] = RESTORE_NONE;
				FrameRestorationType[1] = RESTORE_NONE;
				FrameRestorationType[2] = RESTORE_NONE;
				UsesLr = 0;
				return;
			}
			UsesLr = 0;
			usesChromaLr = 0;

			for (i = 0; (i < NumPlanes); i++)
			{
				this.lr_type = stream.Pick("lr_type", _original != null ? AomArray.IndexOf(Remap_Lr_Type, _original.FrameRestorationType[i]) : this.lr_type, _edited != null ? AomArray.IndexOf(Remap_Lr_Type, _edited.FrameRestorationType[i]) : _original != null ? AomArray.IndexOf(Remap_Lr_Type, _original.FrameRestorationType[i]) : this.lr_type);
				stream.WriteFixed(2, this.lr_type, "lr_type"); 
				FrameRestorationType[i] = Remap_Lr_Type[lr_type];

				if ((FrameRestorationType[i] != RESTORE_NONE))
				{
					UsesLr = 1;

					if ((i > 0))
					{
						usesChromaLr = 1;
					}
				}
			}

			if ((UsesLr != 0))
			{

				if ((use_128x128_superblock != 0))
				{
					this.lr_unit_shift = stream.Pick("lr_unit_shift", _original != null ? (use_128x128_superblock != 0 ? _original.lr_unit_shift - 1 : (_original.lr_unit_shift > 0 ? 1 : 0)) : this.lr_unit_shift, _edited != null ? (use_128x128_superblock != 0 ? _edited.lr_unit_shift - 1 : (_edited.lr_unit_shift > 0 ? 1 : 0)) : _original != null ? (use_128x128_superblock != 0 ? _original.lr_unit_shift - 1 : (_original.lr_unit_shift > 0 ? 1 : 0)) : this.lr_unit_shift);
					stream.WriteFixed(1, this.lr_unit_shift, "lr_unit_shift"); 
					lr_unit_shift++;
				}
				else 
				{
					this.lr_unit_shift = stream.Pick("lr_unit_shift", _original != null ? (use_128x128_superblock != 0 ? _original.lr_unit_shift - 1 : (_original.lr_unit_shift > 0 ? 1 : 0)) : this.lr_unit_shift, _edited != null ? (use_128x128_superblock != 0 ? _edited.lr_unit_shift - 1 : (_edited.lr_unit_shift > 0 ? 1 : 0)) : _original != null ? (use_128x128_superblock != 0 ? _original.lr_unit_shift - 1 : (_original.lr_unit_shift > 0 ? 1 : 0)) : this.lr_unit_shift);
					stream.WriteFixed(1, this.lr_unit_shift, "lr_unit_shift"); 

					if ((lr_unit_shift != 0))
					{
						this.lr_unit_extra_shift = stream.Pick("lr_unit_extra_shift", _original != null ? (_original.lr_unit_shift - 1) : this.lr_unit_extra_shift, _edited != null ? (_edited.lr_unit_shift - 1) : _original != null ? (_original.lr_unit_shift - 1) : this.lr_unit_extra_shift);
						stream.WriteFixed(1, this.lr_unit_extra_shift, "lr_unit_extra_shift"); 
						lr_unit_shift += lr_unit_extra_shift;
					}
				}
				LoopRestorationSize[0] = (RESTORATION_TILESIZE_MAX >> (2 - lr_unit_shift));

				if ((((subsampling_x != 0) && (subsampling_y != 0)) && (usesChromaLr != 0)))
				{
					this.lr_uv_shift = stream.Pick("lr_uv_shift", _original != null ? _original.lr_uv_shift : this.lr_uv_shift, _edited != null ? _edited.lr_uv_shift : _original != null ? _original.lr_uv_shift : this.lr_uv_shift);
					stream.WriteFixed(1, this.lr_uv_shift, "lr_uv_shift"); 
				}
				else 
				{
					lr_uv_shift = 0;
				}
				LoopRestorationSize[1] = (LoopRestorationSize[0] >> lr_uv_shift);
				LoopRestorationSize[2] = (LoopRestorationSize[0] >> lr_uv_shift);
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
		private AomArray<int> loop_filter_level = new AomArray<int>();
		public AomArray<int> _LoopFilterLevel { get { return loop_filter_level; } set { loop_filter_level = value; } }
		private AomArray<int> loop_filter_ref_deltas = new AomArray<int>();
		public AomArray<int> _LoopFilterRefDeltas { get { return loop_filter_ref_deltas; } set { loop_filter_ref_deltas = value; } }
		private AomArray<int> loop_filter_mode_deltas = new AomArray<int>();
		public AomArray<int> _LoopFilterModeDeltas { get { return loop_filter_mode_deltas; } set { loop_filter_mode_deltas = value; } }
		private int loop_filter_sharpness;
		public int _LoopFilterSharpness { get { return loop_filter_sharpness; } set { loop_filter_sharpness = value; } }
		private int loop_filter_delta_enabled;
		public int _LoopFilterDeltaEnabled { get { return loop_filter_delta_enabled; } set { loop_filter_delta_enabled = value; } }
		private int loop_filter_delta_update;
		public int _LoopFilterDeltaUpdate { get { return loop_filter_delta_update; } set { loop_filter_delta_update = value; } }
		private int update_ref_delta;
		public int _UpdateRefDelta { get { return update_ref_delta; } set { update_ref_delta = value; } }
		private int update_mode_delta;
		public int _UpdateModeDelta { get { return update_mode_delta; } set { update_mode_delta = value; } }

        private void LoopFilterParams()
        {
			int i = 0;

			if (((CodedLossless != 0) || (allow_intrabc != 0)))
			{
				loop_filter_level[0] = 0;
				loop_filter_level[1] = 0;
				loop_filter_ref_deltas[INTRA_FRAME] = 1;
				loop_filter_ref_deltas[LAST_FRAME] = 0;
				loop_filter_ref_deltas[LAST2_FRAME] = 0;
				loop_filter_ref_deltas[LAST3_FRAME] = 0;
				loop_filter_ref_deltas[BWDREF_FRAME] = 0;
				loop_filter_ref_deltas[GOLDEN_FRAME] = -1;
				loop_filter_ref_deltas[ALTREF_FRAME] = -1;
				loop_filter_ref_deltas[ALTREF2_FRAME] = -1;

				for (i = 0; (i < 2); i++)
				{
					loop_filter_mode_deltas[i] = 0;
				}
				return;
			}
			stream.ReadFixed(6, out this.loop_filter_level[0], "loop_filter_level"); 
			stream.ReadFixed(6, out this.loop_filter_level[1], "loop_filter_level"); 

			if ((NumPlanes > 1))
			{

				if (((loop_filter_level[0] != 0) || (loop_filter_level[1] != 0)))
				{
					stream.ReadFixed(6, out this.loop_filter_level[2], "loop_filter_level"); 
					stream.ReadFixed(6, out this.loop_filter_level[3], "loop_filter_level"); 
				}
			}
			stream.ReadFixed(3, out this.loop_filter_sharpness, "loop_filter_sharpness"); 
			stream.ReadFixed(1, out this.loop_filter_delta_enabled, "loop_filter_delta_enabled"); 

			if ((loop_filter_delta_enabled == 1))
			{
				stream.ReadFixed(1, out this.loop_filter_delta_update, "loop_filter_delta_update"); 

				if ((loop_filter_delta_update == 1))
				{

					for (i = 0; (i < TOTAL_REFS_PER_FRAME); i++)
					{
						stream.ReadFixed(1, out this.update_ref_delta, "update_ref_delta"); 

						if ((update_ref_delta == 1))
						{
							stream.ReadSignedIntVar((1 + 6), out this.loop_filter_ref_deltas[i], "loop_filter_ref_deltas"); 
						}
					}

					for (i = 0; (i < 2); i++)
					{
						stream.ReadFixed(1, out this.update_mode_delta, "update_mode_delta"); 

						if ((update_mode_delta == 1))
						{
							stream.ReadSignedIntVar((1 + 6), out this.loop_filter_mode_deltas[i], "loop_filter_mode_deltas"); 
						}
					}
				}
			}
        }

        private void WriteLoopFilterParams()
        {
			int i = 0;

			if (((CodedLossless != 0) || (allow_intrabc != 0)))
			{
				loop_filter_level[0] = 0;
				loop_filter_level[1] = 0;
				loop_filter_ref_deltas[INTRA_FRAME] = 1;
				loop_filter_ref_deltas[LAST_FRAME] = 0;
				loop_filter_ref_deltas[LAST2_FRAME] = 0;
				loop_filter_ref_deltas[LAST3_FRAME] = 0;
				loop_filter_ref_deltas[BWDREF_FRAME] = 0;
				loop_filter_ref_deltas[GOLDEN_FRAME] = -1;
				loop_filter_ref_deltas[ALTREF_FRAME] = -1;
				loop_filter_ref_deltas[ALTREF2_FRAME] = -1;

				for (i = 0; (i < 2); i++)
				{
					loop_filter_mode_deltas[i] = 0;
				}
				return;
			}
			this.loop_filter_level[0] = stream.Pick("loop_filter_level", _original != null ? _original.loop_filter_level[0] : this.loop_filter_level[0], _edited != null ? _edited.loop_filter_level[0] : _original != null ? _original.loop_filter_level[0] : this.loop_filter_level[0]);
			stream.WriteFixed(6, this.loop_filter_level[0], "loop_filter_level"); 
			this.loop_filter_level[1] = stream.Pick("loop_filter_level", _original != null ? _original.loop_filter_level[1] : this.loop_filter_level[1], _edited != null ? _edited.loop_filter_level[1] : _original != null ? _original.loop_filter_level[1] : this.loop_filter_level[1]);
			stream.WriteFixed(6, this.loop_filter_level[1], "loop_filter_level"); 

			if ((NumPlanes > 1))
			{

				if (((loop_filter_level[0] != 0) || (loop_filter_level[1] != 0)))
				{
					this.loop_filter_level[2] = stream.Pick("loop_filter_level", _original != null ? _original.loop_filter_level[2] : this.loop_filter_level[2], _edited != null ? _edited.loop_filter_level[2] : _original != null ? _original.loop_filter_level[2] : this.loop_filter_level[2]);
					stream.WriteFixed(6, this.loop_filter_level[2], "loop_filter_level"); 
					this.loop_filter_level[3] = stream.Pick("loop_filter_level", _original != null ? _original.loop_filter_level[3] : this.loop_filter_level[3], _edited != null ? _edited.loop_filter_level[3] : _original != null ? _original.loop_filter_level[3] : this.loop_filter_level[3]);
					stream.WriteFixed(6, this.loop_filter_level[3], "loop_filter_level"); 
				}
			}
			this.loop_filter_sharpness = stream.Pick("loop_filter_sharpness", _original != null ? _original.loop_filter_sharpness : this.loop_filter_sharpness, _edited != null ? _edited.loop_filter_sharpness : _original != null ? _original.loop_filter_sharpness : this.loop_filter_sharpness);
			stream.WriteFixed(3, this.loop_filter_sharpness, "loop_filter_sharpness"); 
			this.loop_filter_delta_enabled = stream.Pick("loop_filter_delta_enabled", _original != null ? _original.loop_filter_delta_enabled : this.loop_filter_delta_enabled, _edited != null ? _edited.loop_filter_delta_enabled : _original != null ? _original.loop_filter_delta_enabled : this.loop_filter_delta_enabled);
			stream.WriteFixed(1, this.loop_filter_delta_enabled, "loop_filter_delta_enabled"); 

			if ((loop_filter_delta_enabled == 1))
			{
				this.loop_filter_delta_update = stream.Pick("loop_filter_delta_update", _original != null ? _original.loop_filter_delta_update : this.loop_filter_delta_update, _edited != null ? _edited.loop_filter_delta_update : _original != null ? _original.loop_filter_delta_update : this.loop_filter_delta_update);
				stream.WriteFixed(1, this.loop_filter_delta_update, "loop_filter_delta_update"); 

				if ((loop_filter_delta_update == 1))
				{

					for (i = 0; (i < TOTAL_REFS_PER_FRAME); i++)
					{
						this.update_ref_delta = stream.Pick("update_ref_delta", _original != null ? (_original.loop_filter_ref_deltas[i] != loop_filter_ref_deltas[i] ? 1 : 0) : this.update_ref_delta, _edited != null ? (_edited.loop_filter_ref_deltas[i] != loop_filter_ref_deltas[i] ? 1 : 0) : _original != null ? (_original.loop_filter_ref_deltas[i] != loop_filter_ref_deltas[i] ? 1 : 0) : this.update_ref_delta);
						stream.WriteFixed(1, this.update_ref_delta, "update_ref_delta"); 

						if ((update_ref_delta == 1))
						{
							this.loop_filter_ref_deltas[i] = stream.Pick("loop_filter_ref_deltas", _original != null ? _original.loop_filter_ref_deltas[i] : this.loop_filter_ref_deltas[i], _edited != null ? _edited.loop_filter_ref_deltas[i] : _original != null ? _original.loop_filter_ref_deltas[i] : this.loop_filter_ref_deltas[i]);
							stream.WriteSignedIntVar((1 + 6), this.loop_filter_ref_deltas[i], "loop_filter_ref_deltas"); 
						}
					}

					for (i = 0; (i < 2); i++)
					{
						this.update_mode_delta = stream.Pick("update_mode_delta", _original != null ? (_original.loop_filter_mode_deltas[i] != loop_filter_mode_deltas[i] ? 1 : 0) : this.update_mode_delta, _edited != null ? (_edited.loop_filter_mode_deltas[i] != loop_filter_mode_deltas[i] ? 1 : 0) : _original != null ? (_original.loop_filter_mode_deltas[i] != loop_filter_mode_deltas[i] ? 1 : 0) : this.update_mode_delta);
						stream.WriteFixed(1, this.update_mode_delta, "update_mode_delta"); 

						if ((update_mode_delta == 1))
						{
							this.loop_filter_mode_deltas[i] = stream.Pick("loop_filter_mode_deltas", _original != null ? _original.loop_filter_mode_deltas[i] : this.loop_filter_mode_deltas[i], _edited != null ? _edited.loop_filter_mode_deltas[i] : _original != null ? _original.loop_filter_mode_deltas[i] : this.loop_filter_mode_deltas[i]);
							stream.WriteSignedIntVar((1 + 6), this.loop_filter_mode_deltas[i], "loop_filter_mode_deltas"); 
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
		public int _TxMode { get { return TxMode; } set { TxMode = value; } }
		private int tx_mode_select;
		public int _TxModeSelect { get { return tx_mode_select; } set { tx_mode_select = value; } }

        private void ReadTxMode()
        {

			if ((CodedLossless == 1))
			{
				TxMode = ONLY_4X4;
			}
			else 
			{
				stream.ReadFixed(1, out this.tx_mode_select, "tx_mode_select"); 

				if ((tx_mode_select != 0))
				{
					TxMode = TX_MODE_SELECT;
				}
				else 
				{
					TxMode = TX_MODE_LARGEST;
				}
			}
        }

        private void WriteReadTxMode()
        {

			if ((CodedLossless == 1))
			{
				TxMode = ONLY_4X4;
			}
			else 
			{
				this.tx_mode_select = stream.Pick("tx_mode_select", _original != null ? _original.tx_mode_select : this.tx_mode_select, _edited != null ? _edited.tx_mode_select : _original != null ? _original.tx_mode_select : this.tx_mode_select);
				stream.WriteFixed(1, this.tx_mode_select, "tx_mode_select"); 

				if ((tx_mode_select != 0))
				{
					TxMode = TX_MODE_SELECT;
				}
				else 
				{
					TxMode = TX_MODE_LARGEST;
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
		public int _ReferenceSelect { get { return reference_select; } set { reference_select = value; } }

        private void FrameReferenceMode()
        {

			if ((FrameIsIntra != 0))
			{
				reference_select = 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.reference_select, "reference_select"); 
			}
        }

        private void WriteFrameReferenceMode()
        {

			if ((FrameIsIntra != 0))
			{
				reference_select = 0;
			}
			else 
			{
				this.reference_select = stream.Pick("reference_select", _original != null ? _original.reference_select : this.reference_select, _edited != null ? _edited.reference_select : _original != null ? _original.reference_select : this.reference_select);
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
		private AomArray<int> SkipModeFrame = new AomArray<int>();
		public AomArray<int> _SkipModeFrame { get { return SkipModeFrame; } set { SkipModeFrame = value; } }
		private int skip_mode_present;
		public int _SkipModePresent { get { return skip_mode_present; } set { skip_mode_present = value; } }

        private void SkipModeParams()
        {
			int i = 0;
			int skipModeAllowed = 0;
			int forwardIdx = 0;
			int backwardIdx = 0;
			int refHint = 0;
			int forwardHint = 0;
			int backwardHint = 0;
			int secondForwardIdx = 0;
			int secondForwardHint = 0;

			if ((((FrameIsIntra != 0) || !(reference_select != 0)) || !(enable_order_hint != 0)))
			{
				skipModeAllowed = 0;
			}
			else 
			{
				forwardIdx = -1;
				backwardIdx = -1;

				for (i = 0; (i < REFS_PER_FRAME); i++)
				{
					refHint = RefOrderHint[ref_frame_idx[i]];

					if ((GetRelativeDist(refHint, OrderHint) < 0))
					{

						if (((forwardIdx < 0) || (GetRelativeDist(refHint, forwardHint) > 0)))
						{
							forwardIdx = i;
							forwardHint = refHint;
						}
					}
					else if ((GetRelativeDist(refHint, OrderHint) > 0))
					{

						if (((backwardIdx < 0) || (GetRelativeDist(refHint, backwardHint) < 0)))
						{
							backwardIdx = i;
							backwardHint = refHint;
						}
					}
				}

				if ((forwardIdx < 0))
				{
					skipModeAllowed = 0;
				}
				else if ((backwardIdx >= 0))
				{
					skipModeAllowed = 1;
					SkipModeFrame[0] = (LAST_FRAME + Min(forwardIdx, backwardIdx));
					SkipModeFrame[1] = (LAST_FRAME + Max(forwardIdx, backwardIdx));
				}
				else 
				{
					secondForwardIdx = -1;

					for (i = 0; (i < REFS_PER_FRAME); i++)
					{
						refHint = RefOrderHint[ref_frame_idx[i]];

						if ((GetRelativeDist(refHint, forwardHint) < 0))
						{

							if (((secondForwardIdx < 0) || (GetRelativeDist(refHint, secondForwardHint) > 0)))
							{
								secondForwardIdx = i;
								secondForwardHint = refHint;
							}
						}
					}

					if ((secondForwardIdx < 0))
					{
						skipModeAllowed = 0;
					}
					else 
					{
						skipModeAllowed = 1;
						SkipModeFrame[0] = (LAST_FRAME + Min(forwardIdx, secondForwardIdx));
						SkipModeFrame[1] = (LAST_FRAME + Max(forwardIdx, secondForwardIdx));
					}
				}
			}

			if ((skipModeAllowed != 0))
			{
				stream.ReadFixed(1, out this.skip_mode_present, "skip_mode_present"); 
			}
			else 
			{
				skip_mode_present = 0;
			}
        }

        private void WriteSkipModeParams()
        {
			int i = 0;
			int skipModeAllowed = 0;
			int forwardIdx = 0;
			int backwardIdx = 0;
			int refHint = 0;
			int forwardHint = 0;
			int backwardHint = 0;
			int secondForwardIdx = 0;
			int secondForwardHint = 0;

			if ((((FrameIsIntra != 0) || !(reference_select != 0)) || !(enable_order_hint != 0)))
			{
				skipModeAllowed = 0;
			}
			else 
			{
				forwardIdx = -1;
				backwardIdx = -1;

				for (i = 0; (i < REFS_PER_FRAME); i++)
				{
					refHint = RefOrderHint[ref_frame_idx[i]];

					if ((GetRelativeDist(refHint, OrderHint) < 0))
					{

						if (((forwardIdx < 0) || (GetRelativeDist(refHint, forwardHint) > 0)))
						{
							forwardIdx = i;
							forwardHint = refHint;
						}
					}
					else if ((GetRelativeDist(refHint, OrderHint) > 0))
					{

						if (((backwardIdx < 0) || (GetRelativeDist(refHint, backwardHint) < 0)))
						{
							backwardIdx = i;
							backwardHint = refHint;
						}
					}
				}

				if ((forwardIdx < 0))
				{
					skipModeAllowed = 0;
				}
				else if ((backwardIdx >= 0))
				{
					skipModeAllowed = 1;
					SkipModeFrame[0] = (LAST_FRAME + Min(forwardIdx, backwardIdx));
					SkipModeFrame[1] = (LAST_FRAME + Max(forwardIdx, backwardIdx));
				}
				else 
				{
					secondForwardIdx = -1;

					for (i = 0; (i < REFS_PER_FRAME); i++)
					{
						refHint = RefOrderHint[ref_frame_idx[i]];

						if ((GetRelativeDist(refHint, forwardHint) < 0))
						{

							if (((secondForwardIdx < 0) || (GetRelativeDist(refHint, secondForwardHint) > 0)))
							{
								secondForwardIdx = i;
								secondForwardHint = refHint;
							}
						}
					}

					if ((secondForwardIdx < 0))
					{
						skipModeAllowed = 0;
					}
					else 
					{
						skipModeAllowed = 1;
						SkipModeFrame[0] = (LAST_FRAME + Min(forwardIdx, secondForwardIdx));
						SkipModeFrame[1] = (LAST_FRAME + Max(forwardIdx, secondForwardIdx));
					}
				}
			}

			if ((skipModeAllowed != 0))
			{
				this.skip_mode_present = stream.Pick("skip_mode_present", _original != null ? _original.skip_mode_present : this.skip_mode_present, _edited != null ? _edited.skip_mode_present : _original != null ? _original.skip_mode_present : this.skip_mode_present);
				stream.WriteFixed(1, this.skip_mode_present, "skip_mode_present"); 
			}
			else 
			{
				skip_mode_present = 0;
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
		private AomArray<int> GmType = new AomArray<int>();
		public AomArray<int> _GmType { get { return GmType; } set { GmType = value; } }
		private AomArray<AomArray<int>> gm_params = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _GmParams { get { return gm_params; } set { gm_params = value; } }
		private int is_global;
		public int _IsGlobal { get { return is_global; } set { is_global = value; } }
		private int is_rot_zoom;
		public int _IsRotZoom { get { return is_rot_zoom; } set { is_rot_zoom = value; } }
		private int is_translation;
		public int _IsTranslation { get { return is_translation; } set { is_translation = value; } }
		private int refc = 0;

        private void GlobalMotionParams()
        {
			int refc = 0;
			int i = 0;
			int type = 0;

			for (refc = LAST_FRAME; (refc <= ALTREF_FRAME); refc++)
			{
				GmType[refc] = IDENTITY;

				for (i = 0; (i < 6); i++)
				{
					gm_params[refc][i] = (((i % 3) == 2) ? (1 << WARPEDMODEL_PREC_BITS) : 0);
				}
			}

			if ((FrameIsIntra != 0))
			{
				return;
			}

			for (refc = LAST_FRAME; (refc <= ALTREF_FRAME); refc++)
			{
				stream.ReadFixed(1, out this.is_global, "is_global"); 

				if ((is_global != 0))
				{
					stream.ReadFixed(1, out this.is_rot_zoom, "is_rot_zoom"); 

					if ((is_rot_zoom != 0))
					{
						type = ROTZOOM;
					}
					else 
					{
						stream.ReadFixed(1, out this.is_translation, "is_translation"); 
						type = ((is_translation != 0) ? TRANSLATION : AFFINE);
					}
				}
				else 
				{
					type = IDENTITY;
				}
				GmType[refc] = type;

				if ((type >= ROTZOOM))
				{
					ReadGlobalParam(type, refc, 2); 
					ReadGlobalParam(type, refc, 3); 

					if ((type == AFFINE))
					{
						ReadGlobalParam(type, refc, 4); 
						ReadGlobalParam(type, refc, 5); 
					}
					else 
					{
						gm_params[refc][4] = -gm_params[refc][3];
						gm_params[refc][5] = gm_params[refc][2];
					}
				}

				if ((type >= TRANSLATION))
				{
					ReadGlobalParam(type, refc, 0); 
					ReadGlobalParam(type, refc, 1); 
				}
			}
        }

        private void WriteGlobalMotionParams()
        {
			int refc = 0;
			int i = 0;
			int type = 0;

			for (refc = LAST_FRAME; (refc <= ALTREF_FRAME); refc++)
			{
				GmType[refc] = IDENTITY;

				for (i = 0; (i < 6); i++)
				{
					gm_params[refc][i] = (((i % 3) == 2) ? (1 << WARPEDMODEL_PREC_BITS) : 0);
				}
			}

			if ((FrameIsIntra != 0))
			{
				return;
			}

			for (refc = LAST_FRAME; (refc <= ALTREF_FRAME); refc++)
			{
				this.is_global = stream.Pick("is_global", _original != null ? (_original.GmType[refc] != IDENTITY ? 1 : 0) : this.is_global, _edited != null ? (_edited.GmType[refc] != IDENTITY ? 1 : 0) : _original != null ? (_original.GmType[refc] != IDENTITY ? 1 : 0) : this.is_global);
				stream.WriteFixed(1, this.is_global, "is_global"); 

				if ((is_global != 0))
				{
					this.is_rot_zoom = stream.Pick("is_rot_zoom", _original != null ? (_original.GmType[refc] == ROTZOOM ? 1 : 0) : this.is_rot_zoom, _edited != null ? (_edited.GmType[refc] == ROTZOOM ? 1 : 0) : _original != null ? (_original.GmType[refc] == ROTZOOM ? 1 : 0) : this.is_rot_zoom);
					stream.WriteFixed(1, this.is_rot_zoom, "is_rot_zoom"); 

					if ((is_rot_zoom != 0))
					{
						type = ROTZOOM;
					}
					else 
					{
						this.is_translation = stream.Pick("is_translation", _original != null ? (_original.GmType[refc] == TRANSLATION ? 1 : 0) : this.is_translation, _edited != null ? (_edited.GmType[refc] == TRANSLATION ? 1 : 0) : _original != null ? (_original.GmType[refc] == TRANSLATION ? 1 : 0) : this.is_translation);
						stream.WriteFixed(1, this.is_translation, "is_translation"); 
						type = ((is_translation != 0) ? TRANSLATION : AFFINE);
					}
				}
				else 
				{
					type = IDENTITY;
				}
				GmType[refc] = type;

				if ((type >= ROTZOOM))
				{
					WriteReadGlobalParam(type, refc, 2); 
					WriteReadGlobalParam(type, refc, 3); 

					if ((type == AFFINE))
					{
						WriteReadGlobalParam(type, refc, 4); 
						WriteReadGlobalParam(type, refc, 5); 
					}
					else 
					{
						gm_params[refc][4] = -gm_params[refc][3];
						gm_params[refc][5] = gm_params[refc][2];
					}
				}

				if ((type >= TRANSLATION))
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
		private int type;
		public int _Type { get { return type; } set { type = value; } }
		private int idx;
		public int _Idx { get { return idx; } set { idx = value; } }

        private void ReadGlobalParam(int type, int refc, int idx)
        {
			int absBits = 0;
			int precBits = 0;
			int precDiff = 0;
			int round = 0;
			int sub = 0;
			int mx = 0;
			int r = 0;
			absBits = GM_ABS_ALPHA_BITS;
			precBits = GM_ALPHA_PREC_BITS;

			if ((idx < 2))
			{

				if ((type == TRANSLATION))
				{
					absBits = (GM_ABS_TRANS_ONLY_BITS - (!(allow_high_precision_mv != 0) ? 1 : 0));
					precBits = (GM_TRANS_ONLY_PREC_BITS - (!(allow_high_precision_mv != 0) ? 1 : 0));
				}
				else 
				{
					absBits = GM_ABS_TRANS_BITS;
					precBits = GM_TRANS_PREC_BITS;
				}
			}
			precDiff = (WARPEDMODEL_PREC_BITS - precBits);
			round = (((idx % 3) == 2) ? (1 << WARPEDMODEL_PREC_BITS) : 0);
			sub = (((idx % 3) == 2) ? (1 << precBits) : 0);
			mx = (1 << absBits);
			r = ((PrevGmParams[refc][idx] >> precDiff) - sub);
			gm_params[refc][idx] = ((DecodeSignedSubexpWithRef(-mx, (mx + 1), r) << precDiff) + round);
        }

        private void WriteReadGlobalParam(int type, int refc, int idx)
        {
			int absBits = 0;
			int precBits = 0;
			int precDiff = 0;
			int round = 0;
			int sub = 0;
			int mx = 0;
			int r = 0;
			absBits = GM_ABS_ALPHA_BITS;
			precBits = GM_ALPHA_PREC_BITS;

			if ((idx < 2))
			{

				if ((type == TRANSLATION))
				{
					absBits = (GM_ABS_TRANS_ONLY_BITS - (!(allow_high_precision_mv != 0) ? 1 : 0));
					precBits = (GM_TRANS_ONLY_PREC_BITS - (!(allow_high_precision_mv != 0) ? 1 : 0));
				}
				else 
				{
					absBits = GM_ABS_TRANS_BITS;
					precBits = GM_TRANS_PREC_BITS;
				}
			}
			precDiff = (WARPEDMODEL_PREC_BITS - precBits);
			round = (((idx % 3) == 2) ? (1 << WARPEDMODEL_PREC_BITS) : 0);
			sub = (((idx % 3) == 2) ? (1 << precBits) : 0);
			mx = (1 << absBits);
			r = ((PrevGmParams[refc][idx] >> precDiff) - sub);
			gm_params[refc][idx] = ((WriteDecodeSignedSubexpWithRef(-mx, (mx + 1), r) << precDiff) + round);
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
		private int apply_grain;
		public int _ApplyGrain { get { return apply_grain; } set { apply_grain = value; } }
		private int grain_seed;
		public int _GrainSeed { get { return grain_seed; } set { grain_seed = value; } }
		private int update_grain;
		public int _UpdateGrain { get { return update_grain; } set { update_grain = value; } }
		private int film_grain_params_ref_idx;
		public int _FilmGrainParamsRefIdx { get { return film_grain_params_ref_idx; } set { film_grain_params_ref_idx = value; } }
		private int num_y_points;
		public int _NumyPoints { get { return num_y_points; } set { num_y_points = value; } }
		private AomArray<int> point_y_value = new AomArray<int>();
		public AomArray<int> _PointyValue { get { return point_y_value; } set { point_y_value = value; } }
		private AomArray<int> point_y_scaling = new AomArray<int>();
		public AomArray<int> _PointyScaling { get { return point_y_scaling; } set { point_y_scaling = value; } }
		private int chroma_scaling_from_luma;
		public int _ChromaScalingFromLuma { get { return chroma_scaling_from_luma; } set { chroma_scaling_from_luma = value; } }
		private int num_cb_points;
		public int _NumCbPoints { get { return num_cb_points; } set { num_cb_points = value; } }
		private int num_cr_points;
		public int _NumCrPoints { get { return num_cr_points; } set { num_cr_points = value; } }
		private AomArray<int> point_cb_value = new AomArray<int>();
		public AomArray<int> _PointCbValue { get { return point_cb_value; } set { point_cb_value = value; } }
		private AomArray<int> point_cb_scaling = new AomArray<int>();
		public AomArray<int> _PointCbScaling { get { return point_cb_scaling; } set { point_cb_scaling = value; } }
		private AomArray<int> point_cr_value = new AomArray<int>();
		public AomArray<int> _PointCrValue { get { return point_cr_value; } set { point_cr_value = value; } }
		private AomArray<int> point_cr_scaling = new AomArray<int>();
		public AomArray<int> _PointCrScaling { get { return point_cr_scaling; } set { point_cr_scaling = value; } }
		private int grain_scaling_minus_8;
		public int _GrainScalingMinus8 { get { return grain_scaling_minus_8; } set { grain_scaling_minus_8 = value; } }
		private int ar_coeff_lag;
		public int _ArCoeffLag { get { return ar_coeff_lag; } set { ar_coeff_lag = value; } }
		private AomArray<int> ar_coeffs_y_plus_128 = new AomArray<int>();
		public AomArray<int> _ArCoeffsyPlus128 { get { return ar_coeffs_y_plus_128; } set { ar_coeffs_y_plus_128 = value; } }
		private AomArray<int> ar_coeffs_cb_plus_128 = new AomArray<int>();
		public AomArray<int> _ArCoeffsCbPlus128 { get { return ar_coeffs_cb_plus_128; } set { ar_coeffs_cb_plus_128 = value; } }
		private AomArray<int> ar_coeffs_cr_plus_128 = new AomArray<int>();
		public AomArray<int> _ArCoeffsCrPlus128 { get { return ar_coeffs_cr_plus_128; } set { ar_coeffs_cr_plus_128 = value; } }
		private int ar_coeff_shift_minus_6;
		public int _ArCoeffShiftMinus6 { get { return ar_coeff_shift_minus_6; } set { ar_coeff_shift_minus_6 = value; } }
		private int grain_scale_shift;
		public int _GrainScaleShift { get { return grain_scale_shift; } set { grain_scale_shift = value; } }
		private int cb_mult;
		public int _CbMult { get { return cb_mult; } set { cb_mult = value; } }
		private int cb_luma_mult;
		public int _CbLumaMult { get { return cb_luma_mult; } set { cb_luma_mult = value; } }
		private int cb_offset;
		public int _CbOffset { get { return cb_offset; } set { cb_offset = value; } }
		private int cr_mult;
		public int _CrMult { get { return cr_mult; } set { cr_mult = value; } }
		private int cr_luma_mult;
		public int _CrLumaMult { get { return cr_luma_mult; } set { cr_luma_mult = value; } }
		private int cr_offset;
		public int _CrOffset { get { return cr_offset; } set { cr_offset = value; } }
		private int overlap_flag;
		public int _OverlapFlag { get { return overlap_flag; } set { overlap_flag = value; } }
		private int clip_to_restricted_range;
		public int _ClipToRestrictedRange { get { return clip_to_restricted_range; } set { clip_to_restricted_range = value; } }

        private void FilmGrainParams()
        {
			int i = 0;
			int tempGrainSeed = 0;
			int numPosLuma = 0;
			int numPosChroma = 0;

			if ((!(film_grain_params_present != 0) || (!(show_frame != 0) && !(showable_frame != 0))))
			{
				reset_grain_params(); 
				return;
			}
			stream.ReadFixed(1, out this.apply_grain, "apply_grain"); 

			if (!(apply_grain != 0))
			{
				reset_grain_params(); 
				return;
			}
			stream.ReadFixed(16, out this.grain_seed, "grain_seed"); 

			if ((frame_type == INTER_FRAME))
			{
				stream.ReadFixed(1, out this.update_grain, "update_grain"); 
			}
			else 
			{
				update_grain = 1;
			}

			if (!(update_grain != 0))
			{
				stream.ReadFixed(3, out this.film_grain_params_ref_idx, "film_grain_params_ref_idx"); 
				tempGrainSeed = grain_seed;
				load_grain_params(film_grain_params_ref_idx); 
				grain_seed = tempGrainSeed;
				return;
			}
			stream.ReadFixed(4, out this.num_y_points, "num_y_points"); 

			for (i = 0; (i < num_y_points); i++)
			{
				stream.ReadFixed(8, out this.point_y_value[i], "point_y_value"); 
				stream.ReadFixed(8, out this.point_y_scaling[i], "point_y_scaling"); 
			}

			if ((mono_chrome != 0))
			{
				chroma_scaling_from_luma = 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.chroma_scaling_from_luma, "chroma_scaling_from_luma"); 
			}

			if ((((mono_chrome != 0) || (chroma_scaling_from_luma != 0)) || (((subsampling_x == 1) && (subsampling_y == 1)) && (num_y_points == 0))))
			{
				num_cb_points = 0;
				num_cr_points = 0;
			}
			else 
			{
				stream.ReadFixed(4, out this.num_cb_points, "num_cb_points"); 

				for (i = 0; (i < num_cb_points); i++)
				{
					stream.ReadFixed(8, out this.point_cb_value[i], "point_cb_value"); 
					stream.ReadFixed(8, out this.point_cb_scaling[i], "point_cb_scaling"); 
				}
				stream.ReadFixed(4, out this.num_cr_points, "num_cr_points"); 

				for (i = 0; (i < num_cr_points); i++)
				{
					stream.ReadFixed(8, out this.point_cr_value[i], "point_cr_value"); 
					stream.ReadFixed(8, out this.point_cr_scaling[i], "point_cr_scaling"); 
				}
			}
			stream.ReadFixed(2, out this.grain_scaling_minus_8, "grain_scaling_minus_8"); 
			stream.ReadFixed(2, out this.ar_coeff_lag, "ar_coeff_lag"); 
			numPosLuma = ((2 * ar_coeff_lag) * (ar_coeff_lag + 1));

			if ((num_y_points != 0))
			{
				numPosChroma = (numPosLuma + 1);

				for (i = 0; (i < numPosLuma); i++)
				{
					stream.ReadFixed(8, out this.ar_coeffs_y_plus_128[i], "ar_coeffs_y_plus_128"); 
				}
			}
			else 
			{
				numPosChroma = numPosLuma;
			}

			if (((chroma_scaling_from_luma != 0) || (num_cb_points != 0)))
			{

				for (i = 0; (i < numPosChroma); i++)
				{
					stream.ReadFixed(8, out this.ar_coeffs_cb_plus_128[i], "ar_coeffs_cb_plus_128"); 
				}
			}

			if (((chroma_scaling_from_luma != 0) || (num_cr_points != 0)))
			{

				for (i = 0; (i < numPosChroma); i++)
				{
					stream.ReadFixed(8, out this.ar_coeffs_cr_plus_128[i], "ar_coeffs_cr_plus_128"); 
				}
			}
			stream.ReadFixed(2, out this.ar_coeff_shift_minus_6, "ar_coeff_shift_minus_6"); 
			stream.ReadFixed(2, out this.grain_scale_shift, "grain_scale_shift"); 

			if ((num_cb_points != 0))
			{
				stream.ReadFixed(8, out this.cb_mult, "cb_mult"); 
				stream.ReadFixed(8, out this.cb_luma_mult, "cb_luma_mult"); 
				stream.ReadFixed(9, out this.cb_offset, "cb_offset"); 
			}

			if ((num_cr_points != 0))
			{
				stream.ReadFixed(8, out this.cr_mult, "cr_mult"); 
				stream.ReadFixed(8, out this.cr_luma_mult, "cr_luma_mult"); 
				stream.ReadFixed(9, out this.cr_offset, "cr_offset"); 
			}
			stream.ReadFixed(1, out this.overlap_flag, "overlap_flag"); 
			stream.ReadFixed(1, out this.clip_to_restricted_range, "clip_to_restricted_range"); 
        }

        private void WriteFilmGrainParams()
        {
			int i = 0;
			int tempGrainSeed = 0;
			int numPosLuma = 0;
			int numPosChroma = 0;

			if ((!(film_grain_params_present != 0) || (!(show_frame != 0) && !(showable_frame != 0))))
			{
				reset_grain_params(); 
				return;
			}
			this.apply_grain = stream.Pick("apply_grain", _original != null ? _original.apply_grain : this.apply_grain, _edited != null ? _edited.apply_grain : _original != null ? _original.apply_grain : this.apply_grain);
			stream.WriteFixed(1, this.apply_grain, "apply_grain"); 

			if (!(apply_grain != 0))
			{
				reset_grain_params(); 
				return;
			}
			this.grain_seed = stream.Pick("grain_seed", _original != null ? _original.grain_seed : this.grain_seed, _edited != null ? _edited.grain_seed : _original != null ? _original.grain_seed : this.grain_seed);
			stream.WriteFixed(16, this.grain_seed, "grain_seed"); 

			if ((frame_type == INTER_FRAME))
			{
				this.update_grain = stream.Pick("update_grain", _original != null ? _original.update_grain : this.update_grain, _edited != null ? _edited.update_grain : _original != null ? _original.update_grain : this.update_grain);
				stream.WriteFixed(1, this.update_grain, "update_grain"); 
			}
			else 
			{
				update_grain = 1;
			}

			if (!(update_grain != 0))
			{
				this.film_grain_params_ref_idx = stream.Pick("film_grain_params_ref_idx", _original != null ? _original.film_grain_params_ref_idx : this.film_grain_params_ref_idx, _edited != null ? _edited.film_grain_params_ref_idx : _original != null ? _original.film_grain_params_ref_idx : this.film_grain_params_ref_idx);
				stream.WriteFixed(3, this.film_grain_params_ref_idx, "film_grain_params_ref_idx"); 
				tempGrainSeed = grain_seed;
				load_grain_params(film_grain_params_ref_idx); 
				grain_seed = tempGrainSeed;
				return;
			}
			this.num_y_points = stream.Pick("num_y_points", _original != null ? _original.num_y_points : this.num_y_points, _edited != null ? _edited.num_y_points : _original != null ? _original.num_y_points : this.num_y_points);
			stream.WriteFixed(4, this.num_y_points, "num_y_points"); 

			for (i = 0; (i < num_y_points); i++)
			{
				this.point_y_value[i] = stream.Pick("point_y_value", _original != null ? _original.point_y_value[i] : this.point_y_value[i], _edited != null ? _edited.point_y_value[i] : _original != null ? _original.point_y_value[i] : this.point_y_value[i]);
				stream.WriteFixed(8, this.point_y_value[i], "point_y_value"); 
				this.point_y_scaling[i] = stream.Pick("point_y_scaling", _original != null ? _original.point_y_scaling[i] : this.point_y_scaling[i], _edited != null ? _edited.point_y_scaling[i] : _original != null ? _original.point_y_scaling[i] : this.point_y_scaling[i]);
				stream.WriteFixed(8, this.point_y_scaling[i], "point_y_scaling"); 
			}

			if ((mono_chrome != 0))
			{
				chroma_scaling_from_luma = 0;
			}
			else 
			{
				this.chroma_scaling_from_luma = stream.Pick("chroma_scaling_from_luma", _original != null ? _original.chroma_scaling_from_luma : this.chroma_scaling_from_luma, _edited != null ? _edited.chroma_scaling_from_luma : _original != null ? _original.chroma_scaling_from_luma : this.chroma_scaling_from_luma);
				stream.WriteFixed(1, this.chroma_scaling_from_luma, "chroma_scaling_from_luma"); 
			}

			if ((((mono_chrome != 0) || (chroma_scaling_from_luma != 0)) || (((subsampling_x == 1) && (subsampling_y == 1)) && (num_y_points == 0))))
			{
				num_cb_points = 0;
				num_cr_points = 0;
			}
			else 
			{
				this.num_cb_points = stream.Pick("num_cb_points", _original != null ? _original.num_cb_points : this.num_cb_points, _edited != null ? _edited.num_cb_points : _original != null ? _original.num_cb_points : this.num_cb_points);
				stream.WriteFixed(4, this.num_cb_points, "num_cb_points"); 

				for (i = 0; (i < num_cb_points); i++)
				{
					this.point_cb_value[i] = stream.Pick("point_cb_value", _original != null ? _original.point_cb_value[i] : this.point_cb_value[i], _edited != null ? _edited.point_cb_value[i] : _original != null ? _original.point_cb_value[i] : this.point_cb_value[i]);
					stream.WriteFixed(8, this.point_cb_value[i], "point_cb_value"); 
					this.point_cb_scaling[i] = stream.Pick("point_cb_scaling", _original != null ? _original.point_cb_scaling[i] : this.point_cb_scaling[i], _edited != null ? _edited.point_cb_scaling[i] : _original != null ? _original.point_cb_scaling[i] : this.point_cb_scaling[i]);
					stream.WriteFixed(8, this.point_cb_scaling[i], "point_cb_scaling"); 
				}
				this.num_cr_points = stream.Pick("num_cr_points", _original != null ? _original.num_cr_points : this.num_cr_points, _edited != null ? _edited.num_cr_points : _original != null ? _original.num_cr_points : this.num_cr_points);
				stream.WriteFixed(4, this.num_cr_points, "num_cr_points"); 

				for (i = 0; (i < num_cr_points); i++)
				{
					this.point_cr_value[i] = stream.Pick("point_cr_value", _original != null ? _original.point_cr_value[i] : this.point_cr_value[i], _edited != null ? _edited.point_cr_value[i] : _original != null ? _original.point_cr_value[i] : this.point_cr_value[i]);
					stream.WriteFixed(8, this.point_cr_value[i], "point_cr_value"); 
					this.point_cr_scaling[i] = stream.Pick("point_cr_scaling", _original != null ? _original.point_cr_scaling[i] : this.point_cr_scaling[i], _edited != null ? _edited.point_cr_scaling[i] : _original != null ? _original.point_cr_scaling[i] : this.point_cr_scaling[i]);
					stream.WriteFixed(8, this.point_cr_scaling[i], "point_cr_scaling"); 
				}
			}
			this.grain_scaling_minus_8 = stream.Pick("grain_scaling_minus_8", _original != null ? _original.grain_scaling_minus_8 : this.grain_scaling_minus_8, _edited != null ? _edited.grain_scaling_minus_8 : _original != null ? _original.grain_scaling_minus_8 : this.grain_scaling_minus_8);
			stream.WriteFixed(2, this.grain_scaling_minus_8, "grain_scaling_minus_8"); 
			this.ar_coeff_lag = stream.Pick("ar_coeff_lag", _original != null ? _original.ar_coeff_lag : this.ar_coeff_lag, _edited != null ? _edited.ar_coeff_lag : _original != null ? _original.ar_coeff_lag : this.ar_coeff_lag);
			stream.WriteFixed(2, this.ar_coeff_lag, "ar_coeff_lag"); 
			numPosLuma = ((2 * ar_coeff_lag) * (ar_coeff_lag + 1));

			if ((num_y_points != 0))
			{
				numPosChroma = (numPosLuma + 1);

				for (i = 0; (i < numPosLuma); i++)
				{
					this.ar_coeffs_y_plus_128[i] = stream.Pick("ar_coeffs_y_plus_128", _original != null ? _original.ar_coeffs_y_plus_128[i] : this.ar_coeffs_y_plus_128[i], _edited != null ? _edited.ar_coeffs_y_plus_128[i] : _original != null ? _original.ar_coeffs_y_plus_128[i] : this.ar_coeffs_y_plus_128[i]);
					stream.WriteFixed(8, this.ar_coeffs_y_plus_128[i], "ar_coeffs_y_plus_128"); 
				}
			}
			else 
			{
				numPosChroma = numPosLuma;
			}

			if (((chroma_scaling_from_luma != 0) || (num_cb_points != 0)))
			{

				for (i = 0; (i < numPosChroma); i++)
				{
					this.ar_coeffs_cb_plus_128[i] = stream.Pick("ar_coeffs_cb_plus_128", _original != null ? _original.ar_coeffs_cb_plus_128[i] : this.ar_coeffs_cb_plus_128[i], _edited != null ? _edited.ar_coeffs_cb_plus_128[i] : _original != null ? _original.ar_coeffs_cb_plus_128[i] : this.ar_coeffs_cb_plus_128[i]);
					stream.WriteFixed(8, this.ar_coeffs_cb_plus_128[i], "ar_coeffs_cb_plus_128"); 
				}
			}

			if (((chroma_scaling_from_luma != 0) || (num_cr_points != 0)))
			{

				for (i = 0; (i < numPosChroma); i++)
				{
					this.ar_coeffs_cr_plus_128[i] = stream.Pick("ar_coeffs_cr_plus_128", _original != null ? _original.ar_coeffs_cr_plus_128[i] : this.ar_coeffs_cr_plus_128[i], _edited != null ? _edited.ar_coeffs_cr_plus_128[i] : _original != null ? _original.ar_coeffs_cr_plus_128[i] : this.ar_coeffs_cr_plus_128[i]);
					stream.WriteFixed(8, this.ar_coeffs_cr_plus_128[i], "ar_coeffs_cr_plus_128"); 
				}
			}
			this.ar_coeff_shift_minus_6 = stream.Pick("ar_coeff_shift_minus_6", _original != null ? _original.ar_coeff_shift_minus_6 : this.ar_coeff_shift_minus_6, _edited != null ? _edited.ar_coeff_shift_minus_6 : _original != null ? _original.ar_coeff_shift_minus_6 : this.ar_coeff_shift_minus_6);
			stream.WriteFixed(2, this.ar_coeff_shift_minus_6, "ar_coeff_shift_minus_6"); 
			this.grain_scale_shift = stream.Pick("grain_scale_shift", _original != null ? _original.grain_scale_shift : this.grain_scale_shift, _edited != null ? _edited.grain_scale_shift : _original != null ? _original.grain_scale_shift : this.grain_scale_shift);
			stream.WriteFixed(2, this.grain_scale_shift, "grain_scale_shift"); 

			if ((num_cb_points != 0))
			{
				this.cb_mult = stream.Pick("cb_mult", _original != null ? _original.cb_mult : this.cb_mult, _edited != null ? _edited.cb_mult : _original != null ? _original.cb_mult : this.cb_mult);
				stream.WriteFixed(8, this.cb_mult, "cb_mult"); 
				this.cb_luma_mult = stream.Pick("cb_luma_mult", _original != null ? _original.cb_luma_mult : this.cb_luma_mult, _edited != null ? _edited.cb_luma_mult : _original != null ? _original.cb_luma_mult : this.cb_luma_mult);
				stream.WriteFixed(8, this.cb_luma_mult, "cb_luma_mult"); 
				this.cb_offset = stream.Pick("cb_offset", _original != null ? _original.cb_offset : this.cb_offset, _edited != null ? _edited.cb_offset : _original != null ? _original.cb_offset : this.cb_offset);
				stream.WriteFixed(9, this.cb_offset, "cb_offset"); 
			}

			if ((num_cr_points != 0))
			{
				this.cr_mult = stream.Pick("cr_mult", _original != null ? _original.cr_mult : this.cr_mult, _edited != null ? _edited.cr_mult : _original != null ? _original.cr_mult : this.cr_mult);
				stream.WriteFixed(8, this.cr_mult, "cr_mult"); 
				this.cr_luma_mult = stream.Pick("cr_luma_mult", _original != null ? _original.cr_luma_mult : this.cr_luma_mult, _edited != null ? _edited.cr_luma_mult : _original != null ? _original.cr_luma_mult : this.cr_luma_mult);
				stream.WriteFixed(8, this.cr_luma_mult, "cr_luma_mult"); 
				this.cr_offset = stream.Pick("cr_offset", _original != null ? _original.cr_offset : this.cr_offset, _edited != null ? _edited.cr_offset : _original != null ? _original.cr_offset : this.cr_offset);
				stream.WriteFixed(9, this.cr_offset, "cr_offset"); 
			}
			this.overlap_flag = stream.Pick("overlap_flag", _original != null ? _original.overlap_flag : this.overlap_flag, _edited != null ? _edited.overlap_flag : _original != null ? _original.overlap_flag : this.overlap_flag);
			stream.WriteFixed(1, this.overlap_flag, "overlap_flag"); 
			this.clip_to_restricted_range = stream.Pick("clip_to_restricted_range", _original != null ? _original.clip_to_restricted_range : this.clip_to_restricted_range, _edited != null ? _edited.clip_to_restricted_range : _original != null ? _original.clip_to_restricted_range : this.clip_to_restricted_range);
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
		public int _UseSuperres { get { return use_superres; } set { use_superres = value; } }
		private int coded_denom;
		public int _CodedDenom { get { return coded_denom; } set { coded_denom = value; } }
		private int SuperresDenom;
		public int _SuperresDenom { get { return SuperresDenom; } set { SuperresDenom = value; } }

        private void SuperresParams()
        {

			if ((enable_superres != 0))
			{
				stream.ReadFixed(1, out this.use_superres, "use_superres"); 
			}
			else 
			{
				use_superres = 0;
			}

			if ((use_superres != 0))
			{
				stream.ReadVariable(SUPERRES_DENOM_BITS, out this.coded_denom, "coded_denom"); 
				SuperresDenom = (coded_denom + SUPERRES_DENOM_MIN);
			}
			else 
			{
				SuperresDenom = SUPERRES_NUM;
			}
			UpscaledWidth = FrameWidth;
			FrameWidth = (((UpscaledWidth * SUPERRES_NUM) + (SuperresDenom / 2)) / SuperresDenom);
        }

        private void WriteSuperresParams()
        {

			if ((enable_superres != 0))
			{
				this.use_superres = stream.Pick("use_superres", _original != null ? _original.use_superres : this.use_superres, _edited != null ? _edited.use_superres : _original != null ? _original.use_superres : this.use_superres);
				stream.WriteFixed(1, this.use_superres, "use_superres"); 
			}
			else 
			{
				use_superres = 0;
			}

			if ((use_superres != 0))
			{
				this.coded_denom = stream.Pick("coded_denom", _original != null ? _original.coded_denom : this.coded_denom, _edited != null ? _edited.coded_denom : _original != null ? _original.coded_denom : this.coded_denom);
				stream.WriteVariable(SUPERRES_DENOM_BITS, this.coded_denom, "coded_denom"); 
				SuperresDenom = (coded_denom + SUPERRES_DENOM_MIN);
			}
			else 
			{
				SuperresDenom = SUPERRES_NUM;
			}
			UpscaledWidth = FrameWidth;
			FrameWidth = (((UpscaledWidth * SUPERRES_NUM) + (SuperresDenom / 2)) / SuperresDenom);
        }

    /*
compute_image_size() { 
 MiCols = 2 * ( ( FrameWidth + 7 ) >> 3 )
 MiRows = 2 * ( ( FrameHeight + 7 ) >> 3 )
 }
    */
		private int MiCols;
		public int _MiCols { get { return MiCols; } set { MiCols = value; } }
		private int MiRows;
		public int _MiRows { get { return MiRows; } set { MiRows = value; } }

        private void ComputeImageSize()
        {
			MiCols = (2 * ((FrameWidth + 7) >> 3));
			MiRows = (2 * ((FrameHeight + 7) >> 3));
        }

    /*
decode_signed_subexp_with_ref( low, high, r ) { 
 x = decode_unsigned_subexp_with_ref(high - low, r - low)
 return x + low
}
    */
		private int low;
		public int _Low { get { return low; } set { low = value; } }
		private int high;
		public int _High { get { return high; } set { high = value; } }
		private int r;

        private int DecodeSignedSubexpWithRef(int low, int high, int r)
        {
			int x = 0;
			x = DecodeUnsignedSubexpWithRef((high - low), (r - low));
			return (x + low);
        }

        private int WriteDecodeSignedSubexpWithRef(int low, int high, int r)
        {
			int x = 0;
			x = WriteDecodeUnsignedSubexpWithRef((high - low), (r - low));
			return (x + low);
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
		private int mx;
		public int _Mx { get { return mx; } set { mx = value; } }

        private int DecodeUnsignedSubexpWithRef(int mx, int r)
        {
			int v = 0;
			v = DecodeSubexp(mx);

			if (((r << 1) <= mx))
			{
				return InverseRecenter(r, v);
			}
			else 
			{
				return ((mx - 1) - InverseRecenter(((mx - 1) - r), v));
			}
        }

        private int WriteDecodeUnsignedSubexpWithRef(int mx, int r)
        {
			int v = 0;
			v = WriteDecodeSubexp(mx);

			if (((r << 1) <= mx))
			{
				return InverseRecenter(r, v);
			}
			else 
			{
				return ((mx - 1) - InverseRecenter(((mx - 1) - r), v));
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
		private int numSyms;
		public int _NumSyms { get { return numSyms; } set { numSyms = value; } }
		private int subexp_final_bits;
		public int _SubexpFinalBits { get { return subexp_final_bits; } set { subexp_final_bits = value; } }
		private int subexp_more_bits;
		public int _SubexpMoreBits { get { return subexp_more_bits; } set { subexp_more_bits = value; } }
		private int subexp_bits;
		public int _SubexpBits { get { return subexp_bits; } set { subexp_bits = value; } }

        private int DecodeSubexp(int numSyms)
        {
			int i = 0;
			int mk = 0;
			int k = 0;
			int b2 = 0;
			int a = 0;
			i = 0;
			mk = 0;
			k = 3;

			while ((1 != 0))
			{
				b2 = ((i != 0) ? ((k + i) - 1) : k);
				a = (1 << b2);

				if ((numSyms <= (mk + (3 * a))))
				{
					stream.Read_ns((numSyms - mk), out this.subexp_final_bits, "subexp_final_bits"); 
					return (subexp_final_bits + mk);
				}
				else 
				{
					stream.ReadFixed(1, out this.subexp_more_bits, "subexp_more_bits"); 

					if ((subexp_more_bits != 0))
					{
						i++;
						mk += a;
					}
					else 
					{
						stream.ReadVariable(b2, out this.subexp_bits, "subexp_bits"); 
						return (subexp_bits + mk);
					}
				}
			}
        }

        private int WriteDecodeSubexp(int numSyms)
        {
			int i = 0;
			int mk = 0;
			int k = 0;
			int b2 = 0;
			int a = 0;
			i = 0;
			mk = 0;
			k = 3;

			while ((1 != 0))
			{
				b2 = ((i != 0) ? ((k + i) - 1) : k);
				a = (1 << b2);

				if ((numSyms <= (mk + (3 * a))))
				{
					this.subexp_final_bits = stream.Pick("subexp_final_bits", _original != null ? _original.subexp_final_bits : this.subexp_final_bits, _edited != null ? _edited.subexp_final_bits : _original != null ? _original.subexp_final_bits : this.subexp_final_bits);
					stream.Write_ns((numSyms - mk), this.subexp_final_bits, "subexp_final_bits"); 
					return (subexp_final_bits + mk);
				}
				else 
				{
					this.subexp_more_bits = stream.Pick("subexp_more_bits", _original != null ? _original.subexp_more_bits : this.subexp_more_bits, _edited != null ? _edited.subexp_more_bits : _original != null ? _original.subexp_more_bits : this.subexp_more_bits);
					stream.WriteFixed(1, this.subexp_more_bits, "subexp_more_bits"); 

					if ((subexp_more_bits != 0))
					{
						i++;
						mk += a;
					}
					else 
					{
						this.subexp_bits = stream.Pick("subexp_bits", _original != null ? _original.subexp_bits : this.subexp_bits, _edited != null ? _edited.subexp_bits : _original != null ? _original.subexp_bits : this.subexp_bits);
						stream.WriteVariable(b2, this.subexp_bits, "subexp_bits"); 
						return (subexp_bits + mk);
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
		private int v;

        private int InverseRecenter(int r, int v)
        {

			if ((v > (2 * r)))
			{
				return v;
			}
			else if (((v & 1) != 0))
			{
				return (r - ((v + 1) >> 1));
			}
			else 
			{
				return (r + (v >> 1));
			}
        }

    /*
temporal_delimiter_obu() { 
 SeenFrameHeader = 0
}
    */

        private void TemporalDelimiterObu()
        {
			SeenFrameHeader = 0;
        }

    /*
padding_obu() {
 obu_padding_length = PayloadBytesBeforeTrailingBits()
 for ( i = 0; i < obu_padding_length; i++ )
 obu_padding_byte f(8)
}
    */
		private int obu_padding_length;
		public int _ObuPaddingLength { get { return obu_padding_length; } set { obu_padding_length = value; } }
		private int obu_padding_byte;
		public int _ObuPaddingByte { get { return obu_padding_byte; } set { obu_padding_byte = value; } }

        private void PaddingObu()
        {
			int i = 0;
			obu_padding_length = PayloadBytesBeforeTrailingBits();

			for (i = 0; (i < obu_padding_length); i++)
			{
				stream.ReadFixed(8, out this.obu_padding_byte, "obu_padding_byte"); 
			}
        }

        private void WritePaddingObu()
        {
			int i = 0;
			obu_padding_length = PayloadBytesBeforeTrailingBits();

			for (i = 0; (i < obu_padding_length); i++)
			{
				this.obu_padding_byte = stream.Pick("obu_padding_byte", _original != null ? _original.obu_padding_byte : this.obu_padding_byte, _edited != null ? _edited.obu_padding_byte : _original != null ? _original.obu_padding_byte : this.obu_padding_byte);
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
 else
 MetadataUnknownPayload()
 }
    */
		private int metadata_type;
		public int _MetadataType { get { return metadata_type; } set { metadata_type = value; } }

        private void MetadataObu()
        {
			stream.ReadLeb128( out this.metadata_type, "metadata_type"); 

			if ((metadata_type == METADATA_TYPE_ITUT_T35))
			{
				MetadataItutT35(); 
			}
			else if ((metadata_type == METADATA_TYPE_HDR_CLL))
			{
				MetadataHdrCll(); 
			}
			else if ((metadata_type == METADATA_TYPE_HDR_MDCV))
			{
				MetadataHdrMdcv(); 
			}
			else if ((metadata_type == METADATA_TYPE_SCALABILITY))
			{
				MetadataScalability(); 
			}
			else if ((metadata_type == METADATA_TYPE_TIMECODE))
			{
				MetadataTimecode(); 
			}
			else 
			{
				MetadataUnknownPayload(); 
			}
        }

        private void WriteMetadataObu()
        {
			this.metadata_type = stream.Pick("metadata_type", _original != null ? _original.metadata_type : this.metadata_type, _edited != null ? _edited.metadata_type : _original != null ? _original.metadata_type : this.metadata_type);
			stream.WriteLeb128( this.metadata_type, "metadata_type"); 

			if ((metadata_type == METADATA_TYPE_ITUT_T35))
			{
				WriteMetadataItutT35(); 
			}
			else if ((metadata_type == METADATA_TYPE_HDR_CLL))
			{
				WriteMetadataHdrCll(); 
			}
			else if ((metadata_type == METADATA_TYPE_HDR_MDCV))
			{
				WriteMetadataHdrMdcv(); 
			}
			else if ((metadata_type == METADATA_TYPE_SCALABILITY))
			{
				WriteMetadataScalability(); 
			}
			else if ((metadata_type == METADATA_TYPE_TIMECODE))
			{
				WriteMetadataTimecode(); 
			}
			else 
			{
				MetadataUnknownPayload(); 
			}
        }

    /*
metadata_itut_t35() { 
 itu_t_t35_country_code f(8)
 if ( itu_t_t35_country_code == 0xFF ) {
 itu_t_t35_country_code_extension_byte f(8)
 }
 ItutT35PayloadBytes()
}
    */
		private int itu_t_t35_country_code;
		public int _ItutT35CountryCode { get { return itu_t_t35_country_code; } set { itu_t_t35_country_code = value; } }
		private int itu_t_t35_country_code_extension_byte;
		public int _ItutT35CountryCodeExtensionByte { get { return itu_t_t35_country_code_extension_byte; } set { itu_t_t35_country_code_extension_byte = value; } }

        private void MetadataItutT35()
        {
			stream.ReadFixed(8, out this.itu_t_t35_country_code, "itu_t_t35_country_code"); 

			if ((itu_t_t35_country_code == 0xFF))
			{
				stream.ReadFixed(8, out this.itu_t_t35_country_code_extension_byte, "itu_t_t35_country_code_extension_byte"); 
			}
			ItutT35PayloadBytes(); 
        }

        private void WriteMetadataItutT35()
        {
			this.itu_t_t35_country_code = stream.Pick("itu_t_t35_country_code", _original != null ? _original.itu_t_t35_country_code : this.itu_t_t35_country_code, _edited != null ? _edited.itu_t_t35_country_code : _original != null ? _original.itu_t_t35_country_code : this.itu_t_t35_country_code);
			stream.WriteFixed(8, this.itu_t_t35_country_code, "itu_t_t35_country_code"); 

			if ((itu_t_t35_country_code == 0xFF))
			{
				this.itu_t_t35_country_code_extension_byte = stream.Pick("itu_t_t35_country_code_extension_byte", _original != null ? _original.itu_t_t35_country_code_extension_byte : this.itu_t_t35_country_code_extension_byte, _edited != null ? _edited.itu_t_t35_country_code_extension_byte : _original != null ? _original.itu_t_t35_country_code_extension_byte : this.itu_t_t35_country_code_extension_byte);
				stream.WriteFixed(8, this.itu_t_t35_country_code_extension_byte, "itu_t_t35_country_code_extension_byte"); 
			}
			ItutT35PayloadBytes(); 
        }

    /*
metadata_hdr_cll() { 
 max_cll f(16)
 max_fall f(16)
}
    */
		private int max_cll;
		public int _MaxCll { get { return max_cll; } set { max_cll = value; } }
		private int max_fall;
		public int _MaxFall { get { return max_fall; } set { max_fall = value; } }

        private void MetadataHdrCll()
        {
			stream.ReadFixed(16, out this.max_cll, "max_cll"); 
			stream.ReadFixed(16, out this.max_fall, "max_fall"); 
        }

        private void WriteMetadataHdrCll()
        {
			this.max_cll = stream.Pick("max_cll", _original != null ? _original.max_cll : this.max_cll, _edited != null ? _edited.max_cll : _original != null ? _original.max_cll : this.max_cll);
			stream.WriteFixed(16, this.max_cll, "max_cll"); 
			this.max_fall = stream.Pick("max_fall", _original != null ? _original.max_fall : this.max_fall, _edited != null ? _edited.max_fall : _original != null ? _original.max_fall : this.max_fall);
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
		private AomArray<int> primary_chromaticity_x = new AomArray<int>();
		public AomArray<int> _PrimaryChromaticityx { get { return primary_chromaticity_x; } set { primary_chromaticity_x = value; } }
		private AomArray<int> primary_chromaticity_y = new AomArray<int>();
		public AomArray<int> _PrimaryChromaticityy { get { return primary_chromaticity_y; } set { primary_chromaticity_y = value; } }
		private int white_point_chromaticity_x;
		public int _WhitePointChromaticityx { get { return white_point_chromaticity_x; } set { white_point_chromaticity_x = value; } }
		private int white_point_chromaticity_y;
		public int _WhitePointChromaticityy { get { return white_point_chromaticity_y; } set { white_point_chromaticity_y = value; } }
		private int luminance_max;
		public int _LuminanceMax { get { return luminance_max; } set { luminance_max = value; } }
		private int luminance_min;
		public int _LuminanceMin { get { return luminance_min; } set { luminance_min = value; } }

        private void MetadataHdrMdcv()
        {
			int i = 0;

			for (i = 0; (i < 3); i++)
			{
				stream.ReadFixed(16, out this.primary_chromaticity_x[i], "primary_chromaticity_x"); 
				stream.ReadFixed(16, out this.primary_chromaticity_y[i], "primary_chromaticity_y"); 
			}
			stream.ReadFixed(16, out this.white_point_chromaticity_x, "white_point_chromaticity_x"); 
			stream.ReadFixed(16, out this.white_point_chromaticity_y, "white_point_chromaticity_y"); 
			stream.ReadFixed(32, out this.luminance_max, "luminance_max"); 
			stream.ReadFixed(32, out this.luminance_min, "luminance_min"); 
        }

        private void WriteMetadataHdrMdcv()
        {
			int i = 0;

			for (i = 0; (i < 3); i++)
			{
				this.primary_chromaticity_x[i] = stream.Pick("primary_chromaticity_x", _original != null ? _original.primary_chromaticity_x[i] : this.primary_chromaticity_x[i], _edited != null ? _edited.primary_chromaticity_x[i] : _original != null ? _original.primary_chromaticity_x[i] : this.primary_chromaticity_x[i]);
				stream.WriteFixed(16, this.primary_chromaticity_x[i], "primary_chromaticity_x"); 
				this.primary_chromaticity_y[i] = stream.Pick("primary_chromaticity_y", _original != null ? _original.primary_chromaticity_y[i] : this.primary_chromaticity_y[i], _edited != null ? _edited.primary_chromaticity_y[i] : _original != null ? _original.primary_chromaticity_y[i] : this.primary_chromaticity_y[i]);
				stream.WriteFixed(16, this.primary_chromaticity_y[i], "primary_chromaticity_y"); 
			}
			this.white_point_chromaticity_x = stream.Pick("white_point_chromaticity_x", _original != null ? _original.white_point_chromaticity_x : this.white_point_chromaticity_x, _edited != null ? _edited.white_point_chromaticity_x : _original != null ? _original.white_point_chromaticity_x : this.white_point_chromaticity_x);
			stream.WriteFixed(16, this.white_point_chromaticity_x, "white_point_chromaticity_x"); 
			this.white_point_chromaticity_y = stream.Pick("white_point_chromaticity_y", _original != null ? _original.white_point_chromaticity_y : this.white_point_chromaticity_y, _edited != null ? _edited.white_point_chromaticity_y : _original != null ? _original.white_point_chromaticity_y : this.white_point_chromaticity_y);
			stream.WriteFixed(16, this.white_point_chromaticity_y, "white_point_chromaticity_y"); 
			this.luminance_max = stream.Pick("luminance_max", _original != null ? _original.luminance_max : this.luminance_max, _edited != null ? _edited.luminance_max : _original != null ? _original.luminance_max : this.luminance_max);
			stream.WriteFixed(32, this.luminance_max, "luminance_max"); 
			this.luminance_min = stream.Pick("luminance_min", _original != null ? _original.luminance_min : this.luminance_min, _edited != null ? _edited.luminance_min : _original != null ? _original.luminance_min : this.luminance_min);
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
		public int _ScalabilityModeIdc { get { return scalability_mode_idc; } set { scalability_mode_idc = value; } }

        private void MetadataScalability()
        {
			stream.ReadFixed(8, out this.scalability_mode_idc, "scalability_mode_idc"); 

			if ((scalability_mode_idc == SCALABILITY_SS))
			{
				ScalabilityStructure(); 
			}
        }

        private void WriteMetadataScalability()
        {
			this.scalability_mode_idc = stream.Pick("scalability_mode_idc", _original != null ? _original.scalability_mode_idc : this.scalability_mode_idc, _edited != null ? _edited.scalability_mode_idc : _original != null ? _original.scalability_mode_idc : this.scalability_mode_idc);
			stream.WriteFixed(8, this.scalability_mode_idc, "scalability_mode_idc"); 

			if ((scalability_mode_idc == SCALABILITY_SS))
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
		public int _SpatialLayersCntMinus1 { get { return spatial_layers_cnt_minus_1; } set { spatial_layers_cnt_minus_1 = value; } }
		private int spatial_layer_dimensions_present_flag;
		public int _SpatialLayerDimensionsPresentFlag { get { return spatial_layer_dimensions_present_flag; } set { spatial_layer_dimensions_present_flag = value; } }
		private int spatial_layer_description_present_flag;
		public int _SpatialLayerDescriptionPresentFlag { get { return spatial_layer_description_present_flag; } set { spatial_layer_description_present_flag = value; } }
		private int temporal_group_description_present_flag;
		public int _TemporalGroupDescriptionPresentFlag { get { return temporal_group_description_present_flag; } set { temporal_group_description_present_flag = value; } }
		private int scalability_structure_reserved_3bits;
		public int _ScalabilityStructureReserved3bits { get { return scalability_structure_reserved_3bits; } set { scalability_structure_reserved_3bits = value; } }
		private AomArray<int> spatial_layer_max_width = new AomArray<int>();
		public AomArray<int> _SpatialLayerMaxWidth { get { return spatial_layer_max_width; } set { spatial_layer_max_width = value; } }
		private AomArray<int> spatial_layer_max_height = new AomArray<int>();
		public AomArray<int> _SpatialLayerMaxHeight { get { return spatial_layer_max_height; } set { spatial_layer_max_height = value; } }
		private AomArray<int> spatial_layer_ref_id = new AomArray<int>();
		public AomArray<int> _SpatialLayerRefId { get { return spatial_layer_ref_id; } set { spatial_layer_ref_id = value; } }
		private int temporal_group_size;
		public int _TemporalGroupSize { get { return temporal_group_size; } set { temporal_group_size = value; } }
		private AomArray<int> temporal_group_temporal_id = new AomArray<int>();
		public AomArray<int> _TemporalGroupTemporalId { get { return temporal_group_temporal_id; } set { temporal_group_temporal_id = value; } }
		private AomArray<int> temporal_group_temporal_switching_up_point_flag = new AomArray<int>();
		public AomArray<int> _TemporalGroupTemporalSwitchingUpPointFlag { get { return temporal_group_temporal_switching_up_point_flag; } set { temporal_group_temporal_switching_up_point_flag = value; } }
		private AomArray<int> temporal_group_spatial_switching_up_point_flag = new AomArray<int>();
		public AomArray<int> _TemporalGroupSpatialSwitchingUpPointFlag { get { return temporal_group_spatial_switching_up_point_flag; } set { temporal_group_spatial_switching_up_point_flag = value; } }
		private AomArray<int> temporal_group_ref_cnt = new AomArray<int>();
		public AomArray<int> _TemporalGroupRefCnt { get { return temporal_group_ref_cnt; } set { temporal_group_ref_cnt = value; } }
		private AomArray<AomArray<int>> temporal_group_ref_pic_diff = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _TemporalGroupRefPicDiff { get { return temporal_group_ref_pic_diff; } set { temporal_group_ref_pic_diff = value; } }

        private void ScalabilityStructure()
        {
			int i = 0;
			int j = 0;
			stream.ReadFixed(2, out this.spatial_layers_cnt_minus_1, "spatial_layers_cnt_minus_1"); 
			stream.ReadFixed(1, out this.spatial_layer_dimensions_present_flag, "spatial_layer_dimensions_present_flag"); 
			stream.ReadFixed(1, out this.spatial_layer_description_present_flag, "spatial_layer_description_present_flag"); 
			stream.ReadFixed(1, out this.temporal_group_description_present_flag, "temporal_group_description_present_flag"); 
			stream.ReadFixed(3, out this.scalability_structure_reserved_3bits, "scalability_structure_reserved_3bits"); 

			if ((spatial_layer_dimensions_present_flag != 0))
			{

				for (i = 0; (i <= spatial_layers_cnt_minus_1); i++)
				{
					stream.ReadFixed(16, out this.spatial_layer_max_width[i], "spatial_layer_max_width"); 
					stream.ReadFixed(16, out this.spatial_layer_max_height[i], "spatial_layer_max_height"); 
				}
			}

			if ((spatial_layer_description_present_flag != 0))
			{

				for (i = 0; (i <= spatial_layers_cnt_minus_1); i++)
				{
					stream.ReadFixed(8, out this.spatial_layer_ref_id[i], "spatial_layer_ref_id"); 
				}
			}

			if ((temporal_group_description_present_flag != 0))
			{
				stream.ReadFixed(8, out this.temporal_group_size, "temporal_group_size"); 

				for (i = 0; (i < temporal_group_size); i++)
				{
					stream.ReadFixed(3, out this.temporal_group_temporal_id[i], "temporal_group_temporal_id"); 
					stream.ReadFixed(1, out this.temporal_group_temporal_switching_up_point_flag[i], "temporal_group_temporal_switching_up_point_flag"); 
					stream.ReadFixed(1, out this.temporal_group_spatial_switching_up_point_flag[i], "temporal_group_spatial_switching_up_point_flag"); 
					stream.ReadFixed(3, out this.temporal_group_ref_cnt[i], "temporal_group_ref_cnt"); 

					for (j = 0; (j < temporal_group_ref_cnt[i]); j++)
					{
						stream.ReadFixed(8, out this.temporal_group_ref_pic_diff[i][j], "temporal_group_ref_pic_diff"); 
					}
				}
			}
        }

        private void WriteScalabilityStructure()
        {
			int i = 0;
			int j = 0;
			this.spatial_layers_cnt_minus_1 = stream.Pick("spatial_layers_cnt_minus_1", _original != null ? _original.spatial_layers_cnt_minus_1 : this.spatial_layers_cnt_minus_1, _edited != null ? _edited.spatial_layers_cnt_minus_1 : _original != null ? _original.spatial_layers_cnt_minus_1 : this.spatial_layers_cnt_minus_1);
			stream.WriteFixed(2, this.spatial_layers_cnt_minus_1, "spatial_layers_cnt_minus_1"); 
			this.spatial_layer_dimensions_present_flag = stream.Pick("spatial_layer_dimensions_present_flag", _original != null ? _original.spatial_layer_dimensions_present_flag : this.spatial_layer_dimensions_present_flag, _edited != null ? _edited.spatial_layer_dimensions_present_flag : _original != null ? _original.spatial_layer_dimensions_present_flag : this.spatial_layer_dimensions_present_flag);
			stream.WriteFixed(1, this.spatial_layer_dimensions_present_flag, "spatial_layer_dimensions_present_flag"); 
			this.spatial_layer_description_present_flag = stream.Pick("spatial_layer_description_present_flag", _original != null ? _original.spatial_layer_description_present_flag : this.spatial_layer_description_present_flag, _edited != null ? _edited.spatial_layer_description_present_flag : _original != null ? _original.spatial_layer_description_present_flag : this.spatial_layer_description_present_flag);
			stream.WriteFixed(1, this.spatial_layer_description_present_flag, "spatial_layer_description_present_flag"); 
			this.temporal_group_description_present_flag = stream.Pick("temporal_group_description_present_flag", _original != null ? _original.temporal_group_description_present_flag : this.temporal_group_description_present_flag, _edited != null ? _edited.temporal_group_description_present_flag : _original != null ? _original.temporal_group_description_present_flag : this.temporal_group_description_present_flag);
			stream.WriteFixed(1, this.temporal_group_description_present_flag, "temporal_group_description_present_flag"); 
			this.scalability_structure_reserved_3bits = stream.Pick("scalability_structure_reserved_3bits", _original != null ? _original.scalability_structure_reserved_3bits : this.scalability_structure_reserved_3bits, _edited != null ? _edited.scalability_structure_reserved_3bits : _original != null ? _original.scalability_structure_reserved_3bits : this.scalability_structure_reserved_3bits);
			stream.WriteFixed(3, this.scalability_structure_reserved_3bits, "scalability_structure_reserved_3bits"); 

			if ((spatial_layer_dimensions_present_flag != 0))
			{

				for (i = 0; (i <= spatial_layers_cnt_minus_1); i++)
				{
					this.spatial_layer_max_width[i] = stream.Pick("spatial_layer_max_width", _original != null ? _original.spatial_layer_max_width[i] : this.spatial_layer_max_width[i], _edited != null ? _edited.spatial_layer_max_width[i] : _original != null ? _original.spatial_layer_max_width[i] : this.spatial_layer_max_width[i]);
					stream.WriteFixed(16, this.spatial_layer_max_width[i], "spatial_layer_max_width"); 
					this.spatial_layer_max_height[i] = stream.Pick("spatial_layer_max_height", _original != null ? _original.spatial_layer_max_height[i] : this.spatial_layer_max_height[i], _edited != null ? _edited.spatial_layer_max_height[i] : _original != null ? _original.spatial_layer_max_height[i] : this.spatial_layer_max_height[i]);
					stream.WriteFixed(16, this.spatial_layer_max_height[i], "spatial_layer_max_height"); 
				}
			}

			if ((spatial_layer_description_present_flag != 0))
			{

				for (i = 0; (i <= spatial_layers_cnt_minus_1); i++)
				{
					this.spatial_layer_ref_id[i] = stream.Pick("spatial_layer_ref_id", _original != null ? _original.spatial_layer_ref_id[i] : this.spatial_layer_ref_id[i], _edited != null ? _edited.spatial_layer_ref_id[i] : _original != null ? _original.spatial_layer_ref_id[i] : this.spatial_layer_ref_id[i]);
					stream.WriteFixed(8, this.spatial_layer_ref_id[i], "spatial_layer_ref_id"); 
				}
			}

			if ((temporal_group_description_present_flag != 0))
			{
				this.temporal_group_size = stream.Pick("temporal_group_size", _original != null ? _original.temporal_group_size : this.temporal_group_size, _edited != null ? _edited.temporal_group_size : _original != null ? _original.temporal_group_size : this.temporal_group_size);
				stream.WriteFixed(8, this.temporal_group_size, "temporal_group_size"); 

				for (i = 0; (i < temporal_group_size); i++)
				{
					this.temporal_group_temporal_id[i] = stream.Pick("temporal_group_temporal_id", _original != null ? _original.temporal_group_temporal_id[i] : this.temporal_group_temporal_id[i], _edited != null ? _edited.temporal_group_temporal_id[i] : _original != null ? _original.temporal_group_temporal_id[i] : this.temporal_group_temporal_id[i]);
					stream.WriteFixed(3, this.temporal_group_temporal_id[i], "temporal_group_temporal_id"); 
					this.temporal_group_temporal_switching_up_point_flag[i] = stream.Pick("temporal_group_temporal_switching_up_point_flag", _original != null ? _original.temporal_group_temporal_switching_up_point_flag[i] : this.temporal_group_temporal_switching_up_point_flag[i], _edited != null ? _edited.temporal_group_temporal_switching_up_point_flag[i] : _original != null ? _original.temporal_group_temporal_switching_up_point_flag[i] : this.temporal_group_temporal_switching_up_point_flag[i]);
					stream.WriteFixed(1, this.temporal_group_temporal_switching_up_point_flag[i], "temporal_group_temporal_switching_up_point_flag"); 
					this.temporal_group_spatial_switching_up_point_flag[i] = stream.Pick("temporal_group_spatial_switching_up_point_flag", _original != null ? _original.temporal_group_spatial_switching_up_point_flag[i] : this.temporal_group_spatial_switching_up_point_flag[i], _edited != null ? _edited.temporal_group_spatial_switching_up_point_flag[i] : _original != null ? _original.temporal_group_spatial_switching_up_point_flag[i] : this.temporal_group_spatial_switching_up_point_flag[i]);
					stream.WriteFixed(1, this.temporal_group_spatial_switching_up_point_flag[i], "temporal_group_spatial_switching_up_point_flag"); 
					this.temporal_group_ref_cnt[i] = stream.Pick("temporal_group_ref_cnt", _original != null ? _original.temporal_group_ref_cnt[i] : this.temporal_group_ref_cnt[i], _edited != null ? _edited.temporal_group_ref_cnt[i] : _original != null ? _original.temporal_group_ref_cnt[i] : this.temporal_group_ref_cnt[i]);
					stream.WriteFixed(3, this.temporal_group_ref_cnt[i], "temporal_group_ref_cnt"); 

					for (j = 0; (j < temporal_group_ref_cnt[i]); j++)
					{
						this.temporal_group_ref_pic_diff[i][j] = stream.Pick("temporal_group_ref_pic_diff", _original != null ? _original.temporal_group_ref_pic_diff[i][j] : this.temporal_group_ref_pic_diff[i][j], _edited != null ? _edited.temporal_group_ref_pic_diff[i][j] : _original != null ? _original.temporal_group_ref_pic_diff[i][j] : this.temporal_group_ref_pic_diff[i][j]);
						stream.WriteFixed(8, this.temporal_group_ref_pic_diff[i][j], "temporal_group_ref_pic_diff"); 
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
		public int _CountingType { get { return counting_type; } set { counting_type = value; } }
		private int full_timestamp_flag;
		public int _FullTimestampFlag { get { return full_timestamp_flag; } set { full_timestamp_flag = value; } }
		private int discontinuity_flag;
		public int _DiscontinuityFlag { get { return discontinuity_flag; } set { discontinuity_flag = value; } }
		private int cnt_dropped_flag;
		public int _CntDroppedFlag { get { return cnt_dropped_flag; } set { cnt_dropped_flag = value; } }
		private int n_frames;
		public int _nFrames { get { return n_frames; } set { n_frames = value; } }
		private int seconds_value;
		public int _SecondsValue { get { return seconds_value; } set { seconds_value = value; } }
		private int minutes_value;
		public int _MinutesValue { get { return minutes_value; } set { minutes_value = value; } }
		private int hours_value;
		public int _HoursValue { get { return hours_value; } set { hours_value = value; } }
		private int seconds_flag;
		public int _SecondsFlag { get { return seconds_flag; } set { seconds_flag = value; } }
		private int minutes_flag;
		public int _MinutesFlag { get { return minutes_flag; } set { minutes_flag = value; } }
		private int hours_flag;
		public int _HoursFlag { get { return hours_flag; } set { hours_flag = value; } }
		private int time_offset_length;
		public int _TimeOffsetLength { get { return time_offset_length; } set { time_offset_length = value; } }
		private int time_offset_value;
		public int _TimeOffsetValue { get { return time_offset_value; } set { time_offset_value = value; } }

        private void MetadataTimecode()
        {
			stream.ReadFixed(5, out this.counting_type, "counting_type"); 
			stream.ReadFixed(1, out this.full_timestamp_flag, "full_timestamp_flag"); 
			stream.ReadFixed(1, out this.discontinuity_flag, "discontinuity_flag"); 
			stream.ReadFixed(1, out this.cnt_dropped_flag, "cnt_dropped_flag"); 
			stream.ReadFixed(9, out this.n_frames, "n_frames"); 

			if ((full_timestamp_flag != 0))
			{
				stream.ReadFixed(6, out this.seconds_value, "seconds_value"); 
				stream.ReadFixed(6, out this.minutes_value, "minutes_value"); 
				stream.ReadFixed(5, out this.hours_value, "hours_value"); 
			}
			else 
			{
				stream.ReadFixed(1, out this.seconds_flag, "seconds_flag"); 

				if ((seconds_flag != 0))
				{
					stream.ReadFixed(6, out this.seconds_value, "seconds_value"); 
					stream.ReadFixed(1, out this.minutes_flag, "minutes_flag"); 

					if ((minutes_flag != 0))
					{
						stream.ReadFixed(6, out this.minutes_value, "minutes_value"); 
						stream.ReadFixed(1, out this.hours_flag, "hours_flag"); 

						if ((hours_flag != 0))
						{
							stream.ReadFixed(5, out this.hours_value, "hours_value"); 
						}
					}
				}
			}
			stream.ReadFixed(5, out this.time_offset_length, "time_offset_length"); 

			if ((time_offset_length > 0))
			{
				stream.ReadVariable(time_offset_length, out this.time_offset_value, "time_offset_value"); 
			}
        }

        private void WriteMetadataTimecode()
        {
			this.counting_type = stream.Pick("counting_type", _original != null ? _original.counting_type : this.counting_type, _edited != null ? _edited.counting_type : _original != null ? _original.counting_type : this.counting_type);
			stream.WriteFixed(5, this.counting_type, "counting_type"); 
			this.full_timestamp_flag = stream.Pick("full_timestamp_flag", _original != null ? _original.full_timestamp_flag : this.full_timestamp_flag, _edited != null ? _edited.full_timestamp_flag : _original != null ? _original.full_timestamp_flag : this.full_timestamp_flag);
			stream.WriteFixed(1, this.full_timestamp_flag, "full_timestamp_flag"); 
			this.discontinuity_flag = stream.Pick("discontinuity_flag", _original != null ? _original.discontinuity_flag : this.discontinuity_flag, _edited != null ? _edited.discontinuity_flag : _original != null ? _original.discontinuity_flag : this.discontinuity_flag);
			stream.WriteFixed(1, this.discontinuity_flag, "discontinuity_flag"); 
			this.cnt_dropped_flag = stream.Pick("cnt_dropped_flag", _original != null ? _original.cnt_dropped_flag : this.cnt_dropped_flag, _edited != null ? _edited.cnt_dropped_flag : _original != null ? _original.cnt_dropped_flag : this.cnt_dropped_flag);
			stream.WriteFixed(1, this.cnt_dropped_flag, "cnt_dropped_flag"); 
			this.n_frames = stream.Pick("n_frames", _original != null ? _original.n_frames : this.n_frames, _edited != null ? _edited.n_frames : _original != null ? _original.n_frames : this.n_frames);
			stream.WriteFixed(9, this.n_frames, "n_frames"); 

			if ((full_timestamp_flag != 0))
			{
				this.seconds_value = stream.Pick("seconds_value", _original != null ? _original.seconds_value : this.seconds_value, _edited != null ? _edited.seconds_value : _original != null ? _original.seconds_value : this.seconds_value);
				stream.WriteFixed(6, this.seconds_value, "seconds_value"); 
				this.minutes_value = stream.Pick("minutes_value", _original != null ? _original.minutes_value : this.minutes_value, _edited != null ? _edited.minutes_value : _original != null ? _original.minutes_value : this.minutes_value);
				stream.WriteFixed(6, this.minutes_value, "minutes_value"); 
				this.hours_value = stream.Pick("hours_value", _original != null ? _original.hours_value : this.hours_value, _edited != null ? _edited.hours_value : _original != null ? _original.hours_value : this.hours_value);
				stream.WriteFixed(5, this.hours_value, "hours_value"); 
			}
			else 
			{
				this.seconds_flag = stream.Pick("seconds_flag", _original != null ? _original.seconds_flag : this.seconds_flag, _edited != null ? _edited.seconds_flag : _original != null ? _original.seconds_flag : this.seconds_flag);
				stream.WriteFixed(1, this.seconds_flag, "seconds_flag"); 

				if ((seconds_flag != 0))
				{
					this.seconds_value = stream.Pick("seconds_value", _original != null ? _original.seconds_value : this.seconds_value, _edited != null ? _edited.seconds_value : _original != null ? _original.seconds_value : this.seconds_value);
					stream.WriteFixed(6, this.seconds_value, "seconds_value"); 
					this.minutes_flag = stream.Pick("minutes_flag", _original != null ? _original.minutes_flag : this.minutes_flag, _edited != null ? _edited.minutes_flag : _original != null ? _original.minutes_flag : this.minutes_flag);
					stream.WriteFixed(1, this.minutes_flag, "minutes_flag"); 

					if ((minutes_flag != 0))
					{
						this.minutes_value = stream.Pick("minutes_value", _original != null ? _original.minutes_value : this.minutes_value, _edited != null ? _edited.minutes_value : _original != null ? _original.minutes_value : this.minutes_value);
						stream.WriteFixed(6, this.minutes_value, "minutes_value"); 
						this.hours_flag = stream.Pick("hours_flag", _original != null ? _original.hours_flag : this.hours_flag, _edited != null ? _edited.hours_flag : _original != null ? _original.hours_flag : this.hours_flag);
						stream.WriteFixed(1, this.hours_flag, "hours_flag"); 

						if ((hours_flag != 0))
						{
							this.hours_value = stream.Pick("hours_value", _original != null ? _original.hours_value : this.hours_value, _edited != null ? _edited.hours_value : _original != null ? _original.hours_value : this.hours_value);
							stream.WriteFixed(5, this.hours_value, "hours_value"); 
						}
					}
				}
			}
			this.time_offset_length = stream.Pick("time_offset_length", _original != null ? _original.time_offset_length : this.time_offset_length, _edited != null ? _edited.time_offset_length : _original != null ? _original.time_offset_length : this.time_offset_length);
			stream.WriteFixed(5, this.time_offset_length, "time_offset_length"); 

			if ((time_offset_length > 0))
			{
				this.time_offset_value = stream.Pick("time_offset_value", _original != null ? _original.time_offset_value : this.time_offset_value, _edited != null ? _edited.time_offset_value : _original != null ? _original.time_offset_value : this.time_offset_value);
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

        private void FrameObu(int sz)
        {
			int startBitPos = 0;
			int endBitPos = 0;
			int headerBytes = 0;
			startBitPos = get_position();
			FrameHeaderObu(); 
			ByteAlignment(); 
			endBitPos = get_position();
			headerBytes = ((endBitPos - startBitPos) / 8);
			sz -= headerBytes;
			TileGroupObu(sz); 
        }

        private void WriteFrameObu(int sz)
        {
			int startBitPos = 0;
			int endBitPos = 0;
			int headerBytes = 0;
			startBitPos = get_position();
			WriteFrameHeaderObu(); 
			WriteByteAlignment(); 
			endBitPos = get_position();
			headerBytes = ((endBitPos - startBitPos) / 8);
			sz -= headerBytes;
			WriteTileGroupObu(sz); 
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
 /* if ( !disable_frame_end_update_cdf ) {
 frame_end_update_cdf()
 } *//*
 decode_frame_wrapup()
 SeenFrameHeader = 0
 }
 }
    */
		private int NumTiles;
		public int _NumTiles { get { return NumTiles; } set { NumTiles = value; } }
		private int tile_start_and_end_present_flag;
		public int _TileStartAndEndPresentFlag { get { return tile_start_and_end_present_flag; } set { tile_start_and_end_present_flag = value; } }
		private int tg_start;
		public int _TgStart { get { return tg_start; } set { tg_start = value; } }
		private int tg_end;
		public int _TgEnd { get { return tg_end; } set { tg_end = value; } }
		private int tile_size_minus_1;
		public int _TileSizeMinus1 { get { return tile_size_minus_1; } set { tile_size_minus_1 = value; } }
		private int MiRowStart;
		public int _MiRowStart { get { return MiRowStart; } set { MiRowStart = value; } }
		private int MiRowEnd;
		public int _MiRowEnd { get { return MiRowEnd; } set { MiRowEnd = value; } }
		private int MiColStart;
		public int _MiColStart { get { return MiColStart; } set { MiColStart = value; } }
		private int MiColEnd;
		public int _MiColEnd { get { return MiColEnd; } set { MiColEnd = value; } }
		private int CurrentQIndex;
		public int _CurrentQIndex { get { return CurrentQIndex; } set { CurrentQIndex = value; } }

        private void TileGroupObu(int sz)
        {
			int TileNum = 0;
			int startBitPos = 0;
			int tileBits = 0;
			int endBitPos = 0;
			int headerBytes = 0;
			int tileRow = 0;
			int tileCol = 0;
			int lastTile = 0;
			int tileSize = 0;
			NumTiles = (TileCols * TileRows);
			startBitPos = get_position();
			tile_start_and_end_present_flag = 0;

			if ((NumTiles > 1))
			{
				stream.ReadFixed(1, out this.tile_start_and_end_present_flag, "tile_start_and_end_present_flag"); 
			}

			if (((NumTiles == 1) || !(tile_start_and_end_present_flag != 0)))
			{
				tg_start = 0;
				tg_end = (NumTiles - 1);
			}
			else 
			{
				tileBits = (TileColsLog2 + TileRowsLog2);
				stream.ReadVariable(tileBits, out this.tg_start, "tg_start"); 
				stream.ReadVariable(tileBits, out this.tg_end, "tg_end"); 
			}
			ByteAlignment(); 
			endBitPos = get_position();
			headerBytes = ((endBitPos - startBitPos) / 8);
			sz -= headerBytes;

			for (TileNum = tg_start; (TileNum <= tg_end); TileNum++)
			{
				tileRow = (TileNum / TileCols);
				tileCol = (TileNum % TileCols);
				lastTile = ((TileNum == tg_end) ? 1 : 0);

				if ((lastTile != 0))
				{
					tileSize = sz;
				}
				else 
				{
					stream.ReadLe(TileSizeBytes, out this.tile_size_minus_1, "tile_size_minus_1"); 
					tileSize = (tile_size_minus_1 + 1);
					sz -= (tileSize + TileSizeBytes);
				}
				MiRowStart = MiRowStarts[tileRow];
				MiRowEnd = MiRowStarts[(tileRow + 1)];
				MiColStart = MiColStarts[tileCol];
				MiColEnd = MiColStarts[(tileCol + 1)];
				CurrentQIndex = base_q_idx;
				init_symbol(tileSize); 
				decode_tile(); 
				exit_symbol(); 
			}

			if ((tg_end == (NumTiles - 1)))
			{
/*  if ( !disable_frame_end_update_cdf ) {
 frame_end_update_cdf()
 }  */

				decode_frame_wrapup(); 
				SeenFrameHeader = 0;
			}
        }

        private void WriteTileGroupObu(int sz)
        {
			int TileNum = 0;
			int startBitPos = 0;
			int tileBits = 0;
			int endBitPos = 0;
			int headerBytes = 0;
			int tileRow = 0;
			int tileCol = 0;
			int lastTile = 0;
			int tileSize = 0;
			NumTiles = (TileCols * TileRows);
			startBitPos = get_position();
			tile_start_and_end_present_flag = 0;

			if ((NumTiles > 1))
			{
				this.tile_start_and_end_present_flag = stream.Pick("tile_start_and_end_present_flag", _original != null ? _original.tile_start_and_end_present_flag : this.tile_start_and_end_present_flag, _edited != null ? _edited.tile_start_and_end_present_flag : _original != null ? _original.tile_start_and_end_present_flag : this.tile_start_and_end_present_flag);
				stream.WriteFixed(1, this.tile_start_and_end_present_flag, "tile_start_and_end_present_flag"); 
			}

			if (((NumTiles == 1) || !(tile_start_and_end_present_flag != 0)))
			{
				tg_start = 0;
				tg_end = (NumTiles - 1);
			}
			else 
			{
				tileBits = (TileColsLog2 + TileRowsLog2);
				this.tg_start = stream.Pick("tg_start", _original != null ? _original.tg_start : this.tg_start, _edited != null ? _edited.tg_start : _original != null ? _original.tg_start : this.tg_start);
				stream.WriteVariable(tileBits, this.tg_start, "tg_start"); 
				this.tg_end = stream.Pick("tg_end", _original != null ? _original.tg_end : this.tg_end, _edited != null ? _edited.tg_end : _original != null ? _original.tg_end : this.tg_end);
				stream.WriteVariable(tileBits, this.tg_end, "tg_end"); 
			}
			WriteByteAlignment(); 
			endBitPos = get_position();
			headerBytes = ((endBitPos - startBitPos) / 8);
			sz -= headerBytes;

			for (TileNum = tg_start; (TileNum <= tg_end); TileNum++)
			{
				tileRow = (TileNum / TileCols);
				tileCol = (TileNum % TileCols);
				lastTile = ((TileNum == tg_end) ? 1 : 0);

				if ((lastTile != 0))
				{
					tileSize = sz;
				}
				else 
				{
					this.tile_size_minus_1 = stream.Pick("tile_size_minus_1", _original != null ? _original.tile_size_minus_1 : this.tile_size_minus_1, _edited != null ? _edited.tile_size_minus_1 : _original != null ? _original.tile_size_minus_1 : this.tile_size_minus_1);
					stream.WriteLe(TileSizeBytes, this.tile_size_minus_1, "tile_size_minus_1"); 
					tileSize = (tile_size_minus_1 + 1);
					sz -= (tileSize + TileSizeBytes);
				}
				MiRowStart = MiRowStarts[tileRow];
				MiRowEnd = MiRowStarts[(tileRow + 1)];
				MiColStart = MiColStarts[tileCol];
				MiColEnd = MiColStarts[(tileCol + 1)];
				CurrentQIndex = base_q_idx;
				init_symbol(tileSize); 
				decode_tile(); 
				exit_symbol(); 
			}

			if ((tg_end == (NumTiles - 1)))
			{
/*  if ( !disable_frame_end_update_cdf ) {
 frame_end_update_cdf()
 }  */

				decode_frame_wrapup(); 
				SeenFrameHeader = 0;
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
		public int _OutputFrameWidthInTilesMinus1 { get { return output_frame_width_in_tiles_minus_1; } set { output_frame_width_in_tiles_minus_1 = value; } }
		private int output_frame_height_in_tiles_minus_1;
		public int _OutputFrameHeightInTilesMinus1 { get { return output_frame_height_in_tiles_minus_1; } set { output_frame_height_in_tiles_minus_1 = value; } }
		private int tile_count_minus_1;
		public int _TileCountMinus1 { get { return tile_count_minus_1; } set { tile_count_minus_1 = value; } }
		private int tile = 0;

        private void TileListObu()
        {
			int tile = 0;
			stream.ReadFixed(8, out this.output_frame_width_in_tiles_minus_1, "output_frame_width_in_tiles_minus_1"); 
			stream.ReadFixed(8, out this.output_frame_height_in_tiles_minus_1, "output_frame_height_in_tiles_minus_1"); 
			stream.ReadFixed(16, out this.tile_count_minus_1, "tile_count_minus_1"); 

			for (tile = 0; (tile <= tile_count_minus_1); tile++)
			{
				TileListEntry(); 
			}
        }

        private void WriteTileListObu()
        {
			int tile = 0;
			this.output_frame_width_in_tiles_minus_1 = stream.Pick("output_frame_width_in_tiles_minus_1", _original != null ? _original.output_frame_width_in_tiles_minus_1 : this.output_frame_width_in_tiles_minus_1, _edited != null ? _edited.output_frame_width_in_tiles_minus_1 : _original != null ? _original.output_frame_width_in_tiles_minus_1 : this.output_frame_width_in_tiles_minus_1);
			stream.WriteFixed(8, this.output_frame_width_in_tiles_minus_1, "output_frame_width_in_tiles_minus_1"); 
			this.output_frame_height_in_tiles_minus_1 = stream.Pick("output_frame_height_in_tiles_minus_1", _original != null ? _original.output_frame_height_in_tiles_minus_1 : this.output_frame_height_in_tiles_minus_1, _edited != null ? _edited.output_frame_height_in_tiles_minus_1 : _original != null ? _original.output_frame_height_in_tiles_minus_1 : this.output_frame_height_in_tiles_minus_1);
			stream.WriteFixed(8, this.output_frame_height_in_tiles_minus_1, "output_frame_height_in_tiles_minus_1"); 
			this.tile_count_minus_1 = stream.Pick("tile_count_minus_1", _original != null ? _original.tile_count_minus_1 : this.tile_count_minus_1, _edited != null ? _edited.tile_count_minus_1 : _original != null ? _original.tile_count_minus_1 : this.tile_count_minus_1);
			stream.WriteFixed(16, this.tile_count_minus_1, "tile_count_minus_1"); 

			for (tile = 0; (tile <= tile_count_minus_1); tile++)
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
		public int _AnchorFrameIdx { get { return anchor_frame_idx; } set { anchor_frame_idx = value; } }
		private int anchor_tile_row;
		public int _AnchorTileRow { get { return anchor_tile_row; } set { anchor_tile_row = value; } }
		private int anchor_tile_col;
		public int _AnchorTileCol { get { return anchor_tile_col; } set { anchor_tile_col = value; } }
		private int tile_data_size_minus_1;
		public int _TileDataSizeMinus1 { get { return tile_data_size_minus_1; } set { tile_data_size_minus_1 = value; } }
		private int N;
		private byte[] coded_tile_data;
		public byte[] _CodedTileData { get { return coded_tile_data; } set { coded_tile_data = value; } }

        private void TileListEntry()
        {
			stream.ReadFixed(8, out this.anchor_frame_idx, "anchor_frame_idx"); 
			stream.ReadFixed(8, out this.anchor_tile_row, "anchor_tile_row"); 
			stream.ReadFixed(8, out this.anchor_tile_col, "anchor_tile_col"); 
			stream.ReadFixed(16, out this.tile_data_size_minus_1, "tile_data_size_minus_1"); 
			N = (8 * (tile_data_size_minus_1 + 1));
			stream.ReadBytes(N, out this.coded_tile_data, "coded_tile_data"); 
        }

        private void WriteTileListEntry()
        {
			this.anchor_frame_idx = stream.Pick("anchor_frame_idx", _original != null ? _original.anchor_frame_idx : this.anchor_frame_idx, _edited != null ? _edited.anchor_frame_idx : _original != null ? _original.anchor_frame_idx : this.anchor_frame_idx);
			stream.WriteFixed(8, this.anchor_frame_idx, "anchor_frame_idx"); 
			this.anchor_tile_row = stream.Pick("anchor_tile_row", _original != null ? _original.anchor_tile_row : this.anchor_tile_row, _edited != null ? _edited.anchor_tile_row : _original != null ? _original.anchor_tile_row : this.anchor_tile_row);
			stream.WriteFixed(8, this.anchor_tile_row, "anchor_tile_row"); 
			this.anchor_tile_col = stream.Pick("anchor_tile_col", _original != null ? _original.anchor_tile_col : this.anchor_tile_col, _edited != null ? _edited.anchor_tile_col : _original != null ? _original.anchor_tile_col : this.anchor_tile_col);
			stream.WriteFixed(8, this.anchor_tile_col, "anchor_tile_col"); 
			this.tile_data_size_minus_1 = stream.Pick("tile_data_size_minus_1", _original != null ? _original.tile_data_size_minus_1 : this.tile_data_size_minus_1, _edited != null ? _edited.tile_data_size_minus_1 : _original != null ? _original.tile_data_size_minus_1 : this.tile_data_size_minus_1);
			stream.WriteFixed(16, this.tile_data_size_minus_1, "tile_data_size_minus_1"); 
			N = (8 * (tile_data_size_minus_1 + 1));
			this.coded_tile_data = stream.Pick("coded_tile_data", _original != null ? _original.coded_tile_data : this.coded_tile_data, _edited != null ? _edited.coded_tile_data : _original != null ? _original.coded_tile_data : this.coded_tile_data);
			stream.WriteBytes(N, this.coded_tile_data, "coded_tile_data"); 
        }

    /*
set_frame_refs() {
/* AV1 Bitstream & Decoding Process Specification, 7.8 Set frame refs process: its code, inc order *//*
for ( i = 0; i < REFS_PER_FRAME; i++ )
  ref_frame_idx[ i ] = -1
ref_frame_idx[ LAST_FRAME - LAST_FRAME ] = last_frame_idx
ref_frame_idx[ GOLDEN_FRAME - LAST_FRAME ] = gold_frame_idx
for ( i = 0; i < NUM_REF_FRAMES; i++ )
  usedFrame[ i ] = 0
usedFrame[ last_frame_idx ] = 1
usedFrame[ gold_frame_idx ] = 1
/* "A variable curFrameHint isc set equal to 1 << (OrderHintBits - 1)." *//*
curFrameHint = 1 << (OrderHintBits - 1)
for ( i = 0; i < NUM_REF_FRAMES; i++ )
  shiftedOrderHints[ i ] = curFrameHint + get_relative_dist( RefOrderHint[ i ], OrderHint )
/* "The variable lastOrderHint ... isc set equal to shiftedOrderHints[ last_frame_idx ]." *//*
lastOrderHint = shiftedOrderHints[ last_frame_idx ]
/* "The variable goldOrderHint ... isc set equal to shiftedOrderHints[ gold_frame_idx ]." *//*
goldOrderHint = shiftedOrderHints[ gold_frame_idx ]
refc = find_latest_backward()
if ( refc >= 0 ) {
  ref_frame_idx[ ALTREF_FRAME - LAST_FRAME ] = refc
  usedFrame[ refc ] = 1
}
refc = find_earliest_backward()
if ( refc >= 0 ) {
  ref_frame_idx[ BWDREF_FRAME - LAST_FRAME ] = refc
  usedFrame[ refc ] = 1
}
refc = find_earliest_backward()
if ( refc >= 0 ) {
  ref_frame_idx[ ALTREF2_FRAME - LAST_FRAME ] = refc
  usedFrame[ refc ] = 1
}
for ( i = 0; i < REFS_PER_FRAME - 2; i++ ) {
  refFrame = Ref_Frame_List[ i ]
  if ( ref_frame_idx[ refFrame - LAST_FRAME ] < 0 ) {
    refc = find_latest_forward()
    if ( refc >= 0 ) {
      ref_frame_idx[ refFrame - LAST_FRAME ] = refc
      usedFrame[ refc ] = 1
    }
  }
}
refc = -1
for ( i = 0; i < NUM_REF_FRAMES; i++ ) {
  hint = shiftedOrderHints[ i ]
  if ( refc < 0 || hint < earliestOrderHint ) {
    refc = i
    earliestOrderHint = hint
  }
}
for ( i = 0; i < REFS_PER_FRAME; i++ ) {
  if ( ref_frame_idx[ i ] < 0 ) {
    ref_frame_idx[ i ] = refc
  }
}
}
    */
		private AomArray<int> usedFrame = new AomArray<int>();
		public AomArray<int> _UsedFrame { get { return usedFrame; } set { usedFrame = value; } }
		private int curFrameHint;
		public int _CurFrameHint { get { return curFrameHint; } set { curFrameHint = value; } }
		private AomArray<int> shiftedOrderHints = new AomArray<int>();
		public AomArray<int> _ShiftedOrderHints { get { return shiftedOrderHints; } set { shiftedOrderHints = value; } }

        private void SetFrameRefs()
        {
			int i = 0;
			int lastOrderHint = 0;
			int goldOrderHint = 0;
			int refc = 0;
			int refFrame = 0;
			int hint = 0;
			int earliestOrderHint = 0;
/*  AV1 Bitstream & Decoding Process Specification, 7.8 Set frame refs process: its code, inc order  */


			for (i = 0; (i < REFS_PER_FRAME); i++)
			{
				ref_frame_idx[i] = -1;
			}
			ref_frame_idx[(LAST_FRAME - LAST_FRAME)] = last_frame_idx;
			ref_frame_idx[(GOLDEN_FRAME - LAST_FRAME)] = gold_frame_idx;

			for (i = 0; (i < NUM_REF_FRAMES); i++)
			{
				usedFrame[i] = 0;
			}
			usedFrame[last_frame_idx] = 1;
			usedFrame[gold_frame_idx] = 1;
			curFrameHint = (1 << (OrderHintBits - 1));

			for (i = 0; (i < NUM_REF_FRAMES); i++)
			{
				shiftedOrderHints[i] = (curFrameHint + GetRelativeDist(RefOrderHint[i], OrderHint));
			}
			lastOrderHint = shiftedOrderHints[last_frame_idx];
			goldOrderHint = shiftedOrderHints[gold_frame_idx];
			refc = FindLatestBackward();

			if ((refc >= 0))
			{
				ref_frame_idx[(ALTREF_FRAME - LAST_FRAME)] = refc;
				usedFrame[refc] = 1;
			}
			refc = FindEarliestBackward();

			if ((refc >= 0))
			{
				ref_frame_idx[(BWDREF_FRAME - LAST_FRAME)] = refc;
				usedFrame[refc] = 1;
			}
			refc = FindEarliestBackward();

			if ((refc >= 0))
			{
				ref_frame_idx[(ALTREF2_FRAME - LAST_FRAME)] = refc;
				usedFrame[refc] = 1;
			}

			for (i = 0; (i < (REFS_PER_FRAME - 2)); i++)
			{
				refFrame = Ref_Frame_List[i];

				if ((ref_frame_idx[(refFrame - LAST_FRAME)] < 0))
				{
					refc = FindLatestForward();

					if ((refc >= 0))
					{
						ref_frame_idx[(refFrame - LAST_FRAME)] = refc;
						usedFrame[refc] = 1;
					}
				}
			}
			refc = -1;

			for (i = 0; (i < NUM_REF_FRAMES); i++)
			{
				hint = shiftedOrderHints[i];

				if (((refc < 0) || (hint < earliestOrderHint)))
				{
					refc = i;
					earliestOrderHint = hint;
				}
			}

			for (i = 0; (i < REFS_PER_FRAME); i++)
			{

				if ((ref_frame_idx[i] < 0))
				{
					ref_frame_idx[i] = refc;
				}
			}
        }

    /*
find_latest_backward() {
/* AV1 Bitstream & Decoding Process Specification, 7.8 Set frame refs process *//*
  refc = -1
  for ( i = 0; i < NUM_REF_FRAMES; i++ ) {
    hint = shiftedOrderHints[ i ]
    if ( !usedFrame[ i ] &&
         hint >= curFrameHint &&
         ( refc < 0 || hint >= latestOrderHint ) ) {
      refc = i
      latestOrderHint = hint
    }
  }
  return refc
}
    */

        private int FindLatestBackward()
        {
			int i = 0;
			int refc = 0;
			int hint = 0;
			int latestOrderHint = 0;
/*  AV1 Bitstream & Decoding Process Specification, 7.8 Set frame refs process  */

			refc = -1;

			for (i = 0; (i < NUM_REF_FRAMES); i++)
			{
				hint = shiftedOrderHints[i];

				if (((!(usedFrame[i] != 0) && (hint >= curFrameHint)) && ((refc < 0) || (hint >= latestOrderHint))))
				{
					refc = i;
					latestOrderHint = hint;
				}
			}
			return refc;
        }

    /*
find_earliest_backward() {
/* AV1 Bitstream & Decoding Process Specification, 7.8 Set frame refs process *//*
  refc = -1
  for ( i = 0; i < NUM_REF_FRAMES; i++ ) {
    hint = shiftedOrderHints[ i ]
    if ( !usedFrame[ i ] &&
         hint >= curFrameHint &&
         ( refc < 0 || hint < earliestOrderHint ) ) {
      refc = i
      earliestOrderHint = hint
    }
  }
  return refc
}
    */

        private int FindEarliestBackward()
        {
			int i = 0;
			int refc = 0;
			int hint = 0;
			int earliestOrderHint = 0;
/*  AV1 Bitstream & Decoding Process Specification, 7.8 Set frame refs process  */

			refc = -1;

			for (i = 0; (i < NUM_REF_FRAMES); i++)
			{
				hint = shiftedOrderHints[i];

				if (((!(usedFrame[i] != 0) && (hint >= curFrameHint)) && ((refc < 0) || (hint < earliestOrderHint))))
				{
					refc = i;
					earliestOrderHint = hint;
				}
			}
			return refc;
        }

    /*
find_latest_forward() {
/* AV1 Bitstream & Decoding Process Specification, 7.8 Set frame refs process *//*
  refc = -1
  for ( i = 0; i < NUM_REF_FRAMES; i++ ) {
    hint = shiftedOrderHints[ i ]
    if ( !usedFrame[ i ] &&
         hint < curFrameHint &&
         ( refc < 0 || hint >= latestOrderHint ) ) {
      refc = i
      latestOrderHint = hint
    }
  }
  return refc
}
    */

        private int FindLatestForward()
        {
			int i = 0;
			int refc = 0;
			int hint = 0;
			int latestOrderHint = 0;
/*  AV1 Bitstream & Decoding Process Specification, 7.8 Set frame refs process  */

			refc = -1;

			for (i = 0; (i < NUM_REF_FRAMES); i++)
			{
				hint = shiftedOrderHints[i];

				if (((!(usedFrame[i] != 0) && (hint < curFrameHint)) && ((refc < 0) || (hint >= latestOrderHint))))
				{
					refc = i;
					latestOrderHint = hint;
				}
			}
			return refc;
        }

    /*
mark_ref_frames( idLen ) {
/* AV1 Bitstream & Decoding Process Specification, 5.9.4 Reference frame marking function *//*
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
    */
		private int idLen;
		public int _IdLen { get { return idLen; } set { idLen = value; } }

        private void MarkRefFrames(int idLen)
        {
			int i = 0;
			int diffLen = 0;
/*  AV1 Bitstream & Decoding Process Specification, 5.9.4 Reference frame marking function  */

			diffLen = (delta_frame_id_length_minus_2 + 2);

			for (i = 0; (i < NUM_REF_FRAMES); i++)
			{

				if ((current_frame_id > (1 << diffLen)))
				{

					if (((RefFrameId[i] > current_frame_id) || (RefFrameId[i] < (current_frame_id - (1 << diffLen)))))
					{
						RefValid[i] = 0;
					}
				}
				else 
				{

					if (((RefFrameId[i] > current_frame_id) && (RefFrameId[i] < (((1 << idLen) + current_frame_id) - (1 << diffLen)))))
					{
						RefValid[i] = 0;
					}
				}
			}
        }

		/// <summary>What the context holds, as SaveContext keeps it.</summary>
		private sealed partial class ContextState
		{
			public int AllLossless;
			public int BitDepth;
			public int CdefDamping;
			public int CodedLossless;
			public int CurrentQIndex;
			public int DeltaFrameId;
			public int DeltaQUAc;
			public int DeltaQUDc;
			public int DeltaQVAc;
			public int DeltaQVDc;
			public int DeltaQYDc;
			public AomArray<AomArray<int>> FeatureData;
			public AomArray<AomArray<int>> FeatureEnabled;
			public int FrameHeight;
			public int FrameIsIntra;
			public AomArray<int> FrameRestorationType;
			public int FrameWidth;
			public AomArray<int> GmType;
			public int LastActiveSegId;
			public AomArray<int> LoopRestorationSize;
			public AomArray<int> LosslessArray;
			public int MiColEnd;
			public int MiColStart;
			public AomArray<int> MiColStarts;
			public int MiCols;
			public int MiRowEnd;
			public int MiRowStart;
			public AomArray<int> MiRowStarts;
			public int MiRows;
			public int N;
			public int NumPlanes;
			public int NumTiles;
			public int OperatingPointIdc;
			public int OrderHint;
			public int OrderHintBits;
			public AomArray<int> OrderHints;
			public int PrevFrameID;
			public AomArray<int> RefFrameSignBias;
			public AomArray<int> RefOrderHint;
			public AomArray<int> RefValid;
			public int RenderHeight;
			public int RenderWidth;
			public int SeenFrameHeader;
			public int SegIdPreSkip;
			public AomArray<AomArray<int>> SegQMLevel;
			public AomArray<int> SkipModeFrame;
			public int SuperresDenom;
			public int TileCols;
			public int TileColsLog2;
			public int TileNum;
			public int TileRows;
			public int TileRowsLog2;
			public int TileSizeBytes;
			public int TxMode;
			public int UpscaledWidth;
			public int UsesLr;
			public int a;
			public int additional_frame_id_length_minus_1;
			public int allow_high_precision_mv;
			public int allow_intrabc;
			public int allow_screen_content_tools;
			public int allow_warped_motion;
			public int anchor_frame_idx;
			public int anchor_tile_col;
			public int anchor_tile_row;
			public int apply_grain;
			public int ar_coeff_lag;
			public int ar_coeff_shift_minus_6;
			public AomArray<int> ar_coeffs_cb_plus_128;
			public AomArray<int> ar_coeffs_cr_plus_128;
			public AomArray<int> ar_coeffs_y_plus_128;
			public int b;
			public int base_q_idx;
			public int blkSize;
			public int buffer_delay_length_minus_1;
			public AomArray<int> buffer_removal_time;
			public int buffer_removal_time_length_minus_1;
			public int buffer_removal_time_present_flag;
			public int cb_luma_mult;
			public int cb_mult;
			public int cb_offset;
			public int cdef_bits;
			public int cdef_damping_minus_3;
			public AomArray<int> cdef_uv_pri_strength;
			public AomArray<int> cdef_uv_sec_strength;
			public AomArray<int> cdef_y_pri_strength;
			public AomArray<int> cdef_y_sec_strength;
			public int chroma_sample_position;
			public int chroma_scaling_from_luma;
			public int clip_to_restricted_range;
			public int cnt_dropped_flag;
			public int coded_denom;
			public byte[] coded_tile_data;
			public int color_description_present_flag;
			public int color_primaries;
			public int color_range;
			public int context_update_tile_id;
			public int counting_type;
			public int cr_luma_mult;
			public int cr_mult;
			public int cr_offset;
			public int curFrameHint;
			public int current_frame_id;
			public AomArray<int> decoder_buffer_delay;
			public int decoder_model_info_present_flag;
			public AomArray<int> decoder_model_present_for_this_op;
			public int delta_coded;
			public int delta_frame_id_length_minus_2;
			public int delta_frame_id_minus_1;
			public int delta_lf_multi;
			public int delta_lf_present;
			public int delta_lf_res;
			public int delta_q;
			public int delta_q_present;
			public int delta_q_res;
			public int diff_uv_delta;
			public int disable_cdf_update;
			public int disable_frame_end_update_cdf;
			public int discontinuity_flag;
			public int display_frame_id;
			public int enable_cdef;
			public int enable_dual_filter;
			public int enable_filter_intra;
			public int enable_interintra_compound;
			public int enable_intra_edge_filter;
			public int enable_jnt_comp;
			public int enable_masked_compound;
			public int enable_order_hint;
			public int enable_ref_frame_mvs;
			public int enable_restoration;
			public int enable_superres;
			public int enable_warped_motion;
			public AomArray<int> encoder_buffer_delay;
			public int equal_picture_interval;
			public int error_resilient_mode;
			public int extension_header_reserved_3bits;
			public int feature_enabled;
			public int feature_value;
			public int film_grain_params_present;
			public int film_grain_params_ref_idx;
			public int force_integer_mv;
			public int found_ref;
			public int frame_height_bits_minus_1;
			public int frame_height_minus_1;
			public int frame_id_numbers_present_flag;
			public int frame_presentation_time;
			public int frame_presentation_time_length_minus_1;
			public int frame_refs_short_signaling;
			public int frame_size_override_flag;
			public int frame_to_show_map_idx;
			public int frame_type;
			public int frame_width_bits_minus_1;
			public int frame_width_minus_1;
			public int full_timestamp_flag;
			public AomArray<AomArray<int>> gm_params;
			public int gold_frame_idx;
			public int grain_scale_shift;
			public int grain_scaling_minus_8;
			public int grain_seed;
			public int height_in_sbs_minus_1;
			public int high;
			public int high_bitdepth;
			public int hours_flag;
			public int hours_value;
			public int idLen;
			public int idx;
			public int increment_tile_cols_log2;
			public int increment_tile_rows_log2;
			public AomArray<int> initial_display_delay_minus_1;
			public int initial_display_delay_present_flag;
			public AomArray<int> initial_display_delay_present_for_this_op;
			public int interpolation_filter;
			public int is_filter_switchable;
			public int is_global;
			public int is_motion_mode_switchable;
			public int is_rot_zoom;
			public int is_translation;
			public int itu_t_t35_country_code;
			public int itu_t_t35_country_code_extension_byte;
			public int last_frame_idx;
			public int loop_filter_delta_enabled;
			public int loop_filter_delta_update;
			public AomArray<int> loop_filter_level;
			public AomArray<int> loop_filter_mode_deltas;
			public AomArray<int> loop_filter_ref_deltas;
			public int loop_filter_sharpness;
			public int low;
			public AomArray<int> low_delay_mode_flag;
			public int lr_type;
			public int lr_unit_extra_shift;
			public int lr_unit_shift;
			public int lr_uv_shift;
			public int luminance_max;
			public int luminance_min;
			public int matrix_coefficients;
			public int max_cll;
			public int max_fall;
			public int max_frame_height_minus_1;
			public int max_frame_width_minus_1;
			public int metadata_type;
			public int minutes_flag;
			public int minutes_value;
			public int mono_chrome;
			public int mx;
			public int n_frames;
			public long nbBits;
			public int numSyms;
			public int num_cb_points;
			public int num_cr_points;
			public uint num_ticks_per_picture_minus_1;
			public int num_units_in_decoding_tick;
			public int num_units_in_display_tick;
			public int num_y_points;
			public int obu_extension_flag;
			public int obu_forbidden_bit;
			public int obu_has_size_field;
			public int obu_padding_byte;
			public int obu_padding_length;
			public int obu_reserved_1bit;
			public int obu_size;
			public int obu_type;
			public int op;
			public AomArray<int> operating_point_idc;
			public int operating_points_cnt_minus_1;
			public int order_hint;
			public int order_hint_bits_minus_1;
			public int output_frame_height_in_tiles_minus_1;
			public int output_frame_width_in_tiles_minus_1;
			public int overlap_flag;
			public AomArray<int> point_cb_scaling;
			public AomArray<int> point_cb_value;
			public AomArray<int> point_cr_scaling;
			public AomArray<int> point_cr_value;
			public AomArray<int> point_y_scaling;
			public AomArray<int> point_y_value;
			public AomArray<int> primary_chromaticity_x;
			public AomArray<int> primary_chromaticity_y;
			public int primary_ref_frame;
			public int qm_u;
			public int qm_v;
			public int qm_y;
			public int r;
			public int reduced_still_picture_header;
			public int reduced_tx_set;
			public AomArray<int> ref_frame_idx;
			public AomArray<int> ref_order_hint;
			public int reference_select;
			public int refresh_frame_flags;
			public int render_and_frame_size_different;
			public int render_height_minus_1;
			public int render_width_minus_1;
			public int scalability_mode_idc;
			public int scalability_structure_reserved_3bits;
			public int seconds_flag;
			public int seconds_value;
			public int segmentation_enabled;
			public int segmentation_temporal_update;
			public int segmentation_update_data;
			public int segmentation_update_map;
			public int separate_uv_delta_q;
			public int seq_choose_integer_mv;
			public int seq_choose_screen_content_tools;
			public int seq_force_integer_mv;
			public int seq_force_screen_content_tools;
			public AomArray<int> seq_level_idx;
			public int seq_profile;
			public AomArray<int> seq_tier;
			public AomArray<int> shiftedOrderHints;
			public int show_existing_frame;
			public int show_frame;
			public int showable_frame;
			public int skip_mode_present;
			public int spatial_id;
			public int spatial_layer_description_present_flag;
			public int spatial_layer_dimensions_present_flag;
			public AomArray<int> spatial_layer_max_height;
			public AomArray<int> spatial_layer_max_width;
			public AomArray<int> spatial_layer_ref_id;
			public int spatial_layers_cnt_minus_1;
			public int startPosition;
			public int still_picture;
			public int subexp_bits;
			public int subexp_final_bits;
			public int subexp_more_bits;
			public int subsampling_x;
			public int subsampling_y;
			public int sz;
			public int target;
			public int temporal_group_description_present_flag;
			public AomArray<int> temporal_group_ref_cnt;
			public AomArray<AomArray<int>> temporal_group_ref_pic_diff;
			public int temporal_group_size;
			public AomArray<int> temporal_group_spatial_switching_up_point_flag;
			public AomArray<int> temporal_group_temporal_id;
			public AomArray<int> temporal_group_temporal_switching_up_point_flag;
			public int temporal_id;
			public int tg_end;
			public int tg_start;
			public int tile_count_minus_1;
			public int tile_data_size_minus_1;
			public int tile_size_bytes_minus_1;
			public int tile_size_minus_1;
			public int tile_start_and_end_present_flag;
			public int time_offset_length;
			public int time_offset_value;
			public int time_scale;
			public int timing_info_present_flag;
			public int trailing_one_bit;
			public int trailing_zero_bit;
			public int transfer_characteristics;
			public int twelve_bit;
			public int tx_mode_select;
			public int type;
			public int uniform_tile_spacing_flag;
			public int update_grain;
			public int update_mode_delta;
			public int update_ref_delta;
			public int use_128x128_superblock;
			public int use_ref_frame_mvs;
			public int use_superres;
			public AomArray<int> usedFrame;
			public int using_qmatrix;
			public int v;
			public int white_point_chromaticity_x;
			public int white_point_chromaticity_y;
			public int width_in_sbs_minus_1;
			public int zero_bit;
		}

		private ContextState SaveContext()
		{
			var state = new ContextState();
			state.AllLossless = this.AllLossless;
			state.BitDepth = this.BitDepth;
			state.CdefDamping = this.CdefDamping;
			state.CodedLossless = this.CodedLossless;
			state.CurrentQIndex = this.CurrentQIndex;
			state.DeltaFrameId = this.DeltaFrameId;
			state.DeltaQUAc = this.DeltaQUAc;
			state.DeltaQUDc = this.DeltaQUDc;
			state.DeltaQVAc = this.DeltaQVAc;
			state.DeltaQVDc = this.DeltaQVDc;
			state.DeltaQYDc = this.DeltaQYDc;
			state.FeatureData = this.FeatureData?.Clone();
			state.FeatureEnabled = this.FeatureEnabled?.Clone();
			state.FrameHeight = this.FrameHeight;
			state.FrameIsIntra = this.FrameIsIntra;
			state.FrameRestorationType = this.FrameRestorationType?.Clone();
			state.FrameWidth = this.FrameWidth;
			state.GmType = this.GmType?.Clone();
			state.LastActiveSegId = this.LastActiveSegId;
			state.LoopRestorationSize = this.LoopRestorationSize?.Clone();
			state.LosslessArray = this.LosslessArray?.Clone();
			state.MiColEnd = this.MiColEnd;
			state.MiColStart = this.MiColStart;
			state.MiColStarts = this.MiColStarts?.Clone();
			state.MiCols = this.MiCols;
			state.MiRowEnd = this.MiRowEnd;
			state.MiRowStart = this.MiRowStart;
			state.MiRowStarts = this.MiRowStarts?.Clone();
			state.MiRows = this.MiRows;
			state.N = this.N;
			state.NumPlanes = this.NumPlanes;
			state.NumTiles = this.NumTiles;
			state.OperatingPointIdc = this.OperatingPointIdc;
			state.OrderHint = this.OrderHint;
			state.OrderHintBits = this.OrderHintBits;
			state.OrderHints = this.OrderHints?.Clone();
			state.PrevFrameID = this.PrevFrameID;
			state.RefFrameSignBias = this.RefFrameSignBias?.Clone();
			state.RefOrderHint = this.RefOrderHint?.Clone();
			state.RefValid = this.RefValid?.Clone();
			state.RenderHeight = this.RenderHeight;
			state.RenderWidth = this.RenderWidth;
			state.SeenFrameHeader = this.SeenFrameHeader;
			state.SegIdPreSkip = this.SegIdPreSkip;
			state.SegQMLevel = this.SegQMLevel?.Clone();
			state.SkipModeFrame = this.SkipModeFrame?.Clone();
			state.SuperresDenom = this.SuperresDenom;
			state.TileCols = this.TileCols;
			state.TileColsLog2 = this.TileColsLog2;
			state.TileNum = this.TileNum;
			state.TileRows = this.TileRows;
			state.TileRowsLog2 = this.TileRowsLog2;
			state.TileSizeBytes = this.TileSizeBytes;
			state.TxMode = this.TxMode;
			state.UpscaledWidth = this.UpscaledWidth;
			state.UsesLr = this.UsesLr;
			state.a = this.a;
			state.additional_frame_id_length_minus_1 = this.additional_frame_id_length_minus_1;
			state.allow_high_precision_mv = this.allow_high_precision_mv;
			state.allow_intrabc = this.allow_intrabc;
			state.allow_screen_content_tools = this.allow_screen_content_tools;
			state.allow_warped_motion = this.allow_warped_motion;
			state.anchor_frame_idx = this.anchor_frame_idx;
			state.anchor_tile_col = this.anchor_tile_col;
			state.anchor_tile_row = this.anchor_tile_row;
			state.apply_grain = this.apply_grain;
			state.ar_coeff_lag = this.ar_coeff_lag;
			state.ar_coeff_shift_minus_6 = this.ar_coeff_shift_minus_6;
			state.ar_coeffs_cb_plus_128 = this.ar_coeffs_cb_plus_128?.Clone();
			state.ar_coeffs_cr_plus_128 = this.ar_coeffs_cr_plus_128?.Clone();
			state.ar_coeffs_y_plus_128 = this.ar_coeffs_y_plus_128?.Clone();
			state.b = this.b;
			state.base_q_idx = this.base_q_idx;
			state.blkSize = this.blkSize;
			state.buffer_delay_length_minus_1 = this.buffer_delay_length_minus_1;
			state.buffer_removal_time = this.buffer_removal_time?.Clone();
			state.buffer_removal_time_length_minus_1 = this.buffer_removal_time_length_minus_1;
			state.buffer_removal_time_present_flag = this.buffer_removal_time_present_flag;
			state.cb_luma_mult = this.cb_luma_mult;
			state.cb_mult = this.cb_mult;
			state.cb_offset = this.cb_offset;
			state.cdef_bits = this.cdef_bits;
			state.cdef_damping_minus_3 = this.cdef_damping_minus_3;
			state.cdef_uv_pri_strength = this.cdef_uv_pri_strength?.Clone();
			state.cdef_uv_sec_strength = this.cdef_uv_sec_strength?.Clone();
			state.cdef_y_pri_strength = this.cdef_y_pri_strength?.Clone();
			state.cdef_y_sec_strength = this.cdef_y_sec_strength?.Clone();
			state.chroma_sample_position = this.chroma_sample_position;
			state.chroma_scaling_from_luma = this.chroma_scaling_from_luma;
			state.clip_to_restricted_range = this.clip_to_restricted_range;
			state.cnt_dropped_flag = this.cnt_dropped_flag;
			state.coded_denom = this.coded_denom;
			state.coded_tile_data = ((byte[])this.coded_tile_data?.Clone());
			state.color_description_present_flag = this.color_description_present_flag;
			state.color_primaries = this.color_primaries;
			state.color_range = this.color_range;
			state.context_update_tile_id = this.context_update_tile_id;
			state.counting_type = this.counting_type;
			state.cr_luma_mult = this.cr_luma_mult;
			state.cr_mult = this.cr_mult;
			state.cr_offset = this.cr_offset;
			state.curFrameHint = this.curFrameHint;
			state.current_frame_id = this.current_frame_id;
			state.decoder_buffer_delay = this.decoder_buffer_delay?.Clone();
			state.decoder_model_info_present_flag = this.decoder_model_info_present_flag;
			state.decoder_model_present_for_this_op = this.decoder_model_present_for_this_op?.Clone();
			state.delta_coded = this.delta_coded;
			state.delta_frame_id_length_minus_2 = this.delta_frame_id_length_minus_2;
			state.delta_frame_id_minus_1 = this.delta_frame_id_minus_1;
			state.delta_lf_multi = this.delta_lf_multi;
			state.delta_lf_present = this.delta_lf_present;
			state.delta_lf_res = this.delta_lf_res;
			state.delta_q = this.delta_q;
			state.delta_q_present = this.delta_q_present;
			state.delta_q_res = this.delta_q_res;
			state.diff_uv_delta = this.diff_uv_delta;
			state.disable_cdf_update = this.disable_cdf_update;
			state.disable_frame_end_update_cdf = this.disable_frame_end_update_cdf;
			state.discontinuity_flag = this.discontinuity_flag;
			state.display_frame_id = this.display_frame_id;
			state.enable_cdef = this.enable_cdef;
			state.enable_dual_filter = this.enable_dual_filter;
			state.enable_filter_intra = this.enable_filter_intra;
			state.enable_interintra_compound = this.enable_interintra_compound;
			state.enable_intra_edge_filter = this.enable_intra_edge_filter;
			state.enable_jnt_comp = this.enable_jnt_comp;
			state.enable_masked_compound = this.enable_masked_compound;
			state.enable_order_hint = this.enable_order_hint;
			state.enable_ref_frame_mvs = this.enable_ref_frame_mvs;
			state.enable_restoration = this.enable_restoration;
			state.enable_superres = this.enable_superres;
			state.enable_warped_motion = this.enable_warped_motion;
			state.encoder_buffer_delay = this.encoder_buffer_delay?.Clone();
			state.equal_picture_interval = this.equal_picture_interval;
			state.error_resilient_mode = this.error_resilient_mode;
			state.extension_header_reserved_3bits = this.extension_header_reserved_3bits;
			state.feature_enabled = this.feature_enabled;
			state.feature_value = this.feature_value;
			state.film_grain_params_present = this.film_grain_params_present;
			state.film_grain_params_ref_idx = this.film_grain_params_ref_idx;
			state.force_integer_mv = this.force_integer_mv;
			state.found_ref = this.found_ref;
			state.frame_height_bits_minus_1 = this.frame_height_bits_minus_1;
			state.frame_height_minus_1 = this.frame_height_minus_1;
			state.frame_id_numbers_present_flag = this.frame_id_numbers_present_flag;
			state.frame_presentation_time = this.frame_presentation_time;
			state.frame_presentation_time_length_minus_1 = this.frame_presentation_time_length_minus_1;
			state.frame_refs_short_signaling = this.frame_refs_short_signaling;
			state.frame_size_override_flag = this.frame_size_override_flag;
			state.frame_to_show_map_idx = this.frame_to_show_map_idx;
			state.frame_type = this.frame_type;
			state.frame_width_bits_minus_1 = this.frame_width_bits_minus_1;
			state.frame_width_minus_1 = this.frame_width_minus_1;
			state.full_timestamp_flag = this.full_timestamp_flag;
			state.gm_params = this.gm_params?.Clone();
			state.gold_frame_idx = this.gold_frame_idx;
			state.grain_scale_shift = this.grain_scale_shift;
			state.grain_scaling_minus_8 = this.grain_scaling_minus_8;
			state.grain_seed = this.grain_seed;
			state.height_in_sbs_minus_1 = this.height_in_sbs_minus_1;
			state.high = this.high;
			state.high_bitdepth = this.high_bitdepth;
			state.hours_flag = this.hours_flag;
			state.hours_value = this.hours_value;
			state.idLen = this.idLen;
			state.idx = this.idx;
			state.increment_tile_cols_log2 = this.increment_tile_cols_log2;
			state.increment_tile_rows_log2 = this.increment_tile_rows_log2;
			state.initial_display_delay_minus_1 = this.initial_display_delay_minus_1?.Clone();
			state.initial_display_delay_present_flag = this.initial_display_delay_present_flag;
			state.initial_display_delay_present_for_this_op = this.initial_display_delay_present_for_this_op?.Clone();
			state.interpolation_filter = this.interpolation_filter;
			state.is_filter_switchable = this.is_filter_switchable;
			state.is_global = this.is_global;
			state.is_motion_mode_switchable = this.is_motion_mode_switchable;
			state.is_rot_zoom = this.is_rot_zoom;
			state.is_translation = this.is_translation;
			state.itu_t_t35_country_code = this.itu_t_t35_country_code;
			state.itu_t_t35_country_code_extension_byte = this.itu_t_t35_country_code_extension_byte;
			state.last_frame_idx = this.last_frame_idx;
			state.loop_filter_delta_enabled = this.loop_filter_delta_enabled;
			state.loop_filter_delta_update = this.loop_filter_delta_update;
			state.loop_filter_level = this.loop_filter_level?.Clone();
			state.loop_filter_mode_deltas = this.loop_filter_mode_deltas?.Clone();
			state.loop_filter_ref_deltas = this.loop_filter_ref_deltas?.Clone();
			state.loop_filter_sharpness = this.loop_filter_sharpness;
			state.low = this.low;
			state.low_delay_mode_flag = this.low_delay_mode_flag?.Clone();
			state.lr_type = this.lr_type;
			state.lr_unit_extra_shift = this.lr_unit_extra_shift;
			state.lr_unit_shift = this.lr_unit_shift;
			state.lr_uv_shift = this.lr_uv_shift;
			state.luminance_max = this.luminance_max;
			state.luminance_min = this.luminance_min;
			state.matrix_coefficients = this.matrix_coefficients;
			state.max_cll = this.max_cll;
			state.max_fall = this.max_fall;
			state.max_frame_height_minus_1 = this.max_frame_height_minus_1;
			state.max_frame_width_minus_1 = this.max_frame_width_minus_1;
			state.metadata_type = this.metadata_type;
			state.minutes_flag = this.minutes_flag;
			state.minutes_value = this.minutes_value;
			state.mono_chrome = this.mono_chrome;
			state.mx = this.mx;
			state.n_frames = this.n_frames;
			state.nbBits = this.nbBits;
			state.numSyms = this.numSyms;
			state.num_cb_points = this.num_cb_points;
			state.num_cr_points = this.num_cr_points;
			state.num_ticks_per_picture_minus_1 = this.num_ticks_per_picture_minus_1;
			state.num_units_in_decoding_tick = this.num_units_in_decoding_tick;
			state.num_units_in_display_tick = this.num_units_in_display_tick;
			state.num_y_points = this.num_y_points;
			state.obu_extension_flag = this.obu_extension_flag;
			state.obu_forbidden_bit = this.obu_forbidden_bit;
			state.obu_has_size_field = this.obu_has_size_field;
			state.obu_padding_byte = this.obu_padding_byte;
			state.obu_padding_length = this.obu_padding_length;
			state.obu_reserved_1bit = this.obu_reserved_1bit;
			state.obu_size = this.obu_size;
			state.obu_type = this.obu_type;
			state.op = this.op;
			state.operating_point_idc = this.operating_point_idc?.Clone();
			state.operating_points_cnt_minus_1 = this.operating_points_cnt_minus_1;
			state.order_hint = this.order_hint;
			state.order_hint_bits_minus_1 = this.order_hint_bits_minus_1;
			state.output_frame_height_in_tiles_minus_1 = this.output_frame_height_in_tiles_minus_1;
			state.output_frame_width_in_tiles_minus_1 = this.output_frame_width_in_tiles_minus_1;
			state.overlap_flag = this.overlap_flag;
			state.point_cb_scaling = this.point_cb_scaling?.Clone();
			state.point_cb_value = this.point_cb_value?.Clone();
			state.point_cr_scaling = this.point_cr_scaling?.Clone();
			state.point_cr_value = this.point_cr_value?.Clone();
			state.point_y_scaling = this.point_y_scaling?.Clone();
			state.point_y_value = this.point_y_value?.Clone();
			state.primary_chromaticity_x = this.primary_chromaticity_x?.Clone();
			state.primary_chromaticity_y = this.primary_chromaticity_y?.Clone();
			state.primary_ref_frame = this.primary_ref_frame;
			state.qm_u = this.qm_u;
			state.qm_v = this.qm_v;
			state.qm_y = this.qm_y;
			state.r = this.r;
			state.reduced_still_picture_header = this.reduced_still_picture_header;
			state.reduced_tx_set = this.reduced_tx_set;
			state.ref_frame_idx = this.ref_frame_idx?.Clone();
			state.ref_order_hint = this.ref_order_hint?.Clone();
			state.reference_select = this.reference_select;
			state.refresh_frame_flags = this.refresh_frame_flags;
			state.render_and_frame_size_different = this.render_and_frame_size_different;
			state.render_height_minus_1 = this.render_height_minus_1;
			state.render_width_minus_1 = this.render_width_minus_1;
			state.scalability_mode_idc = this.scalability_mode_idc;
			state.scalability_structure_reserved_3bits = this.scalability_structure_reserved_3bits;
			state.seconds_flag = this.seconds_flag;
			state.seconds_value = this.seconds_value;
			state.segmentation_enabled = this.segmentation_enabled;
			state.segmentation_temporal_update = this.segmentation_temporal_update;
			state.segmentation_update_data = this.segmentation_update_data;
			state.segmentation_update_map = this.segmentation_update_map;
			state.separate_uv_delta_q = this.separate_uv_delta_q;
			state.seq_choose_integer_mv = this.seq_choose_integer_mv;
			state.seq_choose_screen_content_tools = this.seq_choose_screen_content_tools;
			state.seq_force_integer_mv = this.seq_force_integer_mv;
			state.seq_force_screen_content_tools = this.seq_force_screen_content_tools;
			state.seq_level_idx = this.seq_level_idx?.Clone();
			state.seq_profile = this.seq_profile;
			state.seq_tier = this.seq_tier?.Clone();
			state.shiftedOrderHints = this.shiftedOrderHints?.Clone();
			state.show_existing_frame = this.show_existing_frame;
			state.show_frame = this.show_frame;
			state.showable_frame = this.showable_frame;
			state.skip_mode_present = this.skip_mode_present;
			state.spatial_id = this.spatial_id;
			state.spatial_layer_description_present_flag = this.spatial_layer_description_present_flag;
			state.spatial_layer_dimensions_present_flag = this.spatial_layer_dimensions_present_flag;
			state.spatial_layer_max_height = this.spatial_layer_max_height?.Clone();
			state.spatial_layer_max_width = this.spatial_layer_max_width?.Clone();
			state.spatial_layer_ref_id = this.spatial_layer_ref_id?.Clone();
			state.spatial_layers_cnt_minus_1 = this.spatial_layers_cnt_minus_1;
			state.startPosition = this.startPosition;
			state.still_picture = this.still_picture;
			state.subexp_bits = this.subexp_bits;
			state.subexp_final_bits = this.subexp_final_bits;
			state.subexp_more_bits = this.subexp_more_bits;
			state.subsampling_x = this.subsampling_x;
			state.subsampling_y = this.subsampling_y;
			state.sz = this.sz;
			state.target = this.target;
			state.temporal_group_description_present_flag = this.temporal_group_description_present_flag;
			state.temporal_group_ref_cnt = this.temporal_group_ref_cnt?.Clone();
			state.temporal_group_ref_pic_diff = this.temporal_group_ref_pic_diff?.Clone();
			state.temporal_group_size = this.temporal_group_size;
			state.temporal_group_spatial_switching_up_point_flag = this.temporal_group_spatial_switching_up_point_flag?.Clone();
			state.temporal_group_temporal_id = this.temporal_group_temporal_id?.Clone();
			state.temporal_group_temporal_switching_up_point_flag = this.temporal_group_temporal_switching_up_point_flag?.Clone();
			state.temporal_id = this.temporal_id;
			state.tg_end = this.tg_end;
			state.tg_start = this.tg_start;
			state.tile_count_minus_1 = this.tile_count_minus_1;
			state.tile_data_size_minus_1 = this.tile_data_size_minus_1;
			state.tile_size_bytes_minus_1 = this.tile_size_bytes_minus_1;
			state.tile_size_minus_1 = this.tile_size_minus_1;
			state.tile_start_and_end_present_flag = this.tile_start_and_end_present_flag;
			state.time_offset_length = this.time_offset_length;
			state.time_offset_value = this.time_offset_value;
			state.time_scale = this.time_scale;
			state.timing_info_present_flag = this.timing_info_present_flag;
			state.trailing_one_bit = this.trailing_one_bit;
			state.trailing_zero_bit = this.trailing_zero_bit;
			state.transfer_characteristics = this.transfer_characteristics;
			state.twelve_bit = this.twelve_bit;
			state.tx_mode_select = this.tx_mode_select;
			state.type = this.type;
			state.uniform_tile_spacing_flag = this.uniform_tile_spacing_flag;
			state.update_grain = this.update_grain;
			state.update_mode_delta = this.update_mode_delta;
			state.update_ref_delta = this.update_ref_delta;
			state.use_128x128_superblock = this.use_128x128_superblock;
			state.use_ref_frame_mvs = this.use_ref_frame_mvs;
			state.use_superres = this.use_superres;
			state.usedFrame = this.usedFrame?.Clone();
			state.using_qmatrix = this.using_qmatrix;
			state.v = this.v;
			state.white_point_chromaticity_x = this.white_point_chromaticity_x;
			state.white_point_chromaticity_y = this.white_point_chromaticity_y;
			state.width_in_sbs_minus_1 = this.width_in_sbs_minus_1;
			state.zero_bit = this.zero_bit;
			SaveContextExtra(state);
			return state;
		}

		private void LoadContext(ContextState state, bool copy = true)
		{
			this.AllLossless = state.AllLossless;
			this.BitDepth = state.BitDepth;
			this.CdefDamping = state.CdefDamping;
			this.CodedLossless = state.CodedLossless;
			this.CurrentQIndex = state.CurrentQIndex;
			this.DeltaFrameId = state.DeltaFrameId;
			this.DeltaQUAc = state.DeltaQUAc;
			this.DeltaQUDc = state.DeltaQUDc;
			this.DeltaQVAc = state.DeltaQVAc;
			this.DeltaQVDc = state.DeltaQVDc;
			this.DeltaQYDc = state.DeltaQYDc;
			this.FeatureData = copy ? state.FeatureData?.Clone() : state.FeatureData;
			this.FeatureEnabled = copy ? state.FeatureEnabled?.Clone() : state.FeatureEnabled;
			this.FrameHeight = state.FrameHeight;
			this.FrameIsIntra = state.FrameIsIntra;
			this.FrameRestorationType = copy ? state.FrameRestorationType?.Clone() : state.FrameRestorationType;
			this.FrameWidth = state.FrameWidth;
			this.GmType = copy ? state.GmType?.Clone() : state.GmType;
			this.LastActiveSegId = state.LastActiveSegId;
			this.LoopRestorationSize = copy ? state.LoopRestorationSize?.Clone() : state.LoopRestorationSize;
			this.LosslessArray = copy ? state.LosslessArray?.Clone() : state.LosslessArray;
			this.MiColEnd = state.MiColEnd;
			this.MiColStart = state.MiColStart;
			this.MiColStarts = copy ? state.MiColStarts?.Clone() : state.MiColStarts;
			this.MiCols = state.MiCols;
			this.MiRowEnd = state.MiRowEnd;
			this.MiRowStart = state.MiRowStart;
			this.MiRowStarts = copy ? state.MiRowStarts?.Clone() : state.MiRowStarts;
			this.MiRows = state.MiRows;
			this.N = state.N;
			this.NumPlanes = state.NumPlanes;
			this.NumTiles = state.NumTiles;
			this.OperatingPointIdc = state.OperatingPointIdc;
			this.OrderHint = state.OrderHint;
			this.OrderHintBits = state.OrderHintBits;
			this.OrderHints = copy ? state.OrderHints?.Clone() : state.OrderHints;
			this.PrevFrameID = state.PrevFrameID;
			this.RefFrameSignBias = copy ? state.RefFrameSignBias?.Clone() : state.RefFrameSignBias;
			this.RefOrderHint = copy ? state.RefOrderHint?.Clone() : state.RefOrderHint;
			this.RefValid = copy ? state.RefValid?.Clone() : state.RefValid;
			this.RenderHeight = state.RenderHeight;
			this.RenderWidth = state.RenderWidth;
			this.SeenFrameHeader = state.SeenFrameHeader;
			this.SegIdPreSkip = state.SegIdPreSkip;
			this.SegQMLevel = copy ? state.SegQMLevel?.Clone() : state.SegQMLevel;
			this.SkipModeFrame = copy ? state.SkipModeFrame?.Clone() : state.SkipModeFrame;
			this.SuperresDenom = state.SuperresDenom;
			this.TileCols = state.TileCols;
			this.TileColsLog2 = state.TileColsLog2;
			this.TileNum = state.TileNum;
			this.TileRows = state.TileRows;
			this.TileRowsLog2 = state.TileRowsLog2;
			this.TileSizeBytes = state.TileSizeBytes;
			this.TxMode = state.TxMode;
			this.UpscaledWidth = state.UpscaledWidth;
			this.UsesLr = state.UsesLr;
			this.a = state.a;
			this.additional_frame_id_length_minus_1 = state.additional_frame_id_length_minus_1;
			this.allow_high_precision_mv = state.allow_high_precision_mv;
			this.allow_intrabc = state.allow_intrabc;
			this.allow_screen_content_tools = state.allow_screen_content_tools;
			this.allow_warped_motion = state.allow_warped_motion;
			this.anchor_frame_idx = state.anchor_frame_idx;
			this.anchor_tile_col = state.anchor_tile_col;
			this.anchor_tile_row = state.anchor_tile_row;
			this.apply_grain = state.apply_grain;
			this.ar_coeff_lag = state.ar_coeff_lag;
			this.ar_coeff_shift_minus_6 = state.ar_coeff_shift_minus_6;
			this.ar_coeffs_cb_plus_128 = copy ? state.ar_coeffs_cb_plus_128?.Clone() : state.ar_coeffs_cb_plus_128;
			this.ar_coeffs_cr_plus_128 = copy ? state.ar_coeffs_cr_plus_128?.Clone() : state.ar_coeffs_cr_plus_128;
			this.ar_coeffs_y_plus_128 = copy ? state.ar_coeffs_y_plus_128?.Clone() : state.ar_coeffs_y_plus_128;
			this.b = state.b;
			this.base_q_idx = state.base_q_idx;
			this.blkSize = state.blkSize;
			this.buffer_delay_length_minus_1 = state.buffer_delay_length_minus_1;
			this.buffer_removal_time = copy ? state.buffer_removal_time?.Clone() : state.buffer_removal_time;
			this.buffer_removal_time_length_minus_1 = state.buffer_removal_time_length_minus_1;
			this.buffer_removal_time_present_flag = state.buffer_removal_time_present_flag;
			this.cb_luma_mult = state.cb_luma_mult;
			this.cb_mult = state.cb_mult;
			this.cb_offset = state.cb_offset;
			this.cdef_bits = state.cdef_bits;
			this.cdef_damping_minus_3 = state.cdef_damping_minus_3;
			this.cdef_uv_pri_strength = copy ? state.cdef_uv_pri_strength?.Clone() : state.cdef_uv_pri_strength;
			this.cdef_uv_sec_strength = copy ? state.cdef_uv_sec_strength?.Clone() : state.cdef_uv_sec_strength;
			this.cdef_y_pri_strength = copy ? state.cdef_y_pri_strength?.Clone() : state.cdef_y_pri_strength;
			this.cdef_y_sec_strength = copy ? state.cdef_y_sec_strength?.Clone() : state.cdef_y_sec_strength;
			this.chroma_sample_position = state.chroma_sample_position;
			this.chroma_scaling_from_luma = state.chroma_scaling_from_luma;
			this.clip_to_restricted_range = state.clip_to_restricted_range;
			this.cnt_dropped_flag = state.cnt_dropped_flag;
			this.coded_denom = state.coded_denom;
			this.coded_tile_data = copy ? ((byte[])state.coded_tile_data?.Clone()) : state.coded_tile_data;
			this.color_description_present_flag = state.color_description_present_flag;
			this.color_primaries = state.color_primaries;
			this.color_range = state.color_range;
			this.context_update_tile_id = state.context_update_tile_id;
			this.counting_type = state.counting_type;
			this.cr_luma_mult = state.cr_luma_mult;
			this.cr_mult = state.cr_mult;
			this.cr_offset = state.cr_offset;
			this.curFrameHint = state.curFrameHint;
			this.current_frame_id = state.current_frame_id;
			this.decoder_buffer_delay = copy ? state.decoder_buffer_delay?.Clone() : state.decoder_buffer_delay;
			this.decoder_model_info_present_flag = state.decoder_model_info_present_flag;
			this.decoder_model_present_for_this_op = copy ? state.decoder_model_present_for_this_op?.Clone() : state.decoder_model_present_for_this_op;
			this.delta_coded = state.delta_coded;
			this.delta_frame_id_length_minus_2 = state.delta_frame_id_length_minus_2;
			this.delta_frame_id_minus_1 = state.delta_frame_id_minus_1;
			this.delta_lf_multi = state.delta_lf_multi;
			this.delta_lf_present = state.delta_lf_present;
			this.delta_lf_res = state.delta_lf_res;
			this.delta_q = state.delta_q;
			this.delta_q_present = state.delta_q_present;
			this.delta_q_res = state.delta_q_res;
			this.diff_uv_delta = state.diff_uv_delta;
			this.disable_cdf_update = state.disable_cdf_update;
			this.disable_frame_end_update_cdf = state.disable_frame_end_update_cdf;
			this.discontinuity_flag = state.discontinuity_flag;
			this.display_frame_id = state.display_frame_id;
			this.enable_cdef = state.enable_cdef;
			this.enable_dual_filter = state.enable_dual_filter;
			this.enable_filter_intra = state.enable_filter_intra;
			this.enable_interintra_compound = state.enable_interintra_compound;
			this.enable_intra_edge_filter = state.enable_intra_edge_filter;
			this.enable_jnt_comp = state.enable_jnt_comp;
			this.enable_masked_compound = state.enable_masked_compound;
			this.enable_order_hint = state.enable_order_hint;
			this.enable_ref_frame_mvs = state.enable_ref_frame_mvs;
			this.enable_restoration = state.enable_restoration;
			this.enable_superres = state.enable_superres;
			this.enable_warped_motion = state.enable_warped_motion;
			this.encoder_buffer_delay = copy ? state.encoder_buffer_delay?.Clone() : state.encoder_buffer_delay;
			this.equal_picture_interval = state.equal_picture_interval;
			this.error_resilient_mode = state.error_resilient_mode;
			this.extension_header_reserved_3bits = state.extension_header_reserved_3bits;
			this.feature_enabled = state.feature_enabled;
			this.feature_value = state.feature_value;
			this.film_grain_params_present = state.film_grain_params_present;
			this.film_grain_params_ref_idx = state.film_grain_params_ref_idx;
			this.force_integer_mv = state.force_integer_mv;
			this.found_ref = state.found_ref;
			this.frame_height_bits_minus_1 = state.frame_height_bits_minus_1;
			this.frame_height_minus_1 = state.frame_height_minus_1;
			this.frame_id_numbers_present_flag = state.frame_id_numbers_present_flag;
			this.frame_presentation_time = state.frame_presentation_time;
			this.frame_presentation_time_length_minus_1 = state.frame_presentation_time_length_minus_1;
			this.frame_refs_short_signaling = state.frame_refs_short_signaling;
			this.frame_size_override_flag = state.frame_size_override_flag;
			this.frame_to_show_map_idx = state.frame_to_show_map_idx;
			this.frame_type = state.frame_type;
			this.frame_width_bits_minus_1 = state.frame_width_bits_minus_1;
			this.frame_width_minus_1 = state.frame_width_minus_1;
			this.full_timestamp_flag = state.full_timestamp_flag;
			this.gm_params = copy ? state.gm_params?.Clone() : state.gm_params;
			this.gold_frame_idx = state.gold_frame_idx;
			this.grain_scale_shift = state.grain_scale_shift;
			this.grain_scaling_minus_8 = state.grain_scaling_minus_8;
			this.grain_seed = state.grain_seed;
			this.height_in_sbs_minus_1 = state.height_in_sbs_minus_1;
			this.high = state.high;
			this.high_bitdepth = state.high_bitdepth;
			this.hours_flag = state.hours_flag;
			this.hours_value = state.hours_value;
			this.idLen = state.idLen;
			this.idx = state.idx;
			this.increment_tile_cols_log2 = state.increment_tile_cols_log2;
			this.increment_tile_rows_log2 = state.increment_tile_rows_log2;
			this.initial_display_delay_minus_1 = copy ? state.initial_display_delay_minus_1?.Clone() : state.initial_display_delay_minus_1;
			this.initial_display_delay_present_flag = state.initial_display_delay_present_flag;
			this.initial_display_delay_present_for_this_op = copy ? state.initial_display_delay_present_for_this_op?.Clone() : state.initial_display_delay_present_for_this_op;
			this.interpolation_filter = state.interpolation_filter;
			this.is_filter_switchable = state.is_filter_switchable;
			this.is_global = state.is_global;
			this.is_motion_mode_switchable = state.is_motion_mode_switchable;
			this.is_rot_zoom = state.is_rot_zoom;
			this.is_translation = state.is_translation;
			this.itu_t_t35_country_code = state.itu_t_t35_country_code;
			this.itu_t_t35_country_code_extension_byte = state.itu_t_t35_country_code_extension_byte;
			this.last_frame_idx = state.last_frame_idx;
			this.loop_filter_delta_enabled = state.loop_filter_delta_enabled;
			this.loop_filter_delta_update = state.loop_filter_delta_update;
			this.loop_filter_level = copy ? state.loop_filter_level?.Clone() : state.loop_filter_level;
			this.loop_filter_mode_deltas = copy ? state.loop_filter_mode_deltas?.Clone() : state.loop_filter_mode_deltas;
			this.loop_filter_ref_deltas = copy ? state.loop_filter_ref_deltas?.Clone() : state.loop_filter_ref_deltas;
			this.loop_filter_sharpness = state.loop_filter_sharpness;
			this.low = state.low;
			this.low_delay_mode_flag = copy ? state.low_delay_mode_flag?.Clone() : state.low_delay_mode_flag;
			this.lr_type = state.lr_type;
			this.lr_unit_extra_shift = state.lr_unit_extra_shift;
			this.lr_unit_shift = state.lr_unit_shift;
			this.lr_uv_shift = state.lr_uv_shift;
			this.luminance_max = state.luminance_max;
			this.luminance_min = state.luminance_min;
			this.matrix_coefficients = state.matrix_coefficients;
			this.max_cll = state.max_cll;
			this.max_fall = state.max_fall;
			this.max_frame_height_minus_1 = state.max_frame_height_minus_1;
			this.max_frame_width_minus_1 = state.max_frame_width_minus_1;
			this.metadata_type = state.metadata_type;
			this.minutes_flag = state.minutes_flag;
			this.minutes_value = state.minutes_value;
			this.mono_chrome = state.mono_chrome;
			this.mx = state.mx;
			this.n_frames = state.n_frames;
			this.nbBits = state.nbBits;
			this.numSyms = state.numSyms;
			this.num_cb_points = state.num_cb_points;
			this.num_cr_points = state.num_cr_points;
			this.num_ticks_per_picture_minus_1 = state.num_ticks_per_picture_minus_1;
			this.num_units_in_decoding_tick = state.num_units_in_decoding_tick;
			this.num_units_in_display_tick = state.num_units_in_display_tick;
			this.num_y_points = state.num_y_points;
			this.obu_extension_flag = state.obu_extension_flag;
			this.obu_forbidden_bit = state.obu_forbidden_bit;
			this.obu_has_size_field = state.obu_has_size_field;
			this.obu_padding_byte = state.obu_padding_byte;
			this.obu_padding_length = state.obu_padding_length;
			this.obu_reserved_1bit = state.obu_reserved_1bit;
			this.obu_size = state.obu_size;
			this.obu_type = state.obu_type;
			this.op = state.op;
			this.operating_point_idc = copy ? state.operating_point_idc?.Clone() : state.operating_point_idc;
			this.operating_points_cnt_minus_1 = state.operating_points_cnt_minus_1;
			this.order_hint = state.order_hint;
			this.order_hint_bits_minus_1 = state.order_hint_bits_minus_1;
			this.output_frame_height_in_tiles_minus_1 = state.output_frame_height_in_tiles_minus_1;
			this.output_frame_width_in_tiles_minus_1 = state.output_frame_width_in_tiles_minus_1;
			this.overlap_flag = state.overlap_flag;
			this.point_cb_scaling = copy ? state.point_cb_scaling?.Clone() : state.point_cb_scaling;
			this.point_cb_value = copy ? state.point_cb_value?.Clone() : state.point_cb_value;
			this.point_cr_scaling = copy ? state.point_cr_scaling?.Clone() : state.point_cr_scaling;
			this.point_cr_value = copy ? state.point_cr_value?.Clone() : state.point_cr_value;
			this.point_y_scaling = copy ? state.point_y_scaling?.Clone() : state.point_y_scaling;
			this.point_y_value = copy ? state.point_y_value?.Clone() : state.point_y_value;
			this.primary_chromaticity_x = copy ? state.primary_chromaticity_x?.Clone() : state.primary_chromaticity_x;
			this.primary_chromaticity_y = copy ? state.primary_chromaticity_y?.Clone() : state.primary_chromaticity_y;
			this.primary_ref_frame = state.primary_ref_frame;
			this.qm_u = state.qm_u;
			this.qm_v = state.qm_v;
			this.qm_y = state.qm_y;
			this.r = state.r;
			this.reduced_still_picture_header = state.reduced_still_picture_header;
			this.reduced_tx_set = state.reduced_tx_set;
			this.ref_frame_idx = copy ? state.ref_frame_idx?.Clone() : state.ref_frame_idx;
			this.ref_order_hint = copy ? state.ref_order_hint?.Clone() : state.ref_order_hint;
			this.reference_select = state.reference_select;
			this.refresh_frame_flags = state.refresh_frame_flags;
			this.render_and_frame_size_different = state.render_and_frame_size_different;
			this.render_height_minus_1 = state.render_height_minus_1;
			this.render_width_minus_1 = state.render_width_minus_1;
			this.scalability_mode_idc = state.scalability_mode_idc;
			this.scalability_structure_reserved_3bits = state.scalability_structure_reserved_3bits;
			this.seconds_flag = state.seconds_flag;
			this.seconds_value = state.seconds_value;
			this.segmentation_enabled = state.segmentation_enabled;
			this.segmentation_temporal_update = state.segmentation_temporal_update;
			this.segmentation_update_data = state.segmentation_update_data;
			this.segmentation_update_map = state.segmentation_update_map;
			this.separate_uv_delta_q = state.separate_uv_delta_q;
			this.seq_choose_integer_mv = state.seq_choose_integer_mv;
			this.seq_choose_screen_content_tools = state.seq_choose_screen_content_tools;
			this.seq_force_integer_mv = state.seq_force_integer_mv;
			this.seq_force_screen_content_tools = state.seq_force_screen_content_tools;
			this.seq_level_idx = copy ? state.seq_level_idx?.Clone() : state.seq_level_idx;
			this.seq_profile = state.seq_profile;
			this.seq_tier = copy ? state.seq_tier?.Clone() : state.seq_tier;
			this.shiftedOrderHints = copy ? state.shiftedOrderHints?.Clone() : state.shiftedOrderHints;
			this.show_existing_frame = state.show_existing_frame;
			this.show_frame = state.show_frame;
			this.showable_frame = state.showable_frame;
			this.skip_mode_present = state.skip_mode_present;
			this.spatial_id = state.spatial_id;
			this.spatial_layer_description_present_flag = state.spatial_layer_description_present_flag;
			this.spatial_layer_dimensions_present_flag = state.spatial_layer_dimensions_present_flag;
			this.spatial_layer_max_height = copy ? state.spatial_layer_max_height?.Clone() : state.spatial_layer_max_height;
			this.spatial_layer_max_width = copy ? state.spatial_layer_max_width?.Clone() : state.spatial_layer_max_width;
			this.spatial_layer_ref_id = copy ? state.spatial_layer_ref_id?.Clone() : state.spatial_layer_ref_id;
			this.spatial_layers_cnt_minus_1 = state.spatial_layers_cnt_minus_1;
			this.startPosition = state.startPosition;
			this.still_picture = state.still_picture;
			this.subexp_bits = state.subexp_bits;
			this.subexp_final_bits = state.subexp_final_bits;
			this.subexp_more_bits = state.subexp_more_bits;
			this.subsampling_x = state.subsampling_x;
			this.subsampling_y = state.subsampling_y;
			this.sz = state.sz;
			this.target = state.target;
			this.temporal_group_description_present_flag = state.temporal_group_description_present_flag;
			this.temporal_group_ref_cnt = copy ? state.temporal_group_ref_cnt?.Clone() : state.temporal_group_ref_cnt;
			this.temporal_group_ref_pic_diff = copy ? state.temporal_group_ref_pic_diff?.Clone() : state.temporal_group_ref_pic_diff;
			this.temporal_group_size = state.temporal_group_size;
			this.temporal_group_spatial_switching_up_point_flag = copy ? state.temporal_group_spatial_switching_up_point_flag?.Clone() : state.temporal_group_spatial_switching_up_point_flag;
			this.temporal_group_temporal_id = copy ? state.temporal_group_temporal_id?.Clone() : state.temporal_group_temporal_id;
			this.temporal_group_temporal_switching_up_point_flag = copy ? state.temporal_group_temporal_switching_up_point_flag?.Clone() : state.temporal_group_temporal_switching_up_point_flag;
			this.temporal_id = state.temporal_id;
			this.tg_end = state.tg_end;
			this.tg_start = state.tg_start;
			this.tile_count_minus_1 = state.tile_count_minus_1;
			this.tile_data_size_minus_1 = state.tile_data_size_minus_1;
			this.tile_size_bytes_minus_1 = state.tile_size_bytes_minus_1;
			this.tile_size_minus_1 = state.tile_size_minus_1;
			this.tile_start_and_end_present_flag = state.tile_start_and_end_present_flag;
			this.time_offset_length = state.time_offset_length;
			this.time_offset_value = state.time_offset_value;
			this.time_scale = state.time_scale;
			this.timing_info_present_flag = state.timing_info_present_flag;
			this.trailing_one_bit = state.trailing_one_bit;
			this.trailing_zero_bit = state.trailing_zero_bit;
			this.transfer_characteristics = state.transfer_characteristics;
			this.twelve_bit = state.twelve_bit;
			this.tx_mode_select = state.tx_mode_select;
			this.type = state.type;
			this.uniform_tile_spacing_flag = state.uniform_tile_spacing_flag;
			this.update_grain = state.update_grain;
			this.update_mode_delta = state.update_mode_delta;
			this.update_ref_delta = state.update_ref_delta;
			this.use_128x128_superblock = state.use_128x128_superblock;
			this.use_ref_frame_mvs = state.use_ref_frame_mvs;
			this.use_superres = state.use_superres;
			this.usedFrame = copy ? state.usedFrame?.Clone() : state.usedFrame;
			this.using_qmatrix = state.using_qmatrix;
			this.v = state.v;
			this.white_point_chromaticity_x = state.white_point_chromaticity_x;
			this.white_point_chromaticity_y = state.white_point_chromaticity_y;
			this.width_in_sbs_minus_1 = state.width_in_sbs_minus_1;
			this.zero_bit = state.zero_bit;
			LoadContextExtra(state);
		}

		partial void SaveContextExtra(ContextState state);
		partial void LoadContextExtra(ContextState state);

    }
}
