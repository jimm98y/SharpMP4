using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class MetaDataAccessUnit {
Box boxes[];
}


*/
public partial class MetaDataAccessUnit : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "MetaDataAccessUnit"; } }

	protected Box[] boxes; 
	public Box[] Boxes { get { return this.boxes; } set { this.boxes = value; } }

	public MetaDataAccessUnit(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBox(boxSize, readSize, this,  out this.boxes, "boxes"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBox( this.boxes, "boxes"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += IsoStream.CalculateBoxSize(boxes); // boxes
		return boxSize;
	}
}

}
