using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class WVTTSampleEntry() extends PlainTextSampleEntry ('wvtt'){
	WebVTTConfigurationBox	config;
	WebVTTSourceLabelBox		label;	// recommended
	BitRateBox (); 					// optional
}
*/
public partial class WVTTSampleEntry : PlainTextSampleEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class WVTTSampleEntry() extends PlainTextSampleEntry ('wvtt'){
	WebVTTConfigurationBox	config;
	WebVTTSourceLabelBox		label;	// recommended
	BitRateBox (); 					// optional
}"; } }
	public const string TYPE = "wvtt";
	public override string DisplayName { get { return "WVTTSampleEntry"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stsd"); }

	public WebVTTConfigurationBox Config { get { return this.children.OfType<WebVTTConfigurationBox>().FirstOrDefault(); } }
	public WebVTTSourceLabelBox Label { get { return this.children.OfType<WebVTTSourceLabelBox>().FirstOrDefault(); } }
	public BitRateBox _BitRateBox { get { return this.children.OfType<BitRateBox>().FirstOrDefault(); } }

	public WVTTSampleEntry(): base(IsoStream.FromFourCC("wvtt"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.config, "config"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.label, "label"); // recommended
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.BitRateBox, "BitRateBox"); // optional
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.config, "config"); 
		// boxSize += stream.WriteBox( this.label, "label"); // recommended
		// boxSize += stream.WriteBox( this.BitRateBox, "BitRateBox"); // optional
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(config); // config
		// boxSize += IsoStream.CalculateBoxSize(label); // label
		// boxSize += IsoStream.CalculateBoxSize(BitRateBox); // BitRateBox
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		// boxSize += tracer.Field("config", IsoStream.CalculateBoxSize(config), this.config); 
		// boxSize += tracer.Field("label", IsoStream.CalculateBoxSize(label), this.label); // recommended
		// boxSize += tracer.Field("BitRateBox", IsoStream.CalculateBoxSize(BitRateBox), this.BitRateBox); // optional
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
