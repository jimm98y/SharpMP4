using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class WebVTTSourceLabelBox extends Box('vlab') {
	boxstring	source_label;
}
*/
public partial class WebVTTSourceLabelBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class WebVTTSourceLabelBox extends Box('vlab') {
	boxstring	source_label;
}"; } }
	public const string TYPE = "vlab";
	public override string DisplayName { get { return "WebVTTSourceLabelBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("PlainTextSampleEntry(wvtt)"); }


	protected BinaryUTF8String source_label; 
	public BinaryUTF8String SourceLabel { get { return this.source_label; } set { this.source_label = value; } }

	public WebVTTSourceLabelBox(): base(IsoStream.FromFourCC("vlab"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.source_label, "source_label"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.source_label, "source_label"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(source_label); // source_label
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("source_label", IsoStream.CalculateStringSize(source_label), this.source_label); 
		return boxSize;
	}
}

}
