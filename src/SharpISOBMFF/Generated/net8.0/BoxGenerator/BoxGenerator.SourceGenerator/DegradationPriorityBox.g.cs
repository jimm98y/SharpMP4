using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class DegradationPriorityBox
	extends FullBox('stdp', version = 0, 0) {
	int i;
	for (i=0; i < sample_count; i++) {
		unsigned int(16)	priority;
	}
}
*/
public partial class DegradationPriorityBox : FullBox
{
	public const string TYPE = "stdp";
	public override string DisplayName { get { return "DegradationPriorityBox"; } }

	protected ushort[] priority; 
	public ushort[] Priority { get { return this.priority; } set { this.priority = value; } }

	public DegradationPriorityBox(): base(IsoStream.FromFourCC("stdp"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		int sample_count = (int)((readSize - boxSize) >> 4); // should be taken from the stsz sample_count, but we can calculate it from the readSize - 2 bytes per sample

		

		this.priority = new ushort[IsoStream.GetInt( sample_count)];
		for (int i=0; i < sample_count; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.priority[i], "priority"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		int sample_count = priority.Length;

		

		for (int i=0; i < sample_count; i++)
		{
			boxSize += stream.WriteUInt16( this.priority[i], "priority"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		int sample_count = priority.Length;

		

		for (int i=0; i < sample_count; i++)
		{
			boxSize += 16; // priority
		}
		return boxSize;
	}
}

}
