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
aligned(8) class WhiteBalanceBracketingEntry
	extends VisualSampleGroupEntry('wbbr') {
	unsigned int(16) blue_amber;
	int(8) green_magenta;
}
*/
public partial class WhiteBalanceBracketingEntry : VisualSampleGroupEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class WhiteBalanceBracketingEntry
	extends VisualSampleGroupEntry('wbbr') {
	unsigned int(16) blue_amber;
	int(8) green_magenta;
}"; } }
	public const string TYPE = "wbbr";
	public override string DisplayName { get { return "WhiteBalanceBracketingEntry"; } }

	protected ushort blue_amber; 
	public ushort BlueAmber { get { return this.blue_amber; } set { this.blue_amber = value; } }

	protected sbyte green_magenta; 
	public sbyte GreenMagenta { get { return this.green_magenta; } set { this.green_magenta = value; } }

	public WhiteBalanceBracketingEntry(): base(IsoStream.FromFourCC("wbbr"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.blue_amber, "blue_amber"); 
		boxSize += stream.ReadInt8(boxSize, readSize,  out this.green_magenta, "green_magenta"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.blue_amber, "blue_amber"); 
		boxSize += stream.WriteInt8( this.green_magenta, "green_magenta"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // blue_amber
		boxSize += 8; // green_magenta
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("blue_amber", 16, this.blue_amber); 
		boxSize += tracer.Field("green_magenta", 8, this.green_magenta); 
		return boxSize;
	}
}

}
