using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
abstract class IP_IdentificationDataSet extends BaseDescriptor
: bit(8) tag=ContentIdentDescrTag..SupplContentIdentDescrTag
{
	// empty. To be filled by classes extending this class.
}
*/
public abstract partial class IP_IdentificationDataSet : BaseDescriptor
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"abstract class IP_IdentificationDataSet extends BaseDescriptor
: bit(8) tag=ContentIdentDescrTag..SupplContentIdentDescrTag
{
	// empty. To be filled by classes extending this class.
}"; } }
	public byte TagMin { get; set; } = DescriptorTags.ContentIdentDescrTag;
	public byte TagMax { get; set; } = DescriptorTags.SupplContentIdentDescrTag;	public override string DisplayName { get { return "IP_IdentificationDataSet"; } }

	public IP_IdentificationDataSet(byte tag): base(tag)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		/*  empty. To be filled by classes extending this class. */
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		/*  empty. To be filled by classes extending this class. */
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		/*  empty. To be filled by classes extending this class. */
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		/*  empty. To be filled by classes extending this class. */
		return boxSize;
	}
}

}
