using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class AlbumLoudnessInfo extends LoudnessBaseBox ('alou') { }
*/
public partial class AlbumLoudnessInfo : LoudnessBaseBox
{
	public const string TYPE = "alou";
	public override string DisplayName { get { return "AlbumLoudnessInfo"; } }

	public AlbumLoudnessInfo(): base(IsoStream.FromFourCC("alou"))
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
