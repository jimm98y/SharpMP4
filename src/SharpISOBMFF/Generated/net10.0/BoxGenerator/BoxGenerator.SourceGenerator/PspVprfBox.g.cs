using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class PspVprfBox() extends Box('VPRF') {
	 unsigned int(32) unknown1;
 unsigned int(32) unknown2;
 unsigned int(32) codec;
 unsigned int(32) unknown3;
 unsigned int(32) unknown4;
 unsigned int(32) maxBitrate;
 unsigned int(32) avgBitrate;
 unsigned int(32) frameRate1;
 unsigned int(32) frameRate2;
 unsigned int(16) width;
 unsigned int(16) height;
 unsigned int(32) unknown5;
 }
*/
public partial class PspVprfBox : Box
{
	public const string TYPE = "VPRF";
	public override string DisplayName { get { return "PspVprfBox"; } }

	protected uint unknown1; 
	public uint Unknown1 { get { return this.unknown1; } set { this.unknown1 = value; } }

	protected uint unknown2; 
	public uint Unknown2 { get { return this.unknown2; } set { this.unknown2 = value; } }

	protected uint codec; 
	public uint Codec { get { return this.codec; } set { this.codec = value; } }

	protected uint unknown3; 
	public uint Unknown3 { get { return this.unknown3; } set { this.unknown3 = value; } }

	protected uint unknown4; 
	public uint Unknown4 { get { return this.unknown4; } set { this.unknown4 = value; } }

	protected uint maxBitrate; 
	public uint MaxBitrate { get { return this.maxBitrate; } set { this.maxBitrate = value; } }

	protected uint avgBitrate; 
	public uint AvgBitrate { get { return this.avgBitrate; } set { this.avgBitrate = value; } }

	protected uint frameRate1; 
	public uint FrameRate1 { get { return this.frameRate1; } set { this.frameRate1 = value; } }

	protected uint frameRate2; 
	public uint FrameRate2 { get { return this.frameRate2; } set { this.frameRate2 = value; } }

	protected ushort width; 
	public ushort Width { get { return this.width; } set { this.width = value; } }

	protected ushort height; 
	public ushort Height { get { return this.height; } set { this.height = value; } }

	protected uint unknown5; 
	public uint Unknown5 { get { return this.unknown5; } set { this.unknown5 = value; } }

	public PspVprfBox(): base(IsoStream.FromFourCC("VPRF"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.unknown1, "unknown1"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.unknown2, "unknown2"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.codec, "codec"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.unknown3, "unknown3"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.unknown4, "unknown4"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.maxBitrate, "maxBitrate"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.avgBitrate, "avgBitrate"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.frameRate1, "frameRate1"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.frameRate2, "frameRate2"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.width, "width"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.height, "height"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.unknown5, "unknown5"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.unknown1, "unknown1"); 
		boxSize += stream.WriteUInt32( this.unknown2, "unknown2"); 
		boxSize += stream.WriteUInt32( this.codec, "codec"); 
		boxSize += stream.WriteUInt32( this.unknown3, "unknown3"); 
		boxSize += stream.WriteUInt32( this.unknown4, "unknown4"); 
		boxSize += stream.WriteUInt32( this.maxBitrate, "maxBitrate"); 
		boxSize += stream.WriteUInt32( this.avgBitrate, "avgBitrate"); 
		boxSize += stream.WriteUInt32( this.frameRate1, "frameRate1"); 
		boxSize += stream.WriteUInt32( this.frameRate2, "frameRate2"); 
		boxSize += stream.WriteUInt16( this.width, "width"); 
		boxSize += stream.WriteUInt16( this.height, "height"); 
		boxSize += stream.WriteUInt32( this.unknown5, "unknown5"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // unknown1
		boxSize += 32; // unknown2
		boxSize += 32; // codec
		boxSize += 32; // unknown3
		boxSize += 32; // unknown4
		boxSize += 32; // maxBitrate
		boxSize += 32; // avgBitrate
		boxSize += 32; // frameRate1
		boxSize += 32; // frameRate2
		boxSize += 16; // width
		boxSize += 16; // height
		boxSize += 32; // unknown5
		return boxSize;
	}
}

}
