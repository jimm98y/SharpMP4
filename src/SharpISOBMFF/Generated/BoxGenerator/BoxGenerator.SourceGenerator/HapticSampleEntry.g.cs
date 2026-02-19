using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class HapticSampleEntry(codingname)
	extends SampleEntry(codingname) {
	Box()[]	otherboxes;
}
*/
public partial class HapticSampleEntry : SampleEntry
{
	public override string DisplayName { get { return "HapticSampleEntry"; } }
	public IEnumerable<Box> Otherboxes { get { return this.children.OfType<Box>(); } }

	public HapticSampleEntry(uint codingname = 0): base(codingname)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.otherboxes, "otherboxes"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.otherboxes, "otherboxes"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(otherboxes); // otherboxes
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
