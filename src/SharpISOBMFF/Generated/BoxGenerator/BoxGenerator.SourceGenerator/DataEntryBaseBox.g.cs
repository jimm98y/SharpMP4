using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class DataEntryBaseBox(entry_type, bit(24) flags) 
extends FullBox(entry_type, version = 0, flags) { 

} 
*/
public partial class DataEntryBaseBox : FullBox
{
	public override string DisplayName { get { return "DataEntryBaseBox"; } }

	public DataEntryBaseBox(uint entry_type, uint flags): base(entry_type, 0, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		return boxSize;
	}
}

}
