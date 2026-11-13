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
aligned(8) class XtraValue() {
	unsigned int(32) length;
	unsigned int(16) type;
	unsigned int(8) value[length-6];
}
*/
public partial class XtraValue : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"aligned(8) class XtraValue() {
	unsigned int(32) length;
	unsigned int(16) type;
	unsigned int(8) value[length-6];
}"; } }
	public virtual string DisplayName { get { return "XtraValue"; } }

	protected uint length; 
	public uint Length { get { return this.length; } set { this.length = value; } }

	protected ushort type; 
	public ushort Type { get { return this.type; } set { this.type = value; } }

	protected byte[] value = []; 
	public byte[] Value { get { return this.value; } set { this.value = value; } }

	public XtraValue(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.length, "length"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.type, "type"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(length-6),  out this.value, "value"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt32( this.length, "length"); 
		boxSize += stream.WriteUInt16( this.type, "type"); 
		boxSize += stream.WriteUInt8Array((uint)(length-6),  this.value, "value"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 32; // length
		boxSize += 16; // type
		boxSize += ((ulong)(length-6) * 8); // value
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("length", 32, this.length); 
		boxSize += tracer.Field("type", 16, this.type); 
		boxSize += tracer.Field("value", ((ulong)(length-6) * 8), this.value); 
		return boxSize;
	}
}

}
