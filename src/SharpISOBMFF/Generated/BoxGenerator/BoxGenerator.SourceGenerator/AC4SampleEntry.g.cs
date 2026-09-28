using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// E.4, Pseudocode E.1
aligned(8) class AC4SampleEntry extends AudioSampleEntry('ac-4') {
 AC4SpecificBox();
 // we permit any number of AC4PresentationLabel boxes:
 AC4PresentationLabelBox() [];
 Box () [];
}
*/
public partial class AC4SampleEntry : AudioSampleEntry
{
	public const string TYPE = "ac-4";
	public override string DisplayName { get { return "AC4SampleEntry"; } }
	public AC4SpecificBox _AC4SpecificBox { get { return this.children.OfType<AC4SpecificBox>().FirstOrDefault(); } }
	public IEnumerable<AC4PresentationLabelBox> _AC4PresentationLabelBox { get { return this.children.OfType<AC4PresentationLabelBox>(); } }
	public IEnumerable<Box> _Box { get { return this.children.OfType<Box>(); } }

	public AC4SampleEntry(): base(IsoStream.FromFourCC("ac-4"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.AC4SpecificBox, "AC4SpecificBox"); // we permit any number of AC4PresentationLabel boxes:
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.AC4PresentationLabelBox, "AC4PresentationLabelBox"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.Box, "Box"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.AC4SpecificBox, "AC4SpecificBox"); // we permit any number of AC4PresentationLabel boxes:
		// boxSize += stream.WriteBox( this.AC4PresentationLabelBox, "AC4PresentationLabelBox"); 
		// boxSize += stream.WriteBox( this.Box, "Box"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(AC4SpecificBox); // AC4SpecificBox
		// boxSize += IsoStream.CalculateBoxSize(AC4PresentationLabelBox); // AC4PresentationLabelBox
		// boxSize += IsoStream.CalculateBoxSize(Box); // Box
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
