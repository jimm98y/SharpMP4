using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class HintBytesSentQuickTime() extends Box('trpY') {
 unsigned int(64) bytessent;
}

*/
public partial class HintBytesSentQuickTime : Box
{
	public const string TYPE = "trpY";
	public override string DisplayName { get { return "HintBytesSentQuickTime"; } }

	protected ulong bytessent; 
	public ulong Bytessent { get { return this.bytessent; } set { this.bytessent = value; } }

	public HintBytesSentQuickTime(): base(IsoStream.FromFourCC("trpY"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt64(boxSize, readSize,  out this.bytessent, "bytessent"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt64( this.bytessent, "bytessent"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 64; // bytessent
		return boxSize;
	}
}

}
