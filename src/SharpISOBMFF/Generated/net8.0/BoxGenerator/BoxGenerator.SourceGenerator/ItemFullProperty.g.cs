using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class ItemFullProperty(property_type, version, flags) extends FullBox(property_type, version, flags){}
*/
public partial class ItemFullProperty : FullBox
{
	public override string DisplayName { get { return "ItemFullProperty"; } }

	public ItemFullProperty(uint property_type, byte version, uint flags): base(property_type, version, flags)
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
