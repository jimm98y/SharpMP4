using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class ProfileLevelIndicationIndexDescriptor () extends BaseDescriptor
 : bit(8) ProfileLevelIndicationIndexDescrTag {
 bit(8) profileLevelIndicationIndex;
 }
*/
public partial class ProfileLevelIndicationIndexDescriptor : BaseDescriptor
{
	public const byte TYPE = DescriptorTags.ProfileLevelIndicationIndexDescrTag;
	public override string DisplayName { get { return "ProfileLevelIndicationIndexDescriptor"; } }

	protected byte profileLevelIndicationIndex; 
	public byte ProfileLevelIndicationIndex { get { return this.profileLevelIndicationIndex; } set { this.profileLevelIndicationIndex = value; } }

	public ProfileLevelIndicationIndexDescriptor(): base(DescriptorTags.ProfileLevelIndicationIndexDescrTag)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.profileLevelIndicationIndex, "profileLevelIndicationIndex"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.profileLevelIndicationIndex, "profileLevelIndicationIndex"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // profileLevelIndicationIndex
		return boxSize;
	}
}

}
