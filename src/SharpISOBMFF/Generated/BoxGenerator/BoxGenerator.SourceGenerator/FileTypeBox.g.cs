using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class FileTypeBox
extends Box('ftyp')
{
unsigned int(32) major_brand;
unsigned int(32) minor_version;
unsigned int(32) compatible_brands[];// to end of the box
}
*/
public partial class FileTypeBox : Box
{
	public const string TYPE = "ftyp";
	public override string DisplayName { get { return "FileTypeBox"; } }

	protected uint major_brand; 
	public uint MajorBrand { get { return this.major_brand; } set { this.major_brand = value; } }

	protected uint minor_version; 
	public uint MinorVersion { get { return this.minor_version; } set { this.minor_version = value; } }

	protected uint[] compatible_brands;  //  to end of the box
	public uint[] CompatibleBrands { get { return this.compatible_brands; } set { this.compatible_brands = value; } }

	public FileTypeBox(): base(IsoStream.FromFourCC("ftyp"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.major_brand, "major_brand"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.minor_version, "minor_version"); 
		boxSize += stream.ReadUInt32ArrayTillEnd(boxSize, readSize,  out this.compatible_brands, "compatible_brands"); // to end of the box
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.major_brand, "major_brand"); 
		boxSize += stream.WriteUInt32( this.minor_version, "minor_version"); 
		boxSize += stream.WriteUInt32ArrayTillEnd( this.compatible_brands, "compatible_brands"); // to end of the box
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // major_brand
		boxSize += 32; // minor_version
		boxSize += ((ulong)compatible_brands.Length * 32); // compatible_brands
		return boxSize;
	}
}

}
