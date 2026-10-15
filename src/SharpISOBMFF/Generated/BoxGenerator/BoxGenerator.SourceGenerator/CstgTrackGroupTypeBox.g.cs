using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class CstgTrackGroupTypeBox() extends FullBox('cstg', version = 0, flags = 0)
{
	unsigned int(32) track_group_id;
	// the remaining data may be specified 
	//  for a particular track_group_type
}
*/
public partial class CstgTrackGroupTypeBox : FullBox
{
	public const string TYPE = "cstg";
	public override string DisplayName { get { return "CstgTrackGroupTypeBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("trgr"); }


	protected uint track_group_id;  //  the remaining data may be specified 
	public uint TrackGroupId { get { return this.track_group_id; } set { this.track_group_id = value; } }

	public CstgTrackGroupTypeBox(): base(IsoStream.FromFourCC("cstg"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.track_group_id, "track_group_id"); // the remaining data may be specified 
		/*   for a particular track_group_type */
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.track_group_id, "track_group_id"); // the remaining data may be specified 
		/*   for a particular track_group_type */
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // track_group_id
		/*   for a particular track_group_type */
		return boxSize;
	}

	public override ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += base.ToString(builder);
		boxSize += builder.Field("track_group_id", 32, this.track_group_id); // the remaining data may be specified 
		/*   for a particular track_group_type */
		return boxSize;
	}
}

}
