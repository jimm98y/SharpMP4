using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class SegmentIndexBox extends FullBox('sidx', version, 0) { 
  unsigned int(32) reference_ID; 
  unsigned int(32) timescale; 
  if (version==0) { 
   unsigned int(32) earliest_presentation_time; 
   unsigned int(32) first_offset; 
  }
  else { 
   unsigned int(64) earliest_presentation_time; 
   unsigned int(64) first_offset; 
  }
 unsigned int(16) reserved = 0; 
 unsigned int(16) reference_count; 
 for(i=1; i <= reference_count; i++) 
 {  
  bit(1)   reference_type; 
  unsigned int(31) referenced_size; 
  unsigned int(32) subsegment_duration; 
  bit(1)    starts_with_SAP; 
  unsigned int(3) SAP_type; 
  unsigned int(28) SAP_delta_time;
 } 
} 
*/
public partial class SegmentIndexBox : FullBox
{
	public const string TYPE = "sidx";
	public override string DisplayName { get { return "SegmentIndexBox"; } }

	protected uint reference_ID; 
	public uint ReferenceID { get { return this.reference_ID; } set { this.reference_ID = value; } }

	protected uint timescale; 
	public uint Timescale { get { return this.timescale; } set { this.timescale = value; } }

	protected ulong earliest_presentation_time; 
	public ulong EarliestPresentationTime { get { return this.earliest_presentation_time; } set { this.earliest_presentation_time = value; } }

	protected ulong first_offset; 
	public ulong FirstOffset { get { return this.first_offset; } set { this.first_offset = value; } }

	protected ushort reserved = 0; 
	public ushort Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected ushort reference_count; 
	public ushort ReferenceCount { get { return this.reference_count; } set { this.reference_count = value; } }

	protected bool[] reference_type; 
	public bool[] ReferenceType { get { return this.reference_type; } set { this.reference_type = value; } }

	protected uint[] referenced_size; 
	public uint[] ReferencedSize { get { return this.referenced_size; } set { this.referenced_size = value; } }

	protected uint[] subsegment_duration; 
	public uint[] SubsegmentDuration { get { return this.subsegment_duration; } set { this.subsegment_duration = value; } }

	protected bool[] starts_with_SAP; 
	public bool[] StartsWithSAP { get { return this.starts_with_SAP; } set { this.starts_with_SAP = value; } }

	protected byte[] SAP_type; 
	public byte[] SAPType { get { return this.SAP_type; } set { this.SAP_type = value; } }

	protected uint[] SAP_delta_time; 
	public uint[] SAPDeltaTime { get { return this.SAP_delta_time; } set { this.SAP_delta_time = value; } }

	public SegmentIndexBox(byte version = 0): base(IsoStream.FromFourCC("sidx"), version, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reference_ID, "reference_ID"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.timescale, "timescale"); 

		if (version==0)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.earliest_presentation_time, "earliest_presentation_time"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.first_offset, "first_offset"); 
		}

		else 
		{
			boxSize += stream.ReadUInt64(boxSize, readSize,  out this.earliest_presentation_time, "earliest_presentation_time"); 
			boxSize += stream.ReadUInt64(boxSize, readSize,  out this.first_offset, "first_offset"); 
		}
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.reference_count, "reference_count"); 

		this.reference_type = new bool[IsoStream.GetInt( reference_count)];
		this.referenced_size = new uint[IsoStream.GetInt( reference_count)];
		this.subsegment_duration = new uint[IsoStream.GetInt( reference_count)];
		this.starts_with_SAP = new bool[IsoStream.GetInt( reference_count)];
		this.SAP_type = new byte[IsoStream.GetInt( reference_count)];
		this.SAP_delta_time = new uint[IsoStream.GetInt( reference_count)];
		for (int i=0; i < reference_count; i++)
		{
			boxSize += stream.ReadBit(boxSize, readSize,  out this.reference_type[i], "reference_type"); 
			boxSize += stream.ReadBits(boxSize, readSize, 31,  out this.referenced_size[i], "referenced_size"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.subsegment_duration[i], "subsegment_duration"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.starts_with_SAP[i], "starts_with_SAP"); 
			boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.SAP_type[i], "SAP_type"); 
			boxSize += stream.ReadBits(boxSize, readSize, 28,  out this.SAP_delta_time[i], "SAP_delta_time"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.reference_ID, "reference_ID"); 
		boxSize += stream.WriteUInt32( this.timescale, "timescale"); 

		if (version==0)
		{
			boxSize += stream.WriteUInt32( this.earliest_presentation_time, "earliest_presentation_time"); 
			boxSize += stream.WriteUInt32( this.first_offset, "first_offset"); 
		}

		else 
		{
			boxSize += stream.WriteUInt64( this.earliest_presentation_time, "earliest_presentation_time"); 
			boxSize += stream.WriteUInt64( this.first_offset, "first_offset"); 
		}
		boxSize += stream.WriteUInt16( this.reserved, "reserved"); 
		boxSize += stream.WriteUInt16( this.reference_count, "reference_count"); 

		for (int i=0; i < reference_count; i++)
		{
			boxSize += stream.WriteBit( this.reference_type[i], "reference_type"); 
			boxSize += stream.WriteBits(31,  this.referenced_size[i], "referenced_size"); 
			boxSize += stream.WriteUInt32( this.subsegment_duration[i], "subsegment_duration"); 
			boxSize += stream.WriteBit( this.starts_with_SAP[i], "starts_with_SAP"); 
			boxSize += stream.WriteBits(3,  this.SAP_type[i], "SAP_type"); 
			boxSize += stream.WriteBits(28,  this.SAP_delta_time[i], "SAP_delta_time"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // reference_ID
		boxSize += 32; // timescale

		if (version==0)
		{
			boxSize += 32; // earliest_presentation_time
			boxSize += 32; // first_offset
		}

		else 
		{
			boxSize += 64; // earliest_presentation_time
			boxSize += 64; // first_offset
		}
		boxSize += 16; // reserved
		boxSize += 16; // reference_count

		for (int i=0; i < reference_count; i++)
		{
			boxSize += 1; // reference_type
			boxSize += 31; // referenced_size
			boxSize += 32; // subsegment_duration
			boxSize += 1; // starts_with_SAP
			boxSize += 3; // SAP_type
			boxSize += 28; // SAP_delta_time
		}
		return boxSize;
	}
}

}
