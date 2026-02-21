using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ItemPropertyContainerBox
	extends Box('ipco')
{
	Box properties[];	// boxes derived from
		// ItemProperty or ItemFullProperty, or FreeSpaceBox(es)
		// to fill the box
}

*/
public partial class ItemPropertyContainerBox : Box
{
	public const string TYPE = "ipco";
	public override string DisplayName { get { return "ItemPropertyContainerBox"; } }
	public IEnumerable<Box> Properties { get { return this.children.OfType<Box>(); } }

	public ItemPropertyContainerBox(): base(IsoStream.FromFourCC("ipco"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.properties, "properties"); // boxes derived from
		/*  ItemProperty or ItemFullProperty, or FreeSpaceBox(es) */
		/*  to fill the box */
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.properties, "properties"); // boxes derived from
		/*  ItemProperty or ItemFullProperty, or FreeSpaceBox(es) */
		/*  to fill the box */
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(properties); // properties
		/*  ItemProperty or ItemFullProperty, or FreeSpaceBox(es) */
		/*  to fill the box */
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
