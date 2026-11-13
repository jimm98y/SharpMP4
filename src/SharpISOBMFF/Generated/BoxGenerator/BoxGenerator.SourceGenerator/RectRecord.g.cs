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
aligned(8) class RectRecord() {
	unsigned int(16) top;
	unsigned int(16) left;
	unsigned int(16) bottom;
	unsigned int(16) right;
}
*/
public partial class RectRecord : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"aligned(8) class RectRecord() {
	unsigned int(16) top;
	unsigned int(16) left;
	unsigned int(16) bottom;
	unsigned int(16) right;
}"; } }
	public virtual string DisplayName { get { return "RectRecord"; } }

	protected ushort top; 
	public ushort Top { get { return this.top; } set { this.top = value; } }

	protected ushort left; 
	public ushort Left { get { return this.left; } set { this.left = value; } }

	protected ushort bottom; 
	public ushort Bottom { get { return this.bottom; } set { this.bottom = value; } }

	protected ushort right; 
	public ushort Right { get { return this.right; } set { this.right = value; } }

	public RectRecord(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.top, "top"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.left, "left"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.bottom, "bottom"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.right, "right"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt16( this.top, "top"); 
		boxSize += stream.WriteUInt16( this.left, "left"); 
		boxSize += stream.WriteUInt16( this.bottom, "bottom"); 
		boxSize += stream.WriteUInt16( this.right, "right"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 16; // top
		boxSize += 16; // left
		boxSize += 16; // bottom
		boxSize += 16; // right
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("top", 16, this.top); 
		boxSize += tracer.Field("left", 16, this.left); 
		boxSize += tracer.Field("bottom", 16, this.bottom); 
		boxSize += tracer.Field("right", 16, this.right); 
		return boxSize;
	}
}

}
