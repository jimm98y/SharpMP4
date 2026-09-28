using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// A CR3's raw image: 4 bytes, then its IAD1
aligned(8) class CanonCDI1Box() extends Box('CDI1') {
 unsigned int(32) unknown;
 Box boxes[];
}

*/
public partial class CanonCDI1Box : Box
{
	public const string TYPE = "CDI1";
	public override string DisplayName { get { return "CanonCDI1Box"; } }

	protected uint unknown; 
	public uint Unknown { get { return this.unknown; } set { this.unknown = value; } }

	public CanonCDI1Box(): base(IsoStream.FromFourCC("CDI1"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.unknown, "unknown"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.boxes, "boxes"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.unknown, "unknown"); 
		// boxSize += stream.WriteBox( this.boxes, "boxes"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // unknown
		// boxSize += IsoStream.CalculateBoxSize(boxes); // boxes
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
