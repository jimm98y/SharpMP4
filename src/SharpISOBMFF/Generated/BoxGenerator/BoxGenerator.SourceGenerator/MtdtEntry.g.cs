using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class MtdtEntry() {
 unsigned int(16) size;
 unsigned int(32) type;
 bit(1) reserved;
unsigned int(5)[3] language;
 unsigned int(16) dataType;
 bit(8) data[size-10];
 }

*/
public partial class MtdtEntry : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "MtdtEntry"; } }

	protected ushort size; 
	public ushort Size { get { return this.size; } set { this.size = value; } }

	protected uint type; 
	public uint Type { get { return this.type; } set { this.type = value; } }

	protected bool reserved; 
	public bool Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected string language; 
	public string Language { get { return this.language; } set { this.language = value; } }

	protected ushort dataType; 
	public ushort DataType { get { return this.dataType; } set { this.dataType = value; } }

	protected byte[] data; 
	public byte[] Data { get { return this.data; } set { this.data = value; } }

	public MtdtEntry(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.size, "size"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.type, "type"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadIso639(boxSize, readSize,  out this.language, "language"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.dataType, "dataType"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(size-10),  out this.data, "data"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt16( this.size, "size"); 
		boxSize += stream.WriteUInt32( this.type, "type"); 
		boxSize += stream.WriteBit( this.reserved, "reserved"); 
		boxSize += stream.WriteIso639( this.language, "language"); 
		boxSize += stream.WriteUInt16( this.dataType, "dataType"); 
		boxSize += stream.WriteUInt8Array((uint)(size-10),  this.data, "data"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 16; // size
		boxSize += 32; // type
		boxSize += 1; // reserved
		boxSize += 15; // language
		boxSize += 16; // dataType
		boxSize += ((ulong)(size-10) * 8); // data
		return boxSize;
	}
}

}
