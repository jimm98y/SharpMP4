using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class LevelMeter2Box() extends Box('lvlm') {
 signed int(32) numerator;
 signed int(32) denominator;
}

*/
public partial class LevelMeter2Box : Box
{
	public const string TYPE = "lvlm";
	public override string DisplayName { get { return "LevelMeter2Box"; } }

	protected int numerator; 
	public int Numerator { get { return this.numerator; } set { this.numerator = value; } }

	protected int denominator; 
	public int Denominator { get { return this.denominator; } set { this.denominator = value; } }

	public LevelMeter2Box(): base(IsoStream.FromFourCC("lvlm"))
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
