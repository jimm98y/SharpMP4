using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class ErrorResilientHvxcSpecificConfig() {
  uimsbf(1) isBaseLayer;
  if (isBaseLayer) {
    ErHVXCconfig();
  }
}


*/
public partial class ErrorResilientHvxcSpecificConfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "ErrorResilientHvxcSpecificConfig"; } }

	protected bool isBaseLayer; 
	public bool IsBaseLayer { get { return this.isBaseLayer; } set { this.isBaseLayer = value; } }

	protected ErHVXCconfig ErHVXCconfig; 
	public ErHVXCconfig _ErHVXCconfig { get { return this.ErHVXCconfig; } set { this.ErHVXCconfig = value; } }

	public ErrorResilientHvxcSpecificConfig(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBit(boxSize, readSize,  out this.isBaseLayer, "isBaseLayer"); 

		if (isBaseLayer)
		{
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new ErHVXCconfig(),  out this.ErHVXCconfig, "ErHVXCconfig"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBit( this.isBaseLayer, "isBaseLayer"); 

		if (isBaseLayer)
		{
			boxSize += stream.WriteClass( this.ErHVXCconfig, "ErHVXCconfig"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 1; // isBaseLayer

		if (isBaseLayer)
		{
			boxSize += IsoStream.CalculateClassSize(ErHVXCconfig); // ErHVXCconfig
		}
		return boxSize;
	}
}

}
