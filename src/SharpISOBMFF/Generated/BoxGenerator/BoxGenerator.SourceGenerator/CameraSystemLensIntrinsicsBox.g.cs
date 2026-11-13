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
aligned(8) class CameraSystemLensIntrinsicsBox extends FullBox('lnin', version = 0, flags) {
	signed int(16) denominator_shift_operand;
	signed int(16) skew_denominator_shift_operand;
	signed int(32) focal_length_x;
	signed int(32) principal_point_x;
	signed int(32) principal_point_y;
	if (flags & 1) {
		signed int(32) focal_length_y;
		signed int(32) skew_factor;
	}
	if (flags & 2) {
		BEFloat32 projection_offset;
	}
}
*/
public partial class CameraSystemLensIntrinsicsBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class CameraSystemLensIntrinsicsBox extends FullBox('lnin', version = 0, flags) {
	signed int(16) denominator_shift_operand;
	signed int(16) skew_denominator_shift_operand;
	signed int(32) focal_length_x;
	signed int(32) principal_point_x;
	signed int(32) principal_point_y;
	if (flags & 1) {
		signed int(32) focal_length_y;
		signed int(32) skew_factor;
	}
	if (flags & 2) {
		BEFloat32 projection_offset;
	}
}"; } }
	public const string TYPE = "lnin";
	public override string DisplayName { get { return "CameraSystemLensIntrinsicsBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("lens"); }


	protected short denominator_shift_operand; 
	public short DenominatorShiftOperand { get { return this.denominator_shift_operand; } set { this.denominator_shift_operand = value; } }

	protected short skew_denominator_shift_operand; 
	public short SkewDenominatorShiftOperand { get { return this.skew_denominator_shift_operand; } set { this.skew_denominator_shift_operand = value; } }

	protected int focal_length_x; 
	public int FocalLengthx { get { return this.focal_length_x; } set { this.focal_length_x = value; } }

	protected int principal_point_x; 
	public int PrincipalPointx { get { return this.principal_point_x; } set { this.principal_point_x = value; } }

	protected int principal_point_y; 
	public int PrincipalPointy { get { return this.principal_point_y; } set { this.principal_point_y = value; } }

	protected int focal_length_y; 
	public int FocalLengthy { get { return this.focal_length_y; } set { this.focal_length_y = value; } }

	protected int skew_factor; 
	public int SkewFactor { get { return this.skew_factor; } set { this.skew_factor = value; } }

	protected double projection_offset; 
	public double ProjectionOffset { get { return this.projection_offset; } set { this.projection_offset = value; } }

	public CameraSystemLensIntrinsicsBox(uint flags = 0): base(IsoStream.FromFourCC("lnin"), 0, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.denominator_shift_operand, "denominator_shift_operand"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.skew_denominator_shift_operand, "skew_denominator_shift_operand"); 
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.focal_length_x, "focal_length_x"); 
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.principal_point_x, "principal_point_x"); 
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.principal_point_y, "principal_point_y"); 

		if ((flags  &  1) ==  1)
		{
			boxSize += stream.ReadInt32(boxSize, readSize,  out this.focal_length_y, "focal_length_y"); 
			boxSize += stream.ReadInt32(boxSize, readSize,  out this.skew_factor, "skew_factor"); 
		}

		if ((flags  &  2) ==  2)
		{
			boxSize += stream.ReadFloat32(boxSize, readSize,  out this.projection_offset, "projection_offset"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt16( this.denominator_shift_operand, "denominator_shift_operand"); 
		boxSize += stream.WriteInt16( this.skew_denominator_shift_operand, "skew_denominator_shift_operand"); 
		boxSize += stream.WriteInt32( this.focal_length_x, "focal_length_x"); 
		boxSize += stream.WriteInt32( this.principal_point_x, "principal_point_x"); 
		boxSize += stream.WriteInt32( this.principal_point_y, "principal_point_y"); 

		if ((flags  &  1) ==  1)
		{
			boxSize += stream.WriteInt32( this.focal_length_y, "focal_length_y"); 
			boxSize += stream.WriteInt32( this.skew_factor, "skew_factor"); 
		}

		if ((flags  &  2) ==  2)
		{
			boxSize += stream.WriteFloat32( this.projection_offset, "projection_offset"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // denominator_shift_operand
		boxSize += 16; // skew_denominator_shift_operand
		boxSize += 32; // focal_length_x
		boxSize += 32; // principal_point_x
		boxSize += 32; // principal_point_y

		if ((flags  &  1) ==  1)
		{
			boxSize += 32; // focal_length_y
			boxSize += 32; // skew_factor
		}

		if ((flags  &  2) ==  2)
		{
			boxSize += 32; // projection_offset
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("denominator_shift_operand", 16, this.denominator_shift_operand); 
		boxSize += tracer.Field("skew_denominator_shift_operand", 16, this.skew_denominator_shift_operand); 
		boxSize += tracer.Field("focal_length_x", 32, this.focal_length_x); 
		boxSize += tracer.Field("principal_point_x", 32, this.principal_point_x); 
		boxSize += tracer.Field("principal_point_y", 32, this.principal_point_y); 

		if ((flags  &  1) ==  1)
		{
			boxSize += tracer.Field("focal_length_y", 32, this.focal_length_y); 
			boxSize += tracer.Field("skew_factor", 32, this.skew_factor); 
		}

		if ((flags  &  2) ==  2)
		{
			boxSize += tracer.Field("projection_offset", 32, this.projection_offset); 
		}
		return boxSize;
	}
}

}
