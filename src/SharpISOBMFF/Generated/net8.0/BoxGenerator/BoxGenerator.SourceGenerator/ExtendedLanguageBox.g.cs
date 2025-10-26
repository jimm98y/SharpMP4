using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class ExtendedLanguageBox extends FullBox('elng', 0, 0) {
	utf8string	extended_language;
}
*/
public partial class ExtendedLanguageBox : FullBox
{
	public const string TYPE = "elng";
	public override string DisplayName { get { return "ExtendedLanguageBox"; } }

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
}

}
