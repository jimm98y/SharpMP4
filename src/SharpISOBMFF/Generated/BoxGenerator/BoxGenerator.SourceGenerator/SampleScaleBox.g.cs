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
aligned(8) class SampleScaleBox() extends FullBox('stsl', version = 0, 0) {
	bit(7)	reserved;
	bit(1) constrained;
	unsigned int(8) scaleMethod;
	signed int(16) displayCenterX;
	signed int(16) displayCenterY;
}
*/
public partial class SampleScaleBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class SampleScaleBox() extends FullBox('stsl', version = 0, 0) {
	bit(7)	reserved;
	bit(1) constrained;
	unsigned int(8) scaleMethod;
	signed int(16) displayCenterX;
	signed int(16) displayCenterY;
}"; } }
	public const string TYPE = "stsl";
	public override string DisplayName { get { return "SampleScaleBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("VisualSampleEntry"); }


	protected byte reserved; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected bool constrained; 
	public bool Constrained { get { return this.constrained; } set { this.constrained = value; } }

	protected byte scaleMethod; 
	public byte ScaleMethod { get { return this.scaleMethod; } set { this.scaleMethod = value; } }

	protected short displayCenterX; 
	public short DisplayCenterX { get { return this.displayCenterX; } set { this.displayCenterX = value; } }

	protected short displayCenterY; 
	public short DisplayCenterY { get { return this.displayCenterY; } set { this.displayCenterY = value; } }

	public SampleScaleBox(): base(IsoStream.FromFourCC("stsl"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.constrained, "constrained"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.scaleMethod, "scaleMethod"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.displayCenterX, "displayCenterX"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.displayCenterY, "displayCenterY"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(7,  this.reserved, "reserved"); 
		boxSize += stream.WriteBit( this.constrained, "constrained"); 
		boxSize += stream.WriteUInt8( this.scaleMethod, "scaleMethod"); 
		boxSize += stream.WriteInt16( this.displayCenterX, "displayCenterX"); 
		boxSize += stream.WriteInt16( this.displayCenterY, "displayCenterY"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 7; // reserved
		boxSize += 1; // constrained
		boxSize += 8; // scaleMethod
		boxSize += 16; // displayCenterX
		boxSize += 16; // displayCenterY
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("reserved", 7, this.reserved); 
		boxSize += tracer.Field("constrained", 1, this.constrained); 
		boxSize += tracer.Field("scaleMethod", 8, this.scaleMethod); 
		boxSize += tracer.Field("displayCenterX", 16, this.displayCenterX); 
		boxSize += tracer.Field("displayCenterY", 16, this.displayCenterY); 
		return boxSize;
	}
}

}
