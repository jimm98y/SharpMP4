using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class SampleToMetadataItemEntry() 
extends SampleGroupDescriptionEntry('stmi') {
	unsigned int(32) meta_box_handler_type;
	unsigned int(32) num_items;
	for(i = 0; i < num_items; i++) {
		unsigned int(32) item_id[i];
	}
}
*/
public partial class SampleToMetadataItemEntry : SampleGroupDescriptionEntry
{
	public const string TYPE = "stmi";
	public override string DisplayName { get { return "SampleToMetadataItemEntry"; } }

	protected uint meta_box_handler_type; 
	public uint MetaBoxHandlerType { get { return this.meta_box_handler_type; } set { this.meta_box_handler_type = value; } }

	protected uint num_items; 
	public uint NumItems { get { return this.num_items; } set { this.num_items = value; } }

	protected uint[] item_id; 
	public uint[] ItemId { get { return this.item_id; } set { this.item_id = value; } }

	public SampleToMetadataItemEntry(): base(IsoStream.FromFourCC("stmi"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.meta_box_handler_type, "meta_box_handler_type"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.num_items, "num_items"); 

		this.item_id = new uint[IsoStream.GetInt( num_items)];
		for (int i = 0; i < num_items; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.item_id[i], "item_id"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.meta_box_handler_type, "meta_box_handler_type"); 
		boxSize += stream.WriteUInt32( this.num_items, "num_items"); 

		for (int i = 0; i < num_items; i++)
		{
			boxSize += stream.WriteUInt32( this.item_id[i], "item_id"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // meta_box_handler_type
		boxSize += 32; // num_items

		for (int i = 0; i < num_items; i++)
		{
			boxSize += 32; // item_id
		}
		return boxSize;
	}
}

}
