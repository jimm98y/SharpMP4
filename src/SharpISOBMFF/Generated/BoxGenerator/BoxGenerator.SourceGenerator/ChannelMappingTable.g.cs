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
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"class ChannelMappingTable (unsigned int(8) OutputChannelCount){
	unsigned int(8) StreamCount;
	unsigned int(8) CoupledCount;
	unsigned int(8 * OutputChannelCount) ChannelMapping;
}"; } }
	public virtual string DisplayName { get { return "ChannelMappingTable"; } }

	protected byte StreamCount; 
	public byte _StreamCount { get { return this.StreamCount; } set { this.StreamCount = value; } }

	protected byte CoupledCount; 
	public byte _CoupledCount { get { return this.CoupledCount; } set { this.CoupledCount = value; } }

	protected byte[] ChannelMapping = []; 
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

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("StreamCount", 8, this.StreamCount); 
		boxSize += tracer.Field("CoupledCount", 8, this.CoupledCount); 
		boxSize += tracer.Field("ChannelMapping", (ulong)(8 * OutputChannelCount ), this.ChannelMapping); 
		return boxSize;
	}
}

}
