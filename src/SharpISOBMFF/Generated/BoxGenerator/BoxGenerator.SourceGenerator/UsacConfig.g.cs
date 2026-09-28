using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class UsacConfig()
{
  uimsbf(5) usacSamplingFrequencyIndex;
  if (usacSamplingFrequencyIndex == 0x1f) {
    uimsbf(24) usacSamplingFrequency;
  }
  uimsbf(3) coreSbrFrameLengthIndex;
  uimsbf(5) channelConfigurationIndex;
  if (channelConfigurationIndex == 0) {
    UsacChannelConfig() usacChannelConfig;
  }
  EscapedValue(4, 8, 16) numElementsMinusOne;
  for (i = 0; i < numElementsMinusOne + 1; i++) {
    uimsbf(2) usacElementType;
    if (usacElementType == 0) {
      UsacSingleChannelElementConfig(coreSbrFrameLengthIndex) usacSingleChannelElementConfig;
    }
    if (usacElementType == 1) {
      UsacChannelPairElementConfig(coreSbrFrameLengthIndex) usacChannelPairElementConfig;
    }
    if (usacElementType == 3) {
      UsacExtElementConfig() usacExtElementConfig;
    }
  }
  bslbf(1) usacConfigExtensionPresent;
  if (usacConfigExtensionPresent) {
    UsacConfigExtension() usacConfigExtension;
  }
}

// escapedValue(nBits1, nBits2, nBits3) is EscapedValue, written by hand: a number its parts add up to, of widths its arguments give

*/
public partial class UsacConfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "UsacConfig"; } }

	protected byte usacSamplingFrequencyIndex; 
	public byte UsacSamplingFrequencyIndex { get { return this.usacSamplingFrequencyIndex; } set { this.usacSamplingFrequencyIndex = value; } }

	protected uint usacSamplingFrequency; 
	public uint UsacSamplingFrequency { get { return this.usacSamplingFrequency; } set { this.usacSamplingFrequency = value; } }

	protected byte coreSbrFrameLengthIndex; 
	public byte CoreSbrFrameLengthIndex { get { return this.coreSbrFrameLengthIndex; } set { this.coreSbrFrameLengthIndex = value; } }

	protected byte channelConfigurationIndex; 
	public byte ChannelConfigurationIndex { get { return this.channelConfigurationIndex; } set { this.channelConfigurationIndex = value; } }

	protected UsacChannelConfig usacChannelConfig; 
	public UsacChannelConfig UsacChannelConfig { get { return this.usacChannelConfig; } set { this.usacChannelConfig = value; } }

	protected EscapedValue numElementsMinusOne; 
	public EscapedValue NumElementsMinusOne { get { return this.numElementsMinusOne; } set { this.numElementsMinusOne = value; } }

	protected byte[] usacElementType; 
	public byte[] UsacElementType { get { return this.usacElementType; } set { this.usacElementType = value; } }

	protected UsacSingleChannelElementConfig[] usacSingleChannelElementConfig; 
	public UsacSingleChannelElementConfig[] UsacSingleChannelElementConfig { get { return this.usacSingleChannelElementConfig; } set { this.usacSingleChannelElementConfig = value; } }

	protected UsacChannelPairElementConfig[] usacChannelPairElementConfig; 
	public UsacChannelPairElementConfig[] UsacChannelPairElementConfig { get { return this.usacChannelPairElementConfig; } set { this.usacChannelPairElementConfig = value; } }

	protected UsacExtElementConfig[] usacExtElementConfig; 
	public UsacExtElementConfig[] UsacExtElementConfig { get { return this.usacExtElementConfig; } set { this.usacExtElementConfig = value; } }

	protected bool usacConfigExtensionPresent; 
	public bool UsacConfigExtensionPresent { get { return this.usacConfigExtensionPresent; } set { this.usacConfigExtensionPresent = value; } }

	protected UsacConfigExtension usacConfigExtension; 
	public UsacConfigExtension UsacConfigExtension { get { return this.usacConfigExtension; } set { this.usacConfigExtension = value; } }

	public UsacConfig(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.usacSamplingFrequencyIndex, "usacSamplingFrequencyIndex"); 

		if (usacSamplingFrequencyIndex == 0x1f)
		{
			boxSize += stream.ReadUInt24(boxSize, readSize,  out this.usacSamplingFrequency, "usacSamplingFrequency"); 
		}
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.coreSbrFrameLengthIndex, "coreSbrFrameLengthIndex"); 
		boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.channelConfigurationIndex, "channelConfigurationIndex"); 

		if (channelConfigurationIndex == 0)
		{
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new UsacChannelConfig(),  out this.usacChannelConfig, "usacChannelConfig"); 
		}
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new EscapedValue(4, 8, 16),  out this.numElementsMinusOne, "numElementsMinusOne"); 

		this.usacElementType = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt( numElementsMinusOne + 1), "usacElementType");
		this.usacSingleChannelElementConfig = stream.SafeAllocate<UsacSingleChannelElementConfig>(boxSize, readSize, IsoStream.GetInt( numElementsMinusOne + 1), "usacSingleChannelElementConfig");
		this.usacChannelPairElementConfig = stream.SafeAllocate<UsacChannelPairElementConfig>(boxSize, readSize, IsoStream.GetInt( numElementsMinusOne + 1), "usacChannelPairElementConfig");
		this.usacExtElementConfig = stream.SafeAllocate<UsacExtElementConfig>(boxSize, readSize, IsoStream.GetInt( numElementsMinusOne + 1), "usacExtElementConfig");
		for (int i = 0; i < numElementsMinusOne + 1; i++)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.usacElementType[i], "usacElementType"); 

			if (usacElementType[i] == 0)
			{
				boxSize += stream.ReadClass(boxSize, readSize, this, () => new UsacSingleChannelElementConfig(coreSbrFrameLengthIndex),  out this.usacSingleChannelElementConfig[i], "usacSingleChannelElementConfig"); 
			}

			if (usacElementType[i] == 1)
			{
				boxSize += stream.ReadClass(boxSize, readSize, this, () => new UsacChannelPairElementConfig(coreSbrFrameLengthIndex),  out this.usacChannelPairElementConfig[i], "usacChannelPairElementConfig"); 
			}

			if (usacElementType[i] == 3)
			{
				boxSize += stream.ReadClass(boxSize, readSize, this, () => new UsacExtElementConfig(),  out this.usacExtElementConfig[i], "usacExtElementConfig"); 
			}
		}
		boxSize += stream.ReadBit(boxSize, readSize,  out this.usacConfigExtensionPresent, "usacConfigExtensionPresent"); 

		if (usacConfigExtensionPresent)
		{
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new UsacConfigExtension(),  out this.usacConfigExtension, "usacConfigExtension"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBits(5,  this.usacSamplingFrequencyIndex, "usacSamplingFrequencyIndex"); 

		if (usacSamplingFrequencyIndex == 0x1f)
		{
			boxSize += stream.WriteUInt24( this.usacSamplingFrequency, "usacSamplingFrequency"); 
		}
		boxSize += stream.WriteBits(3,  this.coreSbrFrameLengthIndex, "coreSbrFrameLengthIndex"); 
		boxSize += stream.WriteBits(5,  this.channelConfigurationIndex, "channelConfigurationIndex"); 

		if (channelConfigurationIndex == 0)
		{
			boxSize += stream.WriteClass( this.usacChannelConfig, "usacChannelConfig"); 
		}
		boxSize += stream.WriteClass( this.numElementsMinusOne, "numElementsMinusOne"); 

		for (int i = 0; i < numElementsMinusOne + 1; i++)
		{
			boxSize += stream.WriteBits(2,  this.usacElementType[i], "usacElementType"); 

			if (usacElementType[i] == 0)
			{
				boxSize += stream.WriteClass( this.usacSingleChannelElementConfig[i], "usacSingleChannelElementConfig"); 
			}

			if (usacElementType[i] == 1)
			{
				boxSize += stream.WriteClass( this.usacChannelPairElementConfig[i], "usacChannelPairElementConfig"); 
			}

			if (usacElementType[i] == 3)
			{
				boxSize += stream.WriteClass( this.usacExtElementConfig[i], "usacExtElementConfig"); 
			}
		}
		boxSize += stream.WriteBit( this.usacConfigExtensionPresent, "usacConfigExtensionPresent"); 

		if (usacConfigExtensionPresent)
		{
			boxSize += stream.WriteClass( this.usacConfigExtension, "usacConfigExtension"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 5; // usacSamplingFrequencyIndex

		if (usacSamplingFrequencyIndex == 0x1f)
		{
			boxSize += 24; // usacSamplingFrequency
		}
		boxSize += 3; // coreSbrFrameLengthIndex
		boxSize += 5; // channelConfigurationIndex

		if (channelConfigurationIndex == 0)
		{
			boxSize += IsoStream.CalculateClassSize(usacChannelConfig); // usacChannelConfig
		}
		boxSize += IsoStream.CalculateClassSize(numElementsMinusOne); // numElementsMinusOne

		for (int i = 0; i < numElementsMinusOne + 1; i++)
		{
			boxSize += 2; // usacElementType

			if (usacElementType[i] == 0)
			{
				boxSize += IsoStream.CalculateClassSize(usacSingleChannelElementConfig[i]); // usacSingleChannelElementConfig
			}

			if (usacElementType[i] == 1)
			{
				boxSize += IsoStream.CalculateClassSize(usacChannelPairElementConfig[i]); // usacChannelPairElementConfig
			}

			if (usacElementType[i] == 3)
			{
				boxSize += IsoStream.CalculateClassSize(usacExtElementConfig[i]); // usacExtElementConfig
			}
		}
		boxSize += 1; // usacConfigExtensionPresent

		if (usacConfigExtensionPresent)
		{
			boxSize += IsoStream.CalculateClassSize(usacConfigExtension); // usacConfigExtension
		}
		return boxSize;
	}
}

}
