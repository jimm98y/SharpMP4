using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// ISO/IEC 23001-7:2016/Amd 1:2019, 8.4: the type of an item's auxiliary information, as a sample's in 14496-12
aligned(8) class ItemAuxiliaryInformationBox extends ItemFullProperty('iaux', version=0, flags=0) {
	unsigned int(32) aux_info_type;
	unsigned int(32) aux_info_type_parameter;
}
*/
public partial class ItemAuxiliaryInformationBox : ItemFullProperty
{
	public const string TYPE = "iaux";
	public override string DisplayName { get { return "ItemAuxiliaryInformationBox"; } }

	protected uint aux_info_type; 
	public uint AuxInfoType { get { return this.aux_info_type; } set { this.aux_info_type = value; } }

	protected uint aux_info_type_parameter; 
	public uint AuxInfoTypeParameter { get { return this.aux_info_type_parameter; } set { this.aux_info_type_parameter = value; } }

	public ItemAuxiliaryInformationBox(): base(IsoStream.FromFourCC("iaux"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.aux_info_type, "aux_info_type"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.aux_info_type_parameter, "aux_info_type_parameter"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.aux_info_type, "aux_info_type"); 
		boxSize += stream.WriteUInt32( this.aux_info_type_parameter, "aux_info_type_parameter"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // aux_info_type
		boxSize += 32; // aux_info_type_parameter
		return boxSize;
	}
}

}
