using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class AudioRollRecoveryEntry() extends AudioSampleGroupEntry ('roll')
{
	signed int(16) roll_distance;
}
*/
public partial class AudioRollRecoveryEntry : AudioSampleGroupEntry
{
	public const string TYPE = "roll";
	public override string DisplayName { get { return "AudioRollRecoveryEntry"; } }

	protected short roll_distance; 
	public short RollDistance { get { return this.roll_distance; } set { this.roll_distance = value; } }

	public AudioRollRecoveryEntry(): base(IsoStream.FromFourCC("roll"))
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
