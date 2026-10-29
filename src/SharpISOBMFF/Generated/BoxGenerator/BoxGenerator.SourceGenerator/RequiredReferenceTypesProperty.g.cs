using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

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
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class RequiredReferenceTypesProperty
	extends ItemFullProperty('rref', version = 0, flags = 0){
	unsigned int(8) reference_type_count;
	for (i=0; i< reference_type_count; i++) {
		unsigned int(32) reference_type[i];
	}
}"; } }
	public const string TYPE = "rref";
	public override string DisplayName { get { return "RequiredReferenceTypesProperty"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("ipco"); }


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

		this.reference_type = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt( reference_type_count), "reference_type");
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

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("reference_type_count", 8, this.reference_type_count); 

		for (int i=0; i< reference_type_count; i++)
		{
			boxSize += tracer.Field($"reference_type[{i}]", 32, this.reference_type[i]); 
		}
		return boxSize;
	}
}

}
