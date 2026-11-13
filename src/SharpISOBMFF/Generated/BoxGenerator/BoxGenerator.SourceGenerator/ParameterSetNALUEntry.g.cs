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
class ParameterSetNALUEntry() extends VisualSampleGroupEntry ('pase')
{
	unsigned int(16) ps_nalu_length;
	bit(8* ps_nalu_length) ps_nal_unit;
}
*/
public partial class ParameterSetNALUEntry : VisualSampleGroupEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class ParameterSetNALUEntry() extends VisualSampleGroupEntry ('pase')
{
	unsigned int(16) ps_nalu_length;
	bit(8* ps_nalu_length) ps_nal_unit;
}"; } }
	public const string TYPE = "pase";
	public override string DisplayName { get { return "ParameterSetNALUEntry"; } }

	protected ushort ps_nalu_length; 
	public ushort PsNaluLength { get { return this.ps_nalu_length; } set { this.ps_nalu_length = value; } }

	protected byte[] ps_nal_unit = []; 
	public byte[] PsNalUnit { get { return this.ps_nal_unit; } set { this.ps_nal_unit = value; } }

	public ParameterSetNALUEntry(): base(IsoStream.FromFourCC("pase"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.ps_nalu_length, "ps_nalu_length"); 
		boxSize += stream.ReadBits(boxSize, readSize, (uint)(8* ps_nalu_length ),  out this.ps_nal_unit, "ps_nal_unit"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.ps_nalu_length, "ps_nalu_length"); 
		boxSize += stream.WriteBits((uint)(8* ps_nalu_length ),  this.ps_nal_unit, "ps_nal_unit"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // ps_nalu_length
		boxSize += (ulong)(8* ps_nalu_length ); // ps_nal_unit
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("ps_nalu_length", 16, this.ps_nalu_length); 
		boxSize += tracer.Field("ps_nal_unit", (ulong)(8* ps_nalu_length ), this.ps_nal_unit); 
		return boxSize;
	}
}

}
