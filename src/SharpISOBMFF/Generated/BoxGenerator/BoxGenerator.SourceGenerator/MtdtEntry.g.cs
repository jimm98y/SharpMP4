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
aligned(8) class MtdtEntry() {
	unsigned int(16) size;
	unsigned int(32) type;
	bit(1) reserved;
	unsigned int(5)[3] language;
	unsigned int(16) dataType;
	bit(8) data[size-10];
}
*/
public partial class MtdtEntry : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"aligned(8) class MtdtEntry() {
	unsigned int(16) size;
	unsigned int(32) type;
	bit(1) reserved;
	unsigned int(5)[3] language;
	unsigned int(16) dataType;
	bit(8) data[size-10];
}"; } }
	public virtual string DisplayName { get { return "MtdtEntry"; } }

	protected ushort size; 
	public ushort Size { get { return this.size; } set { this.size = value; } }

	protected uint type; 
	public uint Type { get { return this.type; } set { this.type = value; } }

	protected bool reserved; 
	public bool Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected string language = "und"; 
	public string Language { get { return this.language; } set { this.language = value; } }

	protected ushort dataType; 
	public ushort DataType { get { return this.dataType; } set { this.dataType = value; } }

	protected byte[] data = []; 
	public byte[] Data { get { return this.data; } set { this.data = value; } }

	public MtdtEntry(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.size, "size"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.type, "type"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadIso639(boxSize, readSize,  out this.language, "language"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.dataType, "dataType"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(size-10),  out this.data, "data"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt16( this.size, "size"); 
		boxSize += stream.WriteUInt32( this.type, "type"); 
		boxSize += stream.WriteBit( this.reserved, "reserved"); 
		boxSize += stream.WriteIso639( this.language, "language"); 
		boxSize += stream.WriteUInt16( this.dataType, "dataType"); 
		boxSize += stream.WriteUInt8Array((uint)(size-10),  this.data, "data"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 16; // size
		boxSize += 32; // type
		boxSize += 1; // reserved
		boxSize += 15; // language
		boxSize += 16; // dataType
		boxSize += ((ulong)(size-10) * 8); // data
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("size", 16, this.size); 
		boxSize += tracer.Field("type", 32, this.type); 
		boxSize += tracer.Field("reserved", 1, this.reserved); 
		boxSize += tracer.Field("language", 15, this.language); 
		boxSize += tracer.Field("dataType", 16, this.dataType); 
		boxSize += tracer.Field("data", ((ulong)(size-10) * 8), this.data); 
		return boxSize;
	}
}

}
