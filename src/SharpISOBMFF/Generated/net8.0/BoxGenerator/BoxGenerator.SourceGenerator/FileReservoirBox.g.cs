using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class FileReservoirBox
		extends FullBox('fire', version, 0) {
	if (version == 0) {
		unsigned int(16)	entry_count;
	} else {
		unsigned int(32)	entry_count;
	}
	for (i=1; i <= entry_count; i++) {
		if (version == 0) {
			unsigned int(16)	item_ID;
		} else {
			unsigned int(32)	item_ID;
		}
		unsigned int(32)	symbol_count;
	}
}
*/
public partial class FileReservoirBox : FullBox
{
	public const string TYPE = "fire";
	public override string DisplayName { get { return "FileReservoirBox"; } }

	protected uint entry_count; 
	public uint EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	protected uint[] item_ID; 
	public uint[] ItemID { get { return this.item_ID; } set { this.item_ID = value; } }

	protected uint[] symbol_count; 
	public uint[] SymbolCount { get { return this.symbol_count; } set { this.symbol_count = value; } }

	public FileReservoirBox(byte version = 0): base(IsoStream.FromFourCC("fire"), version, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);

		if (version == 0)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.entry_count, "entry_count"); 
		}

		else 
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entry_count, "entry_count"); 
		}

		this.item_ID = new uint[IsoStream.GetInt( entry_count)];
		this.symbol_count = new uint[IsoStream.GetInt( entry_count)];
		for (int i=0; i < entry_count; i++)
		{

			if (version == 0)
			{
				boxSize += stream.ReadUInt16(boxSize, readSize,  out this.item_ID[i], "item_ID"); 
			}

			else 
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.item_ID[i], "item_ID"); 
			}
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.symbol_count[i], "symbol_count"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);

		if (version == 0)
		{
			boxSize += stream.WriteUInt16( this.entry_count, "entry_count"); 
		}

		else 
		{
			boxSize += stream.WriteUInt32( this.entry_count, "entry_count"); 
		}

		for (int i=0; i < entry_count; i++)
		{

			if (version == 0)
			{
				boxSize += stream.WriteUInt16( this.item_ID[i], "item_ID"); 
			}

			else 
			{
				boxSize += stream.WriteUInt32( this.item_ID[i], "item_ID"); 
			}
			boxSize += stream.WriteUInt32( this.symbol_count[i], "symbol_count"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();

		if (version == 0)
		{
			boxSize += 16; // entry_count
		}

		else 
		{
			boxSize += 32; // entry_count
		}

		for (int i=0; i < entry_count; i++)
		{

			if (version == 0)
			{
				boxSize += 16; // item_ID
			}

			else 
			{
				boxSize += 32; // item_ID
			}
			boxSize += 32; // symbol_count
		}
		return boxSize;
	}
}

}
