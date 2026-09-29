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
   unsigned int(8) sample_data[]; // each sample's IV and subsamples (23001-7 7.2), whose IV size is the track's - its 'tenc', or the sample's 'seig' group - and not in the box: SharpMP4's SampleEncryptionReader reads them
}
*/
public partial class SampleEncryptionBox : FullBox
{
	public const string TYPE = "senc";
	public override string DisplayName { get { return "SampleEncryptionBox"; } }

	protected uint sample_count; 
	public uint SampleCount { get { return this.sample_count; } set { this.sample_count = value; } }

	protected byte[] sample_data;  //  each sample's IV and subsamples (23001-7 7.2), whose IV size is the track's - its 'tenc', or the sample's 'seig' group - and not in the box: SharpMP4's SampleEncryptionReader reads them
	public byte[] SampleData { get { return this.sample_data; } set { this.sample_data = value; } }

	public SampleEncryptionBox(byte version = 0, uint flags = 0): base(IsoStream.FromFourCC("senc"), version, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_count, "sample_count"); 
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.sample_data, "sample_data"); // each sample's IV and subsamples (23001-7 7.2), whose IV size is the track's - its 'tenc', or the sample's 'seig' group - and not in the box: SharpMP4's SampleEncryptionReader reads them
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.sample_count, "sample_count"); 
		boxSize += stream.WriteUInt8ArrayTillEnd( this.sample_data, "sample_data"); // each sample's IV and subsamples (23001-7 7.2), whose IV size is the track's - its 'tenc', or the sample's 'seig' group - and not in the box: SharpMP4's SampleEncryptionReader reads them
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // sample_count
		boxSize += ((ulong)sample_data.Length * 8); // sample_data
		return boxSize;
	}
}

}
