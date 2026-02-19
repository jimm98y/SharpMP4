using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class VvcOperatingPointsRecord { 
unsigned int(8) num_profile_tier_level_minus1;
for (i=0; i<=num_profile_tier_level_minus1; i++) { 
 unsigned int(8) ptl_max_temporal_id[i]; 
 VvcPTLRecord(ptl_max_temporal_id[i]+1) ptl[i];
} 
unsigned int(1) all_independent_layers_flag; 
bit(7) reserved = 0; 
if (all_independent_layers_flag){ 
 unsigned int(1) each_layer_is_an_ols_flag; 
 bit(7) reserved = 0; 
} 
else 
 unsigned int(8) ols_mode_idc; 
unsigned int(16) num_operating_points; 
for (i=0; i<num_operating_points; i++) { 
 unsigned int(16) output_layer_set_idx; 
 unsigned int(8) ptl_idx; 
 unsigned int(8) max_temporal_id; 
 unsigned int(8) layer_count; 
 for (j=0; j<layer_count; j++) { 
  unsigned int(6) layer_id; 
  unsigned int(1) is_outputlayer;
  bit(1) reserved = 0;
 }
 bit(6) reserved = 0; 
 unsigned int(1) frame_rate_info_flag;
 unsigned int(1) bit_rate_info_flag;
 if (frame_rate_info_flag) { 
  unsigned int(16) avgFrameRate; 
  bit(6) reserved = 0; 
  unsigned int(2) constantFrameRate;
 } 
 if (bit_rate_info_flag) { 
  unsigned int(32) maxBitRate; 
  unsigned int(32) avgBitRate;
 }
}
unsigned int(8) max_layer_count; 
for (i=0; i<max_layer_count; i++) { 
 unsigned int(8) layerID; 
 unsigned int(8) num_direct_ref_layers; 
 for (j=0; j<num_direct_ref_layers; j++) 
  unsigned int(8) direct_ref_layerID; 
 unsigned int(8) max_tid_il_ref_pics_plus1; 
 }
}

*/
public partial class VvcOperatingPointsRecord : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "VvcOperatingPointsRecord"; } }

	protected byte num_profile_tier_level_minus1; 
	public byte NumProfileTierLevelMinus1 { get { return this.num_profile_tier_level_minus1; } set { this.num_profile_tier_level_minus1 = value; } }

	protected byte[] ptl_max_temporal_id; 
	public byte[] PtlMaxTemporalId { get { return this.ptl_max_temporal_id; } set { this.ptl_max_temporal_id = value; } }

	protected VvcPTLRecord[] ptl; 
	public VvcPTLRecord[] Ptl { get { return this.ptl; } set { this.ptl = value; } }

	protected bool all_independent_layers_flag; 
	public bool AllIndependentLayersFlag { get { return this.all_independent_layers_flag; } set { this.all_independent_layers_flag = value; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected bool each_layer_is_an_ols_flag; 
	public bool EachLayerIsAnOlsFlag { get { return this.each_layer_is_an_ols_flag; } set { this.each_layer_is_an_ols_flag = value; } }

	protected byte reserved0 = 0; 
	public byte Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected byte ols_mode_idc; 
	public byte OlsModeIdc { get { return this.ols_mode_idc; } set { this.ols_mode_idc = value; } }

	protected ushort num_operating_points; 
	public ushort NumOperatingPoints { get { return this.num_operating_points; } set { this.num_operating_points = value; } }

	protected ushort[] output_layer_set_idx; 
	public ushort[] OutputLayerSetIdx { get { return this.output_layer_set_idx; } set { this.output_layer_set_idx = value; } }

	protected byte[] ptl_idx; 
	public byte[] PtlIdx { get { return this.ptl_idx; } set { this.ptl_idx = value; } }

	protected byte[] max_temporal_id; 
	public byte[] MaxTemporalId { get { return this.max_temporal_id; } set { this.max_temporal_id = value; } }

	protected byte[] layer_count; 
	public byte[] LayerCount { get { return this.layer_count; } set { this.layer_count = value; } }

	protected byte[][] layer_id; 
	public byte[][] LayerId { get { return this.layer_id; } set { this.layer_id = value; } }

	protected bool[][] is_outputlayer; 
	public bool[][] IsOutputlayer { get { return this.is_outputlayer; } set { this.is_outputlayer = value; } }

	protected bool[][] reserved1; 
	public bool[][] Reserved1 { get { return this.reserved1; } set { this.reserved1 = value; } }

	protected byte[] reserved00; 
	public byte[] Reserved00 { get { return this.reserved00; } set { this.reserved00 = value; } }

	protected bool[] frame_rate_info_flag; 
	public bool[] FrameRateInfoFlag { get { return this.frame_rate_info_flag; } set { this.frame_rate_info_flag = value; } }

	protected bool[] bit_rate_info_flag; 
	public bool[] BitRateInfoFlag { get { return this.bit_rate_info_flag; } set { this.bit_rate_info_flag = value; } }

	protected ushort[] avgFrameRate; 
	public ushort[] AvgFrameRate { get { return this.avgFrameRate; } set { this.avgFrameRate = value; } }

	protected byte[] reserved10; 
	public byte[] Reserved10 { get { return this.reserved10; } set { this.reserved10 = value; } }

	protected byte[] constantFrameRate; 
	public byte[] ConstantFrameRate { get { return this.constantFrameRate; } set { this.constantFrameRate = value; } }

	protected uint[] maxBitRate; 
	public uint[] MaxBitRate { get { return this.maxBitRate; } set { this.maxBitRate = value; } }

	protected uint[] avgBitRate; 
	public uint[] AvgBitRate { get { return this.avgBitRate; } set { this.avgBitRate = value; } }

	protected byte max_layer_count; 
	public byte MaxLayerCount { get { return this.max_layer_count; } set { this.max_layer_count = value; } }

	protected byte[] layerID; 
	public byte[] LayerID { get { return this.layerID; } set { this.layerID = value; } }

	protected byte[] num_direct_ref_layers; 
	public byte[] NumDirectRefLayers { get { return this.num_direct_ref_layers; } set { this.num_direct_ref_layers = value; } }

	protected byte[][] direct_ref_layerID; 
	public byte[][] DirectRefLayerID { get { return this.direct_ref_layerID; } set { this.direct_ref_layerID = value; } }

	protected byte[] max_tid_il_ref_pics_plus1; 
	public byte[] MaxTidIlRefPicsPlus1 { get { return this.max_tid_il_ref_pics_plus1; } set { this.max_tid_il_ref_pics_plus1 = value; } }

	public VvcOperatingPointsRecord(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.num_profile_tier_level_minus1, "num_profile_tier_level_minus1"); 

		this.ptl_max_temporal_id = new byte[IsoStream.GetInt(num_profile_tier_level_minus1 + 1)];
		this.ptl = new VvcPTLRecord[IsoStream.GetInt(num_profile_tier_level_minus1 + 1)];
		for (int i=0; i<=num_profile_tier_level_minus1; i++)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.ptl_max_temporal_id[i], "ptl_max_temporal_id"); 
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new VvcPTLRecord((byte)(ptl_max_temporal_id[i]+1)),  out this.ptl[i], "ptl"); 
		}
		boxSize += stream.ReadBit(boxSize, readSize,  out this.all_independent_layers_flag, "all_independent_layers_flag"); 
		boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.reserved, "reserved"); 

		if (all_independent_layers_flag)
		{
			boxSize += stream.ReadBit(boxSize, readSize,  out this.each_layer_is_an_ols_flag, "each_layer_is_an_ols_flag"); 
			boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.reserved0, "reserved0"); 
		}

		else 
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.ols_mode_idc, "ols_mode_idc"); 
		}
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.num_operating_points, "num_operating_points"); 

		this.output_layer_set_idx = new ushort[IsoStream.GetInt(num_operating_points)];
		this.ptl_idx = new byte[IsoStream.GetInt(num_operating_points)];
		this.max_temporal_id = new byte[IsoStream.GetInt(num_operating_points)];
		this.layer_count = new byte[IsoStream.GetInt(num_operating_points)];
		this.layer_id = new byte[IsoStream.GetInt(num_operating_points)][];
		this.is_outputlayer = new bool[IsoStream.GetInt(num_operating_points)][];
		this.reserved1 = new bool[IsoStream.GetInt(num_operating_points)][];
		this.reserved00 = new byte[IsoStream.GetInt(num_operating_points)];
		this.frame_rate_info_flag = new bool[IsoStream.GetInt(num_operating_points)];
		this.bit_rate_info_flag = new bool[IsoStream.GetInt(num_operating_points)];
		this.avgFrameRate = new ushort[IsoStream.GetInt(num_operating_points)];
		this.reserved10 = new byte[IsoStream.GetInt(num_operating_points)];
		this.constantFrameRate = new byte[IsoStream.GetInt(num_operating_points)];
		this.maxBitRate = new uint[IsoStream.GetInt(num_operating_points)];
		this.avgBitRate = new uint[IsoStream.GetInt(num_operating_points)];
		for (int i=0; i<num_operating_points; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.output_layer_set_idx[i], "output_layer_set_idx"); 
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.ptl_idx[i], "ptl_idx"); 
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.max_temporal_id[i], "max_temporal_id"); 
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.layer_count[i], "layer_count"); 

			this.layer_id[i] = new byte[IsoStream.GetInt(layer_count[i])];
			this.is_outputlayer[i] = new bool[IsoStream.GetInt(layer_count[i])];
			this.reserved1[i] = new bool[IsoStream.GetInt(layer_count[i])];
			for (int j=0; j<layer_count[i]; j++)
			{
				boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.layer_id[i][j], "layer_id"); 
				boxSize += stream.ReadBit(boxSize, readSize,  out this.is_outputlayer[i][j], "is_outputlayer"); 
				boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved1[i][j], "reserved1"); 
			}
			boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved00[i], "reserved00"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.frame_rate_info_flag[i], "frame_rate_info_flag"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.bit_rate_info_flag[i], "bit_rate_info_flag"); 

			if (frame_rate_info_flag[i])
			{
				boxSize += stream.ReadUInt16(boxSize, readSize,  out this.avgFrameRate[i], "avgFrameRate"); 
				boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved10[i], "reserved10"); 
				boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.constantFrameRate[i], "constantFrameRate"); 
			}

			if (bit_rate_info_flag[i])
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.maxBitRate[i], "maxBitRate"); 
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.avgBitRate[i], "avgBitRate"); 
			}
		}
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.max_layer_count, "max_layer_count"); 

		this.layerID = new byte[IsoStream.GetInt(max_layer_count)];
		this.num_direct_ref_layers = new byte[IsoStream.GetInt(max_layer_count)];
		this.direct_ref_layerID = new byte[IsoStream.GetInt(max_layer_count)][];
		this.max_tid_il_ref_pics_plus1 = new byte[IsoStream.GetInt(max_layer_count)];
		for (int i=0; i<max_layer_count; i++)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.layerID[i], "layerID"); 
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.num_direct_ref_layers[i], "num_direct_ref_layers"); 

			this.direct_ref_layerID[i] = new byte[IsoStream.GetInt(num_direct_ref_layers[i])];
			for (int j=0; j<num_direct_ref_layers[i]; j++)
			{
				boxSize += stream.ReadUInt8(boxSize, readSize,  out this.direct_ref_layerID[i][j], "direct_ref_layerID"); 
			}
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.max_tid_il_ref_pics_plus1[i], "max_tid_il_ref_pics_plus1"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt8( this.num_profile_tier_level_minus1, "num_profile_tier_level_minus1"); 

		for (int i=0; i<=num_profile_tier_level_minus1; i++)
		{
			boxSize += stream.WriteUInt8( this.ptl_max_temporal_id[i], "ptl_max_temporal_id"); 
			boxSize += stream.WriteClass( this.ptl[i], "ptl"); 
		}
		boxSize += stream.WriteBit( this.all_independent_layers_flag, "all_independent_layers_flag"); 
		boxSize += stream.WriteBits(7,  this.reserved, "reserved"); 

		if (all_independent_layers_flag)
		{
			boxSize += stream.WriteBit( this.each_layer_is_an_ols_flag, "each_layer_is_an_ols_flag"); 
			boxSize += stream.WriteBits(7,  this.reserved0, "reserved0"); 
		}

		else 
		{
			boxSize += stream.WriteUInt8( this.ols_mode_idc, "ols_mode_idc"); 
		}
		boxSize += stream.WriteUInt16( this.num_operating_points, "num_operating_points"); 

		for (int i=0; i<num_operating_points; i++)
		{
			boxSize += stream.WriteUInt16( this.output_layer_set_idx[i], "output_layer_set_idx"); 
			boxSize += stream.WriteUInt8( this.ptl_idx[i], "ptl_idx"); 
			boxSize += stream.WriteUInt8( this.max_temporal_id[i], "max_temporal_id"); 
			boxSize += stream.WriteUInt8( this.layer_count[i], "layer_count"); 

			for (int j=0; j<layer_count[i]; j++)
			{
				boxSize += stream.WriteBits(6,  this.layer_id[i][j], "layer_id"); 
				boxSize += stream.WriteBit( this.is_outputlayer[i][j], "is_outputlayer"); 
				boxSize += stream.WriteBit( this.reserved1[i][j], "reserved1"); 
			}
			boxSize += stream.WriteBits(6,  this.reserved00[i], "reserved00"); 
			boxSize += stream.WriteBit( this.frame_rate_info_flag[i], "frame_rate_info_flag"); 
			boxSize += stream.WriteBit( this.bit_rate_info_flag[i], "bit_rate_info_flag"); 

			if (frame_rate_info_flag[i])
			{
				boxSize += stream.WriteUInt16( this.avgFrameRate[i], "avgFrameRate"); 
				boxSize += stream.WriteBits(6,  this.reserved10[i], "reserved10"); 
				boxSize += stream.WriteBits(2,  this.constantFrameRate[i], "constantFrameRate"); 
			}

			if (bit_rate_info_flag[i])
			{
				boxSize += stream.WriteUInt32( this.maxBitRate[i], "maxBitRate"); 
				boxSize += stream.WriteUInt32( this.avgBitRate[i], "avgBitRate"); 
			}
		}
		boxSize += stream.WriteUInt8( this.max_layer_count, "max_layer_count"); 

		for (int i=0; i<max_layer_count; i++)
		{
			boxSize += stream.WriteUInt8( this.layerID[i], "layerID"); 
			boxSize += stream.WriteUInt8( this.num_direct_ref_layers[i], "num_direct_ref_layers"); 

			for (int j=0; j<num_direct_ref_layers[i]; j++)
			{
				boxSize += stream.WriteUInt8( this.direct_ref_layerID[i][j], "direct_ref_layerID"); 
			}
			boxSize += stream.WriteUInt8( this.max_tid_il_ref_pics_plus1[i], "max_tid_il_ref_pics_plus1"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 8; // num_profile_tier_level_minus1

		for (int i=0; i<=num_profile_tier_level_minus1; i++)
		{
			boxSize += 8; // ptl_max_temporal_id
			boxSize += IsoStream.CalculateClassSize(ptl); // ptl
		}
		boxSize += 1; // all_independent_layers_flag
		boxSize += 7; // reserved

		if (all_independent_layers_flag)
		{
			boxSize += 1; // each_layer_is_an_ols_flag
			boxSize += 7; // reserved0
		}

		else 
		{
			boxSize += 8; // ols_mode_idc
		}
		boxSize += 16; // num_operating_points

		for (int i=0; i<num_operating_points; i++)
		{
			boxSize += 16; // output_layer_set_idx
			boxSize += 8; // ptl_idx
			boxSize += 8; // max_temporal_id
			boxSize += 8; // layer_count

			for (int j=0; j<layer_count[i]; j++)
			{
				boxSize += 6; // layer_id
				boxSize += 1; // is_outputlayer
				boxSize += 1; // reserved1
			}
			boxSize += 6; // reserved00
			boxSize += 1; // frame_rate_info_flag
			boxSize += 1; // bit_rate_info_flag

			if (frame_rate_info_flag[i])
			{
				boxSize += 16; // avgFrameRate
				boxSize += 6; // reserved10
				boxSize += 2; // constantFrameRate
			}

			if (bit_rate_info_flag[i])
			{
				boxSize += 32; // maxBitRate
				boxSize += 32; // avgBitRate
			}
		}
		boxSize += 8; // max_layer_count

		for (int i=0; i<max_layer_count; i++)
		{
			boxSize += 8; // layerID
			boxSize += 8; // num_direct_ref_layers

			for (int j=0; j<num_direct_ref_layers[i]; j++)
			{
				boxSize += 8; // direct_ref_layerID
			}
			boxSize += 8; // max_tid_il_ref_pics_plus1
		}
		return boxSize;
	}
}

}
