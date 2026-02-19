using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class MultiviewGroupBox extends FullBox('mvcg', version = 0, flags) {
	unsigned int(32) multiview_group_id;
	unsigned int(16) num_entries;
	bit(8) reserved = 0;
	for(i=0; i<num_entries; i++) {
		unsigned int(8) entry_type;
		if (entry_type == 0)
			unsigned int(32) track_id;
		else if (entry_type == 1) {
			unsigned int(32) track_id;
			unsigned int(16) tier_id;
		}
		else if (entry_type == 2) {
			bit(6) reserved1 = 0;
			unsigned int(10) output_view_id;
		}
		else if (entry_type == 3) {
			bit(6) reserved2 = 0;
			unsigned int(10) start_view_id;
			unsigned int(16) view_count;
		}
	}
	TierInfoBox subset_stream_info; 			// optional
	MultiviewRelationAttributeBox relation_attributes; // optional
	TierBitRateBox subset_stream_bit_rate; // optional
	BufferingBox subset_stream_buffering; 	// optional
	MultiviewSceneInfoBox multiview_scene_info; 			// optional
}
*/
public partial class MultiviewGroupBox : FullBox
{
	public const string TYPE = "mvcg";
	public override string DisplayName { get { return "MultiviewGroupBox"; } }

	protected uint multiview_group_id; 
	public uint MultiviewGroupId { get { return this.multiview_group_id; } set { this.multiview_group_id = value; } }

	protected ushort num_entries; 
	public ushort NumEntries { get { return this.num_entries; } set { this.num_entries = value; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte[] entry_type; 
	public byte[] EntryType { get { return this.entry_type; } set { this.entry_type = value; } }

	protected uint[] track_id; 
	public uint[] TrackId { get { return this.track_id; } set { this.track_id = value; } }

	protected ushort[] tier_id; 
	public ushort[] TierId { get { return this.tier_id; } set { this.tier_id = value; } }

	protected byte[] reserved1; 
	public byte[] Reserved1 { get { return this.reserved1; } set { this.reserved1 = value; } }

	protected ushort[] output_view_id; 
	public ushort[] OutputViewId { get { return this.output_view_id; } set { this.output_view_id = value; } }

	protected byte[] reserved2; 
	public byte[] Reserved2 { get { return this.reserved2; } set { this.reserved2 = value; } }

	protected ushort[] start_view_id; 
	public ushort[] StartViewId { get { return this.start_view_id; } set { this.start_view_id = value; } }

	protected ushort[] view_count; 
	public ushort[] ViewCount { get { return this.view_count; } set { this.view_count = value; } }
	public TierInfoBox SubsetStreamInfo { get { return this.children.OfType<TierInfoBox>().FirstOrDefault(); } }
	public MultiviewRelationAttributeBox RelationAttributes { get { return this.children.OfType<MultiviewRelationAttributeBox>().FirstOrDefault(); } }
	public TierBitRateBox SubsetStreamBitRate { get { return this.children.OfType<TierBitRateBox>().FirstOrDefault(); } }
	public BufferingBox SubsetStreamBuffering { get { return this.children.OfType<BufferingBox>().FirstOrDefault(); } }
	public MultiviewSceneInfoBox MultiviewSceneInfo { get { return this.children.OfType<MultiviewSceneInfoBox>().FirstOrDefault(); } }

	public MultiviewGroupBox(uint flags = 0): base(IsoStream.FromFourCC("mvcg"), 0, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.multiview_group_id, "multiview_group_id"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.num_entries, "num_entries"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.reserved, "reserved"); 

		this.entry_type = new byte[IsoStream.GetInt(num_entries)];
		this.track_id = new uint[IsoStream.GetInt(num_entries)];
		this.tier_id = new ushort[IsoStream.GetInt(num_entries)];
		this.reserved1 = new byte[IsoStream.GetInt(num_entries)];
		this.output_view_id = new ushort[IsoStream.GetInt(num_entries)];
		this.reserved2 = new byte[IsoStream.GetInt(num_entries)];
		this.start_view_id = new ushort[IsoStream.GetInt(num_entries)];
		this.view_count = new ushort[IsoStream.GetInt(num_entries)];
		for (int i=0; i<num_entries; i++)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.entry_type[i], "entry_type"); 

			if (entry_type[i] == 0)
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.track_id[i], "track_id"); 
			}

			else if (entry_type[i] == 1)
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.track_id[i], "track_id"); 
				boxSize += stream.ReadUInt16(boxSize, readSize,  out this.tier_id[i], "tier_id"); 
			}

			else if (entry_type[i] == 2)
			{
				boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved1[i], "reserved1"); 
				boxSize += stream.ReadBits(boxSize, readSize, 10,  out this.output_view_id[i], "output_view_id"); 
			}

			else if (entry_type[i] == 3)
			{
				boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved2[i], "reserved2"); 
				boxSize += stream.ReadBits(boxSize, readSize, 10,  out this.start_view_id[i], "start_view_id"); 
				boxSize += stream.ReadUInt16(boxSize, readSize,  out this.view_count[i], "view_count"); 
			}
		}
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.subset_stream_info, "subset_stream_info"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.relation_attributes, "relation_attributes"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.subset_stream_bit_rate, "subset_stream_bit_rate"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.subset_stream_buffering, "subset_stream_buffering"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.multiview_scene_info, "multiview_scene_info"); // optional
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.multiview_group_id, "multiview_group_id"); 
		boxSize += stream.WriteUInt16( this.num_entries, "num_entries"); 
		boxSize += stream.WriteUInt8( this.reserved, "reserved"); 

		for (int i=0; i<num_entries; i++)
		{
			boxSize += stream.WriteUInt8( this.entry_type[i], "entry_type"); 

			if (entry_type[i] == 0)
			{
				boxSize += stream.WriteUInt32( this.track_id[i], "track_id"); 
			}

			else if (entry_type[i] == 1)
			{
				boxSize += stream.WriteUInt32( this.track_id[i], "track_id"); 
				boxSize += stream.WriteUInt16( this.tier_id[i], "tier_id"); 
			}

			else if (entry_type[i] == 2)
			{
				boxSize += stream.WriteBits(6,  this.reserved1[i], "reserved1"); 
				boxSize += stream.WriteBits(10,  this.output_view_id[i], "output_view_id"); 
			}

			else if (entry_type[i] == 3)
			{
				boxSize += stream.WriteBits(6,  this.reserved2[i], "reserved2"); 
				boxSize += stream.WriteBits(10,  this.start_view_id[i], "start_view_id"); 
				boxSize += stream.WriteUInt16( this.view_count[i], "view_count"); 
			}
		}
		// boxSize += stream.WriteBox( this.subset_stream_info, "subset_stream_info"); // optional
		// boxSize += stream.WriteBox( this.relation_attributes, "relation_attributes"); // optional
		// boxSize += stream.WriteBox( this.subset_stream_bit_rate, "subset_stream_bit_rate"); // optional
		// boxSize += stream.WriteBox( this.subset_stream_buffering, "subset_stream_buffering"); // optional
		// boxSize += stream.WriteBox( this.multiview_scene_info, "multiview_scene_info"); // optional
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // multiview_group_id
		boxSize += 16; // num_entries
		boxSize += 8; // reserved

		for (int i=0; i<num_entries; i++)
		{
			boxSize += 8; // entry_type

			if (entry_type[i] == 0)
			{
				boxSize += 32; // track_id
			}

			else if (entry_type[i] == 1)
			{
				boxSize += 32; // track_id
				boxSize += 16; // tier_id
			}

			else if (entry_type[i] == 2)
			{
				boxSize += 6; // reserved1
				boxSize += 10; // output_view_id
			}

			else if (entry_type[i] == 3)
			{
				boxSize += 6; // reserved2
				boxSize += 10; // start_view_id
				boxSize += 16; // view_count
			}
		}
		// boxSize += IsoStream.CalculateBoxSize(subset_stream_info); // subset_stream_info
		// boxSize += IsoStream.CalculateBoxSize(relation_attributes); // relation_attributes
		// boxSize += IsoStream.CalculateBoxSize(subset_stream_bit_rate); // subset_stream_bit_rate
		// boxSize += IsoStream.CalculateBoxSize(subset_stream_buffering); // subset_stream_buffering
		// boxSize += IsoStream.CalculateBoxSize(multiview_scene_info); // multiview_scene_info
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
