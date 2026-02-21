using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SoundMediaHeaderBox
	extends FullBox('smhd', version = 0, 0) {
	template int(16) balance = 0;
	const unsigned int(16)	reserved = 0;
}
*/
public partial class SoundMediaHeaderBox : FullBox
{
	public const string TYPE = "smhd";
	public override string DisplayName { get { return "SoundMediaHeaderBox"; } }

	protected short balance = 0; 
	public short Balance { get { return this.balance; } set { this.balance = value; } }

	protected ushort reserved = 0; 
	public ushort Reserved { get { return this.reserved; } set { this.reserved = value; } }

	public SoundMediaHeaderBox(): base(IsoStream.FromFourCC("smhd"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.balance, "balance"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt16( this.balance, "balance"); 
		boxSize += stream.WriteUInt16( this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // balance
		boxSize += 16; // reserved
		return boxSize;
	}
}

}
