using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class RectRecord() {
 unsigned int(16) top;
 unsigned int(16) left;
 unsigned int(16) bottom;
 unsigned int(16) right;
 }
 
*/
public partial class RectRecord : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "RectRecord"; } }

	protected ushort top; 
	public ushort Top { get { return this.top; } set { this.top = value; } }

	protected ushort left; 
	public ushort Left { get { return this.left; } set { this.left = value; } }

	protected ushort bottom; 
	public ushort Bottom { get { return this.bottom; } set { this.bottom = value; } }

	protected ushort right; 
	public ushort Right { get { return this.right; } set { this.right = value; } }

	public RectRecord(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.top, "top"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.left, "left"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.bottom, "bottom"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.right, "right"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt16( this.top, "top"); 
		boxSize += stream.WriteUInt16( this.left, "left"); 
		boxSize += stream.WriteUInt16( this.bottom, "bottom"); 
		boxSize += stream.WriteUInt16( this.right, "right"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 16; // top
		boxSize += 16; // left
		boxSize += 16; // bottom
		boxSize += 16; // right
		return boxSize;
	}
}

}
