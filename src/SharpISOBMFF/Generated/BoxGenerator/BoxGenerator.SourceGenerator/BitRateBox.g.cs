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
class BitRateBox extends Box('btrt'){
	unsigned int(32) bufferSizeDB;
	unsigned int(32) maxBitrate;
	unsigned int(32) avgBitrate;
}
*/
public partial class BitRateBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class BitRateBox extends Box('btrt'){
	unsigned int(32) bufferSizeDB;
	unsigned int(32) maxBitrate;
	unsigned int(32) avgBitrate;
}"; } }
	public const string TYPE = "btrt";
	public override string DisplayName { get { return "BitRateBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("SampleEntry"); }


	protected uint bufferSizeDB; 
	public uint BufferSizeDB { get { return this.bufferSizeDB; } set { this.bufferSizeDB = value; } }

	protected uint maxBitrate; 
	public uint MaxBitrate { get { return this.maxBitrate; } set { this.maxBitrate = value; } }

	protected uint avgBitrate; 
	public uint AvgBitrate { get { return this.avgBitrate; } set { this.avgBitrate = value; } }

	public BitRateBox(): base(IsoStream.FromFourCC("btrt"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.bufferSizeDB, "bufferSizeDB"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.maxBitrate, "maxBitrate"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.avgBitrate, "avgBitrate"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.bufferSizeDB, "bufferSizeDB"); 
		boxSize += stream.WriteUInt32( this.maxBitrate, "maxBitrate"); 
		boxSize += stream.WriteUInt32( this.avgBitrate, "avgBitrate"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // bufferSizeDB
		boxSize += 32; // maxBitrate
		boxSize += 32; // avgBitrate
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("bufferSizeDB", 32, this.bufferSizeDB); 
		boxSize += tracer.Field("maxBitrate", 32, this.maxBitrate); 
		boxSize += tracer.Field("avgBitrate", 32, this.avgBitrate); 
		return boxSize;
	}
}

}
