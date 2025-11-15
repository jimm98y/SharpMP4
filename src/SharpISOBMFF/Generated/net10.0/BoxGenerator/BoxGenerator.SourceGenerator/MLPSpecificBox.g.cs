using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class MLPSpecificBox() extends Box('dmlp') {
 unsigned int(32) formatInfo;
 bit(15) peakDataRate;
 bit(1) reserved;
 unsigned int(32) reserved2; 
 } 
*/
public partial class MLPSpecificBox : Box
{
	public const string TYPE = "dmlp";
	public override string DisplayName { get { return "MLPSpecificBox"; } }

	protected uint formatInfo; 
	public uint FormatInfo { get { return this.formatInfo; } set { this.formatInfo = value; } }

	protected ushort peakDataRate; 
	public ushort PeakDataRate { get { return this.peakDataRate; } set { this.peakDataRate = value; } }

	protected bool reserved; 
	public bool Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected uint reserved2; 
	public uint Reserved2 { get { return this.reserved2; } set { this.reserved2 = value; } }

	public MLPSpecificBox(): base(IsoStream.FromFourCC("dmlp"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.formatInfo, "formatInfo"); 
		boxSize += stream.ReadBits(boxSize, readSize, 15,  out this.peakDataRate, "peakDataRate"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reserved2, "reserved2"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.formatInfo, "formatInfo"); 
		boxSize += stream.WriteBits(15,  this.peakDataRate, "peakDataRate"); 
		boxSize += stream.WriteBit( this.reserved, "reserved"); 
		boxSize += stream.WriteUInt32( this.reserved2, "reserved2"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // formatInfo
		boxSize += 15; // peakDataRate
		boxSize += 1; // reserved
		boxSize += 32; // reserved2
		return boxSize;
	}
}

}
