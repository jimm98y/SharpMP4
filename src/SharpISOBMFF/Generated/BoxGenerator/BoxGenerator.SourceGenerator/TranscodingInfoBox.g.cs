using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class TranscodingInfoBox extends Box('tran'){
	bit(4) reserved = 0;
	unsigned int(2) conversion_idc;
	unsigned int(1) cavlc_info_present_flag;
	unsigned int(1) cabac_info_present_flag;
	if(cavlc_info_present_flag){
		unsigned int(24) cavlc_profile_level_idc;
		unsigned int(32) cavlc_max_bitrate;
		unsigned int(32) cavlc_avg_bitrate;
	}
	if(cabac_info_present_flag){
		unsigned int(24) cabac_profile_level_idc;
		unsigned int(32) cabac_max_bitrate;
		unsigned int(32) cabac_avg_bitrate;
	}
}
*/
public partial class TranscodingInfoBox : Box
{
	public const string TYPE = "tran";
	public override string DisplayName { get { return "TranscodingInfoBox"; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte conversion_idc; 
	public byte ConversionIdc { get { return this.conversion_idc; } set { this.conversion_idc = value; } }

	protected bool cavlc_info_present_flag; 
	public bool CavlcInfoPresentFlag { get { return this.cavlc_info_present_flag; } set { this.cavlc_info_present_flag = value; } }

	protected bool cabac_info_present_flag; 
	public bool CabacInfoPresentFlag { get { return this.cabac_info_present_flag; } set { this.cabac_info_present_flag = value; } }

	protected uint cavlc_profile_level_idc; 
	public uint CavlcProfileLevelIdc { get { return this.cavlc_profile_level_idc; } set { this.cavlc_profile_level_idc = value; } }

	protected uint cavlc_max_bitrate; 
	public uint CavlcMaxBitrate { get { return this.cavlc_max_bitrate; } set { this.cavlc_max_bitrate = value; } }

	protected uint cavlc_avg_bitrate; 
	public uint CavlcAvgBitrate { get { return this.cavlc_avg_bitrate; } set { this.cavlc_avg_bitrate = value; } }

	protected uint cabac_profile_level_idc; 
	public uint CabacProfileLevelIdc { get { return this.cabac_profile_level_idc; } set { this.cabac_profile_level_idc = value; } }

	protected uint cabac_max_bitrate; 
	public uint CabacMaxBitrate { get { return this.cabac_max_bitrate; } set { this.cabac_max_bitrate = value; } }

	protected uint cabac_avg_bitrate; 
	public uint CabacAvgBitrate { get { return this.cabac_avg_bitrate; } set { this.cabac_avg_bitrate = value; } }

	public TranscodingInfoBox(): base(IsoStream.FromFourCC("tran"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.conversion_idc, "conversion_idc"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.cavlc_info_present_flag, "cavlc_info_present_flag"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.cabac_info_present_flag, "cabac_info_present_flag"); 

		if (cavlc_info_present_flag)
		{
			boxSize += stream.ReadUInt24(boxSize, readSize,  out this.cavlc_profile_level_idc, "cavlc_profile_level_idc"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.cavlc_max_bitrate, "cavlc_max_bitrate"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.cavlc_avg_bitrate, "cavlc_avg_bitrate"); 
		}

		if (cabac_info_present_flag)
		{
			boxSize += stream.ReadUInt24(boxSize, readSize,  out this.cabac_profile_level_idc, "cabac_profile_level_idc"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.cabac_max_bitrate, "cabac_max_bitrate"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.cabac_avg_bitrate, "cabac_avg_bitrate"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(4,  this.reserved, "reserved"); 
		boxSize += stream.WriteBits(2,  this.conversion_idc, "conversion_idc"); 
		boxSize += stream.WriteBit( this.cavlc_info_present_flag, "cavlc_info_present_flag"); 
		boxSize += stream.WriteBit( this.cabac_info_present_flag, "cabac_info_present_flag"); 

		if (cavlc_info_present_flag)
		{
			boxSize += stream.WriteUInt24( this.cavlc_profile_level_idc, "cavlc_profile_level_idc"); 
			boxSize += stream.WriteUInt32( this.cavlc_max_bitrate, "cavlc_max_bitrate"); 
			boxSize += stream.WriteUInt32( this.cavlc_avg_bitrate, "cavlc_avg_bitrate"); 
		}

		if (cabac_info_present_flag)
		{
			boxSize += stream.WriteUInt24( this.cabac_profile_level_idc, "cabac_profile_level_idc"); 
			boxSize += stream.WriteUInt32( this.cabac_max_bitrate, "cabac_max_bitrate"); 
			boxSize += stream.WriteUInt32( this.cabac_avg_bitrate, "cabac_avg_bitrate"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 4; // reserved
		boxSize += 2; // conversion_idc
		boxSize += 1; // cavlc_info_present_flag
		boxSize += 1; // cabac_info_present_flag

		if (cavlc_info_present_flag)
		{
			boxSize += 24; // cavlc_profile_level_idc
			boxSize += 32; // cavlc_max_bitrate
			boxSize += 32; // cavlc_avg_bitrate
		}

		if (cabac_info_present_flag)
		{
			boxSize += 24; // cabac_profile_level_idc
			boxSize += 32; // cabac_max_bitrate
			boxSize += 32; // cabac_avg_bitrate
		}
		return boxSize;
	}
}

}
