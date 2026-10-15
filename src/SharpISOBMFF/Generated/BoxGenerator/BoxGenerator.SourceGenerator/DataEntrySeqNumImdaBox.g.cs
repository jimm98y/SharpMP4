using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

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
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("dref"); }


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

	public override ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += base.ToString(builder);
		return boxSize;
	}
}

}
