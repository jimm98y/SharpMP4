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
class AppleColorTableBox() extends Box ('ctab'){
	signed int(32) color_table_seed;
	signed int(16) flags;
	signed int(16) color_table_size;
	AppleColor colors[];
}
*/
public partial class AppleColorTableBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class AppleColorTableBox() extends Box ('ctab'){
	signed int(32) color_table_seed;
	signed int(16) flags;
	signed int(16) color_table_size;
	AppleColor colors[];
}"; } }
	public const string TYPE = "ctab";
	public override string DisplayName { get { return "AppleColorTableBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("moov"); }


	protected int color_table_seed; 
	public int ColorTableSeed { get { return this.color_table_seed; } set { this.color_table_seed = value; } }

	protected short flags; 
	public short Flags { get { return this.flags; } set { this.flags = value; } }

	protected short color_table_size; 
	public short ColorTableSize { get { return this.color_table_size; } set { this.color_table_size = value; } }

	protected AppleColor[] colors = []; 
	public AppleColor[] Colors { get { return this.colors; } set { this.colors = value; } }

	public AppleColorTableBox(): base(IsoStream.FromFourCC("ctab"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.color_table_seed, "color_table_seed"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.flags, "flags"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.color_table_size, "color_table_size"); 
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(uint.MaxValue), () => new AppleColor(),  out this.colors, "colors"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt32( this.color_table_seed, "color_table_seed"); 
		boxSize += stream.WriteInt16( this.flags, "flags"); 
		boxSize += stream.WriteInt16( this.color_table_size, "color_table_size"); 
		boxSize += stream.WriteClass( this.colors, "colors"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // color_table_seed
		boxSize += 16; // flags
		boxSize += 16; // color_table_size
		boxSize += IsoStream.CalculateClassSize(colors); // colors
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("color_table_seed", 32, this.color_table_seed); 
		boxSize += tracer.Field("flags", 16, this.flags); 
		boxSize += tracer.Field("color_table_size", 16, this.color_table_size); 
		boxSize += tracer.Field("colors", IsoStream.CalculateClassSize(colors), this.colors); 
		return boxSize;
	}
}

}
