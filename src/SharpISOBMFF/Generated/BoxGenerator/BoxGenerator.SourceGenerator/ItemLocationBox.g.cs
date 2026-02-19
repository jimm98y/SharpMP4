using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ItemLocationBox extends FullBox('iloc', version, 0) {
	unsigned int(4)	offset_size;
	unsigned int(4)	length_size;
	unsigned int(4)	base_offset_size;
	if ((version == 1) || (version == 2)) {
		unsigned int(4)	index_size;
	} else {
		unsigned int(4)	reserved;
	}
	if (version < 2) {
		unsigned int(16)	item_count;
	} else if (version == 2) {
		unsigned int(32)	item_count;
	}
	for (i=0; i<item_count; i++) {
		if (version < 2) {
			unsigned int(16)	item_ID;
		} else if (version == 2) {
			unsigned int(32)	item_ID;
		}
		if ((version == 1) || (version == 2)) {
			unsigned int(12)	reserved = 0;
			unsigned int(4)	construction_method;
		}
		unsigned int(16)	data_reference_index;
		unsigned int(base_offset_size*8)	base_offset;
		unsigned int(16)		extent_count;
		for (j=0; j<extent_count; j++) {
			if (((version == 1) || (version == 2)) && (index_size > 0)) {
				unsigned int(index_size*8)	item_reference_index;
			}
			unsigned int(offset_size*8)	extent_offset;
			unsigned int(length_size*8)	extent_length;
		}
	}
}
*/
public partial class ItemLocationBox : FullBox
{
	public const string TYPE = "iloc";
	public override string DisplayName { get { return "ItemLocationBox"; } }

	protected byte offset_size; 
	public byte OffsetSize { get { return this.offset_size; } set { this.offset_size = value; } }

	protected byte length_size; 
	public byte LengthSize { get { return this.length_size; } set { this.length_size = value; } }

	protected byte base_offset_size; 
	public byte BaseOffsetSize { get { return this.base_offset_size; } set { this.base_offset_size = value; } }

	protected byte index_size; 
	public byte IndexSize { get { return this.index_size; } set { this.index_size = value; } }

	protected byte reserved; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected uint item_count; 
	public uint ItemCount { get { return this.item_count; } set { this.item_count = value; } }

	protected uint[] item_ID; 
	public uint[] ItemID { get { return this.item_ID; } set { this.item_ID = value; } }

	protected ushort[] reserved0; 
	public ushort[] Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected byte[] construction_method; 
	public byte[] ConstructionMethod { get { return this.construction_method; } set { this.construction_method = value; } }

	protected ushort[] data_reference_index; 
	public ushort[] DataReferenceIndex { get { return this.data_reference_index; } set { this.data_reference_index = value; } }

	protected byte[][] base_offset; 
	public byte[][] BaseOffset { get { return this.base_offset; } set { this.base_offset = value; } }

	protected ushort[] extent_count; 
	public ushort[] ExtentCount { get { return this.extent_count; } set { this.extent_count = value; } }

	protected byte[][][] item_reference_index; 
	public byte[][][] ItemReferenceIndex { get { return this.item_reference_index; } set { this.item_reference_index = value; } }

	protected byte[][][] extent_offset; 
	public byte[][][] ExtentOffset { get { return this.extent_offset; } set { this.extent_offset = value; } }

	protected byte[][][] extent_length; 
	public byte[][][] ExtentLength { get { return this.extent_length; } set { this.extent_length = value; } }

	public ItemLocationBox(byte version = 0): base(IsoStream.FromFourCC("iloc"), version, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.offset_size, "offset_size"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.length_size, "length_size"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.base_offset_size, "base_offset_size"); 

		if ((version == 1) || (version == 2))
		{
			boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.index_size, "index_size"); 
		}

		else 
		{
			boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.reserved, "reserved"); 
		}

		if (version < 2)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.item_count, "item_count"); 
		}

		else if (version == 2)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.item_count, "item_count"); 
		}

		this.item_ID = new uint[IsoStream.GetInt(item_count)];
		this.reserved0 = new ushort[IsoStream.GetInt(item_count)];
		this.construction_method = new byte[IsoStream.GetInt(item_count)];
		this.data_reference_index = new ushort[IsoStream.GetInt(item_count)];
		this.base_offset = new byte[IsoStream.GetInt(item_count)][];
		this.extent_count = new ushort[IsoStream.GetInt(item_count)];
		this.item_reference_index = new byte[IsoStream.GetInt(item_count)][][];
		this.extent_offset = new byte[IsoStream.GetInt(item_count)][][];
		this.extent_length = new byte[IsoStream.GetInt(item_count)][][];
		for (int i=0; i<item_count; i++)
		{

			if (version < 2)
			{
				boxSize += stream.ReadUInt16(boxSize, readSize,  out this.item_ID[i], "item_ID"); 
			}

			else if (version == 2)
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.item_ID[i], "item_ID"); 
			}

			if ((version == 1) || (version == 2))
			{
				boxSize += stream.ReadBits(boxSize, readSize, 12,  out this.reserved0[i], "reserved0"); 
				boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.construction_method[i], "construction_method"); 
			}
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.data_reference_index[i], "data_reference_index"); 
			boxSize += stream.ReadBits(boxSize, readSize, (uint)(base_offset_size*8 ),  out this.base_offset[i], "base_offset"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.extent_count[i], "extent_count"); 

			this.item_reference_index[i] = new byte[IsoStream.GetInt(extent_count[i])][];
			this.extent_offset[i] = new byte[IsoStream.GetInt(extent_count[i])][];
			this.extent_length[i] = new byte[IsoStream.GetInt(extent_count[i])][];
			for (int j=0; j<extent_count[i]; j++)
			{

				if (((version == 1) || (version == 2)) && (index_size > 0))
				{
					boxSize += stream.ReadBits(boxSize, readSize, (uint)(index_size*8 ),  out this.item_reference_index[i][j], "item_reference_index"); 
				}
				boxSize += stream.ReadBits(boxSize, readSize, (uint)(offset_size*8 ),  out this.extent_offset[i][j], "extent_offset"); 
				boxSize += stream.ReadBits(boxSize, readSize, (uint)(length_size*8 ),  out this.extent_length[i][j], "extent_length"); 
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(4,  this.offset_size, "offset_size"); 
		boxSize += stream.WriteBits(4,  this.length_size, "length_size"); 
		boxSize += stream.WriteBits(4,  this.base_offset_size, "base_offset_size"); 

		if ((version == 1) || (version == 2))
		{
			boxSize += stream.WriteBits(4,  this.index_size, "index_size"); 
		}

		else 
		{
			boxSize += stream.WriteBits(4,  this.reserved, "reserved"); 
		}

		if (version < 2)
		{
			boxSize += stream.WriteUInt16( this.item_count, "item_count"); 
		}

		else if (version == 2)
		{
			boxSize += stream.WriteUInt32( this.item_count, "item_count"); 
		}

		for (int i=0; i<item_count; i++)
		{

			if (version < 2)
			{
				boxSize += stream.WriteUInt16( this.item_ID[i], "item_ID"); 
			}

			else if (version == 2)
			{
				boxSize += stream.WriteUInt32( this.item_ID[i], "item_ID"); 
			}

			if ((version == 1) || (version == 2))
			{
				boxSize += stream.WriteBits(12,  this.reserved0[i], "reserved0"); 
				boxSize += stream.WriteBits(4,  this.construction_method[i], "construction_method"); 
			}
			boxSize += stream.WriteUInt16( this.data_reference_index[i], "data_reference_index"); 
			boxSize += stream.WriteBits((uint)(base_offset_size*8 ),  this.base_offset[i], "base_offset"); 
			boxSize += stream.WriteUInt16( this.extent_count[i], "extent_count"); 

			for (int j=0; j<extent_count[i]; j++)
			{

				if (((version == 1) || (version == 2)) && (index_size > 0))
				{
					boxSize += stream.WriteBits((uint)(index_size*8 ),  this.item_reference_index[i][j], "item_reference_index"); 
				}
				boxSize += stream.WriteBits((uint)(offset_size*8 ),  this.extent_offset[i][j], "extent_offset"); 
				boxSize += stream.WriteBits((uint)(length_size*8 ),  this.extent_length[i][j], "extent_length"); 
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 4; // offset_size
		boxSize += 4; // length_size
		boxSize += 4; // base_offset_size

		if ((version == 1) || (version == 2))
		{
			boxSize += 4; // index_size
		}

		else 
		{
			boxSize += 4; // reserved
		}

		if (version < 2)
		{
			boxSize += 16; // item_count
		}

		else if (version == 2)
		{
			boxSize += 32; // item_count
		}

		for (int i=0; i<item_count; i++)
		{

			if (version < 2)
			{
				boxSize += 16; // item_ID
			}

			else if (version == 2)
			{
				boxSize += 32; // item_ID
			}

			if ((version == 1) || (version == 2))
			{
				boxSize += 12; // reserved0
				boxSize += 4; // construction_method
			}
			boxSize += 16; // data_reference_index
			boxSize += (ulong)(base_offset_size*8 ); // base_offset
			boxSize += 16; // extent_count

			for (int j=0; j<extent_count[i]; j++)
			{

				if (((version == 1) || (version == 2)) && (index_size > 0))
				{
					boxSize += (ulong)(index_size*8 ); // item_reference_index
				}
				boxSize += (ulong)(offset_size*8 ); // extent_offset
				boxSize += (ulong)(length_size*8 ); // extent_length
			}
		}
		return boxSize;
	}
}

}
