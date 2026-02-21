using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SubpicCommonGroupBox extends EntityToGroupBox('acgl',0,0)
{	unsigned int(1) level_is_present_flag;
	unsigned int(1) level_is_static_flag;
	bit(6) reserved = 0;
	if( level_is_present_flag )
		unsigned int(8) level_idc;
	if( level_is_static_flag == 0 )
		unsigned_int(32) level_info_entity_idx;
	unsigned int(16) num_active_tracks;
}
*/
public partial class SubpicCommonGroupBox : EntityToGroupBox
{
	public const string TYPE = "acgl";
	public override string DisplayName { get { return "SubpicCommonGroupBox"; } }

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

	protected ushort num_active_tracks; 
	public ushort NumActiveTracks { get { return this.num_active_tracks; } set { this.num_active_tracks = value; } }

	public SubpicCommonGroupBox(): base(IsoStream.FromFourCC("acgl"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.level_is_present_flag, "level_is_present_flag"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.level_is_static_flag, "level_is_static_flag"); 
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved, "reserved"); 

		if ( level_is_present_flag )
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.level_idc, "level_idc"); 
		}

		if ( level_is_static_flag == false )
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.level_info_entity_idx, "level_info_entity_idx"); 
		}
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.num_active_tracks, "num_active_tracks"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBit( this.level_is_present_flag, "level_is_present_flag"); 
		boxSize += stream.WriteBit( this.level_is_static_flag, "level_is_static_flag"); 
		boxSize += stream.WriteBits(6,  this.reserved, "reserved"); 

		if ( level_is_present_flag )
		{
			boxSize += stream.WriteUInt8( this.level_idc, "level_idc"); 
		}

		if ( level_is_static_flag == false )
		{
			boxSize += stream.WriteUInt32( this.level_info_entity_idx, "level_info_entity_idx"); 
		}
		boxSize += stream.WriteUInt16( this.num_active_tracks, "num_active_tracks"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 1; // level_is_present_flag
		boxSize += 1; // level_is_static_flag
		boxSize += 6; // reserved

		if ( level_is_present_flag )
		{
			boxSize += 8; // level_idc
		}

		if ( level_is_static_flag == false )
		{
			boxSize += 32; // level_info_entity_idx
		}
		boxSize += 16; // num_active_tracks
		return boxSize;
	}
}

}
