using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

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
	public const string TYPE = "lsel";
	public override string DisplayName { get { return "LayerSelectorProperty"; } }

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
}

}
