using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class HintMinRelativeTime extends Box('tmin') {
	int(32)		time; }		// smallest relative transmission time, milliseconds

*/
public partial class HintMinRelativeTime : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class HintMinRelativeTime extends Box('tmin') {
	int(32)		time; }		// smallest relative transmission time, milliseconds
"; } }
	public const string TYPE = "tmin";
	public override string DisplayName { get { return "HintMinRelativeTime"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("hinf"); }


	protected int time; 
	public int Time { get { return this.time; } set { this.time = value; } }

	public HintMinRelativeTime(): base(IsoStream.FromFourCC("tmin"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.time, "time"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt32( this.time, "time"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // time
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("time", 32, this.time); 
		return boxSize;
	}
}

}
