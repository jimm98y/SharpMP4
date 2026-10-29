using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class HEVCTileSampleEntry() extends VisualSampleEntry ('hvt1'){
	HEVCTileConfigurationBox	config(); // optional
}
*/
public partial class HEVCTileSampleEntry : VisualSampleEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class HEVCTileSampleEntry() extends VisualSampleEntry ('hvt1'){
	HEVCTileConfigurationBox	config(); // optional
}"; } }
	public const string TYPE = "hvt1";
	public override string DisplayName { get { return "HEVCTileSampleEntry"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stsd"); }

	public HEVCTileConfigurationBox Config { get { return this.children.OfType<HEVCTileConfigurationBox>().FirstOrDefault(); } }

	public HEVCTileSampleEntry(): base(IsoStream.FromFourCC("hvt1"))
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

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		// boxSize += tracer.Field("config", IsoStream.CalculateBoxSize(config), this.config); // optional
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
