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
aligned(8) class MultiviewGroupRelationBox() extends FullBox('swtc', version = 0, flags) {
	unsigned int(32) num_entries;
	for (i=0; i<num_entries; i++)
		unsigned int(32) multiview_group_id;
	MultiviewRelationAttributeBox relation_attributes;
}
*/
public partial class MultiviewGroupRelationBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class MultiviewGroupRelationBox() extends FullBox('swtc', version = 0, flags) {
	unsigned int(32) num_entries;
	for (i=0; i<num_entries; i++)
		unsigned int(32) multiview_group_id;
	MultiviewRelationAttributeBox relation_attributes;
}"; } }
	public const string TYPE = "swtc";
	public override string DisplayName { get { return "MultiviewGroupRelationBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("mvci"); }


	protected uint num_entries; 
	public uint NumEntries { get { return this.num_entries; } set { this.num_entries = value; } }

	protected uint[] multiview_group_id; 
	public uint[] MultiviewGroupId { get { return this.multiview_group_id; } set { this.multiview_group_id = value; } }
	public MultiviewRelationAttributeBox RelationAttributes { get { return this.children.OfType<MultiviewRelationAttributeBox>().FirstOrDefault(); } }

	public MultiviewGroupRelationBox(uint flags = 0): base(IsoStream.FromFourCC("swtc"), 0, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.num_entries, "num_entries"); 

		this.multiview_group_id = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt(num_entries), "multiview_group_id");
		for (int i=0; i<num_entries; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.multiview_group_id[i], "multiview_group_id"); 
		}
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.relation_attributes, "relation_attributes"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.num_entries, "num_entries"); 

		for (int i=0; i<num_entries; i++)
		{
			boxSize += stream.WriteUInt32( this.multiview_group_id[i], "multiview_group_id"); 
		}
		// boxSize += stream.WriteBox( this.relation_attributes, "relation_attributes"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // num_entries

		for (int i=0; i<num_entries; i++)
		{
			boxSize += 32; // multiview_group_id
		}
		// boxSize += IsoStream.CalculateBoxSize(relation_attributes); // relation_attributes
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("num_entries", 32, this.num_entries); 

		for (int i=0; i<num_entries; i++)
		{
			boxSize += tracer.Field($"multiview_group_id[{i}]", 32, this.multiview_group_id[i]); 
		}
		// boxSize += tracer.Field("relation_attributes", IsoStream.CalculateBoxSize(relation_attributes), this.relation_attributes); 
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
