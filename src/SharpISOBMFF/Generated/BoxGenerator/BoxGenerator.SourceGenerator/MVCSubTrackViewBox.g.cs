using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class MVCSubTrackViewBox
	extends FullBox('mstv', 0, 0) {
	unsigned int(16) item_count;
	for(i = 0; i< item_count; i++) {
		unsigned int(10)	view_id;
		unsigned int(4)	temporal_id;
		unsigned int(2)	reserved;
	}
}
*/
public partial class MVCSubTrackViewBox : FullBox
{
	public const string TYPE = "mstv";
	public override string DisplayName { get { return "MVCSubTrackViewBox"; } }

	protected ushort item_count; 
	public ushort ItemCount { get { return this.item_count; } set { this.item_count = value; } }

	protected ushort[] view_id; 
	public ushort[] ViewId { get { return this.view_id; } set { this.view_id = value; } }

	protected byte[] temporal_id; 
	public byte[] TemporalId { get { return this.temporal_id; } set { this.temporal_id = value; } }

	protected byte[] reserved; 
	public byte[] Reserved { get { return this.reserved; } set { this.reserved = value; } }

	public MVCSubTrackViewBox(): base(IsoStream.FromFourCC("mstv"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.item_count, "item_count"); 

		this.view_id = new ushort[IsoStream.GetInt( item_count)];
		this.temporal_id = new byte[IsoStream.GetInt( item_count)];
		this.reserved = new byte[IsoStream.GetInt( item_count)];
		for (int i = 0; i< item_count; i++)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 10,  out this.view_id[i], "view_id"); 
			boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.temporal_id[i], "temporal_id"); 
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.reserved[i], "reserved"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.item_count, "item_count"); 

		for (int i = 0; i< item_count; i++)
		{
			boxSize += stream.WriteBits(10,  this.view_id[i], "view_id"); 
			boxSize += stream.WriteBits(4,  this.temporal_id[i], "temporal_id"); 
			boxSize += stream.WriteBits(2,  this.reserved[i], "reserved"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // item_count

		for (int i = 0; i< item_count; i++)
		{
			boxSize += 10; // view_id
			boxSize += 4; // temporal_id
			boxSize += 2; // reserved
		}
		return boxSize;
	}
}

}
