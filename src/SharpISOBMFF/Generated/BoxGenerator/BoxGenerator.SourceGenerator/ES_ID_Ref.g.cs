using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class ES_ID_Ref extends BaseDescriptor : bit(8) tag=ES_ID_RefTag {
	bit(16) ref_index;
}
*/
public partial class ES_ID_Ref : BaseDescriptor
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class ES_ID_Ref extends BaseDescriptor : bit(8) tag=ES_ID_RefTag {
	bit(16) ref_index;
}"; } }
	public const byte TYPE = DescriptorTags.ES_ID_RefTag;
	public override string DisplayName { get { return "ES_ID_Ref"; } }

	protected ushort ref_index; 
	public ushort RefIndex { get { return this.ref_index; } set { this.ref_index = value; } }

	public ES_ID_Ref(): base(DescriptorTags.ES_ID_RefTag)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.ref_index, "ref_index"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.ref_index, "ref_index"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // ref_index
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("ref_index", 16, this.ref_index); 
		return boxSize;
	}
}

}
