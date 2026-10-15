using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class IncompleteAVCSampleEntry() extends VisualSampleEntry ('icpv'){
	CompleteTrackInfoBox();
	AVCConfigurationBox config;
}
*/
public partial class IncompleteAVCSampleEntry : VisualSampleEntry
{
	public const string TYPE = "icpv";
	public override string DisplayName { get { return "IncompleteAVCSampleEntry"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stsd"); }

	public CompleteTrackInfoBox _CompleteTrackInfoBox { get { return this.children.OfType<CompleteTrackInfoBox>().FirstOrDefault(); } }
	public AVCConfigurationBox Config { get { return this.children.OfType<AVCConfigurationBox>().FirstOrDefault(); } }

	public IncompleteAVCSampleEntry(): base(IsoStream.FromFourCC("icpv"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.CompleteTrackInfoBox, "CompleteTrackInfoBox"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.config, "config"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.CompleteTrackInfoBox, "CompleteTrackInfoBox"); 
		// boxSize += stream.WriteBox( this.config, "config"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(CompleteTrackInfoBox); // CompleteTrackInfoBox
		// boxSize += IsoStream.CalculateBoxSize(config); // config
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += base.ToString(builder);
		// boxSize += builder.Field("CompleteTrackInfoBox", IsoStream.CalculateBoxSize(CompleteTrackInfoBox), this.CompleteTrackInfoBox); 
		// boxSize += builder.Field("config", IsoStream.CalculateBoxSize(config), this.config); 
		boxSize += builder.Children(this);
		return boxSize;
	}
}

}
