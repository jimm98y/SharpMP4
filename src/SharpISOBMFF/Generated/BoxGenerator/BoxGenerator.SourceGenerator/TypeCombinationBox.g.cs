using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class TypeCombinationBox extends Box('tyco') {
	unsigned int(32)	compatible_brands[];	// to end of the box
}
*/
public partial class TypeCombinationBox : Box
{
	public const string TYPE = "tyco";
	public override string DisplayName { get { return "TypeCombinationBox"; } }

	protected uint[] compatible_brands;  //  to end of the box
	public uint[] CompatibleBrands { get { return this.compatible_brands; } set { this.compatible_brands = value; } }

	public TypeCombinationBox(): base(IsoStream.FromFourCC("tyco"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32ArrayTillEnd(boxSize, readSize,  out this.compatible_brands, "compatible_brands"); // to end of the box
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32ArrayTillEnd( this.compatible_brands, "compatible_brands"); // to end of the box
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += ((ulong)compatible_brands.Length * 32); // compatible_brands
		return boxSize;
	}
}

}
