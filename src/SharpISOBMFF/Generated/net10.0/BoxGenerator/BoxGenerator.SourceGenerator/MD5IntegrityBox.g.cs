using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class MD5IntegrityBox()
extends FullBox('md5i', version = 0, flags) {
	unsigned int(8)[16] input_MD5;
	unsigned int(32) input_4cc;
	if (input_4cc == 'sgpd') {
		unsigned int(32) grouping_type;
		if (flags&1)
			unsigned int(32) grouping_type_parameter;
		unsigned int(32) num_entries;
		for(i=0; i<num_entries; i++) {
			unsigned int(32) group_description_index[i];
		}
	}
}
*/
public partial class MD5IntegrityBox : FullBox
{
	public const string TYPE = "md5i";
	public override string DisplayName { get { return "MD5IntegrityBox"; } }

	protected byte[] input_MD5; 
	public byte[] InputMD5 { get { return this.input_MD5; } set { this.input_MD5 = value; } }

	protected uint input_4cc; 
	public uint Input4cc { get { return this.input_4cc; } set { this.input_4cc = value; } }

	protected uint grouping_type; 
	public uint GroupingType { get { return this.grouping_type; } set { this.grouping_type = value; } }

	protected uint grouping_type_parameter; 
	public uint GroupingTypeParameter { get { return this.grouping_type_parameter; } set { this.grouping_type_parameter = value; } }

	protected uint num_entries; 
	public uint NumEntries { get { return this.num_entries; } set { this.num_entries = value; } }

	protected uint[] group_description_index; 
	public uint[] GroupDescriptionIndex { get { return this.group_description_index; } set { this.group_description_index = value; } }

	public MD5IntegrityBox(uint flags = 0): base(IsoStream.FromFourCC("md5i"), 0, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 16,  out this.input_MD5, "input_MD5"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.input_4cc, "input_4cc"); 

		if (input_4cc == IsoStream.FromFourCC("sgpd"))
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.grouping_type, "grouping_type"); 

			if ((flags & 1) == 1)
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.grouping_type_parameter, "grouping_type_parameter"); 
			}
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.num_entries, "num_entries"); 

			this.group_description_index = new uint[IsoStream.GetInt(num_entries)];
			for (int i=0; i<num_entries; i++)
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.group_description_index[i], "group_description_index"); 
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8Array(16,  this.input_MD5, "input_MD5"); 
		boxSize += stream.WriteUInt32( this.input_4cc, "input_4cc"); 

		if (input_4cc == IsoStream.FromFourCC("sgpd"))
		{
			boxSize += stream.WriteUInt32( this.grouping_type, "grouping_type"); 

			if ((flags & 1) == 1)
			{
				boxSize += stream.WriteUInt32( this.grouping_type_parameter, "grouping_type_parameter"); 
			}
			boxSize += stream.WriteUInt32( this.num_entries, "num_entries"); 

			for (int i=0; i<num_entries; i++)
			{
				boxSize += stream.WriteUInt32( this.group_description_index[i], "group_description_index"); 
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16 * 8; // input_MD5
		boxSize += 32; // input_4cc

		if (input_4cc == IsoStream.FromFourCC("sgpd"))
		{
			boxSize += 32; // grouping_type

			if ((flags & 1) == 1)
			{
				boxSize += 32; // grouping_type_parameter
			}
			boxSize += 32; // num_entries

			for (int i=0; i<num_entries; i++)
			{
				boxSize += 32; // group_description_index
			}
		}
		return boxSize;
	}
}

}
