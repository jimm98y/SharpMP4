using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class CxBox() extends Box('_cx_') {
 signed int(32) numerator;
 signed int(32) denominator;
}
*/
public partial class CxBox : Box
{
	public const string TYPE = "_cx_";
	public override string DisplayName { get { return "CxBox"; } }

	protected int numerator; 
	public int Numerator { get { return this.numerator; } set { this.numerator = value; } }

	protected int denominator; 
	public int Denominator { get { return this.denominator; } set { this.denominator = value; } }

	public CxBox(): base(IsoStream.FromFourCC("_cx_"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.numerator, "numerator"); 
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.denominator, "denominator"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt32( this.numerator, "numerator"); 
		boxSize += stream.WriteInt32( this.denominator, "denominator"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // numerator
		boxSize += 32; // denominator
		return boxSize;
	}
}

}
