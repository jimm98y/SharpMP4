using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class NullMediaHeaderBox
	extends FullBox('nmhd', version = 0, flags) {
}
*/
public partial class NullMediaHeaderBox : FullBox
{
	public const string TYPE = "nmhd";
	public override string DisplayName { get { return "NullMediaHeaderBox"; } }

	public NullMediaHeaderBox(uint flags = 0): base(IsoStream.FromFourCC("nmhd"), 0, flags)
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
