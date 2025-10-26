using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class CodingConstraintsBox extends FullBox('ccst', version = 0, flags = 0){
	unsigned int(1) all_ref_pics_intra;
	unsigned int(1) intra_pred_used;
	unsigned int(4) max_ref_per_pic;
	unsigned int(26) reserved;
}

*/
public partial class CodingConstraintsBox : FullBox
{
	public const string TYPE = "ccst";
	public override string DisplayName { get { return "CodingConstraintsBox"; } }

	protected bool all_ref_pics_intra; 
	public bool AllRefPicsIntra { get { return this.all_ref_pics_intra; } set { this.all_ref_pics_intra = value; } }

	protected bool intra_pred_used; 
	public bool IntraPredUsed { get { return this.intra_pred_used; } set { this.intra_pred_used = value; } }

	protected byte max_ref_per_pic; 
	public byte MaxRefPerPic { get { return this.max_ref_per_pic; } set { this.max_ref_per_pic = value; } }

	protected uint reserved; 
	public uint Reserved { get { return this.reserved; } set { this.reserved = value; } }

	public CodingConstraintsBox(): base(IsoStream.FromFourCC("ccst"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.all_ref_pics_intra, "all_ref_pics_intra"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.intra_pred_used, "intra_pred_used"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.max_ref_per_pic, "max_ref_per_pic"); 
		boxSize += stream.ReadBits(boxSize, readSize, 26,  out this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBit( this.all_ref_pics_intra, "all_ref_pics_intra"); 
		boxSize += stream.WriteBit( this.intra_pred_used, "intra_pred_used"); 
		boxSize += stream.WriteBits(4,  this.max_ref_per_pic, "max_ref_per_pic"); 
		boxSize += stream.WriteBits(26,  this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 1; // all_ref_pics_intra
		boxSize += 1; // intra_pred_used
		boxSize += 4; // max_ref_per_pic
		boxSize += 26; // reserved
		return boxSize;
	}
}

}
