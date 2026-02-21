using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class FadeTransitionEffectProperty
extends ItemFullProperty('fade', version=0, flags=0) {
	unsigned int(8) transition_direction;
}
*/
public partial class FadeTransitionEffectProperty : ItemFullProperty
{
	public const string TYPE = "fade";
	public override string DisplayName { get { return "FadeTransitionEffectProperty"; } }

	protected byte transition_direction; 
	public byte TransitionDirection { get { return this.transition_direction; } set { this.transition_direction = value; } }

	public FadeTransitionEffectProperty(): base(IsoStream.FromFourCC("fade"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.transition_direction, "transition_direction"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.transition_direction, "transition_direction"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // transition_direction
		return boxSize;
	}
}

}
