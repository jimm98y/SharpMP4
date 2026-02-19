using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*

*/
public partial class EVCSliceComponentTrackSampleEntryevs2Dup : VisualSampleEntry
{
	public const string TYPE = "evs2";
	public override string DisplayName { get { return "EVCSliceComponentTrackSampleEntryevs2Dup"; } }
	public EVCSliceComponentTrackConfigurationBox Config { get { return this.children.OfType<EVCSliceComponentTrackConfigurationBox>().FirstOrDefault(); } }

	public EVCSliceComponentTrackSampleEntryevs2Dup(): base(IsoStream.FromFourCC("evs2"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.config, "config"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.config, "config"); 
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
