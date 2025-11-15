using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class AuxiliaryTypeProperty
extends ItemFullProperty('auxC', version = 0, flags) {
	string aux_type;
	template unsigned int(8) aux_subtype[];
		// until the end of the box, the semantics depend on the aux_type value
}
*/
public partial class AuxiliaryTypeProperty : ItemFullProperty
{
	public const string TYPE = "auxC";
	public override string DisplayName { get { return "AuxiliaryTypeProperty"; } }

	protected BinaryUTF8String aux_type; 
	public BinaryUTF8String AuxType { get { return this.aux_type; } set { this.aux_type = value; } }

	protected byte[] aux_subtype;  //  until the end of the box, the semantics depend on the aux_type value
	public byte[] AuxSubtype { get { return this.aux_subtype; } set { this.aux_subtype = value; } }

	public AuxiliaryTypeProperty(uint flags = 0): base(IsoStream.FromFourCC("auxC"), 0, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.aux_type, "aux_type"); 
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.aux_subtype, "aux_subtype"); // until the end of the box, the semantics depend on the aux_type value
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.aux_type, "aux_type"); 
		boxSize += stream.WriteUInt8ArrayTillEnd( this.aux_subtype, "aux_subtype"); // until the end of the box, the semantics depend on the aux_type value
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(aux_type); // aux_type
		boxSize += ((ulong)aux_subtype.Length * 8); // aux_subtype
		return boxSize;
	}
}

}
