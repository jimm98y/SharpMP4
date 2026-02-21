using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class ParametricSpecificConfig()
{
  uimsbf(1) isBaseLayer;
  if (isBaseLayer) {
    PARAconfig();
  }
  else {
    HILNenexConfig();
  }
}


*/
public partial class ParametricSpecificConfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "ParametricSpecificConfig"; } }

	protected bool isBaseLayer; 
	public bool IsBaseLayer { get { return this.isBaseLayer; } set { this.isBaseLayer = value; } }

	protected PARAconfig PARAconfig; 
	public PARAconfig _PARAconfig { get { return this.PARAconfig; } set { this.PARAconfig = value; } }

	protected HILNenexConfig HILNenexConfig; 
	public HILNenexConfig _HILNenexConfig { get { return this.HILNenexConfig; } set { this.HILNenexConfig = value; } }

	public ParametricSpecificConfig(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBit(boxSize, readSize,  out this.isBaseLayer, "isBaseLayer"); 

		if (isBaseLayer)
		{
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new PARAconfig(),  out this.PARAconfig, "PARAconfig"); 
		}

		else 
		{
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new HILNenexConfig(),  out this.HILNenexConfig, "HILNenexConfig"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBit( this.isBaseLayer, "isBaseLayer"); 

		if (isBaseLayer)
		{
			boxSize += stream.WriteClass( this.PARAconfig, "PARAconfig"); 
		}

		else 
		{
			boxSize += stream.WriteClass( this.HILNenexConfig, "HILNenexConfig"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 1; // isBaseLayer

		if (isBaseLayer)
		{
			boxSize += IsoStream.CalculateClassSize(PARAconfig); // PARAconfig
		}

		else 
		{
			boxSize += IsoStream.CalculateClassSize(HILNenexConfig); // HILNenexConfig
		}
		return boxSize;
	}
}

}
