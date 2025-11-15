using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class ItemPropertyAssociationBox
	extends FullBox('ipma', version, flags)
{
	unsigned int(32) entry_count;
	for(i = 0; i < entry_count; i++) {
		if (version < 1)
			unsigned int(16)	item_ID;
		else
			unsigned int(32)	item_ID;
		unsigned int(8) association_count;
		for (j=0; j<association_count; j++) {
			bit(1) essential;
			if (flags & 1)
				unsigned int(15) property_index;
			else
				unsigned int(7) property_index;
		}
	}
}

*/
public partial class ItemPropertyAssociationBox : FullBox
{
	public const string TYPE = "ipma";
	public override string DisplayName { get { return "ItemPropertyAssociationBox"; } }

	protected uint entry_count; 
	public uint EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	protected uint[] item_ID; 
	public uint[] ItemID { get { return this.item_ID; } set { this.item_ID = value; } }

	protected byte[] association_count; 
	public byte[] AssociationCount { get { return this.association_count; } set { this.association_count = value; } }

	protected bool[][] essential; 
	public bool[][] Essential { get { return this.essential; } set { this.essential = value; } }

	protected ushort[][] property_index; 
	public ushort[][] PropertyIndex { get { return this.property_index; } set { this.property_index = value; } }

	public ItemPropertyAssociationBox(byte version = 0, uint flags = 0): base(IsoStream.FromFourCC("ipma"), version, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entry_count, "entry_count"); 

		this.item_ID = new uint[IsoStream.GetInt( entry_count)];
		this.association_count = new byte[IsoStream.GetInt( entry_count)];
		this.essential = new bool[IsoStream.GetInt( entry_count)][];
		this.property_index = new ushort[IsoStream.GetInt( entry_count)][];
		for (int i = 0; i < entry_count; i++)
		{

			if (version < 1)
			{
				boxSize += stream.ReadUInt16(boxSize, readSize,  out this.item_ID[i], "item_ID"); 
			}

			else 
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.item_ID[i], "item_ID"); 
			}
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.association_count[i], "association_count"); 

			this.essential[i] = new bool[IsoStream.GetInt(association_count[i])];
			this.property_index[i] = new ushort[IsoStream.GetInt(association_count[i])];
			for (int j=0; j<association_count[i]; j++)
			{
				boxSize += stream.ReadBit(boxSize, readSize,  out this.essential[i][j], "essential"); 

				if ((flags  &  1) ==  1)
				{
					boxSize += stream.ReadBits(boxSize, readSize, 15,  out this.property_index[i][j], "property_index"); 
				}

				else 
				{
					boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.property_index[i][j], "property_index"); 
				}
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.entry_count, "entry_count"); 

		for (int i = 0; i < entry_count; i++)
		{

			if (version < 1)
			{
				boxSize += stream.WriteUInt16( this.item_ID[i], "item_ID"); 
			}

			else 
			{
				boxSize += stream.WriteUInt32( this.item_ID[i], "item_ID"); 
			}
			boxSize += stream.WriteUInt8( this.association_count[i], "association_count"); 

			for (int j=0; j<association_count[i]; j++)
			{
				boxSize += stream.WriteBit( this.essential[i][j], "essential"); 

				if ((flags  &  1) ==  1)
				{
					boxSize += stream.WriteBits(15,  this.property_index[i][j], "property_index"); 
				}

				else 
				{
					boxSize += stream.WriteBits(7,  this.property_index[i][j], "property_index"); 
				}
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // entry_count

		for (int i = 0; i < entry_count; i++)
		{

			if (version < 1)
			{
				boxSize += 16; // item_ID
			}

			else 
			{
				boxSize += 32; // item_ID
			}
			boxSize += 8; // association_count

			for (int j=0; j<association_count[i]; j++)
			{
				boxSize += 1; // essential

				if ((flags  &  1) ==  1)
				{
					boxSize += 15; // property_index
				}

				else 
				{
					boxSize += 7; // property_index
				}
			}
		}
		return boxSize;
	}
}

}
