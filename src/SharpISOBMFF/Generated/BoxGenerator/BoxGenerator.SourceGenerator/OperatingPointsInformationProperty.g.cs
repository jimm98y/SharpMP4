using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

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
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class OperatingPointsInformationProperty
	extends ItemFullProperty('oinf', version = 0, flags = 0){
	OperatingPointsRecord op_info; // specified in ISO/IEC 14496-15
}"; } }
	public const string TYPE = "oinf";
	public override string DisplayName { get { return "OperatingPointsInformationProperty"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("ipco"); }


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

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("op_info", IsoStream.CalculateClassSize(op_info), this.op_info); // specified in ISO/IEC 14496-15
		return boxSize;
	}
}

}
