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
aligned(8) class ItemPropertiesBox
	extends Box('iprp') {
	ItemPropertyContainerBox property_container;
	ItemPropertyAssociationBox association[];
}
*/
public partial class ItemPropertiesBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class ItemPropertiesBox
	extends Box('iprp') {
	ItemPropertyContainerBox property_container;
	ItemPropertyAssociationBox association[];
}"; } }
	public const string TYPE = "iprp";
	public override string DisplayName { get { return "ItemPropertiesBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("FullBox(meta)"); }

	public ItemPropertyContainerBox PropertyContainer { get { return this.children.OfType<ItemPropertyContainerBox>().FirstOrDefault(); } }
	public IEnumerable<ItemPropertyAssociationBox> Association { get { return this.children.OfType<ItemPropertyAssociationBox>(); } }

	public ItemPropertiesBox(): base(IsoStream.FromFourCC("iprp"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.property_container, "property_container"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.association, "association"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.property_container, "property_container"); 
		// boxSize += stream.WriteBox( this.association, "association"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(property_container); // property_container
		// boxSize += IsoStream.CalculateBoxSize(association); // association
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		// boxSize += tracer.Field("property_container", IsoStream.CalculateBoxSize(property_container), this.property_container); 
		// boxSize += tracer.Field("association", IsoStream.CalculateBoxSize(association), this.association); 
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
