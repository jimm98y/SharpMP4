using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class LHEVCConfigurationBox extends Box('lhvC') {
	LHEVCDecoderConfigurationRecord() LHEVCConfig;
}
*/
public partial class LHEVCConfigurationBox : Box
{
	public const string TYPE = "lhvC";
	public override string DisplayName { get { return "LHEVCConfigurationBox"; } }

	protected LHEVCDecoderConfigurationRecord LHEVCConfig; 
	public LHEVCDecoderConfigurationRecord _LHEVCConfig { get { return this.LHEVCConfig; } set { this.LHEVCConfig = value; } }

	public LHEVCConfigurationBox(): base(IsoStream.FromFourCC("lhvC"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new LHEVCDecoderConfigurationRecord(),  out this.LHEVCConfig, "LHEVCConfig"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.LHEVCConfig, "LHEVCConfig"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(LHEVCConfig); // LHEVCConfig
		return boxSize;
	}
}

}
