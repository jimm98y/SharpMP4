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
class SampleToMetadataItemEntry()
	extends SampleGroupDescriptionEntry('stmi') {
	unsigned int(32) meta_box_handler_type;
	unsigned int(32) num_items;
	for(i = 0; i < num_items; i++) {
		unsigned int(32) item_id[i];
	}
}
*/
public partial class SampleToMetadataItemEntry : SampleGroupDescriptionEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class SampleToMetadataItemEntry()
	extends SampleGroupDescriptionEntry('stmi') {
	unsigned int(32) meta_box_handler_type;
	unsigned int(32) num_items;
	for(i = 0; i < num_items; i++) {
		unsigned int(32) item_id[i];
	}
}"; } }
	public const string TYPE = "stmi";
	public override string DisplayName { get { return "SampleToMetadataItemEntry"; } }

	protected uint meta_box_handler_type; 
	public uint MetaBoxHandlerType { get { return this.meta_box_handler_type; } set { this.meta_box_handler_type = value; } }

	protected uint num_items; 
	public uint NumItems { get { return this.num_items; } set { this.num_items = value; } }

	protected uint[] item_id; 
	public uint[] ItemId { get { return this.item_id; } set { this.item_id = value; } }

	public SampleToMetadataItemEntry(): base(IsoStream.FromFourCC("stmi"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.meta_box_handler_type, "meta_box_handler_type"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.num_items, "num_items"); 

		this.item_id = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt( num_items), "item_id");
		for (int i = 0; i < num_items; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.item_id[i], "item_id"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.meta_box_handler_type, "meta_box_handler_type"); 
		boxSize += stream.WriteUInt32( this.num_items, "num_items"); 

		for (int i = 0; i < num_items; i++)
		{
			boxSize += stream.WriteUInt32( this.item_id[i], "item_id"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // meta_box_handler_type
		boxSize += 32; // num_items

		for (int i = 0; i < num_items; i++)
		{
			boxSize += 32; // item_id
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("meta_box_handler_type", 32, this.meta_box_handler_type); 
		boxSize += tracer.Field("num_items", 32, this.num_items); 

		for (int i = 0; i < num_items; i++)
		{
			boxSize += tracer.Field($"item_id[{i}]", 32, this.item_id[i]); 
		}
		return boxSize;
	}
}

}
