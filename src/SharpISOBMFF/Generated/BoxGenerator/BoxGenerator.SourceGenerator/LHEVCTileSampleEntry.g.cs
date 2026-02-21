using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class LHEVCTileSampleEntry() extends VisualSampleEntry ('lht1'){
}
*/
public partial class LHEVCTileSampleEntry : VisualSampleEntry
{
	public const string TYPE = "lht1";
	public override string DisplayName { get { return "LHEVCTileSampleEntry"; } }

	public LHEVCTileSampleEntry(): base(IsoStream.FromFourCC("lht1"))
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
