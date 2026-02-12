using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class PiffSampleEncryptionBox() extends FullBox('uuid a2394f525a9b4f14a2446c427c648df4', version, flags) 
{
 if(flags & 0x1) {
 unsigned int(24) algorithmID;
 unsigned int(8) Per_Sample_IV_Size;
 unsigned int(8) kid[16];
 }
  unsigned int(32)  sample_count;
   SampleEncryptionSample(version, flags, Per_Sample_IV_Size) samples[sample_count];
}

*/
public partial class PiffSampleEncryptionBox : FullBox
{
	public const string TYPE = "uuid";
	public override string DisplayName { get { return "PiffSampleEncryptionBox"; } }

	protected uint algorithmID; 
	public uint AlgorithmID { get { return this.algorithmID; } set { this.algorithmID = value; } }

	protected byte Per_Sample_IV_Size; 
	public byte PerSampleIVSize { get { return this.Per_Sample_IV_Size; } set { this.Per_Sample_IV_Size = value; } }

	protected byte[] kid; 
	public byte[] Kid { get { return this.kid; } set { this.kid = value; } }

	protected uint sample_count; 
	public uint SampleCount { get { return this.sample_count; } set { this.sample_count = value; } }

	protected SampleEncryptionSample[] samples; 
	public SampleEncryptionSample[] Samples { get { return this.samples; } set { this.samples = value; } }

	public PiffSampleEncryptionBox(byte version = 0, uint flags = 0): base(IsoStream.FromFourCC("uuid"), version, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);

		if ((flags  &  0x1) ==  0x1)
		{
			boxSize += stream.ReadUInt24(boxSize, readSize,  out this.algorithmID, "algorithmID"); 
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.Per_Sample_IV_Size, "Per_Sample_IV_Size"); 
			boxSize += stream.ReadUInt8Array(boxSize, readSize, 16,  out this.kid, "kid"); 
		}
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_count, "sample_count"); 
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(sample_count), () => new SampleEncryptionSample(version, flags, Per_Sample_IV_Size),  out this.samples, "samples"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);

		if ((flags  &  0x1) ==  0x1)
		{
			boxSize += stream.WriteUInt24( this.algorithmID, "algorithmID"); 
			boxSize += stream.WriteUInt8( this.Per_Sample_IV_Size, "Per_Sample_IV_Size"); 
			boxSize += stream.WriteUInt8Array(16,  this.kid, "kid"); 
		}
		boxSize += stream.WriteUInt32( this.sample_count, "sample_count"); 
		boxSize += stream.WriteClass( this.samples, "samples"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();

		if ((flags  &  0x1) ==  0x1)
		{
			boxSize += 24; // algorithmID
			boxSize += 8; // Per_Sample_IV_Size
			boxSize += 16 * 8; // kid
		}
		boxSize += 32; // sample_count
		boxSize += IsoStream.CalculateClassSize(samples); // samples
		return boxSize;
	}
}

}
