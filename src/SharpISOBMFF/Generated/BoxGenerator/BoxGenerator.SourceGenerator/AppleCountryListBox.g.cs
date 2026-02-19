using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class AppleCountryListBox() extends Box ('ctry') {
 unsigned int(16) entry_count;
 CountryListEntry entries[];
 }
 
*/
public partial class AppleCountryListBox : Box
{
	public const string TYPE = "ctry";
	public override string DisplayName { get { return "AppleCountryListBox"; } }

	protected ushort entry_count; 
	public ushort EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	protected CountryListEntry[] entries; 
	public CountryListEntry[] Entries { get { return this.entries; } set { this.entries = value; } }

	public AppleCountryListBox(): base(IsoStream.FromFourCC("ctry"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.entry_count, "entry_count"); 
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(uint.MaxValue), () => new CountryListEntry(),  out this.entries, "entries"); 
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
