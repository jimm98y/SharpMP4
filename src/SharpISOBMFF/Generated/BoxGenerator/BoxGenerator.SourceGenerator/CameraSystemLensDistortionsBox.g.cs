using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class CameraSystemLensDistortionsBox extends FullBox('ldst', version = 0, flags) { 
BEFloat32 k1; // radial parameter k1 
BEFloat32 k2; // radial parameter k2 
BEFloat32 p1; // tangential parameter p1 
BEFloat32 p2; // tangential parameter p2 
if (flags & 1) { 
BEFloat32 calibration_limit_radial_angle; 
} 
}
*/
public partial class CameraSystemLensDistortionsBox : FullBox
{
	public const string TYPE = "ldst";
	public override string DisplayName { get { return "CameraSystemLensDistortionsBox"; } }

	protected double k1;  //  radial parameter k1 
	public double K1 { get { return this.k1; } set { this.k1 = value; } }

	protected double k2;  //  radial parameter k2 
	public double K2 { get { return this.k2; } set { this.k2 = value; } }

	protected double p1;  //  tangential parameter p1 
	public double P1 { get { return this.p1; } set { this.p1 = value; } }

	protected double p2;  //  tangential parameter p2 
	public double P2 { get { return this.p2; } set { this.p2 = value; } }

	protected double calibration_limit_radial_angle; 
	public double CalibrationLimitRadialAngle { get { return this.calibration_limit_radial_angle; } set { this.calibration_limit_radial_angle = value; } }

	public CameraSystemLensDistortionsBox(uint flags = 0): base(IsoStream.FromFourCC("ldst"), 0, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadDouble32(boxSize, readSize,  out this.k1, "k1"); // radial parameter k1 
		boxSize += stream.ReadDouble32(boxSize, readSize,  out this.k2, "k2"); // radial parameter k2 
		boxSize += stream.ReadDouble32(boxSize, readSize,  out this.p1, "p1"); // tangential parameter p1 
		boxSize += stream.ReadDouble32(boxSize, readSize,  out this.p2, "p2"); // tangential parameter p2 

		if ((flags  &  1) ==  1)
		{
			boxSize += stream.ReadDouble32(boxSize, readSize,  out this.calibration_limit_radial_angle, "calibration_limit_radial_angle"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteDouble32( this.k1, "k1"); // radial parameter k1 
		boxSize += stream.WriteDouble32( this.k2, "k2"); // radial parameter k2 
		boxSize += stream.WriteDouble32( this.p1, "p1"); // tangential parameter p1 
		boxSize += stream.WriteDouble32( this.p2, "p2"); // tangential parameter p2 

		if ((flags  &  1) ==  1)
		{
			boxSize += stream.WriteDouble32( this.calibration_limit_radial_angle, "calibration_limit_radial_angle"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // k1
		boxSize += 32; // k2
		boxSize += 32; // p1
		boxSize += 32; // p2

		if ((flags  &  1) ==  1)
		{
			boxSize += 32; // calibration_limit_radial_angle
		}
		return boxSize;
	}
}

}
