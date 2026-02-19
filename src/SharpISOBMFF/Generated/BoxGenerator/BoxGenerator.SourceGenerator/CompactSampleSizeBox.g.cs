using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class CompactSampleSizeBox 
		extends FullBox('stz2', version = 0, 0) {
	unsigned int(24)	reserved = 0;
	unsigned int(8)	field_size;
	unsigned int(32)	sample_count;
	for (i=1; i <= sample_count; i++) {
		unsigned int(field_size)	entry_size;
	}
}
*/
public partial class CompactSampleSizeBox : FullBox
{
	public const string TYPE = "stz2";
	public override string DisplayName { get { return "CompactSampleSizeBox"; } }

	protected uint reserved = 0; 
	public uint Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte field_size; 
	public byte FieldSize { get { return this.field_size; } set { this.field_size = value; } }

	protected uint sample_count; 
	public uint SampleCount { get { return this.sample_count; } set { this.sample_count = value; } }

	protected byte[][] entry_size; 
	public byte[][] EntrySize { get { return this.entry_size; } set { this.entry_size = value; } }

	public CompactSampleSizeBox(): base(IsoStream.FromFourCC("stz2"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt24(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.field_size, "field_size"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_count, "sample_count"); 

		this.entry_size = new byte[IsoStream.GetInt( sample_count)][];
		for (int i=0; i < sample_count; i++)
		{
			boxSize += stream.ReadBits(boxSize, readSize, (uint)(field_size ),  out this.entry_size[i], "entry_size"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt24( this.reserved, "reserved"); 
		boxSize += stream.WriteUInt8( this.field_size, "field_size"); 
		boxSize += stream.WriteUInt32( this.sample_count, "sample_count"); 

		for (int i=0; i < sample_count; i++)
		{
			boxSize += stream.WriteBits((uint)(field_size ),  this.entry_size[i], "entry_size"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 24; // reserved
		boxSize += 8; // field_size
		boxSize += 32; // sample_count

		for (int i=0; i < sample_count; i++)
		{
			boxSize += (ulong)(field_size ); // entry_size
		}
		return boxSize;
	}
}

}
