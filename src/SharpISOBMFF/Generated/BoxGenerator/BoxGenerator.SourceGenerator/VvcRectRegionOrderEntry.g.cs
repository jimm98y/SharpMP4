using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class VvcRectRegionOrderEntry () extends VisualSampleGroupEntry('rror')
{
	unsigned int(1) subpic_id_info_flag;
	bit(7) reserved = 0;
	unsigned int(16) num_alternate_region_set;
	for (i = 0; i < num_alternate_region_set; i++) {
		unsigned int(16) num_regions_in_set[i];
		unsigned int(16) alternate_region_set_id[i];
		for (j = 0; j < num_regions_in_set[i]; j++)
			unsigned int(16) groupID[i][j];
	}
	unsigned int(16) num_regions_minus1;
	for (i = 0; i < num_regions_minus1; i++)
		unsigned int(16) region_id[i];
	if (subpic_id_info_flag)
		VVCSubpicIDRewritingInfomationStruct() subpic_id_rewriting_info;
}
*/
public partial class VvcRectRegionOrderEntry : VisualSampleGroupEntry
{
	public const string TYPE = "rror";
	public override string DisplayName { get { return "VvcRectRegionOrderEntry"; } }

	protected bool subpic_id_info_flag; 
	public bool SubpicIdInfoFlag { get { return this.subpic_id_info_flag; } set { this.subpic_id_info_flag = value; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected ushort num_alternate_region_set; 
	public ushort NumAlternateRegionSet { get { return this.num_alternate_region_set; } set { this.num_alternate_region_set = value; } }

	protected ushort[] num_regions_in_set; 
	public ushort[] NumRegionsInSet { get { return this.num_regions_in_set; } set { this.num_regions_in_set = value; } }

	protected ushort[] alternate_region_set_id; 
	public ushort[] AlternateRegionSetId { get { return this.alternate_region_set_id; } set { this.alternate_region_set_id = value; } }

	protected ushort[][] groupID; 
	public ushort[][] GroupID { get { return this.groupID; } set { this.groupID = value; } }

	protected ushort num_regions_minus1; 
	public ushort NumRegionsMinus1 { get { return this.num_regions_minus1; } set { this.num_regions_minus1 = value; } }

	protected ushort[] region_id; 
	public ushort[] RegionId { get { return this.region_id; } set { this.region_id = value; } }

	protected VVCSubpicIDRewritingInfomationStruct subpic_id_rewriting_info; 
	public VVCSubpicIDRewritingInfomationStruct SubpicIdRewritingInfo { get { return this.subpic_id_rewriting_info; } set { this.subpic_id_rewriting_info = value; } }

	public VvcRectRegionOrderEntry(): base(IsoStream.FromFourCC("rror"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.subpic_id_info_flag, "subpic_id_info_flag"); 
		boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.num_alternate_region_set, "num_alternate_region_set"); 

		this.num_regions_in_set = new ushort[IsoStream.GetInt( num_alternate_region_set)];
		this.alternate_region_set_id = new ushort[IsoStream.GetInt( num_alternate_region_set)];
		this.groupID = new ushort[IsoStream.GetInt( num_alternate_region_set)][];
		for (int i = 0; i < num_alternate_region_set; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.num_regions_in_set[i], "num_regions_in_set"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.alternate_region_set_id[i], "alternate_region_set_id"); 

			this.groupID[i] = new ushort[IsoStream.GetInt( num_regions_in_set[i])];
			for (int j = 0; j < num_regions_in_set[i]; j++)
			{
				boxSize += stream.ReadUInt16(boxSize, readSize,  out this.groupID[i][j], "groupID"); 
			}
		}
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.num_regions_minus1, "num_regions_minus1"); 

		this.region_id = new ushort[IsoStream.GetInt( num_regions_minus1 + 1)];
		for (int i = 0; i < num_regions_minus1; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.region_id[i], "region_id"); 
		}

		if (subpic_id_info_flag)
		{
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new VVCSubpicIDRewritingInfomationStruct(),  out this.subpic_id_rewriting_info, "subpic_id_rewriting_info"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBit( this.subpic_id_info_flag, "subpic_id_info_flag"); 
		boxSize += stream.WriteBits(7,  this.reserved, "reserved"); 
		boxSize += stream.WriteUInt16( this.num_alternate_region_set, "num_alternate_region_set"); 

		for (int i = 0; i < num_alternate_region_set; i++)
		{
			boxSize += stream.WriteUInt16( this.num_regions_in_set[i], "num_regions_in_set"); 
			boxSize += stream.WriteUInt16( this.alternate_region_set_id[i], "alternate_region_set_id"); 

			for (int j = 0; j < num_regions_in_set[i]; j++)
			{
				boxSize += stream.WriteUInt16( this.groupID[i][j], "groupID"); 
			}
		}
		boxSize += stream.WriteUInt16( this.num_regions_minus1, "num_regions_minus1"); 

		for (int i = 0; i < num_regions_minus1; i++)
		{
			boxSize += stream.WriteUInt16( this.region_id[i], "region_id"); 
		}

		if (subpic_id_info_flag)
		{
			boxSize += stream.WriteClass( this.subpic_id_rewriting_info, "subpic_id_rewriting_info"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 1; // subpic_id_info_flag
		boxSize += 7; // reserved
		boxSize += 16; // num_alternate_region_set

		for (int i = 0; i < num_alternate_region_set; i++)
		{
			boxSize += 16; // num_regions_in_set
			boxSize += 16; // alternate_region_set_id

			for (int j = 0; j < num_regions_in_set[i]; j++)
			{
				boxSize += 16; // groupID
			}
		}
		boxSize += 16; // num_regions_minus1

		for (int i = 0; i < num_regions_minus1; i++)
		{
			boxSize += 16; // region_id
		}

		if (subpic_id_info_flag)
		{
			boxSize += IsoStream.CalculateClassSize(subpic_id_rewriting_info); // subpic_id_rewriting_info
		}
		return boxSize;
	}
}

}
