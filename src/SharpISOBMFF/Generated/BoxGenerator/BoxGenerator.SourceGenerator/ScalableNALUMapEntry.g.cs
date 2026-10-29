using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class ScalableNALUMapEntry() extends VisualSampleGroupEntry ('scnm') {
	bit(8) reserved = 0;
	unsigned int(8) NALU_count;
	for (i=1; i<= NALU_count; i++)
		unsigned int(8) groupID;
}
*/
public partial class ScalableNALUMapEntry : VisualSampleGroupEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class ScalableNALUMapEntry() extends VisualSampleGroupEntry ('scnm') {
	bit(8) reserved = 0;
	unsigned int(8) NALU_count;
	for (i=1; i<= NALU_count; i++)
		unsigned int(8) groupID;
}"; } }
	public const string TYPE = "scnm";
	public override string DisplayName { get { return "ScalableNALUMapEntry"; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte NALU_count; 
	public byte NALUCount { get { return this.NALU_count; } set { this.NALU_count = value; } }

	protected byte[] groupID; 
	public byte[] GroupID { get { return this.groupID; } set { this.groupID = value; } }

	public ScalableNALUMapEntry(): base(IsoStream.FromFourCC("scnm"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.NALU_count, "NALU_count"); 

		this.groupID = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt( NALU_count), "groupID");
		for (int i=0; i< NALU_count; i++)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.groupID[i], "groupID"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.reserved, "reserved"); 
		boxSize += stream.WriteUInt8( this.NALU_count, "NALU_count"); 

		for (int i=0; i< NALU_count; i++)
		{
			boxSize += stream.WriteUInt8( this.groupID[i], "groupID"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // reserved
		boxSize += 8; // NALU_count

		for (int i=0; i< NALU_count; i++)
		{
			boxSize += 8; // groupID
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("reserved", 8, this.reserved); 
		boxSize += tracer.Field("NALU_count", 8, this.NALU_count); 

		for (int i=0; i< NALU_count; i++)
		{
			boxSize += tracer.Field($"groupID[{i}]", 8, this.groupID[i]); 
		}
		return boxSize;
	}
}

}
