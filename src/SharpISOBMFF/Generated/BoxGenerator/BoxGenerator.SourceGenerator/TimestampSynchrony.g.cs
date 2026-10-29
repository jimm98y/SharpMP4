using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class TimestampSynchrony() extends Box('tssy') {
	unsigned int(6) reserved;
	unsigned int(2) timestamp_sync;
}
*/
public partial class TimestampSynchrony : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class TimestampSynchrony() extends Box('tssy') {
	unsigned int(6) reserved;
	unsigned int(2) timestamp_sync;
}"; } }
	public const string TYPE = "tssy";
	public override string DisplayName { get { return "TimestampSynchrony"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("ReceivedRtpHintSampleEntry(rrtp)"); }


	protected byte reserved; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte timestamp_sync; 
	public byte TimestampSync { get { return this.timestamp_sync; } set { this.timestamp_sync = value; } }

	public TimestampSynchrony(): base(IsoStream.FromFourCC("tssy"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.timestamp_sync, "timestamp_sync"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(6,  this.reserved, "reserved"); 
		boxSize += stream.WriteBits(2,  this.timestamp_sync, "timestamp_sync"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 6; // reserved
		boxSize += 2; // timestamp_sync
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("reserved", 6, this.reserved); 
		boxSize += tracer.Field("timestamp_sync", 2, this.timestamp_sync); 
		return boxSize;
	}
}

}
