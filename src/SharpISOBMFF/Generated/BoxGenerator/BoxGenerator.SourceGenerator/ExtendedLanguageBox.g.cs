using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ExtendedLanguageBox extends FullBox('elng', 0, 0) {
	utf8string	extended_language;
}
*/
public partial class ExtendedLanguageBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class ExtendedLanguageBox extends FullBox('elng', 0, 0) {
	utf8string	extended_language;
}"; } }
	public const string TYPE = "elng";
	public override string DisplayName { get { return "ExtendedLanguageBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("mdia"); }


	protected BinaryUTF8String extended_language; 
	public BinaryUTF8String ExtendedLanguage { get { return this.extended_language; } set { this.extended_language = value; } }

	public ExtendedLanguageBox(): base(IsoStream.FromFourCC("elng"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.extended_language, "extended_language"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.extended_language, "extended_language"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(extended_language); // extended_language
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("extended_language", IsoStream.CalculateStringSize(extended_language), this.extended_language); 
		return boxSize;
	}
}

}
