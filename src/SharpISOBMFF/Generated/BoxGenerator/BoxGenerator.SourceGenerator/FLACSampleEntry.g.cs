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
	public const string TYPE = "fLaC";
	public override string DisplayName { get { return "FLACSampleEntry"; } }
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
}

}
