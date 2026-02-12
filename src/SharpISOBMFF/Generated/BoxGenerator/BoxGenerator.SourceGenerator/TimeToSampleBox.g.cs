using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class TimeToSampleBox
	extends FullBox('stts', version = 0, 0) {
	unsigned int(32)	entry_count;
		int i;
	for (i=0; i < entry_count; i++) {
		unsigned int(32)	sample_count;
		unsigned int(32)	sample_delta;
	}
}
*/
public partial class TimeToSampleBox : FullBox
{
	public const string TYPE = "stts";
	public override string DisplayName { get { return "TimeToSampleBox"; } }

	protected uint entry_count; 
	public uint EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	protected uint[] sample_count; 
	public uint[] SampleCount { get { return this.sample_count; } set { this.sample_count = value; } }

	protected uint[] sample_delta; 
	public uint[] SampleDelta { get { return this.sample_delta; } set { this.sample_delta = value; } }

	public TimeToSampleBox(): base(IsoStream.FromFourCC("stts"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entry_count, "entry_count"); 
		

		this.sample_count = new uint[IsoStream.GetInt( entry_count)];
		this.sample_delta = new uint[IsoStream.GetInt( entry_count)];
		for (int i=0; i < entry_count; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_count[i], "sample_count"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_delta[i], "sample_delta"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.entry_count, "entry_count"); 
		

		for (int i=0; i < entry_count; i++)
		{
			boxSize += stream.WriteUInt32( this.sample_count[i], "sample_count"); 
			boxSize += stream.WriteUInt32( this.sample_delta[i], "sample_delta"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // entry_count
		

		for (int i=0; i < entry_count; i++)
		{
			boxSize += 32; // sample_count
			boxSize += 32; // sample_delta
		}
		return boxSize;
	}
}

}
