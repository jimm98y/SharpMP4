using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class QoS_Qualifier_PREF_MAX_DELAY extends QoS_Qualifier : bit(8) tag=0x02 {
 unsigned int(32) PREF_MAX_DELAY;
 }
 
*/
public partial class QoS_Qualifier_PREF_MAX_DELAY : QoS_Qualifier
{
	public const byte TYPE = 0x02;
	public override string DisplayName { get { return "QoS_Qualifier_PREF_MAX_DELAY"; } }

	protected uint PREF_MAX_DELAY; 
	public uint PREFMAXDELAY { get { return this.PREF_MAX_DELAY; } set { this.PREF_MAX_DELAY = value; } }

	public QoS_Qualifier_PREF_MAX_DELAY(): base(0x02)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.PREF_MAX_DELAY, "PREF_MAX_DELAY"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.PREF_MAX_DELAY, "PREF_MAX_DELAY"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // PREF_MAX_DELAY
		return boxSize;
	}
}

}
