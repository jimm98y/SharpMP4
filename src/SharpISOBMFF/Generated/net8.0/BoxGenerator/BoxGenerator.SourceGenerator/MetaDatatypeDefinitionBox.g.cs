using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class MetaDatatypeDefinitionBox extends Box('dtyp') {
 unsigned int(32) datatype_namespace;
 unsigned int(8) datatype_array[];
}

*/
public partial class MetaDatatypeDefinitionBox : Box
{
	public const string TYPE = "dtyp";
	public override string DisplayName { get { return "MetaDatatypeDefinitionBox"; } }

	protected uint datatype_namespace; 
	public uint DatatypeNamespace { get { return this.datatype_namespace; } set { this.datatype_namespace = value; } }

	protected byte[] datatype_array; 
	public byte[] DatatypeArray { get { return this.datatype_array; } set { this.datatype_array = value; } }

	public MetaDatatypeDefinitionBox(): base(IsoStream.FromFourCC("dtyp"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.datatype_namespace, "datatype_namespace"); 
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.datatype_array, "datatype_array"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.datatype_namespace, "datatype_namespace"); 
		boxSize += stream.WriteUInt8ArrayTillEnd( this.datatype_array, "datatype_array"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // datatype_namespace
		boxSize += ((ulong)datatype_array.Length * 8); // datatype_array
		return boxSize;
	}
}

}
