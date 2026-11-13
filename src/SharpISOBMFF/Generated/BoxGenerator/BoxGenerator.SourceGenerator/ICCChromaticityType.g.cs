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
aligned(8) class ICCChromaticityType {
	unsigned int(32) type_signature;
	unsigned int(32) reserved = 0;
	unsigned int(16) channel_count;
	unsigned int(16) phosphor_or_colorant_type;
	for (i = 0; i < channel_count; i++) {
		unsigned int(32) x;	// u16Fixed16Number
		unsigned int(32) y;
	}
}
*/
public partial class ICCChromaticityType : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"aligned(8) class ICCChromaticityType {
	unsigned int(32) type_signature;
	unsigned int(32) reserved = 0;
	unsigned int(16) channel_count;
	unsigned int(16) phosphor_or_colorant_type;
	for (i = 0; i < channel_count; i++) {
		unsigned int(32) x;	// u16Fixed16Number
		unsigned int(32) y;
	}
}"; } }
	public virtual string DisplayName { get { return "ICCChromaticityType"; } }

	protected uint type_signature; 
	public uint TypeSignature { get { return this.type_signature; } set { this.type_signature = value; } }

	protected uint reserved = 0; 
	public uint Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected ushort channel_count; 
	public ushort ChannelCount { get { return this.channel_count; } set { this.channel_count = value; } }

	protected ushort phosphor_or_colorant_type; 
	public ushort PhosphorOrColorantType { get { return this.phosphor_or_colorant_type; } set { this.phosphor_or_colorant_type = value; } }

	protected uint[] x;  //  u16Fixed16Number
	public uint[] _x { get { return this.x; } set { this.x = value; } }

	protected uint[] y; 
	public uint[] _y { get { return this.y; } set { this.y = value; } }

	public ICCChromaticityType(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.type_signature, "type_signature"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.channel_count, "channel_count"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.phosphor_or_colorant_type, "phosphor_or_colorant_type"); 

		this.x = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt( channel_count), "x");
		this.y = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt( channel_count), "y");
		for (int i = 0; i < channel_count; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.x[i], "x"); // u16Fixed16Number
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.y[i], "y"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt32( this.type_signature, "type_signature"); 
		boxSize += stream.WriteUInt32( this.reserved, "reserved"); 
		boxSize += stream.WriteUInt16( this.channel_count, "channel_count"); 
		boxSize += stream.WriteUInt16( this.phosphor_or_colorant_type, "phosphor_or_colorant_type"); 

		for (int i = 0; i < channel_count; i++)
		{
			boxSize += stream.WriteUInt32( this.x[i], "x"); // u16Fixed16Number
			boxSize += stream.WriteUInt32( this.y[i], "y"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 32; // type_signature
		boxSize += 32; // reserved
		boxSize += 16; // channel_count
		boxSize += 16; // phosphor_or_colorant_type

		for (int i = 0; i < channel_count; i++)
		{
			boxSize += 32; // x
			boxSize += 32; // y
		}
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("type_signature", 32, this.type_signature); 
		boxSize += tracer.Field("reserved", 32, this.reserved); 
		boxSize += tracer.Field("channel_count", 16, this.channel_count); 
		boxSize += tracer.Field("phosphor_or_colorant_type", 16, this.phosphor_or_colorant_type); 

		for (int i = 0; i < channel_count; i++)
		{
			boxSize += tracer.Field($"x[{i}]", 32, this.x[i]); // u16Fixed16Number
			boxSize += tracer.Field($"y[{i}]", 32, this.y[i]); 
		}
		return boxSize;
	}
}

}
