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
aligned(8) class CopyrightBox
	extends FullBox('cprt', version = 0, 0) {
	const bit(1) pad = 0;
	unsigned int(5)[3] language; // ISO-639-2/T language code
	utfstring notice;
}
*/
public partial class CopyrightBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class CopyrightBox
	extends FullBox('cprt', version = 0, 0) {
	const bit(1) pad = 0;
	unsigned int(5)[3] language; // ISO-639-2/T language code
	utfstring notice;
}"; } }
	public const string TYPE = "cprt";
	public override string DisplayName { get { return "CopyrightBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("udta"); }


	protected bool pad = false; 
	public bool Pad { get { return this.pad; } set { this.pad = value; } }

	protected string language = "und";  //  ISO-639-2/T language code
	public string Language { get { return this.language; } set { this.language = value; } }

	protected BinaryUTF8String notice; 
	public BinaryUTF8String Notice { get { return this.notice; } set { this.notice = value; } }

	public CopyrightBox(): base(IsoStream.FromFourCC("cprt"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.pad, "pad"); 
		boxSize += stream.ReadIso639(boxSize, readSize,  out this.language, "language"); // ISO-639-2/T language code
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.notice, "notice"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBit( this.pad, "pad"); 
		boxSize += stream.WriteIso639( this.language, "language"); // ISO-639-2/T language code
		boxSize += stream.WriteStringZeroTerminated( this.notice, "notice"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 1; // pad
		boxSize += 15; // language
		boxSize += IsoStream.CalculateStringSize(notice); // notice
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("pad", 1, this.pad); 
		boxSize += tracer.Field("language", 15, this.language); // ISO-639-2/T language code
		boxSize += tracer.Field("notice", IsoStream.CalculateStringSize(notice), this.notice); 
		return boxSize;
	}
}

}
