using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class MetaDataSetupBox extends Box('setu') { // 'init' instead?
}


*/
public partial class MetaDataSetupBox : Box
{
	public const string TYPE = "setu";
	public override string DisplayName { get { return "MetaDataSetupBox"; } }

	public MetaDataSetupBox(): base(IsoStream.FromFourCC("setu"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		/*  'init' instead? */
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		/*  'init' instead? */
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		/*  'init' instead? */
		return boxSize;
	}
}

}
