using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class GooglePingUrlBox() extends Box('gspu') {
 Box boxes[];
 } 
*/
public partial class GooglePingUrlBox : Box
{
	public const string TYPE = "gspu";
	public override string DisplayName { get { return "GooglePingUrlBox"; } }
	public IEnumerable<Box> Boxes { get { return this.children.OfType<Box>(); } }

	public GooglePingUrlBox(): base(IsoStream.FromFourCC("gspu"))
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
}

}
