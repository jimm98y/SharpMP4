using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class AV1CodecConfigurationRecord
{
  unsigned int(1) marker = 1;
  unsigned int(7) version = 1;
  unsigned int(3) seq_profile;
  unsigned int(5) seq_level_idx_0;
  unsigned int(1) seq_tier_0;
  unsigned int(1) high_bitdepth;
  unsigned int(1) twelve_bit;
  unsigned int(1) monochrome;
  unsigned int(1) chroma_subsampling_x;
  unsigned int(1) chroma_subsampling_y;
  unsigned int(2) chroma_sample_position;
  unsigned int(3) reserved = 0;

  unsigned int(1) initial_presentation_delay_present;
  if(initial_presentation_delay_present) {
    unsigned int(4) initial_presentation_delay_minus_one;
  } else {
    unsigned int(4) reserved = 0;
  }

  unsigned int(8) configOBUs[];
}

*/
public partial class AV1CodecConfigurationRecord : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "AV1CodecConfigurationRecord"; } }

	protected bool marker = true; 
	public bool Marker { get { return this.marker; } set { this.marker = value; } }

	protected byte version = 1; 
	public byte Version { get { return this.version; } set { this.version = value; } }

	protected byte seq_profile; 
	public byte SeqProfile { get { return this.seq_profile; } set { this.seq_profile = value; } }

	protected byte seq_level_idx_0; 
	public byte SeqLevelIdx0 { get { return this.seq_level_idx_0; } set { this.seq_level_idx_0 = value; } }

	protected bool seq_tier_0; 
	public bool SeqTier0 { get { return this.seq_tier_0; } set { this.seq_tier_0 = value; } }

	protected bool high_bitdepth; 
	public bool HighBitdepth { get { return this.high_bitdepth; } set { this.high_bitdepth = value; } }

	protected bool twelve_bit; 
	public bool TwelveBit { get { return this.twelve_bit; } set { this.twelve_bit = value; } }

	protected bool monochrome; 
	public bool Monochrome { get { return this.monochrome; } set { this.monochrome = value; } }

	protected bool chroma_subsampling_x; 
	public bool ChromaSubsamplingx { get { return this.chroma_subsampling_x; } set { this.chroma_subsampling_x = value; } }

	protected bool chroma_subsampling_y; 
	public bool ChromaSubsamplingy { get { return this.chroma_subsampling_y; } set { this.chroma_subsampling_y = value; } }

	protected byte chroma_sample_position; 
	public byte ChromaSamplePosition { get { return this.chroma_sample_position; } set { this.chroma_sample_position = value; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected bool initial_presentation_delay_present; 
	public bool InitialPresentationDelayPresent { get { return this.initial_presentation_delay_present; } set { this.initial_presentation_delay_present = value; } }

	protected byte initial_presentation_delay_minus_one; 
	public byte InitialPresentationDelayMinusOne { get { return this.initial_presentation_delay_minus_one; } set { this.initial_presentation_delay_minus_one = value; } }

	protected byte reserved0 = 0; 
	public byte Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected byte[] configOBUs; 
	public byte[] ConfigOBUs { get { return this.configOBUs; } set { this.configOBUs = value; } }

	public AV1CodecConfigurationRecord(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBit(boxSize, readSize,  out this.marker, "marker"); 
		boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.version, "version"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.seq_profile, "seq_profile"); 
		boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.seq_level_idx_0, "seq_level_idx_0"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.seq_tier_0, "seq_tier_0"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.high_bitdepth, "high_bitdepth"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.twelve_bit, "twelve_bit"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.monochrome, "monochrome"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.chroma_subsampling_x, "chroma_subsampling_x"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.chroma_subsampling_y, "chroma_subsampling_y"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.chroma_sample_position, "chroma_sample_position"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.initial_presentation_delay_present, "initial_presentation_delay_present"); 

		if (initial_presentation_delay_present)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.initial_presentation_delay_minus_one, "initial_presentation_delay_minus_one"); 
		}

		else 
		{
			boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.reserved0, "reserved0"); 
		}
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.configOBUs, "configOBUs"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBit( this.marker, "marker"); 
		boxSize += stream.WriteBits(7,  this.version, "version"); 
		boxSize += stream.WriteBits(3,  this.seq_profile, "seq_profile"); 
		boxSize += stream.WriteBits(5,  this.seq_level_idx_0, "seq_level_idx_0"); 
		boxSize += stream.WriteBit( this.seq_tier_0, "seq_tier_0"); 
		boxSize += stream.WriteBit( this.high_bitdepth, "high_bitdepth"); 
		boxSize += stream.WriteBit( this.twelve_bit, "twelve_bit"); 
		boxSize += stream.WriteBit( this.monochrome, "monochrome"); 
		boxSize += stream.WriteBit( this.chroma_subsampling_x, "chroma_subsampling_x"); 
		boxSize += stream.WriteBit( this.chroma_subsampling_y, "chroma_subsampling_y"); 
		boxSize += stream.WriteBits(2,  this.chroma_sample_position, "chroma_sample_position"); 
		boxSize += stream.WriteBits(3,  this.reserved, "reserved"); 
		boxSize += stream.WriteBit( this.initial_presentation_delay_present, "initial_presentation_delay_present"); 

		if (initial_presentation_delay_present)
		{
			boxSize += stream.WriteBits(4,  this.initial_presentation_delay_minus_one, "initial_presentation_delay_minus_one"); 
		}

		else 
		{
			boxSize += stream.WriteBits(4,  this.reserved0, "reserved0"); 
		}
		boxSize += stream.WriteUInt8ArrayTillEnd( this.configOBUs, "configOBUs"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 1; // marker
		boxSize += 7; // version
		boxSize += 3; // seq_profile
		boxSize += 5; // seq_level_idx_0
		boxSize += 1; // seq_tier_0
		boxSize += 1; // high_bitdepth
		boxSize += 1; // twelve_bit
		boxSize += 1; // monochrome
		boxSize += 1; // chroma_subsampling_x
		boxSize += 1; // chroma_subsampling_y
		boxSize += 2; // chroma_sample_position
		boxSize += 3; // reserved
		boxSize += 1; // initial_presentation_delay_present

		if (initial_presentation_delay_present)
		{
			boxSize += 4; // initial_presentation_delay_minus_one
		}

		else 
		{
			boxSize += 4; // reserved0
		}
		boxSize += ((ulong)configOBUs.Length * 8); // configOBUs
		return boxSize;
	}
}

}
