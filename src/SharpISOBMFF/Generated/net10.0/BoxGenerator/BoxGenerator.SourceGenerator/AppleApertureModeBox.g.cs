using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class AppleApertureModeBox() extends Box('apmd') {
 string mode;
 } 
*/
public partial class AppleApertureModeBox : Box
{
	public const string TYPE = "apmd";
	public override string DisplayName { get { return "AppleApertureModeBox"; } }

	protected BinaryUTF8String mode; 
	public BinaryUTF8String Mode { get { return this.mode; } set { this.mode = value; } }

	public AppleApertureModeBox(): base(IsoStream.FromFourCC("apmd"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.mode, "mode"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.mode, "mode"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(mode); // mode
		return boxSize;
	}
}

}
