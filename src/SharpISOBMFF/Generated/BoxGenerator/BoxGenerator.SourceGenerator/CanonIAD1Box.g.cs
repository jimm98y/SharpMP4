using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// A CR3's image area dimensions (lclevy/canon_cr3, IAD1, reverse engineered): the image's size, then rectangles to the end
aligned(8) class CanonIAD1Box() extends FullBox('IAD1') {
 unsigned int(16) image_width;
 unsigned int(16) image_height;
 unsigned int(16) unknown1; // 1
 unsigned int(16) image_kind; // 0 small, 2 big (sliced?)
 unsigned int(16) unknown2; // 1
 unsigned int(16) unknown3; // 0
 CanonImageArea areas[]; // small: the crop, the whole; big: the crop, left and top optical black, the active area
}
// a rectangle of IAD1: its left, top, right and bottom, as offsets from 0

*/
public partial class CanonIAD1Box : FullBox
{
	public const string TYPE = "IAD1";
	public override string DisplayName { get { return "CanonIAD1Box"; } }

	protected ushort image_width; 
	public ushort ImageWidth { get { return this.image_width; } set { this.image_width = value; } }

	protected ushort image_height; 
	public ushort ImageHeight { get { return this.image_height; } set { this.image_height = value; } }

	protected ushort unknown1;  //  1
	public ushort Unknown1 { get { return this.unknown1; } set { this.unknown1 = value; } }

	protected ushort image_kind;  //  0 small, 2 big (sliced?)
	public ushort ImageKind { get { return this.image_kind; } set { this.image_kind = value; } }

	protected ushort unknown2;  //  1
	public ushort Unknown2 { get { return this.unknown2; } set { this.unknown2 = value; } }

	protected ushort unknown3;  //  0
	public ushort Unknown3 { get { return this.unknown3; } set { this.unknown3 = value; } }

	protected CanonImageArea[] areas;  //  small: the crop, the whole; big: the crop, left and top optical black, the active area
	public CanonImageArea[] Areas { get { return this.areas; } set { this.areas = value; } }

	public CanonIAD1Box(): base(IsoStream.FromFourCC("IAD1"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.image_width, "image_width"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.image_height, "image_height"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.unknown1, "unknown1"); // 1
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.image_kind, "image_kind"); // 0 small, 2 big (sliced?)
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.unknown2, "unknown2"); // 1
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.unknown3, "unknown3"); // 0
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(uint.MaxValue), () => new CanonImageArea(),  out this.areas, "areas"); // small: the crop, the whole; big: the crop, left and top optical black, the active area
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.image_width, "image_width"); 
		boxSize += stream.WriteUInt16( this.image_height, "image_height"); 
		boxSize += stream.WriteUInt16( this.unknown1, "unknown1"); // 1
		boxSize += stream.WriteUInt16( this.image_kind, "image_kind"); // 0 small, 2 big (sliced?)
		boxSize += stream.WriteUInt16( this.unknown2, "unknown2"); // 1
		boxSize += stream.WriteUInt16( this.unknown3, "unknown3"); // 0
		boxSize += stream.WriteClass( this.areas, "areas"); // small: the crop, the whole; big: the crop, left and top optical black, the active area
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // image_width
		boxSize += 16; // image_height
		boxSize += 16; // unknown1
		boxSize += 16; // image_kind
		boxSize += 16; // unknown2
		boxSize += 16; // unknown3
		boxSize += IsoStream.CalculateClassSize(areas); // areas
		return boxSize;
	}
}

}
