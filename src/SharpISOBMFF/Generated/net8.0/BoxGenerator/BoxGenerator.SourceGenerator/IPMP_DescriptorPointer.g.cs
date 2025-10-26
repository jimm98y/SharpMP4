using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class IPMP_DescriptorPointer extends BaseDescriptor : bit(8) tag=IPMP_DescrPointerTag {
 bit(8) IPMP_DescriptorID;
 }
*/
public partial class IPMP_DescriptorPointer : BaseDescriptor
{
	public const byte TYPE = DescriptorTags.IPMP_DescrPointerTag;
	public override string DisplayName { get { return "IPMP_DescriptorPointer"; } }

	protected byte IPMP_DescriptorID; 
	public byte IPMPDescriptorID { get { return this.IPMP_DescriptorID; } set { this.IPMP_DescriptorID = value; } }

	public IPMP_DescriptorPointer(): base(DescriptorTags.IPMP_DescrPointerTag)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.IPMP_DescriptorID, "IPMP_DescriptorID"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.IPMP_DescriptorID, "IPMP_DescriptorID"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // IPMP_DescriptorID
		return boxSize;
	}
}

}
