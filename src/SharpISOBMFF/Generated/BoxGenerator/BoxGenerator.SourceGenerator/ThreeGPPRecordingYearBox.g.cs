using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ThreeGPPRecordingYearBox() extends FullBox('yrrc', version = 0, 0) {
	unsigned int(16) year;
}
*/
public partial class ThreeGPPRecordingYearBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class ThreeGPPRecordingYearBox() extends FullBox('yrrc', version = 0, 0) {
	unsigned int(16) year;
}"; } }
	public const string TYPE = "yrrc";
	public override string DisplayName { get { return "ThreeGPPRecordingYearBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("udta"); }


	protected ushort year; 
	public ushort Year { get { return this.year; } set { this.year = value; } }

	public ThreeGPPRecordingYearBox(): base(IsoStream.FromFourCC("yrrc"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.year, "year"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.year, "year"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // year
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("year", 16, this.year); 
		return boxSize;
	}
}

}
