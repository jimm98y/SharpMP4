using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class CompositionOffsetBox
	extends FullBox('ctts', version, 0) {
	unsigned int(32)	entry_count;
		int i;
	if (version==0) {
		for (i=0; i < entry_count; i++) {
			unsigned int(32)	sample_count;
			unsigned int(32)	sample_offset;
		}
	}
	else if (version == 1) {
		for (i=0; i < entry_count; i++) {
			unsigned int(32)	sample_count;
			signed   int(32)	sample_offset;
		}
	}
}
*/
public partial class CompositionOffsetBox : FullBox
{
	public const string TYPE = "ctts";
	public override string DisplayName { get { return "CompositionOffsetBox"; } }

	protected uint entry_count; 
	public uint EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	protected uint[] sample_count; 
	public uint[] SampleCount { get { return this.sample_count; } set { this.sample_count = value; } }

	protected uint[] sample_offset; 
	public uint[] SampleOffset { get { return this.sample_offset; } set { this.sample_offset = value; } }

	protected int[] sample_offset0; 
	public int[] SampleOffset0 { get { return this.sample_offset0; } set { this.sample_offset0 = value; } }

	public CompositionOffsetBox(byte version = 0): base(IsoStream.FromFourCC("ctts"), version, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entry_count, "entry_count"); 
		

		if (version==0)
		{

			this.sample_count = new uint[IsoStream.GetInt( entry_count)];
			this.sample_offset = new uint[IsoStream.GetInt( entry_count)];
			for (int i=0; i < entry_count; i++)
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_count[i], "sample_count"); 
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_offset[i], "sample_offset"); 
			}
		}

		else if (version == 1)
		{

			this.sample_offset0 = new int[IsoStream.GetInt( entry_count)];
			for (int i=0; i < entry_count; i++)
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_count[i], "sample_count"); 
				boxSize += stream.ReadInt32(boxSize, readSize,  out this.sample_offset0[i], "sample_offset0"); 
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.entry_count, "entry_count"); 
		

		if (version==0)
		{

			for (int i=0; i < entry_count; i++)
			{
				boxSize += stream.WriteUInt32( this.sample_count[i], "sample_count"); 
				boxSize += stream.WriteUInt32( this.sample_offset[i], "sample_offset"); 
			}
		}

		else if (version == 1)
		{

			for (int i=0; i < entry_count; i++)
			{
				boxSize += stream.WriteUInt32( this.sample_count[i], "sample_count"); 
				boxSize += stream.WriteInt32( this.sample_offset0[i], "sample_offset0"); 
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // entry_count
		

		if (version==0)
		{

			for (int i=0; i < entry_count; i++)
			{
				boxSize += 32; // sample_count
				boxSize += 32; // sample_offset
			}
		}

		else if (version == 1)
		{

			for (int i=0; i < entry_count; i++)
			{
				boxSize += 32; // sample_count
				boxSize += 32; // sample_offset0
			}
		}
		return boxSize;
	}
}

}
