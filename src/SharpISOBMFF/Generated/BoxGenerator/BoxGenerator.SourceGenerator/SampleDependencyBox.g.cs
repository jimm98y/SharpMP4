using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SampleDependencyBox 
	extends FullBox('sdep', version = 0, 0) {
	for (i=0; i < sample_count; i++){
		unsigned int(16) dependency_count;
		for (k=0; k < dependency_count; k++) {
			signed int(16) relative_sample_number;
		}
	}
}
*/
public partial class SampleDependencyBox : FullBox
{
	public const string TYPE = "sdep";
	public override string DisplayName { get { return "SampleDependencyBox"; } }

	protected ushort[] dependency_count; 
	public ushort[] DependencyCount { get { return this.dependency_count; } set { this.dependency_count = value; } }

	protected short[][] relative_sample_number; 
	public short[][] RelativeSampleNumber { get { return this.relative_sample_number; } set { this.relative_sample_number = value; } }

	public SampleDependencyBox(): base(IsoStream.FromFourCC("sdep"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		int sample_count = 0; // TODO: taken from the stsz sample_count


		this.dependency_count = new ushort[IsoStream.GetInt( sample_count)];
		this.relative_sample_number = new short[IsoStream.GetInt( sample_count)][];
		for (int i=0; i < sample_count; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.dependency_count[i], "dependency_count"); 

			this.relative_sample_number[i] = new short[IsoStream.GetInt( dependency_count[i])];
			for (int k=0; k < dependency_count[i]; k++)
			{
				boxSize += stream.ReadInt16(boxSize, readSize,  out this.relative_sample_number[i][k], "relative_sample_number"); 
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		int sample_count = 0; // TODO: taken from the stsz sample_count


		for (int i=0; i < sample_count; i++)
		{
			boxSize += stream.WriteUInt16( this.dependency_count[i], "dependency_count"); 

			for (int k=0; k < dependency_count[i]; k++)
			{
				boxSize += stream.WriteInt16( this.relative_sample_number[i][k], "relative_sample_number"); 
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		int sample_count = 0; // TODO: taken from the stsz sample_count


		for (int i=0; i < sample_count; i++)
		{
			boxSize += 16; // dependency_count

			for (int k=0; k < dependency_count[i]; k++)
			{
				boxSize += 16; // relative_sample_number
			}
		}
		return boxSize;
	}
}

}
