using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class TextConfigBox() extends FullBox ('txtC', 0, 0) {
	utf8string text_config;
}
*/
public partial class TextConfigBox : FullBox
{
	public const string TYPE = "txtC";
	public override string DisplayName { get { return "TextConfigBox"; } }

	protected BinaryUTF8String text_config; 
	public BinaryUTF8String TextConfig { get { return this.text_config; } set { this.text_config = value; } }

	public TextConfigBox(): base(IsoStream.FromFourCC("txtC"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.text_config, "text_config"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.text_config, "text_config"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(text_config); // text_config
		return boxSize;
	}
}

}
