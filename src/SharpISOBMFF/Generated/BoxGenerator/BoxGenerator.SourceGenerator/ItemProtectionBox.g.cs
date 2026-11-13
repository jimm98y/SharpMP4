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
aligned(8) class ItemProtectionBox
	extends FullBox('ipro', version = 0, 0) {
	unsigned int(16) protection_count;
	for (i=1; i<=protection_count; i++) {
		ProtectionSchemeInfoBox	protection_information;
	}
}
*/
public partial class ItemProtectionBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class ItemProtectionBox
	extends FullBox('ipro', version = 0, 0) {
	unsigned int(16) protection_count;
	for (i=1; i<=protection_count; i++) {
		ProtectionSchemeInfoBox	protection_information;
	}
}"; } }
	public const string TYPE = "ipro";
	public override string DisplayName { get { return "ItemProtectionBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("FullBox(meta)"); }


	protected ushort protection_count; 
	public ushort ProtectionCount { get { return this.protection_count; } set { this.protection_count = value; } }
	public IEnumerable<ProtectionSchemeInfoBox> ProtectionInformation { get { return this.children.OfType<ProtectionSchemeInfoBox>(); } }

	public ItemProtectionBox(): base(IsoStream.FromFourCC("ipro"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.protection_count, "protection_count"); 

		// for (int i=0; i<protection_count; i++)
			// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.protection_information[i], "protection_information"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.protection_count, "protection_count"); 

		// for (int i=0; i<protection_count; i++)
			// boxSize += stream.WriteBox( this.protection_information[i], "protection_information"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // protection_count

		// for (int i=0; i<protection_count; i++)
			// boxSize += IsoStream.CalculateBoxSize(protection_information[i]); // protection_information
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("protection_count", 16, this.protection_count); 

		// for (int i=0; i<protection_count; i++)
			// boxSize += tracer.Field($"protection_information[{i}]", IsoStream.CalculateBoxSize(protection_information[i]), this.protection_information[i]); 
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
