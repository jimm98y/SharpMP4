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
class VisualRandomAccessEntry() extends VisualSampleGroupEntry ('rap ')
{
	unsigned int(1) num_leading_samples_known;
	unsigned int(7) num_leading_samples;
}
*/
public partial class VisualRandomAccessEntry : VisualSampleGroupEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class VisualRandomAccessEntry() extends VisualSampleGroupEntry ('rap ')
{
	unsigned int(1) num_leading_samples_known;
	unsigned int(7) num_leading_samples;
}"; } }
	public const string TYPE = "rap ";
	public override string DisplayName { get { return "VisualRandomAccessEntry"; } }

	protected bool num_leading_samples_known; 
	public bool NumLeadingSamplesKnown { get { return this.num_leading_samples_known; } set { this.num_leading_samples_known = value; } }

	protected byte num_leading_samples; 
	public byte NumLeadingSamples { get { return this.num_leading_samples; } set { this.num_leading_samples = value; } }

	public VisualRandomAccessEntry(): base(IsoStream.FromFourCC("rap "))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.num_leading_samples_known, "num_leading_samples_known"); 
		boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.num_leading_samples, "num_leading_samples"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBit( this.num_leading_samples_known, "num_leading_samples_known"); 
		boxSize += stream.WriteBits(7,  this.num_leading_samples, "num_leading_samples"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 1; // num_leading_samples_known
		boxSize += 7; // num_leading_samples
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("num_leading_samples_known", 1, this.num_leading_samples_known); 
		boxSize += tracer.Field("num_leading_samples", 7, this.num_leading_samples); 
		return boxSize;
	}
}

}
