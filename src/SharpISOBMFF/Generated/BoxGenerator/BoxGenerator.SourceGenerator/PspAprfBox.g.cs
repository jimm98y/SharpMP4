using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class PspAprfBox() extends Box('APRF') {
	unsigned int(32) unknown1;
	unsigned int(32) unknown2;
	unsigned int(32) codec;
	unsigned int(32) unknown3;
	unsigned int(32) unknown4;
	unsigned int(32) maxBitrate;
	unsigned int(32) avgBitrate;
	unsigned int(32) frameRate;
	unsigned int(32) channels;
}
*/
public partial class PspAprfBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class PspAprfBox() extends Box('APRF') {
	unsigned int(32) unknown1;
	unsigned int(32) unknown2;
	unsigned int(32) codec;
	unsigned int(32) unknown3;
	unsigned int(32) unknown4;
	unsigned int(32) maxBitrate;
	unsigned int(32) avgBitrate;
	unsigned int(32) frameRate;
	unsigned int(32) channels;
}"; } }
	public const string TYPE = "APRF";
	public override string DisplayName { get { return "PspAprfBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("uuid"); }


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

	protected uint frameRate; 
	public uint FrameRate { get { return this.frameRate; } set { this.frameRate = value; } }

	protected uint channels; 
	public uint Channels { get { return this.channels; } set { this.channels = value; } }

	public PspAprfBox(): base(IsoStream.FromFourCC("APRF"))
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
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.frameRate, "frameRate"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.channels, "channels"); 
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
		boxSize += stream.WriteUInt32( this.frameRate, "frameRate"); 
		boxSize += stream.WriteUInt32( this.channels, "channels"); 
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
		boxSize += 32; // frameRate
		boxSize += 32; // channels
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("unknown1", 32, this.unknown1); 
		boxSize += tracer.Field("unknown2", 32, this.unknown2); 
		boxSize += tracer.Field("codec", 32, this.codec); 
		boxSize += tracer.Field("unknown3", 32, this.unknown3); 
		boxSize += tracer.Field("unknown4", 32, this.unknown4); 
		boxSize += tracer.Field("maxBitrate", 32, this.maxBitrate); 
		boxSize += tracer.Field("avgBitrate", 32, this.avgBitrate); 
		boxSize += tracer.Field("frameRate", 32, this.frameRate); 
		boxSize += tracer.Field("channels", 32, this.channels); 
		return boxSize;
	}
}

}
