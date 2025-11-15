using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class SVCConfigurationBox extends Box('svcC') {
	SVCDecoderConfigurationRecord() SVCConfig;
}
*/
public partial class SVCConfigurationBox : Box
{
	public const string TYPE = "svcC";
	public override string DisplayName { get { return "SVCConfigurationBox"; } }

	protected SVCDecoderConfigurationRecord SVCConfig; 
	public SVCDecoderConfigurationRecord _SVCConfig { get { return this.SVCConfig; } set { this.SVCConfig = value; } }

	public SVCConfigurationBox(): base(IsoStream.FromFourCC("svcC"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new SVCDecoderConfigurationRecord(),  out this.SVCConfig, "SVCConfig"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.SVCConfig, "SVCConfig"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(SVCConfig); // SVCConfig
		return boxSize;
	}
}

}
