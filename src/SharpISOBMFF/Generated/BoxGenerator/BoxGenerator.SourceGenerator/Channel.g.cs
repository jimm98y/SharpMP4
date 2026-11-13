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
aligned(8) class Channel() {
	unsigned int(16) channel_index;
	unsigned int(16) channel_type;
	unsigned int(16) channel_association;
}
*/
public partial class Channel : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"aligned(8) class Channel() {
	unsigned int(16) channel_index;
	unsigned int(16) channel_type;
	unsigned int(16) channel_association;
}"; } }
	public virtual string DisplayName { get { return "Channel"; } }

	protected ushort channel_index; 
	public ushort ChannelIndex { get { return this.channel_index; } set { this.channel_index = value; } }

	protected ushort channel_type; 
	public ushort ChannelType { get { return this.channel_type; } set { this.channel_type = value; } }

	protected ushort channel_association; 
	public ushort ChannelAssociation { get { return this.channel_association; } set { this.channel_association = value; } }

	public Channel(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.channel_index, "channel_index"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.channel_type, "channel_type"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.channel_association, "channel_association"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt16( this.channel_index, "channel_index"); 
		boxSize += stream.WriteUInt16( this.channel_type, "channel_type"); 
		boxSize += stream.WriteUInt16( this.channel_association, "channel_association"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 16; // channel_index
		boxSize += 16; // channel_type
		boxSize += 16; // channel_association
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("channel_index", 16, this.channel_index); 
		boxSize += tracer.Field("channel_type", 16, this.channel_type); 
		boxSize += tracer.Field("channel_association", 16, this.channel_association); 
		return boxSize;
	}
}

}
