using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class UsacChannelConfig()
{
  EscapedValue(5, 8, 16) numOutChannels;
  for (i = 0; i < numOutChannels; i++) {
    uimsbf(5) bsOutputChannelPos;
  }
}

// sbrRatioIndex, of coreSbrFrameLengthIndex 2, 3 and 4, is not 0: SBR is used

*/
public partial class UsacChannelConfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "UsacChannelConfig"; } }

	protected EscapedValue numOutChannels; 
	public EscapedValue NumOutChannels { get { return this.numOutChannels; } set { this.numOutChannels = value; } }

	protected byte[] bsOutputChannelPos; 
	public byte[] BsOutputChannelPos { get { return this.bsOutputChannelPos; } set { this.bsOutputChannelPos = value; } }

	public UsacChannelConfig(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new EscapedValue(5, 8, 16),  out this.numOutChannels, "numOutChannels"); 

		this.bsOutputChannelPos = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt( numOutChannels), "bsOutputChannelPos");
		for (int i = 0; i < numOutChannels; i++)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.bsOutputChannelPos[i], "bsOutputChannelPos"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteClass( this.numOutChannels, "numOutChannels"); 

		for (int i = 0; i < numOutChannels; i++)
		{
			boxSize += stream.WriteBits(5,  this.bsOutputChannelPos[i], "bsOutputChannelPos"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += IsoStream.CalculateClassSize(numOutChannels); // numOutChannels

		for (int i = 0; i < numOutChannels; i++)
		{
			boxSize += 5; // bsOutputChannelPos
		}
		return boxSize;
	}
}

}
