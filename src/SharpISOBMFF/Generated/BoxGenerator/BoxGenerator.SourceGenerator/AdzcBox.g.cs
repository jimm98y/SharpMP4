using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class AdzcBox() extends Box('adzc') {
 MultiLanguageString value[];
}

*/
public partial class AdzcBox : Box
{
	public const string TYPE = "adzc";
	public override string DisplayName { get { return "AdzcBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("udta"); }


	protected MultiLanguageString[] value; 
	public MultiLanguageString[] Value { get { return this.value; } set { this.value = value; } }

	public AdzcBox(): base(IsoStream.FromFourCC("adzc"))
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

	public override ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += base.ToString(builder);
		boxSize += builder.Field("value", IsoStream.CalculateStringSizeLangPrefixed(value), this.value); 
		return boxSize;
	}
}

}
