using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class OMADiscreteMediaHeadersBox () extends FullBox('odhe') {
	 unsigned int(8) contentTypeLength;
 unsigned int(8) contentType[contentTypeLength];
 Box boxes[]; 
 } 
*/
public partial class OMADiscreteMediaHeadersBox : FullBox
{
	public const string TYPE = "odhe";
	public override string DisplayName { get { return "OMADiscreteMediaHeadersBox"; } }

	protected byte contentTypeLength; 
	public byte ContentTypeLength { get { return this.contentTypeLength; } set { this.contentTypeLength = value; } }

	protected byte[] contentType; 
	public byte[] ContentType { get { return this.contentType; } set { this.contentType = value; } }
	public IEnumerable<Box> Boxes { get { return this.children.OfType<Box>(); } }

	public OMADiscreteMediaHeadersBox(): base(IsoStream.FromFourCC("odhe"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.contentTypeLength, "contentTypeLength"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(contentTypeLength),  out this.contentType, "contentType"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.boxes, "boxes"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.contentTypeLength, "contentTypeLength"); 
		boxSize += stream.WriteUInt8Array((uint)(contentTypeLength),  this.contentType, "contentType"); 
		// boxSize += stream.WriteBox( this.boxes, "boxes"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // contentTypeLength
		boxSize += ((ulong)(contentTypeLength) * 8); // contentType
		// boxSize += IsoStream.CalculateBoxSize(boxes); // boxes
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
