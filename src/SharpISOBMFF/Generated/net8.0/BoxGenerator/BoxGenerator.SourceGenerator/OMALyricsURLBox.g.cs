using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class OMALyricsURLBox() extends FullBox('lrcu') {
	 string content;
 } 
*/
public partial class OMALyricsURLBox : FullBox
{
	public const string TYPE = "lrcu";
	public override string DisplayName { get { return "OMALyricsURLBox"; } }

	protected BinaryUTF8String content; 
	public BinaryUTF8String Content { get { return this.content; } set { this.content = value; } }

	public OMALyricsURLBox(): base(IsoStream.FromFourCC("lrcu"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.content, "content"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.content, "content"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(content); // content
		return boxSize;
	}
}

}
