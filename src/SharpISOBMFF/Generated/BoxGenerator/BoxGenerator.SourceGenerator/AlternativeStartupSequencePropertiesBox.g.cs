using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class AlternativeStartupSequencePropertiesBox extends FullBox('assp', version, 0) {
	if (version == 0) {
		signed int(32)		min_initial_alt_startup_offset;
	}
	else if (version == 1) {
		unsigned int(32)	num_entries;
		for (j=1; j <= num_entries; j++) {
			unsigned int(32)	grouping_type_parameter;
			signed int(32)		min_initial_alt_startup_offset;
		}
	}
}
*/
public partial class AlternativeStartupSequencePropertiesBox : FullBox
{
	public const string TYPE = "assp";
	public override string DisplayName { get { return "AlternativeStartupSequencePropertiesBox"; } }

	protected int min_initial_alt_startup_offset; 
	public int MinInitialAltStartupOffset { get { return this.min_initial_alt_startup_offset; } set { this.min_initial_alt_startup_offset = value; } }

	protected uint num_entries; 
	public uint NumEntries { get { return this.num_entries; } set { this.num_entries = value; } }

	protected uint[] grouping_type_parameter; 
	public uint[] GroupingTypeParameter { get { return this.grouping_type_parameter; } set { this.grouping_type_parameter = value; } }

	protected int[] min_initial_alt_startup_offset0; 
	public int[] MinInitialAltStartupOffset0 { get { return this.min_initial_alt_startup_offset0; } set { this.min_initial_alt_startup_offset0 = value; } }

	public AlternativeStartupSequencePropertiesBox(byte version = 0): base(IsoStream.FromFourCC("assp"), version, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);

		if (version == 0)
		{
			boxSize += stream.ReadInt32(boxSize, readSize,  out this.min_initial_alt_startup_offset, "min_initial_alt_startup_offset"); 
		}

		else if (version == 1)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.num_entries, "num_entries"); 

			this.grouping_type_parameter = new uint[IsoStream.GetInt( num_entries)];
			this.min_initial_alt_startup_offset0 = new int[IsoStream.GetInt( num_entries)];
			for (int j=0; j < num_entries; j++)
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.grouping_type_parameter[j], "grouping_type_parameter"); 
				boxSize += stream.ReadInt32(boxSize, readSize,  out this.min_initial_alt_startup_offset0[j], "min_initial_alt_startup_offset0"); 
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);

		if (version == 0)
		{
			boxSize += stream.WriteInt32( this.min_initial_alt_startup_offset, "min_initial_alt_startup_offset"); 
		}

		else if (version == 1)
		{
			boxSize += stream.WriteUInt32( this.num_entries, "num_entries"); 

			for (int j=0; j < num_entries; j++)
			{
				boxSize += stream.WriteUInt32( this.grouping_type_parameter[j], "grouping_type_parameter"); 
				boxSize += stream.WriteInt32( this.min_initial_alt_startup_offset0[j], "min_initial_alt_startup_offset0"); 
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();

		if (version == 0)
		{
			boxSize += 32; // min_initial_alt_startup_offset
		}

		else if (version == 1)
		{
			boxSize += 32; // num_entries

			for (int j=0; j < num_entries; j++)
			{
				boxSize += 32; // grouping_type_parameter
				boxSize += 32; // min_initial_alt_startup_offset0
			}
		}
		return boxSize;
	}
}

}
