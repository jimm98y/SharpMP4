using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ViprEntry() {
 unsigned int(6) reserved = 0; 
  unsigned int(10) view_id; 
  unsigned int(32) content_priority_id;
 }

*/
public partial class ViprEntry : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "ViprEntry"; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected ushort view_id; 
	public ushort ViewId { get { return this.view_id; } set { this.view_id = value; } }

	protected uint content_priority_id; 
	public uint ContentPriorityId { get { return this.content_priority_id; } set { this.content_priority_id = value; } }

	public ViprEntry(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBits(boxSize, readSize, 10,  out this.view_id, "view_id"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.content_priority_id, "content_priority_id"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBits(6,  this.reserved, "reserved"); 
		boxSize += stream.WriteBits(10,  this.view_id, "view_id"); 
		boxSize += stream.WriteUInt32( this.content_priority_id, "content_priority_id"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 6; // reserved
		boxSize += 10; // view_id
		boxSize += 32; // content_priority_id
		return boxSize;
	}
}

}
