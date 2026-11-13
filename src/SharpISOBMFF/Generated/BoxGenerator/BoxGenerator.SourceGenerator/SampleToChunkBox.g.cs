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
aligned(8) class SampleToChunkBox
	extends FullBox('stsc', version = 0, 0) {
	unsigned int(32)	entry_count;
	for (i=1; i <= entry_count; i++) {
		unsigned int(32)	first_chunk;
		unsigned int(32)	samples_per_chunk;
		unsigned int(32)	sample_description_index;
	}
}
*/
public partial class SampleToChunkBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class SampleToChunkBox
	extends FullBox('stsc', version = 0, 0) {
	unsigned int(32)	entry_count;
	for (i=1; i <= entry_count; i++) {
		unsigned int(32)	first_chunk;
		unsigned int(32)	samples_per_chunk;
		unsigned int(32)	sample_description_index;
	}
}"; } }
	public const string TYPE = "stsc";
	public override string DisplayName { get { return "SampleToChunkBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stbl"); }


	protected uint entry_count; 
	public uint EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	protected uint[] first_chunk; 
	public uint[] FirstChunk { get { return this.first_chunk; } set { this.first_chunk = value; } }

	protected uint[] samples_per_chunk; 
	public uint[] SamplesPerChunk { get { return this.samples_per_chunk; } set { this.samples_per_chunk = value; } }

	protected uint[] sample_description_index; 
	public uint[] SampleDescriptionIndex { get { return this.sample_description_index; } set { this.sample_description_index = value; } }

	public SampleToChunkBox(): base(IsoStream.FromFourCC("stsc"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entry_count, "entry_count"); 

		this.first_chunk = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt( entry_count), "first_chunk");
		this.samples_per_chunk = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt( entry_count), "samples_per_chunk");
		this.sample_description_index = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt( entry_count), "sample_description_index");
		for (int i=0; i < entry_count; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.first_chunk[i], "first_chunk"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.samples_per_chunk[i], "samples_per_chunk"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_description_index[i], "sample_description_index"); 
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
			boxSize += stream.WriteUInt32( this.first_chunk[i], "first_chunk"); 
			boxSize += stream.WriteUInt32( this.samples_per_chunk[i], "samples_per_chunk"); 
			boxSize += stream.WriteUInt32( this.sample_description_index[i], "sample_description_index"); 
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
			boxSize += 32; // first_chunk
			boxSize += 32; // samples_per_chunk
			boxSize += 32; // sample_description_index
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
			boxSize += tracer.Field($"first_chunk[{i}]", 32, this.first_chunk[i]); 
			boxSize += tracer.Field($"samples_per_chunk[{i}]", 32, this.samples_per_chunk[i]); 
			boxSize += tracer.Field($"sample_description_index[{i}]", 32, this.sample_description_index[i]); 
		}
		return boxSize;
	}
}

}
