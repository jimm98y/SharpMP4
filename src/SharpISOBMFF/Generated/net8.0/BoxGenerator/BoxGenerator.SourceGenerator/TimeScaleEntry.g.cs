using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class TimeScaleEntry() extends Box('tims') {
	uint(32)	timescale;
}


*/
public partial class TimeScaleEntry : Box
{
	public const string TYPE = "tims";
	public override string DisplayName { get { return "TimeScaleEntry"; } }

	protected uint timescale; 
	public uint Timescale { get { return this.timescale; } set { this.timescale = value; } }

	public TimeScaleEntry(): base(IsoStream.FromFourCC("tims"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.timescale, "timescale"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.timescale, "timescale"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // timescale
		return boxSize;
	}
}

}
