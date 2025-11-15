using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class TrackGroupBox extends Box('trgr') {
}
*/
public partial class TrackGroupBox : Box
{
	public const string TYPE = "trgr";
	public override string DisplayName { get { return "TrackGroupBox"; } }

	public TrackGroupBox(): base(IsoStream.FromFourCC("trgr"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
