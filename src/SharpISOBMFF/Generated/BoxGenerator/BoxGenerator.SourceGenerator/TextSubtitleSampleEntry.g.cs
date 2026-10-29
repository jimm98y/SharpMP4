using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class TextSubtitleSampleEntry() extends SubtitleSampleEntry ('sbtt') { 
 string content_encoding; // optional 
 string mime_format; 
 BitRateBox (); // optional 
 TextConfigBox (); // optional 
}
*/
public partial class TextSubtitleSampleEntry : SubtitleSampleEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class TextSubtitleSampleEntry() extends SubtitleSampleEntry ('sbtt') { 
 string content_encoding; // optional 
 string mime_format; 
 BitRateBox (); // optional 
 TextConfigBox (); // optional 
}"; } }
	public const string TYPE = "sbtt";
	public override string DisplayName { get { return "TextSubtitleSampleEntry"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stsd"); }


	protected BinaryUTF8String content_encoding;  //  optional 
	protected bool content_encodingPresent;
	public BinaryUTF8String ContentEncoding { get { return this.content_encoding; } set { this.content_encoding = value; this.content_encodingPresent = true; } }
	public bool ContentEncodingPresent { get { return this.content_encodingPresent; } set { this.content_encodingPresent = value; } }

	protected BinaryUTF8String mime_format; 
	public BinaryUTF8String MimeFormat { get { return this.mime_format; } set { this.mime_format = value; } }
	public BitRateBox _BitRateBox { get { return this.children.OfType<BitRateBox>().FirstOrDefault(); } }
	public TextConfigBox _TextConfigBox { get { return this.children.OfType<TextConfigBox>().FirstOrDefault(); } }

	public TextSubtitleSampleEntry(): base(IsoStream.FromFourCC("sbtt"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		if (stream.HasStringBeforeBoxes(boxSize, readSize)) { boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.content_encoding, "content_encoding"); this.content_encodingPresent = true; } // optional 
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.mime_format, "mime_format"); 
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.BitRateBox, "BitRateBox"); // optional 
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.TextConfigBox, "TextConfigBox"); // optional 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		if (this.content_encodingPresent) boxSize += stream.WriteStringZeroTerminated( this.content_encoding, "content_encoding"); // optional 
		boxSize += stream.WriteStringZeroTerminated( this.mime_format, "mime_format"); 
		// boxSize += stream.WriteBox( this.BitRateBox, "BitRateBox"); // optional 
		// boxSize += stream.WriteBox( this.TextConfigBox, "TextConfigBox"); // optional 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		if (this.content_encodingPresent) boxSize += IsoStream.CalculateStringSize(content_encoding); // content_encoding
		boxSize += IsoStream.CalculateStringSize(mime_format); // mime_format
		// boxSize += IsoStream.CalculateBoxSize(BitRateBox); // BitRateBox
		// boxSize += IsoStream.CalculateBoxSize(TextConfigBox); // TextConfigBox
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		if (this.content_encodingPresent) boxSize += tracer.Field("content_encoding", IsoStream.CalculateStringSize(content_encoding), this.content_encoding); // optional 
		boxSize += tracer.Field("mime_format", IsoStream.CalculateStringSize(mime_format), this.mime_format); 
		// boxSize += tracer.Field("BitRateBox", IsoStream.CalculateBoxSize(BitRateBox), this.BitRateBox); // optional 
		// boxSize += tracer.Field("TextConfigBox", IsoStream.CalculateBoxSize(TextConfigBox), this.TextConfigBox); // optional 
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
