using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class TimeCodeSampleEntry() extends SampleEntry('tmcd') {
	unsigned int(32) reserved;
	unsigned int(32) timecodeFlags;
	unsigned int(32) timeScale;
	unsigned int(32) frameDuration;
	unsigned int(8) numberOfFrames;
	unsigned int(8) reserved2;
	Box boxes[];
}
*/
public partial class TimeCodeSampleEntry : SampleEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class TimeCodeSampleEntry() extends SampleEntry('tmcd') {
	unsigned int(32) reserved;
	unsigned int(32) timecodeFlags;
	unsigned int(32) timeScale;
	unsigned int(32) frameDuration;
	unsigned int(8) numberOfFrames;
	unsigned int(8) reserved2;
	Box boxes[];
}"; } }
	public const string TYPE = "tmcd";
	public override string DisplayName { get { return "TimeCodeSampleEntry"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stsd", "gmhd"); }


	protected uint reserved; 
	public uint Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected uint timecodeFlags; 
	public uint TimecodeFlags { get { return this.timecodeFlags; } set { this.timecodeFlags = value; } }

	protected uint timeScale; 
	public uint TimeScale { get { return this.timeScale; } set { this.timeScale = value; } }

	protected uint frameDuration; 
	public uint FrameDuration { get { return this.frameDuration; } set { this.frameDuration = value; } }

	protected byte numberOfFrames; 
	public byte NumberOfFrames { get { return this.numberOfFrames; } set { this.numberOfFrames = value; } }

	protected byte reserved2; 
	public byte Reserved2 { get { return this.reserved2; } set { this.reserved2 = value; } }

	public TimeCodeSampleEntry(): base(IsoStream.FromFourCC("tmcd"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.timecodeFlags, "timecodeFlags"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.timeScale, "timeScale"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.frameDuration, "frameDuration"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.numberOfFrames, "numberOfFrames"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.reserved2, "reserved2"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.boxes, "boxes"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.reserved, "reserved"); 
		boxSize += stream.WriteUInt32( this.timecodeFlags, "timecodeFlags"); 
		boxSize += stream.WriteUInt32( this.timeScale, "timeScale"); 
		boxSize += stream.WriteUInt32( this.frameDuration, "frameDuration"); 
		boxSize += stream.WriteUInt8( this.numberOfFrames, "numberOfFrames"); 
		boxSize += stream.WriteUInt8( this.reserved2, "reserved2"); 
		// boxSize += stream.WriteBox( this.boxes, "boxes"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // reserved
		boxSize += 32; // timecodeFlags
		boxSize += 32; // timeScale
		boxSize += 32; // frameDuration
		boxSize += 8; // numberOfFrames
		boxSize += 8; // reserved2
		// boxSize += IsoStream.CalculateBoxSize(boxes); // boxes
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("reserved", 32, this.reserved); 
		boxSize += tracer.Field("timecodeFlags", 32, this.timecodeFlags); 
		boxSize += tracer.Field("timeScale", 32, this.timeScale); 
		boxSize += tracer.Field("frameDuration", 32, this.frameDuration); 
		boxSize += tracer.Field("numberOfFrames", 8, this.numberOfFrames); 
		boxSize += tracer.Field("reserved2", 8, this.reserved2); 
		// boxSize += tracer.Field("boxes", IsoStream.CalculateBoxSize(boxes), this.boxes); 
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
