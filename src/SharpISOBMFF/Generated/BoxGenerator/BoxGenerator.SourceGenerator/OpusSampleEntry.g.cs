using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class OpusSampleEntry() extends AudioSampleEntry ('Opus'){
 OpusSpecificBox();
 }
*/
public partial class OpusSampleEntry : AudioSampleEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class OpusSampleEntry() extends AudioSampleEntry ('Opus'){
 OpusSpecificBox();
 }"; } }
	public const string TYPE = "Opus";
	public override string DisplayName { get { return "OpusSampleEntry"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stsd"); }

	public OpusSpecificBox _OpusSpecificBox { get { return this.children.OfType<OpusSpecificBox>().FirstOrDefault(); } }

	public OpusSampleEntry(): base(IsoStream.FromFourCC("Opus"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.OpusSpecificBox, "OpusSpecificBox"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.OpusSpecificBox, "OpusSpecificBox"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(OpusSpecificBox); // OpusSpecificBox
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		// boxSize += tracer.Field("OpusSpecificBox", IsoStream.CalculateBoxSize(OpusSpecificBox), this.OpusSpecificBox); 
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
