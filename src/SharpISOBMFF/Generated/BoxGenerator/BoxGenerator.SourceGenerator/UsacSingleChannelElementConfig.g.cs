using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class UsacSingleChannelElementConfig(coreSbrFrameLengthIndex)
{
  bslbf(1) tw_mdct;
  bslbf(1) noiseFilling;
  if (coreSbrFrameLengthIndex >= 2 && coreSbrFrameLengthIndex <= 4) {
    SbrConfig() sbrConfig;
  }
}


*/
public partial class UsacSingleChannelElementConfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "UsacSingleChannelElementConfig"; } }

	protected bool tw_mdct; 
	public bool TwMdct { get { return this.tw_mdct; } set { this.tw_mdct = value; } }

	protected bool noiseFilling; 
	public bool NoiseFilling { get { return this.noiseFilling; } set { this.noiseFilling = value; } }

	protected SbrConfig sbrConfig; 
	public SbrConfig SbrConfig { get { return this.sbrConfig; } set { this.sbrConfig = value; } }

	protected int coreSbrFrameLengthIndex; 
	public int CoreSbrFrameLengthIndex { get { return this.coreSbrFrameLengthIndex; } set { this.coreSbrFrameLengthIndex = value; } }

	public UsacSingleChannelElementConfig(int coreSbrFrameLengthIndex = 0): base()
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
		}
		return boxSize;
	}
}

}
