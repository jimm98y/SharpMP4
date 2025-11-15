using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class AppleStartTimeScale() extends Box('©TSC') {
 MultiLanguageString value[]; 
 } 
*/
public partial class AppleStartTimeScale : Box
{
	public const string TYPE = "©TSC";
	public override string DisplayName { get { return "AppleStartTimeScale"; } }

	protected MultiLanguageString[] value; 
	public MultiLanguageString[] Value { get { return this.value; } set { this.value = value; } }

	public AppleStartTimeScale(): base(IsoStream.FromFourCC("©TSC"))
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
}

}
