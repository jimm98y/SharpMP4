using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class CanonCTBOEntry() {
 unsigned int(32) index;
 unsigned int(64) offset;
 unsigned int(64) size;
}

*/
public partial class CanonCTBOEntry : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "CanonCTBOEntry"; } }

	protected uint index; 
	public uint Index { get { return this.index; } set { this.index = value; } }

	protected ulong offset; 
	public ulong Offset { get { return this.offset; } set { this.offset = value; } }

	protected ulong size; 
	public ulong Size { get { return this.size; } set { this.size = value; } }

	public CanonCTBOEntry(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.index, "index"); 
		boxSize += stream.ReadUInt64(boxSize, readSize,  out this.offset, "offset"); 
		boxSize += stream.ReadUInt64(boxSize, readSize,  out this.size, "size"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt32( this.index, "index"); 
		boxSize += stream.WriteUInt64( this.offset, "offset"); 
		boxSize += stream.WriteUInt64( this.size, "size"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 32; // index
		boxSize += 64; // offset
		boxSize += 64; // size
		return boxSize;
	}
}

}
