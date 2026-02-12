using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class KindBox extends FullBox('kind', version = 0, 0) {
	utf8string schemeURI;
	utf8string value;
}
*/
public partial class KindBox : FullBox
{
	public const string TYPE = "kind";
	public override string DisplayName { get { return "KindBox"; } }

	protected BinaryUTF8String schemeURI; 
	public BinaryUTF8String SchemeURI { get { return this.schemeURI; } set { this.schemeURI = value; } }

	protected BinaryUTF8String value; 
	public BinaryUTF8String Value { get { return this.value; } set { this.value = value; } }

	public KindBox(): base(IsoStream.FromFourCC("kind"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.schemeURI, "schemeURI"); 
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.value, "value"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.schemeURI, "schemeURI"); 
		boxSize += stream.WriteStringZeroTerminated( this.value, "value"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(schemeURI); // schemeURI
		boxSize += IsoStream.CalculateStringSize(value); // value
		return boxSize;
	}
}

}
