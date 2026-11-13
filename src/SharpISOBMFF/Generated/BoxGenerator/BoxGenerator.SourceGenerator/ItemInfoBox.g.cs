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
aligned(8) class ItemInfoBox
	extends FullBox('iinf', version, 0) {
	if (version == 0) {
		unsigned int(16)	entry_count;
	} else {
		unsigned int(32) entry_count;
	}
	ItemInfoEntry[ entry_count ]		item_infos;
}
*/
public partial class ItemInfoBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class ItemInfoBox
	extends FullBox('iinf', version, 0) {
	if (version == 0) {
		unsigned int(16)	entry_count;
	} else {
		unsigned int(32) entry_count;
	}
	ItemInfoEntry[ entry_count ]		item_infos;
}"; } }
	public const string TYPE = "iinf";
	public override string DisplayName { get { return "ItemInfoBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("FullBox(meta)"); }


	protected uint entry_count; 
	public uint EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }
	public IEnumerable<ItemInfoEntry> ItemInfos { get { return this.children.OfType<ItemInfoEntry>(); } }

	public ItemInfoBox(byte version = 0): base(IsoStream.FromFourCC("iinf"), version, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);

		if (version == 0)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.entry_count, "entry_count"); 
		}

		else 
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entry_count, "entry_count"); 
		}
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.item_infos, "item_infos"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);

		if (version == 0)
		{
			boxSize += stream.WriteUInt16( this.entry_count, "entry_count"); 
		}

		else 
		{
			boxSize += stream.WriteUInt32( this.entry_count, "entry_count"); 
		}
		// boxSize += stream.WriteBox( this.item_infos, "item_infos"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();

		if (version == 0)
		{
			boxSize += 16; // entry_count
		}

		else 
		{
			boxSize += 32; // entry_count
		}
		// boxSize += IsoStream.CalculateBoxSize(item_infos); // item_infos
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);

		if (version == 0)
		{
			boxSize += tracer.Field("entry_count", 16, this.entry_count); 
		}

		else 
		{
			boxSize += tracer.Field("entry_count", 32, this.entry_count); 
		}
		// boxSize += tracer.Field("item_infos", IsoStream.CalculateBoxSize(item_infos), this.item_infos); 
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
