using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// Canon's metadata of a CR3 (and of its MP4 videos): CNCV, CCTP, CTBO, CMT1-4, THMB
aligned(8) class CanonMetadataUuidBox() extends Box('uuid 85c0b687820f11e08111f4ce462b6a48') {
 Box boxes[];
}

*/
public partial class CanonMetadataUuidBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"// Canon's metadata of a CR3 (and of its MP4 videos): CNCV, CCTP, CTBO, CMT1-4, THMB
aligned(8) class CanonMetadataUuidBox() extends Box('uuid 85c0b687820f11e08111f4ce462b6a48') {
 Box boxes[];
}
"; } }
	public const string TYPE = "uuid";
	public override string DisplayName { get { return "CanonMetadataUuidBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn(BoxPlacements.Of(this) ?? new[] { "*" }); }


	public CanonMetadataUuidBox(): base(IsoStream.FromFourCC("uuid"), ConvertEx.FromHexString("85c0b687820f11e08111f4ce462b6a48"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.boxes, "boxes"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.boxes, "boxes"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(boxes); // boxes
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		// boxSize += tracer.Field("boxes", IsoStream.CalculateBoxSize(boxes), this.boxes); 
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
