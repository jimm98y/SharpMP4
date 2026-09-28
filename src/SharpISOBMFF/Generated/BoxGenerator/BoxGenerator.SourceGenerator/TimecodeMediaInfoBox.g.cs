using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class TimecodeMediaInfoBox() extends FullBox ('tcmi'){
 signed int(16) text_font;
 signed int(16) text_face;
 signed int(16) text_size;
 signed int(16) reserved;
 unsigned int(48) text_color;
 unsigned int(48) background_color;
 unsigned int(8) font_name_length;
 unsigned int(8) font_name[font_name_length];
 }
*/
public partial class TimecodeMediaInfoBox : FullBox
{
	public const string TYPE = "tcmi";
	public override string DisplayName { get { return "TimecodeMediaInfoBox"; } }

	protected short text_font; 
	public short TextFont { get { return this.text_font; } set { this.text_font = value; } }

	protected short text_face; 
	public short TextFace { get { return this.text_face; } set { this.text_face = value; } }

	protected short text_size; 
	public short TextSize { get { return this.text_size; } set { this.text_size = value; } }

	protected short reserved; 
	public short Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected ulong text_color; 
	public ulong TextColor { get { return this.text_color; } set { this.text_color = value; } }

	protected ulong background_color; 
	public ulong BackgroundColor { get { return this.background_color; } set { this.background_color = value; } }

	protected byte font_name_length; 
	public byte FontNameLength { get { return this.font_name_length; } set { this.font_name_length = value; } }

	protected byte[] font_name; 
	public byte[] FontName { get { return this.font_name; } set { this.font_name = value; } }

	public TimecodeMediaInfoBox(): base(IsoStream.FromFourCC("tcmi"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.text_font, "text_font"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.text_face, "text_face"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.text_size, "text_size"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt48(boxSize, readSize,  out this.text_color, "text_color"); 
		boxSize += stream.ReadUInt48(boxSize, readSize,  out this.background_color, "background_color"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.font_name_length, "font_name_length"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(font_name_length),  out this.font_name, "font_name"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt16( this.text_font, "text_font"); 
		boxSize += stream.WriteInt16( this.text_face, "text_face"); 
		boxSize += stream.WriteInt16( this.text_size, "text_size"); 
		boxSize += stream.WriteInt16( this.reserved, "reserved"); 
		boxSize += stream.WriteUInt48( this.text_color, "text_color"); 
		boxSize += stream.WriteUInt48( this.background_color, "background_color"); 
		boxSize += stream.WriteUInt8( this.font_name_length, "font_name_length"); 
		boxSize += stream.WriteUInt8Array((uint)(font_name_length),  this.font_name, "font_name"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // text_font
		boxSize += 16; // text_face
		boxSize += 16; // text_size
		boxSize += 16; // reserved
		boxSize += 48; // text_color
		boxSize += 48; // background_color
		boxSize += 8; // font_name_length
		boxSize += ((ulong)(font_name_length) * 8); // font_name
		return boxSize;
	}
}

}
