using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class DownMixInstructions extends FullBox('dmix', version, flags=0) {
	if (version >= 1) {
		bit(1) reserved = 0;
		bit(7) downmix_instructions_count;
	} else {
		int downmix_instructions_count = 1;
	}
	for (a=1; a<=downmix_instructions_count; a++) { 
		unsigned int(8) targetLayout;
 		unsigned int(1) reserved = 0;
		unsigned int(7) targetChannelCount;
		bit(1) in_stream; 
		unsigned int(7) downmix_ID;
		if (in_stream==0) 
		{	// downmix coefficients are out of stream and supplied here
			int i, j;
			if (version >= 1) {
				bit(4) bs_downmix_offset;
				int size = 4;
				for (i=1; i <= targetChannelCount; i++){
					for (j=1; j <= baseChannelCount; j++) {
						bit(5) bs_downmix_coefficient_v1;
						size += 5;
					}
				}
				bit(8 ceil(size / 8) – size) reserved = 0; // byte align
			} else {
				for (i=1; i <= targetChannelCount; i++){
					for (j=1; j <= baseChannelCount; j++) {
						bit(4) bs_downmix_coefficient;
					}
				}
			}
		}
	}
}
*/
public partial class DownMixInstructions : FullBox
{
	public const string TYPE = "dmix";
	public override string DisplayName { get { return "DownMixInstructions"; } }

	protected bool reserved = false; 
	public bool Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte downmix_instructions_count; 
	public byte DownmixInstructionsCount { get { return this.downmix_instructions_count; } set { this.downmix_instructions_count = value; } }

	protected byte[] targetLayout; 
	public byte[] TargetLayout { get { return this.targetLayout; } set { this.targetLayout = value; } }

	protected bool[] reserved0; 
	public bool[] Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected byte[] targetChannelCount; 
	public byte[] TargetChannelCount { get { return this.targetChannelCount; } set { this.targetChannelCount = value; } }

	protected bool[] in_stream; 
	public bool[] InStream { get { return this.in_stream; } set { this.in_stream = value; } }

	protected byte[] downmix_ID; 
	public byte[] DownmixID { get { return this.downmix_ID; } set { this.downmix_ID = value; } }

	protected byte[] bs_downmix_offset; 
	public byte[] BsDownmixOffset { get { return this.bs_downmix_offset; } set { this.bs_downmix_offset = value; } }

	protected byte[][][] bs_downmix_coefficient_v1; 
	public byte[][][] BsDownmixCoefficientV1 { get { return this.bs_downmix_coefficient_v1; } set { this.bs_downmix_coefficient_v1 = value; } }

	protected byte[][] reserved00;  //  byte align
	public byte[][] Reserved00 { get { return this.reserved00; } set { this.reserved00 = value; } }

	protected byte[][][] bs_downmix_coefficient; 
	public byte[][][] BsDownmixCoefficient { get { return this.bs_downmix_coefficient; } set { this.bs_downmix_coefficient = value; } }

	public DownMixInstructions(byte version = 0): base(IsoStream.FromFourCC("dmix"), version, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		int baseChannelCount = 0; // TODO: get somewhere

		if (version >= 1)
		{
			boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved, "reserved"); 
			boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.downmix_instructions_count, "downmix_instructions_count"); 
		}

		else 
		{
			downmix_instructions_count = 1;
		}

		this.targetLayout = new byte[IsoStream.GetInt(downmix_instructions_count)];
		this.reserved0 = new bool[IsoStream.GetInt(downmix_instructions_count)];
		this.targetChannelCount = new byte[IsoStream.GetInt(downmix_instructions_count)];
		this.in_stream = new bool[IsoStream.GetInt(downmix_instructions_count)];
		this.downmix_ID = new byte[IsoStream.GetInt(downmix_instructions_count)];
		this.bs_downmix_offset = new byte[IsoStream.GetInt(downmix_instructions_count)];
		this.bs_downmix_coefficient_v1 = new byte[IsoStream.GetInt(downmix_instructions_count)][][];
		this.reserved00 = new byte[IsoStream.GetInt(downmix_instructions_count)][];
		this.bs_downmix_coefficient = new byte[IsoStream.GetInt(downmix_instructions_count)][][];
		for (int a=0; a<downmix_instructions_count; a++)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.targetLayout[a], "targetLayout"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved0[a], "reserved0"); 
			boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.targetChannelCount[a], "targetChannelCount"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.in_stream[a], "in_stream"); 
			boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.downmix_ID[a], "downmix_ID"); 

			if (in_stream[a]== false)
			{
				/*  downmix coefficients are out of stream and supplied here */
				

				if (version >= 1)
				{
					boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.bs_downmix_offset[a], "bs_downmix_offset"); 
					int size = 4;

					this.bs_downmix_coefficient_v1[a] = new byte[IsoStream.GetInt( targetChannelCount[a])][];
					for (int i=0; i < targetChannelCount[a]; i++)
					{

						this.bs_downmix_coefficient_v1[a][i] = new byte[IsoStream.GetInt( baseChannelCount)];
						for (int j=0; j < baseChannelCount; j++)
						{
							boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.bs_downmix_coefficient_v1[a][i][j], "bs_downmix_coefficient_v1"); 
							size += 5;
						}
					}
					boxSize += stream.ReadBits(boxSize, readSize, (uint)((Math.Ceiling(size / 8d) - size) * 8 ),  out this.reserved00[a], "reserved00"); // byte align
				}

				else 
				{

					this.bs_downmix_coefficient[a] = new byte[IsoStream.GetInt( targetChannelCount[a])][];
					for (int i=0; i < targetChannelCount[a]; i++)
					{

						this.bs_downmix_coefficient[a][i] = new byte[IsoStream.GetInt( baseChannelCount)];
						for (int j=0; j < baseChannelCount; j++)
						{
							boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.bs_downmix_coefficient[a][i][j], "bs_downmix_coefficient"); 
						}
					}
				}
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		int baseChannelCount = 0; // TODO: get somewhere

		if (version >= 1)
		{
			boxSize += stream.WriteBit( this.reserved, "reserved"); 
			boxSize += stream.WriteBits(7,  this.downmix_instructions_count, "downmix_instructions_count"); 
		}

		else 
		{
			downmix_instructions_count = 1;
		}

		for (int a=0; a<downmix_instructions_count; a++)
		{
			boxSize += stream.WriteUInt8( this.targetLayout[a], "targetLayout"); 
			boxSize += stream.WriteBit( this.reserved0[a], "reserved0"); 
			boxSize += stream.WriteBits(7,  this.targetChannelCount[a], "targetChannelCount"); 
			boxSize += stream.WriteBit( this.in_stream[a], "in_stream"); 
			boxSize += stream.WriteBits(7,  this.downmix_ID[a], "downmix_ID"); 

			if (in_stream[a]== false)
			{
				/*  downmix coefficients are out of stream and supplied here */
				

				if (version >= 1)
				{
					boxSize += stream.WriteBits(4,  this.bs_downmix_offset[a], "bs_downmix_offset"); 
					int size = 4;

					for (int i=0; i < targetChannelCount[a]; i++)
					{

						for (int j=0; j < baseChannelCount; j++)
						{
							boxSize += stream.WriteBits(5,  this.bs_downmix_coefficient_v1[a][i][j], "bs_downmix_coefficient_v1"); 
							size += 5;
						}
					}
					boxSize += stream.WriteBits((uint)((Math.Ceiling(size / 8d) - size) * 8 ),  this.reserved00[a], "reserved00"); // byte align
				}

				else 
				{

					for (int i=0; i < targetChannelCount[a]; i++)
					{

						for (int j=0; j < baseChannelCount; j++)
						{
							boxSize += stream.WriteBits(4,  this.bs_downmix_coefficient[a][i][j], "bs_downmix_coefficient"); 
						}
					}
				}
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		int baseChannelCount = 0; // TODO: get somewhere

		if (version >= 1)
		{
			boxSize += 1; // reserved
			boxSize += 7; // downmix_instructions_count
		}

		else 
		{
			downmix_instructions_count = 1;
		}

		for (int a=0; a<downmix_instructions_count; a++)
		{
			boxSize += 8; // targetLayout
			boxSize += 1; // reserved0
			boxSize += 7; // targetChannelCount
			boxSize += 1; // in_stream
			boxSize += 7; // downmix_ID

			if (in_stream[a]== false)
			{
				/*  downmix coefficients are out of stream and supplied here */
				

				if (version >= 1)
				{
					boxSize += 4; // bs_downmix_offset
					int size = 4;

					for (int i=0; i < targetChannelCount[a]; i++)
					{

						for (int j=0; j < baseChannelCount; j++)
						{
							boxSize += 5; // bs_downmix_coefficient_v1
							size += 5;
						}
					}
					boxSize += (ulong)((Math.Ceiling(size / 8d) - size) * 8 ); // reserved00
				}

				else 
				{

					for (int i=0; i < targetChannelCount[a]; i++)
					{

						for (int j=0; j < baseChannelCount; j++)
						{
							boxSize += 4; // bs_downmix_coefficient
						}
					}
				}
			}
		}
		return boxSize;
	}
}

}
