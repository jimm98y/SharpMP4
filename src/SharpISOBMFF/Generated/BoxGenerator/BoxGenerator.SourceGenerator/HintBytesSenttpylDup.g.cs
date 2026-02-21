using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class HintBytesSent extends Box('tpyl') {
	uint(64)	bytessent; }	// total bytes sent, not including RTP headers
*/
public partial class HintBytesSenttpylDup : Box
{
	public const string TYPE = "tpyl";
	public override string DisplayName { get { return "HintBytesSenttpylDup"; } }

	protected ulong bytessent; 
	public ulong Bytessent { get { return this.bytessent; } set { this.bytessent = value; } }

	public HintBytesSenttpylDup(): base(IsoStream.FromFourCC("tpyl"))
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
