using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class SVCPriorityAssignmentBox extends Box('svcP')
{
	unsigned int(8)	method_count;
	string PriorityAssignmentURI[method_count];
}
*/
public partial class SVCPriorityAssignmentBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class SVCPriorityAssignmentBox extends Box('svcP')
{
	unsigned int(8)	method_count;
	string PriorityAssignmentURI[method_count];
}"; } }
	public const string TYPE = "svcP";
	public override string DisplayName { get { return "SVCPriorityAssignmentBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("AVCSVCSampleEntry(avc1,avc3)", "AVC2SVCSampleEntry(avc2,avc4)", "SVCSampleEntry(svc1,svc2)", "SVCMetaDataSampleEntry(svcM)"); }


	protected byte method_count; 
	public byte MethodCount { get { return this.method_count; } set { this.method_count = value; } }

	protected BinaryUTF8String[] PriorityAssignmentURI = []; 
	public BinaryUTF8String[] _PriorityAssignmentURI { get { return this.PriorityAssignmentURI; } set { this.PriorityAssignmentURI = value; } }

	public SVCPriorityAssignmentBox(): base(IsoStream.FromFourCC("svcP"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.method_count, "method_count"); 
		boxSize += stream.ReadStringZeroTerminatedArray(boxSize, readSize, (uint)(method_count),  out this.PriorityAssignmentURI, "PriorityAssignmentURI"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.method_count, "method_count"); 
		boxSize += stream.WriteStringZeroTerminatedArray((uint)(method_count),  this.PriorityAssignmentURI, "PriorityAssignmentURI"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // method_count
		boxSize += IsoStream.CalculateStringSize(PriorityAssignmentURI); // PriorityAssignmentURI
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("method_count", 8, this.method_count); 
		boxSize += tracer.Field("PriorityAssignmentURI", IsoStream.CalculateStringSize(PriorityAssignmentURI), this.PriorityAssignmentURI); 
		return boxSize;
	}
}

}
