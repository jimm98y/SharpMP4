using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class HEVCSliceSegmentDataSampleEntry() extends VisualSampleEntry ('hvt2'){
	HEVCTileConfigurationBox	config(); // optional
}
*/
public partial class HEVCSliceSegmentDataSampleEntry : VisualSampleEntry
{
	public const string TYPE = "hvt2";
	public override string DisplayName { get { return "HEVCSliceSegmentDataSampleEntry"; } }
	public HEVCTileConfigurationBox Config { get { return this.children.OfType<HEVCTileConfigurationBox>().FirstOrDefault(); } }

	public HEVCSliceSegmentDataSampleEntry(): base(IsoStream.FromFourCC("hvt2"))
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
