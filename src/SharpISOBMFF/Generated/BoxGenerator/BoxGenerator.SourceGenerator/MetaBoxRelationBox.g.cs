using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class MetaBoxRelationBox() extends FullBox('mere') {
 unsigned int(32) firstMetaBoxHandlerType;
 unsigned int(32) secondMetaBoxHandlerType;
 bit(8) metaboxRelation; 
 }
*/
public partial class MetaBoxRelationBox : FullBox
{
	public const string TYPE = "mere";
	public override string DisplayName { get { return "MetaBoxRelationBox"; } }

	protected uint firstMetaBoxHandlerType; 
	public uint FirstMetaBoxHandlerType { get { return this.firstMetaBoxHandlerType; } set { this.firstMetaBoxHandlerType = value; } }

	protected uint secondMetaBoxHandlerType; 
	public uint SecondMetaBoxHandlerType { get { return this.secondMetaBoxHandlerType; } set { this.secondMetaBoxHandlerType = value; } }

	protected byte metaboxRelation; 
	public byte MetaboxRelation { get { return this.metaboxRelation; } set { this.metaboxRelation = value; } }

	public MetaBoxRelationBox(): base(IsoStream.FromFourCC("mere"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.firstMetaBoxHandlerType, "firstMetaBoxHandlerType"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.secondMetaBoxHandlerType, "secondMetaBoxHandlerType"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.metaboxRelation, "metaboxRelation"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.firstMetaBoxHandlerType, "firstMetaBoxHandlerType"); 
		boxSize += stream.WriteUInt32( this.secondMetaBoxHandlerType, "secondMetaBoxHandlerType"); 
		boxSize += stream.WriteUInt8( this.metaboxRelation, "metaboxRelation"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // firstMetaBoxHandlerType
		boxSize += 32; // secondMetaBoxHandlerType
		boxSize += 8; // metaboxRelation
		return boxSize;
	}
}

}
