using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class Ovc1VisualSampleEntryImpl() extends Box('ovc1') {
 unsigned int(48) reserved;
 unsigned int(16) dataReferenceIndex;
 bit(8) vc1Content[];
 } 
*/
public partial class Ovc1VisualSampleEntryImpl : Box
{
	public const string TYPE = "ovc1";
	public override string DisplayName { get { return "Ovc1VisualSampleEntryImpl"; } }

	protected ulong reserved; 
	public ulong Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected ushort dataReferenceIndex; 
	public ushort DataReferenceIndex { get { return this.dataReferenceIndex; } set { this.dataReferenceIndex = value; } }

	protected byte[] vc1Content; 
	public byte[] Vc1Content { get { return this.vc1Content; } set { this.vc1Content = value; } }

	public Ovc1VisualSampleEntryImpl(): base(IsoStream.FromFourCC("ovc1"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt48(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.dataReferenceIndex, "dataReferenceIndex"); 
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.vc1Content, "vc1Content"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt48( this.reserved, "reserved"); 
		boxSize += stream.WriteUInt16( this.dataReferenceIndex, "dataReferenceIndex"); 
		boxSize += stream.WriteUInt8ArrayTillEnd( this.vc1Content, "vc1Content"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 48; // reserved
		boxSize += 16; // dataReferenceIndex
		boxSize += ((ulong)vc1Content.Length * 8); // vc1Content
		return boxSize;
	}
}

}
