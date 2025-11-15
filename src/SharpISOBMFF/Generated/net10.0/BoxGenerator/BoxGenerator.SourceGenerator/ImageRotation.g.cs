using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class ImageRotation
extends ItemProperty('irot') {
	unsigned int(6) reserved = 0;
	unsigned int(2) angle;
}
*/
public partial class ImageRotation : ItemProperty
{
	public const string TYPE = "irot";
	public override string DisplayName { get { return "ImageRotation"; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte angle; 
	public byte Angle { get { return this.angle; } set { this.angle = value; } }

	public ImageRotation(): base(IsoStream.FromFourCC("irot"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.angle, "angle"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(6,  this.reserved, "reserved"); 
		boxSize += stream.WriteBits(2,  this.angle, "angle"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 6; // reserved
		boxSize += 2; // angle
		return boxSize;
	}
}

}
