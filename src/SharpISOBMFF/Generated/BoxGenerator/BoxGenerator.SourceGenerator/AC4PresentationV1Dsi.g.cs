using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// skip_area: what is left of pres_bytes (E.6.1), which the presentation ends with
aligned(8) class AC4PresentationV1Dsi(pres_bytes) {
 bit(5) presentation_config_v1;
 if (presentation_config_v1 != 0x06) {
  bit(3) md_compat;
  bit(1) b_presentation_id;
  if (b_presentation_id) {
   bit(5) presentation_id;
  }
  bit(2) dsi_frame_rate_multiply_info;
  bit(2) dsi_frame_rate_fraction_info;
  bit(5) presentation_emdf_version;
  bit(10) presentation_key_id;
  bit(1) b_presentation_channel_coded;
  if (b_presentation_channel_coded) {
   bit(5) dsi_presentation_ch_mode;
   if (dsi_presentation_ch_mode >= 11 && dsi_presentation_ch_mode <= 14) {
    bit(1) pres_b_4_back_channels_present;
    bit(2) pres_top_channel_pairs;
   }
   bit(6) reserved_zero;
   bit(18) presentation_v1_channel_groups;
  }
  bit(1) b_presentation_core_differs;
  if (b_presentation_core_differs) {
   bit(1) b_presentation_core_channel_coded;
   if (b_presentation_core_channel_coded) {
    bit(2) dsi_presentation_channel_mode_core;
   }
  }
  bit(1) b_presentation_filter;
  if (b_presentation_filter) {
   bit(1) b_enable_presentation;
   bit(8) n_filter_bytes;
   bit(8) filter_data[n_filter_bytes];
  }
  if (presentation_config_v1 == 0x1f) {
   AC4SubstreamGroupDsi substream_group;
  }
  else {
   bit(1) b_multi_pid;
   if (presentation_config_v1 <= 2) {
    for (i = 0; i < 2; i++) {
     AC4SubstreamGroupDsi substream_groups;
    }
   }
   if (presentation_config_v1 == 3 || presentation_config_v1 == 4) {
    for (i = 0; i < 3; i++) {
     AC4SubstreamGroupDsi substream_groups;
    }
   }
   if (presentation_config_v1 == 5) {
    bit(3) n_substream_groups_minus2;
    for (i = 0; i < n_substream_groups_minus2 + 2; i++) {
     AC4SubstreamGroupDsi substream_groups;
    }
   }
   if (presentation_config_v1 > 5) {
    bit(7) n_skip_bytes;
    bit(8) skip_data[n_skip_bytes];
   }
  }
  bit(1) b_pre_virtualized;
  bit(1) b_add_emdf_substreams;
 }
 if (presentation_config_v1 == 0x06 || b_add_emdf_substreams) {
  bit(7) n_add_emdf_substreams;
  for (j = 0; j < n_add_emdf_substreams; j++) {
   bit(5) substream_emdf_version;
   bit(10) substream_key_id;
  }
 }
 bit(1) b_presentation_bitrate_info;
 if (b_presentation_bitrate_info) {
  AC4BitrateDsi ac4_bitrate_dsi;
 }
 bit(1) b_alternative;
 if (b_alternative) {
  byte_alignment();
  AC4AlternativeInfo alternative_info;
 }
 byte_alignment();
 if (bits_read() <= (pres_bytes - 1) * 8) {
  bit(1) de_indicator;
  bit(1) immersive_audio_indicator;
  bit(4) reserved;
  bit(1) b_extended_presentation_id;
  if (b_extended_presentation_id) {
   bit(9) extended_presentation_id;
  }
  else {
   bit(1) reserved1;
  }
 }
 bit(8) skip_area[pres_bytes - bits_read() / 8];
}
// E.11.1

*/
public partial class AC4PresentationV1Dsi : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "AC4PresentationV1Dsi"; } }

	protected byte presentation_config_v1; 
	public byte PresentationConfigV1 { get { return this.presentation_config_v1; } set { this.presentation_config_v1 = value; } }

	protected byte md_compat; 
	public byte MdCompat { get { return this.md_compat; } set { this.md_compat = value; } }

	protected bool b_presentation_id; 
	public bool bPresentationId { get { return this.b_presentation_id; } set { this.b_presentation_id = value; } }

	protected byte presentation_id; 
	public byte PresentationId { get { return this.presentation_id; } set { this.presentation_id = value; } }

	protected byte dsi_frame_rate_multiply_info; 
	public byte DsiFrameRateMultiplyInfo { get { return this.dsi_frame_rate_multiply_info; } set { this.dsi_frame_rate_multiply_info = value; } }

	protected byte dsi_frame_rate_fraction_info; 
	public byte DsiFrameRateFractionInfo { get { return this.dsi_frame_rate_fraction_info; } set { this.dsi_frame_rate_fraction_info = value; } }

	protected byte presentation_emdf_version; 
	public byte PresentationEmdfVersion { get { return this.presentation_emdf_version; } set { this.presentation_emdf_version = value; } }

	protected ushort presentation_key_id; 
	public ushort PresentationKeyId { get { return this.presentation_key_id; } set { this.presentation_key_id = value; } }

	protected bool b_presentation_channel_coded; 
	public bool bPresentationChannelCoded { get { return this.b_presentation_channel_coded; } set { this.b_presentation_channel_coded = value; } }

	protected byte dsi_presentation_ch_mode; 
	public byte DsiPresentationChMode { get { return this.dsi_presentation_ch_mode; } set { this.dsi_presentation_ch_mode = value; } }

	protected bool pres_b_4_back_channels_present; 
	public bool Presb4BackChannelsPresent { get { return this.pres_b_4_back_channels_present; } set { this.pres_b_4_back_channels_present = value; } }

	protected byte pres_top_channel_pairs; 
	public byte PresTopChannelPairs { get { return this.pres_top_channel_pairs; } set { this.pres_top_channel_pairs = value; } }

	protected byte reserved_zero; 
	public byte ReservedZero { get { return this.reserved_zero; } set { this.reserved_zero = value; } }

	protected uint presentation_v1_channel_groups; 
	public uint PresentationV1ChannelGroups { get { return this.presentation_v1_channel_groups; } set { this.presentation_v1_channel_groups = value; } }

	protected bool b_presentation_core_differs; 
	public bool bPresentationCoreDiffers { get { return this.b_presentation_core_differs; } set { this.b_presentation_core_differs = value; } }

	protected bool b_presentation_core_channel_coded; 
	public bool bPresentationCoreChannelCoded { get { return this.b_presentation_core_channel_coded; } set { this.b_presentation_core_channel_coded = value; } }

	protected byte dsi_presentation_channel_mode_core; 
	public byte DsiPresentationChannelModeCore { get { return this.dsi_presentation_channel_mode_core; } set { this.dsi_presentation_channel_mode_core = value; } }

	protected bool b_presentation_filter; 
	public bool bPresentationFilter { get { return this.b_presentation_filter; } set { this.b_presentation_filter = value; } }

	protected bool b_enable_presentation; 
	public bool bEnablePresentation { get { return this.b_enable_presentation; } set { this.b_enable_presentation = value; } }

	protected byte n_filter_bytes; 
	public byte nFilterBytes { get { return this.n_filter_bytes; } set { this.n_filter_bytes = value; } }

	protected byte[] filter_data; 
	public byte[] FilterData { get { return this.filter_data; } set { this.filter_data = value; } }

	protected AC4SubstreamGroupDsi substream_group; 
	public AC4SubstreamGroupDsi SubstreamGroup { get { return this.substream_group; } set { this.substream_group = value; } }

	protected bool b_multi_pid; 
	public bool bMultiPid { get { return this.b_multi_pid; } set { this.b_multi_pid = value; } }

	protected AC4SubstreamGroupDsi[] substream_groups; 
	public AC4SubstreamGroupDsi[] SubstreamGroups { get { return this.substream_groups; } set { this.substream_groups = value; } }

	protected byte n_substream_groups_minus2; 
	public byte nSubstreamGroupsMinus2 { get { return this.n_substream_groups_minus2; } set { this.n_substream_groups_minus2 = value; } }

	protected byte n_skip_bytes; 
	public byte nSkipBytes { get { return this.n_skip_bytes; } set { this.n_skip_bytes = value; } }

	protected byte[] skip_data; 
	public byte[] SkipData { get { return this.skip_data; } set { this.skip_data = value; } }

	protected bool b_pre_virtualized; 
	public bool bPreVirtualized { get { return this.b_pre_virtualized; } set { this.b_pre_virtualized = value; } }

	protected bool b_add_emdf_substreams; 
	public bool bAddEmdfSubstreams { get { return this.b_add_emdf_substreams; } set { this.b_add_emdf_substreams = value; } }

	protected byte n_add_emdf_substreams; 
	public byte nAddEmdfSubstreams { get { return this.n_add_emdf_substreams; } set { this.n_add_emdf_substreams = value; } }

	protected byte[] substream_emdf_version; 
	public byte[] SubstreamEmdfVersion { get { return this.substream_emdf_version; } set { this.substream_emdf_version = value; } }

	protected ushort[] substream_key_id; 
	public ushort[] SubstreamKeyId { get { return this.substream_key_id; } set { this.substream_key_id = value; } }

	protected bool b_presentation_bitrate_info; 
	public bool bPresentationBitrateInfo { get { return this.b_presentation_bitrate_info; } set { this.b_presentation_bitrate_info = value; } }

	protected AC4BitrateDsi ac4_bitrate_dsi; 
	public AC4BitrateDsi Ac4BitrateDsi { get { return this.ac4_bitrate_dsi; } set { this.ac4_bitrate_dsi = value; } }

	protected bool b_alternative; 
	public bool bAlternative { get { return this.b_alternative; } set { this.b_alternative = value; } }

	protected AlignmentBits byte_alignment; 
	public AlignmentBits ByteAlignment { get { return this.byte_alignment; } set { this.byte_alignment = value; } }

	protected AC4AlternativeInfo alternative_info; 
	public AC4AlternativeInfo AlternativeInfo { get { return this.alternative_info; } set { this.alternative_info = value; } }

	protected bool de_indicator; 
	public bool DeIndicator { get { return this.de_indicator; } set { this.de_indicator = value; } }

	protected bool immersive_audio_indicator; 
	public bool ImmersiveAudioIndicator { get { return this.immersive_audio_indicator; } set { this.immersive_audio_indicator = value; } }

	protected byte reserved; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected bool b_extended_presentation_id; 
	public bool bExtendedPresentationId { get { return this.b_extended_presentation_id; } set { this.b_extended_presentation_id = value; } }

	protected ushort extended_presentation_id; 
	public ushort ExtendedPresentationId { get { return this.extended_presentation_id; } set { this.extended_presentation_id = value; } }

	protected bool reserved1; 
	public bool Reserved1 { get { return this.reserved1; } set { this.reserved1 = value; } }

	protected byte[] skip_area; 
	public byte[] SkipArea { get { return this.skip_area; } set { this.skip_area = value; } }

	protected int pres_bytes; 
	public int PresBytes { get { return this.pres_bytes; } set { this.pres_bytes = value; } }

	public AC4PresentationV1Dsi(int pres_bytes = 0): base()
	{
		this.pres_bytes = pres_bytes;
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.presentation_config_v1, "presentation_config_v1"); 

		if (presentation_config_v1 != 0x06)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.md_compat, "md_compat"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.b_presentation_id, "b_presentation_id"); 

			if (b_presentation_id)
			{
				boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.presentation_id, "presentation_id"); 
			}
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.dsi_frame_rate_multiply_info, "dsi_frame_rate_multiply_info"); 
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.dsi_frame_rate_fraction_info, "dsi_frame_rate_fraction_info"); 
			boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.presentation_emdf_version, "presentation_emdf_version"); 
			boxSize += stream.ReadBits(boxSize, readSize, 10,  out this.presentation_key_id, "presentation_key_id"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.b_presentation_channel_coded, "b_presentation_channel_coded"); 

			if (b_presentation_channel_coded)
			{
				boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.dsi_presentation_ch_mode, "dsi_presentation_ch_mode"); 

				if (dsi_presentation_ch_mode >= 11 && dsi_presentation_ch_mode <= 14)
				{
					boxSize += stream.ReadBit(boxSize, readSize,  out this.pres_b_4_back_channels_present, "pres_b_4_back_channels_present"); 
					boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.pres_top_channel_pairs, "pres_top_channel_pairs"); 
				}
				boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved_zero, "reserved_zero"); 
				boxSize += stream.ReadBits(boxSize, readSize, 18,  out this.presentation_v1_channel_groups, "presentation_v1_channel_groups"); 
			}
			boxSize += stream.ReadBit(boxSize, readSize,  out this.b_presentation_core_differs, "b_presentation_core_differs"); 

			if (b_presentation_core_differs)
			{
				boxSize += stream.ReadBit(boxSize, readSize,  out this.b_presentation_core_channel_coded, "b_presentation_core_channel_coded"); 

				if (b_presentation_core_channel_coded)
				{
					boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.dsi_presentation_channel_mode_core, "dsi_presentation_channel_mode_core"); 
				}
			}
			boxSize += stream.ReadBit(boxSize, readSize,  out this.b_presentation_filter, "b_presentation_filter"); 

			if (b_presentation_filter)
			{
				boxSize += stream.ReadBit(boxSize, readSize,  out this.b_enable_presentation, "b_enable_presentation"); 
				boxSize += stream.ReadUInt8(boxSize, readSize,  out this.n_filter_bytes, "n_filter_bytes"); 
				boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(n_filter_bytes),  out this.filter_data, "filter_data"); 
			}

			if (presentation_config_v1 == 0x1f)
			{
				boxSize += stream.ReadClass(boxSize, readSize, this, () => new AC4SubstreamGroupDsi(),  out this.substream_group, "substream_group"); 
			}

			else 
			{
				boxSize += stream.ReadBit(boxSize, readSize,  out this.b_multi_pid, "b_multi_pid"); 

				if (presentation_config_v1 <= 2)
				{

					this.substream_groups = stream.SafeAllocate<AC4SubstreamGroupDsi>(boxSize, readSize, IsoStream.GetInt( 2), "substream_groups");
					for (int i = 0; i < 2; i++)
					{
						boxSize += stream.ReadClass(boxSize, readSize, this, () => new AC4SubstreamGroupDsi(),  out this.substream_groups[i], "substream_groups"); 
					}
				}

				if (presentation_config_v1 == 3 || presentation_config_v1 == 4)
				{

					this.substream_groups = stream.SafeAllocate<AC4SubstreamGroupDsi>(boxSize, readSize, IsoStream.GetInt( 3), "substream_groups");
					for (int i = 0; i < 3; i++)
					{
						boxSize += stream.ReadClass(boxSize, readSize, this, () => new AC4SubstreamGroupDsi(),  out this.substream_groups[i], "substream_groups"); 
					}
				}

				if (presentation_config_v1 == 5)
				{
					boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.n_substream_groups_minus2, "n_substream_groups_minus2"); 

					this.substream_groups = stream.SafeAllocate<AC4SubstreamGroupDsi>(boxSize, readSize, IsoStream.GetInt( n_substream_groups_minus2 + 2), "substream_groups");
					for (int i = 0; i < n_substream_groups_minus2 + 2; i++)
					{
						boxSize += stream.ReadClass(boxSize, readSize, this, () => new AC4SubstreamGroupDsi(),  out this.substream_groups[i], "substream_groups"); 
					}
				}

				if (presentation_config_v1 > 5)
				{
					boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.n_skip_bytes, "n_skip_bytes"); 
					boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(n_skip_bytes),  out this.skip_data, "skip_data"); 
				}
			}
			boxSize += stream.ReadBit(boxSize, readSize,  out this.b_pre_virtualized, "b_pre_virtualized"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.b_add_emdf_substreams, "b_add_emdf_substreams"); 
		}

		if (presentation_config_v1 == 0x06 || b_add_emdf_substreams)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.n_add_emdf_substreams, "n_add_emdf_substreams"); 

			this.substream_emdf_version = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt( n_add_emdf_substreams), "substream_emdf_version");
			this.substream_key_id = stream.SafeAllocate<ushort>(boxSize, readSize, IsoStream.GetInt( n_add_emdf_substreams), "substream_key_id");
			for (int j = 0; j < n_add_emdf_substreams; j++)
			{
				boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.substream_emdf_version[j], "substream_emdf_version"); 
				boxSize += stream.ReadBits(boxSize, readSize, 10,  out this.substream_key_id[j], "substream_key_id"); 
			}
		}
		boxSize += stream.ReadBit(boxSize, readSize,  out this.b_presentation_bitrate_info, "b_presentation_bitrate_info"); 

		if (b_presentation_bitrate_info)
		{
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new AC4BitrateDsi(),  out this.ac4_bitrate_dsi, "ac4_bitrate_dsi"); 
		}
		boxSize += stream.ReadBit(boxSize, readSize,  out this.b_alternative, "b_alternative"); 

		if (b_alternative)
		{
			boxSize += stream.ReadByteAlignment(boxSize, readSize,  out this.byte_alignment, "byte_alignment"); 
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new AC4AlternativeInfo(),  out this.alternative_info, "alternative_info"); 
		}
		boxSize += stream.ReadByteAlignment(boxSize, readSize,  out this.byte_alignment, "byte_alignment"); 

		if ((long)boxSize <= (pres_bytes - 1) * 8)
		{
			boxSize += stream.ReadBit(boxSize, readSize,  out this.de_indicator, "de_indicator"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.immersive_audio_indicator, "immersive_audio_indicator"); 
			boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.reserved, "reserved"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.b_extended_presentation_id, "b_extended_presentation_id"); 

			if (b_extended_presentation_id)
			{
				boxSize += stream.ReadBits(boxSize, readSize, 9,  out this.extended_presentation_id, "extended_presentation_id"); 
			}

			else 
			{
				boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved1, "reserved1"); 
			}
		}
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(pres_bytes - (long)boxSize / 8),  out this.skip_area, "skip_area"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBits(5,  this.presentation_config_v1, "presentation_config_v1"); 

		if (presentation_config_v1 != 0x06)
		{
			boxSize += stream.WriteBits(3,  this.md_compat, "md_compat"); 
			boxSize += stream.WriteBit( this.b_presentation_id, "b_presentation_id"); 

			if (b_presentation_id)
			{
				boxSize += stream.WriteBits(5,  this.presentation_id, "presentation_id"); 
			}
			boxSize += stream.WriteBits(2,  this.dsi_frame_rate_multiply_info, "dsi_frame_rate_multiply_info"); 
			boxSize += stream.WriteBits(2,  this.dsi_frame_rate_fraction_info, "dsi_frame_rate_fraction_info"); 
			boxSize += stream.WriteBits(5,  this.presentation_emdf_version, "presentation_emdf_version"); 
			boxSize += stream.WriteBits(10,  this.presentation_key_id, "presentation_key_id"); 
			boxSize += stream.WriteBit( this.b_presentation_channel_coded, "b_presentation_channel_coded"); 

			if (b_presentation_channel_coded)
			{
				boxSize += stream.WriteBits(5,  this.dsi_presentation_ch_mode, "dsi_presentation_ch_mode"); 

				if (dsi_presentation_ch_mode >= 11 && dsi_presentation_ch_mode <= 14)
				{
					boxSize += stream.WriteBit( this.pres_b_4_back_channels_present, "pres_b_4_back_channels_present"); 
					boxSize += stream.WriteBits(2,  this.pres_top_channel_pairs, "pres_top_channel_pairs"); 
				}
				boxSize += stream.WriteBits(6,  this.reserved_zero, "reserved_zero"); 
				boxSize += stream.WriteBits(18,  this.presentation_v1_channel_groups, "presentation_v1_channel_groups"); 
			}
			boxSize += stream.WriteBit( this.b_presentation_core_differs, "b_presentation_core_differs"); 

			if (b_presentation_core_differs)
			{
				boxSize += stream.WriteBit( this.b_presentation_core_channel_coded, "b_presentation_core_channel_coded"); 

				if (b_presentation_core_channel_coded)
				{
					boxSize += stream.WriteBits(2,  this.dsi_presentation_channel_mode_core, "dsi_presentation_channel_mode_core"); 
				}
			}
			boxSize += stream.WriteBit( this.b_presentation_filter, "b_presentation_filter"); 

			if (b_presentation_filter)
			{
				boxSize += stream.WriteBit( this.b_enable_presentation, "b_enable_presentation"); 
				boxSize += stream.WriteUInt8( this.n_filter_bytes, "n_filter_bytes"); 
				boxSize += stream.WriteUInt8Array((uint)(n_filter_bytes),  this.filter_data, "filter_data"); 
			}

			if (presentation_config_v1 == 0x1f)
			{
				boxSize += stream.WriteClass( this.substream_group, "substream_group"); 
			}

			else 
			{
				boxSize += stream.WriteBit( this.b_multi_pid, "b_multi_pid"); 

				if (presentation_config_v1 <= 2)
				{

					for (int i = 0; i < 2; i++)
					{
						boxSize += stream.WriteClass( this.substream_groups[i], "substream_groups"); 
					}
				}

				if (presentation_config_v1 == 3 || presentation_config_v1 == 4)
				{

					for (int i = 0; i < 3; i++)
					{
						boxSize += stream.WriteClass( this.substream_groups[i], "substream_groups"); 
					}
				}

				if (presentation_config_v1 == 5)
				{
					boxSize += stream.WriteBits(3,  this.n_substream_groups_minus2, "n_substream_groups_minus2"); 

					for (int i = 0; i < n_substream_groups_minus2 + 2; i++)
					{
						boxSize += stream.WriteClass( this.substream_groups[i], "substream_groups"); 
					}
				}

				if (presentation_config_v1 > 5)
				{
					boxSize += stream.WriteBits(7,  this.n_skip_bytes, "n_skip_bytes"); 
					boxSize += stream.WriteUInt8Array((uint)(n_skip_bytes),  this.skip_data, "skip_data"); 
				}
			}
			boxSize += stream.WriteBit( this.b_pre_virtualized, "b_pre_virtualized"); 
			boxSize += stream.WriteBit( this.b_add_emdf_substreams, "b_add_emdf_substreams"); 
		}

		if (presentation_config_v1 == 0x06 || b_add_emdf_substreams)
		{
			boxSize += stream.WriteBits(7,  this.n_add_emdf_substreams, "n_add_emdf_substreams"); 

			for (int j = 0; j < n_add_emdf_substreams; j++)
			{
				boxSize += stream.WriteBits(5,  this.substream_emdf_version[j], "substream_emdf_version"); 
				boxSize += stream.WriteBits(10,  this.substream_key_id[j], "substream_key_id"); 
			}
		}
		boxSize += stream.WriteBit( this.b_presentation_bitrate_info, "b_presentation_bitrate_info"); 

		if (b_presentation_bitrate_info)
		{
			boxSize += stream.WriteClass( this.ac4_bitrate_dsi, "ac4_bitrate_dsi"); 
		}
		boxSize += stream.WriteBit( this.b_alternative, "b_alternative"); 

		if (b_alternative)
		{
			boxSize += stream.WriteByteAlignment( this.byte_alignment, "byte_alignment"); 
			boxSize += stream.WriteClass( this.alternative_info, "alternative_info"); 
		}
		boxSize += stream.WriteByteAlignment( this.byte_alignment, "byte_alignment"); 

		if ((long)boxSize <= (pres_bytes - 1) * 8)
		{
			boxSize += stream.WriteBit( this.de_indicator, "de_indicator"); 
			boxSize += stream.WriteBit( this.immersive_audio_indicator, "immersive_audio_indicator"); 
			boxSize += stream.WriteBits(4,  this.reserved, "reserved"); 
			boxSize += stream.WriteBit( this.b_extended_presentation_id, "b_extended_presentation_id"); 

			if (b_extended_presentation_id)
			{
				boxSize += stream.WriteBits(9,  this.extended_presentation_id, "extended_presentation_id"); 
			}

			else 
			{
				boxSize += stream.WriteBit( this.reserved1, "reserved1"); 
			}
		}
		boxSize += stream.WriteUInt8Array((uint)(pres_bytes - (long)boxSize / 8),  this.skip_area, "skip_area"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 5; // presentation_config_v1

		if (presentation_config_v1 != 0x06)
		{
			boxSize += 3; // md_compat
			boxSize += 1; // b_presentation_id

			if (b_presentation_id)
			{
				boxSize += 5; // presentation_id
			}
			boxSize += 2; // dsi_frame_rate_multiply_info
			boxSize += 2; // dsi_frame_rate_fraction_info
			boxSize += 5; // presentation_emdf_version
			boxSize += 10; // presentation_key_id
			boxSize += 1; // b_presentation_channel_coded

			if (b_presentation_channel_coded)
			{
				boxSize += 5; // dsi_presentation_ch_mode

				if (dsi_presentation_ch_mode >= 11 && dsi_presentation_ch_mode <= 14)
				{
					boxSize += 1; // pres_b_4_back_channels_present
					boxSize += 2; // pres_top_channel_pairs
				}
				boxSize += 6; // reserved_zero
				boxSize += 18; // presentation_v1_channel_groups
			}
			boxSize += 1; // b_presentation_core_differs

			if (b_presentation_core_differs)
			{
				boxSize += 1; // b_presentation_core_channel_coded

				if (b_presentation_core_channel_coded)
				{
					boxSize += 2; // dsi_presentation_channel_mode_core
				}
			}
			boxSize += 1; // b_presentation_filter

			if (b_presentation_filter)
			{
				boxSize += 1; // b_enable_presentation
				boxSize += 8; // n_filter_bytes
				boxSize += ((ulong)(n_filter_bytes) * 8); // filter_data
			}

			if (presentation_config_v1 == 0x1f)
			{
				boxSize += IsoStream.CalculateClassSize(substream_group); // substream_group
			}

			else 
			{
				boxSize += 1; // b_multi_pid

				if (presentation_config_v1 <= 2)
				{

					for (int i = 0; i < 2; i++)
					{
						boxSize += IsoStream.CalculateClassSize(substream_groups[i]); // substream_groups
					}
				}

				if (presentation_config_v1 == 3 || presentation_config_v1 == 4)
				{

					for (int i = 0; i < 3; i++)
					{
						boxSize += IsoStream.CalculateClassSize(substream_groups[i]); // substream_groups
					}
				}

				if (presentation_config_v1 == 5)
				{
					boxSize += 3; // n_substream_groups_minus2

					for (int i = 0; i < n_substream_groups_minus2 + 2; i++)
					{
						boxSize += IsoStream.CalculateClassSize(substream_groups[i]); // substream_groups
					}
				}

				if (presentation_config_v1 > 5)
				{
					boxSize += 7; // n_skip_bytes
					boxSize += ((ulong)(n_skip_bytes) * 8); // skip_data
				}
			}
			boxSize += 1; // b_pre_virtualized
			boxSize += 1; // b_add_emdf_substreams
		}

		if (presentation_config_v1 == 0x06 || b_add_emdf_substreams)
		{
			boxSize += 7; // n_add_emdf_substreams

			for (int j = 0; j < n_add_emdf_substreams; j++)
			{
				boxSize += 5; // substream_emdf_version
				boxSize += 10; // substream_key_id
			}
		}
		boxSize += 1; // b_presentation_bitrate_info

		if (b_presentation_bitrate_info)
		{
			boxSize += IsoStream.CalculateClassSize(ac4_bitrate_dsi); // ac4_bitrate_dsi
		}
		boxSize += 1; // b_alternative

		if (b_alternative)
		{
			boxSize += IsoStream.CalculateByteAlignmentSize(boxSize, byte_alignment); // byte_alignment
			boxSize += IsoStream.CalculateClassSize(alternative_info); // alternative_info
		}
		boxSize += IsoStream.CalculateByteAlignmentSize(boxSize, byte_alignment); // byte_alignment

		if ((long)boxSize <= (pres_bytes - 1) * 8)
		{
			boxSize += 1; // de_indicator
			boxSize += 1; // immersive_audio_indicator
			boxSize += 4; // reserved
			boxSize += 1; // b_extended_presentation_id

			if (b_extended_presentation_id)
			{
				boxSize += 9; // extended_presentation_id
			}

			else 
			{
				boxSize += 1; // reserved1
			}
		}
		boxSize += ((ulong)(pres_bytes - (long)boxSize / 8) * 8); // skip_area
		return boxSize;
	}
}

}
