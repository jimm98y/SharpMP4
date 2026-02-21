using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class GASpecificConfig(samplingFrequencyIndex, channelConfiguration, audioObjectType)
{
  bslbf(1) frameLengthFlag;
  bslbf(1) dependsOnCoreCoder;
  if (dependsOnCoreCoder) {
    uimsbf(14) coreCoderDelay;
  }
  bslbf(1) extensionFlag;
  if (!channelConfiguration) {
    program_config_element();
  }
  if ((audioObjectType == 6) || (audioObjectType == 20)) {
    uimsbf(3) layerNr;
  }
  if (extensionFlag) {
    if (audioObjectType == 22) {
      bslbf(5) numOfSubFrame;
      bslbf(11) layer_length;
    }
    if (audioObjectType == 17 || audioObjectType == 19 ||
      audioObjectType == 20 || audioObjectType == 23) {

      bslbf(1) aacSectionDataResilienceFlag;
      bslbf(1) aacScalefactorDataResilienceFlag;
      bslbf(1) aacSpectralDataResilienceFlag;
    }
    bslbf(1) extensionFlag3;
    if (extensionFlag3) {
      /* tbd in version 3 *//*
    }
  }
}


*/
public partial class GASpecificConfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "GASpecificConfig"; } }

	protected bool frameLengthFlag; 
	public bool FrameLengthFlag { get { return this.frameLengthFlag; } set { this.frameLengthFlag = value; } }

	protected bool dependsOnCoreCoder; 
	public bool DependsOnCoreCoder { get { return this.dependsOnCoreCoder; } set { this.dependsOnCoreCoder = value; } }

	protected ushort coreCoderDelay; 
	public ushort CoreCoderDelay { get { return this.coreCoderDelay; } set { this.coreCoderDelay = value; } }

	protected bool extensionFlag; 
	public bool ExtensionFlag { get { return this.extensionFlag; } set { this.extensionFlag = value; } }

	protected program_config_element program_config_element; 
	public program_config_element ProgramConfigElement { get { return this.program_config_element; } set { this.program_config_element = value; } }

	protected byte layerNr; 
	public byte LayerNr { get { return this.layerNr; } set { this.layerNr = value; } }

	protected byte numOfSubFrame; 
	public byte NumOfSubFrame { get { return this.numOfSubFrame; } set { this.numOfSubFrame = value; } }

	protected ushort layer_length; 
	public ushort LayerLength { get { return this.layer_length; } set { this.layer_length = value; } }

	protected bool aacSectionDataResilienceFlag; 
	public bool AacSectionDataResilienceFlag { get { return this.aacSectionDataResilienceFlag; } set { this.aacSectionDataResilienceFlag = value; } }

	protected bool aacScalefactorDataResilienceFlag; 
	public bool AacScalefactorDataResilienceFlag { get { return this.aacScalefactorDataResilienceFlag; } set { this.aacScalefactorDataResilienceFlag = value; } }

	protected bool aacSpectralDataResilienceFlag; 
	public bool AacSpectralDataResilienceFlag { get { return this.aacSpectralDataResilienceFlag; } set { this.aacSpectralDataResilienceFlag = value; } }

	protected bool extensionFlag3; 
	public bool ExtensionFlag3 { get { return this.extensionFlag3; } set { this.extensionFlag3 = value; } }

	protected byte audioObjectType; 
	public byte AudioObjectType { get { return this.audioObjectType; } set { this.audioObjectType = value; } }

	protected int channelConfiguration; 
	public int ChannelConfiguration { get { return this.channelConfiguration; } set { this.channelConfiguration = value; } }

	protected int samplingFrequencyIndex; 
	public int SamplingFrequencyIndex { get { return this.samplingFrequencyIndex; } set { this.samplingFrequencyIndex = value; } }

	public GASpecificConfig(int samplingFrequencyIndex, int channelConfiguration, byte audioObjectType): base()
	{
		this.audioObjectType = audioObjectType;
		this.channelConfiguration = channelConfiguration;
		this.samplingFrequencyIndex = samplingFrequencyIndex;
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBit(boxSize, readSize,  out this.frameLengthFlag, "frameLengthFlag"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.dependsOnCoreCoder, "dependsOnCoreCoder"); 

		if (dependsOnCoreCoder)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 14,  out this.coreCoderDelay, "coreCoderDelay"); 
		}
		boxSize += stream.ReadBit(boxSize, readSize,  out this.extensionFlag, "extensionFlag"); 

		if (channelConfiguration == 0)
		{
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new program_config_element(),  out this.program_config_element, "program_config_element"); 
		}

		if ((audioObjectType == 6) || (audioObjectType == 20))
		{
			boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.layerNr, "layerNr"); 
		}

		if (extensionFlag)
		{

			if (audioObjectType == 22)
			{
				boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.numOfSubFrame, "numOfSubFrame"); 
				boxSize += stream.ReadBits(boxSize, readSize, 11,  out this.layer_length, "layer_length"); 
			}

			if (audioObjectType == 17 || audioObjectType == 19 ||
      audioObjectType == 20 || audioObjectType == 23)
			{
				boxSize += stream.ReadBit(boxSize, readSize,  out this.aacSectionDataResilienceFlag, "aacSectionDataResilienceFlag"); 
				boxSize += stream.ReadBit(boxSize, readSize,  out this.aacScalefactorDataResilienceFlag, "aacScalefactorDataResilienceFlag"); 
				boxSize += stream.ReadBit(boxSize, readSize,  out this.aacSpectralDataResilienceFlag, "aacSpectralDataResilienceFlag"); 
			}
			boxSize += stream.ReadBit(boxSize, readSize,  out this.extensionFlag3, "extensionFlag3"); 

			if (extensionFlag3)
			{
				/*  tbd in version 3  */
			}
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBit( this.frameLengthFlag, "frameLengthFlag"); 
		boxSize += stream.WriteBit( this.dependsOnCoreCoder, "dependsOnCoreCoder"); 

		if (dependsOnCoreCoder)
		{
			boxSize += stream.WriteBits(14,  this.coreCoderDelay, "coreCoderDelay"); 
		}
		boxSize += stream.WriteBit( this.extensionFlag, "extensionFlag"); 

		if (channelConfiguration == 0)
		{
			boxSize += stream.WriteClass( this.program_config_element, "program_config_element"); 
		}

		if ((audioObjectType == 6) || (audioObjectType == 20))
		{
			boxSize += stream.WriteBits(3,  this.layerNr, "layerNr"); 
		}

		if (extensionFlag)
		{

			if (audioObjectType == 22)
			{
				boxSize += stream.WriteBits(5,  this.numOfSubFrame, "numOfSubFrame"); 
				boxSize += stream.WriteBits(11,  this.layer_length, "layer_length"); 
			}

			if (audioObjectType == 17 || audioObjectType == 19 ||
      audioObjectType == 20 || audioObjectType == 23)
			{
				boxSize += stream.WriteBit( this.aacSectionDataResilienceFlag, "aacSectionDataResilienceFlag"); 
				boxSize += stream.WriteBit( this.aacScalefactorDataResilienceFlag, "aacScalefactorDataResilienceFlag"); 
				boxSize += stream.WriteBit( this.aacSpectralDataResilienceFlag, "aacSpectralDataResilienceFlag"); 
			}
			boxSize += stream.WriteBit( this.extensionFlag3, "extensionFlag3"); 

			if (extensionFlag3)
			{
				/*  tbd in version 3  */
			}
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 1; // frameLengthFlag
		boxSize += 1; // dependsOnCoreCoder

		if (dependsOnCoreCoder)
		{
			boxSize += 14; // coreCoderDelay
		}
		boxSize += 1; // extensionFlag

		if (channelConfiguration == 0)
		{
			boxSize += IsoStream.CalculateClassSize(program_config_element); // program_config_element
		}

		if ((audioObjectType == 6) || (audioObjectType == 20))
		{
			boxSize += 3; // layerNr
		}

		if (extensionFlag)
		{

			if (audioObjectType == 22)
			{
				boxSize += 5; // numOfSubFrame
				boxSize += 11; // layer_length
			}

			if (audioObjectType == 17 || audioObjectType == 19 ||
      audioObjectType == 20 || audioObjectType == 23)
			{
				boxSize += 1; // aacSectionDataResilienceFlag
				boxSize += 1; // aacScalefactorDataResilienceFlag
				boxSize += 1; // aacSpectralDataResilienceFlag
			}
			boxSize += 1; // extensionFlag3

			if (extensionFlag3)
			{
				/*  tbd in version 3  */
			}
		}
		return boxSize;
	}
}

}
