using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class DecoderConfigDescriptor extends BaseDescriptor : bit(8) tag=DecoderConfigDescrTag {
 bit(8) objectTypeIndication;
 bit(6) streamType;
 bit(1) upStream;
 const bit(1) reserved=1;
 bit(24) bufferSizeDB;
 bit(32) maxBitrate;
 bit(32) avgBitrate;
 DecoderSpecificInfo decSpecificInfo[0 .. 1];
 ProfileLevelIndicationIndexDescriptor profileLevelIndicationIndexDescr [0..255];
 }
*/
public partial class DecoderConfigDescriptor : BaseDescriptor
{
	public const byte TYPE = DescriptorTags.DecoderConfigDescrTag;
	public override string DisplayName { get { return "DecoderConfigDescriptor"; } }

	protected byte objectTypeIndication; 
	public byte ObjectTypeIndication { get { return this.objectTypeIndication; } set { this.objectTypeIndication = value; } }

	protected byte streamType; 
	public byte StreamType { get { return this.streamType; } set { this.streamType = value; } }

	protected bool upStream; 
	public bool UpStream { get { return this.upStream; } set { this.upStream = value; } }

	protected bool reserved = true; 
	public bool Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected uint bufferSizeDB; 
	public uint BufferSizeDB { get { return this.bufferSizeDB; } set { this.bufferSizeDB = value; } }

	protected uint maxBitrate; 
	public uint MaxBitrate { get { return this.maxBitrate; } set { this.maxBitrate = value; } }

	protected uint avgBitrate; 
	public uint AvgBitrate { get { return this.avgBitrate; } set { this.avgBitrate = value; } }
	public IEnumerable<DecoderSpecificInfo> DecSpecificInfo { get { return this.children.OfType<DecoderSpecificInfo>(); } }
	public IEnumerable<ProfileLevelIndicationIndexDescriptor> ProfileLevelIndicationIndexDescr { get { return this.children.OfType<ProfileLevelIndicationIndexDescriptor>(); } }

	public DecoderConfigDescriptor(): base(DescriptorTags.DecoderConfigDescrTag)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.objectTypeIndication, "objectTypeIndication"); 
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.streamType, "streamType"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.upStream, "upStream"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt24(boxSize, readSize,  out this.bufferSizeDB, "bufferSizeDB"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.maxBitrate, "maxBitrate"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.avgBitrate, "avgBitrate"); 
		// boxSize += stream.ReadDescriptor(boxSize, readSize, this,  out this.decSpecificInfo, "decSpecificInfo"); 
		// boxSize += stream.ReadDescriptor(boxSize, readSize, this,  out this.profileLevelIndicationIndexDescr, "profileLevelIndicationIndexDescr"); 
		boxSize += stream.ReadDescriptorsTillEnd(boxSize, readSize, this, objectTypeIndication);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.objectTypeIndication, "objectTypeIndication"); 
		boxSize += stream.WriteBits(6,  this.streamType, "streamType"); 
		boxSize += stream.WriteBit( this.upStream, "upStream"); 
		boxSize += stream.WriteBit( this.reserved, "reserved"); 
		boxSize += stream.WriteUInt24( this.bufferSizeDB, "bufferSizeDB"); 
		boxSize += stream.WriteUInt32( this.maxBitrate, "maxBitrate"); 
		boxSize += stream.WriteUInt32( this.avgBitrate, "avgBitrate"); 
		// boxSize += stream.WriteDescriptor( this.decSpecificInfo, "decSpecificInfo"); 
		// boxSize += stream.WriteDescriptor( this.profileLevelIndicationIndexDescr, "profileLevelIndicationIndexDescr"); 
		boxSize += stream.WriteDescriptorsTillEnd(this, objectTypeIndication);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // objectTypeIndication
		boxSize += 6; // streamType
		boxSize += 1; // upStream
		boxSize += 1; // reserved
		boxSize += 24; // bufferSizeDB
		boxSize += 32; // maxBitrate
		boxSize += 32; // avgBitrate
		// boxSize += IsoStream.CalculateDescriptorSize(decSpecificInfo); // decSpecificInfo
		// boxSize += IsoStream.CalculateDescriptorSize(profileLevelIndicationIndexDescr); // profileLevelIndicationIndexDescr
		boxSize += IsoStream.CalculateDescriptors(this, objectTypeIndication);
		return boxSize;
	}
}

}
