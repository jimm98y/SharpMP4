using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ScrambleSchemeInfoBox extends Box('scrb') {
	SchemeTypeBox scheme_type_box;
	SchemeInformationBox info; // optional
}
*/
public partial class ScrambleSchemeInfoBox : Box
{
	public const string TYPE = "scrb";
	public override string DisplayName { get { return "ScrambleSchemeInfoBox"; } }
	public SchemeTypeBox SchemeTypeBox { get { return this.children.OfType<SchemeTypeBox>().FirstOrDefault(); } }
	public SchemeInformationBox Info { get { return this.children.OfType<SchemeInformationBox>().FirstOrDefault(); } }

	public ScrambleSchemeInfoBox(): base(IsoStream.FromFourCC("scrb"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.scheme_type_box, "scheme_type_box"); 
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.info, "info"); // optional
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.scheme_type_box, "scheme_type_box"); 
		// boxSize += stream.WriteBox( this.info, "info"); // optional
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(scheme_type_box); // scheme_type_box
		// boxSize += IsoStream.CalculateBoxSize(info); // info
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
