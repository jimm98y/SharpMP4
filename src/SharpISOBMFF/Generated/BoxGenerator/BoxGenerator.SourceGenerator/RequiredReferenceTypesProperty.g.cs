using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class RequiredReferenceTypesProperty
extends ItemFullProperty('rref', version = 0, flags = 0){
	unsigned int(8) reference_type_count;
	for (i=0; i< reference_type_count; i++) {
		unsigned int(32) reference_type[i];
	}
}
*/
public partial class RequiredReferenceTypesProperty : ItemFullProperty
{
	public const string TYPE = "rref";
	public override string DisplayName { get { return "RequiredReferenceTypesProperty"; } }

	protected byte reference_type_count; 
	public byte ReferenceTypeCount { get { return this.reference_type_count; } set { this.reference_type_count = value; } }

	protected uint[] reference_type; 
	public uint[] ReferenceType { get { return this.reference_type; } set { this.reference_type = value; } }

	public RequiredReferenceTypesProperty(): base(IsoStream.FromFourCC("rref"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.reference_type_count, "reference_type_count"); 

		this.reference_type = new uint[IsoStream.GetInt( reference_type_count)];
		for (int i=0; i< reference_type_count; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reference_type[i], "reference_type"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.reference_type_count, "reference_type_count"); 

		for (int i=0; i< reference_type_count; i++)
		{
			boxSize += stream.WriteUInt32( this.reference_type[i], "reference_type"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // reference_type_count

		for (int i=0; i< reference_type_count; i++)
		{
			boxSize += 32; // reference_type
		}
		return boxSize;
	}
}

}
