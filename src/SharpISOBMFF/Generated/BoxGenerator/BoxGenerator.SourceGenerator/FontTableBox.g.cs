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
aligned(8) class FontTableBox() extends Box('ftab') {
	unsigned int(16) entryCount;
	FontRecord fontEntries[entryCount];
}
*/
public partial class FontTableBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class FontTableBox() extends Box('ftab') {
	unsigned int(16) entryCount;
	FontRecord fontEntries[entryCount];
}"; } }
	public const string TYPE = "ftab";
	public override string DisplayName { get { return "FontTableBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("SampleEntry(tx3g,enct)"); }


	protected ushort entryCount; 
	public ushort EntryCount { get { return this.entryCount; } set { this.entryCount = value; } }

	protected FontRecord[] fontEntries = []; 
	public FontRecord[] FontEntries { get { return this.fontEntries; } set { this.fontEntries = value; } }

	public FontTableBox(): base(IsoStream.FromFourCC("ftab"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.entryCount, "entryCount"); 
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(entryCount), () => new FontRecord(),  out this.fontEntries, "fontEntries"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.entryCount, "entryCount"); 
		boxSize += stream.WriteClass( this.fontEntries, "fontEntries"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // entryCount
		boxSize += IsoStream.CalculateClassSize(fontEntries); // fontEntries
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("entryCount", 16, this.entryCount); 
		boxSize += tracer.Field("fontEntries", IsoStream.CalculateClassSize(fontEntries), this.fontEntries); 
		return boxSize;
	}
}

}
