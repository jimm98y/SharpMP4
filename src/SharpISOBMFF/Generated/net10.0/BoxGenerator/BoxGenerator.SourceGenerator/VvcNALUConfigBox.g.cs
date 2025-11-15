using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class VvcNALUConfigBox extends FullBox('vvnC',version=0,flags) {
	unsigned int(6) reserved=0;
	unsigned int(2) LengthSizeMinusOne;
}
*/
public partial class VvcNALUConfigBox : FullBox
{
	public const string TYPE = "vvnC";
	public override string DisplayName { get { return "VvcNALUConfigBox"; } }

	protected byte reserved =0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte LengthSizeMinusOne; 
	public byte _LengthSizeMinusOne { get { return this.LengthSizeMinusOne; } set { this.LengthSizeMinusOne = value; } }

	public VvcNALUConfigBox(uint flags = 0): base(IsoStream.FromFourCC("vvnC"), 0, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.LengthSizeMinusOne, "LengthSizeMinusOne"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(6,  this.reserved, "reserved"); 
		boxSize += stream.WriteBits(2,  this.LengthSizeMinusOne, "LengthSizeMinusOne"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 6; // reserved
		boxSize += 2; // LengthSizeMinusOne
		return boxSize;
	}
}

}
