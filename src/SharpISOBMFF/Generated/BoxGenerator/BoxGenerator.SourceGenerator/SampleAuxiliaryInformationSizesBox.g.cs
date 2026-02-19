using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SampleAuxiliaryInformationSizesBox
	extends FullBox('saiz', version = 0, flags)
{
	if (flags & 1) {
		unsigned int(32) aux_info_type;
		unsigned int(32) aux_info_type_parameter;
	}
	unsigned int(8) default_sample_info_size;
	unsigned int(32) sample_count;
	if (default_sample_info_size == 0) {
		unsigned int(8) sample_info_size[ sample_count ];
	}
}
*/
public partial class SampleAuxiliaryInformationSizesBox : FullBox
{
	public const string TYPE = "saiz";
	public override string DisplayName { get { return "SampleAuxiliaryInformationSizesBox"; } }

	protected uint aux_info_type; 
	public uint AuxInfoType { get { return this.aux_info_type; } set { this.aux_info_type = value; } }

	protected uint aux_info_type_parameter; 
	public uint AuxInfoTypeParameter { get { return this.aux_info_type_parameter; } set { this.aux_info_type_parameter = value; } }

	protected byte default_sample_info_size; 
	public byte DefaultSampleInfoSize { get { return this.default_sample_info_size; } set { this.default_sample_info_size = value; } }

	protected uint sample_count; 
	public uint SampleCount { get { return this.sample_count; } set { this.sample_count = value; } }

	protected byte[] sample_info_size; 
	public byte[] SampleInfoSize { get { return this.sample_info_size; } set { this.sample_info_size = value; } }

	public SampleAuxiliaryInformationSizesBox(uint flags = 0): base(IsoStream.FromFourCC("saiz"), 0, flags)
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
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.default_sample_info_size, "default_sample_info_size"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_count, "sample_count"); 

		if (default_sample_info_size == 0)
		{
			boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)( sample_count ),  out this.sample_info_size, "sample_info_size"); 
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
		boxSize += stream.WriteUInt8( this.default_sample_info_size, "default_sample_info_size"); 
		boxSize += stream.WriteUInt32( this.sample_count, "sample_count"); 

		if (default_sample_info_size == 0)
		{
			boxSize += stream.WriteUInt8Array((uint)( sample_count ),  this.sample_info_size, "sample_info_size"); 
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
		boxSize += 8; // default_sample_info_size
		boxSize += 32; // sample_count

		if (default_sample_info_size == 0)
		{
			boxSize += ((ulong)( sample_count ) * 8); // sample_info_size
		}
		return boxSize;
	}
}

}
