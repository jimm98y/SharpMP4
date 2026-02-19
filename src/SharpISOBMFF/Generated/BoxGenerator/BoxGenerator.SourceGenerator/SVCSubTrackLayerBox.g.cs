using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SVCSubTrackLayerBox
	extends FullBox('sstl', 0, 0) {
	unsigned int(16) item_count;
	for(i = 0; i< item_count; i++) {
		unsigned int(3)	dependency_id;
		unsigned int(4)	quality_id;
		unsigned int(3)	temporal_id;
		unsigned int(6)	priority_id;
		unsigned int(2)	dependency_id_range;
		unsigned int(2) 	quality_id_range;
		unsigned int(2)	temporal_id_range;
		unsigned int(2)	priority_id_range;
	}
}
*/
public partial class SVCSubTrackLayerBox : FullBox
{
	public const string TYPE = "sstl";
	public override string DisplayName { get { return "SVCSubTrackLayerBox"; } }

	protected ushort item_count; 
	public ushort ItemCount { get { return this.item_count; } set { this.item_count = value; } }

	protected byte[] dependency_id; 
	public byte[] DependencyId { get { return this.dependency_id; } set { this.dependency_id = value; } }

	protected byte[] quality_id; 
	public byte[] QualityId { get { return this.quality_id; } set { this.quality_id = value; } }

	protected byte[] temporal_id; 
	public byte[] TemporalId { get { return this.temporal_id; } set { this.temporal_id = value; } }

	protected byte[] priority_id; 
	public byte[] PriorityId { get { return this.priority_id; } set { this.priority_id = value; } }

	protected byte[] dependency_id_range; 
	public byte[] DependencyIdRange { get { return this.dependency_id_range; } set { this.dependency_id_range = value; } }

	protected byte[] quality_id_range; 
	public byte[] QualityIdRange { get { return this.quality_id_range; } set { this.quality_id_range = value; } }

	protected byte[] temporal_id_range; 
	public byte[] TemporalIdRange { get { return this.temporal_id_range; } set { this.temporal_id_range = value; } }

	protected byte[] priority_id_range; 
	public byte[] PriorityIdRange { get { return this.priority_id_range; } set { this.priority_id_range = value; } }

	public SVCSubTrackLayerBox(): base(IsoStream.FromFourCC("sstl"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.item_count, "item_count"); 

		this.dependency_id = new byte[IsoStream.GetInt( item_count)];
		this.quality_id = new byte[IsoStream.GetInt( item_count)];
		this.temporal_id = new byte[IsoStream.GetInt( item_count)];
		this.priority_id = new byte[IsoStream.GetInt( item_count)];
		this.dependency_id_range = new byte[IsoStream.GetInt( item_count)];
		this.quality_id_range = new byte[IsoStream.GetInt( item_count)];
		this.temporal_id_range = new byte[IsoStream.GetInt( item_count)];
		this.priority_id_range = new byte[IsoStream.GetInt( item_count)];
		for (int i = 0; i< item_count; i++)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.dependency_id[i], "dependency_id"); 
			boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.quality_id[i], "quality_id"); 
			boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.temporal_id[i], "temporal_id"); 
			boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.priority_id[i], "priority_id"); 
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.dependency_id_range[i], "dependency_id_range"); 
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.quality_id_range[i], "quality_id_range"); 
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.temporal_id_range[i], "temporal_id_range"); 
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.priority_id_range[i], "priority_id_range"); 
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
			boxSize += stream.WriteBits(3,  this.dependency_id[i], "dependency_id"); 
			boxSize += stream.WriteBits(4,  this.quality_id[i], "quality_id"); 
			boxSize += stream.WriteBits(3,  this.temporal_id[i], "temporal_id"); 
			boxSize += stream.WriteBits(6,  this.priority_id[i], "priority_id"); 
			boxSize += stream.WriteBits(2,  this.dependency_id_range[i], "dependency_id_range"); 
			boxSize += stream.WriteBits(2,  this.quality_id_range[i], "quality_id_range"); 
			boxSize += stream.WriteBits(2,  this.temporal_id_range[i], "temporal_id_range"); 
			boxSize += stream.WriteBits(2,  this.priority_id_range[i], "priority_id_range"); 
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
			boxSize += 3; // dependency_id
			boxSize += 4; // quality_id
			boxSize += 3; // temporal_id
			boxSize += 6; // priority_id
			boxSize += 2; // dependency_id_range
			boxSize += 2; // quality_id_range
			boxSize += 2; // temporal_id_range
			boxSize += 2; // priority_id_range
		}
		return boxSize;
	}
}

}
