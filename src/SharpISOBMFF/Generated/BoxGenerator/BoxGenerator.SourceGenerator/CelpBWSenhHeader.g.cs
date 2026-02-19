using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class CelpBWSenhHeader()
{
  uimsbf(2) BWS_configuration;
}


*/
public partial class CelpBWSenhHeader : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "CelpBWSenhHeader"; } }

	protected byte BWS_configuration; 
	public byte BWSConfiguration { get { return this.BWS_configuration; } set { this.BWS_configuration = value; } }

	public CelpBWSenhHeader(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.BWS_configuration, "BWS_configuration"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBits(2,  this.BWS_configuration, "BWS_configuration"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 2; // BWS_configuration
		return boxSize;
	}
}

}
