using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class MultiSourcePresentationBox extends TrackGroupTypeBox('msrc') {
}
*/
public partial class MultiSourcePresentationBox : TrackGroupTypeBox
{
	public const string TYPE = "msrc";
	public override string DisplayName { get { return "MultiSourcePresentationBox"; } }

	public MultiSourcePresentationBox(): base(IsoStream.FromFourCC("msrc"))
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
