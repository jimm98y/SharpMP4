using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

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
	public const string TYPE = "wvtt";
	public override string DisplayName { get { return "WVTTSampleEntry"; } }
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
}

}
