using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class EVCSliceComponentTrackSampleEntry() 
      extends VisualSampleEntry ('evs1' or 'evs2'){
	EVCSliceComponentTrackConfigurationBox	config;
}
*/
public partial class EVCSliceComponentTrackSampleEntry : VisualSampleEntry
{
	public const string TYPE = "evs1";
	public override string DisplayName { get { return "EVCSliceComponentTrackSampleEntry"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stsd"); }

	public EVCSliceComponentTrackConfigurationBox Config { get { return this.children.OfType<EVCSliceComponentTrackConfigurationBox>().FirstOrDefault(); } }

	public EVCSliceComponentTrackSampleEntry(): base(IsoStream.FromFourCC("evs1"))
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

	public override ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += base.ToString(builder);
		// boxSize += builder.Field("config", IsoStream.CalculateBoxSize(config), this.config); 
		boxSize += builder.Children(this);
		return boxSize;
	}
}

}
