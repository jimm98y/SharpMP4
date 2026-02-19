using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class HalfEquirectangularProjectionBox extends FullBox('hequ', 0, 0) { 
// fields reserved for future use 
}
*/
public partial class HalfEquirectangularProjectionBox : FullBox
{
	public const string TYPE = "hequ";
	public override string DisplayName { get { return "HalfEquirectangularProjectionBox"; } }

	public HalfEquirectangularProjectionBox(): base(IsoStream.FromFourCC("hequ"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		/*  fields reserved for future use  */
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		/*  fields reserved for future use  */
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		/*  fields reserved for future use  */
		return boxSize;
	}
}

}
