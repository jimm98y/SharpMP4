using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class IodsSample() {
	unsigned int(8) incTag;
	unsigned int(8) length;
	unsigned int(32) trackId;
 } 
*/
public partial class IodsSample : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "IodsSample"; } }

	protected byte incTag; 
	public byte IncTag { get { return this.incTag; } set { this.incTag = value; } }

	protected byte length; 
	public byte Length { get { return this.length; } set { this.length = value; } }

	protected uint trackId; 
	public uint TrackId { get { return this.trackId; } set { this.trackId = value; } }

	public IodsSample(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.incTag, "incTag"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.length, "length"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.trackId, "trackId"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt8( this.incTag, "incTag"); 
		boxSize += stream.WriteUInt8( this.length, "length"); 
		boxSize += stream.WriteUInt32( this.trackId, "trackId"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 8; // incTag
		boxSize += 8; // length
		boxSize += 32; // trackId
		return boxSize;
	}
}

}
