using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class TemporalLevelEntry() extends VisualSampleGroupEntry('tele')
{
	bit(1)	level_independently_decodable;
	bit(7)	reserved=0;
}
*/
public partial class TemporalLevelEntry : VisualSampleGroupEntry
{
	public const string TYPE = "tele";
	public override string DisplayName { get { return "TemporalLevelEntry"; } }

	protected bool level_independently_decodable; 
	public bool LevelIndependentlyDecodable { get { return this.level_independently_decodable; } set { this.level_independently_decodable = value; } }

	protected byte reserved =0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	public TemporalLevelEntry(): base(IsoStream.FromFourCC("tele"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.level_independently_decodable, "level_independently_decodable"); 
		boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBit( this.level_independently_decodable, "level_independently_decodable"); 
		boxSize += stream.WriteBits(7,  this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 1; // level_independently_decodable
		boxSize += 7; // reserved
		return boxSize;
	}
}

}
