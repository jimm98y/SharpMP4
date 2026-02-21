using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class SimpleTextSampleEntry(codingname) extends PlainTextSampleEntry ('stxt') { 
 string content_encoding; // optional 
 string mime_format; 
 BitRateBox (); 
    // optional 
 TextConfigBox (); 
 
   // optional 
} 
*/
public partial class SimpleTextSampleEntry : PlainTextSampleEntry
{
	public const string TYPE = "stxt";
	public override string DisplayName { get { return "SimpleTextSampleEntry"; } }

	protected BinaryUTF8String content_encoding;  //  optional 
	public BinaryUTF8String ContentEncoding { get { return this.content_encoding; } set { this.content_encoding = value; } }

	protected BinaryUTF8String mime_format; 
	public BinaryUTF8String MimeFormat { get { return this.mime_format; } set { this.mime_format = value; } }
	public BitRateBox _BitRateBox { get { return this.children.OfType<BitRateBox>().FirstOrDefault(); } }
	public TextConfigBox _TextConfigBox { get { return this.children.OfType<TextConfigBox>().FirstOrDefault(); } }

	public SimpleTextSampleEntry(uint codingname = 0): base(IsoStream.FromFourCC("stxt"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.content_encoding, "content_encoding"); // optional 
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
		boxSize += stream.WriteStringZeroTerminated( this.content_encoding, "content_encoding"); // optional 
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
		boxSize += IsoStream.CalculateStringSize(content_encoding); // content_encoding
		boxSize += IsoStream.CalculateStringSize(mime_format); // mime_format
		// boxSize += IsoStream.CalculateBoxSize(BitRateBox); // BitRateBox
		// boxSize += IsoStream.CalculateBoxSize(TextConfigBox); // TextConfigBox
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
