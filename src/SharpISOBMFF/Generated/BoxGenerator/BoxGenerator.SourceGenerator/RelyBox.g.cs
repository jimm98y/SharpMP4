using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// an RTP hint sample entry's reliability, as GPAC's rely_box_read reads it
aligned(8) class RelyBox() extends Box('rely') {
	bit(6) reserved = 0;
	unsigned int(1) preferred;
	unsigned int(1) required;
}
*/
public partial class RelyBox : Box
{
	public const string TYPE = "rely";
	public override string DisplayName { get { return "RelyBox"; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected bool preferred; 
	public bool Preferred { get { return this.preferred; } set { this.preferred = value; } }

	protected bool required; 
	public bool Required { get { return this.required; } set { this.required = value; } }

	public RelyBox(): base(IsoStream.FromFourCC("rely"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.preferred, "preferred"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.required, "required"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(6,  this.reserved, "reserved"); 
		boxSize += stream.WriteBit( this.preferred, "preferred"); 
		boxSize += stream.WriteBit( this.required, "required"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 6; // reserved
		boxSize += 1; // preferred
		boxSize += 1; // required
		return boxSize;
	}
}

}
