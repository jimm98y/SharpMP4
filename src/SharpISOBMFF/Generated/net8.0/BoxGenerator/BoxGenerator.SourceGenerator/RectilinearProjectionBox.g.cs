using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class RectilinearProjectionBox extends FullBox('rect', 0, 0) { 
// fields reserved for future use 
}
*/
public partial class RectilinearProjectionBox : FullBox
{
	public const string TYPE = "rect";
	public override string DisplayName { get { return "RectilinearProjectionBox"; } }

	public RectilinearProjectionBox(): base(IsoStream.FromFourCC("rect"), 0, 0)
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
