using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// QuickTime.pm, PreviewImage: 0, 1, width, height, 1, the length of the preview, the preview
aligned(8) class CanonPreviewBox() extends Box('PRVW') {
 unsigned int(32) unknown1;
 unsigned int(16) unknown2;
 unsigned int(16) width;
 unsigned int(16) height;
 unsigned int(16) unknown3;
 unsigned int(32) previewSize;
 bit(8) data[];
}

*/
public partial class CanonPreviewBox : Box
{
	public const string TYPE = "PRVW";
	public override string DisplayName { get { return "CanonPreviewBox"; } }

	protected uint unknown1; 
	public uint Unknown1 { get { return this.unknown1; } set { this.unknown1 = value; } }

	protected ushort unknown2; 
	public ushort Unknown2 { get { return this.unknown2; } set { this.unknown2 = value; } }

	protected ushort width; 
	public ushort Width { get { return this.width; } set { this.width = value; } }

	protected ushort height; 
	public ushort Height { get { return this.height; } set { this.height = value; } }

	protected ushort unknown3; 
	public ushort Unknown3 { get { return this.unknown3; } set { this.unknown3 = value; } }

	protected uint previewSize; 
	public uint PreviewSize { get { return this.previewSize; } set { this.previewSize = value; } }

	protected byte[] data; 
	public byte[] Data { get { return this.data; } set { this.data = value; } }

	public CanonPreviewBox(): base(IsoStream.FromFourCC("PRVW"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.unknown1, "unknown1"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.unknown2, "unknown2"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.width, "width"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.height, "height"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.unknown3, "unknown3"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.previewSize, "previewSize"); 
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.data, "data"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.unknown1, "unknown1"); 
		boxSize += stream.WriteUInt16( this.unknown2, "unknown2"); 
		boxSize += stream.WriteUInt16( this.width, "width"); 
		boxSize += stream.WriteUInt16( this.height, "height"); 
		boxSize += stream.WriteUInt16( this.unknown3, "unknown3"); 
		boxSize += stream.WriteUInt32( this.previewSize, "previewSize"); 
		boxSize += stream.WriteUInt8ArrayTillEnd( this.data, "data"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // unknown1
		boxSize += 16; // unknown2
		boxSize += 16; // width
		boxSize += 16; // height
		boxSize += 16; // unknown3
		boxSize += 32; // previewSize
		boxSize += ((ulong)data.Length * 8); // data
		return boxSize;
	}
}

}
