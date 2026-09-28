using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// Canon.pm, CCTP: 12 bytes, then a CCDT for each image track
aligned(8) class CanonCCTPBox() extends Box('CCTP') {
 unsigned int(32) reserved;
 unsigned int(32) unknown;
 unsigned int(32) entryCount;
 Box boxes[];
}

*/
public partial class CanonCCTPBox : Box
{
	public const string TYPE = "CCTP";
	public override string DisplayName { get { return "CanonCCTPBox"; } }

	protected uint reserved; 
	public uint Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected uint unknown; 
	public uint Unknown { get { return this.unknown; } set { this.unknown = value; } }

	protected uint entryCount; 
	public uint EntryCount { get { return this.entryCount; } set { this.entryCount = value; } }

	public CanonCCTPBox(): base(IsoStream.FromFourCC("CCTP"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.unknown, "unknown"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entryCount, "entryCount"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.boxes, "boxes"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.reserved, "reserved"); 
		boxSize += stream.WriteUInt32( this.unknown, "unknown"); 
		boxSize += stream.WriteUInt32( this.entryCount, "entryCount"); 
		// boxSize += stream.WriteBox( this.boxes, "boxes"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // reserved
		boxSize += 32; // unknown
		boxSize += 32; // entryCount
		// boxSize += IsoStream.CalculateBoxSize(boxes); // boxes
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
