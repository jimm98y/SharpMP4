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
aligned(8) class HintMaxRate extends Box('maxr') {	// maximum data rate
	uint(32)	period;			// in milliseconds
	uint(32)	bytes; }			// max bytes sent in any period 'period' long including RTP headers
*/
public partial class HintMaxRate : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class HintMaxRate extends Box('maxr') {	// maximum data rate
	uint(32)	period;			// in milliseconds
	uint(32)	bytes; }			// max bytes sent in any period 'period' long including RTP headers"; } }
	public const string TYPE = "maxr";
	public override string DisplayName { get { return "HintMaxRate"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("hinf"); }


	protected uint period;  //  in milliseconds
	public uint Period { get { return this.period; } set { this.period = value; } }

	protected uint bytes; 
	public uint Bytes { get { return this.bytes; } set { this.bytes = value; } }

	public HintMaxRate(): base(IsoStream.FromFourCC("maxr"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		/*  maximum data rate */
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.period, "period"); // in milliseconds
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.bytes, "bytes"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		/*  maximum data rate */
		boxSize += stream.WriteUInt32( this.period, "period"); // in milliseconds
		boxSize += stream.WriteUInt32( this.bytes, "bytes"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		/*  maximum data rate */
		boxSize += 32; // period
		boxSize += 32; // bytes
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		/*  maximum data rate */
		boxSize += tracer.Field("period", 32, this.period); // in milliseconds
		boxSize += tracer.Field("bytes", 32, this.bytes); 
		return boxSize;
	}
}

}
