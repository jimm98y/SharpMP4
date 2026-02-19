using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class HrefBox() extends Box('href') {
	 unsigned int(8) reserved1[6];
 unsigned int(16) dataReferenceIndex;
 Box boxes[];
 } 
*/
public partial class HrefBox : Box
{
	public const string TYPE = "href";
	public override string DisplayName { get { return "HrefBox"; } }

	protected byte[] reserved1; 
	public byte[] Reserved1 { get { return this.reserved1; } set { this.reserved1 = value; } }

	protected ushort dataReferenceIndex; 
	public ushort DataReferenceIndex { get { return this.dataReferenceIndex; } set { this.dataReferenceIndex = value; } }
	public IEnumerable<Box> Boxes { get { return this.children.OfType<Box>(); } }

	public HrefBox(): base(IsoStream.FromFourCC("href"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 6,  out this.reserved1, "reserved1"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.dataReferenceIndex, "dataReferenceIndex"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.boxes, "boxes"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8Array(6,  this.reserved1, "reserved1"); 
		boxSize += stream.WriteUInt16( this.dataReferenceIndex, "dataReferenceIndex"); 
		// boxSize += stream.WriteBox( this.boxes, "boxes"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 6 * 8; // reserved1
		boxSize += 16; // dataReferenceIndex
		// boxSize += IsoStream.CalculateBoxSize(boxes); // boxes
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
