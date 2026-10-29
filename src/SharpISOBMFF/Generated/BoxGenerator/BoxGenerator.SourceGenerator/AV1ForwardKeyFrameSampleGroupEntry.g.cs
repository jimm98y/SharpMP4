using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class AV1ForwardKeyFrameSampleGroupEntry
	extends VisualSampleGroupEntry('av1f')
{
	unsigned int(8) fwd_distance;
}
*/
public partial class AV1ForwardKeyFrameSampleGroupEntry : VisualSampleGroupEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class AV1ForwardKeyFrameSampleGroupEntry
	extends VisualSampleGroupEntry('av1f')
{
	unsigned int(8) fwd_distance;
}"; } }
	public const string TYPE = "av1f";
	public override string DisplayName { get { return "AV1ForwardKeyFrameSampleGroupEntry"; } }

	protected byte fwd_distance; 
	public byte FwdDistance { get { return this.fwd_distance; } set { this.fwd_distance = value; } }

	public AV1ForwardKeyFrameSampleGroupEntry(): base(IsoStream.FromFourCC("av1f"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.fwd_distance, "fwd_distance"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.fwd_distance, "fwd_distance"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // fwd_distance
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("fwd_distance", 8, this.fwd_distance); 
		return boxSize;
	}
}

}
