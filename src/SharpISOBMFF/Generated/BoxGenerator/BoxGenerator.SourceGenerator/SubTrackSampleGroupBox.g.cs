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
aligned(8) class SubTrackSampleGroupBox
	extends FullBox('stsg', 0, 0){
	unsigned int(32) grouping_type;
	unsigned int(16) item_count;
	for(i = 0; i< item_count; i++)
		unsigned int(32)	group_description_index;
}
*/
public partial class SubTrackSampleGroupBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class SubTrackSampleGroupBox
	extends FullBox('stsg', 0, 0){
	unsigned int(32) grouping_type;
	unsigned int(16) item_count;
	for(i = 0; i< item_count; i++)
		unsigned int(32)	group_description_index;
}"; } }
	public const string TYPE = "stsg";
	public override string DisplayName { get { return "SubTrackSampleGroupBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("strd"); }


	protected uint grouping_type; 
	public uint GroupingType { get { return this.grouping_type; } set { this.grouping_type = value; } }

	protected ushort item_count; 
	public ushort ItemCount { get { return this.item_count; } set { this.item_count = value; } }

	protected uint[] group_description_index; 
	public uint[] GroupDescriptionIndex { get { return this.group_description_index; } set { this.group_description_index = value; } }

	public SubTrackSampleGroupBox(): base(IsoStream.FromFourCC("stsg"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.grouping_type, "grouping_type"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.item_count, "item_count"); 

		this.group_description_index = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt( item_count), "group_description_index");
		for (int i = 0; i< item_count; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.group_description_index[i], "group_description_index"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.grouping_type, "grouping_type"); 
		boxSize += stream.WriteUInt16( this.item_count, "item_count"); 

		for (int i = 0; i< item_count; i++)
		{
			boxSize += stream.WriteUInt32( this.group_description_index[i], "group_description_index"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // grouping_type
		boxSize += 16; // item_count

		for (int i = 0; i< item_count; i++)
		{
			boxSize += 32; // group_description_index
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("grouping_type", 32, this.grouping_type); 
		boxSize += tracer.Field("item_count", 16, this.item_count); 

		for (int i = 0; i< item_count; i++)
		{
			boxSize += tracer.Field($"group_description_index[{i}]", 32, this.group_description_index[i]); 
		}
		return boxSize;
	}
}

}
