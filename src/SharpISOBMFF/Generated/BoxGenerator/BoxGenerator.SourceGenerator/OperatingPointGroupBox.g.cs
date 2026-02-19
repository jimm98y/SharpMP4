using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class OperatingPointGroupBox extends EntityToGroupBox('opeg',0,0)
{
	unsigned int(8) num_profile_tier_level_minus1;
	for (i=0; i<=num_profile_tier_level_minus1; i++)
		VvcPTLRecord(0) opeg_ptl[i];
	bit(6) reserved = 0;
	unsigned int(1) incomplete_operating_points_flag;
	unsigned int(9) num_olss;
	for (i=0; i<num_olss; i++) {
		unsigned int(8) ptl_idx[i];
		unsigned int(9) ols_idx[i];
		unsigned int(6) layer_count[i];
		bit(1) reserved = 0;
		unsigned int(1) layer_info_present_flag[i];
		if (layer_info_present_flag[i]) {
			for (j=0; j<layer_count[i]; j++) {
				unsigned int(6) layer_id[i][j];
				unsigned int(1) is_output_layer[i][j];
				bit(1) reserved = 0;
			}
		}
	}
	bit(4) reserved = 0;
	unsigned int(12) num_operating_points;
	for (i=0; i<num_operating_points; i++) {
		unsigned int(9) ols_loop_entry_idx;
		unsigned int(3) max_temporal_id;
		unsigned int(1) frame_rate_info_flag;
		unsigned int(1) bit_rate_info_flag;
		if (incomplete_operating_points_flag) {
			unsigned int(2) op_availability_idc;
		}
		else
			bit(2) reserved = 0;
		bit(3) reserved = 0;
		unsigned int(2) chroma_format_idc;
		unsigned int(3) bit_depth_minus8;
		unsigned_int(16) max_picture_width;
		unsigned_int(16) max_picture_height;
		if (frame_rate_info_flag) {
			unsigned int(16) avg_frame_rate;
			bit(6) reserved = 0;
			unsigned int(2) constant_frame_rate;
		}
		if (bit_rate_info_flag) {
			unsigned int(32) max_bit_rate;
			unsigned int(32) avg_bit_rate;
		}
		unsigned int(8) entity_count;
		for (j=0; j<entity_count; j++)
			unsigned int(8) entity_idx;
	}
}
*/
public partial class OperatingPointGroupBox : EntityToGroupBox
{
	public const string TYPE = "opeg";
	public override string DisplayName { get { return "OperatingPointGroupBox"; } }

	protected byte num_profile_tier_level_minus1; 
	public byte NumProfileTierLevelMinus1 { get { return this.num_profile_tier_level_minus1; } set { this.num_profile_tier_level_minus1 = value; } }

	protected VvcPTLRecord[] opeg_ptl; 
	public VvcPTLRecord[] OpegPtl { get { return this.opeg_ptl; } set { this.opeg_ptl = value; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected bool incomplete_operating_points_flag; 
	public bool IncompleteOperatingPointsFlag { get { return this.incomplete_operating_points_flag; } set { this.incomplete_operating_points_flag = value; } }

	protected ushort num_olss; 
	public ushort NumOlss { get { return this.num_olss; } set { this.num_olss = value; } }

	protected byte[] ptl_idx; 
	public byte[] PtlIdx { get { return this.ptl_idx; } set { this.ptl_idx = value; } }

	protected ushort[] ols_idx; 
	public ushort[] OlsIdx { get { return this.ols_idx; } set { this.ols_idx = value; } }

	protected byte[] layer_count; 
	public byte[] LayerCount { get { return this.layer_count; } set { this.layer_count = value; } }

	protected bool[] reserved0; 
	public bool[] Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected bool[] layer_info_present_flag; 
	public bool[] LayerInfoPresentFlag { get { return this.layer_info_present_flag; } set { this.layer_info_present_flag = value; } }

	protected byte[][] layer_id; 
	public byte[][] LayerId { get { return this.layer_id; } set { this.layer_id = value; } }

	protected bool[][] is_output_layer; 
	public bool[][] IsOutputLayer { get { return this.is_output_layer; } set { this.is_output_layer = value; } }

	protected bool[][] reserved00; 
	public bool[][] Reserved00 { get { return this.reserved00; } set { this.reserved00 = value; } }

	protected byte reserved1 = 0; 
	public byte Reserved1 { get { return this.reserved1; } set { this.reserved1 = value; } }

	protected ushort num_operating_points; 
	public ushort NumOperatingPoints { get { return this.num_operating_points; } set { this.num_operating_points = value; } }

	protected ushort[] ols_loop_entry_idx; 
	public ushort[] OlsLoopEntryIdx { get { return this.ols_loop_entry_idx; } set { this.ols_loop_entry_idx = value; } }

	protected byte[] max_temporal_id; 
	public byte[] MaxTemporalId { get { return this.max_temporal_id; } set { this.max_temporal_id = value; } }

	protected bool[] frame_rate_info_flag; 
	public bool[] FrameRateInfoFlag { get { return this.frame_rate_info_flag; } set { this.frame_rate_info_flag = value; } }

	protected bool[] bit_rate_info_flag; 
	public bool[] BitRateInfoFlag { get { return this.bit_rate_info_flag; } set { this.bit_rate_info_flag = value; } }

	protected byte[] op_availability_idc; 
	public byte[] OpAvailabilityIdc { get { return this.op_availability_idc; } set { this.op_availability_idc = value; } }

	protected byte[] reserved2; 
	public byte[] Reserved2 { get { return this.reserved2; } set { this.reserved2 = value; } }

	protected byte[] reserved01; 
	public byte[] Reserved01 { get { return this.reserved01; } set { this.reserved01 = value; } }

	protected byte[] chroma_format_idc; 
	public byte[] ChromaFormatIdc { get { return this.chroma_format_idc; } set { this.chroma_format_idc = value; } }

	protected byte[] bit_depth_minus8; 
	public byte[] BitDepthMinus8 { get { return this.bit_depth_minus8; } set { this.bit_depth_minus8 = value; } }

	protected ushort[] max_picture_width; 
	public ushort[] MaxPictureWidth { get { return this.max_picture_width; } set { this.max_picture_width = value; } }

	protected ushort[] max_picture_height; 
	public ushort[] MaxPictureHeight { get { return this.max_picture_height; } set { this.max_picture_height = value; } }

	protected ushort[] avg_frame_rate; 
	public ushort[] AvgFrameRate { get { return this.avg_frame_rate; } set { this.avg_frame_rate = value; } }

	protected byte[] reserved10; 
	public byte[] Reserved10 { get { return this.reserved10; } set { this.reserved10 = value; } }

	protected byte[] constant_frame_rate; 
	public byte[] ConstantFrameRate { get { return this.constant_frame_rate; } set { this.constant_frame_rate = value; } }

	protected uint[] max_bit_rate; 
	public uint[] MaxBitRate { get { return this.max_bit_rate; } set { this.max_bit_rate = value; } }

	protected uint[] avg_bit_rate; 
	public uint[] AvgBitRate { get { return this.avg_bit_rate; } set { this.avg_bit_rate = value; } }

	protected byte[] entity_count; 
	public byte[] EntityCount { get { return this.entity_count; } set { this.entity_count = value; } }

	protected byte[][] entity_idx; 
	public byte[][] EntityIdx { get { return this.entity_idx; } set { this.entity_idx = value; } }

	public OperatingPointGroupBox(): base(IsoStream.FromFourCC("opeg"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.num_profile_tier_level_minus1, "num_profile_tier_level_minus1"); 

		this.opeg_ptl = new VvcPTLRecord[IsoStream.GetInt(num_profile_tier_level_minus1 + 1)];
		for (int i=0; i<=num_profile_tier_level_minus1; i++)
		{
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new VvcPTLRecord(0),  out this.opeg_ptl[i], "opeg_ptl"); 
		}
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.incomplete_operating_points_flag, "incomplete_operating_points_flag"); 
		boxSize += stream.ReadBits(boxSize, readSize, 9,  out this.num_olss, "num_olss"); 

		this.ptl_idx = new byte[IsoStream.GetInt(num_olss)];
		this.ols_idx = new ushort[IsoStream.GetInt(num_olss)];
		this.layer_count = new byte[IsoStream.GetInt(num_olss)];
		this.reserved0 = new bool[IsoStream.GetInt(num_olss)];
		this.layer_info_present_flag = new bool[IsoStream.GetInt(num_olss)];
		this.layer_id = new byte[IsoStream.GetInt(num_olss)][];
		this.is_output_layer = new bool[IsoStream.GetInt(num_olss)][];
		this.reserved00 = new bool[IsoStream.GetInt(num_olss)][];
		for (int i=0; i<num_olss; i++)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.ptl_idx[i], "ptl_idx"); 
			boxSize += stream.ReadBits(boxSize, readSize, 9,  out this.ols_idx[i], "ols_idx"); 
			boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.layer_count[i], "layer_count"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved0[i], "reserved0"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.layer_info_present_flag[i], "layer_info_present_flag"); 

			if (layer_info_present_flag[i])
			{

				this.layer_id[i] = new byte[IsoStream.GetInt(layer_count[i])];
				this.is_output_layer[i] = new bool[IsoStream.GetInt(layer_count[i])];
				this.reserved00[i] = new bool[IsoStream.GetInt(layer_count[i])];
				for (int j=0; j<layer_count[i]; j++)
				{
					boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.layer_id[i][j], "layer_id"); 
					boxSize += stream.ReadBit(boxSize, readSize,  out this.is_output_layer[i][j], "is_output_layer"); 
					boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved00[i][j], "reserved00"); 
				}
			}
		}
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.reserved1, "reserved1"); 
		boxSize += stream.ReadBits(boxSize, readSize, 12,  out this.num_operating_points, "num_operating_points"); 

		this.ols_loop_entry_idx = new ushort[IsoStream.GetInt(num_operating_points)];
		this.max_temporal_id = new byte[IsoStream.GetInt(num_operating_points)];
		this.frame_rate_info_flag = new bool[IsoStream.GetInt(num_operating_points)];
		this.bit_rate_info_flag = new bool[IsoStream.GetInt(num_operating_points)];
		this.op_availability_idc = new byte[IsoStream.GetInt(num_operating_points)];
		this.reserved2 = new byte[IsoStream.GetInt(num_operating_points)];
		this.reserved01 = new byte[IsoStream.GetInt(num_operating_points)];
		this.chroma_format_idc = new byte[IsoStream.GetInt(num_operating_points)];
		this.bit_depth_minus8 = new byte[IsoStream.GetInt(num_operating_points)];
		this.max_picture_width = new ushort[IsoStream.GetInt(num_operating_points)];
		this.max_picture_height = new ushort[IsoStream.GetInt(num_operating_points)];
		this.avg_frame_rate = new ushort[IsoStream.GetInt(num_operating_points)];
		this.reserved10 = new byte[IsoStream.GetInt(num_operating_points)];
		this.constant_frame_rate = new byte[IsoStream.GetInt(num_operating_points)];
		this.max_bit_rate = new uint[IsoStream.GetInt(num_operating_points)];
		this.avg_bit_rate = new uint[IsoStream.GetInt(num_operating_points)];
		this.entity_count = new byte[IsoStream.GetInt(num_operating_points)];
		this.entity_idx = new byte[IsoStream.GetInt(num_operating_points)][];
		for (int i=0; i<num_operating_points; i++)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 9,  out this.ols_loop_entry_idx[i], "ols_loop_entry_idx"); 
			boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.max_temporal_id[i], "max_temporal_id"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.frame_rate_info_flag[i], "frame_rate_info_flag"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.bit_rate_info_flag[i], "bit_rate_info_flag"); 

			if (incomplete_operating_points_flag)
			{
				boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.op_availability_idc[i], "op_availability_idc"); 
			}

			else 
			{
				boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.reserved2[i], "reserved2"); 
			}
			boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.reserved01[i], "reserved01"); 
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.chroma_format_idc[i], "chroma_format_idc"); 
			boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.bit_depth_minus8[i], "bit_depth_minus8"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.max_picture_width[i], "max_picture_width"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.max_picture_height[i], "max_picture_height"); 

			if (frame_rate_info_flag[i])
			{
				boxSize += stream.ReadUInt16(boxSize, readSize,  out this.avg_frame_rate[i], "avg_frame_rate"); 
				boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved10[i], "reserved10"); 
				boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.constant_frame_rate[i], "constant_frame_rate"); 
			}

			if (bit_rate_info_flag[i])
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.max_bit_rate[i], "max_bit_rate"); 
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.avg_bit_rate[i], "avg_bit_rate"); 
			}
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.entity_count[i], "entity_count"); 

			this.entity_idx[i] = new byte[IsoStream.GetInt(entity_count[i])];
			for (int j=0; j<entity_count[i]; j++)
			{
				boxSize += stream.ReadUInt8(boxSize, readSize,  out this.entity_idx[i][j], "entity_idx"); 
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.num_profile_tier_level_minus1, "num_profile_tier_level_minus1"); 

		for (int i=0; i<=num_profile_tier_level_minus1; i++)
		{
			boxSize += stream.WriteClass( this.opeg_ptl[i], "opeg_ptl"); 
		}
		boxSize += stream.WriteBits(6,  this.reserved, "reserved"); 
		boxSize += stream.WriteBit( this.incomplete_operating_points_flag, "incomplete_operating_points_flag"); 
		boxSize += stream.WriteBits(9,  this.num_olss, "num_olss"); 

		for (int i=0; i<num_olss; i++)
		{
			boxSize += stream.WriteUInt8( this.ptl_idx[i], "ptl_idx"); 
			boxSize += stream.WriteBits(9,  this.ols_idx[i], "ols_idx"); 
			boxSize += stream.WriteBits(6,  this.layer_count[i], "layer_count"); 
			boxSize += stream.WriteBit( this.reserved0[i], "reserved0"); 
			boxSize += stream.WriteBit( this.layer_info_present_flag[i], "layer_info_present_flag"); 

			if (layer_info_present_flag[i])
			{

				for (int j=0; j<layer_count[i]; j++)
				{
					boxSize += stream.WriteBits(6,  this.layer_id[i][j], "layer_id"); 
					boxSize += stream.WriteBit( this.is_output_layer[i][j], "is_output_layer"); 
					boxSize += stream.WriteBit( this.reserved00[i][j], "reserved00"); 
				}
			}
		}
		boxSize += stream.WriteBits(4,  this.reserved1, "reserved1"); 
		boxSize += stream.WriteBits(12,  this.num_operating_points, "num_operating_points"); 

		for (int i=0; i<num_operating_points; i++)
		{
			boxSize += stream.WriteBits(9,  this.ols_loop_entry_idx[i], "ols_loop_entry_idx"); 
			boxSize += stream.WriteBits(3,  this.max_temporal_id[i], "max_temporal_id"); 
			boxSize += stream.WriteBit( this.frame_rate_info_flag[i], "frame_rate_info_flag"); 
			boxSize += stream.WriteBit( this.bit_rate_info_flag[i], "bit_rate_info_flag"); 

			if (incomplete_operating_points_flag)
			{
				boxSize += stream.WriteBits(2,  this.op_availability_idc[i], "op_availability_idc"); 
			}

			else 
			{
				boxSize += stream.WriteBits(2,  this.reserved2[i], "reserved2"); 
			}
			boxSize += stream.WriteBits(3,  this.reserved01[i], "reserved01"); 
			boxSize += stream.WriteBits(2,  this.chroma_format_idc[i], "chroma_format_idc"); 
			boxSize += stream.WriteBits(3,  this.bit_depth_minus8[i], "bit_depth_minus8"); 
			boxSize += stream.WriteUInt16( this.max_picture_width[i], "max_picture_width"); 
			boxSize += stream.WriteUInt16( this.max_picture_height[i], "max_picture_height"); 

			if (frame_rate_info_flag[i])
			{
				boxSize += stream.WriteUInt16( this.avg_frame_rate[i], "avg_frame_rate"); 
				boxSize += stream.WriteBits(6,  this.reserved10[i], "reserved10"); 
				boxSize += stream.WriteBits(2,  this.constant_frame_rate[i], "constant_frame_rate"); 
			}

			if (bit_rate_info_flag[i])
			{
				boxSize += stream.WriteUInt32( this.max_bit_rate[i], "max_bit_rate"); 
				boxSize += stream.WriteUInt32( this.avg_bit_rate[i], "avg_bit_rate"); 
			}
			boxSize += stream.WriteUInt8( this.entity_count[i], "entity_count"); 

			for (int j=0; j<entity_count[i]; j++)
			{
				boxSize += stream.WriteUInt8( this.entity_idx[i][j], "entity_idx"); 
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // num_profile_tier_level_minus1

		for (int i=0; i<=num_profile_tier_level_minus1; i++)
		{
			boxSize += IsoStream.CalculateClassSize(opeg_ptl); // opeg_ptl
		}
		boxSize += 6; // reserved
		boxSize += 1; // incomplete_operating_points_flag
		boxSize += 9; // num_olss

		for (int i=0; i<num_olss; i++)
		{
			boxSize += 8; // ptl_idx
			boxSize += 9; // ols_idx
			boxSize += 6; // layer_count
			boxSize += 1; // reserved0
			boxSize += 1; // layer_info_present_flag

			if (layer_info_present_flag[i])
			{

				for (int j=0; j<layer_count[i]; j++)
				{
					boxSize += 6; // layer_id
					boxSize += 1; // is_output_layer
					boxSize += 1; // reserved00
				}
			}
		}
		boxSize += 4; // reserved1
		boxSize += 12; // num_operating_points

		for (int i=0; i<num_operating_points; i++)
		{
			boxSize += 9; // ols_loop_entry_idx
			boxSize += 3; // max_temporal_id
			boxSize += 1; // frame_rate_info_flag
			boxSize += 1; // bit_rate_info_flag

			if (incomplete_operating_points_flag)
			{
				boxSize += 2; // op_availability_idc
			}

			else 
			{
				boxSize += 2; // reserved2
			}
			boxSize += 3; // reserved01
			boxSize += 2; // chroma_format_idc
			boxSize += 3; // bit_depth_minus8
			boxSize += 16; // max_picture_width
			boxSize += 16; // max_picture_height

			if (frame_rate_info_flag[i])
			{
				boxSize += 16; // avg_frame_rate
				boxSize += 6; // reserved10
				boxSize += 2; // constant_frame_rate
			}

			if (bit_rate_info_flag[i])
			{
				boxSize += 32; // max_bit_rate
				boxSize += 32; // avg_bit_rate
			}
			boxSize += 8; // entity_count

			for (int j=0; j<entity_count[i]; j++)
			{
				boxSize += 8; // entity_idx
			}
		}
		return boxSize;
	}
}

}
