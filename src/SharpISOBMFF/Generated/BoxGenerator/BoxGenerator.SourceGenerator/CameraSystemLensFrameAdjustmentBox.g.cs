using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class CameraSystemLensFrameAdjustmentBox extends FullBox('lfad', version = 0, 0) { 
BEFloat32 polynomialParametersX0;
 BEFloat32 polynomialParametersX1;
 BEFloat32 polynomialParametersX2; // parameters for X axis 
BEFloat32 polynomialParametersY0;
 BEFloat32 polynomialParametersY1;
 BEFloat32 polynomialParametersY2; // parameters for Y axis 
} 
*/
public partial class CameraSystemLensFrameAdjustmentBox : FullBox
{
	public const string TYPE = "lfad";
	public override string DisplayName { get { return "CameraSystemLensFrameAdjustmentBox"; } }

	protected double polynomialParametersX0; 
	public double PolynomialParametersX0 { get { return this.polynomialParametersX0; } set { this.polynomialParametersX0 = value; } }

	protected double polynomialParametersX1; 
	public double PolynomialParametersX1 { get { return this.polynomialParametersX1; } set { this.polynomialParametersX1 = value; } }

	protected double polynomialParametersX2;  //  parameters for X axis 
	public double PolynomialParametersX2 { get { return this.polynomialParametersX2; } set { this.polynomialParametersX2 = value; } }

	protected double polynomialParametersY0; 
	public double PolynomialParametersY0 { get { return this.polynomialParametersY0; } set { this.polynomialParametersY0 = value; } }

	protected double polynomialParametersY1; 
	public double PolynomialParametersY1 { get { return this.polynomialParametersY1; } set { this.polynomialParametersY1 = value; } }

	protected double polynomialParametersY2;  //  parameters for Y axis 
	public double PolynomialParametersY2 { get { return this.polynomialParametersY2; } set { this.polynomialParametersY2 = value; } }

	public CameraSystemLensFrameAdjustmentBox(): base(IsoStream.FromFourCC("lfad"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadDouble32(boxSize, readSize,  out this.polynomialParametersX0, "polynomialParametersX0"); 
		boxSize += stream.ReadDouble32(boxSize, readSize,  out this.polynomialParametersX1, "polynomialParametersX1"); 
		boxSize += stream.ReadDouble32(boxSize, readSize,  out this.polynomialParametersX2, "polynomialParametersX2"); // parameters for X axis 
		boxSize += stream.ReadDouble32(boxSize, readSize,  out this.polynomialParametersY0, "polynomialParametersY0"); 
		boxSize += stream.ReadDouble32(boxSize, readSize,  out this.polynomialParametersY1, "polynomialParametersY1"); 
		boxSize += stream.ReadDouble32(boxSize, readSize,  out this.polynomialParametersY2, "polynomialParametersY2"); // parameters for Y axis 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteDouble32( this.polynomialParametersX0, "polynomialParametersX0"); 
		boxSize += stream.WriteDouble32( this.polynomialParametersX1, "polynomialParametersX1"); 
		boxSize += stream.WriteDouble32( this.polynomialParametersX2, "polynomialParametersX2"); // parameters for X axis 
		boxSize += stream.WriteDouble32( this.polynomialParametersY0, "polynomialParametersY0"); 
		boxSize += stream.WriteDouble32( this.polynomialParametersY1, "polynomialParametersY1"); 
		boxSize += stream.WriteDouble32( this.polynomialParametersY2, "polynomialParametersY2"); // parameters for Y axis 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // polynomialParametersX0
		boxSize += 32; // polynomialParametersX1
		boxSize += 32; // polynomialParametersX2
		boxSize += 32; // polynomialParametersY0
		boxSize += 32; // polynomialParametersY1
		boxSize += 32; // polynomialParametersY2
		return boxSize;
	}
}

}
