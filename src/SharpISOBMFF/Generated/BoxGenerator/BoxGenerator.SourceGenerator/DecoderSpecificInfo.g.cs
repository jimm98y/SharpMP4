using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
abstract class DecoderSpecificInfo extends BaseDescriptor : bit(8) tag=DecSpecificInfoTag
 {
 // empty. To be filled by classes extending this class.
 }
*/
public abstract partial class DecoderSpecificInfo : BaseDescriptor
{
	public override string DisplayName { get { return "DecoderSpecificInfo"; } }

	public DecoderSpecificInfo(): base(DescriptorTags.DecSpecificInfoTag)
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
}

}
