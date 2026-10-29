using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class MaskConfigurationProperty
	extends ItemFullProperty('mskC', version = 0, flags = 0){
	unsigned int(8) bits_per_pixel;
}
*/
public partial class MaskConfigurationProperty : ItemFullProperty
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class MaskConfigurationProperty
	extends ItemFullProperty('mskC', version = 0, flags = 0){
	unsigned int(8) bits_per_pixel;
}"; } }
	public const string TYPE = "mskC";
	public override string DisplayName { get { return "MaskConfigurationProperty"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("ipco"); }


	protected byte bits_per_pixel; 
	public byte BitsPerPixel { get { return this.bits_per_pixel; } set { this.bits_per_pixel = value; } }

	public MaskConfigurationProperty(): base(IsoStream.FromFourCC("mskC"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.bits_per_pixel, "bits_per_pixel"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.bits_per_pixel, "bits_per_pixel"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // bits_per_pixel
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("bits_per_pixel", 8, this.bits_per_pixel); 
		return boxSize;
	}
}

}
