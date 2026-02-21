using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class NALUMapEntry() extends VisualSampleGroupEntry ('nalm') {
	bit(6) reserved = 0;
	unsigned int(1) large_size;
	unsigned int(1) rle;
	if (large_size) {
		unsigned int(16) entry_count;
	} else {
		unsigned int(8) entry_count;
	}
	for (i=1; i<= entry_count; i++) {
		if (rle) {
			if (large_size) {
				unsigned int(16) NALU_start_number;
			} else {
				unsigned int(8) NALU_start_number;
			}
		}
		unsigned int(16) groupID;
	}
}
*/
public partial class NALUMapEntry : VisualSampleGroupEntry
{
	public const string TYPE = "nalm";
	public override string DisplayName { get { return "NALUMapEntry"; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected bool large_size; 
	public bool LargeSize { get { return this.large_size; } set { this.large_size = value; } }

	protected bool rle; 
	public bool Rle { get { return this.rle; } set { this.rle = value; } }

	protected ushort entry_count; 
	public ushort EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	protected ushort[] NALU_start_number; 
	public ushort[] NALUStartNumber { get { return this.NALU_start_number; } set { this.NALU_start_number = value; } }

	protected ushort[] groupID; 
	public ushort[] GroupID { get { return this.groupID; } set { this.groupID = value; } }

	public NALUMapEntry(): base(IsoStream.FromFourCC("nalm"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.large_size, "large_size"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.rle, "rle"); 

		if (large_size)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.entry_count, "entry_count"); 
		}

		else 
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.entry_count, "entry_count"); 
		}

		this.NALU_start_number = new ushort[IsoStream.GetInt( entry_count)];
		this.groupID = new ushort[IsoStream.GetInt( entry_count)];
		for (int i=0; i< entry_count; i++)
		{

			if (rle)
			{

				if (large_size)
				{
					boxSize += stream.ReadUInt16(boxSize, readSize,  out this.NALU_start_number[i], "NALU_start_number"); 
				}

				else 
				{
					boxSize += stream.ReadUInt8(boxSize, readSize,  out this.NALU_start_number[i], "NALU_start_number"); 
				}
			}
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.groupID[i], "groupID"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(6,  this.reserved, "reserved"); 
		boxSize += stream.WriteBit( this.large_size, "large_size"); 
		boxSize += stream.WriteBit( this.rle, "rle"); 

		if (large_size)
		{
			boxSize += stream.WriteUInt16( this.entry_count, "entry_count"); 
		}

		else 
		{
			boxSize += stream.WriteUInt8( this.entry_count, "entry_count"); 
		}

		for (int i=0; i< entry_count; i++)
		{

			if (rle)
			{

				if (large_size)
				{
					boxSize += stream.WriteUInt16( this.NALU_start_number[i], "NALU_start_number"); 
				}

				else 
				{
					boxSize += stream.WriteUInt8( this.NALU_start_number[i], "NALU_start_number"); 
				}
			}
			boxSize += stream.WriteUInt16( this.groupID[i], "groupID"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 6; // reserved
		boxSize += 1; // large_size
		boxSize += 1; // rle

		if (large_size)
		{
			boxSize += 16; // entry_count
		}

		else 
		{
			boxSize += 8; // entry_count
		}

		for (int i=0; i< entry_count; i++)
		{

			if (rle)
			{

				if (large_size)
				{
					boxSize += 16; // NALU_start_number
				}

				else 
				{
					boxSize += 8; // NALU_start_number
				}
			}
			boxSize += 16; // groupID
		}
		return boxSize;
	}
}

}
