using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class AudioPreRollEntry() extends AudioSampleGroupEntry ('prol')
{
	signed int(16) roll_distance;
}
*/
public partial class AudioPreRollEntry : AudioSampleGroupEntry
{
	public const string TYPE = "prol";
	public override string DisplayName { get { return "AudioPreRollEntry"; } }

	protected short roll_distance; 
	public short RollDistance { get { return this.roll_distance; } set { this.roll_distance = value; } }

	public AudioPreRollEntry(): base(IsoStream.FromFourCC("prol"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.roll_distance, "roll_distance"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt16( this.roll_distance, "roll_distance"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // roll_distance
		return boxSize;
	}
}

}
