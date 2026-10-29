using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class CompressedSubsegmentIndexBox
	extends CompressedBox('!ssx', 'ssix') {
}
*/
public partial class CompressedSubsegmentIndexBox : CompressedBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class CompressedSubsegmentIndexBox
	extends CompressedBox('!ssx', 'ssix') {
}"; } }
	public const string TYPE = "!ssx";
	public override string DisplayName { get { return "CompressedSubsegmentIndexBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	// its definitions place it in no container: wherever it is, it is not misplaced
	public override bool ValidatePlacement() { return IsPlacedIn(); }


	public CompressedSubsegmentIndexBox(): base(IsoStream.FromFourCC("!ssx"))
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
