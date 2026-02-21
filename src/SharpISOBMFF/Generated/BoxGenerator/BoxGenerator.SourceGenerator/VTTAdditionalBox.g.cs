using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class VTTAdditionalBox() extends Box ('vtta') {
 boxstring cueAdditionalText; 
 }
*/
public partial class VTTAdditionalBox : Box
{
	public const string TYPE = "vtta";
	public override string DisplayName { get { return "VTTAdditionalBox"; } }

	protected BinaryUTF8String cueAdditionalText; 
	public BinaryUTF8String CueAdditionalText { get { return this.cueAdditionalText; } set { this.cueAdditionalText = value; } }

	public VTTAdditionalBox(): base(IsoStream.FromFourCC("vtta"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.cueAdditionalText, "cueAdditionalText"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.cueAdditionalText, "cueAdditionalText"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(cueAdditionalText); // cueAdditionalText
		return boxSize;
	}
}

}
