using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class TrackReferenceBox extends Box('tref') {
	TrackReferenceTypeBox [];
}
*/
public partial class TrackReferenceBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class TrackReferenceBox extends Box('tref') {
	TrackReferenceTypeBox [];
}"; } }
	public const string TYPE = "tref";
	public override string DisplayName { get { return "TrackReferenceBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("trak"); }

	public IEnumerable<TrackReferenceTypeBox> _TrackReferenceTypeBox { get { return this.children.OfType<TrackReferenceTypeBox>(); } }

	public TrackReferenceBox(): base(IsoStream.FromFourCC("tref"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.TrackReferenceTypeBox, "TrackReferenceTypeBox"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.TrackReferenceTypeBox, "TrackReferenceTypeBox"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(TrackReferenceTypeBox); // TrackReferenceTypeBox
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		// boxSize += tracer.Field("TrackReferenceTypeBox", IsoStream.CalculateBoxSize(TrackReferenceTypeBox), this.TrackReferenceTypeBox); 
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
