using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// L-HEVC's operating points (ISO/IEC 14496-15 9.6, not at hand: the edition at hand predates it), as GPAC reads them (isomedia/avc_ext.c, gf_isom_oinf_read_entry); scalability_mask & (1 << j) written as a remainder, which the generator's flags need
class OperatingPointsRecord { 
unsigned int(16) scalability_mask; 
bit(2) reserved = 0; 
unsigned int(6) num_profile_tier_level; 
for (i=0; i<num_profile_tier_level; i++) { 
 unsigned int(2) general_profile_space; 
 unsigned int(1) general_tier_flag; 
 unsigned int(5) general_profile_idc; 
 unsigned int(32) general_profile_compatibility_flags; 
 unsigned int(48) general_constraint_indicator_flags; 
 unsigned int(8) general_level_idc; 
} 
unsigned int(16) num_operating_points; 
for (i=0; i<num_operating_points; i++) { 
 unsigned int(16) output_layer_set_idx; 
 unsigned int(8) max_temporal_id; 
 unsigned int(8) layer_count; 
 for (j=0; j<layer_count; j++) { 
  unsigned int(8) ptl_idx; 
  unsigned int(6) layer_id; 
  unsigned int(1) is_outputlayer; 
  unsigned int(1) is_alternate_outputlayer; 
 } 
 unsigned int(16) minPicWidth; 
 unsigned int(16) minPicHeight; 
 unsigned int(16) maxPicWidth; 
 unsigned int(16) maxPicHeight; 
 unsigned int(2) maxChromaFormat; 
 unsigned int(3) maxBitDepthMinus8; 
 bit(1) reserved = 0; 
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
 unsigned int(8) dependent_layerID; 
 unsigned int(8) num_layers_dependent_on; 
 for (j=0; j<num_layers_dependent_on; j++) 
  unsigned int(8) dependent_on_layerID; 
 for (j=0; j<16; j++) { 
  if ((scalability_mask >> j) % 2 == 1) 
   unsigned int(8) dimension_identifier; 
 } 
} 
}

*/
public partial class OperatingPointsRecord : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "OperatingPointsRecord"; } }

	protected ushort scalability_mask; 
	public ushort ScalabilityMask { get { return this.scalability_mask; } set { this.scalability_mask = value; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte num_profile_tier_level; 
	public byte NumProfileTierLevel { get { return this.num_profile_tier_level; } set { this.num_profile_tier_level = value; } }

	protected byte[] general_profile_space; 
	public byte[] GeneralProfileSpace { get { return this.general_profile_space; } set { this.general_profile_space = value; } }

	protected bool[] general_tier_flag; 
	public bool[] GeneralTierFlag { get { return this.general_tier_flag; } set { this.general_tier_flag = value; } }

	protected byte[] general_profile_idc; 
	public byte[] GeneralProfileIdc { get { return this.general_profile_idc; } set { this.general_profile_idc = value; } }

	protected uint[] general_profile_compatibility_flags; 
	public uint[] GeneralProfileCompatibilityFlags { get { return this.general_profile_compatibility_flags; } set { this.general_profile_compatibility_flags = value; } }

	protected ulong[] general_constraint_indicator_flags; 
	public ulong[] GeneralConstraintIndicatorFlags { get { return this.general_constraint_indicator_flags; } set { this.general_constraint_indicator_flags = value; } }

	protected byte[] general_level_idc; 
	public byte[] GeneralLevelIdc { get { return this.general_level_idc; } set { this.general_level_idc = value; } }

	protected ushort num_operating_points; 
	public ushort NumOperatingPoints { get { return this.num_operating_points; } set { this.num_operating_points = value; } }

	protected ushort[] output_layer_set_idx; 
	public ushort[] OutputLayerSetIdx { get { return this.output_layer_set_idx; } set { this.output_layer_set_idx = value; } }

	protected byte[] max_temporal_id; 
	public byte[] MaxTemporalId { get { return this.max_temporal_id; } set { this.max_temporal_id = value; } }

	protected byte[] layer_count; 
	public byte[] LayerCount { get { return this.layer_count; } set { this.layer_count = value; } }

	protected byte[][] ptl_idx; 
	public byte[][] PtlIdx { get { return this.ptl_idx; } set { this.ptl_idx = value; } }

	protected byte[][] layer_id; 
	public byte[][] LayerId { get { return this.layer_id; } set { this.layer_id = value; } }

	protected bool[][] is_outputlayer; 
	public bool[][] IsOutputlayer { get { return this.is_outputlayer; } set { this.is_outputlayer = value; } }

	protected bool[][] is_alternate_outputlayer; 
	public bool[][] IsAlternateOutputlayer { get { return this.is_alternate_outputlayer; } set { this.is_alternate_outputlayer = value; } }

	protected ushort[] minPicWidth; 
	public ushort[] MinPicWidth { get { return this.minPicWidth; } set { this.minPicWidth = value; } }

	protected ushort[] minPicHeight; 
	public ushort[] MinPicHeight { get { return this.minPicHeight; } set { this.minPicHeight = value; } }

	protected ushort[] maxPicWidth; 
	public ushort[] MaxPicWidth { get { return this.maxPicWidth; } set { this.maxPicWidth = value; } }

	protected ushort[] maxPicHeight; 
	public ushort[] MaxPicHeight { get { return this.maxPicHeight; } set { this.maxPicHeight = value; } }

	protected byte[] maxChromaFormat; 
	public byte[] MaxChromaFormat { get { return this.maxChromaFormat; } set { this.maxChromaFormat = value; } }

	protected byte[] maxBitDepthMinus8; 
	public byte[] MaxBitDepthMinus8 { get { return this.maxBitDepthMinus8; } set { this.maxBitDepthMinus8 = value; } }

	protected bool[] reserved0; 
	public bool[] Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected bool[] frame_rate_info_flag; 
	public bool[] FrameRateInfoFlag { get { return this.frame_rate_info_flag; } set { this.frame_rate_info_flag = value; } }

	protected bool[] bit_rate_info_flag; 
	public bool[] BitRateInfoFlag { get { return this.bit_rate_info_flag; } set { this.bit_rate_info_flag = value; } }

	protected ushort[] avgFrameRate; 
	public ushort[] AvgFrameRate { get { return this.avgFrameRate; } set { this.avgFrameRate = value; } }

	protected byte[] reserved00; 
	public byte[] Reserved00 { get { return this.reserved00; } set { this.reserved00 = value; } }

	protected byte[] constantFrameRate; 
	public byte[] ConstantFrameRate { get { return this.constantFrameRate; } set { this.constantFrameRate = value; } }

	protected uint[] maxBitRate; 
	public uint[] MaxBitRate { get { return this.maxBitRate; } set { this.maxBitRate = value; } }

	protected uint[] avgBitRate; 
	public uint[] AvgBitRate { get { return this.avgBitRate; } set { this.avgBitRate = value; } }

	protected byte max_layer_count; 
	public byte MaxLayerCount { get { return this.max_layer_count; } set { this.max_layer_count = value; } }

	protected byte[] dependent_layerID; 
	public byte[] DependentLayerID { get { return this.dependent_layerID; } set { this.dependent_layerID = value; } }

	protected byte[] num_layers_dependent_on; 
	public byte[] NumLayersDependentOn { get { return this.num_layers_dependent_on; } set { this.num_layers_dependent_on = value; } }

	protected byte[][] dependent_on_layerID; 
	public byte[][] DependentOnLayerID { get { return this.dependent_on_layerID; } set { this.dependent_on_layerID = value; } }

	protected byte[][] dimension_identifier; 
	public byte[][] DimensionIdentifier { get { return this.dimension_identifier; } set { this.dimension_identifier = value; } }

	public OperatingPointsRecord(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.scalability_mask, "scalability_mask"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.num_profile_tier_level, "num_profile_tier_level"); 

		this.general_profile_space = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt(num_profile_tier_level), "general_profile_space");
		this.general_tier_flag = stream.SafeAllocate<bool>(boxSize, readSize, IsoStream.GetInt(num_profile_tier_level), "general_tier_flag");
		this.general_profile_idc = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt(num_profile_tier_level), "general_profile_idc");
		this.general_profile_compatibility_flags = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt(num_profile_tier_level), "general_profile_compatibility_flags");
		this.general_constraint_indicator_flags = stream.SafeAllocate<ulong>(boxSize, readSize, IsoStream.GetInt(num_profile_tier_level), "general_constraint_indicator_flags");
		this.general_level_idc = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt(num_profile_tier_level), "general_level_idc");
		for (int i=0; i<num_profile_tier_level; i++)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.general_profile_space[i], "general_profile_space"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.general_tier_flag[i], "general_tier_flag"); 
			boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.general_profile_idc[i], "general_profile_idc"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.general_profile_compatibility_flags[i], "general_profile_compatibility_flags"); 
			boxSize += stream.ReadUInt48(boxSize, readSize,  out this.general_constraint_indicator_flags[i], "general_constraint_indicator_flags"); 
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.general_level_idc[i], "general_level_idc"); 
		}
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.num_operating_points, "num_operating_points"); 

		this.output_layer_set_idx = stream.SafeAllocate<ushort>(boxSize, readSize, IsoStream.GetInt(num_operating_points), "output_layer_set_idx");
		this.max_temporal_id = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt(num_operating_points), "max_temporal_id");
		this.layer_count = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt(num_operating_points), "layer_count");
		this.ptl_idx = stream.SafeAllocate<byte[]>(boxSize, readSize, IsoStream.GetInt(num_operating_points), "ptl_idx");
		this.layer_id = stream.SafeAllocate<byte[]>(boxSize, readSize, IsoStream.GetInt(num_operating_points), "layer_id");
		this.is_outputlayer = stream.SafeAllocate<bool[]>(boxSize, readSize, IsoStream.GetInt(num_operating_points), "is_outputlayer");
		this.is_alternate_outputlayer = stream.SafeAllocate<bool[]>(boxSize, readSize, IsoStream.GetInt(num_operating_points), "is_alternate_outputlayer");
		this.minPicWidth = stream.SafeAllocate<ushort>(boxSize, readSize, IsoStream.GetInt(num_operating_points), "minPicWidth");
		this.minPicHeight = stream.SafeAllocate<ushort>(boxSize, readSize, IsoStream.GetInt(num_operating_points), "minPicHeight");
		this.maxPicWidth = stream.SafeAllocate<ushort>(boxSize, readSize, IsoStream.GetInt(num_operating_points), "maxPicWidth");
		this.maxPicHeight = stream.SafeAllocate<ushort>(boxSize, readSize, IsoStream.GetInt(num_operating_points), "maxPicHeight");
		this.maxChromaFormat = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt(num_operating_points), "maxChromaFormat");
		this.maxBitDepthMinus8 = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt(num_operating_points), "maxBitDepthMinus8");
		this.reserved0 = stream.SafeAllocate<bool>(boxSize, readSize, IsoStream.GetInt(num_operating_points), "reserved0");
		this.frame_rate_info_flag = stream.SafeAllocate<bool>(boxSize, readSize, IsoStream.GetInt(num_operating_points), "frame_rate_info_flag");
		this.bit_rate_info_flag = stream.SafeAllocate<bool>(boxSize, readSize, IsoStream.GetInt(num_operating_points), "bit_rate_info_flag");
		this.avgFrameRate = stream.SafeAllocate<ushort>(boxSize, readSize, IsoStream.GetInt(num_operating_points), "avgFrameRate");
		this.reserved00 = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt(num_operating_points), "reserved00");
		this.constantFrameRate = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt(num_operating_points), "constantFrameRate");
		this.maxBitRate = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt(num_operating_points), "maxBitRate");
		this.avgBitRate = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt(num_operating_points), "avgBitRate");
		for (int i=0; i<num_operating_points; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.output_layer_set_idx[i], "output_layer_set_idx"); 
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.max_temporal_id[i], "max_temporal_id"); 
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.layer_count[i], "layer_count"); 

			this.ptl_idx[i] = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt(layer_count[i]), "ptl_idx[i]");
			this.layer_id[i] = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt(layer_count[i]), "layer_id[i]");
			this.is_outputlayer[i] = stream.SafeAllocate<bool>(boxSize, readSize, IsoStream.GetInt(layer_count[i]), "is_outputlayer[i]");
			this.is_alternate_outputlayer[i] = stream.SafeAllocate<bool>(boxSize, readSize, IsoStream.GetInt(layer_count[i]), "is_alternate_outputlayer[i]");
			for (int j=0; j<layer_count[i]; j++)
			{
				boxSize += stream.ReadUInt8(boxSize, readSize,  out this.ptl_idx[i][j], "ptl_idx"); 
				boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.layer_id[i][j], "layer_id"); 
				boxSize += stream.ReadBit(boxSize, readSize,  out this.is_outputlayer[i][j], "is_outputlayer"); 
				boxSize += stream.ReadBit(boxSize, readSize,  out this.is_alternate_outputlayer[i][j], "is_alternate_outputlayer"); 
			}
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.minPicWidth[i], "minPicWidth"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.minPicHeight[i], "minPicHeight"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.maxPicWidth[i], "maxPicWidth"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.maxPicHeight[i], "maxPicHeight"); 
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.maxChromaFormat[i], "maxChromaFormat"); 
			boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.maxBitDepthMinus8[i], "maxBitDepthMinus8"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved0[i], "reserved0"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.frame_rate_info_flag[i], "frame_rate_info_flag"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.bit_rate_info_flag[i], "bit_rate_info_flag"); 

			if (frame_rate_info_flag[i])
			{
				boxSize += stream.ReadUInt16(boxSize, readSize,  out this.avgFrameRate[i], "avgFrameRate"); 
				boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved00[i], "reserved00"); 
				boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.constantFrameRate[i], "constantFrameRate"); 
			}

			if (bit_rate_info_flag[i])
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.maxBitRate[i], "maxBitRate"); 
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.avgBitRate[i], "avgBitRate"); 
			}
		}
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.max_layer_count, "max_layer_count"); 

		this.dependent_layerID = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt(max_layer_count), "dependent_layerID");
		this.num_layers_dependent_on = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt(max_layer_count), "num_layers_dependent_on");
		this.dependent_on_layerID = stream.SafeAllocate<byte[]>(boxSize, readSize, IsoStream.GetInt(max_layer_count), "dependent_on_layerID");
		this.dimension_identifier = stream.SafeAllocate<byte[]>(boxSize, readSize, IsoStream.GetInt(max_layer_count), "dimension_identifier");
		for (int i=0; i<max_layer_count; i++)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.dependent_layerID[i], "dependent_layerID"); 
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.num_layers_dependent_on[i], "num_layers_dependent_on"); 

			this.dependent_on_layerID[i] = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt(num_layers_dependent_on[i]), "dependent_on_layerID[i]");
			for (int j=0; j<num_layers_dependent_on[i]; j++)
			{
				boxSize += stream.ReadUInt8(boxSize, readSize,  out this.dependent_on_layerID[i][j], "dependent_on_layerID"); 
			}

			this.dimension_identifier[i] = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt(16), "dimension_identifier[i]");
			for (int j=0; j<16; j++)
			{

				if ((scalability_mask >> j) % 2 == 1)
				{
					boxSize += stream.ReadUInt8(boxSize, readSize,  out this.dimension_identifier[i][j], "dimension_identifier"); 
				}
			}
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt16( this.scalability_mask, "scalability_mask"); 
		boxSize += stream.WriteBits(2,  this.reserved, "reserved"); 
		boxSize += stream.WriteBits(6,  this.num_profile_tier_level, "num_profile_tier_level"); 

		for (int i=0; i<num_profile_tier_level; i++)
		{
			boxSize += stream.WriteBits(2,  this.general_profile_space[i], "general_profile_space"); 
			boxSize += stream.WriteBit( this.general_tier_flag[i], "general_tier_flag"); 
			boxSize += stream.WriteBits(5,  this.general_profile_idc[i], "general_profile_idc"); 
			boxSize += stream.WriteUInt32( this.general_profile_compatibility_flags[i], "general_profile_compatibility_flags"); 
			boxSize += stream.WriteUInt48( this.general_constraint_indicator_flags[i], "general_constraint_indicator_flags"); 
			boxSize += stream.WriteUInt8( this.general_level_idc[i], "general_level_idc"); 
		}
		boxSize += stream.WriteUInt16( this.num_operating_points, "num_operating_points"); 

		for (int i=0; i<num_operating_points; i++)
		{
			boxSize += stream.WriteUInt16( this.output_layer_set_idx[i], "output_layer_set_idx"); 
			boxSize += stream.WriteUInt8( this.max_temporal_id[i], "max_temporal_id"); 
			boxSize += stream.WriteUInt8( this.layer_count[i], "layer_count"); 

			for (int j=0; j<layer_count[i]; j++)
			{
				boxSize += stream.WriteUInt8( this.ptl_idx[i][j], "ptl_idx"); 
				boxSize += stream.WriteBits(6,  this.layer_id[i][j], "layer_id"); 
				boxSize += stream.WriteBit( this.is_outputlayer[i][j], "is_outputlayer"); 
				boxSize += stream.WriteBit( this.is_alternate_outputlayer[i][j], "is_alternate_outputlayer"); 
			}
			boxSize += stream.WriteUInt16( this.minPicWidth[i], "minPicWidth"); 
			boxSize += stream.WriteUInt16( this.minPicHeight[i], "minPicHeight"); 
			boxSize += stream.WriteUInt16( this.maxPicWidth[i], "maxPicWidth"); 
			boxSize += stream.WriteUInt16( this.maxPicHeight[i], "maxPicHeight"); 
			boxSize += stream.WriteBits(2,  this.maxChromaFormat[i], "maxChromaFormat"); 
			boxSize += stream.WriteBits(3,  this.maxBitDepthMinus8[i], "maxBitDepthMinus8"); 
			boxSize += stream.WriteBit( this.reserved0[i], "reserved0"); 
			boxSize += stream.WriteBit( this.frame_rate_info_flag[i], "frame_rate_info_flag"); 
			boxSize += stream.WriteBit( this.bit_rate_info_flag[i], "bit_rate_info_flag"); 

			if (frame_rate_info_flag[i])
			{
				boxSize += stream.WriteUInt16( this.avgFrameRate[i], "avgFrameRate"); 
				boxSize += stream.WriteBits(6,  this.reserved00[i], "reserved00"); 
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
			boxSize += stream.WriteUInt8( this.dependent_layerID[i], "dependent_layerID"); 
			boxSize += stream.WriteUInt8( this.num_layers_dependent_on[i], "num_layers_dependent_on"); 

			for (int j=0; j<num_layers_dependent_on[i]; j++)
			{
				boxSize += stream.WriteUInt8( this.dependent_on_layerID[i][j], "dependent_on_layerID"); 
			}

			for (int j=0; j<16; j++)
			{

				if ((scalability_mask >> j) % 2 == 1)
				{
					boxSize += stream.WriteUInt8( this.dimension_identifier[i][j], "dimension_identifier"); 
				}
			}
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 16; // scalability_mask
		boxSize += 2; // reserved
		boxSize += 6; // num_profile_tier_level

		for (int i=0; i<num_profile_tier_level; i++)
		{
			boxSize += 2; // general_profile_space
			boxSize += 1; // general_tier_flag
			boxSize += 5; // general_profile_idc
			boxSize += 32; // general_profile_compatibility_flags
			boxSize += 48; // general_constraint_indicator_flags
			boxSize += 8; // general_level_idc
		}
		boxSize += 16; // num_operating_points

		for (int i=0; i<num_operating_points; i++)
		{
			boxSize += 16; // output_layer_set_idx
			boxSize += 8; // max_temporal_id
			boxSize += 8; // layer_count

			for (int j=0; j<layer_count[i]; j++)
			{
				boxSize += 8; // ptl_idx
				boxSize += 6; // layer_id
				boxSize += 1; // is_outputlayer
				boxSize += 1; // is_alternate_outputlayer
			}
			boxSize += 16; // minPicWidth
			boxSize += 16; // minPicHeight
			boxSize += 16; // maxPicWidth
			boxSize += 16; // maxPicHeight
			boxSize += 2; // maxChromaFormat
			boxSize += 3; // maxBitDepthMinus8
			boxSize += 1; // reserved0
			boxSize += 1; // frame_rate_info_flag
			boxSize += 1; // bit_rate_info_flag

			if (frame_rate_info_flag[i])
			{
				boxSize += 16; // avgFrameRate
				boxSize += 6; // reserved00
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
			boxSize += 8; // dependent_layerID
			boxSize += 8; // num_layers_dependent_on

			for (int j=0; j<num_layers_dependent_on[i]; j++)
			{
				boxSize += 8; // dependent_on_layerID
			}

			for (int j=0; j<16; j++)
			{

				if ((scalability_mask >> j) % 2 == 1)
				{
					boxSize += 8; // dimension_identifier
				}
			}
		}
		return boxSize;
	}
}

}
