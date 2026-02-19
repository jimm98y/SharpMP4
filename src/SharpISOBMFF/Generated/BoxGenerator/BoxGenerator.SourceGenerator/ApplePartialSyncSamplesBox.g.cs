using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ApplePartialSyncSamplesBox() extends FullBox('stps', version = 0, 0) {
	unsigned int(32)	entry_count;
	int i;
	for (i=0; i < entry_count; i++) {
		unsigned int(32)	sample_number;
	}
}
*/
public partial class ApplePartialSyncSamplesBox : FullBox
{
	public const string TYPE = "stps";
	public override string DisplayName { get { return "ApplePartialSyncSamplesBox"; } }

	protected uint entry_count; 
	public uint EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	protected uint[] sample_number; 
	public uint[] SampleNumber { get { return this.sample_number; } set { this.sample_number = value; } }

	public ApplePartialSyncSamplesBox(): base(IsoStream.FromFourCC("stps"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entry_count, "entry_count"); 
		

		this.sample_number = new uint[IsoStream.GetInt( entry_count)];
		for (int i=0; i < entry_count; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_number[i], "sample_number"); 
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
			boxSize += stream.WriteUInt32( this.sample_number[i], "sample_number"); 
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
			boxSize += 32; // sample_number
		}
		return boxSize;
	}
}

}
