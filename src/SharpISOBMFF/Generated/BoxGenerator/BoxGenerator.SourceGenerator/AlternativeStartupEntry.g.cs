using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class AlternativeStartupEntry() extends VisualSampleGroupEntry ('alst')
{
	unsigned int(16) roll_count;
	unsigned int(16) first_output_sample;
	for (i=1; i <= roll_count; i++)
		unsigned int(32) sample_offset[i];
	 // optional, until the end of the structure
AlternativeStartupEntryItem items[];
}
 
*/
public partial class AlternativeStartupEntry : VisualSampleGroupEntry
{
	public const string TYPE = "alst";
	public override string DisplayName { get { return "AlternativeStartupEntry"; } }

	protected ushort roll_count; 
	public ushort RollCount { get { return this.roll_count; } set { this.roll_count = value; } }

	protected ushort first_output_sample; 
	public ushort FirstOutputSample { get { return this.first_output_sample; } set { this.first_output_sample = value; } }

	protected uint[] sample_offset;  //  optional, until the end of the structure
	public uint[] SampleOffset { get { return this.sample_offset; } set { this.sample_offset = value; } }

	protected AlternativeStartupEntryItem[] items; 
	public AlternativeStartupEntryItem[] Items { get { return this.items; } set { this.items = value; } }

	public AlternativeStartupEntry(): base(IsoStream.FromFourCC("alst"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.roll_count, "roll_count"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.first_output_sample, "first_output_sample"); 

		this.sample_offset = new uint[IsoStream.GetInt( roll_count)];
		for (int i=0; i < roll_count; i++)
		{
			if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_offset[i], "sample_offset"); // optional, until the end of the structure
		}
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(uint.MaxValue), () => new AlternativeStartupEntryItem(),  out this.items, "items"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.roll_count, "roll_count"); 
		boxSize += stream.WriteUInt16( this.first_output_sample, "first_output_sample"); 

		for (int i=0; i < roll_count; i++)
		{
			boxSize += stream.WriteUInt32( this.sample_offset[i], "sample_offset"); // optional, until the end of the structure
		}
		boxSize += stream.WriteClass( this.items, "items"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // roll_count
		boxSize += 16; // first_output_sample

		for (int i=0; i < roll_count; i++)
		{
			boxSize += 32; // sample_offset
		}
		boxSize += IsoStream.CalculateClassSize(items); // items
		return boxSize;
	}
}

}
