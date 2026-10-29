using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SamplingRateBox extends FullBox('srat') {
	unsigned int(32) sampling_rate;
}
*/
public partial class SamplingRateBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class SamplingRateBox extends FullBox('srat') {
	unsigned int(32) sampling_rate;
}"; } }
	public const string TYPE = "srat";
	public override string DisplayName { get { return "SamplingRateBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("AudioSampleEntry"); }


	protected uint sampling_rate; 
	public uint SamplingRate { get { return this.sampling_rate; } set { this.sampling_rate = value; } }

	public SamplingRateBox(): base(IsoStream.FromFourCC("srat"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sampling_rate, "sampling_rate"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.sampling_rate, "sampling_rate"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // sampling_rate
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("sampling_rate", 32, this.sampling_rate); 
		return boxSize;
	}
}

}
