using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class MPEG2TSServerSampleEntry extends MPEG2TSSampleEntry('sm2t') {}
*/
public partial class MPEG2TSServerSampleEntry : MPEG2TSSampleEntry
{
	public const string TYPE = "sm2t";
	public override string DisplayName { get { return "MPEG2TSServerSampleEntry"; } }

	public MPEG2TSServerSampleEntry(): base(IsoStream.FromFourCC("sm2t"))
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
