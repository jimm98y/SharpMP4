using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class SampleAuxiliaryInformationOffsetsBox
	extends FullBox('saio', version, flags)
{
	if (flags & 1) {
		unsigned int(32) aux_info_type;
		unsigned int(32) aux_info_type_parameter;
	}
	unsigned int(32) entry_count;
	if ( version == 0 ) {
		unsigned int(32) offset[ entry_count ];
	}
	else {
		unsigned int(64) offset[ entry_count ];
	}
}
*/
public partial class SampleAuxiliaryInformationOffsetsBox : FullBox
{
	public const string TYPE = "saio";
	public override string DisplayName { get { return "SampleAuxiliaryInformationOffsetsBox"; } }

	protected uint aux_info_type; 
	public uint AuxInfoType { get { return this.aux_info_type; } set { this.aux_info_type = value; } }

	protected uint aux_info_type_parameter; 
	public uint AuxInfoTypeParameter { get { return this.aux_info_type_parameter; } set { this.aux_info_type_parameter = value; } }

	protected uint entry_count; 
	public uint EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	protected ulong[] offset; 
	public ulong[] Offset { get { return this.offset; } set { this.offset = value; } }

	public SampleAuxiliaryInformationOffsetsBox(byte version = 0, uint flags = 0): base(IsoStream.FromFourCC("saio"), version, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);

		if ((flags  &  1) ==  1)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.aux_info_type, "aux_info_type"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.aux_info_type_parameter, "aux_info_type_parameter"); 
		}
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entry_count, "entry_count"); 

		if ( version == 0 )
		{
			boxSize += stream.ReadUInt32Array(boxSize, readSize, (uint)( entry_count ),  out this.offset, "offset"); 
		}

		else 
		{
			boxSize += stream.ReadUInt64Array(boxSize, readSize, (uint)( entry_count ),  out this.offset, "offset"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);

		if ((flags  &  1) ==  1)
		{
			boxSize += stream.WriteUInt32( this.aux_info_type, "aux_info_type"); 
			boxSize += stream.WriteUInt32( this.aux_info_type_parameter, "aux_info_type_parameter"); 
		}
		boxSize += stream.WriteUInt32( this.entry_count, "entry_count"); 

		if ( version == 0 )
		{
			boxSize += stream.WriteUInt32Array((uint)( entry_count ),  this.offset, "offset"); 
		}

		else 
		{
			boxSize += stream.WriteUInt64Array((uint)( entry_count ),  this.offset, "offset"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();

		if ((flags  &  1) ==  1)
		{
			boxSize += 32; // aux_info_type
			boxSize += 32; // aux_info_type_parameter
		}
		boxSize += 32; // entry_count

		if ( version == 0 )
		{
			boxSize += ((ulong)( entry_count ) * 32); // offset
		}

		else 
		{
			boxSize += ((ulong)( entry_count ) * 64); // offset
		}
		return boxSize;
	}
}

}
