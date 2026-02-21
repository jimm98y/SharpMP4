using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class TextMediaBox() extends Box('text') {
 unsigned int(8) reserved1[6];
 unsigned int(16) dataReferenceIndex;
 unsigned int(32) displayFlags;
 unsigned int(32) textJustification;
 unsigned int(16) bgColorRed;
 unsigned int(16) bgColorGreen;
 unsigned int(16) bgColorBlue;
 unsigned int(16) defTextBoxTop;
 unsigned int(16) defTextBoxLeft;
 unsigned int(16) defTextBoxBotton;
 unsigned int(16) defTextBoxRight;
 unsigned int(64) reserved2;
 unsigned int(16) fontNumber;
 unsigned int(16) fontFace;
 unsigned int(8) reserved3;
 unsigned int(16) reserved4;
 unsigned int(16) foreColorRed;
 unsigned int(16) foreColorGreen;
 unsigned int(16) foreColorBlue;
 string name;
 } 
*/
public partial class TextMediaBox : Box
{
	public const string TYPE = "text";
	public override string DisplayName { get { return "TextMediaBox"; } }

	protected byte[] reserved1; 
	public byte[] Reserved1 { get { return this.reserved1; } set { this.reserved1 = value; } }

	protected ushort dataReferenceIndex; 
	public ushort DataReferenceIndex { get { return this.dataReferenceIndex; } set { this.dataReferenceIndex = value; } }

	protected uint displayFlags; 
	public uint DisplayFlags { get { return this.displayFlags; } set { this.displayFlags = value; } }

	protected uint textJustification; 
	public uint TextJustification { get { return this.textJustification; } set { this.textJustification = value; } }

	protected ushort bgColorRed; 
	public ushort BgColorRed { get { return this.bgColorRed; } set { this.bgColorRed = value; } }

	protected ushort bgColorGreen; 
	public ushort BgColorGreen { get { return this.bgColorGreen; } set { this.bgColorGreen = value; } }

	protected ushort bgColorBlue; 
	public ushort BgColorBlue { get { return this.bgColorBlue; } set { this.bgColorBlue = value; } }

	protected ushort defTextBoxTop; 
	public ushort DefTextBoxTop { get { return this.defTextBoxTop; } set { this.defTextBoxTop = value; } }

	protected ushort defTextBoxLeft; 
	public ushort DefTextBoxLeft { get { return this.defTextBoxLeft; } set { this.defTextBoxLeft = value; } }

	protected ushort defTextBoxBotton; 
	public ushort DefTextBoxBotton { get { return this.defTextBoxBotton; } set { this.defTextBoxBotton = value; } }

	protected ushort defTextBoxRight; 
	public ushort DefTextBoxRight { get { return this.defTextBoxRight; } set { this.defTextBoxRight = value; } }

	protected ulong reserved2; 
	public ulong Reserved2 { get { return this.reserved2; } set { this.reserved2 = value; } }

	protected ushort fontNumber; 
	public ushort FontNumber { get { return this.fontNumber; } set { this.fontNumber = value; } }

	protected ushort fontFace; 
	public ushort FontFace { get { return this.fontFace; } set { this.fontFace = value; } }

	protected byte reserved3; 
	public byte Reserved3 { get { return this.reserved3; } set { this.reserved3 = value; } }

	protected ushort reserved4; 
	public ushort Reserved4 { get { return this.reserved4; } set { this.reserved4 = value; } }

	protected ushort foreColorRed; 
	public ushort ForeColorRed { get { return this.foreColorRed; } set { this.foreColorRed = value; } }

	protected ushort foreColorGreen; 
	public ushort ForeColorGreen { get { return this.foreColorGreen; } set { this.foreColorGreen = value; } }

	protected ushort foreColorBlue; 
	public ushort ForeColorBlue { get { return this.foreColorBlue; } set { this.foreColorBlue = value; } }

	protected BinaryUTF8String name; 
	public BinaryUTF8String Name { get { return this.name; } set { this.name = value; } }

	public TextMediaBox(): base(IsoStream.FromFourCC("text"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 6,  out this.reserved1, "reserved1"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.dataReferenceIndex, "dataReferenceIndex"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.displayFlags, "displayFlags"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.textJustification, "textJustification"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.bgColorRed, "bgColorRed"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.bgColorGreen, "bgColorGreen"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.bgColorBlue, "bgColorBlue"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.defTextBoxTop, "defTextBoxTop"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.defTextBoxLeft, "defTextBoxLeft"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.defTextBoxBotton, "defTextBoxBotton"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.defTextBoxRight, "defTextBoxRight"); 
		boxSize += stream.ReadUInt64(boxSize, readSize,  out this.reserved2, "reserved2"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.fontNumber, "fontNumber"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.fontFace, "fontFace"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.reserved3, "reserved3"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.reserved4, "reserved4"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.foreColorRed, "foreColorRed"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.foreColorGreen, "foreColorGreen"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.foreColorBlue, "foreColorBlue"); 
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.name, "name"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8Array(6,  this.reserved1, "reserved1"); 
		boxSize += stream.WriteUInt16( this.dataReferenceIndex, "dataReferenceIndex"); 
		boxSize += stream.WriteUInt32( this.displayFlags, "displayFlags"); 
		boxSize += stream.WriteUInt32( this.textJustification, "textJustification"); 
		boxSize += stream.WriteUInt16( this.bgColorRed, "bgColorRed"); 
		boxSize += stream.WriteUInt16( this.bgColorGreen, "bgColorGreen"); 
		boxSize += stream.WriteUInt16( this.bgColorBlue, "bgColorBlue"); 
		boxSize += stream.WriteUInt16( this.defTextBoxTop, "defTextBoxTop"); 
		boxSize += stream.WriteUInt16( this.defTextBoxLeft, "defTextBoxLeft"); 
		boxSize += stream.WriteUInt16( this.defTextBoxBotton, "defTextBoxBotton"); 
		boxSize += stream.WriteUInt16( this.defTextBoxRight, "defTextBoxRight"); 
		boxSize += stream.WriteUInt64( this.reserved2, "reserved2"); 
		boxSize += stream.WriteUInt16( this.fontNumber, "fontNumber"); 
		boxSize += stream.WriteUInt16( this.fontFace, "fontFace"); 
		boxSize += stream.WriteUInt8( this.reserved3, "reserved3"); 
		boxSize += stream.WriteUInt16( this.reserved4, "reserved4"); 
		boxSize += stream.WriteUInt16( this.foreColorRed, "foreColorRed"); 
		boxSize += stream.WriteUInt16( this.foreColorGreen, "foreColorGreen"); 
		boxSize += stream.WriteUInt16( this.foreColorBlue, "foreColorBlue"); 
		boxSize += stream.WriteStringZeroTerminated( this.name, "name"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 6 * 8; // reserved1
		boxSize += 16; // dataReferenceIndex
		boxSize += 32; // displayFlags
		boxSize += 32; // textJustification
		boxSize += 16; // bgColorRed
		boxSize += 16; // bgColorGreen
		boxSize += 16; // bgColorBlue
		boxSize += 16; // defTextBoxTop
		boxSize += 16; // defTextBoxLeft
		boxSize += 16; // defTextBoxBotton
		boxSize += 16; // defTextBoxRight
		boxSize += 64; // reserved2
		boxSize += 16; // fontNumber
		boxSize += 16; // fontFace
		boxSize += 8; // reserved3
		boxSize += 16; // reserved4
		boxSize += 16; // foreColorRed
		boxSize += 16; // foreColorGreen
		boxSize += 16; // foreColorBlue
		boxSize += IsoStream.CalculateStringSize(name); // name
		return boxSize;
	}
}

}
