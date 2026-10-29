using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class MetaDataInlineKeysPresentBox extends Box('keyi') {
	unsigned int(8) inlineKeyValueBoxesPresent;
}
*/
public partial class MetaDataInlineKeysPresentBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class MetaDataInlineKeysPresentBox extends Box('keyi') {
	unsigned int(8) inlineKeyValueBoxesPresent;
}"; } }
	public const string TYPE = "keyi";
	public override string DisplayName { get { return "MetaDataInlineKeysPresentBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("*"); }


	protected byte inlineKeyValueBoxesPresent; 
	public byte InlineKeyValueBoxesPresent { get { return this.inlineKeyValueBoxesPresent; } set { this.inlineKeyValueBoxesPresent = value; } }

	public MetaDataInlineKeysPresentBox(): base(IsoStream.FromFourCC("keyi"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.inlineKeyValueBoxesPresent, "inlineKeyValueBoxesPresent"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.inlineKeyValueBoxesPresent, "inlineKeyValueBoxesPresent"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // inlineKeyValueBoxesPresent
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("inlineKeyValueBoxesPresent", 8, this.inlineKeyValueBoxesPresent); 
		return boxSize;
	}
}

}
