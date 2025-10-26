using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
abstract aligned(8) expandable(228-1) class QoS_Qualifier : bit(8) tag=0x01..0xff {
 // empty. To be filled by classes extending this class.
 }
 
*/
public abstract partial class QoS_Qualifier : Descriptor
{
	public byte TagMin { get; set; } = 0x01;
	public byte TagMax { get; set; } = 0xff;	public override string DisplayName { get { return "QoS_Qualifier"; } }

	public QoS_Qualifier(byte tag): base(tag)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		/*  empty. To be filled by classes extending this class. */
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		/*  empty. To be filled by classes extending this class. */
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		/*  empty. To be filled by classes extending this class. */
		return boxSize;
	}
}

}
