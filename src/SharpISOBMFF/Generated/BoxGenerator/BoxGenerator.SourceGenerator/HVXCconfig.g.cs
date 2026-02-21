using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class HVXCconfig()
{
  uimsbf(1) HVXCvarMode;
  uimsbf(2) HVXCrateMode;
  uimsbf(1) extensionFlag;
  if (extensionFlag) {
    // to be defined in MPEG-4 Version 2
  }
}


*/
public partial class HVXCconfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "HVXCconfig"; } }

	protected bool HVXCvarMode; 
	public bool _HVXCvarMode { get { return this.HVXCvarMode; } set { this.HVXCvarMode = value; } }

	protected byte HVXCrateMode; 
	public byte _HVXCrateMode { get { return this.HVXCrateMode; } set { this.HVXCrateMode = value; } }

	protected bool extensionFlag; 
	public bool ExtensionFlag { get { return this.extensionFlag; } set { this.extensionFlag = value; } }

	public HVXCconfig(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBit(boxSize, readSize,  out this.HVXCvarMode, "HVXCvarMode"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.HVXCrateMode, "HVXCrateMode"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.extensionFlag, "extensionFlag"); 

		if (extensionFlag)
		{
			/*  to be defined in MPEG-4 Version 2 */
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBit( this.HVXCvarMode, "HVXCvarMode"); 
		boxSize += stream.WriteBits(2,  this.HVXCrateMode, "HVXCrateMode"); 
		boxSize += stream.WriteBit( this.extensionFlag, "extensionFlag"); 

		if (extensionFlag)
		{
			/*  to be defined in MPEG-4 Version 2 */
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 1; // HVXCvarMode
		boxSize += 2; // HVXCrateMode
		boxSize += 1; // extensionFlag

		if (extensionFlag)
		{
			/*  to be defined in MPEG-4 Version 2 */
		}
		return boxSize;
	}
}

}
