using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ImageMirror
extends ItemProperty('imir') {
	unsigned int(7) reserved = 0;
	unsigned int(1) axis;
}
*/
public partial class ImageMirror : ItemProperty
{
	public const string TYPE = "imir";
	public override string DisplayName { get { return "ImageMirror"; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected bool axis; 
	public bool Axis { get { return this.axis; } set { this.axis = value; } }

	public ImageMirror(): base(IsoStream.FromFourCC("imir"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.axis, "axis"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(7,  this.reserved, "reserved"); 
		boxSize += stream.WriteBit( this.axis, "axis"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 7; // reserved
		boxSize += 1; // axis
		return boxSize;
	}
}

}
