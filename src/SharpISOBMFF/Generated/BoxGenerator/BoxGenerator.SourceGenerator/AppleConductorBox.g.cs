using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class AppleConductorBox() extends Box('©con') {
 Box boxes[];
 } 
*/
public partial class AppleConductorBox : Box
{
	public const string TYPE = "©con";
	public override string DisplayName { get { return "AppleConductorBox"; } }
	public IEnumerable<Box> Boxes { get { return this.children.OfType<Box>(); } }

	public AppleConductorBox(): base(IsoStream.FromFourCC("©con"))
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
