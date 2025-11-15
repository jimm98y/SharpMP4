using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class VisualEquivalenceEntry() extends VisualSampleGroupEntry ('eqiv')
{
	signed int(16)   time_offset;
	unsigned int(16) timescale_multiplier;
}
*/
public partial class VisualEquivalenceEntry : VisualSampleGroupEntry
{
	public const string TYPE = "eqiv";
	public override string DisplayName { get { return "VisualEquivalenceEntry"; } }

	protected short time_offset; 
	public short TimeOffset { get { return this.time_offset; } set { this.time_offset = value; } }

	protected ushort timescale_multiplier; 
	public ushort TimescaleMultiplier { get { return this.timescale_multiplier; } set { this.timescale_multiplier = value; } }

	public VisualEquivalenceEntry(): base(IsoStream.FromFourCC("eqiv"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.time_offset, "time_offset"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.timescale_multiplier, "timescale_multiplier"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt16( this.time_offset, "time_offset"); 
		boxSize += stream.WriteUInt16( this.timescale_multiplier, "timescale_multiplier"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // time_offset
		boxSize += 16; // timescale_multiplier
		return boxSize;
	}
}

}
