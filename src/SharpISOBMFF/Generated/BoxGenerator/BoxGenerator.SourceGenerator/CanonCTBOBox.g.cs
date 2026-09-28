using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// Canon.pm, CTBO: int32u entry count N, N x (int32u index, int64u offset, int64u size); index 1 XMP, 2 PRVW, 3 mdat
aligned(8) class CanonCTBOBox() extends Box('CTBO') {
 unsigned int(32) entryCount;
 CanonCTBOEntry entries[entryCount];
}

*/
public partial class CanonCTBOBox : Box
{
	public const string TYPE = "CTBO";
	public override string DisplayName { get { return "CanonCTBOBox"; } }

	protected uint entryCount; 
	public uint EntryCount { get { return this.entryCount; } set { this.entryCount = value; } }

	protected CanonCTBOEntry[] entries; 
	public CanonCTBOEntry[] Entries { get { return this.entries; } set { this.entries = value; } }

	public CanonCTBOBox(): base(IsoStream.FromFourCC("CTBO"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entryCount, "entryCount"); 
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(entryCount), () => new CanonCTBOEntry(),  out this.entries, "entries"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.entryCount, "entryCount"); 
		boxSize += stream.WriteClass( this.entries, "entries"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // entryCount
		boxSize += IsoStream.CalculateClassSize(entries); // entries
		return boxSize;
	}
}

}
