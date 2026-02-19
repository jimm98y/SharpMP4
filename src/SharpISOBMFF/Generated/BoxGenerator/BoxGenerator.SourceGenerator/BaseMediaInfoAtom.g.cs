using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class BaseMediaInfoAtom() extends Box('gmin') {
 unsigned int(16) graphicsMode;
 unsigned int(16) opColorR;
 unsigned int(16) opColorG;
 unsigned int(16) opColorB;
 unsigned int(16) balance;
 unsigned int(16) reserved
 } 
*/
public partial class BaseMediaInfoAtom : Box
{
	public const string TYPE = "gmin";
	public override string DisplayName { get { return "BaseMediaInfoAtom"; } }

	protected ushort graphicsMode; 
	public ushort GraphicsMode { get { return this.graphicsMode; } set { this.graphicsMode = value; } }

	protected ushort opColorR; 
	public ushort OpColorR { get { return this.opColorR; } set { this.opColorR = value; } }

	protected ushort opColorG; 
	public ushort OpColorG { get { return this.opColorG; } set { this.opColorG = value; } }

	protected ushort opColorB; 
	public ushort OpColorB { get { return this.opColorB; } set { this.opColorB = value; } }

	protected ushort balance; 
	public ushort Balance { get { return this.balance; } set { this.balance = value; } }

	protected ushort reserved; 
	public ushort Reserved { get { return this.reserved; } set { this.reserved = value; } }

	public BaseMediaInfoAtom(): base(IsoStream.FromFourCC("gmin"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.graphicsMode, "graphicsMode"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.opColorR, "opColorR"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.opColorG, "opColorG"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.opColorB, "opColorB"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.balance, "balance"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.graphicsMode, "graphicsMode"); 
		boxSize += stream.WriteUInt16( this.opColorR, "opColorR"); 
		boxSize += stream.WriteUInt16( this.opColorG, "opColorG"); 
		boxSize += stream.WriteUInt16( this.opColorB, "opColorB"); 
		boxSize += stream.WriteUInt16( this.balance, "balance"); 
		boxSize += stream.WriteUInt16( this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // graphicsMode
		boxSize += 16; // opColorR
		boxSize += 16; // opColorG
		boxSize += 16; // opColorB
		boxSize += 16; // balance
		boxSize += 16; // reserved
		return boxSize;
	}
}

}
