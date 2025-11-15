using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class AppleWaveBox() extends Box('wave') {
 Box boxes[];
 } 
*/
public partial class AppleWaveBox : Box
{
	public const string TYPE = "wave";
	public override string DisplayName { get { return "AppleWaveBox"; } }
	public IEnumerable<Box> Boxes { get { return this.children.OfType<Box>(); } }

	public AppleWaveBox(): base(IsoStream.FromFourCC("wave"))
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
