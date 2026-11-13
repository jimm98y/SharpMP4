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
class HILNconfig()
{
	uimsbf(1) HILNquantMode;
	uimsbf(8) HILNmaxNumLine;
	uimsbf(4) HILNsampleRateCode;
	uimsbf(12) HILNframeLength;
	uimsbf(2) HILNcontMode;
}
*/
public partial class HILNconfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"class HILNconfig()
{
	uimsbf(1) HILNquantMode;
	uimsbf(8) HILNmaxNumLine;
	uimsbf(4) HILNsampleRateCode;
	uimsbf(12) HILNframeLength;
	uimsbf(2) HILNcontMode;
}"; } }
	public virtual string DisplayName { get { return "HILNconfig"; } }

	protected bool HILNquantMode; 
	public bool _HILNquantMode { get { return this.HILNquantMode; } set { this.HILNquantMode = value; } }

	protected byte HILNmaxNumLine; 
	public byte _HILNmaxNumLine { get { return this.HILNmaxNumLine; } set { this.HILNmaxNumLine = value; } }

	protected byte HILNsampleRateCode; 
	public byte _HILNsampleRateCode { get { return this.HILNsampleRateCode; } set { this.HILNsampleRateCode = value; } }

	protected ushort HILNframeLength; 
	public ushort _HILNframeLength { get { return this.HILNframeLength; } set { this.HILNframeLength = value; } }

	protected byte HILNcontMode; 
	public byte _HILNcontMode { get { return this.HILNcontMode; } set { this.HILNcontMode = value; } }

	public HILNconfig(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBit(boxSize, readSize,  out this.HILNquantMode, "HILNquantMode"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.HILNmaxNumLine, "HILNmaxNumLine"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.HILNsampleRateCode, "HILNsampleRateCode"); 
		boxSize += stream.ReadBits(boxSize, readSize, 12,  out this.HILNframeLength, "HILNframeLength"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.HILNcontMode, "HILNcontMode"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBit( this.HILNquantMode, "HILNquantMode"); 
		boxSize += stream.WriteUInt8( this.HILNmaxNumLine, "HILNmaxNumLine"); 
		boxSize += stream.WriteBits(4,  this.HILNsampleRateCode, "HILNsampleRateCode"); 
		boxSize += stream.WriteBits(12,  this.HILNframeLength, "HILNframeLength"); 
		boxSize += stream.WriteBits(2,  this.HILNcontMode, "HILNcontMode"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 1; // HILNquantMode
		boxSize += 8; // HILNmaxNumLine
		boxSize += 4; // HILNsampleRateCode
		boxSize += 12; // HILNframeLength
		boxSize += 2; // HILNcontMode
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("HILNquantMode", 1, this.HILNquantMode); 
		boxSize += tracer.Field("HILNmaxNumLine", 8, this.HILNmaxNumLine); 
		boxSize += tracer.Field("HILNsampleRateCode", 4, this.HILNsampleRateCode); 
		boxSize += tracer.Field("HILNframeLength", 12, this.HILNframeLength); 
		boxSize += tracer.Field("HILNcontMode", 2, this.HILNcontMode); 
		return boxSize;
	}
}

}
