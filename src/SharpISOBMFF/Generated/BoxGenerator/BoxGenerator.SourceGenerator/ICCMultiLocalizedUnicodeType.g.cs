using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ICCMultiLocalizedUnicodeType {
	unsigned int(32) type_signature;
	unsigned int(32) reserved = 0;
	unsigned int(32) record_count;
	unsigned int(32) record_size;	// 12
	for (i = 0; i < record_count; i++) {
		unsigned int(16) language;	// ISO 639-1
		unsigned int(16) country;	// ISO 3166-1
		unsigned int(32) length;
		unsigned int(32) offset;	// of its string, from the start of the element
	}
	utf16string strings;	// UTF-16BE, each record's at its offset: all that is left
}
*/
public partial class ICCMultiLocalizedUnicodeType : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"aligned(8) class ICCMultiLocalizedUnicodeType {
	unsigned int(32) type_signature;
	unsigned int(32) reserved = 0;
	unsigned int(32) record_count;
	unsigned int(32) record_size;	// 12
	for (i = 0; i < record_count; i++) {
		unsigned int(16) language;	// ISO 639-1
		unsigned int(16) country;	// ISO 3166-1
		unsigned int(32) length;
		unsigned int(32) offset;	// of its string, from the start of the element
	}
	utf16string strings;	// UTF-16BE, each record's at its offset: all that is left
}"; } }
	public virtual string DisplayName { get { return "ICCMultiLocalizedUnicodeType"; } }

	protected uint type_signature; 
	public uint TypeSignature { get { return this.type_signature; } set { this.type_signature = value; } }

	protected uint reserved = 0; 
	public uint Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected uint record_count; 
	public uint RecordCount { get { return this.record_count; } set { this.record_count = value; } }

	protected uint record_size;  //  12
	public uint RecordSize { get { return this.record_size; } set { this.record_size = value; } }

	protected ushort[] language;  //  ISO 639-1
	public ushort[] Language { get { return this.language; } set { this.language = value; } }

	protected ushort[] country;  //  ISO 3166-1
	public ushort[] Country { get { return this.country; } set { this.country = value; } }

	protected uint[] length; 
	public uint[] Length { get { return this.length; } set { this.length = value; } }

	protected uint[] offset;  //  of its string, from the start of the element
	public uint[] Offset { get { return this.offset; } set { this.offset = value; } }

	protected BinaryUTF16String strings;  //  UTF-16BE, each record's at its offset: all that is left
	public BinaryUTF16String Strings { get { return this.strings; } set { this.strings = value; } }

	public ICCMultiLocalizedUnicodeType(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.type_signature, "type_signature"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.record_count, "record_count"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.record_size, "record_size"); // 12

		this.language = stream.SafeAllocate<ushort>(boxSize, readSize, IsoStream.GetInt( record_count), "language");
		this.country = stream.SafeAllocate<ushort>(boxSize, readSize, IsoStream.GetInt( record_count), "country");
		this.length = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt( record_count), "length");
		this.offset = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt( record_count), "offset");
		for (int i = 0; i < record_count; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.language[i], "language"); // ISO 639-1
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.country[i], "country"); // ISO 3166-1
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.length[i], "length"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.offset[i], "offset"); // of its string, from the start of the element
		}
		boxSize += stream.ReadUtf16StringTillEnd(boxSize, readSize,  out this.strings, "strings"); // UTF-16BE, each record's at its offset: all that is left
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt32( this.type_signature, "type_signature"); 
		boxSize += stream.WriteUInt32( this.reserved, "reserved"); 
		boxSize += stream.WriteUInt32( this.record_count, "record_count"); 
		boxSize += stream.WriteUInt32( this.record_size, "record_size"); // 12

		for (int i = 0; i < record_count; i++)
		{
			boxSize += stream.WriteUInt16( this.language[i], "language"); // ISO 639-1
			boxSize += stream.WriteUInt16( this.country[i], "country"); // ISO 3166-1
			boxSize += stream.WriteUInt32( this.length[i], "length"); 
			boxSize += stream.WriteUInt32( this.offset[i], "offset"); // of its string, from the start of the element
		}
		boxSize += stream.WriteUtf16String( this.strings, "strings"); // UTF-16BE, each record's at its offset: all that is left
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 32; // type_signature
		boxSize += 32; // reserved
		boxSize += 32; // record_count
		boxSize += 32; // record_size

		for (int i = 0; i < record_count; i++)
		{
			boxSize += 16; // language
			boxSize += 16; // country
			boxSize += 32; // length
			boxSize += 32; // offset
		}
		boxSize += IsoStream.CalculateStringSize(strings); // strings
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("type_signature", 32, this.type_signature); 
		boxSize += tracer.Field("reserved", 32, this.reserved); 
		boxSize += tracer.Field("record_count", 32, this.record_count); 
		boxSize += tracer.Field("record_size", 32, this.record_size); // 12

		for (int i = 0; i < record_count; i++)
		{
			boxSize += tracer.Field($"language[{i}]", 16, this.language[i]); // ISO 639-1
			boxSize += tracer.Field($"country[{i}]", 16, this.country[i]); // ISO 3166-1
			boxSize += tracer.Field($"length[{i}]", 32, this.length[i]); 
			boxSize += tracer.Field($"offset[{i}]", 32, this.offset[i]); // of its string, from the start of the element
		}
		boxSize += tracer.Field("strings", IsoStream.CalculateStringSize(strings), this.strings); // UTF-16BE, each record's at its offset: all that is left
		return boxSize;
	}
}

}
