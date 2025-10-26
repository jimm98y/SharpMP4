using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class DirectReferenceSamplesList()
extends VisualSampleGroupEntry ('refs') {
	unsigned int(32) sample_id;
	unsigned int(8) num_direct_reference_samples;
	for(i = 0; i < num_direct_reference_samples; i++) {
		unsigned int(32)direct_reference_sample_id;
	}
}
*/
public partial class DirectReferenceSamplesList : VisualSampleGroupEntry
{
	public const string TYPE = "refs";
	public override string DisplayName { get { return "DirectReferenceSamplesList"; } }

	protected uint sample_id; 
	public uint SampleId { get { return this.sample_id; } set { this.sample_id = value; } }

	protected byte num_direct_reference_samples; 
	public byte NumDirectReferenceSamples { get { return this.num_direct_reference_samples; } set { this.num_direct_reference_samples = value; } }

	protected uint[] direct_reference_sample_id; 
	public uint[] DirectReferenceSampleId { get { return this.direct_reference_sample_id; } set { this.direct_reference_sample_id = value; } }

	public DirectReferenceSamplesList(): base(IsoStream.FromFourCC("refs"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_id, "sample_id"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.num_direct_reference_samples, "num_direct_reference_samples"); 

		this.direct_reference_sample_id = new uint[IsoStream.GetInt( num_direct_reference_samples)];
		for (int i = 0; i < num_direct_reference_samples; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.direct_reference_sample_id[i], "direct_reference_sample_id"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.sample_id, "sample_id"); 
		boxSize += stream.WriteUInt8( this.num_direct_reference_samples, "num_direct_reference_samples"); 

		for (int i = 0; i < num_direct_reference_samples; i++)
		{
			boxSize += stream.WriteUInt32( this.direct_reference_sample_id[i], "direct_reference_sample_id"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // sample_id
		boxSize += 8; // num_direct_reference_samples

		for (int i = 0; i < num_direct_reference_samples; i++)
		{
			boxSize += 32; // direct_reference_sample_id
		}
		return boxSize;
	}
}

}
