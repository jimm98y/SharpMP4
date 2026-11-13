// SharpMP4
// Copyright (C) 2026 Lukas Volf
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
// SOFTWARE.

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
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"aligned(8) class FLACMetadataBlock {
	unsigned int(1) LastMetadataBlockFlag;
	unsigned int(7) BlockType;
	unsigned int(24) Length;
	unsigned int(8) BlockData[Length];
}"; } }
	public virtual string DisplayName { get { return "FLACMetadataBlock"; } }

	protected bool LastMetadataBlockFlag; 
	public bool _LastMetadataBlockFlag { get { return this.LastMetadataBlockFlag; } set { this.LastMetadataBlockFlag = value; } }

	protected byte BlockType; 
	public byte _BlockType { get { return this.BlockType; } set { this.BlockType = value; } }

	protected uint Length; 
	public uint _Length { get { return this.Length; } set { this.Length = value; } }

	protected byte[] BlockData = []; 
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

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("LastMetadataBlockFlag", 1, this.LastMetadataBlockFlag); 
		boxSize += tracer.Field("BlockType", 7, this.BlockType); 
		boxSize += tracer.Field("Length", 24, this.Length); 
		boxSize += tracer.Field("BlockData", ((ulong)(Length) * 8), this.BlockData); 
		return boxSize;
	}
}

}
