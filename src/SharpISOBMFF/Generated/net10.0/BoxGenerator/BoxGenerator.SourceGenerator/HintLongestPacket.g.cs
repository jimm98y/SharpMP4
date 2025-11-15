using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class HintLongestPacket extends Box('dmax') {
	uint(32)	time; }			// longest packet duration, milliseconds
*/
public partial class HintLongestPacket : Box
{
	public const string TYPE = "dmax";
	public override string DisplayName { get { return "HintLongestPacket"; } }

	protected uint time; 
	public uint Time { get { return this.time; } set { this.time = value; } }

	public HintLongestPacket(): base(IsoStream.FromFourCC("dmax"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.time, "time"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.time, "time"); 
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
