using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class WebVTTConfigurationBox extends Box('vttC') {
	boxstring	config;
}
*/
public partial class WebVTTConfigurationBox : Box
{
	public const string TYPE = "vttC";
	public override string DisplayName { get { return "WebVTTConfigurationBox"; } }

	protected BinaryUTF8String config; 
	public BinaryUTF8String Config { get { return this.config; } set { this.config = value; } }

	public WebVTTConfigurationBox(): base(IsoStream.FromFourCC("vttC"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.config, "config"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.config, "config"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(config); // config
		return boxSize;
	}
}

}
