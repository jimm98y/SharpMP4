using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ProjectionDataBox(unsigned int(32) proj_type, unsigned int(8) version, unsigned int(24) flags) extends FullBox(proj_type, version, flags) {
 }

*/
public partial class ProjectionDataBox : FullBox
{
	public override string DisplayName { get { return "ProjectionDataBox"; } }

	public ProjectionDataBox(uint proj_type, byte version = 0, uint flags = 0): base(proj_type, version, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		return boxSize;
	}
}

}
