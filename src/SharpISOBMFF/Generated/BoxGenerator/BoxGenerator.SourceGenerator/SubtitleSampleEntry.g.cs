using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class SubtitleSampleEntry(codingname) extends SampleEntry (codingname) { 
} 


*/
public partial class SubtitleSampleEntry : SampleEntry
{
	public override string DisplayName { get { return "SubtitleSampleEntry"; } }

	public SubtitleSampleEntry(uint codingname = 0): base(codingname)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		return boxSize;
	}
}

}
