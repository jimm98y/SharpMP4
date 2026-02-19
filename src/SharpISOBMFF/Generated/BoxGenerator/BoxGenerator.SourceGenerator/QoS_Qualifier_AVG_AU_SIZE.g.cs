using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class QoS_Qualifier_AVG_AU_SIZE extends QoS_Qualifier : bit(8) tag=0x42 {
 unsigned int(32) AVG_AU_SIZE;
 }
 
*/
public partial class QoS_Qualifier_AVG_AU_SIZE : QoS_Qualifier
{
	public const byte TYPE = 0x42;
	public override string DisplayName { get { return "QoS_Qualifier_AVG_AU_SIZE"; } }

	protected uint AVG_AU_SIZE; 
	public uint AVGAUSIZE { get { return this.AVG_AU_SIZE; } set { this.AVG_AU_SIZE = value; } }

	public QoS_Qualifier_AVG_AU_SIZE(): base(0x42)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.AVG_AU_SIZE, "AVG_AU_SIZE"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.AVG_AU_SIZE, "AVG_AU_SIZE"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // AVG_AU_SIZE
		return boxSize;
	}
}

}
