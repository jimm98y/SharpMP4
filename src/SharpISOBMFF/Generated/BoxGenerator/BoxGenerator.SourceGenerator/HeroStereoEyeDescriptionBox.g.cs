using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

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
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class HeroStereoEyeDescriptionBox extends FullBox('hero', 0, 0) 
{ 
unsigned int(8) hero_eye_indicator; // 0 = none, 1 = left, 2 = right, >= 3 reserved 
} "; } }
	public const string TYPE = "hero";
	public override string DisplayName { get { return "HeroStereoEyeDescriptionBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("eyes"); }


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

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("hero_eye_indicator", 8, this.hero_eye_indicator); // 0 = none, 1 = left, 2 = right, >= 3 reserved 
		return boxSize;
	}
}

}
