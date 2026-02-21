using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class VvcSubpicIDEntry() extends VisualSampleGroupEntry('spid')
{
	unsigned int(1) rect_region_flag;
	bit(2) reserved = 0;
	unsigned int(1) continuous_id_flag;
	unsigned int(12) num_subpics_minus1;
	for (i = 0; i <= num_subpics_minus1; i++) {
		if ((continuous_id_flag && i == 0) || !continuous_id_flag)
			unsigned int(16) subpic_id[i];
		if (rect_region_flag)
			unsigned int(16) groupID[i];
	}
}
*/
public partial class VvcSubpicIDEntry : VisualSampleGroupEntry
{
	public const string TYPE = "spid";
	public override string DisplayName { get { return "VvcSubpicIDEntry"; } }

	protected bool rect_region_flag; 
	public bool RectRegionFlag { get { return this.rect_region_flag; } set { this.rect_region_flag = value; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected bool continuous_id_flag; 
	public bool ContinuousIdFlag { get { return this.continuous_id_flag; } set { this.continuous_id_flag = value; } }

	protected ushort num_subpics_minus1; 
	public ushort NumSubpicsMinus1 { get { return this.num_subpics_minus1; } set { this.num_subpics_minus1 = value; } }

	protected ushort[] subpic_id; 
	public ushort[] SubpicId { get { return this.subpic_id; } set { this.subpic_id = value; } }

	protected ushort[] groupID; 
	public ushort[] GroupID { get { return this.groupID; } set { this.groupID = value; } }

	public VvcSubpicIDEntry(): base(IsoStream.FromFourCC("spid"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.rect_region_flag, "rect_region_flag"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.continuous_id_flag, "continuous_id_flag"); 
		boxSize += stream.ReadBits(boxSize, readSize, 12,  out this.num_subpics_minus1, "num_subpics_minus1"); 

		this.subpic_id = new ushort[IsoStream.GetInt( num_subpics_minus1 + 1)];
		this.groupID = new ushort[IsoStream.GetInt( num_subpics_minus1 + 1)];
		for (int i = 0; i <= num_subpics_minus1; i++)
		{

			if ((continuous_id_flag && i == 0) || !continuous_id_flag)
			{
				boxSize += stream.ReadUInt16(boxSize, readSize,  out this.subpic_id[i], "subpic_id"); 
			}

			if (rect_region_flag)
			{
				boxSize += stream.ReadUInt16(boxSize, readSize,  out this.groupID[i], "groupID"); 
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBit( this.rect_region_flag, "rect_region_flag"); 
		boxSize += stream.WriteBits(2,  this.reserved, "reserved"); 
		boxSize += stream.WriteBit( this.continuous_id_flag, "continuous_id_flag"); 
		boxSize += stream.WriteBits(12,  this.num_subpics_minus1, "num_subpics_minus1"); 

		for (int i = 0; i <= num_subpics_minus1; i++)
		{

			if ((continuous_id_flag && i == 0) || !continuous_id_flag)
			{
				boxSize += stream.WriteUInt16( this.subpic_id[i], "subpic_id"); 
			}

			if (rect_region_flag)
			{
				boxSize += stream.WriteUInt16( this.groupID[i], "groupID"); 
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 1; // rect_region_flag
		boxSize += 2; // reserved
		boxSize += 1; // continuous_id_flag
		boxSize += 12; // num_subpics_minus1

		for (int i = 0; i <= num_subpics_minus1; i++)
		{

			if ((continuous_id_flag && i == 0) || !continuous_id_flag)
			{
				boxSize += 16; // subpic_id
			}

			if (rect_region_flag)
			{
				boxSize += 16; // groupID
			}
		}
		return boxSize;
	}
}

}
