using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class TrackHeaderBox
	extends FullBox('tkhd', version, flags){
	if (version==1) {
		unsigned int(64)	creation_time;
		unsigned int(64)	modification_time;
		unsigned int(32)	track_ID;
		const unsigned int(32)	reserved = 0;
		unsigned int(64)	duration;
	} else { // version==0
		unsigned int(32)	creation_time;
		unsigned int(32)	modification_time;
		unsigned int(32)	track_ID;
		const unsigned int(32)	reserved = 0;
		unsigned int(32)	duration;
	}
	const unsigned int(32)[2]	reserved = 0;
	template int(16) layer = 0;
	template int(16) alternate_group = 0;
	template int(16)	volume = {if track_is_audio 0x0100 else 0};
	const unsigned int(16)	reserved = 0;
	template int(32)[9]	matrix=
		{ 0x00010000,0,0,0,0x00010000,0,0,0,0x40000000 };
		// unity matrix
	unsigned int(32) width;
	unsigned int(32) height;
}
*/
public partial class TrackHeaderBox : FullBox
{
	public const string TYPE = "tkhd";
	public override string DisplayName { get { return "TrackHeaderBox"; } }

	protected ulong creation_time; 
	public ulong CreationTime { get { return this.creation_time; } set { this.creation_time = value; } }

	protected ulong modification_time; 
	public ulong ModificationTime { get { return this.modification_time; } set { this.modification_time = value; } }

	protected uint track_ID; 
	public uint TrackID { get { return this.track_ID; } set { this.track_ID = value; } }

	protected uint reserved = 0; 
	public uint Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected ulong duration; 
	public ulong Duration { get { return this.duration; } set { this.duration = value; } }

	protected uint reserved0 = 0; 
	public uint Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected uint[] reserved1 = []; 
	public uint[] Reserved1 { get { return this.reserved1; } set { this.reserved1 = value; } }

	protected short layer = 0; 
	public short Layer { get { return this.layer; } set { this.layer = value; } }

	protected short alternate_group = 0; 
	public short AlternateGroup { get { return this.alternate_group; } set { this.alternate_group = value; } }

	protected short volume = 0; // = { default samplerate of media}<<16;
	public short Volume { get { return this.volume; } set { this.volume = value; } }

	protected ushort reserved2 = 0; 
	public ushort Reserved2 { get { return this.reserved2; } set { this.reserved2 = value; } }

	protected int[] matrix =
		{ 0x00010000,0,0,0,0x00010000,0,0,0,0x40000000 };  //  unity matrix
	public int[] Matrix { get { return this.matrix; } set { this.matrix = value; } }

	protected uint width; 
	public uint Width { get { return this.width; } set { this.width = value; } }

	protected uint height; 
	public uint Height { get { return this.height; } set { this.height = value; } }

	public TrackHeaderBox(byte version = 0, uint flags = 0): base(IsoStream.FromFourCC("tkhd"), version, flags)
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
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.track_ID, "track_ID"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reserved, "reserved"); 
			boxSize += stream.ReadUInt64(boxSize, readSize,  out this.duration, "duration"); 
		}

		else 
		{
			/*  version==0 */
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.creation_time, "creation_time"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.modification_time, "modification_time"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.track_ID, "track_ID"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reserved0, "reserved0"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.duration, "duration"); 
		}
		boxSize += stream.ReadUInt32Array(boxSize, readSize, 2,  out this.reserved1, "reserved1"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.layer, "layer"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.alternate_group, "alternate_group"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.volume, "volume"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.reserved2, "reserved2"); 
		boxSize += stream.ReadInt32Array(boxSize, readSize, 9,  out this.matrix, "matrix"); // unity matrix
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.width, "width"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.height, "height"); 
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
			boxSize += stream.WriteUInt32( this.track_ID, "track_ID"); 
			boxSize += stream.WriteUInt32( this.reserved, "reserved"); 
			boxSize += stream.WriteUInt64( this.duration, "duration"); 
		}

		else 
		{
			/*  version==0 */
			boxSize += stream.WriteUInt32( this.creation_time, "creation_time"); 
			boxSize += stream.WriteUInt32( this.modification_time, "modification_time"); 
			boxSize += stream.WriteUInt32( this.track_ID, "track_ID"); 
			boxSize += stream.WriteUInt32( this.reserved0, "reserved0"); 
			boxSize += stream.WriteUInt32( this.duration, "duration"); 
		}
		boxSize += stream.WriteUInt32Array(2,  this.reserved1, "reserved1"); 
		boxSize += stream.WriteInt16( this.layer, "layer"); 
		boxSize += stream.WriteInt16( this.alternate_group, "alternate_group"); 
		boxSize += stream.WriteInt16( this.volume, "volume"); 
		boxSize += stream.WriteUInt16( this.reserved2, "reserved2"); 
		boxSize += stream.WriteInt32Array(9,  this.matrix, "matrix"); // unity matrix
		boxSize += stream.WriteUInt32( this.width, "width"); 
		boxSize += stream.WriteUInt32( this.height, "height"); 
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
			boxSize += 32; // track_ID
			boxSize += 32; // reserved
			boxSize += 64; // duration
		}

		else 
		{
			/*  version==0 */
			boxSize += 32; // creation_time
			boxSize += 32; // modification_time
			boxSize += 32; // track_ID
			boxSize += 32; // reserved0
			boxSize += 32; // duration
		}
		boxSize += 2 * 32; // reserved1
		boxSize += 16; // layer
		boxSize += 16; // alternate_group
		boxSize += 16; // volume
		boxSize += 16; // reserved2
		boxSize += 9 * 32; // matrix
		boxSize += 32; // width
		boxSize += 32; // height
		return boxSize;
	}
}

}
