using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ItemInfoEntry
		extends FullBox('infe', version, flags) {
	if ((version == 0) || (version == 1)) {
		unsigned int(16) item_ID;
		unsigned int(16) item_protection_index;
		utf8string item_name;
		utf8string content_type;
		utf8string content_encoding; //optional
	}
	if (version == 1) {
		unsigned int(32) extension_type; //optional
		ItemInfoExtension(extension_type); //optional
	}
	if (version >= 2) {
		if (version == 2) {
			unsigned int(16) item_ID;
		} else if (version == 3) {
			unsigned int(32) item_ID;
		}
		unsigned int(16) item_protection_index;
		unsigned int(32) item_type;
		utf8string item_name;
		if (item_type=='mime') {
			utf8string content_type;
			utf8string content_encoding; //optional
		} else if (item_type == 'uri ') {
			utf8string item_uri_type;
		}
	}
}
*/
public partial class ItemInfoEntry : FullBox
{
	public const string TYPE = "infe";
	public override string DisplayName { get { return "ItemInfoEntry"; } }

	protected uint item_ID; 
	public uint ItemID { get { return this.item_ID; } set { this.item_ID = value; } }

	protected ushort item_protection_index; 
	public ushort ItemProtectionIndex { get { return this.item_protection_index; } set { this.item_protection_index = value; } }

	protected BinaryUTF8String item_name; 
	public BinaryUTF8String ItemName { get { return this.item_name; } set { this.item_name = value; } }

	protected BinaryUTF8String content_type; 
	public BinaryUTF8String ContentType { get { return this.content_type; } set { this.content_type = value; } }

	protected BinaryUTF8String content_encoding;  // optional
	public BinaryUTF8String ContentEncoding { get { return this.content_encoding; } set { this.content_encoding = value; } }

	protected uint extension_type;  // optional
	public uint ExtensionType { get { return this.extension_type; } set { this.extension_type = value; } }

	protected ItemInfoExtension ItemInfoExtension;  // optional
	public ItemInfoExtension _ItemInfoExtension { get { return this.ItemInfoExtension; } set { this.ItemInfoExtension = value; } }

	protected uint item_type; 
	public uint ItemType { get { return this.item_type; } set { this.item_type = value; } }

	protected BinaryUTF8String item_uri_type; 
	public BinaryUTF8String ItemUriType { get { return this.item_uri_type; } set { this.item_uri_type = value; } }

	public ItemInfoEntry(byte version = 0, uint flags = 0): base(IsoStream.FromFourCC("infe"), version, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);

		if ((version == 0) || (version == 1))
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.item_ID, "item_ID"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.item_protection_index, "item_protection_index"); 
			boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.item_name, "item_name"); 
			boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.content_type, "content_type"); 
			if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.content_encoding, "content_encoding"); //optional
		}

		if (version == 1)
		{
			if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadUInt32(boxSize, readSize,  out this.extension_type, "extension_type"); //optional
			if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadClass(boxSize, readSize, this, () => new ItemInfoExtension(extension_type),  out this.ItemInfoExtension, "ItemInfoExtension"); //optional
		}

		if (version >= 2)
		{

			if (version == 2)
			{
				boxSize += stream.ReadUInt16(boxSize, readSize,  out this.item_ID, "item_ID"); 
			}

			else if (version == 3)
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.item_ID, "item_ID"); 
			}
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.item_protection_index, "item_protection_index"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.item_type, "item_type"); 
			boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.item_name, "item_name"); 

			if (item_type==IsoStream.FromFourCC("mime"))
			{
				boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.content_type, "content_type"); 
				if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.content_encoding, "content_encoding"); //optional
			}

			else if (item_type == IsoStream.FromFourCC("uri "))
			{
				boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.item_uri_type, "item_uri_type"); 
			}
		}
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);

		if ((version == 0) || (version == 1))
		{
			boxSize += stream.WriteUInt16( this.item_ID, "item_ID"); 
			boxSize += stream.WriteUInt16( this.item_protection_index, "item_protection_index"); 
			boxSize += stream.WriteStringZeroTerminated( this.item_name, "item_name"); 
			boxSize += stream.WriteStringZeroTerminated( this.content_type, "content_type"); 
			boxSize += stream.WriteStringZeroTerminated( this.content_encoding, "content_encoding"); //optional
		}

		if (version == 1)
		{
			boxSize += stream.WriteUInt32( this.extension_type, "extension_type"); //optional
			boxSize += stream.WriteClass( this.ItemInfoExtension, "ItemInfoExtension"); //optional
		}

		if (version >= 2)
		{

			if (version == 2)
			{
				boxSize += stream.WriteUInt16( this.item_ID, "item_ID"); 
			}

			else if (version == 3)
			{
				boxSize += stream.WriteUInt32( this.item_ID, "item_ID"); 
			}
			boxSize += stream.WriteUInt16( this.item_protection_index, "item_protection_index"); 
			boxSize += stream.WriteUInt32( this.item_type, "item_type"); 
			boxSize += stream.WriteStringZeroTerminated( this.item_name, "item_name"); 

			if (item_type==IsoStream.FromFourCC("mime"))
			{
				boxSize += stream.WriteStringZeroTerminated( this.content_type, "content_type"); 
				boxSize += stream.WriteStringZeroTerminated( this.content_encoding, "content_encoding"); //optional
			}

			else if (item_type == IsoStream.FromFourCC("uri "))
			{
				boxSize += stream.WriteStringZeroTerminated( this.item_uri_type, "item_uri_type"); 
			}
		}
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();

		if ((version == 0) || (version == 1))
		{
			boxSize += 16; // item_ID
			boxSize += 16; // item_protection_index
			boxSize += IsoStream.CalculateStringSize(item_name); // item_name
			boxSize += IsoStream.CalculateStringSize(content_type); // content_type
			boxSize += IsoStream.CalculateStringSize(content_encoding); // content_encoding
		}

		if (version == 1)
		{
			boxSize += 32; // extension_type
			boxSize += IsoStream.CalculateClassSize(ItemInfoExtension); // ItemInfoExtension
		}

		if (version >= 2)
		{

			if (version == 2)
			{
				boxSize += 16; // item_ID
			}

			else if (version == 3)
			{
				boxSize += 32; // item_ID
			}
			boxSize += 16; // item_protection_index
			boxSize += 32; // item_type
			boxSize += IsoStream.CalculateStringSize(item_name); // item_name

			if (item_type==IsoStream.FromFourCC("mime"))
			{
				boxSize += IsoStream.CalculateStringSize(content_type); // content_type
				boxSize += IsoStream.CalculateStringSize(content_encoding); // content_encoding
			}

			else if (item_type == IsoStream.FromFourCC("uri "))
			{
				boxSize += IsoStream.CalculateStringSize(item_uri_type); // item_uri_type
			}
		}
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
