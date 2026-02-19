using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class CuePayloadBox() extends Box ('payl') {
 boxstring cueText; 
 }
*/
public partial class CuePayloadBox : Box
{
	public const string TYPE = "payl";
	public override string DisplayName { get { return "CuePayloadBox"; } }

	protected BinaryUTF8String cueText; 
	public BinaryUTF8String CueText { get { return this.cueText; } set { this.cueText = value; } }

	public CuePayloadBox(): base(IsoStream.FromFourCC("payl"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.cueText, "cueText"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.cueText, "cueText"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(cueText); // cueText
		return boxSize;
	}
}

}
