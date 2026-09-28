using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// as FFmpeg's ff_isom_write_lvcc writes it
class LCEVCConfigurationBox extends Box('lvcC') {
	LCEVCDecoderConfigurationRecord() LCEVCConfig;
}
*/
public partial class LCEVCConfigurationBox : Box
{
	public const string TYPE = "lvcC";
	public override string DisplayName { get { return "LCEVCConfigurationBox"; } }

	protected LCEVCDecoderConfigurationRecord LCEVCConfig; 
	public LCEVCDecoderConfigurationRecord _LCEVCConfig { get { return this.LCEVCConfig; } set { this.LCEVCConfig = value; } }

	public LCEVCConfigurationBox(): base(IsoStream.FromFourCC("lvcC"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new LCEVCDecoderConfigurationRecord(),  out this.LCEVCConfig, "LCEVCConfig"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.LCEVCConfig, "LCEVCConfig"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(LCEVCConfig); // LCEVCConfig
		return boxSize;
	}
}

}
