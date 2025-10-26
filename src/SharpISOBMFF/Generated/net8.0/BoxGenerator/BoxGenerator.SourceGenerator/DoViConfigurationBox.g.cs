using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class DoViConfigurationBox() extends Box('dvcC') {
 unsigned int(8) dvVersionMajor;
 unsigned int(8) dvVersionMinor;
 unsigned int(16) profileLevelFlags;
 unsigned int(32) reserved1;
 unsigned int(32) reserved2;
unsigned int(32) reserved3;
unsigned int(32) reserved4;
 
 } 
*/
public partial class DoViConfigurationBox : Box
{
	public const string TYPE = "dvcC";
	public override string DisplayName { get { return "DoViConfigurationBox"; } }

	protected byte dvVersionMajor; 
	public byte DvVersionMajor { get { return this.dvVersionMajor; } set { this.dvVersionMajor = value; } }

	protected byte dvVersionMinor; 
	public byte DvVersionMinor { get { return this.dvVersionMinor; } set { this.dvVersionMinor = value; } }

	protected ushort profileLevelFlags; 
	public ushort ProfileLevelFlags { get { return this.profileLevelFlags; } set { this.profileLevelFlags = value; } }

	protected uint reserved1; 
	public uint Reserved1 { get { return this.reserved1; } set { this.reserved1 = value; } }

	protected uint reserved2; 
	public uint Reserved2 { get { return this.reserved2; } set { this.reserved2 = value; } }

	protected uint reserved3; 
	public uint Reserved3 { get { return this.reserved3; } set { this.reserved3 = value; } }

	protected uint reserved4; 
	public uint Reserved4 { get { return this.reserved4; } set { this.reserved4 = value; } }

	public DoViConfigurationBox(): base(IsoStream.FromFourCC("dvcC"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.dvVersionMajor, "dvVersionMajor"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.dvVersionMinor, "dvVersionMinor"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.profileLevelFlags, "profileLevelFlags"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reserved1, "reserved1"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reserved2, "reserved2"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reserved3, "reserved3"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reserved4, "reserved4"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.dvVersionMajor, "dvVersionMajor"); 
		boxSize += stream.WriteUInt8( this.dvVersionMinor, "dvVersionMinor"); 
		boxSize += stream.WriteUInt16( this.profileLevelFlags, "profileLevelFlags"); 
		boxSize += stream.WriteUInt32( this.reserved1, "reserved1"); 
		boxSize += stream.WriteUInt32( this.reserved2, "reserved2"); 
		boxSize += stream.WriteUInt32( this.reserved3, "reserved3"); 
		boxSize += stream.WriteUInt32( this.reserved4, "reserved4"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // dvVersionMajor
		boxSize += 8; // dvVersionMinor
		boxSize += 16; // profileLevelFlags
		boxSize += 32; // reserved1
		boxSize += 32; // reserved2
		boxSize += 32; // reserved3
		boxSize += 32; // reserved4
		return boxSize;
	}
}

}
