using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class RicohAppleGPSCoordinatesBox() extends Box('@xyz') {
 MultiLanguageString value[]; 
 } 
*/
public partial class RicohAppleGPSCoordinatesBox : Box
{
	public const string TYPE = "@xyz";
	public override string DisplayName { get { return "RicohAppleGPSCoordinatesBox"; } }

	protected MultiLanguageString[] value; 
	public MultiLanguageString[] Value { get { return this.value; } set { this.value = value; } }

	public RicohAppleGPSCoordinatesBox(): base(IsoStream.FromFourCC("@xyz"))
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
