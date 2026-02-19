using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SampleDescriptionBox ()
	extends FullBox('stsd', version, 0){
	int i ;
	unsigned int(32) entry_count;
	for (i = 1 ; i <= entry_count ; i++){
		SampleEntry();		// an instance of a class derived from SampleEntry
	}
}
*/
public partial class SampleDescriptionBox : FullBox
{
	public const string TYPE = "stsd";
	public override string DisplayName { get { return "SampleDescriptionBox"; } }

	protected uint entry_count; 
	public uint EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }
	public IEnumerable<SampleEntry> _SampleEntry { get { return this.children.OfType<SampleEntry>(); } }

	public SampleDescriptionBox(byte version = 0): base(IsoStream.FromFourCC("stsd"), version, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entry_count, "entry_count"); 

		for (int i = 0 ; i < entry_count ; i++)
		{
			// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.SampleEntry[i], "SampleEntry"); // an instance of a class derived from SampleEntry
		}
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		
		boxSize += stream.WriteUInt32( this.entry_count, "entry_count"); 

		for (int i = 0 ; i < entry_count ; i++)
		{
			// boxSize += stream.WriteBox( this.SampleEntry[i], "SampleEntry"); // an instance of a class derived from SampleEntry
		}
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		
		boxSize += 32; // entry_count

		for (int i = 0 ; i < entry_count ; i++)
		{
			// boxSize += IsoStream.CalculateBoxSize(SampleEntry); // SampleEntry
		}
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
