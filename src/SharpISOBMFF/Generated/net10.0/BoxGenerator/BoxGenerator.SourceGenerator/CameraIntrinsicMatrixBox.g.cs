using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class CameraIntrinsicMatrixBox() extends FullBox('cmin') {
	 signed int(32) matrix_focal_length_x;
 signed int(32) matrix_principal_point_x;
 signed int(32) matrix_principal_point_y;
 if( flags & 0x1 ) { 
 signed int(32) matrix_focal_length_y;
 signed int(32) matrix_skew;
 }
 } 
*/
public partial class CameraIntrinsicMatrixBox : FullBox
{
	public const string TYPE = "cmin";
	public override string DisplayName { get { return "CameraIntrinsicMatrixBox"; } }

	protected int matrix_focal_length_x; 
	public int MatrixFocalLengthx { get { return this.matrix_focal_length_x; } set { this.matrix_focal_length_x = value; } }

	protected int matrix_principal_point_x; 
	public int MatrixPrincipalPointx { get { return this.matrix_principal_point_x; } set { this.matrix_principal_point_x = value; } }

	protected int matrix_principal_point_y; 
	public int MatrixPrincipalPointy { get { return this.matrix_principal_point_y; } set { this.matrix_principal_point_y = value; } }

	protected int matrix_focal_length_y; 
	public int MatrixFocalLengthy { get { return this.matrix_focal_length_y; } set { this.matrix_focal_length_y = value; } }

	protected int matrix_skew; 
	public int MatrixSkew { get { return this.matrix_skew; } set { this.matrix_skew = value; } }

	public CameraIntrinsicMatrixBox(): base(IsoStream.FromFourCC("cmin"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.matrix_focal_length_x, "matrix_focal_length_x"); 
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.matrix_principal_point_x, "matrix_principal_point_x"); 
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.matrix_principal_point_y, "matrix_principal_point_y"); 

		if (( flags  &  0x1 ) ==  0x1 )
		{
			boxSize += stream.ReadInt32(boxSize, readSize,  out this.matrix_focal_length_y, "matrix_focal_length_y"); 
			boxSize += stream.ReadInt32(boxSize, readSize,  out this.matrix_skew, "matrix_skew"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt32( this.matrix_focal_length_x, "matrix_focal_length_x"); 
		boxSize += stream.WriteInt32( this.matrix_principal_point_x, "matrix_principal_point_x"); 
		boxSize += stream.WriteInt32( this.matrix_principal_point_y, "matrix_principal_point_y"); 

		if (( flags  &  0x1 ) ==  0x1 )
		{
			boxSize += stream.WriteInt32( this.matrix_focal_length_y, "matrix_focal_length_y"); 
			boxSize += stream.WriteInt32( this.matrix_skew, "matrix_skew"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // matrix_focal_length_x
		boxSize += 32; // matrix_principal_point_x
		boxSize += 32; // matrix_principal_point_y

		if (( flags  &  0x1 ) ==  0x1 )
		{
			boxSize += 32; // matrix_focal_length_y
			boxSize += 32; // matrix_skew
		}
		return boxSize;
	}
}

}
