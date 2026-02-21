using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class OperatingPointsInformation extends VisualSampleGroupEntry ('oinf') {
	OperatingPointsRecord oinf;
}
*/
public partial class OperatingPointsInformation : VisualSampleGroupEntry
{
	public const string TYPE = "oinf";
	public override string DisplayName { get { return "OperatingPointsInformation"; } }

	protected OperatingPointsRecord oinf; 
	public OperatingPointsRecord Oinf { get { return this.oinf; } set { this.oinf = value; } }

	public OperatingPointsInformation(): base(IsoStream.FromFourCC("oinf"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new OperatingPointsRecord(),  out this.oinf, "oinf"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.oinf, "oinf"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(oinf); // oinf
		return boxSize;
	}
}

}
