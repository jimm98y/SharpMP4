using System;
using System.Collections.Generic;
using System.Numerics;
using SharpAVX;

namespace SharpAV2
{

    public partial class AV2Context : IAomContext
    {
        /// <summary>
        /// Writing, the state the OBU's syntax elements were read into, and the same state as it was
        /// changed: an element whose value the two give alike is written as it was read.
        /// </summary>
        private AV2Context _original;
        private AV2Context _edited;

    /*
open_bitstream_unit( sz ) {
obu_header()	
obuPayloadSize = sz - 1 - obu_header_extension_flag	
startPosition = get_position()	
load_xlayer_context( obu_xlayer_id )	
if ( obu_type == OBU_SEQUENCE_HEADER ) {	
sequence_header_obu()	
} else if ( obu_type == OBU_TEMPORAL_DELIMITER ) {	
FirstPictureInTU = 1	
temporal_delimiter_obu()	
} else if ( obu_type == OBU_MSDO ) {	
multistream_decoder_operation_obu()	
} else if ( obu_type == OBU_MULTI_FRAME_HEADER ) {	
multi_frame_header_obu()	
} else if ( is_sef() || is_tip_frame() || obu_type == OBU_BRIDGE_FRAME ) {	
frame_header( 1 )	
} else if ( obu_type == OBU_METADATA_SHORT ) {	
metadata_short_obu( obuPayloadSize )	
} else if ( obu_type == OBU_METADATA_GROUP ) {	
metadata_group_obu()	
} else if ( is_tile_group() ) {	
tile_group_obu( obuPayloadSize )	
} else if ( obu_type == OBU_LAYER_CONFIGURATION_RECORD ) {	
layer_config_record_obu()	
} else if ( obu_type == OBU_ATLAS_SEGMENT ) {	
atlas_segment_info_obu()	
} else if ( obu_type == OBU_OPERATING_POINT_SET ) {	
operating_point_set_obu()	
} else if ( obu_type == OBU_BUFFER_REMOVAL_TIMING ) {	
buffer_removal_timing_obu()	
} else if ( obu_type == OBU_QUANTIZATION_MATRIX ) {	
quantizer_matrix_obu()	
} else if ( obu_type == OBU_FILM_GRAIN ) {	
film_grain_obu()	
} else if ( obu_type == OBU_CONTENT_INTERPRETATION ) {	
content_interpretation_obu()	
} else if ( obu_type == OBU_PADDING ) {	
padding_obu()	
} else {	
reserved_obu()	
}	
usedArith = is_tile_group()	
currentPosition = get_position()	
parsedPayloadBits = currentPosition - startPosition	
remainingPayloadBits = obuPayloadSize * 8 - parsedPayloadBits	
if ( obuPayloadSize > 0 && !usedArith ) {	
if ( is_extensible_obu() ) {		
obu_extension_flag f(1)
if ( obu_extension_flag ) {	
obu_extension_data( remainingPayloadBits - 1 )	
} else {	
trailing_bits( remainingPayloadBits - 1 )	
}	
} else {	
trailing_bits( remainingPayloadBits )	
}	
}	
save_xlayer_context( obu_xlayer_id )	
}
    */
		private int sz;
		public int _Sz { get { return sz; } set { sz = value; } }
		private int FirstPictureInTU;
		public int _FirstPictureInTU { get { return FirstPictureInTU; } set { FirstPictureInTU = value; } }
		private int obu_extension_flag;
		public int _ObuExtensionFlag { get { return obu_extension_flag; } set { obu_extension_flag = value; } }

        private void OpenBitstreamUnit(int sz)
        {
			int obuPayloadSize = 0;
			int startPosition = 0;
			int usedArith = 0;
			int currentPosition = 0;
			int parsedPayloadBits = 0;
			int remainingPayloadBits = 0;
			ObuHeader(); 
			obuPayloadSize = ((sz - 1) - obu_header_extension_flag);
			startPosition = get_position();
			LoadXlayerContext(obu_xlayer_id); 

			if ((obu_type == OBU_SEQUENCE_HEADER))
			{
				SequenceHeaderObu(); 
			}
			else if ((obu_type == OBU_TEMPORAL_DELIMITER))
			{
				FirstPictureInTU = 1;
				TemporalDelimiterObu(); 
			}
			else if ((obu_type == OBU_MSDO))
			{
				MultistreamDecoderOperationObu(); 
			}
			else if ((obu_type == OBU_MULTI_FRAME_HEADER))
			{
				MultiFrameHeaderObu(); 
			}
			else if ((((IsSef() != 0) || (IsTipFrame() != 0)) || (obu_type == OBU_BRIDGE_FRAME)))
			{
				FrameHeader(1); 
			}
			else if ((obu_type == OBU_METADATA_SHORT))
			{
				MetadataShortObu(obuPayloadSize); 
			}
			else if ((obu_type == OBU_METADATA_GROUP))
			{
				MetadataGroupObu(); 
			}
			else if ((IsTileGroup() != 0))
			{
				TileGroupObu(obuPayloadSize); 
			}
			else if ((obu_type == OBU_LAYER_CONFIGURATION_RECORD))
			{
				LayerConfigRecordObu(); 
			}
			else if ((obu_type == OBU_ATLAS_SEGMENT))
			{
				AtlasSegmentInfoObu(); 
			}
			else if ((obu_type == OBU_OPERATING_POINT_SET))
			{
				OperatingPointSetObu(); 
			}
			else if ((obu_type == OBU_BUFFER_REMOVAL_TIMING))
			{
				BufferRemovalTimingObu(); 
			}
			else if ((obu_type == OBU_QUANTIZATION_MATRIX))
			{
				QuantizerMatrixObu(); 
			}
			else if ((obu_type == OBU_FILM_GRAIN))
			{
				FilmGrainObu(); 
			}
			else if ((obu_type == OBU_CONTENT_INTERPRETATION))
			{
				ContentInterpretationObu(); 
			}
			else if ((obu_type == OBU_PADDING))
			{
				PaddingObu(); 
			}
			else 
			{
				ReservedObu(); 
			}
			usedArith = IsTileGroup();
			currentPosition = get_position();
			parsedPayloadBits = (currentPosition - startPosition);
			remainingPayloadBits = ((obuPayloadSize * 8) - parsedPayloadBits);

			if (((obuPayloadSize > 0) && !(usedArith != 0)))
			{

				if ((IsExtensibleObu() != 0))
				{
					stream.ReadFixed(1, out this.obu_extension_flag, "obu_extension_flag"); 

					if ((obu_extension_flag != 0))
					{
						ObuExtensionData((remainingPayloadBits - 1)); 
					}
					else 
					{
						TrailingBits((remainingPayloadBits - 1)); 
					}
				}
				else 
				{
					TrailingBits(remainingPayloadBits); 
				}
			}
			SaveXlayerContext(obu_xlayer_id); 
        }

        private void WriteOpenBitstreamUnit(int sz)
        {
			int obuPayloadSize = 0;
			int startPosition = 0;
			int usedArith = 0;
			int currentPosition = 0;
			int parsedPayloadBits = 0;
			int remainingPayloadBits = 0;
			WriteObuHeader(); 
			obuPayloadSize = ((sz - 1) - obu_header_extension_flag);
			startPosition = get_position();
			LoadXlayerContext(obu_xlayer_id); 

			if ((obu_type == OBU_SEQUENCE_HEADER))
			{
				WriteSequenceHeaderObu(); 
			}
			else if ((obu_type == OBU_TEMPORAL_DELIMITER))
			{
				FirstPictureInTU = 1;
				TemporalDelimiterObu(); 
			}
			else if ((obu_type == OBU_MSDO))
			{
				WriteMultistreamDecoderOperationObu(); 
			}
			else if ((obu_type == OBU_MULTI_FRAME_HEADER))
			{
				WriteMultiFrameHeaderObu(); 
			}
			else if ((((IsSef() != 0) || (IsTipFrame() != 0)) || (obu_type == OBU_BRIDGE_FRAME)))
			{
				WriteFrameHeader(1); 
			}
			else if ((obu_type == OBU_METADATA_SHORT))
			{
				WriteMetadataShortObu(obuPayloadSize); 
			}
			else if ((obu_type == OBU_METADATA_GROUP))
			{
				WriteMetadataGroupObu(); 
			}
			else if ((IsTileGroup() != 0))
			{
				WriteTileGroupObu(obuPayloadSize); 
			}
			else if ((obu_type == OBU_LAYER_CONFIGURATION_RECORD))
			{
				WriteLayerConfigRecordObu(); 
			}
			else if ((obu_type == OBU_ATLAS_SEGMENT))
			{
				WriteAtlasSegmentInfoObu(); 
			}
			else if ((obu_type == OBU_OPERATING_POINT_SET))
			{
				WriteOperatingPointSetObu(); 
			}
			else if ((obu_type == OBU_BUFFER_REMOVAL_TIMING))
			{
				WriteBufferRemovalTimingObu(); 
			}
			else if ((obu_type == OBU_QUANTIZATION_MATRIX))
			{
				WriteQuantizerMatrixObu(); 
			}
			else if ((obu_type == OBU_FILM_GRAIN))
			{
				WriteFilmGrainObu(); 
			}
			else if ((obu_type == OBU_CONTENT_INTERPRETATION))
			{
				WriteContentInterpretationObu(); 
			}
			else if ((obu_type == OBU_PADDING))
			{
				WritePaddingObu(); 
			}
			else 
			{
				ReservedObu(); 
			}
			usedArith = IsTileGroup();
			currentPosition = get_position();
			parsedPayloadBits = (currentPosition - startPosition);
			remainingPayloadBits = ((obuPayloadSize * 8) - parsedPayloadBits);

			if (((obuPayloadSize > 0) && !(usedArith != 0)))
			{

				if ((IsExtensibleObu() != 0))
				{
					this.obu_extension_flag = stream.Pick("obu_extension_flag", _original != null ? _original.obu_extension_flag : this.obu_extension_flag, _edited != null ? _edited.obu_extension_flag : _original != null ? _original.obu_extension_flag : this.obu_extension_flag);
					stream.WriteFixed(1, this.obu_extension_flag, "obu_extension_flag"); 

					if ((obu_extension_flag != 0))
					{
						WriteObuExtensionData((remainingPayloadBits - 1)); 
					}
					else 
					{
						WriteTrailingBits((remainingPayloadBits - 1)); 
					}
				}
				else 
				{
					WriteTrailingBits(remainingPayloadBits); 
				}
			}
			SaveXlayerContext(obu_xlayer_id); 
        }

    /*
obu_extension_data( sz ) {
for ( i = 0; i < sz; i++ ) {	
obu_extension_data_bit	f(1)
}	
}
    */
		private int obu_extension_data_bit;
		public int _ObuExtensionDataBit { get { return obu_extension_data_bit; } set { obu_extension_data_bit = value; } }
		private int i = 0;

        private void ObuExtensionData(int sz)
        {
			int i = 0;

			for (i = 0; (i < sz); i++)
			{
				stream.ReadFixed(1, out this.obu_extension_data_bit, "obu_extension_data_bit"); 
			}
        }

        private void WriteObuExtensionData(int sz)
        {
			int i = 0;

			for (i = 0; (i < sz); i++)
			{
				this.obu_extension_data_bit = stream.Pick("obu_extension_data_bit", _original != null ? _original.obu_extension_data_bit : this.obu_extension_data_bit, _edited != null ? _edited.obu_extension_data_bit : _original != null ? _original.obu_extension_data_bit : this.obu_extension_data_bit);
				stream.WriteFixed(1, this.obu_extension_data_bit, "obu_extension_data_bit"); 
			}
        }

    /*
obu_header() {
obu_header_extension_flag	f(1)
obu_type	f(5)
obu_tlayer_id	f(2)
if ( obu_header_extension_flag == 1 ) {	
obu_mlayer_id	f(3)
obu_xlayer_id	f(5)
} else {	
obu_mlayer_id = 0	
obu_xlayer_id = ( obu_type == OBU_MSDO || obu_type == OBU_TEMPORAL_DELIMITER ) ? GLOBAL_XLAYER_ID : 0	
}	
}
    */
		private int obu_header_extension_flag;
		public int _ObuHeaderExtensionFlag { get { return obu_header_extension_flag; } set { obu_header_extension_flag = value; } }
		private int obu_type;
		public int _ObuType { get { return obu_type; } set { obu_type = value; } }
		private int obu_tlayer_id;
		public int _ObuTlayerId { get { return obu_tlayer_id; } set { obu_tlayer_id = value; } }
		private int obu_mlayer_id;
		public int _ObuMlayerId { get { return obu_mlayer_id; } set { obu_mlayer_id = value; } }
		private int obu_xlayer_id;
		public int _ObuXlayerId { get { return obu_xlayer_id; } set { obu_xlayer_id = value; } }

        private void ObuHeader()
        {
			stream.ReadFixed(1, out this.obu_header_extension_flag, "obu_header_extension_flag"); 
			stream.ReadFixed(5, out this.obu_type, "obu_type"); 
			stream.ReadFixed(2, out this.obu_tlayer_id, "obu_tlayer_id"); 

			if ((obu_header_extension_flag == 1))
			{
				stream.ReadFixed(3, out this.obu_mlayer_id, "obu_mlayer_id"); 
				stream.ReadFixed(5, out this.obu_xlayer_id, "obu_xlayer_id"); 
			}
			else 
			{
				obu_mlayer_id = 0;
				obu_xlayer_id = (((obu_type == OBU_MSDO) || (obu_type == OBU_TEMPORAL_DELIMITER)) ? GLOBAL_XLAYER_ID : 0);
			}
        }

        private void WriteObuHeader()
        {
			this.obu_header_extension_flag = stream.Pick("obu_header_extension_flag", _original != null ? _original.obu_header_extension_flag : this.obu_header_extension_flag, _edited != null ? _edited.obu_header_extension_flag : _original != null ? _original.obu_header_extension_flag : this.obu_header_extension_flag);
			stream.WriteFixed(1, this.obu_header_extension_flag, "obu_header_extension_flag"); 
			this.obu_type = stream.Pick("obu_type", _original != null ? _original.obu_type : this.obu_type, _edited != null ? _edited.obu_type : _original != null ? _original.obu_type : this.obu_type);
			stream.WriteFixed(5, this.obu_type, "obu_type"); 
			this.obu_tlayer_id = stream.Pick("obu_tlayer_id", _original != null ? _original.obu_tlayer_id : this.obu_tlayer_id, _edited != null ? _edited.obu_tlayer_id : _original != null ? _original.obu_tlayer_id : this.obu_tlayer_id);
			stream.WriteFixed(2, this.obu_tlayer_id, "obu_tlayer_id"); 

			if ((obu_header_extension_flag == 1))
			{
				this.obu_mlayer_id = stream.Pick("obu_mlayer_id", _original != null ? _original.obu_mlayer_id : this.obu_mlayer_id, _edited != null ? _edited.obu_mlayer_id : _original != null ? _original.obu_mlayer_id : this.obu_mlayer_id);
				stream.WriteFixed(3, this.obu_mlayer_id, "obu_mlayer_id"); 
				this.obu_xlayer_id = stream.Pick("obu_xlayer_id", _original != null ? _original.obu_xlayer_id : this.obu_xlayer_id, _edited != null ? _edited.obu_xlayer_id : _original != null ? _original.obu_xlayer_id : this.obu_xlayer_id);
				stream.WriteFixed(5, this.obu_xlayer_id, "obu_xlayer_id"); 
			}
			else 
			{
				obu_mlayer_id = 0;
				obu_xlayer_id = (((obu_type == OBU_MSDO) || (obu_type == OBU_TEMPORAL_DELIMITER)) ? GLOBAL_XLAYER_ID : 0);
			}
        }

    /*
trailing_bits( nbBits ) {
trailing_one_bit	f(1)
nbBits--	
while ( nbBits > 0 ) {	
trailing_zero_bit	f(1)
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
while ( get_position() & 7 ) {	
zero_bit	f(1)
}	
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
seq_header_id	uvlc()
seq_profile_idc	f(5)
single_picture_header_flag	f(1)
seq_level_idx	f(5)
if ( seq_level_idx > 3 && !single_picture_header_flag ) {	
seq_tier	f(1)
} else {	
seq_tier = 0	
}	
chroma_format_idc	uvlc()
bit_depth_idc	uvlc()
set_chroma_format_and_bit_depth()	
if ( single_picture_header_flag ) {	
seq_lcr_id = 0	
still_picture = 1	
max_tlayer_id = 0	
max_mlayer_id = 0	
SeqMaxMlayerCnt = 1	
monotonic_output_order_flag = 1	
} else {	
seq_lcr_id	f(3)
still_picture	f(1)
max_tlayer_id	f(2)
max_mlayer_id	f(3)
if ( max_mlayer_id > 0 ) {	
n = CeilLog2(max_mlayer_id + 1)	
seq_max_mlayer_cnt_minus_1	f(n)
SeqMaxMlayerCnt = seq_max_mlayer_cnt_minus_1 + 1	
} else {	
SeqMaxMlayerCnt = 1	
}	
monotonic_output_order_flag	f(1)
}	
frame_width_bits_minus_1	f(4)
frame_height_bits_minus_1	f(4)
n = frame_width_bits_minus_1 + 1	
max_frame_width_minus_1	f(n)
n = frame_height_bits_minus_1 + 1	
max_frame_height_minus_1	f(n)
seq_cropping_window_present_flag	f(1)
if ( seq_cropping_window_present_flag ) {	
seq_cropping_win_left_offset	uvlc()
seq_cropping_win_right_offset	uvlc()
seq_cropping_win_top_offset	uvlc()
seq_cropping_win_bottom_offset	uvlc()
} else {	
seq_cropping_win_left_offset = 0	
seq_cropping_win_right_offset = 0	
seq_cropping_win_top_offset = 0	
seq_cropping_win_bottom_offset = 0	
}	
if ( single_picture_header_flag ) {	
decoder_model_info_present_flag = 0	
} else {	
seq_initial_display_delay_present_flag	f(1)
if ( seq_initial_display_delay_present_flag ) {	
seq_initial_display_delay_minus_1	f(4)
}	
decoder_model_info_present_flag	f(1)
if ( decoder_model_info_present_flag ) {	
num_units_in_decoding_tick	f(32)
seq_decoder_model_info_present_flag	f(1)
if ( seq_decoder_model_info_present_flag ) {	
seq_decoder_model_info()	
}	
}	
}	
for ( mLayer = 0; mLayer < MAX_NUM_MLAYERS; mLayer++ ) {	
for ( currTLayer = 0; currTLayer < MAX_NUM_TLAYERS; currTLayer++ ) {	
for ( refTLayer = 0; refTLayer < MAX_NUM_TLAYERS; refTLayer++ ) {	
TLayerDependencyMap[ mLayer ][ currTLayer ][ refTLayer ] =	
refTLayer <= currTLayer && currTLayer <= max_tlayer_id && mLayer <= max_mlayer_id	
}	
}	
}	
for ( currLayer = 0; currLayer < MAX_NUM_MLAYERS; currLayer++ ) {	
for ( refLayer = 0; refLayer < MAX_NUM_MLAYERS; refLayer++ ) {	
MLayerDependencyMap[ currLayer ][ refLayer ] =	
refLayer <= currLayer && currLayer <= max_mlayer_id	
}	
}	
if ( max_mlayer_id > 0 ) {	
mlayer_dependency_present_flag	f(1)
if ( mlayer_dependency_present_flag ) {	
for ( currLayer = 1; currLayer <= max_mlayer_id; currLayer++ ) {	
for ( refLayer = currLayer; refLayer >= 0; refLayer-- ) {	
mlayer_dependency_map	f(1)
MLayerDependencyMap[ currLayer ][ refLayer ] =	
mlayer_dependency_map	
}	
}	
}	
}	
if ( max_tlayer_id > 0 ) {	
tlayer_dependency_present_flag	f(1)
if ( tlayer_dependency_present_flag ) {	
if ( max_mlayer_id > 0 ) {
multi_tlayer_dependency_map_present_flag	f(1)
}
else {
multi_tlayer_dependency_map_present_flag = 0	
}
for ( mLayer = 0; mLayer <= max_mlayer_id; mLayer++ ) {	
for ( currTLayer = 1; currTLayer <= max_tlayer_id; currTLayer++ ) {	
for ( refTLayer = currTLayer; refTLayer >= 0; refTLayer-- ) {	
if (multi_tlayer_dependency_map_present_flag > 0 ||	mLayer == 0) {	
tlayer_dependency_map	f(1)
TLayerDependencyMap[ mLayer ][ currTLayer ][ refTLayer ] =	tlayer_dependency_map	
} else {	
TLayerDependencyMap[ mLayer ][ currTLayer ][ refTLayer ] =	TLayerDependencyMap[ 0 ][ currTLayer ][ refTLayer ]	
}	
}	
}	
}	
}	
}	
for (mlayerId = 0; mlayerId < MAX_NUM_MLAYERS; mlayerId++) {	
for (refMlayer = 0; refMlayer < MAX_NUM_MLAYERS; refMlayer++) {	
MLayerPresenceMap[mlayerId][refMlayer] = 0	
if ( mlayerId == refMlayer || MLayerDependencyMap[mlayerId][refMlayer]) {	
MLayerPresenceMap[mlayerId][refMlayer] = 1	
for (depMLayerId = 0; depMLayerId < refMlayer; depMLayerId++) {	
MLayerPresenceMap[mlayerId][depMLayerId] |=	MLayerPresenceMap[refMlayer][depMLayerId]	
}	
}	
}	
}	
sequence_partition_config()	
sequence_segment_config()	
sequence_intra_config()	
sequence_inter_config()	
sequence_scc_config()	
sequence_transform_quant_entropy_config()	
sequence_filter_config()	
sequence_tile_config()	
film_grain_params_present	f(1)
save_sequence_header()	
}
    */
		private int seq_header_id;
		public int _SeqHeaderId { get { return seq_header_id; } set { seq_header_id = value; } }
		private int seq_profile_idc;
		public int _SeqProfileIdc { get { return seq_profile_idc; } set { seq_profile_idc = value; } }
		private int single_picture_header_flag;
		public int _SinglePictureHeaderFlag { get { return single_picture_header_flag; } set { single_picture_header_flag = value; } }
		private int seq_level_idx;
		public int _SeqLevelIdx { get { return seq_level_idx; } set { seq_level_idx = value; } }
		private int seq_tier;
		public int _SeqTier { get { return seq_tier; } set { seq_tier = value; } }
		private int chroma_format_idc;
		public int _ChromaFormatIdc { get { return chroma_format_idc; } set { chroma_format_idc = value; } }
		private int bit_depth_idc;
		public int _BitDepthIdc { get { return bit_depth_idc; } set { bit_depth_idc = value; } }
		private int seq_lcr_id;
		public int _SeqLcrId { get { return seq_lcr_id; } set { seq_lcr_id = value; } }
		private int still_picture;
		public int _StillPicture { get { return still_picture; } set { still_picture = value; } }
		private int max_tlayer_id;
		public int _MaxTlayerId { get { return max_tlayer_id; } set { max_tlayer_id = value; } }
		private int max_mlayer_id;
		public int _MaxMlayerId { get { return max_mlayer_id; } set { max_mlayer_id = value; } }
		private int SeqMaxMlayerCnt;
		public int _SeqMaxMlayerCnt { get { return SeqMaxMlayerCnt; } set { SeqMaxMlayerCnt = value; } }
		private int monotonic_output_order_flag;
		public int _MonotonicOutputOrderFlag { get { return monotonic_output_order_flag; } set { monotonic_output_order_flag = value; } }
		private int seq_max_mlayer_cnt_minus_1;
		public int _SeqMaxMlayerCntMinus1 { get { return seq_max_mlayer_cnt_minus_1; } set { seq_max_mlayer_cnt_minus_1 = value; } }
		private int frame_width_bits_minus_1;
		public int _FrameWidthBitsMinus1 { get { return frame_width_bits_minus_1; } set { frame_width_bits_minus_1 = value; } }
		private int frame_height_bits_minus_1;
		public int _FrameHeightBitsMinus1 { get { return frame_height_bits_minus_1; } set { frame_height_bits_minus_1 = value; } }
		private int max_frame_width_minus_1;
		public int _MaxFrameWidthMinus1 { get { return max_frame_width_minus_1; } set { max_frame_width_minus_1 = value; } }
		private int max_frame_height_minus_1;
		public int _MaxFrameHeightMinus1 { get { return max_frame_height_minus_1; } set { max_frame_height_minus_1 = value; } }
		private int seq_cropping_window_present_flag;
		public int _SeqCroppingWindowPresentFlag { get { return seq_cropping_window_present_flag; } set { seq_cropping_window_present_flag = value; } }
		private int seq_cropping_win_left_offset;
		public int _SeqCroppingWinLeftOffset { get { return seq_cropping_win_left_offset; } set { seq_cropping_win_left_offset = value; } }
		private int seq_cropping_win_right_offset;
		public int _SeqCroppingWinRightOffset { get { return seq_cropping_win_right_offset; } set { seq_cropping_win_right_offset = value; } }
		private int seq_cropping_win_top_offset;
		public int _SeqCroppingWinTopOffset { get { return seq_cropping_win_top_offset; } set { seq_cropping_win_top_offset = value; } }
		private int seq_cropping_win_bottom_offset;
		public int _SeqCroppingWinBottomOffset { get { return seq_cropping_win_bottom_offset; } set { seq_cropping_win_bottom_offset = value; } }
		private int decoder_model_info_present_flag;
		public int _DecoderModelInfoPresentFlag { get { return decoder_model_info_present_flag; } set { decoder_model_info_present_flag = value; } }
		private int seq_initial_display_delay_present_flag;
		public int _SeqInitialDisplayDelayPresentFlag { get { return seq_initial_display_delay_present_flag; } set { seq_initial_display_delay_present_flag = value; } }
		private int seq_initial_display_delay_minus_1;
		public int _SeqInitialDisplayDelayMinus1 { get { return seq_initial_display_delay_minus_1; } set { seq_initial_display_delay_minus_1 = value; } }
		private int num_units_in_decoding_tick;
		public int _NumUnitsInDecodingTick { get { return num_units_in_decoding_tick; } set { num_units_in_decoding_tick = value; } }
		private int seq_decoder_model_info_present_flag;
		public int _SeqDecoderModelInfoPresentFlag { get { return seq_decoder_model_info_present_flag; } set { seq_decoder_model_info_present_flag = value; } }
		private AomArray<AomArray<AomArray<int>>> TLayerDependencyMap = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _TLayerDependencyMap { get { return TLayerDependencyMap; } set { TLayerDependencyMap = value; } }
		private AomArray<AomArray<int>> MLayerDependencyMap = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _MLayerDependencyMap { get { return MLayerDependencyMap; } set { MLayerDependencyMap = value; } }
		private int mlayer_dependency_present_flag;
		public int _MlayerDependencyPresentFlag { get { return mlayer_dependency_present_flag; } set { mlayer_dependency_present_flag = value; } }
		private int mlayer_dependency_map;
		public int _MlayerDependencyMap { get { return mlayer_dependency_map; } set { mlayer_dependency_map = value; } }
		private int tlayer_dependency_present_flag;
		public int _TlayerDependencyPresentFlag { get { return tlayer_dependency_present_flag; } set { tlayer_dependency_present_flag = value; } }
		private int multi_tlayer_dependency_map_present_flag;
		public int _MultiTlayerDependencyMapPresentFlag { get { return multi_tlayer_dependency_map_present_flag; } set { multi_tlayer_dependency_map_present_flag = value; } }
		private int tlayer_dependency_map;
		public int _TlayerDependencyMap { get { return tlayer_dependency_map; } set { tlayer_dependency_map = value; } }
		private AomArray<AomArray<int>> MLayerPresenceMap = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _MLayerPresenceMap { get { return MLayerPresenceMap; } set { MLayerPresenceMap = value; } }
		private int film_grain_params_present;
		public int _FilmGrainParamsPresent { get { return film_grain_params_present; } set { film_grain_params_present = value; } }
		private int mLayer = 0;
		private int currTLayer = 0;
		private int refTLayer = 0;
		private int currLayer = 0;
		private int refLayer = 0;
		private int mlayerId = 0;
		private int refMlayer = 0;
		private int depMLayerId = 0;

        private void SequenceHeaderObu()
        {
			int mLayer = 0;
			int currTLayer = 0;
			int refTLayer = 0;
			int currLayer = 0;
			int refLayer = 0;
			int mlayerId = 0;
			int refMlayer = 0;
			int depMLayerId = 0;
			int n = 0;
			stream.ReadUvlc( out this.seq_header_id, "seq_header_id"); 
			stream.ReadFixed(5, out this.seq_profile_idc, "seq_profile_idc"); 
			stream.ReadFixed(1, out this.single_picture_header_flag, "single_picture_header_flag"); 
			stream.ReadFixed(5, out this.seq_level_idx, "seq_level_idx"); 

			if (((seq_level_idx > 3) && !(single_picture_header_flag != 0)))
			{
				stream.ReadFixed(1, out this.seq_tier, "seq_tier"); 
			}
			else 
			{
				seq_tier = 0;
			}
			stream.ReadUvlc( out this.chroma_format_idc, "chroma_format_idc"); 
			stream.ReadUvlc( out this.bit_depth_idc, "bit_depth_idc"); 
			SetChromaFormatAndBitDepth(); 

			if ((single_picture_header_flag != 0))
			{
				seq_lcr_id = 0;
				still_picture = 1;
				max_tlayer_id = 0;
				max_mlayer_id = 0;
				SeqMaxMlayerCnt = 1;
				monotonic_output_order_flag = 1;
			}
			else 
			{
				stream.ReadFixed(3, out this.seq_lcr_id, "seq_lcr_id"); 
				stream.ReadFixed(1, out this.still_picture, "still_picture"); 
				stream.ReadFixed(2, out this.max_tlayer_id, "max_tlayer_id"); 
				stream.ReadFixed(3, out this.max_mlayer_id, "max_mlayer_id"); 

				if ((max_mlayer_id > 0))
				{
					n = CeilLog2((max_mlayer_id + 1));
					stream.ReadVariable(n, out this.seq_max_mlayer_cnt_minus_1, "seq_max_mlayer_cnt_minus_1"); 
					SeqMaxMlayerCnt = (seq_max_mlayer_cnt_minus_1 + 1);
				}
				else 
				{
					SeqMaxMlayerCnt = 1;
				}
				stream.ReadFixed(1, out this.monotonic_output_order_flag, "monotonic_output_order_flag"); 
			}
			stream.ReadFixed(4, out this.frame_width_bits_minus_1, "frame_width_bits_minus_1"); 
			stream.ReadFixed(4, out this.frame_height_bits_minus_1, "frame_height_bits_minus_1"); 
			n = (frame_width_bits_minus_1 + 1);
			stream.ReadVariable(n, out this.max_frame_width_minus_1, "max_frame_width_minus_1"); 
			n = (frame_height_bits_minus_1 + 1);
			stream.ReadVariable(n, out this.max_frame_height_minus_1, "max_frame_height_minus_1"); 
			stream.ReadFixed(1, out this.seq_cropping_window_present_flag, "seq_cropping_window_present_flag"); 

			if ((seq_cropping_window_present_flag != 0))
			{
				stream.ReadUvlc( out this.seq_cropping_win_left_offset, "seq_cropping_win_left_offset"); 
				stream.ReadUvlc( out this.seq_cropping_win_right_offset, "seq_cropping_win_right_offset"); 
				stream.ReadUvlc( out this.seq_cropping_win_top_offset, "seq_cropping_win_top_offset"); 
				stream.ReadUvlc( out this.seq_cropping_win_bottom_offset, "seq_cropping_win_bottom_offset"); 
			}
			else 
			{
				seq_cropping_win_left_offset = 0;
				seq_cropping_win_right_offset = 0;
				seq_cropping_win_top_offset = 0;
				seq_cropping_win_bottom_offset = 0;
			}

			if ((single_picture_header_flag != 0))
			{
				decoder_model_info_present_flag = 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.seq_initial_display_delay_present_flag, "seq_initial_display_delay_present_flag"); 

				if ((seq_initial_display_delay_present_flag != 0))
				{
					stream.ReadFixed(4, out this.seq_initial_display_delay_minus_1, "seq_initial_display_delay_minus_1"); 
				}
				stream.ReadFixed(1, out this.decoder_model_info_present_flag, "decoder_model_info_present_flag"); 

				if ((decoder_model_info_present_flag != 0))
				{
					stream.ReadFixed(32, out this.num_units_in_decoding_tick, "num_units_in_decoding_tick"); 
					stream.ReadFixed(1, out this.seq_decoder_model_info_present_flag, "seq_decoder_model_info_present_flag"); 

					if ((seq_decoder_model_info_present_flag != 0))
					{
						SeqDecoderModelInfo(); 
					}
				}
			}

			for (mLayer = 0; (mLayer < MAX_NUM_MLAYERS); mLayer++)
			{

				for (currTLayer = 0; (currTLayer < MAX_NUM_TLAYERS); currTLayer++)
				{

					for (refTLayer = 0; (refTLayer < MAX_NUM_TLAYERS); refTLayer++)
					{
						TLayerDependencyMap[mLayer][currTLayer][refTLayer] = ((((refTLayer <= currTLayer) && (currTLayer <= max_tlayer_id)) && (mLayer <= max_mlayer_id)) ? 1 : 0);
					}
				}
			}

			for (currLayer = 0; (currLayer < MAX_NUM_MLAYERS); currLayer++)
			{

				for (refLayer = 0; (refLayer < MAX_NUM_MLAYERS); refLayer++)
				{
					MLayerDependencyMap[currLayer][refLayer] = (((refLayer <= currLayer) && (currLayer <= max_mlayer_id)) ? 1 : 0);
				}
			}

			if ((max_mlayer_id > 0))
			{
				stream.ReadFixed(1, out this.mlayer_dependency_present_flag, "mlayer_dependency_present_flag"); 

				if ((mlayer_dependency_present_flag != 0))
				{

					for (currLayer = 1; (currLayer <= max_mlayer_id); currLayer++)
					{

						for (refLayer = currLayer; (refLayer >= 0); refLayer--)
						{
							stream.ReadFixed(1, out this.mlayer_dependency_map, "mlayer_dependency_map"); 
							MLayerDependencyMap[currLayer][refLayer] = mlayer_dependency_map;
						}
					}
				}
			}

			if ((max_tlayer_id > 0))
			{
				stream.ReadFixed(1, out this.tlayer_dependency_present_flag, "tlayer_dependency_present_flag"); 

				if ((tlayer_dependency_present_flag != 0))
				{

					if ((max_mlayer_id > 0))
					{
						stream.ReadFixed(1, out this.multi_tlayer_dependency_map_present_flag, "multi_tlayer_dependency_map_present_flag"); 
					}
					else 
					{
						multi_tlayer_dependency_map_present_flag = 0;
					}

					for (mLayer = 0; (mLayer <= max_mlayer_id); mLayer++)
					{

						for (currTLayer = 1; (currTLayer <= max_tlayer_id); currTLayer++)
						{

							for (refTLayer = currTLayer; (refTLayer >= 0); refTLayer--)
							{

								if (((multi_tlayer_dependency_map_present_flag > 0) || (mLayer == 0)))
								{
									stream.ReadFixed(1, out this.tlayer_dependency_map, "tlayer_dependency_map"); 
									TLayerDependencyMap[mLayer][currTLayer][refTLayer] = tlayer_dependency_map;
								}
								else 
								{
									TLayerDependencyMap[mLayer][currTLayer][refTLayer] = TLayerDependencyMap[0][currTLayer][refTLayer];
								}
							}
						}
					}
				}
			}

			for (mlayerId = 0; (mlayerId < MAX_NUM_MLAYERS); mlayerId++)
			{

				for (refMlayer = 0; (refMlayer < MAX_NUM_MLAYERS); refMlayer++)
				{
					MLayerPresenceMap[mlayerId][refMlayer] = 0;

					if (((mlayerId == refMlayer) || (MLayerDependencyMap[mlayerId][refMlayer] != 0)))
					{
						MLayerPresenceMap[mlayerId][refMlayer] = 1;

						for (depMLayerId = 0; (depMLayerId < refMlayer); depMLayerId++)
						{
							MLayerPresenceMap[mlayerId][depMLayerId] |= MLayerPresenceMap[refMlayer][depMLayerId];
						}
					}
				}
			}
			SequencePartitionConfig(); 
			SequenceSegmentConfig(); 
			SequenceIntraConfig(); 
			SequenceInterConfig(); 
			SequenceSccConfig(); 
			SequenceTransformQuantEntropyConfig(); 
			SequenceFilterConfig(); 
			SequenceTileConfig(); 
			stream.ReadFixed(1, out this.film_grain_params_present, "film_grain_params_present"); 
			save_sequence_header(); 
        }

        private void WriteSequenceHeaderObu()
        {
			int mLayer = 0;
			int currTLayer = 0;
			int refTLayer = 0;
			int currLayer = 0;
			int refLayer = 0;
			int mlayerId = 0;
			int refMlayer = 0;
			int depMLayerId = 0;
			int n = 0;
			this.seq_header_id = stream.Pick("seq_header_id", _original != null ? _original.seq_header_id : this.seq_header_id, _edited != null ? _edited.seq_header_id : _original != null ? _original.seq_header_id : this.seq_header_id);
			stream.WriteUvlc( this.seq_header_id, "seq_header_id"); 
			this.seq_profile_idc = stream.Pick("seq_profile_idc", _original != null ? _original.seq_profile_idc : this.seq_profile_idc, _edited != null ? _edited.seq_profile_idc : _original != null ? _original.seq_profile_idc : this.seq_profile_idc);
			stream.WriteFixed(5, this.seq_profile_idc, "seq_profile_idc"); 
			this.single_picture_header_flag = stream.Pick("single_picture_header_flag", _original != null ? _original.single_picture_header_flag : this.single_picture_header_flag, _edited != null ? _edited.single_picture_header_flag : _original != null ? _original.single_picture_header_flag : this.single_picture_header_flag);
			stream.WriteFixed(1, this.single_picture_header_flag, "single_picture_header_flag"); 
			this.seq_level_idx = stream.Pick("seq_level_idx", _original != null ? _original.seq_level_idx : this.seq_level_idx, _edited != null ? _edited.seq_level_idx : _original != null ? _original.seq_level_idx : this.seq_level_idx);
			stream.WriteFixed(5, this.seq_level_idx, "seq_level_idx"); 

			if (((seq_level_idx > 3) && !(single_picture_header_flag != 0)))
			{
				this.seq_tier = stream.Pick("seq_tier", _original != null ? _original.seq_tier : this.seq_tier, _edited != null ? _edited.seq_tier : _original != null ? _original.seq_tier : this.seq_tier);
				stream.WriteFixed(1, this.seq_tier, "seq_tier"); 
			}
			else 
			{
				seq_tier = 0;
			}
			this.chroma_format_idc = stream.Pick("chroma_format_idc", _original != null ? _original.chroma_format_idc : this.chroma_format_idc, _edited != null ? _edited.chroma_format_idc : _original != null ? _original.chroma_format_idc : this.chroma_format_idc);
			stream.WriteUvlc( this.chroma_format_idc, "chroma_format_idc"); 
			this.bit_depth_idc = stream.Pick("bit_depth_idc", _original != null ? _original.bit_depth_idc : this.bit_depth_idc, _edited != null ? _edited.bit_depth_idc : _original != null ? _original.bit_depth_idc : this.bit_depth_idc);
			stream.WriteUvlc( this.bit_depth_idc, "bit_depth_idc"); 
			SetChromaFormatAndBitDepth(); 

			if ((single_picture_header_flag != 0))
			{
				seq_lcr_id = 0;
				still_picture = 1;
				max_tlayer_id = 0;
				max_mlayer_id = 0;
				SeqMaxMlayerCnt = 1;
				monotonic_output_order_flag = 1;
			}
			else 
			{
				this.seq_lcr_id = stream.Pick("seq_lcr_id", _original != null ? _original.seq_lcr_id : this.seq_lcr_id, _edited != null ? _edited.seq_lcr_id : _original != null ? _original.seq_lcr_id : this.seq_lcr_id);
				stream.WriteFixed(3, this.seq_lcr_id, "seq_lcr_id"); 
				this.still_picture = stream.Pick("still_picture", _original != null ? _original.still_picture : this.still_picture, _edited != null ? _edited.still_picture : _original != null ? _original.still_picture : this.still_picture);
				stream.WriteFixed(1, this.still_picture, "still_picture"); 
				this.max_tlayer_id = stream.Pick("max_tlayer_id", _original != null ? _original.max_tlayer_id : this.max_tlayer_id, _edited != null ? _edited.max_tlayer_id : _original != null ? _original.max_tlayer_id : this.max_tlayer_id);
				stream.WriteFixed(2, this.max_tlayer_id, "max_tlayer_id"); 
				this.max_mlayer_id = stream.Pick("max_mlayer_id", _original != null ? _original.max_mlayer_id : this.max_mlayer_id, _edited != null ? _edited.max_mlayer_id : _original != null ? _original.max_mlayer_id : this.max_mlayer_id);
				stream.WriteFixed(3, this.max_mlayer_id, "max_mlayer_id"); 

				if ((max_mlayer_id > 0))
				{
					n = CeilLog2((max_mlayer_id + 1));
					this.seq_max_mlayer_cnt_minus_1 = stream.Pick("seq_max_mlayer_cnt_minus_1", _original != null ? _original.seq_max_mlayer_cnt_minus_1 : this.seq_max_mlayer_cnt_minus_1, _edited != null ? _edited.seq_max_mlayer_cnt_minus_1 : _original != null ? _original.seq_max_mlayer_cnt_minus_1 : this.seq_max_mlayer_cnt_minus_1);
					stream.WriteVariable(n, this.seq_max_mlayer_cnt_minus_1, "seq_max_mlayer_cnt_minus_1"); 
					SeqMaxMlayerCnt = (seq_max_mlayer_cnt_minus_1 + 1);
				}
				else 
				{
					SeqMaxMlayerCnt = 1;
				}
				this.monotonic_output_order_flag = stream.Pick("monotonic_output_order_flag", _original != null ? _original.monotonic_output_order_flag : this.monotonic_output_order_flag, _edited != null ? _edited.monotonic_output_order_flag : _original != null ? _original.monotonic_output_order_flag : this.monotonic_output_order_flag);
				stream.WriteFixed(1, this.monotonic_output_order_flag, "monotonic_output_order_flag"); 
			}
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
			this.seq_cropping_window_present_flag = stream.Pick("seq_cropping_window_present_flag", _original != null ? _original.seq_cropping_window_present_flag : this.seq_cropping_window_present_flag, _edited != null ? _edited.seq_cropping_window_present_flag : _original != null ? _original.seq_cropping_window_present_flag : this.seq_cropping_window_present_flag);
			stream.WriteFixed(1, this.seq_cropping_window_present_flag, "seq_cropping_window_present_flag"); 

			if ((seq_cropping_window_present_flag != 0))
			{
				this.seq_cropping_win_left_offset = stream.Pick("seq_cropping_win_left_offset", _original != null ? _original.seq_cropping_win_left_offset : this.seq_cropping_win_left_offset, _edited != null ? _edited.seq_cropping_win_left_offset : _original != null ? _original.seq_cropping_win_left_offset : this.seq_cropping_win_left_offset);
				stream.WriteUvlc( this.seq_cropping_win_left_offset, "seq_cropping_win_left_offset"); 
				this.seq_cropping_win_right_offset = stream.Pick("seq_cropping_win_right_offset", _original != null ? _original.seq_cropping_win_right_offset : this.seq_cropping_win_right_offset, _edited != null ? _edited.seq_cropping_win_right_offset : _original != null ? _original.seq_cropping_win_right_offset : this.seq_cropping_win_right_offset);
				stream.WriteUvlc( this.seq_cropping_win_right_offset, "seq_cropping_win_right_offset"); 
				this.seq_cropping_win_top_offset = stream.Pick("seq_cropping_win_top_offset", _original != null ? _original.seq_cropping_win_top_offset : this.seq_cropping_win_top_offset, _edited != null ? _edited.seq_cropping_win_top_offset : _original != null ? _original.seq_cropping_win_top_offset : this.seq_cropping_win_top_offset);
				stream.WriteUvlc( this.seq_cropping_win_top_offset, "seq_cropping_win_top_offset"); 
				this.seq_cropping_win_bottom_offset = stream.Pick("seq_cropping_win_bottom_offset", _original != null ? _original.seq_cropping_win_bottom_offset : this.seq_cropping_win_bottom_offset, _edited != null ? _edited.seq_cropping_win_bottom_offset : _original != null ? _original.seq_cropping_win_bottom_offset : this.seq_cropping_win_bottom_offset);
				stream.WriteUvlc( this.seq_cropping_win_bottom_offset, "seq_cropping_win_bottom_offset"); 
			}
			else 
			{
				seq_cropping_win_left_offset = 0;
				seq_cropping_win_right_offset = 0;
				seq_cropping_win_top_offset = 0;
				seq_cropping_win_bottom_offset = 0;
			}

			if ((single_picture_header_flag != 0))
			{
				decoder_model_info_present_flag = 0;
			}
			else 
			{
				this.seq_initial_display_delay_present_flag = stream.Pick("seq_initial_display_delay_present_flag", _original != null ? _original.seq_initial_display_delay_present_flag : this.seq_initial_display_delay_present_flag, _edited != null ? _edited.seq_initial_display_delay_present_flag : _original != null ? _original.seq_initial_display_delay_present_flag : this.seq_initial_display_delay_present_flag);
				stream.WriteFixed(1, this.seq_initial_display_delay_present_flag, "seq_initial_display_delay_present_flag"); 

				if ((seq_initial_display_delay_present_flag != 0))
				{
					this.seq_initial_display_delay_minus_1 = stream.Pick("seq_initial_display_delay_minus_1", _original != null ? _original.seq_initial_display_delay_minus_1 : this.seq_initial_display_delay_minus_1, _edited != null ? _edited.seq_initial_display_delay_minus_1 : _original != null ? _original.seq_initial_display_delay_minus_1 : this.seq_initial_display_delay_minus_1);
					stream.WriteFixed(4, this.seq_initial_display_delay_minus_1, "seq_initial_display_delay_minus_1"); 
				}
				this.decoder_model_info_present_flag = stream.Pick("decoder_model_info_present_flag", _original != null ? _original.decoder_model_info_present_flag : this.decoder_model_info_present_flag, _edited != null ? _edited.decoder_model_info_present_flag : _original != null ? _original.decoder_model_info_present_flag : this.decoder_model_info_present_flag);
				stream.WriteFixed(1, this.decoder_model_info_present_flag, "decoder_model_info_present_flag"); 

				if ((decoder_model_info_present_flag != 0))
				{
					this.num_units_in_decoding_tick = stream.Pick("num_units_in_decoding_tick", _original != null ? _original.num_units_in_decoding_tick : this.num_units_in_decoding_tick, _edited != null ? _edited.num_units_in_decoding_tick : _original != null ? _original.num_units_in_decoding_tick : this.num_units_in_decoding_tick);
					stream.WriteFixed(32, this.num_units_in_decoding_tick, "num_units_in_decoding_tick"); 
					this.seq_decoder_model_info_present_flag = stream.Pick("seq_decoder_model_info_present_flag", _original != null ? _original.seq_decoder_model_info_present_flag : this.seq_decoder_model_info_present_flag, _edited != null ? _edited.seq_decoder_model_info_present_flag : _original != null ? _original.seq_decoder_model_info_present_flag : this.seq_decoder_model_info_present_flag);
					stream.WriteFixed(1, this.seq_decoder_model_info_present_flag, "seq_decoder_model_info_present_flag"); 

					if ((seq_decoder_model_info_present_flag != 0))
					{
						WriteSeqDecoderModelInfo(); 
					}
				}
			}

			for (mLayer = 0; (mLayer < MAX_NUM_MLAYERS); mLayer++)
			{

				for (currTLayer = 0; (currTLayer < MAX_NUM_TLAYERS); currTLayer++)
				{

					for (refTLayer = 0; (refTLayer < MAX_NUM_TLAYERS); refTLayer++)
					{
						TLayerDependencyMap[mLayer][currTLayer][refTLayer] = ((((refTLayer <= currTLayer) && (currTLayer <= max_tlayer_id)) && (mLayer <= max_mlayer_id)) ? 1 : 0);
					}
				}
			}

			for (currLayer = 0; (currLayer < MAX_NUM_MLAYERS); currLayer++)
			{

				for (refLayer = 0; (refLayer < MAX_NUM_MLAYERS); refLayer++)
				{
					MLayerDependencyMap[currLayer][refLayer] = (((refLayer <= currLayer) && (currLayer <= max_mlayer_id)) ? 1 : 0);
				}
			}

			if ((max_mlayer_id > 0))
			{
				this.mlayer_dependency_present_flag = stream.Pick("mlayer_dependency_present_flag", _original != null ? _original.mlayer_dependency_present_flag : this.mlayer_dependency_present_flag, _edited != null ? _edited.mlayer_dependency_present_flag : _original != null ? _original.mlayer_dependency_present_flag : this.mlayer_dependency_present_flag);
				stream.WriteFixed(1, this.mlayer_dependency_present_flag, "mlayer_dependency_present_flag"); 

				if ((mlayer_dependency_present_flag != 0))
				{

					for (currLayer = 1; (currLayer <= max_mlayer_id); currLayer++)
					{

						for (refLayer = currLayer; (refLayer >= 0); refLayer--)
						{
							this.mlayer_dependency_map = stream.Pick("mlayer_dependency_map", _original != null ? _original.MLayerDependencyMap[currLayer][refLayer] : this.mlayer_dependency_map, _edited != null ? _edited.MLayerDependencyMap[currLayer][refLayer] : _original != null ? _original.MLayerDependencyMap[currLayer][refLayer] : this.mlayer_dependency_map);
							stream.WriteFixed(1, this.mlayer_dependency_map, "mlayer_dependency_map"); 
							MLayerDependencyMap[currLayer][refLayer] = mlayer_dependency_map;
						}
					}
				}
			}

			if ((max_tlayer_id > 0))
			{
				this.tlayer_dependency_present_flag = stream.Pick("tlayer_dependency_present_flag", _original != null ? _original.tlayer_dependency_present_flag : this.tlayer_dependency_present_flag, _edited != null ? _edited.tlayer_dependency_present_flag : _original != null ? _original.tlayer_dependency_present_flag : this.tlayer_dependency_present_flag);
				stream.WriteFixed(1, this.tlayer_dependency_present_flag, "tlayer_dependency_present_flag"); 

				if ((tlayer_dependency_present_flag != 0))
				{

					if ((max_mlayer_id > 0))
					{
						this.multi_tlayer_dependency_map_present_flag = stream.Pick("multi_tlayer_dependency_map_present_flag", _original != null ? _original.multi_tlayer_dependency_map_present_flag : this.multi_tlayer_dependency_map_present_flag, _edited != null ? _edited.multi_tlayer_dependency_map_present_flag : _original != null ? _original.multi_tlayer_dependency_map_present_flag : this.multi_tlayer_dependency_map_present_flag);
						stream.WriteFixed(1, this.multi_tlayer_dependency_map_present_flag, "multi_tlayer_dependency_map_present_flag"); 
					}
					else 
					{
						multi_tlayer_dependency_map_present_flag = 0;
					}

					for (mLayer = 0; (mLayer <= max_mlayer_id); mLayer++)
					{

						for (currTLayer = 1; (currTLayer <= max_tlayer_id); currTLayer++)
						{

							for (refTLayer = currTLayer; (refTLayer >= 0); refTLayer--)
							{

								if (((multi_tlayer_dependency_map_present_flag > 0) || (mLayer == 0)))
								{
									this.tlayer_dependency_map = stream.Pick("tlayer_dependency_map", _original != null ? _original.TLayerDependencyMap[mLayer][currTLayer][refTLayer] : this.tlayer_dependency_map, _edited != null ? _edited.TLayerDependencyMap[mLayer][currTLayer][refTLayer] : _original != null ? _original.TLayerDependencyMap[mLayer][currTLayer][refTLayer] : this.tlayer_dependency_map);
									stream.WriteFixed(1, this.tlayer_dependency_map, "tlayer_dependency_map"); 
									TLayerDependencyMap[mLayer][currTLayer][refTLayer] = tlayer_dependency_map;
								}
								else 
								{
									TLayerDependencyMap[mLayer][currTLayer][refTLayer] = TLayerDependencyMap[0][currTLayer][refTLayer];
								}
							}
						}
					}
				}
			}

			for (mlayerId = 0; (mlayerId < MAX_NUM_MLAYERS); mlayerId++)
			{

				for (refMlayer = 0; (refMlayer < MAX_NUM_MLAYERS); refMlayer++)
				{
					MLayerPresenceMap[mlayerId][refMlayer] = 0;

					if (((mlayerId == refMlayer) || (MLayerDependencyMap[mlayerId][refMlayer] != 0)))
					{
						MLayerPresenceMap[mlayerId][refMlayer] = 1;

						for (depMLayerId = 0; (depMLayerId < refMlayer); depMLayerId++)
						{
							MLayerPresenceMap[mlayerId][depMLayerId] |= MLayerPresenceMap[refMlayer][depMLayerId];
						}
					}
				}
			}
			WriteSequencePartitionConfig(); 
			WriteSequenceSegmentConfig(); 
			WriteSequenceIntraConfig(); 
			WriteSequenceInterConfig(); 
			WriteSequenceSccConfig(); 
			WriteSequenceTransformQuantEntropyConfig(); 
			WriteSequenceFilterConfig(); 
			WriteSequenceTileConfig(); 
			this.film_grain_params_present = stream.Pick("film_grain_params_present", _original != null ? _original.film_grain_params_present : this.film_grain_params_present, _edited != null ? _edited.film_grain_params_present : _original != null ? _original.film_grain_params_present : this.film_grain_params_present);
			stream.WriteFixed(1, this.film_grain_params_present, "film_grain_params_present"); 
			save_sequence_header(); 
        }

    /*
sequence_tile_config() {
seq_tile_info_present_flag	f(1)
if ( seq_tile_info_present_flag ) {	
allow_tile_info_change	f(1)
seqSbSize = get_seq_sb_size()	
( SeqSbRowStarts, SeqSbRows, SeqTileRows, SeqTileRowsLog2,	SeqSbColStarts, SeqSbCols, SeqTileCols, SeqTileColsLog2, SeqUniformTileSpacingFlag, sbShift) = tile_params(	max_frame_width_minus_1 + 1, max_frame_height_minus_1 + 1,	seqSbSize, seqSbSize, 0 )	
}	
}
    */
		private int seq_tile_info_present_flag;
		public int _SeqTileInfoPresentFlag { get { return seq_tile_info_present_flag; } set { seq_tile_info_present_flag = value; } }
		private int allow_tile_info_change;
		public int _AllowTileInfoChange { get { return allow_tile_info_change; } set { allow_tile_info_change = value; } }
		private AomArray<int> SeqSbRowStarts = new AomArray<int>();
		public AomArray<int> _SeqSbRowStarts { get { return SeqSbRowStarts; } set { SeqSbRowStarts = value; } }
		private int SeqSbRows;
		public int _SeqSbRows { get { return SeqSbRows; } set { SeqSbRows = value; } }
		private int SeqTileRows;
		public int _SeqTileRows { get { return SeqTileRows; } set { SeqTileRows = value; } }
		private int SeqTileRowsLog2;
		public int _SeqTileRowsLog2 { get { return SeqTileRowsLog2; } set { SeqTileRowsLog2 = value; } }
		private AomArray<int> SeqSbColStarts = new AomArray<int>();
		public AomArray<int> _SeqSbColStarts { get { return SeqSbColStarts; } set { SeqSbColStarts = value; } }
		private int SeqSbCols;
		public int _SeqSbCols { get { return SeqSbCols; } set { SeqSbCols = value; } }
		private int SeqTileCols;
		public int _SeqTileCols { get { return SeqTileCols; } set { SeqTileCols = value; } }
		private int SeqTileColsLog2;
		public int _SeqTileColsLog2 { get { return SeqTileColsLog2; } set { SeqTileColsLog2 = value; } }
		private int SeqUniformTileSpacingFlag;
		public int _SeqUniformTileSpacingFlag { get { return SeqUniformTileSpacingFlag; } set { SeqUniformTileSpacingFlag = value; } }

        private void SequenceTileConfig()
        {
			int seqSbSize = 0;
			int sbShift = 0;
			stream.ReadFixed(1, out this.seq_tile_info_present_flag, "seq_tile_info_present_flag"); 

			if ((seq_tile_info_present_flag != 0))
			{
				stream.ReadFixed(1, out this.allow_tile_info_change, "allow_tile_info_change"); 
				seqSbSize = GetSeqSbSize();
				(SeqSbRowStarts, SeqSbRows, SeqTileRows, SeqTileRowsLog2, SeqSbColStarts, SeqSbCols, SeqTileCols, SeqTileColsLog2, SeqUniformTileSpacingFlag, sbShift) = TileParams((max_frame_width_minus_1 + 1), (max_frame_height_minus_1 + 1), seqSbSize, seqSbSize, 0);
			}
        }

        private void WriteSequenceTileConfig()
        {
			int seqSbSize = 0;
			int sbShift = 0;
			this.seq_tile_info_present_flag = stream.Pick("seq_tile_info_present_flag", _original != null ? _original.seq_tile_info_present_flag : this.seq_tile_info_present_flag, _edited != null ? _edited.seq_tile_info_present_flag : _original != null ? _original.seq_tile_info_present_flag : this.seq_tile_info_present_flag);
			stream.WriteFixed(1, this.seq_tile_info_present_flag, "seq_tile_info_present_flag"); 

			if ((seq_tile_info_present_flag != 0))
			{
				this.allow_tile_info_change = stream.Pick("allow_tile_info_change", _original != null ? _original.allow_tile_info_change : this.allow_tile_info_change, _edited != null ? _edited.allow_tile_info_change : _original != null ? _original.allow_tile_info_change : this.allow_tile_info_change);
				stream.WriteFixed(1, this.allow_tile_info_change, "allow_tile_info_change"); 
				seqSbSize = GetSeqSbSize();
				(SeqSbRowStarts, SeqSbRows, SeqTileRows, SeqTileRowsLog2, SeqSbColStarts, SeqSbCols, SeqTileCols, SeqTileColsLog2, SeqUniformTileSpacingFlag, sbShift) = WriteTileParams((max_frame_width_minus_1 + 1), (max_frame_height_minus_1 + 1), seqSbSize, seqSbSize, 0);
			}
        }

    /*
sequence_partition_config() {
use_256x256_superblock	f(1)
if ( !use_256x256_superblock ) {	
use_128x128_superblock	f(1)
}	
if ( Monochrome ) {	
enable_sdp = 0	
} else {	
enable_sdp	f(1)
}	
if ( enable_sdp && !single_picture_header_flag ) {	
enable_extended_sdp	f(1)
} else {	
enable_extended_sdp = 0	
}	
enable_ext_partitions	f(1)
if ( enable_ext_partitions ) {	
enable_uneven_4way_partitions	f(1)
} else {	
enable_uneven_4way_partitions = 0	
}	
reduce_pb_aspect_ratio	f(1)
if ( reduce_pb_aspect_ratio ) {	
max_pb_aspect_ratio_log2_minus_1	f(1)
MaxPbAspectRatio = 1 << (max_pb_aspect_ratio_log2_minus_1 + 1)	
} else {	
MaxPbAspectRatio = 8	
}	
}
    */
		private int use_256x256_superblock;
		public int _Use256x256Superblock { get { return use_256x256_superblock; } set { use_256x256_superblock = value; } }
		private int use_128x128_superblock;
		public int _Use128x128Superblock { get { return use_128x128_superblock; } set { use_128x128_superblock = value; } }
		private int enable_sdp;
		public int _EnableSdp { get { return enable_sdp; } set { enable_sdp = value; } }
		private int enable_extended_sdp;
		public int _EnableExtendedSdp { get { return enable_extended_sdp; } set { enable_extended_sdp = value; } }
		private int enable_ext_partitions;
		public int _EnableExtPartitions { get { return enable_ext_partitions; } set { enable_ext_partitions = value; } }
		private int enable_uneven_4way_partitions;
		public int _EnableUneven4wayPartitions { get { return enable_uneven_4way_partitions; } set { enable_uneven_4way_partitions = value; } }
		private int reduce_pb_aspect_ratio;
		public int _ReducePbAspectRatio { get { return reduce_pb_aspect_ratio; } set { reduce_pb_aspect_ratio = value; } }
		private int max_pb_aspect_ratio_log2_minus_1;
		public int _MaxPbAspectRatioLog2Minus1 { get { return max_pb_aspect_ratio_log2_minus_1; } set { max_pb_aspect_ratio_log2_minus_1 = value; } }
		private int MaxPbAspectRatio;
		public int _MaxPbAspectRatio { get { return MaxPbAspectRatio; } set { MaxPbAspectRatio = value; } }

        private void SequencePartitionConfig()
        {
			stream.ReadFixed(1, out this.use_256x256_superblock, "use_256x256_superblock"); 

			if (!(use_256x256_superblock != 0))
			{
				stream.ReadFixed(1, out this.use_128x128_superblock, "use_128x128_superblock"); 
			}

			if ((Monochrome != 0))
			{
				enable_sdp = 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.enable_sdp, "enable_sdp"); 
			}

			if (((enable_sdp != 0) && !(single_picture_header_flag != 0)))
			{
				stream.ReadFixed(1, out this.enable_extended_sdp, "enable_extended_sdp"); 
			}
			else 
			{
				enable_extended_sdp = 0;
			}
			stream.ReadFixed(1, out this.enable_ext_partitions, "enable_ext_partitions"); 

			if ((enable_ext_partitions != 0))
			{
				stream.ReadFixed(1, out this.enable_uneven_4way_partitions, "enable_uneven_4way_partitions"); 
			}
			else 
			{
				enable_uneven_4way_partitions = 0;
			}
			stream.ReadFixed(1, out this.reduce_pb_aspect_ratio, "reduce_pb_aspect_ratio"); 

			if ((reduce_pb_aspect_ratio != 0))
			{
				stream.ReadFixed(1, out this.max_pb_aspect_ratio_log2_minus_1, "max_pb_aspect_ratio_log2_minus_1"); 
				MaxPbAspectRatio = (1 << (max_pb_aspect_ratio_log2_minus_1 + 1));
			}
			else 
			{
				MaxPbAspectRatio = 8;
			}
        }

        private void WriteSequencePartitionConfig()
        {
			this.use_256x256_superblock = stream.Pick("use_256x256_superblock", _original != null ? _original.use_256x256_superblock : this.use_256x256_superblock, _edited != null ? _edited.use_256x256_superblock : _original != null ? _original.use_256x256_superblock : this.use_256x256_superblock);
			stream.WriteFixed(1, this.use_256x256_superblock, "use_256x256_superblock"); 

			if (!(use_256x256_superblock != 0))
			{
				this.use_128x128_superblock = stream.Pick("use_128x128_superblock", _original != null ? _original.use_128x128_superblock : this.use_128x128_superblock, _edited != null ? _edited.use_128x128_superblock : _original != null ? _original.use_128x128_superblock : this.use_128x128_superblock);
				stream.WriteFixed(1, this.use_128x128_superblock, "use_128x128_superblock"); 
			}

			if ((Monochrome != 0))
			{
				enable_sdp = 0;
			}
			else 
			{
				this.enable_sdp = stream.Pick("enable_sdp", _original != null ? _original.enable_sdp : this.enable_sdp, _edited != null ? _edited.enable_sdp : _original != null ? _original.enable_sdp : this.enable_sdp);
				stream.WriteFixed(1, this.enable_sdp, "enable_sdp"); 
			}

			if (((enable_sdp != 0) && !(single_picture_header_flag != 0)))
			{
				this.enable_extended_sdp = stream.Pick("enable_extended_sdp", _original != null ? _original.enable_extended_sdp : this.enable_extended_sdp, _edited != null ? _edited.enable_extended_sdp : _original != null ? _original.enable_extended_sdp : this.enable_extended_sdp);
				stream.WriteFixed(1, this.enable_extended_sdp, "enable_extended_sdp"); 
			}
			else 
			{
				enable_extended_sdp = 0;
			}
			this.enable_ext_partitions = stream.Pick("enable_ext_partitions", _original != null ? _original.enable_ext_partitions : this.enable_ext_partitions, _edited != null ? _edited.enable_ext_partitions : _original != null ? _original.enable_ext_partitions : this.enable_ext_partitions);
			stream.WriteFixed(1, this.enable_ext_partitions, "enable_ext_partitions"); 

			if ((enable_ext_partitions != 0))
			{
				this.enable_uneven_4way_partitions = stream.Pick("enable_uneven_4way_partitions", _original != null ? _original.enable_uneven_4way_partitions : this.enable_uneven_4way_partitions, _edited != null ? _edited.enable_uneven_4way_partitions : _original != null ? _original.enable_uneven_4way_partitions : this.enable_uneven_4way_partitions);
				stream.WriteFixed(1, this.enable_uneven_4way_partitions, "enable_uneven_4way_partitions"); 
			}
			else 
			{
				enable_uneven_4way_partitions = 0;
			}
			this.reduce_pb_aspect_ratio = stream.Pick("reduce_pb_aspect_ratio", _original != null ? _original.reduce_pb_aspect_ratio : this.reduce_pb_aspect_ratio, _edited != null ? _edited.reduce_pb_aspect_ratio : _original != null ? _original.reduce_pb_aspect_ratio : this.reduce_pb_aspect_ratio);
			stream.WriteFixed(1, this.reduce_pb_aspect_ratio, "reduce_pb_aspect_ratio"); 

			if ((reduce_pb_aspect_ratio != 0))
			{
				this.max_pb_aspect_ratio_log2_minus_1 = stream.Pick("max_pb_aspect_ratio_log2_minus_1", _original != null ? _original.max_pb_aspect_ratio_log2_minus_1 : this.max_pb_aspect_ratio_log2_minus_1, _edited != null ? _edited.max_pb_aspect_ratio_log2_minus_1 : _original != null ? _original.max_pb_aspect_ratio_log2_minus_1 : this.max_pb_aspect_ratio_log2_minus_1);
				stream.WriteFixed(1, this.max_pb_aspect_ratio_log2_minus_1, "max_pb_aspect_ratio_log2_minus_1"); 
				MaxPbAspectRatio = (1 << (max_pb_aspect_ratio_log2_minus_1 + 1));
			}
			else 
			{
				MaxPbAspectRatio = 8;
			}
        }

    /*
sequence_segment_config() {
enable_ext_seg	f(1)
MaxSegments = enable_ext_seg ? 16 : 8	
seq_seg_info_present_flag	f(1)
if ( seq_seg_info_present_flag ) {	
seq_allow_seg_info_change	f(1)
( SeqFeatureEnabled, SeqFeatureData ) = seg_info( MaxSegments )	
}	
}
    */
		private int enable_ext_seg;
		public int _EnableExtSeg { get { return enable_ext_seg; } set { enable_ext_seg = value; } }
		private int MaxSegments;
		public int _MaxSegments { get { return MaxSegments; } set { MaxSegments = value; } }
		private int seq_seg_info_present_flag;
		public int _SeqSegInfoPresentFlag { get { return seq_seg_info_present_flag; } set { seq_seg_info_present_flag = value; } }
		private int seq_allow_seg_info_change;
		public int _SeqAllowSegInfoChange { get { return seq_allow_seg_info_change; } set { seq_allow_seg_info_change = value; } }
		private AomArray<AomArray<int>> SeqFeatureEnabled = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _SeqFeatureEnabled { get { return SeqFeatureEnabled; } set { SeqFeatureEnabled = value; } }
		private AomArray<AomArray<int>> SeqFeatureData = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _SeqFeatureData { get { return SeqFeatureData; } set { SeqFeatureData = value; } }

        private void SequenceSegmentConfig()
        {
			stream.ReadFixed(1, out this.enable_ext_seg, "enable_ext_seg"); 
			MaxSegments = ((enable_ext_seg != 0) ? 16 : 8);
			stream.ReadFixed(1, out this.seq_seg_info_present_flag, "seq_seg_info_present_flag"); 

			if ((seq_seg_info_present_flag != 0))
			{
				stream.ReadFixed(1, out this.seq_allow_seg_info_change, "seq_allow_seg_info_change"); 
				(SeqFeatureEnabled, SeqFeatureData) = SegInfo(MaxSegments);
			}
        }

        private void WriteSequenceSegmentConfig()
        {
			this.enable_ext_seg = stream.Pick("enable_ext_seg", _original != null ? _original.enable_ext_seg : this.enable_ext_seg, _edited != null ? _edited.enable_ext_seg : _original != null ? _original.enable_ext_seg : this.enable_ext_seg);
			stream.WriteFixed(1, this.enable_ext_seg, "enable_ext_seg"); 
			MaxSegments = ((enable_ext_seg != 0) ? 16 : 8);
			this.seq_seg_info_present_flag = stream.Pick("seq_seg_info_present_flag", _original != null ? _original.seq_seg_info_present_flag : this.seq_seg_info_present_flag, _edited != null ? _edited.seq_seg_info_present_flag : _original != null ? _original.seq_seg_info_present_flag : this.seq_seg_info_present_flag);
			stream.WriteFixed(1, this.seq_seg_info_present_flag, "seq_seg_info_present_flag"); 

			if ((seq_seg_info_present_flag != 0))
			{
				this.seq_allow_seg_info_change = stream.Pick("seq_allow_seg_info_change", _original != null ? _original.seq_allow_seg_info_change : this.seq_allow_seg_info_change, _edited != null ? _edited.seq_allow_seg_info_change : _original != null ? _original.seq_allow_seg_info_change : this.seq_allow_seg_info_change);
				stream.WriteFixed(1, this.seq_allow_seg_info_change, "seq_allow_seg_info_change"); 
				(SeqFeatureEnabled, SeqFeatureData) = WriteSegInfo(MaxSegments);
			}
        }

    /*
sequence_intra_config() {
enable_dip	f(1)
enable_intra_edge_filter	f(1)
enable_mrls	f(1)
enable_cfl_intra	f(1)
if ( Monochrome ) {	
cfl_ds_filter_index = 0	
} else {	
cfl_ds_filter_index	f(2)
}	
enable_mhccp	f(1)
enable_ibp	f(1)
}
    */
		private int enable_dip;
		public int _EnableDip { get { return enable_dip; } set { enable_dip = value; } }
		private int enable_intra_edge_filter;
		public int _EnableIntraEdgeFilter { get { return enable_intra_edge_filter; } set { enable_intra_edge_filter = value; } }
		private int enable_mrls;
		public int _EnableMrls { get { return enable_mrls; } set { enable_mrls = value; } }
		private int enable_cfl_intra;
		public int _EnableCflIntra { get { return enable_cfl_intra; } set { enable_cfl_intra = value; } }
		private int cfl_ds_filter_index;
		public int _CflDsFilterIndex { get { return cfl_ds_filter_index; } set { cfl_ds_filter_index = value; } }
		private int enable_mhccp;
		public int _EnableMhccp { get { return enable_mhccp; } set { enable_mhccp = value; } }
		private int enable_ibp;
		public int _EnableIbp { get { return enable_ibp; } set { enable_ibp = value; } }

        private void SequenceIntraConfig()
        {
			stream.ReadFixed(1, out this.enable_dip, "enable_dip"); 
			stream.ReadFixed(1, out this.enable_intra_edge_filter, "enable_intra_edge_filter"); 
			stream.ReadFixed(1, out this.enable_mrls, "enable_mrls"); 
			stream.ReadFixed(1, out this.enable_cfl_intra, "enable_cfl_intra"); 

			if ((Monochrome != 0))
			{
				cfl_ds_filter_index = 0;
			}
			else 
			{
				stream.ReadFixed(2, out this.cfl_ds_filter_index, "cfl_ds_filter_index"); 
			}
			stream.ReadFixed(1, out this.enable_mhccp, "enable_mhccp"); 
			stream.ReadFixed(1, out this.enable_ibp, "enable_ibp"); 
        }

        private void WriteSequenceIntraConfig()
        {
			this.enable_dip = stream.Pick("enable_dip", _original != null ? _original.enable_dip : this.enable_dip, _edited != null ? _edited.enable_dip : _original != null ? _original.enable_dip : this.enable_dip);
			stream.WriteFixed(1, this.enable_dip, "enable_dip"); 
			this.enable_intra_edge_filter = stream.Pick("enable_intra_edge_filter", _original != null ? _original.enable_intra_edge_filter : this.enable_intra_edge_filter, _edited != null ? _edited.enable_intra_edge_filter : _original != null ? _original.enable_intra_edge_filter : this.enable_intra_edge_filter);
			stream.WriteFixed(1, this.enable_intra_edge_filter, "enable_intra_edge_filter"); 
			this.enable_mrls = stream.Pick("enable_mrls", _original != null ? _original.enable_mrls : this.enable_mrls, _edited != null ? _edited.enable_mrls : _original != null ? _original.enable_mrls : this.enable_mrls);
			stream.WriteFixed(1, this.enable_mrls, "enable_mrls"); 
			this.enable_cfl_intra = stream.Pick("enable_cfl_intra", _original != null ? _original.enable_cfl_intra : this.enable_cfl_intra, _edited != null ? _edited.enable_cfl_intra : _original != null ? _original.enable_cfl_intra : this.enable_cfl_intra);
			stream.WriteFixed(1, this.enable_cfl_intra, "enable_cfl_intra"); 

			if ((Monochrome != 0))
			{
				cfl_ds_filter_index = 0;
			}
			else 
			{
				this.cfl_ds_filter_index = stream.Pick("cfl_ds_filter_index", _original != null ? _original.cfl_ds_filter_index : this.cfl_ds_filter_index, _edited != null ? _edited.cfl_ds_filter_index : _original != null ? _original.cfl_ds_filter_index : this.cfl_ds_filter_index);
				stream.WriteFixed(2, this.cfl_ds_filter_index, "cfl_ds_filter_index"); 
			}
			this.enable_mhccp = stream.Pick("enable_mhccp", _original != null ? _original.enable_mhccp : this.enable_mhccp, _edited != null ? _edited.enable_mhccp : _original != null ? _original.enable_mhccp : this.enable_mhccp);
			stream.WriteFixed(1, this.enable_mhccp, "enable_mhccp"); 
			this.enable_ibp = stream.Pick("enable_ibp", _original != null ? _original.enable_ibp : this.enable_ibp, _edited != null ? _edited.enable_ibp : _original != null ? _original.enable_ibp : this.enable_ibp);
			stream.WriteFixed(1, this.enable_ibp, "enable_ibp"); 
        }

    /*
sequence_inter_config() {
if ( single_picture_header_flag ) {	
for ( i = 0; i < MOTION_MODES; i++ ) {	
seq_enabled_motion_modes[ i ] = 0	
}	
enable_six_param_warp_delta = 0	
enable_masked_compound = 0	
enable_ref_frame_mvs = 0	
reduced_ref_frame_mvs_mode = 0	
OrderHintBits = 0	
enable_opfl_refine = REFINE_NONE	
enable_refmvbank	f(1)
disable_drl_reorder	f(1)
if ( disable_drl_reorder ) {	
DrlReorder = DRL_REORDER_DISABLED	
} else {	
constrain_drl_reorder	f(1)
DrlReorder = constrain_drl_reorder ?	
DRL_REORDER_CONSTRAINT : DRL_REORDER_ALWAYS	
}	
n = MAX_REF_BV_STACK_SIZE - 1	
seq_max_bvp_drl_bits_minus_1	ns(n)
allow_frame_max_bvp_drl_bits	f(1)
enable_bawp	f(1)
enable_mv_traj = 0	
enable_imp_msk_bld = 0	
NumRefFrames = 2	
long_term_frame_id_bits = 0	
} else {	
motionModeEnabled = 0	
for ( mode = INTERINTRA; mode < MOTION_MODES; mode++ ) {	
seq_enabled_motion_modes[ mode ]	f(1)
motionModeEnabled |= seq_enabled_motion_modes[ mode ]	
}	
if ( motionModeEnabled ) {	
seq_frame_motion_modes_present_flag	f(1)
} else {	
seq_frame_motion_modes_present_flag = 0	
}	
if ( seq_enabled_motion_modes[ DELTAWARP ] ) {	
enable_six_param_warp_delta	f(1)
} else {	
enable_six_param_warp_delta = 0	
}	
enable_masked_compound	f(1)
enable_ref_frame_mvs	f(1)
if ( enable_ref_frame_mvs ) {	
reduced_ref_frame_mvs_mode	f(1)
} else {	
reduced_ref_frame_mvs_mode = 0	
}	
order_hint_bits_minus_1	f(4)
OrderHintBits = order_hint_bits_minus_1 + 1	
enable_refmvbank	f(1)
disable_drl_reorder	f(1)
if ( disable_drl_reorder ) {	
DrlReorder = DRL_REORDER_DISABLED	
} else {	
constrain_drl_reorder	f(1)
DrlReorder = constrain_drl_reorder ? DRL_REORDER_CONSTRAINT :	
DRL_REORDER_ALWAYS	
}	
explicit_ref_frame_map	f(1)
explicit_num_ref_frames	f(1)
if ( explicit_num_ref_frames ) {	
num_ref_frames_minus_1	f(4)
NumRefFrames = num_ref_frames_minus_1 + 1	
} else {	
NumRefFrames = 8	
}	
ActiveNumRefFrames = Min( REFS_PER_FRAME, NumRefFrames )	
long_term_frame_id_bits	f(3)
n = MAX_REF_MV_STACK_SIZE - 1	
seq_max_drl_bits_minus_1	ns(n)
allow_frame_max_drl_bits	f(1)
n = MAX_REF_BV_STACK_SIZE - 1	
seq_max_bvp_drl_bits_minus_1	ns(n)
allow_frame_max_bvp_drl_bits	f(1)
num_same_ref_compound	f(2)
enable_tip	f(1)
if ( enable_tip ) {	
disable_tip_output	f(1)
EnableTipOutput = !disable_tip_output	
enable_tip_hole_fill	f(1)
} else {	
enable_tip_hole_fill = 0	
EnableTipOutput = 0	
}	
enable_mv_traj	f(1)
enable_bawp	f(1)
enable_cwp	f(1)
enable_imp_msk_bld	f(1)
enable_df_sub_pu	f(1)
if ( EnableTipOutput && enable_df_sub_pu ) {	
enable_tip_explicit_qp	f(1)
} else {	
enable_tip_explicit_qp = 0	
}	
enable_opfl_refine	f(2)
enable_refinemv	f(1)
if ( enable_tip && ( enable_opfl_refine != 0 || enable_refinemv ) ) {	
enable_tip_refinemv	f(1)
} else {	
enable_tip_refinemv = 0	
}	
enable_bru	f(1)
enable_adaptive_mvd	f(1)
enable_mvd_sign_derive	f(1)
enable_flex_mvres	f(1)
if ( single_picture_header_flag ) {	
enable_global_motion = 0	
} else {	
enable_global_motion	f(1)
}	
enable_short_refresh_frame_flags	f(1)
}	
}
    */
		private AomArray<int> seq_enabled_motion_modes = new AomArray<int>();
		public AomArray<int> _SeqEnabledMotionModes { get { return seq_enabled_motion_modes; } set { seq_enabled_motion_modes = value; } }
		private int enable_six_param_warp_delta;
		public int _EnableSixParamWarpDelta { get { return enable_six_param_warp_delta; } set { enable_six_param_warp_delta = value; } }
		private int enable_masked_compound;
		public int _EnableMaskedCompound { get { return enable_masked_compound; } set { enable_masked_compound = value; } }
		private int enable_ref_frame_mvs;
		public int _EnableRefFrameMvs { get { return enable_ref_frame_mvs; } set { enable_ref_frame_mvs = value; } }
		private int reduced_ref_frame_mvs_mode;
		public int _ReducedRefFrameMvsMode { get { return reduced_ref_frame_mvs_mode; } set { reduced_ref_frame_mvs_mode = value; } }
		private int OrderHintBits;
		public int _OrderHintBits { get { return OrderHintBits; } set { OrderHintBits = value; } }
		private int enable_opfl_refine;
		public int _EnableOpflRefine { get { return enable_opfl_refine; } set { enable_opfl_refine = value; } }
		private int enable_refmvbank;
		public int _EnableRefmvbank { get { return enable_refmvbank; } set { enable_refmvbank = value; } }
		private int disable_drl_reorder;
		public int _DisableDrlReorder { get { return disable_drl_reorder; } set { disable_drl_reorder = value; } }
		private int DrlReorder;
		public int _DrlReorder { get { return DrlReorder; } set { DrlReorder = value; } }
		private int constrain_drl_reorder;
		public int _ConstrainDrlReorder { get { return constrain_drl_reorder; } set { constrain_drl_reorder = value; } }
		private int seq_max_bvp_drl_bits_minus_1;
		public int _SeqMaxBvpDrlBitsMinus1 { get { return seq_max_bvp_drl_bits_minus_1; } set { seq_max_bvp_drl_bits_minus_1 = value; } }
		private int allow_frame_max_bvp_drl_bits;
		public int _AllowFrameMaxBvpDrlBits { get { return allow_frame_max_bvp_drl_bits; } set { allow_frame_max_bvp_drl_bits = value; } }
		private int enable_bawp;
		public int _EnableBawp { get { return enable_bawp; } set { enable_bawp = value; } }
		private int enable_mv_traj;
		public int _EnableMvTraj { get { return enable_mv_traj; } set { enable_mv_traj = value; } }
		private int enable_imp_msk_bld;
		public int _EnableImpMskBld { get { return enable_imp_msk_bld; } set { enable_imp_msk_bld = value; } }
		private int NumRefFrames;
		public int _NumRefFrames { get { return NumRefFrames; } set { NumRefFrames = value; } }
		private int long_term_frame_id_bits;
		public int _LongTermFrameIdBits { get { return long_term_frame_id_bits; } set { long_term_frame_id_bits = value; } }
		private int seq_frame_motion_modes_present_flag;
		public int _SeqFrameMotionModesPresentFlag { get { return seq_frame_motion_modes_present_flag; } set { seq_frame_motion_modes_present_flag = value; } }
		private int order_hint_bits_minus_1;
		public int _OrderHintBitsMinus1 { get { return order_hint_bits_minus_1; } set { order_hint_bits_minus_1 = value; } }
		private int explicit_ref_frame_map;
		public int _ExplicitRefFrameMap { get { return explicit_ref_frame_map; } set { explicit_ref_frame_map = value; } }
		private int explicit_num_ref_frames;
		public int _ExplicitNumRefFrames { get { return explicit_num_ref_frames; } set { explicit_num_ref_frames = value; } }
		private int num_ref_frames_minus_1;
		public int _NumRefFramesMinus1 { get { return num_ref_frames_minus_1; } set { num_ref_frames_minus_1 = value; } }
		private int ActiveNumRefFrames;
		public int _ActiveNumRefFrames { get { return ActiveNumRefFrames; } set { ActiveNumRefFrames = value; } }
		private int seq_max_drl_bits_minus_1;
		public int _SeqMaxDrlBitsMinus1 { get { return seq_max_drl_bits_minus_1; } set { seq_max_drl_bits_minus_1 = value; } }
		private int allow_frame_max_drl_bits;
		public int _AllowFrameMaxDrlBits { get { return allow_frame_max_drl_bits; } set { allow_frame_max_drl_bits = value; } }
		private int num_same_ref_compound;
		public int _NumSameRefCompound { get { return num_same_ref_compound; } set { num_same_ref_compound = value; } }
		private int enable_tip;
		public int _EnableTip { get { return enable_tip; } set { enable_tip = value; } }
		private int disable_tip_output;
		public int _DisableTipOutput { get { return disable_tip_output; } set { disable_tip_output = value; } }
		private int EnableTipOutput;
		public int _EnableTipOutput { get { return EnableTipOutput; } set { EnableTipOutput = value; } }
		private int enable_tip_hole_fill;
		public int _EnableTipHoleFill { get { return enable_tip_hole_fill; } set { enable_tip_hole_fill = value; } }
		private int enable_cwp;
		public int _EnableCwp { get { return enable_cwp; } set { enable_cwp = value; } }
		private int enable_df_sub_pu;
		public int _EnableDfSubPu { get { return enable_df_sub_pu; } set { enable_df_sub_pu = value; } }
		private int enable_tip_explicit_qp;
		public int _EnableTipExplicitQp { get { return enable_tip_explicit_qp; } set { enable_tip_explicit_qp = value; } }
		private int enable_refinemv;
		public int _EnableRefinemv { get { return enable_refinemv; } set { enable_refinemv = value; } }
		private int enable_tip_refinemv;
		public int _EnableTipRefinemv { get { return enable_tip_refinemv; } set { enable_tip_refinemv = value; } }
		private int enable_bru;
		public int _EnableBru { get { return enable_bru; } set { enable_bru = value; } }
		private int enable_adaptive_mvd;
		public int _EnableAdaptiveMvd { get { return enable_adaptive_mvd; } set { enable_adaptive_mvd = value; } }
		private int enable_mvd_sign_derive;
		public int _EnableMvdSignDerive { get { return enable_mvd_sign_derive; } set { enable_mvd_sign_derive = value; } }
		private int enable_flex_mvres;
		public int _EnableFlexMvres { get { return enable_flex_mvres; } set { enable_flex_mvres = value; } }
		private int enable_global_motion;
		public int _EnableGlobalMotion { get { return enable_global_motion; } set { enable_global_motion = value; } }
		private int enable_short_refresh_frame_flags;
		public int _EnableShortRefreshFrameFlags { get { return enable_short_refresh_frame_flags; } set { enable_short_refresh_frame_flags = value; } }
		private int mode = 0;

        private void SequenceInterConfig()
        {
			int i = 0;
			int mode = 0;
			int n = 0;
			int motionModeEnabled = 0;

			if ((single_picture_header_flag != 0))
			{

				for (i = 0; (i < MOTION_MODES); i++)
				{
					seq_enabled_motion_modes[i] = 0;
				}
				enable_six_param_warp_delta = 0;
				enable_masked_compound = 0;
				enable_ref_frame_mvs = 0;
				reduced_ref_frame_mvs_mode = 0;
				OrderHintBits = 0;
				enable_opfl_refine = REFINE_NONE;
				stream.ReadFixed(1, out this.enable_refmvbank, "enable_refmvbank"); 
				stream.ReadFixed(1, out this.disable_drl_reorder, "disable_drl_reorder"); 

				if ((disable_drl_reorder != 0))
				{
					DrlReorder = DRL_REORDER_DISABLED;
				}
				else 
				{
					stream.ReadFixed(1, out this.constrain_drl_reorder, "constrain_drl_reorder"); 
					DrlReorder = ((constrain_drl_reorder != 0) ? DRL_REORDER_CONSTRAINT : DRL_REORDER_ALWAYS);
				}
				n = (MAX_REF_BV_STACK_SIZE - 1);
				stream.Read_ns(n, out this.seq_max_bvp_drl_bits_minus_1, "seq_max_bvp_drl_bits_minus_1"); 
				stream.ReadFixed(1, out this.allow_frame_max_bvp_drl_bits, "allow_frame_max_bvp_drl_bits"); 
				stream.ReadFixed(1, out this.enable_bawp, "enable_bawp"); 
				enable_mv_traj = 0;
				enable_imp_msk_bld = 0;
				NumRefFrames = 2;
				long_term_frame_id_bits = 0;
			}
			else 
			{
				motionModeEnabled = 0;

				for (mode = INTERINTRA; (mode < MOTION_MODES); mode++)
				{
					stream.ReadFixed(1, out this.seq_enabled_motion_modes[mode], "seq_enabled_motion_modes"); 
					motionModeEnabled |= seq_enabled_motion_modes[mode];
				}

				if ((motionModeEnabled != 0))
				{
					stream.ReadFixed(1, out this.seq_frame_motion_modes_present_flag, "seq_frame_motion_modes_present_flag"); 
				}
				else 
				{
					seq_frame_motion_modes_present_flag = 0;
				}

				if ((seq_enabled_motion_modes[DELTAWARP] != 0))
				{
					stream.ReadFixed(1, out this.enable_six_param_warp_delta, "enable_six_param_warp_delta"); 
				}
				else 
				{
					enable_six_param_warp_delta = 0;
				}
				stream.ReadFixed(1, out this.enable_masked_compound, "enable_masked_compound"); 
				stream.ReadFixed(1, out this.enable_ref_frame_mvs, "enable_ref_frame_mvs"); 

				if ((enable_ref_frame_mvs != 0))
				{
					stream.ReadFixed(1, out this.reduced_ref_frame_mvs_mode, "reduced_ref_frame_mvs_mode"); 
				}
				else 
				{
					reduced_ref_frame_mvs_mode = 0;
				}
				stream.ReadFixed(4, out this.order_hint_bits_minus_1, "order_hint_bits_minus_1"); 
				OrderHintBits = (order_hint_bits_minus_1 + 1);
				stream.ReadFixed(1, out this.enable_refmvbank, "enable_refmvbank"); 
				stream.ReadFixed(1, out this.disable_drl_reorder, "disable_drl_reorder"); 

				if ((disable_drl_reorder != 0))
				{
					DrlReorder = DRL_REORDER_DISABLED;
				}
				else 
				{
					stream.ReadFixed(1, out this.constrain_drl_reorder, "constrain_drl_reorder"); 
					DrlReorder = ((constrain_drl_reorder != 0) ? DRL_REORDER_CONSTRAINT : DRL_REORDER_ALWAYS);
				}
				stream.ReadFixed(1, out this.explicit_ref_frame_map, "explicit_ref_frame_map"); 
				stream.ReadFixed(1, out this.explicit_num_ref_frames, "explicit_num_ref_frames"); 

				if ((explicit_num_ref_frames != 0))
				{
					stream.ReadFixed(4, out this.num_ref_frames_minus_1, "num_ref_frames_minus_1"); 
					NumRefFrames = (num_ref_frames_minus_1 + 1);
				}
				else 
				{
					NumRefFrames = 8;
				}
				ActiveNumRefFrames = Min(REFS_PER_FRAME, NumRefFrames);
				stream.ReadFixed(3, out this.long_term_frame_id_bits, "long_term_frame_id_bits"); 
				n = (MAX_REF_MV_STACK_SIZE - 1);
				stream.Read_ns(n, out this.seq_max_drl_bits_minus_1, "seq_max_drl_bits_minus_1"); 
				stream.ReadFixed(1, out this.allow_frame_max_drl_bits, "allow_frame_max_drl_bits"); 
				n = (MAX_REF_BV_STACK_SIZE - 1);
				stream.Read_ns(n, out this.seq_max_bvp_drl_bits_minus_1, "seq_max_bvp_drl_bits_minus_1"); 
				stream.ReadFixed(1, out this.allow_frame_max_bvp_drl_bits, "allow_frame_max_bvp_drl_bits"); 
				stream.ReadFixed(2, out this.num_same_ref_compound, "num_same_ref_compound"); 
				stream.ReadFixed(1, out this.enable_tip, "enable_tip"); 

				if ((enable_tip != 0))
				{
					stream.ReadFixed(1, out this.disable_tip_output, "disable_tip_output"); 
					EnableTipOutput = (!(disable_tip_output != 0) ? 1 : 0);
					stream.ReadFixed(1, out this.enable_tip_hole_fill, "enable_tip_hole_fill"); 
				}
				else 
				{
					enable_tip_hole_fill = 0;
					EnableTipOutput = 0;
				}
				stream.ReadFixed(1, out this.enable_mv_traj, "enable_mv_traj"); 
				stream.ReadFixed(1, out this.enable_bawp, "enable_bawp"); 
				stream.ReadFixed(1, out this.enable_cwp, "enable_cwp"); 
				stream.ReadFixed(1, out this.enable_imp_msk_bld, "enable_imp_msk_bld"); 
				stream.ReadFixed(1, out this.enable_df_sub_pu, "enable_df_sub_pu"); 

				if (((EnableTipOutput != 0) && (enable_df_sub_pu != 0)))
				{
					stream.ReadFixed(1, out this.enable_tip_explicit_qp, "enable_tip_explicit_qp"); 
				}
				else 
				{
					enable_tip_explicit_qp = 0;
				}
				stream.ReadFixed(2, out this.enable_opfl_refine, "enable_opfl_refine"); 
				stream.ReadFixed(1, out this.enable_refinemv, "enable_refinemv"); 

				if (((enable_tip != 0) && ((enable_opfl_refine != 0) || (enable_refinemv != 0))))
				{
					stream.ReadFixed(1, out this.enable_tip_refinemv, "enable_tip_refinemv"); 
				}
				else 
				{
					enable_tip_refinemv = 0;
				}
				stream.ReadFixed(1, out this.enable_bru, "enable_bru"); 
				stream.ReadFixed(1, out this.enable_adaptive_mvd, "enable_adaptive_mvd"); 
				stream.ReadFixed(1, out this.enable_mvd_sign_derive, "enable_mvd_sign_derive"); 
				stream.ReadFixed(1, out this.enable_flex_mvres, "enable_flex_mvres"); 

				if ((single_picture_header_flag != 0))
				{
					enable_global_motion = 0;
				}
				else 
				{
					stream.ReadFixed(1, out this.enable_global_motion, "enable_global_motion"); 
				}
				stream.ReadFixed(1, out this.enable_short_refresh_frame_flags, "enable_short_refresh_frame_flags"); 
			}
        }

        private void WriteSequenceInterConfig()
        {
			int i = 0;
			int mode = 0;
			int n = 0;
			int motionModeEnabled = 0;

			if ((single_picture_header_flag != 0))
			{

				for (i = 0; (i < MOTION_MODES); i++)
				{
					seq_enabled_motion_modes[i] = 0;
				}
				enable_six_param_warp_delta = 0;
				enable_masked_compound = 0;
				enable_ref_frame_mvs = 0;
				reduced_ref_frame_mvs_mode = 0;
				OrderHintBits = 0;
				enable_opfl_refine = REFINE_NONE;
				this.enable_refmvbank = stream.Pick("enable_refmvbank", _original != null ? _original.enable_refmvbank : this.enable_refmvbank, _edited != null ? _edited.enable_refmvbank : _original != null ? _original.enable_refmvbank : this.enable_refmvbank);
				stream.WriteFixed(1, this.enable_refmvbank, "enable_refmvbank"); 
				this.disable_drl_reorder = stream.Pick("disable_drl_reorder", _original != null ? _original.disable_drl_reorder : this.disable_drl_reorder, _edited != null ? _edited.disable_drl_reorder : _original != null ? _original.disable_drl_reorder : this.disable_drl_reorder);
				stream.WriteFixed(1, this.disable_drl_reorder, "disable_drl_reorder"); 

				if ((disable_drl_reorder != 0))
				{
					DrlReorder = DRL_REORDER_DISABLED;
				}
				else 
				{
					this.constrain_drl_reorder = stream.Pick("constrain_drl_reorder", _original != null ? _original.constrain_drl_reorder : this.constrain_drl_reorder, _edited != null ? _edited.constrain_drl_reorder : _original != null ? _original.constrain_drl_reorder : this.constrain_drl_reorder);
					stream.WriteFixed(1, this.constrain_drl_reorder, "constrain_drl_reorder"); 
					DrlReorder = ((constrain_drl_reorder != 0) ? DRL_REORDER_CONSTRAINT : DRL_REORDER_ALWAYS);
				}
				n = (MAX_REF_BV_STACK_SIZE - 1);
				this.seq_max_bvp_drl_bits_minus_1 = stream.Pick("seq_max_bvp_drl_bits_minus_1", _original != null ? _original.seq_max_bvp_drl_bits_minus_1 : this.seq_max_bvp_drl_bits_minus_1, _edited != null ? _edited.seq_max_bvp_drl_bits_minus_1 : _original != null ? _original.seq_max_bvp_drl_bits_minus_1 : this.seq_max_bvp_drl_bits_minus_1);
				stream.Write_ns(n, this.seq_max_bvp_drl_bits_minus_1, "seq_max_bvp_drl_bits_minus_1"); 
				this.allow_frame_max_bvp_drl_bits = stream.Pick("allow_frame_max_bvp_drl_bits", _original != null ? _original.allow_frame_max_bvp_drl_bits : this.allow_frame_max_bvp_drl_bits, _edited != null ? _edited.allow_frame_max_bvp_drl_bits : _original != null ? _original.allow_frame_max_bvp_drl_bits : this.allow_frame_max_bvp_drl_bits);
				stream.WriteFixed(1, this.allow_frame_max_bvp_drl_bits, "allow_frame_max_bvp_drl_bits"); 
				this.enable_bawp = stream.Pick("enable_bawp", _original != null ? _original.enable_bawp : this.enable_bawp, _edited != null ? _edited.enable_bawp : _original != null ? _original.enable_bawp : this.enable_bawp);
				stream.WriteFixed(1, this.enable_bawp, "enable_bawp"); 
				enable_mv_traj = 0;
				enable_imp_msk_bld = 0;
				NumRefFrames = 2;
				long_term_frame_id_bits = 0;
			}
			else 
			{
				motionModeEnabled = 0;

				for (mode = INTERINTRA; (mode < MOTION_MODES); mode++)
				{
					this.seq_enabled_motion_modes[mode] = stream.Pick("seq_enabled_motion_modes", _original != null ? _original.seq_enabled_motion_modes[mode] : this.seq_enabled_motion_modes[mode], _edited != null ? _edited.seq_enabled_motion_modes[mode] : _original != null ? _original.seq_enabled_motion_modes[mode] : this.seq_enabled_motion_modes[mode]);
					stream.WriteFixed(1, this.seq_enabled_motion_modes[mode], "seq_enabled_motion_modes"); 
					motionModeEnabled |= seq_enabled_motion_modes[mode];
				}

				if ((motionModeEnabled != 0))
				{
					this.seq_frame_motion_modes_present_flag = stream.Pick("seq_frame_motion_modes_present_flag", _original != null ? _original.seq_frame_motion_modes_present_flag : this.seq_frame_motion_modes_present_flag, _edited != null ? _edited.seq_frame_motion_modes_present_flag : _original != null ? _original.seq_frame_motion_modes_present_flag : this.seq_frame_motion_modes_present_flag);
					stream.WriteFixed(1, this.seq_frame_motion_modes_present_flag, "seq_frame_motion_modes_present_flag"); 
				}
				else 
				{
					seq_frame_motion_modes_present_flag = 0;
				}

				if ((seq_enabled_motion_modes[DELTAWARP] != 0))
				{
					this.enable_six_param_warp_delta = stream.Pick("enable_six_param_warp_delta", _original != null ? _original.enable_six_param_warp_delta : this.enable_six_param_warp_delta, _edited != null ? _edited.enable_six_param_warp_delta : _original != null ? _original.enable_six_param_warp_delta : this.enable_six_param_warp_delta);
					stream.WriteFixed(1, this.enable_six_param_warp_delta, "enable_six_param_warp_delta"); 
				}
				else 
				{
					enable_six_param_warp_delta = 0;
				}
				this.enable_masked_compound = stream.Pick("enable_masked_compound", _original != null ? _original.enable_masked_compound : this.enable_masked_compound, _edited != null ? _edited.enable_masked_compound : _original != null ? _original.enable_masked_compound : this.enable_masked_compound);
				stream.WriteFixed(1, this.enable_masked_compound, "enable_masked_compound"); 
				this.enable_ref_frame_mvs = stream.Pick("enable_ref_frame_mvs", _original != null ? _original.enable_ref_frame_mvs : this.enable_ref_frame_mvs, _edited != null ? _edited.enable_ref_frame_mvs : _original != null ? _original.enable_ref_frame_mvs : this.enable_ref_frame_mvs);
				stream.WriteFixed(1, this.enable_ref_frame_mvs, "enable_ref_frame_mvs"); 

				if ((enable_ref_frame_mvs != 0))
				{
					this.reduced_ref_frame_mvs_mode = stream.Pick("reduced_ref_frame_mvs_mode", _original != null ? _original.reduced_ref_frame_mvs_mode : this.reduced_ref_frame_mvs_mode, _edited != null ? _edited.reduced_ref_frame_mvs_mode : _original != null ? _original.reduced_ref_frame_mvs_mode : this.reduced_ref_frame_mvs_mode);
					stream.WriteFixed(1, this.reduced_ref_frame_mvs_mode, "reduced_ref_frame_mvs_mode"); 
				}
				else 
				{
					reduced_ref_frame_mvs_mode = 0;
				}
				this.order_hint_bits_minus_1 = stream.Pick("order_hint_bits_minus_1", _original != null ? _original.order_hint_bits_minus_1 : this.order_hint_bits_minus_1, _edited != null ? _edited.order_hint_bits_minus_1 : _original != null ? _original.order_hint_bits_minus_1 : this.order_hint_bits_minus_1);
				stream.WriteFixed(4, this.order_hint_bits_minus_1, "order_hint_bits_minus_1"); 
				OrderHintBits = (order_hint_bits_minus_1 + 1);
				this.enable_refmvbank = stream.Pick("enable_refmvbank", _original != null ? _original.enable_refmvbank : this.enable_refmvbank, _edited != null ? _edited.enable_refmvbank : _original != null ? _original.enable_refmvbank : this.enable_refmvbank);
				stream.WriteFixed(1, this.enable_refmvbank, "enable_refmvbank"); 
				this.disable_drl_reorder = stream.Pick("disable_drl_reorder", _original != null ? _original.disable_drl_reorder : this.disable_drl_reorder, _edited != null ? _edited.disable_drl_reorder : _original != null ? _original.disable_drl_reorder : this.disable_drl_reorder);
				stream.WriteFixed(1, this.disable_drl_reorder, "disable_drl_reorder"); 

				if ((disable_drl_reorder != 0))
				{
					DrlReorder = DRL_REORDER_DISABLED;
				}
				else 
				{
					this.constrain_drl_reorder = stream.Pick("constrain_drl_reorder", _original != null ? _original.constrain_drl_reorder : this.constrain_drl_reorder, _edited != null ? _edited.constrain_drl_reorder : _original != null ? _original.constrain_drl_reorder : this.constrain_drl_reorder);
					stream.WriteFixed(1, this.constrain_drl_reorder, "constrain_drl_reorder"); 
					DrlReorder = ((constrain_drl_reorder != 0) ? DRL_REORDER_CONSTRAINT : DRL_REORDER_ALWAYS);
				}
				this.explicit_ref_frame_map = stream.Pick("explicit_ref_frame_map", _original != null ? _original.explicit_ref_frame_map : this.explicit_ref_frame_map, _edited != null ? _edited.explicit_ref_frame_map : _original != null ? _original.explicit_ref_frame_map : this.explicit_ref_frame_map);
				stream.WriteFixed(1, this.explicit_ref_frame_map, "explicit_ref_frame_map"); 
				this.explicit_num_ref_frames = stream.Pick("explicit_num_ref_frames", _original != null ? _original.explicit_num_ref_frames : this.explicit_num_ref_frames, _edited != null ? _edited.explicit_num_ref_frames : _original != null ? _original.explicit_num_ref_frames : this.explicit_num_ref_frames);
				stream.WriteFixed(1, this.explicit_num_ref_frames, "explicit_num_ref_frames"); 

				if ((explicit_num_ref_frames != 0))
				{
					this.num_ref_frames_minus_1 = stream.Pick("num_ref_frames_minus_1", _original != null ? _original.num_ref_frames_minus_1 : this.num_ref_frames_minus_1, _edited != null ? _edited.num_ref_frames_minus_1 : _original != null ? _original.num_ref_frames_minus_1 : this.num_ref_frames_minus_1);
					stream.WriteFixed(4, this.num_ref_frames_minus_1, "num_ref_frames_minus_1"); 
					NumRefFrames = (num_ref_frames_minus_1 + 1);
				}
				else 
				{
					NumRefFrames = 8;
				}
				ActiveNumRefFrames = Min(REFS_PER_FRAME, NumRefFrames);
				this.long_term_frame_id_bits = stream.Pick("long_term_frame_id_bits", _original != null ? _original.long_term_frame_id_bits : this.long_term_frame_id_bits, _edited != null ? _edited.long_term_frame_id_bits : _original != null ? _original.long_term_frame_id_bits : this.long_term_frame_id_bits);
				stream.WriteFixed(3, this.long_term_frame_id_bits, "long_term_frame_id_bits"); 
				n = (MAX_REF_MV_STACK_SIZE - 1);
				this.seq_max_drl_bits_minus_1 = stream.Pick("seq_max_drl_bits_minus_1", _original != null ? _original.seq_max_drl_bits_minus_1 : this.seq_max_drl_bits_minus_1, _edited != null ? _edited.seq_max_drl_bits_minus_1 : _original != null ? _original.seq_max_drl_bits_minus_1 : this.seq_max_drl_bits_minus_1);
				stream.Write_ns(n, this.seq_max_drl_bits_minus_1, "seq_max_drl_bits_minus_1"); 
				this.allow_frame_max_drl_bits = stream.Pick("allow_frame_max_drl_bits", _original != null ? _original.allow_frame_max_drl_bits : this.allow_frame_max_drl_bits, _edited != null ? _edited.allow_frame_max_drl_bits : _original != null ? _original.allow_frame_max_drl_bits : this.allow_frame_max_drl_bits);
				stream.WriteFixed(1, this.allow_frame_max_drl_bits, "allow_frame_max_drl_bits"); 
				n = (MAX_REF_BV_STACK_SIZE - 1);
				this.seq_max_bvp_drl_bits_minus_1 = stream.Pick("seq_max_bvp_drl_bits_minus_1", _original != null ? _original.seq_max_bvp_drl_bits_minus_1 : this.seq_max_bvp_drl_bits_minus_1, _edited != null ? _edited.seq_max_bvp_drl_bits_minus_1 : _original != null ? _original.seq_max_bvp_drl_bits_minus_1 : this.seq_max_bvp_drl_bits_minus_1);
				stream.Write_ns(n, this.seq_max_bvp_drl_bits_minus_1, "seq_max_bvp_drl_bits_minus_1"); 
				this.allow_frame_max_bvp_drl_bits = stream.Pick("allow_frame_max_bvp_drl_bits", _original != null ? _original.allow_frame_max_bvp_drl_bits : this.allow_frame_max_bvp_drl_bits, _edited != null ? _edited.allow_frame_max_bvp_drl_bits : _original != null ? _original.allow_frame_max_bvp_drl_bits : this.allow_frame_max_bvp_drl_bits);
				stream.WriteFixed(1, this.allow_frame_max_bvp_drl_bits, "allow_frame_max_bvp_drl_bits"); 
				this.num_same_ref_compound = stream.Pick("num_same_ref_compound", _original != null ? _original.num_same_ref_compound : this.num_same_ref_compound, _edited != null ? _edited.num_same_ref_compound : _original != null ? _original.num_same_ref_compound : this.num_same_ref_compound);
				stream.WriteFixed(2, this.num_same_ref_compound, "num_same_ref_compound"); 
				this.enable_tip = stream.Pick("enable_tip", _original != null ? _original.enable_tip : this.enable_tip, _edited != null ? _edited.enable_tip : _original != null ? _original.enable_tip : this.enable_tip);
				stream.WriteFixed(1, this.enable_tip, "enable_tip"); 

				if ((enable_tip != 0))
				{
					this.disable_tip_output = stream.Pick("disable_tip_output", _original != null ? _original.disable_tip_output : this.disable_tip_output, _edited != null ? _edited.disable_tip_output : _original != null ? _original.disable_tip_output : this.disable_tip_output);
					stream.WriteFixed(1, this.disable_tip_output, "disable_tip_output"); 
					EnableTipOutput = (!(disable_tip_output != 0) ? 1 : 0);
					this.enable_tip_hole_fill = stream.Pick("enable_tip_hole_fill", _original != null ? _original.enable_tip_hole_fill : this.enable_tip_hole_fill, _edited != null ? _edited.enable_tip_hole_fill : _original != null ? _original.enable_tip_hole_fill : this.enable_tip_hole_fill);
					stream.WriteFixed(1, this.enable_tip_hole_fill, "enable_tip_hole_fill"); 
				}
				else 
				{
					enable_tip_hole_fill = 0;
					EnableTipOutput = 0;
				}
				this.enable_mv_traj = stream.Pick("enable_mv_traj", _original != null ? _original.enable_mv_traj : this.enable_mv_traj, _edited != null ? _edited.enable_mv_traj : _original != null ? _original.enable_mv_traj : this.enable_mv_traj);
				stream.WriteFixed(1, this.enable_mv_traj, "enable_mv_traj"); 
				this.enable_bawp = stream.Pick("enable_bawp", _original != null ? _original.enable_bawp : this.enable_bawp, _edited != null ? _edited.enable_bawp : _original != null ? _original.enable_bawp : this.enable_bawp);
				stream.WriteFixed(1, this.enable_bawp, "enable_bawp"); 
				this.enable_cwp = stream.Pick("enable_cwp", _original != null ? _original.enable_cwp : this.enable_cwp, _edited != null ? _edited.enable_cwp : _original != null ? _original.enable_cwp : this.enable_cwp);
				stream.WriteFixed(1, this.enable_cwp, "enable_cwp"); 
				this.enable_imp_msk_bld = stream.Pick("enable_imp_msk_bld", _original != null ? _original.enable_imp_msk_bld : this.enable_imp_msk_bld, _edited != null ? _edited.enable_imp_msk_bld : _original != null ? _original.enable_imp_msk_bld : this.enable_imp_msk_bld);
				stream.WriteFixed(1, this.enable_imp_msk_bld, "enable_imp_msk_bld"); 
				this.enable_df_sub_pu = stream.Pick("enable_df_sub_pu", _original != null ? _original.enable_df_sub_pu : this.enable_df_sub_pu, _edited != null ? _edited.enable_df_sub_pu : _original != null ? _original.enable_df_sub_pu : this.enable_df_sub_pu);
				stream.WriteFixed(1, this.enable_df_sub_pu, "enable_df_sub_pu"); 

				if (((EnableTipOutput != 0) && (enable_df_sub_pu != 0)))
				{
					this.enable_tip_explicit_qp = stream.Pick("enable_tip_explicit_qp", _original != null ? _original.enable_tip_explicit_qp : this.enable_tip_explicit_qp, _edited != null ? _edited.enable_tip_explicit_qp : _original != null ? _original.enable_tip_explicit_qp : this.enable_tip_explicit_qp);
					stream.WriteFixed(1, this.enable_tip_explicit_qp, "enable_tip_explicit_qp"); 
				}
				else 
				{
					enable_tip_explicit_qp = 0;
				}
				this.enable_opfl_refine = stream.Pick("enable_opfl_refine", _original != null ? _original.enable_opfl_refine : this.enable_opfl_refine, _edited != null ? _edited.enable_opfl_refine : _original != null ? _original.enable_opfl_refine : this.enable_opfl_refine);
				stream.WriteFixed(2, this.enable_opfl_refine, "enable_opfl_refine"); 
				this.enable_refinemv = stream.Pick("enable_refinemv", _original != null ? _original.enable_refinemv : this.enable_refinemv, _edited != null ? _edited.enable_refinemv : _original != null ? _original.enable_refinemv : this.enable_refinemv);
				stream.WriteFixed(1, this.enable_refinemv, "enable_refinemv"); 

				if (((enable_tip != 0) && ((enable_opfl_refine != 0) || (enable_refinemv != 0))))
				{
					this.enable_tip_refinemv = stream.Pick("enable_tip_refinemv", _original != null ? _original.enable_tip_refinemv : this.enable_tip_refinemv, _edited != null ? _edited.enable_tip_refinemv : _original != null ? _original.enable_tip_refinemv : this.enable_tip_refinemv);
					stream.WriteFixed(1, this.enable_tip_refinemv, "enable_tip_refinemv"); 
				}
				else 
				{
					enable_tip_refinemv = 0;
				}
				this.enable_bru = stream.Pick("enable_bru", _original != null ? _original.enable_bru : this.enable_bru, _edited != null ? _edited.enable_bru : _original != null ? _original.enable_bru : this.enable_bru);
				stream.WriteFixed(1, this.enable_bru, "enable_bru"); 
				this.enable_adaptive_mvd = stream.Pick("enable_adaptive_mvd", _original != null ? _original.enable_adaptive_mvd : this.enable_adaptive_mvd, _edited != null ? _edited.enable_adaptive_mvd : _original != null ? _original.enable_adaptive_mvd : this.enable_adaptive_mvd);
				stream.WriteFixed(1, this.enable_adaptive_mvd, "enable_adaptive_mvd"); 
				this.enable_mvd_sign_derive = stream.Pick("enable_mvd_sign_derive", _original != null ? _original.enable_mvd_sign_derive : this.enable_mvd_sign_derive, _edited != null ? _edited.enable_mvd_sign_derive : _original != null ? _original.enable_mvd_sign_derive : this.enable_mvd_sign_derive);
				stream.WriteFixed(1, this.enable_mvd_sign_derive, "enable_mvd_sign_derive"); 
				this.enable_flex_mvres = stream.Pick("enable_flex_mvres", _original != null ? _original.enable_flex_mvres : this.enable_flex_mvres, _edited != null ? _edited.enable_flex_mvres : _original != null ? _original.enable_flex_mvres : this.enable_flex_mvres);
				stream.WriteFixed(1, this.enable_flex_mvres, "enable_flex_mvres"); 

				if ((single_picture_header_flag != 0))
				{
					enable_global_motion = 0;
				}
				else 
				{
					this.enable_global_motion = stream.Pick("enable_global_motion", _original != null ? _original.enable_global_motion : this.enable_global_motion, _edited != null ? _edited.enable_global_motion : _original != null ? _original.enable_global_motion : this.enable_global_motion);
					stream.WriteFixed(1, this.enable_global_motion, "enable_global_motion"); 
				}
				this.enable_short_refresh_frame_flags = stream.Pick("enable_short_refresh_frame_flags", _original != null ? _original.enable_short_refresh_frame_flags : this.enable_short_refresh_frame_flags, _edited != null ? _edited.enable_short_refresh_frame_flags : _original != null ? _original.enable_short_refresh_frame_flags : this.enable_short_refresh_frame_flags);
				stream.WriteFixed(1, this.enable_short_refresh_frame_flags, "enable_short_refresh_frame_flags"); 
			}
        }

    /*
sequence_scc_config() {
if ( single_picture_header_flag ) {	
seq_force_screen_content_tools = SELECT_SCREEN_CONTENT_TOOLS	
seq_force_integer_mv = SELECT_INTEGER_MV	
} else {	
seq_choose_screen_content_tools	f(1)
if ( seq_choose_screen_content_tools ) {	
seq_force_screen_content_tools = SELECT_SCREEN_CONTENT_TOOLS	
} else {	
seq_force_screen_content_tools	f(1)
}	
if ( seq_force_screen_content_tools > 0 ) {	
seq_choose_integer_mv	f(1)
if ( seq_choose_integer_mv ) {	
seq_force_integer_mv = SELECT_INTEGER_MV	
} else {	
seq_force_integer_mv	f(1)
}	
} else {	
seq_force_integer_mv = SELECT_INTEGER_MV	
}	
}	
}
    */
		private int seq_force_screen_content_tools;
		public int _SeqForceScreenContentTools { get { return seq_force_screen_content_tools; } set { seq_force_screen_content_tools = value; } }
		private int seq_force_integer_mv;
		public int _SeqForceIntegerMv { get { return seq_force_integer_mv; } set { seq_force_integer_mv = value; } }
		private int seq_choose_screen_content_tools;
		public int _SeqChooseScreenContentTools { get { return seq_choose_screen_content_tools; } set { seq_choose_screen_content_tools = value; } }
		private int seq_choose_integer_mv;
		public int _SeqChooseIntegerMv { get { return seq_choose_integer_mv; } set { seq_choose_integer_mv = value; } }

        private void SequenceSccConfig()
        {

			if ((single_picture_header_flag != 0))
			{
				seq_force_screen_content_tools = SELECT_SCREEN_CONTENT_TOOLS;
				seq_force_integer_mv = SELECT_INTEGER_MV;
			}
			else 
			{
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
			}
        }

        private void WriteSequenceSccConfig()
        {

			if ((single_picture_header_flag != 0))
			{
				seq_force_screen_content_tools = SELECT_SCREEN_CONTENT_TOOLS;
				seq_force_integer_mv = SELECT_INTEGER_MV;
			}
			else 
			{
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
			}
        }

    /*
sequence_transform_quant_entropy_config() {
enable_fsc	f(1)
if ( enable_fsc ) {	
enable_idtx_intra = 1	
} else {	
enable_idtx_intra	f(1)
}	
enable_intra_ist	f(1)
enable_inter_ist	f(1)
if ( Monochrome ) {	
enable_chroma_dctonly = 0	
} else {	
enable_chroma_dctonly	f(1)
}	
if ( !single_picture_header_flag ) {	
enable_inter_ddt	f(1)
}	
reduced_tx_part_set	f(1)
if ( Monochrome ) {	
enable_cctx = 0	
} else {	
enable_cctx	f(1)
}	
enable_tcq	f(1)
if ( enable_tcq && !single_picture_header_flag ) {	
choose_tcq_per_frame	f(1)
} else {	
choose_tcq_per_frame = 0	
}	
if ( enable_tcq && !choose_tcq_per_frame ) {	
enable_parity_hiding = 0	
} else {	
enable_parity_hiding	f(1)
}	
if ( single_picture_header_flag ) {	
enable_avg_cdf = 1	
avg_cdf_type = 1	
} else {	
enable_avg_cdf	f(1)
if ( enable_avg_cdf ) {	
avg_cdf_type	f(1)
}	
}	
if ( Monochrome ) {	
separate_uv_delta_q = 0	
} else {	
separate_uv_delta_q	f(1)
}	
BaseYDcDeltaQ = 0	
BaseUVDcDeltaQ = 0	
BaseUVAcDeltaQ = 0	
y_dc_delta_q_enabled = 0	
uv_dc_delta_q_enabled = 0	
uv_ac_delta_q_enabled = 0	
equal_ac_dc_q	f(1)
if ( !equal_ac_dc_q ) {	
base_y_dc_delta_q	f(5)
BaseYDcDeltaQ = DELTA_DCQUANT_MIN + base_y_dc_delta_q	
y_dc_delta_q_enabled	f(1)
}	
if ( !Monochrome ) {	
if ( !equal_ac_dc_q ) {	
base_uv_dc_delta_q	f(5)
BaseUVDcDeltaQ = DELTA_DCQUANT_MIN + base_uv_dc_delta_q	
uv_dc_delta_q_enabled	f(1)
}	
base_uv_ac_delta_q	f(5)
BaseUVAcDeltaQ = DELTA_DCQUANT_MIN + base_uv_ac_delta_q	
uv_ac_delta_q_enabled	f(1)
if ( equal_ac_dc_q ) {	
BaseUVDcDeltaQ = BaseUVAcDeltaQ	
}	
}	
}
    */
		private int enable_fsc;
		public int _EnableFsc { get { return enable_fsc; } set { enable_fsc = value; } }
		private int enable_idtx_intra;
		public int _EnableIdtxIntra { get { return enable_idtx_intra; } set { enable_idtx_intra = value; } }
		private int enable_intra_ist;
		public int _EnableIntraIst { get { return enable_intra_ist; } set { enable_intra_ist = value; } }
		private int enable_inter_ist;
		public int _EnableInterIst { get { return enable_inter_ist; } set { enable_inter_ist = value; } }
		private int enable_chroma_dctonly;
		public int _EnableChromaDctonly { get { return enable_chroma_dctonly; } set { enable_chroma_dctonly = value; } }
		private int enable_inter_ddt;
		public int _EnableInterDdt { get { return enable_inter_ddt; } set { enable_inter_ddt = value; } }
		private int reduced_tx_part_set;
		public int _ReducedTxPartSet { get { return reduced_tx_part_set; } set { reduced_tx_part_set = value; } }
		private int enable_cctx;
		public int _EnableCctx { get { return enable_cctx; } set { enable_cctx = value; } }
		private int enable_tcq;
		public int _EnableTcq { get { return enable_tcq; } set { enable_tcq = value; } }
		private int choose_tcq_per_frame;
		public int _ChooseTcqPerFrame { get { return choose_tcq_per_frame; } set { choose_tcq_per_frame = value; } }
		private int enable_parity_hiding;
		public int _EnableParityHiding { get { return enable_parity_hiding; } set { enable_parity_hiding = value; } }
		private int enable_avg_cdf;
		public int _EnableAvgCdf { get { return enable_avg_cdf; } set { enable_avg_cdf = value; } }
		private int avg_cdf_type;
		public int _AvgCdfType { get { return avg_cdf_type; } set { avg_cdf_type = value; } }
		private int separate_uv_delta_q;
		public int _SeparateUvDeltaq { get { return separate_uv_delta_q; } set { separate_uv_delta_q = value; } }
		private int BaseYDcDeltaQ;
		public int _BaseYDcDeltaQ { get { return BaseYDcDeltaQ; } set { BaseYDcDeltaQ = value; } }
		private int BaseUVDcDeltaQ;
		public int _BaseUVDcDeltaQ { get { return BaseUVDcDeltaQ; } set { BaseUVDcDeltaQ = value; } }
		private int BaseUVAcDeltaQ;
		public int _BaseUVAcDeltaQ { get { return BaseUVAcDeltaQ; } set { BaseUVAcDeltaQ = value; } }
		private int y_dc_delta_q_enabled;
		public int _yDcDeltaqEnabled { get { return y_dc_delta_q_enabled; } set { y_dc_delta_q_enabled = value; } }
		private int uv_dc_delta_q_enabled;
		public int _UvDcDeltaqEnabled { get { return uv_dc_delta_q_enabled; } set { uv_dc_delta_q_enabled = value; } }
		private int uv_ac_delta_q_enabled;
		public int _UvAcDeltaqEnabled { get { return uv_ac_delta_q_enabled; } set { uv_ac_delta_q_enabled = value; } }
		private int equal_ac_dc_q;
		public int _EqualAcDcq { get { return equal_ac_dc_q; } set { equal_ac_dc_q = value; } }
		private int base_y_dc_delta_q;
		public int _BaseyDcDeltaq { get { return base_y_dc_delta_q; } set { base_y_dc_delta_q = value; } }
		private int base_uv_dc_delta_q;
		public int _BaseUvDcDeltaq { get { return base_uv_dc_delta_q; } set { base_uv_dc_delta_q = value; } }
		private int base_uv_ac_delta_q;
		public int _BaseUvAcDeltaq { get { return base_uv_ac_delta_q; } set { base_uv_ac_delta_q = value; } }

        private void SequenceTransformQuantEntropyConfig()
        {
			stream.ReadFixed(1, out this.enable_fsc, "enable_fsc"); 

			if ((enable_fsc != 0))
			{
				enable_idtx_intra = 1;
			}
			else 
			{
				stream.ReadFixed(1, out this.enable_idtx_intra, "enable_idtx_intra"); 
			}
			stream.ReadFixed(1, out this.enable_intra_ist, "enable_intra_ist"); 
			stream.ReadFixed(1, out this.enable_inter_ist, "enable_inter_ist"); 

			if ((Monochrome != 0))
			{
				enable_chroma_dctonly = 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.enable_chroma_dctonly, "enable_chroma_dctonly"); 
			}

			if (!(single_picture_header_flag != 0))
			{
				stream.ReadFixed(1, out this.enable_inter_ddt, "enable_inter_ddt"); 
			}
			stream.ReadFixed(1, out this.reduced_tx_part_set, "reduced_tx_part_set"); 

			if ((Monochrome != 0))
			{
				enable_cctx = 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.enable_cctx, "enable_cctx"); 
			}
			stream.ReadFixed(1, out this.enable_tcq, "enable_tcq"); 

			if (((enable_tcq != 0) && !(single_picture_header_flag != 0)))
			{
				stream.ReadFixed(1, out this.choose_tcq_per_frame, "choose_tcq_per_frame"); 
			}
			else 
			{
				choose_tcq_per_frame = 0;
			}

			if (((enable_tcq != 0) && !(choose_tcq_per_frame != 0)))
			{
				enable_parity_hiding = 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.enable_parity_hiding, "enable_parity_hiding"); 
			}

			if ((single_picture_header_flag != 0))
			{
				enable_avg_cdf = 1;
				avg_cdf_type = 1;
			}
			else 
			{
				stream.ReadFixed(1, out this.enable_avg_cdf, "enable_avg_cdf"); 

				if ((enable_avg_cdf != 0))
				{
					stream.ReadFixed(1, out this.avg_cdf_type, "avg_cdf_type"); 
				}
			}

			if ((Monochrome != 0))
			{
				separate_uv_delta_q = 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.separate_uv_delta_q, "separate_uv_delta_q"); 
			}
			BaseYDcDeltaQ = 0;
			BaseUVDcDeltaQ = 0;
			BaseUVAcDeltaQ = 0;
			y_dc_delta_q_enabled = 0;
			uv_dc_delta_q_enabled = 0;
			uv_ac_delta_q_enabled = 0;
			stream.ReadFixed(1, out this.equal_ac_dc_q, "equal_ac_dc_q"); 

			if (!(equal_ac_dc_q != 0))
			{
				stream.ReadFixed(5, out this.base_y_dc_delta_q, "base_y_dc_delta_q"); 
				BaseYDcDeltaQ = (DELTA_DCQUANT_MIN + base_y_dc_delta_q);
				stream.ReadFixed(1, out this.y_dc_delta_q_enabled, "y_dc_delta_q_enabled"); 
			}

			if (!(Monochrome != 0))
			{

				if (!(equal_ac_dc_q != 0))
				{
					stream.ReadFixed(5, out this.base_uv_dc_delta_q, "base_uv_dc_delta_q"); 
					BaseUVDcDeltaQ = (DELTA_DCQUANT_MIN + base_uv_dc_delta_q);
					stream.ReadFixed(1, out this.uv_dc_delta_q_enabled, "uv_dc_delta_q_enabled"); 
				}
				stream.ReadFixed(5, out this.base_uv_ac_delta_q, "base_uv_ac_delta_q"); 
				BaseUVAcDeltaQ = (DELTA_DCQUANT_MIN + base_uv_ac_delta_q);
				stream.ReadFixed(1, out this.uv_ac_delta_q_enabled, "uv_ac_delta_q_enabled"); 

				if ((equal_ac_dc_q != 0))
				{
					BaseUVDcDeltaQ = BaseUVAcDeltaQ;
				}
			}
        }

        private void WriteSequenceTransformQuantEntropyConfig()
        {
			this.enable_fsc = stream.Pick("enable_fsc", _original != null ? (_original.enable_idtx_intra == 1 ? 1 : 0) : this.enable_fsc, _edited != null ? (_edited.enable_idtx_intra == 1 ? 1 : 0) : _original != null ? (_original.enable_idtx_intra == 1 ? 1 : 0) : this.enable_fsc);
			stream.WriteFixed(1, this.enable_fsc, "enable_fsc"); 

			if ((enable_fsc != 0))
			{
				enable_idtx_intra = 1;
			}
			else 
			{
				this.enable_idtx_intra = stream.Pick("enable_idtx_intra", _original != null ? _original.enable_idtx_intra : this.enable_idtx_intra, _edited != null ? _edited.enable_idtx_intra : _original != null ? _original.enable_idtx_intra : this.enable_idtx_intra);
				stream.WriteFixed(1, this.enable_idtx_intra, "enable_idtx_intra"); 
			}
			this.enable_intra_ist = stream.Pick("enable_intra_ist", _original != null ? _original.enable_intra_ist : this.enable_intra_ist, _edited != null ? _edited.enable_intra_ist : _original != null ? _original.enable_intra_ist : this.enable_intra_ist);
			stream.WriteFixed(1, this.enable_intra_ist, "enable_intra_ist"); 
			this.enable_inter_ist = stream.Pick("enable_inter_ist", _original != null ? _original.enable_inter_ist : this.enable_inter_ist, _edited != null ? _edited.enable_inter_ist : _original != null ? _original.enable_inter_ist : this.enable_inter_ist);
			stream.WriteFixed(1, this.enable_inter_ist, "enable_inter_ist"); 

			if ((Monochrome != 0))
			{
				enable_chroma_dctonly = 0;
			}
			else 
			{
				this.enable_chroma_dctonly = stream.Pick("enable_chroma_dctonly", _original != null ? _original.enable_chroma_dctonly : this.enable_chroma_dctonly, _edited != null ? _edited.enable_chroma_dctonly : _original != null ? _original.enable_chroma_dctonly : this.enable_chroma_dctonly);
				stream.WriteFixed(1, this.enable_chroma_dctonly, "enable_chroma_dctonly"); 
			}

			if (!(single_picture_header_flag != 0))
			{
				this.enable_inter_ddt = stream.Pick("enable_inter_ddt", _original != null ? _original.enable_inter_ddt : this.enable_inter_ddt, _edited != null ? _edited.enable_inter_ddt : _original != null ? _original.enable_inter_ddt : this.enable_inter_ddt);
				stream.WriteFixed(1, this.enable_inter_ddt, "enable_inter_ddt"); 
			}
			this.reduced_tx_part_set = stream.Pick("reduced_tx_part_set", _original != null ? _original.reduced_tx_part_set : this.reduced_tx_part_set, _edited != null ? _edited.reduced_tx_part_set : _original != null ? _original.reduced_tx_part_set : this.reduced_tx_part_set);
			stream.WriteFixed(1, this.reduced_tx_part_set, "reduced_tx_part_set"); 

			if ((Monochrome != 0))
			{
				enable_cctx = 0;
			}
			else 
			{
				this.enable_cctx = stream.Pick("enable_cctx", _original != null ? _original.enable_cctx : this.enable_cctx, _edited != null ? _edited.enable_cctx : _original != null ? _original.enable_cctx : this.enable_cctx);
				stream.WriteFixed(1, this.enable_cctx, "enable_cctx"); 
			}
			this.enable_tcq = stream.Pick("enable_tcq", _original != null ? _original.enable_tcq : this.enable_tcq, _edited != null ? _edited.enable_tcq : _original != null ? _original.enable_tcq : this.enable_tcq);
			stream.WriteFixed(1, this.enable_tcq, "enable_tcq"); 

			if (((enable_tcq != 0) && !(single_picture_header_flag != 0)))
			{
				this.choose_tcq_per_frame = stream.Pick("choose_tcq_per_frame", _original != null ? _original.choose_tcq_per_frame : this.choose_tcq_per_frame, _edited != null ? _edited.choose_tcq_per_frame : _original != null ? _original.choose_tcq_per_frame : this.choose_tcq_per_frame);
				stream.WriteFixed(1, this.choose_tcq_per_frame, "choose_tcq_per_frame"); 
			}
			else 
			{
				choose_tcq_per_frame = 0;
			}

			if (((enable_tcq != 0) && !(choose_tcq_per_frame != 0)))
			{
				enable_parity_hiding = 0;
			}
			else 
			{
				this.enable_parity_hiding = stream.Pick("enable_parity_hiding", _original != null ? _original.enable_parity_hiding : this.enable_parity_hiding, _edited != null ? _edited.enable_parity_hiding : _original != null ? _original.enable_parity_hiding : this.enable_parity_hiding);
				stream.WriteFixed(1, this.enable_parity_hiding, "enable_parity_hiding"); 
			}

			if ((single_picture_header_flag != 0))
			{
				enable_avg_cdf = 1;
				avg_cdf_type = 1;
			}
			else 
			{
				this.enable_avg_cdf = stream.Pick("enable_avg_cdf", _original != null ? _original.enable_avg_cdf : this.enable_avg_cdf, _edited != null ? _edited.enable_avg_cdf : _original != null ? _original.enable_avg_cdf : this.enable_avg_cdf);
				stream.WriteFixed(1, this.enable_avg_cdf, "enable_avg_cdf"); 

				if ((enable_avg_cdf != 0))
				{
					this.avg_cdf_type = stream.Pick("avg_cdf_type", _original != null ? _original.avg_cdf_type : this.avg_cdf_type, _edited != null ? _edited.avg_cdf_type : _original != null ? _original.avg_cdf_type : this.avg_cdf_type);
					stream.WriteFixed(1, this.avg_cdf_type, "avg_cdf_type"); 
				}
			}

			if ((Monochrome != 0))
			{
				separate_uv_delta_q = 0;
			}
			else 
			{
				this.separate_uv_delta_q = stream.Pick("separate_uv_delta_q", _original != null ? _original.separate_uv_delta_q : this.separate_uv_delta_q, _edited != null ? _edited.separate_uv_delta_q : _original != null ? _original.separate_uv_delta_q : this.separate_uv_delta_q);
				stream.WriteFixed(1, this.separate_uv_delta_q, "separate_uv_delta_q"); 
			}
			BaseYDcDeltaQ = 0;
			BaseUVDcDeltaQ = 0;
			BaseUVAcDeltaQ = 0;
			y_dc_delta_q_enabled = 0;
			uv_dc_delta_q_enabled = 0;
			uv_ac_delta_q_enabled = 0;
			this.equal_ac_dc_q = stream.Pick("equal_ac_dc_q", _original != null ? _original.equal_ac_dc_q : this.equal_ac_dc_q, _edited != null ? _edited.equal_ac_dc_q : _original != null ? _original.equal_ac_dc_q : this.equal_ac_dc_q);
			stream.WriteFixed(1, this.equal_ac_dc_q, "equal_ac_dc_q"); 

			if (!(equal_ac_dc_q != 0))
			{
				this.base_y_dc_delta_q = stream.Pick("base_y_dc_delta_q", _original != null ? _original.base_y_dc_delta_q : this.base_y_dc_delta_q, _edited != null ? _edited.base_y_dc_delta_q : _original != null ? _original.base_y_dc_delta_q : this.base_y_dc_delta_q);
				stream.WriteFixed(5, this.base_y_dc_delta_q, "base_y_dc_delta_q"); 
				BaseYDcDeltaQ = (DELTA_DCQUANT_MIN + base_y_dc_delta_q);
				this.y_dc_delta_q_enabled = stream.Pick("y_dc_delta_q_enabled", _original != null ? _original.y_dc_delta_q_enabled : this.y_dc_delta_q_enabled, _edited != null ? _edited.y_dc_delta_q_enabled : _original != null ? _original.y_dc_delta_q_enabled : this.y_dc_delta_q_enabled);
				stream.WriteFixed(1, this.y_dc_delta_q_enabled, "y_dc_delta_q_enabled"); 
			}

			if (!(Monochrome != 0))
			{

				if (!(equal_ac_dc_q != 0))
				{
					this.base_uv_dc_delta_q = stream.Pick("base_uv_dc_delta_q", _original != null ? _original.base_uv_dc_delta_q : this.base_uv_dc_delta_q, _edited != null ? _edited.base_uv_dc_delta_q : _original != null ? _original.base_uv_dc_delta_q : this.base_uv_dc_delta_q);
					stream.WriteFixed(5, this.base_uv_dc_delta_q, "base_uv_dc_delta_q"); 
					BaseUVDcDeltaQ = (DELTA_DCQUANT_MIN + base_uv_dc_delta_q);
					this.uv_dc_delta_q_enabled = stream.Pick("uv_dc_delta_q_enabled", _original != null ? _original.uv_dc_delta_q_enabled : this.uv_dc_delta_q_enabled, _edited != null ? _edited.uv_dc_delta_q_enabled : _original != null ? _original.uv_dc_delta_q_enabled : this.uv_dc_delta_q_enabled);
					stream.WriteFixed(1, this.uv_dc_delta_q_enabled, "uv_dc_delta_q_enabled"); 
				}
				this.base_uv_ac_delta_q = stream.Pick("base_uv_ac_delta_q", _original != null ? _original.base_uv_ac_delta_q : this.base_uv_ac_delta_q, _edited != null ? _edited.base_uv_ac_delta_q : _original != null ? _original.base_uv_ac_delta_q : this.base_uv_ac_delta_q);
				stream.WriteFixed(5, this.base_uv_ac_delta_q, "base_uv_ac_delta_q"); 
				BaseUVAcDeltaQ = (DELTA_DCQUANT_MIN + base_uv_ac_delta_q);
				this.uv_ac_delta_q_enabled = stream.Pick("uv_ac_delta_q_enabled", _original != null ? _original.uv_ac_delta_q_enabled : this.uv_ac_delta_q_enabled, _edited != null ? _edited.uv_ac_delta_q_enabled : _original != null ? _original.uv_ac_delta_q_enabled : this.uv_ac_delta_q_enabled);
				stream.WriteFixed(1, this.uv_ac_delta_q_enabled, "uv_ac_delta_q_enabled"); 

				if ((equal_ac_dc_q != 0))
				{
					BaseUVDcDeltaQ = BaseUVAcDeltaQ;
				}
			}
        }

    /*
seg_info( numSegments ) {
for ( i = 0; i < numSegments; i++ ) {	
for ( j = 0; j < SEG_LVL_MAX; j++ ) {	
feature_enabled	f(1)
enabled[ i ][ j ] = feature_enabled	
clippedValue = 0	
if ( feature_enabled == 1 ) {	
bitsToRead = Segmentation_Feature_Bits[ j ]	
limit = Segmentation_Feature_Max[ j ]	
if ( Segmentation_Feature_Signed[ j ] == 1 ) {	
n = 1 + bitsToRead	
feature_value	su(n)
clippedValue = Clip3( -limit, limit, feature_value)	
} else {	
feature_value	f(bitsToRead)
clippedValue = Clip3( 0, limit, feature_value)	
}	
}	
data[ i ][ j ] = clippedValue	
}	
}	
return (enabled, data)	
}
    */
		private int numSegments;
		public int _NumSegments { get { return numSegments; } set { numSegments = value; } }
		private int feature_enabled;
		public int _FeatureEnabled { get { return feature_enabled; } set { feature_enabled = value; } }
		private int feature_value;
		public int _FeatureValue { get { return feature_value; } set { feature_value = value; } }
		private int j = 0;

        private (AomArray<AomArray<int>>, AomArray<AomArray<int>>) SegInfo(int numSegments)
        {
			int i = 0;
			int j = 0;
			AomArray<AomArray<int>> enabled = new AomArray<AomArray<int>>(() => new AomArray<int>());
			int clippedValue = 0;
			int bitsToRead = 0;
			int limit = 0;
			int n = 0;
			AomArray<AomArray<int>> data = new AomArray<AomArray<int>>(() => new AomArray<int>());

			for (i = 0; (i < numSegments); i++)
			{

				for (j = 0; (j < SEG_LVL_MAX); j++)
				{
					stream.ReadFixed(1, out this.feature_enabled, "feature_enabled"); 
					enabled[i][j] = feature_enabled;
					clippedValue = 0;

					if ((feature_enabled == 1))
					{
						bitsToRead = Segmentation_Feature_Bits[j];
						limit = Segmentation_Feature_Max[j];

						if ((Segmentation_Feature_Signed[j] == 1))
						{
							n = (1 + bitsToRead);
							stream.ReadSignedIntVar(n, out this.feature_value, "feature_value"); 
							clippedValue = Clip3(-limit, limit, feature_value);
						}
						else 
						{
							stream.ReadVariable(bitsToRead, out this.feature_value, "feature_value"); 
							clippedValue = Clip3(0, limit, feature_value);
						}
					}
					data[i][j] = clippedValue;
				}
			}
			return (enabled, data);
        }

        private (AomArray<AomArray<int>>, AomArray<AomArray<int>>) WriteSegInfo(int numSegments)
        {
			int i = 0;
			int j = 0;
			AomArray<AomArray<int>> enabled = new AomArray<AomArray<int>>(() => new AomArray<int>());
			int clippedValue = 0;
			int bitsToRead = 0;
			int limit = 0;
			int n = 0;
			AomArray<AomArray<int>> data = new AomArray<AomArray<int>>(() => new AomArray<int>());

			for (i = 0; (i < numSegments); i++)
			{

				for (j = 0; (j < SEG_LVL_MAX); j++)
				{
					this.feature_enabled = stream.Pick("feature_enabled", _original != null ? _original.feature_enabled : this.feature_enabled, _edited != null ? _edited.feature_enabled : _original != null ? _original.feature_enabled : this.feature_enabled);
					stream.WriteFixed(1, this.feature_enabled, "feature_enabled"); 
					enabled[i][j] = feature_enabled;
					clippedValue = 0;

					if ((feature_enabled == 1))
					{
						bitsToRead = Segmentation_Feature_Bits[j];
						limit = Segmentation_Feature_Max[j];

						if ((Segmentation_Feature_Signed[j] == 1))
						{
							n = (1 + bitsToRead);
							this.feature_value = stream.Pick("feature_value", _original != null ? _original.feature_value : this.feature_value, _edited != null ? _edited.feature_value : _original != null ? _original.feature_value : this.feature_value);
							stream.WriteSignedIntVar(n, this.feature_value, "feature_value"); 
							clippedValue = Clip3(-limit, limit, feature_value);
						}
						else 
						{
							this.feature_value = stream.Pick("feature_value", _original != null ? _original.feature_value : this.feature_value, _edited != null ? _edited.feature_value : _original != null ? _original.feature_value : this.feature_value);
							stream.WriteVariable(bitsToRead, this.feature_value, "feature_value"); 
							clippedValue = Clip3(0, limit, feature_value);
						}
					}
					data[i][j] = clippedValue;
				}
			}
			return (enabled, data);
        }

    /*
sequence_filter_config() {
disable_loopfilters_across_tiles	f(1)
enable_cdef	f(1)
enable_gdf	f(1)
if ( enable_gdf && get_seq_sb_size() == BLOCK_64X64 ) {	
gdf_unit_matches_sb_size	f(1)
} else {	
gdf_unit_matches_sb_size = 0	
}	
enable_restoration	f(1)
if ( enable_restoration ) {	
lr_tools_disable[ 0 ][ RESTORE_PC_WIENER ]	f(1)
lr_tools_disable[ 0 ][ RESTORE_WIENER_NONSEP ]	f(1)
lr_tools_disable[ 1 ][ RESTORE_PC_WIENER ] = 1	
lr_tools_uv_present	f(1)
if ( lr_tools_uv_present ) {	
lr_tools_disable[ 1 ][ RESTORE_WIENER_NONSEP ]	f(1)
} else {	
lr_tools_disable[ 1 ][ RESTORE_WIENER_NONSEP ] =	
lr_tools_disable[ 0 ][ RESTORE_WIENER_NONSEP ]	
}	
}	
enable_ccso	f(1)
if ( enable_ccso ) {	
ccso_unit_matches_sb_size	f(1)
} else {	
ccso_unit_matches_sb_size = 0	
}	
if ( single_picture_header_flag ) {	
CdefOnSkipTxfm = CDEF_ON_SKIP_TXFM_ADAPTIVE	
} else {	
cdef_on_skip_txfm_always_on	f(1)
if (cdef_on_skip_txfm_always_on) {	
CdefOnSkipTxfm = CDEF_ON_SKIP_TXFM_ALWAYS_ON	
} else {	
cdef_on_skip_txfm_disabled	f(1)
CdefOnSkipTxfm = cdef_on_skip_txfm_disabled ?	
CDEF_ON_SKIP_TXFM_DISABLED : CDEF_ON_SKIP_TXFM_ADAPTIVE	
}	
}	
df_par_bits_minus_2	f(2)
}
    */
		private int disable_loopfilters_across_tiles;
		public int _DisableLoopfiltersAcrossTiles { get { return disable_loopfilters_across_tiles; } set { disable_loopfilters_across_tiles = value; } }
		private int enable_cdef;
		public int _EnableCdef { get { return enable_cdef; } set { enable_cdef = value; } }
		private int enable_gdf;
		public int _EnableGdf { get { return enable_gdf; } set { enable_gdf = value; } }
		private int gdf_unit_matches_sb_size;
		public int _GdfUnitMatchesSbSize { get { return gdf_unit_matches_sb_size; } set { gdf_unit_matches_sb_size = value; } }
		private int enable_restoration;
		public int _EnableRestoration { get { return enable_restoration; } set { enable_restoration = value; } }
		private AomArray<AomArray<int>> lr_tools_disable = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _LrToolsDisable { get { return lr_tools_disable; } set { lr_tools_disable = value; } }
		private int lr_tools_uv_present;
		public int _LrToolsUvPresent { get { return lr_tools_uv_present; } set { lr_tools_uv_present = value; } }
		private int enable_ccso;
		public int _EnableCcso { get { return enable_ccso; } set { enable_ccso = value; } }
		private int ccso_unit_matches_sb_size;
		public int _CcsoUnitMatchesSbSize { get { return ccso_unit_matches_sb_size; } set { ccso_unit_matches_sb_size = value; } }
		private int CdefOnSkipTxfm;
		public int _CdefOnSkipTxfm { get { return CdefOnSkipTxfm; } set { CdefOnSkipTxfm = value; } }
		private int cdef_on_skip_txfm_always_on;
		public int _CdefOnSkipTxfmAlwaysOn { get { return cdef_on_skip_txfm_always_on; } set { cdef_on_skip_txfm_always_on = value; } }
		private int cdef_on_skip_txfm_disabled;
		public int _CdefOnSkipTxfmDisabled { get { return cdef_on_skip_txfm_disabled; } set { cdef_on_skip_txfm_disabled = value; } }
		private int df_par_bits_minus_2;
		public int _DfParBitsMinus2 { get { return df_par_bits_minus_2; } set { df_par_bits_minus_2 = value; } }

        private void SequenceFilterConfig()
        {
			stream.ReadFixed(1, out this.disable_loopfilters_across_tiles, "disable_loopfilters_across_tiles"); 
			stream.ReadFixed(1, out this.enable_cdef, "enable_cdef"); 
			stream.ReadFixed(1, out this.enable_gdf, "enable_gdf"); 

			if (((enable_gdf != 0) && (GetSeqSbSize() == BLOCK_64X64)))
			{
				stream.ReadFixed(1, out this.gdf_unit_matches_sb_size, "gdf_unit_matches_sb_size"); 
			}
			else 
			{
				gdf_unit_matches_sb_size = 0;
			}
			stream.ReadFixed(1, out this.enable_restoration, "enable_restoration"); 

			if ((enable_restoration != 0))
			{
				stream.ReadFixed(1, out this.lr_tools_disable[0][RESTORE_PC_WIENER], "lr_tools_disable"); 
				stream.ReadFixed(1, out this.lr_tools_disable[0][RESTORE_WIENER_NONSEP], "lr_tools_disable"); 
				lr_tools_disable[1][RESTORE_PC_WIENER] = 1;
				stream.ReadFixed(1, out this.lr_tools_uv_present, "lr_tools_uv_present"); 

				if ((lr_tools_uv_present != 0))
				{
					stream.ReadFixed(1, out this.lr_tools_disable[1][RESTORE_WIENER_NONSEP], "lr_tools_disable"); 
				}
				else 
				{
					lr_tools_disable[1][RESTORE_WIENER_NONSEP] = lr_tools_disable[0][RESTORE_WIENER_NONSEP];
				}
			}
			stream.ReadFixed(1, out this.enable_ccso, "enable_ccso"); 

			if ((enable_ccso != 0))
			{
				stream.ReadFixed(1, out this.ccso_unit_matches_sb_size, "ccso_unit_matches_sb_size"); 
			}
			else 
			{
				ccso_unit_matches_sb_size = 0;
			}

			if ((single_picture_header_flag != 0))
			{
				CdefOnSkipTxfm = CDEF_ON_SKIP_TXFM_ADAPTIVE;
			}
			else 
			{
				stream.ReadFixed(1, out this.cdef_on_skip_txfm_always_on, "cdef_on_skip_txfm_always_on"); 

				if ((cdef_on_skip_txfm_always_on != 0))
				{
					CdefOnSkipTxfm = CDEF_ON_SKIP_TXFM_ALWAYS_ON;
				}
				else 
				{
					stream.ReadFixed(1, out this.cdef_on_skip_txfm_disabled, "cdef_on_skip_txfm_disabled"); 
					CdefOnSkipTxfm = ((cdef_on_skip_txfm_disabled != 0) ? CDEF_ON_SKIP_TXFM_DISABLED : CDEF_ON_SKIP_TXFM_ADAPTIVE);
				}
			}
			stream.ReadFixed(2, out this.df_par_bits_minus_2, "df_par_bits_minus_2"); 
        }

        private void WriteSequenceFilterConfig()
        {
			this.disable_loopfilters_across_tiles = stream.Pick("disable_loopfilters_across_tiles", _original != null ? _original.disable_loopfilters_across_tiles : this.disable_loopfilters_across_tiles, _edited != null ? _edited.disable_loopfilters_across_tiles : _original != null ? _original.disable_loopfilters_across_tiles : this.disable_loopfilters_across_tiles);
			stream.WriteFixed(1, this.disable_loopfilters_across_tiles, "disable_loopfilters_across_tiles"); 
			this.enable_cdef = stream.Pick("enable_cdef", _original != null ? _original.enable_cdef : this.enable_cdef, _edited != null ? _edited.enable_cdef : _original != null ? _original.enable_cdef : this.enable_cdef);
			stream.WriteFixed(1, this.enable_cdef, "enable_cdef"); 
			this.enable_gdf = stream.Pick("enable_gdf", _original != null ? _original.enable_gdf : this.enable_gdf, _edited != null ? _edited.enable_gdf : _original != null ? _original.enable_gdf : this.enable_gdf);
			stream.WriteFixed(1, this.enable_gdf, "enable_gdf"); 

			if (((enable_gdf != 0) && (GetSeqSbSize() == BLOCK_64X64)))
			{
				this.gdf_unit_matches_sb_size = stream.Pick("gdf_unit_matches_sb_size", _original != null ? _original.gdf_unit_matches_sb_size : this.gdf_unit_matches_sb_size, _edited != null ? _edited.gdf_unit_matches_sb_size : _original != null ? _original.gdf_unit_matches_sb_size : this.gdf_unit_matches_sb_size);
				stream.WriteFixed(1, this.gdf_unit_matches_sb_size, "gdf_unit_matches_sb_size"); 
			}
			else 
			{
				gdf_unit_matches_sb_size = 0;
			}
			this.enable_restoration = stream.Pick("enable_restoration", _original != null ? _original.enable_restoration : this.enable_restoration, _edited != null ? _edited.enable_restoration : _original != null ? _original.enable_restoration : this.enable_restoration);
			stream.WriteFixed(1, this.enable_restoration, "enable_restoration"); 

			if ((enable_restoration != 0))
			{
				this.lr_tools_disable[0][RESTORE_PC_WIENER] = stream.Pick("lr_tools_disable", _original != null ? _original.lr_tools_disable[0][RESTORE_PC_WIENER] : this.lr_tools_disable[0][RESTORE_PC_WIENER], _edited != null ? _edited.lr_tools_disable[0][RESTORE_PC_WIENER] : _original != null ? _original.lr_tools_disable[0][RESTORE_PC_WIENER] : this.lr_tools_disable[0][RESTORE_PC_WIENER]);
				stream.WriteFixed(1, this.lr_tools_disable[0][RESTORE_PC_WIENER], "lr_tools_disable"); 
				this.lr_tools_disable[0][RESTORE_WIENER_NONSEP] = stream.Pick("lr_tools_disable", _original != null ? _original.lr_tools_disable[0][RESTORE_WIENER_NONSEP] : this.lr_tools_disable[0][RESTORE_WIENER_NONSEP], _edited != null ? _edited.lr_tools_disable[0][RESTORE_WIENER_NONSEP] : _original != null ? _original.lr_tools_disable[0][RESTORE_WIENER_NONSEP] : this.lr_tools_disable[0][RESTORE_WIENER_NONSEP]);
				stream.WriteFixed(1, this.lr_tools_disable[0][RESTORE_WIENER_NONSEP], "lr_tools_disable"); 
				lr_tools_disable[1][RESTORE_PC_WIENER] = 1;
				this.lr_tools_uv_present = stream.Pick("lr_tools_uv_present", _original != null ? _original.lr_tools_uv_present : this.lr_tools_uv_present, _edited != null ? _edited.lr_tools_uv_present : _original != null ? _original.lr_tools_uv_present : this.lr_tools_uv_present);
				stream.WriteFixed(1, this.lr_tools_uv_present, "lr_tools_uv_present"); 

				if ((lr_tools_uv_present != 0))
				{
					this.lr_tools_disable[1][RESTORE_WIENER_NONSEP] = stream.Pick("lr_tools_disable", _original != null ? _original.lr_tools_disable[1][RESTORE_WIENER_NONSEP] : this.lr_tools_disable[1][RESTORE_WIENER_NONSEP], _edited != null ? _edited.lr_tools_disable[1][RESTORE_WIENER_NONSEP] : _original != null ? _original.lr_tools_disable[1][RESTORE_WIENER_NONSEP] : this.lr_tools_disable[1][RESTORE_WIENER_NONSEP]);
					stream.WriteFixed(1, this.lr_tools_disable[1][RESTORE_WIENER_NONSEP], "lr_tools_disable"); 
				}
				else 
				{
					lr_tools_disable[1][RESTORE_WIENER_NONSEP] = lr_tools_disable[0][RESTORE_WIENER_NONSEP];
				}
			}
			this.enable_ccso = stream.Pick("enable_ccso", _original != null ? _original.enable_ccso : this.enable_ccso, _edited != null ? _edited.enable_ccso : _original != null ? _original.enable_ccso : this.enable_ccso);
			stream.WriteFixed(1, this.enable_ccso, "enable_ccso"); 

			if ((enable_ccso != 0))
			{
				this.ccso_unit_matches_sb_size = stream.Pick("ccso_unit_matches_sb_size", _original != null ? _original.ccso_unit_matches_sb_size : this.ccso_unit_matches_sb_size, _edited != null ? _edited.ccso_unit_matches_sb_size : _original != null ? _original.ccso_unit_matches_sb_size : this.ccso_unit_matches_sb_size);
				stream.WriteFixed(1, this.ccso_unit_matches_sb_size, "ccso_unit_matches_sb_size"); 
			}
			else 
			{
				ccso_unit_matches_sb_size = 0;
			}

			if ((single_picture_header_flag != 0))
			{
				CdefOnSkipTxfm = CDEF_ON_SKIP_TXFM_ADAPTIVE;
			}
			else 
			{
				this.cdef_on_skip_txfm_always_on = stream.Pick("cdef_on_skip_txfm_always_on", _original != null ? _original.cdef_on_skip_txfm_always_on : this.cdef_on_skip_txfm_always_on, _edited != null ? _edited.cdef_on_skip_txfm_always_on : _original != null ? _original.cdef_on_skip_txfm_always_on : this.cdef_on_skip_txfm_always_on);
				stream.WriteFixed(1, this.cdef_on_skip_txfm_always_on, "cdef_on_skip_txfm_always_on"); 

				if ((cdef_on_skip_txfm_always_on != 0))
				{
					CdefOnSkipTxfm = CDEF_ON_SKIP_TXFM_ALWAYS_ON;
				}
				else 
				{
					this.cdef_on_skip_txfm_disabled = stream.Pick("cdef_on_skip_txfm_disabled", _original != null ? _original.cdef_on_skip_txfm_disabled : this.cdef_on_skip_txfm_disabled, _edited != null ? _edited.cdef_on_skip_txfm_disabled : _original != null ? _original.cdef_on_skip_txfm_disabled : this.cdef_on_skip_txfm_disabled);
					stream.WriteFixed(1, this.cdef_on_skip_txfm_disabled, "cdef_on_skip_txfm_disabled"); 
					CdefOnSkipTxfm = ((cdef_on_skip_txfm_disabled != 0) ? CDEF_ON_SKIP_TXFM_DISABLED : CDEF_ON_SKIP_TXFM_ADAPTIVE);
				}
			}
			this.df_par_bits_minus_2 = stream.Pick("df_par_bits_minus_2", _original != null ? _original.df_par_bits_minus_2 : this.df_par_bits_minus_2, _edited != null ? _edited.df_par_bits_minus_2 : _original != null ? _original.df_par_bits_minus_2 : this.df_par_bits_minus_2);
			stream.WriteFixed(2, this.df_par_bits_minus_2, "df_par_bits_minus_2"); 
        }

    /*
user_defined_qm( level, t, plane ) {
txSz = Fundamental_Tx_Size[ t ]	
w = Tx_Width[ txSz ]	
h = Tx_Height[ txSz ]	
if ( plane > 0 ) {	
qm_copy_from_previous_plane	f(1)
if ( qm_copy_from_previous_plane ) {	
for ( i = 0; i < h; i++ ) {	
for ( j = 0; j < w; j++ ) {	
UserQm[ level ][ t ][ plane ][ i ][ j ] =	
UserQm[ level ][ t ][ plane - 1 ][ i ][ j ]	
}	
}	
return	
}	
}	
if ( t == 0 ) {	
qm_8x8_is_symmetric	f(1)
} else if ( t == 2 ) {	
qm_4x8_is_transpose_of_8x4	f(1)
if ( qm_4x8_is_transpose_of_8x4 ) {	
for ( i = 0; i < h; i++ ) {	
for ( j = 0; j < w; j++ ) {	
UserQm[ level ][ t ][ plane ][ i ][ j ] =	
UserQm[ level ][ 1 ][ plane ][ j ][ i ]	
}	
}	
return	
}	
}	
scan = get_scan( txSz, TX_CLASS_2D )	
quant = 32	
coefRepeat = 0	
for ( c = 0; c < w * h; c++ ) {	
pos = scan[ c ]	
(row, col) = get_tx_row_col(pos, txSz)	
if ( t == 0 && qm_8x8_is_symmetric && col > row ) {	
quant = UserQm[ level ][ t ][ plane ][ col ][ row ]	
UserQm[ level ][ t ][ plane ][ row ][ col ] = quant	
} else if ( coefRepeat ) {	
UserQm[ level ][ t ][ plane ][ row ][ col ] = quant	
} else {	
quant_delta	svlc()
quant2 = (quant + quant_delta) & 255	
if ( quant2 == 0 ) {	
coefRepeat = 1	
} else {	
quant = quant2	
}	
UserQm[ level ][ t ][ plane ][ row ][ col ] = quant	
}	
}	
}
    */
		private int level;
		public int _Level { get { return level; } set { level = value; } }
		private int t;
		private int plane;
		public int _Plane { get { return plane; } set { plane = value; } }
		private int qm_copy_from_previous_plane;
		public int _QmCopyFromPreviousPlane { get { return qm_copy_from_previous_plane; } set { qm_copy_from_previous_plane = value; } }
		private AomArray<AomArray<AomArray<AomArray<AomArray<int>>>>> UserQm = new AomArray<AomArray<AomArray<AomArray<AomArray<int>>>>>(() => new AomArray<AomArray<AomArray<AomArray<int>>>>(() => new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()))));
		public AomArray<AomArray<AomArray<AomArray<AomArray<int>>>>> _UserQm { get { return UserQm; } set { UserQm = value; } }
		private int qm_8x8_is_symmetric;
		public int _Qm8x8IsSymmetric { get { return qm_8x8_is_symmetric; } set { qm_8x8_is_symmetric = value; } }
		private int qm_4x8_is_transpose_of_8x4;
		public int _Qm4x8IsTransposeOf8x4 { get { return qm_4x8_is_transpose_of_8x4; } set { qm_4x8_is_transpose_of_8x4 = value; } }
		private int quant_delta;
		public int _QuantDelta { get { return quant_delta; } set { quant_delta = value; } }
		private int c = 0;

        private void UserDefinedQm(int level, int t, int plane)
        {
			int i = 0;
			int j = 0;
			int c = 0;
			int txSz = 0;
			int w = 0;
			int h = 0;
			AomArray<int> scan = new AomArray<int>();
			int quant = 0;
			int coefRepeat = 0;
			int pos = 0;
			int row = 0;
			int col = 0;
			int quant2 = 0;
			txSz = Fundamental_Tx_Size[t];
			w = Tx_Width[txSz];
			h = Tx_Height[txSz];

			if ((plane > 0))
			{
				stream.ReadFixed(1, out this.qm_copy_from_previous_plane, "qm_copy_from_previous_plane"); 

				if ((qm_copy_from_previous_plane != 0))
				{

					for (i = 0; (i < h); i++)
					{

						for (j = 0; (j < w); j++)
						{
							UserQm[level][t][plane][i][j] = UserQm[level][t][(plane - 1)][i][j];
						}
					}
					return;
				}
			}

			if ((t == 0))
			{
				stream.ReadFixed(1, out this.qm_8x8_is_symmetric, "qm_8x8_is_symmetric"); 
			}
			else if ((t == 2))
			{
				stream.ReadFixed(1, out this.qm_4x8_is_transpose_of_8x4, "qm_4x8_is_transpose_of_8x4"); 

				if ((qm_4x8_is_transpose_of_8x4 != 0))
				{

					for (i = 0; (i < h); i++)
					{

						for (j = 0; (j < w); j++)
						{
							UserQm[level][t][plane][i][j] = UserQm[level][1][plane][j][i];
						}
					}
					return;
				}
			}
			scan = GetScan(txSz, TX_CLASS_2D);
			quant = 32;
			coefRepeat = 0;

			for (c = 0; (c < (w * h)); c++)
			{
				pos = scan[c];
				(row, col) = GetTxRowCol(pos, txSz);

				if ((((t == 0) && (qm_8x8_is_symmetric != 0)) && (col > row)))
				{
					quant = UserQm[level][t][plane][col][row];
					UserQm[level][t][plane][row][col] = quant;
				}
				else if ((coefRepeat != 0))
				{
					UserQm[level][t][plane][row][col] = quant;
				}
				else 
				{
					stream.ReadSvlc( out this.quant_delta, "quant_delta"); 
					quant2 = ((quant + quant_delta) & 255);

					if ((quant2 == 0))
					{
						coefRepeat = 1;
					}
					else 
					{
						quant = quant2;
					}
					UserQm[level][t][plane][row][col] = quant;
				}
			}
        }

        private void WriteUserDefinedQm(int level, int t, int plane)
        {
			int i = 0;
			int j = 0;
			int c = 0;
			int txSz = 0;
			int w = 0;
			int h = 0;
			AomArray<int> scan = new AomArray<int>();
			int quant = 0;
			int coefRepeat = 0;
			int pos = 0;
			int row = 0;
			int col = 0;
			int quant2 = 0;
			txSz = Fundamental_Tx_Size[t];
			w = Tx_Width[txSz];
			h = Tx_Height[txSz];

			if ((plane > 0))
			{
				this.qm_copy_from_previous_plane = stream.Pick("qm_copy_from_previous_plane", _original != null ? _original.qm_copy_from_previous_plane : this.qm_copy_from_previous_plane, _edited != null ? _edited.qm_copy_from_previous_plane : _original != null ? _original.qm_copy_from_previous_plane : this.qm_copy_from_previous_plane);
				stream.WriteFixed(1, this.qm_copy_from_previous_plane, "qm_copy_from_previous_plane"); 

				if ((qm_copy_from_previous_plane != 0))
				{

					for (i = 0; (i < h); i++)
					{

						for (j = 0; (j < w); j++)
						{
							UserQm[level][t][plane][i][j] = UserQm[level][t][(plane - 1)][i][j];
						}
					}
					return;
				}
			}

			if ((t == 0))
			{
				this.qm_8x8_is_symmetric = stream.Pick("qm_8x8_is_symmetric", _original != null ? _original.qm_8x8_is_symmetric : this.qm_8x8_is_symmetric, _edited != null ? _edited.qm_8x8_is_symmetric : _original != null ? _original.qm_8x8_is_symmetric : this.qm_8x8_is_symmetric);
				stream.WriteFixed(1, this.qm_8x8_is_symmetric, "qm_8x8_is_symmetric"); 
			}
			else if ((t == 2))
			{
				this.qm_4x8_is_transpose_of_8x4 = stream.Pick("qm_4x8_is_transpose_of_8x4", _original != null ? _original.qm_4x8_is_transpose_of_8x4 : this.qm_4x8_is_transpose_of_8x4, _edited != null ? _edited.qm_4x8_is_transpose_of_8x4 : _original != null ? _original.qm_4x8_is_transpose_of_8x4 : this.qm_4x8_is_transpose_of_8x4);
				stream.WriteFixed(1, this.qm_4x8_is_transpose_of_8x4, "qm_4x8_is_transpose_of_8x4"); 

				if ((qm_4x8_is_transpose_of_8x4 != 0))
				{

					for (i = 0; (i < h); i++)
					{

						for (j = 0; (j < w); j++)
						{
							UserQm[level][t][plane][i][j] = UserQm[level][1][plane][j][i];
						}
					}
					return;
				}
			}
			scan = GetScan(txSz, TX_CLASS_2D);
			quant = 32;
			coefRepeat = 0;

			for (c = 0; (c < (w * h)); c++)
			{
				pos = scan[c];
				(row, col) = GetTxRowCol(pos, txSz);

				if ((((t == 0) && (qm_8x8_is_symmetric != 0)) && (col > row)))
				{
					quant = UserQm[level][t][plane][col][row];
					UserQm[level][t][plane][row][col] = quant;
				}
				else if ((coefRepeat != 0))
				{
					UserQm[level][t][plane][row][col] = quant;
				}
				else 
				{
					this.quant_delta = stream.Pick("quant_delta", _original != null ? _original.quant_delta : this.quant_delta, _edited != null ? _edited.quant_delta : _original != null ? _original.quant_delta : this.quant_delta);
					stream.WriteSvlc( this.quant_delta, "quant_delta"); 
					quant2 = ((quant + quant_delta) & 255);

					if ((quant2 == 0))
					{
						coefRepeat = 1;
					}
					else 
					{
						quant = quant2;
					}
					UserQm[level][t][plane][row][col] = quant;
				}
			}
        }

    /*
timing_info() {
num_units_in_display_tick	f(32)
time_scale	f(32)
equal_picture_interval	f(1)
if ( equal_picture_interval ) {	
num_ticks_per_picture_minus_1	uvlc()
}	
}
    */
		private int num_units_in_display_tick;
		public int _NumUnitsInDisplayTick { get { return num_units_in_display_tick; } set { num_units_in_display_tick = value; } }
		private int time_scale;
		public int _TimeScale { get { return time_scale; } set { time_scale = value; } }
		private int equal_picture_interval;
		public int _EqualPictureInterval { get { return equal_picture_interval; } set { equal_picture_interval = value; } }
		private int num_ticks_per_picture_minus_1;
		public int _NumTicksPerPictureMinus1 { get { return num_ticks_per_picture_minus_1; } set { num_ticks_per_picture_minus_1 = value; } }

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
seq_decoder_model_info() {
decoder_buffer_delay	uvlc()
encoder_buffer_delay	uvlc()
low_delay_mode_flag	f(1)
}
    */
		private int decoder_buffer_delay;
		public int _DecoderBufferDelay { get { return decoder_buffer_delay; } set { decoder_buffer_delay = value; } }
		private int encoder_buffer_delay;
		public int _EncoderBufferDelay { get { return encoder_buffer_delay; } set { encoder_buffer_delay = value; } }
		private int low_delay_mode_flag;
		public int _LowDelayModeFlag { get { return low_delay_mode_flag; } set { low_delay_mode_flag = value; } }

        private void SeqDecoderModelInfo()
        {
			stream.ReadUvlc( out this.decoder_buffer_delay, "decoder_buffer_delay"); 
			stream.ReadUvlc( out this.encoder_buffer_delay, "encoder_buffer_delay"); 
			stream.ReadFixed(1, out this.low_delay_mode_flag, "low_delay_mode_flag"); 
        }

        private void WriteSeqDecoderModelInfo()
        {
			this.decoder_buffer_delay = stream.Pick("decoder_buffer_delay", _original != null ? _original.decoder_buffer_delay : this.decoder_buffer_delay, _edited != null ? _edited.decoder_buffer_delay : _original != null ? _original.decoder_buffer_delay : this.decoder_buffer_delay);
			stream.WriteUvlc( this.decoder_buffer_delay, "decoder_buffer_delay"); 
			this.encoder_buffer_delay = stream.Pick("encoder_buffer_delay", _original != null ? _original.encoder_buffer_delay : this.encoder_buffer_delay, _edited != null ? _edited.encoder_buffer_delay : _original != null ? _original.encoder_buffer_delay : this.encoder_buffer_delay);
			stream.WriteUvlc( this.encoder_buffer_delay, "encoder_buffer_delay"); 
			this.low_delay_mode_flag = stream.Pick("low_delay_mode_flag", _original != null ? _original.low_delay_mode_flag : this.low_delay_mode_flag, _edited != null ? _edited.low_delay_mode_flag : _original != null ? _original.low_delay_mode_flag : this.low_delay_mode_flag);
			stream.WriteFixed(1, this.low_delay_mode_flag, "low_delay_mode_flag"); 
        }

    /*
temporal_delimiter_obu() {
SeenFrameHeader = 0	
for ( level = 0; level < 15; level++ ) {	
QmProtected[ level ] = 0	
}	
}
    */
		private int SeenFrameHeader;
		public int _SeenFrameHeader { get { return SeenFrameHeader; } set { SeenFrameHeader = value; } }
		private AomArray<int> QmProtected = new AomArray<int>();
		public AomArray<int> _QmProtected { get { return QmProtected; } set { QmProtected = value; } }

        private void TemporalDelimiterObu()
        {
			int level = 0;
			SeenFrameHeader = 0;

			for (level = 0; (level < 15); level++)
			{
				QmProtected[level] = 0;
			}
        }

    /*
multistream_decoder_operation_obu() {
num_streams_minus_2	f(3)
multistream_profile_idc	f(5)
multistream_level_idx	f(5)
multistream_tier	f(1)
multistream_even_allocation_flag	f(1)
if ( !multistream_even_allocation_flag ) {	
multistream_large_picture_idc	f(3)
}	
for ( i = 0; i < num_streams_minus_2 + 2; i++ ) {	
sub_xlayer_id[ i ]	f(5)
sub_stream_max_profile[ i ]	f(5)
sub_stream_max_level[ i ]	f(5)
sub_stream_max_tier[ i ]	f(1)
}	
multistream_doh_constraint_flag	f(1)
}
    */
		private int num_streams_minus_2;
		public int _NumStreamsMinus2 { get { return num_streams_minus_2; } set { num_streams_minus_2 = value; } }
		private int multistream_profile_idc;
		public int _MultistreamProfileIdc { get { return multistream_profile_idc; } set { multistream_profile_idc = value; } }
		private int multistream_level_idx;
		public int _MultistreamLevelIdx { get { return multistream_level_idx; } set { multistream_level_idx = value; } }
		private int multistream_tier;
		public int _MultistreamTier { get { return multistream_tier; } set { multistream_tier = value; } }
		private int multistream_even_allocation_flag;
		public int _MultistreamEvenAllocationFlag { get { return multistream_even_allocation_flag; } set { multistream_even_allocation_flag = value; } }
		private int multistream_large_picture_idc;
		public int _MultistreamLargePictureIdc { get { return multistream_large_picture_idc; } set { multistream_large_picture_idc = value; } }
		private AomArray<int> sub_xlayer_id = new AomArray<int>();
		public AomArray<int> _SubXlayerId { get { return sub_xlayer_id; } set { sub_xlayer_id = value; } }
		private AomArray<int> sub_stream_max_profile = new AomArray<int>();
		public AomArray<int> _SubStreamMaxProfile { get { return sub_stream_max_profile; } set { sub_stream_max_profile = value; } }
		private AomArray<int> sub_stream_max_level = new AomArray<int>();
		public AomArray<int> _SubStreamMaxLevel { get { return sub_stream_max_level; } set { sub_stream_max_level = value; } }
		private AomArray<int> sub_stream_max_tier = new AomArray<int>();
		public AomArray<int> _SubStreamMaxTier { get { return sub_stream_max_tier; } set { sub_stream_max_tier = value; } }
		private int multistream_doh_constraint_flag;
		public int _MultistreamDohConstraintFlag { get { return multistream_doh_constraint_flag; } set { multistream_doh_constraint_flag = value; } }

        private void MultistreamDecoderOperationObu()
        {
			int i = 0;
			stream.ReadFixed(3, out this.num_streams_minus_2, "num_streams_minus_2"); 
			stream.ReadFixed(5, out this.multistream_profile_idc, "multistream_profile_idc"); 
			stream.ReadFixed(5, out this.multistream_level_idx, "multistream_level_idx"); 
			stream.ReadFixed(1, out this.multistream_tier, "multistream_tier"); 
			stream.ReadFixed(1, out this.multistream_even_allocation_flag, "multistream_even_allocation_flag"); 

			if (!(multistream_even_allocation_flag != 0))
			{
				stream.ReadFixed(3, out this.multistream_large_picture_idc, "multistream_large_picture_idc"); 
			}

			for (i = 0; (i < (num_streams_minus_2 + 2)); i++)
			{
				stream.ReadFixed(5, out this.sub_xlayer_id[i], "sub_xlayer_id"); 
				stream.ReadFixed(5, out this.sub_stream_max_profile[i], "sub_stream_max_profile"); 
				stream.ReadFixed(5, out this.sub_stream_max_level[i], "sub_stream_max_level"); 
				stream.ReadFixed(1, out this.sub_stream_max_tier[i], "sub_stream_max_tier"); 
			}
			stream.ReadFixed(1, out this.multistream_doh_constraint_flag, "multistream_doh_constraint_flag"); 
        }

        private void WriteMultistreamDecoderOperationObu()
        {
			int i = 0;
			this.num_streams_minus_2 = stream.Pick("num_streams_minus_2", _original != null ? _original.num_streams_minus_2 : this.num_streams_minus_2, _edited != null ? _edited.num_streams_minus_2 : _original != null ? _original.num_streams_minus_2 : this.num_streams_minus_2);
			stream.WriteFixed(3, this.num_streams_minus_2, "num_streams_minus_2"); 
			this.multistream_profile_idc = stream.Pick("multistream_profile_idc", _original != null ? _original.multistream_profile_idc : this.multistream_profile_idc, _edited != null ? _edited.multistream_profile_idc : _original != null ? _original.multistream_profile_idc : this.multistream_profile_idc);
			stream.WriteFixed(5, this.multistream_profile_idc, "multistream_profile_idc"); 
			this.multistream_level_idx = stream.Pick("multistream_level_idx", _original != null ? _original.multistream_level_idx : this.multistream_level_idx, _edited != null ? _edited.multistream_level_idx : _original != null ? _original.multistream_level_idx : this.multistream_level_idx);
			stream.WriteFixed(5, this.multistream_level_idx, "multistream_level_idx"); 
			this.multistream_tier = stream.Pick("multistream_tier", _original != null ? _original.multistream_tier : this.multistream_tier, _edited != null ? _edited.multistream_tier : _original != null ? _original.multistream_tier : this.multistream_tier);
			stream.WriteFixed(1, this.multistream_tier, "multistream_tier"); 
			this.multistream_even_allocation_flag = stream.Pick("multistream_even_allocation_flag", _original != null ? _original.multistream_even_allocation_flag : this.multistream_even_allocation_flag, _edited != null ? _edited.multistream_even_allocation_flag : _original != null ? _original.multistream_even_allocation_flag : this.multistream_even_allocation_flag);
			stream.WriteFixed(1, this.multistream_even_allocation_flag, "multistream_even_allocation_flag"); 

			if (!(multistream_even_allocation_flag != 0))
			{
				this.multistream_large_picture_idc = stream.Pick("multistream_large_picture_idc", _original != null ? _original.multistream_large_picture_idc : this.multistream_large_picture_idc, _edited != null ? _edited.multistream_large_picture_idc : _original != null ? _original.multistream_large_picture_idc : this.multistream_large_picture_idc);
				stream.WriteFixed(3, this.multistream_large_picture_idc, "multistream_large_picture_idc"); 
			}

			for (i = 0; (i < (num_streams_minus_2 + 2)); i++)
			{
				this.sub_xlayer_id[i] = stream.Pick("sub_xlayer_id", _original != null ? _original.sub_xlayer_id[i] : this.sub_xlayer_id[i], _edited != null ? _edited.sub_xlayer_id[i] : _original != null ? _original.sub_xlayer_id[i] : this.sub_xlayer_id[i]);
				stream.WriteFixed(5, this.sub_xlayer_id[i], "sub_xlayer_id"); 
				this.sub_stream_max_profile[i] = stream.Pick("sub_stream_max_profile", _original != null ? _original.sub_stream_max_profile[i] : this.sub_stream_max_profile[i], _edited != null ? _edited.sub_stream_max_profile[i] : _original != null ? _original.sub_stream_max_profile[i] : this.sub_stream_max_profile[i]);
				stream.WriteFixed(5, this.sub_stream_max_profile[i], "sub_stream_max_profile"); 
				this.sub_stream_max_level[i] = stream.Pick("sub_stream_max_level", _original != null ? _original.sub_stream_max_level[i] : this.sub_stream_max_level[i], _edited != null ? _edited.sub_stream_max_level[i] : _original != null ? _original.sub_stream_max_level[i] : this.sub_stream_max_level[i]);
				stream.WriteFixed(5, this.sub_stream_max_level[i], "sub_stream_max_level"); 
				this.sub_stream_max_tier[i] = stream.Pick("sub_stream_max_tier", _original != null ? _original.sub_stream_max_tier[i] : this.sub_stream_max_tier[i], _edited != null ? _edited.sub_stream_max_tier[i] : _original != null ? _original.sub_stream_max_tier[i] : this.sub_stream_max_tier[i]);
				stream.WriteFixed(1, this.sub_stream_max_tier[i], "sub_stream_max_tier"); 
			}
			this.multistream_doh_constraint_flag = stream.Pick("multistream_doh_constraint_flag", _original != null ? _original.multistream_doh_constraint_flag : this.multistream_doh_constraint_flag, _edited != null ? _edited.multistream_doh_constraint_flag : _original != null ? _original.multistream_doh_constraint_flag : this.multistream_doh_constraint_flag);
			stream.WriteFixed(1, this.multistream_doh_constraint_flag, "multistream_doh_constraint_flag"); 
        }

    /*
multi_frame_header_obu() {
mfh_seq_header_id	uvlc()
mfh_id_minus_1	uvlc()
mfhId = mfh_id_minus_1 + 1	
MfhSeqHeaderId[ mfhId ] = mfh_seq_header_id	
MfhTLayerId[ mfhId ] = obu_tlayer_id	
MfhMLayerId[ mfhId ] = obu_mlayer_id	
mfh_frame_size_present_flag[ mfhId ]	f(1)
if ( mfh_frame_size_present_flag[ mfhId ] ) {	
mfh_frame_width_bits_minus_1	f(4)
mfh_frame_height_bits_minus_1	f(4)
n = mfh_frame_width_bits_minus_1 + 1	
mfh_frame_width_minus_1[ mfhId ]	f(n)
n = mfh_frame_height_bits_minus_1 + 1	
mfh_frame_height_minus_1[ mfhId ]	f(n)
}	
mfh_deblocking_filter_update[ mfhId ]	f(1)
if ( mfh_deblocking_filter_update[ mfhId ] ) {	
for ( i = 0; i < 4; i++ ) {	
mfh_apply_deblocking_filter[ mfhId ][ i ]	f(1)
}	
}	
mfh_seg_info_present_flag[ mfhId ]	f(1)
if ( mfh_seg_info_present_flag[ mfhId ] ) {	
mfh_ext_seg_flag[ mfhId ]	f(1)
mfh_allow_seg_info_change[ mfhId ]	f(1)
( MfhFeatureEnabled[mfhId], MfhFeatureData[mfhId] ) =	
seg_info( mfh_ext_seg_flag[ mfhId ] ? 16 : 8 )	
}	
}
    */
		private int mfh_seq_header_id;
		public int _MfhSeqHeaderId { get { return mfh_seq_header_id; } set { mfh_seq_header_id = value; } }
		private int mfh_id_minus_1;
		public int _MfhIdMinus1 { get { return mfh_id_minus_1; } set { mfh_id_minus_1 = value; } }
		private AomArray<int> MfhSeqHeaderId = new AomArray<int>();
		public AomArray<int> __MfhSeqHeaderId { get { return MfhSeqHeaderId; } set { MfhSeqHeaderId = value; } }
		private AomArray<int> MfhTLayerId = new AomArray<int>();
		public AomArray<int> _MfhTLayerId { get { return MfhTLayerId; } set { MfhTLayerId = value; } }
		private AomArray<int> MfhMLayerId = new AomArray<int>();
		public AomArray<int> _MfhMLayerId { get { return MfhMLayerId; } set { MfhMLayerId = value; } }
		private AomArray<int> mfh_frame_size_present_flag = new AomArray<int>();
		public AomArray<int> _MfhFrameSizePresentFlag { get { return mfh_frame_size_present_flag; } set { mfh_frame_size_present_flag = value; } }
		private int mfh_frame_width_bits_minus_1;
		public int _MfhFrameWidthBitsMinus1 { get { return mfh_frame_width_bits_minus_1; } set { mfh_frame_width_bits_minus_1 = value; } }
		private int mfh_frame_height_bits_minus_1;
		public int _MfhFrameHeightBitsMinus1 { get { return mfh_frame_height_bits_minus_1; } set { mfh_frame_height_bits_minus_1 = value; } }
		private AomArray<int> mfh_frame_width_minus_1 = new AomArray<int>();
		public AomArray<int> _MfhFrameWidthMinus1 { get { return mfh_frame_width_minus_1; } set { mfh_frame_width_minus_1 = value; } }
		private AomArray<int> mfh_frame_height_minus_1 = new AomArray<int>();
		public AomArray<int> _MfhFrameHeightMinus1 { get { return mfh_frame_height_minus_1; } set { mfh_frame_height_minus_1 = value; } }
		private AomArray<int> mfh_deblocking_filter_update = new AomArray<int>();
		public AomArray<int> _MfhDeblockingFilterUpdate { get { return mfh_deblocking_filter_update; } set { mfh_deblocking_filter_update = value; } }
		private AomArray<AomArray<int>> mfh_apply_deblocking_filter = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _MfhApplyDeblockingFilter { get { return mfh_apply_deblocking_filter; } set { mfh_apply_deblocking_filter = value; } }
		private AomArray<int> mfh_seg_info_present_flag = new AomArray<int>();
		public AomArray<int> _MfhSegInfoPresentFlag { get { return mfh_seg_info_present_flag; } set { mfh_seg_info_present_flag = value; } }
		private AomArray<int> mfh_ext_seg_flag = new AomArray<int>();
		public AomArray<int> _MfhExtSegFlag { get { return mfh_ext_seg_flag; } set { mfh_ext_seg_flag = value; } }
		private AomArray<int> mfh_allow_seg_info_change = new AomArray<int>();
		public AomArray<int> _MfhAllowSegInfoChange { get { return mfh_allow_seg_info_change; } set { mfh_allow_seg_info_change = value; } }
		private AomArray<AomArray<AomArray<int>>> MfhFeatureEnabled = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _MfhFeatureEnabled { get { return MfhFeatureEnabled; } set { MfhFeatureEnabled = value; } }
		private AomArray<AomArray<AomArray<int>>> MfhFeatureData = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _MfhFeatureData { get { return MfhFeatureData; } set { MfhFeatureData = value; } }

        private void MultiFrameHeaderObu()
        {
			int i = 0;
			int mfhId = 0;
			int n = 0;
			stream.ReadUvlc( out this.mfh_seq_header_id, "mfh_seq_header_id"); 
			stream.ReadUvlc( out this.mfh_id_minus_1, "mfh_id_minus_1"); 
			mfhId = (mfh_id_minus_1 + 1);
			MfhSeqHeaderId[mfhId] = mfh_seq_header_id;
			MfhTLayerId[mfhId] = obu_tlayer_id;
			MfhMLayerId[mfhId] = obu_mlayer_id;
			stream.ReadFixed(1, out this.mfh_frame_size_present_flag[mfhId], "mfh_frame_size_present_flag"); 

			if ((mfh_frame_size_present_flag[mfhId] != 0))
			{
				stream.ReadFixed(4, out this.mfh_frame_width_bits_minus_1, "mfh_frame_width_bits_minus_1"); 
				stream.ReadFixed(4, out this.mfh_frame_height_bits_minus_1, "mfh_frame_height_bits_minus_1"); 
				n = (mfh_frame_width_bits_minus_1 + 1);
				stream.ReadVariable(n, out this.mfh_frame_width_minus_1[mfhId], "mfh_frame_width_minus_1"); 
				n = (mfh_frame_height_bits_minus_1 + 1);
				stream.ReadVariable(n, out this.mfh_frame_height_minus_1[mfhId], "mfh_frame_height_minus_1"); 
			}
			stream.ReadFixed(1, out this.mfh_deblocking_filter_update[mfhId], "mfh_deblocking_filter_update"); 

			if ((mfh_deblocking_filter_update[mfhId] != 0))
			{

				for (i = 0; (i < 4); i++)
				{
					stream.ReadFixed(1, out this.mfh_apply_deblocking_filter[mfhId][i], "mfh_apply_deblocking_filter"); 
				}
			}
			stream.ReadFixed(1, out this.mfh_seg_info_present_flag[mfhId], "mfh_seg_info_present_flag"); 

			if ((mfh_seg_info_present_flag[mfhId] != 0))
			{
				stream.ReadFixed(1, out this.mfh_ext_seg_flag[mfhId], "mfh_ext_seg_flag"); 
				stream.ReadFixed(1, out this.mfh_allow_seg_info_change[mfhId], "mfh_allow_seg_info_change"); 
				(MfhFeatureEnabled[mfhId], MfhFeatureData[mfhId]) = SegInfo(((mfh_ext_seg_flag[mfhId] != 0) ? 16 : 8));
			}
        }

        private void WriteMultiFrameHeaderObu()
        {
			int i = 0;
			int mfhId = 0;
			int n = 0;
			this.mfh_seq_header_id = stream.Pick("mfh_seq_header_id", _original != null ? _original.MfhSeqHeaderId[mfhId] : this.mfh_seq_header_id, _edited != null ? _edited.MfhSeqHeaderId[mfhId] : _original != null ? _original.MfhSeqHeaderId[mfhId] : this.mfh_seq_header_id);
			stream.WriteUvlc( this.mfh_seq_header_id, "mfh_seq_header_id"); 
			this.mfh_id_minus_1 = stream.Pick("mfh_id_minus_1", _original != null ? _original.mfh_id_minus_1 : this.mfh_id_minus_1, _edited != null ? _edited.mfh_id_minus_1 : _original != null ? _original.mfh_id_minus_1 : this.mfh_id_minus_1);
			stream.WriteUvlc( this.mfh_id_minus_1, "mfh_id_minus_1"); 
			mfhId = (mfh_id_minus_1 + 1);
			MfhSeqHeaderId[mfhId] = mfh_seq_header_id;
			MfhTLayerId[mfhId] = obu_tlayer_id;
			MfhMLayerId[mfhId] = obu_mlayer_id;
			this.mfh_frame_size_present_flag[mfhId] = stream.Pick("mfh_frame_size_present_flag", _original != null ? _original.mfh_frame_size_present_flag[mfhId] : this.mfh_frame_size_present_flag[mfhId], _edited != null ? _edited.mfh_frame_size_present_flag[mfhId] : _original != null ? _original.mfh_frame_size_present_flag[mfhId] : this.mfh_frame_size_present_flag[mfhId]);
			stream.WriteFixed(1, this.mfh_frame_size_present_flag[mfhId], "mfh_frame_size_present_flag"); 

			if ((mfh_frame_size_present_flag[mfhId] != 0))
			{
				this.mfh_frame_width_bits_minus_1 = stream.Pick("mfh_frame_width_bits_minus_1", _original != null ? _original.mfh_frame_width_bits_minus_1 : this.mfh_frame_width_bits_minus_1, _edited != null ? _edited.mfh_frame_width_bits_minus_1 : _original != null ? _original.mfh_frame_width_bits_minus_1 : this.mfh_frame_width_bits_minus_1);
				stream.WriteFixed(4, this.mfh_frame_width_bits_minus_1, "mfh_frame_width_bits_minus_1"); 
				this.mfh_frame_height_bits_minus_1 = stream.Pick("mfh_frame_height_bits_minus_1", _original != null ? _original.mfh_frame_height_bits_minus_1 : this.mfh_frame_height_bits_minus_1, _edited != null ? _edited.mfh_frame_height_bits_minus_1 : _original != null ? _original.mfh_frame_height_bits_minus_1 : this.mfh_frame_height_bits_minus_1);
				stream.WriteFixed(4, this.mfh_frame_height_bits_minus_1, "mfh_frame_height_bits_minus_1"); 
				n = (mfh_frame_width_bits_minus_1 + 1);
				this.mfh_frame_width_minus_1[mfhId] = stream.Pick("mfh_frame_width_minus_1", _original != null ? _original.mfh_frame_width_minus_1[mfhId] : this.mfh_frame_width_minus_1[mfhId], _edited != null ? _edited.mfh_frame_width_minus_1[mfhId] : _original != null ? _original.mfh_frame_width_minus_1[mfhId] : this.mfh_frame_width_minus_1[mfhId]);
				stream.WriteVariable(n, this.mfh_frame_width_minus_1[mfhId], "mfh_frame_width_minus_1"); 
				n = (mfh_frame_height_bits_minus_1 + 1);
				this.mfh_frame_height_minus_1[mfhId] = stream.Pick("mfh_frame_height_minus_1", _original != null ? _original.mfh_frame_height_minus_1[mfhId] : this.mfh_frame_height_minus_1[mfhId], _edited != null ? _edited.mfh_frame_height_minus_1[mfhId] : _original != null ? _original.mfh_frame_height_minus_1[mfhId] : this.mfh_frame_height_minus_1[mfhId]);
				stream.WriteVariable(n, this.mfh_frame_height_minus_1[mfhId], "mfh_frame_height_minus_1"); 
			}
			this.mfh_deblocking_filter_update[mfhId] = stream.Pick("mfh_deblocking_filter_update", _original != null ? _original.mfh_deblocking_filter_update[mfhId] : this.mfh_deblocking_filter_update[mfhId], _edited != null ? _edited.mfh_deblocking_filter_update[mfhId] : _original != null ? _original.mfh_deblocking_filter_update[mfhId] : this.mfh_deblocking_filter_update[mfhId]);
			stream.WriteFixed(1, this.mfh_deblocking_filter_update[mfhId], "mfh_deblocking_filter_update"); 

			if ((mfh_deblocking_filter_update[mfhId] != 0))
			{

				for (i = 0; (i < 4); i++)
				{
					this.mfh_apply_deblocking_filter[mfhId][i] = stream.Pick("mfh_apply_deblocking_filter", _original != null ? _original.mfh_apply_deblocking_filter[mfhId][i] : this.mfh_apply_deblocking_filter[mfhId][i], _edited != null ? _edited.mfh_apply_deblocking_filter[mfhId][i] : _original != null ? _original.mfh_apply_deblocking_filter[mfhId][i] : this.mfh_apply_deblocking_filter[mfhId][i]);
					stream.WriteFixed(1, this.mfh_apply_deblocking_filter[mfhId][i], "mfh_apply_deblocking_filter"); 
				}
			}
			this.mfh_seg_info_present_flag[mfhId] = stream.Pick("mfh_seg_info_present_flag", _original != null ? _original.mfh_seg_info_present_flag[mfhId] : this.mfh_seg_info_present_flag[mfhId], _edited != null ? _edited.mfh_seg_info_present_flag[mfhId] : _original != null ? _original.mfh_seg_info_present_flag[mfhId] : this.mfh_seg_info_present_flag[mfhId]);
			stream.WriteFixed(1, this.mfh_seg_info_present_flag[mfhId], "mfh_seg_info_present_flag"); 

			if ((mfh_seg_info_present_flag[mfhId] != 0))
			{
				this.mfh_ext_seg_flag[mfhId] = stream.Pick("mfh_ext_seg_flag", _original != null ? _original.mfh_ext_seg_flag[mfhId] : this.mfh_ext_seg_flag[mfhId], _edited != null ? _edited.mfh_ext_seg_flag[mfhId] : _original != null ? _original.mfh_ext_seg_flag[mfhId] : this.mfh_ext_seg_flag[mfhId]);
				stream.WriteFixed(1, this.mfh_ext_seg_flag[mfhId], "mfh_ext_seg_flag"); 
				this.mfh_allow_seg_info_change[mfhId] = stream.Pick("mfh_allow_seg_info_change", _original != null ? _original.mfh_allow_seg_info_change[mfhId] : this.mfh_allow_seg_info_change[mfhId], _edited != null ? _edited.mfh_allow_seg_info_change[mfhId] : _original != null ? _original.mfh_allow_seg_info_change[mfhId] : this.mfh_allow_seg_info_change[mfhId]);
				stream.WriteFixed(1, this.mfh_allow_seg_info_change[mfhId], "mfh_allow_seg_info_change"); 
				(MfhFeatureEnabled[mfhId], MfhFeatureData[mfhId]) = WriteSegInfo(((mfh_ext_seg_flag[mfhId] != 0) ? 16 : 8));
			}
        }

    /*
layer_config_record_obu() {
if ( obu_xlayer_id == GLOBAL_XLAYER_ID ) {	
lcr_global_info()	
} else {	
lcr_local_info( obu_xlayer_id )	
}	
}
    */

        private void LayerConfigRecordObu()
        {

			if ((obu_xlayer_id == GLOBAL_XLAYER_ID))
			{
				LcrGlobalInfo(); 
			}
			else 
			{
				LcrLocalInfo(obu_xlayer_id); 
			}
        }

        private void WriteLayerConfigRecordObu()
        {

			if ((obu_xlayer_id == GLOBAL_XLAYER_ID))
			{
				WriteLcrGlobalInfo(); 
			}
			else 
			{
				WriteLcrLocalInfo(obu_xlayer_id); 
			}
        }

    /*
lcr_global_info() {
lcr_global_config_record_id	f(3)
lcr_xlayer_map	f(31)
LcrMaxNumXLayerCount = 0	
for ( i = 0; i < 31; i++ ) {	
if ( lcr_xlayer_map & ( 1 << i ) ) {	
LcrXLayerID[ LcrMaxNumXLayerCount ] = i	
LcrMaxNumXLayerCount ++	
}	
}	
lcr_aggregate_info_present_flag	f(1)
lcr_seq_profile_tier_level_info_present_flag	f(1)
lcr_global_payload_present_flag	f(1)
lcr_dependent_xlayers_flag	f(1)
lcr_global_atlas_id_present_flag	f(1)
lcr_global_purpose_id	f(7)
lcr_doh_constraint_flag	f(1)
lcr_enforce_tile_alignment_flag	f(1)
if ( lcr_global_atlas_id_present_flag ) {	
lcr_global_atlas_id	f(3)
} else {	
lcr_global_reserved_zero_3bits	f(3)
}	
lcr_global_reserved_zero_5bits	f(5)
if ( lcr_aggregate_info_present_flag ) {	
lcr_aggregate_info()	
}	
if ( lcr_seq_profile_tier_level_info_present_flag ) {	
for ( i = 0; i < LcrMaxNumXLayerCount; i++ ) {	
lcr_seq_profile_tier_level_info( LcrXLayerID[ i ] )	
}	
}	
if ( lcr_global_payload_present_flag ) {	
for ( i = 0; i < LcrMaxNumXLayerCount; i++) {	
lcr_data_size [ i ]	leb128()
lcr_global_payload( LcrXLayerID[ i ], lcr_data_size [ i ] )	
}	
}	
}
    */
		private int lcr_global_config_record_id;
		public int _LcrGlobalConfigRecordId { get { return lcr_global_config_record_id; } set { lcr_global_config_record_id = value; } }
		private int lcr_xlayer_map;
		public int _LcrXlayerMap { get { return lcr_xlayer_map; } set { lcr_xlayer_map = value; } }
		private int LcrMaxNumXLayerCount;
		public int _LcrMaxNumXLayerCount { get { return LcrMaxNumXLayerCount; } set { LcrMaxNumXLayerCount = value; } }
		private AomArray<int> LcrXLayerID = new AomArray<int>();
		public AomArray<int> _LcrXLayerID { get { return LcrXLayerID; } set { LcrXLayerID = value; } }
		private int lcr_aggregate_info_present_flag;
		public int _LcrAggregateInfoPresentFlag { get { return lcr_aggregate_info_present_flag; } set { lcr_aggregate_info_present_flag = value; } }
		private int lcr_seq_profile_tier_level_info_present_flag;
		public int _LcrSeqProfileTierLevelInfoPresentFlag { get { return lcr_seq_profile_tier_level_info_present_flag; } set { lcr_seq_profile_tier_level_info_present_flag = value; } }
		private int lcr_global_payload_present_flag;
		public int _LcrGlobalPayloadPresentFlag { get { return lcr_global_payload_present_flag; } set { lcr_global_payload_present_flag = value; } }
		private int lcr_dependent_xlayers_flag;
		public int _LcrDependentXlayersFlag { get { return lcr_dependent_xlayers_flag; } set { lcr_dependent_xlayers_flag = value; } }
		private int lcr_global_atlas_id_present_flag;
		public int _LcrGlobalAtlasIdPresentFlag { get { return lcr_global_atlas_id_present_flag; } set { lcr_global_atlas_id_present_flag = value; } }
		private int lcr_global_purpose_id;
		public int _LcrGlobalPurposeId { get { return lcr_global_purpose_id; } set { lcr_global_purpose_id = value; } }
		private int lcr_doh_constraint_flag;
		public int _LcrDohConstraintFlag { get { return lcr_doh_constraint_flag; } set { lcr_doh_constraint_flag = value; } }
		private int lcr_enforce_tile_alignment_flag;
		public int _LcrEnforceTileAlignmentFlag { get { return lcr_enforce_tile_alignment_flag; } set { lcr_enforce_tile_alignment_flag = value; } }
		private int lcr_global_atlas_id;
		public int _LcrGlobalAtlasId { get { return lcr_global_atlas_id; } set { lcr_global_atlas_id = value; } }
		private int lcr_global_reserved_zero_3bits;
		public int _LcrGlobalReservedZero3bits { get { return lcr_global_reserved_zero_3bits; } set { lcr_global_reserved_zero_3bits = value; } }
		private int lcr_global_reserved_zero_5bits;
		public int _LcrGlobalReservedZero5bits { get { return lcr_global_reserved_zero_5bits; } set { lcr_global_reserved_zero_5bits = value; } }
		private AomArray<int> lcr_data_size = new AomArray<int>();
		public AomArray<int> _LcrDataSize { get { return lcr_data_size; } set { lcr_data_size = value; } }

        private void LcrGlobalInfo()
        {
			int i = 0;
			stream.ReadFixed(3, out this.lcr_global_config_record_id, "lcr_global_config_record_id"); 
			stream.ReadFixed(31, out this.lcr_xlayer_map, "lcr_xlayer_map"); 
			LcrMaxNumXLayerCount = 0;

			for (i = 0; (i < 31); i++)
			{

				if (((lcr_xlayer_map & (1 << i)) != 0))
				{
					LcrXLayerID[LcrMaxNumXLayerCount] = i;
					LcrMaxNumXLayerCount++;
				}
			}
			stream.ReadFixed(1, out this.lcr_aggregate_info_present_flag, "lcr_aggregate_info_present_flag"); 
			stream.ReadFixed(1, out this.lcr_seq_profile_tier_level_info_present_flag, "lcr_seq_profile_tier_level_info_present_flag"); 
			stream.ReadFixed(1, out this.lcr_global_payload_present_flag, "lcr_global_payload_present_flag"); 
			stream.ReadFixed(1, out this.lcr_dependent_xlayers_flag, "lcr_dependent_xlayers_flag"); 
			stream.ReadFixed(1, out this.lcr_global_atlas_id_present_flag, "lcr_global_atlas_id_present_flag"); 
			stream.ReadFixed(7, out this.lcr_global_purpose_id, "lcr_global_purpose_id"); 
			stream.ReadFixed(1, out this.lcr_doh_constraint_flag, "lcr_doh_constraint_flag"); 
			stream.ReadFixed(1, out this.lcr_enforce_tile_alignment_flag, "lcr_enforce_tile_alignment_flag"); 

			if ((lcr_global_atlas_id_present_flag != 0))
			{
				stream.ReadFixed(3, out this.lcr_global_atlas_id, "lcr_global_atlas_id"); 
			}
			else 
			{
				stream.ReadFixed(3, out this.lcr_global_reserved_zero_3bits, "lcr_global_reserved_zero_3bits"); 
			}
			stream.ReadFixed(5, out this.lcr_global_reserved_zero_5bits, "lcr_global_reserved_zero_5bits"); 

			if ((lcr_aggregate_info_present_flag != 0))
			{
				LcrAggregateInfo(); 
			}

			if ((lcr_seq_profile_tier_level_info_present_flag != 0))
			{

				for (i = 0; (i < LcrMaxNumXLayerCount); i++)
				{
					LcrSeqProfileTierLevelInfo(LcrXLayerID[i]); 
				}
			}

			if ((lcr_global_payload_present_flag != 0))
			{

				for (i = 0; (i < LcrMaxNumXLayerCount); i++)
				{
					stream.ReadLeb128( out this.lcr_data_size[i], "lcr_data_size"); 
					LcrGlobalPayload(LcrXLayerID[i], lcr_data_size[i]); 
				}
			}
        }

        private void WriteLcrGlobalInfo()
        {
			int i = 0;
			this.lcr_global_config_record_id = stream.Pick("lcr_global_config_record_id", _original != null ? _original.lcr_global_config_record_id : this.lcr_global_config_record_id, _edited != null ? _edited.lcr_global_config_record_id : _original != null ? _original.lcr_global_config_record_id : this.lcr_global_config_record_id);
			stream.WriteFixed(3, this.lcr_global_config_record_id, "lcr_global_config_record_id"); 
			this.lcr_xlayer_map = stream.Pick("lcr_xlayer_map", _original != null ? _original.lcr_xlayer_map : this.lcr_xlayer_map, _edited != null ? _edited.lcr_xlayer_map : _original != null ? _original.lcr_xlayer_map : this.lcr_xlayer_map);
			stream.WriteFixed(31, this.lcr_xlayer_map, "lcr_xlayer_map"); 
			LcrMaxNumXLayerCount = 0;

			for (i = 0; (i < 31); i++)
			{

				if (((lcr_xlayer_map & (1 << i)) != 0))
				{
					LcrXLayerID[LcrMaxNumXLayerCount] = i;
					LcrMaxNumXLayerCount++;
				}
			}
			this.lcr_aggregate_info_present_flag = stream.Pick("lcr_aggregate_info_present_flag", _original != null ? _original.lcr_aggregate_info_present_flag : this.lcr_aggregate_info_present_flag, _edited != null ? _edited.lcr_aggregate_info_present_flag : _original != null ? _original.lcr_aggregate_info_present_flag : this.lcr_aggregate_info_present_flag);
			stream.WriteFixed(1, this.lcr_aggregate_info_present_flag, "lcr_aggregate_info_present_flag"); 
			this.lcr_seq_profile_tier_level_info_present_flag = stream.Pick("lcr_seq_profile_tier_level_info_present_flag", _original != null ? _original.lcr_seq_profile_tier_level_info_present_flag : this.lcr_seq_profile_tier_level_info_present_flag, _edited != null ? _edited.lcr_seq_profile_tier_level_info_present_flag : _original != null ? _original.lcr_seq_profile_tier_level_info_present_flag : this.lcr_seq_profile_tier_level_info_present_flag);
			stream.WriteFixed(1, this.lcr_seq_profile_tier_level_info_present_flag, "lcr_seq_profile_tier_level_info_present_flag"); 
			this.lcr_global_payload_present_flag = stream.Pick("lcr_global_payload_present_flag", _original != null ? _original.lcr_global_payload_present_flag : this.lcr_global_payload_present_flag, _edited != null ? _edited.lcr_global_payload_present_flag : _original != null ? _original.lcr_global_payload_present_flag : this.lcr_global_payload_present_flag);
			stream.WriteFixed(1, this.lcr_global_payload_present_flag, "lcr_global_payload_present_flag"); 
			this.lcr_dependent_xlayers_flag = stream.Pick("lcr_dependent_xlayers_flag", _original != null ? _original.lcr_dependent_xlayers_flag : this.lcr_dependent_xlayers_flag, _edited != null ? _edited.lcr_dependent_xlayers_flag : _original != null ? _original.lcr_dependent_xlayers_flag : this.lcr_dependent_xlayers_flag);
			stream.WriteFixed(1, this.lcr_dependent_xlayers_flag, "lcr_dependent_xlayers_flag"); 
			this.lcr_global_atlas_id_present_flag = stream.Pick("lcr_global_atlas_id_present_flag", _original != null ? _original.lcr_global_atlas_id_present_flag : this.lcr_global_atlas_id_present_flag, _edited != null ? _edited.lcr_global_atlas_id_present_flag : _original != null ? _original.lcr_global_atlas_id_present_flag : this.lcr_global_atlas_id_present_flag);
			stream.WriteFixed(1, this.lcr_global_atlas_id_present_flag, "lcr_global_atlas_id_present_flag"); 
			this.lcr_global_purpose_id = stream.Pick("lcr_global_purpose_id", _original != null ? _original.lcr_global_purpose_id : this.lcr_global_purpose_id, _edited != null ? _edited.lcr_global_purpose_id : _original != null ? _original.lcr_global_purpose_id : this.lcr_global_purpose_id);
			stream.WriteFixed(7, this.lcr_global_purpose_id, "lcr_global_purpose_id"); 
			this.lcr_doh_constraint_flag = stream.Pick("lcr_doh_constraint_flag", _original != null ? _original.lcr_doh_constraint_flag : this.lcr_doh_constraint_flag, _edited != null ? _edited.lcr_doh_constraint_flag : _original != null ? _original.lcr_doh_constraint_flag : this.lcr_doh_constraint_flag);
			stream.WriteFixed(1, this.lcr_doh_constraint_flag, "lcr_doh_constraint_flag"); 
			this.lcr_enforce_tile_alignment_flag = stream.Pick("lcr_enforce_tile_alignment_flag", _original != null ? _original.lcr_enforce_tile_alignment_flag : this.lcr_enforce_tile_alignment_flag, _edited != null ? _edited.lcr_enforce_tile_alignment_flag : _original != null ? _original.lcr_enforce_tile_alignment_flag : this.lcr_enforce_tile_alignment_flag);
			stream.WriteFixed(1, this.lcr_enforce_tile_alignment_flag, "lcr_enforce_tile_alignment_flag"); 

			if ((lcr_global_atlas_id_present_flag != 0))
			{
				this.lcr_global_atlas_id = stream.Pick("lcr_global_atlas_id", _original != null ? _original.lcr_global_atlas_id : this.lcr_global_atlas_id, _edited != null ? _edited.lcr_global_atlas_id : _original != null ? _original.lcr_global_atlas_id : this.lcr_global_atlas_id);
				stream.WriteFixed(3, this.lcr_global_atlas_id, "lcr_global_atlas_id"); 
			}
			else 
			{
				this.lcr_global_reserved_zero_3bits = stream.Pick("lcr_global_reserved_zero_3bits", _original != null ? _original.lcr_global_reserved_zero_3bits : this.lcr_global_reserved_zero_3bits, _edited != null ? _edited.lcr_global_reserved_zero_3bits : _original != null ? _original.lcr_global_reserved_zero_3bits : this.lcr_global_reserved_zero_3bits);
				stream.WriteFixed(3, this.lcr_global_reserved_zero_3bits, "lcr_global_reserved_zero_3bits"); 
			}
			this.lcr_global_reserved_zero_5bits = stream.Pick("lcr_global_reserved_zero_5bits", _original != null ? _original.lcr_global_reserved_zero_5bits : this.lcr_global_reserved_zero_5bits, _edited != null ? _edited.lcr_global_reserved_zero_5bits : _original != null ? _original.lcr_global_reserved_zero_5bits : this.lcr_global_reserved_zero_5bits);
			stream.WriteFixed(5, this.lcr_global_reserved_zero_5bits, "lcr_global_reserved_zero_5bits"); 

			if ((lcr_aggregate_info_present_flag != 0))
			{
				WriteLcrAggregateInfo(); 
			}

			if ((lcr_seq_profile_tier_level_info_present_flag != 0))
			{

				for (i = 0; (i < LcrMaxNumXLayerCount); i++)
				{
					WriteLcrSeqProfileTierLevelInfo(LcrXLayerID[i]); 
				}
			}

			if ((lcr_global_payload_present_flag != 0))
			{

				for (i = 0; (i < LcrMaxNumXLayerCount); i++)
				{
					this.lcr_data_size[i] = stream.Pick("lcr_data_size", _original != null ? _original.lcr_data_size[i] : this.lcr_data_size[i], _edited != null ? _edited.lcr_data_size[i] : _original != null ? _original.lcr_data_size[i] : this.lcr_data_size[i]);
					stream.WriteLeb128( this.lcr_data_size[i], "lcr_data_size"); 
					WriteLcrGlobalPayload(LcrXLayerID[i], lcr_data_size[i]); 
				}
			}
        }

    /*
lcr_local_info( xlayerId ) {
lcr_global_id[ xlayerId ]	f(3)
lcr_local_id[ xlayerId ]	f(3)
lcr_profile_tier_level_info_present_flag[ xlayerId ]	f(1)
lcr_local_atlas_id_present_flag[ xlayerId ]	f(1)
if ( lcr_profile_tier_level_info_present_flag[ xlayerId ] ) {	
lcr_seq_profile_tier_level_info( xlayerId )	
}	
if ( lcr_local_atlas_id_present_flag[ xlayerId ] ) {	
lcr_local_atlas_id[ xlayerId ]	f(3)
} else {	
lcr_local_reserved_zero_3bits[ xlayerId ]	f(3)
}	
lcr_local_reserved_zero_5bits[ xlayerId ]	f(5)
lcr_xlayer_info( 0, xlayerId )	
}
    */
		private int xlayerId;
		public int _XlayerId { get { return xlayerId; } set { xlayerId = value; } }
		private AomArray<int> lcr_global_id = new AomArray<int>();
		public AomArray<int> _LcrGlobalId { get { return lcr_global_id; } set { lcr_global_id = value; } }
		private AomArray<int> lcr_local_id = new AomArray<int>();
		public AomArray<int> _LcrLocalId { get { return lcr_local_id; } set { lcr_local_id = value; } }
		private AomArray<int> lcr_profile_tier_level_info_present_flag = new AomArray<int>();
		public AomArray<int> _LcrProfileTierLevelInfoPresentFlag { get { return lcr_profile_tier_level_info_present_flag; } set { lcr_profile_tier_level_info_present_flag = value; } }
		private AomArray<int> lcr_local_atlas_id_present_flag = new AomArray<int>();
		public AomArray<int> _LcrLocalAtlasIdPresentFlag { get { return lcr_local_atlas_id_present_flag; } set { lcr_local_atlas_id_present_flag = value; } }
		private AomArray<int> lcr_local_atlas_id = new AomArray<int>();
		public AomArray<int> _LcrLocalAtlasId { get { return lcr_local_atlas_id; } set { lcr_local_atlas_id = value; } }
		private AomArray<int> lcr_local_reserved_zero_3bits = new AomArray<int>();
		public AomArray<int> _LcrLocalReservedZero3bits { get { return lcr_local_reserved_zero_3bits; } set { lcr_local_reserved_zero_3bits = value; } }
		private AomArray<int> lcr_local_reserved_zero_5bits = new AomArray<int>();
		public AomArray<int> _LcrLocalReservedZero5bits { get { return lcr_local_reserved_zero_5bits; } set { lcr_local_reserved_zero_5bits = value; } }

        private void LcrLocalInfo(int xlayerId)
        {
			stream.ReadFixed(3, out this.lcr_global_id[xlayerId], "lcr_global_id"); 
			stream.ReadFixed(3, out this.lcr_local_id[xlayerId], "lcr_local_id"); 
			stream.ReadFixed(1, out this.lcr_profile_tier_level_info_present_flag[xlayerId], "lcr_profile_tier_level_info_present_flag"); 
			stream.ReadFixed(1, out this.lcr_local_atlas_id_present_flag[xlayerId], "lcr_local_atlas_id_present_flag"); 

			if ((lcr_profile_tier_level_info_present_flag[xlayerId] != 0))
			{
				LcrSeqProfileTierLevelInfo(xlayerId); 
			}

			if ((lcr_local_atlas_id_present_flag[xlayerId] != 0))
			{
				stream.ReadFixed(3, out this.lcr_local_atlas_id[xlayerId], "lcr_local_atlas_id"); 
			}
			else 
			{
				stream.ReadFixed(3, out this.lcr_local_reserved_zero_3bits[xlayerId], "lcr_local_reserved_zero_3bits"); 
			}
			stream.ReadFixed(5, out this.lcr_local_reserved_zero_5bits[xlayerId], "lcr_local_reserved_zero_5bits"); 
			LcrXlayerInfo(0, xlayerId); 
        }

        private void WriteLcrLocalInfo(int xlayerId)
        {
			this.lcr_global_id[xlayerId] = stream.Pick("lcr_global_id", _original != null ? _original.lcr_global_id[xlayerId] : this.lcr_global_id[xlayerId], _edited != null ? _edited.lcr_global_id[xlayerId] : _original != null ? _original.lcr_global_id[xlayerId] : this.lcr_global_id[xlayerId]);
			stream.WriteFixed(3, this.lcr_global_id[xlayerId], "lcr_global_id"); 
			this.lcr_local_id[xlayerId] = stream.Pick("lcr_local_id", _original != null ? _original.lcr_local_id[xlayerId] : this.lcr_local_id[xlayerId], _edited != null ? _edited.lcr_local_id[xlayerId] : _original != null ? _original.lcr_local_id[xlayerId] : this.lcr_local_id[xlayerId]);
			stream.WriteFixed(3, this.lcr_local_id[xlayerId], "lcr_local_id"); 
			this.lcr_profile_tier_level_info_present_flag[xlayerId] = stream.Pick("lcr_profile_tier_level_info_present_flag", _original != null ? _original.lcr_profile_tier_level_info_present_flag[xlayerId] : this.lcr_profile_tier_level_info_present_flag[xlayerId], _edited != null ? _edited.lcr_profile_tier_level_info_present_flag[xlayerId] : _original != null ? _original.lcr_profile_tier_level_info_present_flag[xlayerId] : this.lcr_profile_tier_level_info_present_flag[xlayerId]);
			stream.WriteFixed(1, this.lcr_profile_tier_level_info_present_flag[xlayerId], "lcr_profile_tier_level_info_present_flag"); 
			this.lcr_local_atlas_id_present_flag[xlayerId] = stream.Pick("lcr_local_atlas_id_present_flag", _original != null ? _original.lcr_local_atlas_id_present_flag[xlayerId] : this.lcr_local_atlas_id_present_flag[xlayerId], _edited != null ? _edited.lcr_local_atlas_id_present_flag[xlayerId] : _original != null ? _original.lcr_local_atlas_id_present_flag[xlayerId] : this.lcr_local_atlas_id_present_flag[xlayerId]);
			stream.WriteFixed(1, this.lcr_local_atlas_id_present_flag[xlayerId], "lcr_local_atlas_id_present_flag"); 

			if ((lcr_profile_tier_level_info_present_flag[xlayerId] != 0))
			{
				WriteLcrSeqProfileTierLevelInfo(xlayerId); 
			}

			if ((lcr_local_atlas_id_present_flag[xlayerId] != 0))
			{
				this.lcr_local_atlas_id[xlayerId] = stream.Pick("lcr_local_atlas_id", _original != null ? _original.lcr_local_atlas_id[xlayerId] : this.lcr_local_atlas_id[xlayerId], _edited != null ? _edited.lcr_local_atlas_id[xlayerId] : _original != null ? _original.lcr_local_atlas_id[xlayerId] : this.lcr_local_atlas_id[xlayerId]);
				stream.WriteFixed(3, this.lcr_local_atlas_id[xlayerId], "lcr_local_atlas_id"); 
			}
			else 
			{
				this.lcr_local_reserved_zero_3bits[xlayerId] = stream.Pick("lcr_local_reserved_zero_3bits", _original != null ? _original.lcr_local_reserved_zero_3bits[xlayerId] : this.lcr_local_reserved_zero_3bits[xlayerId], _edited != null ? _edited.lcr_local_reserved_zero_3bits[xlayerId] : _original != null ? _original.lcr_local_reserved_zero_3bits[xlayerId] : this.lcr_local_reserved_zero_3bits[xlayerId]);
				stream.WriteFixed(3, this.lcr_local_reserved_zero_3bits[xlayerId], "lcr_local_reserved_zero_3bits"); 
			}
			this.lcr_local_reserved_zero_5bits[xlayerId] = stream.Pick("lcr_local_reserved_zero_5bits", _original != null ? _original.lcr_local_reserved_zero_5bits[xlayerId] : this.lcr_local_reserved_zero_5bits[xlayerId], _edited != null ? _edited.lcr_local_reserved_zero_5bits[xlayerId] : _original != null ? _original.lcr_local_reserved_zero_5bits[xlayerId] : this.lcr_local_reserved_zero_5bits[xlayerId]);
			stream.WriteFixed(5, this.lcr_local_reserved_zero_5bits[xlayerId], "lcr_local_reserved_zero_5bits"); 
			WriteLcrXlayerInfo(0, xlayerId); 
        }

    /*
lcr_aggregate_info() {
lcr_config_idc	f(6)
lcr_aggregate_level_idx	f(5)
lcr_max_tier_flag	f(1)
lcr_max_interop	f(4)
}
    */
		private int lcr_config_idc;
		public int _LcrConfigIdc { get { return lcr_config_idc; } set { lcr_config_idc = value; } }
		private int lcr_aggregate_level_idx;
		public int _LcrAggregateLevelIdx { get { return lcr_aggregate_level_idx; } set { lcr_aggregate_level_idx = value; } }
		private int lcr_max_tier_flag;
		public int _LcrMaxTierFlag { get { return lcr_max_tier_flag; } set { lcr_max_tier_flag = value; } }
		private int lcr_max_interop;
		public int _LcrMaxInterop { get { return lcr_max_interop; } set { lcr_max_interop = value; } }

        private void LcrAggregateInfo()
        {
			stream.ReadFixed(6, out this.lcr_config_idc, "lcr_config_idc"); 
			stream.ReadFixed(5, out this.lcr_aggregate_level_idx, "lcr_aggregate_level_idx"); 
			stream.ReadFixed(1, out this.lcr_max_tier_flag, "lcr_max_tier_flag"); 
			stream.ReadFixed(4, out this.lcr_max_interop, "lcr_max_interop"); 
        }

        private void WriteLcrAggregateInfo()
        {
			this.lcr_config_idc = stream.Pick("lcr_config_idc", _original != null ? _original.lcr_config_idc : this.lcr_config_idc, _edited != null ? _edited.lcr_config_idc : _original != null ? _original.lcr_config_idc : this.lcr_config_idc);
			stream.WriteFixed(6, this.lcr_config_idc, "lcr_config_idc"); 
			this.lcr_aggregate_level_idx = stream.Pick("lcr_aggregate_level_idx", _original != null ? _original.lcr_aggregate_level_idx : this.lcr_aggregate_level_idx, _edited != null ? _edited.lcr_aggregate_level_idx : _original != null ? _original.lcr_aggregate_level_idx : this.lcr_aggregate_level_idx);
			stream.WriteFixed(5, this.lcr_aggregate_level_idx, "lcr_aggregate_level_idx"); 
			this.lcr_max_tier_flag = stream.Pick("lcr_max_tier_flag", _original != null ? _original.lcr_max_tier_flag : this.lcr_max_tier_flag, _edited != null ? _edited.lcr_max_tier_flag : _original != null ? _original.lcr_max_tier_flag : this.lcr_max_tier_flag);
			stream.WriteFixed(1, this.lcr_max_tier_flag, "lcr_max_tier_flag"); 
			this.lcr_max_interop = stream.Pick("lcr_max_interop", _original != null ? _original.lcr_max_interop : this.lcr_max_interop, _edited != null ? _edited.lcr_max_interop : _original != null ? _original.lcr_max_interop : this.lcr_max_interop);
			stream.WriteFixed(4, this.lcr_max_interop, "lcr_max_interop"); 
        }

    /*
lcr_seq_profile_tier_level_info( i ) {
lcr_seq_profile_idc[ i ]	f(5)
lcr_max_level_idx[ i ]	f(5)
lcr_tier_flag[ i ]	f(1)
lcr_max_mlayer_count[ i ]	f(3)
lsptli_reserved_2bits	f(2)
}
    */
		private AomArray<int> lcr_seq_profile_idc = new AomArray<int>();
		public AomArray<int> _LcrSeqProfileIdc { get { return lcr_seq_profile_idc; } set { lcr_seq_profile_idc = value; } }
		private AomArray<int> lcr_max_level_idx = new AomArray<int>();
		public AomArray<int> _LcrMaxLevelIdx { get { return lcr_max_level_idx; } set { lcr_max_level_idx = value; } }
		private AomArray<int> lcr_tier_flag = new AomArray<int>();
		public AomArray<int> _LcrTierFlag { get { return lcr_tier_flag; } set { lcr_tier_flag = value; } }
		private AomArray<int> lcr_max_mlayer_count = new AomArray<int>();
		public AomArray<int> _LcrMaxMlayerCount { get { return lcr_max_mlayer_count; } set { lcr_max_mlayer_count = value; } }
		private int lsptli_reserved_2bits;
		public int _LsptliReserved2bits { get { return lsptli_reserved_2bits; } set { lsptli_reserved_2bits = value; } }

        private void LcrSeqProfileTierLevelInfo(int i)
        {
			stream.ReadFixed(5, out this.lcr_seq_profile_idc[i], "lcr_seq_profile_idc"); 
			stream.ReadFixed(5, out this.lcr_max_level_idx[i], "lcr_max_level_idx"); 
			stream.ReadFixed(1, out this.lcr_tier_flag[i], "lcr_tier_flag"); 
			stream.ReadFixed(3, out this.lcr_max_mlayer_count[i], "lcr_max_mlayer_count"); 
			stream.ReadFixed(2, out this.lsptli_reserved_2bits, "lsptli_reserved_2bits"); 
        }

        private void WriteLcrSeqProfileTierLevelInfo(int i)
        {
			this.lcr_seq_profile_idc[i] = stream.Pick("lcr_seq_profile_idc", _original != null ? _original.lcr_seq_profile_idc[i] : this.lcr_seq_profile_idc[i], _edited != null ? _edited.lcr_seq_profile_idc[i] : _original != null ? _original.lcr_seq_profile_idc[i] : this.lcr_seq_profile_idc[i]);
			stream.WriteFixed(5, this.lcr_seq_profile_idc[i], "lcr_seq_profile_idc"); 
			this.lcr_max_level_idx[i] = stream.Pick("lcr_max_level_idx", _original != null ? _original.lcr_max_level_idx[i] : this.lcr_max_level_idx[i], _edited != null ? _edited.lcr_max_level_idx[i] : _original != null ? _original.lcr_max_level_idx[i] : this.lcr_max_level_idx[i]);
			stream.WriteFixed(5, this.lcr_max_level_idx[i], "lcr_max_level_idx"); 
			this.lcr_tier_flag[i] = stream.Pick("lcr_tier_flag", _original != null ? _original.lcr_tier_flag[i] : this.lcr_tier_flag[i], _edited != null ? _edited.lcr_tier_flag[i] : _original != null ? _original.lcr_tier_flag[i] : this.lcr_tier_flag[i]);
			stream.WriteFixed(1, this.lcr_tier_flag[i], "lcr_tier_flag"); 
			this.lcr_max_mlayer_count[i] = stream.Pick("lcr_max_mlayer_count", _original != null ? _original.lcr_max_mlayer_count[i] : this.lcr_max_mlayer_count[i], _edited != null ? _edited.lcr_max_mlayer_count[i] : _original != null ? _original.lcr_max_mlayer_count[i] : this.lcr_max_mlayer_count[i]);
			stream.WriteFixed(3, this.lcr_max_mlayer_count[i], "lcr_max_mlayer_count"); 
			this.lsptli_reserved_2bits = stream.Pick("lsptli_reserved_2bits", _original != null ? _original.lsptli_reserved_2bits : this.lsptli_reserved_2bits, _edited != null ? _edited.lsptli_reserved_2bits : _original != null ? _original.lsptli_reserved_2bits : this.lsptli_reserved_2bits);
			stream.WriteFixed(2, this.lsptli_reserved_2bits, "lsptli_reserved_2bits"); 
        }

    /*
lcr_global_payload( n, sz ) {
startPosition = get_position()	
if ( lcr_dependent_xlayers_flag && n > 0 ) {	
lcr_num_dependent_xlayer_map[ n ]	f(n)
}	
lcr_xlayer_info( 1 , n )	
currentPosition = get_position()	
parsedPayloadBits = currentPosition - startPosition	
RemainingLcrPayloadBits = sz * 8 - parsedPayloadBits	
for ( j = 0; j < RemainingLcrPayloadBits; j++ ) {	
lcr_remaining_payload_bit	f(1)
}	
}
    */
		private int n;
		private AomArray<int> lcr_num_dependent_xlayer_map = new AomArray<int>();
		public AomArray<int> _LcrNumDependentXlayerMap { get { return lcr_num_dependent_xlayer_map; } set { lcr_num_dependent_xlayer_map = value; } }
		private int RemainingLcrPayloadBits;
		public int _RemainingLcrPayloadBits { get { return RemainingLcrPayloadBits; } set { RemainingLcrPayloadBits = value; } }
		private int lcr_remaining_payload_bit;
		public int _LcrRemainingPayloadBit { get { return lcr_remaining_payload_bit; } set { lcr_remaining_payload_bit = value; } }

        private void LcrGlobalPayload(int n, int sz)
        {
			int j = 0;
			int startPosition = 0;
			int currentPosition = 0;
			int parsedPayloadBits = 0;
			startPosition = get_position();

			if (((lcr_dependent_xlayers_flag != 0) && (n > 0)))
			{
				stream.ReadVariable(n, out this.lcr_num_dependent_xlayer_map[n], "lcr_num_dependent_xlayer_map"); 
			}
			LcrXlayerInfo(1, n); 
			currentPosition = get_position();
			parsedPayloadBits = (currentPosition - startPosition);
			RemainingLcrPayloadBits = ((sz * 8) - parsedPayloadBits);

			for (j = 0; (j < RemainingLcrPayloadBits); j++)
			{
				stream.ReadFixed(1, out this.lcr_remaining_payload_bit, "lcr_remaining_payload_bit"); 
			}
        }

        private void WriteLcrGlobalPayload(int n, int sz)
        {
			int j = 0;
			int startPosition = 0;
			int currentPosition = 0;
			int parsedPayloadBits = 0;
			startPosition = get_position();

			if (((lcr_dependent_xlayers_flag != 0) && (n > 0)))
			{
				this.lcr_num_dependent_xlayer_map[n] = stream.Pick("lcr_num_dependent_xlayer_map", _original != null ? _original.lcr_num_dependent_xlayer_map[n] : this.lcr_num_dependent_xlayer_map[n], _edited != null ? _edited.lcr_num_dependent_xlayer_map[n] : _original != null ? _original.lcr_num_dependent_xlayer_map[n] : this.lcr_num_dependent_xlayer_map[n]);
				stream.WriteVariable(n, this.lcr_num_dependent_xlayer_map[n], "lcr_num_dependent_xlayer_map"); 
			}
			WriteLcrXlayerInfo(1, n); 
			currentPosition = get_position();
			parsedPayloadBits = (currentPosition - startPosition);
			RemainingLcrPayloadBits = ((sz * 8) - parsedPayloadBits);

			for (j = 0; (j < RemainingLcrPayloadBits); j++)
			{
				this.lcr_remaining_payload_bit = stream.Pick("lcr_remaining_payload_bit", _original != null ? _original.lcr_remaining_payload_bit : this.lcr_remaining_payload_bit, _edited != null ? _edited.lcr_remaining_payload_bit : _original != null ? _original.lcr_remaining_payload_bit : this.lcr_remaining_payload_bit);
				stream.WriteFixed(1, this.lcr_remaining_payload_bit, "lcr_remaining_payload_bit"); 
			}
        }

    /*
lcr_xlayer_info( isGlobal, xId ) {
lcr_rep_info_present_flag[ isGlobal ][ xId ]	f(1)
lcr_xlayer_purpose_present_flag[ isGlobal ][ xId ]	f(1)
lcr_xlayer_color_info_present_flag[ isGlobal ][ xId ]	f(1)
lcr_embedded_layer_info_present_flag[ isGlobal ][ xId ]	f(1)
if ( lcr_rep_info_present_flag[ isGlobal ][ xId ] ) {	
lcr_rep_info( isGlobal, xId )	
}	
if( lcr_xlayer_purpose_present_flag[ isGlobal ][ xId ] ) {	
lcr_xlayer_purpose_id[ isGlobal ][ xId ]	f(7)
}	
if( lcr_xlayer_color_info_present_flag[ isGlobal ][ xId ] ) {	
lcr_xlayer_color_info( isGlobal, xId )	
}	
byte_alignment()	
if ( lcr_embedded_layer_info_present_flag[ isGlobal ][ xId ] ) {	
lcr_embedded_layer_info( isGlobal, xId )	
} else {	
if ( isGlobal && lcr_global_atlas_id_present_flag ) {	
lcr_xlayer_atlas_segment_id[ xId ]	f(8)
lcr_xlayer_priority_order[ xId ]	f(8)
lcr_xlayer_rendering_method[ xId ]	f(8)
}	
}	
}
    */
		private int isGlobal;
		public int _IsGlobal { get { return isGlobal; } set { isGlobal = value; } }
		private int xId;
		public int _XId { get { return xId; } set { xId = value; } }
		private AomArray<AomArray<int>> lcr_rep_info_present_flag = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _LcrRepInfoPresentFlag { get { return lcr_rep_info_present_flag; } set { lcr_rep_info_present_flag = value; } }
		private AomArray<AomArray<int>> lcr_xlayer_purpose_present_flag = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _LcrXlayerPurposePresentFlag { get { return lcr_xlayer_purpose_present_flag; } set { lcr_xlayer_purpose_present_flag = value; } }
		private AomArray<AomArray<int>> lcr_xlayer_color_info_present_flag = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _LcrXlayerColorInfoPresentFlag { get { return lcr_xlayer_color_info_present_flag; } set { lcr_xlayer_color_info_present_flag = value; } }
		private AomArray<AomArray<int>> lcr_embedded_layer_info_present_flag = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _LcrEmbeddedLayerInfoPresentFlag { get { return lcr_embedded_layer_info_present_flag; } set { lcr_embedded_layer_info_present_flag = value; } }
		private AomArray<AomArray<int>> lcr_xlayer_purpose_id = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _LcrXlayerPurposeId { get { return lcr_xlayer_purpose_id; } set { lcr_xlayer_purpose_id = value; } }
		private AomArray<int> lcr_xlayer_atlas_segment_id = new AomArray<int>();
		public AomArray<int> _LcrXlayerAtlasSegmentId { get { return lcr_xlayer_atlas_segment_id; } set { lcr_xlayer_atlas_segment_id = value; } }
		private AomArray<int> lcr_xlayer_priority_order = new AomArray<int>();
		public AomArray<int> _LcrXlayerPriorityOrder { get { return lcr_xlayer_priority_order; } set { lcr_xlayer_priority_order = value; } }
		private AomArray<int> lcr_xlayer_rendering_method = new AomArray<int>();
		public AomArray<int> _LcrXlayerRenderingMethod { get { return lcr_xlayer_rendering_method; } set { lcr_xlayer_rendering_method = value; } }

        private void LcrXlayerInfo(int isGlobal, int xId)
        {
			stream.ReadFixed(1, out this.lcr_rep_info_present_flag[isGlobal][xId], "lcr_rep_info_present_flag"); 
			stream.ReadFixed(1, out this.lcr_xlayer_purpose_present_flag[isGlobal][xId], "lcr_xlayer_purpose_present_flag"); 
			stream.ReadFixed(1, out this.lcr_xlayer_color_info_present_flag[isGlobal][xId], "lcr_xlayer_color_info_present_flag"); 
			stream.ReadFixed(1, out this.lcr_embedded_layer_info_present_flag[isGlobal][xId], "lcr_embedded_layer_info_present_flag"); 

			if ((lcr_rep_info_present_flag[isGlobal][xId] != 0))
			{
				LcrRepInfo(isGlobal, xId); 
			}

			if ((lcr_xlayer_purpose_present_flag[isGlobal][xId] != 0))
			{
				stream.ReadFixed(7, out this.lcr_xlayer_purpose_id[isGlobal][xId], "lcr_xlayer_purpose_id"); 
			}

			if ((lcr_xlayer_color_info_present_flag[isGlobal][xId] != 0))
			{
				LcrXlayerColorInfo(isGlobal, xId); 
			}
			ByteAlignment(); 

			if ((lcr_embedded_layer_info_present_flag[isGlobal][xId] != 0))
			{
				LcrEmbeddedLayerInfo(isGlobal, xId); 
			}
			else 
			{

				if (((isGlobal != 0) && (lcr_global_atlas_id_present_flag != 0)))
				{
					stream.ReadFixed(8, out this.lcr_xlayer_atlas_segment_id[xId], "lcr_xlayer_atlas_segment_id"); 
					stream.ReadFixed(8, out this.lcr_xlayer_priority_order[xId], "lcr_xlayer_priority_order"); 
					stream.ReadFixed(8, out this.lcr_xlayer_rendering_method[xId], "lcr_xlayer_rendering_method"); 
				}
			}
        }

        private void WriteLcrXlayerInfo(int isGlobal, int xId)
        {
			this.lcr_rep_info_present_flag[isGlobal][xId] = stream.Pick("lcr_rep_info_present_flag", _original != null ? _original.lcr_rep_info_present_flag[isGlobal][xId] : this.lcr_rep_info_present_flag[isGlobal][xId], _edited != null ? _edited.lcr_rep_info_present_flag[isGlobal][xId] : _original != null ? _original.lcr_rep_info_present_flag[isGlobal][xId] : this.lcr_rep_info_present_flag[isGlobal][xId]);
			stream.WriteFixed(1, this.lcr_rep_info_present_flag[isGlobal][xId], "lcr_rep_info_present_flag"); 
			this.lcr_xlayer_purpose_present_flag[isGlobal][xId] = stream.Pick("lcr_xlayer_purpose_present_flag", _original != null ? _original.lcr_xlayer_purpose_present_flag[isGlobal][xId] : this.lcr_xlayer_purpose_present_flag[isGlobal][xId], _edited != null ? _edited.lcr_xlayer_purpose_present_flag[isGlobal][xId] : _original != null ? _original.lcr_xlayer_purpose_present_flag[isGlobal][xId] : this.lcr_xlayer_purpose_present_flag[isGlobal][xId]);
			stream.WriteFixed(1, this.lcr_xlayer_purpose_present_flag[isGlobal][xId], "lcr_xlayer_purpose_present_flag"); 
			this.lcr_xlayer_color_info_present_flag[isGlobal][xId] = stream.Pick("lcr_xlayer_color_info_present_flag", _original != null ? _original.lcr_xlayer_color_info_present_flag[isGlobal][xId] : this.lcr_xlayer_color_info_present_flag[isGlobal][xId], _edited != null ? _edited.lcr_xlayer_color_info_present_flag[isGlobal][xId] : _original != null ? _original.lcr_xlayer_color_info_present_flag[isGlobal][xId] : this.lcr_xlayer_color_info_present_flag[isGlobal][xId]);
			stream.WriteFixed(1, this.lcr_xlayer_color_info_present_flag[isGlobal][xId], "lcr_xlayer_color_info_present_flag"); 
			this.lcr_embedded_layer_info_present_flag[isGlobal][xId] = stream.Pick("lcr_embedded_layer_info_present_flag", _original != null ? _original.lcr_embedded_layer_info_present_flag[isGlobal][xId] : this.lcr_embedded_layer_info_present_flag[isGlobal][xId], _edited != null ? _edited.lcr_embedded_layer_info_present_flag[isGlobal][xId] : _original != null ? _original.lcr_embedded_layer_info_present_flag[isGlobal][xId] : this.lcr_embedded_layer_info_present_flag[isGlobal][xId]);
			stream.WriteFixed(1, this.lcr_embedded_layer_info_present_flag[isGlobal][xId], "lcr_embedded_layer_info_present_flag"); 

			if ((lcr_rep_info_present_flag[isGlobal][xId] != 0))
			{
				WriteLcrRepInfo(isGlobal, xId); 
			}

			if ((lcr_xlayer_purpose_present_flag[isGlobal][xId] != 0))
			{
				this.lcr_xlayer_purpose_id[isGlobal][xId] = stream.Pick("lcr_xlayer_purpose_id", _original != null ? _original.lcr_xlayer_purpose_id[isGlobal][xId] : this.lcr_xlayer_purpose_id[isGlobal][xId], _edited != null ? _edited.lcr_xlayer_purpose_id[isGlobal][xId] : _original != null ? _original.lcr_xlayer_purpose_id[isGlobal][xId] : this.lcr_xlayer_purpose_id[isGlobal][xId]);
				stream.WriteFixed(7, this.lcr_xlayer_purpose_id[isGlobal][xId], "lcr_xlayer_purpose_id"); 
			}

			if ((lcr_xlayer_color_info_present_flag[isGlobal][xId] != 0))
			{
				WriteLcrXlayerColorInfo(isGlobal, xId); 
			}
			WriteByteAlignment(); 

			if ((lcr_embedded_layer_info_present_flag[isGlobal][xId] != 0))
			{
				WriteLcrEmbeddedLayerInfo(isGlobal, xId); 
			}
			else 
			{

				if (((isGlobal != 0) && (lcr_global_atlas_id_present_flag != 0)))
				{
					this.lcr_xlayer_atlas_segment_id[xId] = stream.Pick("lcr_xlayer_atlas_segment_id", _original != null ? _original.lcr_xlayer_atlas_segment_id[xId] : this.lcr_xlayer_atlas_segment_id[xId], _edited != null ? _edited.lcr_xlayer_atlas_segment_id[xId] : _original != null ? _original.lcr_xlayer_atlas_segment_id[xId] : this.lcr_xlayer_atlas_segment_id[xId]);
					stream.WriteFixed(8, this.lcr_xlayer_atlas_segment_id[xId], "lcr_xlayer_atlas_segment_id"); 
					this.lcr_xlayer_priority_order[xId] = stream.Pick("lcr_xlayer_priority_order", _original != null ? _original.lcr_xlayer_priority_order[xId] : this.lcr_xlayer_priority_order[xId], _edited != null ? _edited.lcr_xlayer_priority_order[xId] : _original != null ? _original.lcr_xlayer_priority_order[xId] : this.lcr_xlayer_priority_order[xId]);
					stream.WriteFixed(8, this.lcr_xlayer_priority_order[xId], "lcr_xlayer_priority_order"); 
					this.lcr_xlayer_rendering_method[xId] = stream.Pick("lcr_xlayer_rendering_method", _original != null ? _original.lcr_xlayer_rendering_method[xId] : this.lcr_xlayer_rendering_method[xId], _edited != null ? _edited.lcr_xlayer_rendering_method[xId] : _original != null ? _original.lcr_xlayer_rendering_method[xId] : this.lcr_xlayer_rendering_method[xId]);
					stream.WriteFixed(8, this.lcr_xlayer_rendering_method[xId], "lcr_xlayer_rendering_method"); 
				}
			}
        }

    /*
lcr_rep_info( isGlobal, xId ) {
lcr_max_pic_width[ isGlobal ][ xId ]	uvlc()
lcr_max_pic_height[ isGlobal ][ xId ]	uvlc()
lcr_format_info_present_flag[ isGlobal ][ xId ]	f(1)
lcr_cropping_window_present_flag[ isGlobal ][ xId ]	f(1)
if ( lcr_format_info_present_flag[ isGlobal ][ xId ] ) {	
lcr_bit_depth_idc[ isGlobal ][ xId ]	uvlc()
lcr_chroma_format_idc[ isGlobal ][ xId ]	uvlc()
}	
if ( lcr_cropping_window_present_flag[ isGlobal ][ xId ] ) {	
lcr_cropping_win_left_offset [ isGlobal ][ xId ]	uvlc()
lcr_cropping_win_right_offset[ isGlobal ][ xId ]	uvlc()
lcr_cropping_win_top_offset [ isGlobal ][ xId ]	uvlc()
lcr_cropping_win_bottom_offset[ isGlobal ][ xId ]	uvlc()
}	
}
    */
		private AomArray<AomArray<int>> lcr_max_pic_width = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _LcrMaxPicWidth { get { return lcr_max_pic_width; } set { lcr_max_pic_width = value; } }
		private AomArray<AomArray<int>> lcr_max_pic_height = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _LcrMaxPicHeight { get { return lcr_max_pic_height; } set { lcr_max_pic_height = value; } }
		private AomArray<AomArray<int>> lcr_format_info_present_flag = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _LcrFormatInfoPresentFlag { get { return lcr_format_info_present_flag; } set { lcr_format_info_present_flag = value; } }
		private AomArray<AomArray<int>> lcr_cropping_window_present_flag = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _LcrCroppingWindowPresentFlag { get { return lcr_cropping_window_present_flag; } set { lcr_cropping_window_present_flag = value; } }
		private AomArray<AomArray<int>> lcr_bit_depth_idc = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _LcrBitDepthIdc { get { return lcr_bit_depth_idc; } set { lcr_bit_depth_idc = value; } }
		private AomArray<AomArray<int>> lcr_chroma_format_idc = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _LcrChromaFormatIdc { get { return lcr_chroma_format_idc; } set { lcr_chroma_format_idc = value; } }
		private AomArray<AomArray<int>> lcr_cropping_win_left_offset = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _LcrCroppingWinLeftOffset { get { return lcr_cropping_win_left_offset; } set { lcr_cropping_win_left_offset = value; } }
		private AomArray<AomArray<int>> lcr_cropping_win_right_offset = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _LcrCroppingWinRightOffset { get { return lcr_cropping_win_right_offset; } set { lcr_cropping_win_right_offset = value; } }
		private AomArray<AomArray<int>> lcr_cropping_win_top_offset = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _LcrCroppingWinTopOffset { get { return lcr_cropping_win_top_offset; } set { lcr_cropping_win_top_offset = value; } }
		private AomArray<AomArray<int>> lcr_cropping_win_bottom_offset = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _LcrCroppingWinBottomOffset { get { return lcr_cropping_win_bottom_offset; } set { lcr_cropping_win_bottom_offset = value; } }

        private void LcrRepInfo(int isGlobal, int xId)
        {
			stream.ReadUvlc( out this.lcr_max_pic_width[isGlobal][xId], "lcr_max_pic_width"); 
			stream.ReadUvlc( out this.lcr_max_pic_height[isGlobal][xId], "lcr_max_pic_height"); 
			stream.ReadFixed(1, out this.lcr_format_info_present_flag[isGlobal][xId], "lcr_format_info_present_flag"); 
			stream.ReadFixed(1, out this.lcr_cropping_window_present_flag[isGlobal][xId], "lcr_cropping_window_present_flag"); 

			if ((lcr_format_info_present_flag[isGlobal][xId] != 0))
			{
				stream.ReadUvlc( out this.lcr_bit_depth_idc[isGlobal][xId], "lcr_bit_depth_idc"); 
				stream.ReadUvlc( out this.lcr_chroma_format_idc[isGlobal][xId], "lcr_chroma_format_idc"); 
			}

			if ((lcr_cropping_window_present_flag[isGlobal][xId] != 0))
			{
				stream.ReadUvlc( out this.lcr_cropping_win_left_offset[isGlobal][xId], "lcr_cropping_win_left_offset"); 
				stream.ReadUvlc( out this.lcr_cropping_win_right_offset[isGlobal][xId], "lcr_cropping_win_right_offset"); 
				stream.ReadUvlc( out this.lcr_cropping_win_top_offset[isGlobal][xId], "lcr_cropping_win_top_offset"); 
				stream.ReadUvlc( out this.lcr_cropping_win_bottom_offset[isGlobal][xId], "lcr_cropping_win_bottom_offset"); 
			}
        }

        private void WriteLcrRepInfo(int isGlobal, int xId)
        {
			this.lcr_max_pic_width[isGlobal][xId] = stream.Pick("lcr_max_pic_width", _original != null ? _original.lcr_max_pic_width[isGlobal][xId] : this.lcr_max_pic_width[isGlobal][xId], _edited != null ? _edited.lcr_max_pic_width[isGlobal][xId] : _original != null ? _original.lcr_max_pic_width[isGlobal][xId] : this.lcr_max_pic_width[isGlobal][xId]);
			stream.WriteUvlc( this.lcr_max_pic_width[isGlobal][xId], "lcr_max_pic_width"); 
			this.lcr_max_pic_height[isGlobal][xId] = stream.Pick("lcr_max_pic_height", _original != null ? _original.lcr_max_pic_height[isGlobal][xId] : this.lcr_max_pic_height[isGlobal][xId], _edited != null ? _edited.lcr_max_pic_height[isGlobal][xId] : _original != null ? _original.lcr_max_pic_height[isGlobal][xId] : this.lcr_max_pic_height[isGlobal][xId]);
			stream.WriteUvlc( this.lcr_max_pic_height[isGlobal][xId], "lcr_max_pic_height"); 
			this.lcr_format_info_present_flag[isGlobal][xId] = stream.Pick("lcr_format_info_present_flag", _original != null ? _original.lcr_format_info_present_flag[isGlobal][xId] : this.lcr_format_info_present_flag[isGlobal][xId], _edited != null ? _edited.lcr_format_info_present_flag[isGlobal][xId] : _original != null ? _original.lcr_format_info_present_flag[isGlobal][xId] : this.lcr_format_info_present_flag[isGlobal][xId]);
			stream.WriteFixed(1, this.lcr_format_info_present_flag[isGlobal][xId], "lcr_format_info_present_flag"); 
			this.lcr_cropping_window_present_flag[isGlobal][xId] = stream.Pick("lcr_cropping_window_present_flag", _original != null ? _original.lcr_cropping_window_present_flag[isGlobal][xId] : this.lcr_cropping_window_present_flag[isGlobal][xId], _edited != null ? _edited.lcr_cropping_window_present_flag[isGlobal][xId] : _original != null ? _original.lcr_cropping_window_present_flag[isGlobal][xId] : this.lcr_cropping_window_present_flag[isGlobal][xId]);
			stream.WriteFixed(1, this.lcr_cropping_window_present_flag[isGlobal][xId], "lcr_cropping_window_present_flag"); 

			if ((lcr_format_info_present_flag[isGlobal][xId] != 0))
			{
				this.lcr_bit_depth_idc[isGlobal][xId] = stream.Pick("lcr_bit_depth_idc", _original != null ? _original.lcr_bit_depth_idc[isGlobal][xId] : this.lcr_bit_depth_idc[isGlobal][xId], _edited != null ? _edited.lcr_bit_depth_idc[isGlobal][xId] : _original != null ? _original.lcr_bit_depth_idc[isGlobal][xId] : this.lcr_bit_depth_idc[isGlobal][xId]);
				stream.WriteUvlc( this.lcr_bit_depth_idc[isGlobal][xId], "lcr_bit_depth_idc"); 
				this.lcr_chroma_format_idc[isGlobal][xId] = stream.Pick("lcr_chroma_format_idc", _original != null ? _original.lcr_chroma_format_idc[isGlobal][xId] : this.lcr_chroma_format_idc[isGlobal][xId], _edited != null ? _edited.lcr_chroma_format_idc[isGlobal][xId] : _original != null ? _original.lcr_chroma_format_idc[isGlobal][xId] : this.lcr_chroma_format_idc[isGlobal][xId]);
				stream.WriteUvlc( this.lcr_chroma_format_idc[isGlobal][xId], "lcr_chroma_format_idc"); 
			}

			if ((lcr_cropping_window_present_flag[isGlobal][xId] != 0))
			{
				this.lcr_cropping_win_left_offset[isGlobal][xId] = stream.Pick("lcr_cropping_win_left_offset", _original != null ? _original.lcr_cropping_win_left_offset[isGlobal][xId] : this.lcr_cropping_win_left_offset[isGlobal][xId], _edited != null ? _edited.lcr_cropping_win_left_offset[isGlobal][xId] : _original != null ? _original.lcr_cropping_win_left_offset[isGlobal][xId] : this.lcr_cropping_win_left_offset[isGlobal][xId]);
				stream.WriteUvlc( this.lcr_cropping_win_left_offset[isGlobal][xId], "lcr_cropping_win_left_offset"); 
				this.lcr_cropping_win_right_offset[isGlobal][xId] = stream.Pick("lcr_cropping_win_right_offset", _original != null ? _original.lcr_cropping_win_right_offset[isGlobal][xId] : this.lcr_cropping_win_right_offset[isGlobal][xId], _edited != null ? _edited.lcr_cropping_win_right_offset[isGlobal][xId] : _original != null ? _original.lcr_cropping_win_right_offset[isGlobal][xId] : this.lcr_cropping_win_right_offset[isGlobal][xId]);
				stream.WriteUvlc( this.lcr_cropping_win_right_offset[isGlobal][xId], "lcr_cropping_win_right_offset"); 
				this.lcr_cropping_win_top_offset[isGlobal][xId] = stream.Pick("lcr_cropping_win_top_offset", _original != null ? _original.lcr_cropping_win_top_offset[isGlobal][xId] : this.lcr_cropping_win_top_offset[isGlobal][xId], _edited != null ? _edited.lcr_cropping_win_top_offset[isGlobal][xId] : _original != null ? _original.lcr_cropping_win_top_offset[isGlobal][xId] : this.lcr_cropping_win_top_offset[isGlobal][xId]);
				stream.WriteUvlc( this.lcr_cropping_win_top_offset[isGlobal][xId], "lcr_cropping_win_top_offset"); 
				this.lcr_cropping_win_bottom_offset[isGlobal][xId] = stream.Pick("lcr_cropping_win_bottom_offset", _original != null ? _original.lcr_cropping_win_bottom_offset[isGlobal][xId] : this.lcr_cropping_win_bottom_offset[isGlobal][xId], _edited != null ? _edited.lcr_cropping_win_bottom_offset[isGlobal][xId] : _original != null ? _original.lcr_cropping_win_bottom_offset[isGlobal][xId] : this.lcr_cropping_win_bottom_offset[isGlobal][xId]);
				stream.WriteUvlc( this.lcr_cropping_win_bottom_offset[isGlobal][xId], "lcr_cropping_win_bottom_offset"); 
			}
        }

    /*
lcr_embedded_layer_info( isGlobal, xId ) {
lcr_mlayer_map[ isGlobal ][ xId ]	f(8)
for ( j = 0; j < 8; j++ ) {	
if ( lcr_mlayer_map[ isGlobal ][ xId ] & (1 << j) ) {	
n = MAX_NUM_TLAYERS	
lcr_tlayer_map[ isGlobal ][ xId ][ j ]	f(n)
atlasSegmentPresent = isGlobal ?	
lcr_global_atlas_id_present_flag :	
lcr_local_atlas_id_present_flag[ xId ]	
if ( atlasSegmentPresent ) {	
lcr_layer_atlas_segment_id[ isGlobal ][ xId ][ j ]	f(8)
lcr_priority_order[ isGlobal ][ xId ][ j ]	f(8)
lcr_rendering_method[ isGlobal ][ xId ][ j ]	f(8)
}	
lcr_layer_type[ isGlobal ][ xId ][ j ]	f(8)
if ( lcr_layer_type[ isGlobal ][ xId ][ j ] == AUX_LAYER ) {	
lcr_auxiliary_type[ isGlobal ][ xId ][ j ]	f(8)
}	
lcr_view_type[ isGlobal ][ xId ][ j ]	f(8)
if ( lcr_view_type[ isGlobal ][ xId ][ j ] == VIEW_EXPLICIT ) {	
lcr_view_id[ isGlobal ][ xId ][ j ]	f(8)
}	
if ( j > 0 ) {	
lcr_dependent_layer_map[ isGlobal ][ xId ][ j ]	f(j)
}	
lcr_same_sh_max_resolution_flag[ isGlobal ][ xId ][ j ]	f(1)
if ( !lcr_same_sh_max_resolution_flag[ isGlobal ][ xId ][ j ] ) {	
lcr_max_expected_width[ isGlobal ][ xId ][ j ]	uvlc()
lcr_max_expected_height[ isGlobal ][ xId ][ j ]	uvlc()
}	
byte_alignment()	
}	
}	
}
    */
		private AomArray<AomArray<int>> lcr_mlayer_map = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _LcrMlayerMap { get { return lcr_mlayer_map; } set { lcr_mlayer_map = value; } }
		private AomArray<AomArray<AomArray<int>>> lcr_tlayer_map = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _LcrTlayerMap { get { return lcr_tlayer_map; } set { lcr_tlayer_map = value; } }
		private AomArray<AomArray<AomArray<int>>> lcr_layer_atlas_segment_id = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _LcrLayerAtlasSegmentId { get { return lcr_layer_atlas_segment_id; } set { lcr_layer_atlas_segment_id = value; } }
		private AomArray<AomArray<AomArray<int>>> lcr_priority_order = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _LcrPriorityOrder { get { return lcr_priority_order; } set { lcr_priority_order = value; } }
		private AomArray<AomArray<AomArray<int>>> lcr_rendering_method = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _LcrRenderingMethod { get { return lcr_rendering_method; } set { lcr_rendering_method = value; } }
		private AomArray<AomArray<AomArray<int>>> lcr_layer_type = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _LcrLayerType { get { return lcr_layer_type; } set { lcr_layer_type = value; } }
		private AomArray<AomArray<AomArray<int>>> lcr_auxiliary_type = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _LcrAuxiliaryType { get { return lcr_auxiliary_type; } set { lcr_auxiliary_type = value; } }
		private AomArray<AomArray<AomArray<int>>> lcr_view_type = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _LcrViewType { get { return lcr_view_type; } set { lcr_view_type = value; } }
		private AomArray<AomArray<AomArray<int>>> lcr_view_id = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _LcrViewId { get { return lcr_view_id; } set { lcr_view_id = value; } }
		private AomArray<AomArray<AomArray<int>>> lcr_dependent_layer_map = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _LcrDependentLayerMap { get { return lcr_dependent_layer_map; } set { lcr_dependent_layer_map = value; } }
		private AomArray<AomArray<AomArray<int>>> lcr_same_sh_max_resolution_flag = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _LcrSameShMaxResolutionFlag { get { return lcr_same_sh_max_resolution_flag; } set { lcr_same_sh_max_resolution_flag = value; } }
		private AomArray<AomArray<AomArray<int>>> lcr_max_expected_width = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _LcrMaxExpectedWidth { get { return lcr_max_expected_width; } set { lcr_max_expected_width = value; } }
		private AomArray<AomArray<AomArray<int>>> lcr_max_expected_height = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _LcrMaxExpectedHeight { get { return lcr_max_expected_height; } set { lcr_max_expected_height = value; } }

        private void LcrEmbeddedLayerInfo(int isGlobal, int xId)
        {
			int j = 0;
			int n = 0;
			int atlasSegmentPresent = 0;
			stream.ReadFixed(8, out this.lcr_mlayer_map[isGlobal][xId], "lcr_mlayer_map"); 

			for (j = 0; (j < 8); j++)
			{

				if (((lcr_mlayer_map[isGlobal][xId] & (1 << j)) != 0))
				{
					n = MAX_NUM_TLAYERS;
					stream.ReadVariable(n, out this.lcr_tlayer_map[isGlobal][xId][j], "lcr_tlayer_map"); 
					atlasSegmentPresent = ((isGlobal != 0) ? lcr_global_atlas_id_present_flag : lcr_local_atlas_id_present_flag[xId]);

					if ((atlasSegmentPresent != 0))
					{
						stream.ReadFixed(8, out this.lcr_layer_atlas_segment_id[isGlobal][xId][j], "lcr_layer_atlas_segment_id"); 
						stream.ReadFixed(8, out this.lcr_priority_order[isGlobal][xId][j], "lcr_priority_order"); 
						stream.ReadFixed(8, out this.lcr_rendering_method[isGlobal][xId][j], "lcr_rendering_method"); 
					}
					stream.ReadFixed(8, out this.lcr_layer_type[isGlobal][xId][j], "lcr_layer_type"); 

					if ((lcr_layer_type[isGlobal][xId][j] == AUX_LAYER))
					{
						stream.ReadFixed(8, out this.lcr_auxiliary_type[isGlobal][xId][j], "lcr_auxiliary_type"); 
					}
					stream.ReadFixed(8, out this.lcr_view_type[isGlobal][xId][j], "lcr_view_type"); 

					if ((lcr_view_type[isGlobal][xId][j] == VIEW_EXPLICIT))
					{
						stream.ReadFixed(8, out this.lcr_view_id[isGlobal][xId][j], "lcr_view_id"); 
					}

					if ((j > 0))
					{
						stream.ReadVariable(j, out this.lcr_dependent_layer_map[isGlobal][xId][j], "lcr_dependent_layer_map"); 
					}
					stream.ReadFixed(1, out this.lcr_same_sh_max_resolution_flag[isGlobal][xId][j], "lcr_same_sh_max_resolution_flag"); 

					if (!(lcr_same_sh_max_resolution_flag[isGlobal][xId][j] != 0))
					{
						stream.ReadUvlc( out this.lcr_max_expected_width[isGlobal][xId][j], "lcr_max_expected_width"); 
						stream.ReadUvlc( out this.lcr_max_expected_height[isGlobal][xId][j], "lcr_max_expected_height"); 
					}
					ByteAlignment(); 
				}
			}
        }

        private void WriteLcrEmbeddedLayerInfo(int isGlobal, int xId)
        {
			int j = 0;
			int n = 0;
			int atlasSegmentPresent = 0;
			this.lcr_mlayer_map[isGlobal][xId] = stream.Pick("lcr_mlayer_map", _original != null ? _original.lcr_mlayer_map[isGlobal][xId] : this.lcr_mlayer_map[isGlobal][xId], _edited != null ? _edited.lcr_mlayer_map[isGlobal][xId] : _original != null ? _original.lcr_mlayer_map[isGlobal][xId] : this.lcr_mlayer_map[isGlobal][xId]);
			stream.WriteFixed(8, this.lcr_mlayer_map[isGlobal][xId], "lcr_mlayer_map"); 

			for (j = 0; (j < 8); j++)
			{

				if (((lcr_mlayer_map[isGlobal][xId] & (1 << j)) != 0))
				{
					n = MAX_NUM_TLAYERS;
					this.lcr_tlayer_map[isGlobal][xId][j] = stream.Pick("lcr_tlayer_map", _original != null ? _original.lcr_tlayer_map[isGlobal][xId][j] : this.lcr_tlayer_map[isGlobal][xId][j], _edited != null ? _edited.lcr_tlayer_map[isGlobal][xId][j] : _original != null ? _original.lcr_tlayer_map[isGlobal][xId][j] : this.lcr_tlayer_map[isGlobal][xId][j]);
					stream.WriteVariable(n, this.lcr_tlayer_map[isGlobal][xId][j], "lcr_tlayer_map"); 
					atlasSegmentPresent = ((isGlobal != 0) ? lcr_global_atlas_id_present_flag : lcr_local_atlas_id_present_flag[xId]);

					if ((atlasSegmentPresent != 0))
					{
						this.lcr_layer_atlas_segment_id[isGlobal][xId][j] = stream.Pick("lcr_layer_atlas_segment_id", _original != null ? _original.lcr_layer_atlas_segment_id[isGlobal][xId][j] : this.lcr_layer_atlas_segment_id[isGlobal][xId][j], _edited != null ? _edited.lcr_layer_atlas_segment_id[isGlobal][xId][j] : _original != null ? _original.lcr_layer_atlas_segment_id[isGlobal][xId][j] : this.lcr_layer_atlas_segment_id[isGlobal][xId][j]);
						stream.WriteFixed(8, this.lcr_layer_atlas_segment_id[isGlobal][xId][j], "lcr_layer_atlas_segment_id"); 
						this.lcr_priority_order[isGlobal][xId][j] = stream.Pick("lcr_priority_order", _original != null ? _original.lcr_priority_order[isGlobal][xId][j] : this.lcr_priority_order[isGlobal][xId][j], _edited != null ? _edited.lcr_priority_order[isGlobal][xId][j] : _original != null ? _original.lcr_priority_order[isGlobal][xId][j] : this.lcr_priority_order[isGlobal][xId][j]);
						stream.WriteFixed(8, this.lcr_priority_order[isGlobal][xId][j], "lcr_priority_order"); 
						this.lcr_rendering_method[isGlobal][xId][j] = stream.Pick("lcr_rendering_method", _original != null ? _original.lcr_rendering_method[isGlobal][xId][j] : this.lcr_rendering_method[isGlobal][xId][j], _edited != null ? _edited.lcr_rendering_method[isGlobal][xId][j] : _original != null ? _original.lcr_rendering_method[isGlobal][xId][j] : this.lcr_rendering_method[isGlobal][xId][j]);
						stream.WriteFixed(8, this.lcr_rendering_method[isGlobal][xId][j], "lcr_rendering_method"); 
					}
					this.lcr_layer_type[isGlobal][xId][j] = stream.Pick("lcr_layer_type", _original != null ? _original.lcr_layer_type[isGlobal][xId][j] : this.lcr_layer_type[isGlobal][xId][j], _edited != null ? _edited.lcr_layer_type[isGlobal][xId][j] : _original != null ? _original.lcr_layer_type[isGlobal][xId][j] : this.lcr_layer_type[isGlobal][xId][j]);
					stream.WriteFixed(8, this.lcr_layer_type[isGlobal][xId][j], "lcr_layer_type"); 

					if ((lcr_layer_type[isGlobal][xId][j] == AUX_LAYER))
					{
						this.lcr_auxiliary_type[isGlobal][xId][j] = stream.Pick("lcr_auxiliary_type", _original != null ? _original.lcr_auxiliary_type[isGlobal][xId][j] : this.lcr_auxiliary_type[isGlobal][xId][j], _edited != null ? _edited.lcr_auxiliary_type[isGlobal][xId][j] : _original != null ? _original.lcr_auxiliary_type[isGlobal][xId][j] : this.lcr_auxiliary_type[isGlobal][xId][j]);
						stream.WriteFixed(8, this.lcr_auxiliary_type[isGlobal][xId][j], "lcr_auxiliary_type"); 
					}
					this.lcr_view_type[isGlobal][xId][j] = stream.Pick("lcr_view_type", _original != null ? _original.lcr_view_type[isGlobal][xId][j] : this.lcr_view_type[isGlobal][xId][j], _edited != null ? _edited.lcr_view_type[isGlobal][xId][j] : _original != null ? _original.lcr_view_type[isGlobal][xId][j] : this.lcr_view_type[isGlobal][xId][j]);
					stream.WriteFixed(8, this.lcr_view_type[isGlobal][xId][j], "lcr_view_type"); 

					if ((lcr_view_type[isGlobal][xId][j] == VIEW_EXPLICIT))
					{
						this.lcr_view_id[isGlobal][xId][j] = stream.Pick("lcr_view_id", _original != null ? _original.lcr_view_id[isGlobal][xId][j] : this.lcr_view_id[isGlobal][xId][j], _edited != null ? _edited.lcr_view_id[isGlobal][xId][j] : _original != null ? _original.lcr_view_id[isGlobal][xId][j] : this.lcr_view_id[isGlobal][xId][j]);
						stream.WriteFixed(8, this.lcr_view_id[isGlobal][xId][j], "lcr_view_id"); 
					}

					if ((j > 0))
					{
						this.lcr_dependent_layer_map[isGlobal][xId][j] = stream.Pick("lcr_dependent_layer_map", _original != null ? _original.lcr_dependent_layer_map[isGlobal][xId][j] : this.lcr_dependent_layer_map[isGlobal][xId][j], _edited != null ? _edited.lcr_dependent_layer_map[isGlobal][xId][j] : _original != null ? _original.lcr_dependent_layer_map[isGlobal][xId][j] : this.lcr_dependent_layer_map[isGlobal][xId][j]);
						stream.WriteVariable(j, this.lcr_dependent_layer_map[isGlobal][xId][j], "lcr_dependent_layer_map"); 
					}
					this.lcr_same_sh_max_resolution_flag[isGlobal][xId][j] = stream.Pick("lcr_same_sh_max_resolution_flag", _original != null ? _original.lcr_same_sh_max_resolution_flag[isGlobal][xId][j] : this.lcr_same_sh_max_resolution_flag[isGlobal][xId][j], _edited != null ? _edited.lcr_same_sh_max_resolution_flag[isGlobal][xId][j] : _original != null ? _original.lcr_same_sh_max_resolution_flag[isGlobal][xId][j] : this.lcr_same_sh_max_resolution_flag[isGlobal][xId][j]);
					stream.WriteFixed(1, this.lcr_same_sh_max_resolution_flag[isGlobal][xId][j], "lcr_same_sh_max_resolution_flag"); 

					if (!(lcr_same_sh_max_resolution_flag[isGlobal][xId][j] != 0))
					{
						this.lcr_max_expected_width[isGlobal][xId][j] = stream.Pick("lcr_max_expected_width", _original != null ? _original.lcr_max_expected_width[isGlobal][xId][j] : this.lcr_max_expected_width[isGlobal][xId][j], _edited != null ? _edited.lcr_max_expected_width[isGlobal][xId][j] : _original != null ? _original.lcr_max_expected_width[isGlobal][xId][j] : this.lcr_max_expected_width[isGlobal][xId][j]);
						stream.WriteUvlc( this.lcr_max_expected_width[isGlobal][xId][j], "lcr_max_expected_width"); 
						this.lcr_max_expected_height[isGlobal][xId][j] = stream.Pick("lcr_max_expected_height", _original != null ? _original.lcr_max_expected_height[isGlobal][xId][j] : this.lcr_max_expected_height[isGlobal][xId][j], _edited != null ? _edited.lcr_max_expected_height[isGlobal][xId][j] : _original != null ? _original.lcr_max_expected_height[isGlobal][xId][j] : this.lcr_max_expected_height[isGlobal][xId][j]);
						stream.WriteUvlc( this.lcr_max_expected_height[isGlobal][xId][j], "lcr_max_expected_height"); 
					}
					WriteByteAlignment(); 
				}
			}
        }

    /*
lcr_xlayer_color_info( isGlobal, xId ) {
layer_color_description_idc[ isGlobal ][ xId ]	rg(2)
if ( layer_color_description_idc[ isGlobal ][ xId ] == 0 ) {	
layer_color_primaries[ isGlobal ][ xId ]	f(8)
layer_transfer_characteristics[ isGlobal ][ xId ]	f(8)
layer_matrix_coefficients[ isGlobal ][ xId ]	f(8)
}	
layer_full_range_flag[ isGlobal ][ xId ]	f(1)
}
    */
		private AomArray<AomArray<int>> layer_color_description_idc = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _LayerColorDescriptionIdc { get { return layer_color_description_idc; } set { layer_color_description_idc = value; } }
		private AomArray<AomArray<int>> layer_color_primaries = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _LayerColorPrimaries { get { return layer_color_primaries; } set { layer_color_primaries = value; } }
		private AomArray<AomArray<int>> layer_transfer_characteristics = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _LayerTransferCharacteristics { get { return layer_transfer_characteristics; } set { layer_transfer_characteristics = value; } }
		private AomArray<AomArray<int>> layer_matrix_coefficients = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _LayerMatrixCoefficients { get { return layer_matrix_coefficients; } set { layer_matrix_coefficients = value; } }
		private AomArray<AomArray<int>> layer_full_range_flag = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _LayerFullRangeFlag { get { return layer_full_range_flag; } set { layer_full_range_flag = value; } }

        private void LcrXlayerColorInfo(int isGlobal, int xId)
        {
			stream.ReadRg(2, out this.layer_color_description_idc[isGlobal][xId], "layer_color_description_idc"); 

			if ((layer_color_description_idc[isGlobal][xId] == 0))
			{
				stream.ReadFixed(8, out this.layer_color_primaries[isGlobal][xId], "layer_color_primaries"); 
				stream.ReadFixed(8, out this.layer_transfer_characteristics[isGlobal][xId], "layer_transfer_characteristics"); 
				stream.ReadFixed(8, out this.layer_matrix_coefficients[isGlobal][xId], "layer_matrix_coefficients"); 
			}
			stream.ReadFixed(1, out this.layer_full_range_flag[isGlobal][xId], "layer_full_range_flag"); 
        }

        private void WriteLcrXlayerColorInfo(int isGlobal, int xId)
        {
			this.layer_color_description_idc[isGlobal][xId] = stream.Pick("layer_color_description_idc", _original != null ? _original.layer_color_description_idc[isGlobal][xId] : this.layer_color_description_idc[isGlobal][xId], _edited != null ? _edited.layer_color_description_idc[isGlobal][xId] : _original != null ? _original.layer_color_description_idc[isGlobal][xId] : this.layer_color_description_idc[isGlobal][xId]);
			stream.WriteRg(2, this.layer_color_description_idc[isGlobal][xId], "layer_color_description_idc"); 

			if ((layer_color_description_idc[isGlobal][xId] == 0))
			{
				this.layer_color_primaries[isGlobal][xId] = stream.Pick("layer_color_primaries", _original != null ? _original.layer_color_primaries[isGlobal][xId] : this.layer_color_primaries[isGlobal][xId], _edited != null ? _edited.layer_color_primaries[isGlobal][xId] : _original != null ? _original.layer_color_primaries[isGlobal][xId] : this.layer_color_primaries[isGlobal][xId]);
				stream.WriteFixed(8, this.layer_color_primaries[isGlobal][xId], "layer_color_primaries"); 
				this.layer_transfer_characteristics[isGlobal][xId] = stream.Pick("layer_transfer_characteristics", _original != null ? _original.layer_transfer_characteristics[isGlobal][xId] : this.layer_transfer_characteristics[isGlobal][xId], _edited != null ? _edited.layer_transfer_characteristics[isGlobal][xId] : _original != null ? _original.layer_transfer_characteristics[isGlobal][xId] : this.layer_transfer_characteristics[isGlobal][xId]);
				stream.WriteFixed(8, this.layer_transfer_characteristics[isGlobal][xId], "layer_transfer_characteristics"); 
				this.layer_matrix_coefficients[isGlobal][xId] = stream.Pick("layer_matrix_coefficients", _original != null ? _original.layer_matrix_coefficients[isGlobal][xId] : this.layer_matrix_coefficients[isGlobal][xId], _edited != null ? _edited.layer_matrix_coefficients[isGlobal][xId] : _original != null ? _original.layer_matrix_coefficients[isGlobal][xId] : this.layer_matrix_coefficients[isGlobal][xId]);
				stream.WriteFixed(8, this.layer_matrix_coefficients[isGlobal][xId], "layer_matrix_coefficients"); 
			}
			this.layer_full_range_flag[isGlobal][xId] = stream.Pick("layer_full_range_flag", _original != null ? _original.layer_full_range_flag[isGlobal][xId] : this.layer_full_range_flag[isGlobal][xId], _edited != null ? _edited.layer_full_range_flag[isGlobal][xId] : _original != null ? _original.layer_full_range_flag[isGlobal][xId] : this.layer_full_range_flag[isGlobal][xId]);
			stream.WriteFixed(1, this.layer_full_range_flag[isGlobal][xId], "layer_full_range_flag"); 
        }

    /*
atlas_segment_info_obu() {
atlas_segment_id[ obu_xlayer_id ]	f(3)
xAId = atlas_segment_id[ obu_xlayer_id ]	
ats_atlas_segment_mode_idc[ xAId ]	uvlc()
if ( ats_atlas_segment_mode_idc[ xAId ] == ENHANCED_ATLAS ) {	
numSegments = ats_enhanced_atlas_info( xAId )	
} else if ( ats_atlas_segment_mode_idc[ xAId ] == BASIC_ATLAS ) {	
numSegments = ats_basic_info( xAId )	
} else if ( ats_atlas_segment_mode_idc[ xAId ] == SINGLE_ATLAS ) {	
numSegments = 1	
ats_nominal_width_minus_1[ xAId ]	uvlc()
ats_nominal_height_minus_1[ xAId ]	uvlc()
} else if ( ats_atlas_segment_mode_idc[ xAId ] == MULTISTREAM_ATLAS ) {	
numSegments = ats_multistream_info( obu_xlayer_id, xAId )	
} else if ( ats_atlas_segment_mode_idc[ xAId ] ==	
MULTISTREAM_ALPHA_ATLAS ) {	
numSegments = ats_multistream_with_alpha_info( obu_xlayer_id, xAId )	
}	
ats_label_segment_info( obu_xlayer_id, xAId, numSegments )	
}
    */
		private AomArray<int> atlas_segment_id = new AomArray<int>();
		public AomArray<int> _AtlasSegmentId { get { return atlas_segment_id; } set { atlas_segment_id = value; } }
		private AomArray<int> ats_atlas_segment_mode_idc = new AomArray<int>();
		public AomArray<int> _AtsAtlasSegmentModeIdc { get { return ats_atlas_segment_mode_idc; } set { ats_atlas_segment_mode_idc = value; } }
		private AomArray<int> ats_nominal_width_minus_1 = new AomArray<int>();
		public AomArray<int> _AtsNominalWidthMinus1 { get { return ats_nominal_width_minus_1; } set { ats_nominal_width_minus_1 = value; } }
		private AomArray<int> ats_nominal_height_minus_1 = new AomArray<int>();
		public AomArray<int> _AtsNominalHeightMinus1 { get { return ats_nominal_height_minus_1; } set { ats_nominal_height_minus_1 = value; } }

        private void AtlasSegmentInfoObu()
        {
			int xAId = 0;
			int numSegments = 0;
			stream.ReadFixed(3, out this.atlas_segment_id[obu_xlayer_id], "atlas_segment_id"); 
			xAId = atlas_segment_id[obu_xlayer_id];
			stream.ReadUvlc( out this.ats_atlas_segment_mode_idc[xAId], "ats_atlas_segment_mode_idc"); 

			if ((ats_atlas_segment_mode_idc[xAId] == ENHANCED_ATLAS))
			{
				numSegments = AtsEnhancedAtlasInfo(xAId);
			}
			else if ((ats_atlas_segment_mode_idc[xAId] == BASIC_ATLAS))
			{
				numSegments = AtsBasicInfo(xAId);
			}
			else if ((ats_atlas_segment_mode_idc[xAId] == SINGLE_ATLAS))
			{
				numSegments = 1;
				stream.ReadUvlc( out this.ats_nominal_width_minus_1[xAId], "ats_nominal_width_minus_1"); 
				stream.ReadUvlc( out this.ats_nominal_height_minus_1[xAId], "ats_nominal_height_minus_1"); 
			}
			else if ((ats_atlas_segment_mode_idc[xAId] == MULTISTREAM_ATLAS))
			{
				numSegments = AtsMultistreamInfo(obu_xlayer_id, xAId);
			}
			else if ((ats_atlas_segment_mode_idc[xAId] == MULTISTREAM_ALPHA_ATLAS))
			{
				numSegments = AtsMultistreamWithAlphaInfo(obu_xlayer_id, xAId);
			}
			AtsLabelSegmentInfo(obu_xlayer_id, xAId, numSegments); 
        }

        private void WriteAtlasSegmentInfoObu()
        {
			int xAId = 0;
			int numSegments = 0;
			this.atlas_segment_id[obu_xlayer_id] = stream.Pick("atlas_segment_id", _original != null ? _original.atlas_segment_id[obu_xlayer_id] : this.atlas_segment_id[obu_xlayer_id], _edited != null ? _edited.atlas_segment_id[obu_xlayer_id] : _original != null ? _original.atlas_segment_id[obu_xlayer_id] : this.atlas_segment_id[obu_xlayer_id]);
			stream.WriteFixed(3, this.atlas_segment_id[obu_xlayer_id], "atlas_segment_id"); 
			xAId = atlas_segment_id[obu_xlayer_id];
			this.ats_atlas_segment_mode_idc[xAId] = stream.Pick("ats_atlas_segment_mode_idc", _original != null ? _original.ats_atlas_segment_mode_idc[xAId] : this.ats_atlas_segment_mode_idc[xAId], _edited != null ? _edited.ats_atlas_segment_mode_idc[xAId] : _original != null ? _original.ats_atlas_segment_mode_idc[xAId] : this.ats_atlas_segment_mode_idc[xAId]);
			stream.WriteUvlc( this.ats_atlas_segment_mode_idc[xAId], "ats_atlas_segment_mode_idc"); 

			if ((ats_atlas_segment_mode_idc[xAId] == ENHANCED_ATLAS))
			{
				numSegments = WriteAtsEnhancedAtlasInfo(xAId);
			}
			else if ((ats_atlas_segment_mode_idc[xAId] == BASIC_ATLAS))
			{
				numSegments = WriteAtsBasicInfo(xAId);
			}
			else if ((ats_atlas_segment_mode_idc[xAId] == SINGLE_ATLAS))
			{
				numSegments = 1;
				this.ats_nominal_width_minus_1[xAId] = stream.Pick("ats_nominal_width_minus_1", _original != null ? _original.ats_nominal_width_minus_1[xAId] : this.ats_nominal_width_minus_1[xAId], _edited != null ? _edited.ats_nominal_width_minus_1[xAId] : _original != null ? _original.ats_nominal_width_minus_1[xAId] : this.ats_nominal_width_minus_1[xAId]);
				stream.WriteUvlc( this.ats_nominal_width_minus_1[xAId], "ats_nominal_width_minus_1"); 
				this.ats_nominal_height_minus_1[xAId] = stream.Pick("ats_nominal_height_minus_1", _original != null ? _original.ats_nominal_height_minus_1[xAId] : this.ats_nominal_height_minus_1[xAId], _edited != null ? _edited.ats_nominal_height_minus_1[xAId] : _original != null ? _original.ats_nominal_height_minus_1[xAId] : this.ats_nominal_height_minus_1[xAId]);
				stream.WriteUvlc( this.ats_nominal_height_minus_1[xAId], "ats_nominal_height_minus_1"); 
			}
			else if ((ats_atlas_segment_mode_idc[xAId] == MULTISTREAM_ATLAS))
			{
				numSegments = WriteAtsMultistreamInfo(obu_xlayer_id, xAId);
			}
			else if ((ats_atlas_segment_mode_idc[xAId] == MULTISTREAM_ALPHA_ATLAS))
			{
				numSegments = WriteAtsMultistreamWithAlphaInfo(obu_xlayer_id, xAId);
			}
			WriteAtsLabelSegmentInfo(obu_xlayer_id, xAId, numSegments); 
        }

    /*
ats_label_segment_info( xlayerId, xAId, numSegments ) {
ats_signaled_atlas_segment_ids_flag[ xlayerId ][ xAId ]	f(1)
if ( ats_signaled_atlas_segment_ids_flag[ xlayerId ][ xAId ] ) {	
for ( i = 0;i < numSegments; i++ ) {	
ats_atlas_segment_id[ xlayerId ][ xAId ][ i ]	f(8)
AtlasSegmentIDToIndex[ xlayerId ][ xAId ]	
[ ats_atlas_segment_id[ xlayerId ][ xAId ][ i ] ] = i	
AtlasSegmentIndexToID[ xlayerId ][ xAId ][ i ] =	
ats_atlas_segment_id[ xlayerId ][ xAId ][ i ]	
}	
} else {	
for ( i = 0;i < numSegments; i++ ) {	
ats_atlas_segment_id[ xlayerId ][ xAId ][ i ] = i	
AtlasSegmentIDToIndex[ xlayerId ][ xAId ][ i ] = i	
AtlasSegmentIndexToID[ xlayerId ][ xAId ][ i ] = i	
}	
}	
}
    */
		private int xAId;
		public int _XAId { get { return xAId; } set { xAId = value; } }
		private AomArray<AomArray<int>> ats_signaled_atlas_segment_ids_flag = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _AtsSignaledAtlasSegmentIdsFlag { get { return ats_signaled_atlas_segment_ids_flag; } set { ats_signaled_atlas_segment_ids_flag = value; } }
		private AomArray<AomArray<AomArray<int>>> ats_atlas_segment_id = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _AtsAtlasSegmentId { get { return ats_atlas_segment_id; } set { ats_atlas_segment_id = value; } }
		private AomArray<AomArray<AomArray<int>>> AtlasSegmentIDToIndex = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _AtlasSegmentIDToIndex { get { return AtlasSegmentIDToIndex; } set { AtlasSegmentIDToIndex = value; } }
		private AomArray<AomArray<AomArray<int>>> AtlasSegmentIndexToID = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _AtlasSegmentIndexToID { get { return AtlasSegmentIndexToID; } set { AtlasSegmentIndexToID = value; } }

        private void AtsLabelSegmentInfo(int xlayerId, int xAId, int numSegments)
        {
			int i = 0;
			stream.ReadFixed(1, out this.ats_signaled_atlas_segment_ids_flag[xlayerId][xAId], "ats_signaled_atlas_segment_ids_flag"); 

			if ((ats_signaled_atlas_segment_ids_flag[xlayerId][xAId] != 0))
			{

				for (i = 0; (i < numSegments); i++)
				{
					stream.ReadFixed(8, out this.ats_atlas_segment_id[xlayerId][xAId][i], "ats_atlas_segment_id"); 
					AtlasSegmentIDToIndex[xlayerId][xAId][ats_atlas_segment_id[xlayerId][xAId][i]] = i;
					AtlasSegmentIndexToID[xlayerId][xAId][i] = ats_atlas_segment_id[xlayerId][xAId][i];
				}
			}
			else 
			{

				for (i = 0; (i < numSegments); i++)
				{
					ats_atlas_segment_id[xlayerId][xAId][i] = i;
					AtlasSegmentIDToIndex[xlayerId][xAId][i] = i;
					AtlasSegmentIndexToID[xlayerId][xAId][i] = i;
				}
			}
        }

        private void WriteAtsLabelSegmentInfo(int xlayerId, int xAId, int numSegments)
        {
			int i = 0;
			this.ats_signaled_atlas_segment_ids_flag[xlayerId][xAId] = stream.Pick("ats_signaled_atlas_segment_ids_flag", _original != null ? _original.ats_signaled_atlas_segment_ids_flag[xlayerId][xAId] : this.ats_signaled_atlas_segment_ids_flag[xlayerId][xAId], _edited != null ? _edited.ats_signaled_atlas_segment_ids_flag[xlayerId][xAId] : _original != null ? _original.ats_signaled_atlas_segment_ids_flag[xlayerId][xAId] : this.ats_signaled_atlas_segment_ids_flag[xlayerId][xAId]);
			stream.WriteFixed(1, this.ats_signaled_atlas_segment_ids_flag[xlayerId][xAId], "ats_signaled_atlas_segment_ids_flag"); 

			if ((ats_signaled_atlas_segment_ids_flag[xlayerId][xAId] != 0))
			{

				for (i = 0; (i < numSegments); i++)
				{
					this.ats_atlas_segment_id[xlayerId][xAId][i] = stream.Pick("ats_atlas_segment_id", _original != null ? _original.ats_atlas_segment_id[xlayerId][xAId][i] : this.ats_atlas_segment_id[xlayerId][xAId][i], _edited != null ? _edited.ats_atlas_segment_id[xlayerId][xAId][i] : _original != null ? _original.ats_atlas_segment_id[xlayerId][xAId][i] : this.ats_atlas_segment_id[xlayerId][xAId][i]);
					stream.WriteFixed(8, this.ats_atlas_segment_id[xlayerId][xAId][i], "ats_atlas_segment_id"); 
					AtlasSegmentIDToIndex[xlayerId][xAId][ats_atlas_segment_id[xlayerId][xAId][i]] = i;
					AtlasSegmentIndexToID[xlayerId][xAId][i] = ats_atlas_segment_id[xlayerId][xAId][i];
				}
			}
			else 
			{

				for (i = 0; (i < numSegments); i++)
				{
					ats_atlas_segment_id[xlayerId][xAId][i] = i;
					AtlasSegmentIDToIndex[xlayerId][xAId][i] = i;
					AtlasSegmentIndexToID[xlayerId][xAId][i] = i;
				}
			}
        }

    /*
ats_enhanced_atlas_info( xAId ) {
ats_region_info( xAId )	
numSegments = ats_region_to_segment_mapping( xAId )	
return numSegments	
}
    */

        private int AtsEnhancedAtlasInfo(int xAId)
        {
			int numSegments = 0;
			AtsRegionInfo(xAId); 
			numSegments = AtsRegionToSegmentMapping(xAId);
			return numSegments;
        }

        private int WriteAtsEnhancedAtlasInfo(int xAId)
        {
			int numSegments = 0;
			WriteAtsRegionInfo(xAId); 
			numSegments = WriteAtsRegionToSegmentMapping(xAId);
			return numSegments;
        }

    /*
ats_region_info( xAId ) {
ats_num_region_columns_minus_1[ xAId ]	uvlc()
ats_num_region_rows_minus_1[ xAId ]	uvlc()
ats_uniform_spacing_flag[ xAId ]	f(1)
AtlasWidth = 0	
AtlasHeight = 0	
if ( !ats_uniform_spacing_flag[ xAId ] ) {	
for ( i = 0; i < ats_num_region_columns_minus_1[ xAId ] + 1;	
i++ ) {	
ats_column_width_minus_1[ xAId ][ i ]	uvlc()
AtlasWidth += (ats_column_width_minus_1[ xAId ][ i ] + 1)	
}	
for ( i = 0;i < ats_num_region_rows_minus_1[xAId] + 1; i++ ) {	
ats_row_height_minus_1[ xAId ][ i ]	uvlc()
AtlasHeight += (ats_row_height_minus_1[ xAId ][ i ] + 1)	
}	
} else {	
ats_region_width_minus_1[ xAId ]	uvlc()
ats_region_height_minus_1[ xAId ]	uvlc()
AtlasWidth =	
( ats_region_width_minus_1[ xAId ] + 1 ) *	( ats_num_region_columns_minus_1[ xAId ] + 1 )	
AtlasHeight =	
( ats_region_height_minus_1[ xAId ] + 1 ) *	( ats_num_region_rows_minus_1[ xAId ] + 1 )	
}	
NumRegionsInAtlas[ xAId ] =	
( ats_num_region_columns_minus_1[ xAId ] + 1) *	( ats_num_region_rows_minus_1[ xAId ] + 1 )	
}
    */
		private AomArray<int> ats_num_region_columns_minus_1 = new AomArray<int>();
		public AomArray<int> _AtsNumRegionColumnsMinus1 { get { return ats_num_region_columns_minus_1; } set { ats_num_region_columns_minus_1 = value; } }
		private AomArray<int> ats_num_region_rows_minus_1 = new AomArray<int>();
		public AomArray<int> _AtsNumRegionRowsMinus1 { get { return ats_num_region_rows_minus_1; } set { ats_num_region_rows_minus_1 = value; } }
		private AomArray<int> ats_uniform_spacing_flag = new AomArray<int>();
		public AomArray<int> _AtsUniformSpacingFlag { get { return ats_uniform_spacing_flag; } set { ats_uniform_spacing_flag = value; } }
		private int AtlasWidth;
		public int _AtlasWidth { get { return AtlasWidth; } set { AtlasWidth = value; } }
		private int AtlasHeight;
		public int _AtlasHeight { get { return AtlasHeight; } set { AtlasHeight = value; } }
		private AomArray<AomArray<int>> ats_column_width_minus_1 = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _AtsColumnWidthMinus1 { get { return ats_column_width_minus_1; } set { ats_column_width_minus_1 = value; } }
		private AomArray<AomArray<int>> ats_row_height_minus_1 = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _AtsRowHeightMinus1 { get { return ats_row_height_minus_1; } set { ats_row_height_minus_1 = value; } }
		private AomArray<int> ats_region_width_minus_1 = new AomArray<int>();
		public AomArray<int> _AtsRegionWidthMinus1 { get { return ats_region_width_minus_1; } set { ats_region_width_minus_1 = value; } }
		private AomArray<int> ats_region_height_minus_1 = new AomArray<int>();
		public AomArray<int> _AtsRegionHeightMinus1 { get { return ats_region_height_minus_1; } set { ats_region_height_minus_1 = value; } }
		private AomArray<int> NumRegionsInAtlas = new AomArray<int>();
		public AomArray<int> _NumRegionsInAtlas { get { return NumRegionsInAtlas; } set { NumRegionsInAtlas = value; } }

        private void AtsRegionInfo(int xAId)
        {
			int i = 0;
			stream.ReadUvlc( out this.ats_num_region_columns_minus_1[xAId], "ats_num_region_columns_minus_1"); 
			stream.ReadUvlc( out this.ats_num_region_rows_minus_1[xAId], "ats_num_region_rows_minus_1"); 
			stream.ReadFixed(1, out this.ats_uniform_spacing_flag[xAId], "ats_uniform_spacing_flag"); 
			AtlasWidth = 0;
			AtlasHeight = 0;

			if (!(ats_uniform_spacing_flag[xAId] != 0))
			{

				for (i = 0; (i < (ats_num_region_columns_minus_1[xAId] + 1)); i++)
				{
					stream.ReadUvlc( out this.ats_column_width_minus_1[xAId][i], "ats_column_width_minus_1"); 
					AtlasWidth += (ats_column_width_minus_1[xAId][i] + 1);
				}

				for (i = 0; (i < (ats_num_region_rows_minus_1[xAId] + 1)); i++)
				{
					stream.ReadUvlc( out this.ats_row_height_minus_1[xAId][i], "ats_row_height_minus_1"); 
					AtlasHeight += (ats_row_height_minus_1[xAId][i] + 1);
				}
			}
			else 
			{
				stream.ReadUvlc( out this.ats_region_width_minus_1[xAId], "ats_region_width_minus_1"); 
				stream.ReadUvlc( out this.ats_region_height_minus_1[xAId], "ats_region_height_minus_1"); 
				AtlasWidth = ((ats_region_width_minus_1[xAId] + 1) * (ats_num_region_columns_minus_1[xAId] + 1));
				AtlasHeight = ((ats_region_height_minus_1[xAId] + 1) * (ats_num_region_rows_minus_1[xAId] + 1));
			}
			NumRegionsInAtlas[xAId] = ((ats_num_region_columns_minus_1[xAId] + 1) * (ats_num_region_rows_minus_1[xAId] + 1));
        }

        private void WriteAtsRegionInfo(int xAId)
        {
			int i = 0;
			this.ats_num_region_columns_minus_1[xAId] = stream.Pick("ats_num_region_columns_minus_1", _original != null ? _original.ats_num_region_columns_minus_1[xAId] : this.ats_num_region_columns_minus_1[xAId], _edited != null ? _edited.ats_num_region_columns_minus_1[xAId] : _original != null ? _original.ats_num_region_columns_minus_1[xAId] : this.ats_num_region_columns_minus_1[xAId]);
			stream.WriteUvlc( this.ats_num_region_columns_minus_1[xAId], "ats_num_region_columns_minus_1"); 
			this.ats_num_region_rows_minus_1[xAId] = stream.Pick("ats_num_region_rows_minus_1", _original != null ? _original.ats_num_region_rows_minus_1[xAId] : this.ats_num_region_rows_minus_1[xAId], _edited != null ? _edited.ats_num_region_rows_minus_1[xAId] : _original != null ? _original.ats_num_region_rows_minus_1[xAId] : this.ats_num_region_rows_minus_1[xAId]);
			stream.WriteUvlc( this.ats_num_region_rows_minus_1[xAId], "ats_num_region_rows_minus_1"); 
			this.ats_uniform_spacing_flag[xAId] = stream.Pick("ats_uniform_spacing_flag", _original != null ? _original.ats_uniform_spacing_flag[xAId] : this.ats_uniform_spacing_flag[xAId], _edited != null ? _edited.ats_uniform_spacing_flag[xAId] : _original != null ? _original.ats_uniform_spacing_flag[xAId] : this.ats_uniform_spacing_flag[xAId]);
			stream.WriteFixed(1, this.ats_uniform_spacing_flag[xAId], "ats_uniform_spacing_flag"); 
			AtlasWidth = 0;
			AtlasHeight = 0;

			if (!(ats_uniform_spacing_flag[xAId] != 0))
			{

				for (i = 0; (i < (ats_num_region_columns_minus_1[xAId] + 1)); i++)
				{
					this.ats_column_width_minus_1[xAId][i] = stream.Pick("ats_column_width_minus_1", _original != null ? _original.ats_column_width_minus_1[xAId][i] : this.ats_column_width_minus_1[xAId][i], _edited != null ? _edited.ats_column_width_minus_1[xAId][i] : _original != null ? _original.ats_column_width_minus_1[xAId][i] : this.ats_column_width_minus_1[xAId][i]);
					stream.WriteUvlc( this.ats_column_width_minus_1[xAId][i], "ats_column_width_minus_1"); 
					AtlasWidth += (ats_column_width_minus_1[xAId][i] + 1);
				}

				for (i = 0; (i < (ats_num_region_rows_minus_1[xAId] + 1)); i++)
				{
					this.ats_row_height_minus_1[xAId][i] = stream.Pick("ats_row_height_minus_1", _original != null ? _original.ats_row_height_minus_1[xAId][i] : this.ats_row_height_minus_1[xAId][i], _edited != null ? _edited.ats_row_height_minus_1[xAId][i] : _original != null ? _original.ats_row_height_minus_1[xAId][i] : this.ats_row_height_minus_1[xAId][i]);
					stream.WriteUvlc( this.ats_row_height_minus_1[xAId][i], "ats_row_height_minus_1"); 
					AtlasHeight += (ats_row_height_minus_1[xAId][i] + 1);
				}
			}
			else 
			{
				this.ats_region_width_minus_1[xAId] = stream.Pick("ats_region_width_minus_1", _original != null ? _original.ats_region_width_minus_1[xAId] : this.ats_region_width_minus_1[xAId], _edited != null ? _edited.ats_region_width_minus_1[xAId] : _original != null ? _original.ats_region_width_minus_1[xAId] : this.ats_region_width_minus_1[xAId]);
				stream.WriteUvlc( this.ats_region_width_minus_1[xAId], "ats_region_width_minus_1"); 
				this.ats_region_height_minus_1[xAId] = stream.Pick("ats_region_height_minus_1", _original != null ? _original.ats_region_height_minus_1[xAId] : this.ats_region_height_minus_1[xAId], _edited != null ? _edited.ats_region_height_minus_1[xAId] : _original != null ? _original.ats_region_height_minus_1[xAId] : this.ats_region_height_minus_1[xAId]);
				stream.WriteUvlc( this.ats_region_height_minus_1[xAId], "ats_region_height_minus_1"); 
				AtlasWidth = ((ats_region_width_minus_1[xAId] + 1) * (ats_num_region_columns_minus_1[xAId] + 1));
				AtlasHeight = ((ats_region_height_minus_1[xAId] + 1) * (ats_num_region_rows_minus_1[xAId] + 1));
			}
			NumRegionsInAtlas[xAId] = ((ats_num_region_columns_minus_1[xAId] + 1) * (ats_num_region_rows_minus_1[xAId] + 1));
        }

    /*
ats_region_to_segment_mapping( xAId ) {
ats_single_region_per_atlas_segment_flag[ xAId ]	f(1)
if ( !ats_single_region_per_atlas_segment_flag[ xAId ] ) {	
ats_num_atlas_segments_minus_1[ xAId ]	uvlc()
for ( i = 0; i <= ats_num_atlas_segments_minus_1[ xAId ]; i++ ) {	
ats_top_left_region_column[ xAId ][ i ]	uvlc()
ats_top_left_region_row[ xAId ][ i ]	uvlc()
ats_bottom_right_region_column_off[ xAId ][ i ]	uvlc()
ats_bottom_right_region_row_off[ xAId ][ i ]	uvlc()
}	
} else {	
ats_num_atlas_segments_minus_1[ xAId ] =	
NumRegionsInAtlas[ xAId ] - 1	
}	
return ats_num_atlas_segments_minus_1[ xAId ] + 1	
}
    */
		private AomArray<int> ats_single_region_per_atlas_segment_flag = new AomArray<int>();
		public AomArray<int> _AtsSingleRegionPerAtlasSegmentFlag { get { return ats_single_region_per_atlas_segment_flag; } set { ats_single_region_per_atlas_segment_flag = value; } }
		private AomArray<int> ats_num_atlas_segments_minus_1 = new AomArray<int>();
		public AomArray<int> _AtsNumAtlasSegmentsMinus1 { get { return ats_num_atlas_segments_minus_1; } set { ats_num_atlas_segments_minus_1 = value; } }
		private AomArray<AomArray<int>> ats_top_left_region_column = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _AtsTopLeftRegionColumn { get { return ats_top_left_region_column; } set { ats_top_left_region_column = value; } }
		private AomArray<AomArray<int>> ats_top_left_region_row = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _AtsTopLeftRegionRow { get { return ats_top_left_region_row; } set { ats_top_left_region_row = value; } }
		private AomArray<AomArray<int>> ats_bottom_right_region_column_off = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _AtsBottomRightRegionColumnOff { get { return ats_bottom_right_region_column_off; } set { ats_bottom_right_region_column_off = value; } }
		private AomArray<AomArray<int>> ats_bottom_right_region_row_off = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _AtsBottomRightRegionRowOff { get { return ats_bottom_right_region_row_off; } set { ats_bottom_right_region_row_off = value; } }

        private int AtsRegionToSegmentMapping(int xAId)
        {
			int i = 0;
			stream.ReadFixed(1, out this.ats_single_region_per_atlas_segment_flag[xAId], "ats_single_region_per_atlas_segment_flag"); 

			if (!(ats_single_region_per_atlas_segment_flag[xAId] != 0))
			{
				stream.ReadUvlc( out this.ats_num_atlas_segments_minus_1[xAId], "ats_num_atlas_segments_minus_1"); 

				for (i = 0; (i <= ats_num_atlas_segments_minus_1[xAId]); i++)
				{
					stream.ReadUvlc( out this.ats_top_left_region_column[xAId][i], "ats_top_left_region_column"); 
					stream.ReadUvlc( out this.ats_top_left_region_row[xAId][i], "ats_top_left_region_row"); 
					stream.ReadUvlc( out this.ats_bottom_right_region_column_off[xAId][i], "ats_bottom_right_region_column_off"); 
					stream.ReadUvlc( out this.ats_bottom_right_region_row_off[xAId][i], "ats_bottom_right_region_row_off"); 
				}
			}
			else 
			{
				ats_num_atlas_segments_minus_1[xAId] = (NumRegionsInAtlas[xAId] - 1);
			}
			return (ats_num_atlas_segments_minus_1[xAId] + 1);
        }

        private int WriteAtsRegionToSegmentMapping(int xAId)
        {
			int i = 0;
			this.ats_single_region_per_atlas_segment_flag[xAId] = stream.Pick("ats_single_region_per_atlas_segment_flag", _original != null ? _original.ats_single_region_per_atlas_segment_flag[xAId] : this.ats_single_region_per_atlas_segment_flag[xAId], _edited != null ? _edited.ats_single_region_per_atlas_segment_flag[xAId] : _original != null ? _original.ats_single_region_per_atlas_segment_flag[xAId] : this.ats_single_region_per_atlas_segment_flag[xAId]);
			stream.WriteFixed(1, this.ats_single_region_per_atlas_segment_flag[xAId], "ats_single_region_per_atlas_segment_flag"); 

			if (!(ats_single_region_per_atlas_segment_flag[xAId] != 0))
			{
				this.ats_num_atlas_segments_minus_1[xAId] = stream.Pick("ats_num_atlas_segments_minus_1", _original != null ? _original.ats_num_atlas_segments_minus_1[xAId] : this.ats_num_atlas_segments_minus_1[xAId], _edited != null ? _edited.ats_num_atlas_segments_minus_1[xAId] : _original != null ? _original.ats_num_atlas_segments_minus_1[xAId] : this.ats_num_atlas_segments_minus_1[xAId]);
				stream.WriteUvlc( this.ats_num_atlas_segments_minus_1[xAId], "ats_num_atlas_segments_minus_1"); 

				for (i = 0; (i <= ats_num_atlas_segments_minus_1[xAId]); i++)
				{
					this.ats_top_left_region_column[xAId][i] = stream.Pick("ats_top_left_region_column", _original != null ? _original.ats_top_left_region_column[xAId][i] : this.ats_top_left_region_column[xAId][i], _edited != null ? _edited.ats_top_left_region_column[xAId][i] : _original != null ? _original.ats_top_left_region_column[xAId][i] : this.ats_top_left_region_column[xAId][i]);
					stream.WriteUvlc( this.ats_top_left_region_column[xAId][i], "ats_top_left_region_column"); 
					this.ats_top_left_region_row[xAId][i] = stream.Pick("ats_top_left_region_row", _original != null ? _original.ats_top_left_region_row[xAId][i] : this.ats_top_left_region_row[xAId][i], _edited != null ? _edited.ats_top_left_region_row[xAId][i] : _original != null ? _original.ats_top_left_region_row[xAId][i] : this.ats_top_left_region_row[xAId][i]);
					stream.WriteUvlc( this.ats_top_left_region_row[xAId][i], "ats_top_left_region_row"); 
					this.ats_bottom_right_region_column_off[xAId][i] = stream.Pick("ats_bottom_right_region_column_off", _original != null ? _original.ats_bottom_right_region_column_off[xAId][i] : this.ats_bottom_right_region_column_off[xAId][i], _edited != null ? _edited.ats_bottom_right_region_column_off[xAId][i] : _original != null ? _original.ats_bottom_right_region_column_off[xAId][i] : this.ats_bottom_right_region_column_off[xAId][i]);
					stream.WriteUvlc( this.ats_bottom_right_region_column_off[xAId][i], "ats_bottom_right_region_column_off"); 
					this.ats_bottom_right_region_row_off[xAId][i] = stream.Pick("ats_bottom_right_region_row_off", _original != null ? _original.ats_bottom_right_region_row_off[xAId][i] : this.ats_bottom_right_region_row_off[xAId][i], _edited != null ? _edited.ats_bottom_right_region_row_off[xAId][i] : _original != null ? _original.ats_bottom_right_region_row_off[xAId][i] : this.ats_bottom_right_region_row_off[xAId][i]);
					stream.WriteUvlc( this.ats_bottom_right_region_row_off[xAId][i], "ats_bottom_right_region_row_off"); 
				}
			}
			else 
			{
				ats_num_atlas_segments_minus_1[xAId] = (NumRegionsInAtlas[xAId] - 1);
			}
			return (ats_num_atlas_segments_minus_1[xAId] + 1);
        }

    /*
ats_multistream_info( xlayerId, xAId ) {
ats_msi_width[ xlayerId ][ xAId ]	uvlc()
ats_msi_height[ xlayerId ][ xAId ]	uvlc()
AtlasWidth = ats_msi_width[ xlayerId ][ xAId ]	
AtlasHeight = ats_msi_height[ xlayerId ][ xAId ]	
ats_msi_num_atlas_segments_minus_1[ xlayerId ][ xAId ]	uvlc()
ats_msi_background_info_present_flag[ xlayerId ][ xAId ]	f(1)
if ( ats_msi_background_info_present_flag[ xlayerId ][ xAId ] ) {	
ats_msi_background_red_value[ xlayerId ][ xAId ]	f(8)
ats_msi_background_green_value[ xlayerId ][ xAId ]	f(8)
ats_msi_background_blue_value[ xlayerId ][ xAId ]	f(8)
}	
for (i=0;i<=ats_msi_num_atlas_segments_minus_1[ xlayerId ][ xAId ];i++) {	
ats_msi_input_stream_id[ xlayerId ][ xAId ][ i ]	f(5)
ats_msi_segment_top_left_pos_x[ xlayerId ][ xAId ][ i ]	uvlc()
ats_msi_segment_top_left_pos_y[ xlayerId ][ xAId ][ i ]	uvlc()
ats_msi_segment_width[ xlayerId ][ xAId ][ i ]	uvlc()
ats_msi_segment_height[ xlayerId ][ xAId ][ i ]	uvlc()
}	
return ats_msi_num_atlas_segments_minus_1[ xlayerId ][ xAId ] + 1	
}
    */
		private AomArray<AomArray<int>> ats_msi_width = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _AtsMsiWidth { get { return ats_msi_width; } set { ats_msi_width = value; } }
		private AomArray<AomArray<int>> ats_msi_height = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _AtsMsiHeight { get { return ats_msi_height; } set { ats_msi_height = value; } }
		private AomArray<AomArray<int>> ats_msi_num_atlas_segments_minus_1 = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _AtsMsiNumAtlasSegmentsMinus1 { get { return ats_msi_num_atlas_segments_minus_1; } set { ats_msi_num_atlas_segments_minus_1 = value; } }
		private AomArray<AomArray<int>> ats_msi_background_info_present_flag = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _AtsMsiBackgroundInfoPresentFlag { get { return ats_msi_background_info_present_flag; } set { ats_msi_background_info_present_flag = value; } }
		private AomArray<AomArray<int>> ats_msi_background_red_value = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _AtsMsiBackgroundRedValue { get { return ats_msi_background_red_value; } set { ats_msi_background_red_value = value; } }
		private AomArray<AomArray<int>> ats_msi_background_green_value = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _AtsMsiBackgroundGreenValue { get { return ats_msi_background_green_value; } set { ats_msi_background_green_value = value; } }
		private AomArray<AomArray<int>> ats_msi_background_blue_value = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _AtsMsiBackgroundBlueValue { get { return ats_msi_background_blue_value; } set { ats_msi_background_blue_value = value; } }
		private AomArray<AomArray<AomArray<int>>> ats_msi_input_stream_id = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _AtsMsiInputStreamId { get { return ats_msi_input_stream_id; } set { ats_msi_input_stream_id = value; } }
		private AomArray<AomArray<AomArray<int>>> ats_msi_segment_top_left_pos_x = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _AtsMsiSegmentTopLeftPosx { get { return ats_msi_segment_top_left_pos_x; } set { ats_msi_segment_top_left_pos_x = value; } }
		private AomArray<AomArray<AomArray<int>>> ats_msi_segment_top_left_pos_y = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _AtsMsiSegmentTopLeftPosy { get { return ats_msi_segment_top_left_pos_y; } set { ats_msi_segment_top_left_pos_y = value; } }
		private AomArray<AomArray<AomArray<int>>> ats_msi_segment_width = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _AtsMsiSegmentWidth { get { return ats_msi_segment_width; } set { ats_msi_segment_width = value; } }
		private AomArray<AomArray<AomArray<int>>> ats_msi_segment_height = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _AtsMsiSegmentHeight { get { return ats_msi_segment_height; } set { ats_msi_segment_height = value; } }

        private int AtsMultistreamInfo(int xlayerId, int xAId)
        {
			int i = 0;
			stream.ReadUvlc( out this.ats_msi_width[xlayerId][xAId], "ats_msi_width"); 
			stream.ReadUvlc( out this.ats_msi_height[xlayerId][xAId], "ats_msi_height"); 
			AtlasWidth = ats_msi_width[xlayerId][xAId];
			AtlasHeight = ats_msi_height[xlayerId][xAId];
			stream.ReadUvlc( out this.ats_msi_num_atlas_segments_minus_1[xlayerId][xAId], "ats_msi_num_atlas_segments_minus_1"); 
			stream.ReadFixed(1, out this.ats_msi_background_info_present_flag[xlayerId][xAId], "ats_msi_background_info_present_flag"); 

			if ((ats_msi_background_info_present_flag[xlayerId][xAId] != 0))
			{
				stream.ReadFixed(8, out this.ats_msi_background_red_value[xlayerId][xAId], "ats_msi_background_red_value"); 
				stream.ReadFixed(8, out this.ats_msi_background_green_value[xlayerId][xAId], "ats_msi_background_green_value"); 
				stream.ReadFixed(8, out this.ats_msi_background_blue_value[xlayerId][xAId], "ats_msi_background_blue_value"); 
			}

			for (i = 0; (i <= ats_msi_num_atlas_segments_minus_1[xlayerId][xAId]); i++)
			{
				stream.ReadFixed(5, out this.ats_msi_input_stream_id[xlayerId][xAId][i], "ats_msi_input_stream_id"); 
				stream.ReadUvlc( out this.ats_msi_segment_top_left_pos_x[xlayerId][xAId][i], "ats_msi_segment_top_left_pos_x"); 
				stream.ReadUvlc( out this.ats_msi_segment_top_left_pos_y[xlayerId][xAId][i], "ats_msi_segment_top_left_pos_y"); 
				stream.ReadUvlc( out this.ats_msi_segment_width[xlayerId][xAId][i], "ats_msi_segment_width"); 
				stream.ReadUvlc( out this.ats_msi_segment_height[xlayerId][xAId][i], "ats_msi_segment_height"); 
			}
			return (ats_msi_num_atlas_segments_minus_1[xlayerId][xAId] + 1);
        }

        private int WriteAtsMultistreamInfo(int xlayerId, int xAId)
        {
			int i = 0;
			this.ats_msi_width[xlayerId][xAId] = stream.Pick("ats_msi_width", _original != null ? _original.AtlasWidth : this.ats_msi_width[xlayerId][xAId], _edited != null ? _edited.AtlasWidth : _original != null ? _original.AtlasWidth : this.ats_msi_width[xlayerId][xAId]);
			stream.WriteUvlc( this.ats_msi_width[xlayerId][xAId], "ats_msi_width"); 
			this.ats_msi_height[xlayerId][xAId] = stream.Pick("ats_msi_height", _original != null ? _original.AtlasHeight : this.ats_msi_height[xlayerId][xAId], _edited != null ? _edited.AtlasHeight : _original != null ? _original.AtlasHeight : this.ats_msi_height[xlayerId][xAId]);
			stream.WriteUvlc( this.ats_msi_height[xlayerId][xAId], "ats_msi_height"); 
			AtlasWidth = ats_msi_width[xlayerId][xAId];
			AtlasHeight = ats_msi_height[xlayerId][xAId];
			this.ats_msi_num_atlas_segments_minus_1[xlayerId][xAId] = stream.Pick("ats_msi_num_atlas_segments_minus_1", _original != null ? _original.ats_msi_num_atlas_segments_minus_1[xlayerId][xAId] : this.ats_msi_num_atlas_segments_minus_1[xlayerId][xAId], _edited != null ? _edited.ats_msi_num_atlas_segments_minus_1[xlayerId][xAId] : _original != null ? _original.ats_msi_num_atlas_segments_minus_1[xlayerId][xAId] : this.ats_msi_num_atlas_segments_minus_1[xlayerId][xAId]);
			stream.WriteUvlc( this.ats_msi_num_atlas_segments_minus_1[xlayerId][xAId], "ats_msi_num_atlas_segments_minus_1"); 
			this.ats_msi_background_info_present_flag[xlayerId][xAId] = stream.Pick("ats_msi_background_info_present_flag", _original != null ? _original.ats_msi_background_info_present_flag[xlayerId][xAId] : this.ats_msi_background_info_present_flag[xlayerId][xAId], _edited != null ? _edited.ats_msi_background_info_present_flag[xlayerId][xAId] : _original != null ? _original.ats_msi_background_info_present_flag[xlayerId][xAId] : this.ats_msi_background_info_present_flag[xlayerId][xAId]);
			stream.WriteFixed(1, this.ats_msi_background_info_present_flag[xlayerId][xAId], "ats_msi_background_info_present_flag"); 

			if ((ats_msi_background_info_present_flag[xlayerId][xAId] != 0))
			{
				this.ats_msi_background_red_value[xlayerId][xAId] = stream.Pick("ats_msi_background_red_value", _original != null ? _original.ats_msi_background_red_value[xlayerId][xAId] : this.ats_msi_background_red_value[xlayerId][xAId], _edited != null ? _edited.ats_msi_background_red_value[xlayerId][xAId] : _original != null ? _original.ats_msi_background_red_value[xlayerId][xAId] : this.ats_msi_background_red_value[xlayerId][xAId]);
				stream.WriteFixed(8, this.ats_msi_background_red_value[xlayerId][xAId], "ats_msi_background_red_value"); 
				this.ats_msi_background_green_value[xlayerId][xAId] = stream.Pick("ats_msi_background_green_value", _original != null ? _original.ats_msi_background_green_value[xlayerId][xAId] : this.ats_msi_background_green_value[xlayerId][xAId], _edited != null ? _edited.ats_msi_background_green_value[xlayerId][xAId] : _original != null ? _original.ats_msi_background_green_value[xlayerId][xAId] : this.ats_msi_background_green_value[xlayerId][xAId]);
				stream.WriteFixed(8, this.ats_msi_background_green_value[xlayerId][xAId], "ats_msi_background_green_value"); 
				this.ats_msi_background_blue_value[xlayerId][xAId] = stream.Pick("ats_msi_background_blue_value", _original != null ? _original.ats_msi_background_blue_value[xlayerId][xAId] : this.ats_msi_background_blue_value[xlayerId][xAId], _edited != null ? _edited.ats_msi_background_blue_value[xlayerId][xAId] : _original != null ? _original.ats_msi_background_blue_value[xlayerId][xAId] : this.ats_msi_background_blue_value[xlayerId][xAId]);
				stream.WriteFixed(8, this.ats_msi_background_blue_value[xlayerId][xAId], "ats_msi_background_blue_value"); 
			}

			for (i = 0; (i <= ats_msi_num_atlas_segments_minus_1[xlayerId][xAId]); i++)
			{
				this.ats_msi_input_stream_id[xlayerId][xAId][i] = stream.Pick("ats_msi_input_stream_id", _original != null ? _original.ats_msi_input_stream_id[xlayerId][xAId][i] : this.ats_msi_input_stream_id[xlayerId][xAId][i], _edited != null ? _edited.ats_msi_input_stream_id[xlayerId][xAId][i] : _original != null ? _original.ats_msi_input_stream_id[xlayerId][xAId][i] : this.ats_msi_input_stream_id[xlayerId][xAId][i]);
				stream.WriteFixed(5, this.ats_msi_input_stream_id[xlayerId][xAId][i], "ats_msi_input_stream_id"); 
				this.ats_msi_segment_top_left_pos_x[xlayerId][xAId][i] = stream.Pick("ats_msi_segment_top_left_pos_x", _original != null ? _original.ats_msi_segment_top_left_pos_x[xlayerId][xAId][i] : this.ats_msi_segment_top_left_pos_x[xlayerId][xAId][i], _edited != null ? _edited.ats_msi_segment_top_left_pos_x[xlayerId][xAId][i] : _original != null ? _original.ats_msi_segment_top_left_pos_x[xlayerId][xAId][i] : this.ats_msi_segment_top_left_pos_x[xlayerId][xAId][i]);
				stream.WriteUvlc( this.ats_msi_segment_top_left_pos_x[xlayerId][xAId][i], "ats_msi_segment_top_left_pos_x"); 
				this.ats_msi_segment_top_left_pos_y[xlayerId][xAId][i] = stream.Pick("ats_msi_segment_top_left_pos_y", _original != null ? _original.ats_msi_segment_top_left_pos_y[xlayerId][xAId][i] : this.ats_msi_segment_top_left_pos_y[xlayerId][xAId][i], _edited != null ? _edited.ats_msi_segment_top_left_pos_y[xlayerId][xAId][i] : _original != null ? _original.ats_msi_segment_top_left_pos_y[xlayerId][xAId][i] : this.ats_msi_segment_top_left_pos_y[xlayerId][xAId][i]);
				stream.WriteUvlc( this.ats_msi_segment_top_left_pos_y[xlayerId][xAId][i], "ats_msi_segment_top_left_pos_y"); 
				this.ats_msi_segment_width[xlayerId][xAId][i] = stream.Pick("ats_msi_segment_width", _original != null ? _original.ats_msi_segment_width[xlayerId][xAId][i] : this.ats_msi_segment_width[xlayerId][xAId][i], _edited != null ? _edited.ats_msi_segment_width[xlayerId][xAId][i] : _original != null ? _original.ats_msi_segment_width[xlayerId][xAId][i] : this.ats_msi_segment_width[xlayerId][xAId][i]);
				stream.WriteUvlc( this.ats_msi_segment_width[xlayerId][xAId][i], "ats_msi_segment_width"); 
				this.ats_msi_segment_height[xlayerId][xAId][i] = stream.Pick("ats_msi_segment_height", _original != null ? _original.ats_msi_segment_height[xlayerId][xAId][i] : this.ats_msi_segment_height[xlayerId][xAId][i], _edited != null ? _edited.ats_msi_segment_height[xlayerId][xAId][i] : _original != null ? _original.ats_msi_segment_height[xlayerId][xAId][i] : this.ats_msi_segment_height[xlayerId][xAId][i]);
				stream.WriteUvlc( this.ats_msi_segment_height[xlayerId][xAId][i], "ats_msi_segment_height"); 
			}
			return (ats_msi_num_atlas_segments_minus_1[xlayerId][xAId] + 1);
        }

    /*
ats_multistream_with_alpha_info( xlayerId, xAId ) {
ats_msi_width[ xlayerId ][ xAId ]	uvlc()
ats_msi_height[ xlayerId ][ xAId ]	uvlc()
AtlasWidth = ats_msi_width[ xlayerId ][ xAId ]	
AtlasHeight = ats_msi_height[ xlayerId ][ xAId ]	
ats_msi_num_atlas_segments_minus_1[ xlayerId ][ xAId ]	uvlc()
ats_msi_alpha_segments_present_flag[ xlayerId ][ xAId ]	f(1)
ats_msi_background_info_present_flag[ xlayerId ][ xAId ]	f(1)
if ( ats_msi_background_info_present_flag[ xlayerId ][ xAId ] ) {	
ats_msi_background_red_value[ xlayerId ][ xAId ]	f(8)
ats_msi_background_green_value[ xlayerId ][ xAId ]	f(8)
ats_msi_background_blue_value[ xlayerId ][ xAId ]	f(8)
}	
for (i=0;i<=ats_msi_num_atlas_segments_minus_1[ xlayerId ][ xAId ];i++) {	
ats_msi_input_stream_id[ xlayerId ][ xAId ][ i ]	f(5)
ats_msi_segment_top_left_pos_x[ xlayerId ][ xAId ][ i ]	uvlc()
ats_msi_segment_top_left_pos_y[ xlayerId ][ xAId ][ i ]	uvlc()
ats_msi_segment_width[ xlayerId ][ xAId ][ i ]	uvlc()
ats_msi_segment_height[ xlayerId ][ xAId ][ i ]	uvlc()
if ( ats_msi_alpha_segments_present_flag[ xlayerId ][ xAId ] &&	
i != ats_msi_num_atlas_segments_minus_1[ xlayerId ][ xAId ] ) {	
ats_msi_alpha_segment_flag[ xlayerId ][ xAId ][ i ]	f(1)
} else {	
ats_msi_alpha_segment_flag[ xlayerId ][ xAId ][ i ] = 0	
}	
}	
return ats_msi_num_atlas_segments_minus_1[ xlayerId ][ xAId ] + 1	
}
    */
		private AomArray<AomArray<int>> ats_msi_alpha_segments_present_flag = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _AtsMsiAlphaSegmentsPresentFlag { get { return ats_msi_alpha_segments_present_flag; } set { ats_msi_alpha_segments_present_flag = value; } }
		private AomArray<AomArray<AomArray<int>>> ats_msi_alpha_segment_flag = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _AtsMsiAlphaSegmentFlag { get { return ats_msi_alpha_segment_flag; } set { ats_msi_alpha_segment_flag = value; } }

        private int AtsMultistreamWithAlphaInfo(int xlayerId, int xAId)
        {
			int i = 0;
			stream.ReadUvlc( out this.ats_msi_width[xlayerId][xAId], "ats_msi_width"); 
			stream.ReadUvlc( out this.ats_msi_height[xlayerId][xAId], "ats_msi_height"); 
			AtlasWidth = ats_msi_width[xlayerId][xAId];
			AtlasHeight = ats_msi_height[xlayerId][xAId];
			stream.ReadUvlc( out this.ats_msi_num_atlas_segments_minus_1[xlayerId][xAId], "ats_msi_num_atlas_segments_minus_1"); 
			stream.ReadFixed(1, out this.ats_msi_alpha_segments_present_flag[xlayerId][xAId], "ats_msi_alpha_segments_present_flag"); 
			stream.ReadFixed(1, out this.ats_msi_background_info_present_flag[xlayerId][xAId], "ats_msi_background_info_present_flag"); 

			if ((ats_msi_background_info_present_flag[xlayerId][xAId] != 0))
			{
				stream.ReadFixed(8, out this.ats_msi_background_red_value[xlayerId][xAId], "ats_msi_background_red_value"); 
				stream.ReadFixed(8, out this.ats_msi_background_green_value[xlayerId][xAId], "ats_msi_background_green_value"); 
				stream.ReadFixed(8, out this.ats_msi_background_blue_value[xlayerId][xAId], "ats_msi_background_blue_value"); 
			}

			for (i = 0; (i <= ats_msi_num_atlas_segments_minus_1[xlayerId][xAId]); i++)
			{
				stream.ReadFixed(5, out this.ats_msi_input_stream_id[xlayerId][xAId][i], "ats_msi_input_stream_id"); 
				stream.ReadUvlc( out this.ats_msi_segment_top_left_pos_x[xlayerId][xAId][i], "ats_msi_segment_top_left_pos_x"); 
				stream.ReadUvlc( out this.ats_msi_segment_top_left_pos_y[xlayerId][xAId][i], "ats_msi_segment_top_left_pos_y"); 
				stream.ReadUvlc( out this.ats_msi_segment_width[xlayerId][xAId][i], "ats_msi_segment_width"); 
				stream.ReadUvlc( out this.ats_msi_segment_height[xlayerId][xAId][i], "ats_msi_segment_height"); 

				if (((ats_msi_alpha_segments_present_flag[xlayerId][xAId] != 0) && (i != ats_msi_num_atlas_segments_minus_1[xlayerId][xAId])))
				{
					stream.ReadFixed(1, out this.ats_msi_alpha_segment_flag[xlayerId][xAId][i], "ats_msi_alpha_segment_flag"); 
				}
				else 
				{
					ats_msi_alpha_segment_flag[xlayerId][xAId][i] = 0;
				}
			}
			return (ats_msi_num_atlas_segments_minus_1[xlayerId][xAId] + 1);
        }

        private int WriteAtsMultistreamWithAlphaInfo(int xlayerId, int xAId)
        {
			int i = 0;
			this.ats_msi_width[xlayerId][xAId] = stream.Pick("ats_msi_width", _original != null ? _original.AtlasWidth : this.ats_msi_width[xlayerId][xAId], _edited != null ? _edited.AtlasWidth : _original != null ? _original.AtlasWidth : this.ats_msi_width[xlayerId][xAId]);
			stream.WriteUvlc( this.ats_msi_width[xlayerId][xAId], "ats_msi_width"); 
			this.ats_msi_height[xlayerId][xAId] = stream.Pick("ats_msi_height", _original != null ? _original.AtlasHeight : this.ats_msi_height[xlayerId][xAId], _edited != null ? _edited.AtlasHeight : _original != null ? _original.AtlasHeight : this.ats_msi_height[xlayerId][xAId]);
			stream.WriteUvlc( this.ats_msi_height[xlayerId][xAId], "ats_msi_height"); 
			AtlasWidth = ats_msi_width[xlayerId][xAId];
			AtlasHeight = ats_msi_height[xlayerId][xAId];
			this.ats_msi_num_atlas_segments_minus_1[xlayerId][xAId] = stream.Pick("ats_msi_num_atlas_segments_minus_1", _original != null ? _original.ats_msi_num_atlas_segments_minus_1[xlayerId][xAId] : this.ats_msi_num_atlas_segments_minus_1[xlayerId][xAId], _edited != null ? _edited.ats_msi_num_atlas_segments_minus_1[xlayerId][xAId] : _original != null ? _original.ats_msi_num_atlas_segments_minus_1[xlayerId][xAId] : this.ats_msi_num_atlas_segments_minus_1[xlayerId][xAId]);
			stream.WriteUvlc( this.ats_msi_num_atlas_segments_minus_1[xlayerId][xAId], "ats_msi_num_atlas_segments_minus_1"); 
			this.ats_msi_alpha_segments_present_flag[xlayerId][xAId] = stream.Pick("ats_msi_alpha_segments_present_flag", _original != null ? _original.ats_msi_alpha_segments_present_flag[xlayerId][xAId] : this.ats_msi_alpha_segments_present_flag[xlayerId][xAId], _edited != null ? _edited.ats_msi_alpha_segments_present_flag[xlayerId][xAId] : _original != null ? _original.ats_msi_alpha_segments_present_flag[xlayerId][xAId] : this.ats_msi_alpha_segments_present_flag[xlayerId][xAId]);
			stream.WriteFixed(1, this.ats_msi_alpha_segments_present_flag[xlayerId][xAId], "ats_msi_alpha_segments_present_flag"); 
			this.ats_msi_background_info_present_flag[xlayerId][xAId] = stream.Pick("ats_msi_background_info_present_flag", _original != null ? _original.ats_msi_background_info_present_flag[xlayerId][xAId] : this.ats_msi_background_info_present_flag[xlayerId][xAId], _edited != null ? _edited.ats_msi_background_info_present_flag[xlayerId][xAId] : _original != null ? _original.ats_msi_background_info_present_flag[xlayerId][xAId] : this.ats_msi_background_info_present_flag[xlayerId][xAId]);
			stream.WriteFixed(1, this.ats_msi_background_info_present_flag[xlayerId][xAId], "ats_msi_background_info_present_flag"); 

			if ((ats_msi_background_info_present_flag[xlayerId][xAId] != 0))
			{
				this.ats_msi_background_red_value[xlayerId][xAId] = stream.Pick("ats_msi_background_red_value", _original != null ? _original.ats_msi_background_red_value[xlayerId][xAId] : this.ats_msi_background_red_value[xlayerId][xAId], _edited != null ? _edited.ats_msi_background_red_value[xlayerId][xAId] : _original != null ? _original.ats_msi_background_red_value[xlayerId][xAId] : this.ats_msi_background_red_value[xlayerId][xAId]);
				stream.WriteFixed(8, this.ats_msi_background_red_value[xlayerId][xAId], "ats_msi_background_red_value"); 
				this.ats_msi_background_green_value[xlayerId][xAId] = stream.Pick("ats_msi_background_green_value", _original != null ? _original.ats_msi_background_green_value[xlayerId][xAId] : this.ats_msi_background_green_value[xlayerId][xAId], _edited != null ? _edited.ats_msi_background_green_value[xlayerId][xAId] : _original != null ? _original.ats_msi_background_green_value[xlayerId][xAId] : this.ats_msi_background_green_value[xlayerId][xAId]);
				stream.WriteFixed(8, this.ats_msi_background_green_value[xlayerId][xAId], "ats_msi_background_green_value"); 
				this.ats_msi_background_blue_value[xlayerId][xAId] = stream.Pick("ats_msi_background_blue_value", _original != null ? _original.ats_msi_background_blue_value[xlayerId][xAId] : this.ats_msi_background_blue_value[xlayerId][xAId], _edited != null ? _edited.ats_msi_background_blue_value[xlayerId][xAId] : _original != null ? _original.ats_msi_background_blue_value[xlayerId][xAId] : this.ats_msi_background_blue_value[xlayerId][xAId]);
				stream.WriteFixed(8, this.ats_msi_background_blue_value[xlayerId][xAId], "ats_msi_background_blue_value"); 
			}

			for (i = 0; (i <= ats_msi_num_atlas_segments_minus_1[xlayerId][xAId]); i++)
			{
				this.ats_msi_input_stream_id[xlayerId][xAId][i] = stream.Pick("ats_msi_input_stream_id", _original != null ? _original.ats_msi_input_stream_id[xlayerId][xAId][i] : this.ats_msi_input_stream_id[xlayerId][xAId][i], _edited != null ? _edited.ats_msi_input_stream_id[xlayerId][xAId][i] : _original != null ? _original.ats_msi_input_stream_id[xlayerId][xAId][i] : this.ats_msi_input_stream_id[xlayerId][xAId][i]);
				stream.WriteFixed(5, this.ats_msi_input_stream_id[xlayerId][xAId][i], "ats_msi_input_stream_id"); 
				this.ats_msi_segment_top_left_pos_x[xlayerId][xAId][i] = stream.Pick("ats_msi_segment_top_left_pos_x", _original != null ? _original.ats_msi_segment_top_left_pos_x[xlayerId][xAId][i] : this.ats_msi_segment_top_left_pos_x[xlayerId][xAId][i], _edited != null ? _edited.ats_msi_segment_top_left_pos_x[xlayerId][xAId][i] : _original != null ? _original.ats_msi_segment_top_left_pos_x[xlayerId][xAId][i] : this.ats_msi_segment_top_left_pos_x[xlayerId][xAId][i]);
				stream.WriteUvlc( this.ats_msi_segment_top_left_pos_x[xlayerId][xAId][i], "ats_msi_segment_top_left_pos_x"); 
				this.ats_msi_segment_top_left_pos_y[xlayerId][xAId][i] = stream.Pick("ats_msi_segment_top_left_pos_y", _original != null ? _original.ats_msi_segment_top_left_pos_y[xlayerId][xAId][i] : this.ats_msi_segment_top_left_pos_y[xlayerId][xAId][i], _edited != null ? _edited.ats_msi_segment_top_left_pos_y[xlayerId][xAId][i] : _original != null ? _original.ats_msi_segment_top_left_pos_y[xlayerId][xAId][i] : this.ats_msi_segment_top_left_pos_y[xlayerId][xAId][i]);
				stream.WriteUvlc( this.ats_msi_segment_top_left_pos_y[xlayerId][xAId][i], "ats_msi_segment_top_left_pos_y"); 
				this.ats_msi_segment_width[xlayerId][xAId][i] = stream.Pick("ats_msi_segment_width", _original != null ? _original.ats_msi_segment_width[xlayerId][xAId][i] : this.ats_msi_segment_width[xlayerId][xAId][i], _edited != null ? _edited.ats_msi_segment_width[xlayerId][xAId][i] : _original != null ? _original.ats_msi_segment_width[xlayerId][xAId][i] : this.ats_msi_segment_width[xlayerId][xAId][i]);
				stream.WriteUvlc( this.ats_msi_segment_width[xlayerId][xAId][i], "ats_msi_segment_width"); 
				this.ats_msi_segment_height[xlayerId][xAId][i] = stream.Pick("ats_msi_segment_height", _original != null ? _original.ats_msi_segment_height[xlayerId][xAId][i] : this.ats_msi_segment_height[xlayerId][xAId][i], _edited != null ? _edited.ats_msi_segment_height[xlayerId][xAId][i] : _original != null ? _original.ats_msi_segment_height[xlayerId][xAId][i] : this.ats_msi_segment_height[xlayerId][xAId][i]);
				stream.WriteUvlc( this.ats_msi_segment_height[xlayerId][xAId][i], "ats_msi_segment_height"); 

				if (((ats_msi_alpha_segments_present_flag[xlayerId][xAId] != 0) && (i != ats_msi_num_atlas_segments_minus_1[xlayerId][xAId])))
				{
					this.ats_msi_alpha_segment_flag[xlayerId][xAId][i] = stream.Pick("ats_msi_alpha_segment_flag", _original != null ? _original.ats_msi_alpha_segment_flag[xlayerId][xAId][i] : this.ats_msi_alpha_segment_flag[xlayerId][xAId][i], _edited != null ? _edited.ats_msi_alpha_segment_flag[xlayerId][xAId][i] : _original != null ? _original.ats_msi_alpha_segment_flag[xlayerId][xAId][i] : this.ats_msi_alpha_segment_flag[xlayerId][xAId][i]);
					stream.WriteFixed(1, this.ats_msi_alpha_segment_flag[xlayerId][xAId][i], "ats_msi_alpha_segment_flag"); 
				}
				else 
				{
					ats_msi_alpha_segment_flag[xlayerId][xAId][i] = 0;
				}
			}
			return (ats_msi_num_atlas_segments_minus_1[xlayerId][xAId] + 1);
        }

    /*
ats_basic_info( xAId ) {
ats_stream_id_present[ xAId ]	f(1)
ats_width[ xAId ]	uvlc()
ats_height[ xAId ]	uvlc()
ats_num_atlas_segments_minus_1[ xAId ]	uvlc()
AtlasWidth = ats_width[ xAId ]	
AtlasHeight = ats_height[ xAId ]	
for ( i = 0; i <= ats_num_atlas_segments_minus_1[ xAId ]; i++ ) {	
if (ats_stream_id_present[ xAId ]) {	
ats_input_stream_id[ xAId ][ i ]	f(5)
}	
ats_segment_top_left_pos_x[ xAId ][ i ]	uvlc()
ats_segment_top_left_pos_y[ xAId ][ i ]	uvlc()
ats_segment_width[ xAId ][ i ]	uvlc()
ats_segment_height[ xAId ][ i ]	uvlc()
}	
return ats_num_atlas_segments_minus_1[ xAId ] + 1	
}
    */
		private AomArray<int> ats_stream_id_present = new AomArray<int>();
		public AomArray<int> _AtsStreamIdPresent { get { return ats_stream_id_present; } set { ats_stream_id_present = value; } }
		private AomArray<int> ats_width = new AomArray<int>();
		public AomArray<int> _AtsWidth { get { return ats_width; } set { ats_width = value; } }
		private AomArray<int> ats_height = new AomArray<int>();
		public AomArray<int> _AtsHeight { get { return ats_height; } set { ats_height = value; } }
		private AomArray<AomArray<int>> ats_input_stream_id = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _AtsInputStreamId { get { return ats_input_stream_id; } set { ats_input_stream_id = value; } }
		private AomArray<AomArray<int>> ats_segment_top_left_pos_x = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _AtsSegmentTopLeftPosx { get { return ats_segment_top_left_pos_x; } set { ats_segment_top_left_pos_x = value; } }
		private AomArray<AomArray<int>> ats_segment_top_left_pos_y = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _AtsSegmentTopLeftPosy { get { return ats_segment_top_left_pos_y; } set { ats_segment_top_left_pos_y = value; } }
		private AomArray<AomArray<int>> ats_segment_width = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _AtsSegmentWidth { get { return ats_segment_width; } set { ats_segment_width = value; } }
		private AomArray<AomArray<int>> ats_segment_height = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _AtsSegmentHeight { get { return ats_segment_height; } set { ats_segment_height = value; } }

        private int AtsBasicInfo(int xAId)
        {
			int i = 0;
			stream.ReadFixed(1, out this.ats_stream_id_present[xAId], "ats_stream_id_present"); 
			stream.ReadUvlc( out this.ats_width[xAId], "ats_width"); 
			stream.ReadUvlc( out this.ats_height[xAId], "ats_height"); 
			stream.ReadUvlc( out this.ats_num_atlas_segments_minus_1[xAId], "ats_num_atlas_segments_minus_1"); 
			AtlasWidth = ats_width[xAId];
			AtlasHeight = ats_height[xAId];

			for (i = 0; (i <= ats_num_atlas_segments_minus_1[xAId]); i++)
			{

				if ((ats_stream_id_present[xAId] != 0))
				{
					stream.ReadFixed(5, out this.ats_input_stream_id[xAId][i], "ats_input_stream_id"); 
				}
				stream.ReadUvlc( out this.ats_segment_top_left_pos_x[xAId][i], "ats_segment_top_left_pos_x"); 
				stream.ReadUvlc( out this.ats_segment_top_left_pos_y[xAId][i], "ats_segment_top_left_pos_y"); 
				stream.ReadUvlc( out this.ats_segment_width[xAId][i], "ats_segment_width"); 
				stream.ReadUvlc( out this.ats_segment_height[xAId][i], "ats_segment_height"); 
			}
			return (ats_num_atlas_segments_minus_1[xAId] + 1);
        }

        private int WriteAtsBasicInfo(int xAId)
        {
			int i = 0;
			this.ats_stream_id_present[xAId] = stream.Pick("ats_stream_id_present", _original != null ? _original.ats_stream_id_present[xAId] : this.ats_stream_id_present[xAId], _edited != null ? _edited.ats_stream_id_present[xAId] : _original != null ? _original.ats_stream_id_present[xAId] : this.ats_stream_id_present[xAId]);
			stream.WriteFixed(1, this.ats_stream_id_present[xAId], "ats_stream_id_present"); 
			this.ats_width[xAId] = stream.Pick("ats_width", _original != null ? _original.AtlasWidth : this.ats_width[xAId], _edited != null ? _edited.AtlasWidth : _original != null ? _original.AtlasWidth : this.ats_width[xAId]);
			stream.WriteUvlc( this.ats_width[xAId], "ats_width"); 
			this.ats_height[xAId] = stream.Pick("ats_height", _original != null ? _original.AtlasHeight : this.ats_height[xAId], _edited != null ? _edited.AtlasHeight : _original != null ? _original.AtlasHeight : this.ats_height[xAId]);
			stream.WriteUvlc( this.ats_height[xAId], "ats_height"); 
			this.ats_num_atlas_segments_minus_1[xAId] = stream.Pick("ats_num_atlas_segments_minus_1", _original != null ? _original.ats_num_atlas_segments_minus_1[xAId] : this.ats_num_atlas_segments_minus_1[xAId], _edited != null ? _edited.ats_num_atlas_segments_minus_1[xAId] : _original != null ? _original.ats_num_atlas_segments_minus_1[xAId] : this.ats_num_atlas_segments_minus_1[xAId]);
			stream.WriteUvlc( this.ats_num_atlas_segments_minus_1[xAId], "ats_num_atlas_segments_minus_1"); 
			AtlasWidth = ats_width[xAId];
			AtlasHeight = ats_height[xAId];

			for (i = 0; (i <= ats_num_atlas_segments_minus_1[xAId]); i++)
			{

				if ((ats_stream_id_present[xAId] != 0))
				{
					this.ats_input_stream_id[xAId][i] = stream.Pick("ats_input_stream_id", _original != null ? _original.ats_input_stream_id[xAId][i] : this.ats_input_stream_id[xAId][i], _edited != null ? _edited.ats_input_stream_id[xAId][i] : _original != null ? _original.ats_input_stream_id[xAId][i] : this.ats_input_stream_id[xAId][i]);
					stream.WriteFixed(5, this.ats_input_stream_id[xAId][i], "ats_input_stream_id"); 
				}
				this.ats_segment_top_left_pos_x[xAId][i] = stream.Pick("ats_segment_top_left_pos_x", _original != null ? _original.ats_segment_top_left_pos_x[xAId][i] : this.ats_segment_top_left_pos_x[xAId][i], _edited != null ? _edited.ats_segment_top_left_pos_x[xAId][i] : _original != null ? _original.ats_segment_top_left_pos_x[xAId][i] : this.ats_segment_top_left_pos_x[xAId][i]);
				stream.WriteUvlc( this.ats_segment_top_left_pos_x[xAId][i], "ats_segment_top_left_pos_x"); 
				this.ats_segment_top_left_pos_y[xAId][i] = stream.Pick("ats_segment_top_left_pos_y", _original != null ? _original.ats_segment_top_left_pos_y[xAId][i] : this.ats_segment_top_left_pos_y[xAId][i], _edited != null ? _edited.ats_segment_top_left_pos_y[xAId][i] : _original != null ? _original.ats_segment_top_left_pos_y[xAId][i] : this.ats_segment_top_left_pos_y[xAId][i]);
				stream.WriteUvlc( this.ats_segment_top_left_pos_y[xAId][i], "ats_segment_top_left_pos_y"); 
				this.ats_segment_width[xAId][i] = stream.Pick("ats_segment_width", _original != null ? _original.ats_segment_width[xAId][i] : this.ats_segment_width[xAId][i], _edited != null ? _edited.ats_segment_width[xAId][i] : _original != null ? _original.ats_segment_width[xAId][i] : this.ats_segment_width[xAId][i]);
				stream.WriteUvlc( this.ats_segment_width[xAId][i], "ats_segment_width"); 
				this.ats_segment_height[xAId][i] = stream.Pick("ats_segment_height", _original != null ? _original.ats_segment_height[xAId][i] : this.ats_segment_height[xAId][i], _edited != null ? _edited.ats_segment_height[xAId][i] : _original != null ? _original.ats_segment_height[xAId][i] : this.ats_segment_height[xAId][i]);
				stream.WriteUvlc( this.ats_segment_height[xAId][i], "ats_segment_height"); 
			}
			return (ats_num_atlas_segments_minus_1[xAId] + 1);
        }

    /*
operating_point_set_obu() {
ops_reset_flag[ obu_xlayer_id ]	f(1)
ops_id[ obu_xlayer_id ]	f(4)
opsID = ops_id[ obu_xlayer_id ]	
ops_cnt[ obu_xlayer_id ][ opsID ]	f(3)
if ( ops_cnt[ obu_xlayer_id ][ opsID ] > 0 ) {	
ops_priority[ obu_xlayer_id ][ opsID ]	f(4)
ops_intent[ obu_xlayer_id ][ opsID ]	f(7)
ops_intent_present_flag[ obu_xlayer_id ][ opsID ]	f(1)
ops_ptl_present_flag[ obu_xlayer_id ][ opsID ]	f(1)
ops_color_info_present_flag[ obu_xlayer_id ][ opsID ]	f(1)
if ( obu_xlayer_id == GLOBAL_XLAYER_ID ) {	
ops_mlayer_info_idc[ opsID ]	f(2)
} else {	
ops_reserved_2bits	f(2)
}	
for( i = 0; i < ops_cnt[ obu_xlayer_id ][ opsID ]; i++ ) {	
operating_point_payload( obu_xlayer_id, opsID, i )	
}	
}	
}
    */
		private AomArray<int> ops_reset_flag = new AomArray<int>();
		public AomArray<int> _OpsResetFlag { get { return ops_reset_flag; } set { ops_reset_flag = value; } }
		private AomArray<int> ops_id = new AomArray<int>();
		public AomArray<int> _OpsId { get { return ops_id; } set { ops_id = value; } }
		private AomArray<AomArray<int>> ops_cnt = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _OpsCnt { get { return ops_cnt; } set { ops_cnt = value; } }
		private AomArray<AomArray<int>> ops_priority = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _OpsPriority { get { return ops_priority; } set { ops_priority = value; } }
		private AomArray<AomArray<int>> ops_intent = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _OpsIntent { get { return ops_intent; } set { ops_intent = value; } }
		private AomArray<AomArray<int>> ops_intent_present_flag = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _OpsIntentPresentFlag { get { return ops_intent_present_flag; } set { ops_intent_present_flag = value; } }
		private AomArray<AomArray<int>> ops_ptl_present_flag = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _OpsPtlPresentFlag { get { return ops_ptl_present_flag; } set { ops_ptl_present_flag = value; } }
		private AomArray<AomArray<int>> ops_color_info_present_flag = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _OpsColorInfoPresentFlag { get { return ops_color_info_present_flag; } set { ops_color_info_present_flag = value; } }
		private AomArray<int> ops_mlayer_info_idc = new AomArray<int>();
		public AomArray<int> _OpsMlayerInfoIdc { get { return ops_mlayer_info_idc; } set { ops_mlayer_info_idc = value; } }
		private int ops_reserved_2bits;
		public int _OpsReserved2bits { get { return ops_reserved_2bits; } set { ops_reserved_2bits = value; } }

        private void OperatingPointSetObu()
        {
			int i = 0;
			int opsID = 0;
			stream.ReadFixed(1, out this.ops_reset_flag[obu_xlayer_id], "ops_reset_flag"); 
			stream.ReadFixed(4, out this.ops_id[obu_xlayer_id], "ops_id"); 
			opsID = ops_id[obu_xlayer_id];
			stream.ReadFixed(3, out this.ops_cnt[obu_xlayer_id][opsID], "ops_cnt"); 

			if ((ops_cnt[obu_xlayer_id][opsID] > 0))
			{
				stream.ReadFixed(4, out this.ops_priority[obu_xlayer_id][opsID], "ops_priority"); 
				stream.ReadFixed(7, out this.ops_intent[obu_xlayer_id][opsID], "ops_intent"); 
				stream.ReadFixed(1, out this.ops_intent_present_flag[obu_xlayer_id][opsID], "ops_intent_present_flag"); 
				stream.ReadFixed(1, out this.ops_ptl_present_flag[obu_xlayer_id][opsID], "ops_ptl_present_flag"); 
				stream.ReadFixed(1, out this.ops_color_info_present_flag[obu_xlayer_id][opsID], "ops_color_info_present_flag"); 

				if ((obu_xlayer_id == GLOBAL_XLAYER_ID))
				{
					stream.ReadFixed(2, out this.ops_mlayer_info_idc[opsID], "ops_mlayer_info_idc"); 
				}
				else 
				{
					stream.ReadFixed(2, out this.ops_reserved_2bits, "ops_reserved_2bits"); 
				}

				for (i = 0; (i < ops_cnt[obu_xlayer_id][opsID]); i++)
				{
					OperatingPointPayload(obu_xlayer_id, opsID, i); 
				}
			}
        }

        private void WriteOperatingPointSetObu()
        {
			int i = 0;
			int opsID = 0;
			this.ops_reset_flag[obu_xlayer_id] = stream.Pick("ops_reset_flag", _original != null ? _original.ops_reset_flag[obu_xlayer_id] : this.ops_reset_flag[obu_xlayer_id], _edited != null ? _edited.ops_reset_flag[obu_xlayer_id] : _original != null ? _original.ops_reset_flag[obu_xlayer_id] : this.ops_reset_flag[obu_xlayer_id]);
			stream.WriteFixed(1, this.ops_reset_flag[obu_xlayer_id], "ops_reset_flag"); 
			this.ops_id[obu_xlayer_id] = stream.Pick("ops_id", _original != null ? _original.ops_id[obu_xlayer_id] : this.ops_id[obu_xlayer_id], _edited != null ? _edited.ops_id[obu_xlayer_id] : _original != null ? _original.ops_id[obu_xlayer_id] : this.ops_id[obu_xlayer_id]);
			stream.WriteFixed(4, this.ops_id[obu_xlayer_id], "ops_id"); 
			opsID = ops_id[obu_xlayer_id];
			this.ops_cnt[obu_xlayer_id][opsID] = stream.Pick("ops_cnt", _original != null ? _original.ops_cnt[obu_xlayer_id][opsID] : this.ops_cnt[obu_xlayer_id][opsID], _edited != null ? _edited.ops_cnt[obu_xlayer_id][opsID] : _original != null ? _original.ops_cnt[obu_xlayer_id][opsID] : this.ops_cnt[obu_xlayer_id][opsID]);
			stream.WriteFixed(3, this.ops_cnt[obu_xlayer_id][opsID], "ops_cnt"); 

			if ((ops_cnt[obu_xlayer_id][opsID] > 0))
			{
				this.ops_priority[obu_xlayer_id][opsID] = stream.Pick("ops_priority", _original != null ? _original.ops_priority[obu_xlayer_id][opsID] : this.ops_priority[obu_xlayer_id][opsID], _edited != null ? _edited.ops_priority[obu_xlayer_id][opsID] : _original != null ? _original.ops_priority[obu_xlayer_id][opsID] : this.ops_priority[obu_xlayer_id][opsID]);
				stream.WriteFixed(4, this.ops_priority[obu_xlayer_id][opsID], "ops_priority"); 
				this.ops_intent[obu_xlayer_id][opsID] = stream.Pick("ops_intent", _original != null ? _original.ops_intent[obu_xlayer_id][opsID] : this.ops_intent[obu_xlayer_id][opsID], _edited != null ? _edited.ops_intent[obu_xlayer_id][opsID] : _original != null ? _original.ops_intent[obu_xlayer_id][opsID] : this.ops_intent[obu_xlayer_id][opsID]);
				stream.WriteFixed(7, this.ops_intent[obu_xlayer_id][opsID], "ops_intent"); 
				this.ops_intent_present_flag[obu_xlayer_id][opsID] = stream.Pick("ops_intent_present_flag", _original != null ? _original.ops_intent_present_flag[obu_xlayer_id][opsID] : this.ops_intent_present_flag[obu_xlayer_id][opsID], _edited != null ? _edited.ops_intent_present_flag[obu_xlayer_id][opsID] : _original != null ? _original.ops_intent_present_flag[obu_xlayer_id][opsID] : this.ops_intent_present_flag[obu_xlayer_id][opsID]);
				stream.WriteFixed(1, this.ops_intent_present_flag[obu_xlayer_id][opsID], "ops_intent_present_flag"); 
				this.ops_ptl_present_flag[obu_xlayer_id][opsID] = stream.Pick("ops_ptl_present_flag", _original != null ? _original.ops_ptl_present_flag[obu_xlayer_id][opsID] : this.ops_ptl_present_flag[obu_xlayer_id][opsID], _edited != null ? _edited.ops_ptl_present_flag[obu_xlayer_id][opsID] : _original != null ? _original.ops_ptl_present_flag[obu_xlayer_id][opsID] : this.ops_ptl_present_flag[obu_xlayer_id][opsID]);
				stream.WriteFixed(1, this.ops_ptl_present_flag[obu_xlayer_id][opsID], "ops_ptl_present_flag"); 
				this.ops_color_info_present_flag[obu_xlayer_id][opsID] = stream.Pick("ops_color_info_present_flag", _original != null ? _original.ops_color_info_present_flag[obu_xlayer_id][opsID] : this.ops_color_info_present_flag[obu_xlayer_id][opsID], _edited != null ? _edited.ops_color_info_present_flag[obu_xlayer_id][opsID] : _original != null ? _original.ops_color_info_present_flag[obu_xlayer_id][opsID] : this.ops_color_info_present_flag[obu_xlayer_id][opsID]);
				stream.WriteFixed(1, this.ops_color_info_present_flag[obu_xlayer_id][opsID], "ops_color_info_present_flag"); 

				if ((obu_xlayer_id == GLOBAL_XLAYER_ID))
				{
					this.ops_mlayer_info_idc[opsID] = stream.Pick("ops_mlayer_info_idc", _original != null ? _original.ops_mlayer_info_idc[opsID] : this.ops_mlayer_info_idc[opsID], _edited != null ? _edited.ops_mlayer_info_idc[opsID] : _original != null ? _original.ops_mlayer_info_idc[opsID] : this.ops_mlayer_info_idc[opsID]);
					stream.WriteFixed(2, this.ops_mlayer_info_idc[opsID], "ops_mlayer_info_idc"); 
				}
				else 
				{
					this.ops_reserved_2bits = stream.Pick("ops_reserved_2bits", _original != null ? _original.ops_reserved_2bits : this.ops_reserved_2bits, _edited != null ? _edited.ops_reserved_2bits : _original != null ? _original.ops_reserved_2bits : this.ops_reserved_2bits);
					stream.WriteFixed(2, this.ops_reserved_2bits, "ops_reserved_2bits"); 
				}

				for (i = 0; (i < ops_cnt[obu_xlayer_id][opsID]); i++)
				{
					WriteOperatingPointPayload(obu_xlayer_id, opsID, i); 
				}
			}
        }

    /*
operating_point_payload( xId, opsID, i ) {
ops_data_size[ xId ][ opsID ][ i ]	leb128()
startPos = get_position()	
if ( ops_intent_present_flag[ xId ][ opsID ] ) {	
ops_op_intent[ xId ][ opsID ][ i ]	f(7)
}	
if ( ops_ptl_present_flag[ xId ][ opsID ] ) {	
if ( xId == GLOBAL_XLAYER_ID ) {	
ops_aggregate_info( opsID, i )	
} else {	
ops_seq_profile_tier_level_info( xId, opsID, i, xId )	
}	
}	
if ( ops_color_info_present_flag[ xId ][ opsID ] ) {	
ops_color_info( opsID, i )	
}	
ops_decoder_model_info_for_this_op_present_flag[ xId ][ opsID ][ i ]	f(1)
if ( ops_decoder_model_info_for_this_op_present_flag[ xId ][ opsID ][ i ] ) {	
ops_decoder_model_info( opsID, i )	
}	
ops_initial_display_delay_present_flag[ xId ][ opsID ][ i ]	f(1)
if ( ops_initial_display_delay_present_flag[ xId ][ opsID ][ i ] ) {	
ops_initial_display_delay_minus_1[ xId ][ opsID ][ i ]	f(4)
}	
if ( xId == GLOBAL_XLAYER_ID ) {	
ops_xlayer_map[ opsID ][ i ]	f(31)
k = 0	
for ( j = 0; j < 31; j++ ) {	
if ( ops_xlayer_map[ opsID ][ i ] & (1 << j) ) {	
OpsxLayerId[ xId ][ opsID ][ i ][ k ] = j	
k++	
if ( ops_ptl_present_flag[ xId ][ opsID ] ) {	
ops_seq_profile_tier_level_info( xId, opsID, i, j )	
}	
idc = ops_mlayer_info_idc[ opsID ]	
if ( idc == 1 ) {	
ops_mlayer_info( xId, opsID, i, j )	
} else if ( idc == 2 ) {	
ops_mlayer_explicit_info_flag[ opsID ][ i ][ j ]	f(1)
if ( ops_mlayer_explicit_info_flag[ opsID ][ i ][ j ] ) {	
ops_mlayer_info( xId, opsID, i, j )	
} else {	
ops_embedded_ops_id[ opsID ][ i ][ j ]	f(4)
ops_embedded_op_index[ opsID ][ i ][ j ]	f(3)
}	
}	
}	
}	
XCount[ xId ][ opsID ][ i ] = k	
} else {	
XCount[ xId ][ opsID ][ i ] = 1	
OpsxLayerId[ xId ][ opsID ][ i ][ 0 ] = xId	
ops_mlayer_info( xId, opsID, i, xId )	
}	
byte_alignment()	
opsBytes = (get_position() - startPos) >> 3	
}
    */
		private int opsID;
		public int _OpsID { get { return opsID; } set { opsID = value; } }
		private AomArray<AomArray<AomArray<int>>> ops_data_size = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _OpsDataSize { get { return ops_data_size; } set { ops_data_size = value; } }
		private AomArray<AomArray<AomArray<int>>> ops_op_intent = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _OpsOpIntent { get { return ops_op_intent; } set { ops_op_intent = value; } }
		private AomArray<AomArray<AomArray<int>>> ops_decoder_model_info_for_this_op_present_flag = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _OpsDecoderModelInfoForThisOpPresentFlag { get { return ops_decoder_model_info_for_this_op_present_flag; } set { ops_decoder_model_info_for_this_op_present_flag = value; } }
		private AomArray<AomArray<AomArray<int>>> ops_initial_display_delay_present_flag = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _OpsInitialDisplayDelayPresentFlag { get { return ops_initial_display_delay_present_flag; } set { ops_initial_display_delay_present_flag = value; } }
		private AomArray<AomArray<AomArray<int>>> ops_initial_display_delay_minus_1 = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _OpsInitialDisplayDelayMinus1 { get { return ops_initial_display_delay_minus_1; } set { ops_initial_display_delay_minus_1 = value; } }
		private AomArray<AomArray<int>> ops_xlayer_map = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _OpsXlayerMap { get { return ops_xlayer_map; } set { ops_xlayer_map = value; } }
		private AomArray<AomArray<AomArray<AomArray<int>>>> OpsxLayerId = new AomArray<AomArray<AomArray<AomArray<int>>>>(() => new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>())));
		public AomArray<AomArray<AomArray<AomArray<int>>>> _OpsxLayerId { get { return OpsxLayerId; } set { OpsxLayerId = value; } }
		private AomArray<AomArray<AomArray<int>>> ops_mlayer_explicit_info_flag = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _OpsMlayerExplicitInfoFlag { get { return ops_mlayer_explicit_info_flag; } set { ops_mlayer_explicit_info_flag = value; } }
		private AomArray<AomArray<AomArray<int>>> ops_embedded_ops_id = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _OpsEmbeddedOpsId { get { return ops_embedded_ops_id; } set { ops_embedded_ops_id = value; } }
		private AomArray<AomArray<AomArray<int>>> ops_embedded_op_index = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _OpsEmbeddedOpIndex { get { return ops_embedded_op_index; } set { ops_embedded_op_index = value; } }
		private AomArray<AomArray<AomArray<int>>> XCount = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _XCount { get { return XCount; } set { XCount = value; } }

        private void OperatingPointPayload(int xId, int opsID, int i)
        {
			int j = 0;
			int startPos = 0;
			int k = 0;
			int idc = 0;
			int opsBytes = 0;
			stream.ReadLeb128( out this.ops_data_size[xId][opsID][i], "ops_data_size"); 
			startPos = get_position();

			if ((ops_intent_present_flag[xId][opsID] != 0))
			{
				stream.ReadFixed(7, out this.ops_op_intent[xId][opsID][i], "ops_op_intent"); 
			}

			if ((ops_ptl_present_flag[xId][opsID] != 0))
			{

				if ((xId == GLOBAL_XLAYER_ID))
				{
					OpsAggregateInfo(opsID, i); 
				}
				else 
				{
					OpsSeqProfileTierLevelInfo(xId, opsID, i, xId); 
				}
			}

			if ((ops_color_info_present_flag[xId][opsID] != 0))
			{
				OpsColorInfo(opsID, i); 
			}
			stream.ReadFixed(1, out this.ops_decoder_model_info_for_this_op_present_flag[xId][opsID][i], "ops_decoder_model_info_for_this_op_present_flag"); 

			if ((ops_decoder_model_info_for_this_op_present_flag[xId][opsID][i] != 0))
			{
				OpsDecoderModelInfo(opsID, i); 
			}
			stream.ReadFixed(1, out this.ops_initial_display_delay_present_flag[xId][opsID][i], "ops_initial_display_delay_present_flag"); 

			if ((ops_initial_display_delay_present_flag[xId][opsID][i] != 0))
			{
				stream.ReadFixed(4, out this.ops_initial_display_delay_minus_1[xId][opsID][i], "ops_initial_display_delay_minus_1"); 
			}

			if ((xId == GLOBAL_XLAYER_ID))
			{
				stream.ReadFixed(31, out this.ops_xlayer_map[opsID][i], "ops_xlayer_map"); 
				k = 0;

				for (j = 0; (j < 31); j++)
				{

					if (((ops_xlayer_map[opsID][i] & (1 << j)) != 0))
					{
						OpsxLayerId[xId][opsID][i][k] = j;
						k++;

						if ((ops_ptl_present_flag[xId][opsID] != 0))
						{
							OpsSeqProfileTierLevelInfo(xId, opsID, i, j); 
						}
						idc = ops_mlayer_info_idc[opsID];

						if ((idc == 1))
						{
							OpsMlayerInfo(xId, opsID, i, j); 
						}
						else if ((idc == 2))
						{
							stream.ReadFixed(1, out this.ops_mlayer_explicit_info_flag[opsID][i][j], "ops_mlayer_explicit_info_flag"); 

							if ((ops_mlayer_explicit_info_flag[opsID][i][j] != 0))
							{
								OpsMlayerInfo(xId, opsID, i, j); 
							}
							else 
							{
								stream.ReadFixed(4, out this.ops_embedded_ops_id[opsID][i][j], "ops_embedded_ops_id"); 
								stream.ReadFixed(3, out this.ops_embedded_op_index[opsID][i][j], "ops_embedded_op_index"); 
							}
						}
					}
				}
				XCount[xId][opsID][i] = k;
			}
			else 
			{
				XCount[xId][opsID][i] = 1;
				OpsxLayerId[xId][opsID][i][0] = xId;
				OpsMlayerInfo(xId, opsID, i, xId); 
			}
			ByteAlignment(); 
			opsBytes = ((get_position() - startPos) >> 3);
        }

        private void WriteOperatingPointPayload(int xId, int opsID, int i)
        {
			int j = 0;
			int startPos = 0;
			int k = 0;
			int idc = 0;
			int opsBytes = 0;
			this.ops_data_size[xId][opsID][i] = stream.Pick("ops_data_size", _original != null ? _original.ops_data_size[xId][opsID][i] : this.ops_data_size[xId][opsID][i], _edited != null ? _edited.ops_data_size[xId][opsID][i] : _original != null ? _original.ops_data_size[xId][opsID][i] : this.ops_data_size[xId][opsID][i]);
			stream.WriteLeb128( this.ops_data_size[xId][opsID][i], "ops_data_size"); 
			startPos = get_position();

			if ((ops_intent_present_flag[xId][opsID] != 0))
			{
				this.ops_op_intent[xId][opsID][i] = stream.Pick("ops_op_intent", _original != null ? _original.ops_op_intent[xId][opsID][i] : this.ops_op_intent[xId][opsID][i], _edited != null ? _edited.ops_op_intent[xId][opsID][i] : _original != null ? _original.ops_op_intent[xId][opsID][i] : this.ops_op_intent[xId][opsID][i]);
				stream.WriteFixed(7, this.ops_op_intent[xId][opsID][i], "ops_op_intent"); 
			}

			if ((ops_ptl_present_flag[xId][opsID] != 0))
			{

				if ((xId == GLOBAL_XLAYER_ID))
				{
					WriteOpsAggregateInfo(opsID, i); 
				}
				else 
				{
					WriteOpsSeqProfileTierLevelInfo(xId, opsID, i, xId); 
				}
			}

			if ((ops_color_info_present_flag[xId][opsID] != 0))
			{
				WriteOpsColorInfo(opsID, i); 
			}
			this.ops_decoder_model_info_for_this_op_present_flag[xId][opsID][i] = stream.Pick("ops_decoder_model_info_for_this_op_present_flag", _original != null ? _original.ops_decoder_model_info_for_this_op_present_flag[xId][opsID][i] : this.ops_decoder_model_info_for_this_op_present_flag[xId][opsID][i], _edited != null ? _edited.ops_decoder_model_info_for_this_op_present_flag[xId][opsID][i] : _original != null ? _original.ops_decoder_model_info_for_this_op_present_flag[xId][opsID][i] : this.ops_decoder_model_info_for_this_op_present_flag[xId][opsID][i]);
			stream.WriteFixed(1, this.ops_decoder_model_info_for_this_op_present_flag[xId][opsID][i], "ops_decoder_model_info_for_this_op_present_flag"); 

			if ((ops_decoder_model_info_for_this_op_present_flag[xId][opsID][i] != 0))
			{
				WriteOpsDecoderModelInfo(opsID, i); 
			}
			this.ops_initial_display_delay_present_flag[xId][opsID][i] = stream.Pick("ops_initial_display_delay_present_flag", _original != null ? _original.ops_initial_display_delay_present_flag[xId][opsID][i] : this.ops_initial_display_delay_present_flag[xId][opsID][i], _edited != null ? _edited.ops_initial_display_delay_present_flag[xId][opsID][i] : _original != null ? _original.ops_initial_display_delay_present_flag[xId][opsID][i] : this.ops_initial_display_delay_present_flag[xId][opsID][i]);
			stream.WriteFixed(1, this.ops_initial_display_delay_present_flag[xId][opsID][i], "ops_initial_display_delay_present_flag"); 

			if ((ops_initial_display_delay_present_flag[xId][opsID][i] != 0))
			{
				this.ops_initial_display_delay_minus_1[xId][opsID][i] = stream.Pick("ops_initial_display_delay_minus_1", _original != null ? _original.ops_initial_display_delay_minus_1[xId][opsID][i] : this.ops_initial_display_delay_minus_1[xId][opsID][i], _edited != null ? _edited.ops_initial_display_delay_minus_1[xId][opsID][i] : _original != null ? _original.ops_initial_display_delay_minus_1[xId][opsID][i] : this.ops_initial_display_delay_minus_1[xId][opsID][i]);
				stream.WriteFixed(4, this.ops_initial_display_delay_minus_1[xId][opsID][i], "ops_initial_display_delay_minus_1"); 
			}

			if ((xId == GLOBAL_XLAYER_ID))
			{
				this.ops_xlayer_map[opsID][i] = stream.Pick("ops_xlayer_map", _original != null ? _original.ops_xlayer_map[opsID][i] : this.ops_xlayer_map[opsID][i], _edited != null ? _edited.ops_xlayer_map[opsID][i] : _original != null ? _original.ops_xlayer_map[opsID][i] : this.ops_xlayer_map[opsID][i]);
				stream.WriteFixed(31, this.ops_xlayer_map[opsID][i], "ops_xlayer_map"); 
				k = 0;

				for (j = 0; (j < 31); j++)
				{

					if (((ops_xlayer_map[opsID][i] & (1 << j)) != 0))
					{
						OpsxLayerId[xId][opsID][i][k] = j;
						k++;

						if ((ops_ptl_present_flag[xId][opsID] != 0))
						{
							WriteOpsSeqProfileTierLevelInfo(xId, opsID, i, j); 
						}
						idc = ops_mlayer_info_idc[opsID];

						if ((idc == 1))
						{
							WriteOpsMlayerInfo(xId, opsID, i, j); 
						}
						else if ((idc == 2))
						{
							this.ops_mlayer_explicit_info_flag[opsID][i][j] = stream.Pick("ops_mlayer_explicit_info_flag", _original != null ? _original.ops_mlayer_explicit_info_flag[opsID][i][j] : this.ops_mlayer_explicit_info_flag[opsID][i][j], _edited != null ? _edited.ops_mlayer_explicit_info_flag[opsID][i][j] : _original != null ? _original.ops_mlayer_explicit_info_flag[opsID][i][j] : this.ops_mlayer_explicit_info_flag[opsID][i][j]);
							stream.WriteFixed(1, this.ops_mlayer_explicit_info_flag[opsID][i][j], "ops_mlayer_explicit_info_flag"); 

							if ((ops_mlayer_explicit_info_flag[opsID][i][j] != 0))
							{
								WriteOpsMlayerInfo(xId, opsID, i, j); 
							}
							else 
							{
								this.ops_embedded_ops_id[opsID][i][j] = stream.Pick("ops_embedded_ops_id", _original != null ? _original.ops_embedded_ops_id[opsID][i][j] : this.ops_embedded_ops_id[opsID][i][j], _edited != null ? _edited.ops_embedded_ops_id[opsID][i][j] : _original != null ? _original.ops_embedded_ops_id[opsID][i][j] : this.ops_embedded_ops_id[opsID][i][j]);
								stream.WriteFixed(4, this.ops_embedded_ops_id[opsID][i][j], "ops_embedded_ops_id"); 
								this.ops_embedded_op_index[opsID][i][j] = stream.Pick("ops_embedded_op_index", _original != null ? _original.ops_embedded_op_index[opsID][i][j] : this.ops_embedded_op_index[opsID][i][j], _edited != null ? _edited.ops_embedded_op_index[opsID][i][j] : _original != null ? _original.ops_embedded_op_index[opsID][i][j] : this.ops_embedded_op_index[opsID][i][j]);
								stream.WriteFixed(3, this.ops_embedded_op_index[opsID][i][j], "ops_embedded_op_index"); 
							}
						}
					}
				}
				XCount[xId][opsID][i] = k;
			}
			else 
			{
				XCount[xId][opsID][i] = 1;
				OpsxLayerId[xId][opsID][i][0] = xId;
				WriteOpsMlayerInfo(xId, opsID, i, xId); 
			}
			WriteByteAlignment(); 
			opsBytes = ((get_position() - startPos) >> 3);
        }

    /*
ops_aggregate_info( opsID, i ) {
ops_config_idc[ opsID ][ i ]	f(6)
ops_aggregate_level_idx[ opsID ][ i ]	f(5)
ops_max_tier_flag[ opsID ][ i ]	f(1)
ops_max_interop[ opsID ][ i ]	f(4)
}
    */
		private AomArray<AomArray<int>> ops_config_idc = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _OpsConfigIdc { get { return ops_config_idc; } set { ops_config_idc = value; } }
		private AomArray<AomArray<int>> ops_aggregate_level_idx = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _OpsAggregateLevelIdx { get { return ops_aggregate_level_idx; } set { ops_aggregate_level_idx = value; } }
		private AomArray<AomArray<int>> ops_max_tier_flag = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _OpsMaxTierFlag { get { return ops_max_tier_flag; } set { ops_max_tier_flag = value; } }
		private AomArray<AomArray<int>> ops_max_interop = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _OpsMaxInterop { get { return ops_max_interop; } set { ops_max_interop = value; } }

        private void OpsAggregateInfo(int opsID, int i)
        {
			stream.ReadFixed(6, out this.ops_config_idc[opsID][i], "ops_config_idc"); 
			stream.ReadFixed(5, out this.ops_aggregate_level_idx[opsID][i], "ops_aggregate_level_idx"); 
			stream.ReadFixed(1, out this.ops_max_tier_flag[opsID][i], "ops_max_tier_flag"); 
			stream.ReadFixed(4, out this.ops_max_interop[opsID][i], "ops_max_interop"); 
        }

        private void WriteOpsAggregateInfo(int opsID, int i)
        {
			this.ops_config_idc[opsID][i] = stream.Pick("ops_config_idc", _original != null ? _original.ops_config_idc[opsID][i] : this.ops_config_idc[opsID][i], _edited != null ? _edited.ops_config_idc[opsID][i] : _original != null ? _original.ops_config_idc[opsID][i] : this.ops_config_idc[opsID][i]);
			stream.WriteFixed(6, this.ops_config_idc[opsID][i], "ops_config_idc"); 
			this.ops_aggregate_level_idx[opsID][i] = stream.Pick("ops_aggregate_level_idx", _original != null ? _original.ops_aggregate_level_idx[opsID][i] : this.ops_aggregate_level_idx[opsID][i], _edited != null ? _edited.ops_aggregate_level_idx[opsID][i] : _original != null ? _original.ops_aggregate_level_idx[opsID][i] : this.ops_aggregate_level_idx[opsID][i]);
			stream.WriteFixed(5, this.ops_aggregate_level_idx[opsID][i], "ops_aggregate_level_idx"); 
			this.ops_max_tier_flag[opsID][i] = stream.Pick("ops_max_tier_flag", _original != null ? _original.ops_max_tier_flag[opsID][i] : this.ops_max_tier_flag[opsID][i], _edited != null ? _edited.ops_max_tier_flag[opsID][i] : _original != null ? _original.ops_max_tier_flag[opsID][i] : this.ops_max_tier_flag[opsID][i]);
			stream.WriteFixed(1, this.ops_max_tier_flag[opsID][i], "ops_max_tier_flag"); 
			this.ops_max_interop[opsID][i] = stream.Pick("ops_max_interop", _original != null ? _original.ops_max_interop[opsID][i] : this.ops_max_interop[opsID][i], _edited != null ? _edited.ops_max_interop[opsID][i] : _original != null ? _original.ops_max_interop[opsID][i] : this.ops_max_interop[opsID][i]);
			stream.WriteFixed(4, this.ops_max_interop[opsID][i], "ops_max_interop"); 
        }

    /*
ops_seq_profile_tier_level_info( xId, opsID, i, j ) {
ops_seq_profile_idc[ xId ][ opsID ][ i ][ j ]	f(5)
ops_level_idx[ xId ][ opsID ][ i ][ j ]	f(5)
ops_tier_flag[ xId ][ opsID ][ i ][ j ]	f(1)
ops_mlayer_count[ xId ][ opsID ][ i ][ j ]	f(3)
ops_ptl_reserved_2bits	f(2)
}
    */
		private AomArray<AomArray<AomArray<AomArray<int>>>> ops_seq_profile_idc = new AomArray<AomArray<AomArray<AomArray<int>>>>(() => new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>())));
		public AomArray<AomArray<AomArray<AomArray<int>>>> _OpsSeqProfileIdc { get { return ops_seq_profile_idc; } set { ops_seq_profile_idc = value; } }
		private AomArray<AomArray<AomArray<AomArray<int>>>> ops_level_idx = new AomArray<AomArray<AomArray<AomArray<int>>>>(() => new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>())));
		public AomArray<AomArray<AomArray<AomArray<int>>>> _OpsLevelIdx { get { return ops_level_idx; } set { ops_level_idx = value; } }
		private AomArray<AomArray<AomArray<AomArray<int>>>> ops_tier_flag = new AomArray<AomArray<AomArray<AomArray<int>>>>(() => new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>())));
		public AomArray<AomArray<AomArray<AomArray<int>>>> _OpsTierFlag { get { return ops_tier_flag; } set { ops_tier_flag = value; } }
		private AomArray<AomArray<AomArray<AomArray<int>>>> ops_mlayer_count = new AomArray<AomArray<AomArray<AomArray<int>>>>(() => new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>())));
		public AomArray<AomArray<AomArray<AomArray<int>>>> _OpsMlayerCount { get { return ops_mlayer_count; } set { ops_mlayer_count = value; } }
		private int ops_ptl_reserved_2bits;
		public int _OpsPtlReserved2bits { get { return ops_ptl_reserved_2bits; } set { ops_ptl_reserved_2bits = value; } }

        private void OpsSeqProfileTierLevelInfo(int xId, int opsID, int i, int j)
        {
			stream.ReadFixed(5, out this.ops_seq_profile_idc[xId][opsID][i][j], "ops_seq_profile_idc"); 
			stream.ReadFixed(5, out this.ops_level_idx[xId][opsID][i][j], "ops_level_idx"); 
			stream.ReadFixed(1, out this.ops_tier_flag[xId][opsID][i][j], "ops_tier_flag"); 
			stream.ReadFixed(3, out this.ops_mlayer_count[xId][opsID][i][j], "ops_mlayer_count"); 
			stream.ReadFixed(2, out this.ops_ptl_reserved_2bits, "ops_ptl_reserved_2bits"); 
        }

        private void WriteOpsSeqProfileTierLevelInfo(int xId, int opsID, int i, int j)
        {
			this.ops_seq_profile_idc[xId][opsID][i][j] = stream.Pick("ops_seq_profile_idc", _original != null ? _original.ops_seq_profile_idc[xId][opsID][i][j] : this.ops_seq_profile_idc[xId][opsID][i][j], _edited != null ? _edited.ops_seq_profile_idc[xId][opsID][i][j] : _original != null ? _original.ops_seq_profile_idc[xId][opsID][i][j] : this.ops_seq_profile_idc[xId][opsID][i][j]);
			stream.WriteFixed(5, this.ops_seq_profile_idc[xId][opsID][i][j], "ops_seq_profile_idc"); 
			this.ops_level_idx[xId][opsID][i][j] = stream.Pick("ops_level_idx", _original != null ? _original.ops_level_idx[xId][opsID][i][j] : this.ops_level_idx[xId][opsID][i][j], _edited != null ? _edited.ops_level_idx[xId][opsID][i][j] : _original != null ? _original.ops_level_idx[xId][opsID][i][j] : this.ops_level_idx[xId][opsID][i][j]);
			stream.WriteFixed(5, this.ops_level_idx[xId][opsID][i][j], "ops_level_idx"); 
			this.ops_tier_flag[xId][opsID][i][j] = stream.Pick("ops_tier_flag", _original != null ? _original.ops_tier_flag[xId][opsID][i][j] : this.ops_tier_flag[xId][opsID][i][j], _edited != null ? _edited.ops_tier_flag[xId][opsID][i][j] : _original != null ? _original.ops_tier_flag[xId][opsID][i][j] : this.ops_tier_flag[xId][opsID][i][j]);
			stream.WriteFixed(1, this.ops_tier_flag[xId][opsID][i][j], "ops_tier_flag"); 
			this.ops_mlayer_count[xId][opsID][i][j] = stream.Pick("ops_mlayer_count", _original != null ? _original.ops_mlayer_count[xId][opsID][i][j] : this.ops_mlayer_count[xId][opsID][i][j], _edited != null ? _edited.ops_mlayer_count[xId][opsID][i][j] : _original != null ? _original.ops_mlayer_count[xId][opsID][i][j] : this.ops_mlayer_count[xId][opsID][i][j]);
			stream.WriteFixed(3, this.ops_mlayer_count[xId][opsID][i][j], "ops_mlayer_count"); 
			this.ops_ptl_reserved_2bits = stream.Pick("ops_ptl_reserved_2bits", _original != null ? _original.ops_ptl_reserved_2bits : this.ops_ptl_reserved_2bits, _edited != null ? _edited.ops_ptl_reserved_2bits : _original != null ? _original.ops_ptl_reserved_2bits : this.ops_ptl_reserved_2bits);
			stream.WriteFixed(2, this.ops_ptl_reserved_2bits, "ops_ptl_reserved_2bits"); 
        }

    /*
ops_decoder_model_info( opsID, i ) {
ops_decoder_buffer_delay[ obu_xlayer_id ][ opsID ][ i ]	uvlc()
ops_encoder_buffer_delay[ obu_xlayer_id ][ opsID ][ i ]	uvlc()
ops_low_delay_mode_flag[ obu_xlayer_id ][ opsID ][ i ]	f(1)
}
    */
		private AomArray<AomArray<AomArray<int>>> ops_decoder_buffer_delay = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _OpsDecoderBufferDelay { get { return ops_decoder_buffer_delay; } set { ops_decoder_buffer_delay = value; } }
		private AomArray<AomArray<AomArray<int>>> ops_encoder_buffer_delay = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _OpsEncoderBufferDelay { get { return ops_encoder_buffer_delay; } set { ops_encoder_buffer_delay = value; } }
		private AomArray<AomArray<AomArray<int>>> ops_low_delay_mode_flag = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _OpsLowDelayModeFlag { get { return ops_low_delay_mode_flag; } set { ops_low_delay_mode_flag = value; } }

        private void OpsDecoderModelInfo(int opsID, int i)
        {
			stream.ReadUvlc( out this.ops_decoder_buffer_delay[obu_xlayer_id][opsID][i], "ops_decoder_buffer_delay"); 
			stream.ReadUvlc( out this.ops_encoder_buffer_delay[obu_xlayer_id][opsID][i], "ops_encoder_buffer_delay"); 
			stream.ReadFixed(1, out this.ops_low_delay_mode_flag[obu_xlayer_id][opsID][i], "ops_low_delay_mode_flag"); 
        }

        private void WriteOpsDecoderModelInfo(int opsID, int i)
        {
			this.ops_decoder_buffer_delay[obu_xlayer_id][opsID][i] = stream.Pick("ops_decoder_buffer_delay", _original != null ? _original.ops_decoder_buffer_delay[obu_xlayer_id][opsID][i] : this.ops_decoder_buffer_delay[obu_xlayer_id][opsID][i], _edited != null ? _edited.ops_decoder_buffer_delay[obu_xlayer_id][opsID][i] : _original != null ? _original.ops_decoder_buffer_delay[obu_xlayer_id][opsID][i] : this.ops_decoder_buffer_delay[obu_xlayer_id][opsID][i]);
			stream.WriteUvlc( this.ops_decoder_buffer_delay[obu_xlayer_id][opsID][i], "ops_decoder_buffer_delay"); 
			this.ops_encoder_buffer_delay[obu_xlayer_id][opsID][i] = stream.Pick("ops_encoder_buffer_delay", _original != null ? _original.ops_encoder_buffer_delay[obu_xlayer_id][opsID][i] : this.ops_encoder_buffer_delay[obu_xlayer_id][opsID][i], _edited != null ? _edited.ops_encoder_buffer_delay[obu_xlayer_id][opsID][i] : _original != null ? _original.ops_encoder_buffer_delay[obu_xlayer_id][opsID][i] : this.ops_encoder_buffer_delay[obu_xlayer_id][opsID][i]);
			stream.WriteUvlc( this.ops_encoder_buffer_delay[obu_xlayer_id][opsID][i], "ops_encoder_buffer_delay"); 
			this.ops_low_delay_mode_flag[obu_xlayer_id][opsID][i] = stream.Pick("ops_low_delay_mode_flag", _original != null ? _original.ops_low_delay_mode_flag[obu_xlayer_id][opsID][i] : this.ops_low_delay_mode_flag[obu_xlayer_id][opsID][i], _edited != null ? _edited.ops_low_delay_mode_flag[obu_xlayer_id][opsID][i] : _original != null ? _original.ops_low_delay_mode_flag[obu_xlayer_id][opsID][i] : this.ops_low_delay_mode_flag[obu_xlayer_id][opsID][i]);
			stream.WriteFixed(1, this.ops_low_delay_mode_flag[obu_xlayer_id][opsID][i], "ops_low_delay_mode_flag"); 
        }

    /*
ops_color_info( opsID, i ) {
ops_color_description_idc[ obu_xlayer_id ][ opsID ][ i ]	rg(2)
if ( ops_color_description_idc[ obu_xlayer_id ][ opsID ][ i ] == 0 ) {	
ops_color_primaries[ obu_xlayer_id ][ opsID ][ i ]	f(8)
ops_transfer_characteristics[ obu_xlayer_id ][ opsID ][ i ]	f(8)
ops_matrix_coefficients[ obu_xlayer_id ][ opsID ][ i ]	f(8)
}	
ops_full_range_flag[ obu_xlayer_id ][ opsID ][ i ]	f(1)
}
    */
		private AomArray<AomArray<AomArray<int>>> ops_color_description_idc = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _OpsColorDescriptionIdc { get { return ops_color_description_idc; } set { ops_color_description_idc = value; } }
		private AomArray<AomArray<AomArray<int>>> ops_color_primaries = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _OpsColorPrimaries { get { return ops_color_primaries; } set { ops_color_primaries = value; } }
		private AomArray<AomArray<AomArray<int>>> ops_transfer_characteristics = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _OpsTransferCharacteristics { get { return ops_transfer_characteristics; } set { ops_transfer_characteristics = value; } }
		private AomArray<AomArray<AomArray<int>>> ops_matrix_coefficients = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _OpsMatrixCoefficients { get { return ops_matrix_coefficients; } set { ops_matrix_coefficients = value; } }
		private AomArray<AomArray<AomArray<int>>> ops_full_range_flag = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _OpsFullRangeFlag { get { return ops_full_range_flag; } set { ops_full_range_flag = value; } }

        private void OpsColorInfo(int opsID, int i)
        {
			stream.ReadRg(2, out this.ops_color_description_idc[obu_xlayer_id][opsID][i], "ops_color_description_idc"); 

			if ((ops_color_description_idc[obu_xlayer_id][opsID][i] == 0))
			{
				stream.ReadFixed(8, out this.ops_color_primaries[obu_xlayer_id][opsID][i], "ops_color_primaries"); 
				stream.ReadFixed(8, out this.ops_transfer_characteristics[obu_xlayer_id][opsID][i], "ops_transfer_characteristics"); 
				stream.ReadFixed(8, out this.ops_matrix_coefficients[obu_xlayer_id][opsID][i], "ops_matrix_coefficients"); 
			}
			stream.ReadFixed(1, out this.ops_full_range_flag[obu_xlayer_id][opsID][i], "ops_full_range_flag"); 
        }

        private void WriteOpsColorInfo(int opsID, int i)
        {
			this.ops_color_description_idc[obu_xlayer_id][opsID][i] = stream.Pick("ops_color_description_idc", _original != null ? _original.ops_color_description_idc[obu_xlayer_id][opsID][i] : this.ops_color_description_idc[obu_xlayer_id][opsID][i], _edited != null ? _edited.ops_color_description_idc[obu_xlayer_id][opsID][i] : _original != null ? _original.ops_color_description_idc[obu_xlayer_id][opsID][i] : this.ops_color_description_idc[obu_xlayer_id][opsID][i]);
			stream.WriteRg(2, this.ops_color_description_idc[obu_xlayer_id][opsID][i], "ops_color_description_idc"); 

			if ((ops_color_description_idc[obu_xlayer_id][opsID][i] == 0))
			{
				this.ops_color_primaries[obu_xlayer_id][opsID][i] = stream.Pick("ops_color_primaries", _original != null ? _original.ops_color_primaries[obu_xlayer_id][opsID][i] : this.ops_color_primaries[obu_xlayer_id][opsID][i], _edited != null ? _edited.ops_color_primaries[obu_xlayer_id][opsID][i] : _original != null ? _original.ops_color_primaries[obu_xlayer_id][opsID][i] : this.ops_color_primaries[obu_xlayer_id][opsID][i]);
				stream.WriteFixed(8, this.ops_color_primaries[obu_xlayer_id][opsID][i], "ops_color_primaries"); 
				this.ops_transfer_characteristics[obu_xlayer_id][opsID][i] = stream.Pick("ops_transfer_characteristics", _original != null ? _original.ops_transfer_characteristics[obu_xlayer_id][opsID][i] : this.ops_transfer_characteristics[obu_xlayer_id][opsID][i], _edited != null ? _edited.ops_transfer_characteristics[obu_xlayer_id][opsID][i] : _original != null ? _original.ops_transfer_characteristics[obu_xlayer_id][opsID][i] : this.ops_transfer_characteristics[obu_xlayer_id][opsID][i]);
				stream.WriteFixed(8, this.ops_transfer_characteristics[obu_xlayer_id][opsID][i], "ops_transfer_characteristics"); 
				this.ops_matrix_coefficients[obu_xlayer_id][opsID][i] = stream.Pick("ops_matrix_coefficients", _original != null ? _original.ops_matrix_coefficients[obu_xlayer_id][opsID][i] : this.ops_matrix_coefficients[obu_xlayer_id][opsID][i], _edited != null ? _edited.ops_matrix_coefficients[obu_xlayer_id][opsID][i] : _original != null ? _original.ops_matrix_coefficients[obu_xlayer_id][opsID][i] : this.ops_matrix_coefficients[obu_xlayer_id][opsID][i]);
				stream.WriteFixed(8, this.ops_matrix_coefficients[obu_xlayer_id][opsID][i], "ops_matrix_coefficients"); 
			}
			this.ops_full_range_flag[obu_xlayer_id][opsID][i] = stream.Pick("ops_full_range_flag", _original != null ? _original.ops_full_range_flag[obu_xlayer_id][opsID][i] : this.ops_full_range_flag[obu_xlayer_id][opsID][i], _edited != null ? _edited.ops_full_range_flag[obu_xlayer_id][opsID][i] : _original != null ? _original.ops_full_range_flag[obu_xlayer_id][opsID][i] : this.ops_full_range_flag[obu_xlayer_id][opsID][i]);
			stream.WriteFixed(1, this.ops_full_range_flag[obu_xlayer_id][opsID][i], "ops_full_range_flag"); 
        }

    /*
ops_mlayer_info( obuXLId, opsID, opIndex, xLId ) {
ops_mlayer_map[ obuXLId ][ opsID ][ opIndex ][ xLId ]	f(8)
mCount = 0	
for ( j = 0; j < 8; j++ ) {	
if (ops_mlayer_map[ obuXLId ][ opsID ][ opIndex ][ xLId ] & (1 << j)) {	
ops_tlayer_map[ obuXLId ][ opsID ][ opIndex ][ xLId ][ j ]	f(4)
tCount = 0	
for ( k = 0; k < 4; k++ ) {	
if ( ops_tlayer_map[ obuXLId ][ opsID ][ opIndex ][ xLId ][ j ]	
& (1 << k) ) {	
tCount++	
}	
}	
mCount++	
}	
}	
}
    */
		private int obuXLId;
		public int _ObuXLId { get { return obuXLId; } set { obuXLId = value; } }
		private int opIndex;
		public int _OpIndex { get { return opIndex; } set { opIndex = value; } }
		private int xLId;
		public int _XLId { get { return xLId; } set { xLId = value; } }
		private AomArray<AomArray<AomArray<AomArray<int>>>> ops_mlayer_map = new AomArray<AomArray<AomArray<AomArray<int>>>>(() => new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>())));
		public AomArray<AomArray<AomArray<AomArray<int>>>> _OpsMlayerMap { get { return ops_mlayer_map; } set { ops_mlayer_map = value; } }
		private AomArray<AomArray<AomArray<AomArray<AomArray<int>>>>> ops_tlayer_map = new AomArray<AomArray<AomArray<AomArray<AomArray<int>>>>>(() => new AomArray<AomArray<AomArray<AomArray<int>>>>(() => new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()))));
		public AomArray<AomArray<AomArray<AomArray<AomArray<int>>>>> _OpsTlayerMap { get { return ops_tlayer_map; } set { ops_tlayer_map = value; } }
		private int k = 0;

        private void OpsMlayerInfo(int obuXLId, int opsID, int opIndex, int xLId)
        {
			int j = 0;
			int k = 0;
			int mCount = 0;
			int tCount = 0;
			stream.ReadFixed(8, out this.ops_mlayer_map[obuXLId][opsID][opIndex][xLId], "ops_mlayer_map"); 
			mCount = 0;

			for (j = 0; (j < 8); j++)
			{

				if (((ops_mlayer_map[obuXLId][opsID][opIndex][xLId] & (1 << j)) != 0))
				{
					stream.ReadFixed(4, out this.ops_tlayer_map[obuXLId][opsID][opIndex][xLId][j], "ops_tlayer_map"); 
					tCount = 0;

					for (k = 0; (k < 4); k++)
					{

						if (((ops_tlayer_map[obuXLId][opsID][opIndex][xLId][j] & (1 << k)) != 0))
						{
							tCount++;
						}
					}
					mCount++;
				}
			}
        }

        private void WriteOpsMlayerInfo(int obuXLId, int opsID, int opIndex, int xLId)
        {
			int j = 0;
			int k = 0;
			int mCount = 0;
			int tCount = 0;
			this.ops_mlayer_map[obuXLId][opsID][opIndex][xLId] = stream.Pick("ops_mlayer_map", _original != null ? _original.ops_mlayer_map[obuXLId][opsID][opIndex][xLId] : this.ops_mlayer_map[obuXLId][opsID][opIndex][xLId], _edited != null ? _edited.ops_mlayer_map[obuXLId][opsID][opIndex][xLId] : _original != null ? _original.ops_mlayer_map[obuXLId][opsID][opIndex][xLId] : this.ops_mlayer_map[obuXLId][opsID][opIndex][xLId]);
			stream.WriteFixed(8, this.ops_mlayer_map[obuXLId][opsID][opIndex][xLId], "ops_mlayer_map"); 
			mCount = 0;

			for (j = 0; (j < 8); j++)
			{

				if (((ops_mlayer_map[obuXLId][opsID][opIndex][xLId] & (1 << j)) != 0))
				{
					this.ops_tlayer_map[obuXLId][opsID][opIndex][xLId][j] = stream.Pick("ops_tlayer_map", _original != null ? _original.ops_tlayer_map[obuXLId][opsID][opIndex][xLId][j] : this.ops_tlayer_map[obuXLId][opsID][opIndex][xLId][j], _edited != null ? _edited.ops_tlayer_map[obuXLId][opsID][opIndex][xLId][j] : _original != null ? _original.ops_tlayer_map[obuXLId][opsID][opIndex][xLId][j] : this.ops_tlayer_map[obuXLId][opsID][opIndex][xLId][j]);
					stream.WriteFixed(4, this.ops_tlayer_map[obuXLId][opsID][opIndex][xLId][j], "ops_tlayer_map"); 
					tCount = 0;

					for (k = 0; (k < 4); k++)
					{

						if (((ops_tlayer_map[obuXLId][opsID][opIndex][xLId][j] & (1 << k)) != 0))
						{
							tCount++;
						}
					}
					mCount++;
				}
			}
        }

    /*
buffer_removal_timing_obu() {
br_ops_dependent_flag	f(1)
if ( br_ops_dependent_flag ) {	
br_ops_id	f(4)
br_ops_cnt[ br_ops_id ]	f(3)
for ( i = 0; i < br_ops_cnt[ br_ops_id ]; i++ ) {	
br_decoder_model_present_op_flag[ br_ops_id ][ i ]	f(1)
if ( br_decoder_model_present_op_flag[ br_ops_id ][ i ] ) {	
br_time_op[ br_ops_id ][ i ]	rg(4)
}	
}	
} else {	
br_time	rg(4)
}	
}
    */
		private int br_ops_dependent_flag;
		public int _BrOpsDependentFlag { get { return br_ops_dependent_flag; } set { br_ops_dependent_flag = value; } }
		private int br_ops_id;
		public int _BrOpsId { get { return br_ops_id; } set { br_ops_id = value; } }
		private AomArray<int> br_ops_cnt = new AomArray<int>();
		public AomArray<int> _BrOpsCnt { get { return br_ops_cnt; } set { br_ops_cnt = value; } }
		private AomArray<AomArray<int>> br_decoder_model_present_op_flag = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _BrDecoderModelPresentOpFlag { get { return br_decoder_model_present_op_flag; } set { br_decoder_model_present_op_flag = value; } }
		private AomArray<AomArray<int>> br_time_op = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _BrTimeOp { get { return br_time_op; } set { br_time_op = value; } }
		private int br_time;
		public int _BrTime { get { return br_time; } set { br_time = value; } }

        private void BufferRemovalTimingObu()
        {
			int i = 0;
			stream.ReadFixed(1, out this.br_ops_dependent_flag, "br_ops_dependent_flag"); 

			if ((br_ops_dependent_flag != 0))
			{
				stream.ReadFixed(4, out this.br_ops_id, "br_ops_id"); 
				stream.ReadFixed(3, out this.br_ops_cnt[br_ops_id], "br_ops_cnt"); 

				for (i = 0; (i < br_ops_cnt[br_ops_id]); i++)
				{
					stream.ReadFixed(1, out this.br_decoder_model_present_op_flag[br_ops_id][i], "br_decoder_model_present_op_flag"); 

					if ((br_decoder_model_present_op_flag[br_ops_id][i] != 0))
					{
						stream.ReadRg(4, out this.br_time_op[br_ops_id][i], "br_time_op"); 
					}
				}
			}
			else 
			{
				stream.ReadRg(4, out this.br_time, "br_time"); 
			}
        }

        private void WriteBufferRemovalTimingObu()
        {
			int i = 0;
			this.br_ops_dependent_flag = stream.Pick("br_ops_dependent_flag", _original != null ? _original.br_ops_dependent_flag : this.br_ops_dependent_flag, _edited != null ? _edited.br_ops_dependent_flag : _original != null ? _original.br_ops_dependent_flag : this.br_ops_dependent_flag);
			stream.WriteFixed(1, this.br_ops_dependent_flag, "br_ops_dependent_flag"); 

			if ((br_ops_dependent_flag != 0))
			{
				this.br_ops_id = stream.Pick("br_ops_id", _original != null ? _original.br_ops_id : this.br_ops_id, _edited != null ? _edited.br_ops_id : _original != null ? _original.br_ops_id : this.br_ops_id);
				stream.WriteFixed(4, this.br_ops_id, "br_ops_id"); 
				this.br_ops_cnt[br_ops_id] = stream.Pick("br_ops_cnt", _original != null ? _original.br_ops_cnt[br_ops_id] : this.br_ops_cnt[br_ops_id], _edited != null ? _edited.br_ops_cnt[br_ops_id] : _original != null ? _original.br_ops_cnt[br_ops_id] : this.br_ops_cnt[br_ops_id]);
				stream.WriteFixed(3, this.br_ops_cnt[br_ops_id], "br_ops_cnt"); 

				for (i = 0; (i < br_ops_cnt[br_ops_id]); i++)
				{
					this.br_decoder_model_present_op_flag[br_ops_id][i] = stream.Pick("br_decoder_model_present_op_flag", _original != null ? _original.br_decoder_model_present_op_flag[br_ops_id][i] : this.br_decoder_model_present_op_flag[br_ops_id][i], _edited != null ? _edited.br_decoder_model_present_op_flag[br_ops_id][i] : _original != null ? _original.br_decoder_model_present_op_flag[br_ops_id][i] : this.br_decoder_model_present_op_flag[br_ops_id][i]);
					stream.WriteFixed(1, this.br_decoder_model_present_op_flag[br_ops_id][i], "br_decoder_model_present_op_flag"); 

					if ((br_decoder_model_present_op_flag[br_ops_id][i] != 0))
					{
						this.br_time_op[br_ops_id][i] = stream.Pick("br_time_op", _original != null ? _original.br_time_op[br_ops_id][i] : this.br_time_op[br_ops_id][i], _edited != null ? _edited.br_time_op[br_ops_id][i] : _original != null ? _original.br_time_op[br_ops_id][i] : this.br_time_op[br_ops_id][i]);
						stream.WriteRg(4, this.br_time_op[br_ops_id][i], "br_time_op"); 
					}
				}
			}
			else 
			{
				this.br_time = stream.Pick("br_time", _original != null ? _original.br_time : this.br_time, _edited != null ? _edited.br_time : _original != null ? _original.br_time : this.br_time);
				stream.WriteRg(4, this.br_time, "br_time"); 
			}
        }

    /*
quantizer_matrix_obu() {
qm_bit_map	f(15)
qm_chroma_info_present_flag	f(1)
numPlanes = qm_chroma_info_present_flag ? 3 : 1	
if ( qm_bit_map == 0 ){	
for ( level = 0; level < NUM_CUSTOM_QMS; level++ ) {	
QmProtected[ level ] = 1	
QmNumPlanes[ level ] = numPlanes	
QmDataPresent[ level ] = 0	
QmMLayerId[ level ] = -1	
QmTLayerId[ level ] = -1	
}	
} else {	
for ( level = 0; level < 15; level++ ) {	
if ( qm_bit_map & (1 << level) ) {	
QmSeen[ level ] = 1	
QmProtected[ level ] = 1	
QmNumPlanes[ level ] = numPlanes	
QmMLayerId[ level ] = obu_mlayer_id	
QmTLayerId[ level ] = obu_tlayer_id	
QmDataPresent[ level ] = 1	
qm_is_default_flag	f(1)
if ( qm_is_default_flag ) {	
QmDataPresent[ level ] = 0	
} else {	
for ( t = 0; t < 3; t++ ){	
for ( plane = 0; plane < numPlanes; plane++ ) {	
user_defined_qm( level, t, plane )	
}	
}	
}	
}	
}	
}	
}
    */
		private int qm_bit_map;
		public int _QmBitMap { get { return qm_bit_map; } set { qm_bit_map = value; } }
		private int qm_chroma_info_present_flag;
		public int _QmChromaInfoPresentFlag { get { return qm_chroma_info_present_flag; } set { qm_chroma_info_present_flag = value; } }
		private AomArray<int> QmNumPlanes = new AomArray<int>();
		public AomArray<int> _QmNumPlanes { get { return QmNumPlanes; } set { QmNumPlanes = value; } }
		private AomArray<int> QmDataPresent = new AomArray<int>();
		public AomArray<int> _QmDataPresent { get { return QmDataPresent; } set { QmDataPresent = value; } }
		private AomArray<int> QmMLayerId = new AomArray<int>();
		public AomArray<int> _QmMLayerId { get { return QmMLayerId; } set { QmMLayerId = value; } }
		private AomArray<int> QmTLayerId = new AomArray<int>();
		public AomArray<int> _QmTLayerId { get { return QmTLayerId; } set { QmTLayerId = value; } }
		private AomArray<int> QmSeen = new AomArray<int>();
		public AomArray<int> _QmSeen { get { return QmSeen; } set { QmSeen = value; } }
		private int qm_is_default_flag;
		public int _QmIsDefaultFlag { get { return qm_is_default_flag; } set { qm_is_default_flag = value; } }

        private void QuantizerMatrixObu()
        {
			int level = 0;
			int t = 0;
			int plane = 0;
			int numPlanes = 0;
			stream.ReadFixed(15, out this.qm_bit_map, "qm_bit_map"); 
			stream.ReadFixed(1, out this.qm_chroma_info_present_flag, "qm_chroma_info_present_flag"); 
			numPlanes = ((qm_chroma_info_present_flag != 0) ? 3 : 1);

			if ((qm_bit_map == 0))
			{

				for (level = 0; (level < NUM_CUSTOM_QMS); level++)
				{
					QmProtected[level] = 1;
					QmNumPlanes[level] = numPlanes;
					QmDataPresent[level] = 0;
					QmMLayerId[level] = -1;
					QmTLayerId[level] = -1;
				}
			}
			else 
			{

				for (level = 0; (level < 15); level++)
				{

					if (((qm_bit_map & (1 << level)) != 0))
					{
						QmSeen[level] = 1;
						QmProtected[level] = 1;
						QmNumPlanes[level] = numPlanes;
						QmMLayerId[level] = obu_mlayer_id;
						QmTLayerId[level] = obu_tlayer_id;
						QmDataPresent[level] = 1;
						stream.ReadFixed(1, out this.qm_is_default_flag, "qm_is_default_flag"); 

						if ((qm_is_default_flag != 0))
						{
							QmDataPresent[level] = 0;
						}
						else 
						{

							for (t = 0; (t < 3); t++)
							{

								for (plane = 0; (plane < numPlanes); plane++)
								{
									UserDefinedQm(level, t, plane); 
								}
							}
						}
					}
				}
			}
        }

        private void WriteQuantizerMatrixObu()
        {
			int level = 0;
			int t = 0;
			int plane = 0;
			int numPlanes = 0;
			this.qm_bit_map = stream.Pick("qm_bit_map", _original != null ? _original.qm_bit_map : this.qm_bit_map, _edited != null ? _edited.qm_bit_map : _original != null ? _original.qm_bit_map : this.qm_bit_map);
			stream.WriteFixed(15, this.qm_bit_map, "qm_bit_map"); 
			this.qm_chroma_info_present_flag = stream.Pick("qm_chroma_info_present_flag", _original != null ? _original.qm_chroma_info_present_flag : this.qm_chroma_info_present_flag, _edited != null ? _edited.qm_chroma_info_present_flag : _original != null ? _original.qm_chroma_info_present_flag : this.qm_chroma_info_present_flag);
			stream.WriteFixed(1, this.qm_chroma_info_present_flag, "qm_chroma_info_present_flag"); 
			numPlanes = ((qm_chroma_info_present_flag != 0) ? 3 : 1);

			if ((qm_bit_map == 0))
			{

				for (level = 0; (level < NUM_CUSTOM_QMS); level++)
				{
					QmProtected[level] = 1;
					QmNumPlanes[level] = numPlanes;
					QmDataPresent[level] = 0;
					QmMLayerId[level] = -1;
					QmTLayerId[level] = -1;
				}
			}
			else 
			{

				for (level = 0; (level < 15); level++)
				{

					if (((qm_bit_map & (1 << level)) != 0))
					{
						QmSeen[level] = 1;
						QmProtected[level] = 1;
						QmNumPlanes[level] = numPlanes;
						QmMLayerId[level] = obu_mlayer_id;
						QmTLayerId[level] = obu_tlayer_id;
						QmDataPresent[level] = 1;
						this.qm_is_default_flag = stream.Pick("qm_is_default_flag", _original != null ? _original.qm_is_default_flag : this.qm_is_default_flag, _edited != null ? _edited.qm_is_default_flag : _original != null ? _original.qm_is_default_flag : this.qm_is_default_flag);
						stream.WriteFixed(1, this.qm_is_default_flag, "qm_is_default_flag"); 

						if ((qm_is_default_flag != 0))
						{
							QmDataPresent[level] = 0;
						}
						else 
						{

							for (t = 0; (t < 3); t++)
							{

								for (plane = 0; (plane < numPlanes); plane++)
								{
									WriteUserDefinedQm(level, t, plane); 
								}
							}
						}
					}
				}
			}
        }

    /*
film_grain_obu() {
fgm_update_flags	f(8)
fgm_chroma_idc	uvlc()
if ( fgm_chroma_idc == CHROMA_FORMAT_420 ) {	
subX = 1	
subY = 1	
} else if ( fgm_chroma_idc == CHROMA_FORMAT_444 ) {	
subX = 0	
subY = 0	
} else if ( fgm_chroma_idc == CHROMA_FORMAT_422 ) {	
subX = 1	
subY = 0	
} else if ( fgm_chroma_idc == CHROMA_FORMAT_400 ) {	
subX = 1	
subY = 1	
}	
monochrome = fgm_chroma_idc == CHROMA_FORMAT_400	
for ( i = 0; i < MAX_FILM_GRAIN; i++ ) {	
if ( fgm_update_flags & (1 << i) ) {	
FilmGrainPresent[ i ] = 1	
film_grain_model( monochrome, subX, subY)	
save_grain_model( i )	
FgmTLayerId[ i ] = obu_tlayer_id	
FgmMLayerId[ i ] = obu_mlayer_id	
FgmChromaIdc[ i ] = fgm_chroma_idc	
}	
}	
}
    */
		private int fgm_update_flags;
		public int _FgmUpdateFlags { get { return fgm_update_flags; } set { fgm_update_flags = value; } }
		private int fgm_chroma_idc;
		public int _FgmChromaIdc { get { return fgm_chroma_idc; } set { fgm_chroma_idc = value; } }
		private AomArray<int> FilmGrainPresent = new AomArray<int>();
		public AomArray<int> _FilmGrainPresent { get { return FilmGrainPresent; } set { FilmGrainPresent = value; } }
		private AomArray<int> FgmTLayerId = new AomArray<int>();
		public AomArray<int> _FgmTLayerId { get { return FgmTLayerId; } set { FgmTLayerId = value; } }
		private AomArray<int> FgmMLayerId = new AomArray<int>();
		public AomArray<int> _FgmMLayerId { get { return FgmMLayerId; } set { FgmMLayerId = value; } }
		private AomArray<int> FgmChromaIdc = new AomArray<int>();
		public AomArray<int> __FgmChromaIdc { get { return FgmChromaIdc; } set { FgmChromaIdc = value; } }

        private void FilmGrainObu()
        {
			int i = 0;
			int subX = 0;
			int subY = 0;
			int monochrome = 0;
			stream.ReadFixed(8, out this.fgm_update_flags, "fgm_update_flags"); 
			stream.ReadUvlc( out this.fgm_chroma_idc, "fgm_chroma_idc"); 

			if ((fgm_chroma_idc == CHROMA_FORMAT_420))
			{
				subX = 1;
				subY = 1;
			}
			else if ((fgm_chroma_idc == CHROMA_FORMAT_444))
			{
				subX = 0;
				subY = 0;
			}
			else if ((fgm_chroma_idc == CHROMA_FORMAT_422))
			{
				subX = 1;
				subY = 0;
			}
			else if ((fgm_chroma_idc == CHROMA_FORMAT_400))
			{
				subX = 1;
				subY = 1;
			}
			monochrome = ((fgm_chroma_idc == CHROMA_FORMAT_400) ? 1 : 0);

			for (i = 0; (i < MAX_FILM_GRAIN); i++)
			{

				if (((fgm_update_flags & (1 << i)) != 0))
				{
					FilmGrainPresent[i] = 1;
					FilmGrainModel(monochrome, subX, subY); 
					save_grain_model(i); 
					FgmTLayerId[i] = obu_tlayer_id;
					FgmMLayerId[i] = obu_mlayer_id;
					FgmChromaIdc[i] = fgm_chroma_idc;
				}
			}
        }

        private void WriteFilmGrainObu()
        {
			int i = 0;
			int subX = 0;
			int subY = 0;
			int monochrome = 0;
			this.fgm_update_flags = stream.Pick("fgm_update_flags", _original != null ? _original.fgm_update_flags : this.fgm_update_flags, _edited != null ? _edited.fgm_update_flags : _original != null ? _original.fgm_update_flags : this.fgm_update_flags);
			stream.WriteFixed(8, this.fgm_update_flags, "fgm_update_flags"); 
			this.fgm_chroma_idc = stream.Pick("fgm_chroma_idc", _original != null ? _original.fgm_chroma_idc : this.fgm_chroma_idc, _edited != null ? _edited.fgm_chroma_idc : _original != null ? _original.fgm_chroma_idc : this.fgm_chroma_idc);
			stream.WriteUvlc( this.fgm_chroma_idc, "fgm_chroma_idc"); 

			if ((fgm_chroma_idc == CHROMA_FORMAT_420))
			{
				subX = 1;
				subY = 1;
			}
			else if ((fgm_chroma_idc == CHROMA_FORMAT_444))
			{
				subX = 0;
				subY = 0;
			}
			else if ((fgm_chroma_idc == CHROMA_FORMAT_422))
			{
				subX = 1;
				subY = 0;
			}
			else if ((fgm_chroma_idc == CHROMA_FORMAT_400))
			{
				subX = 1;
				subY = 1;
			}
			monochrome = ((fgm_chroma_idc == CHROMA_FORMAT_400) ? 1 : 0);

			for (i = 0; (i < MAX_FILM_GRAIN); i++)
			{

				if (((fgm_update_flags & (1 << i)) != 0))
				{
					FilmGrainPresent[i] = 1;
					WriteFilmGrainModel(monochrome, subX, subY); 
					save_grain_model(i); 
					FgmTLayerId[i] = obu_tlayer_id;
					FgmMLayerId[i] = obu_mlayer_id;
					FgmChromaIdc[i] = fgm_chroma_idc;
				}
			}
        }

    /*
content_interpretation_obu() {
ci_scan_type_idc	f(2)
ci_color_description_present_flag	f(1)
ci_chroma_sample_position_present_flag	f(1)
ci_aspect_ratio_info_present_flag	f(1)
ci_timing_info_present_flag	f(1)
ci_reserved_2bit	f(2)
ci_color_primaries = CP_UNSPECIFIED	
ci_transfer_characteristics = TC_UNSPECIFIED	
ci_matrix_coefficients = MC_UNSPECIFIED	
ci_full_range_flag = 0	
if ( ci_color_description_present_flag ) {	
ci_color_description_idc	rg(2)
if ( ci_color_description_idc == 0 ) {	
ci_color_primaries	f(8)
ci_transfer_characteristics	f(8)
ci_matrix_coefficients	f(8)
}	
ci_full_range_flag	f(1)
}	
if ( ci_chroma_sample_position_present_flag ) {	
ci_chroma_sample_position_top	uvlc()
if ( ci_scan_type_idc != 1 ) {	
ci_chroma_sample_position_bottom	uvlc()
} else {	
ci_chroma_sample_position_bottom = ci_chroma_sample_position_top	
}	
} else {	
ci_chroma_sample_position_top = CSP_UNSPECIFIED	
ci_chroma_sample_position_bottom = CSP_UNSPECIFIED	
}	
if ( ci_aspect_ratio_info_present_flag ) {	
ci_aspect_ratio_idc	f(8)
if ( ci_aspect_ratio_idc == 255 ) {	
ci_sar_width	uvlc()
ci_sar_height	uvlc()
} else {	
ci_sar_width = Aspect_Ratio_Width[ ci_aspect_ratio_idc ]	
ci_sar_height = Aspect_Ratio_Height[ ci_aspect_ratio_idc ]	
}	
}	
if ( ci_timing_info_present_flag ) {	
timing_info()	
}	
}
    */
		private int ci_scan_type_idc;
		public int _CiScanTypeIdc { get { return ci_scan_type_idc; } set { ci_scan_type_idc = value; } }
		private int ci_color_description_present_flag;
		public int _CiColorDescriptionPresentFlag { get { return ci_color_description_present_flag; } set { ci_color_description_present_flag = value; } }
		private int ci_chroma_sample_position_present_flag;
		public int _CiChromaSamplePositionPresentFlag { get { return ci_chroma_sample_position_present_flag; } set { ci_chroma_sample_position_present_flag = value; } }
		private int ci_aspect_ratio_info_present_flag;
		public int _CiAspectRatioInfoPresentFlag { get { return ci_aspect_ratio_info_present_flag; } set { ci_aspect_ratio_info_present_flag = value; } }
		private int ci_timing_info_present_flag;
		public int _CiTimingInfoPresentFlag { get { return ci_timing_info_present_flag; } set { ci_timing_info_present_flag = value; } }
		private int ci_reserved_2bit;
		public int _CiReserved2bit { get { return ci_reserved_2bit; } set { ci_reserved_2bit = value; } }
		private int ci_color_primaries;
		public int _CiColorPrimaries { get { return ci_color_primaries; } set { ci_color_primaries = value; } }
		private int ci_transfer_characteristics;
		public int _CiTransferCharacteristics { get { return ci_transfer_characteristics; } set { ci_transfer_characteristics = value; } }
		private int ci_matrix_coefficients;
		public int _CiMatrixCoefficients { get { return ci_matrix_coefficients; } set { ci_matrix_coefficients = value; } }
		private int ci_full_range_flag;
		public int _CiFullRangeFlag { get { return ci_full_range_flag; } set { ci_full_range_flag = value; } }
		private int ci_color_description_idc;
		public int _CiColorDescriptionIdc { get { return ci_color_description_idc; } set { ci_color_description_idc = value; } }
		private int ci_chroma_sample_position_top;
		public int _CiChromaSamplePositionTop { get { return ci_chroma_sample_position_top; } set { ci_chroma_sample_position_top = value; } }
		private int ci_chroma_sample_position_bottom;
		public int _CiChromaSamplePositionBottom { get { return ci_chroma_sample_position_bottom; } set { ci_chroma_sample_position_bottom = value; } }
		private int ci_aspect_ratio_idc;
		public int _CiAspectRatioIdc { get { return ci_aspect_ratio_idc; } set { ci_aspect_ratio_idc = value; } }
		private int ci_sar_width;
		public int _CiSarWidth { get { return ci_sar_width; } set { ci_sar_width = value; } }
		private int ci_sar_height;
		public int _CiSarHeight { get { return ci_sar_height; } set { ci_sar_height = value; } }

        private void ContentInterpretationObu()
        {
			stream.ReadFixed(2, out this.ci_scan_type_idc, "ci_scan_type_idc"); 
			stream.ReadFixed(1, out this.ci_color_description_present_flag, "ci_color_description_present_flag"); 
			stream.ReadFixed(1, out this.ci_chroma_sample_position_present_flag, "ci_chroma_sample_position_present_flag"); 
			stream.ReadFixed(1, out this.ci_aspect_ratio_info_present_flag, "ci_aspect_ratio_info_present_flag"); 
			stream.ReadFixed(1, out this.ci_timing_info_present_flag, "ci_timing_info_present_flag"); 
			stream.ReadFixed(2, out this.ci_reserved_2bit, "ci_reserved_2bit"); 
			ci_color_primaries = CP_UNSPECIFIED;
			ci_transfer_characteristics = TC_UNSPECIFIED;
			ci_matrix_coefficients = MC_UNSPECIFIED;
			ci_full_range_flag = 0;

			if ((ci_color_description_present_flag != 0))
			{
				stream.ReadRg(2, out this.ci_color_description_idc, "ci_color_description_idc"); 

				if ((ci_color_description_idc == 0))
				{
					stream.ReadFixed(8, out this.ci_color_primaries, "ci_color_primaries"); 
					stream.ReadFixed(8, out this.ci_transfer_characteristics, "ci_transfer_characteristics"); 
					stream.ReadFixed(8, out this.ci_matrix_coefficients, "ci_matrix_coefficients"); 
				}
				stream.ReadFixed(1, out this.ci_full_range_flag, "ci_full_range_flag"); 
			}

			if ((ci_chroma_sample_position_present_flag != 0))
			{
				stream.ReadUvlc( out this.ci_chroma_sample_position_top, "ci_chroma_sample_position_top"); 

				if ((ci_scan_type_idc != 1))
				{
					stream.ReadUvlc( out this.ci_chroma_sample_position_bottom, "ci_chroma_sample_position_bottom"); 
				}
				else 
				{
					ci_chroma_sample_position_bottom = ci_chroma_sample_position_top;
				}
			}
			else 
			{
				ci_chroma_sample_position_top = CSP_UNSPECIFIED;
				ci_chroma_sample_position_bottom = CSP_UNSPECIFIED;
			}

			if ((ci_aspect_ratio_info_present_flag != 0))
			{
				stream.ReadFixed(8, out this.ci_aspect_ratio_idc, "ci_aspect_ratio_idc"); 

				if ((ci_aspect_ratio_idc == 255))
				{
					stream.ReadUvlc( out this.ci_sar_width, "ci_sar_width"); 
					stream.ReadUvlc( out this.ci_sar_height, "ci_sar_height"); 
				}
				else 
				{
					ci_sar_width = Aspect_Ratio_Width[ci_aspect_ratio_idc];
					ci_sar_height = Aspect_Ratio_Height[ci_aspect_ratio_idc];
				}
			}

			if ((ci_timing_info_present_flag != 0))
			{
				TimingInfo(); 
			}
        }

        private void WriteContentInterpretationObu()
        {
			this.ci_scan_type_idc = stream.Pick("ci_scan_type_idc", _original != null ? _original.ci_scan_type_idc : this.ci_scan_type_idc, _edited != null ? _edited.ci_scan_type_idc : _original != null ? _original.ci_scan_type_idc : this.ci_scan_type_idc);
			stream.WriteFixed(2, this.ci_scan_type_idc, "ci_scan_type_idc"); 
			this.ci_color_description_present_flag = stream.Pick("ci_color_description_present_flag", _original != null ? _original.ci_color_description_present_flag : this.ci_color_description_present_flag, _edited != null ? _edited.ci_color_description_present_flag : _original != null ? _original.ci_color_description_present_flag : this.ci_color_description_present_flag);
			stream.WriteFixed(1, this.ci_color_description_present_flag, "ci_color_description_present_flag"); 
			this.ci_chroma_sample_position_present_flag = stream.Pick("ci_chroma_sample_position_present_flag", _original != null ? _original.ci_chroma_sample_position_present_flag : this.ci_chroma_sample_position_present_flag, _edited != null ? _edited.ci_chroma_sample_position_present_flag : _original != null ? _original.ci_chroma_sample_position_present_flag : this.ci_chroma_sample_position_present_flag);
			stream.WriteFixed(1, this.ci_chroma_sample_position_present_flag, "ci_chroma_sample_position_present_flag"); 
			this.ci_aspect_ratio_info_present_flag = stream.Pick("ci_aspect_ratio_info_present_flag", _original != null ? _original.ci_aspect_ratio_info_present_flag : this.ci_aspect_ratio_info_present_flag, _edited != null ? _edited.ci_aspect_ratio_info_present_flag : _original != null ? _original.ci_aspect_ratio_info_present_flag : this.ci_aspect_ratio_info_present_flag);
			stream.WriteFixed(1, this.ci_aspect_ratio_info_present_flag, "ci_aspect_ratio_info_present_flag"); 
			this.ci_timing_info_present_flag = stream.Pick("ci_timing_info_present_flag", _original != null ? _original.ci_timing_info_present_flag : this.ci_timing_info_present_flag, _edited != null ? _edited.ci_timing_info_present_flag : _original != null ? _original.ci_timing_info_present_flag : this.ci_timing_info_present_flag);
			stream.WriteFixed(1, this.ci_timing_info_present_flag, "ci_timing_info_present_flag"); 
			this.ci_reserved_2bit = stream.Pick("ci_reserved_2bit", _original != null ? _original.ci_reserved_2bit : this.ci_reserved_2bit, _edited != null ? _edited.ci_reserved_2bit : _original != null ? _original.ci_reserved_2bit : this.ci_reserved_2bit);
			stream.WriteFixed(2, this.ci_reserved_2bit, "ci_reserved_2bit"); 
			ci_color_primaries = CP_UNSPECIFIED;
			ci_transfer_characteristics = TC_UNSPECIFIED;
			ci_matrix_coefficients = MC_UNSPECIFIED;
			ci_full_range_flag = 0;

			if ((ci_color_description_present_flag != 0))
			{
				this.ci_color_description_idc = stream.Pick("ci_color_description_idc", _original != null ? _original.ci_color_description_idc : this.ci_color_description_idc, _edited != null ? _edited.ci_color_description_idc : _original != null ? _original.ci_color_description_idc : this.ci_color_description_idc);
				stream.WriteRg(2, this.ci_color_description_idc, "ci_color_description_idc"); 

				if ((ci_color_description_idc == 0))
				{
					this.ci_color_primaries = stream.Pick("ci_color_primaries", _original != null ? _original.ci_color_primaries : this.ci_color_primaries, _edited != null ? _edited.ci_color_primaries : _original != null ? _original.ci_color_primaries : this.ci_color_primaries);
					stream.WriteFixed(8, this.ci_color_primaries, "ci_color_primaries"); 
					this.ci_transfer_characteristics = stream.Pick("ci_transfer_characteristics", _original != null ? _original.ci_transfer_characteristics : this.ci_transfer_characteristics, _edited != null ? _edited.ci_transfer_characteristics : _original != null ? _original.ci_transfer_characteristics : this.ci_transfer_characteristics);
					stream.WriteFixed(8, this.ci_transfer_characteristics, "ci_transfer_characteristics"); 
					this.ci_matrix_coefficients = stream.Pick("ci_matrix_coefficients", _original != null ? _original.ci_matrix_coefficients : this.ci_matrix_coefficients, _edited != null ? _edited.ci_matrix_coefficients : _original != null ? _original.ci_matrix_coefficients : this.ci_matrix_coefficients);
					stream.WriteFixed(8, this.ci_matrix_coefficients, "ci_matrix_coefficients"); 
				}
				this.ci_full_range_flag = stream.Pick("ci_full_range_flag", _original != null ? _original.ci_full_range_flag : this.ci_full_range_flag, _edited != null ? _edited.ci_full_range_flag : _original != null ? _original.ci_full_range_flag : this.ci_full_range_flag);
				stream.WriteFixed(1, this.ci_full_range_flag, "ci_full_range_flag"); 
			}

			if ((ci_chroma_sample_position_present_flag != 0))
			{
				this.ci_chroma_sample_position_top = stream.Pick("ci_chroma_sample_position_top", _original != null ? _original.ci_chroma_sample_position_top : this.ci_chroma_sample_position_top, _edited != null ? _edited.ci_chroma_sample_position_top : _original != null ? _original.ci_chroma_sample_position_top : this.ci_chroma_sample_position_top);
				stream.WriteUvlc( this.ci_chroma_sample_position_top, "ci_chroma_sample_position_top"); 

				if ((ci_scan_type_idc != 1))
				{
					this.ci_chroma_sample_position_bottom = stream.Pick("ci_chroma_sample_position_bottom", _original != null ? _original.ci_chroma_sample_position_bottom : this.ci_chroma_sample_position_bottom, _edited != null ? _edited.ci_chroma_sample_position_bottom : _original != null ? _original.ci_chroma_sample_position_bottom : this.ci_chroma_sample_position_bottom);
					stream.WriteUvlc( this.ci_chroma_sample_position_bottom, "ci_chroma_sample_position_bottom"); 
				}
				else 
				{
					ci_chroma_sample_position_bottom = ci_chroma_sample_position_top;
				}
			}
			else 
			{
				ci_chroma_sample_position_top = CSP_UNSPECIFIED;
				ci_chroma_sample_position_bottom = CSP_UNSPECIFIED;
			}

			if ((ci_aspect_ratio_info_present_flag != 0))
			{
				this.ci_aspect_ratio_idc = stream.Pick("ci_aspect_ratio_idc", _original != null ? _original.ci_aspect_ratio_idc : this.ci_aspect_ratio_idc, _edited != null ? _edited.ci_aspect_ratio_idc : _original != null ? _original.ci_aspect_ratio_idc : this.ci_aspect_ratio_idc);
				stream.WriteFixed(8, this.ci_aspect_ratio_idc, "ci_aspect_ratio_idc"); 

				if ((ci_aspect_ratio_idc == 255))
				{
					this.ci_sar_width = stream.Pick("ci_sar_width", _original != null ? _original.ci_sar_width : this.ci_sar_width, _edited != null ? _edited.ci_sar_width : _original != null ? _original.ci_sar_width : this.ci_sar_width);
					stream.WriteUvlc( this.ci_sar_width, "ci_sar_width"); 
					this.ci_sar_height = stream.Pick("ci_sar_height", _original != null ? _original.ci_sar_height : this.ci_sar_height, _edited != null ? _edited.ci_sar_height : _original != null ? _original.ci_sar_height : this.ci_sar_height);
					stream.WriteUvlc( this.ci_sar_height, "ci_sar_height"); 
				}
				else 
				{
					ci_sar_width = Aspect_Ratio_Width[ci_aspect_ratio_idc];
					ci_sar_height = Aspect_Ratio_Height[ci_aspect_ratio_idc];
				}
			}

			if ((ci_timing_info_present_flag != 0))
			{
				WriteTimingInfo(); 
			}
        }

    /*
padding_obu() {
for ( i = 0; i < obu_padding_length; i++ ) {	
obu_padding_byte	f(8)
}	
}
    */
		private int obu_padding_byte;
		public int _ObuPaddingByte { get { return obu_padding_byte; } set { obu_padding_byte = value; } }

        private void PaddingObu()
        {
			int i = 0;

			for (i = 0; (i < obu_padding_length); i++)
			{
				stream.ReadFixed(8, out this.obu_padding_byte, "obu_padding_byte"); 
			}
        }

        private void WritePaddingObu()
        {
			int i = 0;

			for (i = 0; (i < obu_padding_length); i++)
			{
				this.obu_padding_byte = stream.Pick("obu_padding_byte", _original != null ? _original.obu_padding_byte : this.obu_padding_byte, _edited != null ? _edited.obu_padding_byte : _original != null ? _original.obu_padding_byte : this.obu_padding_byte);
				stream.WriteFixed(8, this.obu_padding_byte, "obu_padding_byte"); 
			}
        }

    /*
metadata_unit( metadataPayloadSize ) {
startPosition = get_position()	
if ( metadata_type == METADATA_TYPE_ITUT_T35 ) {	
metadata_itut_t35( metadataPayloadSize )	
} else if ( metadata_type == METADATA_TYPE_HDR_CLL ) {	
metadata_hdr_cll()	
} else if ( metadata_type == METADATA_TYPE_HDR_MDCV ) {	
metadata_hdr_mdcv()	
} else if ( metadata_type == METADATA_TYPE_TIMECODE ) {	
metadata_timecode()	
} else if ( metadata_type == METADATA_TYPE_BANDING_HINTS ) {	
metadata_banding_hints()	
} else if ( metadata_type == METADATA_TYPE_ICC_PROFILE ) {	
metadata_icc_profile( metadataPayloadSize )	
} else if ( metadata_type == METADATA_TYPE_SCAN_TYPE ) {	
metadata_scan_type()	
} else if ( metadata_type == METADATA_TYPE_TEMPORAL_POINT_INFO ) {	
metadata_temporal_point_info()	
} else if ( metadata_type == METADATA_TYPE_DECODED_FRAME_HASH ) {	
metadata_decoded_frame_hash()	
} else if ( metadata_type == METADATA_TYPE_USER_DATA_UNREGISTERED ) {	
metadata_user_data_unregistered( metadataPayloadSize )	
}	
currentPosition = get_position()	
parsedPayloadBits = currentPosition - startPosition	
remainingMuPayloadBits = metadataPayloadSize * 8 - parsedPayloadBits	
for ( j = 0; j < remainingMuPayloadBits; j++ ) {	
metadata_unit_remaining_bit	f(1)
}	
}
    */
		private int metadataPayloadSize;
		public int _MetadataPayloadSize { get { return metadataPayloadSize; } set { metadataPayloadSize = value; } }
		private int metadata_unit_remaining_bit;
		public int _MetadataUnitRemainingBit { get { return metadata_unit_remaining_bit; } set { metadata_unit_remaining_bit = value; } }

        private void MetadataUnit(int metadataPayloadSize)
        {
			int j = 0;
			int startPosition = 0;
			int currentPosition = 0;
			int parsedPayloadBits = 0;
			int remainingMuPayloadBits = 0;
			startPosition = get_position();

			if ((metadata_type == METADATA_TYPE_ITUT_T35))
			{
				MetadataItutT35(metadataPayloadSize); 
			}
			else if ((metadata_type == METADATA_TYPE_HDR_CLL))
			{
				MetadataHdrCll(); 
			}
			else if ((metadata_type == METADATA_TYPE_HDR_MDCV))
			{
				MetadataHdrMdcv(); 
			}
			else if ((metadata_type == METADATA_TYPE_TIMECODE))
			{
				MetadataTimecode(); 
			}
			else if ((metadata_type == METADATA_TYPE_BANDING_HINTS))
			{
				MetadataBandingHints(); 
			}
			else if ((metadata_type == METADATA_TYPE_ICC_PROFILE))
			{
				MetadataIccProfile(metadataPayloadSize); 
			}
			else if ((metadata_type == METADATA_TYPE_SCAN_TYPE))
			{
				MetadataScanType(); 
			}
			else if ((metadata_type == METADATA_TYPE_TEMPORAL_POINT_INFO))
			{
				MetadataTemporalPointInfo(); 
			}
			else if ((metadata_type == METADATA_TYPE_DECODED_FRAME_HASH))
			{
				MetadataDecodedFrameHash(); 
			}
			else if ((metadata_type == METADATA_TYPE_USER_DATA_UNREGISTERED))
			{
				MetadataUserDataUnregistered(metadataPayloadSize); 
			}
			currentPosition = get_position();
			parsedPayloadBits = (currentPosition - startPosition);
			remainingMuPayloadBits = ((metadataPayloadSize * 8) - parsedPayloadBits);

			for (j = 0; (j < remainingMuPayloadBits); j++)
			{
				stream.ReadFixed(1, out this.metadata_unit_remaining_bit, "metadata_unit_remaining_bit"); 
			}
        }

        private void WriteMetadataUnit(int metadataPayloadSize)
        {
			int j = 0;
			int startPosition = 0;
			int currentPosition = 0;
			int parsedPayloadBits = 0;
			int remainingMuPayloadBits = 0;
			startPosition = get_position();

			if ((metadata_type == METADATA_TYPE_ITUT_T35))
			{
				WriteMetadataItutT35(metadataPayloadSize); 
			}
			else if ((metadata_type == METADATA_TYPE_HDR_CLL))
			{
				WriteMetadataHdrCll(); 
			}
			else if ((metadata_type == METADATA_TYPE_HDR_MDCV))
			{
				WriteMetadataHdrMdcv(); 
			}
			else if ((metadata_type == METADATA_TYPE_TIMECODE))
			{
				WriteMetadataTimecode(); 
			}
			else if ((metadata_type == METADATA_TYPE_BANDING_HINTS))
			{
				WriteMetadataBandingHints(); 
			}
			else if ((metadata_type == METADATA_TYPE_ICC_PROFILE))
			{
				WriteMetadataIccProfile(metadataPayloadSize); 
			}
			else if ((metadata_type == METADATA_TYPE_SCAN_TYPE))
			{
				WriteMetadataScanType(); 
			}
			else if ((metadata_type == METADATA_TYPE_TEMPORAL_POINT_INFO))
			{
				WriteMetadataTemporalPointInfo(); 
			}
			else if ((metadata_type == METADATA_TYPE_DECODED_FRAME_HASH))
			{
				WriteMetadataDecodedFrameHash(); 
			}
			else if ((metadata_type == METADATA_TYPE_USER_DATA_UNREGISTERED))
			{
				WriteMetadataUserDataUnregistered(metadataPayloadSize); 
			}
			currentPosition = get_position();
			parsedPayloadBits = (currentPosition - startPosition);
			remainingMuPayloadBits = ((metadataPayloadSize * 8) - parsedPayloadBits);

			for (j = 0; (j < remainingMuPayloadBits); j++)
			{
				this.metadata_unit_remaining_bit = stream.Pick("metadata_unit_remaining_bit", _original != null ? _original.metadata_unit_remaining_bit : this.metadata_unit_remaining_bit, _edited != null ? _edited.metadata_unit_remaining_bit : _original != null ? _original.metadata_unit_remaining_bit : this.metadata_unit_remaining_bit);
				stream.WriteFixed(1, this.metadata_unit_remaining_bit, "metadata_unit_remaining_bit"); 
			}
        }

    /*
metadata_short_obu( obuPayloadSize ) {
metadata_is_suffix	f(1)
metadata_necessity_idc = 0	
metadata_application_id = 0	
muh_layer_idc	f(3)
muh_cancel_flag	f(1)
muh_persistence_idc	f(3)
muh_priority = 0	
metadata_type	leb128()
if ( muh_cancel_flag ) {	
return	
}	
metadataPayloadSize = obuPayloadSize - 2 - Leb128Bytes	
metadata_unit( metadataPayloadSize )	
}
    */
		private int obuPayloadSize;
		public int _ObuPayloadSize { get { return obuPayloadSize; } set { obuPayloadSize = value; } }
		private int metadata_is_suffix;
		public int _MetadataIsSuffix { get { return metadata_is_suffix; } set { metadata_is_suffix = value; } }
		private int metadata_necessity_idc;
		public int _MetadataNecessityIdc { get { return metadata_necessity_idc; } set { metadata_necessity_idc = value; } }
		private int metadata_application_id;
		public int _MetadataApplicationId { get { return metadata_application_id; } set { metadata_application_id = value; } }
		private int muh_layer_idc;
		public int _MuhLayerIdc { get { return muh_layer_idc; } set { muh_layer_idc = value; } }
		private int muh_cancel_flag;
		public int _MuhCancelFlag { get { return muh_cancel_flag; } set { muh_cancel_flag = value; } }
		private int muh_persistence_idc;
		public int _MuhPersistenceIdc { get { return muh_persistence_idc; } set { muh_persistence_idc = value; } }
		private int muh_priority;
		public int _MuhPriority { get { return muh_priority; } set { muh_priority = value; } }
		private int metadata_type;
		public int _MetadataType { get { return metadata_type; } set { metadata_type = value; } }

        private void MetadataShortObu(int obuPayloadSize)
        {
			int metadataPayloadSize = 0;
			stream.ReadFixed(1, out this.metadata_is_suffix, "metadata_is_suffix"); 
			metadata_necessity_idc = 0;
			metadata_application_id = 0;
			stream.ReadFixed(3, out this.muh_layer_idc, "muh_layer_idc"); 
			stream.ReadFixed(1, out this.muh_cancel_flag, "muh_cancel_flag"); 
			stream.ReadFixed(3, out this.muh_persistence_idc, "muh_persistence_idc"); 
			muh_priority = 0;
			stream.ReadLeb128( out this.metadata_type, "metadata_type"); 

			if ((muh_cancel_flag != 0))
			{
				return;
			}
			metadataPayloadSize = ((obuPayloadSize - 2) - Leb128Bytes);
			MetadataUnit(metadataPayloadSize); 
        }

        private void WriteMetadataShortObu(int obuPayloadSize)
        {
			int metadataPayloadSize = 0;
			this.metadata_is_suffix = stream.Pick("metadata_is_suffix", _original != null ? _original.metadata_is_suffix : this.metadata_is_suffix, _edited != null ? _edited.metadata_is_suffix : _original != null ? _original.metadata_is_suffix : this.metadata_is_suffix);
			stream.WriteFixed(1, this.metadata_is_suffix, "metadata_is_suffix"); 
			metadata_necessity_idc = 0;
			metadata_application_id = 0;
			this.muh_layer_idc = stream.Pick("muh_layer_idc", _original != null ? _original.muh_layer_idc : this.muh_layer_idc, _edited != null ? _edited.muh_layer_idc : _original != null ? _original.muh_layer_idc : this.muh_layer_idc);
			stream.WriteFixed(3, this.muh_layer_idc, "muh_layer_idc"); 
			this.muh_cancel_flag = stream.Pick("muh_cancel_flag", _original != null ? _original.muh_cancel_flag : this.muh_cancel_flag, _edited != null ? _edited.muh_cancel_flag : _original != null ? _original.muh_cancel_flag : this.muh_cancel_flag);
			stream.WriteFixed(1, this.muh_cancel_flag, "muh_cancel_flag"); 
			this.muh_persistence_idc = stream.Pick("muh_persistence_idc", _original != null ? _original.muh_persistence_idc : this.muh_persistence_idc, _edited != null ? _edited.muh_persistence_idc : _original != null ? _original.muh_persistence_idc : this.muh_persistence_idc);
			stream.WriteFixed(3, this.muh_persistence_idc, "muh_persistence_idc"); 
			muh_priority = 0;
			this.metadata_type = stream.Pick("metadata_type", _original != null ? _original.metadata_type : this.metadata_type, _edited != null ? _edited.metadata_type : _original != null ? _original.metadata_type : this.metadata_type);
			stream.WriteLeb128( this.metadata_type, "metadata_type"); 

			if ((muh_cancel_flag != 0))
			{
				return;
			}
			metadataPayloadSize = ((obuPayloadSize - 2) - Leb128Bytes);
			WriteMetadataUnit(metadataPayloadSize); 
        }

    /*
metadata_group_obu() {
metadata_is_suffix	f(1)
metadata_necessity_idc	f(2)
metadata_application_id	f(5)
metadata_unit_cnt_minus_1	leb128()
for ( i = 0; i <= metadata_unit_cnt_minus_1; i++ ) {	
metadata_type	leb128()
muh_header_size	f(7)
muh_cancel_flag	f(1)
headerRemainingBytes = muh_header_size	
if ( !muh_cancel_flag ) {	
muh_payload_size	leb128()
headerRemainingBytes -= Leb128Bytes	
muh_layer_idc	f(3)
muh_persistence_idc	f(3)
muh_priority	f(8)
muh_reserved_zero_2bits	f(2)
headerRemainingBytes -= 2	
if ( muh_layer_idc == LAYER_VALUES ) {	
if ( obu_xlayer_id == GLOBAL_XLAYER_ID ) {	
muh_xlayer_map	f(32)
headerRemainingBytes -= 4	
for ( n = 0; n < 31; n++ ) {	
if ( muh_xlayer_map & (0x1 << n) ) {	
muh_mlayer_map	f(8)
headerRemainingBytes -= 1	
}	
}	
} else {	
muh_mlayer_map	f(8)
headerRemainingBytes -= 1	
}	
}	
}	
for ( j = 0; j < headerRemainingBytes; j++ ) {	
muh_header_extension_byte	f(8)
}	
if ( !muh_cancel_flag ) {	
metadata_unit( muh_payload_size )	
}	
}	
}
    */
		private int metadata_unit_cnt_minus_1;
		public int _MetadataUnitCntMinus1 { get { return metadata_unit_cnt_minus_1; } set { metadata_unit_cnt_minus_1 = value; } }
		private int muh_header_size;
		public int _MuhHeaderSize { get { return muh_header_size; } set { muh_header_size = value; } }
		private int muh_payload_size;
		public int _MuhPayloadSize { get { return muh_payload_size; } set { muh_payload_size = value; } }
		private int muh_reserved_zero_2bits;
		public int _MuhReservedZero2bits { get { return muh_reserved_zero_2bits; } set { muh_reserved_zero_2bits = value; } }
		private int muh_xlayer_map;
		public int _MuhXlayerMap { get { return muh_xlayer_map; } set { muh_xlayer_map = value; } }
		private int muh_mlayer_map;
		public int _MuhMlayerMap { get { return muh_mlayer_map; } set { muh_mlayer_map = value; } }
		private int muh_header_extension_byte;
		public int _MuhHeaderExtensionByte { get { return muh_header_extension_byte; } set { muh_header_extension_byte = value; } }

        private void MetadataGroupObu()
        {
			int i = 0;
			int n = 0;
			int j = 0;
			int headerRemainingBytes = 0;
			stream.ReadFixed(1, out this.metadata_is_suffix, "metadata_is_suffix"); 
			stream.ReadFixed(2, out this.metadata_necessity_idc, "metadata_necessity_idc"); 
			stream.ReadFixed(5, out this.metadata_application_id, "metadata_application_id"); 
			stream.ReadLeb128( out this.metadata_unit_cnt_minus_1, "metadata_unit_cnt_minus_1"); 

			for (i = 0; (i <= metadata_unit_cnt_minus_1); i++)
			{
				stream.ReadLeb128( out this.metadata_type, "metadata_type"); 
				stream.ReadFixed(7, out this.muh_header_size, "muh_header_size"); 
				stream.ReadFixed(1, out this.muh_cancel_flag, "muh_cancel_flag"); 
				headerRemainingBytes = muh_header_size;

				if (!(muh_cancel_flag != 0))
				{
					stream.ReadLeb128( out this.muh_payload_size, "muh_payload_size"); 
					headerRemainingBytes -= Leb128Bytes;
					stream.ReadFixed(3, out this.muh_layer_idc, "muh_layer_idc"); 
					stream.ReadFixed(3, out this.muh_persistence_idc, "muh_persistence_idc"); 
					stream.ReadFixed(8, out this.muh_priority, "muh_priority"); 
					stream.ReadFixed(2, out this.muh_reserved_zero_2bits, "muh_reserved_zero_2bits"); 
					headerRemainingBytes -= 2;

					if ((muh_layer_idc == LAYER_VALUES))
					{

						if ((obu_xlayer_id == GLOBAL_XLAYER_ID))
						{
							stream.ReadFixed(32, out this.muh_xlayer_map, "muh_xlayer_map"); 
							headerRemainingBytes -= 4;

							for (n = 0; (n < 31); n++)
							{

								if (((muh_xlayer_map & (0x1 << n)) != 0))
								{
									stream.ReadFixed(8, out this.muh_mlayer_map, "muh_mlayer_map"); 
									headerRemainingBytes -= 1;
								}
							}
						}
						else 
						{
							stream.ReadFixed(8, out this.muh_mlayer_map, "muh_mlayer_map"); 
							headerRemainingBytes -= 1;
						}
					}
				}

				for (j = 0; (j < headerRemainingBytes); j++)
				{
					stream.ReadFixed(8, out this.muh_header_extension_byte, "muh_header_extension_byte"); 
				}

				if (!(muh_cancel_flag != 0))
				{
					MetadataUnit(muh_payload_size); 
				}
			}
        }

        private void WriteMetadataGroupObu()
        {
			int i = 0;
			int n = 0;
			int j = 0;
			int headerRemainingBytes = 0;
			this.metadata_is_suffix = stream.Pick("metadata_is_suffix", _original != null ? _original.metadata_is_suffix : this.metadata_is_suffix, _edited != null ? _edited.metadata_is_suffix : _original != null ? _original.metadata_is_suffix : this.metadata_is_suffix);
			stream.WriteFixed(1, this.metadata_is_suffix, "metadata_is_suffix"); 
			this.metadata_necessity_idc = stream.Pick("metadata_necessity_idc", _original != null ? _original.metadata_necessity_idc : this.metadata_necessity_idc, _edited != null ? _edited.metadata_necessity_idc : _original != null ? _original.metadata_necessity_idc : this.metadata_necessity_idc);
			stream.WriteFixed(2, this.metadata_necessity_idc, "metadata_necessity_idc"); 
			this.metadata_application_id = stream.Pick("metadata_application_id", _original != null ? _original.metadata_application_id : this.metadata_application_id, _edited != null ? _edited.metadata_application_id : _original != null ? _original.metadata_application_id : this.metadata_application_id);
			stream.WriteFixed(5, this.metadata_application_id, "metadata_application_id"); 
			this.metadata_unit_cnt_minus_1 = stream.Pick("metadata_unit_cnt_minus_1", _original != null ? _original.metadata_unit_cnt_minus_1 : this.metadata_unit_cnt_minus_1, _edited != null ? _edited.metadata_unit_cnt_minus_1 : _original != null ? _original.metadata_unit_cnt_minus_1 : this.metadata_unit_cnt_minus_1);
			stream.WriteLeb128( this.metadata_unit_cnt_minus_1, "metadata_unit_cnt_minus_1"); 

			for (i = 0; (i <= metadata_unit_cnt_minus_1); i++)
			{
				this.metadata_type = stream.Pick("metadata_type", _original != null ? _original.metadata_type : this.metadata_type, _edited != null ? _edited.metadata_type : _original != null ? _original.metadata_type : this.metadata_type);
				stream.WriteLeb128( this.metadata_type, "metadata_type"); 
				this.muh_header_size = stream.Pick("muh_header_size", _original != null ? _original.muh_header_size : this.muh_header_size, _edited != null ? _edited.muh_header_size : _original != null ? _original.muh_header_size : this.muh_header_size);
				stream.WriteFixed(7, this.muh_header_size, "muh_header_size"); 
				this.muh_cancel_flag = stream.Pick("muh_cancel_flag", _original != null ? _original.muh_cancel_flag : this.muh_cancel_flag, _edited != null ? _edited.muh_cancel_flag : _original != null ? _original.muh_cancel_flag : this.muh_cancel_flag);
				stream.WriteFixed(1, this.muh_cancel_flag, "muh_cancel_flag"); 
				headerRemainingBytes = muh_header_size;

				if (!(muh_cancel_flag != 0))
				{
					this.muh_payload_size = stream.Pick("muh_payload_size", _original != null ? _original.muh_payload_size : this.muh_payload_size, _edited != null ? _edited.muh_payload_size : _original != null ? _original.muh_payload_size : this.muh_payload_size);
					stream.WriteLeb128( this.muh_payload_size, "muh_payload_size"); 
					headerRemainingBytes -= Leb128Bytes;
					this.muh_layer_idc = stream.Pick("muh_layer_idc", _original != null ? _original.muh_layer_idc : this.muh_layer_idc, _edited != null ? _edited.muh_layer_idc : _original != null ? _original.muh_layer_idc : this.muh_layer_idc);
					stream.WriteFixed(3, this.muh_layer_idc, "muh_layer_idc"); 
					this.muh_persistence_idc = stream.Pick("muh_persistence_idc", _original != null ? _original.muh_persistence_idc : this.muh_persistence_idc, _edited != null ? _edited.muh_persistence_idc : _original != null ? _original.muh_persistence_idc : this.muh_persistence_idc);
					stream.WriteFixed(3, this.muh_persistence_idc, "muh_persistence_idc"); 
					this.muh_priority = stream.Pick("muh_priority", _original != null ? _original.muh_priority : this.muh_priority, _edited != null ? _edited.muh_priority : _original != null ? _original.muh_priority : this.muh_priority);
					stream.WriteFixed(8, this.muh_priority, "muh_priority"); 
					this.muh_reserved_zero_2bits = stream.Pick("muh_reserved_zero_2bits", _original != null ? _original.muh_reserved_zero_2bits : this.muh_reserved_zero_2bits, _edited != null ? _edited.muh_reserved_zero_2bits : _original != null ? _original.muh_reserved_zero_2bits : this.muh_reserved_zero_2bits);
					stream.WriteFixed(2, this.muh_reserved_zero_2bits, "muh_reserved_zero_2bits"); 
					headerRemainingBytes -= 2;

					if ((muh_layer_idc == LAYER_VALUES))
					{

						if ((obu_xlayer_id == GLOBAL_XLAYER_ID))
						{
							this.muh_xlayer_map = stream.Pick("muh_xlayer_map", _original != null ? _original.muh_xlayer_map : this.muh_xlayer_map, _edited != null ? _edited.muh_xlayer_map : _original != null ? _original.muh_xlayer_map : this.muh_xlayer_map);
							stream.WriteFixed(32, this.muh_xlayer_map, "muh_xlayer_map"); 
							headerRemainingBytes -= 4;

							for (n = 0; (n < 31); n++)
							{

								if (((muh_xlayer_map & (0x1 << n)) != 0))
								{
									this.muh_mlayer_map = stream.Pick("muh_mlayer_map", _original != null ? _original.muh_mlayer_map : this.muh_mlayer_map, _edited != null ? _edited.muh_mlayer_map : _original != null ? _original.muh_mlayer_map : this.muh_mlayer_map);
									stream.WriteFixed(8, this.muh_mlayer_map, "muh_mlayer_map"); 
									headerRemainingBytes -= 1;
								}
							}
						}
						else 
						{
							this.muh_mlayer_map = stream.Pick("muh_mlayer_map", _original != null ? _original.muh_mlayer_map : this.muh_mlayer_map, _edited != null ? _edited.muh_mlayer_map : _original != null ? _original.muh_mlayer_map : this.muh_mlayer_map);
							stream.WriteFixed(8, this.muh_mlayer_map, "muh_mlayer_map"); 
							headerRemainingBytes -= 1;
						}
					}
				}

				for (j = 0; (j < headerRemainingBytes); j++)
				{
					this.muh_header_extension_byte = stream.Pick("muh_header_extension_byte", _original != null ? _original.muh_header_extension_byte : this.muh_header_extension_byte, _edited != null ? _edited.muh_header_extension_byte : _original != null ? _original.muh_header_extension_byte : this.muh_header_extension_byte);
					stream.WriteFixed(8, this.muh_header_extension_byte, "muh_header_extension_byte"); 
				}

				if (!(muh_cancel_flag != 0))
				{
					WriteMetadataUnit(muh_payload_size); 
				}
			}
        }

    /*
metadata_itut_t35( metadataPayloadSize ) {
itu_t_t35_country_code	f(8)
t35PayloadSize = metadataPayloadSize - 1	
if ( itu_t_t35_country_code == 0xFF ) {	
itu_t_t35_country_code_extension_byte	f(8)
t35PayloadSize--	
}	
itu_t_t35_payload_bytes	le(t35PayloadSize)
}
    */
		private int itu_t_t35_country_code;
		public int _ItutT35CountryCode { get { return itu_t_t35_country_code; } set { itu_t_t35_country_code = value; } }
		private int itu_t_t35_country_code_extension_byte;
		public int _ItutT35CountryCodeExtensionByte { get { return itu_t_t35_country_code_extension_byte; } set { itu_t_t35_country_code_extension_byte = value; } }
		private int itu_t_t35_payload_bytes;
		public int _ItutT35PayloadBytes { get { return itu_t_t35_payload_bytes; } set { itu_t_t35_payload_bytes = value; } }

        private void MetadataItutT35(int metadataPayloadSize)
        {
			int t35PayloadSize = 0;
			stream.ReadFixed(8, out this.itu_t_t35_country_code, "itu_t_t35_country_code"); 
			t35PayloadSize = (metadataPayloadSize - 1);

			if ((itu_t_t35_country_code == 0xFF))
			{
				stream.ReadFixed(8, out this.itu_t_t35_country_code_extension_byte, "itu_t_t35_country_code_extension_byte"); 
				t35PayloadSize--;
			}
			stream.ReadLe(t35PayloadSize, out this.itu_t_t35_payload_bytes, "itu_t_t35_payload_bytes"); 
        }

        private void WriteMetadataItutT35(int metadataPayloadSize)
        {
			int t35PayloadSize = 0;
			this.itu_t_t35_country_code = stream.Pick("itu_t_t35_country_code", _original != null ? _original.itu_t_t35_country_code : this.itu_t_t35_country_code, _edited != null ? _edited.itu_t_t35_country_code : _original != null ? _original.itu_t_t35_country_code : this.itu_t_t35_country_code);
			stream.WriteFixed(8, this.itu_t_t35_country_code, "itu_t_t35_country_code"); 
			t35PayloadSize = (metadataPayloadSize - 1);

			if ((itu_t_t35_country_code == 0xFF))
			{
				this.itu_t_t35_country_code_extension_byte = stream.Pick("itu_t_t35_country_code_extension_byte", _original != null ? _original.itu_t_t35_country_code_extension_byte : this.itu_t_t35_country_code_extension_byte, _edited != null ? _edited.itu_t_t35_country_code_extension_byte : _original != null ? _original.itu_t_t35_country_code_extension_byte : this.itu_t_t35_country_code_extension_byte);
				stream.WriteFixed(8, this.itu_t_t35_country_code_extension_byte, "itu_t_t35_country_code_extension_byte"); 
				t35PayloadSize--;
			}
			this.itu_t_t35_payload_bytes = stream.Pick("itu_t_t35_payload_bytes", _original != null ? _original.itu_t_t35_payload_bytes : this.itu_t_t35_payload_bytes, _edited != null ? _edited.itu_t_t35_payload_bytes : _original != null ? _original.itu_t_t35_payload_bytes : this.itu_t_t35_payload_bytes);
			stream.WriteLe(t35PayloadSize, this.itu_t_t35_payload_bytes, "itu_t_t35_payload_bytes"); 
        }

    /*
metadata_hdr_cll() {
max_cll	f(16)
max_fall	f(16)
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
primary_chromaticity_x[ i ]	f(16)
primary_chromaticity_y[ i ]	f(16)
}	
white_point_chromaticity_x	f(16)
white_point_chromaticity_y	f(16)
luminance_max	f(32)
luminance_min	f(32)
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
metadata_timecode() {
counting_type	f(5)
full_timestamp_flag	f(1)
discontinuity_flag	f(1)
cnt_dropped_flag	f(1)
n_frames	f(9)
if ( full_timestamp_flag ) {	
seconds_value	f(6)
minutes_value	f(6)
hours_value	f(5)
} else {	
seconds_flag	f(1)
if ( seconds_flag ) {	
seconds_value	f(6)
minutes_flag	f(1)
if ( minutes_flag ) {	
minutes_value	f(6)
hours_flag	f(1)
if ( hours_flag ) {	
hours_value	f(5)
}	
}	
}	
}	
time_offset_length	f(5)
if ( time_offset_length > 0 ) {	
time_offset_value	f(time_offset_length)
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
metadata_banding_hints() {
coding_banding_present_flag	f(1)
source_banding_present_flag	f(1)
if ( coding_banding_present_flag ) {	
banding_hints_flag	f(1)
if ( banding_hints_flag ) {	
three_color_components_flag	f(1)
numComponents = three_color_components_flag ? 3 : 1	
for ( plane = 0; plane < numComponents; plane++ ) {	
banding_in_component_present_flag	f(1)
if ( banding_in_component_present_flag ) {	
max_band_width_minus_4	f(6)
max_band_step_minus_1	f(4)
}	
}	
band_units_information_present_flag	f(1)
if ( band_units_information_present_flag ) {	
num_band_units_rows_minus_1	f(5)
num_band_units_cols_minus_1	f(5)
varying_size_band_units_flag	f(1)
if ( varying_size_band_units_flag ) {	
band_block_in_luma_samples	f(3)
for ( r = 0; r <= num_band_units_rows_minus_1; r++ ) {	
vert_size_in_band_blocks_minus_1	f(5)
}	
for ( c = 0; c <= num_band_units_cols_minus_1; c++ ) {	
horz_size_in_band_blocks_minus_1	f(5)
}	
}	
for ( r = 0; r <= num_band_units_rows_minus_1; r++ ) {	
for ( c = 0; c <= num_band_units_cols_minus_1; c++ ) {	
banding_in_band_unit_present_flag	f(1)
}	
}	
}	
}	
}	
}
    */
		private int coding_banding_present_flag;
		public int _CodingBandingPresentFlag { get { return coding_banding_present_flag; } set { coding_banding_present_flag = value; } }
		private int source_banding_present_flag;
		public int _SourceBandingPresentFlag { get { return source_banding_present_flag; } set { source_banding_present_flag = value; } }
		private int banding_hints_flag;
		public int _BandingHintsFlag { get { return banding_hints_flag; } set { banding_hints_flag = value; } }
		private int three_color_components_flag;
		public int _ThreeColorComponentsFlag { get { return three_color_components_flag; } set { three_color_components_flag = value; } }
		private int banding_in_component_present_flag;
		public int _BandingInComponentPresentFlag { get { return banding_in_component_present_flag; } set { banding_in_component_present_flag = value; } }
		private int max_band_width_minus_4;
		public int _MaxBandWidthMinus4 { get { return max_band_width_minus_4; } set { max_band_width_minus_4 = value; } }
		private int max_band_step_minus_1;
		public int _MaxBandStepMinus1 { get { return max_band_step_minus_1; } set { max_band_step_minus_1 = value; } }
		private int band_units_information_present_flag;
		public int _BandUnitsInformationPresentFlag { get { return band_units_information_present_flag; } set { band_units_information_present_flag = value; } }
		private int num_band_units_rows_minus_1;
		public int _NumBandUnitsRowsMinus1 { get { return num_band_units_rows_minus_1; } set { num_band_units_rows_minus_1 = value; } }
		private int num_band_units_cols_minus_1;
		public int _NumBandUnitsColsMinus1 { get { return num_band_units_cols_minus_1; } set { num_band_units_cols_minus_1 = value; } }
		private int varying_size_band_units_flag;
		public int _VaryingSizeBandUnitsFlag { get { return varying_size_band_units_flag; } set { varying_size_band_units_flag = value; } }
		private int band_block_in_luma_samples;
		public int _BandBlockInLumaSamples { get { return band_block_in_luma_samples; } set { band_block_in_luma_samples = value; } }
		private int vert_size_in_band_blocks_minus_1;
		public int _VertSizeInBandBlocksMinus1 { get { return vert_size_in_band_blocks_minus_1; } set { vert_size_in_band_blocks_minus_1 = value; } }
		private int horz_size_in_band_blocks_minus_1;
		public int _HorzSizeInBandBlocksMinus1 { get { return horz_size_in_band_blocks_minus_1; } set { horz_size_in_band_blocks_minus_1 = value; } }
		private int banding_in_band_unit_present_flag;
		public int _BandingInBandUnitPresentFlag { get { return banding_in_band_unit_present_flag; } set { banding_in_band_unit_present_flag = value; } }
		private int r = 0;

        private void MetadataBandingHints()
        {
			int plane = 0;
			int r = 0;
			int c = 0;
			int numComponents = 0;
			stream.ReadFixed(1, out this.coding_banding_present_flag, "coding_banding_present_flag"); 
			stream.ReadFixed(1, out this.source_banding_present_flag, "source_banding_present_flag"); 

			if ((coding_banding_present_flag != 0))
			{
				stream.ReadFixed(1, out this.banding_hints_flag, "banding_hints_flag"); 

				if ((banding_hints_flag != 0))
				{
					stream.ReadFixed(1, out this.three_color_components_flag, "three_color_components_flag"); 
					numComponents = ((three_color_components_flag != 0) ? 3 : 1);

					for (plane = 0; (plane < numComponents); plane++)
					{
						stream.ReadFixed(1, out this.banding_in_component_present_flag, "banding_in_component_present_flag"); 

						if ((banding_in_component_present_flag != 0))
						{
							stream.ReadFixed(6, out this.max_band_width_minus_4, "max_band_width_minus_4"); 
							stream.ReadFixed(4, out this.max_band_step_minus_1, "max_band_step_minus_1"); 
						}
					}
					stream.ReadFixed(1, out this.band_units_information_present_flag, "band_units_information_present_flag"); 

					if ((band_units_information_present_flag != 0))
					{
						stream.ReadFixed(5, out this.num_band_units_rows_minus_1, "num_band_units_rows_minus_1"); 
						stream.ReadFixed(5, out this.num_band_units_cols_minus_1, "num_band_units_cols_minus_1"); 
						stream.ReadFixed(1, out this.varying_size_band_units_flag, "varying_size_band_units_flag"); 

						if ((varying_size_band_units_flag != 0))
						{
							stream.ReadFixed(3, out this.band_block_in_luma_samples, "band_block_in_luma_samples"); 

							for (r = 0; (r <= num_band_units_rows_minus_1); r++)
							{
								stream.ReadFixed(5, out this.vert_size_in_band_blocks_minus_1, "vert_size_in_band_blocks_minus_1"); 
							}

							for (c = 0; (c <= num_band_units_cols_minus_1); c++)
							{
								stream.ReadFixed(5, out this.horz_size_in_band_blocks_minus_1, "horz_size_in_band_blocks_minus_1"); 
							}
						}

						for (r = 0; (r <= num_band_units_rows_minus_1); r++)
						{

							for (c = 0; (c <= num_band_units_cols_minus_1); c++)
							{
								stream.ReadFixed(1, out this.banding_in_band_unit_present_flag, "banding_in_band_unit_present_flag"); 
							}
						}
					}
				}
			}
        }

        private void WriteMetadataBandingHints()
        {
			int plane = 0;
			int r = 0;
			int c = 0;
			int numComponents = 0;
			this.coding_banding_present_flag = stream.Pick("coding_banding_present_flag", _original != null ? _original.coding_banding_present_flag : this.coding_banding_present_flag, _edited != null ? _edited.coding_banding_present_flag : _original != null ? _original.coding_banding_present_flag : this.coding_banding_present_flag);
			stream.WriteFixed(1, this.coding_banding_present_flag, "coding_banding_present_flag"); 
			this.source_banding_present_flag = stream.Pick("source_banding_present_flag", _original != null ? _original.source_banding_present_flag : this.source_banding_present_flag, _edited != null ? _edited.source_banding_present_flag : _original != null ? _original.source_banding_present_flag : this.source_banding_present_flag);
			stream.WriteFixed(1, this.source_banding_present_flag, "source_banding_present_flag"); 

			if ((coding_banding_present_flag != 0))
			{
				this.banding_hints_flag = stream.Pick("banding_hints_flag", _original != null ? _original.banding_hints_flag : this.banding_hints_flag, _edited != null ? _edited.banding_hints_flag : _original != null ? _original.banding_hints_flag : this.banding_hints_flag);
				stream.WriteFixed(1, this.banding_hints_flag, "banding_hints_flag"); 

				if ((banding_hints_flag != 0))
				{
					this.three_color_components_flag = stream.Pick("three_color_components_flag", _original != null ? _original.three_color_components_flag : this.three_color_components_flag, _edited != null ? _edited.three_color_components_flag : _original != null ? _original.three_color_components_flag : this.three_color_components_flag);
					stream.WriteFixed(1, this.three_color_components_flag, "three_color_components_flag"); 
					numComponents = ((three_color_components_flag != 0) ? 3 : 1);

					for (plane = 0; (plane < numComponents); plane++)
					{
						this.banding_in_component_present_flag = stream.Pick("banding_in_component_present_flag", _original != null ? _original.banding_in_component_present_flag : this.banding_in_component_present_flag, _edited != null ? _edited.banding_in_component_present_flag : _original != null ? _original.banding_in_component_present_flag : this.banding_in_component_present_flag);
						stream.WriteFixed(1, this.banding_in_component_present_flag, "banding_in_component_present_flag"); 

						if ((banding_in_component_present_flag != 0))
						{
							this.max_band_width_minus_4 = stream.Pick("max_band_width_minus_4", _original != null ? _original.max_band_width_minus_4 : this.max_band_width_minus_4, _edited != null ? _edited.max_band_width_minus_4 : _original != null ? _original.max_band_width_minus_4 : this.max_band_width_minus_4);
							stream.WriteFixed(6, this.max_band_width_minus_4, "max_band_width_minus_4"); 
							this.max_band_step_minus_1 = stream.Pick("max_band_step_minus_1", _original != null ? _original.max_band_step_minus_1 : this.max_band_step_minus_1, _edited != null ? _edited.max_band_step_minus_1 : _original != null ? _original.max_band_step_minus_1 : this.max_band_step_minus_1);
							stream.WriteFixed(4, this.max_band_step_minus_1, "max_band_step_minus_1"); 
						}
					}
					this.band_units_information_present_flag = stream.Pick("band_units_information_present_flag", _original != null ? _original.band_units_information_present_flag : this.band_units_information_present_flag, _edited != null ? _edited.band_units_information_present_flag : _original != null ? _original.band_units_information_present_flag : this.band_units_information_present_flag);
					stream.WriteFixed(1, this.band_units_information_present_flag, "band_units_information_present_flag"); 

					if ((band_units_information_present_flag != 0))
					{
						this.num_band_units_rows_minus_1 = stream.Pick("num_band_units_rows_minus_1", _original != null ? _original.num_band_units_rows_minus_1 : this.num_band_units_rows_minus_1, _edited != null ? _edited.num_band_units_rows_minus_1 : _original != null ? _original.num_band_units_rows_minus_1 : this.num_band_units_rows_minus_1);
						stream.WriteFixed(5, this.num_band_units_rows_minus_1, "num_band_units_rows_minus_1"); 
						this.num_band_units_cols_minus_1 = stream.Pick("num_band_units_cols_minus_1", _original != null ? _original.num_band_units_cols_minus_1 : this.num_band_units_cols_minus_1, _edited != null ? _edited.num_band_units_cols_minus_1 : _original != null ? _original.num_band_units_cols_minus_1 : this.num_band_units_cols_minus_1);
						stream.WriteFixed(5, this.num_band_units_cols_minus_1, "num_band_units_cols_minus_1"); 
						this.varying_size_band_units_flag = stream.Pick("varying_size_band_units_flag", _original != null ? _original.varying_size_band_units_flag : this.varying_size_band_units_flag, _edited != null ? _edited.varying_size_band_units_flag : _original != null ? _original.varying_size_band_units_flag : this.varying_size_band_units_flag);
						stream.WriteFixed(1, this.varying_size_band_units_flag, "varying_size_band_units_flag"); 

						if ((varying_size_band_units_flag != 0))
						{
							this.band_block_in_luma_samples = stream.Pick("band_block_in_luma_samples", _original != null ? _original.band_block_in_luma_samples : this.band_block_in_luma_samples, _edited != null ? _edited.band_block_in_luma_samples : _original != null ? _original.band_block_in_luma_samples : this.band_block_in_luma_samples);
							stream.WriteFixed(3, this.band_block_in_luma_samples, "band_block_in_luma_samples"); 

							for (r = 0; (r <= num_band_units_rows_minus_1); r++)
							{
								this.vert_size_in_band_blocks_minus_1 = stream.Pick("vert_size_in_band_blocks_minus_1", _original != null ? _original.vert_size_in_band_blocks_minus_1 : this.vert_size_in_band_blocks_minus_1, _edited != null ? _edited.vert_size_in_band_blocks_minus_1 : _original != null ? _original.vert_size_in_band_blocks_minus_1 : this.vert_size_in_band_blocks_minus_1);
								stream.WriteFixed(5, this.vert_size_in_band_blocks_minus_1, "vert_size_in_band_blocks_minus_1"); 
							}

							for (c = 0; (c <= num_band_units_cols_minus_1); c++)
							{
								this.horz_size_in_band_blocks_minus_1 = stream.Pick("horz_size_in_band_blocks_minus_1", _original != null ? _original.horz_size_in_band_blocks_minus_1 : this.horz_size_in_band_blocks_minus_1, _edited != null ? _edited.horz_size_in_band_blocks_minus_1 : _original != null ? _original.horz_size_in_band_blocks_minus_1 : this.horz_size_in_band_blocks_minus_1);
								stream.WriteFixed(5, this.horz_size_in_band_blocks_minus_1, "horz_size_in_band_blocks_minus_1"); 
							}
						}

						for (r = 0; (r <= num_band_units_rows_minus_1); r++)
						{

							for (c = 0; (c <= num_band_units_cols_minus_1); c++)
							{
								this.banding_in_band_unit_present_flag = stream.Pick("banding_in_band_unit_present_flag", _original != null ? _original.banding_in_band_unit_present_flag : this.banding_in_band_unit_present_flag, _edited != null ? _edited.banding_in_band_unit_present_flag : _original != null ? _original.banding_in_band_unit_present_flag : this.banding_in_band_unit_present_flag);
								stream.WriteFixed(1, this.banding_in_band_unit_present_flag, "banding_in_band_unit_present_flag"); 
							}
						}
					}
				}
			}
        }

    /*
metadata_icc_profile( metadataPayloadSize ) {
icc_profile_data_payload_bytes	le(metadataPayloadSize)
}
    */
		private int icc_profile_data_payload_bytes;
		public int _IccProfileDataPayloadBytes { get { return icc_profile_data_payload_bytes; } set { icc_profile_data_payload_bytes = value; } }

        private void MetadataIccProfile(int metadataPayloadSize)
        {
			stream.ReadLe(metadataPayloadSize, out this.icc_profile_data_payload_bytes, "icc_profile_data_payload_bytes"); 
        }

        private void WriteMetadataIccProfile(int metadataPayloadSize)
        {
			this.icc_profile_data_payload_bytes = stream.Pick("icc_profile_data_payload_bytes", _original != null ? _original.icc_profile_data_payload_bytes : this.icc_profile_data_payload_bytes, _edited != null ? _edited.icc_profile_data_payload_bytes : _original != null ? _original.icc_profile_data_payload_bytes : this.icc_profile_data_payload_bytes);
			stream.WriteLe(metadataPayloadSize, this.icc_profile_data_payload_bytes, "icc_profile_data_payload_bytes"); 
        }

    /*
metadata_scan_type() {
mps_pic_struct_type	f(5)
mps_source_scan_type_idc	f(2)
mps_duplicate_flag	f(1)
}
    */
		private int mps_pic_struct_type;
		public int _MpsPicStructType { get { return mps_pic_struct_type; } set { mps_pic_struct_type = value; } }
		private int mps_source_scan_type_idc;
		public int _MpsSourceScanTypeIdc { get { return mps_source_scan_type_idc; } set { mps_source_scan_type_idc = value; } }
		private int mps_duplicate_flag;
		public int _MpsDuplicateFlag { get { return mps_duplicate_flag; } set { mps_duplicate_flag = value; } }

        private void MetadataScanType()
        {
			stream.ReadFixed(5, out this.mps_pic_struct_type, "mps_pic_struct_type"); 
			stream.ReadFixed(2, out this.mps_source_scan_type_idc, "mps_source_scan_type_idc"); 
			stream.ReadFixed(1, out this.mps_duplicate_flag, "mps_duplicate_flag"); 
        }

        private void WriteMetadataScanType()
        {
			this.mps_pic_struct_type = stream.Pick("mps_pic_struct_type", _original != null ? _original.mps_pic_struct_type : this.mps_pic_struct_type, _edited != null ? _edited.mps_pic_struct_type : _original != null ? _original.mps_pic_struct_type : this.mps_pic_struct_type);
			stream.WriteFixed(5, this.mps_pic_struct_type, "mps_pic_struct_type"); 
			this.mps_source_scan_type_idc = stream.Pick("mps_source_scan_type_idc", _original != null ? _original.mps_source_scan_type_idc : this.mps_source_scan_type_idc, _edited != null ? _edited.mps_source_scan_type_idc : _original != null ? _original.mps_source_scan_type_idc : this.mps_source_scan_type_idc);
			stream.WriteFixed(2, this.mps_source_scan_type_idc, "mps_source_scan_type_idc"); 
			this.mps_duplicate_flag = stream.Pick("mps_duplicate_flag", _original != null ? _original.mps_duplicate_flag : this.mps_duplicate_flag, _edited != null ? _edited.mps_duplicate_flag : _original != null ? _original.mps_duplicate_flag : this.mps_duplicate_flag);
			stream.WriteFixed(1, this.mps_duplicate_flag, "mps_duplicate_flag"); 
        }

    /*
metadata_temporal_point_info() {
frame_presentation_time	leb128()
}
    */
		private int frame_presentation_time;
		public int _FramePresentationTime { get { return frame_presentation_time; } set { frame_presentation_time = value; } }

        private void MetadataTemporalPointInfo()
        {
			stream.ReadLeb128( out this.frame_presentation_time, "frame_presentation_time"); 
        }

        private void WriteMetadataTemporalPointInfo()
        {
			this.frame_presentation_time = stream.Pick("frame_presentation_time", _original != null ? _original.frame_presentation_time : this.frame_presentation_time, _edited != null ? _edited.frame_presentation_time : _original != null ? _original.frame_presentation_time : this.frame_presentation_time);
			stream.WriteLeb128( this.frame_presentation_time, "frame_presentation_time"); 
        }

    /*
metadata_decoded_frame_hash() {
hash_type	f(4)
per_plane	f(1)
has_grain	f(1)
is_monochrome	f(1)
reserved	f(1)
if ( per_plane ) {	
numPlanes = is_monochrome ? 1 : 3	
for ( i = 0; i < numPlanes; i++ ) {	
plane_hash[ i ]	le(16)
}	
} else {	
frame_hash	le(16)
}	
}
    */
		private int hash_type;
		public int _HashType { get { return hash_type; } set { hash_type = value; } }
		private int per_plane;
		public int _PerPlane { get { return per_plane; } set { per_plane = value; } }
		private int has_grain;
		public int _HasGrain { get { return has_grain; } set { has_grain = value; } }
		private int is_monochrome;
		public int _IsMonochrome { get { return is_monochrome; } set { is_monochrome = value; } }
		private int reserved;
		public int _Reserved { get { return reserved; } set { reserved = value; } }
		private AomArray<byte[]> plane_hash = new AomArray<byte[]>();
		public AomArray<byte[]> _PlaneHash { get { return plane_hash; } set { plane_hash = value; } }
		private byte[] frame_hash;
		public byte[] _FrameHash { get { return frame_hash; } set { frame_hash = value; } }

        private void MetadataDecodedFrameHash()
        {
			int i = 0;
			int numPlanes = 0;
			stream.ReadFixed(4, out this.hash_type, "hash_type"); 
			stream.ReadFixed(1, out this.per_plane, "per_plane"); 
			stream.ReadFixed(1, out this.has_grain, "has_grain"); 
			stream.ReadFixed(1, out this.is_monochrome, "is_monochrome"); 
			stream.ReadFixed(1, out this.reserved, "reserved"); 

			if ((per_plane != 0))
			{
				numPlanes = ((is_monochrome != 0) ? 1 : 3);

				for (i = 0; (i < numPlanes); i++)
				{
					stream.ReadBytes(128, out this.plane_hash[i], "plane_hash"); 
				}
			}
			else 
			{
				stream.ReadBytes(128, out this.frame_hash, "frame_hash"); 
			}
        }

        private void WriteMetadataDecodedFrameHash()
        {
			int i = 0;
			int numPlanes = 0;
			this.hash_type = stream.Pick("hash_type", _original != null ? _original.hash_type : this.hash_type, _edited != null ? _edited.hash_type : _original != null ? _original.hash_type : this.hash_type);
			stream.WriteFixed(4, this.hash_type, "hash_type"); 
			this.per_plane = stream.Pick("per_plane", _original != null ? _original.per_plane : this.per_plane, _edited != null ? _edited.per_plane : _original != null ? _original.per_plane : this.per_plane);
			stream.WriteFixed(1, this.per_plane, "per_plane"); 
			this.has_grain = stream.Pick("has_grain", _original != null ? _original.has_grain : this.has_grain, _edited != null ? _edited.has_grain : _original != null ? _original.has_grain : this.has_grain);
			stream.WriteFixed(1, this.has_grain, "has_grain"); 
			this.is_monochrome = stream.Pick("is_monochrome", _original != null ? _original.is_monochrome : this.is_monochrome, _edited != null ? _edited.is_monochrome : _original != null ? _original.is_monochrome : this.is_monochrome);
			stream.WriteFixed(1, this.is_monochrome, "is_monochrome"); 
			this.reserved = stream.Pick("reserved", _original != null ? _original.reserved : this.reserved, _edited != null ? _edited.reserved : _original != null ? _original.reserved : this.reserved);
			stream.WriteFixed(1, this.reserved, "reserved"); 

			if ((per_plane != 0))
			{
				numPlanes = ((is_monochrome != 0) ? 1 : 3);

				for (i = 0; (i < numPlanes); i++)
				{
					this.plane_hash[i] = stream.Pick("plane_hash", _original != null ? _original.plane_hash[i] : this.plane_hash[i], _edited != null ? _edited.plane_hash[i] : _original != null ? _original.plane_hash[i] : this.plane_hash[i]);
					stream.WriteBytes(128, this.plane_hash[i], "plane_hash"); 
				}
			}
			else 
			{
				this.frame_hash = stream.Pick("frame_hash", _original != null ? _original.frame_hash : this.frame_hash, _edited != null ? _edited.frame_hash : _original != null ? _original.frame_hash : this.frame_hash);
				stream.WriteBytes(128, this.frame_hash, "frame_hash"); 
			}
        }

    /*
metadata_user_data_unregistered( metadataPayloadSize ) {
uuid_iso_iec_11578	f(128)
for( i = 16; i < metadataPayloadSize; i++ ) {	
user_data_payload_byte	f(8)
}	
}
    */
		private byte[] uuid_iso_iec_11578;
		public byte[] _UuidIsoIec11578 { get { return uuid_iso_iec_11578; } set { uuid_iso_iec_11578 = value; } }
		private int user_data_payload_byte;
		public int _UserDataPayloadByte { get { return user_data_payload_byte; } set { user_data_payload_byte = value; } }

        private void MetadataUserDataUnregistered(int metadataPayloadSize)
        {
			int i = 0;
			stream.ReadBytes(128, out this.uuid_iso_iec_11578, "uuid_iso_iec_11578"); 

			for (i = 16; (i < metadataPayloadSize); i++)
			{
				stream.ReadFixed(8, out this.user_data_payload_byte, "user_data_payload_byte"); 
			}
        }

        private void WriteMetadataUserDataUnregistered(int metadataPayloadSize)
        {
			int i = 0;
			this.uuid_iso_iec_11578 = stream.Pick("uuid_iso_iec_11578", _original != null ? _original.uuid_iso_iec_11578 : this.uuid_iso_iec_11578, _edited != null ? _edited.uuid_iso_iec_11578 : _original != null ? _original.uuid_iso_iec_11578 : this.uuid_iso_iec_11578);
			stream.WriteBytes(128, this.uuid_iso_iec_11578, "uuid_iso_iec_11578"); 

			for (i = 16; (i < metadataPayloadSize); i++)
			{
				this.user_data_payload_byte = stream.Pick("user_data_payload_byte", _original != null ? _original.user_data_payload_byte : this.user_data_payload_byte, _edited != null ? _edited.user_data_payload_byte : _original != null ? _original.user_data_payload_byte : this.user_data_payload_byte);
				stream.WriteFixed(8, this.user_data_payload_byte, "user_data_payload_byte"); 
			}
        }

    /*
frame_header( isFirst ) {
if ( isFirst ) {	
SeenFrameHeader = 1	
CountFrameHeaderForLevelConstraint = 1	
FrameSymbolCount = 0	
startBitPos = get_position()	
frame_header_info()	
NumFrameHeaderBits = get_position() - startBitPos	
FirstPictureInTU = 0	
if ( IsBridge ) {	
NumTiles = TileCols * TileRows	
tg_start = 0	
tg_end = NumTiles - 1	
tile_group_payload( 0 )	
} else if ( ShowExistingFrame ||	
TipFrameMode == TIP_FRAME_AS_OUTPUT ||	
bru_inactive ) {	
decode_frame_wrapup()	
SeenFrameHeader = 0	
CountFrameHeaderForLevelConstraint = 0	
} else {	
TileNum = 0	
}	
} else {	
CountFrameHeaderForLevelConstraint = 0	
frame_header_copy()	
}	
}
    */
		private int isFirst;
		public int _IsFirst { get { return isFirst; } set { isFirst = value; } }
		private int CountFrameHeaderForLevelConstraint;
		public int _CountFrameHeaderForLevelConstraint { get { return CountFrameHeaderForLevelConstraint; } set { CountFrameHeaderForLevelConstraint = value; } }
		private int FrameSymbolCount;
		public int _FrameSymbolCount { get { return FrameSymbolCount; } set { FrameSymbolCount = value; } }
		private int NumFrameHeaderBits;
		public int _NumFrameHeaderBits { get { return NumFrameHeaderBits; } set { NumFrameHeaderBits = value; } }
		private int NumTiles;
		public int _NumTiles { get { return NumTiles; } set { NumTiles = value; } }
		private int tg_start;
		public int _TgStart { get { return tg_start; } set { tg_start = value; } }
		private int tg_end;
		public int _TgEnd { get { return tg_end; } set { tg_end = value; } }
		private int TileNum;
		public int _TileNum { get { return TileNum; } set { TileNum = value; } }

        private void FrameHeader(int isFirst)
        {
			int startBitPos = 0;

			if ((isFirst != 0))
			{
				SeenFrameHeader = 1;
				CountFrameHeaderForLevelConstraint = 1;
				FrameSymbolCount = 0;
				startBitPos = get_position();
				FrameHeaderInfo(); 
				NumFrameHeaderBits = (get_position() - startBitPos);
				FirstPictureInTU = 0;

				if ((IsBridge != 0))
				{
					NumTiles = (TileCols * TileRows);
					tg_start = 0;
					tg_end = (NumTiles - 1);
					TileGroupPayload(0); 
				}
				else if ((((ShowExistingFrame != 0) || (TipFrameMode == TIP_FRAME_AS_OUTPUT)) || (bru_inactive != 0)))
				{
					decode_frame_wrapup(); 
					SeenFrameHeader = 0;
					CountFrameHeaderForLevelConstraint = 0;
				}
				else 
				{
					TileNum = 0;
				}
			}
			else 
			{
				CountFrameHeaderForLevelConstraint = 0;
				FrameHeaderCopy(); 
			}
        }

        private void WriteFrameHeader(int isFirst)
        {
			int startBitPos = 0;

			if ((isFirst != 0))
			{
				SeenFrameHeader = 1;
				CountFrameHeaderForLevelConstraint = 1;
				FrameSymbolCount = 0;
				startBitPos = get_position();
				WriteFrameHeaderInfo(); 
				NumFrameHeaderBits = (get_position() - startBitPos);
				FirstPictureInTU = 0;

				if ((IsBridge != 0))
				{
					NumTiles = (TileCols * TileRows);
					tg_start = 0;
					tg_end = (NumTiles - 1);
					WriteTileGroupPayload(0); 
				}
				else if ((((ShowExistingFrame != 0) || (TipFrameMode == TIP_FRAME_AS_OUTPUT)) || (bru_inactive != 0)))
				{
					decode_frame_wrapup(); 
					SeenFrameHeader = 0;
					CountFrameHeaderForLevelConstraint = 0;
				}
				else 
				{
					TileNum = 0;
				}
			}
			else 
			{
				CountFrameHeaderForLevelConstraint = 0;
				WriteFrameHeaderCopy(); 
			}
        }

    /*
frame_header_copy() {
for ( i = 0; i < NumFrameHeaderBits; i++ ) {	
header_bit[ i ]	f(1)
}	
}
    */
		private AomArray<int> header_bit = new AomArray<int>();
		public AomArray<int> _HeaderBit { get { return header_bit; } set { header_bit = value; } }

        private void FrameHeaderCopy()
        {
			int i = 0;

			for (i = 0; (i < NumFrameHeaderBits); i++)
			{
				stream.ReadFixed(1, out this.header_bit[i], "header_bit"); 
			}
        }

        private void WriteFrameHeaderCopy()
        {
			int i = 0;

			for (i = 0; (i < NumFrameHeaderBits); i++)
			{
				this.header_bit[i] = stream.Pick("header_bit", _original != null ? _original.header_bit[i] : this.header_bit[i], _edited != null ? _edited.header_bit[i] : _original != null ? _original.header_bit[i] : this.header_bit[i]);
				stream.WriteFixed(1, this.header_bit[i], "header_bit"); 
			}
        }

    /*
frame_header_info() {
keyFrame = obu_type == OBU_CLOSED_LOOP_KEY || obu_type == OBU_OPEN_LOOP_KEY	
IsRegular = ( obu_type == OBU_OPEN_LOOP_KEY ||	
obu_type == OBU_REGULAR_TILE_GROUP ||	
obu_type == OBU_REGULAR_TIP ||	
obu_type == OBU_REGULAR_SEF ||	
obu_type == OBU_SWITCH ||	
obu_type == OBU_RAS_FRAME ||	
obu_type == OBU_BRIDGE_FRAME )	
for ( i = 0; i < NUM_CUSTOM_QMS; i++ ) {	
QmSeen[ i ] = 0	
}	
startCVS = obu_type == OBU_CLOSED_LOOP_KEY && FirstPictureInTU	
if ( startCVS ) {	
OlkEncountered = 0	
for( i = 0; i < MAX_NUM_MLAYERS; i++ ) {	
OlkRefresh[ i ] = 0	
}	
flush_implicit_output_frames( 0 )	
}	
if ( OlkEncountered && IsRegular && FirstPictureInTU ) {	
flush_implicit_output_frames( 1 )	
OlkEncountered = 0	
allowedFrames = 0	
for ( i = 0; i < MAX_NUM_MLAYERS; i++ ) {	
allowedFrames |= OlkRefresh[ i ]	
OlkRefresh[ i ] = 0	
}	
for ( i = 0; i < NUM_REF_FRAMES; i++ ) {	
if ( ( allowedFrames & (1 << i) ) == 0 && RefLongTermId[ i ] == -1 )	
RefValid[ i ] = 0	
}	
}	
IsBridge = obu_type == OBU_BRIDGE_FRAME	
if ( IsBridge ) {	
cur_mfh_id = 0	
} else {	
cur_mfh_id	uvlc()
}	
if ( cur_mfh_id == 0 ) {	
seq_header_id_in_frame_header	uvlc()
load_sequence_header( seq_header_id_in_frame_header )	
mfh_deblocking_filter_update[ cur_mfh_id ] = 0	
} else {	
load_sequence_header( MfhSeqHeaderId[ cur_mfh_id ] )	
}	
if ( keyFrame ) {	
if ( seq_lcr_id != 0 ) {	
activate_layer_configuration_record( seq_lcr_id )	
}	
}	
if ( cur_mfh_id == 0 || !mfh_frame_size_present_flag[ cur_mfh_id ] ) {	
mfh_frame_width_minus_1[ cur_mfh_id ] = max_frame_width_minus_1	
mfh_frame_height_minus_1[ cur_mfh_id ] = max_frame_height_minus_1	
}	
if ( keyFrame && FirstPictureInTU ) {	
reset_qm()	
}	
if ( IsBridge ) {	
n = CeilLog2(NumRefFrames)	
bridge_frame_ref_idx	f(n)
}	
allFrames = (1 << NumRefFrames) - 1	
use_bru = 0	
bru_inactive = 0	
if ( single_picture_header_flag ) {	
ShowExistingFrame = 0	
FrameType = KEY_FRAME	
FrameIsIntra = 1	
immediate_output_frame = 1	
implicit_output_frame = 0	
} else {	
ShowExistingFrame = is_sef()	
if ( ShowExistingFrame == 1 ) {	
n = CeilLog2(NumRefFrames)	
frame_to_show_map_idx	f(n)
derive_sef_order_hint	f(1)
if ( derive_sef_order_hint == 0 ) {	
sef_order_hint	f(OrderHintBits)
OrderHintLsbs = sef_order_hint	
OrderHint = get_disp_order_hint()	
} else {	
OrderHint = RefOrderHint[ frame_to_show_map_idx ]	
}	
if ( IsRegular && OlkEncountered && !FirstPictureInTU ) {	
OlkTUOrderHint = derive_sef_order_hint ?	
RefOrderHint[ frame_to_show_map_idx ] :	
OrderHint	
}	
refresh_frame_flags = 0	
FrameType = RefFrameType[ frame_to_show_map_idx ]	
immediate_output_frame = 1	
film_grain_config()	
if ( derive_sef_order_hint ) {	
save_grain_params( frame_to_show_map_idx )	
}	
TipFrameMode = TIP_FRAME_DISABLED	
return	
}	
if ( IsBridge ) {	
FrameType = INTER_FRAME	
} else if ( obu_type == OBU_SWITCH || obu_type == OBU_RAS_FRAME ) {	
restricted_prediction_switch	f(1)
FrameType = SWITCH_FRAME	
} else if ( is_tip_frame() ) {	
FrameType = INTER_FRAME	
} else if ( obu_type == OBU_CLOSED_LOOP_KEY ||	
obu_type == OBU_OPEN_LOOP_KEY ) {	
FrameType = KEY_FRAME	
} else {	
frame_is_inter	f(1)
FrameType = frame_is_inter ? INTER_FRAME : INTRA_ONLY_FRAME	
}	
LongTermId = -1	
if ( FrameType == KEY_FRAME ) {	
long_term_id_plus_1	f(long_term_frame_id_bits)
LongTermId = long_term_id_plus_1 - 1	
}	
num_key_ref_frames = 0	
if ( (obu_type == OBU_RAS_FRAME || obu_type == OBU_OPEN_LOOP_KEY) &&	
long_term_frame_id_bits != 0) {	
num_key_ref_frames	f(3)
for ( i = 0; i < num_key_ref_frames; i++ ) {	
ref_long_term_id[ i ]	f(long_term_frame_id_bits)
}	
}	
if ( FrameType == SWITCH_FRAME && restricted_prediction_switch ) {	
for (i = 0; i < NUM_REF_FRAMES; i++) {	
if ( MLayerPresenceMap[RefMLayerId[i]][obu_mlayer_id] ) {	
if ( is_frame_eligible_for_output( i ) ) {	
output_frame_buffers( i )	
}	
RefOrderHint[ i ] = RESTRICTED_OH	
}	
}	
}	
if ( obu_type == OBU_RAS_FRAME ||	
(obu_type == OBU_SWITCH && restricted_prediction_switch) ) {	
reset_qm()	
}	
FrameIsIntra = (FrameType == INTRA_ONLY_FRAME ||	
FrameType == KEY_FRAME)	
if ( IsBridge || obu_type == OBU_OPEN_LOOP_KEY ) {	
immediate_output_frame = 0	
} else {	
immediate_output_frame	f(1)
}	
if ( IsBridge || immediate_output_frame || monotonic_output_order_flag ) {	
implicit_output_frame = 0	
} else {	
implicit_output_frame	f(1)
}	
}	
if ( use_256x256_superblock ) {	
SbSize = FrameIsIntra ? BLOCK_128X128 : BLOCK_256X256	
} else if ( use_128x128_superblock ) {	
SbSize = BLOCK_128X128	
} else {	
SbSize = BLOCK_64X64	
}	
if ( FrameType == KEY_FRAME && immediate_output_frame ) {	
for ( i = 0; i < REFS_PER_FRAME; i++ ) {	
OrderHints[ i ] = 0	
}	
}	
disable_cross_frame_cdf_init = 0	
if ( IsBridge ) {	
primary_ref_frame = PRIMARY_REF_NONE	
OrderHintLsbs = RefOrderHintLsbs[ bridge_frame_ref_idx ]	
OrderHint = RefOrderHint[ bridge_frame_ref_idx ]	
} else {	
if ( FrameType == SWITCH_FRAME ) {	
frame_size_override_flag = 1	
} else if ( single_picture_header_flag ) {	
frame_size_override_flag = 0	
} else {	
frame_size_override_flag	f(1)
}	
order_hint	f(OrderHintBits)
OrderHintLsbs = order_hint	
OrderHint = get_disp_order_hint()	
if ( FrameIsIntra || FrameType == SWITCH_FRAME ) {	
primary_ref_frame = PRIMARY_REF_NONE	
} else {	
signal_primary_ref_frame	f(1)
if ( !is_tip_frame() ) {	
disable_cross_frame_cdf_init	f(1)
}	
if ( signal_primary_ref_frame ) {	
primary_ref_frame	f(3)
} else {	
primary_ref_frame = PRIMARY_REF_CHOOSE	
}	
}	
}	
FrameMvPrecision = MV_PRECISION_ONE_PEL	
MvPrecision = FrameMvPrecision	
allow_high_precision_mv = 0	
use_ref_frame_mvs = 0	
allow_intrabc = 0	
allow_global_intrabc = 0	
allow_local_intrabc = 0	
allow_high_precision_mv = 0	
allow_df_sub_pu = 0	
if ( IsBridge ) {	
bridge_frame_overwrite_flag	f(1)
}	
if ( FrameType == KEY_FRAME ) {	
if ( obu_type == OBU_CLOSED_LOOP_KEY && max_mlayer_id == 0 ) {	
refresh_frame_flags = allFrames	
} else if ( enable_short_refresh_frame_flags ) {	
n = CeilLog2(NumRefFrames)	
frame_to_refresh	f(n)
refresh_frame_flags = 1 << frame_to_refresh	
} else {	
refresh_frame_flags	f(NumRefFrames)
}	
if ( obu_type == OBU_CLOSED_LOOP_KEY && FirstPictureInTU ) {	
for ( i = 0; i < NumRefFrames; i++ ) {	
RefValid[i] = 0	
}	
}	
if ( obu_type == OBU_CLOSED_LOOP_KEY ) {	
OlkEncountered = 0	
for( i = 0; i < MAX_NUM_MLAYERS; i++ ) {	
OlkRefresh[ i ] = 0	
}	
}	
if ( obu_type == OBU_OPEN_LOOP_KEY ) {	
OlkEncountered = 1	
OlkRefresh[ obu_mlayer_id ] = refresh_frame_flags	
if ( implicit_output_frame ) {	
OlkTUOrderHint = OrderHint	
}	
}	
} else if ( IsBridge && !bridge_frame_overwrite_flag ) {	
refresh_frame_flags = 1 << bridge_frame_ref_idx	
} else if ( obu_type == OBU_RAS_FRAME && max_mlayer_id == 0 ) {	
refresh_frame_flags = 0	
for ( i = 0; i < NumRefFrames; i++ ) {	
if ( !RefValid[i] || !long_term_id_in_use( RefLongTermId[i] ) ) {	
refresh_frame_flags |= (1 << i)	
}	
}	
} else if ( FrameType == SWITCH_FRAME ) {	
refresh_frame_flags	f(NumRefFrames)
} else if ( enable_short_refresh_frame_flags &&	
FrameType != SWITCH_FRAME &&	
FrameType != KEY_FRAME ) {	
has_refresh_frame_flags	f(1)
if ( has_refresh_frame_flags ) {	
n = CeilLog2(NumRefFrames)	
frame_to_refresh	f(n)
refresh_frame_flags = 1 << frame_to_refresh	
} else {	
refresh_frame_flags = 0	
}	
} else {	
refresh_frame_flags	f(NumRefFrames)
}	
AllowedFrames = -1	
if ( IsRegular && OlkEncountered && !FirstPictureInTU ) {	
AllowedFrames = 0	
for ( i = 0; i < MAX_NUM_MLAYERS; i++ ) {	
AllowedFrames |= OlkRefresh[ i ]	
}	
OlkRefresh[ obu_mlayer_id ] |= refresh_frame_flags	
if ( immediate_output_frame || implicit_output_frame ) {	
OlkTUOrderHint = OrderHint	
}	
}	
if ( FrameIsIntra ) {	
frame_size()	
screen_content_params()	
intrabc_params()	
NumTotalRefs = 0	
TipFrameMode = TIP_FRAME_DISABLED	
} else {	
if ( FrameType == SWITCH_FRAME || IsBridge ) {	
explicitRefFrameMap = 1	
} else if ( explicit_ref_frame_map ) {	
frame_explicit_ref_frame_map	f(1)
explicitRefFrameMap = frame_explicit_ref_frame_map	
} else {	
explicitRefFrameMap = 0	
}	
if ( IsBridge ) {	
NumTotalRefs = 1	
} else if ( explicitRefFrameMap ) {	
num_total_refs	f(3)
NumTotalRefs = num_total_refs	
} else {	
get_ref_frames( 0 )	
}	
for ( i = 0; i < NumTotalRefs; i++ ) {	
if ( IsBridge ) {	
ref_frame_idx[ i ] = bridge_frame_ref_idx	
} else if ( explicitRefFrameMap ) {	
n = CeilLog2(NumRefFrames)	
ref_frame_idx[ i ]	f(n)
}	
}	
if ( IsBridge ) {	
frame_size_with_bridge()	
} else if ( frame_size_override_flag && FrameType != SWITCH_FRAME ) {	
frame_size_with_refs()	
} else {	
frame_size()	
}	
if ( !explicitRefFrameMap ) {	
get_ref_frames( 1 )	
}	
NumSameRefCompound = Min(num_same_ref_compound, NumTotalRefs)	
if ( enable_bru && FrameType == INTER_FRAME && !is_tip_frame() &&	
!IsBridge ) {	
use_bru	f(1)
if ( use_bru ) {	
n = CeilLog2(NumTotalRefs)	
bru_ref	f(n)
bru_inactive	f(1)
}	
}	
if ( explicitRefFrameMap ) {	
for ( i = 0; i < NumTotalRefs; i++ ) {	
ScoresDistance[ i ] = get_relative_dist( OrderHint,	
RefOrderHint[ ref_frame_idx[ i ] ] )	
}	
}	
get_past_future_cur_ref_lists()	
if ( FrameType == SWITCH_FRAME || !enable_ref_frame_mvs ||	
IsBridge || bru_inactive ) {	
use_ref_frame_mvs = 0	
} else {	
use_ref_frame_mvs	f(1)
}	
if ( use_ref_frame_mvs && NumTotalRefs > 1 && SbSize != BLOCK_64X64 ) {	
tmvp_sample_step_minus_1	f(1)
ProjStep = tmvp_sample_step_minus_1 + 1	
} else {	
ProjStep = 1	
}	
for ( i = 0; i < NumTotalRefs; i++ ) {	
FrameDistance[ i ] = get_relative_dist( OrderHint,	
RefOrderHint[ ref_frame_idx[ i ] ] )	
if ( RefOrderHint[ ref_frame_idx[ i ] ] == RESTRICTED_OH ) {	
FrameDistance[ i ] = -FrameDistance[ i ]	
}	
}	
for ( i = 0; i < NumTotalRefs; i++ ) {	
refFrame = i	
hint = RefOrderHint[ ref_frame_idx[ i ] ]	
OrderHints[ refFrame ] = hint	
}	
if ( enable_tip &&	
(use_ref_frame_mvs && NumTotalRefs >= 2) &&	
!bru_inactive ) {	
TipInterpFilter = EIGHTTAP_SHARP	
TipGlobalMv[ 0 ] = 0	
TipGlobalMv[ 1 ] = 0	
if ( EnableTipOutput && is_tip_frame() ) {	
TipFrameMode = TIP_FRAME_AS_OUTPUT	
} else {	
tip_frame_mode	f(1)
TipFrameMode = tip_frame_mode	
}	
frame_opfl_refine_type()	
if ( TipFrameMode != TIP_FRAME_DISABLED &&	
enable_tip_hole_fill ) {	
allow_tip_hole_fill	f(1)
} else {	
allow_tip_hole_fill = 0	
}	
usesEqualWeight = enable_tip_refinemv &&	
NumFutureRefs > 0 && NumPastRefs > 0 &&	
( opfl_refine_type != REFINE_NONE || enable_refinemv )	
if ( TipFrameMode == TIP_FRAME_DISABLED || usesEqualWeight ) {	
tip_global_wtd_index = 0	
} else {	
tip_global_wtd_index	f(3)
}	
if ( TipFrameMode == TIP_FRAME_AS_OUTPUT ) {	
tip_mv_zero	f(1)
if ( !tip_mv_zero ) {	
tip_mv_row	f(4)
tip_mv_col	f(4)
if ( tip_mv_row != 0 ) {	
tip_mv_row_sign	f(1)
TipGlobalMv[ 0 ] = tip_mv_row_sign ?	
-tip_mv_row : tip_mv_row	
}	
if ( tip_mv_col != 0 ) {	
tip_mv_col_sign	f(1)
TipGlobalMv[ 1 ] = tip_mv_col_sign ?	
-tip_mv_col : tip_mv_col	
}	
}	
tip_sharp	f(1)
if ( tip_sharp ) {	
TipInterpFilter = EIGHTTAP_SHARP	
} else {	
tip_regular	f(1)
TipInterpFilter = tip_regular ? EIGHTTAP: EIGHTTAP_SMOOTH	
}	
}	
} else {	
TipFrameMode = TIP_FRAME_DISABLED	
if ( !bru_inactive && !IsBridge ) {	
frame_opfl_refine_type()	
}	
}	
if ( TipFrameMode != TIP_FRAME_AS_OUTPUT && !bru_inactive &&	
!IsBridge ) {	
screen_content_params()	
intrabc_params()	
max_drl_bits_minus_1 = seq_max_drl_bits_minus_1	
if ( allow_frame_max_drl_bits ) {	
change_drl	f(1)
if ( change_drl ) {	
n = MAX_REF_MV_STACK_SIZE - 2	
max_drl_bits_minus_1	ns(n)
if ( max_drl_bits_minus_1 >= seq_max_drl_bits_minus_1 ) {	
max_drl_bits_minus_1 += 1	
}	
}	
}	
if ( force_integer_mv ) {	
FrameMvPrecision = MV_PRECISION_ONE_PEL	
UsePerBlockMvPrecision = 0	
} else {	
use_qtr_precision_mv	f(1)
if ( use_qtr_precision_mv ) {	
FrameMvPrecision = MV_PRECISION_QUARTER_PEL	
} else {	
allow_high_precision_mv	f(1)
FrameMvPrecision = allow_high_precision_mv ?	
MV_PRECISION_EIGHTH_PEL : MV_PRECISION_HALF_PEL	
}	
UsePerBlockMvPrecision = enable_flex_mvres	
}	
MvPrecision = FrameMvPrecision	
read_interpolation_filter()	
for ( mode = INTERINTRA; mode < MOTION_MODES; mode++ ) {	
if ( !seq_frame_motion_modes_present_flag ) {	
frame_enabled_motion_modes[ mode ] =	
seq_enabled_motion_modes[ mode ]	
} else if ( seq_enabled_motion_modes[ mode ] ) {	
frame_enabled_motion_modes[ mode ]	f(1)
} else {	
frame_enabled_motion_modes[ mode ] = 0	
}	
}	
}	
}	
if ( TipFrameMode == TIP_FRAME_AS_OUTPUT ) {	
if ( enable_tip_explicit_qp ) {	
quantization_params()	
}	
if ( enable_df_sub_pu ) {	
allow_df_sub_pu	f(1)
}	
if ( allow_df_sub_pu ) {	
apply_deblocking_filter_tip	f(1)
} else {	
apply_deblocking_filter_tip = 0	
}	
}	
if ( TipFrameMode == TIP_FRAME_AS_OUTPUT || bru_inactive || IsBridge ) {	
for ( i = 0 ; i < 3; i++ ) {	
frame_filters_on[ i ] = 0	
}	
if ( bru_inactive || IsBridge ) {	
if ( IsBridge ) {	
tile_info()	
refIdx = bridge_frame_ref_idx	
} else {	
refIdx = ref_frame_idx[ bru_ref ]	
}	
base_q_idx = RefBaseQIdx[ refIdx ]	
DeltaQUAc = RefDeltaQUAc[ refIdx ]	
DeltaQVAc = RefDeltaQVAc[ refIdx ]	
set_primary_ref_frame_and_ctx( 0 )	
} else if ( apply_deblocking_filter_tip ) {	
tile_info()	
}	
film_grain_config()	
if ( bru_inactive || IsBridge ) {	
set_primary_ref_frame_and_ctx( 1 )	
}	
for (row = 0; row < MiRows; row++) {	
for (col = 0; col < MiCols; col++) {	
SegmentIds[ row ][ col ] = 0	
}	
}	
for ( refc = 0; refc < REFS_PER_FRAME; refc++ ) {	
for ( i = 0; i < 6; i++ ) {	
gm_params[ refc ][ i ] = Default_Warp_Params[ i ]	
}	
}	
} else {	
disable_cdf_update	f(1)
}	
if ( bru_inactive || IsBridge ) {	
apply_deblocking_filter[ 0 ] = 0	
apply_deblocking_filter[ 1 ] = 0	
cdef_frame_enable = 0	
for ( plane = 0; plane < NumPlanes; plane++ ) {	
ccso_planes[ plane ] = 0	
}	
FrameRestorationType[ 0 ] = RESTORE_NONE	
FrameRestorationType[ 1 ] = RESTORE_NONE	
FrameRestorationType[ 2 ] = RESTORE_NONE	
gdf_frame_enable = 0	
segmentation_enabled = 0	
for ( i = 0; i < MAX_SEGMENTS; i++ ) {	
for ( j = 0; j < SEG_LVL_MAX; j++ ) {	
FeatureEnabled[ i ][ j ] = 0	
FeatureData[ i ][ j ] = 0	
}	
}	
if ( primary_ref_frame == PRIMARY_REF_NONE ||	
disable_cross_frame_cdf_init) {	
init_coeff_cdfs()	
}	
return	
}	
if ( use_ref_frame_mvs == 1 ) {	
HasBothRefs = ClosestFuture != NONE && ClosestPast != NONE	
motion_field_estimation()	
if ( TipFrameMode == TIP_FRAME_AS_OUTPUT ) {	
if ( !enable_tip_explicit_qp ) {	
slot0 = ref_frame_idx[ ClosestPast ]	
slot1 = ref_frame_idx[ ClosestFuture ]	
base_q_idx = Round2(RefBaseQIdx[slot0] + RefBaseQIdx[slot1], 1)	
DeltaQUAc = Round2(RefDeltaQUAc[slot0] + RefDeltaQUAc[slot1], 1)	
DeltaQVAc = Round2(RefDeltaQVAc[slot0] + RefDeltaQVAc[slot1], 1)	
}	
set_primary_ref_frame_and_ctx( 1 )	
for (i = 0; i < MAX_SEGMENTS; i++) {	
for ( j = 0; j < SEG_LVL_MAX; j++ ) {	
FeatureData[ i ][ j ] = 0	
FeatureEnabled[ i ][ j ] = 0	
}	
}	
for (row = 0; row < MiRows; row++) {	
for (col = 0; col < MiCols; col++) {	
PrevSegmentIds[ row ][ col ] = 0	
}	
}	
for ( plane = 0; plane < 3; plane++ ) {	
ccso_planes[ plane ] = 0	
}	
if ( primary_ref_frame == PRIMARY_REF_NONE ||	
disable_cross_frame_cdf_init ) {	
init_coeff_cdfs()	
}	
}	
if ( TipFrameMode == TIP_FRAME_DISABLED ) {	
fill_tpl_mvs_sample_gap()	
}	
}	
if ( TipFrameMode != TIP_FRAME_DISABLED ) {	
setup_tip_motion_field()	
}	
if ( TipFrameMode == TIP_FRAME_AS_OUTPUT ) {	
return	
}	
tile_info()	
quantization_params()	
set_primary_ref_frame_and_ctx( 1 )	
segmentation_params()	
setup_qm_params()	
delta_q_params()	
if ( primary_ref_frame == PRIMARY_REF_NONE ||	
disable_cross_frame_cdf_init ) {	
init_coeff_cdfs()	
}	
if ( DerivedPrimaryRefFrame != PRIMARY_REF_NONE ) {	
load_previous_segment_ids()	
}	
CodedLossless = 1	
HasLosslessSegment = 0	
for ( segmentId = 0; segmentId < MaxSegments; segmentId++ ) {	
qindex = get_qindex( 1, segmentId )	
LosslessArray[ segmentId ] = qindex == 0 && delta_q_present == 0 &&	
DeltaQYDc + BaseYDcDeltaQ <= 0 &&	
DeltaQUDc + BaseUVDcDeltaQ <= 0 &&	
DeltaQVDc + BaseUVDcDeltaQ <= 0 &&	
DeltaQUAc + BaseUVAcDeltaQ <= 0 &&	
DeltaQVAc + BaseUVAcDeltaQ <= 0	
if ( LosslessArray[ segmentId ] ) {	
HasLosslessSegment = 1	
} else {	
CodedLossless = 0	
}	
if ( using_qmatrix ) {	
if ( LosslessArray[ segmentId ] ) {	
SegQMLevel[ 0 ][ segmentId ] = 15	
SegQMLevel[ 1 ][ segmentId ] = 15	
SegQMLevel[ 2 ][ segmentId ] = 15	
} else {	
qmNum = pic_qm_num_minus_1 + 1	
qmIndexBits = CeilLog2( qmNum )	
qm_index	f(qmIndexBits)
SegQMLevel[ 0 ][ segmentId ] = qm_y[ qm_index ]	
SegQMLevel[ 1 ][ segmentId ] = qm_u[ qm_index ]	
SegQMLevel[ 2 ][ segmentId ] = qm_v[ qm_index ]	
}	
}	
}	
if ( CodedLossless ) {	
allow_tcq = 0	
} else if ( choose_tcq_per_frame ) {	
allow_tcq	f(1)
} else {	
allow_tcq = enable_tcq	
}	
if ( CodedLossless || !enable_parity_hiding || allow_tcq ) {	
allow_parity_hiding = 0	
} else {	
allow_parity_hiding	f(1)
}	
deblocking_filter_params()	
gdf_params()	
cdef_params()	
lr_params()	
ccso_params()	
read_tx_mode()	
frame_reference_mode()	
skip_mode_params()	
if (!FrameIsIntra && enable_bawp) {	
allow_bawp	f(1)
} else {	
allow_bawp = 0	
}	
if ( !FrameIsIntra && frame_enabled_motion_modes[ DELTAWARP ] ) {	
allow_warpmv_mode	f(1)
} else {	
allow_warpmv_mode = 0	
}	
reduced_tx_set	f(2)
global_motion_params()	
film_grain_config()	
}
    */
		private int IsRegular;
		public int _IsRegular { get { return IsRegular; } set { IsRegular = value; } }
		private int OlkEncountered;
		public int _OlkEncountered { get { return OlkEncountered; } set { OlkEncountered = value; } }
		private AomArray<int> OlkRefresh = new AomArray<int>();
		public AomArray<int> _OlkRefresh { get { return OlkRefresh; } set { OlkRefresh = value; } }
		private AomArray<int> RefValid = new AomArray<int>();
		public AomArray<int> _RefValid { get { return RefValid; } set { RefValid = value; } }
		private int IsBridge;
		public int _IsBridge { get { return IsBridge; } set { IsBridge = value; } }
		private int cur_mfh_id;
		public int _CurMfhId { get { return cur_mfh_id; } set { cur_mfh_id = value; } }
		private int seq_header_id_in_frame_header;
		public int _SeqHeaderIdInFrameHeader { get { return seq_header_id_in_frame_header; } set { seq_header_id_in_frame_header = value; } }
		private int bridge_frame_ref_idx;
		public int _BridgeFrameRefIdx { get { return bridge_frame_ref_idx; } set { bridge_frame_ref_idx = value; } }
		private int use_bru;
		public int _UseBru { get { return use_bru; } set { use_bru = value; } }
		private int bru_inactive;
		public int _BruInactive { get { return bru_inactive; } set { bru_inactive = value; } }
		private int ShowExistingFrame;
		public int _ShowExistingFrame { get { return ShowExistingFrame; } set { ShowExistingFrame = value; } }
		private int FrameType;
		public int _FrameType { get { return FrameType; } set { FrameType = value; } }
		private int FrameIsIntra;
		public int _FrameIsIntra { get { return FrameIsIntra; } set { FrameIsIntra = value; } }
		private int immediate_output_frame;
		public int _ImmediateOutputFrame { get { return immediate_output_frame; } set { immediate_output_frame = value; } }
		private int implicit_output_frame;
		public int _ImplicitOutputFrame { get { return implicit_output_frame; } set { implicit_output_frame = value; } }
		private int frame_to_show_map_idx;
		public int _FrameToShowMapIdx { get { return frame_to_show_map_idx; } set { frame_to_show_map_idx = value; } }
		private int derive_sef_order_hint;
		public int _DeriveSefOrderHint { get { return derive_sef_order_hint; } set { derive_sef_order_hint = value; } }
		private int sef_order_hint;
		public int _SefOrderHint { get { return sef_order_hint; } set { sef_order_hint = value; } }
		private int OrderHintLsbs;
		public int _OrderHintLsbs { get { return OrderHintLsbs; } set { OrderHintLsbs = value; } }
		private int OrderHint;
		public int _OrderHint { get { return OrderHint; } set { OrderHint = value; } }
		private int OlkTUOrderHint;
		public int _OlkTUOrderHint { get { return OlkTUOrderHint; } set { OlkTUOrderHint = value; } }
		private int refresh_frame_flags;
		public int _RefreshFrameFlags { get { return refresh_frame_flags; } set { refresh_frame_flags = value; } }
		private int TipFrameMode;
		public int _TipFrameMode { get { return TipFrameMode; } set { TipFrameMode = value; } }
		private int restricted_prediction_switch;
		public int _RestrictedPredictionSwitch { get { return restricted_prediction_switch; } set { restricted_prediction_switch = value; } }
		private int frame_is_inter;
		public int _FrameIsInter { get { return frame_is_inter; } set { frame_is_inter = value; } }
		private int LongTermId;
		public int _LongTermId { get { return LongTermId; } set { LongTermId = value; } }
		private int long_term_id_plus_1;
		public int _LongTermIdPlus1 { get { return long_term_id_plus_1; } set { long_term_id_plus_1 = value; } }
		private int num_key_ref_frames;
		public int _NumKeyRefFrames { get { return num_key_ref_frames; } set { num_key_ref_frames = value; } }
		private AomArray<int> ref_long_term_id = new AomArray<int>();
		public AomArray<int> _RefLongTermId { get { return ref_long_term_id; } set { ref_long_term_id = value; } }
		private AomArray<int> RefOrderHint = new AomArray<int>();
		public AomArray<int> _RefOrderHint { get { return RefOrderHint; } set { RefOrderHint = value; } }
		private int SbSize;
		public int _SbSize { get { return SbSize; } set { SbSize = value; } }
		private AomArray<int> OrderHints = new AomArray<int>();
		public AomArray<int> _OrderHints { get { return OrderHints; } set { OrderHints = value; } }
		private int disable_cross_frame_cdf_init;
		public int _DisableCrossFrameCdfInit { get { return disable_cross_frame_cdf_init; } set { disable_cross_frame_cdf_init = value; } }
		private int primary_ref_frame;
		public int _PrimaryRefFrame { get { return primary_ref_frame; } set { primary_ref_frame = value; } }
		private int frame_size_override_flag;
		public int _FrameSizeOverrideFlag { get { return frame_size_override_flag; } set { frame_size_override_flag = value; } }
		private int order_hint;
		public int __OrderHint { get { return order_hint; } set { order_hint = value; } }
		private int signal_primary_ref_frame;
		public int _SignalPrimaryRefFrame { get { return signal_primary_ref_frame; } set { signal_primary_ref_frame = value; } }
		private int FrameMvPrecision;
		public int _FrameMvPrecision { get { return FrameMvPrecision; } set { FrameMvPrecision = value; } }
		private int MvPrecision;
		public int _MvPrecision { get { return MvPrecision; } set { MvPrecision = value; } }
		private int allow_high_precision_mv;
		public int _AllowHighPrecisionMv { get { return allow_high_precision_mv; } set { allow_high_precision_mv = value; } }
		private int use_ref_frame_mvs;
		public int _UseRefFrameMvs { get { return use_ref_frame_mvs; } set { use_ref_frame_mvs = value; } }
		private int allow_intrabc;
		public int _AllowIntrabc { get { return allow_intrabc; } set { allow_intrabc = value; } }
		private int allow_global_intrabc;
		public int _AllowGlobalIntrabc { get { return allow_global_intrabc; } set { allow_global_intrabc = value; } }
		private int allow_local_intrabc;
		public int _AllowLocalIntrabc { get { return allow_local_intrabc; } set { allow_local_intrabc = value; } }
		private int allow_df_sub_pu;
		public int _AllowDfSubPu { get { return allow_df_sub_pu; } set { allow_df_sub_pu = value; } }
		private int bridge_frame_overwrite_flag;
		public int _BridgeFrameOverwriteFlag { get { return bridge_frame_overwrite_flag; } set { bridge_frame_overwrite_flag = value; } }
		private int frame_to_refresh;
		public int _FrameToRefresh { get { return frame_to_refresh; } set { frame_to_refresh = value; } }
		private int has_refresh_frame_flags;
		public int _HasRefreshFrameFlags { get { return has_refresh_frame_flags; } set { has_refresh_frame_flags = value; } }
		private int AllowedFrames;
		public int _AllowedFrames { get { return AllowedFrames; } set { AllowedFrames = value; } }
		private int NumTotalRefs;
		public int _NumTotalRefs { get { return NumTotalRefs; } set { NumTotalRefs = value; } }
		private int frame_explicit_ref_frame_map;
		public int _FrameExplicitRefFrameMap { get { return frame_explicit_ref_frame_map; } set { frame_explicit_ref_frame_map = value; } }
		private int num_total_refs;
		public int __NumTotalRefs { get { return num_total_refs; } set { num_total_refs = value; } }
		private AomArray<int> ref_frame_idx = new AomArray<int>();
		public AomArray<int> _RefFrameIdx { get { return ref_frame_idx; } set { ref_frame_idx = value; } }
		private int NumSameRefCompound;
		public int __NumSameRefCompound { get { return NumSameRefCompound; } set { NumSameRefCompound = value; } }
		private int bru_ref;
		public int _BruRef { get { return bru_ref; } set { bru_ref = value; } }
		private AomArray<int> ScoresDistance = new AomArray<int>();
		public AomArray<int> _ScoresDistance { get { return ScoresDistance; } set { ScoresDistance = value; } }
		private int tmvp_sample_step_minus_1;
		public int _TmvpSampleStepMinus1 { get { return tmvp_sample_step_minus_1; } set { tmvp_sample_step_minus_1 = value; } }
		private int ProjStep;
		public int _ProjStep { get { return ProjStep; } set { ProjStep = value; } }
		private AomArray<int> FrameDistance = new AomArray<int>();
		public AomArray<int> _FrameDistance { get { return FrameDistance; } set { FrameDistance = value; } }
		private int TipInterpFilter;
		public int _TipInterpFilter { get { return TipInterpFilter; } set { TipInterpFilter = value; } }
		private AomArray<int> TipGlobalMv = new AomArray<int>();
		public AomArray<int> _TipGlobalMv { get { return TipGlobalMv; } set { TipGlobalMv = value; } }
		private int tip_frame_mode;
		public int __TipFrameMode { get { return tip_frame_mode; } set { tip_frame_mode = value; } }
		private int allow_tip_hole_fill;
		public int _AllowTipHoleFill { get { return allow_tip_hole_fill; } set { allow_tip_hole_fill = value; } }
		private int tip_global_wtd_index;
		public int _TipGlobalWtdIndex { get { return tip_global_wtd_index; } set { tip_global_wtd_index = value; } }
		private int tip_mv_zero;
		public int _TipMvZero { get { return tip_mv_zero; } set { tip_mv_zero = value; } }
		private int tip_mv_row;
		public int _TipMvRow { get { return tip_mv_row; } set { tip_mv_row = value; } }
		private int tip_mv_col;
		public int _TipMvCol { get { return tip_mv_col; } set { tip_mv_col = value; } }
		private int tip_mv_row_sign;
		public int _TipMvRowSign { get { return tip_mv_row_sign; } set { tip_mv_row_sign = value; } }
		private int tip_mv_col_sign;
		public int _TipMvColSign { get { return tip_mv_col_sign; } set { tip_mv_col_sign = value; } }
		private int tip_sharp;
		public int _TipSharp { get { return tip_sharp; } set { tip_sharp = value; } }
		private int tip_regular;
		public int _TipRegular { get { return tip_regular; } set { tip_regular = value; } }
		private int max_drl_bits_minus_1;
		public int _MaxDrlBitsMinus1 { get { return max_drl_bits_minus_1; } set { max_drl_bits_minus_1 = value; } }
		private int change_drl;
		public int _ChangeDrl { get { return change_drl; } set { change_drl = value; } }
		private int UsePerBlockMvPrecision;
		public int _UsePerBlockMvPrecision { get { return UsePerBlockMvPrecision; } set { UsePerBlockMvPrecision = value; } }
		private int use_qtr_precision_mv;
		public int _UseQtrPrecisionMv { get { return use_qtr_precision_mv; } set { use_qtr_precision_mv = value; } }
		private AomArray<int> frame_enabled_motion_modes = new AomArray<int>();
		public AomArray<int> _FrameEnabledMotionModes { get { return frame_enabled_motion_modes; } set { frame_enabled_motion_modes = value; } }
		private int apply_deblocking_filter_tip;
		public int _ApplyDeblockingFilterTip { get { return apply_deblocking_filter_tip; } set { apply_deblocking_filter_tip = value; } }
		private AomArray<int> frame_filters_on = new AomArray<int>();
		public AomArray<int> _FrameFiltersOn { get { return frame_filters_on; } set { frame_filters_on = value; } }
		private int base_q_idx;
		public int _BaseqIdx { get { return base_q_idx; } set { base_q_idx = value; } }
		private int DeltaQUAc;
		public int _DeltaQUAc { get { return DeltaQUAc; } set { DeltaQUAc = value; } }
		private int DeltaQVAc;
		public int _DeltaQVAc { get { return DeltaQVAc; } set { DeltaQVAc = value; } }
		private AomArray<AomArray<int>> SegmentIds = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _SegmentIds { get { return SegmentIds; } set { SegmentIds = value; } }
		private AomArray<AomArray<int>> gm_params = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _GmParams { get { return gm_params; } set { gm_params = value; } }
		private int disable_cdf_update;
		public int _DisableCdfUpdate { get { return disable_cdf_update; } set { disable_cdf_update = value; } }
		private AomArray<int> apply_deblocking_filter = new AomArray<int>();
		public AomArray<int> _ApplyDeblockingFilter { get { return apply_deblocking_filter; } set { apply_deblocking_filter = value; } }
		private int cdef_frame_enable;
		public int _CdefFrameEnable { get { return cdef_frame_enable; } set { cdef_frame_enable = value; } }
		private AomArray<int> ccso_planes = new AomArray<int>();
		public AomArray<int> _CcsoPlanes { get { return ccso_planes; } set { ccso_planes = value; } }
		private AomArray<int> FrameRestorationType = new AomArray<int>();
		public AomArray<int> _FrameRestorationType { get { return FrameRestorationType; } set { FrameRestorationType = value; } }
		private int gdf_frame_enable;
		public int _GdfFrameEnable { get { return gdf_frame_enable; } set { gdf_frame_enable = value; } }
		private int segmentation_enabled;
		public int _SegmentationEnabled { get { return segmentation_enabled; } set { segmentation_enabled = value; } }
		private AomArray<AomArray<int>> FeatureEnabled = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> __FeatureEnabled { get { return FeatureEnabled; } set { FeatureEnabled = value; } }
		private AomArray<AomArray<int>> FeatureData = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _FeatureData { get { return FeatureData; } set { FeatureData = value; } }
		private int HasBothRefs;
		public int _HasBothRefs { get { return HasBothRefs; } set { HasBothRefs = value; } }
		private AomArray<AomArray<int>> PrevSegmentIds = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _PrevSegmentIds { get { return PrevSegmentIds; } set { PrevSegmentIds = value; } }
		private int CodedLossless;
		public int _CodedLossless { get { return CodedLossless; } set { CodedLossless = value; } }
		private int HasLosslessSegment;
		public int _HasLosslessSegment { get { return HasLosslessSegment; } set { HasLosslessSegment = value; } }
		private AomArray<int> LosslessArray = new AomArray<int>();
		public AomArray<int> _LosslessArray { get { return LosslessArray; } set { LosslessArray = value; } }
		private AomArray<AomArray<int>> SegQMLevel = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _SegQMLevel { get { return SegQMLevel; } set { SegQMLevel = value; } }
		private int qm_index;
		public int _QmIndex { get { return qm_index; } set { qm_index = value; } }
		private int allow_tcq;
		public int _AllowTcq { get { return allow_tcq; } set { allow_tcq = value; } }
		private int allow_parity_hiding;
		public int _AllowParityHiding { get { return allow_parity_hiding; } set { allow_parity_hiding = value; } }
		private int allow_bawp;
		public int _AllowBawp { get { return allow_bawp; } set { allow_bawp = value; } }
		private int allow_warpmv_mode;
		public int _AllowWarpmvMode { get { return allow_warpmv_mode; } set { allow_warpmv_mode = value; } }
		private int reduced_tx_set;
		public int _ReducedTxSet { get { return reduced_tx_set; } set { reduced_tx_set = value; } }
		private int row = 0;
		private int col = 0;
		private int refc = 0;
		private int segmentId = 0;

        private void FrameHeaderInfo()
        {
			int i = 0;
			int mode = 0;
			int row = 0;
			int col = 0;
			int refc = 0;
			int plane = 0;
			int j = 0;
			int segmentId = 0;
			int keyFrame = 0;
			int startCVS = 0;
			int allowedFrames = 0;
			int n = 0;
			int allFrames = 0;
			int explicitRefFrameMap = 0;
			int refFrame = 0;
			int hint = 0;
			int usesEqualWeight = 0;
			int refIdx = 0;
			int slot0 = 0;
			int slot1 = 0;
			int qindex = 0;
			int qmNum = 0;
			int qmIndexBits = 0;
			keyFrame = (((obu_type == OBU_CLOSED_LOOP_KEY) || (obu_type == OBU_OPEN_LOOP_KEY)) ? 1 : 0);
			IsRegular = ((((((((obu_type == OBU_OPEN_LOOP_KEY) || (obu_type == OBU_REGULAR_TILE_GROUP)) || (obu_type == OBU_REGULAR_TIP)) || (obu_type == OBU_REGULAR_SEF)) || (obu_type == OBU_SWITCH)) || (obu_type == OBU_RAS_FRAME)) || (obu_type == OBU_BRIDGE_FRAME)) ? 1 : 0);

			for (i = 0; (i < NUM_CUSTOM_QMS); i++)
			{
				QmSeen[i] = 0;
			}
			startCVS = (((obu_type == OBU_CLOSED_LOOP_KEY) && (FirstPictureInTU != 0)) ? 1 : 0);

			if ((startCVS != 0))
			{
				OlkEncountered = 0;

				for (i = 0; (i < MAX_NUM_MLAYERS); i++)
				{
					OlkRefresh[i] = 0;
				}
				flush_implicit_output_frames(0); 
			}

			if ((((OlkEncountered != 0) && (IsRegular != 0)) && (FirstPictureInTU != 0)))
			{
				flush_implicit_output_frames(1); 
				OlkEncountered = 0;
				allowedFrames = 0;

				for (i = 0; (i < MAX_NUM_MLAYERS); i++)
				{
					allowedFrames |= OlkRefresh[i];
					OlkRefresh[i] = 0;
				}

				for (i = 0; (i < NUM_REF_FRAMES); i++)
				{

					if ((((allowedFrames & (1 << i)) == 0) && (RefLongTermId[i] == -1)))
					{
						RefValid[i] = 0;
					}
				}
			}
			IsBridge = ((obu_type == OBU_BRIDGE_FRAME) ? 1 : 0);

			if ((IsBridge != 0))
			{
				cur_mfh_id = 0;
			}
			else 
			{
				stream.ReadUvlc( out this.cur_mfh_id, "cur_mfh_id"); 
			}

			if ((cur_mfh_id == 0))
			{
				stream.ReadUvlc( out this.seq_header_id_in_frame_header, "seq_header_id_in_frame_header"); 
				load_sequence_header(seq_header_id_in_frame_header); 
				mfh_deblocking_filter_update[cur_mfh_id] = 0;
			}
			else 
			{
				load_sequence_header(MfhSeqHeaderId[cur_mfh_id]); 
			}

			if ((keyFrame != 0))
			{

				if ((seq_lcr_id != 0))
				{
					activate_layer_configuration_record(seq_lcr_id); 
				}
			}

			if (((cur_mfh_id == 0) || !(mfh_frame_size_present_flag[cur_mfh_id] != 0)))
			{
				mfh_frame_width_minus_1[cur_mfh_id] = max_frame_width_minus_1;
				mfh_frame_height_minus_1[cur_mfh_id] = max_frame_height_minus_1;
			}

			if (((keyFrame != 0) && (FirstPictureInTU != 0)))
			{
				ResetQm(); 
			}

			if ((IsBridge != 0))
			{
				n = CeilLog2(NumRefFrames);
				stream.ReadVariable(n, out this.bridge_frame_ref_idx, "bridge_frame_ref_idx"); 
			}
			allFrames = ((1 << NumRefFrames) - 1);
			use_bru = 0;
			bru_inactive = 0;

			if ((single_picture_header_flag != 0))
			{
				ShowExistingFrame = 0;
				FrameType = KEY_FRAME;
				FrameIsIntra = 1;
				immediate_output_frame = 1;
				implicit_output_frame = 0;
			}
			else 
			{
				ShowExistingFrame = IsSef();

				if ((ShowExistingFrame == 1))
				{
					n = CeilLog2(NumRefFrames);
					stream.ReadVariable(n, out this.frame_to_show_map_idx, "frame_to_show_map_idx"); 
					stream.ReadFixed(1, out this.derive_sef_order_hint, "derive_sef_order_hint"); 

					if ((derive_sef_order_hint == 0))
					{
						stream.ReadVariable(OrderHintBits, out this.sef_order_hint, "sef_order_hint"); 
						OrderHintLsbs = sef_order_hint;
						OrderHint = GetDispOrderHint();
					}
					else 
					{
						OrderHint = RefOrderHint[frame_to_show_map_idx];
					}

					if ((((IsRegular != 0) && (OlkEncountered != 0)) && !(FirstPictureInTU != 0)))
					{
						OlkTUOrderHint = ((derive_sef_order_hint != 0) ? RefOrderHint[frame_to_show_map_idx] : OrderHint);
					}
					refresh_frame_flags = 0;
					FrameType = RefFrameType[frame_to_show_map_idx];
					immediate_output_frame = 1;
					FilmGrainConfig(); 

					if ((derive_sef_order_hint != 0))
					{
						save_grain_params(frame_to_show_map_idx); 
					}
					TipFrameMode = TIP_FRAME_DISABLED;
					return;
				}

				if ((IsBridge != 0))
				{
					FrameType = INTER_FRAME;
				}
				else if (((obu_type == OBU_SWITCH) || (obu_type == OBU_RAS_FRAME)))
				{
					stream.ReadFixed(1, out this.restricted_prediction_switch, "restricted_prediction_switch"); 
					FrameType = SWITCH_FRAME;
				}
				else if ((IsTipFrame() != 0))
				{
					FrameType = INTER_FRAME;
				}
				else if (((obu_type == OBU_CLOSED_LOOP_KEY) || (obu_type == OBU_OPEN_LOOP_KEY)))
				{
					FrameType = KEY_FRAME;
				}
				else 
				{
					stream.ReadFixed(1, out this.frame_is_inter, "frame_is_inter"); 
					FrameType = ((frame_is_inter != 0) ? INTER_FRAME : INTRA_ONLY_FRAME);
				}
				LongTermId = -1;

				if ((FrameType == KEY_FRAME))
				{
					stream.ReadVariable(long_term_frame_id_bits, out this.long_term_id_plus_1, "long_term_id_plus_1"); 
					LongTermId = (long_term_id_plus_1 - 1);
				}
				num_key_ref_frames = 0;

				if ((((obu_type == OBU_RAS_FRAME) || (obu_type == OBU_OPEN_LOOP_KEY)) && (long_term_frame_id_bits != 0)))
				{
					stream.ReadFixed(3, out this.num_key_ref_frames, "num_key_ref_frames"); 

					for (i = 0; (i < num_key_ref_frames); i++)
					{
						stream.ReadVariable(long_term_frame_id_bits, out this.ref_long_term_id[i], "ref_long_term_id"); 
					}
				}

				if (((FrameType == SWITCH_FRAME) && (restricted_prediction_switch != 0)))
				{

					for (i = 0; (i < NUM_REF_FRAMES); i++)
					{

						if ((MLayerPresenceMap[RefMLayerId[i]][obu_mlayer_id] != 0))
						{

							if ((is_frame_eligible_for_output(i) != 0))
							{
								output_frame_buffers(i); 
							}
							RefOrderHint[i] = RESTRICTED_OH;
						}
					}
				}

				if (((obu_type == OBU_RAS_FRAME) || ((obu_type == OBU_SWITCH) && (restricted_prediction_switch != 0))))
				{
					ResetQm(); 
				}
				FrameIsIntra = (((FrameType == INTRA_ONLY_FRAME) || (FrameType == KEY_FRAME)) ? 1 : 0);

				if (((IsBridge != 0) || (obu_type == OBU_OPEN_LOOP_KEY)))
				{
					immediate_output_frame = 0;
				}
				else 
				{
					stream.ReadFixed(1, out this.immediate_output_frame, "immediate_output_frame"); 
				}

				if ((((IsBridge != 0) || (immediate_output_frame != 0)) || (monotonic_output_order_flag != 0)))
				{
					implicit_output_frame = 0;
				}
				else 
				{
					stream.ReadFixed(1, out this.implicit_output_frame, "implicit_output_frame"); 
				}
			}

			if ((use_256x256_superblock != 0))
			{
				SbSize = ((FrameIsIntra != 0) ? BLOCK_128X128 : BLOCK_256X256);
			}
			else if ((use_128x128_superblock != 0))
			{
				SbSize = BLOCK_128X128;
			}
			else 
			{
				SbSize = BLOCK_64X64;
			}

			if (((FrameType == KEY_FRAME) && (immediate_output_frame != 0)))
			{

				for (i = 0; (i < REFS_PER_FRAME); i++)
				{
					OrderHints[i] = 0;
				}
			}
			disable_cross_frame_cdf_init = 0;

			if ((IsBridge != 0))
			{
				primary_ref_frame = PRIMARY_REF_NONE;
				OrderHintLsbs = RefOrderHintLsbs[bridge_frame_ref_idx];
				OrderHint = RefOrderHint[bridge_frame_ref_idx];
			}
			else 
			{

				if ((FrameType == SWITCH_FRAME))
				{
					frame_size_override_flag = 1;
				}
				else if ((single_picture_header_flag != 0))
				{
					frame_size_override_flag = 0;
				}
				else 
				{
					stream.ReadFixed(1, out this.frame_size_override_flag, "frame_size_override_flag"); 
				}
				stream.ReadVariable(OrderHintBits, out this.order_hint, "order_hint"); 
				OrderHintLsbs = order_hint;
				OrderHint = GetDispOrderHint();

				if (((FrameIsIntra != 0) || (FrameType == SWITCH_FRAME)))
				{
					primary_ref_frame = PRIMARY_REF_NONE;
				}
				else 
				{
					stream.ReadFixed(1, out this.signal_primary_ref_frame, "signal_primary_ref_frame"); 

					if (!(IsTipFrame() != 0))
					{
						stream.ReadFixed(1, out this.disable_cross_frame_cdf_init, "disable_cross_frame_cdf_init"); 
					}

					if ((signal_primary_ref_frame != 0))
					{
						stream.ReadFixed(3, out this.primary_ref_frame, "primary_ref_frame"); 
					}
					else 
					{
						primary_ref_frame = PRIMARY_REF_CHOOSE;
					}
				}
			}
			FrameMvPrecision = MV_PRECISION_ONE_PEL;
			MvPrecision = FrameMvPrecision;
			allow_high_precision_mv = 0;
			use_ref_frame_mvs = 0;
			allow_intrabc = 0;
			allow_global_intrabc = 0;
			allow_local_intrabc = 0;
			allow_high_precision_mv = 0;
			allow_df_sub_pu = 0;

			if ((IsBridge != 0))
			{
				stream.ReadFixed(1, out this.bridge_frame_overwrite_flag, "bridge_frame_overwrite_flag"); 
			}

			if ((FrameType == KEY_FRAME))
			{

				if (((obu_type == OBU_CLOSED_LOOP_KEY) && (max_mlayer_id == 0)))
				{
					refresh_frame_flags = allFrames;
				}
				else if ((enable_short_refresh_frame_flags != 0))
				{
					n = CeilLog2(NumRefFrames);
					stream.ReadVariable(n, out this.frame_to_refresh, "frame_to_refresh"); 
					refresh_frame_flags = (1 << frame_to_refresh);
				}
				else 
				{
					stream.ReadVariable(NumRefFrames, out this.refresh_frame_flags, "refresh_frame_flags"); 
				}

				if (((obu_type == OBU_CLOSED_LOOP_KEY) && (FirstPictureInTU != 0)))
				{

					for (i = 0; (i < NumRefFrames); i++)
					{
						RefValid[i] = 0;
					}
				}

				if ((obu_type == OBU_CLOSED_LOOP_KEY))
				{
					OlkEncountered = 0;

					for (i = 0; (i < MAX_NUM_MLAYERS); i++)
					{
						OlkRefresh[i] = 0;
					}
				}

				if ((obu_type == OBU_OPEN_LOOP_KEY))
				{
					OlkEncountered = 1;
					OlkRefresh[obu_mlayer_id] = refresh_frame_flags;

					if ((implicit_output_frame != 0))
					{
						OlkTUOrderHint = OrderHint;
					}
				}
			}
			else if (((IsBridge != 0) && !(bridge_frame_overwrite_flag != 0)))
			{
				refresh_frame_flags = (1 << bridge_frame_ref_idx);
			}
			else if (((obu_type == OBU_RAS_FRAME) && (max_mlayer_id == 0)))
			{
				refresh_frame_flags = 0;

				for (i = 0; (i < NumRefFrames); i++)
				{

					if ((!(RefValid[i] != 0) || !(LongTermIdInUse(RefLongTermId[i]) != 0)))
					{
						refresh_frame_flags |= (1 << i);
					}
				}
			}
			else if ((FrameType == SWITCH_FRAME))
			{
				stream.ReadVariable(NumRefFrames, out this.refresh_frame_flags, "refresh_frame_flags"); 
			}
			else if ((((enable_short_refresh_frame_flags != 0) && (FrameType != SWITCH_FRAME)) && (FrameType != KEY_FRAME)))
			{
				stream.ReadFixed(1, out this.has_refresh_frame_flags, "has_refresh_frame_flags"); 

				if ((has_refresh_frame_flags != 0))
				{
					n = CeilLog2(NumRefFrames);
					stream.ReadVariable(n, out this.frame_to_refresh, "frame_to_refresh"); 
					refresh_frame_flags = (1 << frame_to_refresh);
				}
				else 
				{
					refresh_frame_flags = 0;
				}
			}
			else 
			{
				stream.ReadVariable(NumRefFrames, out this.refresh_frame_flags, "refresh_frame_flags"); 
			}
			AllowedFrames = -1;

			if ((((IsRegular != 0) && (OlkEncountered != 0)) && !(FirstPictureInTU != 0)))
			{
				AllowedFrames = 0;

				for (i = 0; (i < MAX_NUM_MLAYERS); i++)
				{
					AllowedFrames |= OlkRefresh[i];
				}
				OlkRefresh[obu_mlayer_id] |= refresh_frame_flags;

				if (((immediate_output_frame != 0) || (implicit_output_frame != 0)))
				{
					OlkTUOrderHint = OrderHint;
				}
			}

			if ((FrameIsIntra != 0))
			{
				FrameSize(); 
				ScreenContentParams(); 
				IntrabcParams(); 
				NumTotalRefs = 0;
				TipFrameMode = TIP_FRAME_DISABLED;
			}
			else 
			{

				if (((FrameType == SWITCH_FRAME) || (IsBridge != 0)))
				{
					explicitRefFrameMap = 1;
				}
				else if ((explicit_ref_frame_map != 0))
				{
					stream.ReadFixed(1, out this.frame_explicit_ref_frame_map, "frame_explicit_ref_frame_map"); 
					explicitRefFrameMap = frame_explicit_ref_frame_map;
				}
				else 
				{
					explicitRefFrameMap = 0;
				}

				if ((IsBridge != 0))
				{
					NumTotalRefs = 1;
				}
				else if ((explicitRefFrameMap != 0))
				{
					stream.ReadFixed(3, out this.num_total_refs, "num_total_refs"); 
					NumTotalRefs = num_total_refs;
				}
				else 
				{
					GetRefFrames(0); 
				}

				for (i = 0; (i < NumTotalRefs); i++)
				{

					if ((IsBridge != 0))
					{
						ref_frame_idx[i] = bridge_frame_ref_idx;
					}
					else if ((explicitRefFrameMap != 0))
					{
						n = CeilLog2(NumRefFrames);
						stream.ReadVariable(n, out this.ref_frame_idx[i], "ref_frame_idx"); 
					}
				}

				if ((IsBridge != 0))
				{
					FrameSizeWithBridge(); 
				}
				else if (((frame_size_override_flag != 0) && (FrameType != SWITCH_FRAME)))
				{
					FrameSizeWithRefs(); 
				}
				else 
				{
					FrameSize(); 
				}

				if (!(explicitRefFrameMap != 0))
				{
					GetRefFrames(1); 
				}
				NumSameRefCompound = Min(num_same_ref_compound, NumTotalRefs);

				if (((((enable_bru != 0) && (FrameType == INTER_FRAME)) && !(IsTipFrame() != 0)) && !(IsBridge != 0)))
				{
					stream.ReadFixed(1, out this.use_bru, "use_bru"); 

					if ((use_bru != 0))
					{
						n = CeilLog2(NumTotalRefs);
						stream.ReadVariable(n, out this.bru_ref, "bru_ref"); 
						stream.ReadFixed(1, out this.bru_inactive, "bru_inactive"); 
					}
				}

				if ((explicitRefFrameMap != 0))
				{

					for (i = 0; (i < NumTotalRefs); i++)
					{
						ScoresDistance[i] = GetRelativeDist(OrderHint, RefOrderHint[ref_frame_idx[i]]);
					}
				}
				GetPastFutureCurRefLists(); 

				if (((((FrameType == SWITCH_FRAME) || !(enable_ref_frame_mvs != 0)) || (IsBridge != 0)) || (bru_inactive != 0)))
				{
					use_ref_frame_mvs = 0;
				}
				else 
				{
					stream.ReadFixed(1, out this.use_ref_frame_mvs, "use_ref_frame_mvs"); 
				}

				if ((((use_ref_frame_mvs != 0) && (NumTotalRefs > 1)) && (SbSize != BLOCK_64X64)))
				{
					stream.ReadFixed(1, out this.tmvp_sample_step_minus_1, "tmvp_sample_step_minus_1"); 
					ProjStep = (tmvp_sample_step_minus_1 + 1);
				}
				else 
				{
					ProjStep = 1;
				}

				for (i = 0; (i < NumTotalRefs); i++)
				{
					FrameDistance[i] = GetRelativeDist(OrderHint, RefOrderHint[ref_frame_idx[i]]);

					if ((RefOrderHint[ref_frame_idx[i]] == RESTRICTED_OH))
					{
						FrameDistance[i] = -FrameDistance[i];
					}
				}

				for (i = 0; (i < NumTotalRefs); i++)
				{
					refFrame = i;
					hint = RefOrderHint[ref_frame_idx[i]];
					OrderHints[refFrame] = hint;
				}

				if ((((enable_tip != 0) && ((use_ref_frame_mvs != 0) && (NumTotalRefs >= 2))) && !(bru_inactive != 0)))
				{
					TipInterpFilter = EIGHTTAP_SHARP;
					TipGlobalMv[0] = 0;
					TipGlobalMv[1] = 0;

					if (((EnableTipOutput != 0) && (IsTipFrame() != 0)))
					{
						TipFrameMode = TIP_FRAME_AS_OUTPUT;
					}
					else 
					{
						stream.ReadFixed(1, out this.tip_frame_mode, "tip_frame_mode"); 
						TipFrameMode = tip_frame_mode;
					}
					FrameOpflRefineType(); 

					if (((TipFrameMode != TIP_FRAME_DISABLED) && (enable_tip_hole_fill != 0)))
					{
						stream.ReadFixed(1, out this.allow_tip_hole_fill, "allow_tip_hole_fill"); 
					}
					else 
					{
						allow_tip_hole_fill = 0;
					}
					usesEqualWeight = (((((enable_tip_refinemv != 0) && (NumFutureRefs > 0)) && (NumPastRefs > 0)) && ((opfl_refine_type != REFINE_NONE) || (enable_refinemv != 0))) ? 1 : 0);

					if (((TipFrameMode == TIP_FRAME_DISABLED) || (usesEqualWeight != 0)))
					{
						tip_global_wtd_index = 0;
					}
					else 
					{
						stream.ReadFixed(3, out this.tip_global_wtd_index, "tip_global_wtd_index"); 
					}

					if ((TipFrameMode == TIP_FRAME_AS_OUTPUT))
					{
						stream.ReadFixed(1, out this.tip_mv_zero, "tip_mv_zero"); 

						if (!(tip_mv_zero != 0))
						{
							stream.ReadFixed(4, out this.tip_mv_row, "tip_mv_row"); 
							stream.ReadFixed(4, out this.tip_mv_col, "tip_mv_col"); 

							if ((tip_mv_row != 0))
							{
								stream.ReadFixed(1, out this.tip_mv_row_sign, "tip_mv_row_sign"); 
								TipGlobalMv[0] = ((tip_mv_row_sign != 0) ? -tip_mv_row : tip_mv_row);
							}

							if ((tip_mv_col != 0))
							{
								stream.ReadFixed(1, out this.tip_mv_col_sign, "tip_mv_col_sign"); 
								TipGlobalMv[1] = ((tip_mv_col_sign != 0) ? -tip_mv_col : tip_mv_col);
							}
						}
						stream.ReadFixed(1, out this.tip_sharp, "tip_sharp"); 

						if ((tip_sharp != 0))
						{
							TipInterpFilter = EIGHTTAP_SHARP;
						}
						else 
						{
							stream.ReadFixed(1, out this.tip_regular, "tip_regular"); 
							TipInterpFilter = ((tip_regular != 0) ? EIGHTTAP : EIGHTTAP_SMOOTH);
						}
					}
				}
				else 
				{
					TipFrameMode = TIP_FRAME_DISABLED;

					if ((!(bru_inactive != 0) && !(IsBridge != 0)))
					{
						FrameOpflRefineType(); 
					}
				}

				if ((((TipFrameMode != TIP_FRAME_AS_OUTPUT) && !(bru_inactive != 0)) && !(IsBridge != 0)))
				{
					ScreenContentParams(); 
					IntrabcParams(); 
					max_drl_bits_minus_1 = seq_max_drl_bits_minus_1;

					if ((allow_frame_max_drl_bits != 0))
					{
						stream.ReadFixed(1, out this.change_drl, "change_drl"); 

						if ((change_drl != 0))
						{
							n = (MAX_REF_MV_STACK_SIZE - 2);
							stream.Read_ns(n, out this.max_drl_bits_minus_1, "max_drl_bits_minus_1"); 

							if ((max_drl_bits_minus_1 >= seq_max_drl_bits_minus_1))
							{
								max_drl_bits_minus_1 += 1;
							}
						}
					}

					if ((force_integer_mv != 0))
					{
						FrameMvPrecision = MV_PRECISION_ONE_PEL;
						UsePerBlockMvPrecision = 0;
					}
					else 
					{
						stream.ReadFixed(1, out this.use_qtr_precision_mv, "use_qtr_precision_mv"); 

						if ((use_qtr_precision_mv != 0))
						{
							FrameMvPrecision = MV_PRECISION_QUARTER_PEL;
						}
						else 
						{
							stream.ReadFixed(1, out this.allow_high_precision_mv, "allow_high_precision_mv"); 
							FrameMvPrecision = ((allow_high_precision_mv != 0) ? MV_PRECISION_EIGHTH_PEL : MV_PRECISION_HALF_PEL);
						}
						UsePerBlockMvPrecision = enable_flex_mvres;
					}
					MvPrecision = FrameMvPrecision;
					ReadInterpolationFilter(); 

					for (mode = INTERINTRA; (mode < MOTION_MODES); mode++)
					{

						if (!(seq_frame_motion_modes_present_flag != 0))
						{
							frame_enabled_motion_modes[mode] = seq_enabled_motion_modes[mode];
						}
						else if ((seq_enabled_motion_modes[mode] != 0))
						{
							stream.ReadFixed(1, out this.frame_enabled_motion_modes[mode], "frame_enabled_motion_modes"); 
						}
						else 
						{
							frame_enabled_motion_modes[mode] = 0;
						}
					}
				}
			}

			if ((TipFrameMode == TIP_FRAME_AS_OUTPUT))
			{

				if ((enable_tip_explicit_qp != 0))
				{
					QuantizationParams(); 
				}

				if ((enable_df_sub_pu != 0))
				{
					stream.ReadFixed(1, out this.allow_df_sub_pu, "allow_df_sub_pu"); 
				}

				if ((allow_df_sub_pu != 0))
				{
					stream.ReadFixed(1, out this.apply_deblocking_filter_tip, "apply_deblocking_filter_tip"); 
				}
				else 
				{
					apply_deblocking_filter_tip = 0;
				}
			}

			if ((((TipFrameMode == TIP_FRAME_AS_OUTPUT) || (bru_inactive != 0)) || (IsBridge != 0)))
			{

				for (i = 0; (i < 3); i++)
				{
					frame_filters_on[i] = 0;
				}

				if (((bru_inactive != 0) || (IsBridge != 0)))
				{

					if ((IsBridge != 0))
					{
						TileInfo(); 
						refIdx = bridge_frame_ref_idx;
					}
					else 
					{
						refIdx = ref_frame_idx[bru_ref];
					}
					base_q_idx = RefBaseQIdx[refIdx];
					DeltaQUAc = RefDeltaQUAc[refIdx];
					DeltaQVAc = RefDeltaQVAc[refIdx];
					SetPrimaryRefFrameAndCtx(0); 
				}
				else if ((apply_deblocking_filter_tip != 0))
				{
					TileInfo(); 
				}
				FilmGrainConfig(); 

				if (((bru_inactive != 0) || (IsBridge != 0)))
				{
					SetPrimaryRefFrameAndCtx(1); 
				}

				for (row = 0; (row < MiRows); row++)
				{

					for (col = 0; (col < MiCols); col++)
					{
						SegmentIds[row][col] = 0;
					}
				}

				for (refc = 0; (refc < REFS_PER_FRAME); refc++)
				{

					for (i = 0; (i < 6); i++)
					{
						gm_params[refc][i] = Default_Warp_Params[i];
					}
				}
			}
			else 
			{
				stream.ReadFixed(1, out this.disable_cdf_update, "disable_cdf_update"); 
			}

			if (((bru_inactive != 0) || (IsBridge != 0)))
			{
				apply_deblocking_filter[0] = 0;
				apply_deblocking_filter[1] = 0;
				cdef_frame_enable = 0;

				for (plane = 0; (plane < NumPlanes); plane++)
				{
					ccso_planes[plane] = 0;
				}
				FrameRestorationType[0] = RESTORE_NONE;
				FrameRestorationType[1] = RESTORE_NONE;
				FrameRestorationType[2] = RESTORE_NONE;
				gdf_frame_enable = 0;
				segmentation_enabled = 0;

				for (i = 0; (i < MAX_SEGMENTS); i++)
				{

					for (j = 0; (j < SEG_LVL_MAX); j++)
					{
						FeatureEnabled[i][j] = 0;
						FeatureData[i][j] = 0;
					}
				}

				if (((primary_ref_frame == PRIMARY_REF_NONE) || (disable_cross_frame_cdf_init != 0)))
				{
					init_coeff_cdfs(); 
				}
				return;
			}

			if ((use_ref_frame_mvs == 1))
			{
				HasBothRefs = (((ClosestFuture != NONE) && (ClosestPast != NONE)) ? 1 : 0);
				MotionFieldEstimation(); 

				if ((TipFrameMode == TIP_FRAME_AS_OUTPUT))
				{

					if (!(enable_tip_explicit_qp != 0))
					{
						slot0 = ref_frame_idx[ClosestPast];
						slot1 = ref_frame_idx[ClosestFuture];
						base_q_idx = Round2((RefBaseQIdx[slot0] + RefBaseQIdx[slot1]), 1);
						DeltaQUAc = Round2((RefDeltaQUAc[slot0] + RefDeltaQUAc[slot1]), 1);
						DeltaQVAc = Round2((RefDeltaQVAc[slot0] + RefDeltaQVAc[slot1]), 1);
					}
					SetPrimaryRefFrameAndCtx(1); 

					for (i = 0; (i < MAX_SEGMENTS); i++)
					{

						for (j = 0; (j < SEG_LVL_MAX); j++)
						{
							FeatureData[i][j] = 0;
							FeatureEnabled[i][j] = 0;
						}
					}

					for (row = 0; (row < MiRows); row++)
					{

						for (col = 0; (col < MiCols); col++)
						{
							PrevSegmentIds[row][col] = 0;
						}
					}

					for (plane = 0; (plane < 3); plane++)
					{
						ccso_planes[plane] = 0;
					}

					if (((primary_ref_frame == PRIMARY_REF_NONE) || (disable_cross_frame_cdf_init != 0)))
					{
						init_coeff_cdfs(); 
					}
				}

				if ((TipFrameMode == TIP_FRAME_DISABLED))
				{
					fill_tpl_mvs_sample_gap(); 
				}
			}

			if ((TipFrameMode != TIP_FRAME_DISABLED))
			{
				setup_tip_motion_field(); 
			}

			if ((TipFrameMode == TIP_FRAME_AS_OUTPUT))
			{
				return;
			}
			TileInfo(); 
			QuantizationParams(); 
			SetPrimaryRefFrameAndCtx(1); 
			SegmentationParams(); 
			SetupQmParams(); 
			DeltaqParams(); 

			if (((primary_ref_frame == PRIMARY_REF_NONE) || (disable_cross_frame_cdf_init != 0)))
			{
				init_coeff_cdfs(); 
			}

			if ((DerivedPrimaryRefFrame != PRIMARY_REF_NONE))
			{
				load_previous_segment_ids(); 
			}
			CodedLossless = 1;
			HasLosslessSegment = 0;

			for (segmentId = 0; (segmentId < MaxSegments); segmentId++)
			{
				qindex = get_qindex(1, segmentId);
				LosslessArray[segmentId] = ((((((((qindex == 0) && (delta_q_present == 0)) && ((DeltaQYDc + BaseYDcDeltaQ) <= 0)) && ((DeltaQUDc + BaseUVDcDeltaQ) <= 0)) && ((DeltaQVDc + BaseUVDcDeltaQ) <= 0)) && ((DeltaQUAc + BaseUVAcDeltaQ) <= 0)) && ((DeltaQVAc + BaseUVAcDeltaQ) <= 0)) ? 1 : 0);

				if ((LosslessArray[segmentId] != 0))
				{
					HasLosslessSegment = 1;
				}
				else 
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
						qmNum = (pic_qm_num_minus_1 + 1);
						qmIndexBits = CeilLog2(qmNum);
						stream.ReadVariable(qmIndexBits, out this.qm_index, "qm_index"); 
						SegQMLevel[0][segmentId] = qm_y[qm_index];
						SegQMLevel[1][segmentId] = qm_u[qm_index];
						SegQMLevel[2][segmentId] = qm_v[qm_index];
					}
				}
			}

			if ((CodedLossless != 0))
			{
				allow_tcq = 0;
			}
			else if ((choose_tcq_per_frame != 0))
			{
				stream.ReadFixed(1, out this.allow_tcq, "allow_tcq"); 
			}
			else 
			{
				allow_tcq = enable_tcq;
			}

			if ((((CodedLossless != 0) || !(enable_parity_hiding != 0)) || (allow_tcq != 0)))
			{
				allow_parity_hiding = 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.allow_parity_hiding, "allow_parity_hiding"); 
			}
			DeblockingFilterParams(); 
			GdfParams(); 
			CdefParams(); 
			LrParams(); 
			CcsoParams(); 
			ReadTxMode(); 
			FrameReferenceMode(); 
			SkipModeParams(); 

			if ((!(FrameIsIntra != 0) && (enable_bawp != 0)))
			{
				stream.ReadFixed(1, out this.allow_bawp, "allow_bawp"); 
			}
			else 
			{
				allow_bawp = 0;
			}

			if ((!(FrameIsIntra != 0) && (frame_enabled_motion_modes[DELTAWARP] != 0)))
			{
				stream.ReadFixed(1, out this.allow_warpmv_mode, "allow_warpmv_mode"); 
			}
			else 
			{
				allow_warpmv_mode = 0;
			}
			stream.ReadFixed(2, out this.reduced_tx_set, "reduced_tx_set"); 
			GlobalMotionParams(); 
			FilmGrainConfig(); 
        }

        private void WriteFrameHeaderInfo()
        {
			int i = 0;
			int mode = 0;
			int row = 0;
			int col = 0;
			int refc = 0;
			int plane = 0;
			int j = 0;
			int segmentId = 0;
			int keyFrame = 0;
			int startCVS = 0;
			int allowedFrames = 0;
			int n = 0;
			int allFrames = 0;
			int explicitRefFrameMap = 0;
			int refFrame = 0;
			int hint = 0;
			int usesEqualWeight = 0;
			int refIdx = 0;
			int slot0 = 0;
			int slot1 = 0;
			int qindex = 0;
			int qmNum = 0;
			int qmIndexBits = 0;
			keyFrame = (((obu_type == OBU_CLOSED_LOOP_KEY) || (obu_type == OBU_OPEN_LOOP_KEY)) ? 1 : 0);
			IsRegular = ((((((((obu_type == OBU_OPEN_LOOP_KEY) || (obu_type == OBU_REGULAR_TILE_GROUP)) || (obu_type == OBU_REGULAR_TIP)) || (obu_type == OBU_REGULAR_SEF)) || (obu_type == OBU_SWITCH)) || (obu_type == OBU_RAS_FRAME)) || (obu_type == OBU_BRIDGE_FRAME)) ? 1 : 0);

			for (i = 0; (i < NUM_CUSTOM_QMS); i++)
			{
				QmSeen[i] = 0;
			}
			startCVS = (((obu_type == OBU_CLOSED_LOOP_KEY) && (FirstPictureInTU != 0)) ? 1 : 0);

			if ((startCVS != 0))
			{
				OlkEncountered = 0;

				for (i = 0; (i < MAX_NUM_MLAYERS); i++)
				{
					OlkRefresh[i] = 0;
				}
				flush_implicit_output_frames(0); 
			}

			if ((((OlkEncountered != 0) && (IsRegular != 0)) && (FirstPictureInTU != 0)))
			{
				flush_implicit_output_frames(1); 
				OlkEncountered = 0;
				allowedFrames = 0;

				for (i = 0; (i < MAX_NUM_MLAYERS); i++)
				{
					allowedFrames |= OlkRefresh[i];
					OlkRefresh[i] = 0;
				}

				for (i = 0; (i < NUM_REF_FRAMES); i++)
				{

					if ((((allowedFrames & (1 << i)) == 0) && (RefLongTermId[i] == -1)))
					{
						RefValid[i] = 0;
					}
				}
			}
			IsBridge = ((obu_type == OBU_BRIDGE_FRAME) ? 1 : 0);

			if ((IsBridge != 0))
			{
				cur_mfh_id = 0;
			}
			else 
			{
				this.cur_mfh_id = stream.Pick("cur_mfh_id", _original != null ? _original.cur_mfh_id : this.cur_mfh_id, _edited != null ? _edited.cur_mfh_id : _original != null ? _original.cur_mfh_id : this.cur_mfh_id);
				stream.WriteUvlc( this.cur_mfh_id, "cur_mfh_id"); 
			}

			if ((cur_mfh_id == 0))
			{
				this.seq_header_id_in_frame_header = stream.Pick("seq_header_id_in_frame_header", _original != null ? _original.seq_header_id_in_frame_header : this.seq_header_id_in_frame_header, _edited != null ? _edited.seq_header_id_in_frame_header : _original != null ? _original.seq_header_id_in_frame_header : this.seq_header_id_in_frame_header);
				stream.WriteUvlc( this.seq_header_id_in_frame_header, "seq_header_id_in_frame_header"); 
				load_sequence_header(seq_header_id_in_frame_header); 
				mfh_deblocking_filter_update[cur_mfh_id] = 0;
			}
			else 
			{
				load_sequence_header(MfhSeqHeaderId[cur_mfh_id]); 
			}

			if ((keyFrame != 0))
			{

				if ((seq_lcr_id != 0))
				{
					activate_layer_configuration_record(seq_lcr_id); 
				}
			}

			if (((cur_mfh_id == 0) || !(mfh_frame_size_present_flag[cur_mfh_id] != 0)))
			{
				mfh_frame_width_minus_1[cur_mfh_id] = max_frame_width_minus_1;
				mfh_frame_height_minus_1[cur_mfh_id] = max_frame_height_minus_1;
			}

			if (((keyFrame != 0) && (FirstPictureInTU != 0)))
			{
				ResetQm(); 
			}

			if ((IsBridge != 0))
			{
				n = CeilLog2(NumRefFrames);
				this.bridge_frame_ref_idx = stream.Pick("bridge_frame_ref_idx", _original != null ? _original.bridge_frame_ref_idx : this.bridge_frame_ref_idx, _edited != null ? _edited.bridge_frame_ref_idx : _original != null ? _original.bridge_frame_ref_idx : this.bridge_frame_ref_idx);
				stream.WriteVariable(n, this.bridge_frame_ref_idx, "bridge_frame_ref_idx"); 
			}
			allFrames = ((1 << NumRefFrames) - 1);
			use_bru = 0;
			bru_inactive = 0;

			if ((single_picture_header_flag != 0))
			{
				ShowExistingFrame = 0;
				FrameType = KEY_FRAME;
				FrameIsIntra = 1;
				immediate_output_frame = 1;
				implicit_output_frame = 0;
			}
			else 
			{
				ShowExistingFrame = IsSef();

				if ((ShowExistingFrame == 1))
				{
					n = CeilLog2(NumRefFrames);
					this.frame_to_show_map_idx = stream.Pick("frame_to_show_map_idx", _original != null ? _original.frame_to_show_map_idx : this.frame_to_show_map_idx, _edited != null ? _edited.frame_to_show_map_idx : _original != null ? _original.frame_to_show_map_idx : this.frame_to_show_map_idx);
					stream.WriteVariable(n, this.frame_to_show_map_idx, "frame_to_show_map_idx"); 
					this.derive_sef_order_hint = stream.Pick("derive_sef_order_hint", _original != null ? _original.derive_sef_order_hint : this.derive_sef_order_hint, _edited != null ? _edited.derive_sef_order_hint : _original != null ? _original.derive_sef_order_hint : this.derive_sef_order_hint);
					stream.WriteFixed(1, this.derive_sef_order_hint, "derive_sef_order_hint"); 

					if ((derive_sef_order_hint == 0))
					{
						this.sef_order_hint = stream.Pick("sef_order_hint", _original != null ? _original.OrderHintLsbs : this.sef_order_hint, _edited != null ? _edited.OrderHintLsbs : _original != null ? _original.OrderHintLsbs : this.sef_order_hint);
						stream.WriteVariable(OrderHintBits, this.sef_order_hint, "sef_order_hint"); 
						OrderHintLsbs = sef_order_hint;
						OrderHint = GetDispOrderHint();
					}
					else 
					{
						OrderHint = RefOrderHint[frame_to_show_map_idx];
					}

					if ((((IsRegular != 0) && (OlkEncountered != 0)) && !(FirstPictureInTU != 0)))
					{
						OlkTUOrderHint = ((derive_sef_order_hint != 0) ? RefOrderHint[frame_to_show_map_idx] : OrderHint);
					}
					refresh_frame_flags = 0;
					FrameType = RefFrameType[frame_to_show_map_idx];
					immediate_output_frame = 1;
					WriteFilmGrainConfig(); 

					if ((derive_sef_order_hint != 0))
					{
						save_grain_params(frame_to_show_map_idx); 
					}
					TipFrameMode = TIP_FRAME_DISABLED;
					return;
				}

				if ((IsBridge != 0))
				{
					FrameType = INTER_FRAME;
				}
				else if (((obu_type == OBU_SWITCH) || (obu_type == OBU_RAS_FRAME)))
				{
					this.restricted_prediction_switch = stream.Pick("restricted_prediction_switch", _original != null ? _original.restricted_prediction_switch : this.restricted_prediction_switch, _edited != null ? _edited.restricted_prediction_switch : _original != null ? _original.restricted_prediction_switch : this.restricted_prediction_switch);
					stream.WriteFixed(1, this.restricted_prediction_switch, "restricted_prediction_switch"); 
					FrameType = SWITCH_FRAME;
				}
				else if ((IsTipFrame() != 0))
				{
					FrameType = INTER_FRAME;
				}
				else if (((obu_type == OBU_CLOSED_LOOP_KEY) || (obu_type == OBU_OPEN_LOOP_KEY)))
				{
					FrameType = KEY_FRAME;
				}
				else 
				{
					this.frame_is_inter = stream.Pick("frame_is_inter", _original != null ? _original.frame_is_inter : this.frame_is_inter, _edited != null ? _edited.frame_is_inter : _original != null ? _original.frame_is_inter : this.frame_is_inter);
					stream.WriteFixed(1, this.frame_is_inter, "frame_is_inter"); 
					FrameType = ((frame_is_inter != 0) ? INTER_FRAME : INTRA_ONLY_FRAME);
				}
				LongTermId = -1;

				if ((FrameType == KEY_FRAME))
				{
					this.long_term_id_plus_1 = stream.Pick("long_term_id_plus_1", _original != null ? _original.long_term_id_plus_1 : this.long_term_id_plus_1, _edited != null ? _edited.long_term_id_plus_1 : _original != null ? _original.long_term_id_plus_1 : this.long_term_id_plus_1);
					stream.WriteVariable(long_term_frame_id_bits, this.long_term_id_plus_1, "long_term_id_plus_1"); 
					LongTermId = (long_term_id_plus_1 - 1);
				}
				num_key_ref_frames = 0;

				if ((((obu_type == OBU_RAS_FRAME) || (obu_type == OBU_OPEN_LOOP_KEY)) && (long_term_frame_id_bits != 0)))
				{
					this.num_key_ref_frames = stream.Pick("num_key_ref_frames", _original != null ? _original.num_key_ref_frames : this.num_key_ref_frames, _edited != null ? _edited.num_key_ref_frames : _original != null ? _original.num_key_ref_frames : this.num_key_ref_frames);
					stream.WriteFixed(3, this.num_key_ref_frames, "num_key_ref_frames"); 

					for (i = 0; (i < num_key_ref_frames); i++)
					{
						this.ref_long_term_id[i] = stream.Pick("ref_long_term_id", _original != null ? _original.ref_long_term_id[i] : this.ref_long_term_id[i], _edited != null ? _edited.ref_long_term_id[i] : _original != null ? _original.ref_long_term_id[i] : this.ref_long_term_id[i]);
						stream.WriteVariable(long_term_frame_id_bits, this.ref_long_term_id[i], "ref_long_term_id"); 
					}
				}

				if (((FrameType == SWITCH_FRAME) && (restricted_prediction_switch != 0)))
				{

					for (i = 0; (i < NUM_REF_FRAMES); i++)
					{

						if ((MLayerPresenceMap[RefMLayerId[i]][obu_mlayer_id] != 0))
						{

							if ((is_frame_eligible_for_output(i) != 0))
							{
								output_frame_buffers(i); 
							}
							RefOrderHint[i] = RESTRICTED_OH;
						}
					}
				}

				if (((obu_type == OBU_RAS_FRAME) || ((obu_type == OBU_SWITCH) && (restricted_prediction_switch != 0))))
				{
					ResetQm(); 
				}
				FrameIsIntra = (((FrameType == INTRA_ONLY_FRAME) || (FrameType == KEY_FRAME)) ? 1 : 0);

				if (((IsBridge != 0) || (obu_type == OBU_OPEN_LOOP_KEY)))
				{
					immediate_output_frame = 0;
				}
				else 
				{
					this.immediate_output_frame = stream.Pick("immediate_output_frame", _original != null ? _original.immediate_output_frame : this.immediate_output_frame, _edited != null ? _edited.immediate_output_frame : _original != null ? _original.immediate_output_frame : this.immediate_output_frame);
					stream.WriteFixed(1, this.immediate_output_frame, "immediate_output_frame"); 
				}

				if ((((IsBridge != 0) || (immediate_output_frame != 0)) || (monotonic_output_order_flag != 0)))
				{
					implicit_output_frame = 0;
				}
				else 
				{
					this.implicit_output_frame = stream.Pick("implicit_output_frame", _original != null ? _original.implicit_output_frame : this.implicit_output_frame, _edited != null ? _edited.implicit_output_frame : _original != null ? _original.implicit_output_frame : this.implicit_output_frame);
					stream.WriteFixed(1, this.implicit_output_frame, "implicit_output_frame"); 
				}
			}

			if ((use_256x256_superblock != 0))
			{
				SbSize = ((FrameIsIntra != 0) ? BLOCK_128X128 : BLOCK_256X256);
			}
			else if ((use_128x128_superblock != 0))
			{
				SbSize = BLOCK_128X128;
			}
			else 
			{
				SbSize = BLOCK_64X64;
			}

			if (((FrameType == KEY_FRAME) && (immediate_output_frame != 0)))
			{

				for (i = 0; (i < REFS_PER_FRAME); i++)
				{
					OrderHints[i] = 0;
				}
			}
			disable_cross_frame_cdf_init = 0;

			if ((IsBridge != 0))
			{
				primary_ref_frame = PRIMARY_REF_NONE;
				OrderHintLsbs = RefOrderHintLsbs[bridge_frame_ref_idx];
				OrderHint = RefOrderHint[bridge_frame_ref_idx];
			}
			else 
			{

				if ((FrameType == SWITCH_FRAME))
				{
					frame_size_override_flag = 1;
				}
				else if ((single_picture_header_flag != 0))
				{
					frame_size_override_flag = 0;
				}
				else 
				{
					this.frame_size_override_flag = stream.Pick("frame_size_override_flag", _original != null ? _original.frame_size_override_flag : this.frame_size_override_flag, _edited != null ? _edited.frame_size_override_flag : _original != null ? _original.frame_size_override_flag : this.frame_size_override_flag);
					stream.WriteFixed(1, this.frame_size_override_flag, "frame_size_override_flag"); 
				}
				this.order_hint = stream.Pick("order_hint", _original != null ? _original.OrderHintLsbs : this.order_hint, _edited != null ? _edited.OrderHintLsbs : _original != null ? _original.OrderHintLsbs : this.order_hint);
				stream.WriteVariable(OrderHintBits, this.order_hint, "order_hint"); 
				OrderHintLsbs = order_hint;
				OrderHint = GetDispOrderHint();

				if (((FrameIsIntra != 0) || (FrameType == SWITCH_FRAME)))
				{
					primary_ref_frame = PRIMARY_REF_NONE;
				}
				else 
				{
					this.signal_primary_ref_frame = stream.Pick("signal_primary_ref_frame", _original != null ? _original.signal_primary_ref_frame : this.signal_primary_ref_frame, _edited != null ? _edited.signal_primary_ref_frame : _original != null ? _original.signal_primary_ref_frame : this.signal_primary_ref_frame);
					stream.WriteFixed(1, this.signal_primary_ref_frame, "signal_primary_ref_frame"); 

					if (!(IsTipFrame() != 0))
					{
						this.disable_cross_frame_cdf_init = stream.Pick("disable_cross_frame_cdf_init", _original != null ? _original.disable_cross_frame_cdf_init : this.disable_cross_frame_cdf_init, _edited != null ? _edited.disable_cross_frame_cdf_init : _original != null ? _original.disable_cross_frame_cdf_init : this.disable_cross_frame_cdf_init);
						stream.WriteFixed(1, this.disable_cross_frame_cdf_init, "disable_cross_frame_cdf_init"); 
					}

					if ((signal_primary_ref_frame != 0))
					{
						this.primary_ref_frame = stream.Pick("primary_ref_frame", _original != null ? _original.primary_ref_frame : this.primary_ref_frame, _edited != null ? _edited.primary_ref_frame : _original != null ? _original.primary_ref_frame : this.primary_ref_frame);
						stream.WriteFixed(3, this.primary_ref_frame, "primary_ref_frame"); 
					}
					else 
					{
						primary_ref_frame = PRIMARY_REF_CHOOSE;
					}
				}
			}
			FrameMvPrecision = MV_PRECISION_ONE_PEL;
			MvPrecision = FrameMvPrecision;
			allow_high_precision_mv = 0;
			use_ref_frame_mvs = 0;
			allow_intrabc = 0;
			allow_global_intrabc = 0;
			allow_local_intrabc = 0;
			allow_high_precision_mv = 0;
			allow_df_sub_pu = 0;

			if ((IsBridge != 0))
			{
				this.bridge_frame_overwrite_flag = stream.Pick("bridge_frame_overwrite_flag", _original != null ? _original.bridge_frame_overwrite_flag : this.bridge_frame_overwrite_flag, _edited != null ? _edited.bridge_frame_overwrite_flag : _original != null ? _original.bridge_frame_overwrite_flag : this.bridge_frame_overwrite_flag);
				stream.WriteFixed(1, this.bridge_frame_overwrite_flag, "bridge_frame_overwrite_flag"); 
			}

			if ((FrameType == KEY_FRAME))
			{

				if (((obu_type == OBU_CLOSED_LOOP_KEY) && (max_mlayer_id == 0)))
				{
					refresh_frame_flags = allFrames;
				}
				else if ((enable_short_refresh_frame_flags != 0))
				{
					n = CeilLog2(NumRefFrames);
					this.frame_to_refresh = stream.Pick("frame_to_refresh", _original != null ? _original.frame_to_refresh : this.frame_to_refresh, _edited != null ? _edited.frame_to_refresh : _original != null ? _original.frame_to_refresh : this.frame_to_refresh);
					stream.WriteVariable(n, this.frame_to_refresh, "frame_to_refresh"); 
					refresh_frame_flags = (1 << frame_to_refresh);
				}
				else 
				{
					this.refresh_frame_flags = stream.Pick("refresh_frame_flags", _original != null ? _original.refresh_frame_flags : this.refresh_frame_flags, _edited != null ? _edited.refresh_frame_flags : _original != null ? _original.refresh_frame_flags : this.refresh_frame_flags);
					stream.WriteVariable(NumRefFrames, this.refresh_frame_flags, "refresh_frame_flags"); 
				}

				if (((obu_type == OBU_CLOSED_LOOP_KEY) && (FirstPictureInTU != 0)))
				{

					for (i = 0; (i < NumRefFrames); i++)
					{
						RefValid[i] = 0;
					}
				}

				if ((obu_type == OBU_CLOSED_LOOP_KEY))
				{
					OlkEncountered = 0;

					for (i = 0; (i < MAX_NUM_MLAYERS); i++)
					{
						OlkRefresh[i] = 0;
					}
				}

				if ((obu_type == OBU_OPEN_LOOP_KEY))
				{
					OlkEncountered = 1;
					OlkRefresh[obu_mlayer_id] = refresh_frame_flags;

					if ((implicit_output_frame != 0))
					{
						OlkTUOrderHint = OrderHint;
					}
				}
			}
			else if (((IsBridge != 0) && !(bridge_frame_overwrite_flag != 0)))
			{
				refresh_frame_flags = (1 << bridge_frame_ref_idx);
			}
			else if (((obu_type == OBU_RAS_FRAME) && (max_mlayer_id == 0)))
			{
				refresh_frame_flags = 0;

				for (i = 0; (i < NumRefFrames); i++)
				{

					if ((!(RefValid[i] != 0) || !(LongTermIdInUse(RefLongTermId[i]) != 0)))
					{
						refresh_frame_flags |= (1 << i);
					}
				}
			}
			else if ((FrameType == SWITCH_FRAME))
			{
				this.refresh_frame_flags = stream.Pick("refresh_frame_flags", _original != null ? _original.refresh_frame_flags : this.refresh_frame_flags, _edited != null ? _edited.refresh_frame_flags : _original != null ? _original.refresh_frame_flags : this.refresh_frame_flags);
				stream.WriteVariable(NumRefFrames, this.refresh_frame_flags, "refresh_frame_flags"); 
			}
			else if ((((enable_short_refresh_frame_flags != 0) && (FrameType != SWITCH_FRAME)) && (FrameType != KEY_FRAME)))
			{
				this.has_refresh_frame_flags = stream.Pick("has_refresh_frame_flags", _original != null ? _original.has_refresh_frame_flags : this.has_refresh_frame_flags, _edited != null ? _edited.has_refresh_frame_flags : _original != null ? _original.has_refresh_frame_flags : this.has_refresh_frame_flags);
				stream.WriteFixed(1, this.has_refresh_frame_flags, "has_refresh_frame_flags"); 

				if ((has_refresh_frame_flags != 0))
				{
					n = CeilLog2(NumRefFrames);
					this.frame_to_refresh = stream.Pick("frame_to_refresh", _original != null ? _original.frame_to_refresh : this.frame_to_refresh, _edited != null ? _edited.frame_to_refresh : _original != null ? _original.frame_to_refresh : this.frame_to_refresh);
					stream.WriteVariable(n, this.frame_to_refresh, "frame_to_refresh"); 
					refresh_frame_flags = (1 << frame_to_refresh);
				}
				else 
				{
					refresh_frame_flags = 0;
				}
			}
			else 
			{
				this.refresh_frame_flags = stream.Pick("refresh_frame_flags", _original != null ? _original.refresh_frame_flags : this.refresh_frame_flags, _edited != null ? _edited.refresh_frame_flags : _original != null ? _original.refresh_frame_flags : this.refresh_frame_flags);
				stream.WriteVariable(NumRefFrames, this.refresh_frame_flags, "refresh_frame_flags"); 
			}
			AllowedFrames = -1;

			if ((((IsRegular != 0) && (OlkEncountered != 0)) && !(FirstPictureInTU != 0)))
			{
				AllowedFrames = 0;

				for (i = 0; (i < MAX_NUM_MLAYERS); i++)
				{
					AllowedFrames |= OlkRefresh[i];
				}
				OlkRefresh[obu_mlayer_id] |= refresh_frame_flags;

				if (((immediate_output_frame != 0) || (implicit_output_frame != 0)))
				{
					OlkTUOrderHint = OrderHint;
				}
			}

			if ((FrameIsIntra != 0))
			{
				WriteFrameSize(); 
				WriteScreenContentParams(); 
				WriteIntrabcParams(); 
				NumTotalRefs = 0;
				TipFrameMode = TIP_FRAME_DISABLED;
			}
			else 
			{

				if (((FrameType == SWITCH_FRAME) || (IsBridge != 0)))
				{
					explicitRefFrameMap = 1;
				}
				else if ((explicit_ref_frame_map != 0))
				{
					this.frame_explicit_ref_frame_map = stream.Pick("frame_explicit_ref_frame_map", _original != null ? _original.frame_explicit_ref_frame_map : this.frame_explicit_ref_frame_map, _edited != null ? _edited.frame_explicit_ref_frame_map : _original != null ? _original.frame_explicit_ref_frame_map : this.frame_explicit_ref_frame_map);
					stream.WriteFixed(1, this.frame_explicit_ref_frame_map, "frame_explicit_ref_frame_map"); 
					explicitRefFrameMap = frame_explicit_ref_frame_map;
				}
				else 
				{
					explicitRefFrameMap = 0;
				}

				if ((IsBridge != 0))
				{
					NumTotalRefs = 1;
				}
				else if ((explicitRefFrameMap != 0))
				{
					this.num_total_refs = stream.Pick("num_total_refs", _original != null ? _original.NumTotalRefs : this.num_total_refs, _edited != null ? _edited.NumTotalRefs : _original != null ? _original.NumTotalRefs : this.num_total_refs);
					stream.WriteFixed(3, this.num_total_refs, "num_total_refs"); 
					NumTotalRefs = num_total_refs;
				}
				else 
				{
					GetRefFrames(0); 
				}

				for (i = 0; (i < NumTotalRefs); i++)
				{

					if ((IsBridge != 0))
					{
						ref_frame_idx[i] = bridge_frame_ref_idx;
					}
					else if ((explicitRefFrameMap != 0))
					{
						n = CeilLog2(NumRefFrames);
						this.ref_frame_idx[i] = stream.Pick("ref_frame_idx", _original != null ? _original.ref_frame_idx[i] : this.ref_frame_idx[i], _edited != null ? _edited.ref_frame_idx[i] : _original != null ? _original.ref_frame_idx[i] : this.ref_frame_idx[i]);
						stream.WriteVariable(n, this.ref_frame_idx[i], "ref_frame_idx"); 
					}
				}

				if ((IsBridge != 0))
				{
					WriteFrameSizeWithBridge(); 
				}
				else if (((frame_size_override_flag != 0) && (FrameType != SWITCH_FRAME)))
				{
					WriteFrameSizeWithRefs(); 
				}
				else 
				{
					WriteFrameSize(); 
				}

				if (!(explicitRefFrameMap != 0))
				{
					GetRefFrames(1); 
				}
				NumSameRefCompound = Min(num_same_ref_compound, NumTotalRefs);

				if (((((enable_bru != 0) && (FrameType == INTER_FRAME)) && !(IsTipFrame() != 0)) && !(IsBridge != 0)))
				{
					this.use_bru = stream.Pick("use_bru", _original != null ? _original.use_bru : this.use_bru, _edited != null ? _edited.use_bru : _original != null ? _original.use_bru : this.use_bru);
					stream.WriteFixed(1, this.use_bru, "use_bru"); 

					if ((use_bru != 0))
					{
						n = CeilLog2(NumTotalRefs);
						this.bru_ref = stream.Pick("bru_ref", _original != null ? _original.bru_ref : this.bru_ref, _edited != null ? _edited.bru_ref : _original != null ? _original.bru_ref : this.bru_ref);
						stream.WriteVariable(n, this.bru_ref, "bru_ref"); 
						this.bru_inactive = stream.Pick("bru_inactive", _original != null ? _original.bru_inactive : this.bru_inactive, _edited != null ? _edited.bru_inactive : _original != null ? _original.bru_inactive : this.bru_inactive);
						stream.WriteFixed(1, this.bru_inactive, "bru_inactive"); 
					}
				}

				if ((explicitRefFrameMap != 0))
				{

					for (i = 0; (i < NumTotalRefs); i++)
					{
						ScoresDistance[i] = GetRelativeDist(OrderHint, RefOrderHint[ref_frame_idx[i]]);
					}
				}
				GetPastFutureCurRefLists(); 

				if (((((FrameType == SWITCH_FRAME) || !(enable_ref_frame_mvs != 0)) || (IsBridge != 0)) || (bru_inactive != 0)))
				{
					use_ref_frame_mvs = 0;
				}
				else 
				{
					this.use_ref_frame_mvs = stream.Pick("use_ref_frame_mvs", _original != null ? _original.use_ref_frame_mvs : this.use_ref_frame_mvs, _edited != null ? _edited.use_ref_frame_mvs : _original != null ? _original.use_ref_frame_mvs : this.use_ref_frame_mvs);
					stream.WriteFixed(1, this.use_ref_frame_mvs, "use_ref_frame_mvs"); 
				}

				if ((((use_ref_frame_mvs != 0) && (NumTotalRefs > 1)) && (SbSize != BLOCK_64X64)))
				{
					this.tmvp_sample_step_minus_1 = stream.Pick("tmvp_sample_step_minus_1", _original != null ? _original.tmvp_sample_step_minus_1 : this.tmvp_sample_step_minus_1, _edited != null ? _edited.tmvp_sample_step_minus_1 : _original != null ? _original.tmvp_sample_step_minus_1 : this.tmvp_sample_step_minus_1);
					stream.WriteFixed(1, this.tmvp_sample_step_minus_1, "tmvp_sample_step_minus_1"); 
					ProjStep = (tmvp_sample_step_minus_1 + 1);
				}
				else 
				{
					ProjStep = 1;
				}

				for (i = 0; (i < NumTotalRefs); i++)
				{
					FrameDistance[i] = GetRelativeDist(OrderHint, RefOrderHint[ref_frame_idx[i]]);

					if ((RefOrderHint[ref_frame_idx[i]] == RESTRICTED_OH))
					{
						FrameDistance[i] = -FrameDistance[i];
					}
				}

				for (i = 0; (i < NumTotalRefs); i++)
				{
					refFrame = i;
					hint = RefOrderHint[ref_frame_idx[i]];
					OrderHints[refFrame] = hint;
				}

				if ((((enable_tip != 0) && ((use_ref_frame_mvs != 0) && (NumTotalRefs >= 2))) && !(bru_inactive != 0)))
				{
					TipInterpFilter = EIGHTTAP_SHARP;
					TipGlobalMv[0] = 0;
					TipGlobalMv[1] = 0;

					if (((EnableTipOutput != 0) && (IsTipFrame() != 0)))
					{
						TipFrameMode = TIP_FRAME_AS_OUTPUT;
					}
					else 
					{
						this.tip_frame_mode = stream.Pick("tip_frame_mode", _original != null ? _original.TipFrameMode : this.tip_frame_mode, _edited != null ? _edited.TipFrameMode : _original != null ? _original.TipFrameMode : this.tip_frame_mode);
						stream.WriteFixed(1, this.tip_frame_mode, "tip_frame_mode"); 
						TipFrameMode = tip_frame_mode;
					}
					WriteFrameOpflRefineType(); 

					if (((TipFrameMode != TIP_FRAME_DISABLED) && (enable_tip_hole_fill != 0)))
					{
						this.allow_tip_hole_fill = stream.Pick("allow_tip_hole_fill", _original != null ? _original.allow_tip_hole_fill : this.allow_tip_hole_fill, _edited != null ? _edited.allow_tip_hole_fill : _original != null ? _original.allow_tip_hole_fill : this.allow_tip_hole_fill);
						stream.WriteFixed(1, this.allow_tip_hole_fill, "allow_tip_hole_fill"); 
					}
					else 
					{
						allow_tip_hole_fill = 0;
					}
					usesEqualWeight = (((((enable_tip_refinemv != 0) && (NumFutureRefs > 0)) && (NumPastRefs > 0)) && ((opfl_refine_type != REFINE_NONE) || (enable_refinemv != 0))) ? 1 : 0);

					if (((TipFrameMode == TIP_FRAME_DISABLED) || (usesEqualWeight != 0)))
					{
						tip_global_wtd_index = 0;
					}
					else 
					{
						this.tip_global_wtd_index = stream.Pick("tip_global_wtd_index", _original != null ? _original.tip_global_wtd_index : this.tip_global_wtd_index, _edited != null ? _edited.tip_global_wtd_index : _original != null ? _original.tip_global_wtd_index : this.tip_global_wtd_index);
						stream.WriteFixed(3, this.tip_global_wtd_index, "tip_global_wtd_index"); 
					}

					if ((TipFrameMode == TIP_FRAME_AS_OUTPUT))
					{
						this.tip_mv_zero = stream.Pick("tip_mv_zero", _original != null ? _original.tip_mv_zero : this.tip_mv_zero, _edited != null ? _edited.tip_mv_zero : _original != null ? _original.tip_mv_zero : this.tip_mv_zero);
						stream.WriteFixed(1, this.tip_mv_zero, "tip_mv_zero"); 

						if (!(tip_mv_zero != 0))
						{
							this.tip_mv_row = stream.Pick("tip_mv_row", _original != null ? _original.tip_mv_row : this.tip_mv_row, _edited != null ? _edited.tip_mv_row : _original != null ? _original.tip_mv_row : this.tip_mv_row);
							stream.WriteFixed(4, this.tip_mv_row, "tip_mv_row"); 
							this.tip_mv_col = stream.Pick("tip_mv_col", _original != null ? _original.tip_mv_col : this.tip_mv_col, _edited != null ? _edited.tip_mv_col : _original != null ? _original.tip_mv_col : this.tip_mv_col);
							stream.WriteFixed(4, this.tip_mv_col, "tip_mv_col"); 

							if ((tip_mv_row != 0))
							{
								this.tip_mv_row_sign = stream.Pick("tip_mv_row_sign", _original != null ? _original.tip_mv_row_sign : this.tip_mv_row_sign, _edited != null ? _edited.tip_mv_row_sign : _original != null ? _original.tip_mv_row_sign : this.tip_mv_row_sign);
								stream.WriteFixed(1, this.tip_mv_row_sign, "tip_mv_row_sign"); 
								TipGlobalMv[0] = ((tip_mv_row_sign != 0) ? -tip_mv_row : tip_mv_row);
							}

							if ((tip_mv_col != 0))
							{
								this.tip_mv_col_sign = stream.Pick("tip_mv_col_sign", _original != null ? _original.tip_mv_col_sign : this.tip_mv_col_sign, _edited != null ? _edited.tip_mv_col_sign : _original != null ? _original.tip_mv_col_sign : this.tip_mv_col_sign);
								stream.WriteFixed(1, this.tip_mv_col_sign, "tip_mv_col_sign"); 
								TipGlobalMv[1] = ((tip_mv_col_sign != 0) ? -tip_mv_col : tip_mv_col);
							}
						}
						this.tip_sharp = stream.Pick("tip_sharp", _original != null ? _original.tip_sharp : this.tip_sharp, _edited != null ? _edited.tip_sharp : _original != null ? _original.tip_sharp : this.tip_sharp);
						stream.WriteFixed(1, this.tip_sharp, "tip_sharp"); 

						if ((tip_sharp != 0))
						{
							TipInterpFilter = EIGHTTAP_SHARP;
						}
						else 
						{
							this.tip_regular = stream.Pick("tip_regular", _original != null ? _original.tip_regular : this.tip_regular, _edited != null ? _edited.tip_regular : _original != null ? _original.tip_regular : this.tip_regular);
							stream.WriteFixed(1, this.tip_regular, "tip_regular"); 
							TipInterpFilter = ((tip_regular != 0) ? EIGHTTAP : EIGHTTAP_SMOOTH);
						}
					}
				}
				else 
				{
					TipFrameMode = TIP_FRAME_DISABLED;

					if ((!(bru_inactive != 0) && !(IsBridge != 0)))
					{
						WriteFrameOpflRefineType(); 
					}
				}

				if ((((TipFrameMode != TIP_FRAME_AS_OUTPUT) && !(bru_inactive != 0)) && !(IsBridge != 0)))
				{
					WriteScreenContentParams(); 
					WriteIntrabcParams(); 
					max_drl_bits_minus_1 = seq_max_drl_bits_minus_1;

					if ((allow_frame_max_drl_bits != 0))
					{
						this.change_drl = stream.Pick("change_drl", _original != null ? _original.change_drl : this.change_drl, _edited != null ? _edited.change_drl : _original != null ? _original.change_drl : this.change_drl);
						stream.WriteFixed(1, this.change_drl, "change_drl"); 

						if ((change_drl != 0))
						{
							n = (MAX_REF_MV_STACK_SIZE - 2);
							this.max_drl_bits_minus_1 = stream.Pick("max_drl_bits_minus_1", _original != null ? _original.max_drl_bits_minus_1 : this.max_drl_bits_minus_1, _edited != null ? _edited.max_drl_bits_minus_1 : _original != null ? _original.max_drl_bits_minus_1 : this.max_drl_bits_minus_1);
							stream.Write_ns(n, this.max_drl_bits_minus_1, "max_drl_bits_minus_1"); 

							if ((max_drl_bits_minus_1 >= seq_max_drl_bits_minus_1))
							{
								max_drl_bits_minus_1 += 1;
							}
						}
					}

					if ((force_integer_mv != 0))
					{
						FrameMvPrecision = MV_PRECISION_ONE_PEL;
						UsePerBlockMvPrecision = 0;
					}
					else 
					{
						this.use_qtr_precision_mv = stream.Pick("use_qtr_precision_mv", _original != null ? _original.use_qtr_precision_mv : this.use_qtr_precision_mv, _edited != null ? _edited.use_qtr_precision_mv : _original != null ? _original.use_qtr_precision_mv : this.use_qtr_precision_mv);
						stream.WriteFixed(1, this.use_qtr_precision_mv, "use_qtr_precision_mv"); 

						if ((use_qtr_precision_mv != 0))
						{
							FrameMvPrecision = MV_PRECISION_QUARTER_PEL;
						}
						else 
						{
							this.allow_high_precision_mv = stream.Pick("allow_high_precision_mv", _original != null ? _original.allow_high_precision_mv : this.allow_high_precision_mv, _edited != null ? _edited.allow_high_precision_mv : _original != null ? _original.allow_high_precision_mv : this.allow_high_precision_mv);
							stream.WriteFixed(1, this.allow_high_precision_mv, "allow_high_precision_mv"); 
							FrameMvPrecision = ((allow_high_precision_mv != 0) ? MV_PRECISION_EIGHTH_PEL : MV_PRECISION_HALF_PEL);
						}
						UsePerBlockMvPrecision = enable_flex_mvres;
					}
					MvPrecision = FrameMvPrecision;
					WriteReadInterpolationFilter(); 

					for (mode = INTERINTRA; (mode < MOTION_MODES); mode++)
					{

						if (!(seq_frame_motion_modes_present_flag != 0))
						{
							frame_enabled_motion_modes[mode] = seq_enabled_motion_modes[mode];
						}
						else if ((seq_enabled_motion_modes[mode] != 0))
						{
							this.frame_enabled_motion_modes[mode] = stream.Pick("frame_enabled_motion_modes", _original != null ? _original.frame_enabled_motion_modes[mode] : this.frame_enabled_motion_modes[mode], _edited != null ? _edited.frame_enabled_motion_modes[mode] : _original != null ? _original.frame_enabled_motion_modes[mode] : this.frame_enabled_motion_modes[mode]);
							stream.WriteFixed(1, this.frame_enabled_motion_modes[mode], "frame_enabled_motion_modes"); 
						}
						else 
						{
							frame_enabled_motion_modes[mode] = 0;
						}
					}
				}
			}

			if ((TipFrameMode == TIP_FRAME_AS_OUTPUT))
			{

				if ((enable_tip_explicit_qp != 0))
				{
					WriteQuantizationParams(); 
				}

				if ((enable_df_sub_pu != 0))
				{
					this.allow_df_sub_pu = stream.Pick("allow_df_sub_pu", _original != null ? _original.allow_df_sub_pu : this.allow_df_sub_pu, _edited != null ? _edited.allow_df_sub_pu : _original != null ? _original.allow_df_sub_pu : this.allow_df_sub_pu);
					stream.WriteFixed(1, this.allow_df_sub_pu, "allow_df_sub_pu"); 
				}

				if ((allow_df_sub_pu != 0))
				{
					this.apply_deblocking_filter_tip = stream.Pick("apply_deblocking_filter_tip", _original != null ? _original.apply_deblocking_filter_tip : this.apply_deblocking_filter_tip, _edited != null ? _edited.apply_deblocking_filter_tip : _original != null ? _original.apply_deblocking_filter_tip : this.apply_deblocking_filter_tip);
					stream.WriteFixed(1, this.apply_deblocking_filter_tip, "apply_deblocking_filter_tip"); 
				}
				else 
				{
					apply_deblocking_filter_tip = 0;
				}
			}

			if ((((TipFrameMode == TIP_FRAME_AS_OUTPUT) || (bru_inactive != 0)) || (IsBridge != 0)))
			{

				for (i = 0; (i < 3); i++)
				{
					frame_filters_on[i] = 0;
				}

				if (((bru_inactive != 0) || (IsBridge != 0)))
				{

					if ((IsBridge != 0))
					{
						WriteTileInfo(); 
						refIdx = bridge_frame_ref_idx;
					}
					else 
					{
						refIdx = ref_frame_idx[bru_ref];
					}
					base_q_idx = RefBaseQIdx[refIdx];
					DeltaQUAc = RefDeltaQUAc[refIdx];
					DeltaQVAc = RefDeltaQVAc[refIdx];
					SetPrimaryRefFrameAndCtx(0); 
				}
				else if ((apply_deblocking_filter_tip != 0))
				{
					WriteTileInfo(); 
				}
				WriteFilmGrainConfig(); 

				if (((bru_inactive != 0) || (IsBridge != 0)))
				{
					SetPrimaryRefFrameAndCtx(1); 
				}

				for (row = 0; (row < MiRows); row++)
				{

					for (col = 0; (col < MiCols); col++)
					{
						SegmentIds[row][col] = 0;
					}
				}

				for (refc = 0; (refc < REFS_PER_FRAME); refc++)
				{

					for (i = 0; (i < 6); i++)
					{
						gm_params[refc][i] = Default_Warp_Params[i];
					}
				}
			}
			else 
			{
				this.disable_cdf_update = stream.Pick("disable_cdf_update", _original != null ? _original.disable_cdf_update : this.disable_cdf_update, _edited != null ? _edited.disable_cdf_update : _original != null ? _original.disable_cdf_update : this.disable_cdf_update);
				stream.WriteFixed(1, this.disable_cdf_update, "disable_cdf_update"); 
			}

			if (((bru_inactive != 0) || (IsBridge != 0)))
			{
				apply_deblocking_filter[0] = 0;
				apply_deblocking_filter[1] = 0;
				cdef_frame_enable = 0;

				for (plane = 0; (plane < NumPlanes); plane++)
				{
					ccso_planes[plane] = 0;
				}
				FrameRestorationType[0] = RESTORE_NONE;
				FrameRestorationType[1] = RESTORE_NONE;
				FrameRestorationType[2] = RESTORE_NONE;
				gdf_frame_enable = 0;
				segmentation_enabled = 0;

				for (i = 0; (i < MAX_SEGMENTS); i++)
				{

					for (j = 0; (j < SEG_LVL_MAX); j++)
					{
						FeatureEnabled[i][j] = 0;
						FeatureData[i][j] = 0;
					}
				}

				if (((primary_ref_frame == PRIMARY_REF_NONE) || (disable_cross_frame_cdf_init != 0)))
				{
					init_coeff_cdfs(); 
				}
				return;
			}

			if ((use_ref_frame_mvs == 1))
			{
				HasBothRefs = (((ClosestFuture != NONE) && (ClosestPast != NONE)) ? 1 : 0);
				MotionFieldEstimation(); 

				if ((TipFrameMode == TIP_FRAME_AS_OUTPUT))
				{

					if (!(enable_tip_explicit_qp != 0))
					{
						slot0 = ref_frame_idx[ClosestPast];
						slot1 = ref_frame_idx[ClosestFuture];
						base_q_idx = Round2((RefBaseQIdx[slot0] + RefBaseQIdx[slot1]), 1);
						DeltaQUAc = Round2((RefDeltaQUAc[slot0] + RefDeltaQUAc[slot1]), 1);
						DeltaQVAc = Round2((RefDeltaQVAc[slot0] + RefDeltaQVAc[slot1]), 1);
					}
					SetPrimaryRefFrameAndCtx(1); 

					for (i = 0; (i < MAX_SEGMENTS); i++)
					{

						for (j = 0; (j < SEG_LVL_MAX); j++)
						{
							FeatureData[i][j] = 0;
							FeatureEnabled[i][j] = 0;
						}
					}

					for (row = 0; (row < MiRows); row++)
					{

						for (col = 0; (col < MiCols); col++)
						{
							PrevSegmentIds[row][col] = 0;
						}
					}

					for (plane = 0; (plane < 3); plane++)
					{
						ccso_planes[plane] = 0;
					}

					if (((primary_ref_frame == PRIMARY_REF_NONE) || (disable_cross_frame_cdf_init != 0)))
					{
						init_coeff_cdfs(); 
					}
				}

				if ((TipFrameMode == TIP_FRAME_DISABLED))
				{
					fill_tpl_mvs_sample_gap(); 
				}
			}

			if ((TipFrameMode != TIP_FRAME_DISABLED))
			{
				setup_tip_motion_field(); 
			}

			if ((TipFrameMode == TIP_FRAME_AS_OUTPUT))
			{
				return;
			}
			WriteTileInfo(); 
			WriteQuantizationParams(); 
			SetPrimaryRefFrameAndCtx(1); 
			WriteSegmentationParams(); 
			WriteSetupQmParams(); 
			WriteDeltaqParams(); 

			if (((primary_ref_frame == PRIMARY_REF_NONE) || (disable_cross_frame_cdf_init != 0)))
			{
				init_coeff_cdfs(); 
			}

			if ((DerivedPrimaryRefFrame != PRIMARY_REF_NONE))
			{
				load_previous_segment_ids(); 
			}
			CodedLossless = 1;
			HasLosslessSegment = 0;

			for (segmentId = 0; (segmentId < MaxSegments); segmentId++)
			{
				qindex = get_qindex(1, segmentId);
				LosslessArray[segmentId] = ((((((((qindex == 0) && (delta_q_present == 0)) && ((DeltaQYDc + BaseYDcDeltaQ) <= 0)) && ((DeltaQUDc + BaseUVDcDeltaQ) <= 0)) && ((DeltaQVDc + BaseUVDcDeltaQ) <= 0)) && ((DeltaQUAc + BaseUVAcDeltaQ) <= 0)) && ((DeltaQVAc + BaseUVAcDeltaQ) <= 0)) ? 1 : 0);

				if ((LosslessArray[segmentId] != 0))
				{
					HasLosslessSegment = 1;
				}
				else 
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
						qmNum = (pic_qm_num_minus_1 + 1);
						qmIndexBits = CeilLog2(qmNum);
						this.qm_index = stream.Pick("qm_index", _original != null ? _original.qm_index : this.qm_index, _edited != null ? _edited.qm_index : _original != null ? _original.qm_index : this.qm_index);
						stream.WriteVariable(qmIndexBits, this.qm_index, "qm_index"); 
						SegQMLevel[0][segmentId] = qm_y[qm_index];
						SegQMLevel[1][segmentId] = qm_u[qm_index];
						SegQMLevel[2][segmentId] = qm_v[qm_index];
					}
				}
			}

			if ((CodedLossless != 0))
			{
				allow_tcq = 0;
			}
			else if ((choose_tcq_per_frame != 0))
			{
				this.allow_tcq = stream.Pick("allow_tcq", _original != null ? _original.allow_tcq : this.allow_tcq, _edited != null ? _edited.allow_tcq : _original != null ? _original.allow_tcq : this.allow_tcq);
				stream.WriteFixed(1, this.allow_tcq, "allow_tcq"); 
			}
			else 
			{
				allow_tcq = enable_tcq;
			}

			if ((((CodedLossless != 0) || !(enable_parity_hiding != 0)) || (allow_tcq != 0)))
			{
				allow_parity_hiding = 0;
			}
			else 
			{
				this.allow_parity_hiding = stream.Pick("allow_parity_hiding", _original != null ? _original.allow_parity_hiding : this.allow_parity_hiding, _edited != null ? _edited.allow_parity_hiding : _original != null ? _original.allow_parity_hiding : this.allow_parity_hiding);
				stream.WriteFixed(1, this.allow_parity_hiding, "allow_parity_hiding"); 
			}
			WriteDeblockingFilterParams(); 
			WriteGdfParams(); 
			WriteCdefParams(); 
			WriteLrParams(); 
			WriteCcsoParams(); 
			WriteReadTxMode(); 
			WriteFrameReferenceMode(); 
			WriteSkipModeParams(); 

			if ((!(FrameIsIntra != 0) && (enable_bawp != 0)))
			{
				this.allow_bawp = stream.Pick("allow_bawp", _original != null ? _original.allow_bawp : this.allow_bawp, _edited != null ? _edited.allow_bawp : _original != null ? _original.allow_bawp : this.allow_bawp);
				stream.WriteFixed(1, this.allow_bawp, "allow_bawp"); 
			}
			else 
			{
				allow_bawp = 0;
			}

			if ((!(FrameIsIntra != 0) && (frame_enabled_motion_modes[DELTAWARP] != 0)))
			{
				this.allow_warpmv_mode = stream.Pick("allow_warpmv_mode", _original != null ? _original.allow_warpmv_mode : this.allow_warpmv_mode, _edited != null ? _edited.allow_warpmv_mode : _original != null ? _original.allow_warpmv_mode : this.allow_warpmv_mode);
				stream.WriteFixed(1, this.allow_warpmv_mode, "allow_warpmv_mode"); 
			}
			else 
			{
				allow_warpmv_mode = 0;
			}
			this.reduced_tx_set = stream.Pick("reduced_tx_set", _original != null ? _original.reduced_tx_set : this.reduced_tx_set, _edited != null ? _edited.reduced_tx_set : _original != null ? _original.reduced_tx_set : this.reduced_tx_set);
			stream.WriteFixed(2, this.reduced_tx_set, "reduced_tx_set"); 
			WriteGlobalMotionParams(); 
			WriteFilmGrainConfig(); 
        }

    /*
frame_opfl_refine_type() {
if ( TipFrameMode == TIP_FRAME_AS_OUTPUT ) {	
opfl_refine_type = ( !enable_tip_refinemv ||	
enable_opfl_refine == REFINE_NONE ) ?	
REFINE_NONE : REFINE_ALL	
} else if ( enable_opfl_refine == REFINE_AUTO ) {	
opfl_refine_type	f(1)
if ( opfl_refine_type != REFINE_SWITCHABLE ) {	
opfl_refine_all	f(1)
opfl_refine_type = opfl_refine_all ? REFINE_ALL : REFINE_NONE	
}	
} else {	
opfl_refine_type = enable_opfl_refine	
}	
}
    */
		private int opfl_refine_type;
		public int _OpflRefineType { get { return opfl_refine_type; } set { opfl_refine_type = value; } }
		private int opfl_refine_all;
		public int _OpflRefineAll { get { return opfl_refine_all; } set { opfl_refine_all = value; } }

        private void FrameOpflRefineType()
        {

			if ((TipFrameMode == TIP_FRAME_AS_OUTPUT))
			{
				opfl_refine_type = ((!(enable_tip_refinemv != 0) || (enable_opfl_refine == REFINE_NONE)) ? REFINE_NONE : REFINE_ALL);
			}
			else if ((enable_opfl_refine == REFINE_AUTO))
			{
				stream.ReadFixed(1, out this.opfl_refine_type, "opfl_refine_type"); 

				if ((opfl_refine_type != REFINE_SWITCHABLE))
				{
					stream.ReadFixed(1, out this.opfl_refine_all, "opfl_refine_all"); 
					opfl_refine_type = ((opfl_refine_all != 0) ? REFINE_ALL : REFINE_NONE);
				}
			}
			else 
			{
				opfl_refine_type = enable_opfl_refine;
			}
        }

        private void WriteFrameOpflRefineType()
        {

			if ((TipFrameMode == TIP_FRAME_AS_OUTPUT))
			{
				opfl_refine_type = ((!(enable_tip_refinemv != 0) || (enable_opfl_refine == REFINE_NONE)) ? REFINE_NONE : REFINE_ALL);
			}
			else if ((enable_opfl_refine == REFINE_AUTO))
			{
				this.opfl_refine_type = stream.Pick("opfl_refine_type", _original != null ? _original.opfl_refine_type : this.opfl_refine_type, _edited != null ? _edited.opfl_refine_type : _original != null ? _original.opfl_refine_type : this.opfl_refine_type);
				stream.WriteFixed(1, this.opfl_refine_type, "opfl_refine_type"); 

				if ((opfl_refine_type != REFINE_SWITCHABLE))
				{
					this.opfl_refine_all = stream.Pick("opfl_refine_all", _original != null ? _original.opfl_refine_all : this.opfl_refine_all, _edited != null ? _edited.opfl_refine_all : _original != null ? _original.opfl_refine_all : this.opfl_refine_all);
					stream.WriteFixed(1, this.opfl_refine_all, "opfl_refine_all"); 
					opfl_refine_type = ((opfl_refine_all != 0) ? REFINE_ALL : REFINE_NONE);
				}
			}
			else 
			{
				opfl_refine_type = enable_opfl_refine;
			}
        }

    /*
screen_content_params() {
if ( seq_force_screen_content_tools == SELECT_SCREEN_CONTENT_TOOLS ) {	
allow_screen_content_tools	f(1)
} else {	
allow_screen_content_tools = seq_force_screen_content_tools	
}	
if ( allow_screen_content_tools ) {	
if ( seq_force_integer_mv == SELECT_INTEGER_MV ) {	
force_integer_mv	f(1)
} else {	
force_integer_mv = seq_force_integer_mv	
}	
} else {	
force_integer_mv = 0	
}	
}
    */
		private int allow_screen_content_tools;
		public int _AllowScreenContentTools { get { return allow_screen_content_tools; } set { allow_screen_content_tools = value; } }
		private int force_integer_mv;
		public int _ForceIntegerMv { get { return force_integer_mv; } set { force_integer_mv = value; } }

        private void ScreenContentParams()
        {

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
        }

        private void WriteScreenContentParams()
        {

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
        }

    /*
intrabc_params() {
allow_intrabc	f(1)
if ( allow_intrabc ) {	
if ( FrameIsIntra ) {	
allow_global_intrabc	f(1)
if ( allow_global_intrabc ) {	
allow_local_intrabc	f(1)
} else {	
allow_local_intrabc = 1	
}	
} else {	
allow_global_intrabc = 0	
allow_local_intrabc = 1	
}	
max_bvp_drl_bits_minus_1 = seq_max_bvp_drl_bits_minus_1	
if ( allow_frame_max_bvp_drl_bits ) {	
change_bvp_drl	f(1)
if ( change_bvp_drl ) {	
max_bvp_drl_bits_minus_1	ns(2)
if ( max_bvp_drl_bits_minus_1 >=	
seq_max_bvp_drl_bits_minus_1 ) {	
max_bvp_drl_bits_minus_1 += 1	
}	
}	
}	
}	
}
    */
		private int max_bvp_drl_bits_minus_1;
		public int _MaxBvpDrlBitsMinus1 { get { return max_bvp_drl_bits_minus_1; } set { max_bvp_drl_bits_minus_1 = value; } }
		private int change_bvp_drl;
		public int _ChangeBvpDrl { get { return change_bvp_drl; } set { change_bvp_drl = value; } }

        private void IntrabcParams()
        {
			stream.ReadFixed(1, out this.allow_intrabc, "allow_intrabc"); 

			if ((allow_intrabc != 0))
			{

				if ((FrameIsIntra != 0))
				{
					stream.ReadFixed(1, out this.allow_global_intrabc, "allow_global_intrabc"); 

					if ((allow_global_intrabc != 0))
					{
						stream.ReadFixed(1, out this.allow_local_intrabc, "allow_local_intrabc"); 
					}
					else 
					{
						allow_local_intrabc = 1;
					}
				}
				else 
				{
					allow_global_intrabc = 0;
					allow_local_intrabc = 1;
				}
				max_bvp_drl_bits_minus_1 = seq_max_bvp_drl_bits_minus_1;

				if ((allow_frame_max_bvp_drl_bits != 0))
				{
					stream.ReadFixed(1, out this.change_bvp_drl, "change_bvp_drl"); 

					if ((change_bvp_drl != 0))
					{
						stream.Read_ns(2, out this.max_bvp_drl_bits_minus_1, "max_bvp_drl_bits_minus_1"); 

						if ((max_bvp_drl_bits_minus_1 >= seq_max_bvp_drl_bits_minus_1))
						{
							max_bvp_drl_bits_minus_1 += 1;
						}
					}
				}
			}
        }

        private void WriteIntrabcParams()
        {
			this.allow_intrabc = stream.Pick("allow_intrabc", _original != null ? _original.allow_intrabc : this.allow_intrabc, _edited != null ? _edited.allow_intrabc : _original != null ? _original.allow_intrabc : this.allow_intrabc);
			stream.WriteFixed(1, this.allow_intrabc, "allow_intrabc"); 

			if ((allow_intrabc != 0))
			{

				if ((FrameIsIntra != 0))
				{
					this.allow_global_intrabc = stream.Pick("allow_global_intrabc", _original != null ? _original.allow_global_intrabc : this.allow_global_intrabc, _edited != null ? _edited.allow_global_intrabc : _original != null ? _original.allow_global_intrabc : this.allow_global_intrabc);
					stream.WriteFixed(1, this.allow_global_intrabc, "allow_global_intrabc"); 

					if ((allow_global_intrabc != 0))
					{
						this.allow_local_intrabc = stream.Pick("allow_local_intrabc", _original != null ? _original.allow_local_intrabc : this.allow_local_intrabc, _edited != null ? _edited.allow_local_intrabc : _original != null ? _original.allow_local_intrabc : this.allow_local_intrabc);
						stream.WriteFixed(1, this.allow_local_intrabc, "allow_local_intrabc"); 
					}
					else 
					{
						allow_local_intrabc = 1;
					}
				}
				else 
				{
					allow_global_intrabc = 0;
					allow_local_intrabc = 1;
				}
				max_bvp_drl_bits_minus_1 = seq_max_bvp_drl_bits_minus_1;

				if ((allow_frame_max_bvp_drl_bits != 0))
				{
					this.change_bvp_drl = stream.Pick("change_bvp_drl", _original != null ? _original.change_bvp_drl : this.change_bvp_drl, _edited != null ? _edited.change_bvp_drl : _original != null ? _original.change_bvp_drl : this.change_bvp_drl);
					stream.WriteFixed(1, this.change_bvp_drl, "change_bvp_drl"); 

					if ((change_bvp_drl != 0))
					{
						this.max_bvp_drl_bits_minus_1 = stream.Pick("max_bvp_drl_bits_minus_1", _original != null ? _original.max_bvp_drl_bits_minus_1 : this.max_bvp_drl_bits_minus_1, _edited != null ? _edited.max_bvp_drl_bits_minus_1 : _original != null ? _original.max_bvp_drl_bits_minus_1 : this.max_bvp_drl_bits_minus_1);
						stream.Write_ns(2, this.max_bvp_drl_bits_minus_1, "max_bvp_drl_bits_minus_1"); 

						if ((max_bvp_drl_bits_minus_1 >= seq_max_bvp_drl_bits_minus_1))
						{
							max_bvp_drl_bits_minus_1 += 1;
						}
					}
				}
			}
        }

    /*
frame_size() {
if ( frame_size_override_flag ) {	
n = frame_width_bits_minus_1 + 1	
frame_width_minus_1	f(n)
n = frame_height_bits_minus_1 + 1	
frame_height_minus_1	f(n)
FrameWidth = frame_width_minus_1 + 1	
FrameHeight = frame_height_minus_1 + 1	
} else {	
FrameWidth = mfh_frame_width_minus_1[ cur_mfh_id ] + 1	
FrameHeight = mfh_frame_height_minus_1[ cur_mfh_id ] + 1	
}	
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
				FrameWidth = (mfh_frame_width_minus_1[cur_mfh_id] + 1);
				FrameHeight = (mfh_frame_height_minus_1[cur_mfh_id] + 1);
			}
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
				FrameWidth = (mfh_frame_width_minus_1[cur_mfh_id] + 1);
				FrameHeight = (mfh_frame_height_minus_1[cur_mfh_id] + 1);
			}
			ComputeImageSize(); 
        }

    /*
frame_size_with_bridge() {
n = frame_width_bits_minus_1 + 1	
bridge_frame_width_minus_1	f(n)
n = frame_height_bits_minus_1 + 1	
bridge_frame_height_minus_1	f(n)
FrameWidth = Min( RefFrameWidth[ bridge_frame_ref_idx ],	
bridge_frame_width_minus_1 + 1 )	
FrameHeight = Min( RefFrameHeight[ bridge_frame_ref_idx ],	
bridge_frame_height_minus_1 + 1 )	
compute_image_size()	
}
    */
		private int bridge_frame_width_minus_1;
		public int _BridgeFrameWidthMinus1 { get { return bridge_frame_width_minus_1; } set { bridge_frame_width_minus_1 = value; } }
		private int bridge_frame_height_minus_1;
		public int _BridgeFrameHeightMinus1 { get { return bridge_frame_height_minus_1; } set { bridge_frame_height_minus_1 = value; } }

        private void FrameSizeWithBridge()
        {
			int n = 0;
			n = (frame_width_bits_minus_1 + 1);
			stream.ReadVariable(n, out this.bridge_frame_width_minus_1, "bridge_frame_width_minus_1"); 
			n = (frame_height_bits_minus_1 + 1);
			stream.ReadVariable(n, out this.bridge_frame_height_minus_1, "bridge_frame_height_minus_1"); 
			FrameWidth = Min(RefFrameWidth[bridge_frame_ref_idx], (bridge_frame_width_minus_1 + 1));
			FrameHeight = Min(RefFrameHeight[bridge_frame_ref_idx], (bridge_frame_height_minus_1 + 1));
			ComputeImageSize(); 
        }

        private void WriteFrameSizeWithBridge()
        {
			int n = 0;
			n = (frame_width_bits_minus_1 + 1);
			this.bridge_frame_width_minus_1 = stream.Pick("bridge_frame_width_minus_1", _original != null ? _original.bridge_frame_width_minus_1 : this.bridge_frame_width_minus_1, _edited != null ? _edited.bridge_frame_width_minus_1 : _original != null ? _original.bridge_frame_width_minus_1 : this.bridge_frame_width_minus_1);
			stream.WriteVariable(n, this.bridge_frame_width_minus_1, "bridge_frame_width_minus_1"); 
			n = (frame_height_bits_minus_1 + 1);
			this.bridge_frame_height_minus_1 = stream.Pick("bridge_frame_height_minus_1", _original != null ? _original.bridge_frame_height_minus_1 : this.bridge_frame_height_minus_1, _edited != null ? _edited.bridge_frame_height_minus_1 : _original != null ? _original.bridge_frame_height_minus_1 : this.bridge_frame_height_minus_1);
			stream.WriteVariable(n, this.bridge_frame_height_minus_1, "bridge_frame_height_minus_1"); 
			FrameWidth = Min(RefFrameWidth[bridge_frame_ref_idx], (bridge_frame_width_minus_1 + 1));
			FrameHeight = Min(RefFrameHeight[bridge_frame_ref_idx], (bridge_frame_height_minus_1 + 1));
			ComputeImageSize(); 
        }

    /*
frame_size_with_refs() {
for ( i = 0; i < NumTotalRefs; i++ ) {	
found_ref	f(1)
if ( found_ref == 1 ) {	
FrameWidth = RefFrameWidth[ ref_frame_idx[ i ] ]	
FrameHeight = RefFrameHeight[ ref_frame_idx[ i ] ]	
break	
}	
}	
if ( NumTotalRefs == 0 || found_ref == 0 ) {	
frame_size()	
} else {	
compute_image_size()	
}	
}
    */
		private int found_ref;
		public int _FoundRef { get { return found_ref; } set { found_ref = value; } }

        private void FrameSizeWithRefs()
        {
			int i = 0;

			for (i = 0; (i < NumTotalRefs); i++)
			{
				stream.ReadFixed(1, out this.found_ref, "found_ref"); 

				if ((found_ref == 1))
				{
					FrameWidth = RefFrameWidth[ref_frame_idx[i]];
					FrameHeight = RefFrameHeight[ref_frame_idx[i]];
					break;
				}
			}

			if (((NumTotalRefs == 0) || (found_ref == 0)))
			{
				FrameSize(); 
			}
			else 
			{
				ComputeImageSize(); 
			}
        }

        private void WriteFrameSizeWithRefs()
        {
			int i = 0;

			for (i = 0; (i < NumTotalRefs); i++)
			{
				this.found_ref = stream.Pick("found_ref", _original != null ? (_original.FrameWidth == RefFrameWidth[ref_frame_idx[i]] && _original.FrameHeight == RefFrameHeight[ref_frame_idx[i]] ? 1 : 0) : this.found_ref, _edited != null ? (_edited.FrameWidth == RefFrameWidth[ref_frame_idx[i]] && _edited.FrameHeight == RefFrameHeight[ref_frame_idx[i]] ? 1 : 0) : _original != null ? (_original.FrameWidth == RefFrameWidth[ref_frame_idx[i]] && _original.FrameHeight == RefFrameHeight[ref_frame_idx[i]] ? 1 : 0) : this.found_ref);
				stream.WriteFixed(1, this.found_ref, "found_ref"); 

				if ((found_ref == 1))
				{
					FrameWidth = RefFrameWidth[ref_frame_idx[i]];
					FrameHeight = RefFrameHeight[ref_frame_idx[i]];
					break;
				}
			}

			if (((NumTotalRefs == 0) || (found_ref == 0)))
			{
				WriteFrameSize(); 
			}
			else 
			{
				ComputeImageSize(); 
			}
        }

    /*
read_interpolation_filter() {
is_filter_switchable	f(1)
if ( is_filter_switchable == 1 ) {	
interpolation_filter = SWITCHABLE	
} else {	
interpolation_filter	f(2)
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
deblocking_filter_params() {
if ( CodedLossless ) {	
apply_deblocking_filter[ 0 ] = 0	
apply_deblocking_filter[ 1 ] = 0	
return	
}	
if ( enable_df_sub_pu && FrameType == INTER_FRAME ) {	
allow_df_sub_pu	f(1)
} else {	
allow_df_sub_pu = 0	
}	
if ( mfh_deblocking_filter_update[ cur_mfh_id ] ) {	
apply_deblocking_filter[ 0 ] = mfh_apply_deblocking_filter[ cur_mfh_id ][ 0 ]	
apply_deblocking_filter[ 1 ] = mfh_apply_deblocking_filter[ cur_mfh_id ][ 1 ]	
apply_deblocking_filter[ 2 ] = 0	
apply_deblocking_filter[ 3 ] = 0	
if ( NumPlanes > 1 ) {	
if ( apply_deblocking_filter[0] || apply_deblocking_filter[1] ) {	
apply_deblocking_filter[2] = mfh_apply_deblocking_filter[cur_mfh_id][2]	
apply_deblocking_filter[3] = mfh_apply_deblocking_filter[cur_mfh_id][3]	
}	
}	
} else {	
apply_deblocking_filter[ 0 ]	f(1)
apply_deblocking_filter[ 1 ]	f(1)
apply_deblocking_filter[ 2 ] = 0	
apply_deblocking_filter[ 3 ] = 0	
if ( NumPlanes > 1 ) {	
if ( apply_deblocking_filter[ 0 ] || apply_deblocking_filter[ 1 ] ) {	
apply_deblocking_filter[ 2 ]	f(1)
apply_deblocking_filter[ 3 ]	f(1)
}	
}	
}	
for ( i = 0; i < 4; i++ ) {	
if ( apply_deblocking_filter[ i ] ) {	
df_delta_q_present[ i ]	f(1)
if ( df_delta_q_present[ i ] ) {	
dfParBits = df_par_bits_minus_2 + 2	
df_delta_q[ i ]	f(dfParBits)
DfDeltaQ[ i ] = df_delta_q[ i ] - ( 1 << (dfParBits - 1) )	
} else {	
DfDeltaQ[ i ] = (i == 1) ? DfDeltaQ[ 0 ] : 0	
}	
} else {	
DfDeltaQ[ i ] = 0	
}	
}	
}
    */
		private AomArray<int> df_delta_q_present = new AomArray<int>();
		public AomArray<int> _DfDeltaqPresent { get { return df_delta_q_present; } set { df_delta_q_present = value; } }
		private AomArray<int> df_delta_q = new AomArray<int>();
		public AomArray<int> _DfDeltaq { get { return df_delta_q; } set { df_delta_q = value; } }
		private AomArray<int> DfDeltaQ = new AomArray<int>();
		public AomArray<int> _DfDeltaQ { get { return DfDeltaQ; } set { DfDeltaQ = value; } }

        private void DeblockingFilterParams()
        {
			int i = 0;
			int dfParBits = 0;

			if ((CodedLossless != 0))
			{
				apply_deblocking_filter[0] = 0;
				apply_deblocking_filter[1] = 0;
				return;
			}

			if (((enable_df_sub_pu != 0) && (FrameType == INTER_FRAME)))
			{
				stream.ReadFixed(1, out this.allow_df_sub_pu, "allow_df_sub_pu"); 
			}
			else 
			{
				allow_df_sub_pu = 0;
			}

			if ((mfh_deblocking_filter_update[cur_mfh_id] != 0))
			{
				apply_deblocking_filter[0] = mfh_apply_deblocking_filter[cur_mfh_id][0];
				apply_deblocking_filter[1] = mfh_apply_deblocking_filter[cur_mfh_id][1];
				apply_deblocking_filter[2] = 0;
				apply_deblocking_filter[3] = 0;

				if ((NumPlanes > 1))
				{

					if (((apply_deblocking_filter[0] != 0) || (apply_deblocking_filter[1] != 0)))
					{
						apply_deblocking_filter[2] = mfh_apply_deblocking_filter[cur_mfh_id][2];
						apply_deblocking_filter[3] = mfh_apply_deblocking_filter[cur_mfh_id][3];
					}
				}
			}
			else 
			{
				stream.ReadFixed(1, out this.apply_deblocking_filter[0], "apply_deblocking_filter"); 
				stream.ReadFixed(1, out this.apply_deblocking_filter[1], "apply_deblocking_filter"); 
				apply_deblocking_filter[2] = 0;
				apply_deblocking_filter[3] = 0;

				if ((NumPlanes > 1))
				{

					if (((apply_deblocking_filter[0] != 0) || (apply_deblocking_filter[1] != 0)))
					{
						stream.ReadFixed(1, out this.apply_deblocking_filter[2], "apply_deblocking_filter"); 
						stream.ReadFixed(1, out this.apply_deblocking_filter[3], "apply_deblocking_filter"); 
					}
				}
			}

			for (i = 0; (i < 4); i++)
			{

				if ((apply_deblocking_filter[i] != 0))
				{
					stream.ReadFixed(1, out this.df_delta_q_present[i], "df_delta_q_present"); 

					if ((df_delta_q_present[i] != 0))
					{
						dfParBits = (df_par_bits_minus_2 + 2);
						stream.ReadVariable(dfParBits, out this.df_delta_q[i], "df_delta_q"); 
						DfDeltaQ[i] = (df_delta_q[i] - (1 << (dfParBits - 1)));
					}
					else 
					{
						DfDeltaQ[i] = ((i == 1) ? DfDeltaQ[0] : 0);
					}
				}
				else 
				{
					DfDeltaQ[i] = 0;
				}
			}
        }

        private void WriteDeblockingFilterParams()
        {
			int i = 0;
			int dfParBits = 0;

			if ((CodedLossless != 0))
			{
				apply_deblocking_filter[0] = 0;
				apply_deblocking_filter[1] = 0;
				return;
			}

			if (((enable_df_sub_pu != 0) && (FrameType == INTER_FRAME)))
			{
				this.allow_df_sub_pu = stream.Pick("allow_df_sub_pu", _original != null ? _original.allow_df_sub_pu : this.allow_df_sub_pu, _edited != null ? _edited.allow_df_sub_pu : _original != null ? _original.allow_df_sub_pu : this.allow_df_sub_pu);
				stream.WriteFixed(1, this.allow_df_sub_pu, "allow_df_sub_pu"); 
			}
			else 
			{
				allow_df_sub_pu = 0;
			}

			if ((mfh_deblocking_filter_update[cur_mfh_id] != 0))
			{
				apply_deblocking_filter[0] = mfh_apply_deblocking_filter[cur_mfh_id][0];
				apply_deblocking_filter[1] = mfh_apply_deblocking_filter[cur_mfh_id][1];
				apply_deblocking_filter[2] = 0;
				apply_deblocking_filter[3] = 0;

				if ((NumPlanes > 1))
				{

					if (((apply_deblocking_filter[0] != 0) || (apply_deblocking_filter[1] != 0)))
					{
						apply_deblocking_filter[2] = mfh_apply_deblocking_filter[cur_mfh_id][2];
						apply_deblocking_filter[3] = mfh_apply_deblocking_filter[cur_mfh_id][3];
					}
				}
			}
			else 
			{
				this.apply_deblocking_filter[0] = stream.Pick("apply_deblocking_filter", _original != null ? _original.apply_deblocking_filter[0] : this.apply_deblocking_filter[0], _edited != null ? _edited.apply_deblocking_filter[0] : _original != null ? _original.apply_deblocking_filter[0] : this.apply_deblocking_filter[0]);
				stream.WriteFixed(1, this.apply_deblocking_filter[0], "apply_deblocking_filter"); 
				this.apply_deblocking_filter[1] = stream.Pick("apply_deblocking_filter", _original != null ? _original.apply_deblocking_filter[1] : this.apply_deblocking_filter[1], _edited != null ? _edited.apply_deblocking_filter[1] : _original != null ? _original.apply_deblocking_filter[1] : this.apply_deblocking_filter[1]);
				stream.WriteFixed(1, this.apply_deblocking_filter[1], "apply_deblocking_filter"); 
				apply_deblocking_filter[2] = 0;
				apply_deblocking_filter[3] = 0;

				if ((NumPlanes > 1))
				{

					if (((apply_deblocking_filter[0] != 0) || (apply_deblocking_filter[1] != 0)))
					{
						this.apply_deblocking_filter[2] = stream.Pick("apply_deblocking_filter", _original != null ? _original.apply_deblocking_filter[2] : this.apply_deblocking_filter[2], _edited != null ? _edited.apply_deblocking_filter[2] : _original != null ? _original.apply_deblocking_filter[2] : this.apply_deblocking_filter[2]);
						stream.WriteFixed(1, this.apply_deblocking_filter[2], "apply_deblocking_filter"); 
						this.apply_deblocking_filter[3] = stream.Pick("apply_deblocking_filter", _original != null ? _original.apply_deblocking_filter[3] : this.apply_deblocking_filter[3], _edited != null ? _edited.apply_deblocking_filter[3] : _original != null ? _original.apply_deblocking_filter[3] : this.apply_deblocking_filter[3]);
						stream.WriteFixed(1, this.apply_deblocking_filter[3], "apply_deblocking_filter"); 
					}
				}
			}

			for (i = 0; (i < 4); i++)
			{

				if ((apply_deblocking_filter[i] != 0))
				{
					this.df_delta_q_present[i] = stream.Pick("df_delta_q_present", _original != null ? _original.df_delta_q_present[i] : this.df_delta_q_present[i], _edited != null ? _edited.df_delta_q_present[i] : _original != null ? _original.df_delta_q_present[i] : this.df_delta_q_present[i]);
					stream.WriteFixed(1, this.df_delta_q_present[i], "df_delta_q_present"); 

					if ((df_delta_q_present[i] != 0))
					{
						dfParBits = (df_par_bits_minus_2 + 2);
						this.df_delta_q[i] = stream.Pick("df_delta_q", _original != null ? _original.df_delta_q[i] : this.df_delta_q[i], _edited != null ? _edited.df_delta_q[i] : _original != null ? _original.df_delta_q[i] : this.df_delta_q[i]);
						stream.WriteVariable(dfParBits, this.df_delta_q[i], "df_delta_q"); 
						DfDeltaQ[i] = (df_delta_q[i] - (1 << (dfParBits - 1)));
					}
					else 
					{
						DfDeltaQ[i] = ((i == 1) ? DfDeltaQ[0] : 0);
					}
				}
				else 
				{
					DfDeltaQ[i] = 0;
				}
			}
        }

    /*
quantization_params() {
n = BitDepth == 8 ? 8 : 9	
base_q_idx	f(n)
DeltaQYDc = 0	
DeltaQUDc = 0	
DeltaQUAc = 0	
DeltaQVDc = 0	
DeltaQVAc = 0	
if ( TipFrameMode != TIP_FRAME_AS_OUTPUT && y_dc_delta_q_enabled ) {	
DeltaQYDc = read_delta_q()	
}	
if ( NumPlanes > 1 && (	
uv_ac_delta_q_enabled ||	
(TipFrameMode != TIP_FRAME_AS_OUTPUT && uv_dc_delta_q_enabled)	
) ) {	
if ( separate_uv_delta_q ) {	
diff_uv_delta	f(1)
} else {	
diff_uv_delta = 0	
}	
if ( TipFrameMode != TIP_FRAME_AS_OUTPUT && uv_dc_delta_q_enabled ) {	
DeltaQUDc = read_delta_q()	
}	
if ( uv_ac_delta_q_enabled ) {	
DeltaQUAc = read_delta_q()	
}	
if ( equal_ac_dc_q ) {	
DeltaQUDc = DeltaQUAc	
}	
if ( diff_uv_delta ) {	
if ( TipFrameMode != TIP_FRAME_AS_OUTPUT &&	
uv_dc_delta_q_enabled ) {	
DeltaQVDc = read_delta_q()	
}	
if ( uv_ac_delta_q_enabled ) {	
DeltaQVAc = read_delta_q()	
}	
if ( equal_ac_dc_q ) {	
DeltaQVDc = DeltaQVAc	
}	
} else {	
DeltaQVDc = DeltaQUDc	
DeltaQVAc = DeltaQUAc	
}	
}	
}
    */
		private int DeltaQYDc;
		public int _DeltaQYDc { get { return DeltaQYDc; } set { DeltaQYDc = value; } }
		private int DeltaQUDc;
		public int _DeltaQUDc { get { return DeltaQUDc; } set { DeltaQUDc = value; } }
		private int DeltaQVDc;
		public int _DeltaQVDc { get { return DeltaQVDc; } set { DeltaQVDc = value; } }
		private int diff_uv_delta;
		public int _DiffUvDelta { get { return diff_uv_delta; } set { diff_uv_delta = value; } }

        private void QuantizationParams()
        {
			int n = 0;
			n = ((BitDepth == 8) ? 8 : 9);
			stream.ReadVariable(n, out this.base_q_idx, "base_q_idx"); 
			DeltaQYDc = 0;
			DeltaQUDc = 0;
			DeltaQUAc = 0;
			DeltaQVDc = 0;
			DeltaQVAc = 0;

			if (((TipFrameMode != TIP_FRAME_AS_OUTPUT) && (y_dc_delta_q_enabled != 0)))
			{
				DeltaQYDc = ReadDeltaq();
			}

			if (((NumPlanes > 1) && ((uv_ac_delta_q_enabled != 0) || ((TipFrameMode != TIP_FRAME_AS_OUTPUT) && (uv_dc_delta_q_enabled != 0)))))
			{

				if ((separate_uv_delta_q != 0))
				{
					stream.ReadFixed(1, out this.diff_uv_delta, "diff_uv_delta"); 
				}
				else 
				{
					diff_uv_delta = 0;
				}

				if (((TipFrameMode != TIP_FRAME_AS_OUTPUT) && (uv_dc_delta_q_enabled != 0)))
				{
					DeltaQUDc = ReadDeltaq();
				}

				if ((uv_ac_delta_q_enabled != 0))
				{
					DeltaQUAc = ReadDeltaq();
				}

				if ((equal_ac_dc_q != 0))
				{
					DeltaQUDc = DeltaQUAc;
				}

				if ((diff_uv_delta != 0))
				{

					if (((TipFrameMode != TIP_FRAME_AS_OUTPUT) && (uv_dc_delta_q_enabled != 0)))
					{
						DeltaQVDc = ReadDeltaq();
					}

					if ((uv_ac_delta_q_enabled != 0))
					{
						DeltaQVAc = ReadDeltaq();
					}

					if ((equal_ac_dc_q != 0))
					{
						DeltaQVDc = DeltaQVAc;
					}
				}
				else 
				{
					DeltaQVDc = DeltaQUDc;
					DeltaQVAc = DeltaQUAc;
				}
			}
        }

        private void WriteQuantizationParams()
        {
			int n = 0;
			n = ((BitDepth == 8) ? 8 : 9);
			this.base_q_idx = stream.Pick("base_q_idx", _original != null ? _original.base_q_idx : this.base_q_idx, _edited != null ? _edited.base_q_idx : _original != null ? _original.base_q_idx : this.base_q_idx);
			stream.WriteVariable(n, this.base_q_idx, "base_q_idx"); 
			DeltaQYDc = 0;
			DeltaQUDc = 0;
			DeltaQUAc = 0;
			DeltaQVDc = 0;
			DeltaQVAc = 0;

			if (((TipFrameMode != TIP_FRAME_AS_OUTPUT) && (y_dc_delta_q_enabled != 0)))
			{
				DeltaQYDc = WriteReadDeltaq();
			}

			if (((NumPlanes > 1) && ((uv_ac_delta_q_enabled != 0) || ((TipFrameMode != TIP_FRAME_AS_OUTPUT) && (uv_dc_delta_q_enabled != 0)))))
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

				if (((TipFrameMode != TIP_FRAME_AS_OUTPUT) && (uv_dc_delta_q_enabled != 0)))
				{
					DeltaQUDc = WriteReadDeltaq();
				}

				if ((uv_ac_delta_q_enabled != 0))
				{
					DeltaQUAc = WriteReadDeltaq();
				}

				if ((equal_ac_dc_q != 0))
				{
					DeltaQUDc = DeltaQUAc;
				}

				if ((diff_uv_delta != 0))
				{

					if (((TipFrameMode != TIP_FRAME_AS_OUTPUT) && (uv_dc_delta_q_enabled != 0)))
					{
						DeltaQVDc = WriteReadDeltaq();
					}

					if ((uv_ac_delta_q_enabled != 0))
					{
						DeltaQVAc = WriteReadDeltaq();
					}

					if ((equal_ac_dc_q != 0))
					{
						DeltaQVDc = DeltaQVAc;
					}
				}
				else 
				{
					DeltaQVDc = DeltaQUDc;
					DeltaQVAc = DeltaQUAc;
				}
			}
        }

    /*
setup_qm_params() {
using_qmatrix	f(1)
if ( using_qmatrix ) {	
if ( segmentation_enabled ) {	
pic_qm_num_minus_1	f(2)
} else {	
pic_qm_num_minus_1 = 0	
}	
qmNum = pic_qm_num_minus_1 + 1	
for ( i = 0; i < qmNum; i++ ) {	
qm_y[ i ]	f(4)
if ( NumPlanes > 1 ) {	
qm_uv_same_as_y	f(1)
if ( qm_uv_same_as_y ) {	
qm_u[ i ] = qm_y [ i ]	
qm_v[ i ] = qm_y [ i ]	
} else {	
qm_u[ i ]	f(4)
if ( !separate_uv_delta_q ) {	
qm_v[ i ] = qm_u[ i ]	
} else {	
qm_v[ i ]	f(4)
}	
}	
}	
}	
}	
}
    */
		private int using_qmatrix;
		public int _UsingQmatrix { get { return using_qmatrix; } set { using_qmatrix = value; } }
		private int pic_qm_num_minus_1;
		public int _PicQmNumMinus1 { get { return pic_qm_num_minus_1; } set { pic_qm_num_minus_1 = value; } }
		private AomArray<int> qm_y = new AomArray<int>();
		public AomArray<int> _Qmy { get { return qm_y; } set { qm_y = value; } }
		private int qm_uv_same_as_y;
		public int _QmUvSameAsy { get { return qm_uv_same_as_y; } set { qm_uv_same_as_y = value; } }
		private AomArray<int> qm_u = new AomArray<int>();
		public AomArray<int> _Qmu { get { return qm_u; } set { qm_u = value; } }
		private AomArray<int> qm_v = new AomArray<int>();
		public AomArray<int> _Qmv { get { return qm_v; } set { qm_v = value; } }

        private void SetupQmParams()
        {
			int i = 0;
			int qmNum = 0;
			stream.ReadFixed(1, out this.using_qmatrix, "using_qmatrix"); 

			if ((using_qmatrix != 0))
			{

				if ((segmentation_enabled != 0))
				{
					stream.ReadFixed(2, out this.pic_qm_num_minus_1, "pic_qm_num_minus_1"); 
				}
				else 
				{
					pic_qm_num_minus_1 = 0;
				}
				qmNum = (pic_qm_num_minus_1 + 1);

				for (i = 0; (i < qmNum); i++)
				{
					stream.ReadFixed(4, out this.qm_y[i], "qm_y"); 

					if ((NumPlanes > 1))
					{
						stream.ReadFixed(1, out this.qm_uv_same_as_y, "qm_uv_same_as_y"); 

						if ((qm_uv_same_as_y != 0))
						{
							qm_u[i] = qm_y[i];
							qm_v[i] = qm_y[i];
						}
						else 
						{
							stream.ReadFixed(4, out this.qm_u[i], "qm_u"); 

							if (!(separate_uv_delta_q != 0))
							{
								qm_v[i] = qm_u[i];
							}
							else 
							{
								stream.ReadFixed(4, out this.qm_v[i], "qm_v"); 
							}
						}
					}
				}
			}
        }

        private void WriteSetupQmParams()
        {
			int i = 0;
			int qmNum = 0;
			this.using_qmatrix = stream.Pick("using_qmatrix", _original != null ? _original.using_qmatrix : this.using_qmatrix, _edited != null ? _edited.using_qmatrix : _original != null ? _original.using_qmatrix : this.using_qmatrix);
			stream.WriteFixed(1, this.using_qmatrix, "using_qmatrix"); 

			if ((using_qmatrix != 0))
			{

				if ((segmentation_enabled != 0))
				{
					this.pic_qm_num_minus_1 = stream.Pick("pic_qm_num_minus_1", _original != null ? _original.pic_qm_num_minus_1 : this.pic_qm_num_minus_1, _edited != null ? _edited.pic_qm_num_minus_1 : _original != null ? _original.pic_qm_num_minus_1 : this.pic_qm_num_minus_1);
					stream.WriteFixed(2, this.pic_qm_num_minus_1, "pic_qm_num_minus_1"); 
				}
				else 
				{
					pic_qm_num_minus_1 = 0;
				}
				qmNum = (pic_qm_num_minus_1 + 1);

				for (i = 0; (i < qmNum); i++)
				{
					this.qm_y[i] = stream.Pick("qm_y", _original != null ? _original.qm_y[i] : this.qm_y[i], _edited != null ? _edited.qm_y[i] : _original != null ? _original.qm_y[i] : this.qm_y[i]);
					stream.WriteFixed(4, this.qm_y[i], "qm_y"); 

					if ((NumPlanes > 1))
					{
						this.qm_uv_same_as_y = stream.Pick("qm_uv_same_as_y", _original != null ? _original.qm_uv_same_as_y : this.qm_uv_same_as_y, _edited != null ? _edited.qm_uv_same_as_y : _original != null ? _original.qm_uv_same_as_y : this.qm_uv_same_as_y);
						stream.WriteFixed(1, this.qm_uv_same_as_y, "qm_uv_same_as_y"); 

						if ((qm_uv_same_as_y != 0))
						{
							qm_u[i] = qm_y[i];
							qm_v[i] = qm_y[i];
						}
						else 
						{
							this.qm_u[i] = stream.Pick("qm_u", _original != null ? _original.qm_u[i] : this.qm_u[i], _edited != null ? _edited.qm_u[i] : _original != null ? _original.qm_u[i] : this.qm_u[i]);
							stream.WriteFixed(4, this.qm_u[i], "qm_u"); 

							if (!(separate_uv_delta_q != 0))
							{
								qm_v[i] = qm_u[i];
							}
							else 
							{
								this.qm_v[i] = stream.Pick("qm_v", _original != null ? _original.qm_v[i] : this.qm_v[i], _edited != null ? _edited.qm_v[i] : _original != null ? _original.qm_v[i] : this.qm_v[i]);
								stream.WriteFixed(4, this.qm_v[i], "qm_v"); 
							}
						}
					}
				}
			}
        }

    /*
read_delta_q() {
delta_coded	f(1)
if ( delta_coded ) {	
delta_q	su(7)
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
				stream.ReadSignedIntVar(7, out this.delta_q, "delta_q"); 
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
				stream.WriteSignedIntVar(7, this.delta_q, "delta_q"); 
			}
			else 
			{
				delta_q = 0;
			}
			return delta_q;
        }

    /*
segmentation_params() {
segmentation_enabled	f(1)
if ( segmentation_enabled == 1 ) {	
if ( cur_mfh_id > 0 && mfh_seg_info_present_flag[ cur_mfh_id ] ) {	
haveSegParams = mfh_ext_seg_flag[ cur_mfh_id ] == enable_ext_seg	
allowChange = haveSegParams && mfh_allow_seg_info_change[cur_mfh_id]	
mfhId = cur_mfh_id	
} else if ( seq_seg_info_present_flag ) {	
haveSegParams = 1	
allowChange = seq_allow_seg_info_change	
mfhId = 0	
} else {	
haveSegParams = 0	
allowChange = 0	
}	
if ( allowChange ) {	
reuse_seg_info	f(1)
} else {	
reuse_seg_info = haveSegParams	
}	
if ( reuse_seg_info ) {	
for ( i = 0; i < MAX_SEGMENTS; i++ ) {	
for ( j = 0; j < SEG_LVL_MAX; j++ ) {	
if ( mfhId == 0 ) {	
FeatureData[ i ][ j ] = SeqFeatureData[ i ][ j ]	
FeatureEnabled[ i ][ j ] = SeqFeatureEnabled[ i ][ j ]	
} else {	
FeatureData[ i ][ j ] =	
MfhFeatureData[ mfhId ][ i ][ j ]	
FeatureEnabled[ i ][ j ] =	
MfhFeatureEnabled[ mfhId ][ i ][ j ]	
}	
}	
}	
} else {	
(FeatureEnabled, FeatureData) = seg_info( MaxSegments )	
}	
if ( DerivedPrimaryRefFrame == PRIMARY_REF_NONE ) {	
segmentation_update_map = 1	
segmentation_temporal_update = 0	
} else {	
segmentation_update_map	f(1)
if ( segmentation_update_map == 1 && FrameType != SWITCH_FRAME ) {	
segmentation_temporal_update	f(1)
} else {	
segmentation_temporal_update = 0	
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
for ( i = 0; i < MaxSegments; i++ ) {	
for ( j = 0; j < SEG_LVL_MAX; j++ ) {	
if ( FeatureEnabled[ i ][ j ] ) {	
LastActiveSegId = i	
if ( j >= SEG_LVL_SKIP ) {	
SegIdPreSkip = 1	
}	
}	
}	
}	
}
    */
		private int reuse_seg_info;
		public int _ReuseSegInfo { get { return reuse_seg_info; } set { reuse_seg_info = value; } }
		private int segmentation_update_map;
		public int _SegmentationUpdateMap { get { return segmentation_update_map; } set { segmentation_update_map = value; } }
		private int segmentation_temporal_update;
		public int _SegmentationTemporalUpdate { get { return segmentation_temporal_update; } set { segmentation_temporal_update = value; } }
		private int SegIdPreSkip;
		public int _SegIdPreSkip { get { return SegIdPreSkip; } set { SegIdPreSkip = value; } }
		private int LastActiveSegId;
		public int _LastActiveSegId { get { return LastActiveSegId; } set { LastActiveSegId = value; } }

        private void SegmentationParams()
        {
			int i = 0;
			int j = 0;
			int haveSegParams = 0;
			int allowChange = 0;
			int mfhId = 0;
			stream.ReadFixed(1, out this.segmentation_enabled, "segmentation_enabled"); 

			if ((segmentation_enabled == 1))
			{

				if (((cur_mfh_id > 0) && (mfh_seg_info_present_flag[cur_mfh_id] != 0)))
				{
					haveSegParams = ((mfh_ext_seg_flag[cur_mfh_id] == enable_ext_seg) ? 1 : 0);
					allowChange = (((haveSegParams != 0) && (mfh_allow_seg_info_change[cur_mfh_id] != 0)) ? 1 : 0);
					mfhId = cur_mfh_id;
				}
				else if ((seq_seg_info_present_flag != 0))
				{
					haveSegParams = 1;
					allowChange = seq_allow_seg_info_change;
					mfhId = 0;
				}
				else 
				{
					haveSegParams = 0;
					allowChange = 0;
				}

				if ((allowChange != 0))
				{
					stream.ReadFixed(1, out this.reuse_seg_info, "reuse_seg_info"); 
				}
				else 
				{
					reuse_seg_info = haveSegParams;
				}

				if ((reuse_seg_info != 0))
				{

					for (i = 0; (i < MAX_SEGMENTS); i++)
					{

						for (j = 0; (j < SEG_LVL_MAX); j++)
						{

							if ((mfhId == 0))
							{
								FeatureData[i][j] = SeqFeatureData[i][j];
								FeatureEnabled[i][j] = SeqFeatureEnabled[i][j];
							}
							else 
							{
								FeatureData[i][j] = MfhFeatureData[mfhId][i][j];
								FeatureEnabled[i][j] = MfhFeatureEnabled[mfhId][i][j];
							}
						}
					}
				}
				else 
				{
					(FeatureEnabled, FeatureData) = SegInfo(MaxSegments);
				}

				if ((DerivedPrimaryRefFrame == PRIMARY_REF_NONE))
				{
					segmentation_update_map = 1;
					segmentation_temporal_update = 0;
				}
				else 
				{
					stream.ReadFixed(1, out this.segmentation_update_map, "segmentation_update_map"); 

					if (((segmentation_update_map == 1) && (FrameType != SWITCH_FRAME)))
					{
						stream.ReadFixed(1, out this.segmentation_temporal_update, "segmentation_temporal_update"); 
					}
					else 
					{
						segmentation_temporal_update = 0;
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

			for (i = 0; (i < MaxSegments); i++)
			{

				for (j = 0; (j < SEG_LVL_MAX); j++)
				{

					if ((FeatureEnabled[i][j] != 0))
					{
						LastActiveSegId = i;

						if ((j >= SEG_LVL_SKIP))
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
			int haveSegParams = 0;
			int allowChange = 0;
			int mfhId = 0;
			this.segmentation_enabled = stream.Pick("segmentation_enabled", _original != null ? _original.segmentation_enabled : this.segmentation_enabled, _edited != null ? _edited.segmentation_enabled : _original != null ? _original.segmentation_enabled : this.segmentation_enabled);
			stream.WriteFixed(1, this.segmentation_enabled, "segmentation_enabled"); 

			if ((segmentation_enabled == 1))
			{

				if (((cur_mfh_id > 0) && (mfh_seg_info_present_flag[cur_mfh_id] != 0)))
				{
					haveSegParams = ((mfh_ext_seg_flag[cur_mfh_id] == enable_ext_seg) ? 1 : 0);
					allowChange = (((haveSegParams != 0) && (mfh_allow_seg_info_change[cur_mfh_id] != 0)) ? 1 : 0);
					mfhId = cur_mfh_id;
				}
				else if ((seq_seg_info_present_flag != 0))
				{
					haveSegParams = 1;
					allowChange = seq_allow_seg_info_change;
					mfhId = 0;
				}
				else 
				{
					haveSegParams = 0;
					allowChange = 0;
				}

				if ((allowChange != 0))
				{
					this.reuse_seg_info = stream.Pick("reuse_seg_info", _original != null ? _original.reuse_seg_info : this.reuse_seg_info, _edited != null ? _edited.reuse_seg_info : _original != null ? _original.reuse_seg_info : this.reuse_seg_info);
					stream.WriteFixed(1, this.reuse_seg_info, "reuse_seg_info"); 
				}
				else 
				{
					reuse_seg_info = haveSegParams;
				}

				if ((reuse_seg_info != 0))
				{

					for (i = 0; (i < MAX_SEGMENTS); i++)
					{

						for (j = 0; (j < SEG_LVL_MAX); j++)
						{

							if ((mfhId == 0))
							{
								FeatureData[i][j] = SeqFeatureData[i][j];
								FeatureEnabled[i][j] = SeqFeatureEnabled[i][j];
							}
							else 
							{
								FeatureData[i][j] = MfhFeatureData[mfhId][i][j];
								FeatureEnabled[i][j] = MfhFeatureEnabled[mfhId][i][j];
							}
						}
					}
				}
				else 
				{
					(FeatureEnabled, FeatureData) = WriteSegInfo(MaxSegments);
				}

				if ((DerivedPrimaryRefFrame == PRIMARY_REF_NONE))
				{
					segmentation_update_map = 1;
					segmentation_temporal_update = 0;
				}
				else 
				{
					this.segmentation_update_map = stream.Pick("segmentation_update_map", _original != null ? _original.segmentation_update_map : this.segmentation_update_map, _edited != null ? _edited.segmentation_update_map : _original != null ? _original.segmentation_update_map : this.segmentation_update_map);
					stream.WriteFixed(1, this.segmentation_update_map, "segmentation_update_map"); 

					if (((segmentation_update_map == 1) && (FrameType != SWITCH_FRAME)))
					{
						this.segmentation_temporal_update = stream.Pick("segmentation_temporal_update", _original != null ? _original.segmentation_temporal_update : this.segmentation_temporal_update, _edited != null ? _edited.segmentation_temporal_update : _original != null ? _original.segmentation_temporal_update : this.segmentation_temporal_update);
						stream.WriteFixed(1, this.segmentation_temporal_update, "segmentation_temporal_update"); 
					}
					else 
					{
						segmentation_temporal_update = 0;
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

			for (i = 0; (i < MaxSegments); i++)
			{

				for (j = 0; (j < SEG_LVL_MAX); j++)
				{

					if ((FeatureEnabled[i][j] != 0))
					{
						LastActiveSegId = i;

						if ((j >= SEG_LVL_SKIP))
						{
							SegIdPreSkip = 1;
						}
					}
				}
			}
        }

    /*
tile_info () {
sb4x4 = Num_4x4_Blocks_Wide[ SbSize ]	
sbShift = Mi_Width_Log2[ SbSize ]	
sbCols = ( MiCols + sb4x4 - 1 ) >> sbShift	
sbRows = ( MiRows + sb4x4 - 1 ) >> sbShift	
if ( IsBridge ) {	
haveTileParams = 0	
} else {	
haveTileParams = seq_tile_info_present_flag	
}	
if ( haveTileParams &&	
( SeqUniformTileSpacingFlag ? (	
uniform_eligible( SeqTileRowsLog2, sbRows) &&	
uniform_eligible( SeqTileColsLog2, sbCols) ) :	
( SeqSbCols == sbCols && SeqSbRows == sbRows ) ) ) {	
if ( allow_tile_info_change ) {	
reuse_tile_info	f(1)
} else {	
reuse_tile_info = 1	
}	
} else {	
reuse_tile_info = 0	
}	
seqSbSize = get_seq_sb_size()	
if ( reuse_tile_info ) {	
( sbRowStarts, TileRows, TileRowsLog2, sbColStarts, TileCols,	
TileColsLog2, sbShift2) = reuse_tile_params(SeqUniformTileSpacingFlag,	
SeqSbRowStarts, SeqTileRows, SeqTileRowsLog2,	
SeqSbColStarts, SeqTileCols, SeqTileColsLog2, seqSbSize, SbSize )	
} else {	
( sbRowStarts, sbRows, TileRows, TileRowsLog2, sbColStarts, sbCols,	
TileCols, TileColsLog2, uniformSpacing, sbShift2) = tile_params(	
FrameWidth, FrameHeight, seqSbSize, SbSize, IsBridge )	
}	
for ( i = 0; i < TileCols; i++ ) {	
MiColStarts[ i ] = sbColStarts[ i ] << sbShift2	
}	
for ( i = 0; i < TileRows; i++ ) {	
MiRowStarts[ i ] = sbRowStarts[ i ] << sbShift2	
}	
MiColStarts[ TileCols ] = MiCols	
MiRowStarts[ TileRows ] = MiRows	
if ( (TileCols > 1 || TileRows > 1) && !IsBridge &&	
TipFrameMode != TIP_FRAME_AS_OUTPUT ) {	
if ( !enable_avg_cdf || !avg_cdf_type ) {	
n = TileRowsLog2 + TileColsLog2	
context_update_tile_id	f(n)
}	
tile_size_bytes_minus_1	f(2)
TileSizeBytes = tile_size_bytes_minus_1 + 1	
} else {	
context_update_tile_id = 0	
}	
}
    */
		private int reuse_tile_info;
		public int _ReuseTileInfo { get { return reuse_tile_info; } set { reuse_tile_info = value; } }
		private int TileRows;
		public int _TileRows { get { return TileRows; } set { TileRows = value; } }
		private int TileRowsLog2;
		public int _TileRowsLog2 { get { return TileRowsLog2; } set { TileRowsLog2 = value; } }
		private int TileCols;
		public int _TileCols { get { return TileCols; } set { TileCols = value; } }
		private int TileColsLog2;
		public int _TileColsLog2 { get { return TileColsLog2; } set { TileColsLog2 = value; } }
		private AomArray<int> MiColStarts = new AomArray<int>();
		public AomArray<int> _MiColStarts { get { return MiColStarts; } set { MiColStarts = value; } }
		private AomArray<int> MiRowStarts = new AomArray<int>();
		public AomArray<int> _MiRowStarts { get { return MiRowStarts; } set { MiRowStarts = value; } }
		private int context_update_tile_id;
		public int _ContextUpdateTileId { get { return context_update_tile_id; } set { context_update_tile_id = value; } }
		private int tile_size_bytes_minus_1;
		public int _TileSizeBytesMinus1 { get { return tile_size_bytes_minus_1; } set { tile_size_bytes_minus_1 = value; } }
		private int TileSizeBytes;
		public int _TileSizeBytes { get { return TileSizeBytes; } set { TileSizeBytes = value; } }

        private void TileInfo()
        {
			int i = 0;
			int sb4x4 = 0;
			int sbShift = 0;
			int sbCols = 0;
			int sbRows = 0;
			int haveTileParams = 0;
			int seqSbSize = 0;
			AomArray<int> sbRowStarts = new AomArray<int>();
			AomArray<int> sbColStarts = new AomArray<int>();
			int sbShift2 = 0;
			int uniformSpacing = 0;
			int n = 0;
			sb4x4 = Num_4x4_Blocks_Wide[SbSize];
			sbShift = Mi_Width_Log2[SbSize];
			sbCols = (((MiCols + sb4x4) - 1) >> sbShift);
			sbRows = (((MiRows + sb4x4) - 1) >> sbShift);

			if ((IsBridge != 0))
			{
				haveTileParams = 0;
			}
			else 
			{
				haveTileParams = seq_tile_info_present_flag;
			}

			if (((haveTileParams != 0) && (((SeqUniformTileSpacingFlag != 0) ? (((UniformEligible(SeqTileRowsLog2, sbRows) != 0) && (UniformEligible(SeqTileColsLog2, sbCols) != 0)) ? 1 : 0) : (((SeqSbCols == sbCols) && (SeqSbRows == sbRows)) ? 1 : 0)) != 0)))
			{

				if ((allow_tile_info_change != 0))
				{
					stream.ReadFixed(1, out this.reuse_tile_info, "reuse_tile_info"); 
				}
				else 
				{
					reuse_tile_info = 1;
				}
			}
			else 
			{
				reuse_tile_info = 0;
			}
			seqSbSize = GetSeqSbSize();

			if ((reuse_tile_info != 0))
			{
				(sbRowStarts, TileRows, TileRowsLog2, sbColStarts, TileCols, TileColsLog2, sbShift2) = ReuseTileParams(SeqUniformTileSpacingFlag, SeqSbRowStarts, SeqTileRows, SeqTileRowsLog2, SeqSbColStarts, SeqTileCols, SeqTileColsLog2, seqSbSize, SbSize);
			}
			else 
			{
				(sbRowStarts, sbRows, TileRows, TileRowsLog2, sbColStarts, sbCols, TileCols, TileColsLog2, uniformSpacing, sbShift2) = TileParams(FrameWidth, FrameHeight, seqSbSize, SbSize, IsBridge);
			}

			for (i = 0; (i < TileCols); i++)
			{
				MiColStarts[i] = (sbColStarts[i] << sbShift2);
			}

			for (i = 0; (i < TileRows); i++)
			{
				MiRowStarts[i] = (sbRowStarts[i] << sbShift2);
			}
			MiColStarts[TileCols] = MiCols;
			MiRowStarts[TileRows] = MiRows;

			if (((((TileCols > 1) || (TileRows > 1)) && !(IsBridge != 0)) && (TipFrameMode != TIP_FRAME_AS_OUTPUT)))
			{

				if ((!(enable_avg_cdf != 0) || !(avg_cdf_type != 0)))
				{
					n = (TileRowsLog2 + TileColsLog2);
					stream.ReadVariable(n, out this.context_update_tile_id, "context_update_tile_id"); 
				}
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
			int i = 0;
			int sb4x4 = 0;
			int sbShift = 0;
			int sbCols = 0;
			int sbRows = 0;
			int haveTileParams = 0;
			int seqSbSize = 0;
			AomArray<int> sbRowStarts = new AomArray<int>();
			AomArray<int> sbColStarts = new AomArray<int>();
			int sbShift2 = 0;
			int uniformSpacing = 0;
			int n = 0;
			sb4x4 = Num_4x4_Blocks_Wide[SbSize];
			sbShift = Mi_Width_Log2[SbSize];
			sbCols = (((MiCols + sb4x4) - 1) >> sbShift);
			sbRows = (((MiRows + sb4x4) - 1) >> sbShift);

			if ((IsBridge != 0))
			{
				haveTileParams = 0;
			}
			else 
			{
				haveTileParams = seq_tile_info_present_flag;
			}

			if (((haveTileParams != 0) && (((SeqUniformTileSpacingFlag != 0) ? (((UniformEligible(SeqTileRowsLog2, sbRows) != 0) && (UniformEligible(SeqTileColsLog2, sbCols) != 0)) ? 1 : 0) : (((SeqSbCols == sbCols) && (SeqSbRows == sbRows)) ? 1 : 0)) != 0)))
			{

				if ((allow_tile_info_change != 0))
				{
					this.reuse_tile_info = stream.Pick("reuse_tile_info", _original != null ? _original.reuse_tile_info : this.reuse_tile_info, _edited != null ? _edited.reuse_tile_info : _original != null ? _original.reuse_tile_info : this.reuse_tile_info);
					stream.WriteFixed(1, this.reuse_tile_info, "reuse_tile_info"); 
				}
				else 
				{
					reuse_tile_info = 1;
				}
			}
			else 
			{
				reuse_tile_info = 0;
			}
			seqSbSize = GetSeqSbSize();

			if ((reuse_tile_info != 0))
			{
				(sbRowStarts, TileRows, TileRowsLog2, sbColStarts, TileCols, TileColsLog2, sbShift2) = ReuseTileParams(SeqUniformTileSpacingFlag, SeqSbRowStarts, SeqTileRows, SeqTileRowsLog2, SeqSbColStarts, SeqTileCols, SeqTileColsLog2, seqSbSize, SbSize);
			}
			else 
			{
				(sbRowStarts, sbRows, TileRows, TileRowsLog2, sbColStarts, sbCols, TileCols, TileColsLog2, uniformSpacing, sbShift2) = WriteTileParams(FrameWidth, FrameHeight, seqSbSize, SbSize, IsBridge);
			}

			for (i = 0; (i < TileCols); i++)
			{
				MiColStarts[i] = (sbColStarts[i] << sbShift2);
			}

			for (i = 0; (i < TileRows); i++)
			{
				MiRowStarts[i] = (sbRowStarts[i] << sbShift2);
			}
			MiColStarts[TileCols] = MiCols;
			MiRowStarts[TileRows] = MiRows;

			if (((((TileCols > 1) || (TileRows > 1)) && !(IsBridge != 0)) && (TipFrameMode != TIP_FRAME_AS_OUTPUT)))
			{

				if ((!(enable_avg_cdf != 0) || !(avg_cdf_type != 0)))
				{
					n = (TileRowsLog2 + TileColsLog2);
					this.context_update_tile_id = stream.Pick("context_update_tile_id", _original != null ? _original.context_update_tile_id : this.context_update_tile_id, _edited != null ? _edited.context_update_tile_id : _original != null ? _original.context_update_tile_id : this.context_update_tile_id);
					stream.WriteVariable(n, this.context_update_tile_id, "context_update_tile_id"); 
				}
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
tile_params( frameWidth, frameHeight, uniformSbSize, sbSize, isBridge ) {
miCols = 2 * ( ( frameWidth + 7 ) >> 3 )	
miRows = 2 * ( ( frameHeight + 7 ) >> 3 )	
sb4x4 = Num_4x4_Blocks_Wide[ sbSize ]	
sbShift = Mi_Width_Log2[ sbSize ]	
sbCols = ( miCols + sb4x4 - 1 ) >> sbShift	
sbRows = ( miRows + sb4x4 - 1 ) >> sbShift	
if ( seq_level_idx != 31 ) {	
maxTileWidthSb = ( Tile_Width_Scaling_Factor[seq_tier][seq_level_idx] *	
MAX_TILE_WIDTH ) >> (sbShift + 4)	
maxTileAreaSb = ( Tile_Area_Scaling_Factor[seq_tier][seq_level_idx] *	
MAX_TILE_AREA ) >> ( 2 * (sbShift + 2) + 2 )	
} else {	
maxTileWidthSb = sbCols	
maxTileAreaSb = sbCols * sbRows	
}	
minLog2TileCols = tile_log2(maxTileWidthSb, sbCols)	
maxLog2TileCols = tile_log2(1, Min(sbCols, MAX_TILE_COLS))	
maxLog2TileRows = tile_log2(1, Min(sbRows, MAX_TILE_ROWS))	
minLog2Tiles = Max( minLog2TileCols,	
tile_log2(maxTileAreaSb, sbRows * sbCols))	
if ( isBridge ) {	
uniform_tile_spacing_flag = 1	
} else {	
uniform_tile_spacing_flag	f(1)
}	
if ( uniform_tile_spacing_flag ) {	
sbShift = Mi_Width_Log2[ uniformSbSize ]	
tileColsLog2 = minLog2TileCols	
if ( !isBridge ) {	
while ( tileColsLog2 < maxLog2TileCols ) {	
increment_tile_cols_log2	f(1)
if ( increment_tile_cols_log2 == 1 ) {	
tileColsLog2 += 1	
} else {	
break	
}	
}	
}	
(sbColStarts, tileCols) = uniform_spacing( tileColsLog2, miCols,	
uniformSbSize )	
tileColsLog2 = tile_log2(1, tileCols)	
minLog2TileRows = Max( minLog2Tiles - tileColsLog2, 0)	
tileRowsLog2 = minLog2TileRows	
if ( !isBridge ) {	
while ( tileRowsLog2 < maxLog2TileRows ) {	
increment_tile_rows_log2	f(1)
if ( increment_tile_rows_log2 == 1 ) {	
tileRowsLog2++	
} else {	
break	
}	
}	
}	
(sbRowStarts, tileRows) = uniform_spacing( tileRowsLog2, miRows,	
uniformSbSize )	
} else {	
widestTileSb = 1	
startSb = 0	
for ( i = 0; startSb < sbCols; i++ ) {	
sbColStarts[ i ] = startSb	
n = Min(sbCols - startSb, maxTileWidthSb)	
width_in_sbs_minus_1	ns(n)
sizeSb = width_in_sbs_minus_1 + 1	
widestTileSb = Max( sizeSb, widestTileSb )	
startSb += sizeSb	
}	
tileCols = i	
tileColsLog2 = tile_log2(1, tileCols)	
if (minLog2Tiles > 0) {	
maxTileAreaSb = (sbRows * sbCols) >> (minLog2Tiles + 1)	
} else {	
maxTileAreaSb = sbRows * sbCols	
}	
maxTileHeightSb = Max( maxTileAreaSb / widestTileSb, 1 )	
startSb = 0	
for ( i = 0; startSb < sbRows; i++ ) {	
sbRowStarts[ i ] = startSb	
maxHeight = Min(sbRows - startSb, maxTileHeightSb)	
height_in_sbs_minus_1	ns(maxHeight)
sizeSb = height_in_sbs_minus_1 + 1	
startSb = startSb + sizeSb	
}	
tileRows = i	
}	
tileRowsLog2 = tile_log2(1, tileRows)	
return ( sbRowStarts, sbRows, tileRows, tileRowsLog2, sbColStarts, sbCols,	
tileCols, tileColsLog2, uniform_tile_spacing_flag, sbShift)	
}
    */
		private int frameWidth;
		public int __FrameWidth { get { return frameWidth; } set { frameWidth = value; } }
		private int frameHeight;
		public int __FrameHeight { get { return frameHeight; } set { frameHeight = value; } }
		private int uniformSbSize;
		public int _UniformSbSize { get { return uniformSbSize; } set { uniformSbSize = value; } }
		private int sbSize;
		public int __SbSize { get { return sbSize; } set { sbSize = value; } }
		private int isBridge;
		public int __IsBridge { get { return isBridge; } set { isBridge = value; } }
		private int uniform_tile_spacing_flag;
		public int _UniformTileSpacingFlag { get { return uniform_tile_spacing_flag; } set { uniform_tile_spacing_flag = value; } }
		private int increment_tile_cols_log2;
		public int _IncrementTileColsLog2 { get { return increment_tile_cols_log2; } set { increment_tile_cols_log2 = value; } }
		private int increment_tile_rows_log2;
		public int _IncrementTileRowsLog2 { get { return increment_tile_rows_log2; } set { increment_tile_rows_log2 = value; } }
		private int width_in_sbs_minus_1;
		public int _WidthInSbsMinus1 { get { return width_in_sbs_minus_1; } set { width_in_sbs_minus_1 = value; } }
		private int height_in_sbs_minus_1;
		public int _HeightInSbsMinus1 { get { return height_in_sbs_minus_1; } set { height_in_sbs_minus_1 = value; } }

        private (AomArray<int>, int, int, int, AomArray<int>, int, int, int, int, int) TileParams(int frameWidth, int frameHeight, int uniformSbSize, int sbSize, int isBridge)
        {
			int i = 0;
			int miCols = 0;
			int miRows = 0;
			int sb4x4 = 0;
			int sbShift = 0;
			int sbCols = 0;
			int sbRows = 0;
			int maxTileWidthSb = 0;
			int maxTileAreaSb = 0;
			int minLog2TileCols = 0;
			int maxLog2TileCols = 0;
			int maxLog2TileRows = 0;
			int minLog2Tiles = 0;
			int tileColsLog2 = 0;
			AomArray<int> sbColStarts = new AomArray<int>();
			int tileCols = 0;
			int minLog2TileRows = 0;
			int tileRowsLog2 = 0;
			AomArray<int> sbRowStarts = new AomArray<int>();
			int tileRows = 0;
			int widestTileSb = 0;
			int startSb = 0;
			int n = 0;
			int sizeSb = 0;
			int maxTileHeightSb = 0;
			int maxHeight = 0;
			miCols = (2 * ((frameWidth + 7) >> 3));
			miRows = (2 * ((frameHeight + 7) >> 3));
			sb4x4 = Num_4x4_Blocks_Wide[sbSize];
			sbShift = Mi_Width_Log2[sbSize];
			sbCols = (((miCols + sb4x4) - 1) >> sbShift);
			sbRows = (((miRows + sb4x4) - 1) >> sbShift);

			if ((seq_level_idx != 31))
			{
				maxTileWidthSb = ((Tile_Width_Scaling_Factor[seq_tier][seq_level_idx] * MAX_TILE_WIDTH) >> (sbShift + 4));
				maxTileAreaSb = ((Tile_Area_Scaling_Factor[seq_tier][seq_level_idx] * MAX_TILE_AREA) >> ((2 * (sbShift + 2)) + 2));
			}
			else 
			{
				maxTileWidthSb = sbCols;
				maxTileAreaSb = (sbCols * sbRows);
			}
			minLog2TileCols = TileLog2(maxTileWidthSb, sbCols);
			maxLog2TileCols = TileLog2(1, Min(sbCols, MAX_TILE_COLS));
			maxLog2TileRows = TileLog2(1, Min(sbRows, MAX_TILE_ROWS));
			minLog2Tiles = Max(minLog2TileCols, TileLog2(maxTileAreaSb, (sbRows * sbCols)));

			if ((isBridge != 0))
			{
				uniform_tile_spacing_flag = 1;
			}
			else 
			{
				stream.ReadFixed(1, out this.uniform_tile_spacing_flag, "uniform_tile_spacing_flag"); 
			}

			if ((uniform_tile_spacing_flag != 0))
			{
				sbShift = Mi_Width_Log2[uniformSbSize];
				tileColsLog2 = minLog2TileCols;

				if (!(isBridge != 0))
				{

					while ((tileColsLog2 < maxLog2TileCols))
					{
						stream.ReadFixed(1, out this.increment_tile_cols_log2, "increment_tile_cols_log2"); 

						if ((increment_tile_cols_log2 == 1))
						{
							tileColsLog2 += 1;
						}
						else 
						{
							break;
						}
					}
				}
				(sbColStarts, tileCols) = UniformSpacing(tileColsLog2, miCols, uniformSbSize);
				tileColsLog2 = TileLog2(1, tileCols);
				minLog2TileRows = Max((minLog2Tiles - tileColsLog2), 0);
				tileRowsLog2 = minLog2TileRows;

				if (!(isBridge != 0))
				{

					while ((tileRowsLog2 < maxLog2TileRows))
					{
						stream.ReadFixed(1, out this.increment_tile_rows_log2, "increment_tile_rows_log2"); 

						if ((increment_tile_rows_log2 == 1))
						{
							tileRowsLog2++;
						}
						else 
						{
							break;
						}
					}
				}
				(sbRowStarts, tileRows) = UniformSpacing(tileRowsLog2, miRows, uniformSbSize);
			}
			else 
			{
				widestTileSb = 1;
				startSb = 0;

				for (i = 0; (startSb < sbCols); i++)
				{
					sbColStarts[i] = startSb;
					n = Min((sbCols - startSb), maxTileWidthSb);
					stream.Read_ns(n, out this.width_in_sbs_minus_1, "width_in_sbs_minus_1"); 
					sizeSb = (width_in_sbs_minus_1 + 1);
					widestTileSb = Max(sizeSb, widestTileSb);
					startSb += sizeSb;
				}
				tileCols = i;
				tileColsLog2 = TileLog2(1, tileCols);

				if ((minLog2Tiles > 0))
				{
					maxTileAreaSb = ((sbRows * sbCols) >> (minLog2Tiles + 1));
				}
				else 
				{
					maxTileAreaSb = (sbRows * sbCols);
				}
				maxTileHeightSb = Max((maxTileAreaSb / widestTileSb), 1);
				startSb = 0;

				for (i = 0; (startSb < sbRows); i++)
				{
					sbRowStarts[i] = startSb;
					maxHeight = Min((sbRows - startSb), maxTileHeightSb);
					stream.Read_ns(maxHeight, out this.height_in_sbs_minus_1, "height_in_sbs_minus_1"); 
					sizeSb = (height_in_sbs_minus_1 + 1);
					startSb = (startSb + sizeSb);
				}
				tileRows = i;
			}
			tileRowsLog2 = TileLog2(1, tileRows);
			return (sbRowStarts, sbRows, tileRows, tileRowsLog2, sbColStarts, sbCols, tileCols, tileColsLog2, uniform_tile_spacing_flag, sbShift);
        }

        private (AomArray<int>, int, int, int, AomArray<int>, int, int, int, int, int) WriteTileParams(int frameWidth, int frameHeight, int uniformSbSize, int sbSize, int isBridge)
        {
			int i = 0;
			int miCols = 0;
			int miRows = 0;
			int sb4x4 = 0;
			int sbShift = 0;
			int sbCols = 0;
			int sbRows = 0;
			int maxTileWidthSb = 0;
			int maxTileAreaSb = 0;
			int minLog2TileCols = 0;
			int maxLog2TileCols = 0;
			int maxLog2TileRows = 0;
			int minLog2Tiles = 0;
			int tileColsLog2 = 0;
			AomArray<int> sbColStarts = new AomArray<int>();
			int tileCols = 0;
			int minLog2TileRows = 0;
			int tileRowsLog2 = 0;
			AomArray<int> sbRowStarts = new AomArray<int>();
			int tileRows = 0;
			int widestTileSb = 0;
			int startSb = 0;
			int n = 0;
			int sizeSb = 0;
			int maxTileHeightSb = 0;
			int maxHeight = 0;
			miCols = (2 * ((frameWidth + 7) >> 3));
			miRows = (2 * ((frameHeight + 7) >> 3));
			sb4x4 = Num_4x4_Blocks_Wide[sbSize];
			sbShift = Mi_Width_Log2[sbSize];
			sbCols = (((miCols + sb4x4) - 1) >> sbShift);
			sbRows = (((miRows + sb4x4) - 1) >> sbShift);

			if ((seq_level_idx != 31))
			{
				maxTileWidthSb = ((Tile_Width_Scaling_Factor[seq_tier][seq_level_idx] * MAX_TILE_WIDTH) >> (sbShift + 4));
				maxTileAreaSb = ((Tile_Area_Scaling_Factor[seq_tier][seq_level_idx] * MAX_TILE_AREA) >> ((2 * (sbShift + 2)) + 2));
			}
			else 
			{
				maxTileWidthSb = sbCols;
				maxTileAreaSb = (sbCols * sbRows);
			}
			minLog2TileCols = TileLog2(maxTileWidthSb, sbCols);
			maxLog2TileCols = TileLog2(1, Min(sbCols, MAX_TILE_COLS));
			maxLog2TileRows = TileLog2(1, Min(sbRows, MAX_TILE_ROWS));
			minLog2Tiles = Max(minLog2TileCols, TileLog2(maxTileAreaSb, (sbRows * sbCols)));

			if ((isBridge != 0))
			{
				uniform_tile_spacing_flag = 1;
			}
			else 
			{
				this.uniform_tile_spacing_flag = stream.Pick("uniform_tile_spacing_flag", _original != null ? _original.uniform_tile_spacing_flag : this.uniform_tile_spacing_flag, _edited != null ? _edited.uniform_tile_spacing_flag : _original != null ? _original.uniform_tile_spacing_flag : this.uniform_tile_spacing_flag);
				stream.WriteFixed(1, this.uniform_tile_spacing_flag, "uniform_tile_spacing_flag"); 
			}

			if ((uniform_tile_spacing_flag != 0))
			{
				sbShift = Mi_Width_Log2[uniformSbSize];
				tileColsLog2 = minLog2TileCols;

				if (!(isBridge != 0))
				{

					while ((tileColsLog2 < maxLog2TileCols))
					{
						this.increment_tile_cols_log2 = stream.Pick("increment_tile_cols_log2", _original != null ? ((obu_type == OBU_SEQUENCE_HEADER ? _original.SeqTileColsLog2 : _original.TileColsLog2) > tileColsLog2 ? 1 : 0) : this.increment_tile_cols_log2, _edited != null ? ((obu_type == OBU_SEQUENCE_HEADER ? _edited.SeqTileColsLog2 : _edited.TileColsLog2) > tileColsLog2 ? 1 : 0) : _original != null ? ((obu_type == OBU_SEQUENCE_HEADER ? _original.SeqTileColsLog2 : _original.TileColsLog2) > tileColsLog2 ? 1 : 0) : this.increment_tile_cols_log2);
						stream.WriteFixed(1, this.increment_tile_cols_log2, "increment_tile_cols_log2"); 

						if ((increment_tile_cols_log2 == 1))
						{
							tileColsLog2 += 1;
						}
						else 
						{
							break;
						}
					}
				}
				(sbColStarts, tileCols) = UniformSpacing(tileColsLog2, miCols, uniformSbSize);
				tileColsLog2 = TileLog2(1, tileCols);
				minLog2TileRows = Max((minLog2Tiles - tileColsLog2), 0);
				tileRowsLog2 = minLog2TileRows;

				if (!(isBridge != 0))
				{

					while ((tileRowsLog2 < maxLog2TileRows))
					{
						this.increment_tile_rows_log2 = stream.Pick("increment_tile_rows_log2", _original != null ? ((obu_type == OBU_SEQUENCE_HEADER ? _original.SeqTileRowsLog2 : _original.TileRowsLog2) > tileRowsLog2 ? 1 : 0) : this.increment_tile_rows_log2, _edited != null ? ((obu_type == OBU_SEQUENCE_HEADER ? _edited.SeqTileRowsLog2 : _edited.TileRowsLog2) > tileRowsLog2 ? 1 : 0) : _original != null ? ((obu_type == OBU_SEQUENCE_HEADER ? _original.SeqTileRowsLog2 : _original.TileRowsLog2) > tileRowsLog2 ? 1 : 0) : this.increment_tile_rows_log2);
						stream.WriteFixed(1, this.increment_tile_rows_log2, "increment_tile_rows_log2"); 

						if ((increment_tile_rows_log2 == 1))
						{
							tileRowsLog2++;
						}
						else 
						{
							break;
						}
					}
				}
				(sbRowStarts, tileRows) = UniformSpacing(tileRowsLog2, miRows, uniformSbSize);
			}
			else 
			{
				widestTileSb = 1;
				startSb = 0;

				for (i = 0; (startSb < sbCols); i++)
				{
					sbColStarts[i] = startSb;
					n = Min((sbCols - startSb), maxTileWidthSb);
					this.width_in_sbs_minus_1 = stream.Pick("width_in_sbs_minus_1", _original != null ? _original.width_in_sbs_minus_1 : this.width_in_sbs_minus_1, _edited != null ? _edited.width_in_sbs_minus_1 : _original != null ? _original.width_in_sbs_minus_1 : this.width_in_sbs_minus_1);
					stream.Write_ns(n, this.width_in_sbs_minus_1, "width_in_sbs_minus_1"); 
					sizeSb = (width_in_sbs_minus_1 + 1);
					widestTileSb = Max(sizeSb, widestTileSb);
					startSb += sizeSb;
				}
				tileCols = i;
				tileColsLog2 = TileLog2(1, tileCols);

				if ((minLog2Tiles > 0))
				{
					maxTileAreaSb = ((sbRows * sbCols) >> (minLog2Tiles + 1));
				}
				else 
				{
					maxTileAreaSb = (sbRows * sbCols);
				}
				maxTileHeightSb = Max((maxTileAreaSb / widestTileSb), 1);
				startSb = 0;

				for (i = 0; (startSb < sbRows); i++)
				{
					sbRowStarts[i] = startSb;
					maxHeight = Min((sbRows - startSb), maxTileHeightSb);
					this.height_in_sbs_minus_1 = stream.Pick("height_in_sbs_minus_1", _original != null ? _original.height_in_sbs_minus_1 : this.height_in_sbs_minus_1, _edited != null ? _edited.height_in_sbs_minus_1 : _original != null ? _original.height_in_sbs_minus_1 : this.height_in_sbs_minus_1);
					stream.Write_ns(maxHeight, this.height_in_sbs_minus_1, "height_in_sbs_minus_1"); 
					sizeSb = (height_in_sbs_minus_1 + 1);
					startSb = (startSb + sizeSb);
				}
				tileRows = i;
			}
			tileRowsLog2 = TileLog2(1, tileRows);
			return (sbRowStarts, sbRows, tileRows, tileRowsLog2, sbColStarts, sbCols, tileCols, tileColsLog2, uniform_tile_spacing_flag, sbShift);
        }

    /*
delta_q_params() {
delta_q_res = 0	
delta_q_present = 0	
if ( base_q_idx > 0 ) {	
delta_q_present	f(1)
}	
if ( delta_q_present ) {	
delta_q_res	f(2)
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
gdf_params() {
if ( CodedLossless || !enable_gdf ) {	
gdf_frame_enable = 0	
} else {	
if ( single_picture_header_flag ) {	
gdf_frame_enable = 1	
} else {	
gdf_frame_enable	f(1)
}	
if ( !gdf_frame_enable ) {	
return	
}	
gdfBlkSize = Max(Block_Width[ SbSize ],GDF_MIN_SIZE)	
if ( gdf_unit_matches_sb_size ) {	
gdfBlkSize = Block_Width[ SbSize ]	
} else if ( SbSize == BLOCK_64X64 ) {	
a = 0	
for ( i = 0; i < TileCols; i++ ) {	
a = a | MiColStarts[ i ]	
}	
for ( i = 0; i < TileRows; i++ ) {	
a = a | MiRowStarts[ i ]	
}	
if ( a & 16 ) {	
gdfBlkSize = 64	
}	
}	
GdfBlkSize = gdfBlkSize	
if ( MiCols * MI_SIZE > gdfBlkSize ||	
MiRows * MI_SIZE > gdfBlkSize ||	
( disable_loopfilters_across_tiles &&	
(TileRows > 1 || TileCols > 1) ) ) {	
gdf_per_block	f(1)
} else {	
gdf_per_block = 0	
}	
gdf_pic_qc_idx	f(2)
gdf_pic_scale_idx	f(2)
GdfPixScale = 1 + gdf_pic_scale_idx	
}	
}
    */
		private int GdfBlkSize;
		public int _GdfBlkSize { get { return GdfBlkSize; } set { GdfBlkSize = value; } }
		private int gdf_per_block;
		public int _GdfPerBlock { get { return gdf_per_block; } set { gdf_per_block = value; } }
		private int gdf_pic_qc_idx;
		public int _GdfPicQcIdx { get { return gdf_pic_qc_idx; } set { gdf_pic_qc_idx = value; } }
		private int gdf_pic_scale_idx;
		public int _GdfPicScaleIdx { get { return gdf_pic_scale_idx; } set { gdf_pic_scale_idx = value; } }
		private int GdfPixScale;
		public int _GdfPixScale { get { return GdfPixScale; } set { GdfPixScale = value; } }

        private void GdfParams()
        {
			int i = 0;
			int gdfBlkSize = 0;
			int a = 0;

			if (((CodedLossless != 0) || !(enable_gdf != 0)))
			{
				gdf_frame_enable = 0;
			}
			else 
			{

				if ((single_picture_header_flag != 0))
				{
					gdf_frame_enable = 1;
				}
				else 
				{
					stream.ReadFixed(1, out this.gdf_frame_enable, "gdf_frame_enable"); 
				}

				if (!(gdf_frame_enable != 0))
				{
					return;
				}
				gdfBlkSize = Max(Block_Width[SbSize], GDF_MIN_SIZE);

				if ((gdf_unit_matches_sb_size != 0))
				{
					gdfBlkSize = Block_Width[SbSize];
				}
				else if ((SbSize == BLOCK_64X64))
				{
					a = 0;

					for (i = 0; (i < TileCols); i++)
					{
						a = (a | MiColStarts[i]);
					}

					for (i = 0; (i < TileRows); i++)
					{
						a = (a | MiRowStarts[i]);
					}

					if (((a & 16) != 0))
					{
						gdfBlkSize = 64;
					}
				}
				GdfBlkSize = gdfBlkSize;

				if (((((MiCols * MI_SIZE) > gdfBlkSize) || ((MiRows * MI_SIZE) > gdfBlkSize)) || ((disable_loopfilters_across_tiles != 0) && ((TileRows > 1) || (TileCols > 1)))))
				{
					stream.ReadFixed(1, out this.gdf_per_block, "gdf_per_block"); 
				}
				else 
				{
					gdf_per_block = 0;
				}
				stream.ReadFixed(2, out this.gdf_pic_qc_idx, "gdf_pic_qc_idx"); 
				stream.ReadFixed(2, out this.gdf_pic_scale_idx, "gdf_pic_scale_idx"); 
				GdfPixScale = (1 + gdf_pic_scale_idx);
			}
        }

        private void WriteGdfParams()
        {
			int i = 0;
			int gdfBlkSize = 0;
			int a = 0;

			if (((CodedLossless != 0) || !(enable_gdf != 0)))
			{
				gdf_frame_enable = 0;
			}
			else 
			{

				if ((single_picture_header_flag != 0))
				{
					gdf_frame_enable = 1;
				}
				else 
				{
					this.gdf_frame_enable = stream.Pick("gdf_frame_enable", _original != null ? _original.gdf_frame_enable : this.gdf_frame_enable, _edited != null ? _edited.gdf_frame_enable : _original != null ? _original.gdf_frame_enable : this.gdf_frame_enable);
					stream.WriteFixed(1, this.gdf_frame_enable, "gdf_frame_enable"); 
				}

				if (!(gdf_frame_enable != 0))
				{
					return;
				}
				gdfBlkSize = Max(Block_Width[SbSize], GDF_MIN_SIZE);

				if ((gdf_unit_matches_sb_size != 0))
				{
					gdfBlkSize = Block_Width[SbSize];
				}
				else if ((SbSize == BLOCK_64X64))
				{
					a = 0;

					for (i = 0; (i < TileCols); i++)
					{
						a = (a | MiColStarts[i]);
					}

					for (i = 0; (i < TileRows); i++)
					{
						a = (a | MiRowStarts[i]);
					}

					if (((a & 16) != 0))
					{
						gdfBlkSize = 64;
					}
				}
				GdfBlkSize = gdfBlkSize;

				if (((((MiCols * MI_SIZE) > gdfBlkSize) || ((MiRows * MI_SIZE) > gdfBlkSize)) || ((disable_loopfilters_across_tiles != 0) && ((TileRows > 1) || (TileCols > 1)))))
				{
					this.gdf_per_block = stream.Pick("gdf_per_block", _original != null ? _original.gdf_per_block : this.gdf_per_block, _edited != null ? _edited.gdf_per_block : _original != null ? _original.gdf_per_block : this.gdf_per_block);
					stream.WriteFixed(1, this.gdf_per_block, "gdf_per_block"); 
				}
				else 
				{
					gdf_per_block = 0;
				}
				this.gdf_pic_qc_idx = stream.Pick("gdf_pic_qc_idx", _original != null ? _original.gdf_pic_qc_idx : this.gdf_pic_qc_idx, _edited != null ? _edited.gdf_pic_qc_idx : _original != null ? _original.gdf_pic_qc_idx : this.gdf_pic_qc_idx);
				stream.WriteFixed(2, this.gdf_pic_qc_idx, "gdf_pic_qc_idx"); 
				this.gdf_pic_scale_idx = stream.Pick("gdf_pic_scale_idx", _original != null ? _original.gdf_pic_scale_idx : this.gdf_pic_scale_idx, _edited != null ? _edited.gdf_pic_scale_idx : _original != null ? _original.gdf_pic_scale_idx : this.gdf_pic_scale_idx);
				stream.WriteFixed(2, this.gdf_pic_scale_idx, "gdf_pic_scale_idx"); 
				GdfPixScale = (1 + gdf_pic_scale_idx);
			}
        }

    /*
cdef_params() {
if ( CodedLossless ||	
!enable_cdef ) {	
cdef_frame_enable = 0	
return	
}	
if ( single_picture_header_flag ) {	
cdef_frame_enable = 1	
} else {	
cdef_frame_enable	f(1)
}	
if ( !cdef_frame_enable ) {	
return	
}	
cdef_damping_minus_3	f(2)
CdefDamping = cdef_damping_minus_3 + 3	
cdef_strengths_minus_1	f(3)
CdefStrengths = cdef_strengths_minus_1 + 1	
if ( CdefOnSkipTxfm == CDEF_ON_SKIP_TXFM_ADAPTIVE ) {	
cdef_on_skip_txfm_frame_enable	f(1)
} else if ( CdefOnSkipTxfm == CDEF_ON_SKIP_TXFM_ALWAYS_ON ) {	
cdef_on_skip_txfm_frame_enable = 1	
} else {	
cdef_on_skip_txfm_frame_enable = 0	
}	
for ( i = 0; i < CdefStrengths; i++ ) {	
cdef_y_pri_zero	f(1)
if ( cdef_y_pri_zero ) {	
cdef_y_pri_strength[ i ] = 0	
} else {	
cdef_y_pri_strength[ i ]	f(4)
}	
cdef_y_sec_strength[ i ]	f(2)
if ( cdef_y_sec_strength[ i ] == 3 ) {	
cdef_y_sec_strength[ i ] += 1	
}	
if ( NumPlanes > 1 ) {	
cdef_uv_pri_zero	f(1)
if ( cdef_uv_pri_zero ) {	
cdef_uv_pri_strength[ i ] = 0	
} else {	
cdef_uv_pri_strength[ i ]	f(4)
}	
cdef_uv_sec_strength[ i ]	f(2)
if ( cdef_uv_sec_strength[ i ] == 3 ) {	
cdef_uv_sec_strength[ i ] += 1	
}	
}	
}	
}
    */
		private int cdef_damping_minus_3;
		public int _CdefDampingMinus3 { get { return cdef_damping_minus_3; } set { cdef_damping_minus_3 = value; } }
		private int CdefDamping;
		public int _CdefDamping { get { return CdefDamping; } set { CdefDamping = value; } }
		private int cdef_strengths_minus_1;
		public int _CdefStrengthsMinus1 { get { return cdef_strengths_minus_1; } set { cdef_strengths_minus_1 = value; } }
		private int CdefStrengths;
		public int _CdefStrengths { get { return CdefStrengths; } set { CdefStrengths = value; } }
		private int cdef_on_skip_txfm_frame_enable;
		public int _CdefOnSkipTxfmFrameEnable { get { return cdef_on_skip_txfm_frame_enable; } set { cdef_on_skip_txfm_frame_enable = value; } }
		private int cdef_y_pri_zero;
		public int _CdefyPriZero { get { return cdef_y_pri_zero; } set { cdef_y_pri_zero = value; } }
		private AomArray<int> cdef_y_pri_strength = new AomArray<int>();
		public AomArray<int> _CdefyPriStrength { get { return cdef_y_pri_strength; } set { cdef_y_pri_strength = value; } }
		private AomArray<int> cdef_y_sec_strength = new AomArray<int>();
		public AomArray<int> _CdefySecStrength { get { return cdef_y_sec_strength; } set { cdef_y_sec_strength = value; } }
		private int cdef_uv_pri_zero;
		public int _CdefUvPriZero { get { return cdef_uv_pri_zero; } set { cdef_uv_pri_zero = value; } }
		private AomArray<int> cdef_uv_pri_strength = new AomArray<int>();
		public AomArray<int> _CdefUvPriStrength { get { return cdef_uv_pri_strength; } set { cdef_uv_pri_strength = value; } }
		private AomArray<int> cdef_uv_sec_strength = new AomArray<int>();
		public AomArray<int> _CdefUvSecStrength { get { return cdef_uv_sec_strength; } set { cdef_uv_sec_strength = value; } }

        private void CdefParams()
        {
			int i = 0;

			if (((CodedLossless != 0) || !(enable_cdef != 0)))
			{
				cdef_frame_enable = 0;
				return;
			}

			if ((single_picture_header_flag != 0))
			{
				cdef_frame_enable = 1;
			}
			else 
			{
				stream.ReadFixed(1, out this.cdef_frame_enable, "cdef_frame_enable"); 
			}

			if (!(cdef_frame_enable != 0))
			{
				return;
			}
			stream.ReadFixed(2, out this.cdef_damping_minus_3, "cdef_damping_minus_3"); 
			CdefDamping = (cdef_damping_minus_3 + 3);
			stream.ReadFixed(3, out this.cdef_strengths_minus_1, "cdef_strengths_minus_1"); 
			CdefStrengths = (cdef_strengths_minus_1 + 1);

			if ((CdefOnSkipTxfm == CDEF_ON_SKIP_TXFM_ADAPTIVE))
			{
				stream.ReadFixed(1, out this.cdef_on_skip_txfm_frame_enable, "cdef_on_skip_txfm_frame_enable"); 
			}
			else if ((CdefOnSkipTxfm == CDEF_ON_SKIP_TXFM_ALWAYS_ON))
			{
				cdef_on_skip_txfm_frame_enable = 1;
			}
			else 
			{
				cdef_on_skip_txfm_frame_enable = 0;
			}

			for (i = 0; (i < CdefStrengths); i++)
			{
				stream.ReadFixed(1, out this.cdef_y_pri_zero, "cdef_y_pri_zero"); 

				if ((cdef_y_pri_zero != 0))
				{
					cdef_y_pri_strength[i] = 0;
				}
				else 
				{
					stream.ReadFixed(4, out this.cdef_y_pri_strength[i], "cdef_y_pri_strength"); 
				}
				stream.ReadFixed(2, out this.cdef_y_sec_strength[i], "cdef_y_sec_strength"); 

				if ((cdef_y_sec_strength[i] == 3))
				{
					cdef_y_sec_strength[i] += 1;
				}

				if ((NumPlanes > 1))
				{
					stream.ReadFixed(1, out this.cdef_uv_pri_zero, "cdef_uv_pri_zero"); 

					if ((cdef_uv_pri_zero != 0))
					{
						cdef_uv_pri_strength[i] = 0;
					}
					else 
					{
						stream.ReadFixed(4, out this.cdef_uv_pri_strength[i], "cdef_uv_pri_strength"); 
					}
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

			if (((CodedLossless != 0) || !(enable_cdef != 0)))
			{
				cdef_frame_enable = 0;
				return;
			}

			if ((single_picture_header_flag != 0))
			{
				cdef_frame_enable = 1;
			}
			else 
			{
				this.cdef_frame_enable = stream.Pick("cdef_frame_enable", _original != null ? _original.cdef_frame_enable : this.cdef_frame_enable, _edited != null ? _edited.cdef_frame_enable : _original != null ? _original.cdef_frame_enable : this.cdef_frame_enable);
				stream.WriteFixed(1, this.cdef_frame_enable, "cdef_frame_enable"); 
			}

			if (!(cdef_frame_enable != 0))
			{
				return;
			}
			this.cdef_damping_minus_3 = stream.Pick("cdef_damping_minus_3", _original != null ? _original.cdef_damping_minus_3 : this.cdef_damping_minus_3, _edited != null ? _edited.cdef_damping_minus_3 : _original != null ? _original.cdef_damping_minus_3 : this.cdef_damping_minus_3);
			stream.WriteFixed(2, this.cdef_damping_minus_3, "cdef_damping_minus_3"); 
			CdefDamping = (cdef_damping_minus_3 + 3);
			this.cdef_strengths_minus_1 = stream.Pick("cdef_strengths_minus_1", _original != null ? _original.cdef_strengths_minus_1 : this.cdef_strengths_minus_1, _edited != null ? _edited.cdef_strengths_minus_1 : _original != null ? _original.cdef_strengths_minus_1 : this.cdef_strengths_minus_1);
			stream.WriteFixed(3, this.cdef_strengths_minus_1, "cdef_strengths_minus_1"); 
			CdefStrengths = (cdef_strengths_minus_1 + 1);

			if ((CdefOnSkipTxfm == CDEF_ON_SKIP_TXFM_ADAPTIVE))
			{
				this.cdef_on_skip_txfm_frame_enable = stream.Pick("cdef_on_skip_txfm_frame_enable", _original != null ? _original.cdef_on_skip_txfm_frame_enable : this.cdef_on_skip_txfm_frame_enable, _edited != null ? _edited.cdef_on_skip_txfm_frame_enable : _original != null ? _original.cdef_on_skip_txfm_frame_enable : this.cdef_on_skip_txfm_frame_enable);
				stream.WriteFixed(1, this.cdef_on_skip_txfm_frame_enable, "cdef_on_skip_txfm_frame_enable"); 
			}
			else if ((CdefOnSkipTxfm == CDEF_ON_SKIP_TXFM_ALWAYS_ON))
			{
				cdef_on_skip_txfm_frame_enable = 1;
			}
			else 
			{
				cdef_on_skip_txfm_frame_enable = 0;
			}

			for (i = 0; (i < CdefStrengths); i++)
			{
				this.cdef_y_pri_zero = stream.Pick("cdef_y_pri_zero", _original != null ? (_original.cdef_y_pri_strength[i] == 0 ? 1 : 0) : this.cdef_y_pri_zero, _edited != null ? (_edited.cdef_y_pri_strength[i] == 0 ? 1 : 0) : _original != null ? (_original.cdef_y_pri_strength[i] == 0 ? 1 : 0) : this.cdef_y_pri_zero);
				stream.WriteFixed(1, this.cdef_y_pri_zero, "cdef_y_pri_zero"); 

				if ((cdef_y_pri_zero != 0))
				{
					cdef_y_pri_strength[i] = 0;
				}
				else 
				{
					this.cdef_y_pri_strength[i] = stream.Pick("cdef_y_pri_strength", _original != null ? _original.cdef_y_pri_strength[i] : this.cdef_y_pri_strength[i], _edited != null ? _edited.cdef_y_pri_strength[i] : _original != null ? _original.cdef_y_pri_strength[i] : this.cdef_y_pri_strength[i]);
					stream.WriteFixed(4, this.cdef_y_pri_strength[i], "cdef_y_pri_strength"); 
				}
				this.cdef_y_sec_strength[i] = stream.Pick("cdef_y_sec_strength", _original != null ? (_original.cdef_y_sec_strength[i] == 4 ? 3 : _original.cdef_y_sec_strength[i]) : this.cdef_y_sec_strength[i], _edited != null ? (_edited.cdef_y_sec_strength[i] == 4 ? 3 : _edited.cdef_y_sec_strength[i]) : _original != null ? (_original.cdef_y_sec_strength[i] == 4 ? 3 : _original.cdef_y_sec_strength[i]) : this.cdef_y_sec_strength[i]);
				stream.WriteFixed(2, this.cdef_y_sec_strength[i], "cdef_y_sec_strength"); 

				if ((cdef_y_sec_strength[i] == 3))
				{
					cdef_y_sec_strength[i] += 1;
				}

				if ((NumPlanes > 1))
				{
					this.cdef_uv_pri_zero = stream.Pick("cdef_uv_pri_zero", _original != null ? (_original.cdef_uv_pri_strength[i] == 0 ? 1 : 0) : this.cdef_uv_pri_zero, _edited != null ? (_edited.cdef_uv_pri_strength[i] == 0 ? 1 : 0) : _original != null ? (_original.cdef_uv_pri_strength[i] == 0 ? 1 : 0) : this.cdef_uv_pri_zero);
					stream.WriteFixed(1, this.cdef_uv_pri_zero, "cdef_uv_pri_zero"); 

					if ((cdef_uv_pri_zero != 0))
					{
						cdef_uv_pri_strength[i] = 0;
					}
					else 
					{
						this.cdef_uv_pri_strength[i] = stream.Pick("cdef_uv_pri_strength", _original != null ? _original.cdef_uv_pri_strength[i] : this.cdef_uv_pri_strength[i], _edited != null ? _edited.cdef_uv_pri_strength[i] : _original != null ? _original.cdef_uv_pri_strength[i] : this.cdef_uv_pri_strength[i]);
						stream.WriteFixed(4, this.cdef_uv_pri_strength[i], "cdef_uv_pri_strength"); 
					}
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
if ( CodedLossless || !enable_restoration ) {	
FrameRestorationType[ 0 ] = RESTORE_NONE	
FrameRestorationType[ 1 ] = RESTORE_NONE	
FrameRestorationType[ 2 ] = RESTORE_NONE	
UsesLr = 0	
for ( i = 0; i < 3; i++ ) {	
frame_filters_on[ i ] = 0	
}	
return	
}	
usesLumaLr = 0	
usesChromaLr = 0	
for ( plane = 0; plane < NumPlanes; plane++ ) {	
toolsCount = 1	
indexToTool[ 0 ] = RESTORE_NONE	
for ( i = 1; i < RESTORE_SWITCHABLE_TYPES; i++ ) {	
if ( !lr_tools_disable[ plane > 0 ][ i ] ) {	
indexToTool[ toolsCount ] = i	
toolsCount += 1	
}	
}	
indexToTool[ toolsCount ] = RESTORE_SWITCHABLE	
allowSwitchable = (toolsCount > 2)	
n = toolsCount + allowSwitchable	
tool_index	ns(n)
FrameRestorationType[ plane ] = indexToTool[ tool_index ]	
if ( FrameRestorationType[ plane ] != RESTORE_NONE ) {	
if ( plane == 0 ) {	
usesLumaLr = 1	
} else {	
usesChromaLr = 1	
}	
}	
r = FrameRestorationType[ plane ]	
if ( plane == 0 ) {	
NumFilterClasses = 1	
}	
frame_filters_on[ plane ] = 0	
temporal_pred_flag[ plane ] = 0	
if ( r == RESTORE_WIENER_NONSEP || r == RESTORE_SWITCHABLE ) {	
frame_filters_on[ plane ]	f(1)
if ( frame_filters_on[ plane ] ) {	
numRefFrames = (FrameIsIntra || FrameType == SWITCH_FRAME) ?	
0 : NumTotalRefs	
if ( numRefFrames > 0 ) {	
temporal_pred_flag[ plane ]	f(1)
}	
if ( temporal_pred_flag[ plane ] && numRefFrames > 1 ) {	
n = CeilLog2(numRefFrames)	
rst_ref_pic_idx	f(n)
} else {	
rst_ref_pic_idx = 0	
}	
if ( temporal_pred_flag[ plane ] ) {	
refIdx = ref_frame_idx[ rst_ref_pic_idx ]	
refPlane = plane	
if ( plane > 0 && !RefFrameFiltersOn[ refIdx ][ plane ] ) {	
refPlane = plane == 1 ? 2 : 1	
}	
if ( plane == 0 ) {	
NumFilterClasses = RefNumFilterClasses[ refIdx ]	
}	
for ( c = 0; c < WIENER_NS_CLASSES; c++ ) {	
for ( i = 0; i < WIENER_NS_CHROMA_COEFFS; i++ ) {	
FrameLrWienerNs[plane][c][i] =	
RefFrameLrWienerNs[refIdx][refPlane][c][i]	
}	
}	
}	
}	
if ( plane == 0 && frame_filters_on[ 0 ] ) {	
if ( temporal_pred_flag[ plane ] ) {	
num_filter_classes_idx =	
Encode_Num_Filter_Classes[ NumFilterClasses ]	
} else {	
num_filter_classes_idx	f(3)
NumFilterClasses =	
Decode_Num_Filter_Classes[ num_filter_classes_idx ]	
}	
qindex = base_q_idx	
index = get_filter_set_index(qindex)	
SubclassLookup =	
Pc_Wiener_Sub_Classify2[ index ][ num_filter_classes_idx ]	
}	
}	
}	
UsesLr = usesLumaLr || usesChromaLr	
LoopRestorationSize[ 0 ] = RESTORATION_TILESIZE_MAX >> 3	
LoopRestorationSize[ 1 ] = RESTORATION_TILESIZE_MAX >>	
( 3 + Max(SubsamplingX, SubsamplingY) )	
if ( usesLumaLr ) {	
lr_luma_use_half_size	f(1)
if ( lr_luma_use_half_size ) {	
shift = 1	
} else if ( SbSize == BLOCK_256X256 ) {	
shift = 0	
} else {	
lr_luma_use_max_size	f(1)
if ( lr_luma_use_max_size ) {	
shift = 0	
} else if ( SbSize == BLOCK_128X128 ) {	
shift = 2	
} else {	
lr_luma_use_quarter_size	f(1)
shift = lr_luma_use_quarter_size ? 2 : 3	
}	
}	
LoopRestorationSize[ 0 ] = RESTORATION_TILESIZE_MAX >> shift	
}	
if ( usesChromaLr ) {	
LoopRestorationSize[ 1 ] = RESTORATION_TILESIZE_MAX >>	
Max(SubsamplingX, SubsamplingY)	
lr_chroma_use_half_size	f(1)
if ( lr_chroma_use_half_size ) {	
shift = 1	
} else if ( SbSize == BLOCK_256X256 ) {	
shift = 0	
} else {	
lr_chroma_use_max_size	f(1)
if ( lr_chroma_use_max_size ) {	
shift = 0	
} else if ( SbSize == BLOCK_128X128 ) {	
shift = 2	
} else {	
lr_chroma_use_quarter_size	f(1)
shift = lr_chroma_use_quarter_size ? 2 : 3	
}	
}	
LoopRestorationSize[ 1 ] = LoopRestorationSize[ 1 ] >> shift	
}	
LoopRestorationSize[ 2 ] = LoopRestorationSize[ 1 ]	
for ( plane = 0; plane < NumPlanes; plane++ ) {	
if ( frame_filters_on[ plane ] && !temporal_pred_flag[ plane ] ) {	
read_wienerns_filter(plane, 0, 0, 1)	
}	
}	
}
    */
		private int UsesLr;
		public int _UsesLr { get { return UsesLr; } set { UsesLr = value; } }
		private int tool_index;
		public int _ToolIndex { get { return tool_index; } set { tool_index = value; } }
		private int NumFilterClasses;
		public int _NumFilterClasses { get { return NumFilterClasses; } set { NumFilterClasses = value; } }
		private AomArray<int> temporal_pred_flag = new AomArray<int>();
		public AomArray<int> _TemporalPredFlag { get { return temporal_pred_flag; } set { temporal_pred_flag = value; } }
		private int rst_ref_pic_idx;
		public int _RstRefPicIdx { get { return rst_ref_pic_idx; } set { rst_ref_pic_idx = value; } }
		private AomArray<AomArray<AomArray<int>>> FrameLrWienerNs = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _FrameLrWienerNs { get { return FrameLrWienerNs; } set { FrameLrWienerNs = value; } }
		private int num_filter_classes_idx;
		public int _NumFilterClassesIdx { get { return num_filter_classes_idx; } set { num_filter_classes_idx = value; } }
		private AomArray<int> SubclassLookup = new AomArray<int>();
		public AomArray<int> _SubclassLookup { get { return SubclassLookup; } set { SubclassLookup = value; } }
		private AomArray<int> LoopRestorationSize = new AomArray<int>();
		public AomArray<int> _LoopRestorationSize { get { return LoopRestorationSize; } set { LoopRestorationSize = value; } }
		private int lr_luma_use_half_size;
		public int _LrLumaUseHalfSize { get { return lr_luma_use_half_size; } set { lr_luma_use_half_size = value; } }
		private int lr_luma_use_max_size;
		public int _LrLumaUseMaxSize { get { return lr_luma_use_max_size; } set { lr_luma_use_max_size = value; } }
		private int lr_luma_use_quarter_size;
		public int _LrLumaUseQuarterSize { get { return lr_luma_use_quarter_size; } set { lr_luma_use_quarter_size = value; } }
		private int lr_chroma_use_half_size;
		public int _LrChromaUseHalfSize { get { return lr_chroma_use_half_size; } set { lr_chroma_use_half_size = value; } }
		private int lr_chroma_use_max_size;
		public int _LrChromaUseMaxSize { get { return lr_chroma_use_max_size; } set { lr_chroma_use_max_size = value; } }
		private int lr_chroma_use_quarter_size;
		public int _LrChromaUseQuarterSize { get { return lr_chroma_use_quarter_size; } set { lr_chroma_use_quarter_size = value; } }

        private void LrParams()
        {
			int i = 0;
			int plane = 0;
			int c = 0;
			int usesLumaLr = 0;
			int usesChromaLr = 0;
			int toolsCount = 0;
			AomArray<int> indexToTool = new AomArray<int>();
			int allowSwitchable = 0;
			int n = 0;
			int r = 0;
			int numRefFrames = 0;
			int refIdx = 0;
			int refPlane = 0;
			int qindex = 0;
			int index = 0;
			int shift = 0;

			if (((CodedLossless != 0) || !(enable_restoration != 0)))
			{
				FrameRestorationType[0] = RESTORE_NONE;
				FrameRestorationType[1] = RESTORE_NONE;
				FrameRestorationType[2] = RESTORE_NONE;
				UsesLr = 0;

				for (i = 0; (i < 3); i++)
				{
					frame_filters_on[i] = 0;
				}
				return;
			}
			usesLumaLr = 0;
			usesChromaLr = 0;

			for (plane = 0; (plane < NumPlanes); plane++)
			{
				toolsCount = 1;
				indexToTool[0] = RESTORE_NONE;

				for (i = 1; (i < RESTORE_SWITCHABLE_TYPES); i++)
				{

					if (!(lr_tools_disable[((plane > 0) ? 1 : 0)][i] != 0))
					{
						indexToTool[toolsCount] = i;
						toolsCount += 1;
					}
				}
				indexToTool[toolsCount] = RESTORE_SWITCHABLE;
				allowSwitchable = ((toolsCount > 2) ? 1 : 0);
				n = (toolsCount + allowSwitchable);
				stream.Read_ns(n, out this.tool_index, "tool_index"); 
				FrameRestorationType[plane] = indexToTool[tool_index];

				if ((FrameRestorationType[plane] != RESTORE_NONE))
				{

					if ((plane == 0))
					{
						usesLumaLr = 1;
					}
					else 
					{
						usesChromaLr = 1;
					}
				}
				r = FrameRestorationType[plane];

				if ((plane == 0))
				{
					NumFilterClasses = 1;
				}
				frame_filters_on[plane] = 0;
				temporal_pred_flag[plane] = 0;

				if (((r == RESTORE_WIENER_NONSEP) || (r == RESTORE_SWITCHABLE)))
				{
					stream.ReadFixed(1, out this.frame_filters_on[plane], "frame_filters_on"); 

					if ((frame_filters_on[plane] != 0))
					{
						numRefFrames = (((FrameIsIntra != 0) || (FrameType == SWITCH_FRAME)) ? 0 : NumTotalRefs);

						if ((numRefFrames > 0))
						{
							stream.ReadFixed(1, out this.temporal_pred_flag[plane], "temporal_pred_flag"); 
						}

						if (((temporal_pred_flag[plane] != 0) && (numRefFrames > 1)))
						{
							n = CeilLog2(numRefFrames);
							stream.ReadVariable(n, out this.rst_ref_pic_idx, "rst_ref_pic_idx"); 
						}
						else 
						{
							rst_ref_pic_idx = 0;
						}

						if ((temporal_pred_flag[plane] != 0))
						{
							refIdx = ref_frame_idx[rst_ref_pic_idx];
							refPlane = plane;

							if (((plane > 0) && !(RefFrameFiltersOn[refIdx][plane] != 0)))
							{
								refPlane = ((plane == 1) ? 2 : 1);
							}

							if ((plane == 0))
							{
								NumFilterClasses = RefNumFilterClasses[refIdx];
							}

							for (c = 0; (c < WIENER_NS_CLASSES); c++)
							{

								for (i = 0; (i < WIENER_NS_CHROMA_COEFFS); i++)
								{
									FrameLrWienerNs[plane][c][i] = RefFrameLrWienerNs[refIdx][refPlane][c][i];
								}
							}
						}
					}

					if (((plane == 0) && (frame_filters_on[0] != 0)))
					{

						if ((temporal_pred_flag[plane] != 0))
						{
							num_filter_classes_idx = Encode_Num_Filter_Classes[NumFilterClasses];
						}
						else 
						{
							stream.ReadFixed(3, out this.num_filter_classes_idx, "num_filter_classes_idx"); 
							NumFilterClasses = Decode_Num_Filter_Classes[num_filter_classes_idx];
						}
						qindex = base_q_idx;
						index = GetFilterSetIndex(qindex);
						SubclassLookup = Pc_Wiener_Sub_Classify2[index][num_filter_classes_idx];
					}
				}
			}
			UsesLr = (((usesLumaLr != 0) || (usesChromaLr != 0)) ? 1 : 0);
			LoopRestorationSize[0] = (RESTORATION_TILESIZE_MAX >> 3);
			LoopRestorationSize[1] = (RESTORATION_TILESIZE_MAX >> (3 + Max(SubsamplingX, SubsamplingY)));

			if ((usesLumaLr != 0))
			{
				stream.ReadFixed(1, out this.lr_luma_use_half_size, "lr_luma_use_half_size"); 

				if ((lr_luma_use_half_size != 0))
				{
					shift = 1;
				}
				else if ((SbSize == BLOCK_256X256))
				{
					shift = 0;
				}
				else 
				{
					stream.ReadFixed(1, out this.lr_luma_use_max_size, "lr_luma_use_max_size"); 

					if ((lr_luma_use_max_size != 0))
					{
						shift = 0;
					}
					else if ((SbSize == BLOCK_128X128))
					{
						shift = 2;
					}
					else 
					{
						stream.ReadFixed(1, out this.lr_luma_use_quarter_size, "lr_luma_use_quarter_size"); 
						shift = ((lr_luma_use_quarter_size != 0) ? 2 : 3);
					}
				}
				LoopRestorationSize[0] = (RESTORATION_TILESIZE_MAX >> shift);
			}

			if ((usesChromaLr != 0))
			{
				LoopRestorationSize[1] = (RESTORATION_TILESIZE_MAX >> Max(SubsamplingX, SubsamplingY));
				stream.ReadFixed(1, out this.lr_chroma_use_half_size, "lr_chroma_use_half_size"); 

				if ((lr_chroma_use_half_size != 0))
				{
					shift = 1;
				}
				else if ((SbSize == BLOCK_256X256))
				{
					shift = 0;
				}
				else 
				{
					stream.ReadFixed(1, out this.lr_chroma_use_max_size, "lr_chroma_use_max_size"); 

					if ((lr_chroma_use_max_size != 0))
					{
						shift = 0;
					}
					else if ((SbSize == BLOCK_128X128))
					{
						shift = 2;
					}
					else 
					{
						stream.ReadFixed(1, out this.lr_chroma_use_quarter_size, "lr_chroma_use_quarter_size"); 
						shift = ((lr_chroma_use_quarter_size != 0) ? 2 : 3);
					}
				}
				LoopRestorationSize[1] = (LoopRestorationSize[1] >> shift);
			}
			LoopRestorationSize[2] = LoopRestorationSize[1];

			for (plane = 0; (plane < NumPlanes); plane++)
			{

				if (((frame_filters_on[plane] != 0) && !(temporal_pred_flag[plane] != 0)))
				{
					ReadWienernsFilter(plane, 0, 0, 1); 
				}
			}
        }

        private void WriteLrParams()
        {
			int i = 0;
			int plane = 0;
			int c = 0;
			int usesLumaLr = 0;
			int usesChromaLr = 0;
			int toolsCount = 0;
			AomArray<int> indexToTool = new AomArray<int>();
			int allowSwitchable = 0;
			int n = 0;
			int r = 0;
			int numRefFrames = 0;
			int refIdx = 0;
			int refPlane = 0;
			int qindex = 0;
			int index = 0;
			int shift = 0;

			if (((CodedLossless != 0) || !(enable_restoration != 0)))
			{
				FrameRestorationType[0] = RESTORE_NONE;
				FrameRestorationType[1] = RESTORE_NONE;
				FrameRestorationType[2] = RESTORE_NONE;
				UsesLr = 0;

				for (i = 0; (i < 3); i++)
				{
					frame_filters_on[i] = 0;
				}
				return;
			}
			usesLumaLr = 0;
			usesChromaLr = 0;

			for (plane = 0; (plane < NumPlanes); plane++)
			{
				toolsCount = 1;
				indexToTool[0] = RESTORE_NONE;

				for (i = 1; (i < RESTORE_SWITCHABLE_TYPES); i++)
				{

					if (!(lr_tools_disable[((plane > 0) ? 1 : 0)][i] != 0))
					{
						indexToTool[toolsCount] = i;
						toolsCount += 1;
					}
				}
				indexToTool[toolsCount] = RESTORE_SWITCHABLE;
				allowSwitchable = ((toolsCount > 2) ? 1 : 0);
				n = (toolsCount + allowSwitchable);
				this.tool_index = stream.Pick("tool_index", _original != null ? _original.tool_index : this.tool_index, _edited != null ? _edited.tool_index : _original != null ? _original.tool_index : this.tool_index);
				stream.Write_ns(n, this.tool_index, "tool_index"); 
				FrameRestorationType[plane] = indexToTool[tool_index];

				if ((FrameRestorationType[plane] != RESTORE_NONE))
				{

					if ((plane == 0))
					{
						usesLumaLr = 1;
					}
					else 
					{
						usesChromaLr = 1;
					}
				}
				r = FrameRestorationType[plane];

				if ((plane == 0))
				{
					NumFilterClasses = 1;
				}
				frame_filters_on[plane] = 0;
				temporal_pred_flag[plane] = 0;

				if (((r == RESTORE_WIENER_NONSEP) || (r == RESTORE_SWITCHABLE)))
				{
					this.frame_filters_on[plane] = stream.Pick("frame_filters_on", _original != null ? _original.frame_filters_on[plane] : this.frame_filters_on[plane], _edited != null ? _edited.frame_filters_on[plane] : _original != null ? _original.frame_filters_on[plane] : this.frame_filters_on[plane]);
					stream.WriteFixed(1, this.frame_filters_on[plane], "frame_filters_on"); 

					if ((frame_filters_on[plane] != 0))
					{
						numRefFrames = (((FrameIsIntra != 0) || (FrameType == SWITCH_FRAME)) ? 0 : NumTotalRefs);

						if ((numRefFrames > 0))
						{
							this.temporal_pred_flag[plane] = stream.Pick("temporal_pred_flag", _original != null ? _original.temporal_pred_flag[plane] : this.temporal_pred_flag[plane], _edited != null ? _edited.temporal_pred_flag[plane] : _original != null ? _original.temporal_pred_flag[plane] : this.temporal_pred_flag[plane]);
							stream.WriteFixed(1, this.temporal_pred_flag[plane], "temporal_pred_flag"); 
						}

						if (((temporal_pred_flag[plane] != 0) && (numRefFrames > 1)))
						{
							n = CeilLog2(numRefFrames);
							this.rst_ref_pic_idx = stream.Pick("rst_ref_pic_idx", _original != null ? _original.rst_ref_pic_idx : this.rst_ref_pic_idx, _edited != null ? _edited.rst_ref_pic_idx : _original != null ? _original.rst_ref_pic_idx : this.rst_ref_pic_idx);
							stream.WriteVariable(n, this.rst_ref_pic_idx, "rst_ref_pic_idx"); 
						}
						else 
						{
							rst_ref_pic_idx = 0;
						}

						if ((temporal_pred_flag[plane] != 0))
						{
							refIdx = ref_frame_idx[rst_ref_pic_idx];
							refPlane = plane;

							if (((plane > 0) && !(RefFrameFiltersOn[refIdx][plane] != 0)))
							{
								refPlane = ((plane == 1) ? 2 : 1);
							}

							if ((plane == 0))
							{
								NumFilterClasses = RefNumFilterClasses[refIdx];
							}

							for (c = 0; (c < WIENER_NS_CLASSES); c++)
							{

								for (i = 0; (i < WIENER_NS_CHROMA_COEFFS); i++)
								{
									FrameLrWienerNs[plane][c][i] = RefFrameLrWienerNs[refIdx][refPlane][c][i];
								}
							}
						}
					}

					if (((plane == 0) && (frame_filters_on[0] != 0)))
					{

						if ((temporal_pred_flag[plane] != 0))
						{
							num_filter_classes_idx = Encode_Num_Filter_Classes[NumFilterClasses];
						}
						else 
						{
							this.num_filter_classes_idx = stream.Pick("num_filter_classes_idx", _original != null ? AomArray.IndexOf(Decode_Num_Filter_Classes, _original.NumFilterClasses) : this.num_filter_classes_idx, _edited != null ? AomArray.IndexOf(Decode_Num_Filter_Classes, _edited.NumFilterClasses) : _original != null ? AomArray.IndexOf(Decode_Num_Filter_Classes, _original.NumFilterClasses) : this.num_filter_classes_idx);
							stream.WriteFixed(3, this.num_filter_classes_idx, "num_filter_classes_idx"); 
							NumFilterClasses = Decode_Num_Filter_Classes[num_filter_classes_idx];
						}
						qindex = base_q_idx;
						index = GetFilterSetIndex(qindex);
						SubclassLookup = Pc_Wiener_Sub_Classify2[index][num_filter_classes_idx];
					}
				}
			}
			UsesLr = (((usesLumaLr != 0) || (usesChromaLr != 0)) ? 1 : 0);
			LoopRestorationSize[0] = (RESTORATION_TILESIZE_MAX >> 3);
			LoopRestorationSize[1] = (RESTORATION_TILESIZE_MAX >> (3 + Max(SubsamplingX, SubsamplingY)));

			if ((usesLumaLr != 0))
			{
				this.lr_luma_use_half_size = stream.Pick("lr_luma_use_half_size", _original != null ? _original.lr_luma_use_half_size : this.lr_luma_use_half_size, _edited != null ? _edited.lr_luma_use_half_size : _original != null ? _original.lr_luma_use_half_size : this.lr_luma_use_half_size);
				stream.WriteFixed(1, this.lr_luma_use_half_size, "lr_luma_use_half_size"); 

				if ((lr_luma_use_half_size != 0))
				{
					shift = 1;
				}
				else if ((SbSize == BLOCK_256X256))
				{
					shift = 0;
				}
				else 
				{
					this.lr_luma_use_max_size = stream.Pick("lr_luma_use_max_size", _original != null ? _original.lr_luma_use_max_size : this.lr_luma_use_max_size, _edited != null ? _edited.lr_luma_use_max_size : _original != null ? _original.lr_luma_use_max_size : this.lr_luma_use_max_size);
					stream.WriteFixed(1, this.lr_luma_use_max_size, "lr_luma_use_max_size"); 

					if ((lr_luma_use_max_size != 0))
					{
						shift = 0;
					}
					else if ((SbSize == BLOCK_128X128))
					{
						shift = 2;
					}
					else 
					{
						this.lr_luma_use_quarter_size = stream.Pick("lr_luma_use_quarter_size", _original != null ? _original.lr_luma_use_quarter_size : this.lr_luma_use_quarter_size, _edited != null ? _edited.lr_luma_use_quarter_size : _original != null ? _original.lr_luma_use_quarter_size : this.lr_luma_use_quarter_size);
						stream.WriteFixed(1, this.lr_luma_use_quarter_size, "lr_luma_use_quarter_size"); 
						shift = ((lr_luma_use_quarter_size != 0) ? 2 : 3);
					}
				}
				LoopRestorationSize[0] = (RESTORATION_TILESIZE_MAX >> shift);
			}

			if ((usesChromaLr != 0))
			{
				LoopRestorationSize[1] = (RESTORATION_TILESIZE_MAX >> Max(SubsamplingX, SubsamplingY));
				this.lr_chroma_use_half_size = stream.Pick("lr_chroma_use_half_size", _original != null ? _original.lr_chroma_use_half_size : this.lr_chroma_use_half_size, _edited != null ? _edited.lr_chroma_use_half_size : _original != null ? _original.lr_chroma_use_half_size : this.lr_chroma_use_half_size);
				stream.WriteFixed(1, this.lr_chroma_use_half_size, "lr_chroma_use_half_size"); 

				if ((lr_chroma_use_half_size != 0))
				{
					shift = 1;
				}
				else if ((SbSize == BLOCK_256X256))
				{
					shift = 0;
				}
				else 
				{
					this.lr_chroma_use_max_size = stream.Pick("lr_chroma_use_max_size", _original != null ? _original.lr_chroma_use_max_size : this.lr_chroma_use_max_size, _edited != null ? _edited.lr_chroma_use_max_size : _original != null ? _original.lr_chroma_use_max_size : this.lr_chroma_use_max_size);
					stream.WriteFixed(1, this.lr_chroma_use_max_size, "lr_chroma_use_max_size"); 

					if ((lr_chroma_use_max_size != 0))
					{
						shift = 0;
					}
					else if ((SbSize == BLOCK_128X128))
					{
						shift = 2;
					}
					else 
					{
						this.lr_chroma_use_quarter_size = stream.Pick("lr_chroma_use_quarter_size", _original != null ? _original.lr_chroma_use_quarter_size : this.lr_chroma_use_quarter_size, _edited != null ? _edited.lr_chroma_use_quarter_size : _original != null ? _original.lr_chroma_use_quarter_size : this.lr_chroma_use_quarter_size);
						stream.WriteFixed(1, this.lr_chroma_use_quarter_size, "lr_chroma_use_quarter_size"); 
						shift = ((lr_chroma_use_quarter_size != 0) ? 2 : 3);
					}
				}
				LoopRestorationSize[1] = (LoopRestorationSize[1] >> shift);
			}
			LoopRestorationSize[2] = LoopRestorationSize[1];

			for (plane = 0; (plane < NumPlanes); plane++)
			{

				if (((frame_filters_on[plane] != 0) && !(temporal_pred_flag[plane] != 0)))
				{
					WriteReadWienernsFilter(plane, 0, 0, 1); 
				}
			}
        }

    /*
ccso_params() {
for ( plane = 0; plane < NumPlanes; plane++ ) {	
ccso_planes[ plane ] = 0	
}	
if ( CodedLossless || !enable_ccso ) {	
return	
}	
a = 0	
for ( i = 0; i < TileCols; i++ ) {	
a = a | MiColStarts[ i ]	
}	
for ( i = 0; i < TileRows; i++ ) {	
a = a | MiRowStarts[ i ]	
}	
if ( ccso_unit_matches_sb_size ) {	
CcsoLumaSizeLog2 = Mi_Width_Log2[ SbSize ] + MI_SIZE_LOG2	
} else if ( (a & 63) == 0 ) {	
CcsoLumaSizeLog2 = 8	
} else if ( (a & 31) == 0 ) {	
CcsoLumaSizeLog2 = 7	
} else {	
CcsoLumaSizeLog2 = 6	
}	
if ( single_picture_header_flag ) {	
ccso_frame_flag = 1	
} else {	
ccso_frame_flag	f(1)
}	
if ( !ccso_frame_flag ) {	
return	
}	
for ( plane = 0; plane < NumPlanes; plane++ ) {	
ccso_planes[ plane ]	f(1)
if ( ccso_planes[ plane ] ) {	
if ( FrameIsIntra || FrameType == SWITCH_FRAME ) {	
reuse_ccso[ plane ] = 0	
sb_reuse_ccso[ plane ] = 0	
} else {	
reuse_ccso[ plane ]	f(1)
sb_reuse_ccso[ plane ]	f(1)
}	
if ( reuse_ccso[ plane ] || sb_reuse_ccso[ plane ] ) {	
n = CeilLog2(NumTotalRefs)	
ccso_ref_idx[ plane ]	f(n)
idx = ref_frame_idx[ ccso_ref_idx[ plane ] ]	
tmpCcsoLumaSizeLog2 = CcsoLumaSizeLog2	
load_ccso_params(idx, plane)	
CcsoLumaSizeLog2 = tmpCcsoLumaSizeLog2	
}	
}	
if ( ccso_planes[ plane ] && !reuse_ccso[ plane ] ) {	
ccso_bo_only[ plane ]	f(1)
ccso_scale_idx[ plane ]	f(2)
if ( ccso_bo_only[ plane ] ) {	
ccso_quant_idx[ plane ] = 0	
ccso_ext_filter[ plane ] = 0	
ccso_edge_clf[ plane ] = 0	
} else {	
ccso_quant_idx[ plane ]	f(2)
ccso_ext_filter[ plane ]	f(3)
quantStep = CCSO_Quant_Sz[ ccso_scale_idx[ plane ] ]	
[ ccso_quant_idx[ plane ] ]	
if ( quantStep == 0 ) {	
ccso_edge_clf[ plane ] = 0	
} else {	
ccso_edge_clf[ plane ]	f(1)
}	
}	
n = 2 + ccso_bo_only[ plane ]	
ccso_max_band_log2[ plane ]	f(n)
maxEdgeInterval = CCSO_INPUT_INTERVAL - ccso_edge_clf[ plane ]	
if ( ccso_bo_only[ plane ] ) {	
maxEdgeInterval = 1	
}	
maxBand = 1 << ccso_max_band_log2[ plane ]	
for ( d0 = 0; d0 < maxEdgeInterval; d0++ ) {	
for ( d1 = 0; d1 < maxEdgeInterval; d1++ ) {	
for ( band = 0; band < maxBand; band++ ) {	
ccso_offset_idx	tu(7)
offset = Ccso_Offset[ ccso_offset_idx ] *	
(ccso_scale_idx[ plane ] + 1)	
CcsoFilterOffset[ plane ][ band ][ d0 ][ d1 ] = offset	
}	
}	
}	
}	
}	
}
    */
		private int CcsoLumaSizeLog2;
		public int _CcsoLumaSizeLog2 { get { return CcsoLumaSizeLog2; } set { CcsoLumaSizeLog2 = value; } }
		private int ccso_frame_flag;
		public int _CcsoFrameFlag { get { return ccso_frame_flag; } set { ccso_frame_flag = value; } }
		private AomArray<int> reuse_ccso = new AomArray<int>();
		public AomArray<int> _ReuseCcso { get { return reuse_ccso; } set { reuse_ccso = value; } }
		private AomArray<int> sb_reuse_ccso = new AomArray<int>();
		public AomArray<int> _SbReuseCcso { get { return sb_reuse_ccso; } set { sb_reuse_ccso = value; } }
		private AomArray<int> ccso_ref_idx = new AomArray<int>();
		public AomArray<int> _CcsoRefIdx { get { return ccso_ref_idx; } set { ccso_ref_idx = value; } }
		private AomArray<int> ccso_bo_only = new AomArray<int>();
		public AomArray<int> _CcsoBoOnly { get { return ccso_bo_only; } set { ccso_bo_only = value; } }
		private AomArray<int> ccso_scale_idx = new AomArray<int>();
		public AomArray<int> _CcsoScaleIdx { get { return ccso_scale_idx; } set { ccso_scale_idx = value; } }
		private AomArray<int> ccso_quant_idx = new AomArray<int>();
		public AomArray<int> _CcsoQuantIdx { get { return ccso_quant_idx; } set { ccso_quant_idx = value; } }
		private AomArray<int> ccso_ext_filter = new AomArray<int>();
		public AomArray<int> _CcsoExtFilter { get { return ccso_ext_filter; } set { ccso_ext_filter = value; } }
		private AomArray<int> ccso_edge_clf = new AomArray<int>();
		public AomArray<int> _CcsoEdgeClf { get { return ccso_edge_clf; } set { ccso_edge_clf = value; } }
		private AomArray<int> ccso_max_band_log2 = new AomArray<int>();
		public AomArray<int> _CcsoMaxBandLog2 { get { return ccso_max_band_log2; } set { ccso_max_band_log2 = value; } }
		private int ccso_offset_idx;
		public int _CcsoOffsetIdx { get { return ccso_offset_idx; } set { ccso_offset_idx = value; } }
		private AomArray<AomArray<AomArray<AomArray<int>>>> CcsoFilterOffset = new AomArray<AomArray<AomArray<AomArray<int>>>>(() => new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>())));
		public AomArray<AomArray<AomArray<AomArray<int>>>> _CcsoFilterOffset { get { return CcsoFilterOffset; } set { CcsoFilterOffset = value; } }
		private int d0 = 0;
		private int d1 = 0;
		private int band = 0;

        private void CcsoParams()
        {
			int plane = 0;
			int i = 0;
			int d0 = 0;
			int d1 = 0;
			int band = 0;
			int a = 0;
			int n = 0;
			int idx = 0;
			int tmpCcsoLumaSizeLog2 = 0;
			int quantStep = 0;
			int maxEdgeInterval = 0;
			int maxBand = 0;
			int offset = 0;

			for (plane = 0; (plane < NumPlanes); plane++)
			{
				ccso_planes[plane] = 0;
			}

			if (((CodedLossless != 0) || !(enable_ccso != 0)))
			{
				return;
			}
			a = 0;

			for (i = 0; (i < TileCols); i++)
			{
				a = (a | MiColStarts[i]);
			}

			for (i = 0; (i < TileRows); i++)
			{
				a = (a | MiRowStarts[i]);
			}

			if ((ccso_unit_matches_sb_size != 0))
			{
				CcsoLumaSizeLog2 = (Mi_Width_Log2[SbSize] + MI_SIZE_LOG2);
			}
			else if (((a & 63) == 0))
			{
				CcsoLumaSizeLog2 = 8;
			}
			else if (((a & 31) == 0))
			{
				CcsoLumaSizeLog2 = 7;
			}
			else 
			{
				CcsoLumaSizeLog2 = 6;
			}

			if ((single_picture_header_flag != 0))
			{
				ccso_frame_flag = 1;
			}
			else 
			{
				stream.ReadFixed(1, out this.ccso_frame_flag, "ccso_frame_flag"); 
			}

			if (!(ccso_frame_flag != 0))
			{
				return;
			}

			for (plane = 0; (plane < NumPlanes); plane++)
			{
				stream.ReadFixed(1, out this.ccso_planes[plane], "ccso_planes"); 

				if ((ccso_planes[plane] != 0))
				{

					if (((FrameIsIntra != 0) || (FrameType == SWITCH_FRAME)))
					{
						reuse_ccso[plane] = 0;
						sb_reuse_ccso[plane] = 0;
					}
					else 
					{
						stream.ReadFixed(1, out this.reuse_ccso[plane], "reuse_ccso"); 
						stream.ReadFixed(1, out this.sb_reuse_ccso[plane], "sb_reuse_ccso"); 
					}

					if (((reuse_ccso[plane] != 0) || (sb_reuse_ccso[plane] != 0)))
					{
						n = CeilLog2(NumTotalRefs);
						stream.ReadVariable(n, out this.ccso_ref_idx[plane], "ccso_ref_idx"); 
						idx = ref_frame_idx[ccso_ref_idx[plane]];
						tmpCcsoLumaSizeLog2 = CcsoLumaSizeLog2;
						load_ccso_params(idx, plane); 
						CcsoLumaSizeLog2 = tmpCcsoLumaSizeLog2;
					}
				}

				if (((ccso_planes[plane] != 0) && !(reuse_ccso[plane] != 0)))
				{
					stream.ReadFixed(1, out this.ccso_bo_only[plane], "ccso_bo_only"); 
					stream.ReadFixed(2, out this.ccso_scale_idx[plane], "ccso_scale_idx"); 

					if ((ccso_bo_only[plane] != 0))
					{
						ccso_quant_idx[plane] = 0;
						ccso_ext_filter[plane] = 0;
						ccso_edge_clf[plane] = 0;
					}
					else 
					{
						stream.ReadFixed(2, out this.ccso_quant_idx[plane], "ccso_quant_idx"); 
						stream.ReadFixed(3, out this.ccso_ext_filter[plane], "ccso_ext_filter"); 
						quantStep = CCSO_Quant_Sz[ccso_scale_idx[plane]][ccso_quant_idx[plane]];

						if ((quantStep == 0))
						{
							ccso_edge_clf[plane] = 0;
						}
						else 
						{
							stream.ReadFixed(1, out this.ccso_edge_clf[plane], "ccso_edge_clf"); 
						}
					}
					n = (2 + ccso_bo_only[plane]);
					stream.ReadVariable(n, out this.ccso_max_band_log2[plane], "ccso_max_band_log2"); 
					maxEdgeInterval = (CCSO_INPUT_INTERVAL - ccso_edge_clf[plane]);

					if ((ccso_bo_only[plane] != 0))
					{
						maxEdgeInterval = 1;
					}
					maxBand = (1 << ccso_max_band_log2[plane]);

					for (d0 = 0; (d0 < maxEdgeInterval); d0++)
					{

						for (d1 = 0; (d1 < maxEdgeInterval); d1++)
						{

							for (band = 0; (band < maxBand); band++)
							{
								stream.ReadTu(7, out this.ccso_offset_idx, "ccso_offset_idx"); 
								offset = (Ccso_Offset[ccso_offset_idx] * (ccso_scale_idx[plane] + 1));
								CcsoFilterOffset[plane][band][d0][d1] = offset;
							}
						}
					}
				}
			}
        }

        private void WriteCcsoParams()
        {
			int plane = 0;
			int i = 0;
			int d0 = 0;
			int d1 = 0;
			int band = 0;
			int a = 0;
			int n = 0;
			int idx = 0;
			int tmpCcsoLumaSizeLog2 = 0;
			int quantStep = 0;
			int maxEdgeInterval = 0;
			int maxBand = 0;
			int offset = 0;

			for (plane = 0; (plane < NumPlanes); plane++)
			{
				ccso_planes[plane] = 0;
			}

			if (((CodedLossless != 0) || !(enable_ccso != 0)))
			{
				return;
			}
			a = 0;

			for (i = 0; (i < TileCols); i++)
			{
				a = (a | MiColStarts[i]);
			}

			for (i = 0; (i < TileRows); i++)
			{
				a = (a | MiRowStarts[i]);
			}

			if ((ccso_unit_matches_sb_size != 0))
			{
				CcsoLumaSizeLog2 = (Mi_Width_Log2[SbSize] + MI_SIZE_LOG2);
			}
			else if (((a & 63) == 0))
			{
				CcsoLumaSizeLog2 = 8;
			}
			else if (((a & 31) == 0))
			{
				CcsoLumaSizeLog2 = 7;
			}
			else 
			{
				CcsoLumaSizeLog2 = 6;
			}

			if ((single_picture_header_flag != 0))
			{
				ccso_frame_flag = 1;
			}
			else 
			{
				this.ccso_frame_flag = stream.Pick("ccso_frame_flag", _original != null ? _original.ccso_frame_flag : this.ccso_frame_flag, _edited != null ? _edited.ccso_frame_flag : _original != null ? _original.ccso_frame_flag : this.ccso_frame_flag);
				stream.WriteFixed(1, this.ccso_frame_flag, "ccso_frame_flag"); 
			}

			if (!(ccso_frame_flag != 0))
			{
				return;
			}

			for (plane = 0; (plane < NumPlanes); plane++)
			{
				this.ccso_planes[plane] = stream.Pick("ccso_planes", _original != null ? _original.ccso_planes[plane] : this.ccso_planes[plane], _edited != null ? _edited.ccso_planes[plane] : _original != null ? _original.ccso_planes[plane] : this.ccso_planes[plane]);
				stream.WriteFixed(1, this.ccso_planes[plane], "ccso_planes"); 

				if ((ccso_planes[plane] != 0))
				{

					if (((FrameIsIntra != 0) || (FrameType == SWITCH_FRAME)))
					{
						reuse_ccso[plane] = 0;
						sb_reuse_ccso[plane] = 0;
					}
					else 
					{
						this.reuse_ccso[plane] = stream.Pick("reuse_ccso", _original != null ? _original.reuse_ccso[plane] : this.reuse_ccso[plane], _edited != null ? _edited.reuse_ccso[plane] : _original != null ? _original.reuse_ccso[plane] : this.reuse_ccso[plane]);
						stream.WriteFixed(1, this.reuse_ccso[plane], "reuse_ccso"); 
						this.sb_reuse_ccso[plane] = stream.Pick("sb_reuse_ccso", _original != null ? _original.sb_reuse_ccso[plane] : this.sb_reuse_ccso[plane], _edited != null ? _edited.sb_reuse_ccso[plane] : _original != null ? _original.sb_reuse_ccso[plane] : this.sb_reuse_ccso[plane]);
						stream.WriteFixed(1, this.sb_reuse_ccso[plane], "sb_reuse_ccso"); 
					}

					if (((reuse_ccso[plane] != 0) || (sb_reuse_ccso[plane] != 0)))
					{
						n = CeilLog2(NumTotalRefs);
						this.ccso_ref_idx[plane] = stream.Pick("ccso_ref_idx", _original != null ? _original.ccso_ref_idx[plane] : this.ccso_ref_idx[plane], _edited != null ? _edited.ccso_ref_idx[plane] : _original != null ? _original.ccso_ref_idx[plane] : this.ccso_ref_idx[plane]);
						stream.WriteVariable(n, this.ccso_ref_idx[plane], "ccso_ref_idx"); 
						idx = ref_frame_idx[ccso_ref_idx[plane]];
						tmpCcsoLumaSizeLog2 = CcsoLumaSizeLog2;
						load_ccso_params(idx, plane); 
						CcsoLumaSizeLog2 = tmpCcsoLumaSizeLog2;
					}
				}

				if (((ccso_planes[plane] != 0) && !(reuse_ccso[plane] != 0)))
				{
					this.ccso_bo_only[plane] = stream.Pick("ccso_bo_only", _original != null ? _original.ccso_bo_only[plane] : this.ccso_bo_only[plane], _edited != null ? _edited.ccso_bo_only[plane] : _original != null ? _original.ccso_bo_only[plane] : this.ccso_bo_only[plane]);
					stream.WriteFixed(1, this.ccso_bo_only[plane], "ccso_bo_only"); 
					this.ccso_scale_idx[plane] = stream.Pick("ccso_scale_idx", _original != null ? _original.ccso_scale_idx[plane] : this.ccso_scale_idx[plane], _edited != null ? _edited.ccso_scale_idx[plane] : _original != null ? _original.ccso_scale_idx[plane] : this.ccso_scale_idx[plane]);
					stream.WriteFixed(2, this.ccso_scale_idx[plane], "ccso_scale_idx"); 

					if ((ccso_bo_only[plane] != 0))
					{
						ccso_quant_idx[plane] = 0;
						ccso_ext_filter[plane] = 0;
						ccso_edge_clf[plane] = 0;
					}
					else 
					{
						this.ccso_quant_idx[plane] = stream.Pick("ccso_quant_idx", _original != null ? _original.ccso_quant_idx[plane] : this.ccso_quant_idx[plane], _edited != null ? _edited.ccso_quant_idx[plane] : _original != null ? _original.ccso_quant_idx[plane] : this.ccso_quant_idx[plane]);
						stream.WriteFixed(2, this.ccso_quant_idx[plane], "ccso_quant_idx"); 
						this.ccso_ext_filter[plane] = stream.Pick("ccso_ext_filter", _original != null ? _original.ccso_ext_filter[plane] : this.ccso_ext_filter[plane], _edited != null ? _edited.ccso_ext_filter[plane] : _original != null ? _original.ccso_ext_filter[plane] : this.ccso_ext_filter[plane]);
						stream.WriteFixed(3, this.ccso_ext_filter[plane], "ccso_ext_filter"); 
						quantStep = CCSO_Quant_Sz[ccso_scale_idx[plane]][ccso_quant_idx[plane]];

						if ((quantStep == 0))
						{
							ccso_edge_clf[plane] = 0;
						}
						else 
						{
							this.ccso_edge_clf[plane] = stream.Pick("ccso_edge_clf", _original != null ? _original.ccso_edge_clf[plane] : this.ccso_edge_clf[plane], _edited != null ? _edited.ccso_edge_clf[plane] : _original != null ? _original.ccso_edge_clf[plane] : this.ccso_edge_clf[plane]);
							stream.WriteFixed(1, this.ccso_edge_clf[plane], "ccso_edge_clf"); 
						}
					}
					n = (2 + ccso_bo_only[plane]);
					this.ccso_max_band_log2[plane] = stream.Pick("ccso_max_band_log2", _original != null ? _original.ccso_max_band_log2[plane] : this.ccso_max_band_log2[plane], _edited != null ? _edited.ccso_max_band_log2[plane] : _original != null ? _original.ccso_max_band_log2[plane] : this.ccso_max_band_log2[plane]);
					stream.WriteVariable(n, this.ccso_max_band_log2[plane], "ccso_max_band_log2"); 
					maxEdgeInterval = (CCSO_INPUT_INTERVAL - ccso_edge_clf[plane]);

					if ((ccso_bo_only[plane] != 0))
					{
						maxEdgeInterval = 1;
					}
					maxBand = (1 << ccso_max_band_log2[plane]);

					for (d0 = 0; (d0 < maxEdgeInterval); d0++)
					{

						for (d1 = 0; (d1 < maxEdgeInterval); d1++)
						{

							for (band = 0; (band < maxBand); band++)
							{
								this.ccso_offset_idx = stream.Pick("ccso_offset_idx", _original != null ? ccso_offset_index(_original.CcsoFilterOffset[plane][band][d0][d1], ccso_scale_idx[plane] + 1) : this.ccso_offset_idx, _edited != null ? ccso_offset_index(_edited.CcsoFilterOffset[plane][band][d0][d1], ccso_scale_idx[plane] + 1) : _original != null ? ccso_offset_index(_original.CcsoFilterOffset[plane][band][d0][d1], ccso_scale_idx[plane] + 1) : this.ccso_offset_idx);
								stream.WriteTu(7, this.ccso_offset_idx, "ccso_offset_idx"); 
								offset = (Ccso_Offset[ccso_offset_idx] * (ccso_scale_idx[plane] + 1));
								CcsoFilterOffset[plane][band][d0][d1] = offset;
							}
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
tx_mode_select	f(1)
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
skip_mode_params() {
if ( FrameIsIntra || FrameType == SWITCH_FRAME ) {	
skipModeAllowed = 0	
} else {	
skipModeAllowed = 1	
SkipModeFrame[ 0 ] = 0	
SkipModeFrame[ 1 ] = NumTotalRefs > 1 ? 1 : 0	
if ( NumTotalRefs > 1 ) {	
curToRef0 = Abs(get_relative_dist(OrderHint,	
RefOrderHint[ ref_frame_idx[ 0 ] ]))	
curToRef1 = Abs(get_relative_dist(OrderHint,	
RefOrderHint[ ref_frame_idx[ 1 ] ]))	
if ( OrderHints[ 0 ] == RESTRICTED_OH ) {	
curToRef0 = 0	
}	
if ( OrderHints[ 1 ] == RESTRICTED_OH ) {	
curToRef1 = 0	
}	
if ( Abs(curToRef0 - curToRef1) > 1 ) {	
SkipModeFrame[ 1 ] = 0	
}	
}	
}	
if ( skipModeAllowed ) {	
skip_mode_present	f(1)
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
			int skipModeAllowed = 0;
			int curToRef0 = 0;
			int curToRef1 = 0;

			if (((FrameIsIntra != 0) || (FrameType == SWITCH_FRAME)))
			{
				skipModeAllowed = 0;
			}
			else 
			{
				skipModeAllowed = 1;
				SkipModeFrame[0] = 0;
				SkipModeFrame[1] = ((NumTotalRefs > 1) ? 1 : 0);

				if ((NumTotalRefs > 1))
				{
					curToRef0 = Abs(GetRelativeDist(OrderHint, RefOrderHint[ref_frame_idx[0]]));
					curToRef1 = Abs(GetRelativeDist(OrderHint, RefOrderHint[ref_frame_idx[1]]));

					if ((OrderHints[0] == RESTRICTED_OH))
					{
						curToRef0 = 0;
					}

					if ((OrderHints[1] == RESTRICTED_OH))
					{
						curToRef1 = 0;
					}

					if ((Abs((curToRef0 - curToRef1)) > 1))
					{
						SkipModeFrame[1] = 0;
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
			int skipModeAllowed = 0;
			int curToRef0 = 0;
			int curToRef1 = 0;

			if (((FrameIsIntra != 0) || (FrameType == SWITCH_FRAME)))
			{
				skipModeAllowed = 0;
			}
			else 
			{
				skipModeAllowed = 1;
				SkipModeFrame[0] = 0;
				SkipModeFrame[1] = ((NumTotalRefs > 1) ? 1 : 0);

				if ((NumTotalRefs > 1))
				{
					curToRef0 = Abs(GetRelativeDist(OrderHint, RefOrderHint[ref_frame_idx[0]]));
					curToRef1 = Abs(GetRelativeDist(OrderHint, RefOrderHint[ref_frame_idx[1]]));

					if ((OrderHints[0] == RESTRICTED_OH))
					{
						curToRef0 = 0;
					}

					if ((OrderHints[1] == RESTRICTED_OH))
					{
						curToRef1 = 0;
					}

					if ((Abs((curToRef0 - curToRef1)) > 1))
					{
						SkipModeFrame[1] = 0;
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
frame_reference_mode() {
if ( FrameIsIntra ) {	
reference_select = 0	
} else {	
reference_select	f(1)
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
global_motion_params() {
for ( refc = 0; refc < REFS_PER_FRAME; refc++ ) {	
GmType[ refc ] = IDENTITY	
for ( i = 0; i < 6; i++ ) {	
gm_params[ refc ][ i ] = ( ( i % 3 == 2 ) ?	
1 << WARPEDMODEL_PREC_BITS : 0 )	
}	
}	
if ( FrameIsIntra || !enable_global_motion) {	
return	
}	
use_global_motion	f(1)
if ( !use_global_motion ) {	
return	
}	
for ( i = 0; i < 6; i++ ) {	
baseParams[ i ] = Default_Warp_Params[ i ]	
}	
baseDistance = 1	
if ( FrameType == SWITCH_FRAME ) {	
our_ref = NumTotalRefs	
} else {	
n = NumTotalRefs + 1	
our_ref	ns(n)
}	
if ( our_ref != NumTotalRefs ) {	
refIdx = ref_frame_idx[ our_ref ]	
if ( RefNumTotalRefs[ refIdx ] > 0 ) {	
n = RefNumTotalRefs[ refIdx ]	
their_ref	ns(n)
for ( i = 0; i < 6; i++ ) {	
baseParams[ i ] = SavedGmParams[ refIdx ][ their_ref ][ i ]	
}	
baseDistance = get_relative_dist(OrderHints[ our_ref ],	
SavedOrderHints[ refIdx ][ their_ref ])	
}	
}	
for ( refc = 0; refc < NumTotalRefs; refc++ ) {	
dist = get_relative_dist(OrderHint,OrderHints[ refc ])	
if ( dist == 0 || OrderHints[ refc ] == RESTRICTED_OH ) {	
for ( i = 0; i < 6; i++ ) {	
gm_params[ refc ][ i ] = Default_Warp_Params[ i ]	
}	
GmType[ refc ] = IDENTITY	
} else {	
for ( i = 0; i < 6; i++ ) {	
paramsc = scale_warp_model(baseParams, baseDistance, dist)	
PrevGmParams[ refc ][ i ] = paramsc[ i ]	
}	
is_global	f(1)
if ( is_global ) {	
is_rot_zoom	f(1)
if ( is_rot_zoom ) {	
type = ROTZOOM	
} else {	
type = AFFINE	
}	
} else {	
type = IDENTITY	
}	
GmType[ refc ] = type	
if ( type >= ROTZOOM ) {	
read_global_param(refc,2)	
read_global_param(refc,3)	
if ( type == AFFINE ) {	
read_global_param(refc,4)	
read_global_param(refc,5)	
} else {	
gm_params[ refc ][ 4 ] = -gm_params[ refc ][ 3 ]	
gm_params[ refc ][ 5 ] = gm_params[ refc ][ 2 ]	
}	
read_global_param(refc,0)	
read_global_param(refc,1)	
}	
}	
}	
}
    */
		private AomArray<int> GmType = new AomArray<int>();
		public AomArray<int> _GmType { get { return GmType; } set { GmType = value; } }
		private int use_global_motion;
		public int _UseGlobalMotion { get { return use_global_motion; } set { use_global_motion = value; } }
		private int our_ref;
		public int _OurRef { get { return our_ref; } set { our_ref = value; } }
		private int their_ref;
		public int _TheirRef { get { return their_ref; } set { their_ref = value; } }
		private AomArray<AomArray<int>> PrevGmParams = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _PrevGmParams { get { return PrevGmParams; } set { PrevGmParams = value; } }
		private int is_global;
		public int __IsGlobal { get { return is_global; } set { is_global = value; } }
		private int is_rot_zoom;
		public int _IsRotZoom { get { return is_rot_zoom; } set { is_rot_zoom = value; } }

        private void GlobalMotionParams()
        {
			int refc = 0;
			int i = 0;
			AomArray<int> baseParams = new AomArray<int>();
			int baseDistance = 0;
			int n = 0;
			int refIdx = 0;
			int dist = 0;
			AomArray<int> paramsc = new AomArray<int>();
			int type = 0;

			for (refc = 0; (refc < REFS_PER_FRAME); refc++)
			{
				GmType[refc] = IDENTITY;

				for (i = 0; (i < 6); i++)
				{
					gm_params[refc][i] = (((i % 3) == 2) ? (1 << WARPEDMODEL_PREC_BITS) : 0);
				}
			}

			if (((FrameIsIntra != 0) || !(enable_global_motion != 0)))
			{
				return;
			}
			stream.ReadFixed(1, out this.use_global_motion, "use_global_motion"); 

			if (!(use_global_motion != 0))
			{
				return;
			}

			for (i = 0; (i < 6); i++)
			{
				baseParams[i] = Default_Warp_Params[i];
			}
			baseDistance = 1;

			if ((FrameType == SWITCH_FRAME))
			{
				our_ref = NumTotalRefs;
			}
			else 
			{
				n = (NumTotalRefs + 1);
				stream.Read_ns(n, out this.our_ref, "our_ref"); 
			}

			if ((our_ref != NumTotalRefs))
			{
				refIdx = ref_frame_idx[our_ref];

				if ((RefNumTotalRefs[refIdx] > 0))
				{
					n = RefNumTotalRefs[refIdx];
					stream.Read_ns(n, out this.their_ref, "their_ref"); 

					for (i = 0; (i < 6); i++)
					{
						baseParams[i] = SavedGmParams[refIdx][their_ref][i];
					}
					baseDistance = GetRelativeDist(OrderHints[our_ref], SavedOrderHints[refIdx][their_ref]);
				}
			}

			for (refc = 0; (refc < NumTotalRefs); refc++)
			{
				dist = GetRelativeDist(OrderHint, OrderHints[refc]);

				if (((dist == 0) || (OrderHints[refc] == RESTRICTED_OH)))
				{

					for (i = 0; (i < 6); i++)
					{
						gm_params[refc][i] = Default_Warp_Params[i];
					}
					GmType[refc] = IDENTITY;
				}
				else 
				{

					for (i = 0; (i < 6); i++)
					{
						paramsc = ScaleWarpModel(baseParams, baseDistance, dist);
						PrevGmParams[refc][i] = paramsc[i];
					}
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
							type = AFFINE;
						}
					}
					else 
					{
						type = IDENTITY;
					}
					GmType[refc] = type;

					if ((type >= ROTZOOM))
					{
						ReadGlobalParam(refc, 2); 
						ReadGlobalParam(refc, 3); 

						if ((type == AFFINE))
						{
							ReadGlobalParam(refc, 4); 
							ReadGlobalParam(refc, 5); 
						}
						else 
						{
							gm_params[refc][4] = -gm_params[refc][3];
							gm_params[refc][5] = gm_params[refc][2];
						}
						ReadGlobalParam(refc, 0); 
						ReadGlobalParam(refc, 1); 
					}
				}
			}
        }

        private void WriteGlobalMotionParams()
        {
			int refc = 0;
			int i = 0;
			AomArray<int> baseParams = new AomArray<int>();
			int baseDistance = 0;
			int n = 0;
			int refIdx = 0;
			int dist = 0;
			AomArray<int> paramsc = new AomArray<int>();
			int type = 0;

			for (refc = 0; (refc < REFS_PER_FRAME); refc++)
			{
				GmType[refc] = IDENTITY;

				for (i = 0; (i < 6); i++)
				{
					gm_params[refc][i] = (((i % 3) == 2) ? (1 << WARPEDMODEL_PREC_BITS) : 0);
				}
			}

			if (((FrameIsIntra != 0) || !(enable_global_motion != 0)))
			{
				return;
			}
			this.use_global_motion = stream.Pick("use_global_motion", _original != null ? _original.use_global_motion : this.use_global_motion, _edited != null ? _edited.use_global_motion : _original != null ? _original.use_global_motion : this.use_global_motion);
			stream.WriteFixed(1, this.use_global_motion, "use_global_motion"); 

			if (!(use_global_motion != 0))
			{
				return;
			}

			for (i = 0; (i < 6); i++)
			{
				baseParams[i] = Default_Warp_Params[i];
			}
			baseDistance = 1;

			if ((FrameType == SWITCH_FRAME))
			{
				our_ref = NumTotalRefs;
			}
			else 
			{
				n = (NumTotalRefs + 1);
				this.our_ref = stream.Pick("our_ref", _original != null ? _original.our_ref : this.our_ref, _edited != null ? _edited.our_ref : _original != null ? _original.our_ref : this.our_ref);
				stream.Write_ns(n, this.our_ref, "our_ref"); 
			}

			if ((our_ref != NumTotalRefs))
			{
				refIdx = ref_frame_idx[our_ref];

				if ((RefNumTotalRefs[refIdx] > 0))
				{
					n = RefNumTotalRefs[refIdx];
					this.their_ref = stream.Pick("their_ref", _original != null ? _original.their_ref : this.their_ref, _edited != null ? _edited.their_ref : _original != null ? _original.their_ref : this.their_ref);
					stream.Write_ns(n, this.their_ref, "their_ref"); 

					for (i = 0; (i < 6); i++)
					{
						baseParams[i] = SavedGmParams[refIdx][their_ref][i];
					}
					baseDistance = GetRelativeDist(OrderHints[our_ref], SavedOrderHints[refIdx][their_ref]);
				}
			}

			for (refc = 0; (refc < NumTotalRefs); refc++)
			{
				dist = GetRelativeDist(OrderHint, OrderHints[refc]);

				if (((dist == 0) || (OrderHints[refc] == RESTRICTED_OH)))
				{

					for (i = 0; (i < 6); i++)
					{
						gm_params[refc][i] = Default_Warp_Params[i];
					}
					GmType[refc] = IDENTITY;
				}
				else 
				{

					for (i = 0; (i < 6); i++)
					{
						paramsc = ScaleWarpModel(baseParams, baseDistance, dist);
						PrevGmParams[refc][i] = paramsc[i];
					}
					this.is_global = stream.Pick("is_global", _original != null ? _original.is_global : this.is_global, _edited != null ? _edited.is_global : _original != null ? _original.is_global : this.is_global);
					stream.WriteFixed(1, this.is_global, "is_global"); 

					if ((is_global != 0))
					{
						this.is_rot_zoom = stream.Pick("is_rot_zoom", _original != null ? _original.is_rot_zoom : this.is_rot_zoom, _edited != null ? _edited.is_rot_zoom : _original != null ? _original.is_rot_zoom : this.is_rot_zoom);
						stream.WriteFixed(1, this.is_rot_zoom, "is_rot_zoom"); 

						if ((is_rot_zoom != 0))
						{
							type = ROTZOOM;
						}
						else 
						{
							type = AFFINE;
						}
					}
					else 
					{
						type = IDENTITY;
					}
					GmType[refc] = type;

					if ((type >= ROTZOOM))
					{
						WriteReadGlobalParam(refc, 2); 
						WriteReadGlobalParam(refc, 3); 

						if ((type == AFFINE))
						{
							WriteReadGlobalParam(refc, 4); 
							WriteReadGlobalParam(refc, 5); 
						}
						else 
						{
							gm_params[refc][4] = -gm_params[refc][3];
							gm_params[refc][5] = gm_params[refc][2];
						}
						WriteReadGlobalParam(refc, 0); 
						WriteReadGlobalParam(refc, 1); 
					}
				}
			}
        }

    /*
read_global_param( refc, idx ) {
precBits = GM_ALPHA_PREC_BITS	
mx = GM_ALPHA_MAX	
if ( idx < 2 ) {	
precBits = GM_TRANS_PREC_BITS	
mx = GM_TRANS_MAX	
}	
precDiff = WARPEDMODEL_PREC_BITS - precBits	
round = (idx % 3) == 2 ? (1 << WARPEDMODEL_PREC_BITS) : 0	
sub = (idx % 3) == 2 ? (1 << precBits) : 0	
r = (PrevGmParams[ refc ][ idx ] >> precDiff) - sub	
gm_params[ refc ][ idx ] =	
(decode_signed_subexp_with_ref( -mx, mx + 1, r, 3 ) << precDiff) + round	
}
    */
		private int idx;
		public int _Idx { get { return idx; } set { idx = value; } }

        private void ReadGlobalParam(int refc, int idx)
        {
			int precBits = 0;
			int mx = 0;
			int precDiff = 0;
			int round = 0;
			int sub = 0;
			int r = 0;
			precBits = GM_ALPHA_PREC_BITS;
			mx = GM_ALPHA_MAX;

			if ((idx < 2))
			{
				precBits = GM_TRANS_PREC_BITS;
				mx = GM_TRANS_MAX;
			}
			precDiff = (WARPEDMODEL_PREC_BITS - precBits);
			round = (((idx % 3) == 2) ? (1 << WARPEDMODEL_PREC_BITS) : 0);
			sub = (((idx % 3) == 2) ? (1 << precBits) : 0);
			r = ((PrevGmParams[refc][idx] >> precDiff) - sub);
			gm_params[refc][idx] = ((DecodeSignedSubexpWithRef(-mx, (mx + 1), r, 3) << precDiff) + round);
        }

        private void WriteReadGlobalParam(int refc, int idx)
        {
			int precBits = 0;
			int mx = 0;
			int precDiff = 0;
			int round = 0;
			int sub = 0;
			int r = 0;
			precBits = GM_ALPHA_PREC_BITS;
			mx = GM_ALPHA_MAX;

			if ((idx < 2))
			{
				precBits = GM_TRANS_PREC_BITS;
				mx = GM_TRANS_MAX;
			}
			precDiff = (WARPEDMODEL_PREC_BITS - precBits);
			round = (((idx % 3) == 2) ? (1 << WARPEDMODEL_PREC_BITS) : 0);
			sub = (((idx % 3) == 2) ? (1 << precBits) : 0);
			r = ((PrevGmParams[refc][idx] >> precDiff) - sub);
			gm_params[refc][idx] = ((WriteDecodeSignedSubexpWithRef(-mx, (mx + 1), r, 3) << precDiff) + round);
        }

    /*
decode_signed_subexp_with_ref( low, high, r, k ) {
x = decode_unsigned_subexp_with_ref(high - low, r - low, k)	
return x + low	
}
    */
		private int low;
		public int _Low { get { return low; } set { low = value; } }
		private int high;
		public int _High { get { return high; } set { high = value; } }

        private int DecodeSignedSubexpWithRef(int low, int high, int r, int k)
        {
			int x = 0;
			x = DecodeUnsignedSubexpWithRef((high - low), (r - low), k);
			return (x + low);
        }

        private int WriteDecodeSignedSubexpWithRef(int low, int high, int r, int k)
        {
			int x = 0;
			x = WriteDecodeUnsignedSubexpWithRef((high - low), (r - low), k);
			return (x + low);
        }

    /*
decode_unsigned_subexp_with_ref( mx, r, k ) {
v = decode_subexp( mx, k )	
if ( (r << 1) <= mx ) {	
return inverse_recenter(r, v)	
} else {	
return mx - 1 - inverse_recenter(mx - 1 - r, v)	
}	
}
    */
		private int mx;
		public int _Mx { get { return mx; } set { mx = value; } }

        private int DecodeUnsignedSubexpWithRef(int mx, int r, int k)
        {
			int v = 0;
			v = DecodeSubexp(mx, k);

			if (((r << 1) <= mx))
			{
				return InverseRecenter(r, v);
			}
			else 
			{
				return ((mx - 1) - InverseRecenter(((mx - 1) - r), v));
			}
        }

        private int WriteDecodeUnsignedSubexpWithRef(int mx, int r, int k)
        {
			int v = 0;
			v = WriteDecodeSubexp(mx, k);

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
decode_subexp( numSyms, k ) {
i = 0	
mk = 0	
while ( 1 ) {	
b2 = i ? k + i - 1 : k	
a = 1 << b2	
if ( numSyms <= mk + 3 * a ) {	
n = numSyms - mk	
subexp_final_bits	ns(n)
return subexp_final_bits + mk	
} else {	
subexp_more_bits	f(1)
if ( subexp_more_bits ) {	
i++	
mk += a	
} else {	
subexp_bits	f(b2)
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

        private int DecodeSubexp(int numSyms, int k)
        {
			int i = 0;
			int mk = 0;
			int b2 = 0;
			int a = 0;
			int n = 0;
			i = 0;
			mk = 0;

			while ((1 != 0))
			{
				b2 = ((i != 0) ? ((k + i) - 1) : k);
				a = (1 << b2);

				if ((numSyms <= (mk + (3 * a))))
				{
					n = (numSyms - mk);
					stream.Read_ns(n, out this.subexp_final_bits, "subexp_final_bits"); 
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

        private int WriteDecodeSubexp(int numSyms, int k)
        {
			int i = 0;
			int mk = 0;
			int b2 = 0;
			int a = 0;
			int n = 0;
			i = 0;
			mk = 0;

			while ((1 != 0))
			{
				b2 = ((i != 0) ? ((k + i) - 1) : k);
				a = (1 << b2);

				if ((numSyms <= (mk + (3 * a))))
				{
					n = (numSyms - mk);
					this.subexp_final_bits = stream.Pick("subexp_final_bits", _original != null ? _original.subexp_final_bits : this.subexp_final_bits, _edited != null ? _edited.subexp_final_bits : _original != null ? _original.subexp_final_bits : this.subexp_final_bits);
					stream.Write_ns(n, this.subexp_final_bits, "subexp_final_bits"); 
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
film_grain_config() {
if ( !film_grain_params_present || ( !immediate_output_frame && !implicit_output_frame ) ) {	
apply_grain = 0	
} else if ( single_picture_header_flag ) {	
apply_grain = 1	
} else {	
apply_grain	f(1)
}	
if ( apply_grain ) {	
fgm_id	f(3)
load_grain_model( fgm_id )	
grain_seed	f(16)
}	
}
    */
		private int apply_grain;
		public int _ApplyGrain { get { return apply_grain; } set { apply_grain = value; } }
		private int fgm_id;
		public int _FgmId { get { return fgm_id; } set { fgm_id = value; } }
		private int grain_seed;
		public int _GrainSeed { get { return grain_seed; } set { grain_seed = value; } }

        private void FilmGrainConfig()
        {

			if ((!(film_grain_params_present != 0) || (!(immediate_output_frame != 0) && !(implicit_output_frame != 0))))
			{
				apply_grain = 0;
			}
			else if ((single_picture_header_flag != 0))
			{
				apply_grain = 1;
			}
			else 
			{
				stream.ReadFixed(1, out this.apply_grain, "apply_grain"); 
			}

			if ((apply_grain != 0))
			{
				stream.ReadFixed(3, out this.fgm_id, "fgm_id"); 
				load_grain_model(fgm_id); 
				stream.ReadFixed(16, out this.grain_seed, "grain_seed"); 
			}
        }

        private void WriteFilmGrainConfig()
        {

			if ((!(film_grain_params_present != 0) || (!(immediate_output_frame != 0) && !(implicit_output_frame != 0))))
			{
				apply_grain = 0;
			}
			else if ((single_picture_header_flag != 0))
			{
				apply_grain = 1;
			}
			else 
			{
				this.apply_grain = stream.Pick("apply_grain", _original != null ? _original.apply_grain : this.apply_grain, _edited != null ? _edited.apply_grain : _original != null ? _original.apply_grain : this.apply_grain);
				stream.WriteFixed(1, this.apply_grain, "apply_grain"); 
			}

			if ((apply_grain != 0))
			{
				this.fgm_id = stream.Pick("fgm_id", _original != null ? _original.fgm_id : this.fgm_id, _edited != null ? _edited.fgm_id : _original != null ? _original.fgm_id : this.fgm_id);
				stream.WriteFixed(3, this.fgm_id, "fgm_id"); 
				load_grain_model(fgm_id); 
				this.grain_seed = stream.Pick("grain_seed", _original != null ? _original.grain_seed : this.grain_seed, _edited != null ? _edited.grain_seed : _original != null ? _original.grain_seed : this.grain_seed);
				stream.WriteFixed(16, this.grain_seed, "grain_seed"); 
			}
        }

    /*
film_grain_model( monochrome, subX, subY ) {
if ( monochrome ) {	
chroma_scaling_from_luma = 0	
} else {	
chroma_scaling_from_luma	f(1)
}	
num_y_points	f(4)
if ( num_y_points > 0) {	
point_value_increment_bits_minus_1	f(3)
bitsIncr = point_value_increment_bits_minus_1 + 1	
point_scaling_bits_minus_5	f(2)
bitsScal = point_scaling_bits_minus_5 + 5	
}	
for ( i = 0; i < num_y_points; i++ ) {	
point_y_value[ i ]	f(bitsIncr)
if ( i > 0 ) {	
point_y_value[ i ] += point_y_value[ i - 1 ]	
}	
point_y_scaling[ i ]	f(bitsScal)
}	
if ( monochrome || chroma_scaling_from_luma ) {	
num_cb_points = 0	
num_cr_points = 0	
} else {	
num_cb_points	f(4)
if ( num_cb_points > 0 ) {	
point_value_increment_bits_minus_1	f(3)
bitsIncr = point_value_increment_bits_minus_1 + 1	
point_scaling_bits_minus_5	f(2)
bitsScal = point_scaling_bits_minus_5 + 5	
}	
for ( i = 0; i < num_cb_points; i++ ) {	
point_cb_value[ i ]	f(bitsIncr)
if ( i > 0 ) {	
point_cb_value[ i ] += point_cb_value[ i - 1 ]	
}	
point_cb_scaling[ i ]	f(bitsScal)
}	
num_cr_points	f(4)
if ( num_cr_points > 0 ) {	
point_value_increment_bits_minus_1	f(3)
bitsIncr = point_value_increment_bits_minus_1 + 1	
point_scaling_bits_minus_5	f(2)
bitsScal = point_scaling_bits_minus_5 + 5	
}	
for ( i = 0; i < num_cr_points; i++ ) {	
point_cr_value[ i ]	f(bitsIncr)
if ( i > 0 ) {	
point_cr_value[ i ] += point_cr_value[ i - 1 ]	
}	
point_cr_scaling[ i ]	f(bitsScal)
}	
}	
grain_scaling_minus_8	f(2)
ar_coeff_lag	f(2)
numPosLuma = 2 * ar_coeff_lag * ( ar_coeff_lag + 1 )	
if ( num_y_points ) {	
bits_per_ar_coeff_y_minus_5	f(2)
bitsCoef = bits_per_ar_coeff_y_minus_5 + 5	
numPosChroma = numPosLuma + 1	
for ( i = 0; i < numPosLuma; i++ ) {	
ar_coeffs_y[ i ]	f(bitsCoef)
ar_coeffs_y[ i ] -= (1 << (bitsCoef - 1))	
}	
} else {	
numPosChroma = numPosLuma	
}	
if ( chroma_scaling_from_luma || num_cb_points ) {	
bits_per_ar_coeff_cb_minus_5	f(2)
bitsCoef = bits_per_ar_coeff_cb_minus_5 + 5	
for ( i = 0; i < numPosChroma; i++ ) {	
ar_coeffs_cb[ i ]	f(bitsCoef)
ar_coeffs_cb[ i ] -= (1 << (bitsCoef - 1))	
}	
}	
if ( chroma_scaling_from_luma || num_cr_points ) {	
bits_per_ar_coeff_cr_minus_5	f(2)
bitsCoef = bits_per_ar_coeff_cr_minus_5 + 5	
for ( i = 0; i < numPosChroma; i++ ) {	
ar_coeffs_cr[ i ]	f(bitsCoef)
ar_coeffs_cr[ i ] -= (1 << (bitsCoef - 1))	
}	
}	
ar_coeff_shift_minus_6	f(2)
grain_scale_shift	f(2)
if ( num_cb_points ) {	
cb_mult	f(8)
cb_luma_mult	f(8)
cb_offset	f(9)
}	
if ( num_cr_points ) {	
cr_mult	f(8)
cr_luma_mult	f(8)
cr_offset	f(9)
}	
overlap_flag	f(1)
clip_to_restricted_range	f(1)
if ( clip_to_restricted_range ) {	
fg_mc_identity	f(1)
} else {	
fg_mc_identity = 0	
}	
film_grain_block_size	f(1)
}
    */
		private int monochrome;
		public int _Monochrome { get { return monochrome; } set { monochrome = value; } }
		private int subX;
		public int _SubX { get { return subX; } set { subX = value; } }
		private int subY;
		public int _SubY { get { return subY; } set { subY = value; } }
		private int chroma_scaling_from_luma;
		public int _ChromaScalingFromLuma { get { return chroma_scaling_from_luma; } set { chroma_scaling_from_luma = value; } }
		private int num_y_points;
		public int _NumyPoints { get { return num_y_points; } set { num_y_points = value; } }
		private int point_value_increment_bits_minus_1;
		public int _PointValueIncrementBitsMinus1 { get { return point_value_increment_bits_minus_1; } set { point_value_increment_bits_minus_1 = value; } }
		private int point_scaling_bits_minus_5;
		public int _PointScalingBitsMinus5 { get { return point_scaling_bits_minus_5; } set { point_scaling_bits_minus_5 = value; } }
		private AomArray<int> point_y_value = new AomArray<int>();
		public AomArray<int> _PointyValue { get { return point_y_value; } set { point_y_value = value; } }
		private AomArray<int> point_y_scaling = new AomArray<int>();
		public AomArray<int> _PointyScaling { get { return point_y_scaling; } set { point_y_scaling = value; } }
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
		private int bits_per_ar_coeff_y_minus_5;
		public int _BitsPerArCoeffyMinus5 { get { return bits_per_ar_coeff_y_minus_5; } set { bits_per_ar_coeff_y_minus_5 = value; } }
		private AomArray<int> ar_coeffs_y = new AomArray<int>();
		public AomArray<int> _ArCoeffsy { get { return ar_coeffs_y; } set { ar_coeffs_y = value; } }
		private int bits_per_ar_coeff_cb_minus_5;
		public int _BitsPerArCoeffCbMinus5 { get { return bits_per_ar_coeff_cb_minus_5; } set { bits_per_ar_coeff_cb_minus_5 = value; } }
		private AomArray<int> ar_coeffs_cb = new AomArray<int>();
		public AomArray<int> _ArCoeffsCb { get { return ar_coeffs_cb; } set { ar_coeffs_cb = value; } }
		private int bits_per_ar_coeff_cr_minus_5;
		public int _BitsPerArCoeffCrMinus5 { get { return bits_per_ar_coeff_cr_minus_5; } set { bits_per_ar_coeff_cr_minus_5 = value; } }
		private AomArray<int> ar_coeffs_cr = new AomArray<int>();
		public AomArray<int> _ArCoeffsCr { get { return ar_coeffs_cr; } set { ar_coeffs_cr = value; } }
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
		private int fg_mc_identity;
		public int _FgMcIdentity { get { return fg_mc_identity; } set { fg_mc_identity = value; } }
		private int film_grain_block_size;
		public int _FilmGrainBlockSize { get { return film_grain_block_size; } set { film_grain_block_size = value; } }

        private void FilmGrainModel(int monochrome, int subX, int subY)
        {
			int i = 0;
			int bitsIncr = 0;
			int bitsScal = 0;
			int numPosLuma = 0;
			int bitsCoef = 0;
			int numPosChroma = 0;

			if ((monochrome != 0))
			{
				chroma_scaling_from_luma = 0;
			}
			else 
			{
				stream.ReadFixed(1, out this.chroma_scaling_from_luma, "chroma_scaling_from_luma"); 
			}
			stream.ReadFixed(4, out this.num_y_points, "num_y_points"); 

			if ((num_y_points > 0))
			{
				stream.ReadFixed(3, out this.point_value_increment_bits_minus_1, "point_value_increment_bits_minus_1"); 
				bitsIncr = (point_value_increment_bits_minus_1 + 1);
				stream.ReadFixed(2, out this.point_scaling_bits_minus_5, "point_scaling_bits_minus_5"); 
				bitsScal = (point_scaling_bits_minus_5 + 5);
			}

			for (i = 0; (i < num_y_points); i++)
			{
				stream.ReadVariable(bitsIncr, out this.point_y_value[i], "point_y_value"); 

				if ((i > 0))
				{
					point_y_value[i] += point_y_value[(i - 1)];
				}
				stream.ReadVariable(bitsScal, out this.point_y_scaling[i], "point_y_scaling"); 
			}

			if (((monochrome != 0) || (chroma_scaling_from_luma != 0)))
			{
				num_cb_points = 0;
				num_cr_points = 0;
			}
			else 
			{
				stream.ReadFixed(4, out this.num_cb_points, "num_cb_points"); 

				if ((num_cb_points > 0))
				{
					stream.ReadFixed(3, out this.point_value_increment_bits_minus_1, "point_value_increment_bits_minus_1"); 
					bitsIncr = (point_value_increment_bits_minus_1 + 1);
					stream.ReadFixed(2, out this.point_scaling_bits_minus_5, "point_scaling_bits_minus_5"); 
					bitsScal = (point_scaling_bits_minus_5 + 5);
				}

				for (i = 0; (i < num_cb_points); i++)
				{
					stream.ReadVariable(bitsIncr, out this.point_cb_value[i], "point_cb_value"); 

					if ((i > 0))
					{
						point_cb_value[i] += point_cb_value[(i - 1)];
					}
					stream.ReadVariable(bitsScal, out this.point_cb_scaling[i], "point_cb_scaling"); 
				}
				stream.ReadFixed(4, out this.num_cr_points, "num_cr_points"); 

				if ((num_cr_points > 0))
				{
					stream.ReadFixed(3, out this.point_value_increment_bits_minus_1, "point_value_increment_bits_minus_1"); 
					bitsIncr = (point_value_increment_bits_minus_1 + 1);
					stream.ReadFixed(2, out this.point_scaling_bits_minus_5, "point_scaling_bits_minus_5"); 
					bitsScal = (point_scaling_bits_minus_5 + 5);
				}

				for (i = 0; (i < num_cr_points); i++)
				{
					stream.ReadVariable(bitsIncr, out this.point_cr_value[i], "point_cr_value"); 

					if ((i > 0))
					{
						point_cr_value[i] += point_cr_value[(i - 1)];
					}
					stream.ReadVariable(bitsScal, out this.point_cr_scaling[i], "point_cr_scaling"); 
				}
			}
			stream.ReadFixed(2, out this.grain_scaling_minus_8, "grain_scaling_minus_8"); 
			stream.ReadFixed(2, out this.ar_coeff_lag, "ar_coeff_lag"); 
			numPosLuma = ((2 * ar_coeff_lag) * (ar_coeff_lag + 1));

			if ((num_y_points != 0))
			{
				stream.ReadFixed(2, out this.bits_per_ar_coeff_y_minus_5, "bits_per_ar_coeff_y_minus_5"); 
				bitsCoef = (bits_per_ar_coeff_y_minus_5 + 5);
				numPosChroma = (numPosLuma + 1);

				for (i = 0; (i < numPosLuma); i++)
				{
					stream.ReadVariable(bitsCoef, out this.ar_coeffs_y[i], "ar_coeffs_y"); 
					ar_coeffs_y[i] -= (1 << (bitsCoef - 1));
				}
			}
			else 
			{
				numPosChroma = numPosLuma;
			}

			if (((chroma_scaling_from_luma != 0) || (num_cb_points != 0)))
			{
				stream.ReadFixed(2, out this.bits_per_ar_coeff_cb_minus_5, "bits_per_ar_coeff_cb_minus_5"); 
				bitsCoef = (bits_per_ar_coeff_cb_minus_5 + 5);

				for (i = 0; (i < numPosChroma); i++)
				{
					stream.ReadVariable(bitsCoef, out this.ar_coeffs_cb[i], "ar_coeffs_cb"); 
					ar_coeffs_cb[i] -= (1 << (bitsCoef - 1));
				}
			}

			if (((chroma_scaling_from_luma != 0) || (num_cr_points != 0)))
			{
				stream.ReadFixed(2, out this.bits_per_ar_coeff_cr_minus_5, "bits_per_ar_coeff_cr_minus_5"); 
				bitsCoef = (bits_per_ar_coeff_cr_minus_5 + 5);

				for (i = 0; (i < numPosChroma); i++)
				{
					stream.ReadVariable(bitsCoef, out this.ar_coeffs_cr[i], "ar_coeffs_cr"); 
					ar_coeffs_cr[i] -= (1 << (bitsCoef - 1));
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

			if ((clip_to_restricted_range != 0))
			{
				stream.ReadFixed(1, out this.fg_mc_identity, "fg_mc_identity"); 
			}
			else 
			{
				fg_mc_identity = 0;
			}
			stream.ReadFixed(1, out this.film_grain_block_size, "film_grain_block_size"); 
        }

        private void WriteFilmGrainModel(int monochrome, int subX, int subY)
        {
			int i = 0;
			int bitsIncr = 0;
			int bitsScal = 0;
			int numPosLuma = 0;
			int bitsCoef = 0;
			int numPosChroma = 0;

			if ((monochrome != 0))
			{
				chroma_scaling_from_luma = 0;
			}
			else 
			{
				this.chroma_scaling_from_luma = stream.Pick("chroma_scaling_from_luma", _original != null ? _original.chroma_scaling_from_luma : this.chroma_scaling_from_luma, _edited != null ? _edited.chroma_scaling_from_luma : _original != null ? _original.chroma_scaling_from_luma : this.chroma_scaling_from_luma);
				stream.WriteFixed(1, this.chroma_scaling_from_luma, "chroma_scaling_from_luma"); 
			}
			this.num_y_points = stream.Pick("num_y_points", _original != null ? _original.num_y_points : this.num_y_points, _edited != null ? _edited.num_y_points : _original != null ? _original.num_y_points : this.num_y_points);
			stream.WriteFixed(4, this.num_y_points, "num_y_points"); 

			if ((num_y_points > 0))
			{
				this.point_value_increment_bits_minus_1 = stream.Pick("point_value_increment_bits_minus_1", _original != null ? _original.point_value_increment_bits_minus_1 : this.point_value_increment_bits_minus_1, _edited != null ? _edited.point_value_increment_bits_minus_1 : _original != null ? _original.point_value_increment_bits_minus_1 : this.point_value_increment_bits_minus_1);
				stream.WriteFixed(3, this.point_value_increment_bits_minus_1, "point_value_increment_bits_minus_1"); 
				bitsIncr = (point_value_increment_bits_minus_1 + 1);
				this.point_scaling_bits_minus_5 = stream.Pick("point_scaling_bits_minus_5", _original != null ? _original.point_scaling_bits_minus_5 : this.point_scaling_bits_minus_5, _edited != null ? _edited.point_scaling_bits_minus_5 : _original != null ? _original.point_scaling_bits_minus_5 : this.point_scaling_bits_minus_5);
				stream.WriteFixed(2, this.point_scaling_bits_minus_5, "point_scaling_bits_minus_5"); 
				bitsScal = (point_scaling_bits_minus_5 + 5);
			}

			for (i = 0; (i < num_y_points); i++)
			{
				this.point_y_value[i] = stream.Pick("point_y_value", _original != null ? ((i > 0) ? _original.point_y_value[i] - (point_y_value[(i - 1)]) : _original.point_y_value[i]) : this.point_y_value[i], _edited != null ? ((i > 0) ? _edited.point_y_value[i] - (point_y_value[(i - 1)]) : _edited.point_y_value[i]) : _original != null ? ((i > 0) ? _original.point_y_value[i] - (point_y_value[(i - 1)]) : _original.point_y_value[i]) : this.point_y_value[i]);
				stream.WriteVariable(bitsIncr, this.point_y_value[i], "point_y_value"); 

				if ((i > 0))
				{
					point_y_value[i] += point_y_value[(i - 1)];
				}
				this.point_y_scaling[i] = stream.Pick("point_y_scaling", _original != null ? _original.point_y_scaling[i] : this.point_y_scaling[i], _edited != null ? _edited.point_y_scaling[i] : _original != null ? _original.point_y_scaling[i] : this.point_y_scaling[i]);
				stream.WriteVariable(bitsScal, this.point_y_scaling[i], "point_y_scaling"); 
			}

			if (((monochrome != 0) || (chroma_scaling_from_luma != 0)))
			{
				num_cb_points = 0;
				num_cr_points = 0;
			}
			else 
			{
				this.num_cb_points = stream.Pick("num_cb_points", _original != null ? _original.num_cb_points : this.num_cb_points, _edited != null ? _edited.num_cb_points : _original != null ? _original.num_cb_points : this.num_cb_points);
				stream.WriteFixed(4, this.num_cb_points, "num_cb_points"); 

				if ((num_cb_points > 0))
				{
					this.point_value_increment_bits_minus_1 = stream.Pick("point_value_increment_bits_minus_1", _original != null ? _original.point_value_increment_bits_minus_1 : this.point_value_increment_bits_minus_1, _edited != null ? _edited.point_value_increment_bits_minus_1 : _original != null ? _original.point_value_increment_bits_minus_1 : this.point_value_increment_bits_minus_1);
					stream.WriteFixed(3, this.point_value_increment_bits_minus_1, "point_value_increment_bits_minus_1"); 
					bitsIncr = (point_value_increment_bits_minus_1 + 1);
					this.point_scaling_bits_minus_5 = stream.Pick("point_scaling_bits_minus_5", _original != null ? _original.point_scaling_bits_minus_5 : this.point_scaling_bits_minus_5, _edited != null ? _edited.point_scaling_bits_minus_5 : _original != null ? _original.point_scaling_bits_minus_5 : this.point_scaling_bits_minus_5);
					stream.WriteFixed(2, this.point_scaling_bits_minus_5, "point_scaling_bits_minus_5"); 
					bitsScal = (point_scaling_bits_minus_5 + 5);
				}

				for (i = 0; (i < num_cb_points); i++)
				{
					this.point_cb_value[i] = stream.Pick("point_cb_value", _original != null ? ((i > 0) ? _original.point_cb_value[i] - (point_cb_value[(i - 1)]) : _original.point_cb_value[i]) : this.point_cb_value[i], _edited != null ? ((i > 0) ? _edited.point_cb_value[i] - (point_cb_value[(i - 1)]) : _edited.point_cb_value[i]) : _original != null ? ((i > 0) ? _original.point_cb_value[i] - (point_cb_value[(i - 1)]) : _original.point_cb_value[i]) : this.point_cb_value[i]);
					stream.WriteVariable(bitsIncr, this.point_cb_value[i], "point_cb_value"); 

					if ((i > 0))
					{
						point_cb_value[i] += point_cb_value[(i - 1)];
					}
					this.point_cb_scaling[i] = stream.Pick("point_cb_scaling", _original != null ? _original.point_cb_scaling[i] : this.point_cb_scaling[i], _edited != null ? _edited.point_cb_scaling[i] : _original != null ? _original.point_cb_scaling[i] : this.point_cb_scaling[i]);
					stream.WriteVariable(bitsScal, this.point_cb_scaling[i], "point_cb_scaling"); 
				}
				this.num_cr_points = stream.Pick("num_cr_points", _original != null ? _original.num_cr_points : this.num_cr_points, _edited != null ? _edited.num_cr_points : _original != null ? _original.num_cr_points : this.num_cr_points);
				stream.WriteFixed(4, this.num_cr_points, "num_cr_points"); 

				if ((num_cr_points > 0))
				{
					this.point_value_increment_bits_minus_1 = stream.Pick("point_value_increment_bits_minus_1", _original != null ? _original.point_value_increment_bits_minus_1 : this.point_value_increment_bits_minus_1, _edited != null ? _edited.point_value_increment_bits_minus_1 : _original != null ? _original.point_value_increment_bits_minus_1 : this.point_value_increment_bits_minus_1);
					stream.WriteFixed(3, this.point_value_increment_bits_minus_1, "point_value_increment_bits_minus_1"); 
					bitsIncr = (point_value_increment_bits_minus_1 + 1);
					this.point_scaling_bits_minus_5 = stream.Pick("point_scaling_bits_minus_5", _original != null ? _original.point_scaling_bits_minus_5 : this.point_scaling_bits_minus_5, _edited != null ? _edited.point_scaling_bits_minus_5 : _original != null ? _original.point_scaling_bits_minus_5 : this.point_scaling_bits_minus_5);
					stream.WriteFixed(2, this.point_scaling_bits_minus_5, "point_scaling_bits_minus_5"); 
					bitsScal = (point_scaling_bits_minus_5 + 5);
				}

				for (i = 0; (i < num_cr_points); i++)
				{
					this.point_cr_value[i] = stream.Pick("point_cr_value", _original != null ? ((i > 0) ? _original.point_cr_value[i] - (point_cr_value[(i - 1)]) : _original.point_cr_value[i]) : this.point_cr_value[i], _edited != null ? ((i > 0) ? _edited.point_cr_value[i] - (point_cr_value[(i - 1)]) : _edited.point_cr_value[i]) : _original != null ? ((i > 0) ? _original.point_cr_value[i] - (point_cr_value[(i - 1)]) : _original.point_cr_value[i]) : this.point_cr_value[i]);
					stream.WriteVariable(bitsIncr, this.point_cr_value[i], "point_cr_value"); 

					if ((i > 0))
					{
						point_cr_value[i] += point_cr_value[(i - 1)];
					}
					this.point_cr_scaling[i] = stream.Pick("point_cr_scaling", _original != null ? _original.point_cr_scaling[i] : this.point_cr_scaling[i], _edited != null ? _edited.point_cr_scaling[i] : _original != null ? _original.point_cr_scaling[i] : this.point_cr_scaling[i]);
					stream.WriteVariable(bitsScal, this.point_cr_scaling[i], "point_cr_scaling"); 
				}
			}
			this.grain_scaling_minus_8 = stream.Pick("grain_scaling_minus_8", _original != null ? _original.grain_scaling_minus_8 : this.grain_scaling_minus_8, _edited != null ? _edited.grain_scaling_minus_8 : _original != null ? _original.grain_scaling_minus_8 : this.grain_scaling_minus_8);
			stream.WriteFixed(2, this.grain_scaling_minus_8, "grain_scaling_minus_8"); 
			this.ar_coeff_lag = stream.Pick("ar_coeff_lag", _original != null ? _original.ar_coeff_lag : this.ar_coeff_lag, _edited != null ? _edited.ar_coeff_lag : _original != null ? _original.ar_coeff_lag : this.ar_coeff_lag);
			stream.WriteFixed(2, this.ar_coeff_lag, "ar_coeff_lag"); 
			numPosLuma = ((2 * ar_coeff_lag) * (ar_coeff_lag + 1));

			if ((num_y_points != 0))
			{
				this.bits_per_ar_coeff_y_minus_5 = stream.Pick("bits_per_ar_coeff_y_minus_5", _original != null ? _original.bits_per_ar_coeff_y_minus_5 : this.bits_per_ar_coeff_y_minus_5, _edited != null ? _edited.bits_per_ar_coeff_y_minus_5 : _original != null ? _original.bits_per_ar_coeff_y_minus_5 : this.bits_per_ar_coeff_y_minus_5);
				stream.WriteFixed(2, this.bits_per_ar_coeff_y_minus_5, "bits_per_ar_coeff_y_minus_5"); 
				bitsCoef = (bits_per_ar_coeff_y_minus_5 + 5);
				numPosChroma = (numPosLuma + 1);

				for (i = 0; (i < numPosLuma); i++)
				{
					this.ar_coeffs_y[i] = stream.Pick("ar_coeffs_y", _original != null ? (_original.ar_coeffs_y[i] + ((1 << (bitsCoef - 1)))) : this.ar_coeffs_y[i], _edited != null ? (_edited.ar_coeffs_y[i] + ((1 << (bitsCoef - 1)))) : _original != null ? (_original.ar_coeffs_y[i] + ((1 << (bitsCoef - 1)))) : this.ar_coeffs_y[i]);
					stream.WriteVariable(bitsCoef, this.ar_coeffs_y[i], "ar_coeffs_y"); 
					ar_coeffs_y[i] -= (1 << (bitsCoef - 1));
				}
			}
			else 
			{
				numPosChroma = numPosLuma;
			}

			if (((chroma_scaling_from_luma != 0) || (num_cb_points != 0)))
			{
				this.bits_per_ar_coeff_cb_minus_5 = stream.Pick("bits_per_ar_coeff_cb_minus_5", _original != null ? _original.bits_per_ar_coeff_cb_minus_5 : this.bits_per_ar_coeff_cb_minus_5, _edited != null ? _edited.bits_per_ar_coeff_cb_minus_5 : _original != null ? _original.bits_per_ar_coeff_cb_minus_5 : this.bits_per_ar_coeff_cb_minus_5);
				stream.WriteFixed(2, this.bits_per_ar_coeff_cb_minus_5, "bits_per_ar_coeff_cb_minus_5"); 
				bitsCoef = (bits_per_ar_coeff_cb_minus_5 + 5);

				for (i = 0; (i < numPosChroma); i++)
				{
					this.ar_coeffs_cb[i] = stream.Pick("ar_coeffs_cb", _original != null ? (_original.ar_coeffs_cb[i] + ((1 << (bitsCoef - 1)))) : this.ar_coeffs_cb[i], _edited != null ? (_edited.ar_coeffs_cb[i] + ((1 << (bitsCoef - 1)))) : _original != null ? (_original.ar_coeffs_cb[i] + ((1 << (bitsCoef - 1)))) : this.ar_coeffs_cb[i]);
					stream.WriteVariable(bitsCoef, this.ar_coeffs_cb[i], "ar_coeffs_cb"); 
					ar_coeffs_cb[i] -= (1 << (bitsCoef - 1));
				}
			}

			if (((chroma_scaling_from_luma != 0) || (num_cr_points != 0)))
			{
				this.bits_per_ar_coeff_cr_minus_5 = stream.Pick("bits_per_ar_coeff_cr_minus_5", _original != null ? _original.bits_per_ar_coeff_cr_minus_5 : this.bits_per_ar_coeff_cr_minus_5, _edited != null ? _edited.bits_per_ar_coeff_cr_minus_5 : _original != null ? _original.bits_per_ar_coeff_cr_minus_5 : this.bits_per_ar_coeff_cr_minus_5);
				stream.WriteFixed(2, this.bits_per_ar_coeff_cr_minus_5, "bits_per_ar_coeff_cr_minus_5"); 
				bitsCoef = (bits_per_ar_coeff_cr_minus_5 + 5);

				for (i = 0; (i < numPosChroma); i++)
				{
					this.ar_coeffs_cr[i] = stream.Pick("ar_coeffs_cr", _original != null ? (_original.ar_coeffs_cr[i] + ((1 << (bitsCoef - 1)))) : this.ar_coeffs_cr[i], _edited != null ? (_edited.ar_coeffs_cr[i] + ((1 << (bitsCoef - 1)))) : _original != null ? (_original.ar_coeffs_cr[i] + ((1 << (bitsCoef - 1)))) : this.ar_coeffs_cr[i]);
					stream.WriteVariable(bitsCoef, this.ar_coeffs_cr[i], "ar_coeffs_cr"); 
					ar_coeffs_cr[i] -= (1 << (bitsCoef - 1));
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

			if ((clip_to_restricted_range != 0))
			{
				this.fg_mc_identity = stream.Pick("fg_mc_identity", _original != null ? _original.fg_mc_identity : this.fg_mc_identity, _edited != null ? _edited.fg_mc_identity : _original != null ? _original.fg_mc_identity : this.fg_mc_identity);
				stream.WriteFixed(1, this.fg_mc_identity, "fg_mc_identity"); 
			}
			else 
			{
				fg_mc_identity = 0;
			}
			this.film_grain_block_size = stream.Pick("film_grain_block_size", _original != null ? _original.film_grain_block_size : this.film_grain_block_size, _edited != null ? _edited.film_grain_block_size : _original != null ? _original.film_grain_block_size : this.film_grain_block_size);
			stream.WriteFixed(1, this.film_grain_block_size, "film_grain_block_size"); 
        }

    /*
tile_group_obu( sz ) {
startBitPos = get_position()	
is_first_tile_group	f(1)
if ( is_first_tile_group ) {	
frame_header_present_flag = 1	
} else {	
frame_header_present_flag	f(1)
}	
if ( frame_header_present_flag ) {	
frame_header( is_first_tile_group )	
}	
if ( bru_inactive ) {	
headerBits = get_position() - startBitPos	
remainingBits = sz * 8 - headerBits	
trailing_bits( remainingBits )	
return	
}	
NumTiles = TileCols * TileRows	
tile_start_and_end_present_flag = 0	
if ( NumTiles > 1 ) {	
tile_start_and_end_present_flag	f(1)
}	
if ( NumTiles == 1 || !tile_start_and_end_present_flag ) {	
tg_start = 0	
tg_end = NumTiles - 1	
} else {	
tileBits = TileColsLog2 + TileRowsLog2	
tg_start	f(tileBits)
tg_end	f(tileBits)
}	
if ( use_bru ) {	
if ( NumTiles > 1 ) {	
for ( TileNum = tg_start; TileNum <= tg_end; TileNum++ ) {	
tileRow = TileNum / TileCols	
tileCol = TileNum % TileCols	
bru_tile_active	f(1)
BruTileActives[ tileRow ][ tileCol ] = bru_tile_active	
}	
} else {	
BruTileActives[ 0 ][ 0 ] = 1	
}	
}	
byte_alignment()	
endBitPos = get_position()	
headerBytes = (endBitPos - startBitPos) / 8	
sz -= headerBytes	
tile_group_payload( sz )	
}
    */
		private int is_first_tile_group;
		public int _IsFirstTileGroup { get { return is_first_tile_group; } set { is_first_tile_group = value; } }
		private int frame_header_present_flag;
		public int _FrameHeaderPresentFlag { get { return frame_header_present_flag; } set { frame_header_present_flag = value; } }
		private int tile_start_and_end_present_flag;
		public int _TileStartAndEndPresentFlag { get { return tile_start_and_end_present_flag; } set { tile_start_and_end_present_flag = value; } }
		private int bru_tile_active;
		public int _BruTileActive { get { return bru_tile_active; } set { bru_tile_active = value; } }
		private AomArray<AomArray<int>> BruTileActives = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _BruTileActives { get { return BruTileActives; } set { BruTileActives = value; } }

        private void TileGroupObu(int sz)
        {
			int TileNum = 0;
			int startBitPos = 0;
			int headerBits = 0;
			int remainingBits = 0;
			int tileBits = 0;
			int tileRow = 0;
			int tileCol = 0;
			int endBitPos = 0;
			int headerBytes = 0;
			startBitPos = get_position();
			stream.ReadFixed(1, out this.is_first_tile_group, "is_first_tile_group"); 

			if ((is_first_tile_group != 0))
			{
				frame_header_present_flag = 1;
			}
			else 
			{
				stream.ReadFixed(1, out this.frame_header_present_flag, "frame_header_present_flag"); 
			}

			if ((frame_header_present_flag != 0))
			{
				FrameHeader(is_first_tile_group); 
			}

			if ((bru_inactive != 0))
			{
				headerBits = (get_position() - startBitPos);
				remainingBits = ((sz * 8) - headerBits);
				TrailingBits(remainingBits); 
				return;
			}
			NumTiles = (TileCols * TileRows);
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

			if ((use_bru != 0))
			{

				if ((NumTiles > 1))
				{

					for (TileNum = tg_start; (TileNum <= tg_end); TileNum++)
					{
						tileRow = (TileNum / TileCols);
						tileCol = (TileNum % TileCols);
						stream.ReadFixed(1, out this.bru_tile_active, "bru_tile_active"); 
						BruTileActives[tileRow][tileCol] = bru_tile_active;
					}
				}
				else 
				{
					BruTileActives[0][0] = 1;
				}
			}
			ByteAlignment(); 
			endBitPos = get_position();
			headerBytes = ((endBitPos - startBitPos) / 8);
			sz -= headerBytes;
			TileGroupPayload(sz); 
        }

        private void WriteTileGroupObu(int sz)
        {
			int TileNum = 0;
			int startBitPos = 0;
			int headerBits = 0;
			int remainingBits = 0;
			int tileBits = 0;
			int tileRow = 0;
			int tileCol = 0;
			int endBitPos = 0;
			int headerBytes = 0;
			startBitPos = get_position();
			this.is_first_tile_group = stream.Pick("is_first_tile_group", _original != null ? (_original.frame_header_present_flag == 1 ? 1 : 0) : this.is_first_tile_group, _edited != null ? (_edited.frame_header_present_flag == 1 ? 1 : 0) : _original != null ? (_original.frame_header_present_flag == 1 ? 1 : 0) : this.is_first_tile_group);
			stream.WriteFixed(1, this.is_first_tile_group, "is_first_tile_group"); 

			if ((is_first_tile_group != 0))
			{
				frame_header_present_flag = 1;
			}
			else 
			{
				this.frame_header_present_flag = stream.Pick("frame_header_present_flag", _original != null ? _original.frame_header_present_flag : this.frame_header_present_flag, _edited != null ? _edited.frame_header_present_flag : _original != null ? _original.frame_header_present_flag : this.frame_header_present_flag);
				stream.WriteFixed(1, this.frame_header_present_flag, "frame_header_present_flag"); 
			}

			if ((frame_header_present_flag != 0))
			{
				WriteFrameHeader(is_first_tile_group); 
			}

			if ((bru_inactive != 0))
			{
				headerBits = (get_position() - startBitPos);
				remainingBits = ((sz * 8) - headerBits);
				WriteTrailingBits(remainingBits); 
				return;
			}
			NumTiles = (TileCols * TileRows);
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

			if ((use_bru != 0))
			{

				if ((NumTiles > 1))
				{

					for (TileNum = tg_start; (TileNum <= tg_end); TileNum++)
					{
						tileRow = (TileNum / TileCols);
						tileCol = (TileNum % TileCols);
						this.bru_tile_active = stream.Pick("bru_tile_active", _original != null ? _original.BruTileActives[tileRow][tileCol] : this.bru_tile_active, _edited != null ? _edited.BruTileActives[tileRow][tileCol] : _original != null ? _original.BruTileActives[tileRow][tileCol] : this.bru_tile_active);
						stream.WriteFixed(1, this.bru_tile_active, "bru_tile_active"); 
						BruTileActives[tileRow][tileCol] = bru_tile_active;
					}
				}
				else 
				{
					BruTileActives[0][0] = 1;
				}
			}
			WriteByteAlignment(); 
			endBitPos = get_position();
			headerBytes = ((endBitPos - startBitPos) / 8);
			sz -= headerBytes;
			WriteTileGroupPayload(sz); 
        }

    /*
tile_group_payload( sz ) {
for ( TileNum = tg_start; TileNum <= tg_end; TileNum++ ) {	
tileRow = TileNum / TileCols	
tileCol = TileNum % TileCols	
lastTile = TileNum == tg_end	
if ( lastTile ) {	
tileSize = sz	
} else if ( !IsBridge ) {	
tile_size_minus_1	le(TileSizeBytes)
tileSize = tile_size_minus_1 + 1	
sz -= tileSize + TileSizeBytes	
}	
MiRowStart = MiRowStarts[ tileRow ]	
MiRowEnd = MiRowStarts[ tileRow + 1 ]	
MiColStart = MiColStarts[ tileCol ]	
MiColEnd = MiColStarts[ tileCol + 1 ]	
BruTileActive = use_bru ? BruTileActives[ tileRow ][ tileCol ] : 0	
align = Num_4x4_Blocks_High[ SbSize ]	
shift = Mi_Height_Log2[ SbSize ]	
for( r = MiRowStart; r < ((MiRowEnd + align - 1) >> shift) << shift;	
r++) {	
for( c = MiColStart; c < ((MiColEnd + align - 1) >> shift) << shift;	
c++) {	
IBCCoded[ r ][ c ] = 0	
}	
}	
CurrentQIndex = base_q_idx	
if ( !IsBridge ) {	
init_symbol( tileSize )	
}	
decode_tile()	
if ( !IsBridge ) {	
exit_symbol()	
}	
}	
if ( tg_end == NumTiles - 1 ) {	
if ( !IsBridge ) {	
frame_end_update_cdf()	
}	
decode_frame_wrapup()	
SeenFrameHeader = 0	
}	
}
    */
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
		private int BruTileActive;
		public int __BruTileActive { get { return BruTileActive; } set { BruTileActive = value; } }
		private AomArray<AomArray<int>> IBCCoded = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _IBCCoded { get { return IBCCoded; } set { IBCCoded = value; } }
		private int CurrentQIndex;
		public int _CurrentQIndex { get { return CurrentQIndex; } set { CurrentQIndex = value; } }

        private void TileGroupPayload(int sz)
        {
			int TileNum = 0;
			int r = 0;
			int c = 0;
			int tileRow = 0;
			int tileCol = 0;
			int lastTile = 0;
			int tileSize = 0;
			int align = 0;
			int shift = 0;

			for (TileNum = tg_start; (TileNum <= tg_end); TileNum++)
			{
				tileRow = (TileNum / TileCols);
				tileCol = (TileNum % TileCols);
				lastTile = ((TileNum == tg_end) ? 1 : 0);

				if ((lastTile != 0))
				{
					tileSize = sz;
				}
				else if (!(IsBridge != 0))
				{
					stream.ReadLe(TileSizeBytes, out this.tile_size_minus_1, "tile_size_minus_1"); 
					tileSize = (tile_size_minus_1 + 1);
					sz -= (tileSize + TileSizeBytes);
				}
				MiRowStart = MiRowStarts[tileRow];
				MiRowEnd = MiRowStarts[(tileRow + 1)];
				MiColStart = MiColStarts[tileCol];
				MiColEnd = MiColStarts[(tileCol + 1)];
				BruTileActive = ((use_bru != 0) ? BruTileActives[tileRow][tileCol] : 0);
				align = Num_4x4_Blocks_High[SbSize];
				shift = Mi_Height_Log2[SbSize];

				for (r = MiRowStart; (r < ((((MiRowEnd + align) - 1) >> shift) << shift)); r++)
				{

					for (c = MiColStart; (c < ((((MiColEnd + align) - 1) >> shift) << shift)); c++)
					{
						IBCCoded[r][c] = 0;
					}
				}
				CurrentQIndex = base_q_idx;

				if (!(IsBridge != 0))
				{
					init_symbol(tileSize); 
				}
				decode_tile(); 

				if (!(IsBridge != 0))
				{
					exit_symbol(); 
				}
			}

			if ((tg_end == (NumTiles - 1)))
			{

				if (!(IsBridge != 0))
				{
					frame_end_update_cdf(); 
				}
				decode_frame_wrapup(); 
				SeenFrameHeader = 0;
			}
        }

        private void WriteTileGroupPayload(int sz)
        {
			int TileNum = 0;
			int r = 0;
			int c = 0;
			int tileRow = 0;
			int tileCol = 0;
			int lastTile = 0;
			int tileSize = 0;
			int align = 0;
			int shift = 0;

			for (TileNum = tg_start; (TileNum <= tg_end); TileNum++)
			{
				tileRow = (TileNum / TileCols);
				tileCol = (TileNum % TileCols);
				lastTile = ((TileNum == tg_end) ? 1 : 0);

				if ((lastTile != 0))
				{
					tileSize = sz;
				}
				else if (!(IsBridge != 0))
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
				BruTileActive = ((use_bru != 0) ? BruTileActives[tileRow][tileCol] : 0);
				align = Num_4x4_Blocks_High[SbSize];
				shift = Mi_Height_Log2[SbSize];

				for (r = MiRowStart; (r < ((((MiRowEnd + align) - 1) >> shift) << shift)); r++)
				{

					for (c = MiColStart; (c < ((((MiColEnd + align) - 1) >> shift) << shift)); c++)
					{
						IBCCoded[r][c] = 0;
					}
				}
				CurrentQIndex = base_q_idx;

				if (!(IsBridge != 0))
				{
					init_symbol(tileSize); 
				}
				decode_tile(); 

				if (!(IsBridge != 0))
				{
					exit_symbol(); 
				}
			}

			if ((tg_end == (NumTiles - 1)))
			{

				if (!(IsBridge != 0))
				{
					frame_end_update_cdf(); 
				}
				decode_frame_wrapup(); 
				SeenFrameHeader = 0;
			}
        }

    /*
compute_image_size ( ) {
/* AV2 specification v1.0.0, 5.18.4.4 Compute image size function *//*
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
    */
		private int MiCols;
		public int _MiCols { get { return MiCols; } set { MiCols = value; } }
		private int MiRows;
		public int _MiRows { get { return MiRows; } set { MiRows = value; } }
		private int CropLeft;
		public int _CropLeft { get { return CropLeft; } set { CropLeft = value; } }
		private int CropTop;
		public int _CropTop { get { return CropTop; } set { CropTop = value; } }
		private int CropWidth;
		public int _CropWidth { get { return CropWidth; } set { CropWidth = value; } }
		private int CropHeight;
		public int _CropHeight { get { return CropHeight; } set { CropHeight = value; } }

        private void ComputeImageSize()
        {
			int maxFrameWidth = 0;
			int maxFrameHeight = 0;
			int cropRight = 0;
			int cropBottom = 0;
/*  AV2 specification v1.0.0, 5.18.4.4 Compute image size function  */

			MiCols = (2 * ((FrameWidth + 7) >> 3));
			MiRows = (2 * ((FrameHeight + 7) >> 3));
			maxFrameWidth = (max_frame_width_minus_1 + 1);
			maxFrameHeight = (max_frame_height_minus_1 + 1);
			CropLeft = ((seq_cropping_win_left_offset * FrameWidth) / maxFrameWidth);
			cropRight = (FrameWidth - ((seq_cropping_win_right_offset * FrameWidth) / maxFrameWidth));
			CropTop = ((seq_cropping_win_top_offset * FrameHeight) / maxFrameHeight);
			cropBottom = (FrameHeight - ((seq_cropping_win_bottom_offset * FrameHeight) / maxFrameHeight));
			CropWidth = (cropRight - CropLeft);
			CropHeight = (cropBottom - CropTop);
        }

    /*
get_disp_order_hint ( ) {
/* AV2 specification v1.0.0, 5.18.2 Frame header info syntax *//*
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
    */

        private int GetDispOrderHint()
        {
			int maxDisp = 0;
			int dispOrderHint = 0;
			int offset = 0;
/*  AV2 specification v1.0.0, 5.18.2 Frame header info syntax  */


			if (((obu_type == OBU_CLOSED_LOOP_KEY) || ((!(IsSef() != 0) && (FrameType == SWITCH_FRAME)) && (restricted_prediction_switch != 0))))
			{
				return OrderHintLsbs;
			}
			maxDisp = GetMaxDispOrderHint(1);
			dispOrderHint = OrderHintLsbs;
			offset = ((maxDisp - ((1 << OrderHintBits) >> 1)) - OrderHintLsbs);

			if ((offset >= 0))
			{
				dispOrderHint += (((offset >> OrderHintBits) + 1) << OrderHintBits);
			}
			return dispOrderHint;
        }

    /*
get_relative_dist ( a , b ) {
/* AV2 specification v1.0.0, 5.18.3.1 Get relative distance function *//*
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
    */
		private int a;
		private int b;

        private int GetRelativeDist(int a, int b)
        {
/*  AV2 specification v1.0.0, 5.18.3.1 Get relative distance function  */


			if (((a == RESTRICTED_OH) && (b == RESTRICTED_OH)))
			{
				return 0;
			}
			else if ((a == RESTRICTED_OH))
			{
				return 127;
			}
			else if ((b == RESTRICTED_OH))
			{
				return -127;
			}
			else 
			{
				return Clip3(-127, 127, (a - b));
			}
        }

    /*
get_seq_sb_size () {
/* AV2 specification v1.0.0, 5.18.7.6 Get sequence superblock size function *//*
if ( use_256x256_superblock ) {
return BLOCK_256X256
} else if ( use_128x128_superblock ) {
return BLOCK_128X128
} else {
return BLOCK_64X64
}
}
    */

        private int GetSeqSbSize()
        {
/*  AV2 specification v1.0.0, 5.18.7.6 Get sequence superblock size function  */


			if ((use_256x256_superblock != 0))
			{
				return BLOCK_256X256;
			}
			else if ((use_128x128_superblock != 0))
			{
				return BLOCK_128X128;
			}
			else 
			{
				return BLOCK_64X64;
			}
        }

    /*
inverse_recenter ( r , v ) {
/* AV2 specification v1.0.0, 5.18.9.6 Inverse recenter function *//*
if ( v > 2 * r ) {
return v
} else if ( v & 1 ) {
return r - (( v + 1 ) >> 1 )
} else {
return r + ( v >> 1 )
}
}
    */
		private int v;

        private int InverseRecenter(int r, int v)
        {
/*  AV2 specification v1.0.0, 5.18.9.6 Inverse recenter function  */


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
is_extensible_obu () {
/* AV2 specification v1.0.0, 5.2.1 General OBU syntax *//*
return obu_type == OBU_SEQUENCE_HEADER ||
obu_type == OBU_MULTI_FRAME_HEADER ||
obu_type == OBU_LAYER_CONFIGURATION_RECORD ||
obu_type == OBU_CONTENT_INTERPRETATION ||
obu_type == OBU_OPERATING_POINT_SET ||
obu_type == OBU_ATLAS_SEGMENT
}
    */

        private int IsExtensibleObu()
        {
/*  AV2 specification v1.0.0, 5.2.1 General OBU syntax  */

			return (((((((obu_type == OBU_SEQUENCE_HEADER) || (obu_type == OBU_MULTI_FRAME_HEADER)) || (obu_type == OBU_LAYER_CONFIGURATION_RECORD)) || (obu_type == OBU_CONTENT_INTERPRETATION)) || (obu_type == OBU_OPERATING_POINT_SET)) || (obu_type == OBU_ATLAS_SEGMENT)) ? 1 : 0);
        }

    /*
is_sef () {
/* AV2 specification v1.0.0, 5.2.1 General OBU syntax *//*
return obu_type == OBU_LEADING_SEF || obu_type == OBU_REGULAR_SEF
}
    */

        private int IsSef()
        {
/*  AV2 specification v1.0.0, 5.2.1 General OBU syntax  */

			return (((obu_type == OBU_LEADING_SEF) || (obu_type == OBU_REGULAR_SEF)) ? 1 : 0);
        }

    /*
is_tile_group () {
/* AV2 specification v1.0.0, 5.2.1 General OBU syntax *//*
return obu_type == OBU_LEADING_TILE_GROUP ||
obu_type == OBU_REGULAR_TILE_GROUP ||
obu_type == OBU_CLOSED_LOOP_KEY ||
obu_type == OBU_OPEN_LOOP_KEY ||
obu_type == OBU_SWITCH ||
obu_type == OBU_RAS_FRAME
}
    */

        private int IsTileGroup()
        {
/*  AV2 specification v1.0.0, 5.2.1 General OBU syntax  */

			return (((((((obu_type == OBU_LEADING_TILE_GROUP) || (obu_type == OBU_REGULAR_TILE_GROUP)) || (obu_type == OBU_CLOSED_LOOP_KEY)) || (obu_type == OBU_OPEN_LOOP_KEY)) || (obu_type == OBU_SWITCH)) || (obu_type == OBU_RAS_FRAME)) ? 1 : 0);
        }

    /*
is_tip_frame () {
/* AV2 specification v1.0.0, 5.2.1 General OBU syntax *//*
return obu_type == OBU_LEADING_TIP || obu_type == OBU_REGULAR_TIP
}
    */

        private int IsTipFrame()
        {
/*  AV2 specification v1.0.0, 5.2.1 General OBU syntax  */

			return (((obu_type == OBU_LEADING_TIP) || (obu_type == OBU_REGULAR_TIP)) ? 1 : 0);
        }

    /*
load_xlayer_context ( obu_xlayer_id ) {
/* AV2 specification v1.0.0, 7.6 Extended layer context management *//*
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
    */

        private void LoadXlayerContext(int obu_xlayer_id)
        {
			int i = 0;
			int streamID = 0;
/*  AV2 specification v1.0.0, 7.6 Extended layer context management  */


			if ((obu_xlayer_id == GLOBAL_XLAYER_ID))
			{
				return;
			}

			if ((MultiStreamDecoderMode != 0))
			{

				for (i = 0; (i < (num_streams_minus_2 + 2)); i ++)
				{

					if ((sub_xlayer_id[i] == obu_xlayer_id))
					{
						streamID = i;
						break;
					}
				}
			}
			else 
			{
				streamID = obu_xlayer_id;
			}
			load_context(streamID); 
        }

    /*
long_term_id_in_use ( longTermId ) {
/* AV2 specification v1.0.0, 5.18.2 Frame header info syntax *//*
for ( j = 0 ; j < num_key_ref_frames ; j ++ ) {
if ( longTermId == ref_long_term_id [ j ] ) {
return 1
}
}
return 0
}
    */
		private int longTermId;
		public int __LongTermId { get { return longTermId; } set { longTermId = value; } }

        private int LongTermIdInUse(int longTermId)
        {
			int j = 0;
/*  AV2 specification v1.0.0, 5.18.2 Frame header info syntax  */


			for (j = 0; (j < num_key_ref_frames); j ++)
			{

				if ((longTermId == ref_long_term_id[j]))
				{
					return 1;
				}
			}
			return 0;
        }

    /*
reuse_tile_params ( uniformSpacing , sbRowStarts , tileRows , tileRowsLog2 , sbColStarts , tileCols , tileColsLog2 , seqSbSize , sbSize ) {
/* AV2 specification v1.0.0, 5.18.7.4 Reuse tile paramsc function *//*
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
    */
		private int uniformSpacing;
		public int _UniformSpacing { get { return uniformSpacing; } set { uniformSpacing = value; } }
		private AomArray<int> sbRowStarts = new AomArray<int>();
		public AomArray<int> _SbRowStarts { get { return sbRowStarts; } set { sbRowStarts = value; } }
		private int tileRows;
		public int __TileRows { get { return tileRows; } set { tileRows = value; } }
		private int tileRowsLog2;
		public int __TileRowsLog2 { get { return tileRowsLog2; } set { tileRowsLog2 = value; } }
		private AomArray<int> sbColStarts = new AomArray<int>();
		public AomArray<int> _SbColStarts { get { return sbColStarts; } set { sbColStarts = value; } }
		private int tileCols;
		public int __TileCols { get { return tileCols; } set { tileCols = value; } }
		private int tileColsLog2;
		public int __TileColsLog2 { get { return tileColsLog2; } set { tileColsLog2 = value; } }
		private int seqSbSize;
		public int _SeqSbSize { get { return seqSbSize; } set { seqSbSize = value; } }

        private (AomArray<int>, int, int, AomArray<int>, int, int, int) ReuseTileParams(int uniformSpacing, AomArray<int> sbRowStarts, int tileRows, int tileRowsLog2, AomArray<int> sbColStarts, int tileCols, int tileColsLog2, int seqSbSize, int sbSize)
        {
			int sbShift = 0;
			AomArray<int> unifSbColStarts = new AomArray<int>();
			AomArray<int> unifSbRowStarts = new AomArray<int>();
/*  AV2 specification v1.0.0, 5.18.7.4 Reuse tile paramsc function  */


			if ((uniformSpacing != 0))
			{
				sbShift = Mi_Width_Log2[seqSbSize];
				(unifSbColStarts, tileCols) = UniformSpacing(tileColsLog2, MiCols, seqSbSize);
				(unifSbRowStarts, tileRows) = UniformSpacing(tileRowsLog2, MiRows, seqSbSize);
				tileColsLog2 = TileLog2(1, tileCols);
				tileRowsLog2 = TileLog2(1, tileRows);
				return (unifSbRowStarts, tileRows, tileRowsLog2, unifSbColStarts, tileCols, tileColsLog2, sbShift);
			}
			else 
			{
				sbShift = Mi_Width_Log2[sbSize];
				tileColsLog2 = TileLog2(1, tileCols);
				tileRowsLog2 = TileLog2(1, tileRows);
				return (sbRowStarts, tileRows, tileRowsLog2, sbColStarts, tileCols, tileColsLog2, sbShift);
			}
        }

    /*
save_xlayer_context ( obu_xlayer_id ) {
/* AV2 specification v1.0.0, 7.6 Extended layer context management *//*
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
    */

        private void SaveXlayerContext(int obu_xlayer_id)
        {
			int i = 0;
			int streamID = 0;
/*  AV2 specification v1.0.0, 7.6 Extended layer context management  */


			if ((obu_xlayer_id == GLOBAL_XLAYER_ID))
			{
				return;
			}

			if ((MultiStreamDecoderMode != 0))
			{

				for (i = 0; (i < (num_streams_minus_2 + 2)); i ++)
				{

					if ((sub_xlayer_id[i] == obu_xlayer_id))
					{
						streamID = i;
						break;
					}
				}
			}
			else 
			{
				streamID = obu_xlayer_id;
			}
			save_context(streamID); 
        }

    /*
scale_warp_model ( baseParams , baseDistance , dist ) {
/* AV2 specification v1.0.0, 5.18.9.1 Global motion paramsc syntax *//*
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
paramsc [ i ] = center + output
}
return paramsc
}
    */
		private AomArray<int> baseParams = new AomArray<int>();
		public AomArray<int> _BaseParams { get { return baseParams; } set { baseParams = value; } }
		private int baseDistance;
		public int _BaseDistance { get { return baseDistance; } set { baseDistance = value; } }
		private int dist;
		public int _Dist { get { return dist; } set { dist = value; } }

        private AomArray<int> ScaleWarpModel(AomArray<int> baseParams, int baseDistance, int dist)
        {
			int i = 0;
			int center = 0;
			int limit = 0;
			int input = 0;
			int divShift = 0;
			int divFactor = 0;
			int scaled = 0;
			int output = 0;
			AomArray<int> paramsc = new AomArray<int>();
/*  AV2 specification v1.0.0, 5.18.9.1 Global motion paramsc syntax  */


			if ((baseDistance == 0))
			{
				return Default_Warp_Params;
			}

			if ((baseDistance < 0))
			{
				baseDistance = -baseDistance;
				dist = -dist;
			}

			for (i = 0; (i < 6); i ++)
			{
				center = Default_Warp_Params[i];
				limit = ((1 << 22) - 1);
				input = Clip3(-limit, limit, (baseParams[i] - center));
				(divShift, divFactor) = resolve_divisor(baseDistance);
				scaled = Round2Signed((input * divFactor), divShift);
				output = Round2Signed((scaled * dist), Param_Shift[i]);
				output = (Clip3(Param_Min[i], Param_Max[i], output) << Param_Shift[i]);
				paramsc[i] = (center + output);
			}
			return paramsc;
        }

    /*
set_chroma_format_and_bit_depth ( ) {
/* AV2 specification v1.0.0, 6.4.1 General sequence header OBU semantics *//*
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
    */
		private int SubsamplingX;
		public int _SubsamplingX { get { return SubsamplingX; } set { SubsamplingX = value; } }
		private int SubsamplingY;
		public int _SubsamplingY { get { return SubsamplingY; } set { SubsamplingY = value; } }
		private int BitDepth;
		public int _BitDepth { get { return BitDepth; } set { BitDepth = value; } }
		private int MaxQ;
		public int _MaxQ { get { return MaxQ; } set { MaxQ = value; } }
		private int Monochrome;
		public int __Monochrome { get { return Monochrome; } set { Monochrome = value; } }
		private int NumPlanes;
		public int _NumPlanes { get { return NumPlanes; } set { NumPlanes = value; } }

        private void SetChromaFormatAndBitDepth()
        {
/*  AV2 specification v1.0.0, 6.4.1 General sequence header OBU semantics  */


			if ((chroma_format_idc == CHROMA_FORMAT_420))
			{
				SubsamplingX = 1;
				SubsamplingY = 1;
			}
			else if ((chroma_format_idc == CHROMA_FORMAT_444))
			{
				SubsamplingX = 0;
				SubsamplingY = 0;
			}
			else if ((chroma_format_idc == CHROMA_FORMAT_422))
			{
				SubsamplingX = 1;
				SubsamplingY = 0;
			}
			else if ((chroma_format_idc == CHROMA_FORMAT_400))
			{
				SubsamplingX = 1;
				SubsamplingY = 1;
			}
			BitDepth = lookup_bitdepth(bit_depth_idc);
			MaxQ = lookup_maxq(bit_depth_idc);
			Monochrome = ((chroma_format_idc == CHROMA_FORMAT_400) ? 1 : 0);
			NumPlanes = ((Monochrome != 0) ? 1 : 3);
        }

    /*
set_primary_ref_frame_and_ctx ( loadCdfs ) {
/* AV2 specification v1.0.0, 5.18.2 Frame header info syntax *//*
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
    */
		private int loadCdfs;
		public int _LoadCdfs { get { return loadCdfs; } set { loadCdfs = value; } }
		private int DerivedPrimaryRefFrame;
		public int _DerivedPrimaryRefFrame { get { return DerivedPrimaryRefFrame; } set { DerivedPrimaryRefFrame = value; } }

        private void SetPrimaryRefFrameAndCtx(int loadCdfs)
        {
			int derivedSecondaryRefFrame = 0;
			int blendFrame = 0;
/*  AV2 specification v1.0.0, 5.18.2 Frame header info syntax  */

			(DerivedPrimaryRefFrame, derivedSecondaryRefFrame) = ChoosePrimarySecondaryRefFrame();

			if ((primary_ref_frame == PRIMARY_REF_CHOOSE))
			{
				primary_ref_frame = DerivedPrimaryRefFrame;
			}

			if (((DerivedPrimaryRefFrame == PRIMARY_REF_NONE) || (primary_ref_frame == PRIMARY_REF_NONE)))
			{
				primary_ref_frame = PRIMARY_REF_NONE;
				DerivedPrimaryRefFrame = PRIMARY_REF_NONE;
				disable_cross_frame_cdf_init = 1;
			}

			if (!(loadCdfs != 0))
			{
				return;
			}

			if (((primary_ref_frame == PRIMARY_REF_NONE) || (disable_cross_frame_cdf_init != 0)))
			{
				init_non_coeff_cdfs(); 
			}
			else 
			{
				load_cdfs(ref_frame_idx[primary_ref_frame]); 

				if ((TipFrameMode != TIP_FRAME_AS_OUTPUT))
				{
					blendFrame = ((primary_ref_frame == DerivedPrimaryRefFrame) ? derivedSecondaryRefFrame : DerivedPrimaryRefFrame);

					if (((((enable_avg_cdf != 0) && !(avg_cdf_type != 0)) && (blendFrame != PRIMARY_REF_NONE)) && !(bru_inactive != 0)))
					{
						blend_cdfs(ref_frame_idx[blendFrame]); 
					}
				}
			}

			if ((DerivedPrimaryRefFrame == PRIMARY_REF_NONE))
			{
				setup_past_independence(); 
			}
			else 
			{
				load_previous(); 
			}
        }

    /*
tile_log2 ( blkSize , target ) {
/* AV2 specification v1.0.0, 5.18.7.7 Tile size calculation function *//*
for ( k = 0 ; ( blkSize << k ) < target ; k ++ ) {
}
return k
}
    */
		private int blkSize;
		public int _BlkSize { get { return blkSize; } set { blkSize = value; } }
		private int target;
		public int _Target { get { return target; } set { target = value; } }

        private int TileLog2(int blkSize, int target)
        {
			int k = 0;
/*  AV2 specification v1.0.0, 5.18.7.7 Tile size calculation function  */


			for (k = 0; ((blkSize << k) < target); k ++)
			{
			}
			return k;
        }

    /*
uniform_eligible ( tileLog2 , sbNum ) {
/* AV2 specification v1.0.0, 5.18.7.2 Tile info syntax *//*
tileNum = 1 << tileLog2
tileWidth = ( sbNum + tileNum - 1 ) >> tileLog2
lastTileWidth = sbNum - ( tileNum - 1 ) * tileWidth
return tileWidth >= 1 && lastTileWidth >= 1
}
    */
		private int tileLog2;
		public int _TileLog2 { get { return tileLog2; } set { tileLog2 = value; } }
		private int sbNum;
		public int _SbNum { get { return sbNum; } set { sbNum = value; } }

        private int UniformEligible(int tileLog2, int sbNum)
        {
			int tileNum = 0;
			int tileWidth = 0;
			int lastTileWidth = 0;
/*  AV2 specification v1.0.0, 5.18.7.2 Tile info syntax  */

			tileNum = (1 << tileLog2);
			tileWidth = (((sbNum + tileNum) - 1) >> tileLog2);
			lastTileWidth = (sbNum - ((tileNum - 1) * tileWidth));
			return (((tileWidth >= 1) && (lastTileWidth >= 1)) ? 1 : 0);
        }

    /*
uniform_spacing ( tileLog2 , mis , sbSize ) {
/* AV2 specification v1.0.0, 5.18.7.5 Uniform spacing function *//*
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
    */
		private int mis;
		public int _Mis { get { return mis; } set { mis = value; } }

        private (AomArray<int>, int) UniformSpacing(int tileLog2, int mis, int sbSize)
        {
			int i = 0;
			int sb4x4 = 0;
			int sbShift = 0;
			int sbs = 0;
			int fullSbs = 0;
			int tileSb = 0;
			int extraSbs = 0;
			int startSb = 0;
			AomArray<int> sbStarts = new AomArray<int>();
/*  AV2 specification v1.0.0, 5.18.7.5 Uniform spacing function  */

			sb4x4 = Num_4x4_Blocks_Wide[sbSize];
			sbShift = Mi_Width_Log2[sbSize];
			sbs = (((mis + sb4x4) - 1) >> sbShift);
			fullSbs = (mis >> sbShift);
			tileSb = (fullSbs >> tileLog2);

			if ((tileSb == 0))
			{
				extraSbs = sbs;
			}
			else 
			{
				extraSbs = (fullSbs - (tileSb << tileLog2));
			}
			startSb = 0;

			for (i = 0; ((i < (1 << tileLog2)) && (startSb < sbs)); i ++)
			{
				sbStarts[i] = startSb;
				startSb += tileSb;

				if ((i < extraSbs))
				{
					startSb += 1;
				}
			}
			return (sbStarts, i);
        }

    /*
get_tx_row_col ( pos , txSz ) {
/* AV2 specification v1.0.0, 5.20.7.27 Coefficients syntax *//*
adjTxSz = Adjusted_Tx_Size [ txSz ]
bwl = Tx_Width_Log2 [ adjTxSz ]
row = pos >> bwl
col = pos - ( row << bwl )
return ( row , col )
}
    */
		private int pos;
		public int _Pos { get { return pos; } set { pos = value; } }
		private int txSz;
		public int _TxSz { get { return txSz; } set { txSz = value; } }

        private (int, int) GetTxRowCol(int pos, int txSz)
        {
			int adjTxSz = 0;
			int bwl = 0;
			int row = 0;
			int col = 0;
/*  AV2 specification v1.0.0, 5.20.7.27 Coefficients syntax  */

			adjTxSz = Adjusted_Tx_Size[txSz];
			bwl = Tx_Width_Log2[adjTxSz];
			row = (pos >> bwl);
			col = (pos - (row << bwl));
			return (row, col);
        }

    /*
get_scan ( txSz , txClass ) {
/* AV2 specification v1.0.0, 5.20.7.30 Get scan function *//*
w = Min ( Tx_Width [ txSz ], 32 )
h = Min ( Tx_Height [ txSz ], 32 )
if ( txClass == TX_CLASS_VERT ) {
c = 0
for ( y = 0 ; y < h ; y ++ ) {
for ( x = 0 ; x < w ; x ++ ) {
outc [ c ] = y * w + x
c += 1
}
}
} else if ( txClass == TX_CLASS_HORIZ ) {
c = 0
for ( x = 0 ; x < w ; x ++ ) {
for ( y = 0 ; y < h ; y ++ ) {
outc [ c ] = y * w + x
c += 1
}
}
} else {
x = 0
y = 0
for ( c = 0 ; c < w * h ; c ++ ) {
outc [ c ] = y * w + x
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
return outc
}
    */
		private int txClass;
		public int _TxClass { get { return txClass; } set { txClass = value; } }
		private int y = 0;
		private int x = 0;

        private AomArray<int> GetScan(int txSz, int txClass)
        {
			int y = 0;
			int x = 0;
			int c = 0;
			int w = 0;
			int h = 0;
			AomArray<int> outc = new AomArray<int>();
			int s = 0;
/*  AV2 specification v1.0.0, 5.20.7.30 Get scan function  */

			w = Min(Tx_Width[txSz], 32);
			h = Min(Tx_Height[txSz], 32);

			if ((txClass == TX_CLASS_VERT))
			{
				c = 0;

				for (y = 0; (y < h); y ++)
				{

					for (x = 0; (x < w); x ++)
					{
						outc[c] = ((y * w) + x);
						c += 1;
					}
				}
			}
			else if ((txClass == TX_CLASS_HORIZ))
			{
				c = 0;

				for (x = 0; (x < w); x ++)
				{

					for (y = 0; (y < h); y ++)
					{
						outc[c] = ((y * w) + x);
						c += 1;
					}
				}
			}
			else 
			{
				x = 0;
				y = 0;

				for (c = 0; (c < (w * h)); c ++)
				{
					outc[c] = ((y * w) + x);
					x += 1;
					y -= 1;

					if (((y < 0) || (x >= w)))
					{
						x += 1;
						s = Min(x, ((h - 1) - y));
						x -= s;
						y += s;
					}
				}
			}
			return outc;
        }

    /*
get_filter_set_index ( base_qindex ) {
/* AV2 specification v1.0.0, 5.18.7.11 Loop restoration paramsc syntax *//*
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
    */
		private int base_qindex;
		public int _BaseQindex { get { return base_qindex; } set { base_qindex = value; } }

        private int GetFilterSetIndex(int base_qindex)
        {
/*  AV2 specification v1.0.0, 5.18.7.11 Loop restoration paramsc syntax  */


			if ((base_qindex < 130))
			{
				return 0;
			}
			else if ((base_qindex < 190))
			{
				return 1;
			}
			else if ((base_qindex < 220))
			{
				return 2;
			}
			else 
			{
				return 3;
			}
        }

    /*
read_wienerns_filter( plane, unitRow, unitCol, readFrameFilters ) {
/* AV2 specification v1.0.0, 5.20.10.6 Read Wiener NS syntax *//*
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
refc = groupBase[ group ] + (n >> 1)
if ( n == 1 ) {
matchIndices[ c ] = groupBase[ group ]
} else {
matchIndices[ c ] = decode_signed_subexp_with_ref(
groupBase[ group ],
groupBase[ group ] + n, refc, 4)
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
    */
		private int unitRow;
		public int _UnitRow { get { return unitRow; } set { unitRow = value; } }
		private int unitCol;
		public int _UnitCol { get { return unitCol; } set { unitCol = value; } }
		private int readFrameFilters;
		public int _ReadFrameFilters { get { return readFrameFilters; } set { readFrameFilters = value; } }
		private int use_alt_group;
		public int _UseAltGroup { get { return use_alt_group; } set { use_alt_group = value; } }
		private int group_bit;
		public int _GroupBit { get { return group_bit; } set { group_bit = value; } }
		private int merged_param;
		public int _MergedParam { get { return merged_param; } set { merged_param = value; } }
		private int use_bank;
		public int _UseBank { get { return use_bank; } set { use_bank = value; } }
		private AomArray<AomArray<int>> WienerNsBankSize = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _WienerNsBankSize { get { return WienerNsBankSize; } set { WienerNsBankSize = value; } }
		private AomArray<AomArray<int>> WienerNsPtr = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _WienerNsPtr { get { return WienerNsPtr; } set { WienerNsPtr = value; } }
		private int wiener_ns_length;
		public int _WienerNsLength { get { return wiener_ns_length; } set { wiener_ns_length = value; } }
		private int wiener_ns_uv_sym;
		public int _WienerNsUvSym { get { return wiener_ns_uv_sym; } set { wiener_ns_uv_sym = value; } }
		private AomArray<AomArray<AomArray<AomArray<int>>>> LrWienerNs = new AomArray<AomArray<AomArray<AomArray<int>>>>(() => new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>())));
		public AomArray<AomArray<AomArray<AomArray<int>>>> _LrWienerNs { get { return LrWienerNs; } set { LrWienerNs = value; } }
		private AomArray<AomArray<AomArray<AomArray<int>>>> RefLrWienerNs = new AomArray<AomArray<AomArray<AomArray<int>>>>(() => new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>())));
		public AomArray<AomArray<AomArray<AomArray<int>>>> _RefLrWienerNs { get { return RefLrWienerNs; } set { RefLrWienerNs = value; } }
		private int subset = 0;

        private void ReadWienernsFilter(int plane, int unitRow, int unitCol, int readFrameFilters)
        {
			int i = 0;
			int c = 0;
			int k = 0;
			int subset = 0;
			int j = 0;
			int numClasses = 0;
			int numRefFilters = 0;
			int nopcw = 0;
			AomArray<int> groupCounts = new AomArray<int>();
			AomArray<int> groupHits = new AomArray<int>();
			AomArray<int> groupBase = new AomArray<int>();
			int predGroup = 0;
			int numZeros = 0;
			int altGroup = 0;
			int group = 0;
			int n = 0;
			int refc = 0;
			AomArray<int> matchIndices = new AomArray<int>();
			AomArray<int> merged = new AomArray<int>();
			AomArray<int> refBank = new AomArray<int>();
			int nCoeffs = 0;
			int numSubsets = 0;
			int min = 0;
			int v = 0;
/*  AV2 specification v1.0.0, 5.20.10.6 Read Wiener NS syntax  */

			numClasses = 1;

			if ((frame_filters_on[plane] != 0))
			{

				if (!(readFrameFilters != 0))
				{
					return;
				}
				(numClasses, numRefFilters, _, _, _) = SearchFrameFilters(plane, -1);
				nopcw = lr_tools_disable[0][RESTORE_PC_WIENER];
				groupCounts[0] = numClasses;
				groupCounts[1] = numRefFilters;
				groupCounts[2] = (((plane > 0) || (nopcw != 0)) ? 0 : ((64 - numClasses) - numRefFilters));

				for (i = 0; (i < 3); i++)
				{
					groupHits[i] = 0;
				}
				groupBase[0] = 0;

				for (i = 1; (i < 3); i++)
				{
					groupBase[i] = (groupBase[(i - 1)] + groupCounts[(i - 1)]);
				}

				for (c = 0; (c < numClasses); c++)
				{
					groupCounts[0] = (c + 1);

					if ((c == 0))
					{
						predGroup = ((groupCounts[1] > 2) ? 1 : PredictGroup(groupCounts));
					}
					else 
					{
						predGroup = PredictGroup(groupHits);
					}
					numZeros = 0;
					altGroup = 0;

					for (i = 0; (i < 3); i++)
					{

						if ((i != predGroup))
						{

							if ((groupCounts[i] == 0))
							{
								numZeros += 1;
							}
							else 
							{
								altGroup = i;
							}
						}
					}

					if ((numZeros == 2))
					{
						use_alt_group = 0;
					}
					else 
					{
						stream.ReadFixed(1, out this.use_alt_group, "use_alt_group"); 
					}

					if ((use_alt_group != 0))
					{

						if ((numZeros == 1))
						{
							group = altGroup;
						}
						else 
						{
							stream.ReadFixed(1, out this.group_bit, "group_bit"); 
							group = ((predGroup <= group_bit) ? (group_bit + 1) : group_bit);
						}
					}
					else 
					{
						group = predGroup;
					}
					n = groupCounts[group];
					refc = (groupBase[group] + (n >> 1));

					if ((n == 1))
					{
						matchIndices[c] = groupBase[group];
					}
					else 
					{
						matchIndices[c] = DecodeSignedSubexpWithRef(groupBase[group], (groupBase[group] + n), refc, 4);
					}
					groupHits[group]++;
				}
			}

			for (c = 0; (c < numClasses); c++)
			{

				if ((readFrameFilters != 0))
				{
					stream.ReadFixed(1, out this.merged_param, "merged_param"); 
				}
				else 
				{
					ReadArithmetic( out this.merged_param, "merged_param"); 
				}
				merged[c] = merged_param;

				if ((readFrameFilters != 0))
				{
					refBank[c] = 0;
				}
				else 
				{

					for (k = 0; (k < (WienerNsBankSize[plane][c] - 1)); k++)
					{
						ReadArithmetic( out this.use_bank, "use_bank"); 

						if ((use_bank != 0))
						{
							break;
						}
					}
					refBank[c] = (((WienerNsPtr[plane][c] - k) + LR_BANK_SIZE) % LR_BANK_SIZE);
				}
			}

			for (c = 0; (c < numClasses); c++)
			{

				if ((frame_filters_on[plane] != 0))
				{
					FillFirstSlotOfBankWithFilterMatch(c, plane, matchIndices[c]); 
				}
				nCoeffs = ((plane > 0) ? WIENER_NS_CHROMA_COEFFS : WIENER_NS_LUMA_COEFFS);

				if ((merged[c] != 0))
				{

					if ((WienerNsBankSize[plane][c] == 0))
					{
						WienerNsBankSize[plane][c] = 1;
					}
				}
				else 
				{

					if ((WienerNsBankSize[plane][c] < LR_BANK_SIZE))
					{
						WienerNsPtr[plane][c] = WienerNsBankSize[plane][c];
						WienerNsBankSize[plane][c] += 1;
					}
					else 
					{
						WienerNsPtr[plane][c] = ((WienerNsPtr[plane][c] + 1) % LR_BANK_SIZE);
					}
					numSubsets = ((plane == 0) ? 4 : 3);

					for (subset = 0; (subset < (numSubsets - 1)); subset++)
					{

						if ((readFrameFilters != 0))
						{
							stream.ReadFixed(1, out this.wiener_ns_length, "wiener_ns_length"); 
						}
						else 
						{
							ReadArithmetic( out this.wiener_ns_length, "wiener_ns_length"); 
						}

						if ((wiener_ns_length == 0))
						{
							break;
						}
					}

					if (((plane > 0) && (subset > 0)))
					{

						if ((readFrameFilters != 0))
						{
							stream.ReadFixed(1, out this.wiener_ns_uv_sym, "wiener_ns_uv_sym"); 
						}
						else 
						{
							ReadArithmetic( out this.wiener_ns_uv_sym, "wiener_ns_uv_sym"); 
						}
					}
					else 
					{
						wiener_ns_uv_sym = 0;
					}
				}

				for (j = 0; (j < nCoeffs); j++)
				{
					min = Wiener_Ns_Taps_Min[((plane != 0) ? 1 : 0)][j];
					k = Wiener_Ns_Taps_K[((plane != 0) ? 1 : 0)][j];
					v = RefLrWienerNs[plane][c][refBank[c]][j];

					if (!(merged[c] != 0))
					{

						if ((Wiener_Ns_Taps_Present[((plane != 0) ? 1 : 0)][subset][j] != 0))
						{

							if ((readFrameFilters != 0))
							{
								v = DecodeSignedSubexpWithRef(min, (min + (1 << k)), v, (k - 3));
							}
							else 
							{
								v = DecodeSigned4part(min, k, v);
							}
						}
						else 
						{
							v = 0;
						}
					}

					if ((readFrameFilters != 0))
					{
						FrameLrWienerNs[plane][c][j] = v;

						if ((((!(merged[c] != 0) && (plane > 0)) && (j >= WIENER_NS_SHORT_COEFFS)) && (wiener_ns_uv_sym != 0)))
						{
							FrameLrWienerNs[plane][c][(j + 1)] = v;
							j++;
						}
					}
					else 
					{
						LrWienerNs[plane][unitRow][unitCol][j] = v;

						if (!(merged[c] != 0))
						{
							RefLrWienerNs[plane][c][WienerNsPtr[plane][c]][j] = v;
						}

						if ((((!(merged[c] != 0) && (plane > 0)) && (j >= WIENER_NS_SHORT_COEFFS)) && (wiener_ns_uv_sym != 0)))
						{
							LrWienerNs[plane][unitRow][unitCol][(j + 1)] = v;
							RefLrWienerNs[plane][c][WienerNsPtr[plane][c]][(j + 1)] = v;
							j++;
						}
					}
				}
			}
        }

        private void WriteReadWienernsFilter(int plane, int unitRow, int unitCol, int readFrameFilters)
        {
			int i = 0;
			int c = 0;
			int k = 0;
			int subset = 0;
			int j = 0;
			int numClasses = 0;
			int numRefFilters = 0;
			int nopcw = 0;
			AomArray<int> groupCounts = new AomArray<int>();
			AomArray<int> groupHits = new AomArray<int>();
			AomArray<int> groupBase = new AomArray<int>();
			int predGroup = 0;
			int numZeros = 0;
			int altGroup = 0;
			int group = 0;
			int n = 0;
			int refc = 0;
			AomArray<int> matchIndices = new AomArray<int>();
			AomArray<int> merged = new AomArray<int>();
			AomArray<int> refBank = new AomArray<int>();
			int nCoeffs = 0;
			int numSubsets = 0;
			int min = 0;
			int v = 0;
/*  AV2 specification v1.0.0, 5.20.10.6 Read Wiener NS syntax  */

			numClasses = 1;

			if ((frame_filters_on[plane] != 0))
			{

				if (!(readFrameFilters != 0))
				{
					return;
				}
				(numClasses, numRefFilters, _, _, _) = SearchFrameFilters(plane, -1);
				nopcw = lr_tools_disable[0][RESTORE_PC_WIENER];
				groupCounts[0] = numClasses;
				groupCounts[1] = numRefFilters;
				groupCounts[2] = (((plane > 0) || (nopcw != 0)) ? 0 : ((64 - numClasses) - numRefFilters));

				for (i = 0; (i < 3); i++)
				{
					groupHits[i] = 0;
				}
				groupBase[0] = 0;

				for (i = 1; (i < 3); i++)
				{
					groupBase[i] = (groupBase[(i - 1)] + groupCounts[(i - 1)]);
				}

				for (c = 0; (c < numClasses); c++)
				{
					groupCounts[0] = (c + 1);

					if ((c == 0))
					{
						predGroup = ((groupCounts[1] > 2) ? 1 : PredictGroup(groupCounts));
					}
					else 
					{
						predGroup = PredictGroup(groupHits);
					}
					numZeros = 0;
					altGroup = 0;

					for (i = 0; (i < 3); i++)
					{

						if ((i != predGroup))
						{

							if ((groupCounts[i] == 0))
							{
								numZeros += 1;
							}
							else 
							{
								altGroup = i;
							}
						}
					}

					if ((numZeros == 2))
					{
						use_alt_group = 0;
					}
					else 
					{
						this.use_alt_group = stream.Pick("use_alt_group", _original != null ? _original.use_alt_group : this.use_alt_group, _edited != null ? _edited.use_alt_group : _original != null ? _original.use_alt_group : this.use_alt_group);
						stream.WriteFixed(1, this.use_alt_group, "use_alt_group"); 
					}

					if ((use_alt_group != 0))
					{

						if ((numZeros == 1))
						{
							group = altGroup;
						}
						else 
						{
							this.group_bit = stream.Pick("group_bit", _original != null ? _original.group_bit : this.group_bit, _edited != null ? _edited.group_bit : _original != null ? _original.group_bit : this.group_bit);
							stream.WriteFixed(1, this.group_bit, "group_bit"); 
							group = ((predGroup <= group_bit) ? (group_bit + 1) : group_bit);
						}
					}
					else 
					{
						group = predGroup;
					}
					n = groupCounts[group];
					refc = (groupBase[group] + (n >> 1));

					if ((n == 1))
					{
						matchIndices[c] = groupBase[group];
					}
					else 
					{
						matchIndices[c] = WriteDecodeSignedSubexpWithRef(groupBase[group], (groupBase[group] + n), refc, 4);
					}
					groupHits[group]++;
				}
			}

			for (c = 0; (c < numClasses); c++)
			{

				if ((readFrameFilters != 0))
				{
					this.merged_param = stream.Pick("merged_param", _original != null ? _original.merged_param : this.merged_param, _edited != null ? _edited.merged_param : _original != null ? _original.merged_param : this.merged_param);
					stream.WriteFixed(1, this.merged_param, "merged_param"); 
				}
				else 
				{
					this.merged_param = stream.Pick("merged_param", _original != null ? _original.merged_param : this.merged_param, _edited != null ? _edited.merged_param : _original != null ? _original.merged_param : this.merged_param);
					WriteArithmetic( this.merged_param, "merged_param"); 
				}
				merged[c] = merged_param;

				if ((readFrameFilters != 0))
				{
					refBank[c] = 0;
				}
				else 
				{

					for (k = 0; (k < (WienerNsBankSize[plane][c] - 1)); k++)
					{
						this.use_bank = stream.Pick("use_bank", _original != null ? _original.use_bank : this.use_bank, _edited != null ? _edited.use_bank : _original != null ? _original.use_bank : this.use_bank);
						WriteArithmetic( this.use_bank, "use_bank"); 

						if ((use_bank != 0))
						{
							break;
						}
					}
					refBank[c] = (((WienerNsPtr[plane][c] - k) + LR_BANK_SIZE) % LR_BANK_SIZE);
				}
			}

			for (c = 0; (c < numClasses); c++)
			{

				if ((frame_filters_on[plane] != 0))
				{
					FillFirstSlotOfBankWithFilterMatch(c, plane, matchIndices[c]); 
				}
				nCoeffs = ((plane > 0) ? WIENER_NS_CHROMA_COEFFS : WIENER_NS_LUMA_COEFFS);

				if ((merged[c] != 0))
				{

					if ((WienerNsBankSize[plane][c] == 0))
					{
						WienerNsBankSize[plane][c] = 1;
					}
				}
				else 
				{

					if ((WienerNsBankSize[plane][c] < LR_BANK_SIZE))
					{
						WienerNsPtr[plane][c] = WienerNsBankSize[plane][c];
						WienerNsBankSize[plane][c] += 1;
					}
					else 
					{
						WienerNsPtr[plane][c] = ((WienerNsPtr[plane][c] + 1) % LR_BANK_SIZE);
					}
					numSubsets = ((plane == 0) ? 4 : 3);

					for (subset = 0; (subset < (numSubsets - 1)); subset++)
					{

						if ((readFrameFilters != 0))
						{
							this.wiener_ns_length = stream.Pick("wiener_ns_length", _original != null ? _original.wiener_ns_length : this.wiener_ns_length, _edited != null ? _edited.wiener_ns_length : _original != null ? _original.wiener_ns_length : this.wiener_ns_length);
							stream.WriteFixed(1, this.wiener_ns_length, "wiener_ns_length"); 
						}
						else 
						{
							this.wiener_ns_length = stream.Pick("wiener_ns_length", _original != null ? _original.wiener_ns_length : this.wiener_ns_length, _edited != null ? _edited.wiener_ns_length : _original != null ? _original.wiener_ns_length : this.wiener_ns_length);
							WriteArithmetic( this.wiener_ns_length, "wiener_ns_length"); 
						}

						if ((wiener_ns_length == 0))
						{
							break;
						}
					}

					if (((plane > 0) && (subset > 0)))
					{

						if ((readFrameFilters != 0))
						{
							this.wiener_ns_uv_sym = stream.Pick("wiener_ns_uv_sym", _original != null ? _original.wiener_ns_uv_sym : this.wiener_ns_uv_sym, _edited != null ? _edited.wiener_ns_uv_sym : _original != null ? _original.wiener_ns_uv_sym : this.wiener_ns_uv_sym);
							stream.WriteFixed(1, this.wiener_ns_uv_sym, "wiener_ns_uv_sym"); 
						}
						else 
						{
							this.wiener_ns_uv_sym = stream.Pick("wiener_ns_uv_sym", _original != null ? _original.wiener_ns_uv_sym : this.wiener_ns_uv_sym, _edited != null ? _edited.wiener_ns_uv_sym : _original != null ? _original.wiener_ns_uv_sym : this.wiener_ns_uv_sym);
							WriteArithmetic( this.wiener_ns_uv_sym, "wiener_ns_uv_sym"); 
						}
					}
					else 
					{
						wiener_ns_uv_sym = 0;
					}
				}

				for (j = 0; (j < nCoeffs); j++)
				{
					min = Wiener_Ns_Taps_Min[((plane != 0) ? 1 : 0)][j];
					k = Wiener_Ns_Taps_K[((plane != 0) ? 1 : 0)][j];
					v = RefLrWienerNs[plane][c][refBank[c]][j];

					if (!(merged[c] != 0))
					{

						if ((Wiener_Ns_Taps_Present[((plane != 0) ? 1 : 0)][subset][j] != 0))
						{

							if ((readFrameFilters != 0))
							{
								v = WriteDecodeSignedSubexpWithRef(min, (min + (1 << k)), v, (k - 3));
							}
							else 
							{
								v = DecodeSigned4part(min, k, v);
							}
						}
						else 
						{
							v = 0;
						}
					}

					if ((readFrameFilters != 0))
					{
						FrameLrWienerNs[plane][c][j] = v;

						if ((((!(merged[c] != 0) && (plane > 0)) && (j >= WIENER_NS_SHORT_COEFFS)) && (wiener_ns_uv_sym != 0)))
						{
							FrameLrWienerNs[plane][c][(j + 1)] = v;
							j++;
						}
					}
					else 
					{
						LrWienerNs[plane][unitRow][unitCol][j] = v;

						if (!(merged[c] != 0))
						{
							RefLrWienerNs[plane][c][WienerNsPtr[plane][c]][j] = v;
						}

						if ((((!(merged[c] != 0) && (plane > 0)) && (j >= WIENER_NS_SHORT_COEFFS)) && (wiener_ns_uv_sym != 0)))
						{
							LrWienerNs[plane][unitRow][unitCol][(j + 1)] = v;
							RefLrWienerNs[plane][c][WienerNsPtr[plane][c]][(j + 1)] = v;
							j++;
						}
					}
				}
			}
        }

    /*
reset_qm () {
/* AV2 specification v1.0.0, 5.18.2 Frame header info syntax *//*
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
    */

        private void ResetQm()
        {
			int level = 0;
			int needsReset = 0;
/*  AV2 specification v1.0.0, 5.18.2 Frame header info syntax  */


			for (level = 0; (level < 15); level ++)
			{

				if (((obu_type == OBU_SWITCH) || (obu_type == OBU_RAS_FRAME)))
				{
					needsReset = (((QmMLayerId[level] == -1) || (MLayerPresenceMap[QmMLayerId[level]][obu_mlayer_id] != 0)) ? 1 : 0);
				}
				else 
				{
					needsReset = 1;
				}

				if ((!(QmProtected[level] != 0) && (needsReset != 0)))
				{
					QmDataPresent[level] = 0;
					QmNumPlanes[level] = NumPlanes;
					QmMLayerId[level] = -1;
					QmTLayerId[level] = -1;
				}
			}
        }

    /*
CeilLog2 ( x ) {
/* AV2 specification v1.0.0, 4.8 Mathematical functions *//*
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
    */

        private int CeilLog2(int x)
        {
			int i = 0;
			int p = 0;
/*  AV2 specification v1.0.0, 4.8 Mathematical functions  */


			if ((x < 2))
			{
				return 0;
			}
			i = 1;
			p = 2;

			while ((p < x))
			{
				i++;
				p = (p << 1);
			}
			return i;
        }

    /*
choose_primary_secondary_ref_frame () {
/* AV2 specification v1.0.0, 5.18.2 Frame header info syntax *//*
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
    */

        private (int, int) ChoosePrimarySecondaryRefFrame()
        {
			int i = 0;
			int primary = 0;
			int primaryQpDiff = 0;
			int secondary = 0;
			int secondaryQpDiff = 0;
			int primaryD = 0;
			int secondaryD = 0;
			int primaryRatio = 0;
			int secondaryRatio = 0;
			int idx = 0;
			int q = 0;
			int d = 0;
			int dRatio = 0;
			int qpDiff = 0;
/*  AV2 specification v1.0.0, 5.18.2 Frame header info syntax  */


			if (((FrameIsIntra != 0) || (FrameType == SWITCH_FRAME)))
			{
				return (PRIMARY_REF_NONE, PRIMARY_REF_NONE);
			}
			primary = PRIMARY_REF_NONE;
			primaryQpDiff = 512;
			secondary = PRIMARY_REF_NONE;
			secondaryQpDiff = 512;
			primaryD = 0;
			secondaryD = 0;
			primaryRatio = 0;
			secondaryRatio = 0;

			for (i = 0; (i < NumTotalRefs); i ++)
			{
				idx = ref_frame_idx[i];

				if ((((RefFrameType[idx] == INTER_FRAME) && (FirstSlotWithRef(idx) != 0)) && (RefOrderHint[idx] != RESTRICTED_OH)))
				{
					q = RefBaseQIdx[idx];
					d = RefOrderHint[idx];
					dRatio = FloorLog2((RefFrameWidth[idx] * RefFrameHeight[idx]));
					qpDiff = Abs((q - base_q_idx));

					if (((qpDiff < primaryQpDiff) || ((qpDiff == primaryQpDiff) && (IsRefBetter(d, primaryD, dRatio, primaryRatio) != 0))))
					{
						secondary = primary;
						secondaryQpDiff = primaryQpDiff;
						secondaryD = primaryD;
						secondaryRatio = primaryRatio;
						primary = i;
						primaryQpDiff = qpDiff;
						primaryD = d;
						primaryRatio = dRatio;
					}
					else if (((qpDiff < secondaryQpDiff) || ((qpDiff == secondaryQpDiff) && (IsRefBetter(d, secondaryD, dRatio, secondaryRatio) != 0))))
					{
						secondary = i;
						secondaryQpDiff = qpDiff;
						secondaryD = d;
						secondaryRatio = dRatio;
					}
				}
			}

			if ((signal_primary_ref_frame != 0))
			{

				if ((primary_ref_frame == PRIMARY_REF_NONE))
				{
					primary = PRIMARY_REF_NONE;
					secondary = PRIMARY_REF_NONE;
				}
				else if ((primary_ref_frame != primary))
				{

					if (((secondary == PRIMARY_REF_NONE) || (secondary == primary_ref_frame)))
					{
						secondary = primary;
					}
					primary = primary_ref_frame;
				}
			}
			return (primary, secondary);
        }

    /*
decode_signed_4part ( low , k , r ) {
/* AV2 specification v1.0.0, 5.20.10.6 Read Wiener NS syntax *//*
rOffset = r - low
xOffset = decode_unsigned_4part ( k , rOffset )
x = xOffset + low
return x
}
    */

        private int DecodeSigned4part(int low, int k, int r)
        {
			int rOffset = 0;
			int xOffset = 0;
			int x = 0;
/*  AV2 specification v1.0.0, 5.20.10.6 Read Wiener NS syntax  */

			rOffset = (r - low);
			xOffset = DecodeUnsigned4part(k, rOffset);
			x = (xOffset + low);
			return x;
        }

    /*
fill_first_slot_of_bank_with_filter_match ( c , plane , m ) {
/* AV2 specification v1.0.0, 5.20.10.6 Read Wiener NS syntax *//*
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
    */
		private int m;

        private void FillFirstSlotOfBankWithFilterMatch(int c, int plane, int m)
        {
			int j = 0;
			int numClasses = 0;
			int numRefFilters = 0;
			int matchIdx = 0;
			int matchCls = 0;
			int matchPlane = 0;
			int v = 0;
			int oldCls = 0;
/*  AV2 specification v1.0.0, 5.20.10.6 Read Wiener NS syntax  */

			WienerNsPtr[plane][c] = 0;
			WienerNsBankSize[plane][c] = 1;
			(numClasses, numRefFilters, matchIdx, matchCls, matchPlane) = SearchFrameFilters(plane, m);

			for (j = 0; (j < ((plane > 0) ? WIENER_NS_CHROMA_COEFFS : WIENER_NS_LUMA_COEFFS)); j ++)
			{

				if ((m == 0))
				{
					v = 0;
				}
				else if ((m < numClasses))
				{
					oldCls = (m - 1);
					v = FrameLrWienerNs[plane][oldCls][j];
				}
				else if ((m < (numClasses + numRefFilters)))
				{
					v = RefFrameLrWienerNs[matchIdx][matchPlane][matchCls][j];
				}
				else 
				{
					v = GetTranslatedPcWiener(((m - NumFilterClasses) - numRefFilters), j);
				}
				RefLrWienerNs[plane][c][0][j] = v;
			}
        }

    /*
get_max_disp_order_hint ( onlyShowable ) {
/* AV2 specification v1.0.0, 5.18.2 Frame header info syntax *//*
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
    */
		private int onlyShowable;
		public int _OnlyShowable { get { return onlyShowable; } set { onlyShowable = value; } }

        private int GetMaxDispOrderHint(int onlyShowable)
        {
			int i = 0;
			int maxDisp = 0;
/*  AV2 specification v1.0.0, 5.18.2 Frame header info syntax  */

			maxDisp = 0;

			for (i = 0; (i < NumRefFrames); i ++)
			{

				if (((((RefValid[i] != 0) && (TLayerDependencyMap[obu_mlayer_id][obu_tlayer_id][RefTLayerId[i]] != 0)) && (MLayerDependencyMap[obu_mlayer_id][RefMLayerId[i]] != 0)) && ((!(onlyShowable != 0) || (RefImplicitOutputFrame[i] != 0)) || (RefImmediateOutputFrame[i] != 0))))
				{
					maxDisp = Max(maxDisp, RefOrderHint[i]);
				}
			}
			return maxDisp;
        }

    /*
predict_group ( counts ) {
/* AV2 specification v1.0.0, 5.20.10.6 Read Wiener NS syntax *//*
pred = 0
for ( i = 1 ; i <= 2 ; i ++ ) {
if ( counts [ i ] > counts [ pred ] ) {
pred = i
}
}
return pred
}
    */
		private AomArray<int> counts = new AomArray<int>();
		public AomArray<int> _Counts { get { return counts; } set { counts = value; } }

        private int PredictGroup(AomArray<int> counts)
        {
			int i = 0;
			int pred = 0;
/*  AV2 specification v1.0.0, 5.20.10.6 Read Wiener NS syntax  */

			pred = 0;

			for (i = 1; (i <= 2); i ++)
			{

				if ((counts[i] > counts[pred]))
				{
					pred = i;
				}
			}
			return pred;
        }

    /*
Round2 ( x , n ) {
/* AV2 specification v1.0.0, 4.8 Mathematical functions *//*
if ( n == 0 )
return x
return ( x + ( 1 << ( n - 1 )) ) >> n
}
    */

        private int Round2(int x, int n)
        {
/*  AV2 specification v1.0.0, 4.8 Mathematical functions  */


			if ((n == 0))
			{
				return x;
			}
			return ((x + (1 << (n - 1))) >> n);
        }

    /*
search_frame_filters ( plane , target ) {
/* AV2 specification v1.0.0, 5.20.10.6 Read Wiener NS syntax *//*
nopcw = lr_tools_disable [ 0 ][ RESTORE_PC_WIENER ]
minPcWiener = ( plane > 0 || nopcw ) ? 0 : 16
numClasses = ( plane == 0 ) ? NumFilterClasses : 1
maxRefFilters = ( nopcw ? 16 : 64 ) - numClasses - minPcWiener
numRefFilters = 0
numCheckPlanes = plane > 0 ? 2 : 1
matchIdx = 0
matchCls = 0
matchPlane = plane
for ( refc = 0 ; refc < NumTotalRefs ; refc ++ ) {
if ( FrameType != SWITCH_FRAME && OrderHints [ refc ] != RESTRICTED_OH ) {
idx = ref_frame_idx [ refc ]
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
    */
		private int check = 0;

        private (int, int, int, int, int) SearchFrameFilters(int plane, int target)
        {
			int refc = 0;
			int check = 0;
			int i = 0;
			int nopcw = 0;
			int minPcWiener = 0;
			int numClasses = 0;
			int maxRefFilters = 0;
			int numRefFilters = 0;
			int numCheckPlanes = 0;
			int matchIdx = 0;
			int matchCls = 0;
			int matchPlane = 0;
			int idx = 0;
			int checkPlane = 0;
			int numRefClasses = 0;
/*  AV2 specification v1.0.0, 5.20.10.6 Read Wiener NS syntax  */

			nopcw = lr_tools_disable[0][RESTORE_PC_WIENER];
			minPcWiener = (((plane > 0) || (nopcw != 0)) ? 0 : 16);
			numClasses = ((plane == 0) ? NumFilterClasses : 1);
			maxRefFilters = ((((nopcw != 0) ? 16 : 64) - numClasses) - minPcWiener);
			numRefFilters = 0;
			numCheckPlanes = ((plane > 0) ? 2 : 1);
			matchIdx = 0;
			matchCls = 0;
			matchPlane = plane;

			for (refc = 0; (refc < NumTotalRefs); refc ++)
			{

				if (((FrameType != SWITCH_FRAME) && (OrderHints[refc] != RESTRICTED_OH)))
				{
					idx = ref_frame_idx[refc];

					for (check = 0; (check < numCheckPlanes); check ++)
					{

						if ((check == 0))
						{
							checkPlane = plane;
						}
						else 
						{
							checkPlane = ((plane == 1) ? 2 : 1);
						}

						if ((RefFrameFiltersOn[idx][checkPlane] != 0))
						{
							numRefClasses = ((plane == 0) ? RefNumFilterClasses[idx] : 1);

							for (i = 0; (i < numRefClasses); i ++)
							{

								if ((numRefFilters < maxRefFilters))
								{

									if (((numRefFilters + numClasses) == target))
									{
										matchIdx = idx;
										matchCls = i;
										matchPlane = checkPlane;
									}
									numRefFilters += 1;
								}
							}
						}
					}
				}
			}
			return (numClasses, numRefFilters, matchIdx, matchCls, matchPlane);
        }

    /*
decode_unsigned_4part ( k , r ) {
/* AV2 specification v1.0.0, 5.20.10.6 Read Wiener NS syntax *//*
mx = 1 << k
v = decode_4part ( 6 - k )
if (( r << 1 ) <= mx ) {
offset = inverse_recenter ( r , v )
} else {
offset = mx - 1 - inverse_recenter ( mx - 1 - r , v )
}
return offset
}
    */

        private int DecodeUnsigned4part(int k, int r)
        {
			int mx = 0;
			int v = 0;
			int offset = 0;
/*  AV2 specification v1.0.0, 5.20.10.6 Read Wiener NS syntax  */

			mx = (1 << k);
			v = decode_4part((6 - k));

			if (((r << 1) <= mx))
			{
				offset = InverseRecenter(r, v);
			}
			else 
			{
				offset = ((mx - 1) - InverseRecenter(((mx - 1) - r), v));
			}
			return offset;
        }

    /*
first_slot_with_ref ( i ) {
/* AV2 specification v1.0.0, 7.7 Get refc frames process *//*
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
    */

        private int FirstSlotWithRef(int i)
        {
			int j = 0;
/*  AV2 specification v1.0.0, 7.7 Get refc frames process  */


			if (!(RefValid[i] != 0))
			{
				return 0;
			}

			for (j = 0; (j < i); j ++)
			{

				if (((RefValid[j] != 0) && (RefCounter[j] == RefCounter[i])))
				{
					return 0;
				}
			}
			return 1;
        }

    /*
FloorLog2 ( x ) {
/* AV2 specification v1.0.0, 4.8 Mathematical functions *//*
s = 0
while ( x != 0 ) {
x = x >> 1
s ++
}
return s - 1
}
    */

        private int FloorLog2(int x)
        {
			int s = 0;
/*  AV2 specification v1.0.0, 4.8 Mathematical functions  */

			s = 0;

			while ((x != 0))
			{
				x = (x >> 1);
				s++;
			}
			return (s - 1);
        }

    /*
get_translated_pc_wiener ( m , j ) {
/* AV2 specification v1.0.0, 5.20.10.6 Read Wiener NS syntax *//*
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
    */

        private int GetTranslatedPcWiener(int m, int j)
        {
			int filt = 0;
			int coeff = 0;
			int min = 0;
			int max = 0;
/*  AV2 specification v1.0.0, 5.20.10.6 Read Wiener NS syntax  */


			if ((j >= 12))
			{
				return 0;
			}
			filt = Shuffled_Index[m];
			coeff = Round2Signed(Pc_Wiener_Filters[0][filt][j], (PC_WIENER_PREC_BITS - WIENER_NS_PREC_BITS));
			min = Wiener_Ns_Taps_Min[0][j];
			max = ((min + (1 << Wiener_Ns_Taps_K[0][j])) - 1);
			return Clip3(min, max, coeff);
        }

    /*
is_ref_better ( refDisp , bestDisp , refRatio , bestRatio ) {
/* AV2 specification v1.0.0, 5.18.2 Frame header info syntax *//*
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
    */
		private int refDisp;
		public int _RefDisp { get { return refDisp; } set { refDisp = value; } }
		private int bestDisp;
		public int _BestDisp { get { return bestDisp; } set { bestDisp = value; } }
		private int refRatio;
		public int _RefRatio { get { return refRatio; } set { refRatio = value; } }
		private int bestRatio;
		public int _BestRatio { get { return bestRatio; } set { bestRatio = value; } }

        private int IsRefBetter(int refDisp, int bestDisp, int refRatio, int bestRatio)
        {
			int d0 = 0;
			int d1 = 0;
/*  AV2 specification v1.0.0, 5.18.2 Frame header info syntax  */

			d0 = (Abs(GetRelativeDist(OrderHint, refDisp)) - (refRatio << 1));
			d1 = (Abs(GetRelativeDist(OrderHint, bestDisp)) - (bestRatio << 1));

			if ((d0 < d1))
			{
				return 1;
			}

			if (((d0 == d1) && (GetRelativeDist(refDisp, bestDisp) > 0)))
			{
				return 1;
			}
			return 0;
        }

    /*
bubble_sort_ref_scores ( ) {
/* AV2 specification v1.0.0, 7.7 Get refc frames process *//*
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
    */
		private AomArray<int> ScoresIndex = new AomArray<int>();
		public AomArray<int> _ScoresIndex { get { return ScoresIndex; } set { ScoresIndex = value; } }
		private AomArray<int> ScoresScore = new AomArray<int>();
		public AomArray<int> _ScoresScore { get { return ScoresScore; } set { ScoresScore = value; } }
		private AomArray<int> ScoresOrderHint = new AomArray<int>();
		public AomArray<int> _ScoresOrderHint { get { return ScoresOrderHint; } set { ScoresOrderHint = value; } }
		private AomArray<int> ScoresBaseQIdx = new AomArray<int>();
		public AomArray<int> _ScoresBaseQIdx { get { return ScoresBaseQIdx; } set { ScoresBaseQIdx = value; } }

        private void BubbleSortRefScores()
        {
			int i = 0;
			int j = 0;
			int index = 0;
			int score = 0;
			int displayOrder = 0;
			int distance = 0;
			int baseQIdx = 0;
/*  AV2 specification v1.0.0, 7.7 Get refc frames process  */


			for (i = (NRanked - 1); (i > 0); i --)
			{

				for (j = 0; (j < i); j ++)
				{

					if ((ScoresScore[j] > ScoresScore[(j + 1)]))
					{
						index = ScoresIndex[j];
						score = ScoresScore[j];
						displayOrder = ScoresOrderHint[j];
						distance = ScoresDistance[j];
						baseQIdx = ScoresBaseQIdx[j];
						ScoresIndex[j] = ScoresIndex[(j + 1)];
						ScoresScore[j] = ScoresScore[(j + 1)];
						ScoresOrderHint[j] = ScoresOrderHint[(j + 1)];
						ScoresDistance[j] = ScoresDistance[(j + 1)];
						ScoresBaseQIdx[j] = ScoresBaseQIdx[(j + 1)];
						ScoresIndex[(j + 1)] = index;
						ScoresScore[(j + 1)] = score;
						ScoresOrderHint[(j + 1)] = displayOrder;
						ScoresDistance[(j + 1)] = distance;
						ScoresBaseQIdx[(j + 1)] = baseQIdx;
					}
				}
			}
        }

    /*
get_unmapped_ref ( qThresh ) {
/* AV2 specification v1.0.0, 7.7 Get refc frames process *//*
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
    */
		private int qThresh;
		public int _QThresh { get { return qThresh; } set { qThresh = value; } }

        private int GetUnmappedRef(int qThresh)
        {
			int i = 0;
			int nPast = 0;
			int nFuture = 0;
			int maxPastDistance = 0;
			int maxFutureDistance = 0;
			int pastIdx = 0;
			int futureIdx = 0;
			int d = 0;
/*  AV2 specification v1.0.0, 7.7 Get refc frames process  */

			nPast = 0;
			nFuture = 0;
			maxPastDistance = 0;
			maxFutureDistance = 0;
			pastIdx = 0;
			futureIdx = 0;

			for (i = 0; (i < NRanked); i ++)
			{

				if ((ScoresBaseQIdx[i] >= qThresh))
				{
					d = ScoresDistance[i];

					if ((d > 0))
					{

						if ((d > maxPastDistance))
						{
							maxPastDistance = d;
							pastIdx = i;
						}
						nPast++;
					}
					else if ((d < 0))
					{

						if ((-d > maxFutureDistance))
						{
							maxFutureDistance = -d;
							futureIdx = i;
						}
						nFuture++;
					}
				}
			}

			if ((nPast > nFuture))
			{
				return pastIdx;
			}

			if ((nPast < nFuture))
			{
				return futureIdx;
			}

			if ((nPast > 0))
			{
				return ((maxPastDistance >= maxFutureDistance) ? pastIdx : futureIdx);
			}
			return -1;
        }

    /*
new_score_or_dist ( d , score , mLayer ) {
/* AV2 specification v1.0.0, 7.7 Get refc frames process *//*
for ( i = 0 ; i < NRanked ; i ++ ) {
if ( ScoresOrderHint [ i ] == d &&
ScoresScore [ i ] == score &&
mLayer == ScoresLayer [ i ] ) {
return 0
}
}
return 1
}
    */
		private int d;
		private int score;
		public int _Score { get { return score; } set { score = value; } }

        private int NewScoreOrDist(int d, int score, int mLayer)
        {
			int i = 0;
/*  AV2 specification v1.0.0, 7.7 Get refc frames process  */


			for (i = 0; (i < NRanked); i ++)
			{

				if ((((ScoresOrderHint[i] == d) && (ScoresScore[i] == score)) && (mLayer == ScoresLayer[i])))
				{
					return 0;
				}
			}
			return 1;
        }

    /*
valid_ref_frame_size ( checkRes , slot ) {
/* AV2 specification v1.0.0, 7.7 Get refc frames process *//*
if ( ! checkRes )
return 1
return ( 2 * FrameWidth >= RefFrameWidth [ slot ] &&
2 * FrameHeight >= RefFrameHeight [ slot ] &&
FrameWidth <= 16 * RefFrameWidth [ slot ] &&
FrameHeight <= 16 * RefFrameHeight [ slot ] )
}
    */
		private int checkRes;
		public int _CheckRes { get { return checkRes; } set { checkRes = value; } }
		private int slot;
		public int _Slot { get { return slot; } set { slot = value; } }

        private int ValidRefFrameSize(int checkRes, int slot)
        {
/*  AV2 specification v1.0.0, 7.7 Get refc frames process  */


			if (!(checkRes != 0))
			{
				return 1;
			}
			return ((((((2 * FrameWidth) >= RefFrameWidth[slot]) && ((2 * FrameHeight) >= RefFrameHeight[slot])) && (FrameWidth <= (16 * RefFrameWidth[slot]))) && (FrameHeight <= (16 * RefFrameHeight[slot]))) ? 1 : 0);
        }

    /*
seg_feature_active_idx ( idx , feature ) {
/* AV2 specification v1.0.0, 5.20.5.12 Segmentation feature active function *//*
return segmentation_enabled && FeatureEnabled [ idx ][ feature ]
}
    */
		private int feature;
		public int _Feature { get { return feature; } set { feature = value; } }

        private int SegFeatureActiveIdx(int idx, int feature)
        {
/*  AV2 specification v1.0.0, 5.20.5.12 Segmentation feature active function  */

			return (((segmentation_enabled != 0) && (FeatureEnabled[idx][feature] != 0)) ? 1 : 0);
        }

    /*
get_ref_frames( checkRes ) {
/* AV2 specification v1.0.0, 7.7 Get refc frames process: its code, inc order *//*
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
    */
		private int NRanked;
		public int _NRanked { get { return NRanked; } set { NRanked = value; } }
		private AomArray<int> ScoresLayer = new AomArray<int>();
		public AomArray<int> _ScoresLayer { get { return ScoresLayer; } set { ScoresLayer = value; } }

        private void GetRefFrames(int checkRes)
        {
			int i = 0;
			int maxDisp = 0;
			AomArray<int> mapOrderHint = new AomArray<int>();
			AomArray<int> mapBaseQIdx = new AomArray<int>();
			int maxQ = 0;
			int minQ = 0;
			int d = 0;
			int q = 0;
			int dispDiff = 0;
			int tDist = 0;
			int score = 0;
			int refRatio = 0;
			int qThresh = 0;
			int unmappedIdx = 0;
/*  AV2 specification v1.0.0, 7.7 Get refc frames process: its code, inc order  */

			maxDisp = 0;

			for (i = 0; (i < NumRefFrames); i ++)
			{
				mapOrderHint[i] = -1;

				if (((((((FirstSlotWithRef(i) != 0) && (RefOrderHint[i] != RESTRICTED_OH)) && (!(IsBridge != 0) || (i == bridge_frame_ref_idx))) && ((AllowedFrames & (1 << i)) != 0)) && (TLayerDependencyMap[obu_mlayer_id][obu_tlayer_id][RefTLayerId[i]] != 0)) && (MLayerDependencyMap[obu_mlayer_id][RefMLayerId[i]] != 0)))
				{

					if ((ValidRefFrameSize(checkRes, i) != 0))
					{
						mapOrderHint[i] = RefOrderHint[i];
					}
					mapBaseQIdx[i] = RefBaseQIdx[i];
					maxDisp = Max(maxDisp, RefOrderHint[i]);
				}
			}
			NRanked = 0;
			maxQ = 0;
			minQ = 0;

			for (i = 0; (i < NumRefFrames); i ++)
			{
				d = mapOrderHint[i];

				if ((d != -1))
				{
					q = mapBaseQIdx[i];
					dispDiff = GetRelativeDist(OrderHint, d);
					tDist = ((Abs(dispDiff) + obu_mlayer_id) - RefMLayerId[i]);

					if ((maxDisp > OrderHint))
					{
						score = ((tDist << DIST_WEIGHT_BITS) + q);
					}
					else 
					{
						score = ((Dist_Score_Lookup[Min(tDist, DECAY_DIST_CAP)] + Max((tDist - DECAY_DIST_CAP), 0)) + q);
					}
					refRatio = FloorLog2((RefFrameWidth[i] * RefFrameHeight[i]));
					score -= (refRatio << 5);

					if ((NewScoreOrDist(d, score, RefMLayerId[i]) != 0))
					{
						ScoresIndex[NRanked] = i;
						ScoresScore[NRanked] = score;
						ScoresOrderHint[NRanked] = d;
						ScoresDistance[NRanked] = dispDiff;
						ScoresBaseQIdx[NRanked] = q;
						ScoresLayer[NRanked] = RefMLayerId[i];

						if ((NRanked == 0))
						{
							minQ = q;
							maxQ = q;
						}
						else 
						{
							minQ = Min(q, minQ);
							maxQ = Max(q, maxQ);
						}
						NRanked += 1;
					}
				}
			}

			if ((NRanked > REFS_PER_FRAME))
			{
				qThresh = (((maxQ + minQ) + 1) / 2);
				unmappedIdx = GetUnmappedRef(qThresh);

				if ((unmappedIdx >= 0))
				{
					ScoresScore[unmappedIdx] = 0x7fffffff;
				}
			}
			BubbleSortRefScores(); 
			NumTotalRefs = Min(NRanked, ActiveNumRefFrames);

			for (i = 0; (i < NumTotalRefs); i ++)
			{
				ref_frame_idx[i] = ScoresIndex[i];
			}

			if (((checkRes != 0) && !(IsBridge != 0)))
			{

				for (i = 0; (i < NumRefFrames); i ++)
				{

					if (((((((RefValid[i] != 0) && (RefOrderHint[i] == RESTRICTED_OH)) && (TLayerDependencyMap[obu_mlayer_id][obu_tlayer_id][RefTLayerId[i]] != 0)) && (MLayerDependencyMap[obu_mlayer_id][RefMLayerId[i]] != 0)) && ((AllowedFrames & (1 << i)) != 0)) && (NumTotalRefs < ActiveNumRefFrames)))
					{
						ref_frame_idx[NumTotalRefs] = i;
						NumTotalRefs++;
					}
				}
			}
        }

    /*
get_past_future_cur_ref_lists( ) {
/* AV2 specification v1.0.0, 7.8 Get past future cur refc lists process: its code *//*
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
    */
		private int NumPastRefs;
		public int _NumPastRefs { get { return NumPastRefs; } set { NumPastRefs = value; } }
		private int NumFutureRefs;
		public int _NumFutureRefs { get { return NumFutureRefs; } set { NumFutureRefs = value; } }
		private int FurthestFuture;
		public int _FurthestFuture { get { return FurthestFuture; } set { FurthestFuture = value; } }
		private int ClosestPast;
		public int _ClosestPast { get { return ClosestPast; } set { ClosestPast = value; } }
		private int ClosestFuture;
		public int _ClosestFuture { get { return ClosestFuture; } set { ClosestFuture = value; } }
		private int SkipSegFrame;
		public int _SkipSegFrame { get { return SkipSegFrame; } set { SkipSegFrame = value; } }
		private int OrigClosestFuture;
		public int _OrigClosestFuture { get { return OrigClosestFuture; } set { OrigClosestFuture = value; } }
		private int OrigClosestPast;
		public int _OrigClosestPast { get { return OrigClosestPast; } set { OrigClosestPast = value; } }

        private void GetPastFutureCurRefLists()
        {
			int i = 0;
			int numCurRefs = 0;
			AomArray<int> curRefs = new AomArray<int>();
/*  AV2 specification v1.0.0, 7.8 Get past future cur refc lists process: its code  */

			NumPastRefs = 0;
			NumFutureRefs = 0;
			numCurRefs = 0;
			FurthestFuture = NONE;
			ClosestPast = NONE;
			ClosestFuture = NONE;

			for (i = 0; (i < NumTotalRefs); i ++)
			{

				if ((RefOrderHint[ref_frame_idx[i]] != RESTRICTED_OH))
				{

					if ((ScoresDistance[i] > 0))
					{
						NumPastRefs++;

						if (((ClosestPast == NONE) || (ScoresDistance[i] < ScoresDistance[ClosestPast])))
						{
							ClosestPast = i;
						}
					}
					else if ((ScoresDistance[i] < 0))
					{
						NumFutureRefs++;

						if (((FurthestFuture == NONE) || (RefOrderHint[ref_frame_idx[FurthestFuture]] < RefOrderHint[ref_frame_idx[i]])))
						{
							FurthestFuture = i;
						}

						if (((ClosestFuture == NONE) || (RefOrderHint[ref_frame_idx[i]] < RefOrderHint[ref_frame_idx[ClosestFuture]])))
						{
							ClosestFuture = i;
						}
					}
					else 
					{
						curRefs[numCurRefs] = i;
						numCurRefs++;
					}
				}
			}
			SkipSegFrame = ((numCurRefs > 0) ? curRefs[0] : ClosestPast);

			if ((SkipSegFrame == NONE))
			{
				SkipSegFrame = 0;
			}
			OrigClosestFuture = ClosestFuture;
			OrigClosestPast = ClosestPast;
        }

    /*
motion_field_estimation( ) {
/* AV2 specification v1.0.0, 7.9.1 Motion field estimation process: the code that chooses the references for TIP. *//*
/* The motion field it projects isc the tile data's, and isc left outc. *//*
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
/* "If MotionFieldStackCount isc less than 2, the process immediately terminates." *//*
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
    */
		private AomArray<int> MotionFieldVisited = new AomArray<int>();
		public AomArray<int> _MotionFieldVisited { get { return MotionFieldVisited; } set { MotionFieldVisited = value; } }
		private AomArray<int> MotionFieldDepth = new AomArray<int>();
		public AomArray<int> _MotionFieldDepth { get { return MotionFieldDepth; } set { MotionFieldDepth = value; } }
		private AomArray<AomArray<int>> MotionFieldChecked = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _MotionFieldChecked { get { return MotionFieldChecked; } set { MotionFieldChecked = value; } }
		private int MotionFieldStackCount;
		public int _MotionFieldStackCount { get { return MotionFieldStackCount; } set { MotionFieldStackCount = value; } }
		private int rf = 0;

        private void MotionFieldEstimation()
        {
			int i = 0;
			int j = 0;
			int rf = 0;
			AomArray<int> sortRef = new AomArray<int>();
			int tmp = 0;
			int curIdx = 0;
			int processCount = 0;
			int past = 0;
			int future = 0;
/*  AV2 specification v1.0.0, 7.9.1 Motion field estimation process: the code that chooses the references for TIP.  */

/*  The motion field it projects isc the tile data's, and isc left outc.  */


			for (i = 0; (i < NumTotalRefs); i ++)
			{
				sortRef[i] = i;
			}

			for (i = 0; (i < NumTotalRefs); i ++)
			{

				for (j = (i + 1); (j < NumTotalRefs); j ++)
				{

					if ((GetRelativeDist(OrderHints[sortRef[j]], OrderHints[sortRef[i]]) < 0))
					{
						tmp = sortRef[i];
						sortRef[i] = sortRef[j];
						sortRef[j] = tmp;
					}
				}
			}
			curIdx = -1;

			for (i = 0; (i < NumTotalRefs); i ++)
			{

				if ((GetRelativeDist(OrderHints[sortRef[i]], OrderHint) < 0))
				{
					curIdx = i;
				}
				else 
				{
					break;
				}
			}

			for (rf = 0; (rf < NumTotalRefs); rf ++)
			{
				MotionFieldVisited[rf] = 0;
				MotionFieldDepth[rf] = -1;
				MotionFieldChecked[rf][0] = 0;
				MotionFieldChecked[rf][1] = 0;
			}
			MotionFieldStackCount = 0;

			for (rf = 0; (rf < NumTotalRefs); rf ++)
			{

				if ((OrderHints[rf] != RESTRICTED_OH))
				{
					TopoSortRefs(rf); 
				}
			}
/*  "If MotionFieldStackCount isc less than 2, the process immediately terminates."  */


			if ((MotionFieldStackCount < 2))
			{
				return;
			}
			processCount = 0;

			if (((enable_tip != 0) && (((NumFutureRefs > 0) && (NumPastRefs > 0)) || (NumPastRefs >= 2))))
			{
				past = sortRef[curIdx];

				if (((NumFutureRefs > 0) && (NumPastRefs > 0)))
				{
					future = sortRef[(curIdx + 1)];
				}
				else 
				{
					future = sortRef[(curIdx - 1)];
				}

				if ((MotionFieldDepth[past] > MotionFieldDepth[future]))
				{
					processCount = record_tip_projection(past, 1, future, processCount);
				}
				else 
				{
					processCount = record_tip_projection(future, 0, past, processCount);
				}
				ClosestPast = past;
				ClosestFuture = future;
			}
			else 
			{
				ClosestPast = NONE;
				ClosestFuture = NONE;
			}
        }

    /*
topo_sort_refs ( rf ) {
/* AV2 specification v1.0.0, 7.9.1 *//*
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
    */
		private AomArray<int> MotionFieldStack = new AomArray<int>();
		public AomArray<int> _MotionFieldStack { get { return MotionFieldStack; } set { MotionFieldStack = value; } }

        private void TopoSortRefs(int rf)
        {
			int i = 0;
			int j = 0;
			int refIdx = 0;
/*  AV2 specification v1.0.0, 7.9.1  */


			if ((MotionFieldVisited[rf] != 0))
			{
				return;
			}
			MotionFieldVisited[rf] = 1;
			refIdx = ref_frame_idx[rf];

			if ((RefFrameType[refIdx] == INTER_FRAME))
			{

				for (i = 0; (i < RefNumTotalRefs[refIdx]); i ++)
				{

					if ((SavedOrderHints[refIdx][i] != RESTRICTED_OH))
					{

						for (j = 0; (j < NumTotalRefs); j ++)
						{

							if (((OrderHints[j] == SavedOrderHints[refIdx][i]) && !(IsRefOverlay(j) != 0)))
							{
								TopoSortRefs(j); 
								break;
							}
						}
					}
				}
			}
			MotionFieldDepth[rf] = MotionFieldStackCount;
			MotionFieldStack[MotionFieldStackCount] = rf;
			MotionFieldStackCount++;
        }

    /*
is_ref_overlay ( refc ) {
/* AV2 specification v1.0.0, 7.9.1 *//*
refIdx = ref_frame_idx [ refc ]
for ( i = 0 ; i < RefNumTotalRefs [ refIdx ]; i ++ ) {
if ( SavedOrderHints [ refIdx ][ i ] == RefOrderHint [ refIdx ]) {
return 1
}
}
return 0
}
    */

        private int IsRefOverlay(int refc)
        {
			int i = 0;
			int refIdx = 0;
/*  AV2 specification v1.0.0, 7.9.1  */

			refIdx = ref_frame_idx[refc];

			for (i = 0; (i < RefNumTotalRefs[refIdx]); i ++)
			{

				if ((SavedOrderHints[refIdx][i] == RefOrderHint[refIdx]))
				{
					return 1;
				}
			}
			return 0;
        }

		/// <summary>What sequence_header_obu and the structures it calls assign, as SaveSequenceHeaderObu keeps it.</summary>
		private sealed partial class SequenceHeaderObuState
		{
			public int ActiveNumRefFrames;
			public int BaseUVAcDeltaQ;
			public int BaseUVDcDeltaQ;
			public int BaseYDcDeltaQ;
			public int BitDepth;
			public int CdefOnSkipTxfm;
			public int DrlReorder;
			public int EnableTipOutput;
			public AomArray<AomArray<int>> MLayerDependencyMap;
			public AomArray<AomArray<int>> MLayerPresenceMap;
			public int MaxPbAspectRatio;
			public int MaxQ;
			public int MaxSegments;
			public int Monochrome;
			public int NumPlanes;
			public int NumRefFrames;
			public int OrderHintBits;
			public AomArray<AomArray<int>> SeqFeatureData;
			public AomArray<AomArray<int>> SeqFeatureEnabled;
			public int SeqMaxMlayerCnt;
			public AomArray<int> SeqSbColStarts;
			public int SeqSbCols;
			public AomArray<int> SeqSbRowStarts;
			public int SeqSbRows;
			public int SeqTileCols;
			public int SeqTileColsLog2;
			public int SeqTileRows;
			public int SeqTileRowsLog2;
			public int SeqUniformTileSpacingFlag;
			public int SubsamplingX;
			public int SubsamplingY;
			public AomArray<AomArray<AomArray<int>>> TLayerDependencyMap;
			public int allow_frame_max_bvp_drl_bits;
			public int allow_frame_max_drl_bits;
			public int allow_tile_info_change;
			public int avg_cdf_type;
			public int base_uv_ac_delta_q;
			public int base_uv_dc_delta_q;
			public int base_y_dc_delta_q;
			public int bit_depth_idc;
			public int ccso_unit_matches_sb_size;
			public int cdef_on_skip_txfm_always_on;
			public int cdef_on_skip_txfm_disabled;
			public int cfl_ds_filter_index;
			public int choose_tcq_per_frame;
			public int chroma_format_idc;
			public int constrain_drl_reorder;
			public int decoder_buffer_delay;
			public int decoder_model_info_present_flag;
			public int df_par_bits_minus_2;
			public int disable_drl_reorder;
			public int disable_loopfilters_across_tiles;
			public int disable_tip_output;
			public int enable_adaptive_mvd;
			public int enable_avg_cdf;
			public int enable_bawp;
			public int enable_bru;
			public int enable_ccso;
			public int enable_cctx;
			public int enable_cdef;
			public int enable_cfl_intra;
			public int enable_chroma_dctonly;
			public int enable_cwp;
			public int enable_df_sub_pu;
			public int enable_dip;
			public int enable_ext_partitions;
			public int enable_ext_seg;
			public int enable_extended_sdp;
			public int enable_flex_mvres;
			public int enable_fsc;
			public int enable_gdf;
			public int enable_global_motion;
			public int enable_ibp;
			public int enable_idtx_intra;
			public int enable_imp_msk_bld;
			public int enable_inter_ddt;
			public int enable_inter_ist;
			public int enable_intra_edge_filter;
			public int enable_intra_ist;
			public int enable_masked_compound;
			public int enable_mhccp;
			public int enable_mrls;
			public int enable_mv_traj;
			public int enable_mvd_sign_derive;
			public int enable_opfl_refine;
			public int enable_parity_hiding;
			public int enable_ref_frame_mvs;
			public int enable_refinemv;
			public int enable_refmvbank;
			public int enable_restoration;
			public int enable_sdp;
			public int enable_short_refresh_frame_flags;
			public int enable_six_param_warp_delta;
			public int enable_tcq;
			public int enable_tip;
			public int enable_tip_explicit_qp;
			public int enable_tip_hole_fill;
			public int enable_tip_refinemv;
			public int enable_uneven_4way_partitions;
			public int encoder_buffer_delay;
			public int equal_ac_dc_q;
			public int explicit_num_ref_frames;
			public int explicit_ref_frame_map;
			public int feature_enabled;
			public int feature_value;
			public int film_grain_params_present;
			public int frame_height_bits_minus_1;
			public int frame_width_bits_minus_1;
			public int gdf_unit_matches_sb_size;
			public int height_in_sbs_minus_1;
			public int increment_tile_cols_log2;
			public int increment_tile_rows_log2;
			public int long_term_frame_id_bits;
			public int low_delay_mode_flag;
			public AomArray<AomArray<int>> lr_tools_disable;
			public int lr_tools_uv_present;
			public int max_frame_height_minus_1;
			public int max_frame_width_minus_1;
			public int max_mlayer_id;
			public int max_pb_aspect_ratio_log2_minus_1;
			public int max_tlayer_id;
			public int mlayer_dependency_map;
			public int mlayer_dependency_present_flag;
			public int monotonic_output_order_flag;
			public int multi_tlayer_dependency_map_present_flag;
			public int n;
			public int num_ref_frames_minus_1;
			public int num_same_ref_compound;
			public int num_units_in_decoding_tick;
			public int order_hint_bits_minus_1;
			public int reduce_pb_aspect_ratio;
			public int reduced_ref_frame_mvs_mode;
			public int reduced_tx_part_set;
			public int separate_uv_delta_q;
			public int seq_allow_seg_info_change;
			public int seq_choose_integer_mv;
			public int seq_choose_screen_content_tools;
			public int seq_cropping_win_bottom_offset;
			public int seq_cropping_win_left_offset;
			public int seq_cropping_win_right_offset;
			public int seq_cropping_win_top_offset;
			public int seq_cropping_window_present_flag;
			public int seq_decoder_model_info_present_flag;
			public AomArray<int> seq_enabled_motion_modes;
			public int seq_force_integer_mv;
			public int seq_force_screen_content_tools;
			public int seq_frame_motion_modes_present_flag;
			public int seq_header_id;
			public int seq_initial_display_delay_minus_1;
			public int seq_initial_display_delay_present_flag;
			public int seq_lcr_id;
			public int seq_level_idx;
			public int seq_max_bvp_drl_bits_minus_1;
			public int seq_max_drl_bits_minus_1;
			public int seq_max_mlayer_cnt_minus_1;
			public int seq_profile_idc;
			public int seq_seg_info_present_flag;
			public int seq_tier;
			public int seq_tile_info_present_flag;
			public int single_picture_header_flag;
			public int still_picture;
			public int tlayer_dependency_map;
			public int tlayer_dependency_present_flag;
			public int uniform_tile_spacing_flag;
			public int use_128x128_superblock;
			public int use_256x256_superblock;
			public int uv_ac_delta_q_enabled;
			public int uv_dc_delta_q_enabled;
			public int width_in_sbs_minus_1;
			public int y_dc_delta_q_enabled;
		}

		private SequenceHeaderObuState SaveSequenceHeaderObu()
		{
			var state = new SequenceHeaderObuState();
			state.ActiveNumRefFrames = this.ActiveNumRefFrames;
			state.BaseUVAcDeltaQ = this.BaseUVAcDeltaQ;
			state.BaseUVDcDeltaQ = this.BaseUVDcDeltaQ;
			state.BaseYDcDeltaQ = this.BaseYDcDeltaQ;
			state.BitDepth = this.BitDepth;
			state.CdefOnSkipTxfm = this.CdefOnSkipTxfm;
			state.DrlReorder = this.DrlReorder;
			state.EnableTipOutput = this.EnableTipOutput;
			state.MLayerDependencyMap = this.MLayerDependencyMap?.Clone();
			state.MLayerPresenceMap = this.MLayerPresenceMap?.Clone();
			state.MaxPbAspectRatio = this.MaxPbAspectRatio;
			state.MaxQ = this.MaxQ;
			state.MaxSegments = this.MaxSegments;
			state.Monochrome = this.Monochrome;
			state.NumPlanes = this.NumPlanes;
			state.NumRefFrames = this.NumRefFrames;
			state.OrderHintBits = this.OrderHintBits;
			state.SeqFeatureData = this.SeqFeatureData?.Clone();
			state.SeqFeatureEnabled = this.SeqFeatureEnabled?.Clone();
			state.SeqMaxMlayerCnt = this.SeqMaxMlayerCnt;
			state.SeqSbColStarts = this.SeqSbColStarts?.Clone();
			state.SeqSbCols = this.SeqSbCols;
			state.SeqSbRowStarts = this.SeqSbRowStarts?.Clone();
			state.SeqSbRows = this.SeqSbRows;
			state.SeqTileCols = this.SeqTileCols;
			state.SeqTileColsLog2 = this.SeqTileColsLog2;
			state.SeqTileRows = this.SeqTileRows;
			state.SeqTileRowsLog2 = this.SeqTileRowsLog2;
			state.SeqUniformTileSpacingFlag = this.SeqUniformTileSpacingFlag;
			state.SubsamplingX = this.SubsamplingX;
			state.SubsamplingY = this.SubsamplingY;
			state.TLayerDependencyMap = this.TLayerDependencyMap?.Clone();
			state.allow_frame_max_bvp_drl_bits = this.allow_frame_max_bvp_drl_bits;
			state.allow_frame_max_drl_bits = this.allow_frame_max_drl_bits;
			state.allow_tile_info_change = this.allow_tile_info_change;
			state.avg_cdf_type = this.avg_cdf_type;
			state.base_uv_ac_delta_q = this.base_uv_ac_delta_q;
			state.base_uv_dc_delta_q = this.base_uv_dc_delta_q;
			state.base_y_dc_delta_q = this.base_y_dc_delta_q;
			state.bit_depth_idc = this.bit_depth_idc;
			state.ccso_unit_matches_sb_size = this.ccso_unit_matches_sb_size;
			state.cdef_on_skip_txfm_always_on = this.cdef_on_skip_txfm_always_on;
			state.cdef_on_skip_txfm_disabled = this.cdef_on_skip_txfm_disabled;
			state.cfl_ds_filter_index = this.cfl_ds_filter_index;
			state.choose_tcq_per_frame = this.choose_tcq_per_frame;
			state.chroma_format_idc = this.chroma_format_idc;
			state.constrain_drl_reorder = this.constrain_drl_reorder;
			state.decoder_buffer_delay = this.decoder_buffer_delay;
			state.decoder_model_info_present_flag = this.decoder_model_info_present_flag;
			state.df_par_bits_minus_2 = this.df_par_bits_minus_2;
			state.disable_drl_reorder = this.disable_drl_reorder;
			state.disable_loopfilters_across_tiles = this.disable_loopfilters_across_tiles;
			state.disable_tip_output = this.disable_tip_output;
			state.enable_adaptive_mvd = this.enable_adaptive_mvd;
			state.enable_avg_cdf = this.enable_avg_cdf;
			state.enable_bawp = this.enable_bawp;
			state.enable_bru = this.enable_bru;
			state.enable_ccso = this.enable_ccso;
			state.enable_cctx = this.enable_cctx;
			state.enable_cdef = this.enable_cdef;
			state.enable_cfl_intra = this.enable_cfl_intra;
			state.enable_chroma_dctonly = this.enable_chroma_dctonly;
			state.enable_cwp = this.enable_cwp;
			state.enable_df_sub_pu = this.enable_df_sub_pu;
			state.enable_dip = this.enable_dip;
			state.enable_ext_partitions = this.enable_ext_partitions;
			state.enable_ext_seg = this.enable_ext_seg;
			state.enable_extended_sdp = this.enable_extended_sdp;
			state.enable_flex_mvres = this.enable_flex_mvres;
			state.enable_fsc = this.enable_fsc;
			state.enable_gdf = this.enable_gdf;
			state.enable_global_motion = this.enable_global_motion;
			state.enable_ibp = this.enable_ibp;
			state.enable_idtx_intra = this.enable_idtx_intra;
			state.enable_imp_msk_bld = this.enable_imp_msk_bld;
			state.enable_inter_ddt = this.enable_inter_ddt;
			state.enable_inter_ist = this.enable_inter_ist;
			state.enable_intra_edge_filter = this.enable_intra_edge_filter;
			state.enable_intra_ist = this.enable_intra_ist;
			state.enable_masked_compound = this.enable_masked_compound;
			state.enable_mhccp = this.enable_mhccp;
			state.enable_mrls = this.enable_mrls;
			state.enable_mv_traj = this.enable_mv_traj;
			state.enable_mvd_sign_derive = this.enable_mvd_sign_derive;
			state.enable_opfl_refine = this.enable_opfl_refine;
			state.enable_parity_hiding = this.enable_parity_hiding;
			state.enable_ref_frame_mvs = this.enable_ref_frame_mvs;
			state.enable_refinemv = this.enable_refinemv;
			state.enable_refmvbank = this.enable_refmvbank;
			state.enable_restoration = this.enable_restoration;
			state.enable_sdp = this.enable_sdp;
			state.enable_short_refresh_frame_flags = this.enable_short_refresh_frame_flags;
			state.enable_six_param_warp_delta = this.enable_six_param_warp_delta;
			state.enable_tcq = this.enable_tcq;
			state.enable_tip = this.enable_tip;
			state.enable_tip_explicit_qp = this.enable_tip_explicit_qp;
			state.enable_tip_hole_fill = this.enable_tip_hole_fill;
			state.enable_tip_refinemv = this.enable_tip_refinemv;
			state.enable_uneven_4way_partitions = this.enable_uneven_4way_partitions;
			state.encoder_buffer_delay = this.encoder_buffer_delay;
			state.equal_ac_dc_q = this.equal_ac_dc_q;
			state.explicit_num_ref_frames = this.explicit_num_ref_frames;
			state.explicit_ref_frame_map = this.explicit_ref_frame_map;
			state.feature_enabled = this.feature_enabled;
			state.feature_value = this.feature_value;
			state.film_grain_params_present = this.film_grain_params_present;
			state.frame_height_bits_minus_1 = this.frame_height_bits_minus_1;
			state.frame_width_bits_minus_1 = this.frame_width_bits_minus_1;
			state.gdf_unit_matches_sb_size = this.gdf_unit_matches_sb_size;
			state.height_in_sbs_minus_1 = this.height_in_sbs_minus_1;
			state.increment_tile_cols_log2 = this.increment_tile_cols_log2;
			state.increment_tile_rows_log2 = this.increment_tile_rows_log2;
			state.long_term_frame_id_bits = this.long_term_frame_id_bits;
			state.low_delay_mode_flag = this.low_delay_mode_flag;
			state.lr_tools_disable = this.lr_tools_disable?.Clone();
			state.lr_tools_uv_present = this.lr_tools_uv_present;
			state.max_frame_height_minus_1 = this.max_frame_height_minus_1;
			state.max_frame_width_minus_1 = this.max_frame_width_minus_1;
			state.max_mlayer_id = this.max_mlayer_id;
			state.max_pb_aspect_ratio_log2_minus_1 = this.max_pb_aspect_ratio_log2_minus_1;
			state.max_tlayer_id = this.max_tlayer_id;
			state.mlayer_dependency_map = this.mlayer_dependency_map;
			state.mlayer_dependency_present_flag = this.mlayer_dependency_present_flag;
			state.monotonic_output_order_flag = this.monotonic_output_order_flag;
			state.multi_tlayer_dependency_map_present_flag = this.multi_tlayer_dependency_map_present_flag;
			state.n = this.n;
			state.num_ref_frames_minus_1 = this.num_ref_frames_minus_1;
			state.num_same_ref_compound = this.num_same_ref_compound;
			state.num_units_in_decoding_tick = this.num_units_in_decoding_tick;
			state.order_hint_bits_minus_1 = this.order_hint_bits_minus_1;
			state.reduce_pb_aspect_ratio = this.reduce_pb_aspect_ratio;
			state.reduced_ref_frame_mvs_mode = this.reduced_ref_frame_mvs_mode;
			state.reduced_tx_part_set = this.reduced_tx_part_set;
			state.separate_uv_delta_q = this.separate_uv_delta_q;
			state.seq_allow_seg_info_change = this.seq_allow_seg_info_change;
			state.seq_choose_integer_mv = this.seq_choose_integer_mv;
			state.seq_choose_screen_content_tools = this.seq_choose_screen_content_tools;
			state.seq_cropping_win_bottom_offset = this.seq_cropping_win_bottom_offset;
			state.seq_cropping_win_left_offset = this.seq_cropping_win_left_offset;
			state.seq_cropping_win_right_offset = this.seq_cropping_win_right_offset;
			state.seq_cropping_win_top_offset = this.seq_cropping_win_top_offset;
			state.seq_cropping_window_present_flag = this.seq_cropping_window_present_flag;
			state.seq_decoder_model_info_present_flag = this.seq_decoder_model_info_present_flag;
			state.seq_enabled_motion_modes = this.seq_enabled_motion_modes?.Clone();
			state.seq_force_integer_mv = this.seq_force_integer_mv;
			state.seq_force_screen_content_tools = this.seq_force_screen_content_tools;
			state.seq_frame_motion_modes_present_flag = this.seq_frame_motion_modes_present_flag;
			state.seq_header_id = this.seq_header_id;
			state.seq_initial_display_delay_minus_1 = this.seq_initial_display_delay_minus_1;
			state.seq_initial_display_delay_present_flag = this.seq_initial_display_delay_present_flag;
			state.seq_lcr_id = this.seq_lcr_id;
			state.seq_level_idx = this.seq_level_idx;
			state.seq_max_bvp_drl_bits_minus_1 = this.seq_max_bvp_drl_bits_minus_1;
			state.seq_max_drl_bits_minus_1 = this.seq_max_drl_bits_minus_1;
			state.seq_max_mlayer_cnt_minus_1 = this.seq_max_mlayer_cnt_minus_1;
			state.seq_profile_idc = this.seq_profile_idc;
			state.seq_seg_info_present_flag = this.seq_seg_info_present_flag;
			state.seq_tier = this.seq_tier;
			state.seq_tile_info_present_flag = this.seq_tile_info_present_flag;
			state.single_picture_header_flag = this.single_picture_header_flag;
			state.still_picture = this.still_picture;
			state.tlayer_dependency_map = this.tlayer_dependency_map;
			state.tlayer_dependency_present_flag = this.tlayer_dependency_present_flag;
			state.uniform_tile_spacing_flag = this.uniform_tile_spacing_flag;
			state.use_128x128_superblock = this.use_128x128_superblock;
			state.use_256x256_superblock = this.use_256x256_superblock;
			state.uv_ac_delta_q_enabled = this.uv_ac_delta_q_enabled;
			state.uv_dc_delta_q_enabled = this.uv_dc_delta_q_enabled;
			state.width_in_sbs_minus_1 = this.width_in_sbs_minus_1;
			state.y_dc_delta_q_enabled = this.y_dc_delta_q_enabled;
			return state;
		}

		private void LoadSequenceHeaderObu(SequenceHeaderObuState state)
		{
			this.ActiveNumRefFrames = state.ActiveNumRefFrames;
			this.BaseUVAcDeltaQ = state.BaseUVAcDeltaQ;
			this.BaseUVDcDeltaQ = state.BaseUVDcDeltaQ;
			this.BaseYDcDeltaQ = state.BaseYDcDeltaQ;
			this.BitDepth = state.BitDepth;
			this.CdefOnSkipTxfm = state.CdefOnSkipTxfm;
			this.DrlReorder = state.DrlReorder;
			this.EnableTipOutput = state.EnableTipOutput;
			this.MLayerDependencyMap = state.MLayerDependencyMap?.Clone();
			this.MLayerPresenceMap = state.MLayerPresenceMap?.Clone();
			this.MaxPbAspectRatio = state.MaxPbAspectRatio;
			this.MaxQ = state.MaxQ;
			this.MaxSegments = state.MaxSegments;
			this.Monochrome = state.Monochrome;
			this.NumPlanes = state.NumPlanes;
			this.NumRefFrames = state.NumRefFrames;
			this.OrderHintBits = state.OrderHintBits;
			this.SeqFeatureData = state.SeqFeatureData?.Clone();
			this.SeqFeatureEnabled = state.SeqFeatureEnabled?.Clone();
			this.SeqMaxMlayerCnt = state.SeqMaxMlayerCnt;
			this.SeqSbColStarts = state.SeqSbColStarts?.Clone();
			this.SeqSbCols = state.SeqSbCols;
			this.SeqSbRowStarts = state.SeqSbRowStarts?.Clone();
			this.SeqSbRows = state.SeqSbRows;
			this.SeqTileCols = state.SeqTileCols;
			this.SeqTileColsLog2 = state.SeqTileColsLog2;
			this.SeqTileRows = state.SeqTileRows;
			this.SeqTileRowsLog2 = state.SeqTileRowsLog2;
			this.SeqUniformTileSpacingFlag = state.SeqUniformTileSpacingFlag;
			this.SubsamplingX = state.SubsamplingX;
			this.SubsamplingY = state.SubsamplingY;
			this.TLayerDependencyMap = state.TLayerDependencyMap?.Clone();
			this.allow_frame_max_bvp_drl_bits = state.allow_frame_max_bvp_drl_bits;
			this.allow_frame_max_drl_bits = state.allow_frame_max_drl_bits;
			this.allow_tile_info_change = state.allow_tile_info_change;
			this.avg_cdf_type = state.avg_cdf_type;
			this.base_uv_ac_delta_q = state.base_uv_ac_delta_q;
			this.base_uv_dc_delta_q = state.base_uv_dc_delta_q;
			this.base_y_dc_delta_q = state.base_y_dc_delta_q;
			this.bit_depth_idc = state.bit_depth_idc;
			this.ccso_unit_matches_sb_size = state.ccso_unit_matches_sb_size;
			this.cdef_on_skip_txfm_always_on = state.cdef_on_skip_txfm_always_on;
			this.cdef_on_skip_txfm_disabled = state.cdef_on_skip_txfm_disabled;
			this.cfl_ds_filter_index = state.cfl_ds_filter_index;
			this.choose_tcq_per_frame = state.choose_tcq_per_frame;
			this.chroma_format_idc = state.chroma_format_idc;
			this.constrain_drl_reorder = state.constrain_drl_reorder;
			this.decoder_buffer_delay = state.decoder_buffer_delay;
			this.decoder_model_info_present_flag = state.decoder_model_info_present_flag;
			this.df_par_bits_minus_2 = state.df_par_bits_minus_2;
			this.disable_drl_reorder = state.disable_drl_reorder;
			this.disable_loopfilters_across_tiles = state.disable_loopfilters_across_tiles;
			this.disable_tip_output = state.disable_tip_output;
			this.enable_adaptive_mvd = state.enable_adaptive_mvd;
			this.enable_avg_cdf = state.enable_avg_cdf;
			this.enable_bawp = state.enable_bawp;
			this.enable_bru = state.enable_bru;
			this.enable_ccso = state.enable_ccso;
			this.enable_cctx = state.enable_cctx;
			this.enable_cdef = state.enable_cdef;
			this.enable_cfl_intra = state.enable_cfl_intra;
			this.enable_chroma_dctonly = state.enable_chroma_dctonly;
			this.enable_cwp = state.enable_cwp;
			this.enable_df_sub_pu = state.enable_df_sub_pu;
			this.enable_dip = state.enable_dip;
			this.enable_ext_partitions = state.enable_ext_partitions;
			this.enable_ext_seg = state.enable_ext_seg;
			this.enable_extended_sdp = state.enable_extended_sdp;
			this.enable_flex_mvres = state.enable_flex_mvres;
			this.enable_fsc = state.enable_fsc;
			this.enable_gdf = state.enable_gdf;
			this.enable_global_motion = state.enable_global_motion;
			this.enable_ibp = state.enable_ibp;
			this.enable_idtx_intra = state.enable_idtx_intra;
			this.enable_imp_msk_bld = state.enable_imp_msk_bld;
			this.enable_inter_ddt = state.enable_inter_ddt;
			this.enable_inter_ist = state.enable_inter_ist;
			this.enable_intra_edge_filter = state.enable_intra_edge_filter;
			this.enable_intra_ist = state.enable_intra_ist;
			this.enable_masked_compound = state.enable_masked_compound;
			this.enable_mhccp = state.enable_mhccp;
			this.enable_mrls = state.enable_mrls;
			this.enable_mv_traj = state.enable_mv_traj;
			this.enable_mvd_sign_derive = state.enable_mvd_sign_derive;
			this.enable_opfl_refine = state.enable_opfl_refine;
			this.enable_parity_hiding = state.enable_parity_hiding;
			this.enable_ref_frame_mvs = state.enable_ref_frame_mvs;
			this.enable_refinemv = state.enable_refinemv;
			this.enable_refmvbank = state.enable_refmvbank;
			this.enable_restoration = state.enable_restoration;
			this.enable_sdp = state.enable_sdp;
			this.enable_short_refresh_frame_flags = state.enable_short_refresh_frame_flags;
			this.enable_six_param_warp_delta = state.enable_six_param_warp_delta;
			this.enable_tcq = state.enable_tcq;
			this.enable_tip = state.enable_tip;
			this.enable_tip_explicit_qp = state.enable_tip_explicit_qp;
			this.enable_tip_hole_fill = state.enable_tip_hole_fill;
			this.enable_tip_refinemv = state.enable_tip_refinemv;
			this.enable_uneven_4way_partitions = state.enable_uneven_4way_partitions;
			this.encoder_buffer_delay = state.encoder_buffer_delay;
			this.equal_ac_dc_q = state.equal_ac_dc_q;
			this.explicit_num_ref_frames = state.explicit_num_ref_frames;
			this.explicit_ref_frame_map = state.explicit_ref_frame_map;
			this.feature_enabled = state.feature_enabled;
			this.feature_value = state.feature_value;
			this.film_grain_params_present = state.film_grain_params_present;
			this.frame_height_bits_minus_1 = state.frame_height_bits_minus_1;
			this.frame_width_bits_minus_1 = state.frame_width_bits_minus_1;
			this.gdf_unit_matches_sb_size = state.gdf_unit_matches_sb_size;
			this.height_in_sbs_minus_1 = state.height_in_sbs_minus_1;
			this.increment_tile_cols_log2 = state.increment_tile_cols_log2;
			this.increment_tile_rows_log2 = state.increment_tile_rows_log2;
			this.long_term_frame_id_bits = state.long_term_frame_id_bits;
			this.low_delay_mode_flag = state.low_delay_mode_flag;
			this.lr_tools_disable = state.lr_tools_disable?.Clone();
			this.lr_tools_uv_present = state.lr_tools_uv_present;
			this.max_frame_height_minus_1 = state.max_frame_height_minus_1;
			this.max_frame_width_minus_1 = state.max_frame_width_minus_1;
			this.max_mlayer_id = state.max_mlayer_id;
			this.max_pb_aspect_ratio_log2_minus_1 = state.max_pb_aspect_ratio_log2_minus_1;
			this.max_tlayer_id = state.max_tlayer_id;
			this.mlayer_dependency_map = state.mlayer_dependency_map;
			this.mlayer_dependency_present_flag = state.mlayer_dependency_present_flag;
			this.monotonic_output_order_flag = state.monotonic_output_order_flag;
			this.multi_tlayer_dependency_map_present_flag = state.multi_tlayer_dependency_map_present_flag;
			this.n = state.n;
			this.num_ref_frames_minus_1 = state.num_ref_frames_minus_1;
			this.num_same_ref_compound = state.num_same_ref_compound;
			this.num_units_in_decoding_tick = state.num_units_in_decoding_tick;
			this.order_hint_bits_minus_1 = state.order_hint_bits_minus_1;
			this.reduce_pb_aspect_ratio = state.reduce_pb_aspect_ratio;
			this.reduced_ref_frame_mvs_mode = state.reduced_ref_frame_mvs_mode;
			this.reduced_tx_part_set = state.reduced_tx_part_set;
			this.separate_uv_delta_q = state.separate_uv_delta_q;
			this.seq_allow_seg_info_change = state.seq_allow_seg_info_change;
			this.seq_choose_integer_mv = state.seq_choose_integer_mv;
			this.seq_choose_screen_content_tools = state.seq_choose_screen_content_tools;
			this.seq_cropping_win_bottom_offset = state.seq_cropping_win_bottom_offset;
			this.seq_cropping_win_left_offset = state.seq_cropping_win_left_offset;
			this.seq_cropping_win_right_offset = state.seq_cropping_win_right_offset;
			this.seq_cropping_win_top_offset = state.seq_cropping_win_top_offset;
			this.seq_cropping_window_present_flag = state.seq_cropping_window_present_flag;
			this.seq_decoder_model_info_present_flag = state.seq_decoder_model_info_present_flag;
			this.seq_enabled_motion_modes = state.seq_enabled_motion_modes?.Clone();
			this.seq_force_integer_mv = state.seq_force_integer_mv;
			this.seq_force_screen_content_tools = state.seq_force_screen_content_tools;
			this.seq_frame_motion_modes_present_flag = state.seq_frame_motion_modes_present_flag;
			this.seq_header_id = state.seq_header_id;
			this.seq_initial_display_delay_minus_1 = state.seq_initial_display_delay_minus_1;
			this.seq_initial_display_delay_present_flag = state.seq_initial_display_delay_present_flag;
			this.seq_lcr_id = state.seq_lcr_id;
			this.seq_level_idx = state.seq_level_idx;
			this.seq_max_bvp_drl_bits_minus_1 = state.seq_max_bvp_drl_bits_minus_1;
			this.seq_max_drl_bits_minus_1 = state.seq_max_drl_bits_minus_1;
			this.seq_max_mlayer_cnt_minus_1 = state.seq_max_mlayer_cnt_minus_1;
			this.seq_profile_idc = state.seq_profile_idc;
			this.seq_seg_info_present_flag = state.seq_seg_info_present_flag;
			this.seq_tier = state.seq_tier;
			this.seq_tile_info_present_flag = state.seq_tile_info_present_flag;
			this.single_picture_header_flag = state.single_picture_header_flag;
			this.still_picture = state.still_picture;
			this.tlayer_dependency_map = state.tlayer_dependency_map;
			this.tlayer_dependency_present_flag = state.tlayer_dependency_present_flag;
			this.uniform_tile_spacing_flag = state.uniform_tile_spacing_flag;
			this.use_128x128_superblock = state.use_128x128_superblock;
			this.use_256x256_superblock = state.use_256x256_superblock;
			this.uv_ac_delta_q_enabled = state.uv_ac_delta_q_enabled;
			this.uv_dc_delta_q_enabled = state.uv_dc_delta_q_enabled;
			this.width_in_sbs_minus_1 = state.width_in_sbs_minus_1;
			this.y_dc_delta_q_enabled = state.y_dc_delta_q_enabled;
		}

		/// <summary>What film_grain_model and the structures it calls assign, as SaveFilmGrainModel keeps it.</summary>
		private sealed partial class FilmGrainModelState
		{
			public int ar_coeff_lag;
			public int ar_coeff_shift_minus_6;
			public AomArray<int> ar_coeffs_cb;
			public AomArray<int> ar_coeffs_cr;
			public AomArray<int> ar_coeffs_y;
			public int bits_per_ar_coeff_cb_minus_5;
			public int bits_per_ar_coeff_cr_minus_5;
			public int bits_per_ar_coeff_y_minus_5;
			public int cb_luma_mult;
			public int cb_mult;
			public int cb_offset;
			public int chroma_scaling_from_luma;
			public int clip_to_restricted_range;
			public int cr_luma_mult;
			public int cr_mult;
			public int cr_offset;
			public int fg_mc_identity;
			public int film_grain_block_size;
			public int grain_scale_shift;
			public int grain_scaling_minus_8;
			public int num_cb_points;
			public int num_cr_points;
			public int num_y_points;
			public int overlap_flag;
			public AomArray<int> point_cb_scaling;
			public AomArray<int> point_cb_value;
			public AomArray<int> point_cr_scaling;
			public AomArray<int> point_cr_value;
			public int point_scaling_bits_minus_5;
			public int point_value_increment_bits_minus_1;
			public AomArray<int> point_y_scaling;
			public AomArray<int> point_y_value;
		}

		private FilmGrainModelState SaveFilmGrainModel()
		{
			var state = new FilmGrainModelState();
			state.ar_coeff_lag = this.ar_coeff_lag;
			state.ar_coeff_shift_minus_6 = this.ar_coeff_shift_minus_6;
			state.ar_coeffs_cb = this.ar_coeffs_cb?.Clone();
			state.ar_coeffs_cr = this.ar_coeffs_cr?.Clone();
			state.ar_coeffs_y = this.ar_coeffs_y?.Clone();
			state.bits_per_ar_coeff_cb_minus_5 = this.bits_per_ar_coeff_cb_minus_5;
			state.bits_per_ar_coeff_cr_minus_5 = this.bits_per_ar_coeff_cr_minus_5;
			state.bits_per_ar_coeff_y_minus_5 = this.bits_per_ar_coeff_y_minus_5;
			state.cb_luma_mult = this.cb_luma_mult;
			state.cb_mult = this.cb_mult;
			state.cb_offset = this.cb_offset;
			state.chroma_scaling_from_luma = this.chroma_scaling_from_luma;
			state.clip_to_restricted_range = this.clip_to_restricted_range;
			state.cr_luma_mult = this.cr_luma_mult;
			state.cr_mult = this.cr_mult;
			state.cr_offset = this.cr_offset;
			state.fg_mc_identity = this.fg_mc_identity;
			state.film_grain_block_size = this.film_grain_block_size;
			state.grain_scale_shift = this.grain_scale_shift;
			state.grain_scaling_minus_8 = this.grain_scaling_minus_8;
			state.num_cb_points = this.num_cb_points;
			state.num_cr_points = this.num_cr_points;
			state.num_y_points = this.num_y_points;
			state.overlap_flag = this.overlap_flag;
			state.point_cb_scaling = this.point_cb_scaling?.Clone();
			state.point_cb_value = this.point_cb_value?.Clone();
			state.point_cr_scaling = this.point_cr_scaling?.Clone();
			state.point_cr_value = this.point_cr_value?.Clone();
			state.point_scaling_bits_minus_5 = this.point_scaling_bits_minus_5;
			state.point_value_increment_bits_minus_1 = this.point_value_increment_bits_minus_1;
			state.point_y_scaling = this.point_y_scaling?.Clone();
			state.point_y_value = this.point_y_value?.Clone();
			return state;
		}

		private void LoadFilmGrainModel(FilmGrainModelState state)
		{
			this.ar_coeff_lag = state.ar_coeff_lag;
			this.ar_coeff_shift_minus_6 = state.ar_coeff_shift_minus_6;
			this.ar_coeffs_cb = state.ar_coeffs_cb?.Clone();
			this.ar_coeffs_cr = state.ar_coeffs_cr?.Clone();
			this.ar_coeffs_y = state.ar_coeffs_y?.Clone();
			this.bits_per_ar_coeff_cb_minus_5 = state.bits_per_ar_coeff_cb_minus_5;
			this.bits_per_ar_coeff_cr_minus_5 = state.bits_per_ar_coeff_cr_minus_5;
			this.bits_per_ar_coeff_y_minus_5 = state.bits_per_ar_coeff_y_minus_5;
			this.cb_luma_mult = state.cb_luma_mult;
			this.cb_mult = state.cb_mult;
			this.cb_offset = state.cb_offset;
			this.chroma_scaling_from_luma = state.chroma_scaling_from_luma;
			this.clip_to_restricted_range = state.clip_to_restricted_range;
			this.cr_luma_mult = state.cr_luma_mult;
			this.cr_mult = state.cr_mult;
			this.cr_offset = state.cr_offset;
			this.fg_mc_identity = state.fg_mc_identity;
			this.film_grain_block_size = state.film_grain_block_size;
			this.grain_scale_shift = state.grain_scale_shift;
			this.grain_scaling_minus_8 = state.grain_scaling_minus_8;
			this.num_cb_points = state.num_cb_points;
			this.num_cr_points = state.num_cr_points;
			this.num_y_points = state.num_y_points;
			this.overlap_flag = state.overlap_flag;
			this.point_cb_scaling = state.point_cb_scaling?.Clone();
			this.point_cb_value = state.point_cb_value?.Clone();
			this.point_cr_scaling = state.point_cr_scaling?.Clone();
			this.point_cr_value = state.point_cr_value?.Clone();
			this.point_scaling_bits_minus_5 = state.point_scaling_bits_minus_5;
			this.point_value_increment_bits_minus_1 = state.point_value_increment_bits_minus_1;
			this.point_y_scaling = state.point_y_scaling?.Clone();
			this.point_y_value = state.point_y_value?.Clone();
		}

		/// <summary>What film_grain_config and the structures it calls assign, as SaveFilmGrainConfig keeps it.</summary>
		private sealed partial class FilmGrainConfigState
		{
			public int apply_grain;
			public int fgm_id;
			public int grain_seed;
		}

		private FilmGrainConfigState SaveFilmGrainConfig()
		{
			var state = new FilmGrainConfigState();
			state.apply_grain = this.apply_grain;
			state.fgm_id = this.fgm_id;
			state.grain_seed = this.grain_seed;
			return state;
		}

		private void LoadFilmGrainConfig(FilmGrainConfigState state)
		{
			this.apply_grain = state.apply_grain;
			this.fgm_id = state.fgm_id;
			this.grain_seed = state.grain_seed;
		}

		/// <summary>What the context holds, as SaveContext keeps it.</summary>
		private sealed partial class ContextState
		{
			public int ActiveNumRefFrames;
			public int AllowedFrames;
			public int AtlasHeight;
			public AomArray<AomArray<AomArray<int>>> AtlasSegmentIDToIndex;
			public AomArray<AomArray<AomArray<int>>> AtlasSegmentIndexToID;
			public int AtlasWidth;
			public int BaseUVAcDeltaQ;
			public int BaseUVDcDeltaQ;
			public int BaseYDcDeltaQ;
			public int BitDepth;
			public int BruTileActive;
			public AomArray<AomArray<int>> BruTileActives;
			public AomArray<AomArray<AomArray<AomArray<int>>>> CcsoFilterOffset;
			public int CcsoLumaSizeLog2;
			public int CdefDamping;
			public int CdefOnSkipTxfm;
			public int CdefStrengths;
			public int ClosestFuture;
			public int ClosestPast;
			public int CodedLossless;
			public int CountFrameHeaderForLevelConstraint;
			public int CropHeight;
			public int CropLeft;
			public int CropTop;
			public int CropWidth;
			public int CurrentQIndex;
			public int DeltaQUAc;
			public int DeltaQUDc;
			public int DeltaQVAc;
			public int DeltaQVDc;
			public int DeltaQYDc;
			public int DerivedPrimaryRefFrame;
			public AomArray<int> DfDeltaQ;
			public int DrlReorder;
			public int EnableTipOutput;
			public AomArray<AomArray<int>> FeatureData;
			public AomArray<AomArray<int>> FeatureEnabled;
			public AomArray<int> FgmChromaIdc;
			public AomArray<int> FgmMLayerId;
			public AomArray<int> FgmTLayerId;
			public AomArray<int> FilmGrainPresent;
			public int FirstPictureInTU;
			public AomArray<int> FrameDistance;
			public int FrameHeight;
			public int FrameIsIntra;
			public AomArray<AomArray<AomArray<int>>> FrameLrWienerNs;
			public int FrameMvPrecision;
			public AomArray<int> FrameRestorationType;
			public int FrameSymbolCount;
			public int FrameType;
			public int FrameWidth;
			public int FurthestFuture;
			public int GdfBlkSize;
			public int GdfPixScale;
			public AomArray<int> GmType;
			public int HasBothRefs;
			public int HasLosslessSegment;
			public AomArray<AomArray<int>> IBCCoded;
			public int IsBridge;
			public int IsRegular;
			public int LastActiveSegId;
			public int LcrMaxNumXLayerCount;
			public AomArray<int> LcrXLayerID;
			public int LongTermId;
			public AomArray<int> LoopRestorationSize;
			public AomArray<int> LosslessArray;
			public AomArray<AomArray<AomArray<AomArray<int>>>> LrWienerNs;
			public AomArray<AomArray<int>> MLayerDependencyMap;
			public AomArray<AomArray<int>> MLayerPresenceMap;
			public int MaxPbAspectRatio;
			public int MaxQ;
			public int MaxSegments;
			public AomArray<AomArray<AomArray<int>>> MfhFeatureData;
			public AomArray<AomArray<AomArray<int>>> MfhFeatureEnabled;
			public AomArray<int> MfhMLayerId;
			public AomArray<int> MfhSeqHeaderId;
			public AomArray<int> MfhTLayerId;
			public int MiColEnd;
			public int MiColStart;
			public AomArray<int> MiColStarts;
			public int MiCols;
			public int MiRowEnd;
			public int MiRowStart;
			public AomArray<int> MiRowStarts;
			public int MiRows;
			public int Monochrome;
			public AomArray<AomArray<int>> MotionFieldChecked;
			public AomArray<int> MotionFieldDepth;
			public AomArray<int> MotionFieldStack;
			public int MotionFieldStackCount;
			public AomArray<int> MotionFieldVisited;
			public int MvPrecision;
			public int NRanked;
			public int NumFilterClasses;
			public int NumFrameHeaderBits;
			public int NumFutureRefs;
			public int NumPastRefs;
			public int NumPlanes;
			public int NumRefFrames;
			public AomArray<int> NumRegionsInAtlas;
			public int NumSameRefCompound;
			public int NumTiles;
			public int NumTotalRefs;
			public int OlkEncountered;
			public AomArray<int> OlkRefresh;
			public int OlkTUOrderHint;
			public AomArray<AomArray<AomArray<AomArray<int>>>> OpsxLayerId;
			public int OrderHint;
			public int OrderHintBits;
			public int OrderHintLsbs;
			public AomArray<int> OrderHints;
			public int OrigClosestFuture;
			public int OrigClosestPast;
			public AomArray<AomArray<int>> PrevGmParams;
			public AomArray<AomArray<int>> PrevSegmentIds;
			public int ProjStep;
			public AomArray<int> QmDataPresent;
			public AomArray<int> QmMLayerId;
			public AomArray<int> QmNumPlanes;
			public AomArray<int> QmProtected;
			public AomArray<int> QmSeen;
			public AomArray<int> QmTLayerId;
			public AomArray<AomArray<AomArray<AomArray<int>>>> RefLrWienerNs;
			public AomArray<int> RefOrderHint;
			public AomArray<int> RefValid;
			public int RemainingLcrPayloadBits;
			public int SbSize;
			public AomArray<int> ScoresBaseQIdx;
			public AomArray<int> ScoresDistance;
			public AomArray<int> ScoresIndex;
			public AomArray<int> ScoresLayer;
			public AomArray<int> ScoresOrderHint;
			public AomArray<int> ScoresScore;
			public int SeenFrameHeader;
			public int SegIdPreSkip;
			public AomArray<AomArray<int>> SegQMLevel;
			public AomArray<AomArray<int>> SegmentIds;
			public AomArray<AomArray<int>> SeqFeatureData;
			public AomArray<AomArray<int>> SeqFeatureEnabled;
			public int SeqMaxMlayerCnt;
			public AomArray<int> SeqSbColStarts;
			public int SeqSbCols;
			public AomArray<int> SeqSbRowStarts;
			public int SeqSbRows;
			public int SeqTileCols;
			public int SeqTileColsLog2;
			public int SeqTileRows;
			public int SeqTileRowsLog2;
			public int SeqUniformTileSpacingFlag;
			public int ShowExistingFrame;
			public AomArray<int> SkipModeFrame;
			public int SkipSegFrame;
			public AomArray<int> SubclassLookup;
			public int SubsamplingX;
			public int SubsamplingY;
			public AomArray<AomArray<AomArray<int>>> TLayerDependencyMap;
			public int TileCols;
			public int TileColsLog2;
			public int TileNum;
			public int TileRows;
			public int TileRowsLog2;
			public int TileSizeBytes;
			public int TipFrameMode;
			public AomArray<int> TipGlobalMv;
			public int TipInterpFilter;
			public int TxMode;
			public int UsePerBlockMvPrecision;
			public AomArray<AomArray<AomArray<AomArray<AomArray<int>>>>> UserQm;
			public int UsesLr;
			public AomArray<AomArray<int>> WienerNsBankSize;
			public AomArray<AomArray<int>> WienerNsPtr;
			public AomArray<AomArray<AomArray<int>>> XCount;
			public int a;
			public int allow_bawp;
			public int allow_df_sub_pu;
			public int allow_frame_max_bvp_drl_bits;
			public int allow_frame_max_drl_bits;
			public int allow_global_intrabc;
			public int allow_high_precision_mv;
			public int allow_intrabc;
			public int allow_local_intrabc;
			public int allow_parity_hiding;
			public int allow_screen_content_tools;
			public int allow_tcq;
			public int allow_tile_info_change;
			public int allow_tip_hole_fill;
			public int allow_warpmv_mode;
			public AomArray<int> apply_deblocking_filter;
			public int apply_deblocking_filter_tip;
			public int apply_grain;
			public int ar_coeff_lag;
			public int ar_coeff_shift_minus_6;
			public AomArray<int> ar_coeffs_cb;
			public AomArray<int> ar_coeffs_cr;
			public AomArray<int> ar_coeffs_y;
			public AomArray<int> atlas_segment_id;
			public AomArray<AomArray<AomArray<int>>> ats_atlas_segment_id;
			public AomArray<int> ats_atlas_segment_mode_idc;
			public AomArray<AomArray<int>> ats_bottom_right_region_column_off;
			public AomArray<AomArray<int>> ats_bottom_right_region_row_off;
			public AomArray<AomArray<int>> ats_column_width_minus_1;
			public AomArray<int> ats_height;
			public AomArray<AomArray<int>> ats_input_stream_id;
			public AomArray<AomArray<AomArray<int>>> ats_msi_alpha_segment_flag;
			public AomArray<AomArray<int>> ats_msi_alpha_segments_present_flag;
			public AomArray<AomArray<int>> ats_msi_background_blue_value;
			public AomArray<AomArray<int>> ats_msi_background_green_value;
			public AomArray<AomArray<int>> ats_msi_background_info_present_flag;
			public AomArray<AomArray<int>> ats_msi_background_red_value;
			public AomArray<AomArray<int>> ats_msi_height;
			public AomArray<AomArray<AomArray<int>>> ats_msi_input_stream_id;
			public AomArray<AomArray<int>> ats_msi_num_atlas_segments_minus_1;
			public AomArray<AomArray<AomArray<int>>> ats_msi_segment_height;
			public AomArray<AomArray<AomArray<int>>> ats_msi_segment_top_left_pos_x;
			public AomArray<AomArray<AomArray<int>>> ats_msi_segment_top_left_pos_y;
			public AomArray<AomArray<AomArray<int>>> ats_msi_segment_width;
			public AomArray<AomArray<int>> ats_msi_width;
			public AomArray<int> ats_nominal_height_minus_1;
			public AomArray<int> ats_nominal_width_minus_1;
			public AomArray<int> ats_num_atlas_segments_minus_1;
			public AomArray<int> ats_num_region_columns_minus_1;
			public AomArray<int> ats_num_region_rows_minus_1;
			public AomArray<int> ats_region_height_minus_1;
			public AomArray<int> ats_region_width_minus_1;
			public AomArray<AomArray<int>> ats_row_height_minus_1;
			public AomArray<AomArray<int>> ats_segment_height;
			public AomArray<AomArray<int>> ats_segment_top_left_pos_x;
			public AomArray<AomArray<int>> ats_segment_top_left_pos_y;
			public AomArray<AomArray<int>> ats_segment_width;
			public AomArray<AomArray<int>> ats_signaled_atlas_segment_ids_flag;
			public AomArray<int> ats_single_region_per_atlas_segment_flag;
			public AomArray<int> ats_stream_id_present;
			public AomArray<AomArray<int>> ats_top_left_region_column;
			public AomArray<AomArray<int>> ats_top_left_region_row;
			public AomArray<int> ats_uniform_spacing_flag;
			public AomArray<int> ats_width;
			public int avg_cdf_type;
			public int b;
			public int band_block_in_luma_samples;
			public int band_units_information_present_flag;
			public int banding_hints_flag;
			public int banding_in_band_unit_present_flag;
			public int banding_in_component_present_flag;
			public int baseDistance;
			public AomArray<int> baseParams;
			public int base_q_idx;
			public int base_qindex;
			public int base_uv_ac_delta_q;
			public int base_uv_dc_delta_q;
			public int base_y_dc_delta_q;
			public int bestDisp;
			public int bestRatio;
			public int bit_depth_idc;
			public int bits_per_ar_coeff_cb_minus_5;
			public int bits_per_ar_coeff_cr_minus_5;
			public int bits_per_ar_coeff_y_minus_5;
			public int blkSize;
			public AomArray<AomArray<int>> br_decoder_model_present_op_flag;
			public AomArray<int> br_ops_cnt;
			public int br_ops_dependent_flag;
			public int br_ops_id;
			public int br_time;
			public AomArray<AomArray<int>> br_time_op;
			public int bridge_frame_height_minus_1;
			public int bridge_frame_overwrite_flag;
			public int bridge_frame_ref_idx;
			public int bridge_frame_width_minus_1;
			public int bru_inactive;
			public int bru_ref;
			public int bru_tile_active;
			public int cb_luma_mult;
			public int cb_mult;
			public int cb_offset;
			public AomArray<int> ccso_bo_only;
			public AomArray<int> ccso_edge_clf;
			public AomArray<int> ccso_ext_filter;
			public int ccso_frame_flag;
			public AomArray<int> ccso_max_band_log2;
			public int ccso_offset_idx;
			public AomArray<int> ccso_planes;
			public AomArray<int> ccso_quant_idx;
			public AomArray<int> ccso_ref_idx;
			public AomArray<int> ccso_scale_idx;
			public int ccso_unit_matches_sb_size;
			public int cdef_damping_minus_3;
			public int cdef_frame_enable;
			public int cdef_on_skip_txfm_always_on;
			public int cdef_on_skip_txfm_disabled;
			public int cdef_on_skip_txfm_frame_enable;
			public int cdef_strengths_minus_1;
			public AomArray<int> cdef_uv_pri_strength;
			public int cdef_uv_pri_zero;
			public AomArray<int> cdef_uv_sec_strength;
			public AomArray<int> cdef_y_pri_strength;
			public int cdef_y_pri_zero;
			public AomArray<int> cdef_y_sec_strength;
			public int cfl_ds_filter_index;
			public int change_bvp_drl;
			public int change_drl;
			public int checkRes;
			public int choose_tcq_per_frame;
			public int chroma_format_idc;
			public int chroma_scaling_from_luma;
			public int ci_aspect_ratio_idc;
			public int ci_aspect_ratio_info_present_flag;
			public int ci_chroma_sample_position_bottom;
			public int ci_chroma_sample_position_present_flag;
			public int ci_chroma_sample_position_top;
			public int ci_color_description_idc;
			public int ci_color_description_present_flag;
			public int ci_color_primaries;
			public int ci_full_range_flag;
			public int ci_matrix_coefficients;
			public int ci_reserved_2bit;
			public int ci_sar_height;
			public int ci_sar_width;
			public int ci_scan_type_idc;
			public int ci_timing_info_present_flag;
			public int ci_transfer_characteristics;
			public int clip_to_restricted_range;
			public int cnt_dropped_flag;
			public int coding_banding_present_flag;
			public int constrain_drl_reorder;
			public int context_update_tile_id;
			public int counting_type;
			public AomArray<int> counts;
			public int cr_luma_mult;
			public int cr_mult;
			public int cr_offset;
			public int cur_mfh_id;
			public int d;
			public int decoder_buffer_delay;
			public int decoder_model_info_present_flag;
			public int delta_coded;
			public int delta_q;
			public int delta_q_present;
			public int delta_q_res;
			public int derive_sef_order_hint;
			public AomArray<int> df_delta_q;
			public AomArray<int> df_delta_q_present;
			public int df_par_bits_minus_2;
			public int diff_uv_delta;
			public int disable_cdf_update;
			public int disable_cross_frame_cdf_init;
			public int disable_drl_reorder;
			public int disable_loopfilters_across_tiles;
			public int disable_tip_output;
			public int discontinuity_flag;
			public int dist;
			public int enable_adaptive_mvd;
			public int enable_avg_cdf;
			public int enable_bawp;
			public int enable_bru;
			public int enable_ccso;
			public int enable_cctx;
			public int enable_cdef;
			public int enable_cfl_intra;
			public int enable_chroma_dctonly;
			public int enable_cwp;
			public int enable_df_sub_pu;
			public int enable_dip;
			public int enable_ext_partitions;
			public int enable_ext_seg;
			public int enable_extended_sdp;
			public int enable_flex_mvres;
			public int enable_fsc;
			public int enable_gdf;
			public int enable_global_motion;
			public int enable_ibp;
			public int enable_idtx_intra;
			public int enable_imp_msk_bld;
			public int enable_inter_ddt;
			public int enable_inter_ist;
			public int enable_intra_edge_filter;
			public int enable_intra_ist;
			public int enable_masked_compound;
			public int enable_mhccp;
			public int enable_mrls;
			public int enable_mv_traj;
			public int enable_mvd_sign_derive;
			public int enable_opfl_refine;
			public int enable_parity_hiding;
			public int enable_ref_frame_mvs;
			public int enable_refinemv;
			public int enable_refmvbank;
			public int enable_restoration;
			public int enable_sdp;
			public int enable_short_refresh_frame_flags;
			public int enable_six_param_warp_delta;
			public int enable_tcq;
			public int enable_tip;
			public int enable_tip_explicit_qp;
			public int enable_tip_hole_fill;
			public int enable_tip_refinemv;
			public int enable_uneven_4way_partitions;
			public int encoder_buffer_delay;
			public int equal_ac_dc_q;
			public int equal_picture_interval;
			public int explicit_num_ref_frames;
			public int explicit_ref_frame_map;
			public int feature;
			public int feature_enabled;
			public int feature_value;
			public int fg_mc_identity;
			public int fgm_chroma_idc;
			public int fgm_id;
			public int fgm_update_flags;
			public int film_grain_block_size;
			public int film_grain_params_present;
			public int force_integer_mv;
			public int found_ref;
			public int frameHeight;
			public int frameWidth;
			public AomArray<int> frame_enabled_motion_modes;
			public int frame_explicit_ref_frame_map;
			public AomArray<int> frame_filters_on;
			public byte[] frame_hash;
			public int frame_header_present_flag;
			public int frame_height_bits_minus_1;
			public int frame_height_minus_1;
			public int frame_is_inter;
			public int frame_presentation_time;
			public int frame_size_override_flag;
			public int frame_to_refresh;
			public int frame_to_show_map_idx;
			public int frame_width_bits_minus_1;
			public int frame_width_minus_1;
			public int full_timestamp_flag;
			public int gdf_frame_enable;
			public int gdf_per_block;
			public int gdf_pic_qc_idx;
			public int gdf_pic_scale_idx;
			public int gdf_unit_matches_sb_size;
			public AomArray<AomArray<int>> gm_params;
			public int grain_scale_shift;
			public int grain_scaling_minus_8;
			public int grain_seed;
			public int group_bit;
			public int has_grain;
			public int has_refresh_frame_flags;
			public int hash_type;
			public AomArray<int> header_bit;
			public int height_in_sbs_minus_1;
			public int high;
			public int horz_size_in_band_blocks_minus_1;
			public int hours_flag;
			public int hours_value;
			public int icc_profile_data_payload_bytes;
			public int idx;
			public int immediate_output_frame;
			public int implicit_output_frame;
			public int increment_tile_cols_log2;
			public int increment_tile_rows_log2;
			public int interpolation_filter;
			public int isBridge;
			public int isFirst;
			public int isGlobal;
			public int is_filter_switchable;
			public int is_first_tile_group;
			public int is_global;
			public int is_monochrome;
			public int is_rot_zoom;
			public int itu_t_t35_country_code;
			public int itu_t_t35_country_code_extension_byte;
			public int itu_t_t35_payload_bytes;
			public AomArray<AomArray<int>> layer_color_description_idc;
			public AomArray<AomArray<int>> layer_color_primaries;
			public AomArray<AomArray<int>> layer_full_range_flag;
			public AomArray<AomArray<int>> layer_matrix_coefficients;
			public AomArray<AomArray<int>> layer_transfer_characteristics;
			public int lcr_aggregate_info_present_flag;
			public int lcr_aggregate_level_idx;
			public AomArray<AomArray<AomArray<int>>> lcr_auxiliary_type;
			public AomArray<AomArray<int>> lcr_bit_depth_idc;
			public AomArray<AomArray<int>> lcr_chroma_format_idc;
			public int lcr_config_idc;
			public AomArray<AomArray<int>> lcr_cropping_win_bottom_offset;
			public AomArray<AomArray<int>> lcr_cropping_win_left_offset;
			public AomArray<AomArray<int>> lcr_cropping_win_right_offset;
			public AomArray<AomArray<int>> lcr_cropping_win_top_offset;
			public AomArray<AomArray<int>> lcr_cropping_window_present_flag;
			public AomArray<int> lcr_data_size;
			public AomArray<AomArray<AomArray<int>>> lcr_dependent_layer_map;
			public int lcr_dependent_xlayers_flag;
			public int lcr_doh_constraint_flag;
			public AomArray<AomArray<int>> lcr_embedded_layer_info_present_flag;
			public int lcr_enforce_tile_alignment_flag;
			public AomArray<AomArray<int>> lcr_format_info_present_flag;
			public int lcr_global_atlas_id;
			public int lcr_global_atlas_id_present_flag;
			public int lcr_global_config_record_id;
			public AomArray<int> lcr_global_id;
			public int lcr_global_payload_present_flag;
			public int lcr_global_purpose_id;
			public int lcr_global_reserved_zero_3bits;
			public int lcr_global_reserved_zero_5bits;
			public AomArray<AomArray<AomArray<int>>> lcr_layer_atlas_segment_id;
			public AomArray<AomArray<AomArray<int>>> lcr_layer_type;
			public AomArray<int> lcr_local_atlas_id;
			public AomArray<int> lcr_local_atlas_id_present_flag;
			public AomArray<int> lcr_local_id;
			public AomArray<int> lcr_local_reserved_zero_3bits;
			public AomArray<int> lcr_local_reserved_zero_5bits;
			public AomArray<AomArray<AomArray<int>>> lcr_max_expected_height;
			public AomArray<AomArray<AomArray<int>>> lcr_max_expected_width;
			public int lcr_max_interop;
			public AomArray<int> lcr_max_level_idx;
			public AomArray<int> lcr_max_mlayer_count;
			public AomArray<AomArray<int>> lcr_max_pic_height;
			public AomArray<AomArray<int>> lcr_max_pic_width;
			public int lcr_max_tier_flag;
			public AomArray<AomArray<int>> lcr_mlayer_map;
			public AomArray<int> lcr_num_dependent_xlayer_map;
			public AomArray<AomArray<AomArray<int>>> lcr_priority_order;
			public AomArray<int> lcr_profile_tier_level_info_present_flag;
			public int lcr_remaining_payload_bit;
			public AomArray<AomArray<AomArray<int>>> lcr_rendering_method;
			public AomArray<AomArray<int>> lcr_rep_info_present_flag;
			public AomArray<AomArray<AomArray<int>>> lcr_same_sh_max_resolution_flag;
			public AomArray<int> lcr_seq_profile_idc;
			public int lcr_seq_profile_tier_level_info_present_flag;
			public AomArray<int> lcr_tier_flag;
			public AomArray<AomArray<AomArray<int>>> lcr_tlayer_map;
			public AomArray<AomArray<AomArray<int>>> lcr_view_id;
			public AomArray<AomArray<AomArray<int>>> lcr_view_type;
			public AomArray<int> lcr_xlayer_atlas_segment_id;
			public AomArray<AomArray<int>> lcr_xlayer_color_info_present_flag;
			public int lcr_xlayer_map;
			public AomArray<int> lcr_xlayer_priority_order;
			public AomArray<AomArray<int>> lcr_xlayer_purpose_id;
			public AomArray<AomArray<int>> lcr_xlayer_purpose_present_flag;
			public AomArray<int> lcr_xlayer_rendering_method;
			public int level;
			public int loadCdfs;
			public int longTermId;
			public int long_term_frame_id_bits;
			public int long_term_id_plus_1;
			public int low;
			public int low_delay_mode_flag;
			public int lr_chroma_use_half_size;
			public int lr_chroma_use_max_size;
			public int lr_chroma_use_quarter_size;
			public int lr_luma_use_half_size;
			public int lr_luma_use_max_size;
			public int lr_luma_use_quarter_size;
			public AomArray<AomArray<int>> lr_tools_disable;
			public int lr_tools_uv_present;
			public int lsptli_reserved_2bits;
			public int luminance_max;
			public int luminance_min;
			public int m;
			public int max_band_step_minus_1;
			public int max_band_width_minus_4;
			public int max_bvp_drl_bits_minus_1;
			public int max_cll;
			public int max_drl_bits_minus_1;
			public int max_fall;
			public int max_frame_height_minus_1;
			public int max_frame_width_minus_1;
			public int max_mlayer_id;
			public int max_pb_aspect_ratio_log2_minus_1;
			public int max_tlayer_id;
			public int merged_param;
			public int metadataPayloadSize;
			public int metadata_application_id;
			public int metadata_is_suffix;
			public int metadata_necessity_idc;
			public int metadata_type;
			public int metadata_unit_cnt_minus_1;
			public int metadata_unit_remaining_bit;
			public AomArray<int> mfh_allow_seg_info_change;
			public AomArray<AomArray<int>> mfh_apply_deblocking_filter;
			public AomArray<int> mfh_deblocking_filter_update;
			public AomArray<int> mfh_ext_seg_flag;
			public int mfh_frame_height_bits_minus_1;
			public AomArray<int> mfh_frame_height_minus_1;
			public AomArray<int> mfh_frame_size_present_flag;
			public int mfh_frame_width_bits_minus_1;
			public AomArray<int> mfh_frame_width_minus_1;
			public int mfh_id_minus_1;
			public AomArray<int> mfh_seg_info_present_flag;
			public int mfh_seq_header_id;
			public int minutes_flag;
			public int minutes_value;
			public int mis;
			public int mlayer_dependency_map;
			public int mlayer_dependency_present_flag;
			public int monochrome;
			public int monotonic_output_order_flag;
			public int mps_duplicate_flag;
			public int mps_pic_struct_type;
			public int mps_source_scan_type_idc;
			public int muh_cancel_flag;
			public int muh_header_extension_byte;
			public int muh_header_size;
			public int muh_layer_idc;
			public int muh_mlayer_map;
			public int muh_payload_size;
			public int muh_persistence_idc;
			public int muh_priority;
			public int muh_reserved_zero_2bits;
			public int muh_xlayer_map;
			public int multi_tlayer_dependency_map_present_flag;
			public int multistream_doh_constraint_flag;
			public int multistream_even_allocation_flag;
			public int multistream_large_picture_idc;
			public int multistream_level_idx;
			public int multistream_profile_idc;
			public int multistream_tier;
			public int mx;
			public int n;
			public int n_frames;
			public long nbBits;
			public int numSegments;
			public int numSyms;
			public int num_band_units_cols_minus_1;
			public int num_band_units_rows_minus_1;
			public int num_cb_points;
			public int num_cr_points;
			public int num_filter_classes_idx;
			public int num_key_ref_frames;
			public int num_ref_frames_minus_1;
			public int num_same_ref_compound;
			public int num_streams_minus_2;
			public int num_ticks_per_picture_minus_1;
			public int num_total_refs;
			public int num_units_in_decoding_tick;
			public int num_units_in_display_tick;
			public int num_y_points;
			public int obuPayloadSize;
			public int obuXLId;
			public int obu_extension_data_bit;
			public int obu_extension_flag;
			public int obu_header_extension_flag;
			public int obu_mlayer_id;
			public int obu_padding_byte;
			public int obu_tlayer_id;
			public int obu_type;
			public int obu_xlayer_id;
			public int onlyShowable;
			public int opIndex;
			public int opfl_refine_all;
			public int opfl_refine_type;
			public int opsID;
			public AomArray<AomArray<int>> ops_aggregate_level_idx;
			public AomArray<AomArray<int>> ops_cnt;
			public AomArray<AomArray<AomArray<int>>> ops_color_description_idc;
			public AomArray<AomArray<int>> ops_color_info_present_flag;
			public AomArray<AomArray<AomArray<int>>> ops_color_primaries;
			public AomArray<AomArray<int>> ops_config_idc;
			public AomArray<AomArray<AomArray<int>>> ops_data_size;
			public AomArray<AomArray<AomArray<int>>> ops_decoder_buffer_delay;
			public AomArray<AomArray<AomArray<int>>> ops_decoder_model_info_for_this_op_present_flag;
			public AomArray<AomArray<AomArray<int>>> ops_embedded_op_index;
			public AomArray<AomArray<AomArray<int>>> ops_embedded_ops_id;
			public AomArray<AomArray<AomArray<int>>> ops_encoder_buffer_delay;
			public AomArray<AomArray<AomArray<int>>> ops_full_range_flag;
			public AomArray<int> ops_id;
			public AomArray<AomArray<AomArray<int>>> ops_initial_display_delay_minus_1;
			public AomArray<AomArray<AomArray<int>>> ops_initial_display_delay_present_flag;
			public AomArray<AomArray<int>> ops_intent;
			public AomArray<AomArray<int>> ops_intent_present_flag;
			public AomArray<AomArray<AomArray<AomArray<int>>>> ops_level_idx;
			public AomArray<AomArray<AomArray<int>>> ops_low_delay_mode_flag;
			public AomArray<AomArray<AomArray<int>>> ops_matrix_coefficients;
			public AomArray<AomArray<int>> ops_max_interop;
			public AomArray<AomArray<int>> ops_max_tier_flag;
			public AomArray<AomArray<AomArray<AomArray<int>>>> ops_mlayer_count;
			public AomArray<AomArray<AomArray<int>>> ops_mlayer_explicit_info_flag;
			public AomArray<int> ops_mlayer_info_idc;
			public AomArray<AomArray<AomArray<AomArray<int>>>> ops_mlayer_map;
			public AomArray<AomArray<AomArray<int>>> ops_op_intent;
			public AomArray<AomArray<int>> ops_priority;
			public AomArray<AomArray<int>> ops_ptl_present_flag;
			public int ops_ptl_reserved_2bits;
			public int ops_reserved_2bits;
			public AomArray<int> ops_reset_flag;
			public AomArray<AomArray<AomArray<AomArray<int>>>> ops_seq_profile_idc;
			public AomArray<AomArray<AomArray<AomArray<int>>>> ops_tier_flag;
			public AomArray<AomArray<AomArray<AomArray<AomArray<int>>>>> ops_tlayer_map;
			public AomArray<AomArray<AomArray<int>>> ops_transfer_characteristics;
			public AomArray<AomArray<int>> ops_xlayer_map;
			public int order_hint;
			public int order_hint_bits_minus_1;
			public int our_ref;
			public int overlap_flag;
			public int per_plane;
			public int pic_qm_num_minus_1;
			public int plane;
			public AomArray<byte[]> plane_hash;
			public AomArray<int> point_cb_scaling;
			public AomArray<int> point_cb_value;
			public AomArray<int> point_cr_scaling;
			public AomArray<int> point_cr_value;
			public int point_scaling_bits_minus_5;
			public int point_value_increment_bits_minus_1;
			public AomArray<int> point_y_scaling;
			public AomArray<int> point_y_value;
			public int pos;
			public AomArray<int> primary_chromaticity_x;
			public AomArray<int> primary_chromaticity_y;
			public int primary_ref_frame;
			public int qThresh;
			public int qm_4x8_is_transpose_of_8x4;
			public int qm_8x8_is_symmetric;
			public int qm_bit_map;
			public int qm_chroma_info_present_flag;
			public int qm_copy_from_previous_plane;
			public int qm_index;
			public int qm_is_default_flag;
			public AomArray<int> qm_u;
			public int qm_uv_same_as_y;
			public AomArray<int> qm_v;
			public AomArray<int> qm_y;
			public int quant_delta;
			public int readFrameFilters;
			public int reduce_pb_aspect_ratio;
			public int reduced_ref_frame_mvs_mode;
			public int reduced_tx_part_set;
			public int reduced_tx_set;
			public int refDisp;
			public int refRatio;
			public AomArray<int> ref_frame_idx;
			public AomArray<int> ref_long_term_id;
			public int reference_select;
			public int refresh_frame_flags;
			public int reserved;
			public int restricted_prediction_switch;
			public AomArray<int> reuse_ccso;
			public int reuse_seg_info;
			public int reuse_tile_info;
			public int rst_ref_pic_idx;
			public AomArray<int> sbColStarts;
			public int sbNum;
			public AomArray<int> sbRowStarts;
			public int sbSize;
			public AomArray<int> sb_reuse_ccso;
			public int score;
			public int seconds_flag;
			public int seconds_value;
			public int sef_order_hint;
			public int segmentation_enabled;
			public int segmentation_temporal_update;
			public int segmentation_update_map;
			public int separate_uv_delta_q;
			public int seqSbSize;
			public int seq_allow_seg_info_change;
			public int seq_choose_integer_mv;
			public int seq_choose_screen_content_tools;
			public int seq_cropping_win_bottom_offset;
			public int seq_cropping_win_left_offset;
			public int seq_cropping_win_right_offset;
			public int seq_cropping_win_top_offset;
			public int seq_cropping_window_present_flag;
			public int seq_decoder_model_info_present_flag;
			public AomArray<int> seq_enabled_motion_modes;
			public int seq_force_integer_mv;
			public int seq_force_screen_content_tools;
			public int seq_frame_motion_modes_present_flag;
			public int seq_header_id;
			public int seq_header_id_in_frame_header;
			public int seq_initial_display_delay_minus_1;
			public int seq_initial_display_delay_present_flag;
			public int seq_lcr_id;
			public int seq_level_idx;
			public int seq_max_bvp_drl_bits_minus_1;
			public int seq_max_drl_bits_minus_1;
			public int seq_max_mlayer_cnt_minus_1;
			public int seq_profile_idc;
			public int seq_seg_info_present_flag;
			public int seq_tier;
			public int seq_tile_info_present_flag;
			public int signal_primary_ref_frame;
			public int single_picture_header_flag;
			public int skip_mode_present;
			public int slot;
			public int source_banding_present_flag;
			public int still_picture;
			public int subX;
			public int subY;
			public AomArray<int> sub_stream_max_level;
			public AomArray<int> sub_stream_max_profile;
			public AomArray<int> sub_stream_max_tier;
			public AomArray<int> sub_xlayer_id;
			public int subexp_bits;
			public int subexp_final_bits;
			public int subexp_more_bits;
			public int sz;
			public int t;
			public int target;
			public AomArray<int> temporal_pred_flag;
			public int tg_end;
			public int tg_start;
			public int their_ref;
			public int three_color_components_flag;
			public int tileCols;
			public int tileColsLog2;
			public int tileLog2;
			public int tileRows;
			public int tileRowsLog2;
			public int tile_size_bytes_minus_1;
			public int tile_size_minus_1;
			public int tile_start_and_end_present_flag;
			public int time_offset_length;
			public int time_offset_value;
			public int time_scale;
			public int tip_frame_mode;
			public int tip_global_wtd_index;
			public int tip_mv_col;
			public int tip_mv_col_sign;
			public int tip_mv_row;
			public int tip_mv_row_sign;
			public int tip_mv_zero;
			public int tip_regular;
			public int tip_sharp;
			public int tlayer_dependency_map;
			public int tlayer_dependency_present_flag;
			public int tmvp_sample_step_minus_1;
			public int tool_index;
			public int trailing_one_bit;
			public int trailing_zero_bit;
			public int txClass;
			public int txSz;
			public int tx_mode_select;
			public int uniformSbSize;
			public int uniformSpacing;
			public int uniform_tile_spacing_flag;
			public int unitCol;
			public int unitRow;
			public int use_128x128_superblock;
			public int use_256x256_superblock;
			public int use_alt_group;
			public int use_bank;
			public int use_bru;
			public int use_global_motion;
			public int use_qtr_precision_mv;
			public int use_ref_frame_mvs;
			public int user_data_payload_byte;
			public int using_qmatrix;
			public byte[] uuid_iso_iec_11578;
			public int uv_ac_delta_q_enabled;
			public int uv_dc_delta_q_enabled;
			public int v;
			public int varying_size_band_units_flag;
			public int vert_size_in_band_blocks_minus_1;
			public int white_point_chromaticity_x;
			public int white_point_chromaticity_y;
			public int width_in_sbs_minus_1;
			public int wiener_ns_length;
			public int wiener_ns_uv_sym;
			public int xAId;
			public int xId;
			public int xLId;
			public int xlayerId;
			public int y_dc_delta_q_enabled;
			public int zero_bit;
		}

		private ContextState SaveContext()
		{
			var state = new ContextState();
			state.ActiveNumRefFrames = this.ActiveNumRefFrames;
			state.AllowedFrames = this.AllowedFrames;
			state.AtlasHeight = this.AtlasHeight;
			state.AtlasSegmentIDToIndex = this.AtlasSegmentIDToIndex?.Clone();
			state.AtlasSegmentIndexToID = this.AtlasSegmentIndexToID?.Clone();
			state.AtlasWidth = this.AtlasWidth;
			state.BaseUVAcDeltaQ = this.BaseUVAcDeltaQ;
			state.BaseUVDcDeltaQ = this.BaseUVDcDeltaQ;
			state.BaseYDcDeltaQ = this.BaseYDcDeltaQ;
			state.BitDepth = this.BitDepth;
			state.BruTileActive = this.BruTileActive;
			state.BruTileActives = this.BruTileActives?.Clone();
			state.CcsoFilterOffset = this.CcsoFilterOffset?.Clone();
			state.CcsoLumaSizeLog2 = this.CcsoLumaSizeLog2;
			state.CdefDamping = this.CdefDamping;
			state.CdefOnSkipTxfm = this.CdefOnSkipTxfm;
			state.CdefStrengths = this.CdefStrengths;
			state.ClosestFuture = this.ClosestFuture;
			state.ClosestPast = this.ClosestPast;
			state.CodedLossless = this.CodedLossless;
			state.CountFrameHeaderForLevelConstraint = this.CountFrameHeaderForLevelConstraint;
			state.CropHeight = this.CropHeight;
			state.CropLeft = this.CropLeft;
			state.CropTop = this.CropTop;
			state.CropWidth = this.CropWidth;
			state.CurrentQIndex = this.CurrentQIndex;
			state.DeltaQUAc = this.DeltaQUAc;
			state.DeltaQUDc = this.DeltaQUDc;
			state.DeltaQVAc = this.DeltaQVAc;
			state.DeltaQVDc = this.DeltaQVDc;
			state.DeltaQYDc = this.DeltaQYDc;
			state.DerivedPrimaryRefFrame = this.DerivedPrimaryRefFrame;
			state.DfDeltaQ = this.DfDeltaQ?.Clone();
			state.DrlReorder = this.DrlReorder;
			state.EnableTipOutput = this.EnableTipOutput;
			state.FeatureData = this.FeatureData?.Clone();
			state.FeatureEnabled = this.FeatureEnabled?.Clone();
			state.FgmChromaIdc = this.FgmChromaIdc?.Clone();
			state.FgmMLayerId = this.FgmMLayerId?.Clone();
			state.FgmTLayerId = this.FgmTLayerId?.Clone();
			state.FilmGrainPresent = this.FilmGrainPresent?.Clone();
			state.FirstPictureInTU = this.FirstPictureInTU;
			state.FrameDistance = this.FrameDistance?.Clone();
			state.FrameHeight = this.FrameHeight;
			state.FrameIsIntra = this.FrameIsIntra;
			state.FrameLrWienerNs = this.FrameLrWienerNs?.Clone();
			state.FrameMvPrecision = this.FrameMvPrecision;
			state.FrameRestorationType = this.FrameRestorationType?.Clone();
			state.FrameSymbolCount = this.FrameSymbolCount;
			state.FrameType = this.FrameType;
			state.FrameWidth = this.FrameWidth;
			state.FurthestFuture = this.FurthestFuture;
			state.GdfBlkSize = this.GdfBlkSize;
			state.GdfPixScale = this.GdfPixScale;
			state.GmType = this.GmType?.Clone();
			state.HasBothRefs = this.HasBothRefs;
			state.HasLosslessSegment = this.HasLosslessSegment;
			state.IBCCoded = this.IBCCoded?.Clone();
			state.IsBridge = this.IsBridge;
			state.IsRegular = this.IsRegular;
			state.LastActiveSegId = this.LastActiveSegId;
			state.LcrMaxNumXLayerCount = this.LcrMaxNumXLayerCount;
			state.LcrXLayerID = this.LcrXLayerID?.Clone();
			state.LongTermId = this.LongTermId;
			state.LoopRestorationSize = this.LoopRestorationSize?.Clone();
			state.LosslessArray = this.LosslessArray?.Clone();
			state.LrWienerNs = this.LrWienerNs?.Clone();
			state.MLayerDependencyMap = this.MLayerDependencyMap?.Clone();
			state.MLayerPresenceMap = this.MLayerPresenceMap?.Clone();
			state.MaxPbAspectRatio = this.MaxPbAspectRatio;
			state.MaxQ = this.MaxQ;
			state.MaxSegments = this.MaxSegments;
			state.MfhFeatureData = this.MfhFeatureData?.Clone();
			state.MfhFeatureEnabled = this.MfhFeatureEnabled?.Clone();
			state.MfhMLayerId = this.MfhMLayerId?.Clone();
			state.MfhSeqHeaderId = this.MfhSeqHeaderId?.Clone();
			state.MfhTLayerId = this.MfhTLayerId?.Clone();
			state.MiColEnd = this.MiColEnd;
			state.MiColStart = this.MiColStart;
			state.MiColStarts = this.MiColStarts?.Clone();
			state.MiCols = this.MiCols;
			state.MiRowEnd = this.MiRowEnd;
			state.MiRowStart = this.MiRowStart;
			state.MiRowStarts = this.MiRowStarts?.Clone();
			state.MiRows = this.MiRows;
			state.Monochrome = this.Monochrome;
			state.MotionFieldChecked = this.MotionFieldChecked?.Clone();
			state.MotionFieldDepth = this.MotionFieldDepth?.Clone();
			state.MotionFieldStack = this.MotionFieldStack?.Clone();
			state.MotionFieldStackCount = this.MotionFieldStackCount;
			state.MotionFieldVisited = this.MotionFieldVisited?.Clone();
			state.MvPrecision = this.MvPrecision;
			state.NRanked = this.NRanked;
			state.NumFilterClasses = this.NumFilterClasses;
			state.NumFrameHeaderBits = this.NumFrameHeaderBits;
			state.NumFutureRefs = this.NumFutureRefs;
			state.NumPastRefs = this.NumPastRefs;
			state.NumPlanes = this.NumPlanes;
			state.NumRefFrames = this.NumRefFrames;
			state.NumRegionsInAtlas = this.NumRegionsInAtlas?.Clone();
			state.NumSameRefCompound = this.NumSameRefCompound;
			state.NumTiles = this.NumTiles;
			state.NumTotalRefs = this.NumTotalRefs;
			state.OlkEncountered = this.OlkEncountered;
			state.OlkRefresh = this.OlkRefresh?.Clone();
			state.OlkTUOrderHint = this.OlkTUOrderHint;
			state.OpsxLayerId = this.OpsxLayerId?.Clone();
			state.OrderHint = this.OrderHint;
			state.OrderHintBits = this.OrderHintBits;
			state.OrderHintLsbs = this.OrderHintLsbs;
			state.OrderHints = this.OrderHints?.Clone();
			state.OrigClosestFuture = this.OrigClosestFuture;
			state.OrigClosestPast = this.OrigClosestPast;
			state.PrevGmParams = this.PrevGmParams?.Clone();
			state.PrevSegmentIds = this.PrevSegmentIds?.Clone();
			state.ProjStep = this.ProjStep;
			state.QmDataPresent = this.QmDataPresent?.Clone();
			state.QmMLayerId = this.QmMLayerId?.Clone();
			state.QmNumPlanes = this.QmNumPlanes?.Clone();
			state.QmProtected = this.QmProtected?.Clone();
			state.QmSeen = this.QmSeen?.Clone();
			state.QmTLayerId = this.QmTLayerId?.Clone();
			state.RefLrWienerNs = this.RefLrWienerNs?.Clone();
			state.RefOrderHint = this.RefOrderHint?.Clone();
			state.RefValid = this.RefValid?.Clone();
			state.RemainingLcrPayloadBits = this.RemainingLcrPayloadBits;
			state.SbSize = this.SbSize;
			state.ScoresBaseQIdx = this.ScoresBaseQIdx?.Clone();
			state.ScoresDistance = this.ScoresDistance?.Clone();
			state.ScoresIndex = this.ScoresIndex?.Clone();
			state.ScoresLayer = this.ScoresLayer?.Clone();
			state.ScoresOrderHint = this.ScoresOrderHint?.Clone();
			state.ScoresScore = this.ScoresScore?.Clone();
			state.SeenFrameHeader = this.SeenFrameHeader;
			state.SegIdPreSkip = this.SegIdPreSkip;
			state.SegQMLevel = this.SegQMLevel?.Clone();
			state.SegmentIds = this.SegmentIds?.Clone();
			state.SeqFeatureData = this.SeqFeatureData?.Clone();
			state.SeqFeatureEnabled = this.SeqFeatureEnabled?.Clone();
			state.SeqMaxMlayerCnt = this.SeqMaxMlayerCnt;
			state.SeqSbColStarts = this.SeqSbColStarts?.Clone();
			state.SeqSbCols = this.SeqSbCols;
			state.SeqSbRowStarts = this.SeqSbRowStarts?.Clone();
			state.SeqSbRows = this.SeqSbRows;
			state.SeqTileCols = this.SeqTileCols;
			state.SeqTileColsLog2 = this.SeqTileColsLog2;
			state.SeqTileRows = this.SeqTileRows;
			state.SeqTileRowsLog2 = this.SeqTileRowsLog2;
			state.SeqUniformTileSpacingFlag = this.SeqUniformTileSpacingFlag;
			state.ShowExistingFrame = this.ShowExistingFrame;
			state.SkipModeFrame = this.SkipModeFrame?.Clone();
			state.SkipSegFrame = this.SkipSegFrame;
			state.SubclassLookup = this.SubclassLookup?.Clone();
			state.SubsamplingX = this.SubsamplingX;
			state.SubsamplingY = this.SubsamplingY;
			state.TLayerDependencyMap = this.TLayerDependencyMap?.Clone();
			state.TileCols = this.TileCols;
			state.TileColsLog2 = this.TileColsLog2;
			state.TileNum = this.TileNum;
			state.TileRows = this.TileRows;
			state.TileRowsLog2 = this.TileRowsLog2;
			state.TileSizeBytes = this.TileSizeBytes;
			state.TipFrameMode = this.TipFrameMode;
			state.TipGlobalMv = this.TipGlobalMv?.Clone();
			state.TipInterpFilter = this.TipInterpFilter;
			state.TxMode = this.TxMode;
			state.UsePerBlockMvPrecision = this.UsePerBlockMvPrecision;
			state.UserQm = this.UserQm?.Clone();
			state.UsesLr = this.UsesLr;
			state.WienerNsBankSize = this.WienerNsBankSize?.Clone();
			state.WienerNsPtr = this.WienerNsPtr?.Clone();
			state.XCount = this.XCount?.Clone();
			state.a = this.a;
			state.allow_bawp = this.allow_bawp;
			state.allow_df_sub_pu = this.allow_df_sub_pu;
			state.allow_frame_max_bvp_drl_bits = this.allow_frame_max_bvp_drl_bits;
			state.allow_frame_max_drl_bits = this.allow_frame_max_drl_bits;
			state.allow_global_intrabc = this.allow_global_intrabc;
			state.allow_high_precision_mv = this.allow_high_precision_mv;
			state.allow_intrabc = this.allow_intrabc;
			state.allow_local_intrabc = this.allow_local_intrabc;
			state.allow_parity_hiding = this.allow_parity_hiding;
			state.allow_screen_content_tools = this.allow_screen_content_tools;
			state.allow_tcq = this.allow_tcq;
			state.allow_tile_info_change = this.allow_tile_info_change;
			state.allow_tip_hole_fill = this.allow_tip_hole_fill;
			state.allow_warpmv_mode = this.allow_warpmv_mode;
			state.apply_deblocking_filter = this.apply_deblocking_filter?.Clone();
			state.apply_deblocking_filter_tip = this.apply_deblocking_filter_tip;
			state.apply_grain = this.apply_grain;
			state.ar_coeff_lag = this.ar_coeff_lag;
			state.ar_coeff_shift_minus_6 = this.ar_coeff_shift_minus_6;
			state.ar_coeffs_cb = this.ar_coeffs_cb?.Clone();
			state.ar_coeffs_cr = this.ar_coeffs_cr?.Clone();
			state.ar_coeffs_y = this.ar_coeffs_y?.Clone();
			state.atlas_segment_id = this.atlas_segment_id?.Clone();
			state.ats_atlas_segment_id = this.ats_atlas_segment_id?.Clone();
			state.ats_atlas_segment_mode_idc = this.ats_atlas_segment_mode_idc?.Clone();
			state.ats_bottom_right_region_column_off = this.ats_bottom_right_region_column_off?.Clone();
			state.ats_bottom_right_region_row_off = this.ats_bottom_right_region_row_off?.Clone();
			state.ats_column_width_minus_1 = this.ats_column_width_minus_1?.Clone();
			state.ats_height = this.ats_height?.Clone();
			state.ats_input_stream_id = this.ats_input_stream_id?.Clone();
			state.ats_msi_alpha_segment_flag = this.ats_msi_alpha_segment_flag?.Clone();
			state.ats_msi_alpha_segments_present_flag = this.ats_msi_alpha_segments_present_flag?.Clone();
			state.ats_msi_background_blue_value = this.ats_msi_background_blue_value?.Clone();
			state.ats_msi_background_green_value = this.ats_msi_background_green_value?.Clone();
			state.ats_msi_background_info_present_flag = this.ats_msi_background_info_present_flag?.Clone();
			state.ats_msi_background_red_value = this.ats_msi_background_red_value?.Clone();
			state.ats_msi_height = this.ats_msi_height?.Clone();
			state.ats_msi_input_stream_id = this.ats_msi_input_stream_id?.Clone();
			state.ats_msi_num_atlas_segments_minus_1 = this.ats_msi_num_atlas_segments_minus_1?.Clone();
			state.ats_msi_segment_height = this.ats_msi_segment_height?.Clone();
			state.ats_msi_segment_top_left_pos_x = this.ats_msi_segment_top_left_pos_x?.Clone();
			state.ats_msi_segment_top_left_pos_y = this.ats_msi_segment_top_left_pos_y?.Clone();
			state.ats_msi_segment_width = this.ats_msi_segment_width?.Clone();
			state.ats_msi_width = this.ats_msi_width?.Clone();
			state.ats_nominal_height_minus_1 = this.ats_nominal_height_minus_1?.Clone();
			state.ats_nominal_width_minus_1 = this.ats_nominal_width_minus_1?.Clone();
			state.ats_num_atlas_segments_minus_1 = this.ats_num_atlas_segments_minus_1?.Clone();
			state.ats_num_region_columns_minus_1 = this.ats_num_region_columns_minus_1?.Clone();
			state.ats_num_region_rows_minus_1 = this.ats_num_region_rows_minus_1?.Clone();
			state.ats_region_height_minus_1 = this.ats_region_height_minus_1?.Clone();
			state.ats_region_width_minus_1 = this.ats_region_width_minus_1?.Clone();
			state.ats_row_height_minus_1 = this.ats_row_height_minus_1?.Clone();
			state.ats_segment_height = this.ats_segment_height?.Clone();
			state.ats_segment_top_left_pos_x = this.ats_segment_top_left_pos_x?.Clone();
			state.ats_segment_top_left_pos_y = this.ats_segment_top_left_pos_y?.Clone();
			state.ats_segment_width = this.ats_segment_width?.Clone();
			state.ats_signaled_atlas_segment_ids_flag = this.ats_signaled_atlas_segment_ids_flag?.Clone();
			state.ats_single_region_per_atlas_segment_flag = this.ats_single_region_per_atlas_segment_flag?.Clone();
			state.ats_stream_id_present = this.ats_stream_id_present?.Clone();
			state.ats_top_left_region_column = this.ats_top_left_region_column?.Clone();
			state.ats_top_left_region_row = this.ats_top_left_region_row?.Clone();
			state.ats_uniform_spacing_flag = this.ats_uniform_spacing_flag?.Clone();
			state.ats_width = this.ats_width?.Clone();
			state.avg_cdf_type = this.avg_cdf_type;
			state.b = this.b;
			state.band_block_in_luma_samples = this.band_block_in_luma_samples;
			state.band_units_information_present_flag = this.band_units_information_present_flag;
			state.banding_hints_flag = this.banding_hints_flag;
			state.banding_in_band_unit_present_flag = this.banding_in_band_unit_present_flag;
			state.banding_in_component_present_flag = this.banding_in_component_present_flag;
			state.baseDistance = this.baseDistance;
			state.baseParams = this.baseParams?.Clone();
			state.base_q_idx = this.base_q_idx;
			state.base_qindex = this.base_qindex;
			state.base_uv_ac_delta_q = this.base_uv_ac_delta_q;
			state.base_uv_dc_delta_q = this.base_uv_dc_delta_q;
			state.base_y_dc_delta_q = this.base_y_dc_delta_q;
			state.bestDisp = this.bestDisp;
			state.bestRatio = this.bestRatio;
			state.bit_depth_idc = this.bit_depth_idc;
			state.bits_per_ar_coeff_cb_minus_5 = this.bits_per_ar_coeff_cb_minus_5;
			state.bits_per_ar_coeff_cr_minus_5 = this.bits_per_ar_coeff_cr_minus_5;
			state.bits_per_ar_coeff_y_minus_5 = this.bits_per_ar_coeff_y_minus_5;
			state.blkSize = this.blkSize;
			state.br_decoder_model_present_op_flag = this.br_decoder_model_present_op_flag?.Clone();
			state.br_ops_cnt = this.br_ops_cnt?.Clone();
			state.br_ops_dependent_flag = this.br_ops_dependent_flag;
			state.br_ops_id = this.br_ops_id;
			state.br_time = this.br_time;
			state.br_time_op = this.br_time_op?.Clone();
			state.bridge_frame_height_minus_1 = this.bridge_frame_height_minus_1;
			state.bridge_frame_overwrite_flag = this.bridge_frame_overwrite_flag;
			state.bridge_frame_ref_idx = this.bridge_frame_ref_idx;
			state.bridge_frame_width_minus_1 = this.bridge_frame_width_minus_1;
			state.bru_inactive = this.bru_inactive;
			state.bru_ref = this.bru_ref;
			state.bru_tile_active = this.bru_tile_active;
			state.cb_luma_mult = this.cb_luma_mult;
			state.cb_mult = this.cb_mult;
			state.cb_offset = this.cb_offset;
			state.ccso_bo_only = this.ccso_bo_only?.Clone();
			state.ccso_edge_clf = this.ccso_edge_clf?.Clone();
			state.ccso_ext_filter = this.ccso_ext_filter?.Clone();
			state.ccso_frame_flag = this.ccso_frame_flag;
			state.ccso_max_band_log2 = this.ccso_max_band_log2?.Clone();
			state.ccso_offset_idx = this.ccso_offset_idx;
			state.ccso_planes = this.ccso_planes?.Clone();
			state.ccso_quant_idx = this.ccso_quant_idx?.Clone();
			state.ccso_ref_idx = this.ccso_ref_idx?.Clone();
			state.ccso_scale_idx = this.ccso_scale_idx?.Clone();
			state.ccso_unit_matches_sb_size = this.ccso_unit_matches_sb_size;
			state.cdef_damping_minus_3 = this.cdef_damping_minus_3;
			state.cdef_frame_enable = this.cdef_frame_enable;
			state.cdef_on_skip_txfm_always_on = this.cdef_on_skip_txfm_always_on;
			state.cdef_on_skip_txfm_disabled = this.cdef_on_skip_txfm_disabled;
			state.cdef_on_skip_txfm_frame_enable = this.cdef_on_skip_txfm_frame_enable;
			state.cdef_strengths_minus_1 = this.cdef_strengths_minus_1;
			state.cdef_uv_pri_strength = this.cdef_uv_pri_strength?.Clone();
			state.cdef_uv_pri_zero = this.cdef_uv_pri_zero;
			state.cdef_uv_sec_strength = this.cdef_uv_sec_strength?.Clone();
			state.cdef_y_pri_strength = this.cdef_y_pri_strength?.Clone();
			state.cdef_y_pri_zero = this.cdef_y_pri_zero;
			state.cdef_y_sec_strength = this.cdef_y_sec_strength?.Clone();
			state.cfl_ds_filter_index = this.cfl_ds_filter_index;
			state.change_bvp_drl = this.change_bvp_drl;
			state.change_drl = this.change_drl;
			state.checkRes = this.checkRes;
			state.choose_tcq_per_frame = this.choose_tcq_per_frame;
			state.chroma_format_idc = this.chroma_format_idc;
			state.chroma_scaling_from_luma = this.chroma_scaling_from_luma;
			state.ci_aspect_ratio_idc = this.ci_aspect_ratio_idc;
			state.ci_aspect_ratio_info_present_flag = this.ci_aspect_ratio_info_present_flag;
			state.ci_chroma_sample_position_bottom = this.ci_chroma_sample_position_bottom;
			state.ci_chroma_sample_position_present_flag = this.ci_chroma_sample_position_present_flag;
			state.ci_chroma_sample_position_top = this.ci_chroma_sample_position_top;
			state.ci_color_description_idc = this.ci_color_description_idc;
			state.ci_color_description_present_flag = this.ci_color_description_present_flag;
			state.ci_color_primaries = this.ci_color_primaries;
			state.ci_full_range_flag = this.ci_full_range_flag;
			state.ci_matrix_coefficients = this.ci_matrix_coefficients;
			state.ci_reserved_2bit = this.ci_reserved_2bit;
			state.ci_sar_height = this.ci_sar_height;
			state.ci_sar_width = this.ci_sar_width;
			state.ci_scan_type_idc = this.ci_scan_type_idc;
			state.ci_timing_info_present_flag = this.ci_timing_info_present_flag;
			state.ci_transfer_characteristics = this.ci_transfer_characteristics;
			state.clip_to_restricted_range = this.clip_to_restricted_range;
			state.cnt_dropped_flag = this.cnt_dropped_flag;
			state.coding_banding_present_flag = this.coding_banding_present_flag;
			state.constrain_drl_reorder = this.constrain_drl_reorder;
			state.context_update_tile_id = this.context_update_tile_id;
			state.counting_type = this.counting_type;
			state.counts = this.counts?.Clone();
			state.cr_luma_mult = this.cr_luma_mult;
			state.cr_mult = this.cr_mult;
			state.cr_offset = this.cr_offset;
			state.cur_mfh_id = this.cur_mfh_id;
			state.d = this.d;
			state.decoder_buffer_delay = this.decoder_buffer_delay;
			state.decoder_model_info_present_flag = this.decoder_model_info_present_flag;
			state.delta_coded = this.delta_coded;
			state.delta_q = this.delta_q;
			state.delta_q_present = this.delta_q_present;
			state.delta_q_res = this.delta_q_res;
			state.derive_sef_order_hint = this.derive_sef_order_hint;
			state.df_delta_q = this.df_delta_q?.Clone();
			state.df_delta_q_present = this.df_delta_q_present?.Clone();
			state.df_par_bits_minus_2 = this.df_par_bits_minus_2;
			state.diff_uv_delta = this.diff_uv_delta;
			state.disable_cdf_update = this.disable_cdf_update;
			state.disable_cross_frame_cdf_init = this.disable_cross_frame_cdf_init;
			state.disable_drl_reorder = this.disable_drl_reorder;
			state.disable_loopfilters_across_tiles = this.disable_loopfilters_across_tiles;
			state.disable_tip_output = this.disable_tip_output;
			state.discontinuity_flag = this.discontinuity_flag;
			state.dist = this.dist;
			state.enable_adaptive_mvd = this.enable_adaptive_mvd;
			state.enable_avg_cdf = this.enable_avg_cdf;
			state.enable_bawp = this.enable_bawp;
			state.enable_bru = this.enable_bru;
			state.enable_ccso = this.enable_ccso;
			state.enable_cctx = this.enable_cctx;
			state.enable_cdef = this.enable_cdef;
			state.enable_cfl_intra = this.enable_cfl_intra;
			state.enable_chroma_dctonly = this.enable_chroma_dctonly;
			state.enable_cwp = this.enable_cwp;
			state.enable_df_sub_pu = this.enable_df_sub_pu;
			state.enable_dip = this.enable_dip;
			state.enable_ext_partitions = this.enable_ext_partitions;
			state.enable_ext_seg = this.enable_ext_seg;
			state.enable_extended_sdp = this.enable_extended_sdp;
			state.enable_flex_mvres = this.enable_flex_mvres;
			state.enable_fsc = this.enable_fsc;
			state.enable_gdf = this.enable_gdf;
			state.enable_global_motion = this.enable_global_motion;
			state.enable_ibp = this.enable_ibp;
			state.enable_idtx_intra = this.enable_idtx_intra;
			state.enable_imp_msk_bld = this.enable_imp_msk_bld;
			state.enable_inter_ddt = this.enable_inter_ddt;
			state.enable_inter_ist = this.enable_inter_ist;
			state.enable_intra_edge_filter = this.enable_intra_edge_filter;
			state.enable_intra_ist = this.enable_intra_ist;
			state.enable_masked_compound = this.enable_masked_compound;
			state.enable_mhccp = this.enable_mhccp;
			state.enable_mrls = this.enable_mrls;
			state.enable_mv_traj = this.enable_mv_traj;
			state.enable_mvd_sign_derive = this.enable_mvd_sign_derive;
			state.enable_opfl_refine = this.enable_opfl_refine;
			state.enable_parity_hiding = this.enable_parity_hiding;
			state.enable_ref_frame_mvs = this.enable_ref_frame_mvs;
			state.enable_refinemv = this.enable_refinemv;
			state.enable_refmvbank = this.enable_refmvbank;
			state.enable_restoration = this.enable_restoration;
			state.enable_sdp = this.enable_sdp;
			state.enable_short_refresh_frame_flags = this.enable_short_refresh_frame_flags;
			state.enable_six_param_warp_delta = this.enable_six_param_warp_delta;
			state.enable_tcq = this.enable_tcq;
			state.enable_tip = this.enable_tip;
			state.enable_tip_explicit_qp = this.enable_tip_explicit_qp;
			state.enable_tip_hole_fill = this.enable_tip_hole_fill;
			state.enable_tip_refinemv = this.enable_tip_refinemv;
			state.enable_uneven_4way_partitions = this.enable_uneven_4way_partitions;
			state.encoder_buffer_delay = this.encoder_buffer_delay;
			state.equal_ac_dc_q = this.equal_ac_dc_q;
			state.equal_picture_interval = this.equal_picture_interval;
			state.explicit_num_ref_frames = this.explicit_num_ref_frames;
			state.explicit_ref_frame_map = this.explicit_ref_frame_map;
			state.feature = this.feature;
			state.feature_enabled = this.feature_enabled;
			state.feature_value = this.feature_value;
			state.fg_mc_identity = this.fg_mc_identity;
			state.fgm_chroma_idc = this.fgm_chroma_idc;
			state.fgm_id = this.fgm_id;
			state.fgm_update_flags = this.fgm_update_flags;
			state.film_grain_block_size = this.film_grain_block_size;
			state.film_grain_params_present = this.film_grain_params_present;
			state.force_integer_mv = this.force_integer_mv;
			state.found_ref = this.found_ref;
			state.frameHeight = this.frameHeight;
			state.frameWidth = this.frameWidth;
			state.frame_enabled_motion_modes = this.frame_enabled_motion_modes?.Clone();
			state.frame_explicit_ref_frame_map = this.frame_explicit_ref_frame_map;
			state.frame_filters_on = this.frame_filters_on?.Clone();
			state.frame_hash = ((byte[])this.frame_hash?.Clone());
			state.frame_header_present_flag = this.frame_header_present_flag;
			state.frame_height_bits_minus_1 = this.frame_height_bits_minus_1;
			state.frame_height_minus_1 = this.frame_height_minus_1;
			state.frame_is_inter = this.frame_is_inter;
			state.frame_presentation_time = this.frame_presentation_time;
			state.frame_size_override_flag = this.frame_size_override_flag;
			state.frame_to_refresh = this.frame_to_refresh;
			state.frame_to_show_map_idx = this.frame_to_show_map_idx;
			state.frame_width_bits_minus_1 = this.frame_width_bits_minus_1;
			state.frame_width_minus_1 = this.frame_width_minus_1;
			state.full_timestamp_flag = this.full_timestamp_flag;
			state.gdf_frame_enable = this.gdf_frame_enable;
			state.gdf_per_block = this.gdf_per_block;
			state.gdf_pic_qc_idx = this.gdf_pic_qc_idx;
			state.gdf_pic_scale_idx = this.gdf_pic_scale_idx;
			state.gdf_unit_matches_sb_size = this.gdf_unit_matches_sb_size;
			state.gm_params = this.gm_params?.Clone();
			state.grain_scale_shift = this.grain_scale_shift;
			state.grain_scaling_minus_8 = this.grain_scaling_minus_8;
			state.grain_seed = this.grain_seed;
			state.group_bit = this.group_bit;
			state.has_grain = this.has_grain;
			state.has_refresh_frame_flags = this.has_refresh_frame_flags;
			state.hash_type = this.hash_type;
			state.header_bit = this.header_bit?.Clone();
			state.height_in_sbs_minus_1 = this.height_in_sbs_minus_1;
			state.high = this.high;
			state.horz_size_in_band_blocks_minus_1 = this.horz_size_in_band_blocks_minus_1;
			state.hours_flag = this.hours_flag;
			state.hours_value = this.hours_value;
			state.icc_profile_data_payload_bytes = this.icc_profile_data_payload_bytes;
			state.idx = this.idx;
			state.immediate_output_frame = this.immediate_output_frame;
			state.implicit_output_frame = this.implicit_output_frame;
			state.increment_tile_cols_log2 = this.increment_tile_cols_log2;
			state.increment_tile_rows_log2 = this.increment_tile_rows_log2;
			state.interpolation_filter = this.interpolation_filter;
			state.isBridge = this.isBridge;
			state.isFirst = this.isFirst;
			state.isGlobal = this.isGlobal;
			state.is_filter_switchable = this.is_filter_switchable;
			state.is_first_tile_group = this.is_first_tile_group;
			state.is_global = this.is_global;
			state.is_monochrome = this.is_monochrome;
			state.is_rot_zoom = this.is_rot_zoom;
			state.itu_t_t35_country_code = this.itu_t_t35_country_code;
			state.itu_t_t35_country_code_extension_byte = this.itu_t_t35_country_code_extension_byte;
			state.itu_t_t35_payload_bytes = this.itu_t_t35_payload_bytes;
			state.layer_color_description_idc = this.layer_color_description_idc?.Clone();
			state.layer_color_primaries = this.layer_color_primaries?.Clone();
			state.layer_full_range_flag = this.layer_full_range_flag?.Clone();
			state.layer_matrix_coefficients = this.layer_matrix_coefficients?.Clone();
			state.layer_transfer_characteristics = this.layer_transfer_characteristics?.Clone();
			state.lcr_aggregate_info_present_flag = this.lcr_aggregate_info_present_flag;
			state.lcr_aggregate_level_idx = this.lcr_aggregate_level_idx;
			state.lcr_auxiliary_type = this.lcr_auxiliary_type?.Clone();
			state.lcr_bit_depth_idc = this.lcr_bit_depth_idc?.Clone();
			state.lcr_chroma_format_idc = this.lcr_chroma_format_idc?.Clone();
			state.lcr_config_idc = this.lcr_config_idc;
			state.lcr_cropping_win_bottom_offset = this.lcr_cropping_win_bottom_offset?.Clone();
			state.lcr_cropping_win_left_offset = this.lcr_cropping_win_left_offset?.Clone();
			state.lcr_cropping_win_right_offset = this.lcr_cropping_win_right_offset?.Clone();
			state.lcr_cropping_win_top_offset = this.lcr_cropping_win_top_offset?.Clone();
			state.lcr_cropping_window_present_flag = this.lcr_cropping_window_present_flag?.Clone();
			state.lcr_data_size = this.lcr_data_size?.Clone();
			state.lcr_dependent_layer_map = this.lcr_dependent_layer_map?.Clone();
			state.lcr_dependent_xlayers_flag = this.lcr_dependent_xlayers_flag;
			state.lcr_doh_constraint_flag = this.lcr_doh_constraint_flag;
			state.lcr_embedded_layer_info_present_flag = this.lcr_embedded_layer_info_present_flag?.Clone();
			state.lcr_enforce_tile_alignment_flag = this.lcr_enforce_tile_alignment_flag;
			state.lcr_format_info_present_flag = this.lcr_format_info_present_flag?.Clone();
			state.lcr_global_atlas_id = this.lcr_global_atlas_id;
			state.lcr_global_atlas_id_present_flag = this.lcr_global_atlas_id_present_flag;
			state.lcr_global_config_record_id = this.lcr_global_config_record_id;
			state.lcr_global_id = this.lcr_global_id?.Clone();
			state.lcr_global_payload_present_flag = this.lcr_global_payload_present_flag;
			state.lcr_global_purpose_id = this.lcr_global_purpose_id;
			state.lcr_global_reserved_zero_3bits = this.lcr_global_reserved_zero_3bits;
			state.lcr_global_reserved_zero_5bits = this.lcr_global_reserved_zero_5bits;
			state.lcr_layer_atlas_segment_id = this.lcr_layer_atlas_segment_id?.Clone();
			state.lcr_layer_type = this.lcr_layer_type?.Clone();
			state.lcr_local_atlas_id = this.lcr_local_atlas_id?.Clone();
			state.lcr_local_atlas_id_present_flag = this.lcr_local_atlas_id_present_flag?.Clone();
			state.lcr_local_id = this.lcr_local_id?.Clone();
			state.lcr_local_reserved_zero_3bits = this.lcr_local_reserved_zero_3bits?.Clone();
			state.lcr_local_reserved_zero_5bits = this.lcr_local_reserved_zero_5bits?.Clone();
			state.lcr_max_expected_height = this.lcr_max_expected_height?.Clone();
			state.lcr_max_expected_width = this.lcr_max_expected_width?.Clone();
			state.lcr_max_interop = this.lcr_max_interop;
			state.lcr_max_level_idx = this.lcr_max_level_idx?.Clone();
			state.lcr_max_mlayer_count = this.lcr_max_mlayer_count?.Clone();
			state.lcr_max_pic_height = this.lcr_max_pic_height?.Clone();
			state.lcr_max_pic_width = this.lcr_max_pic_width?.Clone();
			state.lcr_max_tier_flag = this.lcr_max_tier_flag;
			state.lcr_mlayer_map = this.lcr_mlayer_map?.Clone();
			state.lcr_num_dependent_xlayer_map = this.lcr_num_dependent_xlayer_map?.Clone();
			state.lcr_priority_order = this.lcr_priority_order?.Clone();
			state.lcr_profile_tier_level_info_present_flag = this.lcr_profile_tier_level_info_present_flag?.Clone();
			state.lcr_remaining_payload_bit = this.lcr_remaining_payload_bit;
			state.lcr_rendering_method = this.lcr_rendering_method?.Clone();
			state.lcr_rep_info_present_flag = this.lcr_rep_info_present_flag?.Clone();
			state.lcr_same_sh_max_resolution_flag = this.lcr_same_sh_max_resolution_flag?.Clone();
			state.lcr_seq_profile_idc = this.lcr_seq_profile_idc?.Clone();
			state.lcr_seq_profile_tier_level_info_present_flag = this.lcr_seq_profile_tier_level_info_present_flag;
			state.lcr_tier_flag = this.lcr_tier_flag?.Clone();
			state.lcr_tlayer_map = this.lcr_tlayer_map?.Clone();
			state.lcr_view_id = this.lcr_view_id?.Clone();
			state.lcr_view_type = this.lcr_view_type?.Clone();
			state.lcr_xlayer_atlas_segment_id = this.lcr_xlayer_atlas_segment_id?.Clone();
			state.lcr_xlayer_color_info_present_flag = this.lcr_xlayer_color_info_present_flag?.Clone();
			state.lcr_xlayer_map = this.lcr_xlayer_map;
			state.lcr_xlayer_priority_order = this.lcr_xlayer_priority_order?.Clone();
			state.lcr_xlayer_purpose_id = this.lcr_xlayer_purpose_id?.Clone();
			state.lcr_xlayer_purpose_present_flag = this.lcr_xlayer_purpose_present_flag?.Clone();
			state.lcr_xlayer_rendering_method = this.lcr_xlayer_rendering_method?.Clone();
			state.level = this.level;
			state.loadCdfs = this.loadCdfs;
			state.longTermId = this.longTermId;
			state.long_term_frame_id_bits = this.long_term_frame_id_bits;
			state.long_term_id_plus_1 = this.long_term_id_plus_1;
			state.low = this.low;
			state.low_delay_mode_flag = this.low_delay_mode_flag;
			state.lr_chroma_use_half_size = this.lr_chroma_use_half_size;
			state.lr_chroma_use_max_size = this.lr_chroma_use_max_size;
			state.lr_chroma_use_quarter_size = this.lr_chroma_use_quarter_size;
			state.lr_luma_use_half_size = this.lr_luma_use_half_size;
			state.lr_luma_use_max_size = this.lr_luma_use_max_size;
			state.lr_luma_use_quarter_size = this.lr_luma_use_quarter_size;
			state.lr_tools_disable = this.lr_tools_disable?.Clone();
			state.lr_tools_uv_present = this.lr_tools_uv_present;
			state.lsptli_reserved_2bits = this.lsptli_reserved_2bits;
			state.luminance_max = this.luminance_max;
			state.luminance_min = this.luminance_min;
			state.m = this.m;
			state.max_band_step_minus_1 = this.max_band_step_minus_1;
			state.max_band_width_minus_4 = this.max_band_width_minus_4;
			state.max_bvp_drl_bits_minus_1 = this.max_bvp_drl_bits_minus_1;
			state.max_cll = this.max_cll;
			state.max_drl_bits_minus_1 = this.max_drl_bits_minus_1;
			state.max_fall = this.max_fall;
			state.max_frame_height_minus_1 = this.max_frame_height_minus_1;
			state.max_frame_width_minus_1 = this.max_frame_width_minus_1;
			state.max_mlayer_id = this.max_mlayer_id;
			state.max_pb_aspect_ratio_log2_minus_1 = this.max_pb_aspect_ratio_log2_minus_1;
			state.max_tlayer_id = this.max_tlayer_id;
			state.merged_param = this.merged_param;
			state.metadataPayloadSize = this.metadataPayloadSize;
			state.metadata_application_id = this.metadata_application_id;
			state.metadata_is_suffix = this.metadata_is_suffix;
			state.metadata_necessity_idc = this.metadata_necessity_idc;
			state.metadata_type = this.metadata_type;
			state.metadata_unit_cnt_minus_1 = this.metadata_unit_cnt_minus_1;
			state.metadata_unit_remaining_bit = this.metadata_unit_remaining_bit;
			state.mfh_allow_seg_info_change = this.mfh_allow_seg_info_change?.Clone();
			state.mfh_apply_deblocking_filter = this.mfh_apply_deblocking_filter?.Clone();
			state.mfh_deblocking_filter_update = this.mfh_deblocking_filter_update?.Clone();
			state.mfh_ext_seg_flag = this.mfh_ext_seg_flag?.Clone();
			state.mfh_frame_height_bits_minus_1 = this.mfh_frame_height_bits_minus_1;
			state.mfh_frame_height_minus_1 = this.mfh_frame_height_minus_1?.Clone();
			state.mfh_frame_size_present_flag = this.mfh_frame_size_present_flag?.Clone();
			state.mfh_frame_width_bits_minus_1 = this.mfh_frame_width_bits_minus_1;
			state.mfh_frame_width_minus_1 = this.mfh_frame_width_minus_1?.Clone();
			state.mfh_id_minus_1 = this.mfh_id_minus_1;
			state.mfh_seg_info_present_flag = this.mfh_seg_info_present_flag?.Clone();
			state.mfh_seq_header_id = this.mfh_seq_header_id;
			state.minutes_flag = this.minutes_flag;
			state.minutes_value = this.minutes_value;
			state.mis = this.mis;
			state.mlayer_dependency_map = this.mlayer_dependency_map;
			state.mlayer_dependency_present_flag = this.mlayer_dependency_present_flag;
			state.monochrome = this.monochrome;
			state.monotonic_output_order_flag = this.monotonic_output_order_flag;
			state.mps_duplicate_flag = this.mps_duplicate_flag;
			state.mps_pic_struct_type = this.mps_pic_struct_type;
			state.mps_source_scan_type_idc = this.mps_source_scan_type_idc;
			state.muh_cancel_flag = this.muh_cancel_flag;
			state.muh_header_extension_byte = this.muh_header_extension_byte;
			state.muh_header_size = this.muh_header_size;
			state.muh_layer_idc = this.muh_layer_idc;
			state.muh_mlayer_map = this.muh_mlayer_map;
			state.muh_payload_size = this.muh_payload_size;
			state.muh_persistence_idc = this.muh_persistence_idc;
			state.muh_priority = this.muh_priority;
			state.muh_reserved_zero_2bits = this.muh_reserved_zero_2bits;
			state.muh_xlayer_map = this.muh_xlayer_map;
			state.multi_tlayer_dependency_map_present_flag = this.multi_tlayer_dependency_map_present_flag;
			state.multistream_doh_constraint_flag = this.multistream_doh_constraint_flag;
			state.multistream_even_allocation_flag = this.multistream_even_allocation_flag;
			state.multistream_large_picture_idc = this.multistream_large_picture_idc;
			state.multistream_level_idx = this.multistream_level_idx;
			state.multistream_profile_idc = this.multistream_profile_idc;
			state.multistream_tier = this.multistream_tier;
			state.mx = this.mx;
			state.n = this.n;
			state.n_frames = this.n_frames;
			state.nbBits = this.nbBits;
			state.numSegments = this.numSegments;
			state.numSyms = this.numSyms;
			state.num_band_units_cols_minus_1 = this.num_band_units_cols_minus_1;
			state.num_band_units_rows_minus_1 = this.num_band_units_rows_minus_1;
			state.num_cb_points = this.num_cb_points;
			state.num_cr_points = this.num_cr_points;
			state.num_filter_classes_idx = this.num_filter_classes_idx;
			state.num_key_ref_frames = this.num_key_ref_frames;
			state.num_ref_frames_minus_1 = this.num_ref_frames_minus_1;
			state.num_same_ref_compound = this.num_same_ref_compound;
			state.num_streams_minus_2 = this.num_streams_minus_2;
			state.num_ticks_per_picture_minus_1 = this.num_ticks_per_picture_minus_1;
			state.num_total_refs = this.num_total_refs;
			state.num_units_in_decoding_tick = this.num_units_in_decoding_tick;
			state.num_units_in_display_tick = this.num_units_in_display_tick;
			state.num_y_points = this.num_y_points;
			state.obuPayloadSize = this.obuPayloadSize;
			state.obuXLId = this.obuXLId;
			state.obu_extension_data_bit = this.obu_extension_data_bit;
			state.obu_extension_flag = this.obu_extension_flag;
			state.obu_header_extension_flag = this.obu_header_extension_flag;
			state.obu_mlayer_id = this.obu_mlayer_id;
			state.obu_padding_byte = this.obu_padding_byte;
			state.obu_tlayer_id = this.obu_tlayer_id;
			state.obu_type = this.obu_type;
			state.obu_xlayer_id = this.obu_xlayer_id;
			state.onlyShowable = this.onlyShowable;
			state.opIndex = this.opIndex;
			state.opfl_refine_all = this.opfl_refine_all;
			state.opfl_refine_type = this.opfl_refine_type;
			state.opsID = this.opsID;
			state.ops_aggregate_level_idx = this.ops_aggregate_level_idx?.Clone();
			state.ops_cnt = this.ops_cnt?.Clone();
			state.ops_color_description_idc = this.ops_color_description_idc?.Clone();
			state.ops_color_info_present_flag = this.ops_color_info_present_flag?.Clone();
			state.ops_color_primaries = this.ops_color_primaries?.Clone();
			state.ops_config_idc = this.ops_config_idc?.Clone();
			state.ops_data_size = this.ops_data_size?.Clone();
			state.ops_decoder_buffer_delay = this.ops_decoder_buffer_delay?.Clone();
			state.ops_decoder_model_info_for_this_op_present_flag = this.ops_decoder_model_info_for_this_op_present_flag?.Clone();
			state.ops_embedded_op_index = this.ops_embedded_op_index?.Clone();
			state.ops_embedded_ops_id = this.ops_embedded_ops_id?.Clone();
			state.ops_encoder_buffer_delay = this.ops_encoder_buffer_delay?.Clone();
			state.ops_full_range_flag = this.ops_full_range_flag?.Clone();
			state.ops_id = this.ops_id?.Clone();
			state.ops_initial_display_delay_minus_1 = this.ops_initial_display_delay_minus_1?.Clone();
			state.ops_initial_display_delay_present_flag = this.ops_initial_display_delay_present_flag?.Clone();
			state.ops_intent = this.ops_intent?.Clone();
			state.ops_intent_present_flag = this.ops_intent_present_flag?.Clone();
			state.ops_level_idx = this.ops_level_idx?.Clone();
			state.ops_low_delay_mode_flag = this.ops_low_delay_mode_flag?.Clone();
			state.ops_matrix_coefficients = this.ops_matrix_coefficients?.Clone();
			state.ops_max_interop = this.ops_max_interop?.Clone();
			state.ops_max_tier_flag = this.ops_max_tier_flag?.Clone();
			state.ops_mlayer_count = this.ops_mlayer_count?.Clone();
			state.ops_mlayer_explicit_info_flag = this.ops_mlayer_explicit_info_flag?.Clone();
			state.ops_mlayer_info_idc = this.ops_mlayer_info_idc?.Clone();
			state.ops_mlayer_map = this.ops_mlayer_map?.Clone();
			state.ops_op_intent = this.ops_op_intent?.Clone();
			state.ops_priority = this.ops_priority?.Clone();
			state.ops_ptl_present_flag = this.ops_ptl_present_flag?.Clone();
			state.ops_ptl_reserved_2bits = this.ops_ptl_reserved_2bits;
			state.ops_reserved_2bits = this.ops_reserved_2bits;
			state.ops_reset_flag = this.ops_reset_flag?.Clone();
			state.ops_seq_profile_idc = this.ops_seq_profile_idc?.Clone();
			state.ops_tier_flag = this.ops_tier_flag?.Clone();
			state.ops_tlayer_map = this.ops_tlayer_map?.Clone();
			state.ops_transfer_characteristics = this.ops_transfer_characteristics?.Clone();
			state.ops_xlayer_map = this.ops_xlayer_map?.Clone();
			state.order_hint = this.order_hint;
			state.order_hint_bits_minus_1 = this.order_hint_bits_minus_1;
			state.our_ref = this.our_ref;
			state.overlap_flag = this.overlap_flag;
			state.per_plane = this.per_plane;
			state.pic_qm_num_minus_1 = this.pic_qm_num_minus_1;
			state.plane = this.plane;
			state.plane_hash = this.plane_hash?.Clone();
			state.point_cb_scaling = this.point_cb_scaling?.Clone();
			state.point_cb_value = this.point_cb_value?.Clone();
			state.point_cr_scaling = this.point_cr_scaling?.Clone();
			state.point_cr_value = this.point_cr_value?.Clone();
			state.point_scaling_bits_minus_5 = this.point_scaling_bits_minus_5;
			state.point_value_increment_bits_minus_1 = this.point_value_increment_bits_minus_1;
			state.point_y_scaling = this.point_y_scaling?.Clone();
			state.point_y_value = this.point_y_value?.Clone();
			state.pos = this.pos;
			state.primary_chromaticity_x = this.primary_chromaticity_x?.Clone();
			state.primary_chromaticity_y = this.primary_chromaticity_y?.Clone();
			state.primary_ref_frame = this.primary_ref_frame;
			state.qThresh = this.qThresh;
			state.qm_4x8_is_transpose_of_8x4 = this.qm_4x8_is_transpose_of_8x4;
			state.qm_8x8_is_symmetric = this.qm_8x8_is_symmetric;
			state.qm_bit_map = this.qm_bit_map;
			state.qm_chroma_info_present_flag = this.qm_chroma_info_present_flag;
			state.qm_copy_from_previous_plane = this.qm_copy_from_previous_plane;
			state.qm_index = this.qm_index;
			state.qm_is_default_flag = this.qm_is_default_flag;
			state.qm_u = this.qm_u?.Clone();
			state.qm_uv_same_as_y = this.qm_uv_same_as_y;
			state.qm_v = this.qm_v?.Clone();
			state.qm_y = this.qm_y?.Clone();
			state.quant_delta = this.quant_delta;
			state.readFrameFilters = this.readFrameFilters;
			state.reduce_pb_aspect_ratio = this.reduce_pb_aspect_ratio;
			state.reduced_ref_frame_mvs_mode = this.reduced_ref_frame_mvs_mode;
			state.reduced_tx_part_set = this.reduced_tx_part_set;
			state.reduced_tx_set = this.reduced_tx_set;
			state.refDisp = this.refDisp;
			state.refRatio = this.refRatio;
			state.ref_frame_idx = this.ref_frame_idx?.Clone();
			state.ref_long_term_id = this.ref_long_term_id?.Clone();
			state.reference_select = this.reference_select;
			state.refresh_frame_flags = this.refresh_frame_flags;
			state.reserved = this.reserved;
			state.restricted_prediction_switch = this.restricted_prediction_switch;
			state.reuse_ccso = this.reuse_ccso?.Clone();
			state.reuse_seg_info = this.reuse_seg_info;
			state.reuse_tile_info = this.reuse_tile_info;
			state.rst_ref_pic_idx = this.rst_ref_pic_idx;
			state.sbColStarts = this.sbColStarts?.Clone();
			state.sbNum = this.sbNum;
			state.sbRowStarts = this.sbRowStarts?.Clone();
			state.sbSize = this.sbSize;
			state.sb_reuse_ccso = this.sb_reuse_ccso?.Clone();
			state.score = this.score;
			state.seconds_flag = this.seconds_flag;
			state.seconds_value = this.seconds_value;
			state.sef_order_hint = this.sef_order_hint;
			state.segmentation_enabled = this.segmentation_enabled;
			state.segmentation_temporal_update = this.segmentation_temporal_update;
			state.segmentation_update_map = this.segmentation_update_map;
			state.separate_uv_delta_q = this.separate_uv_delta_q;
			state.seqSbSize = this.seqSbSize;
			state.seq_allow_seg_info_change = this.seq_allow_seg_info_change;
			state.seq_choose_integer_mv = this.seq_choose_integer_mv;
			state.seq_choose_screen_content_tools = this.seq_choose_screen_content_tools;
			state.seq_cropping_win_bottom_offset = this.seq_cropping_win_bottom_offset;
			state.seq_cropping_win_left_offset = this.seq_cropping_win_left_offset;
			state.seq_cropping_win_right_offset = this.seq_cropping_win_right_offset;
			state.seq_cropping_win_top_offset = this.seq_cropping_win_top_offset;
			state.seq_cropping_window_present_flag = this.seq_cropping_window_present_flag;
			state.seq_decoder_model_info_present_flag = this.seq_decoder_model_info_present_flag;
			state.seq_enabled_motion_modes = this.seq_enabled_motion_modes?.Clone();
			state.seq_force_integer_mv = this.seq_force_integer_mv;
			state.seq_force_screen_content_tools = this.seq_force_screen_content_tools;
			state.seq_frame_motion_modes_present_flag = this.seq_frame_motion_modes_present_flag;
			state.seq_header_id = this.seq_header_id;
			state.seq_header_id_in_frame_header = this.seq_header_id_in_frame_header;
			state.seq_initial_display_delay_minus_1 = this.seq_initial_display_delay_minus_1;
			state.seq_initial_display_delay_present_flag = this.seq_initial_display_delay_present_flag;
			state.seq_lcr_id = this.seq_lcr_id;
			state.seq_level_idx = this.seq_level_idx;
			state.seq_max_bvp_drl_bits_minus_1 = this.seq_max_bvp_drl_bits_minus_1;
			state.seq_max_drl_bits_minus_1 = this.seq_max_drl_bits_minus_1;
			state.seq_max_mlayer_cnt_minus_1 = this.seq_max_mlayer_cnt_minus_1;
			state.seq_profile_idc = this.seq_profile_idc;
			state.seq_seg_info_present_flag = this.seq_seg_info_present_flag;
			state.seq_tier = this.seq_tier;
			state.seq_tile_info_present_flag = this.seq_tile_info_present_flag;
			state.signal_primary_ref_frame = this.signal_primary_ref_frame;
			state.single_picture_header_flag = this.single_picture_header_flag;
			state.skip_mode_present = this.skip_mode_present;
			state.slot = this.slot;
			state.source_banding_present_flag = this.source_banding_present_flag;
			state.still_picture = this.still_picture;
			state.subX = this.subX;
			state.subY = this.subY;
			state.sub_stream_max_level = this.sub_stream_max_level?.Clone();
			state.sub_stream_max_profile = this.sub_stream_max_profile?.Clone();
			state.sub_stream_max_tier = this.sub_stream_max_tier?.Clone();
			state.sub_xlayer_id = this.sub_xlayer_id?.Clone();
			state.subexp_bits = this.subexp_bits;
			state.subexp_final_bits = this.subexp_final_bits;
			state.subexp_more_bits = this.subexp_more_bits;
			state.sz = this.sz;
			state.t = this.t;
			state.target = this.target;
			state.temporal_pred_flag = this.temporal_pred_flag?.Clone();
			state.tg_end = this.tg_end;
			state.tg_start = this.tg_start;
			state.their_ref = this.their_ref;
			state.three_color_components_flag = this.three_color_components_flag;
			state.tileCols = this.tileCols;
			state.tileColsLog2 = this.tileColsLog2;
			state.tileLog2 = this.tileLog2;
			state.tileRows = this.tileRows;
			state.tileRowsLog2 = this.tileRowsLog2;
			state.tile_size_bytes_minus_1 = this.tile_size_bytes_minus_1;
			state.tile_size_minus_1 = this.tile_size_minus_1;
			state.tile_start_and_end_present_flag = this.tile_start_and_end_present_flag;
			state.time_offset_length = this.time_offset_length;
			state.time_offset_value = this.time_offset_value;
			state.time_scale = this.time_scale;
			state.tip_frame_mode = this.tip_frame_mode;
			state.tip_global_wtd_index = this.tip_global_wtd_index;
			state.tip_mv_col = this.tip_mv_col;
			state.tip_mv_col_sign = this.tip_mv_col_sign;
			state.tip_mv_row = this.tip_mv_row;
			state.tip_mv_row_sign = this.tip_mv_row_sign;
			state.tip_mv_zero = this.tip_mv_zero;
			state.tip_regular = this.tip_regular;
			state.tip_sharp = this.tip_sharp;
			state.tlayer_dependency_map = this.tlayer_dependency_map;
			state.tlayer_dependency_present_flag = this.tlayer_dependency_present_flag;
			state.tmvp_sample_step_minus_1 = this.tmvp_sample_step_minus_1;
			state.tool_index = this.tool_index;
			state.trailing_one_bit = this.trailing_one_bit;
			state.trailing_zero_bit = this.trailing_zero_bit;
			state.txClass = this.txClass;
			state.txSz = this.txSz;
			state.tx_mode_select = this.tx_mode_select;
			state.uniformSbSize = this.uniformSbSize;
			state.uniformSpacing = this.uniformSpacing;
			state.uniform_tile_spacing_flag = this.uniform_tile_spacing_flag;
			state.unitCol = this.unitCol;
			state.unitRow = this.unitRow;
			state.use_128x128_superblock = this.use_128x128_superblock;
			state.use_256x256_superblock = this.use_256x256_superblock;
			state.use_alt_group = this.use_alt_group;
			state.use_bank = this.use_bank;
			state.use_bru = this.use_bru;
			state.use_global_motion = this.use_global_motion;
			state.use_qtr_precision_mv = this.use_qtr_precision_mv;
			state.use_ref_frame_mvs = this.use_ref_frame_mvs;
			state.user_data_payload_byte = this.user_data_payload_byte;
			state.using_qmatrix = this.using_qmatrix;
			state.uuid_iso_iec_11578 = ((byte[])this.uuid_iso_iec_11578?.Clone());
			state.uv_ac_delta_q_enabled = this.uv_ac_delta_q_enabled;
			state.uv_dc_delta_q_enabled = this.uv_dc_delta_q_enabled;
			state.v = this.v;
			state.varying_size_band_units_flag = this.varying_size_band_units_flag;
			state.vert_size_in_band_blocks_minus_1 = this.vert_size_in_band_blocks_minus_1;
			state.white_point_chromaticity_x = this.white_point_chromaticity_x;
			state.white_point_chromaticity_y = this.white_point_chromaticity_y;
			state.width_in_sbs_minus_1 = this.width_in_sbs_minus_1;
			state.wiener_ns_length = this.wiener_ns_length;
			state.wiener_ns_uv_sym = this.wiener_ns_uv_sym;
			state.xAId = this.xAId;
			state.xId = this.xId;
			state.xLId = this.xLId;
			state.xlayerId = this.xlayerId;
			state.y_dc_delta_q_enabled = this.y_dc_delta_q_enabled;
			state.zero_bit = this.zero_bit;
			SaveContextExtra(state);
			return state;
		}

		private void LoadContext(ContextState state, bool copy = true)
		{
			this.ActiveNumRefFrames = state.ActiveNumRefFrames;
			this.AllowedFrames = state.AllowedFrames;
			this.AtlasHeight = state.AtlasHeight;
			this.AtlasSegmentIDToIndex = copy ? state.AtlasSegmentIDToIndex?.Clone() : state.AtlasSegmentIDToIndex;
			this.AtlasSegmentIndexToID = copy ? state.AtlasSegmentIndexToID?.Clone() : state.AtlasSegmentIndexToID;
			this.AtlasWidth = state.AtlasWidth;
			this.BaseUVAcDeltaQ = state.BaseUVAcDeltaQ;
			this.BaseUVDcDeltaQ = state.BaseUVDcDeltaQ;
			this.BaseYDcDeltaQ = state.BaseYDcDeltaQ;
			this.BitDepth = state.BitDepth;
			this.BruTileActive = state.BruTileActive;
			this.BruTileActives = copy ? state.BruTileActives?.Clone() : state.BruTileActives;
			this.CcsoFilterOffset = copy ? state.CcsoFilterOffset?.Clone() : state.CcsoFilterOffset;
			this.CcsoLumaSizeLog2 = state.CcsoLumaSizeLog2;
			this.CdefDamping = state.CdefDamping;
			this.CdefOnSkipTxfm = state.CdefOnSkipTxfm;
			this.CdefStrengths = state.CdefStrengths;
			this.ClosestFuture = state.ClosestFuture;
			this.ClosestPast = state.ClosestPast;
			this.CodedLossless = state.CodedLossless;
			this.CountFrameHeaderForLevelConstraint = state.CountFrameHeaderForLevelConstraint;
			this.CropHeight = state.CropHeight;
			this.CropLeft = state.CropLeft;
			this.CropTop = state.CropTop;
			this.CropWidth = state.CropWidth;
			this.CurrentQIndex = state.CurrentQIndex;
			this.DeltaQUAc = state.DeltaQUAc;
			this.DeltaQUDc = state.DeltaQUDc;
			this.DeltaQVAc = state.DeltaQVAc;
			this.DeltaQVDc = state.DeltaQVDc;
			this.DeltaQYDc = state.DeltaQYDc;
			this.DerivedPrimaryRefFrame = state.DerivedPrimaryRefFrame;
			this.DfDeltaQ = copy ? state.DfDeltaQ?.Clone() : state.DfDeltaQ;
			this.DrlReorder = state.DrlReorder;
			this.EnableTipOutput = state.EnableTipOutput;
			this.FeatureData = copy ? state.FeatureData?.Clone() : state.FeatureData;
			this.FeatureEnabled = copy ? state.FeatureEnabled?.Clone() : state.FeatureEnabled;
			this.FgmChromaIdc = copy ? state.FgmChromaIdc?.Clone() : state.FgmChromaIdc;
			this.FgmMLayerId = copy ? state.FgmMLayerId?.Clone() : state.FgmMLayerId;
			this.FgmTLayerId = copy ? state.FgmTLayerId?.Clone() : state.FgmTLayerId;
			this.FilmGrainPresent = copy ? state.FilmGrainPresent?.Clone() : state.FilmGrainPresent;
			this.FirstPictureInTU = state.FirstPictureInTU;
			this.FrameDistance = copy ? state.FrameDistance?.Clone() : state.FrameDistance;
			this.FrameHeight = state.FrameHeight;
			this.FrameIsIntra = state.FrameIsIntra;
			this.FrameLrWienerNs = copy ? state.FrameLrWienerNs?.Clone() : state.FrameLrWienerNs;
			this.FrameMvPrecision = state.FrameMvPrecision;
			this.FrameRestorationType = copy ? state.FrameRestorationType?.Clone() : state.FrameRestorationType;
			this.FrameSymbolCount = state.FrameSymbolCount;
			this.FrameType = state.FrameType;
			this.FrameWidth = state.FrameWidth;
			this.FurthestFuture = state.FurthestFuture;
			this.GdfBlkSize = state.GdfBlkSize;
			this.GdfPixScale = state.GdfPixScale;
			this.GmType = copy ? state.GmType?.Clone() : state.GmType;
			this.HasBothRefs = state.HasBothRefs;
			this.HasLosslessSegment = state.HasLosslessSegment;
			this.IBCCoded = copy ? state.IBCCoded?.Clone() : state.IBCCoded;
			this.IsBridge = state.IsBridge;
			this.IsRegular = state.IsRegular;
			this.LastActiveSegId = state.LastActiveSegId;
			this.LcrMaxNumXLayerCount = state.LcrMaxNumXLayerCount;
			this.LcrXLayerID = copy ? state.LcrXLayerID?.Clone() : state.LcrXLayerID;
			this.LongTermId = state.LongTermId;
			this.LoopRestorationSize = copy ? state.LoopRestorationSize?.Clone() : state.LoopRestorationSize;
			this.LosslessArray = copy ? state.LosslessArray?.Clone() : state.LosslessArray;
			this.LrWienerNs = copy ? state.LrWienerNs?.Clone() : state.LrWienerNs;
			this.MLayerDependencyMap = copy ? state.MLayerDependencyMap?.Clone() : state.MLayerDependencyMap;
			this.MLayerPresenceMap = copy ? state.MLayerPresenceMap?.Clone() : state.MLayerPresenceMap;
			this.MaxPbAspectRatio = state.MaxPbAspectRatio;
			this.MaxQ = state.MaxQ;
			this.MaxSegments = state.MaxSegments;
			this.MfhFeatureData = copy ? state.MfhFeatureData?.Clone() : state.MfhFeatureData;
			this.MfhFeatureEnabled = copy ? state.MfhFeatureEnabled?.Clone() : state.MfhFeatureEnabled;
			this.MfhMLayerId = copy ? state.MfhMLayerId?.Clone() : state.MfhMLayerId;
			this.MfhSeqHeaderId = copy ? state.MfhSeqHeaderId?.Clone() : state.MfhSeqHeaderId;
			this.MfhTLayerId = copy ? state.MfhTLayerId?.Clone() : state.MfhTLayerId;
			this.MiColEnd = state.MiColEnd;
			this.MiColStart = state.MiColStart;
			this.MiColStarts = copy ? state.MiColStarts?.Clone() : state.MiColStarts;
			this.MiCols = state.MiCols;
			this.MiRowEnd = state.MiRowEnd;
			this.MiRowStart = state.MiRowStart;
			this.MiRowStarts = copy ? state.MiRowStarts?.Clone() : state.MiRowStarts;
			this.MiRows = state.MiRows;
			this.Monochrome = state.Monochrome;
			this.MotionFieldChecked = copy ? state.MotionFieldChecked?.Clone() : state.MotionFieldChecked;
			this.MotionFieldDepth = copy ? state.MotionFieldDepth?.Clone() : state.MotionFieldDepth;
			this.MotionFieldStack = copy ? state.MotionFieldStack?.Clone() : state.MotionFieldStack;
			this.MotionFieldStackCount = state.MotionFieldStackCount;
			this.MotionFieldVisited = copy ? state.MotionFieldVisited?.Clone() : state.MotionFieldVisited;
			this.MvPrecision = state.MvPrecision;
			this.NRanked = state.NRanked;
			this.NumFilterClasses = state.NumFilterClasses;
			this.NumFrameHeaderBits = state.NumFrameHeaderBits;
			this.NumFutureRefs = state.NumFutureRefs;
			this.NumPastRefs = state.NumPastRefs;
			this.NumPlanes = state.NumPlanes;
			this.NumRefFrames = state.NumRefFrames;
			this.NumRegionsInAtlas = copy ? state.NumRegionsInAtlas?.Clone() : state.NumRegionsInAtlas;
			this.NumSameRefCompound = state.NumSameRefCompound;
			this.NumTiles = state.NumTiles;
			this.NumTotalRefs = state.NumTotalRefs;
			this.OlkEncountered = state.OlkEncountered;
			this.OlkRefresh = copy ? state.OlkRefresh?.Clone() : state.OlkRefresh;
			this.OlkTUOrderHint = state.OlkTUOrderHint;
			this.OpsxLayerId = copy ? state.OpsxLayerId?.Clone() : state.OpsxLayerId;
			this.OrderHint = state.OrderHint;
			this.OrderHintBits = state.OrderHintBits;
			this.OrderHintLsbs = state.OrderHintLsbs;
			this.OrderHints = copy ? state.OrderHints?.Clone() : state.OrderHints;
			this.OrigClosestFuture = state.OrigClosestFuture;
			this.OrigClosestPast = state.OrigClosestPast;
			this.PrevGmParams = copy ? state.PrevGmParams?.Clone() : state.PrevGmParams;
			this.PrevSegmentIds = copy ? state.PrevSegmentIds?.Clone() : state.PrevSegmentIds;
			this.ProjStep = state.ProjStep;
			this.QmDataPresent = copy ? state.QmDataPresent?.Clone() : state.QmDataPresent;
			this.QmMLayerId = copy ? state.QmMLayerId?.Clone() : state.QmMLayerId;
			this.QmNumPlanes = copy ? state.QmNumPlanes?.Clone() : state.QmNumPlanes;
			this.QmProtected = copy ? state.QmProtected?.Clone() : state.QmProtected;
			this.QmSeen = copy ? state.QmSeen?.Clone() : state.QmSeen;
			this.QmTLayerId = copy ? state.QmTLayerId?.Clone() : state.QmTLayerId;
			this.RefLrWienerNs = copy ? state.RefLrWienerNs?.Clone() : state.RefLrWienerNs;
			this.RefOrderHint = copy ? state.RefOrderHint?.Clone() : state.RefOrderHint;
			this.RefValid = copy ? state.RefValid?.Clone() : state.RefValid;
			this.RemainingLcrPayloadBits = state.RemainingLcrPayloadBits;
			this.SbSize = state.SbSize;
			this.ScoresBaseQIdx = copy ? state.ScoresBaseQIdx?.Clone() : state.ScoresBaseQIdx;
			this.ScoresDistance = copy ? state.ScoresDistance?.Clone() : state.ScoresDistance;
			this.ScoresIndex = copy ? state.ScoresIndex?.Clone() : state.ScoresIndex;
			this.ScoresLayer = copy ? state.ScoresLayer?.Clone() : state.ScoresLayer;
			this.ScoresOrderHint = copy ? state.ScoresOrderHint?.Clone() : state.ScoresOrderHint;
			this.ScoresScore = copy ? state.ScoresScore?.Clone() : state.ScoresScore;
			this.SeenFrameHeader = state.SeenFrameHeader;
			this.SegIdPreSkip = state.SegIdPreSkip;
			this.SegQMLevel = copy ? state.SegQMLevel?.Clone() : state.SegQMLevel;
			this.SegmentIds = copy ? state.SegmentIds?.Clone() : state.SegmentIds;
			this.SeqFeatureData = copy ? state.SeqFeatureData?.Clone() : state.SeqFeatureData;
			this.SeqFeatureEnabled = copy ? state.SeqFeatureEnabled?.Clone() : state.SeqFeatureEnabled;
			this.SeqMaxMlayerCnt = state.SeqMaxMlayerCnt;
			this.SeqSbColStarts = copy ? state.SeqSbColStarts?.Clone() : state.SeqSbColStarts;
			this.SeqSbCols = state.SeqSbCols;
			this.SeqSbRowStarts = copy ? state.SeqSbRowStarts?.Clone() : state.SeqSbRowStarts;
			this.SeqSbRows = state.SeqSbRows;
			this.SeqTileCols = state.SeqTileCols;
			this.SeqTileColsLog2 = state.SeqTileColsLog2;
			this.SeqTileRows = state.SeqTileRows;
			this.SeqTileRowsLog2 = state.SeqTileRowsLog2;
			this.SeqUniformTileSpacingFlag = state.SeqUniformTileSpacingFlag;
			this.ShowExistingFrame = state.ShowExistingFrame;
			this.SkipModeFrame = copy ? state.SkipModeFrame?.Clone() : state.SkipModeFrame;
			this.SkipSegFrame = state.SkipSegFrame;
			this.SubclassLookup = copy ? state.SubclassLookup?.Clone() : state.SubclassLookup;
			this.SubsamplingX = state.SubsamplingX;
			this.SubsamplingY = state.SubsamplingY;
			this.TLayerDependencyMap = copy ? state.TLayerDependencyMap?.Clone() : state.TLayerDependencyMap;
			this.TileCols = state.TileCols;
			this.TileColsLog2 = state.TileColsLog2;
			this.TileNum = state.TileNum;
			this.TileRows = state.TileRows;
			this.TileRowsLog2 = state.TileRowsLog2;
			this.TileSizeBytes = state.TileSizeBytes;
			this.TipFrameMode = state.TipFrameMode;
			this.TipGlobalMv = copy ? state.TipGlobalMv?.Clone() : state.TipGlobalMv;
			this.TipInterpFilter = state.TipInterpFilter;
			this.TxMode = state.TxMode;
			this.UsePerBlockMvPrecision = state.UsePerBlockMvPrecision;
			this.UserQm = copy ? state.UserQm?.Clone() : state.UserQm;
			this.UsesLr = state.UsesLr;
			this.WienerNsBankSize = copy ? state.WienerNsBankSize?.Clone() : state.WienerNsBankSize;
			this.WienerNsPtr = copy ? state.WienerNsPtr?.Clone() : state.WienerNsPtr;
			this.XCount = copy ? state.XCount?.Clone() : state.XCount;
			this.a = state.a;
			this.allow_bawp = state.allow_bawp;
			this.allow_df_sub_pu = state.allow_df_sub_pu;
			this.allow_frame_max_bvp_drl_bits = state.allow_frame_max_bvp_drl_bits;
			this.allow_frame_max_drl_bits = state.allow_frame_max_drl_bits;
			this.allow_global_intrabc = state.allow_global_intrabc;
			this.allow_high_precision_mv = state.allow_high_precision_mv;
			this.allow_intrabc = state.allow_intrabc;
			this.allow_local_intrabc = state.allow_local_intrabc;
			this.allow_parity_hiding = state.allow_parity_hiding;
			this.allow_screen_content_tools = state.allow_screen_content_tools;
			this.allow_tcq = state.allow_tcq;
			this.allow_tile_info_change = state.allow_tile_info_change;
			this.allow_tip_hole_fill = state.allow_tip_hole_fill;
			this.allow_warpmv_mode = state.allow_warpmv_mode;
			this.apply_deblocking_filter = copy ? state.apply_deblocking_filter?.Clone() : state.apply_deblocking_filter;
			this.apply_deblocking_filter_tip = state.apply_deblocking_filter_tip;
			this.apply_grain = state.apply_grain;
			this.ar_coeff_lag = state.ar_coeff_lag;
			this.ar_coeff_shift_minus_6 = state.ar_coeff_shift_minus_6;
			this.ar_coeffs_cb = copy ? state.ar_coeffs_cb?.Clone() : state.ar_coeffs_cb;
			this.ar_coeffs_cr = copy ? state.ar_coeffs_cr?.Clone() : state.ar_coeffs_cr;
			this.ar_coeffs_y = copy ? state.ar_coeffs_y?.Clone() : state.ar_coeffs_y;
			this.atlas_segment_id = copy ? state.atlas_segment_id?.Clone() : state.atlas_segment_id;
			this.ats_atlas_segment_id = copy ? state.ats_atlas_segment_id?.Clone() : state.ats_atlas_segment_id;
			this.ats_atlas_segment_mode_idc = copy ? state.ats_atlas_segment_mode_idc?.Clone() : state.ats_atlas_segment_mode_idc;
			this.ats_bottom_right_region_column_off = copy ? state.ats_bottom_right_region_column_off?.Clone() : state.ats_bottom_right_region_column_off;
			this.ats_bottom_right_region_row_off = copy ? state.ats_bottom_right_region_row_off?.Clone() : state.ats_bottom_right_region_row_off;
			this.ats_column_width_minus_1 = copy ? state.ats_column_width_minus_1?.Clone() : state.ats_column_width_minus_1;
			this.ats_height = copy ? state.ats_height?.Clone() : state.ats_height;
			this.ats_input_stream_id = copy ? state.ats_input_stream_id?.Clone() : state.ats_input_stream_id;
			this.ats_msi_alpha_segment_flag = copy ? state.ats_msi_alpha_segment_flag?.Clone() : state.ats_msi_alpha_segment_flag;
			this.ats_msi_alpha_segments_present_flag = copy ? state.ats_msi_alpha_segments_present_flag?.Clone() : state.ats_msi_alpha_segments_present_flag;
			this.ats_msi_background_blue_value = copy ? state.ats_msi_background_blue_value?.Clone() : state.ats_msi_background_blue_value;
			this.ats_msi_background_green_value = copy ? state.ats_msi_background_green_value?.Clone() : state.ats_msi_background_green_value;
			this.ats_msi_background_info_present_flag = copy ? state.ats_msi_background_info_present_flag?.Clone() : state.ats_msi_background_info_present_flag;
			this.ats_msi_background_red_value = copy ? state.ats_msi_background_red_value?.Clone() : state.ats_msi_background_red_value;
			this.ats_msi_height = copy ? state.ats_msi_height?.Clone() : state.ats_msi_height;
			this.ats_msi_input_stream_id = copy ? state.ats_msi_input_stream_id?.Clone() : state.ats_msi_input_stream_id;
			this.ats_msi_num_atlas_segments_minus_1 = copy ? state.ats_msi_num_atlas_segments_minus_1?.Clone() : state.ats_msi_num_atlas_segments_minus_1;
			this.ats_msi_segment_height = copy ? state.ats_msi_segment_height?.Clone() : state.ats_msi_segment_height;
			this.ats_msi_segment_top_left_pos_x = copy ? state.ats_msi_segment_top_left_pos_x?.Clone() : state.ats_msi_segment_top_left_pos_x;
			this.ats_msi_segment_top_left_pos_y = copy ? state.ats_msi_segment_top_left_pos_y?.Clone() : state.ats_msi_segment_top_left_pos_y;
			this.ats_msi_segment_width = copy ? state.ats_msi_segment_width?.Clone() : state.ats_msi_segment_width;
			this.ats_msi_width = copy ? state.ats_msi_width?.Clone() : state.ats_msi_width;
			this.ats_nominal_height_minus_1 = copy ? state.ats_nominal_height_minus_1?.Clone() : state.ats_nominal_height_minus_1;
			this.ats_nominal_width_minus_1 = copy ? state.ats_nominal_width_minus_1?.Clone() : state.ats_nominal_width_minus_1;
			this.ats_num_atlas_segments_minus_1 = copy ? state.ats_num_atlas_segments_minus_1?.Clone() : state.ats_num_atlas_segments_minus_1;
			this.ats_num_region_columns_minus_1 = copy ? state.ats_num_region_columns_minus_1?.Clone() : state.ats_num_region_columns_minus_1;
			this.ats_num_region_rows_minus_1 = copy ? state.ats_num_region_rows_minus_1?.Clone() : state.ats_num_region_rows_minus_1;
			this.ats_region_height_minus_1 = copy ? state.ats_region_height_minus_1?.Clone() : state.ats_region_height_minus_1;
			this.ats_region_width_minus_1 = copy ? state.ats_region_width_minus_1?.Clone() : state.ats_region_width_minus_1;
			this.ats_row_height_minus_1 = copy ? state.ats_row_height_minus_1?.Clone() : state.ats_row_height_minus_1;
			this.ats_segment_height = copy ? state.ats_segment_height?.Clone() : state.ats_segment_height;
			this.ats_segment_top_left_pos_x = copy ? state.ats_segment_top_left_pos_x?.Clone() : state.ats_segment_top_left_pos_x;
			this.ats_segment_top_left_pos_y = copy ? state.ats_segment_top_left_pos_y?.Clone() : state.ats_segment_top_left_pos_y;
			this.ats_segment_width = copy ? state.ats_segment_width?.Clone() : state.ats_segment_width;
			this.ats_signaled_atlas_segment_ids_flag = copy ? state.ats_signaled_atlas_segment_ids_flag?.Clone() : state.ats_signaled_atlas_segment_ids_flag;
			this.ats_single_region_per_atlas_segment_flag = copy ? state.ats_single_region_per_atlas_segment_flag?.Clone() : state.ats_single_region_per_atlas_segment_flag;
			this.ats_stream_id_present = copy ? state.ats_stream_id_present?.Clone() : state.ats_stream_id_present;
			this.ats_top_left_region_column = copy ? state.ats_top_left_region_column?.Clone() : state.ats_top_left_region_column;
			this.ats_top_left_region_row = copy ? state.ats_top_left_region_row?.Clone() : state.ats_top_left_region_row;
			this.ats_uniform_spacing_flag = copy ? state.ats_uniform_spacing_flag?.Clone() : state.ats_uniform_spacing_flag;
			this.ats_width = copy ? state.ats_width?.Clone() : state.ats_width;
			this.avg_cdf_type = state.avg_cdf_type;
			this.b = state.b;
			this.band_block_in_luma_samples = state.band_block_in_luma_samples;
			this.band_units_information_present_flag = state.band_units_information_present_flag;
			this.banding_hints_flag = state.banding_hints_flag;
			this.banding_in_band_unit_present_flag = state.banding_in_band_unit_present_flag;
			this.banding_in_component_present_flag = state.banding_in_component_present_flag;
			this.baseDistance = state.baseDistance;
			this.baseParams = copy ? state.baseParams?.Clone() : state.baseParams;
			this.base_q_idx = state.base_q_idx;
			this.base_qindex = state.base_qindex;
			this.base_uv_ac_delta_q = state.base_uv_ac_delta_q;
			this.base_uv_dc_delta_q = state.base_uv_dc_delta_q;
			this.base_y_dc_delta_q = state.base_y_dc_delta_q;
			this.bestDisp = state.bestDisp;
			this.bestRatio = state.bestRatio;
			this.bit_depth_idc = state.bit_depth_idc;
			this.bits_per_ar_coeff_cb_minus_5 = state.bits_per_ar_coeff_cb_minus_5;
			this.bits_per_ar_coeff_cr_minus_5 = state.bits_per_ar_coeff_cr_minus_5;
			this.bits_per_ar_coeff_y_minus_5 = state.bits_per_ar_coeff_y_minus_5;
			this.blkSize = state.blkSize;
			this.br_decoder_model_present_op_flag = copy ? state.br_decoder_model_present_op_flag?.Clone() : state.br_decoder_model_present_op_flag;
			this.br_ops_cnt = copy ? state.br_ops_cnt?.Clone() : state.br_ops_cnt;
			this.br_ops_dependent_flag = state.br_ops_dependent_flag;
			this.br_ops_id = state.br_ops_id;
			this.br_time = state.br_time;
			this.br_time_op = copy ? state.br_time_op?.Clone() : state.br_time_op;
			this.bridge_frame_height_minus_1 = state.bridge_frame_height_minus_1;
			this.bridge_frame_overwrite_flag = state.bridge_frame_overwrite_flag;
			this.bridge_frame_ref_idx = state.bridge_frame_ref_idx;
			this.bridge_frame_width_minus_1 = state.bridge_frame_width_minus_1;
			this.bru_inactive = state.bru_inactive;
			this.bru_ref = state.bru_ref;
			this.bru_tile_active = state.bru_tile_active;
			this.cb_luma_mult = state.cb_luma_mult;
			this.cb_mult = state.cb_mult;
			this.cb_offset = state.cb_offset;
			this.ccso_bo_only = copy ? state.ccso_bo_only?.Clone() : state.ccso_bo_only;
			this.ccso_edge_clf = copy ? state.ccso_edge_clf?.Clone() : state.ccso_edge_clf;
			this.ccso_ext_filter = copy ? state.ccso_ext_filter?.Clone() : state.ccso_ext_filter;
			this.ccso_frame_flag = state.ccso_frame_flag;
			this.ccso_max_band_log2 = copy ? state.ccso_max_band_log2?.Clone() : state.ccso_max_band_log2;
			this.ccso_offset_idx = state.ccso_offset_idx;
			this.ccso_planes = copy ? state.ccso_planes?.Clone() : state.ccso_planes;
			this.ccso_quant_idx = copy ? state.ccso_quant_idx?.Clone() : state.ccso_quant_idx;
			this.ccso_ref_idx = copy ? state.ccso_ref_idx?.Clone() : state.ccso_ref_idx;
			this.ccso_scale_idx = copy ? state.ccso_scale_idx?.Clone() : state.ccso_scale_idx;
			this.ccso_unit_matches_sb_size = state.ccso_unit_matches_sb_size;
			this.cdef_damping_minus_3 = state.cdef_damping_minus_3;
			this.cdef_frame_enable = state.cdef_frame_enable;
			this.cdef_on_skip_txfm_always_on = state.cdef_on_skip_txfm_always_on;
			this.cdef_on_skip_txfm_disabled = state.cdef_on_skip_txfm_disabled;
			this.cdef_on_skip_txfm_frame_enable = state.cdef_on_skip_txfm_frame_enable;
			this.cdef_strengths_minus_1 = state.cdef_strengths_minus_1;
			this.cdef_uv_pri_strength = copy ? state.cdef_uv_pri_strength?.Clone() : state.cdef_uv_pri_strength;
			this.cdef_uv_pri_zero = state.cdef_uv_pri_zero;
			this.cdef_uv_sec_strength = copy ? state.cdef_uv_sec_strength?.Clone() : state.cdef_uv_sec_strength;
			this.cdef_y_pri_strength = copy ? state.cdef_y_pri_strength?.Clone() : state.cdef_y_pri_strength;
			this.cdef_y_pri_zero = state.cdef_y_pri_zero;
			this.cdef_y_sec_strength = copy ? state.cdef_y_sec_strength?.Clone() : state.cdef_y_sec_strength;
			this.cfl_ds_filter_index = state.cfl_ds_filter_index;
			this.change_bvp_drl = state.change_bvp_drl;
			this.change_drl = state.change_drl;
			this.checkRes = state.checkRes;
			this.choose_tcq_per_frame = state.choose_tcq_per_frame;
			this.chroma_format_idc = state.chroma_format_idc;
			this.chroma_scaling_from_luma = state.chroma_scaling_from_luma;
			this.ci_aspect_ratio_idc = state.ci_aspect_ratio_idc;
			this.ci_aspect_ratio_info_present_flag = state.ci_aspect_ratio_info_present_flag;
			this.ci_chroma_sample_position_bottom = state.ci_chroma_sample_position_bottom;
			this.ci_chroma_sample_position_present_flag = state.ci_chroma_sample_position_present_flag;
			this.ci_chroma_sample_position_top = state.ci_chroma_sample_position_top;
			this.ci_color_description_idc = state.ci_color_description_idc;
			this.ci_color_description_present_flag = state.ci_color_description_present_flag;
			this.ci_color_primaries = state.ci_color_primaries;
			this.ci_full_range_flag = state.ci_full_range_flag;
			this.ci_matrix_coefficients = state.ci_matrix_coefficients;
			this.ci_reserved_2bit = state.ci_reserved_2bit;
			this.ci_sar_height = state.ci_sar_height;
			this.ci_sar_width = state.ci_sar_width;
			this.ci_scan_type_idc = state.ci_scan_type_idc;
			this.ci_timing_info_present_flag = state.ci_timing_info_present_flag;
			this.ci_transfer_characteristics = state.ci_transfer_characteristics;
			this.clip_to_restricted_range = state.clip_to_restricted_range;
			this.cnt_dropped_flag = state.cnt_dropped_flag;
			this.coding_banding_present_flag = state.coding_banding_present_flag;
			this.constrain_drl_reorder = state.constrain_drl_reorder;
			this.context_update_tile_id = state.context_update_tile_id;
			this.counting_type = state.counting_type;
			this.counts = copy ? state.counts?.Clone() : state.counts;
			this.cr_luma_mult = state.cr_luma_mult;
			this.cr_mult = state.cr_mult;
			this.cr_offset = state.cr_offset;
			this.cur_mfh_id = state.cur_mfh_id;
			this.d = state.d;
			this.decoder_buffer_delay = state.decoder_buffer_delay;
			this.decoder_model_info_present_flag = state.decoder_model_info_present_flag;
			this.delta_coded = state.delta_coded;
			this.delta_q = state.delta_q;
			this.delta_q_present = state.delta_q_present;
			this.delta_q_res = state.delta_q_res;
			this.derive_sef_order_hint = state.derive_sef_order_hint;
			this.df_delta_q = copy ? state.df_delta_q?.Clone() : state.df_delta_q;
			this.df_delta_q_present = copy ? state.df_delta_q_present?.Clone() : state.df_delta_q_present;
			this.df_par_bits_minus_2 = state.df_par_bits_minus_2;
			this.diff_uv_delta = state.diff_uv_delta;
			this.disable_cdf_update = state.disable_cdf_update;
			this.disable_cross_frame_cdf_init = state.disable_cross_frame_cdf_init;
			this.disable_drl_reorder = state.disable_drl_reorder;
			this.disable_loopfilters_across_tiles = state.disable_loopfilters_across_tiles;
			this.disable_tip_output = state.disable_tip_output;
			this.discontinuity_flag = state.discontinuity_flag;
			this.dist = state.dist;
			this.enable_adaptive_mvd = state.enable_adaptive_mvd;
			this.enable_avg_cdf = state.enable_avg_cdf;
			this.enable_bawp = state.enable_bawp;
			this.enable_bru = state.enable_bru;
			this.enable_ccso = state.enable_ccso;
			this.enable_cctx = state.enable_cctx;
			this.enable_cdef = state.enable_cdef;
			this.enable_cfl_intra = state.enable_cfl_intra;
			this.enable_chroma_dctonly = state.enable_chroma_dctonly;
			this.enable_cwp = state.enable_cwp;
			this.enable_df_sub_pu = state.enable_df_sub_pu;
			this.enable_dip = state.enable_dip;
			this.enable_ext_partitions = state.enable_ext_partitions;
			this.enable_ext_seg = state.enable_ext_seg;
			this.enable_extended_sdp = state.enable_extended_sdp;
			this.enable_flex_mvres = state.enable_flex_mvres;
			this.enable_fsc = state.enable_fsc;
			this.enable_gdf = state.enable_gdf;
			this.enable_global_motion = state.enable_global_motion;
			this.enable_ibp = state.enable_ibp;
			this.enable_idtx_intra = state.enable_idtx_intra;
			this.enable_imp_msk_bld = state.enable_imp_msk_bld;
			this.enable_inter_ddt = state.enable_inter_ddt;
			this.enable_inter_ist = state.enable_inter_ist;
			this.enable_intra_edge_filter = state.enable_intra_edge_filter;
			this.enable_intra_ist = state.enable_intra_ist;
			this.enable_masked_compound = state.enable_masked_compound;
			this.enable_mhccp = state.enable_mhccp;
			this.enable_mrls = state.enable_mrls;
			this.enable_mv_traj = state.enable_mv_traj;
			this.enable_mvd_sign_derive = state.enable_mvd_sign_derive;
			this.enable_opfl_refine = state.enable_opfl_refine;
			this.enable_parity_hiding = state.enable_parity_hiding;
			this.enable_ref_frame_mvs = state.enable_ref_frame_mvs;
			this.enable_refinemv = state.enable_refinemv;
			this.enable_refmvbank = state.enable_refmvbank;
			this.enable_restoration = state.enable_restoration;
			this.enable_sdp = state.enable_sdp;
			this.enable_short_refresh_frame_flags = state.enable_short_refresh_frame_flags;
			this.enable_six_param_warp_delta = state.enable_six_param_warp_delta;
			this.enable_tcq = state.enable_tcq;
			this.enable_tip = state.enable_tip;
			this.enable_tip_explicit_qp = state.enable_tip_explicit_qp;
			this.enable_tip_hole_fill = state.enable_tip_hole_fill;
			this.enable_tip_refinemv = state.enable_tip_refinemv;
			this.enable_uneven_4way_partitions = state.enable_uneven_4way_partitions;
			this.encoder_buffer_delay = state.encoder_buffer_delay;
			this.equal_ac_dc_q = state.equal_ac_dc_q;
			this.equal_picture_interval = state.equal_picture_interval;
			this.explicit_num_ref_frames = state.explicit_num_ref_frames;
			this.explicit_ref_frame_map = state.explicit_ref_frame_map;
			this.feature = state.feature;
			this.feature_enabled = state.feature_enabled;
			this.feature_value = state.feature_value;
			this.fg_mc_identity = state.fg_mc_identity;
			this.fgm_chroma_idc = state.fgm_chroma_idc;
			this.fgm_id = state.fgm_id;
			this.fgm_update_flags = state.fgm_update_flags;
			this.film_grain_block_size = state.film_grain_block_size;
			this.film_grain_params_present = state.film_grain_params_present;
			this.force_integer_mv = state.force_integer_mv;
			this.found_ref = state.found_ref;
			this.frameHeight = state.frameHeight;
			this.frameWidth = state.frameWidth;
			this.frame_enabled_motion_modes = copy ? state.frame_enabled_motion_modes?.Clone() : state.frame_enabled_motion_modes;
			this.frame_explicit_ref_frame_map = state.frame_explicit_ref_frame_map;
			this.frame_filters_on = copy ? state.frame_filters_on?.Clone() : state.frame_filters_on;
			this.frame_hash = copy ? ((byte[])state.frame_hash?.Clone()) : state.frame_hash;
			this.frame_header_present_flag = state.frame_header_present_flag;
			this.frame_height_bits_minus_1 = state.frame_height_bits_minus_1;
			this.frame_height_minus_1 = state.frame_height_minus_1;
			this.frame_is_inter = state.frame_is_inter;
			this.frame_presentation_time = state.frame_presentation_time;
			this.frame_size_override_flag = state.frame_size_override_flag;
			this.frame_to_refresh = state.frame_to_refresh;
			this.frame_to_show_map_idx = state.frame_to_show_map_idx;
			this.frame_width_bits_minus_1 = state.frame_width_bits_minus_1;
			this.frame_width_minus_1 = state.frame_width_minus_1;
			this.full_timestamp_flag = state.full_timestamp_flag;
			this.gdf_frame_enable = state.gdf_frame_enable;
			this.gdf_per_block = state.gdf_per_block;
			this.gdf_pic_qc_idx = state.gdf_pic_qc_idx;
			this.gdf_pic_scale_idx = state.gdf_pic_scale_idx;
			this.gdf_unit_matches_sb_size = state.gdf_unit_matches_sb_size;
			this.gm_params = copy ? state.gm_params?.Clone() : state.gm_params;
			this.grain_scale_shift = state.grain_scale_shift;
			this.grain_scaling_minus_8 = state.grain_scaling_minus_8;
			this.grain_seed = state.grain_seed;
			this.group_bit = state.group_bit;
			this.has_grain = state.has_grain;
			this.has_refresh_frame_flags = state.has_refresh_frame_flags;
			this.hash_type = state.hash_type;
			this.header_bit = copy ? state.header_bit?.Clone() : state.header_bit;
			this.height_in_sbs_minus_1 = state.height_in_sbs_minus_1;
			this.high = state.high;
			this.horz_size_in_band_blocks_minus_1 = state.horz_size_in_band_blocks_minus_1;
			this.hours_flag = state.hours_flag;
			this.hours_value = state.hours_value;
			this.icc_profile_data_payload_bytes = state.icc_profile_data_payload_bytes;
			this.idx = state.idx;
			this.immediate_output_frame = state.immediate_output_frame;
			this.implicit_output_frame = state.implicit_output_frame;
			this.increment_tile_cols_log2 = state.increment_tile_cols_log2;
			this.increment_tile_rows_log2 = state.increment_tile_rows_log2;
			this.interpolation_filter = state.interpolation_filter;
			this.isBridge = state.isBridge;
			this.isFirst = state.isFirst;
			this.isGlobal = state.isGlobal;
			this.is_filter_switchable = state.is_filter_switchable;
			this.is_first_tile_group = state.is_first_tile_group;
			this.is_global = state.is_global;
			this.is_monochrome = state.is_monochrome;
			this.is_rot_zoom = state.is_rot_zoom;
			this.itu_t_t35_country_code = state.itu_t_t35_country_code;
			this.itu_t_t35_country_code_extension_byte = state.itu_t_t35_country_code_extension_byte;
			this.itu_t_t35_payload_bytes = state.itu_t_t35_payload_bytes;
			this.layer_color_description_idc = copy ? state.layer_color_description_idc?.Clone() : state.layer_color_description_idc;
			this.layer_color_primaries = copy ? state.layer_color_primaries?.Clone() : state.layer_color_primaries;
			this.layer_full_range_flag = copy ? state.layer_full_range_flag?.Clone() : state.layer_full_range_flag;
			this.layer_matrix_coefficients = copy ? state.layer_matrix_coefficients?.Clone() : state.layer_matrix_coefficients;
			this.layer_transfer_characteristics = copy ? state.layer_transfer_characteristics?.Clone() : state.layer_transfer_characteristics;
			this.lcr_aggregate_info_present_flag = state.lcr_aggregate_info_present_flag;
			this.lcr_aggregate_level_idx = state.lcr_aggregate_level_idx;
			this.lcr_auxiliary_type = copy ? state.lcr_auxiliary_type?.Clone() : state.lcr_auxiliary_type;
			this.lcr_bit_depth_idc = copy ? state.lcr_bit_depth_idc?.Clone() : state.lcr_bit_depth_idc;
			this.lcr_chroma_format_idc = copy ? state.lcr_chroma_format_idc?.Clone() : state.lcr_chroma_format_idc;
			this.lcr_config_idc = state.lcr_config_idc;
			this.lcr_cropping_win_bottom_offset = copy ? state.lcr_cropping_win_bottom_offset?.Clone() : state.lcr_cropping_win_bottom_offset;
			this.lcr_cropping_win_left_offset = copy ? state.lcr_cropping_win_left_offset?.Clone() : state.lcr_cropping_win_left_offset;
			this.lcr_cropping_win_right_offset = copy ? state.lcr_cropping_win_right_offset?.Clone() : state.lcr_cropping_win_right_offset;
			this.lcr_cropping_win_top_offset = copy ? state.lcr_cropping_win_top_offset?.Clone() : state.lcr_cropping_win_top_offset;
			this.lcr_cropping_window_present_flag = copy ? state.lcr_cropping_window_present_flag?.Clone() : state.lcr_cropping_window_present_flag;
			this.lcr_data_size = copy ? state.lcr_data_size?.Clone() : state.lcr_data_size;
			this.lcr_dependent_layer_map = copy ? state.lcr_dependent_layer_map?.Clone() : state.lcr_dependent_layer_map;
			this.lcr_dependent_xlayers_flag = state.lcr_dependent_xlayers_flag;
			this.lcr_doh_constraint_flag = state.lcr_doh_constraint_flag;
			this.lcr_embedded_layer_info_present_flag = copy ? state.lcr_embedded_layer_info_present_flag?.Clone() : state.lcr_embedded_layer_info_present_flag;
			this.lcr_enforce_tile_alignment_flag = state.lcr_enforce_tile_alignment_flag;
			this.lcr_format_info_present_flag = copy ? state.lcr_format_info_present_flag?.Clone() : state.lcr_format_info_present_flag;
			this.lcr_global_atlas_id = state.lcr_global_atlas_id;
			this.lcr_global_atlas_id_present_flag = state.lcr_global_atlas_id_present_flag;
			this.lcr_global_config_record_id = state.lcr_global_config_record_id;
			this.lcr_global_id = copy ? state.lcr_global_id?.Clone() : state.lcr_global_id;
			this.lcr_global_payload_present_flag = state.lcr_global_payload_present_flag;
			this.lcr_global_purpose_id = state.lcr_global_purpose_id;
			this.lcr_global_reserved_zero_3bits = state.lcr_global_reserved_zero_3bits;
			this.lcr_global_reserved_zero_5bits = state.lcr_global_reserved_zero_5bits;
			this.lcr_layer_atlas_segment_id = copy ? state.lcr_layer_atlas_segment_id?.Clone() : state.lcr_layer_atlas_segment_id;
			this.lcr_layer_type = copy ? state.lcr_layer_type?.Clone() : state.lcr_layer_type;
			this.lcr_local_atlas_id = copy ? state.lcr_local_atlas_id?.Clone() : state.lcr_local_atlas_id;
			this.lcr_local_atlas_id_present_flag = copy ? state.lcr_local_atlas_id_present_flag?.Clone() : state.lcr_local_atlas_id_present_flag;
			this.lcr_local_id = copy ? state.lcr_local_id?.Clone() : state.lcr_local_id;
			this.lcr_local_reserved_zero_3bits = copy ? state.lcr_local_reserved_zero_3bits?.Clone() : state.lcr_local_reserved_zero_3bits;
			this.lcr_local_reserved_zero_5bits = copy ? state.lcr_local_reserved_zero_5bits?.Clone() : state.lcr_local_reserved_zero_5bits;
			this.lcr_max_expected_height = copy ? state.lcr_max_expected_height?.Clone() : state.lcr_max_expected_height;
			this.lcr_max_expected_width = copy ? state.lcr_max_expected_width?.Clone() : state.lcr_max_expected_width;
			this.lcr_max_interop = state.lcr_max_interop;
			this.lcr_max_level_idx = copy ? state.lcr_max_level_idx?.Clone() : state.lcr_max_level_idx;
			this.lcr_max_mlayer_count = copy ? state.lcr_max_mlayer_count?.Clone() : state.lcr_max_mlayer_count;
			this.lcr_max_pic_height = copy ? state.lcr_max_pic_height?.Clone() : state.lcr_max_pic_height;
			this.lcr_max_pic_width = copy ? state.lcr_max_pic_width?.Clone() : state.lcr_max_pic_width;
			this.lcr_max_tier_flag = state.lcr_max_tier_flag;
			this.lcr_mlayer_map = copy ? state.lcr_mlayer_map?.Clone() : state.lcr_mlayer_map;
			this.lcr_num_dependent_xlayer_map = copy ? state.lcr_num_dependent_xlayer_map?.Clone() : state.lcr_num_dependent_xlayer_map;
			this.lcr_priority_order = copy ? state.lcr_priority_order?.Clone() : state.lcr_priority_order;
			this.lcr_profile_tier_level_info_present_flag = copy ? state.lcr_profile_tier_level_info_present_flag?.Clone() : state.lcr_profile_tier_level_info_present_flag;
			this.lcr_remaining_payload_bit = state.lcr_remaining_payload_bit;
			this.lcr_rendering_method = copy ? state.lcr_rendering_method?.Clone() : state.lcr_rendering_method;
			this.lcr_rep_info_present_flag = copy ? state.lcr_rep_info_present_flag?.Clone() : state.lcr_rep_info_present_flag;
			this.lcr_same_sh_max_resolution_flag = copy ? state.lcr_same_sh_max_resolution_flag?.Clone() : state.lcr_same_sh_max_resolution_flag;
			this.lcr_seq_profile_idc = copy ? state.lcr_seq_profile_idc?.Clone() : state.lcr_seq_profile_idc;
			this.lcr_seq_profile_tier_level_info_present_flag = state.lcr_seq_profile_tier_level_info_present_flag;
			this.lcr_tier_flag = copy ? state.lcr_tier_flag?.Clone() : state.lcr_tier_flag;
			this.lcr_tlayer_map = copy ? state.lcr_tlayer_map?.Clone() : state.lcr_tlayer_map;
			this.lcr_view_id = copy ? state.lcr_view_id?.Clone() : state.lcr_view_id;
			this.lcr_view_type = copy ? state.lcr_view_type?.Clone() : state.lcr_view_type;
			this.lcr_xlayer_atlas_segment_id = copy ? state.lcr_xlayer_atlas_segment_id?.Clone() : state.lcr_xlayer_atlas_segment_id;
			this.lcr_xlayer_color_info_present_flag = copy ? state.lcr_xlayer_color_info_present_flag?.Clone() : state.lcr_xlayer_color_info_present_flag;
			this.lcr_xlayer_map = state.lcr_xlayer_map;
			this.lcr_xlayer_priority_order = copy ? state.lcr_xlayer_priority_order?.Clone() : state.lcr_xlayer_priority_order;
			this.lcr_xlayer_purpose_id = copy ? state.lcr_xlayer_purpose_id?.Clone() : state.lcr_xlayer_purpose_id;
			this.lcr_xlayer_purpose_present_flag = copy ? state.lcr_xlayer_purpose_present_flag?.Clone() : state.lcr_xlayer_purpose_present_flag;
			this.lcr_xlayer_rendering_method = copy ? state.lcr_xlayer_rendering_method?.Clone() : state.lcr_xlayer_rendering_method;
			this.level = state.level;
			this.loadCdfs = state.loadCdfs;
			this.longTermId = state.longTermId;
			this.long_term_frame_id_bits = state.long_term_frame_id_bits;
			this.long_term_id_plus_1 = state.long_term_id_plus_1;
			this.low = state.low;
			this.low_delay_mode_flag = state.low_delay_mode_flag;
			this.lr_chroma_use_half_size = state.lr_chroma_use_half_size;
			this.lr_chroma_use_max_size = state.lr_chroma_use_max_size;
			this.lr_chroma_use_quarter_size = state.lr_chroma_use_quarter_size;
			this.lr_luma_use_half_size = state.lr_luma_use_half_size;
			this.lr_luma_use_max_size = state.lr_luma_use_max_size;
			this.lr_luma_use_quarter_size = state.lr_luma_use_quarter_size;
			this.lr_tools_disable = copy ? state.lr_tools_disable?.Clone() : state.lr_tools_disable;
			this.lr_tools_uv_present = state.lr_tools_uv_present;
			this.lsptli_reserved_2bits = state.lsptli_reserved_2bits;
			this.luminance_max = state.luminance_max;
			this.luminance_min = state.luminance_min;
			this.m = state.m;
			this.max_band_step_minus_1 = state.max_band_step_minus_1;
			this.max_band_width_minus_4 = state.max_band_width_minus_4;
			this.max_bvp_drl_bits_minus_1 = state.max_bvp_drl_bits_minus_1;
			this.max_cll = state.max_cll;
			this.max_drl_bits_minus_1 = state.max_drl_bits_minus_1;
			this.max_fall = state.max_fall;
			this.max_frame_height_minus_1 = state.max_frame_height_minus_1;
			this.max_frame_width_minus_1 = state.max_frame_width_minus_1;
			this.max_mlayer_id = state.max_mlayer_id;
			this.max_pb_aspect_ratio_log2_minus_1 = state.max_pb_aspect_ratio_log2_minus_1;
			this.max_tlayer_id = state.max_tlayer_id;
			this.merged_param = state.merged_param;
			this.metadataPayloadSize = state.metadataPayloadSize;
			this.metadata_application_id = state.metadata_application_id;
			this.metadata_is_suffix = state.metadata_is_suffix;
			this.metadata_necessity_idc = state.metadata_necessity_idc;
			this.metadata_type = state.metadata_type;
			this.metadata_unit_cnt_minus_1 = state.metadata_unit_cnt_minus_1;
			this.metadata_unit_remaining_bit = state.metadata_unit_remaining_bit;
			this.mfh_allow_seg_info_change = copy ? state.mfh_allow_seg_info_change?.Clone() : state.mfh_allow_seg_info_change;
			this.mfh_apply_deblocking_filter = copy ? state.mfh_apply_deblocking_filter?.Clone() : state.mfh_apply_deblocking_filter;
			this.mfh_deblocking_filter_update = copy ? state.mfh_deblocking_filter_update?.Clone() : state.mfh_deblocking_filter_update;
			this.mfh_ext_seg_flag = copy ? state.mfh_ext_seg_flag?.Clone() : state.mfh_ext_seg_flag;
			this.mfh_frame_height_bits_minus_1 = state.mfh_frame_height_bits_minus_1;
			this.mfh_frame_height_minus_1 = copy ? state.mfh_frame_height_minus_1?.Clone() : state.mfh_frame_height_minus_1;
			this.mfh_frame_size_present_flag = copy ? state.mfh_frame_size_present_flag?.Clone() : state.mfh_frame_size_present_flag;
			this.mfh_frame_width_bits_minus_1 = state.mfh_frame_width_bits_minus_1;
			this.mfh_frame_width_minus_1 = copy ? state.mfh_frame_width_minus_1?.Clone() : state.mfh_frame_width_minus_1;
			this.mfh_id_minus_1 = state.mfh_id_minus_1;
			this.mfh_seg_info_present_flag = copy ? state.mfh_seg_info_present_flag?.Clone() : state.mfh_seg_info_present_flag;
			this.mfh_seq_header_id = state.mfh_seq_header_id;
			this.minutes_flag = state.minutes_flag;
			this.minutes_value = state.minutes_value;
			this.mis = state.mis;
			this.mlayer_dependency_map = state.mlayer_dependency_map;
			this.mlayer_dependency_present_flag = state.mlayer_dependency_present_flag;
			this.monochrome = state.monochrome;
			this.monotonic_output_order_flag = state.monotonic_output_order_flag;
			this.mps_duplicate_flag = state.mps_duplicate_flag;
			this.mps_pic_struct_type = state.mps_pic_struct_type;
			this.mps_source_scan_type_idc = state.mps_source_scan_type_idc;
			this.muh_cancel_flag = state.muh_cancel_flag;
			this.muh_header_extension_byte = state.muh_header_extension_byte;
			this.muh_header_size = state.muh_header_size;
			this.muh_layer_idc = state.muh_layer_idc;
			this.muh_mlayer_map = state.muh_mlayer_map;
			this.muh_payload_size = state.muh_payload_size;
			this.muh_persistence_idc = state.muh_persistence_idc;
			this.muh_priority = state.muh_priority;
			this.muh_reserved_zero_2bits = state.muh_reserved_zero_2bits;
			this.muh_xlayer_map = state.muh_xlayer_map;
			this.multi_tlayer_dependency_map_present_flag = state.multi_tlayer_dependency_map_present_flag;
			this.multistream_doh_constraint_flag = state.multistream_doh_constraint_flag;
			this.multistream_even_allocation_flag = state.multistream_even_allocation_flag;
			this.multistream_large_picture_idc = state.multistream_large_picture_idc;
			this.multistream_level_idx = state.multistream_level_idx;
			this.multistream_profile_idc = state.multistream_profile_idc;
			this.multistream_tier = state.multistream_tier;
			this.mx = state.mx;
			this.n = state.n;
			this.n_frames = state.n_frames;
			this.nbBits = state.nbBits;
			this.numSegments = state.numSegments;
			this.numSyms = state.numSyms;
			this.num_band_units_cols_minus_1 = state.num_band_units_cols_minus_1;
			this.num_band_units_rows_minus_1 = state.num_band_units_rows_minus_1;
			this.num_cb_points = state.num_cb_points;
			this.num_cr_points = state.num_cr_points;
			this.num_filter_classes_idx = state.num_filter_classes_idx;
			this.num_key_ref_frames = state.num_key_ref_frames;
			this.num_ref_frames_minus_1 = state.num_ref_frames_minus_1;
			this.num_same_ref_compound = state.num_same_ref_compound;
			this.num_streams_minus_2 = state.num_streams_minus_2;
			this.num_ticks_per_picture_minus_1 = state.num_ticks_per_picture_minus_1;
			this.num_total_refs = state.num_total_refs;
			this.num_units_in_decoding_tick = state.num_units_in_decoding_tick;
			this.num_units_in_display_tick = state.num_units_in_display_tick;
			this.num_y_points = state.num_y_points;
			this.obuPayloadSize = state.obuPayloadSize;
			this.obuXLId = state.obuXLId;
			this.obu_extension_data_bit = state.obu_extension_data_bit;
			this.obu_extension_flag = state.obu_extension_flag;
			this.obu_header_extension_flag = state.obu_header_extension_flag;
			this.obu_mlayer_id = state.obu_mlayer_id;
			this.obu_padding_byte = state.obu_padding_byte;
			this.obu_tlayer_id = state.obu_tlayer_id;
			this.obu_type = state.obu_type;
			this.obu_xlayer_id = state.obu_xlayer_id;
			this.onlyShowable = state.onlyShowable;
			this.opIndex = state.opIndex;
			this.opfl_refine_all = state.opfl_refine_all;
			this.opfl_refine_type = state.opfl_refine_type;
			this.opsID = state.opsID;
			this.ops_aggregate_level_idx = copy ? state.ops_aggregate_level_idx?.Clone() : state.ops_aggregate_level_idx;
			this.ops_cnt = copy ? state.ops_cnt?.Clone() : state.ops_cnt;
			this.ops_color_description_idc = copy ? state.ops_color_description_idc?.Clone() : state.ops_color_description_idc;
			this.ops_color_info_present_flag = copy ? state.ops_color_info_present_flag?.Clone() : state.ops_color_info_present_flag;
			this.ops_color_primaries = copy ? state.ops_color_primaries?.Clone() : state.ops_color_primaries;
			this.ops_config_idc = copy ? state.ops_config_idc?.Clone() : state.ops_config_idc;
			this.ops_data_size = copy ? state.ops_data_size?.Clone() : state.ops_data_size;
			this.ops_decoder_buffer_delay = copy ? state.ops_decoder_buffer_delay?.Clone() : state.ops_decoder_buffer_delay;
			this.ops_decoder_model_info_for_this_op_present_flag = copy ? state.ops_decoder_model_info_for_this_op_present_flag?.Clone() : state.ops_decoder_model_info_for_this_op_present_flag;
			this.ops_embedded_op_index = copy ? state.ops_embedded_op_index?.Clone() : state.ops_embedded_op_index;
			this.ops_embedded_ops_id = copy ? state.ops_embedded_ops_id?.Clone() : state.ops_embedded_ops_id;
			this.ops_encoder_buffer_delay = copy ? state.ops_encoder_buffer_delay?.Clone() : state.ops_encoder_buffer_delay;
			this.ops_full_range_flag = copy ? state.ops_full_range_flag?.Clone() : state.ops_full_range_flag;
			this.ops_id = copy ? state.ops_id?.Clone() : state.ops_id;
			this.ops_initial_display_delay_minus_1 = copy ? state.ops_initial_display_delay_minus_1?.Clone() : state.ops_initial_display_delay_minus_1;
			this.ops_initial_display_delay_present_flag = copy ? state.ops_initial_display_delay_present_flag?.Clone() : state.ops_initial_display_delay_present_flag;
			this.ops_intent = copy ? state.ops_intent?.Clone() : state.ops_intent;
			this.ops_intent_present_flag = copy ? state.ops_intent_present_flag?.Clone() : state.ops_intent_present_flag;
			this.ops_level_idx = copy ? state.ops_level_idx?.Clone() : state.ops_level_idx;
			this.ops_low_delay_mode_flag = copy ? state.ops_low_delay_mode_flag?.Clone() : state.ops_low_delay_mode_flag;
			this.ops_matrix_coefficients = copy ? state.ops_matrix_coefficients?.Clone() : state.ops_matrix_coefficients;
			this.ops_max_interop = copy ? state.ops_max_interop?.Clone() : state.ops_max_interop;
			this.ops_max_tier_flag = copy ? state.ops_max_tier_flag?.Clone() : state.ops_max_tier_flag;
			this.ops_mlayer_count = copy ? state.ops_mlayer_count?.Clone() : state.ops_mlayer_count;
			this.ops_mlayer_explicit_info_flag = copy ? state.ops_mlayer_explicit_info_flag?.Clone() : state.ops_mlayer_explicit_info_flag;
			this.ops_mlayer_info_idc = copy ? state.ops_mlayer_info_idc?.Clone() : state.ops_mlayer_info_idc;
			this.ops_mlayer_map = copy ? state.ops_mlayer_map?.Clone() : state.ops_mlayer_map;
			this.ops_op_intent = copy ? state.ops_op_intent?.Clone() : state.ops_op_intent;
			this.ops_priority = copy ? state.ops_priority?.Clone() : state.ops_priority;
			this.ops_ptl_present_flag = copy ? state.ops_ptl_present_flag?.Clone() : state.ops_ptl_present_flag;
			this.ops_ptl_reserved_2bits = state.ops_ptl_reserved_2bits;
			this.ops_reserved_2bits = state.ops_reserved_2bits;
			this.ops_reset_flag = copy ? state.ops_reset_flag?.Clone() : state.ops_reset_flag;
			this.ops_seq_profile_idc = copy ? state.ops_seq_profile_idc?.Clone() : state.ops_seq_profile_idc;
			this.ops_tier_flag = copy ? state.ops_tier_flag?.Clone() : state.ops_tier_flag;
			this.ops_tlayer_map = copy ? state.ops_tlayer_map?.Clone() : state.ops_tlayer_map;
			this.ops_transfer_characteristics = copy ? state.ops_transfer_characteristics?.Clone() : state.ops_transfer_characteristics;
			this.ops_xlayer_map = copy ? state.ops_xlayer_map?.Clone() : state.ops_xlayer_map;
			this.order_hint = state.order_hint;
			this.order_hint_bits_minus_1 = state.order_hint_bits_minus_1;
			this.our_ref = state.our_ref;
			this.overlap_flag = state.overlap_flag;
			this.per_plane = state.per_plane;
			this.pic_qm_num_minus_1 = state.pic_qm_num_minus_1;
			this.plane = state.plane;
			this.plane_hash = copy ? state.plane_hash?.Clone() : state.plane_hash;
			this.point_cb_scaling = copy ? state.point_cb_scaling?.Clone() : state.point_cb_scaling;
			this.point_cb_value = copy ? state.point_cb_value?.Clone() : state.point_cb_value;
			this.point_cr_scaling = copy ? state.point_cr_scaling?.Clone() : state.point_cr_scaling;
			this.point_cr_value = copy ? state.point_cr_value?.Clone() : state.point_cr_value;
			this.point_scaling_bits_minus_5 = state.point_scaling_bits_minus_5;
			this.point_value_increment_bits_minus_1 = state.point_value_increment_bits_minus_1;
			this.point_y_scaling = copy ? state.point_y_scaling?.Clone() : state.point_y_scaling;
			this.point_y_value = copy ? state.point_y_value?.Clone() : state.point_y_value;
			this.pos = state.pos;
			this.primary_chromaticity_x = copy ? state.primary_chromaticity_x?.Clone() : state.primary_chromaticity_x;
			this.primary_chromaticity_y = copy ? state.primary_chromaticity_y?.Clone() : state.primary_chromaticity_y;
			this.primary_ref_frame = state.primary_ref_frame;
			this.qThresh = state.qThresh;
			this.qm_4x8_is_transpose_of_8x4 = state.qm_4x8_is_transpose_of_8x4;
			this.qm_8x8_is_symmetric = state.qm_8x8_is_symmetric;
			this.qm_bit_map = state.qm_bit_map;
			this.qm_chroma_info_present_flag = state.qm_chroma_info_present_flag;
			this.qm_copy_from_previous_plane = state.qm_copy_from_previous_plane;
			this.qm_index = state.qm_index;
			this.qm_is_default_flag = state.qm_is_default_flag;
			this.qm_u = copy ? state.qm_u?.Clone() : state.qm_u;
			this.qm_uv_same_as_y = state.qm_uv_same_as_y;
			this.qm_v = copy ? state.qm_v?.Clone() : state.qm_v;
			this.qm_y = copy ? state.qm_y?.Clone() : state.qm_y;
			this.quant_delta = state.quant_delta;
			this.readFrameFilters = state.readFrameFilters;
			this.reduce_pb_aspect_ratio = state.reduce_pb_aspect_ratio;
			this.reduced_ref_frame_mvs_mode = state.reduced_ref_frame_mvs_mode;
			this.reduced_tx_part_set = state.reduced_tx_part_set;
			this.reduced_tx_set = state.reduced_tx_set;
			this.refDisp = state.refDisp;
			this.refRatio = state.refRatio;
			this.ref_frame_idx = copy ? state.ref_frame_idx?.Clone() : state.ref_frame_idx;
			this.ref_long_term_id = copy ? state.ref_long_term_id?.Clone() : state.ref_long_term_id;
			this.reference_select = state.reference_select;
			this.refresh_frame_flags = state.refresh_frame_flags;
			this.reserved = state.reserved;
			this.restricted_prediction_switch = state.restricted_prediction_switch;
			this.reuse_ccso = copy ? state.reuse_ccso?.Clone() : state.reuse_ccso;
			this.reuse_seg_info = state.reuse_seg_info;
			this.reuse_tile_info = state.reuse_tile_info;
			this.rst_ref_pic_idx = state.rst_ref_pic_idx;
			this.sbColStarts = copy ? state.sbColStarts?.Clone() : state.sbColStarts;
			this.sbNum = state.sbNum;
			this.sbRowStarts = copy ? state.sbRowStarts?.Clone() : state.sbRowStarts;
			this.sbSize = state.sbSize;
			this.sb_reuse_ccso = copy ? state.sb_reuse_ccso?.Clone() : state.sb_reuse_ccso;
			this.score = state.score;
			this.seconds_flag = state.seconds_flag;
			this.seconds_value = state.seconds_value;
			this.sef_order_hint = state.sef_order_hint;
			this.segmentation_enabled = state.segmentation_enabled;
			this.segmentation_temporal_update = state.segmentation_temporal_update;
			this.segmentation_update_map = state.segmentation_update_map;
			this.separate_uv_delta_q = state.separate_uv_delta_q;
			this.seqSbSize = state.seqSbSize;
			this.seq_allow_seg_info_change = state.seq_allow_seg_info_change;
			this.seq_choose_integer_mv = state.seq_choose_integer_mv;
			this.seq_choose_screen_content_tools = state.seq_choose_screen_content_tools;
			this.seq_cropping_win_bottom_offset = state.seq_cropping_win_bottom_offset;
			this.seq_cropping_win_left_offset = state.seq_cropping_win_left_offset;
			this.seq_cropping_win_right_offset = state.seq_cropping_win_right_offset;
			this.seq_cropping_win_top_offset = state.seq_cropping_win_top_offset;
			this.seq_cropping_window_present_flag = state.seq_cropping_window_present_flag;
			this.seq_decoder_model_info_present_flag = state.seq_decoder_model_info_present_flag;
			this.seq_enabled_motion_modes = copy ? state.seq_enabled_motion_modes?.Clone() : state.seq_enabled_motion_modes;
			this.seq_force_integer_mv = state.seq_force_integer_mv;
			this.seq_force_screen_content_tools = state.seq_force_screen_content_tools;
			this.seq_frame_motion_modes_present_flag = state.seq_frame_motion_modes_present_flag;
			this.seq_header_id = state.seq_header_id;
			this.seq_header_id_in_frame_header = state.seq_header_id_in_frame_header;
			this.seq_initial_display_delay_minus_1 = state.seq_initial_display_delay_minus_1;
			this.seq_initial_display_delay_present_flag = state.seq_initial_display_delay_present_flag;
			this.seq_lcr_id = state.seq_lcr_id;
			this.seq_level_idx = state.seq_level_idx;
			this.seq_max_bvp_drl_bits_minus_1 = state.seq_max_bvp_drl_bits_minus_1;
			this.seq_max_drl_bits_minus_1 = state.seq_max_drl_bits_minus_1;
			this.seq_max_mlayer_cnt_minus_1 = state.seq_max_mlayer_cnt_minus_1;
			this.seq_profile_idc = state.seq_profile_idc;
			this.seq_seg_info_present_flag = state.seq_seg_info_present_flag;
			this.seq_tier = state.seq_tier;
			this.seq_tile_info_present_flag = state.seq_tile_info_present_flag;
			this.signal_primary_ref_frame = state.signal_primary_ref_frame;
			this.single_picture_header_flag = state.single_picture_header_flag;
			this.skip_mode_present = state.skip_mode_present;
			this.slot = state.slot;
			this.source_banding_present_flag = state.source_banding_present_flag;
			this.still_picture = state.still_picture;
			this.subX = state.subX;
			this.subY = state.subY;
			this.sub_stream_max_level = copy ? state.sub_stream_max_level?.Clone() : state.sub_stream_max_level;
			this.sub_stream_max_profile = copy ? state.sub_stream_max_profile?.Clone() : state.sub_stream_max_profile;
			this.sub_stream_max_tier = copy ? state.sub_stream_max_tier?.Clone() : state.sub_stream_max_tier;
			this.sub_xlayer_id = copy ? state.sub_xlayer_id?.Clone() : state.sub_xlayer_id;
			this.subexp_bits = state.subexp_bits;
			this.subexp_final_bits = state.subexp_final_bits;
			this.subexp_more_bits = state.subexp_more_bits;
			this.sz = state.sz;
			this.t = state.t;
			this.target = state.target;
			this.temporal_pred_flag = copy ? state.temporal_pred_flag?.Clone() : state.temporal_pred_flag;
			this.tg_end = state.tg_end;
			this.tg_start = state.tg_start;
			this.their_ref = state.their_ref;
			this.three_color_components_flag = state.three_color_components_flag;
			this.tileCols = state.tileCols;
			this.tileColsLog2 = state.tileColsLog2;
			this.tileLog2 = state.tileLog2;
			this.tileRows = state.tileRows;
			this.tileRowsLog2 = state.tileRowsLog2;
			this.tile_size_bytes_minus_1 = state.tile_size_bytes_minus_1;
			this.tile_size_minus_1 = state.tile_size_minus_1;
			this.tile_start_and_end_present_flag = state.tile_start_and_end_present_flag;
			this.time_offset_length = state.time_offset_length;
			this.time_offset_value = state.time_offset_value;
			this.time_scale = state.time_scale;
			this.tip_frame_mode = state.tip_frame_mode;
			this.tip_global_wtd_index = state.tip_global_wtd_index;
			this.tip_mv_col = state.tip_mv_col;
			this.tip_mv_col_sign = state.tip_mv_col_sign;
			this.tip_mv_row = state.tip_mv_row;
			this.tip_mv_row_sign = state.tip_mv_row_sign;
			this.tip_mv_zero = state.tip_mv_zero;
			this.tip_regular = state.tip_regular;
			this.tip_sharp = state.tip_sharp;
			this.tlayer_dependency_map = state.tlayer_dependency_map;
			this.tlayer_dependency_present_flag = state.tlayer_dependency_present_flag;
			this.tmvp_sample_step_minus_1 = state.tmvp_sample_step_minus_1;
			this.tool_index = state.tool_index;
			this.trailing_one_bit = state.trailing_one_bit;
			this.trailing_zero_bit = state.trailing_zero_bit;
			this.txClass = state.txClass;
			this.txSz = state.txSz;
			this.tx_mode_select = state.tx_mode_select;
			this.uniformSbSize = state.uniformSbSize;
			this.uniformSpacing = state.uniformSpacing;
			this.uniform_tile_spacing_flag = state.uniform_tile_spacing_flag;
			this.unitCol = state.unitCol;
			this.unitRow = state.unitRow;
			this.use_128x128_superblock = state.use_128x128_superblock;
			this.use_256x256_superblock = state.use_256x256_superblock;
			this.use_alt_group = state.use_alt_group;
			this.use_bank = state.use_bank;
			this.use_bru = state.use_bru;
			this.use_global_motion = state.use_global_motion;
			this.use_qtr_precision_mv = state.use_qtr_precision_mv;
			this.use_ref_frame_mvs = state.use_ref_frame_mvs;
			this.user_data_payload_byte = state.user_data_payload_byte;
			this.using_qmatrix = state.using_qmatrix;
			this.uuid_iso_iec_11578 = copy ? ((byte[])state.uuid_iso_iec_11578?.Clone()) : state.uuid_iso_iec_11578;
			this.uv_ac_delta_q_enabled = state.uv_ac_delta_q_enabled;
			this.uv_dc_delta_q_enabled = state.uv_dc_delta_q_enabled;
			this.v = state.v;
			this.varying_size_band_units_flag = state.varying_size_band_units_flag;
			this.vert_size_in_band_blocks_minus_1 = state.vert_size_in_band_blocks_minus_1;
			this.white_point_chromaticity_x = state.white_point_chromaticity_x;
			this.white_point_chromaticity_y = state.white_point_chromaticity_y;
			this.width_in_sbs_minus_1 = state.width_in_sbs_minus_1;
			this.wiener_ns_length = state.wiener_ns_length;
			this.wiener_ns_uv_sym = state.wiener_ns_uv_sym;
			this.xAId = state.xAId;
			this.xId = state.xId;
			this.xLId = state.xLId;
			this.xlayerId = state.xlayerId;
			this.y_dc_delta_q_enabled = state.y_dc_delta_q_enabled;
			this.zero_bit = state.zero_bit;
			LoadContextExtra(state);
		}

		partial void SaveContextExtra(ContextState state);
		partial void LoadContextExtra(ContextState state);

    }
}
