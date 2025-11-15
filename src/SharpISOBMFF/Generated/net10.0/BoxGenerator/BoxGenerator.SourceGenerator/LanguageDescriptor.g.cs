using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class LanguageDescriptor extends OCI_Descriptor : bit(8) tag=LanguageDescrTag {
 bit(24) languageCode;
 }
*/
public partial class LanguageDescriptor : OCI_Descriptor
{
	public const byte TYPE = DescriptorTags.LanguageDescrTag;
	public override string DisplayName { get { return "LanguageDescriptor"; } }

	protected uint languageCode; 
	public uint LanguageCode { get { return this.languageCode; } set { this.languageCode = value; } }

	public LanguageDescriptor(): base(DescriptorTags.LanguageDescrTag)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt24(boxSize, readSize,  out this.languageCode, "languageCode"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt24( this.languageCode, "languageCode"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 24; // languageCode
		return boxSize;
	}
}

}
