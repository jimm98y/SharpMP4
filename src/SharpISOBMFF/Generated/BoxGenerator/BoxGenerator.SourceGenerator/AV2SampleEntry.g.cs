using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class AV2SampleEntry extends VisualSampleEntry('av02') {
  AV2CodecConfigurationBox config;
}

*/
public partial class AV2SampleEntry : VisualSampleEntry
{
	public const string TYPE = "av02";
	public override string DisplayName { get { return "AV2SampleEntry"; } }
	public AV2CodecConfigurationBox Config { get { return this.children.OfType<AV2CodecConfigurationBox>().FirstOrDefault(); } }

	public AV2SampleEntry(): base(IsoStream.FromFourCC("av02"))
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
