using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class QoS_Qualifier_MAX_GAP_LOSS extends QoS_Qualifier : bit(8) tag=0x04 {
 unsigned int(32) MAX_GAP_LOSS;
 }
 
*/
public partial class QoS_Qualifier_MAX_GAP_LOSS : QoS_Qualifier
{
	public const byte TYPE = 0x04;
	public override string DisplayName { get { return "QoS_Qualifier_MAX_GAP_LOSS"; } }

	protected uint MAX_GAP_LOSS; 
	public uint MAXGAPLOSS { get { return this.MAX_GAP_LOSS; } set { this.MAX_GAP_LOSS = value; } }

	public QoS_Qualifier_MAX_GAP_LOSS(): base(0x04)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.MAX_GAP_LOSS, "MAX_GAP_LOSS"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.MAX_GAP_LOSS, "MAX_GAP_LOSS"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // MAX_GAP_LOSS
		return boxSize;
	}
}

}
