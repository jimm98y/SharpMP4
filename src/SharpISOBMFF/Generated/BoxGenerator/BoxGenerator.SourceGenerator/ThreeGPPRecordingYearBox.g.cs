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
	public const string TYPE = "yrrc";
	public override string DisplayName { get { return "ThreeGPPRecordingYearBox"; } }

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
}

}
