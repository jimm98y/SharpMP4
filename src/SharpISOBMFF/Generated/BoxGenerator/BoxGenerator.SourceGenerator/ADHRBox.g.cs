using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// Avid's DNxHR header, as FFmpeg's mov_write_avid_tag writes it
aligned(8) class ADHRBox() extends Box('ADHR') {
	unsigned int(32) version;
	unsigned int(32) compressionId;
	unsigned int(32) subSamplingControl;
	unsigned int(32) sampleBitDepth;
	unsigned int(16) colorFormat;
	unsigned int(16) colorVolume;
	unsigned int(16) alphaPresent;
	unsigned int(16) preMultipliedAlpha;
}
*/
public partial class ADHRBox : Box
{
	public const string TYPE = "ADHR";
	public override string DisplayName { get { return "ADHRBox"; } }

	protected uint version; 
	public uint Version { get { return this.version; } set { this.version = value; } }

	protected uint compressionId; 
	public uint CompressionId { get { return this.compressionId; } set { this.compressionId = value; } }

	protected uint subSamplingControl; 
	public uint SubSamplingControl { get { return this.subSamplingControl; } set { this.subSamplingControl = value; } }

	protected uint sampleBitDepth; 
	public uint SampleBitDepth { get { return this.sampleBitDepth; } set { this.sampleBitDepth = value; } }

	protected ushort colorFormat; 
	public ushort ColorFormat { get { return this.colorFormat; } set { this.colorFormat = value; } }

	protected ushort colorVolume; 
	public ushort ColorVolume { get { return this.colorVolume; } set { this.colorVolume = value; } }

	protected ushort alphaPresent; 
	public ushort AlphaPresent { get { return this.alphaPresent; } set { this.alphaPresent = value; } }

	protected ushort preMultipliedAlpha; 
	public ushort PreMultipliedAlpha { get { return this.preMultipliedAlpha; } set { this.preMultipliedAlpha = value; } }

	public ADHRBox(): base(IsoStream.FromFourCC("ADHR"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.version, "version"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.compressionId, "compressionId"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.subSamplingControl, "subSamplingControl"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sampleBitDepth, "sampleBitDepth"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.colorFormat, "colorFormat"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.colorVolume, "colorVolume"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.alphaPresent, "alphaPresent"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.preMultipliedAlpha, "preMultipliedAlpha"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.version, "version"); 
		boxSize += stream.WriteUInt32( this.compressionId, "compressionId"); 
		boxSize += stream.WriteUInt32( this.subSamplingControl, "subSamplingControl"); 
		boxSize += stream.WriteUInt32( this.sampleBitDepth, "sampleBitDepth"); 
		boxSize += stream.WriteUInt16( this.colorFormat, "colorFormat"); 
		boxSize += stream.WriteUInt16( this.colorVolume, "colorVolume"); 
		boxSize += stream.WriteUInt16( this.alphaPresent, "alphaPresent"); 
		boxSize += stream.WriteUInt16( this.preMultipliedAlpha, "preMultipliedAlpha"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // version
		boxSize += 32; // compressionId
		boxSize += 32; // subSamplingControl
		boxSize += 32; // sampleBitDepth
		boxSize += 16; // colorFormat
		boxSize += 16; // colorVolume
		boxSize += 16; // alphaPresent
		boxSize += 16; // preMultipliedAlpha
		return boxSize;
	}
}

}
