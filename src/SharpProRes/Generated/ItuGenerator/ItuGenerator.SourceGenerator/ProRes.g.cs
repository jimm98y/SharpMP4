using System;
using System.Collections.Generic;
using System.Numerics;
using SharpH26X;

namespace SharpProRes
{

    public partial class ProResContext : IItuContext
    {

    }

    /*
frame() {
frame_size u(32)
frame_identifier u(32)
frame_header()
picture( 0 )
if (interlace_mode == 1 || interlace_mode == 2)
picture( 1 )
if (stuffing_size > 0)
stuffing()
}
    */
    public class Frame : IItuSerializable
    {
		private uint frame_size;
		public uint FrameSize { get { return frame_size; } set { frame_size = value; } }
		private uint frame_identifier;
		public uint FrameIdentifier { get { return frame_identifier; } set { frame_identifier = value; } }
		private FrameHeader frame_header;
		public FrameHeader FrameHeader { get { return frame_header; } set { frame_header = value; } }
		private Picture picture;
		public Picture Picture { get { return picture; } set { picture = value; } }
		private Picture picture0;
		public Picture Picture0 { get { return picture0; } set { picture0 = value; } }
		private Stuffing stuffing;
		public Stuffing Stuffing { get { return stuffing; } set { stuffing = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public Frame()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            ProResContext ituContext = context as ProResContext;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type ProResContext");

            ulong size = 0;

			size += stream.ReadUnsignedInt(size, 32, out this.frame_size, "frame_size"); 
			ituContext.OnFrame(frame_size);
			size += stream.ReadUnsignedInt(size, 32, out this.frame_identifier, "frame_identifier"); 
			this.frame_header =  new FrameHeader() ;
			size +=  stream.ReadClass<FrameHeader>(size, context, this.frame_header, "frame_header"); 
			this.picture =  new Picture( 0 ) ;
			size +=  stream.ReadClass<Picture>(size, context, this.picture, "picture"); 

			if (ituContext.InterlaceMode == 1 || ituContext.InterlaceMode == 2)
			{
				this.picture0 =  new Picture( 1 ) ;
				size +=  stream.ReadClass<Picture>(size, context, this.picture0, "picture0"); 
			}

			if (ituContext.StuffingSize > 0)
			{
				this.stuffing =  new Stuffing() ;
				size +=  stream.ReadClass<Stuffing>(size, context, this.stuffing, "stuffing"); 
			}

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            ProResContext ituContext = context as ProResContext;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type ProResContext");
            ulong size = 0;

			size += stream.WriteUnsignedInt(32, this.frame_size, "frame_size"); 
			ituContext.OnFrame(frame_size);
			size += stream.WriteUnsignedInt(32, this.frame_identifier, "frame_identifier"); 
			size += stream.WriteClass<FrameHeader>(context, this.frame_header, "frame_header"); 
			size += stream.WriteClass<Picture>(context, this.picture, "picture"); 

			if (ituContext.InterlaceMode == 1 || ituContext.InterlaceMode == 2)
			{
				size += stream.WriteClass<Picture>(context, this.picture0, "picture0"); 
			}

			if (ituContext.StuffingSize > 0)
			{
				size += stream.WriteClass<Stuffing>(context, this.stuffing, "stuffing"); 
			}

            return size;
         }

    }

    /*


frame_header() {
frame_header_size u(16)
reserved u(8)
bitstream_version u(8)
encoder_identifier u(32)
horizontal_size u(16)
vertical_size u(16)
chroma_format u(2)
reserved_2 u(2)
interlace_mode u(2)
reserved_3 u(2)
aspect_ratio_information u(4)
frame_rate_code u(4)
color_primaries u(8)
transfer_characteristic u(8)
matrix_coefficients u(8)
reserved_4 u(4)
alpha_channel_type u(4)
reserved_5 u(14)
load_luma_quantization_matrix u(1)
load_chroma_quantization_matrix u(1)
if (load_luma_quantization_matrix) {
for (v = 0; v < 8; v++)
for (u = 0; u < 8; u++)
luma_quantization_matrix[v][u] u(8)
}
if (load_chroma_quantization_matrix) {
for (v = 0; v < 8; v++)
for (u = 0; u < 8; u++)
chroma_quantization_matrix[v][u] u(8)
}
frame_header_remainder()
}
    */
    public class FrameHeader : IItuSerializable
    {
		private uint frame_header_size;
		public uint FrameHeaderSize { get { return frame_header_size; } set { frame_header_size = value; } }
		private uint reserved;
		public uint Reserved { get { return reserved; } set { reserved = value; } }
		private uint bitstream_version;
		public uint BitstreamVersion { get { return bitstream_version; } set { bitstream_version = value; } }
		private uint encoder_identifier;
		public uint EncoderIdentifier { get { return encoder_identifier; } set { encoder_identifier = value; } }
		private uint horizontal_size;
		public uint HorizontalSize { get { return horizontal_size; } set { horizontal_size = value; } }
		private uint vertical_size;
		public uint VerticalSize { get { return vertical_size; } set { vertical_size = value; } }
		private uint chroma_format;
		public uint ChromaFormat { get { return chroma_format; } set { chroma_format = value; } }
		private uint reserved_2;
		public uint Reserved2 { get { return reserved_2; } set { reserved_2 = value; } }
		private uint interlace_mode;
		public uint InterlaceMode { get { return interlace_mode; } set { interlace_mode = value; } }
		private uint reserved_3;
		public uint Reserved3 { get { return reserved_3; } set { reserved_3 = value; } }
		private uint aspect_ratio_information;
		public uint AspectRatioInformation { get { return aspect_ratio_information; } set { aspect_ratio_information = value; } }
		private uint frame_rate_code;
		public uint FrameRateCode { get { return frame_rate_code; } set { frame_rate_code = value; } }
		private uint color_primaries;
		public uint ColorPrimaries { get { return color_primaries; } set { color_primaries = value; } }
		private uint transfer_characteristic;
		public uint TransferCharacteristic { get { return transfer_characteristic; } set { transfer_characteristic = value; } }
		private uint matrix_coefficients;
		public uint MatrixCoefficients { get { return matrix_coefficients; } set { matrix_coefficients = value; } }
		private uint reserved_4;
		public uint Reserved4 { get { return reserved_4; } set { reserved_4 = value; } }
		private uint alpha_channel_type;
		public uint AlphaChannelType { get { return alpha_channel_type; } set { alpha_channel_type = value; } }
		private uint reserved_5;
		public uint Reserved5 { get { return reserved_5; } set { reserved_5 = value; } }
		private byte load_luma_quantization_matrix;
		public byte LoadLumaQuantizationMatrix { get { return load_luma_quantization_matrix; } set { load_luma_quantization_matrix = value; } }
		private byte load_chroma_quantization_matrix;
		public byte LoadChromaQuantizationMatrix { get { return load_chroma_quantization_matrix; } set { load_chroma_quantization_matrix = value; } }
		private uint[][] luma_quantization_matrix;
		public uint[][] LumaQuantizationMatrix { get { return luma_quantization_matrix; } set { luma_quantization_matrix = value; } }
		private uint[][] chroma_quantization_matrix;
		public uint[][] ChromaQuantizationMatrix { get { return chroma_quantization_matrix; } set { chroma_quantization_matrix = value; } }
		private FrameHeaderRemainder frame_header_remainder;
		public FrameHeaderRemainder FrameHeaderRemainder { get { return frame_header_remainder; } set { frame_header_remainder = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public FrameHeader()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            ProResContext ituContext = context as ProResContext;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type ProResContext");

            ulong size = 0;

			uint v = 0;
			uint u = 0;
			size += stream.ReadUnsignedInt(size, 16, out this.frame_header_size, "frame_header_size"); 
			ituContext.FrameHeaderSize = frame_header_size;
			size += stream.ReadUnsignedInt(size, 8, out this.reserved, "reserved"); 
			size += stream.ReadUnsignedInt(size, 8, out this.bitstream_version, "bitstream_version"); 
			size += stream.ReadUnsignedInt(size, 32, out this.encoder_identifier, "encoder_identifier"); 
			size += stream.ReadUnsignedInt(size, 16, out this.horizontal_size, "horizontal_size"); 
			ituContext.HorizontalSize = horizontal_size;
			size += stream.ReadUnsignedInt(size, 16, out this.vertical_size, "vertical_size"); 
			ituContext.VerticalSize = vertical_size;
			size += stream.ReadUnsignedInt(size, 2, out this.chroma_format, "chroma_format"); 
			size += stream.ReadUnsignedInt(size, 2, out this.reserved_2, "reserved_2"); 
			size += stream.ReadUnsignedInt(size, 2, out this.interlace_mode, "interlace_mode"); 
			ituContext.InterlaceMode = interlace_mode;
			size += stream.ReadUnsignedInt(size, 2, out this.reserved_3, "reserved_3"); 
			size += stream.ReadUnsignedInt(size, 4, out this.aspect_ratio_information, "aspect_ratio_information"); 
			size += stream.ReadUnsignedInt(size, 4, out this.frame_rate_code, "frame_rate_code"); 
			size += stream.ReadUnsignedInt(size, 8, out this.color_primaries, "color_primaries"); 
			size += stream.ReadUnsignedInt(size, 8, out this.transfer_characteristic, "transfer_characteristic"); 
			size += stream.ReadUnsignedInt(size, 8, out this.matrix_coefficients, "matrix_coefficients"); 
			size += stream.ReadUnsignedInt(size, 4, out this.reserved_4, "reserved_4"); 
			size += stream.ReadUnsignedInt(size, 4, out this.alpha_channel_type, "alpha_channel_type"); 
			size += stream.ReadUnsignedInt(size, 14, out this.reserved_5, "reserved_5"); 
			size += stream.ReadUnsignedInt(size, 1, out this.load_luma_quantization_matrix, "load_luma_quantization_matrix"); 
			size += stream.ReadUnsignedInt(size, 1, out this.load_chroma_quantization_matrix, "load_chroma_quantization_matrix"); 
			ituContext.OnQuantizationMatrices(load_luma_quantization_matrix, load_chroma_quantization_matrix);

			if (load_luma_quantization_matrix != 0)
			{

				stream.CheckArrayAllocation((ulong)( 8), "luma_quantization_matrix");
				this.luma_quantization_matrix = new uint[ 8][];
				for (v = 0; v < 8; v++)
				{

					stream.CheckArrayAllocation((ulong)( 8), "luma_quantization_matrix[v ]");
					this.luma_quantization_matrix[v ] = new uint[ 8];
					for (u = 0; u < 8; u++)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.luma_quantization_matrix[v][u], "luma_quantization_matrix"); 
					}
				}
			}

			if (load_chroma_quantization_matrix != 0)
			{

				stream.CheckArrayAllocation((ulong)( 8), "chroma_quantization_matrix");
				this.chroma_quantization_matrix = new uint[ 8][];
				for (v = 0; v < 8; v++)
				{

					stream.CheckArrayAllocation((ulong)( 8), "chroma_quantization_matrix[v ]");
					this.chroma_quantization_matrix[v ] = new uint[ 8];
					for (u = 0; u < 8; u++)
					{
						size += stream.ReadUnsignedInt(size, 8, out this.chroma_quantization_matrix[v][u], "chroma_quantization_matrix"); 
					}
				}
			}
			this.frame_header_remainder =  new FrameHeaderRemainder() ;
			size +=  stream.ReadClass<FrameHeaderRemainder>(size, context, this.frame_header_remainder, "frame_header_remainder"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            ProResContext ituContext = context as ProResContext;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type ProResContext");
            ulong size = 0;

			uint v = 0;
			uint u = 0;
			size += stream.WriteUnsignedInt(16, this.frame_header_size, "frame_header_size"); 
			ituContext.FrameHeaderSize = frame_header_size;
			size += stream.WriteUnsignedInt(8, this.reserved, "reserved"); 
			size += stream.WriteUnsignedInt(8, this.bitstream_version, "bitstream_version"); 
			size += stream.WriteUnsignedInt(32, this.encoder_identifier, "encoder_identifier"); 
			size += stream.WriteUnsignedInt(16, this.horizontal_size, "horizontal_size"); 
			ituContext.HorizontalSize = horizontal_size;
			size += stream.WriteUnsignedInt(16, this.vertical_size, "vertical_size"); 
			ituContext.VerticalSize = vertical_size;
			size += stream.WriteUnsignedInt(2, this.chroma_format, "chroma_format"); 
			size += stream.WriteUnsignedInt(2, this.reserved_2, "reserved_2"); 
			size += stream.WriteUnsignedInt(2, this.interlace_mode, "interlace_mode"); 
			ituContext.InterlaceMode = interlace_mode;
			size += stream.WriteUnsignedInt(2, this.reserved_3, "reserved_3"); 
			size += stream.WriteUnsignedInt(4, this.aspect_ratio_information, "aspect_ratio_information"); 
			size += stream.WriteUnsignedInt(4, this.frame_rate_code, "frame_rate_code"); 
			size += stream.WriteUnsignedInt(8, this.color_primaries, "color_primaries"); 
			size += stream.WriteUnsignedInt(8, this.transfer_characteristic, "transfer_characteristic"); 
			size += stream.WriteUnsignedInt(8, this.matrix_coefficients, "matrix_coefficients"); 
			size += stream.WriteUnsignedInt(4, this.reserved_4, "reserved_4"); 
			size += stream.WriteUnsignedInt(4, this.alpha_channel_type, "alpha_channel_type"); 
			size += stream.WriteUnsignedInt(14, this.reserved_5, "reserved_5"); 
			size += stream.WriteUnsignedInt(1, this.load_luma_quantization_matrix, "load_luma_quantization_matrix"); 
			size += stream.WriteUnsignedInt(1, this.load_chroma_quantization_matrix, "load_chroma_quantization_matrix"); 
			ituContext.OnQuantizationMatrices(load_luma_quantization_matrix, load_chroma_quantization_matrix);

			if (load_luma_quantization_matrix != 0)
			{

				for (v = 0; v < 8; v++)
				{

					for (u = 0; u < 8; u++)
					{
						size += stream.WriteUnsignedInt(8, this.luma_quantization_matrix[v][u], "luma_quantization_matrix"); 
					}
				}
			}

			if (load_chroma_quantization_matrix != 0)
			{

				for (v = 0; v < 8; v++)
				{

					for (u = 0; u < 8; u++)
					{
						size += stream.WriteUnsignedInt(8, this.chroma_quantization_matrix[v][u], "chroma_quantization_matrix"); 
					}
				}
			}
			size += stream.WriteClass<FrameHeaderRemainder>(context, this.frame_header_remainder, "frame_header_remainder"); 

            return size;
         }

    }

    /*


stuffing() {
for (m = 0; m < stuffing_size; m++)
zero_byte /* Equal to 0x00 *//* u(8)
}
    */
    public class Stuffing : IItuSerializable
    {
		private uint[] zero_byte;
		public uint[] ZeroByte { get { return zero_byte; } set { zero_byte = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public Stuffing()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            ProResContext ituContext = context as ProResContext;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type ProResContext");

            ulong size = 0;

			uint m = 0;

			stream.CheckArrayAllocation((ulong)( ituContext.StuffingSize), "zero_byte");
			this.zero_byte = new uint[ ituContext.StuffingSize];
			for (m = 0; m < ituContext.StuffingSize; m++)
			{
				size += stream.ReadUnsignedInt(size, 8, out this.zero_byte[m ], "zero_byte"); // Equal to 0x00 
			}

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            ProResContext ituContext = context as ProResContext;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type ProResContext");
            ulong size = 0;

			uint m = 0;

			for (m = 0; m < ituContext.StuffingSize; m++)
			{
				size += stream.WriteUnsignedInt(8, this.zero_byte[m ], "zero_byte"); // Equal to 0x00 
			}

            return size;
         }

    }

    /*


picture(temporalOrder) {
picture_header()
slice_table()
slice_data()
}
    */
    public class Picture : IItuSerializable
    {
		private uint temporalOrder;
		public uint TemporalOrder { get { return temporalOrder; } set { temporalOrder = value; } }
		private PictureHeader picture_header;
		public PictureHeader PictureHeader { get { return picture_header; } set { picture_header = value; } }
		private SliceTable slice_table;
		public SliceTable SliceTable { get { return slice_table; } set { slice_table = value; } }
		private SliceData slice_data;
		public SliceData SliceData { get { return slice_data; } set { slice_data = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public Picture(uint temporalOrder)
         { 
			this.temporalOrder = temporalOrder;
         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            ProResContext ituContext = context as ProResContext;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type ProResContext");

            ulong size = 0;

			this.picture_header =  new PictureHeader() ;
			size +=  stream.ReadClass<PictureHeader>(size, context, this.picture_header, "picture_header"); 
			this.slice_table =  new SliceTable() ;
			size +=  stream.ReadClass<SliceTable>(size, context, this.slice_table, "slice_table"); 
			this.slice_data =  new SliceData() ;
			size +=  stream.ReadClass<SliceData>(size, context, this.slice_data, "slice_data"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            ProResContext ituContext = context as ProResContext;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type ProResContext");
            ulong size = 0;

			size += stream.WriteClass<PictureHeader>(context, this.picture_header, "picture_header"); 
			size += stream.WriteClass<SliceTable>(context, this.slice_table, "slice_table"); 
			size += stream.WriteClass<SliceData>(context, this.slice_data, "slice_data"); 

            return size;
         }

    }

    /*


picture_header() {
picture_header_size u(5)
reserved u(3)
picture_size u(32)
deprecated_number_of_slices u(16)
reserved_2 u(2)
log2_desired_slice_size_in_mb u(2)
reserved_3 u(4)
picture_header_remainder()
}
    */
    public class PictureHeader : IItuSerializable
    {
		private uint picture_header_size;
		public uint PictureHeaderSize { get { return picture_header_size; } set { picture_header_size = value; } }
		private uint reserved;
		public uint Reserved { get { return reserved; } set { reserved = value; } }
		private uint picture_size;
		public uint PictureSize { get { return picture_size; } set { picture_size = value; } }
		private uint deprecated_number_of_slices;
		public uint DeprecatedNumberOfSlices { get { return deprecated_number_of_slices; } set { deprecated_number_of_slices = value; } }
		private uint reserved_2;
		public uint Reserved2 { get { return reserved_2; } set { reserved_2 = value; } }
		private uint log2_desired_slice_size_in_mb;
		public uint Log2DesiredSliceSizeInMb { get { return log2_desired_slice_size_in_mb; } set { log2_desired_slice_size_in_mb = value; } }
		private uint reserved_3;
		public uint Reserved3 { get { return reserved_3; } set { reserved_3 = value; } }
		private PictureHeaderRemainder picture_header_remainder;
		public PictureHeaderRemainder PictureHeaderRemainder { get { return picture_header_remainder; } set { picture_header_remainder = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public PictureHeader()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            ProResContext ituContext = context as ProResContext;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type ProResContext");

            ulong size = 0;

			size += stream.ReadUnsignedInt(size, 5, out this.picture_header_size, "picture_header_size"); 
			ituContext.OnPictureHeader(picture_header_size);
			size += stream.ReadUnsignedInt(size, 3, out this.reserved, "reserved"); 
			size += stream.ReadUnsignedInt(size, 32, out this.picture_size, "picture_size"); 
			ituContext.OnPictureSize(picture_size);
			size += stream.ReadUnsignedInt(size, 16, out this.deprecated_number_of_slices, "deprecated_number_of_slices"); 
			size += stream.ReadUnsignedInt(size, 2, out this.reserved_2, "reserved_2"); 
			size += stream.ReadUnsignedInt(size, 2, out this.log2_desired_slice_size_in_mb, "log2_desired_slice_size_in_mb"); 
			ituContext.Log2DesiredSliceSizeInMb = log2_desired_slice_size_in_mb;
			size += stream.ReadUnsignedInt(size, 4, out this.reserved_3, "reserved_3"); 
			this.picture_header_remainder =  new PictureHeaderRemainder() ;
			size +=  stream.ReadClass<PictureHeaderRemainder>(size, context, this.picture_header_remainder, "picture_header_remainder"); 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            ProResContext ituContext = context as ProResContext;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type ProResContext");
            ulong size = 0;

			size += stream.WriteUnsignedInt(5, this.picture_header_size, "picture_header_size"); 
			ituContext.OnPictureHeader(picture_header_size);
			size += stream.WriteUnsignedInt(3, this.reserved, "reserved"); 
			size += stream.WriteUnsignedInt(32, this.picture_size, "picture_size"); 
			ituContext.OnPictureSize(picture_size);
			size += stream.WriteUnsignedInt(16, this.deprecated_number_of_slices, "deprecated_number_of_slices"); 
			size += stream.WriteUnsignedInt(2, this.reserved_2, "reserved_2"); 
			size += stream.WriteUnsignedInt(2, this.log2_desired_slice_size_in_mb, "log2_desired_slice_size_in_mb"); 
			ituContext.Log2DesiredSliceSizeInMb = log2_desired_slice_size_in_mb;
			size += stream.WriteUnsignedInt(4, this.reserved_3, "reserved_3"); 
			size += stream.WriteClass<PictureHeaderRemainder>(context, this.picture_header_remainder, "picture_header_remainder"); 

            return size;
         }

    }

    /*


slice_table() {
for (i = 0; i < height_in_mb; i++)
for (j = 0; j < number_of_slices_per_mb_row; j++)
coded_size_of_slice[i][j] u(16)
}
    */
    public class SliceTable : IItuSerializable
    {
		private uint[][] coded_size_of_slice;
		public uint[][] CodedSizeOfSlice { get { return coded_size_of_slice; } set { coded_size_of_slice = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public SliceTable()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            ProResContext ituContext = context as ProResContext;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type ProResContext");

            ulong size = 0;

			uint i = 0;
			uint j = 0;

			stream.CheckArrayAllocation((ulong)( ituContext.HeightInMb), "coded_size_of_slice");
			this.coded_size_of_slice = new uint[ ituContext.HeightInMb][];
			for (i = 0; i < ituContext.HeightInMb; i++)
			{

				stream.CheckArrayAllocation((ulong)( ituContext.NumberOfSlicesPerMbRow), "coded_size_of_slice[i ]");
				this.coded_size_of_slice[i ] = new uint[ ituContext.NumberOfSlicesPerMbRow];
				for (j = 0; j < ituContext.NumberOfSlicesPerMbRow; j++)
				{
					size += stream.ReadUnsignedInt(size, 16, out this.coded_size_of_slice[i][j], "coded_size_of_slice"); 
				}
			}

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            ProResContext ituContext = context as ProResContext;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type ProResContext");
            ulong size = 0;

			uint i = 0;
			uint j = 0;

			for (i = 0; i < ituContext.HeightInMb; i++)
			{

				for (j = 0; j < ituContext.NumberOfSlicesPerMbRow; j++)
				{
					size += stream.WriteUnsignedInt(16, this.coded_size_of_slice[i][j], "coded_size_of_slice"); 
				}
			}

            return size;
         }

    }

}
