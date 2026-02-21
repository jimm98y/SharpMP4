using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ThreeGPPRecordingYearBox() extends Box('yrrc') {
	bit(1) reserved;
 unsigned int(5)[3] language;
	string value; 
 unsigned int(16) year;
 } 
*/
public partial class ThreeGPPRecordingYearBox : Box
{
	public const string TYPE = "yrrc";
	public override string DisplayName { get { return "ThreeGPPRecordingYearBox"; } }

	protected bool reserved; 
	public bool Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected string language; 
	public string Language { get { return this.language; } set { this.language = value; } }

	protected BinaryUTF8String value; 
	public BinaryUTF8String Value { get { return this.value; } set { this.value = value; } }

	protected ushort year; 
	public ushort Year { get { return this.year; } set { this.year = value; } }

	public ThreeGPPRecordingYearBox(): base(IsoStream.FromFourCC("yrrc"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadIso639(boxSize, readSize,  out this.language, "language"); 
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.value, "value"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.year, "year"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBit( this.reserved, "reserved"); 
		boxSize += stream.WriteIso639( this.language, "language"); 
		boxSize += stream.WriteStringZeroTerminated( this.value, "value"); 
		boxSize += stream.WriteUInt16( this.year, "year"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 1; // reserved
		boxSize += 15; // language
		boxSize += IsoStream.CalculateStringSize(value); // value
		boxSize += 16; // year
		return boxSize;
	}
}

}
