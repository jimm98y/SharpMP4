using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class WipeTransitionEffectProperty
extends ItemFullProperty('wipe', version=0, flags=0) {
	unsigned int(8) transition_direction;
}
*/
public partial class WipeTransitionEffectProperty : ItemFullProperty
{
	public const string TYPE = "wipe";
	public override string DisplayName { get { return "WipeTransitionEffectProperty"; } }

	protected byte transition_direction; 
	public byte TransitionDirection { get { return this.transition_direction; } set { this.transition_direction = value; } }

	public WipeTransitionEffectProperty(): base(IsoStream.FromFourCC("wipe"), 0, 0)
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
