using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ThreeGPPKeywordsBox() extends FullBox('kywd', version = 0, 0) {
	bit(1) reserved = 0;
	unsigned int(5)[3] language;
	unsigned int(8) keywordCount;
	for (i = 0; i < keywordCount; i++) {
		unsigned int(8) keywordSize;
		bit(8*keywordSize) keyword;
	}
}
*/
public partial class ThreeGPPKeywordsBox : FullBox
{
	public const string TYPE = "kywd";
	public override string DisplayName { get { return "ThreeGPPKeywordsBox"; } }

	protected bool reserved = false; 
	public bool Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected string language; 
	public string Language { get { return this.language; } set { this.language = value; } }

	protected byte keywordCount; 
	public byte KeywordCount { get { return this.keywordCount; } set { this.keywordCount = value; } }

	protected byte[] keywordSize; 
	public byte[] KeywordSize { get { return this.keywordSize; } set { this.keywordSize = value; } }

	protected byte[][] keyword; 
	public byte[][] Keyword { get { return this.keyword; } set { this.keyword = value; } }

	public ThreeGPPKeywordsBox(): base(IsoStream.FromFourCC("kywd"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadIso639(boxSize, readSize,  out this.language, "language"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.keywordCount, "keywordCount"); 

		this.keywordSize = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt( keywordCount), "keywordSize");
		this.keyword = stream.SafeAllocate<byte[]>(boxSize, readSize, IsoStream.GetInt( keywordCount), "keyword");
		for (int i = 0; i < keywordCount; i++)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.keywordSize[i], "keywordSize"); 
			boxSize += stream.ReadBits(boxSize, readSize, (uint)(8*keywordSize[i] ),  out this.keyword[i], "keyword"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBit( this.reserved, "reserved"); 
		boxSize += stream.WriteIso639( this.language, "language"); 
		boxSize += stream.WriteUInt8( this.keywordCount, "keywordCount"); 

		for (int i = 0; i < keywordCount; i++)
		{
			boxSize += stream.WriteUInt8( this.keywordSize[i], "keywordSize"); 
			boxSize += stream.WriteBits((uint)(8*keywordSize[i] ),  this.keyword[i], "keyword"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 1; // reserved
		boxSize += 15; // language
		boxSize += 8; // keywordCount

		for (int i = 0; i < keywordCount; i++)
		{
			boxSize += 8; // keywordSize
			boxSize += (ulong)(8*keywordSize[i] ); // keyword
		}
		return boxSize;
	}
}

}
