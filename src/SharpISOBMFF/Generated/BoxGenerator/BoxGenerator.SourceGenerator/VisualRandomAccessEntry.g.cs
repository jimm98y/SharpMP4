using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class VisualRandomAccessEntry() extends VisualSampleGroupEntry ('rap ')
{
	unsigned int(1) num_leading_samples_known;
	unsigned int(7) num_leading_samples;
}
*/
public partial class VisualRandomAccessEntry : VisualSampleGroupEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class VisualRandomAccessEntry() extends VisualSampleGroupEntry ('rap ')
{
	unsigned int(1) num_leading_samples_known;
	unsigned int(7) num_leading_samples;
}"; } }
	public const string TYPE = "rap ";
	public override string DisplayName { get { return "VisualRandomAccessEntry"; } }

	protected bool num_leading_samples_known; 
	public bool NumLeadingSamplesKnown { get { return this.num_leading_samples_known; } set { this.num_leading_samples_known = value; } }

	protected byte num_leading_samples; 
	public byte NumLeadingSamples { get { return this.num_leading_samples; } set { this.num_leading_samples = value; } }

	public VisualRandomAccessEntry(): base(IsoStream.FromFourCC("rap "))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.num_leading_samples_known, "num_leading_samples_known"); 
		boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.num_leading_samples, "num_leading_samples"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBit( this.num_leading_samples_known, "num_leading_samples_known"); 
		boxSize += stream.WriteBits(7,  this.num_leading_samples, "num_leading_samples"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 1; // num_leading_samples_known
		boxSize += 7; // num_leading_samples
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("num_leading_samples_known", 1, this.num_leading_samples_known); 
		boxSize += tracer.Field("num_leading_samples", 7, this.num_leading_samples); 
		return boxSize;
	}
}

}
