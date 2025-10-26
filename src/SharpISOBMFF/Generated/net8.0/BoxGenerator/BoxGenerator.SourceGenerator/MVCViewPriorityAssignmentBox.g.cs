using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class MVCViewPriorityAssignmentBox extends Box('mvcP')
{
	unsigned int(8)	method_count;
	string PriorityAssignmentURI[method_count]; 
}
*/
public partial class MVCViewPriorityAssignmentBox : Box
{
	public const string TYPE = "mvcP";
	public override string DisplayName { get { return "MVCViewPriorityAssignmentBox"; } }

	protected byte method_count; 
	public byte MethodCount { get { return this.method_count; } set { this.method_count = value; } }

	protected BinaryUTF8String[] PriorityAssignmentURI; 
	public BinaryUTF8String[] _PriorityAssignmentURI { get { return this.PriorityAssignmentURI; } set { this.PriorityAssignmentURI = value; } }

	public MVCViewPriorityAssignmentBox(): base(IsoStream.FromFourCC("mvcP"))
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
}

}
