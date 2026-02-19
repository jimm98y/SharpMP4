using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class TrackFragmentRandomAccessBox
 extends FullBox('tfra', version, 0) {
	unsigned int(32)	track_ID;
	const unsigned int(26)	reserved = 0;
	unsigned int(2)	length_size_of_traf_num;
	unsigned int(2)	length_size_of_trun_num;
	unsigned int(2)	length_size_of_sample_num;
	unsigned int(32)	number_of_entry;
	for(i=1; i <= number_of_entry; i++){
		if(version==1){
			unsigned int(64)	time;
			unsigned int(64)	moof_offset;
		}else{
			unsigned int(32)	time;
			unsigned int(32)	moof_offset;
		}
		unsigned int((length_size_of_traf_num+1) * 8)	traf_number;
		unsigned int((length_size_of_trun_num+1) * 8)	trun_number;
		unsigned int((length_size_of_sample_num+1) * 8)	sample_delta;
	}
}
*/
public partial class TrackFragmentRandomAccessBox : FullBox
{
	public const string TYPE = "tfra";
	public override string DisplayName { get { return "TrackFragmentRandomAccessBox"; } }

	protected uint track_ID; 
	public uint TrackID { get { return this.track_ID; } set { this.track_ID = value; } }

	protected uint reserved = 0; 
	public uint Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte length_size_of_traf_num; 
	public byte LengthSizeOfTrafNum { get { return this.length_size_of_traf_num; } set { this.length_size_of_traf_num = value; } }

	protected byte length_size_of_trun_num; 
	public byte LengthSizeOfTrunNum { get { return this.length_size_of_trun_num; } set { this.length_size_of_trun_num = value; } }

	protected byte length_size_of_sample_num; 
	public byte LengthSizeOfSampleNum { get { return this.length_size_of_sample_num; } set { this.length_size_of_sample_num = value; } }

	protected uint number_of_entry; 
	public uint NumberOfEntry { get { return this.number_of_entry; } set { this.number_of_entry = value; } }

	protected ulong[] time; 
	public ulong[] Time { get { return this.time; } set { this.time = value; } }

	protected ulong[] moof_offset; 
	public ulong[] MoofOffset { get { return this.moof_offset; } set { this.moof_offset = value; } }

	protected byte[][] traf_number; 
	public byte[][] TrafNumber { get { return this.traf_number; } set { this.traf_number = value; } }

	protected byte[][] trun_number; 
	public byte[][] TrunNumber { get { return this.trun_number; } set { this.trun_number = value; } }

	protected byte[][] sample_delta; 
	public byte[][] SampleDelta { get { return this.sample_delta; } set { this.sample_delta = value; } }

	public TrackFragmentRandomAccessBox(byte version = 0): base(IsoStream.FromFourCC("tfra"), version, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.track_ID, "track_ID"); 
		boxSize += stream.ReadBits(boxSize, readSize, 26,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.length_size_of_traf_num, "length_size_of_traf_num"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.length_size_of_trun_num, "length_size_of_trun_num"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.length_size_of_sample_num, "length_size_of_sample_num"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.number_of_entry, "number_of_entry"); 

		this.time = new ulong[IsoStream.GetInt( number_of_entry)];
		this.moof_offset = new ulong[IsoStream.GetInt( number_of_entry)];
		this.traf_number = new byte[IsoStream.GetInt( number_of_entry)][];
		this.trun_number = new byte[IsoStream.GetInt( number_of_entry)][];
		this.sample_delta = new byte[IsoStream.GetInt( number_of_entry)][];
		for (int i=0; i < number_of_entry; i++)
		{

			if (version==1)
			{
				boxSize += stream.ReadUInt64(boxSize, readSize,  out this.time[i], "time"); 
				boxSize += stream.ReadUInt64(boxSize, readSize,  out this.moof_offset[i], "moof_offset"); 
			}

			else 
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.time[i], "time"); 
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.moof_offset[i], "moof_offset"); 
			}
			boxSize += stream.ReadBits(boxSize, readSize, (uint)((length_size_of_traf_num+1) * 8 ),  out this.traf_number[i], "traf_number"); 
			boxSize += stream.ReadBits(boxSize, readSize, (uint)((length_size_of_trun_num+1) * 8 ),  out this.trun_number[i], "trun_number"); 
			boxSize += stream.ReadBits(boxSize, readSize, (uint)((length_size_of_sample_num+1) * 8 ),  out this.sample_delta[i], "sample_delta"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.track_ID, "track_ID"); 
		boxSize += stream.WriteBits(26,  this.reserved, "reserved"); 
		boxSize += stream.WriteBits(2,  this.length_size_of_traf_num, "length_size_of_traf_num"); 
		boxSize += stream.WriteBits(2,  this.length_size_of_trun_num, "length_size_of_trun_num"); 
		boxSize += stream.WriteBits(2,  this.length_size_of_sample_num, "length_size_of_sample_num"); 
		boxSize += stream.WriteUInt32( this.number_of_entry, "number_of_entry"); 

		for (int i=0; i < number_of_entry; i++)
		{

			if (version==1)
			{
				boxSize += stream.WriteUInt64( this.time[i], "time"); 
				boxSize += stream.WriteUInt64( this.moof_offset[i], "moof_offset"); 
			}

			else 
			{
				boxSize += stream.WriteUInt32( this.time[i], "time"); 
				boxSize += stream.WriteUInt32( this.moof_offset[i], "moof_offset"); 
			}
			boxSize += stream.WriteBits((uint)((length_size_of_traf_num+1) * 8 ),  this.traf_number[i], "traf_number"); 
			boxSize += stream.WriteBits((uint)((length_size_of_trun_num+1) * 8 ),  this.trun_number[i], "trun_number"); 
			boxSize += stream.WriteBits((uint)((length_size_of_sample_num+1) * 8 ),  this.sample_delta[i], "sample_delta"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // track_ID
		boxSize += 26; // reserved
		boxSize += 2; // length_size_of_traf_num
		boxSize += 2; // length_size_of_trun_num
		boxSize += 2; // length_size_of_sample_num
		boxSize += 32; // number_of_entry

		for (int i=0; i < number_of_entry; i++)
		{

			if (version==1)
			{
				boxSize += 64; // time
				boxSize += 64; // moof_offset
			}

			else 
			{
				boxSize += 32; // time
				boxSize += 32; // moof_offset
			}
			boxSize += (ulong)((length_size_of_traf_num+1) * 8 ); // traf_number
			boxSize += (ulong)((length_size_of_trun_num+1) * 8 ); // trun_number
			boxSize += (ulong)((length_size_of_sample_num+1) * 8 ); // sample_delta
		}
		return boxSize;
	}
}

}
