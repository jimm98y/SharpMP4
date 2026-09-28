using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// ISO/IEC 14496-1:2010's InitialObjectDescriptor, as an MP4 file has it (ISO/IEC 14496-14): ES_ID_Inc for its ES_Descriptors
aligned(8) class IOD_Descriptor extends BaseDescriptor : bit(8) tag=MP4_IOD_Tag {
 bit(10) ObjectDescriptorID;
 bit(1) URL_Flag;
 bit(1) includeInlineProfileLevelFlag;
 bit(4) reserved;
 if (URL_Flag) {
  bit(8) URLlength;
  bit(8) URLstring[URLlength];
 }
 else {
  bit(8) ODProfileLevelIndication;
  bit(8) sceneProfileLevelIndication;
  bit(8) audioProfileLevelIndication;
  bit(8) visualProfileLevelIndication;
  bit(8) graphicsProfileLevelIndication;
 }
 ES_ID_Inc esIdInc[1 .. 255];
 }
 
*/
public partial class IOD_Descriptor : BaseDescriptor
{
	public const byte TYPE = DescriptorTags.MP4_IOD_Tag;
	public override string DisplayName { get { return "IOD_Descriptor"; } }

	protected ushort ObjectDescriptorID; 
	public ushort _ObjectDescriptorID { get { return this.ObjectDescriptorID; } set { this.ObjectDescriptorID = value; } }

	protected bool URL_Flag; 
	public bool URLFlag { get { return this.URL_Flag; } set { this.URL_Flag = value; } }

	protected bool includeInlineProfileLevelFlag; 
	public bool IncludeInlineProfileLevelFlag { get { return this.includeInlineProfileLevelFlag; } set { this.includeInlineProfileLevelFlag = value; } }

	protected byte reserved; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte URLlength; 
	public byte _URLlength { get { return this.URLlength; } set { this.URLlength = value; } }

	protected byte[] URLstring; 
	public byte[] _URLstring { get { return this.URLstring; } set { this.URLstring = value; } }

	protected byte ODProfileLevelIndication; 
	public byte _ODProfileLevelIndication { get { return this.ODProfileLevelIndication; } set { this.ODProfileLevelIndication = value; } }

	protected byte sceneProfileLevelIndication; 
	public byte SceneProfileLevelIndication { get { return this.sceneProfileLevelIndication; } set { this.sceneProfileLevelIndication = value; } }

	protected byte audioProfileLevelIndication; 
	public byte AudioProfileLevelIndication { get { return this.audioProfileLevelIndication; } set { this.audioProfileLevelIndication = value; } }

	protected byte visualProfileLevelIndication; 
	public byte VisualProfileLevelIndication { get { return this.visualProfileLevelIndication; } set { this.visualProfileLevelIndication = value; } }

	protected byte graphicsProfileLevelIndication; 
	public byte GraphicsProfileLevelIndication { get { return this.graphicsProfileLevelIndication; } set { this.graphicsProfileLevelIndication = value; } }
	public IEnumerable<ES_ID_Inc> EsIdInc { get { return this.children.OfType<ES_ID_Inc>(); } }

	public IOD_Descriptor(): base(DescriptorTags.MP4_IOD_Tag)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 10,  out this.ObjectDescriptorID, "ObjectDescriptorID"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.URL_Flag, "URL_Flag"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.includeInlineProfileLevelFlag, "includeInlineProfileLevelFlag"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.reserved, "reserved"); 

		if (URL_Flag)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.URLlength, "URLlength"); 
			boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(URLlength),  out this.URLstring, "URLstring"); 
		}

		else 
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.ODProfileLevelIndication, "ODProfileLevelIndication"); 
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.sceneProfileLevelIndication, "sceneProfileLevelIndication"); 
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.audioProfileLevelIndication, "audioProfileLevelIndication"); 
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.visualProfileLevelIndication, "visualProfileLevelIndication"); 
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.graphicsProfileLevelIndication, "graphicsProfileLevelIndication"); 
		}
		// boxSize += stream.ReadDescriptor(boxSize, readSize, this,  out this.esIdInc, "esIdInc"); 
		boxSize += stream.ReadDescriptorsTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(10,  this.ObjectDescriptorID, "ObjectDescriptorID"); 
		boxSize += stream.WriteBit( this.URL_Flag, "URL_Flag"); 
		boxSize += stream.WriteBit( this.includeInlineProfileLevelFlag, "includeInlineProfileLevelFlag"); 
		boxSize += stream.WriteBits(4,  this.reserved, "reserved"); 

		if (URL_Flag)
		{
			boxSize += stream.WriteUInt8( this.URLlength, "URLlength"); 
			boxSize += stream.WriteUInt8Array((uint)(URLlength),  this.URLstring, "URLstring"); 
		}

		else 
		{
			boxSize += stream.WriteUInt8( this.ODProfileLevelIndication, "ODProfileLevelIndication"); 
			boxSize += stream.WriteUInt8( this.sceneProfileLevelIndication, "sceneProfileLevelIndication"); 
			boxSize += stream.WriteUInt8( this.audioProfileLevelIndication, "audioProfileLevelIndication"); 
			boxSize += stream.WriteUInt8( this.visualProfileLevelIndication, "visualProfileLevelIndication"); 
			boxSize += stream.WriteUInt8( this.graphicsProfileLevelIndication, "graphicsProfileLevelIndication"); 
		}
		// boxSize += stream.WriteDescriptor( this.esIdInc, "esIdInc"); 
		boxSize += stream.WriteDescriptorsTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 10; // ObjectDescriptorID
		boxSize += 1; // URL_Flag
		boxSize += 1; // includeInlineProfileLevelFlag
		boxSize += 4; // reserved

		if (URL_Flag)
		{
			boxSize += 8; // URLlength
			boxSize += ((ulong)(URLlength) * 8); // URLstring
		}

		else 
		{
			boxSize += 8; // ODProfileLevelIndication
			boxSize += 8; // sceneProfileLevelIndication
			boxSize += 8; // audioProfileLevelIndication
			boxSize += 8; // visualProfileLevelIndication
			boxSize += 8; // graphicsProfileLevelIndication
		}
		// boxSize += IsoStream.CalculateDescriptorSize(esIdInc); // esIdInc
		boxSize += IsoStream.CalculateDescriptors(this);
		return boxSize;
	}
}

}
