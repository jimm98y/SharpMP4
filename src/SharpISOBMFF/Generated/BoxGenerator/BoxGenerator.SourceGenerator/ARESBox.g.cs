using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// Avid's resolution, as FFmpeg's mov_write_avid_tag writes it and mov_read_ares reads it, then padding
aligned(8) class ARESBox() extends Box('ARES') {
	unsigned int(32) tag;
	unsigned int(32) version;
	unsigned int(32) compressionId;
	unsigned int(32) fieldWidth;
	unsigned int(32) fieldHeight;
	unsigned int(32) numFields;
	unsigned int(32) numBlackLines;
	unsigned int(32) videoFormat;
	bit(8) reserved[];
}
*/
public partial class ARESBox : Box
{
	public const string TYPE = "ARES";
	public override string DisplayName { get { return "ARESBox"; } }

	protected uint tag; 
	public uint Tag { get { return this.tag; } set { this.tag = value; } }

	protected uint version; 
	public uint Version { get { return this.version; } set { this.version = value; } }

	protected uint compressionId; 
	public uint CompressionId { get { return this.compressionId; } set { this.compressionId = value; } }

	protected uint fieldWidth; 
	public uint FieldWidth { get { return this.fieldWidth; } set { this.fieldWidth = value; } }

	protected uint fieldHeight; 
	public uint FieldHeight { get { return this.fieldHeight; } set { this.fieldHeight = value; } }

	protected uint numFields; 
	public uint NumFields { get { return this.numFields; } set { this.numFields = value; } }

	protected uint numBlackLines; 
	public uint NumBlackLines { get { return this.numBlackLines; } set { this.numBlackLines = value; } }

	protected uint videoFormat; 
	public uint VideoFormat { get { return this.videoFormat; } set { this.videoFormat = value; } }

	protected byte[] reserved; 
	public byte[] Reserved { get { return this.reserved; } set { this.reserved = value; } }

	public ARESBox(): base(IsoStream.FromFourCC("ARES"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.tag, "tag"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.version, "version"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.compressionId, "compressionId"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.fieldWidth, "fieldWidth"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.fieldHeight, "fieldHeight"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.numFields, "numFields"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.numBlackLines, "numBlackLines"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.videoFormat, "videoFormat"); 
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.tag, "tag"); 
		boxSize += stream.WriteUInt32( this.version, "version"); 
		boxSize += stream.WriteUInt32( this.compressionId, "compressionId"); 
		boxSize += stream.WriteUInt32( this.fieldWidth, "fieldWidth"); 
		boxSize += stream.WriteUInt32( this.fieldHeight, "fieldHeight"); 
		boxSize += stream.WriteUInt32( this.numFields, "numFields"); 
		boxSize += stream.WriteUInt32( this.numBlackLines, "numBlackLines"); 
		boxSize += stream.WriteUInt32( this.videoFormat, "videoFormat"); 
		boxSize += stream.WriteUInt8ArrayTillEnd( this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // tag
		boxSize += 32; // version
		boxSize += 32; // compressionId
		boxSize += 32; // fieldWidth
		boxSize += 32; // fieldHeight
		boxSize += 32; // numFields
		boxSize += 32; // numBlackLines
		boxSize += 32; // videoFormat
		boxSize += ((ulong)reserved.Length * 8); // reserved
		return boxSize;
	}
}

}
