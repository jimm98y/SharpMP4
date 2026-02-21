using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class HILNenexConfig()
{
  uimsbf(1) HILNenhaLayer;
  if (HILNenhaLayer) {
    uimsbf(2) HILNenhaQuantMode;
  }
}


*/
public partial class HILNenexConfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "HILNenexConfig"; } }

	protected bool HILNenhaLayer; 
	public bool _HILNenhaLayer { get { return this.HILNenhaLayer; } set { this.HILNenhaLayer = value; } }

	protected byte HILNenhaQuantMode; 
	public byte _HILNenhaQuantMode { get { return this.HILNenhaQuantMode; } set { this.HILNenhaQuantMode = value; } }

	public HILNenexConfig(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBit(boxSize, readSize,  out this.HILNenhaLayer, "HILNenhaLayer"); 

		if (HILNenhaLayer)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.HILNenhaQuantMode, "HILNenhaQuantMode"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBit( this.HILNenhaLayer, "HILNenhaLayer"); 

		if (HILNenhaLayer)
		{
			boxSize += stream.WriteBits(2,  this.HILNenhaQuantMode, "HILNenhaQuantMode"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 1; // HILNenhaLayer

		if (HILNenhaLayer)
		{
			boxSize += 2; // HILNenhaQuantMode
		}
		return boxSize;
	}
}

}
