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
class AppleColor() {
	unsigned int(16) alpha;
	unsigned int(16) red;
	unsigned int(16) green;
	unsigned int(16) blue;
}
*/
public partial class AppleColor : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"class AppleColor() {
	unsigned int(16) alpha;
	unsigned int(16) red;
	unsigned int(16) green;
	unsigned int(16) blue;
}"; } }
	public virtual string DisplayName { get { return "AppleColor"; } }

	protected ushort alpha; 
	public ushort Alpha { get { return this.alpha; } set { this.alpha = value; } }

	protected ushort red; 
	public ushort Red { get { return this.red; } set { this.red = value; } }

	protected ushort green; 
	public ushort Green { get { return this.green; } set { this.green = value; } }

	protected ushort blue; 
	public ushort Blue { get { return this.blue; } set { this.blue = value; } }

	public AppleColor(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.alpha, "alpha"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.red, "red"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.green, "green"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.blue, "blue"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt16( this.alpha, "alpha"); 
		boxSize += stream.WriteUInt16( this.red, "red"); 
		boxSize += stream.WriteUInt16( this.green, "green"); 
		boxSize += stream.WriteUInt16( this.blue, "blue"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 16; // alpha
		boxSize += 16; // red
		boxSize += 16; // green
		boxSize += 16; // blue
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("alpha", 16, this.alpha); 
		boxSize += tracer.Field("red", 16, this.red); 
		boxSize += tracer.Field("green", 16, this.green); 
		boxSize += tracer.Field("blue", 16, this.blue); 
		return boxSize;
	}
}

}
