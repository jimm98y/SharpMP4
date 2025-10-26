using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class CelpHeader(samplingFrequencyIndex)
{
  uimsbf(1) ExcitationMode;
  uimsbf(1) SampleRateMode;
  uimsbf(1) FineRateControl;
  if (ExcitationMode == RPE) {
    uimsbf(3) RPE_Configuration;
  }
  if (ExcitationMode == MPE) {
    uimsbf(5) MPE_Configuration;
    uimsbf(2) NumEnhLayers;
    uimsbf(1) BandwidthScalabilityMode;
  }
}


*/
public partial class CelpHeader : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "CelpHeader"; } }

	protected bool ExcitationMode; 
	public bool _ExcitationMode { get { return this.ExcitationMode; } set { this.ExcitationMode = value; } }

	protected bool SampleRateMode; 
	public bool _SampleRateMode { get { return this.SampleRateMode; } set { this.SampleRateMode = value; } }

	protected bool FineRateControl; 
	public bool _FineRateControl { get { return this.FineRateControl; } set { this.FineRateControl = value; } }

	protected byte RPE_Configuration; 
	public byte RPEConfiguration { get { return this.RPE_Configuration; } set { this.RPE_Configuration = value; } }

	protected byte MPE_Configuration; 
	public byte MPEConfiguration { get { return this.MPE_Configuration; } set { this.MPE_Configuration = value; } }

	protected byte NumEnhLayers; 
	public byte _NumEnhLayers { get { return this.NumEnhLayers; } set { this.NumEnhLayers = value; } }

	protected bool BandwidthScalabilityMode; 
	public bool _BandwidthScalabilityMode { get { return this.BandwidthScalabilityMode; } set { this.BandwidthScalabilityMode = value; } }

	public CelpHeader(int samplingFrequencyIndex): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		const bool RPE = true;

		const bool MPE = false;

		boxSize += stream.ReadBit(boxSize, readSize,  out this.ExcitationMode, "ExcitationMode"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.SampleRateMode, "SampleRateMode"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.FineRateControl, "FineRateControl"); 

		if (ExcitationMode == RPE)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.RPE_Configuration, "RPE_Configuration"); 
		}

		if (ExcitationMode == MPE)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.MPE_Configuration, "MPE_Configuration"); 
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.NumEnhLayers, "NumEnhLayers"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.BandwidthScalabilityMode, "BandwidthScalabilityMode"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		const bool RPE = true;

		const bool MPE = false;

		boxSize += stream.WriteBit( this.ExcitationMode, "ExcitationMode"); 
		boxSize += stream.WriteBit( this.SampleRateMode, "SampleRateMode"); 
		boxSize += stream.WriteBit( this.FineRateControl, "FineRateControl"); 

		if (ExcitationMode == RPE)
		{
			boxSize += stream.WriteBits(3,  this.RPE_Configuration, "RPE_Configuration"); 
		}

		if (ExcitationMode == MPE)
		{
			boxSize += stream.WriteBits(5,  this.MPE_Configuration, "MPE_Configuration"); 
			boxSize += stream.WriteBits(2,  this.NumEnhLayers, "NumEnhLayers"); 
			boxSize += stream.WriteBit( this.BandwidthScalabilityMode, "BandwidthScalabilityMode"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		const bool RPE = true;

		const bool MPE = false;

		boxSize += 1; // ExcitationMode
		boxSize += 1; // SampleRateMode
		boxSize += 1; // FineRateControl

		if (ExcitationMode == RPE)
		{
			boxSize += 3; // RPE_Configuration
		}

		if (ExcitationMode == MPE)
		{
			boxSize += 5; // MPE_Configuration
			boxSize += 2; // NumEnhLayers
			boxSize += 1; // BandwidthScalabilityMode
		}
		return boxSize;
	}
}

}
