using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class TrackExtensionPropertiesBox extends FullBox('trep', 0, 0) {
	unsigned int(32) track_ID;
	// Any number of boxes may follow
}
*/
public partial class TrackExtensionPropertiesBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class TrackExtensionPropertiesBox extends FullBox('trep', 0, 0) {
	unsigned int(32) track_ID;
	// Any number of boxes may follow
}"; } }
	public const string TYPE = "trep";
	public override string DisplayName { get { return "TrackExtensionPropertiesBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("mvex"); }


	protected uint track_ID;  //  Any number of boxes may follow
	public uint TrackID { get { return this.track_ID; } set { this.track_ID = value; } }

	public TrackExtensionPropertiesBox(): base(IsoStream.FromFourCC("trep"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.track_ID, "track_ID"); // Any number of boxes may follow
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.track_ID, "track_ID"); // Any number of boxes may follow
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // track_ID
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("track_ID", 32, this.track_ID); // Any number of boxes may follow
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
