using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class VvcSubpicOrderEntry() extends VisualSampleGroupEntry('spor')
{
	unsigned int(1) subpic_id_info_flag;
	unsigned int(15) num_subpic_ref_idx;
	for (i = 0; i < num_subpic_ref_idx; i++)
		unsigned int(16) subp_track_ref_idx[i];
	if (subpic_id_info_flag)
		VVCSubpicIDRewritingInfomationStruct() subpic_id_rewriting_info;
}
*/
public partial class VvcSubpicOrderEntry : VisualSampleGroupEntry
{
	public const string TYPE = "spor";
	public override string DisplayName { get { return "VvcSubpicOrderEntry"; } }

	protected bool subpic_id_info_flag; 
	public bool SubpicIdInfoFlag { get { return this.subpic_id_info_flag; } set { this.subpic_id_info_flag = value; } }

	protected ushort num_subpic_ref_idx; 
	public ushort NumSubpicRefIdx { get { return this.num_subpic_ref_idx; } set { this.num_subpic_ref_idx = value; } }

	protected ushort[] subp_track_ref_idx; 
	public ushort[] SubpTrackRefIdx { get { return this.subp_track_ref_idx; } set { this.subp_track_ref_idx = value; } }

	protected VVCSubpicIDRewritingInfomationStruct subpic_id_rewriting_info; 
	public VVCSubpicIDRewritingInfomationStruct SubpicIdRewritingInfo { get { return this.subpic_id_rewriting_info; } set { this.subpic_id_rewriting_info = value; } }

	public VvcSubpicOrderEntry(): base(IsoStream.FromFourCC("spor"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.subpic_id_info_flag, "subpic_id_info_flag"); 
		boxSize += stream.ReadBits(boxSize, readSize, 15,  out this.num_subpic_ref_idx, "num_subpic_ref_idx"); 

		this.subp_track_ref_idx = new ushort[IsoStream.GetInt( num_subpic_ref_idx)];
		for (int i = 0; i < num_subpic_ref_idx; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.subp_track_ref_idx[i], "subp_track_ref_idx"); 
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
		boxSize += stream.WriteBits(15,  this.num_subpic_ref_idx, "num_subpic_ref_idx"); 

		for (int i = 0; i < num_subpic_ref_idx; i++)
		{
			boxSize += stream.WriteUInt16( this.subp_track_ref_idx[i], "subp_track_ref_idx"); 
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
		boxSize += 15; // num_subpic_ref_idx

		for (int i = 0; i < num_subpic_ref_idx; i++)
		{
			boxSize += 16; // subp_track_ref_idx
		}

		if (subpic_id_info_flag)
		{
			boxSize += IsoStream.CalculateClassSize(subpic_id_rewriting_info); // subpic_id_rewriting_info
		}
		return boxSize;
	}
}

}
