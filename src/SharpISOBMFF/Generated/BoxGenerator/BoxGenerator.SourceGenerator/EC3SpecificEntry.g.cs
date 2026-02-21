using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class EC3SpecificEntry() {
 bit(2) fscod;
 bit(5) bsid;
 bit(5) bsmod;
 bit(3) acmod;
 bit(1) lfeon;
 bit(3) reserved;
 bit(4) numDepSub;
 if(numDepSub > 0) {
 bit(9) chanLoc;
 }
 else 
 {
 bit(1) reserved2;
 }
 }
 
*/
public partial class EC3SpecificEntry : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "EC3SpecificEntry"; } }

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

	protected byte reserved; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte numDepSub; 
	public byte NumDepSub { get { return this.numDepSub; } set { this.numDepSub = value; } }

	protected ushort chanLoc; 
	public ushort ChanLoc { get { return this.chanLoc; } set { this.chanLoc = value; } }

	protected bool reserved2; 
	public bool Reserved2 { get { return this.reserved2; } set { this.reserved2 = value; } }

	public EC3SpecificEntry(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.fscod, "fscod"); 
		boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.bsid, "bsid"); 
		boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.bsmod, "bsmod"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.acmod, "acmod"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.lfeon, "lfeon"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.numDepSub, "numDepSub"); 

		if (numDepSub > 0)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 9,  out this.chanLoc, "chanLoc"); 
		}

		else 
		{
			boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved2, "reserved2"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBits(2,  this.fscod, "fscod"); 
		boxSize += stream.WriteBits(5,  this.bsid, "bsid"); 
		boxSize += stream.WriteBits(5,  this.bsmod, "bsmod"); 
		boxSize += stream.WriteBits(3,  this.acmod, "acmod"); 
		boxSize += stream.WriteBit( this.lfeon, "lfeon"); 
		boxSize += stream.WriteBits(3,  this.reserved, "reserved"); 
		boxSize += stream.WriteBits(4,  this.numDepSub, "numDepSub"); 

		if (numDepSub > 0)
		{
			boxSize += stream.WriteBits(9,  this.chanLoc, "chanLoc"); 
		}

		else 
		{
			boxSize += stream.WriteBit( this.reserved2, "reserved2"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 2; // fscod
		boxSize += 5; // bsid
		boxSize += 5; // bsmod
		boxSize += 3; // acmod
		boxSize += 1; // lfeon
		boxSize += 3; // reserved
		boxSize += 4; // numDepSub

		if (numDepSub > 0)
		{
			boxSize += 9; // chanLoc
		}

		else 
		{
			boxSize += 1; // reserved2
		}
		return boxSize;
	}
}

}
