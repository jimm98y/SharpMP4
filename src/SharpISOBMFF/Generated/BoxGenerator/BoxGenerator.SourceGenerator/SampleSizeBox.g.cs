using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SampleSizeBox extends FullBox('stsz', version = 0, 0) {
	unsigned int(32)	sample_size;
	unsigned int(32)	sample_count;
	if (sample_size==0) {
		for (i=1; i <= sample_count; i++) {
		unsigned int(32)	entry_size;
		}
	}
}
*/
public partial class SampleSizeBox : FullBox
{
	public const string TYPE = "stsz";
	public override string DisplayName { get { return "SampleSizeBox"; } }

	protected uint sample_size; 
	public uint SampleSize { get { return this.sample_size; } set { this.sample_size = value; } }

	protected uint sample_count; 
	public uint SampleCount { get { return this.sample_count; } set { this.sample_count = value; } }

	protected uint[] entry_size; 
	public uint[] EntrySize { get { return this.entry_size; } set { this.entry_size = value; } }

	public SampleSizeBox(): base(IsoStream.FromFourCC("stsz"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_size, "sample_size"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_count, "sample_count"); 

		if (sample_size==0)
		{

			this.entry_size = new uint[IsoStream.GetInt( sample_count)];
			for (int i=0; i < sample_count; i++)
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entry_size[i], "entry_size"); 
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.sample_size, "sample_size"); 
		boxSize += stream.WriteUInt32( this.sample_count, "sample_count"); 

		if (sample_size==0)
		{

			for (int i=0; i < sample_count; i++)
			{
				boxSize += stream.WriteUInt32( this.entry_size[i], "entry_size"); 
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // sample_size
		boxSize += 32; // sample_count

		if (sample_size==0)
		{

			for (int i=0; i < sample_count; i++)
			{
				boxSize += 32; // entry_size
			}
		}
		return boxSize;
	}
}

}
