using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ItemInfoBox
		extends FullBox('iinf', version, 0) {
	if (version == 0) {
		unsigned int(16)	entry_count;
	} else {
		unsigned int(32) entry_count;
	}
	ItemInfoEntry[ entry_count ]		item_infos;
}
*/
public partial class ItemInfoBox : FullBox
{
	public const string TYPE = "iinf";
	public override string DisplayName { get { return "ItemInfoBox"; } }

	protected uint entry_count; 
	public uint EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }
	public IEnumerable<ItemInfoEntry> ItemInfos { get { return this.children.OfType<ItemInfoEntry>(); } }

	public ItemInfoBox(byte version = 0): base(IsoStream.FromFourCC("iinf"), version, 0)
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
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.item_infos, "item_infos"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
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
		// boxSize += stream.WriteBox( this.item_infos, "item_infos"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
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
		// boxSize += IsoStream.CalculateBoxSize(item_infos); // item_infos
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
