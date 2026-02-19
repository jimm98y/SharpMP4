using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class HintMediaHeaderBox
	extends FullBox('hmhd', version = 0, 0) {
	unsigned int(16)	maxPDUsize;
	unsigned int(16)	avgPDUsize;
	unsigned int(32)	maxbitrate;
	unsigned int(32)	avgbitrate;
	unsigned int(32)	reserved = 0;
}
*/
public partial class HintMediaHeaderBox : FullBox
{
	public const string TYPE = "hmhd";
	public override string DisplayName { get { return "HintMediaHeaderBox"; } }

	protected ushort maxPDUsize; 
	public ushort MaxPDUsize { get { return this.maxPDUsize; } set { this.maxPDUsize = value; } }

	protected ushort avgPDUsize; 
	public ushort AvgPDUsize { get { return this.avgPDUsize; } set { this.avgPDUsize = value; } }

	protected uint maxbitrate; 
	public uint Maxbitrate { get { return this.maxbitrate; } set { this.maxbitrate = value; } }

	protected uint avgbitrate; 
	public uint Avgbitrate { get { return this.avgbitrate; } set { this.avgbitrate = value; } }

	protected uint reserved = 0; 
	public uint Reserved { get { return this.reserved; } set { this.reserved = value; } }

	public HintMediaHeaderBox(): base(IsoStream.FromFourCC("hmhd"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.maxPDUsize, "maxPDUsize"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.avgPDUsize, "avgPDUsize"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.maxbitrate, "maxbitrate"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.avgbitrate, "avgbitrate"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.maxPDUsize, "maxPDUsize"); 
		boxSize += stream.WriteUInt16( this.avgPDUsize, "avgPDUsize"); 
		boxSize += stream.WriteUInt32( this.maxbitrate, "maxbitrate"); 
		boxSize += stream.WriteUInt32( this.avgbitrate, "avgbitrate"); 
		boxSize += stream.WriteUInt32( this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // maxPDUsize
		boxSize += 16; // avgPDUsize
		boxSize += 32; // maxbitrate
		boxSize += 32; // avgbitrate
		boxSize += 32; // reserved
		return boxSize;
	}
}

}
