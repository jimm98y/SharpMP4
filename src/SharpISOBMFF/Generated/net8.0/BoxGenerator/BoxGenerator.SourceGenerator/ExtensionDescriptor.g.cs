using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
abstract class ExtensionDescriptor extends BaseDescriptor : bit(8) tag=ExtDescrTagStartRange..ExtDescrTagEndRange {
 // empty. To be filled by classes extending this class.
 }
*/
public abstract partial class ExtensionDescriptor : BaseDescriptor
{
	public byte TagMin { get; set; } = DescriptorTags.ExtDescrTagStartRange;
	public byte TagMax { get; set; } = DescriptorTags.ExtDescrTagEndRange;	public override string DisplayName { get { return "ExtensionDescriptor"; } }

	public ExtensionDescriptor(byte tag): base(tag)
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
