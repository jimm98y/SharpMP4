using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class TemporalSubLayerEntry() extends VisualSampleGroupEntry ('tsas')
{
}
*/
public partial class TemporalSubLayerEntry : VisualSampleGroupEntry
{
	public const string TYPE = "tsas";
	public override string DisplayName { get { return "TemporalSubLayerEntry"; } }

	public TemporalSubLayerEntry(): base(IsoStream.FromFourCC("tsas"))
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
