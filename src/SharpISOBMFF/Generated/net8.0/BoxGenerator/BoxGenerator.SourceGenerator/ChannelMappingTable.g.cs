using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class ChannelMappingTable (unsigned int(8) OutputChannelCount){
 unsigned int(8) StreamCount;
 unsigned int(8) CoupledCount;
 unsigned int(8 * OutputChannelCount) ChannelMapping;
 }

 
*/
public partial class ChannelMappingTable : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "ChannelMappingTable"; } }

	protected byte StreamCount; 
	public byte _StreamCount { get { return this.StreamCount; } set { this.StreamCount = value; } }

	protected byte CoupledCount; 
	public byte _CoupledCount { get { return this.CoupledCount; } set { this.CoupledCount = value; } }

	protected byte[] ChannelMapping; 
	public byte[] _ChannelMapping { get { return this.ChannelMapping; } set { this.ChannelMapping = value; } }

	protected byte OutputChannelCount; 
	public byte _OutputChannelCount { get { return this.OutputChannelCount; } set { this.OutputChannelCount = value; } }

	public ChannelMappingTable(byte OutputChannelCount): base()
	{
		this.OutputChannelCount = OutputChannelCount;
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.StreamCount, "StreamCount"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.CoupledCount, "CoupledCount"); 
		boxSize += stream.ReadBits(boxSize, readSize, (uint)(8 * OutputChannelCount ),  out this.ChannelMapping, "ChannelMapping"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt8( this.StreamCount, "StreamCount"); 
		boxSize += stream.WriteUInt8( this.CoupledCount, "CoupledCount"); 
		boxSize += stream.WriteBits((uint)(8 * OutputChannelCount ),  this.ChannelMapping, "ChannelMapping"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 8; // StreamCount
		boxSize += 8; // CoupledCount
		boxSize += (ulong)(8 * OutputChannelCount ); // ChannelMapping
		return boxSize;
	}
}

}
