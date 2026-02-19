using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class VvcSubpicSampleEntry() extends VisualSampleEntry ('vvs1') {
	VvcNALUConfigBox config;
}
*/
public partial class VvcSubpicSampleEntry : VisualSampleEntry
{
	public const string TYPE = "vvs1";
	public override string DisplayName { get { return "VvcSubpicSampleEntry"; } }
	public VvcNALUConfigBox Config { get { return this.children.OfType<VvcNALUConfigBox>().FirstOrDefault(); } }

	public VvcSubpicSampleEntry(): base(IsoStream.FromFourCC("vvs1"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.config, "config"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.config, "config"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(config); // config
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
