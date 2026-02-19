using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class HEVCConfigurationBox extends Box('hvcC') {
	HEVCDecoderConfigurationRecord() HEVCConfig;
}
*/
public partial class HEVCConfigurationBox : Box
{
	public const string TYPE = "hvcC";
	public override string DisplayName { get { return "HEVCConfigurationBox"; } }

	protected HEVCDecoderConfigurationRecord HEVCConfig; 
	public HEVCDecoderConfigurationRecord _HEVCConfig { get { return this.HEVCConfig; } set { this.HEVCConfig = value; } }

	public HEVCConfigurationBox(): base(IsoStream.FromFourCC("hvcC"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new HEVCDecoderConfigurationRecord(),  out this.HEVCConfig, "HEVCConfig"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.HEVCConfig, "HEVCConfig"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(HEVCConfig); // HEVCConfig
		return boxSize;
	}
}

}
