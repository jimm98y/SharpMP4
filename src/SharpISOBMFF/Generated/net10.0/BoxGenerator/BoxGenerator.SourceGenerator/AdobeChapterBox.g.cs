using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class AdobeChapterBox() extends FullBox('chpl', version = 0, 0) {
 unsigned int(32) unknown; unsigned int(8) count;
 AdobeChapterRecord chapters[]; 
 }
 
*/
public partial class AdobeChapterBox : FullBox
{
	public const string TYPE = "chpl";
	public override string DisplayName { get { return "AdobeChapterBox"; } }

	protected uint unknown; 
	public uint Unknown { get { return this.unknown; } set { this.unknown = value; } }

	protected byte count; 
	public byte Count { get { return this.count; } set { this.count = value; } }

	protected AdobeChapterRecord[] chapters; 
	public AdobeChapterRecord[] Chapters { get { return this.chapters; } set { this.chapters = value; } }

	public AdobeChapterBox(): base(IsoStream.FromFourCC("chpl"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.unknown, "unknown"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.count, "count"); 
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(uint.MaxValue), () => new AdobeChapterRecord(),  out this.chapters, "chapters"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.unknown, "unknown"); 
		boxSize += stream.WriteUInt8( this.count, "count"); 
		boxSize += stream.WriteClass( this.chapters, "chapters"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // unknown
		boxSize += 8; // count
		boxSize += IsoStream.CalculateClassSize(chapters); // chapters
		return boxSize;
	}
}

}
