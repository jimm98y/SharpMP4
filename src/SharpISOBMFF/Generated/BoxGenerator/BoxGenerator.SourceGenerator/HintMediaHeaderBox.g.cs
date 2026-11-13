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
aligned(8) class HintMediaHeaderBox
	extends FullBox('hmhd', version = 0, 0) {
	unsigned int(16)	maxPDUsize;
	unsigned int(16)	avgPDUsize;
	unsigned int(32)	maxbitrate;
	unsigned int(32)	avgbitrate;
	unsigned int(32)	reserved = 0;
}
*/
public partial class HintMediaHeaderBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class HintMediaHeaderBox
	extends FullBox('hmhd', version = 0, 0) {
	unsigned int(16)	maxPDUsize;
	unsigned int(16)	avgPDUsize;
	unsigned int(32)	maxbitrate;
	unsigned int(32)	avgbitrate;
	unsigned int(32)	reserved = 0;
}"; } }
	public const string TYPE = "hmhd";
	public override string DisplayName { get { return "HintMediaHeaderBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("minf"); }


	protected ushort maxPDUsize; 
	public ushort MaxPDUsize { get { return this.maxPDUsize; } set { this.maxPDUsize = value; } }

	protected ushort avgPDUsize; 
	public ushort AvgPDUsize { get { return this.avgPDUsize; } set { this.avgPDUsize = value; } }

	protected uint maxbitrate; 
	public uint Maxbitrate { get { return this.maxbitrate; } set { this.maxbitrate = value; } }

	protected uint avgbitrate; 
	public uint Avgbitrate { get { return this.avgbitrate; } set { this.avgbitrate = value; } }

	protected uint reserved = 0; 
	public uint Reserved { get { return this.reserved; } set { this.reserved = value; } }

	public HintMediaHeaderBox(): base(IsoStream.FromFourCC("hmhd"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.maxPDUsize, "maxPDUsize"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.avgPDUsize, "avgPDUsize"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.maxbitrate, "maxbitrate"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.avgbitrate, "avgbitrate"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.maxPDUsize, "maxPDUsize"); 
		boxSize += stream.WriteUInt16( this.avgPDUsize, "avgPDUsize"); 
		boxSize += stream.WriteUInt32( this.maxbitrate, "maxbitrate"); 
		boxSize += stream.WriteUInt32( this.avgbitrate, "avgbitrate"); 
		boxSize += stream.WriteUInt32( this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // maxPDUsize
		boxSize += 16; // avgPDUsize
		boxSize += 32; // maxbitrate
		boxSize += 32; // avgbitrate
		boxSize += 32; // reserved
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("maxPDUsize", 16, this.maxPDUsize); 
		boxSize += tracer.Field("avgPDUsize", 16, this.avgPDUsize); 
		boxSize += tracer.Field("maxbitrate", 32, this.maxbitrate); 
		boxSize += tracer.Field("avgbitrate", 32, this.avgbitrate); 
		boxSize += tracer.Field("reserved", 32, this.reserved); 
		return boxSize;
	}
}

}
