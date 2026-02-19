using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class OriginalFileTypeBox extends Box('otyp') {
}
*/
public partial class OriginalFileTypeBox : Box
{
	public const string TYPE = "otyp";
	public override string DisplayName { get { return "OriginalFileTypeBox"; } }

	public OriginalFileTypeBox(): base(IsoStream.FromFourCC("otyp"))
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
