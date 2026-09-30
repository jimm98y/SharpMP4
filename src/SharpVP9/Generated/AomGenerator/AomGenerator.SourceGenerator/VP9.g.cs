using System;
using System.Collections.Generic;
using System.Numerics;
using SharpAVX;
using static SharpVP9.VP9Constants;

namespace SharpVP9
{

    public partial class VP9Context : IAomContext
    {
        /// <summary>
        /// Writing, the state the OBU's syntax elements were read into, and the same state as it was
        /// changed: an element whose value the two give alike is written as it was read.
        /// </summary>
        private VP9Context _original;
        private VP9Context _edited;

    /*
frame( sz ) { 
 startBitPos = get_position( )  
 uncompressed_header( )  
 trailing_bits( )  
 if ( header_size_in_bytes == 0 )  {  
  while ( get_position( ) < startBitPos + 8 * sz)  
   padding_bit f(1) 
  return  
 }  
 load_probs( frame_context_idx )  
 load_probs2( frame_context_idx )  
 clear_counts( )  
 init_bool( header_size_in_bytes )  
 compressed_header( )  
 exit_bool( )  
 endBitPos = get_position( )  
 headerBytes = (endBitPos - startBitPos) / 8  
 decode_tiles( sz - headerBytes )  
 refresh_probs( )  
}
    */
		private int sz;
		public int _Sz { get { return sz; } set { sz = value; } }
		private int padding_bit;
		public int _PaddingBit { get { return padding_bit; } set { padding_bit = value; } }

        private void Frame(int sz)
        {
			int startBitPos = 0;
			int endBitPos = 0;
			int headerBytes = 0;
			startBitPos = get_position();
			UncompressedHeader(); 
			TrailingBits(); 

			if ((header_size_in_bytes == 0))
			{

				while ((get_position() < (startBitPos + (8 * sz))))
				{
					stream.ReadFixed(1, out this.padding_bit, "padding_bit"); 
				}
				return;
			}
			load_probs(frame_context_idx); 
			load_probs2(frame_context_idx); 
			clear_counts(); 
			init_bool(header_size_in_bytes); 
			CompressedHeader(); 
			exit_bool(); 
			endBitPos = get_position();
			headerBytes = ((endBitPos - startBitPos) / 8);
			decode_tiles((sz - headerBytes)); 
			refresh_probs(); 
        }

        private void WriteFrame(int sz)
        {
			int startBitPos = 0;
			int endBitPos = 0;
			int headerBytes = 0;
			startBitPos = get_position();
			WriteUncompressedHeader(); 
			WriteTrailingBits(); 

			if ((header_size_in_bytes == 0))
			{

				while ((get_position() < (startBitPos + (8 * sz))))
				{
					this.padding_bit = stream.Pick("padding_bit", _original != null ? _original.padding_bit : this.padding_bit, _edited != null ? _edited.padding_bit : _original != null ? _original.padding_bit : this.padding_bit);
					stream.WriteFixed(1, this.padding_bit, "padding_bit"); 
				}
				return;
			}
			load_probs(frame_context_idx); 
			load_probs2(frame_context_idx); 
			clear_counts(); 
			init_bool(header_size_in_bytes); 
			WriteCompressedHeader(); 
			exit_bool(); 
			endBitPos = get_position();
			headerBytes = ((endBitPos - startBitPos) / 8);
			decode_tiles((sz - headerBytes)); 
			refresh_probs(); 
        }

    /*
trailing_bits() {
    while (get_position() & 7)  
  zero_bit f(1) 
}
    */
		private int zero_bit;
		public int _ZeroBit { get { return zero_bit; } set { zero_bit = value; } }

        private void TrailingBits()
        {

			while (((get_position() & 7) != 0))
			{
				stream.ReadFixed(1, out this.zero_bit, "zero_bit"); 
			}
        }

        private void WriteTrailingBits()
        {

			while (((get_position() & 7) != 0))
			{
				this.zero_bit = stream.Pick("zero_bit", _original != null ? _original.zero_bit : this.zero_bit, _edited != null ? _edited.zero_bit : _original != null ? _original.zero_bit : this.zero_bit);
				stream.WriteFixed(1, this.zero_bit, "zero_bit"); 
			}
        }

    /*
uncompressed_header() {
 frame_marker f(2) 
 profile_low_bit f(1) 
 profile_high_bit f(1)
    Profile = (profile_high_bit << 1) + profile_low_bit
    if (Profile == 3)  
  reserved_zero f(1) 
 show_existing_frame f(1)
    if (show_existing_frame == 1) {  
  frame_to_show_map_idx f(3)
        header_size_in_bytes = 0
        refresh_frame_flags = 0
        loop_filter_level = 0
        return
    }
    LastFrameType = frame_type  
 frame_type f(1) 
 show_frame f(1) 
 error_resilient_mode f(1)
    if (frame_type == KEY_FRAME) {
        frame_sync_code()
        color_config()
        frame_size()
        render_size()
        refresh_frame_flags = 0xFF
        FrameIsIntra = 1
    } else {
        if (show_frame == 0) {  
   intra_only f(1)
        } else {
            intra_only = 0
        }
        FrameIsIntra = intra_only
        if (error_resilient_mode == 0) {  
   reset_frame_context f(2)
        } else {
            reset_frame_context = 0
        }
        if (intra_only == 1) {
            frame_sync_code()
            if (Profile > 0) {
                color_config()
            } else {
                color_space = CS_BT_601
                subsampling_x = 1
                subsampling_y = 1
                BitDepth = 8
            }  
   refresh_frame_flags f(8)
            frame_size()
            render_size()
        } else {  
   refresh_frame_flags f(8)
            for (i = 0; i < 3; i++) {
                ref_frame_idx[i] f(3)
                ref_frame_sign_bias[LAST_FRAME + i] f(1)
            }
            frame_size_with_refs()  
   allow_high_precision_mv f(1)
            read_interpolation_filter()
        }
    }
    if (error_resilient_mode == 0) {  
  refresh_frame_context f(1) 
  frame_parallel_decoding_mode f(1)
    } else {
        refresh_frame_context = 0
        frame_parallel_decoding_mode = 1
    }  
 frame_context_idx f(2)
    if (FrameIsIntra || error_resilient_mode) {
        setup_past_independence()
        if (frame_type == KEY_FRAME || error_resilient_mode == 1
            || reset_frame_context == 3) {

            for (i = 0; i < 4; i++) {
                save_probs(i)
            }
        } else if (reset_frame_context == 2) {
            save_probs(frame_context_idx)
        }
        frame_context_idx = 0
    }
    loop_filter_params()
    quantization_params()
    segmentation_params()
    tile_info()
 header_size_in_bytes f(16)
}
    */
		private int frame_marker;
		public int _FrameMarker { get { return frame_marker; } set { frame_marker = value; } }
		private int profile_low_bit;
		public int _ProfileLowBit { get { return profile_low_bit; } set { profile_low_bit = value; } }
		private int profile_high_bit;
		public int _ProfileHighBit { get { return profile_high_bit; } set { profile_high_bit = value; } }
		private int Profile;
		public int _Profile { get { return Profile; } set { Profile = value; } }
		private int reserved_zero;
		public int _ReservedZero { get { return reserved_zero; } set { reserved_zero = value; } }
		private int show_existing_frame;
		public int _ShowExistingFrame { get { return show_existing_frame; } set { show_existing_frame = value; } }
		private int frame_to_show_map_idx;
		public int _FrameToShowMapIdx { get { return frame_to_show_map_idx; } set { frame_to_show_map_idx = value; } }
		private int header_size_in_bytes;
		public int _HeaderSizeInBytes { get { return header_size_in_bytes; } set { header_size_in_bytes = value; } }
		private int refresh_frame_flags;
		public int _RefreshFrameFlags { get { return refresh_frame_flags; } set { refresh_frame_flags = value; } }
		private int loop_filter_level;
		public int _LoopFilterLevel { get { return loop_filter_level; } set { loop_filter_level = value; } }
		private int LastFrameType;
		public int _LastFrameType { get { return LastFrameType; } set { LastFrameType = value; } }
		private int frame_type;
		public int _FrameType { get { return frame_type; } set { frame_type = value; } }
		private int show_frame;
		public int _ShowFrame { get { return show_frame; } set { show_frame = value; } }
		private int error_resilient_mode;
		public int _ErrorResilientMode { get { return error_resilient_mode; } set { error_resilient_mode = value; } }
		private int FrameIsIntra;
		public int _FrameIsIntra { get { return FrameIsIntra; } set { FrameIsIntra = value; } }
		private int intra_only;
		public int _IntraOnly { get { return intra_only; } set { intra_only = value; } }
		private int reset_frame_context;
		public int _ResetFrameContext { get { return reset_frame_context; } set { reset_frame_context = value; } }
		private int color_space;
		public int _ColorSpace { get { return color_space; } set { color_space = value; } }
		private int subsampling_x;
		public int _Subsamplingx { get { return subsampling_x; } set { subsampling_x = value; } }
		private int subsampling_y;
		public int _Subsamplingy { get { return subsampling_y; } set { subsampling_y = value; } }
		private int BitDepth;
		public int _BitDepth { get { return BitDepth; } set { BitDepth = value; } }
		private AomArray<int> ref_frame_idx = new AomArray<int>();
		public AomArray<int> _RefFrameIdx { get { return ref_frame_idx; } set { ref_frame_idx = value; } }
		private AomArray<int> ref_frame_sign_bias = new AomArray<int>();
		public AomArray<int> _RefFrameSignBias { get { return ref_frame_sign_bias; } set { ref_frame_sign_bias = value; } }
		private int allow_high_precision_mv;
		public int _AllowHighPrecisionMv { get { return allow_high_precision_mv; } set { allow_high_precision_mv = value; } }
		private int refresh_frame_context;
		public int _RefreshFrameContext { get { return refresh_frame_context; } set { refresh_frame_context = value; } }
		private int frame_parallel_decoding_mode;
		public int _FrameParallelDecodingMode { get { return frame_parallel_decoding_mode; } set { frame_parallel_decoding_mode = value; } }
		private int frame_context_idx;
		public int _FrameContextIdx { get { return frame_context_idx; } set { frame_context_idx = value; } }

        private void UncompressedHeader()
        {
			int i = 0;
			stream.ReadFixed(2, out this.frame_marker, "frame_marker"); 
			stream.ReadFixed(1, out this.profile_low_bit, "profile_low_bit"); 
			stream.ReadFixed(1, out this.profile_high_bit, "profile_high_bit"); 
			Profile = ((profile_high_bit << 1) + profile_low_bit);

			if ((Profile == 3))
			{
				stream.ReadFixed(1, out this.reserved_zero, "reserved_zero"); 
			}
			stream.ReadFixed(1, out this.show_existing_frame, "show_existing_frame"); 

			if ((show_existing_frame == 1))
			{
				stream.ReadFixed(3, out this.frame_to_show_map_idx, "frame_to_show_map_idx"); 
				header_size_in_bytes = 0;
				refresh_frame_flags = 0;
				loop_filter_level = 0;
				return;
			}
			LastFrameType = frame_type;
			stream.ReadFixed(1, out this.frame_type, "frame_type"); 
			stream.ReadFixed(1, out this.show_frame, "show_frame"); 
			stream.ReadFixed(1, out this.error_resilient_mode, "error_resilient_mode"); 

			if ((frame_type == KEY_FRAME))
			{
				FrameSyncCode(); 
				ColorConfig(); 
				FrameSize(); 
				RenderSize(); 
				refresh_frame_flags = 0xFF;
				FrameIsIntra = 1;
			}
			else 
			{

				if ((show_frame == 0))
				{
					stream.ReadFixed(1, out this.intra_only, "intra_only"); 
				}
				else 
				{
					intra_only = 0;
				}
				FrameIsIntra = intra_only;

				if ((error_resilient_mode == 0))
				{
					stream.ReadFixed(2, out this.reset_frame_context, "reset_frame_context"); 
				}
				else 
				{
					reset_frame_context = 0;
				}

				if ((intra_only == 1))
				{
					FrameSyncCode(); 

					if ((Profile > 0))
					{
						ColorConfig(); 
					}
					else 
					{
						color_space = CS_BT_601;
						subsampling_x = 1;
						subsampling_y = 1;
						BitDepth = 8;
					}
					stream.ReadFixed(8, out this.refresh_frame_flags, "refresh_frame_flags"); 
					FrameSize(); 
					RenderSize(); 
				}
				else 
				{
					stream.ReadFixed(8, out this.refresh_frame_flags, "refresh_frame_flags"); 

					for (i = 0; (i < 3); i++)
					{
						stream.ReadFixed(3, out this.ref_frame_idx[i], "ref_frame_idx"); 
						stream.ReadFixed(1, out this.ref_frame_sign_bias[(LAST_FRAME + i)], "ref_frame_sign_bias"); 
					}
					FrameSizeWithRefs(); 
					stream.ReadFixed(1, out this.allow_high_precision_mv, "allow_high_precision_mv"); 
					ReadInterpolationFilter(); 
				}
			}

			if ((error_resilient_mode == 0))
			{
				stream.ReadFixed(1, out this.refresh_frame_context, "refresh_frame_context"); 
				stream.ReadFixed(1, out this.frame_parallel_decoding_mode, "frame_parallel_decoding_mode"); 
			}
			else 
			{
				refresh_frame_context = 0;
				frame_parallel_decoding_mode = 1;
			}
			stream.ReadFixed(2, out this.frame_context_idx, "frame_context_idx"); 

			if (((FrameIsIntra != 0) || (error_resilient_mode != 0)))
			{
				setup_past_independence(); 

				if ((((frame_type == KEY_FRAME) || (error_resilient_mode == 1)) || (reset_frame_context == 3)))
				{

					for (i = 0; (i < 4); i++)
					{
						save_probs(i); 
					}
				}
				else if ((reset_frame_context == 2))
				{
					save_probs(frame_context_idx); 
				}
				frame_context_idx = 0;
			}
			LoopFilterParams(); 
			QuantizationParams(); 
			SegmentationParams(); 
			TileInfo(); 
			stream.ReadFixed(16, out this.header_size_in_bytes, "header_size_in_bytes"); 
        }

        private void WriteUncompressedHeader()
        {
			int i = 0;
			this.frame_marker = stream.Pick("frame_marker", _original != null ? _original.frame_marker : this.frame_marker, _edited != null ? _edited.frame_marker : _original != null ? _original.frame_marker : this.frame_marker);
			stream.WriteFixed(2, this.frame_marker, "frame_marker"); 
			this.profile_low_bit = stream.Pick("profile_low_bit", _original != null ? _original.profile_low_bit : this.profile_low_bit, _edited != null ? _edited.profile_low_bit : _original != null ? _original.profile_low_bit : this.profile_low_bit);
			stream.WriteFixed(1, this.profile_low_bit, "profile_low_bit"); 
			this.profile_high_bit = stream.Pick("profile_high_bit", _original != null ? _original.profile_high_bit : this.profile_high_bit, _edited != null ? _edited.profile_high_bit : _original != null ? _original.profile_high_bit : this.profile_high_bit);
			stream.WriteFixed(1, this.profile_high_bit, "profile_high_bit"); 
			Profile = ((profile_high_bit << 1) + profile_low_bit);

			if ((Profile == 3))
			{
				this.reserved_zero = stream.Pick("reserved_zero", _original != null ? _original.reserved_zero : this.reserved_zero, _edited != null ? _edited.reserved_zero : _original != null ? _original.reserved_zero : this.reserved_zero);
				stream.WriteFixed(1, this.reserved_zero, "reserved_zero"); 
			}
			this.show_existing_frame = stream.Pick("show_existing_frame", _original != null ? _original.show_existing_frame : this.show_existing_frame, _edited != null ? _edited.show_existing_frame : _original != null ? _original.show_existing_frame : this.show_existing_frame);
			stream.WriteFixed(1, this.show_existing_frame, "show_existing_frame"); 

			if ((show_existing_frame == 1))
			{
				this.frame_to_show_map_idx = stream.Pick("frame_to_show_map_idx", _original != null ? _original.frame_to_show_map_idx : this.frame_to_show_map_idx, _edited != null ? _edited.frame_to_show_map_idx : _original != null ? _original.frame_to_show_map_idx : this.frame_to_show_map_idx);
				stream.WriteFixed(3, this.frame_to_show_map_idx, "frame_to_show_map_idx"); 
				header_size_in_bytes = 0;
				refresh_frame_flags = 0;
				loop_filter_level = 0;
				return;
			}
			LastFrameType = frame_type;
			this.frame_type = stream.Pick("frame_type", _original != null ? _original.frame_type : this.frame_type, _edited != null ? _edited.frame_type : _original != null ? _original.frame_type : this.frame_type);
			stream.WriteFixed(1, this.frame_type, "frame_type"); 
			this.show_frame = stream.Pick("show_frame", _original != null ? _original.show_frame : this.show_frame, _edited != null ? _edited.show_frame : _original != null ? _original.show_frame : this.show_frame);
			stream.WriteFixed(1, this.show_frame, "show_frame"); 
			this.error_resilient_mode = stream.Pick("error_resilient_mode", _original != null ? _original.error_resilient_mode : this.error_resilient_mode, _edited != null ? _edited.error_resilient_mode : _original != null ? _original.error_resilient_mode : this.error_resilient_mode);
			stream.WriteFixed(1, this.error_resilient_mode, "error_resilient_mode"); 

			if ((frame_type == KEY_FRAME))
			{
				WriteFrameSyncCode(); 
				WriteColorConfig(); 
				WriteFrameSize(); 
				WriteRenderSize(); 
				refresh_frame_flags = 0xFF;
				FrameIsIntra = 1;
			}
			else 
			{

				if ((show_frame == 0))
				{
					this.intra_only = stream.Pick("intra_only", _original != null ? _original.intra_only : this.intra_only, _edited != null ? _edited.intra_only : _original != null ? _original.intra_only : this.intra_only);
					stream.WriteFixed(1, this.intra_only, "intra_only"); 
				}
				else 
				{
					intra_only = 0;
				}
				FrameIsIntra = intra_only;

				if ((error_resilient_mode == 0))
				{
					this.reset_frame_context = stream.Pick("reset_frame_context", _original != null ? _original.reset_frame_context : this.reset_frame_context, _edited != null ? _edited.reset_frame_context : _original != null ? _original.reset_frame_context : this.reset_frame_context);
					stream.WriteFixed(2, this.reset_frame_context, "reset_frame_context"); 
				}
				else 
				{
					reset_frame_context = 0;
				}

				if ((intra_only == 1))
				{
					WriteFrameSyncCode(); 

					if ((Profile > 0))
					{
						WriteColorConfig(); 
					}
					else 
					{
						color_space = CS_BT_601;
						subsampling_x = 1;
						subsampling_y = 1;
						BitDepth = 8;
					}
					this.refresh_frame_flags = stream.Pick("refresh_frame_flags", _original != null ? _original.refresh_frame_flags : this.refresh_frame_flags, _edited != null ? _edited.refresh_frame_flags : _original != null ? _original.refresh_frame_flags : this.refresh_frame_flags);
					stream.WriteFixed(8, this.refresh_frame_flags, "refresh_frame_flags"); 
					WriteFrameSize(); 
					WriteRenderSize(); 
				}
				else 
				{
					this.refresh_frame_flags = stream.Pick("refresh_frame_flags", _original != null ? _original.refresh_frame_flags : this.refresh_frame_flags, _edited != null ? _edited.refresh_frame_flags : _original != null ? _original.refresh_frame_flags : this.refresh_frame_flags);
					stream.WriteFixed(8, this.refresh_frame_flags, "refresh_frame_flags"); 

					for (i = 0; (i < 3); i++)
					{
						this.ref_frame_idx[i] = stream.Pick("ref_frame_idx", _original != null ? _original.ref_frame_idx[i] : this.ref_frame_idx[i], _edited != null ? _edited.ref_frame_idx[i] : _original != null ? _original.ref_frame_idx[i] : this.ref_frame_idx[i]);
						stream.WriteFixed(3, this.ref_frame_idx[i], "ref_frame_idx"); 
						this.ref_frame_sign_bias[(LAST_FRAME + i)] = stream.Pick("ref_frame_sign_bias", _original != null ? _original.ref_frame_sign_bias[(LAST_FRAME + i)] : this.ref_frame_sign_bias[(LAST_FRAME + i)], _edited != null ? _edited.ref_frame_sign_bias[(LAST_FRAME + i)] : _original != null ? _original.ref_frame_sign_bias[(LAST_FRAME + i)] : this.ref_frame_sign_bias[(LAST_FRAME + i)]);
						stream.WriteFixed(1, this.ref_frame_sign_bias[(LAST_FRAME + i)], "ref_frame_sign_bias"); 
					}
					WriteFrameSizeWithRefs(); 
					this.allow_high_precision_mv = stream.Pick("allow_high_precision_mv", _original != null ? _original.allow_high_precision_mv : this.allow_high_precision_mv, _edited != null ? _edited.allow_high_precision_mv : _original != null ? _original.allow_high_precision_mv : this.allow_high_precision_mv);
					stream.WriteFixed(1, this.allow_high_precision_mv, "allow_high_precision_mv"); 
					WriteReadInterpolationFilter(); 
				}
			}

			if ((error_resilient_mode == 0))
			{
				this.refresh_frame_context = stream.Pick("refresh_frame_context", _original != null ? _original.refresh_frame_context : this.refresh_frame_context, _edited != null ? _edited.refresh_frame_context : _original != null ? _original.refresh_frame_context : this.refresh_frame_context);
				stream.WriteFixed(1, this.refresh_frame_context, "refresh_frame_context"); 
				this.frame_parallel_decoding_mode = stream.Pick("frame_parallel_decoding_mode", _original != null ? _original.frame_parallel_decoding_mode : this.frame_parallel_decoding_mode, _edited != null ? _edited.frame_parallel_decoding_mode : _original != null ? _original.frame_parallel_decoding_mode : this.frame_parallel_decoding_mode);
				stream.WriteFixed(1, this.frame_parallel_decoding_mode, "frame_parallel_decoding_mode"); 
			}
			else 
			{
				refresh_frame_context = 0;
				frame_parallel_decoding_mode = 1;
			}
			this.frame_context_idx = stream.Pick("frame_context_idx", _original != null ? _original.frame_context_idx : this.frame_context_idx, _edited != null ? _edited.frame_context_idx : _original != null ? _original.frame_context_idx : this.frame_context_idx);
			stream.WriteFixed(2, this.frame_context_idx, "frame_context_idx"); 

			if (((FrameIsIntra != 0) || (error_resilient_mode != 0)))
			{
				setup_past_independence(); 

				if ((((frame_type == KEY_FRAME) || (error_resilient_mode == 1)) || (reset_frame_context == 3)))
				{

					for (i = 0; (i < 4); i++)
					{
						save_probs(i); 
					}
				}
				else if ((reset_frame_context == 2))
				{
					save_probs(frame_context_idx); 
				}
				frame_context_idx = 0;
			}
			WriteLoopFilterParams(); 
			WriteQuantizationParams(); 
			WriteSegmentationParams(); 
			WriteTileInfo(); 
			this.header_size_in_bytes = stream.Pick("header_size_in_bytes", _original != null ? _original.header_size_in_bytes : this.header_size_in_bytes, _edited != null ? _edited.header_size_in_bytes : _original != null ? _original.header_size_in_bytes : this.header_size_in_bytes);
			stream.WriteFixed(16, this.header_size_in_bytes, "header_size_in_bytes"); 
        }

    /*
frame_sync_code() {
 frame_sync_byte_0 f(8) 
 frame_sync_byte_1 f(8) 
 frame_sync_byte_2 f(8) 
}
    */
		private int frame_sync_byte_0;
		public int _FrameSyncByte0 { get { return frame_sync_byte_0; } set { frame_sync_byte_0 = value; } }
		private int frame_sync_byte_1;
		public int _FrameSyncByte1 { get { return frame_sync_byte_1; } set { frame_sync_byte_1 = value; } }
		private int frame_sync_byte_2;
		public int _FrameSyncByte2 { get { return frame_sync_byte_2; } set { frame_sync_byte_2 = value; } }

        private void FrameSyncCode()
        {
			stream.ReadFixed(8, out this.frame_sync_byte_0, "frame_sync_byte_0"); 
			stream.ReadFixed(8, out this.frame_sync_byte_1, "frame_sync_byte_1"); 
			stream.ReadFixed(8, out this.frame_sync_byte_2, "frame_sync_byte_2"); 
        }

        private void WriteFrameSyncCode()
        {
			this.frame_sync_byte_0 = stream.Pick("frame_sync_byte_0", _original != null ? _original.frame_sync_byte_0 : this.frame_sync_byte_0, _edited != null ? _edited.frame_sync_byte_0 : _original != null ? _original.frame_sync_byte_0 : this.frame_sync_byte_0);
			stream.WriteFixed(8, this.frame_sync_byte_0, "frame_sync_byte_0"); 
			this.frame_sync_byte_1 = stream.Pick("frame_sync_byte_1", _original != null ? _original.frame_sync_byte_1 : this.frame_sync_byte_1, _edited != null ? _edited.frame_sync_byte_1 : _original != null ? _original.frame_sync_byte_1 : this.frame_sync_byte_1);
			stream.WriteFixed(8, this.frame_sync_byte_1, "frame_sync_byte_1"); 
			this.frame_sync_byte_2 = stream.Pick("frame_sync_byte_2", _original != null ? _original.frame_sync_byte_2 : this.frame_sync_byte_2, _edited != null ? _edited.frame_sync_byte_2 : _original != null ? _original.frame_sync_byte_2 : this.frame_sync_byte_2);
			stream.WriteFixed(8, this.frame_sync_byte_2, "frame_sync_byte_2"); 
        }

    /*
color_config() {
    if (Profile >= 2) {  
  ten_or_twelve_bit f(1)
        BitDepth = ten_or_twelve_bit ? 12 : 10
    } else {
        BitDepth = 8
    }  
 color_space f(3)
    if (color_space != CS_RGB) {  
  color_range f(1)
        if (Profile == 1 || Profile == 3) {  
   subsampling_x f(1) 
   subsampling_y f(1) 
   reserved_zero f(1)
        } else {
            subsampling_x = 1
            subsampling_y = 1
        }
    } else {
        color_range = 1
        if (Profile == 1 || Profile == 3) {
            subsampling_x = 0
            subsampling_y = 0  
   reserved_zero f(1)
        }  
 }  
}
    */
		private int ten_or_twelve_bit;
		public int _TenOrTwelveBit { get { return ten_or_twelve_bit; } set { ten_or_twelve_bit = value; } }
		private int color_range;
		public int _ColorRange { get { return color_range; } set { color_range = value; } }

        private void ColorConfig()
        {

			if ((Profile >= 2))
			{
				stream.ReadFixed(1, out this.ten_or_twelve_bit, "ten_or_twelve_bit"); 
				BitDepth = ((ten_or_twelve_bit != 0) ? 12 : 10);
			}
			else 
			{
				BitDepth = 8;
			}
			stream.ReadFixed(3, out this.color_space, "color_space"); 

			if ((color_space != CS_RGB))
			{
				stream.ReadFixed(1, out this.color_range, "color_range"); 

				if (((Profile == 1) || (Profile == 3)))
				{
					stream.ReadFixed(1, out this.subsampling_x, "subsampling_x"); 
					stream.ReadFixed(1, out this.subsampling_y, "subsampling_y"); 
					stream.ReadFixed(1, out this.reserved_zero, "reserved_zero"); 
				}
				else 
				{
					subsampling_x = 1;
					subsampling_y = 1;
				}
			}
			else 
			{
				color_range = 1;

				if (((Profile == 1) || (Profile == 3)))
				{
					subsampling_x = 0;
					subsampling_y = 0;
					stream.ReadFixed(1, out this.reserved_zero, "reserved_zero"); 
				}
			}
        }

        private void WriteColorConfig()
        {

			if ((Profile >= 2))
			{
				this.ten_or_twelve_bit = stream.Pick("ten_or_twelve_bit", _original != null ? _original.ten_or_twelve_bit : this.ten_or_twelve_bit, _edited != null ? _edited.ten_or_twelve_bit : _original != null ? _original.ten_or_twelve_bit : this.ten_or_twelve_bit);
				stream.WriteFixed(1, this.ten_or_twelve_bit, "ten_or_twelve_bit"); 
				BitDepth = ((ten_or_twelve_bit != 0) ? 12 : 10);
			}
			else 
			{
				BitDepth = 8;
			}
			this.color_space = stream.Pick("color_space", _original != null ? _original.color_space : this.color_space, _edited != null ? _edited.color_space : _original != null ? _original.color_space : this.color_space);
			stream.WriteFixed(3, this.color_space, "color_space"); 

			if ((color_space != CS_RGB))
			{
				this.color_range = stream.Pick("color_range", _original != null ? _original.color_range : this.color_range, _edited != null ? _edited.color_range : _original != null ? _original.color_range : this.color_range);
				stream.WriteFixed(1, this.color_range, "color_range"); 

				if (((Profile == 1) || (Profile == 3)))
				{
					this.subsampling_x = stream.Pick("subsampling_x", _original != null ? _original.subsampling_x : this.subsampling_x, _edited != null ? _edited.subsampling_x : _original != null ? _original.subsampling_x : this.subsampling_x);
					stream.WriteFixed(1, this.subsampling_x, "subsampling_x"); 
					this.subsampling_y = stream.Pick("subsampling_y", _original != null ? _original.subsampling_y : this.subsampling_y, _edited != null ? _edited.subsampling_y : _original != null ? _original.subsampling_y : this.subsampling_y);
					stream.WriteFixed(1, this.subsampling_y, "subsampling_y"); 
					this.reserved_zero = stream.Pick("reserved_zero", _original != null ? _original.reserved_zero : this.reserved_zero, _edited != null ? _edited.reserved_zero : _original != null ? _original.reserved_zero : this.reserved_zero);
					stream.WriteFixed(1, this.reserved_zero, "reserved_zero"); 
				}
				else 
				{
					subsampling_x = 1;
					subsampling_y = 1;
				}
			}
			else 
			{
				color_range = 1;

				if (((Profile == 1) || (Profile == 3)))
				{
					subsampling_x = 0;
					subsampling_y = 0;
					this.reserved_zero = stream.Pick("reserved_zero", _original != null ? _original.reserved_zero : this.reserved_zero, _edited != null ? _edited.reserved_zero : _original != null ? _original.reserved_zero : this.reserved_zero);
					stream.WriteFixed(1, this.reserved_zero, "reserved_zero"); 
				}
			}
        }

    /*
frame_size() { 
 frame_width_minus_1  f(16) 
 frame_height_minus_1  f(16)
    FrameWidth = frame_width_minus_1 + 1
    FrameHeight = frame_height_minus_1 + 1 
 compute_image_size( )  
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
			stream.ReadFixed(16, out this.frame_width_minus_1, "frame_width_minus_1"); 
			stream.ReadFixed(16, out this.frame_height_minus_1, "frame_height_minus_1"); 
			FrameWidth = (frame_width_minus_1 + 1);
			FrameHeight = (frame_height_minus_1 + 1);
			ComputeImageSize(); 
        }

        private void WriteFrameSize()
        {
			this.frame_width_minus_1 = stream.Pick("frame_width_minus_1", _original != null ? _original.frame_width_minus_1 : this.frame_width_minus_1, _edited != null ? _edited.frame_width_minus_1 : _original != null ? _original.frame_width_minus_1 : this.frame_width_minus_1);
			stream.WriteFixed(16, this.frame_width_minus_1, "frame_width_minus_1"); 
			this.frame_height_minus_1 = stream.Pick("frame_height_minus_1", _original != null ? _original.frame_height_minus_1 : this.frame_height_minus_1, _edited != null ? _edited.frame_height_minus_1 : _original != null ? _original.frame_height_minus_1 : this.frame_height_minus_1);
			stream.WriteFixed(16, this.frame_height_minus_1, "frame_height_minus_1"); 
			FrameWidth = (frame_width_minus_1 + 1);
			FrameHeight = (frame_height_minus_1 + 1);
			ComputeImageSize(); 
        }

    /*
render_size() {
 render_and_frame_size_different f(1)
    if (render_and_frame_size_different == 1) {  
  render_width_minus_1 f(16) 
  render_height_minus_1 f(16)
        renderWidth = render_width_minus_1 + 1
        renderHeight = render_height_minus_1 + 1
    } else {
        renderWidth = FrameWidth
        renderHeight = FrameHeight  
 }  
}
    */
		private int render_and_frame_size_different;
		public int _RenderAndFrameSizeDifferent { get { return render_and_frame_size_different; } set { render_and_frame_size_different = value; } }
		private int render_width_minus_1;
		public int _RenderWidthMinus1 { get { return render_width_minus_1; } set { render_width_minus_1 = value; } }
		private int render_height_minus_1;
		public int _RenderHeightMinus1 { get { return render_height_minus_1; } set { render_height_minus_1 = value; } }
		private int renderWidth;
		public int _RenderWidth { get { return renderWidth; } set { renderWidth = value; } }
		private int renderHeight;
		public int _RenderHeight { get { return renderHeight; } set { renderHeight = value; } }

        private void RenderSize()
        {
			stream.ReadFixed(1, out this.render_and_frame_size_different, "render_and_frame_size_different"); 

			if ((render_and_frame_size_different == 1))
			{
				stream.ReadFixed(16, out this.render_width_minus_1, "render_width_minus_1"); 
				stream.ReadFixed(16, out this.render_height_minus_1, "render_height_minus_1"); 
				renderWidth = (render_width_minus_1 + 1);
				renderHeight = (render_height_minus_1 + 1);
			}
			else 
			{
				renderWidth = FrameWidth;
				renderHeight = FrameHeight;
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
				renderWidth = (render_width_minus_1 + 1);
				renderHeight = (render_height_minus_1 + 1);
			}
			else 
			{
				renderWidth = FrameWidth;
				renderHeight = FrameHeight;
			}
        }

    /*
frame_size_with_refs() {
    for (i = 0; i < 3; i++) {  
  found_ref f(1)
        if (found_ref == 1) {
            FrameWidth = RefFrameWidth[ref_frame_idx[i]]
            FrameHeight = RefFrameHeight[ref_frame_idx[i]]
            break
        }
    }
    if (found_ref == 0)
        frame_size()
    else
        compute_image_size()  
 render_size( )  
}
    */
		private int found_ref;
		public int _FoundRef { get { return found_ref; } set { found_ref = value; } }

        private void FrameSizeWithRefs()
        {
			int i = 0;

			for (i = 0; (i < 3); i++)
			{
				stream.ReadFixed(1, out this.found_ref, "found_ref"); 

				if ((found_ref == 1))
				{
					FrameWidth = RefFrameWidth[ref_frame_idx[i]];
					FrameHeight = RefFrameHeight[ref_frame_idx[i]];
					break;
				}
			}

			if ((found_ref == 0))
			{
				FrameSize(); 
			}
			else 
			{
				ComputeImageSize(); 
			}
			RenderSize(); 
        }

        private void WriteFrameSizeWithRefs()
        {
			int i = 0;

			for (i = 0; (i < 3); i++)
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

			if ((found_ref == 0))
			{
				WriteFrameSize(); 
			}
			else 
			{
				ComputeImageSize(); 
			}
			WriteRenderSize(); 
        }

    /*
compute_image_size() {
    MiCols = (FrameWidth + 7) >> 3
    MiRows = (FrameHeight + 7) >> 3
    Sb64Cols = (MiCols + 7) >> 3  
 Sb64Rows = (MiRows + 7) >> 3  
}
    */
		private int MiCols;
		public int _MiCols { get { return MiCols; } set { MiCols = value; } }
		private int MiRows;
		public int _MiRows { get { return MiRows; } set { MiRows = value; } }
		private int Sb64Cols;
		public int _Sb64Cols { get { return Sb64Cols; } set { Sb64Cols = value; } }
		private int Sb64Rows;
		public int _Sb64Rows { get { return Sb64Rows; } set { Sb64Rows = value; } }

        private void ComputeImageSize()
        {
			MiCols = ((FrameWidth + 7) >> 3);
			MiRows = ((FrameHeight + 7) >> 3);
			Sb64Cols = ((MiCols + 7) >> 3);
			Sb64Rows = ((MiRows + 7) >> 3);
        }

    /*
read_interpolation_filter() { 
 is_filter_switchable f(1)
    if (is_filter_switchable == 1) {
        interpolation_filter = SWITCHABLE
    } else {  
  raw_interpolation_filter f(2)
        interpolation_filter = literal_to_type[raw_interpolation_filter]  
 }  
}
    */
		private int is_filter_switchable;
		public int _IsFilterSwitchable { get { return is_filter_switchable; } set { is_filter_switchable = value; } }
		private int interpolation_filter;
		public int _InterpolationFilter { get { return interpolation_filter; } set { interpolation_filter = value; } }
		private int raw_interpolation_filter;
		public int _RawInterpolationFilter { get { return raw_interpolation_filter; } set { raw_interpolation_filter = value; } }

        private void ReadInterpolationFilter()
        {
			stream.ReadFixed(1, out this.is_filter_switchable, "is_filter_switchable"); 

			if ((is_filter_switchable == 1))
			{
				interpolation_filter = SWITCHABLE;
			}
			else 
			{
				stream.ReadFixed(2, out this.raw_interpolation_filter, "raw_interpolation_filter"); 
				interpolation_filter = literal_to_type[raw_interpolation_filter];
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
				this.raw_interpolation_filter = stream.Pick("raw_interpolation_filter", _original != null ? _original.raw_interpolation_filter : this.raw_interpolation_filter, _edited != null ? _edited.raw_interpolation_filter : _original != null ? _original.raw_interpolation_filter : this.raw_interpolation_filter);
				stream.WriteFixed(2, this.raw_interpolation_filter, "raw_interpolation_filter"); 
				interpolation_filter = literal_to_type[raw_interpolation_filter];
			}
        }

    /*
loop_filter_params() {
 loop_filter_level f(6) 
 loop_filter_sharpness f(3) 
 loop_filter_delta_enabled f(1)
    if (loop_filter_delta_enabled == 1) {  
  loop_filter_delta_update f(1)
        if (loop_filter_delta_update == 1) {
            for (i = 0; i < 4; i++) {  
    update_ref_delta f(1)
                if (update_ref_delta == 1)
                    loop_filter_ref_deltas[i] s(6)
            }
            for (i = 0; i < 2; i++) {  
    update_mode_delta f(1)
                if (update_mode_delta == 1)
                    loop_filter_mode_deltas[i] s(6)
            }
        }
 }
}
    */
		private int loop_filter_sharpness;
		public int _LoopFilterSharpness { get { return loop_filter_sharpness; } set { loop_filter_sharpness = value; } }
		private int loop_filter_delta_enabled;
		public int _LoopFilterDeltaEnabled { get { return loop_filter_delta_enabled; } set { loop_filter_delta_enabled = value; } }
		private int loop_filter_delta_update;
		public int _LoopFilterDeltaUpdate { get { return loop_filter_delta_update; } set { loop_filter_delta_update = value; } }
		private int update_ref_delta;
		public int _UpdateRefDelta { get { return update_ref_delta; } set { update_ref_delta = value; } }
		private AomArray<int> loop_filter_ref_deltas = new AomArray<int>();
		public AomArray<int> _LoopFilterRefDeltas { get { return loop_filter_ref_deltas; } set { loop_filter_ref_deltas = value; } }
		private int update_mode_delta;
		public int _UpdateModeDelta { get { return update_mode_delta; } set { update_mode_delta = value; } }
		private AomArray<int> loop_filter_mode_deltas = new AomArray<int>();
		public AomArray<int> _LoopFilterModeDeltas { get { return loop_filter_mode_deltas; } set { loop_filter_mode_deltas = value; } }

        private void LoopFilterParams()
        {
			int i = 0;
			stream.ReadFixed(6, out this.loop_filter_level, "loop_filter_level"); 
			stream.ReadFixed(3, out this.loop_filter_sharpness, "loop_filter_sharpness"); 
			stream.ReadFixed(1, out this.loop_filter_delta_enabled, "loop_filter_delta_enabled"); 

			if ((loop_filter_delta_enabled == 1))
			{
				stream.ReadFixed(1, out this.loop_filter_delta_update, "loop_filter_delta_update"); 

				if ((loop_filter_delta_update == 1))
				{

					for (i = 0; (i < 4); i++)
					{
						stream.ReadFixed(1, out this.update_ref_delta, "update_ref_delta"); 

						if ((update_ref_delta == 1))
						{
							stream.ReadSignMagnitude(6, out this.loop_filter_ref_deltas[i], "loop_filter_ref_deltas"); 
						}
					}

					for (i = 0; (i < 2); i++)
					{
						stream.ReadFixed(1, out this.update_mode_delta, "update_mode_delta"); 

						if ((update_mode_delta == 1))
						{
							stream.ReadSignMagnitude(6, out this.loop_filter_mode_deltas[i], "loop_filter_mode_deltas"); 
						}
					}
				}
			}
        }

        private void WriteLoopFilterParams()
        {
			int i = 0;
			this.loop_filter_level = stream.Pick("loop_filter_level", _original != null ? _original.loop_filter_level : this.loop_filter_level, _edited != null ? _edited.loop_filter_level : _original != null ? _original.loop_filter_level : this.loop_filter_level);
			stream.WriteFixed(6, this.loop_filter_level, "loop_filter_level"); 
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

					for (i = 0; (i < 4); i++)
					{
						this.update_ref_delta = stream.Pick("update_ref_delta", _original != null ? (_original.loop_filter_ref_deltas[i] != loop_filter_ref_deltas[i] ? 1 : 0) : this.update_ref_delta, _edited != null ? (_edited.loop_filter_ref_deltas[i] != loop_filter_ref_deltas[i] ? 1 : 0) : _original != null ? (_original.loop_filter_ref_deltas[i] != loop_filter_ref_deltas[i] ? 1 : 0) : this.update_ref_delta);
						stream.WriteFixed(1, this.update_ref_delta, "update_ref_delta"); 

						if ((update_ref_delta == 1))
						{
							this.loop_filter_ref_deltas[i] = stream.Pick("loop_filter_ref_deltas", _original != null ? _original.loop_filter_ref_deltas[i] : this.loop_filter_ref_deltas[i], _edited != null ? _edited.loop_filter_ref_deltas[i] : _original != null ? _original.loop_filter_ref_deltas[i] : this.loop_filter_ref_deltas[i]);
							stream.WriteSignMagnitude(6, this.loop_filter_ref_deltas[i], "loop_filter_ref_deltas"); 
						}
					}

					for (i = 0; (i < 2); i++)
					{
						this.update_mode_delta = stream.Pick("update_mode_delta", _original != null ? (_original.loop_filter_mode_deltas[i] != loop_filter_mode_deltas[i] ? 1 : 0) : this.update_mode_delta, _edited != null ? (_edited.loop_filter_mode_deltas[i] != loop_filter_mode_deltas[i] ? 1 : 0) : _original != null ? (_original.loop_filter_mode_deltas[i] != loop_filter_mode_deltas[i] ? 1 : 0) : this.update_mode_delta);
						stream.WriteFixed(1, this.update_mode_delta, "update_mode_delta"); 

						if ((update_mode_delta == 1))
						{
							this.loop_filter_mode_deltas[i] = stream.Pick("loop_filter_mode_deltas", _original != null ? _original.loop_filter_mode_deltas[i] : this.loop_filter_mode_deltas[i], _edited != null ? _edited.loop_filter_mode_deltas[i] : _original != null ? _original.loop_filter_mode_deltas[i] : this.loop_filter_mode_deltas[i]);
							stream.WriteSignMagnitude(6, this.loop_filter_mode_deltas[i], "loop_filter_mode_deltas"); 
						}
					}
				}
			}
        }

    /*
quantization_params() {
 base_q_idx f(8)
    delta_q_y_dc = read_delta_q()
    delta_q_uv_dc = read_delta_q()
    delta_q_uv_ac = read_delta_q()
    Lossless = base_q_idx == 0 && delta_q_y_dc == 0 && delta_q_uv_dc == 0 && delta_q_uv_ac == 0 
}
    */
		private int base_q_idx;
		public int _BaseqIdx { get { return base_q_idx; } set { base_q_idx = value; } }
		private int delta_q_y_dc;
		public int _DeltaqyDc { get { return delta_q_y_dc; } set { delta_q_y_dc = value; } }
		private int delta_q_uv_dc;
		public int _DeltaqUvDc { get { return delta_q_uv_dc; } set { delta_q_uv_dc = value; } }
		private int delta_q_uv_ac;
		public int _DeltaqUvAc { get { return delta_q_uv_ac; } set { delta_q_uv_ac = value; } }
		private int Lossless;
		public int _Lossless { get { return Lossless; } set { Lossless = value; } }

        private void QuantizationParams()
        {
			stream.ReadFixed(8, out this.base_q_idx, "base_q_idx"); 
			delta_q_y_dc = ReadDeltaq();
			delta_q_uv_dc = ReadDeltaq();
			delta_q_uv_ac = ReadDeltaq();
			Lossless = (((((base_q_idx == 0) && (delta_q_y_dc == 0)) && (delta_q_uv_dc == 0)) && (delta_q_uv_ac == 0)) ? 1 : 0);
        }

        private void WriteQuantizationParams()
        {
			this.base_q_idx = stream.Pick("base_q_idx", _original != null ? _original.base_q_idx : this.base_q_idx, _edited != null ? _edited.base_q_idx : _original != null ? _original.base_q_idx : this.base_q_idx);
			stream.WriteFixed(8, this.base_q_idx, "base_q_idx"); 
			delta_q_y_dc = WriteReadDeltaq();
			delta_q_uv_dc = WriteReadDeltaq();
			delta_q_uv_ac = WriteReadDeltaq();
			Lossless = (((((base_q_idx == 0) && (delta_q_y_dc == 0)) && (delta_q_uv_dc == 0)) && (delta_q_uv_ac == 0)) ? 1 : 0);
        }

    /*
read_delta_q() {
 delta_coded f(1)
    if (delta_coded) {  
  delta_q s(4)
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
				stream.ReadSignMagnitude(4, out this.delta_q, "delta_q"); 
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
				stream.WriteSignMagnitude(4, this.delta_q, "delta_q"); 
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
    if (segmentation_enabled == 1) {  
  segmentation_update_map f(1)
        if (segmentation_update_map == 1) {
            for (i = 0; i < 7; i++)
                segmentation_tree_probs[i] = read_prob()  
   segmentation_temporal_update f(1)
            for (i = 0; i < 3; i++)
                segmentation_pred_prob[i] = segmentation_temporal_update ?
                    read_prob() : 255


        }  
  segmentation_update_data f(1)
        if (segmentation_update_data == 1) {  
   segmentation_abs_or_delta_update f(1)
            for (i = 0; i < MAX_SEGMENTS; i++) {
                for (j = 0; j < SEG_LVL_MAX; j++) {
                    feature_value = 0  
     feature_enabled f(1)
                    FeatureEnabled[i][j] = feature_enabled
                    if (feature_enabled == 1) {
                        bits_to_read = segmentation_feature_bits[j]  
      feature_value f(bits_to_read)
                        if (segmentation_feature_signed[j] == 1) {  
       feature_sign f(1)
                            if (feature_sign == 1)
                                feature_value *= -1
                        }
                    }
                    FeatureData[i][j] = feature_value
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
		private AomArray<int> segmentation_tree_probs = new AomArray<int>();
		public AomArray<int> _SegmentationTreeProbs { get { return segmentation_tree_probs; } set { segmentation_tree_probs = value; } }
		private int segmentation_temporal_update;
		public int _SegmentationTemporalUpdate { get { return segmentation_temporal_update; } set { segmentation_temporal_update = value; } }
		private AomArray<int> segmentation_pred_prob = new AomArray<int>();
		public AomArray<int> _SegmentationPredProb { get { return segmentation_pred_prob; } set { segmentation_pred_prob = value; } }
		private int segmentation_update_data;
		public int _SegmentationUpdateData { get { return segmentation_update_data; } set { segmentation_update_data = value; } }
		private int segmentation_abs_or_delta_update;
		public int _SegmentationAbsOrDeltaUpdate { get { return segmentation_abs_or_delta_update; } set { segmentation_abs_or_delta_update = value; } }
		private int feature_value;
		public int _FeatureValue { get { return feature_value; } set { feature_value = value; } }
		private int feature_enabled;
		public int _FeatureEnabled { get { return feature_enabled; } set { feature_enabled = value; } }
		private AomArray<AomArray<int>> FeatureEnabled = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> __FeatureEnabled { get { return FeatureEnabled; } set { FeatureEnabled = value; } }
		private int bits_to_read;
		public int _BitsToRead { get { return bits_to_read; } set { bits_to_read = value; } }
		private int feature_sign;
		public int _FeatureSign { get { return feature_sign; } set { feature_sign = value; } }
		private AomArray<AomArray<int>> FeatureData = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _FeatureData { get { return FeatureData; } set { FeatureData = value; } }

        private void SegmentationParams()
        {
			int i = 0;
			int j = 0;
			stream.ReadFixed(1, out this.segmentation_enabled, "segmentation_enabled"); 

			if ((segmentation_enabled == 1))
			{
				stream.ReadFixed(1, out this.segmentation_update_map, "segmentation_update_map"); 

				if ((segmentation_update_map == 1))
				{

					for (i = 0; (i < 7); i++)
					{
						segmentation_tree_probs[i] = ReadProb();
					}
					stream.ReadFixed(1, out this.segmentation_temporal_update, "segmentation_temporal_update"); 

					for (i = 0; (i < 3); i++)
					{
						segmentation_pred_prob[i] = ((segmentation_temporal_update != 0) ? ReadProb() : 255);
					}
				}
				stream.ReadFixed(1, out this.segmentation_update_data, "segmentation_update_data"); 

				if ((segmentation_update_data == 1))
				{
					stream.ReadFixed(1, out this.segmentation_abs_or_delta_update, "segmentation_abs_or_delta_update"); 

					for (i = 0; (i < MAX_SEGMENTS); i++)
					{

						for (j = 0; (j < SEG_LVL_MAX); j++)
						{
							feature_value = 0;
							stream.ReadFixed(1, out this.feature_enabled, "feature_enabled"); 
							FeatureEnabled[i][j] = feature_enabled;

							if ((feature_enabled == 1))
							{
								bits_to_read = segmentation_feature_bits[j];
								stream.ReadVariable(bits_to_read, out this.feature_value, "feature_value"); 

								if ((segmentation_feature_signed[j] == 1))
								{
									stream.ReadFixed(1, out this.feature_sign, "feature_sign"); 

									if ((feature_sign == 1))
									{
										feature_value *= -1;
									}
								}
							}
							FeatureData[i][j] = feature_value;
						}
					}
				}
			}
        }

        private void WriteSegmentationParams()
        {
			int i = 0;
			int j = 0;
			this.segmentation_enabled = stream.Pick("segmentation_enabled", _original != null ? _original.segmentation_enabled : this.segmentation_enabled, _edited != null ? _edited.segmentation_enabled : _original != null ? _original.segmentation_enabled : this.segmentation_enabled);
			stream.WriteFixed(1, this.segmentation_enabled, "segmentation_enabled"); 

			if ((segmentation_enabled == 1))
			{
				this.segmentation_update_map = stream.Pick("segmentation_update_map", _original != null ? _original.segmentation_update_map : this.segmentation_update_map, _edited != null ? _edited.segmentation_update_map : _original != null ? _original.segmentation_update_map : this.segmentation_update_map);
				stream.WriteFixed(1, this.segmentation_update_map, "segmentation_update_map"); 

				if ((segmentation_update_map == 1))
				{

					for (i = 0; (i < 7); i++)
					{
						segmentation_tree_probs[i] = WriteReadProb();
					}
					this.segmentation_temporal_update = stream.Pick("segmentation_temporal_update", _original != null ? _original.segmentation_temporal_update : this.segmentation_temporal_update, _edited != null ? _edited.segmentation_temporal_update : _original != null ? _original.segmentation_temporal_update : this.segmentation_temporal_update);
					stream.WriteFixed(1, this.segmentation_temporal_update, "segmentation_temporal_update"); 

					for (i = 0; (i < 3); i++)
					{
						segmentation_pred_prob[i] = ((segmentation_temporal_update != 0) ? WriteReadProb() : 255);
					}
				}
				this.segmentation_update_data = stream.Pick("segmentation_update_data", _original != null ? _original.segmentation_update_data : this.segmentation_update_data, _edited != null ? _edited.segmentation_update_data : _original != null ? _original.segmentation_update_data : this.segmentation_update_data);
				stream.WriteFixed(1, this.segmentation_update_data, "segmentation_update_data"); 

				if ((segmentation_update_data == 1))
				{
					this.segmentation_abs_or_delta_update = stream.Pick("segmentation_abs_or_delta_update", _original != null ? _original.segmentation_abs_or_delta_update : this.segmentation_abs_or_delta_update, _edited != null ? _edited.segmentation_abs_or_delta_update : _original != null ? _original.segmentation_abs_or_delta_update : this.segmentation_abs_or_delta_update);
					stream.WriteFixed(1, this.segmentation_abs_or_delta_update, "segmentation_abs_or_delta_update"); 

					for (i = 0; (i < MAX_SEGMENTS); i++)
					{

						for (j = 0; (j < SEG_LVL_MAX); j++)
						{
							feature_value = 0;
							this.feature_enabled = stream.Pick("feature_enabled", _original != null ? _original.FeatureEnabled[i][j] : this.feature_enabled, _edited != null ? _edited.FeatureEnabled[i][j] : _original != null ? _original.FeatureEnabled[i][j] : this.feature_enabled);
							stream.WriteFixed(1, this.feature_enabled, "feature_enabled"); 
							FeatureEnabled[i][j] = feature_enabled;

							if ((feature_enabled == 1))
							{
								bits_to_read = segmentation_feature_bits[j];
								this.feature_value = stream.Pick("feature_value", _original != null ? Abs(_original.FeatureData[i][j]) : this.feature_value, _edited != null ? Abs(_edited.FeatureData[i][j]) : _original != null ? Abs(_original.FeatureData[i][j]) : this.feature_value);
								stream.WriteVariable(bits_to_read, this.feature_value, "feature_value"); 

								if ((segmentation_feature_signed[j] == 1))
								{
									this.feature_sign = stream.Pick("feature_sign", _original != null ? (_original.FeatureData[i][j] < 0 ? 1 : 0) : this.feature_sign, _edited != null ? (_edited.FeatureData[i][j] < 0 ? 1 : 0) : _original != null ? (_original.FeatureData[i][j] < 0 ? 1 : 0) : this.feature_sign);
									stream.WriteFixed(1, this.feature_sign, "feature_sign"); 

									if ((feature_sign == 1))
									{
										feature_value *= -1;
									}
								}
							}
							FeatureData[i][j] = feature_value;
						}
					}
				}
			}
        }

    /*
read_prob() {
 prob_coded f(1)
    if (prob_coded) {  
  prob f(8)
    } else {
        prob = 255
    }  
 return prob  
}
    */
		private int prob_coded;
		public int _ProbCoded { get { return prob_coded; } set { prob_coded = value; } }
		private int prob;
		public int _Prob { get { return prob; } set { prob = value; } }

        private int ReadProb()
        {
			stream.ReadFixed(1, out this.prob_coded, "prob_coded"); 

			if ((prob_coded != 0))
			{
				stream.ReadFixed(8, out this.prob, "prob"); 
			}
			else 
			{
				prob = 255;
			}
			return prob;
        }

        private int WriteReadProb()
        {
			this.prob_coded = stream.Pick("prob_coded", _original != null ? _original.prob_coded : this.prob_coded, _edited != null ? _edited.prob_coded : _original != null ? _original.prob_coded : this.prob_coded);
			stream.WriteFixed(1, this.prob_coded, "prob_coded"); 

			if ((prob_coded != 0))
			{
				this.prob = stream.Pick("prob", _original != null ? _original.prob : this.prob, _edited != null ? _edited.prob : _original != null ? _original.prob : this.prob);
				stream.WriteFixed(8, this.prob, "prob"); 
			}
			else 
			{
				prob = 255;
			}
			return prob;
        }

    /*
tile_info() {
    minLog2TileCols = calc_min_log2_tile_cols()
    maxLog2TileCols = calc_max_log2_tile_cols()
    tile_cols_log2 = minLog2TileCols
    while (tile_cols_log2 < maxLog2TileCols) {  
  increment_tile_cols_log2 f(1)
        if (increment_tile_cols_log2 == 1)
            tile_cols_log2++
        else
            break
    }  
 tile_rows_log2 f(1)
    if (tile_rows_log2 == 1) {  
  increment_tile_rows_log2 f(1)
        tile_rows_log2 += increment_tile_rows_log2  
 }  
}
    */
		private int tile_cols_log2;
		public int _TileColsLog2 { get { return tile_cols_log2; } set { tile_cols_log2 = value; } }
		private int increment_tile_cols_log2;
		public int _IncrementTileColsLog2 { get { return increment_tile_cols_log2; } set { increment_tile_cols_log2 = value; } }
		private int tile_rows_log2;
		public int _TileRowsLog2 { get { return tile_rows_log2; } set { tile_rows_log2 = value; } }
		private int increment_tile_rows_log2;
		public int _IncrementTileRowsLog2 { get { return increment_tile_rows_log2; } set { increment_tile_rows_log2 = value; } }

        private void TileInfo()
        {
			int minLog2TileCols = 0;
			int maxLog2TileCols = 0;
			minLog2TileCols = CalcMinLog2TileCols();
			maxLog2TileCols = CalcMaxLog2TileCols();
			tile_cols_log2 = minLog2TileCols;

			while ((tile_cols_log2 < maxLog2TileCols))
			{
				stream.ReadFixed(1, out this.increment_tile_cols_log2, "increment_tile_cols_log2"); 

				if ((increment_tile_cols_log2 == 1))
				{
					tile_cols_log2++;
				}
				else 
				{
					break;
				}
			}
			stream.ReadFixed(1, out this.tile_rows_log2, "tile_rows_log2"); 

			if ((tile_rows_log2 == 1))
			{
				stream.ReadFixed(1, out this.increment_tile_rows_log2, "increment_tile_rows_log2"); 
				tile_rows_log2 += increment_tile_rows_log2;
			}
        }

        private void WriteTileInfo()
        {
			int minLog2TileCols = 0;
			int maxLog2TileCols = 0;
			minLog2TileCols = CalcMinLog2TileCols();
			maxLog2TileCols = CalcMaxLog2TileCols();
			tile_cols_log2 = minLog2TileCols;

			while ((tile_cols_log2 < maxLog2TileCols))
			{
				this.increment_tile_cols_log2 = stream.Pick("increment_tile_cols_log2", _original != null ? (_original.tile_cols_log2 > tile_cols_log2 ? 1 : 0) : this.increment_tile_cols_log2, _edited != null ? (_edited.tile_cols_log2 > tile_cols_log2 ? 1 : 0) : _original != null ? (_original.tile_cols_log2 > tile_cols_log2 ? 1 : 0) : this.increment_tile_cols_log2);
				stream.WriteFixed(1, this.increment_tile_cols_log2, "increment_tile_cols_log2"); 

				if ((increment_tile_cols_log2 == 1))
				{
					tile_cols_log2++;
				}
				else 
				{
					break;
				}
			}
			this.tile_rows_log2 = stream.Pick("tile_rows_log2", _original != null ? (_original.tile_rows_log2 > 0 ? 1 : 0) : this.tile_rows_log2, _edited != null ? (_edited.tile_rows_log2 > 0 ? 1 : 0) : _original != null ? (_original.tile_rows_log2 > 0 ? 1 : 0) : this.tile_rows_log2);
			stream.WriteFixed(1, this.tile_rows_log2, "tile_rows_log2"); 

			if ((tile_rows_log2 == 1))
			{
				this.increment_tile_rows_log2 = stream.Pick("increment_tile_rows_log2", _original != null ? (_original.tile_rows_log2 > 1 ? 1 : 0) : this.increment_tile_rows_log2, _edited != null ? (_edited.tile_rows_log2 > 1 ? 1 : 0) : _original != null ? (_original.tile_rows_log2 > 1 ? 1 : 0) : this.increment_tile_rows_log2);
				stream.WriteFixed(1, this.increment_tile_rows_log2, "increment_tile_rows_log2"); 
				tile_rows_log2 += increment_tile_rows_log2;
			}
        }

    /*
calc_min_log2_tile_cols() {
    minLog2 = 0
    while ((MAX_TILE_WIDTH_B64 << minLog2) < Sb64Cols)
        minLog2++
 return minLog2  
}
    */

        private int CalcMinLog2TileCols()
        {
			int minLog2 = 0;
			minLog2 = 0;

			while (((MAX_TILE_WIDTH_B64 << minLog2) < Sb64Cols))
			{
				minLog2++;
			}
			return minLog2;
        }

    /*
calc_max_log2_tile_cols() {
    maxLog2 = 1
    while ((Sb64Cols >> maxLog2) >= MIN_TILE_WIDTH_B64)
        maxLog2++  
 return maxLog2 - 1  
}
    */

        private int CalcMaxLog2TileCols()
        {
			int maxLog2 = 0;
			maxLog2 = 1;

			while (((Sb64Cols >> maxLog2) >= MIN_TILE_WIDTH_B64))
			{
				maxLog2++;
			}
			return (maxLog2 - 1);
        }

    /*
compressed_header() {
    read_tx_mode()
    if (tx_mode == TX_MODE_SELECT) {
        tx_mode_probs()
    }
    read_coef_probs()
    read_skip_prob()
    if (FrameIsIntra == 0) {
        read_inter_mode_probs()
        if (interpolation_filter == SWITCHABLE)
            read_interp_filter_probs()
        read_is_inter_probs()
        frame_reference_mode()
        frame_reference_mode_probs()
        read_y_mode_probs()
        read_partition_probs()
        mv_probs()  
 }  
}
    */

        private void CompressedHeader()
        {
			ReadTxMode(); 

			if ((tx_mode == TX_MODE_SELECT))
			{
				TxModeProbs(); 
			}
			ReadCoefProbs(); 
			ReadSkipProb(); 

			if ((FrameIsIntra == 0))
			{
				ReadInterModeProbs(); 

				if ((interpolation_filter == SWITCHABLE))
				{
					ReadInterpFilterProbs(); 
				}
				ReadIsInterProbs(); 
				FrameReferenceMode(); 
				FrameReferenceModeProbs(); 
				ReadyModeProbs(); 
				ReadPartitionProbs(); 
				MvProbs(); 
			}
        }

        private void WriteCompressedHeader()
        {
			WriteReadTxMode(); 

			if ((tx_mode == TX_MODE_SELECT))
			{
				WriteTxModeProbs(); 
			}
			WriteReadCoefProbs(); 
			WriteReadSkipProb(); 

			if ((FrameIsIntra == 0))
			{
				WriteReadInterModeProbs(); 

				if ((interpolation_filter == SWITCHABLE))
				{
					WriteReadInterpFilterProbs(); 
				}
				WriteReadIsInterProbs(); 
				WriteFrameReferenceMode(); 
				WriteFrameReferenceModeProbs(); 
				WriteReadyModeProbs(); 
				WriteReadPartitionProbs(); 
				WriteMvProbs(); 
			}
        }

    /*
read_tx_mode() {
    if (Lossless == 1) {
        tx_mode = ONLY_4X4
    } else {  
  tx_mode L(2)
        if (tx_mode == ALLOW_32X32) {  
   tx_mode_select L(1)
            tx_mode += tx_mode_select
        }  
 }  
}
    */
		private int tx_mode;
		public int _TxMode { get { return tx_mode; } set { tx_mode = value; } }
		private int tx_mode_select;
		public int _TxModeSelect { get { return tx_mode_select; } set { tx_mode_select = value; } }

        private void ReadTxMode()
        {

			if ((Lossless == 1))
			{
				tx_mode = ONLY_4X4;
			}
			else 
			{
				stream.ReadLiteral(2, out this.tx_mode, "tx_mode"); 

				if ((tx_mode == ALLOW_32X32))
				{
					stream.ReadLiteral(1, out this.tx_mode_select, "tx_mode_select"); 
					tx_mode += tx_mode_select;
				}
			}
        }

        private void WriteReadTxMode()
        {

			if ((Lossless == 1))
			{
				tx_mode = ONLY_4X4;
			}
			else 
			{
				this.tx_mode = stream.Pick("tx_mode", _original != null ? Min(_original.tx_mode, ALLOW_32X32) : this.tx_mode, _edited != null ? Min(_edited.tx_mode, ALLOW_32X32) : _original != null ? Min(_original.tx_mode, ALLOW_32X32) : this.tx_mode);
				stream.WriteLiteral(2, this.tx_mode, "tx_mode"); 

				if ((tx_mode == ALLOW_32X32))
				{
					this.tx_mode_select = stream.Pick("tx_mode_select", _original != null ? (_original.tx_mode == TX_MODE_SELECT ? 1 : 0) : this.tx_mode_select, _edited != null ? (_edited.tx_mode == TX_MODE_SELECT ? 1 : 0) : _original != null ? (_original.tx_mode == TX_MODE_SELECT ? 1 : 0) : this.tx_mode_select);
					stream.WriteLiteral(1, this.tx_mode_select, "tx_mode_select"); 
					tx_mode += tx_mode_select;
				}
			}
        }

    /*
tx_mode_probs() {
    for (i = 0; i < TX_SIZE_CONTEXTS; i++)
        for (j = 0; j < TX_SIZES - 3; j++)
            tx_probs_8x8[i][j] = diff_update_prob(tx_probs_8x8[i][j])
    for (i = 0; i < TX_SIZE_CONTEXTS; i++)
        for (j = 0; j < TX_SIZES - 2; j++)
            tx_probs_16x16[i][j] = diff_update_prob(tx_probs_16x16[i][j])
    for (i = 0; i < TX_SIZE_CONTEXTS; i++)
        for (j = 0; j < TX_SIZES - 1; j++)
   tx_probs_32x32[ i ][ j ] = diff_update_prob( tx_probs_32x32[ i ][ j ] )
}
    */
		private AomArray<AomArray<int>> tx_probs_8x8 = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _TxProbs8x8 { get { return tx_probs_8x8; } set { tx_probs_8x8 = value; } }
		private AomArray<AomArray<int>> tx_probs_16x16 = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _TxProbs16x16 { get { return tx_probs_16x16; } set { tx_probs_16x16 = value; } }
		private AomArray<AomArray<int>> tx_probs_32x32 = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _TxProbs32x32 { get { return tx_probs_32x32; } set { tx_probs_32x32 = value; } }

        private void TxModeProbs()
        {
			int i = 0;
			int j = 0;

			for (i = 0; (i < TX_SIZE_CONTEXTS); i++)
			{

				for (j = 0; (j < (TX_SIZES - 3)); j++)
				{
					tx_probs_8x8[i][j] = DiffUpdateProb(tx_probs_8x8[i][j]);
				}
			}

			for (i = 0; (i < TX_SIZE_CONTEXTS); i++)
			{

				for (j = 0; (j < (TX_SIZES - 2)); j++)
				{
					tx_probs_16x16[i][j] = DiffUpdateProb(tx_probs_16x16[i][j]);
				}
			}

			for (i = 0; (i < TX_SIZE_CONTEXTS); i++)
			{

				for (j = 0; (j < (TX_SIZES - 1)); j++)
				{
					tx_probs_32x32[i][j] = DiffUpdateProb(tx_probs_32x32[i][j]);
				}
			}
        }

        private void WriteTxModeProbs()
        {
			int i = 0;
			int j = 0;

			for (i = 0; (i < TX_SIZE_CONTEXTS); i++)
			{

				for (j = 0; (j < (TX_SIZES - 3)); j++)
				{
					tx_probs_8x8[i][j] = WriteDiffUpdateProb(tx_probs_8x8[i][j]);
				}
			}

			for (i = 0; (i < TX_SIZE_CONTEXTS); i++)
			{

				for (j = 0; (j < (TX_SIZES - 2)); j++)
				{
					tx_probs_16x16[i][j] = WriteDiffUpdateProb(tx_probs_16x16[i][j]);
				}
			}

			for (i = 0; (i < TX_SIZE_CONTEXTS); i++)
			{

				for (j = 0; (j < (TX_SIZES - 1)); j++)
				{
					tx_probs_32x32[i][j] = WriteDiffUpdateProb(tx_probs_32x32[i][j]);
				}
			}
        }

    /*
diff_update_prob(prob) {
 update_prob B(252)
    if (update_prob == 1) {
        deltaProb = decode_term_subexp()
        prob = inv_remap_prob(deltaProb, prob)
    }  
 return prob  
}
    */
		private int update_prob;
		public int _UpdateProb { get { return update_prob; } set { update_prob = value; } }

        private int DiffUpdateProb(int prob)
        {
			int deltaProb = 0;
			stream.ReadBool(252, out this.update_prob, "update_prob"); 

			if ((update_prob == 1))
			{
				deltaProb = DecodeTermSubexp();
				prob = InvRemapProb(deltaProb, prob);
			}
			return prob;
        }

        private int WriteDiffUpdateProb(int prob)
        {
			int deltaProb = 0;
			this.update_prob = stream.Pick("update_prob", _original != null ? _original.update_prob : this.update_prob, _edited != null ? _edited.update_prob : _original != null ? _original.update_prob : this.update_prob);
			stream.WriteBool(252, this.update_prob, "update_prob"); 

			if ((update_prob == 1))
			{
				deltaProb = WriteDecodeTermSubexp();
				prob = InvRemapProb(deltaProb, prob);
			}
			return prob;
        }

    /*
decode_term_subexp() {
 bit L(1)
    if (bit == 0) {  
  sub_exp_val L(4)
        return sub_exp_val
    }  
 bit L(1)
    if (bit == 0) {  
  sub_exp_val_minus_16 L(4)
        return sub_exp_val_minus_16 + 16
    }  
 bit L(1)
    if (bit == 0) {  
  sub_exp_val_minus_32 L(5)
        return sub_exp_val_minus_32 + 32
    }  
 v L(7)
    if (v < 65)
        return v + 64  
 bit L(1)
return (v << 1) - 1 + bit  
}
    */
		private int bit;
		public int _Bit { get { return bit; } set { bit = value; } }
		private int sub_exp_val;
		public int _SubExpVal { get { return sub_exp_val; } set { sub_exp_val = value; } }
		private int sub_exp_val_minus_16;
		public int _SubExpValMinus16 { get { return sub_exp_val_minus_16; } set { sub_exp_val_minus_16 = value; } }
		private int sub_exp_val_minus_32;
		public int _SubExpValMinus32 { get { return sub_exp_val_minus_32; } set { sub_exp_val_minus_32 = value; } }
		private int v;

        private int DecodeTermSubexp()
        {
			stream.ReadLiteral(1, out this.bit, "bit"); 

			if ((bit == 0))
			{
				stream.ReadLiteral(4, out this.sub_exp_val, "sub_exp_val"); 
				return sub_exp_val;
			}
			stream.ReadLiteral(1, out this.bit, "bit"); 

			if ((bit == 0))
			{
				stream.ReadLiteral(4, out this.sub_exp_val_minus_16, "sub_exp_val_minus_16"); 
				return (sub_exp_val_minus_16 + 16);
			}
			stream.ReadLiteral(1, out this.bit, "bit"); 

			if ((bit == 0))
			{
				stream.ReadLiteral(5, out this.sub_exp_val_minus_32, "sub_exp_val_minus_32"); 
				return (sub_exp_val_minus_32 + 32);
			}
			stream.ReadLiteral(7, out this.v, "v"); 

			if ((v < 65))
			{
				return (v + 64);
			}
			stream.ReadLiteral(1, out this.bit, "bit"); 
			return (((v << 1) - 1) + bit);
        }

        private int WriteDecodeTermSubexp()
        {
			this.bit = stream.Pick("bit", _original != null ? _original.bit : this.bit, _edited != null ? _edited.bit : _original != null ? _original.bit : this.bit);
			stream.WriteLiteral(1, this.bit, "bit"); 

			if ((bit == 0))
			{
				this.sub_exp_val = stream.Pick("sub_exp_val", _original != null ? _original.sub_exp_val : this.sub_exp_val, _edited != null ? _edited.sub_exp_val : _original != null ? _original.sub_exp_val : this.sub_exp_val);
				stream.WriteLiteral(4, this.sub_exp_val, "sub_exp_val"); 
				return sub_exp_val;
			}
			this.bit = stream.Pick("bit", _original != null ? _original.bit : this.bit, _edited != null ? _edited.bit : _original != null ? _original.bit : this.bit);
			stream.WriteLiteral(1, this.bit, "bit"); 

			if ((bit == 0))
			{
				this.sub_exp_val_minus_16 = stream.Pick("sub_exp_val_minus_16", _original != null ? _original.sub_exp_val_minus_16 : this.sub_exp_val_minus_16, _edited != null ? _edited.sub_exp_val_minus_16 : _original != null ? _original.sub_exp_val_minus_16 : this.sub_exp_val_minus_16);
				stream.WriteLiteral(4, this.sub_exp_val_minus_16, "sub_exp_val_minus_16"); 
				return (sub_exp_val_minus_16 + 16);
			}
			this.bit = stream.Pick("bit", _original != null ? _original.bit : this.bit, _edited != null ? _edited.bit : _original != null ? _original.bit : this.bit);
			stream.WriteLiteral(1, this.bit, "bit"); 

			if ((bit == 0))
			{
				this.sub_exp_val_minus_32 = stream.Pick("sub_exp_val_minus_32", _original != null ? _original.sub_exp_val_minus_32 : this.sub_exp_val_minus_32, _edited != null ? _edited.sub_exp_val_minus_32 : _original != null ? _original.sub_exp_val_minus_32 : this.sub_exp_val_minus_32);
				stream.WriteLiteral(5, this.sub_exp_val_minus_32, "sub_exp_val_minus_32"); 
				return (sub_exp_val_minus_32 + 32);
			}
			this.v = stream.Pick("v", _original != null ? _original.v : this.v, _edited != null ? _edited.v : _original != null ? _original.v : this.v);
			stream.WriteLiteral(7, this.v, "v"); 

			if ((v < 65))
			{
				return (v + 64);
			}
			this.bit = stream.Pick("bit", _original != null ? _original.bit : this.bit, _edited != null ? _edited.bit : _original != null ? _original.bit : this.bit);
			stream.WriteLiteral(1, this.bit, "bit"); 
			return (((v << 1) - 1) + bit);
        }

    /*
inv_remap_prob(deltaProb, prob) {
    m = prob
    v = deltaProb
    v = inv_map_table[v]
    m--
    if ((m << 1) <= 255)
        m = 1 + inv_recenter_nonneg(v, m)
    else
        m = 255 - inv_recenter_nonneg(v, 255 - 1 - m)
return m  
}
    */
		private int deltaProb;
		public int _DeltaProb { get { return deltaProb; } set { deltaProb = value; } }

        private int InvRemapProb(int deltaProb, int prob)
        {
			int m = 0;
			m = prob;
			v = deltaProb;
			v = inv_map_table[v];
			m--;

			if (((m << 1) <= 255))
			{
				m = (1 + InvRecenterNonneg(v, m));
			}
			else 
			{
				m = (255 - InvRecenterNonneg(v, ((255 - 1) - m)));
			}
			return m;
        }

    /*
inv_recenter_nonneg(v, m) {
    if (v > 2 * m)
        return v
    if (v & 1)
        return m - ((v + 1) >> 1)  
return m + (v >> 1)
}
    */
		private int m;

        private int InvRecenterNonneg(int v, int m)
        {

			if ((v > (2 * m)))
			{
				return v;
			}

			if (((v & 1) != 0))
			{
				return (m - ((v + 1) >> 1));
			}
			return (m + (v >> 1));
        }

    /*
read_coef_probs() {
    maxTxSize = tx_mode_to_biggest_tx_size[tx_mode]
    for (txSz = TX_4X4; txSz <= maxTxSize; txSz++) {  
  update_probs L(1)
        if (update_probs == 1)
            for (i = 0; i < 2; i++)
                for (j = 0; j < 2; j++)
                    for (k = 0; k < 6; k++) {
                        maxL = (k == 0) ? 3 : 6
                        for (l = 0; l < maxL; l++)
                            for (m = 0; m < 3; m++)
                                coef_probs[txSz][i][j][k][l][m] =
                                    diff_update_prob(coef_probs[txSz][i][j][k][l][m])

                    }  
 }  
}
    */
		private int update_probs;
		public int _UpdateProbs { get { return update_probs; } set { update_probs = value; } }
		private AomArray<AomArray<AomArray<AomArray<AomArray<AomArray<int>>>>>> coef_probs = new AomArray<AomArray<AomArray<AomArray<AomArray<AomArray<int>>>>>>(() => new AomArray<AomArray<AomArray<AomArray<AomArray<int>>>>>(() => new AomArray<AomArray<AomArray<AomArray<int>>>>(() => new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>())))));
		public AomArray<AomArray<AomArray<AomArray<AomArray<AomArray<int>>>>>> _CoefProbs { get { return coef_probs; } set { coef_probs = value; } }

        private void ReadCoefProbs()
        {
			int txSz = 0;
			int i = 0;
			int j = 0;
			int k = 0;
			int l = 0;
			int m = 0;
			int maxTxSize = 0;
			int maxL = 0;
			maxTxSize = tx_mode_to_biggest_tx_size[tx_mode];

			for (txSz = TX_4X4; (txSz <= maxTxSize); txSz++)
			{
				stream.ReadLiteral(1, out this.update_probs, "update_probs"); 

				if ((update_probs == 1))
				{

					for (i = 0; (i < 2); i++)
					{

						for (j = 0; (j < 2); j++)
						{

							for (k = 0; (k < 6); k++)
							{
								maxL = ((k == 0) ? 3 : 6);

								for (l = 0; (l < maxL); l++)
								{

									for (m = 0; (m < 3); m++)
									{
										coef_probs[txSz][i][j][k][l][m] = DiffUpdateProb(coef_probs[txSz][i][j][k][l][m]);
									}
								}
							}
						}
					}
				}
			}
        }

        private void WriteReadCoefProbs()
        {
			int txSz = 0;
			int i = 0;
			int j = 0;
			int k = 0;
			int l = 0;
			int m = 0;
			int maxTxSize = 0;
			int maxL = 0;
			maxTxSize = tx_mode_to_biggest_tx_size[tx_mode];

			for (txSz = TX_4X4; (txSz <= maxTxSize); txSz++)
			{
				this.update_probs = stream.Pick("update_probs", _original != null ? _original.update_probs : this.update_probs, _edited != null ? _edited.update_probs : _original != null ? _original.update_probs : this.update_probs);
				stream.WriteLiteral(1, this.update_probs, "update_probs"); 

				if ((update_probs == 1))
				{

					for (i = 0; (i < 2); i++)
					{

						for (j = 0; (j < 2); j++)
						{

							for (k = 0; (k < 6); k++)
							{
								maxL = ((k == 0) ? 3 : 6);

								for (l = 0; (l < maxL); l++)
								{

									for (m = 0; (m < 3); m++)
									{
										coef_probs[txSz][i][j][k][l][m] = WriteDiffUpdateProb(coef_probs[txSz][i][j][k][l][m]);
									}
								}
							}
						}
					}
				}
			}
        }

    /*
read_skip_prob() {
    for (i = 0; i < SKIP_CONTEXTS; i++)  
  skip_prob[ i ]  = diff_update_prob( skip_prob[ i ] )  
}
    */
		private AomArray<int> skip_prob = new AomArray<int>();
		public AomArray<int> _SkipProb { get { return skip_prob; } set { skip_prob = value; } }

        private void ReadSkipProb()
        {
			int i = 0;

			for (i = 0; (i < SKIP_CONTEXTS); i++)
			{
				skip_prob[i] = DiffUpdateProb(skip_prob[i]);
			}
        }

        private void WriteReadSkipProb()
        {
			int i = 0;

			for (i = 0; (i < SKIP_CONTEXTS); i++)
			{
				skip_prob[i] = WriteDiffUpdateProb(skip_prob[i]);
			}
        }

    /*
read_inter_mode_probs() {
    for (i = 0; i < INTER_MODE_CONTEXTS; i++)
        for (j = 0; j < INTER_MODES - 1; j++)  
   inter_mode_probs[ i ][ j ] = diff_update_prob( inter_mode_probs[ i ][ j ] )  
}
    */
		private AomArray<AomArray<int>> inter_mode_probs = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _InterModeProbs { get { return inter_mode_probs; } set { inter_mode_probs = value; } }

        private void ReadInterModeProbs()
        {
			int i = 0;
			int j = 0;

			for (i = 0; (i < INTER_MODE_CONTEXTS); i++)
			{

				for (j = 0; (j < (INTER_MODES - 1)); j++)
				{
					inter_mode_probs[i][j] = DiffUpdateProb(inter_mode_probs[i][j]);
				}
			}
        }

        private void WriteReadInterModeProbs()
        {
			int i = 0;
			int j = 0;

			for (i = 0; (i < INTER_MODE_CONTEXTS); i++)
			{

				for (j = 0; (j < (INTER_MODES - 1)); j++)
				{
					inter_mode_probs[i][j] = WriteDiffUpdateProb(inter_mode_probs[i][j]);
				}
			}
        }

    /*
read_interp_filter_probs() {
    for (j = 0; j < INTERP_FILTER_CONTEXTS; j++)
        for (i = 0; i < SWITCHABLE_FILTERS - 1; i++)  
   interp_filter_probs[ j ][ i ] = diff_update_prob( interp_filter_probs[ j ][ i ] )  
}
    */
		private AomArray<AomArray<int>> interp_filter_probs = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _InterpFilterProbs { get { return interp_filter_probs; } set { interp_filter_probs = value; } }

        private void ReadInterpFilterProbs()
        {
			int j = 0;
			int i = 0;

			for (j = 0; (j < INTERP_FILTER_CONTEXTS); j++)
			{

				for (i = 0; (i < (SWITCHABLE_FILTERS - 1)); i++)
				{
					interp_filter_probs[j][i] = DiffUpdateProb(interp_filter_probs[j][i]);
				}
			}
        }

        private void WriteReadInterpFilterProbs()
        {
			int j = 0;
			int i = 0;

			for (j = 0; (j < INTERP_FILTER_CONTEXTS); j++)
			{

				for (i = 0; (i < (SWITCHABLE_FILTERS - 1)); i++)
				{
					interp_filter_probs[j][i] = WriteDiffUpdateProb(interp_filter_probs[j][i]);
				}
			}
        }

    /*
read_is_inter_probs() {
    for (i = 0; i < IS_INTER_CONTEXTS; i++)
 is_inter_prob[ i ] = diff_update_prob( is_inter_prob[ i ] )  
}
    */
		private AomArray<int> is_inter_prob = new AomArray<int>();
		public AomArray<int> _IsInterProb { get { return is_inter_prob; } set { is_inter_prob = value; } }

        private void ReadIsInterProbs()
        {
			int i = 0;

			for (i = 0; (i < IS_INTER_CONTEXTS); i++)
			{
				is_inter_prob[i] = DiffUpdateProb(is_inter_prob[i]);
			}
        }

        private void WriteReadIsInterProbs()
        {
			int i = 0;

			for (i = 0; (i < IS_INTER_CONTEXTS); i++)
			{
				is_inter_prob[i] = WriteDiffUpdateProb(is_inter_prob[i]);
			}
        }

    /*
frame_reference_mode() {
    compoundReferenceAllowed = 0
    for (i = 1; i < REFS_PER_FRAME; i++)
        if (ref_frame_sign_bias[i + 1] != ref_frame_sign_bias[1])
            compoundReferenceAllowed = 1
    if (compoundReferenceAllowed == 1) {  
  non_single_reference L(1)
        if (non_single_reference == 0) {
            reference_mode = SINGLE_REFERENCE
        } else {  
   reference_select L(1)
            if (reference_select == 0)
                reference_mode = COMPOUND_REFERENCE
            else
                reference_mode = REFERENCE_MODE_SELECT
            setup_compound_reference_mode()
        }
    } else {
        reference_mode = SINGLE_REFERENCE
 }
}
    */
		private int non_single_reference;
		public int _NonSingleReference { get { return non_single_reference; } set { non_single_reference = value; } }
		private int reference_mode;
		public int _ReferenceMode { get { return reference_mode; } set { reference_mode = value; } }
		private int reference_select;
		public int _ReferenceSelect { get { return reference_select; } set { reference_select = value; } }

        private void FrameReferenceMode()
        {
			int i = 0;
			int compoundReferenceAllowed = 0;
			compoundReferenceAllowed = 0;

			for (i = 1; (i < REFS_PER_FRAME); i++)
			{

				if ((ref_frame_sign_bias[(i + 1)] != ref_frame_sign_bias[1]))
				{
					compoundReferenceAllowed = 1;
				}
			}

			if ((compoundReferenceAllowed == 1))
			{
				stream.ReadLiteral(1, out this.non_single_reference, "non_single_reference"); 

				if ((non_single_reference == 0))
				{
					reference_mode = SINGLE_REFERENCE;
				}
				else 
				{
					stream.ReadLiteral(1, out this.reference_select, "reference_select"); 

					if ((reference_select == 0))
					{
						reference_mode = COMPOUND_REFERENCE;
					}
					else 
					{
						reference_mode = REFERENCE_MODE_SELECT;
					}
					SetupCompoundReferenceMode(); 
				}
			}
			else 
			{
				reference_mode = SINGLE_REFERENCE;
			}
        }

        private void WriteFrameReferenceMode()
        {
			int i = 0;
			int compoundReferenceAllowed = 0;
			compoundReferenceAllowed = 0;

			for (i = 1; (i < REFS_PER_FRAME); i++)
			{

				if ((ref_frame_sign_bias[(i + 1)] != ref_frame_sign_bias[1]))
				{
					compoundReferenceAllowed = 1;
				}
			}

			if ((compoundReferenceAllowed == 1))
			{
				this.non_single_reference = stream.Pick("non_single_reference", _original != null ? _original.non_single_reference : this.non_single_reference, _edited != null ? _edited.non_single_reference : _original != null ? _original.non_single_reference : this.non_single_reference);
				stream.WriteLiteral(1, this.non_single_reference, "non_single_reference"); 

				if ((non_single_reference == 0))
				{
					reference_mode = SINGLE_REFERENCE;
				}
				else 
				{
					this.reference_select = stream.Pick("reference_select", _original != null ? _original.reference_select : this.reference_select, _edited != null ? _edited.reference_select : _original != null ? _original.reference_select : this.reference_select);
					stream.WriteLiteral(1, this.reference_select, "reference_select"); 

					if ((reference_select == 0))
					{
						reference_mode = COMPOUND_REFERENCE;
					}
					else 
					{
						reference_mode = REFERENCE_MODE_SELECT;
					}
					SetupCompoundReferenceMode(); 
				}
			}
			else 
			{
				reference_mode = SINGLE_REFERENCE;
			}
        }

    /*
frame_reference_mode_probs() {
    if (reference_mode == REFERENCE_MODE_SELECT) {
        for (i = 0; i < COMP_MODE_CONTEXTS; i++)
            comp_mode_prob[i] = diff_update_prob(comp_mode_prob[i])
    }
    if (reference_mode != COMPOUND_REFERENCE) {
        for (i = 0; i < REF_CONTEXTS; i++) {
            single_ref_prob[i][0] = diff_update_prob(single_ref_prob[i][0])
            single_ref_prob[i][1] = diff_update_prob(single_ref_prob[i][1])
        }
    }
    if (reference_mode != SINGLE_REFERENCE) {
        for (i = 0; i < REF_CONTEXTS; i++)
            comp_ref_prob[i] = diff_update_prob(comp_ref_prob[i])  
 }  
}
    */
		private AomArray<int> comp_mode_prob = new AomArray<int>();
		public AomArray<int> _CompModeProb { get { return comp_mode_prob; } set { comp_mode_prob = value; } }
		private AomArray<AomArray<int>> single_ref_prob = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _SingleRefProb { get { return single_ref_prob; } set { single_ref_prob = value; } }
		private AomArray<int> comp_ref_prob = new AomArray<int>();
		public AomArray<int> _CompRefProb { get { return comp_ref_prob; } set { comp_ref_prob = value; } }

        private void FrameReferenceModeProbs()
        {
			int i = 0;

			if ((reference_mode == REFERENCE_MODE_SELECT))
			{

				for (i = 0; (i < COMP_MODE_CONTEXTS); i++)
				{
					comp_mode_prob[i] = DiffUpdateProb(comp_mode_prob[i]);
				}
			}

			if ((reference_mode != COMPOUND_REFERENCE))
			{

				for (i = 0; (i < REF_CONTEXTS); i++)
				{
					single_ref_prob[i][0] = DiffUpdateProb(single_ref_prob[i][0]);
					single_ref_prob[i][1] = DiffUpdateProb(single_ref_prob[i][1]);
				}
			}

			if ((reference_mode != SINGLE_REFERENCE))
			{

				for (i = 0; (i < REF_CONTEXTS); i++)
				{
					comp_ref_prob[i] = DiffUpdateProb(comp_ref_prob[i]);
				}
			}
        }

        private void WriteFrameReferenceModeProbs()
        {
			int i = 0;

			if ((reference_mode == REFERENCE_MODE_SELECT))
			{

				for (i = 0; (i < COMP_MODE_CONTEXTS); i++)
				{
					comp_mode_prob[i] = WriteDiffUpdateProb(comp_mode_prob[i]);
				}
			}

			if ((reference_mode != COMPOUND_REFERENCE))
			{

				for (i = 0; (i < REF_CONTEXTS); i++)
				{
					single_ref_prob[i][0] = WriteDiffUpdateProb(single_ref_prob[i][0]);
					single_ref_prob[i][1] = WriteDiffUpdateProb(single_ref_prob[i][1]);
				}
			}

			if ((reference_mode != SINGLE_REFERENCE))
			{

				for (i = 0; (i < REF_CONTEXTS); i++)
				{
					comp_ref_prob[i] = WriteDiffUpdateProb(comp_ref_prob[i]);
				}
			}
        }

    /*
read_y_mode_probs() {
    for (i = 0; i < BLOCK_SIZE_GROUPS; i++)
        for (j = 0; j < INTRA_MODES - 1; j++)  
   y_mode_probs[ i ][ j ] = diff_update_prob( y_mode_probs[ i ][ j ] )  
}
    */
		private AomArray<AomArray<int>> y_mode_probs = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _yModeProbs { get { return y_mode_probs; } set { y_mode_probs = value; } }

        private void ReadyModeProbs()
        {
			int i = 0;
			int j = 0;

			for (i = 0; (i < BLOCK_SIZE_GROUPS); i++)
			{

				for (j = 0; (j < (INTRA_MODES - 1)); j++)
				{
					y_mode_probs[i][j] = DiffUpdateProb(y_mode_probs[i][j]);
				}
			}
        }

        private void WriteReadyModeProbs()
        {
			int i = 0;
			int j = 0;

			for (i = 0; (i < BLOCK_SIZE_GROUPS); i++)
			{

				for (j = 0; (j < (INTRA_MODES - 1)); j++)
				{
					y_mode_probs[i][j] = WriteDiffUpdateProb(y_mode_probs[i][j]);
				}
			}
        }

    /*
read_partition_probs() {
    for (i = 0; i < PARTITION_CONTEXTS; i++)
        for (j = 0; j < PARTITION_TYPES - 1; j++)
   partition_probs[ i ][ j ] = diff_update_prob( partition_probs[ i ][ j ] )
}
    */
		private AomArray<AomArray<int>> partition_probs = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _PartitionProbs { get { return partition_probs; } set { partition_probs = value; } }

        private void ReadPartitionProbs()
        {
			int i = 0;
			int j = 0;

			for (i = 0; (i < PARTITION_CONTEXTS); i++)
			{

				for (j = 0; (j < (PARTITION_TYPES - 1)); j++)
				{
					partition_probs[i][j] = DiffUpdateProb(partition_probs[i][j]);
				}
			}
        }

        private void WriteReadPartitionProbs()
        {
			int i = 0;
			int j = 0;

			for (i = 0; (i < PARTITION_CONTEXTS); i++)
			{

				for (j = 0; (j < (PARTITION_TYPES - 1)); j++)
				{
					partition_probs[i][j] = WriteDiffUpdateProb(partition_probs[i][j]);
				}
			}
        }

    /*
mv_probs() {
    for (j = 0; j < MV_JOINTS - 1; j++)
        mv_joint_probs[j] = update_mv_prob(mv_joint_probs[j])
    for (i = 0; i < 2; i++) {
        mv_sign_prob[i] = update_mv_prob(mv_sign_prob[i])
        for (j = 0; j < MV_CLASSES - 1; j++)
            mv_class_probs[i][j] = update_mv_prob(mv_class_probs[i][j])
        mv_class0_bit_prob[i] = update_mv_prob(mv_class0_bit_prob[i])
        for (j = 0; j < MV_OFFSET_BITS; j++)
            mv_bits_prob[i][j] = update_mv_prob(mv_bits_prob[i][j])
    }
    for (i = 0; i < 2; i++) {
        for (j = 0; j < CLASS0_SIZE; j++)
            for (k = 0; k < MV_FR_SIZE - 1; k++)
                mv_class0_fr_probs[i][j][k] = update_mv_prob(mv_class0_fr_probs[i][j][k])
        for (k = 0; k < MV_FR_SIZE - 1; k++)
            mv_fr_probs[i][k] = update_mv_prob(mv_fr_probs[i][k])
    }
    if (allow_high_precision_mv) {
        for (i = 0; i < 2; i++) {
            mv_class0_hp_prob[i] = update_mv_prob(mv_class0_hp_prob[i])
            mv_hp_prob[i] = update_mv_prob(mv_hp_prob[i])
        }  
 }  
}
    */
		private AomArray<int> mv_joint_probs = new AomArray<int>();
		public AomArray<int> _MvJointProbs { get { return mv_joint_probs; } set { mv_joint_probs = value; } }
		private AomArray<int> mv_sign_prob = new AomArray<int>();
		public AomArray<int> _MvSignProb { get { return mv_sign_prob; } set { mv_sign_prob = value; } }
		private AomArray<AomArray<int>> mv_class_probs = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _MvClassProbs { get { return mv_class_probs; } set { mv_class_probs = value; } }
		private AomArray<int> mv_class0_bit_prob = new AomArray<int>();
		public AomArray<int> _MvClass0BitProb { get { return mv_class0_bit_prob; } set { mv_class0_bit_prob = value; } }
		private AomArray<AomArray<int>> mv_bits_prob = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _MvBitsProb { get { return mv_bits_prob; } set { mv_bits_prob = value; } }
		private AomArray<AomArray<AomArray<int>>> mv_class0_fr_probs = new AomArray<AomArray<AomArray<int>>>(() => new AomArray<AomArray<int>>(() => new AomArray<int>()));
		public AomArray<AomArray<AomArray<int>>> _MvClass0FrProbs { get { return mv_class0_fr_probs; } set { mv_class0_fr_probs = value; } }
		private AomArray<AomArray<int>> mv_fr_probs = new AomArray<AomArray<int>>(() => new AomArray<int>());
		public AomArray<AomArray<int>> _MvFrProbs { get { return mv_fr_probs; } set { mv_fr_probs = value; } }
		private AomArray<int> mv_class0_hp_prob = new AomArray<int>();
		public AomArray<int> _MvClass0HpProb { get { return mv_class0_hp_prob; } set { mv_class0_hp_prob = value; } }
		private AomArray<int> mv_hp_prob = new AomArray<int>();
		public AomArray<int> _MvHpProb { get { return mv_hp_prob; } set { mv_hp_prob = value; } }

        private void MvProbs()
        {
			int j = 0;
			int i = 0;
			int k = 0;

			for (j = 0; (j < (MV_JOINTS - 1)); j++)
			{
				mv_joint_probs[j] = UpdateMvProb(mv_joint_probs[j]);
			}

			for (i = 0; (i < 2); i++)
			{
				mv_sign_prob[i] = UpdateMvProb(mv_sign_prob[i]);

				for (j = 0; (j < (MV_CLASSES - 1)); j++)
				{
					mv_class_probs[i][j] = UpdateMvProb(mv_class_probs[i][j]);
				}
				mv_class0_bit_prob[i] = UpdateMvProb(mv_class0_bit_prob[i]);

				for (j = 0; (j < MV_OFFSET_BITS); j++)
				{
					mv_bits_prob[i][j] = UpdateMvProb(mv_bits_prob[i][j]);
				}
			}

			for (i = 0; (i < 2); i++)
			{

				for (j = 0; (j < CLASS0_SIZE); j++)
				{

					for (k = 0; (k < (MV_FR_SIZE - 1)); k++)
					{
						mv_class0_fr_probs[i][j][k] = UpdateMvProb(mv_class0_fr_probs[i][j][k]);
					}
				}

				for (k = 0; (k < (MV_FR_SIZE - 1)); k++)
				{
					mv_fr_probs[i][k] = UpdateMvProb(mv_fr_probs[i][k]);
				}
			}

			if ((allow_high_precision_mv != 0))
			{

				for (i = 0; (i < 2); i++)
				{
					mv_class0_hp_prob[i] = UpdateMvProb(mv_class0_hp_prob[i]);
					mv_hp_prob[i] = UpdateMvProb(mv_hp_prob[i]);
				}
			}
        }

        private void WriteMvProbs()
        {
			int j = 0;
			int i = 0;
			int k = 0;

			for (j = 0; (j < (MV_JOINTS - 1)); j++)
			{
				mv_joint_probs[j] = WriteUpdateMvProb(mv_joint_probs[j]);
			}

			for (i = 0; (i < 2); i++)
			{
				mv_sign_prob[i] = WriteUpdateMvProb(mv_sign_prob[i]);

				for (j = 0; (j < (MV_CLASSES - 1)); j++)
				{
					mv_class_probs[i][j] = WriteUpdateMvProb(mv_class_probs[i][j]);
				}
				mv_class0_bit_prob[i] = WriteUpdateMvProb(mv_class0_bit_prob[i]);

				for (j = 0; (j < MV_OFFSET_BITS); j++)
				{
					mv_bits_prob[i][j] = WriteUpdateMvProb(mv_bits_prob[i][j]);
				}
			}

			for (i = 0; (i < 2); i++)
			{

				for (j = 0; (j < CLASS0_SIZE); j++)
				{

					for (k = 0; (k < (MV_FR_SIZE - 1)); k++)
					{
						mv_class0_fr_probs[i][j][k] = WriteUpdateMvProb(mv_class0_fr_probs[i][j][k]);
					}
				}

				for (k = 0; (k < (MV_FR_SIZE - 1)); k++)
				{
					mv_fr_probs[i][k] = WriteUpdateMvProb(mv_fr_probs[i][k]);
				}
			}

			if ((allow_high_precision_mv != 0))
			{

				for (i = 0; (i < 2); i++)
				{
					mv_class0_hp_prob[i] = WriteUpdateMvProb(mv_class0_hp_prob[i]);
					mv_hp_prob[i] = WriteUpdateMvProb(mv_hp_prob[i]);
				}
			}
        }

    /*
update_mv_prob(prob) { 
 update_mv_prob B(252)
    if (update_mv_prob == 1) {  
  mv_prob L(7)
        prob = (mv_prob << 1) | 1
    }
 return prob
}
    */
		private int update_mv_prob;
		public int _UpdateMvProb { get { return update_mv_prob; } set { update_mv_prob = value; } }
		private int mv_prob;
		public int _MvProb { get { return mv_prob; } set { mv_prob = value; } }

        private int UpdateMvProb(int prob)
        {
			stream.ReadBool(252, out this.update_mv_prob, "update_mv_prob"); 

			if ((update_mv_prob == 1))
			{
				stream.ReadLiteral(7, out this.mv_prob, "mv_prob"); 
				prob = ((mv_prob << 1) | 1);
			}
			return prob;
        }

        private int WriteUpdateMvProb(int prob)
        {
			this.update_mv_prob = stream.Pick("update_mv_prob", _original != null ? _original.update_mv_prob : this.update_mv_prob, _edited != null ? _edited.update_mv_prob : _original != null ? _original.update_mv_prob : this.update_mv_prob);
			stream.WriteBool(252, this.update_mv_prob, "update_mv_prob"); 

			if ((update_mv_prob == 1))
			{
				this.mv_prob = stream.Pick("mv_prob", _original != null ? _original.mv_prob : this.mv_prob, _edited != null ? _edited.mv_prob : _original != null ? _original.mv_prob : this.mv_prob);
				stream.WriteLiteral(7, this.mv_prob, "mv_prob"); 
				prob = ((mv_prob << 1) | 1);
			}
			return prob;
        }

    /*
setup_compound_reference_mode() {
    if (ref_frame_sign_bias[LAST_FRAME] ==
        ref_frame_sign_bias[GOLDEN_FRAME]) {

        CompFixedRef = ALTREF_FRAME
        CompVarRef[0] = LAST_FRAME
        CompVarRef[1] = GOLDEN_FRAME
    } else if (ref_frame_sign_bias[LAST_FRAME] ==
        ref_frame_sign_bias[ALTREF_FRAME]) {

        CompFixedRef = GOLDEN_FRAME
        CompVarRef[0] = LAST_FRAME
        CompVarRef[1] = ALTREF_FRAME
    } else {
        CompFixedRef = LAST_FRAME
        CompVarRef[0] = GOLDEN_FRAME
        CompVarRef[1] = ALTREF_FRAME  
 }  
}
    */
		private int CompFixedRef;
		public int _CompFixedRef { get { return CompFixedRef; } set { CompFixedRef = value; } }
		private AomArray<int> CompVarRef = new AomArray<int>();
		public AomArray<int> _CompVarRef { get { return CompVarRef; } set { CompVarRef = value; } }

        private void SetupCompoundReferenceMode()
        {

			if ((ref_frame_sign_bias[LAST_FRAME] == ref_frame_sign_bias[GOLDEN_FRAME]))
			{
				CompFixedRef = ALTREF_FRAME;
				CompVarRef[0] = LAST_FRAME;
				CompVarRef[1] = GOLDEN_FRAME;
			}
			else if ((ref_frame_sign_bias[LAST_FRAME] == ref_frame_sign_bias[ALTREF_FRAME]))
			{
				CompFixedRef = GOLDEN_FRAME;
				CompVarRef[0] = LAST_FRAME;
				CompVarRef[1] = ALTREF_FRAME;
			}
			else 
			{
				CompFixedRef = LAST_FRAME;
				CompVarRef[0] = GOLDEN_FRAME;
				CompVarRef[1] = ALTREF_FRAME;
			}
        }

    /*
superframe( sz ) {
    for( i = 0; i < NumFrames; i++ )
        frame( frame_sizes[ i ] )
    superframe_index( )
}
    */

        private void Superframe(int sz)
        {
			int i = 0;

			for (i = 0; (i < NumFrames); i++)
			{
				Frame(frame_sizes[i]); 
			}
			SuperframeIndex(); 
        }

        private void WriteSuperframe(int sz)
        {
			int i = 0;

			for (i = 0; (i < NumFrames); i++)
			{
				WriteFrame(frame_sizes[i]); 
			}
			WriteSuperframeIndex(); 
        }

    /*
superframe_index( ) {
    superframe_header( )
    for( i = 0; i < NumFrames; i++ )
        frame_sizes[ i ] f(SzBytes)
    superframe_header( )
}
    */
		private AomArray<int> frame_sizes = new AomArray<int>();
		public AomArray<int> _FrameSizes { get { return frame_sizes; } set { frame_sizes = value; } }

        private void SuperframeIndex()
        {
			int i = 0;
			SuperframeHeader(); 

			for (i = 0; (i < NumFrames); i++)
			{
				stream.ReadLe(SzBytes, out this.frame_sizes[i], "frame_sizes"); 
			}
			SuperframeHeader(); 
        }

        private void WriteSuperframeIndex()
        {
			int i = 0;
			WriteSuperframeHeader(); 

			for (i = 0; (i < NumFrames); i++)
			{
				this.frame_sizes[i] = stream.Pick("frame_sizes", _original != null ? _original.frame_sizes[i] : this.frame_sizes[i], _edited != null ? _edited.frame_sizes[i] : _original != null ? _original.frame_sizes[i] : this.frame_sizes[i]);
				stream.WriteLe(SzBytes, this.frame_sizes[i], "frame_sizes"); 
			}
			WriteSuperframeHeader(); 
        }

    /*
superframe_header( ) {
    superframe_marker f(3)
    bytes_per_framesize_minus_1 f(2)
    frames_in_superframe_minus_1 f(3)
}
    */
		private int superframe_marker;
		public int _SuperframeMarker { get { return superframe_marker; } set { superframe_marker = value; } }
		private int bytes_per_framesize_minus_1;
		public int _BytesPerFramesizeMinus1 { get { return bytes_per_framesize_minus_1; } set { bytes_per_framesize_minus_1 = value; } }
		private int frames_in_superframe_minus_1;
		public int _FramesInSuperframeMinus1 { get { return frames_in_superframe_minus_1; } set { frames_in_superframe_minus_1 = value; } }

        private void SuperframeHeader()
        {
			stream.ReadFixed(3, out this.superframe_marker, "superframe_marker"); 
			stream.ReadFixed(2, out this.bytes_per_framesize_minus_1, "bytes_per_framesize_minus_1"); 
			stream.ReadFixed(3, out this.frames_in_superframe_minus_1, "frames_in_superframe_minus_1"); 
        }

        private void WriteSuperframeHeader()
        {
			this.superframe_marker = stream.Pick("superframe_marker", _original != null ? _original.superframe_marker : this.superframe_marker, _edited != null ? _edited.superframe_marker : _original != null ? _original.superframe_marker : this.superframe_marker);
			stream.WriteFixed(3, this.superframe_marker, "superframe_marker"); 
			this.bytes_per_framesize_minus_1 = stream.Pick("bytes_per_framesize_minus_1", _original != null ? _original.bytes_per_framesize_minus_1 : this.bytes_per_framesize_minus_1, _edited != null ? _edited.bytes_per_framesize_minus_1 : _original != null ? _original.bytes_per_framesize_minus_1 : this.bytes_per_framesize_minus_1);
			stream.WriteFixed(2, this.bytes_per_framesize_minus_1, "bytes_per_framesize_minus_1"); 
			this.frames_in_superframe_minus_1 = stream.Pick("frames_in_superframe_minus_1", _original != null ? _original.frames_in_superframe_minus_1 : this.frames_in_superframe_minus_1, _edited != null ? _edited.frames_in_superframe_minus_1 : _original != null ? _original.frames_in_superframe_minus_1 : this.frames_in_superframe_minus_1);
			stream.WriteFixed(3, this.frames_in_superframe_minus_1, "frames_in_superframe_minus_1"); 
        }

		/// <summary>What the context holds, as SaveContext keeps it.</summary>
		private sealed partial class ContextState
		{
			public int BitDepth;
			public int CompFixedRef;
			public AomArray<int> CompVarRef;
			public AomArray<AomArray<int>> FeatureData;
			public AomArray<AomArray<int>> FeatureEnabled;
			public int FrameHeight;
			public int FrameIsIntra;
			public int FrameWidth;
			public int LastFrameType;
			public int Lossless;
			public int MiCols;
			public int MiRows;
			public int Profile;
			public int Sb64Cols;
			public int Sb64Rows;
			public int allow_high_precision_mv;
			public int base_q_idx;
			public int bit;
			public int bits_to_read;
			public int bytes_per_framesize_minus_1;
			public int color_range;
			public int color_space;
			public int deltaProb;
			public int delta_coded;
			public int delta_q;
			public int delta_q_uv_ac;
			public int delta_q_uv_dc;
			public int delta_q_y_dc;
			public int error_resilient_mode;
			public int feature_enabled;
			public int feature_sign;
			public int feature_value;
			public int found_ref;
			public int frame_context_idx;
			public int frame_height_minus_1;
			public int frame_marker;
			public int frame_parallel_decoding_mode;
			public AomArray<int> frame_sizes;
			public int frame_sync_byte_0;
			public int frame_sync_byte_1;
			public int frame_sync_byte_2;
			public int frame_to_show_map_idx;
			public int frame_type;
			public int frame_width_minus_1;
			public int frames_in_superframe_minus_1;
			public int header_size_in_bytes;
			public int increment_tile_cols_log2;
			public int increment_tile_rows_log2;
			public int interpolation_filter;
			public int intra_only;
			public int is_filter_switchable;
			public int loop_filter_delta_enabled;
			public int loop_filter_delta_update;
			public int loop_filter_level;
			public AomArray<int> loop_filter_mode_deltas;
			public AomArray<int> loop_filter_ref_deltas;
			public int loop_filter_sharpness;
			public int m;
			public int mv_prob;
			public int non_single_reference;
			public int padding_bit;
			public int prob;
			public int prob_coded;
			public int profile_high_bit;
			public int profile_low_bit;
			public int raw_interpolation_filter;
			public AomArray<int> ref_frame_idx;
			public AomArray<int> ref_frame_sign_bias;
			public int reference_mode;
			public int reference_select;
			public int refresh_frame_context;
			public int refresh_frame_flags;
			public int renderHeight;
			public int renderWidth;
			public int render_and_frame_size_different;
			public int render_height_minus_1;
			public int render_width_minus_1;
			public int reserved_zero;
			public int reset_frame_context;
			public int segmentation_abs_or_delta_update;
			public int segmentation_enabled;
			public AomArray<int> segmentation_pred_prob;
			public int segmentation_temporal_update;
			public AomArray<int> segmentation_tree_probs;
			public int segmentation_update_data;
			public int segmentation_update_map;
			public int show_existing_frame;
			public int show_frame;
			public int sub_exp_val;
			public int sub_exp_val_minus_16;
			public int sub_exp_val_minus_32;
			public int subsampling_x;
			public int subsampling_y;
			public int superframe_marker;
			public int sz;
			public int ten_or_twelve_bit;
			public int tile_cols_log2;
			public int tile_rows_log2;
			public int tx_mode;
			public int tx_mode_select;
			public int update_mode_delta;
			public int update_mv_prob;
			public int update_prob;
			public int update_probs;
			public int update_ref_delta;
			public int v;
			public int zero_bit;
		}

		private ContextState SaveContext()
		{
			var state = new ContextState();
			state.BitDepth = this.BitDepth;
			state.CompFixedRef = this.CompFixedRef;
			state.CompVarRef = this.CompVarRef?.Clone();
			state.FeatureData = this.FeatureData?.Clone();
			state.FeatureEnabled = this.FeatureEnabled?.Clone();
			state.FrameHeight = this.FrameHeight;
			state.FrameIsIntra = this.FrameIsIntra;
			state.FrameWidth = this.FrameWidth;
			state.LastFrameType = this.LastFrameType;
			state.Lossless = this.Lossless;
			state.MiCols = this.MiCols;
			state.MiRows = this.MiRows;
			state.Profile = this.Profile;
			state.Sb64Cols = this.Sb64Cols;
			state.Sb64Rows = this.Sb64Rows;
			state.allow_high_precision_mv = this.allow_high_precision_mv;
			state.base_q_idx = this.base_q_idx;
			state.bit = this.bit;
			state.bits_to_read = this.bits_to_read;
			state.bytes_per_framesize_minus_1 = this.bytes_per_framesize_minus_1;
			state.color_range = this.color_range;
			state.color_space = this.color_space;
			state.deltaProb = this.deltaProb;
			state.delta_coded = this.delta_coded;
			state.delta_q = this.delta_q;
			state.delta_q_uv_ac = this.delta_q_uv_ac;
			state.delta_q_uv_dc = this.delta_q_uv_dc;
			state.delta_q_y_dc = this.delta_q_y_dc;
			state.error_resilient_mode = this.error_resilient_mode;
			state.feature_enabled = this.feature_enabled;
			state.feature_sign = this.feature_sign;
			state.feature_value = this.feature_value;
			state.found_ref = this.found_ref;
			state.frame_context_idx = this.frame_context_idx;
			state.frame_height_minus_1 = this.frame_height_minus_1;
			state.frame_marker = this.frame_marker;
			state.frame_parallel_decoding_mode = this.frame_parallel_decoding_mode;
			state.frame_sizes = this.frame_sizes?.Clone();
			state.frame_sync_byte_0 = this.frame_sync_byte_0;
			state.frame_sync_byte_1 = this.frame_sync_byte_1;
			state.frame_sync_byte_2 = this.frame_sync_byte_2;
			state.frame_to_show_map_idx = this.frame_to_show_map_idx;
			state.frame_type = this.frame_type;
			state.frame_width_minus_1 = this.frame_width_minus_1;
			state.frames_in_superframe_minus_1 = this.frames_in_superframe_minus_1;
			state.header_size_in_bytes = this.header_size_in_bytes;
			state.increment_tile_cols_log2 = this.increment_tile_cols_log2;
			state.increment_tile_rows_log2 = this.increment_tile_rows_log2;
			state.interpolation_filter = this.interpolation_filter;
			state.intra_only = this.intra_only;
			state.is_filter_switchable = this.is_filter_switchable;
			state.loop_filter_delta_enabled = this.loop_filter_delta_enabled;
			state.loop_filter_delta_update = this.loop_filter_delta_update;
			state.loop_filter_level = this.loop_filter_level;
			state.loop_filter_mode_deltas = this.loop_filter_mode_deltas?.Clone();
			state.loop_filter_ref_deltas = this.loop_filter_ref_deltas?.Clone();
			state.loop_filter_sharpness = this.loop_filter_sharpness;
			state.m = this.m;
			state.mv_prob = this.mv_prob;
			state.non_single_reference = this.non_single_reference;
			state.padding_bit = this.padding_bit;
			state.prob = this.prob;
			state.prob_coded = this.prob_coded;
			state.profile_high_bit = this.profile_high_bit;
			state.profile_low_bit = this.profile_low_bit;
			state.raw_interpolation_filter = this.raw_interpolation_filter;
			state.ref_frame_idx = this.ref_frame_idx?.Clone();
			state.ref_frame_sign_bias = this.ref_frame_sign_bias?.Clone();
			state.reference_mode = this.reference_mode;
			state.reference_select = this.reference_select;
			state.refresh_frame_context = this.refresh_frame_context;
			state.refresh_frame_flags = this.refresh_frame_flags;
			state.renderHeight = this.renderHeight;
			state.renderWidth = this.renderWidth;
			state.render_and_frame_size_different = this.render_and_frame_size_different;
			state.render_height_minus_1 = this.render_height_minus_1;
			state.render_width_minus_1 = this.render_width_minus_1;
			state.reserved_zero = this.reserved_zero;
			state.reset_frame_context = this.reset_frame_context;
			state.segmentation_abs_or_delta_update = this.segmentation_abs_or_delta_update;
			state.segmentation_enabled = this.segmentation_enabled;
			state.segmentation_pred_prob = this.segmentation_pred_prob?.Clone();
			state.segmentation_temporal_update = this.segmentation_temporal_update;
			state.segmentation_tree_probs = this.segmentation_tree_probs?.Clone();
			state.segmentation_update_data = this.segmentation_update_data;
			state.segmentation_update_map = this.segmentation_update_map;
			state.show_existing_frame = this.show_existing_frame;
			state.show_frame = this.show_frame;
			state.sub_exp_val = this.sub_exp_val;
			state.sub_exp_val_minus_16 = this.sub_exp_val_minus_16;
			state.sub_exp_val_minus_32 = this.sub_exp_val_minus_32;
			state.subsampling_x = this.subsampling_x;
			state.subsampling_y = this.subsampling_y;
			state.superframe_marker = this.superframe_marker;
			state.sz = this.sz;
			state.ten_or_twelve_bit = this.ten_or_twelve_bit;
			state.tile_cols_log2 = this.tile_cols_log2;
			state.tile_rows_log2 = this.tile_rows_log2;
			state.tx_mode = this.tx_mode;
			state.tx_mode_select = this.tx_mode_select;
			state.update_mode_delta = this.update_mode_delta;
			state.update_mv_prob = this.update_mv_prob;
			state.update_prob = this.update_prob;
			state.update_probs = this.update_probs;
			state.update_ref_delta = this.update_ref_delta;
			state.v = this.v;
			state.zero_bit = this.zero_bit;
			SaveContextExtra(state);
			return state;
		}

		private void LoadContext(ContextState state, bool copy = true)
		{
			this.BitDepth = state.BitDepth;
			this.CompFixedRef = state.CompFixedRef;
			this.CompVarRef = copy ? state.CompVarRef?.Clone() : state.CompVarRef;
			this.FeatureData = copy ? state.FeatureData?.Clone() : state.FeatureData;
			this.FeatureEnabled = copy ? state.FeatureEnabled?.Clone() : state.FeatureEnabled;
			this.FrameHeight = state.FrameHeight;
			this.FrameIsIntra = state.FrameIsIntra;
			this.FrameWidth = state.FrameWidth;
			this.LastFrameType = state.LastFrameType;
			this.Lossless = state.Lossless;
			this.MiCols = state.MiCols;
			this.MiRows = state.MiRows;
			this.Profile = state.Profile;
			this.Sb64Cols = state.Sb64Cols;
			this.Sb64Rows = state.Sb64Rows;
			this.allow_high_precision_mv = state.allow_high_precision_mv;
			this.base_q_idx = state.base_q_idx;
			this.bit = state.bit;
			this.bits_to_read = state.bits_to_read;
			this.bytes_per_framesize_minus_1 = state.bytes_per_framesize_minus_1;
			this.color_range = state.color_range;
			this.color_space = state.color_space;
			this.deltaProb = state.deltaProb;
			this.delta_coded = state.delta_coded;
			this.delta_q = state.delta_q;
			this.delta_q_uv_ac = state.delta_q_uv_ac;
			this.delta_q_uv_dc = state.delta_q_uv_dc;
			this.delta_q_y_dc = state.delta_q_y_dc;
			this.error_resilient_mode = state.error_resilient_mode;
			this.feature_enabled = state.feature_enabled;
			this.feature_sign = state.feature_sign;
			this.feature_value = state.feature_value;
			this.found_ref = state.found_ref;
			this.frame_context_idx = state.frame_context_idx;
			this.frame_height_minus_1 = state.frame_height_minus_1;
			this.frame_marker = state.frame_marker;
			this.frame_parallel_decoding_mode = state.frame_parallel_decoding_mode;
			this.frame_sizes = copy ? state.frame_sizes?.Clone() : state.frame_sizes;
			this.frame_sync_byte_0 = state.frame_sync_byte_0;
			this.frame_sync_byte_1 = state.frame_sync_byte_1;
			this.frame_sync_byte_2 = state.frame_sync_byte_2;
			this.frame_to_show_map_idx = state.frame_to_show_map_idx;
			this.frame_type = state.frame_type;
			this.frame_width_minus_1 = state.frame_width_minus_1;
			this.frames_in_superframe_minus_1 = state.frames_in_superframe_minus_1;
			this.header_size_in_bytes = state.header_size_in_bytes;
			this.increment_tile_cols_log2 = state.increment_tile_cols_log2;
			this.increment_tile_rows_log2 = state.increment_tile_rows_log2;
			this.interpolation_filter = state.interpolation_filter;
			this.intra_only = state.intra_only;
			this.is_filter_switchable = state.is_filter_switchable;
			this.loop_filter_delta_enabled = state.loop_filter_delta_enabled;
			this.loop_filter_delta_update = state.loop_filter_delta_update;
			this.loop_filter_level = state.loop_filter_level;
			this.loop_filter_mode_deltas = copy ? state.loop_filter_mode_deltas?.Clone() : state.loop_filter_mode_deltas;
			this.loop_filter_ref_deltas = copy ? state.loop_filter_ref_deltas?.Clone() : state.loop_filter_ref_deltas;
			this.loop_filter_sharpness = state.loop_filter_sharpness;
			this.m = state.m;
			this.mv_prob = state.mv_prob;
			this.non_single_reference = state.non_single_reference;
			this.padding_bit = state.padding_bit;
			this.prob = state.prob;
			this.prob_coded = state.prob_coded;
			this.profile_high_bit = state.profile_high_bit;
			this.profile_low_bit = state.profile_low_bit;
			this.raw_interpolation_filter = state.raw_interpolation_filter;
			this.ref_frame_idx = copy ? state.ref_frame_idx?.Clone() : state.ref_frame_idx;
			this.ref_frame_sign_bias = copy ? state.ref_frame_sign_bias?.Clone() : state.ref_frame_sign_bias;
			this.reference_mode = state.reference_mode;
			this.reference_select = state.reference_select;
			this.refresh_frame_context = state.refresh_frame_context;
			this.refresh_frame_flags = state.refresh_frame_flags;
			this.renderHeight = state.renderHeight;
			this.renderWidth = state.renderWidth;
			this.render_and_frame_size_different = state.render_and_frame_size_different;
			this.render_height_minus_1 = state.render_height_minus_1;
			this.render_width_minus_1 = state.render_width_minus_1;
			this.reserved_zero = state.reserved_zero;
			this.reset_frame_context = state.reset_frame_context;
			this.segmentation_abs_or_delta_update = state.segmentation_abs_or_delta_update;
			this.segmentation_enabled = state.segmentation_enabled;
			this.segmentation_pred_prob = copy ? state.segmentation_pred_prob?.Clone() : state.segmentation_pred_prob;
			this.segmentation_temporal_update = state.segmentation_temporal_update;
			this.segmentation_tree_probs = copy ? state.segmentation_tree_probs?.Clone() : state.segmentation_tree_probs;
			this.segmentation_update_data = state.segmentation_update_data;
			this.segmentation_update_map = state.segmentation_update_map;
			this.show_existing_frame = state.show_existing_frame;
			this.show_frame = state.show_frame;
			this.sub_exp_val = state.sub_exp_val;
			this.sub_exp_val_minus_16 = state.sub_exp_val_minus_16;
			this.sub_exp_val_minus_32 = state.sub_exp_val_minus_32;
			this.subsampling_x = state.subsampling_x;
			this.subsampling_y = state.subsampling_y;
			this.superframe_marker = state.superframe_marker;
			this.sz = state.sz;
			this.ten_or_twelve_bit = state.ten_or_twelve_bit;
			this.tile_cols_log2 = state.tile_cols_log2;
			this.tile_rows_log2 = state.tile_rows_log2;
			this.tx_mode = state.tx_mode;
			this.tx_mode_select = state.tx_mode_select;
			this.update_mode_delta = state.update_mode_delta;
			this.update_mv_prob = state.update_mv_prob;
			this.update_prob = state.update_prob;
			this.update_probs = state.update_probs;
			this.update_ref_delta = state.update_ref_delta;
			this.v = state.v;
			this.zero_bit = state.zero_bit;
			LoadContextExtra(state);
		}

		partial void SaveContextExtra(ContextState state);
		partial void LoadContextExtra(ContextState state);

    }
}
