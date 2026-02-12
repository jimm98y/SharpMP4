using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class OperatingPointsInformationProperty
extends ItemFullProperty('oinf', version = 0, flags = 0){
	OperatingPointsRecord op_info; // specified in ISO/IEC 14496-15
}
*/
public partial class OperatingPointsInformationProperty : ItemFullProperty
{
	public const string TYPE = "oinf";
	public override string DisplayName { get { return "OperatingPointsInformationProperty"; } }

	protected OperatingPointsRecord op_info;  //  specified in ISO/IEC 14496-15
	public OperatingPointsRecord OpInfo { get { return this.op_info; } set { this.op_info = value; } }

	public OperatingPointsInformationProperty(): base(IsoStream.FromFourCC("oinf"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new OperatingPointsRecord(),  out this.op_info, "op_info"); // specified in ISO/IEC 14496-15
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.op_info, "op_info"); // specified in ISO/IEC 14496-15
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(op_info); // op_info
		return boxSize;
	}
}

}
