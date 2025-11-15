using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class SampleToChunkBox
	extends FullBox('stsc', version = 0, 0) {
	unsigned int(32)	entry_count;
	for (i=1; i <= entry_count; i++) {
		unsigned int(32)	first_chunk;
		unsigned int(32)	samples_per_chunk;
		unsigned int(32)	sample_description_index;
	}
}
*/
public partial class SampleToChunkBox : FullBox
{
	public const string TYPE = "stsc";
	public override string DisplayName { get { return "SampleToChunkBox"; } }

	protected uint entry_count; 
	public uint EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	protected uint[] first_chunk; 
	public uint[] FirstChunk { get { return this.first_chunk; } set { this.first_chunk = value; } }

	protected uint[] samples_per_chunk; 
	public uint[] SamplesPerChunk { get { return this.samples_per_chunk; } set { this.samples_per_chunk = value; } }

	protected uint[] sample_description_index; 
	public uint[] SampleDescriptionIndex { get { return this.sample_description_index; } set { this.sample_description_index = value; } }

	public SampleToChunkBox(): base(IsoStream.FromFourCC("stsc"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entry_count, "entry_count"); 

		this.first_chunk = new uint[IsoStream.GetInt( entry_count)];
		this.samples_per_chunk = new uint[IsoStream.GetInt( entry_count)];
		this.sample_description_index = new uint[IsoStream.GetInt( entry_count)];
		for (int i=0; i < entry_count; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.first_chunk[i], "first_chunk"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.samples_per_chunk[i], "samples_per_chunk"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_description_index[i], "sample_description_index"); 
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
			boxSize += stream.WriteUInt32( this.first_chunk[i], "first_chunk"); 
			boxSize += stream.WriteUInt32( this.samples_per_chunk[i], "samples_per_chunk"); 
			boxSize += stream.WriteUInt32( this.sample_description_index[i], "sample_description_index"); 
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
			boxSize += 32; // first_chunk
			boxSize += 32; // samples_per_chunk
			boxSize += 32; // sample_description_index
		}
		return boxSize;
	}
}

}
