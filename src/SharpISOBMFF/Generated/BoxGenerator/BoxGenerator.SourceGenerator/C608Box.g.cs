using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class C608Box() extends Box('c608') {
	 unsigned int(8) reserved1[4]; unsigned int(16) reserved2;
 unsigned int(16) dataReferenceIndex;
 } 
*/
public partial class C608Box : Box
{
	public const string TYPE = "c608";
	public override string DisplayName { get { return "C608Box"; } }

	protected byte[] reserved1; 
	public byte[] Reserved1 { get { return this.reserved1; } set { this.reserved1 = value; } }

	protected ushort reserved2; 
	public ushort Reserved2 { get { return this.reserved2; } set { this.reserved2 = value; } }

	protected ushort dataReferenceIndex; 
	public ushort DataReferenceIndex { get { return this.dataReferenceIndex; } set { this.dataReferenceIndex = value; } }

	public C608Box(): base(IsoStream.FromFourCC("c608"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 4,  out this.reserved1, "reserved1"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.reserved2, "reserved2"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.dataReferenceIndex, "dataReferenceIndex"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8Array(4,  this.reserved1, "reserved1"); 
		boxSize += stream.WriteUInt16( this.reserved2, "reserved2"); 
		boxSize += stream.WriteUInt16( this.dataReferenceIndex, "dataReferenceIndex"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 4 * 8; // reserved1
		boxSize += 16; // reserved2
		boxSize += 16; // dataReferenceIndex
		return boxSize;
	}
}

}
