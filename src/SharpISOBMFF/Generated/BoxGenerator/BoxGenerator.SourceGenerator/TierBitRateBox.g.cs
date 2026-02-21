using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class TierBitRateBox extends Box('tibr'){ 
	unsigned int(32) baseBitRate;
	unsigned int(32) maxBitRate;
	unsigned int(32) avgBitRate;

	unsigned int(32) tierBaseBitRate;
	unsigned int(32) tierMaxBitRate;
	unsigned int(32) tierAvgBitRate;
}
*/
public partial class TierBitRateBox : Box
{
	public const string TYPE = "tibr";
	public override string DisplayName { get { return "TierBitRateBox"; } }

	protected uint baseBitRate; 
	public uint BaseBitRate { get { return this.baseBitRate; } set { this.baseBitRate = value; } }

	protected uint maxBitRate; 
	public uint MaxBitRate { get { return this.maxBitRate; } set { this.maxBitRate = value; } }

	protected uint avgBitRate; 
	public uint AvgBitRate { get { return this.avgBitRate; } set { this.avgBitRate = value; } }

	protected uint tierBaseBitRate; 
	public uint TierBaseBitRate { get { return this.tierBaseBitRate; } set { this.tierBaseBitRate = value; } }

	protected uint tierMaxBitRate; 
	public uint TierMaxBitRate { get { return this.tierMaxBitRate; } set { this.tierMaxBitRate = value; } }

	protected uint tierAvgBitRate; 
	public uint TierAvgBitRate { get { return this.tierAvgBitRate; } set { this.tierAvgBitRate = value; } }

	public TierBitRateBox(): base(IsoStream.FromFourCC("tibr"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.baseBitRate, "baseBitRate"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.maxBitRate, "maxBitRate"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.avgBitRate, "avgBitRate"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.tierBaseBitRate, "tierBaseBitRate"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.tierMaxBitRate, "tierMaxBitRate"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.tierAvgBitRate, "tierAvgBitRate"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.baseBitRate, "baseBitRate"); 
		boxSize += stream.WriteUInt32( this.maxBitRate, "maxBitRate"); 
		boxSize += stream.WriteUInt32( this.avgBitRate, "avgBitRate"); 
		boxSize += stream.WriteUInt32( this.tierBaseBitRate, "tierBaseBitRate"); 
		boxSize += stream.WriteUInt32( this.tierMaxBitRate, "tierMaxBitRate"); 
		boxSize += stream.WriteUInt32( this.tierAvgBitRate, "tierAvgBitRate"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // baseBitRate
		boxSize += 32; // maxBitRate
		boxSize += 32; // avgBitRate
		boxSize += 32; // tierBaseBitRate
		boxSize += 32; // tierMaxBitRate
		boxSize += 32; // tierAvgBitRate
		return boxSize;
	}
}

}
