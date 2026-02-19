using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class GenericSampleEntry extends Box('encv') {
	// ProtectionSchemeInfoBox {
		// OriginalFormatBox;	// data_format is 'resv'
		// SchemeTypeBox;
		// SchemeInformationBox;
	// }
// tRestrictedSchemeInfoBox {
		// OriginalFormatBox; // data_format indicates a codec, e.g. 'avc1'
		// SchemeTypeBox;
		// SchemeInformationBox;
	// }
	// Boxes specific to the untransformed sample entry type
	// For 'avc1', these would include AVCConfigurationBox
}
*/
public partial class GenericSampleEntry : Box
{
	public const string TYPE = "encv";
	public override string DisplayName { get { return "GenericSampleEntry"; } }

	public GenericSampleEntry(): base(IsoStream.FromFourCC("encv"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		/*  ProtectionSchemeInfoBox { */
		/*  OriginalFormatBox;	// data_format is 'resv' */
		/*  SchemeTypeBox; */
		/*  SchemeInformationBox; */
		/*  } */
		/*  tRestrictedSchemeInfoBox { */
		/*  OriginalFormatBox; // data_format indicates a codec, e.g. 'avc1' */
		/*  SchemeTypeBox; */
		/*  SchemeInformationBox; */
		/*  } */
		/*  Boxes specific to the untransformed sample entry type */
		/*  For 'avc1', these would include AVCConfigurationBox */
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		/*  ProtectionSchemeInfoBox { */
		/*  OriginalFormatBox;	// data_format is 'resv' */
		/*  SchemeTypeBox; */
		/*  SchemeInformationBox; */
		/*  } */
		/*  tRestrictedSchemeInfoBox { */
		/*  OriginalFormatBox; // data_format indicates a codec, e.g. 'avc1' */
		/*  SchemeTypeBox; */
		/*  SchemeInformationBox; */
		/*  } */
		/*  Boxes specific to the untransformed sample entry type */
		/*  For 'avc1', these would include AVCConfigurationBox */
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		/*  ProtectionSchemeInfoBox { */
		/*  OriginalFormatBox;	// data_format is 'resv' */
		/*  SchemeTypeBox; */
		/*  SchemeInformationBox; */
		/*  } */
		/*  tRestrictedSchemeInfoBox { */
		/*  OriginalFormatBox; // data_format indicates a codec, e.g. 'avc1' */
		/*  SchemeTypeBox; */
		/*  SchemeInformationBox; */
		/*  } */
		/*  Boxes specific to the untransformed sample entry type */
		/*  For 'avc1', these would include AVCConfigurationBox */
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
