using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class DTSSpecificBox() extends Box('ddts') {
 unsigned int(32) dtsSamplingFrequency;
 unsigned int(32) maxBitRate;
 unsigned int(32) avgBitRate;
 unsigned int(8) pcmSampleDepth;
 bit(2) frameDuration;
 bit(5) streamConstruction;
 bit(1) coreLFEPresent;
 bit(6) coreLayout;
 bit(14) coreSize;
 bit(1) stereoDownmix;
 bit(3) representationType;
 bit(16) channelLayout;
 bit(1) multiAssetFlag;
 bit(1) lbrDurationMod;
 bit(1) reservedBoxPresent;
 bit(5) reserved;
 } 
*/
public partial class DTSSpecificBox : Box
{
	public const string TYPE = "ddts";
	public override string DisplayName { get { return "DTSSpecificBox"; } }

	protected uint dtsSamplingFrequency; 
	public uint DtsSamplingFrequency { get { return this.dtsSamplingFrequency; } set { this.dtsSamplingFrequency = value; } }

	protected uint maxBitRate; 
	public uint MaxBitRate { get { return this.maxBitRate; } set { this.maxBitRate = value; } }

	protected uint avgBitRate; 
	public uint AvgBitRate { get { return this.avgBitRate; } set { this.avgBitRate = value; } }

	protected byte pcmSampleDepth; 
	public byte PcmSampleDepth { get { return this.pcmSampleDepth; } set { this.pcmSampleDepth = value; } }

	protected byte frameDuration; 
	public byte FrameDuration { get { return this.frameDuration; } set { this.frameDuration = value; } }

	protected byte streamConstruction; 
	public byte StreamConstruction { get { return this.streamConstruction; } set { this.streamConstruction = value; } }

	protected bool coreLFEPresent; 
	public bool CoreLFEPresent { get { return this.coreLFEPresent; } set { this.coreLFEPresent = value; } }

	protected byte coreLayout; 
	public byte CoreLayout { get { return this.coreLayout; } set { this.coreLayout = value; } }

	protected ushort coreSize; 
	public ushort CoreSize { get { return this.coreSize; } set { this.coreSize = value; } }

	protected bool stereoDownmix; 
	public bool StereoDownmix { get { return this.stereoDownmix; } set { this.stereoDownmix = value; } }

	protected byte representationType; 
	public byte RepresentationType { get { return this.representationType; } set { this.representationType = value; } }

	protected ushort channelLayout; 
	public ushort ChannelLayout { get { return this.channelLayout; } set { this.channelLayout = value; } }

	protected bool multiAssetFlag; 
	public bool MultiAssetFlag { get { return this.multiAssetFlag; } set { this.multiAssetFlag = value; } }

	protected bool lbrDurationMod; 
	public bool LbrDurationMod { get { return this.lbrDurationMod; } set { this.lbrDurationMod = value; } }

	protected bool reservedBoxPresent; 
	public bool ReservedBoxPresent { get { return this.reservedBoxPresent; } set { this.reservedBoxPresent = value; } }

	protected byte reserved; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	public DTSSpecificBox(): base(IsoStream.FromFourCC("ddts"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.dtsSamplingFrequency, "dtsSamplingFrequency"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.maxBitRate, "maxBitRate"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.avgBitRate, "avgBitRate"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.pcmSampleDepth, "pcmSampleDepth"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.frameDuration, "frameDuration"); 
		boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.streamConstruction, "streamConstruction"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.coreLFEPresent, "coreLFEPresent"); 
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.coreLayout, "coreLayout"); 
		boxSize += stream.ReadBits(boxSize, readSize, 14,  out this.coreSize, "coreSize"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.stereoDownmix, "stereoDownmix"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.representationType, "representationType"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.channelLayout, "channelLayout"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.multiAssetFlag, "multiAssetFlag"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.lbrDurationMod, "lbrDurationMod"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.reservedBoxPresent, "reservedBoxPresent"); 
		boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.dtsSamplingFrequency, "dtsSamplingFrequency"); 
		boxSize += stream.WriteUInt32( this.maxBitRate, "maxBitRate"); 
		boxSize += stream.WriteUInt32( this.avgBitRate, "avgBitRate"); 
		boxSize += stream.WriteUInt8( this.pcmSampleDepth, "pcmSampleDepth"); 
		boxSize += stream.WriteBits(2,  this.frameDuration, "frameDuration"); 
		boxSize += stream.WriteBits(5,  this.streamConstruction, "streamConstruction"); 
		boxSize += stream.WriteBit( this.coreLFEPresent, "coreLFEPresent"); 
		boxSize += stream.WriteBits(6,  this.coreLayout, "coreLayout"); 
		boxSize += stream.WriteBits(14,  this.coreSize, "coreSize"); 
		boxSize += stream.WriteBit( this.stereoDownmix, "stereoDownmix"); 
		boxSize += stream.WriteBits(3,  this.representationType, "representationType"); 
		boxSize += stream.WriteUInt16( this.channelLayout, "channelLayout"); 
		boxSize += stream.WriteBit( this.multiAssetFlag, "multiAssetFlag"); 
		boxSize += stream.WriteBit( this.lbrDurationMod, "lbrDurationMod"); 
		boxSize += stream.WriteBit( this.reservedBoxPresent, "reservedBoxPresent"); 
		boxSize += stream.WriteBits(5,  this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // dtsSamplingFrequency
		boxSize += 32; // maxBitRate
		boxSize += 32; // avgBitRate
		boxSize += 8; // pcmSampleDepth
		boxSize += 2; // frameDuration
		boxSize += 5; // streamConstruction
		boxSize += 1; // coreLFEPresent
		boxSize += 6; // coreLayout
		boxSize += 14; // coreSize
		boxSize += 1; // stereoDownmix
		boxSize += 3; // representationType
		boxSize += 16; // channelLayout
		boxSize += 1; // multiAssetFlag
		boxSize += 1; // lbrDurationMod
		boxSize += 1; // reservedBoxPresent
		boxSize += 5; // reserved
		return boxSize;
	}
}

}
