using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class TrackReferenceBox extends Box('tref') {
	TrackReferenceTypeBox [];
}
*/
public partial class TrackReferenceBox : Box
{
	public const string TYPE = "tref";
	public override string DisplayName { get { return "TrackReferenceBox"; } }
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
}

}
