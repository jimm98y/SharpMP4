using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// Encapsulation of FLAC in ISO Base Media File Format (Xiph, isoflac.txt), 3.3.1 FLAC Sample Entry
class FLACSampleEntry() extends AudioSampleEntry ('fLaC'){
	FLACSpecificBox();
}
*/
public partial class FLACSampleEntry : AudioSampleEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"// Encapsulation of FLAC in ISO Base Media File Format (Xiph, isoflac.txt), 3.3.1 FLAC Sample Entry
class FLACSampleEntry() extends AudioSampleEntry ('fLaC'){
	FLACSpecificBox();
}"; } }
	public const string TYPE = "fLaC";
	public override string DisplayName { get { return "FLACSampleEntry"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stsd"); }

	public FLACSpecificBox _FLACSpecificBox { get { return this.children.OfType<FLACSpecificBox>().FirstOrDefault(); } }

	public FLACSampleEntry(): base(IsoStream.FromFourCC("fLaC"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.FLACSpecificBox, "FLACSpecificBox"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.FLACSpecificBox, "FLACSpecificBox"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(FLACSpecificBox); // FLACSpecificBox
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		// boxSize += tracer.Field("FLACSpecificBox", IsoStream.CalculateBoxSize(FLACSpecificBox), this.FLACSpecificBox); 
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
