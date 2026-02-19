using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class DfxpSampleEntry() extends SampleEntry('dfxp') {
 } 
*/
public partial class DfxpSampleEntry : SampleEntry
{
	public const string TYPE = "dfxp";
	public override string DisplayName { get { return "DfxpSampleEntry"; } }

	public DfxpSampleEntry(): base(IsoStream.FromFourCC("dfxp"))
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
