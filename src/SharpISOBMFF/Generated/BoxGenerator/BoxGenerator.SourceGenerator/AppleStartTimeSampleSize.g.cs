using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class AppleStartTimeSampleSize() extends Box('©TSZ') {
	MultiLanguageString value[];
}
*/
public partial class AppleStartTimeSampleSize : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class AppleStartTimeSampleSize() extends Box('©TSZ') {
	MultiLanguageString value[];
}"; } }
	public const string TYPE = "©TSZ";
	public override string DisplayName { get { return "AppleStartTimeSampleSize"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("udta", "ilst"); }


	protected MultiLanguageString[] value = []; 
	public MultiLanguageString[] Value { get { return this.value; } set { this.value = value; } }

	public AppleStartTimeSampleSize(): base(IsoStream.FromFourCC("©TSZ"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringSizeLangPrefixed(boxSize, readSize,  out this.value, "value"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringSizeLangPrefixed( this.value, "value"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSizeLangPrefixed(value); // value
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("value", IsoStream.CalculateStringSizeLangPrefixed(value), this.value); 
		return boxSize;
	}
}

}
