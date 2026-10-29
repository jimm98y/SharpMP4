using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class AUDSampleEntry() extends VisualSampleGroupEntry ('aud ')
{
	bit(24) audNalUnit;
}
*/
public partial class AUDSampleEntry : VisualSampleGroupEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class AUDSampleEntry() extends VisualSampleGroupEntry ('aud ')
{
	bit(24) audNalUnit;
}"; } }
	public const string TYPE = "aud ";
	public override string DisplayName { get { return "AUDSampleEntry"; } }

	protected uint audNalUnit; 
	public uint AudNalUnit { get { return this.audNalUnit; } set { this.audNalUnit = value; } }

	public AUDSampleEntry(): base(IsoStream.FromFourCC("aud "))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt24(boxSize, readSize,  out this.audNalUnit, "audNalUnit"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt24( this.audNalUnit, "audNalUnit"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 24; // audNalUnit
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("audNalUnit", 24, this.audNalUnit); 
		return boxSize;
	}
}

}
