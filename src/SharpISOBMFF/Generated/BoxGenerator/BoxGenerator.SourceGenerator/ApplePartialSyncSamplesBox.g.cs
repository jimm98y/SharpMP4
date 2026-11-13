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
aligned(8) class ApplePartialSyncSamplesBox() extends FullBox('stps', version = 0, 0) {
	unsigned int(32)	entry_count;
	int i;
	for (i=0; i < entry_count; i++) {
		unsigned int(32)	sample_number;
	}
}
*/
public partial class ApplePartialSyncSamplesBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class ApplePartialSyncSamplesBox() extends FullBox('stps', version = 0, 0) {
	unsigned int(32)	entry_count;
	int i;
	for (i=0; i < entry_count; i++) {
		unsigned int(32)	sample_number;
	}
}"; } }
	public const string TYPE = "stps";
	public override string DisplayName { get { return "ApplePartialSyncSamplesBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stbl"); }


	protected uint entry_count; 
	public uint EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	protected uint[] sample_number; 
	public uint[] SampleNumber { get { return this.sample_number; } set { this.sample_number = value; } }

	public ApplePartialSyncSamplesBox(): base(IsoStream.FromFourCC("stps"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entry_count, "entry_count"); 
		

		this.sample_number = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt( entry_count), "sample_number");
		for (int i=0; i < entry_count; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_number[i], "sample_number"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.entry_count, "entry_count"); 
		

		for (int i=0; i < entry_count; i++)
		{
			boxSize += stream.WriteUInt32( this.sample_number[i], "sample_number"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // entry_count
		

		for (int i=0; i < entry_count; i++)
		{
			boxSize += 32; // sample_number
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("entry_count", 32, this.entry_count); 
		

		for (int i=0; i < entry_count; i++)
		{
			boxSize += tracer.Field($"sample_number[{i}]", 32, this.sample_number[i]); 
		}
		return boxSize;
	}
}

}
