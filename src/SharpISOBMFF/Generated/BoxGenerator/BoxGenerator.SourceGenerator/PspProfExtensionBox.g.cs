using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class PspProfExtensionBox() extends Box('uuid 50524f4621d24fcebb88695cfac9c740') {
	 unsigned int(32) unknown1;
 unsigned int(32) entry_count;
 Box boxes[];
 }
*/
public partial class PspProfExtensionBox : Box
{
	public const string TYPE = "uuid";
	public override string DisplayName { get { return "PspProfExtensionBox"; } }

	protected uint unknown1; 
	public uint Unknown1 { get { return this.unknown1; } set { this.unknown1 = value; } }

	protected uint entry_count; 
	public uint EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }
	public IEnumerable<Box> Boxes { get { return this.children.OfType<Box>(); } }

	public PspProfExtensionBox(): base(IsoStream.FromFourCC("uuid"), ConvertEx.FromHexString("50524f4621d24fcebb88695cfac9c740"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.unknown1, "unknown1"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entry_count, "entry_count"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.boxes, "boxes"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.unknown1, "unknown1"); 
		boxSize += stream.WriteUInt32( this.entry_count, "entry_count"); 
		// boxSize += stream.WriteBox( this.boxes, "boxes"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // unknown1
		boxSize += 32; // entry_count
		// boxSize += IsoStream.CalculateBoxSize(boxes); // boxes
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
