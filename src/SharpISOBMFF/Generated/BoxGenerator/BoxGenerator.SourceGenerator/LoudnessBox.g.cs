using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class LoudnessBox extends Box('ludt') {
	// not more than one TrackLoudnessInfo box with version>=1 is allowed
	TrackLoudnessInfo[]			loudness;
	// not more than one AlbumLoudnessInfo box with version>=1 is allowed
	AlbumLoudnessInfo[] albumLoudness;
}
*/
public partial class LoudnessBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class LoudnessBox extends Box('ludt') {
	// not more than one TrackLoudnessInfo box with version>=1 is allowed
	TrackLoudnessInfo[]			loudness;
	// not more than one AlbumLoudnessInfo box with version>=1 is allowed
	AlbumLoudnessInfo[] albumLoudness;
}"; } }
	public const string TYPE = "ludt";
	public override string DisplayName { get { return "LoudnessBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("udta"); }

	public IEnumerable<TrackLoudnessInfo> Loudness { get { return this.children.OfType<TrackLoudnessInfo>(); } }
	public IEnumerable<AlbumLoudnessInfo> AlbumLoudness { get { return this.children.OfType<AlbumLoudnessInfo>(); } }

	public LoudnessBox(): base(IsoStream.FromFourCC("ludt"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		/*  not more than one TrackLoudnessInfo box with version>=1 is allowed */
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.loudness, "loudness"); // not more than one AlbumLoudnessInfo box with version>=1 is allowed
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.albumLoudness, "albumLoudness"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		/*  not more than one TrackLoudnessInfo box with version>=1 is allowed */
		// boxSize += stream.WriteBox( this.loudness, "loudness"); // not more than one AlbumLoudnessInfo box with version>=1 is allowed
		// boxSize += stream.WriteBox( this.albumLoudness, "albumLoudness"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		/*  not more than one TrackLoudnessInfo box with version>=1 is allowed */
		// boxSize += IsoStream.CalculateBoxSize(loudness); // loudness
		// boxSize += IsoStream.CalculateBoxSize(albumLoudness); // albumLoudness
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		/*  not more than one TrackLoudnessInfo box with version>=1 is allowed */
		// boxSize += tracer.Field("loudness", IsoStream.CalculateBoxSize(loudness), this.loudness); // not more than one AlbumLoudnessInfo box with version>=1 is allowed
		// boxSize += tracer.Field("albumLoudness", IsoStream.CalculateBoxSize(albumLoudness), this.albumLoudness); 
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
