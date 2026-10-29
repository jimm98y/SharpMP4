using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class IPI_DescrPointer extends BaseDescriptor : bit(8) tag=IPI_DescrPointerTag {
 bit(16) IPI_ES_Id;
 }
*/
public partial class IPI_DescrPointer : BaseDescriptor
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class IPI_DescrPointer extends BaseDescriptor : bit(8) tag=IPI_DescrPointerTag {
 bit(16) IPI_ES_Id;
 }"; } }
	public const byte TYPE = DescriptorTags.IPI_DescrPointerTag;
	public override string DisplayName { get { return "IPI_DescrPointer"; } }

	protected ushort IPI_ES_Id; 
	public ushort IPIESId { get { return this.IPI_ES_Id; } set { this.IPI_ES_Id = value; } }

	public IPI_DescrPointer(): base(DescriptorTags.IPI_DescrPointerTag)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.IPI_ES_Id, "IPI_ES_Id"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.IPI_ES_Id, "IPI_ES_Id"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // IPI_ES_Id
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("IPI_ES_Id", 16, this.IPI_ES_Id); 
		return boxSize;
	}
}

}
