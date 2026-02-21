using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class HintMinRelativeTime extends Box('tmin') {
	int(32)		time; }		// smallest relative transmission time, milliseconds

*/
public partial class HintMinRelativeTime : Box
{
	public const string TYPE = "tmin";
	public override string DisplayName { get { return "HintMinRelativeTime"; } }

	protected int time; 
	public int Time { get { return this.time; } set { this.time = value; } }

	public HintMinRelativeTime(): base(IsoStream.FromFourCC("tmin"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.time, "time"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt32( this.time, "time"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // time
		return boxSize;
	}
}

}
