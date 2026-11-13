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
aligned(8) class AdobeChapterRecord() {
	unsigned int(64) timestamp;
	unsigned int(8) count;
	char title[count];
}
*/
public partial class AdobeChapterRecord : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"aligned(8) class AdobeChapterRecord() {
	unsigned int(64) timestamp;
	unsigned int(8) count;
	char title[count];
}"; } }
	public virtual string DisplayName { get { return "AdobeChapterRecord"; } }

	protected ulong timestamp; 
	public ulong Timestamp { get { return this.timestamp; } set { this.timestamp = value; } }

	protected byte count; 
	public byte Count { get { return this.count; } set { this.count = value; } }

	protected byte[] title = []; 
	public byte[] Title { get { return this.title; } set { this.title = value; } }

	public AdobeChapterRecord(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt64(boxSize, readSize,  out this.timestamp, "timestamp"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.count, "count"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(count),  out this.title, "title"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt64( this.timestamp, "timestamp"); 
		boxSize += stream.WriteUInt8( this.count, "count"); 
		boxSize += stream.WriteUInt8Array((uint)(count),  this.title, "title"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 64; // timestamp
		boxSize += 8; // count
		boxSize += ((ulong)(count) * 8); // title
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("timestamp", 64, this.timestamp); 
		boxSize += tracer.Field("count", 8, this.count); 
		boxSize += tracer.Field("title", ((ulong)(count) * 8), this.title); 
		return boxSize;
	}
}

}
