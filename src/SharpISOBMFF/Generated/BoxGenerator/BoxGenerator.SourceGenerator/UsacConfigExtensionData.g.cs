using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class UsacConfigExtensionData(usacConfigExtLength)
{
  bslbf(8) usacConfigExt[usacConfigExtLength];
}


*/
public partial class UsacConfigExtensionData : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "UsacConfigExtensionData"; } }

	protected byte[] usacConfigExt; 
	public byte[] UsacConfigExt { get { return this.usacConfigExt; } set { this.usacConfigExt = value; } }

	protected uint usacConfigExtLength; 
	public uint UsacConfigExtLength { get { return this.usacConfigExtLength; } set { this.usacConfigExtLength = value; } }

	public UsacConfigExtensionData(uint usacConfigExtLength = 0): base()
	{
		this.usacConfigExtLength = usacConfigExtLength;
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(usacConfigExtLength),  out this.usacConfigExt, "usacConfigExt"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt8Array((uint)(usacConfigExtLength),  this.usacConfigExt, "usacConfigExt"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += ((ulong)(usacConfigExtLength) * 8); // usacConfigExt
		return boxSize;
	}
}

}
