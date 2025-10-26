using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class TrackTypeBox extends GeneralTypeBox ('ttyp')
{}
*/
public partial class TrackTypeBox : GeneralTypeBox
{
	public const string TYPE = "ttyp";
	public override string DisplayName { get { return "TrackTypeBox"; } }

	public TrackTypeBox(): base(IsoStream.FromFourCC("ttyp"))
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
