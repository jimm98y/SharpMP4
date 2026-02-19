using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class MVCConfigurationBox extends Box('mvcC') {
	MVCDecoderConfigurationRecord() MVCConfig;
}
*/
public partial class MVCConfigurationBox : Box
{
	public const string TYPE = "mvcC";
	public override string DisplayName { get { return "MVCConfigurationBox"; } }

	protected MVCDecoderConfigurationRecord MVCConfig; 
	public MVCDecoderConfigurationRecord _MVCConfig { get { return this.MVCConfig; } set { this.MVCConfig = value; } }

	public MVCConfigurationBox(): base(IsoStream.FromFourCC("mvcC"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new MVCDecoderConfigurationRecord(),  out this.MVCConfig, "MVCConfig"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.MVCConfig, "MVCConfig"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(MVCConfig); // MVCConfig
		return boxSize;
	}
}

}
