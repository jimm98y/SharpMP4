using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class hintPacketsSent extends Box('nump') {
	uint(64)	packetssent; }	// total packets sent

*/
public partial class hintPacketsSent : Box
{
	public const string TYPE = "nump";
	public override string DisplayName { get { return "hintPacketsSent"; } }

	protected ulong packetssent; 
	public ulong Packetssent { get { return this.packetssent; } set { this.packetssent = value; } }

	public hintPacketsSent(): base(IsoStream.FromFourCC("nump"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt64(boxSize, readSize,  out this.packetssent, "packetssent"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt64( this.packetssent, "packetssent"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 64; // packetssent
		return boxSize;
	}
}

}
