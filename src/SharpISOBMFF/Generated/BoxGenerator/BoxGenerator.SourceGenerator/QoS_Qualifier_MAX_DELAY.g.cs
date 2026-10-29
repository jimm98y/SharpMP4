using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class QoS_Qualifier_MAX_DELAY extends QoS_Qualifier : bit(8) tag=0x01 {
	unsigned int(32) MAX_DELAY;
}
*/
public partial class QoS_Qualifier_MAX_DELAY : QoS_Qualifier
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class QoS_Qualifier_MAX_DELAY extends QoS_Qualifier : bit(8) tag=0x01 {
	unsigned int(32) MAX_DELAY;
}"; } }
	public const byte TYPE = 0x01;
	public override string DisplayName { get { return "QoS_Qualifier_MAX_DELAY"; } }

	protected uint MAX_DELAY; 
	public uint MAXDELAY { get { return this.MAX_DELAY; } set { this.MAX_DELAY = value; } }

	public QoS_Qualifier_MAX_DELAY(): base(0x01)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.MAX_DELAY, "MAX_DELAY"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.MAX_DELAY, "MAX_DELAY"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // MAX_DELAY
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("MAX_DELAY", 32, this.MAX_DELAY); 
		return boxSize;
	}
}

}
