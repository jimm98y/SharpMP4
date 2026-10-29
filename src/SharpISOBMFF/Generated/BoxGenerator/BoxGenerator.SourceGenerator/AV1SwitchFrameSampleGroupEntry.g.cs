using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class AV1SwitchFrameSampleGroupEntry
extends VisualSampleGroupEntry('av1s')
{
}

*/
public partial class AV1SwitchFrameSampleGroupEntry : VisualSampleGroupEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class AV1SwitchFrameSampleGroupEntry
extends VisualSampleGroupEntry('av1s')
{
}
"; } }
	public const string TYPE = "av1s";
	public override string DisplayName { get { return "AV1SwitchFrameSampleGroupEntry"; } }

	public AV1SwitchFrameSampleGroupEntry(): base(IsoStream.FromFourCC("av1s"))
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
