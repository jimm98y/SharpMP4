using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class ALSSpecificConfig()
{
  uimsbf(32) als_id;
  uimsbf(32) samp_freq;
  uimsbf(32) samples;
  uimsbf(16) channels;
  uimsbf(3) file_type;
  uimsbf(3) resolution;
  uimsbf(1) floating;
  uimsbf(1) msb_first;
  uimsbf(16) frame_length;
  uimsbf(8) random_access;
  uimsbf(2) ra_flag;
  uimsbf(1) adapt_order;
  uimsbf(2) coef_table;
  uimsbf(1) long_term_prediction;
  uimsbf(10) max_order;
  uimsbf(2) block_switching;
  uimsbf(1) bgmc_mode;
  uimsbf(1) sb_part;
  uimsbf(1) joint_stereo;
  uimsbf(1) mc_coding;
  uimsbf(1) chan_config;
  uimsbf(1) chan_sort;
  uimsbf(1) crc_enabled;
  uimsbf(1) RLSLMS;
  uimsbf(5) reserved;
  uimsbf(1) aux_data_enabled;
  if (chan_config) {
    uimsbf(16) chan_config_info;
  }
  if (chan_sort) {
    for (c = 0; c < channels; c++)
      uimsbf(1) chan_pos[c]; // 1..16 uimsbf 
  }
  bslbf(1) byte_align; // TODO: 0..7 bslbf 
  uimsbf(32) header_size;
  uimsbf(32) trailer_size;
  bslbf(header_size * 8) orig_header;
  bslbf(trailer_size * 8) orig_trailer;
  if (crc_enabled) {
    uimsbf(32) crc;
  }
  if ((ra_flag == 2) && (random_access > 0)) {
    for (f = 0; f < ((samples - 1) / (frame_length + 1)) + 1; f++) {
      uimsbf(32) ra_unit_size[f];
    }
  }
  if (aux_data_enabled) {
    uimsbf(32) aux_size;
    bslbf(aux_size * 8) aux_data;
  }
}


*/
public partial class ALSSpecificConfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "ALSSpecificConfig"; } }

	protected uint als_id; 
	public uint AlsId { get { return this.als_id; } set { this.als_id = value; } }

	protected uint samp_freq; 
	public uint SampFreq { get { return this.samp_freq; } set { this.samp_freq = value; } }

	protected uint samples; 
	public uint Samples { get { return this.samples; } set { this.samples = value; } }

	protected ushort channels; 
	public ushort Channels { get { return this.channels; } set { this.channels = value; } }

	protected byte file_type; 
	public byte FileType { get { return this.file_type; } set { this.file_type = value; } }

	protected byte resolution; 
	public byte Resolution { get { return this.resolution; } set { this.resolution = value; } }

	protected bool floating; 
	public bool Floating { get { return this.floating; } set { this.floating = value; } }

	protected bool msb_first; 
	public bool MsbFirst { get { return this.msb_first; } set { this.msb_first = value; } }

	protected ushort frame_length; 
	public ushort FrameLength { get { return this.frame_length; } set { this.frame_length = value; } }

	protected byte random_access; 
	public byte RandomAccess { get { return this.random_access; } set { this.random_access = value; } }

	protected byte ra_flag; 
	public byte RaFlag { get { return this.ra_flag; } set { this.ra_flag = value; } }

	protected bool adapt_order; 
	public bool AdaptOrder { get { return this.adapt_order; } set { this.adapt_order = value; } }

	protected byte coef_table; 
	public byte CoefTable { get { return this.coef_table; } set { this.coef_table = value; } }

	protected bool long_term_prediction; 
	public bool LongTermPrediction { get { return this.long_term_prediction; } set { this.long_term_prediction = value; } }

	protected ushort max_order; 
	public ushort MaxOrder { get { return this.max_order; } set { this.max_order = value; } }

	protected byte block_switching; 
	public byte BlockSwitching { get { return this.block_switching; } set { this.block_switching = value; } }

	protected bool bgmc_mode; 
	public bool BgmcMode { get { return this.bgmc_mode; } set { this.bgmc_mode = value; } }

	protected bool sb_part; 
	public bool SbPart { get { return this.sb_part; } set { this.sb_part = value; } }

	protected bool joint_stereo; 
	public bool JointStereo { get { return this.joint_stereo; } set { this.joint_stereo = value; } }

	protected bool mc_coding; 
	public bool McCoding { get { return this.mc_coding; } set { this.mc_coding = value; } }

	protected bool chan_config; 
	public bool ChanConfig { get { return this.chan_config; } set { this.chan_config = value; } }

	protected bool chan_sort; 
	public bool ChanSort { get { return this.chan_sort; } set { this.chan_sort = value; } }

	protected bool crc_enabled; 
	public bool CrcEnabled { get { return this.crc_enabled; } set { this.crc_enabled = value; } }

	protected bool RLSLMS; 
	public bool _RLSLMS { get { return this.RLSLMS; } set { this.RLSLMS = value; } }

	protected byte reserved; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected bool aux_data_enabled; 
	public bool AuxDataEnabled { get { return this.aux_data_enabled; } set { this.aux_data_enabled = value; } }

	protected ushort chan_config_info; 
	public ushort ChanConfigInfo { get { return this.chan_config_info; } set { this.chan_config_info = value; } }

	protected bool[] chan_pos;  //  1..16 uimsbf 
	public bool[] ChanPos { get { return this.chan_pos; } set { this.chan_pos = value; } }

	protected bool byte_align;  //  TODO: 0..7 bslbf 
	public bool ByteAlign { get { return this.byte_align; } set { this.byte_align = value; } }

	protected uint header_size; 
	public uint HeaderSize { get { return this.header_size; } set { this.header_size = value; } }

	protected uint trailer_size; 
	public uint TrailerSize { get { return this.trailer_size; } set { this.trailer_size = value; } }

	protected byte[] orig_header; 
	public byte[] OrigHeader { get { return this.orig_header; } set { this.orig_header = value; } }

	protected byte[] orig_trailer; 
	public byte[] OrigTrailer { get { return this.orig_trailer; } set { this.orig_trailer = value; } }

	protected uint crc; 
	public uint Crc { get { return this.crc; } set { this.crc = value; } }

	protected uint[] ra_unit_size; 
	public uint[] RaUnitSize { get { return this.ra_unit_size; } set { this.ra_unit_size = value; } }

	protected uint aux_size; 
	public uint AuxSize { get { return this.aux_size; } set { this.aux_size = value; } }

	protected byte[] aux_data; 
	public byte[] AuxData { get { return this.aux_data; } set { this.aux_data = value; } }

	public ALSSpecificConfig(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.als_id, "als_id"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.samp_freq, "samp_freq"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.samples, "samples"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.channels, "channels"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.file_type, "file_type"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.resolution, "resolution"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.floating, "floating"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.msb_first, "msb_first"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.frame_length, "frame_length"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.random_access, "random_access"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.ra_flag, "ra_flag"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.adapt_order, "adapt_order"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.coef_table, "coef_table"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.long_term_prediction, "long_term_prediction"); 
		boxSize += stream.ReadBits(boxSize, readSize, 10,  out this.max_order, "max_order"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.block_switching, "block_switching"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.bgmc_mode, "bgmc_mode"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.sb_part, "sb_part"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.joint_stereo, "joint_stereo"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.mc_coding, "mc_coding"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.chan_config, "chan_config"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.chan_sort, "chan_sort"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.crc_enabled, "crc_enabled"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.RLSLMS, "RLSLMS"); 
		boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.aux_data_enabled, "aux_data_enabled"); 

		if (chan_config)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.chan_config_info, "chan_config_info"); 
		}

		if (chan_sort)
		{

			this.chan_pos = new bool[IsoStream.GetInt( channels)];
			for (int c = 0; c < channels; c++)
			{
				boxSize += stream.ReadBit(boxSize, readSize,  out this.chan_pos[c], "chan_pos"); // 1..16 uimsbf 
			}
		}
		boxSize += stream.ReadBit(boxSize, readSize,  out this.byte_align, "byte_align"); // TODO: 0..7 bslbf 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.header_size, "header_size"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.trailer_size, "trailer_size"); 
		boxSize += stream.ReadBits(boxSize, readSize, (uint)(header_size * 8 ),  out this.orig_header, "orig_header"); 
		boxSize += stream.ReadBits(boxSize, readSize, (uint)(trailer_size * 8 ),  out this.orig_trailer, "orig_trailer"); 

		if (crc_enabled)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.crc, "crc"); 
		}

		if ((ra_flag == 2) && (random_access > 0))
		{

			this.ra_unit_size = new uint[IsoStream.GetInt( ((samples - 1) / (frame_length + 1)) + 1)];
			for (int f = 0; f < ((samples - 1) / (frame_length + 1)) + 1; f++)
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.ra_unit_size[f], "ra_unit_size"); 
			}
		}

		if (aux_data_enabled)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.aux_size, "aux_size"); 
			boxSize += stream.ReadBits(boxSize, readSize, (uint)(aux_size * 8 ),  out this.aux_data, "aux_data"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt32( this.als_id, "als_id"); 
		boxSize += stream.WriteUInt32( this.samp_freq, "samp_freq"); 
		boxSize += stream.WriteUInt32( this.samples, "samples"); 
		boxSize += stream.WriteUInt16( this.channels, "channels"); 
		boxSize += stream.WriteBits(3,  this.file_type, "file_type"); 
		boxSize += stream.WriteBits(3,  this.resolution, "resolution"); 
		boxSize += stream.WriteBit( this.floating, "floating"); 
		boxSize += stream.WriteBit( this.msb_first, "msb_first"); 
		boxSize += stream.WriteUInt16( this.frame_length, "frame_length"); 
		boxSize += stream.WriteUInt8( this.random_access, "random_access"); 
		boxSize += stream.WriteBits(2,  this.ra_flag, "ra_flag"); 
		boxSize += stream.WriteBit( this.adapt_order, "adapt_order"); 
		boxSize += stream.WriteBits(2,  this.coef_table, "coef_table"); 
		boxSize += stream.WriteBit( this.long_term_prediction, "long_term_prediction"); 
		boxSize += stream.WriteBits(10,  this.max_order, "max_order"); 
		boxSize += stream.WriteBits(2,  this.block_switching, "block_switching"); 
		boxSize += stream.WriteBit( this.bgmc_mode, "bgmc_mode"); 
		boxSize += stream.WriteBit( this.sb_part, "sb_part"); 
		boxSize += stream.WriteBit( this.joint_stereo, "joint_stereo"); 
		boxSize += stream.WriteBit( this.mc_coding, "mc_coding"); 
		boxSize += stream.WriteBit( this.chan_config, "chan_config"); 
		boxSize += stream.WriteBit( this.chan_sort, "chan_sort"); 
		boxSize += stream.WriteBit( this.crc_enabled, "crc_enabled"); 
		boxSize += stream.WriteBit( this.RLSLMS, "RLSLMS"); 
		boxSize += stream.WriteBits(5,  this.reserved, "reserved"); 
		boxSize += stream.WriteBit( this.aux_data_enabled, "aux_data_enabled"); 

		if (chan_config)
		{
			boxSize += stream.WriteUInt16( this.chan_config_info, "chan_config_info"); 
		}

		if (chan_sort)
		{

			for (int c = 0; c < channels; c++)
			{
				boxSize += stream.WriteBit( this.chan_pos[c], "chan_pos"); // 1..16 uimsbf 
			}
		}
		boxSize += stream.WriteBit( this.byte_align, "byte_align"); // TODO: 0..7 bslbf 
		boxSize += stream.WriteUInt32( this.header_size, "header_size"); 
		boxSize += stream.WriteUInt32( this.trailer_size, "trailer_size"); 
		boxSize += stream.WriteBits((uint)(header_size * 8 ),  this.orig_header, "orig_header"); 
		boxSize += stream.WriteBits((uint)(trailer_size * 8 ),  this.orig_trailer, "orig_trailer"); 

		if (crc_enabled)
		{
			boxSize += stream.WriteUInt32( this.crc, "crc"); 
		}

		if ((ra_flag == 2) && (random_access > 0))
		{

			for (int f = 0; f < ((samples - 1) / (frame_length + 1)) + 1; f++)
			{
				boxSize += stream.WriteUInt32( this.ra_unit_size[f], "ra_unit_size"); 
			}
		}

		if (aux_data_enabled)
		{
			boxSize += stream.WriteUInt32( this.aux_size, "aux_size"); 
			boxSize += stream.WriteBits((uint)(aux_size * 8 ),  this.aux_data, "aux_data"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 32; // als_id
		boxSize += 32; // samp_freq
		boxSize += 32; // samples
		boxSize += 16; // channels
		boxSize += 3; // file_type
		boxSize += 3; // resolution
		boxSize += 1; // floating
		boxSize += 1; // msb_first
		boxSize += 16; // frame_length
		boxSize += 8; // random_access
		boxSize += 2; // ra_flag
		boxSize += 1; // adapt_order
		boxSize += 2; // coef_table
		boxSize += 1; // long_term_prediction
		boxSize += 10; // max_order
		boxSize += 2; // block_switching
		boxSize += 1; // bgmc_mode
		boxSize += 1; // sb_part
		boxSize += 1; // joint_stereo
		boxSize += 1; // mc_coding
		boxSize += 1; // chan_config
		boxSize += 1; // chan_sort
		boxSize += 1; // crc_enabled
		boxSize += 1; // RLSLMS
		boxSize += 5; // reserved
		boxSize += 1; // aux_data_enabled

		if (chan_config)
		{
			boxSize += 16; // chan_config_info
		}

		if (chan_sort)
		{

			for (int c = 0; c < channels; c++)
			{
				boxSize += 1; // chan_pos
			}
		}
		boxSize += 1; // byte_align
		boxSize += 32; // header_size
		boxSize += 32; // trailer_size
		boxSize += (ulong)(header_size * 8 ); // orig_header
		boxSize += (ulong)(trailer_size * 8 ); // orig_trailer

		if (crc_enabled)
		{
			boxSize += 32; // crc
		}

		if ((ra_flag == 2) && (random_access > 0))
		{

			for (int f = 0; f < ((samples - 1) / (frame_length + 1)) + 1; f++)
			{
				boxSize += 32; // ra_unit_size
			}
		}

		if (aux_data_enabled)
		{
			boxSize += 32; // aux_size
			boxSize += (ulong)(aux_size * 8 ); // aux_data
		}
		return boxSize;
	}
}

}
