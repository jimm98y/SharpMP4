using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class HintBytesSentNoRtpQuickTime() extends Box('tpaY') {
 unsigned int(32) bytessent;
}

*/
public partial class HintBytesSentNoRtpQuickTime : Box
{
	public const string TYPE = "tpaY";
	public override string DisplayName { get { return "HintBytesSentNoRtpQuickTime"; } }

	protected uint bytessent; 
	public uint Bytessent { get { return this.bytessent; } set { this.bytessent = value; } }

	public HintBytesSentNoRtpQuickTime(): base(IsoStream.FromFourCC("tpaY"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.bytessent, "bytessent"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.bytessent, "bytessent"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // bytessent
		return boxSize;
	}
}

}
