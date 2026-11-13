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
aligned(8) class XtraTag() {
	unsigned int(32) inputSize;
	unsigned int(32) tagLength;
	char tagName[tagLength];
	unsigned int(32) count;
	XtraValue values[count];
}
*/
public partial class XtraTag : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"aligned(8) class XtraTag() {
	unsigned int(32) inputSize;
	unsigned int(32) tagLength;
	char tagName[tagLength];
	unsigned int(32) count;
	XtraValue values[count];
}"; } }
	public virtual string DisplayName { get { return "XtraTag"; } }

	protected uint inputSize; 
	public uint InputSize { get { return this.inputSize; } set { this.inputSize = value; } }

	protected uint tagLength; 
	public uint TagLength { get { return this.tagLength; } set { this.tagLength = value; } }

	protected byte[] tagName = []; 
	public byte[] TagName { get { return this.tagName; } set { this.tagName = value; } }

	protected uint count; 
	public uint Count { get { return this.count; } set { this.count = value; } }

	protected XtraValue[] values = []; 
	public XtraValue[] Values { get { return this.values; } set { this.values = value; } }

	public XtraTag(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.inputSize, "inputSize"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.tagLength, "tagLength"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(tagLength),  out this.tagName, "tagName"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.count, "count"); 
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(count), () => new XtraValue(),  out this.values, "values"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt32( this.inputSize, "inputSize"); 
		boxSize += stream.WriteUInt32( this.tagLength, "tagLength"); 
		boxSize += stream.WriteUInt8Array((uint)(tagLength),  this.tagName, "tagName"); 
		boxSize += stream.WriteUInt32( this.count, "count"); 
		boxSize += stream.WriteClass( this.values, "values"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 32; // inputSize
		boxSize += 32; // tagLength
		boxSize += ((ulong)(tagLength) * 8); // tagName
		boxSize += 32; // count
		boxSize += IsoStream.CalculateClassSize(values); // values
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("inputSize", 32, this.inputSize); 
		boxSize += tracer.Field("tagLength", 32, this.tagLength); 
		boxSize += tracer.Field("tagName", ((ulong)(tagLength) * 8), this.tagName); 
		boxSize += tracer.Field("count", 32, this.count); 
		boxSize += tracer.Field("values", IsoStream.CalculateClassSize(values), this.values); 
		return boxSize;
	}
}

}
