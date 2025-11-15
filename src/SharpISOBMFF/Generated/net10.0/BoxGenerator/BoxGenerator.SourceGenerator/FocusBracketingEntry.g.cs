using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class FocusBracketingEntry
extends VisualSampleGroupEntry('fobr') {
	unsigned int(16) focus_distance_numerator;
	unsigned int(16) focus_distance_denominator;
}
*/
public partial class FocusBracketingEntry : VisualSampleGroupEntry
{
	public const string TYPE = "fobr";
	public override string DisplayName { get { return "FocusBracketingEntry"; } }

	protected ushort focus_distance_numerator; 
	public ushort FocusDistanceNumerator { get { return this.focus_distance_numerator; } set { this.focus_distance_numerator = value; } }

	protected ushort focus_distance_denominator; 
	public ushort FocusDistanceDenominator { get { return this.focus_distance_denominator; } set { this.focus_distance_denominator = value; } }

	public FocusBracketingEntry(): base(IsoStream.FromFourCC("fobr"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.focus_distance_numerator, "focus_distance_numerator"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.focus_distance_denominator, "focus_distance_denominator"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.focus_distance_numerator, "focus_distance_numerator"); 
		boxSize += stream.WriteUInt16( this.focus_distance_denominator, "focus_distance_denominator"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // focus_distance_numerator
		boxSize += 16; // focus_distance_denominator
		return boxSize;
	}
}

}
