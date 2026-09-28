using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SerialNumberBox() extends Box('slno') {
 string value;
}
*/
public partial class SerialNumberBox : Box
{
	public const string TYPE = "slno";
	public override string DisplayName { get { return "SerialNumberBox"; } }

	protected BinaryUTF8String value; 
	public BinaryUTF8String Value { get { return this.value; } set { this.value = value; } }

	public SerialNumberBox(): base(IsoStream.FromFourCC("slno"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.value, "value"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.value, "value"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(value); // value
		return boxSize;
	}
}

}
