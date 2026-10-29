using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// ms since 1970, local time (ExifTool)
aligned(8) class PittasoftStartTimeBox() extends Box('sttm') {
 unsigned int(64) startTime;
}

*/
public partial class PittasoftStartTimeBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"// ms since 1970, local time (ExifTool)
aligned(8) class PittasoftStartTimeBox() extends Box('sttm') {
 unsigned int(64) startTime;
}
"; } }
	public const string TYPE = "sttm";
	public override string DisplayName { get { return "PittasoftStartTimeBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("free"); }


	protected ulong startTime; 
	public ulong StartTime { get { return this.startTime; } set { this.startTime = value; } }

	public PittasoftStartTimeBox(): base(IsoStream.FromFourCC("sttm"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt64(boxSize, readSize,  out this.startTime, "startTime"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt64( this.startTime, "startTime"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 64; // startTime
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("startTime", 64, this.startTime); 
		return boxSize;
	}
}

}
