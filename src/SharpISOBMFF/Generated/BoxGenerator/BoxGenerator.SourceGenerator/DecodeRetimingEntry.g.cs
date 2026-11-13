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
class DecodeRetimingEntry() extends VisualSampleGroupEntry ('dtrt') {
	unsigned int(8) tierCount;
	for (i=1; i<=tierCount; i++) {
		unsigned int(16) tierID;
		signed int(16) delta;
	}
}
*/
public partial class DecodeRetimingEntry : VisualSampleGroupEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class DecodeRetimingEntry() extends VisualSampleGroupEntry ('dtrt') {
	unsigned int(8) tierCount;
	for (i=1; i<=tierCount; i++) {
		unsigned int(16) tierID;
		signed int(16) delta;
	}
}"; } }
	public const string TYPE = "dtrt";
	public override string DisplayName { get { return "DecodeRetimingEntry"; } }

	protected byte tierCount; 
	public byte TierCount { get { return this.tierCount; } set { this.tierCount = value; } }

	protected ushort[] tierID; 
	public ushort[] TierID { get { return this.tierID; } set { this.tierID = value; } }

	protected short[] delta; 
	public short[] Delta { get { return this.delta; } set { this.delta = value; } }

	public DecodeRetimingEntry(): base(IsoStream.FromFourCC("dtrt"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.tierCount, "tierCount"); 

		this.tierID = stream.SafeAllocate<ushort>(boxSize, readSize, IsoStream.GetInt(tierCount), "tierID");
		this.delta = stream.SafeAllocate<short>(boxSize, readSize, IsoStream.GetInt(tierCount), "delta");
		for (int i=0; i<tierCount; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.tierID[i], "tierID"); 
			boxSize += stream.ReadInt16(boxSize, readSize,  out this.delta[i], "delta"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.tierCount, "tierCount"); 

		for (int i=0; i<tierCount; i++)
		{
			boxSize += stream.WriteUInt16( this.tierID[i], "tierID"); 
			boxSize += stream.WriteInt16( this.delta[i], "delta"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // tierCount

		for (int i=0; i<tierCount; i++)
		{
			boxSize += 16; // tierID
			boxSize += 16; // delta
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("tierCount", 8, this.tierCount); 

		for (int i=0; i<tierCount; i++)
		{
			boxSize += tracer.Field($"tierID[{i}]", 16, this.tierID[i]); 
			boxSize += tracer.Field($"delta[{i}]", 16, this.delta[i]); 
		}
		return boxSize;
	}
}

}
