using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class TrackRunBox extends FullBox('trun', version, tr_flags) {
    unsigned int(32)	sample_count;
    if(flags & 0x1) {
       signed int(32)	data_offset;
    }
    if(flags & 0x4) {
       unsigned int(32)	first_sample_flags;
    }
    // all fields in the following array are optional
    // as indicated by bits set in the tr_flags
    TrunEntry(version, flags)[ sample_count ];
}


*/
public partial class TrackRunBox : FullBox
{
	public const string TYPE = "trun";
	public override string DisplayName { get { return "TrackRunBox"; } }

	protected uint sample_count; 
	public uint SampleCount { get { return this.sample_count; } set { this.sample_count = value; } }

	protected int data_offset; 
	public int DataOffset { get { return this.data_offset; } set { this.data_offset = value; } }

	protected uint first_sample_flags; 
	public uint FirstSampleFlags { get { return this.first_sample_flags; } set { this.first_sample_flags = value; } }

	protected TrunEntry[] TrunEntry; 
	public TrunEntry[] _TrunEntry { get { return this.TrunEntry; } set { this.TrunEntry = value; } }

	public TrackRunBox(byte version = 0, uint tr_flags = 0): base(IsoStream.FromFourCC("trun"), version, tr_flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_count, "sample_count"); 

		if ((flags  &  0x1) ==  0x1)
		{
			boxSize += stream.ReadInt32(boxSize, readSize,  out this.data_offset, "data_offset"); 
		}

		if ((flags  &  0x4) ==  0x4)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.first_sample_flags, "first_sample_flags"); 
		}
		/*  all fields in the following array are optional */
		/*  as indicated by bits set in the tr_flags */
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)( sample_count ), () => new TrunEntry(version, flags),  out this.TrunEntry, "TrunEntry"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.sample_count, "sample_count"); 

		if ((flags  &  0x1) ==  0x1)
		{
			boxSize += stream.WriteInt32( this.data_offset, "data_offset"); 
		}

		if ((flags  &  0x4) ==  0x4)
		{
			boxSize += stream.WriteUInt32( this.first_sample_flags, "first_sample_flags"); 
		}
		/*  all fields in the following array are optional */
		/*  as indicated by bits set in the tr_flags */
		boxSize += stream.WriteClass( this.TrunEntry, "TrunEntry"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // sample_count

		if ((flags  &  0x1) ==  0x1)
		{
			boxSize += 32; // data_offset
		}

		if ((flags  &  0x4) ==  0x4)
		{
			boxSize += 32; // first_sample_flags
		}
		/*  all fields in the following array are optional */
		/*  as indicated by bits set in the tr_flags */
		boxSize += IsoStream.CalculateClassSize(TrunEntry); // TrunEntry
		return boxSize;
	}
}

}
