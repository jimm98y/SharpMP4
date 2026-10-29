using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class OperatingPointDecodeTimeHint()
extends VisualSampleGroupEntry ('opth')
{
	signed int(32) delta_time;
}

*/
public partial class OperatingPointDecodeTimeHint : VisualSampleGroupEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class OperatingPointDecodeTimeHint()
extends VisualSampleGroupEntry ('opth')
{
	signed int(32) delta_time;
}
"; } }
	public const string TYPE = "opth";
	public override string DisplayName { get { return "OperatingPointDecodeTimeHint"; } }

	protected int delta_time; 
	public int DeltaTime { get { return this.delta_time; } set { this.delta_time = value; } }

	public OperatingPointDecodeTimeHint(): base(IsoStream.FromFourCC("opth"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.delta_time, "delta_time"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt32( this.delta_time, "delta_time"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // delta_time
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("delta_time", 32, this.delta_time); 
		return boxSize;
	}
}

}
