using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

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
	public const string TYPE = "trep";
	public override string DisplayName { get { return "TrackExtensionPropertiesBox"; } }

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
}

}
