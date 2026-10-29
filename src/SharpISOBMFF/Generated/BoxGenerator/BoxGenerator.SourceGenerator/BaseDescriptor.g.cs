using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
abstract aligned(8) expandable(228-1) class BaseDescriptor : bit(8) tag=0 {
	// empty. To be filled by classes extending this class.
}
*/
public abstract partial class BaseDescriptor : Descriptor
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"abstract aligned(8) expandable(228-1) class BaseDescriptor : bit(8) tag=0 {
	// empty. To be filled by classes extending this class.
}"; } }
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

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		/*  empty. To be filled by classes extending this class. */
		return boxSize;
	}
}

}
