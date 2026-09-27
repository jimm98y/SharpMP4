using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class DvvCBox() extends Box('dvvC') {
 unsigned int(8) dv_version_major;
 unsigned int(8) dv_version_minor;
 unsigned int(7) dv_profile;
 unsigned int(6) dv_level;
 bit(1) rpu_present_flag;
 bit(1) el_present_flag;
 bit(1) bl_present_flag;
 unsigned int(4) dv_bl_signal_compatibility_id;
 const unsigned int(28) reserved = 0;
 const unsigned int(32)[4] reserved = 0;
 } 
*/
public partial class DvvCBox : Box
{
	public const string TYPE = "dvvC";
	public override string DisplayName { get { return "DvvCBox"; } }

	protected byte dv_version_major; 
	public byte DvVersionMajor { get { return this.dv_version_major; } set { this.dv_version_major = value; } }

	protected byte dv_version_minor; 
	public byte DvVersionMinor { get { return this.dv_version_minor; } set { this.dv_version_minor = value; } }

	protected byte dv_profile; 
	public byte DvProfile { get { return this.dv_profile; } set { this.dv_profile = value; } }

	protected byte dv_level; 
	public byte DvLevel { get { return this.dv_level; } set { this.dv_level = value; } }

	protected bool rpu_present_flag; 
	public bool RpuPresentFlag { get { return this.rpu_present_flag; } set { this.rpu_present_flag = value; } }

	protected bool el_present_flag; 
	public bool ElPresentFlag { get { return this.el_present_flag; } set { this.el_present_flag = value; } }

	protected bool bl_present_flag; 
	public bool BlPresentFlag { get { return this.bl_present_flag; } set { this.bl_present_flag = value; } }

	protected byte dv_bl_signal_compatibility_id; 
	public byte DvBlSignalCompatibilityId { get { return this.dv_bl_signal_compatibility_id; } set { this.dv_bl_signal_compatibility_id = value; } }

	protected uint reserved = 0; 
	public uint Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected uint[] reserved0 = []; 
	public uint[] Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	public DvvCBox(): base(IsoStream.FromFourCC("dvvC"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.dv_version_major, "dv_version_major"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.dv_version_minor, "dv_version_minor"); 
		boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.dv_profile, "dv_profile"); 
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.dv_level, "dv_level"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.rpu_present_flag, "rpu_present_flag"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.el_present_flag, "el_present_flag"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.bl_present_flag, "bl_present_flag"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.dv_bl_signal_compatibility_id, "dv_bl_signal_compatibility_id"); 
		boxSize += stream.ReadBits(boxSize, readSize, 28,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt32Array(boxSize, readSize, 4,  out this.reserved0, "reserved0"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.dv_version_major, "dv_version_major"); 
		boxSize += stream.WriteUInt8( this.dv_version_minor, "dv_version_minor"); 
		boxSize += stream.WriteBits(7,  this.dv_profile, "dv_profile"); 
		boxSize += stream.WriteBits(6,  this.dv_level, "dv_level"); 
		boxSize += stream.WriteBit( this.rpu_present_flag, "rpu_present_flag"); 
		boxSize += stream.WriteBit( this.el_present_flag, "el_present_flag"); 
		boxSize += stream.WriteBit( this.bl_present_flag, "bl_present_flag"); 
		boxSize += stream.WriteBits(4,  this.dv_bl_signal_compatibility_id, "dv_bl_signal_compatibility_id"); 
		boxSize += stream.WriteBits(28,  this.reserved, "reserved"); 
		boxSize += stream.WriteUInt32Array(4,  this.reserved0, "reserved0"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // dv_version_major
		boxSize += 8; // dv_version_minor
		boxSize += 7; // dv_profile
		boxSize += 6; // dv_level
		boxSize += 1; // rpu_present_flag
		boxSize += 1; // el_present_flag
		boxSize += 1; // bl_present_flag
		boxSize += 4; // dv_bl_signal_compatibility_id
		boxSize += 28; // reserved
		boxSize += 4 * 32; // reserved0
		return boxSize;
	}
}

}
