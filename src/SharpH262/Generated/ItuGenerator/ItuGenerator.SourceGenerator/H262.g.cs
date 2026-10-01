using System;
using System.Collections.Generic;
using System.Numerics;
using SharpH26X;

namespace SharpH262
{

    public partial class H262Context : IItuContext
    {

    }

    /*
sequence_header() {
sequence_header_code u(32)
horizontal_size_value u(12)
vertical_size_value u(12)
aspect_ratio_information u(4)
frame_rate_code u(4)
bit_rate_value u(18)
marker_bit u(1)
vbv_buffer_size_value u(10)
constrained_parameters_flag u(1)
load_intra_quantiser_matrix u(1)
if ( load_intra_quantiser_matrix )
for ( i = 0; i < 64; i++ )
intra_quantiser_matrix[ i ] u(8)
load_non_intra_quantiser_matrix u(1)
if ( load_non_intra_quantiser_matrix )
for ( i = 0; i < 64; i++ )
non_intra_quantiser_matrix[ i ] u(8)
next_start_code()
}
    */
    public class SequenceHeader : IItuSerializable
    {
		private uint sequence_header_code;
		public uint SequenceHeaderCode { get { return sequence_header_code; } set { sequence_header_code = value; } }
		private uint horizontal_size_value;
		public uint HorizontalSizeValue { get { return horizontal_size_value; } set { horizontal_size_value = value; } }
		private uint vertical_size_value;
		public uint VerticalSizeValue { get { return vertical_size_value; } set { vertical_size_value = value; } }
		private uint aspect_ratio_information;
		public uint AspectRatioInformation { get { return aspect_ratio_information; } set { aspect_ratio_information = value; } }
		private uint frame_rate_code;
		public uint FrameRateCode { get { return frame_rate_code; } set { frame_rate_code = value; } }
		private uint bit_rate_value;
		public uint BitRateValue { get { return bit_rate_value; } set { bit_rate_value = value; } }
		private byte marker_bit;
		public byte MarkerBit { get { return marker_bit; } set { marker_bit = value; } }
		private uint vbv_buffer_size_value;
		public uint VbvBufferSizeValue { get { return vbv_buffer_size_value; } set { vbv_buffer_size_value = value; } }
		private byte constrained_parameters_flag;
		public byte ConstrainedParametersFlag { get { return constrained_parameters_flag; } set { constrained_parameters_flag = value; } }
		private byte load_intra_quantiser_matrix;
		public byte LoadIntraQuantiserMatrix { get { return load_intra_quantiser_matrix; } set { load_intra_quantiser_matrix = value; } }
		private uint[] intra_quantiser_matrix;
		public uint[] IntraQuantiserMatrix { get { return intra_quantiser_matrix; } set { intra_quantiser_matrix = value; } }
		private byte load_non_intra_quantiser_matrix;
		public byte LoadNonIntraQuantiserMatrix { get { return load_non_intra_quantiser_matrix; } set { load_non_intra_quantiser_matrix = value; } }
		private uint[] non_intra_quantiser_matrix;
		public uint[] NonIntraQuantiserMatrix { get { return non_intra_quantiser_matrix; } set { non_intra_quantiser_matrix = value; } }
		private NextStartCode next_start_code;
		public NextStartCode NextStartCode { get { return next_start_code; } set { next_start_code = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public SequenceHeader()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");

            ulong size = 0;

			uint i = 0;
			size += stream.ReadUnsignedInt(size, 32, out this.sequence_header_code, "sequence_header_code"); 
			size += stream.ReadUnsignedInt(size, 12, out this.horizontal_size_value, "horizontal_size_value"); 
			size += stream.ReadUnsignedInt(size, 12, out this.vertical_size_value, "vertical_size_value"); 
			size += stream.ReadUnsignedInt(size, 4, out this.aspect_ratio_information, "aspect_ratio_information"); 
			size += stream.ReadUnsignedInt(size, 4, out this.frame_rate_code, "frame_rate_code"); 
			size += stream.ReadUnsignedInt(size, 18, out this.bit_rate_value, "bit_rate_value"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 10, out this.vbv_buffer_size_value, "vbv_buffer_size_value"); 
			size += stream.ReadUnsignedInt(size, 1, out this.constrained_parameters_flag, "constrained_parameters_flag"); 
			size += stream.ReadUnsignedInt(size, 1, out this.load_intra_quantiser_matrix, "load_intra_quantiser_matrix"); 

			if ( load_intra_quantiser_matrix != 0 )
			{

				stream.CheckArrayAllocation((ulong)( 64), "intra_quantiser_matrix");
				this.intra_quantiser_matrix = new uint[ 64];
				for ( i = 0; i < 64; i++ )
				{
					size += stream.ReadUnsignedInt(size, 8, out this.intra_quantiser_matrix[ i ], "intra_quantiser_matrix"); 
				}
			}
			size += stream.ReadUnsignedInt(size, 1, out this.load_non_intra_quantiser_matrix, "load_non_intra_quantiser_matrix"); 

			if ( load_non_intra_quantiser_matrix != 0 )
			{

				stream.CheckArrayAllocation((ulong)( 64), "non_intra_quantiser_matrix");
				this.non_intra_quantiser_matrix = new uint[ 64];
				for ( i = 0; i < 64; i++ )
				{
					size += stream.ReadUnsignedInt(size, 8, out this.non_intra_quantiser_matrix[ i ], "non_intra_quantiser_matrix"); 
				}
			}
			this.next_start_code =  new NextStartCode() ;
			size +=  stream.ReadClass<NextStartCode>(size, context, this.next_start_code, "next_start_code"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");
            ulong size = 0;

			uint i = 0;
			size += stream.WriteUnsignedInt(32, this.sequence_header_code, "sequence_header_code"); 
			size += stream.WriteUnsignedInt(12, this.horizontal_size_value, "horizontal_size_value"); 
			size += stream.WriteUnsignedInt(12, this.vertical_size_value, "vertical_size_value"); 
			size += stream.WriteUnsignedInt(4, this.aspect_ratio_information, "aspect_ratio_information"); 
			size += stream.WriteUnsignedInt(4, this.frame_rate_code, "frame_rate_code"); 
			size += stream.WriteUnsignedInt(18, this.bit_rate_value, "bit_rate_value"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(10, this.vbv_buffer_size_value, "vbv_buffer_size_value"); 
			size += stream.WriteUnsignedInt(1, this.constrained_parameters_flag, "constrained_parameters_flag"); 
			size += stream.WriteUnsignedInt(1, this.load_intra_quantiser_matrix, "load_intra_quantiser_matrix"); 

			if ( load_intra_quantiser_matrix != 0 )
			{

				for ( i = 0; i < 64; i++ )
				{
					size += stream.WriteUnsignedInt(8, this.intra_quantiser_matrix[ i ], "intra_quantiser_matrix"); 
				}
			}
			size += stream.WriteUnsignedInt(1, this.load_non_intra_quantiser_matrix, "load_non_intra_quantiser_matrix"); 

			if ( load_non_intra_quantiser_matrix != 0 )
			{

				for ( i = 0; i < 64; i++ )
				{
					size += stream.WriteUnsignedInt(8, this.non_intra_quantiser_matrix[ i ], "non_intra_quantiser_matrix"); 
				}
			}
			size += stream.WriteClass<NextStartCode>(context, this.next_start_code, "next_start_code"); 

            return size;
         }

    }

    /*


extension_data( after ) {
if ( one_extension ) {
extension_start_code u(32)
if (after == 0) { /* follows sequence_extension() *//*
if ( extension_follows( sequence_display_extension ) )
sequence_display_extension()
else if ( extension_follows( sequence_scalable_extension ) )
sequence_scalable_extension()
else
while ( before_start_code( reserved_extension_data_byte ) )
reserved_extension_data_byte u(8)
}
/* NOTE – i never takes the value 1 because extension_data()
never follows a group_of_pictures_header() *//*
if (after == 2) { /* follows picture_coding_extension() *//*
if ( extension_follows( quant_matrix_extension ) )
quant_matrix_extension()
else if ( extension_follows( copyright_extension ) )
copyright_extension()
else if ( extension_follows( picture_display_extension ) )
picture_display_extension()
else if ( extension_follows( picture_spatial_scalable_extension ) )
picture_spatial_scalable_extension()
else if ( extension_follows( picture_temporal_scalable_extension ) )
picture_temporal_scalable_extension()
else if ( extension_follows( camera_parameters_extension ) )
camera_parameters_extension()
else if ( extension_follows( itu_t_extension ) )
itu_t_extension()
else
while ( before_start_code( reserved_extension_data_byte ) )
reserved_extension_data_byte u(8)
}
}
}
    */
    public class ExtensionData : IItuSerializable
    {
		private uint after;
		public uint After { get { return after; } set { after = value; } }
		private uint extension_start_code;
		public uint ExtensionStartCode { get { return extension_start_code; } set { extension_start_code = value; } }
		private SequenceDisplayExtension sequence_display_extension;
		public SequenceDisplayExtension SequenceDisplayExtension { get { return sequence_display_extension; } set { sequence_display_extension = value; } }
		private SequenceScalableExtension sequence_scalable_extension;
		public SequenceScalableExtension SequenceScalableExtension { get { return sequence_scalable_extension; } set { sequence_scalable_extension = value; } }
		private Dictionary<int, uint> reserved_extension_data_byte;
		public Dictionary<int, uint> ReservedExtensionDataByte { get { return reserved_extension_data_byte ??= new Dictionary<int, uint>(); } set { reserved_extension_data_byte = value; } }
		private QuantMatrixExtension quant_matrix_extension;
		public QuantMatrixExtension QuantMatrixExtension { get { return quant_matrix_extension; } set { quant_matrix_extension = value; } }
		private CopyrightExtension copyright_extension;
		public CopyrightExtension CopyrightExtension { get { return copyright_extension; } set { copyright_extension = value; } }
		private PictureDisplayExtension picture_display_extension;
		public PictureDisplayExtension PictureDisplayExtension { get { return picture_display_extension; } set { picture_display_extension = value; } }
		private PictureSpatialScalableExtension picture_spatial_scalable_extension;
		public PictureSpatialScalableExtension PictureSpatialScalableExtension { get { return picture_spatial_scalable_extension; } set { picture_spatial_scalable_extension = value; } }
		private PictureTemporalScalableExtension picture_temporal_scalable_extension;
		public PictureTemporalScalableExtension PictureTemporalScalableExtension { get { return picture_temporal_scalable_extension; } set { picture_temporal_scalable_extension = value; } }
		private CameraParametersExtension camera_parameters_extension;
		public CameraParametersExtension CameraParametersExtension { get { return camera_parameters_extension; } set { camera_parameters_extension = value; } }
		private ItutExtension itu_t_extension;
		public ItutExtension ItutExtension { get { return itu_t_extension; } set { itu_t_extension = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public ExtensionData(uint after)
         { 
			this.after = after;
         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");

            ulong size = 0;

			int whileIndex = -1;

			if ( 1 != 0 )
			{
				size += stream.ReadUnsignedInt(size, 32, out this.extension_start_code, "extension_start_code"); 

				if (after == 0)
				{
/*  follows sequence_extension()  */


					if ( ituContext.NextBits(stream, 4) == 2 )
					{
						this.sequence_display_extension =  new SequenceDisplayExtension() ;
						size +=  stream.ReadClass<SequenceDisplayExtension>(size, context, this.sequence_display_extension, "sequence_display_extension"); 
					}
					else if ( ituContext.NextBits(stream, 4) == 5 )
					{
						this.sequence_scalable_extension =  new SequenceScalableExtension() ;
						size +=  stream.ReadClass<SequenceScalableExtension>(size, context, this.sequence_scalable_extension, "sequence_scalable_extension"); 
					}
					else 
					{

						while ( !ituContext.AtStartCode(stream) )
						{
							whileIndex++;

							size += stream.ReadUnsignedInt(size, 8, whileIndex, (this.reserved_extension_data_byte ??= new()), "reserved_extension_data_byte"); 
						}
					}
				}
/*  NOTE – i never takes the value 1 because extension_data()
never follows a group_of_pictures_header()  */


				if (after == 2)
				{
/*  follows picture_coding_extension()  */


					if ( ituContext.NextBits(stream, 4) == 3 )
					{
						this.quant_matrix_extension =  new QuantMatrixExtension() ;
						size +=  stream.ReadClass<QuantMatrixExtension>(size, context, this.quant_matrix_extension, "quant_matrix_extension"); 
					}
					else if ( ituContext.NextBits(stream, 4) == 4 )
					{
						this.copyright_extension =  new CopyrightExtension() ;
						size +=  stream.ReadClass<CopyrightExtension>(size, context, this.copyright_extension, "copyright_extension"); 
					}
					else if ( ituContext.NextBits(stream, 4) == 7 )
					{
						this.picture_display_extension =  new PictureDisplayExtension() ;
						size +=  stream.ReadClass<PictureDisplayExtension>(size, context, this.picture_display_extension, "picture_display_extension"); 
					}
					else if ( ituContext.NextBits(stream, 4) == 9 )
					{
						this.picture_spatial_scalable_extension =  new PictureSpatialScalableExtension() ;
						size +=  stream.ReadClass<PictureSpatialScalableExtension>(size, context, this.picture_spatial_scalable_extension, "picture_spatial_scalable_extension"); 
					}
					else if ( ituContext.NextBits(stream, 4) == 10 )
					{
						this.picture_temporal_scalable_extension =  new PictureTemporalScalableExtension() ;
						size +=  stream.ReadClass<PictureTemporalScalableExtension>(size, context, this.picture_temporal_scalable_extension, "picture_temporal_scalable_extension"); 
					}
					else if ( ituContext.NextBits(stream, 4) == 11 )
					{
						this.camera_parameters_extension =  new CameraParametersExtension() ;
						size +=  stream.ReadClass<CameraParametersExtension>(size, context, this.camera_parameters_extension, "camera_parameters_extension"); 
					}
					else if ( ituContext.NextBits(stream, 4) == 12 )
					{
						this.itu_t_extension =  new ItutExtension() ;
						size +=  stream.ReadClass<ItutExtension>(size, context, this.itu_t_extension, "itu_t_extension"); 
					}
					else 
					{

						while ( !ituContext.AtStartCode(stream) )
						{
							whileIndex++;

							size += stream.ReadUnsignedInt(size, 8, whileIndex, (this.reserved_extension_data_byte ??= new()), "reserved_extension_data_byte"); 
						}
					}
				}
			}

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");
            ulong size = 0;

			int whileIndex = -1;

			if ( 1 != 0 )
			{
				size += stream.WriteUnsignedInt(32, this.extension_start_code, "extension_start_code"); 

				if (after == 0)
				{
/*  follows sequence_extension()  */


					if ( this.sequence_display_extension != null )
					{
						size += stream.WriteClass<SequenceDisplayExtension>(context, this.sequence_display_extension, "sequence_display_extension"); 
					}
					else if ( this.sequence_scalable_extension != null )
					{
						size += stream.WriteClass<SequenceScalableExtension>(context, this.sequence_scalable_extension, "sequence_scalable_extension"); 
					}
					else 
					{

						while ( whileIndex + 1 < this.ReservedExtensionDataByte.Count )
						{
							whileIndex++;

							size += stream.WriteUnsignedInt(8, whileIndex, (this.reserved_extension_data_byte ??= new()), "reserved_extension_data_byte"); 
						}
					}
				}
/*  NOTE – i never takes the value 1 because extension_data()
never follows a group_of_pictures_header()  */


				if (after == 2)
				{
/*  follows picture_coding_extension()  */


					if ( this.quant_matrix_extension != null )
					{
						size += stream.WriteClass<QuantMatrixExtension>(context, this.quant_matrix_extension, "quant_matrix_extension"); 
					}
					else if ( this.copyright_extension != null )
					{
						size += stream.WriteClass<CopyrightExtension>(context, this.copyright_extension, "copyright_extension"); 
					}
					else if ( this.picture_display_extension != null )
					{
						size += stream.WriteClass<PictureDisplayExtension>(context, this.picture_display_extension, "picture_display_extension"); 
					}
					else if ( this.picture_spatial_scalable_extension != null )
					{
						size += stream.WriteClass<PictureSpatialScalableExtension>(context, this.picture_spatial_scalable_extension, "picture_spatial_scalable_extension"); 
					}
					else if ( this.picture_temporal_scalable_extension != null )
					{
						size += stream.WriteClass<PictureTemporalScalableExtension>(context, this.picture_temporal_scalable_extension, "picture_temporal_scalable_extension"); 
					}
					else if ( this.camera_parameters_extension != null )
					{
						size += stream.WriteClass<CameraParametersExtension>(context, this.camera_parameters_extension, "camera_parameters_extension"); 
					}
					else if ( this.itu_t_extension != null )
					{
						size += stream.WriteClass<ItutExtension>(context, this.itu_t_extension, "itu_t_extension"); 
					}
					else 
					{

						while ( whileIndex + 1 < this.ReservedExtensionDataByte.Count )
						{
							whileIndex++;

							size += stream.WriteUnsignedInt(8, whileIndex, (this.reserved_extension_data_byte ??= new()), "reserved_extension_data_byte"); 
						}
					}
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
next_start_code()
}
    */
    public class UserData : IItuSerializable
    {
		private uint user_data_start_code;
		public uint UserDataStartCode { get { return user_data_start_code; } set { user_data_start_code = value; } }
		private Dictionary<int, uint> user_data;
		public Dictionary<int, uint> _UserData { get { return user_data ??= new Dictionary<int, uint>(); } set { user_data = value; } }
		private NextStartCode next_start_code;
		public NextStartCode NextStartCode { get { return next_start_code; } set { next_start_code = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public UserData()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");

            ulong size = 0;

			int whileIndex = -1;
			size += stream.ReadUnsignedInt(size, 32, out this.user_data_start_code, "user_data_start_code"); 

			while ( !ituContext.AtStartCode(stream) )
			{
				whileIndex++;

				size += stream.ReadUnsignedInt(size, 8, whileIndex, (this.user_data ??= new()), "user_data"); 
			}
			this.next_start_code =  new NextStartCode() ;
			size +=  stream.ReadClass<NextStartCode>(size, context, this.next_start_code, "next_start_code"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");
            ulong size = 0;

			int whileIndex = -1;
			size += stream.WriteUnsignedInt(32, this.user_data_start_code, "user_data_start_code"); 

			while ( whileIndex + 1 < this._UserData.Count )
			{
				whileIndex++;

				size += stream.WriteUnsignedInt(8, whileIndex, (this.user_data ??= new()), "user_data"); 
			}
			size += stream.WriteClass<NextStartCode>(context, this.next_start_code, "next_start_code"); 

            return size;
         }

    }

    /*


sequence_extension() {
extension_start_code u(32)
extension_start_code_identifier u(4)
profile_and_level_indication u(8)
progressive_sequence u(1)
chroma_format u(2)
horizontal_size_extension u(2)
vertical_size_extension u(2)
bit_rate_extension u(12)
marker_bit u(1)
vbv_buffer_size_extension u(8)
low_delay u(1)
frame_rate_extension_n u(2)
frame_rate_extension_d u(5)
next_start_code()
}
    */
    public class SequenceExtension : IItuSerializable
    {
		private uint extension_start_code;
		public uint ExtensionStartCode { get { return extension_start_code; } set { extension_start_code = value; } }
		private uint extension_start_code_identifier;
		public uint ExtensionStartCodeIdentifier { get { return extension_start_code_identifier; } set { extension_start_code_identifier = value; } }
		private uint profile_and_level_indication;
		public uint ProfileAndLevelIndication { get { return profile_and_level_indication; } set { profile_and_level_indication = value; } }
		private byte progressive_sequence;
		public byte ProgressiveSequence { get { return progressive_sequence; } set { progressive_sequence = value; } }
		private uint chroma_format;
		public uint ChromaFormat { get { return chroma_format; } set { chroma_format = value; } }
		private uint horizontal_size_extension;
		public uint HorizontalSizeExtension { get { return horizontal_size_extension; } set { horizontal_size_extension = value; } }
		private uint vertical_size_extension;
		public uint VerticalSizeExtension { get { return vertical_size_extension; } set { vertical_size_extension = value; } }
		private uint bit_rate_extension;
		public uint BitRateExtension { get { return bit_rate_extension; } set { bit_rate_extension = value; } }
		private byte marker_bit;
		public byte MarkerBit { get { return marker_bit; } set { marker_bit = value; } }
		private uint vbv_buffer_size_extension;
		public uint VbvBufferSizeExtension { get { return vbv_buffer_size_extension; } set { vbv_buffer_size_extension = value; } }
		private byte low_delay;
		public byte LowDelay { get { return low_delay; } set { low_delay = value; } }
		private uint frame_rate_extension_n;
		public uint FrameRateExtensionn { get { return frame_rate_extension_n; } set { frame_rate_extension_n = value; } }
		private uint frame_rate_extension_d;
		public uint FrameRateExtensiond { get { return frame_rate_extension_d; } set { frame_rate_extension_d = value; } }
		private NextStartCode next_start_code;
		public NextStartCode NextStartCode { get { return next_start_code; } set { next_start_code = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public SequenceExtension()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");

            ulong size = 0;

			size += stream.ReadUnsignedInt(size, 32, out this.extension_start_code, "extension_start_code"); 
			size += stream.ReadUnsignedInt(size, 4, out this.extension_start_code_identifier, "extension_start_code_identifier"); 
			size += stream.ReadUnsignedInt(size, 8, out this.profile_and_level_indication, "profile_and_level_indication"); 
			size += stream.ReadUnsignedInt(size, 1, out this.progressive_sequence, "progressive_sequence"); 
			size += stream.ReadUnsignedInt(size, 2, out this.chroma_format, "chroma_format"); 
			size += stream.ReadUnsignedInt(size, 2, out this.horizontal_size_extension, "horizontal_size_extension"); 
			size += stream.ReadUnsignedInt(size, 2, out this.vertical_size_extension, "vertical_size_extension"); 
			size += stream.ReadUnsignedInt(size, 12, out this.bit_rate_extension, "bit_rate_extension"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 8, out this.vbv_buffer_size_extension, "vbv_buffer_size_extension"); 
			size += stream.ReadUnsignedInt(size, 1, out this.low_delay, "low_delay"); 
			size += stream.ReadUnsignedInt(size, 2, out this.frame_rate_extension_n, "frame_rate_extension_n"); 
			size += stream.ReadUnsignedInt(size, 5, out this.frame_rate_extension_d, "frame_rate_extension_d"); 
			this.next_start_code =  new NextStartCode() ;
			size +=  stream.ReadClass<NextStartCode>(size, context, this.next_start_code, "next_start_code"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");
            ulong size = 0;

			size += stream.WriteUnsignedInt(32, this.extension_start_code, "extension_start_code"); 
			size += stream.WriteUnsignedInt(4, this.extension_start_code_identifier, "extension_start_code_identifier"); 
			size += stream.WriteUnsignedInt(8, this.profile_and_level_indication, "profile_and_level_indication"); 
			size += stream.WriteUnsignedInt(1, this.progressive_sequence, "progressive_sequence"); 
			size += stream.WriteUnsignedInt(2, this.chroma_format, "chroma_format"); 
			size += stream.WriteUnsignedInt(2, this.horizontal_size_extension, "horizontal_size_extension"); 
			size += stream.WriteUnsignedInt(2, this.vertical_size_extension, "vertical_size_extension"); 
			size += stream.WriteUnsignedInt(12, this.bit_rate_extension, "bit_rate_extension"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(8, this.vbv_buffer_size_extension, "vbv_buffer_size_extension"); 
			size += stream.WriteUnsignedInt(1, this.low_delay, "low_delay"); 
			size += stream.WriteUnsignedInt(2, this.frame_rate_extension_n, "frame_rate_extension_n"); 
			size += stream.WriteUnsignedInt(5, this.frame_rate_extension_d, "frame_rate_extension_d"); 
			size += stream.WriteClass<NextStartCode>(context, this.next_start_code, "next_start_code"); 

            return size;
         }

    }

    /*


sequence_display_extension() {
extension_start_code_identifier u(4)
video_format u(3)
colour_description u(1)
if ( colour_description ) {
colour_primaries u(8)
transfer_characteristics u(8)
matrix_coefficients u(8)
}
display_horizontal_size u(14)
marker_bit u(1)
display_vertical_size u(14)
next_start_code()
}
    */
    public class SequenceDisplayExtension : IItuSerializable
    {
		private uint extension_start_code_identifier;
		public uint ExtensionStartCodeIdentifier { get { return extension_start_code_identifier; } set { extension_start_code_identifier = value; } }
		private uint video_format;
		public uint VideoFormat { get { return video_format; } set { video_format = value; } }
		private byte colour_description;
		public byte ColourDescription { get { return colour_description; } set { colour_description = value; } }
		private uint colour_primaries;
		public uint ColourPrimaries { get { return colour_primaries; } set { colour_primaries = value; } }
		private uint transfer_characteristics;
		public uint TransferCharacteristics { get { return transfer_characteristics; } set { transfer_characteristics = value; } }
		private uint matrix_coefficients;
		public uint MatrixCoefficients { get { return matrix_coefficients; } set { matrix_coefficients = value; } }
		private uint display_horizontal_size;
		public uint DisplayHorizontalSize { get { return display_horizontal_size; } set { display_horizontal_size = value; } }
		private byte marker_bit;
		public byte MarkerBit { get { return marker_bit; } set { marker_bit = value; } }
		private uint display_vertical_size;
		public uint DisplayVerticalSize { get { return display_vertical_size; } set { display_vertical_size = value; } }
		private NextStartCode next_start_code;
		public NextStartCode NextStartCode { get { return next_start_code; } set { next_start_code = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public SequenceDisplayExtension()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");

            ulong size = 0;

			size += stream.ReadUnsignedInt(size, 4, out this.extension_start_code_identifier, "extension_start_code_identifier"); 
			size += stream.ReadUnsignedInt(size, 3, out this.video_format, "video_format"); 
			size += stream.ReadUnsignedInt(size, 1, out this.colour_description, "colour_description"); 

			if ( colour_description != 0 )
			{
				size += stream.ReadUnsignedInt(size, 8, out this.colour_primaries, "colour_primaries"); 
				size += stream.ReadUnsignedInt(size, 8, out this.transfer_characteristics, "transfer_characteristics"); 
				size += stream.ReadUnsignedInt(size, 8, out this.matrix_coefficients, "matrix_coefficients"); 
			}
			size += stream.ReadUnsignedInt(size, 14, out this.display_horizontal_size, "display_horizontal_size"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 14, out this.display_vertical_size, "display_vertical_size"); 
			this.next_start_code =  new NextStartCode() ;
			size +=  stream.ReadClass<NextStartCode>(size, context, this.next_start_code, "next_start_code"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");
            ulong size = 0;

			size += stream.WriteUnsignedInt(4, this.extension_start_code_identifier, "extension_start_code_identifier"); 
			size += stream.WriteUnsignedInt(3, this.video_format, "video_format"); 
			size += stream.WriteUnsignedInt(1, this.colour_description, "colour_description"); 

			if ( colour_description != 0 )
			{
				size += stream.WriteUnsignedInt(8, this.colour_primaries, "colour_primaries"); 
				size += stream.WriteUnsignedInt(8, this.transfer_characteristics, "transfer_characteristics"); 
				size += stream.WriteUnsignedInt(8, this.matrix_coefficients, "matrix_coefficients"); 
			}
			size += stream.WriteUnsignedInt(14, this.display_horizontal_size, "display_horizontal_size"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(14, this.display_vertical_size, "display_vertical_size"); 
			size += stream.WriteClass<NextStartCode>(context, this.next_start_code, "next_start_code"); 

            return size;
         }

    }

    /*


sequence_scalable_extension() {
extension_start_code_identifier u(4)
scalable_mode u(2)
layer_id u(4)
if (scalable_mode == 1) {
lower_layer_prediction_horizontal_size u(14)
marker_bit u(1)
lower_layer_prediction_vertical_size u(14)
horizontal_subsampling_factor_m u(5)
horizontal_subsampling_factor_n u(5)
vertical_subsampling_factor_m u(5)
vertical_subsampling_factor_n u(5)
}
if ( scalable_mode == 3 ) {
picture_mux_enable u(1)
if ( picture_mux_enable )
mux_to_progressive_sequence u(1)
picture_mux_order u(3)
picture_mux_factor u(3)
}
next_start_code()
}
    */
    public class SequenceScalableExtension : IItuSerializable
    {
		private uint extension_start_code_identifier;
		public uint ExtensionStartCodeIdentifier { get { return extension_start_code_identifier; } set { extension_start_code_identifier = value; } }
		private uint scalable_mode;
		public uint ScalableMode { get { return scalable_mode; } set { scalable_mode = value; } }
		private uint layer_id;
		public uint LayerId { get { return layer_id; } set { layer_id = value; } }
		private uint lower_layer_prediction_horizontal_size;
		public uint LowerLayerPredictionHorizontalSize { get { return lower_layer_prediction_horizontal_size; } set { lower_layer_prediction_horizontal_size = value; } }
		private byte marker_bit;
		public byte MarkerBit { get { return marker_bit; } set { marker_bit = value; } }
		private uint lower_layer_prediction_vertical_size;
		public uint LowerLayerPredictionVerticalSize { get { return lower_layer_prediction_vertical_size; } set { lower_layer_prediction_vertical_size = value; } }
		private uint horizontal_subsampling_factor_m;
		public uint HorizontalSubsamplingFactorm { get { return horizontal_subsampling_factor_m; } set { horizontal_subsampling_factor_m = value; } }
		private uint horizontal_subsampling_factor_n;
		public uint HorizontalSubsamplingFactorn { get { return horizontal_subsampling_factor_n; } set { horizontal_subsampling_factor_n = value; } }
		private uint vertical_subsampling_factor_m;
		public uint VerticalSubsamplingFactorm { get { return vertical_subsampling_factor_m; } set { vertical_subsampling_factor_m = value; } }
		private uint vertical_subsampling_factor_n;
		public uint VerticalSubsamplingFactorn { get { return vertical_subsampling_factor_n; } set { vertical_subsampling_factor_n = value; } }
		private byte picture_mux_enable;
		public byte PictureMuxEnable { get { return picture_mux_enable; } set { picture_mux_enable = value; } }
		private byte mux_to_progressive_sequence;
		public byte MuxToProgressiveSequence { get { return mux_to_progressive_sequence; } set { mux_to_progressive_sequence = value; } }
		private uint picture_mux_order;
		public uint PictureMuxOrder { get { return picture_mux_order; } set { picture_mux_order = value; } }
		private uint picture_mux_factor;
		public uint PictureMuxFactor { get { return picture_mux_factor; } set { picture_mux_factor = value; } }
		private NextStartCode next_start_code;
		public NextStartCode NextStartCode { get { return next_start_code; } set { next_start_code = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public SequenceScalableExtension()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");

            ulong size = 0;

			size += stream.ReadUnsignedInt(size, 4, out this.extension_start_code_identifier, "extension_start_code_identifier"); 
			size += stream.ReadUnsignedInt(size, 2, out this.scalable_mode, "scalable_mode"); 
			size += stream.ReadUnsignedInt(size, 4, out this.layer_id, "layer_id"); 

			if (scalable_mode == 1)
			{
				size += stream.ReadUnsignedInt(size, 14, out this.lower_layer_prediction_horizontal_size, "lower_layer_prediction_horizontal_size"); 
				size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
				size += stream.ReadUnsignedInt(size, 14, out this.lower_layer_prediction_vertical_size, "lower_layer_prediction_vertical_size"); 
				size += stream.ReadUnsignedInt(size, 5, out this.horizontal_subsampling_factor_m, "horizontal_subsampling_factor_m"); 
				size += stream.ReadUnsignedInt(size, 5, out this.horizontal_subsampling_factor_n, "horizontal_subsampling_factor_n"); 
				size += stream.ReadUnsignedInt(size, 5, out this.vertical_subsampling_factor_m, "vertical_subsampling_factor_m"); 
				size += stream.ReadUnsignedInt(size, 5, out this.vertical_subsampling_factor_n, "vertical_subsampling_factor_n"); 
			}

			if ( scalable_mode == 3 )
			{
				size += stream.ReadUnsignedInt(size, 1, out this.picture_mux_enable, "picture_mux_enable"); 

				if ( picture_mux_enable != 0 )
				{
					size += stream.ReadUnsignedInt(size, 1, out this.mux_to_progressive_sequence, "mux_to_progressive_sequence"); 
				}
				size += stream.ReadUnsignedInt(size, 3, out this.picture_mux_order, "picture_mux_order"); 
				size += stream.ReadUnsignedInt(size, 3, out this.picture_mux_factor, "picture_mux_factor"); 
			}
			this.next_start_code =  new NextStartCode() ;
			size +=  stream.ReadClass<NextStartCode>(size, context, this.next_start_code, "next_start_code"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");
            ulong size = 0;

			size += stream.WriteUnsignedInt(4, this.extension_start_code_identifier, "extension_start_code_identifier"); 
			size += stream.WriteUnsignedInt(2, this.scalable_mode, "scalable_mode"); 
			size += stream.WriteUnsignedInt(4, this.layer_id, "layer_id"); 

			if (scalable_mode == 1)
			{
				size += stream.WriteUnsignedInt(14, this.lower_layer_prediction_horizontal_size, "lower_layer_prediction_horizontal_size"); 
				size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
				size += stream.WriteUnsignedInt(14, this.lower_layer_prediction_vertical_size, "lower_layer_prediction_vertical_size"); 
				size += stream.WriteUnsignedInt(5, this.horizontal_subsampling_factor_m, "horizontal_subsampling_factor_m"); 
				size += stream.WriteUnsignedInt(5, this.horizontal_subsampling_factor_n, "horizontal_subsampling_factor_n"); 
				size += stream.WriteUnsignedInt(5, this.vertical_subsampling_factor_m, "vertical_subsampling_factor_m"); 
				size += stream.WriteUnsignedInt(5, this.vertical_subsampling_factor_n, "vertical_subsampling_factor_n"); 
			}

			if ( scalable_mode == 3 )
			{
				size += stream.WriteUnsignedInt(1, this.picture_mux_enable, "picture_mux_enable"); 

				if ( picture_mux_enable != 0 )
				{
					size += stream.WriteUnsignedInt(1, this.mux_to_progressive_sequence, "mux_to_progressive_sequence"); 
				}
				size += stream.WriteUnsignedInt(3, this.picture_mux_order, "picture_mux_order"); 
				size += stream.WriteUnsignedInt(3, this.picture_mux_factor, "picture_mux_factor"); 
			}
			size += stream.WriteClass<NextStartCode>(context, this.next_start_code, "next_start_code"); 

            return size;
         }

    }

    /*


group_of_pictures_header() {
group_start_code u(32)
time_code u(25)
closed_gop u(1)
broken_link u(1)
next_start_code()
}
    */
    public class GroupOfPicturesHeader : IItuSerializable
    {
		private uint group_start_code;
		public uint GroupStartCode { get { return group_start_code; } set { group_start_code = value; } }
		private uint time_code;
		public uint TimeCode { get { return time_code; } set { time_code = value; } }
		private byte closed_gop;
		public byte ClosedGop { get { return closed_gop; } set { closed_gop = value; } }
		private byte broken_link;
		public byte BrokenLink { get { return broken_link; } set { broken_link = value; } }
		private NextStartCode next_start_code;
		public NextStartCode NextStartCode { get { return next_start_code; } set { next_start_code = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public GroupOfPicturesHeader()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");

            ulong size = 0;

			size += stream.ReadUnsignedInt(size, 32, out this.group_start_code, "group_start_code"); 
			size += stream.ReadUnsignedInt(size, 25, out this.time_code, "time_code"); 
			size += stream.ReadUnsignedInt(size, 1, out this.closed_gop, "closed_gop"); 
			size += stream.ReadUnsignedInt(size, 1, out this.broken_link, "broken_link"); 
			this.next_start_code =  new NextStartCode() ;
			size +=  stream.ReadClass<NextStartCode>(size, context, this.next_start_code, "next_start_code"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");
            ulong size = 0;

			size += stream.WriteUnsignedInt(32, this.group_start_code, "group_start_code"); 
			size += stream.WriteUnsignedInt(25, this.time_code, "time_code"); 
			size += stream.WriteUnsignedInt(1, this.closed_gop, "closed_gop"); 
			size += stream.WriteUnsignedInt(1, this.broken_link, "broken_link"); 
			size += stream.WriteClass<NextStartCode>(context, this.next_start_code, "next_start_code"); 

            return size;
         }

    }

    /*


picture_header() {
picture_start_code u(32)
temporal_reference u(10)
picture_coding_type u(3)
vbv_delay u(16)
if ( picture_coding_type == 2 || picture_coding_type == 3) {
full_pel_forward_vector u(1)
forward_f_code u(3)
}
if ( picture_coding_type == 3 ) {
full_pel_backward_vector u(1)
backward_f_code u(3)
}
while ( next_bit_one( extra_bit_picture ) ) {
extra_bit_picture /* with the value '1' *//* u(1)
content_description_data() /* with every 9th bit having the value '1' *//*
}
last_extra_bit_picture /* with the value '0' *//* u(1)
next_start_code()
}
    */
    public class PictureHeader : IItuSerializable
    {
		private uint picture_start_code;
		public uint PictureStartCode { get { return picture_start_code; } set { picture_start_code = value; } }
		private uint temporal_reference;
		public uint TemporalReference { get { return temporal_reference; } set { temporal_reference = value; } }
		private uint picture_coding_type;
		public uint PictureCodingType { get { return picture_coding_type; } set { picture_coding_type = value; } }
		private uint vbv_delay;
		public uint VbvDelay { get { return vbv_delay; } set { vbv_delay = value; } }
		private byte full_pel_forward_vector;
		public byte FullPelForwardVector { get { return full_pel_forward_vector; } set { full_pel_forward_vector = value; } }
		private uint forward_f_code;
		public uint ForwardfCode { get { return forward_f_code; } set { forward_f_code = value; } }
		private byte full_pel_backward_vector;
		public byte FullPelBackwardVector { get { return full_pel_backward_vector; } set { full_pel_backward_vector = value; } }
		private uint backward_f_code;
		public uint BackwardfCode { get { return backward_f_code; } set { backward_f_code = value; } }
		private Dictionary<int, byte> extra_bit_picture;
		public Dictionary<int, byte> ExtraBitPicture { get { return extra_bit_picture ??= new Dictionary<int, byte>(); } set { extra_bit_picture = value; } }
		private Dictionary<int, ContentDescriptionData> content_description_data;
		public Dictionary<int, ContentDescriptionData> ContentDescriptionData { get { return content_description_data ??= new Dictionary<int, ContentDescriptionData>(); } set { content_description_data = value; } }
		private byte last_extra_bit_picture;
		public byte LastExtraBitPicture { get { return last_extra_bit_picture; } set { last_extra_bit_picture = value; } }
		private NextStartCode next_start_code;
		public NextStartCode NextStartCode { get { return next_start_code; } set { next_start_code = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public PictureHeader()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");

            ulong size = 0;

			int whileIndex = -1;
			size += stream.ReadUnsignedInt(size, 32, out this.picture_start_code, "picture_start_code"); 
			size += stream.ReadUnsignedInt(size, 10, out this.temporal_reference, "temporal_reference"); 
			size += stream.ReadUnsignedInt(size, 3, out this.picture_coding_type, "picture_coding_type"); 
			size += stream.ReadUnsignedInt(size, 16, out this.vbv_delay, "vbv_delay"); 

			if ( picture_coding_type == 2 || picture_coding_type == 3)
			{
				size += stream.ReadUnsignedInt(size, 1, out this.full_pel_forward_vector, "full_pel_forward_vector"); 
				size += stream.ReadUnsignedInt(size, 3, out this.forward_f_code, "forward_f_code"); 
			}

			if ( picture_coding_type == 3 )
			{
				size += stream.ReadUnsignedInt(size, 1, out this.full_pel_backward_vector, "full_pel_backward_vector"); 
				size += stream.ReadUnsignedInt(size, 3, out this.backward_f_code, "backward_f_code"); 
			}

			while ( ituContext.NextBits(stream, 1) == 1 )
			{
				whileIndex++;

				size += stream.ReadUnsignedInt(size, 1, whileIndex, (this.extra_bit_picture ??= new()), "extra_bit_picture"); // with the value '1' 
				(this.content_description_data ??= new()).Add(whileIndex,  new ContentDescriptionData() );
				size +=  stream.ReadClass<ContentDescriptionData>(size, context, this.content_description_data[whileIndex], "content_description_data"); // with every 9th bit having the value '1' 
			}
			size += stream.ReadUnsignedInt(size, 1, out this.last_extra_bit_picture, "last_extra_bit_picture"); // with the value '0' 
			this.next_start_code =  new NextStartCode() ;
			size +=  stream.ReadClass<NextStartCode>(size, context, this.next_start_code, "next_start_code"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");
            ulong size = 0;

			int whileIndex = -1;
			size += stream.WriteUnsignedInt(32, this.picture_start_code, "picture_start_code"); 
			size += stream.WriteUnsignedInt(10, this.temporal_reference, "temporal_reference"); 
			size += stream.WriteUnsignedInt(3, this.picture_coding_type, "picture_coding_type"); 
			size += stream.WriteUnsignedInt(16, this.vbv_delay, "vbv_delay"); 

			if ( picture_coding_type == 2 || picture_coding_type == 3)
			{
				size += stream.WriteUnsignedInt(1, this.full_pel_forward_vector, "full_pel_forward_vector"); 
				size += stream.WriteUnsignedInt(3, this.forward_f_code, "forward_f_code"); 
			}

			if ( picture_coding_type == 3 )
			{
				size += stream.WriteUnsignedInt(1, this.full_pel_backward_vector, "full_pel_backward_vector"); 
				size += stream.WriteUnsignedInt(3, this.backward_f_code, "backward_f_code"); 
			}

			while ( whileIndex + 1 < this.ExtraBitPicture.Count )
			{
				whileIndex++;

				size += stream.WriteUnsignedInt(1, whileIndex, (this.extra_bit_picture ??= new()), "extra_bit_picture"); // with the value '1' 
				size += stream.WriteClass<ContentDescriptionData>(context, whileIndex, (this.content_description_data ??= new()), "content_description_data"); // with every 9th bit having the value '1' 
			}
			size += stream.WriteUnsignedInt(1, this.last_extra_bit_picture, "last_extra_bit_picture"); // with the value '0' 
			size += stream.WriteClass<NextStartCode>(context, this.next_start_code, "next_start_code"); 

            return size;
         }

    }

    /*


picture_coding_extension() {
extension_start_code u(32)
extension_start_code_identifier u(4)
f_code[0][0] /* forward horizontal *//* u(4)
f_code[0][1] /* forward vertical *//* u(4)
f_code[1][0] /* backward horizontal *//* u(4)
f_code[1][1] /* backward vertical *//* u(4)
intra_dc_precision u(2)
picture_structure u(2)
top_field_first u(1)
frame_pred_frame_dct u(1)
concealment_motion_vectors u(1)
q_scale_type u(1)
intra_vlc_format u(1)
alternate_scan u(1)
repeat_first_field u(1)
chroma_420_type u(1)
progressive_frame u(1)
composite_display_flag u(1)
if ( composite_display_flag ) {
v_axis u(1)
field_sequence u(3)
sub_carrier u(1)
burst_amplitude u(7)
sub_carrier_phase u(8)
}
next_start_code()
}
    */
    public class PictureCodingExtension : IItuSerializable
    {
		private uint extension_start_code;
		public uint ExtensionStartCode { get { return extension_start_code; } set { extension_start_code = value; } }
		private uint extension_start_code_identifier;
		public uint ExtensionStartCodeIdentifier { get { return extension_start_code_identifier; } set { extension_start_code_identifier = value; } }
		private uint[][] f_code= new uint[][] { new uint[2], new uint[2] };
		public uint[][] fCode { get { return f_code; } set { f_code = value; } }
		private uint intra_dc_precision;
		public uint IntraDcPrecision { get { return intra_dc_precision; } set { intra_dc_precision = value; } }
		private uint picture_structure;
		public uint PictureStructure { get { return picture_structure; } set { picture_structure = value; } }
		private byte top_field_first;
		public byte TopFieldFirst { get { return top_field_first; } set { top_field_first = value; } }
		private byte frame_pred_frame_dct;
		public byte FramePredFrameDct { get { return frame_pred_frame_dct; } set { frame_pred_frame_dct = value; } }
		private byte concealment_motion_vectors;
		public byte ConcealmentMotionVectors { get { return concealment_motion_vectors; } set { concealment_motion_vectors = value; } }
		private byte q_scale_type;
		public byte qScaleType { get { return q_scale_type; } set { q_scale_type = value; } }
		private byte intra_vlc_format;
		public byte IntraVlcFormat { get { return intra_vlc_format; } set { intra_vlc_format = value; } }
		private byte alternate_scan;
		public byte AlternateScan { get { return alternate_scan; } set { alternate_scan = value; } }
		private byte repeat_first_field;
		public byte RepeatFirstField { get { return repeat_first_field; } set { repeat_first_field = value; } }
		private byte chroma_420_type;
		public byte Chroma420Type { get { return chroma_420_type; } set { chroma_420_type = value; } }
		private byte progressive_frame;
		public byte ProgressiveFrame { get { return progressive_frame; } set { progressive_frame = value; } }
		private byte composite_display_flag;
		public byte CompositeDisplayFlag { get { return composite_display_flag; } set { composite_display_flag = value; } }
		private byte v_axis;
		public byte vAxis { get { return v_axis; } set { v_axis = value; } }
		private uint field_sequence;
		public uint FieldSequence { get { return field_sequence; } set { field_sequence = value; } }
		private byte sub_carrier;
		public byte SubCarrier { get { return sub_carrier; } set { sub_carrier = value; } }
		private uint burst_amplitude;
		public uint BurstAmplitude { get { return burst_amplitude; } set { burst_amplitude = value; } }
		private uint sub_carrier_phase;
		public uint SubCarrierPhase { get { return sub_carrier_phase; } set { sub_carrier_phase = value; } }
		private NextStartCode next_start_code;
		public NextStartCode NextStartCode { get { return next_start_code; } set { next_start_code = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public PictureCodingExtension()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");

            ulong size = 0;

			size += stream.ReadUnsignedInt(size, 32, out this.extension_start_code, "extension_start_code"); 
			size += stream.ReadUnsignedInt(size, 4, out this.extension_start_code_identifier, "extension_start_code_identifier"); 
			size += stream.ReadUnsignedInt(size, 4, out this.f_code[0][0], "f_code"); // forward horizontal 
			size += stream.ReadUnsignedInt(size, 4, out this.f_code[0][1], "f_code"); // forward vertical 
			size += stream.ReadUnsignedInt(size, 4, out this.f_code[1][0], "f_code"); // backward horizontal 
			size += stream.ReadUnsignedInt(size, 4, out this.f_code[1][1], "f_code"); // backward vertical 
			size += stream.ReadUnsignedInt(size, 2, out this.intra_dc_precision, "intra_dc_precision"); 
			size += stream.ReadUnsignedInt(size, 2, out this.picture_structure, "picture_structure"); 
			size += stream.ReadUnsignedInt(size, 1, out this.top_field_first, "top_field_first"); 
			size += stream.ReadUnsignedInt(size, 1, out this.frame_pred_frame_dct, "frame_pred_frame_dct"); 
			size += stream.ReadUnsignedInt(size, 1, out this.concealment_motion_vectors, "concealment_motion_vectors"); 
			size += stream.ReadUnsignedInt(size, 1, out this.q_scale_type, "q_scale_type"); 
			size += stream.ReadUnsignedInt(size, 1, out this.intra_vlc_format, "intra_vlc_format"); 
			size += stream.ReadUnsignedInt(size, 1, out this.alternate_scan, "alternate_scan"); 
			size += stream.ReadUnsignedInt(size, 1, out this.repeat_first_field, "repeat_first_field"); 
			size += stream.ReadUnsignedInt(size, 1, out this.chroma_420_type, "chroma_420_type"); 
			size += stream.ReadUnsignedInt(size, 1, out this.progressive_frame, "progressive_frame"); 
			size += stream.ReadUnsignedInt(size, 1, out this.composite_display_flag, "composite_display_flag"); 

			if ( composite_display_flag != 0 )
			{
				size += stream.ReadUnsignedInt(size, 1, out this.v_axis, "v_axis"); 
				size += stream.ReadUnsignedInt(size, 3, out this.field_sequence, "field_sequence"); 
				size += stream.ReadUnsignedInt(size, 1, out this.sub_carrier, "sub_carrier"); 
				size += stream.ReadUnsignedInt(size, 7, out this.burst_amplitude, "burst_amplitude"); 
				size += stream.ReadUnsignedInt(size, 8, out this.sub_carrier_phase, "sub_carrier_phase"); 
			}
			this.next_start_code =  new NextStartCode() ;
			size +=  stream.ReadClass<NextStartCode>(size, context, this.next_start_code, "next_start_code"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");
            ulong size = 0;

			size += stream.WriteUnsignedInt(32, this.extension_start_code, "extension_start_code"); 
			size += stream.WriteUnsignedInt(4, this.extension_start_code_identifier, "extension_start_code_identifier"); 
			size += stream.WriteUnsignedInt(4, this.f_code[0][0], "f_code"); // forward horizontal 
			size += stream.WriteUnsignedInt(4, this.f_code[0][1], "f_code"); // forward vertical 
			size += stream.WriteUnsignedInt(4, this.f_code[1][0], "f_code"); // backward horizontal 
			size += stream.WriteUnsignedInt(4, this.f_code[1][1], "f_code"); // backward vertical 
			size += stream.WriteUnsignedInt(2, this.intra_dc_precision, "intra_dc_precision"); 
			size += stream.WriteUnsignedInt(2, this.picture_structure, "picture_structure"); 
			size += stream.WriteUnsignedInt(1, this.top_field_first, "top_field_first"); 
			size += stream.WriteUnsignedInt(1, this.frame_pred_frame_dct, "frame_pred_frame_dct"); 
			size += stream.WriteUnsignedInt(1, this.concealment_motion_vectors, "concealment_motion_vectors"); 
			size += stream.WriteUnsignedInt(1, this.q_scale_type, "q_scale_type"); 
			size += stream.WriteUnsignedInt(1, this.intra_vlc_format, "intra_vlc_format"); 
			size += stream.WriteUnsignedInt(1, this.alternate_scan, "alternate_scan"); 
			size += stream.WriteUnsignedInt(1, this.repeat_first_field, "repeat_first_field"); 
			size += stream.WriteUnsignedInt(1, this.chroma_420_type, "chroma_420_type"); 
			size += stream.WriteUnsignedInt(1, this.progressive_frame, "progressive_frame"); 
			size += stream.WriteUnsignedInt(1, this.composite_display_flag, "composite_display_flag"); 

			if ( composite_display_flag != 0 )
			{
				size += stream.WriteUnsignedInt(1, this.v_axis, "v_axis"); 
				size += stream.WriteUnsignedInt(3, this.field_sequence, "field_sequence"); 
				size += stream.WriteUnsignedInt(1, this.sub_carrier, "sub_carrier"); 
				size += stream.WriteUnsignedInt(7, this.burst_amplitude, "burst_amplitude"); 
				size += stream.WriteUnsignedInt(8, this.sub_carrier_phase, "sub_carrier_phase"); 
			}
			size += stream.WriteClass<NextStartCode>(context, this.next_start_code, "next_start_code"); 

            return size;
         }

    }

    /*


quant_matrix_extension() {
extension_start_code_identifier u(4)
load_intra_quantiser_matrix u(1)
if ( load_intra_quantiser_matrix )
for ( i = 0; i < 64; i++ )
intra_quantiser_matrix[ i ] u(8)
load_non_intra_quantiser_matrix u(1)
if ( load_non_intra_quantiser_matrix )
for ( i = 0; i < 64; i++ )
non_intra_quantiser_matrix[ i ] u(8)
load_chroma_intra_quantiser_matrix u(1)
if ( load_chroma_intra_quantiser_matrix )
for ( i = 0; i < 64; i++ )
chroma_intra_quantiser_matrix[ i ] u(8)
load_chroma_non_intra_quantiser_matrix u(1)
if ( load_chroma_non_intra_quantiser_matrix )
for ( i = 0; i < 64; i++ )
chroma_non_intra_quantiser_matrix[ i ] u(8)
next_start_code()
}
    */
    public class QuantMatrixExtension : IItuSerializable
    {
		private uint extension_start_code_identifier;
		public uint ExtensionStartCodeIdentifier { get { return extension_start_code_identifier; } set { extension_start_code_identifier = value; } }
		private byte load_intra_quantiser_matrix;
		public byte LoadIntraQuantiserMatrix { get { return load_intra_quantiser_matrix; } set { load_intra_quantiser_matrix = value; } }
		private uint[] intra_quantiser_matrix;
		public uint[] IntraQuantiserMatrix { get { return intra_quantiser_matrix; } set { intra_quantiser_matrix = value; } }
		private byte load_non_intra_quantiser_matrix;
		public byte LoadNonIntraQuantiserMatrix { get { return load_non_intra_quantiser_matrix; } set { load_non_intra_quantiser_matrix = value; } }
		private uint[] non_intra_quantiser_matrix;
		public uint[] NonIntraQuantiserMatrix { get { return non_intra_quantiser_matrix; } set { non_intra_quantiser_matrix = value; } }
		private byte load_chroma_intra_quantiser_matrix;
		public byte LoadChromaIntraQuantiserMatrix { get { return load_chroma_intra_quantiser_matrix; } set { load_chroma_intra_quantiser_matrix = value; } }
		private uint[] chroma_intra_quantiser_matrix;
		public uint[] ChromaIntraQuantiserMatrix { get { return chroma_intra_quantiser_matrix; } set { chroma_intra_quantiser_matrix = value; } }
		private byte load_chroma_non_intra_quantiser_matrix;
		public byte LoadChromaNonIntraQuantiserMatrix { get { return load_chroma_non_intra_quantiser_matrix; } set { load_chroma_non_intra_quantiser_matrix = value; } }
		private uint[] chroma_non_intra_quantiser_matrix;
		public uint[] ChromaNonIntraQuantiserMatrix { get { return chroma_non_intra_quantiser_matrix; } set { chroma_non_intra_quantiser_matrix = value; } }
		private NextStartCode next_start_code;
		public NextStartCode NextStartCode { get { return next_start_code; } set { next_start_code = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public QuantMatrixExtension()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");

            ulong size = 0;

			uint i = 0;
			size += stream.ReadUnsignedInt(size, 4, out this.extension_start_code_identifier, "extension_start_code_identifier"); 
			size += stream.ReadUnsignedInt(size, 1, out this.load_intra_quantiser_matrix, "load_intra_quantiser_matrix"); 

			if ( load_intra_quantiser_matrix != 0 )
			{

				stream.CheckArrayAllocation((ulong)( 64), "intra_quantiser_matrix");
				this.intra_quantiser_matrix = new uint[ 64];
				for ( i = 0; i < 64; i++ )
				{
					size += stream.ReadUnsignedInt(size, 8, out this.intra_quantiser_matrix[ i ], "intra_quantiser_matrix"); 
				}
			}
			size += stream.ReadUnsignedInt(size, 1, out this.load_non_intra_quantiser_matrix, "load_non_intra_quantiser_matrix"); 

			if ( load_non_intra_quantiser_matrix != 0 )
			{

				stream.CheckArrayAllocation((ulong)( 64), "non_intra_quantiser_matrix");
				this.non_intra_quantiser_matrix = new uint[ 64];
				for ( i = 0; i < 64; i++ )
				{
					size += stream.ReadUnsignedInt(size, 8, out this.non_intra_quantiser_matrix[ i ], "non_intra_quantiser_matrix"); 
				}
			}
			size += stream.ReadUnsignedInt(size, 1, out this.load_chroma_intra_quantiser_matrix, "load_chroma_intra_quantiser_matrix"); 

			if ( load_chroma_intra_quantiser_matrix != 0 )
			{

				stream.CheckArrayAllocation((ulong)( 64), "chroma_intra_quantiser_matrix");
				this.chroma_intra_quantiser_matrix = new uint[ 64];
				for ( i = 0; i < 64; i++ )
				{
					size += stream.ReadUnsignedInt(size, 8, out this.chroma_intra_quantiser_matrix[ i ], "chroma_intra_quantiser_matrix"); 
				}
			}
			size += stream.ReadUnsignedInt(size, 1, out this.load_chroma_non_intra_quantiser_matrix, "load_chroma_non_intra_quantiser_matrix"); 

			if ( load_chroma_non_intra_quantiser_matrix != 0 )
			{

				stream.CheckArrayAllocation((ulong)( 64), "chroma_non_intra_quantiser_matrix");
				this.chroma_non_intra_quantiser_matrix = new uint[ 64];
				for ( i = 0; i < 64; i++ )
				{
					size += stream.ReadUnsignedInt(size, 8, out this.chroma_non_intra_quantiser_matrix[ i ], "chroma_non_intra_quantiser_matrix"); 
				}
			}
			this.next_start_code =  new NextStartCode() ;
			size +=  stream.ReadClass<NextStartCode>(size, context, this.next_start_code, "next_start_code"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");
            ulong size = 0;

			uint i = 0;
			size += stream.WriteUnsignedInt(4, this.extension_start_code_identifier, "extension_start_code_identifier"); 
			size += stream.WriteUnsignedInt(1, this.load_intra_quantiser_matrix, "load_intra_quantiser_matrix"); 

			if ( load_intra_quantiser_matrix != 0 )
			{

				for ( i = 0; i < 64; i++ )
				{
					size += stream.WriteUnsignedInt(8, this.intra_quantiser_matrix[ i ], "intra_quantiser_matrix"); 
				}
			}
			size += stream.WriteUnsignedInt(1, this.load_non_intra_quantiser_matrix, "load_non_intra_quantiser_matrix"); 

			if ( load_non_intra_quantiser_matrix != 0 )
			{

				for ( i = 0; i < 64; i++ )
				{
					size += stream.WriteUnsignedInt(8, this.non_intra_quantiser_matrix[ i ], "non_intra_quantiser_matrix"); 
				}
			}
			size += stream.WriteUnsignedInt(1, this.load_chroma_intra_quantiser_matrix, "load_chroma_intra_quantiser_matrix"); 

			if ( load_chroma_intra_quantiser_matrix != 0 )
			{

				for ( i = 0; i < 64; i++ )
				{
					size += stream.WriteUnsignedInt(8, this.chroma_intra_quantiser_matrix[ i ], "chroma_intra_quantiser_matrix"); 
				}
			}
			size += stream.WriteUnsignedInt(1, this.load_chroma_non_intra_quantiser_matrix, "load_chroma_non_intra_quantiser_matrix"); 

			if ( load_chroma_non_intra_quantiser_matrix != 0 )
			{

				for ( i = 0; i < 64; i++ )
				{
					size += stream.WriteUnsignedInt(8, this.chroma_non_intra_quantiser_matrix[ i ], "chroma_non_intra_quantiser_matrix"); 
				}
			}
			size += stream.WriteClass<NextStartCode>(context, this.next_start_code, "next_start_code"); 

            return size;
         }

    }

    /*


picture_display_extension() {
extension_start_code_identifier u(4)
for ( i = 0; i < number_of_frame_centre_offsets; i ++ ) {
frame_centre_horizontal_offset i(16)
loop_marker_bit u(1)
frame_centre_vertical_offset i(16)
loop_marker_bit u(1)
}
next_start_code()
}
    */
    public class PictureDisplayExtension : IItuSerializable
    {
		private uint extension_start_code_identifier;
		public uint ExtensionStartCodeIdentifier { get { return extension_start_code_identifier; } set { extension_start_code_identifier = value; } }
		private int[] frame_centre_horizontal_offset;
		public int[] FrameCentreHorizontalOffset { get { return frame_centre_horizontal_offset; } set { frame_centre_horizontal_offset = value; } }
		private byte[] loop_marker_bit;
		public byte[] LoopMarkerBit { get { return loop_marker_bit; } set { loop_marker_bit = value; } }
		private int[] frame_centre_vertical_offset;
		public int[] FrameCentreVerticalOffset { get { return frame_centre_vertical_offset; } set { frame_centre_vertical_offset = value; } }
		private NextStartCode next_start_code;
		public NextStartCode NextStartCode { get { return next_start_code; } set { next_start_code = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public PictureDisplayExtension()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");

            ulong size = 0;

			uint i = 0;
			size += stream.ReadUnsignedInt(size, 4, out this.extension_start_code_identifier, "extension_start_code_identifier"); 

			stream.CheckArrayAllocation((ulong)( ituContext.NumberOfFrameCentreOffsets), "frame_centre_horizontal_offset");
			this.frame_centre_horizontal_offset = new int[ ituContext.NumberOfFrameCentreOffsets];
			stream.CheckArrayAllocation((ulong)( ituContext.NumberOfFrameCentreOffsets), "loop_marker_bit");
			this.loop_marker_bit = new byte[ ituContext.NumberOfFrameCentreOffsets];
			stream.CheckArrayAllocation((ulong)( ituContext.NumberOfFrameCentreOffsets), "frame_centre_vertical_offset");
			this.frame_centre_vertical_offset = new int[ ituContext.NumberOfFrameCentreOffsets];
			for ( i = 0; i < ituContext.NumberOfFrameCentreOffsets; i ++ )
			{
				size += stream.ReadSignedInt(size, 16, out this.frame_centre_horizontal_offset[ i ], "frame_centre_horizontal_offset"); 
				size += stream.ReadUnsignedInt(size, 1, out this.loop_marker_bit[ i ], "loop_marker_bit"); 
				size += stream.ReadSignedInt(size, 16, out this.frame_centre_vertical_offset[ i ], "frame_centre_vertical_offset"); 
				size += stream.ReadUnsignedInt(size, 1, out this.loop_marker_bit[ i ], "loop_marker_bit"); 
			}
			this.next_start_code =  new NextStartCode() ;
			size +=  stream.ReadClass<NextStartCode>(size, context, this.next_start_code, "next_start_code"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");
            ulong size = 0;

			uint i = 0;
			size += stream.WriteUnsignedInt(4, this.extension_start_code_identifier, "extension_start_code_identifier"); 

			for ( i = 0; i < ituContext.NumberOfFrameCentreOffsets; i ++ )
			{
				size += stream.WriteSignedInt(16,  this.frame_centre_horizontal_offset[ i ], "frame_centre_horizontal_offset"); 
				size += stream.WriteUnsignedInt(1, this.loop_marker_bit[ i ], "loop_marker_bit"); 
				size += stream.WriteSignedInt(16,  this.frame_centre_vertical_offset[ i ], "frame_centre_vertical_offset"); 
				size += stream.WriteUnsignedInt(1, this.loop_marker_bit[ i ], "loop_marker_bit"); 
			}
			size += stream.WriteClass<NextStartCode>(context, this.next_start_code, "next_start_code"); 

            return size;
         }

    }

    /*


picture_temporal_scalable_extension() {
extension_start_code_identifier u(4)
reference_select_code u(2)
forward_temporal_reference u(10)
marker_bit u(1)
backward_temporal_reference u(10)
next_start_code()
}
    */
    public class PictureTemporalScalableExtension : IItuSerializable
    {
		private uint extension_start_code_identifier;
		public uint ExtensionStartCodeIdentifier { get { return extension_start_code_identifier; } set { extension_start_code_identifier = value; } }
		private uint reference_select_code;
		public uint ReferenceSelectCode { get { return reference_select_code; } set { reference_select_code = value; } }
		private uint forward_temporal_reference;
		public uint ForwardTemporalReference { get { return forward_temporal_reference; } set { forward_temporal_reference = value; } }
		private byte marker_bit;
		public byte MarkerBit { get { return marker_bit; } set { marker_bit = value; } }
		private uint backward_temporal_reference;
		public uint BackwardTemporalReference { get { return backward_temporal_reference; } set { backward_temporal_reference = value; } }
		private NextStartCode next_start_code;
		public NextStartCode NextStartCode { get { return next_start_code; } set { next_start_code = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public PictureTemporalScalableExtension()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");

            ulong size = 0;

			size += stream.ReadUnsignedInt(size, 4, out this.extension_start_code_identifier, "extension_start_code_identifier"); 
			size += stream.ReadUnsignedInt(size, 2, out this.reference_select_code, "reference_select_code"); 
			size += stream.ReadUnsignedInt(size, 10, out this.forward_temporal_reference, "forward_temporal_reference"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 10, out this.backward_temporal_reference, "backward_temporal_reference"); 
			this.next_start_code =  new NextStartCode() ;
			size +=  stream.ReadClass<NextStartCode>(size, context, this.next_start_code, "next_start_code"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");
            ulong size = 0;

			size += stream.WriteUnsignedInt(4, this.extension_start_code_identifier, "extension_start_code_identifier"); 
			size += stream.WriteUnsignedInt(2, this.reference_select_code, "reference_select_code"); 
			size += stream.WriteUnsignedInt(10, this.forward_temporal_reference, "forward_temporal_reference"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(10, this.backward_temporal_reference, "backward_temporal_reference"); 
			size += stream.WriteClass<NextStartCode>(context, this.next_start_code, "next_start_code"); 

            return size;
         }

    }

    /*


picture_spatial_scalable_extension() {
extension_start_code_identifier u(4)
lower_layer_temporal_reference u(10)
marker_bit u(1)
lower_layer_horizontal_offset i(15)
marker_bit u(1)
lower_layer_vertical_offset i(15)
spatial_temporal_weight_code_table_index u(2)
lower_layer_progressive_frame u(1)
lower_layer_deinterlaced_field_select u(1)
next_start_code()
}
    */
    public class PictureSpatialScalableExtension : IItuSerializable
    {
		private uint extension_start_code_identifier;
		public uint ExtensionStartCodeIdentifier { get { return extension_start_code_identifier; } set { extension_start_code_identifier = value; } }
		private uint lower_layer_temporal_reference;
		public uint LowerLayerTemporalReference { get { return lower_layer_temporal_reference; } set { lower_layer_temporal_reference = value; } }
		private byte marker_bit;
		public byte MarkerBit { get { return marker_bit; } set { marker_bit = value; } }
		private int lower_layer_horizontal_offset;
		public int LowerLayerHorizontalOffset { get { return lower_layer_horizontal_offset; } set { lower_layer_horizontal_offset = value; } }
		private int lower_layer_vertical_offset;
		public int LowerLayerVerticalOffset { get { return lower_layer_vertical_offset; } set { lower_layer_vertical_offset = value; } }
		private uint spatial_temporal_weight_code_table_index;
		public uint SpatialTemporalWeightCodeTableIndex { get { return spatial_temporal_weight_code_table_index; } set { spatial_temporal_weight_code_table_index = value; } }
		private byte lower_layer_progressive_frame;
		public byte LowerLayerProgressiveFrame { get { return lower_layer_progressive_frame; } set { lower_layer_progressive_frame = value; } }
		private byte lower_layer_deinterlaced_field_select;
		public byte LowerLayerDeinterlacedFieldSelect { get { return lower_layer_deinterlaced_field_select; } set { lower_layer_deinterlaced_field_select = value; } }
		private NextStartCode next_start_code;
		public NextStartCode NextStartCode { get { return next_start_code; } set { next_start_code = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public PictureSpatialScalableExtension()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");

            ulong size = 0;

			size += stream.ReadUnsignedInt(size, 4, out this.extension_start_code_identifier, "extension_start_code_identifier"); 
			size += stream.ReadUnsignedInt(size, 10, out this.lower_layer_temporal_reference, "lower_layer_temporal_reference"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadSignedInt(size, 15, out this.lower_layer_horizontal_offset, "lower_layer_horizontal_offset"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadSignedInt(size, 15, out this.lower_layer_vertical_offset, "lower_layer_vertical_offset"); 
			size += stream.ReadUnsignedInt(size, 2, out this.spatial_temporal_weight_code_table_index, "spatial_temporal_weight_code_table_index"); 
			size += stream.ReadUnsignedInt(size, 1, out this.lower_layer_progressive_frame, "lower_layer_progressive_frame"); 
			size += stream.ReadUnsignedInt(size, 1, out this.lower_layer_deinterlaced_field_select, "lower_layer_deinterlaced_field_select"); 
			this.next_start_code =  new NextStartCode() ;
			size +=  stream.ReadClass<NextStartCode>(size, context, this.next_start_code, "next_start_code"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");
            ulong size = 0;

			size += stream.WriteUnsignedInt(4, this.extension_start_code_identifier, "extension_start_code_identifier"); 
			size += stream.WriteUnsignedInt(10, this.lower_layer_temporal_reference, "lower_layer_temporal_reference"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteSignedInt(15,  this.lower_layer_horizontal_offset, "lower_layer_horizontal_offset"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteSignedInt(15,  this.lower_layer_vertical_offset, "lower_layer_vertical_offset"); 
			size += stream.WriteUnsignedInt(2, this.spatial_temporal_weight_code_table_index, "spatial_temporal_weight_code_table_index"); 
			size += stream.WriteUnsignedInt(1, this.lower_layer_progressive_frame, "lower_layer_progressive_frame"); 
			size += stream.WriteUnsignedInt(1, this.lower_layer_deinterlaced_field_select, "lower_layer_deinterlaced_field_select"); 
			size += stream.WriteClass<NextStartCode>(context, this.next_start_code, "next_start_code"); 

            return size;
         }

    }

    /*


copyright_extension() {
extension_start_code_identifier u(4)
copyright_flag u(1)
copyright_identifier u(8)
original_or_copy u(1)
reserved u(7)
marker_bit u(1)
copyright_number_1 u(20)
marker_bit u(1)
copyright_number_2 u(22)
marker_bit u(1)
copyright_number_3 u(22)
next_start_code()
}
    */
    public class CopyrightExtension : IItuSerializable
    {
		private uint extension_start_code_identifier;
		public uint ExtensionStartCodeIdentifier { get { return extension_start_code_identifier; } set { extension_start_code_identifier = value; } }
		private byte copyright_flag;
		public byte CopyrightFlag { get { return copyright_flag; } set { copyright_flag = value; } }
		private uint copyright_identifier;
		public uint CopyrightIdentifier { get { return copyright_identifier; } set { copyright_identifier = value; } }
		private byte original_or_copy;
		public byte OriginalOrCopy { get { return original_or_copy; } set { original_or_copy = value; } }
		private uint reserved;
		public uint Reserved { get { return reserved; } set { reserved = value; } }
		private byte marker_bit;
		public byte MarkerBit { get { return marker_bit; } set { marker_bit = value; } }
		private uint copyright_number_1;
		public uint CopyrightNumber1 { get { return copyright_number_1; } set { copyright_number_1 = value; } }
		private uint copyright_number_2;
		public uint CopyrightNumber2 { get { return copyright_number_2; } set { copyright_number_2 = value; } }
		private uint copyright_number_3;
		public uint CopyrightNumber3 { get { return copyright_number_3; } set { copyright_number_3 = value; } }
		private NextStartCode next_start_code;
		public NextStartCode NextStartCode { get { return next_start_code; } set { next_start_code = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public CopyrightExtension()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");

            ulong size = 0;

			size += stream.ReadUnsignedInt(size, 4, out this.extension_start_code_identifier, "extension_start_code_identifier"); 
			size += stream.ReadUnsignedInt(size, 1, out this.copyright_flag, "copyright_flag"); 
			size += stream.ReadUnsignedInt(size, 8, out this.copyright_identifier, "copyright_identifier"); 
			size += stream.ReadUnsignedInt(size, 1, out this.original_or_copy, "original_or_copy"); 
			size += stream.ReadUnsignedInt(size, 7, out this.reserved, "reserved"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 20, out this.copyright_number_1, "copyright_number_1"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 22, out this.copyright_number_2, "copyright_number_2"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 22, out this.copyright_number_3, "copyright_number_3"); 
			this.next_start_code =  new NextStartCode() ;
			size +=  stream.ReadClass<NextStartCode>(size, context, this.next_start_code, "next_start_code"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");
            ulong size = 0;

			size += stream.WriteUnsignedInt(4, this.extension_start_code_identifier, "extension_start_code_identifier"); 
			size += stream.WriteUnsignedInt(1, this.copyright_flag, "copyright_flag"); 
			size += stream.WriteUnsignedInt(8, this.copyright_identifier, "copyright_identifier"); 
			size += stream.WriteUnsignedInt(1, this.original_or_copy, "original_or_copy"); 
			size += stream.WriteUnsignedInt(7, this.reserved, "reserved"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(20, this.copyright_number_1, "copyright_number_1"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(22, this.copyright_number_2, "copyright_number_2"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(22, this.copyright_number_3, "copyright_number_3"); 
			size += stream.WriteClass<NextStartCode>(context, this.next_start_code, "next_start_code"); 

            return size;
         }

    }

    /*


camera_parameters_extension() {
extension_start_code_identifier u(4)
reserved u(1)
camera_id i(7)
marker_bit u(1)
height_of_image_device u(22)
marker_bit u(1)
focal_length u(22)
marker_bit u(1)
f_number u(22)
marker_bit u(1)
vertical_angle_of_view u(22)
marker_bit u(1)
camera_position_x_upper i(16)
marker_bit u(1)
camera_position_x_lower u(16)
marker_bit u(1)
camera_position_y_upper i(16)
marker_bit u(1)
camera_position_y_lower u(16)
marker_bit u(1)
camera_position_z_upper i(16)
marker_bit u(1)
camera_position_z_lower u(16)
marker_bit u(1)
camera_direction_x i(22)
marker_bit u(1)
camera_direction_y i(22)
marker_bit u(1)
camera_direction_z i(22)
marker_bit u(1)
image_plane_vertical_x i(22)
marker_bit u(1)
image_plane_vertical_y i(22)
marker_bit u(1)
image_plane_vertical_z i(22)
marker_bit u(1)
reserved u(32)
next_start_code()
}
    */
    public class CameraParametersExtension : IItuSerializable
    {
		private uint extension_start_code_identifier;
		public uint ExtensionStartCodeIdentifier { get { return extension_start_code_identifier; } set { extension_start_code_identifier = value; } }
		private byte reserved;
		public byte Reserved { get { return reserved; } set { reserved = value; } }
		private int camera_id;
		public int CameraId { get { return camera_id; } set { camera_id = value; } }
		private byte marker_bit;
		public byte MarkerBit { get { return marker_bit; } set { marker_bit = value; } }
		private uint height_of_image_device;
		public uint HeightOfImageDevice { get { return height_of_image_device; } set { height_of_image_device = value; } }
		private uint focal_length;
		public uint FocalLength { get { return focal_length; } set { focal_length = value; } }
		private uint f_number;
		public uint fNumber { get { return f_number; } set { f_number = value; } }
		private uint vertical_angle_of_view;
		public uint VerticalAngleOfView { get { return vertical_angle_of_view; } set { vertical_angle_of_view = value; } }
		private int camera_position_x_upper;
		public int CameraPositionxUpper { get { return camera_position_x_upper; } set { camera_position_x_upper = value; } }
		private uint camera_position_x_lower;
		public uint CameraPositionxLower { get { return camera_position_x_lower; } set { camera_position_x_lower = value; } }
		private int camera_position_y_upper;
		public int CameraPositionyUpper { get { return camera_position_y_upper; } set { camera_position_y_upper = value; } }
		private uint camera_position_y_lower;
		public uint CameraPositionyLower { get { return camera_position_y_lower; } set { camera_position_y_lower = value; } }
		private int camera_position_z_upper;
		public int CameraPositionzUpper { get { return camera_position_z_upper; } set { camera_position_z_upper = value; } }
		private uint camera_position_z_lower;
		public uint CameraPositionzLower { get { return camera_position_z_lower; } set { camera_position_z_lower = value; } }
		private int camera_direction_x;
		public int CameraDirectionx { get { return camera_direction_x; } set { camera_direction_x = value; } }
		private int camera_direction_y;
		public int CameraDirectiony { get { return camera_direction_y; } set { camera_direction_y = value; } }
		private int camera_direction_z;
		public int CameraDirectionz { get { return camera_direction_z; } set { camera_direction_z = value; } }
		private int image_plane_vertical_x;
		public int ImagePlaneVerticalx { get { return image_plane_vertical_x; } set { image_plane_vertical_x = value; } }
		private int image_plane_vertical_y;
		public int ImagePlaneVerticaly { get { return image_plane_vertical_y; } set { image_plane_vertical_y = value; } }
		private int image_plane_vertical_z;
		public int ImagePlaneVerticalz { get { return image_plane_vertical_z; } set { image_plane_vertical_z = value; } }
		private NextStartCode next_start_code;
		public NextStartCode NextStartCode { get { return next_start_code; } set { next_start_code = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public CameraParametersExtension()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");

            ulong size = 0;

			size += stream.ReadUnsignedInt(size, 4, out this.extension_start_code_identifier, "extension_start_code_identifier"); 
			size += stream.ReadUnsignedInt(size, 1, out this.reserved, "reserved"); 
			size += stream.ReadSignedInt(size, 7, out this.camera_id, "camera_id"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 22, out this.height_of_image_device, "height_of_image_device"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 22, out this.focal_length, "focal_length"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 22, out this.f_number, "f_number"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 22, out this.vertical_angle_of_view, "vertical_angle_of_view"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadSignedInt(size, 16, out this.camera_position_x_upper, "camera_position_x_upper"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 16, out this.camera_position_x_lower, "camera_position_x_lower"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadSignedInt(size, 16, out this.camera_position_y_upper, "camera_position_y_upper"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 16, out this.camera_position_y_lower, "camera_position_y_lower"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadSignedInt(size, 16, out this.camera_position_z_upper, "camera_position_z_upper"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 16, out this.camera_position_z_lower, "camera_position_z_lower"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadSignedInt(size, 22, out this.camera_direction_x, "camera_direction_x"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadSignedInt(size, 22, out this.camera_direction_y, "camera_direction_y"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadSignedInt(size, 22, out this.camera_direction_z, "camera_direction_z"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadSignedInt(size, 22, out this.image_plane_vertical_x, "image_plane_vertical_x"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadSignedInt(size, 22, out this.image_plane_vertical_y, "image_plane_vertical_y"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadSignedInt(size, 22, out this.image_plane_vertical_z, "image_plane_vertical_z"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 32, out this.reserved, "reserved"); 
			this.next_start_code =  new NextStartCode() ;
			size +=  stream.ReadClass<NextStartCode>(size, context, this.next_start_code, "next_start_code"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");
            ulong size = 0;

			size += stream.WriteUnsignedInt(4, this.extension_start_code_identifier, "extension_start_code_identifier"); 
			size += stream.WriteUnsignedInt(1, this.reserved, "reserved"); 
			size += stream.WriteSignedInt(7,  this.camera_id, "camera_id"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(22, this.height_of_image_device, "height_of_image_device"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(22, this.focal_length, "focal_length"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(22, this.f_number, "f_number"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(22, this.vertical_angle_of_view, "vertical_angle_of_view"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteSignedInt(16,  this.camera_position_x_upper, "camera_position_x_upper"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(16, this.camera_position_x_lower, "camera_position_x_lower"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteSignedInt(16,  this.camera_position_y_upper, "camera_position_y_upper"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(16, this.camera_position_y_lower, "camera_position_y_lower"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteSignedInt(16,  this.camera_position_z_upper, "camera_position_z_upper"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(16, this.camera_position_z_lower, "camera_position_z_lower"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteSignedInt(22,  this.camera_direction_x, "camera_direction_x"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteSignedInt(22,  this.camera_direction_y, "camera_direction_y"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteSignedInt(22,  this.camera_direction_z, "camera_direction_z"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteSignedInt(22,  this.image_plane_vertical_x, "image_plane_vertical_x"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteSignedInt(22,  this.image_plane_vertical_y, "image_plane_vertical_y"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteSignedInt(22,  this.image_plane_vertical_z, "image_plane_vertical_z"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(32, this.reserved, "reserved"); 
			size += stream.WriteClass<NextStartCode>(context, this.next_start_code, "next_start_code"); 

            return size;
         }

    }

    /*


itu_t_extension() {
extension_start_code_identifier u(4)
while ( before_start_code( itu_t_data ) ) {
itu_t_data u(1)
}
next_start_code()
}
    */
    public class ItutExtension : IItuSerializable
    {
		private uint extension_start_code_identifier;
		public uint ExtensionStartCodeIdentifier { get { return extension_start_code_identifier; } set { extension_start_code_identifier = value; } }
		private Dictionary<int, byte> itu_t_data;
		public Dictionary<int, byte> ItutData { get { return itu_t_data ??= new Dictionary<int, byte>(); } set { itu_t_data = value; } }
		private NextStartCode next_start_code;
		public NextStartCode NextStartCode { get { return next_start_code; } set { next_start_code = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public ItutExtension()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");

            ulong size = 0;

			int whileIndex = -1;
			size += stream.ReadUnsignedInt(size, 4, out this.extension_start_code_identifier, "extension_start_code_identifier"); 

			while ( !ituContext.AtStartCode(stream) )
			{
				whileIndex++;

				size += stream.ReadUnsignedInt(size, 1, whileIndex, (this.itu_t_data ??= new()), "itu_t_data"); 
			}
			this.next_start_code =  new NextStartCode() ;
			size +=  stream.ReadClass<NextStartCode>(size, context, this.next_start_code, "next_start_code"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");
            ulong size = 0;

			int whileIndex = -1;
			size += stream.WriteUnsignedInt(4, this.extension_start_code_identifier, "extension_start_code_identifier"); 

			while ( whileIndex + 1 < this.ItutData.Count )
			{
				whileIndex++;

				size += stream.WriteUnsignedInt(1, whileIndex, (this.itu_t_data ??= new()), "itu_t_data"); 
			}
			size += stream.WriteClass<NextStartCode>(context, this.next_start_code, "next_start_code"); 

            return size;
         }

    }

    /*


content_description_data() {
data_type_upper u(8)
marker_bit u(1)
data_type_lower u(8)
marker_bit u(1)
data_length u(8)
if ( data_type == 1 )
padding_bytes()
else if ( data_type == 2 )
capture_timecode()
else if ( data_type == 3 )
additional_pan_scan_parameters()
else if ( data_type == 4 )
active_region_window()
else if ( data_type == 5 )
coded_picture_length()
else
for ( i = 0; i < data_length; i ++ ) {
loop_marker_bit u(1)
reserved_content_description_data u(8)
}
}
    */
    public class ContentDescriptionData : IItuSerializable
    {
		private uint data_type_upper;
		public uint DataTypeUpper { get { return data_type_upper; } set { data_type_upper = value; } }
		private byte marker_bit;
		public byte MarkerBit { get { return marker_bit; } set { marker_bit = value; } }
		private uint data_type_lower;
		public uint DataTypeLower { get { return data_type_lower; } set { data_type_lower = value; } }
		private uint data_length;
		public uint DataLength { get { return data_length; } set { data_length = value; } }
		private PaddingBytes padding_bytes;
		public PaddingBytes PaddingBytes { get { return padding_bytes; } set { padding_bytes = value; } }
		private CaptureTimecode capture_timecode;
		public CaptureTimecode CaptureTimecode { get { return capture_timecode; } set { capture_timecode = value; } }
		private AdditionalPanScanParameters additional_pan_scan_parameters;
		public AdditionalPanScanParameters AdditionalPanScanParameters { get { return additional_pan_scan_parameters; } set { additional_pan_scan_parameters = value; } }
		private ActiveRegionWindow active_region_window;
		public ActiveRegionWindow ActiveRegionWindow { get { return active_region_window; } set { active_region_window = value; } }
		private CodedPictureLength coded_picture_length;
		public CodedPictureLength CodedPictureLength { get { return coded_picture_length; } set { coded_picture_length = value; } }
		private byte[] loop_marker_bit;
		public byte[] LoopMarkerBit { get { return loop_marker_bit; } set { loop_marker_bit = value; } }
		private uint[] reserved_content_description_data;
		public uint[] ReservedContentDescriptionData { get { return reserved_content_description_data; } set { reserved_content_description_data = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public ContentDescriptionData()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");

            ulong size = 0;

			uint i = 0;
			size += stream.ReadUnsignedInt(size, 8, out this.data_type_upper, "data_type_upper"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 8, out this.data_type_lower, "data_type_lower"); 
			ituContext.DataType = (data_type_upper << 8) | data_type_lower;
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 8, out this.data_length, "data_length"); 
			ituContext.DataLength = data_length;

			if ( ituContext.DataType == 1 )
			{
				this.padding_bytes =  new PaddingBytes() ;
				size +=  stream.ReadClass<PaddingBytes>(size, context, this.padding_bytes, "padding_bytes"); 
			}
			else if ( ituContext.DataType == 2 )
			{
				this.capture_timecode =  new CaptureTimecode() ;
				size +=  stream.ReadClass<CaptureTimecode>(size, context, this.capture_timecode, "capture_timecode"); 
			}
			else if ( ituContext.DataType == 3 )
			{
				this.additional_pan_scan_parameters =  new AdditionalPanScanParameters() ;
				size +=  stream.ReadClass<AdditionalPanScanParameters>(size, context, this.additional_pan_scan_parameters, "additional_pan_scan_parameters"); 
			}
			else if ( ituContext.DataType == 4 )
			{
				this.active_region_window =  new ActiveRegionWindow() ;
				size +=  stream.ReadClass<ActiveRegionWindow>(size, context, this.active_region_window, "active_region_window"); 
			}
			else if ( ituContext.DataType == 5 )
			{
				this.coded_picture_length =  new CodedPictureLength() ;
				size +=  stream.ReadClass<CodedPictureLength>(size, context, this.coded_picture_length, "coded_picture_length"); 
			}
			else 
			{

				stream.CheckArrayAllocation((ulong)( ituContext.DataLength), "loop_marker_bit");
				this.loop_marker_bit = new byte[ ituContext.DataLength];
				stream.CheckArrayAllocation((ulong)( ituContext.DataLength), "reserved_content_description_data");
				this.reserved_content_description_data = new uint[ ituContext.DataLength];
				for ( i = 0; i < ituContext.DataLength; i ++ )
				{
					size += stream.ReadUnsignedInt(size, 1, out this.loop_marker_bit[ i ], "loop_marker_bit"); 
					size += stream.ReadUnsignedInt(size, 8, out this.reserved_content_description_data[ i ], "reserved_content_description_data"); 
				}
			}

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");
            ulong size = 0;

			uint i = 0;
			size += stream.WriteUnsignedInt(8, this.data_type_upper, "data_type_upper"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(8, this.data_type_lower, "data_type_lower"); 
			ituContext.DataType = (data_type_upper << 8) | data_type_lower;
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(8, this.data_length, "data_length"); 
			ituContext.DataLength = data_length;

			if ( ituContext.DataType == 1 )
			{
				size += stream.WriteClass<PaddingBytes>(context, this.padding_bytes, "padding_bytes"); 
			}
			else if ( ituContext.DataType == 2 )
			{
				size += stream.WriteClass<CaptureTimecode>(context, this.capture_timecode, "capture_timecode"); 
			}
			else if ( ituContext.DataType == 3 )
			{
				size += stream.WriteClass<AdditionalPanScanParameters>(context, this.additional_pan_scan_parameters, "additional_pan_scan_parameters"); 
			}
			else if ( ituContext.DataType == 4 )
			{
				size += stream.WriteClass<ActiveRegionWindow>(context, this.active_region_window, "active_region_window"); 
			}
			else if ( ituContext.DataType == 5 )
			{
				size += stream.WriteClass<CodedPictureLength>(context, this.coded_picture_length, "coded_picture_length"); 
			}
			else 
			{

				for ( i = 0; i < ituContext.DataLength; i ++ )
				{
					size += stream.WriteUnsignedInt(1, this.loop_marker_bit[ i ], "loop_marker_bit"); 
					size += stream.WriteUnsignedInt(8, this.reserved_content_description_data[ i ], "reserved_content_description_data"); 
				}
			}

            return size;
         }

    }

    /*


padding_bytes() {
for ( i = 0; i < data_length; i ++ ) {
loop_marker_bit u(1)
padding_byte u(8)
}
}
    */
    public class PaddingBytes : IItuSerializable
    {
		private byte[] loop_marker_bit;
		public byte[] LoopMarkerBit { get { return loop_marker_bit; } set { loop_marker_bit = value; } }
		private uint[] padding_byte;
		public uint[] PaddingByte { get { return padding_byte; } set { padding_byte = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public PaddingBytes()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");

            ulong size = 0;

			uint i = 0;

			stream.CheckArrayAllocation((ulong)( ituContext.DataLength), "loop_marker_bit");
			this.loop_marker_bit = new byte[ ituContext.DataLength];
			stream.CheckArrayAllocation((ulong)( ituContext.DataLength), "padding_byte");
			this.padding_byte = new uint[ ituContext.DataLength];
			for ( i = 0; i < ituContext.DataLength; i ++ )
			{
				size += stream.ReadUnsignedInt(size, 1, out this.loop_marker_bit[ i ], "loop_marker_bit"); 
				size += stream.ReadUnsignedInt(size, 8, out this.padding_byte[ i ], "padding_byte"); 
			}

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");
            ulong size = 0;

			uint i = 0;

			for ( i = 0; i < ituContext.DataLength; i ++ )
			{
				size += stream.WriteUnsignedInt(1, this.loop_marker_bit[ i ], "loop_marker_bit"); 
				size += stream.WriteUnsignedInt(8, this.padding_byte[ i ], "padding_byte"); 
			}

            return size;
         }

    }

    /*


capture_timecode() {
marker_bit u(1)
timecode_type u(2)
counting_type u(3)
reserved_bit u(1)
reserved_bit u(1)
reserved_bit u(1)
if ( counting_type != 0 ) {
marker_bit u(1)
nframes_conversion_code u(1)
clock_divisor u(7)
marker_bit u(1)
nframes_multiplier_upper u(8)
marker_bit u(1)
nframes_multiplier_lower u(8)
}
frame_or_field_capture_timestamp()
if ( timecode_type == 3 )
frame_or_field_capture_timestamp()
}
    */
    public class CaptureTimecode : IItuSerializable
    {
		private byte marker_bit;
		public byte MarkerBit { get { return marker_bit; } set { marker_bit = value; } }
		private uint timecode_type;
		public uint TimecodeType { get { return timecode_type; } set { timecode_type = value; } }
		private uint counting_type;
		public uint CountingType { get { return counting_type; } set { counting_type = value; } }
		private byte reserved_bit;
		public byte ReservedBit { get { return reserved_bit; } set { reserved_bit = value; } }
		private byte nframes_conversion_code;
		public byte NframesConversionCode { get { return nframes_conversion_code; } set { nframes_conversion_code = value; } }
		private uint clock_divisor;
		public uint ClockDivisor { get { return clock_divisor; } set { clock_divisor = value; } }
		private uint nframes_multiplier_upper;
		public uint NframesMultiplierUpper { get { return nframes_multiplier_upper; } set { nframes_multiplier_upper = value; } }
		private uint nframes_multiplier_lower;
		public uint NframesMultiplierLower { get { return nframes_multiplier_lower; } set { nframes_multiplier_lower = value; } }
		private FrameOrFieldCaptureTimestamp frame_or_field_capture_timestamp;
		public FrameOrFieldCaptureTimestamp FrameOrFieldCaptureTimestamp { get { return frame_or_field_capture_timestamp; } set { frame_or_field_capture_timestamp = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public CaptureTimecode()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");

            ulong size = 0;

			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 2, out this.timecode_type, "timecode_type"); 
			size += stream.ReadUnsignedInt(size, 3, out this.counting_type, "counting_type"); 
			ituContext.CountingType = counting_type;
			size += stream.ReadUnsignedInt(size, 1, out this.reserved_bit, "reserved_bit"); 
			size += stream.ReadUnsignedInt(size, 1, out this.reserved_bit, "reserved_bit"); 
			size += stream.ReadUnsignedInt(size, 1, out this.reserved_bit, "reserved_bit"); 

			if ( ituContext.CountingType != 0 )
			{
				size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
				size += stream.ReadUnsignedInt(size, 1, out this.nframes_conversion_code, "nframes_conversion_code"); 
				size += stream.ReadUnsignedInt(size, 7, out this.clock_divisor, "clock_divisor"); 
				size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
				size += stream.ReadUnsignedInt(size, 8, out this.nframes_multiplier_upper, "nframes_multiplier_upper"); 
				size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
				size += stream.ReadUnsignedInt(size, 8, out this.nframes_multiplier_lower, "nframes_multiplier_lower"); 
			}
			this.frame_or_field_capture_timestamp =  new FrameOrFieldCaptureTimestamp() ;
			size +=  stream.ReadClass<FrameOrFieldCaptureTimestamp>(size, context, this.frame_or_field_capture_timestamp, "frame_or_field_capture_timestamp"); 

			if ( timecode_type == 3 )
			{
				this.frame_or_field_capture_timestamp =  new FrameOrFieldCaptureTimestamp() ;
				size +=  stream.ReadClass<FrameOrFieldCaptureTimestamp>(size, context, this.frame_or_field_capture_timestamp, "frame_or_field_capture_timestamp"); 
			}

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");
            ulong size = 0;

			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(2, this.timecode_type, "timecode_type"); 
			size += stream.WriteUnsignedInt(3, this.counting_type, "counting_type"); 
			ituContext.CountingType = counting_type;
			size += stream.WriteUnsignedInt(1, this.reserved_bit, "reserved_bit"); 
			size += stream.WriteUnsignedInt(1, this.reserved_bit, "reserved_bit"); 
			size += stream.WriteUnsignedInt(1, this.reserved_bit, "reserved_bit"); 

			if ( ituContext.CountingType != 0 )
			{
				size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
				size += stream.WriteUnsignedInt(1, this.nframes_conversion_code, "nframes_conversion_code"); 
				size += stream.WriteUnsignedInt(7, this.clock_divisor, "clock_divisor"); 
				size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
				size += stream.WriteUnsignedInt(8, this.nframes_multiplier_upper, "nframes_multiplier_upper"); 
				size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
				size += stream.WriteUnsignedInt(8, this.nframes_multiplier_lower, "nframes_multiplier_lower"); 
			}
			size += stream.WriteClass<FrameOrFieldCaptureTimestamp>(context, this.frame_or_field_capture_timestamp, "frame_or_field_capture_timestamp"); 

			if ( timecode_type == 3 )
			{
				size += stream.WriteClass<FrameOrFieldCaptureTimestamp>(context, this.frame_or_field_capture_timestamp, "frame_or_field_capture_timestamp"); 
			}

            return size;
         }

    }

    /*


frame_or_field_capture_timestamp() {
if ( counting_type != 0 ) {
marker_bit u(1)
nframes u(8)
}
marker_bit u(1)
time_discontinuity u(1)
prior_count_dropped u(1)
time_offset_part_a i(6)
marker_bit u(1)
time_offset_part_b u(8)
marker_bit u(1)
time_offset_part_c u(8)
marker_bit u(1)
time_offset_part_d u(8)
marker_bit u(1)
units_of_seconds u(4)
tens_of_seconds u(4)
marker_bit u(1)
units_of_minutes u(4)
tens_of_minutes u(4)
marker_bit u(1)
units_of_hours u(4)
tens_of_hours u(4)
}
    */
    public class FrameOrFieldCaptureTimestamp : IItuSerializable
    {
		private byte marker_bit;
		public byte MarkerBit { get { return marker_bit; } set { marker_bit = value; } }
		private uint nframes;
		public uint Nframes { get { return nframes; } set { nframes = value; } }
		private byte time_discontinuity;
		public byte TimeDiscontinuity { get { return time_discontinuity; } set { time_discontinuity = value; } }
		private byte prior_count_dropped;
		public byte PriorCountDropped { get { return prior_count_dropped; } set { prior_count_dropped = value; } }
		private int time_offset_part_a;
		public int TimeOffsetParta { get { return time_offset_part_a; } set { time_offset_part_a = value; } }
		private uint time_offset_part_b;
		public uint TimeOffsetPartb { get { return time_offset_part_b; } set { time_offset_part_b = value; } }
		private uint time_offset_part_c;
		public uint TimeOffsetPartc { get { return time_offset_part_c; } set { time_offset_part_c = value; } }
		private uint time_offset_part_d;
		public uint TimeOffsetPartd { get { return time_offset_part_d; } set { time_offset_part_d = value; } }
		private uint units_of_seconds;
		public uint UnitsOfSeconds { get { return units_of_seconds; } set { units_of_seconds = value; } }
		private uint tens_of_seconds;
		public uint TensOfSeconds { get { return tens_of_seconds; } set { tens_of_seconds = value; } }
		private uint units_of_minutes;
		public uint UnitsOfMinutes { get { return units_of_minutes; } set { units_of_minutes = value; } }
		private uint tens_of_minutes;
		public uint TensOfMinutes { get { return tens_of_minutes; } set { tens_of_minutes = value; } }
		private uint units_of_hours;
		public uint UnitsOfHours { get { return units_of_hours; } set { units_of_hours = value; } }
		private uint tens_of_hours;
		public uint TensOfHours { get { return tens_of_hours; } set { tens_of_hours = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public FrameOrFieldCaptureTimestamp()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");

            ulong size = 0;


			if ( ituContext.CountingType != 0 )
			{
				size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
				size += stream.ReadUnsignedInt(size, 8, out this.nframes, "nframes"); 
			}
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 1, out this.time_discontinuity, "time_discontinuity"); 
			size += stream.ReadUnsignedInt(size, 1, out this.prior_count_dropped, "prior_count_dropped"); 
			size += stream.ReadSignedInt(size, 6, out this.time_offset_part_a, "time_offset_part_a"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 8, out this.time_offset_part_b, "time_offset_part_b"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 8, out this.time_offset_part_c, "time_offset_part_c"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 8, out this.time_offset_part_d, "time_offset_part_d"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 4, out this.units_of_seconds, "units_of_seconds"); 
			size += stream.ReadUnsignedInt(size, 4, out this.tens_of_seconds, "tens_of_seconds"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 4, out this.units_of_minutes, "units_of_minutes"); 
			size += stream.ReadUnsignedInt(size, 4, out this.tens_of_minutes, "tens_of_minutes"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 4, out this.units_of_hours, "units_of_hours"); 
			size += stream.ReadUnsignedInt(size, 4, out this.tens_of_hours, "tens_of_hours"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");
            ulong size = 0;


			if ( ituContext.CountingType != 0 )
			{
				size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
				size += stream.WriteUnsignedInt(8, this.nframes, "nframes"); 
			}
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(1, this.time_discontinuity, "time_discontinuity"); 
			size += stream.WriteUnsignedInt(1, this.prior_count_dropped, "prior_count_dropped"); 
			size += stream.WriteSignedInt(6,  this.time_offset_part_a, "time_offset_part_a"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(8, this.time_offset_part_b, "time_offset_part_b"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(8, this.time_offset_part_c, "time_offset_part_c"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(8, this.time_offset_part_d, "time_offset_part_d"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(4, this.units_of_seconds, "units_of_seconds"); 
			size += stream.WriteUnsignedInt(4, this.tens_of_seconds, "tens_of_seconds"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(4, this.units_of_minutes, "units_of_minutes"); 
			size += stream.WriteUnsignedInt(4, this.tens_of_minutes, "tens_of_minutes"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(4, this.units_of_hours, "units_of_hours"); 
			size += stream.WriteUnsignedInt(4, this.tens_of_hours, "tens_of_hours"); 

            return size;
         }

    }

    /*


additional_pan_scan_parameters() {
marker_bit u(1)
aspect_ratio_information u(4)
reserved_bit u(1)
reserved_bit u(1)
reserved_bit u(1)
display_size_present u(1)
if (display_size_present == 1 ) {
marker_bit u(1)
reserved_bit u(1)
reserved_bit u(1)
display_horizontal_size_upper u(6)
marker_bit u(1)
display_horizontal_size_lower u(8)
marker_bit u(1)
reserved_bit u(1)
reserved_bit u(1)
display_vertical_size_upper u(6)
marker_bit u(1)
display_vertical_size_lower u(8)
}
for ( i = 0; i < number_of_frame_centre_offsets; i ++ ) {
loop_marker_bit u(1)
frame_centre_horizontal_offset_upper i(8)
loop_marker_bit u(1)
frame_centre_horizontal_offset_lower u(8)
loop_marker_bit u(1)
frame_centre_vertical_offset_upper i(8)
loop_marker_bit u(1)
frame_centre_vertical_offset_lower u(8)
}
}
    */
    public class AdditionalPanScanParameters : IItuSerializable
    {
		private byte marker_bit;
		public byte MarkerBit { get { return marker_bit; } set { marker_bit = value; } }
		private uint aspect_ratio_information;
		public uint AspectRatioInformation { get { return aspect_ratio_information; } set { aspect_ratio_information = value; } }
		private byte reserved_bit;
		public byte ReservedBit { get { return reserved_bit; } set { reserved_bit = value; } }
		private byte display_size_present;
		public byte DisplaySizePresent { get { return display_size_present; } set { display_size_present = value; } }
		private uint display_horizontal_size_upper;
		public uint DisplayHorizontalSizeUpper { get { return display_horizontal_size_upper; } set { display_horizontal_size_upper = value; } }
		private uint display_horizontal_size_lower;
		public uint DisplayHorizontalSizeLower { get { return display_horizontal_size_lower; } set { display_horizontal_size_lower = value; } }
		private uint display_vertical_size_upper;
		public uint DisplayVerticalSizeUpper { get { return display_vertical_size_upper; } set { display_vertical_size_upper = value; } }
		private uint display_vertical_size_lower;
		public uint DisplayVerticalSizeLower { get { return display_vertical_size_lower; } set { display_vertical_size_lower = value; } }
		private byte[] loop_marker_bit;
		public byte[] LoopMarkerBit { get { return loop_marker_bit; } set { loop_marker_bit = value; } }
		private int[] frame_centre_horizontal_offset_upper;
		public int[] FrameCentreHorizontalOffsetUpper { get { return frame_centre_horizontal_offset_upper; } set { frame_centre_horizontal_offset_upper = value; } }
		private uint[] frame_centre_horizontal_offset_lower;
		public uint[] FrameCentreHorizontalOffsetLower { get { return frame_centre_horizontal_offset_lower; } set { frame_centre_horizontal_offset_lower = value; } }
		private int[] frame_centre_vertical_offset_upper;
		public int[] FrameCentreVerticalOffsetUpper { get { return frame_centre_vertical_offset_upper; } set { frame_centre_vertical_offset_upper = value; } }
		private uint[] frame_centre_vertical_offset_lower;
		public uint[] FrameCentreVerticalOffsetLower { get { return frame_centre_vertical_offset_lower; } set { frame_centre_vertical_offset_lower = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public AdditionalPanScanParameters()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");

            ulong size = 0;

			uint i = 0;
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 4, out this.aspect_ratio_information, "aspect_ratio_information"); 
			size += stream.ReadUnsignedInt(size, 1, out this.reserved_bit, "reserved_bit"); 
			size += stream.ReadUnsignedInt(size, 1, out this.reserved_bit, "reserved_bit"); 
			size += stream.ReadUnsignedInt(size, 1, out this.reserved_bit, "reserved_bit"); 
			size += stream.ReadUnsignedInt(size, 1, out this.display_size_present, "display_size_present"); 

			if (display_size_present == 1 )
			{
				size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
				size += stream.ReadUnsignedInt(size, 1, out this.reserved_bit, "reserved_bit"); 
				size += stream.ReadUnsignedInt(size, 1, out this.reserved_bit, "reserved_bit"); 
				size += stream.ReadUnsignedInt(size, 6, out this.display_horizontal_size_upper, "display_horizontal_size_upper"); 
				size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
				size += stream.ReadUnsignedInt(size, 8, out this.display_horizontal_size_lower, "display_horizontal_size_lower"); 
				size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
				size += stream.ReadUnsignedInt(size, 1, out this.reserved_bit, "reserved_bit"); 
				size += stream.ReadUnsignedInt(size, 1, out this.reserved_bit, "reserved_bit"); 
				size += stream.ReadUnsignedInt(size, 6, out this.display_vertical_size_upper, "display_vertical_size_upper"); 
				size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
				size += stream.ReadUnsignedInt(size, 8, out this.display_vertical_size_lower, "display_vertical_size_lower"); 
			}

			stream.CheckArrayAllocation((ulong)( ituContext.NumberOfFrameCentreOffsets), "loop_marker_bit");
			this.loop_marker_bit = new byte[ ituContext.NumberOfFrameCentreOffsets];
			stream.CheckArrayAllocation((ulong)( ituContext.NumberOfFrameCentreOffsets), "frame_centre_horizontal_offset_upper");
			this.frame_centre_horizontal_offset_upper = new int[ ituContext.NumberOfFrameCentreOffsets];
			stream.CheckArrayAllocation((ulong)( ituContext.NumberOfFrameCentreOffsets), "frame_centre_horizontal_offset_lower");
			this.frame_centre_horizontal_offset_lower = new uint[ ituContext.NumberOfFrameCentreOffsets];
			stream.CheckArrayAllocation((ulong)( ituContext.NumberOfFrameCentreOffsets), "frame_centre_vertical_offset_upper");
			this.frame_centre_vertical_offset_upper = new int[ ituContext.NumberOfFrameCentreOffsets];
			stream.CheckArrayAllocation((ulong)( ituContext.NumberOfFrameCentreOffsets), "frame_centre_vertical_offset_lower");
			this.frame_centre_vertical_offset_lower = new uint[ ituContext.NumberOfFrameCentreOffsets];
			for ( i = 0; i < ituContext.NumberOfFrameCentreOffsets; i ++ )
			{
				size += stream.ReadUnsignedInt(size, 1, out this.loop_marker_bit[ i ], "loop_marker_bit"); 
				size += stream.ReadSignedInt(size, 8, out this.frame_centre_horizontal_offset_upper[ i ], "frame_centre_horizontal_offset_upper"); 
				size += stream.ReadUnsignedInt(size, 1, out this.loop_marker_bit[ i ], "loop_marker_bit"); 
				size += stream.ReadUnsignedInt(size, 8, out this.frame_centre_horizontal_offset_lower[ i ], "frame_centre_horizontal_offset_lower"); 
				size += stream.ReadUnsignedInt(size, 1, out this.loop_marker_bit[ i ], "loop_marker_bit"); 
				size += stream.ReadSignedInt(size, 8, out this.frame_centre_vertical_offset_upper[ i ], "frame_centre_vertical_offset_upper"); 
				size += stream.ReadUnsignedInt(size, 1, out this.loop_marker_bit[ i ], "loop_marker_bit"); 
				size += stream.ReadUnsignedInt(size, 8, out this.frame_centre_vertical_offset_lower[ i ], "frame_centre_vertical_offset_lower"); 
			}

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");
            ulong size = 0;

			uint i = 0;
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(4, this.aspect_ratio_information, "aspect_ratio_information"); 
			size += stream.WriteUnsignedInt(1, this.reserved_bit, "reserved_bit"); 
			size += stream.WriteUnsignedInt(1, this.reserved_bit, "reserved_bit"); 
			size += stream.WriteUnsignedInt(1, this.reserved_bit, "reserved_bit"); 
			size += stream.WriteUnsignedInt(1, this.display_size_present, "display_size_present"); 

			if (display_size_present == 1 )
			{
				size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
				size += stream.WriteUnsignedInt(1, this.reserved_bit, "reserved_bit"); 
				size += stream.WriteUnsignedInt(1, this.reserved_bit, "reserved_bit"); 
				size += stream.WriteUnsignedInt(6, this.display_horizontal_size_upper, "display_horizontal_size_upper"); 
				size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
				size += stream.WriteUnsignedInt(8, this.display_horizontal_size_lower, "display_horizontal_size_lower"); 
				size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
				size += stream.WriteUnsignedInt(1, this.reserved_bit, "reserved_bit"); 
				size += stream.WriteUnsignedInt(1, this.reserved_bit, "reserved_bit"); 
				size += stream.WriteUnsignedInt(6, this.display_vertical_size_upper, "display_vertical_size_upper"); 
				size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
				size += stream.WriteUnsignedInt(8, this.display_vertical_size_lower, "display_vertical_size_lower"); 
			}

			for ( i = 0; i < ituContext.NumberOfFrameCentreOffsets; i ++ )
			{
				size += stream.WriteUnsignedInt(1, this.loop_marker_bit[ i ], "loop_marker_bit"); 
				size += stream.WriteSignedInt(8,  this.frame_centre_horizontal_offset_upper[ i ], "frame_centre_horizontal_offset_upper"); 
				size += stream.WriteUnsignedInt(1, this.loop_marker_bit[ i ], "loop_marker_bit"); 
				size += stream.WriteUnsignedInt(8, this.frame_centre_horizontal_offset_lower[ i ], "frame_centre_horizontal_offset_lower"); 
				size += stream.WriteUnsignedInt(1, this.loop_marker_bit[ i ], "loop_marker_bit"); 
				size += stream.WriteSignedInt(8,  this.frame_centre_vertical_offset_upper[ i ], "frame_centre_vertical_offset_upper"); 
				size += stream.WriteUnsignedInt(1, this.loop_marker_bit[ i ], "loop_marker_bit"); 
				size += stream.WriteUnsignedInt(8, this.frame_centre_vertical_offset_lower[ i ], "frame_centre_vertical_offset_lower"); 
			}

            return size;
         }

    }

    /*


active_region_window() {
marker_bit u(1)
top_left_x_upper u(8)
marker_bit u(1)
top_left_x_lower u(8)
marker_bit u(1)
top_left_y_upper u(8)
marker_bit u(1)
top_left_y_lower u(8)
marker_bit u(1)
active_horizontal_size_upper u(8)
marker_bit u(1)
active_horizontal_size_lower u(8)
marker_bit u(1)
active_vertical_size_upper u(8)
marker_bit u(1)
active_vertical_size_lower u(8)
}
    */
    public class ActiveRegionWindow : IItuSerializable
    {
		private byte marker_bit;
		public byte MarkerBit { get { return marker_bit; } set { marker_bit = value; } }
		private uint top_left_x_upper;
		public uint TopLeftxUpper { get { return top_left_x_upper; } set { top_left_x_upper = value; } }
		private uint top_left_x_lower;
		public uint TopLeftxLower { get { return top_left_x_lower; } set { top_left_x_lower = value; } }
		private uint top_left_y_upper;
		public uint TopLeftyUpper { get { return top_left_y_upper; } set { top_left_y_upper = value; } }
		private uint top_left_y_lower;
		public uint TopLeftyLower { get { return top_left_y_lower; } set { top_left_y_lower = value; } }
		private uint active_horizontal_size_upper;
		public uint ActiveHorizontalSizeUpper { get { return active_horizontal_size_upper; } set { active_horizontal_size_upper = value; } }
		private uint active_horizontal_size_lower;
		public uint ActiveHorizontalSizeLower { get { return active_horizontal_size_lower; } set { active_horizontal_size_lower = value; } }
		private uint active_vertical_size_upper;
		public uint ActiveVerticalSizeUpper { get { return active_vertical_size_upper; } set { active_vertical_size_upper = value; } }
		private uint active_vertical_size_lower;
		public uint ActiveVerticalSizeLower { get { return active_vertical_size_lower; } set { active_vertical_size_lower = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public ActiveRegionWindow()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");

            ulong size = 0;

			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 8, out this.top_left_x_upper, "top_left_x_upper"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 8, out this.top_left_x_lower, "top_left_x_lower"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 8, out this.top_left_y_upper, "top_left_y_upper"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 8, out this.top_left_y_lower, "top_left_y_lower"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 8, out this.active_horizontal_size_upper, "active_horizontal_size_upper"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 8, out this.active_horizontal_size_lower, "active_horizontal_size_lower"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 8, out this.active_vertical_size_upper, "active_vertical_size_upper"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 8, out this.active_vertical_size_lower, "active_vertical_size_lower"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");
            ulong size = 0;

			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(8, this.top_left_x_upper, "top_left_x_upper"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(8, this.top_left_x_lower, "top_left_x_lower"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(8, this.top_left_y_upper, "top_left_y_upper"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(8, this.top_left_y_lower, "top_left_y_lower"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(8, this.active_horizontal_size_upper, "active_horizontal_size_upper"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(8, this.active_horizontal_size_lower, "active_horizontal_size_lower"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(8, this.active_vertical_size_upper, "active_vertical_size_upper"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(8, this.active_vertical_size_lower, "active_vertical_size_lower"); 

            return size;
         }

    }

    /*


coded_picture_length() {
marker_bit u(1)
picture_byte_count_part_a u(8)
marker_bit u(1)
picture_byte_count_part_b u(8)
marker_bit u(1)
picture_byte_count_part_c u(8)
marker_bit u(1)
picture_byte_count_part_d u(8)
}
    */
    public class CodedPictureLength : IItuSerializable
    {
		private byte marker_bit;
		public byte MarkerBit { get { return marker_bit; } set { marker_bit = value; } }
		private uint picture_byte_count_part_a;
		public uint PictureByteCountParta { get { return picture_byte_count_part_a; } set { picture_byte_count_part_a = value; } }
		private uint picture_byte_count_part_b;
		public uint PictureByteCountPartb { get { return picture_byte_count_part_b; } set { picture_byte_count_part_b = value; } }
		private uint picture_byte_count_part_c;
		public uint PictureByteCountPartc { get { return picture_byte_count_part_c; } set { picture_byte_count_part_c = value; } }
		private uint picture_byte_count_part_d;
		public uint PictureByteCountPartd { get { return picture_byte_count_part_d; } set { picture_byte_count_part_d = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public CodedPictureLength()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");

            ulong size = 0;

			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 8, out this.picture_byte_count_part_a, "picture_byte_count_part_a"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 8, out this.picture_byte_count_part_b, "picture_byte_count_part_b"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 8, out this.picture_byte_count_part_c, "picture_byte_count_part_c"); 
			size += stream.ReadUnsignedInt(size, 1, out this.marker_bit, "marker_bit"); 
			size += stream.ReadUnsignedInt(size, 8, out this.picture_byte_count_part_d, "picture_byte_count_part_d"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");
            ulong size = 0;

			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(8, this.picture_byte_count_part_a, "picture_byte_count_part_a"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(8, this.picture_byte_count_part_b, "picture_byte_count_part_b"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(8, this.picture_byte_count_part_c, "picture_byte_count_part_c"); 
			size += stream.WriteUnsignedInt(1, this.marker_bit, "marker_bit"); 
			size += stream.WriteUnsignedInt(8, this.picture_byte_count_part_d, "picture_byte_count_part_d"); 

            return size;
         }

    }

    /*


slice() {
slice_start_code u(32)
if (vertical_size > 2800)
slice_vertical_position_extension u(3)
if (sequence_scalable_extension_present) {
if (scalable_mode == 0)
priority_breakpoint u(7)
}
quantiser_scale_code u(5)
if ( next_bit_one( slice_extension_flag ) ) {
slice_extension_flag u(1)
intra_slice u(1)
slice_picture_id_enable u(1)
slice_picture_id u(6)
while ( next_bit_one( extra_bit_slice ) ) {
extra_bit_slice /* with the value '1' *//* u(1)
extra_information_slice u(8)
}
}
last_extra_bit_slice /* with the value '0' *//* u(1)
slice_data()
next_start_code()
}
    */
    public class Slice : IItuSerializable
    {
		private uint slice_start_code;
		public uint SliceStartCode { get { return slice_start_code; } set { slice_start_code = value; } }
		private uint slice_vertical_position_extension;
		public uint SliceVerticalPositionExtension { get { return slice_vertical_position_extension; } set { slice_vertical_position_extension = value; } }
		private uint priority_breakpoint;
		public uint PriorityBreakpoint { get { return priority_breakpoint; } set { priority_breakpoint = value; } }
		private uint quantiser_scale_code;
		public uint QuantiserScaleCode { get { return quantiser_scale_code; } set { quantiser_scale_code = value; } }
		private byte slice_extension_flag;
		public byte SliceExtensionFlag { get { return slice_extension_flag; } set { slice_extension_flag = value; } }
		private byte intra_slice;
		public byte IntraSlice { get { return intra_slice; } set { intra_slice = value; } }
		private byte slice_picture_id_enable;
		public byte SlicePictureIdEnable { get { return slice_picture_id_enable; } set { slice_picture_id_enable = value; } }
		private uint slice_picture_id;
		public uint SlicePictureId { get { return slice_picture_id; } set { slice_picture_id = value; } }
		private Dictionary<int, byte> extra_bit_slice;
		public Dictionary<int, byte> ExtraBitSlice { get { return extra_bit_slice ??= new Dictionary<int, byte>(); } set { extra_bit_slice = value; } }
		private Dictionary<int, uint> extra_information_slice;
		public Dictionary<int, uint> ExtraInformationSlice { get { return extra_information_slice ??= new Dictionary<int, uint>(); } set { extra_information_slice = value; } }
		private byte last_extra_bit_slice;
		public byte LastExtraBitSlice { get { return last_extra_bit_slice; } set { last_extra_bit_slice = value; } }
		private SliceData slice_data;
		public SliceData SliceData { get { return slice_data; } set { slice_data = value; } }
		private NextStartCode next_start_code;
		public NextStartCode NextStartCode { get { return next_start_code; } set { next_start_code = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public Slice()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");

            ulong size = 0;

			int whileIndex = -1;
			size += stream.ReadUnsignedInt(size, 32, out this.slice_start_code, "slice_start_code"); 

			if (ituContext.VerticalSize > 2800)
			{
				size += stream.ReadUnsignedInt(size, 3, out this.slice_vertical_position_extension, "slice_vertical_position_extension"); 
			}

			if ((ituContext.SequenceScalableExtension != null ? 1 : 0) != 0)
			{

				if (ituContext.SequenceScalableExtension.ScalableMode == 0)
				{
					size += stream.ReadUnsignedInt(size, 7, out this.priority_breakpoint, "priority_breakpoint"); 
				}
			}
			size += stream.ReadUnsignedInt(size, 5, out this.quantiser_scale_code, "quantiser_scale_code"); 

			if ( ituContext.NextBits(stream, 1) == 1 )
			{
				size += stream.ReadUnsignedInt(size, 1, out this.slice_extension_flag, "slice_extension_flag"); 
				size += stream.ReadUnsignedInt(size, 1, out this.intra_slice, "intra_slice"); 
				size += stream.ReadUnsignedInt(size, 1, out this.slice_picture_id_enable, "slice_picture_id_enable"); 
				size += stream.ReadUnsignedInt(size, 6, out this.slice_picture_id, "slice_picture_id"); 

				while ( ituContext.NextBits(stream, 1) == 1 )
				{
					whileIndex++;

					size += stream.ReadUnsignedInt(size, 1, whileIndex, (this.extra_bit_slice ??= new()), "extra_bit_slice"); // with the value '1' 
					size += stream.ReadUnsignedInt(size, 8, whileIndex, (this.extra_information_slice ??= new()), "extra_information_slice"); 
				}
			}
			size += stream.ReadUnsignedInt(size, 1, out this.last_extra_bit_slice, "last_extra_bit_slice"); // with the value '0' 
			this.slice_data =  new SliceData() ;
			size +=  stream.ReadClass<SliceData>(size, context, this.slice_data, "slice_data"); 
			this.next_start_code =  new NextStartCode() ;
			size +=  stream.ReadClass<NextStartCode>(size, context, this.next_start_code, "next_start_code"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            H262Context ituContext = context as H262Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H262Context");
            ulong size = 0;

			int whileIndex = -1;
			size += stream.WriteUnsignedInt(32, this.slice_start_code, "slice_start_code"); 

			if (ituContext.VerticalSize > 2800)
			{
				size += stream.WriteUnsignedInt(3, this.slice_vertical_position_extension, "slice_vertical_position_extension"); 
			}

			if ((ituContext.SequenceScalableExtension != null ? 1 : 0) != 0)
			{

				if (ituContext.SequenceScalableExtension.ScalableMode == 0)
				{
					size += stream.WriteUnsignedInt(7, this.priority_breakpoint, "priority_breakpoint"); 
				}
			}
			size += stream.WriteUnsignedInt(5, this.quantiser_scale_code, "quantiser_scale_code"); 

			if ( this.slice_extension_flag == 1 )
			{
				size += stream.WriteUnsignedInt(1, this.slice_extension_flag, "slice_extension_flag"); 
				size += stream.WriteUnsignedInt(1, this.intra_slice, "intra_slice"); 
				size += stream.WriteUnsignedInt(1, this.slice_picture_id_enable, "slice_picture_id_enable"); 
				size += stream.WriteUnsignedInt(6, this.slice_picture_id, "slice_picture_id"); 

				while ( whileIndex + 1 < this.ExtraBitSlice.Count )
				{
					whileIndex++;

					size += stream.WriteUnsignedInt(1, whileIndex, (this.extra_bit_slice ??= new()), "extra_bit_slice"); // with the value '1' 
					size += stream.WriteUnsignedInt(8, whileIndex, (this.extra_information_slice ??= new()), "extra_information_slice"); 
				}
			}
			size += stream.WriteUnsignedInt(1, this.last_extra_bit_slice, "last_extra_bit_slice"); // with the value '0' 
			size += stream.WriteClass<SliceData>(context, this.slice_data, "slice_data"); 
			size += stream.WriteClass<NextStartCode>(context, this.next_start_code, "next_start_code"); 

            return size;
         }

    }

}
