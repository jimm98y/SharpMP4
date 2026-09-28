using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class UsacConfig()
{
  bit(8) data[]; // ISO/IEC 23003-3, not at hand: its bytes, to be defined
}


*/
public partial class UsacConfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "UsacConfig"; } }

	protected byte[] data;  //  ISO/IEC 23003-3, not at hand: its bytes, to be defined
	public byte[] Data { get { return this.data; } set { this.data = value; } }

	public UsacConfig(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.data, "data"); // ISO/IEC 23003-3, not at hand: its bytes, to be defined
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt8ArrayTillEnd( this.data, "data"); // ISO/IEC 23003-3, not at hand: its bytes, to be defined
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += ((ulong)data.Length * 8); // data
		return boxSize;
	}
}

}
