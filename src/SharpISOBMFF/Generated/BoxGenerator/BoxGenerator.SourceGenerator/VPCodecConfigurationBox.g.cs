using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class VPCodecConfigurationBox() extends FullBox('vpcC') {
 unsigned int(8) profile;
 unsigned int(8) level;
 bit(4) bitDepth;
 bit(3) chromaSubsampling;
 bit(1) videoFullRangeFlag;
 unsigned int(8) colourPrimaries; unsigned int(8) transferCharacteristics;
 unsigned int(8) matrixCoefficients;
 unsigned int(16) codecInitializationData;
 } 
*/
public partial class VPCodecConfigurationBox : FullBox
{
	public const string TYPE = "vpcC";
	public override string DisplayName { get { return "VPCodecConfigurationBox"; } }

	protected byte profile; 
	public byte Profile { get { return this.profile; } set { this.profile = value; } }

	protected byte level; 
	public byte Level { get { return this.level; } set { this.level = value; } }

	protected byte bitDepth; 
	public byte BitDepth { get { return this.bitDepth; } set { this.bitDepth = value; } }

	protected byte chromaSubsampling; 
	public byte ChromaSubsampling { get { return this.chromaSubsampling; } set { this.chromaSubsampling = value; } }

	protected bool videoFullRangeFlag; 
	public bool VideoFullRangeFlag { get { return this.videoFullRangeFlag; } set { this.videoFullRangeFlag = value; } }

	protected byte colourPrimaries; 
	public byte ColourPrimaries { get { return this.colourPrimaries; } set { this.colourPrimaries = value; } }

	protected byte transferCharacteristics; 
	public byte TransferCharacteristics { get { return this.transferCharacteristics; } set { this.transferCharacteristics = value; } }

	protected byte matrixCoefficients; 
	public byte MatrixCoefficients { get { return this.matrixCoefficients; } set { this.matrixCoefficients = value; } }

	protected ushort codecInitializationData; 
	public ushort CodecInitializationData { get { return this.codecInitializationData; } set { this.codecInitializationData = value; } }

	public VPCodecConfigurationBox(): base(IsoStream.FromFourCC("vpcC"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.profile, "profile"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.level, "level"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.bitDepth, "bitDepth"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.chromaSubsampling, "chromaSubsampling"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.videoFullRangeFlag, "videoFullRangeFlag"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.colourPrimaries, "colourPrimaries"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.transferCharacteristics, "transferCharacteristics"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.matrixCoefficients, "matrixCoefficients"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.codecInitializationData, "codecInitializationData"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.profile, "profile"); 
		boxSize += stream.WriteUInt8( this.level, "level"); 
		boxSize += stream.WriteBits(4,  this.bitDepth, "bitDepth"); 
		boxSize += stream.WriteBits(3,  this.chromaSubsampling, "chromaSubsampling"); 
		boxSize += stream.WriteBit( this.videoFullRangeFlag, "videoFullRangeFlag"); 
		boxSize += stream.WriteUInt8( this.colourPrimaries, "colourPrimaries"); 
		boxSize += stream.WriteUInt8( this.transferCharacteristics, "transferCharacteristics"); 
		boxSize += stream.WriteUInt8( this.matrixCoefficients, "matrixCoefficients"); 
		boxSize += stream.WriteUInt16( this.codecInitializationData, "codecInitializationData"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // profile
		boxSize += 8; // level
		boxSize += 4; // bitDepth
		boxSize += 3; // chromaSubsampling
		boxSize += 1; // videoFullRangeFlag
		boxSize += 8; // colourPrimaries
		boxSize += 8; // transferCharacteristics
		boxSize += 8; // matrixCoefficients
		boxSize += 16; // codecInitializationData
		return boxSize;
	}
}

}
