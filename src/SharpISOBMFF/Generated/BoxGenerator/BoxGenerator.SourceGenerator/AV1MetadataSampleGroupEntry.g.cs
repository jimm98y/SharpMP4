using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class AV1MetadataSampleGroupEntry
extends VisualSampleGroupEntry('av1M')
{
}

*/
public partial class AV1MetadataSampleGroupEntry : VisualSampleGroupEntry
{
	public const string TYPE = "av1M";
	public override string DisplayName { get { return "AV1MetadataSampleGroupEntry"; } }

	public AV1MetadataSampleGroupEntry(): base(IsoStream.FromFourCC("av1M"))
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
