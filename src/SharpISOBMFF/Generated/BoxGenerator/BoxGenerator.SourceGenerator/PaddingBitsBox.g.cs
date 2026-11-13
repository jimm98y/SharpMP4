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
aligned(8) class PaddingBitsBox extends FullBox('padb', version = 0, 0) {
	unsigned int(32)	sample_count;
	int i;
	for (i=0; i < floor((sample_count + 1)/2); i++) {
		bit(1)	reserved = 0;
		bit(3)	pad1;
		bit(1)	reserved = 0;
		bit(3)	pad2;
	}
}
*/
public partial class PaddingBitsBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class PaddingBitsBox extends FullBox('padb', version = 0, 0) {
	unsigned int(32)	sample_count;
	int i;
	for (i=0; i < floor((sample_count + 1)/2); i++) {
		bit(1)	reserved = 0;
		bit(3)	pad1;
		bit(1)	reserved = 0;
		bit(3)	pad2;
	}
}"; } }
	public const string TYPE = "padb";
	public override string DisplayName { get { return "PaddingBitsBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stbl"); }


	protected uint sample_count; 
	public uint SampleCount { get { return this.sample_count; } set { this.sample_count = value; } }

	protected bool[] reserved; 
	public bool[] Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte[] pad1; 
	public byte[] Pad1 { get { return this.pad1; } set { this.pad1 = value; } }

	protected bool[] reserved0; 
	public bool[] Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected byte[] pad2; 
	public byte[] Pad2 { get { return this.pad2; } set { this.pad2 = value; } }

	public PaddingBitsBox(): base(IsoStream.FromFourCC("padb"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_count, "sample_count"); 
		

		this.reserved = stream.SafeAllocate<bool>(boxSize, readSize, IsoStream.GetInt( (int)Math.Floor((double)(sample_count + 1)/2)), "reserved");
		this.pad1 = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt( (int)Math.Floor((double)(sample_count + 1)/2)), "pad1");
		this.reserved0 = stream.SafeAllocate<bool>(boxSize, readSize, IsoStream.GetInt( (int)Math.Floor((double)(sample_count + 1)/2)), "reserved0");
		this.pad2 = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt( (int)Math.Floor((double)(sample_count + 1)/2)), "pad2");
		for (int i=0; i < (int)Math.Floor((double)(sample_count + 1)/2); i++)
		{
			boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved[i], "reserved"); 
			boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.pad1[i], "pad1"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved0[i], "reserved0"); 
			boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.pad2[i], "pad2"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.sample_count, "sample_count"); 
		

		for (int i=0; i < (int)Math.Floor((double)(sample_count + 1)/2); i++)
		{
			boxSize += stream.WriteBit( this.reserved[i], "reserved"); 
			boxSize += stream.WriteBits(3,  this.pad1[i], "pad1"); 
			boxSize += stream.WriteBit( this.reserved0[i], "reserved0"); 
			boxSize += stream.WriteBits(3,  this.pad2[i], "pad2"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // sample_count
		

		for (int i=0; i < (int)Math.Floor((double)(sample_count + 1)/2); i++)
		{
			boxSize += 1; // reserved
			boxSize += 3; // pad1
			boxSize += 1; // reserved0
			boxSize += 3; // pad2
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("sample_count", 32, this.sample_count); 
		

		for (int i=0; i < (int)Math.Floor((double)(sample_count + 1)/2); i++)
		{
			boxSize += tracer.Field($"reserved[{i}]", 1, this.reserved[i]); 
			boxSize += tracer.Field($"pad1[{i}]", 3, this.pad1[i]); 
			boxSize += tracer.Field($"reserved0[{i}]", 1, this.reserved0[i]); 
			boxSize += tracer.Field($"pad2[{i}]", 3, this.pad2[i]); 
		}
		return boxSize;
	}
}

}
