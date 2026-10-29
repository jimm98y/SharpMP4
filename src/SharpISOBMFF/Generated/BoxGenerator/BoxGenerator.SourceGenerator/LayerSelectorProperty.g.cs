using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class LayerSelectorProperty
extends ItemProperty('lsel') {
	unsigned int(16) layer_id;
}
*/
public partial class LayerSelectorProperty : ItemProperty
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class LayerSelectorProperty
extends ItemProperty('lsel') {
	unsigned int(16) layer_id;
}"; } }
	public const string TYPE = "lsel";
	public override string DisplayName { get { return "LayerSelectorProperty"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("ipco"); }


	protected ushort layer_id; 
	public ushort LayerId { get { return this.layer_id; } set { this.layer_id = value; } }

	public LayerSelectorProperty(): base(IsoStream.FromFourCC("lsel"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.layer_id, "layer_id"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.layer_id, "layer_id"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // layer_id
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("layer_id", 16, this.layer_id); 
		return boxSize;
	}
}

}
