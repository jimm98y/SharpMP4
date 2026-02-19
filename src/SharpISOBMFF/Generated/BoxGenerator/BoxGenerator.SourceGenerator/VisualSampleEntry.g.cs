using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class VisualSampleEntry(codingname) extends SampleEntry (codingname){
	unsigned int(16) pre_defined = 0;
	const unsigned int(16) reserved = 0;
	unsigned int(32)[3]	pre_defined = 0;
	unsigned int(16)	width;
	unsigned int(16)	height;
	template unsigned int(32)	horizresolution = 0x00480000;	// 72 dpi
	template unsigned int(32)	vertresolution  = 0x00480000;	// 72 dpi
	const unsigned int(32)	reserved = 0;
	template unsigned int(16)	frame_count = 1;
	uint(8)[32]	compressorname;
	template unsigned int(16)	depth = 0x0018;
	int(16)	pre_defined = -1;
	// other boxes from derived specifications
	CleanApertureBox			clap;		// optional
	PixelAspectRatioBox		pasp;		// optional
}

*/
public partial class VisualSampleEntry : SampleEntry
{
	public override string DisplayName { get { return "VisualSampleEntry"; } }

	protected ushort pre_defined = 0; 
	public ushort PreDefined { get { return this.pre_defined; } set { this.pre_defined = value; } }

	protected ushort reserved = 0; 
	public ushort Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected uint[] pre_defined0 = []; 
	public uint[] PreDefined0 { get { return this.pre_defined0; } set { this.pre_defined0 = value; } }

	protected ushort width; 
	public ushort Width { get { return this.width; } set { this.width = value; } }

	protected ushort height; 
	public ushort Height { get { return this.height; } set { this.height = value; } }

	protected uint horizresolution = 0x00480000;  //  72 dpi
	public uint Horizresolution { get { return this.horizresolution; } set { this.horizresolution = value; } }

	protected uint vertresolution = 0x00480000;  //  72 dpi
	public uint Vertresolution { get { return this.vertresolution; } set { this.vertresolution = value; } }

	protected uint reserved0 = 0; 
	public uint Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected ushort frame_count = 1; 
	public ushort FrameCount { get { return this.frame_count; } set { this.frame_count = value; } }

	protected byte[] compressorname; 
	public byte[] Compressorname { get { return this.compressorname; } set { this.compressorname = value; } }

	protected ushort depth = 0x0018; 
	public ushort Depth { get { return this.depth; } set { this.depth = value; } }

	protected short pre_defined1 = -1;  //  other boxes from derived specifications
	public short PreDefined1 { get { return this.pre_defined1; } set { this.pre_defined1 = value; } }
	public CleanApertureBox Clap { get { return this.children.OfType<CleanApertureBox>().FirstOrDefault(); } }
	public PixelAspectRatioBox Pasp { get { return this.children.OfType<PixelAspectRatioBox>().FirstOrDefault(); } }

	public VisualSampleEntry(uint codingname = 0): base(codingname)
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
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.horizresolution, "horizresolution"); // 72 dpi
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.vertresolution, "vertresolution"); // 72 dpi
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reserved0, "reserved0"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.frame_count, "frame_count"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 32,  out this.compressorname, "compressorname"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.depth, "depth"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.pre_defined1, "pre_defined1"); // other boxes from derived specifications
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.clap, "clap"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.pasp, "pasp"); // optional
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
		boxSize += stream.WriteUInt32( this.horizresolution, "horizresolution"); // 72 dpi
		boxSize += stream.WriteUInt32( this.vertresolution, "vertresolution"); // 72 dpi
		boxSize += stream.WriteUInt32( this.reserved0, "reserved0"); 
		boxSize += stream.WriteUInt16( this.frame_count, "frame_count"); 
		boxSize += stream.WriteUInt8Array(32,  this.compressorname, "compressorname"); 
		boxSize += stream.WriteUInt16( this.depth, "depth"); 
		boxSize += stream.WriteInt16( this.pre_defined1, "pre_defined1"); // other boxes from derived specifications
		// boxSize += stream.WriteBox( this.clap, "clap"); // optional
		// boxSize += stream.WriteBox( this.pasp, "pasp"); // optional
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
		// boxSize += IsoStream.CalculateBoxSize(clap); // clap
		// boxSize += IsoStream.CalculateBoxSize(pasp); // pasp
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
