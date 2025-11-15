using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class HintLargestPacket extends Box('pmax') {
	uint(32)	bytes; }			// largest packet sent, including RTP header

*/
public partial class HintLargestPacket : Box
{
	public const string TYPE = "pmax";
	public override string DisplayName { get { return "HintLargestPacket"; } }

	protected uint bytes; 
	public uint Bytes { get { return this.bytes; } set { this.bytes = value; } }

	public HintLargestPacket(): base(IsoStream.FromFourCC("pmax"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.bytes, "bytes"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.bytes, "bytes"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // bytes
		return boxSize;
	}
}

}
