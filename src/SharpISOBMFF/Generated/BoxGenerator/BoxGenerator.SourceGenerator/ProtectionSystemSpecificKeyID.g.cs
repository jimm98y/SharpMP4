using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ProtectionSystemSpecificKeyID() {
 unsigned int(8) key[16];
 } 
*/
public partial class ProtectionSystemSpecificKeyID : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "ProtectionSystemSpecificKeyID"; } }

	protected byte[] key; 
	public byte[] Key { get { return this.key; } set { this.key = value; } }

	public ProtectionSystemSpecificKeyID(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 16,  out this.key, "key"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt8Array(16,  this.key, "key"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 16 * 8; // key
		return boxSize;
	}
}

}
