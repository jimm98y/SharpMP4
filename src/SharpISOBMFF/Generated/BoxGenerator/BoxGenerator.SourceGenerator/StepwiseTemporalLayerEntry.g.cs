using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class StepwiseTemporalLayerEntry() extends VisualSampleGroupEntry ('stsa')
{
}
*/
public partial class StepwiseTemporalLayerEntry : VisualSampleGroupEntry
{
	public const string TYPE = "stsa";
	public override string DisplayName { get { return "StepwiseTemporalLayerEntry"; } }

	public StepwiseTemporalLayerEntry(): base(IsoStream.FromFourCC("stsa"))
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
