using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

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
   unsigned int(8) sample_data[]; // each sample's IV and subsamples, the IV of the size above where the flags give one, else of the track's
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

	protected byte[] sample_data;  //  each sample's IV and subsamples, the IV of the size above where the flags give one, else of the track's
	public byte[] SampleData { get { return this.sample_data; } set { this.sample_data = value; } }

	public PiffSampleEncryptionBox(byte version = 0, uint flags = 0): base(IsoStream.FromFourCC("uuid"), version, flags)
	{
		this.uuid = ConvertEx.FromHexString("a2394f525a9b4f14a2446c427c648df4");
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
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.sample_data, "sample_data"); // each sample's IV and subsamples, the IV of the size above where the flags give one, else of the track's
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
		boxSize += stream.WriteUInt8ArrayTillEnd( this.sample_data, "sample_data"); // each sample's IV and subsamples, the IV of the size above where the flags give one, else of the track's
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
		boxSize += ((ulong)sample_data.Length * 8); // sample_data
		return boxSize;
	}
}

}
