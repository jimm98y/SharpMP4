using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class DecodeRetimingEntry() extends VisualSampleGroupEntry ('dtrt') { 
	unsigned int(8) tierCount;
	for (i=1; i<=tierCount; i++) {
		unsigned int(16) tierID;
		signed int(16) delta;
	}
}
*/
public partial class DecodeRetimingEntry : VisualSampleGroupEntry
{
	public const string TYPE = "dtrt";
	public override string DisplayName { get { return "DecodeRetimingEntry"; } }

	protected byte tierCount; 
	public byte TierCount { get { return this.tierCount; } set { this.tierCount = value; } }

	protected ushort[] tierID; 
	public ushort[] TierID { get { return this.tierID; } set { this.tierID = value; } }

	protected short[] delta; 
	public short[] Delta { get { return this.delta; } set { this.delta = value; } }

	public DecodeRetimingEntry(): base(IsoStream.FromFourCC("dtrt"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.tierCount, "tierCount"); 

		this.tierID = new ushort[IsoStream.GetInt(tierCount)];
		this.delta = new short[IsoStream.GetInt(tierCount)];
		for (int i=0; i<tierCount; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.tierID[i], "tierID"); 
			boxSize += stream.ReadInt16(boxSize, readSize,  out this.delta[i], "delta"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.tierCount, "tierCount"); 

		for (int i=0; i<tierCount; i++)
		{
			boxSize += stream.WriteUInt16( this.tierID[i], "tierID"); 
			boxSize += stream.WriteInt16( this.delta[i], "delta"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // tierCount

		for (int i=0; i<tierCount; i++)
		{
			boxSize += 16; // tierID
			boxSize += 16; // delta
		}
		return boxSize;
	}
}

}
