using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class QoS_Qualifier_LOSS_PROB extends QoS_Qualifier : bit(8) tag=0x03 {
 double(32) LOSS_PROB;
 }
 
*/
public partial class QoS_Qualifier_LOSS_PROB : QoS_Qualifier
{
	public const byte TYPE = 0x03;
	public override string DisplayName { get { return "QoS_Qualifier_LOSS_PROB"; } }

	protected double LOSS_PROB; 
	public double LOSSPROB { get { return this.LOSS_PROB; } set { this.LOSS_PROB = value; } }

	public QoS_Qualifier_LOSS_PROB(): base(0x03)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadDouble32(boxSize, readSize,  out this.LOSS_PROB, "LOSS_PROB"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteDouble32( this.LOSS_PROB, "LOSS_PROB"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // LOSS_PROB
		return boxSize;
	}
}

}
