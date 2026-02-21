using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class PaddingBitsBox extends FullBox('padb', version = 0, 0) {
	unsigned int(32)	sample_count;
	int i;
	for (i=0; i < floor((sample_count + 1)/2); i++) {
		bit(1)	reserved = 0;
		bit(3)	pad1;
		bit(1)	reserved = 0;
		bit(3)	pad2;
	}
}
*/
public partial class PaddingBitsBox : FullBox
{
	public const string TYPE = "padb";
	public override string DisplayName { get { return "PaddingBitsBox"; } }

	protected uint sample_count; 
	public uint SampleCount { get { return this.sample_count; } set { this.sample_count = value; } }

	protected bool[] reserved; 
	public bool[] Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte[] pad1; 
	public byte[] Pad1 { get { return this.pad1; } set { this.pad1 = value; } }

	protected bool[] reserved0; 
	public bool[] Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected byte[] pad2; 
	public byte[] Pad2 { get { return this.pad2; } set { this.pad2 = value; } }

	public PaddingBitsBox(): base(IsoStream.FromFourCC("padb"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_count, "sample_count"); 
		

		this.reserved = new bool[IsoStream.GetInt( (int)Math.Floor((double)(sample_count + 1)/2))];
		this.pad1 = new byte[IsoStream.GetInt( (int)Math.Floor((double)(sample_count + 1)/2))];
		this.reserved0 = new bool[IsoStream.GetInt( (int)Math.Floor((double)(sample_count + 1)/2))];
		this.pad2 = new byte[IsoStream.GetInt( (int)Math.Floor((double)(sample_count + 1)/2))];
		for (int i=0; i < (int)Math.Floor((double)(sample_count + 1)/2); i++)
		{
			boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved[i], "reserved"); 
			boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.pad1[i], "pad1"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved0[i], "reserved0"); 
			boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.pad2[i], "pad2"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.sample_count, "sample_count"); 
		

		for (int i=0; i < (int)Math.Floor((double)(sample_count + 1)/2); i++)
		{
			boxSize += stream.WriteBit( this.reserved[i], "reserved"); 
			boxSize += stream.WriteBits(3,  this.pad1[i], "pad1"); 
			boxSize += stream.WriteBit( this.reserved0[i], "reserved0"); 
			boxSize += stream.WriteBits(3,  this.pad2[i], "pad2"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // sample_count
		

		for (int i=0; i < (int)Math.Floor((double)(sample_count + 1)/2); i++)
		{
			boxSize += 1; // reserved
			boxSize += 3; // pad1
			boxSize += 1; // reserved0
			boxSize += 3; // pad2
		}
		return boxSize;
	}
}

}
