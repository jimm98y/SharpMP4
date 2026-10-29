using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ICCTextDescriptionType {
	unsigned int(32) type_signature;
	unsigned int(32) reserved = 0;
	unsigned int(32) ascii_count;
	bytestring ascii[ascii_count];	// 7-bit ASCII, of a terminating null
	unsigned int(32) unicode_language;
	unsigned int(32) unicode_count;
	utf16string unicode[unicode_count];	// UTF-16BE, of a terminating null
	unsigned int(16) scriptcode_code;
	unsigned int(8) scriptcode_count;
	unsigned int(8) scriptcode[67];
}
*/
public partial class ICCTextDescriptionType : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"aligned(8) class ICCTextDescriptionType {
	unsigned int(32) type_signature;
	unsigned int(32) reserved = 0;
	unsigned int(32) ascii_count;
	bytestring ascii[ascii_count];	// 7-bit ASCII, of a terminating null
	unsigned int(32) unicode_language;
	unsigned int(32) unicode_count;
	utf16string unicode[unicode_count];	// UTF-16BE, of a terminating null
	unsigned int(16) scriptcode_code;
	unsigned int(8) scriptcode_count;
	unsigned int(8) scriptcode[67];
}"; } }
	public virtual string DisplayName { get { return "ICCTextDescriptionType"; } }

	protected uint type_signature; 
	public uint TypeSignature { get { return this.type_signature; } set { this.type_signature = value; } }

	protected uint reserved = 0; 
	public uint Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected uint ascii_count; 
	public uint AsciiCount { get { return this.ascii_count; } set { this.ascii_count = value; } }

	protected BinaryUTF8String ascii;  //  7-bit ASCII, of a terminating null
	public BinaryUTF8String Ascii { get { return this.ascii; } set { this.ascii = value; } }

	protected uint unicode_language; 
	public uint UnicodeLanguage { get { return this.unicode_language; } set { this.unicode_language = value; } }

	protected uint unicode_count; 
	public uint UnicodeCount { get { return this.unicode_count; } set { this.unicode_count = value; } }

	protected BinaryUTF16String unicode;  //  UTF-16BE, of a terminating null
	public BinaryUTF16String Unicode { get { return this.unicode; } set { this.unicode = value; } }

	protected ushort scriptcode_code; 
	public ushort ScriptcodeCode { get { return this.scriptcode_code; } set { this.scriptcode_code = value; } }

	protected byte scriptcode_count; 
	public byte ScriptcodeCount { get { return this.scriptcode_count; } set { this.scriptcode_count = value; } }

	protected byte[] scriptcode = new byte[67]; 
	public byte[] Scriptcode { get { return this.scriptcode; } set { this.scriptcode = value; } }

	public ICCTextDescriptionType(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.type_signature, "type_signature"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.ascii_count, "ascii_count"); 
		boxSize += stream.ReadString(boxSize, readSize, (uint)(ascii_count),  out this.ascii, "ascii"); // 7-bit ASCII, of a terminating null
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.unicode_language, "unicode_language"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.unicode_count, "unicode_count"); 
		boxSize += stream.ReadUtf16String(boxSize, readSize, (uint)(unicode_count),  out this.unicode, "unicode"); // UTF-16BE, of a terminating null
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.scriptcode_code, "scriptcode_code"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.scriptcode_count, "scriptcode_count"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 67,  out this.scriptcode, "scriptcode"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt32( this.type_signature, "type_signature"); 
		boxSize += stream.WriteUInt32( this.reserved, "reserved"); 
		boxSize += stream.WriteUInt32( this.ascii_count, "ascii_count"); 
		boxSize += stream.WriteStringZeroTerminated( this.ascii, "ascii"); // 7-bit ASCII, of a terminating null
		boxSize += stream.WriteUInt32( this.unicode_language, "unicode_language"); 
		boxSize += stream.WriteUInt32( this.unicode_count, "unicode_count"); 
		boxSize += stream.WriteUtf16String( this.unicode, "unicode"); // UTF-16BE, of a terminating null
		boxSize += stream.WriteUInt16( this.scriptcode_code, "scriptcode_code"); 
		boxSize += stream.WriteUInt8( this.scriptcode_count, "scriptcode_count"); 
		boxSize += stream.WriteUInt8Array(67,  this.scriptcode, "scriptcode"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 32; // type_signature
		boxSize += 32; // reserved
		boxSize += 32; // ascii_count
		boxSize += IsoStream.CalculateStringSize(ascii); // ascii
		boxSize += 32; // unicode_language
		boxSize += 32; // unicode_count
		boxSize += IsoStream.CalculateStringSize(unicode); // unicode
		boxSize += 16; // scriptcode_code
		boxSize += 8; // scriptcode_count
		boxSize += 67 * 8; // scriptcode
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("type_signature", 32, this.type_signature); 
		boxSize += tracer.Field("reserved", 32, this.reserved); 
		boxSize += tracer.Field("ascii_count", 32, this.ascii_count); 
		boxSize += tracer.Field("ascii", IsoStream.CalculateStringSize(ascii), this.ascii); // 7-bit ASCII, of a terminating null
		boxSize += tracer.Field("unicode_language", 32, this.unicode_language); 
		boxSize += tracer.Field("unicode_count", 32, this.unicode_count); 
		boxSize += tracer.Field("unicode", IsoStream.CalculateStringSize(unicode), this.unicode); // UTF-16BE, of a terminating null
		boxSize += tracer.Field("scriptcode_code", 16, this.scriptcode_code); 
		boxSize += tracer.Field("scriptcode_count", 8, this.scriptcode_count); 
		boxSize += tracer.Field("scriptcode", 67 * 8, this.scriptcode); 
		return boxSize;
	}
}

}
