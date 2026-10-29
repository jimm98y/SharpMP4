using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class TextConfigBox() extends FullBox ('txtC', 0, 0) {
	utf8string text_config;
}
*/
public partial class TextConfigBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class TextConfigBox() extends FullBox ('txtC', 0, 0) {
	utf8string text_config;
}"; } }
	public const string TYPE = "txtC";
	public override string DisplayName { get { return "TextConfigBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("TextMetaDataSampleEntry(mett)", "SimpleTextSampleEntry(stxt)", "TextSubtitleSampleEntry(sbtt)"); }


	protected BinaryUTF8String text_config; 
	public BinaryUTF8String TextConfig { get { return this.text_config; } set { this.text_config = value; } }

	public TextConfigBox(): base(IsoStream.FromFourCC("txtC"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.text_config, "text_config"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.text_config, "text_config"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(text_config); // text_config
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("text_config", IsoStream.CalculateStringSize(text_config), this.text_config); 
		return boxSize;
	}
}

}
