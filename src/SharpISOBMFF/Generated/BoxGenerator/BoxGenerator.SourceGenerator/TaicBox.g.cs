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
aligned(8) class TaicBox() extends FullBox('taic') {
	unsigned int(64) time_uncertainty;
	unsigned int(32) clock_resolution;
	signed int(32) clock_drift_rate;
	unsigned int(8) clock_type;
}
*/
public partial class TaicBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class TaicBox() extends FullBox('taic') {
	unsigned int(64) time_uncertainty;
	unsigned int(32) clock_resolution;
	signed int(32) clock_drift_rate;
	unsigned int(8) clock_type;
}"; } }
	public const string TYPE = "taic";
	public override string DisplayName { get { return "TaicBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("SampleEntry", "ipco"); }


	protected ulong time_uncertainty; 
	public ulong TimeUncertainty { get { return this.time_uncertainty; } set { this.time_uncertainty = value; } }

	protected uint clock_resolution; 
	public uint ClockResolution { get { return this.clock_resolution; } set { this.clock_resolution = value; } }

	protected int clock_drift_rate; 
	public int ClockDriftRate { get { return this.clock_drift_rate; } set { this.clock_drift_rate = value; } }

	protected byte clock_type; 
	public byte ClockType { get { return this.clock_type; } set { this.clock_type = value; } }

	public TaicBox(): base(IsoStream.FromFourCC("taic"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt64(boxSize, readSize,  out this.time_uncertainty, "time_uncertainty"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.clock_resolution, "clock_resolution"); 
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.clock_drift_rate, "clock_drift_rate"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.clock_type, "clock_type"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt64( this.time_uncertainty, "time_uncertainty"); 
		boxSize += stream.WriteUInt32( this.clock_resolution, "clock_resolution"); 
		boxSize += stream.WriteInt32( this.clock_drift_rate, "clock_drift_rate"); 
		boxSize += stream.WriteUInt8( this.clock_type, "clock_type"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 64; // time_uncertainty
		boxSize += 32; // clock_resolution
		boxSize += 32; // clock_drift_rate
		boxSize += 8; // clock_type
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("time_uncertainty", 64, this.time_uncertainty); 
		boxSize += tracer.Field("clock_resolution", 32, this.clock_resolution); 
		boxSize += tracer.Field("clock_drift_rate", 32, this.clock_drift_rate); 
		boxSize += tracer.Field("clock_type", 8, this.clock_type); 
		return boxSize;
	}
}

}
