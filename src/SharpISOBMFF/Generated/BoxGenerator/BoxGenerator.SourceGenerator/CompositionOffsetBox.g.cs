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
aligned(8) class CompositionOffsetBox
	extends FullBox('ctts', version, 0) {
	unsigned int(32)	entry_count;
	int i;
	if (version==0) {
		for (i=0; i < entry_count; i++) {
			unsigned int(32)	sample_count;
			unsigned int(32)	sample_offset;
		}
	}
	else if (version == 1) {
		for (i=0; i < entry_count; i++) {
			unsigned int(32)	sample_count;
			signed   int(32)	sample_offset;
		}
	}
}
*/
public partial class CompositionOffsetBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class CompositionOffsetBox
	extends FullBox('ctts', version, 0) {
	unsigned int(32)	entry_count;
	int i;
	if (version==0) {
		for (i=0; i < entry_count; i++) {
			unsigned int(32)	sample_count;
			unsigned int(32)	sample_offset;
		}
	}
	else if (version == 1) {
		for (i=0; i < entry_count; i++) {
			unsigned int(32)	sample_count;
			signed   int(32)	sample_offset;
		}
	}
}"; } }
	public const string TYPE = "ctts";
	public override string DisplayName { get { return "CompositionOffsetBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stbl"); }


	protected uint entry_count; 
	public uint EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	protected uint[] sample_count; 
	public uint[] SampleCount { get { return this.sample_count; } set { this.sample_count = value; } }

	protected uint[] sample_offset; 
	public uint[] SampleOffset { get { return this.sample_offset; } set { this.sample_offset = value; } }

	protected int[] sample_offset0; 
	public int[] SampleOffset0 { get { return this.sample_offset0; } set { this.sample_offset0 = value; } }

	public CompositionOffsetBox(byte version = 0): base(IsoStream.FromFourCC("ctts"), version, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entry_count, "entry_count"); 
		

		if (version==0)
		{

			this.sample_count = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt( entry_count), "sample_count");
			this.sample_offset = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt( entry_count), "sample_offset");
			for (int i=0; i < entry_count; i++)
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_count[i], "sample_count"); 
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_offset[i], "sample_offset"); 
			}
		}

		else if (version == 1)
		{

			this.sample_offset0 = stream.SafeAllocate<int>(boxSize, readSize, IsoStream.GetInt( entry_count), "sample_offset0");
			this.sample_count = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt( entry_count), "sample_count");
			for (int i=0; i < entry_count; i++)
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_count[i], "sample_count"); 
				boxSize += stream.ReadInt32(boxSize, readSize,  out this.sample_offset0[i], "sample_offset0"); 
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.entry_count, "entry_count"); 
		

		if (version==0)
		{

			for (int i=0; i < entry_count; i++)
			{
				boxSize += stream.WriteUInt32( this.sample_count[i], "sample_count"); 
				boxSize += stream.WriteUInt32( this.sample_offset[i], "sample_offset"); 
			}
		}

		else if (version == 1)
		{

			for (int i=0; i < entry_count; i++)
			{
				boxSize += stream.WriteUInt32( this.sample_count[i], "sample_count"); 
				boxSize += stream.WriteInt32( this.sample_offset0[i], "sample_offset0"); 
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // entry_count
		

		if (version==0)
		{

			for (int i=0; i < entry_count; i++)
			{
				boxSize += 32; // sample_count
				boxSize += 32; // sample_offset
			}
		}

		else if (version == 1)
		{

			for (int i=0; i < entry_count; i++)
			{
				boxSize += 32; // sample_count
				boxSize += 32; // sample_offset0
			}
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("entry_count", 32, this.entry_count); 
		

		if (version==0)
		{

			for (int i=0; i < entry_count; i++)
			{
				boxSize += tracer.Field($"sample_count[{i}]", 32, this.sample_count[i]); 
				boxSize += tracer.Field($"sample_offset[{i}]", 32, this.sample_offset[i]); 
			}
		}

		else if (version == 1)
		{

			for (int i=0; i < entry_count; i++)
			{
				boxSize += tracer.Field($"sample_count[{i}]", 32, this.sample_count[i]); 
				boxSize += tracer.Field($"sample_offset0[{i}]", 32, this.sample_offset0[i]); 
			}
		}
		return boxSize;
	}
}

}
