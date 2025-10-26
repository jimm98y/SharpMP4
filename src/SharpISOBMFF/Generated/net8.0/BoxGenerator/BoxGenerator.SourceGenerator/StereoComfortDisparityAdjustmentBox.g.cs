using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class StereoComfortDisparityAdjustmentBox extends 
FullBox('dadj') { 
int(32) disparity_adjustment; 
} 
*/
public partial class StereoComfortDisparityAdjustmentBox : FullBox
{
	public const string TYPE = "dadj";
	public override string DisplayName { get { return "StereoComfortDisparityAdjustmentBox"; } }

	protected int disparity_adjustment; 
	public int DisparityAdjustment { get { return this.disparity_adjustment; } set { this.disparity_adjustment = value; } }

	public StereoComfortDisparityAdjustmentBox(): base(IsoStream.FromFourCC("dadj"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.disparity_adjustment, "disparity_adjustment"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt32( this.disparity_adjustment, "disparity_adjustment"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // disparity_adjustment
		return boxSize;
	}
}

}
