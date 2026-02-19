using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class DataEntrySeqNumImdaBox (bit(24) flags)
	extends DataEntryBaseBox ('snim', flags) {
}
*/
public partial class DataEntrySeqNumImdaBox : DataEntryBaseBox
{
	public const string TYPE = "snim";
	public override string DisplayName { get { return "DataEntrySeqNumImdaBox"; } }

	public DataEntrySeqNumImdaBox(uint flags = 0): base(IsoStream.FromFourCC("snim"), flags)
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
