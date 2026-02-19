using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class EVCSliceComponentTrackConfigurationBox extends Box('evsC') {
	EVCSliceComponentTrackConfigurationRecord() config;
}
*/
public partial class EVCSliceComponentTrackConfigurationBox : Box
{
	public const string TYPE = "evsC";
	public override string DisplayName { get { return "EVCSliceComponentTrackConfigurationBox"; } }

	protected EVCSliceComponentTrackConfigurationRecord config; 
	public EVCSliceComponentTrackConfigurationRecord Config { get { return this.config; } set { this.config = value; } }

	public EVCSliceComponentTrackConfigurationBox(): base(IsoStream.FromFourCC("evsC"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new EVCSliceComponentTrackConfigurationRecord(),  out this.config, "config"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.config, "config"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(config); // config
		return boxSize;
	}
}

}
