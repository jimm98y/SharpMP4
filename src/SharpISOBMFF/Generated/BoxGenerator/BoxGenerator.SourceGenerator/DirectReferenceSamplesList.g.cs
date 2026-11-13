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
class DirectReferenceSamplesList()
	extends VisualSampleGroupEntry ('refs') {
	unsigned int(32) sample_id;
	unsigned int(8) num_direct_reference_samples;
	for(i = 0; i < num_direct_reference_samples; i++) {
		unsigned int(32)direct_reference_sample_id;
	}
}
*/
public partial class DirectReferenceSamplesList : VisualSampleGroupEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class DirectReferenceSamplesList()
	extends VisualSampleGroupEntry ('refs') {
	unsigned int(32) sample_id;
	unsigned int(8) num_direct_reference_samples;
	for(i = 0; i < num_direct_reference_samples; i++) {
		unsigned int(32)direct_reference_sample_id;
	}
}"; } }
	public const string TYPE = "refs";
	public override string DisplayName { get { return "DirectReferenceSamplesList"; } }

	protected uint sample_id; 
	public uint SampleId { get { return this.sample_id; } set { this.sample_id = value; } }

	protected byte num_direct_reference_samples; 
	public byte NumDirectReferenceSamples { get { return this.num_direct_reference_samples; } set { this.num_direct_reference_samples = value; } }

	protected uint[] direct_reference_sample_id; 
	public uint[] DirectReferenceSampleId { get { return this.direct_reference_sample_id; } set { this.direct_reference_sample_id = value; } }

	public DirectReferenceSamplesList(): base(IsoStream.FromFourCC("refs"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_id, "sample_id"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.num_direct_reference_samples, "num_direct_reference_samples"); 

		this.direct_reference_sample_id = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt( num_direct_reference_samples), "direct_reference_sample_id");
		for (int i = 0; i < num_direct_reference_samples; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.direct_reference_sample_id[i], "direct_reference_sample_id"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.sample_id, "sample_id"); 
		boxSize += stream.WriteUInt8( this.num_direct_reference_samples, "num_direct_reference_samples"); 

		for (int i = 0; i < num_direct_reference_samples; i++)
		{
			boxSize += stream.WriteUInt32( this.direct_reference_sample_id[i], "direct_reference_sample_id"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // sample_id
		boxSize += 8; // num_direct_reference_samples

		for (int i = 0; i < num_direct_reference_samples; i++)
		{
			boxSize += 32; // direct_reference_sample_id
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("sample_id", 32, this.sample_id); 
		boxSize += tracer.Field("num_direct_reference_samples", 8, this.num_direct_reference_samples); 

		for (int i = 0; i < num_direct_reference_samples; i++)
		{
			boxSize += tracer.Field($"direct_reference_sample_id[{i}]", 32, this.direct_reference_sample_id[i]); 
		}
		return boxSize;
	}
}

}
