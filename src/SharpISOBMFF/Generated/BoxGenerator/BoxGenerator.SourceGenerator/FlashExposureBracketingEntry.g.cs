using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class FlashExposureBracketingEntry
extends VisualSampleGroupEntry('afbr') {
	int(8) flash_exposure_numerator;
	int(8) flash_exposure_denominator;
}
*/
public partial class FlashExposureBracketingEntry : VisualSampleGroupEntry
{
	public const string TYPE = "afbr";
	public override string DisplayName { get { return "FlashExposureBracketingEntry"; } }

	protected sbyte flash_exposure_numerator; 
	public sbyte FlashExposureNumerator { get { return this.flash_exposure_numerator; } set { this.flash_exposure_numerator = value; } }

	protected sbyte flash_exposure_denominator; 
	public sbyte FlashExposureDenominator { get { return this.flash_exposure_denominator; } set { this.flash_exposure_denominator = value; } }

	public FlashExposureBracketingEntry(): base(IsoStream.FromFourCC("afbr"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt8(boxSize, readSize,  out this.flash_exposure_numerator, "flash_exposure_numerator"); 
		boxSize += stream.ReadInt8(boxSize, readSize,  out this.flash_exposure_denominator, "flash_exposure_denominator"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt8( this.flash_exposure_numerator, "flash_exposure_numerator"); 
		boxSize += stream.WriteInt8( this.flash_exposure_denominator, "flash_exposure_denominator"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // flash_exposure_numerator
		boxSize += 8; // flash_exposure_denominator
		return boxSize;
	}
}

}
