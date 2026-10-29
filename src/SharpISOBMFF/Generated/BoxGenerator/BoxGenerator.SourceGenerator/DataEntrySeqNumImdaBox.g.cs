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
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class DataEntrySeqNumImdaBox (bit(24) flags)
	extends DataEntryBaseBox ('snim', flags) {
}"; } }
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

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		return boxSize;
	}
}

}
