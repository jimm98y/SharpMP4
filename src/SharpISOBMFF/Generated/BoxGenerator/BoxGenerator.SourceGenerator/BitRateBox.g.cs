using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class BitRateBox extends Box('btrt'){
	unsigned int(32) bufferSizeDB;
	unsigned int(32) maxBitrate;
	unsigned int(32) avgBitrate;
}
*/
public partial class BitRateBox : Box
{
	public const string TYPE = "btrt";
	public override string DisplayName { get { return "BitRateBox"; } }

	protected uint bufferSizeDB; 
	public uint BufferSizeDB { get { return this.bufferSizeDB; } set { this.bufferSizeDB = value; } }

	protected uint maxBitrate; 
	public uint MaxBitrate { get { return this.maxBitrate; } set { this.maxBitrate = value; } }

	protected uint avgBitrate; 
	public uint AvgBitrate { get { return this.avgBitrate; } set { this.avgBitrate = value; } }

	public BitRateBox(): base(IsoStream.FromFourCC("btrt"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.bufferSizeDB, "bufferSizeDB"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.maxBitrate, "maxBitrate"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.avgBitrate, "avgBitrate"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.bufferSizeDB, "bufferSizeDB"); 
		boxSize += stream.WriteUInt32( this.maxBitrate, "maxBitrate"); 
		boxSize += stream.WriteUInt32( this.avgBitrate, "avgBitrate"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // bufferSizeDB
		boxSize += 32; // maxBitrate
		boxSize += 32; // avgBitrate
		return boxSize;
	}
}

}
