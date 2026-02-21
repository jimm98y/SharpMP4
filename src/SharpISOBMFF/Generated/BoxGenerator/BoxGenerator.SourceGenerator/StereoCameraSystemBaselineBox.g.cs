using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class StereoCameraSystemBaselineBox extends FullBox('blin') {
  unsigned int(32) baseline_value;
 }

*/
public partial class StereoCameraSystemBaselineBox : FullBox
{
	public const string TYPE = "blin";
	public override string DisplayName { get { return "StereoCameraSystemBaselineBox"; } }

	protected uint baseline_value; 
	public uint BaselineValue { get { return this.baseline_value; } set { this.baseline_value = value; } }

	public StereoCameraSystemBaselineBox(): base(IsoStream.FromFourCC("blin"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.baseline_value, "baseline_value"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.baseline_value, "baseline_value"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // baseline_value
		return boxSize;
	}
}

}
