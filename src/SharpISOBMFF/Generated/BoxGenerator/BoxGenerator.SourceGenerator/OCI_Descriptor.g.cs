using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
abstract class OCI_Descriptor extends BaseDescriptor : bit(8) tag=OCIDescrTagStartRange..OCIDescrTagEndRange
{
 // empty. To be filled by classes extending this class.
}
*/
public abstract partial class OCI_Descriptor : BaseDescriptor
{
	public byte TagMin { get; set; } = DescriptorTags.OCIDescrTagStartRange;
	public byte TagMax { get; set; } = DescriptorTags.OCIDescrTagEndRange;	public override string DisplayName { get { return "OCI_Descriptor"; } }

	public OCI_Descriptor(byte tag): base(tag)
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
