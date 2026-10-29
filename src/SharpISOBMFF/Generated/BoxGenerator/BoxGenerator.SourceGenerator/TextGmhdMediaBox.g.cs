using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class TextGmhdMediaBox() extends Box('text') {
	unsigned int(8) textData[36];
}
*/
public partial class TextGmhdMediaBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class TextGmhdMediaBox() extends Box('text') {
	unsigned int(8) textData[36];
}"; } }
	public const string TYPE = "text";
	public override string DisplayName { get { return "TextGmhdMediaBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stsd", "gmhd"); }


	protected byte[] textData = new byte[36]; 
	public byte[] TextData { get { return this.textData; } set { this.textData = value; } }

	public TextGmhdMediaBox(): base(IsoStream.FromFourCC("text"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 36,  out this.textData, "textData"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8Array(36,  this.textData, "textData"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 36 * 8; // textData
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("textData", 36 * 8, this.textData); 
		return boxSize;
	}
}

}
