using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// QuickTime.pm, PreviewImage: a CR3's preview, in a PRVW after 8 bytes
aligned(8) class CanonPreviewUuidBox() extends Box('uuid eaf42b5e1c984b88b9fbb7dc406e4d16') {
 unsigned int(32) unknown1;
 unsigned int(32) unknown2;
 Box boxes[];
}

*/
public partial class CanonPreviewUuidBox : Box
{
	public const string TYPE = "uuid";
	public override string DisplayName { get { return "CanonPreviewUuidBox"; } }

	protected uint unknown1; 
	public uint Unknown1 { get { return this.unknown1; } set { this.unknown1 = value; } }

	protected uint unknown2; 
	public uint Unknown2 { get { return this.unknown2; } set { this.unknown2 = value; } }
	public IEnumerable<Box> Boxes { get { return this.children.OfType<Box>(); } }

	public CanonPreviewUuidBox(): base(IsoStream.FromFourCC("uuid"), ConvertEx.FromHexString("eaf42b5e1c984b88b9fbb7dc406e4d16"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.unknown1, "unknown1"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.unknown2, "unknown2"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.boxes, "boxes"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.unknown1, "unknown1"); 
		boxSize += stream.WriteUInt32( this.unknown2, "unknown2"); 
		// boxSize += stream.WriteBox( this.boxes, "boxes"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // unknown1
		boxSize += 32; // unknown2
		// boxSize += IsoStream.CalculateBoxSize(boxes); // boxes
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
