using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class EntityToGroupBox('vvcb', version, flags)
extends FullBox('vvcb', version, flags) {
	unsigned int(32) group_id;
	unsigned int(32) num_entities_in_group;
	for(i=0; i<num_entities_in_group; i++)
		unsigned int(32) entity_id;
// the remaining data may be specified for a particular grouping_type
}
*/
public partial class EntityToGroupBoxvvcbDup : FullBox
{
	public const string TYPE = "vvcb";
	public override string DisplayName { get { return "EntityToGroupBoxvvcbDup"; } }

	protected uint group_id; 
	public uint GroupId { get { return this.group_id; } set { this.group_id = value; } }

	protected uint num_entities_in_group; 
	public uint NumEntitiesInGroup { get { return this.num_entities_in_group; } set { this.num_entities_in_group = value; } }

	protected uint[] entity_id;  //  the remaining data may be specified for a particular grouping_type
	public uint[] EntityId { get { return this.entity_id; } set { this.entity_id = value; } }

	public EntityToGroupBoxvvcbDup(byte version = 0, uint flags = 0): base(IsoStream.FromFourCC("vvcb"), version, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.group_id, "group_id"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.num_entities_in_group, "num_entities_in_group"); 

		this.entity_id = new uint[IsoStream.GetInt(num_entities_in_group)];
		for (int i=0; i<num_entities_in_group; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entity_id[i], "entity_id"); // the remaining data may be specified for a particular grouping_type
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.group_id, "group_id"); 
		boxSize += stream.WriteUInt32( this.num_entities_in_group, "num_entities_in_group"); 

		for (int i=0; i<num_entities_in_group; i++)
		{
			boxSize += stream.WriteUInt32( this.entity_id[i], "entity_id"); // the remaining data may be specified for a particular grouping_type
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // group_id
		boxSize += 32; // num_entities_in_group

		for (int i=0; i<num_entities_in_group; i++)
		{
			boxSize += 32; // entity_id
		}
		return boxSize;
	}
}

}
