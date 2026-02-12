using System;
using System.Linq;
using System.Collections.Generic;

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
	public const string TYPE = "lnin";
	public override string DisplayName { get { return "CameraSystemLensIntrinsicsBox"; } }

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
			boxSize += stream.ReadDouble32(boxSize, readSize,  out this.projection_offset, "projection_offset"); 
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
			boxSize += stream.WriteDouble32( this.projection_offset, "projection_offset"); 
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
}

}
