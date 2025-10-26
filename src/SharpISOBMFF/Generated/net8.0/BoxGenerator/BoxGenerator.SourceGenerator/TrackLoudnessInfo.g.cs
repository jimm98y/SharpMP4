using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class TrackLoudnessInfo extends LoudnessBaseBox('tlou') { }
*/
public partial class TrackLoudnessInfo : LoudnessBaseBox
{
	public const string TYPE = "tlou";
	public override string DisplayName { get { return "TrackLoudnessInfo"; } }

	public TrackLoudnessInfo(): base(IsoStream.FromFourCC("tlou"))
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
