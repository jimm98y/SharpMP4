using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class OCIDecoderConfiguration extends DecoderSpecificInfo : bit(8) tag=DecSpecificInfoTag {
 bit(8) versionLabel = 0x01;
 }
*/
public partial class OCIDecoderConfiguration : DecoderSpecificInfo
{
	public const byte TYPE = DescriptorTags.DecSpecificInfoTag;
	public override string DisplayName { get { return "OCIDecoderConfiguration"; } }

	protected byte versionLabel = 0x01; 
	public byte VersionLabel { get { return this.versionLabel; } set { this.versionLabel = value; } }

	public OCIDecoderConfiguration(): base()
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.versionLabel, "versionLabel"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.versionLabel, "versionLabel"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // versionLabel
		return boxSize;
	}
}

}
