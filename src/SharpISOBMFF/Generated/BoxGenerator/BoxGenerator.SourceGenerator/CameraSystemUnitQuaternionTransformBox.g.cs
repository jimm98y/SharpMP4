using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

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
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class CameraSystemUnitQuaternionTransformBox extends 
FullBox('uqua', 0, 0) { 
BEFloat32 xyz0;
BEFloat32 xyz1;
BEFloat32 xyz2; 
} "; } }
	public const string TYPE = "uqua";
	public override string DisplayName { get { return "CameraSystemUnitQuaternionTransformBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("cxfm"); }


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
		boxSize += stream.ReadFloat32(boxSize, readSize,  out this.xyz0, "xyz0"); 
		boxSize += stream.ReadFloat32(boxSize, readSize,  out this.xyz1, "xyz1"); 
		boxSize += stream.ReadFloat32(boxSize, readSize,  out this.xyz2, "xyz2"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteFloat32( this.xyz0, "xyz0"); 
		boxSize += stream.WriteFloat32( this.xyz1, "xyz1"); 
		boxSize += stream.WriteFloat32( this.xyz2, "xyz2"); 
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

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("xyz0", 32, this.xyz0); 
		boxSize += tracer.Field("xyz1", 32, this.xyz1); 
		boxSize += tracer.Field("xyz2", 32, this.xyz2); 
		return boxSize;
	}
}

}
