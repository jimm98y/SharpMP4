using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class VvcSampleEntry() extends VisualSampleEntry ('vvc1' or 'vvi1') {
	VvcConfigurationBox	config;
	MPEG4ExtensionDescriptorsBox () descr;	// optional
}
*/
public partial class VvcSampleEntry : VisualSampleEntry
{
	public const string TYPE = "vvc1";
	public override string DisplayName { get { return "VvcSampleEntry"; } }
	public VvcConfigurationBox Config { get { return this.children.OfType<VvcConfigurationBox>().FirstOrDefault(); } }
	public MPEG4ExtensionDescriptorsBox Descr { get { return this.children.OfType<MPEG4ExtensionDescriptorsBox>().FirstOrDefault(); } }

	public VvcSampleEntry(): base(IsoStream.FromFourCC("vvc1"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.config, "config"); 
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.descr, "descr"); // optional
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.config, "config"); 
		// boxSize += stream.WriteBox( this.descr, "descr"); // optional
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(config); // config
		// boxSize += IsoStream.CalculateBoxSize(descr); // descr
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
