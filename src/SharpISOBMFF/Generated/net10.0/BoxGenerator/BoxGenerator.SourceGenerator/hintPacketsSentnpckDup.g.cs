using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class hintPacketsSent extends Box('npck') {
	uint(32)	packetssent; }	// total packets sent

*/
public partial class hintPacketsSentnpckDup : Box
{
	public const string TYPE = "npck";
	public override string DisplayName { get { return "hintPacketsSentnpckDup"; } }

	protected uint packetssent; 
	public uint Packetssent { get { return this.packetssent; } set { this.packetssent = value; } }

	public hintPacketsSentnpckDup(): base(IsoStream.FromFourCC("npck"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.packetssent, "packetssent"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.packetssent, "packetssent"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // packetssent
		return boxSize;
	}
}

}
