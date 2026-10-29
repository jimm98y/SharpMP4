using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class AccessibilityTextProperty
	extends ItemFullProperty('altt', version = 0, flags = 0) {
	utf8string alt_text;
	utf8string alt_lang;
}
*/
public partial class AccessibilityTextProperty : ItemFullProperty
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class AccessibilityTextProperty
	extends ItemFullProperty('altt', version = 0, flags = 0) {
	utf8string alt_text;
	utf8string alt_lang;
}"; } }
	public const string TYPE = "altt";
	public override string DisplayName { get { return "AccessibilityTextProperty"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("ipco"); }


	protected BinaryUTF8String alt_text; 
	public BinaryUTF8String AltText { get { return this.alt_text; } set { this.alt_text = value; } }

	protected BinaryUTF8String alt_lang; 
	public BinaryUTF8String AltLang { get { return this.alt_lang; } set { this.alt_lang = value; } }

	public AccessibilityTextProperty(): base(IsoStream.FromFourCC("altt"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.alt_text, "alt_text"); 
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.alt_lang, "alt_lang"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.alt_text, "alt_text"); 
		boxSize += stream.WriteStringZeroTerminated( this.alt_lang, "alt_lang"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(alt_text); // alt_text
		boxSize += IsoStream.CalculateStringSize(alt_lang); // alt_lang
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("alt_text", IsoStream.CalculateStringSize(alt_text), this.alt_text); 
		boxSize += tracer.Field("alt_lang", IsoStream.CalculateStringSize(alt_lang), this.alt_lang); 
		return boxSize;
	}
}

}
