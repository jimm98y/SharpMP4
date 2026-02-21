using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class HEVCLHVCSampleEntry() extends HEVCSampleEntry() {
	LHEVCConfigurationBox		lhvcconfig;
}
*/
public partial class HEVCLHVCSampleEntry : HEVCSampleEntry
{
	public override string DisplayName { get { return "HEVCLHVCSampleEntry"; } }
	public LHEVCConfigurationBox Lhvcconfig { get { return this.children.OfType<LHEVCConfigurationBox>().FirstOrDefault(); } }

	public HEVCLHVCSampleEntry(): base()
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.lhvcconfig, "lhvcconfig"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.lhvcconfig, "lhvcconfig"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(lhvcconfig); // lhvcconfig
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
