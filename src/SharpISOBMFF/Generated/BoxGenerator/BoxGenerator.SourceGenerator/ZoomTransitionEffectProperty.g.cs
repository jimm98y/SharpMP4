// SharpMP4
// Copyright (C) 2026 Lukas Volf
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
// SOFTWARE.

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
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class ZoomTransitionEffectProperty
	extends ItemFullProperty('zoom', version=0, flags=0) {
	unsigned int(1) transition_direction;
	unsigned int(7) transition_shape;
}"; } }
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

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("transition_direction", 1, this.transition_direction); 
		boxSize += tracer.Field("transition_shape", 7, this.transition_shape); 
		return boxSize;
	}
}

}
