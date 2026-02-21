using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class HintBytesSent extends Box('trpy') {
	uint(64)	bytessent; }	// total bytes sent, including 12-byte RTP headers

*/
public partial class HintBytesSent : Box
{
	public const string TYPE = "trpy";
	public override string DisplayName { get { return "HintBytesSent"; } }

	protected ulong bytessent; 
	public ulong Bytessent { get { return this.bytessent; } set { this.bytessent = value; } }

	public HintBytesSent(): base(IsoStream.FromFourCC("trpy"))
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
