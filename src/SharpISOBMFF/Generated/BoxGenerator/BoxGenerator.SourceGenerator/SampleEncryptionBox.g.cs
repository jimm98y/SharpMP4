using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SampleEncryptionBox extends FullBox('senc', version, flags)
{
   unsigned int(32)  sample_count;
   SampleEncryptionSample(version, flags, Per_Sample_IV_Size) samples[sample_count];
}


*/
public partial class SampleEncryptionBox : FullBox
{
	public const string TYPE = "senc";
	public override string DisplayName { get { return "SampleEncryptionBox"; } }

	protected uint sample_count; 
	public uint SampleCount { get { return this.sample_count; } set { this.sample_count = value; } }

	protected SampleEncryptionSample[] samples; 
	public SampleEncryptionSample[] Samples { get { return this.samples; } set { this.samples = value; } }

	protected byte Per_Sample_IV_Size  = 16; // TODO: get from the IsoStream.FromFourCC("tenc") box; 
	public byte PerSampleIVSize { get { return this.Per_Sample_IV_Size; } set { this.Per_Sample_IV_Size = value; } }

	public SampleEncryptionBox(byte version = 0, uint flags = 0): base(IsoStream.FromFourCC("senc"), version, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_count, "sample_count"); 
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(sample_count), () => new SampleEncryptionSample(version, flags, Per_Sample_IV_Size),  out this.samples, "samples"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.sample_count, "sample_count"); 
		boxSize += stream.WriteClass( this.samples, "samples"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // sample_count
		boxSize += IsoStream.CalculateClassSize(samples); // samples
		return boxSize;
	}
}

}
