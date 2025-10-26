using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class sbr_header()
{
  uimsbf(1) bs_amp_res;
  uimsbf(4) bs_start_freq;
  uimsbf(4) bs_stop_freq;
  uimsbf(3) bs_xover_band;
  uimsbf(2) bs_reserved;
  uimsbf(1) bs_header_extra_1;
  uimsbf(1) bs_header_extra_2;

  if (bs_header_extra_1) {
    uimsbf(2) bs_freq_scale;
    uimsbf(1) bs_alter_scale;
    uimsbf(2) bs_noise_bands;
  }

  if (bs_header_extra_2) {
    uimsbf(2) bs_limiter_bands;
    uimsbf(2) bs_limiter_gains;
    uimsbf(1) bs_interpol_freq;
    uimsbf(1) bs_smoothing_mode;
  }
}


*/
public partial class sbr_header : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "sbr_header"; } }

	protected bool bs_amp_res; 
	public bool BsAmpRes { get { return this.bs_amp_res; } set { this.bs_amp_res = value; } }

	protected byte bs_start_freq; 
	public byte BsStartFreq { get { return this.bs_start_freq; } set { this.bs_start_freq = value; } }

	protected byte bs_stop_freq; 
	public byte BsStopFreq { get { return this.bs_stop_freq; } set { this.bs_stop_freq = value; } }

	protected byte bs_xover_band; 
	public byte BsXoverBand { get { return this.bs_xover_band; } set { this.bs_xover_band = value; } }

	protected byte bs_reserved; 
	public byte BsReserved { get { return this.bs_reserved; } set { this.bs_reserved = value; } }

	protected bool bs_header_extra_1; 
	public bool BsHeaderExtra1 { get { return this.bs_header_extra_1; } set { this.bs_header_extra_1 = value; } }

	protected bool bs_header_extra_2; 
	public bool BsHeaderExtra2 { get { return this.bs_header_extra_2; } set { this.bs_header_extra_2 = value; } }

	protected byte bs_freq_scale; 
	public byte BsFreqScale { get { return this.bs_freq_scale; } set { this.bs_freq_scale = value; } }

	protected bool bs_alter_scale; 
	public bool BsAlterScale { get { return this.bs_alter_scale; } set { this.bs_alter_scale = value; } }

	protected byte bs_noise_bands; 
	public byte BsNoiseBands { get { return this.bs_noise_bands; } set { this.bs_noise_bands = value; } }

	protected byte bs_limiter_bands; 
	public byte BsLimiterBands { get { return this.bs_limiter_bands; } set { this.bs_limiter_bands = value; } }

	protected byte bs_limiter_gains; 
	public byte BsLimiterGains { get { return this.bs_limiter_gains; } set { this.bs_limiter_gains = value; } }

	protected bool bs_interpol_freq; 
	public bool BsInterpolFreq { get { return this.bs_interpol_freq; } set { this.bs_interpol_freq = value; } }

	protected bool bs_smoothing_mode; 
	public bool BsSmoothingMode { get { return this.bs_smoothing_mode; } set { this.bs_smoothing_mode = value; } }

	public sbr_header(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBit(boxSize, readSize,  out this.bs_amp_res, "bs_amp_res"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.bs_start_freq, "bs_start_freq"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.bs_stop_freq, "bs_stop_freq"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.bs_xover_band, "bs_xover_band"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.bs_reserved, "bs_reserved"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.bs_header_extra_1, "bs_header_extra_1"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.bs_header_extra_2, "bs_header_extra_2"); 

		if (bs_header_extra_1)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.bs_freq_scale, "bs_freq_scale"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.bs_alter_scale, "bs_alter_scale"); 
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.bs_noise_bands, "bs_noise_bands"); 
		}

		if (bs_header_extra_2)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.bs_limiter_bands, "bs_limiter_bands"); 
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.bs_limiter_gains, "bs_limiter_gains"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.bs_interpol_freq, "bs_interpol_freq"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.bs_smoothing_mode, "bs_smoothing_mode"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBit( this.bs_amp_res, "bs_amp_res"); 
		boxSize += stream.WriteBits(4,  this.bs_start_freq, "bs_start_freq"); 
		boxSize += stream.WriteBits(4,  this.bs_stop_freq, "bs_stop_freq"); 
		boxSize += stream.WriteBits(3,  this.bs_xover_band, "bs_xover_band"); 
		boxSize += stream.WriteBits(2,  this.bs_reserved, "bs_reserved"); 
		boxSize += stream.WriteBit( this.bs_header_extra_1, "bs_header_extra_1"); 
		boxSize += stream.WriteBit( this.bs_header_extra_2, "bs_header_extra_2"); 

		if (bs_header_extra_1)
		{
			boxSize += stream.WriteBits(2,  this.bs_freq_scale, "bs_freq_scale"); 
			boxSize += stream.WriteBit( this.bs_alter_scale, "bs_alter_scale"); 
			boxSize += stream.WriteBits(2,  this.bs_noise_bands, "bs_noise_bands"); 
		}

		if (bs_header_extra_2)
		{
			boxSize += stream.WriteBits(2,  this.bs_limiter_bands, "bs_limiter_bands"); 
			boxSize += stream.WriteBits(2,  this.bs_limiter_gains, "bs_limiter_gains"); 
			boxSize += stream.WriteBit( this.bs_interpol_freq, "bs_interpol_freq"); 
			boxSize += stream.WriteBit( this.bs_smoothing_mode, "bs_smoothing_mode"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 1; // bs_amp_res
		boxSize += 4; // bs_start_freq
		boxSize += 4; // bs_stop_freq
		boxSize += 3; // bs_xover_band
		boxSize += 2; // bs_reserved
		boxSize += 1; // bs_header_extra_1
		boxSize += 1; // bs_header_extra_2

		if (bs_header_extra_1)
		{
			boxSize += 2; // bs_freq_scale
			boxSize += 1; // bs_alter_scale
			boxSize += 2; // bs_noise_bands
		}

		if (bs_header_extra_2)
		{
			boxSize += 2; // bs_limiter_bands
			boxSize += 2; // bs_limiter_gains
			boxSize += 1; // bs_interpol_freq
			boxSize += 1; // bs_smoothing_mode
		}
		return boxSize;
	}
}

}
