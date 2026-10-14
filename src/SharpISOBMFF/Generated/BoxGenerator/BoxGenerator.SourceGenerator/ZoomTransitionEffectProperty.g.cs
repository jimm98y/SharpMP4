using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class ZoomTransitionEffectProperty
extends ItemFullProperty('zoom', version=0, flags=0) {
	unsigned int(1) transition_direction; 
	unsigned int(7) transition_shape;
}
*/
public partial class ZoomTransitionEffectProperty : ItemFullProperty
{
	public const string TYPE = "zoom";
	public override string DisplayName { get { return "ZoomTransitionEffectProperty"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("ipco"); }


	protected bool transition_direction; 
	public bool TransitionDirection { get { return this.transition_direction; } set { this.transition_direction = value; } }

	protected byte transition_shape; 
	public byte TransitionShape { get { return this.transition_shape; } set { this.transition_shape = value; } }

	public ZoomTransitionEffectProperty(): base(IsoStream.FromFourCC("zoom"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.transition_direction, "transition_direction"); 
		boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.transition_shape, "transition_shape"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBit( this.transition_direction, "transition_direction"); 
		boxSize += stream.WriteBits(7,  this.transition_shape, "transition_shape"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 1; // transition_direction
		boxSize += 7; // transition_shape
		return boxSize;
	}
}

}
