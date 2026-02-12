using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class SampleToGroupBox
	extends FullBox('sbgp', version, 0)
{
	unsigned int(32)	grouping_type;
	if (version == 1) {
		unsigned int(32) grouping_type_parameter;
	}
	unsigned int(32)	entry_count;
	for (i=1; i <= entry_count; i++)
	{
		unsigned int(32)	sample_count;
		unsigned int(32)	group_description_index;
	}
}
*/
public partial class SampleToGroupBox : FullBox
{
	public const string TYPE = "sbgp";
	public override string DisplayName { get { return "SampleToGroupBox"; } }

	protected uint grouping_type; 
	public uint GroupingType { get { return this.grouping_type; } set { this.grouping_type = value; } }

	protected uint grouping_type_parameter; 
	public uint GroupingTypeParameter { get { return this.grouping_type_parameter; } set { this.grouping_type_parameter = value; } }

	protected uint entry_count; 
	public uint EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	protected uint[] sample_count; 
	public uint[] SampleCount { get { return this.sample_count; } set { this.sample_count = value; } }

	protected uint[] group_description_index; 
	public uint[] GroupDescriptionIndex { get { return this.group_description_index; } set { this.group_description_index = value; } }

	public SampleToGroupBox(byte version = 0): base(IsoStream.FromFourCC("sbgp"), version, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.grouping_type, "grouping_type"); 

		if (version == 1)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.grouping_type_parameter, "grouping_type_parameter"); 
		}
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entry_count, "entry_count"); 

		this.sample_count = new uint[IsoStream.GetInt( entry_count)];
		this.group_description_index = new uint[IsoStream.GetInt( entry_count)];
		for (int i=0; i < entry_count; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_count[i], "sample_count"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.group_description_index[i], "group_description_index"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.grouping_type, "grouping_type"); 

		if (version == 1)
		{
			boxSize += stream.WriteUInt32( this.grouping_type_parameter, "grouping_type_parameter"); 
		}
		boxSize += stream.WriteUInt32( this.entry_count, "entry_count"); 

		for (int i=0; i < entry_count; i++)
		{
			boxSize += stream.WriteUInt32( this.sample_count[i], "sample_count"); 
			boxSize += stream.WriteUInt32( this.group_description_index[i], "group_description_index"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // grouping_type

		if (version == 1)
		{
			boxSize += 32; // grouping_type_parameter
		}
		boxSize += 32; // entry_count

		for (int i=0; i < entry_count; i++)
		{
			boxSize += 32; // sample_count
			boxSize += 32; // group_description_index
		}
		return boxSize;
	}
}

}
