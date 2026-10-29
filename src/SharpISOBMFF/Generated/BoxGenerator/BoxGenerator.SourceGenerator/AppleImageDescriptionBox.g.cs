using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// A QuickTime image file's (QTIF) image description, as QuickTime's ImageDescription record has it: its own size and compressor type, a visual sample description's fields, then its extensions' atoms
class AppleImageDescriptionBox() extends Box ('idsc'){
 unsigned int(32) descriptionSize;
 unsigned int(32) compressorType;
 unsigned int(8) reserved[6];
 unsigned int(16) dataReferenceIndex;
 unsigned int(16) version;
 unsigned int(16) revisionLevel;
 unsigned int(32) vendor;
 unsigned int(32) temporalQuality;
 unsigned int(32) spatialQuality;
 unsigned int(16) width;
 unsigned int(16) height;
 unsigned int(32) horizontalResolution;
 unsigned int(32) verticalResolution;
 unsigned int(32) dataSize;
 unsigned int(16) frameCount;
 unsigned int(8) compressorName[32];
 unsigned int(16) depth;
 signed int(16) colorTableId;
 Box boxes[];
}
*/
public partial class AppleImageDescriptionBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"// A QuickTime image file's (QTIF) image description, as QuickTime's ImageDescription record has it: its own size and compressor type, a visual sample description's fields, then its extensions' atoms
class AppleImageDescriptionBox() extends Box ('idsc'){
 unsigned int(32) descriptionSize;
 unsigned int(32) compressorType;
 unsigned int(8) reserved[6];
 unsigned int(16) dataReferenceIndex;
 unsigned int(16) version;
 unsigned int(16) revisionLevel;
 unsigned int(32) vendor;
 unsigned int(32) temporalQuality;
 unsigned int(32) spatialQuality;
 unsigned int(16) width;
 unsigned int(16) height;
 unsigned int(32) horizontalResolution;
 unsigned int(32) verticalResolution;
 unsigned int(32) dataSize;
 unsigned int(16) frameCount;
 unsigned int(8) compressorName[32];
 unsigned int(16) depth;
 signed int(16) colorTableId;
 Box boxes[];
}"; } }
	public const string TYPE = "idsc";
	public override string DisplayName { get { return "AppleImageDescriptionBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("file"); }


	protected uint descriptionSize; 
	public uint DescriptionSize { get { return this.descriptionSize; } set { this.descriptionSize = value; } }

	protected uint compressorType; 
	public uint CompressorType { get { return this.compressorType; } set { this.compressorType = value; } }

	protected byte[] reserved = new byte[6]; 
	public byte[] Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected ushort dataReferenceIndex; 
	public ushort DataReferenceIndex { get { return this.dataReferenceIndex; } set { this.dataReferenceIndex = value; } }

	protected ushort version; 
	public ushort Version { get { return this.version; } set { this.version = value; } }

	protected ushort revisionLevel; 
	public ushort RevisionLevel { get { return this.revisionLevel; } set { this.revisionLevel = value; } }

	protected uint vendor; 
	public uint Vendor { get { return this.vendor; } set { this.vendor = value; } }

	protected uint temporalQuality; 
	public uint TemporalQuality { get { return this.temporalQuality; } set { this.temporalQuality = value; } }

	protected uint spatialQuality; 
	public uint SpatialQuality { get { return this.spatialQuality; } set { this.spatialQuality = value; } }

	protected ushort width; 
	public ushort Width { get { return this.width; } set { this.width = value; } }

	protected ushort height; 
	public ushort Height { get { return this.height; } set { this.height = value; } }

	protected uint horizontalResolution; 
	public uint HorizontalResolution { get { return this.horizontalResolution; } set { this.horizontalResolution = value; } }

	protected uint verticalResolution; 
	public uint VerticalResolution { get { return this.verticalResolution; } set { this.verticalResolution = value; } }

	protected uint dataSize; 
	public uint DataSize { get { return this.dataSize; } set { this.dataSize = value; } }

	protected ushort frameCount; 
	public ushort FrameCount { get { return this.frameCount; } set { this.frameCount = value; } }

	protected byte[] compressorName = new byte[32]; 
	public byte[] CompressorName { get { return this.compressorName; } set { this.compressorName = value; } }

	protected ushort depth; 
	public ushort Depth { get { return this.depth; } set { this.depth = value; } }

	protected short colorTableId; 
	public short ColorTableId { get { return this.colorTableId; } set { this.colorTableId = value; } }

	public AppleImageDescriptionBox(): base(IsoStream.FromFourCC("idsc"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.descriptionSize, "descriptionSize"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.compressorType, "compressorType"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 6,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.dataReferenceIndex, "dataReferenceIndex"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.version, "version"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.revisionLevel, "revisionLevel"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.vendor, "vendor"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.temporalQuality, "temporalQuality"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.spatialQuality, "spatialQuality"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.width, "width"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.height, "height"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.horizontalResolution, "horizontalResolution"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.verticalResolution, "verticalResolution"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.dataSize, "dataSize"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.frameCount, "frameCount"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 32,  out this.compressorName, "compressorName"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.depth, "depth"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.colorTableId, "colorTableId"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.boxes, "boxes"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.descriptionSize, "descriptionSize"); 
		boxSize += stream.WriteUInt32( this.compressorType, "compressorType"); 
		boxSize += stream.WriteUInt8Array(6,  this.reserved, "reserved"); 
		boxSize += stream.WriteUInt16( this.dataReferenceIndex, "dataReferenceIndex"); 
		boxSize += stream.WriteUInt16( this.version, "version"); 
		boxSize += stream.WriteUInt16( this.revisionLevel, "revisionLevel"); 
		boxSize += stream.WriteUInt32( this.vendor, "vendor"); 
		boxSize += stream.WriteUInt32( this.temporalQuality, "temporalQuality"); 
		boxSize += stream.WriteUInt32( this.spatialQuality, "spatialQuality"); 
		boxSize += stream.WriteUInt16( this.width, "width"); 
		boxSize += stream.WriteUInt16( this.height, "height"); 
		boxSize += stream.WriteUInt32( this.horizontalResolution, "horizontalResolution"); 
		boxSize += stream.WriteUInt32( this.verticalResolution, "verticalResolution"); 
		boxSize += stream.WriteUInt32( this.dataSize, "dataSize"); 
		boxSize += stream.WriteUInt16( this.frameCount, "frameCount"); 
		boxSize += stream.WriteUInt8Array(32,  this.compressorName, "compressorName"); 
		boxSize += stream.WriteUInt16( this.depth, "depth"); 
		boxSize += stream.WriteInt16( this.colorTableId, "colorTableId"); 
		// boxSize += stream.WriteBox( this.boxes, "boxes"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // descriptionSize
		boxSize += 32; // compressorType
		boxSize += 6 * 8; // reserved
		boxSize += 16; // dataReferenceIndex
		boxSize += 16; // version
		boxSize += 16; // revisionLevel
		boxSize += 32; // vendor
		boxSize += 32; // temporalQuality
		boxSize += 32; // spatialQuality
		boxSize += 16; // width
		boxSize += 16; // height
		boxSize += 32; // horizontalResolution
		boxSize += 32; // verticalResolution
		boxSize += 32; // dataSize
		boxSize += 16; // frameCount
		boxSize += 32 * 8; // compressorName
		boxSize += 16; // depth
		boxSize += 16; // colorTableId
		// boxSize += IsoStream.CalculateBoxSize(boxes); // boxes
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("descriptionSize", 32, this.descriptionSize); 
		boxSize += tracer.Field("compressorType", 32, this.compressorType); 
		boxSize += tracer.Field("reserved", 6 * 8, this.reserved); 
		boxSize += tracer.Field("dataReferenceIndex", 16, this.dataReferenceIndex); 
		boxSize += tracer.Field("version", 16, this.version); 
		boxSize += tracer.Field("revisionLevel", 16, this.revisionLevel); 
		boxSize += tracer.Field("vendor", 32, this.vendor); 
		boxSize += tracer.Field("temporalQuality", 32, this.temporalQuality); 
		boxSize += tracer.Field("spatialQuality", 32, this.spatialQuality); 
		boxSize += tracer.Field("width", 16, this.width); 
		boxSize += tracer.Field("height", 16, this.height); 
		boxSize += tracer.Field("horizontalResolution", 32, this.horizontalResolution); 
		boxSize += tracer.Field("verticalResolution", 32, this.verticalResolution); 
		boxSize += tracer.Field("dataSize", 32, this.dataSize); 
		boxSize += tracer.Field("frameCount", 16, this.frameCount); 
		boxSize += tracer.Field("compressorName", 32 * 8, this.compressorName); 
		boxSize += tracer.Field("depth", 16, this.depth); 
		boxSize += tracer.Field("colorTableId", 16, this.colorTableId); 
		// boxSize += tracer.Field("boxes", IsoStream.CalculateBoxSize(boxes), this.boxes); 
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
