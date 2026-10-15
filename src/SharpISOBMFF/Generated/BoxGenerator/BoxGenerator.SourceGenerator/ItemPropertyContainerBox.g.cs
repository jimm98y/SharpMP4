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
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("iprp"); }


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

	public override ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += base.ToString(builder);
		// boxSize += builder.Field("properties", IsoStream.CalculateBoxSize(properties), this.properties); // boxes derived from
		/*  ItemProperty or ItemFullProperty, or FreeSpaceBox(es) */
		/*  to fill the box */
		boxSize += builder.Children(this);
		return boxSize;
	}
}

}
