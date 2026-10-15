using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// ISO/IEC 23001-17 uncompressed frame configuration: of version 1 a profile alone, of version 0 the components, how they are sampled, interleaved, laid in blocks and aligned, and the tiles
aligned(8) class UncompressedFrameConfigurationBox() extends FullBox('uncC') {
	 unsigned int(32) profile;
 if (version == 0) {
 unsigned int(32) component_count;
 for (i = 0; i < component_count; i++) {
 unsigned int(16) component_index;
 unsigned int(8) component_bit_depth_minus_one;
 unsigned int(8) component_format;
 unsigned int(8) component_align_size;
 }
 unsigned int(8) sampling_type;
 unsigned int(8) interleave_type;
 unsigned int(8) block_size;
 unsigned int(1) components_little_endian;
 unsigned int(1) block_pad_lsb;
 unsigned int(1) block_little_endian;
 unsigned int(1) block_reversed;
 unsigned int(1) pad_unknown;
 unsigned int(3) reserved;
 unsigned int(32) pixel_size;
 unsigned int(32) row_align_size;
 unsigned int(32) tile_align_size;
 unsigned int(32) num_tile_cols_minus_one;
 unsigned int(32) num_tile_rows_minus_one;
 }
 } 
*/
public partial class UncompressedFrameConfigurationBox : FullBox
{
	public const string TYPE = "uncC";
	public override string DisplayName { get { return "UncompressedFrameConfigurationBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("VisualSampleEntry(uncv)", "ipco"); }


	protected uint profile; 
	public uint Profile { get { return this.profile; } set { this.profile = value; } }

	protected uint component_count; 
	public uint ComponentCount { get { return this.component_count; } set { this.component_count = value; } }

	protected ushort[] component_index; 
	public ushort[] ComponentIndex { get { return this.component_index; } set { this.component_index = value; } }

	protected byte[] component_bit_depth_minus_one; 
	public byte[] ComponentBitDepthMinusOne { get { return this.component_bit_depth_minus_one; } set { this.component_bit_depth_minus_one = value; } }

	protected byte[] component_format; 
	public byte[] ComponentFormat { get { return this.component_format; } set { this.component_format = value; } }

	protected byte[] component_align_size; 
	public byte[] ComponentAlignSize { get { return this.component_align_size; } set { this.component_align_size = value; } }

	protected byte sampling_type; 
	public byte SamplingType { get { return this.sampling_type; } set { this.sampling_type = value; } }

	protected byte interleave_type; 
	public byte InterleaveType { get { return this.interleave_type; } set { this.interleave_type = value; } }

	protected byte block_size; 
	public byte BlockSize { get { return this.block_size; } set { this.block_size = value; } }

	protected bool components_little_endian; 
	public bool ComponentsLittleEndian { get { return this.components_little_endian; } set { this.components_little_endian = value; } }

	protected bool block_pad_lsb; 
	public bool BlockPadLsb { get { return this.block_pad_lsb; } set { this.block_pad_lsb = value; } }

	protected bool block_little_endian; 
	public bool BlockLittleEndian { get { return this.block_little_endian; } set { this.block_little_endian = value; } }

	protected bool block_reversed; 
	public bool BlockReversed { get { return this.block_reversed; } set { this.block_reversed = value; } }

	protected bool pad_unknown; 
	public bool PadUnknown { get { return this.pad_unknown; } set { this.pad_unknown = value; } }

	protected byte reserved; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected uint pixel_size; 
	public uint PixelSize { get { return this.pixel_size; } set { this.pixel_size = value; } }

	protected uint row_align_size; 
	public uint RowAlignSize { get { return this.row_align_size; } set { this.row_align_size = value; } }

	protected uint tile_align_size; 
	public uint TileAlignSize { get { return this.tile_align_size; } set { this.tile_align_size = value; } }

	protected uint num_tile_cols_minus_one; 
	public uint NumTileColsMinusOne { get { return this.num_tile_cols_minus_one; } set { this.num_tile_cols_minus_one = value; } }

	protected uint num_tile_rows_minus_one; 
	public uint NumTileRowsMinusOne { get { return this.num_tile_rows_minus_one; } set { this.num_tile_rows_minus_one = value; } }

	public UncompressedFrameConfigurationBox(): base(IsoStream.FromFourCC("uncC"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.profile, "profile"); 

		if (version == 0)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.component_count, "component_count"); 

			this.component_index = stream.SafeAllocate<ushort>(boxSize, readSize, IsoStream.GetInt( component_count), "component_index");
			this.component_bit_depth_minus_one = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt( component_count), "component_bit_depth_minus_one");
			this.component_format = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt( component_count), "component_format");
			this.component_align_size = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt( component_count), "component_align_size");
			for (int i = 0; i < component_count; i++)
			{
				boxSize += stream.ReadUInt16(boxSize, readSize,  out this.component_index[i], "component_index"); 
				boxSize += stream.ReadUInt8(boxSize, readSize,  out this.component_bit_depth_minus_one[i], "component_bit_depth_minus_one"); 
				boxSize += stream.ReadUInt8(boxSize, readSize,  out this.component_format[i], "component_format"); 
				boxSize += stream.ReadUInt8(boxSize, readSize,  out this.component_align_size[i], "component_align_size"); 
			}
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.sampling_type, "sampling_type"); 
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.interleave_type, "interleave_type"); 
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.block_size, "block_size"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.components_little_endian, "components_little_endian"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.block_pad_lsb, "block_pad_lsb"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.block_little_endian, "block_little_endian"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.block_reversed, "block_reversed"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.pad_unknown, "pad_unknown"); 
			boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.reserved, "reserved"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.pixel_size, "pixel_size"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.row_align_size, "row_align_size"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.tile_align_size, "tile_align_size"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.num_tile_cols_minus_one, "num_tile_cols_minus_one"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.num_tile_rows_minus_one, "num_tile_rows_minus_one"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.profile, "profile"); 

		if (version == 0)
		{
			boxSize += stream.WriteUInt32( this.component_count, "component_count"); 

			for (int i = 0; i < component_count; i++)
			{
				boxSize += stream.WriteUInt16( this.component_index[i], "component_index"); 
				boxSize += stream.WriteUInt8( this.component_bit_depth_minus_one[i], "component_bit_depth_minus_one"); 
				boxSize += stream.WriteUInt8( this.component_format[i], "component_format"); 
				boxSize += stream.WriteUInt8( this.component_align_size[i], "component_align_size"); 
			}
			boxSize += stream.WriteUInt8( this.sampling_type, "sampling_type"); 
			boxSize += stream.WriteUInt8( this.interleave_type, "interleave_type"); 
			boxSize += stream.WriteUInt8( this.block_size, "block_size"); 
			boxSize += stream.WriteBit( this.components_little_endian, "components_little_endian"); 
			boxSize += stream.WriteBit( this.block_pad_lsb, "block_pad_lsb"); 
			boxSize += stream.WriteBit( this.block_little_endian, "block_little_endian"); 
			boxSize += stream.WriteBit( this.block_reversed, "block_reversed"); 
			boxSize += stream.WriteBit( this.pad_unknown, "pad_unknown"); 
			boxSize += stream.WriteBits(3,  this.reserved, "reserved"); 
			boxSize += stream.WriteUInt32( this.pixel_size, "pixel_size"); 
			boxSize += stream.WriteUInt32( this.row_align_size, "row_align_size"); 
			boxSize += stream.WriteUInt32( this.tile_align_size, "tile_align_size"); 
			boxSize += stream.WriteUInt32( this.num_tile_cols_minus_one, "num_tile_cols_minus_one"); 
			boxSize += stream.WriteUInt32( this.num_tile_rows_minus_one, "num_tile_rows_minus_one"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // profile

		if (version == 0)
		{
			boxSize += 32; // component_count

			for (int i = 0; i < component_count; i++)
			{
				boxSize += 16; // component_index
				boxSize += 8; // component_bit_depth_minus_one
				boxSize += 8; // component_format
				boxSize += 8; // component_align_size
			}
			boxSize += 8; // sampling_type
			boxSize += 8; // interleave_type
			boxSize += 8; // block_size
			boxSize += 1; // components_little_endian
			boxSize += 1; // block_pad_lsb
			boxSize += 1; // block_little_endian
			boxSize += 1; // block_reversed
			boxSize += 1; // pad_unknown
			boxSize += 3; // reserved
			boxSize += 32; // pixel_size
			boxSize += 32; // row_align_size
			boxSize += 32; // tile_align_size
			boxSize += 32; // num_tile_cols_minus_one
			boxSize += 32; // num_tile_rows_minus_one
		}
		return boxSize;
	}

	public override ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += base.ToString(builder);
		boxSize += builder.Field("profile", 32, this.profile); 

		if (version == 0)
		{
			boxSize += builder.Field("component_count", 32, this.component_count); 

			for (int i = 0; i < component_count; i++)
			{
				boxSize += builder.Field($"component_index[{i}]", 16, this.component_index[i]); 
				boxSize += builder.Field($"component_bit_depth_minus_one[{i}]", 8, this.component_bit_depth_minus_one[i]); 
				boxSize += builder.Field($"component_format[{i}]", 8, this.component_format[i]); 
				boxSize += builder.Field($"component_align_size[{i}]", 8, this.component_align_size[i]); 
			}
			boxSize += builder.Field("sampling_type", 8, this.sampling_type); 
			boxSize += builder.Field("interleave_type", 8, this.interleave_type); 
			boxSize += builder.Field("block_size", 8, this.block_size); 
			boxSize += builder.Field("components_little_endian", 1, this.components_little_endian); 
			boxSize += builder.Field("block_pad_lsb", 1, this.block_pad_lsb); 
			boxSize += builder.Field("block_little_endian", 1, this.block_little_endian); 
			boxSize += builder.Field("block_reversed", 1, this.block_reversed); 
			boxSize += builder.Field("pad_unknown", 1, this.pad_unknown); 
			boxSize += builder.Field("reserved", 3, this.reserved); 
			boxSize += builder.Field("pixel_size", 32, this.pixel_size); 
			boxSize += builder.Field("row_align_size", 32, this.row_align_size); 
			boxSize += builder.Field("tile_align_size", 32, this.tile_align_size); 
			boxSize += builder.Field("num_tile_cols_minus_one", 32, this.num_tile_cols_minus_one); 
			boxSize += builder.Field("num_tile_rows_minus_one", 32, this.num_tile_rows_minus_one); 
		}
		return boxSize;
	}
}

}
