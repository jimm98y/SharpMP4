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
class VisualEquivalenceEntry() extends VisualSampleGroupEntry ('eqiv')
{
	signed int(16)   time_offset;
	unsigned int(16) timescale_multiplier;
}
*/
public partial class VisualEquivalenceEntry : VisualSampleGroupEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class VisualEquivalenceEntry() extends VisualSampleGroupEntry ('eqiv')
{
	signed int(16)   time_offset;
	unsigned int(16) timescale_multiplier;
}"; } }
	public const string TYPE = "eqiv";
	public override string DisplayName { get { return "VisualEquivalenceEntry"; } }

	protected short time_offset; 
	public short TimeOffset { get { return this.time_offset; } set { this.time_offset = value; } }

	protected ushort timescale_multiplier; 
	public ushort TimescaleMultiplier { get { return this.timescale_multiplier; } set { this.timescale_multiplier = value; } }

	public VisualEquivalenceEntry(): base(IsoStream.FromFourCC("eqiv"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.time_offset, "time_offset"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.timescale_multiplier, "timescale_multiplier"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt16( this.time_offset, "time_offset"); 
		boxSize += stream.WriteUInt16( this.timescale_multiplier, "timescale_multiplier"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // time_offset
		boxSize += 16; // timescale_multiplier
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("time_offset", 16, this.time_offset); 
		boxSize += tracer.Field("timescale_multiplier", 16, this.timescale_multiplier); 
		return boxSize;
	}
}

}
