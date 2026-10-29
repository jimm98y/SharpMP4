using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
abstract class AudioSampleGroupEntry (unsigned int(32) grouping_type) extends
	SampleGroupDescriptionEntry (grouping_type)
{
}
*/
public abstract partial class AudioSampleGroupEntry : SampleGroupDescriptionEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"abstract class AudioSampleGroupEntry (unsigned int(32) grouping_type) extends
	SampleGroupDescriptionEntry (grouping_type)
{
}"; } }
	public override string DisplayName { get { return "AudioSampleGroupEntry"; } }

	public AudioSampleGroupEntry(uint grouping_type): base(grouping_type)
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

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		return boxSize;
	}
}

}
