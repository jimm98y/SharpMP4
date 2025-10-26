using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class TrackExtendsBox extends FullBox('trex', 0, 0){
	unsigned int(32)	track_ID;
	unsigned int(32)	default_sample_description_index;
	unsigned int(32)	default_sample_duration;
	unsigned int(32)	default_sample_size;
	unsigned int(32)	default_sample_flags;
}
*/
public partial class TrackExtendsBox : FullBox
{
	public const string TYPE = "trex";
	public override string DisplayName { get { return "TrackExtendsBox"; } }

	protected uint track_ID; 
	public uint TrackID { get { return this.track_ID; } set { this.track_ID = value; } }

	protected uint default_sample_description_index; 
	public uint DefaultSampleDescriptionIndex { get { return this.default_sample_description_index; } set { this.default_sample_description_index = value; } }

	protected uint default_sample_duration; 
	public uint DefaultSampleDuration { get { return this.default_sample_duration; } set { this.default_sample_duration = value; } }

	protected uint default_sample_size; 
	public uint DefaultSampleSize { get { return this.default_sample_size; } set { this.default_sample_size = value; } }

	protected uint default_sample_flags; 
	public uint DefaultSampleFlags { get { return this.default_sample_flags; } set { this.default_sample_flags = value; } }

	public TrackExtendsBox(): base(IsoStream.FromFourCC("trex"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.track_ID, "track_ID"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.default_sample_description_index, "default_sample_description_index"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.default_sample_duration, "default_sample_duration"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.default_sample_size, "default_sample_size"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.default_sample_flags, "default_sample_flags"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.track_ID, "track_ID"); 
		boxSize += stream.WriteUInt32( this.default_sample_description_index, "default_sample_description_index"); 
		boxSize += stream.WriteUInt32( this.default_sample_duration, "default_sample_duration"); 
		boxSize += stream.WriteUInt32( this.default_sample_size, "default_sample_size"); 
		boxSize += stream.WriteUInt32( this.default_sample_flags, "default_sample_flags"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // track_ID
		boxSize += 32; // default_sample_description_index
		boxSize += 32; // default_sample_duration
		boxSize += 32; // default_sample_size
		boxSize += 32; // default_sample_flags
		return boxSize;
	}
}

}
