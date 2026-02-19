using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class CompressedSubsegmentIndexBox
	extends CompressedBox('!ssx', 'ssix') {
}
*/
public partial class CompressedSubsegmentIndexBox : CompressedBox
{
	public const string TYPE = "ssix";
	public override string DisplayName { get { return "CompressedSubsegmentIndexBox"; } }

	public CompressedSubsegmentIndexBox(): base(IsoStream.FromFourCC("ssix"))
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
