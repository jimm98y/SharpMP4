using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

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
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class WipeTransitionEffectProperty
extends ItemFullProperty('wipe', version=0, flags=0) {
	unsigned int(8) transition_direction;
}"; } }
	public const string TYPE = "wipe";
	public override string DisplayName { get { return "WipeTransitionEffectProperty"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("ipco"); }


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

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("transition_direction", 8, this.transition_direction); 
		return boxSize;
	}
}

}
