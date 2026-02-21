using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class FilePartitionBox
		extends FullBox('fpar', version, 0) {
	if (version == 0) {
		unsigned int(16)	item_ID;
	} else {
		unsigned int(32)	item_ID;
	}
	unsigned int(16)	packet_payload_size;
	unsigned int(8)	reserved = 0;
	unsigned int(8)	FEC_encoding_ID;
	unsigned int(16)	FEC_instance_ID;
	unsigned int(16)	max_source_block_length;
	unsigned int(16)	encoding_symbol_length;
	unsigned int(16)	max_number_of_encoding_symbols;
	base64string		scheme_specific_info;
	if (version == 0) {
		unsigned int(16)	entry_count;
	} else {
		unsigned int(32)	entry_count;
	}
	for (i=1; i <= entry_count; i++) {
		unsigned int(16)	block_count;
		unsigned int(32)	block_size;
	}
}
*/
public partial class FilePartitionBox : FullBox
{
	public const string TYPE = "fpar";
	public override string DisplayName { get { return "FilePartitionBox"; } }

	protected uint item_ID; 
	public uint ItemID { get { return this.item_ID; } set { this.item_ID = value; } }

	protected ushort packet_payload_size; 
	public ushort PacketPayloadSize { get { return this.packet_payload_size; } set { this.packet_payload_size = value; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte FEC_encoding_ID; 
	public byte FECEncodingID { get { return this.FEC_encoding_ID; } set { this.FEC_encoding_ID = value; } }

	protected ushort FEC_instance_ID; 
	public ushort FECInstanceID { get { return this.FEC_instance_ID; } set { this.FEC_instance_ID = value; } }

	protected ushort max_source_block_length; 
	public ushort MaxSourceBlockLength { get { return this.max_source_block_length; } set { this.max_source_block_length = value; } }

	protected ushort encoding_symbol_length; 
	public ushort EncodingSymbolLength { get { return this.encoding_symbol_length; } set { this.encoding_symbol_length = value; } }

	protected ushort max_number_of_encoding_symbols; 
	public ushort MaxNumberOfEncodingSymbols { get { return this.max_number_of_encoding_symbols; } set { this.max_number_of_encoding_symbols = value; } }

	protected BinaryUTF8String scheme_specific_info; 
	public BinaryUTF8String SchemeSpecificInfo { get { return this.scheme_specific_info; } set { this.scheme_specific_info = value; } }

	protected uint entry_count; 
	public uint EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	protected ushort[] block_count; 
	public ushort[] BlockCount { get { return this.block_count; } set { this.block_count = value; } }

	protected uint[] block_size; 
	public uint[] BlockSize { get { return this.block_size; } set { this.block_size = value; } }

	public FilePartitionBox(byte version = 0): base(IsoStream.FromFourCC("fpar"), version, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);

		if (version == 0)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.item_ID, "item_ID"); 
		}

		else 
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.item_ID, "item_ID"); 
		}
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.packet_payload_size, "packet_payload_size"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.FEC_encoding_ID, "FEC_encoding_ID"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.FEC_instance_ID, "FEC_instance_ID"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.max_source_block_length, "max_source_block_length"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.encoding_symbol_length, "encoding_symbol_length"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.max_number_of_encoding_symbols, "max_number_of_encoding_symbols"); 
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.scheme_specific_info, "scheme_specific_info"); 

		if (version == 0)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.entry_count, "entry_count"); 
		}

		else 
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entry_count, "entry_count"); 
		}

		this.block_count = new ushort[IsoStream.GetInt( entry_count)];
		this.block_size = new uint[IsoStream.GetInt( entry_count)];
		for (int i=0; i < entry_count; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.block_count[i], "block_count"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.block_size[i], "block_size"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);

		if (version == 0)
		{
			boxSize += stream.WriteUInt16( this.item_ID, "item_ID"); 
		}

		else 
		{
			boxSize += stream.WriteUInt32( this.item_ID, "item_ID"); 
		}
		boxSize += stream.WriteUInt16( this.packet_payload_size, "packet_payload_size"); 
		boxSize += stream.WriteUInt8( this.reserved, "reserved"); 
		boxSize += stream.WriteUInt8( this.FEC_encoding_ID, "FEC_encoding_ID"); 
		boxSize += stream.WriteUInt16( this.FEC_instance_ID, "FEC_instance_ID"); 
		boxSize += stream.WriteUInt16( this.max_source_block_length, "max_source_block_length"); 
		boxSize += stream.WriteUInt16( this.encoding_symbol_length, "encoding_symbol_length"); 
		boxSize += stream.WriteUInt16( this.max_number_of_encoding_symbols, "max_number_of_encoding_symbols"); 
		boxSize += stream.WriteStringZeroTerminated( this.scheme_specific_info, "scheme_specific_info"); 

		if (version == 0)
		{
			boxSize += stream.WriteUInt16( this.entry_count, "entry_count"); 
		}

		else 
		{
			boxSize += stream.WriteUInt32( this.entry_count, "entry_count"); 
		}

		for (int i=0; i < entry_count; i++)
		{
			boxSize += stream.WriteUInt16( this.block_count[i], "block_count"); 
			boxSize += stream.WriteUInt32( this.block_size[i], "block_size"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();

		if (version == 0)
		{
			boxSize += 16; // item_ID
		}

		else 
		{
			boxSize += 32; // item_ID
		}
		boxSize += 16; // packet_payload_size
		boxSize += 8; // reserved
		boxSize += 8; // FEC_encoding_ID
		boxSize += 16; // FEC_instance_ID
		boxSize += 16; // max_source_block_length
		boxSize += 16; // encoding_symbol_length
		boxSize += 16; // max_number_of_encoding_symbols
		boxSize += IsoStream.CalculateStringSize(scheme_specific_info); // scheme_specific_info

		if (version == 0)
		{
			boxSize += 16; // entry_count
		}

		else 
		{
			boxSize += 32; // entry_count
		}

		for (int i=0; i < entry_count; i++)
		{
			boxSize += 16; // block_count
			boxSize += 32; // block_size
		}
		return boxSize;
	}
}

}
