using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class AutoExposureBracketingEntry
extends VisualSampleGroupEntry('aebr') {
	int(8) exposure_step;
	int(8) exposure_numerator;
}
*/
public partial class AutoExposureBracketingEntry : VisualSampleGroupEntry
{
	public const string TYPE = "aebr";
	public override string DisplayName { get { return "AutoExposureBracketingEntry"; } }

	protected sbyte exposure_step; 
	public sbyte ExposureStep { get { return this.exposure_step; } set { this.exposure_step = value; } }

	protected sbyte exposure_numerator; 
	public sbyte ExposureNumerator { get { return this.exposure_numerator; } set { this.exposure_numerator = value; } }

	public AutoExposureBracketingEntry(): base(IsoStream.FromFourCC("aebr"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt8(boxSize, readSize,  out this.exposure_step, "exposure_step"); 
		boxSize += stream.ReadInt8(boxSize, readSize,  out this.exposure_numerator, "exposure_numerator"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt8( this.exposure_step, "exposure_step"); 
		boxSize += stream.WriteInt8( this.exposure_numerator, "exposure_numerator"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // exposure_step
		boxSize += 8; // exposure_numerator
		return boxSize;
	}
}

}
