using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class SequenceOffset() extends Box('snro') {
	int(32)		offset;
}
*/
public partial class SequenceOffset : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class SequenceOffset() extends Box('snro') {
	int(32)		offset;
}"; } }
	public const string TYPE = "snro";
	public override string DisplayName { get { return "SequenceOffset"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("HintSampleEntry(rtp )", "SrtpHintSampleEntry(srtp)"); }


	protected int offset; 
	public int Offset { get { return this.offset; } set { this.offset = value; } }

	public SequenceOffset(): base(IsoStream.FromFourCC("snro"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.offset, "offset"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt32( this.offset, "offset"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // offset
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("offset", 32, this.offset); 
		return boxSize;
	}
}

}
