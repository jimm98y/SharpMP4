using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class EndOfBitstreamSampleEntry() extends VisualSampleGroupEntry ('eob ')
{
	bit(16) eobNalUnit;
}
*/
public partial class EndOfBitstreamSampleEntry : VisualSampleGroupEntry
{
	public const string TYPE = "eob ";
	public override string DisplayName { get { return "EndOfBitstreamSampleEntry"; } }

	protected ushort eobNalUnit; 
	public ushort EobNalUnit { get { return this.eobNalUnit; } set { this.eobNalUnit = value; } }

	public EndOfBitstreamSampleEntry(): base(IsoStream.FromFourCC("eob "))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.eobNalUnit, "eobNalUnit"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.eobNalUnit, "eobNalUnit"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // eobNalUnit
		return boxSize;
	}
}

}
