using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class SynchronizedTrackBox() extends Box ('sync'){
 // a track reference (ISO/IEC 14496-12 8.3.3, QuickTime): the IDs of the tracks referred to
 unsigned int(32) track_IDs[];
 }
*/
public partial class SynchronizedTrackBox : Box
{
	public const string TYPE = "sync";
	public override string DisplayName { get { return "SynchronizedTrackBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("tref"); }


	protected uint[] track_IDs; 
	public uint[] TrackIDs { get { return this.track_IDs; } set { this.track_IDs = value; } }

	public SynchronizedTrackBox(): base(IsoStream.FromFourCC("sync"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		/*  a track reference (ISO/IEC 14496-12 8.3.3, QuickTime): the IDs of the tracks referred to */
		boxSize += stream.ReadUInt32ArrayTillEnd(boxSize, readSize,  out this.track_IDs, "track_IDs"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		/*  a track reference (ISO/IEC 14496-12 8.3.3, QuickTime): the IDs of the tracks referred to */
		boxSize += stream.WriteUInt32ArrayTillEnd( this.track_IDs, "track_IDs"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		/*  a track reference (ISO/IEC 14496-12 8.3.3, QuickTime): the IDs of the tracks referred to */
		boxSize += ((ulong)track_IDs.Length * 32); // track_IDs
		return boxSize;
	}

	public override ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += base.ToString(builder);
		/*  a track reference (ISO/IEC 14496-12 8.3.3, QuickTime): the IDs of the tracks referred to */
		boxSize += builder.Field("track_IDs", ((ulong)track_IDs.Length * 32), this.track_IDs); 
		return boxSize;
	}
}

}
