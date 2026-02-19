using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class QoS_Qualifier_MAX_AU_RATE extends QoS_Qualifier : bit(8) tag=0x43 {
 unsigned int(32) MAX_AU_RATE;
 }
*/
public partial class QoS_Qualifier_MAX_AU_RATE : QoS_Qualifier
{
	public const byte TYPE = 0x43;
	public override string DisplayName { get { return "QoS_Qualifier_MAX_AU_RATE"; } }

	protected uint MAX_AU_RATE; 
	public uint MAXAURATE { get { return this.MAX_AU_RATE; } set { this.MAX_AU_RATE = value; } }

	public QoS_Qualifier_MAX_AU_RATE(): base(0x43)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.MAX_AU_RATE, "MAX_AU_RATE"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.MAX_AU_RATE, "MAX_AU_RATE"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // MAX_AU_RATE
		return boxSize;
	}
}

}
