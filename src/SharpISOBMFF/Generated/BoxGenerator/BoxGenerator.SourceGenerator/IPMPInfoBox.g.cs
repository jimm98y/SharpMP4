using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class IPMPInfoBox() extends FullBox('imif') {
 Descriptor descriptor[0 .. 255];
 }
*/
public partial class IPMPInfoBox : FullBox
{
	public const string TYPE = "imif";
	public override string DisplayName { get { return "IPMPInfoBox"; } }

	protected Descriptor[] descriptor; 
	public Descriptor[] Descriptor { get { return this.descriptor; } set { this.descriptor = value; } }

	public IPMPInfoBox(): base(IsoStream.FromFourCC("imif"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadDescriptor(boxSize, readSize, this,  out this.descriptor, "descriptor"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteDescriptor( this.descriptor, "descriptor"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateDescriptorSize(descriptor); // descriptor
		return boxSize;
	}
}

}
