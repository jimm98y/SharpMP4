using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// ISO/IEC 14496-30 7.4 - left out of the spec text MPEG's repository has: a WebVTT cue, in a sample of a 'wvtt' track - its source ID, ID, time, settings and payload
aligned(8) class VTTCueBox() extends Box('vttc') {
	Box boxes[];
}
*/
public partial class VTTCueBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"// ISO/IEC 14496-30 7.4 - left out of the spec text MPEG's repository has: a WebVTT cue, in a sample of a 'wvtt' track - its source ID, ID, time, settings and payload
aligned(8) class VTTCueBox() extends Box('vttc') {
	Box boxes[];
}"; } }
	public const string TYPE = "vttc";
	public override string DisplayName { get { return "VTTCueBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("*"); }


	public VTTCueBox(): base(IsoStream.FromFourCC("vttc"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.boxes, "boxes"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.boxes, "boxes"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(boxes); // boxes
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		// boxSize += tracer.Field("boxes", IsoStream.CalculateBoxSize(boxes), this.boxes); 
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
