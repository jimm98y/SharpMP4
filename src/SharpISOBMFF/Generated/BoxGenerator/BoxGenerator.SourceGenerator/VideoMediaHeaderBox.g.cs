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
aligned(8) class VideoMediaHeaderBox
	extends FullBox('vmhd', version = 0, 1) {
	template unsigned int(16)		graphicsmode = 0;	// copy, see below
	template unsigned int(16)[3]	opcolor = {0, 0, 0};
}
*/
public partial class VideoMediaHeaderBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class VideoMediaHeaderBox
	extends FullBox('vmhd', version = 0, 1) {
	template unsigned int(16)		graphicsmode = 0;	// copy, see below
	template unsigned int(16)[3]	opcolor = {0, 0, 0};
}"; } }
	public const string TYPE = "vmhd";
	public override string DisplayName { get { return "VideoMediaHeaderBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("minf"); }


	protected ushort graphicsmode = 0;  //  copy, see below
	public ushort Graphicsmode { get { return this.graphicsmode; } set { this.graphicsmode = value; } }

	protected ushort[] opcolor = {0, 0, 0}; 
	public ushort[] Opcolor { get { return this.opcolor; } set { this.opcolor = value; } }

	public VideoMediaHeaderBox(): base(IsoStream.FromFourCC("vmhd"), 0, 1)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.graphicsmode, "graphicsmode"); // copy, see below
		boxSize += stream.ReadUInt16Array(boxSize, readSize, 3,  out this.opcolor, "opcolor"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.graphicsmode, "graphicsmode"); // copy, see below
		boxSize += stream.WriteUInt16Array(3,  this.opcolor, "opcolor"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // graphicsmode
		boxSize += 3 * 16; // opcolor
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("graphicsmode", 16, this.graphicsmode); // copy, see below
		boxSize += tracer.Field("opcolor", 3 * 16, this.opcolor); 
		return boxSize;
	}
}

}
