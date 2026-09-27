using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ThreeGPPClassificationBox() extends FullBox('clsf', version = 0, 0) {
	unsigned int(32) classificationEntity;
	unsigned int(16) classificationTable;
	bit(1) reserved = 0;
	unsigned int(5)[3] language;
	string value;
} 
*/
public partial class ThreeGPPClassificationBox : FullBox
{
	public const string TYPE = "clsf";
	public override string DisplayName { get { return "ThreeGPPClassificationBox"; } }

	protected uint classificationEntity; 
	public uint ClassificationEntity { get { return this.classificationEntity; } set { this.classificationEntity = value; } }

	protected ushort classificationTable; 
	public ushort ClassificationTable { get { return this.classificationTable; } set { this.classificationTable = value; } }

	protected bool reserved = false; 
	public bool Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected string language; 
	public string Language { get { return this.language; } set { this.language = value; } }

	protected BinaryUTF8String value; 
	public BinaryUTF8String Value { get { return this.value; } set { this.value = value; } }

	public ThreeGPPClassificationBox(): base(IsoStream.FromFourCC("clsf"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.classificationEntity, "classificationEntity"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.classificationTable, "classificationTable"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadIso639(boxSize, readSize,  out this.language, "language"); 
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.value, "value"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.classificationEntity, "classificationEntity"); 
		boxSize += stream.WriteUInt16( this.classificationTable, "classificationTable"); 
		boxSize += stream.WriteBit( this.reserved, "reserved"); 
		boxSize += stream.WriteIso639( this.language, "language"); 
		boxSize += stream.WriteStringZeroTerminated( this.value, "value"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // classificationEntity
		boxSize += 16; // classificationTable
		boxSize += 1; // reserved
		boxSize += 15; // language
		boxSize += IsoStream.CalculateStringSize(value); // value
		return boxSize;
	}
}

}
