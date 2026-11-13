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
aligned(8) class SingleItemTypeReferenceBoxLarge(referenceType) extends Box(referenceType) {
	unsigned int(32) from_item_ID;
	unsigned int(16) reference_count;
	for (j=0; j<reference_count; j++) {
		unsigned int(32) to_item_ID;
	}
}
*/
public partial class SingleItemTypeReferenceBoxLarge : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class SingleItemTypeReferenceBoxLarge(referenceType) extends Box(referenceType) {
	unsigned int(32) from_item_ID;
	unsigned int(16) reference_count;
	for (j=0; j<reference_count; j++) {
		unsigned int(32) to_item_ID;
	}
}"; } }
	public override string DisplayName { get { return "SingleItemTypeReferenceBoxLarge"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn(BoxPlacements.Of(this) ?? new[] { "iref" }); }


	protected uint from_item_ID; 
	public uint FromItemID { get { return this.from_item_ID; } set { this.from_item_ID = value; } }

	protected ushort reference_count; 
	public ushort ReferenceCount { get { return this.reference_count; } set { this.reference_count = value; } }

	protected uint[] to_item_ID; 
	public uint[] ToItemID { get { return this.to_item_ID; } set { this.to_item_ID = value; } }

	public SingleItemTypeReferenceBoxLarge(uint referenceType): base(referenceType)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.from_item_ID, "from_item_ID"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.reference_count, "reference_count"); 

		this.to_item_ID = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt(reference_count), "to_item_ID");
		for (int j=0; j<reference_count; j++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.to_item_ID[j], "to_item_ID"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.from_item_ID, "from_item_ID"); 
		boxSize += stream.WriteUInt16( this.reference_count, "reference_count"); 

		for (int j=0; j<reference_count; j++)
		{
			boxSize += stream.WriteUInt32( this.to_item_ID[j], "to_item_ID"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // from_item_ID
		boxSize += 16; // reference_count

		for (int j=0; j<reference_count; j++)
		{
			boxSize += 32; // to_item_ID
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("from_item_ID", 32, this.from_item_ID); 
		boxSize += tracer.Field("reference_count", 16, this.reference_count); 

		for (int j=0; j<reference_count; j++)
		{
			boxSize += tracer.Field($"to_item_ID[{j}]", 32, this.to_item_ID[j]); 
		}
		return boxSize;
	}
}

}
