using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class HintMaxRelativeTime extends Box('tmax') {
	int(32)		time; }		// largest relative transmission time, milliseconds
*/
public partial class HintMaxRelativeTime : Box
{
	public const string TYPE = "tmax";
	public override string DisplayName { get { return "HintMaxRelativeTime"; } }

	protected int time; 
	public int Time { get { return this.time; } set { this.time = value; } }

	public HintMaxRelativeTime(): base(IsoStream.FromFourCC("tmax"))
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
