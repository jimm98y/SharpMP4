using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
abstract class HintSampleGroupEntry (unsigned int(32) grouping_type) extends 
SampleGroupDescriptionEntry (grouping_type) 
{ 
} 


*/
public abstract partial class HintSampleGroupEntry : SampleGroupDescriptionEntry
{
	public override string DisplayName { get { return "HintSampleGroupEntry"; } }

	public HintSampleGroupEntry(uint grouping_type): base(grouping_type)
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
