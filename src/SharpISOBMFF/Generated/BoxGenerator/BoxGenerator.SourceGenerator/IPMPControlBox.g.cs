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
aligned(8) class IPMPControlBox() extends FullBox('ipmc') {
	Descriptor toollist;
	unsigned int(8) count;
	Descriptor descriptor[count];
}
*/
public partial class IPMPControlBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class IPMPControlBox() extends FullBox('ipmc') {
	Descriptor toollist;
	unsigned int(8) count;
	Descriptor descriptor[count];
}"; } }
	public const string TYPE = "ipmc";
	public override string DisplayName { get { return "IPMPControlBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("moov", "meta"); }


	protected Descriptor toollist; 
	public Descriptor Toollist { get { return this.toollist; } set { this.toollist = value; } }

	protected byte count; 
	public byte Count { get { return this.count; } set { this.count = value; } }

	protected Descriptor[] descriptor = []; 
	public Descriptor[] Descriptor { get { return this.descriptor; } set { this.descriptor = value; } }

	public IPMPControlBox(): base(IsoStream.FromFourCC("ipmc"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadDescriptor(boxSize, readSize, this,  out this.toollist, "toollist"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.count, "count"); 
		// boxSize += stream.ReadDescriptor(boxSize, readSize, this,  out this.descriptor, "descriptor"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteDescriptor( this.toollist, "toollist"); 
		boxSize += stream.WriteUInt8( this.count, "count"); 
		// boxSize += stream.WriteDescriptor( this.descriptor, "descriptor"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateDescriptorSize(toollist); // toollist
		boxSize += 8; // count
		// boxSize += IsoStream.CalculateDescriptorSize(descriptor); // descriptor
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		// boxSize += tracer.Field("toollist", IsoStream.CalculateDescriptorSize(toollist), this.toollist); 
		boxSize += tracer.Field("count", 8, this.count); 
		// boxSize += tracer.Field("descriptor", IsoStream.CalculateDescriptorSize(descriptor), this.descriptor); 
		return boxSize;
	}
}

}
