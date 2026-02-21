using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SampleScaleBox() extends FullBox('stsl', version = 0, 0) {
	bit(7)	reserved;
	bit(1) constrained; unsigned int(8) scaleMethod; signed int(16) displayCenterX; signed int(16) displayCenterY;
	}

*/
public partial class SampleScaleBox : FullBox
{
	public const string TYPE = "stsl";
	public override string DisplayName { get { return "SampleScaleBox"; } }

	protected byte reserved; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected bool constrained; 
	public bool Constrained { get { return this.constrained; } set { this.constrained = value; } }

	protected byte scaleMethod; 
	public byte ScaleMethod { get { return this.scaleMethod; } set { this.scaleMethod = value; } }

	protected short displayCenterX; 
	public short DisplayCenterX { get { return this.displayCenterX; } set { this.displayCenterX = value; } }

	protected short displayCenterY; 
	public short DisplayCenterY { get { return this.displayCenterY; } set { this.displayCenterY = value; } }

	public SampleScaleBox(): base(IsoStream.FromFourCC("stsl"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.constrained, "constrained"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.scaleMethod, "scaleMethod"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.displayCenterX, "displayCenterX"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.displayCenterY, "displayCenterY"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(7,  this.reserved, "reserved"); 
		boxSize += stream.WriteBit( this.constrained, "constrained"); 
		boxSize += stream.WriteUInt8( this.scaleMethod, "scaleMethod"); 
		boxSize += stream.WriteInt16( this.displayCenterX, "displayCenterX"); 
		boxSize += stream.WriteInt16( this.displayCenterY, "displayCenterY"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 7; // reserved
		boxSize += 1; // constrained
		boxSize += 8; // scaleMethod
		boxSize += 16; // displayCenterX
		boxSize += 16; // displayCenterY
		return boxSize;
	}
}

}
