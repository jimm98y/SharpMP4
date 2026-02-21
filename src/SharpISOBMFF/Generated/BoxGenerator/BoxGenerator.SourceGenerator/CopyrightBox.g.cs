using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class CopyrightBox
	extends FullBox('cprt', version = 0, 0) {
	const bit(1) pad = 0;
	unsigned int(5)[3] language; // ISO-639-2/T language code
	utfstring notice;
}
*/
public partial class CopyrightBox : FullBox
{
	public const string TYPE = "cprt";
	public override string DisplayName { get { return "CopyrightBox"; } }

	protected bool pad = false; 
	public bool Pad { get { return this.pad; } set { this.pad = value; } }

	protected string language;  //  ISO-639-2/T language code
	public string Language { get { return this.language; } set { this.language = value; } }

	protected BinaryUTF8String notice; 
	public BinaryUTF8String Notice { get { return this.notice; } set { this.notice = value; } }

	public CopyrightBox(): base(IsoStream.FromFourCC("cprt"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.pad, "pad"); 
		boxSize += stream.ReadIso639(boxSize, readSize,  out this.language, "language"); // ISO-639-2/T language code
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.notice, "notice"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBit( this.pad, "pad"); 
		boxSize += stream.WriteIso639( this.language, "language"); // ISO-639-2/T language code
		boxSize += stream.WriteStringZeroTerminated( this.notice, "notice"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 1; // pad
		boxSize += 15; // language
		boxSize += IsoStream.CalculateStringSize(notice); // notice
		return boxSize;
	}
}

}
