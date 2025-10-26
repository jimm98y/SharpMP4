using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
abstract aligned(8) expandable(228-1) class BaseDescriptor : bit(8) tag=0 {
 // empty. To be filled by classes extending this class.
 }
*/
public abstract partial class BaseDescriptor : Descriptor
{
	public override string DisplayName { get { return "BaseDescriptor"; } }

	public BaseDescriptor(byte tag): base(tag)
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
