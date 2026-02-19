using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class PARAconfig()
{
  uimsbf(2) PARAmode;
  if (PARAmode != 1) {
    ErHVXCconfig();
  }
  if (PARAmode != 0) {
    HILNconfig();
  }
  uimsbf(1) PARAextensionFlag;
  if (PARAextensionFlag) {
    /* to be defined in MPEG-4 Phase 3 *//*
  }
}


*/
public partial class PARAconfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "PARAconfig"; } }

	protected byte PARAmode; 
	public byte _PARAmode { get { return this.PARAmode; } set { this.PARAmode = value; } }

	protected ErHVXCconfig ErHVXCconfig; 
	public ErHVXCconfig _ErHVXCconfig { get { return this.ErHVXCconfig; } set { this.ErHVXCconfig = value; } }

	protected HILNconfig HILNconfig; 
	public HILNconfig _HILNconfig { get { return this.HILNconfig; } set { this.HILNconfig = value; } }

	protected bool PARAextensionFlag; 
	public bool _PARAextensionFlag { get { return this.PARAextensionFlag; } set { this.PARAextensionFlag = value; } }

	public PARAconfig(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.PARAmode, "PARAmode"); 

		if (PARAmode != 1)
		{
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new ErHVXCconfig(),  out this.ErHVXCconfig, "ErHVXCconfig"); 
		}

		if (PARAmode != 0)
		{
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new HILNconfig(),  out this.HILNconfig, "HILNconfig"); 
		}
		boxSize += stream.ReadBit(boxSize, readSize,  out this.PARAextensionFlag, "PARAextensionFlag"); 

		if (PARAextensionFlag)
		{
			/*  to be defined in MPEG-4 Phase 3  */
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBits(2,  this.PARAmode, "PARAmode"); 

		if (PARAmode != 1)
		{
			boxSize += stream.WriteClass( this.ErHVXCconfig, "ErHVXCconfig"); 
		}

		if (PARAmode != 0)
		{
			boxSize += stream.WriteClass( this.HILNconfig, "HILNconfig"); 
		}
		boxSize += stream.WriteBit( this.PARAextensionFlag, "PARAextensionFlag"); 

		if (PARAextensionFlag)
		{
			/*  to be defined in MPEG-4 Phase 3  */
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 2; // PARAmode

		if (PARAmode != 1)
		{
			boxSize += IsoStream.CalculateClassSize(ErHVXCconfig); // ErHVXCconfig
		}

		if (PARAmode != 0)
		{
			boxSize += IsoStream.CalculateClassSize(HILNconfig); // HILNconfig
		}
		boxSize += 1; // PARAextensionFlag

		if (PARAextensionFlag)
		{
			/*  to be defined in MPEG-4 Phase 3  */
		}
		return boxSize;
	}
}

}
