using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class LHEVCSampleEntry() extends VisualSampleEntry ('lhv1') {
	LHEVCConfigurationBox		lhvcconfig;
	MPEG4ExtensionDescriptorsBox () descr;	// optional
}
*/
public partial class LHEVCSampleEntry : VisualSampleEntry
{
	public const string TYPE = "lhv1";
	public override string DisplayName { get { return "LHEVCSampleEntry"; } }
	public LHEVCConfigurationBox Lhvcconfig { get { return this.children.OfType<LHEVCConfigurationBox>().FirstOrDefault(); } }
	public MPEG4ExtensionDescriptorsBox Descr { get { return this.children.OfType<MPEG4ExtensionDescriptorsBox>().FirstOrDefault(); } }

	public LHEVCSampleEntry(): base(IsoStream.FromFourCC("lhv1"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.lhvcconfig, "lhvcconfig"); 
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.descr, "descr"); // optional
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.lhvcconfig, "lhvcconfig"); 
		// boxSize += stream.WriteBox( this.descr, "descr"); // optional
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(lhvcconfig); // lhvcconfig
		// boxSize += IsoStream.CalculateBoxSize(descr); // descr
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
