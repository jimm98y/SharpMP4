using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class SSCSpecificConfig(channelConfiguration)
{
  uimsbf(2) decoder_level;
  uimsbf(4) update_rate;
  uimsbf(2) synthesis_method;
  if (channelConfiguration != 1) {
    uimsbf(2) mode_ext;
    if ((channelConfiguration == 2) && (mode_ext == 1)) {
      uimsbf(2) reserved;
    }
  }
}


*/
public partial class SSCSpecificConfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "SSCSpecificConfig"; } }

	protected byte decoder_level; 
	public byte DecoderLevel { get { return this.decoder_level; } set { this.decoder_level = value; } }

	protected byte update_rate; 
	public byte UpdateRate { get { return this.update_rate; } set { this.update_rate = value; } }

	protected byte synthesis_method; 
	public byte SynthesisMethod { get { return this.synthesis_method; } set { this.synthesis_method = value; } }

	protected byte mode_ext; 
	public byte ModeExt { get { return this.mode_ext; } set { this.mode_ext = value; } }

	protected byte reserved; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected int channelConfiguration; 
	public int ChannelConfiguration { get { return this.channelConfiguration; } set { this.channelConfiguration = value; } }

	public SSCSpecificConfig(int channelConfiguration): base()
	{
		this.channelConfiguration = channelConfiguration;
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.decoder_level, "decoder_level"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.update_rate, "update_rate"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.synthesis_method, "synthesis_method"); 

		if (channelConfiguration != 1)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.mode_ext, "mode_ext"); 

			if ((channelConfiguration == 2) && (mode_ext == 1))
			{
				boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.reserved, "reserved"); 
			}
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBits(2,  this.decoder_level, "decoder_level"); 
		boxSize += stream.WriteBits(4,  this.update_rate, "update_rate"); 
		boxSize += stream.WriteBits(2,  this.synthesis_method, "synthesis_method"); 

		if (channelConfiguration != 1)
		{
			boxSize += stream.WriteBits(2,  this.mode_ext, "mode_ext"); 

			if ((channelConfiguration == 2) && (mode_ext == 1))
			{
				boxSize += stream.WriteBits(2,  this.reserved, "reserved"); 
			}
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 2; // decoder_level
		boxSize += 4; // update_rate
		boxSize += 2; // synthesis_method

		if (channelConfiguration != 1)
		{
			boxSize += 2; // mode_ext

			if ((channelConfiguration == 2) && (mode_ext == 1))
			{
				boxSize += 2; // reserved
			}
		}
		return boxSize;
	}
}

}
