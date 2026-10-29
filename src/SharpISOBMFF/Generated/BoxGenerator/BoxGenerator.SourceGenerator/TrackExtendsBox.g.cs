using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

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
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class TrackExtendsBox extends FullBox('trex', 0, 0){
	unsigned int(32)	track_ID;
	unsigned int(32)	default_sample_description_index;
	unsigned int(32)	default_sample_duration;
	unsigned int(32)	default_sample_size;
	unsigned int(32)	default_sample_flags;
}"; } }
	public const string TYPE = "trex";
	public override string DisplayName { get { return "TrackExtendsBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("mvex"); }


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

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("track_ID", 32, this.track_ID); 
		boxSize += tracer.Field("default_sample_description_index", 32, this.default_sample_description_index); 
		boxSize += tracer.Field("default_sample_duration", 32, this.default_sample_duration); 
		boxSize += tracer.Field("default_sample_size", 32, this.default_sample_size); 
		boxSize += tracer.Field("default_sample_flags", 32, this.default_sample_flags); 
		return boxSize;
	}
}

}
