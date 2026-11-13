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
class ReceivedRtpHintSampleEntry() extends HintSampleEntry ('rrtp') {
	uint(16)		hinttrackversion = 1;
	uint(16)		highestcompatibleversion = 1;
	uint(32)		maxpacketsize;
}
*/
public partial class ReceivedRtpHintSampleEntry : HintSampleEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class ReceivedRtpHintSampleEntry() extends HintSampleEntry ('rrtp') {
	uint(16)		hinttrackversion = 1;
	uint(16)		highestcompatibleversion = 1;
	uint(32)		maxpacketsize;
}"; } }
	public const string TYPE = "rrtp";
	public override string DisplayName { get { return "ReceivedRtpHintSampleEntry"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stsd"); }


	protected ushort hinttrackversion = 1; 
	public ushort Hinttrackversion { get { return this.hinttrackversion; } set { this.hinttrackversion = value; } }

	protected ushort highestcompatibleversion = 1; 
	public ushort Highestcompatibleversion { get { return this.highestcompatibleversion; } set { this.highestcompatibleversion = value; } }

	protected uint maxpacketsize; 
	public uint Maxpacketsize { get { return this.maxpacketsize; } set { this.maxpacketsize = value; } }

	public ReceivedRtpHintSampleEntry(): base(IsoStream.FromFourCC("rrtp"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.hinttrackversion, "hinttrackversion"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.highestcompatibleversion, "highestcompatibleversion"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.maxpacketsize, "maxpacketsize"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.hinttrackversion, "hinttrackversion"); 
		boxSize += stream.WriteUInt16( this.highestcompatibleversion, "highestcompatibleversion"); 
		boxSize += stream.WriteUInt32( this.maxpacketsize, "maxpacketsize"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // hinttrackversion
		boxSize += 16; // highestcompatibleversion
		boxSize += 32; // maxpacketsize
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("hinttrackversion", 16, this.hinttrackversion); 
		boxSize += tracer.Field("highestcompatibleversion", 16, this.highestcompatibleversion); 
		boxSize += tracer.Field("maxpacketsize", 32, this.maxpacketsize); 
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
