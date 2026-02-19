using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class HeroStereoEyeDescriptionBox extends FullBox('hero', 0, 0) 
{ 
unsigned int(8) hero_eye_indicator; // 0 = none, 1 = left, 2 = right, >= 3 reserved 
} 
*/
public partial class HeroStereoEyeDescriptionBox : FullBox
{
	public const string TYPE = "hero";
	public override string DisplayName { get { return "HeroStereoEyeDescriptionBox"; } }

	protected byte hero_eye_indicator;  //  0 = none, 1 = left, 2 = right, >= 3 reserved 
	public byte HeroEyeIndicator { get { return this.hero_eye_indicator; } set { this.hero_eye_indicator = value; } }

	public HeroStereoEyeDescriptionBox(): base(IsoStream.FromFourCC("hero"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.hero_eye_indicator, "hero_eye_indicator"); // 0 = none, 1 = left, 2 = right, >= 3 reserved 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.hero_eye_indicator, "hero_eye_indicator"); // 0 = none, 1 = left, 2 = right, >= 3 reserved 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // hero_eye_indicator
		return boxSize;
	}
}

}
