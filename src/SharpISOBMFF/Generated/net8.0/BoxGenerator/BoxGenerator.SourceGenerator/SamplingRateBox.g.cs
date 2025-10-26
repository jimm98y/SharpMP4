using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class SamplingRateBox extends FullBox('srat') {
	unsigned int(32) sampling_rate;
}
*/
public partial class SamplingRateBox : FullBox
{
	public const string TYPE = "srat";
	public override string DisplayName { get { return "SamplingRateBox"; } }

	protected uint sampling_rate; 
	public uint SamplingRate { get { return this.sampling_rate; } set { this.sampling_rate = value; } }

	public SamplingRateBox(): base(IsoStream.FromFourCC("srat"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sampling_rate, "sampling_rate"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.sampling_rate, "sampling_rate"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // sampling_rate
		return boxSize;
	}
}

}
