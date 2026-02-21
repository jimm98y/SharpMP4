using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class SuggestedTransitionPeriodProperty
extends ItemFullProperty('stpe', version=0, flags=0) {
	unsigned int(8) transition_period;
}
*/
public partial class SuggestedTransitionPeriodProperty : ItemFullProperty
{
	public const string TYPE = "stpe";
	public override string DisplayName { get { return "SuggestedTransitionPeriodProperty"; } }

	protected byte transition_period; 
	public byte TransitionPeriod { get { return this.transition_period; } set { this.transition_period = value; } }

	public SuggestedTransitionPeriodProperty(): base(IsoStream.FromFourCC("stpe"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.transition_period, "transition_period"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.transition_period, "transition_period"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // transition_period
		return boxSize;
	}
}

}
