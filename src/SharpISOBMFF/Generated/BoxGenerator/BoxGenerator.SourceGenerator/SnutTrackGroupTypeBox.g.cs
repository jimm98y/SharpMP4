using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SnutTrackGroupTypeBox() extends FullBox('snut', version = 0, flags = 0)
{
	unsigned int(32) track_group_id;
	// the remaining data may be specified 
	//  for a particular track_group_type
}
*/
public partial class SnutTrackGroupTypeBox : FullBox
{
	public const string TYPE = "snut";
	public override string DisplayName { get { return "SnutTrackGroupTypeBox"; } }

	protected uint track_group_id;  //  the remaining data may be specified 
	public uint TrackGroupId { get { return this.track_group_id; } set { this.track_group_id = value; } }

	public SnutTrackGroupTypeBox(): base(IsoStream.FromFourCC("snut"), 0, 0)
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
}

}
