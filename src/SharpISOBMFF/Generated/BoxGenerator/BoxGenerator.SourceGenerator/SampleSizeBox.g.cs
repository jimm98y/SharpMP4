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
aligned(8) class SampleSizeBox extends FullBox('stsz', version = 0, 0) {
	unsigned int(32)	sample_size;
	unsigned int(32)	sample_count;
	if (sample_size==0) {
		for (i=1; i <= sample_count; i++) {
			unsigned int(32)	entry_size;
		}
	}
}
*/
public partial class SampleSizeBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class SampleSizeBox extends FullBox('stsz', version = 0, 0) {
	unsigned int(32)	sample_size;
	unsigned int(32)	sample_count;
	if (sample_size==0) {
		for (i=1; i <= sample_count; i++) {
			unsigned int(32)	entry_size;
		}
	}
}"; } }
	public const string TYPE = "stsz";
	public override string DisplayName { get { return "SampleSizeBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stbl"); }


	protected uint sample_size; 
	public uint SampleSize { get { return this.sample_size; } set { this.sample_size = value; } }

	protected uint sample_count; 
	public uint SampleCount { get { return this.sample_count; } set { this.sample_count = value; } }

	protected uint[] entry_size; 
	public uint[] EntrySize { get { return this.entry_size; } set { this.entry_size = value; } }

	public SampleSizeBox(): base(IsoStream.FromFourCC("stsz"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_size, "sample_size"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_count, "sample_count"); 

		if (sample_size==0)
		{

			this.entry_size = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt( sample_count), "entry_size");
			for (int i=0; i < sample_count; i++)
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entry_size[i], "entry_size"); 
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.sample_size, "sample_size"); 
		boxSize += stream.WriteUInt32( this.sample_count, "sample_count"); 

		if (sample_size==0)
		{

			for (int i=0; i < sample_count; i++)
			{
				boxSize += stream.WriteUInt32( this.entry_size[i], "entry_size"); 
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // sample_size
		boxSize += 32; // sample_count

		if (sample_size==0)
		{

			for (int i=0; i < sample_count; i++)
			{
				boxSize += 32; // entry_size
			}
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("sample_size", 32, this.sample_size); 
		boxSize += tracer.Field("sample_count", 32, this.sample_count); 

		if (sample_size==0)
		{

			for (int i=0; i < sample_count; i++)
			{
				boxSize += tracer.Field($"entry_size[{i}]", 32, this.entry_size[i]); 
			}
		}
		return boxSize;
	}
}

}
