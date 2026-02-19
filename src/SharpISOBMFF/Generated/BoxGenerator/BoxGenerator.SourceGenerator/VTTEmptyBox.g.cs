using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class VTTEmptyBox() extends Box ('vtte') { }
*/
public partial class VTTEmptyBox : Box
{
	public const string TYPE = "vtte";
	public override string DisplayName { get { return "VTTEmptyBox"; } }

	public VTTEmptyBox(): base(IsoStream.FromFourCC("vtte"))
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
