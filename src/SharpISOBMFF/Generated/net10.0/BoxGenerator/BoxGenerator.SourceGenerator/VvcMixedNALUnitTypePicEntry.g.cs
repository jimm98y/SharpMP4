using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class VvcMixedNALUnitTypePicEntry() extends VisualSampleGroupEntry('minp')
{
	unsigned int(16) num_mix_nalu_pic_idx;
	for (i = 0; i < num_mix_nalu_pic_idx; i++){
		unsigned int(16) mix_subp_track_idx1[i];
		unsigned int(16) mix_subp_track_idx2[i];
	}
	unsigned int(10) pps_mix_nalu_types_in_pic_bit_pos;
	unsigned int(6) pps_id;
}
*/
public partial class VvcMixedNALUnitTypePicEntry : VisualSampleGroupEntry
{
	public const string TYPE = "minp";
	public override string DisplayName { get { return "VvcMixedNALUnitTypePicEntry"; } }

	protected ushort num_mix_nalu_pic_idx; 
	public ushort NumMixNaluPicIdx { get { return this.num_mix_nalu_pic_idx; } set { this.num_mix_nalu_pic_idx = value; } }

	protected ushort[] mix_subp_track_idx1; 
	public ushort[] MixSubpTrackIdx1 { get { return this.mix_subp_track_idx1; } set { this.mix_subp_track_idx1 = value; } }

	protected ushort[] mix_subp_track_idx2; 
	public ushort[] MixSubpTrackIdx2 { get { return this.mix_subp_track_idx2; } set { this.mix_subp_track_idx2 = value; } }

	protected ushort pps_mix_nalu_types_in_pic_bit_pos; 
	public ushort PpsMixNaluTypesInPicBitPos { get { return this.pps_mix_nalu_types_in_pic_bit_pos; } set { this.pps_mix_nalu_types_in_pic_bit_pos = value; } }

	protected byte pps_id; 
	public byte PpsId { get { return this.pps_id; } set { this.pps_id = value; } }

	public VvcMixedNALUnitTypePicEntry(): base(IsoStream.FromFourCC("minp"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.num_mix_nalu_pic_idx, "num_mix_nalu_pic_idx"); 

		this.mix_subp_track_idx1 = new ushort[IsoStream.GetInt( num_mix_nalu_pic_idx)];
		this.mix_subp_track_idx2 = new ushort[IsoStream.GetInt( num_mix_nalu_pic_idx)];
		for (int i = 0; i < num_mix_nalu_pic_idx; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.mix_subp_track_idx1[i], "mix_subp_track_idx1"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.mix_subp_track_idx2[i], "mix_subp_track_idx2"); 
		}
		boxSize += stream.ReadBits(boxSize, readSize, 10,  out this.pps_mix_nalu_types_in_pic_bit_pos, "pps_mix_nalu_types_in_pic_bit_pos"); 
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.pps_id, "pps_id"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.num_mix_nalu_pic_idx, "num_mix_nalu_pic_idx"); 

		for (int i = 0; i < num_mix_nalu_pic_idx; i++)
		{
			boxSize += stream.WriteUInt16( this.mix_subp_track_idx1[i], "mix_subp_track_idx1"); 
			boxSize += stream.WriteUInt16( this.mix_subp_track_idx2[i], "mix_subp_track_idx2"); 
		}
		boxSize += stream.WriteBits(10,  this.pps_mix_nalu_types_in_pic_bit_pos, "pps_mix_nalu_types_in_pic_bit_pos"); 
		boxSize += stream.WriteBits(6,  this.pps_id, "pps_id"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // num_mix_nalu_pic_idx

		for (int i = 0; i < num_mix_nalu_pic_idx; i++)
		{
			boxSize += 16; // mix_subp_track_idx1
			boxSize += 16; // mix_subp_track_idx2
		}
		boxSize += 10; // pps_mix_nalu_types_in_pic_bit_pos
		boxSize += 6; // pps_id
		return boxSize;
	}
}

}
