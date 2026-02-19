using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class OMAIconURLBox() extends FullBox('icnu') {
	 string content;
 } 
*/
public partial class OMAIconURLBox : FullBox
{
	public const string TYPE = "icnu";
	public override string DisplayName { get { return "OMAIconURLBox"; } }

	protected BinaryUTF8String content; 
	public BinaryUTF8String Content { get { return this.content; } set { this.content = value; } }

	public OMAIconURLBox(): base(IsoStream.FromFourCC("icnu"))
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
