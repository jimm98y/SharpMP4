using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class MetaDataExtensionsBox extends Box('exte') {
 Box extensions[];
}


*/
public partial class MetaDataExtensionsBox : Box
{
	public const string TYPE = "exte";
	public override string DisplayName { get { return "MetaDataExtensionsBox"; } }
	public IEnumerable<Box> Extensions { get { return this.children.OfType<Box>(); } }

	public MetaDataExtensionsBox(): base(IsoStream.FromFourCC("exte"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.extensions, "extensions"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.extensions, "extensions"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(extensions); // extensions
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
