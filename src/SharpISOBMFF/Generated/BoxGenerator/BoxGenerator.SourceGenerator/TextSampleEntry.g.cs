using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class TextSampleEntry() extends FullBox('enct') {
 unsigned int(16) dataReferenceIndex;
 unsigned int(32) displayFlags;
 unsigned int(8) horizontalJustification;
 unsigned int(8) verticalJustification;
 unsigned int(8) backgroundColorRgba[4];
 RectRecord rectRecord;
 StyleRecord styleRecord; Box boxes[];
 }
 
*/
public partial class TextSampleEntry : FullBox
{
	public const string TYPE = "enct";
	public override string DisplayName { get { return "TextSampleEntry"; } }

	protected ushort dataReferenceIndex; 
	public ushort DataReferenceIndex { get { return this.dataReferenceIndex; } set { this.dataReferenceIndex = value; } }

	protected uint displayFlags; 
	public uint DisplayFlags { get { return this.displayFlags; } set { this.displayFlags = value; } }

	protected byte horizontalJustification; 
	public byte HorizontalJustification { get { return this.horizontalJustification; } set { this.horizontalJustification = value; } }

	protected byte verticalJustification; 
	public byte VerticalJustification { get { return this.verticalJustification; } set { this.verticalJustification = value; } }

	protected byte[] backgroundColorRgba; 
	public byte[] BackgroundColorRgba { get { return this.backgroundColorRgba; } set { this.backgroundColorRgba = value; } }

	protected RectRecord rectRecord; 
	public RectRecord RectRecord { get { return this.rectRecord; } set { this.rectRecord = value; } }

	protected StyleRecord styleRecord; 
	public StyleRecord StyleRecord { get { return this.styleRecord; } set { this.styleRecord = value; } }
	public IEnumerable<Box> Boxes { get { return this.children.OfType<Box>(); } }

	public TextSampleEntry(): base(IsoStream.FromFourCC("enct"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.dataReferenceIndex, "dataReferenceIndex"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.displayFlags, "displayFlags"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.horizontalJustification, "horizontalJustification"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.verticalJustification, "verticalJustification"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 4,  out this.backgroundColorRgba, "backgroundColorRgba"); 
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new RectRecord(),  out this.rectRecord, "rectRecord"); 
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new StyleRecord(),  out this.styleRecord, "styleRecord"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.boxes, "boxes"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.dataReferenceIndex, "dataReferenceIndex"); 
		boxSize += stream.WriteUInt32( this.displayFlags, "displayFlags"); 
		boxSize += stream.WriteUInt8( this.horizontalJustification, "horizontalJustification"); 
		boxSize += stream.WriteUInt8( this.verticalJustification, "verticalJustification"); 
		boxSize += stream.WriteUInt8Array(4,  this.backgroundColorRgba, "backgroundColorRgba"); 
		boxSize += stream.WriteClass( this.rectRecord, "rectRecord"); 
		boxSize += stream.WriteClass( this.styleRecord, "styleRecord"); 
		// boxSize += stream.WriteBox( this.boxes, "boxes"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // dataReferenceIndex
		boxSize += 32; // displayFlags
		boxSize += 8; // horizontalJustification
		boxSize += 8; // verticalJustification
		boxSize += 4 * 8; // backgroundColorRgba
		boxSize += IsoStream.CalculateClassSize(rectRecord); // rectRecord
		boxSize += IsoStream.CalculateClassSize(styleRecord); // styleRecord
		// boxSize += IsoStream.CalculateBoxSize(boxes); // boxes
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
