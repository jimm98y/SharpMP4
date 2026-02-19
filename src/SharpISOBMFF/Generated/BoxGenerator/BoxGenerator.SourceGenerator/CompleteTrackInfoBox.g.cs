using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class CompleteTrackInfoBox(fmt) extends Box('cinf') {
	OriginalFormatBox(fmt)	original_format;
}
*/
public partial class CompleteTrackInfoBox : Box
{
	public const string TYPE = "cinf";
	public override string DisplayName { get { return "CompleteTrackInfoBox"; } }
	public OriginalFormatBox OriginalFormat { get { return this.children.OfType<OriginalFormatBox>().FirstOrDefault(); } }

	public CompleteTrackInfoBox(uint fmt = 0): base(IsoStream.FromFourCC("cinf"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.original_format, "original_format"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.original_format, "original_format"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(original_format); // original_format
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
