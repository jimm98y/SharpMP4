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
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"// E.4, Pseudocode E.1
aligned(8) class AC4SampleEntry extends AudioSampleEntry('ac-4') {
	AC4SpecificBox();
	// we permit any number of AC4PresentationLabel boxes:
	AC4PresentationLabelBox() [];
	Box () [];
}"; } }
	public const string TYPE = "ac-4";
	public override string DisplayName { get { return "AC4SampleEntry"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stsd"); }

	public AC4SpecificBox _AC4SpecificBox { get { return this.children.OfType<AC4SpecificBox>().FirstOrDefault(); } }
	public IEnumerable<AC4PresentationLabelBox> _AC4PresentationLabelBox { get { return this.children.OfType<AC4PresentationLabelBox>(); } }

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

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		// boxSize += tracer.Field("AC4SpecificBox", IsoStream.CalculateBoxSize(AC4SpecificBox), this.AC4SpecificBox); // we permit any number of AC4PresentationLabel boxes:
		// boxSize += tracer.Field("AC4PresentationLabelBox", IsoStream.CalculateBoxSize(AC4PresentationLabelBox), this.AC4PresentationLabelBox); 
		// boxSize += tracer.Field("Box", IsoStream.CalculateBoxSize(Box), this.Box); 
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
