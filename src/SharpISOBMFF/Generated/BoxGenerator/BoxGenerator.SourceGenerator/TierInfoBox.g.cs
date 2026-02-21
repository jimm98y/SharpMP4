using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class TierInfoBox extends Box('tiri'){ //Mandatory Box
	unsigned int(16) tierID;
	unsigned int(8) profileIndication;
	unsigned int(8) profile_compatibility;
	unsigned int(8) levelIndication;
	bit(8) reserved = 0;

	unsigned int(16) visualWidth;
	unsigned int(16) visualHeight;

	unsigned int(2) discardable;
	unsigned int(2) constantFrameRate;
	bit(4) reserved = 0;
	unsigned int(16) frameRate;
}
*/
public partial class TierInfoBox : Box
{
	public const string TYPE = "tiri";
	public override string DisplayName { get { return "TierInfoBox"; } }

	protected ushort tierID; 
	public ushort TierID { get { return this.tierID; } set { this.tierID = value; } }

	protected byte profileIndication; 
	public byte ProfileIndication { get { return this.profileIndication; } set { this.profileIndication = value; } }

	protected byte profile_compatibility; 
	public byte ProfileCompatibility { get { return this.profile_compatibility; } set { this.profile_compatibility = value; } }

	protected byte levelIndication; 
	public byte LevelIndication { get { return this.levelIndication; } set { this.levelIndication = value; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected ushort visualWidth; 
	public ushort VisualWidth { get { return this.visualWidth; } set { this.visualWidth = value; } }

	protected ushort visualHeight; 
	public ushort VisualHeight { get { return this.visualHeight; } set { this.visualHeight = value; } }

	protected byte discardable; 
	public byte Discardable { get { return this.discardable; } set { this.discardable = value; } }

	protected byte constantFrameRate; 
	public byte ConstantFrameRate { get { return this.constantFrameRate; } set { this.constantFrameRate = value; } }

	protected byte reserved0 = 0; 
	public byte Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected ushort frameRate; 
	public ushort FrameRate { get { return this.frameRate; } set { this.frameRate = value; } }

	public TierInfoBox(): base(IsoStream.FromFourCC("tiri"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		/* Mandatory Box */
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.tierID, "tierID"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.profileIndication, "profileIndication"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.profile_compatibility, "profile_compatibility"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.levelIndication, "levelIndication"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.visualWidth, "visualWidth"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.visualHeight, "visualHeight"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.discardable, "discardable"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.constantFrameRate, "constantFrameRate"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.reserved0, "reserved0"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.frameRate, "frameRate"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		/* Mandatory Box */
		boxSize += stream.WriteUInt16( this.tierID, "tierID"); 
		boxSize += stream.WriteUInt8( this.profileIndication, "profileIndication"); 
		boxSize += stream.WriteUInt8( this.profile_compatibility, "profile_compatibility"); 
		boxSize += stream.WriteUInt8( this.levelIndication, "levelIndication"); 
		boxSize += stream.WriteUInt8( this.reserved, "reserved"); 
		boxSize += stream.WriteUInt16( this.visualWidth, "visualWidth"); 
		boxSize += stream.WriteUInt16( this.visualHeight, "visualHeight"); 
		boxSize += stream.WriteBits(2,  this.discardable, "discardable"); 
		boxSize += stream.WriteBits(2,  this.constantFrameRate, "constantFrameRate"); 
		boxSize += stream.WriteBits(4,  this.reserved0, "reserved0"); 
		boxSize += stream.WriteUInt16( this.frameRate, "frameRate"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		/* Mandatory Box */
		boxSize += 16; // tierID
		boxSize += 8; // profileIndication
		boxSize += 8; // profile_compatibility
		boxSize += 8; // levelIndication
		boxSize += 8; // reserved
		boxSize += 16; // visualWidth
		boxSize += 16; // visualHeight
		boxSize += 2; // discardable
		boxSize += 2; // constantFrameRate
		boxSize += 4; // reserved0
		boxSize += 16; // frameRate
		return boxSize;
	}
}

}
