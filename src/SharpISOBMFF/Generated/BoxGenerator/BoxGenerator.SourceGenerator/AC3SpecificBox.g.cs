using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// ETSI TS 102 366 F.4, as GPAC's gf_odf_ac3_cfg_parse_bs and FFmpeg's mov_read_dac3 read it
aligned(8) class AC3SpecificBox() extends Box('dac3') {
	unsigned int(2) fscod;
	unsigned int(5) bsid;
	unsigned int(3) bsmod;
	unsigned int(3) acmod;
	unsigned int(1) lfeon;
	unsigned int(5) bit_rate_code;
	bit(5) reserved = 0;
}
*/
public partial class AC3SpecificBox : Box
{
	public const string TYPE = "dac3";
	public override string DisplayName { get { return "AC3SpecificBox"; } }

	protected byte fscod; 
	public byte Fscod { get { return this.fscod; } set { this.fscod = value; } }

	protected byte bsid; 
	public byte Bsid { get { return this.bsid; } set { this.bsid = value; } }

	protected byte bsmod; 
	public byte Bsmod { get { return this.bsmod; } set { this.bsmod = value; } }

	protected byte acmod; 
	public byte Acmod { get { return this.acmod; } set { this.acmod = value; } }

	protected bool lfeon; 
	public bool Lfeon { get { return this.lfeon; } set { this.lfeon = value; } }

	protected byte bit_rate_code; 
	public byte BitRateCode { get { return this.bit_rate_code; } set { this.bit_rate_code = value; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	public AC3SpecificBox(): base(IsoStream.FromFourCC("dac3"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.fscod, "fscod"); 
		boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.bsid, "bsid"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.bsmod, "bsmod"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.acmod, "acmod"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.lfeon, "lfeon"); 
		boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.bit_rate_code, "bit_rate_code"); 
		boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(2,  this.fscod, "fscod"); 
		boxSize += stream.WriteBits(5,  this.bsid, "bsid"); 
		boxSize += stream.WriteBits(3,  this.bsmod, "bsmod"); 
		boxSize += stream.WriteBits(3,  this.acmod, "acmod"); 
		boxSize += stream.WriteBit( this.lfeon, "lfeon"); 
		boxSize += stream.WriteBits(5,  this.bit_rate_code, "bit_rate_code"); 
		boxSize += stream.WriteBits(5,  this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 2; // fscod
		boxSize += 5; // bsid
		boxSize += 3; // bsmod
		boxSize += 3; // acmod
		boxSize += 1; // lfeon
		boxSize += 5; // bit_rate_code
		boxSize += 5; // reserved
		return boxSize;
	}
}

}
