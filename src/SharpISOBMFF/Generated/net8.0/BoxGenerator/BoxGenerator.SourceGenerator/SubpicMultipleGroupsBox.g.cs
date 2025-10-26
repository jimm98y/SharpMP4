using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class SubpicMultipleGroupsBox extends EntityToGroupBox('amgl',0,0)
{
	unsigned int(1) level_is_present_flag;
	unsigned int(1) level_is_static_flag;
	bit(7) reserved = 0;
	if( level_is_present_flag )
		unsigned int(8) level_idc;
	if( level_is_static_flag == 0 )
		unsigned_int(32) level_info_entity_idx;
	unsigned int(16) num_subgroup_ids;
	subgroupIdLen = (num_subgroup_ids >= (1 << 8)) ? 16 : 8;
	for (i = 0; i < num_entities_in_group; i++)
		unsigned int(subgroupIdLen) track_subgroup_id[i];
	for (i = 0; i < num_subgroup_ids; i++)
		unsigned int(16) num_active_tracks[i];
}
*/
public partial class SubpicMultipleGroupsBox : EntityToGroupBox
{
	public const string TYPE = "amgl";
	public override string DisplayName { get { return "SubpicMultipleGroupsBox"; } }

	protected bool level_is_present_flag; 
	public bool LevelIsPresentFlag { get { return this.level_is_present_flag; } set { this.level_is_present_flag = value; } }

	protected bool level_is_static_flag; 
	public bool LevelIsStaticFlag { get { return this.level_is_static_flag; } set { this.level_is_static_flag = value; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte level_idc; 
	public byte LevelIdc { get { return this.level_idc; } set { this.level_idc = value; } }

	protected uint level_info_entity_idx; 
	public uint LevelInfoEntityIdx { get { return this.level_info_entity_idx; } set { this.level_info_entity_idx = value; } }

	protected ushort num_subgroup_ids; 
	public ushort NumSubgroupIds { get { return this.num_subgroup_ids; } set { this.num_subgroup_ids = value; } }

	protected byte[][] track_subgroup_id; 
	public byte[][] TrackSubgroupId { get { return this.track_subgroup_id; } set { this.track_subgroup_id = value; } }

	protected ushort[] num_active_tracks; 
	public ushort[] NumActiveTracks { get { return this.num_active_tracks; } set { this.num_active_tracks = value; } }

	public SubpicMultipleGroupsBox(): base(IsoStream.FromFourCC("amgl"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.level_is_present_flag, "level_is_present_flag"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.level_is_static_flag, "level_is_static_flag"); 
		boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.reserved, "reserved"); 

		if ( level_is_present_flag )
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.level_idc, "level_idc"); 
		}

		if ( level_is_static_flag == false )
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.level_info_entity_idx, "level_info_entity_idx"); 
		}
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.num_subgroup_ids, "num_subgroup_ids"); 
		ulong subgroupIdLen = (ulong)((num_subgroup_ids >= (1 << 8)) ? 16 : 8);

		this.track_subgroup_id = new byte[IsoStream.GetInt( num_entities_in_group)][];
		for (int i = 0; i < num_entities_in_group; i++)
		{
			boxSize += stream.ReadBits(boxSize, readSize, (uint)(subgroupIdLen ),  out this.track_subgroup_id[i], "track_subgroup_id"); 
		}

		this.num_active_tracks = new ushort[IsoStream.GetInt( num_subgroup_ids)];
		for (int i = 0; i < num_subgroup_ids; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.num_active_tracks[i], "num_active_tracks"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBit( this.level_is_present_flag, "level_is_present_flag"); 
		boxSize += stream.WriteBit( this.level_is_static_flag, "level_is_static_flag"); 
		boxSize += stream.WriteBits(7,  this.reserved, "reserved"); 

		if ( level_is_present_flag )
		{
			boxSize += stream.WriteUInt8( this.level_idc, "level_idc"); 
		}

		if ( level_is_static_flag == false )
		{
			boxSize += stream.WriteUInt32( this.level_info_entity_idx, "level_info_entity_idx"); 
		}
		boxSize += stream.WriteUInt16( this.num_subgroup_ids, "num_subgroup_ids"); 
		ulong subgroupIdLen = (ulong)((num_subgroup_ids >= (1 << 8)) ? 16 : 8);

		for (int i = 0; i < num_entities_in_group; i++)
		{
			boxSize += stream.WriteBits((uint)(subgroupIdLen ),  this.track_subgroup_id[i], "track_subgroup_id"); 
		}

		for (int i = 0; i < num_subgroup_ids; i++)
		{
			boxSize += stream.WriteUInt16( this.num_active_tracks[i], "num_active_tracks"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 1; // level_is_present_flag
		boxSize += 1; // level_is_static_flag
		boxSize += 7; // reserved

		if ( level_is_present_flag )
		{
			boxSize += 8; // level_idc
		}

		if ( level_is_static_flag == false )
		{
			boxSize += 32; // level_info_entity_idx
		}
		boxSize += 16; // num_subgroup_ids
		ulong subgroupIdLen = (ulong)((num_subgroup_ids >= (1 << 8)) ? 16 : 8);

		for (int i = 0; i < num_entities_in_group; i++)
		{
			boxSize += (ulong)(subgroupIdLen ); // track_subgroup_id
		}

		for (int i = 0; i < num_subgroup_ids; i++)
		{
			boxSize += 16; // num_active_tracks
		}
		return boxSize;
	}
}

}
