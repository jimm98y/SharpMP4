using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// A CR3's image: a visual sample entry, then its type (3, 0 for JPEG; 1, 1 for raw) and its boxes (JPEG, CMP1, CDI1)
aligned(8) class CanonRawSampleEntry() extends SampleEntry('CRAW') {
 unsigned int(16) pre_defined;
 unsigned int(16) reserved;
 unsigned int(32) pre_defined0[3];
 unsigned int(16) width;
 unsigned int(16) height;
 unsigned int(32) horizresolution;
 unsigned int(32) vertresolution;
 unsigned int(32) reserved0;
 unsigned int(16) frame_count;
 unsigned int(8) compressorname[32];
 unsigned int(16) depth;
 signed int(16) pre_defined1;
 unsigned int(16) imageType;
 unsigned int(16) imageFlags;
 Box boxes[];
}

*/
public partial class CanonRawSampleEntry : SampleEntry
{
	public const string TYPE = "CRAW";
	public override string DisplayName { get { return "CanonRawSampleEntry"; } }

	protected ushort pre_defined; 
	public ushort PreDefined { get { return this.pre_defined; } set { this.pre_defined = value; } }

	protected ushort reserved; 
	public ushort Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected uint[] pre_defined0; 
	public uint[] PreDefined0 { get { return this.pre_defined0; } set { this.pre_defined0 = value; } }

	protected ushort width; 
	public ushort Width { get { return this.width; } set { this.width = value; } }

	protected ushort height; 
	public ushort Height { get { return this.height; } set { this.height = value; } }

	protected uint horizresolution; 
	public uint Horizresolution { get { return this.horizresolution; } set { this.horizresolution = value; } }

	protected uint vertresolution; 
	public uint Vertresolution { get { return this.vertresolution; } set { this.vertresolution = value; } }

	protected uint reserved0; 
	public uint Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected ushort frame_count; 
	public ushort FrameCount { get { return this.frame_count; } set { this.frame_count = value; } }

	protected byte[] compressorname; 
	public byte[] Compressorname { get { return this.compressorname; } set { this.compressorname = value; } }

	protected ushort depth; 
	public ushort Depth { get { return this.depth; } set { this.depth = value; } }

	protected short pre_defined1; 
	public short PreDefined1 { get { return this.pre_defined1; } set { this.pre_defined1 = value; } }

	protected ushort imageType; 
	public ushort ImageType { get { return this.imageType; } set { this.imageType = value; } }

	protected ushort imageFlags; 
	public ushort ImageFlags { get { return this.imageFlags; } set { this.imageFlags = value; } }

	public CanonRawSampleEntry(): base(IsoStream.FromFourCC("CRAW"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.pre_defined, "pre_defined"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt32Array(boxSize, readSize, 3,  out this.pre_defined0, "pre_defined0"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.width, "width"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.height, "height"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.horizresolution, "horizresolution"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.vertresolution, "vertresolution"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reserved0, "reserved0"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.frame_count, "frame_count"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 32,  out this.compressorname, "compressorname"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.depth, "depth"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.pre_defined1, "pre_defined1"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.imageType, "imageType"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.imageFlags, "imageFlags"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.boxes, "boxes"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.pre_defined, "pre_defined"); 
		boxSize += stream.WriteUInt16( this.reserved, "reserved"); 
		boxSize += stream.WriteUInt32Array(3,  this.pre_defined0, "pre_defined0"); 
		boxSize += stream.WriteUInt16( this.width, "width"); 
		boxSize += stream.WriteUInt16( this.height, "height"); 
		boxSize += stream.WriteUInt32( this.horizresolution, "horizresolution"); 
		boxSize += stream.WriteUInt32( this.vertresolution, "vertresolution"); 
		boxSize += stream.WriteUInt32( this.reserved0, "reserved0"); 
		boxSize += stream.WriteUInt16( this.frame_count, "frame_count"); 
		boxSize += stream.WriteUInt8Array(32,  this.compressorname, "compressorname"); 
		boxSize += stream.WriteUInt16( this.depth, "depth"); 
		boxSize += stream.WriteInt16( this.pre_defined1, "pre_defined1"); 
		boxSize += stream.WriteUInt16( this.imageType, "imageType"); 
		boxSize += stream.WriteUInt16( this.imageFlags, "imageFlags"); 
		// boxSize += stream.WriteBox( this.boxes, "boxes"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // pre_defined
		boxSize += 16; // reserved
		boxSize += 3 * 32; // pre_defined0
		boxSize += 16; // width
		boxSize += 16; // height
		boxSize += 32; // horizresolution
		boxSize += 32; // vertresolution
		boxSize += 32; // reserved0
		boxSize += 16; // frame_count
		boxSize += 32 * 8; // compressorname
		boxSize += 16; // depth
		boxSize += 16; // pre_defined1
		boxSize += 16; // imageType
		boxSize += 16; // imageFlags
		// boxSize += IsoStream.CalculateBoxSize(boxes); // boxes
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
