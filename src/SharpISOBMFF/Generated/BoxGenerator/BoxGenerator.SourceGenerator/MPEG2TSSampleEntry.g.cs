using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class MPEG2TSSampleEntry(name) extends HintSampleEntry(name) { 
 uint(16) hinttrackversion = 1; 
 uint(16) highestcompatibleversion = 1; 
 uint(8) precedingbyteslen; 
 uint(8) trailingbyteslen; 
 uint(1) precomputed_only_flag; 
 uint(7) reserved; 
 Box  additionaldata[]; 
} 
*/
public partial class MPEG2TSSampleEntry : HintSampleEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class MPEG2TSSampleEntry(name) extends HintSampleEntry(name) { 
 uint(16) hinttrackversion = 1; 
 uint(16) highestcompatibleversion = 1; 
 uint(8) precedingbyteslen; 
 uint(8) trailingbyteslen; 
 uint(1) precomputed_only_flag; 
 uint(7) reserved; 
 Box  additionaldata[]; 
} "; } }
	public override string DisplayName { get { return "MPEG2TSSampleEntry"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn(BoxPlacements.Of(this) ?? new[] { "*" }); }


	protected ushort hinttrackversion = 1; 
	public ushort Hinttrackversion { get { return this.hinttrackversion; } set { this.hinttrackversion = value; } }

	protected ushort highestcompatibleversion = 1; 
	public ushort Highestcompatibleversion { get { return this.highestcompatibleversion; } set { this.highestcompatibleversion = value; } }

	protected byte precedingbyteslen; 
	public byte Precedingbyteslen { get { return this.precedingbyteslen; } set { this.precedingbyteslen = value; } }

	protected byte trailingbyteslen; 
	public byte Trailingbyteslen { get { return this.trailingbyteslen; } set { this.trailingbyteslen = value; } }

	protected bool precomputed_only_flag; 
	public bool PrecomputedOnlyFlag { get { return this.precomputed_only_flag; } set { this.precomputed_only_flag = value; } }

	protected byte reserved; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	public MPEG2TSSampleEntry(uint name): base(name)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.hinttrackversion, "hinttrackversion"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.highestcompatibleversion, "highestcompatibleversion"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.precedingbyteslen, "precedingbyteslen"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.trailingbyteslen, "trailingbyteslen"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.precomputed_only_flag, "precomputed_only_flag"); 
		boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.reserved, "reserved"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.additionaldata, "additionaldata"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.hinttrackversion, "hinttrackversion"); 
		boxSize += stream.WriteUInt16( this.highestcompatibleversion, "highestcompatibleversion"); 
		boxSize += stream.WriteUInt8( this.precedingbyteslen, "precedingbyteslen"); 
		boxSize += stream.WriteUInt8( this.trailingbyteslen, "trailingbyteslen"); 
		boxSize += stream.WriteBit( this.precomputed_only_flag, "precomputed_only_flag"); 
		boxSize += stream.WriteBits(7,  this.reserved, "reserved"); 
		// boxSize += stream.WriteBox( this.additionaldata, "additionaldata"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // hinttrackversion
		boxSize += 16; // highestcompatibleversion
		boxSize += 8; // precedingbyteslen
		boxSize += 8; // trailingbyteslen
		boxSize += 1; // precomputed_only_flag
		boxSize += 7; // reserved
		// boxSize += IsoStream.CalculateBoxSize(additionaldata); // additionaldata
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("hinttrackversion", 16, this.hinttrackversion); 
		boxSize += tracer.Field("highestcompatibleversion", 16, this.highestcompatibleversion); 
		boxSize += tracer.Field("precedingbyteslen", 8, this.precedingbyteslen); 
		boxSize += tracer.Field("trailingbyteslen", 8, this.trailingbyteslen); 
		boxSize += tracer.Field("precomputed_only_flag", 1, this.precomputed_only_flag); 
		boxSize += tracer.Field("reserved", 7, this.reserved); 
		// boxSize += tracer.Field("additionaldata", IsoStream.CalculateBoxSize(additionaldata), this.additionaldata); 
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
