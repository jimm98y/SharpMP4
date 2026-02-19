using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class AV1SampleEntry
extends VisualSampleEntry('av01')
{
  AV1CodecConfigurationBox config;
}

*/
public partial class AV1SampleEntry : VisualSampleEntry
{
	public const string TYPE = "av01";
	public override string DisplayName { get { return "AV1SampleEntry"; } }
	public AV1CodecConfigurationBox Config { get { return this.children.OfType<AV1CodecConfigurationBox>().FirstOrDefault(); } }

	public AV1SampleEntry(): base(IsoStream.FromFourCC("av01"))
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
