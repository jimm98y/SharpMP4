using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class OMAInfoURLBox() extends FullBox('infu') {
	string content;
}
*/
public partial class OMAInfoURLBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class OMAInfoURLBox() extends FullBox('infu') {
	string content;
}"; } }
	public const string TYPE = "infu";
	public override string DisplayName { get { return "OMAInfoURLBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("udta"); }


	protected BinaryUTF8String content; 
	public BinaryUTF8String Content { get { return this.content; } set { this.content = value; } }

	public OMAInfoURLBox(): base(IsoStream.FromFourCC("infu"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.content, "content"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.content, "content"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(content); // content
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("content", IsoStream.CalculateStringSize(content), this.content); 
		return boxSize;
	}
}

}
