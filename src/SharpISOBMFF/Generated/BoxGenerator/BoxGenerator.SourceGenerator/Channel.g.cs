using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class Channel() {
 unsigned int(16) channel_index;
 unsigned int(16) channel_type;
 unsigned int(16) channel_association;
 } 
*/
public partial class Channel : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "Channel"; } }

	protected ushort channel_index; 
	public ushort ChannelIndex { get { return this.channel_index; } set { this.channel_index = value; } }

	protected ushort channel_type; 
	public ushort ChannelType { get { return this.channel_type; } set { this.channel_type = value; } }

	protected ushort channel_association; 
	public ushort ChannelAssociation { get { return this.channel_association; } set { this.channel_association = value; } }

	public Channel(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.channel_index, "channel_index"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.channel_type, "channel_type"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.channel_association, "channel_association"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt16( this.channel_index, "channel_index"); 
		boxSize += stream.WriteUInt16( this.channel_type, "channel_type"); 
		boxSize += stream.WriteUInt16( this.channel_association, "channel_association"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 16; // channel_index
		boxSize += 16; // channel_type
		boxSize += 16; // channel_association
		return boxSize;
	}
}

}
