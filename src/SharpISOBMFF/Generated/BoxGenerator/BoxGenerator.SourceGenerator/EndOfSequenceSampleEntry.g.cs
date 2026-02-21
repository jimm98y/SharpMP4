using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class EndOfSequenceSampleEntry() extends VisualSampleGroupEntry ('eos ')
{
	unsigned int(8) num_eos_nal_unit_minus1;
	for (i=0; i <= num_eos_nal_unit_minus1; i++)
		bit(16) eosNalUnit[i];
}
*/
public partial class EndOfSequenceSampleEntry : VisualSampleGroupEntry
{
	public const string TYPE = "eos ";
	public override string DisplayName { get { return "EndOfSequenceSampleEntry"; } }

	protected byte num_eos_nal_unit_minus1; 
	public byte NumEosNalUnitMinus1 { get { return this.num_eos_nal_unit_minus1; } set { this.num_eos_nal_unit_minus1 = value; } }

	protected ushort[] eosNalUnit; 
	public ushort[] EosNalUnit { get { return this.eosNalUnit; } set { this.eosNalUnit = value; } }

	public EndOfSequenceSampleEntry(): base(IsoStream.FromFourCC("eos "))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.num_eos_nal_unit_minus1, "num_eos_nal_unit_minus1"); 

		this.eosNalUnit = new ushort[IsoStream.GetInt( num_eos_nal_unit_minus1 + 1)];
		for (int i=0; i <= num_eos_nal_unit_minus1; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.eosNalUnit[i], "eosNalUnit"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.num_eos_nal_unit_minus1, "num_eos_nal_unit_minus1"); 

		for (int i=0; i <= num_eos_nal_unit_minus1; i++)
		{
			boxSize += stream.WriteUInt16( this.eosNalUnit[i], "eosNalUnit"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // num_eos_nal_unit_minus1

		for (int i=0; i <= num_eos_nal_unit_minus1; i++)
		{
			boxSize += 16; // eosNalUnit
		}
		return boxSize;
	}
}

}
