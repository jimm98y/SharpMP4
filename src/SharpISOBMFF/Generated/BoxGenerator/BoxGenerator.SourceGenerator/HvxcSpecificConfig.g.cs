using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class HvxcSpecificConfig() {
  uimsbf(1) isBaseLayer;
  if (isBaseLayer) {
    HVXCconfig();
  }
}


*/
public partial class HvxcSpecificConfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "HvxcSpecificConfig"; } }

	protected bool isBaseLayer; 
	public bool IsBaseLayer { get { return this.isBaseLayer; } set { this.isBaseLayer = value; } }

	protected HVXCconfig HVXCconfig; 
	public HVXCconfig _HVXCconfig { get { return this.HVXCconfig; } set { this.HVXCconfig = value; } }

	public HvxcSpecificConfig(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBit(boxSize, readSize,  out this.isBaseLayer, "isBaseLayer"); 

		if (isBaseLayer)
		{
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new HVXCconfig(),  out this.HVXCconfig, "HVXCconfig"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBit( this.isBaseLayer, "isBaseLayer"); 

		if (isBaseLayer)
		{
			boxSize += stream.WriteClass( this.HVXCconfig, "HVXCconfig"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 1; // isBaseLayer

		if (isBaseLayer)
		{
			boxSize += IsoStream.CalculateClassSize(HVXCconfig); // HVXCconfig
		}
		return boxSize;
	}
}

}
