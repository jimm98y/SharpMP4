using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class CompactSampleToGroupBox
	extends FullBox('csgp', version, flags)
{
	unsigned int(32) grouping_type;

	if (grouping_type_parameter_present == 1) {
		unsigned int(32) grouping_type_parameter;
	}
	unsigned int(32) pattern_count;
	totalPatternLength = 0;
	for (i=1; i <= pattern_count; i++) {
		unsigned int(f(pattern_size_code)) pattern_length[i];
		unsigned int(f(count_size_code)) sample_count[i];
	}
	for (j=1; j <= pattern_count; j++) {
		for (k=1; k <= pattern_length[j]; k++) {
			unsigned int(f(index_size_code))
						 sample_group_description_index[j][k];
			// whose msb might indicate fragment_local or global
		}
	}
}
*/
public partial class CompactSampleToGroupBox : FullBox
{
	public const string TYPE = "csgp";
	public override string DisplayName { get { return "CompactSampleToGroupBox"; } }

	protected uint grouping_type; 
	public uint GroupingType { get { return this.grouping_type; } set { this.grouping_type = value; } }

	protected uint grouping_type_parameter; 
	public uint GroupingTypeParameter { get { return this.grouping_type_parameter; } set { this.grouping_type_parameter = value; } }

	protected uint pattern_count; 
	public uint PatternCount { get { return this.pattern_count; } set { this.pattern_count = value; } }

	protected byte[][] pattern_length; 
	public byte[][] PatternLength { get { return this.pattern_length; } set { this.pattern_length = value; } }

	protected byte[][] sample_count; 
	public byte[][] SampleCount { get { return this.sample_count; } set { this.sample_count = value; } }

	protected byte[][][] sample_group_description_index;  //  whose msb might indicate fragment_local or global
	public byte[][][] SampleGroupDescriptionIndex { get { return this.sample_group_description_index; } set { this.sample_group_description_index = value; } }

	public CompactSampleToGroupBox(byte version = 0, uint flags = 0): base(IsoStream.FromFourCC("csgp"), version, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		bool grouping_type_parameter_present = (flags & (1 << 6)) == (1 << 6);
		uint count_size_code = (flags >> 2) & 0x3;
		uint pattern_size_code = (flags >> 4) & 0x3;
		uint index_size_code = flags & 0x3;

		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.grouping_type, "grouping_type"); 

		if (grouping_type_parameter_present == true)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.grouping_type_parameter, "grouping_type_parameter"); 
		}
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.pattern_count, "pattern_count"); 
		

		this.pattern_length = new byte[IsoStream.GetInt( pattern_count)][];
		this.sample_count = new byte[IsoStream.GetInt( pattern_count)][];
		for (int i=0; i < pattern_count; i++)
		{
			boxSize += stream.ReadBits(boxSize, readSize, (uint)(pattern_size_code ),  out this.pattern_length[i], "pattern_length"); 
			boxSize += stream.ReadBits(boxSize, readSize, (uint)(count_size_code ),  out this.sample_count[i], "sample_count"); 
		}

		this.sample_group_description_index = new byte[IsoStream.GetInt( pattern_count)][][];
		for (int j=0; j < pattern_count; j++)
		{

			this.sample_group_description_index[j] = new byte[IsoStream.GetInt( IsoStream.GetInt(pattern_length[j]))][];
			for (int k=0; k < IsoStream.GetInt(pattern_length[j]); k++)
			{
				boxSize += stream.ReadBits(boxSize, readSize, (uint)(index_size_code ),  out this.sample_group_description_index[j][k], "sample_group_description_index"); // whose msb might indicate fragment_local or global
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		bool grouping_type_parameter_present = (flags & (1 << 6)) == (1 << 6);
		uint count_size_code = (flags >> 2) & 0x3;
		uint pattern_size_code = (flags >> 4) & 0x3;
		uint index_size_code = flags & 0x3;

		boxSize += stream.WriteUInt32( this.grouping_type, "grouping_type"); 

		if (grouping_type_parameter_present == true)
		{
			boxSize += stream.WriteUInt32( this.grouping_type_parameter, "grouping_type_parameter"); 
		}
		boxSize += stream.WriteUInt32( this.pattern_count, "pattern_count"); 
		

		for (int i=0; i < pattern_count; i++)
		{
			boxSize += stream.WriteBits((uint)(pattern_size_code ),  this.pattern_length[i], "pattern_length"); 
			boxSize += stream.WriteBits((uint)(count_size_code ),  this.sample_count[i], "sample_count"); 
		}

		for (int j=0; j < pattern_count; j++)
		{

			for (int k=0; k < IsoStream.GetInt(pattern_length[j]); k++)
			{
				boxSize += stream.WriteBits((uint)(index_size_code ),  this.sample_group_description_index[j][k], "sample_group_description_index"); // whose msb might indicate fragment_local or global
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		bool grouping_type_parameter_present = (flags & (1 << 6)) == (1 << 6);
		uint count_size_code = (flags >> 2) & 0x3;
		uint pattern_size_code = (flags >> 4) & 0x3;
		uint index_size_code = flags & 0x3;

		boxSize += 32; // grouping_type

		if (grouping_type_parameter_present == true)
		{
			boxSize += 32; // grouping_type_parameter
		}
		boxSize += 32; // pattern_count
		

		for (int i=0; i < pattern_count; i++)
		{
			boxSize += (ulong)(pattern_size_code ); // pattern_length
			boxSize += (ulong)(count_size_code ); // sample_count
		}

		for (int j=0; j < pattern_count; j++)
		{

			for (int k=0; k < IsoStream.GetInt(pattern_length[j]); k++)
			{
				boxSize += (ulong)(index_size_code ); // sample_group_description_index
			}
		}
		return boxSize;
	}
}

}
