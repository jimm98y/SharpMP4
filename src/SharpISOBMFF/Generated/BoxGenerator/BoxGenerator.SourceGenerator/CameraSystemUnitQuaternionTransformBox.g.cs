using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class CameraSystemUnitQuaternionTransformBox extends 
FullBox('uqua', 0, 0) { 
BEFloat32 xyz0;
BEFloat32 xyz1;
BEFloat32 xyz2; 
} 
*/
public partial class CameraSystemUnitQuaternionTransformBox : FullBox
{
	public const string TYPE = "uqua";
	public override string DisplayName { get { return "CameraSystemUnitQuaternionTransformBox"; } }

	protected double xyz0; 
	public double Xyz0 { get { return this.xyz0; } set { this.xyz0 = value; } }

	protected double xyz1; 
	public double Xyz1 { get { return this.xyz1; } set { this.xyz1 = value; } }

	protected double xyz2; 
	public double Xyz2 { get { return this.xyz2; } set { this.xyz2 = value; } }

	public CameraSystemUnitQuaternionTransformBox(): base(IsoStream.FromFourCC("uqua"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadDouble32(boxSize, readSize,  out this.xyz0, "xyz0"); 
		boxSize += stream.ReadDouble32(boxSize, readSize,  out this.xyz1, "xyz1"); 
		boxSize += stream.ReadDouble32(boxSize, readSize,  out this.xyz2, "xyz2"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteDouble32( this.xyz0, "xyz0"); 
		boxSize += stream.WriteDouble32( this.xyz1, "xyz1"); 
		boxSize += stream.WriteDouble32( this.xyz2, "xyz2"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // xyz0
		boxSize += 32; // xyz1
		boxSize += 32; // xyz2
		return boxSize;
	}
}

}
