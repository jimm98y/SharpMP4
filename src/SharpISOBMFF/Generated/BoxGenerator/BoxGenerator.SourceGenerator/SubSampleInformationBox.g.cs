using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SubSampleInformationBox
	extends FullBox('subs', version, flags) {
	unsigned int(32) entry_count;
	int i,j;
	for (i=0; i < entry_count; i++) {
		unsigned int(32) sample_delta;
		unsigned int(16) subsample_count;
		if (subsample_count > 0) {
			for (j=0; j < subsample_count; j++) {
				if(version == 1)
				{
					unsigned int(32) subsample_size;
				}
				else
				{
					unsigned int(16) subsample_size;
				}
				unsigned int(8) subsample_priority;
				unsigned int(8) discardable;
				unsigned int(32) codec_specific_parameters;
			}
		}
	}
}
*/
public partial class SubSampleInformationBox : FullBox
{
	public const string TYPE = "subs";
	public override string DisplayName { get { return "SubSampleInformationBox"; } }

	protected uint entry_count; 
	public uint EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	protected uint[] sample_delta; 
	public uint[] SampleDelta { get { return this.sample_delta; } set { this.sample_delta = value; } }

	protected ushort[] subsample_count; 
	public ushort[] SubsampleCount { get { return this.subsample_count; } set { this.subsample_count = value; } }

	protected uint[][] subsample_size; 
	public uint[][] SubsampleSize { get { return this.subsample_size; } set { this.subsample_size = value; } }

	protected byte[][] subsample_priority; 
	public byte[][] SubsamplePriority { get { return this.subsample_priority; } set { this.subsample_priority = value; } }

	protected byte[][] discardable; 
	public byte[][] Discardable { get { return this.discardable; } set { this.discardable = value; } }

	protected uint[][] codec_specific_parameters; 
	public uint[][] CodecSpecificParameters { get { return this.codec_specific_parameters; } set { this.codec_specific_parameters = value; } }

	public SubSampleInformationBox(byte version = 0, uint flags = 0): base(IsoStream.FromFourCC("subs"), version, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entry_count, "entry_count"); 
		

		this.sample_delta = new uint[IsoStream.GetInt( entry_count)];
		this.subsample_count = new ushort[IsoStream.GetInt( entry_count)];
		this.subsample_size = new uint[IsoStream.GetInt( entry_count)][];
		this.subsample_priority = new byte[IsoStream.GetInt( entry_count)][];
		this.discardable = new byte[IsoStream.GetInt( entry_count)][];
		this.codec_specific_parameters = new uint[IsoStream.GetInt( entry_count)][];
		for (int i=0; i < entry_count; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_delta[i], "sample_delta"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.subsample_count[i], "subsample_count"); 

			if (subsample_count[i] > 0)
			{

				this.subsample_size[i] = new uint[IsoStream.GetInt( subsample_count[i])];
				this.subsample_priority[i] = new byte[IsoStream.GetInt( subsample_count[i])];
				this.discardable[i] = new byte[IsoStream.GetInt( subsample_count[i])];
				this.codec_specific_parameters[i] = new uint[IsoStream.GetInt( subsample_count[i])];
				for (int j=0; j < subsample_count[i]; j++)
				{

					if (version == 1)
					{
						boxSize += stream.ReadUInt32(boxSize, readSize,  out this.subsample_size[i][j], "subsample_size"); 
					}

					else 
					{
						boxSize += stream.ReadUInt16(boxSize, readSize,  out this.subsample_size[i][j], "subsample_size"); 
					}
					boxSize += stream.ReadUInt8(boxSize, readSize,  out this.subsample_priority[i][j], "subsample_priority"); 
					boxSize += stream.ReadUInt8(boxSize, readSize,  out this.discardable[i][j], "discardable"); 
					boxSize += stream.ReadUInt32(boxSize, readSize,  out this.codec_specific_parameters[i][j], "codec_specific_parameters"); 
				}
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.entry_count, "entry_count"); 
		

		for (int i=0; i < entry_count; i++)
		{
			boxSize += stream.WriteUInt32( this.sample_delta[i], "sample_delta"); 
			boxSize += stream.WriteUInt16( this.subsample_count[i], "subsample_count"); 

			if (subsample_count[i] > 0)
			{

				for (int j=0; j < subsample_count[i]; j++)
				{

					if (version == 1)
					{
						boxSize += stream.WriteUInt32( this.subsample_size[i][j], "subsample_size"); 
					}

					else 
					{
						boxSize += stream.WriteUInt16( this.subsample_size[i][j], "subsample_size"); 
					}
					boxSize += stream.WriteUInt8( this.subsample_priority[i][j], "subsample_priority"); 
					boxSize += stream.WriteUInt8( this.discardable[i][j], "discardable"); 
					boxSize += stream.WriteUInt32( this.codec_specific_parameters[i][j], "codec_specific_parameters"); 
				}
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // entry_count
		

		for (int i=0; i < entry_count; i++)
		{
			boxSize += 32; // sample_delta
			boxSize += 16; // subsample_count

			if (subsample_count[i] > 0)
			{

				for (int j=0; j < subsample_count[i]; j++)
				{

					if (version == 1)
					{
						boxSize += 32; // subsample_size
					}

					else 
					{
						boxSize += 16; // subsample_size
					}
					boxSize += 8; // subsample_priority
					boxSize += 8; // discardable
					boxSize += 32; // codec_specific_parameters
				}
			}
		}
		return boxSize;
	}
}

}
