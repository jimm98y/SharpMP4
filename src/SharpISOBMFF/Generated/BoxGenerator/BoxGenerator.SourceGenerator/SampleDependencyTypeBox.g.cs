using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SampleDependencyTypeBox
	extends FullBox('sdtp', version = 0, 0) {
	for (i=0; i < sample_count; i++){
		unsigned int(2) is_leading;
		unsigned int(2) sample_depends_on;
		unsigned int(2) sample_is_depended_on;
		unsigned int(2) sample_has_redundancy;
	}
}
*/
public partial class SampleDependencyTypeBox : FullBox
{
	public const string TYPE = "sdtp";
	public override string DisplayName { get { return "SampleDependencyTypeBox"; } }

	protected byte[] is_leading; 
	public byte[] IsLeading { get { return this.is_leading; } set { this.is_leading = value; } }

	protected byte[] sample_depends_on; 
	public byte[] SampleDependsOn { get { return this.sample_depends_on; } set { this.sample_depends_on = value; } }

	protected byte[] sample_is_depended_on; 
	public byte[] SampleIsDependedOn { get { return this.sample_is_depended_on; } set { this.sample_is_depended_on = value; } }

	protected byte[] sample_has_redundancy; 
	public byte[] SampleHasRedundancy { get { return this.sample_has_redundancy; } set { this.sample_has_redundancy = value; } }

	public SampleDependencyTypeBox(): base(IsoStream.FromFourCC("sdtp"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		int sample_count = (int)((readSize - boxSize) >> 3); // should be taken from the stsz sample_count, but we can calculate it from the readSize - 1 byte per sample


		this.is_leading = new byte[IsoStream.GetInt( sample_count)];
		this.sample_depends_on = new byte[IsoStream.GetInt( sample_count)];
		this.sample_is_depended_on = new byte[IsoStream.GetInt( sample_count)];
		this.sample_has_redundancy = new byte[IsoStream.GetInt( sample_count)];
		for (int i=0; i < sample_count; i++)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.is_leading[i], "is_leading"); 
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.sample_depends_on[i], "sample_depends_on"); 
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.sample_is_depended_on[i], "sample_is_depended_on"); 
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.sample_has_redundancy[i], "sample_has_redundancy"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		int sample_count = is_leading.Length;


		for (int i=0; i < sample_count; i++)
		{
			boxSize += stream.WriteBits(2,  this.is_leading[i], "is_leading"); 
			boxSize += stream.WriteBits(2,  this.sample_depends_on[i], "sample_depends_on"); 
			boxSize += stream.WriteBits(2,  this.sample_is_depended_on[i], "sample_is_depended_on"); 
			boxSize += stream.WriteBits(2,  this.sample_has_redundancy[i], "sample_has_redundancy"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		int sample_count = is_leading.Length;


		for (int i=0; i < sample_count; i++)
		{
			boxSize += 2; // is_leading
			boxSize += 2; // sample_depends_on
			boxSize += 2; // sample_is_depended_on
			boxSize += 2; // sample_has_redundancy
		}
		return boxSize;
	}
}

}
