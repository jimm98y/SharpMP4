using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class HEVCTileSSHInfoSampleEntry() extends VisualSampleEntry ('hvt3'){
	HEVCTileConfigurationBox config(); // optional 
}
*/
public partial class HEVCTileSSHInfoSampleEntry : VisualSampleEntry
{
	public const string TYPE = "hvt3";
	public override string DisplayName { get { return "HEVCTileSSHInfoSampleEntry"; } }
	public HEVCTileConfigurationBox Config { get { return this.children.OfType<HEVCTileConfigurationBox>().FirstOrDefault(); } }

	public HEVCTileSSHInfoSampleEntry(): base(IsoStream.FromFourCC("hvt3"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.config, "config"); // optional 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.config, "config"); // optional 
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
