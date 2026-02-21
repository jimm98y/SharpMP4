using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class FontSampleEntry(codingname) extends SampleEntry (codingname){
	//other boxes from derived specifications
}
*/
public partial class FontSampleEntry : SampleEntry
{
	public override string DisplayName { get { return "FontSampleEntry"; } }

	public FontSampleEntry(uint codingname = 0): base(codingname)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		/* other boxes from derived specifications */
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		/* other boxes from derived specifications */
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		/* other boxes from derived specifications */
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
