using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ExtendedTypeBox extends Box('etyp') {
	TypeCombinationBox	compatible_combinations[];	// to end of the box
}
*/
public partial class ExtendedTypeBox : Box
{
	public const string TYPE = "etyp";
	public override string DisplayName { get { return "ExtendedTypeBox"; } }
	public IEnumerable<TypeCombinationBox> CompatibleCombinations { get { return this.children.OfType<TypeCombinationBox>(); } }

	public ExtendedTypeBox(): base(IsoStream.FromFourCC("etyp"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.compatible_combinations, "compatible_combinations"); // to end of the box
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.compatible_combinations, "compatible_combinations"); // to end of the box
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(compatible_combinations); // compatible_combinations
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
