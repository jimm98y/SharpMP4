using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class MetaDataInlineKeysPresentBox extends Box('keyi') {
unsigned int(8) inlineKeyValueBoxesPresent;
}


*/
public partial class MetaDataInlineKeysPresentBox : Box
{
	public const string TYPE = "keyi";
	public override string DisplayName { get { return "MetaDataInlineKeysPresentBox"; } }

	protected byte inlineKeyValueBoxesPresent; 
	public byte InlineKeyValueBoxesPresent { get { return this.inlineKeyValueBoxesPresent; } set { this.inlineKeyValueBoxesPresent = value; } }

	public MetaDataInlineKeysPresentBox(): base(IsoStream.FromFourCC("keyi"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.inlineKeyValueBoxesPresent, "inlineKeyValueBoxesPresent"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.inlineKeyValueBoxesPresent, "inlineKeyValueBoxesPresent"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // inlineKeyValueBoxesPresent
		return boxSize;
	}
}

}
