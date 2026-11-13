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
aligned(8) class SampleDependencyBox
	extends FullBox('sdep', version = 0, 0) {
	for (i=0; i < sample_count; i++){
		unsigned int(16) dependency_count;
		for (k=0; k < dependency_count; k++) {
			signed int(16) relative_sample_number;
		}
	}
}
*/
public partial class SampleDependencyBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class SampleDependencyBox
	extends FullBox('sdep', version = 0, 0) {
	for (i=0; i < sample_count; i++){
		unsigned int(16) dependency_count;
		for (k=0; k < dependency_count; k++) {
			signed int(16) relative_sample_number;
		}
	}
}"; } }
	public const string TYPE = "sdep";
	public override string DisplayName { get { return "SampleDependencyBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stbl"); }


	protected ushort[] dependency_count; 
	public ushort[] DependencyCount { get { return this.dependency_count; } set { this.dependency_count = value; } }

	protected short[][] relative_sample_number; 
	public short[][] RelativeSampleNumber { get { return this.relative_sample_number; } set { this.relative_sample_number = value; } }

	public SampleDependencyBox(): base(IsoStream.FromFourCC("sdep"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		int sample_count = 0; // TODO: taken from the stsz sample_count


		this.dependency_count = stream.SafeAllocate<ushort>(boxSize, readSize, IsoStream.GetInt( sample_count), "dependency_count");
		this.relative_sample_number = stream.SafeAllocate<short[]>(boxSize, readSize, IsoStream.GetInt( sample_count), "relative_sample_number");
		for (int i=0; i < sample_count; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.dependency_count[i], "dependency_count"); 

			this.relative_sample_number[i] = stream.SafeAllocate<short>(boxSize, readSize, IsoStream.GetInt( dependency_count[i]), "relative_sample_number[i]");
			for (int k=0; k < dependency_count[i]; k++)
			{
				boxSize += stream.ReadInt16(boxSize, readSize,  out this.relative_sample_number[i][k], "relative_sample_number"); 
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		int sample_count = 0; // TODO: taken from the stsz sample_count


		for (int i=0; i < sample_count; i++)
		{
			boxSize += stream.WriteUInt16( this.dependency_count[i], "dependency_count"); 

			for (int k=0; k < dependency_count[i]; k++)
			{
				boxSize += stream.WriteInt16( this.relative_sample_number[i][k], "relative_sample_number"); 
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		int sample_count = 0; // TODO: taken from the stsz sample_count


		for (int i=0; i < sample_count; i++)
		{
			boxSize += 16; // dependency_count

			for (int k=0; k < dependency_count[i]; k++)
			{
				boxSize += 16; // relative_sample_number
			}
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		int sample_count = 0; // TODO: taken from the stsz sample_count


		for (int i=0; i < sample_count; i++)
		{
			boxSize += tracer.Field($"dependency_count[{i}]", 16, this.dependency_count[i]); 

			for (int k=0; k < dependency_count[i]; k++)
			{
				boxSize += tracer.Field($"relative_sample_number[{i}][{k}]", 16, this.relative_sample_number[i][k]); 
			}
		}
		return boxSize;
	}
}

}
