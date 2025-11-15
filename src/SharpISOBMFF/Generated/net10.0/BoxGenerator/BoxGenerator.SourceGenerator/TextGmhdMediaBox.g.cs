using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class TextGmhdMediaBox() extends Box('text') {
 unsigned int(8) textData[36];
 } 
*/
public partial class TextGmhdMediaBox : Box
{
	public const string TYPE = "text";
	public override string DisplayName { get { return "TextGmhdMediaBox"; } }

	protected byte[] textData; 
	public byte[] TextData { get { return this.textData; } set { this.textData = value; } }

	public TextGmhdMediaBox(): base(IsoStream.FromFourCC("text"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 36,  out this.textData, "textData"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8Array(36,  this.textData, "textData"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 36 * 8; // textData
		return boxSize;
	}
}

}
