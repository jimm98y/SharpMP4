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
aligned(8) class PcmCBox() extends FullBox('pcmC') {
	unsigned int(8) formatFlags;
	unsigned int(8) pcmSampleSize;
}
*/
public partial class PcmCBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class PcmCBox() extends FullBox('pcmC') {
	unsigned int(8) formatFlags;
	unsigned int(8) pcmSampleSize;
}"; } }
	public const string TYPE = "pcmC";
	public override string DisplayName { get { return "PcmCBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("AudioSampleEntry(ipcm,fpcm)"); }


	protected byte formatFlags; 
	public byte FormatFlags { get { return this.formatFlags; } set { this.formatFlags = value; } }

	protected byte pcmSampleSize; 
	public byte PcmSampleSize { get { return this.pcmSampleSize; } set { this.pcmSampleSize = value; } }

	public PcmCBox(): base(IsoStream.FromFourCC("pcmC"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.formatFlags, "formatFlags"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.pcmSampleSize, "pcmSampleSize"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.formatFlags, "formatFlags"); 
		boxSize += stream.WriteUInt8( this.pcmSampleSize, "pcmSampleSize"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // formatFlags
		boxSize += 8; // pcmSampleSize
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("formatFlags", 8, this.formatFlags); 
		boxSize += tracer.Field("pcmSampleSize", 8, this.pcmSampleSize); 
		return boxSize;
	}
}

}
