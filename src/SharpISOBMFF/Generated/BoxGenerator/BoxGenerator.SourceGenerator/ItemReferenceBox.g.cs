using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ItemReferenceBox extends FullBox('iref', version, 0) {
	if (version==0) {
		SingleItemTypeReferenceBox				references[];
	} else if (version==1) {
		SingleItemTypeReferenceBoxLarge	references[];
	}
}
*/
public partial class ItemReferenceBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class ItemReferenceBox extends FullBox('iref', version, 0) {
	if (version==0) {
		SingleItemTypeReferenceBox				references[];
	} else if (version==1) {
		SingleItemTypeReferenceBoxLarge	references[];
	}
}"; } }
	public const string TYPE = "iref";
	public override string DisplayName { get { return "ItemReferenceBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("FullBox(meta)"); }


	protected SingleItemTypeReferenceBox[] references = []; 
	public SingleItemTypeReferenceBox[] References { get { return this.references; } set { this.references = value; } }

	protected SingleItemTypeReferenceBoxLarge[] references0 = []; 
	public SingleItemTypeReferenceBoxLarge[] References0 { get { return this.references0; } set { this.references0 = value; } }

	public ItemReferenceBox(byte version = 0): base(IsoStream.FromFourCC("iref"), version, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);

		if (version==0)
		{
			boxSize += stream.ReadBox(boxSize, readSize, (boxHeader) => new SingleItemTypeReferenceBox(boxHeader.Type), this, out this.references, "references"); 
		}

		else if (version==1)
		{
			boxSize += stream.ReadBox(boxSize, readSize, (boxHeader) => new SingleItemTypeReferenceBoxLarge(boxHeader.Type), this, out this.references0, "references0"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);

		if (version==0)
		{
			boxSize += stream.WriteBox( this.references, "references"); 
		}

		else if (version==1)
		{
			boxSize += stream.WriteBox( this.references0, "references0"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();

		if (version==0)
		{
			boxSize += IsoStream.CalculateBoxSize(references); // references
		}

		else if (version==1)
		{
			boxSize += IsoStream.CalculateBoxSize(references0); // references0
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);

		if (version==0)
		{
			boxSize += tracer.Field("references", IsoStream.CalculateBoxSize(references), this.references); 
		}

		else if (version==1)
		{
			boxSize += tracer.Field("references0", IsoStream.CalculateBoxSize(references0), this.references0); 
		}
		return boxSize;
	}
}

}
