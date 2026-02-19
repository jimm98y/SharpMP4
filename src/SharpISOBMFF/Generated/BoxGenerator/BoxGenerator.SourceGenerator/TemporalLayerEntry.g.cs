using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class TemporalLayerEntry() extends VisualSampleGroupEntry ('tscl')
{
	unsigned int(8)  temporalLayerId;
	unsigned int(2)  tlprofile_space;
	unsigned int(1)  tltier_flag;
	unsigned int(5)  tlprofile_idc;
	unsigned int(32) tlprofile_compatibility_flags;
	unsigned int(48) tlconstraint_indicator_flags;
	unsigned int(8)  tllevel_idc;
	unsigned int(16) tlMaxBitRate;
	unsigned int(16) tlAvgBitRate;
	unsigned int(8)  tlConstantFrameRate;
	unsigned int(16) tlAvgFrameRate;
}
*/
public partial class TemporalLayerEntry : VisualSampleGroupEntry
{
	public const string TYPE = "tscl";
	public override string DisplayName { get { return "TemporalLayerEntry"; } }

	protected byte temporalLayerId; 
	public byte TemporalLayerId { get { return this.temporalLayerId; } set { this.temporalLayerId = value; } }

	protected byte tlprofile_space; 
	public byte TlprofileSpace { get { return this.tlprofile_space; } set { this.tlprofile_space = value; } }

	protected bool tltier_flag; 
	public bool TltierFlag { get { return this.tltier_flag; } set { this.tltier_flag = value; } }

	protected byte tlprofile_idc; 
	public byte TlprofileIdc { get { return this.tlprofile_idc; } set { this.tlprofile_idc = value; } }

	protected uint tlprofile_compatibility_flags; 
	public uint TlprofileCompatibilityFlags { get { return this.tlprofile_compatibility_flags; } set { this.tlprofile_compatibility_flags = value; } }

	protected ulong tlconstraint_indicator_flags; 
	public ulong TlconstraintIndicatorFlags { get { return this.tlconstraint_indicator_flags; } set { this.tlconstraint_indicator_flags = value; } }

	protected byte tllevel_idc; 
	public byte TllevelIdc { get { return this.tllevel_idc; } set { this.tllevel_idc = value; } }

	protected ushort tlMaxBitRate; 
	public ushort TlMaxBitRate { get { return this.tlMaxBitRate; } set { this.tlMaxBitRate = value; } }

	protected ushort tlAvgBitRate; 
	public ushort TlAvgBitRate { get { return this.tlAvgBitRate; } set { this.tlAvgBitRate = value; } }

	protected byte tlConstantFrameRate; 
	public byte TlConstantFrameRate { get { return this.tlConstantFrameRate; } set { this.tlConstantFrameRate = value; } }

	protected ushort tlAvgFrameRate; 
	public ushort TlAvgFrameRate { get { return this.tlAvgFrameRate; } set { this.tlAvgFrameRate = value; } }

	public TemporalLayerEntry(): base(IsoStream.FromFourCC("tscl"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.temporalLayerId, "temporalLayerId"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.tlprofile_space, "tlprofile_space"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.tltier_flag, "tltier_flag"); 
		boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.tlprofile_idc, "tlprofile_idc"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.tlprofile_compatibility_flags, "tlprofile_compatibility_flags"); 
		boxSize += stream.ReadUInt48(boxSize, readSize,  out this.tlconstraint_indicator_flags, "tlconstraint_indicator_flags"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.tllevel_idc, "tllevel_idc"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.tlMaxBitRate, "tlMaxBitRate"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.tlAvgBitRate, "tlAvgBitRate"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.tlConstantFrameRate, "tlConstantFrameRate"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.tlAvgFrameRate, "tlAvgFrameRate"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.temporalLayerId, "temporalLayerId"); 
		boxSize += stream.WriteBits(2,  this.tlprofile_space, "tlprofile_space"); 
		boxSize += stream.WriteBit( this.tltier_flag, "tltier_flag"); 
		boxSize += stream.WriteBits(5,  this.tlprofile_idc, "tlprofile_idc"); 
		boxSize += stream.WriteUInt32( this.tlprofile_compatibility_flags, "tlprofile_compatibility_flags"); 
		boxSize += stream.WriteUInt48( this.tlconstraint_indicator_flags, "tlconstraint_indicator_flags"); 
		boxSize += stream.WriteUInt8( this.tllevel_idc, "tllevel_idc"); 
		boxSize += stream.WriteUInt16( this.tlMaxBitRate, "tlMaxBitRate"); 
		boxSize += stream.WriteUInt16( this.tlAvgBitRate, "tlAvgBitRate"); 
		boxSize += stream.WriteUInt8( this.tlConstantFrameRate, "tlConstantFrameRate"); 
		boxSize += stream.WriteUInt16( this.tlAvgFrameRate, "tlAvgFrameRate"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // temporalLayerId
		boxSize += 2; // tlprofile_space
		boxSize += 1; // tltier_flag
		boxSize += 5; // tlprofile_idc
		boxSize += 32; // tlprofile_compatibility_flags
		boxSize += 48; // tlconstraint_indicator_flags
		boxSize += 8; // tllevel_idc
		boxSize += 16; // tlMaxBitRate
		boxSize += 16; // tlAvgBitRate
		boxSize += 8; // tlConstantFrameRate
		boxSize += 16; // tlAvgFrameRate
		return boxSize;
	}
}

}
