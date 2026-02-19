using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SubtitleMediaHeaderBox
	extends FullBox ('sthd', version = 0, flags = 0){
}
*/
public partial class SubtitleMediaHeaderBox : FullBox
{
	public const string TYPE = "sthd";
	public override string DisplayName { get { return "SubtitleMediaHeaderBox"; } }

	public SubtitleMediaHeaderBox(): base(IsoStream.FromFourCC("sthd"), 0, 0)
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
