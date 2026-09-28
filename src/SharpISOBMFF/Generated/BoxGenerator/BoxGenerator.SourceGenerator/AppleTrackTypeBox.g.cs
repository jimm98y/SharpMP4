using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class AppleTrackTypeBox() extends Box ('kgtt'){
 MultiLanguageString value[];
 }
*/
public partial class AppleTrackTypeBox : Box
{
	public const string TYPE = "kgtt";
	public override string DisplayName { get { return "AppleTrackTypeBox"; } }

	protected MultiLanguageString[] value; 
	public MultiLanguageString[] Value { get { return this.value; } set { this.value = value; } }

	public AppleTrackTypeBox(): base(IsoStream.FromFourCC("kgtt"))
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
