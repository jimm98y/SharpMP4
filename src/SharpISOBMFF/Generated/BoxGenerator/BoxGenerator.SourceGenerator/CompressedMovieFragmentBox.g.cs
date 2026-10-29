using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class CompressedMovieFragmentBox
	extends CompressedBox('!mof', 'moof') {
}
*/
public partial class CompressedMovieFragmentBox : CompressedBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class CompressedMovieFragmentBox
	extends CompressedBox('!mof', 'moof') {
}"; } }
	public const string TYPE = "!mof";
	public override string DisplayName { get { return "CompressedMovieFragmentBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	// its definitions place it in no container: wherever it is, it is not misplaced
	public override bool ValidatePlacement() { return IsPlacedIn(); }


	public CompressedMovieFragmentBox(): base(IsoStream.FromFourCC("!mof"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
