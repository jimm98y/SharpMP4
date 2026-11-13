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
// Canon.pm, CCTP: 12 bytes, then a CCDT for each image track
aligned(8) class CanonCCTPBox() extends Box('CCTP') {
	unsigned int(32) reserved;
	unsigned int(32) unknown;
	unsigned int(32) entryCount;
	Box boxes[];
}
*/
public partial class CanonCCTPBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"// Canon.pm, CCTP: 12 bytes, then a CCDT for each image track
aligned(8) class CanonCCTPBox() extends Box('CCTP') {
	unsigned int(32) reserved;
	unsigned int(32) unknown;
	unsigned int(32) entryCount;
	Box boxes[];
}"; } }
	public const string TYPE = "CCTP";
	public override string DisplayName { get { return "CanonCCTPBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("uuid"); }


	protected uint reserved; 
	public uint Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected uint unknown; 
	public uint Unknown { get { return this.unknown; } set { this.unknown = value; } }

	protected uint entryCount; 
	public uint EntryCount { get { return this.entryCount; } set { this.entryCount = value; } }

	public CanonCCTPBox(): base(IsoStream.FromFourCC("CCTP"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.unknown, "unknown"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entryCount, "entryCount"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.boxes, "boxes"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.reserved, "reserved"); 
		boxSize += stream.WriteUInt32( this.unknown, "unknown"); 
		boxSize += stream.WriteUInt32( this.entryCount, "entryCount"); 
		// boxSize += stream.WriteBox( this.boxes, "boxes"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // reserved
		boxSize += 32; // unknown
		boxSize += 32; // entryCount
		// boxSize += IsoStream.CalculateBoxSize(boxes); // boxes
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("reserved", 32, this.reserved); 
		boxSize += tracer.Field("unknown", 32, this.unknown); 
		boxSize += tracer.Field("entryCount", 32, this.entryCount); 
		// boxSize += tracer.Field("boxes", IsoStream.CalculateBoxSize(boxes), this.boxes); 
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
