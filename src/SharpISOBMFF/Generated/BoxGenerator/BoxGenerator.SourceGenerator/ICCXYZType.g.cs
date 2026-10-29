using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ICCXYZType {
	unsigned int(32) type_signature;
	unsigned int(32) reserved = 0;
	ICCXYZNumber values[];	// to the end of the element
}
*/
public partial class ICCXYZType : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"aligned(8) class ICCXYZType {
	unsigned int(32) type_signature;
	unsigned int(32) reserved = 0;
	ICCXYZNumber values[];	// to the end of the element
}"; } }
	public virtual string DisplayName { get { return "ICCXYZType"; } }

	protected uint type_signature; 
	public uint TypeSignature { get { return this.type_signature; } set { this.type_signature = value; } }

	protected uint reserved = 0; 
	public uint Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected ICCXYZNumber[] values = [];  //  to the end of the element
	public ICCXYZNumber[] Values { get { return this.values; } set { this.values = value; } }

	public ICCXYZType(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.type_signature, "type_signature"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(uint.MaxValue), () => new ICCXYZNumber(),  out this.values, "values"); // to the end of the element
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt32( this.type_signature, "type_signature"); 
		boxSize += stream.WriteUInt32( this.reserved, "reserved"); 
		boxSize += stream.WriteClass( this.values, "values"); // to the end of the element
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 32; // type_signature
		boxSize += 32; // reserved
		boxSize += IsoStream.CalculateClassSize(values); // values
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("type_signature", 32, this.type_signature); 
		boxSize += tracer.Field("reserved", 32, this.reserved); 
		boxSize += tracer.Field("values", IsoStream.CalculateClassSize(values), this.values); // to the end of the element
		return boxSize;
	}
}

}
