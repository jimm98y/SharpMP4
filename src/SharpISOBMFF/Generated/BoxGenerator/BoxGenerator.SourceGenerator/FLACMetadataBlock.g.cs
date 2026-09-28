using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class FLACMetadataBlock {
 unsigned int(1) LastMetadataBlockFlag;
 unsigned int(7) BlockType;
 unsigned int(24) Length;
 unsigned int(8) BlockData[Length];
 }
*/
public partial class FLACMetadataBlock : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "FLACMetadataBlock"; } }

	protected bool LastMetadataBlockFlag; 
	public bool _LastMetadataBlockFlag { get { return this.LastMetadataBlockFlag; } set { this.LastMetadataBlockFlag = value; } }

	protected byte BlockType; 
	public byte _BlockType { get { return this.BlockType; } set { this.BlockType = value; } }

	protected uint Length; 
	public uint _Length { get { return this.Length; } set { this.Length = value; } }

	protected byte[] BlockData; 
	public byte[] _BlockData { get { return this.BlockData; } set { this.BlockData = value; } }

	public FLACMetadataBlock(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBit(boxSize, readSize,  out this.LastMetadataBlockFlag, "LastMetadataBlockFlag"); 
		boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.BlockType, "BlockType"); 
		boxSize += stream.ReadUInt24(boxSize, readSize,  out this.Length, "Length"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(Length),  out this.BlockData, "BlockData"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBit( this.LastMetadataBlockFlag, "LastMetadataBlockFlag"); 
		boxSize += stream.WriteBits(7,  this.BlockType, "BlockType"); 
		boxSize += stream.WriteUInt24( this.Length, "Length"); 
		boxSize += stream.WriteUInt8Array((uint)(Length),  this.BlockData, "BlockData"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 1; // LastMetadataBlockFlag
		boxSize += 7; // BlockType
		boxSize += 24; // Length
		boxSize += ((ulong)(Length) * 8); // BlockData
		return boxSize;
	}
}

}
