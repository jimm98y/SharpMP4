using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class PspMtdtBox() extends Box('MTDT') {
	 unsigned int(16) entry_count;
 MtdtEntry entries[ entry_count ];
 }
 
*/
public partial class PspMtdtBox : Box
{
	public const string TYPE = "MTDT";
	public override string DisplayName { get { return "PspMtdtBox"; } }

	protected ushort entry_count; 
	public ushort EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	protected MtdtEntry[] entries; 
	public MtdtEntry[] Entries { get { return this.entries; } set { this.entries = value; } }

	public PspMtdtBox(): base(IsoStream.FromFourCC("MTDT"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.entry_count, "entry_count"); 
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)( entry_count ), () => new MtdtEntry(),  out this.entries, "entries"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.entry_count, "entry_count"); 
		boxSize += stream.WriteClass( this.entries, "entries"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // entry_count
		boxSize += IsoStream.CalculateClassSize(entries); // entries
		return boxSize;
	}
}

}
