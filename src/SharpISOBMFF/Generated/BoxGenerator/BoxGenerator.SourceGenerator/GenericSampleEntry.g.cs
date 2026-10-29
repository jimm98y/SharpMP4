using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

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
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class GenericSampleEntry extends Box('encv') {
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
}"; } }
	public const string TYPE = "encv";
	public override string DisplayName { get { return "GenericSampleEntry"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stsd"); }


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

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
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
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
