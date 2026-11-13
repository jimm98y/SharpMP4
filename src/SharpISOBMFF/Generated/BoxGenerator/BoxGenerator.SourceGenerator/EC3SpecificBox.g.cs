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
aligned(8) class EC3SpecificBox() extends Box('dec3') {
	bit(13) dataRate;
	bit(3) numIndSub;
	EC3SpecificEntry entries[numIndSub + 1];
}
*/
public partial class EC3SpecificBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class EC3SpecificBox() extends Box('dec3') {
	bit(13) dataRate;
	bit(3) numIndSub;
	EC3SpecificEntry entries[numIndSub + 1];
}"; } }
	public const string TYPE = "dec3";
	public override string DisplayName { get { return "EC3SpecificBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("AudioSampleEntry(ec-3,enca)", "wave"); }


	protected ushort dataRate; 
	public ushort DataRate { get { return this.dataRate; } set { this.dataRate = value; } }

	protected byte numIndSub; 
	public byte NumIndSub { get { return this.numIndSub; } set { this.numIndSub = value; } }

	protected EC3SpecificEntry[] entries = []; 
	public EC3SpecificEntry[] Entries { get { return this.entries; } set { this.entries = value; } }

	public EC3SpecificBox(): base(IsoStream.FromFourCC("dec3"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 13,  out this.dataRate, "dataRate"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.numIndSub, "numIndSub"); 
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(numIndSub + 1), () => new EC3SpecificEntry(),  out this.entries, "entries"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(13,  this.dataRate, "dataRate"); 
		boxSize += stream.WriteBits(3,  this.numIndSub, "numIndSub"); 
		boxSize += stream.WriteClass( this.entries, "entries"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 13; // dataRate
		boxSize += 3; // numIndSub
		boxSize += IsoStream.CalculateClassSize(entries); // entries
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("dataRate", 13, this.dataRate); 
		boxSize += tracer.Field("numIndSub", 3, this.numIndSub); 
		boxSize += tracer.Field("entries", IsoStream.CalculateClassSize(entries), this.entries); 
		return boxSize;
	}
}

}
