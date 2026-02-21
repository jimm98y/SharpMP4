using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class FontRecord() {
 unsigned int(16) fontId;
 unsigned int(8) count;
 unsigned int(8) fontName[count];
 }
*/
public partial class FontRecord : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "FontRecord"; } }

	protected ushort fontId; 
	public ushort FontId { get { return this.fontId; } set { this.fontId = value; } }

	protected byte count; 
	public byte Count { get { return this.count; } set { this.count = value; } }

	protected byte[] fontName; 
	public byte[] FontName { get { return this.fontName; } set { this.fontName = value; } }

	public FontRecord(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.fontId, "fontId"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.count, "count"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(count),  out this.fontName, "fontName"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt16( this.fontId, "fontId"); 
		boxSize += stream.WriteUInt8( this.count, "count"); 
		boxSize += stream.WriteUInt8Array((uint)(count),  this.fontName, "fontName"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 16; // fontId
		boxSize += 8; // count
		boxSize += ((ulong)(count) * 8); // fontName
		return boxSize;
	}
}

}
