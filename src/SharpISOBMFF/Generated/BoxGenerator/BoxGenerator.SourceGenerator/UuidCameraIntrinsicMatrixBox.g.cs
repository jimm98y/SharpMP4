using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class UuidCameraIntrinsicMatrixBox() extends FullBox('uuid 22cc04c7d6d94e079d904eb6ecbaf3a3') {
	 signed int(32) matrix_focal_length_x;
 signed int(32) matrix_principal_point_x;
 signed int(32) matrix_principal_point_y;
 if( flags & 0x1 ) { 
 signed int(32) matrix_focal_length_y;
 signed int(32) matrix_skew;
 }
 } 
*/
public partial class UuidCameraIntrinsicMatrixBox : FullBox
{
	public const string TYPE = "uuid";
	public override string DisplayName { get { return "UuidCameraIntrinsicMatrixBox"; } }

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

	public UuidCameraIntrinsicMatrixBox(): base(IsoStream.FromFourCC("uuid"), ConvertEx.FromHexString("22cc04c7d6d94e079d904eb6ecbaf3a3"))
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
