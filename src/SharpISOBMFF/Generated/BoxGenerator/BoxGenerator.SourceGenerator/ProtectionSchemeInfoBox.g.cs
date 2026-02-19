using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ProtectionSchemeInfoBox(fmt) extends Box('sinf') {
	OriginalFormatBox(fmt)	original_format;

	SchemeTypeBox			scheme_type_box;		// optional
	SchemeInformationBox	info;						// optional
}
*/
public partial class ProtectionSchemeInfoBox : Box
{
	public const string TYPE = "sinf";
	public override string DisplayName { get { return "ProtectionSchemeInfoBox"; } }
	public OriginalFormatBox OriginalFormat { get { return this.children.OfType<OriginalFormatBox>().FirstOrDefault(); } }
	public SchemeTypeBox SchemeTypeBox { get { return this.children.OfType<SchemeTypeBox>().FirstOrDefault(); } }
	public SchemeInformationBox Info { get { return this.children.OfType<SchemeInformationBox>().FirstOrDefault(); } }

	public ProtectionSchemeInfoBox(uint fmt = 0): base(IsoStream.FromFourCC("sinf"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.original_format, "original_format"); 
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.scheme_type_box, "scheme_type_box"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.info, "info"); // optional
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.original_format, "original_format"); 
		// boxSize += stream.WriteBox( this.scheme_type_box, "scheme_type_box"); // optional
		// boxSize += stream.WriteBox( this.info, "info"); // optional
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(original_format); // original_format
		// boxSize += IsoStream.CalculateBoxSize(scheme_type_box); // scheme_type_box
		// boxSize += IsoStream.CalculateBoxSize(info); // info
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
