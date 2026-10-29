using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class HintBox() extends Box('hint') {
	// a track reference (ISO/IEC 14496-12 8.3.3): the IDs of the tracks a hint track hints, one or more
	unsigned int(32) track_IDs[];
}
*/
public partial class HintBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class HintBox() extends Box('hint') {
	// a track reference (ISO/IEC 14496-12 8.3.3): the IDs of the tracks a hint track hints, one or more
	unsigned int(32) track_IDs[];
}"; } }
	public const string TYPE = "hint";
	public override string DisplayName { get { return "HintBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("tref"); }


	protected uint[] track_IDs = []; 
	public uint[] TrackIDs { get { return this.track_IDs; } set { this.track_IDs = value; } }

	public HintBox(): base(IsoStream.FromFourCC("hint"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		/*  a track reference (ISO/IEC 14496-12 8.3.3): the IDs of the tracks a hint track hints, one or more */
		boxSize += stream.ReadUInt32ArrayTillEnd(boxSize, readSize,  out this.track_IDs, "track_IDs"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		/*  a track reference (ISO/IEC 14496-12 8.3.3): the IDs of the tracks a hint track hints, one or more */
		boxSize += stream.WriteUInt32ArrayTillEnd( this.track_IDs, "track_IDs"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		/*  a track reference (ISO/IEC 14496-12 8.3.3): the IDs of the tracks a hint track hints, one or more */
		boxSize += ((ulong)track_IDs.Length * 32); // track_IDs
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		/*  a track reference (ISO/IEC 14496-12 8.3.3): the IDs of the tracks a hint track hints, one or more */
		boxSize += tracer.Field("track_IDs", ((ulong)track_IDs.Length * 32), this.track_IDs); 
		return boxSize;
	}
}

}
