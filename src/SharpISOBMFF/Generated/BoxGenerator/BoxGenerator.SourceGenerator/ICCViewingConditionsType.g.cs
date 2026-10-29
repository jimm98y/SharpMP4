using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ICCViewingConditionsType {
	unsigned int(32) type_signature;
	unsigned int(32) reserved = 0;
	ICCXYZNumber illuminant;	// in cd/m2
	ICCXYZNumber surround;	// in cd/m2
	unsigned int(32) illuminant_type;
}
*/
public partial class ICCViewingConditionsType : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"aligned(8) class ICCViewingConditionsType {
	unsigned int(32) type_signature;
	unsigned int(32) reserved = 0;
	ICCXYZNumber illuminant;	// in cd/m2
	ICCXYZNumber surround;	// in cd/m2
	unsigned int(32) illuminant_type;
}"; } }
	public virtual string DisplayName { get { return "ICCViewingConditionsType"; } }

	protected uint type_signature; 
	public uint TypeSignature { get { return this.type_signature; } set { this.type_signature = value; } }

	protected uint reserved = 0; 
	public uint Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected ICCXYZNumber illuminant;  //  in cd/m2
	public ICCXYZNumber Illuminant { get { return this.illuminant; } set { this.illuminant = value; } }

	protected ICCXYZNumber surround;  //  in cd/m2
	public ICCXYZNumber Surround { get { return this.surround; } set { this.surround = value; } }

	protected uint illuminant_type; 
	public uint IlluminantType { get { return this.illuminant_type; } set { this.illuminant_type = value; } }

	public ICCViewingConditionsType(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.type_signature, "type_signature"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new ICCXYZNumber(),  out this.illuminant, "illuminant"); // in cd/m2
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new ICCXYZNumber(),  out this.surround, "surround"); // in cd/m2
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.illuminant_type, "illuminant_type"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt32( this.type_signature, "type_signature"); 
		boxSize += stream.WriteUInt32( this.reserved, "reserved"); 
		boxSize += stream.WriteClass( this.illuminant, "illuminant"); // in cd/m2
		boxSize += stream.WriteClass( this.surround, "surround"); // in cd/m2
		boxSize += stream.WriteUInt32( this.illuminant_type, "illuminant_type"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 32; // type_signature
		boxSize += 32; // reserved
		boxSize += IsoStream.CalculateClassSize(illuminant); // illuminant
		boxSize += IsoStream.CalculateClassSize(surround); // surround
		boxSize += 32; // illuminant_type
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("type_signature", 32, this.type_signature); 
		boxSize += tracer.Field("reserved", 32, this.reserved); 
		boxSize += tracer.Field("illuminant", IsoStream.CalculateClassSize(illuminant), this.illuminant); // in cd/m2
		boxSize += tracer.Field("surround", IsoStream.CalculateClassSize(surround), this.surround); // in cd/m2
		boxSize += tracer.Field("illuminant_type", 32, this.illuminant_type); 
		return boxSize;
	}
}

}
