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
class SAPEntry() extends  SampleGroupDescriptionEntry('sap ')
{
	unsigned int(1) dependent_flag;
	unsigned int(3) reserved;
	unsigned int(4) SAP_type;
}
*/
public partial class SAPEntry : SampleGroupDescriptionEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class SAPEntry() extends  SampleGroupDescriptionEntry('sap ')
{
	unsigned int(1) dependent_flag;
	unsigned int(3) reserved;
	unsigned int(4) SAP_type;
}"; } }
	public const string TYPE = "sap ";
	public override string DisplayName { get { return "SAPEntry"; } }

	protected bool dependent_flag; 
	public bool DependentFlag { get { return this.dependent_flag; } set { this.dependent_flag = value; } }

	protected byte reserved; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte SAP_type; 
	public byte SAPType { get { return this.SAP_type; } set { this.SAP_type = value; } }

	public SAPEntry(): base(IsoStream.FromFourCC("sap "))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.dependent_flag, "dependent_flag"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.SAP_type, "SAP_type"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBit( this.dependent_flag, "dependent_flag"); 
		boxSize += stream.WriteBits(3,  this.reserved, "reserved"); 
		boxSize += stream.WriteBits(4,  this.SAP_type, "SAP_type"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 1; // dependent_flag
		boxSize += 3; // reserved
		boxSize += 4; // SAP_type
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("dependent_flag", 1, this.dependent_flag); 
		boxSize += tracer.Field("reserved", 3, this.reserved); 
		boxSize += tracer.Field("SAP_type", 4, this.SAP_type); 
		return boxSize;
	}
}

}
