// SharpMP4
// Copyright (C) 2026 Lukas Volf
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
// SOFTWARE.

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
