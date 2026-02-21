using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class VvcPTLRecord(num_sublayers) {
	bit(2) reserved = 0;
	unsigned int(6) num_bytes_constraint_info;
	unsigned int(7) general_profile_idc;
	unsigned int(1) general_tier_flag;
	unsigned int(8) general_level_idc;
	unsigned int(1) ptl_frame_only_constraint_flag;
	unsigned int(1) ptl_multi_layer_enabled_flag;
	unsigned int(8*num_bytes_constraint_info - 2) general_constraint_info;
	for (i=num_sublayers - 2; i >= 0; i--)
		unsigned int(1) ptl_sublayer_level_present_flag[i];
	for (j=num_sublayers; j<=8 && num_sublayers > 1; j++)
		bit(1) ptl_reserved_zero_bit = 0;
	for (i=num_sublayers-2; i >= 0; i--) {
		if (ptl_sublayer_level_present_flag[i])
			unsigned int(8) sublayer_level_idc[i];
		}
	unsigned int(8) ptl_num_sub_profiles;
	for (j=0; j < ptl_num_sub_profiles; j++)
		unsigned int(32) general_sub_profile_idc[j];
} 
*/
public partial class VvcPTLRecord : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "VvcPTLRecord"; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte num_bytes_constraint_info; 
	public byte NumBytesConstraintInfo { get { return this.num_bytes_constraint_info; } set { this.num_bytes_constraint_info = value; } }

	protected byte general_profile_idc; 
	public byte GeneralProfileIdc { get { return this.general_profile_idc; } set { this.general_profile_idc = value; } }

	protected bool general_tier_flag; 
	public bool GeneralTierFlag { get { return this.general_tier_flag; } set { this.general_tier_flag = value; } }

	protected byte general_level_idc; 
	public byte GeneralLevelIdc { get { return this.general_level_idc; } set { this.general_level_idc = value; } }

	protected bool ptl_frame_only_constraint_flag; 
	public bool PtlFrameOnlyConstraintFlag { get { return this.ptl_frame_only_constraint_flag; } set { this.ptl_frame_only_constraint_flag = value; } }

	protected bool ptl_multi_layer_enabled_flag; 
	public bool PtlMultiLayerEnabledFlag { get { return this.ptl_multi_layer_enabled_flag; } set { this.ptl_multi_layer_enabled_flag = value; } }

	protected byte[] general_constraint_info; 
	public byte[] GeneralConstraintInfo { get { return this.general_constraint_info; } set { this.general_constraint_info = value; } }

	protected bool[] ptl_sublayer_level_present_flag; 
	public bool[] PtlSublayerLevelPresentFlag { get { return this.ptl_sublayer_level_present_flag; } set { this.ptl_sublayer_level_present_flag = value; } }

	protected bool[] ptl_reserved_zero_bit; 
	public bool[] PtlReservedZeroBit { get { return this.ptl_reserved_zero_bit; } set { this.ptl_reserved_zero_bit = value; } }

	protected byte[] sublayer_level_idc; 
	public byte[] SublayerLevelIdc { get { return this.sublayer_level_idc; } set { this.sublayer_level_idc = value; } }

	protected byte ptl_num_sub_profiles; 
	public byte PtlNumSubProfiles { get { return this.ptl_num_sub_profiles; } set { this.ptl_num_sub_profiles = value; } }

	protected uint[] general_sub_profile_idc; 
	public uint[] GeneralSubProfileIdc { get { return this.general_sub_profile_idc; } set { this.general_sub_profile_idc = value; } }

	protected byte num_sublayers; 
	public byte NumSublayers { get { return this.num_sublayers; } set { this.num_sublayers = value; } }

	public VvcPTLRecord(byte num_sublayers): base()
	{
		this.num_sublayers = num_sublayers;
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.num_bytes_constraint_info, "num_bytes_constraint_info"); 
		boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.general_profile_idc, "general_profile_idc"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.general_tier_flag, "general_tier_flag"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.general_level_idc, "general_level_idc"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.ptl_frame_only_constraint_flag, "ptl_frame_only_constraint_flag"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.ptl_multi_layer_enabled_flag, "ptl_multi_layer_enabled_flag"); 
		boxSize += stream.ReadBits(boxSize, readSize, (uint)(8*num_bytes_constraint_info - 2 ),  out this.general_constraint_info, "general_constraint_info"); 

		this.ptl_sublayer_level_present_flag = new bool[IsoStream.GetInt(num_sublayers - 1)];
		for (int i=num_sublayers - 2; i >= 0; i--)
		{
			boxSize += stream.ReadBit(boxSize, readSize,  out this.ptl_sublayer_level_present_flag[i], "ptl_sublayer_level_present_flag"); 
		}

		this.ptl_reserved_zero_bit = new bool[IsoStream.GetInt(9)];
		for (int j=num_sublayers; j<=8 && num_sublayers > 1; j++)
		{
			boxSize += stream.ReadBit(boxSize, readSize,  out this.ptl_reserved_zero_bit[j], "ptl_reserved_zero_bit"); 
		}

		this.sublayer_level_idc = new byte[IsoStream.GetInt(num_sublayers - 1)];
		for (int i=num_sublayers-2; i >= 0; i--)
		{

			if (ptl_sublayer_level_present_flag[i])
			{
				boxSize += stream.ReadUInt8(boxSize, readSize,  out this.sublayer_level_idc[i], "sublayer_level_idc"); 
			}
		}
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.ptl_num_sub_profiles, "ptl_num_sub_profiles"); 

		this.general_sub_profile_idc = new uint[IsoStream.GetInt( ptl_num_sub_profiles)];
		for (int j=0; j < ptl_num_sub_profiles; j++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.general_sub_profile_idc[j], "general_sub_profile_idc"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBits(2,  this.reserved, "reserved"); 
		boxSize += stream.WriteBits(6,  this.num_bytes_constraint_info, "num_bytes_constraint_info"); 
		boxSize += stream.WriteBits(7,  this.general_profile_idc, "general_profile_idc"); 
		boxSize += stream.WriteBit( this.general_tier_flag, "general_tier_flag"); 
		boxSize += stream.WriteUInt8( this.general_level_idc, "general_level_idc"); 
		boxSize += stream.WriteBit( this.ptl_frame_only_constraint_flag, "ptl_frame_only_constraint_flag"); 
		boxSize += stream.WriteBit( this.ptl_multi_layer_enabled_flag, "ptl_multi_layer_enabled_flag"); 
		boxSize += stream.WriteBits((uint)(8*num_bytes_constraint_info - 2 ),  this.general_constraint_info, "general_constraint_info"); 

		for (int i=num_sublayers - 2; i >= 0; i--)
		{
			boxSize += stream.WriteBit( this.ptl_sublayer_level_present_flag[i], "ptl_sublayer_level_present_flag"); 
		}

		for (int j=num_sublayers; j<=8 && num_sublayers > 1; j++)
		{
			boxSize += stream.WriteBit( this.ptl_reserved_zero_bit[j], "ptl_reserved_zero_bit"); 
		}

		for (int i=num_sublayers-2; i >= 0; i--)
		{

			if (ptl_sublayer_level_present_flag[i])
			{
				boxSize += stream.WriteUInt8( this.sublayer_level_idc[i], "sublayer_level_idc"); 
			}
		}
		boxSize += stream.WriteUInt8( this.ptl_num_sub_profiles, "ptl_num_sub_profiles"); 

		for (int j=0; j < ptl_num_sub_profiles; j++)
		{
			boxSize += stream.WriteUInt32( this.general_sub_profile_idc[j], "general_sub_profile_idc"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 2; // reserved
		boxSize += 6; // num_bytes_constraint_info
		boxSize += 7; // general_profile_idc
		boxSize += 1; // general_tier_flag
		boxSize += 8; // general_level_idc
		boxSize += 1; // ptl_frame_only_constraint_flag
		boxSize += 1; // ptl_multi_layer_enabled_flag
		boxSize += (ulong)(8*num_bytes_constraint_info - 2 ); // general_constraint_info

		for (int i=num_sublayers - 2; i >= 0; i--)
		{
			boxSize += 1; // ptl_sublayer_level_present_flag
		}

		for (int j=num_sublayers; j<=8 && num_sublayers > 1; j++)
		{
			boxSize += 1; // ptl_reserved_zero_bit
		}

		for (int i=num_sublayers-2; i >= 0; i--)
		{

			if (ptl_sublayer_level_present_flag[i])
			{
				boxSize += 8; // sublayer_level_idc
			}
		}
		boxSize += 8; // ptl_num_sub_profiles

		for (int j=0; j < ptl_num_sub_profiles; j++)
		{
			boxSize += 32; // general_sub_profile_idc
		}
		return boxSize;
	}
}

}
