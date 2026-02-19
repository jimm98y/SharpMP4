using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SegmentTypeBox extends GeneralTypeBox ('styp')
{}
*/
public partial class SegmentTypeBox : GeneralTypeBox
{
	public const string TYPE = "styp";
	public override string DisplayName { get { return "SegmentTypeBox"; } }

	public SegmentTypeBox(): base(IsoStream.FromFourCC("styp"))
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
