using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class FDItemInfoExtension() extends ItemInfoExtension ('fdel') {
	utf8string content_location;
	utf8string content_MD5;
	unsigned int(64) content_length;
	unsigned int(64) transfer_length;
	unsigned int(8) entry_count;
	for (i=1; i <= entry_count; i++)
		unsigned int(32) group_id;
}
*/
public partial class FDItemInfoExtension : ItemInfoExtension
{
	public const string TYPE = "fdel";
	public override string DisplayName { get { return "FDItemInfoExtension"; } }

	protected BinaryUTF8String content_location; 
	public BinaryUTF8String ContentLocation { get { return this.content_location; } set { this.content_location = value; } }

	protected BinaryUTF8String content_MD5; 
	public BinaryUTF8String ContentMD5 { get { return this.content_MD5; } set { this.content_MD5 = value; } }

	protected ulong content_length; 
	public ulong ContentLength { get { return this.content_length; } set { this.content_length = value; } }

	protected ulong transfer_length; 
	public ulong TransferLength { get { return this.transfer_length; } set { this.transfer_length = value; } }

	protected byte entry_count; 
	public byte EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	protected uint[] group_id; 
	public uint[] GroupId { get { return this.group_id; } set { this.group_id = value; } }

	public FDItemInfoExtension(): base(IsoStream.FromFourCC("fdel"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.content_location, "content_location"); 
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.content_MD5, "content_MD5"); 
		boxSize += stream.ReadUInt64(boxSize, readSize,  out this.content_length, "content_length"); 
		boxSize += stream.ReadUInt64(boxSize, readSize,  out this.transfer_length, "transfer_length"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.entry_count, "entry_count"); 

		this.group_id = new uint[IsoStream.GetInt( entry_count)];
		for (int i=0; i < entry_count; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.group_id[i], "group_id"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.content_location, "content_location"); 
		boxSize += stream.WriteStringZeroTerminated( this.content_MD5, "content_MD5"); 
		boxSize += stream.WriteUInt64( this.content_length, "content_length"); 
		boxSize += stream.WriteUInt64( this.transfer_length, "transfer_length"); 
		boxSize += stream.WriteUInt8( this.entry_count, "entry_count"); 

		for (int i=0; i < entry_count; i++)
		{
			boxSize += stream.WriteUInt32( this.group_id[i], "group_id"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(content_location); // content_location
		boxSize += IsoStream.CalculateStringSize(content_MD5); // content_MD5
		boxSize += 64; // content_length
		boxSize += 64; // transfer_length
		boxSize += 8; // entry_count

		for (int i=0; i < entry_count; i++)
		{
			boxSize += 32; // group_id
		}
		return boxSize;
	}
}

}
