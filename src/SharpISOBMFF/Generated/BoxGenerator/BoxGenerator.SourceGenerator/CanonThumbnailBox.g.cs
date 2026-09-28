using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// Canon.pm, THMB: the thumbnail is after 16 bytes
aligned(8) class CanonThumbnailBox() extends FullBox('THMB') {
 unsigned int(16) width;
 unsigned int(16) height;
 unsigned int(32) thumbnailSize;
 unsigned int(16) unknown1;
 unsigned int(16) unknown2;
 bit(8) data[];
}

*/
public partial class CanonThumbnailBox : FullBox
{
	public const string TYPE = "THMB";
	public override string DisplayName { get { return "CanonThumbnailBox"; } }

	protected ushort width; 
	public ushort Width { get { return this.width; } set { this.width = value; } }

	protected ushort height; 
	public ushort Height { get { return this.height; } set { this.height = value; } }

	protected uint thumbnailSize; 
	public uint ThumbnailSize { get { return this.thumbnailSize; } set { this.thumbnailSize = value; } }

	protected ushort unknown1; 
	public ushort Unknown1 { get { return this.unknown1; } set { this.unknown1 = value; } }

	protected ushort unknown2; 
	public ushort Unknown2 { get { return this.unknown2; } set { this.unknown2 = value; } }

	protected byte[] data; 
	public byte[] Data { get { return this.data; } set { this.data = value; } }

	public CanonThumbnailBox(): base(IsoStream.FromFourCC("THMB"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.width, "width"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.height, "height"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.thumbnailSize, "thumbnailSize"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.unknown1, "unknown1"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.unknown2, "unknown2"); 
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.data, "data"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.width, "width"); 
		boxSize += stream.WriteUInt16( this.height, "height"); 
		boxSize += stream.WriteUInt32( this.thumbnailSize, "thumbnailSize"); 
		boxSize += stream.WriteUInt16( this.unknown1, "unknown1"); 
		boxSize += stream.WriteUInt16( this.unknown2, "unknown2"); 
		boxSize += stream.WriteUInt8ArrayTillEnd( this.data, "data"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // width
		boxSize += 16; // height
		boxSize += 32; // thumbnailSize
		boxSize += 16; // unknown1
		boxSize += 16; // unknown2
		boxSize += ((ulong)data.Length * 8); // data
		return boxSize;
	}
}

}
