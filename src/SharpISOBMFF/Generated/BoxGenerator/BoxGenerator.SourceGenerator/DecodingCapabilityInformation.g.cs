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
class DecodingCapabilityInformation extends VisualSampleGroupEntry ('dcfi') {
	unsigned int(16) dci_nal_unit_length;
	bit(8*dci_nal_unit_length) dci_nal_unit;
}
*/
public partial class DecodingCapabilityInformation : VisualSampleGroupEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class DecodingCapabilityInformation extends VisualSampleGroupEntry ('dcfi') {
	unsigned int(16) dci_nal_unit_length;
	bit(8*dci_nal_unit_length) dci_nal_unit;
}"; } }
	public const string TYPE = "dcfi";
	public override string DisplayName { get { return "DecodingCapabilityInformation"; } }

	protected ushort dci_nal_unit_length; 
	public ushort DciNalUnitLength { get { return this.dci_nal_unit_length; } set { this.dci_nal_unit_length = value; } }

	protected byte[] dci_nal_unit = []; 
	public byte[] DciNalUnit { get { return this.dci_nal_unit; } set { this.dci_nal_unit = value; } }

	public DecodingCapabilityInformation(): base(IsoStream.FromFourCC("dcfi"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.dci_nal_unit_length, "dci_nal_unit_length"); 
		boxSize += stream.ReadBits(boxSize, readSize, (uint)(8*dci_nal_unit_length ),  out this.dci_nal_unit, "dci_nal_unit"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.dci_nal_unit_length, "dci_nal_unit_length"); 
		boxSize += stream.WriteBits((uint)(8*dci_nal_unit_length ),  this.dci_nal_unit, "dci_nal_unit"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // dci_nal_unit_length
		boxSize += (ulong)(8*dci_nal_unit_length ); // dci_nal_unit
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("dci_nal_unit_length", 16, this.dci_nal_unit_length); 
		boxSize += tracer.Field("dci_nal_unit", (ulong)(8*dci_nal_unit_length ), this.dci_nal_unit); 
		return boxSize;
	}
}

}
