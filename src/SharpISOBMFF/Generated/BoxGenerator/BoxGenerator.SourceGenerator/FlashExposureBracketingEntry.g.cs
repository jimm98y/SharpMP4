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
aligned(8) class FlashExposureBracketingEntry
	extends VisualSampleGroupEntry('afbr') {
	int(8) flash_exposure_numerator;
	int(8) flash_exposure_denominator;
}
*/
public partial class FlashExposureBracketingEntry : VisualSampleGroupEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class FlashExposureBracketingEntry
	extends VisualSampleGroupEntry('afbr') {
	int(8) flash_exposure_numerator;
	int(8) flash_exposure_denominator;
}"; } }
	public const string TYPE = "afbr";
	public override string DisplayName { get { return "FlashExposureBracketingEntry"; } }

	protected sbyte flash_exposure_numerator; 
	public sbyte FlashExposureNumerator { get { return this.flash_exposure_numerator; } set { this.flash_exposure_numerator = value; } }

	protected sbyte flash_exposure_denominator; 
	public sbyte FlashExposureDenominator { get { return this.flash_exposure_denominator; } set { this.flash_exposure_denominator = value; } }

	public FlashExposureBracketingEntry(): base(IsoStream.FromFourCC("afbr"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt8(boxSize, readSize,  out this.flash_exposure_numerator, "flash_exposure_numerator"); 
		boxSize += stream.ReadInt8(boxSize, readSize,  out this.flash_exposure_denominator, "flash_exposure_denominator"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt8( this.flash_exposure_numerator, "flash_exposure_numerator"); 
		boxSize += stream.WriteInt8( this.flash_exposure_denominator, "flash_exposure_denominator"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // flash_exposure_numerator
		boxSize += 8; // flash_exposure_denominator
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("flash_exposure_numerator", 8, this.flash_exposure_numerator); 
		boxSize += tracer.Field("flash_exposure_denominator", 8, this.flash_exposure_denominator); 
		return boxSize;
	}
}

}
