using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class StereoComfortBox extends FullBox('cmfy') {
  unsigned int(32) baseline_value;
 }

*/
public partial class StereoComfortBox : FullBox
{
	public const string TYPE = "cmfy";
	public override string DisplayName { get { return "StereoComfortBox"; } }

	protected uint baseline_value; 
	public uint BaselineValue { get { return this.baseline_value; } set { this.baseline_value = value; } }

	public StereoComfortBox(): base(IsoStream.FromFourCC("cmfy"))
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
