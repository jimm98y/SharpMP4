﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class SubTrackDefinitionBox extends Box('strd') {
}
*/
public partial class SubTrackDefinitionBox : Box
{
	public const string TYPE = "strd";
	public override string DisplayName { get { return "SubTrackDefinitionBox"; } }

	public SubTrackDefinitionBox(): base(IsoStream.FromFourCC("strd"))
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
