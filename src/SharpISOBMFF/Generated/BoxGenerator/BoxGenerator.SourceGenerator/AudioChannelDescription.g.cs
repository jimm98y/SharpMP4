using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class AudioChannelDescription() {
 unsigned int(32) channelLabel;
 unsigned int(32) channelFlags;
 unsigned int(32) coordinates[3];
 } 
*/
public partial class AudioChannelDescription : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "AudioChannelDescription"; } }

	protected uint channelLabel; 
	public uint ChannelLabel { get { return this.channelLabel; } set { this.channelLabel = value; } }

	protected uint channelFlags; 
	public uint ChannelFlags { get { return this.channelFlags; } set { this.channelFlags = value; } }

	protected uint[] coordinates; 
	public uint[] Coordinates { get { return this.coordinates; } set { this.coordinates = value; } }

	public AudioChannelDescription(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.channelLabel, "channelLabel"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.channelFlags, "channelFlags"); 
		boxSize += stream.ReadUInt32Array(boxSize, readSize, 3,  out this.coordinates, "coordinates"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt32( this.channelLabel, "channelLabel"); 
		boxSize += stream.WriteUInt32( this.channelFlags, "channelFlags"); 
		boxSize += stream.WriteUInt32Array(3,  this.coordinates, "coordinates"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 32; // channelLabel
		boxSize += 32; // channelFlags
		boxSize += 3 * 32; // coordinates
		return boxSize;
	}
}

}
