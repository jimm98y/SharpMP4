using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class HintBytesSent extends Box('totl') {
	uint(32)	bytessent; }	// total bytes sent, including 12-byte RTP headers

*/
public partial class HintBytesSenttotlDup : Box
{
	public const string TYPE = "totl";
	public override string DisplayName { get { return "HintBytesSenttotlDup"; } }

	protected uint bytessent; 
	public uint Bytessent { get { return this.bytessent; } set { this.bytessent = value; } }

	public HintBytesSenttotlDup(): base(IsoStream.FromFourCC("totl"))
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
