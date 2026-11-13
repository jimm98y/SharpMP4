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
abstract aligned(8) class SampleEntry (unsigned int(32) format) extends Box(format){
	const unsigned int(8)[6] reservedSampleEntry = 0;
	unsigned int(16) data_reference_index;
}
*/
public abstract partial class SampleEntry : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"abstract aligned(8) class SampleEntry (unsigned int(32) format) extends Box(format){
	const unsigned int(8)[6] reservedSampleEntry = 0;
	unsigned int(16) data_reference_index;
}"; } }
	public override string DisplayName { get { return "SampleEntry"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn(BoxPlacements.Of(this) ?? new[] { "*" }); }


	protected byte[] reservedSampleEntry = new byte[6]; 
	public byte[] ReservedSampleEntry { get { return this.reservedSampleEntry; } set { this.reservedSampleEntry = value; } }

	protected ushort data_reference_index; 
	public ushort DataReferenceIndex { get { return this.data_reference_index; } set { this.data_reference_index = value; } }

	public SampleEntry(uint format): base(format)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 6,  out this.reservedSampleEntry, "reservedSampleEntry"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.data_reference_index, "data_reference_index"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8Array(6,  this.reservedSampleEntry, "reservedSampleEntry"); 
		boxSize += stream.WriteUInt16( this.data_reference_index, "data_reference_index"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 6 * 8; // reservedSampleEntry
		boxSize += 16; // data_reference_index
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("reservedSampleEntry", 6 * 8, this.reservedSampleEntry); 
		boxSize += tracer.Field("data_reference_index", 16, this.data_reference_index); 
		return boxSize;
	}
}

}
