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
aligned(8) class DegradationPriorityBox
	extends FullBox('stdp', version = 0, 0) {
	int i;
	for (i=0; i < sample_count; i++) {
		unsigned int(16)	priority;
	}
}
*/
public partial class DegradationPriorityBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class DegradationPriorityBox
	extends FullBox('stdp', version = 0, 0) {
	int i;
	for (i=0; i < sample_count; i++) {
		unsigned int(16)	priority;
	}
}"; } }
	public const string TYPE = "stdp";
	public override string DisplayName { get { return "DegradationPriorityBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stbl"); }


	protected ushort[] priority; 
	public ushort[] Priority { get { return this.priority; } set { this.priority = value; } }

	public DegradationPriorityBox(): base(IsoStream.FromFourCC("stdp"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		int sample_count = (int)((readSize - boxSize) >> 4); // should be taken from the stsz sample_count, but we can calculate it from the readSize - 2 bytes per sample

		

		this.priority = stream.SafeAllocate<ushort>(boxSize, readSize, IsoStream.GetInt( sample_count), "priority");
		for (int i=0; i < sample_count; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.priority[i], "priority"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		int sample_count = priority.Length;

		

		for (int i=0; i < sample_count; i++)
		{
			boxSize += stream.WriteUInt16( this.priority[i], "priority"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		int sample_count = priority.Length;

		

		for (int i=0; i < sample_count; i++)
		{
			boxSize += 16; // priority
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		int sample_count = priority.Length;

		

		for (int i=0; i < sample_count; i++)
		{
			boxSize += tracer.Field($"priority[{i}]", 16, this.priority[i]); 
		}
		return boxSize;
	}
}

}
