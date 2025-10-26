using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class IPMPDecoderConfiguration extends DecoderSpecificInfo : bit(8) tag=DecSpecificInfoTag {
 // IPMP system specific configuration information
 }
*/
public partial class IPMPDecoderConfiguration : DecoderSpecificInfo
{
	public const byte TYPE = DescriptorTags.DecSpecificInfoTag;
	public override string DisplayName { get { return "IPMPDecoderConfiguration"; } }

	public IPMPDecoderConfiguration(): base()
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		/*  IPMP system specific configuration information */
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		/*  IPMP system specific configuration information */
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		/*  IPMP system specific configuration information */
		return boxSize;
	}
}

}
