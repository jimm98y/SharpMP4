using System;
using System.Linq;
using System.Collections.Generic;

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
	public const string TYPE = "iprp";
	public override string DisplayName { get { return "ItemPropertiesBox"; } }
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
}

}
