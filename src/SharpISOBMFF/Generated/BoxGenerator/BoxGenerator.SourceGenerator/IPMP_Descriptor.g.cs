using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class IPMP_Descriptor() extends BaseDescriptor : bit(8) tag=IPMP_DescrTag {
 bit(8) IPMP_DescriptorID;
 unsigned int(16)
 IPMPS_Type;
 if (IPMPS_Type == 0) {
 bit(8) URLString[sizeOfInstance-3];
 } else {
 bit(8) IPMP_data[sizeOfInstance-3];
 }
 }
*/
public partial class IPMP_Descriptor : BaseDescriptor
{
	public const byte TYPE = DescriptorTags.IPMP_DescrTag;
	public override string DisplayName { get { return "IPMP_Descriptor"; } }

	protected byte IPMP_DescriptorID; 
	public byte IPMPDescriptorID { get { return this.IPMP_DescriptorID; } set { this.IPMP_DescriptorID = value; } }

	protected ushort IPMPS_Type; 
	public ushort IPMPSType { get { return this.IPMPS_Type; } set { this.IPMPS_Type = value; } }

	protected byte[] URLString; 
	public byte[] _URLString { get { return this.URLString; } set { this.URLString = value; } }

	protected byte[] IPMP_data; 
	public byte[] IPMPData { get { return this.IPMP_data; } set { this.IPMP_data = value; } }

	public IPMP_Descriptor(): base(DescriptorTags.IPMP_DescrTag)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.IPMP_DescriptorID, "IPMP_DescriptorID"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.IPMPS_Type, "IPMPS_Type"); 

		if (IPMPS_Type == 0)
		{
			boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(sizeOfInstance-3),  out this.URLString, "URLString"); 
		}

		else 
		{
			boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(sizeOfInstance-3),  out this.IPMP_data, "IPMP_data"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.IPMP_DescriptorID, "IPMP_DescriptorID"); 
		boxSize += stream.WriteUInt16( this.IPMPS_Type, "IPMPS_Type"); 

		if (IPMPS_Type == 0)
		{
			boxSize += stream.WriteUInt8Array((uint)(sizeOfInstance-3),  this.URLString, "URLString"); 
		}

		else 
		{
			boxSize += stream.WriteUInt8Array((uint)(sizeOfInstance-3),  this.IPMP_data, "IPMP_data"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // IPMP_DescriptorID
		boxSize += 16; // IPMPS_Type

		if (IPMPS_Type == 0)
		{
			boxSize += ((ulong)(sizeOfInstance-3) * 8); // URLString
		}

		else 
		{
			boxSize += ((ulong)(sizeOfInstance-3) * 8); // IPMP_data
		}
		return boxSize;
	}
}

}
