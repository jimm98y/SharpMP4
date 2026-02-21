using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class MovieHeaderBox extends FullBox('mvhd', version, 0) {
	if (version==1) {
		unsigned int(64)	creation_time;
		unsigned int(64)	modification_time;
		unsigned int(32)	timescale;
		unsigned int(64)	duration;
	} else { // version==0
		unsigned int(32)	creation_time;
		unsigned int(32)	modification_time;
		unsigned int(32)	timescale;
		unsigned int(32)	duration;
	}
	template int(32)	rate = 0x00010000;	// typically 1.0
	template int(16)	volume = 0x0100;	// typically, full volume
	const bit(16)	reserved = 0;
	const unsigned int(32)[2]	reserved = 0;
	template int(32)[9]	matrix =
		{ 0x00010000,0,0,0,0x00010000,0,0,0,0x40000000 };
		// Unity matrix
	bit(32)[6]	pre_defined = 0;
	unsigned int(32)	next_track_ID;
}
*/
public partial class MovieHeaderBox : FullBox
{
	public const string TYPE = "mvhd";
	public override string DisplayName { get { return "MovieHeaderBox"; } }

	protected ulong creation_time; 
	public ulong CreationTime { get { return this.creation_time; } set { this.creation_time = value; } }

	protected ulong modification_time; 
	public ulong ModificationTime { get { return this.modification_time; } set { this.modification_time = value; } }

	protected uint timescale; 
	public uint Timescale { get { return this.timescale; } set { this.timescale = value; } }

	protected ulong duration; 
	public ulong Duration { get { return this.duration; } set { this.duration = value; } }

	protected int rate = 0x00010000;  //  typically 1.0
	public int Rate { get { return this.rate; } set { this.rate = value; } }

	protected short volume = 0x0100;  //  typically, full volume
	public short Volume { get { return this.volume; } set { this.volume = value; } }

	protected ushort reserved = 0; 
	public ushort Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected uint[] reserved0 = []; 
	public uint[] Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected int[] matrix =
		{ 0x00010000,0,0,0,0x00010000,0,0,0,0x40000000 };  //  Unity matrix
	public int[] Matrix { get { return this.matrix; } set { this.matrix = value; } }

	protected uint[] pre_defined = []; 
	public uint[] PreDefined { get { return this.pre_defined; } set { this.pre_defined = value; } }

	protected uint next_track_ID; 
	public uint NextTrackID { get { return this.next_track_ID; } set { this.next_track_ID = value; } }

	public MovieHeaderBox(byte version = 0): base(IsoStream.FromFourCC("mvhd"), version, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);

		if (version==1)
		{
			boxSize += stream.ReadUInt64(boxSize, readSize,  out this.creation_time, "creation_time"); 
			boxSize += stream.ReadUInt64(boxSize, readSize,  out this.modification_time, "modification_time"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.timescale, "timescale"); 
			boxSize += stream.ReadUInt64(boxSize, readSize,  out this.duration, "duration"); 
		}

		else 
		{
			/*  version==0 */
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.creation_time, "creation_time"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.modification_time, "modification_time"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.timescale, "timescale"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.duration, "duration"); 
		}
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.rate, "rate"); // typically 1.0
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.volume, "volume"); // typically, full volume
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt32Array(boxSize, readSize, 2,  out this.reserved0, "reserved0"); 
		boxSize += stream.ReadInt32Array(boxSize, readSize, 9,  out this.matrix, "matrix"); // Unity matrix
		boxSize += stream.ReadUInt32Array(boxSize, readSize, 6,  out this.pre_defined, "pre_defined"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.next_track_ID, "next_track_ID"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);

		if (version==1)
		{
			boxSize += stream.WriteUInt64( this.creation_time, "creation_time"); 
			boxSize += stream.WriteUInt64( this.modification_time, "modification_time"); 
			boxSize += stream.WriteUInt32( this.timescale, "timescale"); 
			boxSize += stream.WriteUInt64( this.duration, "duration"); 
		}

		else 
		{
			/*  version==0 */
			boxSize += stream.WriteUInt32( this.creation_time, "creation_time"); 
			boxSize += stream.WriteUInt32( this.modification_time, "modification_time"); 
			boxSize += stream.WriteUInt32( this.timescale, "timescale"); 
			boxSize += stream.WriteUInt32( this.duration, "duration"); 
		}
		boxSize += stream.WriteInt32( this.rate, "rate"); // typically 1.0
		boxSize += stream.WriteInt16( this.volume, "volume"); // typically, full volume
		boxSize += stream.WriteUInt16( this.reserved, "reserved"); 
		boxSize += stream.WriteUInt32Array(2,  this.reserved0, "reserved0"); 
		boxSize += stream.WriteInt32Array(9,  this.matrix, "matrix"); // Unity matrix
		boxSize += stream.WriteUInt32Array(6,  this.pre_defined, "pre_defined"); 
		boxSize += stream.WriteUInt32( this.next_track_ID, "next_track_ID"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();

		if (version==1)
		{
			boxSize += 64; // creation_time
			boxSize += 64; // modification_time
			boxSize += 32; // timescale
			boxSize += 64; // duration
		}

		else 
		{
			/*  version==0 */
			boxSize += 32; // creation_time
			boxSize += 32; // modification_time
			boxSize += 32; // timescale
			boxSize += 32; // duration
		}
		boxSize += 32; // rate
		boxSize += 16; // volume
		boxSize += 16; // reserved
		boxSize += 2 * 32; // reserved0
		boxSize += 9 * 32; // matrix
		boxSize += 6 * 32; // pre_defined
		boxSize += 32; // next_track_ID
		return boxSize;
	}
}

}
