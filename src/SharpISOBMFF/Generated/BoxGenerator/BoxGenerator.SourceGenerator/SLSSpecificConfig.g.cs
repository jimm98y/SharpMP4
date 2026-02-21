using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class SLSSpecificConfig(samplingFrequencyIndex,
  channelConfiguration,
  audioObjectType)
{
  uimsbf(3) pcmWordLength;
  uimsbf(1) aac_core_present;
  uimsbf(1) lle_main_stream;
  uimsbf(1) reserved_bit;
  uimsbf(3) frameLength;
  if (!channelConfiguration) {
    program_config_element();
  }
}


*/
public partial class SLSSpecificConfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "SLSSpecificConfig"; } }

	protected byte pcmWordLength; 
	public byte PcmWordLength { get { return this.pcmWordLength; } set { this.pcmWordLength = value; } }

	protected bool aac_core_present; 
	public bool AacCorePresent { get { return this.aac_core_present; } set { this.aac_core_present = value; } }

	protected bool lle_main_stream; 
	public bool LleMainStream { get { return this.lle_main_stream; } set { this.lle_main_stream = value; } }

	protected bool reserved_bit; 
	public bool ReservedBit { get { return this.reserved_bit; } set { this.reserved_bit = value; } }

	protected byte frameLength; 
	public byte FrameLength { get { return this.frameLength; } set { this.frameLength = value; } }

	protected program_config_element program_config_element; 
	public program_config_element ProgramConfigElement { get { return this.program_config_element; } set { this.program_config_element = value; } }

	protected byte audioObjectType; 
	public byte AudioObjectType { get { return this.audioObjectType; } set { this.audioObjectType = value; } }

	protected int channelConfiguration; 
	public int ChannelConfiguration { get { return this.channelConfiguration; } set { this.channelConfiguration = value; } }

	protected int samplingFrequencyIndex; 
	public int SamplingFrequencyIndex { get { return this.samplingFrequencyIndex; } set { this.samplingFrequencyIndex = value; } }

	public SLSSpecificConfig(int samplingFrequencyIndex, int channelConfiguration, byte audioObjectType): base()
	{
		this.audioObjectType = audioObjectType;
		this.channelConfiguration = channelConfiguration;
		this.samplingFrequencyIndex = samplingFrequencyIndex;
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.pcmWordLength, "pcmWordLength"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.aac_core_present, "aac_core_present"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.lle_main_stream, "lle_main_stream"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved_bit, "reserved_bit"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.frameLength, "frameLength"); 

		if (channelConfiguration == 0)
		{
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new program_config_element(),  out this.program_config_element, "program_config_element"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBits(3,  this.pcmWordLength, "pcmWordLength"); 
		boxSize += stream.WriteBit( this.aac_core_present, "aac_core_present"); 
		boxSize += stream.WriteBit( this.lle_main_stream, "lle_main_stream"); 
		boxSize += stream.WriteBit( this.reserved_bit, "reserved_bit"); 
		boxSize += stream.WriteBits(3,  this.frameLength, "frameLength"); 

		if (channelConfiguration == 0)
		{
			boxSize += stream.WriteClass( this.program_config_element, "program_config_element"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 3; // pcmWordLength
		boxSize += 1; // aac_core_present
		boxSize += 1; // lle_main_stream
		boxSize += 1; // reserved_bit
		boxSize += 3; // frameLength

		if (channelConfiguration == 0)
		{
			boxSize += IsoStream.CalculateClassSize(program_config_element); // program_config_element
		}
		return boxSize;
	}
}

}
