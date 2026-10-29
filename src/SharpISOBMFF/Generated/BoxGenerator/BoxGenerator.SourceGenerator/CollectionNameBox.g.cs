using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class CollectionNameBox() extends FullBox('coll', version = 0, 0) {
	bit(1) reserved = 0;
	unsigned int(5)[3] language;
	string value;
}
*/
public partial class CollectionNameBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class CollectionNameBox() extends FullBox('coll', version = 0, 0) {
	bit(1) reserved = 0;
	unsigned int(5)[3] language;
	string value;
}"; } }
	public const string TYPE = "coll";
	public override string DisplayName { get { return "CollectionNameBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("udta"); }


	protected bool reserved = false; 
	public bool Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected string language = "und"; 
	public string Language { get { return this.language; } set { this.language = value; } }

	protected BinaryUTF8String value; 
	public BinaryUTF8String Value { get { return this.value; } set { this.value = value; } }

	public CollectionNameBox(): base(IsoStream.FromFourCC("coll"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadIso639(boxSize, readSize,  out this.language, "language"); 
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.value, "value"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBit( this.reserved, "reserved"); 
		boxSize += stream.WriteIso639( this.language, "language"); 
		boxSize += stream.WriteStringZeroTerminated( this.value, "value"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 1; // reserved
		boxSize += 15; // language
		boxSize += IsoStream.CalculateStringSize(value); // value
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("reserved", 1, this.reserved); 
		boxSize += tracer.Field("language", 15, this.language); 
		boxSize += tracer.Field("value", IsoStream.CalculateStringSize(value), this.value); 
		return boxSize;
	}
}

}
