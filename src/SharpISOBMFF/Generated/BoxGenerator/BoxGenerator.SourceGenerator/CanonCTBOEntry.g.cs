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
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"aligned(8) class CanonCTBOEntry() {
	unsigned int(32) index;
	unsigned int(64) offset;
	unsigned int(64) size;
}"; } }
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

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("index", 32, this.index); 
		boxSize += tracer.Field("offset", 64, this.offset); 
		boxSize += tracer.Field("size", 64, this.size); 
		return boxSize;
	}
}

}
