using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ImageScaling
extends ItemFullProperty('iscl', version = 0, flags = 0) {
	unsigned int(16) target_width_numerator;
	unsigned int(16) target_width_denominator;
	unsigned int(16) target_height_numerator;
	unsigned int(16) target_height_denominator;
}
*/
public partial class ImageScaling : ItemFullProperty
{
	public const string TYPE = "iscl";
	public override string DisplayName { get { return "ImageScaling"; } }

	protected ushort target_width_numerator; 
	public ushort TargetWidthNumerator { get { return this.target_width_numerator; } set { this.target_width_numerator = value; } }

	protected ushort target_width_denominator; 
	public ushort TargetWidthDenominator { get { return this.target_width_denominator; } set { this.target_width_denominator = value; } }

	protected ushort target_height_numerator; 
	public ushort TargetHeightNumerator { get { return this.target_height_numerator; } set { this.target_height_numerator = value; } }

	protected ushort target_height_denominator; 
	public ushort TargetHeightDenominator { get { return this.target_height_denominator; } set { this.target_height_denominator = value; } }

	public ImageScaling(): base(IsoStream.FromFourCC("iscl"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.target_width_numerator, "target_width_numerator"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.target_width_denominator, "target_width_denominator"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.target_height_numerator, "target_height_numerator"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.target_height_denominator, "target_height_denominator"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.target_width_numerator, "target_width_numerator"); 
		boxSize += stream.WriteUInt16( this.target_width_denominator, "target_width_denominator"); 
		boxSize += stream.WriteUInt16( this.target_height_numerator, "target_height_numerator"); 
		boxSize += stream.WriteUInt16( this.target_height_denominator, "target_height_denominator"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // target_width_numerator
		boxSize += 16; // target_width_denominator
		boxSize += 16; // target_height_numerator
		boxSize += 16; // target_height_denominator
		return boxSize;
	}
}

}
