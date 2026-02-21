using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class MetaDataLocaleBox extends Box('loca') {
 string locale_string;
}


*/
public partial class MetaDataLocaleBox : Box
{
	public const string TYPE = "loca";
	public override string DisplayName { get { return "MetaDataLocaleBox"; } }

	protected BinaryUTF8String locale_string; 
	public BinaryUTF8String LocaleString { get { return this.locale_string; } set { this.locale_string = value; } }

	public MetaDataLocaleBox(): base(IsoStream.FromFourCC("loca"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.locale_string, "locale_string"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.locale_string, "locale_string"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(locale_string); // locale_string
		return boxSize;
	}
}

}
