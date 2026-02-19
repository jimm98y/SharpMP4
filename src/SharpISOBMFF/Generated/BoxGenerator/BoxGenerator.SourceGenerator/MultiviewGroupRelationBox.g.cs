using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class MultiviewGroupRelationBox() extends FullBox('swtc', version = 0, flags) {
	unsigned int(32) num_entries;
	for (i=0; i<num_entries; i++)
		unsigned int(32) multiview_group_id;
	MultiviewRelationAttributeBox relation_attributes;
}
*/
public partial class MultiviewGroupRelationBox : FullBox
{
	public const string TYPE = "swtc";
	public override string DisplayName { get { return "MultiviewGroupRelationBox"; } }

	protected uint num_entries; 
	public uint NumEntries { get { return this.num_entries; } set { this.num_entries = value; } }

	protected uint[] multiview_group_id; 
	public uint[] MultiviewGroupId { get { return this.multiview_group_id; } set { this.multiview_group_id = value; } }
	public MultiviewRelationAttributeBox RelationAttributes { get { return this.children.OfType<MultiviewRelationAttributeBox>().FirstOrDefault(); } }

	public MultiviewGroupRelationBox(uint flags = 0): base(IsoStream.FromFourCC("swtc"), 0, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.num_entries, "num_entries"); 

		this.multiview_group_id = new uint[IsoStream.GetInt(num_entries)];
		for (int i=0; i<num_entries; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.multiview_group_id[i], "multiview_group_id"); 
		}
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.relation_attributes, "relation_attributes"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.num_entries, "num_entries"); 

		for (int i=0; i<num_entries; i++)
		{
			boxSize += stream.WriteUInt32( this.multiview_group_id[i], "multiview_group_id"); 
		}
		// boxSize += stream.WriteBox( this.relation_attributes, "relation_attributes"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // num_entries

		for (int i=0; i<num_entries; i++)
		{
			boxSize += 32; // multiview_group_id
		}
		// boxSize += IsoStream.CalculateBoxSize(relation_attributes); // relation_attributes
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
