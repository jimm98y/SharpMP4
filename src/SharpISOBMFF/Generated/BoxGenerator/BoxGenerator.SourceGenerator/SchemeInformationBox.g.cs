using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SchemeInformationBox extends Box('schi') {
	Box	scheme_specific_data[];
}
*/
public partial class SchemeInformationBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class SchemeInformationBox extends Box('schi') {
	Box	scheme_specific_data[];
}"; } }
	public const string TYPE = "schi";
	public override string DisplayName { get { return "SchemeInformationBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("sinf", "rinf", "srpp"); }


	public SchemeInformationBox(): base(IsoStream.FromFourCC("schi"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.scheme_specific_data, "scheme_specific_data"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.scheme_specific_data, "scheme_specific_data"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(scheme_specific_data); // scheme_specific_data
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		// boxSize += tracer.Field("scheme_specific_data", IsoStream.CalculateBoxSize(scheme_specific_data), this.scheme_specific_data); 
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
