using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class DataReferenceBox
	extends FullBox('dref', version = 0, 0) {
	unsigned int(32)	entry_count;
	for (i=1; i <= entry_count; i++) {
		DataEntryBaseBox(entry_type, entry_flags)	data_entry;
	}
}
*/
public partial class DataReferenceBox : FullBox
{
	public const string TYPE = "dref";
	public override string DisplayName { get { return "DataReferenceBox"; } }

	protected uint entry_count; 
	public uint EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }
	public IEnumerable<DataEntryBaseBox> DataEntry { get { return this.children.OfType<DataEntryBaseBox>(); } }

	public DataReferenceBox(): base(IsoStream.FromFourCC("dref"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entry_count, "entry_count"); 

		for (int i=0; i < entry_count; i++)
		{
			// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.data_entry[i], "data_entry"); 
		}
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.entry_count, "entry_count"); 

		for (int i=0; i < entry_count; i++)
		{
			// boxSize += stream.WriteBox( this.data_entry[i], "data_entry"); 
		}
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // entry_count

		for (int i=0; i < entry_count; i++)
		{
			// boxSize += IsoStream.CalculateBoxSize(data_entry); // data_entry
		}
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
