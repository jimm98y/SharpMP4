using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class TrackReferenceTypeBox (unsigned int(32) reference_type) extends Box(reference_type) {
	unsigned int(32) track_IDs[];
}
*/
public partial class TrackReferenceTypeBox : Box
{
	public override string DisplayName { get { return "TrackReferenceTypeBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn(BoxPlacements.Of(this) ?? new[] { "tref", "iref" }); }


	protected uint[] track_IDs; 
	public uint[] TrackIDs { get { return this.track_IDs; } set { this.track_IDs = value; } }

	public TrackReferenceTypeBox(uint reference_type): base(reference_type)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32ArrayTillEnd(boxSize, readSize,  out this.track_IDs, "track_IDs"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32ArrayTillEnd( this.track_IDs, "track_IDs"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += ((ulong)track_IDs.Length * 32); // track_IDs
		return boxSize;
	}

	public override ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += base.ToString(builder);
		boxSize += builder.Field("track_IDs", ((ulong)track_IDs.Length * 32), this.track_IDs); 
		return boxSize;
	}
}

}
