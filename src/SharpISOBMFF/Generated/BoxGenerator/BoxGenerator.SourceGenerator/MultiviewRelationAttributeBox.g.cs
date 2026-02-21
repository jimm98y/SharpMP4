using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class MultiviewRelationAttributeBox
	extends FullBox('mvra', version = 0, flags) {
	bit(16) reserved1 = 0;
	unsigned int(16) num_common_attributes;
	for (i=0; i<num_common_attributes; i++) {
		unsigned int(32) common_attribute; 
		unsigned int(32) common_value;
	}
	bit(16) reserved2 = 0;
	unsigned int(16) num_differentiating_attributes;
	for (i=0; i<num_differentiating_attributes; i++)
		unsigned int(32) differentiating_attribute;
}
*/
public partial class MultiviewRelationAttributeBox : FullBox
{
	public const string TYPE = "mvra";
	public override string DisplayName { get { return "MultiviewRelationAttributeBox"; } }

	protected ushort reserved1 = 0; 
	public ushort Reserved1 { get { return this.reserved1; } set { this.reserved1 = value; } }

	protected ushort num_common_attributes; 
	public ushort NumCommonAttributes { get { return this.num_common_attributes; } set { this.num_common_attributes = value; } }

	protected uint[] common_attribute; 
	public uint[] CommonAttribute { get { return this.common_attribute; } set { this.common_attribute = value; } }

	protected uint[] common_value; 
	public uint[] CommonValue { get { return this.common_value; } set { this.common_value = value; } }

	protected ushort reserved2 = 0; 
	public ushort Reserved2 { get { return this.reserved2; } set { this.reserved2 = value; } }

	protected ushort num_differentiating_attributes; 
	public ushort NumDifferentiatingAttributes { get { return this.num_differentiating_attributes; } set { this.num_differentiating_attributes = value; } }

	protected uint[] differentiating_attribute; 
	public uint[] DifferentiatingAttribute { get { return this.differentiating_attribute; } set { this.differentiating_attribute = value; } }

	public MultiviewRelationAttributeBox(uint flags = 0): base(IsoStream.FromFourCC("mvra"), 0, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.reserved1, "reserved1"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.num_common_attributes, "num_common_attributes"); 

		this.common_attribute = new uint[IsoStream.GetInt(num_common_attributes)];
		this.common_value = new uint[IsoStream.GetInt(num_common_attributes)];
		for (int i=0; i<num_common_attributes; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.common_attribute[i], "common_attribute"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.common_value[i], "common_value"); 
		}
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.reserved2, "reserved2"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.num_differentiating_attributes, "num_differentiating_attributes"); 

		this.differentiating_attribute = new uint[IsoStream.GetInt(num_differentiating_attributes)];
		for (int i=0; i<num_differentiating_attributes; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.differentiating_attribute[i], "differentiating_attribute"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.reserved1, "reserved1"); 
		boxSize += stream.WriteUInt16( this.num_common_attributes, "num_common_attributes"); 

		for (int i=0; i<num_common_attributes; i++)
		{
			boxSize += stream.WriteUInt32( this.common_attribute[i], "common_attribute"); 
			boxSize += stream.WriteUInt32( this.common_value[i], "common_value"); 
		}
		boxSize += stream.WriteUInt16( this.reserved2, "reserved2"); 
		boxSize += stream.WriteUInt16( this.num_differentiating_attributes, "num_differentiating_attributes"); 

		for (int i=0; i<num_differentiating_attributes; i++)
		{
			boxSize += stream.WriteUInt32( this.differentiating_attribute[i], "differentiating_attribute"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // reserved1
		boxSize += 16; // num_common_attributes

		for (int i=0; i<num_common_attributes; i++)
		{
			boxSize += 32; // common_attribute
			boxSize += 32; // common_value
		}
		boxSize += 16; // reserved2
		boxSize += 16; // num_differentiating_attributes

		for (int i=0; i<num_differentiating_attributes; i++)
		{
			boxSize += 32; // differentiating_attribute
		}
		return boxSize;
	}
}

}
