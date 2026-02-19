using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

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
}

}
