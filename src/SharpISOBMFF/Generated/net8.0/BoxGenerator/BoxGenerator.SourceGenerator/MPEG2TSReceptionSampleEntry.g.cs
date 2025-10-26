using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class MPEG2TSReceptionSampleEntry extends MPEG2TSSampleEntry('rm2t') {}
*/
public partial class MPEG2TSReceptionSampleEntry : MPEG2TSSampleEntry
{
	public const string TYPE = "rm2t";
	public override string DisplayName { get { return "MPEG2TSReceptionSampleEntry"; } }

	public MPEG2TSReceptionSampleEntry(): base(IsoStream.FromFourCC("rm2t"))
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
