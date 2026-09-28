using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// VC-1's configuration (SMPTE RP 2025), as FFmpeg's mov_write_dvc1_structs writes it, the sequence header after
aligned(8) class Dvc1Box() extends Box('dvc1') {
	unsigned int(4) profile;
	unsigned int(3) level;
	bit(1) reserved = 0;
	unsigned int(3) level2;
	unsigned int(1) cbr;
	bit(6) reserved0 = 0;
	unsigned int(1) no_interlace;
	unsigned int(1) no_multiple_seq;
	unsigned int(1) no_multiple_entry;
	unsigned int(1) no_slice_code;
	unsigned int(1) no_bframe;
	bit(1) reserved1 = 0;
	unsigned int(32) framerate;
	bit(8) sequenceHeader[];
}
*/
public partial class Dvc1Box : Box
{
	public const string TYPE = "dvc1";
	public override string DisplayName { get { return "Dvc1Box"; } }

	protected byte profile; 
	public byte Profile { get { return this.profile; } set { this.profile = value; } }

	protected byte level; 
	public byte Level { get { return this.level; } set { this.level = value; } }

	protected bool reserved = false; 
	public bool Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte level2; 
	public byte Level2 { get { return this.level2; } set { this.level2 = value; } }

	protected bool cbr; 
	public bool Cbr { get { return this.cbr; } set { this.cbr = value; } }

	protected byte reserved0 = 0; 
	public byte Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected bool no_interlace; 
	public bool NoInterlace { get { return this.no_interlace; } set { this.no_interlace = value; } }

	protected bool no_multiple_seq; 
	public bool NoMultipleSeq { get { return this.no_multiple_seq; } set { this.no_multiple_seq = value; } }

	protected bool no_multiple_entry; 
	public bool NoMultipleEntry { get { return this.no_multiple_entry; } set { this.no_multiple_entry = value; } }

	protected bool no_slice_code; 
	public bool NoSliceCode { get { return this.no_slice_code; } set { this.no_slice_code = value; } }

	protected bool no_bframe; 
	public bool NoBframe { get { return this.no_bframe; } set { this.no_bframe = value; } }

	protected bool reserved1 = false; 
	public bool Reserved1 { get { return this.reserved1; } set { this.reserved1 = value; } }

	protected uint framerate; 
	public uint Framerate { get { return this.framerate; } set { this.framerate = value; } }

	protected byte[] sequenceHeader; 
	public byte[] SequenceHeader { get { return this.sequenceHeader; } set { this.sequenceHeader = value; } }

	public Dvc1Box(): base(IsoStream.FromFourCC("dvc1"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.profile, "profile"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.level, "level"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.level2, "level2"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.cbr, "cbr"); 
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved0, "reserved0"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.no_interlace, "no_interlace"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.no_multiple_seq, "no_multiple_seq"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.no_multiple_entry, "no_multiple_entry"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.no_slice_code, "no_slice_code"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.no_bframe, "no_bframe"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved1, "reserved1"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.framerate, "framerate"); 
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.sequenceHeader, "sequenceHeader"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(4,  this.profile, "profile"); 
		boxSize += stream.WriteBits(3,  this.level, "level"); 
		boxSize += stream.WriteBit( this.reserved, "reserved"); 
		boxSize += stream.WriteBits(3,  this.level2, "level2"); 
		boxSize += stream.WriteBit( this.cbr, "cbr"); 
		boxSize += stream.WriteBits(6,  this.reserved0, "reserved0"); 
		boxSize += stream.WriteBit( this.no_interlace, "no_interlace"); 
		boxSize += stream.WriteBit( this.no_multiple_seq, "no_multiple_seq"); 
		boxSize += stream.WriteBit( this.no_multiple_entry, "no_multiple_entry"); 
		boxSize += stream.WriteBit( this.no_slice_code, "no_slice_code"); 
		boxSize += stream.WriteBit( this.no_bframe, "no_bframe"); 
		boxSize += stream.WriteBit( this.reserved1, "reserved1"); 
		boxSize += stream.WriteUInt32( this.framerate, "framerate"); 
		boxSize += stream.WriteUInt8ArrayTillEnd( this.sequenceHeader, "sequenceHeader"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 4; // profile
		boxSize += 3; // level
		boxSize += 1; // reserved
		boxSize += 3; // level2
		boxSize += 1; // cbr
		boxSize += 6; // reserved0
		boxSize += 1; // no_interlace
		boxSize += 1; // no_multiple_seq
		boxSize += 1; // no_multiple_entry
		boxSize += 1; // no_slice_code
		boxSize += 1; // no_bframe
		boxSize += 1; // reserved1
		boxSize += 32; // framerate
		boxSize += ((ulong)sequenceHeader.Length * 8); // sequenceHeader
		return boxSize;
	}
}

}
