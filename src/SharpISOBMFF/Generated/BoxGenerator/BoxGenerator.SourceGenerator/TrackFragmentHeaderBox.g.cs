using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class TrackFragmentHeaderBox
                extends FullBox('tfhd', 0, tf_flags){
        unsigned int(32)	track_ID;
        // all the following are optional fields
        // their presence is indicated by bits in the tf_flags
        if (flags & 0x1) {
           unsigned int(64)	base_data_offset;
        }
        if (flags & 0x2) {
           unsigned int(32)	sample_description_index;
        }
        if (flags & 0x8) {
           unsigned int(32)	default_sample_duration;
        }
        if (flags & 0x10) {
           unsigned int(32)	default_sample_size;
        }
        if (flags & 0x20) {
           unsigned int(32)	default_sample_flags;
        }
    }
*/
public partial class TrackFragmentHeaderBox : FullBox
{
	public const string TYPE = "tfhd";
	public override string DisplayName { get { return "TrackFragmentHeaderBox"; } }

	protected uint track_ID;  //  all the following are optional fields
	public uint TrackID { get { return this.track_ID; } set { this.track_ID = value; } }

	protected ulong base_data_offset; 
	public ulong BaseDataOffset { get { return this.base_data_offset; } set { this.base_data_offset = value; } }

	protected uint sample_description_index; 
	public uint SampleDescriptionIndex { get { return this.sample_description_index; } set { this.sample_description_index = value; } }

	protected uint default_sample_duration; 
	public uint DefaultSampleDuration { get { return this.default_sample_duration; } set { this.default_sample_duration = value; } }

	protected uint default_sample_size; 
	public uint DefaultSampleSize { get { return this.default_sample_size; } set { this.default_sample_size = value; } }

	protected uint default_sample_flags; 
	public uint DefaultSampleFlags { get { return this.default_sample_flags; } set { this.default_sample_flags = value; } }

	public TrackFragmentHeaderBox(uint tf_flags = 0): base(IsoStream.FromFourCC("tfhd"), 0, tf_flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadUInt32(boxSize, readSize,  out this.track_ID, "track_ID"); // all the following are optional fields
		/*  their presence is indicated by bits in the tf_flags */

		if ((flags  &  0x1) ==  0x1)
		{
			boxSize += stream.ReadUInt64(boxSize, readSize,  out this.base_data_offset, "base_data_offset"); 
		}

		if ((flags  &  0x2) ==  0x2)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_description_index, "sample_description_index"); 
		}

		if ((flags  &  0x8) ==  0x8)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.default_sample_duration, "default_sample_duration"); 
		}

		if ((flags  &  0x10) ==  0x10)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.default_sample_size, "default_sample_size"); 
		}

		if ((flags  &  0x20) ==  0x20)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.default_sample_flags, "default_sample_flags"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.track_ID, "track_ID"); // all the following are optional fields
		/*  their presence is indicated by bits in the tf_flags */

		if ((flags  &  0x1) ==  0x1)
		{
			boxSize += stream.WriteUInt64( this.base_data_offset, "base_data_offset"); 
		}

		if ((flags  &  0x2) ==  0x2)
		{
			boxSize += stream.WriteUInt32( this.sample_description_index, "sample_description_index"); 
		}

		if ((flags  &  0x8) ==  0x8)
		{
			boxSize += stream.WriteUInt32( this.default_sample_duration, "default_sample_duration"); 
		}

		if ((flags  &  0x10) ==  0x10)
		{
			boxSize += stream.WriteUInt32( this.default_sample_size, "default_sample_size"); 
		}

		if ((flags  &  0x20) ==  0x20)
		{
			boxSize += stream.WriteUInt32( this.default_sample_flags, "default_sample_flags"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // track_ID
		/*  their presence is indicated by bits in the tf_flags */

		if ((flags  &  0x1) ==  0x1)
		{
			boxSize += 64; // base_data_offset
		}

		if ((flags  &  0x2) ==  0x2)
		{
			boxSize += 32; // sample_description_index
		}

		if ((flags  &  0x8) ==  0x8)
		{
			boxSize += 32; // default_sample_duration
		}

		if ((flags  &  0x10) ==  0x10)
		{
			boxSize += 32; // default_sample_size
		}

		if ((flags  &  0x20) ==  0x20)
		{
			boxSize += 32; // default_sample_flags
		}
		return boxSize;
	}
}

}
