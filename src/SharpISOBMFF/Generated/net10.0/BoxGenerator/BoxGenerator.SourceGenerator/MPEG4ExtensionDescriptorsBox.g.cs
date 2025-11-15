using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class MPEG4ExtensionDescriptorsBox extends Box('m4ds') {
	Descriptor Descr[0 .. 255];
}
*/
public partial class MPEG4ExtensionDescriptorsBox : Box
{
	public const string TYPE = "m4ds";
	public override string DisplayName { get { return "MPEG4ExtensionDescriptorsBox"; } }

	protected Descriptor[] Descr; 
	public Descriptor[] _Descr { get { return this.Descr; } set { this.Descr = value; } }

	public MPEG4ExtensionDescriptorsBox(): base(IsoStream.FromFourCC("m4ds"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadDescriptor(boxSize, readSize, this,  out this.Descr, "Descr"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteDescriptor( this.Descr, "Descr"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateDescriptorSize(Descr); // Descr
		return boxSize;
	}
}

}
