using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// the MIME type of a subtitle sample entry's samples; GPAC reads it as its TextConfigBox
aligned(8) class MIMEBox() extends FullBox('mime', version = 0, 0) {
	utf8string content_type;
}
*/
public partial class MIMEBox : FullBox
{
	public const string TYPE = "mime";
	public override string DisplayName { get { return "MIMEBox"; } }

	protected BinaryUTF8String content_type; 
	public BinaryUTF8String ContentType { get { return this.content_type; } set { this.content_type = value; } }

	public MIMEBox(): base(IsoStream.FromFourCC("mime"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.content_type, "content_type"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.content_type, "content_type"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(content_type); // content_type
		return boxSize;
	}
}

}
