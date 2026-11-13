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
aligned(8) class EquirectangularProjection extends ProjectionDataBox('equi', 0, 0) {
	unsigned int(32) projection_bounds_top;
	unsigned int(32) projection_bounds_bottom;
	unsigned int(32) projection_bounds_left;
	unsigned int(32) projection_bounds_right;
}
*/
public partial class EquirectangularProjection : ProjectionDataBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class EquirectangularProjection extends ProjectionDataBox('equi', 0, 0) {
	unsigned int(32) projection_bounds_top;
	unsigned int(32) projection_bounds_bottom;
	unsigned int(32) projection_bounds_left;
	unsigned int(32) projection_bounds_right;
}"; } }
	public const string TYPE = "equi";
	public override string DisplayName { get { return "EquirectangularProjection"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("proj"); }


	protected uint projection_bounds_top; 
	public uint ProjectionBoundsTop { get { return this.projection_bounds_top; } set { this.projection_bounds_top = value; } }

	protected uint projection_bounds_bottom; 
	public uint ProjectionBoundsBottom { get { return this.projection_bounds_bottom; } set { this.projection_bounds_bottom = value; } }

	protected uint projection_bounds_left; 
	public uint ProjectionBoundsLeft { get { return this.projection_bounds_left; } set { this.projection_bounds_left = value; } }

	protected uint projection_bounds_right; 
	public uint ProjectionBoundsRight { get { return this.projection_bounds_right; } set { this.projection_bounds_right = value; } }

	public EquirectangularProjection(): base(IsoStream.FromFourCC("equi"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.projection_bounds_top, "projection_bounds_top"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.projection_bounds_bottom, "projection_bounds_bottom"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.projection_bounds_left, "projection_bounds_left"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.projection_bounds_right, "projection_bounds_right"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.projection_bounds_top, "projection_bounds_top"); 
		boxSize += stream.WriteUInt32( this.projection_bounds_bottom, "projection_bounds_bottom"); 
		boxSize += stream.WriteUInt32( this.projection_bounds_left, "projection_bounds_left"); 
		boxSize += stream.WriteUInt32( this.projection_bounds_right, "projection_bounds_right"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // projection_bounds_top
		boxSize += 32; // projection_bounds_bottom
		boxSize += 32; // projection_bounds_left
		boxSize += 32; // projection_bounds_right
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("projection_bounds_top", 32, this.projection_bounds_top); 
		boxSize += tracer.Field("projection_bounds_bottom", 32, this.projection_bounds_bottom); 
		boxSize += tracer.Field("projection_bounds_left", 32, this.projection_bounds_left); 
		boxSize += tracer.Field("projection_bounds_right", 32, this.projection_bounds_right); 
		return boxSize;
	}
}

}
