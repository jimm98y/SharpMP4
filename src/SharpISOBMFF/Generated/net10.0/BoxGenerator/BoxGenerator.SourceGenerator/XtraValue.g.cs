using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class XtraValue() {
 unsigned int(32) length;
 unsigned int(16) type;
 unsigned int(8) value[length-6];
 }
*/
public partial class XtraValue : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "XtraValue"; } }

	protected uint length; 
	public uint Length { get { return this.length; } set { this.length = value; } }

	protected ushort type; 
	public ushort Type { get { return this.type; } set { this.type = value; } }

	protected byte[] value; 
	public byte[] Value { get { return this.value; } set { this.value = value; } }

	public XtraValue(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.length, "length"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.type, "type"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(length-6),  out this.value, "value"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt32( this.length, "length"); 
		boxSize += stream.WriteUInt16( this.type, "type"); 
		boxSize += stream.WriteUInt8Array((uint)(length-6),  this.value, "value"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 32; // length
		boxSize += 16; // type
		boxSize += ((ulong)(length-6) * 8); // value
		return boxSize;
	}
}

}
