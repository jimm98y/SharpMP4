using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class UsacChannelPairElementConfig(coreSbrFrameLengthIndex)
{
  bslbf(1) tw_mdct;
  bslbf(1) noiseFilling;
  if (coreSbrFrameLengthIndex >= 2 && coreSbrFrameLengthIndex <= 4) {
    SbrConfig() sbrConfig;
    uimsbf(2) stereoConfigIndex;
  }
  if (stereoConfigIndex > 0) {
    uimsbf(3) bsFreqRes;
    uimsbf(3) bsFixedGainDMX;
    uimsbf(2) bsTempShapeConfig;
    uimsbf(2) bsDecorrConfig;
    bslbf(1) bsHighRateMode;
    bslbf(1) bsPhaseCoding;
    bslbf(1) bsOttBandsPhasePresent;
    if (bsOttBandsPhasePresent) {
      uimsbf(5) bsOttBandsPhase;
    }
    if (stereoConfigIndex > 1) {
      uimsbf(5) bsResidualBands;
      bslbf(1) bsPseudoLr;
    }
    if (bsTempShapeConfig == 2) {
      bslbf(1) bsEnvQuantMode;
    }
  }
}


*/
public partial class UsacChannelPairElementConfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "UsacChannelPairElementConfig"; } }

	protected bool tw_mdct; 
	public bool TwMdct { get { return this.tw_mdct; } set { this.tw_mdct = value; } }

	protected bool noiseFilling; 
	public bool NoiseFilling { get { return this.noiseFilling; } set { this.noiseFilling = value; } }

	protected SbrConfig sbrConfig; 
	public SbrConfig SbrConfig { get { return this.sbrConfig; } set { this.sbrConfig = value; } }

	protected byte stereoConfigIndex; 
	public byte StereoConfigIndex { get { return this.stereoConfigIndex; } set { this.stereoConfigIndex = value; } }

	protected byte bsFreqRes; 
	public byte BsFreqRes { get { return this.bsFreqRes; } set { this.bsFreqRes = value; } }

	protected byte bsFixedGainDMX; 
	public byte BsFixedGainDMX { get { return this.bsFixedGainDMX; } set { this.bsFixedGainDMX = value; } }

	protected byte bsTempShapeConfig; 
	public byte BsTempShapeConfig { get { return this.bsTempShapeConfig; } set { this.bsTempShapeConfig = value; } }

	protected byte bsDecorrConfig; 
	public byte BsDecorrConfig { get { return this.bsDecorrConfig; } set { this.bsDecorrConfig = value; } }

	protected bool bsHighRateMode; 
	public bool BsHighRateMode { get { return this.bsHighRateMode; } set { this.bsHighRateMode = value; } }

	protected bool bsPhaseCoding; 
	public bool BsPhaseCoding { get { return this.bsPhaseCoding; } set { this.bsPhaseCoding = value; } }

	protected bool bsOttBandsPhasePresent; 
	public bool BsOttBandsPhasePresent { get { return this.bsOttBandsPhasePresent; } set { this.bsOttBandsPhasePresent = value; } }

	protected byte bsOttBandsPhase; 
	public byte BsOttBandsPhase { get { return this.bsOttBandsPhase; } set { this.bsOttBandsPhase = value; } }

	protected byte bsResidualBands; 
	public byte BsResidualBands { get { return this.bsResidualBands; } set { this.bsResidualBands = value; } }

	protected bool bsPseudoLr; 
	public bool BsPseudoLr { get { return this.bsPseudoLr; } set { this.bsPseudoLr = value; } }

	protected bool bsEnvQuantMode; 
	public bool BsEnvQuantMode { get { return this.bsEnvQuantMode; } set { this.bsEnvQuantMode = value; } }

	protected int coreSbrFrameLengthIndex; 
	public int CoreSbrFrameLengthIndex { get { return this.coreSbrFrameLengthIndex; } set { this.coreSbrFrameLengthIndex = value; } }

	public UsacChannelPairElementConfig(int coreSbrFrameLengthIndex = 0): base()
	{
		this.coreSbrFrameLengthIndex = coreSbrFrameLengthIndex;
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBit(boxSize, readSize,  out this.tw_mdct, "tw_mdct"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.noiseFilling, "noiseFilling"); 

		if (coreSbrFrameLengthIndex >= 2 && coreSbrFrameLengthIndex <= 4)
		{
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new SbrConfig(),  out this.sbrConfig, "sbrConfig"); 
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.stereoConfigIndex, "stereoConfigIndex"); 
		}

		if (stereoConfigIndex > 0)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.bsFreqRes, "bsFreqRes"); 
			boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.bsFixedGainDMX, "bsFixedGainDMX"); 
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.bsTempShapeConfig, "bsTempShapeConfig"); 
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.bsDecorrConfig, "bsDecorrConfig"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.bsHighRateMode, "bsHighRateMode"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.bsPhaseCoding, "bsPhaseCoding"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.bsOttBandsPhasePresent, "bsOttBandsPhasePresent"); 

			if (bsOttBandsPhasePresent)
			{
				boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.bsOttBandsPhase, "bsOttBandsPhase"); 
			}

			if (stereoConfigIndex > 1)
			{
				boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.bsResidualBands, "bsResidualBands"); 
				boxSize += stream.ReadBit(boxSize, readSize,  out this.bsPseudoLr, "bsPseudoLr"); 
			}

			if (bsTempShapeConfig == 2)
			{
				boxSize += stream.ReadBit(boxSize, readSize,  out this.bsEnvQuantMode, "bsEnvQuantMode"); 
			}
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBit( this.tw_mdct, "tw_mdct"); 
		boxSize += stream.WriteBit( this.noiseFilling, "noiseFilling"); 

		if (coreSbrFrameLengthIndex >= 2 && coreSbrFrameLengthIndex <= 4)
		{
			boxSize += stream.WriteClass( this.sbrConfig, "sbrConfig"); 
			boxSize += stream.WriteBits(2,  this.stereoConfigIndex, "stereoConfigIndex"); 
		}

		if (stereoConfigIndex > 0)
		{
			boxSize += stream.WriteBits(3,  this.bsFreqRes, "bsFreqRes"); 
			boxSize += stream.WriteBits(3,  this.bsFixedGainDMX, "bsFixedGainDMX"); 
			boxSize += stream.WriteBits(2,  this.bsTempShapeConfig, "bsTempShapeConfig"); 
			boxSize += stream.WriteBits(2,  this.bsDecorrConfig, "bsDecorrConfig"); 
			boxSize += stream.WriteBit( this.bsHighRateMode, "bsHighRateMode"); 
			boxSize += stream.WriteBit( this.bsPhaseCoding, "bsPhaseCoding"); 
			boxSize += stream.WriteBit( this.bsOttBandsPhasePresent, "bsOttBandsPhasePresent"); 

			if (bsOttBandsPhasePresent)
			{
				boxSize += stream.WriteBits(5,  this.bsOttBandsPhase, "bsOttBandsPhase"); 
			}

			if (stereoConfigIndex > 1)
			{
				boxSize += stream.WriteBits(5,  this.bsResidualBands, "bsResidualBands"); 
				boxSize += stream.WriteBit( this.bsPseudoLr, "bsPseudoLr"); 
			}

			if (bsTempShapeConfig == 2)
			{
				boxSize += stream.WriteBit( this.bsEnvQuantMode, "bsEnvQuantMode"); 
			}
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 1; // tw_mdct
		boxSize += 1; // noiseFilling

		if (coreSbrFrameLengthIndex >= 2 && coreSbrFrameLengthIndex <= 4)
		{
			boxSize += IsoStream.CalculateClassSize(sbrConfig); // sbrConfig
			boxSize += 2; // stereoConfigIndex
		}

		if (stereoConfigIndex > 0)
		{
			boxSize += 3; // bsFreqRes
			boxSize += 3; // bsFixedGainDMX
			boxSize += 2; // bsTempShapeConfig
			boxSize += 2; // bsDecorrConfig
			boxSize += 1; // bsHighRateMode
			boxSize += 1; // bsPhaseCoding
			boxSize += 1; // bsOttBandsPhasePresent

			if (bsOttBandsPhasePresent)
			{
				boxSize += 5; // bsOttBandsPhase
			}

			if (stereoConfigIndex > 1)
			{
				boxSize += 5; // bsResidualBands
				boxSize += 1; // bsPseudoLr
			}

			if (bsTempShapeConfig == 2)
			{
				boxSize += 1; // bsEnvQuantMode
			}
		}
		return boxSize;
	}
}

}
