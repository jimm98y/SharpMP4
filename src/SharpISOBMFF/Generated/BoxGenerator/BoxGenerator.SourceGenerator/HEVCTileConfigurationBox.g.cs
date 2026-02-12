using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class HEVCTileConfigurationBox extends Box('hvtC') {
	HEVCTileTierLevelConfigurationRecord() HEVCTileTierLevelConfig;
}
*/
public partial class HEVCTileConfigurationBox : Box
{
	public const string TYPE = "hvtC";
	public override string DisplayName { get { return "HEVCTileConfigurationBox"; } }

	protected HEVCTileTierLevelConfigurationRecord HEVCTileTierLevelConfig; 
	public HEVCTileTierLevelConfigurationRecord _HEVCTileTierLevelConfig { get { return this.HEVCTileTierLevelConfig; } set { this.HEVCTileTierLevelConfig = value; } }

	public HEVCTileConfigurationBox(): base(IsoStream.FromFourCC("hvtC"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new HEVCTileTierLevelConfigurationRecord(),  out this.HEVCTileTierLevelConfig, "HEVCTileTierLevelConfig"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.HEVCTileTierLevelConfig, "HEVCTileTierLevelConfig"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(HEVCTileTierLevelConfig); // HEVCTileTierLevelConfig
		return boxSize;
	}
}

}
