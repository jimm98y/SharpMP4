using System;
using System.Collections.Generic;
using System.Numerics;
using SharpH26X;

namespace SharpMPEG4
{

    public partial class MPEG4Context : IItuContext
    {

    }

    /*
VisualObjectSequence() {
visual_object_sequence_start_code u(32)
profile_and_level_indication u(8)
next_start_code()
}
    */
    public class VisualObjectSequence : IItuSerializable
    {
		private uint visual_object_sequence_start_code;
		public uint VisualObjectSequenceStartCode { get { return visual_object_sequence_start_code; } set { visual_object_sequence_start_code = value; } }
		private uint profile_and_level_indication;
		public uint ProfileAndLevelIndication { get { return profile_and_level_indication; } set { profile_and_level_indication = value; } }
		private NextStartCode next_start_code;
		public NextStartCode NextStartCode { get { return next_start_code; } set { next_start_code = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public VisualObjectSequence()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            MPEG4Context ituContext = context as MPEG4Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type MPEG4Context");

            ulong size = 0;

			size += stream.ReadUnsignedInt(size, 32, out this.visual_object_sequence_start_code, "visual_object_sequence_start_code"); 
			size += stream.ReadUnsignedInt(size, 8, out this.profile_and_level_indication, "profile_and_level_indication"); 
			this.next_start_code =  new NextStartCode() ;
			size +=  stream.ReadClass<NextStartCode>(size, context, this.next_start_code, "next_start_code"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            MPEG4Context ituContext = context as MPEG4Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type MPEG4Context");
            ulong size = 0;

			size += stream.WriteUnsignedInt(32, this.visual_object_sequence_start_code, "visual_object_sequence_start_code"); 
			size += stream.WriteUnsignedInt(8, this.profile_and_level_indication, "profile_and_level_indication"); 
			size += stream.WriteClass<NextStartCode>(context, this.next_start_code, "next_start_code"); 

            return size;
         }

    }

    /*


VisualObjectSequenceEnd() {
visual_object_sequence_end_code u(32)
next_start_code()
}
    */
    public class VisualObjectSequenceEnd : IItuSerializable
    {
		private uint visual_object_sequence_end_code;
		public uint VisualObjectSequenceEndCode { get { return visual_object_sequence_end_code; } set { visual_object_sequence_end_code = value; } }
		private NextStartCode next_start_code;
		public NextStartCode NextStartCode { get { return next_start_code; } set { next_start_code = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public VisualObjectSequenceEnd()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            MPEG4Context ituContext = context as MPEG4Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type MPEG4Context");

            ulong size = 0;

			size += stream.ReadUnsignedInt(size, 32, out this.visual_object_sequence_end_code, "visual_object_sequence_end_code"); 
			this.next_start_code =  new NextStartCode() ;
			size +=  stream.ReadClass<NextStartCode>(size, context, this.next_start_code, "next_start_code"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            MPEG4Context ituContext = context as MPEG4Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type MPEG4Context");
            ulong size = 0;

			size += stream.WriteUnsignedInt(32, this.visual_object_sequence_end_code, "visual_object_sequence_end_code"); 
			size += stream.WriteClass<NextStartCode>(context, this.next_start_code, "next_start_code"); 

            return size;
         }

    }

    /*


VisualObject() {
visual_object_start_code u(32)
is_visual_object_identifier u(1)
if (is_visual_object_identifier) {
visual_object_verid u(4)
visual_object_priority u(3)
}
visual_object_type u(4)
if (visual_object_type == 1 || visual_object_type == 2) {
video_signal_type()
}
next_start_code()
}
    */
    public class VisualObject : IItuSerializable
    {
		private uint visual_object_start_code;
		public uint VisualObjectStartCode { get { return visual_object_start_code; } set { visual_object_start_code = value; } }
		private byte is_visual_object_identifier;
		public byte IsVisualObjectIdentifier { get { return is_visual_object_identifier; } set { is_visual_object_identifier = value; } }
		private uint visual_object_verid;
		public uint VisualObjectVerid { get { return visual_object_verid; } set { visual_object_verid = value; } }
		private uint visual_object_priority;
		public uint VisualObjectPriority { get { return visual_object_priority; } set { visual_object_priority = value; } }
		private uint visual_object_type;
		public uint VisualObjectType { get { return visual_object_type; } set { visual_object_type = value; } }
		private VideoSignalType video_signal_type;
		public VideoSignalType VideoSignalType { get { return video_signal_type; } set { video_signal_type = value; } }
		private NextStartCode next_start_code;
		public NextStartCode NextStartCode { get { return next_start_code; } set { next_start_code = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public VisualObject()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            MPEG4Context ituContext = context as MPEG4Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type MPEG4Context");

            ulong size = 0;

			size += stream.ReadUnsignedInt(size, 32, out this.visual_object_start_code, "visual_object_start_code"); 
			ituContext.OnVisualObject(this);
			size += stream.ReadUnsignedInt(size, 1, out this.is_visual_object_identifier, "is_visual_object_identifier"); 

			if (is_visual_object_identifier != 0)
			{
				size += stream.ReadUnsignedInt(size, 4, out this.visual_object_verid, "visual_object_verid"); 
				ituContext.VisualObjectVerid = visual_object_verid;
				size += stream.ReadUnsignedInt(size, 3, out this.visual_object_priority, "visual_object_priority"); 
			}
			size += stream.ReadUnsignedInt(size, 4, out this.visual_object_type, "visual_object_type"); 

			if (visual_object_type == 1 || visual_object_type == 2)
			{
				this.video_signal_type =  new VideoSignalType() ;
				size +=  stream.ReadClass<VideoSignalType>(size, context, this.video_signal_type, "video_signal_type"); 
			}
			this.next_start_code =  new NextStartCode() ;
			size +=  stream.ReadClass<NextStartCode>(size, context, this.next_start_code, "next_start_code"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            MPEG4Context ituContext = context as MPEG4Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type MPEG4Context");
            ulong size = 0;

			size += stream.WriteUnsignedInt(32, this.visual_object_start_code, "visual_object_start_code"); 
			ituContext.OnVisualObject(this);
			size += stream.WriteUnsignedInt(1, this.is_visual_object_identifier, "is_visual_object_identifier"); 

			if (is_visual_object_identifier != 0)
			{
				size += stream.WriteUnsignedInt(4, this.visual_object_verid, "visual_object_verid"); 
				ituContext.VisualObjectVerid = visual_object_verid;
				size += stream.WriteUnsignedInt(3, this.visual_object_priority, "visual_object_priority"); 
			}
			size += stream.WriteUnsignedInt(4, this.visual_object_type, "visual_object_type"); 

			if (visual_object_type == 1 || visual_object_type == 2)
			{
				size += stream.WriteClass<VideoSignalType>(context, this.video_signal_type, "video_signal_type"); 
			}
			size += stream.WriteClass<NextStartCode>(context, this.next_start_code, "next_start_code"); 

            return size;
         }

    }

    /*


VideoObject() {
video_object_start_code u(32)
next_start_code()
}
    */
    public class VideoObject : IItuSerializable
    {
		private uint video_object_start_code;
		public uint VideoObjectStartCode { get { return video_object_start_code; } set { video_object_start_code = value; } }
		private NextStartCode next_start_code;
		public NextStartCode NextStartCode { get { return next_start_code; } set { next_start_code = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public VideoObject()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            MPEG4Context ituContext = context as MPEG4Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type MPEG4Context");

            ulong size = 0;

			size += stream.ReadUnsignedInt(size, 32, out this.video_object_start_code, "video_object_start_code"); 
			this.next_start_code =  new NextStartCode() ;
			size +=  stream.ReadClass<NextStartCode>(size, context, this.next_start_code, "next_start_code"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            MPEG4Context ituContext = context as MPEG4Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type MPEG4Context");
            ulong size = 0;

			size += stream.WriteUnsignedInt(32, this.video_object_start_code, "video_object_start_code"); 
			size += stream.WriteClass<NextStartCode>(context, this.next_start_code, "next_start_code"); 

            return size;
         }

    }

    /*


video_signal_type() {
video_signal_type u(1)
if (video_signal_type) {
video_format u(3)
video_range u(1)
colour_description u(1)
if (colour_description) {
colour_primaries u(8)
transfer_characteristics u(8)
matrix_coefficients u(8)
}
}
}
    */
    public class VideoSignalType : IItuSerializable
    {
		private byte video_signal_type;
		public byte _VideoSignalType { get { return video_signal_type; } set { video_signal_type = value; } }
		private uint video_format;
		public uint VideoFormat { get { return video_format; } set { video_format = value; } }
		private byte video_range;
		public byte VideoRange { get { return video_range; } set { video_range = value; } }
		private byte colour_description;
		public byte ColourDescription { get { return colour_description; } set { colour_description = value; } }
		private uint colour_primaries;
		public uint ColourPrimaries { get { return colour_primaries; } set { colour_primaries = value; } }
		private uint transfer_characteristics;
		public uint TransferCharacteristics { get { return transfer_characteristics; } set { transfer_characteristics = value; } }
		private uint matrix_coefficients;
		public uint MatrixCoefficients { get { return matrix_coefficients; } set { matrix_coefficients = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public VideoSignalType()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            MPEG4Context ituContext = context as MPEG4Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type MPEG4Context");

            ulong size = 0;

			size += stream.ReadUnsignedInt(size, 1, out this.video_signal_type, "video_signal_type"); 

			if (video_signal_type != 0)
			{
				size += stream.ReadUnsignedInt(size, 3, out this.video_format, "video_format"); 
				size += stream.ReadUnsignedInt(size, 1, out this.video_range, "video_range"); 
				size += stream.ReadUnsignedInt(size, 1, out this.colour_description, "colour_description"); 

				if (colour_description != 0)
				{
					size += stream.ReadUnsignedInt(size, 8, out this.colour_primaries, "colour_primaries"); 
					size += stream.ReadUnsignedInt(size, 8, out this.transfer_characteristics, "transfer_characteristics"); 
					size += stream.ReadUnsignedInt(size, 8, out this.matrix_coefficients, "matrix_coefficients"); 
				}
			}

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            MPEG4Context ituContext = context as MPEG4Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type MPEG4Context");
            ulong size = 0;

			size += stream.WriteUnsignedInt(1, this.video_signal_type, "video_signal_type"); 

			if (video_signal_type != 0)
			{
				size += stream.WriteUnsignedInt(3, this.video_format, "video_format"); 
				size += stream.WriteUnsignedInt(1, this.video_range, "video_range"); 
				size += stream.WriteUnsignedInt(1, this.colour_description, "colour_description"); 

				if (colour_description != 0)
				{
					size += stream.WriteUnsignedInt(8, this.colour_primaries, "colour_primaries"); 
					size += stream.WriteUnsignedInt(8, this.transfer_characteristics, "transfer_characteristics"); 
					size += stream.WriteUnsignedInt(8, this.matrix_coefficients, "matrix_coefficients"); 
				}
			}

            return size;
         }

    }

    /*


user_data() {
user_data_start_code u(32)
while ( before_start_code( user_data ) ) {
user_data u(8)
}
}
    */
    public class UserData : IItuSerializable
    {
		private uint user_data_start_code;
		public uint UserDataStartCode { get { return user_data_start_code; } set { user_data_start_code = value; } }
		private Dictionary<int, uint> user_data;
		public Dictionary<int, uint> _UserData { get { return user_data ??= new Dictionary<int, uint>(); } set { user_data = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public UserData()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            MPEG4Context ituContext = context as MPEG4Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type MPEG4Context");

            ulong size = 0;

			int whileIndex = -1;
			size += stream.ReadUnsignedInt(size, 32, out this.user_data_start_code, "user_data_start_code"); 

			while ( !ituContext.AtStartCode(stream) )
			{
				whileIndex++;

				size += stream.ReadUnsignedInt(size, 8, whileIndex, (this.user_data ??= new()), "user_data"); 
			}

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            MPEG4Context ituContext = context as MPEG4Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type MPEG4Context");
            ulong size = 0;

			int whileIndex = -1;
			size += stream.WriteUnsignedInt(32, this.user_data_start_code, "user_data_start_code"); 

			while ( whileIndex + 1 < this._UserData.Count )
			{
				whileIndex++;

				size += stream.WriteUnsignedInt(8, whileIndex, (this.user_data ??= new()), "user_data"); 
			}

            return size;
         }

    }

    /*


VideoObjectLayer() {
video_object_layer_start_code u(32)
random_accessible_vol u(1)
video_object_type_indication u(8)
if ( video_object_type_indication == 18 ) {
fgs_layer_type u(2)
video_object_layer_priority u(3)
aspect_ratio_info u(4)
if (aspect_ratio_info == 15) {
par_width u(8)
par_height u(8)
}
vol_control_parameters u(1)
if (vol_control_parameters) {
chroma_format u(2)
low_delay u(1)
}
marker_bit u(1)
vop_time_increment_resolution u(16)
marker_bit_2 u(1)
fixed_vop_rate u(1)
if (fixed_vop_rate)
fixed_vop_time_increment u(v)
marker_bit_3 u(1)
video_object_layer_width u(13)
marker_bit_4 u(1)
video_object_layer_height u(13)
marker_bit_5 u(1)
interlaced u(1)
if (fgs_layer_type == 2 || fgs_layer_type == 3)
fgs_ref_layer_id u(4)
if (fgs_layer_type == 1 || fgs_layer_type == 3) {
fgs_frequency_weighting_enable u(1)
if ( fgs_frequency_weighting_enable ) {
load_fgs_frequency_weighting_matrix u(1)
if (load_fgs_frequency_weighting_matrix)
fgs_frequency_weighting_matrix()
}
}
if (fgs_layer_type == 2 || fgs_layer_type == 3)
{
fgst_frequency_weighting_enable u(1)
if ( fgst_frequency_weighting_enable ) {
load_fgst_frequency_weighting_matrix u(1)
if (load_fgst_frequency_weighting_matrix)
fgst_frequency_weighting_matrix()
}
}
quarter_sample u(1)
fgs_resync_marker_disable u(1)
next_start_code()
} else {
is_object_layer_identifier u(1)
if (is_object_layer_identifier) {
video_object_layer_verid u(4)
video_object_layer_priority u(3)
}
aspect_ratio_info u(4)
if (aspect_ratio_info == 15) {
par_width u(8)
par_height u(8)
}
vol_control_parameters u(1)
if (vol_control_parameters) {
chroma_format u(2)
low_delay u(1)
vbv_parameters u(1)
if (vbv_parameters) {
first_half_bit_rate u(15)
marker_bit_6 u(1)
latter_half_bit_rate u(15)
marker_bit_7 u(1)
first_half_vbv_buffer_size u(15)
marker_bit_8 u(1)
latter_half_vbv_buffer_size u(3)
first_half_vbv_occupancy u(11)
marker_bit_9 u(1)
latter_half_vbv_occupancy u(15)
marker_bit_10 u(1)
}
}
video_object_layer_shape u(2)
    if (video_object_layer_shape == 3
&& video_object_layer_verid != 1)
video_object_layer_shape_extension u(4)
marker_bit_11 u(1)
vop_time_increment_resolution u(16)
marker_bit_12 u(1)
fixed_vop_rate u(1)
if (fixed_vop_rate)
fixed_vop_time_increment u(v)
if (video_object_layer_shape != 2) {
if (video_object_layer_shape == 0) {
marker_bit_13 u(1)
video_object_layer_width u(13)
marker_bit_14 u(1)
video_object_layer_height u(13)
marker_bit_15 u(1)
}
interlaced u(1)
obmc_disable u(1)
    if (video_object_layer_verid == 1)
sprite_enable u(1)
else
sprite_enable u(2)
if (sprite_enable == 1 || sprite_enable == 2) {
if (sprite_enable != 2) {
sprite_width u(13)
marker_bit_16 u(1)
sprite_height u(13)
marker_bit_17 u(1)
sprite_left_coordinate i(13)
marker_bit_18 u(1)
sprite_top_coordinate i(13)
marker_bit_19 u(1)
}
no_of_sprite_warping_points u(6)
sprite_warping_accuracy u(2)
sprite_brightness_change u(1)
if (sprite_enable != 2)
low_latency_sprite_enable u(1)
}
    if (video_object_layer_verid != 1 &&
video_object_layer_shape != 0)
sadct_disable u(1)
not_8_bit u(1)
if (not_8_bit) {
quant_precision u(4)
bits_per_pixel u(4)
}
if (video_object_layer_shape == 3) {
no_gray_quant_update u(1)
composition_method u(1)
linear_composition u(1)
}
quant_type u(1)
if (quant_type) {
load_intra_quant_mat u(1)
if (load_intra_quant_mat)
intra_quant_mat()
load_nonintra_quant_mat u(1)
if (load_nonintra_quant_mat)
nonintra_quant_mat()
if(video_object_layer_shape == 3) {
for(i=0; i<aux_comp_count; i++) {
load_intra_quant_mat_grayscale u(1)
if(load_intra_quant_mat_grayscale)
intra_quant_mat_grayscale( i )
load_nonintra_quant_mat_grayscale u(1)
if(load_nonintra_quant_mat_grayscale)
nonintra_quant_mat_grayscale( i )
}
}
}
    if (video_object_layer_verid != 1)
quarter_sample u(1)
complexity_estimation_disable u(1)
if (!complexity_estimation_disable)
define_vop_complexity_estimation_header()
resync_marker_disable u(1)
data_partitioned u(1)
if(data_partitioned)
reversible_vlc u(1)
if(video_object_layer_verid != 1) {
newpred_enable u(1)
if (newpred_enable) {
requested_upstream_message_type u(2)
newpred_segment_type u(1)
}
reduced_resolution_vop_enable u(1)
}
scalability u(1)
if (scalability) {
hierarchy_type u(1)
ref_layer_id u(4)
ref_layer_sampling_direc u(1)
hor_sampling_factor_n u(5)
hor_sampling_factor_m u(5)
vert_sampling_factor_n u(5)
vert_sampling_factor_m u(5)
enhancement_type u(1)
    if (video_object_layer_shape == 1 && hierarchy_type == 0) {
use_ref_shape u(1)
use_ref_texture u(1)
shape_hor_sampling_factor_n u(5)
shape_hor_sampling_factor_m u(5)
shape_vert_sampling_factor_n u(5)
shape_vert_sampling_factor_m u(5)
}
}
}
else {
if(video_object_layer_verid != 1) {
scalability u(1)
if(scalability) {
ref_layer_id u(4)
shape_hor_sampling_factor_n u(5)
shape_hor_sampling_factor_m u(5)
shape_vert_sampling_factor_n u(5)
shape_vert_sampling_factor_m u(5)
}
}
resync_marker_disable u(1)
}
next_start_code()
}
}
    */
    public class VideoObjectLayer : IItuSerializable
    {
		private uint video_object_layer_start_code;
		public uint VideoObjectLayerStartCode { get { return video_object_layer_start_code; } set { video_object_layer_start_code = value; } }
		private byte random_accessible_vol;
		public byte RandomAccessibleVol { get { return random_accessible_vol; } set { random_accessible_vol = value; } }
		private uint video_object_type_indication;
		public uint VideoObjectTypeIndication { get { return video_object_type_indication; } set { video_object_type_indication = value; } }
		private uint fgs_layer_type;
		public uint FgsLayerType { get { return fgs_layer_type; } set { fgs_layer_type = value; } }
		private uint video_object_layer_priority;
		public uint VideoObjectLayerPriority { get { return video_object_layer_priority; } set { video_object_layer_priority = value; } }
		private uint aspect_ratio_info;
		public uint AspectRatioInfo { get { return aspect_ratio_info; } set { aspect_ratio_info = value; } }
		private uint par_width;
		public uint ParWidth { get { return par_width; } set { par_width = value; } }
		private uint par_height;
		public uint ParHeight { get { return par_height; } set { par_height = value; } }
		private byte vol_control_parameters;
		public byte VolControlParameters { get { return vol_control_parameters; } set { vol_control_parameters = value; } }
		private uint chroma_format;
		public uint ChromaFormat { get { return chroma_format; } set { chroma_format = value; } }
		private byte low_delay;
		public byte LowDelay { get { return low_delay; } set { low_delay = value; } }
		private byte marker_bit;
		public byte MarkerBit { get { return marker_bit; } set { marker_bit = value; } }
		private uint vop_time_increment_resolution;
		public uint VopTimeIncrementResolution { get { return vop_time_increment_resolution; } set { vop_time_increment_resolution = value; } }
		private byte marker_bit_2;
		public byte MarkerBit2 { get { return marker_bit_2; } set { marker_bit_2 = value; } }
		private byte fixed_vop_rate;
		public byte FixedVopRate { get { return fixed_vop_rate; } set { fixed_vop_rate = value; } }
		private ulong fixed_vop_time_increment;
		public ulong FixedVopTimeIncrement { get { return fixed_vop_time_increment; } set { fixed_vop_time_increment = value; } }
		private byte marker_bit_3;
		public byte MarkerBit3 { get { return marker_bit_3; } set { marker_bit_3 = value; } }
		private uint video_object_layer_width;
		public uint VideoObjectLayerWidth { get { return video_object_layer_width; } set { video_object_layer_width = value; } }
		private byte marker_bit_4;
		public byte MarkerBit4 { get { return marker_bit_4; } set { marker_bit_4 = value; } }
		private uint video_object_layer_height;
		public uint VideoObjectLayerHeight { get { return video_object_layer_height; } set { video_object_layer_height = value; } }
		private byte marker_bit_5;
		public byte MarkerBit5 { get { return marker_bit_5; } set { marker_bit_5 = value; } }
		private byte interlaced;
		public byte Interlaced { get { return interlaced; } set { interlaced = value; } }
		private uint fgs_ref_layer_id;
		public uint FgsRefLayerId { get { return fgs_ref_layer_id; } set { fgs_ref_layer_id = value; } }
		private byte fgs_frequency_weighting_enable;
		public byte FgsFrequencyWeightingEnable { get { return fgs_frequency_weighting_enable; } set { fgs_frequency_weighting_enable = value; } }
		private byte load_fgs_frequency_weighting_matrix;
		public byte LoadFgsFrequencyWeightingMatrix { get { return load_fgs_frequency_weighting_matrix; } set { load_fgs_frequency_weighting_matrix = value; } }
		private FgsFrequencyWeightingMatrix fgs_frequency_weighting_matrix;
		public FgsFrequencyWeightingMatrix FgsFrequencyWeightingMatrix { get { return fgs_frequency_weighting_matrix; } set { fgs_frequency_weighting_matrix = value; } }
		private byte fgst_frequency_weighting_enable;
		public byte FgstFrequencyWeightingEnable { get { return fgst_frequency_weighting_enable; } set { fgst_frequency_weighting_enable = value; } }
		private byte load_fgst_frequency_weighting_matrix;
		public byte LoadFgstFrequencyWeightingMatrix { get { return load_fgst_frequency_weighting_matrix; } set { load_fgst_frequency_weighting_matrix = value; } }
		private FgstFrequencyWeightingMatrix fgst_frequency_weighting_matrix;
		public FgstFrequencyWeightingMatrix FgstFrequencyWeightingMatrix { get { return fgst_frequency_weighting_matrix; } set { fgst_frequency_weighting_matrix = value; } }
		private byte quarter_sample;
		public byte QuarterSample { get { return quarter_sample; } set { quarter_sample = value; } }
		private byte fgs_resync_marker_disable;
		public byte FgsResyncMarkerDisable { get { return fgs_resync_marker_disable; } set { fgs_resync_marker_disable = value; } }
		private NextStartCode next_start_code;
		public NextStartCode NextStartCode { get { return next_start_code; } set { next_start_code = value; } }
		private byte is_object_layer_identifier;
		public byte IsObjectLayerIdentifier { get { return is_object_layer_identifier; } set { is_object_layer_identifier = value; } }
		private uint video_object_layer_verid;
		public uint VideoObjectLayerVerid { get { return video_object_layer_verid; } set { video_object_layer_verid = value; } }
		private byte vbv_parameters;
		public byte VbvParameters { get { return vbv_parameters; } set { vbv_parameters = value; } }
		private uint first_half_bit_rate;
		public uint FirstHalfBitRate { get { return first_half_bit_rate; } set { first_half_bit_rate = value; } }
		private byte marker_bit_6;
		public byte MarkerBit6 { get { return marker_bit_6; } set { marker_bit_6 = value; } }
		private uint latter_half_bit_rate;
		public uint LatterHalfBitRate { get { return latter_half_bit_rate; } set { latter_half_bit_rate = value; } }
		private byte marker_bit_7;
		public byte MarkerBit7 { get { return marker_bit_7; } set { marker_bit_7 = value; } }
		private uint first_half_vbv_buffer_size;
		public uint FirstHalfVbvBufferSize { get { return first_half_vbv_buffer_size; } set { first_half_vbv_buffer_size = value; } }
		private byte marker_bit_8;
		public byte MarkerBit8 { get { return marker_bit_8; } set { marker_bit_8 = value; } }
		private uint latter_half_vbv_buffer_size;
		public uint LatterHalfVbvBufferSize { get { return latter_half_vbv_buffer_size; } set { latter_half_vbv_buffer_size = value; } }
		private uint first_half_vbv_occupancy;
		public uint FirstHalfVbvOccupancy { get { return first_half_vbv_occupancy; } set { first_half_vbv_occupancy = value; } }
		private byte marker_bit_9;
		public byte MarkerBit9 { get { return marker_bit_9; } set { marker_bit_9 = value; } }
		private uint latter_half_vbv_occupancy;
		public uint LatterHalfVbvOccupancy { get { return latter_half_vbv_occupancy; } set { latter_half_vbv_occupancy = value; } }
		private byte marker_bit_10;
		public byte MarkerBit10 { get { return marker_bit_10; } set { marker_bit_10 = value; } }
		private uint video_object_layer_shape;
		public uint VideoObjectLayerShape { get { return video_object_layer_shape; } set { video_object_layer_shape = value; } }
		private uint video_object_layer_shape_extension;
		public uint VideoObjectLayerShapeExtension { get { return video_object_layer_shape_extension; } set { video_object_layer_shape_extension = value; } }
		private byte marker_bit_11;
		public byte MarkerBit11 { get { return marker_bit_11; } set { marker_bit_11 = value; } }
		private byte marker_bit_12;
		public byte MarkerBit12 { get { return marker_bit_12; } set { marker_bit_12 = value; } }
		private byte marker_bit_13;
		public byte MarkerBit13 { get { return marker_bit_13; } set { marker_bit_13 = value; } }
		private byte marker_bit_14;
		public byte MarkerBit14 { get { return marker_bit_14; } set { marker_bit_14 = value; } }
		private byte marker_bit_15;
		public byte MarkerBit15 { get { return marker_bit_15; } set { marker_bit_15 = value; } }
		private byte obmc_disable;
		public byte ObmcDisable { get { return obmc_disable; } set { obmc_disable = value; } }
		private byte sprite_enable;
		public byte SpriteEnable { get { return sprite_enable; } set { sprite_enable = value; } }
		private uint sprite_width;
		public uint SpriteWidth { get { return sprite_width; } set { sprite_width = value; } }
		private byte marker_bit_16;
		public byte MarkerBit16 { get { return marker_bit_16; } set { marker_bit_16 = value; } }
		private uint sprite_height;
		public uint SpriteHeight { get { return sprite_height; } set { sprite_height = value; } }
		private byte marker_bit_17;
		public byte MarkerBit17 { get { return marker_bit_17; } set { marker_bit_17 = value; } }
		private int sprite_left_coordinate;
		public int SpriteLeftCoordinate { get { return sprite_left_coordinate; } set { sprite_left_coordinate = value; } }
		private byte marker_bit_18;
		public byte MarkerBit18 { get { return marker_bit_18; } set { marker_bit_18 = value; } }
		private int sprite_top_coordinate;
		public int SpriteTopCoordinate { get { return sprite_top_coordinate; } set { sprite_top_coordinate = value; } }
		private byte marker_bit_19;
		public byte MarkerBit19 { get { return marker_bit_19; } set { marker_bit_19 = value; } }
		private uint no_of_sprite_warping_points;
		public uint NoOfSpriteWarpingPoints { get { return no_of_sprite_warping_points; } set { no_of_sprite_warping_points = value; } }
		private uint sprite_warping_accuracy;
		public uint SpriteWarpingAccuracy { get { return sprite_warping_accuracy; } set { sprite_warping_accuracy = value; } }
		private byte sprite_brightness_change;
		public byte SpriteBrightnessChange { get { return sprite_brightness_change; } set { sprite_brightness_change = value; } }
		private byte low_latency_sprite_enable;
		public byte LowLatencySpriteEnable { get { return low_latency_sprite_enable; } set { low_latency_sprite_enable = value; } }
		private byte sadct_disable;
		public byte SadctDisable { get { return sadct_disable; } set { sadct_disable = value; } }
		private byte not_8_bit;
		public byte Not8Bit { get { return not_8_bit; } set { not_8_bit = value; } }
		private uint quant_precision;
		public uint QuantPrecision { get { return quant_precision; } set { quant_precision = value; } }
		private uint bits_per_pixel;
		public uint BitsPerPixel { get { return bits_per_pixel; } set { bits_per_pixel = value; } }
		private byte no_gray_quant_update;
		public byte NoGrayQuantUpdate { get { return no_gray_quant_update; } set { no_gray_quant_update = value; } }
		private byte composition_method;
		public byte CompositionMethod { get { return composition_method; } set { composition_method = value; } }
		private byte linear_composition;
		public byte LinearComposition { get { return linear_composition; } set { linear_composition = value; } }
		private byte quant_type;
		public byte QuantType { get { return quant_type; } set { quant_type = value; } }
		private byte load_intra_quant_mat;
		public byte LoadIntraQuantMat { get { return load_intra_quant_mat; } set { load_intra_quant_mat = value; } }
		private IntraQuantMat intra_quant_mat;
		public IntraQuantMat IntraQuantMat { get { return intra_quant_mat; } set { intra_quant_mat = value; } }
		private byte load_nonintra_quant_mat;
		public byte LoadNonintraQuantMat { get { return load_nonintra_quant_mat; } set { load_nonintra_quant_mat = value; } }
		private NonintraQuantMat nonintra_quant_mat;
		public NonintraQuantMat NonintraQuantMat { get { return nonintra_quant_mat; } set { nonintra_quant_mat = value; } }
		private byte[] load_intra_quant_mat_grayscale;
		public byte[] LoadIntraQuantMatGrayscale { get { return load_intra_quant_mat_grayscale; } set { load_intra_quant_mat_grayscale = value; } }
		private IntraQuantMatGrayscale[] intra_quant_mat_grayscale;
		public IntraQuantMatGrayscale[] IntraQuantMatGrayscale { get { return intra_quant_mat_grayscale; } set { intra_quant_mat_grayscale = value; } }
		private byte[] load_nonintra_quant_mat_grayscale;
		public byte[] LoadNonintraQuantMatGrayscale { get { return load_nonintra_quant_mat_grayscale; } set { load_nonintra_quant_mat_grayscale = value; } }
		private NonintraQuantMatGrayscale[] nonintra_quant_mat_grayscale;
		public NonintraQuantMatGrayscale[] NonintraQuantMatGrayscale { get { return nonintra_quant_mat_grayscale; } set { nonintra_quant_mat_grayscale = value; } }
		private byte complexity_estimation_disable;
		public byte ComplexityEstimationDisable { get { return complexity_estimation_disable; } set { complexity_estimation_disable = value; } }
		private DefineVopComplexityEstimationHeader define_vop_complexity_estimation_header;
		public DefineVopComplexityEstimationHeader DefineVopComplexityEstimationHeader { get { return define_vop_complexity_estimation_header; } set { define_vop_complexity_estimation_header = value; } }
		private byte resync_marker_disable;
		public byte ResyncMarkerDisable { get { return resync_marker_disable; } set { resync_marker_disable = value; } }
		private byte data_partitioned;
		public byte DataPartitioned { get { return data_partitioned; } set { data_partitioned = value; } }
		private byte reversible_vlc;
		public byte ReversibleVlc { get { return reversible_vlc; } set { reversible_vlc = value; } }
		private byte newpred_enable;
		public byte NewpredEnable { get { return newpred_enable; } set { newpred_enable = value; } }
		private uint requested_upstream_message_type;
		public uint RequestedUpstreamMessageType { get { return requested_upstream_message_type; } set { requested_upstream_message_type = value; } }
		private byte newpred_segment_type;
		public byte NewpredSegmentType { get { return newpred_segment_type; } set { newpred_segment_type = value; } }
		private byte reduced_resolution_vop_enable;
		public byte ReducedResolutionVopEnable { get { return reduced_resolution_vop_enable; } set { reduced_resolution_vop_enable = value; } }
		private byte scalability;
		public byte Scalability { get { return scalability; } set { scalability = value; } }
		private byte hierarchy_type;
		public byte HierarchyType { get { return hierarchy_type; } set { hierarchy_type = value; } }
		private uint ref_layer_id;
		public uint RefLayerId { get { return ref_layer_id; } set { ref_layer_id = value; } }
		private byte ref_layer_sampling_direc;
		public byte RefLayerSamplingDirec { get { return ref_layer_sampling_direc; } set { ref_layer_sampling_direc = value; } }
		private uint hor_sampling_factor_n;
		public uint HorSamplingFactorn { get { return hor_sampling_factor_n; } set { hor_sampling_factor_n = value; } }
		private uint hor_sampling_factor_m;
		public uint HorSamplingFactorm { get { return hor_sampling_factor_m; } set { hor_sampling_factor_m = value; } }
		private uint vert_sampling_factor_n;
		public uint VertSamplingFactorn { get { return vert_sampling_factor_n; } set { vert_sampling_factor_n = value; } }
		private uint vert_sampling_factor_m;
		public uint VertSamplingFactorm { get { return vert_sampling_factor_m; } set { vert_sampling_factor_m = value; } }
		private byte enhancement_type;
		public byte EnhancementType { get { return enhancement_type; } set { enhancement_type = value; } }
		private byte use_ref_shape;
		public byte UseRefShape { get { return use_ref_shape; } set { use_ref_shape = value; } }
		private byte use_ref_texture;
		public byte UseRefTexture { get { return use_ref_texture; } set { use_ref_texture = value; } }
		private uint shape_hor_sampling_factor_n;
		public uint ShapeHorSamplingFactorn { get { return shape_hor_sampling_factor_n; } set { shape_hor_sampling_factor_n = value; } }
		private uint shape_hor_sampling_factor_m;
		public uint ShapeHorSamplingFactorm { get { return shape_hor_sampling_factor_m; } set { shape_hor_sampling_factor_m = value; } }
		private uint shape_vert_sampling_factor_n;
		public uint ShapeVertSamplingFactorn { get { return shape_vert_sampling_factor_n; } set { shape_vert_sampling_factor_n = value; } }
		private uint shape_vert_sampling_factor_m;
		public uint ShapeVertSamplingFactorm { get { return shape_vert_sampling_factor_m; } set { shape_vert_sampling_factor_m = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public VideoObjectLayer()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            MPEG4Context ituContext = context as MPEG4Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type MPEG4Context");

            ulong size = 0;

			uint i = 0;
			size += stream.ReadUnsignedInt(size, 32, out this.video_object_layer_start_code, "video_object_layer_start_code"); 
			ituContext.OnVideoObjectLayer(this);
			size += stream.ReadUnsignedInt(size, 1, out this.random_accessible_vol, "random_accessible_vol"); 
			size += stream.ReadUnsignedInt(size, 8, out this.video_object_type_indication, "video_object_type_indication"); 

			if ( video_object_type_indication == 18 )
			{
				size += stream.ReadUnsignedInt(size, 2, out this.fgs_layer_type, "fgs_layer_type"); 
				size += stream.ReadUnsignedInt(size, 3, out this.video_object_layer_priority, "video_object_layer_priority"); 
				size += stream.ReadUnsignedInt(size, 4, out this.aspect_ratio_info, "aspect_ratio_info"); 

				if (aspect_ratio_info == 15)
				{
					size += stream.ReadUnsignedInt(size, 8, out this.par_width, "par_width"); 
					size += stream.ReadUnsignedInt(size, 8, out this.par_height, "par_height"); 
				}
				size += stream.ReadUnsignedInt(size, 1, out this.vol_control_parameters, "vol_control_parameters"); 

				if (vol_control_parameters != 0)
				{
					size += stream.ReadUnsignedInt(size, 2, out this.chroma_format, "chroma_format"); 
					size += stream.ReadUnsignedInt(size, 1, out this.low_delay, "low_delay"); 
				}
				size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
				size += stream.ReadUnsignedInt(size, 16, out this.vop_time_increment_resolution, "vop_time_increment_resolution"); 
				size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_2, "marker_bit_2"); 
				size += stream.ReadUnsignedInt(size, 1, out this.fixed_vop_rate, "fixed_vop_rate"); 

				if (fixed_vop_rate != 0)
				{
					size += stream.ReadUnsignedIntVariable(size, ituContext.VopTimeIncrementBits, out this.fixed_vop_time_increment, "fixed_vop_time_increment"); 
				}
				size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_3, "marker_bit_3"); 
				size += stream.ReadUnsignedInt(size, 13, out this.video_object_layer_width, "video_object_layer_width"); 
				size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_4, "marker_bit_4"); 
				size += stream.ReadUnsignedInt(size, 13, out this.video_object_layer_height, "video_object_layer_height"); 
				size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_5, "marker_bit_5"); 
				size += stream.ReadUnsignedInt(size, 1, out this.interlaced, "interlaced"); 

				if (fgs_layer_type == 2 || fgs_layer_type == 3)
				{
					size += stream.ReadUnsignedInt(size, 4, out this.fgs_ref_layer_id, "fgs_ref_layer_id"); 
				}

				if (fgs_layer_type == 1 || fgs_layer_type == 3)
				{
					size += stream.ReadUnsignedInt(size, 1, out this.fgs_frequency_weighting_enable, "fgs_frequency_weighting_enable"); 

					if ( fgs_frequency_weighting_enable != 0 )
					{
						size += stream.ReadUnsignedInt(size, 1, out this.load_fgs_frequency_weighting_matrix, "load_fgs_frequency_weighting_matrix"); 

						if (load_fgs_frequency_weighting_matrix != 0)
						{
							this.fgs_frequency_weighting_matrix =  new FgsFrequencyWeightingMatrix() ;
							size +=  stream.ReadClass<FgsFrequencyWeightingMatrix>(size, context, this.fgs_frequency_weighting_matrix, "fgs_frequency_weighting_matrix"); 
						}
					}
				}

				if (fgs_layer_type == 2 || fgs_layer_type == 3)
				{
					size += stream.ReadUnsignedInt(size, 1, out this.fgst_frequency_weighting_enable, "fgst_frequency_weighting_enable"); 

					if ( fgst_frequency_weighting_enable != 0 )
					{
						size += stream.ReadUnsignedInt(size, 1, out this.load_fgst_frequency_weighting_matrix, "load_fgst_frequency_weighting_matrix"); 

						if (load_fgst_frequency_weighting_matrix != 0)
						{
							this.fgst_frequency_weighting_matrix =  new FgstFrequencyWeightingMatrix() ;
							size +=  stream.ReadClass<FgstFrequencyWeightingMatrix>(size, context, this.fgst_frequency_weighting_matrix, "fgst_frequency_weighting_matrix"); 
						}
					}
				}
				size += stream.ReadUnsignedInt(size, 1, out this.quarter_sample, "quarter_sample"); 
				size += stream.ReadUnsignedInt(size, 1, out this.fgs_resync_marker_disable, "fgs_resync_marker_disable"); 
				this.next_start_code =  new NextStartCode() ;
				size +=  stream.ReadClass<NextStartCode>(size, context, this.next_start_code, "next_start_code"); 
			}
			else 
			{
				size += stream.ReadUnsignedInt(size, 1, out this.is_object_layer_identifier, "is_object_layer_identifier"); 
				ituContext.OnObjectLayerIdentifier(is_object_layer_identifier);

				if (is_object_layer_identifier != 0)
				{
					size += stream.ReadUnsignedInt(size, 4, out this.video_object_layer_verid, "video_object_layer_verid"); 
					ituContext.VideoObjectLayerVerid = video_object_layer_verid;
					size += stream.ReadUnsignedInt(size, 3, out this.video_object_layer_priority, "video_object_layer_priority"); 
				}
				size += stream.ReadUnsignedInt(size, 4, out this.aspect_ratio_info, "aspect_ratio_info"); 

				if (aspect_ratio_info == 15)
				{
					size += stream.ReadUnsignedInt(size, 8, out this.par_width, "par_width"); 
					size += stream.ReadUnsignedInt(size, 8, out this.par_height, "par_height"); 
				}
				size += stream.ReadUnsignedInt(size, 1, out this.vol_control_parameters, "vol_control_parameters"); 

				if (vol_control_parameters != 0)
				{
					size += stream.ReadUnsignedInt(size, 2, out this.chroma_format, "chroma_format"); 
					size += stream.ReadUnsignedInt(size, 1, out this.low_delay, "low_delay"); 
					size += stream.ReadUnsignedInt(size, 1, out this.vbv_parameters, "vbv_parameters"); 

					if (vbv_parameters != 0)
					{
						size += stream.ReadUnsignedInt(size, 15, out this.first_half_bit_rate, "first_half_bit_rate"); 
						size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_6, "marker_bit_6"); 
						size += stream.ReadUnsignedInt(size, 15, out this.latter_half_bit_rate, "latter_half_bit_rate"); 
						size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_7, "marker_bit_7"); 
						size += stream.ReadUnsignedInt(size, 15, out this.first_half_vbv_buffer_size, "first_half_vbv_buffer_size"); 
						size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_8, "marker_bit_8"); 
						size += stream.ReadUnsignedInt(size, 3, out this.latter_half_vbv_buffer_size, "latter_half_vbv_buffer_size"); 
						size += stream.ReadUnsignedInt(size, 11, out this.first_half_vbv_occupancy, "first_half_vbv_occupancy"); 
						size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_9, "marker_bit_9"); 
						size += stream.ReadUnsignedInt(size, 15, out this.latter_half_vbv_occupancy, "latter_half_vbv_occupancy"); 
						size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_10, "marker_bit_10"); 
					}
				}
				size += stream.ReadUnsignedInt(size, 2, out this.video_object_layer_shape, "video_object_layer_shape"); 

				if (ituContext.VideoObjectLayer.VideoObjectLayerShape == 3
&& ituContext.VideoObjectLayerVerid != 1)
				{
					size += stream.ReadUnsignedInt(size, 4, out this.video_object_layer_shape_extension, "video_object_layer_shape_extension"); 
				}
				size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_11, "marker_bit_11"); 
				size += stream.ReadUnsignedInt(size, 16, out this.vop_time_increment_resolution, "vop_time_increment_resolution"); 
				size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_12, "marker_bit_12"); 
				size += stream.ReadUnsignedInt(size, 1, out this.fixed_vop_rate, "fixed_vop_rate"); 

				if (fixed_vop_rate != 0)
				{
					size += stream.ReadUnsignedIntVariable(size, ituContext.VopTimeIncrementBits, out this.fixed_vop_time_increment, "fixed_vop_time_increment"); 
				}

				if (ituContext.VideoObjectLayer.VideoObjectLayerShape != 2)
				{

					if (ituContext.VideoObjectLayer.VideoObjectLayerShape == 0)
					{
						size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_13, "marker_bit_13"); 
						size += stream.ReadUnsignedInt(size, 13, out this.video_object_layer_width, "video_object_layer_width"); 
						size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_14, "marker_bit_14"); 
						size += stream.ReadUnsignedInt(size, 13, out this.video_object_layer_height, "video_object_layer_height"); 
						size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_15, "marker_bit_15"); 
					}
					size += stream.ReadUnsignedInt(size, 1, out this.interlaced, "interlaced"); 
					size += stream.ReadUnsignedInt(size, 1, out this.obmc_disable, "obmc_disable"); 

					if (ituContext.VideoObjectLayerVerid == 1)
					{
						size += stream.ReadUnsignedInt(size, 1, out this.sprite_enable, "sprite_enable"); 
					}
					else 
					{
						size += stream.ReadUnsignedInt(size, 2, out this.sprite_enable, "sprite_enable"); 
					}

					if (ituContext.VideoObjectLayer.SpriteEnable == 1 || ituContext.VideoObjectLayer.SpriteEnable == 2)
					{

						if (ituContext.VideoObjectLayer.SpriteEnable != 2)
						{
							size += stream.ReadUnsignedInt(size, 13, out this.sprite_width, "sprite_width"); 
							size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_16, "marker_bit_16"); 
							size += stream.ReadUnsignedInt(size, 13, out this.sprite_height, "sprite_height"); 
							size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_17, "marker_bit_17"); 
							size += stream.ReadSignedInt(size, 13, out this.sprite_left_coordinate, "sprite_left_coordinate"); 
							size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_18, "marker_bit_18"); 
							size += stream.ReadSignedInt(size, 13, out this.sprite_top_coordinate, "sprite_top_coordinate"); 
							size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_19, "marker_bit_19"); 
						}
						size += stream.ReadUnsignedInt(size, 6, out this.no_of_sprite_warping_points, "no_of_sprite_warping_points"); 
						size += stream.ReadUnsignedInt(size, 2, out this.sprite_warping_accuracy, "sprite_warping_accuracy"); 
						size += stream.ReadUnsignedInt(size, 1, out this.sprite_brightness_change, "sprite_brightness_change"); 

						if (ituContext.VideoObjectLayer.SpriteEnable != 2)
						{
							size += stream.ReadUnsignedInt(size, 1, out this.low_latency_sprite_enable, "low_latency_sprite_enable"); 
						}
					}

					if (ituContext.VideoObjectLayerVerid != 1 &&
ituContext.VideoObjectLayer.VideoObjectLayerShape != 0)
					{
						size += stream.ReadUnsignedInt(size, 1, out this.sadct_disable, "sadct_disable"); 
					}
					size += stream.ReadUnsignedInt(size, 1, out this.not_8_bit, "not_8_bit"); 

					if (not_8_bit != 0)
					{
						size += stream.ReadUnsignedInt(size, 4, out this.quant_precision, "quant_precision"); 
						size += stream.ReadUnsignedInt(size, 4, out this.bits_per_pixel, "bits_per_pixel"); 
					}

					if (ituContext.VideoObjectLayer.VideoObjectLayerShape == 3)
					{
						size += stream.ReadUnsignedInt(size, 1, out this.no_gray_quant_update, "no_gray_quant_update"); 
						size += stream.ReadUnsignedInt(size, 1, out this.composition_method, "composition_method"); 
						size += stream.ReadUnsignedInt(size, 1, out this.linear_composition, "linear_composition"); 
					}
					size += stream.ReadUnsignedInt(size, 1, out this.quant_type, "quant_type"); 

					if (quant_type != 0)
					{
						size += stream.ReadUnsignedInt(size, 1, out this.load_intra_quant_mat, "load_intra_quant_mat"); 

						if (load_intra_quant_mat != 0)
						{
							this.intra_quant_mat =  new IntraQuantMat() ;
							size +=  stream.ReadClass<IntraQuantMat>(size, context, this.intra_quant_mat, "intra_quant_mat"); 
						}
						size += stream.ReadUnsignedInt(size, 1, out this.load_nonintra_quant_mat, "load_nonintra_quant_mat"); 

						if (load_nonintra_quant_mat != 0)
						{
							this.nonintra_quant_mat =  new NonintraQuantMat() ;
							size +=  stream.ReadClass<NonintraQuantMat>(size, context, this.nonintra_quant_mat, "nonintra_quant_mat"); 
						}

						if (ituContext.VideoObjectLayer.VideoObjectLayerShape == 3)
						{

							stream.CheckArrayAllocation((ulong)(ituContext.AuxCompCount), "load_intra_quant_mat_grayscale");
							this.load_intra_quant_mat_grayscale = new byte[ituContext.AuxCompCount];
							stream.CheckArrayAllocation((ulong)(ituContext.AuxCompCount), "intra_quant_mat_grayscale");
							this.intra_quant_mat_grayscale = new IntraQuantMatGrayscale[ituContext.AuxCompCount];
							stream.CheckArrayAllocation((ulong)(ituContext.AuxCompCount), "load_nonintra_quant_mat_grayscale");
							this.load_nonintra_quant_mat_grayscale = new byte[ituContext.AuxCompCount];
							stream.CheckArrayAllocation((ulong)(ituContext.AuxCompCount), "nonintra_quant_mat_grayscale");
							this.nonintra_quant_mat_grayscale = new NonintraQuantMatGrayscale[ituContext.AuxCompCount];
							for (i=0; i<ituContext.AuxCompCount; i++)
							{
								size += stream.ReadUnsignedInt(size, 1, out this.load_intra_quant_mat_grayscale[i], "load_intra_quant_mat_grayscale"); 

								if (load_intra_quant_mat_grayscale[i] != 0)
								{
									this.intra_quant_mat_grayscale[i] =  new IntraQuantMatGrayscale( i ) ;
									size +=  stream.ReadClass<IntraQuantMatGrayscale>(size, context, this.intra_quant_mat_grayscale[i], "intra_quant_mat_grayscale"); 
								}
								size += stream.ReadUnsignedInt(size, 1, out this.load_nonintra_quant_mat_grayscale[i], "load_nonintra_quant_mat_grayscale"); 

								if (load_nonintra_quant_mat_grayscale[i] != 0)
								{
									this.nonintra_quant_mat_grayscale[i] =  new NonintraQuantMatGrayscale( i ) ;
									size +=  stream.ReadClass<NonintraQuantMatGrayscale>(size, context, this.nonintra_quant_mat_grayscale[i], "nonintra_quant_mat_grayscale"); 
								}
							}
						}
					}

					if (ituContext.VideoObjectLayerVerid != 1)
					{
						size += stream.ReadUnsignedInt(size, 1, out this.quarter_sample, "quarter_sample"); 
					}
					size += stream.ReadUnsignedInt(size, 1, out this.complexity_estimation_disable, "complexity_estimation_disable"); 

					if (ituContext.VideoObjectLayer.ComplexityEstimationDisable== 0)
					{
						this.define_vop_complexity_estimation_header =  new DefineVopComplexityEstimationHeader() ;
						size +=  stream.ReadClass<DefineVopComplexityEstimationHeader>(size, context, this.define_vop_complexity_estimation_header, "define_vop_complexity_estimation_header"); 
					}
					size += stream.ReadUnsignedInt(size, 1, out this.resync_marker_disable, "resync_marker_disable"); 
					size += stream.ReadUnsignedInt(size, 1, out this.data_partitioned, "data_partitioned"); 

					if (data_partitioned != 0)
					{
						size += stream.ReadUnsignedInt(size, 1, out this.reversible_vlc, "reversible_vlc"); 
					}

					if (ituContext.VideoObjectLayerVerid != 1)
					{
						size += stream.ReadUnsignedInt(size, 1, out this.newpred_enable, "newpred_enable"); 

						if (ituContext.VideoObjectLayer.NewpredEnable != 0)
						{
							size += stream.ReadUnsignedInt(size, 2, out this.requested_upstream_message_type, "requested_upstream_message_type"); 
							size += stream.ReadUnsignedInt(size, 1, out this.newpred_segment_type, "newpred_segment_type"); 
						}
						size += stream.ReadUnsignedInt(size, 1, out this.reduced_resolution_vop_enable, "reduced_resolution_vop_enable"); 
					}
					size += stream.ReadUnsignedInt(size, 1, out this.scalability, "scalability"); 

					if (ituContext.VideoObjectLayer.Scalability != 0)
					{
						size += stream.ReadUnsignedInt(size, 1, out this.hierarchy_type, "hierarchy_type"); 
						size += stream.ReadUnsignedInt(size, 4, out this.ref_layer_id, "ref_layer_id"); 
						size += stream.ReadUnsignedInt(size, 1, out this.ref_layer_sampling_direc, "ref_layer_sampling_direc"); 
						size += stream.ReadUnsignedInt(size, 5, out this.hor_sampling_factor_n, "hor_sampling_factor_n"); 
						size += stream.ReadUnsignedInt(size, 5, out this.hor_sampling_factor_m, "hor_sampling_factor_m"); 
						size += stream.ReadUnsignedInt(size, 5, out this.vert_sampling_factor_n, "vert_sampling_factor_n"); 
						size += stream.ReadUnsignedInt(size, 5, out this.vert_sampling_factor_m, "vert_sampling_factor_m"); 
						size += stream.ReadUnsignedInt(size, 1, out this.enhancement_type, "enhancement_type"); 

						if (ituContext.VideoObjectLayer.VideoObjectLayerShape == 1 && hierarchy_type == 0)
						{
							size += stream.ReadUnsignedInt(size, 1, out this.use_ref_shape, "use_ref_shape"); 
							size += stream.ReadUnsignedInt(size, 1, out this.use_ref_texture, "use_ref_texture"); 
							size += stream.ReadUnsignedInt(size, 5, out this.shape_hor_sampling_factor_n, "shape_hor_sampling_factor_n"); 
							size += stream.ReadUnsignedInt(size, 5, out this.shape_hor_sampling_factor_m, "shape_hor_sampling_factor_m"); 
							size += stream.ReadUnsignedInt(size, 5, out this.shape_vert_sampling_factor_n, "shape_vert_sampling_factor_n"); 
							size += stream.ReadUnsignedInt(size, 5, out this.shape_vert_sampling_factor_m, "shape_vert_sampling_factor_m"); 
						}
					}
				}
				else 
				{

					if (ituContext.VideoObjectLayerVerid != 1)
					{
						size += stream.ReadUnsignedInt(size, 1, out this.scalability, "scalability"); 

						if (ituContext.VideoObjectLayer.Scalability != 0)
						{
							size += stream.ReadUnsignedInt(size, 4, out this.ref_layer_id, "ref_layer_id"); 
							size += stream.ReadUnsignedInt(size, 5, out this.shape_hor_sampling_factor_n, "shape_hor_sampling_factor_n"); 
							size += stream.ReadUnsignedInt(size, 5, out this.shape_hor_sampling_factor_m, "shape_hor_sampling_factor_m"); 
							size += stream.ReadUnsignedInt(size, 5, out this.shape_vert_sampling_factor_n, "shape_vert_sampling_factor_n"); 
							size += stream.ReadUnsignedInt(size, 5, out this.shape_vert_sampling_factor_m, "shape_vert_sampling_factor_m"); 
						}
					}
					size += stream.ReadUnsignedInt(size, 1, out this.resync_marker_disable, "resync_marker_disable"); 
				}
				this.next_start_code =  new NextStartCode() ;
				size +=  stream.ReadClass<NextStartCode>(size, context, this.next_start_code, "next_start_code"); 
			}

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            MPEG4Context ituContext = context as MPEG4Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type MPEG4Context");
            ulong size = 0;

			uint i = 0;
			size += stream.WriteUnsignedInt(32, this.video_object_layer_start_code, "video_object_layer_start_code"); 
			ituContext.OnVideoObjectLayer(this);
			size += stream.WriteUnsignedInt(1, this.random_accessible_vol, "random_accessible_vol"); 
			size += stream.WriteUnsignedInt(8, this.video_object_type_indication, "video_object_type_indication"); 

			if ( video_object_type_indication == 18 )
			{
				size += stream.WriteUnsignedInt(2, this.fgs_layer_type, "fgs_layer_type"); 
				size += stream.WriteUnsignedInt(3, this.video_object_layer_priority, "video_object_layer_priority"); 
				size += stream.WriteUnsignedInt(4, this.aspect_ratio_info, "aspect_ratio_info"); 

				if (aspect_ratio_info == 15)
				{
					size += stream.WriteUnsignedInt(8, this.par_width, "par_width"); 
					size += stream.WriteUnsignedInt(8, this.par_height, "par_height"); 
				}
				size += stream.WriteUnsignedInt(1, this.vol_control_parameters, "vol_control_parameters"); 

				if (vol_control_parameters != 0)
				{
					size += stream.WriteUnsignedInt(2, this.chroma_format, "chroma_format"); 
					size += stream.WriteUnsignedInt(1, this.low_delay, "low_delay"); 
				}
				size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
				size += stream.WriteUnsignedInt(16, this.vop_time_increment_resolution, "vop_time_increment_resolution"); 
				size += stream.WriteUnsignedInt(1, this.marker_bit_2, "marker_bit_2"); 
				size += stream.WriteUnsignedInt(1, this.fixed_vop_rate, "fixed_vop_rate"); 

				if (fixed_vop_rate != 0)
				{
					size += stream.WriteUnsignedIntVariable(ituContext.VopTimeIncrementBits, this.fixed_vop_time_increment, "fixed_vop_time_increment"); 
				}
				size += stream.WriteUnsignedInt(1, this.marker_bit_3, "marker_bit_3"); 
				size += stream.WriteUnsignedInt(13, this.video_object_layer_width, "video_object_layer_width"); 
				size += stream.WriteUnsignedInt(1, this.marker_bit_4, "marker_bit_4"); 
				size += stream.WriteUnsignedInt(13, this.video_object_layer_height, "video_object_layer_height"); 
				size += stream.WriteUnsignedInt(1, this.marker_bit_5, "marker_bit_5"); 
				size += stream.WriteUnsignedInt(1, this.interlaced, "interlaced"); 

				if (fgs_layer_type == 2 || fgs_layer_type == 3)
				{
					size += stream.WriteUnsignedInt(4, this.fgs_ref_layer_id, "fgs_ref_layer_id"); 
				}

				if (fgs_layer_type == 1 || fgs_layer_type == 3)
				{
					size += stream.WriteUnsignedInt(1, this.fgs_frequency_weighting_enable, "fgs_frequency_weighting_enable"); 

					if ( fgs_frequency_weighting_enable != 0 )
					{
						size += stream.WriteUnsignedInt(1, this.load_fgs_frequency_weighting_matrix, "load_fgs_frequency_weighting_matrix"); 

						if (load_fgs_frequency_weighting_matrix != 0)
						{
							size += stream.WriteClass<FgsFrequencyWeightingMatrix>(context, this.fgs_frequency_weighting_matrix, "fgs_frequency_weighting_matrix"); 
						}
					}
				}

				if (fgs_layer_type == 2 || fgs_layer_type == 3)
				{
					size += stream.WriteUnsignedInt(1, this.fgst_frequency_weighting_enable, "fgst_frequency_weighting_enable"); 

					if ( fgst_frequency_weighting_enable != 0 )
					{
						size += stream.WriteUnsignedInt(1, this.load_fgst_frequency_weighting_matrix, "load_fgst_frequency_weighting_matrix"); 

						if (load_fgst_frequency_weighting_matrix != 0)
						{
							size += stream.WriteClass<FgstFrequencyWeightingMatrix>(context, this.fgst_frequency_weighting_matrix, "fgst_frequency_weighting_matrix"); 
						}
					}
				}
				size += stream.WriteUnsignedInt(1, this.quarter_sample, "quarter_sample"); 
				size += stream.WriteUnsignedInt(1, this.fgs_resync_marker_disable, "fgs_resync_marker_disable"); 
				size += stream.WriteClass<NextStartCode>(context, this.next_start_code, "next_start_code"); 
			}
			else 
			{
				size += stream.WriteUnsignedInt(1, this.is_object_layer_identifier, "is_object_layer_identifier"); 
				ituContext.OnObjectLayerIdentifier(is_object_layer_identifier);

				if (is_object_layer_identifier != 0)
				{
					size += stream.WriteUnsignedInt(4, this.video_object_layer_verid, "video_object_layer_verid"); 
					ituContext.VideoObjectLayerVerid = video_object_layer_verid;
					size += stream.WriteUnsignedInt(3, this.video_object_layer_priority, "video_object_layer_priority"); 
				}
				size += stream.WriteUnsignedInt(4, this.aspect_ratio_info, "aspect_ratio_info"); 

				if (aspect_ratio_info == 15)
				{
					size += stream.WriteUnsignedInt(8, this.par_width, "par_width"); 
					size += stream.WriteUnsignedInt(8, this.par_height, "par_height"); 
				}
				size += stream.WriteUnsignedInt(1, this.vol_control_parameters, "vol_control_parameters"); 

				if (vol_control_parameters != 0)
				{
					size += stream.WriteUnsignedInt(2, this.chroma_format, "chroma_format"); 
					size += stream.WriteUnsignedInt(1, this.low_delay, "low_delay"); 
					size += stream.WriteUnsignedInt(1, this.vbv_parameters, "vbv_parameters"); 

					if (vbv_parameters != 0)
					{
						size += stream.WriteUnsignedInt(15, this.first_half_bit_rate, "first_half_bit_rate"); 
						size += stream.WriteUnsignedInt(1, this.marker_bit_6, "marker_bit_6"); 
						size += stream.WriteUnsignedInt(15, this.latter_half_bit_rate, "latter_half_bit_rate"); 
						size += stream.WriteUnsignedInt(1, this.marker_bit_7, "marker_bit_7"); 
						size += stream.WriteUnsignedInt(15, this.first_half_vbv_buffer_size, "first_half_vbv_buffer_size"); 
						size += stream.WriteUnsignedInt(1, this.marker_bit_8, "marker_bit_8"); 
						size += stream.WriteUnsignedInt(3, this.latter_half_vbv_buffer_size, "latter_half_vbv_buffer_size"); 
						size += stream.WriteUnsignedInt(11, this.first_half_vbv_occupancy, "first_half_vbv_occupancy"); 
						size += stream.WriteUnsignedInt(1, this.marker_bit_9, "marker_bit_9"); 
						size += stream.WriteUnsignedInt(15, this.latter_half_vbv_occupancy, "latter_half_vbv_occupancy"); 
						size += stream.WriteUnsignedInt(1, this.marker_bit_10, "marker_bit_10"); 
					}
				}
				size += stream.WriteUnsignedInt(2, this.video_object_layer_shape, "video_object_layer_shape"); 

				if (ituContext.VideoObjectLayer.VideoObjectLayerShape == 3
&& ituContext.VideoObjectLayerVerid != 1)
				{
					size += stream.WriteUnsignedInt(4, this.video_object_layer_shape_extension, "video_object_layer_shape_extension"); 
				}
				size += stream.WriteUnsignedInt(1, this.marker_bit_11, "marker_bit_11"); 
				size += stream.WriteUnsignedInt(16, this.vop_time_increment_resolution, "vop_time_increment_resolution"); 
				size += stream.WriteUnsignedInt(1, this.marker_bit_12, "marker_bit_12"); 
				size += stream.WriteUnsignedInt(1, this.fixed_vop_rate, "fixed_vop_rate"); 

				if (fixed_vop_rate != 0)
				{
					size += stream.WriteUnsignedIntVariable(ituContext.VopTimeIncrementBits, this.fixed_vop_time_increment, "fixed_vop_time_increment"); 
				}

				if (ituContext.VideoObjectLayer.VideoObjectLayerShape != 2)
				{

					if (ituContext.VideoObjectLayer.VideoObjectLayerShape == 0)
					{
						size += stream.WriteUnsignedInt(1, this.marker_bit_13, "marker_bit_13"); 
						size += stream.WriteUnsignedInt(13, this.video_object_layer_width, "video_object_layer_width"); 
						size += stream.WriteUnsignedInt(1, this.marker_bit_14, "marker_bit_14"); 
						size += stream.WriteUnsignedInt(13, this.video_object_layer_height, "video_object_layer_height"); 
						size += stream.WriteUnsignedInt(1, this.marker_bit_15, "marker_bit_15"); 
					}
					size += stream.WriteUnsignedInt(1, this.interlaced, "interlaced"); 
					size += stream.WriteUnsignedInt(1, this.obmc_disable, "obmc_disable"); 

					if (ituContext.VideoObjectLayerVerid == 1)
					{
						size += stream.WriteUnsignedInt(1, this.sprite_enable, "sprite_enable"); 
					}
					else 
					{
						size += stream.WriteUnsignedInt(2, this.sprite_enable, "sprite_enable"); 
					}

					if (ituContext.VideoObjectLayer.SpriteEnable == 1 || ituContext.VideoObjectLayer.SpriteEnable == 2)
					{

						if (ituContext.VideoObjectLayer.SpriteEnable != 2)
						{
							size += stream.WriteUnsignedInt(13, this.sprite_width, "sprite_width"); 
							size += stream.WriteUnsignedInt(1, this.marker_bit_16, "marker_bit_16"); 
							size += stream.WriteUnsignedInt(13, this.sprite_height, "sprite_height"); 
							size += stream.WriteUnsignedInt(1, this.marker_bit_17, "marker_bit_17"); 
							size += stream.WriteSignedInt(13,  this.sprite_left_coordinate, "sprite_left_coordinate"); 
							size += stream.WriteUnsignedInt(1, this.marker_bit_18, "marker_bit_18"); 
							size += stream.WriteSignedInt(13,  this.sprite_top_coordinate, "sprite_top_coordinate"); 
							size += stream.WriteUnsignedInt(1, this.marker_bit_19, "marker_bit_19"); 
						}
						size += stream.WriteUnsignedInt(6, this.no_of_sprite_warping_points, "no_of_sprite_warping_points"); 
						size += stream.WriteUnsignedInt(2, this.sprite_warping_accuracy, "sprite_warping_accuracy"); 
						size += stream.WriteUnsignedInt(1, this.sprite_brightness_change, "sprite_brightness_change"); 

						if (ituContext.VideoObjectLayer.SpriteEnable != 2)
						{
							size += stream.WriteUnsignedInt(1, this.low_latency_sprite_enable, "low_latency_sprite_enable"); 
						}
					}

					if (ituContext.VideoObjectLayerVerid != 1 &&
ituContext.VideoObjectLayer.VideoObjectLayerShape != 0)
					{
						size += stream.WriteUnsignedInt(1, this.sadct_disable, "sadct_disable"); 
					}
					size += stream.WriteUnsignedInt(1, this.not_8_bit, "not_8_bit"); 

					if (not_8_bit != 0)
					{
						size += stream.WriteUnsignedInt(4, this.quant_precision, "quant_precision"); 
						size += stream.WriteUnsignedInt(4, this.bits_per_pixel, "bits_per_pixel"); 
					}

					if (ituContext.VideoObjectLayer.VideoObjectLayerShape == 3)
					{
						size += stream.WriteUnsignedInt(1, this.no_gray_quant_update, "no_gray_quant_update"); 
						size += stream.WriteUnsignedInt(1, this.composition_method, "composition_method"); 
						size += stream.WriteUnsignedInt(1, this.linear_composition, "linear_composition"); 
					}
					size += stream.WriteUnsignedInt(1, this.quant_type, "quant_type"); 

					if (quant_type != 0)
					{
						size += stream.WriteUnsignedInt(1, this.load_intra_quant_mat, "load_intra_quant_mat"); 

						if (load_intra_quant_mat != 0)
						{
							size += stream.WriteClass<IntraQuantMat>(context, this.intra_quant_mat, "intra_quant_mat"); 
						}
						size += stream.WriteUnsignedInt(1, this.load_nonintra_quant_mat, "load_nonintra_quant_mat"); 

						if (load_nonintra_quant_mat != 0)
						{
							size += stream.WriteClass<NonintraQuantMat>(context, this.nonintra_quant_mat, "nonintra_quant_mat"); 
						}

						if (ituContext.VideoObjectLayer.VideoObjectLayerShape == 3)
						{

							for (i=0; i<ituContext.AuxCompCount; i++)
							{
								size += stream.WriteUnsignedInt(1, this.load_intra_quant_mat_grayscale[i], "load_intra_quant_mat_grayscale"); 

								if (load_intra_quant_mat_grayscale[i] != 0)
								{
									size += stream.WriteClass<IntraQuantMatGrayscale>(context, this.intra_quant_mat_grayscale[i], "intra_quant_mat_grayscale"); 
								}
								size += stream.WriteUnsignedInt(1, this.load_nonintra_quant_mat_grayscale[i], "load_nonintra_quant_mat_grayscale"); 

								if (load_nonintra_quant_mat_grayscale[i] != 0)
								{
									size += stream.WriteClass<NonintraQuantMatGrayscale>(context, this.nonintra_quant_mat_grayscale[i], "nonintra_quant_mat_grayscale"); 
								}
							}
						}
					}

					if (ituContext.VideoObjectLayerVerid != 1)
					{
						size += stream.WriteUnsignedInt(1, this.quarter_sample, "quarter_sample"); 
					}
					size += stream.WriteUnsignedInt(1, this.complexity_estimation_disable, "complexity_estimation_disable"); 

					if (ituContext.VideoObjectLayer.ComplexityEstimationDisable== 0)
					{
						size += stream.WriteClass<DefineVopComplexityEstimationHeader>(context, this.define_vop_complexity_estimation_header, "define_vop_complexity_estimation_header"); 
					}
					size += stream.WriteUnsignedInt(1, this.resync_marker_disable, "resync_marker_disable"); 
					size += stream.WriteUnsignedInt(1, this.data_partitioned, "data_partitioned"); 

					if (data_partitioned != 0)
					{
						size += stream.WriteUnsignedInt(1, this.reversible_vlc, "reversible_vlc"); 
					}

					if (ituContext.VideoObjectLayerVerid != 1)
					{
						size += stream.WriteUnsignedInt(1, this.newpred_enable, "newpred_enable"); 

						if (ituContext.VideoObjectLayer.NewpredEnable != 0)
						{
							size += stream.WriteUnsignedInt(2, this.requested_upstream_message_type, "requested_upstream_message_type"); 
							size += stream.WriteUnsignedInt(1, this.newpred_segment_type, "newpred_segment_type"); 
						}
						size += stream.WriteUnsignedInt(1, this.reduced_resolution_vop_enable, "reduced_resolution_vop_enable"); 
					}
					size += stream.WriteUnsignedInt(1, this.scalability, "scalability"); 

					if (ituContext.VideoObjectLayer.Scalability != 0)
					{
						size += stream.WriteUnsignedInt(1, this.hierarchy_type, "hierarchy_type"); 
						size += stream.WriteUnsignedInt(4, this.ref_layer_id, "ref_layer_id"); 
						size += stream.WriteUnsignedInt(1, this.ref_layer_sampling_direc, "ref_layer_sampling_direc"); 
						size += stream.WriteUnsignedInt(5, this.hor_sampling_factor_n, "hor_sampling_factor_n"); 
						size += stream.WriteUnsignedInt(5, this.hor_sampling_factor_m, "hor_sampling_factor_m"); 
						size += stream.WriteUnsignedInt(5, this.vert_sampling_factor_n, "vert_sampling_factor_n"); 
						size += stream.WriteUnsignedInt(5, this.vert_sampling_factor_m, "vert_sampling_factor_m"); 
						size += stream.WriteUnsignedInt(1, this.enhancement_type, "enhancement_type"); 

						if (ituContext.VideoObjectLayer.VideoObjectLayerShape == 1 && hierarchy_type == 0)
						{
							size += stream.WriteUnsignedInt(1, this.use_ref_shape, "use_ref_shape"); 
							size += stream.WriteUnsignedInt(1, this.use_ref_texture, "use_ref_texture"); 
							size += stream.WriteUnsignedInt(5, this.shape_hor_sampling_factor_n, "shape_hor_sampling_factor_n"); 
							size += stream.WriteUnsignedInt(5, this.shape_hor_sampling_factor_m, "shape_hor_sampling_factor_m"); 
							size += stream.WriteUnsignedInt(5, this.shape_vert_sampling_factor_n, "shape_vert_sampling_factor_n"); 
							size += stream.WriteUnsignedInt(5, this.shape_vert_sampling_factor_m, "shape_vert_sampling_factor_m"); 
						}
					}
				}
				else 
				{

					if (ituContext.VideoObjectLayerVerid != 1)
					{
						size += stream.WriteUnsignedInt(1, this.scalability, "scalability"); 

						if (ituContext.VideoObjectLayer.Scalability != 0)
						{
							size += stream.WriteUnsignedInt(4, this.ref_layer_id, "ref_layer_id"); 
							size += stream.WriteUnsignedInt(5, this.shape_hor_sampling_factor_n, "shape_hor_sampling_factor_n"); 
							size += stream.WriteUnsignedInt(5, this.shape_hor_sampling_factor_m, "shape_hor_sampling_factor_m"); 
							size += stream.WriteUnsignedInt(5, this.shape_vert_sampling_factor_n, "shape_vert_sampling_factor_n"); 
							size += stream.WriteUnsignedInt(5, this.shape_vert_sampling_factor_m, "shape_vert_sampling_factor_m"); 
						}
					}
					size += stream.WriteUnsignedInt(1, this.resync_marker_disable, "resync_marker_disable"); 
				}
				size += stream.WriteClass<NextStartCode>(context, this.next_start_code, "next_start_code"); 
			}

            return size;
         }

    }

    /*


Stuffing() {
stuffing_start_code u(32)
while ( before_start_code( stuffing_byte ) )
stuffing_byte u(8)
}
    */
    public class Stuffing : IItuSerializable
    {
		private uint stuffing_start_code;
		public uint StuffingStartCode { get { return stuffing_start_code; } set { stuffing_start_code = value; } }
		private Dictionary<int, uint> stuffing_byte;
		public Dictionary<int, uint> StuffingByte { get { return stuffing_byte ??= new Dictionary<int, uint>(); } set { stuffing_byte = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public Stuffing()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            MPEG4Context ituContext = context as MPEG4Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type MPEG4Context");

            ulong size = 0;

			int whileIndex = -1;
			size += stream.ReadUnsignedInt(size, 32, out this.stuffing_start_code, "stuffing_start_code"); 

			while ( !ituContext.AtStartCode(stream) )
			{
				whileIndex++;

				size += stream.ReadUnsignedInt(size, 8, whileIndex, (this.stuffing_byte ??= new()), "stuffing_byte"); 
			}

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            MPEG4Context ituContext = context as MPEG4Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type MPEG4Context");
            ulong size = 0;

			int whileIndex = -1;
			size += stream.WriteUnsignedInt(32, this.stuffing_start_code, "stuffing_start_code"); 

			while ( whileIndex + 1 < this.StuffingByte.Count )
			{
				whileIndex++;

				size += stream.WriteUnsignedInt(8, whileIndex, (this.stuffing_byte ??= new()), "stuffing_byte"); 
			}

            return size;
         }

    }

    /*


define_vop_complexity_estimation_header() {
estimation_method u(2)
    if (estimation_method == 0 || estimation_method == 1) {
shape_complexity_estimation_disable u(1)
if (!shape_complexity_estimation_disable) {
opaque u(1)
transparent u(1)
intra_cae u(1)
inter_cae u(1)
no_update u(1)
upsampling u(1)
}
texture_complexity_estimation_set_1_disable u(1)
if (!texture_complexity_estimation_set_1_disable) {
intra_blocks u(1)
inter_blocks u(1)
inter4v_blocks u(1)
not_coded_blocks u(1)
}
marker_bit u(1)
texture_complexity_estimation_set_2_disable u(1)
if (!texture_complexity_estimation_set_2_disable) {
dct_coefs u(1)
dct_lines u(1)
vlc_symbols u(1)
vlc_bits u(1)
}
motion_compensation_complexity_disable u(1)
if (!motion_compensation_complexity_disable) {
apm u(1)
npm u(1)
interpolate_mc_q u(1)
forw_back_mc_q u(1)
halfpel2 u(1)
halfpel4 u(1)
}
marker_bit_2 u(1)
        if (estimation_method == 1) {
version2_complexity_estimation_disable u(1)
if (!version2_complexity_estimation_disable) {
sadct u(1)
quarterpel u(1)
}
}
}
}
    */
    public class DefineVopComplexityEstimationHeader : IItuSerializable
    {
		private uint estimation_method;
		public uint EstimationMethod { get { return estimation_method; } set { estimation_method = value; } }
		private byte shape_complexity_estimation_disable;
		public byte ShapeComplexityEstimationDisable { get { return shape_complexity_estimation_disable; } set { shape_complexity_estimation_disable = value; } }
		private byte opaque;
		public byte Opaque { get { return opaque; } set { opaque = value; } }
		private byte transparent;
		public byte Transparent { get { return transparent; } set { transparent = value; } }
		private byte intra_cae;
		public byte IntraCae { get { return intra_cae; } set { intra_cae = value; } }
		private byte inter_cae;
		public byte InterCae { get { return inter_cae; } set { inter_cae = value; } }
		private byte no_update;
		public byte NoUpdate { get { return no_update; } set { no_update = value; } }
		private byte upsampling;
		public byte Upsampling { get { return upsampling; } set { upsampling = value; } }
		private byte texture_complexity_estimation_set_1_disable;
		public byte TextureComplexityEstimationSet1Disable { get { return texture_complexity_estimation_set_1_disable; } set { texture_complexity_estimation_set_1_disable = value; } }
		private byte intra_blocks;
		public byte IntraBlocks { get { return intra_blocks; } set { intra_blocks = value; } }
		private byte inter_blocks;
		public byte InterBlocks { get { return inter_blocks; } set { inter_blocks = value; } }
		private byte inter4v_blocks;
		public byte Inter4vBlocks { get { return inter4v_blocks; } set { inter4v_blocks = value; } }
		private byte not_coded_blocks;
		public byte NotCodedBlocks { get { return not_coded_blocks; } set { not_coded_blocks = value; } }
		private byte marker_bit;
		public byte MarkerBit { get { return marker_bit; } set { marker_bit = value; } }
		private byte texture_complexity_estimation_set_2_disable;
		public byte TextureComplexityEstimationSet2Disable { get { return texture_complexity_estimation_set_2_disable; } set { texture_complexity_estimation_set_2_disable = value; } }
		private byte dct_coefs;
		public byte DctCoefs { get { return dct_coefs; } set { dct_coefs = value; } }
		private byte dct_lines;
		public byte DctLines { get { return dct_lines; } set { dct_lines = value; } }
		private byte vlc_symbols;
		public byte VlcSymbols { get { return vlc_symbols; } set { vlc_symbols = value; } }
		private byte vlc_bits;
		public byte VlcBits { get { return vlc_bits; } set { vlc_bits = value; } }
		private byte motion_compensation_complexity_disable;
		public byte MotionCompensationComplexityDisable { get { return motion_compensation_complexity_disable; } set { motion_compensation_complexity_disable = value; } }
		private byte apm;
		public byte Apm { get { return apm; } set { apm = value; } }
		private byte npm;
		public byte Npm { get { return npm; } set { npm = value; } }
		private byte interpolate_mc_q;
		public byte InterpolateMcq { get { return interpolate_mc_q; } set { interpolate_mc_q = value; } }
		private byte forw_back_mc_q;
		public byte ForwBackMcq { get { return forw_back_mc_q; } set { forw_back_mc_q = value; } }
		private byte halfpel2;
		public byte Halfpel2 { get { return halfpel2; } set { halfpel2 = value; } }
		private byte halfpel4;
		public byte Halfpel4 { get { return halfpel4; } set { halfpel4 = value; } }
		private byte marker_bit_2;
		public byte MarkerBit2 { get { return marker_bit_2; } set { marker_bit_2 = value; } }
		private byte version2_complexity_estimation_disable;
		public byte Version2ComplexityEstimationDisable { get { return version2_complexity_estimation_disable; } set { version2_complexity_estimation_disable = value; } }
		private byte sadct;
		public byte Sadct { get { return sadct; } set { sadct = value; } }
		private byte quarterpel;
		public byte Quarterpel { get { return quarterpel; } set { quarterpel = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public DefineVopComplexityEstimationHeader()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            MPEG4Context ituContext = context as MPEG4Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type MPEG4Context");

            ulong size = 0;

			size += stream.ReadUnsignedInt(size, 2, out this.estimation_method, "estimation_method"); 
			ituContext.ComplexityEstimation = this;

			if (ituContext.ComplexityEstimation.EstimationMethod == 0 || ituContext.ComplexityEstimation.EstimationMethod == 1)
			{
				size += stream.ReadUnsignedInt(size, 1, out this.shape_complexity_estimation_disable, "shape_complexity_estimation_disable"); 

				if (shape_complexity_estimation_disable== 0)
				{
					size += stream.ReadUnsignedInt(size, 1, out this.opaque, "opaque"); 
					size += stream.ReadUnsignedInt(size, 1, out this.transparent, "transparent"); 
					size += stream.ReadUnsignedInt(size, 1, out this.intra_cae, "intra_cae"); 
					size += stream.ReadUnsignedInt(size, 1, out this.inter_cae, "inter_cae"); 
					size += stream.ReadUnsignedInt(size, 1, out this.no_update, "no_update"); 
					size += stream.ReadUnsignedInt(size, 1, out this.upsampling, "upsampling"); 
				}
				size += stream.ReadUnsignedInt(size, 1, out this.texture_complexity_estimation_set_1_disable, "texture_complexity_estimation_set_1_disable"); 

				if (texture_complexity_estimation_set_1_disable== 0)
				{
					size += stream.ReadUnsignedInt(size, 1, out this.intra_blocks, "intra_blocks"); 
					size += stream.ReadUnsignedInt(size, 1, out this.inter_blocks, "inter_blocks"); 
					size += stream.ReadUnsignedInt(size, 1, out this.inter4v_blocks, "inter4v_blocks"); 
					size += stream.ReadUnsignedInt(size, 1, out this.not_coded_blocks, "not_coded_blocks"); 
				}
				size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
				size += stream.ReadUnsignedInt(size, 1, out this.texture_complexity_estimation_set_2_disable, "texture_complexity_estimation_set_2_disable"); 

				if (texture_complexity_estimation_set_2_disable== 0)
				{
					size += stream.ReadUnsignedInt(size, 1, out this.dct_coefs, "dct_coefs"); 
					size += stream.ReadUnsignedInt(size, 1, out this.dct_lines, "dct_lines"); 
					size += stream.ReadUnsignedInt(size, 1, out this.vlc_symbols, "vlc_symbols"); 
					size += stream.ReadUnsignedInt(size, 1, out this.vlc_bits, "vlc_bits"); 
				}
				size += stream.ReadUnsignedInt(size, 1, out this.motion_compensation_complexity_disable, "motion_compensation_complexity_disable"); 

				if (motion_compensation_complexity_disable== 0)
				{
					size += stream.ReadUnsignedInt(size, 1, out this.apm, "apm"); 
					size += stream.ReadUnsignedInt(size, 1, out this.npm, "npm"); 
					size += stream.ReadUnsignedInt(size, 1, out this.interpolate_mc_q, "interpolate_mc_q"); 
					size += stream.ReadUnsignedInt(size, 1, out this.forw_back_mc_q, "forw_back_mc_q"); 
					size += stream.ReadUnsignedInt(size, 1, out this.halfpel2, "halfpel2"); 
					size += stream.ReadUnsignedInt(size, 1, out this.halfpel4, "halfpel4"); 
				}
				size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_2, "marker_bit_2"); 

				if (ituContext.ComplexityEstimation.EstimationMethod == 1)
				{
					size += stream.ReadUnsignedInt(size, 1, out this.version2_complexity_estimation_disable, "version2_complexity_estimation_disable"); 

					if (version2_complexity_estimation_disable== 0)
					{
						size += stream.ReadUnsignedInt(size, 1, out this.sadct, "sadct"); 
						size += stream.ReadUnsignedInt(size, 1, out this.quarterpel, "quarterpel"); 
					}
				}
			}

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            MPEG4Context ituContext = context as MPEG4Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type MPEG4Context");
            ulong size = 0;

			size += stream.WriteUnsignedInt(2, this.estimation_method, "estimation_method"); 
			ituContext.ComplexityEstimation = this;

			if (ituContext.ComplexityEstimation.EstimationMethod == 0 || ituContext.ComplexityEstimation.EstimationMethod == 1)
			{
				size += stream.WriteUnsignedInt(1, this.shape_complexity_estimation_disable, "shape_complexity_estimation_disable"); 

				if (shape_complexity_estimation_disable== 0)
				{
					size += stream.WriteUnsignedInt(1, this.opaque, "opaque"); 
					size += stream.WriteUnsignedInt(1, this.transparent, "transparent"); 
					size += stream.WriteUnsignedInt(1, this.intra_cae, "intra_cae"); 
					size += stream.WriteUnsignedInt(1, this.inter_cae, "inter_cae"); 
					size += stream.WriteUnsignedInt(1, this.no_update, "no_update"); 
					size += stream.WriteUnsignedInt(1, this.upsampling, "upsampling"); 
				}
				size += stream.WriteUnsignedInt(1, this.texture_complexity_estimation_set_1_disable, "texture_complexity_estimation_set_1_disable"); 

				if (texture_complexity_estimation_set_1_disable== 0)
				{
					size += stream.WriteUnsignedInt(1, this.intra_blocks, "intra_blocks"); 
					size += stream.WriteUnsignedInt(1, this.inter_blocks, "inter_blocks"); 
					size += stream.WriteUnsignedInt(1, this.inter4v_blocks, "inter4v_blocks"); 
					size += stream.WriteUnsignedInt(1, this.not_coded_blocks, "not_coded_blocks"); 
				}
				size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
				size += stream.WriteUnsignedInt(1, this.texture_complexity_estimation_set_2_disable, "texture_complexity_estimation_set_2_disable"); 

				if (texture_complexity_estimation_set_2_disable== 0)
				{
					size += stream.WriteUnsignedInt(1, this.dct_coefs, "dct_coefs"); 
					size += stream.WriteUnsignedInt(1, this.dct_lines, "dct_lines"); 
					size += stream.WriteUnsignedInt(1, this.vlc_symbols, "vlc_symbols"); 
					size += stream.WriteUnsignedInt(1, this.vlc_bits, "vlc_bits"); 
				}
				size += stream.WriteUnsignedInt(1, this.motion_compensation_complexity_disable, "motion_compensation_complexity_disable"); 

				if (motion_compensation_complexity_disable== 0)
				{
					size += stream.WriteUnsignedInt(1, this.apm, "apm"); 
					size += stream.WriteUnsignedInt(1, this.npm, "npm"); 
					size += stream.WriteUnsignedInt(1, this.interpolate_mc_q, "interpolate_mc_q"); 
					size += stream.WriteUnsignedInt(1, this.forw_back_mc_q, "forw_back_mc_q"); 
					size += stream.WriteUnsignedInt(1, this.halfpel2, "halfpel2"); 
					size += stream.WriteUnsignedInt(1, this.halfpel4, "halfpel4"); 
				}
				size += stream.WriteUnsignedInt(1, this.marker_bit_2, "marker_bit_2"); 

				if (ituContext.ComplexityEstimation.EstimationMethod == 1)
				{
					size += stream.WriteUnsignedInt(1, this.version2_complexity_estimation_disable, "version2_complexity_estimation_disable"); 

					if (version2_complexity_estimation_disable== 0)
					{
						size += stream.WriteUnsignedInt(1, this.sadct, "sadct"); 
						size += stream.WriteUnsignedInt(1, this.quarterpel, "quarterpel"); 
					}
				}
			}

            return size;
         }

    }

    /*


Group_of_VideoObjectPlane() {
group_of_vop_start_code u(32)
time_code u(18)
closed_gov u(1)
broken_link u(1)
next_start_code()
}
    */
    public class GroupOfVideoObjectPlane : IItuSerializable
    {
		private uint group_of_vop_start_code;
		public uint GroupOfVopStartCode { get { return group_of_vop_start_code; } set { group_of_vop_start_code = value; } }
		private uint time_code;
		public uint TimeCode { get { return time_code; } set { time_code = value; } }
		private byte closed_gov;
		public byte ClosedGov { get { return closed_gov; } set { closed_gov = value; } }
		private byte broken_link;
		public byte BrokenLink { get { return broken_link; } set { broken_link = value; } }
		private NextStartCode next_start_code;
		public NextStartCode NextStartCode { get { return next_start_code; } set { next_start_code = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public GroupOfVideoObjectPlane()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            MPEG4Context ituContext = context as MPEG4Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type MPEG4Context");

            ulong size = 0;

			size += stream.ReadUnsignedInt(size, 32, out this.group_of_vop_start_code, "group_of_vop_start_code"); 
			size += stream.ReadUnsignedInt(size, 18, out this.time_code, "time_code"); 
			size += stream.ReadUnsignedInt(size, 1, out this.closed_gov, "closed_gov"); 
			size += stream.ReadUnsignedInt(size, 1, out this.broken_link, "broken_link"); 
			this.next_start_code =  new NextStartCode() ;
			size +=  stream.ReadClass<NextStartCode>(size, context, this.next_start_code, "next_start_code"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            MPEG4Context ituContext = context as MPEG4Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type MPEG4Context");
            ulong size = 0;

			size += stream.WriteUnsignedInt(32, this.group_of_vop_start_code, "group_of_vop_start_code"); 
			size += stream.WriteUnsignedInt(18, this.time_code, "time_code"); 
			size += stream.WriteUnsignedInt(1, this.closed_gov, "closed_gov"); 
			size += stream.WriteUnsignedInt(1, this.broken_link, "broken_link"); 
			size += stream.WriteClass<NextStartCode>(context, this.next_start_code, "next_start_code"); 

            return size;
         }

    }

    /*


VideoObjectPlane() {
vop_start_code u(32)
vop_coding_type u(2)
do {
modulo_time_base u(1)
} while (more_modulo_time_base)
marker_bit u(1)
vop_time_increment u(v)
marker_bit_2 u(1)
vop_coded u(1)
if (vop_coded == 0) {
next_start_code()
}
else {
if (newpred_enable) {
vop_id u(v)
vop_id_for_prediction_indication u(1)
if (vop_id_for_prediction_indication)
vop_id_for_prediction u(v)
marker_bit_3 u(1)
}
if ((video_object_layer_shape != 2) &&
(vop_coding_type == 1 ||
(vop_coding_type == 3 && sprite_enable == 2)))
vop_rounding_type u(1)
if ((reduced_resolution_vop_enable) &&
(video_object_layer_shape == 0) &&
((vop_coding_type == 1) || (vop_coding_type == 0)))
vop_reduced_resolution u(1)
if (video_object_layer_shape != 0) {
if(!(sprite_enable == 1 && vop_coding_type == 0)) {
vop_width u(13)
marker_bit u(1)
vop_height u(13)
marker_bit_2 u(1)
vop_horizontal_mc_spatial_ref i(13)
marker_bit_3 u(1)
vop_vertical_mc_spatial_ref i(13)
marker_bit_4 u(1)
}
if ((video_object_layer_shape != 2) &&
scalability && enhancement_type)
background_composition u(1)
change_conv_ratio_disable u(1)
vop_constant_alpha u(1)
if (vop_constant_alpha)
vop_constant_alpha_value u(8)
}
if (video_object_layer_shape != 2)
if (!complexity_estimation_disable)
read_vop_complexity_estimation_header()
if (video_object_layer_shape != 2) {
intra_dc_vlc_thr u(3)
if (interlaced) {
top_field_first u(1)
alternate_vertical_scan_flag u(1)
}
}
if ((sprite_enable == 1 || sprite_enable == 2) &&
vop_coding_type == 3) {
if (no_of_sprite_warping_points > 0)
sprite_trajectory()
if (sprite_brightness_change)
brightness_change_factor()
if(sprite_enable == 1) {
static_sprite_vop()
}
}
if (video_object_layer_shape != 2) {
vop_quant u(v)
if(video_object_layer_shape == 3)
for(i=0; i<aux_comp_count; i++)
vop_alpha_quant[i] u(6)
if (vop_coding_type != 0)
vop_fcode_forward u(3)
if (vop_coding_type == 2)
vop_fcode_backward u(3)
if (!scalability) {
if (video_object_layer_shape != 0
&& vop_coding_type != 0)
vop_shape_coding_type u(1)
vop_data()
}
else {
if (enhancement_type) {
load_backward_shape u(1)
if (load_backward_shape) {
backward_shape_width u(13)
marker_bit u(1)
backward_shape_height u(13)
marker_bit_2 u(1)
backward_shape_horizontal_mc_spatial_ref i(13)
marker_bit_3 u(1)
backward_shape_vertical_mc_spatial_ref i(13)
backward_shape()
load_forward_shape u(1)
if (load_forward_shape) {
forward_shape_width u(13)
marker_bit_4 u(1)
forward_shape_height u(13)
marker_bit_5 u(1)
forward_shape_horizontal_mc_spatial_ref i(13)
marker_bit_6 u(1)
forward_shape_vertical_mc_spatial_ref i(13)
forward_shape()
}
}
}
ref_select_code u(2)
vop_data()
}
}
else {
vop_data()
}
}
}
    */
    public class VideoObjectPlane : IItuSerializable
    {
		private uint vop_start_code;
		public uint VopStartCode { get { return vop_start_code; } set { vop_start_code = value; } }
		private uint vop_coding_type;
		public uint VopCodingType { get { return vop_coding_type; } set { vop_coding_type = value; } }
		private Dictionary<int, byte> modulo_time_base;
		public Dictionary<int, byte> ModuloTimeBase { get { return modulo_time_base ??= new Dictionary<int, byte>(); } set { modulo_time_base = value; } }
		private byte marker_bit;
		public byte MarkerBit { get { return marker_bit; } set { marker_bit = value; } }
		private ulong vop_time_increment;
		public ulong VopTimeIncrement { get { return vop_time_increment; } set { vop_time_increment = value; } }
		private byte marker_bit_2;
		public byte MarkerBit2 { get { return marker_bit_2; } set { marker_bit_2 = value; } }
		private byte vop_coded;
		public byte VopCoded { get { return vop_coded; } set { vop_coded = value; } }
		private NextStartCode next_start_code;
		public NextStartCode NextStartCode { get { return next_start_code; } set { next_start_code = value; } }
		private ulong vop_id;
		public ulong VopId { get { return vop_id; } set { vop_id = value; } }
		private byte vop_id_for_prediction_indication;
		public byte VopIdForPredictionIndication { get { return vop_id_for_prediction_indication; } set { vop_id_for_prediction_indication = value; } }
		private ulong vop_id_for_prediction;
		public ulong VopIdForPrediction { get { return vop_id_for_prediction; } set { vop_id_for_prediction = value; } }
		private byte marker_bit_3;
		public byte MarkerBit3 { get { return marker_bit_3; } set { marker_bit_3 = value; } }
		private byte vop_rounding_type;
		public byte VopRoundingType { get { return vop_rounding_type; } set { vop_rounding_type = value; } }
		private byte vop_reduced_resolution;
		public byte VopReducedResolution { get { return vop_reduced_resolution; } set { vop_reduced_resolution = value; } }
		private uint vop_width;
		public uint VopWidth { get { return vop_width; } set { vop_width = value; } }
		private uint vop_height;
		public uint VopHeight { get { return vop_height; } set { vop_height = value; } }
		private int vop_horizontal_mc_spatial_ref;
		public int VopHorizontalMcSpatialRef { get { return vop_horizontal_mc_spatial_ref; } set { vop_horizontal_mc_spatial_ref = value; } }
		private int vop_vertical_mc_spatial_ref;
		public int VopVerticalMcSpatialRef { get { return vop_vertical_mc_spatial_ref; } set { vop_vertical_mc_spatial_ref = value; } }
		private byte marker_bit_4;
		public byte MarkerBit4 { get { return marker_bit_4; } set { marker_bit_4 = value; } }
		private byte background_composition;
		public byte BackgroundComposition { get { return background_composition; } set { background_composition = value; } }
		private byte change_conv_ratio_disable;
		public byte ChangeConvRatioDisable { get { return change_conv_ratio_disable; } set { change_conv_ratio_disable = value; } }
		private byte vop_constant_alpha;
		public byte VopConstantAlpha { get { return vop_constant_alpha; } set { vop_constant_alpha = value; } }
		private uint vop_constant_alpha_value;
		public uint VopConstantAlphaValue { get { return vop_constant_alpha_value; } set { vop_constant_alpha_value = value; } }
		private ReadVopComplexityEstimationHeader read_vop_complexity_estimation_header;
		public ReadVopComplexityEstimationHeader ReadVopComplexityEstimationHeader { get { return read_vop_complexity_estimation_header; } set { read_vop_complexity_estimation_header = value; } }
		private uint intra_dc_vlc_thr;
		public uint IntraDcVlcThr { get { return intra_dc_vlc_thr; } set { intra_dc_vlc_thr = value; } }
		private byte top_field_first;
		public byte TopFieldFirst { get { return top_field_first; } set { top_field_first = value; } }
		private byte alternate_vertical_scan_flag;
		public byte AlternateVerticalScanFlag { get { return alternate_vertical_scan_flag; } set { alternate_vertical_scan_flag = value; } }
		private SpriteTrajectory sprite_trajectory;
		public SpriteTrajectory SpriteTrajectory { get { return sprite_trajectory; } set { sprite_trajectory = value; } }
		private BrightnessChangeFactor brightness_change_factor;
		public BrightnessChangeFactor BrightnessChangeFactor { get { return brightness_change_factor; } set { brightness_change_factor = value; } }
		private StaticSpriteVop static_sprite_vop;
		public StaticSpriteVop StaticSpriteVop { get { return static_sprite_vop; } set { static_sprite_vop = value; } }
		private ulong vop_quant;
		public ulong VopQuant { get { return vop_quant; } set { vop_quant = value; } }
		private uint[] vop_alpha_quant;
		public uint[] VopAlphaQuant { get { return vop_alpha_quant; } set { vop_alpha_quant = value; } }
		private uint vop_fcode_forward;
		public uint VopFcodeForward { get { return vop_fcode_forward; } set { vop_fcode_forward = value; } }
		private uint vop_fcode_backward;
		public uint VopFcodeBackward { get { return vop_fcode_backward; } set { vop_fcode_backward = value; } }
		private byte vop_shape_coding_type;
		public byte VopShapeCodingType { get { return vop_shape_coding_type; } set { vop_shape_coding_type = value; } }
		private VopData vop_data;
		public VopData VopData { get { return vop_data; } set { vop_data = value; } }
		private byte load_backward_shape;
		public byte LoadBackwardShape { get { return load_backward_shape; } set { load_backward_shape = value; } }
		private uint backward_shape_width;
		public uint BackwardShapeWidth { get { return backward_shape_width; } set { backward_shape_width = value; } }
		private uint backward_shape_height;
		public uint BackwardShapeHeight { get { return backward_shape_height; } set { backward_shape_height = value; } }
		private int backward_shape_horizontal_mc_spatial_ref;
		public int BackwardShapeHorizontalMcSpatialRef { get { return backward_shape_horizontal_mc_spatial_ref; } set { backward_shape_horizontal_mc_spatial_ref = value; } }
		private int backward_shape_vertical_mc_spatial_ref;
		public int BackwardShapeVerticalMcSpatialRef { get { return backward_shape_vertical_mc_spatial_ref; } set { backward_shape_vertical_mc_spatial_ref = value; } }
		private BackwardShape backward_shape;
		public BackwardShape BackwardShape { get { return backward_shape; } set { backward_shape = value; } }
		private byte load_forward_shape;
		public byte LoadForwardShape { get { return load_forward_shape; } set { load_forward_shape = value; } }
		private uint forward_shape_width;
		public uint ForwardShapeWidth { get { return forward_shape_width; } set { forward_shape_width = value; } }
		private uint forward_shape_height;
		public uint ForwardShapeHeight { get { return forward_shape_height; } set { forward_shape_height = value; } }
		private byte marker_bit_5;
		public byte MarkerBit5 { get { return marker_bit_5; } set { marker_bit_5 = value; } }
		private int forward_shape_horizontal_mc_spatial_ref;
		public int ForwardShapeHorizontalMcSpatialRef { get { return forward_shape_horizontal_mc_spatial_ref; } set { forward_shape_horizontal_mc_spatial_ref = value; } }
		private byte marker_bit_6;
		public byte MarkerBit6 { get { return marker_bit_6; } set { marker_bit_6 = value; } }
		private int forward_shape_vertical_mc_spatial_ref;
		public int ForwardShapeVerticalMcSpatialRef { get { return forward_shape_vertical_mc_spatial_ref; } set { forward_shape_vertical_mc_spatial_ref = value; } }
		private ForwardShape forward_shape;
		public ForwardShape ForwardShape { get { return forward_shape; } set { forward_shape = value; } }
		private uint ref_select_code;
		public uint RefSelectCode { get { return ref_select_code; } set { ref_select_code = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public VideoObjectPlane()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            MPEG4Context ituContext = context as MPEG4Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type MPEG4Context");

            ulong size = 0;

			int whileIndex = -1;
			uint i = 0;
			size += stream.ReadUnsignedInt(size, 32, out this.vop_start_code, "vop_start_code"); 
			size += stream.ReadUnsignedInt(size, 2, out this.vop_coding_type, "vop_coding_type"); 
			ituContext.VopCodingType = vop_coding_type;

			do
			{
				whileIndex++;

				size += stream.ReadUnsignedInt(size, 1, whileIndex, (this.modulo_time_base ??= new()), "modulo_time_base"); 
			} while ((this.modulo_time_base[whileIndex] == 1 ? 1 : 0) != 0);
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedIntVariable(size, ituContext.VopTimeIncrementBits, out this.vop_time_increment, "vop_time_increment"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_2, "marker_bit_2"); 
			size += stream.ReadUnsignedInt(size, 1, out this.vop_coded, "vop_coded"); 

			if (vop_coded == 0)
			{
				this.next_start_code =  new NextStartCode() ;
				size +=  stream.ReadClass<NextStartCode>(size, context, this.next_start_code, "next_start_code"); 
			}
			else 
			{

				if (ituContext.VideoObjectLayer.NewpredEnable != 0)
				{
					size += stream.ReadUnsignedIntVariable(size, ituContext.VopIdBits, out this.vop_id, "vop_id"); 
					size += stream.ReadUnsignedInt(size, 1, out this.vop_id_for_prediction_indication, "vop_id_for_prediction_indication"); 

					if (vop_id_for_prediction_indication != 0)
					{
						size += stream.ReadUnsignedIntVariable(size, ituContext.VopIdBits, out this.vop_id_for_prediction, "vop_id_for_prediction"); 
					}
					size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_3, "marker_bit_3"); 
				}

				if ((ituContext.VideoObjectLayer.VideoObjectLayerShape != 2) &&
(ituContext.VopCodingType == 1 ||
(ituContext.VopCodingType == 3 && ituContext.VideoObjectLayer.SpriteEnable == 2)))
				{
					size += stream.ReadUnsignedInt(size, 1, out this.vop_rounding_type, "vop_rounding_type"); 
				}

				if ((ituContext.VideoObjectLayer.ReducedResolutionVopEnable != 0) &&
(ituContext.VideoObjectLayer.VideoObjectLayerShape == 0) &&
((ituContext.VopCodingType == 1) || (ituContext.VopCodingType == 0)))
				{
					size += stream.ReadUnsignedInt(size, 1, out this.vop_reduced_resolution, "vop_reduced_resolution"); 
				}

				if (ituContext.VideoObjectLayer.VideoObjectLayerShape != 0)
				{

					if (!(ituContext.VideoObjectLayer.SpriteEnable == 1 && ituContext.VopCodingType == 0))
					{
						size += stream.ReadUnsignedInt(size, 13, out this.vop_width, "vop_width"); 
						size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
						size += stream.ReadUnsignedInt(size, 13, out this.vop_height, "vop_height"); 
						size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_2, "marker_bit_2"); 
						size += stream.ReadSignedInt(size, 13, out this.vop_horizontal_mc_spatial_ref, "vop_horizontal_mc_spatial_ref"); 
						size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_3, "marker_bit_3"); 
						size += stream.ReadSignedInt(size, 13, out this.vop_vertical_mc_spatial_ref, "vop_vertical_mc_spatial_ref"); 
						size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_4, "marker_bit_4"); 
					}

					if ((ituContext.VideoObjectLayer.VideoObjectLayerShape != 2) &&
ituContext.VideoObjectLayer.Scalability != 0 && ituContext.VideoObjectLayer.EnhancementType != 0)
					{
						size += stream.ReadUnsignedInt(size, 1, out this.background_composition, "background_composition"); 
					}
					size += stream.ReadUnsignedInt(size, 1, out this.change_conv_ratio_disable, "change_conv_ratio_disable"); 
					size += stream.ReadUnsignedInt(size, 1, out this.vop_constant_alpha, "vop_constant_alpha"); 

					if (vop_constant_alpha != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.vop_constant_alpha_value, "vop_constant_alpha_value"); 
					}
				}

				if (ituContext.VideoObjectLayer.VideoObjectLayerShape != 2)
				{

					if (ituContext.VideoObjectLayer.ComplexityEstimationDisable== 0)
					{
						this.read_vop_complexity_estimation_header =  new ReadVopComplexityEstimationHeader() ;
						size +=  stream.ReadClass<ReadVopComplexityEstimationHeader>(size, context, this.read_vop_complexity_estimation_header, "read_vop_complexity_estimation_header"); 
					}
				}

				if (ituContext.VideoObjectLayer.VideoObjectLayerShape != 2)
				{
					size += stream.ReadUnsignedInt(size, 3, out this.intra_dc_vlc_thr, "intra_dc_vlc_thr"); 

					if (ituContext.VideoObjectLayer.Interlaced != 0)
					{
						size += stream.ReadUnsignedInt(size, 1, out this.top_field_first, "top_field_first"); 
						size += stream.ReadUnsignedInt(size, 1, out this.alternate_vertical_scan_flag, "alternate_vertical_scan_flag"); 
					}
				}

				if ((ituContext.VideoObjectLayer.SpriteEnable == 1 || ituContext.VideoObjectLayer.SpriteEnable == 2) &&
ituContext.VopCodingType == 3)
				{

					if (ituContext.VideoObjectLayer.NoOfSpriteWarpingPoints > 0)
					{
						this.sprite_trajectory =  new SpriteTrajectory() ;
						size +=  stream.ReadClass<SpriteTrajectory>(size, context, this.sprite_trajectory, "sprite_trajectory"); 
					}

					if (ituContext.VideoObjectLayer.SpriteBrightnessChange != 0)
					{
						this.brightness_change_factor =  new BrightnessChangeFactor() ;
						size +=  stream.ReadClass<BrightnessChangeFactor>(size, context, this.brightness_change_factor, "brightness_change_factor"); 
					}

					if (ituContext.VideoObjectLayer.SpriteEnable == 1)
					{
						this.static_sprite_vop =  new StaticSpriteVop() ;
						size +=  stream.ReadClass<StaticSpriteVop>(size, context, this.static_sprite_vop, "static_sprite_vop"); 
					}
				}

				if (ituContext.VideoObjectLayer.VideoObjectLayerShape != 2)
				{
					size += stream.ReadUnsignedIntVariable(size, ituContext.QuantBits, out this.vop_quant, "vop_quant"); 

					if (ituContext.VideoObjectLayer.VideoObjectLayerShape == 3)
					{

						stream.CheckArrayAllocation((ulong)(ituContext.AuxCompCount), "vop_alpha_quant");
						this.vop_alpha_quant = new uint[ituContext.AuxCompCount];
						for (i=0; i<ituContext.AuxCompCount; i++)
						{
							size += stream.ReadUnsignedInt(size, 6, out this.vop_alpha_quant[i], "vop_alpha_quant"); 
						}
					}

					if (ituContext.VopCodingType != 0)
					{
						size += stream.ReadUnsignedInt(size, 3, out this.vop_fcode_forward, "vop_fcode_forward"); 
					}

					if (ituContext.VopCodingType == 2)
					{
						size += stream.ReadUnsignedInt(size, 3, out this.vop_fcode_backward, "vop_fcode_backward"); 
					}

					if (ituContext.VideoObjectLayer.Scalability== 0)
					{

						if (ituContext.VideoObjectLayer.VideoObjectLayerShape != 0
&& ituContext.VopCodingType != 0)
						{
							size += stream.ReadUnsignedInt(size, 1, out this.vop_shape_coding_type, "vop_shape_coding_type"); 
						}
						this.vop_data =  new VopData() ;
						size +=  stream.ReadClass<VopData>(size, context, this.vop_data, "vop_data"); 
					}
					else 
					{

						if (ituContext.VideoObjectLayer.EnhancementType != 0)
						{
							size += stream.ReadUnsignedInt(size, 1, out this.load_backward_shape, "load_backward_shape"); 

							if (load_backward_shape != 0)
							{
								size += stream.ReadUnsignedInt(size, 13, out this.backward_shape_width, "backward_shape_width"); 
								size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
								size += stream.ReadUnsignedInt(size, 13, out this.backward_shape_height, "backward_shape_height"); 
								size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_2, "marker_bit_2"); 
								size += stream.ReadSignedInt(size, 13, out this.backward_shape_horizontal_mc_spatial_ref, "backward_shape_horizontal_mc_spatial_ref"); 
								size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_3, "marker_bit_3"); 
								size += stream.ReadSignedInt(size, 13, out this.backward_shape_vertical_mc_spatial_ref, "backward_shape_vertical_mc_spatial_ref"); 
								this.backward_shape =  new BackwardShape() ;
								size +=  stream.ReadClass<BackwardShape>(size, context, this.backward_shape, "backward_shape"); 
								size += stream.ReadUnsignedInt(size, 1, out this.load_forward_shape, "load_forward_shape"); 

								if (load_forward_shape != 0)
								{
									size += stream.ReadUnsignedInt(size, 13, out this.forward_shape_width, "forward_shape_width"); 
									size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_4, "marker_bit_4"); 
									size += stream.ReadUnsignedInt(size, 13, out this.forward_shape_height, "forward_shape_height"); 
									size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_5, "marker_bit_5"); 
									size += stream.ReadSignedInt(size, 13, out this.forward_shape_horizontal_mc_spatial_ref, "forward_shape_horizontal_mc_spatial_ref"); 
									size += stream.ReadUnsignedInt(size, 1, out this.marker_bit_6, "marker_bit_6"); 
									size += stream.ReadSignedInt(size, 13, out this.forward_shape_vertical_mc_spatial_ref, "forward_shape_vertical_mc_spatial_ref"); 
									this.forward_shape =  new ForwardShape() ;
									size +=  stream.ReadClass<ForwardShape>(size, context, this.forward_shape, "forward_shape"); 
								}
							}
						}
						size += stream.ReadUnsignedInt(size, 2, out this.ref_select_code, "ref_select_code"); 
						this.vop_data =  new VopData() ;
						size +=  stream.ReadClass<VopData>(size, context, this.vop_data, "vop_data"); 
					}
				}
				else 
				{
					this.vop_data =  new VopData() ;
					size +=  stream.ReadClass<VopData>(size, context, this.vop_data, "vop_data"); 
				}
			}

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            MPEG4Context ituContext = context as MPEG4Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type MPEG4Context");
            ulong size = 0;

			int whileIndex = -1;
			uint i = 0;
			size += stream.WriteUnsignedInt(32, this.vop_start_code, "vop_start_code"); 
			size += stream.WriteUnsignedInt(2, this.vop_coding_type, "vop_coding_type"); 
			ituContext.VopCodingType = vop_coding_type;

			do
			{
				whileIndex++;

				size += stream.WriteUnsignedInt(1, whileIndex, (this.modulo_time_base ??= new()), "modulo_time_base"); 
			} while ((whileIndex + 1 < this.ModuloTimeBase.Count ? 1 : 0) != 0);
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedIntVariable(ituContext.VopTimeIncrementBits, this.vop_time_increment, "vop_time_increment"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit_2, "marker_bit_2"); 
			size += stream.WriteUnsignedInt(1, this.vop_coded, "vop_coded"); 

			if (vop_coded == 0)
			{
				size += stream.WriteClass<NextStartCode>(context, this.next_start_code, "next_start_code"); 
			}
			else 
			{

				if (ituContext.VideoObjectLayer.NewpredEnable != 0)
				{
					size += stream.WriteUnsignedIntVariable(ituContext.VopIdBits, this.vop_id, "vop_id"); 
					size += stream.WriteUnsignedInt(1, this.vop_id_for_prediction_indication, "vop_id_for_prediction_indication"); 

					if (vop_id_for_prediction_indication != 0)
					{
						size += stream.WriteUnsignedIntVariable(ituContext.VopIdBits, this.vop_id_for_prediction, "vop_id_for_prediction"); 
					}
					size += stream.WriteUnsignedInt(1, this.marker_bit_3, "marker_bit_3"); 
				}

				if ((ituContext.VideoObjectLayer.VideoObjectLayerShape != 2) &&
(ituContext.VopCodingType == 1 ||
(ituContext.VopCodingType == 3 && ituContext.VideoObjectLayer.SpriteEnable == 2)))
				{
					size += stream.WriteUnsignedInt(1, this.vop_rounding_type, "vop_rounding_type"); 
				}

				if ((ituContext.VideoObjectLayer.ReducedResolutionVopEnable != 0) &&
(ituContext.VideoObjectLayer.VideoObjectLayerShape == 0) &&
((ituContext.VopCodingType == 1) || (ituContext.VopCodingType == 0)))
				{
					size += stream.WriteUnsignedInt(1, this.vop_reduced_resolution, "vop_reduced_resolution"); 
				}

				if (ituContext.VideoObjectLayer.VideoObjectLayerShape != 0)
				{

					if (!(ituContext.VideoObjectLayer.SpriteEnable == 1 && ituContext.VopCodingType == 0))
					{
						size += stream.WriteUnsignedInt(13, this.vop_width, "vop_width"); 
						size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
						size += stream.WriteUnsignedInt(13, this.vop_height, "vop_height"); 
						size += stream.WriteUnsignedInt(1, this.marker_bit_2, "marker_bit_2"); 
						size += stream.WriteSignedInt(13,  this.vop_horizontal_mc_spatial_ref, "vop_horizontal_mc_spatial_ref"); 
						size += stream.WriteUnsignedInt(1, this.marker_bit_3, "marker_bit_3"); 
						size += stream.WriteSignedInt(13,  this.vop_vertical_mc_spatial_ref, "vop_vertical_mc_spatial_ref"); 
						size += stream.WriteUnsignedInt(1, this.marker_bit_4, "marker_bit_4"); 
					}

					if ((ituContext.VideoObjectLayer.VideoObjectLayerShape != 2) &&
ituContext.VideoObjectLayer.Scalability != 0 && ituContext.VideoObjectLayer.EnhancementType != 0)
					{
						size += stream.WriteUnsignedInt(1, this.background_composition, "background_composition"); 
					}
					size += stream.WriteUnsignedInt(1, this.change_conv_ratio_disable, "change_conv_ratio_disable"); 
					size += stream.WriteUnsignedInt(1, this.vop_constant_alpha, "vop_constant_alpha"); 

					if (vop_constant_alpha != 0)
					{
						size += stream.WriteUnsignedInt(8, this.vop_constant_alpha_value, "vop_constant_alpha_value"); 
					}
				}

				if (ituContext.VideoObjectLayer.VideoObjectLayerShape != 2)
				{

					if (ituContext.VideoObjectLayer.ComplexityEstimationDisable== 0)
					{
						size += stream.WriteClass<ReadVopComplexityEstimationHeader>(context, this.read_vop_complexity_estimation_header, "read_vop_complexity_estimation_header"); 
					}
				}

				if (ituContext.VideoObjectLayer.VideoObjectLayerShape != 2)
				{
					size += stream.WriteUnsignedInt(3, this.intra_dc_vlc_thr, "intra_dc_vlc_thr"); 

					if (ituContext.VideoObjectLayer.Interlaced != 0)
					{
						size += stream.WriteUnsignedInt(1, this.top_field_first, "top_field_first"); 
						size += stream.WriteUnsignedInt(1, this.alternate_vertical_scan_flag, "alternate_vertical_scan_flag"); 
					}
				}

				if ((ituContext.VideoObjectLayer.SpriteEnable == 1 || ituContext.VideoObjectLayer.SpriteEnable == 2) &&
ituContext.VopCodingType == 3)
				{

					if (ituContext.VideoObjectLayer.NoOfSpriteWarpingPoints > 0)
					{
						size += stream.WriteClass<SpriteTrajectory>(context, this.sprite_trajectory, "sprite_trajectory"); 
					}

					if (ituContext.VideoObjectLayer.SpriteBrightnessChange != 0)
					{
						size += stream.WriteClass<BrightnessChangeFactor>(context, this.brightness_change_factor, "brightness_change_factor"); 
					}

					if (ituContext.VideoObjectLayer.SpriteEnable == 1)
					{
						size += stream.WriteClass<StaticSpriteVop>(context, this.static_sprite_vop, "static_sprite_vop"); 
					}
				}

				if (ituContext.VideoObjectLayer.VideoObjectLayerShape != 2)
				{
					size += stream.WriteUnsignedIntVariable(ituContext.QuantBits, this.vop_quant, "vop_quant"); 

					if (ituContext.VideoObjectLayer.VideoObjectLayerShape == 3)
					{

						for (i=0; i<ituContext.AuxCompCount; i++)
						{
							size += stream.WriteUnsignedInt(6, this.vop_alpha_quant[i], "vop_alpha_quant"); 
						}
					}

					if (ituContext.VopCodingType != 0)
					{
						size += stream.WriteUnsignedInt(3, this.vop_fcode_forward, "vop_fcode_forward"); 
					}

					if (ituContext.VopCodingType == 2)
					{
						size += stream.WriteUnsignedInt(3, this.vop_fcode_backward, "vop_fcode_backward"); 
					}

					if (ituContext.VideoObjectLayer.Scalability== 0)
					{

						if (ituContext.VideoObjectLayer.VideoObjectLayerShape != 0
&& ituContext.VopCodingType != 0)
						{
							size += stream.WriteUnsignedInt(1, this.vop_shape_coding_type, "vop_shape_coding_type"); 
						}
						size += stream.WriteClass<VopData>(context, this.vop_data, "vop_data"); 
					}
					else 
					{

						if (ituContext.VideoObjectLayer.EnhancementType != 0)
						{
							size += stream.WriteUnsignedInt(1, this.load_backward_shape, "load_backward_shape"); 

							if (load_backward_shape != 0)
							{
								size += stream.WriteUnsignedInt(13, this.backward_shape_width, "backward_shape_width"); 
								size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
								size += stream.WriteUnsignedInt(13, this.backward_shape_height, "backward_shape_height"); 
								size += stream.WriteUnsignedInt(1, this.marker_bit_2, "marker_bit_2"); 
								size += stream.WriteSignedInt(13,  this.backward_shape_horizontal_mc_spatial_ref, "backward_shape_horizontal_mc_spatial_ref"); 
								size += stream.WriteUnsignedInt(1, this.marker_bit_3, "marker_bit_3"); 
								size += stream.WriteSignedInt(13,  this.backward_shape_vertical_mc_spatial_ref, "backward_shape_vertical_mc_spatial_ref"); 
								size += stream.WriteClass<BackwardShape>(context, this.backward_shape, "backward_shape"); 
								size += stream.WriteUnsignedInt(1, this.load_forward_shape, "load_forward_shape"); 

								if (load_forward_shape != 0)
								{
									size += stream.WriteUnsignedInt(13, this.forward_shape_width, "forward_shape_width"); 
									size += stream.WriteUnsignedInt(1, this.marker_bit_4, "marker_bit_4"); 
									size += stream.WriteUnsignedInt(13, this.forward_shape_height, "forward_shape_height"); 
									size += stream.WriteUnsignedInt(1, this.marker_bit_5, "marker_bit_5"); 
									size += stream.WriteSignedInt(13,  this.forward_shape_horizontal_mc_spatial_ref, "forward_shape_horizontal_mc_spatial_ref"); 
									size += stream.WriteUnsignedInt(1, this.marker_bit_6, "marker_bit_6"); 
									size += stream.WriteSignedInt(13,  this.forward_shape_vertical_mc_spatial_ref, "forward_shape_vertical_mc_spatial_ref"); 
									size += stream.WriteClass<ForwardShape>(context, this.forward_shape, "forward_shape"); 
								}
							}
						}
						size += stream.WriteUnsignedInt(2, this.ref_select_code, "ref_select_code"); 
						size += stream.WriteClass<VopData>(context, this.vop_data, "vop_data"); 
					}
				}
				else 
				{
					size += stream.WriteClass<VopData>(context, this.vop_data, "vop_data"); 
				}
			}

            return size;
         }

    }

    /*


read_vop_complexity_estimation_header() {
if (estimation_method == 0 || estimation_method == 1) {
if (vop_coding_type == 0) {
if (opaque) dcecs_opaque u(8)
if (transparent) dcecs_transparent u(8)
if (intra_cae) dcecs_intra_cae u(8)
if (inter_cae) dcecs_inter_cae u(8)
if (no_update) dcecs_no_update u(8)
if (upsampling) dcecs_upsampling u(8)
if (intra_blocks) dcecs_intra_blocks u(8)
if (not_coded_blocks) dcecs_not_coded_blocks u(8)
if (dct_coefs) dcecs_dct_coefs u(8)
if (dct_lines) dcecs_dct_lines u(8)
if (vlc_symbols) dcecs_vlc_symbols u(8)
if (vlc_bits) dcecs_vlc_bits u(4)
if (sadct) dcecs_sadct u(8)
}
if (vop_coding_type == 1) {
if (opaque) dcecs_opaque u(8)
if (transparent) dcecs_transparent u(8)
if (intra_cae) dcecs_intra_cae u(8)
if (inter_cae) dcecs_inter_cae u(8)
if (no_update) dcecs_no_update u(8)
if (upsampling) dcecs_upsampling u(8)
if (intra_blocks) dcecs_intra_blocks u(8)
if (not_coded) dcecs_not_coded_blocks u(8)
if (dct_coefs) dcecs_dct_coefs u(8)
if (dct_lines) dcecs_dct_lines u(8)
if (vlc_symbols) dcecs_vlc_symbols u(8)
if (vlc_bits) dcecs_vlc_bits u(4)
if (inter_blocks) dcecs_inter_blocks u(8)
if (inter4v_blocks) dcecs_inter4v_blocks u(8)
if (apm) dcecs_apm u(8)
if (npm) dcecs_npm u(8)
if (forw_back_mc_q) dcecs_forw_back_mc_q u(8)
if (halfpel2) dcecs_halfpel2 u(8)
if (halfpel4) dcecs_halfpel4 u(8)
if (sadct) dcecs_sadct u(8)
if (quarterpel) dcecs_quarterpel u(8)
}
if (vop_coding_type == 2) {
if (opaque) dcecs_opaque u(8)
if (transparent) dcecs_transparent u(8)
if (intra_cae) dcecs_intra_cae u(8)
if (inter_cae) dcecs_inter_cae u(8)
if (no_update) dcecs_no_update u(8)
if (upsampling) dcecs_upsampling u(8)
if (intra_blocks) dcecs_intra_blocks u(8)
if (not_coded_blocks) dcecs_not_coded_blocks u(8)
if (dct_coefs) dcecs_dct_coefs u(8)
if (dct_lines) dcecs_dct_lines u(8)
if (vlc_symbols) dcecs_vlc_symbols u(8)
if (vlc_bits) dcecs_vlc_bits u(4)
if (inter_blocks) dcecs_inter_blocks u(8)
if (inter4v_blocks) dcecs_inter4v_blocks u(8)
if (apm) dcecs_apm u(8)
if (npm) dcecs_npm u(8)
if (forw_back_mc_q) dcecs_forw_back_mc_q u(8)
if (halfpel2) dcecs_halfpel2 u(8)
if (halfpel4) dcecs_halfpel4 u(8)
if (interpolate_mc_q) dcecs_interpolate_mc_q u(8)
if (sadct) dcecs_sadct u(8)
if (quarterpel) dcecs_quarterpel u(8)
}
    if (vop_coding_type == 3&& sprite_enable == 1) {
if (intra_blocks) dcecs_intra_blocks u(8)
if (not_coded_blocks) dcecs_not_coded_blocks u(8)
if (dct_coefs) dcecs_dct_coefs u(8)
if (dct_lines) dcecs_dct_lines u(8)
if (vlc_symbols) dcecs_vlc_symbols u(8)
if (vlc_bits) dcecs_vlc_bits u(4)
if (inter_blocks) dcecs_inter_blocks u(8)
if (inter4v_blocks) dcecs_inter4v_blocks u(8)
if (apm) dcecs_apm u(8)
if (npm) dcecs_npm u(8)
if (forw_back_mc_q) dcecs_forw_back_mc_q u(8)
if (halfpel2) dcecs_halfpel2 u(8)
if (halfpel4) dcecs_halfpel4 u(8)
if (interpolate_mc_q) dcecs_interpolate_mc_q u(8)
}
}
}
    */
    public class ReadVopComplexityEstimationHeader : IItuSerializable
    {
		private uint dcecs_opaque;
		public uint DcecsOpaque { get { return dcecs_opaque; } set { dcecs_opaque = value; } }
		private uint dcecs_transparent;
		public uint DcecsTransparent { get { return dcecs_transparent; } set { dcecs_transparent = value; } }
		private uint dcecs_intra_cae;
		public uint DcecsIntraCae { get { return dcecs_intra_cae; } set { dcecs_intra_cae = value; } }
		private uint dcecs_inter_cae;
		public uint DcecsInterCae { get { return dcecs_inter_cae; } set { dcecs_inter_cae = value; } }
		private uint dcecs_no_update;
		public uint DcecsNoUpdate { get { return dcecs_no_update; } set { dcecs_no_update = value; } }
		private uint dcecs_upsampling;
		public uint DcecsUpsampling { get { return dcecs_upsampling; } set { dcecs_upsampling = value; } }
		private uint dcecs_intra_blocks;
		public uint DcecsIntraBlocks { get { return dcecs_intra_blocks; } set { dcecs_intra_blocks = value; } }
		private uint dcecs_not_coded_blocks;
		public uint DcecsNotCodedBlocks { get { return dcecs_not_coded_blocks; } set { dcecs_not_coded_blocks = value; } }
		private uint dcecs_dct_coefs;
		public uint DcecsDctCoefs { get { return dcecs_dct_coefs; } set { dcecs_dct_coefs = value; } }
		private uint dcecs_dct_lines;
		public uint DcecsDctLines { get { return dcecs_dct_lines; } set { dcecs_dct_lines = value; } }
		private uint dcecs_vlc_symbols;
		public uint DcecsVlcSymbols { get { return dcecs_vlc_symbols; } set { dcecs_vlc_symbols = value; } }
		private uint dcecs_vlc_bits;
		public uint DcecsVlcBits { get { return dcecs_vlc_bits; } set { dcecs_vlc_bits = value; } }
		private uint dcecs_sadct;
		public uint DcecsSadct { get { return dcecs_sadct; } set { dcecs_sadct = value; } }
		private uint dcecs_inter_blocks;
		public uint DcecsInterBlocks { get { return dcecs_inter_blocks; } set { dcecs_inter_blocks = value; } }
		private uint dcecs_inter4v_blocks;
		public uint DcecsInter4vBlocks { get { return dcecs_inter4v_blocks; } set { dcecs_inter4v_blocks = value; } }
		private uint dcecs_apm;
		public uint DcecsApm { get { return dcecs_apm; } set { dcecs_apm = value; } }
		private uint dcecs_npm;
		public uint DcecsNpm { get { return dcecs_npm; } set { dcecs_npm = value; } }
		private uint dcecs_forw_back_mc_q;
		public uint DcecsForwBackMcq { get { return dcecs_forw_back_mc_q; } set { dcecs_forw_back_mc_q = value; } }
		private uint dcecs_halfpel2;
		public uint DcecsHalfpel2 { get { return dcecs_halfpel2; } set { dcecs_halfpel2 = value; } }
		private uint dcecs_halfpel4;
		public uint DcecsHalfpel4 { get { return dcecs_halfpel4; } set { dcecs_halfpel4 = value; } }
		private uint dcecs_quarterpel;
		public uint DcecsQuarterpel { get { return dcecs_quarterpel; } set { dcecs_quarterpel = value; } }
		private uint dcecs_interpolate_mc_q;
		public uint DcecsInterpolateMcq { get { return dcecs_interpolate_mc_q; } set { dcecs_interpolate_mc_q = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public ReadVopComplexityEstimationHeader()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            MPEG4Context ituContext = context as MPEG4Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type MPEG4Context");

            ulong size = 0;


			if (ituContext.ComplexityEstimation.EstimationMethod == 0 || ituContext.ComplexityEstimation.EstimationMethod == 1)
			{

				if (ituContext.VopCodingType == 0)
				{

					if (ituContext.ComplexityEstimation.Opaque != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_opaque, "dcecs_opaque"); 
					}

					if (ituContext.ComplexityEstimation.Transparent != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_transparent, "dcecs_transparent"); 
					}

					if (ituContext.ComplexityEstimation.IntraCae != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_intra_cae, "dcecs_intra_cae"); 
					}

					if (ituContext.ComplexityEstimation.InterCae != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_inter_cae, "dcecs_inter_cae"); 
					}

					if (ituContext.ComplexityEstimation.NoUpdate != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_no_update, "dcecs_no_update"); 
					}

					if (ituContext.ComplexityEstimation.Upsampling != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_upsampling, "dcecs_upsampling"); 
					}

					if (ituContext.ComplexityEstimation.IntraBlocks != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_intra_blocks, "dcecs_intra_blocks"); 
					}

					if (ituContext.ComplexityEstimation.NotCodedBlocks != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_not_coded_blocks, "dcecs_not_coded_blocks"); 
					}

					if (ituContext.ComplexityEstimation.DctCoefs != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_dct_coefs, "dcecs_dct_coefs"); 
					}

					if (ituContext.ComplexityEstimation.DctLines != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_dct_lines, "dcecs_dct_lines"); 
					}

					if (ituContext.ComplexityEstimation.VlcSymbols != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_vlc_symbols, "dcecs_vlc_symbols"); 
					}

					if (ituContext.ComplexityEstimation.VlcBits != 0)
					{
						size += stream.ReadUnsignedInt(size, 4, out this.dcecs_vlc_bits, "dcecs_vlc_bits"); 
					}

					if (ituContext.ComplexityEstimation.Sadct != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_sadct, "dcecs_sadct"); 
					}
				}

				if (ituContext.VopCodingType == 1)
				{

					if (ituContext.ComplexityEstimation.Opaque != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_opaque, "dcecs_opaque"); 
					}

					if (ituContext.ComplexityEstimation.Transparent != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_transparent, "dcecs_transparent"); 
					}

					if (ituContext.ComplexityEstimation.IntraCae != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_intra_cae, "dcecs_intra_cae"); 
					}

					if (ituContext.ComplexityEstimation.InterCae != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_inter_cae, "dcecs_inter_cae"); 
					}

					if (ituContext.ComplexityEstimation.NoUpdate != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_no_update, "dcecs_no_update"); 
					}

					if (ituContext.ComplexityEstimation.Upsampling != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_upsampling, "dcecs_upsampling"); 
					}

					if (ituContext.ComplexityEstimation.IntraBlocks != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_intra_blocks, "dcecs_intra_blocks"); 
					}

					if (ituContext.ComplexityEstimation.NotCodedBlocks != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_not_coded_blocks, "dcecs_not_coded_blocks"); 
					}

					if (ituContext.ComplexityEstimation.DctCoefs != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_dct_coefs, "dcecs_dct_coefs"); 
					}

					if (ituContext.ComplexityEstimation.DctLines != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_dct_lines, "dcecs_dct_lines"); 
					}

					if (ituContext.ComplexityEstimation.VlcSymbols != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_vlc_symbols, "dcecs_vlc_symbols"); 
					}

					if (ituContext.ComplexityEstimation.VlcBits != 0)
					{
						size += stream.ReadUnsignedInt(size, 4, out this.dcecs_vlc_bits, "dcecs_vlc_bits"); 
					}

					if (ituContext.ComplexityEstimation.InterBlocks != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_inter_blocks, "dcecs_inter_blocks"); 
					}

					if (ituContext.ComplexityEstimation.Inter4vBlocks != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_inter4v_blocks, "dcecs_inter4v_blocks"); 
					}

					if (ituContext.ComplexityEstimation.Apm != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_apm, "dcecs_apm"); 
					}

					if (ituContext.ComplexityEstimation.Npm != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_npm, "dcecs_npm"); 
					}

					if (ituContext.ComplexityEstimation.ForwBackMcq != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_forw_back_mc_q, "dcecs_forw_back_mc_q"); 
					}

					if (ituContext.ComplexityEstimation.Halfpel2 != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_halfpel2, "dcecs_halfpel2"); 
					}

					if (ituContext.ComplexityEstimation.Halfpel4 != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_halfpel4, "dcecs_halfpel4"); 
					}

					if (ituContext.ComplexityEstimation.Sadct != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_sadct, "dcecs_sadct"); 
					}

					if (ituContext.ComplexityEstimation.Quarterpel != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_quarterpel, "dcecs_quarterpel"); 
					}
				}

				if (ituContext.VopCodingType == 2)
				{

					if (ituContext.ComplexityEstimation.Opaque != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_opaque, "dcecs_opaque"); 
					}

					if (ituContext.ComplexityEstimation.Transparent != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_transparent, "dcecs_transparent"); 
					}

					if (ituContext.ComplexityEstimation.IntraCae != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_intra_cae, "dcecs_intra_cae"); 
					}

					if (ituContext.ComplexityEstimation.InterCae != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_inter_cae, "dcecs_inter_cae"); 
					}

					if (ituContext.ComplexityEstimation.NoUpdate != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_no_update, "dcecs_no_update"); 
					}

					if (ituContext.ComplexityEstimation.Upsampling != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_upsampling, "dcecs_upsampling"); 
					}

					if (ituContext.ComplexityEstimation.IntraBlocks != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_intra_blocks, "dcecs_intra_blocks"); 
					}

					if (ituContext.ComplexityEstimation.NotCodedBlocks != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_not_coded_blocks, "dcecs_not_coded_blocks"); 
					}

					if (ituContext.ComplexityEstimation.DctCoefs != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_dct_coefs, "dcecs_dct_coefs"); 
					}

					if (ituContext.ComplexityEstimation.DctLines != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_dct_lines, "dcecs_dct_lines"); 
					}

					if (ituContext.ComplexityEstimation.VlcSymbols != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_vlc_symbols, "dcecs_vlc_symbols"); 
					}

					if (ituContext.ComplexityEstimation.VlcBits != 0)
					{
						size += stream.ReadUnsignedInt(size, 4, out this.dcecs_vlc_bits, "dcecs_vlc_bits"); 
					}

					if (ituContext.ComplexityEstimation.InterBlocks != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_inter_blocks, "dcecs_inter_blocks"); 
					}

					if (ituContext.ComplexityEstimation.Inter4vBlocks != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_inter4v_blocks, "dcecs_inter4v_blocks"); 
					}

					if (ituContext.ComplexityEstimation.Apm != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_apm, "dcecs_apm"); 
					}

					if (ituContext.ComplexityEstimation.Npm != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_npm, "dcecs_npm"); 
					}

					if (ituContext.ComplexityEstimation.ForwBackMcq != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_forw_back_mc_q, "dcecs_forw_back_mc_q"); 
					}

					if (ituContext.ComplexityEstimation.Halfpel2 != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_halfpel2, "dcecs_halfpel2"); 
					}

					if (ituContext.ComplexityEstimation.Halfpel4 != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_halfpel4, "dcecs_halfpel4"); 
					}

					if (ituContext.ComplexityEstimation.InterpolateMcq != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_interpolate_mc_q, "dcecs_interpolate_mc_q"); 
					}

					if (ituContext.ComplexityEstimation.Sadct != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_sadct, "dcecs_sadct"); 
					}

					if (ituContext.ComplexityEstimation.Quarterpel != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_quarterpel, "dcecs_quarterpel"); 
					}
				}

				if (ituContext.VopCodingType == 3&& ituContext.VideoObjectLayer.SpriteEnable == 1)
				{

					if (ituContext.ComplexityEstimation.IntraBlocks != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_intra_blocks, "dcecs_intra_blocks"); 
					}

					if (ituContext.ComplexityEstimation.NotCodedBlocks != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_not_coded_blocks, "dcecs_not_coded_blocks"); 
					}

					if (ituContext.ComplexityEstimation.DctCoefs != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_dct_coefs, "dcecs_dct_coefs"); 
					}

					if (ituContext.ComplexityEstimation.DctLines != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_dct_lines, "dcecs_dct_lines"); 
					}

					if (ituContext.ComplexityEstimation.VlcSymbols != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_vlc_symbols, "dcecs_vlc_symbols"); 
					}

					if (ituContext.ComplexityEstimation.VlcBits != 0)
					{
						size += stream.ReadUnsignedInt(size, 4, out this.dcecs_vlc_bits, "dcecs_vlc_bits"); 
					}

					if (ituContext.ComplexityEstimation.InterBlocks != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_inter_blocks, "dcecs_inter_blocks"); 
					}

					if (ituContext.ComplexityEstimation.Inter4vBlocks != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_inter4v_blocks, "dcecs_inter4v_blocks"); 
					}

					if (ituContext.ComplexityEstimation.Apm != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_apm, "dcecs_apm"); 
					}

					if (ituContext.ComplexityEstimation.Npm != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_npm, "dcecs_npm"); 
					}

					if (ituContext.ComplexityEstimation.ForwBackMcq != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_forw_back_mc_q, "dcecs_forw_back_mc_q"); 
					}

					if (ituContext.ComplexityEstimation.Halfpel2 != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_halfpel2, "dcecs_halfpel2"); 
					}

					if (ituContext.ComplexityEstimation.Halfpel4 != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_halfpel4, "dcecs_halfpel4"); 
					}

					if (ituContext.ComplexityEstimation.InterpolateMcq != 0)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.dcecs_interpolate_mc_q, "dcecs_interpolate_mc_q"); 
					}
				}
			}

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            MPEG4Context ituContext = context as MPEG4Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type MPEG4Context");
            ulong size = 0;


			if (ituContext.ComplexityEstimation.EstimationMethod == 0 || ituContext.ComplexityEstimation.EstimationMethod == 1)
			{

				if (ituContext.VopCodingType == 0)
				{

					if (ituContext.ComplexityEstimation.Opaque != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_opaque, "dcecs_opaque"); 
					}

					if (ituContext.ComplexityEstimation.Transparent != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_transparent, "dcecs_transparent"); 
					}

					if (ituContext.ComplexityEstimation.IntraCae != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_intra_cae, "dcecs_intra_cae"); 
					}

					if (ituContext.ComplexityEstimation.InterCae != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_inter_cae, "dcecs_inter_cae"); 
					}

					if (ituContext.ComplexityEstimation.NoUpdate != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_no_update, "dcecs_no_update"); 
					}

					if (ituContext.ComplexityEstimation.Upsampling != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_upsampling, "dcecs_upsampling"); 
					}

					if (ituContext.ComplexityEstimation.IntraBlocks != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_intra_blocks, "dcecs_intra_blocks"); 
					}

					if (ituContext.ComplexityEstimation.NotCodedBlocks != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_not_coded_blocks, "dcecs_not_coded_blocks"); 
					}

					if (ituContext.ComplexityEstimation.DctCoefs != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_dct_coefs, "dcecs_dct_coefs"); 
					}

					if (ituContext.ComplexityEstimation.DctLines != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_dct_lines, "dcecs_dct_lines"); 
					}

					if (ituContext.ComplexityEstimation.VlcSymbols != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_vlc_symbols, "dcecs_vlc_symbols"); 
					}

					if (ituContext.ComplexityEstimation.VlcBits != 0)
					{
						size += stream.WriteUnsignedInt(4, this.dcecs_vlc_bits, "dcecs_vlc_bits"); 
					}

					if (ituContext.ComplexityEstimation.Sadct != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_sadct, "dcecs_sadct"); 
					}
				}

				if (ituContext.VopCodingType == 1)
				{

					if (ituContext.ComplexityEstimation.Opaque != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_opaque, "dcecs_opaque"); 
					}

					if (ituContext.ComplexityEstimation.Transparent != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_transparent, "dcecs_transparent"); 
					}

					if (ituContext.ComplexityEstimation.IntraCae != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_intra_cae, "dcecs_intra_cae"); 
					}

					if (ituContext.ComplexityEstimation.InterCae != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_inter_cae, "dcecs_inter_cae"); 
					}

					if (ituContext.ComplexityEstimation.NoUpdate != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_no_update, "dcecs_no_update"); 
					}

					if (ituContext.ComplexityEstimation.Upsampling != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_upsampling, "dcecs_upsampling"); 
					}

					if (ituContext.ComplexityEstimation.IntraBlocks != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_intra_blocks, "dcecs_intra_blocks"); 
					}

					if (ituContext.ComplexityEstimation.NotCodedBlocks != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_not_coded_blocks, "dcecs_not_coded_blocks"); 
					}

					if (ituContext.ComplexityEstimation.DctCoefs != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_dct_coefs, "dcecs_dct_coefs"); 
					}

					if (ituContext.ComplexityEstimation.DctLines != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_dct_lines, "dcecs_dct_lines"); 
					}

					if (ituContext.ComplexityEstimation.VlcSymbols != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_vlc_symbols, "dcecs_vlc_symbols"); 
					}

					if (ituContext.ComplexityEstimation.VlcBits != 0)
					{
						size += stream.WriteUnsignedInt(4, this.dcecs_vlc_bits, "dcecs_vlc_bits"); 
					}

					if (ituContext.ComplexityEstimation.InterBlocks != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_inter_blocks, "dcecs_inter_blocks"); 
					}

					if (ituContext.ComplexityEstimation.Inter4vBlocks != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_inter4v_blocks, "dcecs_inter4v_blocks"); 
					}

					if (ituContext.ComplexityEstimation.Apm != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_apm, "dcecs_apm"); 
					}

					if (ituContext.ComplexityEstimation.Npm != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_npm, "dcecs_npm"); 
					}

					if (ituContext.ComplexityEstimation.ForwBackMcq != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_forw_back_mc_q, "dcecs_forw_back_mc_q"); 
					}

					if (ituContext.ComplexityEstimation.Halfpel2 != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_halfpel2, "dcecs_halfpel2"); 
					}

					if (ituContext.ComplexityEstimation.Halfpel4 != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_halfpel4, "dcecs_halfpel4"); 
					}

					if (ituContext.ComplexityEstimation.Sadct != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_sadct, "dcecs_sadct"); 
					}

					if (ituContext.ComplexityEstimation.Quarterpel != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_quarterpel, "dcecs_quarterpel"); 
					}
				}

				if (ituContext.VopCodingType == 2)
				{

					if (ituContext.ComplexityEstimation.Opaque != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_opaque, "dcecs_opaque"); 
					}

					if (ituContext.ComplexityEstimation.Transparent != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_transparent, "dcecs_transparent"); 
					}

					if (ituContext.ComplexityEstimation.IntraCae != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_intra_cae, "dcecs_intra_cae"); 
					}

					if (ituContext.ComplexityEstimation.InterCae != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_inter_cae, "dcecs_inter_cae"); 
					}

					if (ituContext.ComplexityEstimation.NoUpdate != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_no_update, "dcecs_no_update"); 
					}

					if (ituContext.ComplexityEstimation.Upsampling != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_upsampling, "dcecs_upsampling"); 
					}

					if (ituContext.ComplexityEstimation.IntraBlocks != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_intra_blocks, "dcecs_intra_blocks"); 
					}

					if (ituContext.ComplexityEstimation.NotCodedBlocks != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_not_coded_blocks, "dcecs_not_coded_blocks"); 
					}

					if (ituContext.ComplexityEstimation.DctCoefs != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_dct_coefs, "dcecs_dct_coefs"); 
					}

					if (ituContext.ComplexityEstimation.DctLines != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_dct_lines, "dcecs_dct_lines"); 
					}

					if (ituContext.ComplexityEstimation.VlcSymbols != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_vlc_symbols, "dcecs_vlc_symbols"); 
					}

					if (ituContext.ComplexityEstimation.VlcBits != 0)
					{
						size += stream.WriteUnsignedInt(4, this.dcecs_vlc_bits, "dcecs_vlc_bits"); 
					}

					if (ituContext.ComplexityEstimation.InterBlocks != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_inter_blocks, "dcecs_inter_blocks"); 
					}

					if (ituContext.ComplexityEstimation.Inter4vBlocks != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_inter4v_blocks, "dcecs_inter4v_blocks"); 
					}

					if (ituContext.ComplexityEstimation.Apm != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_apm, "dcecs_apm"); 
					}

					if (ituContext.ComplexityEstimation.Npm != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_npm, "dcecs_npm"); 
					}

					if (ituContext.ComplexityEstimation.ForwBackMcq != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_forw_back_mc_q, "dcecs_forw_back_mc_q"); 
					}

					if (ituContext.ComplexityEstimation.Halfpel2 != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_halfpel2, "dcecs_halfpel2"); 
					}

					if (ituContext.ComplexityEstimation.Halfpel4 != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_halfpel4, "dcecs_halfpel4"); 
					}

					if (ituContext.ComplexityEstimation.InterpolateMcq != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_interpolate_mc_q, "dcecs_interpolate_mc_q"); 
					}

					if (ituContext.ComplexityEstimation.Sadct != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_sadct, "dcecs_sadct"); 
					}

					if (ituContext.ComplexityEstimation.Quarterpel != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_quarterpel, "dcecs_quarterpel"); 
					}
				}

				if (ituContext.VopCodingType == 3&& ituContext.VideoObjectLayer.SpriteEnable == 1)
				{

					if (ituContext.ComplexityEstimation.IntraBlocks != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_intra_blocks, "dcecs_intra_blocks"); 
					}

					if (ituContext.ComplexityEstimation.NotCodedBlocks != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_not_coded_blocks, "dcecs_not_coded_blocks"); 
					}

					if (ituContext.ComplexityEstimation.DctCoefs != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_dct_coefs, "dcecs_dct_coefs"); 
					}

					if (ituContext.ComplexityEstimation.DctLines != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_dct_lines, "dcecs_dct_lines"); 
					}

					if (ituContext.ComplexityEstimation.VlcSymbols != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_vlc_symbols, "dcecs_vlc_symbols"); 
					}

					if (ituContext.ComplexityEstimation.VlcBits != 0)
					{
						size += stream.WriteUnsignedInt(4, this.dcecs_vlc_bits, "dcecs_vlc_bits"); 
					}

					if (ituContext.ComplexityEstimation.InterBlocks != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_inter_blocks, "dcecs_inter_blocks"); 
					}

					if (ituContext.ComplexityEstimation.Inter4vBlocks != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_inter4v_blocks, "dcecs_inter4v_blocks"); 
					}

					if (ituContext.ComplexityEstimation.Apm != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_apm, "dcecs_apm"); 
					}

					if (ituContext.ComplexityEstimation.Npm != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_npm, "dcecs_npm"); 
					}

					if (ituContext.ComplexityEstimation.ForwBackMcq != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_forw_back_mc_q, "dcecs_forw_back_mc_q"); 
					}

					if (ituContext.ComplexityEstimation.Halfpel2 != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_halfpel2, "dcecs_halfpel2"); 
					}

					if (ituContext.ComplexityEstimation.Halfpel4 != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_halfpel4, "dcecs_halfpel4"); 
					}

					if (ituContext.ComplexityEstimation.InterpolateMcq != 0)
					{
						size += stream.WriteUnsignedInt(8, this.dcecs_interpolate_mc_q, "dcecs_interpolate_mc_q"); 
					}
				}
			}

            return size;
         }

    }

    /*


video_plane_with_short_header() {
short_video_start_marker u(22)
temporal_reference u(8)
marker_bit u(1)
zero_bit u(1)
split_screen_indicator u(1)
document_camera_indicator u(1)
full_picture_freeze_release u(1)
source_format u(3)
picture_coding_type u(1)
four_reserved_zero_bits u(4)
vop_quant u(5)
zero_bit u(1)
do{
pei u(1)
if (pei_set)
psupp u(8)
} while (pei_set)
vop_data()
}
    */
    public class VideoPlaneWithShortHeader : IItuSerializable
    {
		private uint short_video_start_marker;
		public uint ShortVideoStartMarker { get { return short_video_start_marker; } set { short_video_start_marker = value; } }
		private uint temporal_reference;
		public uint TemporalReference { get { return temporal_reference; } set { temporal_reference = value; } }
		private byte marker_bit;
		public byte MarkerBit { get { return marker_bit; } set { marker_bit = value; } }
		private byte zero_bit;
		public byte ZeroBit { get { return zero_bit; } set { zero_bit = value; } }
		private byte split_screen_indicator;
		public byte SplitScreenIndicator { get { return split_screen_indicator; } set { split_screen_indicator = value; } }
		private byte document_camera_indicator;
		public byte DocumentCameraIndicator { get { return document_camera_indicator; } set { document_camera_indicator = value; } }
		private byte full_picture_freeze_release;
		public byte FullPictureFreezeRelease { get { return full_picture_freeze_release; } set { full_picture_freeze_release = value; } }
		private uint source_format;
		public uint SourceFormat { get { return source_format; } set { source_format = value; } }
		private byte picture_coding_type;
		public byte PictureCodingType { get { return picture_coding_type; } set { picture_coding_type = value; } }
		private uint four_reserved_zero_bits;
		public uint FourReservedZeroBits { get { return four_reserved_zero_bits; } set { four_reserved_zero_bits = value; } }
		private uint vop_quant;
		public uint VopQuant { get { return vop_quant; } set { vop_quant = value; } }
		private Dictionary<int, byte> pei;
		public Dictionary<int, byte> Pei { get { return pei ??= new Dictionary<int, byte>(); } set { pei = value; } }
		private Dictionary<int, uint> psupp;
		public Dictionary<int, uint> Psupp { get { return psupp ??= new Dictionary<int, uint>(); } set { psupp = value; } }
		private VopData vop_data;
		public VopData VopData { get { return vop_data; } set { vop_data = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public VideoPlaneWithShortHeader()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            MPEG4Context ituContext = context as MPEG4Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type MPEG4Context");

            ulong size = 0;

			int whileIndex = -1;
			size += stream.ReadUnsignedInt(size, 22, out this.short_video_start_marker, "short_video_start_marker"); 
			size += stream.ReadUnsignedInt(size, 8, out this.temporal_reference, "temporal_reference"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 1, out this.zero_bit, "zero_bit"); 
			size += stream.ReadUnsignedInt(size, 1, out this.split_screen_indicator, "split_screen_indicator"); 
			size += stream.ReadUnsignedInt(size, 1, out this.document_camera_indicator, "document_camera_indicator"); 
			size += stream.ReadUnsignedInt(size, 1, out this.full_picture_freeze_release, "full_picture_freeze_release"); 
			size += stream.ReadUnsignedInt(size, 3, out this.source_format, "source_format"); 
			size += stream.ReadUnsignedInt(size, 1, out this.picture_coding_type, "picture_coding_type"); 
			size += stream.ReadUnsignedInt(size, 4, out this.four_reserved_zero_bits, "four_reserved_zero_bits"); 
			size += stream.ReadUnsignedInt(size, 5, out this.vop_quant, "vop_quant"); 
			size += stream.ReadUnsignedInt(size, 1, out this.zero_bit, "zero_bit"); 

			do
			{
				whileIndex++;

				size += stream.ReadUnsignedInt(size, 1, whileIndex, (this.pei ??= new()), "pei"); 

				if ((this.pei[whileIndex] == 1 ? 1 : 0) != 0)
				{
					size += stream.ReadUnsignedInt(size, 8, whileIndex, (this.psupp ??= new()), "psupp"); 
				}
			} while ((this.pei[whileIndex] == 1 ? 1 : 0) != 0);
			this.vop_data =  new VopData() ;
			size +=  stream.ReadClass<VopData>(size, context, this.vop_data, "vop_data"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            MPEG4Context ituContext = context as MPEG4Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type MPEG4Context");
            ulong size = 0;

			int whileIndex = -1;
			size += stream.WriteUnsignedInt(22, this.short_video_start_marker, "short_video_start_marker"); 
			size += stream.WriteUnsignedInt(8, this.temporal_reference, "temporal_reference"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(1, this.zero_bit, "zero_bit"); 
			size += stream.WriteUnsignedInt(1, this.split_screen_indicator, "split_screen_indicator"); 
			size += stream.WriteUnsignedInt(1, this.document_camera_indicator, "document_camera_indicator"); 
			size += stream.WriteUnsignedInt(1, this.full_picture_freeze_release, "full_picture_freeze_release"); 
			size += stream.WriteUnsignedInt(3, this.source_format, "source_format"); 
			size += stream.WriteUnsignedInt(1, this.picture_coding_type, "picture_coding_type"); 
			size += stream.WriteUnsignedInt(4, this.four_reserved_zero_bits, "four_reserved_zero_bits"); 
			size += stream.WriteUnsignedInt(5, this.vop_quant, "vop_quant"); 
			size += stream.WriteUnsignedInt(1, this.zero_bit, "zero_bit"); 

			do
			{
				whileIndex++;

				size += stream.WriteUnsignedInt(1, whileIndex, (this.pei ??= new()), "pei"); 

				if ((this.pei[whileIndex] == 1 ? 1 : 0) != 0)
				{
					size += stream.WriteUnsignedInt(8, whileIndex, (this.psupp ??= new()), "psupp"); 
				}
			} while ((this.pei[whileIndex] == 1 ? 1 : 0) != 0);
			size += stream.WriteClass<VopData>(context, this.vop_data, "vop_data"); 

            return size;
         }

    }

}
