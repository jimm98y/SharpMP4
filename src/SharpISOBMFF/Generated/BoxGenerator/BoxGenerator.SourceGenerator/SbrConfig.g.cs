using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class SbrConfig()
{
  bslbf(1) harmonicSBR;
  bslbf(1) bs_interTes;
  bslbf(1) bs_pvc;
  uimsbf(4) dflt_start_freq;
  uimsbf(4) dflt_stop_freq;
  bslbf(1) dflt_header_extra1;
  bslbf(1) dflt_header_extra2;
  if (dflt_header_extra1) {
    uimsbf(2) dflt_freq_scale;
    bslbf(1) dflt_alter_scale;
    uimsbf(2) dflt_noise_bands;
  }
  if (dflt_header_extra2) {
    uimsbf(2) dflt_limiter_bands;
    uimsbf(2) dflt_limiter_gains;
    bslbf(1) dflt_interpol_freq;
    bslbf(1) dflt_smoothing_mode;
  }
}


*/
public partial class SbrConfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "SbrConfig"; } }

	protected bool harmonicSBR; 
	public bool HarmonicSBR { get { return this.harmonicSBR; } set { this.harmonicSBR = value; } }

	protected bool bs_interTes; 
	public bool BsInterTes { get { return this.bs_interTes; } set { this.bs_interTes = value; } }

	protected bool bs_pvc; 
	public bool BsPvc { get { return this.bs_pvc; } set { this.bs_pvc = value; } }

	protected byte dflt_start_freq; 
	public byte DfltStartFreq { get { return this.dflt_start_freq; } set { this.dflt_start_freq = value; } }

	protected byte dflt_stop_freq; 
	public byte DfltStopFreq { get { return this.dflt_stop_freq; } set { this.dflt_stop_freq = value; } }

	protected bool dflt_header_extra1; 
	public bool DfltHeaderExtra1 { get { return this.dflt_header_extra1; } set { this.dflt_header_extra1 = value; } }

	protected bool dflt_header_extra2; 
	public bool DfltHeaderExtra2 { get { return this.dflt_header_extra2; } set { this.dflt_header_extra2 = value; } }

	protected byte dflt_freq_scale; 
	public byte DfltFreqScale { get { return this.dflt_freq_scale; } set { this.dflt_freq_scale = value; } }

	protected bool dflt_alter_scale; 
	public bool DfltAlterScale { get { return this.dflt_alter_scale; } set { this.dflt_alter_scale = value; } }

	protected byte dflt_noise_bands; 
	public byte DfltNoiseBands { get { return this.dflt_noise_bands; } set { this.dflt_noise_bands = value; } }

	protected byte dflt_limiter_bands; 
	public byte DfltLimiterBands { get { return this.dflt_limiter_bands; } set { this.dflt_limiter_bands = value; } }

	protected byte dflt_limiter_gains; 
	public byte DfltLimiterGains { get { return this.dflt_limiter_gains; } set { this.dflt_limiter_gains = value; } }

	protected bool dflt_interpol_freq; 
	public bool DfltInterpolFreq { get { return this.dflt_interpol_freq; } set { this.dflt_interpol_freq = value; } }

	protected bool dflt_smoothing_mode; 
	public bool DfltSmoothingMode { get { return this.dflt_smoothing_mode; } set { this.dflt_smoothing_mode = value; } }

	public SbrConfig(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBit(boxSize, readSize,  out this.harmonicSBR, "harmonicSBR"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.bs_interTes, "bs_interTes"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.bs_pvc, "bs_pvc"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.dflt_start_freq, "dflt_start_freq"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.dflt_stop_freq, "dflt_stop_freq"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.dflt_header_extra1, "dflt_header_extra1"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.dflt_header_extra2, "dflt_header_extra2"); 

		if (dflt_header_extra1)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.dflt_freq_scale, "dflt_freq_scale"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.dflt_alter_scale, "dflt_alter_scale"); 
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.dflt_noise_bands, "dflt_noise_bands"); 
		}

		if (dflt_header_extra2)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.dflt_limiter_bands, "dflt_limiter_bands"); 
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.dflt_limiter_gains, "dflt_limiter_gains"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.dflt_interpol_freq, "dflt_interpol_freq"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.dflt_smoothing_mode, "dflt_smoothing_mode"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBit( this.harmonicSBR, "harmonicSBR"); 
		boxSize += stream.WriteBit( this.bs_interTes, "bs_interTes"); 
		boxSize += stream.WriteBit( this.bs_pvc, "bs_pvc"); 
		boxSize += stream.WriteBits(4,  this.dflt_start_freq, "dflt_start_freq"); 
		boxSize += stream.WriteBits(4,  this.dflt_stop_freq, "dflt_stop_freq"); 
		boxSize += stream.WriteBit( this.dflt_header_extra1, "dflt_header_extra1"); 
		boxSize += stream.WriteBit( this.dflt_header_extra2, "dflt_header_extra2"); 

		if (dflt_header_extra1)
		{
			boxSize += stream.WriteBits(2,  this.dflt_freq_scale, "dflt_freq_scale"); 
			boxSize += stream.WriteBit( this.dflt_alter_scale, "dflt_alter_scale"); 
			boxSize += stream.WriteBits(2,  this.dflt_noise_bands, "dflt_noise_bands"); 
		}

		if (dflt_header_extra2)
		{
			boxSize += stream.WriteBits(2,  this.dflt_limiter_bands, "dflt_limiter_bands"); 
			boxSize += stream.WriteBits(2,  this.dflt_limiter_gains, "dflt_limiter_gains"); 
			boxSize += stream.WriteBit( this.dflt_interpol_freq, "dflt_interpol_freq"); 
			boxSize += stream.WriteBit( this.dflt_smoothing_mode, "dflt_smoothing_mode"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 1; // harmonicSBR
		boxSize += 1; // bs_interTes
		boxSize += 1; // bs_pvc
		boxSize += 4; // dflt_start_freq
		boxSize += 4; // dflt_stop_freq
		boxSize += 1; // dflt_header_extra1
		boxSize += 1; // dflt_header_extra2

		if (dflt_header_extra1)
		{
			boxSize += 2; // dflt_freq_scale
			boxSize += 1; // dflt_alter_scale
			boxSize += 2; // dflt_noise_bands
		}

		if (dflt_header_extra2)
		{
			boxSize += 2; // dflt_limiter_bands
			boxSize += 2; // dflt_limiter_gains
			boxSize += 1; // dflt_interpol_freq
			boxSize += 1; // dflt_smoothing_mode
		}
		return boxSize;
	}
}

}
