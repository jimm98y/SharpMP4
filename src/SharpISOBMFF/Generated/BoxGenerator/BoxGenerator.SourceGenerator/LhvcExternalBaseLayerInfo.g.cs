using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class LhvcExternalBaseLayerInfo() extends VisualSampleGroupEntry('lbli')
{
	bit(1) reserved = 1;
	unsigned int(1) bl_irap_pic_flag;
	unsigned int(6) bl_irap_nal_unit_type;
	signed   int(8) sample_offset;
}
*/
public partial class LhvcExternalBaseLayerInfo : VisualSampleGroupEntry
{
	public const string TYPE = "lbli";
	public override string DisplayName { get { return "LhvcExternalBaseLayerInfo"; } }

	protected bool reserved = true; 
	public bool Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected bool bl_irap_pic_flag; 
	public bool BlIrapPicFlag { get { return this.bl_irap_pic_flag; } set { this.bl_irap_pic_flag = value; } }

	protected byte bl_irap_nal_unit_type; 
	public byte BlIrapNalUnitType { get { return this.bl_irap_nal_unit_type; } set { this.bl_irap_nal_unit_type = value; } }

	protected sbyte sample_offset; 
	public sbyte SampleOffset { get { return this.sample_offset; } set { this.sample_offset = value; } }

	public LhvcExternalBaseLayerInfo(): base(IsoStream.FromFourCC("lbli"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.bl_irap_pic_flag, "bl_irap_pic_flag"); 
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.bl_irap_nal_unit_type, "bl_irap_nal_unit_type"); 
		boxSize += stream.ReadInt8(boxSize, readSize,  out this.sample_offset, "sample_offset"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBit( this.reserved, "reserved"); 
		boxSize += stream.WriteBit( this.bl_irap_pic_flag, "bl_irap_pic_flag"); 
		boxSize += stream.WriteBits(6,  this.bl_irap_nal_unit_type, "bl_irap_nal_unit_type"); 
		boxSize += stream.WriteInt8( this.sample_offset, "sample_offset"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 1; // reserved
		boxSize += 1; // bl_irap_pic_flag
		boxSize += 6; // bl_irap_nal_unit_type
		boxSize += 8; // sample_offset
		return boxSize;
	}
}

}
