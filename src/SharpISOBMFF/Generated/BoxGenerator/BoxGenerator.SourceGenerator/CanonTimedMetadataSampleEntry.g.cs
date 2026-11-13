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
// Canon.pm, CTMD: a CR3's timed metadata; its entry lists the type and size of each record
aligned(8) class CanonTimedMetadataSampleEntry() extends SampleEntry('CTMD') {
	unsigned int(32) recordCount;
	CanonCTMDRecordType recordTypes[recordCount];
}
*/
public partial class CanonTimedMetadataSampleEntry : SampleEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"// Canon.pm, CTMD: a CR3's timed metadata; its entry lists the type and size of each record
aligned(8) class CanonTimedMetadataSampleEntry() extends SampleEntry('CTMD') {
	unsigned int(32) recordCount;
	CanonCTMDRecordType recordTypes[recordCount];
}"; } }
	public const string TYPE = "CTMD";
	public override string DisplayName { get { return "CanonTimedMetadataSampleEntry"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stsd"); }


	protected uint recordCount; 
	public uint RecordCount { get { return this.recordCount; } set { this.recordCount = value; } }

	protected CanonCTMDRecordType[] recordTypes = []; 
	public CanonCTMDRecordType[] RecordTypes { get { return this.recordTypes; } set { this.recordTypes = value; } }

	public CanonTimedMetadataSampleEntry(): base(IsoStream.FromFourCC("CTMD"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.recordCount, "recordCount"); 
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(recordCount), () => new CanonCTMDRecordType(),  out this.recordTypes, "recordTypes"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.recordCount, "recordCount"); 
		boxSize += stream.WriteClass( this.recordTypes, "recordTypes"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // recordCount
		boxSize += IsoStream.CalculateClassSize(recordTypes); // recordTypes
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("recordCount", 32, this.recordCount); 
		boxSize += tracer.Field("recordTypes", IsoStream.CalculateClassSize(recordTypes), this.recordTypes); 
		return boxSize;
	}
}

}
