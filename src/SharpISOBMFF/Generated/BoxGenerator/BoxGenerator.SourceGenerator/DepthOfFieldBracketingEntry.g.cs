using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class DepthOfFieldBracketingEntry
extends VisualSampleGroupEntry('dobr') {
	int(8) f_stop_numerator;
	int(8) f_stop_denominator;
}
*/
public partial class DepthOfFieldBracketingEntry : VisualSampleGroupEntry
{
	public const string TYPE = "dobr";
	public override string DisplayName { get { return "DepthOfFieldBracketingEntry"; } }

	protected sbyte f_stop_numerator; 
	public sbyte fStopNumerator { get { return this.f_stop_numerator; } set { this.f_stop_numerator = value; } }

	protected sbyte f_stop_denominator; 
	public sbyte fStopDenominator { get { return this.f_stop_denominator; } set { this.f_stop_denominator = value; } }

	public DepthOfFieldBracketingEntry(): base(IsoStream.FromFourCC("dobr"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt8(boxSize, readSize,  out this.f_stop_numerator, "f_stop_numerator"); 
		boxSize += stream.ReadInt8(boxSize, readSize,  out this.f_stop_denominator, "f_stop_denominator"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt8( this.f_stop_numerator, "f_stop_numerator"); 
		boxSize += stream.WriteInt8( this.f_stop_denominator, "f_stop_denominator"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // f_stop_numerator
		boxSize += 8; // f_stop_denominator
		return boxSize;
	}
}

}
