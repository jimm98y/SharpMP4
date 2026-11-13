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
class EndOfSequenceSampleEntry() extends VisualSampleGroupEntry ('eos ')
{
	unsigned int(8) num_eos_nal_unit_minus1;
	for (i=0; i <= num_eos_nal_unit_minus1; i++)
		bit(16) eosNalUnit[i];
}
*/
public partial class EndOfSequenceSampleEntry : VisualSampleGroupEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class EndOfSequenceSampleEntry() extends VisualSampleGroupEntry ('eos ')
{
	unsigned int(8) num_eos_nal_unit_minus1;
	for (i=0; i <= num_eos_nal_unit_minus1; i++)
		bit(16) eosNalUnit[i];
}"; } }
	public const string TYPE = "eos ";
	public override string DisplayName { get { return "EndOfSequenceSampleEntry"; } }

	protected byte num_eos_nal_unit_minus1; 
	public byte NumEosNalUnitMinus1 { get { return this.num_eos_nal_unit_minus1; } set { this.num_eos_nal_unit_minus1 = value; } }

	protected ushort[] eosNalUnit; 
	public ushort[] EosNalUnit { get { return this.eosNalUnit; } set { this.eosNalUnit = value; } }

	public EndOfSequenceSampleEntry(): base(IsoStream.FromFourCC("eos "))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.num_eos_nal_unit_minus1, "num_eos_nal_unit_minus1"); 

		this.eosNalUnit = stream.SafeAllocate<ushort>(boxSize, readSize, IsoStream.GetInt( num_eos_nal_unit_minus1 + 1), "eosNalUnit");
		for (int i=0; i <= num_eos_nal_unit_minus1; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.eosNalUnit[i], "eosNalUnit"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.num_eos_nal_unit_minus1, "num_eos_nal_unit_minus1"); 

		for (int i=0; i <= num_eos_nal_unit_minus1; i++)
		{
			boxSize += stream.WriteUInt16( this.eosNalUnit[i], "eosNalUnit"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // num_eos_nal_unit_minus1

		for (int i=0; i <= num_eos_nal_unit_minus1; i++)
		{
			boxSize += 16; // eosNalUnit
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("num_eos_nal_unit_minus1", 8, this.num_eos_nal_unit_minus1); 

		for (int i=0; i <= num_eos_nal_unit_minus1; i++)
		{
			boxSize += tracer.Field($"eosNalUnit[{i}]", 16, this.eosNalUnit[i]); 
		}
		return boxSize;
	}
}

}
