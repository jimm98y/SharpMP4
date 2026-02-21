using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class VvcOperatingPointsInformation extends VisualSampleGroupEntry ('vopi') {
	VvcOperatingPointsRecord oinf;
}
*/
public partial class VvcOperatingPointsInformation : VisualSampleGroupEntry
{
	public const string TYPE = "vopi";
	public override string DisplayName { get { return "VvcOperatingPointsInformation"; } }

	protected VvcOperatingPointsRecord oinf; 
	public VvcOperatingPointsRecord Oinf { get { return this.oinf; } set { this.oinf = value; } }

	public VvcOperatingPointsInformation(): base(IsoStream.FromFourCC("vopi"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new VvcOperatingPointsRecord(),  out this.oinf, "oinf"); 
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
