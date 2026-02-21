using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class CompressedSegmentIndexBox
	extends CompressedBox('!six', 'sidx') {
}

*/
public partial class CompressedSegmentIndexBox : CompressedBox
{
	public const string TYPE = "sidx";
	public override string DisplayName { get { return "CompressedSegmentIndexBox"; } }

	public CompressedSegmentIndexBox(): base(IsoStream.FromFourCC("sidx"))
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
