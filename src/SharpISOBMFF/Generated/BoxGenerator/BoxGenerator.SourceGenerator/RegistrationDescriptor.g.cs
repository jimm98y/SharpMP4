using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class RegistrationDescriptor extends BaseDescriptor : bit(8) tag=RegistrationDescrTag {
 bit(32) formatIdentifier;
 bit(8) additionalIdentificationInfo[sizeOfInstance-4];
 }
*/
public partial class RegistrationDescriptor : BaseDescriptor
{
	public const byte TYPE = DescriptorTags.RegistrationDescrTag;
	public override string DisplayName { get { return "RegistrationDescriptor"; } }

	protected uint formatIdentifier; 
	public uint FormatIdentifier { get { return this.formatIdentifier; } set { this.formatIdentifier = value; } }

	protected byte[] additionalIdentificationInfo; 
	public byte[] AdditionalIdentificationInfo { get { return this.additionalIdentificationInfo; } set { this.additionalIdentificationInfo = value; } }

	public RegistrationDescriptor(): base(DescriptorTags.RegistrationDescrTag)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.formatIdentifier, "formatIdentifier"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(sizeOfInstance-4),  out this.additionalIdentificationInfo, "additionalIdentificationInfo"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.formatIdentifier, "formatIdentifier"); 
		boxSize += stream.WriteUInt8Array((uint)(sizeOfInstance-4),  this.additionalIdentificationInfo, "additionalIdentificationInfo"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // formatIdentifier
		boxSize += ((ulong)(sizeOfInstance-4) * 8); // additionalIdentificationInfo
		return boxSize;
	}
}

}
